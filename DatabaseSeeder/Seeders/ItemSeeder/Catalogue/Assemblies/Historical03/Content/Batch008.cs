#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical03Stock
{
	public static CatalogueItemDefinition MedievalSteppeRitualFurCap() => new()
	{
		Metadata = new("medieval_steppe_ritual_fur_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9209", []),
		Noun = "cap",
		ShortDescription = "a fur-trimmed ritual cap",
		LongDescription = null,
		FullDescription = "This fur-trimmed ritual cap is a warm headpiece with a soft crown and a thick band around the brow. The fur gives the cap a rough, animal-textured edge, contrasting with the simpler cloth or felt of the crown. It is visually suited to cold open country and to a specialist set apart by distinctive winter headwear.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
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

	public static CatalogueItemDefinition MedievalSteppeRitualSash() => new()
	{
		Metadata = new("medieval_steppe_ritual_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9239", []),
		Noun = "sash",
		ShortDescription = "a braided wool ritual sash",
		LongDescription = null,
		FullDescription = "This braided wool ritual sash is a sturdy belt-like length of woven cloth with a visibly twisted structure. Its surface is rough and practical, with ends that hang plainly once tied. It adds a marked band across a robe or coat without claiming pockets, tools, or other unsupported functions.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 12.0m,
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
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalAnnularBronzeBrooch() => new()
	{
		Metadata = new("medieval_annular_bronze_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9269", []),
		Noun = "brooch",
		ShortDescription = "a bronze annular brooch",
		LongDescription = null,
		FullDescription = "This bronze annular brooch is shaped as a plain circular ring with a slim pin crossing the opening. The surface is modestly polished, with most of its detail in the clean curve and the slight flattening around the pin hinge. It is sized to sit visibly at a shoulder or chest over a cloak, mantle, or heavy outer layer.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPenannularBronzeBrooch() => new()
	{
		Metadata = new("medieval_penannular_bronze_brooch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9298", []),
		Noun = "brooch",
		ShortDescription = "a bronze penannular brooch",
		LongDescription = null,
		FullDescription = "This bronze penannular brooch forms an open ring with two slightly thickened terminals and a long movable pin. Fine filing marks show around the ends, while the broader front catches light when worn over wool. Its shape gives a clear cloak-fastening appearance without relying on any hidden mechanism.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPairOvalBrooches() => new()
	{
		Metadata = new("medieval_pair_oval_brooches", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9327", []),
		Noun = "brooches",
		ShortDescription = "a pair of bronze oval brooches",
		LongDescription = null,
		FullDescription = "These bronze oval brooches are made as a matched pair, each with a domed face and a narrow back pin. The surfaces carry shallow raised lines that break up the polished metal without making them overly ornate. They are visually suited to being worn symmetrically over a strap dress or front-hanging garment layer.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 110.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Wear_Shoulders",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPlainBronzeCloakPin() => new()
	{
		Metadata = new("medieval_plain_bronze_cloak_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9356", []),
		Noun = "pin",
		ShortDescription = "a plain bronze cloak pin",
		LongDescription = null,
		FullDescription = "This plain bronze cloak pin is a straight, practical fastening piece with a narrow shaft and slightly broader head. Its metal has a muted brown-gold sheen, with no decoration beyond the worked tip and smoothed edges. It reads as a humble outerwear fitting rather than a jewel or badge.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalIronCloakPin() => new()
	{
		Metadata = new("medieval_iron_cloak_pin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9385", []),
		Noun = "pin",
		ShortDescription = "an iron cloak pin",
		LongDescription = null,
		FullDescription = "This iron cloak pin is dark, narrow, and plainly forged, with a simple head and a tapering point. The metal is sturdy rather than elegant, showing faint hammering and a workmanlike finish. It suits rough wool, patched cloaks, and road clothing where visible utility matters more than display.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Wear_Shoulder",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBronzeMountedLeatherBelt() => new()
	{
		Metadata = new("medieval_bronze_mounted_leather_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9414", []),
		Noun = "belt",
		ShortDescription = "a leather belt with bronze mounts",
		LongDescription = null,
		FullDescription = "This leather belt is fitted with small bronze mounts set at intervals along the strap. The leather is broad enough to show its grain, while the metal pieces give it a stronger and more prosperous look. It is decorative waistwear and does not imply pockets, hooks, or storage beyond its visible belt form.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 46.0m,
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

	public static CatalogueItemDefinition MedievalSilverMountedLeatherBelt() => new()
	{
		Metadata = new("medieval_silver_mounted_leather_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9444", []),
		Noun = "belt",
		ShortDescription = "a fine silver-mounted belt",
		LongDescription = null,
		FullDescription = "This fine leather belt is set with small silver mounts and a neatly finished buckle. The strap is smooth and carefully edged, with the pale metal standing out against the darker leather. It presents wealth and care in dress without suggesting any hidden attachments or carried goods.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 280.0,
		Cost = 150.0m,
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

	public static CatalogueItemDefinition MedievalWoolLegTies() => new()
	{
		Metadata = new("medieval_wool_leg_ties", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9476", []),
		Noun = "leg ties",
		ShortDescription = "a pair of $colour wool leg ties",
		LongDescription = null,
		FullDescription = "These $colour wool leg ties are simple narrow lengths of woven cloth, made for binding loose lower-leg layers into a tidy line. The weave is plain and slightly springy, with ends that show minor fraying from use. Their $colour appearance is practical and understated, suited to common travel or labour clothing.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 50.0,
		Cost = 5.0m,
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

	public static CatalogueItemDefinition MedievalLinenHeadband() => new()
	{
		Metadata = new("medieval_linen_headband", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9507", []),
		Noun = "headband",
		ShortDescription = "a $colour linen headband",
		LongDescription = null,
		FullDescription = "This $colour linen headband is a narrow strip of folded cloth with lightly hemmed edges. It is simple enough for work wear, keeping its shape through neat creases rather than stiffness. The $colour dye is plain and even across the band.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Headband",
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

	public static CatalogueItemDefinition MedievalSilkHairRibbon() => new()
	{
		Metadata = new("medieval_silk_hair_ribbon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9538", []),
		Noun = "ribbon",
		ShortDescription = "a fine $colour silk hair ribbon",
		LongDescription = null,
		FullDescription = "This fine $colour silk hair ribbon is a narrow, smooth strip with a soft sheen and delicate folded edges. It is light enough to trail from tied hair or a small head arrangement without adding bulk. The $colour surface gives it a refined decorative quality suited to elite or formal dress.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Headband",
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

	public static CatalogueItemDefinition MedievalSilkRobeSash() => new()
	{
		Metadata = new("medieval_silk_robe_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9569", []),
		Noun = "sash",
		ShortDescription = "a fine $colour silk robe sash",
		LongDescription = null,
		FullDescription = "This fine $colour silk robe sash is long, smooth, and lightly weighted so it can lie in clean folds at the waist. The edges are carefully turned, and the fabric has enough sheen to mark it as a dressier layer. Its $colour surface is meant for visible contrast over robes, caftans, or formal gowns.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 130.0,
		Cost = 78.0m,
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

	public static CatalogueItemDefinition MedievalWovenKosodeSash() => new()
	{
		Metadata = new("medieval_woven_kosode_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9600", []),
		Noun = "sash",
		ShortDescription = "a $colour woven kosode sash",
		LongDescription = null,
		FullDescription = "This $colour woven kosode sash is a narrow, straight length of fabric with a firm hand and flat selvages. It is simpler and less formal than later elaborate waist sashes, with its effect coming from tidy wrapping rather than ornament. The $colour dye gives a clear band of contrast against a robe.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Sash",
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

	public static CatalogueItemDefinition MedievalHempWaistCord() => new()
	{
		Metadata = new("medieval_hemp_waist_cord", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9631", []),
		Noun = "cord",
		ShortDescription = "a plaited hemp waist cord",
		LongDescription = null,
		FullDescription = "This plaited hemp waist cord is made from coarse fibres twisted into a narrow, durable line. The surface is dry and slightly rough, with simple knotted ends and no metal fittings. It is a humble waist binding and carries no implication of attachments, storage, or rank.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
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

	public static CatalogueItemDefinition MedievalCottonWaistCord() => new()
	{
		Metadata = new("medieval_cotton_waist_cord", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9661", []),
		Noun = "cord",
		ShortDescription = "a white cotton waist cord",
		LongDescription = null,
		FullDescription = "This white cotton waist cord is a soft, narrow length of twisted cotton with plain finished ends. It is lighter and cleaner-looking than hemp cord, suited to warm-climate linen or cotton layers. The cord reads as a modest binding rather than a display belt.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
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

	public static CatalogueItemDefinition MedievalGlassBeadNecklace() => new()
	{
		Metadata = new("medieval_glass_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9691", []),
		Noun = "necklace",
		ShortDescription = "a glass bead necklace",
		LongDescription = null,
		FullDescription = "This necklace is strung with small glass beads of mixed muted tones, each bead catching light along its rounded edge. The strand is simple and close enough to sit at the base of the throat. It gives colour and trade-good texture without becoming a formal jewel.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalAmberBeadNecklace() => new()
	{
		Metadata = new("medieval_amber_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9721", []),
		Noun = "necklace",
		ShortDescription = "an amber bead necklace",
		LongDescription = null,
		FullDescription = "This amber bead necklace is made from warm translucent beads, each polished into a rounded or slightly irregular shape. The colour varies from honey to darker resinous brown, giving the strand a natural depth. It is a conspicuous but still wearable sign of northern trade and household wealth.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "amber",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Household Goods / Luxury Wares"
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

	public static CatalogueItemDefinition MedievalShellBeadNecklace() => new()
	{
		Metadata = new("medieval_shell_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9751", []),
		Noun = "necklace",
		ShortDescription = "a shell bead necklace",
		LongDescription = null,
		FullDescription = "This shell bead necklace is strung from pale, smoothed shell pieces with a faint natural lustre. The beads are irregular enough to show their organic origin, but they have been worked into a comfortable line. It suits coastal and trade-linked dress without suggesting any ritual or magical purpose.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Household Goods / Simple Wares"
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

	public static CatalogueItemDefinition MedievalBronzeArmlet() => new()
	{
		Metadata = new("medieval_bronze_armlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9781", []),
		Noun = "armlet",
		ShortDescription = "a bronze armlet",
		LongDescription = null,
		FullDescription = "This bronze armlet is a curved band of metal with slightly rounded edges and a plain polished face. It is sturdy enough to hold its shape, with no sharp projections or hanging elements. The warm bronze colour makes it visible against sleeves or bare skin.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Household Goods / Simple Wares"
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

	public static CatalogueItemDefinition MedievalBronzeBracelets() => new()
	{
		Metadata = new("medieval_bronze_bracelets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9811", []),
		Noun = "bracelets",
		ShortDescription = "a pair of bronze bracelets",
		LongDescription = null,
		FullDescription = "These bronze bracelets are a matched pair of simple rounded bands with smoothed inner faces. The metal is polished but not precious, giving them a warm muted shine. They are decorative wristwear suited to everyday prosperity rather than formal court display.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Household Goods / Simple Wares"
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

	public static CatalogueItemDefinition MedievalBronzeBangles() => new()
	{
		Metadata = new("medieval_bronze_bangles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9841", []),
		Noun = "bangles",
		ShortDescription = "a pair of bronze bangles",
		LongDescription = null,
		FullDescription = "These bronze bangles are circular, smooth, and slightly heavier than narrow bracelets. Their plain rounded surfaces make the metal itself the main ornament. They are suited to layered jewellery looks without relying on gems or written markings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Household Goods / Simple Wares"
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

	public static CatalogueItemDefinition MedievalBronzeAnklets() => new()
	{
		Metadata = new("medieval_bronze_anklets", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9871", []),
		Noun = "anklets",
		ShortDescription = "a pair of bronze anklets",
		LongDescription = null,
		FullDescription = "These bronze anklets are made as a pair of smooth metal rings sized for the lower leg. The bronze is plainly finished, with enough weight to look substantial without being ostentatious. They are decorative worn ornaments and do not include bells, charms, or other carried fittings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 115.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Household Goods / Simple Wares"
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

	public static CatalogueItemDefinition MedievalSilverEarrings() => new()
	{
		Metadata = new("medieval_silver_earrings", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9901", []),
		Noun = "earrings",
		ShortDescription = "a pair of silver earrings",
		LongDescription = null,
		FullDescription = "These silver earrings are a matched pair of small curved ornaments with bright polished surfaces. Their form is restrained, relying on the clean metal and neat symmetry rather than large stones. They add a clear note of wealth while remaining general enough for many local styles.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Household Goods / Luxury Wares"
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

	public static CatalogueItemDefinition MedievalSimpleBronzeRing() => new()
	{
		Metadata = new("medieval_simple_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9931", []),
		Noun = "ring",
		ShortDescription = "a simple bronze finger ring",
		LongDescription = null,
		FullDescription = "This simple bronze finger ring is a narrow band with a smoothed inner face and a plain rounded outside. The metal is modest and workmanlike, showing small variations from hand finishing. It is visible personal adornment without any seal, inscription, or hidden function.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineSilverRing() => new()
	{
		Metadata = new("medieval_fine_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9961", []),
		Noun = "ring",
		ShortDescription = "a fine silver finger ring",
		LongDescription = null,
		FullDescription = "This fine silver finger ring is a cleanly made band with a bright surface and a slightly raised outer curve. Its edges are even, and the polish makes it stand out against the hand. The ring is decorative rather than a signet or mechanically meaningful seal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Fashion Accessories",
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBoneHairpin() => new()
	{
		Metadata = new("medieval_bone_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:9991", []),
		Noun = "hairpin",
		ShortDescription = "a bone hairpin",
		LongDescription = null,
		FullDescription = "This bone hairpin is slender, pale, and smoothly polished, with a simple rounded head. The shaft is narrow enough to disappear partly into hair, while the worked top remains visible. It is a practical ornament and fastening piece without carving or precious decoration.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Headband",
				"Destroyable_Misc",
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

	public static CatalogueItemDefinition MedievalBronzeHairpin() => new()
	{
		Metadata = new("medieval_bronze_hairpin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10021", []),
		Noun = "hairpin",
		ShortDescription = "a bronze hairpin",
		LongDescription = null,
		FullDescription = "This bronze hairpin has a slim shaft and a small shaped head with a muted metallic sheen. The surface is smooth, with only minor filing marks near the tip. It gives a modest decorative accent to dressed hair without becoming elaborate jewellery.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Headband",
				"Destroyable_HeavyMetal",
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

	public static CatalogueItemDefinition MedievalSilverHairpins() => new()
	{
		Metadata = new("medieval_silver_hairpins", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10051", []),
		Noun = "hairpins",
		ShortDescription = "a pair of silver hairpins",
		LongDescription = null,
		FullDescription = "These silver hairpins are a matched pair of slender pins with bright polished heads. Their long shafts are plain and neat, leaving the silver tips as the visible ornament. They are refined hair adornments suited to formal dress, not hidden tools or weapons.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Headband",
				"Destroyable_HeavyMetal",
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

	public static CatalogueItemDefinition MedievalTabletWovenBelt() => new()
	{
		Metadata = new("medieval_tablet_woven_belt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10081", []),
		Noun = "belt",
		ShortDescription = "a $colour tablet-woven belt",
		LongDescription = null,
		FullDescription = "This $colour tablet-woven belt is a firm narrow band with a tight patterned structure and neat edges. The wool is compact rather than bulky, giving the belt enough body to lie flat around the waist. The $colour dye and visible weave make it a small but noticeable textile display.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Belts"
			],
		Components = [
				"Holdable",
				"Wear_Waist",
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

	public static CatalogueItemDefinition MedievalDecoratedLeatherGirdle() => new()
	{
		Metadata = new("medieval_decorated_leather_girdle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10112", []),
		Noun = "girdle",
		ShortDescription = "a decorated leather girdle",
		LongDescription = null,
		FullDescription = "This decorated leather girdle is a narrow waist strap with careful edging and small tooled details along its length. The leather is supple and smooth, intended to hang cleanly over a gown or kirtle. It is ornamental waistwear and does not promise storage, attachments, or mechanical support.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 170.0,
		Cost = 48.0m,
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

	public static CatalogueItemDefinition MedievalTravelWoolCloak() => new()
	{
		Metadata = new("medieval_travel_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10142", []),
		Noun = "cloak",
		ShortDescription = "a weather-stained $colour wool travel cloak",
		LongDescription = null,
		FullDescription = "This weather-stained $colour wool travel cloak is cut full enough to wrap over the shoulders and upper body. The wool shows worn folds, dulled edges, and a practical weight suited to road use. Its $colour surface looks faded and serviceable rather than courtly or ceremonial.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1150.0,
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
				"Wear_Cloak_(Closed)",
				"Destroyable_Clothing",
				"Insulation_Strong",
				"Armour_HeavyClothing",
				"Variable_DrabColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBroadBrimFeltHat() => new()
	{
		Metadata = new("medieval_broad_brim_felt_hat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10173", []),
		Noun = "hat",
		ShortDescription = "a broad-brim $colour felt travel hat",
		LongDescription = null,
		FullDescription = "This broad-brim $colour felt travel hat has a low crown and a wide soft brim. The felt is dense but flexible, with slight waviness where it has been handled and packed. The $colour shade is plain enough for road wear while still reading as deliberate headgear.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 190.0,
		Cost = 12.0m,
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

	public static CatalogueItemDefinition MedievalTravelWoolGaiters() => new()
	{
		Metadata = new("medieval_travel_wool_gaiters", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10204", []),
		Noun = "gaiters",
		ShortDescription = "a pair of $colour wool travel gaiters",
		LongDescription = null,
		FullDescription = "These $colour wool travel gaiters are shaped as lower-leg coverings with simple ties along the side. The wool is thicker than ordinary leg ties, with reinforced edges where the cloth meets footwear. Their $colour surface gives a practical layer for road dust and rough ground.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 210.0,
		Cost = 8.0m,
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

	public static CatalogueItemDefinition MedievalOiledLinenRainCape() => new()
	{
		Metadata = new("medieval_oiled_linen_rain_cape", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10235", []),
		Noun = "cape",
		ShortDescription = "an oiled linen rain cape",
		LongDescription = null,
		FullDescription = "This linen rain cape has a slightly darkened, oiled-looking surface and a simple shoulder-draped cut. The cloth is lighter than wool but stiffer from its finish, falling in broad practical folds. It reads as weather-facing outerwear without claiming special waterproof mechanics.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Cape",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalShepherdsHoodedCloak() => new()
	{
		Metadata = new("medieval_shepherds_hooded_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10265", []),
		Noun = "cloak",
		ShortDescription = "a rough $colour shepherd's cloak",
		LongDescription = null,
		FullDescription = "This rough $colour shepherd's cloak is heavy, hooded, and cut to hang well below the waist. The wool is coarse, with a thick nap and broad folds that make it look suited to open fields. The $colour dye is plain and uneven enough to feel rural without becoming deliberately ragged.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1320.0,
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

	public static CatalogueItemDefinition MedievalRoughFeltHerderHat() => new()
	{
		Metadata = new("medieval_rough_felt_herder_hat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10296", []),
		Noun = "hat",
		ShortDescription = "a rough felt herder's hat",
		LongDescription = null,
		FullDescription = "This rough felt herder's hat has a simple crown and a worn, uneven brim. The felt is dense and plain, with a slightly lumpy surface from hand shaping. It is practical rural headwear rather than a court cap or formal town hat.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
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
				"Armour_LightClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFisherHeadcloth() => new()
	{
		Metadata = new("medieval_fisher_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10326", []),
		Noun = "headcloth",
		ShortDescription = "a $colour fisher's headcloth",
		LongDescription = null,
		FullDescription = "This $colour fisher's headcloth is a square of linen folded to sit close around the hair and brow. The edges are simply hemmed, and the cloth has the light creasing of a practical work covering. Its $colour surface is plain and washable, suited to damp labour rather than display.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 75.0,
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
				"Wear_Kerchief",
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

	public static CatalogueItemDefinition MedievalShortBoatmansTunic() => new()
	{
		Metadata = new("medieval_short_boatmans_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10357", []),
		Noun = "tunic",
		ShortDescription = "a short $colour boatman's tunic",
		LongDescription = null,
		FullDescription = "This short $colour boatman's tunic is cut above the knee, with modest side openings and sleeves kept clear of the wrist. The wool is plain and sturdy, made for movement rather than warmth alone. The $colour dye sits evenly across a garment meant for wet work and cramped footing.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 470.0,
		Cost = 14.0m,
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

	public static CatalogueItemDefinition MedievalRopeSoledHempSandals() => new()
	{
		Metadata = new("medieval_rope_soled_hemp_sandals", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10388", []),
		Noun = "sandals",
		ShortDescription = "a pair of rope-soled hemp sandals",
		LongDescription = null,
		FullDescription = "These hemp sandals have rope-like soles and simple woven straps across the foot. The fibre is rough, pale, and plainly twisted, giving the sandals a light but workmanlike appearance. They are humble footwear for warm or wet work rather than fine town shoes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
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

	public static CatalogueItemDefinition MedievalLeatherHawkingGlove() => new()
	{
		Metadata = new("medieval_leather_hawking_glove", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10418", []),
		Noun = "glove",
		ShortDescription = "a single leather hawking glove",
		LongDescription = null,
		FullDescription = "This single leather hawking glove is long through the wrist, with a heavier palm and a smooth outer back. The leather is supple but thick, showing careful stitching around the fingers and cuff. It is visibly specialised outdoor clothing, not a weapon or animal-handling tool by itself.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 170.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Gloves"
			],
		Components = [
				"Holdable",
				"Wear_Gloves",
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

	public static CatalogueItemDefinition MedievalRoughWoolGaiters() => new()
	{
		Metadata = new("medieval_rough_wool_gaiters", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10448", []),
		Noun = "gaiters",
		ShortDescription = "a pair of rough $colour wool gaiters",
		LongDescription = null,
		FullDescription = "These rough $colour wool gaiters are plain lower-leg coverings with thick hems and visible tie points. The fabric looks coarse and hard-worn, with a few uneven patches in the weave. Their $colour shade is deliberately dull, fitting mud, yard work, and long walking.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230.0,
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
				"Variable_DrabColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalPatchedWoolCloak() => new()
	{
		Metadata = new("medieval_patched_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10479", []),
		Noun = "cloak",
		ShortDescription = "a patched $colour wool cloak",
		LongDescription = null,
		FullDescription = "This patched $colour wool cloak is made from worn fabric reinforced with several visible repairs. The edges are uneven but functional, and the patchwork breaks up the original line of the cloth. Its $colour surface is faded and humble, marking it as a hard-used outer layer.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1040.0,
		Cost = 14.0m,
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
				"Variable_DrabColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalStablehandWoolCap() => new()
	{
		Metadata = new("medieval_stablehand_wool_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10510", []),
		Noun = "cap",
		ShortDescription = "a $colour stablehand's wool cap",
		LongDescription = null,
		FullDescription = "This $colour stablehand's wool cap is close-fitting, soft, and simply seamed. The wool has a slightly fuzzy surface, giving it warmth without looking fine or expensive. Its $colour tone is ordinary and practical for yard work or cart service.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
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

	public static CatalogueItemDefinition MedievalCartDriverWoolCoat() => new()
	{
		Metadata = new("medieval_cart_driver_wool_coat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10541", []),
		Noun = "coat",
		ShortDescription = "a belted $colour cart driver's coat",
		LongDescription = null,
		FullDescription = "This belted $colour cart driver's coat is a front-opening wool layer cut long enough to cover the hips. The sleeves are practical and the skirt has enough room for walking, riding, or climbing onto a cart. The $colour wool is plain, with the belt line giving it a tidy working shape.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 820.0,
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

	public static CatalogueItemDefinition MedievalPlainServiceGown() => new()
	{
		Metadata = new("medieval_plain_service_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10572", []),
		Noun = "gown",
		ShortDescription = "a plain $colour service gown",
		LongDescription = null,
		FullDescription = "This plain $colour service gown is cut from straight wool panels with a modest neck and simple sleeves. The hem is practical rather than trailing, and the seams are made for wear rather than display. Its $colour cloth is clean enough for household service but not fine enough for court dress.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 18.0m,
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

	public static CatalogueItemDefinition MedievalTuckedWorkKirtle() => new()
	{
		Metadata = new("medieval_tucked_work_kirtle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10603", []),
		Noun = "kirtle",
		ShortDescription = "a tucked $colour work kirtle",
		LongDescription = null,
		FullDescription = "This tucked $colour work kirtle is a practical wool garment with its skirt arranged to sit higher and freer than a courtly gown. The sleeves are narrow enough for labour, and the waist can be controlled by a sash or cord. The $colour fabric remains plain, emphasizing use and movement.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 18.0m,
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

	public static CatalogueItemDefinition MedievalLargeLinenWorkApron() => new()
	{
		Metadata = new("medieval_large_linen_work_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10634", []),
		Noun = "apron",
		ShortDescription = "a large $colour linen work apron",
		LongDescription = null,
		FullDescription = "This large $colour linen work apron is made from a broad rectangular panel with simple ties. The linen is sturdy and washable-looking, falling over the front of the body in clean practical folds. Its $colour surface is plain enough for kitchens, laundry, and other household labour.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
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
				"Wear_Apron",
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

	public static CatalogueItemDefinition MedievalLinenLaundryHeadcloth() => new()
	{
		Metadata = new("medieval_linen_laundry_headcloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10665", []),
		Noun = "headcloth",
		ShortDescription = "a $colour laundry headcloth",
		LongDescription = null,
		FullDescription = "This $colour laundry headcloth is a folded linen covering meant to sit securely around the hair. The cloth is light but closely woven, with a simple hem and no decorative border. Its $colour dye is modest and suited to damp, practical work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
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
				"Wear_Kerchief",
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

	public static CatalogueItemDefinition MedievalShortServiceTunic() => new()
	{
		Metadata = new("medieval_short_service_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10696", []),
		Noun = "tunic",
		ShortDescription = "a short $colour service tunic",
		LongDescription = null,
		FullDescription = "This short $colour service tunic is cut for easy movement, with a plain round neck and sturdy sleeves. The hem falls high enough to keep clear of steps, yards, and kitchen work. Its $colour wool is tidy but unadorned, marking it as working dress rather than field rags.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 470.0,
		Cost = 14.0m,
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

	public static CatalogueItemDefinition MedievalHouseholdWorkSash() => new()
	{
		Metadata = new("medieval_household_work_sash", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10727", []),
		Noun = "sash",
		ShortDescription = "a $colour household work sash",
		LongDescription = null,
		FullDescription = "This $colour household work sash is a narrow wool band with plain edges and enough length to tie securely. It gives shape to a loose tunic, gown, or robe without adding ornament. The $colour cloth is practical and subdued, suitable for ordinary service work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 5.0m,
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

	public static CatalogueItemDefinition MedievalKitchenSleeveTies() => new()
	{
		Metadata = new("medieval_kitchen_sleeve_ties", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10758", []),
		Noun = "sleeve ties",
		ShortDescription = "a pair of $colour kitchen sleeve ties",
		LongDescription = null,
		FullDescription = "These $colour kitchen sleeve ties are narrow strips of linen made to bind or gather fabric near the forearms. The edges are simply finished, and the cloth has the softened look of repeated laundering. Their $colour shade is plain, emphasizing cleanliness and practical control of sleeves.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Gloves"
			],
		Components = [
				"Holdable",
				"Wear_Bracers",
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

	public static CatalogueItemDefinition MedievalScullionLinenCoif() => new()
	{
		Metadata = new("medieval_scullion_linen_coif", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10789", []),
		Noun = "coif",
		ShortDescription = "a $colour scullion's linen coif",
		LongDescription = null,
		FullDescription = "This $colour scullion's linen coif is close-fitting, with ties under the chin and a plain rounded crown. The linen is light and washable-looking, with no embroidery or fine trim. Its $colour surface makes it serviceable headwear for kitchens and busy households.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 4.0m,
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

	public static CatalogueItemDefinition MedievalLeatherStableApron() => new()
	{
		Metadata = new("medieval_leather_stable_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10820", []),
		Noun = "apron",
		ShortDescription = "a leather stable apron",
		LongDescription = null,
		FullDescription = "This leather stable apron is a plain protective front layer with a broad body and simple waist ties. The leather is thick enough to look durable, with scuffs and creases across the lower panel. It is worn clothing for dirty yard work, not a tool belt or storage apron.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
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

	public static CatalogueItemDefinition MedievalInnWorkersWoolHood() => new()
	{
		Metadata = new("medieval_inn_workers_wool_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10850", []),
		Noun = "hood",
		ShortDescription = "a $colour inn worker's wool hood",
		LongDescription = null,
		FullDescription = "This $colour inn worker's wool hood has a short shoulder cape and a close face opening. The wool is plain but tidy, giving a warmer service layer than a cap or coif. Its $colour cloth suits public-facing work without suggesting formal livery.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230.0,
		Cost = 10.0m,
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

	public static CatalogueItemDefinition MedievalStudentWoolGown() => new()
	{
		Metadata = new("medieval_student_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10881", []),
		Noun = "gown",
		ShortDescription = "a plain $colour student gown",
		LongDescription = null,
		FullDescription = "This plain $colour student gown is a long wool garment with straight sleeves and an unadorned fall. The cut is sober and practical, covering most ordinary clothing beneath it. Its $colour fabric is respectable but restrained, fitting study or clerical town life rather than court display.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
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

	public static CatalogueItemDefinition MedievalMasterScholarGown() => new()
	{
		Metadata = new("medieval_master_scholar_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10912", []),
		Noun = "gown",
		ShortDescription = "a sober $colour scholar's gown",
		LongDescription = null,
		FullDescription = "This sober $colour scholar's gown is long, full, and carefully made from good wool. The sleeves fall cleanly, and the body has enough breadth to drape with learned dignity rather than workroom tightness. The $colour shade is rich but controlled, suggesting standing without theatrical ornament.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 60.0m,
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

	public static CatalogueItemDefinition MedievalPhysicianWoolGown() => new()
	{
		Metadata = new("medieval_physician_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10943", []),
		Noun = "gown",
		ShortDescription = "a long $colour physician's gown",
		LongDescription = null,
		FullDescription = "This long $colour physician's gown is a dignified wool garment with full sleeves and a straight, weighty fall. The seams are neat and the cloth is better than ordinary work wear, but it avoids embroidery or official insignia. Its $colour tone gives it a sober professional presence.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 840.0,
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

	public static CatalogueItemDefinition MedievalNotaryWoolGown() => new()
	{
		Metadata = new("medieval_notary_wool_gown", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:10974", []),
		Noun = "gown",
		ShortDescription = "a neat $colour notary's gown",
		LongDescription = null,
		FullDescription = "This neat $colour notary's gown is cut cleanly, with modest sleeves and a tidy hem. The wool is good enough to show status but not so rich as to look noble or ceremonial. The $colour surface gives a restrained professional look suitable for writing desks, courts, and town offices.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
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

	public static CatalogueItemDefinition MedievalLinedScholarsHood() => new()
	{
		Metadata = new("medieval_lined_scholars_hood", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11005", []),
		Noun = "hood",
		ShortDescription = "a lined $colour scholar's hood",
		LongDescription = null,
		FullDescription = "This lined $colour scholar's hood has a close face opening and a short shoulder fall. The wool exterior is smooth, while the lining adds enough body to make the folds sit neatly. The $colour fabric gives it a more formal look than an ordinary work hood.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hoodie",
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

	public static CatalogueItemDefinition MedievalPlainBlackSkullcap() => new()
	{
		Metadata = new("medieval_plain_black_skullcap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11036", []),
		Noun = "skullcap",
		ShortDescription = "a plain black skullcap",
		LongDescription = null,
		FullDescription = "This plain black skullcap is small, close-fitting, and made from dark wool. Its shape is simple and rounded, with no embroidery, badge, or written mark. It gives a sober head-covering option for learned or clerical dress without making a culture-specific claim.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Skullcap",
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

	public static CatalogueItemDefinition MedievalSoberLectureMantle() => new()
	{
		Metadata = new("medieval_sober_lecture_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11066", []),
		Noun = "mantle",
		ShortDescription = "a sober $colour lecture mantle",
		LongDescription = null,
		FullDescription = "This sober $colour lecture mantle is a broad wool outer layer with a controlled drape and plain finished border. It is less martial and less courtly than a display cloak, relying on clean cloth and measured folds. The $colour shade is dignified without being bright or festive.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 52.0m,
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
				"Variable_FineColour"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalFineScholarsCap() => new()
	{
		Metadata = new("medieval_fine_scholars_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11097", []),
		Noun = "cap",
		ShortDescription = "a fine $colour scholar's cap",
		LongDescription = null,
		FullDescription = "This fine $colour scholar's cap is a small shaped wool cap with a neat crown and clean lower edge. The cloth is smooth and carefully finished, giving it more presence than a work cap. The $colour tone makes it suitable for learned public dress without implying a specific degree or office.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 80.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
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

	public static CatalogueItemDefinition MedievalChildWoolTunic() => new()
	{
		Metadata = new("medieval_child_wool_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11128", []),
		Noun = "tunic",
		ShortDescription = "a small $colour child's wool tunic",
		LongDescription = null,
		FullDescription = "This small $colour child's wool tunic is cut simply, with straight sleeves and a short practical hem. The wool is plain and sturdy, built like adult common clothing scaled down in bulk. Its $colour fabric is ordinary and suited to daily household or street wear.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 8.0m,
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

	public static CatalogueItemDefinition MedievalChildLinenShift() => new()
	{
		Metadata = new("medieval_child_linen_shift", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11159", []),
		Noun = "shift",
		ShortDescription = "a small $colour child's linen shift",
		LongDescription = null,
		FullDescription = "This small $colour child's linen shift is a light undergarment with a simple neck and loose sleeves. The linen is soft and plain, with seams kept unobtrusive for wear beneath heavier layers. The $colour tone is modest and even across the small panels.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 5.0m,
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

	public static CatalogueItemDefinition MedievalChildLeatherShoes() => new()
	{
		Metadata = new("medieval_child_leather_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11190", []),
		Noun = "shoes",
		ShortDescription = "a pair of small leather child's shoes",
		LongDescription = null,
		FullDescription = "These small leather child's shoes are soft, low, and simply stitched. The soles are thin compared with adult work shoes, and the uppers show modest shaping around the foot. They are ordinary child-sized footwear without ornament or hidden fittings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 230.0,
		Cost = 10.0m,
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

	public static CatalogueItemDefinition MedievalChildWoolCloak() => new()
	{
		Metadata = new("medieval_child_wool_cloak", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11220", []),
		Noun = "cloak",
		ShortDescription = "a small $colour child's wool cloak",
		LongDescription = null,
		FullDescription = "This small $colour child's wool cloak is a short outer layer with enough fullness to wrap around narrow shoulders. The wool is plain and warm-looking, with a simple finished edge and no heavy trim. Its $colour dye matches common clothing more than formal display.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 12.0m,
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

	public static CatalogueItemDefinition MedievalApprenticeWorkTunic() => new()
	{
		Metadata = new("medieval_apprentice_work_tunic", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11251", []),
		Noun = "tunic",
		ShortDescription = "a short $colour apprentice's tunic",
		LongDescription = null,
		FullDescription = "This short $colour apprentice's tunic is cut for movement, with plain sleeves and a modest hem. The wool is sturdy but not fine, suited to errands, workshops, and household labour. The $colour cloth gives it a tidy working look without any badge or formal guild mark.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 440.0,
		Cost = 12.0m,
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

	public static CatalogueItemDefinition MedievalApprenticeLinenCap() => new()
	{
		Metadata = new("medieval_apprentice_linen_cap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11282", []),
		Noun = "cap",
		ShortDescription = "a $colour apprentice's linen cap",
		LongDescription = null,
		FullDescription = "This $colour apprentice's linen cap is light, close-fitting, and simply seamed. The linen is plain and easy to launder, with no ornament or rank sign. Its $colour surface suits a young worker or trainee more than a prosperous master.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Headwear"
			],
		Components = [
				"Holdable",
				"Wear_Hat",
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

	public static CatalogueItemDefinition MedievalChildSilkCourtRobe() => new()
	{
		Metadata = new("medieval_child_silk_court_robe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11313", []),
		Noun = "robe",
		ShortDescription = "a small fine $colour silk court robe",
		LongDescription = null,
		FullDescription = "This small fine $colour silk court robe is cut from smooth fabric with a careful straight fall and neat sleeve openings. The garment is sized for a child but made with the same attention as elite adult dress. The $colour silk gives it a bright, formal presence without needing jewellery or insignia.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 360.0,
		Cost = 110.0m,
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

	public static CatalogueItemDefinition MedievalApprenticeCanvasApron() => new()
	{
		Metadata = new("medieval_apprentice_canvas_apron", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11344", []),
		Noun = "apron",
		ShortDescription = "a $colour apprentice's canvas apron",
		LongDescription = null,
		FullDescription = "This $colour apprentice's canvas apron is a plain front layer with simple ties and a hard-wearing weave. The canvas is thicker than linen and sits flat over a tunic or kirtle. Its $colour surface is practical and unmarked, avoiding any claim of a formal guild uniform.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Apron",
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

	public static CatalogueItemDefinition MedievalSoftLeatherShoes() => new()
	{
		Metadata = new("medieval_soft_leather_shoes", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11377", []),
		Noun = "shoes",
		ShortDescription = "a pair of soft leather shoes",
		LongDescription = null,
		FullDescription = "These soft leather shoes are low, flexible footwear with rounded toes and simple stitched uppers. The soles are sturdy enough for regular use without making the shoes stiff or high. Natural variations in the leather show along the seams, toe creases, and cut edges.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
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

	public static CatalogueItemDefinition MedievalLatinWhiteAlb() => new()
	{
		Metadata = new("medieval_latin_white_alb", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11407", []),
		Noun = "alb",
		ShortDescription = "a white linen alb",
		LongDescription = null,
		FullDescription = "This white linen alb is a long liturgical robe with narrow sleeves and a full, clean fall to the feet. The linen is light but well finished, showing broad pale folds where it gathers at the waist. Its edges are plain and carefully hemmed so that brighter vestments can sit clearly over it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 460.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
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

	public static CatalogueItemDefinition MedievalLatinChasuble() => new()
	{
		Metadata = new("medieval_latin_chasuble", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11445", []),
		Noun = "chasuble",
		ShortDescription = "a fine $colour silk chasuble",
		LongDescription = null,
		FullDescription = "This fine $colour silk chasuble is cut as a broad outer vestment with a head opening and a sweeping fall over the torso. The silk lies in rounded folds, its $colour surface broken by the shadow of the shoulders and the heavy lower edge. It is visibly more formal than ordinary clothing, relying on rich cloth, breadth, and clean ceremonial shape.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Poncho",
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

	public static CatalogueItemDefinition MedievalBuddhistFormalKesa() => new()
	{
		Metadata = new("medieval_buddhist_formal_kesa", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11478", []),
		Noun = "kesa",
		ShortDescription = "a fine silk kesa mantle",
		LongDescription = null,
		FullDescription = "This fine silk kesa mantle is a rectangular patchwork garment made from smooth, carefully joined panels. The silk has a muted ceremonial sheen, and the panel layout gives the cloth a measured, gridded order. It is worn as an outer mantle, more formal than a common kasaya but still defined by patchwork construction.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 360.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Market / Clothing / Luxury Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Mantle",
				"Destroyable_Clothing",
				"Insulation_Moderate",
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalBuddhistPlainUnderrobe() => new()
	{
		Metadata = new("medieval_buddhist_plain_underrobe", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11508", []),
		Noun = "underrobe",
		ShortDescription = "a plain hemp monastic underrobe",
		LongDescription = null,
		FullDescription = "This plain hemp monastic underrobe is a straight, cross-wrapped robe with close sleeves and a practical hem. The hemp is sturdy, dull, and lightly textured, giving the garment a subdued working surface. It is meant to sit beneath a kasaya or mantle, with no decoration competing with the outer cloth.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Clothing / Standard Clothing",
				"Functions / Worn Items / Bodywear"
			],
		Components = [
				"Holdable",
				"Wear_Robe",
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

	public static CatalogueItemDefinition MedievalBuddhistTravellingMantle() => new()
	{
		Metadata = new("medieval_buddhist_travelling_mantle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11538", []),
		Noun = "mantle",
		ShortDescription = "a dark wool monastic travelling mantle",
		LongDescription = null,
		FullDescription = "This dark wool monastic travelling mantle is a broad outer cloth for sheltering a robe on the road. The wool is plain and serviceable, falling in wide folds over the shoulders and down the back. Its shape is practical and unornamented, closer to a traveller’s wrap than a formal temple mantle.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 760.0,
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
				"Armour_HeavyClothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalHempSandals() => new()
	{
		Metadata = new("medieval_hemp_sandals", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalClothing.cs:11568", []),
		Noun = "sandals",
		ShortDescription = "a pair of woven hemp sandals",
		LongDescription = null,
		FullDescription = "These woven hemp sandals are light, open footwear made from braided plant-fibre straps and simple flat soles. The weave is coarse but regular, leaving the toes and much of the foot exposed. The natural hemp surface is plainly visible, with small slubs and a practical, unpolished finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
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

	public static CatalogueItemDefinition MedievalLockingTradeClerkCoinLockbox() => new()
	{
		Metadata = new("medieval_locking_trade_clerk_coin_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:11", []),
		Noun = "lockbox",
		ShortDescription = "a clerk's coin lockbox",
		LongDescription = null,
		FullDescription = "This clerk's coin lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 52.0m,
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
		BuilderNotes = "Small built-in-lock box for coin, tallies, seals, or market takings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeClothMerchantCoffer() => new()
	{
		Metadata = new("medieval_locking_trade_cloth_merchant_coffer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:41", []),
		Noun = "coffer",
		ShortDescription = "a cloth merchant's coffer",
		LongDescription = null,
		FullDescription = "This cloth merchant's coffer is a medium-sized, well-made coffer built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 17000.0,
		Cost = 230.0m,
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
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking coffer for bolts of fine cloth, sample books, and trade accounts.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeCustomsChest() => new()
	{
		Metadata = new("medieval_locking_trade_customs_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:71", []),
		Noun = "chest",
		ShortDescription = "a customs strong chest",
		LongDescription = null,
		FullDescription = "This customs strong chest is a medium-sized, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 20000.0,
		Cost = 250.0m,
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
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Locking chest for customs duties, sealed permits, and cargo records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeFairTakingsLockbox() => new()
	{
		Metadata = new("medieval_locking_trade_fair_takings_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:101", []),
		Noun = "lockbox",
		ShortDescription = "a fair-takings lockbox",
		LongDescription = null,
		FullDescription = "This fair-takings lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 62.0m,
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
		BuilderNotes = "Portable lockbox for fair, booth, or stall takings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeGuildStrongChest() => new()
	{
		Metadata = new("medieval_locking_trade_guild_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:131", []),
		Noun = "chest",
		ShortDescription = "a guild strong chest",
		LongDescription = null,
		FullDescription = "This guild strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 42000.0,
		Cost = 460.0m,
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
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy institutional chest for guild funds, charters, and records.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeJewellerParcelLockbox() => new()
	{
		Metadata = new("medieval_locking_trade_jeweller_parcel_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:161", []),
		Noun = "lockbox",
		ShortDescription = "a jeweller's parcel lockbox",
		LongDescription = null,
		FullDescription = "This jeweller's parcel lockbox is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 150.0m,
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
		BuilderNotes = "Compact lockbox for small high-value parcels and wrapped valuables.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeMarketRentChest() => new()
	{
		Metadata = new("medieval_locking_trade_market_rent_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:191", []),
		Noun = "chest",
		ShortDescription = "a market-rent chest",
		LongDescription = null,
		FullDescription = "This market-rent chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 18500.0,
		Cost = 230.0m,
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
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Chest for market dues, rent rolls, and payments.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeMerchantStrongbox() => new()
	{
		Metadata = new("medieval_locking_trade_merchant_strongbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:221", []),
		Noun = "strongbox",
		ShortDescription = "a merchant strongbox",
		LongDescription = null,
		FullDescription = "This merchant strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 15000.0,
		Cost = 210.0m,
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
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stout trade strongbox for valuables, accounts, and compact cargo.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeMoneyChangerCashBox() => new()
	{
		Metadata = new("medieval_locking_trade_money_changer_cash_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:251", []),
		Noun = "lockbox",
		ShortDescription = "a money-changer's cash box",
		LongDescription = null,
		FullDescription = "This money-changer's cash box is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 120.0m,
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
		BuilderNotes = "Metal cash box for money-changing counters and trade booths.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeNotaryDocumentLockbox() => new()
	{
		Metadata = new("medieval_locking_trade_notary_document_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:281", []),
		Noun = "lockbox",
		ShortDescription = "a notary's document lockbox",
		LongDescription = null,
		FullDescription = "This notary's document lockbox is a small, well-made lockbox built from walnut boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 110.0m,
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
		BuilderNotes = "Small lockbox for deeds, writs, contracts, and sealed papers.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeSpiceChest() => new()
	{
		Metadata = new("medieval_locking_trade_spice_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:311", []),
		Noun = "chest",
		ShortDescription = "a locked spice chest",
		LongDescription = null,
		FullDescription = "This locked spice chest is a medium-sized, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 14500.0,
		Cost = 240.0m,
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
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Built-in-lock chest for spices, dyestuffs, or small high-value goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeSpiceSampleLockbox() => new()
	{
		Metadata = new("medieval_locking_trade_spice_sample_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:341", []),
		Noun = "lockbox",
		ShortDescription = "a spice-sample lockbox",
		LongDescription = null,
		FullDescription = "This spice-sample lockbox is a small, well-made lockbox built from cedar boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 95.0m,
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
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Built-in-lock box for valuable spice samples or small packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeTallySealLockbox() => new()
	{
		Metadata = new("medieval_locking_trade_tally_seal_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:371", []),
		Noun = "lockbox",
		ShortDescription = "a tally-seal lockbox",
		LongDescription = null,
		FullDescription = "This tally-seal lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3000.0,
		Cost = 58.0m,
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
		BuilderNotes = "Lockbox for marked tallies, wax seals, and small account packets.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeTaxStrongChest() => new()
	{
		Metadata = new("medieval_locking_trade_tax_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:401", []),
		Noun = "chest",
		ShortDescription = "a tax strong chest",
		LongDescription = null,
		FullDescription = "This tax strong chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 62000.0,
		Cost = 680.0m,
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
		BuilderNotes = "Heavy metal-reinforced chest for taxes, fees, or administrative funds.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeTollChest() => new()
	{
		Metadata = new("medieval_locking_trade_toll_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:431", []),
		Noun = "chest",
		ShortDescription = "a toll-keeper's chest",
		LongDescription = null,
		FullDescription = "This toll-keeper's chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 225.0m,
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
				"LockingContainer_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Built-in-lock chest for tolls, fees, or gatehouse takings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeWarehouseStrongChest() => new()
	{
		Metadata = new("medieval_locking_trade_warehouse_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:461", []),
		Noun = "chest",
		ShortDescription = "a warehouse strong chest",
		LongDescription = null,
		FullDescription = "This warehouse strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 36000.0,
		Cost = 360.0m,
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
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy built-in-lock chest for warehouse keys, bonds, and high-value stored goods.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalLockingTradeWeightsLockbox() => new()
	{
		Metadata = new("medieval_locking_trade_weights_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:491", []),
		Noun = "lockbox",
		ShortDescription = "a weights lockbox",
		LongDescription = null,
		FullDescription = "This weights lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3200.0,
		Cost = 64.0m,
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
		BuilderNotes = "Lockbox for keeping small scales weights, seals, and trade tokens together.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeApothecaryIngredientBox() => new()
	{
		Metadata = new("medieval_trade_apothecary_ingredient_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:521", []),
		Noun = "box",
		ShortDescription = "an apothecary ingredient box",
		LongDescription = null,
		FullDescription = "This apothecary ingredient box is a medium-sized, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
		BuilderNotes = "Lidded box for labelled packets, dried roots, gum resins, minerals, and other apothecary stock.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeArmorerRivetBox() => new()
	{
		Metadata = new("medieval_trade_armorer_rivet_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:551", []),
		Noun = "box",
		ShortDescription = "an armourer's rivet box",
		LongDescription = null,
		FullDescription = "This armourer's rivet box is a small, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 20.0m,
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
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small dense box for rivets, buckles, rings, plates, and armour-work fittings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeAshFruitCrate() => new()
	{
		Metadata = new("medieval_trade_ash_fruit_crate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:581", []),
		Noun = "crate",
		ShortDescription = "an ash fruit crate",
		LongDescription = null,
		FullDescription = "This ash fruit crate is a medium-sized, workmanlike crate built from ash boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light rigid crate for orchard fruit, delicate vegetables, or packed table produce.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeAshLimeBin() => new()
	{
		Metadata = new("medieval_trade_ash_lime_bin", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:612", []),
		Noun = "bin",
		ShortDescription = "an ash lime bin",
		LongDescription = null,
		FullDescription = "This ash lime bin is a large, workmanlike bin built from ash boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7800.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ash",
		Tags = [
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Dry workshop bin for lime, chalk, ash, sand, or powdered construction material.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalTradeAshToolBox() => new()
	{
		Metadata = new("medieval_trade_ash_tool_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalContainers.cs:643", []),
		Noun = "box",
		ShortDescription = "an ash tool box",
		LongDescription = null,
		FullDescription = "This ash tool box is a medium-sized, workmanlike box built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3800.0,
		Cost = 26.0m,
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
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy box for trade tools, fittings, measures, and workshop pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
