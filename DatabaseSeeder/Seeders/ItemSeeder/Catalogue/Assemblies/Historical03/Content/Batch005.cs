#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical03Stock
{
	public static CatalogueItemDefinition MedievalMilitaryBundleOfLinenBowstrings() => new()
	{
		Metadata = new("medieval_military_bundle_of_linen_bowstrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalArmour.cs:4731", []),
		Noun = "bowstring",
		ShortDescription = "a bundle of linen bowstrings",
		LongDescription = null,
		FullDescription = "These linen bowstrings are neatly twisted, lightly waxed, and tied in a compact bundle. The loops are more even than common hemp strings, and the cordage has a smooth pale finish. They are small but useful support gear for an archer who expects wear and breakage.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 40.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Weapons" ],
		Components = [ "Holdable", "Stack_Number", "Destroyable_Misc" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitarySinewBowstringBundle() => new()
	{
		Metadata = new("medieval_military_sinew_bowstring_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalArmour.cs:4751", []),
		Noun = "bowstring",
		ShortDescription = "a bundle of sinew bowstrings",
		LongDescription = null,
		FullDescription = "These bowstrings are made from twisted sinew and bound into a careful bundle. They have a slightly glossy, fibrous surface and strong end loops suited to compact bows. The work is more demanding than simple plant cordage, giving them the look of valued spare gear.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 42.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sinew",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Weapons" ],
		Components = [ "Holdable", "Stack_Number", "Destroyable_Misc" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMilitarySpareCrossbowString() => new()
	{
		Metadata = new("medieval_military_spare_crossbow_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalArmour.cs:4771", []),
		Noun = "string",
		ShortDescription = "a spare crossbow string",
		LongDescription = null,
		FullDescription = "This heavy spare string is made from tightly twisted hemp cord with thick served loops at both ends. It is shorter and stouter than an ordinary bowstring, meant for the harsher strain of a crossbow. The cord is kept in a tight coil so it can be carried with other missile gear.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 85.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Weapons" ],
		Components = [ "Holdable", "Destroyable_Misc" ],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenBraies() => new()
	{
		Metadata = new("medieval_linen_braies", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:15", []),
		Noun = "braies",
		ShortDescription = "a $colour pair of linen braies",
		LongDescription = null,
		FullDescription = "These $colour linen braies are cut as loose under-breeches with a gathered waist, roomy seat, and short legs. The linen is light enough to sit beneath hose or trousers, with practical seams kept flat against the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shorts",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenUndertunic() => new()
	{
		Metadata = new("medieval_linen_undertunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:46", []),
		Noun = "undertunic",
		ShortDescription = "a $colour linen undertunic",
		LongDescription = null,
		FullDescription = "This $colour linen undertunic is made from straight, plain linen panels, with narrow sleeves and small side openings for movement. The neck is simply finished and the hem is light enough to tuck beneath heavier layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenChemise() => new()
	{
		Metadata = new("medieval_linen_chemise", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:77", []),
		Noun = "chemise",
		ShortDescription = "a $colour linen chemise",
		LongDescription = null,
		FullDescription = "This $colour linen chemise is made from long, plain linen panels, with narrow sleeves and small side openings for movement. The neck is simply finished and the hem is light enough to tuck beneath heavier layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenBreastband() => new()
	{
		Metadata = new("medieval_linen_breastband", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:108", []),
		Noun = "breastband",
		ShortDescription = "a $colour linen breastband",
		LongDescription = null,
		FullDescription = "This $colour linen breastband is a long folded strip of cloth meant to wrap firmly around the torso. The ends are narrow enough to tie without bulky knots, while the middle is broad and smooth against the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Bra",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPlainLeatherBelt() => new()
	{
		Metadata = new("medieval_plain_leather_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:139", []),
		Noun = "belt",
		ShortDescription = "a plain leather belt",
		LongDescription = null,
		FullDescription = "This plain leather belt is a long strap of sturdy leather with a simple fastening and burnished edges. The surface is left unornamented, relying on the grain of the hide and the evenness of the cut for its appearance. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIronBuckledLeatherBelt() => new()
	{
		Metadata = new("medieval_iron_buckled_leather_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:169", []),
		Noun = "belt",
		ShortDescription = "an iron-buckled leather belt",
		LongDescription = null,
		FullDescription = "This iron-buckled leather belt is a sturdy strap fitted with a dark iron buckle and a row of cleanly punched holes. The leather is thick enough to hold its shape around the waist, with burnished edges and visible stitching near the buckle fold. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Waist",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSimpleWovenSash() => new()
	{
		Metadata = new("medieval_simple_woven_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:199", []),
		Noun = "sash",
		ShortDescription = "a $colour woven sash",
		LongDescription = null,
		FullDescription = "This $colour woven sash is a long wool band with firm selvages and simple squared ends. It is made to wrap, knot, or girdle clothing without adding a hard buckle or metal fitting. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineWovenGirdle() => new()
	{
		Metadata = new("medieval_fine_woven_girdle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:230", []),
		Noun = "girdle",
		ShortDescription = "a fine $colour woven girdle",
		LongDescription = null,
		FullDescription = "This fine $colour woven girdle is a narrow wool band with firm edges and a smooth, even hand. It is long enough to wrap the waist and leave modest hanging ends, while the weave keeps it from twisting. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 100.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineLeatherShoes() => new()
	{
		Metadata = new("medieval_fine_leather_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:261", []),
		Noun = "shoes",
		ShortDescription = "a pair of fine leather shoes",
		LongDescription = null,
		FullDescription = "These fine leather shoes are low, carefully shaped footwear with smooth uppers and neatly turned edges. The soles are firm but not heavy, and the stitching is kept regular around the heel and vamp. Natural variations in the leather show softly across the polished surface and flexed creases.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 380.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalAnkleLeatherBoots() => new()
	{
		Metadata = new("medieval_ankle_leather_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:291", []),
		Noun = "boots",
		ShortDescription = "a pair of ankle leather boots",
		LongDescription = null,
		FullDescription = "These ankle leather boots are sturdy low boots with firm soles, rounded toes, and shafts that cover the ankle. The uppers are stitched from practical leather panels, with the seams placed where they will not rub too sharply. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Boots",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHighLeatherBoots() => new()
	{
		Metadata = new("medieval_high_leather_boots", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:321", []),
		Noun = "boots",
		ShortDescription = "a pair of high leather boots",
		LongDescription = null,
		FullDescription = "These high leather boots rise well above the ankle, with firm shafts and soles meant for riding or rough ground. The leather is shaped to the foot and lower leg, with visible stitching around the sole and up the back. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_High_Boots",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPlainLeatherSandals() => new()
	{
		Metadata = new("medieval_plain_leather_sandals", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:351", []),
		Noun = "sandals",
		ShortDescription = "a pair of plain leather sandals",
		LongDescription = null,
		FullDescription = "These plain leather sandals are open footwear with flat soles and simple straps across the foot. The cut is practical and spare, leaving most of the foot exposed while protecting the underside. Natural variations in the leather show along the grain, flex marks, and cut edge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Sandals",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolHose() => new()
	{
		Metadata = new("medieval_wool_hose", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:381", []),
		Noun = "hose",
		ShortDescription = "a pair of $colour wool hose",
		LongDescription = null,
		FullDescription = "These $colour wool hose are close-fitting wool leg coverings with shaped lower portions and neatly finished upper edges. The cloth is cut to follow the leg without the bulk of full trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Stockings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolChausses() => new()
	{
		Metadata = new("medieval_wool_chausses", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:412", []),
		Noun = "chausses",
		ShortDescription = "a pair of $colour wool chausses",
		LongDescription = null,
		FullDescription = "These $colour wool chausses are close-fitting wool leg coverings with shaped lower portions and neatly finished upper edges. The cloth is cut to follow the leg without the bulk of full trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Chausses",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNarrowWoolTrousers() => new()
	{
		Metadata = new("medieval_narrow_wool_trousers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:443", []),
		Noun = "trousers",
		ShortDescription = "a pair of narrow $colour wool trousers",
		LongDescription = null,
		FullDescription = "These narrow $colour wool trousers are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolTunic() => new()
	{
		Metadata = new("medieval_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:474", []),
		Noun = "tunic",
		ShortDescription = "a $colour wool tunic",
		LongDescription = null,
		FullDescription = "This $colour wool tunic is a straight wool garment made from straight body panels with sleeves set in simply at the shoulder. The neck and cuffs are simply finished, leaving the garment useful under a cloak, mantle, or belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLongWoolTunic() => new()
	{
		Metadata = new("medieval_long_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:505", []),
		Noun = "tunic",
		ShortDescription = "a long $colour wool tunic",
		LongDescription = null,
		FullDescription = "This long $colour wool tunic is a long wool garment made from straight body panels with sleeves set in simply at the shoulder. The neck and cuffs are simply finished, leaving the garment useful under a cloak, mantle, or belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHeavyWoolCloak() => new()
	{
		Metadata = new("medieval_heavy_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:536", []),
		Noun = "cloak",
		ShortDescription = "a heavy $colour wool cloak",
		LongDescription = null,
		FullDescription = "This heavy $colour wool cloak is a broad outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSimpleWoolMantle() => new()
	{
		Metadata = new("medieval_simple_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:567", []),
		Noun = "mantle",
		ShortDescription = "a $colour wool mantle",
		LongDescription = null,
		FullDescription = "This $colour wool mantle is a broad outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurLinedCloak() => new()
	{
		Metadata = new("medieval_fur_lined_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:598", []),
		Noun = "cloak",
		ShortDescription = "a $colour fur-lined cloak",
		LongDescription = null,
		FullDescription = "This $colour fur-lined cloak is a broad outer cloth meant to drape over the shoulders and upper body. Fur shows at the opening and edges, adding thickness where wind would otherwise slip beneath the outer cloth. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1550.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenCoif() => new()
	{
		Metadata = new("medieval_linen_coif", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:629", []),
		Noun = "coif",
		ShortDescription = "a $colour linen coif",
		LongDescription = null,
		FullDescription = "This $colour linen coif is a close-fitting head covering with rounded crown panels and ties below the jaw or at the nape. The edges are narrow and smooth, keeping the cloth neat beneath a hood, cap, or veil. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Coif",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenVeil() => new()
	{
		Metadata = new("medieval_linen_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:660", []),
		Noun = "veil",
		ShortDescription = "a $colour linen veil",
		LongDescription = null,
		FullDescription = "This $colour linen veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolHood() => new()
	{
		Metadata = new("medieval_wool_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:693", []),
		Noun = "hood",
		ShortDescription = "a $colour wool hood",
		LongDescription = null,
		FullDescription = "This $colour wool hood is a wool head covering with a rounded crown and a fall that protects the neck and upper shoulders. The face opening is plainly finished, and the lower edge sits comfortably over other garments. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFeltCap() => new()
	{
		Metadata = new("medieval_felt_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:724", []),
		Noun = "cap",
		ShortDescription = "a $colour felt cap",
		LongDescription = null,
		FullDescription = "This $colour felt cap is a felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalRectangularWoolCloak() => new()
	{
		Metadata = new("medieval_rectangular_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:755", []),
		Noun = "cloak",
		ShortDescription = "a $colour rectangular wool cloak",
		LongDescription = null,
		FullDescription = "This $colour rectangular wool cloak is a rectangular outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBroochedWoolMantle() => new()
	{
		Metadata = new("medieval_brooched_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:786", []),
		Noun = "mantle",
		ShortDescription = "a $colour brooched wool mantle",
		LongDescription = null,
		FullDescription = "This $colour brooched wool mantle is a broad outer cloth meant to drape over the shoulders and upper body. The fastening is plainly visible near the shoulder, gathering the cloth into heavy folds without concealing how it is held. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTabletBandedWoolTunic() => new()
	{
		Metadata = new("medieval_tablet_banded_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:817", []),
		Noun = "tunic",
		ShortDescription = "a $colour tablet-banded tunic",
		LongDescription = null,
		FullDescription = "This $colour tablet-banded tunic is a tablet-banded wool garment made from straight body panels with sleeves set in simply at the shoulder. Tablet-woven bands frame the neck, cuffs, and lower hem, adding visible structure to the simple body cut. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 560.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalStraightWoolGown() => new()
	{
		Metadata = new("medieval_straight_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:848", []),
		Noun = "gown",
		ShortDescription = "a straight $colour wool gown",
		LongDescription = null,
		FullDescription = "This straight $colour wool gown is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPinnedWoolGown() => new()
	{
		Metadata = new("medieval_pinned_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:879", []),
		Noun = "gown",
		ShortDescription = "a pinned $colour wool gown",
		LongDescription = null,
		FullDescription = "This pinned $colour wool gown is a long wool garment with visible pinning points at the upper body. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineBorderedWoolGown() => new()
	{
		Metadata = new("medieval_fine_bordered_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:910", []),
		Noun = "gown",
		ShortDescription = "a fine $colour bordered gown",
		LongDescription = null,
		FullDescription = "This fine $colour bordered gown is a long wool garment with a controlled border at the neck, cuffs, and lower hem. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolLegWraps() => new()
	{
		Metadata = new("medieval_wool_leg_wraps", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:941", []),
		Noun = "leg wraps",
		ShortDescription = "a pair of $colour wool leg wraps",
		LongDescription = null,
		FullDescription = "These $colour wool leg wraps are long narrow strips of cloth made to wind around the lower legs or feet. The ends are left thin enough to tuck back under the wrapping, while the body of each strip has enough texture to grip itself. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Leggings",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenSaerk() => new()
	{
		Metadata = new("medieval_linen_saerk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:972", []),
		Noun = "særk",
		ShortDescription = "a $colour linen særk",
		LongDescription = null,
		FullDescription = "This $colour linen særk is a plain linen underdress with long sleeves, straight sides, and a simple rounded neck. It is long enough to show beneath sleeveless outer garments while still sitting close as an underlayer. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 340.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolHangerok() => new()
	{
		Metadata = new("medieval_wool_hangerok", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1003", []),
		Noun = "hangerok",
		ShortDescription = "a $colour wool hangerok",
		LongDescription = null,
		FullDescription = "This $colour wool hangerok is a sleeveless wool overdress with a straight body and strap-like shoulder hangers. The upper edge and shoulder loops are plainly finished, showing how it hangs over a linen underdress. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineWoolHangerok() => new()
	{
		Metadata = new("medieval_fine_wool_hangerok", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1034", []),
		Noun = "hangerok",
		ShortDescription = "a fine $colour wool hangerok",
		LongDescription = null,
		FullDescription = "This fine $colour wool hangerok is a sleeveless wool overdress with a straight body and strap-like shoulder hangers. The upper edge and hanging loops are more carefully finished, giving it a richer look over a linen underdress. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCheckedWoolMantle() => new()
	{
		Metadata = new("medieval_checked_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1065", []),
		Noun = "mantle",
		ShortDescription = "a $colour1 and $colour2 checked mantle",
		LongDescription = null,
		FullDescription = "This $colour1 and $colour2 checked mantle is a broad outer cloth meant to drape over the shoulders and upper body. The checked weave remains visible across the folds, breaking the broad surface into regular blocks of colour. The $colour1 and $colour2 threads are woven into an even checked pattern, giving the mantle visible contrast without relying on applied ornament.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_2BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineCheckedWoolMantle() => new()
	{
		Metadata = new("medieval_fine_checked_wool_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1096", []),
		Noun = "mantle",
		ShortDescription = "a fine $colour1 and $colour2 checked mantle",
		LongDescription = null,
		FullDescription = "This fine $colour1 and $colour2 checked mantle is a broad outer cloth meant to drape over the shoulders and upper body. The checked weave remains visible across the folds, breaking the broad surface into regular blocks of colour. The $colour1 and $colour2 threads are woven into an even checked pattern, giving the mantle visible contrast without relying on applied ornament.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 920.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_2FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalGaelicLeine() => new()
	{
		Metadata = new("medieval_gaelic_leine", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1127", []),
		Noun = "léine",
		ShortDescription = "a long $colour linen léine",
		LongDescription = null,
		FullDescription = "This long $colour linen léine is a long linen tunic with full sleeves and a generous body cut from straight panels. The length and loose fall make it useful as both underlayer and visible garment. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineBorderedLeine() => new()
	{
		Metadata = new("medieval_fine_bordered_leine", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1158", []),
		Noun = "léine",
		ShortDescription = "a fine $colour bordered léine",
		LongDescription = null,
		FullDescription = "This fine $colour bordered léine is a long linen tunic with full sleeves and a generous body cut from straight panels. The length and loose fall make it useful as both underlayer and visible garment. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 560.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolBrat() => new()
	{
		Metadata = new("medieval_wool_brat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1189", []),
		Noun = "brat",
		ShortDescription = "a $colour wool brat",
		LongDescription = null,
		FullDescription = "This $colour wool brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineBorderedBrat() => new()
	{
		Metadata = new("medieval_fine_bordered_brat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1220", []),
		Noun = "brat",
		ShortDescription = "a fine $colour bordered brat",
		LongDescription = null,
		FullDescription = "This fine $colour bordered brat is a broad wool cloak-mantle with a rectangular fall and enough weight to hold a strong shoulder drape. The edges are the most visible part, especially where the cloth is folded back from the body. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1000.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolTrews() => new()
	{
		Metadata = new("medieval_wool_trews", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1251", []),
		Noun = "trews",
		ShortDescription = "a pair of $colour wool trews",
		LongDescription = null,
		FullDescription = "These $colour wool trews are practical legwear with a gathered waist, straight seams, and enough room through the seat for riding or work. The legs narrow enough to sit beneath boots or wraps without excessive bunching. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFrankishKneeTunic() => new()
	{
		Metadata = new("medieval_frankish_knee_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1282", []),
		Noun = "tunic",
		ShortDescription = "a knee-length $colour wool tunic",
		LongDescription = null,
		FullDescription = "This knee-length $colour wool tunic is a knee-length wool garment made from straight body panels with sleeves set in simply at the shoulder. The neck and cuffs are simply finished, leaving the garment useful under a cloak, mantle, or belt. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineFrankishTunic() => new()
	{
		Metadata = new("medieval_fine_frankish_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1313", []),
		Noun = "tunic",
		ShortDescription = "a fine $colour bordered tunic",
		LongDescription = null,
		FullDescription = "This fine $colour bordered tunic is a bordered wool garment made from straight body panels with sleeves set in simply at the shoulder. A neat border marks the neck, cuffs, and hem, concentrating decoration where it will not be hidden by a belt. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 540.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCarolingianCloak() => new()
	{
		Metadata = new("medieval_carolingian_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1344", []),
		Noun = "cloak",
		ShortDescription = "a $colour shoulder-fastened cloak",
		LongDescription = null,
		FullDescription = "This $colour shoulder-fastened cloak is a broad outer cloth meant to drape over the shoulders and upper body. The fastening is plainly visible near the shoulder, gathering the cloth into heavy folds without concealing how it is held. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 880.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalNormanSplitTunic() => new()
	{
		Metadata = new("medieval_norman_split_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1375", []),
		Noun = "tunic",
		ShortDescription = "a split-skirt $colour wool tunic",
		LongDescription = null,
		FullDescription = "This split-skirt $colour wool tunic is a split-skirt wool garment made from straight body panels with sleeves set in simply at the shoulder. The skirt is split for easier riding and striding, with the openings finished so they do not tear upward. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSplitRidingTunic() => new()
	{
		Metadata = new("medieval_fine_split_riding_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1406", []),
		Noun = "tunic",
		ShortDescription = "a fine split-skirt $colour tunic",
		LongDescription = null,
		FullDescription = "This fine split-skirt $colour tunic is a split-skirt wool garment made from straight body panels with sleeves set in simply at the shoulder. The skirt is split for easier riding and striding, with the openings finished so they do not tear upward. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 640.0,
		Cost = 66.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCapetianCotte() => new()
	{
		Metadata = new("medieval_capetian_cotte", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1437", []),
		Noun = "cotte",
		ShortDescription = "a $colour wool cotte",
		LongDescription = null,
		FullDescription = "This $colour wool cotte is a long wool body garment cut close enough to layer under a surcoat or mantle. The sleeves are narrow through the forearm, and the skirt has enough fullness for walking. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges. With plain seams and a serviceable fall at the cuffs, neck, and hem.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineWoolCotte() => new()
	{
		Metadata = new("medieval_fine_wool_cotte", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1468", []),
		Noun = "cotte",
		ShortDescription = "a fine $colour wool cotte",
		LongDescription = null,
		FullDescription = "This fine $colour wool cotte is a long wool body garment cut close enough to layer under a surcoat or mantle. The sleeves are narrow through the forearm, and the skirt has enough fullness for walking. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow. With a smooth, more carefully finished fall at the cuffs, neck, and hem.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 600.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenChainse() => new()
	{
		Metadata = new("medieval_linen_chainse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1499", []),
		Noun = "chainse",
		ShortDescription = "a $colour linen chainse",
		LongDescription = null,
		FullDescription = "This $colour linen chainse is made from straight, plain linen panels, with narrow sleeves and small side openings for movement. The neck is simply finished and the hem is light enough to tuck beneath heavier layers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolKirtle() => new()
	{
		Metadata = new("medieval_wool_kirtle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1530", []),
		Noun = "kirtle",
		ShortDescription = "a $colour wool kirtle",
		LongDescription = null,
		FullDescription = "This $colour wool kirtle is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineWoolKirtle() => new()
	{
		Metadata = new("medieval_fine_wool_kirtle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1561", []),
		Noun = "kirtle",
		ShortDescription = "a fine $colour wool kirtle",
		LongDescription = null,
		FullDescription = "This fine $colour wool kirtle is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineBliaut() => new()
	{
		Metadata = new("medieval_fine_bliaut", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1592", []),
		Noun = "bliaut",
		ShortDescription = "a fine $colour bliaut",
		LongDescription = null,
		FullDescription = "This fine $colour bliaut is a long formal gown with close upper sleeves, a fitted body, and a fuller fall below the waist. The skirt hangs in deep, ordered folds and the sleeve openings are finished with care. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 880.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSimpleSurcoat() => new()
	{
		Metadata = new("medieval_simple_surcoat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1623", []),
		Noun = "surcoat",
		ShortDescription = "a sleeveless $colour surcoat",
		LongDescription = null,
		FullDescription = "This sleeveless $colour surcoat is a sleeveless wool overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 560.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSilkSurcoat() => new()
	{
		Metadata = new("medieval_fine_silk_surcoat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1654", []),
		Noun = "surcoat",
		ShortDescription = "a fine $colour silk surcoat",
		LongDescription = null,
		FullDescription = "This fine $colour silk surcoat is a sleeveless silk overgarment with open sides and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHalfCircleMantle() => new()
	{
		Metadata = new("medieval_half_circle_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1685", []),
		Noun = "mantle",
		ShortDescription = "a $colour half-circle mantle",
		LongDescription = null,
		FullDescription = "This $colour half-circle mantle is a half-circle outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFurTrimmedMantle() => new()
	{
		Metadata = new("medieval_fur_trimmed_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1716", []),
		Noun = "mantle",
		ShortDescription = "a $colour fur-trimmed mantle",
		LongDescription = null,
		FullDescription = "This $colour fur-trimmed mantle is a broad outer cloth meant to drape over the shoulders and upper body. Fur shows at the opening and edges, adding thickness where wind would otherwise slip beneath the outer cloth. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalGugelHood() => new()
	{
		Metadata = new("medieval_gugel_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1747", []),
		Noun = "hood",
		ShortDescription = "a $colour wool gugel hood",
		LongDescription = null,
		FullDescription = "This $colour wool gugel hood is a fitted hood with a short shoulder cape and a close face opening. The cape spreads over the upper shoulders, keeping the neck covered beneath cloaks or mantles. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hoodie",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBarbetteVeil() => new()
	{
		Metadata = new("medieval_barbette_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1778", []),
		Noun = "veil",
		ShortDescription = "a $colour linen barbette veil",
		LongDescription = null,
		FullDescription = "This $colour linen barbette veil combines a head veil with a band that frames the chin and temples. The cloth is light, even, and arranged to make a composed outline around the face. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineLinenVeil() => new()
	{
		Metadata = new("medieval_fine_linen_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1809", []),
		Noun = "veil",
		ShortDescription = "a fine $colour linen veil",
		LongDescription = null,
		FullDescription = "This fine $colour linen veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 110.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalChristianIberianCamisa() => new()
	{
		Metadata = new("medieval_christian_iberian_camisa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1840", []),
		Noun = "camisa",
		ShortDescription = "a $colour linen camisa",
		LongDescription = null,
		FullDescription = "This $colour linen camisa is a light linen shirt with straight-cut sleeves, a simple neck slit, and a loose body. The cloth is fine enough for warm weather or under-layering beneath woollen garments. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 11.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolSaya() => new()
	{
		Metadata = new("medieval_wool_saya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1871", []),
		Noun = "saya",
		ShortDescription = "a $colour wool saya",
		LongDescription = null,
		FullDescription = "This $colour wool saya is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineWoolSaya() => new()
	{
		Metadata = new("medieval_fine_wool_saya", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1902", []),
		Noun = "saya",
		ShortDescription = "a fine $colour wool saya",
		LongDescription = null,
		FullDescription = "This fine $colour wool saya is a long wool garment with a straight body, long sleeves, and a full lower fall. The cloth is shaped by seams and belting rather than close tailoring, falling in broad folds from shoulder to hem. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 730.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Dress",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIberianPellote() => new()
	{
		Metadata = new("medieval_iberian_pellote", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1933", []),
		Noun = "pellote",
		ShortDescription = "a sleeveless $colour pellote",
		LongDescription = null,
		FullDescription = "This sleeveless $colour pellote is a sleeveless wool overgarment with deep side openings and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSilkPellote() => new()
	{
		Metadata = new("medieval_fine_silk_pellote", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1964", []),
		Noun = "pellote",
		ShortDescription = "a fine $colour silk pellote",
		LongDescription = null,
		FullDescription = "This fine $colour silk pellote is a sleeveless silk overgarment with deep side openings and a long straight fall. It is made to sit over a cotte, saya, or gown, showing the layer beneath through the openings. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 460.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Sleeveless_Gown",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIberianManto() => new()
	{
		Metadata = new("medieval_iberian_manto", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:1995", []),
		Noun = "manto",
		ShortDescription = "a $colour wool manto",
		LongDescription = null,
		FullDescription = "This $colour wool manto is a broad mantle with a generous curved or rectangular fall over the shoulders. It is made to frame the upper body and cover the outer garment beneath without close tailoring. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineIberianManto() => new()
	{
		Metadata = new("medieval_fine_iberian_manto", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2026", []),
		Noun = "manto",
		ShortDescription = "a fine $colour silk-lined manto",
		LongDescription = null,
		FullDescription = "This fine $colour silk-lined manto is a broad mantle with a generous curved or rectangular fall over the shoulders. It is made to frame the upper body and cover the outer garment beneath without close tailoring. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 128.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenToca() => new()
	{
		Metadata = new("medieval_linen_toca", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2057", []),
		Noun = "toca",
		ShortDescription = "a $colour linen toca",
		LongDescription = null,
		FullDescription = "This $colour linen toca is a wrapped headcloth with enough length to cover the hair and frame the sides of the face. The fabric is light and foldable, with the edge kept plain for repeated tying. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenKamision() => new()
	{
		Metadata = new("medieval_linen_kamision", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2088", []),
		Noun = "kamision",
		ShortDescription = "a $colour linen kamision",
		LongDescription = null,
		FullDescription = "This $colour linen kamision is a linen body garment cut in straight panels with long sleeves and a plain neck opening. It is made to sit beneath heavier robes, leaving only the clean edges likely to show. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Shirt",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolDalmatic() => new()
	{
		Metadata = new("medieval_wool_dalmatic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2119", []),
		Noun = "dalmatic",
		ShortDescription = "a $colour wool dalmatic",
		LongDescription = null,
		FullDescription = "This $colour wool dalmatic is a loose robe with broad sleeves, a straight body, and a dignified fall from shoulder to hem. The cut leaves room for an underlayer, while the neck and cuffs are the clearest places for visible finishing. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSilkDalmatic() => new()
	{
		Metadata = new("medieval_silk_dalmatic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2150", []),
		Noun = "dalmatic",
		ShortDescription = "a fine $colour silk dalmatic",
		LongDescription = null,
		FullDescription = "This fine $colour silk dalmatic is a loose robe with broad sleeves, a straight body, and a dignified fall from shoulder to hem. The cut leaves room for an underlayer, while the neck and cuffs are the clearest places for visible finishing. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSkaramangion() => new()
	{
		Metadata = new("medieval_skaramangion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2181", []),
		Noun = "skaramangion",
		ShortDescription = "a $colour skaramangion",
		LongDescription = null,
		FullDescription = "This $colour skaramangion is a long, front-closing silk coat with a controlled skirt and sleeves suited to formal mounted dress. The panels are cut to hang close through the body before loosening below the waist. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 640.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSagion() => new()
	{
		Metadata = new("medieval_sagion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2212", []),
		Noun = "sagion",
		ShortDescription = "a $colour sagion",
		LongDescription = null,
		FullDescription = "This $colour sagion is a wool shoulder-cloak with a short, controlled fall and a visible fastening point near the upper edge. The cloth crosses the upper body cleanly without the volume of a full mantle. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTablionCloak() => new()
	{
		Metadata = new("medieval_tablion_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2243", []),
		Noun = "cloak",
		ShortDescription = "a $colour tablion cloak",
		LongDescription = null,
		FullDescription = "This $colour tablion cloak is a broad outer cloth meant to drape over the shoulders and upper body. The edges are turned or selvaged to keep the broad cloth from fraying under repeated wear. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Open)",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLorosSash() => new()
	{
		Metadata = new("medieval_loros_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2274", []),
		Noun = "sash",
		ShortDescription = "a fine $colour loros sash",
		LongDescription = null,
		FullDescription = "This fine $colour loros sash is a long, weighty strip of silk meant to cross and loop over formal robes. Its surface is closely woven, with the edges kept straight so the band lies in controlled folds. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 240.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Sash",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMaforion() => new()
	{
		Metadata = new("medieval_maforion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2305", []),
		Noun = "maforion",
		ShortDescription = "a $colour maforion veil",
		LongDescription = null,
		FullDescription = "This $colour maforion veil is a broad head-and-shoulder veil with a long fall around the face and upper body. The cloth is cut generously enough to layer over a robe without losing its clean outline. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineMaforion() => new()
	{
		Metadata = new("medieval_fine_maforion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2336", []),
		Noun = "maforion",
		ShortDescription = "a fine $colour maforion veil",
		LongDescription = null,
		FullDescription = "This fine $colour maforion veil is a broad head-and-shoulder veil with a long fall around the face and upper body. The cloth is cut generously enough to layer over a robe without losing its clean outline. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSoftCampagiShoes() => new()
	{
		Metadata = new("medieval_soft_campagi_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2367", []),
		Noun = "campagi",
		ShortDescription = "a pair of soft leather campagi",
		LongDescription = null,
		FullDescription = "These soft leather campagi are low shoes with rounded toes, open shaping at the upper, and neat straps across the foot. The leather is smoothed and carefully finished, giving the pair a cleaner appearance than ordinary work shoes. Natural variations in the leather show along the seams, toe creases, and cut edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenQamis() => new()
	{
		Metadata = new("medieval_linen_qamis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2397", []),
		Noun = "qamis",
		ShortDescription = "a $colour linen qamis",
		LongDescription = null,
		FullDescription = "This $colour linen qamis is a long, loose shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCottonQamis() => new()
	{
		Metadata = new("medieval_cotton_qamis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2428", []),
		Noun = "qamis",
		ShortDescription = "a $colour cotton qamis",
		LongDescription = null,
		FullDescription = "This $colour cotton qamis is a long, loose shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSilkQamis() => new()
	{
		Metadata = new("medieval_fine_silk_qamis", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2459", []),
		Noun = "qamis",
		ShortDescription = "a fine $colour silk qamis",
		LongDescription = null,
		FullDescription = "This fine $colour silk qamis is a long, loose shirt-like garment with straight side seams, long sleeves, and a simple neck opening. The cloth hangs freely from the shoulders and can be belted or left loose over trousers. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 340.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long-Sleeved_Tunic",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCottonSirwal() => new()
	{
		Metadata = new("medieval_cotton_sirwal", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2490", []),
		Noun = "sirwal",
		ShortDescription = "a pair of $colour cotton sirwal",
		LongDescription = null,
		FullDescription = "These $colour cotton sirwal are loose drawstring trousers with a roomy seat and tapered lower legs. The cloth gathers softly at the waist and falls in practical folds around the thighs. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSilkSirwal() => new()
	{
		Metadata = new("medieval_fine_silk_sirwal", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2521", []),
		Noun = "sirwal",
		ShortDescription = "a pair of fine $colour silk sirwal",
		LongDescription = null,
		FullDescription = "These fine $colour silk sirwal are loose drawstring trousers with a roomy seat and tapered lower legs. The cloth gathers softly at the waist and falls in practical folds around the thighs. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Legwear"
			],
		Components = [
				"Holdable",
				"Wear_Trousers",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolJubba() => new()
	{
		Metadata = new("medieval_wool_jubba", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2552", []),
		Noun = "jubba",
		ShortDescription = "a $colour wool jubba",
		LongDescription = null,
		FullDescription = "This $colour wool jubba is a loose wool robe with long sleeves, a straight body, and a generous front opening. It is cut to layer over lighter garments, with the edges and cuffs given the most visible finish. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineTirazRobe() => new()
	{
		Metadata = new("medieval_fine_tiraz_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2583", []),
		Noun = "robe",
		ShortDescription = "a fine $colour tiraz robe",
		LongDescription = null,
		FullDescription = "This fine $colour tiraz robe is a long silk robe with straight panels, full sleeves, and decorative bands set where the upper body is most visible. The bands read as formal textile ornament, worked into the surface rather than attached separately. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLinenIzar() => new()
	{
		Metadata = new("medieval_linen_izar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2614", []),
		Noun = "izar",
		ShortDescription = "a $colour linen izar",
		LongDescription = null,
		FullDescription = "This $colour linen izar is a broad linen wrap with enough length to pass around the body and fall in layered folds. The edges are plain and straight, letting the drape and overlap form most of the garment's shape. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 280.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Loincloth",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWoolBurnous() => new()
	{
		Metadata = new("medieval_wool_burnous", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2645", []),
		Noun = "burnous",
		ShortDescription = "a $colour wool burnous",
		LongDescription = null,
		FullDescription = "This $colour wool burnous is a wool cloak with a hooded upper edge and a loose body meant to wrap around the shoulders. The cloth falls in deep folds and leaves the front easy to draw close against wind or dust. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineBurnous() => new()
	{
		Metadata = new("medieval_fine_burnous", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2676", []),
		Noun = "burnous",
		ShortDescription = "a fine $colour wool burnous",
		LongDescription = null,
		FullDescription = "This fine $colour wool burnous is a fine wool cloak with a hooded upper edge and a loose body meant to wrap around the shoulders. The cloth falls in deep folds and leaves the front easy to draw close against wind or dust. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1000.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWrappedCottonTurban() => new()
	{
		Metadata = new("medieval_wrapped_cotton_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2707", []),
		Noun = "turban",
		ShortDescription = "a $colour cotton turban",
		LongDescription = null,
		FullDescription = "This $colour cotton turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSilkTurban() => new()
	{
		Metadata = new("medieval_fine_silk_turban", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2738", []),
		Noun = "turban",
		ShortDescription = "a fine $colour silk turban",
		LongDescription = null,
		FullDescription = "This fine $colour silk turban is a long cloth headwrap wound in layered folds around the crown. The length gives it a full profile without needing a stiff frame, and the tail can be tucked neatly into the wrapping. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Turban",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalCottonHeadVeil() => new()
	{
		Metadata = new("medieval_cotton_head_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2769", []),
		Noun = "veil",
		ShortDescription = "a $colour cotton head veil",
		LongDescription = null,
		FullDescription = "This $colour cotton head veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSilkVeil() => new()
	{
		Metadata = new("medieval_fine_silk_veil", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2800", []),
		Noun = "veil",
		ShortDescription = "a fine $colour silk veil",
		LongDescription = null,
		FullDescription = "This fine $colour silk veil is a light cloth head covering with a straight fall and softly folded edges. It can frame the face, cover the hair, or fall over the shoulders depending on how it is arranged. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 100.0,
		Cost = 56.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Veil",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSoftLeatherSlippers() => new()
	{
		Metadata = new("medieval_soft_leather_slippers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2831", []),
		Noun = "slippers",
		ShortDescription = "a pair of soft leather slippers",
		LongDescription = null,
		FullDescription = "These soft leather slippers are low, supple shoes with flexible soles and shallow uppers. The heels are lightly shaped and the opening is broad enough to slip on without heavy fastenings. Natural variations in the leather show along the seams, toe creases, and cut edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Footwear"
			],
		Components = [
				"Holdable",
				"Wear_Shoes",
				"Destroyable_Clothing",
				"Insulation_Minor",
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalSeljukCaftan() => new()
	{
		Metadata = new("medieval_seljuk_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2861", []),
		Noun = "caftan",
		ShortDescription = "a $colour front-opening caftan",
		LongDescription = null,
		FullDescription = "This $colour front-opening caftan is a front-opening coat with long sleeves, a beltable waist, and a skirt shaped for movement. The front edges and cuffs are the most carefully finished parts of the garment. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSeljukCaftan() => new()
	{
		Metadata = new("medieval_fine_seljuk_caftan", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2892", []),
		Noun = "caftan",
		ShortDescription = "a fine $colour silk caftan",
		LongDescription = null,
		FullDescription = "This fine $colour silk caftan is a front-opening coat with long sleeves, a beltable waist, and a skirt shaped for movement. The front edges and cuffs are the most carefully finished parts of the garment. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 680.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFeltTurkicCap() => new()
	{
		Metadata = new("medieval_felt_turkic_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2923", []),
		Noun = "cap",
		ShortDescription = "a $colour pointed felt cap",
		LongDescription = null,
		FullDescription = "This $colour pointed felt cap is a pointed felt cap with a firm crown and a plain lower edge. The felt is dense enough to keep its shape without much internal structure. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_LightClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalMagyarRidingCoat() => new()
	{
		Metadata = new("medieval_magyar_riding_coat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2954", []),
		Noun = "coat",
		ShortDescription = "a $colour belted riding coat",
		LongDescription = null,
		FullDescription = "This $colour belted riding coat is a long belted wool coat with a front opening and a skirt shaped for movement. The sleeves are practical and the waist is intended to be gathered by a belt rather than close tailoring. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_BasicColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineMagyarCoat() => new()
	{
		Metadata = new("medieval_fine_magyar_coat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:2985", []),
		Noun = "coat",
		ShortDescription = "a fine $colour fur-trimmed coat",
		LongDescription = null,
		FullDescription = "This fine $colour fur-trimmed coat is a long belted outer garment with fur marking the collar, cuffs, and front edge. The skirt is cut with enough width for riding or striding, while the upper body keeps a neat formal line. The $colour dye reads cleanly across the cloth, with richer depth where the folds overlap and catch shadow.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1050.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Long_Coat",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
