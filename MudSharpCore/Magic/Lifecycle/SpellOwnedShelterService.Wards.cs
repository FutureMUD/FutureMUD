#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedShelterService
{
	internal static void DetachCommittedWard(IRoom room, MudSharp.Effects.EffectHandler handler,
		SpellLifecycleOrigin origin, SpellShelterWardConfiguration? configuration)
	{
		if (!ReferenceEquals(handler.Parent, room)) throw new InvalidOperationException("The committed room effect handler changed owner.");
		RequireRetirementEffects(room, origin, configuration);
		handler.ForgetCommittedRetirementEffects();
		EnvironmentalExposureService.ForgetCommittedShelterRoom(room.Gameworld, room);
	}

	internal static void RequireWardAuthority(IRoom room, SpellLifecycleOrigin origin,
		SpellShelterWardConfiguration? configuration, bool requirePresent)
	{
		var wards = room.Effects.OfType<SpellShelterWard>().ToArray();
		if (configuration is null ? wards.Length != 0 : wards.Length > 1 ||
			wards.Any(x => !ReferenceEquals(x.Owner, room) || !x.Matches(origin, configuration)) || requirePresent && wards.Length != 1)
			throw new InvalidOperationException("The shelter's exact owned ward is missing or changed; retain closed topology for recovery.");
	}

	internal static void RequireRetirementEffects(IRoom room, SpellLifecycleOrigin origin,
		SpellShelterWardConfiguration? configuration)
	{
		RequireWardAuthority(room, origin, configuration, false);
		if (room.Hooks.Any() || room.Effects.Any(x => x is not SpellShelterWard))
			throw new InvalidOperationException("Foreign room hooks or effects require recovery before topology removal.");
	}

	internal static void RequirePersistedWardAuthority(string data, SpellLifecycleOrigin origin,
		SpellShelterWardConfiguration? configuration)
	{
		var root = XElement.Parse(data);
		if (root.Name != "Effects" || root.Elements().Any(x => x.Name != "Effect"))
			throw new FormatException("Unsupported shelter effect envelope.");
		var envelopes = root.Elements("Effect").ToArray();
		// Only the exact native loader paths are compared. Any other room effect is borrowed.
		if (envelopes.Length > (configuration is null ? 0 : 1) || envelopes.Any(x =>
			!XNode.DeepEquals(x, SpellShelterWard.Envelope(origin.Id, origin.SpellId, configuration!))))
			throw new InvalidOperationException("Persisted foreign or changed room effects require recovery.");
	}
}
