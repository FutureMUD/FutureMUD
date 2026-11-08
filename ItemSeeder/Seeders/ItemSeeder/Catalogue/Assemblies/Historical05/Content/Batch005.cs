#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalJewelleryEarPairOfGildedBronzeEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_gilded_bronze_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:6980", []),
		Noun = "earring",
		ShortDescription = "a pair of gilded bronze earrings",
		LongDescription = null,
		FullDescription = "A pair of gilded bronze earrings is small enough to depend on close detail, not weight. The visible face uses bronze showing through a thin golden surface at nicks and bends, while the back and fastening are plainer. The worked parts of the pair of gilded bronze earrings are decorative, but none suggest a moving catch or concealed compartment. The pair of gilded bronze earrings leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded bronze",
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

	public static CatalogueItemDefinition MedievalJewelleryEarSingleBronzeEarring() => new()
	{
		Metadata = new("medieval_jewellery_ear_single_bronze_earring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7010", []),
		Noun = "earring",
		ShortDescription = "a single bronze earring",
		LongDescription = null,
		FullDescription = "A single bronze earring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the single bronze earring shows small tool marks rather than cast-perfect smoothness. The single bronze earring reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarSingleSilverEarring() => new()
	{
		Metadata = new("medieval_jewellery_ear_single_silver_earring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7040", []),
		Noun = "earring",
		ShortDescription = "a single silver earring",
		LongDescription = null,
		FullDescription = "A single silver earring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the single silver earring shows small tool marks rather than cast-perfect smoothness. The single silver earring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Earring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarSingleGoldEarring() => new()
	{
		Metadata = new("medieval_jewellery_ear_single_gold_earring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7070", []),
		Noun = "earring",
		ShortDescription = "a single gold earring",
		LongDescription = null,
		FullDescription = "A single gold earring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The single gold earring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 5.0,
		Cost = 95.0m,
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
				"Wear_Earring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfConchShellEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_conch_shell_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7100", []),
		Noun = "earring",
		ShortDescription = "a pair of conch shell earrings",
		LongDescription = null,
		FullDescription = "There is a deliberate economy to the piece: little material, but careful placement. Cut conch shell with creamy curves and warmer pinkish edges stands out in the face of the ornament, with simpler work hidden behind it. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of conch shell earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "conch shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfCowrieEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_cowrie_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7130", []),
		Noun = "earring",
		ShortDescription = "a pair of cowrie earrings",
		LongDescription = null,
		FullDescription = "A pair of cowrie earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of cowrie earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cowrie shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Commoner Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfMotherOfPearlEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_mother_of_pearl_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7160", []),
		Noun = "earring",
		ShortDescription = "a pair of mother-of-pearl earrings",
		LongDescription = null,
		FullDescription = "The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Mother-of-pearl flashing softly under the surface gives it colour and finish without making it look bulky. The worked parts of the pair of mother-of-pearl earrings are decorative, but none suggest a moving catch or concealed compartment. The pair of mother-of-pearl earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 8.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mother-of-pearl",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfJadeEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_jade_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7190", []),
		Noun = "earring",
		ShortDescription = "a pair of jade earrings",
		LongDescription = null,
		FullDescription = "A pair of jade earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Rubbing has brightened the exposed parts of the pair of jade earrings without making it look neglected. The pair of jade earrings carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfLapisBeadEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_lapis_bead_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7220", []),
		Noun = "earring",
		ShortDescription = "a pair of lapis bead earrings",
		LongDescription = null,
		FullDescription = "There is a deliberate economy to the piece: little material, but careful placement. Deep blue stone flecked with paler specks stands out in the face of the ornament, with simpler work hidden behind it. Close inspection of the pair of lapis bead earrings shows small tool marks rather than cast-perfect smoothness. The pair of lapis bead earrings leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 9.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lapis lazuli",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfCarneliearrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_carneliearrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7250", []),
		Noun = "earring",
		ShortDescription = "a pair of carnelian earrings",
		LongDescription = null,
		FullDescription = "A pair of carnelian earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Rubbing has brightened the exposed parts of the pair of carnelian earrings without making it look neglected. The pair of carnelian earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 9.0,
		Cost = 65.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carnelian",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfRockCrystalEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_rock_crystal_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7280", []),
		Noun = "earring",
		ShortDescription = "a pair of rock crystal earrings",
		LongDescription = null,
		FullDescription = "A pair of rock crystal earrings is small enough to depend on close detail, not weight. The visible face uses clear rock crystal with tiny inner veils, while the back and fastening are plainer. The hidden side of the pair of rock crystal earrings is plainer, so attention stays on a small hanging point polished more brightly than the rest and the visible finish. The pair of rock crystal earrings carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 8.0,
		Cost = 100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "rock crystal",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfEnamelledEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_enamelled_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7310", []),
		Noun = "earring",
		ShortDescription = "a pair of enamelled earrings",
		LongDescription = null,
		FullDescription = "The ornament is light in the hand, with visible enamel detail worked into the public face worked into the part most likely to catch the eye. Enamel gives it colour and finish without making it look bulky. The hidden side of the pair of enamelled earrings is plainer, so attention stays on visible enamel detail worked into the public face and the visible finish. The pair of enamelled earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "enamel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfShapeEarrings() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_shape_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7340", []),
		Noun = "earring",
		ShortDescription = "a pair of $shape earrings",
		LongDescription = null,
		FullDescription = "There is a deliberate economy to the piece: little material, but careful placement. Pale silver brightened at its raised surfaces stands out in the face of the ornament, with simpler work hidden behind it. Rubbing has brightened the exposed parts of the pair of $shape earrings without making it look neglected. The pair of $shape earrings leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 78.0m,
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
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarSmallGoldNoseRing() => new()
	{
		Metadata = new("medieval_jewellery_ear_small_gold_nose_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7371", []),
		Noun = "nose ring",
		ShortDescription = "a small gold nose ring",
		LongDescription = null,
		FullDescription = "A small gold nose ring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the nose ring's highest places, leaving faint tool chatter along the inner face easier to pick out. The small gold nose ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 4.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarSilverNoseRing() => new()
	{
		Metadata = new("medieval_jewellery_ear_silver_nose_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7402", []),
		Noun = "nose ring",
		ShortDescription = "a silver nose ring",
		LongDescription = null,
		FullDescription = "A silver nose ring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Rubbing has brightened the exposed parts of the silver nose ring without making it look neglected. The silver nose ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarPearlNoseStud() => new()
	{
		Metadata = new("medieval_jewellery_ear_pearl_nose_stud", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7433", []),
		Noun = "stud",
		ShortDescription = "a pearl nose stud",
		LongDescription = null,
		FullDescription = "A pearl nose stud has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. The hidden side of the pearl nose stud is plainer, so attention stays on a softened seam where the maker closed it and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 3.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarTinyTurquoiseNoseStud() => new()
	{
		Metadata = new("medieval_jewellery_ear_tiny_turquoise_nose_stud", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7464", []),
		Noun = "stud",
		ShortDescription = "a tiny turquoise nose stud",
		LongDescription = null,
		FullDescription = "A tiny turquoise nose stud is small enough to depend on close detail, not weight. The visible face uses blue-green turquoise with fine darker lines, while the back and fastening are plainer. Close inspection of the tiny turquoise nose stud shows small tool marks rather than cast-perfect smoothness. The tiny turquoise nose stud carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 3.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "turquoise",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarCopperNoseRing() => new()
	{
		Metadata = new("medieval_jewellery_ear_copper_nose_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7495", []),
		Noun = "nose ring",
		ShortDescription = "a copper nose ring",
		LongDescription = null,
		FullDescription = "There is a deliberate economy to the piece: little material, but careful placement. Warm reddish copper darkened in the recesses stands out in the face of the ornament, with simpler work hidden behind it. The worked parts of the copper nose ring are decorative, but none suggest a moving catch or concealed compartment. The copper nose ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarFlowerShapedNoseStud() => new()
	{
		Metadata = new("medieval_jewellery_ear_flower_shaped_nose_stud", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7526", []),
		Noun = "stud",
		ShortDescription = "a flower-shaped nose stud",
		LongDescription = null,
		FullDescription = "There is a deliberate economy to the piece: little material, but careful placement. Deep yellow gold with a warm, steady shine stands out in the face of the ornament, with simpler work hidden behind it. The worked parts of the flower-shaped nose stud are decorative, but none suggest a moving catch or concealed compartment. The flower-shaped nose stud is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 3.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEarMotifSilverNoseRing() => new()
	{
		Metadata = new("medieval_jewellery_ear_motif_silver_nose_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7557", []),
		Noun = "nose ring",
		ShortDescription = "a $motif silver nose ring",
		LongDescription = null,
		FullDescription = "A $motif silver nose ring is small enough to depend on close detail, not weight. The visible face uses pale silver brightened at its raised surfaces, while the back and fastening are plainer. Rubbing has brightened the exposed parts of the $motif silver nose ring without making it look neglected. The $motif silver nose ring reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Nose_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfSilverEarStuds() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_silver_ear_studs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7589", []),
		Noun = "stud",
		ShortDescription = "a pair of silver ear studs",
		LongDescription = null,
		FullDescription = "The ornament is light in the hand, with a softened seam where the maker closed it worked into the part most likely to catch the eye. Pale silver brightened at its raised surfaces gives it colour and finish without making it look bulky. Small handling marks cross the pair of silver ear studs unevenly, especially where it would brush cloth or skin. The pair of silver ear studs is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 5.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Functions / Worn Items / Jewellery / Piercings / Ear Studs",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfGlassEarStuds() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_glass_ear_studs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7620", []),
		Noun = "stud",
		ShortDescription = "a pair of glass ear studs",
		LongDescription = null,
		FullDescription = "A pair of glass ear studs has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. The worked parts of the pair of glass ear studs are decorative, but none suggest a moving catch or concealed compartment. The pair of glass ear studs leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Functions / Worn Items / Jewellery / Piercings / Ear Studs",
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

	public static CatalogueItemDefinition MedievalJewelleryEarPairOfGoldEarStuds() => new()
	{
		Metadata = new("medieval_jewellery_ear_pair_of_gold_ear_studs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7651", []),
		Noun = "stud",
		ShortDescription = "a pair of gold ear studs",
		LongDescription = null,
		FullDescription = "A pair of gold ear studs has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the pair of gold ear studs shows small tool marks rather than cast-perfect smoothness. The pair of gold ear studs leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 5.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Functions / Worn Items / Jewellery / Piercings / Ear Studs",
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

	public static CatalogueItemDefinition MedievalJewelleryBroochPlainBronzeRingBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_plain_bronze_ring_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7682", []),
		Noun = "brooch",
		ShortDescription = "a plain bronze ring brooch",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Rubbing has brightened the exposed parts of the plain bronze ring brooch without making it look neglected. The plain bronze ring brooch feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochSilverAnnularBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_silver_annular_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7712", []),
		Noun = "brooch",
		ShortDescription = "a silver annular brooch",
		LongDescription = null,
		FullDescription = "A silver annular brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. The hidden side of the silver annular brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The silver annular brooch feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 38.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochGildedCopperDiscBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_gilded_copper_disc_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7742", []),
		Noun = "brooch",
		ShortDescription = "a gilded copper disc brooch",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. The hidden side of the gilded copper disc brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 48.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochGarnetColouredBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_garnet_coloured_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7772", []),
		Noun = "brooch",
		ShortDescription = "a garnet-coloured brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked deep yellow gold with a warm, steady shine, leaving the display surface clean. Small handling marks cross the garnet-coloured brooch unevenly, especially where it would brush cloth or skin. The garnet-coloured brooch reads as noble adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 46.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochGlassInlaidBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_glass_inlaid_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7802", []),
		Noun = "brooch",
		ShortDescription = "a glass-inlaid brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked brown-gold bronze with rubbed high points, leaving the display surface clean. The hidden side of the glass-inlaid brooch is plainer, so attention stays on visible glass detail worked into the public face and the visible finish. The glass-inlaid brooch reads as merchant adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 50.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochPenannularSilverBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_penannular_silver_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7832", []),
		Noun = "brooch",
		ShortDescription = "a penannular silver brooch",
		LongDescription = null,
		FullDescription = "A penannular silver brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. Small handling marks cross the penannular silver brooch unevenly, especially where it would brush cloth or skin. The penannular silver brooch carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochHeavyCloakBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_heavy_cloak_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7862", []),
		Noun = "brooch",
		ShortDescription = "a heavy cloak brooch",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. The hidden side of the heavy cloak brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The heavy cloak brooch feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochPairedOvalBroochSet() => new()
	{
		Metadata = new("medieval_jewellery_brooch_paired_oval_brooch_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7892", []),
		Noun = "brooch",
		ShortDescription = "a paired oval brooch set",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The paired oval brooch set carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooches",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochPairedSilverBroochSet() => new()
	{
		Metadata = new("medieval_jewellery_brooch_paired_silver_brooch_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7922", []),
		Noun = "brooch",
		ShortDescription = "a paired silver brooch set",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked pale silver brightened at its raised surfaces, leaving the display surface clean. Small handling marks cross the paired silver brooch set unevenly, especially where it would brush cloth or skin. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 130.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooches",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochSmallPewterBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_small_pewter_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7952", []),
		Noun = "brooch",
		ShortDescription = "a small pewter brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked soft grey pewter with a low satin sheen, leaving the display surface clean. The hidden side of the small pewter brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The small pewter brooch reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochBrassRingBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_brass_ring_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:7982", []),
		Noun = "brooch",
		ShortDescription = "a brass ring brooch",
		LongDescription = null,
		FullDescription = "A brass ring brooch has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with a firm rim and decorated face set where it will show against cloth. The hidden side of the brass ring brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The brass ring brooch reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochEnamelDiscBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_enamel_disc_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8012", []),
		Noun = "brooch",
		ShortDescription = "an enamel disc brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked enamel, leaving the display surface clean. Close inspection of the enamel disc brooch shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 44.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "enamel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochNielloInlaidSilverBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_niello_inlaid_silver_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8042", []),
		Noun = "brooch",
		ShortDescription = "a niello-inlaid silver brooch",
		LongDescription = null,
		FullDescription = "A niello-inlaid silver brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with visible niello detail worked into the public face set where it will show against cloth. The worked parts of the niello-inlaid silver brooch are decorative, but none suggest a moving catch or concealed compartment. The niello-inlaid silver brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 46.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochJetMourningBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_jet_mourning_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8072", []),
		Noun = "brooch",
		ShortDescription = "a jet mourning brooch",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The jet mourning brooch leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochAmberFacedBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_amber_faced_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8102", []),
		Noun = "brooch",
		ShortDescription = "an amber-faced brooch",
		LongDescription = null,
		FullDescription = "An amber-faced brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the amber-faced brooch without making it look neglected. The amber-faced brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 48.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochCoralSetBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_coral_set_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8132", []),
		Noun = "brooch",
		ShortDescription = "a coral-set brooch",
		LongDescription = null,
		FullDescription = "A coral-set brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with visible coral detail worked into the public face set where it will show against cloth. The hidden side of the coral-set brooch is plainer, so attention stays on visible coral detail worked into the public face and the visible finish. The coral-set brooch carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 44.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochLapisSetCourtBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_lapis_set_court_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8162", []),
		Noun = "brooch",
		ShortDescription = "a lapis-set court brooch",
		LongDescription = null,
		FullDescription = "A lapis-set court brooch has a public face and a practical back, the two sides plainly made for different jobs. Deep yellow gold with a warm, steady shine forms the visible body, with visible lapis detail worked into the public face set where it will show against cloth. The hidden side of the lapis-set court brooch is plainer, so attention stays on visible lapis detail worked into the public face and the visible finish. The lapis-set court brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 50.0,
		Cost = 520.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochRockCrystalBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_rock_crystal_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8192", []),
		Noun = "brooch",
		ShortDescription = "a rock crystal brooch",
		LongDescription = null,
		FullDescription = "A rock crystal brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. Fine wear gathers around the brooch's highest places, leaving visible rock crystal detail worked into the public face easier to pick out. The rock crystal brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 42.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochSimpleBoneBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_simple_bone_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8222", []),
		Noun = "brooch",
		ShortDescription = "a simple bone brooch",
		LongDescription = null,
		FullDescription = "A simple bone brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale bone burnished by handling forms the visible body, with a firm rim and decorated face set where it will show against cloth. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The simple bone brooch leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochHornCloakBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_horn_cloak_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8252", []),
		Noun = "brooch",
		ShortDescription = "a horn cloak brooch",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the horn cloak brooch unevenly, especially where it would brush cloth or skin. The horn cloak brooch carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochShellFacedBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_shell_faced_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8282", []),
		Noun = "brooch",
		ShortDescription = "a shell-faced brooch",
		LongDescription = null,
		FullDescription = "A shell-faced brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale shell showing irregular growth lines forms the visible body, with a firm rim and decorated face set where it will show against cloth. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The shell-faced brooch carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochMotherOfPearlBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_mother_of_pearl_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8312", []),
		Noun = "brooch",
		ShortDescription = "a mother-of-pearl brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked mother-of-pearl flashing softly under the surface, leaving the display surface clean. Close inspection of the mother-of-pearl brooch shows small tool marks rather than cast-perfect smoothness. The mother-of-pearl brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mother-of-pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochMotifBronzeBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_motif_bronze_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8342", []),
		Noun = "brooch",
		ShortDescription = "a $motif bronze brooch",
		LongDescription = null,
		FullDescription = "A $motif bronze brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Close inspection of the $motif bronze brooch shows small tool marks rather than cast-perfect smoothness. The $motif bronze brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
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

	public static CatalogueItemDefinition MedievalJewelleryBroochShapeSilverBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_shape_silver_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8373", []),
		Noun = "brooch",
		ShortDescription = "a $shape silver brooch",
		LongDescription = null,
		FullDescription = "A $shape silver brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the $shape silver brooch without making it look neglected. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 42.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochInlayGoldBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_inlay_gold_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8404", []),
		Noun = "brooch",
		ShortDescription = "a $inlay gold brooch",
		LongDescription = null,
		FullDescription = "A $inlay gold brooch has a public face and a practical back, the two sides plainly made for different jobs. Deep yellow gold with a warm, steady shine forms the visible body, with the chosen inlay style running through the visible lines set where it will show against cloth. The hidden side of the $inlay gold brooch is plainer, so attention stays on the chosen inlay style running through the visible lines and the visible finish. The $inlay gold brooch leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 46.0,
		Cost = 560.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal",
				"Variable_InlayStyle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochFinishBrassBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_finish_brass_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8435", []),
		Noun = "brooch",
		ShortDescription = "a $finish brass brooch",
		LongDescription = null,
		FullDescription = "A $finish brass brooch has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with the chosen finish left deliberately visible on the worked surface set where it will show against cloth. Close inspection of the $finish brass brooch shows small tool marks rather than cast-perfect smoothness. The $finish brass brooch feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
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

	public static CatalogueItemDefinition MedievalJewelleryBroochTrefoilLikeBronzeBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_trefoil_like_bronze_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8466", []),
		Noun = "brooch",
		ShortDescription = "a trefoil-like bronze brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked brown-gold bronze with rubbed high points, leaving the display surface clean. Small handling marks cross the trefoil-like bronze brooch unevenly, especially where it would brush cloth or skin. The trefoil-like bronze brooch feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochBroadSilverCloakBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_broad_silver_cloak_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8496", []),
		Noun = "brooch",
		ShortDescription = "a broad silver cloak brooch",
		LongDescription = null,
		FullDescription = "A broad silver cloak brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with a firm rim and decorated face set where it will show against cloth. The hidden side of the broad silver cloak brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The broad silver cloak brooch feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 80.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochLittleCopperDressBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_little_copper_dress_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8526", []),
		Noun = "brooch",
		ShortDescription = "a little copper dress brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked warm reddish copper darkened in the recesses, leaving the display surface clean. Small handling marks cross the little copper dress brooch unevenly, especially where it would brush cloth or skin. The little copper dress brooch carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBroochFineGoldCourtBrooch() => new()
	{
		Metadata = new("medieval_jewellery_brooch_fine_gold_court_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8556", []),
		Noun = "brooch",
		ShortDescription = "a fine gold court brooch",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked deep yellow gold with a warm, steady shine, leaving the display surface clean. Close inspection of the fine gold court brooch shows small tool marks rather than cast-perfect smoothness. The fine gold court brooch reads as courtly adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 52.0,
		Cost = 760.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinBronzeCloakPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_bronze_cloak_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8586", []),
		Noun = "pin",
		ShortDescription = "a bronze cloak pin",
		LongDescription = null,
		FullDescription = "A bronze cloak pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Fine wear gathers around the pin's highest places, leaving a shaft kept straighter than the decorated head easier to pick out. The bronze cloak pin is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinSilverCloakPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_silver_cloak_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8616", []),
		Noun = "pin",
		ShortDescription = "a silver cloak pin",
		LongDescription = null,
		FullDescription = "A silver cloak pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. The worked parts of the silver cloak pin are decorative, but none suggest a moving catch or concealed compartment. The silver cloak pin leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinIronDressPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_iron_dress_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8646", []),
		Noun = "pin",
		ShortDescription = "an iron dress pin",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked dark wrought iron with a waxed, workmanlike finish, leaving the display surface clean. The hidden side of the iron dress pin is plainer, so attention stays on a shaft kept straighter than the decorated head and the visible finish. The iron dress pin reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinBoneGarmentPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_bone_garment_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8676", []),
		Noun = "pin",
		ShortDescription = "a bone garment pin",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. The hidden side of the bone garment pin is plainer, so attention stays on a shaft kept straighter than the decorated head and the visible finish. The bone garment pin feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinBrassRingPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_brass_ring_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8706", []),
		Noun = "pin",
		ShortDescription = "a brass ring pin",
		LongDescription = null,
		FullDescription = "A brass ring pin has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with a shaft kept straighter than the decorated head set where it will show against cloth. The worked parts of the brass ring pin are decorative, but none suggest a moving catch or concealed compartment. The brass ring pin is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinGoldHeadedPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_gold_headed_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8736", []),
		Noun = "pin",
		ShortDescription = "a gold-headed pin",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. The worked parts of the gold-headed pin are decorative, but none suggest a moving catch or concealed compartment. The gold-headed pin carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinPearlHeadedPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_pearl_headed_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8766", []),
		Noun = "pin",
		ShortDescription = "a pearl-headed pin",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked small pearls with uneven but gentle lustre, leaving the display surface clean. The worked parts of the pearl-headed pin are decorative, but none suggest a moving catch or concealed compartment. The pearl-headed pin leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinGlassHeadedPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_glass_headed_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8796", []),
		Noun = "pin",
		ShortDescription = "a glass-headed pin",
		LongDescription = null,
		FullDescription = "The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked coloured glass polished smooth enough to glow at the edges, leaving the display surface clean. Rubbing has brightened the exposed parts of the glass-headed pin without making it look neglected. The glass-headed pin reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinRockCrystalPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_rock_crystal_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8826", []),
		Noun = "pin",
		ShortDescription = "a rock crystal pin",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. The worked parts of the rock crystal pin are decorative, but none suggest a moving catch or concealed compartment. The rock crystal pin carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 105.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "rock crystal",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinCarvedHornPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_carved_horn_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8856", []),
		Noun = "pin",
		ShortDescription = "a carved horn pin",
		LongDescription = null,
		FullDescription = "A carved horn pin has a public face and a practical back, the two sides plainly made for different jobs. Horn with layered translucent streaks through it forms the visible body, with a shaft kept straighter than the decorated head set where it will show against cloth. Small handling marks cross the carved horn pin unevenly, especially where it would brush cloth or skin. The carved horn pin reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinMotifSilverPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_motif_silver_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8886", []),
		Noun = "pin",
		ShortDescription = "a $motif silver pin",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. The selected motif set into the outward-facing surface gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the $motif silver pin unevenly, especially where it would brush cloth or skin. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 22.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
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

	public static CatalogueItemDefinition MedievalJewelleryPinFinishBronzePin() => new()
	{
		Metadata = new("medieval_jewellery_pin_finish_bronze_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8917", []),
		Noun = "pin",
		ShortDescription = "a $finish bronze pin",
		LongDescription = null,
		FullDescription = "A $finish bronze pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the $finish bronze pin without making it look neglected. The $finish bronze pin reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
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

	public static CatalogueItemDefinition MedievalJewelleryPinLongBrassVeilPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_long_brass_veil_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8948", []),
		Noun = "pin",
		ShortDescription = "a long brass veil pin",
		LongDescription = null,
		FullDescription = "A long brass veil pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as yellow brass polished brighter on the edges, with handling marks gathered near the fastening points. The worked parts of the long brass veil pin are decorative, but none suggest a moving catch or concealed compartment. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinNarrowIvoryHairPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_narrow_ivory_hair_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:8978", []),
		Noun = "pin",
		ShortDescription = "a narrow ivory hair pin",
		LongDescription = null,
		FullDescription = "A narrow ivory hair pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as close-grained ivory with a creamy, old polish, with handling marks gathered near the fastening points. The hidden side of the narrow ivory hair pin is plainer, so attention stays on a shaft kept straighter than the decorated head and the visible finish. The narrow ivory hair pin leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinCopperFasteningPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_copper_fastening_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9008", []),
		Noun = "pin",
		ShortDescription = "a copper fastening pin",
		LongDescription = null,
		FullDescription = "A copper fastening pin has a public face and a practical back, the two sides plainly made for different jobs. Warm reddish copper darkened in the recesses forms the visible body, with a shaft kept straighter than the decorated head set where it will show against cloth. Rubbing has brightened the exposed parts of the copper fastening pin without making it look neglected. The copper fastening pin reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryPinSilverGiltCourtPin() => new()
	{
		Metadata = new("medieval_jewellery_pin_silver_gilt_court_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9038", []),
		Noun = "pin",
		ShortDescription = "a silver-gilt court pin",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. Rubbing has brightened the exposed parts of the silver-gilt court pin without making it look neglected. The silver-gilt court pin feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 26.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeStampedBrassBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_stamped_brass_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9068", []),
		Noun = "badge",
		ShortDescription = "a stamped brass badge",
		LongDescription = null,
		FullDescription = "A stamped brass badge has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. Close inspection of the stamped brass badge shows small tool marks rather than cast-perfect smoothness. The piece would signal professional or guild taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Professional Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgePewterHouseholdBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_pewter_household_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9098", []),
		Noun = "badge",
		ShortDescription = "a pewter household badge",
		LongDescription = null,
		FullDescription = "A pewter household badge has a public face and a practical back, the two sides plainly made for different jobs. Soft grey pewter with a low satin sheen forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. Fine wear gathers around the badge's highest places, leaving a face broad enough to carry a clear visible device easier to pick out. The pewter household badge carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeSilverGuildBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_silver_guild_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9128", []),
		Noun = "badge",
		ShortDescription = "a silver guild badge",
		LongDescription = null,
		FullDescription = "A silver guild badge has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. The worked parts of the silver guild badge are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeCopperLoveBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_copper_love_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9158", []),
		Noun = "badge",
		ShortDescription = "a copper love badge",
		LongDescription = null,
		FullDescription = "A copper love badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as warm reddish copper darkened in the recesses, with handling marks gathered near the fastening points. Small handling marks cross the copper love badge unevenly, especially where it would brush cloth or skin. The copper love badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeEnamelCivicBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_enamel_civic_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9188", []),
		Noun = "badge",
		ShortDescription = "an enamel civic badge",
		LongDescription = null,
		FullDescription = "An enamel civic badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as enamel, with handling marks gathered near the fastening points. The hidden side of the enamel civic badge is plainer, so attention stays on a face broad enough to carry a clear visible device and the visible finish. The enamel civic badge carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 26.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "enamel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeGildedBronzeBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_gilded_bronze_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9218", []),
		Noun = "badge",
		ShortDescription = "a gilded bronze badge",
		LongDescription = null,
		FullDescription = "A gilded bronze badge has a public face and a practical back, the two sides plainly made for different jobs. Bronze showing through a thin golden surface at nicks and bends forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. The worked parts of the gilded bronze badge are decorative, but none suggest a moving catch or concealed compartment. The gilded bronze badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeSmallLeadTokenBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_small_lead_token_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9248", []),
		Noun = "badge",
		ShortDescription = "a small lead token badge",
		LongDescription = null,
		FullDescription = "A small lead token badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as lead, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the small lead token badge without making it look neglected. The piece would signal commoner taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeMotifCourtBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_motif_court_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9278", []),
		Noun = "badge",
		ShortDescription = "a $motif court badge",
		LongDescription = null,
		FullDescription = "A $motif court badge has a public face and a practical back, the two sides plainly made for different jobs. Gold-washed silver, brighter on the raised places and paler at worn edges forms the visible body, with the selected motif set into the outward-facing surface set where it will show against cloth. The hidden side of the $motif court badge is plainer, so attention stays on the selected motif set into the outward-facing surface and the visible finish. The piece would signal courtly taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 30.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
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

	public static CatalogueItemDefinition MedievalJewelleryBadgeShapeBrassBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_shape_brass_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9309", []),
		Noun = "badge",
		ShortDescription = "a $shape brass badge",
		LongDescription = null,
		FullDescription = "A $shape brass badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as yellow brass polished brighter on the edges, with handling marks gathered near the fastening points. Small handling marks cross the $shape brass badge unevenly, especially where it would brush cloth or skin. The piece would signal professional or guild taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Professional Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeMotherOfPearlBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_mother_of_pearl_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9340", []),
		Noun = "badge",
		ShortDescription = "a mother-of-pearl badge",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A face broad enough to carry a clear visible device gives the face definition, while the reverse remains plain and workmanlike. The worked parts of the mother-of-pearl badge are decorative, but none suggest a moving catch or concealed compartment. The mother-of-pearl badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mother-of-pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeNielloSilverBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_niello_silver_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9370", []),
		Noun = "badge",
		ShortDescription = "a niello silver badge",
		LongDescription = null,
		FullDescription = "A niello silver badge has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with visible niello detail worked into the public face set where it will show against cloth. The worked parts of the niello silver badge are decorative, but none suggest a moving catch or concealed compartment. The niello silver badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 26.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryBadgeGoldHouseholdBadge() => new()
	{
		Metadata = new("medieval_jewellery_badge_gold_household_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9400", []),
		Noun = "badge",
		ShortDescription = "a gold household badge",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A face broad enough to carry a clear visible device gives the face definition, while the reverse remains plain and workmanlike. Rubbing has brightened the exposed parts of the gold household badge without making it look neglected. The gold household badge reads as noble adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 30.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadBronzeHairpin() => new()
	{
		Metadata = new("medieval_jewellery_head_bronze_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9430", []),
		Noun = "hairpin",
		ShortDescription = "a bronze hairpin",
		LongDescription = null,
		FullDescription = "A bronze hairpin is made for the hair or the side of the head, where balance matters as much as ornament. Brown-gold bronze with rubbed high points is worked into the visible end, while the gripping part is simpler and more direct. The hidden side of the bronze hairpin is plainer, so attention stays on a narrow working end and a more decorative visible head and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadSilverHairpin() => new()
	{
		Metadata = new("medieval_jewellery_head_silver_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9460", []),
		Noun = "hairpin",
		ShortDescription = "a silver hairpin",
		LongDescription = null,
		FullDescription = "A silver hairpin is made for the hair or the side of the head, where balance matters as much as ornament. Pale silver brightened at its raised surfaces is worked into the visible end, while the gripping part is simpler and more direct. Rubbing has brightened the exposed parts of the silver hairpin without making it look neglected. The silver hairpin leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 16.0,
		Cost = 65.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadGoldHairpin() => new()
	{
		Metadata = new("medieval_jewellery_head_gold_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9490", []),
		Noun = "hairpin",
		ShortDescription = "a gold hairpin",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Deep yellow gold with a warm, steady shine gives the visible portion its colour, and the working end is kept clean of fussy decoration. The worked parts of the gold hairpin are decorative, but none suggest a moving catch or concealed compartment. The gold hairpin leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 16.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadJadeHairpin() => new()
	{
		Metadata = new("medieval_jewellery_head_jade_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9520", []),
		Noun = "hairpin",
		ShortDescription = "a jade hairpin",
		LongDescription = null,
		FullDescription = "The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Rubbing has brightened the exposed parts of the jade hairpin without making it look neglected. The jade hairpin reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadTortoiseshellHairpin() => new()
	{
		Metadata = new("medieval_jewellery_head_tortoiseshell_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9550", []),
		Noun = "hairpin",
		ShortDescription = "a tortoiseshell hairpin",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Tortoiseshell mottled between honey, brown, and smoke-dark patches gives the visible portion its colour, and the working end is kept clean of fussy decoration. Close inspection of the tortoiseshell hairpin shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "tortoiseshell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadBambooHairpin() => new()
	{
		Metadata = new("medieval_jewellery_head_bamboo_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9580", []),
		Noun = "hairpin",
		ShortDescription = "a bamboo hairpin",
		LongDescription = null,
		FullDescription = "A bamboo hairpin is made for the hair or the side of the head, where balance matters as much as ornament. Smooth bamboo with the joints left subtly visible is worked into the visible end, while the gripping part is simpler and more direct. The worked parts of the bamboo hairpin are decorative, but none suggest a moving catch or concealed compartment. The bamboo hairpin is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfSilverHairpins() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_silver_hairpins", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9610", []),
		Noun = "hairpin",
		ShortDescription = "a pair of silver hairpins",
		LongDescription = null,
		FullDescription = "The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Small handling marks cross the pair of silver hairpins unevenly, especially where it would brush cloth or skin. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 32.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpins",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfJadeHairpins() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_jade_hairpins", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9640", []),
		Noun = "hairpin",
		ShortDescription = "a pair of jade hairpins",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Green jade smoothed to a cool waxy finish gives the visible portion its colour, and the working end is kept clean of fussy decoration. The hidden side of the pair of jade hairpins is plainer, so attention stays on a narrow working end and a more decorative visible head and the visible finish. The pair of jade hairpins is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 36.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hairpins",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadBoxwoodHairComb() => new()
	{
		Metadata = new("medieval_jewellery_head_boxwood_hair_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9670", []),
		Noun = "comb",
		ShortDescription = "a boxwood hair comb",
		LongDescription = null,
		FullDescription = "The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Fine wear gathers around the comb's highest places, leaving a narrow working end and a more decorative visible head easier to pick out. The boxwood hair comb reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "boxwood",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadTortoiseshellHairComb() => new()
	{
		Metadata = new("medieval_jewellery_head_tortoiseshell_hair_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9700", []),
		Noun = "comb",
		ShortDescription = "a tortoiseshell hair comb",
		LongDescription = null,
		FullDescription = "A tortoiseshell hair comb would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as tortoiseshell mottled between honey, brown, and smoke-dark patches. The worked parts of the tortoiseshell hair comb are decorative, but none suggest a moving catch or concealed compartment. The tortoiseshell hair comb carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 22.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "tortoiseshell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadMotherOfPearlComb() => new()
	{
		Metadata = new("medieval_jewellery_head_mother_of_pearl_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9730", []),
		Noun = "comb",
		ShortDescription = "a mother-of-pearl comb",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Mother-of-pearl flashing softly under the surface gives the visible portion its colour, and the working end is kept clean of fussy decoration. Close inspection of the mother-of-pearl comb shows small tool marks rather than cast-perfect smoothness. The mother-of-pearl comb reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mother-of-pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfIvoryHairCombs() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_ivory_hair_combs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9760", []),
		Noun = "comb",
		ShortDescription = "a pair of ivory hair combs",
		LongDescription = null,
		FullDescription = "A pair of ivory hair combs is made for the hair or the side of the head, where balance matters as much as ornament. Close-grained ivory with a creamy, old polish is worked into the visible end, while the gripping part is simpler and more direct. Fine wear gathers around the comb's highest places, leaving a narrow working end and a more decorative visible head easier to pick out. The pair of ivory hair combs carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 42.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Combs",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadGoldHairOrnament() => new()
	{
		Metadata = new("medieval_jewellery_head_gold_hair_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9790", []),
		Noun = "ornament",
		ShortDescription = "a gold hair ornament",
		LongDescription = null,
		FullDescription = "The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Rubbing has brightened the exposed parts of the gold hair ornament without making it look neglected. The gold hair ornament reads as noble adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 320.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadSilverHairPlaque() => new()
	{
		Metadata = new("medieval_jewellery_head_silver_hair_plaque", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9820", []),
		Noun = "ornament",
		ShortDescription = "a silver hair plaque",
		LongDescription = null,
		FullDescription = "A silver hair plaque would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as pale silver brightened at its raised surfaces. Rubbing has brightened the exposed parts of the silver hair plaque without making it look neglected. The silver hair plaque feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 22.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadMotifHairOrnament() => new()
	{
		Metadata = new("medieval_jewellery_head_motif_hair_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9850", []),
		Noun = "ornament",
		ShortDescription = "a $motif hair ornament",
		LongDescription = null,
		FullDescription = "A $motif hair ornament would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as gold-washed silver, brighter on the raised places and paler at worn edges. Close inspection of the $motif hair ornament shows small tool marks rather than cast-perfect smoothness. The $motif hair ornament feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Ornament",
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

	public static CatalogueItemDefinition MedievalJewelleryHeadSilkTiedHairOrnament() => new()
	{
		Metadata = new("medieval_jewellery_head_silk_tied_hair_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9881", []),
		Noun = "ornament",
		ShortDescription = "a silk-tied hair ornament",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Silk cord with a soft light across its twist gives the visible portion its colour, and the working end is kept clean of fussy decoration. Rubbing has brightened the exposed parts of the silk-tied hair ornament without making it look neglected. The silk-tied hair ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Ornament",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadLacqueredBambooHairComb() => new()
	{
		Metadata = new("medieval_jewellery_head_lacquered_bamboo_hair_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9911", []),
		Noun = "comb",
		ShortDescription = "a lacquered bamboo hair comb",
		LongDescription = null,
		FullDescription = "The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Small handling marks cross the lacquered bamboo hair comb unevenly, especially where it would brush cloth or skin. The lacquered bamboo hair comb is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfBronzeTempleRings() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_bronze_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9941", []),
		Noun = "ring",
		ShortDescription = "a pair of bronze temple rings",
		LongDescription = null,
		FullDescription = "The piece narrows or curves where it must hold, then broadens into the part meant to be seen. Small side loops arranged to frame the face rather than hang low keeps the ornament from looking like an ordinary fastening. Rubbing has brightened the exposed parts of the pair of bronze temple rings without making it look neglected. The pair of bronze temple rings carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfSilverTempleRings() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_silver_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:9971", []),
		Noun = "ring",
		ShortDescription = "a pair of silver temple rings",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Pale silver brightened at its raised surfaces gives the visible portion its colour, and the working end is kept clean of fussy decoration. Fine wear gathers around the ring's highest places, leaving small side loops arranged to frame the face rather than hang low easier to pick out. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 40.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Temple_Rings",
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
