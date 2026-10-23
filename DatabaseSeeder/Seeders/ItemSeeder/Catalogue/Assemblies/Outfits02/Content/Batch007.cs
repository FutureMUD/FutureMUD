#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits02Stock
{
	public static CatalogueOutfitDefinition EarlymodernOutfit0847() => new(
		Metadata: new("earlymodern_outfit_0847", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_hindu_prayer_mala", "item:earlymodern_religious_jain_mouthcloth", "item:medieval_jain_white_ascetic_robe", "item:medieval_jain_white_shoulder_wrap", "item:renaissance_shared_clothing_breast_wrap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Śvetāmbara Jain nun",
		Description: "Grouping: Jain. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_breast_wrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_jain_white_ascetic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_jain_white_shoulder_wrap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("earlymodern_religious_jain_mouthcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("earlymodern_religious_hindu_prayer_mala", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0848() => new(
		Metadata: new("earlymodern_outfit_0848", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_sikh_chola", "item:earlymodern_religious_sikh_kachera", "item:earlymodern_religious_sikh_kanga", "item:earlymodern_religious_sikh_kara", "item:renaissance_southasian_bunched_ankletrousers", "item:renaissance_southasian_curvedtoe_shoes", "item:renaissance_southasian_flat_wound_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Khalsa Sikh male",
		Description: "Grouping: Sikh. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_religious_sikh_kachera", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_bunched_ankletrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_chola", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_curvedtoe_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_flat_wound_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kara", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kanga", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0849() => new(
		Metadata: new("earlymodern_outfit_0849", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_sikh_kachera", "item:earlymodern_religious_sikh_kanga", "item:earlymodern_religious_sikh_kara", "item:renaissance_southasian_bunched_ankletrousers", "item:renaissance_southasian_curvedtoe_shoes", "item:renaissance_southasian_flat_wound_turban", "item:renaissance_southasian_shoulderdraped_garment"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Khalsa Sikh female",
		Description: "Grouping: Sikh. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_religious_sikh_kachera", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_bunched_ankletrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_shoulderdraped_garment", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_curvedtoe_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_flat_wound_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kara", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kanga", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0850() => new(
		Metadata: new("earlymodern_outfit_0850", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_sikh_chola", "item:earlymodern_religious_sikh_kachera", "item:earlymodern_religious_sikh_kanga", "item:earlymodern_religious_sikh_kara", "item:earlymodern_southasian_clothing_light_cotton_shawl", "item:renaissance_southasian_bunched_ankletrousers", "item:renaissance_southasian_curvedtoe_shoes", "item:renaissance_southasian_flat_wound_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Sikh granthi or scripture reader",
		Description: "Grouping: Sikh. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_religious_sikh_kachera", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_bunched_ankletrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_chola", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_curvedtoe_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_flat_wound_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southasian_clothing_light_cotton_shawl", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kara", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kanga", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0851() => new(
		Metadata: new("earlymodern_outfit_0851", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_sikh_chola", "item:earlymodern_religious_sikh_kachera", "item:earlymodern_religious_sikh_kanga", "item:earlymodern_religious_sikh_kara", "item:earlymodern_southasian_clothing_light_cotton_shawl", "item:renaissance_persianate_soft_ridingboots", "item:renaissance_southasian_bunched_ankletrousers", "item:renaissance_southasian_flat_wound_turban"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Travelling Sikh",
		Description: "Grouping: Sikh. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_religious_sikh_kachera", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_bunched_ankletrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_chola", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_persianate_soft_ridingboots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_southasian_flat_wound_turban", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_southasian_clothing_light_cotton_shawl", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kara", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_sikh_kanga", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0852() => new(
		Metadata: new("earlymodern_outfit_0852", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_theravada_underrobe", "item:earlymodern_religious_theravada_upperrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Theravada monk",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_religious_theravada_underrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"saffron yellow\""),
			new("earlymodern_religious_theravada_upperrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"saffron yellow\"")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0853() => new(
		Metadata: new("earlymodern_outfit_0853", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_theravada_underrobe", "item:earlymodern_religious_theravada_upperrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Theravada novice",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_religious_theravada_underrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"saffron yellow\""),
			new("earlymodern_religious_theravada_upperrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"saffron yellow\""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0854() => new(
		Metadata: new("earlymodern_outfit_0854", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_theravada_nun_robe", "item:earlymodern_seasia_clothing_light_shoulder_cloth", "item:renaissance_shared_clothing_textile_sandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Theravada nun or female renunciant",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_religious_theravada_nun_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_seasia_clothing_light_shoulder_cloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_textile_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0855() => new(
		Metadata: new("earlymodern_outfit_0855", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_tibetan_mantle", "item:earlymodern_religious_tibetan_shamtab", "item:earlymodern_religious_tibetan_vest", "item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_long_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Tibetan monk",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_tibetan_shamtab", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("earlymodern_religious_tibetan_vest", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("earlymodern_religious_tibetan_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0856() => new(
		Metadata: new("earlymodern_outfit_0856", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_tibetan_mantle", "item:earlymodern_religious_tibetan_shamtab", "item:earlymodern_religious_tibetan_vest", "item:earlymodern_religious_tibetan_yellow_hat", "item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_long_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Tibetan ceremonial scholar-monk",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_tibetan_shamtab", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("earlymodern_religious_tibetan_vest", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("earlymodern_religious_tibetan_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_tibetan_yellow_hat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0857() => new(
		Metadata: new("earlymodern_outfit_0857", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_tibetan_mantle", "item:earlymodern_religious_tibetan_shamtab", "item:earlymodern_religious_tibetan_vest", "item:renaissance_shared_clothing_ankle_boots", "item:renaissance_shared_clothing_long_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Tibetan nun",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_tibetan_shamtab", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("earlymodern_religious_tibetan_vest", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("earlymodern_religious_tibetan_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=\"maroon red\""),
			new("renaissance_shared_clothing_ankle_boots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0858() => new(
		Metadata: new("earlymodern_outfit_0858", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_qing_clothing_cloth_shoes", "item:earlymodern_qing_clothing_wide_cotton_trousers", "item:earlymodern_religious_buddhist_prayer_beads", "item:medieval_buddhist_black_monastic_robe", "item:medieval_buddhist_patched_kasaya", "item:renaissance_shared_clothing_close_cloth_cap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Chinese Buddhist monk",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_qing_clothing_wide_cotton_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_qing_clothing_cloth_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_black_monastic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_patched_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_close_cloth_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0859() => new(
		Metadata: new("earlymodern_outfit_0859", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_indianocean_clothing_light_cotton_head_veil", "item:earlymodern_qing_clothing_cloth_shoes", "item:earlymodern_qing_clothing_wide_cotton_trousers", "item:earlymodern_religious_buddhist_prayer_beads", "item:medieval_buddhist_nun_robe", "item:medieval_buddhist_patched_kasaya"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Chinese Buddhist nun",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_qing_clothing_wide_cotton_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_qing_clothing_cloth_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_nun_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_patched_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_indianocean_clothing_light_cotton_head_veil", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0860() => new(
		Metadata: new("earlymodern_outfit_0860", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_qing_clothing_cloth_shoes", "item:earlymodern_qing_clothing_wide_cotton_trousers", "item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_eastasian_brocade_kasaya", "item:medieval_buddhist_black_monastic_robe", "item:renaissance_shared_clothing_close_cloth_cap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern East Asian Buddhist abbot in formal vestments",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_qing_clothing_wide_cotton_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_qing_clothing_cloth_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_black_monastic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_eastasian_brocade_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_close_cloth_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0861() => new(
		Metadata: new("earlymodern_outfit_0861", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_joseon_clothing_full_baji_trousers", "item:earlymodern_religious_buddhist_prayer_beads", "item:medieval_buddhist_black_monastic_robe", "item:medieval_buddhist_patched_kasaya", "item:renaissance_joseon_white_clothshoes", "item:renaissance_shared_clothing_close_cloth_cap"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Joseon Buddhist monk",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("earlymodern_joseon_clothing_full_baji_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_joseon_white_clothshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_black_monastic_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_patched_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_shared_clothing_close_cloth_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0862() => new(
		Metadata: new("earlymodern_outfit_0862", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_joseon_clothing_full_baji_trousers", "item:earlymodern_religious_joseon_ritual_cap", "item:earlymodern_religious_joseon_simui", "item:preindustrial_clothing_simple_woven_sash", "item:renaissance_joseon_long_crossfront_underrobe", "item:renaissance_joseon_white_clothshoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Joseon Confucian ritual scholar",
		Description: "Grouping: Confucian ritual. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_joseon_long_crossfront_underrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_joseon_clothing_full_baji_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_joseon_white_clothshoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_joseon_simui", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_joseon_ritual_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0863() => new(
		Metadata: new("earlymodern_outfit_0863", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_edo_clothing_woven_sandals", "item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_japanese_rakusu", "item:renaissance_japanese_full_pleated_hakama", "item:renaissance_japanese_smallsleeve_wraprobe", "item:renaissance_japanese_splittoe_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Edo Zen monk",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_japanese_smallsleeve_wraprobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_full_pleated_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_splittoe_socks", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_edo_clothing_woven_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_japanese_rakusu", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0864() => new(
		Metadata: new("earlymodern_outfit_0864", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_edo_clothing_woven_sandals", "item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_eastasian_brocade_kasaya", "item:earlymodern_religious_japanese_rakusu", "item:renaissance_japanese_full_pleated_hakama", "item:renaissance_japanese_smallsleeve_wraprobe", "item:renaissance_japanese_splittoe_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Edo Buddhist priest in formal kesa",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_japanese_smallsleeve_wraprobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_full_pleated_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_splittoe_socks", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_edo_clothing_woven_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_japanese_rakusu", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_eastasian_brocade_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0865() => new(
		Metadata: new("earlymodern_outfit_0865", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_edo_clothing_woven_sandals", "item:earlymodern_religious_buddhist_prayer_beads", "item:medieval_buddhist_patched_kasaya", "item:renaissance_japanese_full_pleated_hakama", "item:renaissance_japanese_splittoe_socks", "item:renaissance_japanese_unlined_summerrobe"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Edo Buddhist nun",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_japanese_unlined_summerrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_full_pleated_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_splittoe_socks", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_edo_clothing_woven_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_patched_kasaya", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0866() => new(
		Metadata: new("earlymodern_outfit_0866", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_edo_clothing_woven_sandals", "item:earlymodern_religious_buddhist_prayer_beads", "item:medieval_buddhist_travelling_mantle", "item:renaissance_japanese_field_trousers", "item:renaissance_japanese_lacquered_conicalhat", "item:renaissance_japanese_smallsleeve_wraprobe", "item:renaissance_japanese_splittoe_socks"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Japanese travelling monk",
		Description: "Grouping: Buddhist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_japanese_smallsleeve_wraprobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_field_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_splittoe_socks", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_edo_clothing_woven_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_buddhist_travelling_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_japanese_lacquered_conicalhat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0867() => new(
		Metadata: new("earlymodern_outfit_0867", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_qing_clothing_cloth_shoes", "item:medieval_daoist_cross_collar_robe", "item:medieval_daoist_ritual_cap", "item:preindustrial_clothing_simple_woven_sash", "item:renaissance_ming_crosscollar_innerrobe", "item:renaissance_ming_wide_drawstring_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Daoist priest",
		Description: "Grouping: Daoist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_ming_crosscollar_innerrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_wide_drawstring_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_qing_clothing_cloth_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_daoist_cross_collar_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_daoist_ritual_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("preindustrial_clothing_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0868() => new(
		Metadata: new("earlymodern_outfit_0868", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_buddhist_prayer_beads", "item:earlymodern_religious_daoist_cloud_shoes", "item:earlymodern_religious_daoist_lotus_crown", "item:medieval_daoist_cloud_sleeved_robe", "item:preindustrial_clothing_simple_woven_sash", "item:renaissance_ming_crosscollar_innerrobe", "item:renaissance_ming_wide_drawstring_trousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Daoist ritual master",
		Description: "Grouping: Daoist. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_ming_crosscollar_innerrobe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_ming_wide_drawstring_trousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_daoist_cloud_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("medieval_daoist_cloud_sleeved_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_daoist_lotus_crown", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("preindustrial_clothing_simple_woven_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_buddhist_prayer_beads", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0869() => new(
		Metadata: new("earlymodern_outfit_0869", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_edo_clothing_narrow_woven_obi", "item:earlymodern_edo_clothing_woven_sandals", "item:medieval_shinto_priest_eboshi", "item:medieval_shinto_priest_hakama", "item:medieval_shinto_white_joe_robe", "item:renaissance_shared_clothing_long_undershirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Shinto shrine priest",
		Description: "Grouping: Shinto. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_shared_clothing_long_undershirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_shinto_white_joe_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_shinto_priest_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("earlymodern_edo_clothing_woven_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_shinto_priest_eboshi", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=black"),
			new("earlymodern_edo_clothing_narrow_woven_obi", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0870() => new(
		Metadata: new("earlymodern_outfit_0870", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_edo_clothing_wide_woven_obi", "item:earlymodern_edo_clothing_woven_sandals", "item:medieval_shinto_miko_red_hakama", "item:medieval_shinto_miko_white_kosode"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Shinto shrine attendant",
		Description: "Grouping: Shinto. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("medieval_shinto_miko_white_kosode", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=white"),
			new("medieval_shinto_miko_red_hakama", null, null, null, OutfitTemplateItemPlacement.Worn, null, "colour=red"),
			new("earlymodern_edo_clothing_woven_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_edo_clothing_wide_woven_obi", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0871() => new(
		Metadata: new("earlymodern_outfit_0871", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_ryukyu_noro_headcloth", "item:earlymodern_religious_ryukyu_noro_necklace", "item:earlymodern_religious_ryukyu_noro_robe", "item:earlymodern_ryukyu_clothing_woven_fibre_sandals", "item:renaissance_ryukyu_pleated_wrapskirt"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Ryukyuan noro priestess",
		Description: "Grouping: Ryukyuan religion. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_ryukyu_pleated_wrapskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_ryukyu_clothing_woven_fibre_sandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_ryukyu_noro_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_ryukyu_noro_headcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_ryukyu_noro_necklace", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0872() => new(
		Metadata: new("earlymodern_outfit_0872", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_local_beadwork_collar", "item:medieval_steppe_ritual_felt_coat", "item:medieval_steppe_ritual_fur_cap", "item:medieval_steppe_ritual_sash", "item:renaissance_steppe_soft_highboots", "item:renaissance_steppe_wide_ridingtrousers"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Steppe ritual specialist",
		Description: "Grouping: Local ritual. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_steppe_wide_ridingtrousers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_steppe_soft_highboots", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_steppe_ritual_felt_coat", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_steppe_ritual_sash", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("medieval_steppe_ritual_fur_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_beadwork_collar", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0873() => new(
		Metadata: new("earlymodern_outfit_0873", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_local_feather_headband", "item:earlymodern_religious_local_shell_necklace", "item:renaissance_mesoamerican_panelled_loincloth", "item:renaissance_mesoamerican_shouldertied_cloak", "item:renaissance_mesoamerican_side_seamed_tunic", "item:renaissance_mesoamerican_woven_fibresandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mesoamerican local ritual specialist male",
		Description: "Grouping: Local ritual. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_mesoamerican_panelled_loincloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_side_seamed_tunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_woven_fibresandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_shouldertied_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_feather_headband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_shell_necklace", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0874() => new(
		Metadata: new("earlymodern_outfit_0874", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_local_beadwork_collar", "item:earlymodern_religious_local_fringed_mantle", "item:earlymodern_religious_local_woven_headband", "item:renaissance_mesoamerican_long_wrapskirt", "item:renaissance_mesoamerican_rectangular_blouse", "item:renaissance_mesoamerican_woven_fibresandals"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Mesoamerican local ritual specialist female",
		Description: "Grouping: Local ritual. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_mesoamerican_long_wrapskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_rectangular_blouse", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_mesoamerican_woven_fibresandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_fringed_mantle", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_woven_headband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_beadwork_collar", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0875() => new(
		Metadata: new("earlymodern_outfit_0875", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_andeancolonial_clothing_wool_poncho", "item:earlymodern_religious_local_beadwork_collar", "item:renaissance_andean_braided_fibresandals", "item:renaissance_andean_long_headcloth", "item:renaissance_andean_pinned_wrapskirt", "item:renaissance_andean_straight_sleevelesstunic"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Andean local ritual specialist",
		Description: "Grouping: Local ritual. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_andean_straight_sleevelesstunic", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_pinned_wrapskirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_braided_fibresandals", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_andeancolonial_clothing_wool_poncho", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_andean_long_headcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_beadwork_collar", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition EarlymodernOutfit0876() => new(
		Metadata: new("earlymodern_outfit_0876", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:earlymodern_religious_local_feather_headband", "item:earlymodern_religious_local_shell_necklace", "item:renaissance_northamerican_fur_robe", "item:renaissance_northamerican_hide_breechcloth", "item:renaissance_northamerican_hide_shirt", "item:renaissance_northamerican_paired_hideleggings", "item:renaissance_northamerican_soft_moccasins"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/EarlyModernOutfitManifestSpecs", []),
		Name: "Early Modern Indigenous North American local ritual specialist",
		Description: "Grouping: Local ritual. Collection: Fourth-pass religious outfit manifests.",
		Items:
		[
			new("renaissance_northamerican_hide_breechcloth", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_northamerican_paired_hideleggings", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_northamerican_hide_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_northamerican_soft_moccasins", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_northamerican_fur_robe", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_feather_headband", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("earlymodern_religious_local_shell_necklace", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
