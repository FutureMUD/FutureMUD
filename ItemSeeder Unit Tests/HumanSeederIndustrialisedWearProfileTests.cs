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

using Fixture = MudSharp_Unit_Tests.HumanWearProfileFixture;
using static MudSharp_Unit_Tests.HumanWearProfileFixture;

namespace MudSharp_Unit_Tests;

[TestClass]
[TestCategory("Integration")]
public class HumanSeederIndustrialisedWearProfileTests
{

	[TestMethod]
	[DataRow(false, true)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(true, false)]
	public void PersistedProfiles_ResolveAgainstStockHumanNamesShapesAndWearablePartTypes(bool explicitBones, bool inventoryHands)
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

	[TestMethod]
	[DataRow(false, true)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(true, false)]
	public void PlannedBases_ResolveStockGeometryAndNewBasesPreserveAuthoredThickness(bool explicitBones, bool inventoryHands)
	{
		using var fixture = new Fixture(explicitBones, inventoryHands);
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var geometry = IndustrialisedClothingWearProfiles.Read(fixture.Context);
		var profiles = fixture.Context.WearProfiles.ToArray();
		var components = fixture.Context.GameItemComponentProtos.ToArray();
		var holdable = new GameItemComponentProto { Id = -1, Name = "Holdable", Type = "Holdable", Definition = "<Definition/>" };
		var rows = IndustrialisedClothingDependencyPlan.Rows;
		Assert.IsTrue(rows.Count > 0);
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

	[TestMethod]
	[DataRow(false, true, false)]
	[DataRow(false, false, false)]
	[DataRow(true, true, false)]
	[DataRow(true, false, false)]
	[DataRow(false, true, true)]
	[DataRow(false, false, true)]
	[DataRow(true, true, true)]
	[DataRow(true, false, true)]
	public void PlannedOutfits_ResolveRuntimeFootprintsAndLayering(bool explicitBones, bool inventoryHands,
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
		Assert.IsTrue(outfits.Length > 0);
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

	private static IUneditableAll<T> Collection<T>(params T[] items) where T : class, IFrameworkItem
	{
		var collection = new Mock<IUneditableAll<T>>();
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => items.SingleOrDefault(x => x.Id == id)!);
		return collection.Object;
	}
}
