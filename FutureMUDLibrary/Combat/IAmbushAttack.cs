#nullable enable

using System.Collections.Generic;
using MudSharp.Construction;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

/// <summary>An authored natural strike that can cross a legal local layer boundary and attempt an initial seize.</summary>
public interface IAmbushAttack : IWeaponAttack
{
	IReadOnlyCollection<RoomLayer> SourceLayers { get; }
	IReadOnlyCollection<RoomLayer> DestinationLayers { get; }
	bool AttemptSeize { get; }
	Difficulty SeizeDifficulty { get; }
}
