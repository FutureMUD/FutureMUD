#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits01Stock
{
	public static CatalogueOutfitDefinition AntiquityOutfit0001() => new(
		Metadata: new("antiquity_outfit_0001", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_linen_loincloth", "item:antiquity_plain_leather_belt", "item:antiquity_rectangular_wool_cloak", "item:antiquity_sleeved_common_wool_tunic", "item:antiquity_soft_leather_shoes", "item:antiquity_wool_braccae"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Celtic common male",
		Description: "Builder-facing antiquity clothing outfit for Celtic common male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_sleeved_common_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_braccae", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_rectangular_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0002() => new(
		Metadata: new("antiquity_outfit_0002", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_ankle_leather_boots", "item:antiquity_bronze_buckled_leather_belt", "item:antiquity_fine_bordered_wool_tunic", "item:antiquity_fine_checked_wool_cloak", "item:antiquity_fine_wool_braccae", "item:antiquity_linen_loincloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Celtic elite male",
		Description: "Builder-facing antiquity clothing outfit for Celtic elite male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_bronze_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_bordered_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_wool_braccae", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_checked_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0003() => new(
		Metadata: new("antiquity_outfit_0003", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_broad_wool_mantle", "item:antiquity_linen_breastband", "item:antiquity_long_sleeved_wool_tunic", "item:antiquity_plain_leather_belt", "item:antiquity_soft_leather_shoes", "item:antiquity_wool_wrap_skirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Celtic common female",
		Description: "Builder-facing antiquity clothing outfit for Celtic common female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_long_sleeved_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_wrap_skirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_broad_wool_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0004() => new(
		Metadata: new("antiquity_outfit_0004", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_ankle_leather_boots", "item:antiquity_fine_bordered_wool_mantle", "item:antiquity_fine_sleeved_wool_gown", "item:antiquity_fine_woven_sash", "item:antiquity_linen_breastband", "item:antiquity_linen_shoulder_veil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Celtic elite female",
		Description: "Builder-facing antiquity clothing outfit for Celtic elite female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_sleeved_wool_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_bordered_wool_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_linen_shoulder_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0005() => new(
		Metadata: new("antiquity_outfit_0005", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_ankle_leather_boots", "item:antiquity_heavy_wool_cloak", "item:antiquity_linen_loincloth", "item:antiquity_narrow_wool_trousers", "item:antiquity_plain_leather_belt", "item:antiquity_straight_wool_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Germanic common male",
		Description: "Builder-facing antiquity clothing outfit for Germanic common male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_straight_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_narrow_wool_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_heavy_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0006() => new(
		Metadata: new("antiquity_outfit_0006", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_ankle_leather_boots", "item:antiquity_bronze_buckled_leather_belt", "item:antiquity_fine_banded_wool_tunic", "item:antiquity_fine_tapered_wool_trousers", "item:antiquity_fur_lined_wool_cloak", "item:antiquity_linen_loincloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Germanic elite male",
		Description: "Builder-facing antiquity clothing outfit for Germanic elite male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_bronze_buckled_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_banded_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_tapered_wool_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fur_lined_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0007() => new(
		Metadata: new("antiquity_outfit_0007", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_ankle_leather_boots", "item:antiquity_checked_wool_scarf", "item:antiquity_linen_breastband", "item:antiquity_long_straight_wool_tunic", "item:antiquity_overlapping_wool_skirt", "item:antiquity_plain_leather_belt", "item:antiquity_woolly_skin_cape"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Germanic common female",
		Description: "Builder-facing antiquity clothing outfit for Germanic common female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_long_straight_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_overlapping_wool_skirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_checked_wool_scarf", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_woolly_skin_cape", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0008() => new(
		Metadata: new("antiquity_outfit_0008", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_ankle_leather_boots", "item:antiquity_fine_heavy_wool_mantle", "item:antiquity_fine_long_wool_gown", "item:antiquity_fine_woven_sash", "item:antiquity_linen_breastband", "item:antiquity_linen_head_veil"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Germanic elite female",
		Description: "Builder-facing antiquity clothing outfit for Germanic elite female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_ankle_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_long_wool_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_heavy_wool_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_linen_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0009() => new(
		Metadata: new("antiquity_outfit_0009", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_knee_length_wool_tunica", "item:antiquity_linen_loincloth", "item:antiquity_plain_leather_belt", "item:antiquity_plain_leather_sandals", "item:antiquity_wool_travel_mantle"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Italic common male",
		Description: "Builder-facing antiquity clothing outfit for Italic common male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_knee_length_wool_tunica", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_travel_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0010() => new(
		Metadata: new("antiquity_outfit_0010", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_leather_sandals", "item:antiquity_fine_linen_tunica", "item:antiquity_linen_loincloth", "item:antiquity_wool_toga"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Italic elite male",
		Description: "Builder-facing antiquity clothing outfit for Italic elite male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_linen_tunica", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_toga", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0011() => new(
		Metadata: new("antiquity_outfit_0011", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_linen_breastband", "item:antiquity_long_wool_tunica", "item:antiquity_plain_leather_sandals", "item:antiquity_simple_woven_sash", "item:antiquity_wool_palla"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Italic common female",
		Description: "Builder-facing antiquity clothing outfit for Italic common female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_long_wool_tunica", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_palla", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0012() => new(
		Metadata: new("antiquity_outfit_0012", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_leather_sandals", "item:antiquity_fine_long_linen_tunica", "item:antiquity_fine_wool_palla", "item:antiquity_fine_woven_sash", "item:antiquity_linen_breastband", "item:antiquity_wool_stola"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Italic elite female",
		Description: "Builder-facing antiquity clothing outfit for Italic elite female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_long_linen_tunica", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_stola", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_wool_palla", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0013() => new(
		Metadata: new("antiquity_outfit_0013", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_linen_loincloth", "item:antiquity_plain_leather_sandals", "item:antiquity_short_wool_chiton", "item:antiquity_simple_woven_sash", "item:antiquity_wool_himation"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Hellenic common male",
		Description: "Builder-facing antiquity clothing outfit for Hellenic common male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_short_wool_chiton", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_himation", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0014() => new(
		Metadata: new("antiquity_outfit_0014", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_leather_sandals", "item:antiquity_fine_linen_chiton", "item:antiquity_fine_wool_himation", "item:antiquity_fine_woven_sash", "item:antiquity_linen_loincloth", "item:antiquity_short_wool_chlamys"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Hellenic elite male",
		Description: "Builder-facing antiquity clothing outfit for Hellenic elite male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_linen_chiton", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_wool_himation", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_short_wool_chlamys", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0015() => new(
		Metadata: new("antiquity_outfit_0015", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_full_length_wool_peplos", "item:antiquity_full_wool_himation", "item:antiquity_linen_breastband", "item:antiquity_plain_leather_sandals", "item:antiquity_simple_woven_sash"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Hellenic common female",
		Description: "Builder-facing antiquity clothing outfit for Hellenic common female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_full_length_wool_peplos", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_full_wool_himation", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0016() => new(
		Metadata: new("antiquity_outfit_0016", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_full_wool_himation", "item:antiquity_fine_leather_sandals", "item:antiquity_fine_long_linen_chiton", "item:antiquity_fine_woven_sash", "item:antiquity_light_linen_head_veil", "item:antiquity_linen_breastband"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Hellenic elite female",
		Description: "Builder-facing antiquity clothing outfit for Hellenic elite female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_long_linen_chiton", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_full_wool_himation", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_light_linen_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0017() => new(
		Metadata: new("antiquity_outfit_0017", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_conical_felt_cap", "item:antiquity_linen_loincloth", "item:antiquity_plain_leather_belt", "item:antiquity_plain_leather_sandals", "item:antiquity_short_fitted_linen_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Phoenician/Punic-inspired common male",
		Description: "Builder-facing antiquity clothing outfit for Phoenician/Punic-inspired common male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_short_fitted_linen_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_conical_felt_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0018() => new(
		Metadata: new("antiquity_outfit_0018", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_conical_felt_cap", "item:antiquity_fine_front_knotted_girdle", "item:antiquity_fine_leather_sandals", "item:antiquity_linen_loincloth", "item:antiquity_long_linen_inner_robe", "item:antiquity_one_shoulder_wool_mantle", "item:antiquity_patterned_linen_waistcloth", "item:antiquity_short_sleeved_linen_overblouse"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Phoenician/Punic-inspired elite male",
		Description: "Builder-facing antiquity clothing outfit for Phoenician/Punic-inspired elite male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_front_knotted_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_patterned_linen_waistcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_short_sleeved_linen_overblouse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_long_linen_inner_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_one_shoulder_wool_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_conical_felt_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0019() => new(
		Metadata: new("antiquity_outfit_0019", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_conical_felt_cap", "item:antiquity_fine_front_knotted_girdle", "item:antiquity_fine_papyrus_sandals", "item:antiquity_linen_loincloth", "item:antiquity_one_shoulder_wool_mantle", "item:antiquity_star_bordered_linen_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Phoenician/Punic-inspired ritual or temple male variant",
		Description: "Builder-facing antiquity clothing outfit for Phoenician/Punic-inspired ritual or temple male variant.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_papyrus_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_front_knotted_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_star_bordered_linen_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_one_shoulder_wool_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_conical_felt_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0020() => new(
		Metadata: new("antiquity_outfit_0020", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_front_knotted_girdle", "item:antiquity_linen_breastband", "item:antiquity_long_folded_linen_robe", "item:antiquity_loose_linen_hood", "item:antiquity_plain_leather_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Phoenician/Punic-inspired common female",
		Description: "Builder-facing antiquity clothing outfit for Phoenician/Punic-inspired common female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_front_knotted_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_long_folded_linen_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_loose_linen_hood", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0021() => new(
		Metadata: new("antiquity_outfit_0021", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_front_knotted_girdle", "item:antiquity_fine_full_linen_gown", "item:antiquity_fine_leather_sandals", "item:antiquity_left_shoulder_overdrape", "item:antiquity_linen_breastband", "item:antiquity_loose_linen_hood"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Phoenician/Punic-inspired elite female",
		Description: "Builder-facing antiquity clothing outfit for Phoenician/Punic-inspired elite female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_front_knotted_girdle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_full_linen_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_left_shoulder_overdrape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_loose_linen_hood", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0022() => new(
		Metadata: new("antiquity_outfit_0022", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_anaxyrides_wool_trousers", "item:antiquity_linen_loincloth", "item:antiquity_rounded_felt_cap", "item:antiquity_sarapis_wool_tunic", "item:antiquity_soft_leather_riding_boots", "item:antiquity_wide_cloth_belt", "item:antiquity_wool_kandys"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Persian-inspired riding male",
		Description: "Builder-facing antiquity clothing outfit for Persian-inspired riding male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_soft_leather_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wide_cloth_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_sarapis_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_anaxyrides_wool_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_kandys", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_rounded_felt_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0023() => new(
		Metadata: new("antiquity_outfit_0023", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_patterned_anaxyrides", "item:antiquity_fine_sarapis_linen_tunic", "item:antiquity_fine_wide_cloth_belt", "item:antiquity_fine_wool_kandys", "item:antiquity_fluted_felt_hat", "item:antiquity_linen_loincloth", "item:antiquity_low_strapped_leather_shoes", "item:antiquity_pleated_court_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Persian-inspired court elite male",
		Description: "Builder-facing antiquity clothing outfit for Persian-inspired court elite male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_low_strapped_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_wide_cloth_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_sarapis_linen_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_patterned_anaxyrides", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_pleated_court_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_wool_kandys", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fluted_felt_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0024() => new(
		Metadata: new("antiquity_outfit_0024", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_anaxyrides_wool_trousers", "item:antiquity_linen_breastband", "item:antiquity_sarapis_wool_tunic", "item:antiquity_soft_leather_riding_boots", "item:antiquity_tall_kyrbasia", "item:antiquity_wide_cloth_belt", "item:antiquity_wool_kandys"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Persian-inspired riding female",
		Description: "Builder-facing antiquity clothing outfit for Persian-inspired riding female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_soft_leather_riding_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wide_cloth_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_sarapis_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_anaxyrides_wool_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_kandys", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_tall_kyrbasia", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0025() => new(
		Metadata: new("antiquity_outfit_0025", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_pleated_court_gown", "item:antiquity_fine_wide_cloth_belt", "item:antiquity_fine_wool_kandys", "item:antiquity_full_head_and_neck_veil", "item:antiquity_linen_breastband", "item:antiquity_low_strapped_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Persian-inspired court elite female",
		Description: "Builder-facing antiquity clothing outfit for Persian-inspired court elite female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_low_strapped_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_wide_cloth_belt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_pleated_court_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_wool_kandys", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_full_head_and_neck_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0026() => new(
		Metadata: new("antiquity_outfit_0026", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_linen_loincloth", "item:antiquity_papyrus_sandals", "item:antiquity_simple_linen_shendyt", "item:antiquity_wrapped_linen_headcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Egyptian-inspired common male",
		Description: "Builder-facing antiquity clothing outfit for Egyptian-inspired common male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_papyrus_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_simple_linen_shendyt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wrapped_linen_headcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0027() => new(
		Metadata: new("antiquity_outfit_0027", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_linen_headcloth", "item:antiquity_fine_papyrus_sandals", "item:antiquity_glass_usekh_collar", "item:antiquity_linen_loincloth", "item:antiquity_pleated_linen_shendyt", "item:antiquity_sheer_linen_overshirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Egyptian-inspired elite male",
		Description: "Builder-facing antiquity clothing outfit for Egyptian-inspired elite male.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_papyrus_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_pleated_linen_shendyt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_sheer_linen_overshirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_linen_headcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_glass_usekh_collar", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0028() => new(
		Metadata: new("antiquity_outfit_0028", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_linen_breastband", "item:antiquity_linen_shoulder_shawl", "item:antiquity_papyrus_sandals", "item:antiquity_straight_linen_kalasiris", "item:antiquity_wrapped_linen_headcloth"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Egyptian-inspired common female",
		Description: "Builder-facing antiquity clothing outfit for Egyptian-inspired common female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_papyrus_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_straight_linen_kalasiris", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_linen_shoulder_shawl", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wrapped_linen_headcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0029() => new(
		Metadata: new("antiquity_outfit_0029", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_fine_linen_headcloth", "item:antiquity_fine_papyrus_sandals", "item:antiquity_fine_pleated_kalasiris", "item:antiquity_fine_sheer_linen_cape", "item:antiquity_glass_usekh_collar", "item:antiquity_linen_breastband"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Egyptian-inspired elite female",
		Description: "Builder-facing antiquity clothing outfit for Egyptian-inspired elite female.",
		Items:
		[
			new("antiquity_linen_breastband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_papyrus_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_pleated_kalasiris", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_sheer_linen_cape", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_fine_linen_headcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_glass_usekh_collar", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition AntiquityOutfit0030() => new(
		Metadata: new("antiquity_outfit_0030", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:antiquity_linen_loincloth", "item:antiquity_plain_leather_sandals", "item:antiquity_simple_woven_sash", "item:antiquity_wool_exomis"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/AntiquityOutfitManifestSpecs", []),
		Name: "Antiquity Hellenic workshop labourer",
		Description: "Admission: Hellenic urban, dock, mine, kiln, building, and workshop contexts. Purpose: labour dress built around the existing exomis rather than another generic chiton.",
		Items:
		[
			new("antiquity_linen_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_wool_exomis", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("antiquity_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
