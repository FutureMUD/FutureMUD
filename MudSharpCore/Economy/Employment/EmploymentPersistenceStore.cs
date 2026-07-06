using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.Vehicles;
using DbActionPlan = MudSharp.Models.EmploymentActionPlanRecord;
using DbActionStep = MudSharp.Models.EmploymentActionStepRecord;
using DbActiveTask = MudSharp.Models.EmploymentActiveTaskRecord;
using DbActiveTaskStepState = MudSharp.Models.EmploymentActiveTaskStepStateRecord;
using DbApplication = MudSharp.Models.EmploymentApplicationRecord;
using DbContract = MudSharp.Models.EmploymentContractRecord;
using DbConditionPredicate = MudSharp.Models.EmploymentConditionPredicateRecord;
using DbHostState = MudSharp.Models.EmploymentHostState;
using DbJobOpening = MudSharp.Models.EmploymentJobOpeningRecord;
using DbJobRequirement = MudSharp.Models.EmploymentJobOpeningRequirement;
using DbLedgerEntry = MudSharp.Models.EmploymentLedgerEntryRecord;
using DbManagerGoal = MudSharp.Models.EmploymentManagerGoalRecord;
using DbPayable = MudSharp.Models.EmploymentPayableRecord;
using DbRegisterEntry = MudSharp.Models.EmploymentRegisterEntryRecord;
using DbScheduledRule = MudSharp.Models.EmploymentScheduledTaskRuleRecord;
using DbScheduledRuleTemplate = MudSharp.Models.EmploymentScheduledRuleTemplateRecord;
using DbTaskCondition = MudSharp.Models.EmploymentTaskConditionRecord;
using DbBoard = MudSharp.Models.Board;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed class EmploymentPersistenceStore : IEmploymentPersistenceStore
{
	private enum JobRequirementType
	{
		Skill,
		Knowledge,
		Capability,
		Tag
	}

	private sealed class GetItemsByIdStepPayload
	{
		public GetItemsByIdStepPayload()
		{
		}

		public GetItemsByIdStepPayload(int quantity, long[] itemPrototypeIds, long[] sourceLocationIds,
			long[]? specificItemIds = null)
		{
			Quantity = quantity;
			ItemPrototypeIds = itemPrototypeIds;
			SourceLocationIds = sourceLocationIds;
			SpecificItemIds = specificItemIds ?? [];
		}

		public int Quantity { get; set; }
		public long[] ItemPrototypeIds { get; set; } = [];
		public long[] SpecificItemIds { get; set; } = [];
		[JsonPropertyName("ItemIds")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public long[]? LegacyItemIds { get; set; }
		public long[] SourceLocationIds { get; set; } = [];
		[JsonIgnore]
		public long[] ResolvedItemPrototypeIds => ItemPrototypeIds is { Length: > 0 } ? ItemPrototypeIds : LegacyItemIds ?? [];
	}

	private sealed record GetItemsByTagStepPayload(int Quantity, string TagName, long[] SourceLocationIds);

	private sealed record GetCommodityStepPayload(double RequiredWeight, string MaterialName, string? TagName,
		Dictionary<string, string> Characteristics, long[] SourceLocationIds);

	private sealed record ItemSelectorPayload(string Kind, long? Id, string? Text);

	private sealed record DeliverItemsStepPayload(long DestinationCellId, long? ContainerId = null,
		string? ContainerTag = null, ItemSelectorPayload? ContainerSelector = null);

	private sealed record ShopStockTransferStepPayload(long SourceShopId, long TargetShopId, long TargetMerchandiseId,
		long DestinationCellId, long? ContainerId = null, string? ContainerTag = null,
		ItemSelectorPayload? ContainerSelector = null);

	private sealed record AuctionLotListingStepPayload(long AuctionHouseId, ItemSelectorPayload ItemSelector,
		long ReserveCurrencyId, decimal ReserveAmount, long? BuyoutCurrencyId = null, decimal? BuyoutAmount = null,
		long? DurationTicks = null);

	private sealed record AuctionSettlementStepPayload(long AuctionHouseId, long? AssetId = null,
		string? AssetType = null, string? AssetName = null);

	private sealed record AuctionClaimStepPayload(long AuctionHouseId, long AssetId, string AssetType,
		string? AssetName = null);

	private sealed record ArenaEventAdministrationStepPayload(string Operation, long ArenaId,
		long? EventTypeId = null, string? EventTypeName = null, long? EventId = null, string? EventName = null,
		DateTime? ScheduledForUtc = null, string? TargetState = null, string? Reason = null);

	private sealed record BankAdministrationStepPayload(string Operation, long BankId, long? CurrencyId = null,
		decimal? Amount = null, string? AccountSelector = null, string? TargetStatus = null,
		long? SourceBranchId = null, long? DestinationBranchId = null, string? Reason = null);

	private sealed record LoadItemsStepPayload(long? ContainerId = null, string? ContainerTag = null,
		long? TargetLocationId = null, ItemSelectorPayload? ContainerSelector = null);

	private sealed record UnloadItemsStepPayload(long? ContainerId = null, string? ContainerTag = null,
		long? SourceLocationId = null, ItemSelectorPayload? ContainerSelector = null);

	private sealed record ReturnAssetStepPayload(long? ContainerId, string? ContainerTag, long DestinationCellId,
		long? DestinationContainerId, string? DestinationContainerTag, ItemSelectorPayload? ContainerSelector = null,
		ItemSelectorPayload? DestinationContainerSelector = null);

	private sealed record VehicleOperationStepPayload(long VehicleId, long? CargoSpaceId = null, string Operation = "cargo");

	private sealed record StableAnimalOperationStepPayload(string Operation, long? MountId = null, long? StableId = null,
		long? StayId = null, long? DestinationCellId = null, bool WaiveFees = false);

	private sealed record StableAdministrationStepPayload(string Operation, long StableId, long? StayId = null,
		long? AccountId = null, string? Note = null);

	private sealed record HotelAdministrationStepPayload(string Operation, long PropertyId, long? RoomCellId = null,
		long? LostPropertyBundleId = null, long? PatronId = null, string? PatronSelector = null, string? Note = null);

	private sealed record HospitalServiceStepPayload(long HospitalId, long RequestId);

	private sealed record HospitalSupplyPreparationStepPayload(long HospitalId, long RequestId);

	private sealed record HospitalAdministrationStepPayload(string Operation, long HospitalId, string? Note = null);

	private sealed record CataloguedActionShellPayload(string ActionKey, string Description, long? TargetLocationId,
		long? AmountCurrencyId = null, decimal? Amount = null, IReadOnlyList<long>? RouteStopIds = null);

	private sealed record PurchaseStepPayload(int Quantity, string MerchandiseSelector, string SupplierSelector,
		long CurrencyId, decimal? MaximumAmount, string? KeywordFilter,
		string TargetKind = "Merchandise", ItemSelectorPayload? ItemSelector = null,
		double? CommodityWeight = null, string? CommodityDescriptor = null);

	private sealed record ShopFloatAdjustmentStepPayload(bool FillRegister, long CurrencyId, decimal Amount,
		ItemSelectorPayload? RegisterSelector);

	private sealed record PhysicalFloatStepPayload(string Operation, long? CurrencyId, decimal? Amount,
		string TargetKind, ItemSelectorPayload? TargetSelector);

	private sealed record PriceChangeStepPayload(string Kind, string MerchandiseSelector, long? CurrencyId,
		decimal? ExactPrice, string MarketSelector, string CategorySelector, double SupplyImpact, double DemandImpact,
		double FlatPriceImpact, string InfluenceName, long? DurationTicks, string? UntilText);

	private sealed record ShopDealAdministrationStepPayload(string Operation, string Name, string DealType,
		string TargetType, string? TargetSelector, decimal PriceAdjustmentPercentage, int MinimumQuantity,
		string Applicability, long? EligibilityProgId, bool IsCumulative, string? Expiry, string? DealSelector);

	private sealed record JobOpeningDefinitionPayload(
		string Role,
		JobRequirementsPayload Requirements,
		CompensationTermsPayload Compensation,
		WorkSchedulePayload Schedule,
		EmploymentDurationPayload Duration,
		int MaxPositions,
		bool NpcApplicationsOnly,
		PaymentMethodPayload PaymentMethod,
		long Authority);

	private sealed record JobRequirementsPayload(string[] Skills, double[] SkillMinimums, string[] Knowledges,
		string[] Capabilities, string[] Tags);

	private sealed record CompensationTermsPayload(long? FixedCurrencyId, decimal? FixedAmount,
		string? MarketBindingType, decimal? MarketBindingValue, string Cadence, long? MinimumCurrencyId,
		decimal? MinimumAmount, string EmployerPaymentSource);

	private sealed record WorkSchedulePayload(string Description, long? StartsAtTicks, long? EndsAtTicks);

	private sealed record EmploymentDurationPayload(string DurationType, long? LengthTicks);

	private sealed record PaymentMethodPayload(string MethodKind, long? BankAccountId, long? PaymentItemId,
		string? PaymentItemType, string? Notes);

	private sealed record EmploymentCandidateProfilePayload(decimal ReservationWage, long? ReservationWageCurrencyId,
		Dictionary<string, double> Skills, List<string> Knowledges,
		List<EmploymentAICapability> Capabilities, List<string> Tags,
		List<PaymentMethodKind> AcceptedPaymentMethods);

	private sealed record JobOpeningAdministrationStepPayload(string Operation, long? OpeningId,
		JobOpeningDefinitionPayload? Definition, string Reason);

	private sealed record ShopStocktakeStepPayload(string Scope, string? MerchandiseSelector, string? MerchandiseName);

	private sealed record ScheduledRuleAdministrationStepPayload(string Operation, string RuleId, string RuleName,
		string Reason, string? ManualKey);

	private sealed record ActiveTaskAdministrationStepPayload(string Operation, string TaskId, string TaskName,
		long? EmployeeId, string? EmployeeName, string Reason);

	private sealed record ManagerGoalAdministrationStepPayload(string Operation, long GoalId, string GoalName,
		string Reason);

	private const string DeprecatedMarketPriceCompatibilityMarker = "Deprecated employment market-price action";

	private readonly IEmploymentHost _host;
	private readonly IFuturemud _gameworld;

	private EmploymentPersistenceStore(long stateId, IEmploymentHost host, IFuturemud gameworld)
	{
		StateId = stateId;
		_host = host;
		_gameworld = gameworld;
	}

	public long StateId { get; }

	public static IEmploymentHostState LoadOrCreate(IEmploymentHost host)
	{
		var gameworld = ResolveGameworld(host);
		if (gameworld is null)
		{
			return new EmploymentHostState(host);
		}

		using (new FMDB())
		{
			var context = FMDB.Context;
			var hostType = host.EmploymentHostType.ToString();
			var state = context.EmploymentHostStates
			                   .FirstOrDefault(x => x.HostType == hostType && x.HostId == host.Id);
			if (state is null)
			{
				state = CreateHostState(context, host);
			}

			var board = ResolveBoard(context, gameworld, state);
			var store = new EmploymentPersistenceStore(state.Id, host, gameworld);
			return store.LoadState(context, host, board);
		}
	}

	public static bool HasScheduledRules(IEmploymentHost host)
	{
		var gameworld = ResolveGameworld(host);
		if (gameworld is null)
		{
			return false;
		}

		using (new FMDB())
		{
			var hostType = host.EmploymentHostType.ToString();
			var stateId = FMDB.Context.EmploymentHostStates
			                  .Where(x => x.HostType == hostType && x.HostId == host.Id)
			                  .Select(x => (long?)x.Id)
			                  .FirstOrDefault();
			return stateId.HasValue &&
			       FMDB.Context.EmploymentScheduledTaskRules.Any(x => x.EmploymentHostStateId == stateId.Value);
		}
	}

	public void SaveContract(EmploymentContract contract)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentContracts
			                    .FirstOrDefault(x => x.RuntimeId == contract.Id && x.EmploymentHostStateId == StateId);
			if (dbitem is null)
			{
				context.EmploymentContracts.Add(ToRecord(contract));
			}
			else
			{
				WriteContractRecord(dbitem, contract);
			}

			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveContractEnded(EmploymentContract contract)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentContracts
			                    .FirstOrDefault(x => x.RuntimeId == contract.Id && x.EmploymentHostStateId == StateId);
			if (dbitem is null)
			{
				context.EmploymentContracts.Add(ToRecord(contract));
			}
			else
			{
				WriteContractRecord(dbitem, contract);
			}

			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveContractAuthority(EmploymentContract contract)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentContracts
			                    .FirstOrDefault(x => x.RuntimeId == contract.Id && x.EmploymentHostStateId == StateId);
			if (dbitem is null)
			{
				context.EmploymentContracts.Add(ToRecord(contract));
			}
			else
			{
				WriteContractRecord(dbitem, contract);
			}

			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveJobOpening(JobOpening opening)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentJobOpenings
			                    .Include(x => x.Requirements)
			                    .FirstOrDefault(x =>
				                    x.RuntimeId == opening.Id &&
				                    x.EmploymentHostStateId == StateId);
			if (dbitem is null)
			{
				dbitem = ToRecord(opening);
				context.EmploymentJobOpenings.Add(dbitem);
			}
			else
			{
				dbitem.Role = (int)opening.Role;
				dbitem.Status = (int)opening.Status;
				dbitem.MaxPositions = opening.MaxPositions;
				dbitem.NpcApplicationsOnly = opening.NpcApplicationsOnly;
				dbitem.Authority = (long)opening.Authority.Authorities;
				dbitem.RevisionNumber = opening.RevisionNumber;
				WriteTerms(dbitem, opening.Compensation, opening.Schedule, opening.Duration, opening.PaymentMethod);
				context.EmploymentJobOpeningRequirements.RemoveRange(dbitem.Requirements);
				dbitem.Requirements.Clear();
			}

			foreach (var requirement in ToRequirementRecords(opening))
			{
				dbitem.Requirements.Add(requirement);
			}

			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveApplication(EmploymentApplication application)
	{
		WithContext(context =>
		{
			var opening = context.EmploymentJobOpenings
			                     .FirstOrDefault(x =>
				                     x.EmploymentHostStateId == StateId &&
				                     x.RuntimeId == application.Opening.Id);
			if (opening is null)
			{
				if (application.Opening is JobOpening concrete)
				{
					SaveJobOpening(concrete);
					opening = context.EmploymentJobOpenings
					                 .First(x =>
						                 x.EmploymentHostStateId == StateId &&
						                 x.RuntimeId == application.Opening.Id);
				}
				else
				{
					throw new InvalidOperationException("Cannot persist an application whose opening has not been saved.");
				}
			}

			var existing = context.EmploymentApplications.FirstOrDefault(x =>
				    x.EmploymentJobOpeningId == opening.Id &&
				    x.RuntimeId == application.Id);
			if (existing is not null)
			{
				existing.Status = (int)application.Status;
				existing.DecisionReason = application.DecisionReason;
				existing.OfferedOpeningRevision = application.OfferedOpeningRevision;
				existing.CandidateProfileJson = SerializeCandidateProfile(application.CandidateProfile);
				Touch(context);
				context.SaveChanges();
				return;
			}

			context.EmploymentApplications.Add(new DbApplication
			{
				RuntimeId = application.Id,
				EmploymentJobOpeningId = opening.Id,
				CandidateId = CharacterInstanceIdentityComparer.IdentityId(application.Candidate),
				AppliedAt = application.AppliedAt.UtcDateTime,
				Status = (int)application.Status,
				DecisionReason = application.DecisionReason,
				OfferedOpeningRevision = application.OfferedOpeningRevision,
				CandidateProfileJson = SerializeCandidateProfile(application.CandidateProfile)
			});
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SavePayable(EmploymentPayable payable)
	{
		WithContext(context =>
		{
			if (context.EmploymentPayables.Any(x => x.RuntimeId == payable.Id && x.EmploymentHostStateId == StateId))
			{
				return;
			}

			context.EmploymentPayables.Add(ToRecord(payable));
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SavePayableState(EmploymentPayable payable)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentPayables
			                    .FirstOrDefault(x => x.RuntimeId == payable.Id && x.EmploymentHostStateId == StateId);
			if (dbitem is null)
			{
				context.EmploymentPayables.Add(ToRecord(payable));
			}
			else
			{
				dbitem.Status = (int)payable.Status;
				dbitem.SettledAt = payable.SettledAt?.UtcDateTime;
				dbitem.ClaimedAt = payable.ClaimedAt?.UtcDateTime;
				dbitem.SettlementNote = payable.SettlementNote;
			}

			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveRegisterEntry(EmploymentRegisterEntry entry)
	{
		WithContext(context =>
		{
			context.EmploymentRegisterEntries.Add(new DbRegisterEntry
			{
				EmploymentHostStateId = StateId,
				CorrelationId = entry.CorrelationId.ToString("D"),
				EntryType = (int)entry.EntryType,
				ActorId = entry.Actor is null ? null : CharacterInstanceIdentityComparer.IdentityId(entry.Actor),
				Description = entry.Description,
				RecordedAt = entry.RecordedAt.UtcDateTime
			});
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveLedgerEntry(EmploymentLedgerEntry entry)
	{
		WithContext(context =>
		{
			context.EmploymentLedgerEntries.Add(new DbLedgerEntry
			{
				EmploymentHostStateId = StateId,
				CorrelationId = entry.CorrelationId.ToString("D"),
				EntryType = (int)entry.EntryType,
				ActorId = entry.Actor is null ? null : CharacterInstanceIdentityComparer.IdentityId(entry.Actor),
				AmountCurrencyId = entry.Amount?.Currency.Id,
				Amount = entry.Amount?.Amount,
				Description = entry.Description,
				RecordedAt = entry.RecordedAt.UtcDateTime
			});
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveScheduledRule(EmploymentScheduledTaskRule rule)
	{
		WithContext(context =>
		{
			if (context.EmploymentScheduledTaskRules.Any(x => x.PublicId == rule.Id.ToString("D")))
			{
				return;
			}

			var planId = SaveActionPlan(context, $"{rule.Name} action plan", rule.ActionPlan);
			var dbitem = new DbScheduledRule
			{
				PublicId = rule.Id.ToString("D"),
				EmploymentHostStateId = StateId,
				Name = rule.Name,
				IdempotencyKey = rule.IdempotencyKey,
				EmploymentActionPlanId = planId,
				ExpressionJson = SerializeExpression(rule.ConditionExpression),
				Status = (int)rule.Status,
				CooldownTicks = rule.Cooldown.Ticks,
				LastSpawnedAt = rule.LastSpawnedAt?.UtcDateTime
			};
			context.EmploymentScheduledTaskRules.Add(dbitem);
			AddConditions(dbitem.Conditions, rule.Conditions);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveScheduledRuleState(EmploymentScheduledTaskRule rule)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentScheduledTaskRules
			                    .FirstOrDefault(x => x.PublicId == rule.Id.ToString("D"));
			if (dbitem is null)
			{
				SaveScheduledRule(rule);
				return;
			}

			dbitem.LastSpawnedAt = rule.LastSpawnedAt?.UtcDateTime;
			dbitem.Status = (int)rule.Status;
			Touch(context);
			context.SaveChanges();
		});
	}

	public void DeleteScheduledRule(EmploymentScheduledTaskRule rule)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentScheduledTaskRules
			                    .Include(x => x.Conditions)
			                    .FirstOrDefault(x => x.PublicId == rule.Id.ToString("D"));
			if (dbitem is null)
			{
				return;
			}

			context.EmploymentScheduledTaskRules.Remove(dbitem);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveConditionPredicate(EmploymentConditionPredicate predicate)
	{
		WithContext(context =>
		{
			if (context.EmploymentConditionPredicates.Any(x => x.PublicId == predicate.Id.ToString("D")))
			{
				return;
			}

			var dbitem = new DbConditionPredicate
			{
				PublicId = predicate.Id.ToString("D"),
				EmploymentHostStateId = StateId,
				Name = predicate.Name,
				ExpressionJson = SerializeExpression(predicate.ConditionExpression)
			};
			context.EmploymentConditionPredicates.Add(dbitem);
			AddConditions(dbitem.Conditions, predicate.Conditions);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void DeleteConditionPredicate(EmploymentConditionPredicate predicate)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentConditionPredicates
			                    .Include(x => x.Conditions)
			                    .FirstOrDefault(x => x.PublicId == predicate.Id.ToString("D"));
			if (dbitem is null)
			{
				return;
			}

			context.EmploymentConditionPredicates.Remove(dbitem);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveScheduledRuleTemplate(EmploymentScheduledRuleTemplate template)
	{
		WithContext(context =>
		{
			if (context.EmploymentScheduledRuleTemplates.Any(x => x.PublicId == template.Id.ToString("D")))
			{
				return;
			}

			var planId = SaveActionPlan(context, $"{template.Name} template action plan", template.ActionPlan);
			var dbitem = new DbScheduledRuleTemplate
			{
				PublicId = template.Id.ToString("D"),
				EmploymentHostStateId = StateId,
				Name = template.Name,
				IdempotencyKeyPattern = template.IdempotencyKeyPattern,
				EmploymentActionPlanId = planId,
				ExpressionJson = SerializeExpression(template.ConditionExpression),
				CooldownTicks = template.Cooldown.Ticks
			};
			context.EmploymentScheduledRuleTemplates.Add(dbitem);
			AddConditions(dbitem.Conditions, template.Conditions);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void DeleteScheduledRuleTemplate(EmploymentScheduledRuleTemplate template)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentScheduledRuleTemplates
			                    .Include(x => x.Conditions)
			                    .FirstOrDefault(x => x.PublicId == template.Id.ToString("D"));
			if (dbitem is null)
			{
				return;
			}

			context.EmploymentScheduledRuleTemplates.Remove(dbitem);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveActiveTask(EmploymentActiveTask task)
	{
		WithContext(context =>
		{
			if (context.EmploymentActiveTasks.Any(x => x.PublicId == task.Id.ToString("D")))
			{
				return;
			}

			var planId = SaveActionPlan(context, $"{task.Name} action plan", task.ActionPlan);
			var dbitem = new DbActiveTask
			{
				PublicId = task.Id.ToString("D"),
				EmploymentHostStateId = StateId,
				Name = task.Name,
				EmploymentActionPlanId = planId,
				Status = (int)task.Status,
				AssignedEmployeeId = task.AssignedEmployee is null
					? null
					: CharacterInstanceIdentityComparer.IdentityId(task.AssignedEmployee),
				BlockedReason = task.BlockedReason,
				CorrelationId = task.CorrelationId.ToString("D"),
				IdempotencyKey = task.IdempotencyKey
			};
			context.EmploymentActiveTasks.Add(dbitem);
			AddStepStates(dbitem.StepStates, task.StepStates, task.StepOperationalStates);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveActiveTaskState(EmploymentActiveTask task)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentActiveTasks
			                    .Include(x => x.StepStates)
			                    .FirstOrDefault(x => x.PublicId == task.Id.ToString("D"));
			if (dbitem is null)
			{
				SaveActiveTask(task);
				return;
			}

			dbitem.Status = (int)task.Status;
			dbitem.AssignedEmployeeId = task.AssignedEmployee is null
				? null
				: CharacterInstanceIdentityComparer.IdentityId(task.AssignedEmployee);
			dbitem.BlockedReason = task.BlockedReason;
			context.EmploymentActiveTaskStepStates.RemoveRange(dbitem.StepStates);
			AddStepStates(dbitem.StepStates, task.StepStates, task.StepOperationalStates);
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveManagerGoal(ManagerGoal goal)
	{
		WithContext(context =>
		{
			if (context.EmploymentManagerGoals.Any(x => x.RuntimeId == goal.Id && x.EmploymentHostStateId == StateId))
			{
				return;
			}

			var planId = goal.Configuration.ActionPlan is null
				? (long?)null
				: SaveActionPlan(context, $"{goal.Configuration.Description} action plan", goal.Configuration.ActionPlan);
			var dbitem = new DbManagerGoal
			{
				RuntimeId = goal.Id,
				EmploymentHostStateId = StateId,
				GoalType = (int)goal.GoalType,
				RequiredAuthority = (long)goal.RequiredAuthority.Authorities,
				Status = (int)goal.Status,
				ConfigurationDescription = goal.Configuration.Description,
				EmploymentActionPlanId = planId,
				Priority = goal.Priority,
				EvaluationCadenceTicks = goal.EvaluationCadence.Ticks,
				LastEvaluatedAt = goal.LastEvaluatedAt?.UtcDateTime,
				LastEvaluationResult = goal.LastEvaluationResult,
				CorrelationId = goal.CorrelationId.ToString("D")
			};
			context.EmploymentManagerGoals.Add(dbitem);
			AddConditions(dbitem.Conditions, goal.Configuration.Conditions ?? Array.Empty<IEmploymentTaskCondition>());
			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveManagerGoalState(ManagerGoal goal)
	{
		WithContext(context =>
		{
			var dbitem = context.EmploymentManagerGoals
			                    .FirstOrDefault(x => x.RuntimeId == goal.Id && x.EmploymentHostStateId == StateId);
			if (dbitem is null)
			{
				SaveManagerGoal(goal);
				return;
			}

			dbitem.Status = (int)goal.Status;
			dbitem.LastEvaluatedAt = goal.LastEvaluatedAt?.UtcDateTime;
			dbitem.LastEvaluationResult = goal.LastEvaluationResult;
			Touch(context);
			context.SaveChanges();
		});
	}

	private EmploymentHostState LoadState(FuturemudDatabaseContext context, IEmploymentHost host, IBoard board)
	{
		var actionPlans = context.EmploymentActionPlans
		                         .Include(x => x.Steps)
		                         .Where(x => x.EmploymentHostStateId == StateId)
		                         .AsNoTracking()
		                         .ToList();
		var planLookup = actionPlans.ToDictionary(x => x.Id);
		var planCache = new Dictionary<long, EmploymentActionPlan>();
		EmploymentActionPlan RuntimePlan(long id)
		{
			if (planCache.TryGetValue(id, out var existing))
			{
				return existing;
			}

			if (!planLookup.TryGetValue(id, out var record))
			{
				existing = new EmploymentActionPlan(Array.Empty<IEmploymentActionStep>());
				planCache[id] = existing;
				return existing;
			}

			existing = new EmploymentActionPlan(record.Steps
			                                         .OrderBy(x => x.SortOrder)
			                                         .Select(x => ToActionStep(x))
			                                         .OfType<IEmploymentActionStep>());
			planCache[id] = existing;
			return existing;
		}

		var contracts = context.EmploymentContracts
		                       .Where(x => x.EmploymentHostStateId == StateId)
		                       .AsNoTracking()
		                       .ToList()
		                       .Select(x => ToContract(x))
		                       .OfType<IEmploymentContract>()
		                       .ToList();
		var openingRecords = context.EmploymentJobOpenings
		                            .Include(x => x.Requirements)
		                            .Where(x => x.EmploymentHostStateId == StateId)
		                            .AsNoTracking()
		                            .ToList();
		var openingsByRecordId = openingRecords
		                         .Select(x => (Record: x, Opening: ToJobOpening(x)))
		                         .Where(x => x.Opening is not null)
		                         .ToDictionary(x => x.Record.Id, x => x.Opening!);
		var openings = openingsByRecordId.Values.ToList();
		var applications = openingRecords
		                   .SelectMany(x => context.EmploymentApplications
		                                           .Where(y => y.EmploymentJobOpeningId == x.Id)
		                                           .AsNoTracking()
		                                           .ToList())
		                   .Select(x => ToApplication(x, openingsByRecordId))
		                   .OfType<IEmploymentApplication>()
		                   .ToList();
		var registerEntries = context.EmploymentRegisterEntries
		                             .Where(x => x.EmploymentHostStateId == StateId)
		                             .OrderBy(x => x.Id)
		                             .AsNoTracking()
		                             .ToList()
		                             .Select(ToRegisterEntry)
		                             .ToList();
		var ledgerEntries = context.EmploymentLedgerEntries
		                           .Where(x => x.EmploymentHostStateId == StateId)
		                           .OrderBy(x => x.Id)
		                           .AsNoTracking()
		                           .ToList()
		                           .Select(ToLedgerEntry)
		                           .ToList();
		var payables = context.EmploymentPayables
		                      .Where(x => x.EmploymentHostStateId == StateId)
		                      .OrderBy(x => x.Id)
		                      .AsNoTracking()
		                      .ToList()
		                      .Select(ToPayable)
		                      .OfType<IEmploymentPayable>()
		                      .ToList();
		var conditionPredicates = context.EmploymentConditionPredicates
		                                 .Include(x => x.Conditions)
		                                 .Where(x => x.EmploymentHostStateId == StateId)
		                                 .AsNoTracking()
		                                 .ToList()
		                                 .Select(ToConditionPredicate)
		                                 .OfType<IEmploymentConditionPredicate>()
		                                 .ToList();
		var scheduledRuleTemplates = context.EmploymentScheduledRuleTemplates
		                                    .Include(x => x.Conditions)
		                                    .Where(x => x.EmploymentHostStateId == StateId)
		                                    .AsNoTracking()
		                                    .ToList()
		                                    .Select(x => ToScheduledRuleTemplate(x,
			                                    RuntimePlan(x.EmploymentActionPlanId)))
		                                    .OfType<IEmploymentScheduledRuleTemplate>()
		                                    .ToList();
		var scheduledRules = context.EmploymentScheduledTaskRules
		                            .Include(x => x.Conditions)
		                            .Where(x => x.EmploymentHostStateId == StateId)
		                            .AsNoTracking()
		                            .ToList()
		                            .Select(x => ToScheduledRule(x, RuntimePlan(x.EmploymentActionPlanId)))
		                            .OfType<IEmploymentScheduledTaskRule>()
		                            .ToList();
		var activeTasks = context.EmploymentActiveTasks
		                         .Include(x => x.StepStates)
		                         .Where(x => x.EmploymentHostStateId == StateId)
		                         .AsNoTracking()
		                         .ToList()
		                         .Select(x => ToActiveTask(x, RuntimePlan(x.EmploymentActionPlanId)))
		                         .OfType<IEmploymentActiveTask>()
		                         .ToList();
		var managerGoals = context.EmploymentManagerGoals
		                          .Include(x => x.Conditions)
		                          .Where(x => x.EmploymentHostStateId == StateId)
		                          .AsNoTracking()
		                          .ToList()
		                          .Select(x => ToManagerGoal(x,
			                          x.EmploymentActionPlanId.HasValue ? RuntimePlan(x.EmploymentActionPlanId.Value) : null))
		                          .OfType<IManagerGoal>()
		                          .ToList();

		var hostState = new EmploymentHostState(
			host,
			board,
			this,
			contracts,
			openings,
			applications,
			ledgerEntries,
			payables,
			registerEntries,
			scheduledRules,
			activeTasks,
			managerGoals,
			conditionPredicates,
			scheduledRuleTemplates);
		ApplyDeprecatedMarketPriceCompatibility(context, hostState, registerEntries);
		return hostState;
	}

	private void ApplyDeprecatedMarketPriceCompatibility(FuturemudDatabaseContext context, EmploymentHostState state,
		List<EmploymentRegisterEntry> registerEntries)
	{
		var newEntries = new List<EmploymentRegisterEntry>();

		foreach (var rule in state.TaskBoard.ScheduledRules.OfType<EmploymentScheduledTaskRule>())
		{
			var deprecatedSteps = DeprecatedMarketPriceSteps(rule.ActionPlan).ToList();
			if (!deprecatedSteps.Any() || rule.Status == EmploymentScheduledRuleStatus.Paused)
			{
				continue;
			}

			var reason = DeprecatedMarketPriceCompatibilityReport($"scheduled rule {rule.Name}", deprecatedSteps);
			rule.Pause();
			var dbitem = context.EmploymentScheduledTaskRules
			                    .FirstOrDefault(x => x.PublicId == rule.Id.ToString("D"));
			if (dbitem is not null)
			{
				dbitem.Status = (int)EmploymentScheduledRuleStatus.Paused;
			}

			newEntries.Add(AddCompatibilityRegisterEntry(context, EmploymentRegisterEntryType.ScheduledRulePaused,
				reason, rule.Id));
		}

		foreach (var task in state.TaskBoard.ActiveTasks.OfType<EmploymentActiveTask>())
		{
			var deprecatedSteps = DeprecatedMarketPriceSteps(task.ActionPlan).ToList();
			if (!deprecatedSteps.Any() ||
			    task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed ||
			    task.Status == EmploymentTaskStatus.Blocked &&
			    task.BlockedReason?.Contains(DeprecatedMarketPriceCompatibilityMarker,
				    StringComparison.InvariantCultureIgnoreCase) == true)
			{
				continue;
			}

			var reason = DeprecatedMarketPriceCompatibilityReport($"active task {task.Name}", deprecatedSteps);
			if (task.Status == EmploymentTaskStatus.Blocked && !string.IsNullOrWhiteSpace(task.BlockedReason))
			{
				reason = $"{reason} Previous blocked reason: {task.BlockedReason}";
			}

			task.BlockForCompatibilityLoad(reason);
			var dbitem = context.EmploymentActiveTasks
			                    .FirstOrDefault(x => x.PublicId == task.Id.ToString("D"));
			if (dbitem is not null)
			{
				dbitem.Status = (int)EmploymentTaskStatus.Blocked;
				dbitem.BlockedReason = reason;
			}

			newEntries.Add(AddCompatibilityRegisterEntry(context, EmploymentRegisterEntryType.ActiveTaskBlocked,
				reason, task.CorrelationId));
		}

		foreach (var goal in state.ManagerGoalBoard.Goals.OfType<ManagerGoal>())
		{
			var deprecatedSteps = DeprecatedMarketPriceSteps(goal.Configuration.ActionPlan).ToList();
			if (!deprecatedSteps.Any() ||
			    goal.Status is ManagerGoalStatus.Completed or ManagerGoalStatus.Cancelled ||
			    goal.Status == ManagerGoalStatus.Blocked &&
			    goal.LastEvaluationResult?.Contains(DeprecatedMarketPriceCompatibilityMarker,
				    StringComparison.InvariantCultureIgnoreCase) == true)
			{
				continue;
			}

			var now = EmploymentClock.CurrentInstant(_host);
			var reason = DeprecatedMarketPriceCompatibilityReport($"manager goal #{goal.Id:N0} ({goal.GoalType.DescribeEnum()})",
				deprecatedSteps);
			if (!string.IsNullOrWhiteSpace(goal.LastEvaluationResult))
			{
				reason = $"{reason} Previous goal result: {goal.LastEvaluationResult}";
			}

			goal.BlockForCompatibilityLoad(now, reason);
			var dbitem = context.EmploymentManagerGoals
			                    .FirstOrDefault(x => x.RuntimeId == goal.Id && x.EmploymentHostStateId == StateId);
			if (dbitem is not null)
			{
				dbitem.Status = (int)ManagerGoalStatus.Blocked;
				dbitem.LastEvaluatedAt = now.UtcDateTime;
				dbitem.LastEvaluationResult = reason;
			}

			newEntries.Add(AddCompatibilityRegisterEntry(context, EmploymentRegisterEntryType.AuditActionRecorded,
				reason, goal.CorrelationId));
		}

		if (!newEntries.Any())
		{
			return;
		}

		registerEntries.AddRange(newEntries);
		if (state.EmploymentRegister is EmploymentRegister register)
		{
			register.AddForCompatibilityLoad(newEntries);
		}

		Touch(context);
		context.SaveChanges();
	}

	private static IEnumerable<DeprecatedMarketPriceChangeActionStep> DeprecatedMarketPriceSteps(
		EmploymentActionPlan? actionPlan)
	{
		return actionPlan?.Steps.OfType<DeprecatedMarketPriceChangeActionStep>() ??
		       Enumerable.Empty<DeprecatedMarketPriceChangeActionStep>();
	}

	private string DeprecatedMarketPriceCompatibilityReport(string owner,
		IReadOnlyCollection<DeprecatedMarketPriceChangeActionStep> steps)
	{
		var details = steps
		              .Select((step, index) => DescribeDeprecatedMarketPriceStep(step, index + 1));
		return $"{DeprecatedMarketPriceCompatibilityMarker} requires builder review. Host {_host.EmploymentHostName}; {owner}; {string.Join("; ", details)}";
	}

	private static string DescribeDeprecatedMarketPriceStep(DeprecatedMarketPriceChangeActionStep step, int number)
	{
		var payload = TryDeserializeActionPayload<PriceChangeStepPayload>(step.OriginalPayload);
		if (payload is null)
		{
			return $"step {number}: original payload unavailable; diagnostic {step.Diagnostic}";
		}

		return $"step {number}: market {DisplayCompatibilityValue(payload.MarketSelector)}, category {DisplayCompatibilityValue(payload.CategorySelector)}, impacts supply {payload.SupplyImpact}, demand {payload.DemandImpact}, flat {payload.FlatPriceImpact}, influence {DisplayCompatibilityValue(payload.InfluenceName)}, expiry {DeprecatedMarketPriceExpiry(payload)}";
	}

	private static string DeprecatedMarketPriceExpiry(PriceChangeStepPayload payload)
	{
		if (!string.IsNullOrWhiteSpace(payload.UntilText))
		{
			return payload.UntilText.Trim();
		}

		return payload.DurationTicks.HasValue ? TimeSpan.FromTicks(payload.DurationTicks.Value).ToString() : "none";
	}

	private static string DisplayCompatibilityValue(string? text)
	{
		return string.IsNullOrWhiteSpace(text) ? "(not recorded)" : text.Trim();
	}

	private EmploymentRegisterEntry AddCompatibilityRegisterEntry(FuturemudDatabaseContext context,
		EmploymentRegisterEntryType entryType, string description, Guid correlationId)
	{
		var entry = new EmploymentRegisterEntry(
			correlationId,
			entryType,
			_host,
			null,
			description,
			EmploymentClock.CurrentInstant(_host));
		context.EmploymentRegisterEntries.Add(new DbRegisterEntry
		{
			EmploymentHostStateId = StateId,
			CorrelationId = entry.CorrelationId.ToString("D"),
			EntryType = (int)entry.EntryType,
			ActorId = null,
			Description = entry.Description,
			RecordedAt = entry.RecordedAt.UtcDateTime
		});
		return entry;
	}

	private static IFuturemud? ResolveGameworld(IEmploymentHost host)
	{
		return host switch
		{
			IHaveFuturemud have => have.Gameworld,
			IHotel { Property: IHaveFuturemud property } => property.Gameworld,
			_ => null
		};
	}

	private static DbHostState CreateHostState(FuturemudDatabaseContext context, IEmploymentHost host)
	{
		var now = DateTime.UtcNow;
		var dbBoard = new DbBoard
		{
			Name = TruncateBoardName($"{host.EmploymentHostName} Staff Board"),
			ShowOnLogin = false,
			CalendarId = ResolveCalendarId(host)
		};
		context.Boards.Add(dbBoard);
		context.SaveChanges();

		var state = new DbHostState
		{
			HostType = host.EmploymentHostType.ToString(),
			HostId = host.Id,
			BoardId = dbBoard.Id,
			CreatedAt = now,
			LastUpdatedAt = now
		};
		context.EmploymentHostStates.Add(state);
		context.SaveChanges();
		return state;
	}

	private static string TruncateBoardName(string text)
	{
		text = string.IsNullOrWhiteSpace(text) ? "Staff Board" : text.Trim();
		return text.Length <= 45 ? text : text[..45];
	}

	private static long? ResolveCalendarId(IEmploymentHost host)
	{
		return host is IClan clan ? clan.Calendar?.Id : ResolveEconomicZone(host)?.FinancialPeriodReferenceCalendar?.Id;
	}

	private static IEconomicZone? ResolveEconomicZone(IEmploymentHost host)
	{
		return host switch
		{
			IShop shop => shop.EconomicZone,
			IAuctionHouse auctionHouse => auctionHouse.EconomicZone,
			ICombatArena arena => arena.EconomicZone,
			IBank bank => bank.EconomicZone,
			IStable stable => stable.EconomicZone,
			IHotel hotel => hotel.EconomicZone,
			IClan clan => ResolveClanEconomicZone(clan),
			_ => null
		};
	}

	private static IEconomicZone? ResolveClanEconomicZone(IClan clan)
	{
		if (clan is not IHaveFuturemud haveFuturemud)
		{
			return null;
		}

		var gameworld = haveFuturemud.Gameworld;
		if (gameworld is null)
		{
			return null;
		}

		return (gameworld.EconomicZones ?? Enumerable.Empty<IEconomicZone>())
		       .FirstOrDefault(x => x.ControllingClan == clan) ??
		       (gameworld.Properties ?? Enumerable.Empty<IProperty>())
		       .FirstOrDefault(x =>
			       x.PropertyOwners.Any(y =>
				       y.Owner is IClan ownerClan &&
				       ownerClan.Id == clan.Id))?.EconomicZone;
	}

	private static IBoard ResolveBoard(FuturemudDatabaseContext context, IFuturemud gameworld, DbHostState state)
	{
		var existing = gameworld.Boards.Get(state.BoardId);
		if (existing is not null)
		{
			return existing;
		}

		var dbBoard = context.Boards
		                     .Include(x => x.BoardPosts)
		                     .First(x => x.Id == state.BoardId);
		var board = new Board(dbBoard, gameworld);
		gameworld.Add(board);
		return board;
	}

	private static ICurrency? ResolveHostCurrency(IEmploymentHost host)
	{
		return host switch
		{
			IShop shop => shop.Currency,
			IAuctionHouse auction => auction.EconomicZone.Currency,
			ICombatArena arena => arena.Currency,
			IBank bank => bank.PrimaryCurrency,
			IStable stable => stable.Currency,
			IHotel hotel => hotel.Currency,
			IClan clan => clan.ClanBankAccount?.Currency ?? ResolveContractCurrency(host),
			_ => null
		};
	}

	private static ICurrency? ResolveContractCurrency(IEmploymentHost host)
	{
		return host.EmploymentContracts
		           .Select(x => x.Compensation.FixedRate?.Currency ?? x.Compensation.MinimumEffectivePay?.Currency)
		           .FirstOrDefault(x => x is not null);
	}

	private DbContract ToRecord(EmploymentContract contract)
	{
		var record = new DbContract
		{
			RuntimeId = contract.Id,
			EmploymentHostStateId = StateId
		};
		WriteContractRecord(record, contract);
		return record;
	}

	private static void WriteContractRecord(DbContract record, EmploymentContract contract)
	{
		record.EmployeeId = CharacterInstanceIdentityComparer.IdentityId(contract.Employee);
		record.Role = (int)contract.Role;
		record.Status = (int)contract.Status;
		record.Authority = (long)contract.Authority.Authorities;
		record.StartedAt = contract.StartedAt.UtcDateTime;
		record.EndsAt = contract.EndsAt?.UtcDateTime;
		record.EndReason = contract.EndReason.HasValue ? (int)contract.EndReason.Value : null;
		record.OriginOpeningId = contract.OriginOpeningId;
		record.OriginApplicationId = contract.OriginApplicationId;
		WriteTerms(record, contract.Compensation, contract.Schedule, contract.Duration, contract.PaymentMethod);
	}

	private DbJobOpening ToRecord(JobOpening opening)
	{
		var record = new DbJobOpening
		{
			RuntimeId = opening.Id,
			EmploymentHostStateId = StateId,
			Role = (int)opening.Role,
			Status = (int)opening.Status,
			MaxPositions = opening.MaxPositions,
			NpcApplicationsOnly = opening.NpcApplicationsOnly,
			Authority = (long)opening.Authority.Authorities,
			RevisionNumber = opening.RevisionNumber
		};
		WriteTerms(record, opening.Compensation, opening.Schedule, opening.Duration, opening.PaymentMethod);
		return record;
	}

	private DbPayable ToRecord(EmploymentPayable payable)
	{
		return new DbPayable
		{
			RuntimeId = payable.Id,
			EmploymentHostStateId = StateId,
			CorrelationId = payable.CorrelationId.ToString("D"),
			ContractRuntimeId = payable.ContractId,
			EmployeeId = payable.EmployeeId,
			EmployeeName = payable.EmployeeName,
			Role = (int)payable.Role,
			AmountCurrencyId = payable.Amount.Currency.Id,
			Amount = payable.Amount.Amount,
			PayCadence = (int)payable.Cadence,
			PaymentMethodKind = (int)payable.PaymentMethod.MethodKind,
			PaymentBankAccountId = payable.PaymentMethod.BankAccount?.Id,
			PaymentItemId = payable.PaymentMethod.PaymentItemPrototype?.Id,
			PaymentItemType = payable.PaymentMethod.PaymentItemPrototype?.FrameworkItemType,
			PaymentNotes = payable.PaymentMethod.Notes,
			PayPeriodStart = payable.PayPeriodStart.UtcDateTime,
			PayPeriodEnd = payable.PayPeriodEnd.UtcDateTime,
			DueAt = payable.DueAt.UtcDateTime,
			AccruedAt = payable.AccruedAt.UtcDateTime,
			Status = (int)payable.Status,
			SettledAt = payable.SettledAt?.UtcDateTime,
			ClaimedAt = payable.ClaimedAt?.UtcDateTime,
			SettlementNote = payable.SettlementNote
		};
	}

	private static void WriteTerms(DbContract record, CompensationTerms compensation, WorkSchedule schedule,
		EmploymentDuration duration, PaymentMethod paymentMethod)
	{
		record.FixedRateCurrencyId = compensation.FixedRate?.Currency.Id;
		record.FixedRateAmount = compensation.FixedRate?.Amount;
		record.MarketBindingType = (int)(compensation.MarketBinding?.BindingType ?? MarketRateBindingType.None);
		record.MarketBindingValue = compensation.MarketBinding?.Value;
		record.PayCadence = (int)compensation.Cadence;
		record.MinimumEffectivePayCurrencyId = compensation.MinimumEffectivePay?.Currency.Id;
		record.MinimumEffectivePayAmount = compensation.MinimumEffectivePay?.Amount;
		record.EmployerPaymentSource = (int)compensation.EmployerPaymentSource;
		record.ScheduleDescription = schedule.Description;
		record.ScheduleStartTicks = schedule.StartsAt?.Ticks;
		record.ScheduleEndTicks = schedule.EndsAt?.Ticks;
		record.DurationType = (int)duration.DurationType;
		record.DurationTicks = duration.Length?.Ticks;
		record.PaymentMethodKind = (int)paymentMethod.MethodKind;
		record.PaymentBankAccountId = paymentMethod.BankAccount?.Id;
		record.PaymentItemId = paymentMethod.PaymentItemPrototype?.Id;
		record.PaymentItemType = paymentMethod.PaymentItemPrototype?.FrameworkItemType;
		record.PaymentNotes = paymentMethod.Notes;
	}

	private static void WriteTerms(DbJobOpening record, CompensationTerms compensation, WorkSchedule schedule,
		EmploymentDuration duration, PaymentMethod paymentMethod)
	{
		record.FixedRateCurrencyId = compensation.FixedRate?.Currency.Id;
		record.FixedRateAmount = compensation.FixedRate?.Amount;
		record.MarketBindingType = (int)(compensation.MarketBinding?.BindingType ?? MarketRateBindingType.None);
		record.MarketBindingValue = compensation.MarketBinding?.Value;
		record.PayCadence = (int)compensation.Cadence;
		record.MinimumEffectivePayCurrencyId = compensation.MinimumEffectivePay?.Currency.Id;
		record.MinimumEffectivePayAmount = compensation.MinimumEffectivePay?.Amount;
		record.EmployerPaymentSource = (int)compensation.EmployerPaymentSource;
		record.ScheduleDescription = schedule.Description;
		record.ScheduleStartTicks = schedule.StartsAt?.Ticks;
		record.ScheduleEndTicks = schedule.EndsAt?.Ticks;
		record.DurationType = (int)duration.DurationType;
		record.DurationTicks = duration.Length?.Ticks;
		record.PaymentMethodKind = (int)paymentMethod.MethodKind;
		record.PaymentBankAccountId = paymentMethod.BankAccount?.Id;
		record.PaymentItemId = paymentMethod.PaymentItemPrototype?.Id;
		record.PaymentItemType = paymentMethod.PaymentItemPrototype?.FrameworkItemType;
		record.PaymentNotes = paymentMethod.Notes;
	}

	private IEnumerable<DbJobRequirement> ToRequirementRecords(JobOpening opening)
	{
		foreach (var requirement in opening.Requirements.Skills)
		{
			yield return new DbJobRequirement
			{
				RequirementType = (int)JobRequirementType.Skill,
				Name = requirement.SkillName,
				NumericValue = requirement.MinimumValue
			};
		}

		foreach (var requirement in opening.Requirements.Knowledges)
		{
			yield return new DbJobRequirement
			{
				RequirementType = (int)JobRequirementType.Knowledge,
				Name = requirement.KnowledgeName
			};
		}

		foreach (var requirement in opening.Requirements.Capabilities)
		{
			yield return new DbJobRequirement
			{
				RequirementType = (int)JobRequirementType.Capability,
				Name = requirement.Capability.ToString(),
				Capability = (int)requirement.Capability
			};
		}

		foreach (var requirement in opening.Requirements.Tags)
		{
			yield return new DbJobRequirement
			{
				RequirementType = (int)JobRequirementType.Tag,
				Name = requirement.TagName
			};
		}
	}

	private EmploymentContract? ToContract(DbContract record)
	{
		var employee = _gameworld.TryGetCharacter(record.EmployeeId, true);
		if (employee is null)
		{
			return null;
		}

		return new EmploymentContract(
			record.RuntimeId,
			_host,
			employee,
			(EmploymentRole)record.Role,
			(EmploymentStatus)record.Status,
			new EmploymentAuthoritySet((EmploymentAuthority)record.Authority),
			ToCompensation(record),
			ToSchedule(record),
			ToDuration(record),
			ToPaymentMethod(record),
			EmploymentClock.NormaliseLoadedInstant(_host, record.StartedAt),
			EmploymentClock.NormaliseLoadedInstant(_host, record.EndsAt),
			record.EndReason.HasValue ? (EmploymentTerminationReason)record.EndReason.Value : null,
			record.OriginOpeningId,
			record.OriginApplicationId);
	}

	private JobOpening? ToJobOpening(DbJobOpening record)
	{
		return new JobOpening(
			record.RuntimeId,
			_host,
			(EmploymentRole)record.Role,
			ToRequirements(record.Requirements),
			ToCompensation(record),
			ToSchedule(record),
			ToDuration(record),
			(JobOpeningStatus)record.Status,
			record.MaxPositions,
			record.NpcApplicationsOnly,
			ToPaymentMethod(record),
			new EmploymentAuthoritySet((EmploymentAuthority)record.Authority),
			Math.Max(1, record.RevisionNumber));
	}

	private EmploymentApplication? ToApplication(DbApplication record, IReadOnlyDictionary<long, JobOpening> openings)
	{
		if (!openings.TryGetValue(record.EmploymentJobOpeningId, out var opening))
		{
			return null;
		}

		var candidate = _gameworld.TryGetCharacter(record.CandidateId, true);
		if (candidate is null)
		{
			return null;
		}

		return new EmploymentApplication(
			record.RuntimeId,
			opening,
			candidate,
			ToOffset(record.AppliedAt),
			(JobApplicationStatus)record.Status,
			record.DecisionReason,
			Math.Max(1, record.OfferedOpeningRevision),
			DeserializeCandidateProfile(record.CandidateProfileJson, candidate));
	}

	private static string? SerializeCandidateProfile(EmploymentCandidateProfile? profile)
	{
		if (profile is null)
		{
			return null;
		}

		return JsonSerializer.Serialize(new EmploymentCandidateProfilePayload(
			profile.ReservationWage,
			profile.ReservationWageCurrency?.Id,
			profile.Skills.ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase),
			profile.Knowledges.ToList(),
			profile.Capabilities.ToList(),
			profile.Tags.ToList(),
			profile.AcceptedPaymentMethods.ToList()));
	}

	private EmploymentCandidateProfile? DeserializeCandidateProfile(string? text, ICharacter candidate)
	{
		var payload = TryDeserializeActionPayload<EmploymentCandidateProfilePayload>(text);
		if (payload is null)
		{
			return null;
		}

		var currency = payload.ReservationWageCurrencyId.HasValue
			? _gameworld.Currencies.Get(payload.ReservationWageCurrencyId.Value)
			: null;
		return new EmploymentCandidateProfile(
			candidate,
			payload.ReservationWage,
			new Dictionary<string, double>(payload.Skills ?? new Dictionary<string, double>(),
				StringComparer.InvariantCultureIgnoreCase),
			new HashSet<string>(payload.Knowledges ?? [], StringComparer.InvariantCultureIgnoreCase),
			new HashSet<EmploymentAICapability>(payload.Capabilities ?? []),
			new HashSet<string>(payload.Tags ?? [], StringComparer.InvariantCultureIgnoreCase),
			payload.AcceptedPaymentMethods ?? [],
			currency);
	}

	private EmploymentPayable? ToPayable(DbPayable record)
	{
		var amount = ToMoney(record.AmountCurrencyId, record.Amount);
		if (amount is null)
		{
			return null;
		}

		var payPeriodStart = ToOffset(record.PayPeriodStart);
		if (EmploymentClock.EconomicZone(_host) is not null && !EmploymentClock.IsEncodedInstant(payPeriodStart))
		{
			return null;
		}

		return new EmploymentPayable(
			record.RuntimeId,
			Guid.TryParse(record.CorrelationId, out var id) ? id : Guid.NewGuid(),
			_host,
			record.ContractRuntimeId,
			record.EmployeeId,
			record.EmployeeName,
			(EmploymentRole)record.Role,
			amount,
			(PayCadence)record.PayCadence,
			new PaymentMethod(
				(PaymentMethodKind)record.PaymentMethodKind,
				record.PaymentBankAccountId.HasValue ? _gameworld.BankAccounts.Get(record.PaymentBankAccountId.Value) : null,
				ResolveFrameworkItem(record.PaymentItemId, record.PaymentItemType),
				record.PaymentNotes),
			payPeriodStart,
			ToOffset(record.PayPeriodEnd),
			ToOffset(record.DueAt),
			ToOffset(record.AccruedAt),
			(EmploymentPayableStatus)record.Status,
			ToNullableOffset(record.SettledAt),
			ToNullableOffset(record.ClaimedAt),
			record.SettlementNote);
	}

	private JobRequirementSet ToRequirements(IEnumerable<DbJobRequirement> requirements)
	{
		return new JobRequirementSet(
			requirements
				.Where(x => x.RequirementType == (int)JobRequirementType.Skill)
				.Select(x => new SkillRequirement(x.Name, x.NumericValue ?? 0.0))
				.ToList(),
			requirements
				.Where(x => x.RequirementType == (int)JobRequirementType.Knowledge)
				.Select(x => new KnowledgeRequirement(x.Name))
				.ToList(),
			requirements
				.Where(x => x.RequirementType == (int)JobRequirementType.Capability && x.Capability.HasValue)
				.Select(x => new AICapabilityRequirement((EmploymentAICapability)x.Capability!.Value))
				.ToList(),
			requirements
				.Where(x => x.RequirementType == (int)JobRequirementType.Tag)
				.Select(x => new TagRequirement(x.Name))
				.ToList());
	}

	private CompensationTerms ToCompensation(DbContract record)
	{
		return new CompensationTerms(
			ToMoney(record.FixedRateCurrencyId, record.FixedRateAmount),
			record.MarketBindingType == (int)MarketRateBindingType.None
				? null
				: new MarketRateBinding((MarketRateBindingType)record.MarketBindingType, record.MarketBindingValue ?? 0.0M),
			(PayCadence)record.PayCadence,
			ToMoney(record.MinimumEffectivePayCurrencyId, record.MinimumEffectivePayAmount),
			(PaymentSource)record.EmployerPaymentSource);
	}

	private CompensationTerms ToCompensation(DbJobOpening record)
	{
		return new CompensationTerms(
			ToMoney(record.FixedRateCurrencyId, record.FixedRateAmount),
			record.MarketBindingType == (int)MarketRateBindingType.None
				? null
				: new MarketRateBinding((MarketRateBindingType)record.MarketBindingType, record.MarketBindingValue ?? 0.0M),
			(PayCadence)record.PayCadence,
			ToMoney(record.MinimumEffectivePayCurrencyId, record.MinimumEffectivePayAmount),
			(PaymentSource)record.EmployerPaymentSource);
	}

	private WorkSchedule ToSchedule(DbContract record)
	{
		return new WorkSchedule(
			record.ScheduleDescription,
			record.ScheduleStartTicks.HasValue ? TimeSpan.FromTicks(record.ScheduleStartTicks.Value) : null,
			record.ScheduleEndTicks.HasValue ? TimeSpan.FromTicks(record.ScheduleEndTicks.Value) : null);
	}

	private WorkSchedule ToSchedule(DbJobOpening record)
	{
		return new WorkSchedule(
			record.ScheduleDescription,
			record.ScheduleStartTicks.HasValue ? TimeSpan.FromTicks(record.ScheduleStartTicks.Value) : null,
			record.ScheduleEndTicks.HasValue ? TimeSpan.FromTicks(record.ScheduleEndTicks.Value) : null);
	}

	private EmploymentDuration ToDuration(DbContract record)
	{
		return new EmploymentDuration(
			(EmploymentDurationType)record.DurationType,
			record.DurationTicks.HasValue ? TimeSpan.FromTicks(record.DurationTicks.Value) : null);
	}

	private EmploymentDuration ToDuration(DbJobOpening record)
	{
		return new EmploymentDuration(
			(EmploymentDurationType)record.DurationType,
			record.DurationTicks.HasValue ? TimeSpan.FromTicks(record.DurationTicks.Value) : null);
	}

	private PaymentMethod ToPaymentMethod(DbContract record)
	{
		return new PaymentMethod(
			(PaymentMethodKind)record.PaymentMethodKind,
			record.PaymentBankAccountId.HasValue ? _gameworld.BankAccounts.Get(record.PaymentBankAccountId.Value) : null,
			ResolveFrameworkItem(record.PaymentItemId, record.PaymentItemType),
			record.PaymentNotes);
	}

	private PaymentMethod ToPaymentMethod(DbJobOpening record)
	{
		return new PaymentMethod(
			(PaymentMethodKind)record.PaymentMethodKind,
			record.PaymentBankAccountId.HasValue ? _gameworld.BankAccounts.Get(record.PaymentBankAccountId.Value) : null,
			ResolveFrameworkItem(record.PaymentItemId, record.PaymentItemType),
			record.PaymentNotes);
	}

	private IFrameworkItem? ResolveFrameworkItem(long? id, string? itemType)
	{
		if (!id.HasValue || string.IsNullOrWhiteSpace(itemType))
		{
			return null;
		}

		return new FrameworkItemReference(id.Value, itemType, _gameworld).GetItem;
	}

	private MoneyAmount? ToMoney(long? currencyId, decimal? amount)
	{
		if (!amount.HasValue)
		{
			return null;
		}

		var currency = currencyId.HasValue ? _gameworld.Currencies.Get(currencyId.Value) : ResolveHostCurrency(_host);
		return currency is null ? null : new MoneyAmount(currency, amount.Value);
	}

	private IEmploymentActionStep? ToActionStep(DbActionStep record)
	{
		var amount = ToMoney(record.AmountCurrencyId, record.Amount);
		var destination = record.DestinationCellId.HasValue ? _gameworld.Cells.Get(record.DestinationCellId.Value) : null;
		var executionCell = record.ExecutionCellId.HasValue ? _gameworld.Cells.Get(record.ExecutionCellId.Value) : null;

		return (EmploymentActionStepType)record.StepType switch
		{
			EmploymentActionStepType.SupplierSelection =>
				ToSupplierSelectionStep(record),
			EmploymentActionStepType.Purchase when TryDeserializeActionPayload<PurchaseStepPayload>(record.BoardText) is { } purchasePayload =>
				ToPurchaseStep(purchasePayload, record.ExistingFinancialRecord),
			EmploymentActionStepType.Purchase when amount is not null =>
				new PurchaseActionStep(record.Description ?? "purchase", amount, record.ExistingFinancialRecord),
			EmploymentActionStepType.MoveOrDeliver =>
				new MovementDeliveryActionStep(record.Description ?? "delivery", destination),
			EmploymentActionStepType.CraftTrigger =>
				new CraftTriggerActionStep(record.Description ?? "craft", record.ExistingFinancialRecord),
			EmploymentActionStepType.Command =>
				new CommandActionStep(record.CommandName ?? string.Empty, record.CommandArguments ?? string.Empty, executionCell),
			EmploymentActionStepType.BankDeposit when amount is not null =>
				new BankDepositActionStep(amount, record.ExistingFinancialRecord),
			EmploymentActionStepType.BankWithdrawal when amount is not null =>
				new BankWithdrawalActionStep(amount, record.ExistingFinancialRecord),
			EmploymentActionStepType.AccountTransfer when amount is not null =>
				new BankAccountTransferActionStep(record.AccountName ?? string.Empty, amount, record.ExistingFinancialRecord),
			EmploymentActionStepType.HostSettlement when amount is not null =>
				new HostSettlementActionStep(record.AccountName ?? string.Empty, amount, record.ExistingFinancialRecord),
			EmploymentActionStepType.StoreAccountPayment when amount is not null =>
				new StoreAccountPaymentActionStep(record.AccountName ?? string.Empty, amount, record.ExistingFinancialRecord),
			EmploymentActionStepType.BoardPost =>
				new BoardPostActionStep(record.BoardTitle ?? "Notice", record.BoardText ?? string.Empty),
			EmploymentActionStepType.GetItemsById =>
				ToGetItemsByIdStep(record),
			EmploymentActionStepType.GetItemsByTag =>
				ToGetItemsByTagStep(record),
			EmploymentActionStepType.GetCommodity =>
				ToGetCommodityStep(record),
			EmploymentActionStepType.DeliverItems =>
				ToDeliverItemsStep(record, destination),
			EmploymentActionStepType.ShopStockTransfer =>
				ToShopStockTransferStep(record),
			EmploymentActionStepType.AuctionLotListing =>
				ToAuctionLotListingStep(record),
			EmploymentActionStepType.AuctionSettlement =>
				ToAuctionSettlementStep(record),
			EmploymentActionStepType.AuctionClaim =>
				ToAuctionClaimStep(record),
			EmploymentActionStepType.ArenaEventAdministration =>
				ToArenaEventAdministrationStep(record),
			EmploymentActionStepType.BankAdministration =>
				ToBankAdministrationStep(record),
			EmploymentActionStepType.StableAdministration =>
				ToStableAdministrationStep(record),
			EmploymentActionStepType.HotelAdministration =>
				ToHotelAdministrationStep(record),
			EmploymentActionStepType.HospitalService =>
				ToHospitalServiceStep(record),
			EmploymentActionStepType.HospitalSupplyPreparation =>
				ToHospitalSupplyPreparationStep(record),
			EmploymentActionStepType.HospitalAdministration =>
				ToHospitalAdministrationStep(record),
			EmploymentActionStepType.LoadItems =>
				ToLoadItemsStep(record),
			EmploymentActionStepType.UnloadItems =>
				ToUnloadItemsStep(record),
			EmploymentActionStepType.ReturnAsset =>
				ToReturnAssetStep(record),
			EmploymentActionStepType.VehicleOperation =>
				ToVehicleOperationStep(record),
			EmploymentActionStepType.StableAnimalOperation =>
				ToStableAnimalOperationStep(record),
			EmploymentActionStepType.CataloguedShell =>
				ToCataloguedShellStep(record, destination),
			EmploymentActionStepType.TaxPayment =>
				new TaxPaymentActionStep(amount),
			EmploymentActionStepType.PayrollSettlement =>
				new PayrollSettlementActionStep(record.AccountName ?? record.CommandArguments ?? "all",
					record.Description),
			EmploymentActionStepType.ShopCashReconciliation =>
				new ShopCashReconciliationActionStep(record.Description),
			EmploymentActionStepType.ShopFloatAdjustment =>
				ToShopFloatAdjustmentStep(record),
			EmploymentActionStepType.PhysicalFloat =>
				ToPhysicalFloatStep(record),
			EmploymentActionStepType.CraftStation =>
				new CraftStationActionStep(record.Description ?? "here"),
			EmploymentActionStepType.PriceChange =>
				ToPriceChangeStep(record),
			EmploymentActionStepType.ShopStocktake =>
				ToShopStocktakeStep(record),
			EmploymentActionStepType.ShopDealAdministration =>
				ToShopDealAdministrationStep(record),
			EmploymentActionStepType.JobOpeningAdministration =>
				ToJobOpeningAdministrationStep(record),
			EmploymentActionStepType.ScheduledRuleAdministration =>
				ToScheduledRuleAdministrationStep(record),
			EmploymentActionStepType.ActiveTaskAdministration =>
				ToActiveTaskAdministrationStep(record),
			EmploymentActionStepType.ManagerGoalAdministration =>
				ToManagerGoalAdministrationStep(record),
			_ => null
		};
	}

	private SupplierSelectionActionStep? ToSupplierSelectionStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<PurchaseStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var purchase = ToPurchaseStep(payload, null);
		return purchase?.IsExecutablePurchase == true ? new SupplierSelectionActionStep(purchase) : null;
	}
	private PurchaseActionStep? ToPurchaseStep(PurchaseStepPayload payload, string? existingFinancialRecord)
	{
		var currency = _gameworld.Currencies.Get(payload.CurrencyId);
		if (currency is null)
		{
			return null;
		}

		var maximum = payload.MaximumAmount.HasValue ? new MoneyAmount(currency, payload.MaximumAmount.Value) : null;
		var targetKind = payload.TargetKind.TryParseEnum<EmploymentPurchaseTargetKind>(out var parsedKind)
			? parsedKind
			: EmploymentPurchaseTargetKind.Merchandise;
		return targetKind switch
		{
			EmploymentPurchaseTargetKind.Item when ToItemSelector(payload.ItemSelector) is { } selector =>
				new PurchaseActionStep(payload.Quantity, selector, payload.SupplierSelector, currency, maximum,
					payload.KeywordFilter, existingFinancialRecord),
			EmploymentPurchaseTargetKind.Commodity when payload.CommodityWeight.HasValue &&
			                                        !string.IsNullOrWhiteSpace(payload.CommodityDescriptor) =>
				new PurchaseActionStep(payload.CommodityWeight.Value, payload.CommodityDescriptor,
					payload.SupplierSelector, currency, maximum, payload.KeywordFilter, existingFinancialRecord),
			_ => new PurchaseActionStep(payload.Quantity, payload.MerchandiseSelector, payload.SupplierSelector,
				currency, maximum, payload.KeywordFilter, existingFinancialRecord)
		};
	}

	private ShopFloatAdjustmentActionStep? ToShopFloatAdjustmentStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ShopFloatAdjustmentStepPayload>(record.BoardText);
		var amount = payload is null
			? ToMoney(record.AmountCurrencyId, record.Amount)
			: ToMoney(payload.CurrencyId, payload.Amount);
		if (amount is null)
		{
			return null;
		}

		return new ShopFloatAdjustmentActionStep(payload?.FillRegister ?? true, amount,
			ToItemSelector(payload?.RegisterSelector));
	}

	private PhysicalFloatActionStep? ToPhysicalFloatStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<PhysicalFloatStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<PhysicalFloatOperation>(out var operation))
		{
			return null;
		}

		var amount = payload.Amount.HasValue ? ToMoney(payload.CurrencyId, payload.Amount) : null;
		return new PhysicalFloatActionStep(operation, amount, payload.TargetKind,
			ToItemSelector(payload.TargetSelector));
	}

	private ShopStocktakeActionStep? ToShopStocktakeStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ShopStocktakeStepPayload>(record.BoardText);
		if (payload is null || !payload.Scope.TryParseEnum<ShopStocktakeScope>(out var scope))
		{
			return null;
		}

		return new ShopStocktakeActionStep(scope, payload.MerchandiseSelector, payload.MerchandiseName);
	}
	private IEmploymentActionStep? ToPriceChangeStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<PriceChangeStepPayload>(record.BoardText);
		if (payload is null || !payload.Kind.TryParseEnum<PriceChangeActionKind>(out var kind))
		{
			return null;
		}

		return kind switch
		{
			PriceChangeActionKind.Merchandise when payload.CurrencyId.HasValue && payload.ExactPrice.HasValue &&
			                                       ToMoney(payload.CurrencyId, payload.ExactPrice) is { } amount =>
				new PriceChangeActionStep(payload.MerchandiseSelector, amount),
			PriceChangeActionKind.MarketCategory =>
				new DeprecatedMarketPriceChangeActionStep(
					"Employment market-price actions are deprecated and require builder review before replacement with merchandise repricing or native shop deals.",
					record.BoardText),
			_ => null
		};
	}

	private ShopDealAdministrationActionStep? ToShopDealAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ShopDealAdministrationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<ShopDealAdministrationActionKind>(out var operation))
		{
			return null;
		}

		if (operation == ShopDealAdministrationActionKind.Cancel)
		{
			var selector = payload.DealSelector ?? payload.Name;
			return string.IsNullOrWhiteSpace(selector) ? null : new ShopDealAdministrationActionStep(selector);
		}

		if (!payload.DealType.TryParseEnum<ShopDealType>(out var dealType) ||
		    !payload.TargetType.TryParseEnum<ShopDealTargetType>(out var targetType) ||
		    !payload.Applicability.TryParseEnum<ShopDealApplicability>(out var applicability))
		{
			return null;
		}

		var expiry = string.IsNullOrWhiteSpace(payload.Expiry)
			? MudDateTime.Never
			: MudDateTime.FromStoredStringOrFallback(payload.Expiry, _gameworld,
				StoredMudDateTimeFallback.Never, "EmploymentActionStep", null, payload.Name, "ShopDealExpiry");
		var prog = payload.EligibilityProgId.HasValue ? _gameworld.FutureProgs.Get(payload.EligibilityProgId.Value) : null;
		if (operation == ShopDealAdministrationActionKind.Modify)
		{
			var selector = payload.DealSelector ?? payload.Name;
			return string.IsNullOrWhiteSpace(selector)
				? null
				: new ShopDealAdministrationActionStep(
					selector,
					payload.Name,
					dealType,
					targetType,
					payload.TargetSelector,
					payload.PriceAdjustmentPercentage,
					payload.MinimumQuantity,
					applicability,
					prog,
					payload.IsCumulative,
					expiry);
		}

		return new ShopDealAdministrationActionStep(
			payload.Name,
			dealType,
			targetType,
			payload.TargetSelector,
			payload.PriceAdjustmentPercentage,
			payload.MinimumQuantity,
			applicability,
			prog,
			payload.IsCumulative,
			expiry);
	}

	private JobOpeningAdministrationActionStep? ToJobOpeningAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<JobOpeningAdministrationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<JobOpeningAdministrationActionKind>(out var operation))
		{
			return null;
		}

		var definition = payload.Definition is null ? null : ToJobOpeningDefinition(payload.Definition);
		if (operation == JobOpeningAdministrationActionKind.Create)
		{
			return definition is null ? null : new JobOpeningAdministrationActionStep(definition, payload.Reason);
		}

		return payload.OpeningId.HasValue
			? new JobOpeningAdministrationActionStep(operation, payload.OpeningId.Value, definition, payload.Reason)
			: null;
	}

	private ScheduledRuleAdministrationActionStep? ToScheduledRuleAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ScheduledRuleAdministrationStepPayload>(record.BoardText);
		if (payload is null ||
		    !payload.Operation.TryParseEnum<ScheduledRuleAdministrationActionKind>(out var operation) ||
		    !Guid.TryParse(payload.RuleId, out var ruleId))
		{
			return null;
		}

		return new ScheduledRuleAdministrationActionStep(operation, ruleId, payload.RuleName, payload.Reason,
			payload.ManualKey);
	}

	private ActiveTaskAdministrationActionStep? ToActiveTaskAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ActiveTaskAdministrationStepPayload>(record.BoardText);
		if (payload is null ||
		    !payload.Operation.TryParseEnum<ActiveTaskAdministrationActionKind>(out var operation) ||
		    !Guid.TryParse(payload.TaskId, out var taskId))
		{
			return null;
		}

		return new ActiveTaskAdministrationActionStep(operation, taskId, payload.TaskName,
			payload.EmployeeId, payload.EmployeeName, payload.Reason);
	}

	private ManagerGoalAdministrationActionStep? ToManagerGoalAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ManagerGoalAdministrationStepPayload>(record.BoardText);
		if (payload is null ||
		    !payload.Operation.TryParseEnum<ManagerGoalAdministrationActionKind>(out var operation) ||
		    payload.GoalId <= 0)
		{
			return null;
		}

		return new ManagerGoalAdministrationActionStep(operation, payload.GoalId, payload.GoalName,
			payload.Reason);
	}

	private GetItemsByIdActionStep? ToGetItemsByIdStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<GetItemsByIdStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		return new GetItemsByIdActionStep(payload.Quantity, payload.ResolvedItemPrototypeIds,
			ResolveCells(payload.SourceLocationIds), payload.SpecificItemIds);
	}

	private GetItemsByTagActionStep? ToGetItemsByTagStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<GetItemsByTagStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		return new GetItemsByTagActionStep(payload.Quantity, payload.TagName, ResolveCells(payload.SourceLocationIds));
	}

	private GetCommodityActionStep? ToGetCommodityStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<GetCommodityStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		return new GetCommodityActionStep(payload.RequiredWeight, payload.MaterialName, payload.TagName,
			payload.Characteristics, ResolveCells(payload.SourceLocationIds));
	}

	private DeliverItemsActionStep? ToDeliverItemsStep(DbActionStep record, ICell? destination)
	{
		var payload = TryDeserializeActionPayload<DeliverItemsStepPayload>(record.BoardText);
		destination ??= payload is null ? null : _gameworld.Cells.Get(payload.DestinationCellId);
		if (destination is null)
		{
			return null;
		}

		return new DeliverItemsActionStep(destination,
			ToItemSelector(payload?.ContainerSelector, payload?.ContainerId, payload?.ContainerTag));
	}

	private ShopStockTransferActionStep? ToShopStockTransferStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ShopStockTransferStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		if (_gameworld.Shops.Get(payload.SourceShopId) is not IPermanentShop sourceShop ||
		    _gameworld.Shops.Get(payload.TargetShopId) is not IPermanentShop targetShop)
		{
			return null;
		}

		var destination = _gameworld.Cells.Get(payload.DestinationCellId);
		var merchandise = targetShop.Merchandises.FirstOrDefault(x => x.Id == payload.TargetMerchandiseId);
		if (destination is null || merchandise is null)
		{
			return null;
		}

		return new ShopStockTransferActionStep(sourceShop, targetShop, merchandise, destination,
			ToItemSelector(payload.ContainerSelector, payload.ContainerId, payload.ContainerTag));
	}

	private AuctionLotListingActionStep? ToAuctionLotListingStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<AuctionLotListingStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var auctionHouse = _gameworld.AuctionHouses.Get(payload.AuctionHouseId);
		var reserve = ToMoney(payload.ReserveCurrencyId, payload.ReserveAmount);
		var buyout = ToMoney(payload.BuyoutCurrencyId, payload.BuyoutAmount);
		var selector = ToItemSelector(payload.ItemSelector);
		if (auctionHouse is null || reserve is null || selector is null)
		{
			return null;
		}

		return new AuctionLotListingActionStep(auctionHouse, selector, reserve, buyout,
			payload.DurationTicks.HasValue ? TimeSpan.FromTicks(payload.DurationTicks.Value) : null);
	}

	private AuctionSettlementActionStep? ToAuctionSettlementStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<AuctionSettlementStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var auctionHouse = _gameworld.AuctionHouses.Get(payload.AuctionHouseId);
		return auctionHouse is null
			? null
			: new AuctionSettlementActionStep(auctionHouse, payload.AssetId, payload.AssetType, payload.AssetName);
	}

	private AuctionClaimActionStep? ToAuctionClaimStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<AuctionClaimStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var auctionHouse = _gameworld.AuctionHouses.Get(payload.AuctionHouseId);
		return auctionHouse is null
			? null
			: new AuctionClaimActionStep(auctionHouse, payload.AssetId, payload.AssetType, payload.AssetName);
	}

	private ArenaEventAdministrationActionStep? ToArenaEventAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ArenaEventAdministrationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<ArenaEventAdministrationActionKind>(out var operation))
		{
			return null;
		}

		var arena = _gameworld.CombatArenas.Get(payload.ArenaId);
		if (arena is null)
		{
			return null;
		}

		var targetState = !string.IsNullOrWhiteSpace(payload.TargetState) &&
		                  payload.TargetState.TryParseEnum<ArenaEventState>(out var parsedState)
			? parsedState
			: (ArenaEventState?)null;
		return new ArenaEventAdministrationActionStep(arena, operation, payload.EventTypeId, payload.EventTypeName,
			payload.EventId, payload.EventName, payload.ScheduledForUtc, targetState, payload.Reason);
	}

	private BankAdministrationActionStep? ToBankAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<BankAdministrationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<BankAdministrationActionKind>(out var operation))
		{
			return null;
		}

		var bank = _gameworld.Banks.Get(payload.BankId);
		if (bank is null)
		{
			return null;
		}

		var amount = payload.Amount.HasValue ? ToMoney(payload.CurrencyId, payload.Amount) : null;
		BankAccountStatus? status = null;
		if (!string.IsNullOrWhiteSpace(payload.TargetStatus))
		{
			if (!payload.TargetStatus.TryParseEnum<BankAccountStatus>(out var parsedStatus))
			{
				return null;
			}

			status = parsedStatus;
		}

		var sourceBranch = payload.SourceBranchId.HasValue ? _gameworld.Cells.Get(payload.SourceBranchId.Value) : null;
		var destinationBranch = payload.DestinationBranchId.HasValue ? _gameworld.Cells.Get(payload.DestinationBranchId.Value) : null;
		return new BankAdministrationActionStep(bank, operation, amount, payload.AccountSelector, status,
			sourceBranch, destinationBranch, payload.Reason);
	}
	private LoadItemsActionStep? ToLoadItemsStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<LoadItemsStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var location = payload.TargetLocationId.HasValue ? _gameworld.Cells.Get(payload.TargetLocationId.Value) : null;
		return new LoadItemsActionStep(ToItemSelector(payload.ContainerSelector, payload.ContainerId, payload.ContainerTag),
			location);
	}

	private UnloadItemsActionStep? ToUnloadItemsStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<UnloadItemsStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var location = payload.SourceLocationId.HasValue ? _gameworld.Cells.Get(payload.SourceLocationId.Value) : null;
		return new UnloadItemsActionStep(ToItemSelector(payload.ContainerSelector, payload.ContainerId, payload.ContainerTag),
			location);
	}

	private ReturnAssetActionStep? ToReturnAssetStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<ReturnAssetStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var destination = _gameworld.Cells.Get(payload.DestinationCellId);
		if (destination is null)
		{
			return null;
		}

		return new ReturnAssetActionStep(
			ToItemSelector(payload.ContainerSelector, payload.ContainerId, payload.ContainerTag),
			destination,
			ToItemSelector(payload.DestinationContainerSelector, payload.DestinationContainerId,
				payload.DestinationContainerTag));
	}

	private VehicleOperationActionStep? ToVehicleOperationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<VehicleOperationStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var vehicle = _gameworld.Vehicles.Get(payload.VehicleId);
		if (vehicle is null)
		{
			return null;
		}

		if (payload.Operation.EqualTo("assign") || payload.CargoSpaceId is null)
		{
			return new VehicleOperationActionStep(vehicle);
		}

		var cargo = vehicle.CargoSpaces.FirstOrDefault(x => x.Id == payload.CargoSpaceId.Value);
		return cargo is null ? null : new VehicleOperationActionStep(vehicle, cargo);
	}

	private StableAnimalOperationActionStep? ToStableAnimalOperationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<StableAnimalOperationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<EmploymentAnimalOperationKind>(out var operation))
		{
			return null;
		}

		var mount = payload.MountId.HasValue ? _gameworld.TryGetCharacter(payload.MountId.Value, true) : null;
		var stable = payload.StableId.HasValue ? _gameworld.Stables.Get(payload.StableId.Value) : null;
		var stay = payload.StayId.HasValue && stable is not null
			? stable.Stays.FirstOrDefault(x => x.Id == payload.StayId.Value)
			: null;
		var destination = payload.DestinationCellId.HasValue ? _gameworld.Cells.Get(payload.DestinationCellId.Value) : null;
		return new StableAnimalOperationActionStep(operation, mount, stable, stay, destination, payload.WaiveFees);
	}

	private StableAdministrationActionStep? ToStableAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<StableAdministrationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<StableAdministrationActionKind>(out var operation))
		{
			return null;
		}

		var stable = _gameworld.Stables.Get(payload.StableId);
		if (stable is null)
		{
			return null;
		}

		var stay = payload.StayId.HasValue
			? stable.Stays.FirstOrDefault(x => x.Id == payload.StayId.Value)
			: null;
		var account = payload.AccountId.HasValue
			? stable.StableAccounts.FirstOrDefault(x => x.Id == payload.AccountId.Value)
			: null;
		return new StableAdministrationActionStep(stable, operation, stay, account, payload.Note);
	}

	private HotelAdministrationActionStep? ToHotelAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<HotelAdministrationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<HotelAdministrationActionKind>(out var operation))
		{
			return null;
		}

		var hotel = _gameworld.Properties.Get(payload.PropertyId)?.Hotel;
		if (hotel is null)
		{
			return null;
		}

		var room = payload.RoomCellId.HasValue
			? hotel.Rooms.FirstOrDefault(x => x.Cell.Id == payload.RoomCellId.Value)
			: null;
		var lost = payload.LostPropertyBundleId.HasValue
			? hotel.Property.HotelLostProperties.FirstOrDefault(x => x.BundleId == payload.LostPropertyBundleId.Value)
			: null;
		var balance = payload.PatronId.HasValue
			? hotel.Property.HotelPatronBalances.FirstOrDefault(x => x.PatronId == payload.PatronId.Value)
			: null;
		return new HotelAdministrationActionStep(hotel, operation, room, lost, balance, payload.PatronSelector,
			payload.Note);
	}
	private HospitalServiceActionStep? ToHospitalServiceStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<HospitalServiceStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var hospital = _gameworld.Hospitals.Get(payload.HospitalId);
		var request = hospital?.RequestById(payload.RequestId.ToString(CultureInfo.InvariantCulture));
		return hospital is null || request is null ? null : new HospitalServiceActionStep(hospital, request);
	}

	private HospitalSupplyPreparationActionStep? ToHospitalSupplyPreparationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<HospitalSupplyPreparationStepPayload>(record.BoardText);
		if (payload is null)
		{
			return null;
		}

		var hospital = _gameworld.Hospitals.Get(payload.HospitalId);
		var request = hospital?.RequestById(payload.RequestId.ToString(CultureInfo.InvariantCulture));
		return hospital is null || request is null ? null : new HospitalSupplyPreparationActionStep(hospital, request);
	}
	private HospitalAdministrationActionStep? ToHospitalAdministrationStep(DbActionStep record)
	{
		var payload = TryDeserializeActionPayload<HospitalAdministrationStepPayload>(record.BoardText);
		if (payload is null || !payload.Operation.TryParseEnum<HospitalAdministrationActionKind>(out var operation))
		{
			return null;
		}

		var hospital = _gameworld.Hospitals.Get(payload.HospitalId);
		return hospital is null ? null : new HospitalAdministrationActionStep(hospital, operation, payload.Note);
	}
	private EmploymentItemSelector? ToItemSelector(ItemSelectorPayload? payload, long? legacyItemId = null,
		string? legacyTag = null)
	{
		if (payload is not null && payload.Kind.TryParseEnum<EmploymentItemSelectorKind>(out var kind))
		{
			return kind switch
			{
				EmploymentItemSelectorKind.PrototypeId when payload.Id.HasValue =>
					EmploymentItemSelector.ForPrototype(payload.Id.Value),
				EmploymentItemSelectorKind.ItemId when payload.Id.HasValue =>
					_gameworld.TryGetItem(payload.Id.Value, true) is { } item
						? EmploymentItemSelector.ForItem(item)
						: EmploymentItemSelector.ForItemId(payload.Id.Value),
				EmploymentItemSelectorKind.Keyword when payload.Id.HasValue =>
					new EmploymentItemSelector(EmploymentItemSelectorKind.Keyword, payload.Id, payload.Text,
						_gameworld.TryGetItem(payload.Id.Value, true)),
				EmploymentItemSelectorKind.Keyword =>
					string.IsNullOrWhiteSpace(payload.Text) ? null : EmploymentItemSelector.ForKeyword(payload.Text),
				EmploymentItemSelectorKind.Tag =>
					string.IsNullOrWhiteSpace(payload.Text) ? null : EmploymentItemSelector.ForTag(payload.Text),
				_ => null
			};
		}

		if (legacyItemId.HasValue)
		{
			var item = _gameworld.TryGetItem(legacyItemId.Value, true);
			return item is null ? EmploymentItemSelector.ForItemId(legacyItemId.Value) : EmploymentItemSelector.ForItem(item);
		}

		return string.IsNullOrWhiteSpace(legacyTag) ? null : EmploymentItemSelector.ForTag(legacyTag);
	}

	private static ItemSelectorPayload? FromItemSelector(EmploymentItemSelector? selector)
	{
		return selector is null
			? null
			: new ItemSelectorPayload(selector.Kind.ToString(), selector.Id, selector.Text);
	}

	private JobOpeningDefinition? ToJobOpeningDefinition(JobOpeningDefinitionPayload payload)
	{
		if (!payload.Role.TryParseEnum<EmploymentRole>(out var role))
		{
			return null;
		}

		if (!payload.Compensation.Cadence.TryParseEnum<PayCadence>(out var cadence) ||
		    !payload.Compensation.EmployerPaymentSource.TryParseEnum<PaymentSource>(out var source))
		{
			return null;
		}

		var compensation = new CompensationTerms(
			ToMoney(payload.Compensation.FixedCurrencyId, payload.Compensation.FixedAmount),
			string.IsNullOrWhiteSpace(payload.Compensation.MarketBindingType) ||
			!payload.Compensation.MarketBindingType.TryParseEnum<MarketRateBindingType>(out var bindingType) ||
			bindingType == MarketRateBindingType.None
				? null
				: new MarketRateBinding(bindingType, payload.Compensation.MarketBindingValue ?? 0.0M),
			cadence,
			ToMoney(payload.Compensation.MinimumCurrencyId, payload.Compensation.MinimumAmount),
			source);

		if (!payload.Duration.DurationType.TryParseEnum<EmploymentDurationType>(out var durationType) ||
		    !payload.PaymentMethod.MethodKind.TryParseEnum<PaymentMethodKind>(out var paymentKind))
		{
			return null;
		}

		return new JobOpeningDefinition(
			role,
			ToJobRequirements(payload.Requirements),
			compensation,
			new WorkSchedule(
				payload.Schedule.Description,
				payload.Schedule.StartsAtTicks.HasValue ? TimeSpan.FromTicks(payload.Schedule.StartsAtTicks.Value) : null,
				payload.Schedule.EndsAtTicks.HasValue ? TimeSpan.FromTicks(payload.Schedule.EndsAtTicks.Value) : null),
			new EmploymentDuration(
				durationType,
				payload.Duration.LengthTicks.HasValue ? TimeSpan.FromTicks(payload.Duration.LengthTicks.Value) : null),
			payload.MaxPositions,
			payload.NpcApplicationsOnly,
			new PaymentMethod(
				paymentKind,
				payload.PaymentMethod.BankAccountId.HasValue
					? _gameworld.BankAccounts.Get(payload.PaymentMethod.BankAccountId.Value)
					: null,
				ResolveFrameworkItem(payload.PaymentMethod.PaymentItemId, payload.PaymentMethod.PaymentItemType),
				payload.PaymentMethod.Notes),
			new EmploymentAuthoritySet((EmploymentAuthority)payload.Authority));
	}

	private static JobRequirementSet ToJobRequirements(JobRequirementsPayload payload)
	{
		var skillRequirements = payload.Skills
		                               .Select((name, index) => new SkillRequirement(
			                               name,
			                               index < payload.SkillMinimums.Length ? payload.SkillMinimums[index] : 0.0))
		                               .ToList();
		return new JobRequirementSet(
			skillRequirements,
			payload.Knowledges.Select(x => new KnowledgeRequirement(x)).ToList(),
			payload.Capabilities
			       .Select(x => x.TryParseEnum<EmploymentAICapability>(out var capability)
				       ? new AICapabilityRequirement(capability)
				       : null)
			       .OfType<AICapabilityRequirement>()
			       .ToList(),
			payload.Tags.Select(x => new TagRequirement(x)).ToList());
	}

	private static JobOpeningDefinitionPayload FromJobOpeningDefinition(JobOpeningDefinition definition)
	{
		return new JobOpeningDefinitionPayload(
			definition.Role.ToString(),
			new JobRequirementsPayload(
				definition.Requirements.Skills.Select(x => x.SkillName).ToArray(),
				definition.Requirements.Skills.Select(x => x.MinimumValue).ToArray(),
				definition.Requirements.Knowledges.Select(x => x.KnowledgeName).ToArray(),
				definition.Requirements.Capabilities.Select(x => x.Capability.ToString()).ToArray(),
				definition.Requirements.Tags.Select(x => x.TagName).ToArray()),
			new CompensationTermsPayload(
				definition.Compensation.FixedRate?.Currency.Id,
				definition.Compensation.FixedRate?.Amount,
				definition.Compensation.MarketBinding?.BindingType.ToString(),
				definition.Compensation.MarketBinding?.Value,
				definition.Compensation.Cadence.ToString(),
				definition.Compensation.MinimumEffectivePay?.Currency.Id,
				definition.Compensation.MinimumEffectivePay?.Amount,
				definition.Compensation.EmployerPaymentSource.ToString()),
			new WorkSchedulePayload(
				definition.Schedule.Description,
				definition.Schedule.StartsAt?.Ticks,
				definition.Schedule.EndsAt?.Ticks),
			new EmploymentDurationPayload(
				definition.Duration.DurationType.ToString(),
				definition.Duration.Length?.Ticks),
			definition.MaxPositions,
			definition.NpcApplicationsOnly,
			new PaymentMethodPayload(
				definition.PaymentMethod.MethodKind.ToString(),
				definition.PaymentMethod.BankAccount?.Id,
				definition.PaymentMethod.PaymentItemPrototype?.Id,
				definition.PaymentMethod.PaymentItemPrototype?.FrameworkItemType,
				definition.PaymentMethod.Notes),
			(long)definition.Authority.Authorities);
	}

	private CataloguedActionShellStep? ToCataloguedShellStep(DbActionStep record, ICell? destination)
	{
		var payload = TryDeserializeActionPayload<CataloguedActionShellPayload>(record.BoardText);
		var actionKey = payload?.ActionKey ?? record.CommandName ?? record.Description;
		if (string.IsNullOrWhiteSpace(actionKey))
		{
			return null;
		}

		var definition = EmploymentActionCatalog.Get(actionKey);
		if (definition is null || definition.Status == EmploymentActionCatalogStatus.Deferred)
		{
			return null;
		}

		var targetLocationId = payload?.TargetLocationId ?? record.DestinationCellId;
		destination ??= targetLocationId.HasValue ? _gameworld.Cells.Get(targetLocationId.Value) : null;
		var amount = payload?.Amount is not null
			? ToMoney(payload.AmountCurrencyId, payload.Amount)
			: ToMoney(record.AmountCurrencyId, record.Amount);
		var routeStopIds = payload?.RouteStopIds;
		var routeStops = routeStopIds is { Count: > 0 }
			? ResolveCells(routeStopIds).ToList()
			: new List<ICell>();
		return new CataloguedActionShellStep(
			actionKey,
			payload?.Description ?? record.Description ?? actionKey,
			amount,
			destination,
			routeStops);
	}

	private IEnumerable<ICell> ResolveCells(IEnumerable<long>? ids)
	{
		if (ids is null)
		{
			yield break;
		}

		foreach (var id in ids)
		{
			var cell = _gameworld.Cells.Get(id);
			if (cell is not null)
			{
				yield return cell;
			}
		}
	}

	private long SaveActionPlan(FuturemudDatabaseContext context, string name, EmploymentActionPlan actionPlan)
	{
		var dbitem = new DbActionPlan
		{
			EmploymentHostStateId = StateId,
			Name = name
		};
		context.EmploymentActionPlans.Add(dbitem);
		var order = 0;
		foreach (var step in actionPlan.Steps)
		{
			dbitem.Steps.Add(ToRecord(step, order++));
		}

		context.SaveChanges();
		return dbitem.Id;
	}

	private static DbActionStep ToRecord(IEmploymentActionStep step, int sortOrder)
	{
		var record = new DbActionStep
		{
			SortOrder = sortOrder,
			StepType = (int)step.StepType,
			RequiredAuthority = (long)step.RequiredAuthority.Authorities,
			RequiredCapabilities = string.Join(",", step.RequiredCapabilities.Select(x => ((int)x).ToString("F0"))),
			RequiresPaymentAuthorisation = step.RequiresPaymentAuthorisation,
			IsFinancialStep = step.IsFinancialStep
		};

		switch (step)
		{
			case SupplierSelectionActionStep supplier:
				var supplierPurchase = supplier.Purchase;
				record.Description = $"supplier {supplierPurchase.PurchaseDescription}";
				record.AmountCurrencyId = supplierPurchase.Amount.Currency.Id;
				record.Amount = supplierPurchase.Amount.Amount;
				record.BoardText = SerializeActionPayload(new PurchaseStepPayload(
					supplierPurchase.Quantity ?? 0,
					supplierPurchase.MerchandiseSelector ?? string.Empty,
					supplierPurchase.SupplierSelector ?? "any",
					supplierPurchase.Amount.Currency.Id,
					supplierPurchase.MaximumAmount?.Amount,
					supplierPurchase.KeywordFilter,
					supplierPurchase.TargetKind.ToString(),
					FromItemSelector(supplierPurchase.ItemSelector),
					supplierPurchase.CommodityWeight,
					supplierPurchase.CommodityDescriptor));
				break;
			case PurchaseActionStep purchase:
				record.Description = purchase.PurchaseDescription;
				record.AmountCurrencyId = purchase.Amount.Currency.Id;
				record.Amount = purchase.Amount.Amount;
				record.ExistingFinancialRecord = purchase.ExistingFinancialRecord;
				if (purchase.IsExecutablePurchase)
				{
					record.BoardText = SerializeActionPayload(new PurchaseStepPayload(
						purchase.Quantity ?? 0,
						purchase.MerchandiseSelector ?? string.Empty,
						purchase.SupplierSelector ?? "any",
						purchase.Amount.Currency.Id,
						purchase.MaximumAmount?.Amount,
						purchase.KeywordFilter,
						purchase.TargetKind.ToString(),
						FromItemSelector(purchase.ItemSelector),
						purchase.CommodityWeight,
						purchase.CommodityDescriptor));
				}
				break;
			case MovementDeliveryActionStep delivery:
				record.Description = delivery.DeliveryDescription;
				record.DestinationCellId = delivery.Destination?.Id;
				break;
			case CraftTriggerActionStep craft:
				record.Description = craft.CraftDescription;
				record.ExistingFinancialRecord = craft.ExistingFinancialRecord;
				break;
			case CraftStationActionStep station:
				record.Description = station.StationSelector;
				break;
			case CommandActionStep command:
				record.CommandName = command.CommandName;
				record.CommandArguments = command.CommandArguments;
				record.ExecutionCellId = command.ExecutionLocation?.Id;
				break;
			case BankDepositActionStep deposit:
				record.AmountCurrencyId = deposit.Amount.Currency.Id;
				record.Amount = deposit.Amount.Amount;
				record.ExistingFinancialRecord = deposit.ExistingFinancialRecord;
				break;
			case BankWithdrawalActionStep withdrawal:
				record.AmountCurrencyId = withdrawal.Amount.Currency.Id;
				record.Amount = withdrawal.Amount.Amount;
				record.ExistingFinancialRecord = withdrawal.ExistingFinancialRecord;
				break;
			case BankAccountTransferActionStep transfer:
				record.AccountName = transfer.TargetAccountKey;
				record.AmountCurrencyId = transfer.Amount.Currency.Id;
				record.Amount = transfer.Amount.Amount;
				record.ExistingFinancialRecord = transfer.ExistingFinancialRecord;
				break;
			case HostSettlementActionStep settlement:
				record.AccountName = settlement.TargetHostKey;
				record.AmountCurrencyId = settlement.Amount.Currency.Id;
				record.Amount = settlement.Amount.Amount;
				record.ExistingFinancialRecord = settlement.ExistingFinancialRecord;
				break;
			case StoreAccountPaymentActionStep account:
				record.AccountName = account.AccountName;
				record.AmountCurrencyId = account.Amount.Currency.Id;
				record.Amount = account.Amount.Amount;
				record.ExistingFinancialRecord = account.ExistingFinancialRecord;
				break;
			case BoardPostActionStep boardPost:
				record.BoardTitle = boardPost.Title;
				record.BoardText = boardPost.Text;
				break;
			case GetItemsByIdActionStep getById:
				record.Description = $"get {getById.Quantity:N0} item(s) by item selector";
				record.BoardText = SerializeActionPayload(new GetItemsByIdStepPayload(
					getById.Quantity,
					getById.ItemPrototypeIds.ToArray(),
					getById.SourceLocations.Select(x => x.Id).ToArray(),
					getById.SpecificItemIds.ToArray()));
				break;
			case GetItemsByTagActionStep getByTag:
				record.Description = $"get {getByTag.Quantity:N0} item(s) tagged {getByTag.TagName}";
				record.BoardText = SerializeActionPayload(new GetItemsByTagStepPayload(
					getByTag.Quantity,
					getByTag.TagName,
					getByTag.SourceLocations.Select(x => x.Id).ToArray()));
				break;
			case GetCommodityActionStep getCommodity:
				record.Description = $"get {getCommodity.RequiredWeight:N2} weight of {getCommodity.MaterialName}";
				record.BoardText = SerializeActionPayload(new GetCommodityStepPayload(
					getCommodity.RequiredWeight,
					getCommodity.MaterialName,
					getCommodity.TagName,
					new Dictionary<string, string>(getCommodity.Characteristics,
						StringComparer.InvariantCultureIgnoreCase),
					getCommodity.SourceLocations.Select(x => x.Id).ToArray()));
				break;
			case DeliverItemsActionStep deliver:
				record.Description = "deliver task items";
				record.DestinationCellId = deliver.Destination.Id;
				record.BoardText = SerializeActionPayload(new DeliverItemsStepPayload(
					deliver.Destination.Id,
					deliver.Container?.Id,
					deliver.ContainerTag,
					FromItemSelector(deliver.ContainerSelector)));
				break;
			case ShopStockTransferActionStep stockTransfer:
				record.Description = $"transfer stock to {stockTransfer.TargetShop.Name}";
				record.DestinationCellId = stockTransfer.Destination.Id;
				record.BoardText = SerializeActionPayload(new ShopStockTransferStepPayload(
					stockTransfer.SourceShop.Id,
					stockTransfer.TargetShop.Id,
					stockTransfer.TargetMerchandise.Id,
					stockTransfer.Destination.Id,
					stockTransfer.Container?.Id,
					stockTransfer.ContainerTag,
					FromItemSelector(stockTransfer.ContainerSelector)));
				break;
			case AuctionLotListingActionStep auctionList:
				record.Description = $"list auction lot at {auctionList.AuctionHouse.Name}";
				record.BoardText = SerializeActionPayload(new AuctionLotListingStepPayload(
					auctionList.AuctionHouse.Id,
					FromItemSelector(auctionList.ItemSelector)!,
					auctionList.ReservePrice.Currency.Id,
					auctionList.ReservePrice.Amount,
					auctionList.BuyoutPrice?.Currency.Id,
					auctionList.BuyoutPrice?.Amount,
					auctionList.Duration?.Ticks));
				break;
			case AuctionSettlementActionStep auctionSettlement:
				record.Description = auctionSettlement.SettleAllDue
					? $"settle due auction lots at {auctionSettlement.AuctionHouse.Name}"
					: $"settle auction lot {auctionSettlement.AssetName ?? auctionSettlement.AssetId?.ToString("F0", CultureInfo.InvariantCulture)}";
				record.BoardText = SerializeActionPayload(new AuctionSettlementStepPayload(
					auctionSettlement.AuctionHouse.Id,
					auctionSettlement.AssetId,
					auctionSettlement.AssetType,
					auctionSettlement.AssetName));
				break;
			case AuctionClaimActionStep auctionClaim:
				record.Description = $"claim auction lot {auctionClaim.AssetName ?? auctionClaim.AssetId.ToString("F0", CultureInfo.InvariantCulture)}";
				record.BoardText = SerializeActionPayload(new AuctionClaimStepPayload(
					auctionClaim.AuctionHouse.Id,
					auctionClaim.AssetId,
					auctionClaim.AssetType,
					auctionClaim.AssetName));
				break;
			case BankAdministrationActionStep bankAdmin:
				record.AmountCurrencyId = bankAdmin.Amount?.Currency.Id;
				record.Amount = bankAdmin.Amount?.Amount;
				record.AccountName = bankAdmin.AccountSelector;
				record.Description = bankAdmin.Reason;
				record.ExecutionCellId = bankAdmin.SourceBranch?.Id;
				record.DestinationCellId = bankAdmin.DestinationBranch?.Id;
				record.BoardText = SerializeActionPayload(new BankAdministrationStepPayload(
					bankAdmin.Operation.ToString(),
					bankAdmin.Bank.Id,
					bankAdmin.Amount?.Currency.Id,
					bankAdmin.Amount?.Amount,
					bankAdmin.AccountSelector,
					bankAdmin.TargetStatus?.ToString(),
					bankAdmin.SourceBranch?.Id,
					bankAdmin.DestinationBranch?.Id,
					bankAdmin.Reason));
				break;			case ArenaEventAdministrationActionStep arenaEvent:
				record.Description = arenaEvent.Operation switch
				{
					ArenaEventAdministrationActionKind.Create => $"create arena event {arenaEvent.EventTypeName ?? arenaEvent.EventTypeId?.ToString("F0", CultureInfo.InvariantCulture)}",
					ArenaEventAdministrationActionKind.Transition => $"move arena event {arenaEvent.EventName ?? arenaEvent.EventId?.ToString("F0", CultureInfo.InvariantCulture)} to {arenaEvent.TargetState}",
					ArenaEventAdministrationActionKind.Abort => $"abort arena event {arenaEvent.EventName ?? arenaEvent.EventId?.ToString("F0", CultureInfo.InvariantCulture)}",
					_ => "manage arena event"
				};
				record.BoardText = SerializeActionPayload(new ArenaEventAdministrationStepPayload(
					arenaEvent.Operation.ToString(),
					arenaEvent.Arena.Id,
					arenaEvent.EventTypeId,
					arenaEvent.EventTypeName,
					arenaEvent.EventId,
					arenaEvent.EventName,
					arenaEvent.ScheduledForUtc,
					arenaEvent.TargetState?.ToString(),
					arenaEvent.Reason));
				break;
			case LoadItemsActionStep load:
				record.Description = "load task items";
				record.DestinationCellId = load.TargetLocation?.Id;
				record.BoardText = SerializeActionPayload(new LoadItemsStepPayload(
					load.TargetContainer?.Id,
					load.TargetContainerTag,
					load.TargetLocation?.Id,
					FromItemSelector(load.TargetContainerSelector)));
				break;
			case UnloadItemsActionStep unload:
				record.Description = "unload task items";
				record.DestinationCellId = unload.SourceLocation?.Id;
				record.BoardText = SerializeActionPayload(new UnloadItemsStepPayload(
					unload.SourceContainer?.Id,
					unload.SourceContainerTag,
					unload.SourceLocation?.Id,
					FromItemSelector(unload.SourceContainerSelector)));
				break;
			case ReturnAssetActionStep returnAsset:
				record.Description = "return task container";
				record.DestinationCellId = returnAsset.Destination.Id;
				record.BoardText = SerializeActionPayload(new ReturnAssetStepPayload(
					returnAsset.Container?.Id,
					returnAsset.ContainerTag,
					returnAsset.Destination.Id,
					returnAsset.DestinationContainer?.Id,
					returnAsset.DestinationContainerTag,
					FromItemSelector(returnAsset.ContainerSelector),
					FromItemSelector(returnAsset.DestinationContainerSelector)));
				break;
			case VehicleOperationActionStep vehicle:
				record.Description = vehicle.AssignsDriver
					? $"assign driver to {vehicle.Vehicle.Name}"
					: $"select cargo {vehicle.CargoSpace!.Name} on {vehicle.Vehicle.Name}";
				record.DestinationCellId = vehicle.Vehicle.Location?.Id;
				record.BoardText = SerializeActionPayload(new VehicleOperationStepPayload(
					vehicle.Vehicle.Id,
					vehicle.CargoSpace?.Id,
					vehicle.AssignsDriver ? "assign" : "cargo"));
				break;
			case StableAnimalOperationActionStep animal:
				record.Description = $"{animal.Operation.DescribeEnum()} animal operation";
				record.DestinationCellId = animal.Destination?.Id ?? animal.Stable?.Location.Id ?? animal.Mount?.Location.Id;
				record.BoardText = SerializeActionPayload(new StableAnimalOperationStepPayload(
					animal.Operation.ToString(),
					animal.Mount is null ? null : CharacterInstanceIdentityComparer.IdentityId(animal.Mount),
					animal.Stable?.Id,
					animal.Stay?.Id,
					animal.Destination?.Id,
					animal.WaiveFees));
				break;
			case StableAdministrationActionStep stableAdmin:
				record.Description = $"{stableAdmin.Operation} stable administration";
				record.DestinationCellId = stableAdmin.Stable.Location.Id;
				record.BoardText = SerializeActionPayload(new StableAdministrationStepPayload(
					stableAdmin.Operation.ToString(),
					stableAdmin.Stable.Id,
					stableAdmin.Stay?.Id,
					stableAdmin.Account?.Id,
					stableAdmin.Note));
				break;
			case HotelAdministrationActionStep hotelAdmin:
				record.Description = $"{hotelAdmin.Operation} hotel administration";
				record.DestinationCellId = hotelAdmin.Room?.Cell.Id;
				record.BoardText = SerializeActionPayload(new HotelAdministrationStepPayload(
					hotelAdmin.Operation.ToString(),
					hotelAdmin.Hotel.Property.Id,
					hotelAdmin.Room?.Cell.Id,
					hotelAdmin.LostProperty?.BundleId,
					hotelAdmin.PatronBalance?.PatronId,
					hotelAdmin.PatronSelector,
					hotelAdmin.Note));
				break;
			case HospitalServiceActionStep hospitalService:
				record.Description = $"service request #{hospitalService.Request.Id.ToString("N0", CultureInfo.InvariantCulture)} at {hospitalService.Hospital.Name}";
				record.DestinationCellId = hospitalService.Request.OperatingTheatreCellId ?? hospitalService.Request.Patient?.Location?.Id;
				record.BoardText = SerializeActionPayload(new HospitalServiceStepPayload(
					hospitalService.Hospital.Id,
					hospitalService.Request.Id));
				break;
			case HospitalSupplyPreparationActionStep hospitalSupply:
				record.Description = $"prepare supplies for request #{hospitalSupply.Request.Id.ToString("N0", CultureInfo.InvariantCulture)} at {hospitalSupply.Hospital.Name}";
				record.DestinationCellId = hospitalSupply.Request.OperatingTheatreCellId ?? hospitalSupply.Hospital.SupplyRooms.FirstOrDefault()?.Id;
				record.BoardText = SerializeActionPayload(new HospitalSupplyPreparationStepPayload(
					hospitalSupply.Hospital.Id,
					hospitalSupply.Request.Id));
				break;
			case HospitalAdministrationActionStep hospitalAdmin:
				record.Description = $"{hospitalAdmin.Operation} hospital administration";
				record.DestinationCellId = hospitalAdmin.Hospital.WaitingRooms.Concat(hospitalAdmin.Hospital.OperatingTheatres).FirstOrDefault()?.Id;
				record.BoardText = SerializeActionPayload(new HospitalAdministrationStepPayload(
					hospitalAdmin.Operation.ToString(),
					hospitalAdmin.Hospital.Id,
					hospitalAdmin.Note));
				break;
			case CataloguedActionShellStep shell:
				record.CommandName = shell.ActionKey;
				record.Description = shell.ActionDescription;
				record.AmountCurrencyId = shell.Amount?.Currency.Id;
				record.Amount = shell.Amount?.Amount;
				record.DestinationCellId = shell.TargetLocation?.Id;
				record.BoardText = SerializeActionPayload(new CataloguedActionShellPayload(
					shell.ActionKey,
					shell.ActionDescription,
					shell.TargetLocation?.Id,
					shell.Amount?.Currency.Id,
					shell.Amount?.Amount,
					shell.RouteStops.Count == 0 ? null : shell.RouteStops.Select(x => x.Id).ToArray()));
				break;
			case TaxPaymentActionStep tax:
				record.Description = "pay supported host taxes";
				record.AmountCurrencyId = tax.MaximumAmount?.Currency.Id;
				record.Amount = tax.MaximumAmount?.Amount;
				break;
			case PayrollSettlementActionStep payroll:
				record.Description = payroll.Reason;
				record.AccountName = payroll.Selector;
				break;
			case ShopCashReconciliationActionStep cashReconciliation:
				record.Description = cashReconciliation.Note;
				break;
			case ShopFloatAdjustmentActionStep shopFloat:
				record.Description = shopFloat.FillRegister ? "fill shop float" : "skim shop float";
				record.AmountCurrencyId = shopFloat.Amount.Currency.Id;
				record.Amount = shopFloat.Amount.Amount;
				record.BoardText = SerializeActionPayload(new ShopFloatAdjustmentStepPayload(
					shopFloat.FillRegister,
					shopFloat.Amount.Currency.Id,
					shopFloat.Amount.Amount,
					FromItemSelector(shopFloat.RegisterSelector)));
				break;
			case PhysicalFloatActionStep physicalFloat:
				record.Description = $"{physicalFloat.Operation} physical float";
				record.AmountCurrencyId = physicalFloat.Amount?.Currency.Id;
				record.Amount = physicalFloat.Amount?.Amount;
				record.BoardText = SerializeActionPayload(new PhysicalFloatStepPayload(
					physicalFloat.Operation.ToString(),
					physicalFloat.Amount?.Currency.Id,
					physicalFloat.Amount?.Amount,
					physicalFloat.TargetKind,
					FromItemSelector(physicalFloat.TargetSelector)));
				break;
			case DeprecatedMarketPriceChangeActionStep deprecated:
				record.Description = "deprecated market price action";
				record.BoardText = deprecated.OriginalPayload;
				break;
			case ShopStocktakeActionStep stocktake:
				record.Description = stocktake.Scope == ShopStocktakeScope.All
					? "Stocktake all merchandise"
					: $"Stocktake merchandise {stocktake.MerchandiseName ?? stocktake.MerchandiseSelector}";
				record.AccountName = stocktake.MerchandiseSelector;
				record.BoardText = SerializeActionPayload(new ShopStocktakeStepPayload(
					stocktake.Scope.ToString(),
					stocktake.MerchandiseSelector,
					stocktake.MerchandiseName));
				break;
			case PriceChangeActionStep price:
				record.Description = $"price merchandise {price.MerchandiseSelector}";
				record.AmountCurrencyId = price.ExactPrice?.Currency.Id;
				record.Amount = price.ExactPrice?.Amount;
				record.BoardText = SerializeActionPayload(new PriceChangeStepPayload(
					price.PriceChangeKind.ToString(),
					price.MerchandiseSelector,
					price.ExactPrice?.Currency.Id,
					price.ExactPrice?.Amount,
					string.Empty,
					string.Empty,
					0.0,
					0.0,
					0.0,
					string.Empty,
					null,
					null));
				break;
			case ShopDealAdministrationActionStep deal:
				record.Description = deal.Operation switch
				{
					ShopDealAdministrationActionKind.Create => $"shop deal create {deal.Name}",
					ShopDealAdministrationActionKind.Modify => $"shop deal modify {deal.DealSelector}",
					_ => $"shop deal cancel {deal.DealSelector}"
				};
				record.AccountName = deal.Operation == ShopDealAdministrationActionKind.Create ? deal.Name : deal.DealSelector;
				record.BoardText = SerializeActionPayload(new ShopDealAdministrationStepPayload(
					deal.Operation.ToString(),
					deal.Name,
					deal.DealType.ToString(),
					deal.TargetType.ToString(),
					deal.TargetSelector,
					deal.PriceAdjustmentPercentage,
					deal.MinimumQuantity,
					deal.Applicability.ToString(),
					deal.EligibilityProg?.Id,
					deal.IsCumulative,
					deal.Expiry.Date is null ? null : deal.Expiry.GetDateTimeString(),
					deal.DealSelector));
				break;
			case JobOpeningAdministrationActionStep opening:
				record.Description = $"{opening.Operation} job opening";
				record.AccountName = opening.OpeningId?.ToString("F0");
				record.BoardText = SerializeActionPayload(new JobOpeningAdministrationStepPayload(
					opening.Operation.ToString(),
					opening.OpeningId,
					opening.Definition is null ? null : FromJobOpeningDefinition(opening.Definition),
					opening.Reason));
				break;
			case ScheduledRuleAdministrationActionStep ruleAdmin:
				record.Description = $"{ruleAdmin.Operation} scheduled rule {ruleAdmin.RuleName}";
				record.AccountName = ruleAdmin.RuleId.ToString("D");
				record.BoardText = SerializeActionPayload(new ScheduledRuleAdministrationStepPayload(
					ruleAdmin.Operation.ToString(),
					ruleAdmin.RuleId.ToString("D"),
					ruleAdmin.RuleName,
					ruleAdmin.Reason,
					ruleAdmin.ManualKey));
				break;
			case ActiveTaskAdministrationActionStep taskAdmin:
				record.Description = $"{taskAdmin.Operation} active task {taskAdmin.TaskName}";
				record.AccountName = taskAdmin.TaskId.ToString("D");
				record.BoardText = SerializeActionPayload(new ActiveTaskAdministrationStepPayload(
					taskAdmin.Operation.ToString(),
					taskAdmin.TaskId.ToString("D"),
					taskAdmin.TaskName,
					taskAdmin.EmployeeId,
					taskAdmin.EmployeeName,
					taskAdmin.Reason));
				break;
			case ManagerGoalAdministrationActionStep goalAdmin:
				record.Description = $"{goalAdmin.Operation} manager goal {goalAdmin.GoalName}";
				record.AccountName = goalAdmin.GoalId.ToString("F0", CultureInfo.InvariantCulture);
				record.BoardText = SerializeActionPayload(new ManagerGoalAdministrationStepPayload(
					goalAdmin.Operation.ToString(),
					goalAdmin.GoalId,
					goalAdmin.GoalName,
					goalAdmin.Reason));
				break;
		}

		return record;
	}

	private static string SerializeActionPayload<T>(T payload)
	{
		return JsonSerializer.Serialize(payload);
	}

	private static T? TryDeserializeActionPayload<T>(string? text)
		where T : class
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		try
		{
			return JsonSerializer.Deserialize<T>(text);
		}
		catch (JsonException)
		{
			return null;
		}
	}

	private EmploymentScheduledTaskRule? ToScheduledRule(DbScheduledRule record, EmploymentActionPlan actionPlan)
	{
		return Guid.TryParse(record.PublicId, out var id)
			? new EmploymentScheduledTaskRule(
				id,
				_host,
				record.Name,
				record.IdempotencyKey,
				record.Conditions.OrderBy(x => x.SortOrder).Select(ToCondition).OfType<IEmploymentTaskCondition>(),
				TryDeserializeExpression(record.ExpressionJson),
				actionPlan,
				TimeSpan.FromTicks(record.CooldownTicks),
				ToNullableOffset(record.LastSpawnedAt),
				(EmploymentScheduledRuleStatus)record.Status)
			: null;
	}

	private EmploymentConditionPredicate? ToConditionPredicate(DbConditionPredicate record)
	{
		return Guid.TryParse(record.PublicId, out var id)
			? new EmploymentConditionPredicate(
				id,
				_host,
				record.Name,
				record.Conditions.OrderBy(x => x.SortOrder).Select(ToCondition).OfType<IEmploymentTaskCondition>(),
				TryDeserializeExpression(record.ExpressionJson))
			: null;
	}

	private EmploymentScheduledRuleTemplate? ToScheduledRuleTemplate(DbScheduledRuleTemplate record,
		EmploymentActionPlan actionPlan)
	{
		return Guid.TryParse(record.PublicId, out var id)
			? new EmploymentScheduledRuleTemplate(
				id,
				_host,
				record.Name,
				record.IdempotencyKeyPattern,
				record.Conditions.OrderBy(x => x.SortOrder).Select(ToCondition).OfType<IEmploymentTaskCondition>(),
				TryDeserializeExpression(record.ExpressionJson),
				actionPlan,
				TimeSpan.FromTicks(record.CooldownTicks))
			: null;
	}

	private EmploymentActiveTask? ToActiveTask(DbActiveTask record, EmploymentActionPlan actionPlan)
	{
		if (!Guid.TryParse(record.PublicId, out var id) || !Guid.TryParse(record.CorrelationId, out var correlationId))
		{
			return null;
		}

		var assigned = record.AssignedEmployeeId.HasValue
			? _gameworld.TryGetCharacter(record.AssignedEmployeeId.Value, true)
			: null;
		return new EmploymentActiveTask(
			id,
			_host,
			record.Name,
			actionPlan,
			(EmploymentTaskStatus)record.Status,
			assigned,
			record.BlockedReason,
			record.StepStates.OrderBy(x => x.SortOrder).Select(x => (EmploymentActionStepStatus)x.Status),
			record.StepStates.OrderBy(x => x.SortOrder).Select(ToOperationalState),
			correlationId,
			record.IdempotencyKey,
			this);
	}

	private ManagerGoal? ToManagerGoal(DbManagerGoal record, EmploymentActionPlan? actionPlan)
	{
		if (!Guid.TryParse(record.CorrelationId, out var correlationId))
		{
			correlationId = Guid.NewGuid();
		}

		return new ManagerGoal(
			record.RuntimeId,
			_host,
			(ManagerGoalType)record.GoalType,
			new EmploymentAuthoritySet((EmploymentAuthority)record.RequiredAuthority),
			(ManagerGoalStatus)record.Status,
			new ManagerGoalConfiguration(
				record.ConfigurationDescription,
				actionPlan,
				record.Conditions.OrderBy(x => x.SortOrder).Select(ToCondition).OfType<IEmploymentTaskCondition>().ToList()),
			record.Priority,
			TimeSpan.FromTicks(record.EvaluationCadenceTicks),
			ManagerGoalPolicy.Default,
			ToNullableOffset(record.LastEvaluatedAt),
			record.LastEvaluationResult,
			correlationId,
			this);
	}

	private IEmploymentTaskCondition? ToCondition(DbTaskCondition record)
	{
		return (EmploymentTaskConditionType)record.ConditionType switch
		{
			EmploymentTaskConditionType.ManualOrder =>
				new ManualOrderCondition(record.Key ?? string.Empty),
			EmploymentTaskConditionType.TimeWindow =>
				new TimeWindowCondition(
					TimeSpan.FromTicks(record.EarliestTicks ?? 0L),
					TimeSpan.FromTicks(record.LatestTicks ?? TimeSpan.TicksPerDay - 1)),
			EmploymentTaskConditionType.StockThreshold =>
				new StockThresholdCondition(record.Key ?? string.Empty, record.ThresholdInt ?? 0, record.BoolValue ?? true),
			EmploymentTaskConditionType.AccountBalance =>
				new AccountBalanceCondition(record.Key ?? string.Empty, record.ThresholdDecimal ?? 0.0M,
					record.BoolValue ?? true),
			EmploymentTaskConditionType.ItemThreshold =>
				new ItemThresholdCondition(record.Key ?? string.Empty, record.ThresholdInt ?? 0, record.BoolValue ?? true),
			EmploymentTaskConditionType.CommodityThreshold =>
				new CommodityThresholdCondition(record.Key ?? string.Empty, record.ThresholdDecimal ?? 0.0M,
					record.BoolValue ?? true),
			EmploymentTaskConditionType.ShopAccountOwing =>
				new ShopAccountOwingCondition(record.Key ?? string.Empty, record.ThresholdDecimal ?? 0.0M,
					record.BoolValue ?? true),
			EmploymentTaskConditionType.ShopFloatThreshold =>
				new ShopFloatThresholdCondition(record.Key ?? string.Empty, record.ThresholdDecimal ?? 0.0M,
					record.BoolValue ?? true),
			EmploymentTaskConditionType.WeatherLevel =>
				new WeatherLevelCondition(record.Key ?? string.Empty),
			EmploymentTaskConditionType.TaxOwing =>
				new TaxOwingCondition(record.ThresholdDecimal ?? 0.0M, record.BoolValue ?? true),
			EmploymentTaskConditionType.MarketPrice =>
				new MarketPriceCondition(record.Key ?? string.Empty, record.ThresholdDecimal ?? 0.0M,
					record.BoolValue ?? true),
			EmploymentTaskConditionType.PayrollLiability =>
				new PayrollLiabilityCondition(record.Key ?? string.Empty, record.ThresholdDecimal ?? 0.0M,
					record.BoolValue ?? true),
			EmploymentTaskConditionType.StaffingLevel =>
				new StaffingLevelCondition(record.Key ?? string.Empty, record.ThresholdInt ?? 0,
					record.BoolValue ?? true),
			EmploymentTaskConditionType.HospitalSupplyStock =>
				HospitalSupplyStockCondition.FromRecord(record.Key ?? string.Empty, record.ThresholdInt ?? 30,
					record.ThresholdDecimal),
			_ => null
		};
	}

	private static void AddConditions(ICollection<DbTaskCondition> target, IEnumerable<IEmploymentTaskCondition> conditions)
	{
		var order = 0;
		foreach (var condition in conditions)
		{
			target.Add(ToRecord(condition, order++));
		}
	}

	private static DbTaskCondition ToRecord(IEmploymentTaskCondition condition, int sortOrder)
	{
		var record = new DbTaskCondition
		{
			SortOrder = sortOrder,
			ConditionType = (int)condition.ConditionType
		};

		switch (condition)
		{
			case ManualOrderCondition manual:
				record.Key = manual.Key;
				break;
			case TimeWindowCondition time:
				record.EarliestTicks = time.EarliestTime.Ticks;
				record.LatestTicks = time.LatestTime.Ticks;
				break;
			case StockThresholdCondition stock:
				record.Key = stock.StockKey;
				record.ThresholdInt = stock.Threshold;
				record.BoolValue = stock.BelowThreshold;
				break;
			case AccountBalanceCondition account:
				record.Key = account.AccountKey;
				record.ThresholdDecimal = account.Threshold;
				record.BoolValue = account.BelowThreshold;
				break;
			case ItemThresholdCondition item:
				record.Key = item.ItemKey;
				record.ThresholdInt = item.Threshold;
				record.BoolValue = item.BelowThreshold;
				break;
			case CommodityThresholdCondition commodity:
				record.Key = commodity.CommodityKey;
				record.ThresholdDecimal = commodity.ThresholdWeight;
				record.BoolValue = commodity.BelowThreshold;
				break;
			case ShopAccountOwingCondition accountOwing:
				record.Key = accountOwing.AccountKey;
				record.ThresholdDecimal = accountOwing.Threshold;
				record.BoolValue = accountOwing.AboveThreshold;
				break;
			case ShopFloatThresholdCondition shopFloat:
				record.Key = shopFloat.FloatKey;
				record.ThresholdDecimal = shopFloat.Threshold;
				record.BoolValue = shopFloat.BelowThreshold;
				break;
			case WeatherLevelCondition weather:
				record.Key = weather.WeatherKey;
				break;
			case TaxOwingCondition tax:
				record.ThresholdDecimal = tax.Threshold;
				record.BoolValue = tax.AboveThreshold;
				break;
			case MarketPriceCondition marketPrice:
				record.Key = marketPrice.PriceKey;
				record.ThresholdDecimal = marketPrice.Threshold;
				record.BoolValue = marketPrice.AboveThreshold;
				break;
			case PayrollLiabilityCondition payroll:
				record.Key = payroll.Metric;
				record.ThresholdDecimal = payroll.Threshold;
				record.BoolValue = payroll.AboveThreshold;
				break;
			case StaffingLevelCondition staffing:
				record.Key = staffing.StaffingKey;
				record.ThresholdInt = staffing.Threshold;
				record.BoolValue = staffing.BelowThreshold;
				break;
			case HospitalSupplyStockCondition hospitalStock:
				record.Key = hospitalStock.Key;
				record.ThresholdInt = hospitalStock.ProcedureCount;
				record.ThresholdDecimal = hospitalStock.MaximumLineAmount;
				break;
		}

		return record;
	}

	private static string? SerializeExpression(EmploymentConditionExpression? expression)
	{
		return expression is null ? null : JsonSerializer.Serialize(expression);
	}

	private static EmploymentConditionExpression? TryDeserializeExpression(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		try
		{
			return JsonSerializer.Deserialize<EmploymentConditionExpression>(text);
		}
		catch (JsonException)
		{
			return null;
		}
	}

	private static EmploymentActionStepOperationalState ToOperationalState(DbActiveTaskStepState record)
	{
		return new EmploymentActionStepOperationalState(
			record.OperationalPayload,
			record.TransactionReference,
			record.SelectedResources,
			record.ReservationReference,
			record.RouteResult,
			record.CraftJobReference,
			record.LoadedAssets,
			record.FailureDiagnostic);
	}

	private static void AddStepStates(ICollection<DbActiveTaskStepState> target,
		IEnumerable<EmploymentActionStepStatus> states,
		IReadOnlyList<EmploymentActionStepOperationalState> operationalStates)
	{
		var order = 0;
		foreach (var state in states)
		{
			var operationalState = order < operationalStates.Count
				? operationalStates[order]
				: EmploymentActionStepOperationalState.Empty;
			target.Add(new DbActiveTaskStepState
			{
				SortOrder = order++,
				Status = (int)state,
				OperationalPayload = operationalState.OperationalPayload,
				TransactionReference = operationalState.TransactionReference,
				SelectedResources = operationalState.SelectedResources,
				ReservationReference = operationalState.ReservationReference,
				RouteResult = operationalState.RouteResult,
				CraftJobReference = operationalState.CraftJobReference,
				LoadedAssets = operationalState.LoadedAssets,
				FailureDiagnostic = operationalState.FailureDiagnostic
			});
		}
	}

	private EmploymentRegisterEntry ToRegisterEntry(DbRegisterEntry record)
	{
		return new EmploymentRegisterEntry(
			Guid.TryParse(record.CorrelationId, out var id) ? id : Guid.NewGuid(),
			(EmploymentRegisterEntryType)record.EntryType,
			_host,
			record.ActorId.HasValue ? _gameworld.TryGetCharacter(record.ActorId.Value, true) : null,
			record.Description,
			EmploymentClock.NormaliseLoadedInstant(_host, record.RecordedAt));
	}

	private EmploymentLedgerEntry ToLedgerEntry(DbLedgerEntry record)
	{
		return new EmploymentLedgerEntry(
			Guid.TryParse(record.CorrelationId, out var id) ? id : Guid.NewGuid(),
			(EmploymentLedgerEntryType)record.EntryType,
			_host,
			record.ActorId.HasValue ? _gameworld.TryGetCharacter(record.ActorId.Value, true) : null,
			ToMoney(record.AmountCurrencyId, record.Amount),
			record.Description,
			EmploymentClock.NormaliseLoadedInstant(_host, record.RecordedAt));
	}

	private static DateTimeOffset ToOffset(DateTime value)
	{
		return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
	}

	private static DateTimeOffset? ToNullableOffset(DateTime? value)
	{
		return value.HasValue ? ToOffset(value.Value) : null;
	}

	private void WithContext(Action<FuturemudDatabaseContext> action)
	{
		using (new FMDB())
		{
			action(FMDB.Context);
		}
	}

	private void Touch(FuturemudDatabaseContext context)
	{
		var state = context.EmploymentHostStates.Find(StateId);
		if (state is not null)
		{
			state.LastUpdatedAt = DateTime.UtcNow;
		}
	}
}
