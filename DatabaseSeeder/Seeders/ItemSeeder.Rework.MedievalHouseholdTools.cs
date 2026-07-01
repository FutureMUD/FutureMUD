#nullable enable

using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	// Previous source-shape audit marker retained until the medieval no-op audit is retired:
	// private void SeedMedievalHouseholdCraftTools() { }
	private void SeedMedievalHouseholdCraftTools()
	{
		// Medieval industry, workshop, and craft-tool catalogue
		CreateItem
		(
			"medieval_workshop_forge",
			"forge",
			"an unlit smithing forge",
			null,
			"This low clay-and-stone forge has a charcoal basin, tuyere mouth, and battered working lip for heating small bars, fittings, tools, and weapon stock. Soot stains the rim and the packed hearth floor around it. It is made to take bellows air rather than to serve as an ordinary domestic fire.",
			SizeCategory.Large,
			ItemQuality.Standard,
			42000.0,
			120.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Forge" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_lit_forge",
			"forge",
			"a lit smithing forge",
			null,
			"This smithing forge glows with a bed of charcoal heat, the air above it wavering from steady bellows draught. The clay throat and stone rim are scorched black from repeated hot work. It is ready for ordinary forge welding, small tools, hardware, blades, and metal stock.",
			SizeCategory.Large,
			ItemQuality.Standard,
			42300.0,
			128.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Forge", "Functions / Material Functions / Hot Fire" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			"medieval_workshop_forge",
			"$0 burns low and settles back into $1.",
			TimeSpan.FromHours(3.0),
			null
		);

		CreateItem
		(
			"medieval_workshop_smelting_furnace",
			"furnace",
			"an unlit bloomery furnace",
			null,
			"This squat bloomery furnace is built from clay, stone, and repaired refractory lining, with a charging mouth above and a slag channel near the base. The tuyere socket shows where bellows can drive air into the charcoal and ore charge. It is workshop-scale primary industry apparatus rather than domestic heating.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			88000.0,
			180.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Smelting Furnace" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_lit_smelting_furnace",
			"furnace",
			"a lit bloomery furnace",
			null,
			"This bloomery furnace burns hot through its clay shaft, with charcoal glare showing at the charging mouth and slag crusted near the lower channel. The tuyere mouth is set for bellows air and the lining is darkened from repeated ore charges. It is in its working hot state for smelting rather than ordinary household fire.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			88400.0,
			190.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Smelting Furnace", "Functions / Tools / Smelting Tools / Smelting Furnace / Lit Smelting Furnace", "Functions / Material Functions / Hot Fire" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			"medieval_workshop_smelting_furnace",
			"$0 cools and dies back into $1.",
			TimeSpan.FromHours(5.0),
			null
		);

		CreateItem
		(
			"medieval_workshop_lime_kiln",
			"kiln",
			"a small lime kiln",
			null,
			"This upright kiln has a stone lower draw arch, a clay-lined shaft, and a blackened firing mouth for burning limestone, chalk, or shell into quicklime. Chips of pale stone and ash cling around the base. It is scaled for workshop and estate production rather than a town-sized kiln.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			96000.0,
			160.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Primary Production Tools / Lime Kiln", "Functions / Tools / Kiln Tool" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_glass_glory_hole",
			"furnace",
			"an unlit glass glory hole",
			null,
			"This rounded furnace chamber is lined with pale refractory clay and has a working opening sized for reheating gathered glass. Tool marks, old glass trails, and ash stain the mouth. It is meant for glass shaping alongside a batch furnace and annealing lehr.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			78000.0,
			170.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Glory Hole" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_annealing_lehr",
			"lehr",
			"an unlit annealing lehr",
			null,
			"This long low lehr has a clay-lined tunnel and a soot-dark opening for cooling finished glass gradually. The working shelf is worn smooth from bottles, beads, cups, and small panes being set inside. It is glasshouse apparatus, not a cooking oven.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			72000.0,
			150.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Annealing Lehr" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_ropewalk",
			"ropewalk",
			"a timber ropewalk frame",
			null,
			"This long timber frame carries fixed hooks, a turning handle, and spaced guide points for laying cordage under tension. The rails are rubbed smooth where hemp and flax ropes have passed along them. It folds only crudely and is best treated as workshop apparatus.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			28000.0,
			95.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Ropemaking Tools / Rope Hook" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_fulling_stocks",
			"stocks",
			"a pair of fulling stocks",
			null,
			"These heavy wooden fulling stocks are built with stout hammers, a trough bed, and pegged framing for pounding wool cloth after soaking. Their striking faces are smoothed by use and the trough smells faintly of old wool and alkaline liquor. They represent heavier fulling infrastructure than a hand trough.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			46000.0,
			130.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Fulling Stocks" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_papermakers_vat",
			"vat",
			"a papermaker's vat",
			null,
			"This broad timber vat is stained by pale pulp and sized for dipping a mould and deckle. Pegged hoops hold the staves tight and a low rim gives room to couch wet sheets without spilling the slurry. It is made for rag-paper production rather than dyeing or brewing.",
			SizeCategory.Large,
			ItemQuality.Standard,
			34000.0,
			80.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Papermaker's Vat" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_rag_beating_trough",
			"trough",
			"a rag-beating trough",
			null,
			"This deep wooden trough has a scarred inner bed and a broad lip for beating soaked rags into paper pulp. Old fibres are caught in the seams and the rim is darkened by repeated wet use. It is a papermaker's preparation vessel rather than an animal trough.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			48.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Rag Beating Trough" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_book_press",
			"press",
			"a wooden book press",
			null,
			"This stout book press has two flat wooden boards, a pair of screw posts, and dark stains from glue, leather, and inked quires. The pressure faces are broad enough for ordinary codices and ledgers. Wedges and screw handles show its use for binding rather than for fruit or oil.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			110.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Book Press" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_lying_press",
			"press",
			"a bookbinder's lying press",
			null,
			"This narrow lying press has paired wooden cheeks and a screw tightening bar for holding a book spine while trimming, backing, or sewing. The jaws are lined with stained leather to keep boards from slipping. It is compact but clearly a specialist binding tool.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			70.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Lying Press" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_bookbinders_sewing_frame",
			"frame",
			"a bookbinder's sewing frame",
			null,
			"This upright frame holds cords under tension between a base and crossbar for sewing folded quires. The wood is grooved by thread and marked with small knife cuts from previous bindings. It is light enough to move but belongs on a binder's bench.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			48.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_pole_lathe",
			"lathe",
			"a pole-lathe frame",
			null,
			"This timber pole lathe has a simple bed, treadle cord, sprung pole, and paired centres for turning pegs, handles, bowls, beads, and small round furniture parts. The tool rest is nicked and dark from use. It is workshop apparatus rather than an ordinary piece of furniture.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			85.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Lathe" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_bake_oven",
			"oven",
			"a clay bake oven",
			null,
			"This domed clay oven has a low arched mouth, a soot-black roof inside, and a worn stone sill for raking coals and sliding loaves or pies. The outside is patched with straw-tempered clay. It is meant for baking and drying rather than direct forge work.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			68000.0,
			90.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Cooking / Cookware / Bakeware" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_brew_copper",
			"copper",
			"a bronze brew copper",
			null,
			"This large bronze brewing copper has a hammered belly, a broad rolled rim, and heavy lugs for hanging or seating over a furnace. The inside is stained by wort, herbs, and boiled decoctions. It serves brewers and apothecaries wherever large heated liquids are prepared.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			150.0m,
			false,
			false,
			"bronze",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Brewing Tools / Brew Copper" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_mash_tun",
			"tun",
			"a wooden mash tun",
			null,
			"This coopered mash tun is broad, deep, and darkly stained by grain liquor. Its staves are hoop-bound and a fitted wooden cover rests across the top. It is made for brewing grain mashes rather than ordinary dry storage.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			60.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Brewing Tools / Mash Tun" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_workshop_fermenting_gyle_tun",
			"tun",
			"a fermenting gyle tun",
			null,
			"This open-topped gyle tun is made from tight oak staves and carries a faint sour smell from old fermenting ale. The rim is broad enough to skim and the base is stained by repeated brews. It is a brewing vessel, not a general household tub.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16500.0,
			52.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Brewing Tools / Fermenting Gyle Tun" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_charcoal_rake",
			"rake",
			"a long charcoal rake",
			null,
			"This long-handled iron rake has a narrow hooked head for drawing charcoal, clinker, and ash across a hot hearth or furnace mouth. The haft is darkened near the head from heat and smoke. It is too coarse for gardening and clearly belongs near kilns and furnaces.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1350.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Charcoal Burning Tool" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_ore_crusher",
			"crusher",
			"a stone ore crusher",
			null,
			"This heavy stone crusher is set with an iron-shod striking face and a shallow anvil block for breaking ore, slag, cullet, and hard pigment stones. Dust sits in the crevices around the working face. It is a rough preparation tool, not a finished mill.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			18500.0,
			45.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Ore Crusher" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_ore_roaster",
			"roaster",
			"a shallow ore-roasting pan",
			null,
			"This broad iron pan has low sides and a scarred base for spreading crushed ore over heat before smelting. Mineral stains and slaggy crusts cling to the corners. It is made for roasting charges, not for food or domestic work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			38.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Ore Roaster" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_charging_bucket",
			"bucket",
			"an iron charging bucket",
			null,
			"This iron bucket has a reinforced rim, a bale handle, and blackened sides from furnace work. It is sized for lifting ore, charcoal, flux, glass batch, or clay charges without scattering them across the floor. The base is dented from being set near hot brick and stone.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2400.0,
			22.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Charging Bucket" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_slag_hammer",
			"hammer",
			"a slag-breaking hammer",
			null,
			"This iron hammer has a squat head, one narrow peen, and a scarred wooden haft for breaking slag cakes and bloom crust. The striking face is chipped from mineral work. It is a furnace tool rather than a carpenter's mallet or weapon hammer.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Slag Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_slag_skimmer",
			"skimmer",
			"a long slag skimmer",
			null,
			"This long iron skimmer has a flattened perforated end for drawing slag, clinker, and floating dross away from a hot charge. The shaft is pitted from heat and handled with wrapped grips at the far end. It is built for furnace work, not kitchen use.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1450.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Slag Skimmer" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_tap_rod",
			"rod",
			"an iron furnace tap rod",
			null,
			"This long iron rod has a narrow chisel-like end for clearing slag channels, tapping furnace mouths, and opening clogged drains in hot mineral work. The handle end is wrapped in stained leather. Its length and heat marks make its purpose unmistakable.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Tap Rod" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_crucible",
			"crucible",
			"a ceramic crucible",
			null,
			"This thick ceramic crucible is darkened around the lip and vitrified in places from repeated hot work. Its rounded body and small pouring mouth suit metal, pigment, or glass batches. Old flux glass clings to the lower curve.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			16.0m,
			false,
			false,
			"ceramic",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Crucible" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Glassware" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_crucible_tongs",
			"tongs",
			"a pair of crucible tongs",
			null,
			"These long iron tongs have curved jaws sized to cradle a hot crucible without crushing it. The arms are heat-blued near the hinge and wrapped at the grips. They are more delicate than ordinary forge tongs but still made for serious heat.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Crucible Tongs" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_lye_leaching_barrel",
			"barrel",
			"an ash-lye leaching barrel",
			null,
			"This upright wooden barrel has a perforated false bottom, a dark drain plug, and ash stains down its sides. It is meant to leach potash or soda-rich ash into lye for washing, textile preparation, and workshop chemistry. The interior smells faintly alkaline rather than of drink or food.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13000.0,
			38.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Primary Production Tools / Leaching Tub", "Functions / Tools / Alkali Tool" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_felling_axe",
			"axe",
			"an iron felling axe",
			null,
			"This iron axe has a broad cutting bit, a poll scarred by wedges, and a long ash haft polished by two-handed work. The edge is ground for cutting green timber rather than for battle. Pitch and bark marks cling near the eye.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Felling Axe" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Weapon" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_splitting_axe",
			"axe",
			"an iron splitting axe",
			null,
			"This heavy iron axe has a wedge-thick head and a stout ash haft for cleaving rounds, billets, and stave stock. The bit is not fine; it is built to force wood apart. Old resin darkens the handle below the head.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			30.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Splitting Axe" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Weapon" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_adze",
			"adze",
			"an iron woodworking adze",
			null,
			"This iron adze is hafted across a curved wooden handle, its cutting edge set at right angles for dressing boards, hollowing timber, and trimming rough carpentry. The haft is dark and slightly bowed from use. It belongs to joiners, coopers, and timber workers.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			28.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Adze" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_drawknife",
			"drawknife",
			"an iron drawknife",
			null,
			"This drawknife has a straight iron blade between two turned wooden handles. Its edge is suited to pulling shavings from staves, shafts, spokes, handles, and bow staves. Bright scrape marks run along the blade where it has been repeatedly honed.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_spokeshave",
			"spokeshave",
			"an iron-edged spokeshave",
			null,
			"This small two-handled tool holds a narrow iron edge in a wooden body, made for rounding spokes, pegs, bows, shafts, and curved handle stock. The sole is worn smooth from passing over wood. It is a finer shaping tool than a drawknife.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			14.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_hand_saw",
			"saw",
			"an iron hand saw",
			null,
			"This iron hand saw has a tapered blade, filed teeth, and a riveted wooden grip. It is sized for boards, pegs, small beams, and furniture work rather than felling trees. The blade bears shallow scratches from repeated sharpening.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			26.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Hand Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_bow_saw",
			"saw",
			"a wooden bow saw",
			null,
			"This saw holds a narrow iron blade under tension in a curved wooden frame. A twisted cord and toggle draw the frame tight for controlled cuts in boards, curves, and small billets. The wood is polished where a hand grips the frame.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			24.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Bow Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_forest_saw",
			"saw",
			"a two-handled forest saw",
			null,
			"This long iron saw has a wooden grip at each end and large teeth for timber work. The blade is darkened by sap, oil, and filing marks. It is made to be pulled by two workers through logs or large beams.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3200.0,
			48.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Forest Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_fine_saw",
			"saw",
			"a fine-toothed cabinet saw",
			null,
			"This small iron saw has a stiffened back, fine teeth, and a smooth wooden grip. It is meant for boxes, inlay, book boards, pegs, and small cabinet joints where a rough saw would tear the work. The blade is kept bright and lightly oiled.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Woodcrafting Tools / Saws / Fine Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_wood_chisel",
			"chisel",
			"an iron wood chisel",
			null,
			"This narrow iron chisel has a bevelled edge and a faceted boxwood handle, kept sharp for mortises, hinge seats, carved fittings, and fine trimming. The handle end is bruised from mallet blows. It is a joiner's tool rather than a stone chisel.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Chisel" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_wood_auger",
			"auger",
			"an iron wood auger",
			null,
			"This iron auger has a spoon-like bit, a cross handle, and darkened spirals from boring peg holes, hinge sockets, bung holes, and wheel or frame joints. The handle is worn smooth by both palms. It is compact but forceful.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			22.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Auger" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_wood_file",
			"file",
			"an iron wood file",
			null,
			"This coarse iron file has a tang set into a simple wooden handle and a face cut for shaping wood, horn, bone, and soft stone. Its teeth are clogged with pale dust and resin. It is used for fairing rough curves after knife or saw work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			14.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood File" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_wood_clamp",
			"clamp",
			"a wedge-keyed wood clamp",
			null,
			"This wooden clamp has opposing jaws, peg holes, and a wedge key for holding boards, glued frames, shield layers, and book boards under pressure. Glue and pitch have darkened the inner faces. It is crude but reliable workshop hardware.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			16.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Clamp" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_coopers_adze",
			"adze",
			"a cooper's iron adze",
			null,
			"This short-handled adze has a curved iron bit shaped for hollowing the inner faces of staves and tubs. Its handle is slick from wet barrel work and its edge is ground with a tight curve. It is more specialised than an ordinary timber adze.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			28.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Cooper's Adze" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_coopers_jointer",
			"jointer",
			"a cooper's jointer plane",
			null,
			"This long wooden jointer holds a straight iron blade for truing barrel staves to meet cleanly at their edges. The bed is polished from repeated passes and faint hoop marks cross the handle. It is a cooper's bench tool rather than a carpenter's general plane.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			30.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_croze",
			"croze",
			"a cooper's croze",
			null,
			"This croze has a curved wooden body and a small iron cutter for cutting the grooves that receive cask heads and bucket bottoms. Its fence is polished by repeated contact with stave rims. The tool is compact but visibly specialised.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			22.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Croze" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_hoop_driver",
			"driver",
			"a wooden hoop driver",
			null,
			"This blunt wooden driver has a broad striking face and a hand-smoothed grip for walking hoops down over staves. Its nose is battered from iron and wooden hoops alike. It is simple, heavy, and made to be struck with a mallet.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			8.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Hoop Driver" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_bung_borer",
			"borer",
			"an iron bung borer",
			null,
			"This iron-edged borer is set into a wooden brace for opening neat round holes in casks, tubs, and brewing vessels. The bit is dark with tar, pitch, and damp oak dust. It is a cooper's tool for closures and taps.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			20.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Bung Borer" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_distaff",
			"distaff",
			"a carved wooden distaff",
			null,
			"This light wooden distaff is notched near the top to hold flax, wool, hemp, or cotton fibre ready for spinning. The shaft is slim enough to tuck under an arm or stand beside a spindle. Smooth handling marks darken the middle.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			240.0,
			6.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Spinning Tools / Distaff" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_wool_combs",
			"combs",
			"a pair of iron wool combs",
			null,
			"These paired wool combs have rows of iron teeth fixed into wooden handles for drawing locks of fleece into straight fibre. The teeth are blunt-tipped but formidable and the handles are polished by greasy wool. They are textile tools, not grooming combs.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1100.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Flax Processing Tools / Fibre Comb" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_flax_hackle",
			"hackle",
			"an iron flax hackle",
			null,
			"This flax hackle is a wooden board set with sharp iron teeth for drawing flax or hemp fibre clean after breaking and scutching. Tow fibres cling between the teeth. It is a stationary textile-preparation tool with a dangerous-looking working face.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			26.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Flax Processing Tools / Hackle" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_scutching_knife",
			"knife",
			"a wooden scutching knife",
			null,
			"This flat wooden blade is used to scrape and beat broken flax or hemp stems away from the useful fibre. Its edge is blunt but broad and the handle is darkened by retted stalks. It belongs to fibre preparation, not kitchen cutting.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			4.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Flax Processing Tools" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_niddy_noddy",
			"winder",
			"a wooden skein winder",
			null,
			"This niddy-noddy has a central wooden bar with crosspieces at either end for winding thread or yarn into measured skeins. The corners are rounded from repeated wrapping. It is light, simple, and essential to keeping spun yarn orderly.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			5.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_warping_board",
			"board",
			"a pegged warping board",
			null,
			"This flat wooden board is set with rows of smooth pegs for measuring and arranging warp threads before they go to a loom. The peg tops are polished by yarn. Inked notches and knife marks show common warp lengths.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			20.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Warping Board" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_shuttle",
			"shuttle",
			"a smooth wooden weaving shuttle",
			null,
			"This narrow wooden shuttle is notched to carry weft thread smoothly through a shed. Its sides are polished by repeated passes across warp threads. It is plain but carefully rounded so it will not snag fine cloth.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			4.0m,
			false,
			false,
			"boxwood",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Shuttle" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_beater_batten",
			"batten",
			"a wooden beater batten",
			null,
			"This flat wooden batten has a smooth working edge for beating weft firmly into a woven cloth. The handle is darker where it has been gripped. It is a simple loom tool, larger than a shuttle and lighter than a sword-like beater.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			5.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Beater Batten" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_heddle_rod",
			"rod",
			"a smooth heddle rod",
			null,
			"This straight wooden heddle rod is polished smooth and sized to carry loops that lift selected warp threads on a loom. Cord marks run along its length. It is modest but important loom furniture.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			6.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Heddle Rod" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_lease_rod",
			"rod",
			"a paired lease rod",
			null,
			"These paired smooth rods keep the cross in a prepared warp so the threads stay in order. Their ends are cord-bound to prevent splitting and their middles are rubbed clean by yarn. They are plain loom-preparation tools.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			7.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Lease Rod" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_weaving_reed",
			"reed",
			"a framed weaving reed",
			null,
			"This framed weaving reed holds many narrow dents in a wooden frame for spacing warp threads and beating cloth evenly. The reed is stained by wool oil and plant fibre. It is delicate enough to be kept wrapped when not in use.",
			SizeCategory.Normal,
			ItemQuality.Good,
			850.0,
			24.0m,
			false,
			false,
			"reed",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Weaving Reed" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_tablet_weaving_cards",
			"cards",
			"a set of tablet-weaving cards",
			null,
			"These small square cards are cut from bone and drilled at the corners for turning patterned bands. The edges are smoothed so they will not fray thread. They are tied together on a cord when stored.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			12.0m,
			false,
			false,
			"bone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Tablet Weaving Cards" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_dye_strainer",
			"strainer",
			"a linen dye strainer",
			null,
			"This tight linen strainer is stretched over a willow hoop for separating plant matter, lake pigment, and grit from a dye bath. Its cloth is permanently stained in muted layers of brown, red, and blue. It dries stiff at the edges after use.",
			SizeCategory.Small,
			ItemQuality.Standard,
			100.0,
			5.0m,
			false,
			false,
			"linen",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Strainer" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_mordant_cauldron",
			"cauldron",
			"a mordant cauldron",
			null,
			"This bronze cauldron has a rounded belly, soot-black base, and mineral stains around the rim from alum, iron, and copper mordants. Its handles are heavy enough for workshop lifting. It is visibly a dyer's vessel rather than kitchen cookware.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			75.0m,
			false,
			false,
			"bronze",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Mordant Cauldron" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_dye_stirring_pole",
			"pole",
			"a stained dye-stirring pole",
			null,
			"This long wooden pole is smoothed by use and stained in uneven dark bands from stirring vats of dyed yarn and cloth. One end is flattened into a paddle shape for lifting submerged textile. It smells faintly of wet wool and mordant.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			6.0m,
			false,
			false,
			"ash",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Stirring Pole" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_skein_rack",
			"rack",
			"a wooden skein rack",
			null,
			"This light rack has projecting pegs and crossbars for hanging skeins after washing or dyeing. The lower rails are blotched with dye and the pegs are polished by damp yarn loops. It folds only awkwardly and belongs in a textile workshop.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6200.0,
			28.0m,
			false,
			false,
			"ash",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Skein Rack" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_dye_drying_line",
			"line",
			"a portable dye-drying line",
			null,
			"This coil of hemp line comes with wooden pegs and short hooked stakes for suspending dyed skeins, narrow cloth, or small garments while they dry. The cord is permanently marked with old red, yellow, and blue stains. It is a workshop drying aid, not a finished textile good.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			8.0m,
			false,
			false,
			"hemp",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Drying Line" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_fullers_trough",
			"trough",
			"a fuller's wooden trough",
			null,
			"This broad trough is pegged from heavy boards and stained by wet wool, fuller's earth, and alkaline liquor. The bottom is worn smooth by feet, mallets, and soaked cloth. It is sized for hand fulling rather than livestock watering.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			46.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Fuller's Trough" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_fullers_mallet",
			"mallet",
			"a fuller's wooden mallet",
			null,
			"This heavy wooden mallet has a broad rounded head and a sturdy haft for beating soaked wool cloth. The working face is worn smooth and slightly cupped by repeated blows. It is too blunt and heavy for fine joinery.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			8.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Fuller's Mallet", "Functions / Tools / Striking Tools / Mallet" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_tenter_frame",
			"frame",
			"a cloth tenter frame",
			null,
			"This wooden frame has rows of small hooks and pegged adjustment holes for stretching damp fullered cloth to width. Rust marks and wool fibres cling along the hook line. It is a finishing frame rather than a display rack.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			18000.0,
			58.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Cloth Tenter Frame" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_teasel_frame",
			"frame",
			"a teasel nap-raising frame",
			null,
			"This light wooden frame is set to hold dried teasels for raising the nap of wool cloth. Loose burrs and fine wool lint cling to its crossbars. It is a textile finishing aid, not a comb for animals or hair.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			18.0m,
			false,
			false,
			"ash",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Teasel Frame" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_napping_shears",
			"shears",
			"a pair of broad napping shears",
			null,
			"These broad iron shears have long springy arms and a wide bite for clipping the raised nap of finished wool. Their blades are polished from cloth work rather than kitchen or hair use. They are heavy enough to require careful handling.",
			SizeCategory.Small,
			ItemQuality.Good,
			980.0,
			38.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Napping Shears", "Functions / Separation / Shearing / Shears" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_reed_splitter",
			"splitter",
			"an iron reed splitter",
			null,
			"This small iron splitter has several narrow fins radiating from a tapered point, made to divide reeds, willow rods, or cane into even splints. It fits neatly in the hand and is darkened by plant sap. Basketmakers and mat weavers would know it at once.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			9.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Basketry Tools / Reed Splitter" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_basket_knife",
			"knife",
			"an iron basket knife",
			null,
			"This small iron knife has a hooked tip and a stout handle for trimming willow, reed, bark, cane, and split splints. Its edge is sharpened for plant material rather than meat. A smear of old sap darkens the heel.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			10.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Basketry Tools / Basket Knife" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_weaving_bodkin",
			"bodkin",
			"a polished bone weaving bodkin",
			null,
			"This smooth bone bodkin is shaped to open tight woven courses, tuck splint ends, and draw bindings through basketry, straps, or heavy cloth. Its rounded point will not cut fibres. The middle is glossy from repeated handling.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			5.0m,
			false,
			false,
			"bone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Basketry Tools / Weaving Bodkin" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_rope_hook",
			"hook",
			"an iron ropemaking hook",
			null,
			"This stout iron hook is set into a short wooden bar for holding cordage under tension while strands are twisted together. The bend is polished bright where hemp and flax have bitten into it. It is simple, rugged, and workshop-made.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_rope_top",
			"top",
			"a grooved ropemaking top",
			null,
			"This conical wooden top is cut with smooth grooves that keep strands apart as they are laid into rope. The grooves are dark with fibre dust and wax. It is used with hooks and tension rather than as a toy.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			6.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_marlinespike",
			"spike",
			"an iron marlinespike",
			null,
			"This tapered iron spike has a rounded head and a smooth shank for opening knots, splices, and tight cordage. The point is blunt enough not to cut rope fibres. It is a ropeworker's tool rather than a weapon point.",
			SizeCategory.Small,
			ItemQuality.Standard,
			310.0,
			8.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_skinning_knife",
			"knife",
			"an iron skinning knife",
			null,
			"This short iron knife has a curved belly, a plain wooden grip, and a keen edge for opening hides away from a carcass. The tip is strong but not needle-sharp. It is a butcher's field tool rather than a table knife.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			12.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Butcher Tools / Field Dressing Tools / Skinning Knife" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Weapon" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_fleshing_knife",
			"knife",
			"a two-handled fleshing knife",
			null,
			"This broad iron blade has a handle at each end so it can be drawn over a hide on a beam. The edge is rounded enough to scrape flesh and fat without cutting deep holes. Grease has darkened the wooden grips.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Butcher Tools / Field Dressing Tools / Fleshing Knife" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_hide_scraper",
			"scraper",
			"an iron hide scraper",
			null,
			"This iron scraper has a broad slightly curved edge set into a plain wooden handle for cleaning hair, flesh, and old membrane from hides. The blade is duller than a knife but polished bright along the working curve. It smells faintly of lime and tallow.",
			SizeCategory.Small,
			ItemQuality.Standard,
			380.0,
			12.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Tanning Tools / Hide Scraper" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
		);

		CreateItem
		(
			"medieval_tool_tanning_beam",
			"beam",
			"a sloped tanning beam",
			null,
			"This rounded wooden beam is set at a working angle and polished smooth where hides are drawn over it for scraping. Dark streaks of lime, oil, and old flesh mark the lower end. It is heavy enough to stay put during hard leather work.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			34.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Tanning Tools / Tanning Beam" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_dehairing_knife",
			"knife",
			"a blunt dehairing knife",
			null,
			"This broad iron knife has a deliberately blunt scraping edge for pushing loosened hair from limed hides. Both ends have simple grips so it can be drawn with even pressure. It is marked by pale lime residue along the spine.",
			SizeCategory.Small,
			ItemQuality.Standard,
			600.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Tanning Tools / Leather Dehairing Knife" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_tanning_paddle",
			"paddle",
			"a broad tanning paddle",
			null,
			"This long wooden paddle has a flat stained blade for stirring hides in bark liquor, oil baths, or rinse vats. The handle is darkened by tannin and the end is battered from pushing heavy wet skins. It is too large for ordinary kitchen use.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			7.0m,
			false,
			false,
			"ash",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Tanning Tools / Tanning Paddle" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_currying_knife",
			"knife",
			"a curved currying knife",
			null,
			"This curved iron currying knife is made to dress, scrape, and finish leather after tanning. The working edge is smooth and controlled rather than keen. Oil stains darken the grip and back of the blade.",
			SizeCategory.Small,
			ItemQuality.Standard,
			340.0,
			14.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Leatherworking Tools" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_saddlers_clamp",
			"clamp",
			"a saddler's stitching clamp",
			null,
			"This wooden clamp has long jaws and a footed base for holding straps, shoes, harness pieces, and saddle leather while both hands work the seam. Dark wax and thread marks line the inner jaws. It is a leatherworker's bench tool.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			22.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Leatherworking Tools / Leather Stitching Pony" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_leather_creaser",
			"creaser",
			"an iron leather creaser",
			null,
			"This small iron creaser has a rounded working tip and a wooden handle for marking straight lines along leather straps, belts, covers, and scabbards. The tool is polished where it has been drawn against damp leather. It makes visible guide lines rather than cutting.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			8.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Leatherworking Tools / Leather Creaser" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_burnisher",
			"burnisher",
			"a polished bone burnisher",
			null,
			"This smooth bone burnisher is shaped with several rounded grooves for rubbing edges, parchment, leather seams, and small bone or horn pieces to a compact sheen. The middle is glossy from grip wear. It is simple but carefully finished.",
			SizeCategory.Small,
			ItemQuality.Standard,
			95.0,
			6.0m,
			false,
			false,
			"bone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Burnisher" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_bone_saw",
			"saw",
			"a fine iron bone saw",
			null,
			"This narrow iron saw has fine teeth and a short wooden handle for cutting bone, horn, antler, and small hard blanks. Pale dust clings to the teeth. It is finer than a butcher's splitting saw and sturdier than a cabinet saw.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Butcher Tools / Meat Cutting Tools / Bone Saw" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_bone_file",
			"file",
			"a small boneworker's file",
			null,
			"This small iron file has a narrow face and a simple handle for refining bone, horn, shell, and soft stone blanks. The teeth hold pale dust from earlier work. It is meant for detail fitting rather than timber shaping.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			12.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood File" ],
			[ "Holdable", "Tool_Leatherworking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_parchment_stretching_frame",
			"frame",
			"a parchment stretching frame",
			null,
			"This rectangular wooden frame is pierced with peg holes and fitted with cords for drawing a wet skin tight while it dries into parchment. Chalk dust and knife marks cover the rails. It is too specialised to mistake for a picture frame.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8800.0,
			44.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Parchmentmaking Tools / Parchment Stretching Frame" ],
			[ "Holdable", "Tool_Parchmentmaking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_parchment_lunellum",
			"lunellum",
			"a crescent parchment lunellum",
			null,
			"This crescent-shaped iron scraping knife has a short central grip and a smooth curved edge for shaving stretched parchment. Its edge is duller than a cutting knife but polished along the working curve. Chalk and skin dust remain in the handle seam.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Parchmentmaking Tools / Parchment Lunellum" ],
			[ "Holdable", "Tool_Parchmentmaking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_parchment_pumice",
			"pumice",
			"a block of parchment pumice",
			null,
			"This pale porous pumice block is worn flat on one side from smoothing parchment and paper surfaces. Chalky dust sits in its pits and a thong loop keeps it from being lost. It is a surface-preparation tool, not a bathing stone.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			4.0m,
			false,
			false,
			"pumice",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Parchmentmaking Tools / Parchment Pumice" ],
			[ "Holdable", "Tool_Parchmentmaking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_pounce_bag",
			"bag",
			"a chalk-filled pounce bag",
			null,
			"This small linen bag is filled with fine chalky pounce for preparing parchment and paper before writing. Its surface is whitened by powder and tied with a simple cord. It is meant to dust a surface, not to carry valuables.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			3.0m,
			false,
			false,
			"linen",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Parchmentmaking Tools / Pounce Bag" ],
			[ "Holdable", "Tool_Parchmentmaking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_smithing_punch_set",
			"punches",
			"a set of smithing punches",
			null,
			"This roll of iron punches includes round, square, and narrow points for driving holes, forming rivet seats, and marking hot work. The heads are upset from hammering and the points are kept greased. A rough leather wrap keeps the set together.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			36.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_drawplate",
			"drawplate",
			"an iron drawplate",
			null,
			"This thick iron plate is pierced by rows of progressively smaller holes for drawing wire from rods of soft metal. The face is polished in rings around the holes and darkened by oil. It is useful to smiths, jewellers, and mail makers.",
			SizeCategory.Small,
			ItemQuality.Good,
			1600.0,
			80.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Jewellery Tools / Jeweller's Drawplate", "Consumables / Drawplate" ],
			[ "Holdable", "Tool_Jewellery_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_grindstone",
			"grindstone",
			"a treadled grindstone",
			null,
			"This round stone wheel sits in a wooden frame with a simple treadle and water trough for sharpening blades, tools, needles, and armour edges. The rim is worn into a shallow curve from repeated use. Grey slurry stains the frame below it.",
			SizeCategory.Large,
			ItemQuality.Standard,
			34000.0,
			65.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Weaponsmithing Tools / Grindstone", "Functions / Sharpening" ],
			[ "Holdable", "Tool_Weaponsmithing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_whetstone",
			"whetstone",
			"a grooved sharpening whetstone",
			null,
			"This palm-sized stone is worn into shallow grooves by knives, chisels, needles, and small blades. It is kept damp or oiled when used and has a small hole for a thong. It is the common finishing companion to larger grinding tools.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			220.0,
			4.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Sharpening" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_quenching_trough",
			"trough",
			"an iron-bound quenching trough",
			null,
			"This long wooden trough is iron-bound and blackened around the rim, used to cool hot blades, tools, and fittings in water, oil, or brine. The inside is dark and slick from repeated quenching. It belongs beside a forge, not in a stableyard.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			48.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Weaponsmithing Tools / Quenching Trough" ],
			[ "Holdable", "Tool_Weaponsmithing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_fuller_tool",
			"fuller",
			"an iron fuller tool",
			null,
			"This iron fuller has a rounded working edge and a struck head for forming grooves and fuller lines in blades and bars. Hammer marks spread across the top while the working edge is kept smooth. It is a weaponsmith's shaping tool.",
			SizeCategory.Small,
			ItemQuality.Standard,
			480.0,
			22.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Weaponsmithing Tools / Fuller Tool" ],
			[ "Holdable", "Tool_Weaponsmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_tang_punch",
			"punch",
			"a narrow tang punch",
			null,
			"This narrow iron punch is shaped to open and refine tang holes, rivet seats, and grip fittings on weapon heads and blades. The struck end is mushroomed from hard use. It is carried with other smithing punches but shaped for weapon work.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			220.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Weaponsmithing Tools / Tang Punch" ],
			[ "Holdable", "Tool_Weaponsmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_sword_vise",
			"vise",
			"a wooden sword vise",
			null,
			"This long wooden vise has padded jaws, peg holes, and wedge keys for holding a blade or hilt steady while the grip, guard, or pommel is fitted. Cut marks and oil stains line the jaws. It is a specialist bench aid for weapon assembly.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			42.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Weaponsmithing Tools / Sword Vise" ],
			[ "Holdable", "Tool_Weaponsmithing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_crossguard_fixture",
			"fixture",
			"a crossguard fitting fixture",
			null,
			"This compact wooden-and-iron fixture has slots and pegs for seating a sword guard square against a blade shoulder while the hilt is assembled. Its working faces are scarred by files and light hammering. It is too specialised for ordinary carpentry.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Weaponsmithing Tools / Crossguard Fixture" ],
			[ "Holdable", "Tool_Weaponsmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_armourers_stake",
			"stake",
			"an armourer's iron stake",
			null,
			"This heavy iron stake has a tapered foot and several rounded working faces for shaping plates, scales, and helmet pieces. The surfaces are bright where hammer blows have worked metal over them. It is meant to be set in a stump or bench block.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8200.0,
			85.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Armouring Tools / Armourer's Stake" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_ball_stake",
			"stake",
			"a ball stake",
			null,
			"This iron stake ends in a polished rounded ball for doming shield bosses, cups, helmet plates, and raised metal fittings. The shaft below it is thick and battered from being mounted upright. It is a small but important armourer's form.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6400.0,
			72.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Armouring Tools / Ball Stake" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_dishing_form",
			"form",
			"a wooden dishing form",
			null,
			"This dense wooden block is hollowed into several shallow bowls for hammering plate, bosses, and lamellar curves into shape. The hollows are darkened and compressed by repeated blows. It is heavy, plain, and deeply practical.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7200.0,
			34.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Dishing Form" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_forming_bag",
			"bag",
			"an armourer's forming bag",
			null,
			"This heavy leather bag is packed tight with sand and grit so plate and boss blanks can be dished over it without sharp marks. The surface is scarred and dark from hammering. It is dense, soft-edged, and clearly a shaping support.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			12500.0,
			38.0m,
			false,
			false,
			"leather",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Armourer's Forming Bags" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_planishing_hammer",
			"hammer",
			"a polished planishing hammer",
			null,
			"This small iron hammer has a polished face for smoothing armour plates and sheet metal after rough forming. The head is lighter than a forge hammer and carefully dressed at the edges. Its haft is slim for controlled blows.",
			SizeCategory.Small,
			ItemQuality.Good,
			780.0,
			36.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Armouring Tools / Planishing Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_raising_hammer",
			"hammer",
			"an armourer's raising hammer",
			null,
			"This iron raising hammer has a rounded face and a narrow peen for lifting sheet metal into cups, bosses, and helmet bowls. The head is marked by controlled work rather than heavy forge blows. It belongs with stakes and dishing forms.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			28.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Raising Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_plate_snips",
			"snips",
			"a pair of iron plate snips",
			null,
			"These heavy iron snips have short jaws and long handles for cutting thin armour plate, sheet metal, scales, and lamella blanks. The pivot is dark with grease and the jaws are nicked from hard stock. They are too heavy for cloth.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1450.0,
			30.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Plate Snips" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_armourers_pliers",
			"pliers",
			"a pair of armourer's pliers",
			null,
			"These iron pliers have stout jaws for closing mail rings, gripping hot rivets, bending lamella lacing tabs, and handling small armour fittings. The grips are wrapped in dark leather. They are heavier than ordinary household pliers.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Armourer's Pliers", "Functions / Tools / Gripping Tools / Pliers" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_locksmithing_fabrication_kit",
			"kit",
			"a locksmith's fabrication kit",
			null,
			"This compact kit holds small files, warding picks, punches, gauges, tweezers, and narrow chisels for making locks, keys, and internal wards. The tools sit in a fitted leather roll with dark oil marks. It is meant for fabrication, not for picking a lock in a doorway.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			95.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Locksmithing Tools" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_locksmithing_installation_kit",
			"kit",
			"a locksmith's installation kit",
			null,
			"This small kit bundles files, awls, punches, screw-like spikes, wedges, and measuring strips for fitting locks, hasps, latches, and keys to chests or doors. The tools are stained by oak dust and iron filings. It is practical trade equipment rather than a burglary set.",
			SizeCategory.Small,
			ItemQuality.Good,
			1250.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Locksmithing Tools" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_jewellers_anvil",
			"anvil",
			"a small jeweller's anvil",
			null,
			"This small iron anvil has a bright flat face, a narrow horn, and a fitted timber base for shaping rings, brooch pins, settings, and small sheet ornaments. Tiny hammer marks cover the surface. It is much smaller and finer than a forge anvil.",
			SizeCategory.Small,
			ItemQuality.Good,
			2800.0,
			70.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Jewellery Tools / Jeweller's Anvil" ],
			[ "Holdable", "Tool_Jewellery_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_jewellers_crimping_pliers",
			"pliers",
			"a pair of jeweller's crimping pliers",
			null,
			"These fine iron pliers have small grooved jaws for closing crimps, clasps, wire loops, and setting tabs. The handles are thin and carefully wrapped for control. They are trade tools for jewellery work rather than general workshop gripping.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			170.0,
			45.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Jewellery Tools / Crimping Pliers" ],
			[ "Holdable", "Tool_Jewellery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_jewellers_burnisher",
			"burnisher",
			"a jeweller's polished burnisher",
			null,
			"This fine steel-bright burnisher has a rounded polished tip and a boxwood handle for closing settings, smoothing wire, and finishing small ornaments. Its working end is mirror-smooth. The tool is delicate but durable.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			80.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Jewellery Tools / Jeweller's Burnisher" ],
			[ "Holdable", "Tool_Jewellery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_lapidary_saw",
			"saw",
			"a bow-framed lapidary saw",
			null,
			"This small bow saw holds a thin abrasive blade for cutting shell, jet, amber, soft stone, and gem rough under grit and water. The frame is stained by polishing slurry. It is slow and precise rather than strong.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			60.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Lapidary Tools / Lapidary Saw" ],
			[ "Holdable", "Tool_Lapidary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_lapidary_wheel",
			"wheel",
			"a hand-turned lapidary wheel",
			null,
			"This small stone wheel sits in a wooden frame with a hand crank and shallow slurry trough for grinding beads, cabochons, shell, amber, and soft stone. The rim is smooth and stained by abrasive paste. It belongs to lapidary and jeweller's work.",
			SizeCategory.Normal,
			ItemQuality.Good,
			12000.0,
			95.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Lapidary Tools / Lapidary Wheel" ],
			[ "Holdable", "Tool_Lapidary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_drill_bow",
			"drill",
			"a bow drill with small bits",
			null,
			"This bow drill has a corded bow, hand block, spindle, and several small iron bits for boring beads, shell, bone, horn, and soft stone. The spindle is darkened by hand oil and polishing grit. It can serve jewellers, lapidaries, and woodworkers alike.",
			SizeCategory.Small,
			ItemQuality.Standard,
			460.0,
			22.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Bow Drill", "Functions / Tools / Lapidary Tools / Bead Drill" ],
			[ "Holdable", "Tool_Lapidary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_potters_wheel",
			"wheel",
			"a kick-driven potter's wheel",
			null,
			"This heavy wooden potter's wheel has a low kick wheel below and a smooth throwing head above. Clay slip fills the seams and the upper surface is worn into a faint dish. It is built for thrown vessels rather than hand moulding alone.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			60.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Potter's Wheel" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_potters_rib",
			"rib",
			"a wooden potter's rib",
			null,
			"This curved wooden rib is polished smooth by wet clay, with one broad face and one sharper edge for drawing vessel walls into even curves. It is light, flat, and easy to hold. Clay has dried pale along the grain.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			4.0m,
			false,
			false,
			"boxwood",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Potter's Rib" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_clay_knife",
			"knife",
			"a blunt clay knife",
			null,
			"This small wooden-handled knife has a blunt iron edge for cutting wet clay, trimming vessel rims, and cleaning the foot of a pot before firing. It is not sharpened like a kitchen knife. Dried slip cakes the shoulder of the blade.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			6.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Clay Knife" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_wire_cutter",
			"cutter",
			"a potter's wire cutter",
			null,
			"This simple cutter is a taut wire set between two small wooden grips for cutting clay lumps and freeing thrown pots from a wheel head. Clay crust and damp wear mark the wire ends. It is precise but fragile.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			60.0,
			4.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Wire Cutter" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_press_mold",
			"mould",
			"a fired-clay press mould",
			null,
			"This fired-clay mould has a shallow carved cavity for pressing tiles, lamps, relief panels, or repeated vessel parts. Dried clay sits in the corners and the outside is rough enough to grip. It is a tool for repeated forms rather than a finished decoration.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			18.0m,
			false,
			false,
			"fired clay",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Press Mold" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Glassware" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_clay_stamp",
			"stamp",
			"a carved clay stamp",
			null,
			"This small fired-clay stamp has a carved face for pressing simple marks, borders, or maker's signs into wet clay. The back is smoothed into a grip and the face holds traces of old slip. It is a compact potter's marking tool.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			95.0,
			6.0m,
			false,
			false,
			"fired clay",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Clay Stamp" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Glassware" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_masons_hammer",
			"hammer",
			"an iron mason's hammer",
			null,
			"This mason's hammer has a square striking face, a narrow pick, and a tough ash handle for dressing stone. Chips have whitened the head and the grip is dusty with lime and grit. It is balanced for stone rather than metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1300.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Stoneworking Tools / Bush Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_point_chisel",
			"chisel",
			"an iron point chisel",
			null,
			"This iron point chisel tapers to a strong narrow tip for roughing out stone blocks, slate, and masonry details. The struck end is battered and the working point is bright. It is not meant for wood or leather.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Stoneworking Tools / Stone Chisel" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_tooth_chisel",
			"chisel",
			"an iron tooth chisel",
			null,
			"This stone chisel has several small teeth across its cutting edge for refining rough-dressed stone after the point chisel. Dust lies between the teeth and the shaft is scarred by mallet blows. It leaves controlled parallel marks on workable stone.",
			SizeCategory.Small,
			ItemQuality.Standard,
			440.0,
			12.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Stoneworking Tools / Stone Chisel" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_masons_trowel",
			"trowel",
			"an iron mason's trowel",
			null,
			"This flat iron trowel has a pointed blade and a wooden handle for spreading mortar, plaster, limewash, and tile bedding. The blade is dulled by grit and pale streaks of lime cling to the ferrule. It is a construction tool, not kitchenware.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			8.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Construction Tools / Construction Trowel", "Functions / Tools / Finishing Tools / Trowel" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_masons_line",
			"line",
			"a mason's line and pins",
			null,
			"This small bundle holds a hemp line wrapped around two iron pins for setting straight courses, door openings, and wall faces. Lime dust and stone grit cling to the cord. It is simple but crucial layout equipment.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			160.0,
			5.0m,
			false,
			false,
			"hemp",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Layout Tools / Stringline" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_masons_square",
			"square",
			"a wooden mason's square",
			null,
			"This right-angled wooden square is reinforced with small iron plates and marked by chalk and stone dust. It is used to check block corners, door frames, and fitted masonry. The inner angle is carefully kept smooth and true.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			10.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Construction Tools / Construction Ruler" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_glass_blowpipe",
			"blowpipe",
			"an iron glassblowing pipe",
			null,
			"This long iron blowpipe has a narrow bore, a wrapped mouth end, and a darkened gathering end for taking molten glass from a furnace. The shaft is straight and carefully balanced. It belongs in a glasshouse, not a forge.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1400.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Blowpipe" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_pontil_rod",
			"rod",
			"an iron pontil rod",
			null,
			"This straight iron pontil rod has a rounded working end for supporting hot glass while the mouth or base is shaped. Small glass scars cling near the tip. The handle end is wrapped to spare the hand from heat.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Pontil Rod" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_marver_table",
			"table",
			"a smooth stone marver table",
			null,
			"This low stone-topped table has a flat polished surface for rolling, shaping, and cooling gathered glass. The edges are darkened by soot and the top is faintly iridescent from old glass contact. Its timber base is squat and steady.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			95.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Glassblowing Tools / Marver Table" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_glass_jacks",
			"jacks",
			"a pair of glassworker's jacks",
			null,
			"These iron jacks have long tapering blades and wrapped handles for opening, necking, and shaping hot glass. The tips are polished bright from contact with molten surfaces. They are delicate tools despite their heat-stained metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			45.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Glassblowing Tools / Jacks" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_glass_shears",
			"shears",
			"a pair of iron glass shears",
			null,
			"These iron shears have short strong blades and long handles for cutting hot glass necks, gathers, and waste trails. The blades are heat-dark and their pivot is kept loose. They are not sharpened like textile shears.",
			SizeCategory.Small,
			ItemQuality.Standard,
			740.0,
			28.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Glass Shears" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_glass_blocks",
			"blocks",
			"a set of wet wooden glass blocks",
			null,
			"These shaped wooden blocks are darkened by water and heat, hollowed to cup and smooth gathered glass as it turns. Each block has a handle and a charred inner curve. They must be kept damp when used near the furnace.",
			SizeCategory.Small,
			ItemQuality.Standard,
			980.0,
			22.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Blocks" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_mould_and_deckle",
			"mould",
			"a mould and deckle",
			null,
			"This papermaker's mould and deckle are built from a wooden frame, fine wire or reed screen, and a removable raised rim for forming rag-paper sheets. Pulp residue whitens the mesh and corners. It is carefully made so sheets dry close to square.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			50.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Papermaking Tools / Mould and Deckle" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_couching_blanket",
			"blanket",
			"a wool couching blanket",
			null,
			"This thick wool blanket is used to receive wet paper sheets from a mould before pressing. It is felted, absorbent, and permanently marked by pale pulp patches. The edges are bound in linen to slow fraying.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"wool",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Couching Blanket" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Clothing" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_press_felt",
			"felt",
			"a papermaker's press felt",
			null,
			"This dense felt sheet is cut to paper-press size and marked by countless wet sheets stacked against it. It is thick enough to absorb water while protecting the paper surface. The edges are darker where hands have lifted it from the pile.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			8.0m,
			false,
			false,
			"felt",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Press Felt" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Clothing" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_lay_press",
			"press",
			"a wooden lay press",
			null,
			"This low wooden press has flat boards and screw posts for pressing stacks of wet paper or sized sheets. Pulp, glue, and water stains mark the faces. It is lighter than a book press but built for repeated papermaking pressure.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			62.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Lay Press" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_rag_sorting_knife",
			"knife",
			"a rag-sorting knife",
			null,
			"This small iron knife has a rounded point and keen edge for cutting old linen, hemp, or cotton rags before soaking and beating. Fibres cling around the tang and the handle is nicked by work. It is a papermaker's preparation knife.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			8.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Rag Sorting Knife" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_paper_sizing_brush",
			"brush",
			"a paper-sizing brush",
			null,
			"This broad brush has a wooden back and densely tied bristles for laying size onto paper or parchment. Dried glue stiffens the bristle roots and the handle is pale from wet hands. It is for finishing sheets, not painting walls.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			12.0m,
			false,
			false,
			"wood",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Paper Sizing Brush" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_bookbinders_needle",
			"needle",
			"a bookbinder's iron needle",
			null,
			"This stout iron needle has a broad eye and polished shaft for sewing quires through fold holes and around support cords. It is larger than a clothing needle and less sharp at the point. Thread wear shines near the eye.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			3.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Bookbinder's Needle" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_bookbinders_punch",
			"punch",
			"a bookbinder's awl punch",
			null,
			"This small awl-like punch has a sharp iron point and a rounded wooden grip for opening holes through quires, parchment, leather covers, and thin book boards. The point is polished by paper dust. It is finer than a leather awl.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			6.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Bookbinder's Punch" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_backing_hammer",
			"hammer",
			"a bookbinder's backing hammer",
			null,
			"This small iron hammer has a broad polished face and rounded edges for shaping the spine of a sewn book without cutting the leather or parchment. Glue and leather dust mark the handle. It is a binding tool, not a smith's hammer.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Backing Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_leather_paring_knife",
			"knife",
			"a leather paring knife",
			null,
			"This thin iron knife has a slanted edge and a flat back for shaving leather covers, turn-ins, and spine pieces thin enough for bookbinding. The blade is keen but controlled. Dark leather dust marks the handle.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			130.0,
			9.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Leather Paring Knife" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_qalam_cutter",
			"knife",
			"a small qalam cutter",
			null,
			"This small iron knife has a short keen blade and narrow handle for trimming reed pens and qalams to a clean writing angle. Tiny reed shavings cling to the edge. It is kept with writing tools rather than cooking knives.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			70.0,
			12.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Calligraphy Tools / Qalam Cutter" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_quill_curing_sand",
			"sand",
			"a packet of quill-curing sand",
			null,
			"This small wrapped packet holds clean heated sand used to dry and harden quills before cutting them into pens. The outer parchment is pale and dusted with grit. It is a writing-workshop supply rather than ordinary floor sand.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			180.0,
			4.0m,
			false,
			false,
			"sand",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Calligraphy Tools / Quill Curing Sand" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_ruling_board",
			"board",
			"a ruled manuscript board",
			null,
			"This flat wooden board is marked with straight grooves, pinholes, and faint dark lines for ruling manuscript pages and quires. The edges are squared and smoothed for repeated alignment. It is a layout tool for scribes and binders.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			24.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Calligraphy Tools / Ruling Board" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_manuscript_pricker",
			"pricker",
			"a manuscript pricker",
			null,
			"This slender iron pricker has a sharp point and a small bone handle for marking guide holes before ruling pages. Its point is kept fine and straight. The tool is tiny but vital for regular manuscript layout.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			6.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Calligraphy Tools / Manuscript Pricker" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_block_carving_knife",
			"knife",
			"a woodblock carving knife",
			null,
			"This small iron knife has a fine angled edge for carving letters and images into a prepared woodblock. The handle is short and controlled, and the blade is kept bright. It is a printer's carving tool rather than a general knife.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			90.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Woodblock Printing Tools / Block Carving Knife" ],
			[ "Holdable", "Tool_Printing_Woodblock_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_block_clearing_chisel",
			"chisel",
			"a block-clearing chisel",
			null,
			"This narrow iron chisel has a small wooden handle and a flat edge for clearing blank areas from a carved printing block. Wood dust clings to the shoulder of the blade. It is made for careful relief carving, not heavy carpentry.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			10.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodblock Printing Tools / Block Clearing Chisel" ],
			[ "Holdable", "Tool_Printing_Woodblock_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_printing_baren",
			"baren",
			"a woven printing baren",
			null,
			"This round printing baren is made from woven fibre over a smooth backing disc for rubbing damp paper against an inked woodblock. The grip side is bound with cord and the working face is slightly glossy. It is lightweight but carefully made.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			140.0,
			14.0m,
			false,
			false,
			"bamboo",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodblock Printing Tools / Printing Baren" ],
			[ "Holdable", "Tool_Printing_Woodblock_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_ink_dauber",
			"dauber",
			"a cloth-bound ink dauber",
			null,
			"This small dauber has a cloth-wrapped pad tied over a short wooden handle for spreading ink or pigment across a carved block. The pad is dark and stiff from old ink. It is meant for printing rather than sealing wax.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			6.0m,
			false,
			false,
			"linen",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodblock Printing Tools / Ink Dauber" ],
			[ "Holdable", "Tool_Printing_Woodblock_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_impression_spoon",
			"spoon",
			"a polished impression spoon",
			null,
			"This smooth wooden spoon has a rounded back used to rub paper down over an inked block or incised surface. The bowl is polished by pressure rather than food. It is simple, quiet printing equipment.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			60.0,
			5.0m,
			false,
			false,
			"boxwood",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodblock Printing Tools / Impression Spoon" ],
			[ "Holdable", "Tool_Printing_Woodblock_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_flour_sieve",
			"sieve",
			"a framed flour sieve",
			null,
			"This round wooden sieve is stretched with fine woven mesh for sifting flour, meal, bran, powdered pigment, or fine dry medicines. Pale dust sits in the frame joints. It is a miller's and baker's tool as much as a kitchen implement.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			8.0m,
			false,
			false,
			"wood",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Milling Tools / Grain Sieve", "Functions / Tools / Cooking / Cooking Utensils / Sifter" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_kneading_trough",
			"trough",
			"a wooden kneading trough",
			null,
			"This long wooden trough has a smoothed interior for mixing and kneading dough, paste, or thick prepared stock. Flour dust sits in the corners and the outer boards are pegged tight. It is broad enough for a baker's batch rather than a table bowl.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			34.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Cooking / Cooking Utensils" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_salting_trough",
			"trough",
			"an oak salting trough",
			null,
			"This stout oak trough is darkened by salt, fish, and meat juices from preserving work. Its inner seams are tight and the base is slightly sloped for draining. It is meant for curing stock, not washing linen.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			40.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Butcher Tools / Meat Processing Tools" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_smoking_rack",
			"rack",
			"a blackened smoking rack",
			null,
			"This wooden rack is blackened by smoke and fitted with hooks and crossbars for hanging meat, fish, herbs, or hides above a low fire. The rails smell of salt and old smoke. It is portable enough to move but belongs near a smokehouse or hearth.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			28.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Butcher Tools / Meat Storage Tools / Hanging Rack" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_oil_press",
			"press",
			"a wooden oil press",
			null,
			"This screw press has a heavy wooden frame, broad pressing board, and stained catch channel for oilseed, nuts, or olives. The screw is dark with grease and old press cake. It is a workshop press, not a bookbinder's tool.",
			SizeCategory.Huge,
			ItemQuality.Good,
			52000.0,
			180.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Cooking / Cooking Utensils / Juice Press" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_fruit_press",
			"press",
			"a wooden fruit press",
			null,
			"This wooden screw press has slatted baskets, a broad pressure plate, and a trough for must, juice, syrups, or wine stock. Fruit stains and wax seal marks darken the frame. It is built for pressing soft produce rather than paper, cloth, or books.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			48000.0,
			135.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Cooking / Cooking Utensils / Juice Press" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_mashing_paddle",
			"paddle",
			"a broad mashing paddle",
			null,
			"This long wooden paddle has a broad pierced blade for stirring mash, wort, medicinal decoctions, or large grain mixtures. The blade is stained by malt and herbs. It is too long for ordinary table cooking.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			7.0m,
			false,
			false,
			"ash",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Brewing Tools / Mashing Paddle" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_mortar_and_pestle",
			"mortar",
			"a stone mortar and pestle",
			null,
			"This heavy stone mortar has a deep bowl, a matching pestle, and a rim worn by grinding herbs, spices, pigments, and medicines. Dark flecks sit in the pores of the stone. It is a shared apothecary, kitchen, and pigment tool.",
			SizeCategory.Small,
			ItemQuality.Standard,
			3200.0,
			24.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Apothecary Tools / Apothecary Mortar", "Functions / Tools / Apothecary Tools / Apothecary Pestle", "Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_medicine_strainer",
			"strainer",
			"a fine medicine strainer",
			null,
			"This fine linen strainer is stretched over a small wooden hoop for filtering decoctions, syrups, tinctures, and infused oils. The cloth is stained by herbs and honeyed stock. It is finer than an ordinary kitchen sieve.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			5.0m,
			false,
			false,
			"linen",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Medical Tools / Medicine Strainer", "Functions / Tools / Apothecary Tools / Apothecary Strainer" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_ointment_spatula",
			"spatula",
			"a horn ointment spatula",
			null,
			"This small horn spatula has a rounded blade for lifting salves, poultices, creams, and softened wax without cutting the mixture. Its handle is stained by oils and herbs. It is smooth enough for medical and apothecary work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			28.0,
			4.0m,
			false,
			false,
			"horn",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Medical Tools / Ointment Spatula", "Functions / Tools / Apothecary Tools / Apothecary Spatula" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_cupping_vessel",
			"cup",
			"a small glass cupping vessel",
			null,
			"This small thick-walled glass cup has a rounded lip and a heat-smoothed rim for medical cupping. The glass is faintly green and slightly bubbled. Soot marks near the base show where heat has been used to draw air from it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			16.0m,
			false,
			false,
			"glass",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Medical Tools / Cupping Vessel" ],
			[ "Holdable", "Tool_Medical_General", "Destroyable_Glassware" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_cautery_iron",
			"iron",
			"an iron cautery tool",
			null,
			"This iron cautery tool has a long shaft, wrapped grip, and rounded working tip for heated medical, veterinary, or farrier use. The end is dark from repeated heating. It is blunt, controlled, and meant to be used hot.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Surgical Tools / Cautery Iron" ],
			[ "Holdable", "Tool_Medical_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_suture_needle",
			"needle",
			"a curved suture needle",
			null,
			"This small curved iron needle has a fine eye and a controlled point for drawing gut or silk thread through flesh or heavy dressing stock. It sits in a tiny waxed cloth wrap. It is clearly medical rather than ordinary sewing gear.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			6.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Surgical Tools / Surgical Suture Needle" ],
			[ "Holdable", "Tool_Medical_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_surgical_probe",
			"probe",
			"a slender surgical probe",
			null,
			"This slender bronze probe has a rounded end and a flattened handle for checking wound depth, lifting dressing edges, and guiding small medical work. It is polished smooth along the shaft. The blunt end avoids cutting while still finding channels and obstructions.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			35.0,
			14.0m,
			false,
			false,
			"bronze",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Surgical Tools / Surgical Probe" ],
			[ "Holdable", "Tool_Medical_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);

		CreateItem
		(
			"medieval_tool_forceps",
			"forceps",
			"a pair of bronze medical forceps",
			null,
			"These slim bronze forceps have narrow jaws and a springy grip for lifting dressings, removing splinters, handling sutures, or holding small medical stock. The tips are smoothed and rounded. They sit in a simple linen wrap when stored.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			42.0,
			16.0m,
			false,
			false,
			"bronze",
			[ "Market / Professional Tools / High-Quality Tools", "Functions / Tools / Surgical Tools / Forceps" ],
			[ "Holdable", "Tool_Medical_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null
		);
	}
}
