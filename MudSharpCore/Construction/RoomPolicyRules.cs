#nullable enable

using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Construction;

public static class RoomPolicyRules
{
	public static bool Restricts(IRoom? room, RoomRestrictions restriction) =>
		room?.EffectsOfType<IRoomPolicyEffect>().Any(x => x.Applies() && (x.Restrictions & restriction) != 0) == true;

	public static bool AllowsWandering(IRoom? destination) => !Restricts(destination, RoomRestrictions.NpcWandering);

	public static string? MagicDenial(ICharacter actor, IEnumerable<IPerceivable> targets)
	{
		if (Restricts(actor.Location, RoomRestrictions.Magic)) return "Magic cannot be used in this location.";
		foreach (var target in targets)
		{
			if (target is PerceivableGroup group)
			{
				if (MagicDenial(actor, group.Members) is { } groupDenial) return groupDenial;
				continue;
			}
			if (Restricts(target as IRoom ?? target.Location, RoomRestrictions.Magic))
				return "Magic cannot affect a target in that location.";
		}
		return null;
	}

	public static bool ApplyEntryHazard(IRoom room, ICharacter actor)
	{
		if (!Restricts(room, RoomRestrictions.Lethal) || actor.IsAdministrator()) return false;
		actor.OutputHandler.Send("You cannot survive here.");
		actor.Die();
		return true;
	}

	public static string? EntryDenial(ICharacter actor, IRoomExit exit, IPositionState requiredPosition)
		=> EntryDenial(actor, exit, requiredPosition, out _);

	public static string? EntryDenial(ICharacter actor, IRoomExit exit, IPositionState requiredPosition,
		out IRoomExitPolicyEffect? denyingPolicy)
	{
		denyingPolicy = null;
		var destination = exit.Destination;
		if (actor.EffectsOfType<IImmwalkEffect>().Any()) return null;
		foreach (var policy in exit.Origin.EffectsOfType<IRoomExitPolicyEffect>())
			if (policy.DenialReason(actor, exit) is { } denial)
			{
				denyingPolicy = policy;
				return denial;
			}
		if (Restricts(destination, RoomRestrictions.Mounts) &&
		    (actor.RidingMount is not null || actor.Riders.Any() || (actor is INPC npc && npc.AIs.OfType<IMountableAI>().Any())))
			return "You cannot take a mount there.";
		if (Restricts(destination, RoomRestrictions.Flight) &&
		    (actor.PositionState == PositionFlying.Instance || requiredPosition == PositionFlying.Instance))
			return "You cannot fly into that location.";
		if (Restricts(destination, RoomRestrictions.Climbing) &&
		    (exit.IsClimbExit || requiredPosition == PositionClimbing.Instance))
			return "There is no way to climb there.";
		return CapacityDenial(destination, actor);
	}

	public static string? ExitDenial(ICharacter? actor, IRoomExit exit) => exit.Origin
		.EffectsOfType<IRoomExitPolicyEffect>().Select(x => x.DenialReason(actor, exit)).FirstOrDefault(x => x is not null);

	public static string? OpeningDenial(ICharacter? actor, IExit? exit)
	{
		if (actor?.Location is not { } location || exit?.RoomExitFor(location) is not { } side) return null;
		if (actor.EffectsOfType<IImmwalkEffect>().Any()) return null;
		return location.EffectsOfType<IRoomExitPolicyEffect>().Where(x => x.ControlsOpening)
			.Select(x => x.DenialReason(actor, side)).FirstOrDefault(x => x is not null);
	}

	public static string? CapacityDenial(IRoom room, ICharacter actor)
	{
		var policies = room.EffectsOfType<IRoomPolicyEffect>().Where(x => x.Applies() && x.MaximumOccupants > 0).ToArray();
		if (policies.Length == 0 || room.Characters.Any(x => ReferenceEquals(x, actor))) return null;
		var arriving = new[] { actor }.Concat(actor.Riders ?? []).Concat(actor.RidingMount is { } mount ? [mount] : [])
			.Distinct<ICharacter>(ReferenceEqualityComparer.Instance).ToArray();
		foreach (var policy in policies)
		{
			double Cost(ICharacter character)
			{
				if (character.IsAdministrator()) return 0;
				if (policy.OccupancyCostProgId == 0) return 1;
				var prog = room.Gameworld.FutureProgs.Get(policy.OccupancyCostProgId);
				var cost = prog?.ExecuteDouble(character) ?? double.PositiveInfinity;
				return double.IsFinite(cost) && cost >= 0 ? cost : double.PositiveInfinity;
			}
			if (room.Characters.Concat(arriving).Distinct<ICharacter>(ReferenceEqualityComparer.Instance).Sum(Cost) > policy.MaximumOccupants)
				return "There is not enough room for you there.";
		}
		return null;
	}
}
