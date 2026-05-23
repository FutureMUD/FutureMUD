#nullable enable

using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientAgriculturalProcessingKnowledge = "Ancient Agricultural Processing";

	private void SeedAntiquityAgriculturalProcessingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		AddAntiquityCraft(
			"select antiquity seed stock",
			"Farming",
			"select viable seed or planting stock from harvested crop commodity",
			"selecting seed stock",
			"a seed selection task",
			AncientAgriculturalProcessingKnowledge,
			"Farming",
			10,
			Difficulty.Easy,
			SimpleFoodPhases("$0 sort|sorts through $i1, discarding damaged material and setting aside viable seed stock.",
				"$0 finish|finishes selecting $p1 from the harvested crop."),
			["CommodityTag - 5 kilograms of a material tagged as Agriculture Seedable; tag Seeded Yield"],
			["TagTool - Held - an item with the Winnowing Basket tag"],
			["ScrapInput - 25.00% by weight of 5 kilograms of a material tagged as Agriculture Seedable ($i1); tag Seeds"],
			["ScrapInput - 5.00% by weight of 5 kilograms of a material tagged as Agriculture Seedable ($i1); tag Seeds"],
			knowledgeSubtype: "Farming",
			knowledgeDescription: "Ancient crop selection, seed saving, and farm-stock processing.",
			knowledgeLongDescription: "This knowledge covers selecting viable seed stock and processing common ancient agricultural outputs into reusable craft commodities.");

		AddAntiquityCraft(
			"strain fresh milk into amphora",
			"Food Processing",
			"strain fresh herd milk into a serving amphora",
			"straining fresh milk",
			"a fresh milk straining task",
			AncientAgriculturalProcessingKnowledge,
			"Cooking",
			10,
			Difficulty.Easy,
			SimpleFoodPhases("$0 strain|strains $i1 through $t1, keeping curd and hair out of the milk.",
				"$0 pour|pours the strained milk into $p1."),
			["Commodity - 3 kilograms of milk; piletag Raw Milk"],
			["TagTool - Held - an item with the Honey Strainer tag"],
			[$"LiquidProduct - 1x {_items["antiquity_food_serving_amphora"].ShortDescription} (#{_items["antiquity_food_serving_amphora"].Id}) filled with 3 litres of milk"],
			[],
			knowledgeSubtype: "Food Processing");

		AddAntiquityCraft(
			"heap herd manure compost",
			"Farming",
			"turn raw herd manure and crop refuse into compost",
			"heaping herd manure compost",
			"a manure composting task",
			AncientAgriculturalProcessingKnowledge,
			"Farming",
			10,
			Difficulty.Easy,
			SimpleFoodPhases("$0 fork|forks $i1 and $i2 together into a hot compost heap.",
				"$0 turn|turns the heap and sets aside usable compost."),
			[
				"Commodity - 3 kilograms of feces; piletag Manure Commodity",
				"Commodity - 2 kilograms of vegetation; piletag Seeded Yield"
			],
			["TagTool - Held - an item with the Pitchfork tag"],
			["CommodityProduct - 3 kilograms 500 grams of compost commodity; tag Manure Commodity"],
			["CommodityProduct - 1 kilogram of compost commodity; tag Manure Commodity"],
			knowledgeSubtype: "Farming");

		AddAntiquityCraft(
			"ferment indigo dye cakes",
			"Dyeing",
			"ferment and dry indigo leaves into dye cakes",
			"fermenting indigo dye cakes",
			"an indigo dye fermentation task",
			AncientAgriculturalProcessingKnowledge,
			"Dyeing",
			20,
			Difficulty.Normal,
			SimpleFoodPhases("$0 steep|steeps $i1 in $t1 and work|works the vat until the blue settles.",
				"$0 dry|dries the residue into compact dye cakes."),
			["Commodity - 2 kilograms of indigo crop; piletag Seeded Yield"],
			[
				"TagTool - InRoom - an item with the Fermentation Amphora tag",
				"TagTool - Held - an item with the Honey Strainer tag"
			],
			["CommodityProduct - 500 grams of indigo dye cake commodity; tag Textile Dye Stock"],
			["CommodityProduct - 100 grams of indigo dye cake commodity; tag Textile Dye Stock"],
			knowledgeSubtype: "Dyeing");

		AddAntiquityCraft(
			"strip pomegranate rind dye stock",
			"Dyeing",
			"strip pomegranate rind for dye stock",
			"stripping pomegranate rind",
			"a pomegranate rind stripping task",
			AncientAgriculturalProcessingKnowledge,
			"Dyeing",
			10,
			Difficulty.Easy,
			SimpleFoodPhases("$0 score|scores and peel|peels $i1 with $t1.",
				"$0 dry|dries and bundles the rind for dyeing."),
			["Commodity - 2 kilograms of pomegranate; piletag Seeded Yield"],
			["TagTool - Held - an item with the Cooking Knife tag"],
			["CommodityProduct - 350 grams of pomegranate rind commodity; tag Textile Dye Stock"],
			["CommodityProduct - 80 grams of pomegranate rind commodity; tag Textile Dye Stock"],
			knowledgeSubtype: "Dyeing");

		AddAntiquityCraft(
			"separate walnut hull dye stock",
			"Dyeing",
			"separate walnut hulls for brown dye stock",
			"separating walnut hulls",
			"a walnut hull preparation task",
			AncientAgriculturalProcessingKnowledge,
			"Dyeing",
			10,
			Difficulty.Easy,
			SimpleFoodPhases("$0 crack|cracks and bruise|bruises $i1 with $t1.",
				"$0 pick|picks out the staining hulls and sets them aside."),
			["Commodity - 2 kilograms of walnut nut; piletag Seeded Yield"],
			["TagTool - Held - an item with the Mortar and Pestle tag"],
			["CommodityProduct - 300 grams of walnut hull commodity; tag Textile Dye Stock"],
			["CommodityProduct - 70 grams of walnut hull commodity; tag Textile Dye Stock"],
			knowledgeSubtype: "Dyeing");

		AddAntiquityCraft(
			"dry saffron threads",
			"Food Processing",
			"dry saffron crocus stigmas into saffron",
			"drying saffron threads",
			"a saffron drying task",
			AncientAgriculturalProcessingKnowledge,
			"Cooking",
			20,
			Difficulty.Hard,
			SimpleFoodPhases("$0 pluck|plucks the tiny stigmas from $i1.",
				"$0 dry|dries the threads slowly into saffron."),
			["Commodity - 500 grams of saffron crocus; piletag Seeded Yield"],
			["TagTool - InRoom - an item with the Drying Rack tag"],
			["CommodityProduct - 45 grams of saffron commodity; tag Textile Dye Stock"],
			["CommodityProduct - 8 grams of saffron commodity; tag Textile Dye Stock"],
			knowledgeSubtype: "Food Processing");
	}
}
