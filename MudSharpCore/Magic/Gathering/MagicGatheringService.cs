#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic.Environment;

namespace MudSharp.Magic.Gathering;

/// <summary>
/// A world-local, capability-gated transfer service. Preparation stays entirely transient; the first durable
/// receipt is written immediately before any gathering-caused debit or bodily price.
/// </summary>
public sealed class MagicGatheringService : IMagicGatheringService
{
	private sealed class LiveOperation
	{
		public required Guid Id { get; init; }
		public required ICharacter Actor { get; init; }
		public required ICharacter Owner { get; init; }
		public required long BodyId { get; init; }
		public required IMagicGatheringCapability Capability { get; init; }
		public required MagicGatheringMethodDefinition Method { get; init; }
		public required ICell Cell { get; init; }
		public required SpatialLocation Location { get; init; }
		public required MagicGatheringQuote Quote { get; init; }
		public required long StartedTimestamp { get; init; }
		public MagicGatheringTimedAction? Action { get; set; }
		public bool Committing { get; set; }
		public bool Cancelled { get; set; }
	}

	private readonly IFuturemud _gameworld;
	private readonly IMagicGatheringReceiptStore _store;
	private readonly TimeProvider _clock;
	private readonly Func<ICharacter, ICell?, MagicGatheringReceipt, bool>? _persistAccounting;
	private readonly object _guard = new();
	private readonly Dictionary<Guid, LiveOperation> _liveById = [];
	private readonly Dictionary<long, LiveOperation> _liveByOwner = [];
	private readonly HashSet<long> _committingOwners = [];
	private readonly HashSet<(long CellId, long ResourceId)> _committingSources = [];

	public MagicGatheringService(IFuturemud gameworld, IMagicGatheringReceiptStore? store = null,
		TimeProvider? clock = null, Func<ICharacter, ICell?, MagicGatheringReceipt, bool>? persistAccounting = null)
	{
		_gameworld = gameworld;
		_store = store ?? new MagicGatheringReceiptStore();
		_clock = clock ?? TimeProvider.System;
		_persistAccounting = persistAccounting;
	}

	private DateTime UtcNow => _clock.GetUtcNow().UtcDateTime;

	public IReadOnlyList<MagicGatheringMethodView> Methods(ICharacter actor, IMagicGatheringCapability capability)
	{
		if (AccessError(actor, capability) is { } error)
		{
			return capability.GatheringMethods.Select(x => new MagicGatheringMethodView(x.Key, x.Alias, x.Name, x.Kind,
				x.MinimumAmount, x.MaximumAmount, error)).ToArray();
		}

		return capability.GatheringMethods.Select(method =>
		{
			MagicGatheringResult quote = Quote(actor, capability, method, method.MinimumAmount);
			return new MagicGatheringMethodView(method.Key, method.Alias, method.Name, method.Kind, method.MinimumAmount,
				method.MaximumAmount, quote.Success ? null : quote.Message);
		}).ToArray();
	}

	public MagicGatheringResult Preview(ICharacter actor, IMagicGatheringCapability capability, string method, double amount)
	{
		MagicGatheringMethodDefinition? definition = ResolveMethod(capability, method);
		return definition is null
			? Refused("There is no such gathering method on that capability.")
			: Quote(actor, capability, definition, amount);
	}

