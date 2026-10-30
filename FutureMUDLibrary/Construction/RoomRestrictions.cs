#nullable enable

using System;
using MudSharp.Effects;
using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.Construction;

[Flags]
public enum RoomRestrictions
{
	None = 0,
	Vehicles = 1 << 0,
	Mounts = 1 << 1,
	Flight = 1 << 2,
	Climbing = 1 << 3,
	NpcWandering = 1 << 4,
	Magic = 1 << 5,
	Lethal = 1 << 6
}

/// <summary>Persistent physical room policies, independent of its overlay's terrain.</summary>
public interface IRoomPolicyEffect : IEffectSubtype
{
	RoomRestrictions Restrictions { get; }
	int MaximumOccupants { get; }
	long OccupancyCostProgId => 0;
}

/// <summary>A pure access check used by both route planning and movement execution.</summary>
public interface IRoomExitPolicyEffect : IEffectSubtype
{
	bool ControlsOpening => false;
	string? DenialReason(ICharacter? actor, IRoomExit exit);
	void OnDeniedAttempt(ICharacter actor, IRoomExit exit) { }
}
