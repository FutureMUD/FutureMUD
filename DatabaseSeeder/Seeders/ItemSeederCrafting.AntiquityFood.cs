#nullable enable

using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedAntiquityFoodCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		SeedAntiquityFoodProcessingCrafts();
		SeedAntiquityButcheryFoodCrafts();
		SeedAntiquityBeverageCrafts();
		SeedAntiquityCultureFoodCrafts();
	}

	private void SeedAntiquityFoodProcessingCrafts()
	{
		var cooking = _traits["Cooking"] ?? _traits["Cook"] ?? _traits["Surviving"] ?? _traits["Survival"] ?? _traits.First().Value;
		var farming = _traits["Farming"] ?? _traits["Agriculture"] ?? cooking;
		var threshing = _traits["Threshing"] ?? _traits["Thresher"] ?? farming;
		var milling = _traits["Milling"] ?? _traits["Miller"] ?? cooking;
		var brewing = _traits["Brewing"] ?? _traits["Brewer"] ?? cooking;

		AddCraft("thresh grain into heads", "Food Processing", "thresh raw grain heads", "threshing grain",
			"a grain threshing task", "HasThreshing", null, null, null, threshing, Difficulty.Easy, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 beat|beats the grain with $t1, loosening kernels from stalk and husk.",
				"$0 gather|gathers the threshed grain into a useful pile."),
			["CommodityTag - 2 kilograms of a material tagged as Grain Crop"],
			["TagTool - Held - an item with the Threshing Flail tag"],
			[
				$"CommodityProduct - {FormatCommodityAmount(1700.0)} of wheat commodity; tag Grain Cleaning Stock",
				$"CommodityProduct - {FormatCommodityAmount(250.0)} of wheat commodity; tag Bran Commodity"
			],
			[],
			[(1, 1), (2, 1)]);

		AddCraft("winnow threshed grain", "Food Processing", "winnow grain from chaff", "winnowing grain",
			"a grain winnowing task", "HasThreshing", null, null, null, threshing, Difficulty.Easy, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 toss|tosses $i1 in $t1, letting dust and chaff fall away.",
				"$0 collect|collects the cleaned grain."),
			["CommodityTag - 1 kilogram 500 grams of a material tagged as Grain Crop; piletag Grain Cleaning Stock"],
			["TagTool - Held - an item with the Winnowing Basket tag"],
			[$"CommodityProduct - {FormatCommodityAmount(1350.0)} of wheat commodity; tag Cleaned Grain Commodity"],
			[],
			[(1, 1)]);

		AddCraft("mill cleaned grain into flour", "Food Processing", "mill cleaned grain into flour", "milling grain",
			"a flour milling task", "HasMilling", null, null, null, milling, Difficulty.Normal, Outcome.MinorFail, 5, 3,
			false,
			SimpleFoodPhases("$0 feed|feeds $i1 into $t1 and grind|grinds it between stones.",
				"$0 sift|sifts the meal into flour and bran."),
			["CommodityTag - 1 kilogram of a material tagged as Grain Crop; piletag Cleaned Grain Commodity"],
			["TagTool - InRoom - an item with the Hand Quern tag", "TagTool - Held - an item with the Grain Sieve tag"],
			[
				$"CommodityProduct - {FormatCommodityAmount(800.0)} of wheat commodity; tag Flour Commodity",
				$"CommodityProduct - {FormatCommodityAmount(150.0)} of wheat commodity; tag Bran Commodity"
			],
			[],
			[(1, 1), (2, 1)]);

		AddCraft("grind cleaned grain into meal", "Food Processing", "grind grain into coarse meal", "grinding grain",
			"a grain meal grinding task", "HasMilling", null, null, null, milling, Difficulty.Easy, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 crush|crushes $i1 with $t1 into a coarse meal.",
				"$0 gather|gathers the grain meal."),
			["CommodityTag - 1 kilogram of a material tagged as Grain Crop; piletag Cleaned Grain Commodity"],
			["TagTool - InRoom - an item with the Hand Quern tag"],
			[$"CommodityProduct - {FormatCommodityAmount(900.0)} of wheat commodity; tag Meal Commodity"],
			[],
			[(1, 1)]);

		AddCraft("split and grind pulses", "Food Processing", "split and grind dry pulses", "grinding pulses",
			"a pulse grinding task", "HasMilling", null, null, null, milling, Difficulty.Easy, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 crack|cracks $i1 in $t1 and grind|grinds the pulses into meal.",
				"$0 gather|gathers the pulse meal."),
			["CommodityTag - 1 kilogram of a material tagged as Pulse Crop"],
			["TagTool - Held - an item with the Mortar and Pestle tag"],
			[$"CommodityProduct - {FormatCommodityAmount(850.0)} of lentil commodity; tag Pulse Meal Commodity"],
			[],
			[(1, 1)]);

		AddCraft("chop vegetables for cooking", "Food Processing", "chop aromatic vegetables", "chopping vegetables",
			"a vegetable preparation task", "HasCooking", null, null, null, cooking, Difficulty.Easy, Outcome.MinorFail, 5,
			2, false,
			SimpleFoodPhases("$0 chop|chops $i1 with $t1 into small cooking pieces.",
				"$0 gather|gathers the chopped vegetables."),
			["CommodityTag - 1 kilogram of a material tagged as Vegetable Prep Crop"],
			["TagTool - Held - an item with the Cooking Knife tag"],
			[$"CommodityProduct - {FormatCommodityAmount(900.0)} of onion commodity; tag Vegetable Prep Commodity"],
			[],
			[(1, 1)]);

		AddCraft("press fruit must", "Food Processing", "press fruit into must", "pressing fruit must",
			"a fruit pressing task", "HasBrewing", null, null, null, brewing, Difficulty.Normal, Outcome.MinorFail, 5, 3,
			false,
			SimpleFoodPhases("$0 crush|crushes $i1 and load|loads it into $t1.",
				"$0 press|presses the fruit down into a wet must."),
			["CommodityTag - 2 kilograms of a material tagged as Fruit Must Crop"],
			["TagTool - InRoom - an item with the Fruit Press tag"],
			[$"CommodityProduct - {FormatCommodityAmount(1500.0)} of grape commodity; tag Fruit Must Commodity"],
			[],
			[(1, 1)]);

		AddCraft("crush oilseeds for pressing", "Food Processing", "crush oilseed crop into pressable mash",
			"crushing oilseeds", "an oilseed crushing task", "HasMilling", null, null, null, milling, Difficulty.Easy,
			Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 crush|crushes $i1 with $t1 into a damp mash.",
				"$0 gather|gathers the oilseed mash for pressing."),
			["CommodityTag - 2 kilograms of a material tagged as Oilseed Crop"],
			["TagTool - Held - an item with the Mortar and Pestle tag"],
			[$"CommodityProduct - {FormatCommodityAmount(1800.0)} of olive crop commodity; tag Oilseed Mash Commodity"],
			[],
			[(1, 1)]);

		AddCraft("press vegetable oil from oilseed mash", "Food Processing", "press edible oil from oilseed mash",
			"pressing vegetable oil", "an oil pressing task", "HasMilling", null, null, null, milling,
			Difficulty.Normal, Outcome.MinorFail, 5, 3,
			false,
			SimpleFoodPhases("$0 load|loads $i1 into $t1 and begin|begins applying pressure.",
				"$0 draw|draws off the oil into $p1 and set|sets aside the press cake."),
			["CommodityTag - 1 kilogram 800 grams of a material tagged as Oilseed Crop; piletag Oilseed Mash Commodity"],
			["TagTool - InRoom - an item with the Oil Press tag"],
			[
				$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 1 litre of vegetable oil",
				$"CommodityProduct - {FormatCommodityAmount(900.0)} of olive crop commodity; tag Oilseed Cake Commodity"
			],
			[],
			[(2, 1)]);

		AddCraft("press olive oil from olive mash", "Food Processing", "press olive oil from crushed olive mash",
			"pressing olive oil", "an olive oil pressing task", "HasMilling", null, null, null, milling,
			Difficulty.Normal, Outcome.MinorFail, 5, 3,
			false,
			SimpleFoodPhases("$0 load|loads $i1 into $t1 and begin|begins applying pressure.",
				"$0 draw|draws off the olive oil into $p1 and set|sets aside the olive press cake."),
			["Commodity - 1 kilogram 800 grams of olive crop; piletag Oilseed Mash Commodity"],
			["TagTool - InRoom - an item with the Oil Press tag"],
			[
				$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 1 litre of olive oil",
				$"CommodityProduct - {FormatCommodityAmount(900.0)} of olive crop commodity; tag Oilseed Cake Commodity"
			],
			[],
			[(2, 1)]);

		AddCraft("prepare fruit for eating", "Food Processing", "prepare edible fruit from commodity stock",
			"preparing fruit", "a fruit preparation task", "HasCooking", null, null, null, cooking, Difficulty.Easy,
			Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 cut|cuts, seed|seeds and arrange|arranges $i1 with $t1.",
				"$0 set|sets aside $p1, ready to eat."),
			["CommodityTag - 500 grams of a material tagged as Ready Fruit Crop"],
			["TagTool - Held - an item with the Cooking Knife tag"],
			[$"CookedFoodProduct - 1x {_items["antiquity_food_prepared_fruit"].ShortDescription} (#{_items["antiquity_food_prepared_fruit"].Id}); ingredient $i1 = fruit"],
			[]);

		AddCraft("brine bitter fruit stock", "Food Processing", "brine bitter fruit for later eating",
			"brining bitter fruit", "a fruit brining task", "HasCooking", null, null, null, cooking, Difficulty.Easy,
			Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 pack|packs $i1 in salt and water inside $t1.",
				"$0 set|sets aside the brined fruit stock."),
			[
				"CommodityTag - 500 grams of a material tagged as Fruit Brining Crop",
				"LiquidUse - 1 litre of Water",
				"Commodity - 100 grams of salt"
			],
			["TagTool - InRoom - an item with the Salting Trough tag"],
			[$"CommodityProduct - {FormatCommodityAmount(500.0)} of olive crop commodity; tag Brined Fruit Commodity"],
			[],
			[(1, 1)]);

		AddCraft("serve brined fruit", "Food Processing", "prepare brined fruit as an edible serving",
			"serving brined fruit", "a brined fruit serving task", "HasCooking", null, null, null, cooking,
			Difficulty.Easy, Outcome.MinorFail, 5, 1, false,
			SimpleFoodPhases("$0 rinse|rinses and arrange|arranges $i1 with $t1.",
				"$0 set|sets aside $p1, ready to eat."),
			["CommodityTag - 250 grams of a material tagged as Fruit Brining Crop; piletag Brined Fruit Commodity"],
			["TagTool - Held - an item with the Cooking Knife tag"],
			[$"CookedFoodProduct - 1x {_items["antiquity_food_brined_fruit"].ShortDescription} (#{_items["antiquity_food_brined_fruit"].Id}); ingredient $i1 = fruit"],
			[]);
	}

	private void SeedAntiquityButcheryFoodCrafts()
	{
		var cooking = _traits["Cooking"] ?? _traits["Cook"] ?? _traits["Butchering"] ?? _traits["Butcher"] ?? _traits.First().Value;
		var butchery = _traits["Butchering"] ?? _traits["Butcher"] ?? cooking;

		AddCraft("break down raw meat cuts", "Food Processing", "break raw carcass cuts into meat commodity",
			"breaking down meat cuts", "a raw meat breakdown task", "HasButchering", null, null, null, butchery,
			Difficulty.Easy, Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 trim|trims $i1 with $t1 into usable pieces of raw meat.",
				"$0 gather|gathers the raw meat commodity."),
			["Tag - 1x an item with the Raw Meat Cut tag"],
			["TagTool - Held - an item with the Butcher's Knife tag"],
			[$"CommodityProduct - {FormatCommodityAmount(2500.0)} of meat commodity; tag Raw Meat Commodity"],
			[]);

		AddCraft("break down raw offal", "Food Processing", "break raw offal into cooking commodity",
			"breaking down offal", "an offal preparation task", "HasButchering", null, null, null, butchery,
			Difficulty.Easy, Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 sort|sorts and trim|trims $i1 with $t1.",
				"$0 gather|gathers the offal as a raw meat commodity."),
			["Tag - 1x an item with the Offal tag"],
			["TagTool - Held - an item with the Butcher's Knife tag"],
			[$"CommodityProduct - {FormatCommodityAmount(1200.0)} of meat commodity; tag Raw Meat Commodity"],
			[]);

		AddCraft("render animal fat", "Food Processing", "render animal fat", "rendering animal fat",
			"a fat rendering task", "HasCooking", null, null, null, cooking, Difficulty.Normal, Outcome.MinorFail, 5, 3,
			false,
			SimpleFoodPhases("$0 heat|heats $i1 slowly in $t1 over $t2.",
				"$0 skim|skims the rendered fat away from the solids."),
			["CommodityTag - 1 kilogram of a material tagged as Meat; piletag Raw Meat Commodity"],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			[$"CommodityProduct - {FormatCommodityAmount(500.0)} of meat commodity; tag Rendered Fat Commodity"],
			[]);

		AddCraft("cook raw meat commodity", "Food Processing", "cook raw meat commodity for recipes", "cooking meat",
			"a meat preparation task", "HasCooking", null, null, null, cooking, Difficulty.Easy, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 cook|cooks $i1 over $t1 until it is firm and safe to use.",
				"$0 set|sets aside the prepared meat."),
			["CommodityTag - 1 kilogram of a material tagged as Meat; piletag Raw Meat Commodity"],
			["TagTool - InRoom - an item with the Fire tag"],
			[$"CommodityProduct - {FormatCommodityAmount(900.0)} of meat commodity; tag Prepared Meat Commodity"],
			[],
			[(1, 1)]);

		AddCraft("salt raw meat", "Food Processing", "salt raw meat for storage", "salting meat",
			"a meat salting task", "HasCooking", null, null, null, cooking, Difficulty.Easy, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 pack|packs $i1 in $i2 inside $t1.",
				"$0 set|sets aside the salted meat."),
			[
				"CommodityTag - 1 kilogram of a material tagged as Meat; piletag Raw Meat Commodity",
				"Commodity - 200 grams of salt"
			],
			["TagTool - InRoom - an item with the Salting Trough tag"],
			[$"CommodityProduct - {FormatCommodityAmount(1000.0)} of meat commodity; tag Salted Meat Commodity"],
			[],
			[(1, 1)]);

		AddCraft("dry prepared meat", "Food Processing", "dry meat for storage", "drying meat",
			"a meat drying task", "HasCooking", null, null, null, cooking, Difficulty.Easy, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 lay|lays $i1 on $t1 to dry in thin pieces.",
				"$0 collect|collects the dried meat."),
			["CommodityTag - 1 kilogram of a material tagged as Meat; piletag Salted Meat Commodity"],
			["TagTool - InRoom - an item with the Drying Rack tag"],
			[$"CommodityProduct - {FormatCommodityAmount(700.0)} of meat commodity; tag Dried Meat Commodity"],
			[],
			[(1, 1)]);

		AddCraft("smoke prepared meat", "Food Processing", "smoke meat for storage", "smoking meat",
			"a meat smoking task", "HasCooking", null, null, null, cooking, Difficulty.Normal, Outcome.MinorFail, 5, 3,
			false,
			SimpleFoodPhases("$0 hang|hangs $i1 from $t1 over the smoke.",
				"$0 gather|gathers the smoked meat."),
			["CommodityTag - 1 kilogram of a material tagged as Meat; piletag Salted Meat Commodity"],
			["TagTool - InRoom - an item with the Smoking Rack tag", "TagTool - InRoom - an item with the Fire tag"],
			[$"CommodityProduct - {FormatCommodityAmount(800.0)} of meat commodity; tag Smoked Meat Commodity"],
			[],
			[(1, 1)]);

		AddCraft("boil meat broth", "Cooking", "boil meat and bones into broth", "boiling meat broth",
			"a broth boiling task", "HasCooking", null, null, null, cooking, Difficulty.Easy, Outcome.MinorFail, 5, 3,
			false,
			SimpleFoodPhases("$0 simmer|simmers $i1 in $i2 inside $t1.",
				"$0 strain|strains the savoury broth into $p1."),
			[
				"CommodityTag - 500 grams of a material tagged as Meat; piletag Raw Meat Commodity",
				"LiquidUse - 3 litres of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			[$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 3 litres of meat broth"],
			[]);
	}

	private void SeedAntiquityBeverageCrafts()
	{
		var cooking = _traits["Cooking"] ?? _traits["Cook"] ?? _traits.First().Value;
		var brewing = _traits["Brewing"] ?? _traits["Brewer"] ?? cooking;

		AddCraft("mash grain wort", "Brewing", "mash grain into wort", "mashing grain wort",
			"a mashing task", "HasBrewing", null, null, null, brewing, Difficulty.Normal, Outcome.MinorFail, 5, 3, false,
			SimpleFoodPhases("$0 stir|stirs $i1 into hot water in $t1.",
				"$0 draw|draws off a sweet grain wort."),
			[
				"CommodityTag - 1 kilogram of a material tagged as Grain Crop; piletag Cleaned Grain Commodity",
				"LiquidUse - 3 litres of Water"
			],
			["TagTool - InRoom - an item with the Mash Tun tag"],
			[$"CommodityProduct - {FormatCommodityAmount(2500.0)} of barley commodity; tag Wort Commodity"],
			[],
			[(1, 1)]);

		AddCraft("fill beer fermenting amphora", "Brewing", "fill a beer fermenting amphora", "filling a beer fermenting amphora",
			"a beer fermentation task", "HasBrewing", null, null, null, brewing, Difficulty.Normal, Outcome.MinorFail, 5, 2,
			false,
			SimpleFoodPhases("$0 pour|pours $i1 into $t1 and seal|seals it.",
				"$0 set|sets aside $p1 to ferment."),
			["CommodityTag - 2 kilograms of a material tagged as Grain Crop; piletag Wort Commodity"],
			["TagTool - Held - an item with the Fermentation Amphora tag"],
			[$"SimpleProduct - 1x {_items["antiquity_food_fermenting_beer_amphora"].ShortDescription} (#{_items["antiquity_food_fermenting_beer_amphora"].Id})"],
			[]);

		AddCraft("fill amphora with barley beer", "Brewing", "fill a serving amphora with barley beer",
			"filling an amphora with barley beer", "a beer filling task", "HasBrewing", null, null, null, brewing,
			Difficulty.Easy, Outcome.MinorFail, 5, 1, false,
			SimpleFoodPhases("$0 strain|strains beer into $p1.",
				"$0 seal|seals the beer amphora."),
			["CommodityTag - 2 kilograms of a material tagged as Grain Crop; piletag Wort Commodity"],
			[],
			[$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 3 litres of barley beer"],
			[]);

		AddCraft("fill amphora with date beer", "Brewing", "fill a serving amphora with date beer",
			"filling an amphora with date beer", "a date beer filling task", "HasBrewing", null, null, null, brewing,
			Difficulty.Easy, Outcome.MinorFail, 5, 1, false,
			SimpleFoodPhases("$0 strain|strains date beer into $p1.",
				"$0 seal|seals the date beer amphora."),
			[
				"CommodityTag - 1 kilogram of a material tagged as Grain Crop; piletag Wort Commodity",
				"Commodity - 500 grams of date"
			],
			[],
			[$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 3 litres of date beer"],
			[]);

		AddCraft("fill amphora with fish sauce", "Food Processing", "fill an amphora with fermented fish sauce",
			"filling an amphora with fish sauce", "a fish sauce filling task", "HasCooking", null, null, null, cooking,
			Difficulty.Normal, Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 pack|packs fish and salt down for fermentation.",
				"$0 strain|strains the sauce into $p1."),
			[
				"CommodityTag - 1 kilogram of a material tagged as Meat; piletag Raw Meat Commodity",
				"Commodity - 300 grams of salt"
			],
			[],
			[$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 3 litres of garum sauce"],
			[]);
	}

	private void SeedAntiquityCultureFoodCrafts()
	{
		foreach (var culture in AntiquityFoodCultures)
		{
			SeedAntiquityCultureFoodCrafts(culture);
		}
	}

	private void SeedAntiquityCultureFoodCrafts(AntiquityFoodCultureSpec culture)
	{
		var cooking = _traits["Cooking"] ?? _traits["Cook"] ?? _traits.First().Value;
		var baking = _traits["Baking"] ?? _traits["Baker"] ?? cooking;
		var brewing = _traits["Brewing"] ?? _traits["Brewer"] ?? cooking;
		var key = culture.Key;

		AddCultureFoodCraft(culture, $"bake {culture.Display.ToLowerInvariant()} flatbread", "bake a simple flatbread",
			"baking flatbread", "a flatbread baking task",
			[
				$"Commodity - {FormatCommodityAmount(500.0)} of {culture.GrainMaterial}; piletag Flour Commodity",
				"LiquidUse - 250 millilitres of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			CookedProduct($"antiquity_food_{key}_flatbread", "ingredient $i1 = grain, $i2 = water"),
			baking, [(1, 1)]);

		AddCultureFoodCraft(culture, $"cook {culture.Display.ToLowerInvariant()} grain porridge", "cook grain porridge",
			"cooking grain porridge", "a porridge cooking task",
			[
				$"Commodity - {FormatCommodityAmount(600.0)} of {culture.GrainMaterial}; piletag Meal Commodity",
				"LiquidUse - 1 litre of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			CookedProduct($"antiquity_food_{key}_porridge", "ingredient $i1 = grain, $i2 = water"),
			cooking, [(1, 1)]);

		AddCultureFoodCraft(culture, $"cook {culture.Display.ToLowerInvariant()} pulse stew", "cook pulse stew",
			"cooking pulse stew", "a pulse stew cooking task",
			[
				$"Commodity - {FormatCommodityAmount(500.0)} of {culture.PulseMaterial}; piletag Pulse Meal Commodity",
				"CommodityTag - 250 grams of a material tagged as Vegetable Prep Crop; piletag Vegetable Prep Commodity",
				"LiquidUse - 1 litre of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			CookedProduct($"antiquity_food_{key}_pulse_stew", "ingredient $i1 = pulse, $i2 = vegetable, $i3 = broth"),
			cooking, [(1, 1)]);

		AddCultureFoodCraft(culture, $"cook {culture.Display.ToLowerInvariant()} meat grain dish", "cook a meat and grain dish",
			"cooking a meat and grain dish", "a meat dish cooking task",
			[
				"CommodityTag - 500 grams of a material tagged as Meat; piletag Prepared Meat Commodity",
				$"Commodity - {FormatCommodityAmount(400.0)} of {culture.GrainMaterial}; piletag Meal Commodity",
				"LiquidUse - 500 millilitres of meat broth"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			CookedProduct($"antiquity_food_{key}_meat_dish", "ingredient $i1 = meat, $i2 = grain, $i3 = broth"),
			cooking);

		AddCultureFoodCraft(culture, $"pack {culture.Display.ToLowerInvariant()} preserved meat ration", "pack preserved meat ration",
			"packing preserved meat", "a preserved ration packing task",
			[
				"CommodityTag - 400 grams of a material tagged as Meat; piletag Dried Meat Commodity",
				$"Commodity - {FormatCommodityAmount(250.0)} of {culture.GrainMaterial}; piletag Bran Commodity"
			],
			[],
			CookedProduct($"antiquity_food_{key}_preserved_meat", "ingredient $i1 = meat, $i2 = grain"),
			cooking);

		AddCultureFoodCraft(culture, $"make {culture.Display.ToLowerInvariant()} fruit sweet", "make a fruit and grain sweet",
			"making a fruit sweet", "a fruit sweet preparation task",
			[
				$"Commodity - {FormatCommodityAmount(400.0)} of {culture.SweetMaterial}; piletag Fruit Must Commodity",
				$"Commodity - {FormatCommodityAmount(250.0)} of {culture.GrainMaterial}; piletag Flour Commodity"
			],
			["TagTool - Held - an item with the Mortar and Pestle tag"],
			CookedProduct($"antiquity_food_{key}_sweet", "ingredient $i1 = fruit, $i2 = grain"),
			cooking, [(1, 1)]);

		AddCultureFoodCraft(culture, $"fill {culture.Display.ToLowerInvariant()} beverage amphora", "fill a staple beverage amphora",
			"filling a beverage amphora", "a beverage filling task",
			[
				culture.BeverageLiquid.Contains("wine", StringComparison.OrdinalIgnoreCase)
					? $"Commodity - {FormatCommodityAmount(1200.0)} of {culture.SweetMaterial}; piletag Fruit Must Commodity"
					: "CommodityTag - 1 kilogram of a material tagged as Grain Crop; piletag Wort Commodity"
			],
			[],
			$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 3 litres of {culture.BeverageLiquid}",
			brewing);
	}

	private void AddCultureFoodCraft(AntiquityFoodCultureSpec culture, string name, string blurb, string action,
		string itemSdesc, IEnumerable<string> inputs, IEnumerable<string> tools, string product,
		MudSharp.Models.TraitDefinition trait, List<(int Product, int Input)>? materialInputs = null)
	{
		AddCraft(name, "Cooking", blurb, action, itemSdesc, culture.Knowledge, trait.Name, null, Difficulty.Normal,
			Outcome.MinorFail, 5, 3, false,
			SimpleFoodPhases("$0 prepare|prepares $i1 and the other ingredients.",
				"$0 finish|finishes the dish and set|sets aside $p1."),
			inputs,
			tools,
			[product],
			[],
			materialInputs,
			null,
			knowledgeType: "Crafting",
			knowledgeSubtype: "Foodways",
			knowledgeDescription: $"Knowledge of {culture.Display} foodways.",
			knowledgeLongDescription: $"This knowledge covers staple, preserved, sweet and beverage preparations associated with {culture.Display} foodways.");
	}

	private string CookedProduct(string stableReference, string ingredientOptions)
	{
		var item = _items[stableReference];
		return $"CookedFoodProduct - 1x {item.ShortDescription} (#{item.Id}); {ingredientOptions}";
	}

	private static IEnumerable<(int Seconds, string Echo, string FailEcho)> SimpleFoodPhases(string first, string second)
	{
		return
		[
			(20, first, first),
			(25, second, second)
		];
	}
}