	public MagicGatheringResult Begin(ICharacter actor, IMagicGatheringCapability capability, string method, double amount)
	{
		MagicGatheringMethodDefinition? definition = ResolveMethod(capability, method);
		if (definition is null)
		{
			return Refused("There is no such gathering method on that capability.");
		}

		ICharacter owner = MagicGatheringPolicy.Owner(actor);
		lock (_guard)
		{
			if (_liveByOwner.ContainsKey(owner.Id) || _committingOwners.Contains(owner.Id))
			{
				return Refused("That identity already has a live gathering action.");
			}
		}

		if (_store.HasUnresolved(owner.Id))
		{
			return Refused("A previous gathering receipt needs staff review before this identity can gather again.");
		}

		MagicGatheringResult quoteResult = Quote(actor, capability, definition, amount);
		if (!quoteResult.Success || quoteResult.Quote is not { } quote)
		{
			return quoteResult;
		}

		if (actor.Location is not ICell cell)
		{
			return Refused("You must be physically located in a cell to begin gathering.");
		}
		if (quote.Kind == MagicGatheringMethodKind.Gentle && quote.SourceResourceId is { } sourceResourceId &&
			_store.HasUnresolvedForSource(cell.Id, sourceResourceId))
		{
			return Refused("That environmental source has an unresolved gathering receipt and is temporarily quarantined for staff review.");
		}

		LiveOperation live = new()
		{
			Id = Guid.NewGuid(),
			Actor = actor,
			Owner = owner,
			BodyId = actor.Body?.Id ?? 0,
			Capability = capability,
			Method = definition,
			Cell = cell,
			Location = actor.SpatialLocation,
			Quote = quote,
			StartedTimestamp = _clock.GetTimestamp()
		};

		lock (_guard)
		{
			if (_liveByOwner.ContainsKey(owner.Id) || _committingOwners.Contains(owner.Id))
			{
				return Refused("That identity already has a live gathering action.");
			}

			_liveByOwner.Add(owner.Id, live);
			_liveById.Add(live.Id, live);
		}

		try
		{
			TimeSpan duration = TimeSpan.FromSeconds(quote.DurationSeconds);
			live.Action = new MagicGatheringTimedAction(actor, live.Id, $"gathering {definition.Name}",
				() => actor.OutputHandler.Send(Complete(actor, live.Id).Message),
				() => CancelFromAction(live.Id),
				() => StillValid(live));
			actor.AddEffect(live.Action, duration);
			return new MagicGatheringResult(true,
				$"You begin gathering {definition.Name.ColourName()}. It will take {duration.Describe(actor)} if uninterrupted.",
				live.Id, quote);
		}
		catch (Exception ex)
		{
			RemoveLive(live);
			return Refused($"The gathering action could not begin: {ex.Message}");
		}
	}

	public MagicGatheringResult Complete(ICharacter actor, Guid operationId)
	{
		LiveOperation? live;
		lock (_guard)
		{
			if (!_liveById.TryGetValue(operationId, out live) || !ReferenceEquals(live.Actor, actor) ||
				live.Owner.Id != MagicGatheringPolicy.Owner(actor).Id)
			{
				return Refused("There is no live gathering operation with that token for this actor.");
			}

			if (live.Committing)
			{
				return Refused("That gathering operation is already committing.");
			}
			if (live.Cancelled)
			{
				return Refused("That gathering operation has already been cancelled.");
			}

			if (_clock.GetElapsedTime(live.StartedTimestamp) < TimeSpan.FromSeconds(live.Quote.DurationSeconds))
			{
				return Refused("That gathering operation has not completed its required duration.");
			}

			live.Committing = true;
			_committingOwners.Add(live.Owner.Id);
			if (live.Quote.Kind == MagicGatheringMethodKind.Gentle && live.Quote.SourceResourceId is { } source)
			{
				if (_committingSources.Contains((live.Cell.Id, source)))
				{
					live.Committing = false;
					_committingOwners.Remove(live.Owner.Id);
					return Refused("Another gathering operation is committing against that exact environmental source.");
				}
				_committingSources.Add((live.Cell.Id, source));
			}
		}

		try
		{
			live.Action?.FinishExternally();
			return Commit(live);
		}
		catch (Exception ex)
		{
			return Refused($"The gathering operation stopped before commitment: {ex.Message}");
		}
		finally
		{
			lock (_guard)
			{
				_committingOwners.Remove(live.Owner.Id);
				if (live.Quote.Kind == MagicGatheringMethodKind.Gentle && live.Quote.SourceResourceId is { } source)
				{
					_committingSources.Remove((live.Cell.Id, source));
				}
			}
			RemoveLive(live);
		}
	}

	public MagicGatheringResult Cancel(ICharacter actor, Guid? operationId = null)
	{
		ICharacter owner = MagicGatheringPolicy.Owner(actor);
		LiveOperation? live;
		lock (_guard)
		{
			if (!_liveByOwner.TryGetValue(owner.Id, out live) || (operationId.HasValue && live.Id != operationId.Value))
			{
				return Refused("You have no matching live gathering action to cancel.");
			}
			if (live.Committing)
			{
				return Refused("That gathering action has reached commitment and cannot be cancelled.");
			}
			live.Cancelled = true;
		}

		live.Action?.CancelExternally();
		RemoveLive(live);
		return new MagicGatheringResult(true, "Your gathering action has been cancelled without a gathering cost or credit.", live.Id);
	}

