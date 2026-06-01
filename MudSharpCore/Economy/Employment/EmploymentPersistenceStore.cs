using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Vehicles;
using DbActionPlan = MudSharp.Models.EmploymentActionPlanRecord;
using DbActionStep = MudSharp.Models.EmploymentActionStepRecord;
using DbActiveTask = MudSharp.Models.EmploymentActiveTaskRecord;
using DbActiveTaskStepState = MudSharp.Models.EmploymentActiveTaskStepStateRecord;
using DbApplication = MudSharp.Models.EmploymentApplicationRecord;
using DbContract = MudSharp.Models.EmploymentContractRecord;
using DbHostState = MudSharp.Models.EmploymentHostState;
using DbJobOpening = MudSharp.Models.EmploymentJobOpeningRecord;
using DbJobRequirement = MudSharp.Models.EmploymentJobOpeningRequirement;
using DbLedgerEntry = MudSharp.Models.EmploymentLedgerEntryRecord;
using DbManagerGoal = MudSharp.Models.EmploymentManagerGoalRecord;
using DbPayable = MudSharp.Models.EmploymentPayableRecord;
using DbRegisterEntry = MudSharp.Models.EmploymentRegisterEntryRecord;
using DbScheduledRule = MudSharp.Models.EmploymentScheduledTaskRuleRecord;
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
		public long[] ResolvedItemPrototypeIds => ItemPrototypeIds.Length > 0 ? ItemPrototypeIds : LegacyItemIds ?? [];
	}

	private sealed record GetItemsByTagStepPayload(int Quantity, string TagName, long[] SourceLocationIds);

	private sealed record GetCommodityStepPayload(double RequiredWeight, string MaterialName, string? TagName,
		Dictionary<string, string> Characteristics, long[] SourceLocationIds);

	private sealed record ItemSelectorPayload(string Kind, long? Id, string? Text);

	private sealed record DeliverItemsStepPayload(long DestinationCellId, long? ContainerId = null,
		string? ContainerTag = null, ItemSelectorPayload? ContainerSelector = null);

	private sealed record LoadItemsStepPayload(long? ContainerId = null, string? ContainerTag = null,
		long? TargetLocationId = null, ItemSelectorPayload? ContainerSelector = null);

	private sealed record UnloadItemsStepPayload(long? ContainerId = null, string? ContainerTag = null,
		long? SourceLocationId = null, ItemSelectorPayload? ContainerSelector = null);

	private sealed record ReturnAssetStepPayload(long? ContainerId, string? ContainerTag, long DestinationCellId,
		long? DestinationContainerId, string? DestinationContainerTag, ItemSelectorPayload? ContainerSelector = null,
		ItemSelectorPayload? DestinationContainerSelector = null);

	private sealed record VehicleOperationStepPayload(long VehicleId, long CargoSpaceId);

	private sealed record CataloguedActionShellPayload(string ActionKey, string Description, long? TargetLocationId,
		long? AmountCurrencyId = null, decimal? Amount = null);

	private sealed record PurchaseStepPayload(int Quantity, string MerchandiseSelector, string SupplierSelector,
		long CurrencyId, decimal? MaximumAmount, string? KeywordFilter);

	private sealed record ShopFloatAdjustmentStepPayload(bool FillRegister, long CurrencyId, decimal Amount,
		ItemSelectorPayload? RegisterSelector);

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

	public void SaveContract(EmploymentContract contract)
	{
		WithContext(context =>
		{
			if (context.EmploymentContracts.Any(x => x.RuntimeId == contract.Id && x.EmploymentHostStateId == StateId))
			{
				return;
			}

			context.EmploymentContracts.Add(ToRecord(contract));
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
				dbitem.Status = (int)contract.Status;
				dbitem.EndsAt = contract.EndsAt?.UtcDateTime;
				dbitem.EndReason = contract.EndReason.HasValue ? (int)contract.EndReason.Value : null;
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
				dbitem.Authority = (long)contract.Authority.Authorities;
			}

			Touch(context);
			context.SaveChanges();
		});
	}

	public void SaveJobOpening(JobOpening opening)
	{
		WithContext(context =>
		{
			if (context.EmploymentJobOpenings.Any(x => x.RuntimeId == opening.Id && x.EmploymentHostStateId == StateId))
			{
				return;
			}

			var dbitem = ToRecord(opening);
			context.EmploymentJobOpenings.Add(dbitem);
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
				Touch(context);
				context.SaveChanges();
				return;
			}

			context.EmploymentApplications.Add(new DbApplication
			{
				RuntimeId = application.Id,
				EmploymentJobOpeningId = opening.Id,
				CandidateId = application.Candidate.Id,
				AppliedAt = application.AppliedAt.UtcDateTime,
				Status = (int)application.Status,
				DecisionReason = application.DecisionReason
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
				ActorId = entry.Actor?.Id,
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
				ActorId = entry.Actor?.Id,
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
				AssignedEmployeeId = task.AssignedEmployee?.Id,
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
			dbitem.AssignedEmployeeId = task.AssignedEmployee?.Id;
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

		return new EmploymentHostState(
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
			managerGoals);
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
		return ResolveEconomicZone(host)?.FinancialPeriodReferenceCalendar?.Id;
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
			_ => null
		};
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
			_ => null
		};
	}

	private DbContract ToRecord(EmploymentContract contract)
	{
		var record = new DbContract
		{
			RuntimeId = contract.Id,
			EmploymentHostStateId = StateId,
			EmployeeId = contract.Employee.Id,
			Role = (int)contract.Role,
			Status = (int)contract.Status,
			Authority = (long)contract.Authority.Authorities,
			StartedAt = contract.StartedAt.UtcDateTime,
			EndsAt = contract.EndsAt?.UtcDateTime,
			EndReason = contract.EndReason.HasValue ? (int)contract.EndReason.Value : null
		};
		WriteTerms(record, contract.Compensation, contract.Schedule, contract.Duration, contract.PaymentMethod);
		return record;
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
			Authority = (long)opening.Authority.Authorities
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
			record.EndReason.HasValue ? (EmploymentTerminationReason)record.EndReason.Value : null);
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
			new EmploymentAuthoritySet((EmploymentAuthority)record.Authority));
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
			record.DecisionReason);
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
			EmploymentActionStepType.LoadItems =>
				ToLoadItemsStep(record),
			EmploymentActionStepType.UnloadItems =>
				ToUnloadItemsStep(record),
			EmploymentActionStepType.ReturnAsset =>
				ToReturnAssetStep(record),
			EmploymentActionStepType.VehicleOperation =>
				ToVehicleOperationStep(record),
			EmploymentActionStepType.CataloguedShell =>
				ToCataloguedShellStep(record, destination),
			EmploymentActionStepType.TaxPayment =>
				new TaxPaymentActionStep(amount),
			EmploymentActionStepType.ShopFloatAdjustment =>
				ToShopFloatAdjustmentStep(record),
			_ => null
		};
	}

	private PurchaseActionStep? ToPurchaseStep(PurchaseStepPayload payload, string? existingFinancialRecord)
	{
		var currency = _gameworld.Currencies.Get(payload.CurrencyId);
		if (currency is null)
		{
			return null;
		}

		var maximum = payload.MaximumAmount.HasValue ? new MoneyAmount(currency, payload.MaximumAmount.Value) : null;
		return new PurchaseActionStep(payload.Quantity, payload.MerchandiseSelector, payload.SupplierSelector,
			currency, maximum, payload.KeywordFilter, existingFinancialRecord);
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
		var cargo = vehicle?.CargoSpaces.FirstOrDefault(x => x.Id == payload.CargoSpaceId);
		return vehicle is null || cargo is null ? null : new VehicleOperationActionStep(vehicle, cargo);
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
		return new CataloguedActionShellStep(
			actionKey,
			payload?.Description ?? record.Description ?? actionKey,
			amount,
			destination);
	}

	private IEnumerable<ICell> ResolveCells(IEnumerable<long> ids)
	{
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
			case PurchaseActionStep purchase:
				record.Description = purchase.PurchaseDescription;
				record.AmountCurrencyId = purchase.Amount.Currency.Id;
				record.Amount = purchase.Amount.Amount;
				record.ExistingFinancialRecord = purchase.ExistingFinancialRecord;
				if (purchase.IsExecutablePurchase)
				{
					record.BoardText = SerializeActionPayload(new PurchaseStepPayload(
						purchase.Quantity!.Value,
						purchase.MerchandiseSelector!,
						purchase.SupplierSelector ?? "any",
						purchase.Amount.Currency.Id,
						purchase.MaximumAmount?.Amount,
						purchase.KeywordFilter));
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
				record.Description = $"select cargo {vehicle.CargoSpace.Name} on {vehicle.Vehicle.Name}";
				record.DestinationCellId = vehicle.Vehicle.Location?.Id;
				record.BoardText = SerializeActionPayload(new VehicleOperationStepPayload(
					vehicle.Vehicle.Id,
					vehicle.CargoSpace.Id));
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
					shell.Amount?.Amount));
				break;
			case TaxPaymentActionStep tax:
				record.Description = "pay supported host taxes";
				record.AmountCurrencyId = tax.MaximumAmount?.Currency.Id;
				record.Amount = tax.MaximumAmount?.Amount;
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
				actionPlan,
				TimeSpan.FromTicks(record.CooldownTicks),
				ToNullableOffset(record.LastSpawnedAt),
				(EmploymentScheduledRuleStatus)record.Status)
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
		}

		return record;
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
