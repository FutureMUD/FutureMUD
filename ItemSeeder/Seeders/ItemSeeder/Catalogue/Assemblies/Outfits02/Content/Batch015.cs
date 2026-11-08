#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Outfits02Stock
{
	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitMughalCavalryman() => new(
		Metadata: new("renaissance_military_armour_outfit_mughal_cavalryman", "renaissance", ["renaissance"], ["item:renaissance_military_char_aina_mughal", "item:renaissance_military_dastana_mughal", "item:renaissance_military_khula_khud_mughal", "item:renaissance_military_mail_shirt_mughal", "item:renaissance_military_sipahi_greaves_lamed"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Mughal cavalryman",
		Description: "Builder-facing Renaissance armour loadout for a Mughal cavalryman.",
		Items:
		[
			new("renaissance_military_khula_khud_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_char_aina_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_mail_shirt_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_dastana_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sipahi_greaves_lamed", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitRajputNoble() => new(
		Metadata: new("renaissance_military_armour_outfit_rajput_noble", "renaissance", ["renaissance"], ["item:renaissance_military_char_aina_mughal", "item:renaissance_military_dastana_mughal", "item:renaissance_military_rajput_coat_quilted", "item:renaissance_military_rajput_mail_coif", "item:renaissance_military_sipahi_greaves_lamed"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Rajput noble",
		Description: "Builder-facing Renaissance armour loadout for a Rajput noble.",
		Items:
		[
			new("renaissance_military_rajput_mail_coif", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_rajput_coat_quilted", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_char_aina_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_dastana_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sipahi_greaves_lamed", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitDeccanCavalryman() => new(
		Metadata: new("renaissance_military_armour_outfit_deccan_cavalryman", "renaissance", ["renaissance"], ["item:renaissance_military_dastana_mughal", "item:renaissance_military_deccan_cuirass_laminar", "item:renaissance_military_khula_khud_mughal", "item:renaissance_military_mail_shirt_mughal", "item:renaissance_military_sipahi_greaves_lamed"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Deccan cavalryman",
		Description: "Builder-facing Renaissance armour loadout for a Deccan cavalryman.",
		Items:
		[
			new("renaissance_military_khula_khud_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_deccan_cuirass_laminar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_mail_shirt_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_dastana_mughal", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sipahi_greaves_lamed", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitVijayanagaraGuard() => new(
		Metadata: new("renaissance_military_armour_outfit_vijayanagara_guard", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_deccan_cuirass_laminar", "item:renaissance_military_plate_greaves", "item:renaissance_military_rajput_coat_quilted", "item:renaissance_military_vijayanagara_helmet_conical"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Vijayanagara guard",
		Description: "Builder-facing Renaissance armour loadout for a Vijayanagara guard.",
		Items:
		[
			new("renaissance_military_vijayanagara_helmet_conical", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_rajput_coat_quilted", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_deccan_cuirass_laminar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_plate_greaves", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitMingBorderInfantry() => new(
		Metadata: new("renaissance_military_armour_outfit_ming_border_infantry", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_brigandine_panelled", "item:renaissance_military_ming_helmet_brimmed", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Ming border infantry",
		Description: "Builder-facing Renaissance armour loadout for Ming border infantry.",
		Items:
		[
			new("renaissance_military_ming_helmet_brimmed", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_brigandine_panelled", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitMingCavalryman() => new(
		Metadata: new("renaissance_military_armour_outfit_ming_cavalryman", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_cuirass_uzbek_lamellar", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_helmet_brimmed", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Ming cavalryman",
		Description: "Builder-facing Renaissance armour loadout for a Ming cavalryman.",
		Items:
		[
			new("renaissance_military_ming_helmet_brimmed", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_cuirass_uzbek_lamellar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitMingPalaceGuard() => new(
		Metadata: new("renaissance_military_armour_outfit_ming_palace_guard", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_brigandine_livery_vest", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_brigandine_panelled", "item:renaissance_military_ming_helmet_brimmed", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Ming palace guard",
		Description: "Builder-facing Renaissance armour loadout for a Ming palace guard.",
		Items:
		[
			new("renaissance_military_ming_helmet_brimmed", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_brigandine_panelled", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_brigandine_livery_vest", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitJoseonArcher() => new(
		Metadata: new("renaissance_military_armour_outfit_joseon_archer", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_joseon_brigandine", "item:renaissance_military_joseon_helmet_brimmed", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Joseon archer",
		Description: "Builder-facing Renaissance armour loadout for a Joseon archer.",
		Items:
		[
			new("renaissance_military_joseon_helmet_brimmed", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_joseon_brigandine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitJoseonGunner() => new(
		Metadata: new("renaissance_military_armour_outfit_joseon_gunner", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_joseon_brigandine", "item:renaissance_military_joseon_helmet_brimmed", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Joseon gunner",
		Description: "Builder-facing Renaissance armour loadout for a Joseon gunner.",
		Items:
		[
			new("renaissance_military_joseon_helmet_brimmed", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_joseon_brigandine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitJapaneseAshigaruSpearman() => new(
		Metadata: new("renaissance_military_armour_outfit_japanese_ashigaru_spearman", "renaissance", ["renaissance"], ["item:renaissance_military_do_japanese_lamellar", "item:renaissance_military_kabuto_ridged", "item:renaissance_military_kote_japanese", "item:renaissance_military_kusazuri_japanese", "item:renaissance_military_sode_japanese", "item:renaissance_military_suneate_japanese"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Japanese ashigaru spearman",
		Description: "Builder-facing Renaissance armour loadout for a Japanese ashigaru spearman.",
		Items:
		[
			new("renaissance_military_kabuto_ridged", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_do_japanese_lamellar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sode_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kote_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kusazuri_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_suneate_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitJapaneseAshigaruArquebusier() => new(
		Metadata: new("renaissance_military_armour_outfit_japanese_ashigaru_arquebusier", "renaissance", ["renaissance"], ["item:renaissance_military_do_japanese_lamellar", "item:renaissance_military_kabuto_ridged", "item:renaissance_military_kote_japanese", "item:renaissance_military_kusazuri_japanese", "item:renaissance_military_sode_japanese", "item:renaissance_military_suneate_japanese"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Japanese ashigaru arquebusier",
		Description: "Builder-facing Renaissance armour loadout for a Japanese ashigaru arquebusier.",
		Items:
		[
			new("renaissance_military_kabuto_ridged", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_do_japanese_lamellar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sode_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kote_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kusazuri_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_suneate_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitJapaneseSamurai() => new(
		Metadata: new("renaissance_military_armour_outfit_japanese_samurai", "renaissance", ["renaissance"], ["item:renaissance_military_do_japanese_lamellar", "item:renaissance_military_kabuto_ridged", "item:renaissance_military_kote_japanese", "item:renaissance_military_kusazuri_japanese", "item:renaissance_military_menpo_japanese", "item:renaissance_military_sode_japanese", "item:renaissance_military_suneate_japanese"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Japanese samurai",
		Description: "Builder-facing Renaissance armour loadout for a Japanese samurai.",
		Items:
		[
			new("renaissance_military_kabuto_ridged", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_do_japanese_lamellar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sode_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kote_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kusazuri_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_suneate_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_menpo_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitJapaneseMountedArcher() => new(
		Metadata: new("renaissance_military_armour_outfit_japanese_mounted_archer", "renaissance", ["renaissance"], ["item:renaissance_military_do_japanese_lamellar", "item:renaissance_military_kabuto_ridged", "item:renaissance_military_kote_japanese", "item:renaissance_military_kusazuri_japanese", "item:renaissance_military_sode_japanese", "item:renaissance_military_suneate_japanese"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Japanese mounted archer",
		Description: "Builder-facing Renaissance armour loadout for a Japanese mounted archer.",
		Items:
		[
			new("renaissance_military_kabuto_ridged", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_do_japanese_lamellar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sode_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kote_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_kusazuri_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_suneate_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitTatarHorseArcher() => new(
		Metadata: new("renaissance_military_armour_outfit_tatar_horse_archer", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_helmet_tatar_conical", "item:renaissance_military_hussar_karwasze", "item:renaissance_military_mail_shirt_tatar", "item:renaissance_military_sipahi_greaves_lamed"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Tatar horse archer",
		Description: "Builder-facing Renaissance armour loadout for a Tatar horse archer.",
		Items:
		[
			new("renaissance_military_helmet_tatar_conical", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_mail_shirt_tatar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_hussar_karwasze", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sipahi_greaves_lamed", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitUzbekCavalryman() => new(
		Metadata: new("renaissance_military_armour_outfit_uzbek_cavalryman", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_cuirass_uzbek_lamellar", "item:renaissance_military_helmet_uzbek_lamellar", "item:renaissance_military_hussar_karwasze", "item:renaissance_military_sipahi_greaves_lamed"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Uzbek cavalryman",
		Description: "Builder-facing Renaissance armour loadout for an Uzbek cavalryman.",
		Items:
		[
			new("renaissance_military_helmet_uzbek_lamellar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_cuirass_uzbek_lamellar", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_hussar_karwasze", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sipahi_greaves_lamed", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitSiameseInfantryman() => new(
		Metadata: new("renaissance_military_armour_outfit_siamese_infantryman", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_helmet_burmese_lacquered", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_leg_guards", "item:renaissance_military_rattan_vest_siamese"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Siamese infantryman",
		Description: "Builder-facing Renaissance armour loadout for a Siamese infantryman.",
		Items:
		[
			new("renaissance_military_helmet_burmese_lacquered", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_rattan_vest_siamese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitBurmeseRoyalGuard() => new(
		Metadata: new("renaissance_military_armour_outfit_burmese_royal_guard", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_cuirass_burmese_rattan", "item:renaissance_military_helmet_burmese_lacquered", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Burmese royal guard",
		Description: "Builder-facing Renaissance armour loadout for a Burmese royal guard.",
		Items:
		[
			new("renaissance_military_helmet_burmese_lacquered", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_cuirass_burmese_rattan", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitSahelianCavalryman() => new(
		Metadata: new("renaissance_military_armour_outfit_sahelian_cavalryman", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_bazubands_ottoman", "item:renaissance_military_jack_sahelian_quilted", "item:renaissance_military_sipahi_greaves_lamed", "item:renaissance_military_turban_helmet_ottoman"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Sahelian cavalryman",
		Description: "Builder-facing Renaissance armour loadout for a Sahelian cavalryman.",
		Items:
		[
			new("renaissance_military_turban_helmet_ottoman", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_jack_sahelian_quilted", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_bazubands_ottoman", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sipahi_greaves_lamed", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitEthiopianHighlandGuard() => new(
		Metadata: new("renaissance_military_armour_outfit_ethiopian_highland_guard", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_bazubands_ottoman", "item:renaissance_military_mail_shirt_ethiopian", "item:renaissance_military_morion_combed", "item:renaissance_military_plate_greaves"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Ethiopian highland guard",
		Description: "Builder-facing Renaissance armour loadout for an Ethiopian highland guard.",
		Items:
		[
			new("renaissance_military_morion_combed", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_mail_shirt_ethiopian", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_bazubands_ottoman", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_plate_greaves", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitSwahiliCityGuard() => new(
		Metadata: new("renaissance_military_armour_outfit_swahili_city_guard", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_bazubands_ottoman", "item:renaissance_military_jack_sahelian_quilted", "item:renaissance_military_sipahi_greaves_lamed", "item:renaissance_military_turban_helmet_ottoman"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Swahili city guard",
		Description: "Builder-facing Renaissance armour loadout for a Swahili city guard.",
		Items:
		[
			new("renaissance_military_turban_helmet_ottoman", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_jack_sahelian_quilted", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_bazubands_ottoman", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_sipahi_greaves_lamed", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitMesoamericanWarrior() => new(
		Metadata: new("renaissance_military_armour_outfit_mesoamerican_warrior", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_ichcahuipilli_mesoamerican", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Mesoamerican warrior",
		Description: "Builder-facing Renaissance armour loadout for a Mesoamerican warrior.",
		Items:
		[
			new("renaissance_military_ichcahuipilli_mesoamerican", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryArmourOutfitAndeanGuard() => new(
		Metadata: new("renaissance_military_armour_outfit_andean_guard", "renaissance", ["renaissance"], ["item:renaissance_military_articulated_gauntlets", "item:renaissance_military_cuirass_andean_rawhide", "item:renaissance_military_ming_arm_guards", "item:renaissance_military_ming_leg_guards"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Armour: Andean guard",
		Description: "Builder-facing Renaissance armour loadout for an Andean guard.",
		Items:
		[
			new("renaissance_military_cuirass_andean_rawhide", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_arm_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_articulated_gauntlets", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_ming_leg_guards", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryMountOutfitWesternEuropeanField() => new(
		Metadata: new("renaissance_military_mount_outfit_western_european_field", "renaissance", ["renaissance"], ["item:renaissance_military_barding_caparison_brigandine", "item:renaissance_military_barding_chanfron", "item:renaissance_military_barding_criniere", "item:renaissance_military_barding_croupiere", "item:renaissance_military_barding_flanchards", "item:renaissance_military_barding_peytral"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Mount Armour: Western European field barding",
		Description: "Builder-facing Renaissance horse armour loadout for Western European field service.",
		Items:
		[
			new("renaissance_military_barding_caparison_brigandine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_chanfron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_criniere", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_peytral", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_flanchards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_croupiere", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryMountOutfitWesternEuropeanJoust() => new(
		Metadata: new("renaissance_military_mount_outfit_western_european_joust", "renaissance", ["renaissance"], ["item:renaissance_military_barding_chanfron", "item:renaissance_military_barding_criniere", "item:renaissance_military_barding_croupiere", "item:renaissance_military_barding_flanchards", "item:renaissance_military_barding_peytral"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Mount Armour: Western European joust barding",
		Description: "Builder-facing Renaissance horse armour loadout for Western European jousting.",
		Items:
		[
			new("renaissance_military_barding_chanfron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_criniere", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_peytral", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_flanchards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_croupiere", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryMountOutfitPolishHungarianHussar() => new(
		Metadata: new("renaissance_military_mount_outfit_polish_hungarian_hussar", "renaissance", ["renaissance"], ["item:renaissance_military_barding_caparison_brigandine", "item:renaissance_military_barding_croupiere", "item:renaissance_military_barding_flanchards", "item:renaissance_military_hussar_chanfron", "item:renaissance_military_hussar_peytral"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Mount Armour: Polish and Hungarian hussar barding",
		Description: "Builder-facing Renaissance horse armour loadout for Polish and Hungarian hussar service.",
		Items:
		[
			new("renaissance_military_barding_caparison_brigandine", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_hussar_chanfron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_hussar_peytral", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_flanchards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_croupiere", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryMountOutfitMuscoviteBoyar() => new(
		Metadata: new("renaissance_military_mount_outfit_muscovite_boyar", "renaissance", ["renaissance"], ["item:renaissance_military_barding_chanfron", "item:renaissance_military_barding_criniere", "item:renaissance_military_barding_croupiere", "item:renaissance_military_barding_flanchards", "item:renaissance_military_barding_peytral", "item:renaissance_military_muscovite_caparison"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Mount Armour: Muscovite boyar barding",
		Description: "Builder-facing Renaissance horse armour loadout for a Muscovite boyar.",
		Items:
		[
			new("renaissance_military_muscovite_caparison", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_chanfron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_criniere", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_peytral", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_flanchards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_croupiere", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryMountOutfitOttomanSipahi() => new(
		Metadata: new("renaissance_military_mount_outfit_ottoman_sipahi", "renaissance", ["renaissance"], ["item:renaissance_military_barding_chanfron", "item:renaissance_military_barding_croupiere", "item:renaissance_military_barding_flanchards", "item:renaissance_military_barding_peytral", "item:renaissance_military_criniere_ottoman", "item:renaissance_military_ottoman_caparison"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Mount Armour: Ottoman sipahi barding",
		Description: "Builder-facing Renaissance horse armour loadout for an Ottoman sipahi.",
		Items:
		[
			new("renaissance_military_ottoman_caparison", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_chanfron", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_criniere_ottoman", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_peytral", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_flanchards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_croupiere", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceMilitaryMountOutfitJapaneseRetainer() => new(
		Metadata: new("renaissance_military_mount_outfit_japanese_retainer", "renaissance", ["renaissance"], ["item:renaissance_military_bamen_japanese", "item:renaissance_military_barding_criniere", "item:renaissance_military_barding_croupiere", "item:renaissance_military_barding_flanchards", "item:renaissance_military_barding_peytral", "item:renaissance_military_caparison_japanese"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceMilitaryArmourOutfitManifestSpecs", []),
		Name: "Renaissance Mount Armour: Japanese retainer barding",
		Description: "Builder-facing Renaissance horse armour loadout for a Japanese mounted retainer.",
		Items:
		[
			new("renaissance_military_caparison_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_bamen_japanese", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_criniere", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_peytral", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_flanchards", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_military_barding_croupiere", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitWesternOrdinary() => new(
		Metadata: new("renaissance_outfit_western_ordinary", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_shared_clothing_drawstring_drawers", "item:renaissance_western_cloth_jerkin", "item:renaissance_western_flat_cap", "item:renaissance_western_gathered_linen_shirt", "item:renaissance_western_joined_hose", "item:renaissance_western_square_toed_shoes"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western European Ordinary Town Dress",
		Description: "Admission: Western European Renaissance, 1450-1575. Purpose: ordinary urban or skilled-household dress.",
		Items:
		[
			new("renaissance_shared_clothing_drawstring_drawers", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_gathered_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_cloth_jerkin", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_joined_hose", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_flat_cap", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_square_toed_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);

	public static CatalogueOutfitDefinition RenaissanceOutfitWesternStatus() => new(
		Metadata: new("renaissance_outfit_western_status", "outfits", ["antiquity", "earlymodern", "medieval", "renaissance"], ["item:renaissance_western_feathered_velvet_bonnet", "item:renaissance_western_fine_latchet_shoes", "item:renaissance_western_paned_trunk_hose", "item:renaissance_western_pleated_linen_shirt", "item:renaissance_western_short_cloak", "item:renaissance_western_slashed_doublet"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "ItemSeeder.ClothingOutfitManifestData.Generated.cs/RenaissanceOutfitManifestSpecs", []),
		Name: "Renaissance Western European Court Dress",
		Description: "Admission: Western European Renaissance, 1500-1600; court/status gate. Purpose: court, diplomatic, or prosperous ceremonial dress.",
		Items:
		[
			new("renaissance_western_pleated_linen_shirt", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_slashed_doublet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_paned_trunk_hose", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_short_cloak", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_feathered_velvet_bonnet", null, null, null, OutfitTemplateItemPlacement.Worn, null, ""),
			new("renaissance_western_fine_latchet_shoes", null, null, null, OutfitTemplateItemPlacement.Worn, null, "")
		]
	);
}
