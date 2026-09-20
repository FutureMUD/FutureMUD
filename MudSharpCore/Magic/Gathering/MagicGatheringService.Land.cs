#nullable enable

using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.Magic.Environment;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.Work.Agriculture;

namespace MudSharp.Magic.Gathering;

public sealed partial class MagicGatheringService
{
	private sealed record LandSourceState(string Selector, double Stock, NativeOrganicSourceSnapshot? Organic);
	private static string[] LandCommitKeys(MagicGatheringQuote quote) =>
		LandParticipantKeys(quote).Append("ecology").ToArray();

	private static string[] LandParticipantKeys(MagicGatheringQuote quote) =>
		quote.LandSources.Select(x => x.Selector)
			.Concat(quote.LandSources.Where(x => x.Lifecycle?.FieldId is not null)
				.Select(x => $"field:{x.Lifecycle!.FieldId}"))
			.Append(quote.CropHealthCost > 0.0 ? "crop" : "")
			.Append(quote.WoodlandHealthCost > 0.0 ? "woodland" : "")
			.Append(quote.CropHealthLifecycle?.FieldId is { } cropField ? $"field:{cropField}" : "")
			.Append(quote.WoodlandHealthLifecycle?.FieldId is { } woodField ? $"field:{woodField}" : "")
		.Where(x => x.Length > 0)
			.Distinct(StringComparer.Ordinal)
			.Order(StringComparer.Ordinal)
			.ToArray();

	private bool HasUnresolvedGatheringSource(long cellId, MagicGatheringQuote quote)
	{
		if (quote.Kind == MagicGatheringMethodKind.Gentle)
		{
			return quote.SourceResourceId is { } resourceId &&
			       (_store.HasUnresolvedForSource(cellId, resourceId) ||
			        _store.HasUnresolvedForParticipant(cellId, $"ambient:{resourceId}"));
		}
		return quote.Kind == MagicGatheringMethodKind.Land &&
		       (LandParticipantKeys(quote).Any(key => _store.HasUnresolvedForParticipant(cellId, key)) ||
		        quote.LandSources.Where(x => x.Selector.StartsWith("ambient:", StringComparison.Ordinal))
			        .Any(x => _store.HasUnresolvedForSource(cellId,
				        long.Parse(x.Selector[8..], CultureInfo.InvariantCulture))));
	}

	private sealed record LandReceiptDetail(int Version, string Stage,
		IReadOnlyList<MagicLandSourceAllocation> CapturedSources,
		IReadOnlyList<MagicLandSourceAllocation> AppliedSources,
		EnvironmentalMagicOperationRequest EcologicalRequest,
		EnvironmentalMagicOperationResult? EcologicalResult)
	{
		public IReadOnlyList<LandHealthChange> HealthChanges { get; init; } = [];
	}

	private sealed record LandHealthChange(string Selector, NativeOrganicLifecycleIdentity Lifecycle,
		int AppliedLoss, decimal DiscardedPrepaidFraction);

	private static IReadOnlyDictionary<string, double>? LandDetails(MagicGatheringReceipt? receipt)
	{
		if (receipt?.Kind != MagicGatheringMethodKind.Land || string.IsNullOrWhiteSpace(receipt.LandDetailJson))
		{
			return null;
		}
		LandReceiptDetail? detail;
		try
		{
			detail = JsonSerializer.Deserialize<LandReceiptDetail>(receipt.LandDetailJson);
		}
		catch (JsonException)
		{
			return null;
		}
		if (detail is null || detail.Version != 1) return null;
		var values = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
		{
			["version"] = detail.Version,
			["requestedCredit"] = receipt.RequestedAmount,
			["credited"] = receipt.DestinationCredited ? receipt.RequestedAmount : 0.0,
			["scar"] = detail.EcologicalResult?.AppliedDamage ?? detail.EcologicalRequest.Damage,
			["pressure"] = detail.EcologicalResult?.AppliedPressure ?? detail.EcologicalRequest.Pressure,
			["ecologyApplied"] = receipt.EcologicalApplied ? 1.0 : 0.0,
			["accountingPersisted"] = receipt.AccountingPersisted ? 1.0 : 0.0
		};
		foreach (MagicLandSourceAllocation source in detail.CapturedSources)
		{
			values[$"funding:{source.Selector}"] = source.FundingUnits;
			values[$"collateral:{source.Selector}"] = source.CollateralUnits;
			values[$"total:{source.Selector}"] = source.TotalUnits;
		}
		foreach (MagicLandSourceAllocation source in detail.AppliedSources)
		{
			values[$"paid:{source.Selector}"] = source.TotalUnits;
			if (source.Lifecycle is null) continue;
			values[$"wholeDebit:{source.Selector}"] = source.WholeNativeDebit;
			values[$"openingPrepaid:{source.Selector}"] = (double)source.OpeningPrepaidFraction;
			values[$"closingPrepaid:{source.Selector}"] = (double)source.ClosingPrepaidFraction;
		}
		foreach (LandHealthChange change in detail.HealthChanges)
		{
			values[$"healthLoss:{change.Selector}"] = change.AppliedLoss;
			values[$"discardedPrepaid:{change.Selector}"] = (double)change.DiscardedPrepaidFraction;
		}
		return values;
	}

