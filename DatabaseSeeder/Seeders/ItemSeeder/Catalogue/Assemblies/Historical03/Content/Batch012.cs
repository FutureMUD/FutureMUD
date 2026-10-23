#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical03Stock
{
	public static CatalogueItemDefinition MedievalDomesticWalnutDocumentCase() => new()
	{
		Metadata = new("medieval_domestic_walnut_document_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:9981", []),
		Noun = "case",
		ShortDescription = "a walnut document case",
		LongDescription = null,
		FullDescription = "This walnut document case is a small, well-made case built from walnut boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 430.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
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
		BuilderNotes = "Finer rigid case for household papers, seals, and protected personal writing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWashedLinenBundle() => new()
	{
		Metadata = new("medieval_domestic_washed_linen_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10012", []),
		Noun = "bundle",
		ShortDescription = "a washed linen bundle",
		LongDescription = null,
		FullDescription = "This washed linen bundle is a medium-sized, workmanlike bundle made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
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
				"Container_Sack",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Clean tied bundle for fresh linen, towels, napery, or folded garments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWaxTabletPouch() => new()
	{
		Metadata = new("medieval_domestic_wax_tablet_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10044", []),
		Noun = "pouch",
		ShortDescription = "a wax-tablet pouch",
		LongDescription = null,
		FullDescription = "This wax-tablet pouch is a small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230.0,
		Cost = 10.0m,
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
		BuilderNotes = "Flat beltable pouch sized for a small wax tablet, stylus, folded note, or household account slip.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWaxedCanvasPouch() => new()
	{
		Metadata = new("medieval_domestic_waxed_canvas_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10075", []),
		Noun = "pouch",
		ShortDescription = "a waxed canvas pouch",
		LongDescription = null,
		FullDescription = "This waxed canvas pouch is a very small, well-made pouch made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 135.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
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
		BuilderNotes = "Slightly weather-shedding pouch for tinder, waxed thread, tablets, or small dry packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWinterFurPack() => new()
	{
		Metadata = new("medieval_domestic_winter_fur_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10106", []),
		Noun = "pack",
		ShortDescription = "a fur-lined winter pack",
		LongDescription = null,
		FullDescription = "This fur-lined winter pack is a medium-sized, well-made pack made from dressed fur. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
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
		BuilderNotes = "Warm-lined pack for winter travel, spare mitts, clothing, and protected goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWoodenSpoolCase() => new()
	{
		Metadata = new("medieval_domestic_wooden_spool_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10137", []),
		Noun = "case",
		ShortDescription = "a wooden spool case",
		LongDescription = null,
		FullDescription = "This wooden spool case is a very small, workmanlike case built from beech boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
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
		BuilderNotes = "Compact case for thread spools, lace ends, and fine textile supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWoolDrawstringPouch() => new()
	{
		Metadata = new("medieval_domestic_wool_drawstring_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10168", []),
		Noun = "pouch",
		ShortDescription = "a wool drawstring pouch",
		LongDescription = null,
		FullDescription = "This wool drawstring pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
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
		BuilderNotes = "Soft drawstring pouch suited to counters, small game pieces, sewing odds, or minor keepsakes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWoolLinedBookBag() => new()
	{
		Metadata = new("medieval_domestic_wool_lined_book_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10200", []),
		Noun = "bag",
		ShortDescription = "a wool-lined book bag",
		LongDescription = null,
		FullDescription = "This wool-lined book bag is a small, well-made bag made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 24.0m,
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
				"Container_Tote",
				"Wear_Shoulder"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft-lined bag for books, tablets, papers, and objects needing some padding.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWoolTravelPack() => new()
	{
		Metadata = new("medieval_domestic_wool_travel_pack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10232", []),
		Noun = "pack",
		ShortDescription = "a wool travel pack",
		LongDescription = null,
		FullDescription = "This wool travel pack is a medium-sized, workmanlike pack made from woven wool. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
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
		BuilderNotes = "Back-worn pack for spare clothes, food, bedding, and travel goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDomesticWovenWillowBag() => new()
	{
		Metadata = new("medieval_domestic_woven_willow_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10264", []),
		Noun = "bag",
		ShortDescription = "a woven willow shoulder bag",
		LongDescription = null,
		FullDescription = "This woven willow shoulder bag is a small, workmanlike bag built from willow boards. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
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
		BuilderNotes = "Rigid woven shoulder bag for eggs, bread, vegetables, or light household goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingPersonalCoinCasket() => new()
	{
		Metadata = new("medieval_locking_personal_coin_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10295", []),
		Noun = "casket",
		ShortDescription = "a small coin casket",
		LongDescription = null,
		FullDescription = "This small coin casket is a small, well-made casket worked from brass. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small metal casket for household coin and compact valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingPersonalIvoryInlaidCasket() => new()
	{
		Metadata = new("medieval_locking_personal_ivory_inlaid_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10325", []),
		Noun = "casket",
		ShortDescription = "an ivory-inlaid casket",
		LongDescription = null,
		FullDescription = "This ivory-inlaid casket is a small, well-made casket worked from ivory. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 240.0m,
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
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine small casket with visible ivory panels and built-in lock behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingPersonalJewelCasket() => new()
	{
		Metadata = new("medieval_locking_personal_jewel_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10355", []),
		Noun = "casket",
		ShortDescription = "a small jewel casket",
		LongDescription = null,
		FullDescription = "This small jewel casket is a small, well-made casket built from walnut boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small lockable casket for jewellery, seals, keepsakes, or precious trinkets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingPersonalKeepsakeLockbox() => new()
	{
		Metadata = new("medieval_locking_personal_keepsake_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10385", []),
		Noun = "lockbox",
		ShortDescription = "a keepsake lockbox",
		LongDescription = null,
		FullDescription = "This keepsake lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain lockbox for personal letters, tokens, and small possessions.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingPersonalLetterLockbox() => new()
	{
		Metadata = new("medieval_locking_personal_letter_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10415", []),
		Noun = "lockbox",
		ShortDescription = "a letter lockbox",
		LongDescription = null,
		FullDescription = "This letter lockbox is a small, workmanlike lockbox built from beech boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small built-in-lock box for private letters and folded documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingPersonalSealCasket() => new()
	{
		Metadata = new("medieval_locking_personal_seal_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10445", []),
		Noun = "casket",
		ShortDescription = "a seal casket",
		LongDescription = null,
		FullDescription = "This seal casket is a small, well-made casket worked from bronze. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small bronze casket for personal seals, rings, or signet equipment as stored items.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingPersonalThreadCasket() => new()
	{
		Metadata = new("medieval_locking_personal_thread_casket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:10475", []),
		Noun = "casket",
		ShortDescription = "a needlework lock casket",
		LongDescription = null,
		FullDescription = "This needlework lock casket is a small, well-made casket built from walnut boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lockable casket for fine thread, needles, patterns, and personal work goods.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
