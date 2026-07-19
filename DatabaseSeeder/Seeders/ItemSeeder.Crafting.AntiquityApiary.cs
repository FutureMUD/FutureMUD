using MudSharp.RPG.Checks;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientApicultureKnowledge = "Ancient Apiculture";

	private static readonly string[] AntiquityApiaryStableReferences =
	[
		"antiquity_wicker_beehive",
		"antiquity_clay_tube_hive",
		"antiquity_wooden_hive_stand",
		"antiquity_bee_smoke_pot",
		"antiquity_honey_knife",
		"antiquity_honey_press",
		"antiquity_honey_strainer"
	];

	private void SeedAntiquityApiaryCrafts()
	{
		if (!ShouldSeedAntiquityCrafts() ||
		    AntiquityApiaryStableReferences.Any(x => !TryLookupReworkItem(x, out _)))
		{
			return;
		}

		AddAntiquityCraft(
			"weave wicker beehive",
			"Basketry",
			"weave and seal a wicker beehive",
			"weaving a wicker beehive",
			"a wicker hive under construction",
			AncientApicultureKnowledge,
			"Basketry",
			15,
			Difficulty.Normal,
			[
				(35, "$0 soak|soaks and flex|flexes the splints from $i1, sorting them by thickness.", "$0 kink|kinks too many splints while preparing them."),
				(45, "$0 weave|weaves the hive body upward in a tight coil, packing each course with $t2.", "$0 weave|weaves the hive body unevenly and leaves gaps in the coil."),
				(35, "$0 bind|binds the lip with $i2 and smear|smears $i3 into the seams.", "$0 bind|binds the lip poorly and the hive will not hold its shape."),
				(25, "$0 set|sets aside $p1 after checking the entrance and sealed crown.", "$0 set|sets aside only $f1 after the hive body collapses.")
			],
			[
				CommodityInput(1800.0, "willow", "Basketry Splint"),
				CommodityInput(150.0, "linen", "Spun Yarn"),
				CommodityInput(120.0, "pitch", "Prepared Pitch")
			],
			[
				"TagTool - Held - an item with the Basket Knife tag",
				"TagTool - Held - an item with the Packing Bone tag"
			],
			[StableSimpleProduct("antiquity_wicker_beehive")]);

		AddAntiquityCraft(
			"finish clay tube beehive",
			"Pottery",
			"line and finish a clay tube hive",
			"finishing a clay tube hive",
			"a clay tube hive being finished",
			AncientApicultureKnowledge,
			"Pottery",
			20,
			Difficulty.Normal,
			[
				(30, "$0 checks $i1 for cracks and true|trues its mouth with $t2.", "$0 find|finds the fired body warped and hard to true."),
				(40, "$0 turn|turns $i1 on $t1, smoothing the inner mouth and capping surfaces.", "$0 scrape|scrapes unevenly and chips the hive mouth."),
				(35, "$0 warm|warms and smear|smears $i2 along the seams and plug line.", "$0 smear|smears the pitch too thinly to seal the hive."),
				(25, "$0 set|sets aside $p1 as a sealed clay hive.", "$0 set|sets aside only $f1 after the hive cracks.")
			],
			[
				CommodityInput(4500.0, "fired clay", "Bisque Vessel Blank"),
				CommodityInput(180.0, "pitch", "Prepared Pitch")
			],
			[
				"TagTool - InRoom - an item with the Potter's Wheel tag",
				"TagTool - Held - an item with the Potter's Rib tag",
				"TagTool - InRoom - an item with the Lit Kiln tag"
			],
			[StableSimpleProduct("antiquity_clay_tube_hive")]);

		AddAntiquityCraft(
			"build wooden hive stand",
			"Carpentry",
			"build a raised wooden hive stand",
			"building a hive stand",
			"a wooden hive stand under construction",
			AncientApicultureKnowledge,
			"Carpentry",
			15,
			Difficulty.Easy,
			[
				(35, "$0 mark|marks and saw|saws the stand parts from $i1.", "$0 saw|saws the stand parts unevenly."),
				(40, "$0 chisel|chisels notches and test|tests the fit with $t2.", "$0 chisel|chisels the notches too loose."),
				(35, "$0 bind|binds the frame with $i2 and clamp|clamps it square.", "$0 bind|binds the frame out of square."),
				(25, "$0 set|sets aside $p1 after checking that it sits level.", "$0 set|sets aside only $f1 after the stand twists apart.")
			],
			[
				CommodityInput(8000.0, "oak", "Furniture Timber Stock"),
				CommodityInput(350.0, "bronze", "Hoop Stock")
			],
			[
				"TagTool - Held - an item with the Hand Saw tag",
				"TagTool - Held - an item with the Wood Chisel tag",
				"TagTool - InRoom - an item with the Wood Clamp tag"
			],
			[StableSimpleProduct("antiquity_wooden_hive_stand")]);

		AddAntiquityCraft(
			"make bee smoke pot",
			"Pottery",
			"make a clay smoke pot for tending hives",
			"making a bee smoke pot",
			"a clay smoke pot under construction",
			AncientApicultureKnowledge,
			"Pottery",
			15,
			Difficulty.Easy,
			[
				(30, "$0 shape|shapes a small lidded pot from $i1.", "$0 shape|shapes the pot too thinly."),
				(35, "$0 draw|draws out a narrow spout and smooth|smooths it with $t2.", "$0 pinch|pinches the spout closed by mistake."),
				(45, "$0 fire|fires the smoke pot in $t3.", "$0 fire|fires the pot too quickly and it cracks."),
				(20, "$0 set|sets aside $p1.", "$0 set|sets aside only $f1.")
			],
			[CommodityInput(850.0, "fired clay", "Pottery Clay Body")],
			[
				"TagTool - InRoom - an item with the Potter's Wheel tag",
				"TagTool - Held - an item with the Potter's Rib tag",
				"TagTool - InRoom - an item with the Lit Kiln tag"
			],
			[StableSimpleProduct("antiquity_bee_smoke_pot")]);

		AddAntiquityCraft(
			"forge honey knife",
			"Blacksmithing",
			"forge a broad honey knife",
			"forging a honey knife",
			"a honey knife being forged",
			AncientApicultureKnowledge,
			"Blacksmithing",
			20,
			Difficulty.Normal,
			[
				(35, "$0 heat|heats $i1 in $t3 and grip|grips it with $t4.", "$0 heat|heats $i1 unevenly."),
				(45, "$0 hammer|hammers the blank wide and thin on $t2.", "$0 hammer|hammers a ragged, warped blade."),
				(35, "$0 fit|fits the handle stock from $i2 and bind|binds it tight.", "$0 fit|fits the handle poorly."),
				(25, "$0 set|sets aside $p1 after checking the edge.", "$0 set|sets aside only $f1 after the knife fails.")
			],
			[
				CommodityInput(250.0, "bronze", "Tool Blank Stock"),
				CommodityInput(90.0, "oak", "Tool Blank Stock")
			],
			[
				"TagTool - Held - an item with the Hammer tag",
				"TagTool - InRoom - an item with the Anvil tag",
				"TagTool - InRoom - an item with the Hot Fire tag",
				"TagTool - Held - an item with the Forge Tongs tag"
			],
			[StableSimpleProduct("antiquity_honey_knife")]);

		AddAntiquityCraft(
			"build honey press",
			"Carpentry",
			"build a small wooden honey press",
			"building a honey press",
			"a honey press under construction",
			AncientApicultureKnowledge,
			"Carpentry",
			25,
			Difficulty.Normal,
			[
				(45, "$0 saw|saws frame parts from $i1 and basket slats from $i2.", "$0 saw|saws the press parts unevenly."),
				(45, "$0 chisel|chisels the screw channel and pressure plate.", "$0 chisel|chisels the screw channel off centre."),
				(40, "$0 hoop|hoops the basket with $i3 and clamp|clamps the frame square.", "$0 hoop|hoops the basket too loosely."),
				(30, "$0 turn|turns the screw and set|sets aside $p1.", "$0 turn|turns the screw, but the press binds and fails.")
			],
			[
				CommodityInput(12000.0, "oak", "Furniture Timber Stock"),
				CommodityInput(2500.0, "willow", "Basketry Splint"),
				CommodityInput(700.0, "bronze", "Hoop Stock")
			],
			[
				"TagTool - Held - an item with the Hand Saw tag",
				"TagTool - Held - an item with the Wood Chisel tag",
				"TagTool - InRoom - an item with the Wood Clamp tag"
			],
			[StableSimpleProduct("antiquity_honey_press")]);

		AddAntiquityCraft(
			"stitch honey strainer",
			"Tailoring",
			"stitch a linen strainer for honey",
			"stitching a honey strainer",
			"a honey strainer being stitched",
			AncientApicultureKnowledge,
			"Tailoring",
			10,
			Difficulty.Easy,
			[
				(25, "$0 cut|cuts a clean square from $i1.", "$0 cut|cuts the cloth too small."),
				(30, "$0 stitch|stitches the cloth around the hoop stock from $i2.", "$0 stitch|stitches the cloth unevenly."),
				(20, "$0 pull|pulls the strainer taut and set|sets aside $p1.", "$0 pull|pulls the strainer out of shape.")
			],
			[
				CommodityInput(120.0, "linen", "Garment Cloth", colour: true, fineColour: true),
				CommodityInput(80.0, "willow", "Hoop Stock")
			],
			[
				"TagTool - Held - an item with the Sewing Needle tag",
				"TagTool - Held - an item with the Shears tag"
			],
			[StableSimpleProduct("antiquity_honey_strainer")]);

		AddAntiquityCraft(
			"press raw honeycomb",
			"Farming",
			"press raw honeycomb into honey and beeswax",
			"pressing raw honeycomb",
			"a batch of honeycomb being pressed",
			AncientApicultureKnowledge,
			"Farming",
			15,
			Difficulty.Easy,
			[
				(30, "$0 cut|cuts the raw comb with $t1 and load|loads it into $t2.", "$0 cut|cuts the comb raggedly and spills much of it."),
				(45, "$0 press|presses the comb, letting honey run through $t3.", "$0 press|presses too hard and fouls the strainer."),
				(35, "$0 scrape|scrapes waxy comb residue aside from the pressed honey.", "$0 scrape|scrapes wax and honey into a messy slurry."),
				(25, "$0 set|sets aside $p1 and $p2.", "$0 set|sets aside only $f1 after the batch is spoiled.")
			],
			["Commodity - 2 kilograms of honeycomb; piletag Raw Honeycomb"],
			[
				"TagTool - Held - an item with the Honey Knife tag",
				"TagTool - InRoom - an item with the Honey Press tag",
				"TagTool - Held - an item with the Honey Strainer tag"
			],
			[
				"CommodityProduct - 1 kilogram 200 grams of honey commodity; tag Pressed Honey",
				"CommodityProduct - 350 grams of beeswax commodity; tag Rendered Beeswax"
			],
			["CommodityProduct - 200 grams of honeycomb commodity; tag Raw Honeycomb"]);
	}
}
