#nullable enable

using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

/// <summary>
/// Represents a configured participant slot in an arena event.
/// </summary>
public interface IArenaParticipant : IEditableItem
{
	/// <summary>Character identifier for this slot occupant.</summary>
	long CharacterId { get; }
	/// <summary>Character occupying the slot, null if pending NPC fill.</summary>
	ICharacter? Character { get; }
	/// <summary>Combatant class chosen for the participant.</summary>
	ICombatantClass CombatantClass { get; }
	/// <summary>Side index this participant belongs to (zero-based).</summary>
	int SideIndex { get; }
	/// <summary>True when the participant is an NPC supplied by the arena.</summary>
	bool IsNpc { get; }
	/// <summary>Optional show name for intros and echoes.</summary>
	string? StageName { get; }
	/// <summary>Optional ANSI colour applied to the participant name.</summary>
	string? SignatureColour { get; }
	/// <summary>Rating snapshot when registration closed.</summary>
	decimal? StartingRating { get; }
}

/// <summary>
/// Defines per-side capacity and policy for an event type.
/// </summary>
public interface IArenaEventTypeSide : IEditableItem
{
	int Index { get; }
	int Capacity { get; }
	ArenaSidePolicy Policy { get; }
	IEnumerable<ICombatantClass> EligibleClasses { get; }
	IFutureProg? OutfitProg { get; }
	bool AllowNpcSignup { get; }
	bool AutoFillNpc { get; }
	IFutureProg? NpcLoaderProg { get; }
}