	private MagicGatheringResult CommitLand(LiveOperation live)
	{
		MagicGatheringMethodDefinition? currentMethod = live.Capability.GatheringMethods
			.FirstOrDefault(x => x.Key == live.Method.Key);
		if (currentMethod is null)
		{
			return Refused("Land gathering cancelled because its method was removed.");
		}
		MagicGatheringResult fresh = Quote(live.Actor, live.Capability, currentMethod,
			live.Quote.RequestedAmount, out DirectHealthCostPlan? healthPlan, live.Action, live.Quote);
		if (!fresh.Success || fresh.Quote is not { } quote)
		{
			return Refused($"Land gathering cancelled before payment: {fresh.Message}");
		}
		if (!Equivalent(live.Quote, quote) || !EquivalentHealthPlan(live.HealthPlan, healthPlan) ||
		    WasCancelled(live) || !StillValid(live))
		{
			return Refused("Land gathering cancelled because its captured price, source or action changed.");
		}
		if (HasUnresolvedGatheringSource(live.Cell.Id, quote))
		{
			return Refused("A Land participant has an unresolved earlier operation.");
		}
		IEnvironmentalMagicService environmental = _gameworld.EnvironmentalMagic!;
		var nativePlans = new List<NativeOrganicDebitPlan>();
		foreach (MagicLandSourceAllocation allocation in quote.LandSources.Where(x => x.Lifecycle is not null))
		{
			if (!environmental.TryPlanOrganicDebit(live.Cell, allocation.Selector, allocation.TotalUnits,
			    out NativeOrganicDebitPlan plan, out string? error) || plan.Lifecycle != allocation.Lifecycle)
			{
				return Refused(error ?? "The captured native Land source is no longer eligible.");
			}
			nativePlans.Add(plan);
		}

		var childRequest = new EnvironmentalMagicOperationRequest(Guid.NewGuid(), live.Actor.Id,
			$"Land gathering {live.Method.Key}", quote.LandDamage, quote.LandPressure);
		LandReceiptDetail detail = new(1, "Prepared", quote.LandSources, [], childRequest, null);
		MagicGatheringReceipt receipt = NewReceipt(live, quote) with
		{
			EcologicalChildId = childRequest.OperationId,
			LandDetailJson = JsonSerializer.Serialize(detail),
			ParticipantKeys = LandParticipantKeys(quote)
		};
		if (!_store.TryCreate(receipt))
		{
			return Refused("The durable Land marker could not be saved; no source was debited.");
		}

		try
		{
			var appliedSources = new List<MagicLandSourceAllocation>();
			EnvironmentalLandAmbientDebit[] ambientDebits = quote.LandSources
				.Where(x => x.Selector.StartsWith("ambient:", StringComparison.Ordinal))
				.Select(x => new EnvironmentalLandAmbientDebit(
					_gameworld.MagicResources.Get(long.Parse(x.Selector[8..], CultureInfo.InvariantCulture))!,
					x.TotalUnits)).ToArray();
			bool groupSuccess = environmental.TryApplyLandDebitGroup(live.Cell, ambientDebits, nativePlans,
				out IReadOnlyList<long> paidAmbient, out IReadOnlyList<NativeOrganicDebitPlan> paidNative,
				out string? groupError);
			foreach (long resourceId in paidAmbient)
			{
				appliedSources.Add(quote.LandSources.Single(x => x.Selector == $"ambient:{resourceId}"));
			}
			foreach (NativeOrganicDebitPlan plan in paidNative)
			{
				MagicLandSourceAllocation allocation = quote.LandSources.Single(x => x.Selector == plan.Selector);
				appliedSources.Add(allocation with
				{
					NativeStock = plan.ExpectedNativeStock,
					OpeningPrepaidFraction = plan.OpeningPrepaidFraction,
					WholeNativeDebit = plan.WholeNativeDebit,
					ClosingPrepaidFraction = plan.ClosingPrepaidFraction
				});
			}
			detail = detail with { Stage = groupSuccess ? "SourcesDebited" : "SourcesUncertain",
				AppliedSources = appliedSources.ToArray() };
			receipt = receipt with { SourceDebited = appliedSources.Count > 0,
				LandDetailJson = JsonSerializer.Serialize(detail), UpdatedUtc = UtcNow };
			if (!groupSuccess)
			{
				return LandSourceRefusal(live, receipt, appliedSources.Count > 0,
					groupError ?? "The complete Land source group refused.");
			}
			if (!TryRecord(receipt))
			{
				return LandUnrecordedSourceProgress(live, receipt,
					"Land source group succeeded but receipt progress could not be saved.");
			}
			// Confirm the exact paid group and child identity before applying ecology.
			// Physical owners retain their existing post-child checkpoint boundary.
			detail = detail with { Stage = "EcologyPrepared" };
			receipt = receipt with { LandDetailJson = JsonSerializer.Serialize(detail), UpdatedUtc = UtcNow };
			if (!TryRecord(receipt))
			{
				return LandUnrecordedSourceProgress(live, receipt,
					"The paid Land group could not be acknowledged before ecology.");
			}
			var healthChanges = new List<LandHealthChange>();
			foreach (var health in new (NativeOrganicSourceKind Kind, double Loss,
				NativeOrganicLifecycleIdentity? Lifecycle)[]
			{
				(Kind: NativeOrganicSourceKind.Crop, Loss: quote.CropHealthCost,
					Lifecycle: quote.CropHealthLifecycle),
				(NativeOrganicSourceKind.Woodland, quote.WoodlandHealthCost,
					quote.WoodlandHealthLifecycle)
			})
			{
				if (health.Loss <= 0.0) continue;
				IAgricultureField? field = environmental.FieldFor(live.Cell);
				string healthError = "The indexed living field is no longer available.";
				int appliedLoss = 0;
				decimal discardedPrepaid = 0m;
				if (field is null || health.Lifecycle is null ||
				    !field.TryApplyLandHealthCost(health.Kind, health.Lifecycle, (int)health.Loss,
					    out appliedLoss, out discardedPrepaid, out healthError) ||
				    appliedLoss != (int)health.Loss)
				{
					return LandUnrecordedSourceProgress(live, receipt,
						$"Vegetation health payment became uncertain: {healthError}");
				}
				healthChanges.Add(new(NativeOrganicSourceSelectors.Canonical(health.Kind),
					health.Lifecycle, appliedLoss, discardedPrepaid));
				detail = detail with { Stage = "HealthApplied", HealthChanges = healthChanges.ToArray() };
				receipt = receipt with { LandDetailJson = JsonSerializer.Serialize(detail), UpdatedUtc = UtcNow };
				if (!TryRecord(receipt))
				{
					return LandUnrecordedSourceProgress(live, receipt,
						"Vegetation health loss succeeded but receipt progress could not be saved.");
				}
			}

			EnvironmentalMagicOperationResult ecological = environmental.ApplyOperation(live.Cell, childRequest);
			if (!ecological.Success)
			{
				return LandUnrecordedSourceProgress(live, receipt,
					$"The ecological child outcome is unconfirmed: {ecological.Error}");
			}
			detail = detail with { Stage = "EcologyApplied", EcologicalResult = ecological };
			receipt = receipt with { EcologicalApplied = true, LandDetailJson = JsonSerializer.Serialize(detail), UpdatedUtc = UtcNow };
			if (!TryRecord(receipt))
			{
				return LandUnrecordedSourceProgress(live, receipt,
					"Ecological child succeeded but receipt progress could not be saved.");
			}
			PersistLandSourceCheckpoint(live, receipt);
			if (!ApplyBodyCosts(live.Actor, quote, live.Method.MaximumHealthSeverity, healthPlan,
			    out IReadOnlyList<IWound> touchedWounds, out string? bodyError))
			{
				MarkNeedsReview(receipt, $"Bodily price could not be verified: {bodyError}");
				return Refused("Land price became unresolved; no personal credit was issued.");
			}
			PersistBodyCosts(live.Actor, touchedWounds);
			detail = detail with { Stage = "BodyPersisted" };
			receipt = receipt with { BodilyCostApplied = true, LandDetailJson = JsonSerializer.Serialize(detail), UpdatedUtc = UtcNow };
			if (!TryRecord(receipt))
			{
				MarkNeedsReview(receipt, "Bodily price succeeded but receipt progress could not be saved.");
				return Refused("Land bodily price is unresolved; no personal credit was issued.");
			}
			if (!CreditDestination(live.Actor, quote, out string? creditError))
			{
				MarkNeedsReview(receipt, $"Personal credit could not be verified: {creditError}");
				return Refused("Land personal credit is unresolved; no automatic retry is permitted.");
			}
			detail = detail with { Stage = "AccountingPersisted" };
			receipt = receipt with { DestinationCredited = true, AccountingPersisted = true,
				LandDetailJson = JsonSerializer.Serialize(detail), UpdatedUtc = UtcNow };
			PersistAccounting(live.Actor, live.Cell, receipt);
			return CompleteLandNotification(live, quote, receipt);
		}
		catch (Exception ex)
		{
			MarkNeedsReview(receipt, $"Land commitment became uncertain: {ex.Message}");
			return Refused("Land commitment is unresolved and will not be retried automatically; staff review is required.");
		}
	}

