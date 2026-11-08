#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfWireTempleRings() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_wire_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10001", []),
		Noun = "ring",
		ShortDescription = "a pair of wire temple rings",
		LongDescription = null,
		FullDescription = "A pair of wire temple rings would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as warm reddish copper darkened in the recesses. The hidden side of the pair of wire temple rings is plainer, so attention stays on small side loops arranged to frame the face rather than hang low and the visible finish. The pair of wire temple rings leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Commoner Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfPearlTempleRings() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_pearl_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10031", []),
		Noun = "ring",
		ShortDescription = "a pair of pearl temple rings",
		LongDescription = null,
		FullDescription = "A pair of pearl temple rings would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as small pearls with uneven but gentle lustre. Fine wear gathers around the ring's highest places, leaving small side loops arranged to frame the face rather than hang low easier to pick out. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 34.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfGiltTempleRings() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_gilt_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10061", []),
		Noun = "ring",
		ShortDescription = "a pair of gilt temple rings",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Bronze showing through a thin golden surface at nicks and bends gives the visible portion its colour, and the working end is kept clean of fussy decoration. Close inspection of the pair of gilt temple rings shows small tool marks rather than cast-perfect smoothness. The pair of gilt temple rings leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 44.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded bronze",
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

	public static CatalogueItemDefinition MedievalJewelleryHeadPairOfBeadHungTempleRings() => new()
	{
		Metadata = new("medieval_jewellery_head_pair_of_bead_hung_temple_rings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10091", []),
		Noun = "ring",
		ShortDescription = "a pair of bead-hung temple rings",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Coloured glass polished smooth enough to glow at the edges gives the visible portion its colour, and the working end is kept clean of fussy decoration. Small handling marks cross the pair of bead-hung temple rings unevenly, especially where it would brush cloth or skin. The pair of bead-hung temple rings is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 46.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPlainBrassCirclet() => new()
	{
		Metadata = new("medieval_jewellery_head_plain_brass_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10121", []),
		Noun = "circlet",
		ShortDescription = "a plain brass circlet",
		LongDescription = null,
		FullDescription = "A plain brass circlet is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The hidden side of the plain brass circlet is plainer, so attention stays on a stronger front line and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadSilverCirclet() => new()
	{
		Metadata = new("medieval_jewellery_head_silver_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10151", []),
		Noun = "circlet",
		ShortDescription = "a silver circlet",
		LongDescription = null,
		FullDescription = "The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. A stronger front line breaks the line before it runs away into plain side pieces. Rubbing has brightened the exposed parts of the silver circlet without making it look neglected. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 75.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPearlCirclet() => new()
	{
		Metadata = new("medieval_jewellery_head_pearl_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10181", []),
		Noun = "circlet",
		ShortDescription = "a pearl circlet",
		LongDescription = null,
		FullDescription = "A pearl circlet is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The worked parts of the pearl circlet are decorative, but none suggest a moving catch or concealed compartment. The pearl circlet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 62.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Circlet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadGoldCirclet() => new()
	{
		Metadata = new("medieval_jewellery_head_gold_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10211", []),
		Noun = "circlet",
		ShortDescription = "a gold circlet",
		LongDescription = null,
		FullDescription = "A gold circlet is shaped to frame the head without covering the face. The front carries the strongest detail in deep yellow gold with a warm, steady shine, while the sides are softened for wear with hair, cap, or veil. Small handling marks cross the gold circlet unevenly, especially where it would brush cloth or skin. The gold circlet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 80.0,
		Cost = 620.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadGarnetSetDiadem() => new()
	{
		Metadata = new("medieval_jewellery_head_garnet_set_diadem", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10241", []),
		Noun = "diadem",
		ShortDescription = "a garnet-set diadem",
		LongDescription = null,
		FullDescription = "A garnet-set diadem is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The hidden side of the garnet-set diadem is plainer, so attention stays on visible garnet detail worked into the public face and the visible finish. The garnet-set diadem feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 95.0,
		Cost = 820.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Diadems",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Diadem",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadEnamelledDiadem() => new()
	{
		Metadata = new("medieval_jewellery_head_enamelled_diadem", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10271", []),
		Noun = "diadem",
		ShortDescription = "an enamelled diadem",
		LongDescription = null,
		FullDescription = "An enamelled diadem is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The worked parts of the enamelled diadem are decorative, but none suggest a moving catch or concealed compartment. The enamelled diadem is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 80.0,
		Cost = 480.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "enamel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Diadems",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Diadem",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadSilverGiltCoronet() => new()
	{
		Metadata = new("medieval_jewellery_head_silver_gilt_coronet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10301", []),
		Noun = "coronet",
		ShortDescription = "a silver-gilt coronet",
		LongDescription = null,
		FullDescription = "The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. A stronger front line breaks the line before it runs away into plain side pieces. The worked parts of the silver-gilt coronet are decorative, but none suggest a moving catch or concealed compartment. The silver-gilt coronet feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 140.0,
		Cost = 900.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Coronets",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Coronet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadSmallGoldCoronet() => new()
	{
		Metadata = new("medieval_jewellery_head_small_gold_coronet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10331", []),
		Noun = "coronet",
		ShortDescription = "a small gold coronet",
		LongDescription = null,
		FullDescription = "The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. A stronger front line breaks the line before it runs away into plain side pieces. Rubbing has brightened the exposed parts of the small gold coronet without making it look neglected. The small gold coronet reads as regalia adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 150.0,
		Cost = 1400.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Coronets",
				"Market / Jewellery / Regalia"
			],
		Components = [
				"Holdable",
				"Wear_Coronet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadCourtCrown() => new()
	{
		Metadata = new("medieval_jewellery_head_court_crown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10361", []),
		Noun = "crown",
		ShortDescription = "a court crown",
		LongDescription = null,
		FullDescription = "Worn high, this piece would alter the outline of the head more than it would weigh it down. Its material shows as deep yellow gold with a warm, steady shine, with plainer work where it would be hidden by hair or cloth. Close inspection of the court crown shows small tool marks rather than cast-perfect smoothness. The court crown leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 450.0,
		Cost = 3500.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Crowns",
				"Market / Jewellery / Regalia"
			],
		Components = [
				"Holdable",
				"Wear_Crown",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadJadeForeheadOrnament() => new()
	{
		Metadata = new("medieval_jewellery_head_jade_forehead_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10391", []),
		Noun = "ornament",
		ShortDescription = "a jade forehead ornament",
		LongDescription = null,
		FullDescription = "A jade forehead ornament is shaped to frame the head without covering the face. The front carries the strongest detail in green jade smoothed to a cool waxy finish, while the sides are softened for wear with hair, cap, or veil. Fine wear gathers around the ornament's highest places, leaving a stronger front line easier to pick out. The jade forehead ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadGoldForeheadPendant() => new()
	{
		Metadata = new("medieval_jewellery_head_gold_forehead_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10421", []),
		Noun = "ornament",
		ShortDescription = "a gold forehead pendant",
		LongDescription = null,
		FullDescription = "A gold forehead pendant is shaped to frame the head without covering the face. The front carries the strongest detail in deep yellow gold with a warm, steady shine, while the sides are softened for wear with hair, cap, or veil. Close inspection of the gold forehead pendant shows small tool marks rather than cast-perfect smoothness. The gold forehead pendant is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 35.0,
		Cost = 320.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadPearlForeheadOrnament() => new()
	{
		Metadata = new("medieval_jewellery_head_pearl_forehead_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10451", []),
		Noun = "ornament",
		ShortDescription = "a pearl forehead ornament",
		LongDescription = null,
		FullDescription = "A pearl forehead ornament is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The worked parts of the pearl forehead ornament are decorative, but none suggest a moving catch or concealed compartment. The pearl forehead ornament carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 30.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadSilkRibbonChaplet() => new()
	{
		Metadata = new("medieval_jewellery_head_silk_ribbon_chaplet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10481", []),
		Noun = "chaplet",
		ShortDescription = "a silk ribbon chaplet",
		LongDescription = null,
		FullDescription = "A silk ribbon chaplet is gathered from silk cord with a soft light across its twist, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The worked parts of the silk ribbon chaplet are decorative, but none suggest a moving catch or concealed compartment. The silk ribbon chaplet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadFreshFlowerChaplet() => new()
	{
		Metadata = new("medieval_jewellery_head_fresh_flower_chaplet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10511", []),
		Noun = "chaplet",
		ShortDescription = "a fresh flower chaplet",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from fresh flowers still full in the petals and green at the stems rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. Rubbing has brightened the exposed parts of the fresh flower chaplet without making it look neglected. The piece would signal festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fresh flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadWovenStrawChaplet() => new()
	{
		Metadata = new("medieval_jewellery_head_woven_straw_chaplet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10541", []),
		Noun = "chaplet",
		ShortDescription = "a woven straw chaplet",
		LongDescription = null,
		FullDescription = "A woven straw chaplet has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. Close inspection of the woven straw chaplet shows small tool marks rather than cast-perfect smoothness. The woven straw chaplet feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "straw",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadLaurelFestivalWreath() => new()
	{
		Metadata = new("medieval_jewellery_head_laurel_festival_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10571", []),
		Noun = "wreath",
		ShortDescription = "a laurel festival wreath",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from laurel leaves laid in a clean repeating line rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The worked parts of the laurel festival wreath are decorative, but none suggest a moving catch or concealed compartment. The laurel festival wreath reads as festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "laurel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadIvyHeadWreath() => new()
	{
		Metadata = new("medieval_jewellery_head_ivy_head_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10601", []),
		Noun = "wreath",
		ShortDescription = "an ivy head wreath",
		LongDescription = null,
		FullDescription = "An ivy head wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The worked parts of the ivy head wreath are decorative, but none suggest a moving catch or concealed compartment. The ivy head wreath leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivy",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadFreshBlossomHeadGarland() => new()
	{
		Metadata = new("medieval_jewellery_head_fresh_blossom_head_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10631", []),
		Noun = "garland",
		ShortDescription = "a fresh blossom head garland",
		LongDescription = null,
		FullDescription = "A fresh blossom head garland has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The hidden side of the fresh blossom head garland is plainer, so attention stays on a flexible line made to settle into hair or a veil and the visible finish. The fresh blossom head garland feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "blossom",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadJasmineHeadGarland() => new()
	{
		Metadata = new("medieval_jewellery_head_jasmine_head_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10661", []),
		Noun = "garland",
		ShortDescription = "a jasmine head garland",
		LongDescription = null,
		FullDescription = "A jasmine head garland is gathered from jasmine flowers strung in small pale stars, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. Close inspection of the jasmine head garland shows small tool marks rather than cast-perfect smoothness. The jasmine head garland reads as festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jasmine",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadMarigoldHeadGarland() => new()
	{
		Metadata = new("medieval_jewellery_head_marigold_head_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10691", []),
		Noun = "garland",
		ShortDescription = "a marigold head garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from marigold heads crowded in warm ruffled knots rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. Small handling marks cross the marigold head garland unevenly, especially where it would brush cloth or skin. The marigold head garland feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marigold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadFlowerHeadGarland() => new()
	{
		Metadata = new("medieval_jewellery_head_flower_head_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10721", []),
		Noun = "garland",
		ShortDescription = "a $flower head garland",
		LongDescription = null,
		FullDescription = "A $flower head garland is gathered from fresh flowers still full in the petals and green at the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. Small handling marks cross the $flower head garland unevenly, especially where it would brush cloth or skin. The $flower head garland feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fresh flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing",
				"Variable_Flower"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryHeadDriedFlowerWreath() => new()
	{
		Metadata = new("medieval_jewellery_head_dried_flower_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10752", []),
		Noun = "wreath",
		ShortDescription = "a dried flower wreath",
		LongDescription = null,
		FullDescription = "A dried flower wreath is gathered from dried flowers papery at the edges and brittle along the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. Rubbing has brightened the exposed parts of the dried flower wreath without making it look neglected. The dried flower wreath reads as temporary festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistBronzeWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_bronze_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10782", []),
		Noun = "chain",
		ShortDescription = "a bronze waist chain",
		LongDescription = null,
		FullDescription = "A bronze waist chain reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Rubbing has brightened the exposed parts of the bronze waist chain without making it look neglected. The bronze waist chain carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistSilverWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_silver_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10812", []),
		Noun = "chain",
		ShortDescription = "a silver waist chain",
		LongDescription = null,
		FullDescription = "A silver waist chain reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the chain's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The silver waist chain reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistGoldWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_gold_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10842", []),
		Noun = "chain",
		ShortDescription = "a gold waist chain",
		LongDescription = null,
		FullDescription = "A gold waist chain reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the chain's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The piece would signal noble taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 110.0,
		Cost = 620.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistBrassMerchantWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_brass_merchant_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10872", []),
		Noun = "chain",
		ShortDescription = "a brass merchant waist chain",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. The hidden side of the brass merchant waist chain is plainer, so attention stays on small plates or links spaced to move against the garment and the visible finish. The brass merchant waist chain is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistCoralHungWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_coral_hung_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10902", []),
		Noun = "chain",
		ShortDescription = "a coral-hung waist chain",
		LongDescription = null,
		FullDescription = "The object has the careful rhythm of a worn display piece. Pale silver brightened at its raised surfaces catches most strongly on the raised or dangling parts, while the back remains workmanlike. Small handling marks cross the coral-hung waist chain unevenly, especially where it would brush cloth or skin. The coral-hung waist chain leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 105.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistBellLessSilverGirdleChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_bell_less_silver_girdle_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10932", []),
		Noun = "chain",
		ShortDescription = "a bell-less silver girdle chain",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Close inspection of the bell-less silver girdle chain shows small tool marks rather than cast-perfect smoothness. The bell-less silver girdle chain feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 100.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistFinishBronzeWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_finish_bronze_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10962", []),
		Noun = "chain",
		ShortDescription = "a $finish bronze waist chain",
		LongDescription = null,
		FullDescription = "A $finish bronze waist chain is meant to sit at the belt line, where movement would make small details catch the eye. Brown-gold bronze with rubbed high points forms the visible parts, with the chosen finish left deliberately visible on the worked surface interrupting the line. Small handling marks cross the $finish bronze waist chain unevenly, especially where it would brush cloth or skin. The $finish bronze waist chain carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
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

	public static CatalogueItemDefinition MedievalJewelleryWaistSilkTiedWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_waist_silk_tied_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:10993", []),
		Noun = "chain",
		ShortDescription = "a silk-tied waist chain",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Close inspection of the silk-tied waist chain shows small tool marks rather than cast-perfect smoothness. The silk-tied waist chain is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistSilverGirdleOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_silver_girdle_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11023", []),
		Noun = "ornament",
		ShortDescription = "a silver girdle ornament",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Rubbing has brightened the exposed parts of the silver girdle ornament without making it look neglected. The silver girdle ornament leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 45.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistBrassGirdleOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_brass_girdle_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11053", []),
		Noun = "ornament",
		ShortDescription = "a brass girdle ornament",
		LongDescription = null,
		FullDescription = "A brass girdle ornament reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Small handling marks cross the brass girdle ornament unevenly, especially where it would brush cloth or skin. The brass girdle ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistGoldGirdlePendant() => new()
	{
		Metadata = new("medieval_jewellery_waist_gold_girdle_pendant", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11083", []),
		Noun = "ornament",
		ShortDescription = "a gold girdle pendant",
		LongDescription = null,
		FullDescription = "A gold girdle pendant is meant to sit at the belt line, where movement would make small details catch the eye. Deep yellow gold with a warm, steady shine forms the visible parts, with small plates or links spaced to move against the garment interrupting the line. Rubbing has brightened the exposed parts of the gold girdle pendant without making it look neglected. The piece would signal noble taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 52.0,
		Cost = 350.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistEnamelGirdleMount() => new()
	{
		Metadata = new("medieval_jewellery_waist_enamel_girdle_mount", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11113", []),
		Noun = "ornament",
		ShortDescription = "an enamel girdle mount",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Rubbing has brightened the exposed parts of the enamel girdle mount without making it look neglected. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 42.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "enamel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistCoralGirdleOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_coral_girdle_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11143", []),
		Noun = "ornament",
		ShortDescription = "a coral girdle ornament",
		LongDescription = null,
		FullDescription = "A coral girdle ornament reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the ornament's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The coral girdle ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 36.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistPewterGirdleTag() => new()
	{
		Metadata = new("medieval_jewellery_waist_pewter_girdle_tag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11173", []),
		Noun = "ornament",
		ShortDescription = "a pewter girdle tag",
		LongDescription = null,
		FullDescription = "The object has the careful rhythm of a worn display piece. Soft grey pewter with a low satin sheen catches most strongly on the raised or dangling parts, while the back remains workmanlike. Close inspection of the pewter girdle tag shows small tool marks rather than cast-perfect smoothness. The piece would signal commoner taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 36.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistMotifGirdleOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_motif_girdle_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11203", []),
		Noun = "ornament",
		ShortDescription = "a $motif girdle ornament",
		LongDescription = null,
		FullDescription = "The object has the careful rhythm of a worn display piece. Pale silver brightened at its raised surfaces catches most strongly on the raised or dangling parts, while the back remains workmanlike. The worked parts of the $motif girdle ornament are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 44.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
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

	public static CatalogueItemDefinition MedievalJewelleryWaistSilverGiltGirdleJewel() => new()
	{
		Metadata = new("medieval_jewellery_waist_silver_gilt_girdle_jewel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11234", []),
		Noun = "ornament",
		ShortDescription = "a silver-gilt girdle jewel",
		LongDescription = null,
		FullDescription = "The object has the careful rhythm of a worn display piece. Gold-washed silver, brighter on the raised places and paler at worn edges catches most strongly on the raised or dangling parts, while the back remains workmanlike. The hidden side of the silver-gilt girdle jewel is plainer, so attention stays on small plates or links spaced to move against the garment and the visible finish. The silver-gilt girdle jewel carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 54.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistBronzeBeltOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_bronze_belt_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11264", []),
		Noun = "ornament",
		ShortDescription = "a bronze belt ornament",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Rubbing has brightened the exposed parts of the bronze belt ornament without making it look neglected. The bronze belt ornament reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 32.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistBrassBeltJewel() => new()
	{
		Metadata = new("medieval_jewellery_waist_brass_belt_jewel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11294", []),
		Noun = "ornament",
		ShortDescription = "a brass belt jewel",
		LongDescription = null,
		FullDescription = "A brass belt jewel reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Rubbing has brightened the exposed parts of the brass belt jewel without making it look neglected. The brass belt jewel leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 30.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistSilverBeltOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_silver_belt_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11324", []),
		Noun = "ornament",
		ShortDescription = "a silver belt ornament",
		LongDescription = null,
		FullDescription = "A silver belt ornament reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Close inspection of the silver belt ornament shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistGildedCopperBeltMount() => new()
	{
		Metadata = new("medieval_jewellery_waist_gilded_copper_belt_mount", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11354", []),
		Noun = "ornament",
		ShortDescription = "a gilded copper belt mount",
		LongDescription = null,
		FullDescription = "The object has the careful rhythm of a worn display piece. Coppery red metal visible beneath worn gilt edges catches most strongly on the raised or dangling parts, while the back remains workmanlike. Fine wear gathers around the ornament's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The gilded copper belt mount feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistShapeBeltOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_shape_belt_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11384", []),
		Noun = "ornament",
		ShortDescription = "a $shape belt ornament",
		LongDescription = null,
		FullDescription = "A $shape belt ornament is meant to sit at the belt line, where movement would make small details catch the eye. Gold-washed silver, brighter on the raised places and paler at worn edges forms the visible parts, with the chosen shape giving the object its main outline interrupting the line. The hidden side of the $shape belt ornament is plainer, so attention stays on the chosen shape giving the object its main outline and the visible finish. The $shape belt ornament reads as courtly adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 36.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Ornament",
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

	public static CatalogueItemDefinition MedievalJewelleryWaistSetOfBronzeBeltPlaques() => new()
	{
		Metadata = new("medieval_jewellery_waist_set_of_bronze_belt_plaques", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11415", []),
		Noun = "plaque",
		ShortDescription = "a set of bronze belt plaques",
		LongDescription = null,
		FullDescription = "The object has the careful rhythm of a worn display piece. Brown-gold bronze with rubbed high points catches most strongly on the raised or dangling parts, while the back remains workmanlike. Close inspection of the set of bronze belt plaques shows small tool marks rather than cast-perfect smoothness. The set of bronze belt plaques feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistSetOfSilverBeltPlaques() => new()
	{
		Metadata = new("medieval_jewellery_waist_set_of_silver_belt_plaques", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11445", []),
		Noun = "plaque",
		ShortDescription = "a set of silver belt plaques",
		LongDescription = null,
		FullDescription = "A set of silver belt plaques reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. The hidden side of the set of silver belt plaques is plainer, so attention stays on small plates or links spaced to move against the garment and the visible finish. The set of silver belt plaques carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 120.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistSetOfGildedBeltPlaques() => new()
	{
		Metadata = new("medieval_jewellery_waist_set_of_gilded_belt_plaques", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11475", []),
		Noun = "plaque",
		ShortDescription = "a set of gilded belt plaques",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Small handling marks cross the set of gilded belt plaques unevenly, especially where it would brush cloth or skin. The set of gilded belt plaques feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 125.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistSteppeBeltPlaqueSet() => new()
	{
		Metadata = new("medieval_jewellery_waist_steppe_belt_plaque_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11505", []),
		Noun = "plaque",
		ShortDescription = "a steppe belt plaque set",
		LongDescription = null,
		FullDescription = "The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Close inspection of the steppe belt plaque set shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 280.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryWaistLeatherWaistOrnament() => new()
	{
		Metadata = new("medieval_jewellery_waist_leather_waist_ornament", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11535", []),
		Noun = "ornament",
		ShortDescription = "a leather waist ornament",
		LongDescription = null,
		FullDescription = "A leather waist ornament is meant to sit at the belt line, where movement would make small details catch the eye. Worked leather with creases at the folds forms the visible parts, with small plates or links spaced to move against the garment interrupting the line. The worked parts of the leather waist ornament are decorative, but none suggest a moving catch or concealed compartment. The leather waist ornament reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Ornaments",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Ornament",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEliteJewelledGoldCourtCollar() => new()
	{
		Metadata = new("medieval_jewellery_elite_jewelled_gold_court_collar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11565", []),
		Noun = "collar",
		ShortDescription = "a jewelled gold court collar",
		LongDescription = null,
		FullDescription = "A jewelled gold court collar carries its value in the rhythm of its material: deep yellow gold with a warm, steady shine. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The hidden side of the jewelled gold court collar is plainer, so attention stays on craft details crowded into the most visible face of the piece and the visible finish. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 320.0,
		Cost = 2400.0m,
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

	public static CatalogueItemDefinition MedievalJewelleryElitePearlAndGoldCollar() => new()
	{
		Metadata = new("medieval_jewellery_elite_pearl_and_gold_collar", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11595", []),
		Noun = "collar",
		ShortDescription = "a pearl and gold collar",
		LongDescription = null,
		FullDescription = "Seen flat, pearl and gold collar shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Fine wear gathers around the collar's highest places, leaving visible pearl detail worked into the public face easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 260.0,
		Cost = 1900.0m,
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

	public static CatalogueItemDefinition MedievalJewelleryEliteGemstoneCirclet() => new()
	{
		Metadata = new("medieval_jewellery_elite_gemstone_circlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11625", []),
		Noun = "circlet",
		ShortDescription = "a gemstone circlet",
		LongDescription = null,
		FullDescription = "The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. Craft details crowded into the most visible face of the piece breaks the line before it runs away into plain side pieces. The worked parts of the gemstone circlet are decorative, but none suggest a moving catch or concealed compartment. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 120.0,
		Cost = 1800.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Regalia"
			],
		Components = [
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEliteSapphireDiadem() => new()
	{
		Metadata = new("medieval_jewellery_elite_sapphire_diadem", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11655", []),
		Noun = "diadem",
		ShortDescription = "a sapphire diadem",
		LongDescription = null,
		FullDescription = "Worn high, this piece would alter the outline of the head more than it would weigh it down. Its material shows as deep yellow gold with a warm, steady shine, with plainer work where it would be hidden by hair or cloth. Fine wear gathers around the diadem's highest places, leaving visible sapphire detail worked into the public face easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 110.0,
		Cost = 2200.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Diadems",
				"Market / Jewellery / Regalia"
			],
		Components = [
				"Holdable",
				"Wear_Diadem",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEliteRubyCoronet() => new()
	{
		Metadata = new("medieval_jewellery_elite_ruby_coronet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11685", []),
		Noun = "coronet",
		ShortDescription = "a ruby coronet",
		LongDescription = null,
		FullDescription = "A ruby coronet is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The hidden side of the ruby coronet is plainer, so attention stays on visible ruby detail worked into the public face and the visible finish. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 170.0,
		Cost = 2600.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Coronets",
				"Market / Jewellery / Regalia"
			],
		Components = [
				"Holdable",
				"Wear_Coronet",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEliteGreatPearlCourtChain() => new()
	{
		Metadata = new("medieval_jewellery_elite_great_pearl_court_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11715", []),
		Noun = "chain",
		ShortDescription = "a great pearl court chain",
		LongDescription = null,
		FullDescription = "The great pearl court chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is small pearls with uneven but gentle lustre, with craft details crowded into the most visible face of the piece giving the eye a place to pause. Small handling marks cross the great pearl court chain unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 210.0,
		Cost = 1600.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Court Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEliteSilverGiltNobleBrooch() => new()
	{
		Metadata = new("medieval_jewellery_elite_silver_gilt_noble_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11745", []),
		Noun = "brooch",
		ShortDescription = "a silver-gilt noble brooch",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. Craft details crowded into the most visible face of the piece gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the silver-gilt noble brooch unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 95.0,
		Cost = 520.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
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

	public static CatalogueItemDefinition MedievalJewelleryEliteGarnetCloisonLikeBrooch() => new()
	{
		Metadata = new("medieval_jewellery_elite_garnet_cloison_like_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11775", []),
		Noun = "brooch",
		ShortDescription = "a garnet cloison-like brooch",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. Visible garnet detail worked into the public face gives the face definition, while the reverse remains plain and workmanlike. Close inspection of the garnet cloison-like brooch shows small tool marks rather than cast-perfect smoothness. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 88.0,
		Cost = 900.0m,
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

	public static CatalogueItemDefinition MedievalJewelleryEliteLapisCourtBadge() => new()
	{
		Metadata = new("medieval_jewellery_elite_lapis_court_badge", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11805", []),
		Noun = "badge",
		ShortDescription = "a lapis court badge",
		LongDescription = null,
		FullDescription = "This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. Visible lapis detail worked into the public face gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the lapis court badge unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 70.0,
		Cost = 620.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Court Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEliteGoldTempleRingSet() => new()
	{
		Metadata = new("medieval_jewellery_elite_gold_temple_ring_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11835", []),
		Noun = "ring",
		ShortDescription = "a gold temple-ring set",
		LongDescription = null,
		FullDescription = "A gold temple-ring set would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as deep yellow gold with a warm, steady shine. Rubbing has brightened the exposed parts of the gold temple-ring set without making it look neglected. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 80.0,
		Cost = 740.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEliteJadeCourtHairpinSet() => new()
	{
		Metadata = new("medieval_jewellery_elite_jade_court_hairpin_set", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11865", []),
		Noun = "hairpin",
		ShortDescription = "a jade court hairpin set",
		LongDescription = null,
		FullDescription = "A jade court hairpin set would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as green jade smoothed to a cool waxy finish. The worked parts of the jade court hairpin set are decorative, but none suggest a moving catch or concealed compartment. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 48.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Court Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEliteTortoiseshellCourtComb() => new()
	{
		Metadata = new("medieval_jewellery_elite_tortoiseshell_court_comb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11895", []),
		Noun = "comb",
		ShortDescription = "a tortoiseshell court comb",
		LongDescription = null,
		FullDescription = "There is a quiet asymmetry to the ornament, useful for placing it by touch. Tortoiseshell mottled between honey, brown, and smoke-dark patches gives the visible portion its colour, and the working end is kept clean of fussy decoration. Fine wear gathers around the comb's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 32.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "tortoiseshell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Court Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryEliteHeavySilverTorc() => new()
	{
		Metadata = new("medieval_jewellery_elite_heavy_silver_torc", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11925", []),
		Noun = "torc",
		ShortDescription = "a heavy silver torc",
		LongDescription = null,
		FullDescription = "The heavy silver torc falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with craft details crowded into the most visible face of the piece giving the eye a place to pause. Fine wear gathers around the torc's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 320.0,
		Cost = 520.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
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

	public static CatalogueItemDefinition MedievalJewelleryEliteEnamelledGoldBracelet() => new()
	{
		Metadata = new("medieval_jewellery_elite_enamelled_gold_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11955", []),
		Noun = "bracelet",
		ShortDescription = "an enamelled gold bracelet",
		LongDescription = null,
		FullDescription = "An enamelled gold bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Small handling marks cross the enamelled gold bracelet unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 85.0,
		Cost = 650.0m,
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

	public static CatalogueItemDefinition MedievalJewelleryElitePearlForeheadChain() => new()
	{
		Metadata = new("medieval_jewellery_elite_pearl_forehead_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11985", []),
		Noun = "ornament",
		ShortDescription = "a pearl forehead chain",
		LongDescription = null,
		FullDescription = "Worn high, this piece would alter the outline of the head more than it would weigh it down. Its material shows as small pearls with uneven but gentle lustre, with plainer work where it would be hidden by hair or cloth. Close inspection of the pearl forehead chain shows small tool marks rather than cast-perfect smoothness. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 44.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEliteNobleGoldWaistChain() => new()
	{
		Metadata = new("medieval_jewellery_elite_noble_gold_waist_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12015", []),
		Noun = "chain",
		ShortDescription = "a noble gold waist chain",
		LongDescription = null,
		FullDescription = "The object has the careful rhythm of a worn display piece. Deep yellow gold with a warm, steady shine catches most strongly on the raised or dangling parts, while the back remains workmanlike. Fine wear gathers around the chain's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 180.0,
		Cost = 1300.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEliteCourtGirdleJewel() => new()
	{
		Metadata = new("medieval_jewellery_elite_court_girdle_jewel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12045", []),
		Noun = "ornament",
		ShortDescription = "a court girdle jewel",
		LongDescription = null,
		FullDescription = "A court girdle jewel reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the ornament's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 72.0,
		Cost = 720.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryEliteRoyalSealSignetRing() => new()
	{
		Metadata = new("medieval_jewellery_elite_royal_seal_signet_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12075", []),
		Noun = "ring",
		ShortDescription = "a royal seal signet ring",
		LongDescription = null,
		FullDescription = "A royal seal signet ring is a compact band of deep yellow gold with a warm, steady shine, made with craft details crowded into the most visible face of the piece rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Close inspection of the royal seal signet ring shows small tool marks rather than cast-perfect smoothness. It reads as court or treasury jewellery rather than a market trinket.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 26.0,
		Cost = 1100.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Regalia"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_NobleSignetRing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingReligiousAlmsChest() => new()
	{
		Metadata = new("medieval_locking_religious_alms_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12106", []),
		Noun = "chest",
		ShortDescription = "a heavy alms chest",
		LongDescription = null,
		FullDescription = "This heavy alms chest is a large, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 34000.0,
		Cost = 280.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Large locking chest for alms, gifts, and institutional funds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingReligiousDonationBox() => new()
	{
		Metadata = new("medieval_locking_religious_donation_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12136", []),
		Noun = "box",
		ShortDescription = "a locking donation box",
		LongDescription = null,
		FullDescription = "This locking donation box is a small, workmanlike box built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 80.0m,
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
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Built-in-lock donation box for coins or offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingReligiousReliquaryLockbox() => new()
	{
		Metadata = new("medieval_locking_religious_reliquary_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12166", []),
		Noun = "lockbox",
		ShortDescription = "a locked reliquary box",
		LongDescription = null,
		FullDescription = "This locked reliquary box is a small, well-made lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
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
		BuilderNotes = "Small built-in-lock box for relics, seals, or sacred keepsakes as stored contents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingReligiousScriptureChest() => new()
	{
		Metadata = new("medieval_locking_religious_scripture_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12196", []),
		Noun = "chest",
		ShortDescription = "a locking scripture chest",
		LongDescription = null,
		FullDescription = "This locking scripture chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 28000.0,
		Cost = 260.0m,
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
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking chest for books, scrolls, tablets, or wrapped texts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingReligiousTreasuryChest() => new()
	{
		Metadata = new("medieval_locking_religious_treasury_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12226", []),
		Noun = "chest",
		ShortDescription = "a sanctuary treasury chest",
		LongDescription = null,
		FullDescription = "This sanctuary treasury chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 68000.0,
		Cost = 720.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy safe-chest profile for a religious treasury or secure sacristy room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousAngledScriptureLectern() => new()
	{
		Metadata = new("medieval_religious_angled_scripture_lectern", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12256", []),
		Noun = "lectern",
		ShortDescription = "an angled scripture lectern",
		LongDescription = null,
		FullDescription = "This angled scripture lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 8800.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sloped reading surface for large sacred books, scrolls, tablets, or sermon notes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousBronzeIncenseBurner() => new()
	{
		Metadata = new("medieval_religious_bronze_incense_burner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12290", []),
		Noun = "burner",
		ShortDescription = "a bronze incense burner",
		LongDescription = null,
		FullDescription = "This bronze incense burner is a small, workmanlike burner worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small solid-fuel burner for charcoal and incense; not a dry container while using brazier behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousBronzeOfferingTray() => new()
	{
		Metadata = new("medieval_religious_bronze_offering_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12322", []),
		Noun = "tray",
		ShortDescription = "a bronze offering tray",
		LongDescription = null,
		FullDescription = "This bronze offering tray is a medium-sized, workmanlike tray worked from bronze. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flat tray for ritual food portions, flowers, incense packets, tapers, or carried offerings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousGlassRelicCase() => new()
	{
		Metadata = new("medieval_religious_glass_relic_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12356", []),
		Noun = "case",
		ShortDescription = "a glass relic case",
		LongDescription = null,
		FullDescription = "This glass relic case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"Container_Display_Case"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Transparent display case for reliquaries, memorial objects, sealed offerings, or other protected sacred pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousHangingCenser() => new()
	{
		Metadata = new("medieval_religious_hanging_censer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12388", []),
		Noun = "censer",
		ShortDescription = "a hanging bronze censer",
		LongDescription = null,
		FullDescription = "This hanging bronze censer is a small, well-made censer worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1250.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Suspended censer for incense smoke, modelled as a small solid-fuel burner rather than as jewellery or a personal charm.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousHangingOilLamp() => new()
	{
		Metadata = new("medieval_religious_hanging_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12420", []),
		Noun = "lamp",
		ShortDescription = "a hanging oil lamp",
		LongDescription = null,
		FullDescription = "This hanging oil lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Suspended liquid-fuel lamp for a sanctuary, shrine, or religious hall.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousIncenseBox() => new()
	{
		Metadata = new("medieval_religious_incense_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12451", []),
		Noun = "box",
		ShortDescription = "a cedar incense box",
		LongDescription = null,
		FullDescription = "This cedar incense box is a small, workmanlike box built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Lidded dry container for incense chips, resins, powdered aromatics, or fragrant woods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousIncenseCabinet() => new()
	{
		Metadata = new("medieval_religious_incense_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12483", []),
		Noun = "cabinet",
		ShortDescription = "an incense-store cabinet",
		LongDescription = null,
		FullDescription = "This incense-store cabinet is a medium-sized, well-made cabinet built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 7600.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small cabinet for incense, tapers, ritual oil flasks, and other aromatic service supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousLampTray() => new()
	{
		Metadata = new("medieval_religious_lamp_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12515", []),
		Noun = "tray",
		ShortDescription = "a bronze lamp tray",
		LongDescription = null,
		FullDescription = "This bronze lamp tray is a medium-sized, workmanlike tray worked from bronze. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Its formal proportions give it a public, ritual, and institutional presence. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Tray for arranging oil lamps, candle cups, wicks, and lighting supplies; inert unless separate lamps are placed on it.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousLongAltarCandle() => new()
	{
		Metadata = new("medieval_religious_long_altar_candle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12548", []),
		Noun = "candle",
		ShortDescription = "a long altar candle",
		LongDescription = null,
		FullDescription = "This long altar candle is a small, well-made candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Candles"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Longer beeswax candle suited to altar, shrine, memorial, or festival lighting.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousLongCongregationBench() => new()
	{
		Metadata = new("medieval_religious_long_congregation_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12579", []),
		Noun = "bench",
		ShortDescription = "a long congregation bench",
		LongDescription = null,
		FullDescription = "This long congregation bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 24000.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long religious-hall bench with ordinary bench seating and a surface that can accept small placed objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousLowOfferingStand() => new()
	{
		Metadata = new("medieval_religious_low_offering_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12613", []),
		Noun = "stand",
		ShortDescription = "a low offering stand",
		LongDescription = null,
		FullDescription = "This low offering stand is a medium-sized, workmanlike stand built from beech boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beech",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Temple Offerings"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Low surface for offerings, lamps, incense bowls, small icons, tablets, or ritual vessels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousLustralWaterBucket() => new()
	{
		Metadata = new("medieval_religious_lustral_water_bucket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12646", []),
		Noun = "bucket",
		ShortDescription = "a lustral water bucket",
		LongDescription = null,
		FullDescription = "This lustral water bucket is a medium-sized, well-made bucket worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2800.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Handled liquid vessel for sprinkled, poured, or distributed sacred water.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousOakOfferingTable() => new()
	{
		Metadata = new("medieval_religious_oak_offering_table", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12678", []),
		Noun = "table",
		ShortDescription = "an oak offering table",
		LongDescription = null,
		FullDescription = "This oak offering table is a large, workmanlike table built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18000.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Temple Offerings"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "General offering or service table for religious halls, chapels, shrines, and household ritual rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousOfferingBowl() => new()
	{
		Metadata = new("medieval_religious_offering_bowl", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12712", []),
		Noun = "bowl",
		ShortDescription = "a bronze offering bowl",
		LongDescription = null,
		FullDescription = "This bronze offering bowl is a small, workmanlike bowl worked from bronze. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Plate"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open bowl for grain, flowers, ashes, small offerings, or other solid ritual goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousOilCruet() => new()
	{
		Metadata = new("medieval_religious_oil_cruet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12746", []),
		Noun = "cruet",
		ShortDescription = "a small ritual oil cruet",
		LongDescription = null,
		FullDescription = "This small ritual oil cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small liquid container for lamp oil, chrism-like oil, perfumed oil, or anointing liquid.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousOpenDevotionalShelves() => new()
	{
		Metadata = new("medieval_religious_open_devotional_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12778", []),
		Noun = "shelves",
		ShortDescription = "open devotional shelves",
		LongDescription = null,
		FullDescription = "These open devotional shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14500.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open shelves for religious books, lamps, offering bowls, memorial tablets, or displayed sacred objects.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousPaintedScreenPanel() => new()
	{
		Metadata = new("medieval_religious_painted_screen_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12811", []),
		Noun = "screen",
		ShortDescription = "a painted shrine screen",
		LongDescription = null,
		FullDescription = "This painted shrine screen is a large, workmanlike screen built from pine boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable screen panel for marking off a shrine, altar side, vestry corner, or sacred display area.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousPlainOilLamp() => new()
	{
		Metadata = new("medieval_religious_plain_oil_lamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12841", []),
		Noun = "lamp",
		ShortDescription = "a plain ritual oil lamp",
		LongDescription = null,
		FullDescription = "This plain ritual oil lamp is a small, workmanlike lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Liquid-fuel lamp for altars, shrines, chapels, and religious household rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousPlainPrayerBench() => new()
	{
		Metadata = new("medieval_religious_plain_prayer_bench", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12872", []),
		Noun = "bench",
		ShortDescription = "a plain prayer bench",
		LongDescription = null,
		FullDescription = "This plain prayer bench is a medium-sized, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5500.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Compact kneeling or seated prayer bench; a furnishing rather than a personal devotional object.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousRitualSupplyCupboard() => new()
	{
		Metadata = new("medieval_religious_ritual_supply_cupboard", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12903", []),
		Noun = "cupboard",
		ShortDescription = "a ritual supply cupboard",
		LongDescription = null,
		FullDescription = "This ritual supply cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18500.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "General cupboard for candles, cloths, incense, oil flasks, basins, and other communal religious supplies.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousRitualWashingBasin() => new()
	{
		Metadata = new("medieval_religious_ritual_washing_basin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12935", []),
		Noun = "basin",
		ShortDescription = "a ritual washing basin",
		LongDescription = null,
		FullDescription = "This ritual washing basin is a medium-sized, workmanlike basin formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Water-holding basin for ablution, lustration, or ceremonial washing.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousScriptureCabinet() => new()
	{
		Metadata = new("medieval_religious_scripture_cabinet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:12967", []),
		Noun = "cabinet",
		ShortDescription = "a scripture cabinet",
		LongDescription = null,
		FullDescription = "This scripture cabinet is a large, well-made cabinet built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 22000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cabinet for sacred books, scroll cases, tablets, commentaries, or ritual documents.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousSimpleReadingStand() => new()
	{
		Metadata = new("medieval_religious_simple_reading_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13000", []),
		Noun = "stand",
		ShortDescription = "a simple reading stand",
		LongDescription = null,
		FullDescription = "This simple reading stand is a medium-sized, workmanlike stand built from pine boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3400.0,
		Cost = 15.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Portable sloped stand for a book, scroll, scripture bundle, or ritual text.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalReligiousStandingCenser() => new()
	{
		Metadata = new("medieval_religious_standing_censer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:13034", []),
		Noun = "censer",
		ShortDescription = "a standing iron censer",
		LongDescription = null,
		FullDescription = "This standing iron censer is a medium-sized, workmanlike censer worked from wrought iron. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Freestanding incense burner for shrines, halls, porches, or side chapels.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
