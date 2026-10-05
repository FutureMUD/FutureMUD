using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Vancian;
using MudSharp.Planes;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.RPG.Checks;
using System.Collections.Concurrent;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using Microsoft.EntityFrameworkCore;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private readonly object _deviceGuard = new();
	private readonly Dictionary<Guid, DeviceWork> _deviceWork = [];
	internal Action<IGameItemComponent>? DevicePersistence { get; set; }
	private sealed record DeviceFocusUse(ChargedMagicDeviceGameItemComponent Device, long ActorId, long BodyId, long OwnerId,
		IGameItemComponentProto Prototype, string Configuration);
	private readonly ConcurrentDictionary<MagicCastingIntent, DeviceFocusUse> _deviceFocus = new(ReferenceEqualityComparer.Instance);
	private sealed record DeviceQuote(IMagicCastingCapability Capability, MagicSpell Spell, ITraitDefinition Trait,
		StoredSpellSnapshot Snapshot, IReadOnlyList<CastingPayment> Payments, string Configuration, double Raw);
	private sealed record DeviceWork(ICharacter Actor, ChargedMagicDeviceGameItemComponent Device, DeviceQuote Quote,
		CastingOperation Operation, int Grade, int Count, DateTime Deadline, object Location, string ItemConfiguration)
	{
		public VancianTimedAction? Action { get; set; }
	}
	internal void PersistDevice(ChargedMagicDeviceGameItemComponent device)
	{
		if (DevicePersistence is { } persist) { persist(device); return; }
		if (FMDB.WritesAreSuppressed) throw new InvalidOperationException("Device persistence cannot escape a read-only database scope.");
		_world.SaveManager.Abort(device);
		var definition = device.Export().ToString();
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		if (device.Id <= 0 || FMDB.Context.Database.ExecuteSqlInterpolated($"UPDATE GameItemComponents SET Definition={definition} WHERE Id={device.Id} AND BINARY Definition=BINARY {device.PersistedDefinition}") != 1)
			throw new InvalidOperationException("The durable device bank changed concurrently or is not yet persisted; staff review is required.");
		device.AcceptPersistedDefinition(definition);
	}
	private static readonly HashSet<string> DeviceEffects = new(StringComparer.OrdinalIgnoreCase)
	{
		"damage", "heal", "mend", "boost", "glow", "blindness", "removeblindness", "cureblindness",
		"deafness", "invisibility", "removeinvisibility", "dispelinvisibility", "silence", "removesilence",
		"sleep", "removesleep", "paralysis", "removeparalysis", "waterbreathing", "removewaterbreathing"
	};
	private static void ValidateDeviceSpell(MagicSpell spell)
	{
		var errors = ScrollSpellCompatibility.Errors(spell, false).ToList();
		foreach (var effect in spell.SpellEffects.Concat(spell.CasterSpellEffects))
			if (!DeviceEffects.Contains((string?)effect.SaveToXml().Attribute("type") ?? "")) errors.Add("This effect has no charged-device carrier adapter.");
		if (!SpellTargetCapture.SupportsCompleteSpecification(spell.Trigger)) errors.Add("Unsupported charged-device target adapter.");
		if (errors.Count != 0) throw new InvalidOperationException(string.Join("\n", errors));
	}
	private string? DeviceItemError(ICharacter actor, ChargedMagicDeviceGameItemComponent device, Guid? continuing = null)
	{
		if (device.DataError is { } error) return $"Device data is disabled: {error}";
		var proto = (ChargedMagicDeviceGameItemComponentProto)device.Prototype;
		if (proto.ConfigurationErrors().FirstOrDefault() is { } configuration) return configuration;
		if (device.Charges > device.Capacity) return "The charge bank exceeds its revised capacity; staff review is required.";
		if (!ReferenceEquals(device.Parent.GetItemType<IChargedMagicDevice>(), device) || actor.Body is null || actor.Location is null || !actor.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, device.Parent)) ||
			device.Parent.Deleted || !actor.CanSee(device.Parent) || !actor.CanManipulateItem(device.Parent).Truth)
			return "Hold or wield a visible, manipulable device in your current body.";
		if (device.Parent.GetItemType<IStackable>() is not null || device.Parent.GetItemType<IContainer>() is not null ||
			device.Parent.GetItemType<ISpellScroll>() is not null || device.Parent.GetItemType<ISpellbook>() is not null)
			return "A charged device cannot also be stackable, a container, a scroll or a spellbook.";
		if (!VancianPolicy.Permits(_world.FutureProgs.Get(proto.UseProgId), proto.UseProgId == 0, actor, device.Parent)) return "The item usability policy refuses this body.";
		if (device.Reservation is { } reservation && reservation != continuing) return "This item is reserved; persisted incomplete work requires staff review.";
		return CastingQuarantineReason(actor, itemIds: [device.Parent.Id], continuingOperation: continuing);
	}
	private bool CurrentDeviceCaster(ICharacter actor, IMagicCapability capability)
	{
		if (!ReferenceEquals(_world.MagicCapabilities.Get(capability.Id), capability)) return false;
		if (capability is IMagicCastingCapability { HasCastingPolicy: true } configured)
			return configured.CastingPolicy is { Enabled: true } && configured.CastingConfigurationErrors().Count == 0;
		// Other established routes keep their own legitimate runtime capability; an item is never a capability.
		return capability is IVancianMagicCapability vancian ? VancianMagicService.For(_world).AccessError(actor, vancian) is null : true;
	}
	private string? DeviceEligibilityError(ICharacter actor, ChargedMagicDeviceGameItemComponent device, long? focusSpell = null)
	{
		var proto = (ChargedMagicDeviceGameItemComponentProto)device.Prototype;
		var casters = actor.Capabilities.Where(x => CurrentDeviceCaster(actor, x)).ToArray();
		if (proto.MinimumUseGrade > 0 && ((focusSpell ?? device.SpellId) is not { } requiredSpell || !casters.OfType<IMagicCastingCapability>()
			.Any(x => x.CastingPolicy is { Enabled: true } policy && policy.Admissions.Any(a => a.SpellId == requiredSpell) &&
				Preflight(actor, x.Id, requiredSpell, Math.Max(aMinimum(x, requiredSpell), proto.MinimumUseGrade), false) is null)))
			return "This device requires its additional currently acquired controlled grade.";
		return proto.Eligibility switch
		{
			MagicDeviceEligibility.Anyone => null,
			MagicDeviceEligibility.Caster => casters.Length > 0 ? null : "This device requires a currently entitled caster.",
			MagicDeviceEligibility.MagicType => casters.Any(x => x.Id == proto.CapabilityId) ? null : "This device requires its configured magic type.",
			MagicDeviceEligibility.AcquiredSpell => (focusSpell ?? device.SpellId) is { } spell && casters.OfType<IMagicCastingCapability>().Where(x => x.CastingPolicy is { Enabled: true })
				.Any(x => x.CastingPolicy!.Admissions.Any(a => a.SpellId == spell) && Preflight(actor, x.Id, spell, aMinimum(x, spell), false) is null)
				? null : "This device requires a current admitted acquired-spell route.",
			_ => "Unsupported activation eligibility."
		};
		static int aMinimum(IMagicCastingCapability capability, long spell) => capability.CastingPolicy!.Admissions.Single(x => x.SpellId == spell).MinimumGrade;
	}
	private DeviceQuote QuoteDeviceProduction(ICharacter actor, ChargedMagicDeviceGameItemComponent device, long capabilityId,
		long spellId, int grade, int count, Guid? continuing = null)
	{
		if (VancianMagicService.ActionError(actor) is { } physical) throw new InvalidOperationException(physical);
		if (DeviceItemError(actor, device, continuing) is { } itemError) throw new InvalidOperationException(itemError);
		if (ResolveRoute(actor, capabilityId, spellId, grade, false, out var capability, out _, out var spell, out var trait,
			out var difficulty, continuingOperation: continuing) is { } route) throw new InvalidOperationException(route);
		var proto = (ChargedMagicDeviceGameItemComponentProto)device.Prototype;
		if (proto.Role == MagicDeviceRole.Focus || !proto.Spells.Contains(spellId)) throw new InvalidOperationException("This carrier does not explicitly allow that charged payload.");
		if (count < 1 || count > device.Capacity - device.Charges) throw new InvalidOperationException("Only missing charge capacity can be filled.");
		ValidateDeviceSpell(spell);
		var acquired = Acquisition(actor, spellId)!;
		var power = spell.GradeProfile!.Grades.Single(x => x.Grade == grade).Power;
		var snapshot = StoredSpellSnapshot.CaptureDevice(spell, actor, trait, grade, acquired.ControlledGrade, power, _clock());
		var raw = Owner(actor).TraitRawValue(trait);
		if (!double.IsFinite(raw) || raw < 0) throw new InvalidOperationException("Invalid raw producer proficiency.");
		if (device.Charges != 0)
		{
			if (raw < device.RequiredRaw || !snapshot.CanReproduceDevice(device.Snapshot!, _world))
				throw new InvalidOperationException("You cannot prove at least this bank's frozen configuration and potency; deplete it before changing payload.");
			// Qualifying stronger production tops up the original bank, preserving its homogeneous stored potency.
			snapshot = device.Snapshot!;
		}
		var copy = spell.CastingCopy(actor, trait, grade, power, difficulty, acquired.ControlledGrade);
		List<CastingPayment> payments = [];
		var policy = capability.CastingPolicy!;
		foreach (var (resource, expression) in copy.CastingCosts)
		{
			var amount = resource.Id == policy.SourceResourceId && spell.GradeProfile.Efficiency is { } efficiency
				? efficiency.Cost(acquired.ControlledGrade, grade) : expression.EvaluateWith(actor, trait, TraitBonusContext.SpellCost, ("self", 0));
			amount *= count;
			var destination = resource.Id == policy.SourceResourceId ? _world.MagicResources.Get(policy.ReserveResourceId)! : resource;
			var holder = ReserveHolder(actor, destination.Id);
			if (!double.IsFinite(amount) || amount < 0 || !MagicResourceCapacity.TryGetCap(destination, holder, out var cap, out _) ||
				!double.IsFinite(holder.MagicResourceAmounts.GetValueOrDefault(destination)) || Math.Min(cap, holder.MagicResourceAmounts.GetValueOrDefault(destination)) < amount ||
				!holder.CanUseResource(destination, amount)) throw new InvalidOperationException("Insufficient or invalid production reserve.");
			if (CastingQuarantineReason(actor, reserveId: destination.Id, continuingOperation: continuing) is { } quarantine) throw new InvalidOperationException(quarantine);
			payments.Add(new(holder, destination, amount));
		}
		payments = payments.GroupBy(x => (x.Holder.Id, x.Resource.Id)).Select(x => new CastingPayment(x.First().Holder, x.First().Resource, x.Sum(y => y.Amount))).ToList();
		foreach (var payment in payments)
			if (!double.IsFinite(payment.Amount) || !payment.Holder.CanUseResource(payment.Resource, payment.Amount)) throw new InvalidOperationException("Insufficient combined production reserve.");
		return new(capability, copy, trait, snapshot, payments, CaptureConfiguration(spell), raw);
	}
	private static XElement DeviceReceipt(ChargedMagicDeviceGameItemComponent device, string kind, IEnumerable<CastingPayment>? payments = null) =>
		new("Casting", new XAttribute("version", 1), new XAttribute("kind", kind), new XElement("Item", new XAttribute("id", device.Parent.Id)),
			(payments ?? []).Select(x => new XElement("Cost", new XAttribute("holder", x.Holder.Id), new XAttribute("resource", x.Resource.Id), new XAttribute("amount", x.Amount))));
	private MagicCastingResult DeviceUncertain(CastingOperation operation, Exception error)
	{
		var uncertain = operation with { Stage = "NeedsReview", Diagnostic = error.Message, UpdatedUtc = _clock() };
		_uncertain[operation.Id] = uncertain;
		try
		{
			// A failed insert may have an unknown result. Only amend a receipt that carries this exact claim.
			if (_store.Operation(operation.Id) is { } persisted && persisted.Definition == operation.Definition) _store.Write(uncertain);
		}
		catch { /* The earlier durable operation and item reservation prohibit retry. */ }
		return new(MagicCastingStatus.NeedsReview, $"Operation {operation.Id} needs staff review; no refund or automatic replay. {error.Message}", operation.Id);
	}
	private bool ClaimDeviceActivation(CastingOperation operation)
	{
		if (_store is MagicCastingStateStore) return MagicDeviceJournal.TryClaim(operation);
		// Injected stores are used by unit fixtures. Their owner/item guards provide the single-process boundary.
		if (_store.Operation(operation.Id) is not null) return false;
		_store.Write(operation); return true;
	}
	public MagicCastingResult BeginDeviceProduction(ICharacter actor, IGameItem item, long capabilityId, long spellId, int grade, int count)
	{
		lock (_deviceGuard)
		lock (Guard(actor))
		{
			if (!_mutating.TryAdd(Owner(actor).Id, 0)) return new(MagicCastingStatus.Refused, "A magical mutation is already active.");
			CastingOperation? operation = null;
			try
			{
				if (item.GetItemType<IChargedMagicDevice>() is not ChargedMagicDeviceGameItemComponent device) throw new InvalidOperationException("This item has no charged-device component.");
				if (actor.EffectsOfType<VancianTimedAction>().Any()) throw new InvalidOperationException("Finish current magical work first.");
				var quote = QuoteDeviceProduction(actor, device, capabilityId, spellId, grade, count);
				var proto = (ChargedMagicDeviceGameItemComponentProto)device.Prototype;
				using var plan = new VancianProductionPlan(actor, Enumerable.Range(0, count).SelectMany(_ => new[] { quote.Spell.InventoryPlanTemplate, proto.ProductionPlan }).ToArray());
				if (plan.Validate(item) is { } materials) throw new InvalidOperationException(materials);
				var liveQuote = QuoteDeviceProduction(actor, device, capabilityId, spellId, grade, count);
				if (liveQuote.Configuration != quote.Configuration || liveQuote.Snapshot.PotencyFingerprint != quote.Snapshot.PotencyFingerprint ||
					!liveQuote.Payments.Select(x => (x.Holder.Id, x.Resource.Id, x.Amount)).SequenceEqual(quote.Payments.Select(x => (x.Holder.Id, x.Resource.Id, x.Amount)))) throw new InvalidOperationException("Production inputs changed while scouting supplies; request a fresh operation.");
				var id = Guid.NewGuid();
				var payload = DeviceReceipt(device, "DeviceProduction", quote.Payments);
				var deadline = _clock() + TimeSpan.FromSeconds(checked(proto.SecondsPerCharge * count));
				payload.SetAttributeValue("deadline", deadline.ToString("O")); payload.SetAttributeValue("count", count);
				payload.Add(quote.Snapshot.Save());
				operation = new(id, Owner(actor).Id, actor.InstanceId, actor.Body.Id, capabilityId, spellId, quote.Trait.Id,
					quote.Capability.CastingPolicy!.ReserveResourceId, "DevicePaying", payload.ToString(), _clock(), _clock());
				_store.Write(operation); _checkpoint?.Invoke("DevicePaying");
				if (!device.Reserve(id)) throw new InvalidOperationException("The device was reserved during commitment.");
				PersistDevice(device);
				foreach (var payment in quote.Payments)
					if (!payment.Holder.UseResource(payment.Resource, payment.Amount)) throw new InvalidOperationException("Production payment was declined after commitment.");
				plan.Execute(); Flush(actor); _checkpoint?.Invoke("DevicePaid");
				operation = operation with { Stage = "DeviceProducing", UpdatedUtc = _clock() }; _store.Write(operation);
				var work = new DeviceWork(actor, device, quote, operation, grade, count, deadline, actor.Location, proto.Configuration);
				_deviceWork.Add(id, work);
				work.Action = new VancianTimedAction(actor, id, "charging a magical device", () => actor.Send(CompleteDeviceProduction(actor, id).Message),
					() => CancelDeviceProduction(actor, id), () => DeviceWorkError(work) is null, [item]);
				actor.AddEffect(work.Action, deadline - _clock());
				return new(MagicCastingStatus.Started, "Production costs are paid. Maintain the held device and casting inputs until completion; interruption gives no refund.", id);
			}
			catch (Exception ex) { return operation is null ? new(MagicCastingStatus.Refused, ex.Message) : DeviceUncertain(operation, ex); }
			finally { _mutating.TryRemove(Owner(actor).Id, out _); }
		}
	}
	private string? DeviceWorkError(DeviceWork work)
	{
		var actor = work.Actor;
		if (actor.InstanceId != work.Operation.ActorId || actor.Body?.Id != work.Operation.BodyId || Owner(actor).Id != work.Operation.CharacterId ||
			!ReferenceEquals(actor.Location, work.Location) || VancianMagicService.ActionError(actor) is not null) return "Production lost its acting body, location or physical inputs.";
		if (DeviceItemError(actor, work.Device, work.Operation.Id) is { } itemError) return itemError;
		if (work.Device.Reservation != work.Operation.Id || ((ChargedMagicDeviceGameItemComponentProto)work.Device.Prototype).Configuration != work.ItemConfiguration) return "The reserved device or configuration changed.";
		if (ResolveRoute(actor, work.Quote.Capability.Id, work.Operation.SpellId, work.Grade, false, out var capability, out _, out var spell, out var trait,
			out _, continuingOperation: work.Operation.Id) is { } route) return route;
		if (!ReferenceEquals(capability, work.Quote.Capability) || trait.Id != work.Quote.Trait.Id || CaptureConfiguration(spell) != work.Quote.Configuration) return "The producer route or source configuration changed.";
		var candidate = StoredSpellSnapshot.CaptureDevice(spell, actor, trait, work.Grade, Acquisition(actor, spell.Id)!.ControlledGrade, work.Quote.Snapshot.Power, _clock());
		return candidate.CanReproduceDevice(work.Quote.Snapshot, _world) && Owner(actor).TraitRawValue(trait) >= (work.Device.Charges > 0 ? work.Device.RequiredRaw : work.Quote.Raw)
			? null : "The producer can no longer reproduce the paid potency.";
	}
	public MagicCastingResult CompleteDeviceProduction(ICharacter actor, Guid token)
	{
		lock (_deviceGuard)
		lock (Guard(actor))
		{
			if (!_deviceWork.TryGetValue(token, out var work) || !ReferenceEquals(actor, work.Actor)) return new(MagicCastingStatus.Refused, "No live engine-issued work exists; persisted incomplete work needs staff review.");
			if (_clock() < work.Deadline) return new(MagicCastingStatus.Refused, "Production time has not elapsed.");
			if (!_mutating.TryAdd(Owner(actor).Id, 0)) return new(MagicCastingStatus.Refused, "A magical mutation is already active.");
			try
			{
				if (DeviceWorkError(work) is { } error) return CancelDeviceProduction(actor, token, error);
				var committing = work.Operation with { Stage = "DeviceFilling", UpdatedUtc = _clock() }; _store.Write(committing);
				_checkpoint?.Invoke("DeviceFilling");
				work.Device.Fill(token, work.Quote.Snapshot, work.Count, work.Quote.Capability.Id, work.Quote.Trait.Id, work.Quote.Raw);
				PersistDevice(work.Device); _checkpoint?.Invoke("DeviceFilled");
				work.Quote.Spell.ApplyProductionLockouts(actor); Flush(actor);
				_store.Write(committing with { Stage = "Completed", UpdatedUtc = _clock() });
				_deviceWork.Remove(token); work.Action?.FinishExternally();
				return new(MagicCastingStatus.Succeeded, $"Produced {work.Count} homogeneous charges without manifesting the spell.", token);
			}
			catch (Exception ex) { _deviceWork.Remove(token); work.Action?.FinishExternally(); return DeviceUncertain(work.Operation, ex); }
			finally { _mutating.TryRemove(Owner(actor).Id, out _); }
		}
	}
	public MagicCastingResult CancelDeviceProduction(ICharacter actor, Guid token, string reason = "Production interrupted; no charges or refund.")
	{
		lock (_deviceGuard)
		{
			if (!_deviceWork.TryGetValue(token, out var work) || !ReferenceEquals(actor, work.Actor)) return new(MagicCastingStatus.Refused, "No cancellable live device work.");
			if (_store.Operation(token)?.Stage != "DeviceProducing") return new(MagicCastingStatus.NeedsReview, "Commitment changed; staff review is required.", token);
			try
			{
				work.Device.Release(token); PersistDevice(work.Device);
				work.Quote.Spell.ApplyProductionLockouts(actor); Flush(actor);
				_store.Write(work.Operation with { Stage = "Completed", Diagnostic = reason, UpdatedUtc = _clock() });
				_deviceWork.Remove(token); work.Action?.FinishExternally();
				return new(MagicCastingStatus.Failed, reason, token);
			}
			catch (Exception ex) { _deviceWork.Remove(token); work.Action?.FinishExternally(); return DeviceUncertain(work.Operation, ex); }
		}
	}
	public MagicCastingResult ActivateDevice(ICharacter actor, IGameItem item, string targets)
	{
		lock (_deviceGuard)
		lock (Guard(actor))
		{
			if (!_mutating.TryAdd(Owner(actor).Id, 0)) return new(MagicCastingStatus.Refused, "A magical mutation is already active.");
			CastingOperation? operation = null;
			ChargedMagicDeviceGameItemComponent? device = null;
			Guid? charge = null;
			try
			{
				device = item.GetItemType<IChargedMagicDevice>() as ChargedMagicDeviceGameItemComponent ?? throw new InvalidOperationException("No charged device component.");
				var proto = (ChargedMagicDeviceGameItemComponentProto)device.Prototype;
				if (proto.Role == MagicDeviceRole.Focus || device.NextCharge is not { } next) throw new InvalidOperationException("No charged payload is available. Choose focus mode explicitly to cast personally.");
				charge = next;
				if (DeviceItemError(actor, device) is { } itemError) throw new InvalidOperationException(itemError);
				if (VancianMagicService.CastingError(actor) is { } physical) throw new InvalidOperationException(physical);
				if (SpeechEligibility(actor, MudSharp.Form.Audio.AudioVolume.Decent) is { } speech) throw new InvalidOperationException(speech);
				if (DeviceEligibilityError(actor, device) is { } eligibility) throw new InvalidOperationException(eligibility);
				if (!proto.Spells.Contains(device.SpellId!.Value) || _store.Operation(next) is not null || _uncertain.ContainsKey(next)) throw new InvalidOperationException("This payload was revoked or the exact charge was already committed.");
				var spell = device.Snapshot!.CreateSpell(_world, false); ValidateDeviceSpell(spell);
				var actingInstance = actor.InstanceId; var actingBody = actor.Body.Id; var actingOwner = Owner(actor).Id;
				var target = SpellTargetCapture.Resolve(actor, spell, device.Snapshot.Power, new StringStack(targets), true) ?? throw new InvalidOperationException("No valid complete target specification.");
				if (target.Target is PerceivableGroup || target.Target is { } recipient && !actor.CanInteractPlanar(recipient, PlanarInteractionKind.Magic)) throw new InvalidOperationException("This initial device carrier requires one reachable target.");
				if (!device.Reserve(next)) throw new InvalidOperationException("The charge was reserved by another action.");
				var itemConfiguration = proto.Configuration;
				var invocation = new SpellInvocationContext(SpellInvocationSource.ScrollActivation, device.Snapshot.Numbers.Outcome, _ =>
				{
					if (actor.InstanceId != actingInstance || actor.Body?.Id != actingBody || Owner(actor).Id != actingOwner ||
						DeviceItemError(actor, device, next) is not null || DeviceEligibilityError(actor, device) is not null ||
					proto.Configuration != itemConfiguration || !ReferenceEquals(device.Prototype, proto) || device.NextCharge != next ||
					_store.Operation(next) is not null || VancianMagicService.CastingError(actor) is not null ||
					SpeechEligibility(actor, MudSharp.Form.Audio.AudioVolume.Decent) is not null) return false;
					var live = SpellTargetCapture.Resolve(actor, spell, device.Snapshot.Power, new StringStack(targets), true);
					if (live is null || !ReferenceEquals(live.Target, target.Target)) return false;
					var payload = DeviceReceipt(device, "DeviceActivation"); payload.Add(device.Snapshot.Save()); payload.SetAttributeValue("charge", next);
					payload.SetAttributeValue("claim", Guid.NewGuid());
					operation = new(next, Owner(actor).Id, actor.InstanceId, actor.Body.Id, device.ProducerCapability, spell.Id, device.ProducerTrait,
						0, "DeviceConsuming", payload.ToString(), _clock(), _clock());
					if (!ClaimDeviceActivation(operation)) { operation = null; return false; }
					_checkpoint?.Invoke("DeviceConsuming");
					device.Consume(next); PersistDevice(device); _checkpoint?.Invoke("DeviceConsumed");
					if (proto.CheckTraitId == 0) return true;
					using var noProgress = new CheckImprovementScope(actor);
					return _world.GetCheck(CheckType.CastSpellCheck).Check(actor, proto.CheckDifficulty, _world.Traits.Get(proto.CheckTraitId), target.Target).Outcome >= proto.MinimumOutcome;
				});
				spell.CastVancian(actor, target.Target, device.Snapshot.Power, invocation, target.Parameters);
				if (operation is null) { device.Release(next); return new(MagicCastingStatus.Refused, "Activation preflight refused; the charge remains intact."); }
				Flush(actor);
				_store.Write(operation with { Stage = "Completed", Diagnostic = invocation.Status.ToString(), UpdatedUtc = _clock() });
				return new(invocation.Status == MagicInvocationStatus.Succeeded ? MagicCastingStatus.Succeeded : MagicCastingStatus.Failed,
					"Exactly one device charge was spent; target resistance and wards remain live.", next);
			}
			catch (Exception ex) { if (operation is null && charge is { } token) device?.Release(token); return operation is null ? new(MagicCastingStatus.Refused, ex.Message) : DeviceUncertain(operation, ex); }
			finally { _mutating.TryRemove(Owner(actor).Id, out _); }
		}
	}
	public MagicCastingResult CastDeviceFocus(MagicCastingIntent intent, IGameItem item)
	{
		lock (_deviceGuard)
		lock (Guard(intent.Actor))
		{
			if (item.GetItemType<IChargedMagicDevice>() is not ChargedMagicDeviceGameItemComponent device || device.Role == MagicDeviceRole.Charged)
				return new(MagicCastingStatus.Refused, "That item has no explicit focus mode.");
			if (intent.Actor.Body is null || intent.Mode == MagicCastingMode.Practice) return new(MagicCastingStatus.Refused, "Focus mode needs a physical body and a manifestation cast.");
			var proto = (ChargedMagicDeviceGameItemComponentProto)device.Prototype;
			if (!_deviceFocus.TryAdd(intent, new(device, intent.Actor.InstanceId, intent.Actor.Body.Id, Owner(intent.Actor).Id, proto, proto.Configuration)))
				return new(MagicCastingStatus.Refused, "This focus invocation is already active.");
			try { return Cast(intent); }
			finally { _deviceFocus.TryRemove(intent, out _); }
		}
	}
	private long[] ValidateDeviceFocus(MagicCastingIntent intent, IInventoryPlan plan, long[] items)
	{
		if (!_deviceFocus.TryGetValue(intent, out var use)) return items;
		var actor = intent.Actor; var device = use.Device;
		if (actor.InstanceId != use.ActorId || actor.Body?.Id != use.BodyId || Owner(actor).Id != use.OwnerId ||
			!ReferenceEquals(device.Prototype, use.Prototype) || ((ChargedMagicDeviceGameItemComponentProto)device.Prototype).Configuration != use.Configuration ||
			device.Role == MagicDeviceRole.Charged || !((ChargedMagicDeviceGameItemComponentProto)device.Prototype).Spells.Contains(intent.SpellId))
			throw new InvalidOperationException("The explicit focus identity, mode or configuration changed.");
		if ((DeviceItemError(actor, device) ?? DeviceEligibilityError(actor, device, intent.SpellId)) is { } error) throw new InvalidOperationException(error);
		if (plan is InventoryPlan native && native.Phases.Values.SelectMany(x => x.ScoutedItems).Any(x =>
			ReferenceEquals(x.Primary, device.Parent) && x.Action.DesiredState is DesiredItemState.Consumed or DesiredItemState.ConsumeCommodity or DesiredItemState.ConsumeLiquid or DesiredItemState.Apply))
			throw new InvalidOperationException("The spell plan cannot consume its held focus.");
		return items.Append(device.Parent.Id).Distinct().ToArray();
	}
}
