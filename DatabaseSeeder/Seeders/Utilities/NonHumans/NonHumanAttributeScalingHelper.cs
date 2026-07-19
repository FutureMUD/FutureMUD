#nullable enable

using MudSharp.Models;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

internal sealed record NonHumanAttributeProfile(
	int StrengthBonus,
	int ConstitutionBonus,
	int AgilityBonus,
	int DexterityBonus,
	int WillpowerBonus = 0,
	int PerceptionBonus = 0,
	int AuraBonus = 0,
	string? IntelligenceDiceExpression = null,
	string? WillpowerDiceExpression = null,
	string? PerceptionDiceExpression = null,
	string? AuraDiceExpression = null)
{
	public NonHumanAttributeProfile Add(NonHumanAttributeProfile other)
	{
		return new(
			StrengthBonus + other.StrengthBonus,
			ConstitutionBonus + other.ConstitutionBonus,
			AgilityBonus + other.AgilityBonus,
			DexterityBonus + other.DexterityBonus,
			WillpowerBonus + other.WillpowerBonus,
			PerceptionBonus + other.PerceptionBonus,
			AuraBonus + other.AuraBonus,
			other.IntelligenceDiceExpression ?? IntelligenceDiceExpression,
			other.WillpowerDiceExpression ?? WillpowerDiceExpression,
			other.PerceptionDiceExpression ?? PerceptionDiceExpression,
			other.AuraDiceExpression ?? AuraDiceExpression
		);
	}

	public NonHumanAttributeProfile Clamp(int minimum, int maximum)
	{
		return new(
			Math.Clamp(StrengthBonus, minimum, maximum),
			Math.Clamp(ConstitutionBonus, minimum, maximum),
			Math.Clamp(AgilityBonus, minimum, maximum),
			Math.Clamp(DexterityBonus, minimum, maximum),
			Math.Clamp(WillpowerBonus, minimum, maximum),
			Math.Clamp(PerceptionBonus, minimum, maximum),
			Math.Clamp(AuraBonus, minimum, maximum),
			IntelligenceDiceExpression,
			WillpowerDiceExpression,
			PerceptionDiceExpression,
			AuraDiceExpression
		);
	}
}

internal static class NonHumanAttributeScalingHelper
{
	private static readonly HashSet<string> StrengthNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Strength",
		"Upper Body Strength"
	};

	private static readonly HashSet<string> ConstitutionNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Constitution",
		"Hardiness",
		"Endurance",
		"Stamina"
	};

	private static readonly HashSet<string> HybridPhysicalNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Body",
		"Physique"
	};

	private static readonly HashSet<string> AgilityNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Agility"
	};

	private static readonly HashSet<string> DexterityNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Dexterity"
	};

	private static readonly HashSet<string> IntelligenceNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Intelligence",
		"Mind"
	};

	private static readonly HashSet<string> WillpowerNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Willpower",
		"Will",
		"Resolve",
		"Grit"
	};

	private static readonly HashSet<string> PerceptionNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Perception",
		"Awareness"
	};

	private static readonly HashSet<string> AuraNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Aura",
		"Luck",
		"Spirit"
	};

	internal static int GetAttributeBonus(TraitDefinition attribute, NonHumanAttributeProfile profile)
	{
		var physicalHybridBonus = (int)Math.Round(
			(profile.StrengthBonus + profile.ConstitutionBonus) / 2.0,
			MidpointRounding.AwayFromZero);
		return GetAttributeBonus(attribute.Name, profile, physicalHybridBonus);
	}

	private static int GetAttributeBonus(string attributeName, NonHumanAttributeProfile profile, int physicalHybridBonus)
	{
		if (StrengthNames.Contains(attributeName))
		{
			return profile.StrengthBonus;
		}

		if (ConstitutionNames.Contains(attributeName))
		{
			return profile.ConstitutionBonus;
		}

		if (HybridPhysicalNames.Contains(attributeName))
		{
			return physicalHybridBonus;
		}

		if (AgilityNames.Contains(attributeName))
		{
			return profile.AgilityBonus;
		}

		if (DexterityNames.Contains(attributeName))
		{
			return profile.DexterityBonus;
		}

		if (WillpowerNames.Contains(attributeName))
		{
			return profile.WillpowerBonus;
		}

		if (PerceptionNames.Contains(attributeName))
		{
			return profile.PerceptionBonus;
		}

		if (AuraNames.Contains(attributeName))
		{
			return profile.AuraBonus;
		}

		return 0;
	}

	internal static string? GetAttributeDiceExpression(TraitDefinition attribute, NonHumanAttributeProfile profile)
	{
		if (IntelligenceNames.Contains(attribute.Name))
		{
			return profile.IntelligenceDiceExpression;
		}

		if (WillpowerNames.Contains(attribute.Name))
		{
			return profile.WillpowerDiceExpression;
		}

		if (PerceptionNames.Contains(attribute.Name))
		{
			return profile.PerceptionDiceExpression;
		}

		if (AuraNames.Contains(attribute.Name))
		{
			return profile.AuraDiceExpression;
		}

		return null;
	}
}
