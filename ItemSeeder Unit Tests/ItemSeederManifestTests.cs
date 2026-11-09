#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederManifestTests
{
	[TestMethod]
	public void SourceFingerprint_TracksRelocatedPartialContents()
	{
		var root = Path.Combine(Path.GetTempPath(), $"seeder-fingerprint-{Guid.NewGuid():N}");
		var sourceDirectory = Path.Combine(root, "ItemSeeder", "Seeders", "ItemSeeder");
		Directory.CreateDirectory(sourceDirectory);
		try
		{
			File.WriteAllText(Path.Combine(sourceDirectory, "ItemSeeder.cs"), "root");
			var partialPath = Path.Combine(sourceDirectory, "ItemSeeder.Crafting.cs");
			File.WriteAllText(partialPath, "first");
			var original = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root);
			File.WriteAllText(partialPath, "second");
			Assert.AreNotEqual(original, ItemSeederManifestCatalogue.ComputeSourceFingerprint(root));
		}
		finally
		{
			Directory.Delete(root, true);
		}
	}

	[TestMethod]
	public void PackagedManifest_LoadsFromCompiledCatalogueWithoutSidecars()
	{
		var actual = ItemSeederManifestCatalogue.LoadForRuntime();
		Assert.AreSame(actual, ItemSeederManifestCatalogue.LoadForRuntime());
		Assert.IsTrue(actual.Entries.Count > 0);
		Assert.AreEqual("compiled", actual.SourceFingerprint);
		ItemSeederManifestCatalogue.Validate(actual.Entries);
	}

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
	public void RuntimeManifestLoad_RejectsOlderContractEvenWhenEntriesAreOtherwiseValid()
	{
		var path = Path.GetTempFileName();
		try
		{
			var document = ItemSeederManifestCatalogue.BuildDocument([], "test") with { ManifestVersion = "1" };
			File.WriteAllText(path, ItemSeederManifestCatalogue.Serialize(document));
			StringAssert.Contains(Assert.ThrowsExactly<InvalidDataException>(() => ItemSeederManifestCatalogue.Load(path)).Message,
				"recapture the manifest");
		}
		finally { File.Delete(path); }
	}

	[TestMethod]
	public void BundledSeederOwnershipRecordsRemainRecognisedByTheStandaloneProduct()
	{
		using var context = ItemSeederProgram.CreateItemManifestCaptureContext();
		var componentNames = new[]
		{
			"Container_Table", "Container_Armor_Stand", "Container_Weapon_Rack", "Container_Cot_Surface",
			"Container_Bed_Surface", "Container_Couch_Surface", "Container_Counter", "Container_Bench_Surface",
			"Container_Desk_Surface", "Container_Wide_Shelves", "Container_Open_Bin", "Container_Trunk",
			"TimePiece_Antiquity_Sundial", "WaterSource_Antiquity_PublicWell", "Dice_Antiquity_Knucklebones",
			"DragAid_Antiquity_FieldStretcher", "Locksmithing_Antiquity_BronzePoor", "ShopStall_Antiquity_OpenCounter",
			"MarketGoodWeight_Antiquity_StapleFood", "SealStamp_Antiquity_BronzeSignet", "Sealable_Envelope",
			"MeasuringInstrument_Antiquity_BalanceScale", "Insulation_Minor", "Destroyable_Misc", "Torch_Infinite",
			"Wear_Saddle", "Wear_Bridle", "Wear_Chanfron", "Wear_Criniere", "Wear_Croupiere",
			"Wear_Flanchards", "Wear_Peytral", "Wear_Caparison"
		};
		context.GameItemComponentProtos.AddRange(componentNames.Select((name, index) => new GameItemComponentProto
		{
			Id = index + 1,
			Type = "Test",
			Definition = "<Definition/>",
			Description = name,
			Name = name,
			RevisionNumber = 0
		}));
		context.Tags.Add(new Tag { Id = 1, Name = "Functions" });
		context.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Id = 1,
			Seeder = "Items",
			Module = "medieval",
			EntityType = "item",
			StableKey = "existing_bundled_item",
			AppliedFingerprint = "legacy-fingerprint",
			ManifestVersion = "2"
		});
		context.SaveChanges();

		var seeder = new ItemSeeder();
		Assert.AreEqual("Items", seeder.Name);
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
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

	}

	[TestMethod]
	public void PersistenceWrites_AreConfinedToManifestAppliers()
	{
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var seederPath = Path.Combine(root, "ItemSeeder", "Seeders", "ItemSeeder");
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
