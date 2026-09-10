#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string MedievalFoodKnowledge = "Medieval Food Production";
	private const string MedievalFoodCraftedSource = "medieval_food_crafted";

	internal sealed record MedievalFoodDependencyTestData(
		string Contract,
		string? StableReference,
		string SourceStatus,
		string SourceOwner,
		int SourcePhase);

	internal sealed record MedievalFoodCraftSpecTestData(
		int Phase,
		string Name,
		string Category,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		string KnowledgeSubtype,
		IReadOnlyCollection<string> Inputs,
		IReadOnlyCollection<string> Tools,
		IReadOnlyCollection<string> Products,
		IReadOnlyCollection<MedievalFoodDependencyTestData> Dependencies,
		IReadOnlyCollection<string> SourceOwnership);

	private sealed record MedievalFoodInput(
		string Import,
		string? StableReference,
		string SourceStatus,
		string SourceOwner,
		int SourcePhase);

	private sealed record MedievalFoodTool(
		string Tag,
		string Location,
		string SourceStatus,
		string SourceOwner,
		int SourcePhase);

	private sealed record MedievalFoodIngredient(int InputIndex, string Role);

	private sealed record MedievalFoodProduct(
		string Kind,
		string? StableReference,
		string? Material,
		string? PileTag,
		double Amount,
		string? Liquid,
		IReadOnlyList<MedievalFoodIngredient> Ingredients);

	private sealed record MedievalFoodCraftSpec(
		int Phase,
		string Name,
		string Category,
		string Trait,
		int MinimumTraitValue,
		Difficulty Difficulty,
		string KnowledgeSubtype,
		IReadOnlyList<MedievalFoodInput> Inputs,
		IReadOnlyList<MedievalFoodTool> Tools,
		IReadOnlyList<MedievalFoodProduct> Products);

	internal static IReadOnlyCollection<MedievalFoodCraftSpecTestData> MedievalFoodCraftSpecsForTesting =>
		MedievalFoodCraftSpecs()
			.Select(x =>
			{
				var dependencies = x.Inputs
					.Select(input => new MedievalFoodDependencyTestData(
						input.StableReference ?? input.Import,
						input.StableReference,
						input.SourceStatus,
						input.SourceOwner,
						input.SourcePhase))
					.Concat(x.Tools.Select(tool => new MedievalFoodDependencyTestData(
						FoodToolImport(tool),
						null,
						tool.SourceStatus,
						tool.SourceOwner,
						tool.SourcePhase)))
					.ToArray();
				return new MedievalFoodCraftSpecTestData(
					x.Phase,
					x.Name,
					x.Category,
					x.Trait,
					x.MinimumTraitValue,
					x.Difficulty,
					x.KnowledgeSubtype,
					x.Inputs.Select(input => input.StableReference ?? input.Import).ToArray(),
					x.Tools.Select(FoodToolImport).ToArray(),
					x.Products.Select(ProductContractForTesting).ToArray(),
					dependencies,
					dependencies
						.Select(y => y.SourceStatus)
						.Append(MedievalFoodCraftedSource)
						.Distinct(StringComparer.Ordinal)
						.ToArray());
			})
			.ToArray();

	internal void SeedMedievalFoodBeverageCraftsForTesting(FuturemudDatabaseContext context)
	{
		InitialiseCraftAuthoringForTesting(context);
		_questionAnswers = new Dictionary<string, string>
		{
			["eras"] = "medieval"
		};
		SeedMedievalFoodBeverageCrafts();
	}

	private void SeedMedievalFoodBeverageCrafts()
	{
		if (!ShouldSeedMedievalCrafts())
		{
			return;
		}

		foreach (var spec in MedievalFoodCraftSpecs().OrderBy(x => x.Phase))
		{
			AddMedievalFoodCraft(spec);
		}
	}

	private Craft? AddMedievalFoodCraft(MedievalFoodCraftSpec spec)
	{
		return AddCraft(
			spec.Name,
			spec.Category,
			spec.Name,
			spec.Name,
			$"{spec.Name} in progress",
			MedievalFoodKnowledge,
			spec.Trait,
			spec.MinimumTraitValue,
			spec.Difficulty,
			Outcome.MinorFail,
			5,
			3,
			false,
			MedievalFoodCraftingPhases(spec.Phase),
			spec.Inputs.Select(x => x.StableReference is null ? x.Import : StableSimpleItemInput(x.StableReference)),
			spec.Tools.Select(FoodToolImport),
			spec.Products.Select(ProductImport),
			[],
			knowledgeSubtype: spec.KnowledgeSubtype,
			knowledgeDescription: "Practical medieval food production, preservation, brewing, and workshop equipment.",
			knowledgeLongDescription: "This knowledge covers the shared medieval production chain from grain, fruit, oilseed, meat, fish, salt, water, and fuel through staple food, preserved provisions, ale, wine, and the apparatus needed to make them.");
	}

	private static IReadOnlyList<MedievalFoodCraftSpec> MedievalFoodCraftSpecs()
	{
		return
		[
			FoodToolCraft(
				"forge broad butcher's knife", "Blacksmithing", 20, Difficulty.Normal,
				"medieval_tool_butchers_knife",
				[ProductionItem("medieval_industry_stock_iron_bar"), ProductionItem("medieval_industry_stock_handle_blanks")],
				[ExternalRoomTool("Anvil", HistoricFoundationSource, "Historic Foundation"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"forge kitchen cooking knife", "Blacksmithing", 20, Difficulty.Normal,
				"medieval_tool_cooking_knife",
				[ProductionItem("medieval_industry_stock_iron_bar"), ProductionItem("medieval_industry_stock_handle_blanks")],
				[ExternalRoomTool("Anvil", HistoricFoundationSource, "Historic Foundation"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"build jointed threshing flail", "Carpentry", 10, Difficulty.Easy,
				"medieval_tool_threshing_flail",
				[ProductionItem("medieval_industry_stock_handle_blanks"), ProductionItem("medieval_industry_stock_leather_panel")],
				[ExternalTool("Awl Punch", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"weave broad winnowing basket", "Basketry", 10, Difficulty.Easy,
				"medieval_tool_winnowing_basket",
				[ExternalCommodity(1200.0, "willow", "Basketry Splint", "Agriculture and woodland production")],
				[ExternalTool("Knife", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"raise bronze cooking pot", "Blacksmithing", 20, Difficulty.Normal,
				"medieval_tool_cooking_pot",
				[ProductionItem("medieval_industry_stock_bronze_bar"), ProductionItem("medieval_industry_stock_wire_coil")],
				[ExternalRoomTool("Anvil", HistoricFoundationSource, "Historic Foundation"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"build slatted lauter tun", "Coopering", 25, Difficulty.Hard,
				"medieval_workshop_lauter_tun",
				[ProductionItem("medieval_industry_stock_plank_bundle"), ProductionItem("medieval_industry_stock_rivet_packet")],
				[ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation"), ProductionTool("Wood Auger")]),
			FoodToolCraft(
				"build clay bake oven", "Pottery", 25, Difficulty.Hard,
				"medieval_workshop_bake_oven",
				[ProductionItem("medieval_industry_stock_clay_body_lump"), ProductionItem("medieval_industry_stock_fired_brick_stack")],
				[ExternalTool("Kiln Tool", PrimaryProductionSource, "Primary Production")]),
			FoodToolCraft(
				"raise bronze brew copper", "Blacksmithing", 25, Difficulty.Hard,
				"medieval_workshop_brew_copper",
				[ProductionItem("medieval_industry_stock_bronze_bar"), ProductionItem("medieval_industry_stock_rivet_packet")],
				[ExternalRoomTool("Anvil", HistoricFoundationSource, "Historic Foundation"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"cooper wooden mash tun", "Coopering", 25, Difficulty.Hard,
				"medieval_workshop_mash_tun",
				[ProductionItem("medieval_industry_stock_plank_bundle"), ProductionItem("medieval_industry_stock_rivet_packet")],
				[ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation"), ProductionTool("Wood Auger")]),
			FoodToolCraft(
				"cooper fermenting gyle tun", "Coopering", 25, Difficulty.Hard,
				"medieval_workshop_fermenting_gyle_tun",
				[ProductionItem("medieval_industry_stock_plank_bundle"), ProductionItem("medieval_industry_stock_rivet_packet")],
				[ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation"), ProductionTool("Wood Auger")]),
			FoodToolCraft(
				"frame fine flour sieve", "Carpentry", 20, Difficulty.Normal,
				"medieval_tool_flour_sieve",
				[ProductionItem("medieval_industry_stock_handle_blanks"), ProductionItem("medieval_industry_stock_plain_cloth_bolt")],
				[ExternalTool("Awl Punch", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"hollow wooden kneading trough", "Carpentry", 20, Difficulty.Normal,
				"medieval_tool_kneading_trough",
				[ProductionItem("medieval_industry_stock_plank_bundle")],
				[ProductionTool("Wood Chisel"), ProductionTool("Drawknife")]),
			FoodToolCraft(
				"build oak salting trough", "Carpentry", 20, Difficulty.Normal,
				"medieval_tool_salting_trough",
				[ProductionItem("medieval_industry_stock_plank_bundle"), ProductionItem("medieval_industry_stock_rivet_packet")],
				[ProductionTool("Wood Chisel"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"build smoking and drying rack", "Carpentry", 20, Difficulty.Normal,
				"medieval_tool_smoking_rack",
				[ProductionItem("medieval_industry_stock_plank_bundle"), ProductionItem("medieval_industry_stock_iron_bar")],
				[ProductionTool("Hand Saw"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"build wooden oil press", "Carpentry", 25, Difficulty.Hard,
				"medieval_tool_oil_press",
				[ProductionItem("medieval_industry_stock_plank_bundle"), ProductionItem("medieval_industry_stock_iron_bar"), ProductionItem("medieval_industry_stock_rivet_packet")],
				[ProductionTool("Wood Chisel"), ProductionTool("Wood Auger"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"build wooden fruit press", "Carpentry", 25, Difficulty.Hard,
				"medieval_tool_fruit_press",
				[ProductionItem("medieval_industry_stock_plank_bundle"), ProductionItem("medieval_industry_stock_iron_bar"), ProductionItem("medieval_industry_stock_rivet_packet")],
				[ProductionTool("Wood Chisel"), ProductionTool("Wood Auger"), ExternalTool("Hammer", HistoricFoundationSource, "Historic Foundation")]),
			FoodToolCraft(
				"shape broad mashing paddle", "Carpentry", 20, Difficulty.Normal,
				"medieval_tool_mashing_paddle",
				[ProductionItem("medieval_industry_stock_handle_blanks")],
				[ProductionTool("Drawknife"), ProductionTool("Wood Auger")]),

			FoodProcess(
				"thresh grain into cleaning stock", "Grain Processing", "Threshing", 10, Difficulty.Easy,
				[ExternalTaggedCommodity(2000.0, "Grain Crop", null, "Agriculture")],
				[FoodCraftedTool("Threshing Flail")],
				[CommodityProduct(1700.0, "wheat", "Grain Cleaning Stock"), CommodityProduct(250.0, "wheat", "Bran Commodity")]),
			FoodProcess(
				"winnow threshed grain", "Grain Processing", "Threshing", 10, Difficulty.Easy,
				[FoodTaggedCommodity(1700.0, "Grain Crop", "Grain Cleaning Stock")],
				[FoodCraftedTool("Winnowing Basket")],
				[CommodityProduct(1500.0, "wheat", "Cleaned Grain Commodity")]),
			FoodProcess(
				"mill cleaned grain into flour", "Grain Processing", "Milling", 15, Difficulty.Normal,
				[FoodTaggedCommodity(1350.0, "Grain Crop", "Cleaned Grain Commodity")],
				[ExternalRoomTool("Hand Quern", HistoricFoundationSource, "Historic Foundation"), FoodCraftedTool("Grain Sieve")],
				[CommodityProduct(1100.0, "wheat", "Flour Commodity"), CommodityProduct(150.0, "wheat", "Bran Commodity")]),
			FoodProcess(
				"grind cleaned grain into meal", "Grain Processing", "Milling", 10, Difficulty.Easy,
				[FoodTaggedCommodity(1000.0, "Grain Crop", "Cleaned Grain Commodity")],
				[ExternalRoomTool("Hand Quern", HistoricFoundationSource, "Historic Foundation")],
				[CommodityProduct(900.0, "wheat", "Meal Commodity")]),
			FoodProcess(
				"malt cleaned grain", "Grain Processing", "Brewing", 15, Difficulty.Normal,
				[FoodTaggedCommodity(1200.0, "Grain Crop", "Cleaned Grain Commodity"), ExternalLiquid(1.0, "Water")],
				[FoodCraftedTool("Drying Rack")],
				[CommodityProduct(1050.0, "wheat", "Malted Grain Commodity")]),
			FoodProcess(
				"mix bread dough", "Baking and Pottage", "Cooking", 10, Difficulty.Easy,
				[FoodTaggedCommodity(800.0, "Grain Crop", "Flour Commodity"), ExternalLiquid(0.6, "Water"), ExternalCommodity(15.0, "salt", null, "Primary Production saltworking")],
				[FoodCraftedTool("Kneading Trough")],
				[CommodityProduct(1300.0, "wheat", "Dough Commodity")]),
			FoodProcess(
				"crush oilseed into mash", "Oil and Fruit Pressing", "Milling", 10, Difficulty.Easy,
				[ExternalTaggedCommodity(2000.0, "Oilseed Crop", null, "Agriculture")],
				[ProductionCraftedTool("Mortar and Pestle")],
				[CommodityProduct(1800.0, "olive crop", "Oilseed Mash Commodity")]),
			FoodProcess(
				"press cooking oil", "Oil and Fruit Pressing", "Milling", 15, Difficulty.Normal,
				[FoodTaggedCommodity(1800.0, "Oilseed Crop", "Oilseed Mash Commodity")],
				[FoodCraftedTool("Oil Press")],
				[LiquidProduct("medieval_tableware_oil_amphora", 1.0, "vegetable oil"), CommodityProduct(800.0, "olive crop", "Oilseed Cake Commodity")]),
			FoodProcess(
				"press fruit must", "Oil and Fruit Pressing", "Brewing", 15, Difficulty.Normal,
				[ExternalTaggedCommodity(2000.0, "Fruit Must Crop", null, "Agriculture and orchards")],
				[FoodCraftedTool("Fruit Press")],
				[CommodityProduct(1500.0, "grape", "Fruit Must Commodity")]),
			FoodProcess(
				"mash and lauter grain wort", "Brewing and Winemaking", "Brewing", 20, Difficulty.Normal,
				[FoodTaggedCommodity(1000.0, "Grain Crop", "Malted Grain Commodity"), ExternalLiquid(3.0, "Water")],
				[FoodCraftedTool("Mash Tun"), FoodCraftedTool("Brew Copper"), FoodCraftedTool("Mashing Paddle"), FoodCraftedTool("Lauter Tun")],
				[CommodityProduct(3500.0, "wheat", "Wort Commodity")]),
			FoodProcess(
				"break down raw meat cuts", "Meat Preservation", "Butchering", 10, Difficulty.Easy,
				[ExternalTaggedItem("Raw Non-Fish Meat Cut", "Animal Butchery")],
				[FoodCraftedTool("Butcher's Knife")],
				[CommodityProduct(2000.0, "meat", "Raw Meat Commodity")]),
			FoodProcess(
				"break down raw fish cuts", "Fish Preservation", "Butchering", 10, Difficulty.Easy,
				[ExternalTaggedItem("Raw Fish Cut", "Animal Butchery")],
				[FoodCraftedTool("Butcher's Knife")],
				[CommodityProduct(1000.0, "fish", "Raw Fish Commodity")]),
			FoodProcess(
				"salt raw meat", "Meat Preservation", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(1000.0, "meat", "Raw Meat Commodity"), ExternalCommodity(200.0, "salt", null, "Primary Production saltworking")],
				[FoodCraftedTool("Salting Trough")],
				[CommodityProduct(1000.0, "meat", "Salted Meat Commodity")]),
			FoodProcess(
				"dry salted meat", "Meat Preservation", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(1000.0, "meat", "Salted Meat Commodity")],
				[FoodCraftedTool("Drying Rack")],
				[CommodityProduct(700.0, "meat", "Dried Meat Commodity")]),
			FoodProcess(
				"smoke salted meat", "Meat Preservation", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(1000.0, "meat", "Salted Meat Commodity")],
				[FoodCraftedTool("Smoking Rack"), ExternalRoomTool("Fire", HistoricFoundationSource, "Historic Foundation")],
				[CommodityProduct(800.0, "meat", "Smoked Meat Commodity")]),
			FoodProcess(
				"salt raw fish", "Fish Preservation", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(1000.0, "fish", "Raw Fish Commodity"), ExternalCommodity(200.0, "salt", null, "Primary Production saltworking")],
				[FoodCraftedTool("Salting Trough")],
				[CommodityProduct(1000.0, "fish", "Salted Fish Commodity")]),
			FoodProcess(
				"dry salted fish", "Fish Preservation", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(1000.0, "fish", "Salted Fish Commodity")],
				[FoodCraftedTool("Drying Rack")],
				[CommodityProduct(700.0, "fish", "Dried Fish Commodity")]),
			FoodProcess(
				"smoke salted fish", "Fish Preservation", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(1000.0, "fish", "Salted Fish Commodity")],
				[FoodCraftedTool("Smoking Rack"), ExternalRoomTool("Fire", HistoricFoundationSource, "Historic Foundation")],
				[CommodityProduct(800.0, "fish", "Smoked Fish Commodity")]),

			FinishedFood(
				"bake coarse bread loaf", "Baking and Pottage", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(700.0, "wheat", "Dough Commodity")],
				[FoodCraftedTool("Bake Oven"), ExternalRoomTool("Fire", HistoricFoundationSource, "Historic Foundation")],
				FoodItemProduct("medieval_food_coarse_bread_loaf", new MedievalFoodIngredient(1, "grain"))),
			FinishedFood(
				"bake broad flatbread", "Baking and Pottage", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(320.0, "wheat", "Dough Commodity")],
				[FoodCraftedTool("Bake Oven"), ExternalRoomTool("Fire", HistoricFoundationSource, "Historic Foundation")],
				FoodItemProduct("medieval_food_flatbread", new MedievalFoodIngredient(1, "grain"))),
			FinishedFood(
				"bake hard travel bread", "Baking and Pottage", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(450.0, "wheat", "Dough Commodity")],
				[FoodCraftedTool("Bake Oven"), ExternalRoomTool("Fire", HistoricFoundationSource, "Historic Foundation")],
				FoodItemProduct("medieval_food_hard_bread", new MedievalFoodIngredient(1, "grain"))),
			FinishedFood(
				"cook thick grain pottage", "Baking and Pottage", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(400.0, "wheat", "Meal Commodity"), ExternalLiquid(2.0, "Water")],
				[FoodCraftedTool("Cooking Pot"), ExternalRoomTool("Fire", HistoricFoundationSource, "Historic Foundation")],
				FoodItemProduct("medieval_food_grain_pottage", new MedievalFoodIngredient(1, "grain"))),
			FinishedFood(
				"cook meat and grain pottage", "Baking and Pottage", "Cooking", 15, Difficulty.Normal,
				[FoodCommodity(250.0, "wheat", "Meal Commodity"), FoodCommodity(400.0, "meat", "Raw Meat Commodity"), ExternalLiquid(2.0, "Water")],
				[FoodCraftedTool("Cooking Pot"), ExternalRoomTool("Fire", HistoricFoundationSource, "Historic Foundation")],
				FoodItemProduct("medieval_food_meat_pottage", new MedievalFoodIngredient(1, "grain"), new MedievalFoodIngredient(2, "meat"))),
			FinishedFood(
				"portion salted meat ration", "Meat Preservation", "Cooking", 10, Difficulty.Easy,
				[FoodCommodity(520.0, "meat", "Salted Meat Commodity")],
				[FoodCraftedTool("Cooking Knife")],
				FoodItemProduct("medieval_food_salted_meat_ration", new MedievalFoodIngredient(1, "provision"))),
			FinishedFood(
				"portion dried meat ration", "Meat Preservation", "Cooking", 10, Difficulty.Easy,
				[FoodCommodity(380.0, "meat", "Dried Meat Commodity")],
				[FoodCraftedTool("Cooking Knife")],
				FoodItemProduct("medieval_food_dried_meat_ration", new MedievalFoodIngredient(1, "provision"))),
			FinishedFood(
				"portion smoked meat ration", "Meat Preservation", "Cooking", 10, Difficulty.Easy,
				[FoodCommodity(430.0, "meat", "Smoked Meat Commodity")],
				[FoodCraftedTool("Cooking Knife")],
				FoodItemProduct("medieval_food_smoked_meat_ration", new MedievalFoodIngredient(1, "provision"))),
			FinishedFood(
				"portion salted fish ration", "Fish Preservation", "Cooking", 10, Difficulty.Easy,
				[FoodCommodity(500.0, "fish", "Salted Fish Commodity")],
				[FoodCraftedTool("Cooking Knife")],
				FoodItemProduct("medieval_food_salted_fish_ration", new MedievalFoodIngredient(1, "provision"))),
			FinishedFood(
				"portion dried fish ration", "Fish Preservation", "Cooking", 10, Difficulty.Easy,
				[FoodCommodity(340.0, "fish", "Dried Fish Commodity")],
				[FoodCraftedTool("Cooking Knife")],
				FoodItemProduct("medieval_food_dried_fish_ration", new MedievalFoodIngredient(1, "provision"))),
			FinishedFood(
				"portion smoked fish ration", "Fish Preservation", "Cooking", 10, Difficulty.Easy,
				[FoodCommodity(390.0, "fish", "Smoked Fish Commodity")],
				[FoodCraftedTool("Cooking Knife")],
				FoodItemProduct("medieval_food_smoked_fish_ration", new MedievalFoodIngredient(1, "provision"))),
			FinishedDrink(
				"ferment cask of amber ale", "Brewing", 20, Difficulty.Hard,
				[FoodCommodity(3500.0, "wheat", "Wort Commodity")],
				[FoodCraftedTool("Fermenting Gyle Tun")],
				LiquidProduct("medieval_tableware_table_beer_cask", 3.5, "amber ale")),
			FinishedDrink(
				"ferment cask of red wine", "Brewing", 20, Difficulty.Hard,
				[FoodCommodity(4000.0, "grape", "Fruit Must Commodity")],
				[FoodCraftedTool("Fermenting Gyle Tun")],
				LiquidProduct("medieval_tableware_small_wine_cask", 3.5, "red wine"))
		];
	}

	private static MedievalFoodCraftSpec FoodToolCraft(
		string action,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		string stableReference,
		IReadOnlyList<MedievalFoodInput> inputs,
		IReadOnlyList<MedievalFoodTool> tools)
	{
		return DefineFoodCraft(
			1, action, "Medieval Food / Toolmaking", trait, minimumTraitValue, difficulty, "Food Tools",
			inputs, tools, [SimpleProduct(stableReference)]);
	}

	private static MedievalFoodCraftSpec FoodProcess(
		string action,
		string subtype,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		IReadOnlyList<MedievalFoodInput> inputs,
		IReadOnlyList<MedievalFoodTool> tools,
		IReadOnlyList<MedievalFoodProduct> products)
	{
		return DefineFoodCraft(
			2, action, "Medieval Food / Processing", trait, minimumTraitValue, difficulty, subtype,
			inputs, tools, products);
	}

	private static MedievalFoodCraftSpec FinishedFood(
		string action,
		string subtype,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		IReadOnlyList<MedievalFoodInput> inputs,
		IReadOnlyList<MedievalFoodTool> tools,
		MedievalFoodProduct product)
	{
		return DefineFoodCraft(
			3, action, "Medieval Food / Finished Provisions", trait, minimumTraitValue, difficulty, subtype,
			inputs, tools, [product]);
	}

	private static MedievalFoodCraftSpec FinishedDrink(
		string action,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		IReadOnlyList<MedievalFoodInput> inputs,
		IReadOnlyList<MedievalFoodTool> tools,
		MedievalFoodProduct product)
	{
		return DefineFoodCraft(
			3, action, "Medieval Food / Brewing", trait, minimumTraitValue, difficulty, "Brewing and Winemaking",
			inputs, tools, [product]);
	}

	private static MedievalFoodCraftSpec DefineFoodCraft(
		int phase,
		string action,
		string category,
		string trait,
		int minimumTraitValue,
		Difficulty difficulty,
		string subtype,
		IReadOnlyList<MedievalFoodInput> inputs,
		IReadOnlyList<MedievalFoodTool> tools,
		IReadOnlyList<MedievalFoodProduct> products)
	{
		return new MedievalFoodCraftSpec(
			phase,
			$"medieval food - {action}",
			category,
			trait,
			minimumTraitValue,
			difficulty,
			subtype,
			inputs,
			tools,
			products);
	}

	private static MedievalFoodInput ProductionItem(string stableReference)
	{
		return new MedievalFoodInput(
			string.Empty, stableReference, MedievalCraftedSource, "Medieval Industry Foundations", 0);
	}

	private static MedievalFoodInput ExternalCommodity(
		double grams,
		string material,
		string? pileTag,
		string sourceOwner)
	{
		return new MedievalFoodInput(
			CommodityInput(grams, material, pileTag), null, UpstreamSourceExempt, sourceOwner, 0);
	}

	private static MedievalFoodInput ExternalTaggedCommodity(
		double grams,
		string materialTag,
		string? pileTag,
		string sourceOwner)
	{
		return new MedievalFoodInput(
			TaggedCommodityInput(grams, materialTag, pileTag), null, UpstreamSourceExempt, sourceOwner, 0);
	}

	private static MedievalFoodInput ExternalTaggedItem(string tag, string sourceOwner)
	{
		return new MedievalFoodInput(
			$"Tag - 1x an item with the {tag} tag", null, UpstreamSourceExempt, sourceOwner, 0);
	}

	private static MedievalFoodInput ExternalLiquid(double litres, string liquid)
	{
		return new MedievalFoodInput(
			$"LiquidUse - {FormatLiquidAmount(litres)} of {liquid}", null, UpstreamSourceExempt, "Core liquids", 0);
	}

	private static MedievalFoodInput FoodCommodity(double grams, string material, string pileTag)
	{
		return new MedievalFoodInput(
			CommodityInput(grams, material, pileTag), null, MedievalFoodCraftedSource, MedievalFoodKnowledge, 2);
	}

	private static MedievalFoodInput FoodTaggedCommodity(double grams, string materialTag, string pileTag)
	{
		return new MedievalFoodInput(
			TaggedCommodityInput(grams, materialTag, pileTag), null, MedievalFoodCraftedSource, MedievalFoodKnowledge, 2);
	}

	private static string TaggedCommodityInput(double grams, string materialTag, string? pileTag)
	{
		return $"CommodityTag - {FormatCommodityAmount(grams)} of a material tagged as {materialTag}" +
		       (string.IsNullOrWhiteSpace(pileTag) ? string.Empty : $"; piletag {pileTag}");
	}

	private static MedievalFoodTool ExternalTool(string tag, string sourceStatus, string sourceOwner)
	{
		return new MedievalFoodTool(tag, "Held", sourceStatus, sourceOwner, 0);
	}

	private static MedievalFoodTool ExternalRoomTool(string tag, string sourceStatus, string sourceOwner)
	{
		return new MedievalFoodTool(tag, "InRoom", sourceStatus, sourceOwner, 0);
	}

	private static MedievalFoodTool ProductionTool(string tag)
	{
		return new MedievalFoodTool(tag, "Held", MedievalCraftedSource, "Medieval Industry Foundations", 0);
	}

	private static MedievalFoodTool ProductionCraftedTool(string tag)
	{
		return new MedievalFoodTool(tag, "Held", MedievalCraftedSource, "Medieval Industry Foundations", 0);
	}

	private static MedievalFoodTool FoodCraftedTool(string tag)
	{
		var location = tag is
			"Threshing Flail" or
			"Winnowing Basket" or
			"Grain Sieve" or
			"Mashing Paddle" or
			"Butcher's Knife" or
			"Cooking Knife"
				? "Held"
				: "InRoom";
		return new MedievalFoodTool(tag, location, MedievalFoodCraftedSource, MedievalFoodKnowledge, 1);
	}

	private static MedievalFoodProduct SimpleProduct(string stableReference)
	{
		return new MedievalFoodProduct("simple", stableReference, null, null, 1.0, null, []);
	}

	private static MedievalFoodProduct CommodityProduct(double grams, string material, string pileTag)
	{
		return new MedievalFoodProduct("commodity", null, material, pileTag, grams, null, []);
	}

	private static MedievalFoodProduct FoodItemProduct(
		string stableReference,
		params MedievalFoodIngredient[] ingredients)
	{
		return new MedievalFoodProduct("food", stableReference, null, null, 1.0, null, ingredients);
	}

	private static MedievalFoodProduct LiquidProduct(string stableReference, double litres, string liquid)
	{
		return new MedievalFoodProduct("liquid", stableReference, null, null, litres, liquid, []);
	}

	private string ProductImport(MedievalFoodProduct product)
	{
		return product.Kind switch
		{
			"simple" => StableSimpleProduct(product.StableReference!),
			"commodity" =>
				$"CommodityProduct - {FormatCommodityAmount(product.Amount)} of {product.Material} commodity; tag {product.PileTag}",
			"food" =>
				$"CookedFoodProduct - 1x {StableFoodProductItemDescription(product.StableReference!)}" +
				string.Concat(product.Ingredients.Select(x => $"; ingredient $i{x.InputIndex} = {x.Role}")),
			"liquid" =>
				$"LiquidProduct - 1x {StableFoodProductItemDescription(product.StableReference!)} filled with {FormatLiquidAmount(product.Amount)} of {product.Liquid}",
			_ => throw new ApplicationException($"Unknown Medieval food product kind {product.Kind}")
		};
	}

	private string StableFoodProductItemDescription(string stableReference)
	{
		var item = LookupReworkItem(stableReference);
		return $"{item.ShortDescription} (#{item.Id})";
	}

	private static string ProductContractForTesting(MedievalFoodProduct product)
	{
		return product.Kind switch
		{
			"simple" or "food" => product.StableReference!,
			"commodity" => $"commodity:{product.PileTag}",
			"liquid" => $"liquid:{product.StableReference}:{product.Liquid}:{product.Amount:0.###}",
			_ => product.Kind
		};
	}

	private static string FoodToolImport(MedievalFoodTool tool)
	{
		return $"TagTool - {tool.Location} - an item with the {tool.Tag} tag";
	}

	private static string FormatLiquidAmount(double litres)
	{
		if (litres >= 1.0)
		{
			return $"{litres:0.###} litre{(Math.Abs(litres - 1.0) < 0.0001 ? string.Empty : "s")}";
		}

		var millilitres = litres * 1000.0;
		return $"{millilitres:0.###} millilitre{(Math.Abs(millilitres - 1.0) < 0.0001 ? string.Empty : "s")}";
	}

	private static (int Seconds, string Echo, string FailEcho)[] MedievalFoodCraftingPhases(int phase)
	{
		if (phase == 1)
		{
			return
			[
				(30, "$0 inspect|inspects the stock and lay|lays out the foodmaking equipment pieces.", "$0 overlook|overlooks faults in the equipment stock."),
				(45, "$0 shape|shapes and fit|fits the pieces together with $t1.", "$0 shape|shapes the pieces unevenly and struggle|struggles with the fit."),
				(45, "$0 finish|finishes the equipment and set|sets aside $p1.", "$0 botch|botches the finishing work and spoil|spoils the assembly.")
			];
		}

		return
		[
			(30, "$0 inspect|inspects $i1 and prepare|prepares the work with $t1.", "$0 misjudge|misjudges the condition of $i1."),
			(45, "$0 work|works the ingredients steadily through the main preparation.", "$0 handle|handles the ingredients poorly and lose|loses control of the process."),
			(45, "$0 finish|finishes the preparation and set|sets aside $p1.", "$0 spoil|spoils the final preparation and have|has nothing useful to show for it.")
		];
	}
}
