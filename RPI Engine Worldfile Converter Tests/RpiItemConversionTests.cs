#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPI_Engine_Worldfile_Converter;

namespace RPI_Engine_Worldfile_Converter_Tests;

[TestClass]
public class RpiItemConversionTests
{
	[TestMethod]
	public void Parser_ReadsFixtureCorpus_AndBuildsTypedPayloads()
	{
		var parser = new RpiWorldfileParser();
		var corpus = parser.ParseDirectory(GetFixtureDirectory());

		Assert.AreEqual(11, corpus.Items.Count);
		Assert.AreEqual(0, corpus.Failures.Count);

		var worn = corpus.Items.Single(x => x.Vnum == 1000);
		Assert.AreEqual("masked-face", worn.DescKeys);
		Assert.AreEqual(RPIItemType.Worn, worn.ItemType);

		var armour = corpus.Items.Single(x => x.Vnum == 1001);
		Assert.IsNotNull(armour.ArmourData);
		Assert.AreEqual("Mail", armour.ArmourData!.ArmourFamilyName);

		var weapon = corpus.Items.Single(x => x.Vnum == 1002);
		Assert.IsNotNull(weapon.WeaponData);
		Assert.AreEqual(RPISkill.LightEdge, weapon.WeaponData!.UseSkill);
		Assert.AreEqual(1, weapon.Affects.Count);
		Assert.AreEqual(RPISkill.LightEdge, weapon.Affects[0].Skill);

		var lantern = corpus.Items.Single(x => x.Vnum == 1003);
		Assert.IsNotNull(lantern.LightData);
		Assert.AreEqual(20, lantern.LightData!.Capacity);

		var drinkContainer = corpus.Items.Single(x => x.Vnum == 1004);
		Assert.IsNotNull(drinkContainer.DrinkContainerData);
		Assert.AreEqual(40, drinkContainer.DrinkContainerData!.Capacity);

		var food = corpus.Items.Single(x => x.Vnum == 1005);
		Assert.IsNotNull(food.FoodData);
		Assert.AreEqual(6, food.FoodData!.Bites);
		Assert.AreEqual(1, food.Poisons.Count);

		var key = corpus.Items.Single(x => x.Vnum == 1006);
		Assert.IsNotNull(key.KeyData);
		Assert.AreEqual(1234, key.KeyData!.KeyedToVnum);

		var quiver = corpus.Items.Single(x => x.Vnum == 1007);
		Assert.IsNotNull(quiver.ContainerData);
		Assert.AreEqual(30, quiver.ContainerData!.Capacity);

		var repair = corpus.Items.Single(x => x.Vnum == 1008);
		Assert.IsNotNull(repair.RepairKitData);
		Assert.AreEqual((int)RPIItemType.Armor, repair.RepairKitData!.RepairItemTypeValue);

		var book = corpus.Items.Single(x => x.Vnum == 1009);
		Assert.IsNotNull(book.WritingData);
		Assert.AreEqual(1, book.ExtraDescriptions.Count);

		var dwelling = corpus.Items.Single(x => x.Vnum == 1010);
		Assert.AreEqual(1, dwelling.Clans.Count);
		Assert.AreEqual(RPIItemType.Dwelling, dwelling.ItemType);
	}

