#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshIvyWreath() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_ivy_wreath", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_ivy_wreath"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:917", []),
		Noun = "wreath",
		ShortDescription = "a fresh ivy wreath",
		LongDescription = null,
		FullDescription = "A fresh ivy wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh ivy wreath carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivy",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Fresh Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_ivy_wreath",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedLaurelWreath() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_laurel_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:948", []),
		Noun = "wreath",
		ShortDescription = "a dried laurel wreath",
		LongDescription = null,
		FullDescription = "A dried laurel wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried laurel wreath feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "laurel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Dried Wreaths",
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
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedLaurelWreath() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_laurel_wreath", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_laurel_wreath"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:979", []),
		Noun = "wreath",
		ShortDescription = "a wilted laurel wreath",
		LongDescription = null,
		FullDescription = "Fresh or faded, wilted laurel wreath keeps the uneven look of something made by hand from living material. The binding is visible in places, with a smooth hand-worn polish interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "laurel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Flower Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_laurel_wreath",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshLaurelWreath() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_laurel_wreath", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_laurel_wreath"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1010", []),
		Noun = "wreath",
		ShortDescription = "a fresh laurel wreath",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from laurel leaves laid in a clean repeating line rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh laurel wreath feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "laurel",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Fresh Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_laurel_wreath",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedVioletWristGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_violet_wrist_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1041", []),
		Noun = "garland",
		ShortDescription = "a dried violet wrist garland",
		LongDescription = null,
		FullDescription = "A dried violet wrist garland has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried violet wrist garland leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedVioletWristGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_violet_wrist_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_violet_wrist_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1072", []),
		Noun = "garland",
		ShortDescription = "a wilted violet wrist garland",
		LongDescription = null,
		FullDescription = "Fresh or faded, wilted violet wrist garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The wilted violet wrist garland carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_violet_wrist_garland",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshVioletWristGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_violet_wrist_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_violet_wrist_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1103", []),
		Noun = "garland",
		ShortDescription = "a fresh violet wrist garland",
		LongDescription = null,
		FullDescription = "A fresh violet wrist garland is gathered from violets gathered into small dark-faced clusters, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh violet wrist garland reads as festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "violet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_violet_wrist_garland",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedBlossomAnklet() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_blossom_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1134", []),
		Noun = "anklet",
		ShortDescription = "a dried blossom anklet",
		LongDescription = null,
		FullDescription = "Fresh or faded, dried blossom anklet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a line of tiny punch marks interrupting the line of petals and leaves. The dry pieces rustle faintly rather than bending back into shape. The dried blossom anklet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ankle_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedBlossomAnklet() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_blossom_anklet", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_blossom_anklet"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1165", []),
		Noun = "anklet",
		ShortDescription = "a wilted blossom anklet",
		LongDescription = null,
		FullDescription = "A wilted blossom anklet is gathered from wilted flowers with limp stems and darkening petal tips, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The drooping parts still show where the brighter fresh shape used to be. The wilted blossom anklet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ankle_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_blossom_anklet",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshBlossomAnklet() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_blossom_anklet", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_blossom_anklet"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1196", []),
		Noun = "anklet",
		ShortDescription = "a fresh blossom anklet",
		LongDescription = null,
		FullDescription = "A fresh blossom anklet has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh blossom anklet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "blossom",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ankle_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_blossom_anklet",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedDaisyHeadGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_daisy_head_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1227", []),
		Noun = "garland",
		ShortDescription = "a dried daisy head garland",
		LongDescription = null,
		FullDescription = "A dried daisy head garland is gathered from dried flowers papery at the edges and brittle along the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The dry pieces rustle faintly rather than bending back into shape. The dried daisy head garland leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
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
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedDaisyHeadGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_daisy_head_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_daisy_head_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1258", []),
		Noun = "garland",
		ShortDescription = "a wilted daisy head garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted daisy head garland feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_daisy_head_garland",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshDaisyHeadGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_daisy_head_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_daisy_head_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1289", []),
		Noun = "garland",
		ShortDescription = "a fresh daisy head garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from daisies with pale petals radiating around yellow centres rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh daisy head garland reads as festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "daisy",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_daisy_head_garland",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedLotusGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_lotus_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1320", []),
		Noun = "garland",
		ShortDescription = "a dried lotus garland",
		LongDescription = null,
		FullDescription = "A dried lotus garland has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried lotus garland carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedLotusGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_lotus_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_lotus_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1350", []),
		Noun = "garland",
		ShortDescription = "a wilted lotus garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted lotus garland feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_lotus_garland",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshLotusGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_lotus_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_lotus_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1380", []),
		Noun = "garland",
		ShortDescription = "a fresh lotus garland",
		LongDescription = null,
		FullDescription = "A fresh lotus garland is gathered from lotus petals folded and tied into a broad soft chain, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh lotus garland leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lotus flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_lotus_garland",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedLilyChaplet() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_lily_chaplet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1411", []),
		Noun = "chaplet",
		ShortDescription = "a dried lily chaplet",
		LongDescription = null,
		FullDescription = "Fresh or faded, dried lily chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a softened seam where the maker closed it interrupting the line of petals and leaves. The dry pieces rustle faintly rather than bending back into shape. The dried lily chaplet feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
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
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedLilyChaplet() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_lily_chaplet", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_lily_chaplet"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1441", []),
		Noun = "chaplet",
		ShortDescription = "a wilted lily chaplet",
		LongDescription = null,
		FullDescription = "Fresh or faded, wilted lily chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a smooth hand-worn polish interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
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
		MorphToStableReference = "medieval_jewellery_garland_dried_lily_chaplet",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshLilyChaplet() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_lily_chaplet", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_lily_chaplet"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1471", []),
		Noun = "chaplet",
		ShortDescription = "a fresh lily chaplet",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from lily petals bound carefully so their long curves are not crushed rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh lily chaplet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lily",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_lily_chaplet",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedChamomileWristGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_chamomile_wrist_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1502", []),
		Noun = "garland",
		ShortDescription = "a dried chamomile wrist garland",
		LongDescription = null,
		FullDescription = "A dried chamomile wrist garland is gathered from dried flowers papery at the edges and brittle along the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The dry pieces rustle faintly rather than bending back into shape. The dried chamomile wrist garland reads as temporary festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedChamomileWristGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_chamomile_wrist_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_chamomile_wrist_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1533", []),
		Noun = "garland",
		ShortDescription = "a wilted chamomile wrist garland",
		LongDescription = null,
		FullDescription = "Fresh or faded, wilted chamomile wrist garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_chamomile_wrist_garland",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshChamomileWristGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_chamomile_wrist_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_chamomile_wrist_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1564", []),
		Noun = "garland",
		ShortDescription = "a fresh chamomile wrist garland",
		LongDescription = null,
		FullDescription = "A fresh chamomile wrist garland is gathered from chamomile, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh chamomile wrist garland carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "chamomile",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_chamomile_wrist_garland",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingThinCopperRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_thin_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1595", []),
		Noun = "ring",
		ShortDescription = "a thin copper ring",
		LongDescription = null,
		FullDescription = "A thin copper ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as warm reddish copper darkened in the recesses, with the inner surface left plain and comfortable. Fine wear gathers around the ring's highest places, leaving a faint solder seam easier to pick out. The thin copper ring feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingHammeredBronzeRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_hammered_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1625", []),
		Noun = "ring",
		ShortDescription = "a hammered bronze ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Brown-gold bronze with rubbed high points forms a steady circle, broken only by hammer facets that do not quite line up. Fine wear gathers around the ring's highest places, leaving hammer facets that do not quite line up easier to pick out. The hammered bronze ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingPlainPewterRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_plain_pewter_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1655", []),
		Noun = "ring",
		ShortDescription = "a plain pewter ring",
		LongDescription = null,
		FullDescription = "A plain pewter ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as soft grey pewter with a low satin sheen, with the inner surface left plain and comfortable. Small handling marks cross the plain pewter ring unevenly, especially where it would brush cloth or skin. The plain pewter ring feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingBrassTwistRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_brass_twist_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1685", []),
		Noun = "ring",
		ShortDescription = "a brass twist ring",
		LongDescription = null,
		FullDescription = "At close range, brass twist ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. Two visible strands crossing at the front draws the eye around the band. Close inspection of the brass twist ring shows small tool marks rather than cast-perfect smoothness. The brass twist ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 13.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingNarrowIronRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_narrow_iron_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1715", []),
		Noun = "ring",
		ShortDescription = "a narrow iron ring",
		LongDescription = null,
		FullDescription = "A narrow iron ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as dark wrought iron with a waxed, workmanlike finish, with the inner surface left plain and comfortable. Close inspection of the narrow iron ring shows small tool marks rather than cast-perfect smoothness. The narrow iron ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 11.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingSilverWireRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_silver_wire_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1745", []),
		Noun = "ring",
		ShortDescription = "a silver wire ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by a wire loop wound twice around itself. Small handling marks cross the silver wire ring unevenly, especially where it would brush cloth or skin. The silver wire ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 9.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingSterlingSilverBand() => new()
	{
		Metadata = new("medieval_jewellery_ring_sterling_silver_band", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1775", []),
		Noun = "ring",
		ShortDescription = "a sterling silver band",
		LongDescription = null,
		FullDescription = "A sterling silver band keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as clean, pale silver with a crisp shine, with the inner surface left plain and comfortable. The worked parts of the sterling silver band are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 11.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sterling silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingSilverGiltFingerRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_silver_gilt_finger_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1805", []),
		Noun = "ring",
		ShortDescription = "a silver-gilt finger ring",
		LongDescription = null,
		FullDescription = "At close range, silver-gilt finger ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A worn paler edge beneath the gilding draws the eye around the band. The worked parts of the silver-gilt finger ring are decorative, but none suggest a moving catch or concealed compartment. The silver-gilt finger ring feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 160.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingSmallGoldRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_small_gold_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1835", []),
		Noun = "ring",
		ShortDescription = "a small gold ring",
		LongDescription = null,
		FullDescription = "A small gold ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the small gold ring without making it look neglected. The small gold ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 9.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingElectrumFingerRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_electrum_finger_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1865", []),
		Noun = "ring",
		ShortDescription = "an electrum finger ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale yellow electrum, neither fully silver nor fully gold in colour forms a steady circle, broken only by a pale yellow sheen. Fine wear gathers around the ring's highest places, leaving a pale yellow sheen easier to pick out. The electrum finger ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "electrum",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingCarvedBoneRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_carved_bone_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1895", []),
		Noun = "ring",
		ShortDescription = "a carved bone ring",
		LongDescription = null,
		FullDescription = "A carved bone ring is a compact band of pale bone burnished by handling, made with shallow cross-cut scoring rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving shallow cross-cut scoring easier to pick out. The piece would signal commoner taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingDarkJetRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_dark_jet_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1925", []),
		Noun = "ring",
		ShortDescription = "a dark jet ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Black jet polished until it gives back a quiet reflection forms a steady circle, broken only by a black surface polished like still water. Close inspection of the dark jet ring shows small tool marks rather than cast-perfect smoothness. The dark jet ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingAmberSetBronzeRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_amber_set_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1955", []),
		Noun = "ring",
		ShortDescription = "an amber-set bronze ring",
		LongDescription = null,
		FullDescription = "An amber-set bronze ring is a compact band of brown-gold bronze with rubbed high points, made with a raised amber bead rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Close inspection of the amber-set bronze ring shows small tool marks rather than cast-perfect smoothness. The amber-set bronze ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 16.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingCoralSetSilverRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_coral_set_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:1985", []),
		Noun = "ring",
		ShortDescription = "a coral-set silver ring",
		LongDescription = null,
		FullDescription = "A coral-set silver ring is a compact band of pale silver brightened at its raised surfaces, made with a small coral cabochon rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving a small coral cabochon easier to pick out. The coral-set silver ring feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 14.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingGarnetSetGoldRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_garnet_set_gold_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2015", []),
		Noun = "ring",
		ShortDescription = "a garnet-set gold ring",
		LongDescription = null,
		FullDescription = "A garnet-set gold ring is a compact band of deep yellow gold with a warm, steady shine, made with visible garnet detail worked into the public face rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the garnet-set gold ring unevenly, especially where it would brush cloth or skin. The garnet-set gold ring reads as noble adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 13.0,
		Cost = 420.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingCarnelisilverRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_carnelisilver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2045", []),
		Noun = "ring",
		ShortDescription = "a carnelian silver ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by visible carnelian detail worked into the public face. The hidden side of the carnelian silver ring is plainer, so attention stays on visible carnelian detail worked into the public face and the visible finish. The carnelian silver ring reads as merchant adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 15.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingTurquoiseBrassRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_turquoise_brass_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2075", []),
		Noun = "ring",
		ShortDescription = "a turquoise brass ring",
		LongDescription = null,
		FullDescription = "A turquoise brass ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as yellow brass polished brighter on the edges, with the inner surface left plain and comfortable. The hidden side of the turquoise brass ring is plainer, so attention stays on visible turquoise detail worked into the public face and the visible finish. The turquoise brass ring feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 14.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingRockCrystalRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_rock_crystal_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2105", []),
		Noun = "ring",
		ShortDescription = "a rock crystal ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by visible rock crystal detail worked into the public face. Small handling marks cross the rock crystal ring unevenly, especially where it would brush cloth or skin. The rock crystal ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 14.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingLapisSignetLikeRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_lapis_signet_like_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2135", []),
		Noun = "ring",
		ShortDescription = "a lapis signet-like ring",
		LongDescription = null,
		FullDescription = "A lapis signet-like ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. The worked parts of the lapis signet-like ring are decorative, but none suggest a moving catch or concealed compartment. The lapis signet-like ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingJadeThumbRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_jade_thumb_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2165", []),
		Noun = "ring",
		ShortDescription = "a jade thumb ring",
		LongDescription = null,
		FullDescription = "A jade thumb ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as green jade smoothed to a cool waxy finish, with the inner surface left plain and comfortable. Small handling marks cross the jade thumb ring unevenly, especially where it would brush cloth or skin. The jade thumb ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 22.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingAgateBeadRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_agate_bead_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2195", []),
		Noun = "ring",
		ShortDescription = "an agate bead ring",
		LongDescription = null,
		FullDescription = "An agate bead ring is a compact band of agate, made with natural banding through the stone rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the agate bead ring unevenly, especially where it would brush cloth or skin. The agate bead ring reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "agate",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingBloodstoneSignetRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_bloodstone_signet_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2225", []),
		Noun = "ring",
		ShortDescription = "a bloodstone signet ring",
		LongDescription = null,
		FullDescription = "At close range, bloodstone signet ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A dark flat bezel draws the eye around the band. The worked parts of the bloodstone signet ring are decorative, but none suggest a moving catch or concealed compartment. The bloodstone signet ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 115.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingMoonstoneGoldRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_moonstone_gold_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2255", []),
		Noun = "ring",
		ShortDescription = "a moonstone gold ring",
		LongDescription = null,
		FullDescription = "A moonstone gold ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the moonstone gold ring without making it look neglected. The moonstone gold ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 13.0,
		Cost = 360.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingOnyxSilverRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_onyx_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2285", []),
		Noun = "ring",
		ShortDescription = "an onyx silver ring",
		LongDescription = null,
		FullDescription = "An onyx silver ring is a compact band of pale silver brightened at its raised surfaces, made with visible onyx detail worked into the public face rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. The worked parts of the onyx silver ring are decorative, but none suggest a moving catch or concealed compartment. The onyx silver ring reads as merchant adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 14.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingRoseQuartzLoveRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_rose_quartz_love_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2315", []),
		Noun = "ring",
		ShortDescription = "a rose quartz love ring",
		LongDescription = null,
		FullDescription = "At close range, rose quartz love ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. Visible quartz detail worked into the public face draws the eye around the band. Fine wear gathers around the ring's highest places, leaving visible quartz detail worked into the public face easier to pick out. The rose quartz love ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 13.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingAmethystCourtRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_amethyst_court_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2345", []),
		Noun = "ring",
		ShortDescription = "an amethyst court ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Deep yellow gold with a warm, steady shine forms a steady circle, broken only by visible amethyst detail worked into the public face. Fine wear gathers around the ring's highest places, leaving visible amethyst detail worked into the public face easier to pick out. The amethyst court ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 15.0,
		Cost = 480.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingSapphireCourtRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_sapphire_court_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2375", []),
		Noun = "ring",
		ShortDescription = "a sapphire court ring",
		LongDescription = null,
		FullDescription = "A sapphire court ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Small handling marks cross the sapphire court ring unevenly, especially where it would brush cloth or skin. The sapphire court ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 14.0,
		Cost = 720.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingEmeraldCourtRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_emerald_court_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2405", []),
		Noun = "ring",
		ShortDescription = "an emerald court ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Deep yellow gold with a warm, steady shine forms a steady circle, broken only by visible emerald detail worked into the public face. Small handling marks cross the emerald court ring unevenly, especially where it would brush cloth or skin. The emerald court ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 14.0,
		Cost = 760.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingRubyCourtRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_ruby_court_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2435", []),
		Noun = "ring",
		ShortDescription = "a ruby court ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Deep yellow gold with a warm, steady shine forms a steady circle, broken only by visible ruby detail worked into the public face. The hidden side of the ruby court ring is plainer, so attention stays on visible ruby detail worked into the public face and the visible finish. The ruby court ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 14.0,
		Cost = 780.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingMotifBronzeRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_motif_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2465", []),
		Noun = "ring",
		ShortDescription = "a $motif bronze ring",
		LongDescription = null,
		FullDescription = "A $motif bronze ring is a compact band of brown-gold bronze with rubbed high points, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving the selected motif set into the outward-facing surface easier to pick out. The $motif bronze ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryRingMotifSilverRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_motif_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2496", []),
		Noun = "ring",
		ShortDescription = "a $motif silver ring",
		LongDescription = null,
		FullDescription = "A $motif silver ring is a compact band of pale silver brightened at its raised surfaces, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving the selected motif set into the outward-facing surface easier to pick out. The $motif silver ring feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 75.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryRingMotifSilverGiltRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_motif_silver_gilt_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2527", []),
		Noun = "ring",
		ShortDescription = "a $motif silver-gilt ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Gold-washed silver, brighter on the raised places and paler at worn edges forms a steady circle, broken only by the selected motif set into the outward-facing surface. Fine wear gathers around the ring's highest places, leaving the selected motif set into the outward-facing surface easier to pick out. The $motif silver-gilt ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryRingMotifGoldRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_motif_gold_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2558", []),
		Noun = "ring",
		ShortDescription = "a $motif gold ring",
		LongDescription = null,
		FullDescription = "A $motif gold ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the $motif gold ring without making it look neglected. The $motif gold ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 390.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryRingMotifBrassRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_motif_brass_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2589", []),
		Noun = "ring",
		ShortDescription = "a $motif brass ring",
		LongDescription = null,
		FullDescription = "A $motif brass ring is a compact band of yellow brass polished brighter on the edges, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the $motif brass ring unevenly, especially where it would brush cloth or skin. The $motif brass ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryRingMotifCopperRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_motif_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2620", []),
		Noun = "ring",
		ShortDescription = "a $motif copper ring",
		LongDescription = null,
		FullDescription = "A $motif copper ring is a compact band of warm reddish copper darkened in the recesses, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the $motif copper ring unevenly, especially where it would brush cloth or skin. The $motif copper ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryRingPlainMerchantSignetRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_plain_merchant_signet_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2651", []),
		Noun = "ring",
		ShortDescription = "a plain merchant signet ring",
		LongDescription = null,
		FullDescription = "A plain merchant signet ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the plain merchant signet ring without making it look neglected. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_MerchantSignetRing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional signet row; seal device belongs in skin or component metadata.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingPersonalSealRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_personal_seal_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2682", []),
		Noun = "ring",
		ShortDescription = "a personal seal ring",
		LongDescription = null,
		FullDescription = "A personal seal ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as brown-gold bronze with rubbed high points, with the inner surface left plain and comfortable. The hidden side of the personal seal ring is plainer, so attention stays on a flat face prepared for an impression and the visible finish. The personal seal ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_PersonalSignetRing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional signet row; seal device belongs in skin or component metadata.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingNobleSignetRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_noble_signet_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2713", []),
		Noun = "ring",
		ShortDescription = "a noble signet ring",
		LongDescription = null,
		FullDescription = "At close range, noble signet ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A flat face prepared for an impression draws the eye around the band. Small handling marks cross the noble signet ring unevenly, especially where it would brush cloth or skin. The noble signet ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 520.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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
		BuilderNotes = "Functional signet row; seal device belongs in skin or component metadata.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingSignetOfSilverGilt() => new()
	{
		Metadata = new("medieval_jewellery_ring_signet_of_silver_gilt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2744", []),
		Noun = "ring",
		ShortDescription = "a ring signet of silver-gilt",
		LongDescription = null,
		FullDescription = "A ring signet of silver-gilt is a compact band of gold-washed silver, brighter on the raised places and paler at worn edges, made with a flat face prepared for an impression rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the ring signet of silver-gilt unevenly, especially where it would brush cloth or skin. The ring signet of silver-gilt reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver-gilt",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_RingSignet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional signet row; seal device belongs in skin or component metadata.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingBrassTradeSignetRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_brass_trade_signet_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2775", []),
		Noun = "ring",
		ShortDescription = "a brass trade signet ring",
		LongDescription = null,
		FullDescription = "A brass trade signet ring is a compact band of yellow brass polished brighter on the edges, made with a flat face prepared for an impression rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. The worked parts of the brass trade signet ring are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_MerchantSignetRing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional signet row; seal device belongs in skin or component metadata.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingBronzeHouseholdSignet() => new()
	{
		Metadata = new("medieval_jewellery_ring_bronze_household_signet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2806", []),
		Noun = "ring",
		ShortDescription = "a bronze household signet",
		LongDescription = null,
		FullDescription = "A bronze household signet keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as brown-gold bronze with rubbed high points, with the inner surface left plain and comfortable. Fine wear gathers around the ring's highest places, leaving a flat face prepared for an impression easier to pick out. The bronze household signet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_PersonalSignetRing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Functional signet row; seal device belongs in skin or component metadata.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingChildSizedCopperRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_child_sized_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2837", []),
		Noun = "ring",
		ShortDescription = "a child-sized copper ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Warm reddish copper darkened in the recesses forms a steady circle, broken only by a tiny opening and rounded edge. Close inspection of the child-sized copper ring shows small tool marks rather than cast-perfect smoothness. The child-sized copper ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Children's Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingPairedCourtshipRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_paired_courtship_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2867", []),
		Noun = "ring",
		ShortDescription = "a paired courtship ring",
		LongDescription = null,
		FullDescription = "A paired courtship ring is a compact band of pale silver brightened at its raised surfaces, made with two shallow grooves running side by side rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving two shallow grooves running side by side easier to pick out. The paired courtship ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingKnotworkLoveRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_knotwork_love_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2897", []),
		Noun = "ring",
		ShortDescription = "a knotwork love ring",
		LongDescription = null,
		FullDescription = "A knotwork love ring is a compact band of pale silver brightened at its raised surfaces, made with interlaced lines around the face rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the knotwork love ring unevenly, especially where it would brush cloth or skin. The knotwork love ring reads as merchant adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 11.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingBlackenedNielloRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_blackened_niello_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2927", []),
		Noun = "ring",
		ShortDescription = "a blackened niello ring",
		LongDescription = null,
		FullDescription = "A blackened niello ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. The worked parts of the blackened niello ring are decorative, but none suggest a moving catch or concealed compartment. The blackened niello ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingGildedCopperRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_gilded_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2957", []),
		Noun = "ring",
		ShortDescription = "a gilded copper ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Coppery red metal visible beneath worn gilt edges forms a steady circle, broken only by gold colour worn thin near the palm side. Fine wear gathers around the ring's highest places, leaving gold colour worn thin near the palm side easier to pick out. The gilded copper ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingGildedBronzeRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_gilded_bronze_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:2987", []),
		Noun = "ring",
		ShortDescription = "a gilded bronze ring",
		LongDescription = null,
		FullDescription = "A gilded bronze ring is a compact band of bronze showing through a thin golden surface at nicks and bends, made with a small raised boss on the front rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Close inspection of the gilded bronze ring shows small tool marks rather than cast-perfect smoothness. The gilded bronze ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 13.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gilded bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingMotherOfPearlRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_mother_of_pearl_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3017", []),
		Noun = "ring",
		ShortDescription = "a mother-of-pearl ring",
		LongDescription = null,
		FullDescription = "A mother-of-pearl ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as mother-of-pearl flashing softly under the surface, with the inner surface left plain and comfortable. Close inspection of the mother-of-pearl ring shows small tool marks rather than cast-perfect smoothness. The mother-of-pearl ring reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 8.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mother-of-pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingConchShellRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_conch_shell_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3047", []),
		Noun = "ring",
		ShortDescription = "a conch shell ring",
		LongDescription = null,
		FullDescription = "At close range, conch shell ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. Pink-white shell smoothed into a narrow hoop draws the eye around the band. The worked parts of the conch shell ring are decorative, but none suggest a moving catch or concealed compartment. The conch shell ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "conch shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingCowrieShellRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_cowrie_shell_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3077", []),
		Noun = "ring",
		ShortDescription = "a cowrie shell ring",
		LongDescription = null,
		FullDescription = "At close range, cowrie shell ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A shell cap lashed onto a small band draws the eye around the band. The hidden side of the cowrie shell ring is plainer, so attention stays on a shell cap lashed onto a small band and the visible finish. The cowrie shell ring carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cowrie shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryRingDoubleWireSilverRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_double_wire_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3107", []),
		Noun = "ring",
		ShortDescription = "a double wire silver ring",
		LongDescription = null,
		FullDescription = "A double wire silver ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. Small handling marks cross the double wire silver ring unevenly, especially where it would brush cloth or skin. The piece would signal merchant taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingHeavyGoldThumbRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_heavy_gold_thumb_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3137", []),
		Noun = "ring",
		ShortDescription = "a heavy gold thumb ring",
		LongDescription = null,
		FullDescription = "At close range, heavy gold thumb ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A broad smooth curve draws the eye around the band. Small handling marks cross the heavy gold thumb ring unevenly, especially where it would brush cloth or skin. The heavy gold thumb ring feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 650.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingStampedBrassRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_stamped_brass_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3167", []),
		Noun = "ring",
		ShortDescription = "a stamped brass ring",
		LongDescription = null,
		FullDescription = "A stamped brass ring is a compact band of yellow brass polished brighter on the edges, made with little punched triangles around the band rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Rubbing has brightened the exposed parts of the stamped brass ring without making it look neglected. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryRingFinishSilverRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_finish_silver_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3197", []),
		Noun = "ring",
		ShortDescription = "a $finish silver ring",
		LongDescription = null,
		FullDescription = "A $finish silver ring is a compact band of pale silver brightened at its raised surfaces, made with the chosen finish left deliberately visible on the worked surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving the chosen finish left deliberately visible on the worked surface easier to pick out. The $finish silver ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
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

	public static CatalogueItemDefinition MedievalJewelleryRingShapeGemRing() => new()
	{
		Metadata = new("medieval_jewellery_ring_shape_gem_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3228", []),
		Noun = "ring",
		ShortDescription = "a $shape gem ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by the chosen shape giving the object its main outline. Fine wear gathers around the ring's highest places, leaving the chosen shape giving the object its main outline easier to pick out. The $shape gem ring leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 14.0,
		Cost = 145.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape",
				"Variable_Gem"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryNeckBlueGlassBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_blue_glass_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3260", []),
		Noun = "necklace",
		ShortDescription = "a blue glass bead necklace",
		LongDescription = null,
		FullDescription = "The blue glass bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is coloured glass polished smooth enough to glow at the edges, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. Close inspection of the blue glass bead necklace shows small tool marks rather than cast-perfect smoothness. The piece would signal ordinary market taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryNeckFaienceBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_faience_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3291", []),
		Noun = "necklace",
		ShortDescription = "a faience bead necklace",
		LongDescription = null,
		FullDescription = "The faience bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is glazed faience with small pooling marks in the colour, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The hidden side of the faience bead necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The faience bead necklace reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "faience",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryNeckAmberBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_amber_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3322", []),
		Noun = "necklace",
		ShortDescription = "an amber bead necklace",
		LongDescription = null,
		FullDescription = "The amber bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is honey-coloured amber with cloudy threads caught inside, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The hidden side of the amber bead necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "amber",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckJetBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_jet_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3353", []),
		Noun = "necklace",
		ShortDescription = "a jet bead necklace",
		LongDescription = null,
		FullDescription = "Seen flat, jet bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Rubbing has brightened the exposed parts of the jet bead necklace without making it look neglected. The jet bead necklace leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckCoralBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_coral_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3384", []),
		Noun = "necklace",
		ShortDescription = "a coral bead necklace",
		LongDescription = null,
		FullDescription = "A coral bead necklace is arranged for the throat or upper chest, where its line of coral beads with a warm sea-worn surface would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Close inspection of the coral bead necklace shows small tool marks rather than cast-perfect smoothness. The coral bead necklace feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckBoneBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_bone_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3415", []),
		Noun = "necklace",
		ShortDescription = "a bone bead necklace",
		LongDescription = null,
		FullDescription = "Seen flat, bone bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the bone bead necklace are decorative, but none suggest a moving catch or concealed compartment. The bone bead necklace leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckHornBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_horn_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3446", []),
		Noun = "necklace",
		ShortDescription = "a horn bead necklace",
		LongDescription = null,
		FullDescription = "The horn bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is horn with layered translucent streaks through it, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The worked parts of the horn bead necklace are decorative, but none suggest a moving catch or concealed compartment. The horn bead necklace reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckCowrieShellNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_cowrie_shell_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3477", []),
		Noun = "necklace",
		ShortDescription = "a cowrie shell necklace",
		LongDescription = null,
		FullDescription = "A cowrie shell necklace is arranged for the throat or upper chest, where its line of small cowrie shells with smooth backs and pale mouths would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the cowrie shell necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The cowrie shell necklace reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cowrie shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckConchShellNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_conch_shell_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3508", []),
		Noun = "necklace",
		ShortDescription = "a conch shell necklace",
		LongDescription = null,
		FullDescription = "The conch shell necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is cut conch shell with creamy curves and warmer pinkish edges, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The worked parts of the conch shell necklace are decorative, but none suggest a moving catch or concealed compartment. The conch shell necklace leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "conch shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckMotherOfPearlNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_mother_of_pearl_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3539", []),
		Noun = "necklace",
		ShortDescription = "a mother-of-pearl necklace",
		LongDescription = null,
		FullDescription = "The mother-of-pearl necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is mother-of-pearl flashing softly under the surface, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The hidden side of the mother-of-pearl necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "mother-of-pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckSmallPearlNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_small_pearl_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3570", []),
		Noun = "necklace",
		ShortDescription = "a small pearl necklace",
		LongDescription = null,
		FullDescription = "The small pearl necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is small pearls with uneven but gentle lustre, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The worked parts of the small pearl necklace are decorative, but none suggest a moving catch or concealed compartment. The piece would signal wealthy taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 240.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckLapisBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_lapis_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3601", []),
		Noun = "necklace",
		ShortDescription = "a lapis bead necklace",
		LongDescription = null,
		FullDescription = "Seen flat, lapis bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the lapis bead necklace are decorative, but none suggest a moving catch or concealed compartment. The lapis bead necklace feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lapis lazuli",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckCarnelibeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_carnelibead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3632", []),
		Noun = "necklace",
		ShortDescription = "a carnelian bead necklace",
		LongDescription = null,
		FullDescription = "Seen flat, carnelian bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Close inspection of the carnelian bead necklace shows small tool marks rather than cast-perfect smoothness. The carnelian bead necklace is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carnelian",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckTurquoiseBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_turquoise_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3663", []),
		Noun = "necklace",
		ShortDescription = "a turquoise bead necklace",
		LongDescription = null,
		FullDescription = "A turquoise bead necklace carries its value in the rhythm of its material: blue-green turquoise with fine darker lines. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the necklace's highest places, leaving beads chosen more for visible rhythm than identical size easier to pick out. The turquoise bead necklace reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "turquoise",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckJadeBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_jade_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3694", []),
		Noun = "necklace",
		ShortDescription = "a jade bead necklace",
		LongDescription = null,
		FullDescription = "A jade bead necklace is arranged for the throat or upper chest, where its line of green jade smoothed to a cool waxy finish would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the jade bead necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The jade bead necklace reads as wealthy adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 210.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckRockCrystalNecklace() => new()
	{
		Metadata = new("medieval_jewellery_neck_rock_crystal_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3725", []),
		Noun = "necklace",
		ShortDescription = "a rock crystal necklace",
		LongDescription = null,
		FullDescription = "Seen flat, rock crystal necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The hidden side of the rock crystal necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The rock crystal necklace carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "rock crystal",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckAgateBeadString() => new()
	{
		Metadata = new("medieval_jewellery_neck_agate_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3756", []),
		Noun = "necklace",
		ShortDescription = "an agate bead string",
		LongDescription = null,
		FullDescription = "An agate bead string carries its value in the rhythm of its material: agate. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Small handling marks cross the agate bead string unevenly, especially where it would brush cloth or skin. The agate bead string feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "agate",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckJasperBeadString() => new()
	{
		Metadata = new("medieval_jewellery_neck_jasper_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3787", []),
		Noun = "necklace",
		ShortDescription = "a jasper bead string",
		LongDescription = null,
		FullDescription = "A jasper bead string carries its value in the rhythm of its material: jasper. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Close inspection of the jasper bead string shows small tool marks rather than cast-perfect smoothness. The jasper bead string feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jasper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckChalcedonyBeadString() => new()
	{
		Metadata = new("medieval_jewellery_neck_chalcedony_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3818", []),
		Noun = "necklace",
		ShortDescription = "a chalcedony bead string",
		LongDescription = null,
		FullDescription = "A chalcedony bead string is arranged for the throat or upper chest, where its line of chalcedony would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Close inspection of the chalcedony bead string shows small tool marks rather than cast-perfect smoothness. The chalcedony bead string reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "chalcedony",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckQuartzBeadString() => new()
	{
		Metadata = new("medieval_jewellery_neck_quartz_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3849", []),
		Noun = "necklace",
		ShortDescription = "a quartz bead string",
		LongDescription = null,
		FullDescription = "The quartz bead string falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is quartz, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. Fine wear gathers around the necklace's highest places, leaving beads chosen more for visible rhythm than identical size easier to pick out. The quartz bead string feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "quartz",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
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

	public static CatalogueItemDefinition MedievalJewelleryNeckPlainBronzeChain() => new()
	{
		Metadata = new("medieval_jewellery_neck_plain_bronze_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3880", []),
		Noun = "chain",
		ShortDescription = "a plain bronze chain",
		LongDescription = null,
		FullDescription = "Seen flat, plain bronze chain shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Close inspection of the plain bronze chain shows small tool marks rather than cast-perfect smoothness. The plain bronze chain reads as ordinary market adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Standard Jewellery"
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

	public static CatalogueItemDefinition MedievalJewelleryNeckNarrowSilverChain() => new()
	{
		Metadata = new("medieval_jewellery_neck_narrow_silver_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3910", []),
		Noun = "chain",
		ShortDescription = "a narrow silver chain",
		LongDescription = null,
		FullDescription = "The narrow silver chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with links or knots set at slightly irregular intervals giving the eye a place to pause. Close inspection of the narrow silver chain shows small tool marks rather than cast-perfect smoothness. The narrow silver chain leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 48.0,
		Cost = 115.0m,
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

	public static CatalogueItemDefinition MedievalJewelleryNeckGoldLinkChain() => new()
	{
		Metadata = new("medieval_jewellery_neck_gold_link_chain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:3940", []),
		Noun = "chain",
		ShortDescription = "a gold link chain",
		LongDescription = null,
		FullDescription = "A gold link chain is arranged for the throat or upper chest, where its line of deep yellow gold with a warm, steady shine would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Rubbing has brightened the exposed parts of the gold link chain without making it look neglected. The gold link chain carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 62.0,
		Cost = 520.0m,
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
}