	public MagicGatheringOperationSummary? Operation(Guid operationId) => _store.Operation(operationId)?.Summary();

	public IReadOnlyList<MagicGatheringOperationSummary> UnresolvedOperations(long? ownerId = null) =>
		_store.Unresolved(ownerId).Select(x => x.Summary()).ToArray();

	public MagicGatheringResult Acknowledge(Guid operationId)
	{
		MagicGatheringReceipt? receipt = _store.Operation(operationId);
		if (receipt is null)
		{
			return Refused("There is no such gathering receipt.");
		}
		if (receipt.Status is not ("Committing" or "Invoking" or "NeedsReview"))
		{
			return Refused("That gathering receipt does not require acknowledgement.");
		}

		try
		{
			_store.Record(receipt with { Status = "Acknowledged", UpdatedUtc = UtcNow,
				Diagnostic = string.IsNullOrWhiteSpace(receipt.Diagnostic) ? "Staff acknowledgement; no payout or callback replayed." :
				$"{receipt.Diagnostic}\nStaff acknowledgement; no payout or callback replayed." });
			return new MagicGatheringResult(true, "The receipt has been acknowledged. No resource transfer or callback was replayed.", receipt.Id);
		}
		catch (Exception ex)
		{
			return Refused($"The receipt could not be acknowledged: {ex.Message}");
		}
	}

