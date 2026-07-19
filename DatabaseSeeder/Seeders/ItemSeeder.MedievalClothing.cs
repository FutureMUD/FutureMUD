#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalClothing()
	{
		CreateItem(
			"medieval_linen_braies",
			"braies",
			"a $colour pair of linen braies",
			null,
			"These $colour linen braies are cut as loose under-breeches with a gathered waist, roomy seat, and short legs. The linen is light enough to sit beneath hose or trousers, with practical seams kept flat against the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shorts",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_undertunic",
			"undertunic",
			"a $colour linen undertunic",
			null,
			"This $colour linen undertunic is made from straight, plain linen panels, with narrow sleeves and small side openings for movement. The neck is simply finished and the hem is light enough to tuck beneath heavier layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_chemise",
			"chemise",
			"a $colour linen chemise",
			null,
			"This $colour linen chemise is made from long, plain linen panels, with narrow sleeves and small side openings for movement. The neck is simply finished and the hem is light enough to tuck beneath heavier layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_breastband",
			"breastband",
			"a $colour linen breastband",
			null,
			"This $colour linen breastband is a long folded strip of cloth meant to wrap firmly around the torso. The ends are narrow enough to tie without bulky knots, while the middle is broad and smooth against the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			80.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Bra",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_plain_leather_belt",
			"belt",
			"a plain leather belt",
			null,
			"This plain leather belt is a long strap of sturdy leather with a simple fastening and burnished edges. The surface is left unornamented, relying on the grain of the hide and the evenness of the cut for its appearance. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			10.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_iron_buckled_leather_belt",
			"belt",
			"an iron-buckled leather belt",
			null,
			"This iron-buckled leather belt is a sturdy strap fitted with a dark iron buckle and a row of cleanly punched holes. The leather is thick enough to hold its shape around the waist, with burnished edges and visible stitching near the buckle fold. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Good,
			240.0,
			30.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_simple_woven_sash",
			"sash",
			"a $colour woven sash",
			null,
			"This $colour woven sash is a long wool band with firm selvages and simple squared ends. It is made to wrap, knot, or girdle clothing without adding a hard buckle or metal fitting. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_woven_girdle",
			"girdle",
			"a fine $colour woven girdle",
			null,
			"This fine $colour woven girdle is a narrow wool band with firm edges and a smooth, even hand. It is long enough to wrap the waist and leave modest hanging ends, while the weave keeps it from twisting. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			100.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_soft_leather_shoes",
			"shoes",
			"a pair of soft leather shoes",
			null,
			"These soft leather shoes are low, flexible footwear with rounded toes and simple stitched uppers. The soles are sturdy enough for regular use without making the shoes stiff or high. Natural variations in the leather show along the seams, toe creases, and cut edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_leather_shoes",
			"shoes",
			"a pair of fine leather shoes",
			null,
			"These fine leather shoes are low, carefully shaped footwear with smooth uppers and neatly turned edges. The soles are firm but not heavy, and the stitching is kept regular around the heel and vamp. Natural variations in the leather show softly across the polished surface and flexed creases.",
			SizeCategory.Small,
			ItemQuality.Good,
			380.0,
			44.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_ankle_leather_boots",
			"boots",
			"a pair of ankle leather boots",
			null,
			"These ankle leather boots are sturdy low boots with firm soles, rounded toes, and shafts that cover the ankle. The uppers are stitched from practical leather panels, with the seams placed where they will not rub too sharply. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			30.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Boots",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_high_leather_boots",
			"boots",
			"a pair of high leather boots",
			null,
			"These high leather boots rise well above the ankle, with firm shafts and soles meant for riding or rough ground. The leather is shaped to the foot and lower leg, with visible stitching around the sole and up the back. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			64.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_High_Boots",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_plain_leather_sandals",
			"sandals",
			"a pair of plain leather sandals",
			null,
			"These plain leather sandals are open footwear with flat soles and simple straps across the foot. The cut is practical and spare, leaving most of the foot exposed while protecting the underside. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			12.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Sandals",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hemp_sandals",
			"sandals",
			"a pair of woven hemp sandals",
			null,
			"These woven hemp sandals are light, open footwear made from braided plant-fibre straps and simple flat soles. The weave is coarse but regular, leaving the toes and much of the foot exposed. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			6.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Sandals",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_hose",
			"hose",
			"a pair of $colour wool hose",
			null,
			"These $colour wool hose are close-fitting wool leg coverings with shaped lower portions and neatly finished upper edges. The cloth is cut to follow the leg without the bulk of full trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			10.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Stockings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_chausses",
			"chausses",
			"a pair of $colour wool chausses",
			null,
			"These $colour wool chausses are close-fitting wool leg coverings with shaped lower portions and neatly finished upper edges. The cloth is cut to follow the leg without the bulk of full trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Chausses",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_narrow_wool_trousers",
			"trousers",
			"a pair of narrow $colour wool trousers",
			null,
			"These narrow $colour wool trousers are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			450.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_tunic",
			"tunic",
			"a $colour wool tunic",
			null,
			"This $colour wool tunic is a straight wool garment made from straight body panels with sleeves set in simply at the shoulder. The neck and cuffs are simply finished, leaving the garment useful under a cloak, mantle, or belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_long_wool_tunic",
			"tunic",
			"a long $colour wool tunic",
			null,
			"This long $colour wool tunic is a long wool garment made from straight body panels with sleeves set in simply at the shoulder. The neck and cuffs are simply finished, leaving the garment useful under a cloak, mantle, or belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			20.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_wool_cloak",
			"cloak",
			"a heavy $colour wool cloak",
			null,
			"This heavy $colour wool cloak is a broad outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			34.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_simple_wool_mantle",
			"mantle",
			"a $colour wool mantle",
			null,
			"This $colour wool mantle is a broad outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			820.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fur_lined_cloak",
			"cloak",
			"a $colour fur-lined cloak",
			null,
			"This $colour fur-lined cloak is a broad outer cloth meant to drape over the shoulders and upper body. Fur shows at the opening and edges, adding thickness where wind would otherwise slip beneath the outer cloth. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1550.0,
			110.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_coif",
			"coif",
			"a $colour linen coif",
			null,
			"This $colour linen coif is a close-fitting head covering with rounded crown panels and ties below the jaw or at the nape. The edges are narrow and smooth, keeping the cloth neat beneath a hood, cap, or veil. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Coif",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_veil",
			"veil",
			"a $colour linen veil",
			null,
			"This $colour linen veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			7.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_wimple",
			"wimple",
			"a $colour linen wimple",
			null,
			"This $colour linen wimple is a linen head and neck covering shaped by smooth folds around the throat and lower face. The fabric is light but close, leaving the outer edge neat enough to sit under a veil or hood. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			9.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_hood",
			"hood",
			"a $colour wool hood",
			null,
			"This $colour wool hood is a wool head covering with a rounded crown and a fall that protects the neck and upper shoulders. The face opening is plainly finished, and the lower edge sits comfortably over other garments. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_felt_cap",
			"cap",
			"a $colour felt cap",
			null,
			"This $colour felt cap is a felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			8.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rectangular_wool_cloak",
			"cloak",
			"a $colour rectangular wool cloak",
			null,
			"This $colour rectangular wool cloak is a rectangular outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_brooched_wool_mantle",
			"mantle",
			"a $colour brooched wool mantle",
			null,
			"This $colour brooched wool mantle is a broad outer cloth meant to drape over the shoulders and upper body. The fastening is plainly visible near the shoulder, gathering the cloth into heavy folds without concealing how it is held. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_tablet_banded_wool_tunic",
			"tunic",
			"a $colour tablet-banded tunic",
			null,
			"This $colour tablet-banded tunic is a tablet-banded wool garment made from straight body panels with sleeves set in simply at the shoulder. Tablet-woven bands frame the neck, cuffs, and lower hem, adding visible structure to the simple body cut. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			560.0,
			44.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_straight_wool_gown",
			"gown",
			"a straight $colour wool gown",
			null,
			"This straight $colour wool gown is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_pinned_wool_gown",
			"gown",
			"a pinned $colour wool gown",
			null,
			"This pinned $colour wool gown is a long wool garment with visible pinning points at the upper body. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_bordered_wool_gown",
			"gown",
			"a fine $colour bordered gown",
			null,
			"This fine $colour bordered gown is a long wool garment with a controlled border at the neck, cuffs, and lower hem. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			70.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_leg_wraps",
			"leg wraps",
			"a pair of $colour wool leg wraps",
			null,
			"These $colour wool leg wraps are long narrow strips of cloth made to wind around the lower legs or feet. The ends are left thin enough to tuck back under the wrapping, while the body of each strip has enough texture to grip itself. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Leggings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_saerk",
			"særk",
			"a $colour linen særk",
			null,
			"This $colour linen særk is a plain linen underdress with long sleeves, straight sides, and a simple rounded neck. It is long enough to show beneath sleeveless outer garments while still sitting close as an underlayer. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			340.0,
			12.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_hangerok",
			"hangerok",
			"a $colour wool hangerok",
			null,
			"This $colour wool hangerok is a sleeveless wool overdress with a straight body and strap-like shoulder hangers. The upper edge and shoulder loops are plainly finished, showing how it hangs over a linen underdress. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_wool_hangerok",
			"hangerok",
			"a fine $colour wool hangerok",
			null,
			"This fine $colour wool hangerok is a sleeveless wool overdress with a straight body and strap-like shoulder hangers. The upper edge and hanging loops are more carefully finished, giving it a richer look over a linen underdress. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			620.0,
			64.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_checked_wool_mantle",
			"mantle",
			"a $colour1 and $colour2 checked mantle",
			null,
			"This $colour1 and $colour2 checked mantle is a broad outer cloth meant to drape over the shoulders and upper body. The checked weave remains visible across the folds, breaking the broad surface into regular blocks of colour. The $colour1 and $colour2 threads are woven into an even checked pattern, giving the mantle visible contrast without relying on applied ornament.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_2BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_checked_wool_mantle",
			"mantle",
			"a fine $colour1 and $colour2 checked mantle",
			null,
			"This fine $colour1 and $colour2 checked mantle is a broad outer cloth meant to drape over the shoulders and upper body. The checked weave remains visible across the folds, breaking the broad surface into regular blocks of colour. The $colour1 and $colour2 threads are woven into an even checked pattern, giving the mantle visible contrast without relying on applied ornament.",
			SizeCategory.Normal,
			ItemQuality.Good,
			920.0,
			86.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_2FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_gaelic_leine",
			"léine",
			"a long $colour linen léine",
			null,
			"This long $colour linen léine is a long linen tunic with full sleeves and a generous body cut from straight panels. The length and loose fall make it useful as both underlayer and visible garment. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_bordered_leine",
			"léine",
			"a fine $colour bordered léine",
			null,
			"This fine $colour bordered léine is a long linen tunic with full sleeves and a generous body cut from straight panels. The length and loose fall make it useful as both underlayer and visible garment. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			560.0,
			58.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_brat",
			"brat",
			"a $colour wool brat",
			null,
			"This $colour wool brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			980.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_bordered_brat",
			"brat",
			"a fine $colour bordered brat",
			null,
			"This fine $colour bordered brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1000.0,
			82.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_trews",
			"trews",
			"a pair of $colour wool trews",
			null,
			"These $colour wool trews are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			480.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_frankish_knee_tunic",
			"tunic",
			"a knee-length $colour wool tunic",
			null,
			"This knee-length $colour wool tunic is a knee-length wool garment made from straight body panels with sleeves set in simply at the shoulder. The neck and cuffs are simply finished, leaving the garment useful under a cloak, mantle, or belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			500.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_frankish_tunic",
			"tunic",
			"a fine $colour bordered tunic",
			null,
			"This fine $colour bordered tunic is a bordered wool garment made from straight body panels with sleeves set in simply at the shoulder. A neat border marks the neck, cuffs, and hem, concentrating decoration where it will not be hidden by a belt. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			540.0,
			54.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_carolingian_cloak",
			"cloak",
			"a $colour shoulder-fastened cloak",
			null,
			"This $colour shoulder-fastened cloak is a broad outer cloth meant to drape over the shoulders and upper body. The fastening is plainly visible near the shoulder, gathering the cloth into heavy folds without concealing how it is held. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			880.0,
			26.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_norman_split_tunic",
			"tunic",
			"a split-skirt $colour wool tunic",
			null,
			"This split-skirt $colour wool tunic is a split-skirt wool garment made from straight body panels with sleeves set in simply at the shoulder. The skirt is split for easier riding and striding, with the openings finished so they do not tear upward. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_split_riding_tunic",
			"tunic",
			"a fine split-skirt $colour tunic",
			null,
			"This fine split-skirt $colour tunic is a split-skirt wool garment made from straight body panels with sleeves set in simply at the shoulder. The skirt is split for easier riding and striding, with the openings finished so they do not tear upward. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			640.0,
			66.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_capetian_cotte",
			"cotte",
			"a $colour wool cotte",
			null,
			"This $colour wool cotte is a long wool body garment cut close enough to layer under a surcoat or mantle. The sleeves are narrow through the forearm, and the skirt has enough fullness for walking. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges. With plain seams and a serviceable fall at the cuffs, neck, and hem.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_wool_cotte",
			"cotte",
			"a fine $colour wool cotte",
			null,
			"This fine $colour wool cotte is a long wool body garment cut close enough to layer under a surcoat or mantle. The sleeves are narrow through the forearm, and the skirt has enough fullness for walking. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow. With a smooth, more carefully finished fall at the cuffs, neck, and hem.",
			SizeCategory.Normal,
			ItemQuality.Good,
			600.0,
			58.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_chainse",
			"chainse",
			"a $colour linen chainse",
			null,
			"This $colour linen chainse is made from straight, plain linen panels, with narrow sleeves and small side openings for movement. The neck is simply finished and the hem is light enough to tuck beneath heavier layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			12.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_kirtle",
			"kirtle",
			"a $colour wool kirtle",
			null,
			"This $colour wool kirtle is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			700.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_wool_kirtle",
			"kirtle",
			"a fine $colour wool kirtle",
			null,
			"This fine $colour wool kirtle is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			720.0,
			72.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_bliaut",
			"bliaut",
			"a fine $colour bliaut",
			null,
			"This fine $colour bliaut is a long formal gown with close upper sleeves, a fitted body, and a fuller fall below the waist. The skirt hangs in deep, ordered folds and the sleeve openings are finished with care. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			880.0,
			96.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_simple_surcoat",
			"surcoat",
			"a sleeveless $colour surcoat",
			null,
			"This sleeveless $colour surcoat is a sleeveless wool overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			560.0,
			20.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_surcoat",
			"surcoat",
			"a fine $colour silk surcoat",
			null,
			"This fine $colour silk surcoat is a sleeveless silk overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			420.0,
			150.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_half_circle_mantle",
			"mantle",
			"a $colour half-circle mantle",
			null,
			"This $colour half-circle mantle is a half-circle outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fur_trimmed_mantle",
			"mantle",
			"a $colour fur-trimmed mantle",
			null,
			"This $colour fur-trimmed mantle is a broad outer cloth meant to drape over the shoulders and upper body. Fur shows at the opening and edges, adding thickness where wind would otherwise slip beneath the outer cloth. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1200.0,
			120.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_gugel_hood",
			"hood",
			"a $colour wool gugel hood",
			null,
			"This $colour wool gugel hood is a fitted hood with a short shoulder cape and a close face opening. The cape spreads over the upper shoulders, keeping the neck covered beneath cloaks or mantles. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_barbette_veil",
			"veil",
			"a $colour linen barbette veil",
			null,
			"This $colour linen barbette veil combines a head veil with a band that frames the chin and temples. The cloth is light, even, and arranged to make a composed outline around the face. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			150.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_linen_veil",
			"veil",
			"a fine $colour linen veil",
			null,
			"This fine $colour linen veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			110.0,
			24.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_christian_iberian_camisa",
			"camisa",
			"a $colour linen camisa",
			null,
			"This $colour linen camisa is a light linen shirt with straight-cut sleeves, a simple neck slit, and a loose body. The cloth is fine enough for warm weather or under-layering beneath woollen garments. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			11.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_saya",
			"saya",
			"a $colour wool saya",
			null,
			"This $colour wool saya is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			720.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_wool_saya",
			"saya",
			"a fine $colour wool saya",
			null,
			"This fine $colour wool saya is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			730.0,
			76.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_iberian_pellote",
			"pellote",
			"a sleeveless $colour pellote",
			null,
			"This sleeveless $colour pellote is a sleeveless wool overgarment with deep side openings and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			26.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_pellote",
			"pellote",
			"a fine $colour silk pellote",
			null,
			"This fine $colour silk pellote is a sleeveless silk overgarment with deep side openings and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			460.0,
			150.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_iberian_manto",
			"manto",
			"a $colour wool manto",
			null,
			"This $colour wool manto is a broad mantle with a generous curved or rectangular fall over the shoulders. It is made to frame the upper body and cover the outer garment beneath without close tailoring. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_iberian_manto",
			"manto",
			"a fine $colour silk-lined manto",
			null,
			"This fine $colour silk-lined manto is a broad mantle with a generous curved or rectangular fall over the shoulders. It is made to frame the upper body and cover the outer garment beneath without close tailoring. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1050.0,
			128.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_toca",
			"toca",
			"a $colour linen toca",
			null,
			"This $colour linen toca is a wrapped headcloth with enough length to cover the hair and frame the sides of the face. The fabric is light and foldable, with the edge kept plain for repeated tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_kamision",
			"kamision",
			"a $colour linen kamision",
			null,
			"This $colour linen kamision is a linen body garment cut in straight panels with long sleeves and a plain neck opening. It is made to sit beneath heavier robes, leaving only the clean edges likely to show. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			14.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_dalmatic",
			"dalmatic",
			"a $colour wool dalmatic",
			null,
			"This $colour wool dalmatic is a loose robe with broad sleeves, a straight body, and a dignified fall from shoulder to hem. The cut leaves room for an underlayer, while the neck and cuffs are the clearest places for visible finishing. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_silk_dalmatic",
			"dalmatic",
			"a fine $colour silk dalmatic",
			null,
			"This fine $colour silk dalmatic is a loose robe with broad sleeves, a straight body, and a dignified fall from shoulder to hem. The cut leaves room for an underlayer, while the neck and cuffs are the clearest places for visible finishing. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			520.0,
			180.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_skaramangion",
			"skaramangion",
			"a $colour skaramangion",
			null,
			"This $colour skaramangion is a long, front-closing silk coat with a controlled skirt and sleeves suited to formal mounted dress. The panels are cut to hang close through the body before loosening below the waist. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			640.0,
			170.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sagion",
			"sagion",
			"a $colour sagion",
			null,
			"This $colour sagion is a wool shoulder-cloak with a short, controlled fall and a visible fastening point near the upper edge. The cloth crosses the upper body cleanly without the volume of a full mantle. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			32.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_tablion_cloak",
			"cloak",
			"a $colour tablion cloak",
			null,
			"This $colour tablion cloak is a broad outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			210.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_loros_sash",
			"sash",
			"a fine $colour loros sash",
			null,
			"This fine $colour loros sash is a long, weighty strip of silk meant to cross and loop over formal robes. Its surface is closely woven, with the edges kept straight so the band lies in controlled folds. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			240.0,
			160.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_maforion",
			"maforion",
			"a $colour maforion veil",
			null,
			"This $colour maforion veil is a broad head-and-shoulder veil with a long fall around the face and upper body. The cloth is cut generously enough to layer over a robe without losing its clean outline. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			14.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_maforion",
			"maforion",
			"a fine $colour maforion veil",
			null,
			"This fine $colour maforion veil is a broad head-and-shoulder veil with a long fall around the face and upper body. The cloth is cut generously enough to layer over a robe without losing its clean outline. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			160.0,
			70.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_soft_campagi_shoes",
			"campagi",
			"a pair of soft leather campagi",
			null,
			"These soft leather campagi are low shoes with rounded toes, open shaping at the upper, and neat straps across the foot. The leather is smoothed and carefully finished, giving the pair a cleaner appearance than ordinary work shoes. Natural variations in the leather show along the seams, toe creases, and cut edges.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			54.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_qamis",
			"qamis",
			"a $colour linen qamis",
			null,
			"This $colour linen qamis is a long, loose shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_qamis",
			"qamis",
			"a $colour cotton qamis",
			null,
			"This $colour cotton qamis is a long, loose shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			16.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_qamis",
			"qamis",
			"a fine $colour silk qamis",
			null,
			"This fine $colour silk qamis is a long, loose shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			340.0,
			120.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_sirwal",
			"sirwal",
			"a pair of $colour cotton sirwal",
			null,
			"These $colour cotton sirwal are loose drawstring trousers with a roomy seat and tapered lower legs. The cloth gathers softly at the waist and falls in practical folds around the thighs. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_sirwal",
			"sirwal",
			"a pair of fine $colour silk sirwal",
			null,
			"These fine $colour silk sirwal are loose drawstring trousers with a roomy seat and tapered lower legs. The cloth gathers softly at the waist and falls in practical folds around the thighs. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			300.0,
			80.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_jubba",
			"jubba",
			"a $colour wool jubba",
			null,
			"This $colour wool jubba is a loose wool robe with long sleeves, a straight body, and a generous front opening. It is cut to layer over lighter garments, with the edges and cuffs given the most visible finish. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			34.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_tiraz_robe",
			"robe",
			"a fine $colour tiraz robe",
			null,
			"This fine $colour tiraz robe is a long silk robe with straight panels, full sleeves, and decorative bands set where the upper body is most visible. The bands read as formal textile ornament, worked into the surface rather than attached separately. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			620.0,
			220.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_izar",
			"izar",
			"a $colour linen izar",
			null,
			"This $colour linen izar is a broad linen wrap with enough length to pass around the body and fall in layered folds. The edges are plain and straight, letting the drape and overlap form most of the garment's shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_burnous",
			"burnous",
			"a $colour wool burnous",
			null,
			"This $colour wool burnous is a wool cloak with a hooded upper edge and a loose body meant to wrap around the shoulders. The cloth falls in deep folds and leaves the front easy to draw close against wind or dust. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			980.0,
			34.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_burnous",
			"burnous",
			"a fine $colour wool burnous",
			null,
			"This fine $colour wool burnous is a fine wool cloak with a hooded upper edge and a loose body meant to wrap around the shoulders. The cloth falls in deep folds and leaves the front easy to draw close against wind or dust. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1000.0,
			96.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wrapped_cotton_turban",
			"turban",
			"a $colour cotton turban",
			null,
			"This $colour cotton turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			12.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_turban",
			"turban",
			"a fine $colour silk turban",
			null,
			"This fine $colour silk turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			180.0,
			70.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_head_veil",
			"veil",
			"a $colour cotton head veil",
			null,
			"This $colour cotton head veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			9.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_veil",
			"veil",
			"a fine $colour silk veil",
			null,
			"This fine $colour silk veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			100.0,
			56.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_soft_leather_slippers",
			"slippers",
			"a pair of soft leather slippers",
			null,
			"These soft leather slippers are low, supple shoes with flexible soles and shallow uppers. The heels are lightly shaped and the opening is broad enough to slip on without heavy fastenings. Natural variations in the leather show along the seams, toe creases, and cut edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_seljuk_caftan",
			"caftan",
			"a $colour front-opening caftan",
			null,
			"This $colour front-opening caftan is a front-opening coat with long sleeves, a beltable waist, and a skirt shaped for movement. The front edges and cuffs are the most carefully finished parts of the garment. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			42.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_seljuk_caftan",
			"caftan",
			"a fine $colour silk caftan",
			null,
			"This fine $colour silk caftan is a front-opening coat with long sleeves, a beltable waist, and a skirt shaped for movement. The front edges and cuffs are the most carefully finished parts of the garment. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			680.0,
			190.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_felt_turkic_cap",
			"cap",
			"a $colour pointed felt cap",
			null,
			"This $colour pointed felt cap is a pointed felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			150.0,
			10.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_magyar_riding_coat",
			"coat",
			"a $colour belted riding coat",
			null,
			"This $colour belted riding coat is a long belted wool coat with a front opening and a skirt shaped for movement. The sleeves are practical and the waist is intended to be gathered by a belt rather than close tailoring. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			38.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_magyar_coat",
			"coat",
			"a fine $colour fur-trimmed coat",
			null,
			"This fine $colour fur-trimmed coat is a long belted outer garment with fur marking the collar, cuffs, and front edge. The skirt is cut with enough width for riding or striding, while the upper body keeps a neat formal line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1050.0,
			120.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_steppe_riding_caftan",
			"caftan",
			"a $colour riding caftan",
			null,
			"This $colour riding caftan is a front-opening coat with long sleeves, a beltable waist, and a skirt shaped for movement. The front edges and cuffs are the most carefully finished parts of the garment. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			880.0,
			40.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_steppe_caftan",
			"caftan",
			"a fine $colour silk-banded caftan",
			null,
			"This fine $colour silk-banded caftan is a front-opening coat with decorative bands at the neck, front edges, and cuffs. The skirt has enough flare for riding, and the sleeves are cut to fall cleanly from the shoulder. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			920.0,
			125.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_steppe_felt_boots",
			"boots",
			"a pair of felt riding boots",
			null,
			"These felt riding boots are tall, warm boots shaped from dense felt with reinforced soles and a firm lower edge. The shafts are broad enough to fit over trousers while still narrowing toward the ankle. The natural felt surface is plainly visible, with a compacted texture suited to cold, dry travel.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			24.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Boots",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_steppe_felt_cap",
			"cap",
			"a $colour steppe felt cap",
			null,
			"This $colour steppe felt cap is a steppe felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			9.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_steppe_silk_belt",
			"belt",
			"a fine $colour silk belt",
			null,
			"This fine $colour silk belt is a long, narrow band of silk woven to sit flat around the waist. The ends are finished cleanly so they can hang in ordered folds after being wrapped. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			120.0,
			64.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_deel",
			"robe",
			"a $colour belted steppe robe",
			null,
			"This $colour belted steppe robe is a belted robe with a wrapping front, long sleeves, and a skirt shaped for riding or walking. The cloth crosses the chest in firm folds and is meant to be controlled by a waist belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			980.0,
			46.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_deel",
			"robe",
			"a fine $colour silk steppe robe",
			null,
			"This fine $colour silk steppe robe is a belted robe with a wrapping front, long sleeves, and a skirt shaped for riding or walking. The cloth crosses the chest in firm folds and is meant to be controlled by a waist belt. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			700.0,
			210.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_linen_rubakha",
			"rubakha",
			"a $colour linen rubakha",
			null,
			"This $colour linen rubakha is a long shirt-like garment with a slit neck, narrow cuffs, and straight body panels. The hem falls low enough to blouse over a belt, and the sleeves are cut for work beneath a heavier coat. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			430.0,
			14.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_rus_rubakha",
			"rubakha",
			"a fine $colour embroidered rubakha",
			null,
			"This fine $colour embroidered rubakha is a long shirt-like garment with a small embroidered-looking facing at the neck and cuffs. The hem falls low enough to blouse over a belt, and the sleeves are cut for work beneath a heavier coat. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			450.0,
			54.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_wool_porty",
			"porty",
			"a pair of $colour wool porty",
			null,
			"These $colour wool porty are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_onuchi",
			"onuchi",
			"a pair of $colour onuchi leg wraps",
			null,
			"These $colour onuchi leg wraps are long narrow strips of cloth made to wind around the lower legs or feet. The ends are left thin enough to tuck back under the wrapping, while the body of each strip has enough texture to grip itself. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Leggings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_poneva",
			"poneva",
			"a $colour1 and $colour2 woven poneva",
			null,
			"This $colour1 and $colour2 woven poneva is a wrapped wool skirt made from patterned panels with a visible overlap. The waist edge is firm enough to hold a girdle, while the lower hem hangs in straight folds. The $colour1 and $colour2 yarns show in alternating bands across the weave, making the poneva patterned in the cloth rather than painted on top.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_2BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_sleeveless_overgown",
			"overgown",
			"a $colour sleeveless overgown",
			null,
			"This $colour sleeveless overgown is a long wool garment with open armholes and a sleeveless upper body. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			26.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_shuba",
			"shuba",
			"a $colour fur-lined shuba",
			null,
			"This $colour fur-lined shuba is a warm overcoat with wool outside and fur visible along the collar, cuffs, and opening. The body is cut generously so it can close over a rubakha, trousers, and winter layers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			120.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_fur_hat",
			"hat",
			"a fur-trimmed wool hat",
			null,
			"This fur-trimmed wool hat is a rounded cap with a wool crown and fur marking the lower edge. The trim adds warmth around the brow and ears without covering the whole hat in pelt. Natural variations in the fur and wool show where the materials meet at the brim.",
			SizeCategory.Small,
			ItemQuality.Good,
			220.0,
			36.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_dhoti",
			"dhoti",
			"a $colour cotton dhoti",
			null,
			"This $colour cotton dhoti is a long wrapped lower garment made from a rectangular length of cotton. The fabric is folded and passed around the waist and legs, making its shape depend on the wrapping rather than stitching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			10.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_cotton_dhoti",
			"dhoti",
			"a fine $colour cotton dhoti",
			null,
			"This fine $colour cotton dhoti is a long wrapped lower garment made from a rectangular length of cotton. The fabric is folded and passed around the waist and legs, making its shape depend on the wrapping rather than stitching. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			36.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_uttariya",
			"uttariya",
			"a $colour cotton uttariya",
			null,
			"This $colour cotton uttariya is a long draped shoulder cloth meant to pass over one or both shoulders. The edges are kept straight, and the fabric is light enough to fold without making a bulky knot. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			10.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_uttariya",
			"uttariya",
			"a fine $colour silk uttariya",
			null,
			"This fine $colour silk uttariya is a long draped shoulder cloth meant to pass over one or both shoulders. The edges are kept straight, and the fabric is light enough to fold without making a bulky knot. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			180.0,
			72.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_turban",
			"turban",
			"a $colour cotton turban",
			null,
			"This $colour cotton turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			200.0,
			11.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_turban_indian",
			"turban",
			"a fine $colour silk turban",
			null,
			"This fine $colour silk turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			170.0,
			70.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_sari",
			"sari",
			"a $colour cotton sari",
			null,
			"This $colour cotton sari is a long wrapped garment with enough length to form both lower drape and upper fold. The cloth relies on pleating, tucking, and wrapping rather than seams, leaving the edge visible along the fall. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			18.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_sari",
			"sari",
			"a fine $colour silk sari",
			null,
			"This fine $colour silk sari is a long wrapped garment with enough length to form both lower drape and upper fold. The cloth relies on pleating, tucking, and wrapping rather than seams, leaving the edge visible along the fall. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			420.0,
			140.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_cotton_bodice",
			"bodice",
			"a short $colour cotton bodice",
			null,
			"This short $colour cotton bodice is a short fitted upper-body garment with a close waist, shaped sides, and narrow shoulder coverage. It is meant to sit with a wrapped lower cloth or skirt, keeping the torso neat without heavy layering. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			10.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_bodice",
			"bodice",
			"a fine $colour silk bodice",
			null,
			"This fine $colour silk bodice is a short fitted upper-body garment with a close waist, shaped sides, and narrow shoulder coverage. It is meant to sit with a wrapped lower cloth or skirt, keeping the torso neat without heavy layering. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			140.0,
			54.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_lower_cloth",
			"cloth",
			"a $colour cotton lower cloth",
			null,
			"This $colour cotton lower cloth is a rectangular lower cloth meant to wrap at the waist and fall in plain vertical folds. The fabric is simple, flexible, and edged for repeated tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			9.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_south_indian_veshti",
			"veshti",
			"a $colour cotton veshti",
			null,
			"This $colour cotton veshti is a rectangular cotton lower cloth wrapped around the waist and falling in straight folds. The edges are clean and visible, and the front can be tucked to keep the hem clear for movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			10.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_silk_bordered_veshti",
			"veshti",
			"a fine $colour silk-bordered veshti",
			null,
			"This fine $colour silk-bordered veshti is a rectangular cotton lower cloth wrapped around the waist and falling in straight folds. The edges are clean and visible, and the front can be tucked to keep the hem clear for movement. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			320.0,
			44.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_south_indian_angavastram",
			"angavastram",
			"a $colour cotton angavastram",
			null,
			"This $colour cotton angavastram is a broad shoulder cloth that can be folded, draped, or drawn around the upper body. The cloth is rectangular and plainly edged, relying on the wearer's wrapping for its final shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			230.0,
			10.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_angavastram",
			"angavastram",
			"a fine $colour silk angavastram",
			null,
			"This fine $colour silk angavastram is a broad shoulder cloth that can be folded, draped, or drawn around the upper body. The cloth is rectangular and plainly edged, relying on the wearer's wrapping for its final shape. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			170.0,
			70.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_wrap_skirt",
			"skirt",
			"a $colour cotton wrap skirt",
			null,
			"This $colour cotton wrap skirt is a wrapped skirt made from a broad cloth panel with a visible overlap and plain hem. The fabric falls in practical folds and can be gathered by a sash or belt at the waist. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silk_wrap_skirt",
			"skirt",
			"a fine $colour silk wrap skirt",
			null,
			"This fine $colour silk wrap skirt is a wrapped skirt made from a broad cloth panel with a visible overlap and plain hem. The fabric falls in practical folds and can be gathered by a sash or belt at the waist. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			300.0,
			80.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_cross_collar_robe",
			"robe",
			"a $colour cross-collar robe",
			null,
			"This $colour cross-collar robe is a long cloth robe with a diagonal front overlap and straight sleeves. The collar line crosses the chest cleanly, and the skirt falls in narrow vertical folds. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			48.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_round_collar_robe",
			"robe",
			"a $colour round-collar robe",
			null,
			"This $colour round-collar robe is a long silk robe with a rounded neck opening, long sleeves, and a controlled straight fall. The front and cuffs are neatly finished, giving the garment a more formal silhouette than a work jacket. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			650.0,
			96.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_beizi",
			"beizi",
			"a long $colour beizi over-robe",
			null,
			"This long $colour beizi over-robe is a long over-robe with a straight open front, narrow sleeves, and side slits that let the underlayers move. The cloth falls in vertical lines from shoulder to lower hem, giving the garment a composed outer silhouette. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			520.0,
			92.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_ru_jacket",
			"jacket",
			"a $colour cross-collar jacket",
			null,
			"This $colour cross-collar jacket is a short cross-collar jacket with straight sleeves, a wrapped front, and a tidy waist-length body. The garment is cut for movement, with the front overlap and cuffs giving it most of its visible shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			34.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Jacket",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_changqun_skirt",
			"skirt",
			"a $colour long pleated skirt",
			null,
			"This $colour long pleated skirt is a long skirt with narrow pleats falling from a firm waist edge. The pleats give the cloth movement while keeping the upper line orderly. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			480.0,
			36.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_trousers",
			"trousers",
			"a pair of $colour cloth trousers",
			null,
			"These $colour cloth trousers are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_cloth_shoes",
			"shoes",
			"a pair of cloth shoes",
			null,
			"These cloth shoes are light, low footwear with soft uppers and flat soles. The seams are kept close to the edges, and the shape is simple enough to disappear beneath long robes or trousers. The fabric surface is plain and practical, with wear most visible around the toe and heel.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			12.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_official_cap",
			"cap",
			"a black official cap",
			null,
			"This black official cap is a structured headpiece with a dark cloth body and crisp, formal lines. Its shape is controlled and symmetrical, with the edges finished cleanly to hold the cap's profile. The black surface is plain and deliberate, relying on silhouette rather than bright ornament.",
			SizeCategory.Small,
			ItemQuality.Good,
			150.0,
			70.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_headscarf",
			"headscarf",
			"a $colour cloth headscarf",
			null,
			"This $colour cloth headscarf is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			6.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_cross_collar_jacket",
			"jacket",
			"a $colour cross-collar jacket",
			null,
			"This $colour cross-collar jacket is a short cross-collar jacket with straight sleeves, a wrapped front, and a tidy waist-length body. The garment is cut for movement, with the front overlap and cuffs giving it most of its visible shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			380.0,
			34.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Jacket",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_baji",
			"baji",
			"a pair of $colour baji trousers",
			null,
			"These $colour baji trousers are loose trousers with a gathered waist, full seat, and narrowing lower legs. The cloth is cut for easy movement beneath a jacket or robe, with plain seams and practical hems. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			380.0,
			16.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_chima",
			"chima",
			"a $colour chima skirt",
			null,
			"This $colour chima skirt is a full skirt with a gathered upper edge and a broad fall from waist to lower hem. The fabric spreads in soft folds and is meant to sit below a short jacket or over-robe. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			480.0,
			40.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_po_robe",
			"po",
			"a $colour po over-robe",
			null,
			"This $colour po over-robe is a long over-robe with broad sleeves, a wrapped front, and a steady fall below the knee. It is cut to sit over a jacket and trousers or skirt, adding a cleaner outer line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			620.0,
			90.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_silk_overrobe",
			"over-robe",
			"a fine $colour silk over-robe",
			null,
			"This fine $colour silk over-robe is a substantial outer robe with padded body panels and long sleeves. The added thickness gives the robe a rounded fall and makes the edges sit away from the underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			650.0,
			140.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_headcloth",
			"headcloth",
			"a $colour cloth headcloth",
			null,
			"This $colour cloth headcloth is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			100.0,
			6.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_felt_hat",
			"hat",
			"a $colour felt hat",
			null,
			"This $colour felt hat is a shaped felt hat with a rounded crown and a firm lower edge. The felt holds a simple profile, with little ornament beyond the shape and colour of the material. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			150.0,
			10.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_kosode",
			"kosode",
			"a $colour kosode robe",
			null,
			"This $colour kosode robe is a straight-seamed robe with small sleeve openings, a wrapped front, and a narrow collar. The body is made from rectangular panels, giving it a clean fall and visible folded overlap at the chest. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			560.0,
			48.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_hakama",
			"hakama",
			"a pair of $colour hakama",
			null,
			"These $colour hakama are divided skirt-like trousers with deep folds falling from the waist. The legs are broad and straight, giving the garment a formal drape while still allowing movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			38.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_suikan",
			"suikan",
			"a $colour suikan robe",
			null,
			"This $colour suikan robe is a robe with a straight body, open sleeves, and ties that control the loose outer fall. The panels are arranged to show broad, flat surfaces and a clean drape over underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			600.0,
			100.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_hitatare",
			"hitatare",
			"a $colour hitatare robe",
			null,
			"This $colour hitatare robe is a robe with wide sleeves, a squared body, and a practical tied front. The panels are arranged to show broad, flat surfaces and a clean drape over underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			620.0,
			110.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_uchigi",
			"uchigi",
			"a fine $colour uchigi robe",
			null,
			"This fine $colour uchigi robe is a formal silk robe with broad sleeves, a straight body, and a soft layered fall. It is meant to sit among other robes, showing most clearly at the collar, sleeve edges, and hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			640.0,
			150.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_kariginu",
			"kariginu",
			"a fine $colour kariginu robe",
			null,
			"This fine $colour kariginu robe is a robe with wide sleeves, a loose body, and a formal hunting-robe silhouette. The panels are arranged to show broad, flat surfaces and a clean drape over underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			620.0,
			150.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_eboshi",
			"eboshi",
			"a black eboshi cap",
			null,
			"This black eboshi cap is a tall, folded headpiece with a crisp dark surface and a narrow base. The shape rises cleanly from the head, giving it a distinctive formal silhouette. The black cloth is plain and smooth, with the emphasis on line rather than ornament.",
			SizeCategory.Small,
			ItemQuality.Good,
			80.0,
			44.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_hemp_sandals",
			"sandals",
			"a pair of hemp sandals",
			null,
			"These woven hemp sandals are light, open footwear made from braided plant-fibre straps and simple flat soles. The weave is coarse but regular, leaving the toes and much of the foot exposed. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			6.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Sandals",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_headcloth",
			"headcloth",
			"a $colour linen headcloth",
			null,
			"This $colour linen headcloth is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_over_tunic",
			"overtunic",
			"a heavy $colour wool overtunic",
			null,
			"This heavy $colour wool overtunic is cut larger than an under-tunic, with broad sleeves and a weighty skirt for layering. The wool is dense and slightly bulky, making the garment useful over lighter clothing in cold air. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_lined_wool_hose",
			"hose",
			"a pair of thick $colour wool hose",
			null,
			"These thick $colour wool hose are close-fitting leg coverings made from dense wool, shaped to add warmth beneath outer clothing. The feet and ankles are cut with practical seams, while the upper edges are finished to sit under a tunic or breeches. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Stockings",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_footwraps",
			"footwraps",
			"a pair of thick $colour wool footwraps",
			null,
			"These thick $colour wool footwraps are long narrow strips of cloth made to wind around the lower legs or feet. The ends are left thin enough to tuck back under the wrapping, while the body of each strip has enough texture to grip itself. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Socks",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_mittens",
			"mittens",
			"a pair of $colour wool mittens",
			null,
			"These $colour wool mittens are simple wool hand coverings with a separate thumb and a rounded body for the fingers. The cuffs are long enough to tuck beneath sleeves or cloak folds. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Mittens",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_leather_winter_mittens",
			"mittens",
			"a pair of fur-lined leather mittens",
			null,
			"These fur-lined leather mittens are thick hand coverings with leather outside and warm fur visible at the cuff. The thumb is separately shaped, while the main body keeps the fingers together for warmth. Natural variations in the leather and fur show at the seams, cuff, and flexed places.",
			SizeCategory.Small,
			ItemQuality.Good,
			180.0,
			34.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Mittens",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_lined_leather_boots",
			"boots",
			"a pair of lined leather boots",
			null,
			"These lined leather boots are sturdy outdoor footwear with leather uppers, firm soles, and a warm inner lining. The shafts rise high enough to protect the ankle and lower leg, while the seams are placed away from the worst rubbing points. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Good,
			980.0,
			56.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Boots",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_hooded_wool_cloak",
			"cloak",
			"a heavy $colour hooded wool cloak",
			null,
			"This heavy $colour hooded wool cloak is a hooded outer cloth meant to drape over the shoulders and upper body. The hood is cut as part of the weather-facing shape, falling forward enough to shadow the face without becoming a mask. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1650.0,
			46.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fur_faced_winter_cloak",
			"cloak",
			"a fine $colour fur-faced cloak",
			null,
			"This fine $colour fur-faced cloak is a broad outer cloth meant to drape over the shoulders and upper body. Fur shows at the opening and edges, adding thickness where wind would otherwise slip beneath the outer cloth. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1950.0,
			170.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_waxed_skin_cape",
			"cape",
			"a waxed skin cape",
			null,
			"This waxed skin cape is a weather-facing outer layer made from treated animal skin with a broad shoulder fall. The surface has a dull waxy sheen, and the edges are left sturdy rather than decorative. Natural variations in the skin show at the edges, seams, and flexed places.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			32.0m,
			true,
			false,
			"animal skin",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cape",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_checked_wool_mantle",
			"mantle",
			"a heavy $colour1 and $colour2 checked mantle",
			null,
			"This heavy $colour1 and $colour2 checked mantle is a broad outer cloth meant to drape over the shoulders and upper body. The checked weave remains visible across the folds, breaking the broad surface into regular blocks of colour. The $colour1 and $colour2 threads are woven into an even checked pattern, giving the mantle visible contrast without relying on applied ornament.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			42.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_2BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_wool_brat",
			"brat",
			"a heavy $colour wool brat",
			null,
			"This heavy $colour wool brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1400.0,
			42.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fur_trimmed_brat",
			"brat",
			"a fine $colour fur-trimmed brat",
			null,
			"This fine $colour fur-trimmed brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			120.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_cowl_hood",
			"hood",
			"a $colour wool cowl hood",
			null,
			"This $colour wool cowl hood is a roomy hood with a deep face opening and a short drape around the shoulders. The wool gathers in heavy folds at the neck, adding warmth where cloaks often leave gaps. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			20.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_wool_surcoat",
			"surcoat",
			"a heavy $colour wool surcoat",
			null,
			"This heavy $colour wool surcoat is a sleeveless wool overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			32.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_lined_wool_manto",
			"manto",
			"a lined $colour wool manto",
			null,
			"This lined $colour wool manto is a broad mantle with a generous curved or rectangular fall over the shoulders. It is made to frame the upper body and cover the outer garment beneath without close tailoring. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			42.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_paenula",
			"paenula",
			"a hooded $colour wool paenula",
			null,
			"This hooded $colour wool paenula is a closed, weather-facing cloak with a rounded body and an attached hood. The cloth hangs as a broad shell around the shoulders, with the front falling in heavy protective folds. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			38.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_sagion",
			"sagion",
			"a heavy $colour sagion cloak",
			null,
			"This heavy $colour sagion cloak is a substantial wool shoulder-cloak with a dense fall and weather-ready weight. It gathers near one shoulder and hangs in broad folds across the body. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1150.0,
			72.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_wool_burnous",
			"burnous",
			"a heavy $colour wool burnous",
			null,
			"This heavy $colour wool burnous is a heavy wool cloak with a hooded upper edge and a loose body meant to wrap around the shoulders. The cloth falls in deep folds and leaves the front easy to draw close against wind or dust. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			42.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_padded_wool_jubba",
			"jubba",
			"a padded $colour wool jubba",
			null,
			"This padded $colour wool jubba is a substantial robe with quilted thickness through the body and sleeves. The front hangs open or overlaps in heavy folds, making it suitable as a cool-season outer layer. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			58.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_izar_wrap",
			"izar",
			"a thick $colour wool izar",
			null,
			"This thick $colour wool izar is a broad thick wool wrap with enough length to pass around the body and fall in layered folds. The edges are plain and straight, letting the drape and overlap form most of the garment's shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fur_lined_caftan",
			"caftan",
			"a fine $colour fur-lined caftan",
			null,
			"This fine $colour fur-lined caftan is a front-opening coat with warm fur visible at the collar, cuffs, and inner edge. The skirt is cut for riding and walking, with side fullness that keeps the garment from binding. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1400.0,
			150.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_felt_winter_deel",
			"robe",
			"a $colour felt-lined steppe robe",
			null,
			"This $colour felt-lined steppe robe is a heavy belted outer robe with a dense inner lining and a front that wraps across the body. The sleeves and skirt are roomy, allowing it to sit over trousers and other cold-weather layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1350.0,
			56.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_quilted_winter_trousers",
			"trousers",
			"a pair of quilted $colour winter trousers",
			null,
			"These quilted $colour winter trousers are padded legwear stitched in rows to hold the filling evenly. They are cut roomy enough to pull over lighter layers, with reinforced hems where boots would rub. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fur_earflap_hat",
			"hat",
			"a fur earflap hat",
			null,
			"This fur earflap hat is a warm head covering with soft fur over the crown and flaps that fall to cover the ears. The shape is practical and rounded, with edges made thick enough to block wind. Natural variations in the fur show at the tips, seams, and places where the nap changes direction.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			50.0m,
			true,
			false,
			"fur",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_felt_stockings",
			"stockings",
			"a pair of thick felt stockings",
			null,
			"These thick felt stockings are close, warm leg coverings made from dense felt with a soft but compact surface. They are shaped to sit inside boots or beneath trousers, with plain edges and little ornament. The natural felt surface is plainly visible, with a practical, unpolished finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			12.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Stockings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_shawl",
			"shawl",
			"a thick $colour wool shawl",
			null,
			"This thick $colour wool shawl is a rectangular wrap with enough breadth to cover the shoulders and upper body. The cloth is thick and soft, falling in warm folds when drawn close. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			500.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_thick_cotton_shawl",
			"shawl",
			"a thick $colour cotton shawl",
			null,
			"This thick $colour cotton shawl is a rectangular wrap with enough breadth to cover the shoulders and upper body. The cloth is thick and soft, falling in warm folds when drawn close. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_goat_hair_shawl",
			"shawl",
			"a fine $colour goat-hair shawl",
			null,
			"This fine $colour goat-hair shawl is a rectangular wrap with enough breadth to cover the shoulders and upper body. The cloth is thick and soft, falling in warm folds when drawn close. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			320.0,
			120.0m,
			true,
			false,
			"cashmere",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_cotton_angavastram",
			"angavastram",
			"a thick $colour cotton angavastram",
			null,
			"This thick $colour cotton angavastram is a broad shoulder cloth that can be folded, draped, or drawn around the upper body. The cloth is rectangular and plainly edged, relying on the wearer's wrapping for its final shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_padded_silk_overrobe",
			"over-robe",
			"a padded $colour silk over-robe",
			null,
			"This padded $colour silk over-robe is a substantial outer robe with padded body panels and long sleeves. The added thickness gives the robe a rounded fall and makes the edges sit away from the underlayers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			180.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_heavy_beizi_overrobe",
			"beizi",
			"a heavy $colour beizi over-robe",
			null,
			"This heavy $colour beizi over-robe is a long over-robe with a straight open front, narrow sleeves, and side slits that let the underlayers move. The cloth falls in vertical lines from shoulder to lower hem, giving the garment a composed outer silhouette. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			760.0,
			130.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_padded_po_robe",
			"po",
			"a padded $colour po robe",
			null,
			"This padded $colour po robe is a long over-robe with broad sleeves, a wrapped front, and a steady fall below the knee. It is cut to sit over a jacket and trousers or skirt, adding a cleaner outer line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			135.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_lined_kosode",
			"kosode",
			"a lined $colour kosode robe",
			null,
			"This lined $colour kosode robe is a straight-seamed robe with small sleeve openings, a wrapped front, and an added lining for warmth. The collar runs in a clean diagonal line, and the sleeves hang in measured rectangles. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			62.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_layered_uchigi_set",
			"uchigi",
			"a layered $colour uchigi set",
			null,
			"This layered $colour uchigi set is a group of formal silk robe layers with straight panels and broad sleeves. The nested collars and hems create visible tiers, making the colour strongest where several layers overlap. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1100.0,
			220.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_work_apron",
			"apron",
			"a $colour linen work apron",
			null,
			"This $colour linen work apron is a plain protective cloth with a straight fall over the front of the body. The upper edge and ties are simple, keeping the garment easy to put on over ordinary working clothes. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hemp_work_apron",
			"apron",
			"a coarse hemp work apron",
			null,
			"This hemp work apron is a coarse protective layer with a straight bib or waist fall and plain ties. The cloth is sturdy, rough-textured, and meant to take stains before the clothing beneath does. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			6.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_leather_smith_apron",
			"apron",
			"a leather smith's apron",
			null,
			"This leather smith's apron is a heavy protective apron cut from sturdy leather with a broad chest and long lower fall. The surface is plain and tough, with visible creases and scuffs where tools, heat, and work would mark it. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			28.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_sleeve_guards",
			"sleeve guards",
			"a pair of $colour linen sleeve guards",
			null,
			"These $colour linen sleeve guards are narrow protective wraps for the forearms, made to cover cuffs and keep sleeves close during work. The cloth is light and washable, with simple ties or tucked ends controlling the fit. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			80.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Bracers",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_leather_work_gloves",
			"gloves",
			"a pair of leather work gloves",
			null,
			"These leather work gloves are plain hand coverings with sturdy palms, short cuffs, and practical seams between the fingers. The leather is thick enough to protect against abrasion without becoming armour. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			12.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Gloves",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_clean_linen_coif",
			"coif",
			"a clean $colour linen coif",
			null,
			"This clean $colour linen coif is a close-fitting head covering with rounded crown panels and ties below the jaw or at the nape. The edges are narrow and smooth, keeping the cloth neat beneath a hood, cap, or veil. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			85.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Coif",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_work_cap",
			"cap",
			"a $colour linen work cap",
			null,
			"This $colour linen work cap is a simple cloth cap with a rounded crown and a narrow lower edge. It is cut for everyday wear, sitting close to the head without broad brim or heavy ornament. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			95.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wide_work_sash",
			"sash",
			"a wide $colour work sash",
			null,
			"This wide $colour work sash is a broad wool band meant to gather loose clothing close to the body. The weave is firm rather than delicate, with squared ends and enough breadth to spread pressure across the waist. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_canvas_work_tabard",
			"tabard",
			"a $colour canvas work tabard",
			null,
			"This $colour canvas work tabard is a loose sleeveless canvas overgarment with open sides and a straight fall from the shoulders. It is cut to protect the front and back of clothing while leaving the arms free for work. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			14.0m,
			true,
			false,
			"canvas",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tabard",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drab_dyers_apron",
			"apron",
			"a stained $colour dye-worker's apron",
			null,
			"This stained $colour dye-worker's apron is a linen work layer marked by uneven colour and old splashes in the cloth. It has a plain protective fall and simple ties, prioritising coverage over neat display. The $colour tone is uneven and work-stained, strongest along the creases, edges, and places most handled.",
			SizeCategory.Small,
			ItemQuality.Standard,
			230.0,
			7.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_DrabColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_wool_work_tunic",
			"tunic",
			"a short $colour wool work tunic",
			null,
			"This short $colour wool work tunic is a short wool garment made from straight body panels with sleeves set in simply at the shoulder. The proportions are tidier than a field garment, with the hem kept clear enough for work in streets or workshops. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			440.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_clean_town_wool_tunic",
			"tunic",
			"a neat $colour town tunic",
			null,
			"This neat $colour town tunic is a straight wool garment made from straight body panels with sleeves set in simply at the shoulder. The proportions are tidier than a field garment, with the hem kept clear enough for work in streets or workshops. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			520.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_tucked_wool_work_gown",
			"gown",
			"a tucked $colour work gown",
			null,
			"This tucked $colour work gown is a long wool garment with a skirt that can be tucked up clear of work. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			700.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_market_veil",
			"veil",
			"a plain $colour market veil",
			null,
			"This plain $colour market veil is a simple rectangular cloth for covering the hair and shoulders during public work. The edges are narrow and practical, with no heavy ornament to snag or weigh it down. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			110.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_brooched_market_mantle",
			"mantle",
			"a neat $colour brooched mantle",
			null,
			"This neat $colour brooched mantle is a broad outer cloth meant to drape over the shoulders and upper body. The fastening is plainly visible near the shoulder, gathering the cloth into heavy folds without concealing how it is held. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			860.0,
			48.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_leather_trader_belt",
			"belt",
			"a broad leather trader's belt",
			null,
			"This broad leather trader's belt is cut wider than a common waist strap, with a firm body and a plain fastening. The edges are rubbed smooth and the outer face shows creases from being drawn tight over working clothes. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			34.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_market_headcloth",
			"headcloth",
			"a $colour wool market headcloth",
			null,
			"This $colour wool market headcloth is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			130.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_town_leine",
			"léine",
			"a clean $colour town léine",
			null,
			"This clean $colour town léine is a long linen tunic with full sleeves and a generous body cut from straight panels. The length and loose fall make it useful as both underlayer and visible garment. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			520.0,
			34.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_work_wool_cotte",
			"cotte",
			"a plain $colour wool work cotte",
			null,
			"This plain $colour wool work cotte is a long wool body garment cut close enough to layer under a surcoat or mantle. The sleeves are narrow through the forearm, and the skirt has enough fullness for walking. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges. With plain seams and a serviceable fall at the cuffs, neck, and hem.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			540.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_town_work_kirtle",
			"kirtle",
			"a tucked $colour work kirtle",
			null,
			"This tucked $colour work kirtle is a long wool garment with a skirt that can be tucked up clear of work. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			26.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sleeveless_work_surcoat",
			"surcoat",
			"a plain $colour sleeveless work surcoat",
			null,
			"This plain $colour sleeveless work surcoat is a sleeveless wool overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			560.0,
			20.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_plain_wool_shop_gown",
			"gown",
			"a neat $colour wool shop gown",
			null,
			"This neat $colour wool shop gown is a long wool garment with a tidy fall suitable for counter, desk, or workshop wear. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			760.0,
			52.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_clerk_wool_gown",
			"gown",
			"a plain $colour clerk's gown",
			null,
			"This plain $colour clerk's gown is a long wool garment with a tidy fall suitable for counter, desk, or workshop wear. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			780.0,
			56.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_neat_wool_town_hood",
			"hood",
			"a neat $colour wool town hood",
			null,
			"This neat $colour wool town hood is a wool head covering with a rounded crown and a fall that protects the neck and upper shoulders. The face opening is plainly finished, and the lower edge sits comfortably over other garments. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			240.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_merchants_wool_hood",
			"hood",
			"a fine $colour merchant's hood",
			null,
			"This fine $colour merchant's hood is a neatly cut wool hood with a controlled face opening and a tidy shoulder fall. The finish is cleaner than rough weather wear, with attention at the edge most visible around the face. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			44.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_shop_wimple",
			"wimple",
			"a clean $colour shop wimple",
			null,
			"This clean $colour shop wimple is a linen head and neck covering shaped by smooth folds around the throat and lower face. The fabric is light but close, leaving the outer edge neat enough to sit under a veil or hood. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			150.0,
			14.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_baker_cap",
			"cap",
			"a $colour linen baker's cap",
			null,
			"This $colour linen baker's cap is a light work cap with a soft crown and a narrow band to hold it close. The cloth is washable-looking and plain, suited to keeping hair contained around flour and ovens. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			100.0,
			7.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_light_linen_work_camisa",
			"camisa",
			"a light $colour linen work camisa",
			null,
			"This light $colour linen work camisa is a light linen shirt with straight-cut sleeves, a simple neck slit, and a loose body. The cloth is fine enough for warm weather or under-layering beneath woollen garments. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			300.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_market_toca",
			"toca",
			"a $colour cotton market toca",
			null,
			"This $colour cotton market toca is a wrapped headcloth with enough length to cover the hair and frame the sides of the face. The fabric is light and foldable, with the edge kept plain for repeated tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			9.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_work_pellote",
			"pellote",
			"a plain $colour wool work pellote",
			null,
			"This plain $colour wool work pellote is a sleeveless wool overgarment with deep side openings and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			26.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_plain_work_dalmatic",
			"dalmatic",
			"a plain $colour work dalmatic",
			null,
			"This plain $colour work dalmatic is a loose robe with broad sleeves, a straight body, and a dignified fall from shoulder to hem. The cut leaves room for an underlayer, while the neck and cuffs are the clearest places for visible finishing. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			740.0,
			32.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_work_sagion",
			"sagion",
			"a short $colour work sagion",
			null,
			"This short $colour work sagion is a compact wool cloak cut to cover the shoulders without hanging too low over the arms. The edges are plain and the fastening area is kept simple for daily town wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			720.0,
			26.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_shop_maforion",
			"maforion",
			"a plain $colour shop maforion",
			null,
			"This plain $colour shop maforion is a broad head-and-shoulder veil with a long fall around the face and upper body. The cloth is cut generously enough to layer over a robe without losing its clean outline. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			130.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_cotton_work_qamis",
			"qamis",
			"a short $colour cotton work qamis",
			null,
			"This short $colour cotton work qamis is a short, workmanlike shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_work_sirwal",
			"sirwal",
			"a pair of $colour linen work sirwal",
			null,
			"These $colour linen work sirwal are loose drawstring trousers with a roomy seat and tapered lower legs. The cloth gathers softly at the waist and falls in practical folds around the thighs. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			330.0,
			12.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_shop_turban",
			"turban",
			"a clean $colour linen shop turban",
			null,
			"This clean $colour linen shop turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Standard,
			170.0,
			12.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_waist_apron",
			"apron",
			"a $colour cotton waist apron",
			null,
			"This $colour cotton waist apron is a plain protective cloth with a straight fall over the front of the body. The upper edge and ties are simple, keeping the garment easy to put on over ordinary working clothes. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			170.0,
			7.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sleeveless_work_jubba",
			"jubba",
			"a sleeveless $colour work jubba",
			null,
			"This sleeveless $colour work jubba is a loose wool overgarment with open armholes and a straight body. It is cut to add a protective layer over a qamis while leaving the arms free for labour. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bazaar_merchant_jubba",
			"jubba",
			"a neat $colour bazaar jubba",
			null,
			"This neat $colour bazaar jubba is a loose wool robe with long sleeves, a straight body, and a generous front opening. It is cut to layer over lighter garments, with the edges and cuffs given the most visible finish. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			64.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_scribe_linen_sleeves",
			"sleeves",
			"a pair of $colour linen scribe sleeves",
			null,
			"These $colour linen scribe sleeves are separate forearm coverings made to protect robe or tunic sleeves from ink and desk wear. The fabric is light, smooth, and gathered just enough to stay clear of the hands. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			70.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Bracers",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_market_izar_wrap",
			"izar",
			"a $colour market izar wrap",
			null,
			"This $colour market izar wrap is a broad linen wrap with enough length to pass around the body and fall in layered folds. The edges are plain and straight, letting the drape and overlap form most of the garment's shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_work_caftan",
			"caftan",
			"a short $colour work caftan",
			null,
			"This short $colour work caftan is a practical front-opening coat with a trimmed skirt and sleeves kept clear of the hands. It fastens close enough for movement in a market or workshop, without the length of a formal robe. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			720.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rus_town_rubakha",
			"rubakha",
			"a neat $colour town rubakha",
			null,
			"This neat $colour town rubakha is a long shirt-like garment with a slit neck, narrow cuffs, and straight body panels. The hem falls low enough to blouse over a belt, and the sleeves are cut for work beneath a heavier coat. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			430.0,
			30.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_tucked_poneva_work_skirt",
			"poneva",
			"a tucked $colour poneva work skirt",
			null,
			"This tucked $colour poneva work skirt is a wrapped wool skirt arranged to keep the lower edge clear of dirty floors or tools. The cloth overlaps at the waist and falls in practical folds across the hips. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			20.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_furriers_leather_apron",
			"apron",
			"a leather furrier's apron",
			null,
			"This leather furrier's apron is a broad work apron with a long fall and smooth enough surface to shed hair and scraps. The upper edge sits high on the body, while the lower portion is plain and durable. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			24.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_felt_market_cap",
			"cap",
			"a neat $colour felt market cap",
			null,
			"This neat $colour felt market cap is a market felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			10.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wide_leather_work_belt",
			"belt",
			"a wide leather work belt",
			null,
			"This wide leather work belt is a broad strap of sturdy leather with a plain buckle and reinforced holes. It is shaped to sit firmly over work clothes, with scuffed edges and a practical, unadorned face. Natural variations in the leather show along the grain, flex marks, and cut edge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			22.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_mounted_trader_coat",
			"coat",
			"a neat $colour mounted trader's coat",
			null,
			"This neat $colour mounted trader's coat is a long wool coat with a practical front opening and a skirt cut for sitting in a saddle. The cloth is better finished than field wear, but the seams remain sturdy and work-minded. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			900.0,
			70.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_cotton_work_dhoti",
			"dhoti",
			"a short $colour cotton work dhoti",
			null,
			"This short $colour cotton work dhoti is a short work lower garment made from a rectangular length of cotton. The fabric is folded and passed around the waist and legs, making its shape depend on the wrapping rather than stitching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			8.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_work_waistcloth",
			"waistcloth",
			"a $colour cotton workshop waistcloth",
			null,
			"This $colour cotton workshop waistcloth is a practical rectangular wrap for tying around the waist while working. The cloth is light enough to tuck up, and the edges are plainly finished to survive repeated washing and tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			7.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_weavers_cotton_shouldercloth",
			"shouldercloth",
			"a $colour weaver's shouldercloth",
			null,
			"This $colour weaver's shouldercloth is a practical cotton drape sized to cover one shoulder and part of the chest. The cloth is light, foldable, and easy to shift aside while working with thread or tools. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			9.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_market_cotton_turban",
			"turban",
			"a clean $colour market turban",
			null,
			"This clean $colour market turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			12.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_trade_bordered_uttariya",
			"uttariya",
			"a $colour trade-bordered uttariya",
			null,
			"This $colour trade-bordered uttariya is a long draped shoulder cloth meant to pass over one or both shoulders. The edges are kept straight, and the fabric is light enough to fold without making a bulky knot. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			230.0,
			30.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_south_indian_work_veshti",
			"veshti",
			"a tucked $colour cotton work veshti",
			null,
			"This tucked $colour cotton work veshti is a rectangular cotton lower cloth wrapped around the waist and falling in straight folds. The edges are clean and visible, and the front can be tucked to keep the hem clear for movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			250.0,
			9.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_bazaar_sari",
			"sari",
			"a tucked $colour cotton bazaar sari",
			null,
			"This tucked $colour cotton bazaar sari is a long wrapped garment with enough length to form both lower drape and upper fold. The cloth relies on pleating, tucking, and wrapping rather than seams, leaving the edge visible along the fall. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			480.0,
			18.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_work_shan_jacket",
			"jacket",
			"a plain $colour work shan jacket",
			null,
			"This plain $colour work shan jacket is a short cross-collar jacket with straight sleeves, a wrapped front, and a tidy waist-length body. The garment is cut for movement, with the front overlap and cuffs giving it most of its visible shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			330.0,
			18.0m,
			true,
			false,
			"ramie",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Jacket",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_short_work_trousers",
			"trousers",
			"a pair of short $colour work trousers",
			null,
			"These short $colour work trousers are practical trousers cut shorter through the lower leg for workshop and market movement. The waist is gathered plainly, and the hems are kept simple so they can be tucked or left loose. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			300.0,
			12.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_shopkeeper_robe",
			"robe",
			"a neat $colour shopkeeper's robe",
			null,
			"This neat $colour shopkeeper's robe is a long silk robe with orderly sleeves, a tidy collar, and a fall suited to indoor trade or counter work. It is more polished than a labourer's jacket but less ornate than court dress. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			560.0,
			76.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_merchant_headscarf",
			"headscarf",
			"a neat $colour merchant headscarf",
			null,
			"This neat $colour merchant headscarf is a square or rectangular cloth for covering the head and tying close at the neck or nape. The edges are plainly hemmed, and the folds can be drawn low for shade or kept tidy for town wear. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Small,
			ItemQuality.Good,
			90.0,
			10.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_song_cotton_work_apron",
			"apron",
			"a $colour cotton work apron",
			null,
			"This $colour cotton work apron is a plain protective cloth with a straight fall over the front of the body. The upper edge and ties are simple, keeping the garment easy to put on over ordinary working clothes. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			7.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_work_jeogori",
			"jeogori",
			"a plain $colour work jeogori",
			null,
			"This plain $colour work jeogori is a short cross-collar jacket with narrow sleeves and ties at the front. The body ends high at the waist, leaving room for trousers or a skirt below. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			340.0,
			18.0m,
			true,
			false,
			"ramie",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Jacket",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_work_baji",
			"baji",
			"a pair of $colour work baji",
			null,
			"These $colour work baji are loose trousers with a gathered waist, full seat, and narrowing lower legs. The cloth is cut for easy movement beneath a jacket or robe, with plain seams and practical hems. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			340.0,
			14.0m,
			true,
			false,
			"ramie",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_market_chima",
			"chima",
			"a plain $colour market chima",
			null,
			"This plain $colour market chima is a full skirt with a gathered upper edge and a broad fall from waist to lower hem. The fabric spreads in soft folds and is meant to sit below a short jacket or over-robe. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			430.0,
			20.0m,
			true,
			false,
			"ramie",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Long_Skirt",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_goryeo_shop_po",
			"po",
			"a neat $colour shop po",
			null,
			"This neat $colour shop po is a long over-robe with broad sleeves, a wrapped front, and a steady fall below the knee. It is cut to sit over a jacket and trousers or skirt, adding a cleaner outer line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
			SizeCategory.Normal,
			ItemQuality.Good,
			580.0,
			82.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_work_kosode",
			"kosode",
			"a plain $colour work kosode",
			null,
			"This plain $colour work kosode is a straight-seamed robe with small sleeve openings, a wrapped front, and a narrow collar. The body is made from rectangular panels, giving it a clean fall and visible folded overlap at the chest. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			480.0,
			22.0m,
			true,
			false,
			"ramie",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_work_hakama",
			"hakama",
			"a pair of $colour work hakama",
			null,
			"These $colour work hakama are divided skirt-like trousers with deep folds falling from the waist. The legs are broad and straight, giving the garment a formal drape while still allowing movement. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			380.0,
			18.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_hemp_work_apron",
			"apron",
			"a hemp work apron",
			null,
			"This hemp work apron is a coarse protective layer with a straight bib or waist fall and plain ties. The cloth is sturdy, rough-textured, and meant to take stains before the clothing beneath does. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			170.0,
			6.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_japanese_yumaki_wrap",
			"yumaki",
			"a $colour yumaki waist wrap",
			null,
			"This $colour yumaki waist wrap is a simple hemp cloth tied around the waist as an apron-like working layer. The fabric is rectangular, foldable, and light enough to overlap without becoming bulky. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			8.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_black_monastic_habit",
			"habit",
			"a black wool monastic habit",
			null,
			"This black wool monastic habit is cut as a long, loose robe with full sleeves and a plain round neck. The cloth falls in heavy vertical folds from shoulder to ankle, leaving enough breadth for an undertunic beneath. Its colour and cut are deliberately austere, with only the dark wool, simple seams, and even hem giving it shape.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_black_scapular",
			"scapular",
			"a black wool scapular",
			null,
			"This black wool scapular is made as a long, narrow over-panel to hang down the front and back of the body. The shoulders are plain and the sides are left open, so the garment reads as a simple vertical strip rather than a coat. The wool is closely woven but unadorned, with a straight hem and restrained black surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tabard",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_black_cowl",
			"cowl",
			"a black wool cowl",
			null,
			"This black wool cowl is a heavy hooded overgarment made to fall in a single dark mass around the shoulders. The hood is deep, the body loose, and the hem broad enough to drape over a habit beneath it. Its surface is plain and matte, with only the weight of the wool and the shadow of the hood giving it emphasis.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_white_monastic_tunic",
			"tunic",
			"an undyed wool monastic tunic",
			null,
			"This undyed wool monastic tunic is a long, straight robe with full sleeves and a modest neckline. The pale wool keeps a natural cream cast, broken only by simple seams at the shoulders, sleeves, and sides. It is practical rather than fine, shaped to hang plainly from the body under a scapular, cowl, or mantle.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			820.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_black_over_scapular",
			"scapular",
			"a black wool over-scapular",
			null,
			"This black wool over-scapular is a severe sleeveless panel meant to be worn over a pale habit. It hangs as two straight strips joined at the shoulders, framing the body without closing at the sides. The contrast between its dark wool and plain geometry makes it visibly distinct without adding ornament.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tabard",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_white_carthusian_habit",
			"habit",
			"a white wool hermit's habit",
			null,
			"This white wool hermit's habit is cut as a roomy, ankle-length robe with broad sleeves and a sober fall. The cloth is undyed, thick, and slightly rough, giving the garment a pale but workmanlike appearance. It is deliberately spare in construction, with plain seams, a simple waistline, and no decorative edging.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_canon_black_habit",
			"habit",
			"a black wool canon's habit",
			null,
			"This black wool canon's habit is a long clerical robe with ample sleeves and a neat, restrained cut. The wool is darker and smoother than most labouring garments, but it keeps a sober face without bright trim. Its hem and cuffs are carefully finished, giving it a disciplined appearance suitable for choir or school use.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_white_rochet",
			"rochet",
			"a white linen rochet",
			null,
			"This white linen rochet is a clean, knee-length clerical overgarment with narrow sleeves and a squared body. The linen is lighter than wool but crisply finished, allowing it to fall in pale folds over a darker habit or gown. Its edges are plain and tidy, relying on whiteness and neat cutting rather than decoration.",
			SizeCategory.Normal,
			ItemQuality.Good,
			320.0,
			28.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_friar_grey_habit",
			"habit",
			"a grey wool friar's habit",
			null,
			"This grey wool friar's habit is a loose, ankle-length robe with simple sleeves and a practical hooded fall. The wool is coarse enough to show its weave, with a muted grey colour that avoids costly dye. Its shape is deliberately plain, meant to be gathered with a cord rather than tailored closely.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			20.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_rope_cincture",
			"cincture",
			"a knotted rope cincture",
			null,
			"This rope cincture is twisted from sturdy hemp into a rough belt for tying around a habit. Several visible knots break the line of the cord, leaving the ends to hang plainly at one side. The fibre is pale, dry, and uneven, more practical than decorative.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			4.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_white_friar_habit",
			"habit",
			"a white wool friar's habit",
			null,
			"This white wool friar's habit is a long, simple robe with full sleeves and a straight front. The undyed wool has a pale cream tone, kept clean enough to stand out beneath a darker travelling cloak. Its construction is modest but orderly, with a close neckline, even cuffs, and a hem meant to clear the feet.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_black_friar_cappa",
			"cappa",
			"a black wool friar's cappa",
			null,
			"This black wool friar's cappa is a full open cloak made to hang over a pale habit. The cloth gathers heavily at the shoulders and spreads in a dark sweep down the body. A simple hood or cape-like upper fold gives it sheltering weight without bright trim or fittings.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			720.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_brown_friar_habit",
			"habit",
			"a brown wool friar's habit",
			null,
			"This brown wool friar's habit is a plain robe with a deep body, loose sleeves, and enough length to cover the ankles. The brown wool is practical and subdued, with a slightly uneven texture that shows the hand of the cloth. It is shaped to be belted at the waist and worn under a mantle when needed.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			800.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_white_friar_mantle",
			"mantle",
			"a white wool friar's mantle",
			null,
			"This white wool friar's mantle is a broad outer wrap with a pale, even face and cleanly finished borders. It drapes across the shoulders and down the back, leaving the arms free beneath the loose fall of cloth. The mantle is finer than a labourer's cloak but remains modest, with no ornament beyond its colour and full sweep.",
			SizeCategory.Normal,
			ItemQuality.Good,
			640.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_simple_nun_habit",
			"habit",
			"a dark wool nun's habit",
			null,
			"This dark wool nun's habit is a long, full robe with modest sleeves and a straight, unshaped body. The dark cloth is plain and heavy, falling in quiet folds from the shoulders to the feet. Its cut is conservative, leaving the garment defined by coverage, simplicity, and an orderly hem.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			820.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_nun_wimple_veil",
			"veil",
			"a white linen wimple and veil",
			null,
			"This white linen wimple and veil is arranged as layered headcloths that frame the face and cover the throat. The linen is plain but clean, with folded edges that sit smoothly beneath a darker veil or hood. Its appearance is modest and structured, giving the head and shoulders a pale, enclosed outline.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			12.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_poor_clare_grey_habit",
			"habit",
			"a grey wool sister's habit",
			null,
			"This grey wool sister's habit is a plain, ankle-length robe with simple sleeves and a loosely falling body. The cloth is muted and inexpensive, with a rough grey surface that avoids display. It is shaped for a cord belt and veil, making its austerity visible in every line.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_poor_clare_veil",
			"veil",
			"a plain linen sister's veil",
			null,
			"This plain linen sister's veil is a simple white head covering with little shaping beyond its folded front edge. It falls over the hair and shoulders in modest layers, leaving the face open. The fabric is light, matte, and undecorated, suited to an austere habit rather than courtly dress.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_black_clerical_gown",
			"gown",
			"a black wool clerical gown",
			null,
			"This black wool clerical gown is a long, sober garment with straight sleeves and a restrained neckline. The cloth is smooth enough for formal town or church use, but its colour and lack of ornament keep it severe. It hangs neatly from shoulder to ankle, with a plain front and carefully finished hems.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_white_alb",
			"alb",
			"a white linen alb",
			null,
			"This white linen alb is a long liturgical robe with narrow sleeves and a full, clean fall to the feet. The linen is light but well finished, showing broad pale folds where it gathers at the waist. Its edges are plain and carefully hemmed so that brighter vestments can sit clearly over it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			460.0,
			36.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_amice",
			"amice",
			"a white linen amice",
			null,
			"This white linen amice is a rectangular cloth sized to sit around the neck and shoulders beneath other vestments. Its edges are neatly folded, and small ties are worked into the corners to keep the cloth close. The linen is plain, pale, and smooth, serving as a clean visible layer at the collar.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_linen_cincture",
			"cincture",
			"a white linen cincture",
			null,
			"This white linen cincture is a narrow corded sash for drawing in a loose robe at the waist. It is braided from pale linen with a simple hand and modest hanging ends. The surface is clean and undecorated, giving it the appearance of an ordered working tie rather than a rich belt.",
			SizeCategory.Small,
			ItemQuality.Standard,
			80.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_stole",
			"stole",
			"a $colour silk stole",
			null,
			"This $colour silk stole is a long, narrow band of cloth with neatly weighted ends and a smooth lustre. The silk catches light along its length, making the $colour tone appear richer where the fabric bends. Its construction is formal and restrained, with straight edges and a clearly ceremonial fall.",
			SizeCategory.Small,
			ItemQuality.Good,
			110.0,
			50.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_maniple",
			"maniple",
			"a $colour silk maniple",
			null,
			"This $colour silk maniple is a short band of fine cloth made to hang from the forearm. The silk is light and smooth, with the $colour shade most visible across the folded central panel. Its ends are carefully squared, giving the small vestment a deliberate and formal outline.",
			SizeCategory.Small,
			ItemQuality.Good,
			60.0,
			30.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Bracer",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_chasuble",
			"chasuble",
			"a fine $colour silk chasuble",
			null,
			"This fine $colour silk chasuble is cut as a broad outer vestment with a head opening and a sweeping fall over the torso. The silk lies in rounded folds, its $colour surface broken by the shadow of the shoulders and the heavy lower edge. It is visibly more formal than ordinary clothing, relying on rich cloth, breadth, and clean ceremonial shape.",
			SizeCategory.Normal,
			ItemQuality.Good,
			720.0,
			160.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Poncho",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_dalmatic_vestment",
			"dalmatic",
			"a fine $colour silk dalmatic",
			null,
			"This fine $colour silk dalmatic is a loose, straight vestment with broad sleeves and a squared body. The $colour silk hangs smoothly from the shoulders, leaving the sleeve openings wide and visible. Its formal cut gives it a robe-like dignity while keeping the shape distinct from a closed cloak or mantle.",
			SizeCategory.Normal,
			ItemQuality.Good,
			650.0,
			140.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_processional_cope",
			"cope",
			"a fine $colour silk cope",
			null,
			"This fine $colour silk cope is a great open cloak with a broad semicircular sweep and a smooth outer face. The silk gathers at the shoulders before spreading into a full ceremonial fall, showing the $colour most clearly across the back. Its edge is carefully finished, making the whole garment read as formal outerwear rather than a travelling cloak.",
			SizeCategory.Normal,
			ItemQuality.Good,
			900.0,
			180.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_bishop_mitre",
			"mitre",
			"a fine white silk mitre",
			null,
			"This fine white silk mitre is a tall folded headpiece with two stiffened peaks rising above the brow. The silk is pale and smooth, with carefully joined seams holding the upright form. Narrow hanging bands fall from the back, making the headwear unmistakably formal without adding other carried objects.",
			SizeCategory.Small,
			ItemQuality.Good,
			160.0,
			140.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_bishop_gloves",
			"gloves",
			"a pair of fine white liturgical gloves",
			null,
			"These fine white liturgical gloves are sewn from smooth silk with close fingers and neat wrist openings. The fabric has a pale sheen and lies cleanly against the hand when worn. Their finish is more delicate than working gloves, with the emphasis on spotless cloth and careful seams.",
			SizeCategory.Small,
			ItemQuality.Good,
			90.0,
			80.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Gloves",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_latin_archbishop_pallium",
			"pallium",
			"a white wool pallium band",
			null,
			"This white wool pallium band is a narrow looped vestment with hanging ends that fall over the chest and back. The wool is fine, pale, and tightly woven, giving the small garment a clean ceremonial presence. Dark cross-like markings are worked into the band as visible decoration rather than separate metal fittings.",
			SizeCategory.Small,
			ItemQuality.Good,
			140.0,
			100.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_black_riassa",
			"riassa",
			"a black wool riassa",
			null,
			"This black wool riassa is a long outer robe with a loose body and broad, unhurried sleeves. The cloth is sober and dark, falling from the shoulders in a straight vertical line. Its construction is plain but ample, giving the wearer a formal clerical outline without bright trimming.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_monastic_mantle",
			"mantle",
			"a black wool monastic mantle",
			null,
			"This black wool monastic mantle is a wide outer wrap with enough breadth to cover the shoulders and much of the body. Its folds are heavy and dark, giving the garment a solemn, enclosing silhouette. The edges are simple and unembroidered, with visual weight supplied by the cloth itself.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			34.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_black_klobuk",
			"klobuk",
			"a black veiled klobuk",
			null,
			"This black veiled klobuk combines a firm dark cap with a hanging veil that falls around the back and sides. The wool is matte and close, with the veil softening the hard line of the cap. It frames the head in a severe, recognisable shape without relying on jewels or bright colour.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_womens_monastic_veil",
			"veil",
			"a black monastic veil",
			null,
			"This black monastic veil is a plain wool head covering made to fall over the hair, shoulders, and upper back. The fabric is soft enough to fold close to the face but heavy enough to keep a dark, settled outline. It is modest and undecorated, with only the clean fall of cloth marking its form.",
			SizeCategory.Small,
			ItemQuality.Standard,
			200.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_sticharion",
			"sticharion",
			"a white linen sticharion",
			null,
			"This white linen sticharion is a long, straight liturgical robe with close sleeves and a pale, even surface. The linen falls cleanly to the feet, forming a bright underlayer beneath richer vestments. Its seams are tidy and restrained, emphasizing length and whiteness rather than decoration.",
			SizeCategory.Normal,
			ItemQuality.Good,
			480.0,
			40.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_orarion",
			"orarion",
			"a $colour silk orarion",
			null,
			"This $colour silk orarion is a long narrow vestment meant to lie over the shoulder in a clear vertical line. The silk is smooth and formal, making the $colour surface catch light along its length. Its edges are straight and carefully finished, giving the band a precise liturgical appearance.",
			SizeCategory.Small,
			ItemQuality.Good,
			130.0,
			60.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_epitrachelion",
			"epitrachelion",
			"a $colour silk epitrachelion",
			null,
			"This $colour silk epitrachelion is a paired stole joined at the neck and falling in two long panels down the front. The $colour silk has a formal sheen, broken by the fold where the fabric sits around the collar. Its length, symmetry, and careful edging make it distinct from an ordinary scarf.",
			SizeCategory.Small,
			ItemQuality.Good,
			190.0,
			70.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_epimanikia",
			"epimanikia",
			"a pair of $colour silk epimanikia",
			null,
			"These $colour silk epimanikia are fitted cuffs made to wrap around the lower sleeves. The silk panels are small but carefully shaped, with ties or narrow bands visible at the inner wrist. Their $colour surface gives a bright, formal finish to the hands and forearms.",
			SizeCategory.Small,
			ItemQuality.Good,
			80.0,
			50.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Bracers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_phelonion",
			"phelonion",
			"a fine $colour silk phelonion",
			null,
			"This fine $colour silk phelonion is a wide sleeveless outer vestment with a rounded fall from the shoulders. The front is cut to clear the hands while the back drops in a fuller sweep, showing the $colour silk in deep folds. Its form resembles a ceremonial mantle, but the head opening and continuous body give it a distinctive liturgical shape.",
			SizeCategory.Normal,
			ItemQuality.Good,
			780.0,
			170.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Poncho",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_sakkos",
			"sakkos",
			"a fine $colour silk sakkos",
			null,
			"This fine $colour silk sakkos is a square-cut outer vestment with broad sleeves and a structured, tunic-like body. The sides are visibly joined by small fastenings or ties, leaving the garment formal but not closely fitted. Its $colour silk and deliberate shape make it more imposing than an ordinary robe.",
			SizeCategory.Normal,
			ItemQuality.Good,
			800.0,
			210.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_omophorion",
			"omophorion",
			"a fine white silk omophorion",
			null,
			"This fine white silk omophorion is a broad band made to lie over the shoulders and fall across the chest. The silk is pale, smooth, and substantial enough to hold its line over heavier vestments. Dark cross-like markings break the white field, giving the band an episcopal and ceremonial appearance.",
			SizeCategory.Small,
			ItemQuality.Good,
			220.0,
			120.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Scarf",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_eastern_kamilavkion",
			"kamilavkion",
			"a black felt kamilavkion",
			null,
			"This black felt kamilavkion is a stiff cylindrical cap with a flat crown and straight sides. The felt is dense and matte, holding the cap in a crisp, upright shape. Its severe black surface and clean silhouette give it a formal clerical character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			170.0,
			20.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_islamic_plain_imam_qamis",
			"qamis",
			"a plain white cotton imam qamis",
			null,
			"This plain white cotton qamis is a long, straight robe with modest sleeves and a clean front. The cotton is light but opaque, falling in simple folds suited to warm interiors and layered dress. Its whiteness and spare construction make it visibly clean without making it ornate.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			18.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_islamic_plain_imam_turban",
			"turban",
			"a plain white cotton imam turban",
			null,
			"This plain white cotton imam turban is made from a long length of cloth wrapped into a neat, layered head covering. The cotton is clean and soft, with overlapping folds forming a modest rounded shape. No jewel or badge marks it, leaving its presence in the whiteness and regularity of the wrap.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_islamic_scholars_jubba",
			"jubba",
			"a dark wool scholar's jubba",
			null,
			"This dark wool scholar's jubba is a long outer robe with full sleeves and a dignified, loose body. The wool is smoother than common work cloth, giving the dark surface a measured formal weight. It is cut for layering over a qamis and sirwal, with plain hems and a calm, authoritative silhouette.",
			SizeCategory.Normal,
			ItemQuality.Good,
			850.0,
			48.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_islamic_taylasan",
			"taylasan",
			"a dark wool taylasan hood",
			null,
			"This dark wool taylasan is a hooded head covering with a short shoulder fall and a sober drape. The wool is fine enough for learned or official wear, but its colour remains restrained. Its shape frames the head and upper shoulders without becoming a full cloak.",
			SizeCategory.Small,
			ItemQuality.Good,
			240.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_islamic_qadi_turban",
			"turban",
			"a fine white scholar's turban",
			null,
			"This fine white scholar's turban is wrapped from a long, clean length of cotton into a careful formal shape. The folds are regular and layered, giving the headwear more presence than a simple working wrap. Its colour is plain, but the neatness of the cotton and the size of the winding give it dignity.",
			SizeCategory.Small,
			ItemQuality.Good,
			300.0,
			26.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sufi_patched_khirqa",
			"khirqa",
			"a patched wool khirqa",
			null,
			"This patched wool khirqa is a loose robe assembled from visibly different pieces of worn cloth. The patches vary in tone and texture, but the whole garment keeps a roughly ordered robe shape with long sleeves and a full body. It looks intentionally poor and mended, using its roughness as its most visible feature.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sufi_wool_sash",
			"sash",
			"a coarse wool Sufi sash",
			null,
			"This coarse wool Sufi sash is a plain length of cloth meant to bind a robe at the waist. The wool is rough, dark, and slightly uneven, with frayed-looking ends left visible. It is more severe than decorative, adding a practical horizontal line to otherwise loose clothing.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sufi_felt_cap",
			"cap",
			"a plain felt dervish cap",
			null,
			"This plain felt dervish cap is a simple, close head covering shaped from dense felt. Its sides are unadorned, and the crown is rounded without embroidery or metal fittings. The material gives it a soft but durable outline suited to rough travel or simple lodgings.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			10.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sufi_coarse_wool_cloak",
			"cloak",
			"a coarse wool Sufi cloak",
			null,
			"This coarse wool Sufi cloak is a heavy open wrap with a rough, plainly woven surface. It falls from the shoulders in uneven dark folds, large enough to cover a patched robe or plain qamis beneath. The cloak is austere rather than elegant, with a practical weight and little concern for polish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			980.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewish_tallit_gadol",
			"tallit",
			"a white wool tallit gadol",
			null,
			"This white wool tallit gadol is a broad rectangular prayer shawl with dark narrow stripes near the ends. The wool is light enough to drape over the shoulders but large enough to cover much of the upper body. Long fringes hang from the corners, making the garment visibly distinct from an ordinary mantle.",
			SizeCategory.Normal,
			ItemQuality.Good,
			420.0,
			40.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewish_tallit_katan",
			"tallit",
			"a white wool tallit katan",
			null,
			"This white wool tallit katan is a small four-cornered undergarment with a simple head opening and straight hanging panels. The wool is plain and pale, meant to sit beneath or between ordinary clothing layers. Fringes at the corners mark it clearly even though the body of the garment is spare.",
			SizeCategory.Small,
			ItemQuality.Standard,
			230.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Vest",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewish_skullcap",
			"skullcap",
			"a plain wool skullcap",
			null,
			"This plain wool skullcap is a small close-fitting cap made to sit neatly on the crown of the head. The wool is dark, soft, and unornamented, with shallow seams shaping the rounded form. It is modest and compact, giving little visual weight beyond its careful fit.",
			SizeCategory.Small,
			ItemQuality.Standard,
			60.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Skullcap",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewish_scholars_robe",
			"robe",
			"a dark wool scholar's robe",
			null,
			"This dark wool scholar's robe is a long, dignified garment with full sleeves and a restrained front. The wool is smooth enough to suggest careful making, while the dark colour keeps it sober. It is shaped for town and synagogue use, with neat hems and an uncluttered fall over inner layers.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			44.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hindu_white_priest_dhoti",
			"dhoti",
			"a white cotton priest's dhoti",
			null,
			"This white cotton priest's dhoti is a long lower cloth folded and wrapped into a clean, ankle-length drape. The cotton is light, plain, and kept pale, with the broad folds showing the garment's rectangular origin. Its simplicity and whiteness make it visibly more ritual-clean than a colourful everyday lower cloth.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			16.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hindu_white_uttariya",
			"uttariya",
			"a white cotton uttariya",
			null,
			"This white cotton uttariya is a rectangular shoulder cloth made to pass over the upper body in a simple drape. The cotton is thin, clean, and unpatterned, showing soft folds where it crosses the shoulder. It is modest and light, meant to sit over bare skin or a simple underlayer without ornament.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			10.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hindu_kaupina",
			"kaupina",
			"a plain cotton kaupina",
			null,
			"This plain cotton kaupina is a very simple loincloth made from a narrow length of white cloth. It is cut and folded for minimal coverage, with no decoration beyond the clean edges of the fabric. The cotton is light and practical, giving the garment an intentionally austere appearance.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			4.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hindu_ochre_ascetic_wrap",
			"wrap",
			"an ochre cotton ascetic wrap",
			null,
			"This ochre cotton ascetic wrap is a long rectangular cloth arranged to cover the body in loose folds. The cotton has a warm earth-coloured tone and a plain, unshaped surface. Its form is intentionally simple, relying on wrapping and tucking rather than tailoring.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hindu_ochre_shoulder_cloth",
			"cloth",
			"an ochre cotton shoulder cloth",
			null,
			"This ochre cotton shoulder cloth is a narrow rectangular drape for the upper body. The fabric is light and plain, with its warm colour showing most clearly along the broad hanging panels. It can fall over one shoulder or both, giving a simple ascetic layer above a lower wrap.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			8.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jain_white_ascetic_robe",
			"robe",
			"a white cotton ascetic robe",
			null,
			"This white cotton ascetic robe is a plain, lightly wrapped garment with a straight and modest fall. The cotton is pale, unadorned, and deliberately simple, showing only folds and hems. Its appearance is white-clad and austere, avoiding rich dye, trim, or display.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			16.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jain_white_shoulder_wrap",
			"wrap",
			"a white cotton shoulder wrap",
			null,
			"This white cotton shoulder wrap is a plain rectangular cloth made to cover the upper body in a modest drape. The cotton is light and uncoloured, with neat edges and no visible decoration. It sits quietly over a robe or lower cloth, reinforcing a spare white-clad appearance.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			8.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_buddhist_plain_underrobe",
			"underrobe",
			"a plain hemp monastic underrobe",
			null,
			"This plain hemp monastic underrobe is a straight, cross-wrapped robe with close sleeves and a practical hem. The hemp is sturdy, dull, and lightly textured, giving the garment a subdued working surface. It is meant to sit beneath a kasaya or mantle, with no decoration competing with the outer cloth.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			16.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_buddhist_patched_kasaya",
			"kasaya",
			"a patched cotton kasaya",
			null,
			"This patched cotton kasaya is a rectangular mantle built from many sewn panels. The cotton pieces are arranged in a deliberate grid, with seams forming a visible patchwork field across the cloth. It drapes over the shoulder and upper body, looking formal through pattern and construction rather than luxury.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			320.0,
			24.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_buddhist_nun_robe",
			"robe",
			"a dark hemp nun robe",
			null,
			"This dark hemp nun robe is a plain wrapped garment with long sleeves and a full, modest body. The hemp is firm and matte, with a darkened surface that hides wear better than pale cloth. Its lines are simple and practical, leaving the robe suited to layered monastic dress.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			560.0,
			18.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_buddhist_formal_kesa",
			"kesa",
			"a fine silk kesa mantle",
			null,
			"This fine silk kesa mantle is a rectangular patchwork garment made from smooth, carefully joined panels. The silk has a muted ceremonial sheen, and the panel layout gives the cloth a measured, gridded order. It is worn as an outer mantle, more formal than a common kasaya but still defined by patchwork construction.",
			SizeCategory.Normal,
			ItemQuality.Good,
			360.0,
			110.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_buddhist_travelling_mantle",
			"mantle",
			"a dark wool monastic travelling mantle",
			null,
			"This dark wool monastic travelling mantle is a broad outer cloth for sheltering a robe on the road. The wool is plain and serviceable, falling in wide folds over the shoulders and down the back. Its shape is practical and unornamented, closer to a traveller’s wrap than a formal temple mantle.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_buddhist_black_monastic_robe",
			"robe",
			"a black hemp monastic robe",
			null,
			"This black hemp monastic robe is a straight wrapped garment with close sleeves and a plain collar line. The hemp has a dry, matte surface, darkened into a sober black without decorative contrast. Its cut is practical and restrained, keeping the focus on length, folds, and simplicity.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			540.0,
			18.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_daoist_cross_collar_robe",
			"robe",
			"a dark cotton Daoist robe",
			null,
			"This dark cotton Daoist robe is cut with a cross-collar front, long sleeves, and a straight body. The cotton is plain and smooth, allowing the overlapping collar and tied closure to define the garment. It looks formal but not lavish, suited to a religious specialist rather than court display.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			24.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_daoist_cloud_sleeved_robe",
			"robe",
			"a fine $colour cloud-sleeved robe",
			null,
			"This fine $colour cloud-sleeved robe is a silk ritual garment with broad sleeves and a loose cross-collar body. The sleeves spread widely from the arm, making the $colour silk fall in soft ceremonial folds. Its shape is more expansive than an ordinary robe, giving the garment a deliberate temple-formal presence.",
			SizeCategory.Normal,
			ItemQuality.Good,
			680.0,
			130.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_daoist_ritual_cap",
			"cap",
			"a black cloth ritual cap",
			null,
			"This black cloth ritual cap is a compact headpiece made from stiffened cotton panels. The shape is neat and angular, with a dark surface that sits close to the head. It is modest in scale but clearly formal, adding a precise finish to a robe without bright decoration.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			12.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_shinto_white_joe_robe",
			"jōe",
			"a white hemp jōe robe",
			null,
			"This white hemp jōe robe is a pale ceremonial garment with a straight body and broad, courtly sleeves. The hemp is clean and carefully finished, giving the robe a crisp surface despite its simple colour. Its shape recalls formal court dress, but the lack of pattern keeps it restrained and shrine-focused.",
			SizeCategory.Normal,
			ItemQuality.Good,
			520.0,
			42.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_shinto_priest_hakama",
			"hakama",
			"a pair of white shrine hakama",
			null,
			"These white shrine hakama are broad pleated trousers with a full, skirt-like fall. The hemp cloth is pale and crisp, holding the pleats in clean vertical lines. Their breadth and colour make them visibly ceremonial rather than ordinary work hakama.",
			SizeCategory.Normal,
			ItemQuality.Good,
			480.0,
			34.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_shinto_priest_eboshi",
			"eboshi",
			"a black lacquered eboshi",
			null,
			"This black lacquered eboshi is a tall, folded headpiece with a crisp upright silhouette. The silk body is stiffened and darkened to a smooth black surface, keeping its shape above the brow. It looks formal and courtly, pairing with shrine robes without needing additional ornament.",
			SizeCategory.Small,
			ItemQuality.Good,
			110.0,
			38.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_shinto_miko_white_kosode",
			"kosode",
			"a white hemp shrine kosode",
			null,
			"This white hemp shrine kosode is a short-sleeved robe with a straight wrapped front and narrow body. The hemp is pale, clean, and simply finished, showing little beyond the collar line and sleeve openings. It forms a modest upper layer for shrine service rather than a fashionable court garment.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			24.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_shinto_miko_red_hakama",
			"hakama",
			"a pair of red shrine hakama",
			null,
			"These red shrine hakama are broad pleated trousers made to fall in long, straight folds. The hemp cloth is dyed a clear red, making the lower garment stand out sharply beneath a white kosode. Their pleats and fullness give them a formal shrine-service outline while remaining practical to move in.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			460.0,
			28.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_steppe_ritual_felt_coat",
			"coat",
			"a felt-lined ritual coat",
			null,
			"This felt-lined ritual coat is a long front-opening garment with loose sleeves and a practical steppe cut. Dense felt gives it body and warmth, while the outer surface remains plain enough to resemble travelling clothing. Its ritual character comes from its deliberate use as a special outer layer rather than from metal fittings or carried objects.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			34.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_steppe_ritual_fur_cap",
			"cap",
			"a fur-trimmed ritual cap",
			null,
			"This fur-trimmed ritual cap is a warm headpiece with a soft crown and a thick band around the brow. The fur gives the cap a rough, animal-textured edge, contrasting with the simpler cloth or felt of the crown. It is visually suited to cold open country and to a specialist set apart by distinctive winter headwear.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			24.0m,
			true,
			false,
			"fur",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_steppe_ritual_sash",
			"sash",
			"a braided wool ritual sash",
			null,
			"This braided wool ritual sash is a sturdy belt-like length of woven cloth with a visibly twisted structure. Its surface is rough and practical, with ends that hang plainly once tied. It adds a marked band across a robe or coat without claiming pockets, tools, or other unsupported functions.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_annular_bronze_brooch",
			"brooch",
			"a bronze annular brooch",
			null,
			"This bronze annular brooch is shaped as a plain circular ring with a slim pin crossing the opening. The surface is modestly polished, with most of its detail in the clean curve and the slight flattening around the pin hinge. It is sized to sit visibly at a shoulder or chest over a cloak, mantle, or heavy outer layer.",
			SizeCategory.Small,
			ItemQuality.Standard,
			45.0,
			18.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_penannular_bronze_brooch",
			"brooch",
			"a bronze penannular brooch",
			null,
			"This bronze penannular brooch forms an open ring with two slightly thickened terminals and a long movable pin. Fine filing marks show around the ends, while the broader front catches light when worn over wool. Its shape gives a clear cloak-fastening appearance without relying on any hidden mechanism.",
			SizeCategory.Small,
			ItemQuality.Good,
			55.0,
			32.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_pair_oval_brooches",
			"brooches",
			"a pair of bronze oval brooches",
			null,
			"These bronze oval brooches are made as a matched pair, each with a domed face and a narrow back pin. The surfaces carry shallow raised lines that break up the polished metal without making them overly ornate. They are visually suited to being worn symmetrically over a strap dress or front-hanging garment layer.",
			SizeCategory.Small,
			ItemQuality.Good,
			110.0,
			70.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Wear_Shoulders",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_plain_bronze_cloak_pin",
			"pin",
			"a plain bronze cloak pin",
			null,
			"This plain bronze cloak pin is a straight, practical fastening piece with a narrow shaft and slightly broader head. Its metal has a muted brown-gold sheen, with no decoration beyond the worked tip and smoothed edges. It reads as a humble outerwear fitting rather than a jewel or badge.",
			SizeCategory.Small,
			ItemQuality.Standard,
			35.0,
			12.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_iron_cloak_pin",
			"pin",
			"an iron cloak pin",
			null,
			"This iron cloak pin is dark, narrow, and plainly forged, with a simple head and a tapering point. The metal is sturdy rather than elegant, showing faint hammering and a workmanlike finish. It suits rough wool, patched cloaks, and road clothing where visible utility matters more than display.",
			SizeCategory.Small,
			ItemQuality.Standard,
			45.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bronze_mounted_leather_belt",
			"belt",
			"a leather belt with bronze mounts",
			null,
			"This leather belt is fitted with small bronze mounts set at intervals along the strap. The leather is broad enough to show its grain, while the metal pieces give it a stronger and more prosperous look. It is decorative waistwear and does not imply pockets, hooks, or storage beyond its visible belt form.",
			SizeCategory.Small,
			ItemQuality.Good,
			300.0,
			46.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_silver_mounted_leather_belt",
			"belt",
			"a fine silver-mounted belt",
			null,
			"This fine leather belt is set with small silver mounts and a neatly finished buckle. The strap is smooth and carefully edged, with the pale metal standing out against the darker leather. It presents wealth and care in dress without suggesting any hidden attachments or carried goods.",
			SizeCategory.Small,
			ItemQuality.Good,
			280.0,
			150.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_tablet_woven_garters",
			"garters",
			"a pair of $colour tablet-woven garters",
			null,
			"These $colour tablet-woven garters are narrow wool bands with a firm, patterned structure. Each strip is long enough to wrap neatly around the leg, with cleanly finished ends that can lie flat against hose or chausses. The $colour tone shows best across the raised weave and makes them more refined than plain work ties.",
			SizeCategory.Small,
			ItemQuality.Good,
			60.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Leggings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_wool_leg_ties",
			"leg ties",
			"a pair of $colour wool leg ties",
			null,
			"These $colour wool leg ties are simple narrow lengths of woven cloth, made for binding loose lower-leg layers into a tidy line. The weave is plain and slightly springy, with ends that show minor fraying from use. Their $colour appearance is practical and understated, suited to common travel or labour clothing.",
			SizeCategory.Small,
			ItemQuality.Standard,
			50.0,
			5.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Leggings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_headband",
			"headband",
			"a $colour linen headband",
			null,
			"This $colour linen headband is a narrow strip of folded cloth with lightly hemmed edges. It is simple enough for work wear, keeping its shape through neat creases rather than stiffness. The $colour dye is plain and even across the band.",
			SizeCategory.Small,
			ItemQuality.Standard,
			35.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Headband",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_silk_hair_ribbon",
			"ribbon",
			"a fine $colour silk hair ribbon",
			null,
			"This fine $colour silk hair ribbon is a narrow, smooth strip with a soft sheen and delicate folded edges. It is light enough to trail from tied hair or a small head arrangement without adding bulk. The $colour surface gives it a refined decorative quality suited to elite or formal dress.",
			SizeCategory.Small,
			ItemQuality.Good,
			20.0,
			30.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Headband",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_silk_robe_sash",
			"sash",
			"a fine $colour silk robe sash",
			null,
			"This fine $colour silk robe sash is long, smooth, and lightly weighted so it can lie in clean folds at the waist. The edges are carefully turned, and the fabric has enough sheen to mark it as a dressier layer. Its $colour surface is meant for visible contrast over robes, caftans, or formal gowns.",
			SizeCategory.Small,
			ItemQuality.Good,
			130.0,
			78.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_woven_kosode_sash",
			"sash",
			"a $colour woven kosode sash",
			null,
			"This $colour woven kosode sash is a narrow, straight length of fabric with a firm hand and flat selvages. It is simpler and less formal than later elaborate waist sashes, with its effect coming from tidy wrapping rather than ornament. The $colour dye gives a clear band of contrast against a robe.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			34.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_hemp_waist_cord",
			"cord",
			"a plaited hemp waist cord",
			null,
			"This plaited hemp waist cord is made from coarse fibres twisted into a narrow, durable line. The surface is dry and slightly rough, with simple knotted ends and no metal fittings. It is a humble waist binding and carries no implication of attachments, storage, or rank.",
			SizeCategory.Small,
			ItemQuality.Standard,
			70.0,
			3.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cotton_waist_cord",
			"cord",
			"a white cotton waist cord",
			null,
			"This white cotton waist cord is a soft, narrow length of twisted cotton with plain finished ends. It is lighter and cleaner-looking than hemp cord, suited to warm-climate linen or cotton layers. The cord reads as a modest binding rather than a display belt.",
			SizeCategory.Small,
			ItemQuality.Standard,
			55.0,
			4.0m,
			true,
			false,
			"cotton",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_glass_bead_necklace",
			"necklace",
			"a glass bead necklace",
			null,
			"This necklace is strung with small glass beads of mixed muted tones, each bead catching light along its rounded edge. The strand is simple and close enough to sit at the base of the throat. It gives colour and trade-good texture without becoming a formal jewel.",
			SizeCategory.Small,
			ItemQuality.Standard,
			85.0,
			20.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_amber_bead_necklace",
			"necklace",
			"an amber bead necklace",
			null,
			"This amber bead necklace is made from warm translucent beads, each polished into a rounded or slightly irregular shape. The colour varies from honey to darker resinous brown, giving the strand a natural depth. It is a conspicuous but still wearable sign of northern trade and household wealth.",
			SizeCategory.Small,
			ItemQuality.Good,
			70.0,
			82.0m,
			true,
			false,
			"amber",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_shell_bead_necklace",
			"necklace",
			"a shell bead necklace",
			null,
			"This shell bead necklace is strung from pale, smoothed shell pieces with a faint natural lustre. The beads are irregular enough to show their organic origin, but they have been worked into a comfortable line. It suits coastal and trade-linked dress without suggesting any ritual or magical purpose.",
			SizeCategory.Small,
			ItemQuality.Standard,
			65.0,
			18.0m,
			true,
			false,
			"shell",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bronze_armlet",
			"armlet",
			"a bronze armlet",
			null,
			"This bronze armlet is a curved band of metal with slightly rounded edges and a plain polished face. It is sturdy enough to hold its shape, with no sharp projections or hanging elements. The warm bronze colour makes it visible against sleeves or bare skin.",
			SizeCategory.Small,
			ItemQuality.Standard,
			95.0,
			24.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bronze_bracelets",
			"bracelets",
			"a pair of bronze bracelets",
			null,
			"These bronze bracelets are a matched pair of simple rounded bands with smoothed inner faces. The metal is polished but not precious, giving them a warm muted shine. They are decorative wristwear suited to everyday prosperity rather than formal court display.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			22.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bronze_bangles",
			"bangles",
			"a pair of bronze bangles",
			null,
			"These bronze bangles are circular, smooth, and slightly heavier than narrow bracelets. Their plain rounded surfaces make the metal itself the main ornament. They are suited to layered jewellery looks without relying on gems or written markings.",
			SizeCategory.Small,
			ItemQuality.Standard,
			110.0,
			24.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bronze_anklets",
			"anklets",
			"a pair of bronze anklets",
			null,
			"These bronze anklets are made as a pair of smooth metal rings sized for the lower leg. The bronze is plainly finished, with enough weight to look substantial without being ostentatious. They are decorative worn ornaments and do not include bells, charms, or other carried fittings.",
			SizeCategory.Small,
			ItemQuality.Standard,
			115.0,
			26.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Anklets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_silver_earrings",
			"earrings",
			"a pair of silver earrings",
			null,
			"These silver earrings are a matched pair of small curved ornaments with bright polished surfaces. Their form is restrained, relying on the clean metal and neat symmetry rather than large stones. They add a clear note of wealth while remaining general enough for many local styles.",
			SizeCategory.Small,
			ItemQuality.Good,
			18.0,
			90.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_simple_bronze_ring",
			"ring",
			"a simple bronze finger ring",
			null,
			"This simple bronze finger ring is a narrow band with a smoothed inner face and a plain rounded outside. The metal is modest and workmanlike, showing small variations from hand finishing. It is visible personal adornment without any seal, inscription, or hidden function.",
			SizeCategory.Small,
			ItemQuality.Standard,
			10.0,
			10.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_silver_ring",
			"ring",
			"a fine silver finger ring",
			null,
			"This fine silver finger ring is a cleanly made band with a bright surface and a slightly raised outer curve. Its edges are even, and the polish makes it stand out against the hand. The ring is decorative rather than a signet or mechanically meaningful seal.",
			SizeCategory.Small,
			ItemQuality.Good,
			12.0,
			80.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bone_hairpin",
			"hairpin",
			"a bone hairpin",
			null,
			"This bone hairpin is slender, pale, and smoothly polished, with a simple rounded head. The shaft is narrow enough to disappear partly into hair, while the worked top remains visible. It is a practical ornament and fastening piece without carving or precious decoration.",
			SizeCategory.Small,
			ItemQuality.Standard,
			12.0,
			5.0m,
			true,
			false,
			"bone",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Headband",
				"Destroyable_Misc",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_bronze_hairpin",
			"hairpin",
			"a bronze hairpin",
			null,
			"This bronze hairpin has a slim shaft and a small shaped head with a muted metallic sheen. The surface is smooth, with only minor filing marks near the tip. It gives a modest decorative accent to dressed hair without becoming elaborate jewellery.",
			SizeCategory.Small,
			ItemQuality.Standard,
			18.0,
			16.0m,
			true,
			false,
			"bronze",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Headband",
				"Destroyable_HeavyMetal",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_silver_hairpins",
			"hairpins",
			"a pair of silver hairpins",
			null,
			"These silver hairpins are a matched pair of slender pins with bright polished heads. Their long shafts are plain and neat, leaving the silver tips as the visible ornament. They are refined hair adornments suited to formal dress, not hidden tools or weapons.",
			SizeCategory.Small,
			ItemQuality.Good,
			28.0,
			110.0m,
			true,
			false,
			"silver",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Headband",
				"Destroyable_HeavyMetal",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_tablet_woven_belt",
			"belt",
			"a $colour tablet-woven belt",
			null,
			"This $colour tablet-woven belt is a firm narrow band with a tight patterned structure and neat edges. The wool is compact rather than bulky, giving the belt enough body to lie flat around the waist. The $colour dye and visible weave make it a small but noticeable textile display.",
			SizeCategory.Small,
			ItemQuality.Good,
			95.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_decorated_leather_girdle",
			"girdle",
			"a decorated leather girdle",
			null,
			"This decorated leather girdle is a narrow waist strap with careful edging and small tooled details along its length. The leather is supple and smooth, intended to hang cleanly over a gown or kirtle. It is ornamental waistwear and does not promise storage, attachments, or mechanical support.",
			SizeCategory.Small,
			ItemQuality.Good,
			170.0,
			48.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_travel_wool_cloak",
			"cloak",
			"a weather-stained $colour wool travel cloak",
			null,
			"This weather-stained $colour wool travel cloak is cut full enough to wrap over the shoulders and upper body. The wool shows worn folds, dulled edges, and a practical weight suited to road use. Its $colour surface looks faded and serviceable rather than courtly or ceremonial.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1150.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_DrabColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_broad_brim_felt_hat",
			"hat",
			"a broad-brim $colour felt travel hat",
			null,
			"This broad-brim $colour felt travel hat has a low crown and a wide soft brim. The felt is dense but flexible, with slight waviness where it has been handled and packed. The $colour shade is plain enough for road wear while still reading as deliberate headgear.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			12.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_travel_wool_gaiters",
			"gaiters",
			"a pair of $colour wool travel gaiters",
			null,
			"These $colour wool travel gaiters are shaped as lower-leg coverings with simple ties along the side. The wool is thicker than ordinary leg ties, with reinforced edges where the cloth meets footwear. Their $colour surface gives a practical layer for road dust and rough ground.",
			SizeCategory.Small,
			ItemQuality.Standard,
			210.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Leggings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_oiled_linen_rain_cape",
			"cape",
			"an oiled linen rain cape",
			null,
			"This linen rain cape has a slightly darkened, oiled-looking surface and a simple shoulder-draped cut. The cloth is lighter than wool but stiffer from its finish, falling in broad practical folds. It reads as weather-facing outerwear without claiming special waterproof mechanics.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			24.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cape",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_shepherds_hooded_cloak",
			"cloak",
			"a rough $colour shepherd's cloak",
			null,
			"This rough $colour shepherd's cloak is heavy, hooded, and cut to hang well below the waist. The wool is coarse, with a thick nap and broad folds that make it look suited to open fields. The $colour dye is plain and uneven enough to feel rural without becoming deliberately ragged.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1320.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rough_felt_herder_hat",
			"hat",
			"a rough felt herder's hat",
			null,
			"This rough felt herder's hat has a simple crown and a worn, uneven brim. The felt is dense and plain, with a slightly lumpy surface from hand shaping. It is practical rural headwear rather than a court cap or formal town hat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			8.0m,
			true,
			false,
			"felt",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fisher_headcloth",
			"headcloth",
			"a $colour fisher's headcloth",
			null,
			"This $colour fisher's headcloth is a square of linen folded to sit close around the hair and brow. The edges are simply hemmed, and the cloth has the light creasing of a practical work covering. Its $colour surface is plain and washable, suited to damp labour rather than display.",
			SizeCategory.Small,
			ItemQuality.Standard,
			75.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_boatmans_tunic",
			"tunic",
			"a short $colour boatman's tunic",
			null,
			"This short $colour boatman's tunic is cut above the knee, with modest side openings and sleeves kept clear of the wrist. The wool is plain and sturdy, made for movement rather than warmth alone. The $colour dye sits evenly across a garment meant for wet work and cramped footing.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			470.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rope_soled_hemp_sandals",
			"sandals",
			"a pair of rope-soled hemp sandals",
			null,
			"These hemp sandals have rope-like soles and simple woven straps across the foot. The fibre is rough, pale, and plainly twisted, giving the sandals a light but workmanlike appearance. They are humble footwear for warm or wet work rather than fine town shoes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			5.0m,
			true,
			false,
			"hemp",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Sandals",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_leather_hawking_glove",
			"glove",
			"a single leather hawking glove",
			null,
			"This single leather hawking glove is long through the wrist, with a heavier palm and a smooth outer back. The leather is supple but thick, showing careful stitching around the fingers and cuff. It is visibly specialised outdoor clothing, not a weapon or animal-handling tool by itself.",
			SizeCategory.Small,
			ItemQuality.Good,
			170.0,
			36.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Gloves",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_rough_wool_gaiters",
			"gaiters",
			"a pair of rough $colour wool gaiters",
			null,
			"These rough $colour wool gaiters are plain lower-leg coverings with thick hems and visible tie points. The fabric looks coarse and hard-worn, with a few uneven patches in the weave. Their $colour shade is deliberately dull, fitting mud, yard work, and long walking.",
			SizeCategory.Small,
			ItemQuality.Standard,
			230.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
			[
				"Holdable",
				"Wear_Leggings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_DrabColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_patched_wool_cloak",
			"cloak",
			"a patched $colour wool cloak",
			null,
			"This patched $colour wool cloak is made from worn fabric reinforced with several visible repairs. The edges are uneven but functional, and the patchwork breaks up the original line of the cloth. Its $colour surface is faded and humble, marking it as a hard-used outer layer.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1040.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_DrabColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_stablehand_wool_cap",
			"cap",
			"a $colour stablehand's wool cap",
			null,
			"This $colour stablehand's wool cap is close-fitting, soft, and simply seamed. The wool has a slightly fuzzy surface, giving it warmth without looking fine or expensive. Its $colour tone is ordinary and practical for yard work or cart service.",
			SizeCategory.Small,
			ItemQuality.Standard,
			95.0,
			5.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_cart_driver_wool_coat",
			"coat",
			"a belted $colour cart driver's coat",
			null,
			"This belted $colour cart driver's coat is a front-opening wool layer cut long enough to cover the hips. The sleeves are practical and the skirt has enough room for walking, riding, or climbing onto a cart. The $colour wool is plain, with the belt line giving it a tidy working shape.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			820.0,
			26.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_plain_service_gown",
			"gown",
			"a plain $colour service gown",
			null,
			"This plain $colour service gown is cut from straight wool panels with a modest neck and simple sleeves. The hem is practical rather than trailing, and the seams are made for wear rather than display. Its $colour cloth is clean enough for household service but not fine enough for court dress.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			700.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_tucked_work_kirtle",
			"kirtle",
			"a tucked $colour work kirtle",
			null,
			"This tucked $colour work kirtle is a practical wool garment with its skirt arranged to sit higher and freer than a courtly gown. The sleeves are narrow enough for labour, and the waist can be controlled by a sash or cord. The $colour fabric remains plain, emphasizing use and movement.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_large_linen_work_apron",
			"apron",
			"a large $colour linen work apron",
			null,
			"This large $colour linen work apron is made from a broad rectangular panel with simple ties. The linen is sturdy and washable-looking, falling over the front of the body in clean practical folds. Its $colour surface is plain enough for kitchens, laundry, and other household labour.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_linen_laundry_headcloth",
			"headcloth",
			"a $colour laundry headcloth",
			null,
			"This $colour laundry headcloth is a folded linen covering meant to sit securely around the hair. The cloth is light but closely woven, with a simple hem and no decorative border. Its $colour dye is modest and suited to damp, practical work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			80.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Kerchief",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_short_service_tunic",
			"tunic",
			"a short $colour service tunic",
			null,
			"This short $colour service tunic is cut for easy movement, with a plain round neck and sturdy sleeves. The hem falls high enough to keep clear of steps, yards, and kitchen work. Its $colour wool is tidy but unadorned, marking it as working dress rather than field rags.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			470.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_household_work_sash",
			"sash",
			"a $colour household work sash",
			null,
			"This $colour household work sash is a narrow wool band with plain edges and enough length to tie securely. It gives shape to a loose tunic, gown, or robe without adding ornament. The $colour cloth is practical and subdued, suitable for ordinary service work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			110.0,
			5.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
			[
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_kitchen_sleeve_ties",
			"sleeve ties",
			"a pair of $colour kitchen sleeve ties",
			null,
			"These $colour kitchen sleeve ties are narrow strips of linen made to bind or gather fabric near the forearms. The edges are simply finished, and the cloth has the softened look of repeated laundering. Their $colour shade is plain, emphasizing cleanliness and practical control of sleeves.",
			SizeCategory.Small,
			ItemQuality.Standard,
			45.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
			[
				"Holdable",
				"Wear_Bracers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_scullion_linen_coif",
			"coif",
			"a $colour scullion's linen coif",
			null,
			"This $colour scullion's linen coif is close-fitting, with ties under the chin and a plain rounded crown. The linen is light and washable-looking, with no embroidery or fine trim. Its $colour surface makes it serviceable headwear for kitchens and busy households.",
			SizeCategory.Small,
			ItemQuality.Standard,
			85.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Coif",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_leather_stable_apron",
			"apron",
			"a leather stable apron",
			null,
			"This leather stable apron is a plain protective front layer with a broad body and simple waist ties. The leather is thick enough to look durable, with scuffs and creases across the lower panel. It is worn clothing for dirty yard work, not a tool belt or storage apron.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_inn_workers_wool_hood",
			"hood",
			"a $colour inn worker's wool hood",
			null,
			"This $colour inn worker's wool hood has a short shoulder cape and a close face opening. The wool is plain but tidy, giving a warmer service layer than a cap or coif. Its $colour cloth suits public-facing work without suggesting formal livery.",
			SizeCategory.Small,
			ItemQuality.Standard,
			230.0,
			10.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_student_wool_gown",
			"gown",
			"a plain $colour student gown",
			null,
			"This plain $colour student gown is a long wool garment with straight sleeves and an unadorned fall. The cut is sober and practical, covering most ordinary clothing beneath it. Its $colour fabric is respectable but restrained, fitting study or clerical town life rather than court display.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_master_scholar_gown",
			"gown",
			"a sober $colour scholar's gown",
			null,
			"This sober $colour scholar's gown is long, full, and carefully made from good wool. The sleeves fall cleanly, and the body has enough breadth to drape with learned dignity rather than workroom tightness. The $colour shade is rich but controlled, suggesting standing without theatrical ornament.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			60.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_physician_wool_gown",
			"gown",
			"a long $colour physician's gown",
			null,
			"This long $colour physician's gown is a dignified wool garment with full sleeves and a straight, weighty fall. The seams are neat and the cloth is better than ordinary work wear, but it avoids embroidery or official insignia. Its $colour tone gives it a sober professional presence.",
			SizeCategory.Normal,
			ItemQuality.Good,
			840.0,
			70.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_notary_wool_gown",
			"gown",
			"a neat $colour notary's gown",
			null,
			"This neat $colour notary's gown is cut cleanly, with modest sleeves and a tidy hem. The wool is good enough to show status but not so rich as to look noble or ceremonial. The $colour surface gives a restrained professional look suitable for writing desks, courts, and town offices.",
			SizeCategory.Normal,
			ItemQuality.Good,
			760.0,
			54.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_lined_scholars_hood",
			"hood",
			"a lined $colour scholar's hood",
			null,
			"This lined $colour scholar's hood has a close face opening and a short shoulder fall. The wool exterior is smooth, while the lining adds enough body to make the folds sit neatly. The $colour fabric gives it a more formal look than an ordinary work hood.",
			SizeCategory.Small,
			ItemQuality.Good,
			300.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_plain_black_skullcap",
			"skullcap",
			"a plain black skullcap",
			null,
			"This plain black skullcap is small, close-fitting, and made from dark wool. Its shape is simple and rounded, with no embroidery, badge, or written mark. It gives a sober head-covering option for learned or clerical dress without making a culture-specific claim.",
			SizeCategory.Small,
			ItemQuality.Standard,
			45.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Skullcap",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_sober_lecture_mantle",
			"mantle",
			"a sober $colour lecture mantle",
			null,
			"This sober $colour lecture mantle is a broad wool outer layer with a controlled drape and plain finished border. It is less martial and less courtly than a display cloak, relying on clean cloth and measured folds. The $colour shade is dignified without being bright or festive.",
			SizeCategory.Normal,
			ItemQuality.Good,
			720.0,
			52.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_fine_scholars_cap",
			"cap",
			"a fine $colour scholar's cap",
			null,
			"This fine $colour scholar's cap is a small shaped wool cap with a neat crown and clean lower edge. The cloth is smooth and carefully finished, giving it more presence than a work cap. The $colour tone makes it suitable for learned public dress without implying a specific degree or office.",
			SizeCategory.Small,
			ItemQuality.Good,
			80.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_child_wool_tunic",
			"tunic",
			"a small $colour child's wool tunic",
			null,
			"This small $colour child's wool tunic is cut simply, with straight sleeves and a short practical hem. The wool is plain and sturdy, built like adult common clothing scaled down in bulk. Its $colour fabric is ordinary and suited to daily household or street wear.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_child_linen_shift",
			"shift",
			"a small $colour child's linen shift",
			null,
			"This small $colour child's linen shift is a light undergarment with a simple neck and loose sleeves. The linen is soft and plain, with seams kept unobtrusive for wear beneath heavier layers. The $colour tone is modest and even across the small panels.",
			SizeCategory.Small,
			ItemQuality.Standard,
			150.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_child_leather_shoes",
			"shoes",
			"a pair of small leather child's shoes",
			null,
			"These small leather child's shoes are soft, low, and simply stitched. The soles are thin compared with adult work shoes, and the uppers show modest shaping around the foot. They are ordinary child-sized footwear without ornament or hidden fittings.",
			SizeCategory.Small,
			ItemQuality.Standard,
			230.0,
			10.0m,
			true,
			false,
			"leather",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
			[
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_child_wool_cloak",
			"cloak",
			"a small $colour child's wool cloak",
			null,
			"This small $colour child's wool cloak is a short outer layer with enough fullness to wrap around narrow shoulders. The wool is plain and warm-looking, with a simple finished edge and no heavy trim. Its $colour dye matches common clothing more than formal display.",
			SizeCategory.Small,
			ItemQuality.Standard,
			480.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_apprentice_work_tunic",
			"tunic",
			"a short $colour apprentice's tunic",
			null,
			"This short $colour apprentice's tunic is cut for movement, with plain sleeves and a modest hem. The wool is sturdy but not fine, suited to errands, workshops, and household labour. The $colour cloth gives it a tidy working look without any badge or formal guild mark.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			440.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_apprentice_linen_cap",
			"cap",
			"a $colour apprentice's linen cap",
			null,
			"This $colour apprentice's linen cap is light, close-fitting, and simply seamed. The linen is plain and easy to launder, with no ornament or rank sign. Its $colour surface suits a young worker or trainee more than a prosperous master.",
			SizeCategory.Small,
			ItemQuality.Standard,
			55.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
			[
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_child_silk_court_robe",
			"robe",
			"a small fine $colour silk court robe",
			null,
			"This small fine $colour silk court robe is cut from smooth fabric with a careful straight fall and neat sleeve openings. The garment is sized for a child but made with the same attention as elite adult dress. The $colour silk gives it a bright, formal presence without needing jewellery or insignia.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			110.0m,
			true,
			false,
			"silk",
			[
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_apprentice_canvas_apron",
			"apron",
			"a $colour apprentice's canvas apron",
			null,
			"This $colour apprentice's canvas apron is a plain front layer with simple ties and a hard-wearing weave. The canvas is thicker than linen and sits flat over a tunic or kirtle. Its $colour surface is practical and unmarked, avoiding any claim of a formal guild uniform.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			6.0m,
			true,
			false,
			"canvas",
			[
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
			[
				"Holdable",
				"Wear_Apron",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
			null,
			null,
			null,
			null
		);
	}
}
