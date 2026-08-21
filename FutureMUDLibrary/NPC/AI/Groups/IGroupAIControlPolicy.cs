using System;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.NPC.AI.Groups;

/// <summary>
/// Describes the parts of an NPC's ordinary AI that a live group AI currently coordinates.
/// Individual AIs retain any behaviour that the group does not claim.
/// </summary>
[Flags]
public enum GroupAIControlScope
{
	None = 0,
	Movement = 1 << 0,
	Feeding = 1 << 1,
	Threats = 1 << 2,
	Activity = 1 << 3,
	Shelter = 1 << 4,
	Senses = 1 << 5
}

/// <summary>
/// Implemented by group AI types that coordinate selected autonomous NPC behaviours.
/// </summary>
public interface IGroupAIControlPolicy
{
	GroupAIControlScope ControlScope { get; }
}

/// <summary>
/// Optional builder surface for configurable group AI types.
/// </summary>
public interface IEditableGroupAIType
{
	bool BuildingCommand(ICharacter actor, StringStack command);
	string Show(ICharacter actor);
}
