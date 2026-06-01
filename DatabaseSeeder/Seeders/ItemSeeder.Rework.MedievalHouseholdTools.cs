#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalHouseholdCraftTools()
	{
		#region Medieval Household Craft Tools

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Coopering Tools / Croze"
			]
		);

		CreateItem(
			"medieval_coopers_croze",
			"croze",
			"a cooper's iron croze",
			null,
			"This cooper's croze has a timber body and an iron cutter for grooving staves to receive cask heads.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Coopering Tools / Croze"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Planer"
			]
		);

		CreateItem(
			"medieval_iron_wood_plane",
			"plane",
			"a long wooden joiner's plane",
			null,
			"This long joiner's plane carries an iron blade in a wedge throat, suitable for doors, chests, trestles, and boards.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			32.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Planer"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools",
				"Functions / Tools / Bookbinding Tools / Book Press"
			]
		);

		CreateItem(
			"medieval_bookbinder_press",
			"press",
			"a screw bookbinder's press",
			null,
			"This heavy timber press has screw pressure and broad boards for flattening gatherings, boards, and leather-covered books.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			95.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools",
				"Functions / Tools / Bookbinding Tools / Book Press"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools",
				"Functions / Tools / Metalworking Tools / Locksmithing Tools / Locksmith File Set"
			]
		);

		CreateItem(
			"medieval_locksmith_file_set",
			"files",
			"a roll of locksmith's files",
			null,
			"This leather roll holds small iron files, blanks, and rubbing wax for shaping keys, tuning wards, and fitting chest locks.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			34.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Security Tools",
				"Functions / Tools / Metalworking Tools / Locksmithing Tools / Locksmith File Set"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Fulling Stocks",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Fuller's Trough"
			]
		);

		CreateItem(
			"medieval_household_fulling_stocks",
			"stocks",
			"a set of timber fulling stocks",
			null,
			"These fulling stocks have a trough, foot beams, and enough working room to beat, scour, and thicken wool cloth into a medieval broadcloth finish.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			42000.0,
			82.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Fulling Stocks",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Fuller's Trough"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Teasel Frame"
			]
		);

		CreateItem(
			"medieval_household_teasel_frame",
			"frame",
			"a teasel napping frame",
			null,
			"This frame is set with rows of dried teasels for raising nap on fulled cloth before it is shorn smooth.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			28.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Teasel Frame"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Napping Shears",
				"Functions / Separation / Shearing / Shears"
			]
		);

		CreateItem(
			"medieval_household_napping_shears",
			"shears",
			"a pair of long napping shears",
			null,
			"These long-bladed shears are weighted for trimming raised nap and finishing broadcloth with an even surface.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1400.0,
			38.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Napping Shears",
				"Functions / Separation / Shearing / Shears"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Cloth Tenter Frame"
			]
		);

		CreateItem(
			"medieval_household_cloth_tenter_frame",
			"frame",
			"a cloth tenter frame",
			null,
			"This pegged timber frame stretches damp cloth under tension so broadcloth, linens, and dyed lengths dry true.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			48000.0,
			78.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Fulling Tools / Cloth Tenter Frame"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Embroidery Tools / Embroidery Frame"
			]
		);

		CreateItem(
			"medieval_household_embroidery_frame",
			"frame",
			"a small embroidery frame",
			null,
			"This small frame holds cloth under tension for trim, inscriptions, heraldic devices, and devotional stitchwork.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Embroidery Tools / Embroidery Frame"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("bone", MaterialBehaviourType.Bone,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Weaving Tools / Tablet Weaving Cards"
			]
		);

		CreateItem(
			"medieval_household_tablet_weaving_cards",
			"cards",
			"a pack of tablet-weaving cards",
			null,
			"These pierced cards are numbered for weaving patterned bands, girdles, straps, and garment edging.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			12.0m,
			false,
			false,
			"bone",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Textilecraft Tools / Weaving Tools / Tablet Weaving Cards"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Leatherworking Tools / Shoe Last"
			]
		);

		CreateItem(
			"medieval_household_turnshoe_last",
			"last",
			"a wooden turnshoe last",
			null,
			"This shaped wooden last is sized for turning and finishing soft leather shoes, boots, and sandals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			820.0,
			18.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Leatherworking Tools / Shoe Last"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame"
			]
		);

		CreateItem(
			"medieval_household_bookbinder_sewing_frame",
			"frame",
			"a bookbinder's sewing frame",
			null,
			"This compact frame holds support cords taut while quires are sewn into codices, account books, and registers.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			42.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Leather Paring Knife",
				"Functions / Tools / Leatherworking Tools / Leather Paring Knife"
			]
		);

		CreateItem(
			"medieval_household_leather_paring_knife",
			"knife",
			"a leather paring knife",
			null,
			"This short sharp knife has a wide blade for thinning bookbinding leather, shoe uppers, and fine pouch panels.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Leather Paring Knife",
				"Functions / Tools / Leatherworking Tools / Leather Paring Knife"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Drawplate",
				"Functions / Consumables / Drawplate"
			]
		);

		CreateItem(
			"medieval_household_drawplate",
			"drawplate",
			"an iron wire drawplate",
			null,
			"This drawplate has rows of tapered holes for drawing wire used in mail, chains, hinges, pins, and fine fittings.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			72.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Drawplate",
				"Functions / Consumables / Drawplate"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Armouring Tools / Armourer's Anvil"
			]
		);

		CreateItem(
			"medieval_household_armourers_anvil",
			"anvil",
			"a small armourer's anvil",
			null,
			"This armourer's anvil has a polished face, rounded edge, and small horns for raising plates, riveting rings, and shaping helmets.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			95.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Armouring Tools / Armourer's Anvil"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Armouring Tools / Planishing Hammer",
				"Functions / Tools / Striking Tools / Hammer"
			]
		);

		CreateItem(
			"medieval_household_planishing_hammer",
			"hammer",
			"a polished planishing hammer",
			null,
			"This smooth-faced hammer is balanced for finishing helmets, plates, hinges, and other bright metalwork without deep marks.",
			SizeCategory.Small,
			ItemQuality.Good,
			760.0,
			34.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Armouring Tools / Planishing Hammer",
				"Functions / Tools / Striking Tools / Hammer"
			],
			[
				"Holdable",
				"Melee_Improvised Bludgeon",
				"Destroyable_Weapon"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Armouring Tools / Mail Riveting Tongs"
			]
		);

		CreateItem(
			"medieval_household_mail_riveting_tongs",
			"tongs",
			"a pair of mail riveting tongs",
			null,
			"These fine-jawed tongs are made for closing and riveting small mail rings without crushing the weave.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			36.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Armouring Tools / Mail Riveting Tongs"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Bowyer Tools / Bow Press"
			]
		);

		CreateItem(
			"medieval_household_bow_press",
			"press",
			"a bowyer's bow press",
			null,
			"This timber press braces staves, prods, and bindings while bows or crossbow parts are tillered and set.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			60.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Bowyer Tools / Bow Press"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Bowyer Tools / Tillering Stick"
			]
		);

		CreateItem(
			"medieval_household_tillering_stick",
			"stick",
			"a marked tillering stick",
			null,
			"This marked stick is notched for judging draw, bend, and balance while shaping bows and crossbow prods.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			18.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Bowyer Tools / Tillering Stick"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Bowyer Tools / Crossbow Tiller Jig"
			]
		);

		CreateItem(
			"medieval_household_crossbow_tiller_jig",
			"jig",
			"a crossbow tiller jig",
			null,
			"This shaped jig holds a crossbow tiller while its groove, nut seat, stirrup, and lockwork are fitted true.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			46.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodcrafting Tools / Bowyer Tools / Crossbow Tiller Jig"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Mould and Deckle"
			]
		);

		CreateItem(
			"medieval_household_papermakers_mould",
			"mould",
			"a papermaker's mould and deckle",
			null,
			"This framed mould and deckle lifts wet sheets from rag pulp and leaves a neat edge for drying and sizing.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1400.0,
			34.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Mould and Deckle"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Papermaking Vat"
			]
		);

		CreateItem(
			"medieval_household_papermakers_vat",
			"vat",
			"a papermaker's pulp vat",
			null,
			"This broad timber vat is sized for soaking rag pulp and keeping it stirred while sheets are drawn.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			52.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Papermaking Vat"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("bronze", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scribing Tools / Wax Spatula"
			]
		);

		CreateItem(
			"medieval_household_wax_spatula",
			"spatula",
			"a small wax spatula",
			null,
			"This flat spatula smooths melted wax on tablets, seal cakes, charter tags, and tamper marks.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			90.0,
			8.0m,
			false,
			false,
			"bronze",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scribing Tools / Wax Spatula"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Food and Drink / Medieval Food / Dairy",
				"Functions / Tools / Foodmaking Tools / Cheese Press"
			]
		);

		CreateItem(
			"medieval_household_cheese_press",
			"press",
			"a small screw cheese press",
			null,
			"This wooden press has a drip board and screw plate for setting curds into firm wheels for household, market, or monastic dairies.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9200.0,
			38.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Food and Drink / Medieval Food / Dairy",
				"Functions / Tools / Foodmaking Tools / Cheese Press"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Food and Drink / Medieval Food / Brewing",
				"Functions / Tools / Foodmaking Tools / Brewing Tools / Lauter Tun"
			]
		);

		CreateItem(
			"medieval_household_lauter_tun",
			"tun",
			"a small brewing lauter tun",
			null,
			"This open tun has a slotted false bottom and stout staves for draining wort from mash in ale, beer, and monastic brewing.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			58.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Food and Drink / Medieval Food / Brewing",
				"Functions / Tools / Foodmaking Tools / Brewing Tools / Lauter Tun"
			],
			[
				"Holdable",
				"Container_Open_Bin",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("willow", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Milling Tools / Grain Sieve"
			]
		);

		CreateItem(
			"medieval_household_millers_sieve",
			"sieve",
			"a miller's grain sieve",
			null,
			"This framed sieve has a woven mesh for cleaning grain, bolting flour, and separating bran at a mill or bakehouse.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			18.0m,
			false,
			false,
			"willow",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Milling Tools / Grain Sieve"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Glassworking Tools / Grozing Iron"
			]
		);

		CreateItem(
			"medieval_household_glaziers_grozing_iron",
			"iron",
			"a glazier's grozing iron",
			null,
			"This small iron nipper chips and trims coloured glass quarries for windows, lanterns, and reliquary panels.",
			SizeCategory.Small,
			ItemQuality.Standard,
			380.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Glassworking Tools / Grozing Iron"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("wrought iron", MaterialBehaviourType.Metal,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Glassworking Tools / Lead Knife"
			]
		);

		CreateItem(
			"medieval_household_glaziers_lead_knife",
			"knife",
			"a glazier's lead knife",
			null,
			"This broad knife cuts and opens soft lead came for fitting coloured glass into panels.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			20.0m,
			false,
			false,
			"wrought iron",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Glassworking Tools / Lead Knife"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Tile Mould"
			]
		);

		CreateItem(
			"medieval_household_tile_mould",
			"mould",
			"a roof tile mould",
			null,
			"This rectangular wooden mould shapes clay roof tiles and floor tiles before drying, glazing, and firing.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			18.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Tile Mould"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Glazing Basin"
			]
		);

		CreateItem(
			"medieval_household_glazing_basin",
			"basin",
			"a pottery glazing basin",
			null,
			"This shallow basin holds glaze slurry for dipping tiles, jugs, lamps, and market wares before a kiln firing.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			14.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Pottery Tools / Glazing Basin"
			],
			[
				"Holdable",
				"Container_Bowl",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Casting Mould / Lantern Pane Mould"
			]
		);

		CreateItem(
			"medieval_household_lantern_pane_mould",
			"mould",
			"a lantern-pane casting mould",
			null,
			"This small mould shapes thin glass panes for lanterns, reliquaries, and small workshop windows.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			34.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Metalworking Tools / Casting Mould / Lantern Pane Mould"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval workshop and household tool item."
		);

		#endregion
	}
}
