#nullable enable
using MudSharp.Character;
using MudSharp.NPC.AI;

namespace MudSharp.NPC;

/// <summary>Uses the existing NPC bodyguard association; does not establish ownership or combat duties.</summary>
internal static class NpcCompanionLifecycle
{
	internal static bool FollowsHandler(ICharacter npc) => npc is INPC creature &&
		creature.AIs.OfType<INpcCompanionLifecyclePolicy>().Any(x => x.CompanionFollowsHandler);

	internal static bool SetFollowing(ICharacter follower, ICharacter? leader)
	{
		if (leader is not null)
		{
			if (!follower.ColocatedWith(leader) || follower.State.HasFlag(CharacterState.Dead) || leader.State.HasFlag(CharacterState.Dead)) return false;
			var visited = new HashSet<ICharacter>(ReferenceEqualityComparer.Instance);
			for (var current = leader; current is not null; current = current.Following as ICharacter)
			{
				if (ReferenceEquals(current, follower) || !visited.Add(current)) return false;
			}
		}
		if (ReferenceEquals(follower.Following, leader)) return true;
		follower.Follow(leader);
		return ReferenceEquals(follower.Following, leader);
	}

	internal static bool Assign(INPC npc, ICharacter? handler)
	{
		if (handler is not null && (!handler.IsPlayerCharacter || npc.State.HasFlag(CharacterState.Dead) ||
		    handler.State.HasFlag(CharacterState.Dead) || !npc.ColocatedWith(handler))) return false;
		// Offline companions are restored by their existing cache owner, never orphaned by a prog.
		if (npc.Gameworld.CachedBodyguards.Values.Any(x => x.Any(y => ReferenceEquals(y, npc)))) return false;
		CommandExecutionScope.MarkCommitted(npc);
		npc.BodyguardingCharacterID = handler is null ? null : CharacterInstanceIdentityComparer.IdentityId(handler);
		return true;
	}

	internal static void Mounted(ICharacter mount, ICharacter rider)
	{
		if (!FollowsHandler(mount) || mount is not INPC npc || !rider.IsPlayerCharacter) return;
		if (Assign(npc, rider)) mount.CeaseFollowing();
	}

	internal static void PrepareForLogout(ICharacter mount)
	{
		if (!FollowsHandler(mount)) return;
		foreach (var rider in mount.Riders.ToArray()) mount.RemoveRider(rider);
	}

	internal static void Reunited(ICharacter npc, ICharacter handler)
	{
		if (FollowsHandler(npc) && npc is INPC creature && handler.IsPlayerCharacter &&
		    creature.BodyguardingCharacterID == CharacterInstanceIdentityComparer.IdentityId(handler) &&
		    !handler.State.HasFlag(CharacterState.Dead) && npc.ColocatedWith(handler) &&
		    !npc.Riders.Any() && npc.Following is null) SetFollowing(npc, handler);
	}

	internal static bool RequiresColocation(ICharacter npc) => npc is INPC creature &&
		creature.AIs.OfType<INpcCompanionLifecyclePolicy>().Any(x => x.CompanionRequiresColocation);

	internal static bool CanReunite(ICharacter npc, long? roomId, int roomLayer) =>
		!RequiresColocation(npc) || roomId is > 0 && npc.Location?.Id == roomId && (int)npc.RoomLayer == roomLayer;

	internal static bool CanReuniteWith(ICharacter npc, ICharacter handler) =>
		!RequiresColocation(npc) || npc is INPC creature &&
		creature.BodyguardingCharacterID == CharacterInstanceIdentityComparer.IdentityId(handler) &&
		!handler.State.HasFlag(CharacterState.Dead) && CanReunite(npc, handler.Location?.Id, (int)handler.RoomLayer);
}
