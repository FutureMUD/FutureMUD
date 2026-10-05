using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Resources;
using MudSharp.Magic.Vancian;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.Planes;
using MudSharp.RPG.Merits;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private sealed record DeviceFocusUse(ChargedMagicDeviceGameItemComponent Device, ICharacter Actor, IBody? Body,
		ICharacter Owner, long ActorId, long BodyId, long OwnerId, IGameItemComponentProto Prototype,
		string Configuration, ICell? Location, RoomLayer Layer, IMerit[] Merits, IEffect[] CapabilityEffects)
	{
		public DeviceTargetState[]? Targets { get; set; }
	}
	private sealed record DeviceTargetState(IPerceivable Target, ICell? Location, RoomLayer Layer, IBody? Body);
	private static DeviceFocusUse CaptureDeviceUse(ICharacter actor, ChargedMagicDeviceGameItemComponent device) =>
		new(device, actor, actor.Body, Owner(actor), actor.InstanceId, actor.Body?.Id ?? 0, Owner(actor).Id,
			device.Prototype, ((ChargedMagicDeviceGameItemComponentProto)device.Prototype).Configuration, actor.Location,
			actor.RoomLayer, actor.Merits.ToArray(), DeviceCapabilityEffects(actor));
	private static IEffect[] DeviceCapabilityEffects(ICharacter actor) => actor.Effects.Concat(actor.Body?.Effects ?? [])
		.Where(x => x is IGiveMagicCapabilityEffect).ToArray();
	private static DeviceTargetState[] CaptureDeviceTargets(IPerceivable? target) =>
		(target is PerceivableGroup group ? group.Members : target is { } single ? [single] : [])
		.Select(x => new DeviceTargetState(x, x.Location, x.RoomLayer, (x as ICharacter)?.Body)).ToArray();
	private static string? DeviceIdentityError(ICharacter actor, DeviceFocusUse use) =>
		!ReferenceEquals(actor, use.Actor) || !ReferenceEquals(actor.Body, use.Body) || !ReferenceEquals(Owner(actor), use.Owner) ||
		actor.InstanceId != use.ActorId || actor.Body?.Id != use.BodyId || Owner(actor).Id != use.OwnerId ||
		!ReferenceEquals(use.Device.Prototype, use.Prototype) || ((ChargedMagicDeviceGameItemComponentProto)use.Device.Prototype).Configuration != use.Configuration
			? "The explicit device identity or configuration changed." : null;
	/// <summary>Last admission check. No capabilities, Applies, policy, reach, physical or speech evaluation.
	/// Native body inventory, identity, merit/effect lists and component fields are structural getters.
	/// Target Location/RoomLayer follow the native raw spatial-host/custody graph, never visibility policies.</summary>
	private string? DeviceStructuralError(ICharacter actor, DeviceFocusUse use, Guid? continuing = null,
		DeviceTargetState[]? targets = null)
	{
		if (DeviceIdentityError(actor, use) is { } identity) return identity;
		var device = use.Device;
		if (!ReferenceEquals(actor.Location, use.Location) || actor.RoomLayer != use.Layer || actor.Location is null ||
			!actor.Merits.SequenceEqual(use.Merits, ReferenceEqualityComparer.Instance) ||
			!DeviceCapabilityEffects(actor).SequenceEqual(use.CapabilityEffects, ReferenceEqualityComparer.Instance))
			return "The acting body's location or entitlement inputs changed.";
		if (device.DataError is not null || device.Charges > device.Capacity || device.Parent.Deleted ||
			!ReferenceEquals(device.Parent.GetItemType<IChargedMagicDevice>(), device) ||
			actor.Body is null || !actor.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, device.Parent)) ||
			device.Reservation is { } reservation && reservation != continuing)
			return "The captured device custody, bank or reservation changed.";
		if (targets is not null && targets.Any(x => !ReferenceEquals(x.Target.Location, x.Location) || x.Target.RoomLayer != x.Layer ||
			!ReferenceEquals((x.Target as ICharacter)?.Body, x.Body))) return "A captured target changed location, layer or body.";
		// Receipt inspection is callback-free and global to the physical item, irrespective of its current owner.
		return CastingQuarantineReason(actor, itemIds: [device.Parent.Id], continuingOperation: continuing);
	}
	private string? DeviceCallbackError(ICharacter actor, DeviceFocusUse use, Guid? continuing = null, long? focusSpell = null) =>
		DeviceItemError(actor, use.Device, continuing) ?? VancianMagicService.CastingError(actor) ??
		SpeechEligibility(actor, MudSharp.Form.Audio.AudioVolume.Decent) ?? DeviceEligibilityError(actor, use.Device, focusSpell);

	private sealed record DevicePaymentState(CastingPayment Payment, double Balance, bool Reserve, ICharacter Owner,
		SimpleMagicResource? Simple, IFutureProg? CapProg, string? CapText, MagicResourceAttributeCapacity? AttributeCapacity);
	private MagicResourceCapacityAdmission AdmitDevicePayments(ICharacter actor, IReadOnlyList<CastingPayment> payments,
		DeviceFocusUse use, Guid? continuing = null, DeviceTargetState[]? targets = null, long? focusSpell = null)
	{
		// Capture BEFORE callbacks. A valid returned cap must not conceal a balance, holder or definition mutation.
		var states = payments.Select(x => new DevicePaymentState(x, x.Holder.MagicResourceAmounts.GetValueOrDefault(x.Resource),
			IsConfiguredReserve(_world, x.Resource.Id), Owner(x.Holder), x.Resource as SimpleMagicResource,
			(x.Resource as SimpleMagicResource)?.ResourceCapProg, (x.Resource as SimpleMagicResource)?.ResourceCapProg?.FunctionText,
			(x.Resource as SimpleMagicResource)?.AttributeCapacity)).ToArray();
		var definitions = _world.MagicCapabilities.OfType<IMagicCastingCapability>().Select(x => (Capability: x, Configuration: System.Text.Json.JsonSerializer.Serialize(x.CastingPolicy))).ToArray();
		var traits = actor.Traits.Concat(Owner(actor).Traits).Distinct<ITrait>(ReferenceEqualityComparer.Instance).Select(x => (Trait: x, Raw: x.RawValue)).ToArray();
		_checkpoint?.Invoke("DeviceAdmissionPolicies");
		var admissions = states.Select(x =>
		{
			if (!MagicResourceCapacity.TryGetCap(x.Payment.Resource, x.Payment.Holder, out var cap, out var error)) throw new InvalidOperationException(error);
			if (!double.IsFinite(x.Balance) || !double.IsFinite(x.Payment.Amount) || x.Payment.Amount < 0 || Math.Min(cap, x.Balance) < x.Payment.Amount)
				throw new InvalidOperationException("Insufficient or invalid admitted device payment.");
			return new MagicResourceCapacityAdmission.Payment(x.Payment.Holder, x.Payment.Resource, x.Payment.Amount, cap);
		}).ToArray();
		if (targets is not null && targets.Any(x => !actor.CanSee(x.Target) || !actor.CanInteractPlanar(x.Target, PlanarInteractionKind.Magic)))
			throw new InvalidOperationException("The admitted device target is no longer reachable.");
		if (DeviceCallbackError(actor, use, continuing, focusSpell) is { } callback) throw new InvalidOperationException(callback);
		// Everything below is structural. The scope is opened by the caller only around native payment.
		if (states.Any(x => x.Payment.Holder.MagicResourceAmounts.GetValueOrDefault(x.Payment.Resource) != x.Balance ||
			!ReferenceEquals(ReserveHolder(actor, x.Payment.Resource.Id), x.Payment.Holder) || !ReferenceEquals(Owner(x.Payment.Holder), x.Owner) ||
			IsConfiguredReserve(_world, x.Payment.Resource.Id) != x.Reserve ||
			!ReferenceEquals(_world.MagicResources.Get(x.Payment.Resource.Id), x.Payment.Resource) ||
			x.Simple is not null && (!ReferenceEquals(x.Simple.ResourceCapProg, x.CapProg) || x.Simple.ResourceCapProg?.FunctionText != x.CapText || x.Simple.AttributeCapacity != x.AttributeCapacity)) ||
			definitions.Any(x => !ReferenceEquals(_world.MagicCapabilities.Get(x.Capability.Id), x.Capability) || System.Text.Json.JsonSerializer.Serialize(x.Capability.CastingPolicy) != x.Configuration) ||
			traits.Any(x => x.Trait.RawValue != x.Raw) || !actor.Traits.Concat(Owner(actor).Traits).Distinct<ITrait>(ReferenceEqualityComparer.Instance)
				.SequenceEqual(traits.Select(x => x.Trait), ReferenceEqualityComparer.Instance))
			throw new InvalidOperationException("Captured payment balance, holder or resource configuration changed.");
		if (DeviceStructuralError(actor, use, continuing, targets) is { } structural) throw new InvalidOperationException(structural);
		return new(admissions);
	}
	private MagicResourceCapacityAdmission? AdmitDeviceFocusPayment(MagicCastingIntent intent, Prepared prepared)
	{
		if (!_deviceFocus.TryGetValue(intent, out var use)) return null;
		return AdmitDevicePayments(intent.Actor, prepared.Payments, use, targets: use.Targets, focusSpell: intent.SpellId);
	}
}
