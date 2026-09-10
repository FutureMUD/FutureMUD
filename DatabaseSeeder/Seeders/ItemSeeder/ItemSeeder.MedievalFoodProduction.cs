#nullable enable

using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string MedievalPreparedFoodTagPath = "Food and Drink / Medieval Food / Prepared Foods";

	private static readonly string[] MedievalFoodProductionToolStableReferences =
	[
		"medieval_tool_butchers_knife",
		"medieval_tool_cooking_knife",
		"medieval_tool_threshing_flail",
		"medieval_tool_winnowing_basket",
		"medieval_tool_cooking_pot",
		"medieval_workshop_lauter_tun"
	];

	private static readonly string[] MedievalPreparedFoodStableReferences =
	[
		"medieval_food_coarse_bread_loaf",
		"medieval_food_flatbread",
		"medieval_food_hard_bread",
		"medieval_food_grain_pottage",
		"medieval_food_meat_pottage",
		"medieval_food_salted_meat_ration",
		"medieval_food_dried_meat_ration",
		"medieval_food_smoked_meat_ration",
		"medieval_food_salted_fish_ration",
		"medieval_food_dried_fish_ration",
		"medieval_food_smoked_fish_ration"
	];

	internal static IReadOnlyCollection<string> MedievalFoodProductionToolStableReferencesForTesting =>
		MedievalFoodProductionToolStableReferences;

	internal static IReadOnlyCollection<string> MedievalPreparedFoodStableReferencesForTesting =>
		MedievalPreparedFoodStableReferences;

	private void SeedMedievalFoodProductionFoundationItems()
	{
		EnsureAntiquityTagPath(MedievalPreparedFoodTagPath);
		EnsureMedievalPreparedFoodComponents();
		SeedMedievalFoodProductionTools();
		SeedMedievalPreparedFoods();
	}

	private void EnsureMedievalPreparedFoodComponents()
	{
		EnsurePreparedFoodComponent(
			"PreparedFood_Medieval_Bread",
			"Medieval bread and flatbread",
			FoodDefinition(
				"Item", 3.5, 0.05, -0.1, 0.0, 6, 1.0, 0.75, 0.2, 0.15, 3, 8,
				"It tastes of toasted grain, smoke, and a little salt.",
				"$sdesc",
				"$sdesc\nIt has been baked from $ingredients.",
				("grain", "prepared grain dough", "toasted grain")));
		EnsurePreparedFoodComponent(
			"PreparedFood_Medieval_HardBread",
			"Medieval hard bread and travel biscuit",
			FoodDefinition(
				"Item", 3.0, 0.0, -0.2, 0.0, 8, 1.0, 0.9, 0.4, 0.05, 30, 120,
				"It tastes dry, hard, and strongly of baked grain.",
				"$sdesc",
				"$sdesc\nIt is a durable travel bread baked from $ingredients.",
				("grain", "twice-baked grain dough", "dry toasted grain")));
		EnsurePreparedFoodComponent(
			"PreparedFood_Medieval_Pottage",
			"Medieval porridges and pottages",
			FoodDefinition(
				"Item", 4.5, 0.3, 0.1, 0.0, 8, 1.0, 0.7, 0.2, 0.45, 2, 5,
				"It tastes warm, soft, and filling.",
				"$sdesc",
				"$sdesc\nIt is a moist cooked dish made from $ingredients.",
				("ingredient", "boiled grain and additions", "warm savoury grain")));
		EnsurePreparedFoodComponent(
			"PreparedFood_Medieval_PreservedProvision",
			"Medieval preserved meat and fish provisions",
			FoodDefinition(
				"Item", 3.5, 0.02, -0.2, 0.0, 6, 1.0, 0.9, 0.45, 0.08, 14, 60,
				"It tastes salty, dry, and concentrated.",
				"$sdesc",
				"$sdesc\nIt is a preserved provision prepared from $ingredients.",
				("provision", "preserved meat or fish", "salty preserved flesh")));
	}

	private void SeedMedievalFoodProductionTools()
	{
		CreateItem(
			"medieval_tool_butchers_knife",
			"knife",
			"a broad iron butcher's knife",
			null,
			"This broad wrought-iron knife has a thick spine, a keen working edge, and an ash handle pinned firmly through the tang for breaking down heavy raw cuts.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"wrought iron",
			["Functions / Tools / Butcher Tools / Meat Cutting Tools / Butcher's Knife", "Market / Professional Tools / Standard Tools"],
			["Holdable", "Destroyable_Weapon"],
			null,
			null,
			null,
			null);
		CreateItem(
			"medieval_tool_cooking_knife",
			"knife",
			"an iron cooking knife",
			null,
			"This practical wrought-iron kitchen knife has a medium blade, a plain wooden handle, and a fine edge suited to chopping vegetables, trimming dough, and preparing cooked food.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			12.0m,
			false,
			false,
			"wrought iron",
			["Functions / Tools / Cooking / Cooking Utensils / Cooking Knife", "Market / Professional Tools / Standard Tools"],
			["Holdable", "Destroyable_Weapon"],
			null,
			null,
			null,
			null);
		CreateItem(
			"medieval_tool_threshing_flail",
			"flail",
			"a jointed wooden threshing flail",
			null,
			"This long ash handle is joined by a short leather hinge to a heavier oak swipple, forming a durable flail for beating grain loose from straw.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			8.0m,
			false,
			false,
			"oak",
			["Functions / Tools / Foodmaking Tools / Threshing Flail", "Market / Professional Tools / Standard Tools"],
			["Holdable", "Destroyable_Misc"],
			null,
			null,
			null,
			null);
		CreateItem(
			"medieval_tool_winnowing_basket",
			"basket",
			"a broad woven winnowing basket",
			null,
			"This broad, shallow willow basket has a tightly woven base and low rim, allowing threshed grain to be tossed into a breeze while chaff blows free.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			7.0m,
			false,
			false,
			"willow",
			["Functions / Tools / Agricultural Tools / Winnowing Basket", "Market / Professional Tools / Standard Tools"],
			["Holdable", "Destroyable_Misc"],
			null,
			null,
			null,
			null);
		CreateItem(
			"medieval_tool_cooking_pot",
			"pot",
			"a soot-darkened bronze cooking pot",
			null,
			"This round-bellied bronze pot has a reinforced rim, a fitted carrying bail, and a blackened base from long use over hearth fires.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			55.0m,
			false,
			false,
			"bronze",
			["Functions / Tools / Cooking / Cookware / Cooking Pot", "Market / Professional Tools / Standard Tools"],
			["Holdable", "Destroyable_HeavyMetal"],
			null,
			null,
			null,
			null);
		CreateItem(
			"medieval_workshop_lauter_tun",
			"tun",
			"a slatted wooden lauter tun",
			null,
			"This broad oak tun has tight coopered staves, a slatted false bottom, and a plugged outlet low on one side for separating sweet wort from spent grain.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			68.0m,
			false,
			false,
			"oak",
			["Functions / Tools / Brewing Tools / Lauter Tun", "Market / Professional Tools / Standard Tools"],
			["Holdable", "Destroyable_Furniture"],
			null,
			null,
			null,
			null);
	}

	private void SeedMedievalPreparedFoods()
	{
		CreateMedievalPreparedFood(
			"medieval_food_coarse_bread_loaf", "loaf", "a coarse brown bread loaf",
			"This heavy round loaf has a dark, flour-dusted crust and a dense crumb flecked with coarse meal.",
			SizeCategory.Small, 700.0, "bread", "PreparedFood_Medieval_Bread");
		CreateMedievalPreparedFood(
			"medieval_food_flatbread", "bread", "a broad griddle-baked flatbread",
			"This broad, flexible flatbread is browned in patches and carries faint flour and smoke marks from a hot griddle.",
			SizeCategory.Small, 320.0, "bread", "PreparedFood_Medieval_Bread");
		CreateMedievalPreparedFood(
			"medieval_food_hard_bread", "bread", "a hard travel bread",
			"This twice-baked slab of bread is pale, dry, and hard enough to keep through a long journey when stored away from damp.",
			SizeCategory.Small, 450.0, "bread", "PreparedFood_Medieval_HardBread");
		CreateMedievalPreparedFood(
			"medieval_food_grain_pottage", "pottage", "a bowlful of thick grain pottage",
			"This thick spoonable pottage is made from simmered coarse grain, with a soft texture and a plain toasted aroma.",
			SizeCategory.Small, 520.0, "bread", "PreparedFood_Medieval_Pottage");
		CreateMedievalPreparedFood(
			"medieval_food_meat_pottage", "pottage", "a bowlful of meat and grain pottage",
			"This rich pottage combines softened grain with small pieces of cooked meat in a thick savoury broth.",
			SizeCategory.Small, 650.0, "meat", "PreparedFood_Medieval_Pottage");
		CreateMedievalPreparedFood(
			"medieval_food_salted_meat_ration", "ration", "a ration of salted meat",
			"These firm cuts of meat have been heavily salted and packed together as a compact stored ration.",
			SizeCategory.Small, 520.0, "meat", "PreparedFood_Medieval_PreservedProvision");
		CreateMedievalPreparedFood(
			"medieval_food_dried_meat_ration", "ration", "a ration of dried meat",
			"These dark strips of lean meat have been dried until tough and light, making a durable travelling ration.",
			SizeCategory.Small, 380.0, "meat", "PreparedFood_Medieval_PreservedProvision");
		CreateMedievalPreparedFood(
			"medieval_food_smoked_meat_ration", "ration", "a ration of smoked meat",
			"These browned cuts of meat carry a dark smoky rind and have been bundled as a compact preserved ration.",
			SizeCategory.Small, 430.0, "meat", "PreparedFood_Medieval_PreservedProvision");
		CreateMedievalPreparedFood(
			"medieval_food_salted_fish_ration", "ration", "a ration of salted fish",
			"These firm fish pieces have been packed heavily with salt and bundled as a compact stored ration.",
			SizeCategory.Small, 500.0, "fish", "PreparedFood_Medieval_PreservedProvision");
		CreateMedievalPreparedFood(
			"medieval_food_dried_fish_ration", "ration", "a ration of dried fish",
			"These split fish pieces have been air-dried until light and leathery, ready for travel or later soaking.",
			SizeCategory.Small, 340.0, "fish", "PreparedFood_Medieval_PreservedProvision");
		CreateMedievalPreparedFood(
			"medieval_food_smoked_fish_ration", "ration", "a ration of smoked fish",
			"These fish pieces have a golden-brown surface and a strong woodsmoke aroma from slow curing above a low fire.",
			SizeCategory.Small, 390.0, "fish", "PreparedFood_Medieval_PreservedProvision");
	}

	private GameItemProto? CreateMedievalPreparedFood(
		string stableReference,
		string noun,
		string shortDescription,
		string fullDescription,
		SizeCategory size,
		double weight,
		string material,
		string preparedFoodComponent)
	{
		return CreateItem(
			stableReference,
			noun,
			shortDescription,
			null,
			fullDescription,
			size,
			ItemQuality.Standard,
			weight,
			4.0m,
			false,
			false,
			material,
			[MedievalPreparedFoodTagPath, "Market / Household Goods / Standard Wares"],
			["Holdable", "Destroyable_Misc", preparedFoodComponent],
			null,
			null,
			null,
			null);
	}
}
