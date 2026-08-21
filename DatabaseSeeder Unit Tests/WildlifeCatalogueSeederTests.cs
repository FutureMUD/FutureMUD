#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WildlifeCatalogueSeederTests
{
	[TestMethod]
	public void WildlifeCatalogue_SourceContract_HasNoValidationIssues()
	{
		IReadOnlyList<string> issues = WildlifeCatalogue.ValidateCatalogForTesting();
		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues));
	}

	[TestMethod]
	public void WildlifeCatalogue_MapsEveryNormalAnimalExactlyOnce()
	{
		HashSet<string> expected = AnimalSeeder.AnimalAIRecommendationsForTesting.Keys
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		IReadOnlyList<WildlifeRecommendation> recommendations = WildlifeCatalogue.Recommendations
			.Where(x => !x.Mythical)
			.ToArray();
		HashSet<string> actual = recommendations.Select(x => x.RaceName)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		Assert.IsTrue(expected.SetEquals(actual), "Every installed normal animal should have one finished wild recommendation.");
		Assert.AreEqual(actual.Count, recommendations.Count,
			"No normal animal should have duplicate finished wild recommendations.");
		Assert.IsTrue(recommendations.All(x => x.IndividualAiTemplate.StartsWith("Wildlife - ", StringComparison.Ordinal)),
			"Finished recommendations must never send builders back to legacy Animal example rows.");
	}

	[TestMethod]
	public void WildlifeCatalogue_MapsOnlyExplicitlyEligibleMythicalBeasts()
	{
		HashSet<string> eligible = MythicalAnimalSeeder.TemplatesForTesting.Values
			.Where(x => x.WildlifeEligible)
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		HashSet<string> ineligible = MythicalAnimalSeeder.TemplatesForTesting.Values
			.Where(x => !x.WildlifeEligible)
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		IReadOnlyList<WildlifeRecommendation> mythical = WildlifeCatalogue.Recommendations
			.Where(x => x.Mythical)
			.ToArray();
		HashSet<string> actual = mythical.Select(x => x.RaceName).ToHashSet(StringComparer.OrdinalIgnoreCase);

		Assert.IsTrue(eligible.SetEquals(actual),
			"Only non-sapient mythical races marked as wildlife-eligible in source metadata should receive wildlife controllers.");
		Assert.IsFalse(actual.Overlaps(ineligible),
			"Humanoid and sapient mythical peoples must not receive wildlife controller recommendations.");
		Assert.AreEqual(actual.Count, mythical.Count, "Eligible mythical beasts should map exactly once.");
	}

	[TestMethod]
	public void WildlifeCatalogue_ManagedAnimalsCoverAgricultureAndRequestedCompanions()
	{
		string[] requiredCompanions = ["Cat", "Dog", "Ferret", "Hamster", "Parrot", "Macaw", "Cockatoo", "Koi"];
		HashSet<string> required = AgricultureSeeder.StockHerdAnimalRaceNamesForWildlife
			.Concat(requiredCompanions)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (string raceName in required)
		{
			WildlifeRecommendation recommendation = WildlifeCatalogue.Recommendations.Single(x =>
				!x.Mythical && x.RaceName.Equals(raceName, StringComparison.OrdinalIgnoreCase));
			Assert.IsNotNull(recommendation.ManagedIndividualAiTemplate,
				$"{raceName} should retain a distinct managed individual controller.");
			Assert.IsNotNull(recommendation.ManagedGroupTemplate,
				$"{raceName} should retain a distinct managed group recommendation.");
			Assert.IsTrue(recommendation.ManagedIndividualAiTemplate!.StartsWith("Managed Animal - ", StringComparison.Ordinal));
			Assert.IsTrue(recommendation.ManagedGroupTemplate!.StartsWith("Managed Animal Group - ", StringComparison.Ordinal));
		}

		CollectionAssert.AreEquivalent(required.OrderBy(x => x).ToArray(),
			WildlifeCatalogue.ManagedEligibleRaceNames.OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void WildlifeCatalogue_SocialRecommendationsResolveToFinishedGroupTemplatesOrSolitary()
	{
		HashSet<string> groupNames = WildlifeCatalogue.GroupTemplateNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (WildlifeRecommendation recommendation in WildlifeCatalogue.Recommendations)
		{
			Assert.IsTrue(recommendation.GroupTemplate == "Solitary" || groupNames.Contains(recommendation.GroupTemplate),
				$"{recommendation.RaceName} should resolve to a finished group template or explicit Solitary.");
		}

		string[] requiredTemplates =
		[
			"Wildlife Group - Timid Grazing Herd", "Wildlife Group - Cursorial Hunting Pack",
			"Wildlife Group - Arboreal-Roost Flock", "Wildlife Group - Marine School",
			"Wildlife Group - Surface-Breathing Pod", "Wildlife Group - Insect Colony and Swarm",
			"Wildlife Group - Mythic Aerial Hunting Flight", "Managed Animal Group - Livestock Herd",
			"Managed Animal Group - Poultry and Waterfowl Flock", "Managed Animal Group - Companion Pack"
		];
		CollectionAssert.IsSubsetOf(requiredTemplates, groupNames.ToArray());
	}

	[TestMethod]
	public void WildlifeCatalogue_GroupTemplatesUseTheSingleTargetThreatContract()
	{
		string[] validGroupThreatProgs =
		[
			WildlifeCatalogue.GroupAnimalPreyProg,
			WildlifeCatalogue.GroupIntruderProg
		];

		Assert.IsTrue(WildlifeCatalogue.GroupTemplates.All(x =>
			validGroupThreatProgs.Contains(x.ThreatProg, StringComparer.OrdinalIgnoreCase)),
			"Group templates must use a one-character threat prog, because GroupAITemplate supplies the candidate rather than an individual animal and candidate pair.");
		Assert.IsFalse(WildlifeCatalogue.GroupTemplates.Any(x =>
			string.Equals(x.ThreatProg, WildlifeCatalogue.AnimalPreyProg, StringComparison.OrdinalIgnoreCase) ||
			string.Equals(x.ThreatProg, WildlifeCatalogue.IntruderProg, StringComparison.OrdinalIgnoreCase)),
			"Individual two-character threat progs must not be assigned to group templates.");
		CollectionAssert.IsSubsetOf(validGroupThreatProgs,
			WildlifeCatalogue.SupportProgNames.ToArray());
	}

	[TestMethod]
	public void WildlifeCatalogue_ProfilesAndTaxonomyAreFinishedAndComplete()
	{
		string[] requiredHabitats =
		[
			"Grassland", "Shrubland", "Woodland", "Highland", "Cliff", "Cave", "Subterranean", "Wetland",
			"Riverine", "Freshwater", "Lake", "Marine", "Coast", "Open Ocean", "Reef", "Polar", "Tundra",
			"Desert", "Agricultural Land"
		];
		string[] requiredShelters = ["Burrow", "Den", "GroundNest", "TreeNest", "Lair", "Lodge", "WebNest"];

		CollectionAssert.AreEquivalent(requiredHabitats.OrderBy(x => x).ToArray(),
			WildlifeCatalogue.HabitatTagNames.OrderBy(x => x).ToArray());
		CollectionAssert.AreEquivalent(requiredShelters.OrderBy(x => x).ToArray(),
			WildlifeCatalogue.Shelters.Select(x => x.Key).OrderBy(x => x).ToArray());
		string[] exactStockTerrainNames =
		[
			"Grasslands", "Shrublands", "Shortgrass Prairie", "Tallgrass Prairie", "Field", "Chaparral",
			"Escarpment", "Mountain Ridge", "Grotto", "Cave Entrance", "Mudflat", "Deep Ocean"
		];
		CollectionAssert.IsSubsetOf(exactStockTerrainNames,
			WildlifeCatalogue.StockTerrainHabitatTags.Keys.ToArray(),
			"Wildlife habitat metadata should address exact CoreDataSeeder terrain names rather than invented aliases.");
		Assert.IsFalse(WildlifeCatalogue.StockTerrainHabitatTags.ContainsKey("Grassland"));
		Assert.IsFalse(WildlifeCatalogue.StockTerrainHabitatTags.ContainsKey("Scrub"));
		Assert.IsTrue(WildlifeCatalogue.IndividualProfiles.All(x =>
			x.Name.StartsWith("Wildlife - ", StringComparison.Ordinal) ||
			x.Name.StartsWith("Managed Animal - ", StringComparison.Ordinal)),
			"The finished catalogue should use stock-owned names rather than anonymous examples.");
		Assert.IsFalse(WildlifeCatalogue.IndividualProfiles.Any(x =>
			x.PreferredHabitatProg.Contains("Always", StringComparison.OrdinalIgnoreCase) ||
			x.ToleratedHabitatProg.Contains("Always", StringComparison.OrdinalIgnoreCase)),
			"Finished habitat policy must not fall back to an inert always-true placeholder.");
		Assert.IsTrue(WildlifeCatalogue.IndividualProfiles.Any(x => x.Nesting && x.NestingSeason is not null),
			"Finished nesting profiles must use a hemisphere-aware nesting-season policy.");
		Assert.IsTrue(WildlifeCatalogue.IndividualProfiles.Any(x =>
			x.SeasonalHabitatSeason is not null && x.SeasonalHabitatProg is not null),
			"Finished profiles must exercise season-specific preferred habitats rather than leaving that policy inert.");
	}

	[TestMethod]
	public void WildlifeCatalogue_ManifestIsDeterministicAndContainsEcologyMetadata()
	{
		using JsonDocument manifest = JsonDocument.Parse(WildlifeCatalogue.RecommendationManifestJsonForTesting);
		JsonElement[] rows = manifest.RootElement.EnumerateArray().ToArray();

		Assert.AreEqual(WildlifeCatalogue.Recommendations.Count, rows.Length);
		Assert.IsTrue(rows.All(x => x.TryGetProperty("PreferredHabitat", out _) &&
		                             x.TryGetProperty("ActivityPolicy", out _) &&
		                             x.TryGetProperty("ThreatPolicy", out _) &&
		                             x.TryGetProperty("SensesPolicy", out _)),
			"The generated manifest should be a usable builder-facing ecology recommendation record.");
		CollectionAssert.AreEqual(rows.Select(x => x.GetProperty("RaceName").GetString()).OrderBy(x => x).ToArray(),
			rows.Select(x => x.GetProperty("RaceName").GetString()).ToArray(),
			"The manifest should be stable-sorted by race name for source-control review.");
	}
}
