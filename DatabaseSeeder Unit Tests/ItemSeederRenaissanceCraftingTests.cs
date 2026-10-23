#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederRenaissanceCraftingTests
{
	[TestMethod]
	public void RenaissanceFinishedItemCrafts_AreWiredAndDoNotDuplicateSpecialistJewelleryOrDoorRoutes()
	{
		var recipes = Recipes.Recipes.Where(recipe => recipe.Gate is CatalogueKnowledgeRecipeGate gate && gate.Knowledge == "Renaissance Finished Item Crafting").ToArray();
		Assert.IsTrue(recipes.Length > 1000);
		Assert.IsFalse(recipes.SelectMany(Recipes.Outputs).Any(key => key.StartsWith("renaissance_jewellery_", StringComparison.Ordinal) || key.StartsWith("renaissance_door_", StringComparison.Ordinal)));
	}

	[TestMethod]
	public void RenaissanceCraftToolTags_HaveSharedProvidersForBothTargetEras()
	{
		var manifest = LoadManifest();
		var sharedItems = manifest.Entries
			.Where(x => x.EntityType.Equals("item", StringComparison.OrdinalIgnoreCase))
			.ToDictionary(x => x.StableKey, StringComparer.OrdinalIgnoreCase);
		var historicSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.HistoricFoundation.cs");
		var aliasSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.PreIndustrialBaseline.Aliases.cs");

		foreach (var requirement in ItemSeeder.RenaissanceCraftToolRequirementsForTesting)
		{
			Assert.IsTrue(sharedItems.TryGetValue(requirement.ProviderStableReference, out var provider),
				$"Missing shared provider {requirement.ProviderStableReference} for {requirement.Tag}.");
			Assert.IsTrue(provider.EraAdmissions.Contains("medieval", StringComparer.OrdinalIgnoreCase),
				$"{requirement.ProviderStableReference} is not admitted to Medieval.");
			Assert.IsTrue(provider.EraAdmissions.Contains("renaissance", StringComparer.OrdinalIgnoreCase),
				$"{requirement.ProviderStableReference} is not admitted to Renaissance.");
			Assert.IsTrue(historicSource.Contains(requirement.RequiredTagPath, StringComparison.OrdinalIgnoreCase) ||
			              aliasSource.Contains(requirement.RequiredTagPath, StringComparison.OrdinalIgnoreCase),
				$"{requirement.ProviderStableReference} does not declare {requirement.RequiredTagPath} in its stock source.");
		}
	}

	[TestMethod]
	public void MedievalAndRenaissanceDirectItemCraftCoverage_ExceedsNinetyEightPercent()
	{
		var manifest = LoadManifest();
		var targets = manifest.Entries
			.Where(x => x.EntityType.Equals("item", StringComparison.OrdinalIgnoreCase))
			.Where(x => x.StableKey.StartsWith("medieval_", StringComparison.OrdinalIgnoreCase) ||
			            x.StableKey.StartsWith("renaissance_", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.StableKey)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var craftedTargets = manifest.Entries
			.Where(x => x.EntityType.Equals("craft", StringComparison.OrdinalIgnoreCase))
			.SelectMany(x => x.Dependencies)
			.Where(x => x.StartsWith("item:", StringComparison.OrdinalIgnoreCase))
			.Select(x => x["item:".Length..])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var covered = targets.Count(craftedTargets.Contains);
		var coverage = targets.Count == 0 ? 0.0 : (double)covered / targets.Count;

		Assert.IsTrue(coverage > 0.98,
			$"Medieval/Renaissance direct-item craft coverage is {coverage:P2} ({covered:N0}/{targets.Count:N0}). Missing: {string.Join(", ", targets.Except(craftedTargets, StringComparer.OrdinalIgnoreCase).Take(20))}");
	}

	private static ItemSeederManifestDocument LoadManifest()
	{
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var path = Path.Combine(root,
			ItemSeederManifestCatalogue.DefaultRelativePath.Replace('/', Path.DirectorySeparatorChar));
		return ItemSeederManifestCatalogue.Load(path);
	}

	private static string ReadSource(params string[] parts)
	{
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		return File.ReadAllText(Path.Combine([root, .. parts]));
	}
}
