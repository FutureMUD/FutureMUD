#nullable enable

using System;

namespace MudSharp.Arenas;

public sealed record ArenaRatingSummary(
	long ArenaId,
	long CharacterId,
	string CharacterName,
	long CombatantClassId,
	string CombatantClassName,
	decimal Rating,
	DateTime? LastUpdatedAt);
