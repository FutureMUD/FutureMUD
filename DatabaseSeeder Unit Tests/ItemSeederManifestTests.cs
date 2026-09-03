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

		foreach (var expected in new[]
		         {
			         "shared-industrialised", "industrial", "modern", "nuclear", "information"
		         })
		{
			Assert.IsTrue(modules.Any(x => x.Key.Equals(expected, StringComparison.OrdinalIgnoreCase)), expected);
		}
	}

	[TestMethod]
	public void RuntimeManifestLoad_RejectsOlderContractEvenWhenEntriesAreOtherwiseValid()
	{
		var path = Path.GetTempFileName();
		try
		{
			var document = ItemSeederManifestCatalogue.BuildDocument([], "test") with { ManifestVersion = "2" };
			File.WriteAllText(path, ItemSeederManifestCatalogue.Serialize(document));
			StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() => ItemSeederManifestCatalogue.Load(path)).Message,
				"recapture the manifest");
		}
		finally { File.Delete(path); }
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
	public void IndustrialEra_IsAdvertisedButLaterNoOpErasAreNot()
	{
		var question = new DatabaseSeeder.Seeders.ItemSeeder().SeederQuestions.Single(x => x.Id == "eras");
		Assert.IsTrue(question.Validator("industrial", null!).Success);
		Assert.IsTrue(question.Validator("revolution", null!).Success);
		foreach (var unsupported in new[]
		         {
			         "modern", "nuclear", "atomic", "information", "computer"
		         })
		{
			Assert.IsFalse(question.Validator(unsupported, null!).Success, unsupported);
		}
	}

	[TestMethod]
	public void LaterEraRegistry_UsesReadableCanonicalKeysAndLegacyVehicleAliases()
	{
		var definitions = DatabaseSeeder.Seeders.ItemSeeder.EraDefinitionsForTesting;
		Assert.AreEqual(8, definitions.Count);
		Assert.AreEqual(5, definitions.Count(x => x.Selectable));
		Assert.IsTrue(definitions.Single(x => x.Key == "industrial").Aliases.Contains("revolution"));
		Assert.AreEqual("revolution", definitions.Single(x => x.Key == "industrial").VehicleEraKey);
		Assert.IsTrue(definitions.Single(x => x.Key == "nuclear").Aliases.Contains("atomic"));
		Assert.AreEqual("atomic", definitions.Single(x => x.Key == "nuclear").VehicleEraKey);
		Assert.IsTrue(definitions.Single(x => x.Key == "information").Aliases.Contains("computer"));
		Assert.AreEqual("computer", definitions.Single(x => x.Key == "information").VehicleEraKey);
	}

	[TestMethod]
	public void TechnologyProfileQuestions_RemainInsideItemSeederAndInactiveForCurrentEras()
	{
		var seeder = new DatabaseSeeder.Seeders.ItemSeeder();
		var questions = seeder.SeederQuestions.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
		var currentAnswers = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["eras"] = "medieval"
		};
		Assert.IsFalse(questions["technologyprofile"].Filter(null!, currentAnswers));

		var plannedAnswers = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["eras"] = "industrial",
			["technologyprofile"] = "custom"
		};
		Assert.IsTrue(questions["technologyprofile"].Filter(null!, plannedAnswers));
		Assert.IsTrue(questions["technologypower"].Filter(null!, plannedAnswers));
		Assert.IsTrue(questions["technologypaper"].Filter(null!, plannedAnswers));
		Assert.IsTrue(questions["technologytelecom"].Filter(null!, plannedAnswers));
		Assert.IsTrue(questions["technologynetworkmedia"].Filter(null!, plannedAnswers));
		Assert.IsTrue(questions["technologyvehicle"].Filter(null!, plannedAnswers));
		Assert.IsTrue(questions["technologyprofile"].Validator("neutral", null!).Success);
		Assert.IsFalse(questions["technologyprofile"].Validator("unknown", null!).Success);
		Assert.AreEqual(8, DatabaseSeeder.Seeders.ItemSeeder.TechnologyProfilesForTesting.Count);
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
