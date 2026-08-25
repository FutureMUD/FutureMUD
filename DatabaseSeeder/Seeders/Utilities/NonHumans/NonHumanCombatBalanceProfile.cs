#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

internal enum NonHumanCombatTier
{
	Nuisance = 0,
	MinorThreat = 1,
	SeriousThreat = 2,
	EliteThreat = 3,
	Monster = 4,
	GreatBeast = 5,
	PartyBoss = 6,
	Avatar = 7
}

internal sealed record NonHumanCombatBalanceProfile(
	NonHumanCombatTier Tier,
	string BaselineKey,
	double PainToleranceMultiplier,
	ItemQuality NaturalArmourQuality,
	string AttackProfileKey,
	bool GrantBehemothCharge,
	string? SignatureActionKey = null);

internal static class NonHumanCombatBalanceProfileHelper
{
	private const int AverageUnmodifiedAttribute = 11;

	internal static NonHumanAttributeProfile WithEffectiveTargets(
		NonHumanAttributeProfile source,
		int strength,
		int constitution,
		int agility,
		int dexterity,
		int willpower,
		int perception,
		int aura)
	{
		return new(
			strength - AverageUnmodifiedAttribute,
			constitution - AverageUnmodifiedAttribute,
			agility - AverageUnmodifiedAttribute,
			dexterity - AverageUnmodifiedAttribute,
			willpower - AverageUnmodifiedAttribute,
			perception - AverageUnmodifiedAttribute,
			aura - AverageUnmodifiedAttribute,
			source.IntelligenceDiceExpression,
			source.WillpowerDiceExpression,
			source.PerceptionDiceExpression,
			source.AuraDiceExpression);
	}
}
