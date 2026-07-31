#nullable enable

using MudSharp.FutureProg;
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

		SeedAntiquityFoodVesselCrafts();
		SeedAntiquityFoodProcessingCrafts();
		SeedAntiquityButcheryFoodCrafts();
		SeedAntiquityBeverageCrafts();
		SeedAntiquityCultureFoodCrafts();
	}

	private void SeedAntiquityFoodVesselCrafts()
	{
		AddAntiquityCraft(
			"finish clay serving amphora",
			"Pottery",
			"finish a sealed clay serving amphora",
			"finishing a clay serving amphora",
			"a clay serving amphora being finished",
			AncientCeramicVesselmakingKnowledge,
			"Pottery",
			20,
			Difficulty.Normal,
			SimpleFoodPhases("$0 turn|turns $i1 on $t1 and smooth|smooths the shoulders with $t2.",
				"$0 fire|fires and seal|seals $p1 in $t3 with prepared pitch."),
			[
				CommodityInput(900.0, "fired clay", "Bisque Vessel Blank"),
				CommodityInput(80.0, "pitch", "Prepared Pitch")
			],
			[
				"TagTool - InRoom - an item with the Potter's Wheel tag",
				"TagTool - Held - an item with the Potter's Rib tag",
				"TagTool - InRoom - an item with the Lit Kiln tag"
			],
			[StableSimpleProduct("antiquity_food_serving_amphora")],
			knowledgeSubtype: "Ceramics",
			knowledgeDescription: "Ancient ceramic vesselmaking for domestic and food-storage vessels.",
			knowledgeLongDescription: "This knowledge covers shaping, firing, lining, and finishing ancient ceramic vessels used for household and food-production workflows.");

		AddAntiquityCraft(
			"line pitch fermenting amphora",
			"Pottery",
			"finish a pitch-lined fermenting amphora",
			"lining a fermenting amphora",
			"a fermenting amphora being lined with pitch",
			AncientCeramicVesselmakingKnowledge,
			"Pottery",
			25,
			Difficulty.Normal,
			SimpleFoodPhases("$0 shape|shapes the mouth and belly of $i1 on $t1, smoothing it with $t2.",
				"$0 warm|warms prepared pitch and line|lines $p1 before setting it in $t3."),
			[
				CommodityInput(900.0, "fired clay", "Bisque Vessel Blank"),
				CommodityInput(240.0, "pitch", "Prepared Pitch")
			],
			[
				"TagTool - InRoom - an item with the Potter's Wheel tag",
				"TagTool - Held - an item with the Potter's Rib tag",
				"TagTool - InRoom - an item with the Lit Kiln tag"
			],
			[StableSimpleProduct("antiquity_food_fermenting_amphora")],
			knowledgeSubtype: "Ceramics",
			knowledgeDescription: "Ancient ceramic vesselmaking for domestic and food-storage vessels.",
			knowledgeLongDescription: "This knowledge covers shaping, firing, lining, and finishing ancient ceramic vessels used for household and food-production workflows.");
	}

	private void SeedAntiquityFoodProcessingCrafts()
	{
		var cooking = _traits["Cooking"] ?? _traits["Cook"] ?? _traits["Surviving"] ?? _traits["Survival"] ?? _traits.First().Value;
		var farming = _traits["Farming"] ?? cooking;
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
		var cooking = _traits["Cooking"] ?? _traits["Cook"] ?? _traits["Butchering"] ?? _traits["Butchery"] ?? _traits.First().Value;
		var butchery = _traits["Butchering"] ?? _traits["Butchery"] ?? cooking;

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
			[StableSimpleProduct("antiquity_food_fermenting_beer_amphora")],
			[]);

		AddCraft("fill date beer fermenting amphora", "Brewing", "fill a date beer fermenting amphora",
			"filling a date beer fermenting amphora", "a date beer fermentation task", "HasBrewing", null, null, null,
			brewing, Difficulty.Normal, Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 pour|pours $i1 and $i2 into $t1 and seal|seals it.",
				"$0 set|sets aside $p1 to ferment."),
			[
				"CommodityTag - 1 kilogram of a material tagged as Grain Crop; piletag Wort Commodity",
				"Commodity - 500 grams of date"
			],
			["TagTool - Held - an item with the Fermentation Amphora tag"],
			[StableSimpleProduct("antiquity_food_fermenting_date_beer_amphora")],
			[]);

		AddCraft("fill amphora with fish sauce", "Food Processing", "fill an amphora with fermented fish sauce",
			"filling an amphora with fish sauce", "a fish sauce filling task", "HasCooking", null, null, null, cooking,
			Difficulty.Normal, Outcome.MinorFail, 5, 2, false,
			SimpleFoodPhases("$0 pack|packs fish and salt down for fermentation.",
				"$0 seal|seals the fish sauce in $p1 to ferment."),
			[
				"CommodityTag - 1 kilogram of a material tagged as Meat; piletag Raw Meat Commodity",
				"Commodity - 300 grams of salt"
			],
			["TagTool - Held - an item with the Fermentation Amphora tag"],
			[StableSimpleProduct("antiquity_food_fermenting_garum_amphora")],
			[]);
	}

	private void SeedAntiquityCultureFoodCrafts()
	{
		SeedAntiquityGroupedPreparedFoodCrafts();
		foreach (var culture in AntiquityFoodCultures)
		{
			SeedAntiquityCultureBeverageCrafts(culture);
		}
	}

	private void SeedAntiquityGroupedPreparedFoodCrafts()
	{
		var cooking = _traits["Cooking"] ?? _traits["Cook"] ?? _traits.First().Value;
		var baking = _traits["Baking"] ?? _traits["Baker"] ?? cooking;

		AddAntiquityGroupedPreparedFoodCraft(
			"flatbread",
			"bake Antiquity flatbread",
			"bake a generalized ancient flatbread",
			"baking ancient flatbread",
			"an Antiquity flatbread baking task",
			[
				"CommodityTag - 500 grams of a material tagged as Food Crop; piletag Flour Commodity",
				"LiquidUse - 250 millilitres of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			baking,
			[(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 mix|mixes $i1 with $i2 into a rough dough.",
				"$0 knead|kneads the dough and shape|shapes it into a thin round.",
				"$0 bake|bakes the flatbread over $t2 and set|sets aside $p1."),
			difficulty: Difficulty.Normal);

		AddAntiquityGroupedPreparedFoodCraft(
			"porridge",
			"cook Antiquity grain porridge",
			"cook generalized ancient grain porridge",
			"cooking ancient grain porridge",
			"an Antiquity porridge cooking task",
			[
				"CommodityTag - 600 grams of a material tagged as Food Crop; piletag Meal Commodity",
				"LiquidUse - 1 litre of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			cooking,
			[(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 sort|sorts $i1 and stir|stirs it into $i2 in $t1.",
				"$0 simmer|simmers the grain until it softens and thickens.",
				"$0 taste|tastes the porridge and set|sets aside $p1."),
			difficulty: Difficulty.Normal);

		AddAntiquityGroupedPreparedFoodCraft(
			"pulse_stew",
			"cook Antiquity pulse stew",
			"cook generalized ancient pulse stew",
			"cooking ancient pulse stew",
			"an Antiquity pulse stew cooking task",
			[
				"CommodityTag - 500 grams of a material tagged as Food Crop; piletag Pulse Meal Commodity",
				"CommodityTag - 250 grams of a material tagged as Vegetable Prep Crop; piletag Vegetable Prep Commodity",
				"LiquidUse - 1 litre of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			cooking,
			[(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 chop|chops $i2 and combine|combines it with $i1.",
				"$0 simmer|simmers the pulses and vegetables in $i3 over $t2.",
				"$0 season|seasons $p1 and set|sets the stew aside."),
			difficulty: Difficulty.Normal);

		AddAntiquityGroupedPreparedFoodCraft(
			"meat_dish",
			"cook Antiquity meat and grain dish",
			"cook a generalized ancient meat and grain dish",
			"cooking an ancient meat and grain dish",
			"an Antiquity meat dish cooking task",
			[
				"CommodityTag - 500 grams of a material tagged as Meat; piletag Prepared Meat Commodity",
				"CommodityTag - 400 grams of a material tagged as Food Crop; piletag Meal Commodity",
				"LiquidUse - 500 millilitres of meat broth"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			cooking,
			phases: LuxuryFoodPhases(
				"$0 cut|cuts $i1 and lay|lays it over $i2.",
				"$0 simmer|simmers the meat and grain in $i3 over $t2.",
				"$0 rest|rests $p1 before sending it to the table."),
			difficulty: Difficulty.Normal);

		AddAntiquityGroupedPreparedFoodCraft(
			"preserved_meat",
			"pack Antiquity preserved meat ration",
			"pack a generalized ancient preserved meat ration",
			"packing an ancient preserved meat ration",
			"an Antiquity preserved ration packing task",
			[
				"CommodityTag - 400 grams of a material tagged as Meat; piletag Dried Meat Commodity",
				"CommodityTag - 250 grams of a material tagged as Food Crop; piletag Bran Commodity"
			],
			[],
			cooking,
			phases: LuxuryFoodPhases(
				"$0 portion|portions $i1 and mix|mixes it with $i2.",
				"$0 press|presses the ration into a compact travel cake.",
				"$0 wrap|wraps $p1 against damp and vermin."),
			difficulty: Difficulty.Easy);

		AddAntiquityGroupedPreparedFoodCraft(
			"sweet",
			"make Antiquity fruit and grain sweet",
			"make a generalized ancient fruit and grain sweet",
			"making an ancient fruit sweet",
			"an Antiquity fruit sweet preparation task",
			[
				"CommodityTag - 400 grams of a material tagged as Fruit Must Crop; piletag Fruit Must Commodity",
				"CommodityTag - 250 grams of a material tagged as Food Crop; piletag Flour Commodity"
			],
			["TagTool - Held - an item with the Mortar and Pestle tag"],
			cooking,
			[(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 crush|crushes $i1 with $t1.",
				"$0 stir|stirs in $i2 until the sweet thickens.",
				"$0 shape|shapes $p1 and set|sets it aside to firm."),
			difficulty: Difficulty.Normal);

		AddAntiquityGroupedPreparedFoodCraft(
			"fruit_platter",
			"serve Antiquity fresh fruit platter",
			"prepare a generalized ancient fresh fruit platter",
			"preparing an ancient fresh fruit platter",
			"an Antiquity fresh fruit preparation task",
			["CommodityTag - 500 grams of a material tagged as Ready Fruit Crop; piletag Seeded Yield"],
			["TagTool - Held - an item with the Cooking Knife tag"],
			cooking,
			[(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 cut|cuts and seed|seeds $i1 with $t1.",
				"$0 arrange|arranges the prepared pieces on the serving dish.",
				"$0 set|sets aside $p1 while the fruit is fresh."),
			difficulty: Difficulty.Easy);

		AddAntiquityGroupedPreparedFoodCraft(
			"oilseed_cake",
			"bake Antiquity oilseed cake",
			"bake a generalized ancient oilseed cake",
			"baking an ancient oilseed cake",
			"an Antiquity oilseed cake baking task",
			[
				"CommodityTag - 300 grams of a material tagged as Oilseed Crop; piletag Oilseed Cake Commodity",
				"CommodityTag - 300 grams of a material tagged as Food Crop; piletag Flour Commodity",
				"LiquidUse - 150 millilitres of Water"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			baking,
			[(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 mix|mixes $i1 and $i2 with $i3 into a coarse batter.",
				"$0 press|presses the batter into small cakes.",
				"$0 bake|bakes the cakes over $t2 and set|sets aside $p1."),
			difficulty: Difficulty.Normal);

		AddAntiquityGroupedPreparedFoodCraft(
			"spiced_meat_stew",
			"cook Antiquity spiced meat stew",
			"cook a generalized ancient luxury meat stew",
			"cooking an ancient spiced meat stew",
			"an Antiquity luxury meat stew cooking task",
			[
				"CommodityTag - 500 grams of a material tagged as Meat; piletag Prepared Meat Commodity",
				"CommodityTag - 350 grams of a material tagged as Food Crop; piletag Pulse Meal Commodity",
				"CommodityTag - 250 grams of a material tagged as Vegetable Prep Crop; piletag Vegetable Prep Commodity",
				"LiquidUse - 750 millilitres of meat broth",
				"Commodity - 25 grams of coriander; piletag Seeded Yield",
				"Commodity - 10 grams of cumin; piletag Seeded Yield"
			],
			[
				"TagTool - InRoom - an item with the Cooking Pot tag",
				"TagTool - InRoom - an item with the Fire tag",
				"TagTool - Held - an item with the Mortar and Pestle tag"
			],
			cooking,
			materialInputs: [(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 toast|toasts $i5 and $i6 with $t3 until the spice wakes.",
				"$0 simmer|simmers meat, pulses, vegetables and broth together in $t1.",
				"$0 finish|finishes the rich stew and set|sets aside $p1."),
			difficulty: Difficulty.Hard);

		AddAntiquityGroupedPreparedFoodCraft(
			"honeyed_pastry",
			"bake Antiquity honeyed pastry",
			"bake a generalized ancient honeyed pastry",
			"baking an ancient honeyed pastry",
			"an Antiquity honeyed pastry baking task",
			[
				"CommodityTag - 400 grams of a material tagged as Food Crop; piletag Flour Commodity",
				"Commodity - 200 grams of honey; piletag Pressed Honey",
				"LiquidUse - 150 millilitres of olive oil",
				"Commodity - 5 grams of saffron; piletag Textile Dye Stock"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			baking,
			materialInputs: [(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 knead|kneads flour with honey and olive oil.",
				"$0 tint|tints the pastry with $i4 and shape|shapes it carefully.",
				"$0 bake|bakes the pastry near $t2 and set|sets aside $p1."),
			difficulty: Difficulty.Hard);

		AddAntiquityGroupedPreparedFoodCraft(
			"fish_sauce_relish",
			"prepare Antiquity fish sauce relish",
			"prepare a generalized ancient fish sauce relish",
			"preparing an ancient fish sauce relish",
			"an Antiquity fish sauce relish preparation task",
			[
				"LiquidUse - 250 millilitres of garum sauce",
				"CommodityTag - 200 grams of a material tagged as Fruit Brining Crop; piletag Brined Fruit Commodity",
				"CommodityTag - 200 grams of a material tagged as Vegetable Prep Crop; piletag Vegetable Prep Commodity",
				"Commodity - 15 grams of black pepper; piletag Seeded Yield"
			],
			["TagTool - Held - an item with the Mortar and Pestle tag", "TagTool - Held - an item with the Cooking Knife tag"],
			cooking,
			phases: LuxuryFoodPhases(
				"$0 mince|minces $i2 and $i3 with $t2.",
				"$0 pound|pounds $i4 and garum together in $t1.",
				"$0 fold|folds the relish together and set|sets aside $p1."),
			difficulty: Difficulty.Hard);

		AddAntiquityGroupedPreparedFoodCraft(
			"stuffed_flatbread",
			"bake Antiquity stuffed flatbread",
			"bake a generalized ancient stuffed flatbread",
			"baking an ancient stuffed flatbread",
			"an Antiquity stuffed flatbread baking task",
			[
				"CommodityTag - 350 grams of a material tagged as Food Crop; piletag Flour Commodity",
				"CommodityTag - 300 grams of a material tagged as Meat; piletag Prepared Meat Commodity",
				"CommodityTag - 150 grams of a material tagged as Fruit Must Crop; piletag Fruit Must Commodity",
				"LiquidUse - 100 millilitres of olive oil",
				"Commodity - 10 grams of cumin; piletag Seeded Yield"
			],
			["TagTool - InRoom - an item with the Cooking Pot tag", "TagTool - InRoom - an item with the Fire tag"],
			baking,
			materialInputs: [(1, 1)],
			phases: LuxuryFoodPhases(
				"$0 roll|rolls flour dough thin and oil|oils it with $i4.",
				"$0 fill|fills the dough with meat, fruit and $i5.",
				"$0 bake|bakes the stuffed bread near $t2 and set|sets aside $p1."),
			difficulty: Difficulty.Hard);
	}

	private void AddAntiquityGroupedPreparedFoodCraft(
		string suffix,
		string name,
		string blurb,
		string action,
		string itemSdesc,
		IEnumerable<string> inputs,
		IEnumerable<string> tools,
		MudSharp.Models.TraitDefinition trait,
		List<(int Product, int Input)>? materialInputs = null,
		IEnumerable<(int Seconds, string Echo, string FailEcho)>? phases = null,
		Difficulty difficulty = Difficulty.Normal)
	{
		var selector = EnsureAntiquityPreparedFoodSelectorProg(suffix);
		AddCraft(
			name,
			"Cooking",
			blurb,
			action,
			itemSdesc,
			"Antiquity Foodways",
			trait.Name,
			null,
			difficulty,
			Outcome.MinorFail,
			5,
			3,
			false,
			phases ?? SimpleFoodPhases(
				"$0 prepare|prepares $i1 and the other ingredients.",
				"$0 finish|finishes the dish and set|sets aside $p1."),
			inputs,
			tools,
			[$"ProgCookedFoodProduct - {selector}"],
			[],
			materialInputs,
			null,
			knowledgeType: "Crafting",
			knowledgeSubtype: "Foodways",
			knowledgeDescription: "Shared Antiquity foodways for generalized culture dishes.",
			knowledgeLongDescription: "This knowledge covers shared grain, pulse, meat, fruit, preserved, luxury, and condiment preparations used across the Antiquity culture catalogue.");
	}

	private string EnsureAntiquityPreparedFoodSelectorProg(string suffix)
	{
		var progName = $"ItemSeederAntiquityFood_{SanitiseProgPart(suffix)}";
		var stableReferences = AntiquityFoodCultures
			.Select(x => $"antiquity_food_{x.Key}_{suffix}")
			.ToArray();
		var body = new List<string> { "var products as item collection" };
		body.AddRange(stableReferences.Select(x => $"additem products loaditem(\"{x}\")"));
		body.Add("return collectionfirst(collectionshuffle(@products))");
		const string generatedComment =
			"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.";
		var prog = EnsureFutureProg(
			progName,
			"Crafting",
			"Antiquity Foodways",
			ProgVariableTypes.Item,
			generatedComment,
			[],
			string.Join(Environment.NewLine, body));
		var expectedBody = string.Join(Environment.NewLine, body);
		if (!prog.FunctionComment.Equals(generatedComment, StringComparison.Ordinal) ||
			!prog.FunctionText.Equals(expectedBody, StringComparison.Ordinal) ||
			prog.ReturnType != (long)ProgVariableTypes.Item)
		{
			prog.FunctionComment = generatedComment;
			prog.FunctionText = expectedBody;
			prog.ReturnType = (long)ProgVariableTypes.Item;
		}
		SaveFutureProgsIfRequired(prog);
		return progName;
	}

	private void SeedAntiquityCultureBeverageCrafts(AntiquityFoodCultureSpec culture)
	{
		var brewing = _traits["Brewing"] ?? _traits["Brewer"] ?? _traits["Cooking"] ?? _traits.First().Value;

		AddCultureFoodCraft(culture, $"fill {culture.Display.ToLowerInvariant()} beverage amphora", "fill a staple beverage amphora",
			"filling a beverage amphora", "a beverage fermentation task",
			[
				CultureBeverageStockInput(culture, 1200.0, 1000.0)
			],
			["TagTool - Held - an item with the Fermentation Amphora tag"],
			StableSimpleProduct(CultureBeverageFermentingStableReference(culture)),
			brewing,
			phases: SimpleFoodPhases("$0 pour|pours the beverage stock into $t1 and seal|seals it.",
				"$0 set|sets aside $p1 to ferment."));

		AddCultureFoodCraft(culture, $"fill {culture.Display.ToLowerInvariant()} spiced beverage amphora",
			"fill a luxury spiced beverage amphora", "filling a spiced beverage amphora",
			"a spiced beverage aging task",
			[
				CultureBeverageStockInput(culture, 1200.0, 1000.0),
				"Commodity - 250 grams of honey; piletag Pressed Honey",
				"Commodity - 15 grams of coriander; piletag Seeded Yield"
			],
			[
				"TagTool - Held - an item with the Mortar and Pestle tag",
				"TagTool - Held - an item with the Fermentation Amphora tag"
			],
			StableSimpleProduct(CultureLuxuryBeverageAgingStableReference(culture)),
			brewing,
			phases: LuxuryFoodPhases("$0 bruise|bruises $i3 in $t1 and stir|stirs it with honey.",
				"$0 blend|blends the sweet spice into the beverage stock.",
				"$0 strain|strains the luxury drink into $t2 and set|sets aside $p1 to age."),
			difficulty: Difficulty.Hard);
	}

	private void AddCultureFoodCraft(AntiquityFoodCultureSpec culture, string name, string blurb, string action,
		string itemSdesc, IEnumerable<string> inputs, IEnumerable<string> tools, string product,
		MudSharp.Models.TraitDefinition trait, List<(int Product, int Input)>? materialInputs = null,
		IEnumerable<(int Seconds, string Echo, string FailEcho)>? phases = null,
		Difficulty difficulty = Difficulty.Normal)
	{
		AddCraft(name, "Cooking", blurb, action, itemSdesc, culture.Knowledge, trait.Name, null, difficulty,
			Outcome.MinorFail, 5, 3, false,
			phases ?? SimpleFoodPhases("$0 prepare|prepares $i1 and the other ingredients.",
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

	private string CultureBeverageStockInput(AntiquityFoodCultureSpec culture, double wineFruitGrams, double wortGrams)
	{
		if (culture.BeverageLiquid.Contains("kumis", StringComparison.OrdinalIgnoreCase))
		{
			return "LiquidUse - 3 litres of milk";
		}

		return culture.BeverageLiquid.Contains("wine", StringComparison.OrdinalIgnoreCase)
			? $"Commodity - {FormatCommodityAmount(wineFruitGrams)} of {culture.SweetMaterial}; piletag Fruit Must Commodity"
			: $"CommodityTag - {FormatCommodityAmount(wortGrams)} of a material tagged as Grain Crop; piletag Wort Commodity";
	}

	private static string CultureLuxuryBeverageLiquid(AntiquityFoodCultureSpec culture)
	{
		if (culture.BeverageLiquid.Contains("wine", StringComparison.OrdinalIgnoreCase))
		{
			return "spiced wine";
		}

		if (culture.BeverageLiquid.Contains("kumis", StringComparison.OrdinalIgnoreCase))
		{
			return "spiced kumis";
		}

		return "spiced beer";
	}

	private static string CultureBeverageFermentingStableReference(AntiquityFoodCultureSpec culture)
	{
		if (culture.BeverageLiquid.Equals("red wine", StringComparison.OrdinalIgnoreCase))
		{
			return "antiquity_food_fermenting_red_wine_amphora";
		}

		if (culture.BeverageLiquid.Equals("white wine", StringComparison.OrdinalIgnoreCase))
		{
			return "antiquity_food_fermenting_white_wine_amphora";
		}

		if (culture.BeverageLiquid.Contains("date beer", StringComparison.OrdinalIgnoreCase))
		{
			return "antiquity_food_fermenting_date_beer_amphora";
		}

		if (culture.BeverageLiquid.Contains("kumis", StringComparison.OrdinalIgnoreCase))
		{
			return "antiquity_food_fermenting_kumis_amphora";
		}

		return "antiquity_food_fermenting_beer_amphora";
	}

	private static string CultureLuxuryBeverageAgingStableReference(AntiquityFoodCultureSpec culture)
	{
		return CultureLuxuryBeverageLiquid(culture) switch
		{
			"spiced wine" => "antiquity_food_aging_spiced_wine_amphora",
			"spiced kumis" => "antiquity_food_aging_spiced_kumis_amphora",
			_ => "antiquity_food_aging_spiced_beer_amphora"
		};
	}

	private static IEnumerable<(int Seconds, string Echo, string FailEcho)> SimpleFoodPhases(string first, string second)
	{
		return
		[
			(20, first, first),
			(25, second, second)
		];
	}

	private static IEnumerable<(int Seconds, string Echo, string FailEcho)> LuxuryFoodPhases(string first, string second, string third)
	{
		return
		[
			(25, first, first),
			(30, second, second),
			(25, third, third)
		];
	}
}
