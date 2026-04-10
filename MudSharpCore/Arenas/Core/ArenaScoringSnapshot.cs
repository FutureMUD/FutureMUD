#nullable enable

using MudSharp.Character;

namespace MudSharp.Arenas;

internal sealed record ArenaScoringSnapshot(
	ICharacter Attacker,
	ICharacter Defender,
	int AttackerSideIndex,
	int DefenderSideIndex,
	int LandedHit,
	int UndefendedHit,
	string ImpactLocationKey,
	string ImpactBodypartIdentity
);