	private MagicGatheringResult Commit(LiveOperation live)
	{
		MagicGatheringResult fresh = Quote(live.Actor, live.Capability, live.Method, live.Quote.RequestedAmount);
		if (!fresh.Success || fresh.Quote is not { } quote)
		{
			return Refused($"Gathering cancelled before payment: {fresh.Message}");
		}
		if (!Equivalent(live.Quote, quote))
		{
			return Refused("Gathering cancelled because its configured price, duration, source or destination changed.");
		}
		if (WasCancelled(live) || !StillValid(live))
		{
			return Refused("Gathering cancelled because the captured body, capability or location is no longer valid.");
		}
		if (quote.Kind == MagicGatheringMethodKind.Gentle && quote.SourceResourceId is { } sourceResourceId &&
			_store.HasUnresolvedForSource(live.Cell.Id, sourceResourceId))
		{
			return Refused("Gathering cancelled because its environmental source has an unresolved transfer requiring staff review.");
		}

		MagicGatheringReceipt receipt = NewReceipt(live, quote);
		if (!_store.TryCreate(receipt))
		{
			return Refused("The gathering receipt could not be saved, so no resource was debited.");
		}

		try
		{
			string? recordError = null;
			if (quote.Kind == MagicGatheringMethodKind.Gentle)
			{
				IEnvironmentalMagicService? environmental = _gameworld.EnvironmentalMagic;
				IMagicResource? source = quote.SourceResourceId is { } sourceId ? _gameworld.MagicResources.Get(sourceId) : null;
				string? debitError = null;
				bool debited = environmental is not null && source is not null &&
					environmental.TryDebit(live.Cell, source, quote.SourceDebit, out debitError);
				if (!debited)
				{
					TryRecord(receipt with { Status = "Cancelled", UpdatedUtc = UtcNow,
						Diagnostic = debitError ?? "The environmental source refused the exact debit." });
					return Refused(debitError ?? "The environmental source no longer has the required recorded energy.");
				}

				receipt = receipt with { SourceDebited = true, UpdatedUtc = UtcNow };
				if (!TryRecord(receipt, out recordError))
				{
					MarkNeedsReview(receipt, $"Source debit succeeded but receipt progress could not be saved: {recordError}");
					return Refused("The source debit completed, but its receipt needs staff review before any further work.");
				}
			}

			if (!ApplyBodyCosts(live.Actor, quote, out string? bodyError))
			{
				MarkNeedsReview(receipt, $"Bodily price was not safely verified: {bodyError}");
				return Refused("The gathering price could not be safely verified; staff review is required and no credit was issued.");
			}

			receipt = receipt with { BodilyCostApplied = true, UpdatedUtc = UtcNow };
			if (!TryRecord(receipt, out recordError))
			{
				MarkNeedsReview(receipt, $"Bodily price was applied but receipt progress could not be saved: {recordError}");
				return Refused("The bodily price was applied, but the receipt needs staff review before any credit.");
			}

			if (!CreditDestination(live.Actor, quote, out string? creditError))
			{
				MarkNeedsReview(receipt, $"Destination credit was not exactly verified: {creditError}");
				return Refused("The destination credit could not be verified. Staff review is required; no automatic retry or refund will occur.");
			}

			receipt = receipt with { DestinationCredited = true, UpdatedUtc = UtcNow };
			PersistAccounting(live.Actor, quote.Kind == MagicGatheringMethodKind.Gentle ? live.Cell : null,
				receipt with { AccountingPersisted = true });
			receipt = receipt with { AccountingPersisted = true, UpdatedUtc = UtcNow };

			IFutureProg? callback = live.Method.OnGatheredProgId == 0 ? null : _gameworld.FutureProgs.Get(live.Method.OnGatheredProgId);
			if (callback is null)
			{
				MagicGatheringReceipt completed = receipt with { Status = "Completed", NotificationCompleted = true, UpdatedUtc = UtcNow };
				if (!TryRecord(completed, out recordError))
				{
					MarkNeedsReview(completed, $"Transfer accounting was saved but final receipt acknowledgement could not be saved: {recordError}");
					return new MagicGatheringResult(true, $"{SuccessMessage(live, quote)} Its final receipt acknowledgement needs staff review.", live.Id, quote);
				}

				return new MagicGatheringResult(true, SuccessMessage(live, quote), live.Id, quote);
			}

			receipt = receipt with { Status = "Invoking", UpdatedUtc = UtcNow };
			if (!TryRecord(receipt, out recordError))
			{
				MarkNeedsReview(receipt, $"Accounting was saved but callback state could not be saved: {recordError}");
				return new MagicGatheringResult(true, $"{SuccessMessage(live, quote)} The post-gather callback needs staff review.", live.Id, quote);
			}

			try
			{
				if (!callback.ExecuteWithStatus(out _, live.Actor, live.Owner, live.Capability, live.Method.Key.ToString(),
					quote.RequestedAmount, live.Cell, live.Id.ToString()))
				{
					throw new InvalidOperationException($"Prog #{callback.Id} reported execution failure.");
				}
				if (_gameworld.SaveManager.Flushing)
				{
					throw new InvalidOperationException("The post-gather callback ran during another save flush.");
				}
				_gameworld.SaveManager.Flush();
				if (_gameworld.VariableRegister.Changed || _gameworld.SaveManager.IsQueued(_gameworld.VariableRegister))
				{
					throw new InvalidOperationException("Post-gather register side effects were not durably saved.");
				}
				MagicGatheringReceipt completed = receipt with { Status = "Completed", NotificationCompleted = true, UpdatedUtc = UtcNow };
				if (!TryRecord(completed, out recordError))
				{
					MarkNeedsReview(completed, $"Post-gather callback ran but its final acknowledgement could not be saved: {recordError}");
					return new MagicGatheringResult(true, $"{SuccessMessage(live, quote)} The post-gather callback needs staff review.", live.Id, quote);
				}

				return new MagicGatheringResult(true, SuccessMessage(live, quote), live.Id, quote);
			}
			catch (Exception ex)
			{
				MarkNeedsReview(receipt, $"Transfer accounting completed but post-gather callback was not acknowledged: {ex.Message}");
				return new MagicGatheringResult(true, $"{SuccessMessage(live, quote)} The post-gather callback needs staff review.", live.Id, quote);
			}
		}
		catch (Exception ex)
		{
			MarkNeedsReview(receipt, $"Gathering commitment became uncertain after its durable marker: {ex.Message}");
			return Refused("The gathering commitment is unresolved and will not be retried automatically; staff review is required.");
		}
	}