	private MagicGatheringResult LandSourceRefusal(LiveOperation live, MagicGatheringReceipt receipt,
		bool afterMutation, string error)
	{
		if (afterMutation) return LandUnrecordedSourceProgress(live, receipt, error);
		TryRecord(receipt with { Status = "Cancelled", Diagnostic = error, UpdatedUtc = UtcNow });
		return Refused($"Land gathering cancelled before payment: {error}");
	}

	private MagicGatheringResult LandUnrecordedSourceProgress(LiveOperation live,
		MagicGatheringReceipt receipt, string diagnostic)
	{
		MagicGatheringReceipt unresolved = receipt with
		{
			Status = "NeedsReview", Diagnostic = diagnostic, UpdatedUtc = UtcNow
		};
		try
		{
			PersistLandSourceCheckpoint(live, unresolved);
		}
		catch (Exception ex)
		{
			MarkNeedsReview(unresolved, $"{diagnostic} Source owner save also failed: {ex.Message}");
			return Refused("Land source accounting is unresolved; staff review is required.");
		}
		MarkNeedsReview(unresolved, diagnostic);
		return Refused("Land source accounting is unresolved; staff review is required.");
	}

	private void PersistLandSourceCheckpoint(LiveOperation live, MagicGatheringReceipt receipt)
	{
		if (_persistLandSources is not null)
		{
			if (!_persistLandSources(live.Actor, live.Cell, receipt))
			{
				live.Cell.Changed = true;
				throw new InvalidOperationException("Injected Land source checkpoint failed.");
			}
			return;
		}
		IAgricultureField? field = live.Quote.LandSources.Any(x => x.Lifecycle?.FieldId is not null) ||
			live.Quote.CropHealthCost > 0.0 || live.Quote.WoodlandHealthCost > 0.0
			? _gameworld.EnvironmentalMagic?.FieldFor(live.Cell) : null;
		var cell = live.Cell as Cell;
		try
		{
			using IDisposable? isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
			using (new FMDB())
			using (var transaction = FMDB.Context.Database.BeginTransaction())
			{
				cell?.PrepareForSaveAttempt();
				field?.Save();
				live.Cell.Save();
				MagicGatheringReceiptStore.WriteCurrent(receipt);
				FMDB.Context.SaveChanges();
				transaction.Commit();
			}
		}
		catch
		{
			cell?.RecoverFromSaveFailure();
			field?.Changed = true;
			live.Cell.Changed = true;
			throw;
		}
	}

