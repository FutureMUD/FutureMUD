#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonCatalogueProvisionsTests
{
	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void Provisions_FixedDirection_ExactNutritionUnitsAndDurableRerun(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueProvisions.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		foreach (var key in new[] { "arm.item.food.bread", "arm.item.food.fruit", "arm.item.food.stew" })
		{
			var item = db.GameItemProtos.Single(x => x.Id == first.Identities[key]);
			Assert.AreEqual(key.EndsWith("bread") ? "flatbread" : key.EndsWith("fruit") ? "fruit" : "stew", item.Name);
			Assert.AreEqual("Armageddon " + new ArmageddonStockCatalogue().ByKey[key].Name, item.UniqueName);
			Assert.AreEqual(250.0, item.Weight);
			var id = db.SeederManagedRecords.Single(x => x.StableKey == key + ".component").LogicalId;
			var definition = XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == id).Definition);
			Assert.AreEqual(5.0, (double)definition.Attribute("Satiation")!);
			Assert.AreEqual(5.0, (double)definition.Attribute("Bites")!);
			Assert.AreEqual(0.1, (double)definition.Attribute("Water")!);
			Assert.IsFalse(definition.Attributes().Any(x => x.Name.LocalName.Contains("calor", StringComparison.OrdinalIgnoreCase)));
		}
		var bottleId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.drink_container.component").LogicalId;
		Assert.AreEqual("bottle", db.GameItemProtos.Single(x => x.Id == first.Identities["arm.item.drink_container"]).Name);
		var bottle = XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == bottleId).Definition);
		Assert.AreEqual(10.0, (double)bottle.Attribute("LiquidCapacity")!);
		Assert.AreEqual(0L, (long)bottle.Attribute("DefaultLiquid")!);
		Assert.AreEqual(true, (bool)bottle.Attribute("Closable")!);
		var wine = db.Liquids.Single(x => x.Id == first.Identities["arm.liquid.wine"]);
		Assert.AreEqual(990.0, wine.Density);
		Assert.AreEqual(0.88, wine.WaterLitresPerLitre);
		Assert.AreEqual(0.12, wine.AlcoholLitresPerLitre);
		Assert.AreEqual(0, db.MagicSpells.Count());
		Assert.AreEqual(0, db.GameItems.Count());
		Assert.AreEqual(0, db.Characters.Count());
		var records = db.SeederManagedRecords.ToArray().ToDictionary(x => x.StableKey, x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueProvisions.Install(factory).Status);
		db.ChangeTracker.Clear();
		foreach (var record in db.SeederManagedRecords)
			Assert.AreEqual(records[record.StableKey], (record.LogicalId, record.RevisionNumber, record.SeedBaseline, record.AppliedAt), record.StableKey);
		ArmageddonInstallation.Prepare(factory, mode);
	}

	[TestMethod]
	public void Provisions_NativeFoodAndBottleFactories_PartialOrExhaustedReloadDoesNotRefill()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueProvisions.Install(ArmageddonInstallationSettingsTests.Factory(db)).Status);
		db.ChangeTracker.Clear();
		var world = World(db);
		var parent = new Mock<IGameItem>(); parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		var manager = new GameItemComponentManager();
		foreach (var key in new[] { "arm.item.food.bread", "arm.item.food.fruit", "arm.item.food.stew" })
		{
			var id = db.SeederManagedRecords.Single(x => x.StableKey == key + ".component").LogicalId;
			var proto = manager.GetProto(db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == id), world.Object) ?? throw new InvalidOperationException("Native food factory missing.");
			Assert.IsInstanceOfType(proto, typeof(FoodGameItemComponentProto));
			Assert.IsTrue(proto.CanSubmit());
			foreach (var remaining in new[] { 2, 0 })
			{
				var edible = (IEdible)proto.LoadComponent(new Db.GameItemComponent { Id = 1, Definition = $"<Definition><Bites>{remaining}</Bites></Definition>" }, parent.Object);
				Assert.AreEqual((double)remaining, edible.BitesRemaining);
				Assert.AreEqual(5.0, edible.TotalBites);
				Assert.AreEqual(5.0, edible.SatiationPoints);
				Assert.AreEqual(0.1, edible.WaterLitres);
			}
		}
		var bottleId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.drink_container.component").LogicalId;
		var bottleProto = manager.GetProto(db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == bottleId), world.Object) ?? throw new InvalidOperationException("Native bottle factory missing.");
		Assert.IsInstanceOfType(bottleProto, typeof(LiquidContainerGameItemComponentProto));
		var bottle = (ILiquidContainer)bottleProto.LoadComponent(new Db.GameItemComponent { Id = 2, Definition = "<Definition Open=\"false\"><Locks/><Mix/></Definition>" }, parent.Object);
		Assert.IsTrue(bottle.LiquidMixture is null || bottle.LiquidMixture.IsEmpty);
		Assert.AreEqual(10.0, bottle.LiquidCapacity);
		parent.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void Provisions_BuilderNutritionAndLiquidEdits_RenamedFoodBindingPreserved()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueProvisions.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var id = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.food.bread.component").LogicalId;
		var food = db.GameItemComponentProtos.Single(x => x.Id == id);
		var xml = XElement.Parse(food.Definition); xml.SetAttributeValue("Satiation", 7); food.Definition = xml.ToString();
		db.Liquids.Single(x => x.Id == first.Identities["arm.liquid.wine"]).TasteText = "The builder's spiced wine.";
		db.Materials.Single(x => x.Name == "food").Name = "Builder prepared-food material";
		db.SaveChanges();
		var rerun = ArmageddonCatalogueProvisions.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(7.0, (double)XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == id).Definition).Attribute("Satiation")!);
		Assert.AreEqual("The builder's spiced wine.", db.Liquids.Single(x => x.Id == first.Identities["arm.liquid.wine"]).TasteText);
		Assert.IsTrue(rerun.Messages.Any(x => x.Contains("preserved builder edit")));
	}

	[TestMethod]
	public void Catalogue_RealSpellDependenciesDeferTwelveFociDevicesAndTheirRecipes()
	{
		var deferred = new ArmageddonStockCatalogue().Blueprints.Where(x => x.DeferredToS4).ToArray();
		Assert.AreEqual(12, deferred.Length);
		Assert.AreEqual(10, deferred.Count(x => x.Kind == ArmageddonBlueprintKind.Focus));
		Assert.AreEqual(2, deferred.Count(x => x.Kind == ArmageddonBlueprintKind.DeviceBlank));
		Assert.IsTrue(deferred.All(x => x.Recipe is not null));
		Assert.AreEqual(5.0, ArmageddonCatalogueProvisions.SatiationHours(500));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonCatalogueProvisions.SatiationHours(double.NaN));
	}

	internal static Mock<IFuturemud> World(FuturemudDatabaseContext db)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.Liquids).Returns(new All<ILiquid>());
		var decorators = new All<IStackDecorator>(); world.SetupGet(x => x.StackDecorators).Returns(decorators);
		foreach (var row in db.StackDecorators) decorators.Add(StackDecorator.LoadStackDecorator(row));
		return world;
	}

	internal static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode, bool classes)
	{
		var db = ArmageddonStockCatalogueTests.Fixture(mode, classes);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(ArmageddonInstallationSettingsTests.Factory(db)).Status);
		db.Materials.Add(new Db.Material { Id = 4, Name = "food", BehaviourType = (int)MaterialBehaviourType.Food, Density = 1000 });
		db.UnitsOfMeasure.Single(x => x.Name == "litre").BaseMultiplier = 10;
		db.SaveChanges();
		return db;
	}
}