	private MagicGatheringResult Quote(ICharacter actor, IMagicGatheringCapability capability,
		MagicGatheringMethodDefinition method, double amount)
	{
		try
		{
			if (AccessError(actor, capability) is { } access)
			{
				return Refused(access);
			}
			if (!double.IsFinite(amount) || amount <= 0.0 || amount < method.MinimumAmount || amount > method.MaximumAmount)
			{
				return Refused($"The amount must be finite and between {method.MinimumAmount:N2} and {method.MaximumAmount:N2}.");
			}
			if (method.Kind is not (MagicGatheringMethodKind.Self or MagicGatheringMethodKind.Gentle))
			{
				return Refused("That gathering method has an unsupported semantic kind.");
			}
			if (!ReferenceEquals(actor.Gameworld, _gameworld))
			{
				return Refused("The actor is not in this gameworld.");
			}
			if (actor.Body is null)
			{
				return Refused("You need a current body to gather magic.");
			}
			if (actor.Location is not ICell cell || !ReferenceEquals(cell.Gameworld, _gameworld))
			{
				return Refused("You must be physically located in a cell to gather.");
			}
			if (ActionError(actor) is { } actionError)
			{
				return Refused(actionError);
			}

			ICharacter owner = MagicGatheringPolicy.Owner(actor);
			object[] policyArguments = [actor, owner, capability, method.Key.ToString(), amount, cell];
			IFutureProg? permission = Prog(method.PermissionProgId);
			if (!MagicGatheringPolicy.Permits(permission, policyArguments))
			{
				return Refused("The configured gathering policy does not currently permit that method.");
			}

			double duration = MagicGatheringPolicy.Number(Prog(method.DurationProgId), method.DurationSeconds, "duration", policyArguments);
			double stamina = MagicGatheringPolicy.Number(Prog(method.StaminaCostProgId), method.StaminaCost, "stamina cost", policyArguments);
			double damage = MagicGatheringPolicy.Number(Prog(method.DamageCostProgId), method.DamageCost, "damage cost", policyArguments);
			double pain = MagicGatheringPolicy.Number(Prog(method.PainCostProgId), method.PainCost, "pain cost", policyArguments);
			double stun = MagicGatheringPolicy.Number(Prog(method.StunCostProgId), method.StunCost, "stun cost", policyArguments);
			if (!double.IsFinite(duration) || duration <= 0.0 || duration > TimeSpan.MaxValue.TotalSeconds)
			{
				return Refused("The configured duration must evaluate to a finite positive real-time duration.");
			}
			if (method.Kind == MagicGatheringMethodKind.Self && stamina <= 0.0 && damage <= 0.0 && pain <= 0.0 && stun <= 0.0)
			{
				return Refused("Self gathering requires an actual bodily price.");
			}

			IMagicResource? destination = _gameworld.MagicResources.Get(method.DestinationResourceId);
			if (destination is null || !destination.ResourceType.HasFlag(MagicResourceType.PlayerResource) ||
				!actor.MagicResources.Any(x => x.Id == destination.Id))
			{
				return Refused("The configured personal destination resource is unavailable to this actor.");
			}
			double current = Amount(actor.MagicResourceAmounts, destination);
			double cap = destination.ResourceCap(actor);
			if (!double.IsFinite(current) || !double.IsFinite(cap) || current < 0.0 || cap < current || cap - current < amount)
			{
				return Refused("You do not currently have enough safe destination-resource headroom for the full amount.");
			}
			if (!CanPayBodyCosts(actor, stamina, method.MinimumStamina, damage, pain, stun, method.MaximumHealthSeverity, out string? bodyError))
			{
				return Refused(bodyError!);
			}

			long? sourceId = null;
			long? profileId = null;
			long? profileRevision = null;
			double sourceDebit = 0.0;
			if (method.Kind == MagicGatheringMethodKind.Gentle)
			{
				IMagicResource? source = method.SourceResourceId is { } id ? _gameworld.MagicResources.Get(id) : null;
				IEnvironmentalMagicService? environmental = _gameworld.EnvironmentalMagic;
				if (source is null || !source.ResourceType.HasFlag(MagicResourceType.LocationResource) || environmental is null)
				{
					return Refused("The configured gentle source is unavailable.");
				}
				if (!double.IsFinite(method.SourceUnitsPerDestinationUnit) || method.SourceUnitsPerDestinationUnit <= 0.0 ||
					!double.IsFinite(sourceDebit = amount * method.SourceUnitsPerDestinationUnit) || sourceDebit <= 0.0)
				{
					return Refused("The configured Gentle source conversion ratio is invalid.");
				}

				EnvironmentalMagicSnapshot snapshot = environmental.Inspect(cell);
				if (!snapshot.IsValid || !snapshot.ProfileId.HasValue || !environmental.TryInspectResource(snapshot, source, out EnvironmentalResourceSnapshot output) ||
					!output.IsValid || !double.IsFinite(output.Balance) || !double.IsFinite(output.Maximum) || output.Balance < 0.0 || output.Maximum < 0.0 ||
					Math.Min(output.Balance, output.Maximum) < sourceDebit)
				{
					return Refused("The selected environmental source does not currently have enough valid recorded energy.");
				}

				sourceId = source.Id;
				profileId = snapshot.ProfileId;
				profileRevision = (_gameworld.MagicResourceRegenerators.Get(snapshot.ProfileId.Value) as IEnvironmentalMagicProfile)?.Revision;
			}

			return new MagicGatheringResult(true, QuoteMessage(method, amount, sourceDebit, stamina, damage, pain, stun, duration), null,
				new MagicGatheringQuote(method.Key, method.StructuralVersion, method.Kind, destination.Id, sourceId, cell.Id,
					profileId, profileRevision, amount, sourceDebit, duration, stamina, method.MinimumStamina, damage, pain, stun));
		}
		catch (Exception ex)
		{
			return Refused($"Gathering is unavailable: {ex.Message}");
		}
	}

