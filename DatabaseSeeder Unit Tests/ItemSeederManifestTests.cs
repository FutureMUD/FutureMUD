#nullable enable

using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederManifestTests
{
	[TestMethod]
	public void ModuleGraph_HasUniqueKeysAndResolvableDependencies()
	{
		var modules = ItemSeederManifestCatalogue.Modules;
		Assert.AreEqual(modules.Count, modules.Select(x => x.Key).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (var module in modules)
		{
			foreach (var dependency in module.Dependencies)
			{
				Assert.IsTrue(modules.Any(x => x.Key.Equals(dependency, StringComparison.OrdinalIgnoreCase)),
					$"{module.Key} -> {dependency}");
			}
		}
	}

	[TestMethod]
	public void CanonicalManifest_IsValidAndCurrent()
	{
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var path = Path.Combine(root,
			ItemSeederManifestCatalogue.DefaultRelativePath.Replace('/', Path.DirectorySeparatorChar));
		Assert.IsTrue(File.Exists(path), "The checked-in ItemSeeder manifest is missing.");
		var document = ItemSeederManifestCatalogue.Load(path);
		Assert.AreEqual(ItemSeederManifestCatalogue.ManifestVersion, document.ManifestVersion);
		Assert.AreEqual(ItemSeederManifestCatalogue.ComputeSourceFingerprint(root), document.SourceFingerprint,
			"The ItemSeeder sources changed without refreshing Seeded_Item_Manifest.json.");
		Assert.IsTrue(document.Entries.Any(x => x.EntityType == "item"));
		Assert.IsTrue(document.Entries.Any(x => x.EntityType == "craft"));
		Assert.IsTrue(document.Entries.Any(x => x.EntityType == "vehicle"));
	}

	[TestMethod]
	public void LaterNoOpEras_AreNotAdvertised()
	{
		var question = new DatabaseSeeder.Seeders.ItemSeeder().SeederQuestions.Single(x => x.Id == "eras");
		foreach (var unsupported in new[] { "revolution", "modern", "atomic", "computer" })
		{
			Assert.IsFalse(question.Validator(unsupported, null!).Success, unsupported);
		}
	}

	[TestMethod]
	public void PersistenceWrites_AreConfinedToManifestAppliers()
	{
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var seederPath = Path.Combine(root, "DatabaseSeeder", "Seeders");
		var allowed = new[]
		{
			"ItemSeeder.cs",
			"ItemSeeder.Manifest.cs",
			"ItemSeeder.Crafting.cs",
			"ItemSeeder.AntiquityComponentGaps.cs",
			"ItemSeeder.AntiquityFood.cs",
			"ItemSeeder.AntiquityWriting.cs",
			"ItemSeeder.PreIndustrialFoodCatalogue.cs",
			"ItemSeeder.ClothingOutfitManifests.cs",
			"ItemSeeder.Vehicles.Persistence.cs",
			"ItemSeeder.Vehicles.Children.cs",
			"ItemSeeder.Vehicles.Children.Projections.cs"
		}.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var offenders = Directory.GetFiles(seederPath, "ItemSeeder*.cs")
			.Where(path => !allowed.Contains(Path.GetFileName(path)))
			.Where(path =>
			{
				var source = File.ReadAllText(path);
				return Regex.IsMatch(source,
					@"(?:_context!?|context)\.[A-Za-z0-9_]+\.(?:Add|AddRange|Remove|RemoveRange)\(");
			})
			.Select(Path.GetFileName)
			.ToArray();
		Assert.AreEqual(0, offenders.Length,
			$"Direct ItemSeeder persistence was added outside a reviewed manifest applier: {string.Join(", ", offenders)}");
	}
}
