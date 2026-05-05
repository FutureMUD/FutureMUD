#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RpiLegacySkillBaselineTests
{
	[TestMethod]
	public void SkillPackageSeeder_ComplexNonGerundNames_IncludeRpiLegacyGeneralAndCraftSkills()
	{
		var names = new SkillPackageSeeder()
			.ComplexNonGerundSkillNamesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		AssertContainsAll(names, new[]
		{
			"Swimming",
			"Scan",
			"Tracking",
			"Healing",
			"Picklock",
			"Butchery",
			"Metalcraft",
			"Woodcraft",
			"Textilecraft",
			"Cookery",
			"Baking",
			"Hideworking",
			"Stonecraft",
			"Candlery",
			"Brewing",
			"Distilling",
			"Dyecraft",
			"Apothecary",
			"Glasswork",
			"Gemcraft",
			"Milling",
			"Mining",
			"Perfumery",
			"Pottery",
			"Farming",
			"Ritual",
			"Barter",
			"Poisoning",
			"Alchemy",
			"Clairvoyance",
			"Danger-Sense",
			"Empathy",
			"Hex",
			"Psychic-Bolt",
			"Prescience",
			"Aura-Sight",
			"Telepathy",
			"Sarati",
			"Tengwar",
			"Cirth",
			"Valarin-Script",
			"Black-Wise",
			"Grey-Wise",
			"White-Wise",
			"Runecasting",
			"Astronomy",
			"Sling",
			"Blowgun",
			"Eavesdrop"
		});
	}

	[TestMethod]
	public void CombatSeeder_NonGerundNames_IncludeRpiLegacyCombatSkills()
	{
		var names = CombatSeeder.RpiNonGerundCombatSkillNamesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		AssertContainsAll(names, new[]
		{
			"Brawling",
			"Subdue",
			"Throwing",
			"Small-Blade",
			"Sword",
			"Axe",
			"Polearm",
			"Club",
			"Flail",
			"Double-Handed",
			"Sole-Wield",
			"Shield-Use",
			"Dual-Wield",
			"Blowgun",
			"Sling",
			"Hunting-bow",
			"Warbow",
			"Avert"
		});
	}

	[TestMethod]
	public void CultureSeeder_MiddleEarthLanguages_IncludeRpiLegacyLanguageTraits()
	{
		var names = CultureSeeder.RpiLegacyMiddleEarthLanguageNamesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		AssertContainsAll(names, new[]
		{
			"Taliska",
			"Haladin",
			"Thrunon",
			"Beast-Tongue",
			"Valarin",
			"Nandorin",
			"Druag",
			"Sindarin",
			"Quenya",
			"Avarin",
			"Khuzdul",
			"Orkish",
			"Trollish",
			"Atliduk",
			"Haradaic",
			"Dunael",
			"Labba",
			"Norliduk",
			"Talathic",
			"Umitic",
			"Nahaiduk",
			"Pukael"
		});
	}

	private static void AssertContainsAll(HashSet<string> actual, IReadOnlyCollection<string> expected)
	{
		var missing = expected
			.Where(x => !actual.Contains(x))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		Assert.AreEqual(0, missing.Count, $"Missing seeded RPI names: {string.Join(", ", missing)}");
	}
}
