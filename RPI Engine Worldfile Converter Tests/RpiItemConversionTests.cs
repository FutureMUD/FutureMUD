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

	[TestMethod]
	public void Transformer_ExtractsBoundedAnsiColour_ToCustomColour()
	{
		var transformer = new FutureMUDItemTransformer(BuildCatalog());
		var item = BuildItem(
			shortDescription: "#5a violet sash#0",
			longDescription: "#5A violet sash has been left here.#0",
			fullDescription: "This still has #5inline#0 markup.");

		var converted = transformer.Convert(item);

		Assert.AreEqual("magenta", converted.CustomColour);
		Assert.AreEqual("a violet sash", converted.ShortDescription);
		Assert.AreEqual("A violet sash has been left here.", converted.LongDescription);
		Assert.AreEqual("This still has #5inline#0 markup.", converted.FullDescription);
	}

	[TestMethod]
	public void Transformer_PreservesLegacyVariableFamilies_AndAddsMatchingComponents()
	{
		var transformer = new FutureMUDItemTransformer(BuildCatalog());

		var fine = transformer.Convert(BuildItem(
			shortDescription: "a $finecolor cloak",
			longDescription: "A $finecolor cloak lies here.",
			fullDescription: "It is $finecolor."));
		CollectionAssert.Contains(fine.ComponentNames.ToList(), "Variable_FineColour");
		Assert.IsTrue(fine.ShortDescription.Contains("$finecolor", StringComparison.Ordinal));

		var drab = transformer.Convert(BuildItem(
			shortDescription: "a $drabcolor cloak",
			longDescription: "A $drabcolor cloak lies here."));
		CollectionAssert.Contains(drab.ComponentNames.ToList(), "Variable_DrabColour");

		var basic = transformer.Convert(BuildItem(
			shortDescription: "a $color cloak",
			longDescription: "A $color cloak lies here."));
		CollectionAssert.Contains(basic.ComponentNames.ToList(), "Variable_BasicColour");

		var gem = transformer.Convert(BuildItem(
			rawName: "ring $gem METAL jewelry $gemcolor~",
			shortDescription: "a $gemcolor-set ring",
			longDescription: "A $gemcolor-set ring lies here."));
		CollectionAssert.Contains(gem.ComponentNames.ToList(), "Variable_Gem");
		Assert.IsTrue(gem.ShortDescription.Contains("$gemcolor", StringComparison.Ordinal));

		var stone = transformer.Convert(BuildItem(
			shortDescription: "a chunk of $stone",
			longDescription: "A chunk of $stone lies here."));
		CollectionAssert.Contains(stone.ComponentNames.ToList(), "Variable_CommonStone");

		var mixed = transformer.Convert(BuildItem(
			shortDescription: "a $finecolor shirt studded with a $finegemcolor",
			longDescription: "A $finecolor shirt studded with a $finegemcolor lies here."));
		CollectionAssert.Contains(mixed.ComponentNames.ToList(), "Variable_RpiMixedVariables");
		Assert.IsFalse(mixed.Warnings.Any(x => x.Code == "mixed-variable-profiles"));
	}

	[TestMethod]
	public void Transformer_DoesNotAddHoldable_WhenTakeBitAbsent()
	{
		var transformer = new FutureMUDItemTransformer(BuildCatalog());
		var item = BuildItem(wearBits: 0);

		var converted = transformer.Convert(item);

		CollectionAssert.DoesNotContain(converted.ComponentNames.ToList(), "Holdable");
		CollectionAssert.Contains(converted.ComponentNames.ToList(), "Destroyable_Misc");
	}

	[TestMethod]
	public void Transformer_MapsAdditionalWearBits_ForArmour()
	{
		var transformer = new FutureMUDItemTransformer(BuildCatalog());
		var item = BuildItem(
			itemType: RPIItemType.Armor,
			wearBits: RPIWearBits.Take | RPIWearBits.Neck,
			rawName: "steel gorget METAL~",
			shortDescription: "a steel gorget",
			longDescription: "A steel gorget lies here.",
			armourData: new RpiArmourData(1, 5, "Plate"));

		var converted = transformer.Convert(item);

		CollectionAssert.Contains(converted.ComponentNames.ToList(), "Wear_Gorget");
		CollectionAssert.Contains(converted.ComponentNames.ToList(), "Armour_Platemail");
		Assert.IsFalse(converted.Warnings.Any(x => x.Code == "unmapped-wear-profile"));
	}

	[TestMethod]
	public void Transformer_MapsBoardItems_ToGeneratedBoardComponent()
	{
		var catalog = BuildCatalog();
		var transformer = new FutureMUDItemTransformer(catalog);
		var item = BuildItem(
			itemType: RPIItemType.Board,
			wearBits: 0,
			rawName: "market board OTHER~",
			shortDescription: "a market board",
			longDescription: "A market board is mounted here.",
			clans: [new RpiClanRecord("gondor", "Captain")]);

		var converted = transformer.Convert(item);
		var issues = FutureMudItemValidation.Validate(catalog, [converted]);

		Assert.IsNotNull(converted.BoardDefinition);
		Assert.AreEqual("market", converted.BoardDefinition!.LegacyBoardKey);
		Assert.AreEqual("RPI_Board_market", converted.BoardDefinition.ComponentName);
		Assert.AreEqual(1, converted.BoardDefinition.ClanRestrictions.Count);
		Assert.AreEqual("gondor", converted.BoardDefinition.ClanRestrictions[0].ClanAlias);
		Assert.AreEqual("Captain", converted.BoardDefinition.ClanRestrictions[0].RankName);
		CollectionAssert.Contains(converted.ComponentNames.ToList(), converted.BoardDefinition.ComponentName);
		Assert.IsFalse(converted.Warnings.Any(x => x.Code == "board-clan-access-not-mapped"));
		Assert.IsFalse(issues.Any(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void Validation_RequiresImportedClans_BeforeClanRestrictedBoardItems()
	{
		var catalog = BuildCatalog();
		catalog.ClansByAlias.Clear();
		var transformer = new FutureMUDItemTransformer(catalog);
		var item = BuildItem(
			itemType: RPIItemType.Board,
			wearBits: 0,
			rawName: "market board OTHER~",
			shortDescription: "a market board",
			longDescription: "A market board is mounted here.",
			clans: [new RpiClanRecord("gondor", "Captain")]);

		var converted = transformer.Convert(item);
		var issues = FutureMudItemValidation.Validate(catalog, [converted]);

		Assert.IsTrue(issues.Any(x =>
			x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase) &&
			x.Message.Contains("Run apply-clans before apply-items", StringComparison.OrdinalIgnoreCase)));
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

	private static RpiItemRecord BuildItem(
		int vnum = 9000,
		RPIItemType itemType = RPIItemType.Other,
		RPIWearBits wearBits = RPIWearBits.Take,
		RPIExtraBits extraBits = 0,
		string rawName = "item OTHER~",
		string shortDescription = "an item",
		string longDescription = "An item lies here.",
		string fullDescription = "It is an item.",
		RpiArmourData? armourData = null,
		IReadOnlyList<RpiClanRecord>? clans = null)
	{
		return new RpiItemRecord
		{
			Vnum = vnum,
			SourceFile = @"C:\rpi\objs.99",
			Zone = 99,
			RawName = rawName,
			NameKeywords = rawName
				.TrimEnd('~')
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
			ShortDescription = shortDescription,
			LongDescription = longDescription,
			FullDescription = fullDescription,
			ItemType = itemType,
			ExtraBits = extraBits,
			WearBits = wearBits,
			RawOvals = new RpiRawOvalValues(0, 0, 0, 0, 0, 0),
			RawStateValues = [],
			RawTailValues = [],
			Weight = 100,
			SilverValue = 0,
			RoomPosition = 0,
			Activation = 0,
			Quality = 0,
			RawEconFlags = 0,
			Size = 0,
			Count = 1,
			NumericTail = new RpiNumericTail(0, 0, 0, 0, 0, 0, 0),
			InferredMaterial = RPIMaterial.Other,
			ArmourData = armourData,
			Clans = clans ?? []
		};
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
			["Destroyable_Door"] = new(19, 0, "Destroyable_Door", "Destroyable"),
			["Variable_Colour"] = new(20, 0, "Variable_Colour", "Variable"),
			["Variable_BasicColour"] = new(21, 0, "Variable_BasicColour", "Variable"),
			["Variable_FineColour"] = new(22, 0, "Variable_FineColour", "Variable"),
			["Variable_DrabColour"] = new(23, 0, "Variable_DrabColour", "Variable"),
			["Variable_Gem"] = new(24, 0, "Variable_Gem", "Variable"),
			["Variable_FineGem"] = new(25, 0, "Variable_FineGem", "Variable"),
			["Variable_CommonStone"] = new(26, 0, "Variable_CommonStone", "Variable"),
			["Variable_RpiMixedVariables"] = new(27, 0, "Variable_RpiMixedVariables", "Variable"),
			["Wear_Ring"] = new(28, 0, "Wear_Ring", "Wearable"),
			["Wear_Gorget"] = new(29, 0, "Wear_Gorget", "Wearable"),
			["Wear_Necklace"] = new(30, 0, "Wear_Necklace", "Wearable"),
			["Wear_Choker"] = new(31, 0, "Wear_Choker", "Wearable"),
			["Wear_Backpack"] = new(32, 0, "Wear_Backpack", "Wearable"),
			["Wear_Cloak_(Closed)"] = new(33, 0, "Wear_Cloak_(Closed)", "Wearable"),
			["Wear_Cloak_(Open)"] = new(34, 0, "Wear_Cloak_(Open)", "Wearable"),
			["Wear_Cape"] = new(35, 0, "Wear_Cape", "Wearable"),
			["Wear_Bracelets"] = new(36, 0, "Wear_Bracelets", "Wearable"),
			["Wear_Earrings"] = new(37, 0, "Wear_Earrings", "Wearable"),
			["Wear_Anklets"] = new(38, 0, "Wear_Anklets", "Wearable"),
			["Wear_Wig"] = new(39, 0, "Wear_Wig", "Wearable"),
			["Wear_Armlet"] = new(40, 0, "Wear_Armlet", "Wearable"),
			["Wear_Mask"] = new(41, 0, "Wear_Mask", "Wearable"),
			["Wear_Veil"] = new(42, 0, "Wear_Veil", "Wearable"),
			["Wear_Backplate"] = new(43, 0, "Wear_Backplate", "Wearable"),
			["Wear_Bracers"] = new(44, 0, "Wear_Bracers", "Wearable"),
			["Armour_Platemail"] = new(45, 0, "Armour_Platemail", "Armour")
		};

		return new FutureMudBaselineCatalog
		{
			Components = components,
			ComponentsByType = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
			{
				["Food"] = ["Food_Generic"],
				["Variable"] =
				[
					"Variable_BasicColour",
					"Variable_CommonStone",
					"Variable_DrabColour",
					"Variable_FineColour",
					"Variable_FineGem",
					"Variable_Gem",
					"Variable_RpiMixedVariables",
					"Variable_Colour"
				]
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
			},
			ClansByAlias = new Dictionary<string, FutureMudClanReference>(StringComparer.OrdinalIgnoreCase)
			{
				["gondor"] = new FutureMudClanReference(
					42,
					"gondor",
					new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
					{
						["Captain"] = 4201
					})
			},
			FutureProgIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["AlwaysTrue"] = 1
			}
		};
	}
}