	private MagicGatheringResult CompleteLandNotification(LiveOperation live, MagicGatheringQuote quote,
		MagicGatheringReceipt receipt)
	{
		EmitLandEmotes(live, live.Method.LandActorCompleteEmote, live.Method.LandObserverCompleteEmote);
		IFutureProg? callback = live.Method.OnGatheredProgId == 0 ? null : Prog(live.Method.OnGatheredProgId);
		if (callback is null)
		{
			MagicGatheringReceipt completed = receipt with { Status = "Completed", NotificationCompleted = true,
				UpdatedUtc = UtcNow };
			if (!TryRecord(completed))
			{
				MarkNeedsReview(completed, "Land accounting was saved but its final acknowledgement failed.");
				return new(true, $"{SuccessMessage(live, quote)} The notification receipt needs staff review.", live.Id, quote);
			}
			return new(true, SuccessMessage(live, quote), live.Id, quote);
		}
		receipt = receipt with { Status = "Invoking", UpdatedUtc = UtcNow };
		if (!TryRecord(receipt))
		{
			MarkNeedsReview(receipt, "Land accounting was saved but callback state could not be saved.");
			return new(true, $"{SuccessMessage(live, quote)} The callback needs staff review.", live.Id, quote);
		}
		try
		{
			if (!callback.ExecuteWithStatus(out _, live.Actor, live.Owner, live.Capability,
			    live.Method.Key.ToString(), quote.RequestedAmount, live.Cell, live.Id.ToString()))
			{
				throw new InvalidOperationException($"Prog #{callback.Id} reported execution failure.");
			}
			if (_gameworld.SaveManager.Flushing)
			{
				throw new InvalidOperationException("The callback ran during another save flush.");
			}
			_gameworld.SaveManager.Flush();
			if (_gameworld.VariableRegister.Changed || _gameworld.SaveManager.IsQueued(_gameworld.VariableRegister))
			{
				throw new InvalidOperationException("Callback register effects were not durably saved.");
			}
			MagicGatheringReceipt completed = receipt with { Status = "Completed", NotificationCompleted = true,
				UpdatedUtc = UtcNow };
			if (!TryRecord(completed))
			{
				MarkNeedsReview(completed, "Land callback ran but acknowledgement failed.");
				return new(true, $"{SuccessMessage(live, quote)} The callback needs staff review.", live.Id, quote);
			}
			return new(true, SuccessMessage(live, quote), live.Id, quote);
		}
		catch (Exception ex)
		{
			MarkNeedsReview(receipt, $"Land accounting completed but callback was not acknowledged: {ex.Message}");
			return new(true, $"{SuccessMessage(live, quote)} The callback needs staff review.", live.Id, quote);
		}
	}

