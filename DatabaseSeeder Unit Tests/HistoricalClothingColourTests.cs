#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HistoricalClothingColourTests
{
	private static readonly IReadOnlyDictionary<string, string> ConventionalDefaults =
		new Dictionary<string, string>(StringComparer.Ordinal)
		{
			["medieval_jewish_tallit_gadol"] = "colour1=white colour2=black",
			["medieval_jewish_tallit_katan"] = "colour=white",
			["medieval_jewish_skullcap"] = "colour=black",
			["medieval_islamic_plain_imam_qamis"] = "colour=white",
			["medieval_hindu_white_priest_dhoti"] = "colour=white",
			["medieval_latin_amice"] = "colour=white",
			["medieval_latin_linen_cincture"] = "colour=white",
			["medieval_eastern_sticharion"] = "colour=white",
			["medieval_eastern_black_riassa"] = "colour=black",
			["medieval_eastern_kamilavkion"] = "colour=black",
			["medieval_hindu_kaupina"] = "colour=white",
			["medieval_jain_white_ascetic_robe"] = "colour=white",
			["medieval_jain_white_shoulder_wrap"] = "colour=white",
			["medieval_daoist_cross_collar_robe"] = "colour=black",
			["medieval_daoist_ritual_cap"] = "colour=black",
			["medieval_shinto_white_joe_robe"] = "colour=white",
			["medieval_shinto_priest_hakama"] = "colour=white",
			["medieval_shinto_miko_white_kosode"] = "colour=white",
			["medieval_shinto_miko_red_hakama"] = "colour=red"
		};

	private sealed record TextileContract(string Reference, string Noun, string Material, SizeCategory Size,
		ItemQuality Quality, double Weight, decimal Cost, string Components, string Tags, string? DefaultArguments,
		string VariableComponent = "Variable_Colour");

	// Pre-correction physical/economic contracts: adding colour must not silently change them.
	private static readonly TextileContract[] AdditionalTextiles =
	[
		new("earlymodern_footwear_indianocean_ropesole_deckshoes", "shoes", "canvas", SizeCategory.Small, ItemQuality.Standard, 420, 30.0m,
			"Holdable;Destroyable_Clothing;Wear_Shoes;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Functions / Worn Items / Footwear;Market / Clothing / Maritime Clothing", null),
		new("earlymodern_religious_daoist_cloud_shoes", "shoes", "cotton", SizeCategory.Small, ItemQuality.Good, 420, 96.0m,
			"Holdable;Destroyable_Clothing;Wear_Shoes;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=black"),
		new("earlymodern_religious_hindu_sacred_thread", "thread", "cotton", SizeCategory.VerySmall, ItemQuality.Good, 15, 24.0m,
			"Holdable;Destroyable_Clothing;Wear_Bandolier;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=white"),
		new("earlymodern_religious_jain_mouthcloth", "mouthcloth", "cotton", SizeCategory.VerySmall, ItemQuality.Good, 18, 18.0m,
			"Holdable;Destroyable_Clothing;Wear_Mask;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=white"),
		new("earlymodern_religious_reformed_preaching_bands", "bands", "linen", SizeCategory.VerySmall, ItemQuality.Good, 35, 48.0m,
			"Holdable;Destroyable_Clothing;Wear_Scarf;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=white"),
		new("earlymodern_religious_sikh_kachera", "kachera", "cotton", SizeCategory.Small, ItemQuality.Good, 180, 56.0m,
			"Holdable;Destroyable_Clothing;Wear_Shorts;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", null),
		new("earlymodern_religious_sikh_chola", "chola", "cotton", SizeCategory.Normal, ItemQuality.Good, 720, 120.0m,
			"Holdable;Destroyable_Clothing;Wear_Robe;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", null),
		new("earlymodern_religious_theravada_underrobe", "robe", "cotton", SizeCategory.Normal, ItemQuality.Good, 620, 72.0m,
			"Holdable;Destroyable_Clothing;Wear_Robe;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=\"saffron yellow\""),
		new("earlymodern_religious_theravada_upperrobe", "mantle", "cotton", SizeCategory.Normal, ItemQuality.Good, 780, 88.0m,
			"Holdable;Destroyable_Clothing;Wear_Mantle;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=\"saffron yellow\""),
		new("earlymodern_religious_tibetan_shamtab", "shamtab", "wool", SizeCategory.Normal, ItemQuality.Good, 720, 96.0m,
			"Holdable;Destroyable_Clothing;Wear_Long_Skirt;Armour_HeavyClothing;Insulation_Moderate", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=\"maroon red\""),
		new("earlymodern_religious_tibetan_vest", "vest", "wool", SizeCategory.Normal, ItemQuality.Good, 520, 88.0m,
			"Holdable;Destroyable_Clothing;Wear_Vest;Armour_HeavyClothing;Insulation_Moderate", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=\"maroon red\""),
		new("earlymodern_religious_tibetan_mantle", "mantle", "wool", SizeCategory.Normal, ItemQuality.Good, 940, 120.0m,
			"Holdable;Destroyable_Clothing;Wear_Mantle;Armour_HeavyClothing;Insulation_Moderate", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=\"maroon red\""),
		new("earlymodern_religious_zoroastrian_sudreh", "sudreh", "cotton", SizeCategory.Normal, ItemQuality.Good, 170, 32.0m,
			"Holdable;Destroyable_Clothing;Wear_Shirt;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=white"),
		new("earlymodern_religious_zoroastrian_kusti", "kusti", "wool", SizeCategory.Small, ItemQuality.Good, 65, 32.0m,
			"Holdable;Destroyable_Misc;Wear_Sash", "Era / Early Modern Era;Market / Religious Goods / Ritual Supplies", "colour=white"),
		new("earlymodern_religious_zoroastrian_prayer_cap", "cap", "cotton", SizeCategory.Small, ItemQuality.Good, 70, 40.0m,
			"Holdable;Destroyable_Clothing;Wear_Skullcap;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing;Market / Religious Goods / Ritual Supplies", "colour=white"),
		new("renaissance_institution_academic_robe", "robe", "broadcloth", SizeCategory.Normal, ItemQuality.Good, 1200.0, 120.0m,
			"Holdable;Destroyable_Clothing;Wear_Long_Open_Robe;Armour_LightClothing;Insulation_Moderate", "Era / Renaissance Era;Market / Clothing / Religious Clothing;Institution / Religious", null),
		new("renaissance_institution_full_cowl", "cowl", "wool", SizeCategory.Normal, ItemQuality.Good, 900.0, 80.0m,
			"Holdable;Destroyable_Clothing;Wear_Cloak_(Closed);Armour_LightClothing;Insulation_Moderate", "Era / Renaissance Era;Market / Clothing / Religious Clothing;Institution / Religious", null),
		new("renaissance_institution_linen_surplus", "surplice", "linen", SizeCategory.Normal, ItemQuality.Good, 650.0, 60.0m,
			"Holdable;Destroyable_Clothing;Wear_Tabard;Armour_LightClothing;Insulation_Minor", "Era / Renaissance Era;Market / Clothing / Religious Clothing;Institution / Religious", "colour=white"),
		new("renaissance_institution_monastic_scapular", "scapular", "wool", SizeCategory.Normal, ItemQuality.Good, 700.0, 55.0m,
			"Holdable;Destroyable_Clothing;Wear_Tabard;Armour_LightClothing;Insulation_Moderate", "Era / Renaissance Era;Market / Clothing / Religious Clothing;Institution / Religious", null),
		new("renaissance_institution_plain_cassock", "cassock", "broadcloth", SizeCategory.Normal, ItemQuality.Good, 1050.0, 100.0m,
			"Holdable;Destroyable_Clothing;Wear_Robe_Layer_0_75_NonBulky;Armour_LightClothing;Insulation_Moderate", "Era / Renaissance Era;Market / Clothing / Religious Clothing;Institution / Religious", null),
	];

	private static readonly TextileContract[] NativeGarments =
	[
		new("earlymodern_colonialnorthamerica_clothing_soft_moccasins", "moccasins", "leather", SizeCategory.Small, ItemQuality.Standard, 650, 33.0m,
			"Holdable;Destroyable_Clothing;Wear_Shoes;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing", null, "Variable_LeatherColour"),
		new("earlymodern_dutch_clothing_wooden_clogs", "clogs", "wood", SizeCategory.Small, ItemQuality.Standard, 700, 15.0m,
			"Holdable;Destroyable_Clothing;Wear_Shoes;Armour_LightClothing;Insulation_Minor", "Era / Early Modern Era;Market / Clothing / Standard Clothing", null, "Variable_WoodColour"),
		new("earlymodern_footwear_northamerican_furlined_moccasinboots", "boots", "deer leather", SizeCategory.Small, ItemQuality.Good, 720, 86.0m,
			"Holdable;Destroyable_Clothing;Wear_Boots;Armour_LightClothing;Insulation_Strong", "Era / Early Modern Era;Functions / Worn Items / Footwear;Market / Clothing / Standard Clothing", null, "Variable_LeatherColour"),
		new("medieval_leather_smith_apron", "apron", "leather", SizeCategory.Small, ItemQuality.Standard, 650.0, 28.0m,
			"Holdable;Wear_Apron;Destroyable_Clothing;Insulation_Minor;Armour_LightClothing", "Market / Clothing / Standard Clothing;Functions / Worn Items / Bodywear", null, "Variable_LeatherColour"),
		new("medieval_shinto_priest_eboshi", "eboshi", "silk", SizeCategory.Small, ItemQuality.Good, 110.0, 38.0m,
			"Holdable;Wear_Hat;Destroyable_Clothing;Insulation_Minor;Armour_LightClothing", "Market / Clothing / Luxury Clothing;Functions / Worn Items / Headwear", "colour=black", "Variable_LacquerColour"),
		new("preindustrial_clothing_plain_leather_belt", "belt", "leather", SizeCategory.Small, ItemQuality.Standard, 180.0, 10.0m,
			"Holdable;Wear_Waist;Destroyable_Clothing;Insulation_Minor;Armour_LightClothing;Belt_2", "Market / Clothing / Standard Clothing;Functions / Worn Items / Belts", null, "Variable_LeatherColour"),
		new("renaissance_frontier_split_skirt_riding_boots", "boots", "leather", SizeCategory.Small, ItemQuality.Good, 1220, 151.0m,
			"Holdable;Destroyable_Clothing;Wear_High_Boots;Armour_LightClothing;Insulation_Minor", "Era / Renaissance Era;Market / Clothing / Luxury Clothing", null, "Variable_LeatherColour"),
		new("renaissance_japanese_wooden_clogs", "clogs", "wood", SizeCategory.Small, ItemQuality.Standard, 700, 15.0m,
			"Holdable;Destroyable_Clothing;Wear_Shoes;Armour_LightClothing;Insulation_Minor", "Era / Renaissance Era;Market / Clothing / Standard Clothing", null, "Variable_WoodColour"),
		new("renaissance_shared_clothing_leather_gloves", "gloves", "leather", SizeCategory.Small, ItemQuality.Standard, 220, 21.0m,
			"Holdable;Destroyable_Clothing;Wear_Gloves;Armour_LightClothing;Insulation_Minor", "Era / Renaissance Era;Market / Clothing / Standard Clothing", null, "Variable_LeatherColour"),
		new("renaissance_shared_clothing_leather_sandals", "sandals", "leather", SizeCategory.Small, ItemQuality.Standard, 410, 21.0m,
			"Holdable;Destroyable_Clothing;Wear_Sandals;Armour_LightClothing;Insulation_Minor", "Era / Renaissance Era;Market / Clothing / Standard Clothing", null, "Variable_LeatherColour"),
		new("renaissance_shared_clothing_soft_slippers", "slippers", "leather", SizeCategory.Small, ItemQuality.Standard, 650, 33.0m,
			"Holdable;Destroyable_Clothing;Wear_Shoes;Armour_LightClothing;Insulation_Minor", "Era / Renaissance Era;Market / Clothing / Standard Clothing", null, "Variable_LeatherColour"),
		new("renaissance_southasian_toepost_woodensandals", "sandals", "wood", SizeCategory.Small, ItemQuality.Standard, 450, 10.0m,
			"Holdable;Destroyable_Clothing;Wear_Sandals;Armour_LightClothing;Insulation_Minor", "Era / Renaissance Era;Market / Clothing / Standard Clothing", null, "Variable_WoodColour"),
	];

	[TestMethod]
	public void AuthoredTextileSources_HaveVariableProseAndExactlyOneColourComponent()
	{
		Assert.AreEqual(19, ConventionalDefaults.Count);
		foreach (var (reference, arguments) in ConventionalDefaults)
		{
			var source = ItemSeeder.FindHistoricalClothingSource(reference)!;
			Assert.IsNotNull(source, reference);
			Assert.IsTrue(source.Skinnable, reference);
			var variables = source.Components.Where(x => x.StartsWith("Variable_", StringComparison.Ordinal)).ToArray();
			CollectionAssert.AreEqual(new[] { arguments.Contains("colour2=") ? "Variable_2Colour" : "Variable_Colour" }, variables, reference);
			foreach (var argument in arguments.Split(' '))
			{
				var variable = argument.Split('=')[0];
				StringAssert.Contains(source.FullDescription, "$" + variable, reference);
			}
			StringAssert.Contains(source.ShortDescription, "$colour", reference);
			Assert.IsFalse(source.FullDescription.Contains("Source:", StringComparison.OrdinalIgnoreCase), reference);
		}
	}

	[TestMethod]
	public void HistoricalOutfits_KeepConventionsOnEntriesNotOnGarments()
	{
		var entries = ItemSeeder.MedievalOutfitManifestSpecsForTesting
			.Concat(ItemSeeder.RenaissanceOutfitManifestSpecsForTesting)
			.Concat(ItemSeeder.EarlyModernOutfitManifestSpecsForTesting)
			.SelectMany(x => x.Items).ToArray();
		foreach (var (reference, arguments) in ConventionalDefaults)
		{
			var uses = entries.Where(x => x.ItemStableReference == reference).ToArray();
			Assert.IsTrue(uses.Length > 0, reference);
			Assert.IsTrue(uses.All(x => x.SkinStableReference is null), reference);
			Assert.IsTrue(uses.All(x => x.LoadArguments == arguments), reference);
		}
	}

	[TestMethod]
	public void AllNineteenSources_ResolveCompleteRealStockPalettesAndAlternativeSelections()
	{
		using var context = Stock();
		var snapshot = IndustrialisedClothingColourBindings.Read(context);
		var location = new ClothingSourceLocation("HistoricalClothingColourTests", 1);
		var values = context.CharacteristicValues.Where(x => x.Definition.Name == "Colour")
			.Select(x => x.Name).OrderBy(x => x).ToArray();
		Assert.IsTrue(values.Length > 3);
		Assert.AreEqual(0, values.GroupBy(x => x, StringComparer.OrdinalIgnoreCase).Count(x => x.Count() > 1));
		foreach (var (reference, arguments) in ConventionalDefaults)
		{
			var source = ItemSeeder.FindHistoricalClothingSource(reference)!;
			var componentName = source.Components.Single(x => x.StartsWith("Variable_", StringComparison.Ordinal));
			var component = context.GameItemComponentProtos.Single(x => x.Name == componentName);
			var channels = new Dictionary<string, ClothingColourRow>(StringComparer.Ordinal);
			foreach (var characteristic in XElement.Parse(component.Definition).Elements("Characteristic"))
			{
				var definitionId = (long)characteristic.Attribute("Value")!;
				var profileId = (long)characteristic.Attribute("Profile")!;
				var definition = context.CharacteristicDefinitions.Single(x => x.Id == definitionId);
				var profile = context.CharacteristicProfiles.Single(x => x.Id == profileId);
				var variable = definition.Name.ToLowerInvariant();
				channels.Add(variable, new(location, reference, variable, definition.Name, profile.Name,
					values, "blue", "", "", ""));
			}
			var bound = snapshot.Bind(channels, [component], location);
			var defaults = arguments.Split(' ').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
			var numeric = IndustrialisedClothingColourBindings.LoadArguments(bound, defaults, location);
			Assert.AreEqual(defaults.Count, numeric.Split(' ').Length);
			foreach (var argument in numeric.Split(' '))
				Assert.IsTrue(long.Parse(argument.Split('=')[1]) > 0, reference);
			var alternative = bound.Keys.ToDictionary(x => x, _ => "blue");
			Assert.AreNotEqual(numeric, IndustrialisedClothingColourBindings.LoadArguments(bound, alternative, location), reference);
		}
	}

	[TestMethod]
	public void AdditionalTextiles_PreservePhysicalEconomicsAndExposeCompleteStandaloneVariableProse()
	{
		Assert.AreEqual(20, AdditionalTextiles.Length);
		Assert.AreEqual(12, NativeGarments.Length);
		foreach (var expected in AdditionalTextiles.Concat(NativeGarments))
		{
			var source = ItemSeeder.FindHistoricalClothingSource(expected.Reference)!;
			Assert.IsNotNull(source, expected.Reference);
			Assert.AreEqual(expected.Noun, source.Noun);
			Assert.AreEqual(expected.Material, source.Material);
			Assert.AreEqual(expected.Size, source.Size);
			Assert.AreEqual(expected.Quality, source.Quality);
			Assert.AreEqual(expected.Weight, source.WeightInGrams);
			Assert.AreEqual(expected.Cost, source.Cost);
			CollectionAssert.AreEqual(expected.Components.Split(';').Append(expected.VariableComponent).ToArray(), source.Components.ToArray());
			CollectionAssert.AreEqual(expected.Tags.Split(';'), source.Tags.ToArray());
			Assert.IsTrue(source.Skinnable);
			Assert.IsFalse(source.HiddenFromPlayers);
			Assert.IsNull(source.LongDescription);
			Assert.IsNull(source.MorphTo);
			Assert.IsNull(source.DestroyedItem);
			StringAssert.Contains(source.ShortDescription, "$colour");
			StringAssert.Contains(source.FullDescription, "$colour");
			Assert.IsTrue(source.FullDescription.Length >= 300);
			Assert.IsFalse(source.FullDescription.Contains("Full description:", StringComparison.Ordinal));
			Assert.IsFalse(source.FullDescription.Contains("[skinnable]", StringComparison.Ordinal));
		}
	}

	[TestMethod]
	public void AdditionalTextileOutfits_KeepResolvableConventionsLocalAndAllowOtherColours()
	{
		using var context = Stock();
		var snapshot = IndustrialisedClothingColourBindings.Read(context);
		var component = context.GameItemComponentProtos.Single(x => x.Name == "Variable_Colour");
		var characteristic = XElement.Parse(component.Definition).Element("Characteristic")!;
		var definition = context.CharacteristicDefinitions.Single(x => x.Id == (long)characteristic.Attribute("Value")!);
		var profile = context.CharacteristicProfiles.Single(x => x.Id == (long)characteristic.Attribute("Profile")!);
		var values = context.CharacteristicValues.Where(x => x.Definition.Name == "Colour").Select(x => x.Name).ToArray();
		var outfits = ItemSeeder.RenaissanceOutfitManifestSpecsForTesting
			.Concat(ItemSeeder.EarlyModernOutfitManifestSpecsForTesting).ToArray();
		foreach (var expected in AdditionalTextiles)
		{
			var location = new ClothingSourceLocation(expected.Reference, 1);
			var channels = new Dictionary<string, ClothingColourRow>
			{
				["colour"] = new(location, expected.Reference, "colour", definition.Name, profile.Name, values, "blue", "", "", "")
			};
			var bound = snapshot.Bind(channels, [component], location);
			var uses = outfits.SelectMany(outfit => outfit.Items
				.Where(item => item.ItemStableReference == expected.Reference).Select(item => (outfit.StableKey, Item: item))).ToArray();
			if (expected.Reference == "earlymodern_footwear_indianocean_ropesole_deckshoes")
				Assert.AreEqual(0, uses.Length, "This standalone historical stock has later clothing-plan consumers, not an existing historical outfit.");
			else
				Assert.IsTrue(uses.Length > 0, expected.Reference);
			foreach (var (outfit, item) in uses)
			{
				var arguments = expected.Reference == "renaissance_institution_academic_robe" && outfit == "earlymodern_outfit_0884"
					? "colour=black" : expected.DefaultArguments ?? "";
				Assert.AreEqual(arguments, item.LoadArguments, outfit + " / " + expected.Reference);
				if (arguments.Length == 0) continue;
				var colour = arguments["colour=".Length..].Trim('"');
				var selected = IndustrialisedClothingColourBindings.LoadArguments(bound, new Dictionary<string, string> { ["colour"] = colour }, location);
				var alternative = IndustrialisedClothingColourBindings.LoadArguments(bound, new Dictionary<string, string> { ["colour"] = "blue" }, location);
				Assert.AreNotEqual(selected, alternative, expected.Reference);
			}
		}
		var judicial = ItemSeeder.DocumentedClothingSkinsForTesting.Single(x => x.StableReference == "earlymodern_skin_judicial_full_sleeved_robe");
		Assert.IsNull(judicial.Quality);
		StringAssert.Contains(judicial.ShortDescription, "$colour");
		StringAssert.Contains(judicial.FullDescription, "$colour");
	}

	[TestMethod]
	public void NativeProfiles_BindEveryConsumerAndRejectOutOfPaletteSelectionsWithoutMutatingStock()
	{
		using var context = Stock();
		using var export = JsonDocument.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(
			DatabaseSeeder.ItemSeederManifestCatalogue.FindRepositoryRoot(), "Design Documents", "Data", "Seeded_Item_Components.json")));
		foreach (var name in new[] { "Variable_LeatherColour", "Variable_WoodColour", "Variable_LacquerColour" })
		{
			var live = context.GameItemComponentProtos.Single(x => x.Name == name);
			var row = export.RootElement.EnumerateArray().Single(x => x.GetProperty("Component Name").GetString() == name);
			Assert.AreEqual(live.Description, row.GetProperty("Component Description").GetString());
			Assert.AreEqual(live.Type, row.GetProperty("Component Type").GetString());
		}
		var snapshot = IndustrialisedClothingColourBindings.Read(context);
		var components = context.GameItemComponentProtos.ToDictionary(x => x.Name);
		var profiles = context.CharacteristicProfiles.ToDictionary(x => x.Id);
		var definitions = context.CharacteristicDefinitions.ToDictionary(x => x.Id);
		foreach (var expected in NativeGarments)
		{
			var location = new ClothingSourceLocation(expected.Reference, 1);
			var component = components[expected.VariableComponent];
			var characteristic = XElement.Parse(component.Definition).Element("Characteristic")!;
			var definition = definitions[(long)characteristic.Attribute("Value")!];
			var profile = profiles[(long)characteristic.Attribute("Profile")!];
			var values = XElement.Parse(profile.Definition).Elements("Value").Select(x => x.Value).ToArray();
			var channel = new ClothingColourRow(location, expected.Reference, "colour", definition.Name, profile.Name, values, values[0], "", "", "");
			var bound = snapshot.Bind(new Dictionary<string, ClothingColourRow> { ["colour"] = channel }, [component], location);
			var arguments = values.Select(value => IndustrialisedClothingColourBindings.LoadArguments(bound,
				new Dictionary<string, string> { ["colour"] = value }, location)).ToArray();
			Assert.AreEqual(values.Length, arguments.Distinct().Count(), expected.Reference);
			Assert.IsTrue(arguments.Length >= 3, expected.Reference);
			Assert.ThrowsException<System.IO.InvalidDataException>(() => IndustrialisedClothingColourBindings.LoadArguments(bound,
				new Dictionary<string, string> { ["colour"] = "blue" }, location));
			Assert.IsFalse(context.ChangeTracker.Entries().Any(x => x.State != EntityState.Unchanged));
		}
		var before = components.ToDictionary(x => x.Key, x => (x.Value.Id, x.Value.RevisionNumber, x.Value.Definition));
		new UsefulSeeder().SeedVariablesForTesting(context);
		new UsefulSeeder().SeedVariablesForTesting(context);
		Assert.AreEqual(before.Count, context.GameItemComponentProtos.Count());
		foreach (var component in context.GameItemComponentProtos)
			Assert.AreEqual(before[component.Name], (component.Id, component.RevisionNumber, component.Definition));
	}

	[TestMethod]
	public void EboshiBlackConvention_IsAnOutfitDefaultAndTheBeltRetainsItsAlias()
	{
		var entries = ItemSeeder.MedievalOutfitManifestSpecsForTesting
			.Concat(ItemSeeder.RenaissanceOutfitManifestSpecsForTesting)
			.Concat(ItemSeeder.EarlyModernOutfitManifestSpecsForTesting).SelectMany(x => x.Items)
			.Where(x => x.ItemStableReference == "medieval_shinto_priest_eboshi").ToArray();
		Assert.IsTrue(entries.Length > 0);
		Assert.IsTrue(entries.All(x => x.LoadArguments == "colour=black"));
		Assert.AreEqual("medieval_plain_leather_belt", ItemSeeder.FindHistoricalClothingSource("preindustrial_clothing_plain_leather_belt")!.LegacyAliasReference);
	}

	[DataTestMethod]
	[DataRow("missing")]
	[DataRow("duplicate")]
	[DataRow("case")]
	[DataRow("target")]
	public void NativeVariablePrerequisiteFailure_AddsNoPartialComponentBatch(string mutation)
	{
		using var context = Stock();
		context.GameItemComponentProtos.RemoveRange(context.GameItemComponentProtos);
		var profile = context.CharacteristicProfiles.Single(x => x.Name == "Clothing_Lacquer_Colours");
		switch (mutation)
		{
			case "missing": context.CharacteristicProfiles.Remove(profile); break;
			case "case": profile.Name = "CLOTHING_LACQUER_COLOURS"; break;
			case "target": profile.TargetDefinitionId = context.CharacteristicDefinitions.Single(x => x.Name == "Colour1").Id; break;
			case "duplicate": context.CharacteristicProfiles.Add(new CharacteristicProfile
			{
				Name = profile.Name, Type = profile.Type, Description = "Duplicate prerequisite",
				TargetDefinitionId = profile.TargetDefinitionId, Definition = profile.Definition
			}); break;
			default: Assert.Fail(mutation); break;
		}
		context.SaveChanges();
		var exception = Assert.ThrowsException<InvalidOperationException>(() => new UsefulSeeder().SeedVariablesForTesting(context));
		StringAssert.Contains(exception.Message, "Clothing_Lacquer_Colours");
		Assert.AreEqual(0, context.GameItemComponentProtos.Count());
		Assert.IsFalse(context.ChangeTracker.Entries().Any(x => x.State != EntityState.Unchanged));
	}

	private static FuturemudDatabaseContext Stock()
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		context.Accounts.Add(new Account
		{
			Id = 1, Name = "ColourTest", Password = "unused", Salt = 1, Email = "test@example.invalid",
			LastLoginIp = "127.0.0.1", FormatLength = 80, InnerFormatLength = 78, ActiveCharactersAllowed = 1,
			TimeZoneId = "UTC", CultureName = "en-AU", RegistrationCode = "", RecoveryCode = "",
			UnitPreference = "metric", CreationDate = DateTime.UtcNow
		});
		// CoreDataSeeder installs earlier characteristic values before its colour catalogue.
		var priorDefinition = new CharacteristicDefinition
		{
			Id = 9000, Name = "Prior stock characteristic", Pattern = "^prior$",
			Description = "Colour-writer prerequisite", Model = "standard"
		};
		context.CharacteristicDefinitions.Add(priorDefinition);
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = 1, Name = "prior", Definition = priorDefinition, Value = "prior"
		});
		context.SaveChanges();
		new CoreDataSeeder().SeedColours(context);
		new UsefulSeeder().SeedVariablesForTesting(context);
		return context;
	}

	[TestMethod]
	public void AllVariableCapableInventoryRows_ResolveTheirActualStockChannelsAndCompleteProfiles()
	{
		using var context = Stock();
		var snapshot = IndustrialisedClothingColourBindings.Read(context);
		var location = new ClothingSourceLocation("HistoricalClothingColourTests", 1);
		var definitions = context.CharacteristicDefinitions.ToDictionary(x => x.Id);
		var values = context.CharacteristicValues.ToArray();
		var profiles = context.CharacteristicProfiles.ToDictionary(x => x.Id);
		var components = context.GameItemComponentProtos.ToDictionary(x => x.Name);
		var rows = IndustrialisedClothingDependencyPlan.Rows
			.Where(x => x.Components.Any(c => c.StartsWith("Variable_", StringComparison.Ordinal))).ToArray();
		Assert.AreEqual(364, rows.Length);
		foreach (var row in rows)
		{
			var component = components[row.Components.Single(x => x.StartsWith("Variable_", StringComparison.Ordinal))];
			var channels = new Dictionary<string, ClothingColourRow>(StringComparer.Ordinal);
			foreach (var characteristic in XElement.Parse(component.Definition).Elements("Characteristic"))
			{
				var definition = definitions[(long)characteristic.Attribute("Value")!];
				var profile = profiles[(long)characteristic.Attribute("Profile")!];
				var ancestors = new HashSet<long>();
				for (CharacteristicDefinition? ancestor = definitions[profile.TargetDefinitionId]; ancestor is not null;
					ancestor = ancestor.ParentId is { } parent ? definitions[parent] : null)
					Assert.IsTrue(ancestors.Add(ancestor.Id), row.ItemReference);
				var candidates = values.Where(x => ancestors.Contains(x.DefinitionId)).ToArray();
				Assert.IsTrue(profile.Type.Equals("all", StringComparison.OrdinalIgnoreCase) ||
					profile.Type.Equals("standard", StringComparison.OrdinalIgnoreCase), profile.Name);
				var allowed = profile.Type.Equals("all", StringComparison.OrdinalIgnoreCase)
					? candidates.Select(x => x.Name).ToArray()
					: XElement.Parse(profile.Definition).Elements("Value")
						.Select(x => candidates.Single(v => v.Name == x.Value || v.Id.ToString() == x.Value).Name)
						.Distinct().ToArray();
				var variable = new[] { "colour", "colour1", "colour2", "colour3", "finecolour", "drabcolour", "finish" }
					.First(x => Regex.IsMatch(x, definition.Pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
				channels.Add(variable, new(location, row.ItemReference, variable, definition.Name, profile.Name,
					allowed, allowed[0], "", "", ""));
			}
			var bound = snapshot.Bind(channels, [component], location);
			Assert.AreEqual(channels.Count, bound.Count, row.ItemReference);
			// This proves stock channel/profile integrity, not material-specific palette approval.
		}
	}
}