	private bool StillValid(LiveOperation live)
	{
		if (!ReferenceEquals(live.Actor.Gameworld, _gameworld) || !ReferenceEquals(live.Cell.Gameworld, _gameworld) ||
			live.Actor.SpatialLocation != live.Location ||
			live.Actor.Body?.Id != live.BodyId || ActionError(live.Actor) is not null ||
			MagicGatheringPolicy.Owner(live.Actor).Id != live.Owner.Id)
		{
			return false;
		}

		return live.Actor.Capabilities.Any(x => ReferenceEquals(x, live.Capability)) &&
			live.Capability.GatheringMethods.Any(x => x.Key == live.Method.Key && x.StructuralVersion == live.Method.StructuralVersion);
	}

	private string? AccessError(ICharacter actor, IMagicGatheringCapability capability)
	{
		if (!actor.Capabilities.Any(x => ReferenceEquals(x, capability)))
		{
			return $"You do not currently have {capability.Name}.";
		}

		IReadOnlyList<string> errors = capability.GatheringConfigurationErrors();
		return errors.Count == 0 ? null : $"{capability.Name} gathering is disabled: {string.Join("; ", errors)}";
	}

	private static MagicGatheringMethodDefinition? ResolveMethod(IMagicGatheringCapability capability, string text)
	{
		if (Guid.TryParse(text, out Guid key))
		{
			return capability.GatheringMethods.FirstOrDefault(x => x.Key == key);
		}

		MagicGatheringMethodDefinition[] matches = capability.GatheringMethods.Where(x => x.Alias.EqualTo(text) ||
			x.Alias.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToArray();
		return matches.Length == 1 ? matches[0] : null;
	}

	private IFutureProg? Prog(long id) => id == 0 ? null : _gameworld.FutureProgs.Get(id);

	private static string? ActionError(ICharacter actor)
	{
		if (!actor.State.IsConscious() || actor.State.HasFlag(CharacterState.Sleeping) ||
			actor.State.HasFlag(CharacterState.Stasis) || actor.State.HasFlag(CharacterState.Paralysed))
		{
			return "You must be awake and physically able to gather magic.";
		}

		if (actor.Identity?.FocusedInstance is { } focused && !ReferenceEquals(focused, actor) && actor.IsPlayerCharacter)
		{
			return "You must focus on the acting instance before gathering magic.";
		}

		return actor.Combat is not null || actor.Movement is not null
			? "You must be stationary and out of combat before gathering magic."
			: null;
	}

	private static bool CanPayBodyCosts(ICharacter actor, double stamina, double minimumStamina, double damage,
		double pain, double stun, WoundSeverity maximumHealthSeverity, out string? error)
	{
		error = null;
		if (stamina > 0.0 && (!double.IsFinite(actor.CurrentStamina) || actor.CurrentStamina < stamina + minimumStamina || !actor.CanSpendStamina(stamina)))
		{
			error = "You do not have enough stamina while preserving the configured minimum remainder.";
			return false;
		}
		if (damage <= 0.0 && pain <= 0.0 && stun <= 0.0)
		{
			return true;
		}
		if (maximumHealthSeverity == WoundSeverity.None || actor.Body?.RandomBodypart is null || actor.HealthStrategy is null)
		{
			error = "Your current body cannot safely apply the configured health price.";
			return false;
		}

		try
		{
			double highest = Math.Max(damage, Math.Max(pain, stun));
			if (actor.HealthStrategy.GetSeverity(highest) > maximumHealthSeverity)
			{
				error = "The configured health price exceeds this health strategy's allowed severity.";
				return false;
			}
		}
		catch (Exception ex)
		{
			error = $"The health strategy could not validate the configured price: {ex.Message}";
			return false;
		}

		return true;
	}

	private static bool ApplyBodyCosts(ICharacter actor, MagicGatheringQuote quote, out string? error)
	{
		error = null;
		if (quote.StaminaCost > 0.0)
		{
			double before = actor.CurrentStamina;
			if (!actor.CanSpendStamina(quote.StaminaCost) || before < quote.StaminaCost + quote.MinimumStamina)
			{
				error = "Stamina is no longer available at the committed price.";
				return false;
			}
			actor.SpendStamina(quote.StaminaCost);
			if (!double.IsFinite(actor.CurrentStamina) || actor.CurrentStamina > before - quote.StaminaCost + 0.000001 || actor.CurrentStamina < quote.MinimumStamina - 0.000001)
			{
				error = "The stamina price did not apply with the required minimum remainder.";
				return false;
			}
		}

		if (quote.DamageCost <= 0.0 && quote.PainCost <= 0.0 && quote.StunCost <= 0.0)
		{
			return true;
		}

		IBodypart? bodypart = actor.Body?.RandomBodypart;
		if (bodypart is null)
		{
			error = "The captured body has no valid part for the health price.";
			return false;
		}
		(double Damage, double Pain, double Stun) beforeWounds = WoundTotals(actor);
		IEnumerable<IWound> applied = actor.SufferDamage(new Damage
		{
			// Cellular damage takes the native direct-health route and is not armour-negatable.
			DamageType = DamageType.Cellular,
			DamageAmount = quote.DamageCost,
			PainAmount = quote.PainCost,
			StunAmount = quote.StunCost,
			Bodypart = bodypart,
			ActorOrigin = actor
		}).ToArray();
		(double Damage, double Pain, double Stun) afterWounds = WoundTotals(actor);
		if (!applied.Any() || afterWounds.Damage < beforeWounds.Damage + quote.DamageCost - 0.000001 ||
			afterWounds.Pain < beforeWounds.Pain + quote.PainCost - 0.000001 ||
			afterWounds.Stun < beforeWounds.Stun + quote.StunCost - 0.000001)
		{
			error = "The native health strategy did not apply every mandatory damage, pain and stun channel.";
			return false;
		}

		return true;
	}

	private static (double Damage, double Pain, double Stun) WoundTotals(ICharacter actor) =>
		(actor.Wounds.Sum(x => x.CurrentDamage), actor.Wounds.Sum(x => x.CurrentPain), actor.Wounds.Sum(x => x.CurrentStun));

	private bool CreditDestination(ICharacter actor, MagicGatheringQuote quote, out string? error)
	{
		error = null;
		IMagicResource? destination = _gameworld.MagicResources.Get(quote.DestinationResourceId);
		if (destination is null)
		{
			error = "The configured destination resource no longer exists.";
			return false;
		}

		double before = Amount(actor.MagicResourceAmounts, destination);
		double cap = destination.ResourceCap(actor);
		if (!double.IsFinite(before) || !double.IsFinite(cap) || cap - before < quote.RequestedAmount)
		{
			error = "The destination no longer has enough exact headroom.";
			return false;
		}
		actor.AddResource(destination, quote.RequestedAmount);
		double after = Amount(actor.MagicResourceAmounts, destination);
		if (!double.IsFinite(after) || Math.Abs((after - before) - quote.RequestedAmount) > 0.000001)
		{
			error = "The destination holder declined or clamped the requested credit.";
			return false;
		}

		return true;
	}

	private void PersistAccounting(ICharacter actor, ICell? cell, MagicGatheringReceipt receipt)
	{
		if (_persistAccounting is not null)
		{
			if (!_persistAccounting(actor, cell, receipt))
			{
				throw new InvalidOperationException("Injected gathering accounting persistence failed.");
			}
			return;
		}

		using IDisposable? isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
		using (new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction())
		{
			try
			{
				actor.Body.Save();
				actor.Save();
				cell?.Save();
				MagicGatheringReceiptStore.WriteCurrent(receipt);
				FMDB.Context.SaveChanges();
				transaction.Commit();
			}
			catch
			{
				actor.Body.Changed = true;
				actor.Changed = true;
				if (cell is not null)
				{
					cell.Changed = true;
				}
				throw;
			}
		}
	}

	private MagicGatheringReceipt NewReceipt(LiveOperation live, MagicGatheringQuote quote) => new(
		live.Id, live.Owner.Id, live.Actor.Id, live.BodyId, live.Capability.Id, live.Method.Key, live.Method.StructuralVersion,
		quote.Kind, quote.SourceCellId, quote.SourceProfileId, quote.SourceProfileRevision, quote.SourceResourceId,
		quote.DestinationResourceId, quote.RequestedAmount, quote.SourceDebit, quote.StaminaCost, quote.DamageCost,
		quote.PainCost, quote.StunCost, false, false, false, false, false, "Committing", UtcNow);

	private static double Amount(IReadOnlyDictionary<IMagicResource, double> amounts, IMagicResource resource) =>
		amounts.TryGetValue(resource, out double value) ? value : 0.0;

	private static bool Equivalent(MagicGatheringQuote expected, MagicGatheringQuote actual) =>
		expected.MethodKey == actual.MethodKey && expected.MethodVersion == actual.MethodVersion && expected.Kind == actual.Kind &&
		expected.DestinationResourceId == actual.DestinationResourceId && expected.SourceResourceId == actual.SourceResourceId &&
		expected.SourceCellId == actual.SourceCellId && expected.SourceProfileId == actual.SourceProfileId &&
		expected.SourceProfileRevision == actual.SourceProfileRevision &&
		Same(expected.RequestedAmount, actual.RequestedAmount) && Same(expected.SourceDebit, actual.SourceDebit) &&
		Same(expected.DurationSeconds, actual.DurationSeconds) && Same(expected.StaminaCost, actual.StaminaCost) &&
		Same(expected.MinimumStamina, actual.MinimumStamina) && Same(expected.DamageCost, actual.DamageCost) &&
		Same(expected.PainCost, actual.PainCost) && Same(expected.StunCost, actual.StunCost);

	private static bool Same(double left, double right) => Math.Abs(left - right) <= 0.000001;

	private static string QuoteMessage(MagicGatheringMethodDefinition method, double amount, double source, double stamina,
		double damage, double pain, double stun, double duration) =>
		$"{method.Name}: gain {amount:N2}; " +
		(method.Kind == MagicGatheringMethodKind.Gentle ? $"exact environmental debit {source:N2}; " : string.Empty) +
		$"body price stamina {stamina:N2}, damage {damage:N2}, pain {pain:N2}, stun {stun:N2}; duration {duration:N2} seconds.";

	private string SuccessMessage(LiveOperation live, MagicGatheringQuote quote)
	{
		string destination = _gameworld.MagicResources.Get(quote.DestinationResourceId)?.Name ?? "magic resource";
		return $"You complete {live.Method.Name.ColourName()} and gain {quote.RequestedAmount:N2} {destination.ColourValue()}.";
	}

	private static MagicGatheringResult Refused(string message) => new(false, message);

	private void CancelFromAction(Guid id)
	{
		lock (_guard)
		{
			if (_liveById.TryGetValue(id, out LiveOperation? live))
			{
				live.Cancelled = true;
				if (!live.Committing)
				{
					_liveById.Remove(id);
					_liveByOwner.Remove(live.Owner.Id);
				}
			}
		}
	}

	private bool WasCancelled(LiveOperation live)
	{
		lock (_guard)
		{
			return live.Cancelled;
		}
	}

	private void RemoveLive(LiveOperation live)
	{
		lock (_guard)
		{
			if (_liveById.TryGetValue(live.Id, out LiveOperation? current) && ReferenceEquals(current, live))
			{
				_liveById.Remove(live.Id);
			}
			if (_liveByOwner.TryGetValue(live.Owner.Id, out current) && ReferenceEquals(current, live))
			{
				_liveByOwner.Remove(live.Owner.Id);
			}
		}
	}

	private bool TryRecord(MagicGatheringReceipt receipt) => TryRecord(receipt, out _);

	private bool TryRecord(MagicGatheringReceipt receipt, out string? error)
	{
		try
		{
			_store.Record(receipt);
			error = null;
			return true;
		}
		catch (Exception ex)
		{
			error = ex.Message;
			return false;
		}
	}

	private void MarkNeedsReview(MagicGatheringReceipt receipt, string diagnostic)
	{
		TryRecord(receipt with { Status = "NeedsReview", Diagnostic = diagnostic, UpdatedUtc = UtcNow });
	}
}