	[TestMethod]
	public void Transformer_MapsFixtureCorpus_ToExpectedStatusesAndComponents()
	{
		var parser = new RpiWorldfileParser();
		var corpus = parser.ParseDirectory(GetFixtureDirectory());
		var transformer = new FutureMUDItemTransformer(BuildCatalog());
		var converted = transformer.Convert(corpus.Items).ToDictionary(x => x.Vnum);

		CollectionAssert.Contains(converted[1000].ComponentNames.ToList(), "Wear_Skullcap");
		CollectionAssert.Contains(converted[1000].TagNames.ToList(), "Hats");
		Assert.AreEqual(ConversionStatus.FunctionalImport, converted[1000].Status);

		CollectionAssert.Contains(converted[1001].ComponentNames.ToList(), "Armour_Chainmail");
		CollectionAssert.Contains(converted[1001].ComponentNames.ToList(), "Wear_Hauberk");

		CollectionAssert.Contains(converted[1002].ComponentNames.ToList(), "Melee_Longsword");
		Assert.AreEqual(1, converted[1002].TraitReferences.Count);
		Assert.AreEqual("Light-Edge", converted[1002].TraitReferences[0].TraitName);

		CollectionAssert.Contains(converted[1003].ComponentNames.ToList(), "Lantern");
		Assert.AreEqual("water", converted[1003].LiquidReference!.LiquidName);

		CollectionAssert.Contains(converted[1004].ComponentNames.ToList(), "LContainer_40ozBottle");
		CollectionAssert.Contains(converted[1005].ComponentNames.ToList(), "Food_Generic");
		CollectionAssert.Contains(converted[1006].ComponentNames.ToList(), "Warded_Key");
		CollectionAssert.Contains(converted[1007].ComponentNames.ToList(), "Container_Quiver");
		CollectionAssert.Contains(converted[1008].ComponentNames.ToList(), "Repair_Metal_Armour");
		CollectionAssert.Contains(converted[1009].ComponentNames.ToList(), "Book_Small_40_Page");

		Assert.AreEqual(ConversionStatus.DeferredBehaviorPropImport, converted[1010].Status);
		CollectionAssert.Contains(converted[1010].ComponentNames.ToList(), "Destroyable_Door");
	}

	private static string GetFixtureDirectory()
	{
		var candidates = new[]
		{
			Path.Combine(AppContext.BaseDirectory, "Fixtures"),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Fixtures")),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter Tests", "Fixtures"))
		};

		return candidates.First(x => File.Exists(Path.Combine(x, "objs.1")));
	}

	private static FutureMudBaselineCatalog BuildCatalog()
	{
		var components = new Dictionary<string, FutureMudComponentReference>(StringComparer.OrdinalIgnoreCase)
		{
			["Holdable"] = new(1, 0, "Holdable", "Holdable"),
			["Wear_Skullcap"] = new(2, 0, "Wear_Skullcap", "Wearable"),
			["Armour_LightClothing"] = new(3, 0, "Armour_LightClothing", "Armour"),
			["Destroyable_Clothing"] = new(4, 0, "Destroyable_Clothing", "Destroyable"),
			["Wear_Hauberk"] = new(5, 0, "Wear_Hauberk", "Wearable"),
			["Armour_Chainmail"] = new(6, 0, "Armour_Chainmail", "Armour"),
			["Destroyable_Armour"] = new(7, 0, "Destroyable_Armour", "Destroyable"),
			["Melee_Longsword"] = new(8, 0, "Melee_Longsword", "Weapon"),
			["Destroyable_Weapon"] = new(9, 0, "Destroyable_Weapon", "Destroyable"),
			["Lantern"] = new(10, 0, "Lantern", "Light"),
			["LContainer_40ozBottle"] = new(11, 0, "LContainer_40ozBottle", "LiquidContainer"),
			["Food_Generic"] = new(12, 0, "Food_Generic", "Food"),
			["Destroyable_Misc"] = new(13, 0, "Destroyable_Misc", "Destroyable"),
			["Warded_Key"] = new(14, 0, "Warded_Key", "Key"),
			["Container_Quiver"] = new(15, 0, "Container_Quiver", "Container"),
			["Repair_Metal_Armour"] = new(16, 0, "Repair_Metal_Armour", "RepairKit"),
			["Book_Small_40_Page"] = new(17, 0, "Book_Small_40_Page", "Book"),
			["Destroyable_Paper"] = new(18, 0, "Destroyable_Paper", "Destroyable"),
			["Destroyable_Door"] = new(19, 0, "Destroyable_Door", "Destroyable")
		};

		return new FutureMudBaselineCatalog
		{
			Components = components,
			ComponentsByType = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
			{
				["Food"] = ["Food_Generic"]
			},
			MaterialIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["linen"] = 1,
				["steel"] = 2,
				["brass"] = 3,
				["leather"] = 4,
				["other"] = 5,
				["bronze"] = 6,
				["Paper"] = 7,
				["textile"] = 8
			},
			TagIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["Hats"] = 1,
				["Mail Armour"] = 2,
				["Lamps"] = 3,
				["Watertight Container"] = 4,
				["Food"] = 5,
				["Luxury Wares"] = 6,
				["Open Container"] = 7,
				["Military Goods"] = 8,
				["Repairing"] = 9,
				["Standard Tools"] = 10
			},
			LiquidIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["water"] = 1
			},
			TraitIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["Light-Edge"] = 1
			}
		};
	}
}
