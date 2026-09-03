#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.Models;
using WearProfile = MudSharp.Models.WearProfile;
using Match = System.Text.RegularExpressions.Match;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HumanSeederIndustrialisedWearProfileTests
{
	internal static readonly string[] ExpectedProfiles =
	[
		"Split Drawers", "Camisole", "Nappy", "Infant Bodysuit", "Infant Gown", "Short Stays", "Rear Skirt Support",
		"Girdle", "Stocking Support Belt", "High-Neck Shirt", "Bib Overalls", "Trained Skirt", "Cutaway Coat", "Tailcoat",
		"Hooded Long Coat", "Hooded Long Coat Lowered", "Detachable Collar", "Standing Collar", "Detachable Cuffs", "Shirtfront", "Braces", "Rank Slides",
		"Ribbon Bar", "Long Gloves", "Bonnet", "Hairnet", "Net Skirt", "Waist Apron", "Open-Back Gown", "High-Neck Jacket", "Garters",
		"Cutaway Jacket", "Low-Cut Shoes", "Backless Shoes", "Infant Footwear", "High-Neck Dress", "Short Wrap Jacket",
		"High-Waisted Skirt", "Wide Over-Robe", "Clerical Collar", "Academic Hood", "Nappy Pins", "Spats"
	];

	[TestMethod]
	public void Stock_ProvidesApprovedProfileGapsLoweredHoodAndHistoricalGartersWithValidNonduplicateGeometry()
	{
		Assert.AreEqual(43, ExpectedProfiles.Length);
		var profiles = HumanSeeder.AdditionalHumanWearProfileXmlForTesting;
		foreach (var name in ExpectedProfiles)
		{
			Assert.IsTrue(profiles.ContainsKey(name), name);
			var root = XElement.Parse(profiles[name]);
			Assert.IsTrue(root.Elements().Any(x => (bool)x.Attribute("Mandatory")!));
			Assert.AreEqual(root.Elements().Count(), root.Elements().Select(x => (string?)(x.Attribute("Bodypart") ?? x.Attribute("ShapeId"))).Distinct().Count(), name);
		}
		CollectionAssert.AreEquivalent(ExpectedProfiles.Where(x => x is not ("Hooded Long Coat Lowered" or "Garters")).ToArray(), IndustrialisedClothingDependencyPlan.Rows.Where(x => !x.Reused)
			.Select(x => x.WearProfile!).Where(ExpectedProfiles.Contains).Distinct().ToArray());
	}

	[TestMethod]
	public void FreshAndIdenticalRerun_HaveExactProfileReferencesStableIdsAndNoChanges()
	{
		using var fixture = new Fixture();
		Assert.IsTrue(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.IsTrue(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		var before = fixture.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		CollectionAssert.AreEqual(before, fixture.Signatures());
		foreach (var name in ExpectedProfiles)
		{
			var profile = fixture.Context.WearProfiles.Single(x => x.Name == name);
			Assert.AreEqual(1L, profile.BodyPrototypeId);
			var component = fixture.Context.GameItemComponentProtos.Single(x => x.Name == ComponentName(name));
			Assert.AreEqual("Wearable", component.Type);
			var xml = XElement.Parse(component.Definition);
			var references = xml.Element("Profiles")!.Elements("Profile").Select(x => (long)x).ToArray();
			CollectionAssert.Contains(references, profile.Id);
			CollectionAssert.Contains(references, (long)xml.Element("Profiles")!.Attribute("Default")!);
			Assert.AreEqual(references.Length, references.Distinct().Count());
			Assert.IsTrue(references.All(id => fixture.Context.WearProfiles.Any(x => x.Id == id)));
		}
		var coat = XElement.Parse(fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Hooded_Long_Coat").Definition).Element("Profiles")!;
		Assert.AreEqual(fixture.Context.WearProfiles.Single(x => x.Name == "Hooded Long Coat Lowered").Id, (long)coat.Attribute("Default")!);
		Assert.AreEqual(2, coat.Elements("Profile").Count());
	}

	[TestMethod]
	public void Rerun_RestoresMissingComponentWithoutReplacingProfileOrOtherStock()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var profile = fixture.Context.WearProfiles.Single(x => x.Name == "Split Drawers");
		var profileId = profile.Id;
		var removed = fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Split_Drawers");
		fixture.Context.GameItemComponentProtos.Remove(removed);
		fixture.Context.SaveChanges();
		Assert.IsTrue(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.IsTrue(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		Assert.AreEqual(profileId, fixture.Context.WearProfiles.Single(x => x.Name == "Split Drawers").Id);
		Assert.AreEqual(1, fixture.Context.GameItemComponentProtos.Count(x => x.Name == "Wear_Split_Drawers"));
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
	}

	[TestMethod]
	public void Rerun_PreservesCustomizedNewProfilesAndEveryExistingComponentRevision()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var profile = fixture.Context.WearProfiles.Single(x => x.Name == "Camisole");
		profile.Description = "Builder's alternative camisole coverage";
		profile.WearlocProfiles = "<Profiles><Profile Bodypart=\"belly\" Mandatory=\"true\" Transparent=\"false\" NoArmour=\"false\" PreventsRemoval=\"false\" HidesSevered=\"false\" /></Profiles>";
		var component = fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Camisole");
		component.Description = "Builder's lightweight camisole";
		component.Definition = component.Definition.Replace("</Definition>", "<LayerWeightConsumption>0.1</LayerWeightConsumption></Definition>", StringComparison.Ordinal);
		fixture.Context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Id = component.Id, RevisionNumber = 1, Name = component.Name, Type = component.Type,
			Description = "Builder's later revision", Definition = component.Definition
		});
		fixture.Context.SaveChanges();
		var before = fixture.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		CollectionAssert.AreEqual(before, fixture.Signatures());
	}

	[DataTestMethod]
	[DataRow(false, true)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(true, false)]
	public void All43PersistedProfiles_ResolveAgainstStockHumanNamesShapesAndWearablePartTypes(bool explicitBones, bool inventoryHands)
	{
		using var fixture = new Fixture(explicitBones, inventoryHands);
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var geometry = IndustrialisedClothingWearProfiles.Read(fixture.Context);
		foreach (var profile in fixture.Context.WearProfiles.AsEnumerable().Where(x => ExpectedProfiles.Contains(x.Name)))
		{
			var source = new ClothingSourceLocation("HumanSeeder.WearProfiles.Industrialised.cs", 1);
			var bound = geometry.Bind(profile, source);
			Assert.IsTrue(bound.Locations.Count > 0, profile.Name);
			Assert.IsTrue(bound.Locations.Any(x => x.Mandatory), profile.Name);
			var component = fixture.Context.GameItemComponentProtos.Single(x => x.Name == ComponentName(profile.Name));
			var holdable = new GameItemComponentProto { Id = 99999, RevisionNumber = 0, Name = "Holdable", Type = "Holdable", Definition = "<Definition/>" };
			var binding = IndustrialisedClothingPhysicalBindings.Bind([holdable, component], fixture.Context.WearProfiles.ToArray(), [], source);
			CollectionAssert.Contains(binding.ProfileIds.ToArray(), profile.Id);
		}
	}

	[TestMethod]
	public void PersistedCoverage_MatchesActualRuntimeWearProfileLoaders()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var world = new Mock<IFuturemud>();
		var body = new Mock<IBodyPrototype>();
		body.Setup(x => x.Id).Returns(1);
		var shapes = fixture.Context.BodypartShapes.AsEnumerable().Select(row =>
		{
			var shape = new Mock<MudSharp.Form.Shape.IBodypartShape>();
			shape.Setup(x => x.Id).Returns(row.Id);
			shape.Setup(x => x.Name).Returns(row.Name);
			return shape.Object;
		}).ToArray();
		var parts = fixture.Context.BodypartProtos.AsEnumerable().Where(x => IndustrialisedClothingWearProfiles.IsWearLocation((BodypartTypeEnum)x.BodypartType)).Select(row =>
		{
			var part = new Mock<IExternalBodypart>();
			part.Setup(x => x.Id).Returns(row.Id);
			part.Setup(x => x.Name).Returns(row.Name);
			part.Setup(x => x.Shape).Returns(shapes.Single(x => x.Id == row.BodypartShapeId));
			part.As<IWear>();
			return part.Object;
		}).ToArray();
		body.Setup(x => x.AllExternalBodyparts).Returns(parts);
		body.Setup(x => x.AllBodyparts).Returns(parts);
		world.Setup(x => x.BodyPrototypes).Returns(Collection(body.Object));
		world.Setup(x => x.BodypartShapes).Returns(Collection(shapes));
		var geometry = IndustrialisedClothingWearProfiles.Read(fixture.Context);
		foreach (var row in fixture.Context.WearProfiles.AsEnumerable().Where(x => ExpectedProfiles.Contains(x.Name)))
		{
			var bound = geometry.Bind(row, new("HumanSeeder.WearProfiles.Industrialised.cs", 1));
			var runtime = MudSharp.GameItems.Inventory.WearProfile.LoadWearProfile(row, world.Object);
			Assert.IsTrue(runtime.AllProfiles.Count > 0, row.Name);
			if (!bound.IsShape) Assert.AreEqual(bound.Locations.Count, runtime.AllProfiles.Count, row.Name);
			foreach (var (part, coverage) in runtime.AllProfiles)
			{
				var expected = bound.Locations.Single(x => x.TargetId == (bound.IsShape ? part.Shape.Id : part.Id));
				Assert.AreEqual(expected.Mandatory, coverage.Mandatory, row.Name);
				Assert.AreEqual(expected.Transparent, coverage.Transparent, row.Name);
				Assert.AreEqual(expected.NoArmour, coverage.NoArmour, row.Name);
				Assert.AreEqual(expected.PreventsRemoval, coverage.PreventsRemoval, row.Name);
				Assert.AreEqual(expected.HidesSevered, coverage.HidesSeveredBodyparts, row.Name);
			}
		}
	}

	[DataTestMethod]
	[DataRow("Split Drawers", "groin")]
	[DataRow("Split Drawers", "penis")]
	[DataRow("Backless Shoes", "rheel")]
	[DataRow("Backless Shoes", "lheel")]
	[DataRow("Backless Shoes", "rankle")]
	[DataRow("Open-Back Gown", "uback")]
	[DataRow("Open-Back Gown", "rbuttock")]
	[DataRow("Short Stays", "belly")]
	[DataRow("Short Wrap Jacket", "abdomen")]
	[DataRow("High-Waisted Skirt", "rbreast")]
	[DataRow("Bib Overalls", "uback")]
	[DataRow("Bib Overalls", "rforearm")]
	[DataRow("Spats", "rbigtoe")]
	[DataRow("Spats", "rcalf")]
	[DataRow("Academic Hood", "scalp")]
	public void OpenAndShortGarments_DoNotAcquireUnintendedCoverage(string profile, string bodypart) =>
		Assert.IsFalse(XElement.Parse(HumanSeeder.AdditionalHumanWearProfileXmlForTesting[profile]).Elements()
			.Any(x => (string?)x.Attribute("Bodypart") == bodypart), $"{profile} must not cover {bodypart}.");

	[DataTestMethod]
	[DataRow("Hairnet")]
	[DataRow("Net Skirt")]
	[DataRow("Braces")]
	[DataRow("Rank Slides")]
	[DataRow("Nappy Pins")]
	[DataRow("Rear Skirt Support")]
	public void PartialAndMeshCoverage_DoesNotHideUnderlyingGarments(string profile) =>
		Assert.IsTrue(XElement.Parse(HumanSeeder.AdditionalHumanWearProfileXmlForTesting[profile]).Elements()
			.All(x => (bool)x.Attribute("Transparent")! && !(bool)x.Attribute("HidesSevered")!));

	[TestMethod]
	public void HighNecks_CoverNeckWhileHoodsKeepFaceOpenAndLowCutShoesKeepAnklesBare()
	{
		foreach (var name in new[] { "High-Neck Shirt", "High-Neck Jacket", "High-Neck Dress", "Standing Collar" })
		{
			var locations = XElement.Parse(HumanSeeder.AdditionalHumanWearProfileXmlForTesting[name]).Elements()
				.ToDictionary(x => (string)x.Attribute("Bodypart")!);
			foreach (var part in new[] { "neck", "throat", "bneck" }) Assert.IsFalse((bool)locations[part].Attribute("Transparent")!);
		}
		var hood = XElement.Parse(HumanSeeder.AdditionalHumanWearProfileXmlForTesting["Hooded Long Coat"]);
		var lowered = XElement.Parse(HumanSeeder.AdditionalHumanWearProfileXmlForTesting["Hooded Long Coat Lowered"]);
		var headParts = new[] { "scalp", "bhead", "rear", "lear", "rtemple", "ltemple" };
		CollectionAssert.AreEqual(lowered.Elements().Select(x => x.ToString()).ToArray(),
			hood.Elements().Where(x => !headParts.Contains((string)x.Attribute("Bodypart")!)).Select(x => x.ToString()).ToArray());
		Assert.IsTrue(hood.Elements().Any(x => (string?)x.Attribute("Bodypart") == "scalp"));
		Assert.IsFalse(hood.Elements().Any(x => (string?)x.Attribute("Bodypart") is "face" or "reye" or "leye" or "mouth"));
		var shoes = XElement.Parse(HumanSeeder.AdditionalHumanWearProfileXmlForTesting["Low-Cut Shoes"]);
		Assert.IsFalse(shoes.Elements().Any(x => (string?)x.Attribute("Bodypart") is "rankle" or "lankle"));
	}

	[TestMethod]
	public void MaintainedExport_ContainsExactInstalledStockNamesTypesAndDescriptions()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(ItemSeederManifestCatalogue.FindRepositoryRoot(), "Design Documents/Data/Seeded_Item_Components.json")));
		var rows = json.RootElement.EnumerateArray().ToDictionary(x => x.GetProperty("Component Name").GetString()!, StringComparer.Ordinal);
		var layerNames = IndustrialisedClothingDependencyPlan.WearLayerStock.Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
		foreach (var component in fixture.Context.GameItemComponentProtos.AsEnumerable().Where(x => layerNames.Contains(x.Name) || ExpectedProfiles.Any(p => ComponentName(p) == x.Name)))
		{
			Assert.IsTrue(rows.TryGetValue(component.Name, out var row), component.Name);
			Assert.AreEqual(component.Type, row.GetProperty("Component Type").GetString(), component.Name);
			Assert.AreEqual(component.Description, row.GetProperty("Component Description").GetString(), component.Name);
		}
	}

	[DataTestMethod]
	[DataRow(false, true)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(true, false)]
	public void All364Bases_ResolveStockGeometryAndNewBasesPreserveAuthoredThickness(bool explicitBones, bool inventoryHands)
	{
		using var fixture = new Fixture(explicitBones, inventoryHands);
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var geometry = IndustrialisedClothingWearProfiles.Read(fixture.Context);
		var profiles = fixture.Context.WearProfiles.ToArray();
		var components = fixture.Context.GameItemComponentProtos.ToArray();
		var holdable = new GameItemComponentProto { Id = -1, Name = "Holdable", Type = "Holdable", Definition = "<Definition/>" };
		var rows = IndustrialisedClothingDependencyPlan.Rows;
		Assert.AreEqual(364, rows.Count);
		foreach (var row in rows)
		{
			var component = components.Single(x => x.Type == "Wearable" && row.Components.Contains(x.Name));
			var bound = IndustrialisedClothingPhysicalBindings.Bind([holdable, component], profiles, [], row.Source);
			if (!row.Reused)
			{
				Assert.AreEqual(row.RequiredLayerWeight!.Value, bound.LayerWeight, row.PlanningKey);
				Assert.IsTrue(bound.ProfileIds.Select(id => profiles.Single(x => x.Id == id).Name).Contains(row.WearProfile), row.PlanningKey);
			}
			foreach (var profile in profiles.Where(x => bound.ProfileIds.Contains(x.Id)))
				Assert.IsTrue(geometry.Bind(profile, row.Source).Locations.Count > 0, row.PlanningKey);
		}
	}

	[DataTestMethod]
	[DataRow(false, true, false)]
	[DataRow(false, false, false)]
	[DataRow(true, true, false)]
	[DataRow(true, false, false)]
	[DataRow(false, true, true)]
	[DataRow(false, false, true)]
	[DataRow(true, true, true)]
	[DataRow(true, false, true)]
	public void All134PlannedOutfits_ResolveRuntimeFootprintsAndLayering(bool explicitBones, bool inventoryHands,
		bool countsAsAliases)
	{
		using var fixture = new Fixture(explicitBones, inventoryHands);
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var world = new Mock<IFuturemud>();
		var designedBody = new Mock<IBodyPrototype>();
		designedBody.Setup(x => x.Id).Returns(1);
		var shapes = fixture.Context.BodypartShapes.AsEnumerable().Select(row =>
		{
			var shape = new Mock<MudSharp.Form.Shape.IBodypartShape>();
			shape.Setup(x => x.Id).Returns(row.Id);
			shape.Setup(x => x.Name).Returns(row.Name);
			return shape.Object;
		}).ToArray();
		var parts = fixture.Context.BodypartProtos.AsEnumerable()
			.Where(x => IndustrialisedClothingWearProfiles.IsExternalLocation((BodypartTypeEnum)x.BodypartType))
			.Select(row =>
			{
				var part = new Mock<IExternalBodypart>();
				part.Setup(x => x.Id).Returns(row.Id);
				part.Setup(x => x.Name).Returns(row.Name);
				part.Setup(x => x.Shape).Returns(shapes.Single(x => x.Id == row.BodypartShapeId));
				part.Setup(x => x.CountsAs(It.IsAny<IBodypart>())).Returns((IBodypart other) => other.Id == row.Id);
				if (IndustrialisedClothingWearProfiles.IsWearLocation((BodypartTypeEnum)row.BodypartType)) part.As<IWear>();
				return part.Object;
			}).ToArray();
		var wearerParts = countsAsAliases
			? fixture.Context.BodypartProtos.AsEnumerable()
				.Where(x => IndustrialisedClothingWearProfiles.IsExternalLocation((BodypartTypeEnum)x.BodypartType))
				.Select(row =>
				{
					var part = new Mock<IExternalBodypart>();
					part.Setup(x => x.Id).Returns(row.Id + 100000);
					part.Setup(x => x.Name).Returns($"derived {row.Name}");
					part.Setup(x => x.Shape).Returns(shapes.Single(x => x.Id == row.BodypartShapeId));
					part.Setup(x => x.CountsAs(It.IsAny<IBodypart>())).Returns((IBodypart other) => other.Id == row.Id);
					if (IndustrialisedClothingWearProfiles.IsWearLocation((BodypartTypeEnum)row.BodypartType)) part.As<IWear>();
					return part.Object;
				}).ToArray()
			: parts;
		designedBody.Setup(x => x.AllExternalBodyparts).Returns(parts);
		designedBody.Setup(x => x.AllBodyparts).Returns(parts);
		world.Setup(x => x.BodyPrototypes).Returns(Collection(designedBody.Object));
		world.Setup(x => x.BodypartShapes).Returns(Collection(shapes));
		var profiles = fixture.Context.WearProfiles.ToArray();
		var runtimeProfiles = profiles.ToDictionary(x => x.Id, x => MudSharp.GameItems.Inventory.WearProfile.LoadWearProfile(x, world.Object));
		var components = fixture.Context.GameItemComponentProtos.ToArray();
		var holdable = new GameItemComponentProto { Id = -1, Name = "Holdable", Type = "Holdable", Definition = "<Definition/>" };
		var bindings = IndustrialisedClothingDependencyPlan.Rows.ToDictionary(x => x.ItemReference,
			x => IndustrialisedClothingPhysicalBindings.Bind([holdable, components.Single(c => c.Type == "Wearable" && x.Components.Contains(c.Name))], profiles, [], x.Source));
		var repository = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var inventory = File.ReadLines(Path.Combine(repository, IndustrialisedClothingDependencyAudit.InventoryPath))
			.Where(x => Regex.IsMatch(x, @"^\| [a-z][a-z0-9_]* \|"))
			.Select(x => x.Trim('|').Split('|', StringSplitOptions.TrimEntries))
			.ToDictionary(x => x[0], x => x[4] == "new" ? IndustrialisedClothingDependencyPlan.Rows.Single(row => !row.Reused && row.PlanningKey == x[0]).ItemReference : x[4]);
		var outfits = File.ReadLines(Path.Combine(repository, IndustrialisedClothingDependencyAudit.OutfitsPath))
			.Where(x => Regex.IsMatch(x, @"^\| [a-z][a-z0-9_]* \|"))
			.Select(x => x.Trim('|').Split('|', StringSplitOptions.TrimEntries)).ToArray();
		Assert.AreEqual(134, outfits.Length);
		var maximum = IndustrialisedClothingWearProfiles.MaximumLayerWeight([], new("outfits", 1));
		var issues = new List<string>();
		foreach (var outfit in outfits)
		{
			var worn = new List<(IWear Part, string Key, ClothingWearableBinding Binding, IWearlocProfile Coverage)>();
			var wearer = new Mock<IBody>();
			wearer.Setup(x => x.WearLocs).Returns(wearerParts.OfType<IWear>().ToArray());
			wearer.Setup(x => x.WornItemCounts).Returns(() => wearerParts.OfType<IWear>()
				.Select(part => (Part: part, Count: worn.Count(x => x.Part == part))).ToLookup(x => x.Part, x => x.Count));
			foreach (var entry in outfit[2].Split(';', StringSplitOptions.TrimEntries))
			{
				var key = entry.Split('@')[0];
				var binding = bindings[inventory[key]];
				var footprint = runtimeProfiles[binding.DefaultProfileId].Profile(wearer.Object);
				Assert.IsNotNull(footprint, $"{outfit[0]}/{key}");
				foreach (var (part, coverage) in footprint)
				{
					var prior = worn.Where(x => x.Part == part).ToArray();
					if (prior.Sum(x => x.Binding.LayerWeight) + binding.LayerWeight > maximum)
						issues.Add($"{outfit[0]}/{key}: {part.Name} exceeds {maximum} layers ({string.Join(',', prior.Select(x => x.Key))})");
					if (binding.Bulky && coverage.Mandatory && prior.Any(x => x.Binding.Bulky && x.Coverage.Mandatory))
						issues.Add($"{outfit[0]}/{key}: bulky conflict at {part.Name} ({string.Join(',', prior.Where(x => x.Binding.Bulky).Select(x => x.Key))})");
					worn.Add((part, key, binding, coverage));
				}
			}
		}
		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues.Distinct()));
	}

	[TestMethod]
	public void CorrectedHistoricalLayers_UseExactDedicatedStockWithoutChangingSharedProfiles()
	{
		using var fixture = new Fixture();
		var original = fixture.Context.GameItemComponentProtos.AsEnumerable().ToDictionary(x => x.Name, x => x.Definition);
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var components = fixture.Context.GameItemComponentProtos.ToArray();
		var profiles = fixture.Context.WearProfiles.ToArray();
		var holdable = new GameItemComponentProto { Id = -1, Name = "Holdable", Type = "Holdable", Definition = "<Definition/>" };
		(string Reference, string Component, double Weight)[] corrections =
		[
			("medieval_tablet_woven_garters", "Wear_Garters_Layer_0_1", 0.1),
			("medieval_latin_linen_cincture", "Wear_Waist_Layer_0_1", 0.1),
			("medieval_eastern_sticharion", "Wear_Robe_Layer_0_5_NonBulky", 0.5),
			("medieval_daoist_cross_collar_robe", "Wear_Robe_Layer_0_5_NonBulky", 0.5),
			("renaissance_shared_clothing_straight_underrobe", "Wear_Long-Sleeved_Gown_Layer_0_25_NonBulky", 0.25),
			("renaissance_japanese_smallsleeve_wraprobe", "Wear_Robe_Layer_0_75_NonBulky", 0.75),
			("renaissance_institution_plain_cassock", "Wear_Robe_Layer_0_75_NonBulky", 0.75),
			("renaissance_institution_liturgical_alb", "Wear_Robe_Layer_0_5_NonBulky", 0.5),
			("earlymodern_qing_clothing_long_sidefastened_robe", "Wear_Robe_Layer_0_75_NonBulky", 0.75)
		];
		foreach (var correction in corrections)
		{
			var source = ItemSeeder.FindHistoricalClothingSource(correction.Reference)!;
			var component = components.Single(x => x.Type == "Wearable" && source.Components.Contains(x.Name));
			Assert.AreEqual(correction.Component, component.Name);
			var bound = IndustrialisedClothingPhysicalBindings.Bind([holdable, component], profiles, [], new("historical layering", 1));
			Assert.AreEqual(correction.Weight, bound.LayerWeight);
			Assert.IsFalse(bound.Bulky, correction.Reference);
		}
		foreach (var (name, definition) in original)
			Assert.AreEqual(definition, components.Single(x => x.Name == name).Definition, name);
		var garters = XElement.Parse(profiles.Single(x => x.Name == "Garters").WearlocProfiles).Elements().ToArray();
		CollectionAssert.AreEquivalent(new[] { "rknee", "lknee", "rkneeback", "lkneeback" }, garters.Select(x => (string)x.Attribute("Bodypart")!).ToArray());
		Assert.IsTrue(garters.All(x => (bool)x.Attribute("Transparent")! && !(bool)x.Attribute("HidesSevered")!));
		Assert.AreEqual(109, IndustrialisedClothingDependencyPlan.WearLayerStock.Count);
	}

	[TestMethod]
	public void FreshWearStock_AndBaselineUpgradeHaveTheSameCompleteDefinitions()
	{
		using var fresh = new Fixture(seedBaseline: false);
		HumanSeeder.SeedHumanWearProfilesForTesting(fresh.Context);
		using var upgrade = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(upgrade.Context);
		CollectionAssert.AreEqual(upgrade.Signatures(), fresh.Signatures());
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fresh.Context));
		var before = fresh.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fresh.Context));
		CollectionAssert.AreEqual(before, fresh.Signatures());
	}

	[TestMethod]
	public void ThicknessVariants_PreserveSourceSemanticsAndExistingConfigurations()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var components = fixture.Context.GameItemComponentProtos.ToArray();
		foreach (var stock in IndustrialisedClothingDependencyPlan.WearLayerStock)
		{
			var source = components.Single(x => x.Name == stock.SourceComponent);
			var variant = components.Single(x => x.Name == stock.Name);
			Assert.AreNotEqual(source.Id, variant.Id);
			var sourceXml = XElement.Parse(source.Definition);
			var variantXml = XElement.Parse(variant.Definition);
			Assert.AreEqual(stock.LayerWeight, (double)variantXml.Element("LayerWeightConsumption")!, stock.Name);
			if (stock.Bulky is { } bulky)
			{
				Assert.AreEqual(bulky, (bool)variantXml.Attribute("Bulky")!, stock.Name);
				sourceXml.SetAttributeValue("Bulky", bulky);
			}
			sourceXml.Elements("LayerWeightConsumption").Remove();
			variantXml.Elements("LayerWeightConsumption").Remove();
			Assert.IsTrue(XNode.DeepEquals(sourceXml, variantXml), stock.Name);
		}
		var target = components.Single(x => x.Name == "Wear_Camisole_Layer_0_25");
		target.Description = "Builder's custom layering";
		target.Definition = target.Definition.Replace("0.25", "0.3", StringComparison.Ordinal);
		fixture.Context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Id = target.Id, RevisionNumber = 1, Name = target.Name, Type = target.Type,
			Description = "Builder's later revision", Definition = target.Definition
		});
		fixture.Context.SaveChanges();
		var before = fixture.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		CollectionAssert.AreEqual(before, fixture.Signatures());
		fixture.Context.GameItemComponentProtos.Remove(components.Single(x => x.Name == "Wear_Split_Drawers_Layer_0_25"));
		fixture.Context.SaveChanges();
		Assert.IsTrue(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.IsTrue(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.AreEqual("Builder's custom layering", target.Description);
	}

	[TestMethod]
	public void MissingThicknessBatch_BadSourceFailsWithoutPartialVariantCreation()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var names = IndustrialisedClothingDependencyPlan.WearLayerStock.Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
		fixture.Context.GameItemComponentProtos.RemoveRange(fixture.Context.GameItemComponentProtos.AsEnumerable().Where(x => names.Contains(x.Name)));
		fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Vest").Type = "Holdable";
		fixture.Context.SaveChanges();
		var before = fixture.Signatures();
		StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() => HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context)).Message, "Wear_Vest");
		CollectionAssert.AreEqual(before, fixture.Signatures());
		Assert.AreEqual(0, fixture.Context.ChangeTracker.Entries().Count(x => x.State is EntityState.Added or EntityState.Modified));
	}

	private static string ComponentName(string profile) => $"Wear_{profile.Replace(' ', '_')}";
	private static IUneditableAll<T> Collection<T>(params T[] items) where T : class, IFrameworkItem
	{
		var collection = new Mock<IUneditableAll<T>>();
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => items.SingleOrDefault(x => x.Id == id)!);
		return collection.Object;
	}

	private sealed class Fixture : IDisposable
	{
		internal FuturemudDatabaseContext Context { get; } = new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		internal Fixture(bool explicitBones = false, bool inventoryHands = true, bool seedBaseline = true)
		{
			Context.Accounts.Add(new Account { Id = 1, Name = "wear-profile-tests", CultureName = "en-AU", TimeZoneId = "UTC", UnitPreference = "metric" });
			Context.BodyProtos.Add(new BodyProto
			{
				Id = 1, Name = "Humanoid", ConsiderString = "a humanoid", LegDescriptionPlural = "legs", LegDescriptionSingular = "leg",
				WielderDescriptionPlural = "hands", WielderDescriptionSingle = "hand"
			});
			// Read the actual source anatomy/shape/type vocabulary; this fixture proves geometry,
			// not race/gender sizing or a complete HumanSeeder fresh-world installation.
			var source = SeederSourceTestHelper.ReadPartialFamily("HumanSeeder");
			var matches = Regex.Matches(source, "CreateBodypart\\(baseHumanoid,\\s*\"(?<name>[^\"]+)\",\\s*\"[^\"]+\",\\s*\"(?<shape>[^\"]+)\",\\s*(?<type>[^,]+),")
				.Cast<Match>().GroupBy(x => x.Groups["name"].Value).Select(x => x.First()).ToArray();
			Assert.IsTrue(matches.Length >= 80);
			var shapes = matches.Select(x => x.Groups["shape"].Value).Distinct().Select((name, index) => new BodypartShape { Id = index + 1, Name = name }).ToArray();
			Context.BodypartShapes.AddRange(shapes);
			Context.BodypartProtos.AddRange(matches.Select((match, index) => new BodypartProto
			{
				Id = index + 1, BodyId = 1, Name = match.Groups["name"].Value, Description = match.Groups["name"].Value,
				BodypartShapeId = shapes.Single(x => x.Name == match.Groups["shape"].Value).Id,
				BodypartType = (int)(Regex.Replace(match.Groups["type"].Value, @"\s+", "") switch
				{
					"drapeableType" => explicitBones ? BodypartTypeEnum.Wear : BodypartTypeEnum.BonyDrapeable,
					"niDrapeableType" => explicitBones ? BodypartTypeEnum.Wear : BodypartTypeEnum.NonImmobilisingBonyDrapeable,
					"gwType" => explicitBones ? BodypartTypeEnum.GrabbingWielding : BodypartTypeEnum.BonyGrabbingWielding,
					"_questionAnswers[\"inventory\"].ToLowerInvariant().Equals(\"hands\")?gwType:BodypartTypeEnum.Wielding" =>
						!inventoryHands ? BodypartTypeEnum.Wielding : explicitBones ? BodypartTypeEnum.GrabbingWielding : BodypartTypeEnum.BonyGrabbingWielding,
					var type when type.StartsWith("BodypartTypeEnum.", StringComparison.Ordinal) => Enum.Parse<BodypartTypeEnum>(type.Split('.')[1]),
					var type => throw new InvalidDataException($"Unrecognised stock bodypart type expression: {type}")
				})
			}));
			Context.SaveChanges();
			// Use the actual fresh-install baseline writer, not empty placeholder profiles. Additional
			// stock is installed by the same refresh path exercised by each test below.
			if (seedBaseline) HumanSeeder.SeedHumanWearProfilesForTesting(Context, includeAdditional: false);
		}
		internal string[] Signatures() => Context.WearProfiles.OrderBy(x => x.Id).AsEnumerable()
			.Select(x => $"{x.Id}|{x.Name}|{x.Type}|{x.Description}|{x.WearlocProfiles}")
			.Concat(Context.GameItemComponentProtos.OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber).AsEnumerable()
				.Select(x => $"{x.Id}:{x.RevisionNumber}|{x.Name}|{x.Type}|{x.Description}|{x.Definition}")).ToArray();
		public void Dispose() => Context.Dispose();
	}
}
