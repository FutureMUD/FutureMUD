#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalJewelleryNeckSilverGiltCollarChain() => new()
	{
		Metadata = new("medieval_jewellery_neck_silver_gilt_collar_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3970", []),
		Noun = "chain",
		ShortDescription = "a silver-gilt collar chain",
		LongDescription = null,
		FullDescription = "A silver-gilt collar chain is arranged for the throat or upper chest, where its line of gold-washed silver, brighter on the raised places and paler at worn edges would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the silver-gilt collar chain is plainer, so attention stays on links or knots set at slightly irregular intervals and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 80.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckBrassMerchantChain() => new()
	{
		Metadata = new("medieval_jewellery_neck_brass_merchant_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4000", []),
		Noun = "chain",
		ShortDescription = "a brass merchant chain",
		LongDescription = null,
		FullDescription = "The brass merchant chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is yellow brass polished brighter on the edges, with links or knots set at slightly irregular intervals giving the eye a place to pause. The hidden side of the brass merchant chain is plainer, so attention stays on links or knots set at slightly irregular intervals and the visible finish. The brass merchant chain carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckHeavyCourtChain() => new()
	{
		Metadata = new("medieval_jewellery_neck_heavy_court_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4030", []),
		Noun = "chain",
		ShortDescription = "a heavy court chain",
		LongDescription = null,
		FullDescription = "Seen flat, heavy court chain shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the heavy court chain are decorative, but none suggest a moving catch or concealed compartment. The heavy court chain reads as courtly adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 160.0,
		Cost = 1100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckDoubleStrandSilverNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_double_strand_silver_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4060", []),
		Noun = "necklace",
		ShortDescription = "a double-strand silver necklace",
		LongDescription = null,
		FullDescription = "The double-strand silver necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with links or knots set at slightly irregular intervals giving the eye a place to pause. Close inspection of the double-strand silver necklace shows small tool marks rather than cast-perfect smoothness. The double-strand silver necklace is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 82.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckSmallPendantChain() => new()
	{
		Metadata = new("medieval_jewellery_neck_small_pendant_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4090", []),
		Noun = "chain",
		ShortDescription = "a small pendant chain",
		LongDescription = null,
		FullDescription = "The small pendant chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with links or knots set at slightly irregular intervals giving the eye a place to pause. Small handling marks cross the small pendant chain unevenly, especially where it would brush cloth or skin. The small pendant chain carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 42.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckLeatherTokenNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_leather_token_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4120", []),
		Noun = "necklace",
		ShortDescription = "a leather token necklace",
		LongDescription = null,
		FullDescription = "The leather token necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is worked leather with creases at the folds, with links or knots set at slightly irregular intervals giving the eye a place to pause. Rubbing has brightened the exposed parts of the leather token necklace without making it look neglected. The leather token necklace reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckSilkCordNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_silk_cord_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4150", []),
		Noun = "necklace",
		ShortDescription = "a silk cord necklace",
		LongDescription = null,
		FullDescription = "A silk cord necklace carries its value in the rhythm of its material: silk cord with a soft light across its twist. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The worked parts of the silk cord necklace are decorative, but none suggest a moving catch or concealed compartment. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckHempCordPendantString() => new()
	{
		Metadata = new("medieval_jewellery_neck_hemp_cord_pendant_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4180", []),
		Noun = "necklace",
		ShortDescription = "a hemp cord pendant string",
		LongDescription = null,
		FullDescription = "The hemp cord pendant string falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is coarse hemp cord with small stray fibres, with links or knots set at slightly irregular intervals giving the eye a place to pause. The worked parts of the hemp cord pendant string are decorative, but none suggest a moving catch or concealed compartment. The hemp cord pendant string feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Simple Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckGoldPendantNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_gold_pendant_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4210", []),
		Noun = "necklace",
		ShortDescription = "a gold pendant necklace",
		LongDescription = null,
		FullDescription = "A gold pendant necklace is arranged for the throat or upper chest, where its line of deep yellow gold with a warm, steady shine would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the gold pendant necklace is plainer, so attention stays on links or knots set at slightly irregular intervals and the visible finish. The gold pendant necklace carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 460.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckSilverCrescentPendant() => new()
	{
		Metadata = new("medieval_jewellery_neck_silver_crescent_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4240", []),
		Noun = "pendant",
		ShortDescription = "a silver crescent pendant",
		LongDescription = null,
		FullDescription = "A silver crescent pendant carries its value in the rhythm of its material: pale silver brightened at its raised surfaces. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Rubbing has brightened the exposed parts of the silver crescent pendant without making it look neglected. The silver crescent pendant reads as merchant adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 22.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckRockCrystalPendant() => new()
	{
		Metadata = new("medieval_jewellery_neck_rock_crystal_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4270", []),
		Noun = "pendant",
		ShortDescription = "a rock crystal pendant",
		LongDescription = null,
		FullDescription = "A rock crystal pendant carries its value in the rhythm of its material: clear rock crystal with tiny inner veils. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Small handling marks cross the rock crystal pendant unevenly, especially where it would brush cloth or skin. The rock crystal pendant carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "rock crystal",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckBlackJetChoker() => new()
	{
		Metadata = new("medieval_jewellery_neck_black_jet_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4300", []),
		Noun = "choker",
		ShortDescription = "a black jet choker",
		LongDescription = null,
		FullDescription = "The black jet choker falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is black jet polished until it gives back a quiet reflection, with a close line meant to rest high on the throat giving the eye a place to pause. The hidden side of the black jet choker is plainer, so attention stays on a close line meant to rest high on the throat and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckRedSilkChoker() => new()
	{
		Metadata = new("medieval_jewellery_neck_red_silk_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4330", []),
		Noun = "choker",
		ShortDescription = "a red silk choker",
		LongDescription = null,
		FullDescription = "The red silk choker falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is silk cord with a soft light across its twist, with a close line meant to rest high on the throat giving the eye a place to pause. Small handling marks cross the red silk choker unevenly, especially where it would brush cloth or skin. The red silk choker reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckSilverCloseCollar() => new()
	{
		Metadata = new("medieval_jewellery_neck_silver_close_collar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4360", []),
		Noun = "choker",
		ShortDescription = "a silver close collar",
		LongDescription = null,
		FullDescription = "Seen flat, silver close collar shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Rubbing has brightened the exposed parts of the silver close collar without making it look neglected. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 65.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckCowrieShellChoker() => new()
	{
		Metadata = new("medieval_jewellery_neck_cowrie_shell_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4390", []),
		Noun = "choker",
		ShortDescription = "a cowrie shell choker",
		LongDescription = null,
		FullDescription = "Seen flat, cowrie shell choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Fine wear gathers around the choker's highest places, leaving a close line meant to rest high on the throat easier to pick out. The piece would signal commoner taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 38.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cowrie shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckGoldBeadChoker() => new()
	{
		Metadata = new("medieval_jewellery_neck_gold_bead_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4420", []),
		Noun = "choker",
		ShortDescription = "a gold bead choker",
		LongDescription = null,
		FullDescription = "A gold bead choker carries its value in the rhythm of its material: deep yellow gold with a warm, steady shine. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The worked parts of the gold bead choker are decorative, but none suggest a moving catch or concealed compartment. The gold bead choker leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 58.0,
		Cost = 430.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckCoralChoker() => new()
	{
		Metadata = new("medieval_jewellery_neck_coral_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4450", []),
		Noun = "choker",
		ShortDescription = "a coral choker",
		LongDescription = null,
		FullDescription = "Seen flat, coral choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the coral choker are decorative, but none suggest a moving catch or concealed compartment. The coral choker leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 52.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckBeadpatternGlassChoker() => new()
	{
		Metadata = new("medieval_jewellery_neck_beadpattern_glass_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4480", []),
		Noun = "choker",
		ShortDescription = "a $beadpattern glass choker",
		LongDescription = null,
		FullDescription = "Seen flat, $beadpattern glass choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Fine wear gathers around the choker's highest places, leaving the chosen bead pattern breaking the line into visible rhythm easier to pick out. The $beadpattern glass choker reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 46.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_Glassware",
				"Variable_BeadPattern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckPearlCourtChoker() => new()
	{
		Metadata = new("medieval_jewellery_neck_pearl_court_choker", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4511", []),
		Noun = "choker",
		ShortDescription = "a pearl court choker",
		LongDescription = null,
		FullDescription = "Seen flat, pearl court choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Small handling marks cross the pearl court choker unevenly, especially where it would brush cloth or skin. The pearl court choker leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 50.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckBronzeNeckRing() => new()
	{
		Metadata = new("medieval_jewellery_neck_bronze_neck_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4541", []),
		Noun = "torc",
		ShortDescription = "a bronze neck ring",
		LongDescription = null,
		FullDescription = "A bronze neck ring carries its value in the rhythm of its material: brown-gold bronze with rubbed high points. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the torc's highest places, leaving terminals that are finished more boldly than the back curve easier to pick out. The bronze neck ring reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckTwistedSilverTorc() => new()
	{
		Metadata = new("medieval_jewellery_neck_twisted_silver_torc", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4571", []),
		Noun = "torc",
		ShortDescription = "a twisted silver torc",
		LongDescription = null,
		FullDescription = "Seen flat, twisted silver torc shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The hidden side of the twisted silver torc is plainer, so attention stays on terminals that are finished more boldly than the back curve and the visible finish. The twisted silver torc feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 280.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckHeavyGoldTorc() => new()
	{
		Metadata = new("medieval_jewellery_neck_heavy_gold_torc", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4601", []),
		Noun = "torc",
		ShortDescription = "a heavy gold torc",
		LongDescription = null,
		FullDescription = "A heavy gold torc is arranged for the throat or upper chest, where its line of deep yellow gold with a warm, steady shine would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Rubbing has brightened the exposed parts of the heavy gold torc without making it look neglected. The heavy gold torc leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 1250.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckIronNeckRing() => new()
	{
		Metadata = new("medieval_jewellery_neck_iron_neck_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4631", []),
		Noun = "torc",
		ShortDescription = "an iron neck ring",
		LongDescription = null,
		FullDescription = "The iron neck ring falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is dark wrought iron with a waxed, workmanlike finish, with terminals that are finished more boldly than the back curve giving the eye a place to pause. Rubbing has brightened the exposed parts of the iron neck ring without making it look neglected. The iron neck ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckGildedBronzeNeckRing() => new()
	{
		Metadata = new("medieval_jewellery_neck_gilded_bronze_neck_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4661", []),
		Noun = "torc",
		ShortDescription = "a gilded bronze neck ring",
		LongDescription = null,
		FullDescription = "A gilded bronze neck ring carries its value in the rhythm of its material: bronze showing through a thin golden surface at nicks and bends. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The hidden side of the gilded bronze neck ring is plainer, so attention stays on terminals that are finished more boldly than the back curve and the visible finish. The gilded bronze neck ring reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 170.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckPlainCopperTorc() => new()
	{
		Metadata = new("medieval_jewellery_neck_plain_copper_torc", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4691", []),
		Noun = "torc",
		ShortDescription = "a plain copper torc",
		LongDescription = null,
		FullDescription = "The plain copper torc falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is warm reddish copper darkened in the recesses, with terminals that are finished more boldly than the back curve giving the eye a place to pause. Fine wear gathers around the torc's highest places, leaving terminals that are finished more boldly than the back curve easier to pick out. The plain copper torc leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckAmberEndedSilverTorc() => new()
	{
		Metadata = new("medieval_jewellery_neck_amber_ended_silver_torc", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4721", []),
		Noun = "torc",
		ShortDescription = "an amber-ended silver torc",
		LongDescription = null,
		FullDescription = "An amber-ended silver torc is arranged for the throat or upper chest, where its line of pale silver brightened at its raised surfaces would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the amber-ended silver torc is plainer, so attention stays on visible amber detail worked into the public face and the visible finish. The amber-ended silver torc is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 210.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckBoneTokenPendant() => new()
	{
		Metadata = new("medieval_jewellery_neck_bone_token_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4751", []),
		Noun = "pendant",
		ShortDescription = "a bone token pendant",
		LongDescription = null,
		FullDescription = "Seen flat, bone token pendant shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the bone token pendant are decorative, but none suggest a moving catch or concealed compartment. The piece would signal commoner taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckCarvedAmberPendant() => new()
	{
		Metadata = new("medieval_jewellery_neck_carved_amber_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4782", []),
		Noun = "pendant",
		ShortDescription = "a carved amber pendant",
		LongDescription = null,
		FullDescription = "Seen flat, carved amber pendant shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the carved amber pendant are decorative, but none suggest a moving catch or concealed compartment. The carved amber pendant reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "amber",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckTurquoisePendant() => new()
	{
		Metadata = new("medieval_jewellery_neck_turquoise_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4813", []),
		Noun = "pendant",
		ShortDescription = "a turquoise pendant",
		LongDescription = null,
		FullDescription = "A turquoise pendant carries its value in the rhythm of its material: blue-green turquoise with fine darker lines. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Close inspection of the turquoise pendant shows small tool marks rather than cast-perfect smoothness. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 16.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "turquoise",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckJadePendant() => new()
	{
		Metadata = new("medieval_jewellery_neck_jade_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4844", []),
		Noun = "pendant",
		ShortDescription = "a jade pendant",
		LongDescription = null,
		FullDescription = "Seen flat, jade pendant shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The hidden side of the jade pendant is plainer, so attention stays on a small suspension hole worn smooth at the top and the visible finish. The jade pendant is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 22.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckNielloInlaidPendant() => new()
	{
		Metadata = new("medieval_jewellery_neck_niello_inlaid_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4875", []),
		Noun = "pendant",
		ShortDescription = "a niello-inlaid pendant",
		LongDescription = null,
		FullDescription = "The niello-inlaid pendant falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with visible niello detail worked into the public face giving the eye a place to pause. Close inspection of the niello-inlaid pendant shows small tool marks rather than cast-perfect smoothness. The niello-inlaid pendant carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbCopperWireBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_copper_wire_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4906", []),
		Noun = "bracelet",
		ShortDescription = "a copper wire bracelet",
		LongDescription = null,
		FullDescription = "A copper wire bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the copper wire bracelet without making it look neglected. The copper wire bracelet feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBronzeCuffBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_bronze_cuff_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4936", []),
		Noun = "bracelet",
		ShortDescription = "a bronze cuff bracelet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Brown-gold bronze with rubbed high points shows most clearly on the outer face, while the side that rests against the wearer is quieter. The worked parts of the bronze cuff bracelet are decorative, but none suggest a moving catch or concealed compartment. The bronze cuff bracelet feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBrassBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_brass_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4966", []),
		Noun = "bangle",
		ShortDescription = "a brass bangle",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of yellow brass polished brighter on the edges. Close inspection of the brass bangle shows small tool marks rather than cast-perfect smoothness. The brass bangle leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbSilverBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_silver_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:4996", []),
		Noun = "bracelet",
		ShortDescription = "a silver bracelet",
		LongDescription = null,
		FullDescription = "A silver bracelet is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and a repeated rhythm of small surface marks gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the silver bracelet without making it look neglected. The silver bracelet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 48.0,
		Cost = 92.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbSilverGiltBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_silver_gilt_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5026", []),
		Noun = "bracelet",
		ShortDescription = "a silver-gilt bracelet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Gold-washed silver, brighter on the raised places and paler at worn edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. The hidden side of the silver-gilt bracelet is plainer, so attention stays on a repeated rhythm of small surface marks and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 62.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGoldBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_gold_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5056", []),
		Noun = "bracelet",
		ShortDescription = "a gold bracelet",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of deep yellow gold with a warm, steady shine. Rubbing has brightened the exposed parts of the gold bracelet without making it look neglected. The gold bracelet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 60.0,
		Cost = 440.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBoneBeadBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_bone_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5086", []),
		Noun = "bracelet",
		ShortDescription = "a bone bead bracelet",
		LongDescription = null,
		FullDescription = "A bone bead bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the bone bead bracelet without making it look neglected. The bone bead bracelet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbShellBeadBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_shell_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5116", []),
		Noun = "bracelet",
		ShortDescription = "a shell bead bracelet",
		LongDescription = null,
		FullDescription = "A shell bead bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the shell bead bracelet are decorative, but none suggest a moving catch or concealed compartment. The shell bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbCoralBeadBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_coral_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5146", []),
		Noun = "bracelet",
		ShortDescription = "a coral bead bracelet",
		LongDescription = null,
		FullDescription = "A coral bead bracelet is made to circle a limb, with coral beads with a warm sea-worn surface providing the visible body of the ornament. The edges are kept low and rounded, and a repeated rhythm of small surface marks gives the piece a recognisable worked surface. The worked parts of the coral bead bracelet are decorative, but none suggest a moving catch or concealed compartment. The coral bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 32.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbJetBeadBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_jet_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5176", []),
		Noun = "bracelet",
		ShortDescription = "a jet bead bracelet",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of black jet polished until it gives back a quiet reflection. The hidden side of the jet bead bracelet is plainer, so attention stays on a repeated rhythm of small surface marks and the visible finish. The jet bead bracelet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGlassBeadBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_glass_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5206", []),
		Noun = "bracelet",
		ShortDescription = "a glass bead bracelet",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of coloured glass polished smooth enough to glow at the edges. Fine wear gathers around the bracelet's highest places, leaving a repeated rhythm of small surface marks easier to pick out. The glass bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbFaienceBeadBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_faience_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5236", []),
		Noun = "bracelet",
		ShortDescription = "a faience bead bracelet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Glazed faience with small pooling marks in the colour shows most clearly on the outer face, while the side that rests against the wearer is quieter. Small handling marks cross the faience bead bracelet unevenly, especially where it would brush cloth or skin. The faience bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "faience",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbLeatherCordBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_leather_cord_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5266", []),
		Noun = "bracelet",
		ShortDescription = "a leather cord bracelet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Worked leather with creases at the folds shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bracelet's highest places, leaving a repeated rhythm of small surface marks easier to pick out. The leather cord bracelet reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbSilkKnotBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_silk_knot_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5296", []),
		Noun = "bracelet",
		ShortDescription = "a silk knot bracelet",
		LongDescription = null,
		FullDescription = "A silk knot bracelet is made to circle a limb, with silk cord with a soft light across its twist providing the visible body of the ornament. The edges are kept low and rounded, and a repeated rhythm of small surface marks gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the silk knot bracelet without making it look neglected. The silk knot bracelet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedBronzeBraceletSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_bronze_bracelet_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5326", []),
		Noun = "bracelet",
		ShortDescription = "a paired bronze bracelet set",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Brown-gold bronze with rubbed high points shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the paired bronze bracelet set without making it look neglected. The paired bronze bracelet set is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedSilverBraceletSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_silver_bracelet_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5356", []),
		Noun = "bracelet",
		ShortDescription = "a paired silver bracelet set",
		LongDescription = null,
		FullDescription = "A paired silver bracelet set would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the paired silver bracelet set are decorative, but none suggest a moving catch or concealed compartment. The paired silver bracelet set feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 84.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbMotifBrassBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_motif_brass_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5386", []),
		Noun = "bracelet",
		ShortDescription = "a $motif brass bracelet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Yellow brass polished brighter on the edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the $motif brass bracelet without making it look neglected. The $motif brass bracelet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 48.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbFinishSilverBracelet() => new()
	{
		Metadata = new("medieval_jewellery_limb_finish_silver_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5417", []),
		Noun = "bracelet",
		ShortDescription = "a $finish silver bracelet",
		LongDescription = null,
		FullDescription = "A $finish silver bracelet is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and the chosen finish left deliberately visible on the worked surface gives the piece a recognisable worked surface. Small handling marks cross the $finish silver bracelet unevenly, especially where it would brush cloth or skin. The $finish silver bracelet feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 46.0,
		Cost = 100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal",
				"Variable_MetalFinish"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbNarrowGlassBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_narrow_glass_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5448", []),
		Noun = "bangle",
		ShortDescription = "a narrow glass bangle",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Coloured glass polished smooth enough to glow at the edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bangle's highest places, leaving a continuous curve made to show as the hand moves easier to pick out. The narrow glass bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBlueFaienceBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_blue_faience_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5478", []),
		Noun = "bangle",
		ShortDescription = "a blue faience bangle",
		LongDescription = null,
		FullDescription = "A blue faience bangle would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Small handling marks cross the blue faience bangle unevenly, especially where it would brush cloth or skin. The blue faience bangle feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "faience",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbConchShellBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_conch_shell_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5508", []),
		Noun = "bangle",
		ShortDescription = "a conch shell bangle",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Cut conch shell with creamy curves and warmer pinkish edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bangle's highest places, leaving a continuous curve made to show as the hand moves easier to pick out. The conch shell bangle leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 38.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "conch shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbMotherOfPearlBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_mother_of_pearl_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5538", []),
		Noun = "bangle",
		ShortDescription = "a mother-of-pearl bangle",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Mother-of-pearl flashing softly under the surface shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bangle's highest places, leaving a continuous curve made to show as the hand moves easier to pick out. The mother-of-pearl bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 34.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mother-of-pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPlainGoldBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_plain_gold_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5568", []),
		Noun = "bangle",
		ShortDescription = "a plain gold bangle",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A continuous curve made to show as the hand moves breaks up the line of deep yellow gold with a warm, steady shine. Rubbing has brightened the exposed parts of the plain gold bangle without making it look neglected. The plain gold bangle feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 400.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbRedCoralBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_red_coral_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5598", []),
		Noun = "bangle",
		ShortDescription = "a red coral bangle",
		LongDescription = null,
		FullDescription = "A red coral bangle would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the red coral bangle without making it look neglected. The red coral bangle leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 45.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedGlassBangleSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_glass_bangle_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5628", []),
		Noun = "bangle",
		ShortDescription = "a paired glass bangle set",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Coloured glass polished smooth enough to glow at the edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Close inspection of the paired glass bangle set shows small tool marks rather than cast-perfect smoothness. The paired glass bangle set is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedGoldBangleSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_gold_bangle_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5658", []),
		Noun = "bangle",
		ShortDescription = "a paired gold bangle set",
		LongDescription = null,
		FullDescription = "A paired gold bangle set would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the paired gold bangle set are decorative, but none suggest a moving catch or concealed compartment. The paired gold bangle set is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 110.0,
		Cost = 780.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedConchBangleSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_conch_bangle_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5688", []),
		Noun = "bangle",
		ShortDescription = "a paired conch bangle set",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A continuous curve made to show as the hand moves breaks up the line of cut conch shell with creamy curves and warmer pinkish edges. The hidden side of the paired conch bangle set is plainer, so attention stays on a continuous curve made to show as the hand moves and the visible finish. The paired conch bangle set carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "conch shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbSilverGiltBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_silver_gilt_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5718", []),
		Noun = "bangle",
		ShortDescription = "a silver-gilt bangle",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Gold-washed silver, brighter on the raised places and paler at worn edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Close inspection of the silver-gilt bangle shows small tool marks rather than cast-perfect smoothness. The silver-gilt bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 52.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGildedCopperBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_gilded_copper_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5748", []),
		Noun = "bangle",
		ShortDescription = "a gilded copper bangle",
		LongDescription = null,
		FullDescription = "A gilded copper bangle is made to circle a limb, with coppery red metal visible beneath worn gilt edges providing the visible body of the ornament. The edges are kept low and rounded, and a continuous curve made to show as the hand moves gives the piece a recognisable worked surface. The hidden side of the gilded copper bangle is plainer, so attention stays on a continuous curve made to show as the hand moves and the visible finish. The gilded copper bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbJadeBangle() => new()
	{
		Metadata = new("medieval_jewellery_limb_jade_bangle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5778", []),
		Noun = "bangle",
		ShortDescription = "a jade bangle",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Green jade smoothed to a cool waxy finish shows most clearly on the outer face, while the side that rests against the wearer is quieter. The worked parts of the jade bangle are decorative, but none suggest a moving catch or concealed compartment. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 60.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBronzeUpperArmArmlet() => new()
	{
		Metadata = new("medieval_jewellery_limb_bronze_upper_arm_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5808", []),
		Noun = "armlet",
		ShortDescription = "a bronze upper-arm armlet",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A slightly widened face meant to turn outward breaks up the line of brown-gold bronze with rubbed high points. The hidden side of the bronze upper-arm armlet is plainer, so attention stays on a slightly widened face meant to turn outward and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbSilverArmlet() => new()
	{
		Metadata = new("medieval_jewellery_limb_silver_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5838", []),
		Noun = "armlet",
		ShortDescription = "a silver armlet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Pale silver brightened at its raised surfaces shows most clearly on the outer face, while the side that rests against the wearer is quieter. Close inspection of the silver armlet shows small tool marks rather than cast-perfect smoothness. The silver armlet reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 105.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGoldArmlet() => new()
	{
		Metadata = new("medieval_jewellery_limb_gold_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5868", []),
		Noun = "armlet",
		ShortDescription = "a gold armlet",
		LongDescription = null,
		FullDescription = "A gold armlet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Small handling marks cross the gold armlet unevenly, especially where it would brush cloth or skin. The gold armlet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 115.0,
		Cost = 760.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbLeatherUpperArmBand() => new()
	{
		Metadata = new("medieval_jewellery_limb_leather_upper_arm_band", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5898", []),
		Noun = "armlet",
		ShortDescription = "a leather upper-arm band",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Worked leather with creases at the folds shows most clearly on the outer face, while the side that rests against the wearer is quieter. The worked parts of the leather upper-arm band are decorative, but none suggest a moving catch or concealed compartment. The leather upper-arm band carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbHornArmlet() => new()
	{
		Metadata = new("medieval_jewellery_limb_horn_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5928", []),
		Noun = "armlet",
		ShortDescription = "a horn armlet",
		LongDescription = null,
		FullDescription = "A horn armlet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Fine wear gathers around the armlet's highest places, leaving a slightly widened face meant to turn outward easier to pick out. The horn armlet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 60.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGildedBronzeArmlet() => new()
	{
		Metadata = new("medieval_jewellery_limb_gilded_bronze_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5958", []),
		Noun = "armlet",
		ShortDescription = "a gilded bronze armlet",
		LongDescription = null,
		FullDescription = "A gilded bronze armlet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The hidden side of the gilded bronze armlet is plainer, so attention stays on a slightly widened face meant to turn outward and the visible finish. The gilded bronze armlet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 100.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbMotifSilverArmlet() => new()
	{
		Metadata = new("medieval_jewellery_limb_motif_silver_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:5988", []),
		Noun = "armlet",
		ShortDescription = "a $motif silver armlet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Pale silver brightened at its raised surfaces shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the $motif silver armlet without making it look neglected. The $motif silver armlet reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 110.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbCoralSetArmlet() => new()
	{
		Metadata = new("medieval_jewellery_limb_coral_set_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6019", []),
		Noun = "armlet",
		ShortDescription = "a coral-set armlet",
		LongDescription = null,
		FullDescription = "A coral-set armlet is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and visible coral detail worked into the public face gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the coral-set armlet without making it look neglected. The coral-set armlet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 125.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbSteppeSilverArmRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_steppe_silver_arm_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6049", []),
		Noun = "armlet",
		ShortDescription = "a steppe silver arm ring",
		LongDescription = null,
		FullDescription = "A steppe silver arm ring would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Close inspection of the steppe silver arm ring shows small tool marks rather than cast-perfect smoothness. The steppe silver arm ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 130.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbHeavyBronzeArmRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_heavy_bronze_arm_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6079", []),
		Noun = "armlet",
		ShortDescription = "a heavy bronze arm ring",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A slightly widened face meant to turn outward breaks up the line of brown-gold bronze with rubbed high points. Fine wear gathers around the armlet's highest places, leaving a slightly widened face meant to turn outward easier to pick out. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbCopperAnklet() => new()
	{
		Metadata = new("medieval_jewellery_limb_copper_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6109", []),
		Noun = "anklet",
		ShortDescription = "a copper anklet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Warm reddish copper darkened in the recesses shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the copper anklet without making it look neglected. The copper anklet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedBrassAnkletSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_brass_anklet_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6139", []),
		Noun = "anklet",
		ShortDescription = "a paired brass anklet set",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. The inner side kept plainer than the visible outer arc breaks up the line of yellow brass polished brighter on the edges. Rubbing has brightened the exposed parts of the paired brass anklet set without making it look neglected. The paired brass anklet set carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklets",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedSilverAnkletSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_silver_anklet_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6169", []),
		Noun = "anklet",
		ShortDescription = "a paired silver anklet set",
		LongDescription = null,
		FullDescription = "A paired silver anklet set is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and the inner side kept plainer than the visible outer arc gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the paired silver anklet set without making it look neglected. The paired silver anklet set is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 88.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklets",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBellLessBronzeAnklet() => new()
	{
		Metadata = new("medieval_jewellery_limb_bell_less_bronze_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6199", []),
		Noun = "anklet",
		ShortDescription = "a bell-less bronze anklet",
		LongDescription = null,
		FullDescription = "A bell-less bronze anklet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the bell-less bronze anklet without making it look neglected. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 54.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbCowrieShellAnkletString() => new()
	{
		Metadata = new("medieval_jewellery_limb_cowrie_shell_anklet_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6229", []),
		Noun = "anklet",
		ShortDescription = "a cowrie shell anklet string",
		LongDescription = null,
		FullDescription = "A cowrie shell anklet string would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the cowrie shell anklet string are decorative, but none suggest a moving catch or concealed compartment. The cowrie shell anklet string carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 38.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cowrie shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbConchShellAnklet() => new()
	{
		Metadata = new("medieval_jewellery_limb_conch_shell_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6259", []),
		Noun = "anklet",
		ShortDescription = "a conch shell anklet",
		LongDescription = null,
		FullDescription = "A conch shell anklet is made to circle a limb, with cut conch shell with creamy curves and warmer pinkish edges providing the visible body of the ornament. The edges are kept low and rounded, and the inner side kept plainer than the visible outer arc gives the piece a recognisable worked surface. Small handling marks cross the conch shell anklet unevenly, especially where it would brush cloth or skin. The conch shell anklet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "conch shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGoldCourtAnklet() => new()
	{
		Metadata = new("medieval_jewellery_limb_gold_court_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6289", []),
		Noun = "anklet",
		ShortDescription = "a gold court anklet",
		LongDescription = null,
		FullDescription = "A gold court anklet is made to circle a limb, with deep yellow gold with a warm, steady shine providing the visible body of the ornament. The edges are kept low and rounded, and the inner side kept plainer than the visible outer arc gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the gold court anklet without making it look neglected. The gold court anklet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 62.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGlassBeadAnklet() => new()
	{
		Metadata = new("medieval_jewellery_limb_glass_bead_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6319", []),
		Noun = "anklet",
		ShortDescription = "a glass bead anklet",
		LongDescription = null,
		FullDescription = "A glass bead anklet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Fine wear gathers around the anklet's highest places, leaving the inner side kept plainer than the visible outer arc easier to pick out. The glass bead anklet feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 36.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbCoralBeadAnklet() => new()
	{
		Metadata = new("medieval_jewellery_limb_coral_bead_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6349", []),
		Noun = "anklet",
		ShortDescription = "a coral bead anklet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Coral beads with a warm sea-worn surface shows most clearly on the outer face, while the side that rests against the wearer is quieter. Small handling marks cross the coral bead anklet unevenly, especially where it would brush cloth or skin. The coral bead anklet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 40.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBraidedHempAnklet() => new()
	{
		Metadata = new("medieval_jewellery_limb_braided_hemp_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6379", []),
		Noun = "anklet",
		ShortDescription = "a braided hemp anklet",
		LongDescription = null,
		FullDescription = "A braided hemp anklet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Fine wear gathers around the anklet's highest places, leaving the inner side kept plainer than the visible outer arc easier to pick out. The piece would signal simple household taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Simple Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPlainSilverToeRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_plain_silver_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6409", []),
		Noun = "ring",
		ShortDescription = "a plain silver toe ring",
		LongDescription = null,
		FullDescription = "A plain silver toe ring would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The hidden side of the plain silver toe ring is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The plain silver toe ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbPairedSilverToeRingSet() => new()
	{
		Metadata = new("medieval_jewellery_limb_paired_silver_toe_ring_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6439", []),
		Noun = "ring",
		ShortDescription = "a paired silver toe-ring set",
		LongDescription = null,
		FullDescription = "A paired silver toe-ring set would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The hidden side of the paired silver toe-ring set is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbCopperToeRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_copper_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6469", []),
		Noun = "ring",
		ShortDescription = "a copper toe ring",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A small open gap where the band can be adjusted breaks up the line of warm reddish copper darkened in the recesses. The hidden side of the copper toe ring is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The copper toe ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbGoldToeRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_gold_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6499", []),
		Noun = "ring",
		ShortDescription = "a gold toe ring",
		LongDescription = null,
		FullDescription = "A gold toe ring is made to circle a limb, with deep yellow gold with a warm, steady shine providing the visible body of the ornament. The edges are kept low and rounded, and a small open gap where the band can be adjusted gives the piece a recognisable worked surface. Fine wear gathers around the ring's highest places, leaving a small open gap where the band can be adjusted easier to pick out. The gold toe ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 5.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbShellSetToeRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_shell_set_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6529", []),
		Noun = "ring",
		ShortDescription = "a shell-set toe ring",
		LongDescription = null,
		FullDescription = "A shell-set toe ring is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and visible shell detail worked into the public face gives the piece a recognisable worked surface. The worked parts of the shell-set toe ring are decorative, but none suggest a moving catch or concealed compartment. The shell-set toe ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbBrassToeRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_brass_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6559", []),
		Noun = "ring",
		ShortDescription = "a brass toe ring",
		LongDescription = null,
		FullDescription = "A brass toe ring is made to circle a limb, with yellow brass polished brighter on the edges providing the visible body of the ornament. The edges are kept low and rounded, and a small open gap where the band can be adjusted gives the piece a recognisable worked surface. The hidden side of the brass toe ring is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The brass toe ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbTinyPearlToeRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_tiny_pearl_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6589", []),
		Noun = "ring",
		ShortDescription = "a tiny pearl toe ring",
		LongDescription = null,
		FullDescription = "A tiny pearl toe ring is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and visible pearl detail worked into the public face gives the piece a recognisable worked surface. The worked parts of the tiny pearl toe ring are decorative, but none suggest a moving catch or concealed compartment. The tiny pearl toe ring reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 5.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryLimbMotifToeRing() => new()
	{
		Metadata = new("medieval_jewellery_limb_motif_toe_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6619", []),
		Noun = "ring",
		ShortDescription = "a $motif toe ring",
		LongDescription = null,
		FullDescription = "A $motif toe ring is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and the selected motif set into the outward-facing surface gives the piece a recognisable worked surface. Small handling marks cross the $motif toe ring unevenly, especially where it would brush cloth or skin. The $motif toe ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfCopperHoopEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_copper_hoop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6650", []),
		Noun = "earring",
		ShortDescription = "a pair of copper hoop earrings",
		LongDescription = null,
		FullDescription = "The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Warm reddish copper darkened in the recesses gives it colour and finish without making it look bulky. Close inspection of the pair of copper hoop earrings shows small tool marks rather than cast-perfect smoothness. The pair of copper hoop earrings carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfBronzeHoopEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_bronze_hoop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6680", []),
		Noun = "earring",
		ShortDescription = "a pair of bronze hoop earrings",
		LongDescription = null,
		FullDescription = "A pair of bronze hoop earrings is small enough to depend on close detail, not weight. The visible face uses brown-gold bronze with rubbed high points, while the back and fastening are plainer. Small handling marks cross the pair of bronze hoop earrings unevenly, especially where it would brush cloth or skin. The pair of bronze hoop earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfSilverHoopEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_silver_hoop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6710", []),
		Noun = "earring",
		ShortDescription = "a pair of silver hoop earrings",
		LongDescription = null,
		FullDescription = "There is a deliberate economy to the piece: little material, but careful placement. Pale silver brightened at its raised surfaces stands out in the face of the ornament, with simpler work hidden behind it. Close inspection of the pair of silver hoop earrings shows small tool marks rather than cast-perfect smoothness. The pair of silver hoop earrings carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfGoldHoopEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_gold_hoop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6740", []),
		Noun = "earring",
		ShortDescription = "a pair of gold hoop earrings",
		LongDescription = null,
		FullDescription = "A pair of gold hoop earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. The worked parts of the pair of gold hoop earrings are decorative, but none suggest a moving catch or concealed compartment. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 11.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfPearlDropEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_pearl_drop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6770", []),
		Noun = "earring",
		ShortDescription = "a pair of pearl drop earrings",
		LongDescription = null,
		FullDescription = "A pair of pearl drop earrings is small enough to depend on close detail, not weight. The visible face uses small pearls with uneven but gentle lustre, while the back and fastening are plainer. Small handling marks cross the pair of pearl drop earrings unevenly, especially where it would brush cloth or skin. The pair of pearl drop earrings leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 9.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfCoralDropEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_coral_drop_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6800", []),
		Noun = "earring",
		ShortDescription = "a pair of coral drop earrings",
		LongDescription = null,
		FullDescription = "A pair of coral drop earrings is small enough to depend on close detail, not weight. The visible face uses coral beads with a warm sea-worn surface, while the back and fastening are plainer. Small handling marks cross the pair of coral drop earrings unevenly, especially where it would brush cloth or skin. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfGlassBeadEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_glass_bead_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6830", []),
		Noun = "earring",
		ShortDescription = "a pair of glass bead earrings",
		LongDescription = null,
		FullDescription = "A pair of glass bead earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the pair of glass bead earrings shows small tool marks rather than cast-perfect smoothness. The pair of glass bead earrings reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfAmberBeadEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_amber_bead_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6860", []),
		Noun = "earring",
		ShortDescription = "a pair of amber bead earrings",
		LongDescription = null,
		FullDescription = "There is a deliberate economy to the piece: little material, but careful placement. Honey-coloured amber with cloudy threads caught inside stands out in the face of the ornament, with simpler work hidden behind it. Close inspection of the pair of amber bead earrings shows small tool marks rather than cast-perfect smoothness. The pair of amber bead earrings feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "amber",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfTurquoiseEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_turquoise_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6890", []),
		Noun = "earring",
		ShortDescription = "a pair of turquoise earrings",
		LongDescription = null,
		FullDescription = "The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Blue-green turquoise with fine darker lines gives it colour and finish without making it look bulky. Rubbing has brightened the exposed parts of the pair of turquoise earrings without making it look neglected. The pair of turquoise earrings feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "turquoise",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfGarnetEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_garnet_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6920", []),
		Noun = "earring",
		ShortDescription = "a pair of garnet earrings",
		LongDescription = null,
		FullDescription = "The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Dark red stones that brighten at their cut edges gives it colour and finish without making it look bulky. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of garnet earrings leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 8.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "garnet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfSilverWireEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_silver_wire_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6950", []),
		Noun = "earring",
		ShortDescription = "a pair of silver wire earrings",
		LongDescription = null,
		FullDescription = "A pair of silver wire earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of silver wire earrings feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 7.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
