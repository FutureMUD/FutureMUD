#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Magic;
using MudSharp.Planes;

namespace MudSharp.Effects.Concrete;

/// <summary>Nonpersistent runtime boundary reconstructed only for an already committed creation.</summary>
public sealed class SpellProjectionBoundary(ICharacter owner, SpellProjectionAnchor anchor) : Effect(owner), ISpellProjectionBoundary, IPlanarOverlayEffect
{
	public SpellProjectionKind Kind => anchor.Configuration.Kind;
	protected override string SpecificEffectType => "SpellProjectionBoundary";
	public override string Describe(IPerceiver voyeur) => $"Bounded {Kind.DescribeEnum()} presence; physical manipulation is unavailable.";
	public int PlanarPriority => int.MaxValue;
	public bool OverridesBasePlanarPresence => true;
	public PlanarPresenceDefinition PlanarPresenceDefinition => new([anchor.Configuration.PlaneId],
		Kind == SpellProjectionKind.SandEffigy ? [anchor.Configuration.PlaneId, Gameworld.DefaultPlane.Id] : [anchor.Configuration.PlaneId],
		[anchor.Configuration.PlaneId, Gameworld.DefaultPlane.Id],
		Enum.GetValues<PlanarInteractionKind>().ToDictionary(x => x, x =>
			(IEnumerable<long>)(x is PlanarInteractionKind.Observe or PlanarInteractionKind.Hear ? [anchor.Configuration.PlaneId, Gameworld.DefaultPlane.Id] : [])),
		true, false, anchor.Configuration.CrossClosedDoors, false, true, false, false, "ownedprojection");

	public string? TravelError(SpatialLocation destination, bool throughExit)
	{
		if (!owner.IsEmbodied || !owner.IsControllable) return "That projected presence is collapsing.";
		if (Kind == SpellProjectionKind.SandEffigy)
			return destination.Room?.Id == anchor.RoomId && (int)destination.Layer == anchor.Layer && destination.RoutePositionMetres == anchor.RoutePosition
				? null : "The sand effigy cannot move from its anchor.";
		if (destination.Room?.Id == owner.Location?.Id && destination.Layer == owner.RoomLayer && destination.RoutePositionMetres == owner.RoutePositionMetres)
			return null;
		if (!throughExit || destination.Layer != (RoomLayer)anchor.Layer || destination.RoutePositionMetres is not null)
			return "The walking shadow may travel only through native exits on its anchored layer, outside route rooms.";
		if (!WithinRange(Gameworld.Rooms.Get(anchor.RoomId), destination.Room, owner, anchor.Configuration.MaximumRoomDistance))
			return "That destination is beyond the walking shadow's anchor range.";
		return null;
	}

	internal static bool WithinRange(IRoom? anchor, IRoom? destination, ICharacter actor, int distance)
	{
		if (anchor is null || destination is null || distance is < 0 or > 32) return false;
		var seen = new HashSet<long> { anchor.Id }; var pending = new Queue<(IRoom Room, int Distance)>(); pending.Enqueue((anchor, 0));
		while (pending.TryDequeue(out var step))
		{
			if (step.Room.Id == destination.Id) return true;
			if (step.Distance >= distance) continue;
			foreach (var exit in step.Room.ExitsFor(actor, true))
			{
				if (exit.Destination is not { RouteDefinition: null } room || !seen.Add(room.Id)) continue;
				if (seen.Count > 4096) return false;
				pending.Enqueue((room, step.Distance + 1));
			}
		}
		return false;
	}
}

public sealed class SpellProjectionTrance(ICharacter owner, long instanceId) : Effect(owner), IHelplessEffect
{
	public long ProjectionInstanceId => instanceId;
	protected override string SpecificEffectType => "SpellProjectionTrance";
	public override bool Applies() => owner.Identity.FocusedInstance?.InstanceId == instanceId;
	public override bool Applies(object? target) => Applies();
	public override bool Applies(object? target, object? thirdparty) => Applies();
	public override bool Applies(object? target, PerceiveIgnoreFlags flags) => Applies();
	public override string Describe(IPerceiver voyeur) => "Helpless in a trance while focused through a walking shadow.";
}