	private static void EmitLandEmotes(LiveOperation live, string? actorMessage, string? observerMessage)
	{
		if (!string.IsNullOrWhiteSpace(actorMessage))
		{
			live.Actor.OutputHandler.Send(new EmoteOutput(new Emote(actorMessage, live.Actor, live.Actor)));
		}
		if (!string.IsNullOrWhiteSpace(observerMessage))
		{
			live.Actor.OutputHandler.Handle(new EmoteOutput(new Emote(observerMessage, live.Actor, live.Actor),
				flags: OutputFlags.SuppressSource), OutputRange.Local);
		}
	}

	private MagicGatheringResult QuoteLand(ICharacter actor, ICharacter owner, IMagicGatheringCapability capability,
		MagicGatheringMethodDefinition method, double amount, ICell cell, IMagicResource destination, double duration,
		double stamina, double damage, double pain, double stun, DirectHealthCostPlan? healthPlan,
		IReadOnlyList<MagicLandSourceAllocation>? captured = null)
	{
		IEnvironmentalMagicService? environmental = _gameworld.EnvironmentalMagic;
		if (environmental is null || method.LandSources.Count is < 1 or > 16)
		{
			return Refused("Land gathering requires one to sixteen configured local source entries and environmental accounting.");
		}
		EnvironmentalOrganicProfileSnapshot profileView = environmental.InspectOrganicProfile(cell);
		if (!profileView.ProfileId.HasValue || !profileView.HasOrganicConfiguration ||
		    profileView.HasPendingOperation || profileView.Errors.Count > 0)
		{
			return Refused("This land has no valid, opted-in environmental profile or has an unresolved ecological operation.");
		}
		IEnvironmentalMagicProfile? profile = _gameworld.MagicResourceRegenerators.Get(profileView.ProfileId.Value)
			as IEnvironmentalMagicProfile;
		if (profile is null || profile.Revision != profileView.Revision)
		{
			return Refused("The local environmental profile cannot be resolved safely.");
		}
		if (profile.OrganicProtectionProgId is { } protectionId)
		{
			IFutureProg? protection = profile.OrganicProtectionProg;
			if (protection?.Id != protectionId ||
			    !MagicGatheringPolicy.ValidSignature(protection, "permission") ||
			    protection.StaticType != FutureProgStaticType.NotStatic ||
			    !MagicGatheringPolicy.Permits(protection, actor, owner, capability, method.Key.ToString(), amount, cell))
			{
				return Refused("The land's protection policy does not permit this gathering action.");
			}
		}

		var states = new Dictionary<string, LandSourceState>(StringComparer.Ordinal);
		var fundingRatios = new Dictionary<string, double>(StringComparer.Ordinal);
		var prices = new List<(MagicLandSourceDefinition Entry, string Selector, double Ratio)>();
		var entryPrices = new List<MagicLandEntryPrice>();
		foreach (MagicLandSourceDefinition entry in method.LandSources)
		{
			if (!TryCanonicalLandSelector(entry.Selector, out string selector))
			{
				return Refused($"Land source '{entry.Selector}' has a malformed selector.");
			}
			if (selector.StartsWith("ambient:", StringComparison.Ordinal) &&
			    _store.HasUnresolvedForSource(cell.Id,
				    long.Parse(selector[8..], CultureInfo.InvariantCulture)))
			{
				return Refused($"Land ambient source {selector} has an unresolved gathering receipt.");
			}
			double ratio = entry.UnitsPerDestinationUnit;
			if (entry.RatioProgId != 0)
			{
				IFutureProg? ratioProg = Prog(entry.RatioProgId);
				if (!MagicGatheringPolicy.ValidSignature(ratioProg, "landratio") ||
				    ratioProg!.StaticType != FutureProgStaticType.NotStatic)
				{
					return Refused($"Land source {selector} has an invalid dynamic price policy.");
				}
				ratio = MagicGatheringPolicy.Number(ratioProg, ratio, "Land source ratio", actor, owner,
					capability, method.Key.ToString(), amount, cell, entry.Key.ToString());
			}
			if (!double.IsFinite(ratio) || ratio < 0.0 || (!entry.IsCollateral && ratio <= 0.0) ||
			    entry.IsCollateral && entry.AllowAbsent)
			{
				return Refused($"Land source {selector} has an invalid or optional mandatory price.");
			}
			if (!entry.IsCollateral && fundingRatios.TryGetValue(selector, out double earlierRatio) &&
			    !Same(earlierRatio, ratio))
			{
				return Refused($"Repeated Land funding source {selector} has conflicting conversion rates.");
			}
			if (!entry.IsCollateral) fundingRatios[selector] = ratio;
			prices.Add((entry, selector, ratio));
			entryPrices.Add(new(entry.Key, selector, ratio, entry.IsCollateral));
			if (states.TryGetValue(selector, out LandSourceState? existing))
			{
				if (existing.Organic?.Status == NativeOrganicSourceStatus.Absent && !entry.AllowAbsent)
				{
					return Refused($"Required Land source {selector} is absent.");
				}
				continue;
			}

			if (selector.StartsWith("ambient:", StringComparison.Ordinal))
			{
				long resourceId = long.Parse(selector[8..], CultureInfo.InvariantCulture);
				IMagicResource? resource = _gameworld.MagicResources.Get(resourceId);
				if (resource is null || !resource.ResourceType.HasFlag(MagicResourceType.LocationResource) ||
				    !environmental.TryInspectLandResource(cell, resource, out EnvironmentalResourceSnapshot output) ||
				    !output.IsValid || !double.IsFinite(output.Balance) || !double.IsFinite(output.Maximum) ||
				    output.Balance < 0.0 || output.Maximum < 0.0)
				{
					return Refused($"Land ambient source {selector} is not a valid managed output here.");
				}
				states.Add(selector, new(selector, Math.Min(output.Balance, output.Maximum), null));
				continue;
			}

			NativeOrganicSourceSnapshot native = environmental.InspectOrganicSource(cell, selector);
			if (!native.IsEligible || native.Lifecycle is null)
			{
				if (native.Status == NativeOrganicSourceStatus.Absent && entry.AllowAbsent && !entry.IsCollateral &&
				    native.EnvironmentalProfileId == profile.Id &&
				    profile.OrganicSources.Any(x => x.Selector.Equals(selector, StringComparison.OrdinalIgnoreCase)))
				{
					states.Add(selector, new(selector, 0.0, native));
					continue;
				}
				return Refused(native.Diagnostic ?? $"Native source {selector} is unavailable ({native.Status}).");
			}
			if (!double.IsFinite(native.ConvertibleStock) || native.ConvertibleStock < 0.0)
			{
				return Refused($"Native source {selector} has invalid accounting.");
			}
			states.Add(selector, new(selector, native.ConvertibleStock, native));
		}

		var remaining = states.ToDictionary(x => x.Key, x => x.Value.Stock, StringComparer.Ordinal);
		var funding = states.Keys.ToDictionary(x => x, _ => 0.0, StringComparer.Ordinal);
		var collateral = states.Keys.ToDictionary(x => x, _ => 0.0, StringComparer.Ordinal);
		foreach (var price in prices.Where(x => x.Entry.IsCollateral))
		{
			double required = amount * price.Ratio;
			if (!double.IsFinite(required) || required < 0.0 || required > remaining[price.Selector])
			{
				return Refused($"Mandatory Land collateral {price.Selector} is unavailable in full.");
			}
			remaining[price.Selector] -= required;
			collateral[price.Selector] += required;
		}
		double unfunded = amount;
		if (captured is null)
		{
			foreach (var price in prices.Where(x => !x.Entry.IsCollateral))
			{
				if (unfunded <= 0.0) break;
				double used = Math.Min(remaining[price.Selector], unfunded * price.Ratio);
				if (used <= 0.0) continue;
				if (!double.IsFinite(used)) return Refused("Land allocation exceeds the supported source range.");
				funding[price.Selector] += used;
				remaining[price.Selector] -= used;
				unfunded -= used / price.Ratio;
			}
		}
		else
		{
			foreach (MagicLandSourceAllocation saved in captured)
			{
				if (!states.TryGetValue(saved.Selector, out LandSourceState? state) ||
				    !Same(collateral[saved.Selector], saved.CollateralUnits) ||
				    saved.FundingUnits < 0.0 || saved.FundingUnits > remaining[saved.Selector] ||
				    state.Organic?.Lifecycle != saved.Lifecycle ||
				    saved.FundingUnits > 0.0 && !fundingRatios.ContainsKey(saved.Selector))
				{
					return Refused("A captured Land source is no longer eligible or affordable.");
				}
				funding[saved.Selector] = saved.FundingUnits;
				remaining[saved.Selector] -= saved.FundingUnits;
				if (saved.FundingUnits > 0.0)
				{
					unfunded -= saved.FundingUnits / fundingRatios[saved.Selector];
				}
			}
		}
		if (Math.Abs(unfunded) > 0.000000001)
		{
			return Refused("The declared Land sources cannot fund the full requested personal credit.");
		}

		var allocations = new List<MagicLandSourceAllocation>();
		foreach (LandSourceState state in states.Values)
		{
			double total = funding[state.Selector] + collateral[state.Selector];
			if (total <= 0.0) continue;
			if (state.Organic is null)
			{
				allocations.Add(new(state.Selector, null, funding[state.Selector], collateral[state.Selector],
					state.Stock, 0m, 0, 0m));
				continue;
			}
			if (!environmental.TryPlanOrganicDebit(cell, state.Selector, total,
				    out NativeOrganicDebitPlan nativePlan, out string? nativeError))
			{
				return Refused(nativeError ?? $"Native source {state.Selector} cannot fund its complete allocation.");
			}
			allocations.Add(new(state.Selector, nativePlan.Lifecycle, funding[state.Selector],
				collateral[state.Selector], nativePlan.ExpectedNativeStock, nativePlan.OpeningPrepaidFraction,
				nativePlan.WholeNativeDebit, nativePlan.ClosingPrepaidFraction));
		}
		if (captured is not null && !EquivalentLandAllocations(captured, allocations))
		{
			return Refused("The selected Land source mix or a vegetation lifecycle changed; start a new action.");
		}

		var sourceUnits = new Dictionary<string, IProgVariable>(StringComparer.OrdinalIgnoreCase);
		foreach (LandSourceState state in states.Values)
		{
			sourceUnits[$"funding:{state.Selector}"] = new NumberVariable(funding[state.Selector]);
			sourceUnits[$"collateral:{state.Selector}"] = new NumberVariable(collateral[state.Selector]);
			sourceUnits[$"total:{state.Selector}"] = new NumberVariable(funding[state.Selector] + collateral[state.Selector]);
		}
		var sourceDictionary = new DictionaryVariable(sourceUnits, ProgVariableTypes.Number);
		double scar = method.LandDamageProgId == 0 ? amount * method.LandDamagePerDestinationUnit :
			EvaluateLandEcologicalPrice(method.LandDamageProgId, "Land scar", actor, owner, capability,
				method, amount, cell, sourceDictionary);
		double pressure = method.LandPressureProgId == 0
			? amount * (method.LandPressurePerDestinationUnit ?? method.LandDamagePerDestinationUnit)
			: EvaluateLandEcologicalPrice(method.LandPressureProgId, "Land pressure", actor, owner, capability,
				method, amount, cell, sourceDictionary);
		EnvironmentalMagicStateSnapshot stateView = environmental.InspectState(cell);
		if (!double.IsFinite(scar) || scar <= 0.0 || !double.IsFinite(pressure) || pressure < 0.0)
		{
			return Refused("Land gathering requires a finite, positive ecological scar and valid pressure.");
		}
		if (!double.IsFinite(stateView.State.ScarDamage + scar) ||
		    stateView.State.ScarDamage + scar <= stateView.State.ScarDamage ||
		    !double.IsFinite(stateView.Pressure) || stateView.Pressure < 0.0)
		{
			return Refused("The quantified Land damage cannot be applied to the current ecological state.");
		}
		if (!TryPlanLandHealthCost(environmental, cell, NativeOrganicSourceKind.Crop,
			    method.CropHealthCostPerDestinationUnit, method.CropHealthCostProgId,
			    actor, owner, capability, method, amount,
			    out int cropHealth, out NativeOrganicLifecycleIdentity? cropLifecycle, out string? cropError))
		{
			return Refused(cropError!);
		}
		if (!TryPlanLandHealthCost(environmental, cell, NativeOrganicSourceKind.Woodland,
			    method.WoodlandHealthCostPerDestinationUnit, method.WoodlandHealthCostProgId,
			    actor, owner, capability, method, amount,
			    out int woodlandHealth, out NativeOrganicLifecycleIdentity? woodlandLifecycle, out string? woodlandError))
		{
			return Refused(woodlandError!);
		}
		MagicGatheringQuote quote = new(method.Key, method.StructuralVersion, MagicGatheringMethodKind.Land,
			destination.Id, null, cell.Id, profile.Id, profile.Revision, amount, 0.0, duration, stamina,
			method.MinimumStamina, damage, pain, stun, healthPlan?.Bodypart.Id,
			healthPlan?.ExistingWound is not null)
		{
			LandSources = allocations,
			LandEntryPrices = entryPrices,
			LandDamage = scar,
			LandPressure = pressure,
			CropHealthCost = cropHealth,
			WoodlandHealthCost = woodlandHealth,
			CropHealthLifecycle = cropLifecycle,
			WoodlandHealthLifecycle = woodlandLifecycle
		};
		return new(true, $"Destructive Land gathering: gain {amount:N2} personal units from " +
			$"{string.Join(", ", allocations.Select(x => $"{x.TotalUnits:N2} {x.Selector}"))}; " +
			$"scar {scar:N2}, pressure {pressure:N2}; body price stamina {stamina:N2}, damage {damage:N2}, " +
			$"pain {pain:N2}, stun {stun:N2}; duration {duration:N2} seconds.", null, quote);
	}

