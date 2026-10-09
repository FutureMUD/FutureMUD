#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public enum MagicAttackRange { Melee, Ranged }
public enum MagicAttackEffectType { BreakClinch, Disarm, Stagger, Knockdown, Pushback, Pull }
public enum MagicDefenseMode { Opposed, Charged, Absorption }

[Flags]
public enum MagicDefenseThreat
{
	None = 0,
	Weapon = 1,
	Natural = 2,
	Ranged = 4,
	Magic = 8,
	Control = 16
}

public sealed record MagicAttackEffect(MagicAttackEffectType Type, Difficulty Resistance, double Strength,
	double DurationSeconds, string SuccessEmote, string ResistEmote)
{
	public const double MaximumStrength = 1000;
	public const double MaximumDurationSeconds = 86400;
	public bool HasValidParameters => double.IsFinite(Strength) && Strength > 0 && Strength <= MaximumStrength &&
		double.IsFinite(DurationSeconds) && DurationSeconds >= 0 && DurationSeconds <= MaximumDurationSeconds;
}

public interface IMagicDefensePower : IMagicPower
{
	MagicDefenseMode DefenseMode { get; }
	MagicDefenseThreat Threats { get; }
	ITraitDefinition DefenseTrait { get; }
	Difficulty DefenseDifficulty { get; }
	double ReactionStamina { get; }
	int MaximumCharges { get; }
	double MaximumCapacity { get; }
	bool CanDefend(ICharacter defender, ICombatMove attack);
	void ConsumeReaction(ICharacter defender);
}
