#nullable enable

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
		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.Renaissance.cs");
		var dispatcher = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.cs");

		StringAssert.Contains(dispatcher, "SeedRenaissanceFinishedItemCrafts();");
		StringAssert.Contains(source, "renaissance_jewellery_");
		StringAssert.Contains(source, "renaissance_door_");
		StringAssert.Contains(source, "an in-progress {StripLeadingArticle(displayName)} craft");
		StringAssert.Contains(source, "Commodity - {materialAmount} grams of {material}");
		StringAssert.Contains(source, "SimpleProduct - 1x {displayName} (#{item.Value.Id})");
		Assert.IsTrue(ItemSeeder.ShouldSeedRenaissanceFinishedItemCraftsForTesting("renaissance"));
		Assert.IsFalse(ItemSeeder.ShouldSeedRenaissanceFinishedItemCraftsForTesting("medieval"));
	}

	[TestMethod]
	public void RenaissanceCraftToolTags_HaveSharedProvidersForBothTargetEras()
	{
		var manifest = LoadManifest();
		var sharedItems = manifest.Entries
			.Where(x => x.EntityType.Equals("item", StringComparison.OrdinalIgnoreCase))
			.ToDictionary(x => x.StableKey, StringComparer.OrdinalIgnoreCase);
		var historicSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.HistoricFoundation.cs");
		var aliasSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialBaseline.Aliases.cs");

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
