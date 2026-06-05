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
	}
}
