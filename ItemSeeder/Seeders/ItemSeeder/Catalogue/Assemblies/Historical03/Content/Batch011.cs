#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical03Stock
{
	public static CatalogueItemDefinition MedievalDomesticGarmentPinPouch() => new()
	{
		Metadata = new("medieval_domestic_garment_pin_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6829", []),
		Noun = "pouch",
		ShortDescription = "a garment-pin pouch",
		LongDescription = null,
		FullDescription = "This garment-pin pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small pouch for garment pins, hooks, toggles, and spare fastenings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticGoatskinStyleWaterskin() => new()
	{
		Metadata = new("medieval_domestic_goatskin_style_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6860", []),
		Noun = "waterskin",
		ShortDescription = "a goatskin-style waterskin",
		LongDescription = null,
		FullDescription = "This goatskin-style waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Traditional leather liquid vessel with shoulder carry behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHempHouseholdPack() => new()
	{
		Metadata = new("medieval_domestic_hemp_household_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6892", []),
		Noun = "pack",
		ShortDescription = "a hemp household pack",
		LongDescription = null,
		FullDescription = "This hemp household pack is a medium-sized, workmanlike pack made from woven hemp. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 780.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain pack for carrying bedding, linen, food, and domestic loads.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHempMarketingSack() => new()
	{
		Metadata = new("medieval_domestic_hemp_marketing_sack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6924", []),
		Noun = "sack",
		ShortDescription = "a hemp marketing sack",
		LongDescription = null,
		FullDescription = "This hemp marketing sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 430.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough shoulder-carried sack for household market purchases and dry goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHempWorkPouch() => new()
	{
		Metadata = new("medieval_domestic_hemp_work_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6956", []),
		Noun = "pouch",
		ShortDescription = "a hemp work pouch",
		LongDescription = null,
		FullDescription = "This hemp work pouch is a very small, workmanlike pouch made from woven hemp. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 100.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain work pouch for nails, pegs, chalk, hooks, or other rough household bits.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHerbGatheringPouch() => new()
	{
		Metadata = new("medieval_domestic_herb_gathering_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:6988", []),
		Noun = "pouch",
		ShortDescription = "an herb-gathering pouch",
		LongDescription = null,
		FullDescription = "This herb-gathering pouch is a small, workmanlike pouch made from woven hemp. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Breathable pouch for gathered herbs, flowers, tinder, or kitchen greens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHerderPack() => new()
	{
		Metadata = new("medieval_domestic_herder_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7020", []),
		Noun = "pack",
		ShortDescription = "a herder pack",
		LongDescription = null,
		FullDescription = "This herder pack is a medium-sized, workmanlike pack made from worked leather. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1150.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Weathered pack for food, tools, spare cord, and outdoor household gear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHornNeedleCase() => new()
	{
		Metadata = new("medieval_domestic_horn_needle_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7051", []),
		Noun = "case",
		ShortDescription = "a horn needle case",
		LongDescription = null,
		FullDescription = "This horn needle case is a tiny, well-made case worked from horn. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 45.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small rigid case for needles, pins, and small mending tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHornPowderFlask() => new()
	{
		Metadata = new("medieval_domestic_horn_powder_flask", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7082", []),
		Noun = "flask",
		ShortDescription = "a horn powder flask",
		LongDescription = null,
		FullDescription = "This horn powder flask is a very small, workmanlike flask worked from horn. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hard beltable dry flask for powdered pigment, salt, chalk, or spice.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHouseholdAccountsSatchel() => new()
	{
		Metadata = new("medieval_domestic_household_accounts_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7113", []),
		Noun = "satchel",
		ShortDescription = "a household accounts satchel",
		LongDescription = null,
		FullDescription = "This household accounts satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 570.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Satchel for tallies, account tablets, seals, and folded household records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHouseholdArrowQuiver() => new()
	{
		Metadata = new("medieval_domestic_household_arrow_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7144", []),
		Noun = "quiver",
		ShortDescription = "a household arrow quiver",
		LongDescription = null,
		FullDescription = "This household arrow quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shoulder quiver for arrows, bolts, long pins, or narrow rods stored at home.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHouseholdSpendingPurse() => new()
	{
		Metadata = new("medieval_domestic_household_spending_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7175", []),
		Noun = "purse",
		ShortDescription = "a household spending purse",
		LongDescription = null,
		FullDescription = "This household spending purse is a very small, workmanlike purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Everyday purse for small purchases, household accounts, and minor valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticHuntingQuiver() => new()
	{
		Metadata = new("medieval_domestic_hunting_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7206", []),
		Noun = "quiver",
		ShortDescription = "a hunting quiver",
		LongDescription = null,
		FullDescription = "This hunting quiver is a small, well-made quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-made shoulder quiver for hunting arrows and outdoor household equipment.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticIncenseSachet() => new()
	{
		Metadata = new("medieval_domestic_incense_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7237", []),
		Noun = "sachet",
		ShortDescription = "an incense sachet",
		LongDescription = null,
		FullDescription = "This incense sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 22.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine sachet for incense pieces, aromatic resin, or domestic devotional scenting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticIvoryCombCase() => new()
	{
		Metadata = new("medieval_domestic_ivory_comb_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7269", []),
		Noun = "case",
		ShortDescription = "an ivory comb case",
		LongDescription = null,
		FullDescription = "This ivory comb case is a very small, well-made case worked from ivory. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 110.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine rigid case for combs, pins, and dressing-table tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticKeyPouch() => new()
	{
		Metadata = new("medieval_domestic_key_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7300", []),
		Noun = "pouch",
		ShortDescription = "a household key pouch",
		LongDescription = null,
		FullDescription = "This household key pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 115.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Belt pouch intended for carrying keys, small latch pins, and household access pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticKitchenErrandPack() => new()
	{
		Metadata = new("medieval_domestic_kitchen_errand_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7331", []),
		Noun = "pack",
		ShortDescription = "a kitchen errand pack",
		LongDescription = null,
		FullDescription = "This kitchen errand pack is a medium-sized, workmanlike pack made from coarse canvas. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pack for carried food, utensils, pantry goods, and household errands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLampWickPouch() => new()
	{
		Metadata = new("medieval_domestic_lamp_wick_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7363", []),
		Noun = "pouch",
		ShortDescription = "a lamp-wick pouch",
		LongDescription = null,
		FullDescription = "This lamp-wick pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Clean pouch for spare lamp wicks, trimmed cord, and small lighting supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargeHouseholdBundle() => new()
	{
		Metadata = new("medieval_domestic_large_household_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7395", []),
		Noun = "bundle",
		ShortDescription = "a large household bundle",
		LongDescription = null,
		FullDescription = "This large household bundle is a medium-sized, workmanlike bundle made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft tied bundle for moving clothes, bedding, or household odds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLargeTravelWaterskin() => new()
	{
		Metadata = new("medieval_domestic_large_travel_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7427", []),
		Noun = "waterskin",
		ShortDescription = "a large travel waterskin",
		LongDescription = null,
		FullDescription = "This large travel waterskin is a medium-sized, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 680.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Larger shoulder waterskin for travel, field work, and household journeys.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLaundryBackPack() => new()
	{
		Metadata = new("medieval_domestic_laundry_back_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7459", []),
		Noun = "pack",
		ShortDescription = "a laundry back-pack",
		LongDescription = null,
		FullDescription = "This laundry back-pack is a medium-sized, workmanlike pack made from woven linen. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft back-worn pack for laundry, bedding, or folded household cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLaundryCarryBag() => new()
	{
		Metadata = new("medieval_domestic_laundry_carry_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7491", []),
		Noun = "bag",
		ShortDescription = "a laundry carry bag",
		LongDescription = null,
		FullDescription = "This laundry carry bag is a medium-sized, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large cloth bag for carrying washing, folded linen, or bedding between household spaces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLeatherBeltPouch() => new()
	{
		Metadata = new("medieval_domestic_leather_belt_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7523", []),
		Noun = "pouch",
		ShortDescription = "a leather belt pouch",
		LongDescription = null,
		FullDescription = "This leather belt pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable flap pouch for carried household tools, small valuables, and personal effects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLeatherCoinPurse() => new()
	{
		Metadata = new("medieval_domestic_leather_coin_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7554", []),
		Noun = "purse",
		ShortDescription = "a leather coin purse",
		LongDescription = null,
		FullDescription = "This leather coin purse is a very small, workmanlike purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 105.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable purse for coin, keys, seals, or compact personal goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLeatherShoulderSatchel() => new()
	{
		Metadata = new("medieval_domestic_leather_shoulder_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7585", []),
		Noun = "satchel",
		ShortDescription = "a leather shoulder satchel",
		LongDescription = null,
		FullDescription = "This leather shoulder satchel is a small, workmanlike satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable satchel for household papers, food, tools, and travel goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLeatherToolRoll() => new()
	{
		Metadata = new("medieval_domestic_leather_tool_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7616", []),
		Noun = "roll",
		ShortDescription = "a leather tool roll",
		LongDescription = null,
		FullDescription = "This leather tool roll is a small, well-made roll made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tough roll for finer tools, styluses, knives, punches, and mending implements.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLeatherTravelPack() => new()
	{
		Metadata = new("medieval_domestic_leather_travel_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7647", []),
		Noun = "pack",
		ShortDescription = "a leather travel pack",
		LongDescription = null,
		FullDescription = "This leather travel pack is a medium-sized, well-made pack made from worked leather. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Durable back-worn pack for valuable travel goods and protected personal belongings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLeatherWritingWallet() => new()
	{
		Metadata = new("medieval_domestic_leather_writing_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7678", []),
		Noun = "wallet",
		ShortDescription = "a leather writing wallet",
		LongDescription = null,
		FullDescription = "This leather writing wallet is a small, workmanlike wallet made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flat wallet for a wax tablet, stylus, notes, or domestic writing pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenBeltPouch() => new()
	{
		Metadata = new("medieval_domestic_linen_belt_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7709", []),
		Noun = "pouch",
		ShortDescription = "a linen belt pouch",
		LongDescription = null,
		FullDescription = "This linen belt pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small tied pouch for coins, household tokens, thread, or other daily pocket goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenBreadBag() => new()
	{
		Metadata = new("medieval_domestic_linen_bread_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7741", []),
		Noun = "bag",
		ShortDescription = "a linen bread bag",
		LongDescription = null,
		FullDescription = "This linen bread bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Breathable cloth bag for bread, rolls, wrapped food, or kitchen errands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenCoinPurse() => new()
	{
		Metadata = new("medieval_domestic_linen_coin_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7773", []),
		Noun = "purse",
		ShortDescription = "a linen coin purse",
		LongDescription = null,
		FullDescription = "This linen coin purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 75.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small purse for coins, tokens, and household spending money.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenKerchiefBag() => new()
	{
		Metadata = new("medieval_domestic_linen_kerchief_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7805", []),
		Noun = "bag",
		ShortDescription = "a linen kerchief bag",
		LongDescription = null,
		FullDescription = "This linen kerchief bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light bag for kerchiefs, veils, napkins, and small clean cloths.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticLinenShoppingBag() => new()
	{
		Metadata = new("medieval_domestic_linen_shopping_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7837", []),
		Noun = "bag",
		ShortDescription = "a linen shopping bag",
		LongDescription = null,
		FullDescription = "This linen shopping bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light cloth bag for carrying small domestic purchases and kitchen goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMarketPurse() => new()
	{
		Metadata = new("medieval_domestic_market_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7869", []),
		Noun = "purse",
		ShortDescription = "a market purse",
		LongDescription = null,
		FullDescription = "This market purse is a very small, workmanlike purse made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hard-wearing purse for market errands and domestic buying money.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMarketTote() => new()
	{
		Metadata = new("medieval_domestic_market_tote", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7901", []),
		Noun = "bag",
		ShortDescription = "a market tote bag",
		LongDescription = null,
		FullDescription = "This market tote bag is a small, workmanlike bag made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Reusable shoulder bag for household shopping, vegetables, bread, and parcels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMedicinePacket() => new()
	{
		Metadata = new("medieval_domestic_medicine_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7933", []),
		Noun = "packet",
		ShortDescription = "a medicine packet",
		LongDescription = null,
		FullDescription = "This medicine packet is a tiny, workmanlike packet made from layered paper. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Folded packet for small doses of household remedy powder or dried medicinal matter.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMedicinePouch() => new()
	{
		Metadata = new("medieval_domestic_medicine_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7965", []),
		Noun = "pouch",
		ShortDescription = "a medicine pouch",
		LongDescription = null,
		FullDescription = "This medicine pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cloth pouch for domestic remedies, wrapped herbs, salves, and measured packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMedicineSatchel() => new()
	{
		Metadata = new("medieval_domestic_medicine_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:7997", []),
		Noun = "satchel",
		ShortDescription = "a medicine satchel",
		LongDescription = null,
		FullDescription = "This medicine satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 580.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Satchel for remedy packets, herb pouches, small bottles, and household treatment supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMendingSatchel() => new()
	{
		Metadata = new("medieval_domestic_mending_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8028", []),
		Noun = "satchel",
		ShortDescription = "a mending satchel",
		LongDescription = null,
		FullDescription = "This mending satchel is a small, workmanlike satchel made from coarse canvas. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shoulder satchel for patches, thread, needles, laces, and repair tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMiniatureSewingBox() => new()
	{
		Metadata = new("medieval_domestic_miniature_sewing_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8060", []),
		Noun = "box",
		ShortDescription = "a miniature sewing box",
		LongDescription = null,
		FullDescription = "This miniature sewing box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small box for needles, thread, patches, thimbles, and household mending tools.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticMothHerbSachet() => new()
	{
		Metadata = new("medieval_domestic_moth_herb_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8090", []),
		Noun = "sachet",
		ShortDescription = "a moth-herb sachet",
		LongDescription = null,
		FullDescription = "This moth-herb sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small sachet intended for storing with linens, furs, or clothing chests.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNeedlePacket() => new()
	{
		Metadata = new("medieval_domestic_needle_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8122", []),
		Noun = "packet",
		ShortDescription = "a needle packet",
		LongDescription = null,
		FullDescription = "This needle packet is a tiny, workmanlike packet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny cloth packet for needles, pins, fishhooks, or small sharp goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNeedlePurse() => new()
	{
		Metadata = new("medieval_domestic_needle_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8154", []),
		Noun = "purse",
		ShortDescription = "a needle purse",
		LongDescription = null,
		FullDescription = "This needle purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small purse for needles, pins, buttons, and delicate repair pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticNeedleworkPouch() => new()
	{
		Metadata = new("medieval_domestic_needlework_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8186", []),
		Noun = "pouch",
		ShortDescription = "a needlework pouch",
		LongDescription = null,
		FullDescription = "This needlework pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft domestic pouch for needles, thread cards, thimbles, and small sewing repairs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticOakTabletCase() => new()
	{
		Metadata = new("medieval_domestic_oak_tablet_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8218", []),
		Noun = "case",
		ShortDescription = "an oak tablet case",
		LongDescription = null,
		FullDescription = "This oak tablet case is a small, workmanlike case built from oak boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rigid case sized for small wax tablets, folded notes, or household account slips.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticOiledDocumentWallet() => new()
	{
		Metadata = new("medieval_domestic_oiled_document_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8249", []),
		Noun = "wallet",
		ShortDescription = "an oiled document wallet",
		LongDescription = null,
		FullDescription = "This oiled document wallet is a small, well-made wallet made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 310.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Oiled leather wallet for folded papers, tablets, and protected account notes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPaintedKeepsakeBox() => new()
	{
		Metadata = new("medieval_domestic_painted_keepsake_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8280", []),
		Noun = "box",
		ShortDescription = "a painted keepsake box",
		LongDescription = null,
		FullDescription = "This painted keepsake box is a small, well-made box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small painted box for keepsakes, beads, ribbons, and personal mementos.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPaintedLeatherPurse() => new()
	{
		Metadata = new("medieval_domestic_painted_leather_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8310", []),
		Noun = "purse",
		ShortDescription = "a painted leather purse",
		LongDescription = null,
		FullDescription = "This painted leather purse is a very small, well-made purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine purse with decorative surface work for personal valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPantrySatchel() => new()
	{
		Metadata = new("medieval_domestic_pantry_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8341", []),
		Noun = "satchel",
		ShortDescription = "a pantry satchel",
		LongDescription = null,
		FullDescription = "This pantry satchel is a small, workmanlike satchel made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 310.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light satchel for kitchen packets, herbs, counted goods, and pantry errands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPantryTokenPouch() => new()
	{
		Metadata = new("medieval_domestic_pantry_token_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8373", []),
		Noun = "pouch",
		ShortDescription = "a pantry-token pouch",
		LongDescription = null,
		FullDescription = "This pantry-token pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 75.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small household pouch for tally tokens, marked scraps, tags, and pantry-counting pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPaperPatternTube() => new()
	{
		Metadata = new("medieval_domestic_paper_pattern_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8405", []),
		Noun = "tube",
		ShortDescription = "a paper pattern tube",
		LongDescription = null,
		FullDescription = "This paper pattern tube is a small, workmanlike tube made from layered paper. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Container_Pouch",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light tube for rolled patterns, notes, and household instructions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticParchmentRollCase() => new()
	{
		Metadata = new("medieval_domestic_parchment_roll_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8437", []),
		Noun = "case",
		ShortDescription = "a parchment roll case",
		LongDescription = null,
		FullDescription = "This parchment roll case is a small, workmanlike case made from layered parchment. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Container_Pouch",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flexible roll case for patterns, household notes, and small written pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticParchmentSatchel() => new()
	{
		Metadata = new("medieval_domestic_parchment_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8469", []),
		Noun = "satchel",
		ShortDescription = "a parchment satchel",
		LongDescription = null,
		FullDescription = "This parchment satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 590.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stiff satchel for folded parchment, letters, and household papers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPatchworkPurse() => new()
	{
		Metadata = new("medieval_domestic_patchwork_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8500", []),
		Noun = "purse",
		ShortDescription = "a patchwork purse",
		LongDescription = null,
		FullDescription = "This patchwork purse is a very small, plain purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 70.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Crude purse made from remnant cloth for low-value daily carrying.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPersonalAccountBox() => new()
	{
		Metadata = new("medieval_domestic_personal_account_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8532", []),
		Noun = "box",
		ShortDescription = "a personal account box",
		LongDescription = null,
		FullDescription = "This personal account box is a small, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact box for tally sticks, wax tablets, small weights, and household account slips.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPilgrimShoulderBag() => new()
	{
		Metadata = new("medieval_domestic_pilgrim_shoulder_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8562", []),
		Noun = "bag",
		ShortDescription = "a pilgrim shoulder bag",
		LongDescription = null,
		FullDescription = "This pilgrim shoulder bag is a small, workmanlike bag made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 330.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain shoulder bag for travel food, tokens, documents, and personal devotional items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainShoulderSatchel() => new()
	{
		Metadata = new("medieval_domestic_plain_shoulder_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8594", []),
		Noun = "satchel",
		ShortDescription = "a plain shoulder satchel",
		LongDescription = null,
		FullDescription = "This plain shoulder satchel is a small, workmanlike satchel made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple shoulder container for errands, food parcels, cloth, and small domestic goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPlainWaterskin() => new()
	{
		Metadata = new("medieval_domestic_plain_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8626", []),
		Noun = "waterskin",
		ShortDescription = "a plain waterskin",
		LongDescription = null,
		FullDescription = "This plain waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shoulder-carried liquid container for water, watered wine, or other permitted contents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPortableNestingBox() => new()
	{
		Metadata = new("medieval_domestic_portable_nesting_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8658", []),
		Noun = "box",
		ShortDescription = "a portable nesting box",
		LongDescription = null,
		FullDescription = "This portable nesting box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lidded box that can hold nested little boxes, packets, or personal goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPortableRelicBox() => new()
	{
		Metadata = new("medieval_domestic_portable_relic_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8688", []),
		Noun = "box",
		ShortDescription = "a small keepsake box",
		LongDescription = null,
		FullDescription = "This small keepsake box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lidded personal box for tokens, keepsakes, prayer beads, or precious mementos.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticPowderPacket() => new()
	{
		Metadata = new("medieval_domestic_powder_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8718", []),
		Noun = "packet",
		ShortDescription = "a powder packet",
		LongDescription = null,
		FullDescription = "This powder packet is a tiny, workmanlike packet made from layered parchment. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small folded packet for powders, pigments, cosmetics, or fine dry materials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticReedlikeScrollQuiver() => new()
	{
		Metadata = new("medieval_domestic_reedlike_scroll_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8749", []),
		Noun = "quiver",
		ShortDescription = "a scroll quiver",
		LongDescription = null,
		FullDescription = "This scroll quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Quiver-shaped case for rolled documents, arrows, or similarly long narrow goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRelicSachet() => new()
	{
		Metadata = new("medieval_domestic_relic_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8780", []),
		Noun = "sachet",
		ShortDescription = "a tiny keepsake sachet",
		LongDescription = null,
		FullDescription = "This tiny keepsake sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine tiny sachet for a keepsake, charm, bead, token, or devotional scrap.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRepairPouch() => new()
	{
		Metadata = new("medieval_domestic_repair_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8812", []),
		Noun = "pouch",
		ShortDescription = "a repair pouch",
		LongDescription = null,
		FullDescription = "This repair pouch is a small, workmanlike pouch made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 170.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small work pouch for awls, needles, scraps, laces, and mending supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRingPouch() => new()
	{
		Metadata = new("medieval_domestic_ring_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8844", []),
		Noun = "pouch",
		ShortDescription = "a ring pouch",
		LongDescription = null,
		FullDescription = "This ring pouch is a very small, well-made pouch made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine soft pouch for rings, earrings, small jewels, and other precious personal pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticRingPurse() => new()
	{
		Metadata = new("medieval_domestic_ring_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8876", []),
		Noun = "purse",
		ShortDescription = "a ring purse",
		LongDescription = null,
		FullDescription = "This ring purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft purse suited to rings and small precious objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSaffronSilkSachet() => new()
	{
		Metadata = new("medieval_domestic_saffron_silk_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8908", []),
		Noun = "sachet",
		ShortDescription = "a saffron silk sachet",
		LongDescription = null,
		FullDescription = "This saffron silk sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 15.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny fine sachet for rare spices, scent, or measured elite household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSaltSachet() => new()
	{
		Metadata = new("medieval_domestic_salt_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8940", []),
		Noun = "sachet",
		ShortDescription = "a salt sachet",
		LongDescription = null,
		FullDescription = "This salt sachet is a tiny, workmanlike sachet made from woven hemp. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small packet for salt, coarse seasoning, or little dry household stores.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticScribeSatchel() => new()
	{
		Metadata = new("medieval_domestic_scribe_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:8972", []),
		Noun = "satchel",
		ShortDescription = "a scribe satchel",
		LongDescription = null,
		FullDescription = "This scribe satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Better-made satchel for tablets, pens, packets, and domestic documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSealAndWaxBox() => new()
	{
		Metadata = new("medieval_domestic_seal_and_wax_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9003", []),
		Noun = "box",
		ShortDescription = "a seal-and-wax box",
		LongDescription = null,
		FullDescription = "This seal-and-wax box is a small, well-made box built from boxwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 400.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "boxwood",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lidded box for seal matrices, wax pieces, cords, and domestic document fittings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSealPouch() => new()
	{
		Metadata = new("medieval_domestic_seal_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9033", []),
		Noun = "pouch",
		ShortDescription = "a seal pouch",
		LongDescription = null,
		FullDescription = "This seal pouch is a very small, well-made pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small protective pouch for personal seals, wax fragments, and household account tokens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticShoeCarryBag() => new()
	{
		Metadata = new("medieval_domestic_shoe_carry_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9064", []),
		Noun = "bag",
		ShortDescription = "a shoe carry bag",
		LongDescription = null,
		FullDescription = "This shoe carry bag is a small, workmanlike bag made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shoulder bag for spare shoes, sandals, shoe brushes, or household footgear.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticShoulderQuiver() => new()
	{
		Metadata = new("medieval_domestic_shoulder_quiver", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9096", []),
		Noun = "quiver",
		ShortDescription = "a leather shoulder quiver",
		LongDescription = null,
		FullDescription = "This leather shoulder quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Quiver for arrows, bolts, short rods, or other long narrow carried items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticShoulderWaterskin() => new()
	{
		Metadata = new("medieval_domestic_shoulder_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9127", []),
		Noun = "waterskin",
		ShortDescription = "a shoulder waterskin",
		LongDescription = null,
		FullDescription = "This shoulder waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shoulder-carried liquid container for household travel, work, or errands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSilkGiftPurse() => new()
	{
		Metadata = new("medieval_domestic_silk_gift_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9159", []),
		Noun = "purse",
		ShortDescription = "a silk gift purse",
		LongDescription = null,
		FullDescription = "This silk gift purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 45.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small fine purse suited to jewellery, gift tokens, and elite personal display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSkinBundlePack() => new()
	{
		Metadata = new("medieval_domestic_skin_bundle_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9191", []),
		Noun = "pack",
		ShortDescription = "an animal-skin pack",
		LongDescription = null,
		FullDescription = "This animal-skin pack is a medium-sized, workmanlike pack made from cured animal skin. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "animal skin",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough hide pack for outdoor carrying, domestic travel, and durable field use.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallBeltWaterskin() => new()
	{
		Metadata = new("medieval_domestic_small_belt_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9222", []),
		Noun = "waterskin",
		ShortDescription = "a small belt waterskin",
		LongDescription = null,
		FullDescription = "This small belt waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small beltable liquid vessel for personal drinking water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallFoodWallet() => new()
	{
		Metadata = new("medieval_domestic_small_food_wallet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9254", []),
		Noun = "wallet",
		ShortDescription = "a small food wallet",
		LongDescription = null,
		FullDescription = "This small food wallet is a small, workmanlike wallet made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Simple wallet for wrapped food, flatbread, dried fruit, or kitchen portions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallPrayerBookSatchel() => new()
	{
		Metadata = new("medieval_domestic_small_prayer_book_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9286", []),
		Noun = "satchel",
		ShortDescription = "a small prayer-book satchel",
		LongDescription = null,
		FullDescription = "This small prayer-book satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 430.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact shoulder satchel for a small book, beads, papers, and devotional items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallSpoolPouch() => new()
	{
		Metadata = new("medieval_domestic_small_spool_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9317", []),
		Noun = "pouch",
		ShortDescription = "a small spool pouch",
		LongDescription = null,
		FullDescription = "This small spool pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft pouch for thread spools, lace ends, and small mending pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSmallWaterskin() => new()
	{
		Metadata = new("medieval_domestic_small_waterskin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9349", []),
		Noun = "waterskin",
		ShortDescription = "a small waterskin",
		LongDescription = null,
		FullDescription = "This small waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small beltable liquid container for water or drink during errands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSoapFlakeSachet() => new()
	{
		Metadata = new("medieval_domestic_soap_flake_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9381", []),
		Noun = "sachet",
		ShortDescription = "a soap-flake sachet",
		LongDescription = null,
		FullDescription = "This soap-flake sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small domestic sachet for soap flakes, washing scent, or linen-care materials.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSoftClothBundle() => new()
	{
		Metadata = new("medieval_domestic_soft_cloth_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9413", []),
		Noun = "bundle",
		ShortDescription = "a soft cloth bundle",
		LongDescription = null,
		FullDescription = "This soft cloth bundle is a small, workmanlike bundle made from woven wool. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tied bundle for spare clothing, bedding pieces, or household cloth.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpareClothesPack() => new()
	{
		Metadata = new("medieval_domestic_spare_clothes_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9445", []),
		Noun = "pack",
		ShortDescription = "a spare-clothes pack",
		LongDescription = null,
		FullDescription = "This spare-clothes pack is a medium-sized, workmanlike pack made from woven linen. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 640.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft pack for clothing changes, folded linens, or travel garments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpicePinchPouch() => new()
	{
		Metadata = new("medieval_domestic_spice_pinch_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9477", []),
		Noun = "pouch",
		ShortDescription = "a spice-pinch pouch",
		LongDescription = null,
		FullDescription = "This spice-pinch pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny pouch for pinches of spice, salt, seeds, or measured dry flavourings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpiceSachet() => new()
	{
		Metadata = new("medieval_domestic_spice_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9509", []),
		Noun = "sachet",
		ShortDescription = "a spice sachet",
		LongDescription = null,
		FullDescription = "This spice sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tiny sachet for measured spices, household flavourings, or aromatic packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSpindlePouch() => new()
	{
		Metadata = new("medieval_domestic_spindle_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9541", []),
		Noun = "pouch",
		ShortDescription = "a spindle pouch",
		LongDescription = null,
		FullDescription = "This spindle pouch is a small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Longer pouch for a drop spindle, small distaff parts, thread, and fibre-working pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSplitLeatherPouch() => new()
	{
		Metadata = new("medieval_domestic_split_leather_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9573", []),
		Noun = "pouch",
		ShortDescription = "a split leather pouch",
		LongDescription = null,
		FullDescription = "This split leather pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 155.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pouch divided into small internal spaces for keys, rings, seals, or personal counters.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticStitchedCoinPouch() => new()
	{
		Metadata = new("medieval_domestic_stitched_coin_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9604", []),
		Noun = "pouch",
		ShortDescription = "a stitched coin pouch",
		LongDescription = null,
		FullDescription = "This stitched coin pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small beltable pouch for coins and tokens without giving it built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticStrapHungBottleBasket() => new()
	{
		Metadata = new("medieval_domestic_strap_hung_bottle_basket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9635", []),
		Noun = "basket",
		ShortDescription = "a strap-hung bottle basket",
		LongDescription = null,
		FullDescription = "This strap-hung bottle basket is a small, workmanlike basket built from willow boards. The body is woven around a firm rim, with a flat base and open mouth. The weave tightens at the lower corners where the load bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rigid shoulder basket for bottles, jars, and fragile kitchen goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticStringAndCordPouch() => new()
	{
		Metadata = new("medieval_domestic_string_and_cord_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9666", []),
		Noun = "pouch",
		ShortDescription = "a string-and-cord pouch",
		LongDescription = null,
		FullDescription = "This string-and-cord pouch is a very small, workmanlike pouch made from woven hemp. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Rough pouch for string, cord, ties, and small household bindings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticSweetScentSachet() => new()
	{
		Metadata = new("medieval_domestic_sweet_scent_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9698", []),
		Noun = "sachet",
		ShortDescription = "a sweet-scent sachet",
		LongDescription = null,
		FullDescription = "This sweet-scent sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine sachet for scented herbs, flowers, and personal fragrance packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTabletAndStylusPouch() => new()
	{
		Metadata = new("medieval_domestic_tablet_and_stylus_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9730", []),
		Noun = "pouch",
		ShortDescription = "a tablet-and-stylus pouch",
		LongDescription = null,
		FullDescription = "This tablet-and-stylus pouch is a small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pouch fitted to carry a small tablet, stylus, and folded account pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTabletBandedPurse() => new()
	{
		Metadata = new("medieval_domestic_tablet_banded_purse", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9761", []),
		Noun = "purse",
		ShortDescription = "a tablet-banded purse",
		LongDescription = null,
		FullDescription = "This tablet-banded purse is a very small, well-made purse made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated purse for coins, gift money, or formal-use personal goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTabletWovenPouch() => new()
	{
		Metadata = new("medieval_domestic_tablet_woven_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9793", []),
		Noun = "pouch",
		ShortDescription = "a tablet-woven pouch",
		LongDescription = null,
		FullDescription = "This tablet-woven pouch is a very small, well-made pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorated small pouch for household valuables, prayer beads, or gift tokens.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTeaLeafSachet() => new()
	{
		Metadata = new("medieval_domestic_tea_leaf_sachet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9825", []),
		Noun = "sachet",
		ShortDescription = "a leaf sachet",
		LongDescription = null,
		FullDescription = "This leaf sachet is a tiny, workmanlike sachet made from layered paper. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Folded packet for dried leaves, herbs, or other delicate domestic plant matter.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticThreadPouch() => new()
	{
		Metadata = new("medieval_domestic_thread_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9857", []),
		Noun = "pouch",
		ShortDescription = "a thread pouch",
		LongDescription = null,
		FullDescription = "This thread pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small pouch for thread rolls, spare laces, patches, and domestic textile repairs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTinderPouch() => new()
	{
		Metadata = new("medieval_domestic_tinder_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9889", []),
		Noun = "pouch",
		ShortDescription = "a tinder pouch",
		LongDescription = null,
		FullDescription = "This tinder pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 125.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small leather pouch for tinder, flint fragments, wick scraps, and dry fire-starting supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticToolCarrySatchel() => new()
	{
		Metadata = new("medieval_domestic_tool_carry_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9920", []),
		Noun = "satchel",
		ShortDescription = "a household tool satchel",
		LongDescription = null,
		FullDescription = "This household tool satchel is a small, workmanlike satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Shoulder satchel for small domestic tools, repair pieces, hooks, nails, and cords.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticTravelSpiceBox() => new()
	{
		Metadata = new("medieval_domestic_travel_spice_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9951", []),
		Noun = "box",
		ShortDescription = "a travel spice box",
		LongDescription = null,
		FullDescription = "This travel spice box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 480.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lidded box for travel spices, scent packets, and measured dry goods.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
