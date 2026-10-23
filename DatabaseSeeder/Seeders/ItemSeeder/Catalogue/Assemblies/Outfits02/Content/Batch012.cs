#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits02Stock
{
	public static CatalogueOutfitDefinition MedievalOutfit0112() => new(
		Metadata: new("medieval_outfit_0112", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_latin_black_clerical_gown", "item:medieval_linen_coif", "item:medieval_linen_undertunic", "item:medieval_soft_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Latin Christian parish cleric",
		Description: "Builder-facing medieval clothing outfit for Latin Christian parish cleric.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_black_clerical_gown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_coif", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0113() => new(
		Metadata: new("medieval_outfit_0113", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_latin_amice", "item:medieval_latin_chasuble", "item:medieval_latin_linen_cincture", "item:medieval_latin_maniple", "item:medieval_latin_stole", "item:medieval_latin_white_alb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Latin Christian priest at Mass",
		Description: "Builder-facing medieval clothing outfit for Latin Christian priest at Mass.",
		Items:
		[
			new("medieval_latin_amice", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_white_alb", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_linen_cincture", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_stole", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_maniple", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_chasuble", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0114() => new(
		Metadata: new("medieval_outfit_0114", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_latin_amice", "item:medieval_latin_dalmatic_vestment", "item:medieval_latin_linen_cincture", "item:medieval_latin_maniple", "item:medieval_latin_stole", "item:medieval_latin_white_alb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Latin Christian deacon at Mass",
		Description: "Builder-facing medieval clothing outfit for Latin Christian deacon at Mass.",
		Items:
		[
			new("medieval_latin_amice", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_white_alb", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_linen_cincture", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_stole", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_maniple", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_dalmatic_vestment", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0115() => new(
		Metadata: new("medieval_outfit_0115", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_latin_amice", "item:medieval_latin_bishop_gloves", "item:medieval_latin_bishop_mitre", "item:medieval_latin_chasuble", "item:medieval_latin_linen_cincture", "item:medieval_latin_processional_cope", "item:medieval_latin_stole", "item:medieval_latin_white_alb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Latin Christian bishop in pontificals",
		Description: "Builder-facing medieval clothing outfit for Latin Christian bishop in pontificals.",
		Items:
		[
			new("medieval_latin_amice", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_white_alb", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_linen_cincture", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_stole", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_chasuble", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_processional_cope", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_bishop_mitre", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_bishop_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0116() => new(
		Metadata: new("medieval_outfit_0116", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_latin_amice", "item:medieval_latin_archbishop_pallium", "item:medieval_latin_bishop_gloves", "item:medieval_latin_bishop_mitre", "item:medieval_latin_chasuble", "item:medieval_latin_linen_cincture", "item:medieval_latin_processional_cope", "item:medieval_latin_stole", "item:medieval_latin_white_alb"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Latin Christian archbishop in pallium vestments",
		Description: "Builder-facing medieval clothing outfit for Latin Christian archbishop in pallium vestments.",
		Items:
		[
			new("medieval_latin_amice", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_white_alb", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_linen_cincture", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_latin_stole", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_chasuble", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_processional_cope", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_bishop_mitre", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_bishop_gloves", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_latin_archbishop_pallium", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0117() => new(
		Metadata: new("medieval_outfit_0117", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_eastern_black_klobuk", "item:medieval_eastern_black_riassa", "item:medieval_eastern_monastic_mantle", "item:medieval_linen_undertunic", "item:medieval_soft_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Eastern Christian monk",
		Description: "Builder-facing medieval clothing outfit for Eastern Christian monk.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_black_riassa", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_eastern_monastic_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_black_klobuk", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0118() => new(
		Metadata: new("medieval_outfit_0118", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_eastern_black_riassa", "item:medieval_eastern_monastic_mantle", "item:medieval_eastern_womens_monastic_veil", "item:medieval_linen_chemise", "item:medieval_soft_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Eastern Christian nun",
		Description: "Builder-facing medieval clothing outfit for Eastern Christian nun.",
		Items:
		[
			new("medieval_linen_chemise", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_black_riassa", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_eastern_monastic_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_womens_monastic_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0119() => new(
		Metadata: new("medieval_outfit_0119", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_eastern_epimanikia", "item:medieval_eastern_orarion", "item:medieval_eastern_sticharion", "item:medieval_soft_leather_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Eastern Christian deacon",
		Description: "Builder-facing medieval clothing outfit for Eastern Christian deacon.",
		Items:
		[
			new("medieval_eastern_sticharion", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_eastern_orarion", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_epimanikia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0120() => new(
		Metadata: new("medieval_outfit_0120", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_eastern_epimanikia", "item:medieval_eastern_epitrachelion", "item:medieval_eastern_kamilavkion", "item:medieval_eastern_phelonion", "item:medieval_eastern_sticharion"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Eastern Christian priest",
		Description: "Builder-facing medieval clothing outfit for Eastern Christian priest.",
		Items:
		[
			new("medieval_eastern_sticharion", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_eastern_epitrachelion", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_epimanikia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_phelonion", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_kamilavkion", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0121() => new(
		Metadata: new("medieval_outfit_0121", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_eastern_epimanikia", "item:medieval_eastern_epitrachelion", "item:medieval_eastern_kamilavkion", "item:medieval_eastern_omophorion", "item:medieval_eastern_sakkos", "item:medieval_eastern_sticharion"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Eastern Christian bishop",
		Description: "Builder-facing medieval clothing outfit for Eastern Christian bishop.",
		Items:
		[
			new("medieval_eastern_sticharion", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_eastern_epitrachelion", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_epimanikia", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_sakkos", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_omophorion", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_eastern_kamilavkion", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0122() => new(
		Metadata: new("medieval_outfit_0122", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_cotton_sirwal", "item:medieval_islamic_plain_imam_qamis", "item:medieval_islamic_plain_imam_turban", "item:medieval_plain_leather_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Islamic imam / khatib",
		Description: "Builder-facing medieval clothing outfit for Islamic imam / khatib.",
		Items:
		[
			new("medieval_cotton_sirwal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_islamic_plain_imam_qamis", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_islamic_plain_imam_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0123() => new(
		Metadata: new("medieval_outfit_0123", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_cotton_sirwal", "item:medieval_islamic_qadi_turban", "item:medieval_islamic_scholars_jubba", "item:medieval_islamic_taylasan", "item:medieval_linen_qamis", "item:medieval_soft_leather_slippers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Islamic qadi / madrasa scholar",
		Description: "Builder-facing medieval clothing outfit for Islamic qadi / madrasa scholar.",
		Items:
		[
			new("medieval_cotton_sirwal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_linen_qamis", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_islamic_scholars_jubba", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_islamic_taylasan", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_islamic_qadi_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_slippers", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0124() => new(
		Metadata: new("medieval_outfit_0124", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_cotton_sirwal", "item:medieval_linen_qamis", "item:medieval_sufi_felt_cap", "item:medieval_sufi_patched_khirqa", "item:medieval_sufi_wool_sash"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Islamic Sufi dervish",
		Description: "Builder-facing medieval clothing outfit for Islamic Sufi dervish.",
		Items:
		[
			new("medieval_linen_qamis", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_cotton_sirwal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_sufi_patched_khirqa", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_sufi_wool_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_sufi_felt_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0125() => new(
		Metadata: new("medieval_outfit_0125", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_cotton_qamis", "item:medieval_cotton_sirwal", "item:medieval_islamic_plain_imam_turban", "item:medieval_islamic_scholars_jubba", "item:medieval_sufi_coarse_wool_cloak"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Islamic Sufi sheikh",
		Description: "Builder-facing medieval clothing outfit for Islamic Sufi sheikh.",
		Items:
		[
			new("medieval_cotton_qamis", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_cotton_sirwal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_islamic_scholars_jubba", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_sufi_coarse_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_islamic_plain_imam_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0126() => new(
		Metadata: new("medieval_outfit_0126", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_scholars_robe", "item:medieval_jewish_skullcap", "item:medieval_jewish_tallit_gadol", "item:medieval_linen_undertunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Jewish synagogue scholar",
		Description: "Builder-facing medieval clothing outfit for Jewish synagogue scholar.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_jewish_scholars_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_jewish_skullcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_jewish_tallit_gadol", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour1=white colour2=black")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0127() => new(
		Metadata: new("medieval_outfit_0127", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_jewish_skullcap", "item:medieval_jewish_tallit_gadol", "item:medieval_jewish_tallit_katan", "item:medieval_linen_undertunic", "item:medieval_simple_woven_sash"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Jewish prayer leader",
		Description: "Builder-facing medieval clothing outfit for Jewish prayer leader.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_jewish_tallit_katan", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_jewish_tallit_gadol", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour1=white colour2=black"),
			new("medieval_jewish_skullcap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0128() => new(
		Metadata: new("medieval_outfit_0128", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_hindu_white_priest_dhoti", "item:medieval_hindu_white_uttariya", "item:medieval_plain_leather_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Hindu temple priest",
		Description: "Builder-facing medieval clothing outfit for Hindu temple priest.",
		Items:
		[
			new("medieval_hindu_white_priest_dhoti", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_hindu_white_uttariya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0129() => new(
		Metadata: new("medieval_outfit_0129", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_hindu_kaupina", "item:medieval_hindu_ochre_ascetic_wrap", "item:medieval_hindu_ochre_shoulder_cloth", "item:medieval_plain_leather_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Hindu ascetic",
		Description: "Builder-facing medieval clothing outfit for Hindu ascetic.",
		Items:
		[
			new("medieval_hindu_kaupina", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_hindu_ochre_ascetic_wrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_hindu_ochre_shoulder_cloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0130() => new(
		Metadata: new("medieval_outfit_0130", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_jain_white_ascetic_robe", "item:medieval_jain_white_shoulder_wrap", "item:medieval_plain_leather_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Jain Śvetāmbara monk",
		Description: "Builder-facing medieval clothing outfit for Jain Śvetāmbara monk.",
		Items:
		[
			new("medieval_jain_white_ascetic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_jain_white_shoulder_wrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0131() => new(
		Metadata: new("medieval_outfit_0131", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_jain_white_ascetic_robe", "item:medieval_jain_white_shoulder_wrap", "item:medieval_linen_veil", "item:medieval_plain_leather_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Jain Śvetāmbara nun",
		Description: "Builder-facing medieval clothing outfit for Jain Śvetāmbara nun.",
		Items:
		[
			new("medieval_jain_white_ascetic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_jain_white_shoulder_wrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_linen_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_plain_leather_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0132() => new(
		Metadata: new("medieval_outfit_0132", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_buddhist_patched_kasaya", "item:medieval_buddhist_plain_underrobe", "item:medieval_hemp_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Buddhist monk",
		Description: "Builder-facing medieval clothing outfit for Buddhist monk.",
		Items:
		[
			new("medieval_buddhist_plain_underrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_patched_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_hemp_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0133() => new(
		Metadata: new("medieval_outfit_0133", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_buddhist_nun_robe", "item:medieval_buddhist_patched_kasaya", "item:medieval_hemp_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Buddhist nun",
		Description: "Builder-facing medieval clothing outfit for Buddhist nun.",
		Items:
		[
			new("medieval_buddhist_nun_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_patched_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_hemp_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0134() => new(
		Metadata: new("medieval_outfit_0134", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_buddhist_formal_kesa", "item:medieval_buddhist_plain_underrobe", "item:medieval_buddhist_travelling_mantle", "item:medieval_hemp_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Buddhist formal officiant",
		Description: "Builder-facing medieval clothing outfit for Buddhist formal officiant.",
		Items:
		[
			new("medieval_buddhist_plain_underrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_formal_kesa", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_travelling_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_hemp_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0135() => new(
		Metadata: new("medieval_outfit_0135", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_buddhist_black_monastic_robe", "item:medieval_buddhist_patched_kasaya", "item:medieval_buddhist_travelling_mantle", "item:medieval_hemp_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Buddhist travelling monk",
		Description: "Builder-facing medieval clothing outfit for Buddhist travelling monk.",
		Items:
		[
			new("medieval_buddhist_black_monastic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_patched_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_travelling_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_hemp_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0136() => new(
		Metadata: new("medieval_outfit_0136", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_daoist_cloud_sleeved_robe", "item:medieval_daoist_cross_collar_robe", "item:medieval_daoist_ritual_cap", "item:medieval_song_cloth_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Daoist priest",
		Description: "Builder-facing medieval clothing outfit for Daoist priest.",
		Items:
		[
			new("medieval_song_cloth_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_daoist_cross_collar_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_daoist_cloud_sleeved_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_daoist_ritual_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0137() => new(
		Metadata: new("medieval_outfit_0137", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_daoist_cross_collar_robe", "item:medieval_daoist_ritual_cap", "item:medieval_song_cloth_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Daoist nun",
		Description: "Builder-facing medieval clothing outfit for Daoist nun.",
		Items:
		[
			new("medieval_song_cloth_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_daoist_cross_collar_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_daoist_ritual_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0138() => new(
		Metadata: new("medieval_outfit_0138", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_hemp_sandals", "item:medieval_shinto_priest_eboshi", "item:medieval_shinto_priest_hakama", "item:medieval_shinto_white_joe_robe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Shinto priest",
		Description: "Builder-facing medieval clothing outfit for Shinto priest.",
		Items:
		[
			new("medieval_shinto_white_joe_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_shinto_priest_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_shinto_priest_eboshi", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_hemp_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0139() => new(
		Metadata: new("medieval_outfit_0139", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_hemp_sandals", "item:medieval_shinto_miko_red_hakama", "item:medieval_shinto_miko_white_kosode"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Shrine attendant",
		Description: "Builder-facing medieval clothing outfit for Shrine attendant.",
		Items:
		[
			new("medieval_shinto_miko_white_kosode", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_shinto_miko_red_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=red"),
			new("medieval_hemp_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0140() => new(
		Metadata: new("medieval_outfit_0140", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_high_leather_boots", "item:medieval_steppe_ritual_felt_coat", "item:medieval_steppe_ritual_fur_cap", "item:medieval_steppe_ritual_sash", "item:medieval_wool_deel"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Steppe ritual specialist",
		Description: "Builder-facing medieval clothing outfit for Steppe ritual specialist.",
		Items:
		[
			new("medieval_wool_deel", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_steppe_ritual_felt_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_steppe_ritual_fur_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_steppe_ritual_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_high_leather_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition MedievalOutfit0141() => new(
		Metadata: new("medieval_outfit_0141", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:medieval_annular_bronze_brooch", "item:medieval_broad_brim_felt_hat", "item:medieval_linen_undertunic", "item:medieval_soft_leather_shoes", "item:medieval_travel_wool_cloak", "item:medieval_travel_wool_gaiters", "item:medieval_wool_hose", "item:medieval_wool_tunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/MedievalOutfitManifestSpecs", []),
		Name: "Medieval Shared western road traveller",
		Description: "Builder-facing medieval clothing outfit for Shared western road traveller.",
		Items:
		[
			new("medieval_linen_undertunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_wool_hose", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_soft_leather_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_travel_wool_gaiters", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_travel_wool_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_broad_brim_felt_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_annular_bronze_brooch", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