	private double EvaluateLandEcologicalPrice(long progId, string label, ICharacter actor, ICharacter owner,
		IMagicGatheringCapability capability, MagicGatheringMethodDefinition method, double amount, ICell cell,
		DictionaryVariable sourceUnits)
	{
		IFutureProg? prog = Prog(progId);
		if (!MagicGatheringPolicy.ValidSignature(prog, "landdamage") ||
		    prog!.StaticType != FutureProgStaticType.NotStatic)
		{
			throw new InvalidOperationException($"{label} has an invalid dynamic calculation policy.");
		}
		return MagicGatheringPolicy.Number(prog, 0.0, label, actor, owner, capability,
			method.Key.ToString(), amount, cell, sourceUnits);
	}

	private bool TryPlanLandHealthCost(IEnvironmentalMagicService environmental, ICell cell,
		NativeOrganicSourceKind kind, double rate, long progId, ICharacter actor, ICharacter owner,
		IMagicGatheringCapability capability, MagicGatheringMethodDefinition method, double amount,
		out int cost, out NativeOrganicLifecycleIdentity? lifecycle, out string? error)
	{
		cost = 0;
		lifecycle = null;
		error = null;
		if (progId != 0)
		{
			IFutureProg? prog = Prog(progId);
			if (!MagicGatheringPolicy.ValidSignature(prog, "landratio") ||
			    prog!.StaticType != FutureProgStaticType.NotStatic)
			{
				error = $"The {kind} health-cost policy is invalid.";
				return false;
			}
			rate = MagicGatheringPolicy.Number(prog, rate, $"{kind} health cost", actor, owner,
				capability, method.Key.ToString(), amount, cell, $"{kind.ToString().ToLowerInvariant()}-health");
		}
		double total = amount * rate;
		if (!double.IsFinite(total) || total < 0.0 || total > int.MaxValue)
		{
			error = $"The {kind} health cost is outside the supported native score range.";
			return false;
		}
		if (total == 0.0) return true;
		NativeOrganicSourceSnapshot source = environmental.InspectOrganicSource(cell,
			NativeOrganicSourceSelectors.Canonical(kind));
		IAgricultureField? field = environmental.FieldFor(cell);
		int health = kind == NativeOrganicSourceKind.Crop ? field?.CropHealth ?? 0 : field?.WoodlandHealth ?? 0;
		if (!source.IsEligible || source.Lifecycle is null || field is null || health <= 0)
		{
			error = source.Diagnostic ?? $"A living, authorised {kind} is required for this health cost.";
			return false;
		}
		cost = Math.Min(health, checked((int)Math.Ceiling(total)));
		lifecycle = source.Lifecycle;
		return true;
	}

	internal static bool TryCanonicalLandSelector(string input, out string selector)
	{
		selector = input.Trim().ToLowerInvariant();
		if (selector == "orchard") selector = "crop";
		if (selector is "crop" or "woodland" or "pasture") return true;
		if (selector.StartsWith("forage:", StringComparison.Ordinal))
		{
			return NativeOrganicSourceSelectors.IsValidForageKey(selector[7..]);
		}
		if (selector.StartsWith("ambient:", StringComparison.Ordinal) &&
		    long.TryParse(selector[8..], NumberStyles.None, CultureInfo.InvariantCulture, out long id) && id > 0)
		{
			selector = $"ambient:{id}";
			return true;
		}
		return false;
	}

	private static bool EquivalentLandAllocations(IReadOnlyList<MagicLandSourceAllocation> expected,
		IReadOnlyList<MagicLandSourceAllocation> actual) => expected.Count == actual.Count &&
		expected.Zip(actual).All(x => x.First.Selector == x.Second.Selector &&
			x.First.Lifecycle == x.Second.Lifecycle && Same(x.First.FundingUnits, x.Second.FundingUnits) &&
			Same(x.First.CollateralUnits, x.Second.CollateralUnits));
}
