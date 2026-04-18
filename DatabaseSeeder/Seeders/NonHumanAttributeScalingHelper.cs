#nullable enable

using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseSeeder.Seeders;

internal sealed record NonHumanAttributeProfile(
	int StrengthBonus,
	int ConstitutionBonus,
	int AgilityBonus,
	int DexterityBonus)
{
	public NonHumanAttributeProfile Add(NonHumanAttributeProfile other)
	{
		return new(
			StrengthBonus + other.StrengthBonus,
			ConstitutionBonus + other.ConstitutionBonus,
			AgilityBonus + other.AgilityBonus,
			DexterityBonus + other.DexterityBonus
		);
	}

	public NonHumanAttributeProfile Clamp(int minimum, int maximum)
	{
		return new(
			Math.Clamp(StrengthBonus, minimum, maximum),
			Math.Clamp(ConstitutionBonus, minimum, maximum),
			Math.Clamp(AgilityBonus, minimum, maximum),
			Math.Clamp(DexterityBonus, minimum, maximum)
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

	internal static string BuildAttributeBonusProgText(IEnumerable<TraitDefinition> attributes, NonHumanAttributeProfile profile)
	{
		var physicalHybridBonus = (int)Math.Round(
			(profile.StrengthBonus + profile.ConstitutionBonus) / 2.0,
			MidpointRounding.AwayFromZero);

		var sb = new StringBuilder();
		sb.AppendLine("switch (@trait.Name)");

		foreach (var attribute in attributes.OrderBy(x => x.Id))
		{
			sb.AppendLine($"  case (\"{attribute.Name}\")");
			sb.AppendLine($"    return {GetAttributeBonus(attribute.Name, profile, physicalHybridBonus)}");
		}

		sb.AppendLine("end switch");
		sb.AppendLine("return 0");
		return sb.ToString();
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

		return 0;
	}
}
