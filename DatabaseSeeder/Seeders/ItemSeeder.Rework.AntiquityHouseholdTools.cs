using MudSharp.GameItems;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedAntiquityHouseholdCraftTools()
	{
		CreateAntiquityHouseholdCraftTool("antiquity_bronze_hand_saw", "saw", "a bronze hand saw",
			"A short bronze saw with a riveted wooden grip and a straight row of hand-filed teeth, suitable for cutting boards, dowels, and small furniture members.",
			SizeCategory.Small, 520.0, 24.0m, "bronze",
			["Functions / Tools / Woodcrafting Tools / Saws / Hand Saw"]);
		CreateAntiquityHouseholdCraftTool("antiquity_bronze_adze", "adze", "a bronze woodworking adze",
			"A bronze adze blade is lashed firmly across a smooth wooden haft, giving a furniture maker enough bite to dress boards, hollow stools, and rough vessel staves.",
			SizeCategory.Normal, 1150.0, 30.0m, "bronze",
			["Functions / Tools / Woodcrafting Tools / Adze"]);
		CreateAntiquityHouseholdCraftTool("antiquity_bronze_wood_chisel", "chisel", "a bronze wood chisel",
			"A narrow bronze chisel with a faceted wooden handle, kept sharp for cutting mortises, trimming pegs, and carving small household fittings.",
			SizeCategory.Small, 280.0, 18.0m, "bronze",
			["Functions / Tools / Woodcrafting Tools / Wood Chisel"]);
		CreateAntiquityHouseholdCraftTool("antiquity_bronze_wood_auger", "auger", "a bronze wood auger",
			"A bronze auger with a simple cross-grip, used to bore peg holes, hinge sockets, bung holes, and narrow openings in wooden vessels.",
			SizeCategory.Small, 470.0, 26.0m, "bronze",
			["Functions / Tools / Woodcrafting Tools / Wood Auger"]);
		CreateAntiquityHouseholdCraftTool("antiquity_wooden_smoothing_plane", "plane", "a wooden smoothing plane with a bronze iron",
			"A blocky wooden smoothing plane holds a small bronze iron in a wedged throat, letting a joiner level tabletops, chest boards, and visible furniture faces.",
			SizeCategory.Small, 900.0, 28.0m, "oak",
			["Functions / Tools / Woodcrafting Tools / Planer"]);
		CreateAntiquityHouseholdCraftTool("antiquity_wooden_joinery_clamp", "clamp", "a wooden joinery clamp",
			"A wooden clamp with opposing jaws and a wedge key, useful for holding glued boards, pegged frames, or leather-covered furniture while the work sets.",
			SizeCategory.Normal, 1600.0, 16.0m, "oak",
			["Functions / Tools / Woodcrafting Tools / Wood Clamp"]);
		CreateAntiquityHouseholdCraftTool("antiquity_bow_drill", "drill", "a bow drill with a bronze bit",
			"A simple bow drill with a corded bow, wooden hand block, and small bronze bit, suited to slow, controlled holes in wood, bone, shell, and soft stone.",
			SizeCategory.Small, 430.0, 18.0m, "oak",
			["Functions / Tools / Woodcrafting Tools / Bow Drill"]);
		CreateAntiquityHouseholdCraftTool("antiquity_wood_rasp", "rasp", "a bronze-toothed wood rasp",
			"A bronze rasp punched with coarse teeth and fitted with a wooden grip, used to soften corners and fair irregular curves before final smoothing.",
			SizeCategory.Small, 380.0, 15.0m, "bronze",
			["Functions / Tools / Woodcrafting Tools / Rasp"]);
		CreateAntiquityHouseholdCraftTool("antiquity_coopers_adze", "adze", "a cooper's bronze adze",
			"A short-handled cooper's adze with a curved bronze bit, shaped for hollowing and dressing the inner faces of staves.",
			SizeCategory.Normal, 1250.0, 32.0m, "bronze",
			["Functions / Tools / Woodcrafting Tools / Coopering Tools / Cooper's Adze"]);
		CreateAntiquityHouseholdCraftTool("antiquity_coopers_croze", "croze", "a cooper's croze",
			"A wooden-bodied croze carries a bronze cutter for cutting the grooves that receive cask heads and bucket bottoms.",
			SizeCategory.Small, 520.0, 22.0m, "oak",
			["Functions / Tools / Woodcrafting Tools / Coopering Tools / Croze"]);
		CreateAntiquityHouseholdCraftTool("antiquity_hoop_driver", "driver", "a wooden hoop driver",
			"A blunt wooden hoop driver with a broad striking face, made for walking hoops down over staves without splitting them.",
			SizeCategory.Normal, 900.0, 8.0m, "oak",
			["Functions / Tools / Woodcrafting Tools / Coopering Tools / Hoop Driver"]);
		CreateAntiquityHouseholdCraftTool("antiquity_bung_borer", "borer", "a bronze bung borer",
			"A bronze-edged bung borer set into a wooden brace, used to open neat round holes in casks, tubs, and brewing vessels.",
			SizeCategory.Small, 620.0, 24.0m, "bronze",
			["Functions / Tools / Woodcrafting Tools / Coopering Tools / Bung Borer"]);
		CreateAntiquityHouseholdCraftTool("antiquity_basket_knife", "knife", "a bronze basket knife",
			"A small bronze knife with a hooked tip for trimming willow, reed, bark, and splints during basketry and matting work.",
			SizeCategory.Small, 180.0, 10.0m, "bronze",
			["Functions / Tools / Textilecraft Tools / Basketry Tools / Basket Knife"]);
		CreateAntiquityHouseholdCraftTool("antiquity_reed_splitter", "splitter", "a bronze reed splitter",
			"A bronze reed splitter with three narrow fins that divide reeds and willow rods into even splints for baskets, trays, and seats.",
			SizeCategory.VerySmall, 85.0, 9.0m, "bronze",
			["Functions / Tools / Textilecraft Tools / Basketry Tools / Reed Splitter"]);
		CreateAntiquityHouseholdCraftTool("antiquity_weaving_bodkin", "bodkin", "a bronze weaving bodkin",
			"A smooth bronze bodkin used to open tight woven courses, tuck ends, and draw bindings through basketry or leather work.",
			SizeCategory.VerySmall, 45.0, 7.0m, "bronze",
			["Functions / Tools / Textilecraft Tools / Basketry Tools / Weaving Bodkin"]);
		CreateAntiquityHouseholdCraftTool("antiquity_packing_bone", "bone", "a polished packing bone",
			"A polished bone tool for pressing weavers close, creasing folds, and settling splints without cutting their fibres.",
			SizeCategory.Small, 90.0, 5.0m, "horn",
			["Functions / Tools / Textilecraft Tools / Basketry Tools / Packing Bone"]);
		CreateAntiquityHouseholdCraftTool("antiquity_leather_awl_punch", "awl", "a bronze leather awl punch",
			"A tapered bronze awl punch with a sturdy wooden grip, used to open stitch holes in leather panels, straps, and fitted covers.",
			SizeCategory.Small, 140.0, 9.0m, "bronze",
			["Functions / Tools / Leatherworking Tools / Awl Punch"]);
		CreateAntiquityHouseholdCraftTool("antiquity_leather_stitching_pony", "pony", "a wooden leather stitching pony",
			"A simple wooden clamp with jaws sized for gripping leather panels so both hands can work a needle and cord through heavy seams.",
			SizeCategory.Normal, 2100.0, 18.0m, "oak",
			["Functions / Tools / Leatherworking Tools / Leather Stitching Pony"]);
		CreateAntiquityHouseholdCraftTool("antiquity_leather_edge_beveller", "beveller", "a bronze leather edge beveller",
			"A small bronze edge beveller for shaving hard corners from straps, pouches, buckets, and leather-covered boxes.",
			SizeCategory.VerySmall, 90.0, 8.0m, "bronze",
			["Functions / Tools / Leatherworking Tools / Edge Beveller"]);
		CreateAntiquityHouseholdCraftTool("antiquity_slow_potters_wheel", "wheel", "a slow wooden potter's wheel",
			"A heavy wooden turntable set on a low pivot, spun by hand for shaping small bowls, jars, lamps, and household vessels.",
			SizeCategory.Large, 18000.0, 42.0m, "oak",
			["Functions / Tools / Pottery Tools / Potter's Wheel"]);
		CreateAntiquityHouseholdCraftTool("antiquity_clay_knife", "knife", "a bronze clay knife",
			"A blunt-edged bronze clay knife for cutting wet clay, trimming vessel mouths, and cleaning the foot of a pot before firing.",
			SizeCategory.Small, 160.0, 8.0m, "bronze",
			["Functions / Tools / Pottery Tools / Clay Knife"]);
		CreateAntiquityHouseholdCraftTool("antiquity_potters_rib", "rib", "a wooden potter's rib",
			"A curved wooden potter's rib polished smooth from drawing wet clay walls into steady, even curves.",
			SizeCategory.Small, 70.0, 4.0m, "boxwood",
			["Functions / Tools / Pottery Tools / Potter's Rib"]);
		CreateAntiquityHouseholdCraftTool("antiquity_loop_tool", "tool", "a bronze loop tool",
			"A bronze loop tool with a narrow cutting loop, made for hollowing, trimming, and refining leather-hard clay.",
			SizeCategory.VerySmall, 60.0, 8.0m, "bronze",
			["Functions / Tools / Pottery Tools / Loop Tool"]);
		CreateAntiquityHouseholdCraftTool("antiquity_wire_cutter", "cutter", "a linen-cord clay wire cutter",
			"A taut linen cord stretched between wooden toggles, used to lift clay from the wheel and divide prepared clay bodies.",
			SizeCategory.VerySmall, 40.0, 3.0m, "linen",
			["Functions / Tools / Pottery Tools / Wire Cutter"]);
		CreateAntiquityHouseholdCraftTool("antiquity_clay_stamp", "stamp", "a carved clay stamp",
			"A fired-clay stamp with a simple carved face, useful for repeating marks, borders, and workshop signs in soft clay.",
			SizeCategory.VerySmall, 120.0, 5.0m, "fired clay",
			["Functions / Tools / Pottery Tools / Clay Stamp"]);
		CreateAntiquityHouseholdCraftTool("antiquity_updraft_kiln", "kiln", "an updraft pottery kiln",
			"A squat earthenware updraft kiln with a low firebox and a pierced setting floor for firing small batches of vessels, lamps, and tiles.",
			SizeCategory.Huge, 65000.0, 120.0m, "earthenware",
			["Functions / Tools / Pottery Tools / Kiln"]);
		CreateAntiquityHouseholdCraftTool("antiquity_glass_blowpipe", "blowpipe", "a bronze glass blowpipe",
			"A long bronze blowpipe with a flared mouthpiece and a heat-darkened working end for gathering and inflating molten glass.",
			SizeCategory.Normal, 1800.0, 38.0m, "bronze",
			["Functions / Tools / Glassblowing Tools / Blowpipe"]);
		CreateAntiquityHouseholdCraftTool("antiquity_pontil_rod", "rod", "a bronze pontil rod",
			"A straight bronze pontil rod used to carry hot glass while the mouth, foot, or rim of a vessel is finished.",
			SizeCategory.Normal, 1650.0, 28.0m, "bronze",
			["Functions / Tools / Glassblowing Tools / Pontil Rod"]);
		CreateAntiquityHouseholdCraftTool("antiquity_marver_slab", "slab", "a stone marver slab",
			"A smooth stone marver slab where a glassworker can roll a gather of hot glass to centre and cool its surface.",
			SizeCategory.Large, 36000.0, 44.0m, "stone",
			["Functions / Tools / Glassblowing Tools / Marver Table"]);
		CreateAntiquityHouseholdCraftTool("antiquity_glassworking_jacks", "jacks", "a pair of bronze glassworking jacks",
			"A pair of bronze jacks with long handles and blunt blades for shaping necks, shoulders, and rims on hot glass vessels.",
			SizeCategory.Small, 700.0, 26.0m, "bronze",
			["Functions / Tools / Glassblowing Tools / Jacks"]);
		CreateAntiquityHouseholdCraftTool("antiquity_glass_shears", "shears", "a pair of bronze glass shears",
			"Heavy bronze shears with long handles, made to cut hot glass threads and trim vessel mouths before annealing.",
			SizeCategory.Small, 820.0, 24.0m, "bronze",
			["Functions / Tools / Glassblowing Tools / Glass Shears"]);
		CreateAntiquityHouseholdCraftTool("antiquity_annealing_lehr", "lehr", "a small annealing lehr",
			"A long earthenware annealing lehr that holds finished glass at a gentler heat so vessels can cool without cracking.",
			SizeCategory.Huge, 52000.0, 110.0m, "earthenware",
			["Functions / Tools / Glassblowing Tools / Annealing Lehr"]);
		CreateAntiquityHouseholdCraftTool("antiquity_casting_crucible", "crucible", "a bronze casting crucible",
			"A thick-walled bronze crucible with a darkened lip for melting small charges of metal for vessels, fittings, and decorative mounts.",
			SizeCategory.Small, 1600.0, 36.0m, "bronze",
			["Functions / Tools / Metalworking Tools / Crucible"]);
		CreateAntiquityHouseholdCraftTool("antiquity_crucible_tongs", "tongs", "a pair of bronze crucible tongs",
			"Long bronze tongs with hooked jaws, shaped to grip hot crucibles and carry them clear of the furnace mouth.",
			SizeCategory.Normal, 1450.0, 28.0m, "bronze",
			["Functions / Tools / Smelting Tools / Crucible Tongs"]);
		CreateAntiquityHouseholdCraftTool("antiquity_vessel_casting_mould", "mould", "a clay vessel casting mould",
			"A two-part fired-clay mould for rough-casting simple cups, bowls, trays, and lamp bodies before hammering and burnishing.",
			SizeCategory.Normal, 4200.0, 32.0m, "fired clay",
			["Functions / Tools / Metalworking Tools / Casting Mould / Vessel Casting Mould"]);
		CreateAntiquityHouseholdCraftTool("antiquity_bronze_burnisher", "burnisher", "a polished bronze burnisher",
			"A rounded bronze burnisher kept smooth for brightening cast vessels, compressing rims, and settling thin decorative sheet.",
			SizeCategory.Small, 360.0, 18.0m, "bronze",
			["Functions / Tools / Metalworking Tools / Bronze Burnisher"]);
		CreateAntiquityHouseholdCraftTool("antiquity_stone_chisel", "chisel", "a bronze stone chisel",
			"A stout bronze chisel with a short edge for working soft stone, alabaster, horn, and ivory vessel blanks.",
			SizeCategory.Small, 540.0, 20.0m, "bronze",
			["Functions / Tools / Stoneworking Tools / Stone Chisel"]);
		CreateAntiquityHouseholdCraftTool("antiquity_stone_mallet", "mallet", "a wooden stoneworking mallet",
			"A dense wooden mallet with a rounded head, used to drive chisels without shattering fragile vessel blanks.",
			SizeCategory.Normal, 1300.0, 8.0m, "oak",
			["Functions / Tools / Stoneworking Tools / Stone Mallet", "Functions / Tools / Striking Tools / Mallet"]);
		CreateAntiquityHouseholdCraftTool("antiquity_polishing_stone", "stone", "a smooth polishing stone",
			"A palm-sized polishing stone worn flat by use, suited to rubbing soft stone, horn, clay, and finished wood to a low sheen.",
			SizeCategory.Small, 260.0, 4.0m, "stone",
			["Functions / Tools / Stoneworking Tools / Polishing Stone"]);
		CreateAntiquityHouseholdCraftTool("antiquity_candle_mould", "mould", "a clay candle mould",
			"A fired-clay candle mould pierced with narrow channels and wick holes for shaping beeswax household candles.",
			SizeCategory.Small, 900.0, 12.0m, "fired clay",
			["Functions / Tools / Candlemaking Tools / Candle Mould"]);
		CreateAntiquityHouseholdCraftTool("antiquity_lamp_mould", "mould", "a clay lamp mould",
			"A two-piece fired-clay lamp mould for pressing small saucer lamps with pinched spouts and shallow oil reservoirs.",
			SizeCategory.Small, 1100.0, 14.0m, "fired clay",
			["Functions / Tools / Candlemaking Tools / Lamp Mould"]);
		CreateAntiquityHouseholdCraftTool("antiquity_lacquer_brush", "brush", "a fine lacquerer's brush",
			"A fine brush with a narrow wooden handle and tightly bound bristles for laying smooth coats of lacquer over boxes, stands, and fittings.",
			SizeCategory.VerySmall, 35.0, 7.0m, "wood",
			["Functions / Tools / Lacquerwork Tools / Lacquerer's Brush"]);
		CreateAntiquityHouseholdCraftTool("antiquity_lacquer_spatula", "spatula", "a wooden lacquer spatula",
			"A thin wooden spatula for spreading, scraping, and lifting lacquer while preparing decorated household surfaces.",
			SizeCategory.VerySmall, 45.0, 6.0m, "boxwood",
			["Functions / Tools / Lacquerwork Tools / Lacquer Spatula"]);
	}

	private void CreateAntiquityHouseholdCraftTool(string stableReference, string noun, string shortDescription,
		string fullDescription, SizeCategory size, double weightInGrams, decimal cost, string material,
		IEnumerable<string> functionalTags)
	{
		var tags = new List<string> { "Market / Professional Tools / Standard Tools" };
		tags.AddRange(functionalTags);
		CreateItem(
			stableReference,
			noun,
			shortDescription,
			null,
			fullDescription,
			size,
			ItemQuality.Standard,
			weightInGrams,
			cost,
			false,
			false,
			material,
			tags,
			["Holdable", (int)size >= (int)SizeCategory.Large ? "Destroyable_Furniture" : "Destroyable_Misc"],
			null,
			null,
			null,
			null
		);
	}
}
