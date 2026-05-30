#nullable enable

using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string MedievalRootTagPath = "Eras / Medieval";
	private const string MedievalCultureTagRoot = "Eras / Medieval / Cultures";
	private const string MedievalStatusTagRoot = "Eras / Medieval / Status Roles";
	private const string HistoricRootTagPath = "Eras / Historic";
	private const string MedievalImplementedSealMeasureNote =
		"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder.";
	private const string MedievalDeferredGapNote =
		"Seeded as a prop because richer runtime support is deferred to a future engine component.";
	private const string MedievalCatalogueAliasReason =
		"Covered by an existing broader stable reference until a dedicated exact item spec is worth seeding.";
	private const string MedievalCatalogueDeferredReason =
		"Documented culture target retained for future material-culture expansion; no dedicated seeded item spec or alias exists yet.";
	private const string MedievalCatalogueOutfitCoverageReason =
		"Covered by explicit outfit-piece item specs rather than by an independent culture-catalogue item.";

	private sealed record MedievalCultureProfile(
		string Key,
		string Display,
		string ClothingCue,
		string ArmourDescription,
		string WeaponNoun,
		string WeaponShortDescription,
		string WeaponFullDescription,
		string WeaponComponent,
		string ShieldShortDescription,
		string ShieldFullDescription,
		string ShieldComponent,
		string FoodCue,
		string WritingCue);

	private sealed record MedievalStatusRoleProfile(
		string Key,
		string Display,
		string TagName,
		string GarmentToken,
		string Noun,
		string ShortDescription,
		string FullDescription,
		string Material,
		MaterialBehaviourType MaterialType,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string MarketTag,
		string WearComponent,
		string ArmourComponent);

	private sealed record MedievalWardrobePiece(
		string SlotKey,
		string Token,
		string Noun,
		string ShortDescription,
		string FullDescription,
		string Material,
		MaterialBehaviourType MaterialType,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string MarketTag,
		string FunctionTag,
		string WearComponent,
		string ArmourComponent,
		string InsulationComponent,
		string[] ExtraComponents);

	private sealed record MedievalCultureCatalogue(
		string CultureKey,
		string Display,
		IReadOnlyDictionary<string, IReadOnlyCollection<string>> StableReferencesByGroup)
	{
		public IEnumerable<MedievalCultureCatalogueEntry> Entries =>
			StableReferencesByGroup.SelectMany(group => group.Value.Select(stableReference =>
			{
				var classification = ClassifyMedievalCultureCatalogueReference(CultureKey, group.Key, stableReference);
				return new MedievalCultureCatalogueEntry(
					CultureKey,
					Display,
					group.Key,
					stableReference,
					BuildMedievalExplicitShortDescription(CultureKey, stableReference),
					classification.Status,
					classification.ImplementationStableReference,
					classification.Reason,
					classification.CraftCoverageExemption);
			}));
	}

	private sealed record MedievalCultureCatalogueEntry(
		string CultureKey,
		string Display,
		string Group,
		string StableReference,
		string ShortDescription,
		MedievalCultureCatalogueReferenceStatus Status,
		string? ImplementationStableReference,
		string? Reason,
		string? CraftCoverageExemption);

	internal enum MedievalCultureCatalogueReferenceStatus
	{
		ImplementedItem,
		CoveredByOutfitPiece,
		AliasOfExistingStableReference,
		Deferred
	}

	private sealed record MedievalCultureCatalogueReferenceClassification(
		MedievalCultureCatalogueReferenceStatus Status,
		string? ImplementationStableReference,
		string? Reason,
		string? CraftCoverageExemption);

	internal sealed record MedievalAuthoredOutfitPieceTestData(
		string StableReference,
		string OutfitReference,
		string CultureKey,
		string SexGenderPresentation,
		string SocialClassRole,
		IReadOnlyCollection<string> SlotKeys,
		string PieceName,
		string Noun,
		string ShortDescription,
		string FullDescription,
		string Material,
		MaterialBehaviourType MaterialType,
		ItemQuality Quality,
		SizeCategory Size,
		double WeightInGrams,
		decimal Cost,
		string? VariableColourComponent,
		IReadOnlyCollection<string> ColourVariablesUsed,
		IReadOnlyCollection<string> Components,
		IReadOnlyCollection<string> CraftInputs,
		IReadOnlyCollection<string> CraftTools,
		bool IntentionallySharedOrGeneric);

	internal sealed record EraItemSpecTestData(
		string StableReference,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string Material,
		MaterialBehaviourType MaterialType,
		IReadOnlyCollection<string> Tags,
		IReadOnlyCollection<string> Components,
		string? BuilderNotes);

	private sealed record MedievalBespokeOutfitPieceSpec(
		string StableReference,
		string OutfitReference,
		string CultureKey,
		string SexGenderPresentation,
		string SocialClassRole,
		string SlotKey,
		string PieceName,
		string Noun,
		string ShortDescription,
		string FullDescription,
		string Material,
		MaterialBehaviourType MaterialType,
		ItemQuality Quality,
		SizeCategory Size,
		double WeightInGrams,
		decimal Cost,
		string? VariableColourComponent,
		string[] ColourVariablesUsed,
		string[] Components,
		string[] CraftInputs,
		string[] CraftTools,
		string? ImplementationNotes,
		string? AuthoringGuidelineNotes);

	private sealed record MedievalBespokeOutfitPieceCraftSpec(
		string StableReference,
		string[] Inputs,
		string[] Tools);

	private static readonly MedievalCultureProfile[] MedievalCultureProfiles =
	[
		new("early_anglo_saxon", "Early Anglo-Saxon/Insular", "with tablet-woven edging",
			"a mail shirt over a belted tunic", "seax", "a broad iron seax",
			"This broad single-edged blade has a plain wooden grip, a serviceable iron back, and a belt-ready sheath.",
			"Melee_Shortsword", "a round linden shield",
			"This round shield has a limewood body, rawhide edging, and a simple iron boss at the centre.",
			"Shield_Round", "oat cakes, ale, and pottage", "wax tablets and charter strips"),
		new("anglo_danish", "Late Anglo-Saxon/Anglo-Danish", "with narrow braid and panelled seams",
			"a mail coat with a nasal helm", "long seax", "a long iron seax",
			"This long single-edged blade sits between knife and sword, with a narrow fuller and a wrapped grip.",
			"Melee_Shortsword", "a painted round shield",
			"This painted round shield has a stout boss, leather hand-grip, and layered timber planks under hide facing.",
			"Shield_Round", "rye bread, smoked fish, and ale", "sealed writs and reeve tallies"),
		new("norse", "Norse", "with practical gores and pinned straps",
			"a riveted mail shirt and conical helm", "bearded axe", "a bearded iron axe",
			"This bearded axe has a broad lower blade, a compact iron head, and a smooth haft for close fighting or ship work.",
			"Melee_Axe", "a bossed round shield",
			"This round shield is light enough to move quickly, with painted boards and a heavy iron boss.",
			"Shield_Round", "flatbread, stockfish, and sour milk", "runic tallies and trade tags"),
		new("norman", "Norman/Angevin", "with fitted sleeves and a slit skirt",
			"a long mail hauberk and nasal helm", "arming sword", "a cruciform arming sword",
			"This straight double-edged sword has a cruciform guard, wheel pommel, and serviceable iron blade.",
			"Melee_Longsword", "a kite shield",
			"This long kite shield narrows to a point, with a curved face and painted linen over timber.",
			"Shield_Thureos", "wheaten bread, wine, and stewed meat", "sealed charters and exchequer rolls"),
		new("high_british", "High Medieval Britain/Marcher", "with fitted side lacing",
			"a mail hauberk over a padded aketon", "arming sword", "a well-balanced arming sword",
			"This arming sword has a bright iron blade, straight guard, and a grip wrapped for mounted or foot service.",
			"Melee_Longsword", "a heater shield",
			"This heater shield has a compact triangular face suited to mounted and foot combat alike.",
			"Shield_Thureos", "trencher bread, ale, and cheese", "seal tags and manor accounts"),
		new("gaelic", "Gaelic/Welsh/Highland", "with a belted outer wrap",
			"a padded coat with light mail patches", "long spear", "a long leaf-bladed spear",
			"This spear has a leaf-shaped iron head, a stout ash shaft, and a rawhide binding below the socket.",
			"Melee_Long Spear", "a small hide targe",
			"This compact hide-faced shield is easy to carry through hills, woods, and rough border country.",
			"Shield_Targe", "oat bread, curds, and smoked meat", "memoranda strips and boundary tallies"),
		new("carolingian", "Carolingian/Frankish", "with broad bands and a high belt",
			"a mail shirt with a reinforced helm", "spatha", "a broad iron spatha",
			"This broad double-edged sword has a modest fuller, simple guard, and balanced grip for close formation fighting.",
			"Melee_Longsword", "a large round shield",
			"This large round shield has painted hide, a domed boss, and a grip set for shield-wall work.",
			"Shield_Round", "barley bread, pork, and ale", "capitularies and estate lists"),
		new("capetian", "Capetian/Low Countries", "with neat tailoring and a lined fall",
			"a padded aketon under mail", "arming sword", "a narrow arming sword",
			"This narrow sword has a straight guard, a tapered blade, and a leather-wrapped grip worn smooth by drill.",
			"Melee_Longsword", "a heater shield",
			"This heater shield has a linened face, a sturdy hand-grip, and a shape suited to heraldic painting.",
			"Shield_Thureos", "white bread, onions, and wine", "notarial notes and sealed letters"),
		new("german_hre", "German/HRE/Alpine-North Italian", "with close sleeves and a short mantle",
			"a hauberk with plate-reinforced fittings", "war hammer", "a beaked iron war hammer",
			"This compact war hammer has a striking face, a rear beak, and a dense haft for armoured fighting.",
			"Melee_Warhammer", "a reinforced heater shield",
			"This reinforced heater shield has a painted face, leather edging, and a compact shape for town or field service.",
			"Shield_Thureos", "rye bread, sausage, and beer", "guild marks and court seals"),
		new("iberian_christian", "Iberian Christian", "with a short overcloak and narrow sleeves",
			"a mail shirt over a quilted coat", "falcata-like sword", "a curved iron war sword",
			"This curved war sword has a forward-weighted edge, a guarded grip, and a blade meant for hard cutting blows.",
			"Melee_Longsword", "an almond shield",
			"This almond-shaped shield is hide-faced, bright-painted, and built for mounted or frontier fighting.",
			"Shield_Thureos", "wheat bread, olives, and wine", "frontier charters and seal cords"),
		new("andalusi", "al-Andalus/Maghreb", "with flowing layers and light trim",
			"a quilted coat with mail reinforcement", "saif", "a slender iron saif",
			"This slender sword has a lively balance, a wrapped grip, and a long cutting edge with a slight curve.",
			"Melee_Longsword", "a hide adarga",
			"This hide shield is light, curved, and tough, with layered leather stiffened for fast mounted work.",
			"Shield_Dhal", "flatbread, oil, dates, and spiced stew", "paper contracts and office seals"),
		new("byzantine", "Byzantine", "with layered silk bands and formal closures",
			"a lamellar corselet over a padded coat", "paramerion", "a curved iron paramerion",
			"This curved sword has a guarded grip, a polished edge, and fittings suited to formal military service.",
			"Melee_Longsword", "a painted oval shield",
			"This oval shield has painted linen over timber, a central grip, and a practical campaign curve.",
			"Shield_Thureos", "wheat bread, wine, olives, and fish sauce", "chrysobull copies and sealed packets"),
		new("abbasid", "Abbasid/Persianate", "with wrapped closures and fine edging",
			"a lamellar coat with padded sleeves", "straight sword", "a straight iron sword",
			"This straight sword has a narrow blade, rounded pommel, and carefully wrapped grip for court or field service.",
			"Melee_Longsword", "a round dhal shield",
			"This round shield has a hide face, metal bosses, and a compact form suited to infantry or mounted guard use.",
			"Shield_Dhal", "rice, flatbread, sour milk, and syrups", "paper decrees and chancery seals"),
		new("fatimid", "Fatimid Egypt/Ifriqiya", "with light linen layers and formal cuffs",
			"a padded coat with scale panels", "guard spear", "a socketed guard spear",
			"This spear has a neat iron head, a long straight shaft, and ferrules for standing guard or field service.",
			"Melee_Long Spear", "a round hide shield",
			"This round hide shield is light, dark, and edged with stitching around a metal-bossed centre.",
			"Shield_Dhal", "flatbread, lentils, dates, and oil", "tax rolls and sealed paper orders"),
		new("seljuk_ayyubid", "Seljuk/Ayyubid/early Mamluk", "with riding slits and wrapped fastenings",
			"a lamellar riding coat and mail aventail", "cavalry mace", "a flanged iron mace",
			"This flanged mace has a compact head, short haft, and enough weight to threaten armour from horseback.",
			"Melee_Mace", "a round cavalry shield",
			"This round shield is hide-faced, compact, and reinforced with metal bosses for mobile fighting.",
			"Shield_Dhal", "flatbread, yogurt, pilaf, and spiced meat", "iqta records and sealed orders"),
		new("rus_novgorod", "Kyivan Rus/Novgorod", "with fur edging and long closures",
			"a mail shirt under a fur-trimmed coat", "war axe", "a broad iron war axe",
			"This broad axe has a socketed iron head, reinforced haft, and a practical edge for field or river work.",
			"Melee_Axe", "a tall oval shield",
			"This tall shield is timber-built, hide-faced, and edged for spear or axe fighting.",
			"Shield_Thureos", "rye bread, fish, mushrooms, and honey drink", "birchbark notes and princely seals"),
		new("steppe_turkic", "Steppe Turkic/Cuman/Mongol-adjacent", "with riding panels and tied closures",
			"a lamellar riding coat over quilted cloth", "sabre", "a curved riding sabre",
			"This curved sabre has a lively single edge, a grip suited to mounted use, and plain iron fittings.",
			"Melee_Longsword", "a compact hide shield",
			"This compact shield is hide-faced and light enough for fast mounted handling.",
			"Shield_Dhal", "millet, dried curds, meat, and fermented milk", "paiza tags and sealed pouches"),
		new("song_china", "Song China", "with crossed collars and bound hems",
			"a lamellar vest over a padded robe", "dao", "a single-edged iron dao",
			"This single-edged dao has a gentle curve, a guarded grip, and a plain scabbard suited to militia or guard work.",
			"Melee_Longsword", "a rattan shield",
			"This rattan shield is woven thick, lacquered dark, and fitted with a secure hand grip.",
			"Shield_Wicker", "rice, wheat noodles, tea, and pickled greens", "paper registers and official chops")
	];

	private const string MedievalCatalogueGroupClothing = "Clothing";
	private const string MedievalCatalogueGroupMilitary = "Military";
	private const string MedievalCatalogueGroupFoodAndBeverage = "Food and Beverage";
	private const string MedievalCatalogueGroupWritingAndAdministration = "Writing and Administration";
	private const string MedievalCatalogueGroupHouseholdAndDevotional = "Household and Devotional";

	private static readonly string[] MedievalCultureCatalogueGroups =
	[
		MedievalCatalogueGroupClothing,
		MedievalCatalogueGroupMilitary,
		MedievalCatalogueGroupFoodAndBeverage,
		MedievalCatalogueGroupWritingAndAdministration,
		MedievalCatalogueGroupHouseholdAndDevotional
	];

	private static readonly string[] MedievalCatalogueLowSignalWords =
	[
		"a",
		"an",
		"and",
		"of",
		"the",
		"with",
		"wool",
		"linen",
		"leather",
		"silk",
		"cotton",
		"iron",
		"wooden",
		"soft",
		"fine",
		"plain",
		"simple",
		"heavy",
		"light",
		"small",
		"large",
		"noble",
		"court",
		"work",
		"military",
		"religious",
		"monastic",
		"regional",
		"medieval"
	];

	private const string MedievalExplicitCultureCatalogueSource = @"
early_anglo_saxon|Clothing|medieval_clothing_early_anglo_saxon_tablet_banded_wool_tunic,medieval_clothing_early_anglo_saxon_square_cloak_disc_brooch,medieval_clothing_early_anglo_saxon_linen_head_veil,medieval_clothing_early_anglo_saxon_seax_belt,medieval_clothing_early_anglo_saxon_bead_necklace,medieval_clothing_early_anglo_saxon_monastic_wool_habit,medieval_clothing_early_anglo_saxon_embroidered_noble_mantle,medieval_clothing_early_anglo_saxon_brooch_fastened_riding_cloak,medieval_clothing_early_anglo_saxon_plain_work_smock,medieval_clothing_early_anglo_saxon_wool_cap,medieval_clothing_early_anglo_saxon_wool_leg_wraps,medieval_clothing_early_anglo_saxon_soft_leather_ankle_shoes
early_anglo_saxon|Military|medieval_weapon_early_anglo_saxon_broad_seax,medieval_weapon_early_anglo_saxon_ash_spear,medieval_shield_early_anglo_saxon_bossed_round_shield,medieval_military_early_anglo_saxon_mail_shirt,medieval_military_early_anglo_saxon_spangenhelm,medieval_military_early_anglo_saxon_shield_wall_belt,medieval_military_early_anglo_saxon_leather_archer_bracer,medieval_military_early_anglo_saxon_war_cloak
early_anglo_saxon|Food and Beverage|medieval_food_early_anglo_saxon_oat_flatbread,medieval_food_early_anglo_saxon_barley_pottage,medieval_food_early_anglo_saxon_smoked_eel_packet,medieval_food_early_anglo_saxon_ale_cup,medieval_food_early_anglo_saxon_curd_cheese_bowl,medieval_food_early_anglo_saxon_honeyed_oat_cake,medieval_food_early_anglo_saxon_monastery_loaf,medieval_food_early_anglo_saxon_wooden_trencher
early_anglo_saxon|Writing and Administration|medieval_writing_early_anglo_saxon_wax_diptych,medieval_writing_early_anglo_saxon_charter_strip,medieval_writing_early_anglo_saxon_monastic_manuscript_leaf,medieval_writing_early_anglo_saxon_reeve_tally,medieval_writing_early_anglo_saxon_seal_tag_bundle,medieval_writing_early_anglo_saxon_gospel_book_pouch
early_anglo_saxon|Household and Devotional|medieval_household_early_anglo_saxon_carved_hall_chest,medieval_household_early_anglo_saxon_hanging_clay_lamp,medieval_household_early_anglo_saxon_monastic_book_stand,medieval_jewellery_early_anglo_saxon_brooch_display_pin,medieval_devotional_early_anglo_saxon_wooden_reliquary
anglo_danish|Clothing|medieval_clothing_anglo_danish_panelled_wool_tunic,medieval_clothing_anglo_danish_long_seax_belt,medieval_clothing_anglo_danish_wrapped_trews,medieval_clothing_anglo_danish_embroidered_collar_tunic,medieval_clothing_anglo_danish_ring_pin_cloak,medieval_clothing_anglo_danish_leather_war_belt,medieval_clothing_anglo_danish_padded_shield_wall_tunic,medieval_clothing_anglo_danish_wool_hood,medieval_clothing_anglo_danish_trader_purse_belt,medieval_clothing_anglo_danish_head_rail,medieval_clothing_anglo_danish_heavy_sea_cloak,medieval_clothing_anglo_danish_leather_boots
anglo_danish|Military|medieval_weapon_anglo_danish_long_seax,medieval_weapon_anglo_danish_dane_axe,medieval_weapon_anglo_danish_socketed_spear,medieval_shield_anglo_danish_painted_round_shield,medieval_military_anglo_danish_mail_coat,medieval_military_anglo_danish_nasal_helm,medieval_military_anglo_danish_shield_wall_sling,medieval_military_anglo_danish_huscarl_weapon_belt
anglo_danish|Food and Beverage|medieval_food_anglo_danish_rye_loaf,medieval_food_anglo_danish_smoked_fish_packet,medieval_food_anglo_danish_ale_jack,medieval_food_anglo_danish_pea_pottage,medieval_food_anglo_danish_salt_beef_ration,medieval_food_anglo_danish_cheese_trencher,medieval_food_anglo_danish_honey_cake,medieval_food_anglo_danish_market_bread_bundle
anglo_danish|Writing and Administration|medieval_writing_anglo_danish_writ_packet,medieval_writing_anglo_danish_estate_tally,medieval_writing_anglo_danish_rune_marked_trade_tag,medieval_writing_anglo_danish_wax_tablet,medieval_writing_anglo_danish_sealed_rent_record,medieval_writing_anglo_danish_seax_belt_document_pouch
anglo_danish|Household and Devotional|medieval_household_anglo_danish_iron_bound_hall_chest,medieval_household_anglo_danish_reeve_account_box,medieval_household_anglo_danish_drinking_horn_rack,medieval_devotional_anglo_danish_carved_prayer_cross,medieval_jewellery_anglo_danish_ring_pin
norse|Clothing|medieval_clothing_norse_hangerok_apron_dress,medieval_clothing_norse_linen_underdress,medieval_jewellery_norse_oval_brooch_pair,medieval_clothing_norse_bead_strung_apron_straps,medieval_clothing_norse_heavy_sea_cloak,medieval_clothing_norse_fur_edged_hood,medieval_clothing_norse_wool_leg_wraps,medieval_clothing_norse_high_leather_boots,medieval_clothing_norse_trader_kaftan,medieval_clothing_norse_wool_cap,medieval_clothing_norse_arming_tunic,medieval_clothing_norse_leather_belt_pouch
norse|Military|medieval_weapon_norse_bearded_axe,medieval_weapon_norse_broad_axe,medieval_weapon_norse_socketed_spear,medieval_shield_norse_bossed_round_shield,medieval_military_norse_riveted_mail_shirt,medieval_military_norse_conical_helmet,medieval_weapon_norse_hunting_bow,medieval_military_norse_gorytos_quiver
norse|Food and Beverage|medieval_food_norse_stockfish_packet,medieval_food_norse_sour_milk_skin,medieval_food_norse_rye_flatbread,medieval_food_norse_preserved_meat_bundle,medieval_food_norse_curd_bowl,medieval_food_norse_shipboard_ration,medieval_food_norse_ale_horn,medieval_food_norse_smoked_fish_stew
norse|Writing and Administration|medieval_writing_norse_runic_tally_stick,medieval_writing_norse_merchant_weight_tag,medieval_writing_norse_ship_cargo_tally,medieval_writing_norse_wax_tablet,medieval_writing_norse_trade_tablet_wallet,medieval_writing_norse_runic_memorial_plaque
norse|Household and Devotional|medieval_household_norse_sea_chest,medieval_household_norse_carved_comb_case,medieval_trade_norse_weight_pouch,medieval_household_norse_drinking_horn,medieval_household_norse_shipboard_storage_box
norman|Clothing|medieval_clothing_norman_split_riding_tunic,medieval_clothing_norman_long_sleeved_cote,medieval_clothing_norman_short_mantle,medieval_clothing_norman_mail_surcoat,medieval_clothing_norman_arming_coif,medieval_clothing_norman_court_bliaut_gown,medieval_clothing_norman_linen_coif,medieval_clothing_norman_fine_leather_belt,medieval_clothing_norman_riding_boots,medieval_clothing_norman_hooded_travel_cloak,medieval_jewellery_norman_cloak_clasp,medieval_clothing_norman_chapel_robe
norman|Military|medieval_weapon_norman_arming_sword,medieval_weapon_norman_couched_lance,medieval_weapon_norman_military_spear,medieval_shield_norman_kite_shield,medieval_military_norman_mail_hauberk,medieval_military_norman_nasal_helmet,medieval_military_norman_padded_aketon,medieval_military_norman_arming_belt
norman|Food and Beverage|medieval_food_norman_wheaten_loaf,medieval_food_norman_meat_pottage,medieval_food_norman_cheese_trencher,medieval_food_norman_wine_cup,medieval_food_norman_salted_beef_ration,medieval_food_norman_feast_roast_platter,medieval_food_norman_spiced_wine_cup,medieval_food_norman_market_pie
norman|Writing and Administration|medieval_writing_norman_sealed_parchment_charter,medieval_writing_norman_exchequer_roll,medieval_writing_norman_writ_packet,medieval_writing_norman_notary_seal,medieval_writing_norman_manorial_account_roll,medieval_writing_norman_court_summons_tag
norman|Household and Devotional|medieval_household_norman_manor_chest,medieval_household_norman_chapel_lectern,medieval_household_norman_heraldic_shield_rack,medieval_household_norman_wall_candle_sconce,medieval_devotional_norman_pilgrim_badge
high_british|Clothing|medieval_clothing_high_british_wool_cote,medieval_clothing_high_british_sleeveless_surcoat,medieval_clothing_high_british_linen_coif,medieval_clothing_high_british_wimple_and_veil,medieval_clothing_high_british_braies,medieval_clothing_high_british_wool_chausses,medieval_clothing_high_british_arming_gambeson,medieval_clothing_high_british_long_belt_purse,medieval_clothing_high_british_merchant_gown,medieval_clothing_high_british_wool_mantle,medieval_clothing_high_british_archer_bracer,medieval_clothing_high_british_archer_belt
high_british|Military|medieval_weapon_high_british_arming_sword,medieval_weapon_high_british_war_spear,medieval_weapon_high_british_longbow,medieval_shield_high_british_heater_shield,medieval_military_high_british_mail_hauberk,medieval_military_high_british_kettle_hat,medieval_military_high_british_quilted_gambeson,medieval_military_high_british_arrow_bag
high_british|Food and Beverage|medieval_food_high_british_trencher_bread,medieval_food_high_british_ale_cup,medieval_food_high_british_cheese_wedge,medieval_food_high_british_pottage_bowl,medieval_food_high_british_salted_fish_ration,medieval_food_high_british_meat_pie,medieval_food_high_british_honeyed_pastry,medieval_food_high_british_cider_jack
high_british|Writing and Administration|medieval_writing_high_british_manor_account_roll,medieval_writing_high_british_guild_register_leaf,medieval_writing_high_british_sealed_writ,medieval_writing_high_british_archery_levy_list,medieval_writing_high_british_chapel_book,medieval_writing_high_british_toll_tally
high_british|Household and Devotional|medieval_household_high_british_guild_counter,medieval_household_high_british_wool_merchant_bale,medieval_household_high_british_parish_alms_box,medieval_household_high_british_archery_target_bundle,medieval_devotional_high_british_chapel_candle_stand
gaelic|Clothing|medieval_clothing_gaelic_brat_mantle,medieval_clothing_gaelic_leine_long_shirt,medieval_jewellery_gaelic_ring_pin,medieval_clothing_gaelic_deerskin_shoes,medieval_clothing_gaelic_woven_belt,medieval_clothing_gaelic_hill_cloak,medieval_clothing_gaelic_spear_carrier_belt,medieval_clothing_gaelic_linen_headcloth,medieval_clothing_gaelic_wool_trews,medieval_clothing_gaelic_bardic_mantle,medieval_clothing_gaelic_pastoral_pouch,medieval_clothing_gaelic_rough_weather_hood
gaelic|Military|medieval_weapon_gaelic_long_spear,medieval_weapon_gaelic_javelin_bundle,medieval_weapon_gaelic_short_knife,medieval_shield_gaelic_small_hide_targe,medieval_military_gaelic_light_padded_coat,medieval_military_gaelic_ring_pin_war_cloak,medieval_weapon_gaelic_hunting_bow,medieval_military_gaelic_leather_bracers
gaelic|Food and Beverage|medieval_food_gaelic_oat_bannock,medieval_food_gaelic_curd_bowl,medieval_food_gaelic_smoked_meat_packet,medieval_food_gaelic_ale_cup,medieval_food_gaelic_honey_cake,medieval_food_gaelic_pastoral_cheese,medieval_food_gaelic_fish_stew,medieval_food_gaelic_travel_ration
gaelic|Writing and Administration|medieval_writing_gaelic_boundary_tally,medieval_writing_gaelic_bardic_manuscript_leaf,medieval_writing_gaelic_kinship_record_strip,medieval_writing_gaelic_ring_pin_document_pouch,medieval_writing_gaelic_monastic_note_board,medieval_writing_gaelic_tribute_tally
gaelic|Household and Devotional|medieval_devotional_gaelic_ring_pin_shrine_cloth,medieval_music_gaelic_harp_prop,medieval_household_gaelic_hide_travel_bag,medieval_household_gaelic_pastoral_milk_vessel,medieval_household_gaelic_carved_oath_stick
carolingian|Clothing|medieval_clothing_carolingian_high_belted_tunic,medieval_clothing_carolingian_broad_banded_mantle,medieval_clothing_carolingian_court_cloak,medieval_clothing_carolingian_spatha_belt,medieval_clothing_carolingian_wool_leg_wraps,medieval_clothing_carolingian_clerical_dalmatic_robe,medieval_clothing_carolingian_embroidered_noble_tunic,medieval_clothing_carolingian_heavy_riding_cloak,medieval_clothing_carolingian_leather_boots,medieval_jewellery_carolingian_noble_fibula,medieval_clothing_carolingian_palace_servant_tunic,medieval_clothing_carolingian_monastic_cowl
carolingian|Military|medieval_weapon_carolingian_spatha,medieval_weapon_carolingian_war_spear,medieval_weapon_carolingian_light_axe,medieval_shield_carolingian_large_round_shield,medieval_military_carolingian_mail_shirt,medieval_military_carolingian_reinforced_helm,medieval_military_carolingian_riding_spurs,medieval_military_carolingian_palace_guard_spear
carolingian|Food and Beverage|medieval_food_carolingian_barley_bread,medieval_food_carolingian_pork_pottage,medieval_food_carolingian_ale_jug,medieval_food_carolingian_monastic_loaf,medieval_food_carolingian_salted_pork_packet,medieval_food_carolingian_feast_roast,medieval_food_carolingian_cheese_board,medieval_food_carolingian_honeyed_pastry
carolingian|Writing and Administration|medieval_writing_carolingian_capitulary_copy,medieval_writing_carolingian_estate_polyptych_leaf,medieval_writing_carolingian_palace_wax_tablet,medieval_writing_carolingian_monastic_codex,medieval_writing_carolingian_seal_tag_packet,medieval_writing_carolingian_tax_tally
carolingian|Household and Devotional|medieval_household_carolingian_palace_chest,medieval_household_carolingian_manor_table,medieval_devotional_carolingian_reliquary_box,medieval_household_carolingian_monastic_lectern,medieval_jewellery_carolingian_enamel_brooch
capetian|Clothing|medieval_clothing_capetian_wool_cote,medieval_clothing_capetian_bliaut_style_gown,medieval_clothing_capetian_lined_burgher_gown,medieval_clothing_capetian_wimple,medieval_clothing_capetian_linen_coif,medieval_clothing_capetian_fitted_hood,medieval_clothing_capetian_fine_purse_belt,medieval_clothing_capetian_guild_apron,medieval_clothing_capetian_court_mantle,medieval_clothing_capetian_silk_trimmed_surcoat,medieval_clothing_capetian_wool_hose,medieval_clothing_capetian_winter_mantle
capetian|Military|medieval_weapon_capetian_arming_sword,medieval_weapon_capetian_town_spear,medieval_weapon_capetian_crossbow,medieval_shield_capetian_heater_shield,medieval_military_capetian_mail_hauberk,medieval_military_capetian_padded_aketon,medieval_military_capetian_kettle_hat,medieval_military_capetian_guild_militia_belt
capetian|Food and Beverage|medieval_food_capetian_white_bread,medieval_food_capetian_onion_pottage,medieval_food_capetian_wine_cup,medieval_food_capetian_cheese_trencher,medieval_food_capetian_salted_fish_packet,medieval_food_capetian_feast_pastry,medieval_food_capetian_market_ration,medieval_food_capetian_spiced_meat_stew
capetian|Writing and Administration|medieval_writing_capetian_notarial_note,medieval_writing_capetian_sealed_letter,medieval_writing_capetian_guild_register,medieval_writing_capetian_town_toll_tally,medieval_writing_capetian_merchant_contract,medieval_writing_capetian_chapel_booklet
capetian|Household and Devotional|medieval_household_capetian_guild_counter,medieval_household_capetian_cloth_merchant_bale,medieval_household_capetian_town_chest,medieval_devotional_capetian_pilgrim_badge,medieval_household_capetian_stained_glass_quarry_panel
german_hre|Clothing|medieval_clothing_german_hre_guild_apron,medieval_clothing_german_hre_civic_gown,medieval_clothing_german_hre_fur_lined_mantle,medieval_clothing_german_hre_fitted_tunic,medieval_clothing_german_hre_alpine_felt_cap,medieval_clothing_german_hre_fine_hood,medieval_clothing_german_hre_merchant_belt,medieval_clothing_german_hre_arming_jack,medieval_clothing_german_hre_town_shoes,medieval_clothing_german_hre_winter_boots,medieval_clothing_german_hre_embroidered_church_robe,medieval_clothing_german_hre_court_hat
german_hre|Military|medieval_weapon_german_hre_arming_sword,medieval_weapon_german_hre_beaked_war_hammer,medieval_weapon_german_hre_town_spear,medieval_shield_german_hre_reinforced_heater_shield,medieval_military_german_hre_mail_hauberk,medieval_military_german_hre_plate_reinforced_fittings,medieval_weapon_german_hre_town_crossbow,medieval_military_german_hre_guild_armour_stand
german_hre|Food and Beverage|medieval_food_german_hre_rye_bread,medieval_food_german_hre_sausage_packet,medieval_food_german_hre_beer_mug,medieval_food_german_hre_cabbage_pottage,medieval_food_german_hre_cheese_board,medieval_food_german_hre_feast_roast,medieval_food_german_hre_market_ration,medieval_food_german_hre_spiced_wine_cup
german_hre|Writing and Administration|medieval_writing_german_hre_guild_mark_register,medieval_writing_german_hre_court_seal_packet,medieval_writing_german_hre_town_account_roll,medieval_writing_german_hre_trade_weight_note,medieval_writing_german_hre_monastery_codex,medieval_writing_german_hre_tax_tally
german_hre|Household and Devotional|medieval_household_german_hre_guild_chest,medieval_household_german_hre_beer_hall_bench,medieval_household_german_hre_wall_sconce,medieval_devotional_german_hre_reliquary_box,medieval_household_german_hre_locksmith_counter
iberian_christian|Clothing|medieval_clothing_iberian_christian_saya,medieval_clothing_iberian_christian_pellote,medieval_clothing_iberian_christian_manto,medieval_clothing_iberian_christian_toca_head_veil,medieval_clothing_iberian_christian_frontier_riding_cloak,medieval_clothing_iberian_christian_leather_boots,medieval_clothing_iberian_christian_narrow_sleeved_tunic,medieval_clothing_iberian_christian_knightly_surcoat,medieval_clothing_iberian_christian_noble_silk_belt,medieval_clothing_iberian_christian_pilgrim_cloak,medieval_clothing_iberian_christian_court_gown,medieval_clothing_iberian_christian_arming_coat
iberian_christian|Military|medieval_weapon_iberian_christian_war_sword,medieval_weapon_iberian_christian_frontier_spear,medieval_weapon_iberian_christian_javelin,medieval_shield_iberian_christian_almond_shield,medieval_military_iberian_christian_mail_shirt,medieval_military_iberian_christian_quilted_coat,medieval_military_iberian_christian_cavalry_spurs,medieval_military_iberian_christian_frontier_bow
iberian_christian|Food and Beverage|medieval_food_iberian_christian_wheat_bread,medieval_food_iberian_christian_olive_relish,medieval_food_iberian_christian_wine_cup,medieval_food_iberian_christian_chickpea_pottage,medieval_food_iberian_christian_salted_meat_ration,medieval_food_iberian_christian_honey_pastry,medieval_food_iberian_christian_feast_stew,medieval_food_iberian_christian_market_flatbread
iberian_christian|Writing and Administration|medieval_writing_iberian_christian_frontier_charter,medieval_writing_iberian_christian_seal_cord_packet,medieval_writing_iberian_christian_military_order_roll,medieval_writing_iberian_christian_toll_tally,medieval_writing_iberian_christian_notary_contract,medieval_writing_iberian_christian_chapel_booklet
iberian_christian|Household and Devotional|medieval_household_iberian_christian_frontier_chest,medieval_household_iberian_christian_olive_oil_jug,medieval_household_iberian_christian_pilgrim_staff_rack,medieval_devotional_iberian_christian_shrine_badge,medieval_household_iberian_christian_courtyard_lamp
andalusi|Clothing|medieval_clothing_andalusi_linen_qamis,medieval_clothing_andalusi_wool_sirwal,medieval_clothing_andalusi_burnous_cloak,medieval_clothing_andalusi_wrapped_turban,medieval_clothing_andalusi_veiled_headcloth,medieval_clothing_andalusi_qaba_caftan,medieval_clothing_andalusi_tiraz_banded_robe,medieval_clothing_andalusi_soft_slippers,medieval_clothing_andalusi_merchant_sash,medieval_clothing_andalusi_scholar_robe,medieval_clothing_andalusi_riding_cloak,medieval_clothing_andalusi_embroidered_belt_pouch
andalusi|Military|medieval_weapon_andalusi_saif,medieval_weapon_andalusi_light_spear,medieval_weapon_andalusi_composite_bow,medieval_shield_andalusi_hide_adarga,medieval_military_andalusi_quilted_coat,medieval_military_andalusi_turban_helm,medieval_military_andalusi_horseman_quiver,medieval_military_andalusi_leather_bracers
andalusi|Food and Beverage|medieval_food_andalusi_flatbread,medieval_food_andalusi_lentil_stew,medieval_food_andalusi_date_sweet,medieval_food_andalusi_yogurt_bowl,medieval_food_andalusi_spiced_meat_dish,medieval_food_andalusi_syrup_drink,medieval_food_andalusi_oil_herb_relish,medieval_food_andalusi_market_ration
andalusi|Writing and Administration|medieval_writing_andalusi_paper_contract,medieval_writing_andalusi_office_seal_packet,medieval_writing_andalusi_scholar_notebook,medieval_writing_andalusi_market_ledger_leaf,medieval_writing_andalusi_quran_stand_note,medieval_writing_andalusi_tax_order
andalusi|Household and Devotional|medieval_household_andalusi_glazed_bowl,medieval_household_andalusi_brass_lamp,medieval_household_andalusi_writing_box,medieval_household_andalusi_carved_screen_panel,medieval_household_andalusi_perfume_oil_flask
byzantine|Clothing|medieval_clothing_byzantine_silk_dalmatic,medieval_clothing_byzantine_skaramangion_riding_coat,medieval_clothing_byzantine_sagion_cloak,medieval_clothing_byzantine_court_belt,medieval_clothing_byzantine_formal_silk_robe,medieval_clothing_byzantine_veil_headcloth,medieval_clothing_byzantine_embroidered_cuffs,medieval_clothing_byzantine_icon_pouch,medieval_clothing_byzantine_military_padded_tunic,medieval_clothing_byzantine_lamellar_under_robe,medieval_clothing_byzantine_soft_boots,medieval_clothing_byzantine_monastic_robe
byzantine|Military|medieval_weapon_byzantine_paramerion,medieval_weapon_byzantine_guard_spear,medieval_weapon_byzantine_composite_bow,medieval_shield_byzantine_painted_oval_shield,medieval_military_byzantine_lamellar_corselet,medieval_military_byzantine_mail_coif,medieval_military_byzantine_military_belt,medieval_military_byzantine_icon_marked_banner
byzantine|Food and Beverage|medieval_food_byzantine_wheat_bread,medieval_food_byzantine_olive_dish,medieval_food_byzantine_wine_cup,medieval_food_byzantine_fish_sauce_relish,medieval_food_byzantine_cheese_dish,medieval_food_byzantine_feast_fish_platter,medieval_food_byzantine_monastery_ration,medieval_food_byzantine_spiced_stew
byzantine|Writing and Administration|medieval_writing_byzantine_chrysobull_copy,medieval_writing_byzantine_icon_label_tablet,medieval_writing_byzantine_monastery_codex,medieval_writing_byzantine_sealed_packet,medieval_writing_byzantine_tax_register,medieval_writing_byzantine_court_order_roll
byzantine|Household and Devotional|medieval_devotional_byzantine_icon_panel,medieval_household_byzantine_hanging_lamp,medieval_jewellery_byzantine_enamel_pendant,medieval_devotional_byzantine_silk_altar_cloth,medieval_devotional_byzantine_bronze_censer
abbasid|Clothing|medieval_clothing_abbasid_linen_qamis,medieval_clothing_abbasid_qaba_caftan,medieval_clothing_abbasid_sirwal,medieval_clothing_abbasid_wrapped_turban,medieval_clothing_abbasid_scholar_robe,medieval_clothing_abbasid_belted_court_robe,medieval_clothing_abbasid_wrapped_sash,medieval_clothing_abbasid_soft_slippers,medieval_clothing_abbasid_military_riding_coat,medieval_clothing_abbasid_fine_headcloth,medieval_clothing_abbasid_merchant_robe,medieval_clothing_abbasid_book_pouch
abbasid|Military|medieval_weapon_abbasid_straight_sword,medieval_weapon_abbasid_guard_spear,medieval_weapon_abbasid_composite_bow,medieval_shield_abbasid_round_dhal,medieval_military_abbasid_lamellar_coat,medieval_military_abbasid_padded_sleeves,medieval_military_abbasid_horseman_quiver,medieval_military_abbasid_guard_belt
abbasid|Food and Beverage|medieval_food_abbasid_flatbread,medieval_food_abbasid_rice_pilaf,medieval_food_abbasid_sour_milk_bowl,medieval_food_abbasid_date_syrup_sweet,medieval_food_abbasid_spiced_meat_dish,medieval_food_abbasid_sherbet_cup,medieval_food_abbasid_lentil_stew,medieval_food_abbasid_market_ration
abbasid|Writing and Administration|medieval_writing_abbasid_paper_decree,medieval_writing_abbasid_scholar_notebook,medieval_writing_abbasid_chancery_seal_packet,medieval_writing_abbasid_ink_case,medieval_writing_abbasid_waqf_record,medieval_writing_abbasid_market_contract
abbasid|Household and Devotional|medieval_household_abbasid_brass_lamp,medieval_household_abbasid_glazed_bowl,medieval_household_abbasid_writing_box,medieval_household_abbasid_book_stand,medieval_household_abbasid_perfume_flask
fatimid|Clothing|medieval_clothing_fatimid_linen_robe,medieval_clothing_fatimid_tiraz_banded_tunic,medieval_clothing_fatimid_cotton_wrap,medieval_clothing_fatimid_wrapped_turban,medieval_clothing_fatimid_court_kaftan,medieval_clothing_fatimid_veiled_headcloth,medieval_clothing_fatimid_linen_merchant_robe,medieval_clothing_fatimid_light_sandals,medieval_clothing_fatimid_noble_sash,medieval_clothing_fatimid_scribe_robe,medieval_clothing_fatimid_guard_padded_coat,medieval_clothing_fatimid_devotional_amulet_cord
fatimid|Military|medieval_weapon_fatimid_guard_spear,medieval_weapon_fatimid_saif,medieval_weapon_fatimid_composite_bow,medieval_shield_fatimid_round_hide_dhal,medieval_military_fatimid_padded_coat_scale_panels,medieval_military_fatimid_guard_helmet,medieval_military_fatimid_archer_quiver,medieval_military_fatimid_palace_guard_belt
fatimid|Food and Beverage|medieval_food_fatimid_flatbread,medieval_food_fatimid_lentil_bowl,medieval_food_fatimid_date_sweet,medieval_food_fatimid_oil_relish,medieval_food_fatimid_spiced_fish_dish,medieval_food_fatimid_syrup_drink,medieval_food_fatimid_market_ration,medieval_food_fatimid_feast_stew
fatimid|Writing and Administration|medieval_writing_fatimid_tax_roll,medieval_writing_fatimid_tiraz_workshop_tag,medieval_writing_fatimid_paper_order,medieval_writing_fatimid_merchant_contract,medieval_writing_fatimid_mosque_endowment_note,medieval_writing_fatimid_seal_packet
fatimid|Household and Devotional|medieval_household_fatimid_glass_lamp,medieval_household_fatimid_ivory_box,medieval_household_fatimid_linen_bale,medieval_household_fatimid_red_sea_trade_casket,medieval_devotional_fatimid_prayer_bead_strand
seljuk_ayyubid|Clothing|medieval_clothing_seljuk_ayyubid_riding_caftan,medieval_clothing_seljuk_ayyubid_quilted_coat,medieval_clothing_seljuk_ayyubid_sirwal,medieval_clothing_seljuk_ayyubid_wrapped_turban,medieval_clothing_seljuk_ayyubid_high_riding_boots,medieval_clothing_seljuk_ayyubid_bowcase_belt,medieval_clothing_seljuk_ayyubid_lamellar_coat_cover,medieval_clothing_seljuk_ayyubid_scholar_robe,medieval_clothing_seljuk_ayyubid_merchant_sash,medieval_clothing_seljuk_ayyubid_fur_cap,medieval_clothing_seljuk_ayyubid_cavalry_cloak,medieval_clothing_seljuk_ayyubid_leather_gloves
seljuk_ayyubid|Military|medieval_weapon_seljuk_ayyubid_flanged_mace,medieval_weapon_seljuk_ayyubid_curved_sabre,medieval_weapon_seljuk_ayyubid_cavalry_lance,medieval_weapon_seljuk_ayyubid_composite_bow,medieval_shield_seljuk_ayyubid_round_cavalry_dhal,medieval_military_seljuk_ayyubid_lamellar_riding_coat,medieval_military_seljuk_ayyubid_mail_aventail_helmet,medieval_military_seljuk_ayyubid_bowcase_quiver
seljuk_ayyubid|Food and Beverage|medieval_food_seljuk_ayyubid_flatbread,medieval_food_seljuk_ayyubid_yogurt_bowl,medieval_food_seljuk_ayyubid_pilaf,medieval_food_seljuk_ayyubid_spiced_meat,medieval_food_seljuk_ayyubid_dried_fruit_packet,medieval_food_seljuk_ayyubid_sour_milk_cup,medieval_food_seljuk_ayyubid_market_ration,medieval_food_seljuk_ayyubid_feast_stew
seljuk_ayyubid|Writing and Administration|medieval_writing_seljuk_ayyubid_iqta_record,medieval_writing_seljuk_ayyubid_madrasa_book,medieval_writing_seljuk_ayyubid_sealed_order,medieval_writing_seljuk_ayyubid_cavalry_muster_roll,medieval_writing_seljuk_ayyubid_paper_contract,medieval_writing_seljuk_ayyubid_court_seal_tag
seljuk_ayyubid|Household and Devotional|medieval_household_seljuk_ayyubid_riding_saddlebag,medieval_household_seljuk_ayyubid_brass_lamp,medieval_household_seljuk_ayyubid_prayer_rug_prop,medieval_household_seljuk_ayyubid_inlaid_bowl,medieval_household_seljuk_ayyubid_madrasa_book_stand
rus_novgorod|Clothing|medieval_clothing_rus_novgorod_rubakha_tunic,medieval_clothing_rus_novgorod_fur_edged_kaftan,medieval_clothing_rus_novgorod_porty_trousers,medieval_clothing_rus_novgorod_onuchi_footwraps,medieval_clothing_rus_novgorod_fur_hat,medieval_clothing_rus_novgorod_leather_boots,medieval_clothing_rus_novgorod_cloak_with_pin,medieval_clothing_rus_novgorod_birchbark_document_pouch,medieval_clothing_rus_novgorod_orthodox_pendant_cord,medieval_clothing_rus_novgorod_warrior_belt,medieval_clothing_rus_novgorod_river_trader_coat,medieval_clothing_rus_novgorod_wool_mittens
rus_novgorod|Military|medieval_weapon_rus_novgorod_war_axe,medieval_weapon_rus_novgorod_socketed_spear,medieval_weapon_rus_novgorod_broad_sword,medieval_weapon_rus_novgorod_bow,medieval_shield_rus_novgorod_tall_oval_shield,medieval_military_rus_novgorod_mail_shirt,medieval_military_rus_novgorod_fur_trimmed_helmet,medieval_military_rus_novgorod_river_guard_quiver
rus_novgorod|Food and Beverage|medieval_food_rus_novgorod_rye_bread,medieval_food_rus_novgorod_mushroom_fish_stew,medieval_food_rus_novgorod_honey_drink_cup,medieval_food_rus_novgorod_smoked_fish_packet,medieval_food_rus_novgorod_curd_cheese_bowl,medieval_food_rus_novgorod_river_trader_ration,medieval_food_rus_novgorod_pirog_pastry,medieval_food_rus_novgorod_pickle_crock
rus_novgorod|Writing and Administration|medieval_writing_rus_novgorod_birchbark_letter,medieval_writing_rus_novgorod_princely_seal_tag,medieval_writing_rus_novgorod_river_trade_tally,medieval_writing_rus_novgorod_orthodox_prayer_slip,medieval_writing_rus_novgorod_wax_tablet,medieval_writing_rus_novgorod_fur_tax_record
rus_novgorod|Household and Devotional|medieval_household_rus_novgorod_icon_shelf,medieval_household_rus_novgorod_birchbark_letter_box,medieval_household_rus_novgorod_fur_lined_chest,medieval_household_rus_novgorod_honey_drink_crock,medieval_trade_rus_novgorod_river_balance_case
steppe_turkic|Clothing|medieval_clothing_steppe_turkic_felt_riding_caftan,medieval_clothing_steppe_turkic_tied_riding_coat,medieval_clothing_steppe_turkic_riding_trousers,medieval_clothing_steppe_turkic_high_boots,medieval_clothing_steppe_turkic_fur_cap,medieval_clothing_steppe_turkic_bowcase_quiver_belt,medieval_clothing_steppe_turkic_felt_cloak,medieval_clothing_steppe_turkic_sash,medieval_clothing_steppe_turkic_lamellar_coat_cover,medieval_clothing_steppe_turkic_horseman_gloves,medieval_clothing_steppe_turkic_winter_mittens,medieval_clothing_steppe_turkic_travel_pouch
steppe_turkic|Military|medieval_weapon_steppe_turkic_curved_sabre,medieval_weapon_steppe_turkic_composite_bow,medieval_weapon_steppe_turkic_lance,medieval_weapon_steppe_turkic_mace,medieval_shield_steppe_turkic_compact_hide_shield,medieval_military_steppe_turkic_lamellar_coat,medieval_military_steppe_turkic_fur_cap_helmet,medieval_military_steppe_turkic_bowcase_quiver
steppe_turkic|Food and Beverage|medieval_food_steppe_turkic_millet_gruel,medieval_food_steppe_turkic_dried_curds,medieval_food_steppe_turkic_kumis_skin,medieval_food_steppe_turkic_dried_meat_packet,medieval_food_steppe_turkic_riding_ration,medieval_food_steppe_turkic_boiled_meat_bowl,medieval_food_steppe_turkic_felt_wrapped_travel_food,medieval_food_steppe_turkic_mare_milk_vessel
steppe_turkic|Writing and Administration|medieval_writing_steppe_turkic_paiza_tag_prop,medieval_writing_steppe_turkic_sealed_leather_pouch,medieval_writing_steppe_turkic_herd_tally,medieval_writing_steppe_turkic_tribute_strip,medieval_writing_steppe_turkic_bowcase_ownership_tag,medieval_writing_steppe_turkic_messenger_packet
steppe_turkic|Household and Devotional|medieval_household_steppe_turkic_felt_tent_panel,medieval_household_steppe_turkic_saddlebag,medieval_household_steppe_turkic_kumis_skin,medieval_household_steppe_turkic_bowcase_rack,medieval_household_steppe_turkic_felt_carpet
song_china|Clothing|medieval_clothing_song_china_cross_collar_robe,medieval_clothing_song_china_scholar_robe,medieval_clothing_song_china_official_cap,medieval_clothing_song_china_cloth_shoes,medieval_clothing_song_china_padded_winter_robe,medieval_clothing_song_china_silk_sash,medieval_clothing_song_china_tea_house_robe,medieval_clothing_song_china_merchant_robe,medieval_clothing_song_china_servant_jacket,medieval_clothing_song_china_narrow_trousers,medieval_clothing_song_china_writing_sleeve_pouch,medieval_clothing_song_china_military_padded_vest
song_china|Military|medieval_weapon_song_china_single_edged_dao,medieval_weapon_song_china_qiang_spear,medieval_weapon_song_china_military_crossbow,medieval_shield_song_china_lacquered_rattan_shield,medieval_military_song_china_lamellar_vest,medieval_military_song_china_padded_military_robe,medieval_weapon_song_china_militia_bow,medieval_military_song_china_guard_baton
song_china|Food and Beverage|medieval_food_song_china_rice_bowl,medieval_food_song_china_wheat_noodle_bowl,medieval_food_song_china_tea_cup,medieval_food_song_china_pickled_greens_jar,medieval_food_song_china_steamed_bun,medieval_food_song_china_fish_dish,medieval_food_song_china_scholar_snack_box,medieval_food_song_china_market_ration
song_china|Writing and Administration|medieval_writing_song_china_paper_register,medieval_writing_song_china_printed_notice,medieval_writing_song_china_scholar_notebook,medieval_writing_song_china_official_chop_document,medieval_writing_song_china_tea_house_account_slip,medieval_writing_song_china_examination_essay_booklet
song_china|Household and Devotional|medieval_household_song_china_tea_cup,medieval_household_song_china_lacquer_writing_box,medieval_household_song_china_porcelain_bowl,medieval_household_song_china_scholar_brush_rest,medieval_household_song_china_printed_notice_board
";

	private static readonly MedievalCultureCatalogue[] MedievalExplicitCultureCatalogues =
		BuildMedievalCultureCatalogues();

	private static readonly MedievalStatusRoleProfile[] MedievalStatusRoleProfiles =
	[
		new("peasant", "Peasant", "Peasant", "work_tunic", "tunic", "a coarse wool work tunic",
			"This work tunic is cut for hard use, with patched stress points, plain hems, and enough length for field or household labour.",
			"wool", MaterialBehaviourType.Fabric, ItemQuality.Standard, 780.0, 16.0m,
			"Market / Clothing / Simple Clothing", "Wear_Tunic", "Armour_LightClothing"),
		new("artisan", "Artisan", "Artisan", "apron_tunic", "tunic", "a sturdy apron-front tunic",
			"This sturdy tunic has an apron-like working front, close sleeves, and reinforced seams suited to workshop labour.",
			"linen", MaterialBehaviourType.Fabric, ItemQuality.Standard, 620.0, 22.0m,
			"Market / Clothing / Standard Clothing", "Wear_Apron", "Armour_LightClothing"),
		new("merchant", "Merchant/Burgher", "Merchant Burgher", "lined_gown", "gown", "a lined merchant gown",
			"This lined gown is practical but respectable, with a neat fall, better cloth, and closures that can sit cleanly under a belt.",
			"wool", MaterialBehaviourType.Fabric, ItemQuality.Good, 980.0, 42.0m,
			"Market / Clothing / Standard Clothing", "Wear_Gown", "Armour_LightClothing"),
		new("noble", "Noble/Court", "Noble Court", "silk_surcoat", "surcoat", "a fine court surcoat",
			"This fine surcoat uses better cloth and careful finishing, with a long visible line suitable for court, ceremony, or formal attendance.",
			"silk", MaterialBehaviourType.Fabric, ItemQuality.Good, 540.0, 95.0m,
			"Market / Clothing / Luxury Clothing", "Wear_Mantle", "Armour_LightClothing"),
		new("clergy", "Clergy/Monastic", "Clergy Monastic", "clerical_robe", "robe", "a plain clerical robe",
			"This long robe is deliberately modest, with a sober fall, serviceable stitching, and room for a cord, belt, or devotional token.",
			"wool", MaterialBehaviourType.Fabric, ItemQuality.Standard, 1100.0, 34.0m,
			"Market / Clothing / Standard Clothing", "Wear_Robe", "Armour_LightClothing"),
		new("military", "Military", "Military", "arming_coat", "coat", "a padded arming coat",
			"This padded coat is quilted for armour wear, with close sleeves, reinforced ties, and enough thickness to cushion mail or lamellar.",
			"linen", MaterialBehaviourType.Fabric, ItemQuality.Standard, 1600.0, 48.0m,
			"Market / Clothing / Winter Clothing", "Wear_Long-Sleeved_Tunic", "Armour_HeavyClothing")
	];

	private static readonly string[] MedievalWardrobeSlotKeys =
	[
		"underlayer",
		"headwear",
		"hood",
		"outerwear",
		"legwear",
		"handwear",
		"sockwear",
		"footwear",
		"belt",
		"pouch"
	];

	private const string EraOutfitSlotSpecUnderlayer = "underlayer";
	private const string EraOutfitSlotSpecLowerBody = "lower_body";
	private const string EraOutfitSlotSpecLegOrSockLayer = "leg_or_sock_layer";
	private const string EraOutfitSlotSpecFootwear = "footwear";
	private const string EraOutfitSlotSpecBodywear = "bodywear";
	private const string EraOutfitSlotSpecOuterwear = "outerwear";
	private const string EraOutfitSlotSpecHeadwear = "headwear";
	private const string EraOutfitSlotSpecBeltOrSash = "belt_or_sash";
	private const string EraOutfitSlotSpecWornContainer = "worn_container";
	private const string EraOutfitSlotSpecFastenerOrJewellery = "fastener_or_jewellery";
	private const string EraOutfitSlotSpecRoleItem = "role_item";

	private static readonly EraOutfitSlotSpec[] EraOutfitSlotSpecs =
	[
		new(EraOutfitSlotSpecUnderlayer, "Underlayer", true, []),
		new(EraOutfitSlotSpecLowerBody, "Lower Body", true, []),
		new(EraOutfitSlotSpecLegOrSockLayer, "Leg/Sock Layer", true, []),
		new(EraOutfitSlotSpecFootwear, "Footwear", true, []),
		new(EraOutfitSlotSpecBodywear, "Bodywear", true, []),
		new(EraOutfitSlotSpecOuterwear, "Outerwear", true, []),
		new(EraOutfitSlotSpecHeadwear, "Headwear", true, []),
		new(EraOutfitSlotSpecBeltOrSash, "Belt or Sash", true, []),
		new(EraOutfitSlotSpecWornContainer, "Worn Container", true, []),
		new(EraOutfitSlotSpecFastenerOrJewellery, "Fastener/Jewellery", true, []),
		new(EraOutfitSlotSpecRoleItem, "Role Item", false, ["merchant", "religious", "military"])
	];

	private static readonly EraCultureSpec[] MedievalEraCultureSpecs =
		MedievalCultureProfiles
			.Select(x => new EraCultureSpec(x.Key, x.Display, SafeMedievalTagName(x.Display)))
			.ToArray();

	private static readonly EraSeederConfiguration MedievalEraConfiguration = new(
		"medieval",
		MedievalRootTagPath,
		MedievalCultureTagRoot,
		MedievalStatusTagRoot,
		"medieval",
		[
			"Market / Clothing / Simple Clothing",
			"Market / Clothing / Standard Clothing",
			"Market / Clothing / Luxury Clothing",
			"Market / Clothing / Footwear",
			"Market / Clothing / Winter Clothing"
		],
		new EraVariableColourPolicy(
			"Variable_FineColour",
			"Variable_2FineColour",
			["$colour"],
			["$colour1", "$colour2"],
			["paper", "oak", "bronze", "silver", "gold", "wrought iron", "beeswax"]),
		[
			HistoricFoundationKnowledge,
			MedievalWorkshopKnowledge,
			MedievalClothingKnowledgePrefix
		],
		["Tailoring", "Leathermaking", "Metalworking", "Writing", "Carpentry"],
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["garment_cloth"] = "Garment Cloth",
			["tablet_woven_band"] = "Tablet-Woven Band Stock",
			["embroidered_trim"] = "Embroidered Trim Stock",
			["fur_panel"] = "Fur Panel Stock",
			["felt_cloth"] = "Fulled Cloth",
			["turnshoe_upper"] = "Turnshoe Upper Stock",
			["paper_sheet"] = "Paper Sheet Stock",
			["bookbinding_leather"] = "Bookbinding Leather Stock",
			["tool_blank"] = "Tool Blank Stock",
			["quilted_padding"] = "Quilted Armour Padding"
		},
		EraOutfitSlotSpecs,
		true,
		true,
		false);

	private static readonly string[] MedievalOutfitSexGenderPresentationKeys = ["male", "female"];

	private static readonly string[] MedievalOutfitSocialClassRoleKeys =
	[
		"peasant",
		"artisan",
		"merchant",
		"noble",
		"religious",
		"military"
	];

	private static readonly IReadOnlyDictionary<string, string> MedievalOutfitRoleToStatusRoleKey =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["peasant"] = "peasant",
			["artisan"] = "artisan",
			["merchant"] = "merchant",
			["noble"] = "noble",
			["religious"] = "clergy",
			["military"] = "military"
		};

	private static readonly string[] MedievalNorthAtlanticOutfitCultureKeys =
	[
		"early_anglo_saxon",
		"anglo_danish",
		"norse",
		"norman",
		"high_british",
		"gaelic"
	];

	private static readonly string[] MedievalContinentalWesternOutfitCultureKeys =
	[
		"carolingian",
		"capetian",
		"german_hre",
		"iberian_christian"
	];

	private static readonly string[] MedievalEasternOutfitCultureKeys =
	[
		"andalusi",
		"byzantine",
		"abbasid",
		"fatimid",
		"seljuk_ayyubid",
		"rus_novgorod",
		"steppe_turkic",
		"song_china"
	];

	private static readonly string[] MedievalExplicitOutfitCultureKeys =
		MedievalNorthAtlanticOutfitCultureKeys
			.Concat(MedievalContinentalWesternOutfitCultureKeys)
			.Concat(MedievalEasternOutfitCultureKeys)
			.ToArray();

	private const string MedievalExplicitOutfitPieceSource = @"
medieval_outfit_early_anglo_saxon_male_peasant|linen shirt; wool braies; wool leg wraps; soft ankle shoes; tablet-banded wool tunic; square work cloak; wool cap; rope belt; small belt pouch; plain disc brooch
medieval_outfit_early_anglo_saxon_female_peasant|linen shift; wool wrap skirt; wool footwraps; soft ankle shoes; tablet-banded wool gown; square cloak; linen head veil; woven girdle; small pouch; simple cloak brooch
medieval_outfit_early_anglo_saxon_male_artisan|linen work shirt; wool braies; wool leg wraps; leather shoes; short sleeved work tunic; workshop cloak; wool cap; seax belt; tool pouch; iron cloak pin
medieval_outfit_early_anglo_saxon_female_artisan|linen shift; wool work skirt; wool footwraps; leather shoes; belted work gown; apron; linen headcloth; woven belt; tool pouch; bronze ring pin
medieval_outfit_early_anglo_saxon_male_merchant|fine linen shirt; wool hose; leather shoes; bordered tunic; lined mantle; felt cap; leather purse belt; document pouch; silver cloak brooch; counting tally cord
medieval_outfit_early_anglo_saxon_female_merchant|fine linen shift; wool skirt; leather shoes; bordered gown; lined mantle; linen veil; decorated girdle; belt purse; silver brooch; bead necklace
medieval_outfit_early_anglo_saxon_male_noble|fine linen undertunic; wool hose; soft leather shoes; embroidered noble tunic; rich mantle; decorated cap; seax belt with mounts; document pouch; enamel brooch; bead necklace
medieval_outfit_early_anglo_saxon_female_noble|fine linen shift; embroidered undergown; soft shoes; embroidered noble gown; brooch-fastened mantle; long linen veil; decorated girdle; alms purse; enamel brooch; bead necklace
medieval_outfit_early_anglo_saxon_male_religious|plain linen undertunic; wool hose; sandals; monastic wool habit; heavy cowl cloak; tonsure cap or hood; cord belt; book pouch; wooden cross; wax tablet
medieval_outfit_early_anglo_saxon_female_religious|plain linen shift; wool undergown; sandals; monastic robe; heavy cowl cloak; linen veil; cord belt; book pouch; wooden cross; prayer tablet
medieval_outfit_early_anglo_saxon_male_military|arming shirt; wool braies; leg wraps; leather boots; padded shield-wall tunic; war cloak; padded cap; seax belt; field pouch; shield brooch; archer bracer
medieval_outfit_early_anglo_saxon_female_military|arming shift; wool trews or skirted leg wraps; leather boots; padded war gown; brooch-fastened war cloak; linen headwrap; arming belt; field pouch; cloak brooch; leather bracers
medieval_outfit_anglo_danish_male_peasant|linen shirt; wool braies; wrapped trews; rough shoes; panelled wool tunic; rough cloak; wool hood; rope belt; belt pouch; plain ring pin
medieval_outfit_anglo_danish_female_peasant|linen shift; wool skirt; footwraps; rough shoes; panelled wool gown; rough cloak; head rail; woven belt; belt pouch; bronze ring pin
medieval_outfit_anglo_danish_male_artisan|work shirt; wool trews; leather shoes; narrow-braid tunic; work cloak; fitted cap; long seax belt; tool pouch; iron cloak pin; workshop mitts
medieval_outfit_anglo_danish_female_artisan|work shift; wool skirt; leather shoes; narrow-braid work gown; apron; head rail; leather belt; tool pouch; bronze brooch; sleeve ties
medieval_outfit_anglo_danish_male_merchant|fine shirt; wool hose; town shoes; embroidered collar tunic; lined mantle; felt cap; purse belt; reeve tally pouch; silver ring pin; seal cord
medieval_outfit_anglo_danish_female_merchant|fine shift; wool gown; town shoes; embroidered collar overgown; lined mantle; linen head rail; decorated girdle; purse; silver brooch; bead string
medieval_outfit_anglo_danish_male_noble|fine undertunic; wool hose; soft boots; panelled noble tunic; fur-edged cloak; decorated cap; mounted seax belt; document pouch; silver brooch; gold bead string
medieval_outfit_anglo_danish_female_noble|fine shift; embroidered gown; soft shoes; panelled overgown; fur-edged cloak; long veil; decorated girdle; alms purse; silver brooch; bead necklace
medieval_outfit_anglo_danish_male_religious|plain undertunic; wool hose; sandals; clerical robe; cowl; linen cap; cord belt; book pouch; wooden cross; wax tablet
medieval_outfit_anglo_danish_female_religious|plain shift; wool gown; sandals; modest robe; cowl cloak; linen veil; cord belt; book pouch; wooden cross; prayer strip
medieval_outfit_anglo_danish_male_military|arming shirt; wrapped trews; boots; padded shield-wall tunic; heavy cloak; nasal cap liner; long seax belt; field pouch; war brooch; leather bracers
medieval_outfit_anglo_danish_female_military|arming shift; wrapped trews; boots; padded shield-wall gown; heavy cloak; head rail under cap; arming belt; field pouch; war brooch; leather bracers
medieval_outfit_norse_male_peasant|linen shirt; wool trousers; leg wraps; rough shoes; plain wool tunic; sea cloak; wool cap; leather belt; belt pouch; simple cloak pin
medieval_outfit_norse_female_peasant|linen underdress; wool lower skirt; leg wraps; rough shoes; hangerok apron dress; sea cloak; wool headcloth; woven belt; small pouch; oval brooch pair
medieval_outfit_norse_male_artisan|work shirt; wool trousers; leg wraps; leather shoes; trader tunic; short work cloak; wool cap; tool belt; tool pouch; ring pin
medieval_outfit_norse_female_artisan|linen underdress; wool skirt; leg wraps; leather shoes; work hangerok; apron; headcloth; woven belt; tool pouch; oval brooch pair
medieval_outfit_norse_male_merchant|fine shirt; wool trousers; leg wraps; polished shoes; trader kaftan; fur-edged sea cloak; cap; decorated belt; trade pouch; runic trade tag
medieval_outfit_norse_female_merchant|fine underdress; wool dress; leg wraps; polished shoes; bead-strung hangerok; fur-edged cloak; headcloth; decorated belt; purse; oval brooch pair and beads
medieval_outfit_norse_male_noble|fine linen shirt; wool trousers; high boots; decorated kaftan; fur-lined cloak; embroidered cap; silver-mounted belt; document pouch; silver brooch; bead necklace
medieval_outfit_norse_female_noble|fine underdress; embroidered gown; high shoes; rich hangerok; fur-lined cloak; silk head veil; decorated girdle; alms purse; oval brooch pair; bead strand
medieval_outfit_norse_male_religious|plain shirt; wool trousers; sandals; simple robe; heavy cloak; hood; cord belt; book pouch; wooden cross or amulet; wax tablet
medieval_outfit_norse_female_religious|plain shift; wool gown; sandals; modest robe; heavy cloak; linen veil; cord belt; book pouch; wooden cross or amulet; prayer tablet
medieval_outfit_norse_male_military|arming shirt; wool trousers; leg wraps; high boots; arming tunic; heavy sea cloak; padded cap; weapon belt; field pouch; axe loop; leather bracers
medieval_outfit_norse_female_military|arming shift; wool trousers or split skirt; leg wraps; high boots; arming hangerok or tunic; heavy sea cloak; headwrap under cap; weapon belt; field pouch; leather bracers
medieval_outfit_norman_male_peasant|linen shirt; braies; wool hose; rough shoes; plain long tunic; hooded cloak; linen coif; rope belt; belt pouch; simple cloak clasp
medieval_outfit_norman_female_peasant|linen shift; wool skirt; wool hose; rough shoes; plain gown; hooded cloak; linen veil; woven belt; belt pouch; simple cloak clasp
medieval_outfit_norman_male_artisan|work shirt; braies; hose; leather shoes; fitted work tunic; short cloak; coif; tool belt; tool pouch; iron buckle
medieval_outfit_norman_female_artisan|work shift; wool skirt; hose; leather shoes; work gown; apron; head veil; leather belt; tool pouch; bronze brooch
medieval_outfit_norman_male_merchant|fine shirt; braies; fitted hose; town shoes; long-sleeved cote; lined mantle; coif and cap; purse belt; document pouch; cloak clasp
medieval_outfit_norman_female_merchant|fine chemise; fitted gown; hose; town shoes; bliaut-style overgown; lined mantle; wimple and veil; girdle; purse; cloak brooch
medieval_outfit_norman_male_noble|fine undertunic; fitted hose; soft boots; split riding tunic; court mantle; decorated cap; mounted belt; seal pouch; rich cloak clasp; gloves
medieval_outfit_norman_female_noble|fine chemise; court bliaut; fine hose; soft shoes; noble mantle; wimple and veil; jeweled girdle; alms purse; cloak clasp; embroidered sleeve ties
medieval_outfit_norman_male_religious|linen undertunic; wool hose; sandals; clerical robe; cowl cloak; coif; cord belt; book pouch; cross pendant; wax tablet
medieval_outfit_norman_female_religious|linen shift; wool undergown; sandals; religious robe; cowl cloak; veil and wimple; cord belt; book pouch; cross pendant; prayer slip
medieval_outfit_norman_male_military|arming shirt; braies; chausses; riding boots; padded aketon; mail surcoat; nasal arming coif; arming belt; field pouch; scabbard harness
medieval_outfit_norman_female_military|arming shift; split riding skirt or chausses; riding boots; padded aketon gown; mail surcoat; nasal arming coif and veil; arming belt; field pouch; scabbard harness
medieval_outfit_high_british_male_peasant|linen shirt; braies; wool hose; rough shoes; wool cote; rough cloak; linen coif; rope belt; belt pouch; simple clasp
medieval_outfit_high_british_female_peasant|linen shift; wool kirtle; hose; rough shoes; plain gown; rough cloak; linen headcloth; woven belt; belt pouch; simple clasp
medieval_outfit_high_british_male_artisan|work shirt; braies; hose; leather shoes; work cote; hood; coif; tool belt; tool pouch; work gloves
medieval_outfit_high_british_female_artisan|work shift; kirtle; hose; leather shoes; work gown; apron; headcloth; leather belt; tool pouch; pin brooch
medieval_outfit_high_british_male_merchant|fine shirt; braies; fitted hose; polished shoes; lined cote; travel mantle; hood; purse belt; document pouch; guild badge
medieval_outfit_high_british_female_merchant|fine chemise; kirtle; fitted hose; polished shoes; merchant gown; lined mantle; wimple; decorated girdle; purse; guild badge
medieval_outfit_high_british_male_noble|fine undertunic; fitted hose; soft shoes; silk-trimmed surcoat; fur-lined mantle; court cap; jeweled belt; alms purse; cloak brooch; gloves
medieval_outfit_high_british_female_noble|fine chemise; court gown; fine hose; soft shoes; silk-trimmed overgown; fur mantle; wimple and veil; jeweled girdle; alms purse; brooch
medieval_outfit_high_british_male_religious|linen undertunic; wool hose; sandals; clerical robe; cowl cloak; coif; cord belt; book pouch; cross pendant; small prayer book
medieval_outfit_high_british_female_religious|linen shift; wool robe; sandals; nun's habit or religious gown; cowl cloak; veil and wimple; cord belt; book pouch; cross pendant; prayer book
medieval_outfit_high_british_male_military|arming shirt; braies; padded chausses; riding boots; gambeson; surcoat; arming coif; arming belt; field pouch; archer bracer
medieval_outfit_high_british_female_military|arming shift; chausses or split riding skirt; boots; fitted gambeson; surcoat; arming coif with headcloth; arming belt; field pouch; archer bracer
medieval_outfit_gaelic_male_peasant|linen long shirt; wool trews; footwraps; deerskin shoes; plain leine-style tunic; brat mantle; wool cap; woven belt; pastoral pouch; ring pin
medieval_outfit_gaelic_female_peasant|linen shift; wool wrap skirt; footwraps; deerskin shoes; long wool gown; brat mantle; linen headcloth; woven belt; pastoral pouch; ring pin
medieval_outfit_gaelic_male_artisan|work shirt; wool trews; leather shoes; work long shirt; short hill cloak; wool cap; tool belt; tool pouch; ring pin; mitts
medieval_outfit_gaelic_female_artisan|work shift; wool skirt; leather shoes; work gown; apron; headcloth; woven belt; tool pouch; bronze ring pin; sleeve ties
medieval_outfit_gaelic_male_merchant|fine long shirt; wool trews; boots; bordered tunic; lined brat; cap; purse belt; document pouch; silver ring pin; tally cord
medieval_outfit_gaelic_female_merchant|fine shift; wool gown; boots; bordered overgown; lined brat; head veil; girdle; purse; silver ring pin; bead cord
medieval_outfit_gaelic_male_noble|fine linen shirt; wool trews; boots; embroidered tunic; bardic or lordly mantle; decorated cap; fine belt; document pouch; ornate ring pin; brooch
medieval_outfit_gaelic_female_noble|fine shift; embroidered gown; soft shoes; noble overgown; bardic mantle; long veil; fine girdle; alms purse; ornate ring pin; brooch
medieval_outfit_gaelic_male_religious|plain linen shirt; wool trews; sandals; monastic robe; cowl cloak; hood; cord belt; book pouch; wooden cross; note board
medieval_outfit_gaelic_female_religious|plain shift; wool robe; sandals; religious gown; cowl cloak; veil; cord belt; book pouch; wooden cross; prayer slip
medieval_outfit_gaelic_male_military|arming shirt; wool trews; boots; light padded coat; war brat; padded cap; spear carrier belt; field pouch; ring pin; bracers
medieval_outfit_gaelic_female_military|arming shift; split skirt or trews; boots; light padded war gown; war brat; headcloth under cap; spear carrier belt; field pouch; ring pin; bracers
medieval_outfit_carolingian_male_peasant|linen shirt; braies; leg wraps; rough shoes; high-belted tunic; broad-banded mantle; wool cap; rope belt; belt pouch; simple pin
medieval_outfit_carolingian_female_peasant|linen shift; wool gown; footwraps; rough shoes; high-belted work dress; broad-banded mantle; linen head veil; woven belt; pouch; simple brooch
medieval_outfit_carolingian_male_artisan|work shirt; braies; leg wraps; leather shoes; broad-banded work tunic; short cloak; cap; tool belt; tool pouch; iron pin
medieval_outfit_carolingian_female_artisan|work shift; wool dress; footwraps; leather shoes; broad-banded work gown; apron; head veil; leather belt; tool pouch; bronze pin
medieval_outfit_carolingian_male_merchant|fine shirt; hose; leather shoes; broad-banded tunic; lined mantle; felt cap; purse belt; capitulary estate-list pouch; silver fibula; tally cord
medieval_outfit_carolingian_female_merchant|fine shift; wool gown; shoes; bordered overgown; lined mantle; veil; decorated girdle; purse; silver brooch; bead strand
medieval_outfit_carolingian_male_noble|fine undertunic; hose; boots; high-belted noble tunic; court cloak; decorated cap; spatha belt; document pouch; noble fibula; gloves
medieval_outfit_carolingian_female_noble|fine shift; embroidered gown; soft shoes; court overgown; rich mantle; long veil; decorated girdle; alms purse; noble fibula; bead strand
medieval_outfit_carolingian_male_religious|linen undertunic; wool hose; sandals; clerical dalmatic-style robe; monastic cowl; hood; cord belt; book pouch; cross pendant; manuscript leaf
medieval_outfit_carolingian_female_religious|linen shift; wool robe; sandals; monastic robe; cowl cloak; veil; cord belt; book pouch; cross pendant; prayer tablet
medieval_outfit_carolingian_male_military|arming shirt; braies; leg wraps; boots; padded war tunic; broad-banded mantle; padded cap; spatha belt; field pouch; riding spurs
medieval_outfit_carolingian_female_military|arming shift; split skirt or trews; boots; padded war gown; broad-banded mantle; head veil under cap; arming belt; field pouch; cloak pin; bracers
medieval_outfit_capetian_male_peasant|linen shirt; braies; wool hose; rough shoes; plain wool cote; rough cloak; linen coif; rope belt; pouch; clasp
medieval_outfit_capetian_female_peasant|linen shift; wool kirtle; hose; rough shoes; plain gown; rough cloak; head veil; woven belt; pouch; simple brooch
medieval_outfit_capetian_male_artisan|work shirt; braies; hose; leather shoes; guild work cote; guild apron; coif; tool belt; tool pouch; guild token
medieval_outfit_capetian_female_artisan|work shift; kirtle; hose; leather shoes; work gown; guild apron; headcloth; leather belt; tool pouch; guild token
medieval_outfit_capetian_male_merchant|fine shirt; braies; fitted hose; polished shoes; lined burgher gown; travel mantle; hood; purse belt; contract pouch; guild badge
medieval_outfit_capetian_female_merchant|fine chemise; fitted gown; fine hose; polished shoes; lined burgher overgown; mantle; wimple; decorated girdle; purse; guild badge
medieval_outfit_capetian_male_noble|fine undertunic; fitted hose; soft shoes; silk-trimmed cote; court mantle; court cap; jeweled belt; alms purse; cloak brooch; gloves
medieval_outfit_capetian_female_noble|fine chemise; bliaut-style gown; soft shoes; silk-trimmed overgown; rich mantle; wimple and veil; jeweled girdle; alms purse; brooch; gloves
medieval_outfit_capetian_male_religious|linen undertunic; hose; sandals; clerical robe; cowl cloak; coif; cord belt; book pouch; cross pendant; chapel book
medieval_outfit_capetian_female_religious|linen shift; wool robe; sandals; religious habit; cowl cloak; wimple and veil; cord belt; book pouch; cross pendant; prayer book
medieval_outfit_capetian_male_military|arming shirt; braies; chausses; boots; padded aketon; surcoat; arming coif; arming belt; field pouch; scabbard harness
medieval_outfit_capetian_female_military|arming shift; chausses or split skirt; boots; padded aketon gown; surcoat; arming coif and headcloth; arming belt; field pouch; scabbard harness
medieval_outfit_german_hre_male_peasant|linen shirt; braies; wool hose; rough shoes; fitted wool tunic; winter cloak; alpine felt cap; rope belt; pouch; clasp
medieval_outfit_german_hre_female_peasant|linen shift; wool gown; hose; rough shoes; fitted work gown; winter cloak; headcloth; woven belt; pouch; simple pin
medieval_outfit_german_hre_male_artisan|work shirt; braies; hose; leather shoes; guild apron over tunic; short cloak; alpine felt cap; tool belt; tool pouch; guild mark
medieval_outfit_german_hre_female_artisan|work shift; wool gown; hose; leather shoes; guild apron; short cloak; headcloth; tool belt; tool pouch; guild mark
medieval_outfit_german_hre_male_merchant|fine shirt; fitted hose; polished shoes; civic gown; fur-lined mantle; town hat; purse belt; account pouch; guild badge; gloves
medieval_outfit_german_hre_female_merchant|fine shift; fitted gown; polished shoes; civic overgown; fur-lined mantle; fine hood; girdle; purse; guild badge; gloves
medieval_outfit_german_hre_male_noble|fine undertunic; silk hose; soft shoes; court gown; fur-lined mantle; court hat; jeweled belt; seal pouch; belt mounts; gloves
medieval_outfit_german_hre_female_noble|fine chemise; court gown; soft shoes; embroidered overgown; fur-lined mantle; fine hood or veil; jeweled girdle; alms purse; brooch; gloves
medieval_outfit_german_hre_male_religious|linen undertunic; hose; sandals; church robe; cowl cloak; coif; cord belt; book pouch; cross pendant; manuscript leaf
medieval_outfit_german_hre_female_religious|linen shift; wool robe; sandals; religious habit; cowl cloak; veil; cord belt; book pouch; cross pendant; prayer book
medieval_outfit_german_hre_male_military|arming shirt; braies; chausses; boots; arming jack; short mantle; padded cap; arming belt; field pouch; town crossbow militia hook
medieval_outfit_german_hre_female_military|arming shift; chausses or split skirt; boots; arming jack gown; short mantle; headcloth under cap; arming belt; field pouch; bracers; town crossbow militia hook
medieval_outfit_iberian_christian_male_peasant|linen shirt; braies; wool hose; sandals; simple saya; short manto; cloth cap; rope belt; pouch; clasp
medieval_outfit_iberian_christian_female_peasant|linen shift; wool skirt; hose; sandals; simple saya gown; manto; toca head veil; woven belt; pouch; pin
medieval_outfit_iberian_christian_male_artisan|work shirt; braies; hose; leather shoes; narrow-sleeved tunic; short cloak; cap; tool belt; pouch; buckle
medieval_outfit_iberian_christian_female_artisan|work shift; wool gown; hose; leather shoes; narrow-sleeved work gown; apron; toca; leather belt; tool pouch; pin
medieval_outfit_iberian_christian_male_merchant|fine shirt; fitted hose; shoes; pellote over tunic; lined manto; cap; purse belt; contract pouch; belt mount; gloves
medieval_outfit_iberian_christian_female_merchant|fine shift; fitted gown; shoes; pellote overgown; lined manto; toca; decorated girdle; purse; brooch; gloves
medieval_outfit_iberian_christian_male_noble|fine undertunic; silk hose; boots; silk-trimmed saya; court manto; court cap; noble belt; seal pouch; cloak clasp; gloves
medieval_outfit_iberian_christian_female_noble|fine chemise; court gown; soft shoes; silk pellote; court manto; fine toca and veil; jeweled girdle; alms purse; brooch; gloves
medieval_outfit_iberian_christian_male_religious|linen undertunic; hose; sandals; clerical robe; pilgrim cloak; coif; cord belt; book pouch; cross pendant; chapel booklet
medieval_outfit_iberian_christian_female_religious|linen shift; wool robe; sandals; religious habit; pilgrim cloak; veil and toca; cord belt; book pouch; cross pendant; prayer slip
medieval_outfit_iberian_christian_male_military|arming shirt; braies; chausses; riding boots; quilted coat; knightly surcoat; arming cap; weapon belt; field pouch; cloak clasp; frontier riding cloak
medieval_outfit_iberian_christian_female_military|arming shift; split riding skirt; chausses; riding boots; quilted coat gown; knightly surcoat; head veil under cap; weapon belt; field pouch; cloak clasp; frontier riding cloak
medieval_outfit_andalusi_male_peasant|linen qamis; wool sirwal; footwraps; sandals; plain outer tunic; light burnous; simple turban; woven sash; belt pouch; cord amulet
medieval_outfit_andalusi_female_peasant|linen shift; loose sirwal or skirt; footwraps; sandals; plain long robe; light wrap cloak; veiled headcloth; woven sash; pouch; cord amulet
medieval_outfit_andalusi_male_artisan|work qamis; sirwal; leather sandals; workshop robe; short burnous; wrapped turban; leather sash; tool pouch; small amulet; sleeve ties
medieval_outfit_andalusi_female_artisan|work shift; loose trousers or skirt; sandals; work robe; apron wrap; veiled headcloth; sash; tool pouch; amulet; sleeve ties
medieval_outfit_andalusi_male_merchant|fine qamis; sirwal; soft slippers; qaba caftan; lined burnous; turban; merchant sash; contract pouch; signet ring; perfume flask
medieval_outfit_andalusi_female_merchant|fine shift; sirwal or under-robe; soft slippers; tiraz-banded robe; lined cloak; veil; decorated sash; purse; amulet pendant; perfume flask
medieval_outfit_andalusi_male_noble|silk qamis; fine sirwal; soft slippers; tiraz-banded court robe; rich burnous; fine turban; silk sash; seal pouch; signet ring; embroidered gloves
medieval_outfit_andalusi_female_noble|fine shift; silk under-robe; soft slippers; embroidered court robe; rich mantle; fine veil; silk sash; alms purse; pendant; perfume flask
medieval_outfit_andalusi_male_religious|plain qamis; sirwal; sandals; scholar robe; plain cloak; wrapped turban; cord sash; book pouch; prayer beads; writing tablet
medieval_outfit_andalusi_female_religious|plain shift; under-robe; sandals; modest robe; plain wrap; veil; cord sash; book pouch; prayer beads; devotional slip
medieval_outfit_andalusi_male_military|arming qamis; sirwal; riding boots; quilted coat; riding burnous; turban-helm liner; bowcase belt; field pouch; amulet; leather bracers
medieval_outfit_andalusi_female_military|arming shift; sirwal or split skirt; riding boots; quilted riding coat; riding burnous; veiled headwrap under cap; bowcase belt; field pouch; amulet; bracers
medieval_outfit_byzantine_male_peasant|linen under-robe; simple trousers; footwraps; sandals; wool tunic; short sagion cloak; cloth cap; plain belt; pouch; wooden cross
medieval_outfit_byzantine_female_peasant|linen shift; lower gown; footwraps; sandals; plain long tunic; wool wrap; head veil; woven belt; pouch; wooden cross
medieval_outfit_byzantine_male_artisan|work under-robe; trousers; shoes; workshop tunic; short cloak; cap; tool belt; tool pouch; small icon token; gloves
medieval_outfit_byzantine_female_artisan|work shift; undergown; shoes; work gown; apron wrap; head veil; tool belt; pouch; icon token; sleeve ties
medieval_outfit_byzantine_male_merchant|fine under-robe; trousers; soft shoes; belted skaramangion robe; lined sagion; cap; purse belt; account pouch; seal ring; gloves
medieval_outfit_byzantine_female_merchant|fine shift; undergown; soft shoes; formal gown; lined mantle; veil; decorated girdle; purse; icon pendant; gloves
medieval_outfit_byzantine_male_noble|silk under-robe; fine trousers; soft boots; silk dalmatic; court sagion; court cap; court belt; seal pouch; enamel pendant; gloves
medieval_outfit_byzantine_female_noble|silk shift; fine gown; soft shoes; embroidered silk robe; court mantle; fine veil; court girdle; alms purse; enamel pendant; icon pouch
medieval_outfit_byzantine_male_religious|plain under-robe; trousers; sandals; monastic robe; cowl cloak; hood; cord belt; book pouch; cross pendant; icon tablet
medieval_outfit_byzantine_female_religious|plain shift; wool robe; sandals; monastic robe; cowl cloak; veil; cord belt; book pouch; cross pendant; prayer tablet
medieval_outfit_byzantine_male_military|arming under-robe; trousers; boots; military padded tunic; lamellar coat cover; padded cap; military belt; field pouch; icon token; scabbard harness
medieval_outfit_byzantine_female_military|arming shift; trousers or split skirt; boots; military padded robe; lamellar coat cover; head veil under cap; military belt; field pouch; icon token; scabbard harness
medieval_outfit_abbasid_male_peasant|linen qamis; wool sirwal; footwraps; sandals; plain robe; light cloak; simple turban; woven sash; pouch; amulet cord
medieval_outfit_abbasid_female_peasant|linen shift; loose sirwal or skirt; footwraps; sandals; plain robe; wrap cloak; head veil; woven sash; pouch; amulet cord
medieval_outfit_abbasid_male_artisan|work qamis; sirwal; sandals; work caftan; short cloak; turban; tool sash; tool pouch; amulet; sleeve ties
medieval_outfit_abbasid_female_artisan|work shift; loose trousers; sandals; work robe; apron wrap; veil; sash; tool pouch; amulet; sleeve ties
medieval_outfit_abbasid_male_merchant|fine qamis; sirwal; soft slippers; qaba caftan; lined cloak; turban; merchant sash; contract pouch; signet ring; ink case
medieval_outfit_abbasid_female_merchant|fine shift; under-robe; soft slippers; fine robe; lined cloak; veil; decorated sash; purse; pendant; scent flask
medieval_outfit_abbasid_male_noble|silk qamis; fine sirwal; soft slippers; belted court robe; rich cloak; fine turban; silk sash; seal pouch; signet ring; gloves
medieval_outfit_abbasid_female_noble|silk shift; fine under-robe; soft slippers; court robe; rich mantle; fine veil; silk sash; alms purse; pendant; gloves
medieval_outfit_abbasid_male_religious|plain qamis; sirwal; sandals; scholar robe; plain cloak; turban; cord sash; book pouch; prayer beads; notebook
medieval_outfit_abbasid_female_religious|plain shift; modest robe; sandals; scholar or devotional robe; plain wrap; veil; cord sash; book pouch; prayer beads; prayer slip
medieval_outfit_abbasid_male_military|arming qamis; sirwal; riding boots; lamellar-sleeved coat; riding cloak; turban-helm liner; weapon sash; field pouch; amulet; bowcase belt
medieval_outfit_abbasid_female_military|arming shift; sirwal or split skirt; riding boots; lamellar-sleeved riding coat; riding cloak; veiled headwrap; weapon sash; field pouch; amulet; bowcase belt
medieval_outfit_fatimid_male_peasant|linen qamis; cotton lower wrap; sandals; light linen robe; shoulder cloth; simple turban; woven sash; pouch; amulet cord; headcloth
medieval_outfit_fatimid_female_peasant|linen shift; cotton wrap skirt; sandals; light robe; shoulder wrap; veiled headcloth; woven sash; pouch; amulet cord; bead string
medieval_outfit_fatimid_male_artisan|work qamis; cotton wrap; sandals; linen work robe; apron cloth; turban; tool sash; pouch; amulet; sleeve bands
medieval_outfit_fatimid_female_artisan|work shift; cotton wrap skirt; sandals; linen work gown; apron; veil; sash; tool pouch; amulet; sleeve bands
medieval_outfit_fatimid_male_merchant|fine linen qamis; light trousers; soft sandals; tiraz-banded tunic; lined robe; turban; merchant sash; tax pouch; signet ring; perfume flask
medieval_outfit_fatimid_female_merchant|fine shift; cotton under-robe; soft sandals; tiraz-banded robe; light mantle; veil; decorated sash; purse; pendant; perfume flask
medieval_outfit_fatimid_male_noble|silk qamis; fine trousers; soft slippers; court kaftan; formal cuffs; fine turban; silk sash; seal pouch; signet ring; gloves
medieval_outfit_fatimid_female_noble|silk shift; fine robe; soft slippers; court robe; light mantle; fine veil; silk sash; alms purse; pendant; scent flask
medieval_outfit_fatimid_male_religious|plain qamis; light trousers; sandals; scholar robe; plain cloak; turban; cord sash; book pouch; prayer beads; endowment slip
medieval_outfit_fatimid_female_religious|plain shift; modest robe; sandals; devotional robe; plain wrap; veil; cord sash; book pouch; prayer beads; prayer slip
medieval_outfit_fatimid_male_military|arming qamis; trousers; riding boots; padded coat with scale panels; guard cloak; turban-helm liner; weapon belt; field pouch; amulet; archer quiver
medieval_outfit_fatimid_female_military|arming shift; trousers or split skirt; riding boots; padded guard coat; guard cloak; veiled headwrap; weapon belt; field pouch; amulet; archer quiver
medieval_outfit_seljuk_ayyubid_male_peasant|linen qamis; wool sirwal; footwraps; boots; plain caftan; felt cloak; simple turban; sash; pouch; amulet
medieval_outfit_seljuk_ayyubid_female_peasant|linen shift; loose sirwal or skirt; footwraps; boots; plain long robe; felt cloak; veiled headwrap; sash; pouch; amulet
medieval_outfit_seljuk_ayyubid_male_artisan|work qamis; sirwal; boots; quilted coat; short cloak; turban; tool sash; tool pouch; amulet; gloves
medieval_outfit_seljuk_ayyubid_female_artisan|work shift; sirwal or skirt; boots; quilted work robe; apron wrap; veil; sash; tool pouch; amulet; gloves
medieval_outfit_seljuk_ayyubid_male_merchant|fine qamis; sirwal; soft boots; riding caftan; lined cloak; turban; merchant sash; contract pouch; signet ring; gloves
medieval_outfit_seljuk_ayyubid_female_merchant|fine shift; under-robe; soft boots; fine caftan robe; lined mantle; veil; decorated sash; purse; pendant; gloves
medieval_outfit_seljuk_ayyubid_male_noble|silk qamis; fine sirwal; high boots; court caftan; fur-edged cloak; fine turban; silk sash; seal pouch; belt plaques; gloves
medieval_outfit_seljuk_ayyubid_female_noble|silk shift; fine under-robe; soft boots; embroidered court robe; fur-edged mantle; fine veil; silk sash; alms purse; pendant; gloves
medieval_outfit_seljuk_ayyubid_male_religious|plain qamis; sirwal; sandals; scholar robe; plain cloak; turban; cord sash; book pouch; prayer beads; madrasa notebook
medieval_outfit_seljuk_ayyubid_female_religious|plain shift; modest robe; sandals; devotional robe; plain wrap; veil; cord sash; book pouch; prayer beads; prayer slip
medieval_outfit_seljuk_ayyubid_male_military|arming qamis; sirwal; high riding boots; quilted riding coat; lamellar coat cover; turban-helm liner; bowcase belt; field pouch; amulet; leather gloves
medieval_outfit_seljuk_ayyubid_female_military|arming shift; sirwal or split skirt; high riding boots; quilted riding robe; lamellar coat cover; veiled headwrap under cap; bowcase belt; field pouch; amulet; gloves
medieval_outfit_rus_novgorod_male_peasant|linen rubakha; porty trousers; onuchi footwraps; bast or leather shoes; wool tunic; rough cloak; fur cap; woven belt; belt pouch; simple cross cord
medieval_outfit_rus_novgorod_female_peasant|linen shift; wool skirt; onuchi footwraps; leather shoes; rubakha-style gown; rough cloak; headscarf; woven belt; pouch; simple cross cord
medieval_outfit_rus_novgorod_male_artisan|work rubakha; porty trousers; onuchi; boots; work kaftan; short cloak; fur cap; tool belt; tool pouch; bronze cross; mittens
medieval_outfit_rus_novgorod_female_artisan|work shift; wool skirt; onuchi; boots; work gown; apron; headscarf; tool belt; tool pouch; bronze cross; mittens
medieval_outfit_rus_novgorod_male_merchant|fine rubakha; porty trousers; boots; fur-edged kaftan; lined cloak; fur hat; purse belt; birchbark document pouch; silver cross; gloves
medieval_outfit_rus_novgorod_female_merchant|fine shift; wool gown; boots; fur-edged overgown; lined cloak; head veil; decorated girdle; purse; silver cross; document pouch
medieval_outfit_rus_novgorod_male_noble|fine shirt; trousers; soft boots; embroidered kaftan; fur-lined mantle; fur hat; rich belt; seal pouch; Orthodox pendant; gloves
medieval_outfit_rus_novgorod_female_noble|fine shift; embroidered gown; soft boots; fur-edged court robe; fur-lined mantle; fine veil; rich girdle; alms purse; Orthodox pendant; gloves
medieval_outfit_rus_novgorod_male_religious|plain rubakha; trousers; sandals; monastic robe; heavy cloak; hood; cord belt; book pouch; wooden cross; prayer slip
medieval_outfit_rus_novgorod_female_religious|plain shift; wool robe; sandals; monastic robe; heavy cloak; veil; cord belt; book pouch; wooden cross; prayer slip
medieval_outfit_rus_novgorod_male_military|arming rubakha; trousers; boots; padded war coat; fur-edged cloak; padded cap or helm liner; warrior belt; field pouch; bronze cross; axe loop
medieval_outfit_rus_novgorod_female_military|arming shift; trousers or split skirt; boots; padded war coat; fur-edged cloak; headscarf under cap; warrior belt; field pouch; bronze cross; axe loop
medieval_outfit_steppe_turkic_male_peasant|linen under-shirt; riding trousers; felt footwraps; high boots; tied riding coat; felt cloak; fur cap; sash; travel pouch; amulet
medieval_outfit_steppe_turkic_female_peasant|linen shift; riding trousers or split skirt; felt footwraps; high boots; tied long riding coat; felt cloak; fur cap or headwrap; sash; travel pouch; amulet
medieval_outfit_steppe_turkic_male_artisan|work shirt; trousers; boots; work caftan; short felt cloak; fur cap; tool sash; tool pouch; amulet; gloves
medieval_outfit_steppe_turkic_female_artisan|work shift; trousers or skirt; boots; work coat; apron wrap; fur cap/headwrap; tool sash; tool pouch; amulet; gloves
medieval_outfit_steppe_turkic_male_merchant|fine shirt; riding trousers; high boots; felt riding caftan; lined cloak; fur cap; merchant sash; trade pouch; seal tag; gloves
medieval_outfit_steppe_turkic_female_merchant|fine shift; riding trousers or skirt; high boots; long riding caftan; lined cloak; headwrap; decorated sash; purse; seal tag; gloves
medieval_outfit_steppe_turkic_male_noble|silk under-shirt; fine trousers; high boots; embroidered riding caftan; fur-lined cloak; ornate fur cap; silk sash; seal pouch; belt plaques; gloves
medieval_outfit_steppe_turkic_female_noble|silk shift; fine trousers or skirt; high boots; embroidered long caftan; fur-lined cloak; ornate headwrap; silk sash; alms purse; belt plaques; gloves
medieval_outfit_steppe_turkic_male_religious|plain shirt; trousers; boots; sober caftan; felt cloak; fur cap; cord sash; pouch; prayer beads or amulet; herd tally
medieval_outfit_steppe_turkic_female_religious|plain shift; trousers or skirt; boots; sober long coat; felt cloak; headwrap; cord sash; pouch; prayer beads or amulet; herd tally
medieval_outfit_steppe_turkic_male_military|arming shirt; riding trousers; high boots; lamellar coat cover; felt war cloak; fur cap helm liner; bowcase-and-quiver belt; field pouch; amulet; horseman gloves
medieval_outfit_steppe_turkic_female_military|arming shift; riding trousers; high boots; lamellar riding coat cover; felt war cloak; fur cap/headwrap; bowcase-and-quiver belt; field pouch; amulet; horseman gloves
medieval_outfit_song_china_male_peasant|plain under-robe; narrow trousers; cloth socks; cloth shoes; short working jacket; rain cape; cloth headwrap; simple sash; belt pouch; wooden tally
medieval_outfit_song_china_female_peasant|plain shift; skirt or trousers; cloth socks; cloth shoes; cross-collar work robe; rain cape; headcloth; simple sash; pouch; hair pin
medieval_outfit_song_china_male_artisan|work under-robe; narrow trousers; cloth shoes; work jacket; apron; cloth cap; tool sash; sleeve pouch; workshop tally; gloves
medieval_outfit_song_china_female_artisan|work shift; skirt or trousers; cloth shoes; work robe; apron; headcloth; tool sash; sleeve pouch; hair pin; gloves
medieval_outfit_song_china_male_merchant|fine under-robe; trousers; cloth shoes; merchant robe; lined outer robe; scholar-style cap; silk sash; account sleeve pouch; seal cord; gloves
medieval_outfit_song_china_female_merchant|fine shift; skirt; cloth shoes; cross-collar merchant robe; lined outer robe; headcloth or cap; silk sash; purse; hair ornament; account sleeve pouch
medieval_outfit_song_china_male_noble|silk under-robe; fine trousers; soft shoes; scholar robe; padded winter robe; official cap; silk sash; document sleeve pouch; official badge; gloves
medieval_outfit_song_china_female_noble|silk shift; fine skirt; soft shoes; elegant cross-collar robe; padded winter robe; formal headwear; silk sash; alms purse; hair ornament; pendant
medieval_outfit_song_china_male_religious|plain under-robe; trousers; sandals or cloth shoes; scholar-monastic robe; plain cloak; simple cap; cord sash; book pouch; prayer beads; notebook
medieval_outfit_song_china_female_religious|plain shift; skirt or trousers; cloth shoes; religious robe; plain cloak; head veil or cloth; cord sash; book pouch; prayer beads; prayer slip
medieval_outfit_song_china_male_military|arming under-robe; trousers; boots; padded military vest; lamellar cover robe; military cap; weapon sash; field pouch; guard token; bracers
medieval_outfit_song_china_female_military|arming shift; trousers or split skirt; boots; padded military vest; lamellar cover robe; headcloth under cap; weapon sash; field pouch; guard token; bracers
";

	private static readonly EraOutfitPieceSpec[] MedievalExplicitOutfitPieces =
		BuildMedievalExplicitOutfitPieces();

	private static readonly EraOutfitSpec[] MedievalOutfits = BuildMedievalOutfits();


	private static readonly IReadOnlyDictionary<string, MedievalBespokeOutfitPieceSpec>
		MedievalBespokeOutfitPieces =
			BuildMedievalBespokeOutfitPieces()
				.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);

	private static readonly IReadOnlyDictionary<string, MedievalBespokeOutfitPieceCraftSpec>
		MedievalBespokeOutfitPieceCraftSpecs =
			MedievalBespokeOutfitPieces.Values
				.Select(x => new MedievalBespokeOutfitPieceCraftSpec(
					x.StableReference,
					x.CraftInputs,
					x.CraftTools))
				.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);

	private static IReadOnlyList<EraItemSpec> HistoricFoundationItemSpecs()
	{
		const string ToolTag = "Market / Professional Tools / Standard Tools";
		return
		[
			new("historic_workshop_hearth", "hearth", "an unlit workshop hearth",
				"This low clay hearth has a charcoal bed and a battered lip for pots, wax vessels, glue, pitch, and small workshop heating jobs.",
				SizeCategory.Large, ItemQuality.Standard, 28000.0, 42.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Material Functions / Fire"], ["Holdable", "Destroyable_Furniture"],
				"Cross-era workshop heat source for antiquity and medieval installs."),
			new("historic_lit_workshop_hearth", "hearth", "a lit workshop hearth",
				"This low clay hearth burns with a bed of glowing charcoal, giving steady heat for ordinary workshop preparation.",
				SizeCategory.Large, ItemQuality.Standard, 28200.0, 46.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Material Functions / Fire", "Functions / Material Functions / Hot Fire"], ["Holdable", "Destroyable_Furniture"],
				"Lit state for the shared historic hearth.",
				"historic_workshop_hearth", "$0 burns low and settles back into $1.", TimeSpan.FromHours(2)),
			new("historic_updraft_kiln", "kiln", "an unlit updraft kiln",
				"This squat updraft kiln has a low firebox, pierced setting floor, and enough chamber space for lamps, small vessels, tiles, and sealed wares.",
				SizeCategory.Huge, ItemQuality.Standard, 65000.0, 120.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Pottery Tools / Kiln"], ["Holdable", "Destroyable_Furniture"],
				"Cross-era pottery kiln shared by antiquity and medieval installs."),
			new("historic_lit_updraft_kiln", "kiln", "a lit updraft kiln",
				"This updraft kiln is stoked with charcoal heat, drawing fire through the chamber for firing vessels, lamps, tiles, and small wares.",
				SizeCategory.Huge, ItemQuality.Standard, 65200.0, 128.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Pottery Tools / Kiln", "Functions / Tools / Pottery Tools / Kiln / Lit Kiln", "Functions / Material Functions / Hot Fire"], ["Holdable", "Destroyable_Furniture"],
				"Lit state for the shared historic kiln.",
				"historic_updraft_kiln", "$0 cools and dies back into $1.", TimeSpan.FromHours(4)),
			new("historic_warp_weighted_loom", "loom", "a warp-weighted loom",
				"This timber loom leans against a frame with rows of hanging loom weights, ready for broad cloth, bands, and household textiles.",
				SizeCategory.Large, ItemQuality.Standard, 24000.0, 75.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Loom", "Functions / Tools / Textilecraft Tools / Weaving Tools / Warp-Weighted Loom"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_treadle_loom", "loom", "a timber treadle loom",
				"This treadle loom has a heavier timber frame, working pedals, and a beam wide enough for bolts of cloth.",
				SizeCategory.Huge, ItemQuality.Standard, 36000.0, 120.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Loom", "Functions / Tools / Textilecraft Tools / Weaving Tools / Hand Loom"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_drop_spindle", "spindle", "a weighted drop spindle",
				"This small weighted spindle draws prepared fibre into yarn for ordinary household, workshop, and market textile production.",
				SizeCategory.Tiny, ItemQuality.Standard, 120.0, 4.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Spinning Tools / Drop Spindle"], ["Holdable", "Destroyable_Misc"]),
			new("historic_sewing_needle", "needle", "an iron sewing needle",
				"This slender iron needle has a sharp point and narrow eye, suitable for clothing, light leather, book cloth, and household sewing.",
				SizeCategory.VerySmall, ItemQuality.Standard, 18.0, 2.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Sewing Needle", "Functions / Joining / Sewing"], ["Holdable", "Destroyable_Misc"]),
			new("historic_textile_shears", "shears", "a pair of iron textile shears",
				"These springy iron shears have broad blades and worn grips, useful for cloth, thread, felt, and light leather cutting.",
				SizeCategory.Small, ItemQuality.Standard, 780.0, 18.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Separation / Shearing / Shears"], ["Holdable", "Destroyable_Misc"]),
			new("historic_awl_punch", "awl", "an iron awl punch",
				"This iron awl punch has a tapered point and simple wooden grip for opening stitch holes in leather, parchment, and heavy cloth.",
				SizeCategory.Small, ItemQuality.Standard, 160.0, 9.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Leatherworking Tools / Awl Punch"], ["Holdable", "Destroyable_Misc"]),
			new("historic_dye_vat", "vat", "a timber dye vat",
				"This timber dye vat is darkly stained around the rim and broad enough to soak skeins, narrow cloth, or small garments.",
				SizeCategory.Large, ItemQuality.Standard, 32000.0, 60.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Textilecraft Tools / Dyeing Tools / Dye Vat"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_tanning_rack", "rack", "a hide tanning rack",
				"This timber rack is pegged and corded for stretching hides while they dry, smoke, oil, or take tannin.",
				SizeCategory.Large, ItemQuality.Standard, 18000.0, 42.0m, "oak", MaterialBehaviourType.Wood,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Tanning Tools / Tanning Rack"], ["Holdable", "Destroyable_Furniture"]),
			new("historic_hand_quern", "quern", "a rotary hand quern",
				"This two-stone rotary quern has a worn upper stone, a side handle, and a central feed hole for grinding grain or dry dye materials.",
				SizeCategory.Normal, ItemQuality.Standard, 7800.0, 24.0m, "stone", MaterialBehaviourType.Stone,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Foodmaking Tools / Hand Quern", "Functions / Tools / Milling Tools / Rotary Quern"], ["Holdable", "Destroyable_Misc"]),
			new("historic_oil_lamp", "lamp", "an unlit clay oil lamp",
				"This small clay oil lamp has a shallow reservoir, pinched wick spout, and soot-darkened lip from earlier use.",
				SizeCategory.Tiny, ItemQuality.Standard, 180.0, 5.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, "Market / Lighting", "Functions / Household Items / Household Lighting"], ["Holdable", "Destroyable_Misc"]),
			new("historic_lit_oil_lamp", "lamp", "a lit clay oil lamp",
				"This clay oil lamp burns with a small steady flame at the wick, throwing enough light for close work or table use.",
				SizeCategory.Tiny, ItemQuality.Standard, 190.0, 6.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[HistoricRootTagPath, "Market / Lighting", "Functions / Household Items / Household Lighting", "Functions / Material Functions / Fire"], ["Holdable", "Lantern", "Destroyable_Misc"],
				"Lit state for the shared historic lamp.",
				"historic_oil_lamp", "$0 gutters and goes out, leaving $1.", TimeSpan.FromHours(3)),
			new("historic_workshop_anvil", "anvil", "a low workshop anvil",
				"This low iron anvil has a broad working face, battered edges, and a fitted timber block for small fittings, rings, blades, and sheet work.",
				SizeCategory.Large, ItemQuality.Standard, 34000.0, 70.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Metalworking Tools / Anvil"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("historic_forge_tongs", "tongs", "a pair of forge tongs",
				"These long iron tongs have squared jaws and wrapped grips for shifting heated blanks between a hearth, furnace, and anvil.",
				SizeCategory.Normal, ItemQuality.Standard, 1280.0, 24.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Metalworking Tools / Forge Tongs"], ["Holdable", "Destroyable_Misc"]),
			new("historic_workshop_hammer", "hammer", "an iron workshop hammer",
				"This compact iron hammer has a square striking face, peen, and smooth haft for riveting, shaping, and general workshop blows.",
				SizeCategory.Small, ItemQuality.Standard, 980.0, 20.0m, "wrought iron", MaterialBehaviourType.Metal,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Striking Tools / Hammer", "Functions / Tools / Metalworking Tools / Forge Hammer"], ["Holdable", "Melee_Improvised Bludgeon", "Destroyable_Weapon"]),
			new("historic_bellows", "bellows", "a leather workshop bellows",
				"These leather bellows are fixed between timber boards with a narrow nozzle, ready to drive air into hearths and furnaces.",
				SizeCategory.Normal, ItemQuality.Standard, 1800.0, 24.0m, "leather", MaterialBehaviourType.Leather,
				[HistoricRootTagPath, ToolTag, "Functions / Tools / Metalworking Tools / Bellows"], ["Holdable", "Destroyable_Misc"])
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalClothingItemSpecs()
	{
		// Generic baseline wardrobe only; explicit culture targets live in MedievalExplicitCultureCatalogues.
		var specs = new List<EraItemSpec>();
		foreach (var culture in MedievalCultureProfiles)
		{
			foreach (var status in MedievalStatusRoleProfiles)
			{
				specs.Add(new EraItemSpec(
					$"medieval_clothing_{culture.Key}_{status.Key}_{status.GarmentToken}",
					status.Noun,
					$"{status.ShortDescription} {culture.ClothingCue}",
					$"{status.FullDescription} It is finished {culture.ClothingCue}, with the cut, trim, and fastening details worked into the visible construction.",
					SizeCategory.Normal,
					status.Quality,
					status.WeightInGrams,
					status.Cost,
					status.Material,
					status.MaterialType,
					[
						MedievalRootTagPath,
						$"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}",
						$"{MedievalStatusTagRoot} / {status.TagName}",
						status.MarketTag,
						"Functions / Worn Items / Bodywear"
					],
					["Holdable", status.WearComponent, "Destroyable_Clothing", "Insulation_Moderate", status.ArmourComponent],
					$"Medieval generic baseline wardrobe entry. Culture slice: {culture.Display}. Status/role axis: {status.Display}. Wardrobe slot: bodywear. Control access with knowledge gates and tags."));

				foreach (var slotKey in MedievalWardrobeSlotKeys)
				{
					var piece = BuildMedievalWardrobePiece(culture, status, slotKey);
					specs.Add(new EraItemSpec(
						$"medieval_clothing_{culture.Key}_{status.Key}_{piece.Token}",
						piece.Noun,
						$"{piece.ShortDescription} {culture.ClothingCue}",
						$"{piece.FullDescription} It is finished {culture.ClothingCue}, with the regional line carried through its seams, hems, and fastenings.",
						piece.Size,
						piece.Quality,
						piece.WeightInGrams,
						piece.Cost,
						piece.Material,
						piece.MaterialType,
						[
							MedievalRootTagPath,
							$"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}",
							$"{MedievalStatusTagRoot} / {status.TagName}",
							piece.MarketTag,
							piece.FunctionTag
						],
						[
							.. new[]
							{
								"Holdable",
								piece.WearComponent,
								"Destroyable_Clothing",
								piece.InsulationComponent,
								piece.ArmourComponent
							},
							.. piece.ExtraComponents
						],
						$"Medieval generic baseline wardrobe entry. Culture slice: {culture.Display}. Status/role axis: {status.Display}. Wardrobe slot: {piece.SlotKey}. Control access with knowledge gates and tags."));
				}
			}
		}

		specs.AddRange(MedievalExplicitOutfitPieceItemSpecs());

		return specs;
	}

	private static MedievalWardrobePiece BuildMedievalWardrobePiece(MedievalCultureProfile culture,
		MedievalStatusRoleProfile status, string slotKey)
	{
		return slotKey switch
		{
			"underlayer" => status.Key switch
			{
				"peasant" => FabricPiece(slotKey, "linen_shirt", "shirt", "a plain linen shirt",
					"This plain linen shirt sits close enough to work under a tunic, with simple seams and sleeves meant for daily laundering.",
					"linen", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Underwear",
					"Wear_Long-Sleeved_Tunic", "Insulation_Minor", 420.0, 9.0m),
				"artisan" => FabricPiece(slotKey, "work_shirt", "shirt", "a sturdy work shirt",
					"This sturdy shirt has reinforced underarms and plain cuffs for long days in a workshop or yard.",
					"linen", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Underwear",
					"Wear_Long-Sleeved_Tunic", "Insulation_Minor", 480.0, 12.0m),
				"merchant" => FabricPiece(slotKey, "fine_linen_shirt", "shirt", "a fine linen shirt",
					"This fine linen shirt is neatly hemmed and cut to sit cleanly beneath better gowns or travel clothing.",
					"linen", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Underwear",
					"Wear_Long-Sleeved_Tunic", "Insulation_Minor", 380.0, 24.0m),
				"noble" => FabricPiece(slotKey, "silk_lined_chemise", "chemise", "a silk-lined chemise",
					"This light chemise has fine seams, smooth lining, and a soft fall meant for court dress or private chambers.",
					"silk", status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Underwear",
					"Wear_Long-Sleeved_Tunic", "Insulation_Minor", 320.0, 55.0m),
				"clergy" => FabricPiece(slotKey, "linen_undertunic", "undertunic", "a plain linen undertunic",
					"This sober undertunic is cut for modesty and washing, with little decoration beyond careful seams.",
					"linen", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Underwear",
					"Wear_Long-Sleeved_Tunic", "Insulation_Minor", 520.0, 14.0m),
				_ => FabricPiece(slotKey, "arming_shirt", "shirt", "a padded arming shirt",
					"This arming shirt is quilted lightly at the shoulders and ties, giving a washable layer beneath heavier equipment.",
					"linen", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Underwear",
					"Wear_Long-Sleeved_Tunic", "Insulation_Moderate", 820.0, 28.0m, armourComponent: "Armour_HeavyClothing")
			},
			"headwear" => status.Key switch
			{
				"peasant" => FabricPiece(slotKey, "wool_cap", "cap", "a simple wool cap",
					"This simple cap is close-fitted, warm, and plain enough for field, hearth, or road.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Moderate", 130.0, 7.0m),
				"artisan" => FabricPiece(slotKey, "work_cap", "cap", "a fitted work cap",
					"This fitted work cap keeps hair and sweat out of the way, with a folded edge and stout stitching.",
					"linen", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Minor", 110.0, 8.0m),
				"merchant" => FabricPiece(slotKey, "lined_hat", "hat", "a lined town hat",
					"This lined hat has a neat brim and a respectable finish suitable for shop, guildhall, or road.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Moderate", 180.0, 20.0m),
				"noble" => FabricPiece(slotKey, "court_cap", "cap", "a fine court cap",
					"This fine cap is carefully shaped and trimmed, meant to complete a formal court outfit.",
					"silk", status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Minor", 120.0, 50.0m),
				"clergy" => FabricPiece(slotKey, "plain_coif", "coif", "a plain cloth coif",
					"This plain coif ties close under the chin and keeps a modest, orderly line around the head.",
					"linen", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Minor", 100.0, 8.0m),
				_ => FabricPiece(slotKey, "padded_arming_cap", "cap", "a padded arming cap",
					"This padded cap is quilted to sit beneath a helm or mail coif, with tie points at the chin.",
					"linen", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Moderate", 360.0, 18.0m, armourComponent: "Armour_HeavyClothing")
			},
			"hood" => status.Key switch
			{
				"peasant" => FabricPiece(slotKey, "rough_hood", "hood", "a rough wool hood",
					"This rough hood covers the head and shoulders, giving cheap warmth in wind and rain.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Balanced_Warm", 360.0, 12.0m),
				"artisan" => FabricPiece(slotKey, "work_hood", "hood", "a sturdy work hood",
					"This sturdy hood has a short shoulder fall and simple ties that stay out of tools and flame.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Balanced_Warm", 340.0, 16.0m),
				"merchant" => FabricPiece(slotKey, "lined_hood", "hood", "a lined travel hood",
					"This lined hood is cut for road wear, with a clean face opening and enough cloth to shed weather.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Balanced_Warm", 420.0, 28.0m),
				"noble" => FabricPiece(slotKey, "fur_lined_hood", "hood", "a fur-lined hood",
					"This soft hood has a rich lining and careful finishing around the face, throat, and shoulder edge.",
					"silk", status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Strong", 520.0, 95.0m),
				"clergy" => FabricPiece(slotKey, "monastic_cowl", "cowl", "a plain monastic cowl",
					"This deep cowl falls from head to shoulders with deliberate plainness and a sober drape.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Balanced_Warm", 540.0, 24.0m),
				_ => FabricPiece(slotKey, "arming_hood", "hood", "a quilted arming hood",
					"This quilted hood protects the scalp, neck, and cheeks from chafing mail and helmet edges.",
					"linen", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Headwear",
					"Wear_Hat", "Insulation_Moderate", 620.0, 30.0m, armourComponent: "Armour_HeavyClothing")
			},
			"outerwear" => status.Key switch
			{
				"peasant" => FabricPiece(slotKey, "rough_cloak", "cloak", "a rough wool cloak",
					"This rough cloak is broad, warm, and plainly hemmed, useful as blanket, rain cover, or work wrap.",
					"wool", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Outerwear",
					"Wear_Cloak_(Closed)", "Insulation_Strong", 1600.0, 26.0m, armourComponent: "Armour_HeavyClothing"),
				"artisan" => FabricPiece(slotKey, "hooded_work_cloak", "cloak", "a hooded work cloak",
					"This hooded cloak is cut short enough for movement and long enough to protect tools and shoulders from weather.",
					"wool", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Outerwear",
					"Wear_Cloak_(Closed)", "Insulation_Strong", 1500.0, 34.0m, armourComponent: "Armour_HeavyClothing"),
				"merchant" => FabricPiece(slotKey, "travel_mantle", "mantle", "a lined travel mantle",
					"This lined mantle has good cloth, a practical clasp point, and a respectable fall for town and road.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Outerwear",
					"Wear_Mantle", "Insulation_Balanced_Warm", 1300.0, 70.0m, armourComponent: "Armour_HeavyClothing"),
				"noble" => FabricPiece(slotKey, "fur_lined_mantle", "mantle", "a fur-lined court mantle",
					"This mantle is rich, warm, and deliberately showy, with a generous fall and careful edging.",
					"silk", status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Outerwear",
					"Wear_Mantle", "Insulation_Strong", 1200.0, 160.0m, armourComponent: "Armour_HeavyClothing"),
				"clergy" => FabricPiece(slotKey, "wool_cowl_cloak", "cloak", "a heavy wool cowl-cloak",
					"This heavy cowl-cloak gives plain warmth for cloister, road, choir, or cold stone rooms.",
					"wool", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Outerwear",
					"Wear_Cloak_(Closed)", "Insulation_Strong", 1700.0, 38.0m, armourComponent: "Armour_HeavyClothing"),
				_ => FabricPiece(slotKey, "weather_cloak", "cloak", "a military weather cloak",
					"This weather cloak is thick enough for marching and watch duty, with a short practical cut over equipment.",
					"wool", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Outerwear",
					"Wear_Cloak_(Closed)", "Insulation_Strong", 1400.0, 44.0m, armourComponent: "Armour_HeavyClothing")
			},
			"legwear" => status.Key switch
			{
				"peasant" => FabricPiece(slotKey, "wool_leggings", "leggings", "a pair of wool leggings",
					"These wool leggings are plain, close, and warm enough for fieldwork, walking, and winter chores.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Legwear",
					"Wear_Chausses", "Insulation_Moderate", 420.0, 10.0m),
				"artisan" => FabricPiece(slotKey, "work_hose", "hose", "a pair of sturdy work hose",
					"These work hose are close-cut and reinforced around the knees for bench, yard, or scaffold labour.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Legwear",
					"Wear_Chausses", "Insulation_Moderate", 480.0, 16.0m),
				"merchant" => FabricPiece(slotKey, "fitted_hose", "hose", "a pair of fitted hose",
					"These fitted hose are neatly cut and better finished than common legwear, suitable for town wear.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Legwear",
					"Wear_Chausses", "Insulation_Moderate", 420.0, 28.0m),
				"noble" => FabricPiece(slotKey, "fine_hose", "hose", "a pair of fine court hose",
					"These fine hose use good cloth and careful shaping for a smooth formal line.",
					"silk", status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Legwear",
					"Wear_Chausses", "Insulation_Minor", 340.0, 70.0m),
				"clergy" => FabricPiece(slotKey, "plain_chausses", "chausses", "a pair of plain chausses",
					"These plain chausses are sober, serviceable, and cut to sit beneath a robe without display.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Legwear",
					"Wear_Chausses", "Insulation_Moderate", 460.0, 14.0m),
				_ => FabricPiece(slotKey, "padded_chausses", "chausses", "a pair of padded arming chausses",
					"These padded chausses cushion the legs beneath mail, straps, or riding wear.",
					"linen", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Legwear",
					"Wear_Chausses", "Insulation_Moderate", 760.0, 34.0m, armourComponent: "Armour_HeavyClothing")
			},
			"handwear" => status.Key switch
			{
				"peasant" => FabricPiece(slotKey, "wool_mittens", "mittens", "a pair of wool mittens",
					"These wool mittens are plain, warm, and loosely shaped for cold morning work.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Gloves",
					"Wear_Mittens", "Insulation_Moderate", 160.0, 7.0m),
				"artisan" => LeatherPiece(slotKey, "leather_work_gloves", "gloves", "a pair of leather work gloves",
					"These leather gloves are tough around the palm and open enough for practical workshop handling.",
					status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Gloves",
					"Wear_Fingerless_Gloves", "Insulation_Minor", 260.0, 18.0m),
				"merchant" => LeatherPiece(slotKey, "lined_gloves", "gloves", "a pair of lined leather gloves",
					"These lined gloves are tidy and comfortable, suited to road, counting room, or public business.",
					status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Gloves",
					"Wear_Fingerless_Gloves", "Insulation_Moderate", 220.0, 32.0m),
				"noble" => FabricPiece(slotKey, "fine_gloves", "gloves", "a pair of fine court gloves",
					"These fine gloves are soft, close-fitting, and finished for display rather than hard work.",
					"silk", status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Gloves",
					"Wear_Fingerless_Gloves", "Insulation_Minor", 120.0, 60.0m),
				"clergy" => FabricPiece(slotKey, "plain_mittens", "mittens", "a pair of plain wool mittens",
					"These plain mittens are warm, modest, and serviceable for cold cloisters and travel.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Gloves",
					"Wear_Mittens", "Insulation_Moderate", 160.0, 8.0m),
				_ => LeatherPiece(slotKey, "arming_gloves", "gloves", "a pair of arming gloves",
					"These arming gloves are reinforced for weapon grips, straps, and marching wear.",
					status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Gloves",
					"Wear_Fingerless_Gloves", "Insulation_Moderate", 360.0, 34.0m, armourComponent: "Armour_HeavyClothing")
			},
			"sockwear" => status.Key switch
			{
				"peasant" => FabricPiece(slotKey, "wool_footwraps", "footwraps", "a pair of wool footwraps",
					"These wool footwraps are simple rectangles meant to pad shoes or boots through a long day.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Socks",
					"Wear_Socks", "Insulation_Moderate", 160.0, 5.0m),
				"artisan" => FabricPiece(slotKey, "wool_socks", "socks", "a pair of wool work socks",
					"These wool socks are thick at the heel and toe, made for workshop floors and city streets.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Socks",
					"Wear_Socks", "Insulation_Moderate", 180.0, 8.0m),
				"merchant" => FabricPiece(slotKey, "fine_socks", "socks", "a pair of fine wool socks",
					"These fine socks are warm, neat, and shaped cleanly for better shoes or travel boots.",
					"wool", status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Socks",
					"Wear_Socks", "Insulation_Moderate", 140.0, 18.0m),
				"noble" => FabricPiece(slotKey, "silk_socks", "socks", "a pair of silk socks",
					"These silk socks are soft, light, and made for comfort inside fine shoes.",
					"silk", status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Socks",
					"Wear_Socks", "Insulation_Minor", 90.0, 40.0m),
				"clergy" => FabricPiece(slotKey, "plain_socks", "socks", "a pair of plain wool socks",
					"These plain socks are unshowy, warm, and practical beneath sandals or shoes.",
					"wool", status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Socks",
					"Wear_Socks", "Insulation_Moderate", 150.0, 7.0m),
				_ => FabricPiece(slotKey, "boot_socks", "socks", "a pair of thick boot socks",
					"These thick boot socks are padded for marching, riding, and long watch duty.",
					"wool", status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Socks",
					"Wear_Socks", "Insulation_Balanced_Warm", 240.0, 14.0m)
			},
			"footwear" => status.Key switch
			{
				"peasant" => LeatherPiece(slotKey, "rough_turnshoes", "shoes", "a pair of rough leather turnshoes",
					"These rough turnshoes are simply cut and repaired at the sole, good enough for field paths and village floors.",
					status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Footwear",
					"Wear_Shoes", "Insulation_Minor", 780.0, 16.0m),
				"artisan" => LeatherPiece(slotKey, "work_boots", "boots", "a pair of leather work boots",
					"These work boots are scuffed, sturdy, and high enough to protect the ankle around tools and mud.",
					status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Footwear",
					"Wear_Boots", "Insulation_Moderate", 1300.0, 28.0m),
				"merchant" => LeatherPiece(slotKey, "town_shoes", "shoes", "a pair of polished town shoes",
					"These town shoes are neatly stitched and polished, suitable for shop, hall, or road.",
					status, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Footwear",
					"Wear_Shoes", "Insulation_Minor", 900.0, 36.0m),
				"noble" => LeatherPiece(slotKey, "fine_court_shoes", "shoes", "a pair of fine court shoes",
					"These fine shoes are soft, elegant, and carefully shaped for formal indoor wear.",
					status, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Footwear",
					"Wear_Shoes", "Insulation_Minor", 760.0, 80.0m),
				"clergy" => LeatherPiece(slotKey, "plain_sandals", "sandals", "a pair of plain leather sandals",
					"These plain sandals are sturdy, modest, and easy to repair after travel or cloister use.",
					status, "Market / Clothing / Simple Clothing", "Functions / Worn Items / Footwear",
					"Wear_Sandals", "Insulation_Minor", 620.0, 14.0m),
				_ => LeatherPiece(slotKey, "riding_boots", "boots", "a pair of leather riding boots",
					"These riding boots are tall, reinforced, and shaped for stirrups, marching, and field duty.",
					status, "Market / Clothing / Winter Clothing", "Functions / Worn Items / Footwear",
					"Wear_Boots", "Insulation_Moderate", 1600.0, 48.0m, armourComponent: "Armour_BoiledLeather")
			},
			"belt" => status.Key switch
			{
				"peasant" => BeltPiece(slotKey, "rope_belt", "belt", "a knotted rope belt",
					"This rope belt is rough, cheap, and easy to knot around a tunic or cloak.", "hemp", MaterialBehaviourType.Fabric,
					status, "Market / Clothing / Simple Clothing", 220.0, 5.0m, "Belt_2"),
				"artisan" => BeltPiece(slotKey, "tool_belt", "belt", "a leather tool belt",
					"This leather belt has sturdy stitching and enough hanging points for pouches, knives, or small tools.", "leather", MaterialBehaviourType.Leather,
					status, "Market / Clothing / Standard Clothing", 420.0, 22.0m, "Belt_4"),
				"merchant" => BeltPiece(slotKey, "purse_belt", "belt", "a merchant's purse belt",
					"This respectable belt has metal fittings and reinforced points for a purse, keys, or small account pouch.", "leather", MaterialBehaviourType.Leather,
					status, "Market / Clothing / Standard Clothing", 360.0, 34.0m, "Belt_4"),
				"noble" => BeltPiece(slotKey, "fine_girdle", "girdle", "a fine court girdle",
					"This fine girdle is narrow, carefully finished, and suited to formal robes or court surcoats.", "silk", MaterialBehaviourType.Fabric,
					status, "Market / Clothing / Luxury Clothing", 180.0, 90.0m, "Belt_6"),
				"clergy" => BeltPiece(slotKey, "cord_belt", "belt", "a plain cord belt",
					"This plain cord belt is deliberately modest, tying a robe without display.", "hemp", MaterialBehaviourType.Fabric,
					status, "Market / Clothing / Simple Clothing", 160.0, 4.0m, "Belt_2"),
				_ => BeltPiece(slotKey, "arming_belt", "belt", "a broad arming belt",
					"This broad belt is reinforced for scabbard, pouch, and field gear weight.", "leather", MaterialBehaviourType.Leather,
					status, "Market / Clothing / Standard Clothing", 620.0, 42.0m, "Belt_6", armourComponent: "Armour_BoiledLeather")
			},
			"pouch" => status.Key switch
			{
				"peasant" => PouchPiece(slotKey, "belt_pouch", "pouch", "a small belt pouch",
					"This small pouch ties to a belt and holds coins, seed, food scraps, or household odds and ends.",
					"linen", MaterialBehaviourType.Fabric, status, "Market / Clothing / Simple Clothing", 120.0, 8.0m),
				"artisan" => PouchPiece(slotKey, "tool_pouch", "pouch", "a leather tool pouch",
					"This leather pouch has divided folds for chalk, awls, needles, files, or other small workshop tools.",
					"leather", MaterialBehaviourType.Leather, status, "Market / Clothing / Standard Clothing", 240.0, 20.0m),
				"merchant" => PouchPiece(slotKey, "belt_purse", "purse", "a merchant's belt purse",
					"This purse is lined and closable, with reinforced loops for a belt and enough room for coin or tallies.",
					"leather", MaterialBehaviourType.Leather, status, "Market / Clothing / Standard Clothing", 220.0, 32.0m, containerComponent: "Container_Purse"),
				"noble" => PouchPiece(slotKey, "alms_purse", "purse", "a fine alms purse",
					"This fine purse is soft, decorative, and made to hang visibly from a formal girdle.",
					"silk", MaterialBehaviourType.Fabric, status, "Market / Clothing / Luxury Clothing", 140.0, 70.0m, containerComponent: "Container_Purse"),
				"clergy" => PouchPiece(slotKey, "book_pouch", "pouch", "a plain book pouch",
					"This plain pouch is sized for a small book, prayer slip, seal cord, or writing scraps.",
					"linen", MaterialBehaviourType.Fabric, status, "Market / Clothing / Standard Clothing", 180.0, 18.0m),
				_ => PouchPiece(slotKey, "field_pouch", "pouch", "a military field pouch",
					"This field pouch is sturdy enough for cord, whetstone, wax, food, or small campaign tools.",
					"leather", MaterialBehaviourType.Leather, status, "Market / Clothing / Standard Clothing", 300.0, 26.0m)
			},
			_ => throw new ApplicationException($"Unknown medieval wardrobe slot {slotKey}.")
		};
	}

	private static MedievalWardrobePiece FabricPiece(string slotKey, string token, string noun, string shortDescription,
		string fullDescription, string material, MedievalStatusRoleProfile status, string marketTag, string functionTag,
		string wearComponent, string insulationComponent, double weightInGrams, decimal cost,
		string armourComponent = "Armour_LightClothing", string[]? extraComponents = null)
	{
		return new MedievalWardrobePiece(slotKey, token, noun, shortDescription, fullDescription, material,
			MaterialBehaviourType.Fabric, SizeCategory.Normal, status.Quality, weightInGrams, cost, marketTag,
			functionTag, wearComponent, armourComponent, insulationComponent, extraComponents ?? []);
	}

	private static MedievalWardrobePiece LeatherPiece(string slotKey, string token, string noun, string shortDescription,
		string fullDescription, MedievalStatusRoleProfile status, string marketTag, string functionTag,
		string wearComponent, string insulationComponent, double weightInGrams, decimal cost,
		string armourComponent = "Armour_LightClothing", string[]? extraComponents = null)
	{
		return new MedievalWardrobePiece(slotKey, token, noun, shortDescription, fullDescription, "leather",
			MaterialBehaviourType.Leather, SizeCategory.Normal, status.Quality, weightInGrams, cost, marketTag,
			functionTag, wearComponent, armourComponent, insulationComponent, extraComponents ?? []);
	}

	private static MedievalWardrobePiece BeltPiece(string slotKey, string token, string noun, string shortDescription,
		string fullDescription, string material, MaterialBehaviourType materialType, MedievalStatusRoleProfile status,
		string marketTag, double weightInGrams, decimal cost, string beltComponent,
		string armourComponent = "Armour_LightClothing")
	{
		return new MedievalWardrobePiece(slotKey, token, noun, shortDescription, fullDescription, material,
			materialType, SizeCategory.Small, status.Quality, weightInGrams, cost, marketTag,
			"Functions / Worn Items / Belts", "Wear_Waist", armourComponent, "Insulation_Minor", [beltComponent]);
	}

	private static MedievalWardrobePiece PouchPiece(string slotKey, string token, string noun, string shortDescription,
		string fullDescription, string material, MaterialBehaviourType materialType, MedievalStatusRoleProfile status,
		string marketTag, double weightInGrams, decimal cost, string containerComponent = "Container_Pouch")
	{
		return new MedievalWardrobePiece(slotKey, token, noun, shortDescription, fullDescription, material,
			materialType, SizeCategory.Small, status.Quality, weightInGrams, cost, marketTag,
			"Functions / Worn Items / Pouches", "Wear_Waist", "Armour_LightClothing", "Insulation_Minor",
			[containerComponent, "Beltable"]);
	}

	private static IReadOnlyList<EraItemSpec> MedievalEquipmentItemSpecs()
	{
		var specs = new List<EraItemSpec>();
		foreach (var culture in MedievalCultureProfiles)
		{
			specs.Add(new EraItemSpec(
				$"medieval_military_{culture.Key}_armour",
				"armour",
				culture.ArmourDescription,
				$"This armour package represents {culture.ArmourDescription}, with the regional silhouette captured for builders while using existing mail, lamellar, or padded component behaviour.",
				SizeCategory.Large,
				ItemQuality.Standard,
				9000.0,
				180.0m,
				"wrought iron",
				MaterialBehaviourType.Metal,
				[MedievalRootTagPath, $"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}", "Market / Military Goods / Armour"],
				["Holdable", culture.ArmourDescription.Contains("lamellar", StringComparison.OrdinalIgnoreCase) ? "Armour_Lamellar" : "Armour_Chainmail", "Wear_Hauberk", "Destroyable_Armour"],
				$"Medieval culture slice: {culture.Display}. Military-status armour variant."));
			specs.Add(new EraItemSpec(
				$"medieval_weapon_{culture.Key}_{StableReferenceToken(culture.WeaponNoun)}",
				culture.WeaponNoun,
				culture.WeaponShortDescription,
				culture.WeaponFullDescription,
				SizeCategory.Normal,
				ItemQuality.Standard,
				1600.0,
				90.0m,
				"wrought iron",
				MaterialBehaviourType.Metal,
				[MedievalRootTagPath, $"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}", "Market / Military Goods / Weapons"],
				["Holdable", culture.WeaponComponent, "Destroyable_Weapon"],
				$"Medieval culture slice: {culture.Display}. Military role weapon."));
			specs.Add(new EraItemSpec(
				$"medieval_shield_{culture.Key}",
				"shield",
				culture.ShieldShortDescription,
				culture.ShieldFullDescription,
				SizeCategory.Normal,
				ItemQuality.Standard,
				4500.0,
				55.0m,
				"oak",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, $"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}", "Market / Military Goods / Armour / Shields"],
				["Holdable", culture.ShieldComponent, "Melee_Shield", "Destroyable_Armour"],
				$"Medieval culture slice: {culture.Display}. Military role shield."));
			specs.AddRange(MedievalEquipmentAccessoryItemSpecs(culture));
		}

		specs.AddRange(
		[
			new EraItemSpec(
				"medieval_weapon_common_crossbow",
				"crossbow",
				"a reinforced medieval crossbow",
				"This crossbow has a carved tiller, fitted nut, iron prod, stirrup, and cord ready for towns, castles, militias, hunts, or siege-adjacent scenes.",
				SizeCategory.Normal,
				ItemQuality.Standard,
				3800.0,
				180.0m,
				"oak",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Military Goods / Weapons / Crossbows"],
				["Holdable", "Crossbow", "Melee_Improvised Bludgeon", "Destroyable_Weapon"],
				"Medieval common weapon stock. Crossbow production uses tiller, prod, nut/lockwork, and bowstring subassemblies."),
			new EraItemSpec(
				"medieval_weapon_common_crossbow_bolts",
				"bolt",
				"a crossbow bolt",
				"This short bolt has a stout wooden shaft, quarrel head, and short fletching for crossbow ranges and armour-piercing scenes.",
				SizeCategory.Tiny,
				ItemQuality.Standard,
				75.0,
				2.5m,
				"oak",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Military Goods / Weapons / Crossbows", "Market / Military Goods / Ammunition"],
				["Holdable", "Stack_Number", "Ammo_BroadheadBolt", "Destroyable_Misc"],
				"Medieval common missile stock. Bolt making consumes shaft, head, and fletching stock.")
		]);

		return specs;
	}

	private static IReadOnlyList<EraItemSpec> MedievalEquipmentAccessoryItemSpecs(MedievalCultureProfile culture)
	{
		var cultureTags = new[]
		{
			MedievalRootTagPath,
			$"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}"
		};

		return
		[
			new(
				$"medieval_military_{culture.Key}_helmet",
				"helmet",
				"a regional medieval helmet",
				$"This helmet follows the same regional equipment language as {culture.ArmourDescription}, giving builders a head-protection counterpart to the seeded armour package.",
				SizeCategory.Small,
				ItemQuality.Standard,
				1800.0,
				52.0m,
				"wrought iron",
				MaterialBehaviourType.Metal,
				[cultureTags[0], cultureTags[1], "Market / Military Goods / Armour"],
				["Holdable", "Wear_Hat", "Armour_Chainmail", "Destroyable_Armour"],
				$"Medieval culture slice: {culture.Display}. Military helmet accessory."),
			new(
				$"medieval_military_{culture.Key}_padded_coif",
				"coif",
				"a padded military coif",
				$"This padded coif is cut to sit under or beside {culture.ArmourDescription}, with quilting, tie cords, and a practical campaign finish.",
				SizeCategory.Small,
				ItemQuality.Standard,
				620.0,
				18.0m,
				"linen",
				MaterialBehaviourType.Fabric,
				[cultureTags[0], cultureTags[1], "Market / Military Goods / Armour", "Market / Clothing / Standard Clothing"],
				["Holdable", "Wear_Hat", "Armour_HeavyClothing", "Insulation_Major", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Military padded headwear."),
			new(
				$"medieval_military_{culture.Key}_sidearm_harness",
				"harness",
				"a leather sidearm harness",
				$"This sidearm harness has a belt loop, shoulder keeper, and large sheath fitting sized for the regional weapon pattern: {culture.WeaponShortDescription}.",
				SizeCategory.Small,
				ItemQuality.Standard,
				720.0,
				24.0m,
				"leather",
				MaterialBehaviourType.Leather,
				[cultureTags[0], cultureTags[1], "Market / Military Goods / Weapon Accessories", "Market / Clothing / Standard Clothing"],
				["Holdable", "Sheath_Large", "Wear_Waist", "Beltable", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Military sidearm carriage."),
			new(
				$"medieval_military_{culture.Key}_arrow_quiver",
				"quiver",
				"a leather arrow quiver",
				$"This quiver is a regional military accessory with a reinforced mouth, shoulder strap, and enough internal space for arrows, bolts, or small campaign shafts.",
				SizeCategory.Normal,
				ItemQuality.Standard,
				950.0,
				22.0m,
				"leather",
				MaterialBehaviourType.Leather,
				[cultureTags[0], cultureTags[1], "Market / Military Goods / Weapon Accessories", "Market / Military Goods / Weapons / Bows"],
				["Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Military missile-carriage accessory."),
			new(
				$"medieval_military_{culture.Key}_field_pack",
				"pack",
				"a military field pack",
				$"This field pack has a flap, buckle straps, and enough room for spare cord, rations, repair pieces, and campaign writing scraps.",
				SizeCategory.Normal,
				ItemQuality.Standard,
				1800.0,
				30.0m,
				"leather",
				MaterialBehaviourType.Leather,
				[cultureTags[0], cultureTags[1], "Market / Military Goods / Weapon Accessories", "Market / Household Goods / Standard Wares"],
				["Holdable", "Container_Pack", "Wear_Backpack", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Campaign-carrying accessory."),
			new(
				$"medieval_military_{culture.Key}_war_banner",
				"banner",
				"a regional war banner",
				$"This cloth banner is sized for a spear, hall beam, or camp staff, with regional colour placement left open for builder customisation.",
				SizeCategory.Normal,
				ItemQuality.Standard,
				680.0,
				28.0m,
				"linen",
				MaterialBehaviourType.Fabric,
				[cultureTags[0], cultureTags[1], "Market / Military Goods / Standards and Banners", "Market / Clothing / Standard Clothing"],
				["Holdable", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Military banner and unit-marker accessory.")
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalHouseholdToolItemSpecs()
	{
		return
		[
			new("medieval_coopers_croze", "croze", "a cooper's iron croze",
				"This cooper's croze has a timber body and an iron cutter for grooving staves to receive cask heads.",
				SizeCategory.Small, ItemQuality.Standard, 680.0, 24.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Croze"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_iron_wood_plane", "plane", "a long wooden joiner's plane",
				"This long joiner's plane carries an iron blade in a wedge throat, suitable for doors, chests, trestles, and boards.",
				SizeCategory.Small, ItemQuality.Standard, 1200.0, 32.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Planer"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_bookbinder_press", "press", "a screw bookbinder's press",
				"This heavy timber press has screw pressure and broad boards for flattening gatherings, boards, and leather-covered books.",
				SizeCategory.Large, ItemQuality.Good, 28000.0, 95.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools", "Functions / Tools / Bookbinding Tools / Book Press"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_locksmith_file_set", "files", "a roll of locksmith's files",
				"This leather roll holds small iron files, blanks, and rubbing wax for shaping keys, tuning wards, and fitting chest locks.",
				SizeCategory.Small, ItemQuality.Standard, 650.0, 34.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Security Tools", "Functions / Tools / Metalworking Tools / Locksmithing Tools / Locksmith File Set"], ["Holdable", "Container_Pouch", "Destroyable_Misc"]),
			new("medieval_household_fulling_stocks", "stocks", "a set of timber fulling stocks",
				"These fulling stocks have a trough, foot beams, and enough working room to beat, scour, and thicken wool cloth into a medieval broadcloth finish.",
				SizeCategory.Huge, ItemQuality.Standard, 42000.0, 82.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Fulling Stocks", "Functions / Tools / Textilecraft Tools / Fulling Tools / Fuller's Trough"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_household_teasel_frame", "frame", "a teasel napping frame",
				"This frame is set with rows of dried teasels for raising nap on fulled cloth before it is shorn smooth.",
				SizeCategory.Large, ItemQuality.Standard, 12000.0, 28.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Teasel Frame"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_household_napping_shears", "shears", "a pair of long napping shears",
				"These long-bladed shears are weighted for trimming raised nap and finishing broadcloth with an even surface.",
				SizeCategory.Normal, ItemQuality.Good, 1400.0, 38.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Napping Shears", "Functions / Separation / Shearing / Shears"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_cloth_tenter_frame", "frame", "a cloth tenter frame",
				"This pegged timber frame stretches damp cloth under tension so broadcloth, linens, and dyed lengths dry true.",
				SizeCategory.Huge, ItemQuality.Standard, 48000.0, 78.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Cloth Tenter Frame"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_household_embroidery_frame", "frame", "a small embroidery frame",
				"This small frame holds cloth under tension for trim, inscriptions, heraldic devices, and devotional stitchwork.",
				SizeCategory.Small, ItemQuality.Standard, 420.0, 16.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Embroidery Tools / Embroidery Frame"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_tablet_weaving_cards", "cards", "a pack of tablet-weaving cards",
				"These pierced cards are numbered for weaving patterned bands, girdles, straps, and garment edging.",
				SizeCategory.Tiny, ItemQuality.Standard, 120.0, 12.0m, "bone", MaterialBehaviourType.Bone,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Weaving Tools / Tablet Weaving Cards"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_turnshoe_last", "last", "a wooden turnshoe last",
				"This shaped wooden last is sized for turning and finishing soft leather shoes, boots, and sandals.",
				SizeCategory.Small, ItemQuality.Standard, 820.0, 18.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Leatherworking Tools / Shoe Last"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_bookbinder_sewing_frame", "frame", "a bookbinder's sewing frame",
				"This compact frame holds support cords taut while quires are sewn into codices, account books, and registers.",
				SizeCategory.Normal, ItemQuality.Standard, 3200.0, 42.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_household_leather_paring_knife", "knife", "a leather paring knife",
				"This short sharp knife has a wide blade for thinning bookbinding leather, shoe uppers, and fine pouch panels.",
				SizeCategory.Small, ItemQuality.Standard, 240.0, 18.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Leather Paring Knife", "Functions / Tools / Leatherworking Tools / Leather Paring Knife"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_drawplate", "drawplate", "an iron wire drawplate",
				"This drawplate has rows of tapered holes for drawing wire used in mail, chains, hinges, pins, and fine fittings.",
				SizeCategory.Small, ItemQuality.Good, 4200.0, 72.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Drawplate", "Functions / Consumables / Drawplate"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("medieval_household_armourers_anvil", "anvil", "a small armourer's anvil",
				"This armourer's anvil has a polished face, rounded edge, and small horns for raising plates, riveting rings, and shaping helmets.",
				SizeCategory.Large, ItemQuality.Good, 26000.0, 95.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Armouring Tools / Armourer's Anvil"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("medieval_household_planishing_hammer", "hammer", "a polished planishing hammer",
				"This smooth-faced hammer is balanced for finishing helmets, plates, hinges, and other bright metalwork without deep marks.",
				SizeCategory.Small, ItemQuality.Good, 760.0, 34.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Armouring Tools / Planishing Hammer", "Functions / Tools / Striking Tools / Hammer"], ["Holdable", "Melee_Improvised Bludgeon", "Destroyable_Weapon"]),
			new("medieval_household_mail_riveting_tongs", "tongs", "a pair of mail riveting tongs",
				"These fine-jawed tongs are made for closing and riveting small mail rings without crushing the weave.",
				SizeCategory.Small, ItemQuality.Good, 520.0, 36.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Armouring Tools / Mail Riveting Tongs"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_bow_press", "press", "a bowyer's bow press",
				"This timber press braces staves, prods, and bindings while bows or crossbow parts are tillered and set.",
				SizeCategory.Large, ItemQuality.Standard, 18000.0, 60.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Bowyer Tools / Bow Press"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_household_tillering_stick", "stick", "a marked tillering stick",
				"This marked stick is notched for judging draw, bend, and balance while shaping bows and crossbow prods.",
				SizeCategory.Normal, ItemQuality.Standard, 950.0, 18.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Bowyer Tools / Tillering Stick"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_crossbow_tiller_jig", "jig", "a crossbow tiller jig",
				"This shaped jig holds a crossbow tiller while its groove, nut seat, stirrup, and lockwork are fitted true.",
				SizeCategory.Normal, ItemQuality.Standard, 2600.0, 46.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Bowyer Tools / Crossbow Tiller Jig"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_papermakers_mould", "mould", "a papermaker's mould and deckle",
				"This framed mould and deckle lifts wet sheets from rag pulp and leaves a neat edge for drying and sizing.",
				SizeCategory.Normal, ItemQuality.Standard, 1400.0, 34.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Mould and Deckle"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_papermakers_vat", "vat", "a papermaker's pulp vat",
				"This broad timber vat is sized for soaking rag pulp and keeping it stirred while sheets are drawn.",
				SizeCategory.Large, ItemQuality.Standard, 26000.0, 52.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Papermaking Vat"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_household_wax_spatula", "spatula", "a small wax spatula",
				"This flat spatula smooths melted wax on tablets, seal cakes, charter tags, and tamper marks.",
				SizeCategory.Tiny, ItemQuality.Standard, 90.0, 8.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Scribing Tools / Wax Spatula"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_cheese_press", "press", "a small screw cheese press",
				"This wooden press has a drip board and screw plate for setting curds into firm wheels for household, market, or monastic dairies.",
				SizeCategory.Normal, ItemQuality.Standard, 9200.0, 38.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Food and Drink / Medieval Food / Dairy", "Functions / Tools / Foodmaking Tools / Cheese Press"], ["Holdable", "Destroyable_WoodenHeavy"]),
			new("medieval_household_lauter_tun", "tun", "a small brewing lauter tun",
				"This open tun has a slotted false bottom and stout staves for draining wort from mash in ale, beer, and monastic brewing.",
				SizeCategory.Large, ItemQuality.Standard, 24000.0, 58.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Food and Drink / Medieval Food / Brewing", "Functions / Tools / Foodmaking Tools / Brewing Tools / Lauter Tun"], ["Holdable", "Container_Open_Bin", "Destroyable_WoodenHeavy"]),
			new("medieval_household_millers_sieve", "sieve", "a miller's grain sieve",
				"This framed sieve has a woven mesh for cleaning grain, bolting flour, and separating bran at a mill or bakehouse.",
				SizeCategory.Normal, ItemQuality.Standard, 900.0, 18.0m, "willow", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Milling Tools / Grain Sieve"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_glaziers_grozing_iron", "iron", "a glazier's grozing iron",
				"This small iron nipper chips and trims coloured glass quarries for windows, lanterns, and reliquary panels.",
				SizeCategory.Small, ItemQuality.Standard, 380.0, 24.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassworking Tools / Grozing Iron"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_glaziers_lead_knife", "knife", "a glazier's lead knife",
				"This broad knife cuts and opens soft lead came for fitting coloured glass into panels.",
				SizeCategory.Small, ItemQuality.Standard, 320.0, 20.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassworking Tools / Lead Knife"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_tile_mould", "mould", "a roof tile mould",
				"This rectangular wooden mould shapes clay roof tiles and floor tiles before drying, glazing, and firing.",
				SizeCategory.Normal, ItemQuality.Standard, 1600.0, 18.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Tile Mould"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_glazing_basin", "basin", "a pottery glazing basin",
				"This shallow basin holds glaze slurry for dipping tiles, jugs, lamps, and market wares before a kiln firing.",
				SizeCategory.Small, ItemQuality.Standard, 1400.0, 14.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Glazing Basin"], ["Holdable", "Container_Bowl", "Destroyable_Misc"]),
			new("medieval_household_lantern_pane_mould", "mould", "a lantern-pane casting mould",
				"This small mould shapes thin glass panes for lanterns, reliquaries, and small workshop windows.",
				SizeCategory.Small, ItemQuality.Standard, 1800.0, 34.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Casting Mould / Lantern Pane Mould"], ["Holdable", "Destroyable_Misc"])
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalFoodAndBeverageItemSpecs()
	{
		return MedievalCultureProfiles
			.SelectMany(MedievalFoodwayItemSpecs)
			.Concat(
			[
				new("medieval_food_grain_measure_sack", "sack", "a measured grain sack",
					"This tied grain sack is marked for standard portions and built to be paired with weight and grain-measure instruments.",
					SizeCategory.Normal, ItemQuality.Standard, 9000.0, 18.0m, "linen", MaterialBehaviourType.Fabric,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Grain", "Functions / Tools / Measurement Tools"], ["Holdable", "Container_Pouch", "Destroyable_Clothing"]),
				new("medieval_food_wine_measure_jug", "jug", "a marked wine measure jug",
					"This marked wine jug has a narrow neck and scored fill lines for tavern, monastic, or customs-house measures.",
					SizeCategory.Small, ItemQuality.Standard, 1100.0, 14.0m, "earthenware", MaterialBehaviourType.Ceramic,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Vessels", "Functions / Tools / Measurement Tools"], ["Holdable", "LContainer_WineGlass", "MeasuringInstrument_Antiquity_WineCup", "Destroyable_Misc"],
					MedievalImplementedSealMeasureNote),
				new("medieval_food_oil_measure_jug", "jug", "a marked oil measure jug",
					"This small oil jug has a pinched lip and scored measure marks for kitchen, market, and customs work.",
					SizeCategory.Small, ItemQuality.Standard, 900.0, 12.0m, "earthenware", MaterialBehaviourType.Ceramic,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Vessels", "Functions / Tools / Measurement Tools"], ["Holdable", "LContainer_DrinkingGlass", "MeasuringInstrument_Antiquity_OilCup", "Destroyable_Misc"],
					MedievalImplementedSealMeasureNote),
				new("medieval_food_cheese_mould", "mould", "a slatted cheese mould",
					"This small slatted mould drains curds and gives cheese a portable market shape for dairies, kitchens, and monastery stores.",
					SizeCategory.Small, ItemQuality.Standard, 780.0, 14.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Dairy", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Tray", "Destroyable_WoodenHeavy"]),
				new("medieval_food_butter_churn", "churn", "a small butter churn",
					"This butter churn has a fitted lid, dasher, and tight staves for household, farm, or monastic dairy work.",
					SizeCategory.Normal, ItemQuality.Standard, 7200.0, 28.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Dairy", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Cupboard", "Destroyable_WoodenHeavy"]),
				new("medieval_food_ale_cask", "cask", "a small ale cask",
					"This small ale cask has tight staves, pitch-darkened seams, and enough capacity for tavern, hall, or monastery brewing scenes.",
					SizeCategory.Normal, ItemQuality.Standard, 15000.0, 36.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Brewing", "Market / Household Goods / Standard Wares"], ["Holdable", "LContainer_Barrel", "Destroyable_WoodenHeavy"]),
				new("medieval_food_cider_cask", "cask", "a small cider cask",
					"This small cider cask is coopered for fruit drink storage, with a bung, hoop marks, and travel scuffs.",
					SizeCategory.Normal, ItemQuality.Standard, 15000.0, 34.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Brewing", "Market / Household Goods / Standard Wares"], ["Holdable", "LContainer_Barrel", "Destroyable_WoodenHeavy"]),
				new("medieval_food_mead_crock", "crock", "a sealed mead crock",
					"This pottery crock has a waxed cloth cover and a narrow mouth for mead, small beer, vinegar, or sweet syrups.",
					SizeCategory.Small, ItemQuality.Standard, 1800.0, 18.0m, "earthenware", MaterialBehaviourType.Ceramic,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Brewing", "Food and Drink / Medieval Food / Vessels"], ["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"]),
				new("medieval_food_bakers_peel", "peel", "a long baker's peel",
					"This long baker's peel has a flat wooden blade and a heat-darkened handle for hearth loaves, trenchers, and small pies.",
					SizeCategory.Normal, ItemQuality.Standard, 1400.0, 16.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Baking", "Market / Professional Tools / Standard Tools"], ["Holdable", "Destroyable_WoodenHeavy"]),
				new("medieval_food_bakers_tray", "tray", "a baker's wooden tray",
					"This broad tray is flour-dusted and shallow, suited to carrying loaves, trenchers, pies, or prepared market food.",
					SizeCategory.Normal, ItemQuality.Standard, 1600.0, 14.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Baking", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Tray", "Destroyable_WoodenHeavy"]),
				new("medieval_food_salt_box", "box", "a small salt box",
					"This small lidded box keeps salt dry for kitchens, ships, infirmaries, and preserving tables.",
					SizeCategory.Small, ItemQuality.Standard, 900.0, 12.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Preserving", "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Pouch", "Destroyable_WoodenHeavy"]),
				new("medieval_food_spice_box", "box", "a partitioned spice box",
					"This small partitioned box has labelled compartments for costly spices, dried herbs, saffron threads, or apothecary overlap stock.",
					SizeCategory.Small, ItemQuality.Good, 950.0, 42.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Preserving", "Market / Household Goods / Luxury Wares"], ["Holdable", "Container_Pouch", "Destroyable_WoodenHeavy"]),
				new("medieval_food_brewing_tub", "tub", "a wide brewing tub",
					"This wide tub has heavy staves, an open top, and room for mash, washing, soaking, or large kitchen preparation.",
					SizeCategory.Large, ItemQuality.Standard, 22000.0, 46.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Food and Drink / Medieval Food / Brewing", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Open_Bin", "Destroyable_WoodenHeavy"])
			]).ToArray();
	}

	private static IReadOnlyList<EraItemSpec> MedievalFoodwayItemSpecs(MedievalCultureProfile culture)
	{
		var cultureTag = $"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}";
		return
		[
			new(
				$"medieval_food_{culture.Key}_meal_platter",
				"platter",
				"a regional medieval meal platter",
				$"This meal platter groups reusable foodway cues for the slice: {culture.FoodCue}. It is a prepared-food stock example rather than a complete cuisine system.",
				SizeCategory.Normal,
				ItemQuality.Standard,
				1200.0,
				16.0m,
				"oak",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, cultureTag, "Food and Drink / Medieval Food / Prepared Foods"],
				["Holdable", "Container_Plate", "Destroyable_Misc"],
				$"Medieval culture slice: {culture.Display}. Food cue: {culture.FoodCue}."),
			new(
				$"medieval_food_{culture.Key}_staple_bread",
				"bread",
				"a regional staple bread board",
				$"This board presents the staple bread side of the slice's foodway cue: {culture.FoodCue}. It is suitable for inn tables, ration stock, or household meals.",
				SizeCategory.Small,
				ItemQuality.Standard,
				780.0,
				8.0m,
				"oak",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, cultureTag, "Food and Drink / Medieval Food / Breads"],
				["Holdable", "Container_Plate", "Destroyable_Misc"],
				$"Medieval culture slice: {culture.Display}. Staple bread cue."),
			new(
				$"medieval_food_{culture.Key}_pottage_bowl",
				"bowl",
				"a regional pottage bowl",
				$"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by {culture.FoodCue}.",
				SizeCategory.Small,
				ItemQuality.Standard,
				650.0,
				7.0m,
				"earthenware",
				MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, cultureTag, "Food and Drink / Medieval Food / Prepared Foods", "Food and Drink / Medieval Food / Vessels"],
				["Holdable", "Container_Plate", "Destroyable_Misc"],
				$"Medieval culture slice: {culture.Display}. Everyday bowl cue."),
			new(
				$"medieval_food_{culture.Key}_preserved_provision",
				"packet",
				"a wrapped preserved provision packet",
				$"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: {culture.FoodCue}.",
				SizeCategory.Small,
				ItemQuality.Standard,
				720.0,
				9.0m,
				"linen",
				MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, cultureTag, "Food and Drink / Medieval Food / Preserving"],
				["Holdable", "Container_Pouch", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Preserved provision cue."),
			new(
				$"medieval_food_{culture.Key}_drinking_vessel",
				"cup",
				"a regional drinking vessel",
				$"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by {culture.FoodCue}.",
				SizeCategory.Small,
				ItemQuality.Standard,
				520.0,
				8.0m,
				"earthenware",
				MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, cultureTag, "Food and Drink / Medieval Food / Vessels"],
				["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"],
				$"Medieval culture slice: {culture.Display}. Drinking vessel cue."),
			new(
				$"medieval_food_{culture.Key}_feast_dish",
				"dish",
				"a regional feast dish",
				$"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: {culture.FoodCue}.",
				SizeCategory.Normal,
				ItemQuality.Good,
				1600.0,
				24.0m,
				"oak",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, cultureTag, "Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Luxury Wares"],
				["Holdable", "Container_Tray", "Destroyable_WoodenHeavy"],
				$"Medieval culture slice: {culture.Display}. Feast dish cue."),
			new(
				$"medieval_food_{culture.Key}_market_ration",
				"ration",
				"a wrapped market ration",
				$"This compact ration bundles the portable, purchasable side of {culture.FoodCue} for soldiers, workers, travellers, and market scenes.",
				SizeCategory.Small,
				ItemQuality.Standard,
				900.0,
				10.0m,
				"linen",
				MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, cultureTag, "Food and Drink / Medieval Food / Prepared Foods", "Market / Household Goods / Standard Wares"],
				["Holdable", "Container_Pouch", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Market ration cue.")
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalFurnitureContainerItemSpecs()
	{
		return
		[
			new("medieval_household_trestle_table", "table", "a trestle work table",
				"This trestle table has removable boards and a broad working top for household work, trade, writing, or meals.",
				SizeCategory.Large, ItemQuality.Standard, 32000.0, 55.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture"], ["Holdable", "Container_Table", "Destroyable_Furniture"]),
			new("medieval_household_boarded_chest", "chest", "a boarded oak chest",
				"This oak chest has iron straps, a hinged lid, and enough interior room for cloth, documents, or portable household goods.",
				SizeCategory.Large, ItemQuality.Standard, 26000.0, 62.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Functions / Security Tools"], ["Holdable", "Container_Trunk", "Sealable_Container_Wax", "Destroyable_WoodenHeavy"],
				MedievalImplementedSealMeasureNote),
			new("medieval_household_lockable_strongbox", "strongbox", "a lockable iron-bound strongbox",
				"This compact strongbox has iron straps, a lock plate, and sealing points for wax or clay tamper marks.",
				SizeCategory.Small, ItemQuality.Good, 8500.0, 90.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Functions / Security Tools"], ["Holdable", "LockingContainer_Lockbox", "Sealable_Container_Wax", "Destroyable_WoodenHeavy"],
				MedievalImplementedSealMeasureNote),
			new("medieval_household_aumbry_cupboard", "cupboard", "a small wall aumbry cupboard",
				"This small cupboard has a plain door, shelves, and peg holes for fitting it into a chapel, hall, counting room, or storeroom.",
				SizeCategory.Large, ItemQuality.Standard, 18000.0, 48.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture", "Market / Religious Goods"], ["Holdable", "Container_Cupboard", "Destroyable_Furniture"]),
			new("medieval_household_plank_bench", "bench", "a long plank bench",
				"This long plank bench has trestle feet and a worn sitting surface for halls, taverns, workshops, courts, or market booths.",
				SizeCategory.Large, ItemQuality.Standard, 24000.0, 28.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture"], ["Holdable", "Container_Bench_Surface", "Destroyable_Furniture"]),
			new("medieval_household_three_legged_stool", "stool", "a three-legged wooden stool",
				"This three-legged stool is compact, sturdy, and suited to kitchens, workshops, stalls, and servants' corners.",
				SizeCategory.Normal, ItemQuality.Standard, 6500.0, 18.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture"], ["Holdable", "Destroyable_Furniture"]),
			new("medieval_household_lordly_chair", "chair", "a carved high-backed chair",
				"This high-backed chair has a carved crest rail, heavy legs, and a seat meant for a hall, dais, chamber, or guild room.",
				SizeCategory.Large, ItemQuality.Good, 28000.0, 85.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Luxury Furniture"], ["Holdable", "Container_Couch_Surface", "Destroyable_Furniture"]),
			new("medieval_household_rope_bedframe", "bedframe", "a rope-laced bedframe",
				"This bedframe has pegged rails, a rope-laced support, and enough space for mattress, blankets, and small bedside goods.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 52000.0, 75.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture"], ["Holdable", "Container_Bed_Surface", "Destroyable_Furniture"]),
			new("medieval_household_straw_mattress", "mattress", "a straw-stuffed mattress",
				"This cloth mattress is stuffed with straw or rushes, tied at the edges, and sized for a simple bed, cot, or infirmary pallet.",
				SizeCategory.Large, ItemQuality.Standard, 9000.0, 22.0m, "linen", MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture"], ["Holdable", "Container_Cot_Surface", "Destroyable_Clothing"]),
			new("medieval_household_blanket_chest", "chest", "a blanket chest",
				"This broad blanket chest has a hinged lid and enough interior space for bedding, spare clothing, household linen, or seasonal gear.",
				SizeCategory.Large, ItemQuality.Standard, 30000.0, 54.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Blanket_Box", "Destroyable_WoodenHeavy"]),
			new("medieval_household_wall_shelf", "shelf", "a pegged wall shelf",
				"This wall shelf has peg holes, a raised lip, and enough surface space for bowls, lamps, books, jars, or devotional goods.",
				SizeCategory.Normal, ItemQuality.Standard, 4200.0, 18.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture"], ["Holdable", "Container_Wall_Shelf", "Destroyable_Furniture"]),
			new("medieval_household_book_shelves", "shelves", "a narrow book shelf",
				"This narrow shelf unit is built for codices, rolls, account bundles, wax tablets, and small chapel or school goods.",
				SizeCategory.Large, ItemQuality.Standard, 21000.0, 46.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture", "Market / Writing Materials"], ["Holdable", "Container_Bookcase_Shelves", "Destroyable_Furniture"]),
			new("medieval_household_market_counter", "counter", "a market counter",
				"This market counter has a broad display top, a front plank face, and enough working space for trade, measuring, or food service.",
				SizeCategory.Large, ItemQuality.Standard, 36000.0, 58.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Counter", "Destroyable_Furniture"]),
			new("medieval_household_writing_desk", "desk", "a small writing desk",
				"This small desk has a sloped writing face and shallow space for parchment, wax tablets, pens, seal cord, and account scraps.",
				SizeCategory.Large, ItemQuality.Standard, 24000.0, 62.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture", "Market / Writing Materials"], ["Holdable", "Container_Desk_Surface", "Container_Desk_Drawers", "Destroyable_Furniture"]),
			new("medieval_household_lectern", "lectern", "a standing wooden lectern",
				"This standing lectern has a sloped book rest, a peg for a chain or cord, and a base suited to chapels, schools, or halls.",
				SizeCategory.Large, ItemQuality.Standard, 22000.0, 48.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture", "Market / Religious Goods", "Market / Writing Materials"], ["Holdable", "Container_Desk_Surface", "Destroyable_Furniture"]),
			new("medieval_household_storage_barrel", "barrel", "a coopered storage barrel",
				"This coopered barrel has hoop marks, a tight bung, and a broad enough body for grain, dry goods, ale, wine, oil, or salted stores.",
				SizeCategory.Large, ItemQuality.Standard, 22000.0, 38.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Food and Drink / Medieval Food / Vessels"], ["Holdable", "LContainer_Barrel", "Destroyable_WoodenHeavy"]),
			new("medieval_household_wicker_basket", "basket", "a wicker market basket",
				"This wicker basket has a round belly, a wrapped handle, and room for produce, tools, laundry, kindling, or household trade goods.",
				SizeCategory.Normal, ItemQuality.Standard, 1200.0, 10.0m, "willow", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Open_Bin", "Destroyable_Misc"]),
			new("medieval_household_canvas_sack", "sack", "a stout canvas sack",
				"This canvas sack is heavy enough for grain, wool, charcoal, laundry, or pack goods, with a cord tie at the mouth.",
				SizeCategory.Normal, ItemQuality.Standard, 900.0, 8.0m, "linen", MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Sack", "Destroyable_Clothing"]),
			new("medieval_household_iron_lantern", "lantern", "an iron hand lantern",
				"This iron lantern has a pierced body, a hinged door, and a handle for household, stable, street, or watch use.",
				SizeCategory.Small, ItemQuality.Standard, 1200.0, 26.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Lighting", "Functions / Household Items / Household Lighting", "Functions / Material Functions / Fire"], ["Holdable", "Lantern", "Destroyable_HeavyMetal"]),
			new("medieval_household_charcoal_brazier", "brazier", "a small charcoal brazier",
				"This small brazier is pierced for air flow and suited to chamber heat, workshop heat, cooking support, or ritual atmosphere.",
				SizeCategory.Normal, ItemQuality.Standard, 5200.0, 32.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Lighting", "Functions / Material Functions / Fire", "Market / Household Goods / Standard Wares"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("medieval_household_candle_stand", "stand", "a pricket candle stand",
				"This candle stand has a narrow stem, a pricket point, and a small drip pan for hall, chapel, chamber, or counting-room light.",
				SizeCategory.Normal, ItemQuality.Standard, 1800.0, 24.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Lighting", "Functions / Household Items / Household Lighting", "Market / Religious Goods"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("medieval_household_stained_glass_panel", "panel", "a small stained glass panel",
				"This small leaded panel has coloured glass quarries set in came, suitable for chapels, halls, shrines, wealthy chambers, or lantern repair stock.",
				SizeCategory.Normal, ItemQuality.Good, 2600.0, 95.0m, "glass", MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, "Market / Household Goods / Luxury Decorations", "Market / Religious Goods", "Functions / Household Items / Household Decorations"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_roof_tile_stack", "tiles", "a stack of glazed roof tiles",
				"This stack of fired roof tiles has glazed faces and rough backs, useful for town roofs, chapel repairs, kilns, and high-status building props.",
				SizeCategory.Large, ItemQuality.Standard, 16000.0, 36.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Market / Professional Tools / Standard Tools"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_household_door_bar", "bar", "a heavy door bar",
				"This heavy door bar is a visible security prop for gates, storerooms, halls, and shops until richer door-hardware item behaviour is needed.",
				SizeCategory.Normal, ItemQuality.Standard, 6200.0, 22.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Functions / Security Tools"], ["Holdable", "Destroyable_WoodenHeavy"]),
			new("medieval_household_iron_lockplate", "lockplate", "an iron lockplate",
				"This iron lockplate has a keyhole, rivet marks, and enough weight for a chest, strongbox, shop door, or storeroom door.",
				SizeCategory.Small, ItemQuality.Standard, 950.0, 24.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Functions / Security Tools"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("medieval_household_keyring", "keyring", "a ring of warded keys",
				"This keyring carries several simple warded keys for halls, cupboards, chests, shops, or institutional offices.",
				SizeCategory.Tiny, ItemQuality.Standard, 320.0, 20.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Functions / Security Tools"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("medieval_household_market_stall", "stall", "a collapsible market stall",
				"This market stall has folding trestles, a plank top, and a simple cloth shade for fairs, streets, docks, or town squares.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 46000.0, 68.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Standard Furniture", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Counter", "Destroyable_Furniture"])
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalJewelleryDevotionalItemSpecs()
	{
		return MedievalCultureProfiles
			.Select(culture => new EraItemSpec(
				$"medieval_devotional_{culture.Key}_pilgrim_token",
				"token",
				"a regional devotional token",
				$"This small devotional token is deliberately generic enough for the {culture.Display} slice: a pilgrim badge, amulet, shrine marker, prayer charm, or household blessing piece.",
				SizeCategory.Tiny,
				ItemQuality.Standard,
				35.0,
				8.0m,
				"bronze",
				MaterialBehaviourType.Metal,
				[MedievalRootTagPath, $"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}", "Market / Religious Goods", "Functions / Worn Items / Jewellery / Necklaces"],
				["Holdable", "Wear_Necklace", "Destroyable_HeavyMetal"],
				$"Medieval culture slice: {culture.Display}. Devotional token."))
			.Concat(
		[
			new("medieval_devotional_wooden_rosary", "rosary", "a wooden prayer bead strand",
				"This prayer bead strand has small polished beads, a cord loop, and a plain devotional pendant.",
				SizeCategory.Tiny, ItemQuality.Standard, 90.0, 12.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Religious Goods", "Market / Clothing / Standard Clothing", "Functions / Worn Items / Jewellery / Necklaces"], ["Holdable", "Wear_Necklace", "Destroyable_Misc"]),
			new("medieval_jewellery_silver_brooch", "brooch", "a silver cloak brooch",
				"This silver brooch has a bright pin, a framed face, and enough strength to hold a cloak or formal mantle closed.",
				SizeCategory.Tiny, ItemQuality.Good, 45.0, 75.0m, "silver", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Jewellery"], ["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"]),
			new("medieval_devotional_reliquary_locket", "locket", "a small reliquary locket",
				"This small locket is fitted with a tiny inner compartment for a relic, prayer slip, pressed flower, or private devotional token.",
				SizeCategory.Tiny, ItemQuality.Good, 60.0, 90.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Religious Goods", "Functions / Worn Items / Jewellery / Necklaces"], ["Holdable", "Wear_Necklace", "Container_Pouch", "Destroyable_HeavyMetal"]),
			new("medieval_jewellery_bronze_ring_pin", "pin", "a bronze ring pin",
				"This bronze ring pin is a practical cloak or mantle fastener with a plain hoop and a sharpened pin tongue.",
				SizeCategory.Tiny, ItemQuality.Standard, 35.0, 18.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Clothing / Standard Clothing", "Functions / Worn Items / Jewellery"], ["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"]),
			new("medieval_jewellery_enamel_disc_brooch", "brooch", "an enamelled disc brooch",
				"This disc brooch has a bright enamel face, a pin back, and enough finish for court, burgher, or churchly display.",
				SizeCategory.Tiny, ItemQuality.Good, 50.0, 82.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Jewellery"], ["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"]),
			new("medieval_jewellery_inlaid_belt_mount", "mount", "an inlaid belt mount",
				"This small belt mount has a polished face and rivet holes for decorating a court belt, guild belt, or military officer's harness.",
				SizeCategory.Tiny, ItemQuality.Good, 30.0, 38.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Belts", "Functions / Worn Items / Jewellery"], ["Holdable", "Wear_Waist", "Beltable", "Destroyable_HeavyMetal"]),
			new("medieval_jewellery_court_circlet", "circlet", "a narrow court circlet",
				"This narrow circlet is polished and lightly decorated, suitable for a court household, noble ceremony, or high-status ritual role.",
				SizeCategory.Small, ItemQuality.Good, 180.0, 140.0m, "silver", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Jewellery"], ["Holdable", "Wear_Hat", "Destroyable_HeavyMetal"]),
			new("medieval_devotional_icon_pendant", "pendant", "a painted icon pendant",
				"This small icon pendant has a painted face under a simple frame, with a cord loop for devotional wear or shrine display.",
				SizeCategory.Tiny, ItemQuality.Standard, 45.0, 18.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Religious Goods", "Functions / Worn Items / Jewellery / Necklaces"], ["Holdable", "Wear_Necklace", "Destroyable_Misc"]),
			new("medieval_devotional_pilgrim_badge", "badge", "a cast pilgrim badge",
				"This small badge has a cast devotional emblem and pin points for attaching to a hat, cloak, satchel, or shrine cloth.",
				SizeCategory.Tiny, ItemQuality.Standard, 28.0, 14.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Religious Goods", "Functions / Worn Items / Jewellery"], ["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"]),
			new("medieval_devotional_reliquary_box", "box", "a small reliquary box",
				"This small reliquary box has a close-fitting lid and room for a wrapped relic, prayer slip, seal tag, or private devotional keepsake.",
				SizeCategory.Small, ItemQuality.Good, 260.0, 95.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Religious Goods", "Market / Household Goods / Luxury Wares"], ["Holdable", "Container_Pouch", "Destroyable_HeavyMetal"]),
			new("medieval_devotional_scripture_tablet", "tablet", "a small devotional tablet",
				"This small tablet has a smoothed face for a painted, carved, or written devotional passage and a hole for hanging.",
				SizeCategory.Small, ItemQuality.Standard, 180.0, 16.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Religious Goods", "Market / Writing Materials"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_offering_basin", "basin", "a devotional offering basin",
				"This small bronze basin can sit in a chapel, shrine, or hall to receive votive offerings and devotional gifts.",
				SizeCategory.Small, ItemQuality.Standard, 1400.0, 24.0m, "bronze", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Religious Goods"], ["Holdable", "OfferingReceiver_Antiquity_VotiveBasin", "Destroyable_HeavyMetal"],
				"Uses the live OfferingReceiver stock component prototype until medieval-specific devotional receiver variants are worth splitting."),
			new("medieval_jewellery_silver_finger_ring", "ring", "a simple silver finger ring",
				"This simple silver ring has a plain polished band suitable for betrothal, guild display, devotional wear, or personal ornament.",
				SizeCategory.Tiny, ItemQuality.Good, 18.0, 46.0m, "silver", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Clothing / Luxury Clothing", "Functions / Worn Items / Jewellery"], ["Holdable", "Wear_Ring", "Destroyable_HeavyMetal"])
		]).ToArray();
	}

	private static IReadOnlyList<EraItemSpec> MedievalMedicalApothecaryItemSpecs()
	{
		return
		[
			new("medieval_medical_linen_bandage_roll", "bandage", "a roll of clean linen bandage",
				"This clean linen bandage is rolled tightly for field, household, infirmary, or monastic use.",
				SizeCategory.Tiny, ItemQuality.Standard, 120.0, 5.0m, "linen", MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "Bandage_Simple", "Destroyable_Clothing"]),
			new("medieval_medical_apothecary_mortar", "mortar", "a stone apothecary mortar",
				"This stone mortar has a matching pestle, a stained bowl, and enough weight for roots, gums, mineral powders, and dried herbs.",
				SizeCategory.Small, ItemQuality.Standard, 2200.0, 26.0m, "stone", MaterialBehaviourType.Stone,
				[MedievalRootTagPath, "Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle", "Functions / Medical Items", "Market / Professional Tools / Standard Tools"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_medical_herb_pouch", "pouch", "an apothecary herb pouch",
				"This small pouch is divided into labelled folds for dried herbs, simples, gums, and small wrapped powders.",
				SizeCategory.Tiny, ItemQuality.Standard, 160.0, 12.0m, "leather", MaterialBehaviourType.Leather,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "Container_Pouch", "Destroyable_Clothing"]),
			new("medieval_medical_field_stretcher", "stretcher", "a canvas field stretcher",
				"This field stretcher uses two poles and a laced canvas bed for moving wounded people through halls, streets, camps, or battlefields.",
				SizeCategory.Large, ItemQuality.Standard, 5200.0, 28.0m, "linen", MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Professional Tools / Standard Tools"], ["Holdable", "DragAid_Antiquity_FieldStretcher", "Destroyable_Misc"],
				"Uses the live DragAid stock component prototype until medieval-specific variants are worth splitting."),
			new("medieval_medical_poultice_cloth", "cloth", "a folded poultice cloth",
				"This folded cloth is ready for herbs, honey, vinegar, warm mash, or other poultice treatments in household or infirmary scenes.",
				SizeCategory.Tiny, ItemQuality.Standard, 100.0, 5.0m, "linen", MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "Bandage_Simple", "Destroyable_Clothing"]),
			new("medieval_medical_salve_pot", "pot", "a small salve pot",
				"This small pottery pot is sized for ointments, fats, honeyed preparations, balms, or other external treatments.",
				SizeCategory.Tiny, ItemQuality.Standard, 260.0, 8.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"]),
			new("medieval_medical_apothecary_jar", "jar", "a labelled apothecary jar",
				"This labelled jar has a narrow mouth and a cloth cover for dried simples, syrups, oils, powders, gums, or mineral preparations.",
				SizeCategory.Small, ItemQuality.Standard, 900.0, 16.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"]),
			new("medieval_medical_cupping_horn", "horn", "a polished cupping horn",
				"This polished horn has a smooth rim and narrow end, suitable as a medical prop for cupping, suction, or healer kit scenes.",
				SizeCategory.Tiny, ItemQuality.Standard, 80.0, 10.0m, "bone", MaterialBehaviourType.Bone,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "Destroyable_Misc"]),
			new("medieval_medical_surgical_roll", "roll", "a surgeon's tool roll",
				"This leather roll has narrow pockets for knives, probes, needles, clamps, hooks, thread, and wrapped instrument blanks.",
				SizeCategory.Small, ItemQuality.Standard, 620.0, 34.0m, "leather", MaterialBehaviourType.Leather,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Pouch", "Destroyable_Clothing"]),
			new("medieval_medical_bone_saw", "saw", "a small bone saw",
				"This small iron saw has a stiff blade and a wrapped grip for surgeon, barber, butcher, or battlefield medical scenes.",
				SizeCategory.Small, ItemQuality.Standard, 680.0, 32.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Professional Tools / Standard Tools"], ["Holdable", "Destroyable_HeavyMetal"]),
			new("medieval_medical_splint_set", "splints", "a bundled splint set",
				"This bundled splint set contains thin boards, cloth ties, and padding for immobilising limbs in household, camp, or infirmary work.",
				SizeCategory.Small, ItemQuality.Standard, 700.0, 12.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Pouch", "Destroyable_WoodenHeavy"]),
			new("medieval_medical_crutch_pair", "crutches", "a pair of wooden crutches",
				"This pair of crutches has padded tops, hand grips, and adjusted lengths for convalescent movement or infirmary furnishing.",
				SizeCategory.Normal, ItemQuality.Standard, 2400.0, 22.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Professional Tools / Standard Tools"], ["Holdable", "Destroyable_WoodenHeavy"]),
			new("medieval_medical_physicians_bag", "bag", "a physician's shoulder bag",
				"This leather shoulder bag has internal loops and pockets for jars, bandages, surgical rolls, wax tablets, and small instruments.",
				SizeCategory.Normal, ItemQuality.Good, 1400.0, 54.0m, "leather", MaterialBehaviourType.Leather,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "Container_Tote", "Wear_Shoulder", "Destroyable_Clothing"]),
			new("medieval_medical_monastic_infirmary_kit", "kit", "a monastic infirmary kit",
				"This portable kit bundles bandage rolls, salve pots, herb pouches, a small cup, and record scraps for a monastery or charitable infirmary.",
				SizeCategory.Normal, ItemQuality.Standard, 2400.0, 48.0m, "leather", MaterialBehaviourType.Leather,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine", "Market / Religious Goods"], ["Holdable", "Container_Pack", "Wear_Backpack", "Destroyable_Clothing"]),
			new("medieval_medical_herb_drying_tray", "tray", "a herb drying tray",
				"This shallow tray has a woven base for drying herbs, flowers, roots, cloth strips, or apothecary ingredients.",
				SizeCategory.Normal, ItemQuality.Standard, 1100.0, 12.0m, "willow", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Tray", "Destroyable_Misc"]),
			new("medieval_medical_bleeding_bowl", "bowl", "a small bleeding bowl",
				"This small bowl has a pouring lip and stained interior, suitable for barber-surgeon, infirmary, or ritual medical scenes.",
				SizeCategory.Small, ItemQuality.Standard, 520.0, 14.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[MedievalRootTagPath, "Functions / Medical Items", "Market / Medicine / Herbal Medicine"], ["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"])
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalWritingAdministrationItemSpecs()
	{
		return MedievalCultureProfiles
			.SelectMany(MedievalCultureAdministrationItemSpecs)
			.Concat(
			[
				new("medieval_writing_parchment_charter", "charter", "a sealable parchment charter",
					"This blank parchment charter has a folded foot, room for witness marks, and a prepared tag for wax sealing.",
					SizeCategory.Small, ItemQuality.Standard, 60.0, 16.0m, "parchment", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Writing Materials / Parchment", "Functions / Security Tools"], ["Holdable", "PaperSheet_Scroll", "Sealable_Document_Wax", "Destroyable_Paper"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_sealable_envelope", "envelope", "a sealable parchment envelope",
					"This folded parchment envelope has a closable flap, a small address face, and a prepared seal spot.",
					SizeCategory.Tiny, ItemQuality.Standard, 24.0, 6.0m, "parchment", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Writing Materials / Document Containers", "Functions / Security Tools"], ["Holdable", "Container_Envelope", "PaperSheet_Envelope", "Sealable_Envelope", "Destroyable_Paper"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_office_signet_ring", "ring", "an office signet ring",
					"This signet ring has a raised face cut for official impressions and enough wear to suggest regular chancery or household use.",
					SizeCategory.Tiny, ItemQuality.Good, 28.0, 60.0m, "bronze", MaterialBehaviourType.Metal,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Security Tools"], ["Holdable", "Wear_Ring", "SealStamp_Antiquity_BronzeSignet", "Destroyable_HeavyMetal"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_office_seal_matrix", "matrix", "a bronze office seal matrix",
					"This bronze seal matrix has a flat engraved face, a small loop, and a plain grip for pressing wax seals on charters and packets.",
					SizeCategory.Tiny, ItemQuality.Good, 120.0, 80.0m, "bronze", MaterialBehaviourType.Metal,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Security Tools"], ["Holdable", "SealStamp_Antiquity_BronzeSignet", "Destroyable_HeavyMetal"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_wax_seal_cake", "cake", "a cake of red sealing wax",
					"This small cake of sealing wax is wrapped in cloth and ready to melt for charters, envelopes, bales, or chests.",
					SizeCategory.Tiny, ItemQuality.Standard, 70.0, 5.0m, "beeswax", MaterialBehaviourType.Wax,
					[MedievalRootTagPath, "Market / Writing Materials / Ink", "Functions / Security Tools"], ["Holdable", "Destroyable_Misc"]),
				new("medieval_writing_quill_pen", "pen", "a trimmed quill pen",
					"This trimmed quill pen is cut for ink writing and suited to chancery, monastic, scholastic, mercantile, or household records.",
					SizeCategory.Tiny, ItemQuality.Standard, 14.0, 5.0m, "bone", MaterialBehaviourType.Bone,
					[MedievalRootTagPath, "Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Quill Pen"], ["Holdable", "Destroyable_Misc"]),
				new("medieval_writing_reed_pen", "pen", "a cut reed pen",
					"This cut reed pen has a shaped nib and hollow body for paper, parchment, wax-tablet notes, or practice writing.",
					SizeCategory.Tiny, ItemQuality.Standard, 10.0, 3.0m, "willow", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Reed Pen"], ["Holdable", "Destroyable_Misc"]),
				new("medieval_writing_ink_horn", "horn", "a small ink horn",
					"This small ink horn is stoppered and corded for carrying black ink, coloured ink, or scribe-mixed writing fluid.",
					SizeCategory.Tiny, ItemQuality.Standard, 120.0, 10.0m, "bone", MaterialBehaviourType.Bone,
					[MedievalRootTagPath, "Market / Writing Materials / Ink", "Functions / Tools / Scribing Tools / Inkwell"], ["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"]),
				new("medieval_writing_wax_tablet", "tablet", "a wooden wax tablet",
					"This wooden tablet has a shallow waxed face for temporary notes, school exercises, accounts, passwords, or message drafts.",
					SizeCategory.Small, ItemQuality.Standard, 320.0, 9.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Market / Writing Materials", "Functions / Writing Goods"], ["Holdable", "PaperSheet_Scroll", "Destroyable_Misc"]),
				new("medieval_writing_paper_sheet", "sheet", "a loose paper sheet",
					"This loose paper sheet is sized for letters, contracts, copies, notes, accounts, or chancery drafts.",
					SizeCategory.Tiny, ItemQuality.Standard, 20.0, 4.0m, "parchment", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Writing Materials / Paper", "Functions / Writing Goods"], ["Holdable", "PaperSheet_Scroll", "Destroyable_Paper"]),
				new("medieval_writing_parchment_quire", "quire", "a folded parchment quire",
					"This folded parchment quire is pricked near the spine and ready for binding, record copying, scholarship, liturgy, or legal archives.",
					SizeCategory.Small, ItemQuality.Standard, 180.0, 22.0m, "parchment", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Writing Materials / Parchment", "Functions / Writing Goods"], ["Holdable", "PaperSheet_Scroll", "Destroyable_Paper"]),
				new("medieval_writing_bound_codex", "codex", "a small bound codex",
					"This small codex has stiff boards, sewn gatherings, and room for devotional, scholastic, legal, merchant, or household text.",
					SizeCategory.Small, ItemQuality.Good, 900.0, 80.0m, "parchment", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Writing Materials / Books", "Functions / Writing Goods"], ["Holdable", "Book_Small_40_Page", "Destroyable_Paper"]),
				new("medieval_writing_account_roll", "roll", "a sealed account roll",
					"This account roll is tied with cord, labelled on the outside, and prepared to take a wax seal for audit or archive work.",
					SizeCategory.Small, ItemQuality.Standard, 160.0, 18.0m, "parchment", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Writing Materials", "Functions / Security Tools"], ["Holdable", "PaperSheet_Scroll", "Sealable_Document_Wax", "Destroyable_Paper"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_tally_sticks", "tallies", "a split tally stick set",
					"This split tally set has matching notched pieces for debt, rent, tax, custody, delivery, or market-account scenes.",
					SizeCategory.Small, ItemQuality.Standard, 180.0, 6.0m, "willow", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Market / Writing Materials", "Functions / Writing Goods"], ["Holdable", "Container_Pouch", "Destroyable_Misc"]),
				new("medieval_writing_charter_tag_set", "tags", "a charter tag set",
					"This tag set bundles parchment tongues, cords, and prepared loops for attaching seals to charters and legal packets.",
					SizeCategory.Tiny, ItemQuality.Standard, 45.0, 7.0m, "parchment", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Writing Materials / Parchment", "Functions / Security Tools"], ["Holdable", "Container_Pouch", "Destroyable_Paper"]),
				new("medieval_writing_seal_cord_bundle", "cord", "a bundle of seal cord",
					"This cord bundle is cut and waxed for tying scrolls, charters, packets, trade bales, and labelled account bundles.",
					SizeCategory.Tiny, ItemQuality.Standard, 60.0, 5.0m, "linen", MaterialBehaviourType.Fabric,
					[MedievalRootTagPath, "Market / Writing Materials", "Functions / Security Tools"], ["Holdable", "Destroyable_Clothing"]),
				new("medieval_writing_document_satchel", "satchel", "a sealable document satchel",
					"This leather satchel has a shoulder strap, internal sleeve, and a closing flap prepared for tamper-evident sealing.",
					SizeCategory.Normal, ItemQuality.Standard, 1100.0, 38.0m, "leather", MaterialBehaviourType.Leather,
					[MedievalRootTagPath, "Market / Writing Materials / Document Containers", "Functions / Security Tools"], ["Holdable", "Container_Tote", "Wear_Shoulder", "Sealable_Container_Wax", "Destroyable_Clothing"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_ledger_chest", "chest", "a sealable ledger chest",
					"This narrow chest is sized for ledgers, rolls, tallies, seal matrices, wax cakes, and locked office papers.",
					SizeCategory.Large, ItemQuality.Standard, 18000.0, 68.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Market / Writing Materials / Document Containers", "Functions / Security Tools"], ["Holdable", "Container_Trunk", "Sealable_Container_Wax", "Destroyable_WoodenHeavy"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_notary_kit", "kit", "a notary's sealing kit",
					"This kit holds a small seal, wax, cord, folded sheets, tally slips, and a pouch of witness tags for legal or mercantile work.",
					SizeCategory.Small, ItemQuality.Good, 1600.0, 95.0m, "leather", MaterialBehaviourType.Leather,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Security Tools"], ["Holdable", "Container_Pouch", "SealStamp_Antiquity_BronzeSignet", "Sealable_Container_Wax", "Destroyable_Clothing"],
					MedievalImplementedSealMeasureNote),
				new("medieval_writing_guild_stamp", "stamp", "a guild seal stamp",
					"This small stamp has a plain handle and engraved face for guild, workshop, office, or household authority marks.",
					SizeCategory.Tiny, ItemQuality.Good, 140.0, 64.0m, "bronze", MaterialBehaviourType.Metal,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Security Tools"], ["Holdable", "SealStamp_Antiquity_BronzeSignet", "Destroyable_HeavyMetal"],
					MedievalImplementedSealMeasureNote),
				new("medieval_trade_balance_scale", "scale", "a merchant balance scale",
					"This portable balance scale has paired pans, a folding beam, and cords suitable for table, market, or customs use.",
					SizeCategory.Small, ItemQuality.Standard, 1800.0, 42.0m, "bronze", MaterialBehaviourType.Metal,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Measurement Tools"], ["Holdable", "MeasuringInstrument_Antiquity_BalanceScale", "Destroyable_HeavyMetal"],
					MedievalImplementedSealMeasureNote),
				new("medieval_trade_standard_weight_set", "weights", "a standard weight set",
					"This set of stamped weights nests into a pouch, marked for checking fair trade at market, mill, mint, or customs gate.",
					SizeCategory.Small, ItemQuality.Good, 3000.0, 65.0m, "bronze", MaterialBehaviourType.Metal,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Measurement Tools"], ["Holdable", "Container_Pouch", "MeasuringInstrument_Antiquity_StandardWeights", "Destroyable_HeavyMetal"],
					MedievalImplementedSealMeasureNote),
				new("medieval_trade_false_weight_set", "weights", "a false weight set",
					"This false weight set is outwardly respectable, but its pieces are subtly biased for dishonest measuring.",
					SizeCategory.Small, ItemQuality.Standard, 2900.0, 45.0m, "bronze", MaterialBehaviourType.Metal,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Measurement Tools"], ["Holdable", "Container_Pouch", "MeasuringInstrument_Antiquity_FalseWeights", "Destroyable_HeavyMetal"],
					MedievalImplementedSealMeasureNote),
				new("medieval_trade_grain_measure", "measure", "a wooden grain measure",
					"This wooden grain measure has a level rim, a reinforced base, and calibration marks for granary, mill, or tithe use.",
					SizeCategory.Normal, ItemQuality.Standard, 2400.0, 22.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Measurement Tools"], ["Holdable", "MeasuringInstrument_Antiquity_GrainMeasure", "Destroyable_WoodenHeavy"],
					MedievalImplementedSealMeasureNote),
				new("medieval_trade_tax_customs_kit", "kit", "a tax and customs measuring kit",
					"This kit bundles a folding scale, nested weights, cord tags, tally slips, and sealing wax for inspecting taxable goods.",
					SizeCategory.Small, ItemQuality.Good, 5200.0, 120.0m, "oak", MaterialBehaviourType.Wood,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Measurement Tools", "Functions / Security Tools"], ["Holdable", "Container_Pouch", "MeasuringInstrument_Antiquity_TaxAssessorKit", "Sealable_Container_Wax", "Destroyable_WoodenHeavy"],
					MedievalImplementedSealMeasureNote),
				new("medieval_trade_sealable_bale", "bale", "a sealable cloth trade bale",
					"This trade bale is wrapped in stout cloth, tied with cord, and prepared to take a wax or clay seal as customs evidence.",
					SizeCategory.Large, ItemQuality.Standard, 18000.0, 35.0m, "linen", MaterialBehaviourType.Fabric,
					[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Functions / Security Tools"], ["Holdable", "Container_Trunk", "Sealable_Container_Wax", "Destroyable_Clothing"],
					MedievalImplementedSealMeasureNote),
				new("medieval_surveyor_measuring_rope", "rope", "a knotted measuring rope",
					"This knotted rope is marked for field and building measures, but remains a prop until length and surveying measurement modes exist.",
					SizeCategory.Small, ItemQuality.Standard, 900.0, 12.0m, "hemp", MaterialBehaviourType.Plant,
					[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Tools / Measurement Tools"], ["Holdable", "Destroyable_Misc"],
					MedievalDeferredGapNote)
			]).ToArray();
	}

	private static IReadOnlyList<EraItemSpec> MedievalCultureAdministrationItemSpecs(MedievalCultureProfile culture)
	{
		var cultureTag = $"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}";
		return
		[
			new(
				$"medieval_writing_{culture.Key}_office_bundle",
				"bundle",
				"a sealed administrative document bundle",
				$"This tied bundle represents regional administrative practice for builders: {culture.WritingCue}. It has prepared sealing points and a writable surface.",
				SizeCategory.Small,
				ItemQuality.Standard,
				180.0,
				20.0m,
				"parchment",
				MaterialBehaviourType.Plant,
				[MedievalRootTagPath, cultureTag, "Market / Writing Materials", "Functions / Security Tools"],
				["Holdable", "PaperSheet_Scroll", "Sealable_Document_Wax", "Destroyable_Paper"],
				$"{MedievalImplementedSealMeasureNote} Medieval culture slice: {culture.Display}."),
			new(
				$"medieval_writing_{culture.Key}_record_tablet",
				"tablet",
				"a regional record tablet",
				$"This tablet gives the {culture.Display} slice a reusable short-record surface inspired by {culture.WritingCue}.",
				SizeCategory.Small,
				ItemQuality.Standard,
				420.0,
				10.0m,
				"oak",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, cultureTag, "Market / Writing Materials", "Functions / Writing Goods"],
				["Holdable", "PaperSheet_Scroll", "Destroyable_Misc"],
				$"Medieval culture slice: {culture.Display}. Regional short-record surface."),
			new(
				$"medieval_writing_{culture.Key}_tally_bundle",
				"bundle",
				"a regional tally bundle",
				$"This tied tally bundle represents local counting, custody, taxation, rent, or trade practices suggested by {culture.WritingCue}.",
				SizeCategory.Small,
				ItemQuality.Standard,
				260.0,
				8.0m,
				"willow",
				MaterialBehaviourType.Wood,
				[MedievalRootTagPath, cultureTag, "Market / Writing Materials", "Functions / Writing Goods"],
				["Holdable", "Container_Pouch", "Destroyable_Misc"],
				$"Medieval culture slice: {culture.Display}. Tally and account prop."),
			new(
				$"medieval_writing_{culture.Key}_seal_tag_packet",
				"packet",
				"a regional seal-tag packet",
				$"This small packet holds tags, knots, strings, slips, or labels for authority, witness, or custody marks in the style of {culture.WritingCue}.",
				SizeCategory.Tiny,
				ItemQuality.Standard,
				80.0,
				9.0m,
				"linen",
				MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, cultureTag, "Market / Writing Materials", "Functions / Security Tools"],
				["Holdable", "Container_Pouch", "Destroyable_Clothing"],
				$"Medieval culture slice: {culture.Display}. Seal-tag and office-label support.")
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalComponentGapPropItemSpecs()
	{
		return
		[
			new("medieval_music_psaltery", "psaltery", "a small psaltery",
				"This small psaltery has a shallow soundbox and wire strings. It is a social prop until musical instrument components exist.",
				SizeCategory.Small, ItemQuality.Standard, 1200.0, 40.0m, "oak", MaterialBehaviourType.Wood,
				[MedievalRootTagPath, "Market / Household Goods / Luxury Decorations", "Functions / Household Items / Leisure Goods"], ["Holdable", "Destroyable_Misc"], MedievalDeferredGapNote),
			new("medieval_game_chess_set", "set", "a carved chess set",
				"This carved chess set includes a folding board and simple pieces. It remains a prop until rules-aware game-set components exist.",
				SizeCategory.Small, ItemQuality.Good, 900.0, 55.0m, "bone", MaterialBehaviourType.Bone,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares", "Functions / Household Items / Leisure Goods"], ["Holdable", "Container_Tray", "Destroyable_Misc"], MedievalDeferredGapNote),
			new("medieval_horse_tack_display_set", "set", "a horse tack display set",
				"This bundle of bridle, straps, and harness fittings is useful as stock decor or trade goods until animal tack and harness components exist.",
				SizeCategory.Normal, ItemQuality.Standard, 3200.0, 38.0m, "leather", MaterialBehaviourType.Leather,
				[MedievalRootTagPath, "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Pouch", "Destroyable_Clothing"], MedievalDeferredGapNote)
		];
	}

	private static IReadOnlyList<EraItemSpec> MedievalRepairKitItemSpecs()
	{
		return
		[
			new("medieval_textile_repair_kit", "kit", "a textile repair kit",
				"This compact repair kit holds patches, thread, a needle case, and small shears for clothing, padding, and textile furnishings.",
				SizeCategory.Small, ItemQuality.Standard, 650.0, 22.0m, "linen", MaterialBehaviourType.Fabric,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Repairing"], ["Holdable", "Container_Pouch", "Repair_Cloth", "Destroyable_Misc"]),
			new("medieval_leather_repair_kit", "kit", "a leather repair kit",
				"This leather repair kit contains awls, waxed thread, small patches, and strap offcuts for field and workshop repairs.",
				SizeCategory.Small, ItemQuality.Standard, 900.0, 28.0m, "leather", MaterialBehaviourType.Leather,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Repairing"], ["Holdable", "Container_Pouch", "Repair_Leather", "Destroyable_Misc"]),
			new("medieval_metal_repair_kit", "kit", "a metal repair kit",
				"This metal repair kit carries rivets, wire, small plates, and fitting blanks for armour, tools, locks, and household hardware.",
				SizeCategory.Small, ItemQuality.Standard, 1800.0, 38.0m, "wrought iron", MaterialBehaviourType.Metal,
				[MedievalRootTagPath, "Market / Professional Tools / Standard Tools", "Functions / Repairing"], ["Holdable", "Container_Pouch", "Repair_Metal", "Destroyable_Misc"])
		];
	}

	private static MedievalCultureCatalogue[] BuildMedievalCultureCatalogues()
	{
		var rows = MedievalExplicitCultureCatalogueSource
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var groupedReferences = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);

		foreach (var row in rows)
		{
			var parts = row.Split('|', 3, StringSplitOptions.TrimEntries);
			if (parts.Length != 3)
			{
				throw new ApplicationException($"Invalid medieval culture catalogue row: {row}");
			}

			var cultureKey = parts[0];
			var group = parts[1];
			if (!MedievalCultureCatalogueGroups.Contains(group, StringComparer.Ordinal))
			{
				throw new ApplicationException($"Invalid medieval culture catalogue group {group} for {cultureKey}.");
			}

			var stableReferences = parts[2]
				.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			if (!groupedReferences.TryGetValue(cultureKey, out var cultureGroups))
			{
				cultureGroups = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
				groupedReferences[cultureKey] = cultureGroups;
			}

			cultureGroups[group] = stableReferences.ToList();
		}

		return MedievalCultureProfiles
			.Select(culture =>
			{
				if (!groupedReferences.TryGetValue(culture.Key, out var cultureGroups))
				{
					throw new ApplicationException($"Medieval culture catalogue is missing {culture.Key}.");
				}

				var stableReferencesByGroup = MedievalCultureCatalogueGroups
					.ToDictionary(
						group => group,
						group =>
						{
							if (!cultureGroups.TryGetValue(group, out var stableReferences))
							{
								throw new ApplicationException(
									$"Medieval culture catalogue is missing {culture.Key} / {group}.");
							}

							return (IReadOnlyCollection<string>)stableReferences.ToArray();
						},
						StringComparer.OrdinalIgnoreCase);

				return new MedievalCultureCatalogue(culture.Key, culture.Display, stableReferencesByGroup);
			})
			.ToArray();
	}

	private static MedievalCultureCatalogueReferenceClassification ClassifyMedievalCultureCatalogueReference(
		string cultureKey, string group, string stableReference)
	{
		var implementedStableReferences = MedievalAllItemSpecs()
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		if (implementedStableReferences.Contains(stableReference))
		{
			return new MedievalCultureCatalogueReferenceClassification(
				MedievalCultureCatalogueReferenceStatus.ImplementedItem,
				stableReference,
				null,
				null);
		}

		if (TryGetMedievalOutfitPieceCoverage(cultureKey, group, stableReference, out var outfitCoverage))
		{
			return new MedievalCultureCatalogueReferenceClassification(
				MedievalCultureCatalogueReferenceStatus.CoveredByOutfitPiece,
				outfitCoverage,
				MedievalCatalogueOutfitCoverageReason,
				null);
		}

		if (TryGetMedievalCultureCatalogueAlias(cultureKey, group, stableReference, implementedStableReferences,
			    out var implementationStableReference))
		{
			return new MedievalCultureCatalogueReferenceClassification(
				MedievalCultureCatalogueReferenceStatus.AliasOfExistingStableReference,
				implementationStableReference,
				MedievalCatalogueAliasReason,
				null);
		}

		return new MedievalCultureCatalogueReferenceClassification(
			MedievalCultureCatalogueReferenceStatus.Deferred,
			null,
			MedievalCatalogueDeferredReasonForGroup(group),
			"Deferred");
	}

	private static bool TryGetMedievalOutfitPieceCoverage(string cultureKey, string group, string stableReference,
		out string implementationStableReference)
	{
		implementationStableReference = string.Empty;
		if (!group.Equals(MedievalCatalogueGroupClothing, StringComparison.Ordinal))
		{
			return false;
		}

		var tokenWords = MedievalCatalogueReferenceWords(cultureKey, stableReference);
		var matches = MedievalExplicitOutfitPieces
			.Where(x => x.CultureKey.Equals(cultureKey, StringComparison.OrdinalIgnoreCase))
			.Select(piece => (
				piece.StableReference,
				Score: MedievalCatalogueWordOverlapScore(tokenWords, MedievalCatalogueReferenceWords(cultureKey, piece.PieceName))))
			.Where(x => x.Score >= 2 || tokenWords.Count <= 2 && x.Score >= 1)
			.OrderByDescending(x => x.Score)
			.ThenBy(x => x.StableReference, StringComparer.OrdinalIgnoreCase)
			.Select(x => x.StableReference)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Take(3)
			.ToArray();

		if (matches.Length == 0)
		{
			return false;
		}

		implementationStableReference = string.Join("; ", matches);
		return true;
	}

	private static bool TryGetMedievalCultureCatalogueAlias(string cultureKey, string group, string stableReference,
		IReadOnlySet<string> implementedStableReferences, out string implementationStableReference)
	{
		implementationStableReference = group switch
		{
			MedievalCatalogueGroupMilitary => MedievalMilitaryCatalogueAlias(cultureKey, stableReference),
			MedievalCatalogueGroupFoodAndBeverage => MedievalFoodCatalogueAlias(cultureKey, stableReference),
			MedievalCatalogueGroupWritingAndAdministration => MedievalWritingCatalogueAlias(cultureKey, stableReference),
			MedievalCatalogueGroupHouseholdAndDevotional => MedievalHouseholdDevotionalCatalogueAlias(cultureKey, stableReference),
			_ => string.Empty
		};

		if (string.IsNullOrWhiteSpace(implementationStableReference) ||
		    !implementedStableReferences.Contains(implementationStableReference))
		{
			implementationStableReference = string.Empty;
			return false;
		}

		return true;
	}

	private static string MedievalMilitaryCatalogueAlias(string cultureKey, string stableReference)
	{
		var token = MedievalCatalogueReferenceToken(cultureKey, stableReference);
		if (stableReference.StartsWith($"medieval_shield_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			return $"medieval_shield_{cultureKey}";
		}

		if (stableReference.StartsWith($"medieval_weapon_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			if (token.Contains("crossbow", StringComparison.OrdinalIgnoreCase))
			{
				return "medieval_weapon_common_crossbow";
			}

			var culture = MedievalCultureProfiles.Single(x => x.Key.Equals(cultureKey, StringComparison.OrdinalIgnoreCase));
			var implementation = $"medieval_weapon_{culture.Key}_{StableReferenceToken(culture.WeaponNoun)}";
			var implementationToken = MedievalCatalogueReferenceToken(cultureKey, implementation);
			return MedievalCatalogueTokensOverlap(token, implementationToken) ? implementation : string.Empty;
		}

		if (!stableReference.StartsWith($"medieval_military_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			return string.Empty;
		}

		if (MedievalCatalogueTokenContainsAny(token, "mail", "hauberk", "shirt", "coat", "armour", "armor", "lamellar",
			    "corselet", "aketon", "gambeson", "quilted", "padded", "scale", "vest", "fittings"))
		{
			return $"medieval_military_{cultureKey}_armour";
		}

		if (MedievalCatalogueTokenContainsAny(token, "helm", "helmet", "kettle", "aventail"))
		{
			return $"medieval_military_{cultureKey}_helmet";
		}

		if (token.Contains("coif", StringComparison.OrdinalIgnoreCase))
		{
			return $"medieval_military_{cultureKey}_padded_coif";
		}

		if (MedievalCatalogueTokenContainsAny(token, "quiver", "gorytos", "bowcase", "arrow_bag"))
		{
			return $"medieval_military_{cultureKey}_arrow_quiver";
		}

		if (MedievalCatalogueTokenContainsAny(token, "belt", "harness", "sling"))
		{
			return $"medieval_military_{cultureKey}_sidearm_harness";
		}

		if (MedievalCatalogueTokenContainsAny(token, "banner", "standard"))
		{
			return $"medieval_military_{cultureKey}_war_banner";
		}

		return token.Contains("pack", StringComparison.OrdinalIgnoreCase)
			? $"medieval_military_{cultureKey}_field_pack"
			: string.Empty;
	}

	private static string MedievalFoodCatalogueAlias(string cultureKey, string stableReference)
	{
		var token = MedievalCatalogueReferenceToken(cultureKey, stableReference);
		var target = MedievalCatalogueTokenContainsAny(token, "cup", "jack", "jug", "horn", "skin", "drink", "wine",
			             "ale", "beer", "tea", "kumis", "sherbet", "syrup", "mare_milk", "sour_milk")
			? "drinking_vessel"
			: MedievalCatalogueTokenContainsAny(token, "ration", "packet", "stockfish", "smoked", "salted", "dried",
				"preserved", "travel", "shipboard", "wrapped")
				? "market_ration"
				: MedievalCatalogueTokenContainsAny(token, "bread", "loaf", "flatbread", "bannock", "trencher",
					"noodle", "bun")
					? "staple_bread"
					: MedievalCatalogueTokenContainsAny(token, "pottage", "stew", "gruel", "pilaf", "bowl", "relish",
						"greens", "olive", "lentil", "curd", "cheese", "rice")
						? "pottage_bowl"
						: MedievalCatalogueTokenContainsAny(token, "feast", "roast", "platter", "pastry", "cake",
							"sweet", "dish", "meat", "fish", "snack")
							? "feast_dish"
							: "meal_platter";

		return $"medieval_food_{cultureKey}_{target}";
	}

	private static string MedievalWritingCatalogueAlias(string cultureKey, string stableReference)
	{
		var token = MedievalCatalogueReferenceToken(cultureKey, stableReference);
		var target = token.Contains("tally", StringComparison.OrdinalIgnoreCase)
			? "tally_bundle"
			: MedievalCatalogueTokenContainsAny(token, "seal", "tag", "packet", "cord")
				? "seal_tag_packet"
				: MedievalCatalogueTokenContainsAny(token, "tablet", "diptych", "board", "plaque", "notice")
					? "record_tablet"
					: "office_bundle";

		return $"medieval_writing_{cultureKey}_{target}";
	}

	private static string MedievalHouseholdDevotionalCatalogueAlias(string cultureKey, string stableReference)
	{
		var token = MedievalCatalogueReferenceToken(cultureKey, stableReference);
		if (stableReference.StartsWith($"medieval_trade_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			return MedievalCatalogueTokenContainsAny(token, "weight", "balance")
				? "medieval_trade_balance_scale"
				: string.Empty;
		}

		if (stableReference.StartsWith($"medieval_music_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			return "medieval_music_psaltery";
		}

		if (stableReference.StartsWith($"medieval_jewellery_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			if (MedievalCatalogueTokenContainsAny(token, "ring_pin", "pin"))
			{
				return "medieval_jewellery_bronze_ring_pin";
			}

			if (MedievalCatalogueTokenContainsAny(token, "brooch", "fibula", "clasp"))
			{
				return "medieval_jewellery_silver_brooch";
			}

			if (MedievalCatalogueTokenContainsAny(token, "pendant", "necklace"))
			{
				return "medieval_devotional_icon_pendant";
			}
		}

		if (stableReference.StartsWith($"medieval_devotional_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			if (MedievalCatalogueTokenContainsAny(token, "reliquary"))
			{
				return "medieval_devotional_reliquary_box";
			}

			if (MedievalCatalogueTokenContainsAny(token, "bead", "rosary"))
			{
				return "medieval_devotional_wooden_rosary";
			}

			if (MedievalCatalogueTokenContainsAny(token, "icon"))
			{
				return "medieval_devotional_icon_pendant";
			}

			if (MedievalCatalogueTokenContainsAny(token, "scripture", "tablet"))
			{
				return "medieval_devotional_scripture_tablet";
			}

			if (MedievalCatalogueTokenContainsAny(token, "basin", "offering"))
			{
				return "medieval_offering_basin";
			}

			return $"medieval_devotional_{cultureKey}_pilgrim_token";
		}

		if (!stableReference.StartsWith($"medieval_household_{cultureKey}_", StringComparison.OrdinalIgnoreCase))
		{
			return string.Empty;
		}

		if (MedievalCatalogueTokenContainsAny(token, "chest", "box", "casket", "case", "cupboard"))
		{
			return "medieval_household_boarded_chest";
		}

		if (MedievalCatalogueTokenContainsAny(token, "bag", "saddlebag", "pouch", "satchel"))
		{
			return "medieval_writing_document_satchel";
		}

		if (MedievalCatalogueTokenContainsAny(token, "lamp", "lantern"))
		{
			return "medieval_household_iron_lantern";
		}

		if (MedievalCatalogueTokenContainsAny(token, "sconce", "candle"))
		{
			return "medieval_household_candle_stand";
		}

		if (MedievalCatalogueTokenContainsAny(token, "book_stand", "lectern", "stand"))
		{
			return "medieval_household_lectern";
		}

		if (MedievalCatalogueTokenContainsAny(token, "shelf", "rack"))
		{
			return "medieval_household_wall_shelf";
		}

		if (MedievalCatalogueTokenContainsAny(token, "table"))
		{
			return "medieval_household_trestle_table";
		}

		if (MedievalCatalogueTokenContainsAny(token, "bench"))
		{
			return "medieval_household_plank_bench";
		}

		if (MedievalCatalogueTokenContainsAny(token, "counter"))
		{
			return "medieval_household_market_counter";
		}

		if (MedievalCatalogueTokenContainsAny(token, "bale"))
		{
			return "medieval_trade_sealable_bale";
		}

		if (MedievalCatalogueTokenContainsAny(token, "horn", "cup", "bowl", "jug", "vessel", "crock", "flask"))
		{
			return $"medieval_food_{cultureKey}_drinking_vessel";
		}

		return string.Empty;
	}

	private static string MedievalCatalogueReferenceToken(string cultureKey, string stableReference)
	{
		foreach (var prefix in new[]
		         {
			         $"medieval_clothing_{cultureKey}_",
			         $"medieval_jewellery_{cultureKey}_",
			         $"medieval_devotional_{cultureKey}_",
			         $"medieval_military_{cultureKey}_",
			         $"medieval_weapon_{cultureKey}_",
			         $"medieval_shield_{cultureKey}_",
			         $"medieval_food_{cultureKey}_",
			         $"medieval_writing_{cultureKey}_",
			         $"medieval_household_{cultureKey}_",
			         $"medieval_trade_{cultureKey}_",
			         $"medieval_music_{cultureKey}_"
		         })
		{
			if (stableReference.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			{
				return stableReference[prefix.Length..];
			}
		}

		var cultureMarker = $"_{cultureKey}_";
		var cultureIndex = stableReference.IndexOf(cultureMarker, StringComparison.OrdinalIgnoreCase);
		return cultureIndex >= 0
			? stableReference[(cultureIndex + cultureMarker.Length)..]
			: stableReference[(stableReference.LastIndexOf('_') + 1)..];
	}

	private static IReadOnlyCollection<string> MedievalCatalogueReferenceWords(string cultureKey, string text)
	{
		return StableReferenceToken(MedievalCatalogueReferenceToken(cultureKey, text))
			.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Where(x => !MedievalCatalogueLowSignalWords.Contains(x, StringComparer.OrdinalIgnoreCase))
			.ToArray();
	}

	private static int MedievalCatalogueWordOverlapScore(IReadOnlyCollection<string> left, IReadOnlyCollection<string> right)
	{
		return left
			.Intersect(right, StringComparer.OrdinalIgnoreCase)
			.Count();
	}

	private static bool MedievalCatalogueTokenContainsAny(string token, params string[] terms)
	{
		return terms.Any(term => token.Contains(term, StringComparison.OrdinalIgnoreCase));
	}

	private static bool MedievalCatalogueTokensOverlap(string left, string right)
	{
		return left.Contains(right, StringComparison.OrdinalIgnoreCase) ||
		       right.Contains(left, StringComparison.OrdinalIgnoreCase);
	}

	private static string MedievalCatalogueDeferredReasonForGroup(string group)
	{
		return group switch
		{
			MedievalCatalogueGroupClothing =>
				"Exact clothing target is documented but not yet tied to a dedicated item spec or sufficiently close explicit outfit piece.",
			MedievalCatalogueGroupMilitary =>
				"Distinct weapon, armour, or military accessory target awaits a dedicated exact item spec or richer component family.",
			MedievalCatalogueGroupFoodAndBeverage =>
				"Distinct food or beverage target awaits prepared-food or vessel content beyond the current generated foodway families.",
			MedievalCatalogueGroupWritingAndAdministration =>
				"Distinct writing or administration target awaits a dedicated document, inscribable surface, or office-tool item spec.",
			MedievalCatalogueGroupHouseholdAndDevotional =>
				"Distinct household, trade, devotional, jewellery, music, game, or tack target awaits a dedicated exact item spec.",
			_ => MedievalCatalogueDeferredReason
		};
	}

	private static string BuildMedievalExplicitShortDescription(string cultureKey, string stableReference)
	{
		var cultureMarker = $"_{cultureKey}_";
		var cultureIndex = stableReference.IndexOf(cultureMarker, StringComparison.Ordinal);
		var token = cultureIndex >= 0
			? stableReference[(cultureIndex + cultureMarker.Length)..]
			: stableReference[(stableReference.LastIndexOf('_') + 1)..];
		var itemName = token.Replace('_', ' ');
		var article = "aeiou".Contains(char.ToLowerInvariant(itemName[0])) ? "an" : "a";
		return $"{article} {itemName}";
	}

	private static EraOutfitPieceSpec[] BuildMedievalExplicitOutfitPieces()
	{
		return MedievalExplicitOutfitPieceSource
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.SelectMany(row =>
			{
				var parts = row.Split('|', 2, StringSplitOptions.TrimEntries);
				if (parts.Length != 2)
				{
					throw new ApplicationException($"Invalid medieval outfit piece row: {row}");
				}

				var outfitReference = parts[0];
				var (cultureKey, sexGenderPresentation, socialClassRole) =
					ParseMedievalOutfitReference(outfitReference);
				if (!MedievalExplicitOutfitCultureKeys.Contains(cultureKey, StringComparer.OrdinalIgnoreCase))
				{
					throw new ApplicationException($"Medieval explicit outfit source includes out-of-scope culture {cultureKey}.");
				}

				var pieces = parts[1]
					.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				return BuildMedievalOutfitPieceSlotAssignments(socialClassRole, pieces)
					.Select(assignment =>
					{
						var stableReference =
							$"medieval_outfit_piece_{cultureKey}_{sexGenderPresentation}_{socialClassRole}_{StableReferenceToken(assignment.PieceName)}";
						return new EraOutfitPieceSpec(
							outfitReference,
							cultureKey,
							sexGenderPresentation,
							socialClassRole,
							assignment.SlotKey,
							assignment.PieceName,
							stableReference,
							true);
					});
			})
			.ToArray();
	}

	private static (string CultureKey, string SexGenderPresentation, string SocialClassRole) ParseMedievalOutfitReference(
		string outfitReference)
	{
		const string Prefix = "medieval_outfit_";
		if (!outfitReference.StartsWith(Prefix, StringComparison.Ordinal))
		{
			throw new ApplicationException($"Invalid medieval outfit reference {outfitReference}.");
		}

		var remainder = outfitReference[Prefix.Length..];
		var socialClassRole = MedievalOutfitSocialClassRoleKeys.SingleOrDefault(role =>
			remainder.EndsWith($"_{role}", StringComparison.Ordinal));
		if (string.IsNullOrWhiteSpace(socialClassRole))
		{
			throw new ApplicationException($"Invalid medieval outfit role in {outfitReference}.");
		}

		var withoutRole = remainder[..^(socialClassRole.Length + 1)];
		var sexGenderPresentation = MedievalOutfitSexGenderPresentationKeys.SingleOrDefault(sex =>
			withoutRole.EndsWith($"_{sex}", StringComparison.Ordinal));
		if (string.IsNullOrWhiteSpace(sexGenderPresentation))
		{
			throw new ApplicationException($"Invalid medieval outfit sex/gender presentation in {outfitReference}.");
		}

		var cultureKey = withoutRole[..^(sexGenderPresentation.Length + 1)];
		if (!MedievalCultureProfiles.Any(x => x.Key.Equals(cultureKey, StringComparison.OrdinalIgnoreCase)))
		{
			throw new ApplicationException($"Invalid medieval outfit culture in {outfitReference}.");
		}

		return (cultureKey, sexGenderPresentation, socialClassRole);
	}

	private static IReadOnlyCollection<(string SlotKey, string PieceName)> BuildMedievalOutfitPieceSlotAssignments(
		string socialClassRole, string[] pieces)
	{
		if (!MedievalOutfitRoleItemRequiredForRole(socialClassRole))
		{
			if (pieces.Length < 10)
			{
				throw new ApplicationException($"{socialClassRole} outfit rows require at least ten target pieces.");
			}

			if (pieces.Length == 10 && IsMedievalOutfitFootwearPiece(pieces[2]))
			{
				return
				[
					(EraOutfitSlotSpecUnderlayer, pieces[0]),
					(EraOutfitSlotSpecLowerBody, pieces[1]),
					(EraOutfitSlotSpecLegOrSockLayer, pieces[1]),
					(EraOutfitSlotSpecFootwear, pieces[2]),
					(EraOutfitSlotSpecBodywear, pieces[3]),
					(EraOutfitSlotSpecOuterwear, pieces[4]),
					(EraOutfitSlotSpecHeadwear, pieces[5]),
					(EraOutfitSlotSpecBeltOrSash, pieces[6]),
					(EraOutfitSlotSpecWornContainer, pieces[7]),
					(EraOutfitSlotSpecFastenerOrJewellery, pieces[8]),
					(EraOutfitSlotSpecRoleItem, pieces[9])
				];
			}

			return AssignMedievalOutfitPieces(
				pieces,
				[
					EraOutfitSlotSpecUnderlayer,
					EraOutfitSlotSpecLowerBody,
					EraOutfitSlotSpecLegOrSockLayer,
					EraOutfitSlotSpecFootwear,
					EraOutfitSlotSpecBodywear,
					EraOutfitSlotSpecOuterwear,
					EraOutfitSlotSpecHeadwear,
					EraOutfitSlotSpecBeltOrSash,
					EraOutfitSlotSpecWornContainer,
					EraOutfitSlotSpecFastenerOrJewellery
				]);
		}

		if (pieces.Length >= 11)
		{
			return AssignMedievalOutfitPieces(
				pieces,
				[
					EraOutfitSlotSpecUnderlayer,
					EraOutfitSlotSpecLowerBody,
					EraOutfitSlotSpecLegOrSockLayer,
					EraOutfitSlotSpecFootwear,
					EraOutfitSlotSpecBodywear,
					EraOutfitSlotSpecOuterwear,
					EraOutfitSlotSpecHeadwear,
					EraOutfitSlotSpecBeltOrSash,
					EraOutfitSlotSpecWornContainer,
					EraOutfitSlotSpecFastenerOrJewellery,
					EraOutfitSlotSpecRoleItem
				]);
		}

		if (pieces.Length == 9 && IsMedievalOutfitFootwearPiece(pieces[2]))
		{
			return
			[
				(EraOutfitSlotSpecUnderlayer, pieces[0]),
				(EraOutfitSlotSpecLowerBody, pieces[1]),
				(EraOutfitSlotSpecLegOrSockLayer, pieces[1]),
				(EraOutfitSlotSpecFootwear, pieces[2]),
				(EraOutfitSlotSpecBodywear, pieces[3]),
				(EraOutfitSlotSpecOuterwear, pieces[4]),
				(EraOutfitSlotSpecHeadwear, pieces[5]),
				(EraOutfitSlotSpecBeltOrSash, pieces[6]),
				(EraOutfitSlotSpecWornContainer, pieces[7]),
				(EraOutfitSlotSpecFastenerOrJewellery, pieces[8]),
				(EraOutfitSlotSpecRoleItem, pieces[8])
			];
		}

		if (pieces.Length != 10)
		{
			throw new ApplicationException($"{socialClassRole} outfit rows require nine, ten, or eleven target pieces.");
		}

		if (IsMedievalOutfitFootwearPiece(pieces[2]))
		{
			return
			[
				(EraOutfitSlotSpecUnderlayer, pieces[0]),
				(EraOutfitSlotSpecLowerBody, pieces[1]),
				(EraOutfitSlotSpecLegOrSockLayer, pieces[1]),
				(EraOutfitSlotSpecFootwear, pieces[2]),
				(EraOutfitSlotSpecBodywear, pieces[3]),
				(EraOutfitSlotSpecOuterwear, pieces[4]),
				(EraOutfitSlotSpecHeadwear, pieces[5]),
				(EraOutfitSlotSpecBeltOrSash, pieces[6]),
				(EraOutfitSlotSpecWornContainer, pieces[7]),
				(EraOutfitSlotSpecFastenerOrJewellery, pieces[8]),
				(EraOutfitSlotSpecRoleItem, pieces[9])
			];
		}

		return
		[
			(EraOutfitSlotSpecUnderlayer, pieces[0]),
			(EraOutfitSlotSpecLowerBody, pieces[1]),
			(EraOutfitSlotSpecLegOrSockLayer, pieces[2]),
			(EraOutfitSlotSpecFootwear, pieces[3]),
			(EraOutfitSlotSpecBodywear, pieces[4]),
			(EraOutfitSlotSpecOuterwear, pieces[5]),
			(EraOutfitSlotSpecHeadwear, pieces[6]),
			(EraOutfitSlotSpecBeltOrSash, pieces[7]),
			(EraOutfitSlotSpecWornContainer, pieces[8]),
			(EraOutfitSlotSpecFastenerOrJewellery, pieces[9]),
			(EraOutfitSlotSpecRoleItem, pieces[9])
		];
	}

	private static IReadOnlyCollection<(string SlotKey, string PieceName)> AssignMedievalOutfitPieces(
		string[] pieces, string[] slotKeys)
	{
		return slotKeys
			.Select((slotKey, index) => (slotKey, pieces[index]))
			.ToArray();
	}

	private static bool MedievalOutfitRoleItemRequiredForRole(string socialClassRole)
	{
		return EraOutfitSlotSpecs
			.Single(x => x.Key.Equals(EraOutfitSlotSpecRoleItem, StringComparison.OrdinalIgnoreCase))
			.RequiredForRoles
			.Contains(socialClassRole, StringComparer.OrdinalIgnoreCase);
	}

	private static bool IsMedievalOutfitFootwearPiece(string pieceName)
	{
		var lower = pieceName.ToLowerInvariant();
		return lower.Contains("shoe", StringComparison.Ordinal) ||
		       lower.Contains("boot", StringComparison.Ordinal) ||
		       lower.Contains("sandal", StringComparison.Ordinal) ||
		       lower.Contains("slipper", StringComparison.Ordinal);
	}

	private static IReadOnlyList<MedievalBespokeOutfitPieceSpec> BuildMedievalBespokeOutfitPieces()
	{
		return
		[
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_male_peasant_tablet_banded_wool_tunic",
				"medieval_outfit_early_anglo_saxon_male_peasant",
				"early_anglo_saxon",
				"male",
				"peasant",
				"bodywear",
				"tablet-banded wool tunic",
				"tunic",
				"a $colour1 tablet-banded wool tunic",
				"This $colour1 knee-length wool tunic is cut simply, with side gores for work and a narrow tablet-woven band at the neck and cuffs. The cloth is sturdy rather than fine, and the coloured edging gives even a common outfit a recognisably island-hall finish. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				850.0,
				28.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(35.0, "wool", "Tablet-Woven Band Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_female_peasant_tablet_banded_wool_gown",
				"medieval_outfit_early_anglo_saxon_female_peasant",
				"early_anglo_saxon",
				"female",
				"peasant",
				"bodywear",
				"tablet-banded wool gown",
				"gown",
				"a $colour1 tablet-banded wool gown",
				"This $colour1 long wool gown hangs from the shoulders in a plain working line, with tablet-woven edging at the neck, sleeve ends, and hem. It is practical enough for household labour but has enough patterned trim to mark it as part of an island-hall wardrobe. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				980.0,
				32.0m,
				"Variable_2DrabColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(40.0, "wool", "Tablet-Woven Band Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_female_peasant_linen_head_veil",
				"medieval_outfit_early_anglo_saxon_female_peasant",
				"early_anglo_saxon",
				"female",
				"peasant",
				"headwear",
				"linen head veil",
				"veil",
				"a $colour linen head veil",
				"This $colour rectangular linen veil is hemmed for repeated pinning and washing. It drapes cleanly over the hair and neck, suited to village, hall, and church settings without the stiffness or ornament of a noble veil.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				180.0,
				10.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_male_artisan_seax_belt",
				"medieval_outfit_early_anglo_saxon_male_artisan",
				"early_anglo_saxon",
				"male",
				"artisan",
				"belt_or_sash",
				"seax belt",
				"belt",
				"a $colour seax belt",
				"This $colour leather belt is broad enough to carry a seax sheath and small work pouch. The strap is plain, but the buckle tongue and reinforced hanging points show it was made for daily wear rather than ceremony.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				420.0,
				30.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_4",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_male_merchant_bordered_tunic",
				"medieval_outfit_early_anglo_saxon_male_merchant",
				"early_anglo_saxon",
				"male",
				"merchant",
				"bodywear",
				"bordered tunic",
				"tunic",
				"a $colour1 bordered merchant tunic",
				"This $colour1 better wool tunic has a tidy fall and a woven border placed to show at the wrists, neck, and lower hem. The garment is conservative enough for market business but finished above ordinary village workwear. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				820.0,
				46.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_female_merchant_silver_brooch",
				"medieval_outfit_early_anglo_saxon_female_merchant",
				"early_anglo_saxon",
				"female",
				"merchant",
				"fastener_or_jewellery",
				"silver brooch",
				"brooch",
				"a silver brooch",
				"This silver brooch has a neat pin, a framed face, and shallow punched ornament around the rim. It is strong enough to close a mantle while still reading as a respectable piece of merchant-household display.",
				"silver",
				MaterialBehaviourType.Metal,
				ItemQuality.Good,
				SizeCategory.Small,
				50.0,
				90.0m,
				null,
				[],
				["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"],
				[CraftInput(120.0, "silver", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_female_noble_brooch_fastened_mantle",
				"medieval_outfit_early_anglo_saxon_female_noble",
				"early_anglo_saxon",
				"female",
				"noble",
				"outerwear",
				"brooch-fastened mantle",
				"mantle",
				"a $colour1 brooch-fastened mantle",
				"This $colour1 rich wool mantle is cut generously so it can be swept across the shoulders and fixed with a display brooch. The edge is embroidered, but the main cloth remains heavy enough for draughty halls and outdoor processions. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1550.0,
				95.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_male_religious_monastic_wool_habit",
				"medieval_outfit_early_anglo_saxon_male_religious",
				"early_anglo_saxon",
				"male",
				"religious",
				"bodywear",
				"monastic wool habit",
				"habit",
				"a $colour monastic wool habit",
				"This $colour undyed wool habit is deliberately plain, with a loose body, long sleeves, and a fall that hides the shape of the wearer. The seams are strong and spare, meant for prayer, copying, field work, and cold stone rooms.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1250.0,
				34.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_male_military_padded_shield_wall_tunic",
				"medieval_outfit_early_anglo_saxon_male_military",
				"early_anglo_saxon",
				"male",
				"military",
				"bodywear",
				"padded shield-wall tunic",
				"tunic",
				"a $colour padded shield-wall tunic",
				"This $colour quilted tunic is thickest through the chest and shoulders, where spear shafts, shield rims, and mail edges would bite. Tie points keep it close enough under a belt while leaving the arms free for shield-wall work.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1750.0,
				58.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_early_anglo_saxon_female_military_brooch_fastened_war_cloak",
				"medieval_outfit_early_anglo_saxon_female_military",
				"early_anglo_saxon",
				"female",
				"military",
				"outerwear",
				"brooch-fastened war cloak",
				"cloak",
				"a $colour brooch-fastened war cloak",
				"This $colour heavy cloak is short enough to clear the knees and broad enough to wrap around armour or padding. Reinforced corners take a brooch or pin, making it useful on watch, march, or in a timber hall.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1450.0,
				50.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_male_peasant_panelled_wool_tunic",
				"medieval_outfit_anglo_danish_male_peasant",
				"anglo_danish",
				"male",
				"peasant",
				"bodywear",
				"panelled wool tunic",
				"tunic",
				"a $colour1 panelled wool tunic",
				"This $colour1 wool tunic is made from visible panels with narrow seam lines, giving it a tougher, more structured look than a plain village shirt. The cut leaves room for wrapped trews and a belt pouch without dragging at the hips. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				860.0,
				28.0m,
				"Variable_2DrabColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_female_peasant_head_rail",
				"medieval_outfit_anglo_danish_female_peasant",
				"anglo_danish",
				"female",
				"peasant",
				"headwear",
				"head rail",
				"veil",
				"a $colour linen head rail",
				"This $colour long linen head rail can be wrapped close for work or left to fall over the shoulders in colder weather. Its edges are narrow-hemmed so a pin can hold it without tearing the cloth.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				200.0,
				11.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_male_artisan_narrow_braid_tunic",
				"medieval_outfit_anglo_danish_male_artisan",
				"anglo_danish",
				"male",
				"artisan",
				"bodywear",
				"narrow-braid tunic",
				"tunic",
				"a $colour1 narrow-braid tunic",
				"This $colour1 work tunic has braid set close to the collar and cuffs rather than broad courtly borders. The cloth is sturdy, the sleeves are practical, and the decorative restraint suits a skilled artisan or town worker. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				800.0,
				32.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(35.0, "wool", "Tablet-Woven Band Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_male_artisan_long_seax_belt",
				"medieval_outfit_anglo_danish_male_artisan",
				"anglo_danish",
				"male",
				"artisan",
				"belt_or_sash",
				"long seax belt",
				"belt",
				"a $colour long seax belt",
				"This $colour stout leather belt is cut to carry the weight of a long seax without sagging. It has reinforced suspension points, a plain buckle, and enough space for a small pouch or work knife.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				520.0,
				34.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_4",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_male_merchant_reeve_tally_pouch",
				"medieval_outfit_anglo_danish_male_merchant",
				"anglo_danish",
				"male",
				"merchant",
				"worn_container",
				"reeve tally pouch",
				"pouch",
				"a $colour reeve tally pouch",
				"This $colour closable leather pouch has an inner divider for tally sticks, cord tags, and folded writ scraps. It is worn on a belt and built for someone who must keep village, estate, or market accounts close to hand.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Good,
				SizeCategory.Small,
				260.0,
				42.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Container_Pouch",
					"Beltable",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(520.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_female_merchant_embroidered_collar_overgown",
				"medieval_outfit_anglo_danish_female_merchant",
				"anglo_danish",
				"female",
				"merchant",
				"bodywear",
				"embroidered collar overgown",
				"gown",
				"a $colour1 embroidered-collar overgown",
				"This $colour1 overgown is shaped from good wool and marked by embroidery concentrated around the collar. The rest of the garment is reserved and practical, suggesting a household with means but not courtly extravagance. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1050.0,
				64.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_male_noble_panelled_noble_tunic",
				"medieval_outfit_anglo_danish_male_noble",
				"anglo_danish",
				"male",
				"noble",
				"bodywear",
				"panelled noble tunic",
				"tunic",
				"a $colour1 panelled noble tunic",
				"This $colour1 noble tunic uses broad panels of fine wool to create a crisp, imposing line. The seams are intentionally visible and decorated, turning practical construction into a sign of rank and controlled wealth. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				900.0,
				72.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_female_noble_fur_edged_cloak",
				"medieval_outfit_anglo_danish_female_noble",
				"anglo_danish",
				"female",
				"noble",
				"outerwear",
				"fur-edged cloak",
				"cloak",
				"a $colour fur-edged cloak",
				"This $colour wool cloak is edged in a narrow band of fur, adding warmth and status without becoming ostentatious. The fastening point is reinforced for a silver brooch, making it suitable for a hall, court, or cold journey.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1650.0,
				100.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(140.0, "fur", "Fur Panel Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_male_military_nasal_cap_liner",
				"medieval_outfit_anglo_danish_male_military",
				"anglo_danish",
				"male",
				"military",
				"headwear",
				"nasal cap liner",
				"cap",
				"a $colour nasal-helm cap liner",
				"This $colour padded cap liner is shaped to sit beneath a nasal helm, with extra quilting at the brow and crown. It ties under the chin and keeps iron edges from chafing during long drill or watch duty.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				360.0,
				20.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_anglo_danish_female_military_padded_shield_wall_gown",
				"medieval_outfit_anglo_danish_female_military",
				"anglo_danish",
				"female",
				"military",
				"bodywear",
				"padded shield-wall gown",
				"gown",
				"a $colour padded shield-wall gown",
				"This $colour padded gown adapts shield-wall protection to a longer cut, with quilting over the torso and a split lower fall for movement. It is austere, hard-wearing, and made to sit under a belt and cloak.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1800.0,
				62.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_female_peasant_hangerok_apron_dress",
				"medieval_outfit_norse_female_peasant",
				"norse",
				"female",
				"peasant",
				"bodywear",
				"hangerok apron dress",
				"dress",
				"a $colour hangerok apron dress",
				"This $colour wool hangerok hangs from shoulder straps and falls over the underdress in a straight working line. The strap points are reinforced for oval brooches, and the front is plain enough for daily labour while still unmistakably northern sea-trading.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				980.0,
				42.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_female_peasant_oval_brooch_pair",
				"medieval_outfit_norse_female_peasant",
				"norse",
				"female",
				"peasant",
				"fastener_or_jewellery",
				"oval brooch pair",
				"brooch",
				"oval brooch pair",
				"This matched pair of oval brooches has raised decoration and strong pins for holding apron-dress straps. The backs are practical and worn smooth, while the fronts are meant to catch the eye at the chest.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Standard,
				SizeCategory.Small,
				95.0,
				55.0m,
				null,
				[],
				["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_male_peasant_sea_cloak",
				"medieval_outfit_norse_male_peasant",
				"norse",
				"male",
				"peasant",
				"outerwear",
				"sea cloak",
				"cloak",
				"a $colour sea cloak",
				"This $colour heavy wool cloak is cut to wrap tight against wind and spray. The edges are plain but tough, and the fastening point is reinforced for a pin or brooch that can be handled with cold fingers.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1600.0,
				48.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_male_merchant_trader_kaftan",
				"medieval_outfit_norse_male_merchant",
				"norse",
				"male",
				"merchant",
				"bodywear",
				"trader kaftan",
				"kaftan",
				"a $colour1 trader kaftan",
				"This $colour1 front-opening kaftan is made for travel and bargaining, with a wrap closure that sits neatly under a decorated belt. The cut hints at eastern trade fashions while remaining sturdy enough for a sea-road road or ship. A secondary $colour2 detail marks the visible fastening points and finish.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				980.0,
				68.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_male_merchant_runic_trade_tag",
				"medieval_outfit_norse_male_merchant",
				"norse",
				"male",
				"merchant",
				"role_item",
				"runic trade tag",
				"tag",
				"a runic trade tag",
				"This small wooden tag is notched and marked with short runic signs for ownership, cargo, or debt. It hangs from cord and is small enough to travel with a purse, bale, or sea chest.",
				"oak",
				MaterialBehaviourType.Wood,
				ItemQuality.Standard,
				SizeCategory.Small,
				80.0,
				12.0m,
				null,
				[],
				["Holdable", "Destroyable_Misc"],
				[CraftInput(80.0, "oak", "Tally Stick Stock")],
				["TagTool - Held - an item with the Awl Punch tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_female_merchant_bead_strung_hangerok",
				"medieval_outfit_norse_female_merchant",
				"norse",
				"female",
				"merchant",
				"bodywear",
				"bead-strung hangerok",
				"dress",
				"a $colour1 bead-strung hangerok",
				"This $colour1 apron dress uses better wool and reinforced brooch straps threaded with coloured bead strings. The garment is still practical, but the hanging beads and neat fall make it suitable for trade, hosting, or market display. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1020.0,
				78.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_male_noble_decorated_kaftan",
				"medieval_outfit_norse_male_noble",
				"norse",
				"male",
				"noble",
				"bodywear",
				"decorated kaftan",
				"kaftan",
				"a $colour1 decorated kaftan",
				"This $colour1 wool kaftan is cut with a confident front overlap and decorated edging. Its fittings and trim speak of wealth from trade or tribute, while the cloth remains heavy enough for travel between hall, ship, and market. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1100.0,
				92.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_female_noble_fur_lined_cloak",
				"medieval_outfit_norse_female_noble",
				"norse",
				"female",
				"noble",
				"outerwear",
				"fur-lined cloak",
				"cloak",
				"a $colour fur-lined cloak",
				"This $colour cloak has a heavy wool face and a warm fur lining visible at the turn of the edge. It is grand without being delicate, made for someone who expects to be seen outdoors as well as inside the hall.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1800.0,
				130.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(240.0, "fur", "Fur Panel Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_male_military_axe_loop",
				"medieval_outfit_norse_male_military",
				"norse",
				"male",
				"military",
				"role_item",
				"axe loop",
				"loop",
				"a $colour axe loop",
				"This $colour leather loop is made to hang a small axe from a belt or harness without fouling the wearer’s stride. The stitching is waxed and the riveted mouth is scuffed from repeated field use.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				180.0,
				18.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Beltable",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norse_female_military_arming_hangerok_or_tunic",
				"medieval_outfit_norse_female_military",
				"norse",
				"female",
				"military",
				"bodywear",
				"arming hangerok or tunic",
				"tunic",
				"a $colour arming hangerok-tunic",
				"This $colour quilted arming garment borrows the hanging line of an apron dress while keeping the torso close and padded. It is meant for weapon belts, field pouches, and rough movement rather than household display.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1650.0,
				56.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_male_merchant_long_sleeved_cote",
				"medieval_outfit_norman_male_merchant",
				"norman",
				"male",
				"merchant",
				"bodywear",
				"long-sleeved cote",
				"cote",
				"a $colour long-sleeved cote",
				"This $colour long-sleeved wool cote is fitted closely enough to look respectable without losing ease of movement. The seams are neat, the cuffs are tidy, and the fall is suited to a town, court office, or manor hall.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				900.0,
				56.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_female_merchant_bliaut_style_overgown",
				"medieval_outfit_norman_female_merchant",
				"norman",
				"female",
				"merchant",
				"bodywear",
				"bliaut-style overgown",
				"gown",
				"a $colour bliaut-style overgown",
				"This $colour overgown has a long, elegant fall and fitted upper sleeves that mark it as better town or household clothing. The cut nods toward court fashion without requiring the richest fabric.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1080.0,
				72.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_male_noble_split_riding_tunic",
				"medieval_outfit_norman_male_noble",
				"norman",
				"male",
				"noble",
				"bodywear",
				"split riding tunic",
				"tunic",
				"a $colour split riding tunic",
				"This $colour fine tunic is split for riding, with reinforced edges that sit cleanly over the saddle. The cloth is carefully finished, making it a garment for a mounted lord rather than a foot traveller.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				960.0,
				84.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_female_noble_court_bliaut",
				"medieval_outfit_norman_female_noble",
				"norman",
				"female",
				"noble",
				"bodywear",
				"court bliaut",
				"gown",
				"a $colour court bliaut",
				"This $colour court bliaut uses fine cloth and an elongated line to create a formal silhouette. The sleeves and body are cut for display in hall or chapel, with a finish too delicate for ordinary work.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				880.0,
				130.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_female_noble_wimple_and_veil",
				"medieval_outfit_norman_female_noble",
				"norman",
				"female",
				"noble",
				"headwear",
				"wimple and veil",
				"veil",
				"a $colour wimple and veil",
				"This $colour layered linen wimple covers the throat and frames the face beneath a fine veil. The set gives a formal, high-status line while remaining sober enough for court, chapel, or household ceremony.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				260.0,
				42.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_male_military_padded_aketon",
				"medieval_outfit_norman_male_military",
				"norman",
				"male",
				"military",
				"bodywear",
				"padded aketon",
				"aketon",
				"a $colour padded aketon",
				"This $colour quilted aketon is thick through the shoulders and chest, with tie points set to keep it close beneath mail or a surcoat. It is practical armour clothing, not a decorative court garment.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1850.0,
				64.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_male_military_mail_surcoat",
				"medieval_outfit_norman_male_military",
				"norman",
				"male",
				"military",
				"outerwear",
				"mail surcoat",
				"surcoat",
				"a $colour mail surcoat",
				"This $colour sleeveless surcoat is cut to fall over mail, breaking up the shine of iron and giving space for colour or household marks. The cloth is sturdy and lightly lined against the edges of armour.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				780.0,
				48.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_male_military_nasal_arming_coif",
				"medieval_outfit_norman_male_military",
				"norman",
				"male",
				"military",
				"headwear",
				"nasal arming coif",
				"coif",
				"a $colour nasal-helm arming coif",
				"This $colour padded coif ties under the chin and is shaped to sit beneath a nasal helm. The brow and crown are reinforced, while the sides are kept close so they do not interfere with a mail coif or helmet strap.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				420.0,
				26.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_male_military_scabbard_harness",
				"medieval_outfit_norman_male_military",
				"norman",
				"male",
				"military",
				"role_item",
				"scabbard harness",
				"harness",
				"a $colour scabbard harness",
				"This $colour leather harness carries a sword scabbard at the hip with a slight forward angle for mounted use. The straps are riveted and adjustable, built to sit over an arming belt without slipping.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				540.0,
				40.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Beltable",
					"Sheath_Large",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Scabbard Leather Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_norman_male_peasant_linen_coif",
				"medieval_outfit_norman_male_peasant",
				"norman",
				"male",
				"peasant",
				"headwear",
				"linen coif",
				"coif",
				"a $colour linen coif",
				"This $colour plain linen coif ties below the chin and keeps hair contained under a hood, cap, or work cloak. It is cheap, washable, and useful across village, kitchen, and stable work.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				110.0,
				8.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_male_peasant_wool_cote",
				"medieval_outfit_high_british_male_peasant",
				"high_british",
				"male",
				"peasant",
				"bodywear",
				"wool cote",
				"cote",
				"a $colour wool cote",
				"This $colour simple wool cote is cut for long use, with a straight body, modest sleeves, and enough length for work in field or town. Its plainness makes the belt, hood, and tools do most of the social talking.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				820.0,
				28.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_female_peasant_plain_gown",
				"medieval_outfit_high_british_female_peasant",
				"high_british",
				"female",
				"peasant",
				"bodywear",
				"plain gown",
				"gown",
				"a $colour plain wool gown",
				"This $colour plain gown is made from serviceable wool and shaped for work rather than display. The seams are strong, the skirt has enough room for movement, and the hem is high enough to survive mud and rushes.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1050.0,
				30.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_male_merchant_lined_cote",
				"medieval_outfit_high_british_male_merchant",
				"high_british",
				"male",
				"merchant",
				"bodywear",
				"lined cote",
				"cote",
				"a $colour lined cote",
				"This $colour lined cote has a cleaner fall and warmer body than common wool clothing. It suits a prosperous townsperson or manor official who wants respectable dress without noble extravagance.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1050.0,
				62.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_female_merchant_wimple",
				"medieval_outfit_high_british_female_merchant",
				"high_british",
				"female",
				"merchant",
				"headwear",
				"wimple",
				"wimple",
				"a $colour wimple",
				"This $colour linen wimple frames the face and covers the throat in the respectable style of a settled town or manor household. It is carefully hemmed and meant to sit neatly beneath a mantle or veil.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				220.0,
				34.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_male_noble_silk_trimmed_surcoat",
				"medieval_outfit_high_british_male_noble",
				"high_british",
				"male",
				"noble",
				"bodywear",
				"silk-trimmed surcoat",
				"surcoat",
				"a $colour1 silk-trimmed surcoat",
				"This $colour1 surcoat is cut long and clean, with silk trim used where it will show at the chest and hem. It is a garment for court, hall, or formal attendance, designed to display status without becoming fragile. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				840.0,
				110.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(70.0, "silk", "Silk Brocade Panel"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_female_noble_fur_mantle",
				"medieval_outfit_high_british_female_noble",
				"high_british",
				"female",
				"noble",
				"outerwear",
				"fur mantle",
				"mantle",
				"a $colour fur mantle",
				"This $colour mantle is lined and edged with fur, giving it weight, warmth, and public authority. It is made to sit over a court gown, fastened visibly with a brooch or clasp.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1700.0,
				135.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(220.0, "fur", "Fur Panel Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_male_religious_small_prayer_book",
				"medieval_outfit_high_british_male_religious",
				"high_british",
				"male",
				"religious",
				"role_item",
				"small prayer book",
				"book",
				"a small prayer book",
				"This small bound prayer book has stiff covers, sewn gatherings, and a plain protective wrap. It is sized for a pouch or chapel desk, with enough presence to mark the wearer as literate or religiously attached.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				420.0,
				85.0m,
				null,
				[],
				["Holdable", "Book_Small_40_Page", "Destroyable_Paper"],
				[
					CraftInput(90.0, "paper", "Paper Sheet Stock", colour: true, fineColour: true),
					CraftInput(70.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true),
					CraftInput(35.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - InRoom - an item with the Book Press tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_male_military_gambeson",
				"medieval_outfit_high_british_male_military",
				"high_british",
				"male",
				"military",
				"bodywear",
				"gambeson",
				"gambeson",
				"a $colour gambeson",
				"This $colour quilted gambeson is closely sewn through the body and shoulders, with enough stiffness to cushion blows and keep mail from biting. The cut allows a belt, arming points, and a surcoat to sit over it.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1900.0,
				68.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_male_military_archer_bracer",
				"medieval_outfit_high_british_male_military",
				"high_british",
				"male",
				"military",
				"role_item",
				"archer bracer",
				"bracer",
				"a $colour archer bracer",
				"This $colour leather bracer is shaped to protect the inside of the forearm from bowstring slap. The surface is smooth and darkened from use, with simple ties that can be adjusted over a sleeve.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				160.0,
				18.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Fingerless_Gloves",
					"Armour_BoiledLeather",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_high_british_female_military_fitted_gambeson",
				"medieval_outfit_high_british_female_military",
				"high_british",
				"female",
				"military",
				"bodywear",
				"fitted gambeson",
				"gambeson",
				"a $colour fitted gambeson",
				"This $colour fitted gambeson uses the same dense quilting as a soldier’s coat but is cut with a closer waist and longer fall. It is meant for riding, archery, or watch duty rather than courtly display.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1800.0,
				68.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_male_peasant_plain_leine_style_tunic",
				"medieval_outfit_gaelic_male_peasant",
				"gaelic",
				"male",
				"peasant",
				"bodywear",
				"plain leine-style tunic",
				"tunic",
				"a $colour plain léine-style tunic",
				"This $colour long linen tunic has the loose fall and easy sleeves of a léine-style garment. It is plain enough for pastoral work, but the length and belt line give it a clearly hill-and-hall silhouette.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				850.0,
				26.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_male_peasant_brat_mantle",
				"medieval_outfit_gaelic_male_peasant",
				"gaelic",
				"male",
				"peasant",
				"outerwear",
				"brat mantle",
				"brat",
				"a $colour brat mantle",
				"This $colour wool brat is a broad mantle that can be wrapped, belted, or thrown over the shoulder. Its weight makes it useful against rain and hill wind, while the ring pin gives it a strong cultural line.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1550.0,
				48.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_female_peasant_ring_pin",
				"medieval_outfit_gaelic_female_peasant",
				"gaelic",
				"female",
				"peasant",
				"fastener_or_jewellery",
				"ring pin",
				"pin",
				"a ring pin",
				"This bronze ring pin has a moving hoop and a long sharpened tongue for fastening a mantle or cloak. The form is practical, visible, and suited to both ordinary dress and formal gatherings.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Standard,
				SizeCategory.Small,
				45.0,
				22.0m,
				null,
				[],
				["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_male_merchant_lined_brat",
				"medieval_outfit_gaelic_male_merchant",
				"gaelic",
				"male",
				"merchant",
				"outerwear",
				"lined brat",
				"brat",
				"a $colour lined brat",
				"This $colour lined brat uses better wool than a hill cloak, with a firm edge that hangs cleanly when pinned. It is made for travel, bargaining, and guesting, where warmth and visible respectability both matter.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1650.0,
				76.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_male_noble_bardic_or_lordly_mantle",
				"medieval_outfit_gaelic_male_noble",
				"gaelic",
				"male",
				"noble",
				"outerwear",
				"bardic or lordly mantle",
				"mantle",
				"a $colour bardic lordly mantle",
				"This $colour mantle is wide, heavy, and deliberately dramatic, made to be drawn around the body or thrown back in hall. Its border is worked with fine stitching, suitable for a lord, poet, or honoured guest.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1720.0,
				105.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_female_noble_noble_overgown",
				"medieval_outfit_gaelic_female_noble",
				"gaelic",
				"female",
				"noble",
				"bodywear",
				"noble overgown",
				"gown",
				"a $colour noble overgown",
				"This $colour fine wool overgown is cut to sit beneath a mantle without losing its own shape. The neckline and cuffs carry restrained embroidery, making it rich but still tied to local dress rather than foreign court fashion.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1080.0,
				86.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_male_religious_note_board",
				"medieval_outfit_gaelic_male_religious",
				"gaelic",
				"male",
				"religious",
				"role_item",
				"note board",
				"board",
				"a monastic note board",
				"This small wooden note board is smoothed for temporary marks, teaching notes, or boundary memoranda. It is plain enough for a travelling cleric and tough enough to survive in a satchel.",
				"oak",
				MaterialBehaviourType.Wood,
				ItemQuality.Standard,
				SizeCategory.Small,
				260.0,
				14.0m,
				null,
				[],
				["Holdable", "Destroyable_Misc"],
				[CraftInput(220.0, "oak", "Furniture Panel Stock")],
				["TagTool - Held - an item with the Awl Punch tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_male_military_light_padded_coat",
				"medieval_outfit_gaelic_male_military",
				"gaelic",
				"male",
				"military",
				"bodywear",
				"light padded coat",
				"coat",
				"a $colour light padded coat",
				"This $colour light padded coat protects the torso without the bulk of heavier mail clothing. The cut is short enough for rough ground and fast movement, making it useful for hill fighting and border service.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1450.0,
				50.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_male_military_spear_carrier_belt",
				"medieval_outfit_gaelic_male_military",
				"gaelic",
				"male",
				"military",
				"belt_or_sash",
				"spear carrier belt",
				"belt",
				"a $colour spear carrier belt",
				"This $colour leather belt is reinforced to support a knife, pouch, and loops for carrying light spear gear. It is functional first, with only a little stamped decoration near the buckle.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				500.0,
				32.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_4",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_gaelic_female_military_war_brat",
				"medieval_outfit_gaelic_female_military",
				"gaelic",
				"female",
				"military",
				"outerwear",
				"war brat",
				"brat",
				"a $colour war brat",
				"This $colour war brat is shorter and tighter-woven than a ceremonial mantle, built to keep rain and wind off without tangling around weapons. Its pinning edge is reinforced for hard campaigning.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1450.0,
				52.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_peasant_high_belted_tunic",
				"medieval_outfit_carolingian_male_peasant",
				"carolingian",
				"male",
				"peasant",
				"bodywear",
				"high-belted tunic",
				"tunic",
				"a $colour high-belted tunic",
				"This $colour wool tunic is cut to sit high under a belt, creating the compact line common to early palace-estate dress. It is plain, durable, and well suited to field work or estate service.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				820.0,
				28.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_peasant_broad_banded_mantle",
				"medieval_outfit_carolingian_male_peasant",
				"carolingian",
				"male",
				"peasant",
				"outerwear",
				"broad-banded mantle",
				"mantle",
				"a $colour1 broad-banded mantle",
				"This $colour1 wool mantle has a broad decorative band that stands out against otherwise practical cloth. The band gives even a modest outfit a recognisably palace-estate shape without making it luxurious. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1500.0,
				48.0m,
				"Variable_2DrabColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_merchant_capitulary_estate_list_pouch",
				"medieval_outfit_carolingian_male_merchant",
				"carolingian",
				"male",
				"merchant",
				"worn_container",
				"capitulary estate-list pouch",
				"pouch",
				"a $colour1 capitulary estate-list pouch",
				"This $colour1 leather pouch is long enough for folded estate notes, tally slips, and small wax tablets. It marks the wearer as someone concerned with administration rather than simple trade alone. A secondary $colour2 detail marks the visible fastening points and finish.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Good,
				SizeCategory.Small,
				280.0,
				44.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Waist",
					"Container_Pouch",
					"Beltable",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(520.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_noble_spatha_belt",
				"medieval_outfit_carolingian_male_noble",
				"carolingian",
				"male",
				"noble",
				"belt_or_sash",
				"spatha belt",
				"belt",
				"a $colour spatha belt",
				"This $colour strong leather belt is built to carry a spatha and visible fittings. Its mounted plates are polished but not delicate, reflecting a courtly warrior’s need for both display and function.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Good,
				SizeCategory.Small,
				620.0,
				70.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_6",
					"Armour_BoiledLeather",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(90.0, "bronze", "Tool Blank Stock"),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_noble_noble_fibula",
				"medieval_outfit_carolingian_male_noble",
				"carolingian",
				"male",
				"noble",
				"fastener_or_jewellery",
				"noble fibula",
				"fibula",
				"a noble fibula",
				"This fibula has a bright metal face and a strong fastening pin for a mantle or cloak. Its shape is formal and old-fashioned, suitable for a palace household or high estate retinue.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Good,
				SizeCategory.Small,
				60.0,
				75.0m,
				null,
				[],
				["Holdable", "Wear_Shoulder", "Destroyable_HeavyMetal"],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_religious_clerical_dalmatic_style_robe",
				"medieval_outfit_carolingian_male_religious",
				"carolingian",
				"male",
				"religious",
				"bodywear",
				"clerical dalmatic-style robe",
				"robe",
				"a $colour clerical dalmatic-style robe",
				"This $colour robe has the broad, ordered fall of church clothing without rich ornament. The sleeves are generous, the body is plain, and the whole garment is made to be read as clerical from a distance.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1200.0,
				44.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_religious_manuscript_leaf",
				"medieval_outfit_carolingian_male_religious",
				"carolingian",
				"male",
				"religious",
				"role_item",
				"manuscript leaf",
				"leaf",
				"a manuscript leaf",
				"This loose manuscript leaf is ruled and prepared for a careful hand, with enough margin for correction or gloss. It can serve as a teaching sample, devotional text, or administrative copy.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				25.0,
				12.0m,
				null,
				[],
				["Holdable", "PaperSheet_Scroll", "Destroyable_Paper"],
				[CraftInput(35.0, "paper", "Paper Sheet Stock")],
				["TagTool - Held - an item with the Quill Pen tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_military_padded_war_tunic",
				"medieval_outfit_carolingian_male_military",
				"carolingian",
				"male",
				"military",
				"bodywear",
				"padded war tunic",
				"tunic",
				"a $colour padded war tunic",
				"This $colour quilted tunic gives basic protection beneath mail or a heavy belt. The padding is practical rather than bulky, allowing formation movement and long marching without swallowing the wearer.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1650.0,
				54.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_male_military_riding_spurs",
				"medieval_outfit_carolingian_male_military",
				"carolingian",
				"male",
				"military",
				"role_item",
				"riding spurs",
				"spurs",
				"riding spurs",
				"These bronze spurs are short, practical, and strapped for a mounted retainer. The metal is plain but polished, showing the wearer’s association with horses, rank, and military service.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Good,
				SizeCategory.Small,
				180.0,
				65.0m,
				null,
				[],
				["Holdable", "Wear_Shoes", "Destroyable_HeavyMetal"],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_carolingian_female_noble_embroidered_gown",
				"medieval_outfit_carolingian_female_noble",
				"carolingian",
				"female",
				"noble",
				"bodywear",
				"embroidered gown",
				"gown",
				"a $colour1 embroidered gown",
				"This $colour1 fine wool gown is embroidered at the neckline and sleeve ends, giving it a formal palace-household appearance. The garment remains modest in shape but unmistakably high in status. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1050.0,
				88.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_male_peasant_plain_wool_cote",
				"medieval_outfit_capetian_male_peasant",
				"capetian",
				"male",
				"peasant",
				"bodywear",
				"plain wool cote",
				"cote",
				"a $colour plain wool cote",
				"This $colour wool cote is plainly cut with a practical fall, suited to village work or low town service. The garment is intentionally unornamented, relying on a belt and coif for its finished shape.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				820.0,
				28.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_female_peasant_wool_kirtle",
				"medieval_outfit_capetian_female_peasant",
				"capetian",
				"female",
				"peasant",
				"bodywear",
				"wool kirtle",
				"kirtle",
				"a $colour wool kirtle",
				"This $colour simple wool kirtle is shaped for work, with enough skirt to move and enough firmness to sit neatly under a cloak. It is plain, useful, and recognisably part of a settled high-medieval wardrobe.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				980.0,
				32.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_male_artisan_guild_work_cote",
				"medieval_outfit_capetian_male_artisan",
				"capetian",
				"male",
				"artisan",
				"bodywear",
				"guild work cote",
				"cote",
				"a $colour guild work cote",
				"This $colour work cote is cut for an artisan who needs respectable but durable clothing. The elbows and side seams are strengthened, and the cloth is tidy enough for a shopfront or guildhall.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				880.0,
				38.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_male_artisan_guild_apron",
				"medieval_outfit_capetian_male_artisan",
				"capetian",
				"male",
				"artisan",
				"outerwear",
				"guild apron",
				"apron",
				"a $colour guild apron",
				"This $colour apron is made from heavy linen, with a reinforced front and long ties that wrap securely around the body. It is both protection and a visible sign of workshop identity.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				520.0,
				24.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Apron",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_male_merchant_lined_burgher_gown",
				"medieval_outfit_capetian_male_merchant",
				"capetian",
				"male",
				"merchant",
				"bodywear",
				"lined burgher gown",
				"gown",
				"a $colour lined burgher gown",
				"This $colour lined gown has a sober, prosperous cut suited to a town merchant or office holder. Its better wool, clean front, and careful lining make it respectable without entering noble extravagance.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1120.0,
				76.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_female_merchant_wimple",
				"medieval_outfit_capetian_female_merchant",
				"capetian",
				"female",
				"merchant",
				"headwear",
				"wimple",
				"wimple",
				"a $colour wimple",
				"This $colour linen wimple is shaped to frame the face and cover the throat in a respectable urban style. The cloth is fine enough for a merchant household but not heavy with court ornament.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				230.0,
				34.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_female_noble_bliaut_style_gown",
				"medieval_outfit_capetian_female_noble",
				"capetian",
				"female",
				"noble",
				"bodywear",
				"bliaut-style gown",
				"gown",
				"a $colour bliaut-style gown",
				"This $colour gown has the long fall and refined upper shape associated with courtly dress. Its fabric is carefully finished, and the sleeves are made to show status rather than withstand heavy labour.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				900.0,
				128.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_male_religious_chapel_book",
				"medieval_outfit_capetian_male_religious",
				"capetian",
				"male",
				"religious",
				"role_item",
				"chapel book",
				"book",
				"a chapel book",
				"This small bound chapel book has sewn gatherings and plain boards for repeated devotional use. It is modest enough for daily office but substantial enough to be carried as a religious role marker.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				480.0,
				85.0m,
				null,
				[],
				["Holdable", "Book_Small_40_Page", "Destroyable_Paper"],
				[
					CraftInput(90.0, "paper", "Paper Sheet Stock", colour: true, fineColour: true),
					CraftInput(70.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true),
					CraftInput(35.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - InRoom - an item with the Book Press tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_male_military_padded_aketon",
				"medieval_outfit_capetian_male_military",
				"capetian",
				"male",
				"military",
				"bodywear",
				"padded aketon",
				"aketon",
				"a $colour padded aketon",
				"This $colour aketon is densely quilted through the torso, with a firm shoulder line for supporting mail or a surcoat. It is built for militia, retinue, or field service rather than pageantry.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1800.0,
				62.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_capetian_female_military_surcoat",
				"medieval_outfit_capetian_female_military",
				"capetian",
				"female",
				"military",
				"outerwear",
				"surcoat",
				"surcoat",
				"a $colour military surcoat",
				"This $colour surcoat is cut to cover padded armour while leaving the arms free. The cloth is durable, lightly lined, and suitable for household colours, town militia marks, or campaign wear.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				760.0,
				42.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_peasant_alpine_felt_cap",
				"medieval_outfit_german_hre_male_peasant",
				"german_hre",
				"male",
				"peasant",
				"headwear",
				"alpine felt cap",
				"cap",
				"a $colour alpine felt cap",
				"This $colour felt cap is compact, warm, and shaped for upland weather. The brim is modest and the crown is firm enough to hold its shape after being stuffed into a pouch or under a cloak.",
				"felt",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				180.0,
				18.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "felt", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_artisan_guild_apron_over_tunic",
				"medieval_outfit_german_hre_male_artisan",
				"german_hre",
				"male",
				"artisan",
				"bodywear",
				"guild apron over tunic",
				"apron",
				"a $colour guild apron over tunic",
				"This $colour heavy apron is meant to be worn over a fitted tunic, with a broad front that protects against bench work, dust, and sparks. Its cut and ties make the wearer read immediately as a guild worker.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				650.0,
				34.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Apron",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_merchant_civic_gown",
				"medieval_outfit_german_hre_male_merchant",
				"german_hre",
				"male",
				"merchant",
				"bodywear",
				"civic gown",
				"gown",
				"a $colour civic gown",
				"This $colour civic gown has a long, formal fall and a sober shape suited to town office, guild business, or a prosperous household. Its better cloth is visible in the smooth drape rather than loud ornament.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1160.0,
				82.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_merchant_fur_lined_mantle",
				"medieval_outfit_german_hre_male_merchant",
				"german_hre",
				"male",
				"merchant",
				"outerwear",
				"fur-lined mantle",
				"mantle",
				"a $colour fur-lined mantle",
				"This $colour mantle is lined with fur and cut to sit squarely across the shoulders, giving warmth and civic dignity. It is a garment for council rooms, market travel, and winter streets.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1750.0,
				130.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(220.0, "fur", "Fur Panel Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_noble_belt_mounts",
				"medieval_outfit_german_hre_male_noble",
				"german_hre",
				"male",
				"noble",
				"role_item",
				"belt mounts",
				"mounts",
				"belt mounts",
				"These polished belt mounts are shaped to rivet along a court belt or formal harness. The pieces are small, bright, and regular, designed to turn an ordinary belt into an object of status.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Good,
				SizeCategory.Small,
				90.0,
				75.0m,
				null,
				[],
				[
					"Holdable",
					"Wear_Waist",
					"Beltable",
					"Destroyable_HeavyMetal",
				],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_female_noble_embroidered_overgown",
				"medieval_outfit_german_hre_female_noble",
				"german_hre",
				"female",
				"noble",
				"bodywear",
				"embroidered overgown",
				"gown",
				"a $colour1 embroidered overgown",
				"This $colour1 overgown uses good wool and controlled embroidery rather than loose display. The fitted upper body and decorated edges make it suitable for court, church, and formal urban households. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1080.0,
				92.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_religious_church_robe",
				"medieval_outfit_german_hre_male_religious",
				"german_hre",
				"male",
				"religious",
				"bodywear",
				"church robe",
				"robe",
				"a $colour church robe",
				"This $colour church robe is plain in colour but carefully made, with a long body and sleeves suited to ritual and daily office. It is sober enough for discipline and formal enough for public worship.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1250.0,
				46.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_military_arming_jack",
				"medieval_outfit_german_hre_male_military",
				"german_hre",
				"male",
				"military",
				"bodywear",
				"arming jack",
				"jack",
				"a $colour arming jack",
				"This $colour arming jack is quilted and fitted for town or field service, with a practical length and reinforced points for gear. It is less courtly than a surcoat and more protective than ordinary clothing.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1750.0,
				60.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_male_military_town_crossbow_militia_hook",
				"medieval_outfit_german_hre_male_military",
				"german_hre",
				"male",
				"military",
				"role_item",
				"town crossbow militia hook",
				"hook",
				"a town crossbow militia hook",
				"This metal belt hook is made to help carry or brace crossbow gear for a town militia fighter. The hook is plain but sturdy, with rivet holes for fixing it to an arming belt.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Standard,
				SizeCategory.Small,
				180.0,
				38.0m,
				null,
				[],
				[
					"Holdable",
					"Wear_Waist",
					"Beltable",
					"Destroyable_HeavyMetal",
				],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_german_hre_female_military_arming_jack_gown",
				"medieval_outfit_german_hre_female_military",
				"german_hre",
				"female",
				"military",
				"bodywear",
				"arming jack gown",
				"gown",
				"a $colour arming jack gown",
				"This $colour quilted arming gown adapts the protective jack into a longer, more fitted cut. It keeps padding over the torso while allowing the lower body to move under a belt, pouch, and short mantle.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1720.0,
				60.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_male_peasant_simple_saya",
				"medieval_outfit_iberian_christian_male_peasant",
				"iberian_christian",
				"male",
				"peasant",
				"bodywear",
				"simple saya",
				"saya",
				"a $colour simple saya",
				"This $colour simple saya is cut from serviceable wool with narrow sleeves and an easy fall. It is practical rural clothing, but its shape gives an frontier surface distinct from northern cotes and tunics.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				850.0,
				30.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_female_peasant_toca_head_veil",
				"medieval_outfit_iberian_christian_female_peasant",
				"iberian_christian",
				"female",
				"peasant",
				"headwear",
				"toca head veil",
				"veil",
				"a $colour toca head veil",
				"This $colour linen toca wraps and frames the head with a clean, modest line. It is suited to work, church, or market wear, and can be pinned more formally for a better household.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				210.0,
				18.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_male_merchant_pellote_over_tunic",
				"medieval_outfit_iberian_christian_male_merchant",
				"iberian_christian",
				"male",
				"merchant",
				"bodywear",
				"pellote over tunic",
				"pellote",
				"a $colour pellote over-tunic",
				"This $colour pellote is worn over a tunic, with open sides and a clean front that show the quality of the clothing beneath. It is a strong town and frontier status garment without being purely courtly.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				920.0,
				70.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_female_merchant_lined_manto",
				"medieval_outfit_iberian_christian_female_merchant",
				"iberian_christian",
				"female",
				"merchant",
				"outerwear",
				"lined manto",
				"manto",
				"a $colour lined manto",
				"This $colour lined manto is made from warm wool with a smooth inner face and a reinforced fastening edge. It gives a merchant household outfit a public, travel-ready finish.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1500.0,
				80.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_male_noble_silk_trimmed_saya",
				"medieval_outfit_iberian_christian_male_noble",
				"iberian_christian",
				"male",
				"noble",
				"bodywear",
				"silk-trimmed saya",
				"saya",
				"a $colour1 silk-trimmed saya",
				"This $colour1 saya uses fine wool and visible silk trim at the neck, cuffs, and hem. The garment is restrained enough for a frontier court but visibly beyond ordinary town clothing. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				900.0,
				105.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(70.0, "silk", "Silk Brocade Panel"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_female_noble_silk_pellote",
				"medieval_outfit_iberian_christian_female_noble",
				"iberian_christian",
				"female",
				"noble",
				"bodywear",
				"silk pellote",
				"pellote",
				"a $colour1 silk pellote",
				"This $colour1 pellote is made from fine silk-faced cloth and cut to frame the gown beneath it. The side openings, smooth drape, and bright trim make it a clear court garment. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				760.0,
				140.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_male_religious_pilgrim_cloak",
				"medieval_outfit_iberian_christian_male_religious",
				"iberian_christian",
				"male",
				"religious",
				"outerwear",
				"pilgrim cloak",
				"cloak",
				"a $colour pilgrim cloak",
				"This $colour cloak is plain, road-worn, and cut to cover both body and satchel in bad weather. Its simple fastening and sturdy hem suit pilgrimage, parish travel, and frontier chaplaincy.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1450.0,
				44.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_male_military_quilted_coat",
				"medieval_outfit_iberian_christian_male_military",
				"iberian_christian",
				"male",
				"military",
				"bodywear",
				"quilted coat",
				"coat",
				"a $colour quilted coat",
				"This $colour quilted coat is made for frontier service, with dense padding over the torso and shoulders. It sits well beneath a surcoat or cloak and can be worn on foot or horseback.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1750.0,
				60.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_female_military_frontier_riding_cloak",
				"medieval_outfit_iberian_christian_female_military",
				"iberian_christian",
				"female",
				"military",
				"outerwear",
				"frontier riding cloak",
				"cloak",
				"a $colour frontier riding cloak",
				"This $colour riding cloak is cut shorter in front and fuller behind, helping it fall over a saddle without trapping the legs. The cloth is tough and weather-ready, with little decoration beyond a strong clasp point.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1500.0,
				58.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_iberian_christian_male_merchant_belt_mount",
				"medieval_outfit_iberian_christian_male_merchant",
				"iberian_christian",
				"male",
				"merchant",
				"fastener_or_jewellery",
				"belt mount",
				"mount",
				"a belt mount",
				"This small bronze belt mount has rivet holes and a polished face, adding status to a merchant or frontier official’s belt. Its shape is neat rather than ostentatious, made for repeated public wear.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Good,
				SizeCategory.Small,
				35.0,
				35.0m,
				null,
				[],
				[
					"Holdable",
					"Wear_Waist",
					"Beltable",
					"Destroyable_HeavyMetal",
				],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_peasant_linen_qamis",
				"medieval_outfit_andalusi_male_peasant",
				"andalusi",
				"male",
				"peasant",
				"underlayer",
				"linen qamis",
				"qamis",
				"a $colour linen qamis",
				"This $colour qamis is made from light linen with a loose body and simple sleeves for heat and movement. It forms the clean base of the outfit, worn beneath robes, burnous, or work layers.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				620.0,
				24.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Long-Sleeved_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_peasant_wool_sirwal",
				"medieval_outfit_andalusi_male_peasant",
				"andalusi",
				"male",
				"peasant",
				"lower_body",
				"wool sirwal",
				"sirwal",
				"$colour wool sirwal",
				"These $colour loose sirwal are gathered for comfort and movement, giving the lower body a very different line from northern hose. The wool is light enough for daily wear and broad enough for kneeling, riding, or work.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				520.0,
				24.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Chausses",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_peasant_light_burnous",
				"medieval_outfit_andalusi_male_peasant",
				"andalusi",
				"male",
				"peasant",
				"outerwear",
				"light burnous",
				"burnous",
				"a $colour light burnous",
				"This $colour light burnous hangs from the shoulders with a hooded fall that shields against sun, dust, and cool nights. Its plain cloth marks it as common wear, but the silhouette is unmistakably western courtly urban.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1150.0,
				38.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_peasant_simple_turban",
				"medieval_outfit_andalusi_male_peasant",
				"andalusi",
				"male",
				"peasant",
				"headwear",
				"simple turban",
				"turban",
				"a $colour simple turban",
				"This $colour plain turban is made from a long strip of linen, wrapped to protect the head and frame the face. It is serviceable, washable, and suitable for work, prayer, or travel.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				240.0,
				16.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_merchant_qaba_caftan",
				"medieval_outfit_andalusi_male_merchant",
				"andalusi",
				"male",
				"merchant",
				"bodywear",
				"qaba caftan",
				"caftan",
				"a $colour1 qabā’ caftan",
				"This $colour1 front-closing caftan is made from better cloth and shaped to sit smoothly over a qamis and sirwal. Its trim is light but deliberate, giving a merchant or scholar a polished urban line. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				980.0,
				76.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_female_merchant_tiraz_banded_robe",
				"medieval_outfit_andalusi_female_merchant",
				"andalusi",
				"female",
				"merchant",
				"bodywear",
				"tiraz-banded robe",
				"robe",
				"a $colour1 tiraz-banded robe",
				"This $colour1 robe is defined by a band of worked inscription-like trim at the sleeve or upper arm. The cloth is fine, the body modest, and the decoration immediately signals urban wealth and courtly urban textile taste. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				980.0,
				90.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_noble_rich_burnous",
				"medieval_outfit_andalusi_male_noble",
				"andalusi",
				"male",
				"noble",
				"outerwear",
				"rich burnous",
				"burnous",
				"a $colour1 rich burnous",
				"This $colour1 burnous uses finer wool and a fuller hooded fall than common travel cloaks. Its edge is lightly embroidered, making it suitable for courtly riding, public attendance, or formal receiving rooms. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1500.0,
				105.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_religious_scholar_robe",
				"medieval_outfit_andalusi_male_religious",
				"andalusi",
				"male",
				"religious",
				"bodywear",
				"scholar robe",
				"robe",
				"a $colour scholar robe",
				"This $colour sober robe is made for study, teaching, and public dignity rather than labour. The sleeves are roomy enough for writing and the fall is deliberately plain, letting the turban and book pouch define the role.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				900.0,
				44.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_male_military_bowcase_belt",
				"medieval_outfit_andalusi_male_military",
				"andalusi",
				"male",
				"military",
				"belt_or_sash",
				"bowcase belt",
				"belt",
				"a $colour bowcase belt",
				"This $colour leather belt is reinforced for a bowcase or quiver attachment, with a broad body and secure hanging points. It is made for mounted service and fast handling rather than town display.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				650.0,
				42.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_6",
					"Beltable",
					"Armour_BoiledLeather",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_andalusi_female_military_riding_burnous",
				"medieval_outfit_andalusi_female_military",
				"andalusi",
				"female",
				"military",
				"outerwear",
				"riding burnous",
				"burnous",
				"a $colour riding burnous",
				"This $colour riding burnous is shorter and more controlled than a town cloak, with a hood and reinforced edge for travel. It is built to move with a saddle and weapon belt without swallowing the wearer.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1350.0,
				58.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "wool", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_peasant_short_sagion_cloak",
				"medieval_outfit_byzantine_male_peasant",
				"byzantine",
				"male",
				"peasant",
				"outerwear",
				"short sagion cloak",
				"sagion",
				"a $colour short sagion cloak",
				"This $colour short sagion is a practical cloak with a clean military-urban line, fastened high and kept clear of the knees. The cloth is plain, but the silhouette gives the outfit an eastern imperial character.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1200.0,
				40.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_merchant_belted_skaramangion_robe",
				"medieval_outfit_byzantine_male_merchant",
				"byzantine",
				"male",
				"merchant",
				"bodywear",
				"belted skaramangion robe",
				"robe",
				"a $colour belted skaramangion robe",
				"This $colour robe is cut for a polished urban silhouette, wrapping neatly under a belt and falling with controlled folds. It is suitable for a merchant, official, or court-adjacent household.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				980.0,
				80.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_merchant_lined_sagion",
				"medieval_outfit_byzantine_male_merchant",
				"byzantine",
				"male",
				"merchant",
				"outerwear",
				"lined sagion",
				"sagion",
				"a $colour lined sagion",
				"This $colour lined sagion has a compact cloak shape and a smoother inner face, making it warm without bulk. The garment is formal enough for business and practical enough for city travel.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1450.0,
				78.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_noble_silk_dalmatic",
				"medieval_outfit_byzantine_male_noble",
				"byzantine",
				"male",
				"noble",
				"bodywear",
				"silk dalmatic",
				"dalmatic",
				"a $colour1 silk dalmatic",
				"This $colour1 silk dalmatic has a broad formal fall, with worked bands placed to frame the chest and sleeves. It reads as courtly and ceremonial, made for visibility in palace, church, or formal reception. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				760.0,
				160.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_noble_court_sagion",
				"medieval_outfit_byzantine_male_noble",
				"byzantine",
				"male",
				"noble",
				"outerwear",
				"court sagion",
				"sagion",
				"a $colour court sagion",
				"This $colour court sagion is made from fine cloth and fastened to display the upper body rather than hide it. Its edges are carefully finished, giving the wearer a controlled imperial silhouette.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1100.0,
				130.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_female_noble_icon_pouch",
				"medieval_outfit_byzantine_female_noble",
				"byzantine",
				"female",
				"noble",
				"role_item",
				"icon pouch",
				"pouch",
				"a $colour1 icon pouch",
				"This $colour1 small pouch is sized for a tiny icon, prayer slip, or folded devotional cloth. Its embroidered face and careful closure make it devotional, personal, and suitable for a high-status outfit. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				120.0,
				70.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Waist",
					"Container_Pouch",
					"Beltable",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_religious_icon_tablet",
				"medieval_outfit_byzantine_male_religious",
				"byzantine",
				"male",
				"religious",
				"role_item",
				"icon tablet",
				"tablet",
				"an icon tablet",
				"This small wooden tablet has a smoothed face for a painted holy image or devotional inscription. It is portable enough for a pouch but substantial enough to serve as a visible religious role item.",
				"oak",
				MaterialBehaviourType.Wood,
				ItemQuality.Standard,
				SizeCategory.Small,
				220.0,
				26.0m,
				null,
				[],
				["Holdable", "Destroyable_Misc"],
				[CraftInput(220.0, "oak", "Icon Panel Stock")],
				["TagTool - Held - an item with the Hammer tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_military_military_padded_tunic",
				"medieval_outfit_byzantine_male_military",
				"byzantine",
				"male",
				"military",
				"bodywear",
				"military padded tunic",
				"tunic",
				"a $colour military padded tunic",
				"This $colour padded tunic is cut close enough to sit under lamellar or a belt without bunching. The quilting is dense at the torso and shoulders, reflecting a practical campaign garment.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1650.0,
				58.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_male_military_lamellar_coat_cover",
				"medieval_outfit_byzantine_male_military",
				"byzantine",
				"male",
				"military",
				"outerwear",
				"lamellar coat cover",
				"cover",
				"a $colour lamellar coat cover",
				"This $colour outer cover is made to sit over or around lamellar equipment, protecting straps and giving the armour a more ordered appearance. It carries tie points and reinforced seams rather than loose ornament.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				900.0,
				55.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(420.0, "wrought iron", "Armour Lamella Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_byzantine_female_military_head_veil_under_cap",
				"medieval_outfit_byzantine_female_military",
				"byzantine",
				"female",
				"military",
				"headwear",
				"head veil under cap",
				"veil",
				"a $colour head veil for an arming cap",
				"This $colour linen veil is cut short and controlled so it can sit beneath a padded cap or helmet liner. It keeps the hair contained without adding bulk under military headgear.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				160.0,
				14.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_peasant_linen_qamis",
				"medieval_outfit_abbasid_male_peasant",
				"abbasid",
				"male",
				"peasant",
				"underlayer",
				"linen qamis",
				"qamis",
				"a $colour linen qamis",
				"This $colour loose linen qamis is the clean foundation of the outfit, light enough for heat and long enough for modest daily wear. Its construction is simple, with strength in the seams rather than decoration.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				620.0,
				24.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Long-Sleeved_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_peasant_wool_sirwal",
				"medieval_outfit_abbasid_male_peasant",
				"abbasid",
				"male",
				"peasant",
				"lower_body",
				"wool sirwal",
				"sirwal",
				"$colour wool sirwal",
				"These $colour sirwal are loose, gathered, and practical, allowing seated work, prayer, and travel without the constriction of hose. The wool is modest and hard-wearing.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				520.0,
				24.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Chausses",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_merchant_qaba_caftan",
				"medieval_outfit_abbasid_male_merchant",
				"abbasid",
				"male",
				"merchant",
				"bodywear",
				"qaba caftan",
				"caftan",
				"a $colour1 qaba caftan",
				"This $colour1 front-closing caftan is cut from better cloth and designed to sit cleanly over qamis and sirwal. Its wrapped front and trim make it suitable for market, court office, or learned company. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				980.0,
				78.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_religious_scholar_robe",
				"medieval_outfit_abbasid_male_religious",
				"abbasid",
				"male",
				"religious",
				"bodywear",
				"scholar robe",
				"robe",
				"a $colour scholar robe",
				"This $colour scholar robe is plain but deliberate, with generous sleeves and a controlled fall. It is made for study, teaching, and debate, where dignity matters more than bright ornament.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				900.0,
				44.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_religious_notebook",
				"medieval_outfit_abbasid_male_religious",
				"abbasid",
				"male",
				"religious",
				"role_item",
				"notebook",
				"book",
				"a scholar notebook",
				"This small paper notebook is bound for repeated notes, excerpts, and lessons. Its covers are plain, but the quires are carefully sewn and sized for a scholar’s pouch or desk.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				420.0,
				80.0m,
				null,
				[],
				["Holdable", "Book_Small_40_Page", "Destroyable_Paper"],
				[
					CraftInput(90.0, "paper", "Paper Sheet Stock", colour: true, fineColour: true),
					CraftInput(70.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true),
					CraftInput(35.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - InRoom - an item with the Book Press tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_noble_belted_court_robe",
				"medieval_outfit_abbasid_male_noble",
				"abbasid",
				"male",
				"noble",
				"bodywear",
				"belted court robe",
				"robe",
				"a $colour belted court robe",
				"This $colour court robe has a smooth, belted fall and a fine finish at the cuffs and front. It is made to show control, wealth, and urban refinement rather than martial bulk.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				920.0,
				145.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_female_noble_rich_mantle",
				"medieval_outfit_abbasid_female_noble",
				"abbasid",
				"female",
				"noble",
				"outerwear",
				"rich mantle",
				"mantle",
				"a $colour1 rich mantle",
				"This $colour1 mantle uses fine cloth with a soft drape and decorated edge, suited to high-status indoor and courtyard settings. It frames the robe beneath without overwhelming the veil and sash. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1050.0,
				135.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_military_lamellar_sleeved_coat",
				"medieval_outfit_abbasid_male_military",
				"abbasid",
				"male",
				"military",
				"bodywear",
				"lamellar-sleeved coat",
				"coat",
				"a $colour lamellar-sleeved coat",
				"This $colour coat has padded cloth sleeves and a body prepared to work with lamellar reinforcement. The garment is made for mounted or guard service, balancing movement with visible protection.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1700.0,
				70.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(420.0, "wrought iron", "Armour Lamella Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_male_military_weapon_sash",
				"medieval_outfit_abbasid_male_military",
				"abbasid",
				"male",
				"military",
				"belt_or_sash",
				"weapon sash",
				"sash",
				"a $colour weapon sash",
				"This $colour broad sash wraps firmly around the waist to stabilise a knife, pouch, or light sidearm. The cloth is strong and waxed at stress points, made for service rather than court display.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				360.0,
				28.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_4",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_abbasid_female_merchant_scent_flask",
				"medieval_outfit_abbasid_female_merchant",
				"abbasid",
				"female",
				"merchant",
				"role_item",
				"scent flask",
				"flask",
				"a scent flask",
				"This small scent flask is a personal luxury, stoppered and corded for wear or pouch storage. It gives the outfit a refined urban note tied to hospitality, grooming, and private display.",
				"glass",
				MaterialBehaviourType.Ceramic,
				ItemQuality.Good,
				SizeCategory.Small,
				140.0,
				58.0m,
				null,
				[],
				["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"],
				[CraftInput(180.0, "glass", "Glass Vessel Blank")],
				["TagTool - InRoom - an item with the Hot Fire tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_male_peasant_linen_qamis",
				"medieval_outfit_fatimid_male_peasant",
				"fatimid",
				"male",
				"peasant",
				"underlayer",
				"linen qamis",
				"qamis",
				"a $colour linen qamis",
				"This $colour light linen qamis is cut for heat, modesty, and constant wear. The cloth is pale and washable, making it a practical base layer for city, field, or river work.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				600.0,
				22.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Long-Sleeved_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_male_peasant_cotton_lower_wrap",
				"medieval_outfit_fatimid_male_peasant",
				"fatimid",
				"male",
				"peasant",
				"lower_body",
				"cotton lower wrap",
				"wrap",
				"a $colour cotton lower wrap",
				"This $colour cotton lower wrap is light, breathable, and easily re-tied. It gives a practical Egyptian and North African line to common clothing, especially beneath a robe or shoulder cloth.",
				"cotton",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				480.0,
				22.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Chausses",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "cotton", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "cotton", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_male_merchant_tiraz_banded_tunic",
				"medieval_outfit_fatimid_male_merchant",
				"fatimid",
				"male",
				"merchant",
				"bodywear",
				"tiraz-banded tunic",
				"tunic",
				"a $colour1 tiraz-banded tunic",
				"This $colour1 fine linen tunic is marked with a tiraz-style band at the sleeve or upper arm. The band signals connection to courtly textile culture while the body remains suitable for mercantile wear. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				880.0,
				82.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_female_merchant_perfume_flask",
				"medieval_outfit_fatimid_female_merchant",
				"fatimid",
				"female",
				"merchant",
				"role_item",
				"perfume flask",
				"flask",
				"a perfume flask",
				"This small glass perfume flask is stoppered and shaped to sit in a purse or be carried on a cord. It suggests urban wealth, trade access, and careful personal presentation.",
				"glass",
				MaterialBehaviourType.Ceramic,
				ItemQuality.Good,
				SizeCategory.Small,
				140.0,
				60.0m,
				null,
				[],
				["Holdable", "LContainer_DrinkingGlass", "Destroyable_Misc"],
				[CraftInput(180.0, "glass", "Glass Vessel Blank")],
				["TagTool - InRoom - an item with the Hot Fire tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_male_noble_court_kaftan",
				"medieval_outfit_fatimid_male_noble",
				"fatimid",
				"male",
				"noble",
				"bodywear",
				"court kaftan",
				"kaftan",
				"a $colour1 court kaftan",
				"This $colour1 court kaftan uses fine cloth and formal cuffs to create a light but authoritative silhouette. It is made for reception rooms, ceremonies, and the public display of rank. A secondary $colour2 detail marks the visible fastening points and finish.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				900.0,
				145.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_female_noble_light_mantle",
				"medieval_outfit_fatimid_female_noble",
				"fatimid",
				"female",
				"noble",
				"outerwear",
				"light mantle",
				"mantle",
				"a $colour light mantle",
				"This $colour mantle is light enough for warm interiors but fine enough for formal display. Its edge is carefully worked, and the cloth frames a court robe without smothering it.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				850.0,
				105.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_male_religious_endowment_slip",
				"medieval_outfit_fatimid_male_religious",
				"fatimid",
				"male",
				"religious",
				"role_item",
				"endowment slip",
				"slip",
				"an endowment slip",
				"This folded paper slip records a small endowment, instruction, or mosque account note. It is sized for a book pouch and marks the wearer as connected to religious administration.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				22.0,
				10.0m,
				null,
				[],
				["Holdable", "PaperSheet_Scroll", "Destroyable_Paper"],
				[CraftInput(30.0, "paper", "Paper Sheet Stock")],
				["TagTool - Held - an item with the Reed Pen tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_male_military_padded_coat_with_scale_panels",
				"medieval_outfit_fatimid_male_military",
				"fatimid",
				"male",
				"military",
				"bodywear",
				"padded coat with scale panels",
				"coat",
				"a $colour padded coat with scale panels",
				"This $colour guard coat combines quilted linen with small scale-reinforced panels over the most exposed areas. It is light enough for hot service but still visibly martial.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1750.0,
				72.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(420.0, "wrought iron", "Armour Lamella Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_male_military_archer_quiver",
				"medieval_outfit_fatimid_male_military",
				"fatimid",
				"male",
				"military",
				"role_item",
				"archer quiver",
				"quiver",
				"a $colour archer quiver",
				"This $colour leather quiver has a reinforced mouth and a strap arrangement suited to a guard or mounted archer. The outside is plain, but its stitching is tight and field-ready.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Normal,
				780.0,
				36.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Shoulder",
					"Container_Quiver",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_fatimid_female_artisan_linen_work_gown",
				"medieval_outfit_fatimid_female_artisan",
				"fatimid",
				"female",
				"artisan",
				"bodywear",
				"linen work gown",
				"gown",
				"a $colour linen work gown",
				"This $colour linen work gown is loose, breathable, and cut for movement in heat. Its sleeves can be tied or pushed back, making it useful for household, shop, or workshop labour.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				850.0,
				28.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Gown",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_peasant_plain_caftan",
				"medieval_outfit_seljuk_ayyubid_male_peasant",
				"seljuk_ayyubid",
				"male",
				"peasant",
				"bodywear",
				"plain caftan",
				"caftan",
				"a $colour plain caftan",
				"This $colour plain caftan has a front closure and a modest riding-friendly cut. It is practical for travel, household work, and cooler evenings without the decoration of court or cavalry clothing.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				950.0,
				32.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_merchant_riding_caftan",
				"medieval_outfit_seljuk_ayyubid_male_merchant",
				"seljuk_ayyubid",
				"male",
				"merchant",
				"bodywear",
				"riding caftan",
				"caftan",
				"a $colour riding caftan",
				"This $colour caftan is cut for riding and travel, with enough overlap to stay closed and enough sweep to sit over trousers. Better cloth and careful seams make it suitable for a merchant or official on the road.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1050.0,
				74.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_noble_high_boots",
				"medieval_outfit_seljuk_ayyubid_male_noble",
				"seljuk_ayyubid",
				"male",
				"noble",
				"footwear",
				"high boots",
				"boots",
				"$colour high riding boots",
				"These $colour high leather boots are shaped for stirrups, dust, and long days mounted. The uppers are firm without being rigid, and the seams are placed away from the inside of the leg.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Good,
				SizeCategory.Normal,
				1350.0,
				72.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Boots",
					"Insulation_Moderate",
					"Armour_BoiledLeather",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(520.0, "leather", "Turnshoe Upper Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_noble_fur_edged_cloak",
				"medieval_outfit_seljuk_ayyubid_male_noble",
				"seljuk_ayyubid",
				"male",
				"noble",
				"outerwear",
				"fur-edged cloak",
				"cloak",
				"a $colour fur-edged cloak",
				"This $colour riding cloak is edged with fur and cut to sit well over a caftan. It displays rank in a way suited to the road, the court, and the camp rather than only an indoor hall.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1650.0,
				120.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(180.0, "fur", "Fur Panel Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_religious_madrasa_notebook",
				"medieval_outfit_seljuk_ayyubid_male_religious",
				"seljuk_ayyubid",
				"male",
				"religious",
				"role_item",
				"madrasa notebook",
				"book",
				"a madrasa notebook",
				"This paper notebook has plain covers and sewn quires for lessons, copied passages, or legal notes. It is compact enough for a scholar’s pouch and sturdy enough for daily handling.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				420.0,
				78.0m,
				null,
				[],
				["Holdable", "Book_Small_40_Page", "Destroyable_Paper"],
				[
					CraftInput(90.0, "paper", "Paper Sheet Stock", colour: true, fineColour: true),
					CraftInput(70.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true),
					CraftInput(35.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - InRoom - an item with the Book Press tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_military_quilted_riding_coat",
				"medieval_outfit_seljuk_ayyubid_male_military",
				"seljuk_ayyubid",
				"male",
				"military",
				"bodywear",
				"quilted riding coat",
				"coat",
				"a $colour quilted riding coat",
				"This $colour riding coat is quilted for protection while preserving the movement needed on horseback. The skirts are split, the sleeves are practical, and the body is ready for a bowcase belt.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1700.0,
				64.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_military_bowcase_belt",
				"medieval_outfit_seljuk_ayyubid_male_military",
				"seljuk_ayyubid",
				"male",
				"military",
				"belt_or_sash",
				"bowcase belt",
				"belt",
				"a $colour bowcase belt",
				"This $colour broad leather belt is built with reinforced points for a bowcase and quiver. It is meant for a mounted archer, with enough stiffness to hold gear steady at speed.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				680.0,
				48.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_6",
					"Beltable",
					"Armour_BoiledLeather",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_female_military_quilted_riding_robe",
				"medieval_outfit_seljuk_ayyubid_female_military",
				"seljuk_ayyubid",
				"female",
				"military",
				"bodywear",
				"quilted riding robe",
				"robe",
				"a $colour quilted riding robe",
				"This $colour quilted robe adapts riding armour clothing to a longer line, with split lower panels and firm ties. It is made for field movement, not merely for looking martial.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1650.0,
				64.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_peasant_simple_turban",
				"medieval_outfit_seljuk_ayyubid_male_peasant",
				"seljuk_ayyubid",
				"male",
				"peasant",
				"headwear",
				"simple turban",
				"turban",
				"a $colour simple turban",
				"This $colour simple turban is wrapped from practical linen, protecting the head from sun and dust. It is plain enough for common work but still gives the outfit its regional shape.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				240.0,
				16.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_seljuk_ayyubid_male_military_lamellar_coat_cover",
				"medieval_outfit_seljuk_ayyubid_male_military",
				"seljuk_ayyubid",
				"male",
				"military",
				"outerwear",
				"lamellar coat cover",
				"cover",
				"a $colour lamellar coat cover",
				"This $colour cloth cover is cut to sit over a lamellar riding coat, protecting lacing and muting the hard outline of armour. Reinforced seams and tie points show it is a campaign garment.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				950.0,
				56.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Mantle",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
					CraftInput(420.0, "wrought iron", "Armour Lamella Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_peasant_linen_rubakha",
				"medieval_outfit_rus_novgorod_male_peasant",
				"rus_novgorod",
				"male",
				"peasant",
				"underlayer",
				"linen rubakha",
				"rubakha",
				"a $colour linen rubakha",
				"This $colour rubakha is a long linen shirt with a simple neck opening and sturdy seams. It forms the base of the outfit and can be belted or worn beneath a kaftan, coat, or cloak.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				680.0,
				24.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Long-Sleeved_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_peasant_onuchi_footwraps",
				"medieval_outfit_rus_novgorod_male_peasant",
				"rus_novgorod",
				"male",
				"peasant",
				"leg_or_sock_layer",
				"onuchi footwraps",
				"footwraps",
				"$colour onuchi footwraps",
				"These $colour long cloth footwraps are meant to be wound around the feet and lower legs before shoes or boots. They are plain, washable, and well suited to cold roads and river towns.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				220.0,
				10.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Socks",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_merchant_fur_edged_kaftan",
				"medieval_outfit_rus_novgorod_male_merchant",
				"rus_novgorod",
				"male",
				"merchant",
				"bodywear",
				"fur-edged kaftan",
				"kaftan",
				"a $colour fur-edged kaftan",
				"This $colour kaftan has a front closure and a narrow fur edge that marks it as winter-capable and prosperous. It is made for a trader who moves between river, market, and hall.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1250.0,
				98.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
					CraftInput(160.0, "fur", "Fur Panel Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_merchant_birchbark_document_pouch",
				"medieval_outfit_rus_novgorod_male_merchant",
				"rus_novgorod",
				"male",
				"merchant",
				"worn_container",
				"birchbark document pouch",
				"pouch",
				"a $colour birchbark document pouch",
				"This $colour leather pouch is stiffened and sized for folded birchbark notes, trade tallies, or small sealed tags. It hangs securely from a belt and keeps papers dry near river traffic.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Good,
				SizeCategory.Small,
				260.0,
				45.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Container_Pouch",
					"Beltable",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(520.0, "leather", "Prepared Leather Panel", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_noble_embroidered_kaftan",
				"medieval_outfit_rus_novgorod_male_noble",
				"rus_novgorod",
				"male",
				"noble",
				"bodywear",
				"embroidered kaftan",
				"kaftan",
				"a $colour1 embroidered kaftan",
				"This $colour1 fine kaftan has a controlled front closure, rich edge stitching, and a weighty winter-ready fall. It is suitable for princely households, wealthy merchants, and formal Orthodox settings. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"wool",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1200.0,
				105.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "wool", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_noble_fur_hat",
				"medieval_outfit_rus_novgorod_male_noble",
				"rus_novgorod",
				"male",
				"noble",
				"headwear",
				"fur hat",
				"hat",
				"a $colour fur hat",
				"This $colour fur hat is warm, rounded, and visibly high-status, with a soft crown and carefully set band. It suits winter travel, court display, and the public dignity of a northern household.",
				"fur",
				MaterialBehaviourType.Hair,
				ItemQuality.Good,
				SizeCategory.Small,
				260.0,
				75.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[CraftInput(180.0, "fur", "Fur Panel Stock")],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_religious_prayer_slip",
				"medieval_outfit_rus_novgorod_male_religious",
				"rus_novgorod",
				"male",
				"religious",
				"role_item",
				"prayer slip",
				"slip",
				"an Orthodox prayer slip",
				"This small prayer slip is written on a narrow sheet and folded for a pouch or icon shelf. It is a humble devotional object rather than a formal book.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Small,
				20.0,
				8.0m,
				null,
				[],
				["Holdable", "PaperSheet_Scroll", "Destroyable_Paper"],
				[CraftInput(30.0, "paper", "Paper Sheet Stock")],
				["TagTool - Held - an item with the Quill Pen tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_military_padded_war_coat",
				"medieval_outfit_rus_novgorod_male_military",
				"rus_novgorod",
				"male",
				"military",
				"bodywear",
				"padded war coat",
				"coat",
				"a $colour padded war coat",
				"This $colour war coat is quilted and cut to sit beneath mail, a belt, and a fur-edged cloak. It gives warmth as well as protection, reflecting northern campaign conditions.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1750.0,
				62.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "linen", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_male_military_axe_loop",
				"medieval_outfit_rus_novgorod_male_military",
				"rus_novgorod",
				"male",
				"military",
				"role_item",
				"axe loop",
				"loop",
				"a $colour axe loop",
				"This $colour leather loop is riveted to carry a small axe at the belt without swinging loose. It is a practical accessory for river guards, warriors, and travelling retainers.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				180.0,
				18.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Beltable",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_rus_novgorod_female_merchant_head_veil",
				"medieval_outfit_rus_novgorod_female_merchant",
				"rus_novgorod",
				"female",
				"merchant",
				"headwear",
				"head veil",
				"veil",
				"a $colour head veil",
				"This $colour linen veil is cut to sit beneath a cloak or fur-edged overgown, covering the hair in a modest and orderly way. It is finer than a work scarf but not as elaborate as court headwear.",
				"linen",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				210.0,
				28.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "linen", "Broadcloth Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_peasant_tied_riding_coat",
				"medieval_outfit_steppe_turkic_male_peasant",
				"steppe_turkic",
				"male",
				"peasant",
				"bodywear",
				"tied riding coat",
				"coat",
				"a $colour tied riding coat",
				"This $colour tied riding coat closes across the body and leaves the legs free for mounting, walking, or sitting low. It is made from sturdy felted wool and built for travel more than display.",
				"felt",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1150.0,
				42.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "felt", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_peasant_high_boots",
				"medieval_outfit_steppe_turkic_male_peasant",
				"steppe_turkic",
				"male",
				"peasant",
				"footwear",
				"high boots",
				"boots",
				"$colour high boots",
				"These $colour high leather boots are shaped for stirrups and cold grassland travel. The soles are firm, the uppers are practical, and the seams avoid the inside of the leg.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1300.0,
				50.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Boots",
					"Insulation_Moderate",
					"Armour_BoiledLeather",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Turnshoe Upper Stock", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_merchant_felt_riding_caftan",
				"medieval_outfit_steppe_turkic_male_merchant",
				"steppe_turkic",
				"male",
				"merchant",
				"bodywear",
				"felt riding caftan",
				"caftan",
				"a $colour felt riding caftan",
				"This $colour riding caftan uses fulled felt for warmth and structure, closing securely across the body. It is suited to travel, trade, and camp life where weather and mobility matter more than loose drape.",
				"felt",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1300.0,
				68.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "felt", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_merchant_seal_tag",
				"medieval_outfit_steppe_turkic_male_merchant",
				"steppe_turkic",
				"male",
				"merchant",
				"role_item",
				"seal tag",
				"tag",
				"a seal tag",
				"This small tag is corded for a pouch, bale, or messenger packet, carrying a mark of ownership or authority. It is light but important, the kind of object a travelling trader guards closely.",
				"wood",
				MaterialBehaviourType.Wood,
				ItemQuality.Standard,
				SizeCategory.Small,
				60.0,
				10.0m,
				null,
				[],
				["Holdable", "Destroyable_Misc"],
				[CraftInput(60.0, "willow", "Tally Stick Stock")],
				["TagTool - Held - an item with the Awl Punch tag"],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_noble_embroidered_riding_caftan",
				"medieval_outfit_steppe_turkic_male_noble",
				"steppe_turkic",
				"male",
				"noble",
				"bodywear",
				"embroidered riding caftan",
				"caftan",
				"a $colour1 embroidered riding caftan",
				"This $colour1 caftan is cut for riding but finished for rank, with embroidery along the closure and cuffs. The cloth is warm, the ties are secure, and the line remains mobile despite its display. The contrasting $colour2 work is concentrated at the visible edges, ties, or fastening points.",
				"felt",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				1350.0,
				96.0m,
				"Variable_2FineColour",
				["$colour1", "$colour2"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_2FineColour",
				],
				[
					CraftInput(650.0, "felt", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_noble_silk_sash",
				"medieval_outfit_steppe_turkic_male_noble",
				"steppe_turkic",
				"male",
				"noble",
				"belt_or_sash",
				"silk sash",
				"sash",
				"a $colour silk sash",
				"This $colour silk sash wraps firmly around the coat while adding colour and rank. It can secure a pouch, knife, or seal tag, but its smooth cloth and fine finish are meant to be seen.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				260.0,
				80.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_4",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_military_felt_war_cloak",
				"medieval_outfit_steppe_turkic_male_military",
				"steppe_turkic",
				"male",
				"military",
				"outerwear",
				"felt war cloak",
				"cloak",
				"a $colour felt war cloak",
				"This $colour felt war cloak is heavy, weatherproof, and cut to clear a bowcase belt. It is made for riders who need warmth at speed without loose cloth tangling around weapons.",
				"felt",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Large,
				1600.0,
				62.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Cloak_(Closed)",
					"Insulation_Strong",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "felt", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_male_military_bowcase_and_quiver_belt",
				"medieval_outfit_steppe_turkic_male_military",
				"steppe_turkic",
				"male",
				"military",
				"belt_or_sash",
				"bowcase-and-quiver belt",
				"belt",
				"a $colour bowcase-and-quiver belt",
				"This $colour broad leather belt has reinforced hangers for a bowcase, quiver, and field pouch. Its weight and construction are meant for a mounted archer, not a town pedestrian.",
				"leather",
				MaterialBehaviourType.Leather,
				ItemQuality.Standard,
				SizeCategory.Small,
				760.0,
				55.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Waist",
					"Belt_6",
					"Beltable",
					"Armour_BoiledLeather",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(520.0, "leather", "Leather Strap", colour: true, fineColour: true),
					CraftInput(55.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Awl Punch tag",
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_female_military_lamellar_riding_coat_cover",
				"medieval_outfit_steppe_turkic_female_military",
				"steppe_turkic",
				"female",
				"military",
				"bodywear",
				"lamellar riding coat cover",
				"cover",
				"a $colour lamellar riding coat cover",
				"This $colour cloth cover is shaped to sit over lamellar armour while preserving the tied-coat riding silhouette. It protects lacing and quiets the armour’s outline under a cloak.",
				"felt",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1100.0,
				60.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Moderate",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "felt", "Fulled Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "wool", "Spun Yarn", colour: true),
					CraftInput(420.0, "wrought iron", "Armour Lamella Stock"),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_steppe_turkic_female_peasant_fur_cap_or_headwrap",
				"medieval_outfit_steppe_turkic_female_peasant",
				"steppe_turkic",
				"female",
				"peasant",
				"headwear",
				"fur cap or headwrap",
				"hat",
				"a $colour fur cap and headwrap",
				"This $colour winter head covering combines a soft fur cap with a wrap that can be drawn around the ears and neck. It is practical on open ground and immediately marks the outfit as steppe clothing.",
				"fur",
				MaterialBehaviourType.Hair,
				ItemQuality.Standard,
				SizeCategory.Small,
				260.0,
				42.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[CraftInput(180.0, "fur", "Fur Panel Stock")],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_peasant_short_working_jacket",
				"medieval_outfit_song_china_male_peasant",
				"song_china",
				"male",
				"peasant",
				"bodywear",
				"short working jacket",
				"jacket",
				"a $colour short working jacket",
				"This $colour short jacket is cut for labour, with narrow sleeves and a plain front that stays out of tools and water. The cloth is modest but neatly sewn, suited to a town worker or rural household.",
				"cotton",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				620.0,
				24.0m,
				"Variable_DrabColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_DrabColour",
				],
				[
					CraftInput(650.0, "cotton", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "cotton", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_female_peasant_cross_collar_work_robe",
				"medieval_outfit_song_china_female_peasant",
				"song_china",
				"female",
				"peasant",
				"bodywear",
				"cross-collar work robe",
				"robe",
				"a $colour cross-collar work robe",
				"This $colour cross-collar robe uses practical cloth and a restrained line, wrapping neatly without the bulk of court dress. It is built for daily household or market work while preserving a scholarly urban silhouette.",
				"cotton",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				850.0,
				36.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "cotton", "Garment Cloth", colour: true, fineColour: true),
					CraftInput(55.0, "cotton", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_merchant_merchant_robe",
				"medieval_outfit_song_china_male_merchant",
				"song_china",
				"male",
				"merchant",
				"bodywear",
				"merchant robe",
				"robe",
				"a $colour merchant robe",
				"This $colour robe is made from better cloth with a smooth cross-body fall and tidy sleeve line. It suggests a prosperous urban wearer who needs to move between shop, account table, and tea room.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				840.0,
				105.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_merchant_scholar_style_cap",
				"medieval_outfit_song_china_male_merchant",
				"song_china",
				"male",
				"merchant",
				"headwear",
				"scholar-style cap",
				"cap",
				"a $colour scholar-style cap",
				"This $colour cap has a formal, orderly shape that evokes literacy and urban status without requiring official rank. It is light, neat, and meant to complete a merchant’s respectable public appearance.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				120.0,
				55.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_noble_scholar_robe",
				"medieval_outfit_song_china_male_noble",
				"song_china",
				"male",
				"noble",
				"bodywear",
				"scholar robe",
				"robe",
				"a $colour scholar robe",
				"This $colour scholar robe uses fine cloth and a long, calm line, made for study, reception, and official-adjacent dignity. The sleeves are generous but controlled, and the finish is deliberately restrained.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Normal,
				900.0,
				125.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_noble_official_cap",
				"medieval_outfit_song_china_male_noble",
				"song_china",
				"male",
				"noble",
				"headwear",
				"official cap",
				"cap",
				"a $colour official cap",
				"This $colour formal cap is shaped to signal rank, learning, or bureaucratic association. It is light and precise, with a disciplined silhouette that changes the whole outfit even before the robe is noticed.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				140.0,
				70.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Hat",
					"Insulation_Minor",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Silk Brocade Panel", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_female_noble_padded_winter_robe",
				"medieval_outfit_song_china_female_noble",
				"song_china",
				"female",
				"noble",
				"outerwear",
				"padded winter robe",
				"robe",
				"a $colour padded winter robe",
				"This $colour winter robe is softly quilted, giving warmth without losing the cross-collar line of formal dress. It is a high-status garment for cold rooms, travel, and public appearance in colder months.",
				"silk",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Large,
				1400.0,
				140.0m,
				"Variable_FineColour",
				["$colour"],
				[
					"Holdable",
					"Wear_Robe",
					"Insulation_Strong",
					"Armour_LightClothing",
					"Destroyable_Clothing",
					"Variable_FineColour",
				],
				[
					CraftInput(650.0, "silk", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "silk", "Spun Yarn", colour: true),
					CraftInput(45.0, "linen", "Embroidered Trim Stock", colour: true, fineColour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_religious_notebook",
				"medieval_outfit_song_china_male_religious",
				"song_china",
				"male",
				"religious",
				"role_item",
				"notebook",
				"book",
				"a scholar-monastic notebook",
				"This notebook is made for copied passages, accounts, or teaching notes, with compact paper gatherings and a plain cover. It belongs as much to a study desk as to a book pouch.",
				"paper",
				MaterialBehaviourType.Fabric,
				ItemQuality.Good,
				SizeCategory.Small,
				420.0,
				82.0m,
				null,
				[],
				["Holdable", "Book_Small_40_Page", "Destroyable_Paper"],
				[
					CraftInput(90.0, "paper", "Paper Sheet Stock", colour: true, fineColour: true),
					CraftInput(70.0, "leather", "Bookbinding Leather Stock", colour: true, fineColour: true),
					CraftInput(35.0, "linen", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - InRoom - an item with the Book Press tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_military_padded_military_vest",
				"medieval_outfit_song_china_male_military",
				"song_china",
				"male",
				"military",
				"bodywear",
				"padded military vest",
				"vest",
				"a $colour padded military vest",
				"This $colour padded vest protects the torso without restricting the arms, making it useful beneath lamellar or over an under-robe. The quilting is close and functional, with little ornament.",
				"cotton",
				MaterialBehaviourType.Fabric,
				ItemQuality.Standard,
				SizeCategory.Normal,
				1250.0,
				52.0m,
				"Variable_Colour",
				["$colour"],
				[
					"Holdable",
					"Wear_Tunic",
					"Insulation_Moderate",
					"Armour_HeavyClothing",
					"Destroyable_Clothing",
					"Variable_Colour",
				],
				[
					CraftInput(650.0, "cotton", "Quilted Armour Padding", colour: true, fineColour: true),
					CraftInput(55.0, "cotton", "Spun Yarn", colour: true),
				],
				[
					"TagTool - Held - an item with the Sewing Needle tag",
					"TagTool - Held - an item with the Shears tag",
				],
				null,
				"Uses variable colour in sdesc/full description; avoids explicit culture adjective in player-facing description."),
			BespokeOutfitPiece(
				"medieval_outfit_piece_song_china_male_military_guard_token",
				"medieval_outfit_song_china_male_military",
				"song_china",
				"male",
				"military",
				"role_item",
				"guard token",
				"token",
				"a guard token",
				"This small token marks guard duty, command authority, or access to an official post. It is simple enough to carry but distinct enough to serve as a visible role marker.",
				"bronze",
				MaterialBehaviourType.Metal,
				ItemQuality.Standard,
				SizeCategory.Small,
				80.0,
				32.0m,
				null,
				[],
				["Holdable", "Wear_Necklace", "Destroyable_HeavyMetal"],
				[CraftInput(120.0, "bronze", "Tool Blank Stock")],
				[
					"TagTool - InRoom - an item with the Anvil tag",
					"TagTool - Held - an item with the Hammer tag",
				],
				null,
				"No variable colour recommended because item material or function is not primarily a garment/colourable textile/leather piece."),
		];
	}

	private static MedievalBespokeOutfitPieceSpec BespokeOutfitPiece(
		string stableReference,
		string outfitReference,
		string cultureKey,
		string sexGenderPresentation,
		string socialClassRole,
		string slotKey,
		string pieceName,
		string noun,
		string shortDescription,
		string fullDescription,
		string material,
		MaterialBehaviourType materialType,
		ItemQuality quality,
		SizeCategory size,
		double weightInGrams,
		decimal cost,
		string? variableColourComponent,
		string[] colourVariablesUsed,
		string[] components,
		string[] craftInputs,
		string[] craftTools,
		string? implementationNotes,
		string? authoringGuidelineNotes)
	{
		return new MedievalBespokeOutfitPieceSpec(
			stableReference,
			outfitReference,
			cultureKey,
			sexGenderPresentation,
			socialClassRole,
			slotKey,
			pieceName,
			noun,
			shortDescription,
			fullDescription,
			material,
			materialType,
			quality,
			size,
			weightInGrams,
			cost,
			variableColourComponent,
			colourVariablesUsed,
			components,
			craftInputs,
			craftTools,
			implementationNotes,
			authoringGuidelineNotes);
	}

	private static string CraftInput(double grams, string material, string? pileTag = null, bool colour = false,
		bool fineColour = false)
	{
		return CommodityInput(grams, material, pileTag, colour, fineColour);
	}

	private static string[] BuildMedievalBespokeOutfitPieceSpecComponents(
		MedievalBespokeOutfitPieceSpec bespokeSpec)
	{
		var components = bespokeSpec.Components.ToList();

		void AddIfMissing(string component)
		{
			if (!components.Contains(component, StringComparer.OrdinalIgnoreCase))
			{
				components.Add(component);
			}
		}

		AddIfMissing("Holdable");
		if (!string.IsNullOrWhiteSpace(bespokeSpec.VariableColourComponent))
		{
			AddIfMissing(bespokeSpec.VariableColourComponent);
		}

		return components
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static string BuildMedievalBespokeOutfitPieceSpecBuilderNotes(
		MedievalBespokeOutfitPieceSpec bespokeSpec)
	{
		var lines = new List<string>
		{
			"Explicit medieval outfit piece authored catalogue entry.",
			$"Outfit reference: {bespokeSpec.OutfitReference}.",
			$"Culture key: {bespokeSpec.CultureKey}.",
			$"Sex/gender presentation: {bespokeSpec.SexGenderPresentation}.",
			$"Social class/role: {bespokeSpec.SocialClassRole}.",
			$"Slot: {bespokeSpec.SlotKey}.",
			$"Piece target: {bespokeSpec.PieceName}."
		};

		if (!string.IsNullOrWhiteSpace(bespokeSpec.AuthoringGuidelineNotes))
		{
			lines.Add($"Authoring notes: {bespokeSpec.AuthoringGuidelineNotes}.");
		}

		if (!string.IsNullOrWhiteSpace(bespokeSpec.ImplementationNotes))
		{
			lines.Add($"Implementation notes: {bespokeSpec.ImplementationNotes}.");
		}

		return string.Join("\n", lines);
	}

	private static IReadOnlyList<EraClothingPieceSpec> MedievalExplicitOutfitClothingPieces()
	{
		return MedievalExplicitOutfitPieces
			.GroupBy(x => x.StableReference, StringComparer.OrdinalIgnoreCase)
			.Select(group => BuildMedievalExplicitOutfitClothingPieceSpec(
				group.First(),
				group.Select(x => x.SlotKey).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
				group.Select(x => x.OutfitReference).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()))
			.ToArray();
	}

	private static IReadOnlyList<EraItemSpec> MedievalExplicitOutfitPieceItemSpecs()
	{
		return MedievalExplicitOutfitClothingPieces()
			.Select(x => x.Item)
			.ToArray();
	}

	private static EraClothingPieceSpec BuildMedievalExplicitOutfitClothingPieceSpec(
		EraOutfitPieceSpec piece,
		IReadOnlyCollection<string> slotKeys,
		IReadOnlyCollection<string> outfitReferences)
	{
		var item = BuildMedievalExplicitOutfitPieceItemSpec(piece, slotKeys);
		var variableColourComponent = MedievalExplicitOutfitPieceVariableColourComponent(item);
		return new EraClothingPieceSpec(
			item,
			piece.OutfitReference,
			piece.CultureKey,
			piece.SexGenderPresentation,
			piece.SocialClassRole,
			piece.PieceName,
			piece.CultureSpecificOrClusterSpecific,
			slotKeys,
			outfitReferences,
			variableColourComponent,
			MedievalExplicitOutfitPieceColourVariables(variableColourComponent),
			BuildMedievalExplicitOutfitPieceCraftSpec(item),
			!piece.CultureSpecificOrClusterSpecific);
	}

	private static EraItemSpec BuildMedievalExplicitOutfitPieceItemSpec(EraOutfitPieceSpec piece,
		IReadOnlyCollection<string> slotKeys)
	{
		var culture = MedievalCultureProfiles.Single(x => x.Key.Equals(piece.CultureKey, StringComparison.OrdinalIgnoreCase));
		var status = MedievalStatusRoleProfiles.Single(x =>
			x.Key.Equals(MedievalOutfitRoleToStatusRoleKey[piece.SocialClassRole], StringComparison.OrdinalIgnoreCase));
		var (material, materialType) = MedievalExplicitOutfitPieceMaterial(piece.PieceName, slotKeys);
		var variableColourComponent = MedievalExplicitOutfitPieceVariableColourComponent(piece.PieceName, material, materialType);
		var colourVariables = MedievalExplicitOutfitPieceColourVariables(variableColourComponent);
		var components = MedievalExplicitOutfitPieceComponents(piece.PieceName, slotKeys, materialType, variableColourComponent);
		var quality = MedievalExplicitOutfitPieceQuality(piece.PieceName, piece.SocialClassRole);
		var size = MedievalExplicitOutfitPieceSize(slotKeys);
		var weight = MedievalExplicitOutfitPieceWeightInGrams(piece.PieceName, slotKeys);
		var cost = MedievalExplicitOutfitPieceCost(piece.PieceName, slotKeys, quality);
		var shortDescription = BuildMedievalExplicitOutfitPieceShortDescription(piece.PieceName, variableColourComponent);
		var slotList = string.Join(", ", slotKeys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));

		if (MedievalBespokeOutfitPieces.TryGetValue(piece.StableReference, out var bespokeSpec))
		{
			return new EraItemSpec(
				piece.StableReference,
				bespokeSpec.Noun,
				bespokeSpec.ShortDescription,
				bespokeSpec.FullDescription,
				bespokeSpec.Size,
				bespokeSpec.Quality,
				bespokeSpec.WeightInGrams,
				bespokeSpec.Cost,
				bespokeSpec.Material,
				bespokeSpec.MaterialType,
				[
					MedievalRootTagPath,
					$"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}",
					$"{MedievalStatusTagRoot} / {status.TagName}",
					MedievalExplicitOutfitPieceMarketTag(slotKeys, bespokeSpec.Quality),
					MedievalExplicitOutfitPieceFunctionTag(slotKeys)
				],
				BuildMedievalBespokeOutfitPieceSpecComponents(bespokeSpec),
				BuildMedievalBespokeOutfitPieceSpecBuilderNotes(bespokeSpec));
		}

		return new EraItemSpec(
			piece.StableReference,
			MedievalExplicitOutfitPieceNoun(piece.PieceName),
			shortDescription,
			BuildMedievalExplicitOutfitPieceFullDescription(piece.PieceName, slotKeys, material, variableColourComponent),
			size,
			quality,
			weight,
			cost,
			material,
			materialType,
			[
				MedievalRootTagPath,
				$"{MedievalCultureTagRoot} / {SafeMedievalTagName(culture.Display)}",
				$"{MedievalStatusTagRoot} / {status.TagName}",
				MedievalExplicitOutfitPieceMarketTag(slotKeys, quality),
				MedievalExplicitOutfitPieceFunctionTag(slotKeys)
			],
			components,
			$"Explicit medieval outfit piece authored catalogue entry. Outfit reference: {piece.OutfitReference}. Culture key: {piece.CultureKey}. Culture: {culture.Display}. Sex/gender presentation: {piece.SexGenderPresentation}. Social class/role: {piece.SocialClassRole}. Piece target: {piece.PieceName}. Slot(s): {slotList}. Colour variables: {string.Join(", ", colourVariables)}.");
	}

	private static string? MedievalExplicitOutfitPieceVariableColourComponent(string pieceName, string material,
		MaterialBehaviourType materialType)
	{
		if (material.Equals("paper", StringComparison.OrdinalIgnoreCase) ||
		    materialType is MaterialBehaviourType.Metal or MaterialBehaviourType.Wood or MaterialBehaviourType.Wax or MaterialBehaviourType.Ceramic)
		{
			return null;
		}

		if (MedievalExplicitOutfitPieceUsesTwoColours(pieceName))
		{
			return materialType is MaterialBehaviourType.Leather or MaterialBehaviourType.Hair
				? "Variable_2DrabColour"
				: "Variable_2FineColour";
		}

		return materialType is MaterialBehaviourType.Leather or MaterialBehaviourType.Hair ||
		       material.Equals("felt", StringComparison.OrdinalIgnoreCase) ||
		       material.Equals("hemp", StringComparison.OrdinalIgnoreCase)
			? "Variable_DrabColour"
			: "Variable_FineColour";
	}

	private static bool MedievalExplicitOutfitPieceUsesTwoColours(string pieceName)
	{
		return ContainsAny(pieceName,
			"tablet",
			"band",
			"border",
			"embroider",
			"tiraz",
			"panel",
			"cuff",
			"hem",
			"trim",
			"braid",
			"brocade",
			"decorated",
			"fur-edged",
			"fur-lined",
			"formal",
			"official",
			"rich",
			"court",
			"lined");
	}

	private static string[] MedievalExplicitOutfitPieceColourVariables(string? variableColourComponent)
	{
		return variableColourComponent switch
		{
			null => [],
			_ when variableColourComponent.Contains("_2", StringComparison.OrdinalIgnoreCase) => ["$colour1", "$colour2"],
			_ => ["$colour"]
		};
	}

	private static string BuildMedievalExplicitOutfitPieceShortDescription(string pieceName,
		string? variableColourComponent)
	{
		var colourPrefix = variableColourComponent switch
		{
			null => string.Empty,
			_ when variableColourComponent.Contains("_2", StringComparison.OrdinalIgnoreCase) => "$colour1 ",
			_ => "$colour "
		};
		var namedPiece = $"{colourPrefix}{pieceName}";
		if (IsPluralMedievalOutfitPiece(pieceName))
		{
			return namedPiece;
		}

		var article = "aeiou".Contains(char.ToLowerInvariant(pieceName[0])) ? "an" : "a";
		return $"{article} {namedPiece}";
	}

	private static string BuildMedievalExplicitOutfitPieceFullDescription(string pieceName,
		IReadOnlyCollection<string> slotKeys, string material, string? variableColourComponent)
	{
		var lower = pieceName.ToLowerInvariant();
		var colourLead = variableColourComponent switch
		{
			null => string.Empty,
			_ when variableColourComponent.Contains("_2", StringComparison.OrdinalIgnoreCase) => "$colour1 ",
			_ => "$colour "
		};
		var materialText = material.Equals("fur", StringComparison.OrdinalIgnoreCase)
			? "fur and hide"
			: material;
		var construction = slotKeys.Contains(EraOutfitSlotSpecFootwear, StringComparer.OrdinalIgnoreCase)
			? "turned seams, firm soles, and close stitching around the foot"
			: slotKeys.Contains(EraOutfitSlotSpecHeadwear, StringComparer.OrdinalIgnoreCase)
				? "a shaped crown, hemmed edge, and ties or pins where the form needs them"
				: slotKeys.Contains(EraOutfitSlotSpecBeltOrSash, StringComparer.OrdinalIgnoreCase)
					? "worked strap ends, reinforced stress points, and a fastening meant for daily wear"
					: slotKeys.Contains(EraOutfitSlotSpecWornContainer, StringComparer.OrdinalIgnoreCase)
						? "a reinforced mouth, close seams, and loops or ties for wearing at the body"
						: slotKeys.Contains(EraOutfitSlotSpecFastenerOrJewellery, StringComparer.OrdinalIgnoreCase) ||
						  slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase)
							? "small fitted details, rubbed edges, and a shape meant to sit securely when worn or carried"
							: slotKeys.Contains(EraOutfitSlotSpecOuterwear, StringComparer.OrdinalIgnoreCase)
								? "broad panels, weather-facing seams, and enough weight to hang cleanly over other garments"
								: slotKeys.Contains(EraOutfitSlotSpecLegOrSockLayer, StringComparer.OrdinalIgnoreCase) ||
								  slotKeys.Contains(EraOutfitSlotSpecLowerBody, StringComparer.OrdinalIgnoreCase)
									? "narrow panels, folded edges, and stitching placed for movement at the knee and ankle"
									: "cut panels, finished hems, and seams placed to sit smoothly in layered dress";
		var use = slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase)
			? "It is small enough to keep close at hand while still showing its use through shape and wear."
			: "It is made for layered daily wear, with the useful parts reinforced before the decorative parts are finished.";
		var decoration = variableColourComponent switch
		{
			null => lower.Contains("book", StringComparison.OrdinalIgnoreCase) || lower.Contains("slip", StringComparison.OrdinalIgnoreCase)
				? "The surface is plain enough for writing, sealing, or handling without hiding its prepared edges."
				: "Its finish is restrained, relying on form, material, and worn edges rather than applied colour.",
			_ when variableColourComponent.Contains("_2", StringComparison.OrdinalIgnoreCase) =>
				"The $colour2 work is used on visible edges, bands, borders, panels, or fastening points so the contrast shows while the piece is worn.",
			_ => "The colour is worked through the visible cloth or leather rather than painted over the finished surface."
		};

		return $"This {colourLead}{pieceName} is made from {materialText}, with {construction}. {use} {decoration}";
	}

	private static string MedievalOutfitCultureAdjective(string cultureKey)
	{
		return cultureKey switch
		{
			"early_anglo_saxon" => "Early Anglo-Saxon",
			"anglo_danish" => "Anglo-Danish",
			"norse" => "Norse",
			"norman" => "Norman",
			"high_british" => "High British",
			"gaelic" => "Gaelic",
			"carolingian" => "Carolingian",
			"capetian" => "Capetian",
			"german_hre" => "German-HRE",
			"iberian_christian" => "Iberian Christian",
			"andalusi" => "Andalusi",
			"byzantine" => "Byzantine",
			"abbasid" => "Abbasid",
			"fatimid" => "Fatimid",
			"seljuk_ayyubid" => "Seljuk-Ayyubid",
			"rus_novgorod" => "Rus-Novgorod",
			"steppe_turkic" => "Steppe Turkic",
			"song_china" => "Song Chinese",
			_ => MedievalCultureProfiles.Single(x => x.Key.Equals(cultureKey, StringComparison.OrdinalIgnoreCase)).Display
		};
	}

	private static bool IsPluralMedievalOutfitPiece(string pieceName)
	{
		var lower = pieceName.ToLowerInvariant();
		return lower.EndsWith("braies", StringComparison.Ordinal) ||
		       lower.EndsWith("trews", StringComparison.Ordinal) ||
		       lower.EndsWith("trousers", StringComparison.Ordinal) ||
		       lower.EndsWith("wraps", StringComparison.Ordinal) ||
		       lower.EndsWith("footwraps", StringComparison.Ordinal) ||
		       lower.EndsWith("shoes", StringComparison.Ordinal) ||
		       lower.EndsWith("slippers", StringComparison.Ordinal) ||
		       lower.EndsWith("boots", StringComparison.Ordinal) ||
		       lower.EndsWith("sandals", StringComparison.Ordinal) ||
		       lower.EndsWith("socks", StringComparison.Ordinal) ||
		       lower.EndsWith("hose", StringComparison.Ordinal) ||
		       lower.EndsWith("chausses", StringComparison.Ordinal) ||
		       lower.EndsWith("sirwal", StringComparison.Ordinal) ||
		       lower.EndsWith("onuchi", StringComparison.Ordinal) ||
		       lower.EndsWith("bracers", StringComparison.Ordinal) ||
		       lower.EndsWith("gloves", StringComparison.Ordinal) ||
		       lower.EndsWith("mitts", StringComparison.Ordinal) ||
		       lower.EndsWith("ties", StringComparison.Ordinal) ||
		       lower.EndsWith("beads", StringComparison.Ordinal);
	}

	private static string MedievalExplicitOutfitPieceNoun(string pieceName)
	{
		var lower = pieceName.ToLowerInvariant();
		foreach (var noun in new[]
		         {
			         "hangerok apron dress", "oval brooch pair", "cloak brooch", "ring pin", "head veil",
			         "head rail", "headcloth", "field pouch", "tool pouch", "document pouch", "book pouch",
			         "belt pouch", "purse belt", "seax belt", "arming belt", "weapon belt", "spear carrier belt",
			         "scabbard harness", "archer bracer", "leather bracers", "tablet", "prayer book",
			         "prayer slip", "cross pendant", "wooden cross", "guild badge", "trade tag", "tally cord",
			         "seal cord", "cloak clasp", "brooch", "pin", "girdle", "belt", "pouch", "purse",
			         "shirt", "shift", "tunic", "gown", "robe", "habit", "cloak", "mantle", "brat",
			         "cap", "coif", "hood", "veil", "shoes", "boots", "sandals", "hose", "chausses",
			         "trews", "braies", "wraps", "skirt", "gloves", "mitts"
		         })
		{
			if (lower.Contains(noun, StringComparison.Ordinal))
			{
				return noun.Split(' ').Last();
			}
		}

		return lower.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
	}

	private static (string Material, MaterialBehaviourType MaterialType) MedievalExplicitOutfitPieceMaterial(
		string pieceName, IReadOnlyCollection<string> slotKeys)
	{
		var lower = pieceName.ToLowerInvariant();
		if (slotKeys.Contains(EraOutfitSlotSpecBeltOrSash, StringComparer.OrdinalIgnoreCase) &&
		    lower.Contains("belt", StringComparison.Ordinal))
		{
			return ("leather", MaterialBehaviourType.Leather);
		}

		if (lower.Contains("silver", StringComparison.Ordinal))
		{
			return ("silver", MaterialBehaviourType.Metal);
		}

		if (lower.Contains("gold", StringComparison.Ordinal))
		{
			return ("gold", MaterialBehaviourType.Metal);
		}

		if (lower.Contains("bronze", StringComparison.Ordinal) ||
		    lower.Contains("brooch", StringComparison.Ordinal) ||
		    lower.Contains("pin", StringComparison.Ordinal) ||
		    lower.Contains("clasp", StringComparison.Ordinal) ||
		    lower.Contains("buckle", StringComparison.Ordinal) ||
		    lower.Contains("badge", StringComparison.Ordinal) ||
		    lower.Contains("spurs", StringComparison.Ordinal) ||
		    lower.Contains("mount", StringComparison.Ordinal) ||
		    lower.Contains("hook", StringComparison.Ordinal) ||
		    lower.Contains("token", StringComparison.Ordinal) ||
		    lower.Contains("mark", StringComparison.Ordinal))
		{
			return ("bronze", MaterialBehaviourType.Metal);
		}

		if (lower.Contains("iron", StringComparison.Ordinal))
		{
			return ("wrought iron", MaterialBehaviourType.Metal);
		}

		if (lower.Contains("wax", StringComparison.Ordinal))
		{
			return ("beeswax", MaterialBehaviourType.Wax);
		}

		if (lower.Contains("paper", StringComparison.Ordinal) ||
		    lower.Contains("notebook", StringComparison.Ordinal) ||
		    lower.Contains("slip", StringComparison.Ordinal) ||
		    lower.Contains("leaf", StringComparison.Ordinal) ||
		    lower.Contains("booklet", StringComparison.Ordinal) ||
		    lower.Contains("book", StringComparison.Ordinal) &&
		    !lower.Contains("pouch", StringComparison.Ordinal))
		{
			return ("paper", MaterialBehaviourType.Fabric);
		}

		if (lower.Contains("wooden", StringComparison.Ordinal) ||
		    lower.Contains("tally", StringComparison.Ordinal) ||
		    lower.Contains("tablet", StringComparison.Ordinal) &&
		    !lower.Contains("tablet-banded", StringComparison.Ordinal) &&
		    !lower.Contains("tablet-woven", StringComparison.Ordinal) ||
		    lower.Contains("board", StringComparison.Ordinal))
		{
			return ("oak", MaterialBehaviourType.Wood);
		}

		if (lower.Contains("cloth shoe", StringComparison.Ordinal))
		{
			return ("cotton", MaterialBehaviourType.Fabric);
		}

		if (lower.Contains("cotton", StringComparison.Ordinal))
		{
			return ("cotton", MaterialBehaviourType.Fabric);
		}

		if (lower.Contains("felt", StringComparison.Ordinal))
		{
			return ("felt", MaterialBehaviourType.Fabric);
		}

		if (slotKeys.Contains(EraOutfitSlotSpecHeadwear, StringComparer.OrdinalIgnoreCase) &&
		    (lower.Contains("fur cap", StringComparison.Ordinal) ||
		     lower.Contains("fur hat", StringComparison.Ordinal)))
		{
			return ("fur", MaterialBehaviourType.Hair);
		}

		if (lower.Contains("leather", StringComparison.Ordinal) ||
		    lower.Contains("deerskin", StringComparison.Ordinal) ||
		    lower.Contains("shoe", StringComparison.Ordinal) ||
		    lower.Contains("boot", StringComparison.Ordinal) ||
		    lower.Contains("sandal", StringComparison.Ordinal) ||
		    lower.Contains("slipper", StringComparison.Ordinal) ||
		    lower.Contains("harness", StringComparison.Ordinal) ||
		    lower.Contains("bracer", StringComparison.Ordinal))
		{
			return ("leather", MaterialBehaviourType.Leather);
		}

		if (lower.Contains("silk", StringComparison.Ordinal))
		{
			return ("silk", MaterialBehaviourType.Fabric);
		}

		if (lower.Contains("linen", StringComparison.Ordinal) ||
		    lower.Contains("qamis", StringComparison.Ordinal) ||
		    lower.Contains("rubakha", StringComparison.Ordinal) ||
		    lower.Contains("shift", StringComparison.Ordinal) ||
		    lower.Contains("under-robe", StringComparison.Ordinal) ||
		    lower.Contains("cloth", StringComparison.Ordinal) ||
		    lower.Contains("coif", StringComparison.Ordinal) ||
		    lower.Contains("veil", StringComparison.Ordinal) ||
		    lower.Contains("headcloth", StringComparison.Ordinal) ||
		    lower.Contains("headwrap", StringComparison.Ordinal) ||
		    lower.Contains("headscarf", StringComparison.Ordinal) ||
		    lower.Contains("head rail", StringComparison.Ordinal))
		{
			return ("linen", MaterialBehaviourType.Fabric);
		}

		if (lower.Contains("rope", StringComparison.Ordinal) ||
		    lower.Contains("cord", StringComparison.Ordinal))
		{
			return ("hemp", MaterialBehaviourType.Fabric);
		}

		if (lower.Contains("sash", StringComparison.Ordinal) ||
		    lower.Contains("girdle", StringComparison.Ordinal))
		{
			return ("wool", MaterialBehaviourType.Fabric);
		}

		if (slotKeys.Contains(EraOutfitSlotSpecBeltOrSash, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecWornContainer, StringComparer.OrdinalIgnoreCase))
		{
			return ("leather", MaterialBehaviourType.Leather);
		}

		return ("wool", MaterialBehaviourType.Fabric);
	}

	private static string[] MedievalExplicitOutfitPieceComponents(string pieceName,
		IReadOnlyCollection<string> slotKeys, MaterialBehaviourType materialType, string? variableColourComponent)
	{
		var components = new List<string> { "Holdable" };
		var lower = pieceName.ToLowerInvariant();
		if (slotKeys.Contains(EraOutfitSlotSpecFootwear, StringComparer.OrdinalIgnoreCase))
		{
			components.Add(lower.Contains("boot", StringComparison.Ordinal) ? "Wear_Boots" :
				lower.Contains("sandal", StringComparison.Ordinal) ? "Wear_Sandals" : "Wear_Shoes");
			components.Add("Insulation_Minor");
			components.Add("Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecBeltOrSash, StringComparer.OrdinalIgnoreCase))
		{
			components.Add("Wear_Waist");
			components.Add("Belt_4");
			components.Add("Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecWornContainer, StringComparer.OrdinalIgnoreCase))
		{
			components.Add("Wear_Waist");
			components.Add(lower.Contains("purse", StringComparison.Ordinal) ? "Container_Purse" : "Container_Pouch");
			components.Add("Beltable");
			components.Add("Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecFastenerOrJewellery, StringComparer.OrdinalIgnoreCase))
		{
			components.Add(lower.Contains("necklace", StringComparison.Ordinal) ||
			               lower.Contains("bead", StringComparison.Ordinal) ||
			               lower.Contains("cross", StringComparison.Ordinal) ||
			               lower.Contains("pendant", StringComparison.Ordinal)
				? "Wear_Necklace"
				: "Wear_Shoulder");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecHeadwear, StringComparer.OrdinalIgnoreCase))
		{
			components.Add("Wear_Hat");
			components.Add("Insulation_Minor");
			components.Add("Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecOuterwear, StringComparer.OrdinalIgnoreCase))
		{
			components.Add(lower.Contains("mantle", StringComparison.Ordinal) ||
			               lower.Contains("brat", StringComparison.Ordinal) ||
			               lower.Contains("surcoat", StringComparison.Ordinal)
				? "Wear_Mantle"
				: "Wear_Cloak_(Closed)");
			components.Add("Insulation_Strong");
			components.Add(lower.Contains("mail", StringComparison.Ordinal) || lower.Contains("war", StringComparison.Ordinal)
				? "Armour_HeavyClothing"
				: "Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecBodywear, StringComparer.OrdinalIgnoreCase))
		{
			components.Add(lower.Contains("apron", StringComparison.Ordinal) ? "Wear_Apron" :
				lower.Contains("robe", StringComparison.Ordinal) || lower.Contains("habit", StringComparison.Ordinal) ? "Wear_Robe" :
				lower.Contains("gown", StringComparison.Ordinal) || lower.Contains("dress", StringComparison.Ordinal) ||
				lower.Contains("bliaut", StringComparison.Ordinal) || lower.Contains("hangerok", StringComparison.Ordinal)
					? "Wear_Gown"
					: "Wear_Tunic");
			components.Add(lower.Contains("padded", StringComparison.Ordinal) ||
			               lower.Contains("arming", StringComparison.Ordinal) ||
			               lower.Contains("gambeson", StringComparison.Ordinal)
				? "Insulation_Moderate"
				: "Insulation_Minor");
			components.Add(lower.Contains("padded", StringComparison.Ordinal) ||
			               lower.Contains("arming", StringComparison.Ordinal) ||
			               lower.Contains("gambeson", StringComparison.Ordinal)
				? "Armour_HeavyClothing"
				: "Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecLegOrSockLayer, StringComparer.OrdinalIgnoreCase))
		{
			components.Add(lower.Contains("footwrap", StringComparison.Ordinal) ||
			               lower.Contains("sock", StringComparison.Ordinal) ||
			               lower.Contains("leg wrap", StringComparison.Ordinal) ||
			               lower.Contains("onuchi", StringComparison.Ordinal)
				? "Wear_Socks"
				: "Wear_Chausses");
			components.Add("Insulation_Moderate");
			components.Add("Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecLowerBody, StringComparer.OrdinalIgnoreCase))
		{
			components.Add("Wear_Chausses");
			components.Add("Insulation_Moderate");
			components.Add("Armour_LightClothing");
		}
		else if (slotKeys.Contains(EraOutfitSlotSpecUnderlayer, StringComparer.OrdinalIgnoreCase))
		{
			components.Add("Wear_Long-Sleeved_Tunic");
			components.Add("Insulation_Minor");
			components.Add("Armour_LightClothing");
		}

		if (slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase))
		{
			if (lower.Contains("bracer", StringComparison.Ordinal) ||
			    lower.Contains("glove", StringComparison.Ordinal) ||
			    lower.Contains("mitt", StringComparison.Ordinal) ||
			    lower.Contains("sleeve", StringComparison.Ordinal))
			{
				components.Add("Wear_Fingerless_Gloves");
			}
			else if (lower.Contains("harness", StringComparison.Ordinal) ||
			         lower.Contains("axe loop", StringComparison.Ordinal) ||
			         lower.Contains("hook", StringComparison.Ordinal) ||
			         lower.Contains("bowcase", StringComparison.Ordinal) ||
			         lower.Contains("quiver", StringComparison.Ordinal))
			{
				components.Add("Wear_Waist");
				components.Add("Beltable");
			}
			else if (lower.Contains("cross", StringComparison.Ordinal) ||
			         lower.Contains("amulet", StringComparison.Ordinal) ||
			         lower.Contains("bead", StringComparison.Ordinal) ||
			         lower.Contains("pendant", StringComparison.Ordinal))
			{
				components.Add("Wear_Necklace");
			}
		}

		components.Add(materialType == MaterialBehaviourType.Metal ? "Destroyable_HeavyMetal" :
			materialType == MaterialBehaviourType.Wood || materialType == MaterialBehaviourType.Wax ? "Destroyable_Misc" :
			"Destroyable_Clothing");
		if (!string.IsNullOrWhiteSpace(variableColourComponent))
		{
			components.Add(variableColourComponent);
		}

		return components
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static ItemQuality MedievalExplicitOutfitPieceQuality(string pieceName, string socialClassRole)
	{
		var lower = pieceName.ToLowerInvariant();
		if (socialClassRole.Equals("noble", StringComparison.OrdinalIgnoreCase) ||
		    lower.Contains("fine", StringComparison.Ordinal) ||
		    lower.Contains("silver", StringComparison.Ordinal) ||
		    lower.Contains("gold", StringComparison.Ordinal) ||
		    lower.Contains("embroidered", StringComparison.Ordinal) ||
		    lower.Contains("jeweled", StringComparison.Ordinal) ||
		    lower.Contains("silk", StringComparison.Ordinal) ||
		    lower.Contains("rich", StringComparison.Ordinal))
		{
			return ItemQuality.Good;
		}

		return ItemQuality.Standard;
	}

	private static SizeCategory MedievalExplicitOutfitPieceSize(IReadOnlyCollection<string> slotKeys)
	{
		if (slotKeys.Contains(EraOutfitSlotSpecOuterwear, StringComparer.OrdinalIgnoreCase))
		{
			return SizeCategory.Large;
		}

		if (slotKeys.Contains(EraOutfitSlotSpecHeadwear, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecBeltOrSash, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecWornContainer, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecFastenerOrJewellery, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase))
		{
			return SizeCategory.Small;
		}

		return SizeCategory.Normal;
	}

	private static double MedievalExplicitOutfitPieceWeightInGrams(string pieceName, IReadOnlyCollection<string> slotKeys)
	{
		if (slotKeys.Contains(EraOutfitSlotSpecOuterwear, StringComparer.OrdinalIgnoreCase))
		{
			return 1450.0;
		}

		if (slotKeys.Contains(EraOutfitSlotSpecFootwear, StringComparer.OrdinalIgnoreCase))
		{
			return 900.0;
		}

		if (slotKeys.Contains(EraOutfitSlotSpecBodywear, StringComparer.OrdinalIgnoreCase))
		{
			return pieceName.Contains("padded", StringComparison.OrdinalIgnoreCase) ||
			       pieceName.Contains("gambeson", StringComparison.OrdinalIgnoreCase)
				? 1800.0
				: 850.0;
		}

		if (slotKeys.Contains(EraOutfitSlotSpecBeltOrSash, StringComparer.OrdinalIgnoreCase))
		{
			return 360.0;
		}

		if (slotKeys.Contains(EraOutfitSlotSpecWornContainer, StringComparer.OrdinalIgnoreCase))
		{
			return 220.0;
		}

		if (slotKeys.Contains(EraOutfitSlotSpecHeadwear, StringComparer.OrdinalIgnoreCase))
		{
			return 180.0;
		}

		if (slotKeys.Contains(EraOutfitSlotSpecFastenerOrJewellery, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase))
		{
			return 120.0;
		}

		return 420.0;
	}

	private static decimal MedievalExplicitOutfitPieceCost(string pieceName, IReadOnlyCollection<string> slotKeys,
		ItemQuality quality)
	{
		var baseCost = slotKeys.Contains(EraOutfitSlotSpecFastenerOrJewellery, StringComparer.OrdinalIgnoreCase) ? 18.0m :
			slotKeys.Contains(EraOutfitSlotSpecOuterwear, StringComparer.OrdinalIgnoreCase) ? 42.0m :
			slotKeys.Contains(EraOutfitSlotSpecBodywear, StringComparer.OrdinalIgnoreCase) ? 28.0m :
			slotKeys.Contains(EraOutfitSlotSpecFootwear, StringComparer.OrdinalIgnoreCase) ? 18.0m :
			slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase) ? 12.0m :
			10.0m;
		if (pieceName.Contains("silver", StringComparison.OrdinalIgnoreCase) ||
		    pieceName.Contains("gold", StringComparison.OrdinalIgnoreCase) ||
		    pieceName.Contains("jeweled", StringComparison.OrdinalIgnoreCase))
		{
			baseCost *= 3.0m;
		}

		return quality >= ItemQuality.Good ? baseCost * 2.0m : baseCost;
	}

	private static string MedievalExplicitOutfitPieceMarketTag(IReadOnlyCollection<string> slotKeys, ItemQuality quality)
	{
		if (slotKeys.Contains(EraOutfitSlotSpecFastenerOrJewellery, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase))
		{
			return quality >= ItemQuality.Good ? "Market / Clothing / Luxury Clothing" : "Market / Clothing / Standard Clothing";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecFootwear, StringComparer.OrdinalIgnoreCase))
		{
			return "Market / Clothing / Footwear";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecOuterwear, StringComparer.OrdinalIgnoreCase))
		{
			return "Market / Clothing / Winter Clothing";
		}

		return quality >= ItemQuality.Good ? "Market / Clothing / Luxury Clothing" : "Market / Clothing / Standard Clothing";
	}

	private static string MedievalExplicitOutfitPieceFunctionTag(IReadOnlyCollection<string> slotKeys)
	{
		if (slotKeys.Contains(EraOutfitSlotSpecFootwear, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Footwear";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecHeadwear, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Headwear";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecBeltOrSash, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Belts";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecWornContainer, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Pouches";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecFastenerOrJewellery, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecRoleItem, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Jewellery";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecOuterwear, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Outerwear";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecUnderlayer, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Underwear";
		}

		if (slotKeys.Contains(EraOutfitSlotSpecLegOrSockLayer, StringComparer.OrdinalIgnoreCase) ||
		    slotKeys.Contains(EraOutfitSlotSpecLowerBody, StringComparer.OrdinalIgnoreCase))
		{
			return "Functions / Worn Items / Legwear";
		}

		return "Functions / Worn Items / Bodywear";
	}

	private static EraOutfitSpec[] BuildMedievalOutfits()
	{
		return MedievalCultureProfiles
			.SelectMany(culture => MedievalOutfitSexGenderPresentationKeys.SelectMany(sex =>
				MedievalOutfitSocialClassRoleKeys.Select(role => BuildMedievalOutfit(culture, sex, role))))
			.ToArray();
	}

	private static EraOutfitSpec BuildMedievalOutfit(MedievalCultureProfile culture, string sexGenderPresentation,
		string socialClassRole)
	{
		var outfitReference = $"medieval_outfit_{culture.Key}_{sexGenderPresentation}_{socialClassRole}";
		var explicitPieces = MedievalExplicitOutfitPieces
			.Where(x => x.OutfitReference.Equals(outfitReference, StringComparison.OrdinalIgnoreCase))
			.ToArray();
		if (explicitPieces.Length > 0)
		{
			return new EraOutfitSpec(
				outfitReference,
				culture.Key,
				sexGenderPresentation,
				socialClassRole,
				$"{culture.Display} {sexGenderPresentation} {socialClassRole} outfit",
				explicitPieces
					.GroupBy(x => x.SlotKey, StringComparer.OrdinalIgnoreCase)
					.ToDictionary(x => x.Key, x => x.First().StableReference, StringComparer.OrdinalIgnoreCase),
				[]);
		}

		var status = MedievalStatusRoleProfiles.Single(x =>
			x.Key.Equals(MedievalOutfitRoleToStatusRoleKey[socialClassRole], StringComparison.OrdinalIgnoreCase));
		var slots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			[EraOutfitSlotSpecUnderlayer] = MedievalOutfitClothingStableReference(culture, status, "underlayer"),
			[EraOutfitSlotSpecLowerBody] = MedievalOutfitClothingStableReference(culture, status, "legwear"),
			[EraOutfitSlotSpecLegOrSockLayer] = MedievalOutfitClothingStableReference(culture, status, "sockwear"),
			[EraOutfitSlotSpecFootwear] = MedievalOutfitClothingStableReference(culture, status, "footwear"),
			[EraOutfitSlotSpecBodywear] = $"medieval_clothing_{culture.Key}_{status.Key}_{status.GarmentToken}",
			[EraOutfitSlotSpecOuterwear] = MedievalOutfitClothingStableReference(culture, status, "outerwear"),
			[EraOutfitSlotSpecHeadwear] = MedievalOutfitClothingStableReference(culture, status, "headwear"),
			[EraOutfitSlotSpecBeltOrSash] = MedievalOutfitClothingStableReference(culture, status, "belt"),
			[EraOutfitSlotSpecWornContainer] = MedievalOutfitClothingStableReference(culture, status, "pouch"),
			[EraOutfitSlotSpecFastenerOrJewellery] = MedievalOutfitFastenerStableReference(socialClassRole)
		};

		var roleItem = MedievalOutfitRoleItemStableReference(culture, socialClassRole);
		if (!string.IsNullOrWhiteSpace(roleItem))
		{
			slots[EraOutfitSlotSpecRoleItem] = roleItem;
		}

		return new EraOutfitSpec(
			outfitReference,
			culture.Key,
			sexGenderPresentation,
			socialClassRole,
			$"{culture.Display} {sexGenderPresentation} {socialClassRole} outfit",
			slots,
			slots.Keys.ToArray());
	}

	private static string MedievalOutfitClothingStableReference(MedievalCultureProfile culture,
		MedievalStatusRoleProfile status, string wardrobeSlotKey)
	{
		var piece = BuildMedievalWardrobePiece(culture, status, wardrobeSlotKey);
		return $"medieval_clothing_{culture.Key}_{status.Key}_{piece.Token}";
	}

	private static string MedievalOutfitFastenerStableReference(string socialClassRole)
	{
		return socialClassRole switch
		{
			"merchant" => "medieval_jewellery_silver_brooch",
			"noble" => "medieval_jewellery_enamel_disc_brooch",
			"religious" => "medieval_devotional_wooden_rosary",
			_ => "medieval_jewellery_bronze_ring_pin"
		};
	}

	private static string? MedievalOutfitRoleItemStableReference(MedievalCultureProfile culture, string socialClassRole)
	{
		return socialClassRole switch
		{
			"merchant" => $"medieval_writing_{culture.Key}_office_bundle",
			"religious" => $"medieval_devotional_{culture.Key}_pilgrim_token",
			"military" => $"medieval_military_{culture.Key}_sidearm_harness",
			_ => null
		};
	}

	internal static IReadOnlyCollection<string> HistoricFoundationStableReferencesForTesting =>
		HistoricFoundationItemSpecs()
			.Select(x => x.StableReference)
			.ToArray();

	internal static IReadOnlyCollection<EraItemSpecTestData> HistoricFoundationItemSpecsForTesting =>
		HistoricFoundationItemSpecs()
			.Select(x => new EraItemSpecTestData(
				x.StableReference,
				x.Noun,
				x.ShortDescription,
				x.FullDescription,
				x.Size,
				x.Quality,
				x.WeightInGrams,
				x.Cost,
				x.Material,
				x.MaterialType,
				x.Tags,
				x.Components,
				x.BuilderNotes))
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalCultureKeysForTesting =>
		MedievalCultureProfiles
			.Select(x => x.Key)
			.ToArray();

	internal static IReadOnlyDictionary<string, IReadOnlyCollection<string>> MedievalExplicitCultureStableReferencesForTesting =>
		MedievalExplicitCultureCatalogues
			.ToDictionary(
				x => x.CultureKey,
				x => (IReadOnlyCollection<string>)x.StableReferencesByGroup
					.SelectMany(group => group.Value)
					.ToArray(),
				StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> MedievalExplicitCultureStableReferenceGroupsForTesting =>
		MedievalExplicitCultureCatalogues
			.ToDictionary(
				x => x.CultureKey,
				x => x.StableReferencesByGroup,
				StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyCollection<(string CultureKey, string Group, string StableReference, string ShortDescription, MedievalCultureCatalogueReferenceStatus Status, string? ImplementationStableReference, string? Reason, string? CraftCoverageExemption)> MedievalExplicitCultureCatalogueEntriesForTesting =>
		MedievalExplicitCultureCatalogues
			.SelectMany(x => x.Entries.Select(entry => (entry.CultureKey, entry.Group, entry.StableReference,
				entry.ShortDescription, entry.Status, entry.ImplementationStableReference, entry.Reason,
				entry.CraftCoverageExemption)))
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalStatusRoleKeysForTesting =>
		MedievalStatusRoleProfiles
			.Select(x => x.Key)
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalWardrobeSlotKeysForTesting =>
		MedievalWardrobeSlotKeys
			.ToArray();

	internal static IReadOnlyCollection<(string Key, bool RequiredForAllOutfits, IReadOnlyCollection<string> RequiredForRoles)> MedievalOutfitSlotsForTesting =>
		EraOutfitSlotSpecs
			.Select(x => (x.Key, x.RequiredForAllOutfits, (IReadOnlyCollection<string>)x.RequiredForRoles.ToArray()))
			.ToArray();

	internal static (string EraKey, string EraRootTag, string CultureTagRoot, string? StatusOrSocialRoleTagRoot, bool CompleteOutfitCataloguesRequired, bool GenericBaselineWardrobeGenerationAllowed, bool PlayerFacingDescriptionsMayIncludeCultureNames, IReadOnlyCollection<string> SlotKeys) MedievalEraConfigurationForTesting =>
		(MedievalEraConfiguration.EraKey,
			MedievalEraConfiguration.EraRootTag,
			MedievalEraConfiguration.CultureTagRoot,
			MedievalEraConfiguration.StatusOrSocialRoleTagRoot,
			MedievalEraConfiguration.CompleteOutfitCataloguesRequired,
			MedievalEraConfiguration.GenericBaselineWardrobeGenerationAllowed,
			MedievalEraConfiguration.PlayerFacingDescriptionsMayIncludeCultureNames,
			MedievalEraConfiguration.ClothingSlotDefinitions.Select(x => x.Key).ToArray());

	internal static IReadOnlyCollection<string> MedievalOutfitSexGenderPresentationKeysForTesting =>
		MedievalOutfitSexGenderPresentationKeys
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalOutfitSocialClassRoleKeysForTesting =>
		MedievalOutfitSocialClassRoleKeys
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalNorthAtlanticOutfitCultureKeysForTesting =>
		MedievalNorthAtlanticOutfitCultureKeys
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalContinentalWesternOutfitCultureKeysForTesting =>
		MedievalContinentalWesternOutfitCultureKeys
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalEasternOutfitCultureKeysForTesting =>
		MedievalEasternOutfitCultureKeys
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalExplicitOutfitCultureKeysForTesting =>
		MedievalExplicitOutfitCultureKeys
			.ToArray();

	internal static IReadOnlyCollection<(string OutfitReference, string CultureKey, string SexGenderPresentation, string SocialClassRole, string DisplayName, IReadOnlyDictionary<string, string> SlotItemStableReferences, IReadOnlyCollection<string> IntentionallySharedOrGenericSlots)> MedievalOutfitsForTesting =>
		MedievalOutfits
			.Select(x => (x.OutfitReference, x.CultureKey, x.SexGenderPresentation, x.SocialClassRole, x.DisplayName,
				x.SlotItemStableReferences, x.IntentionallySharedOrGenericSlots))
			.ToArray();

	internal static IReadOnlyCollection<(string OutfitReference, string CultureKey, string SexGenderPresentation, string SocialClassRole, string SlotKey, string PieceName, string StableReference, bool CultureSpecificOrClusterSpecific)> MedievalExplicitOutfitPiecesForTesting =>
		MedievalExplicitOutfitPieces
			.Select(x => (x.OutfitReference, x.CultureKey, x.SexGenderPresentation, x.SocialClassRole, x.SlotKey,
				x.PieceName, x.StableReference, x.CultureSpecificOrClusterSpecific))
			.ToArray();

	internal static IReadOnlyCollection<MedievalAuthoredOutfitPieceTestData> MedievalAuthoredOutfitPiecesForTesting =>
		MedievalExplicitOutfitClothingPieces()
			.Select(piece =>
			{
				var item = piece.Item;
				return new MedievalAuthoredOutfitPieceTestData(
					item.StableReference,
					piece.OutfitReference,
					piece.CultureKey,
					piece.SexGenderPresentation,
					piece.SocialClassRole,
					piece.SlotKeys,
					piece.PieceName,
					item.Noun,
					item.ShortDescription,
					item.FullDescription,
					item.Material,
					item.MaterialType,
					item.Quality,
					item.Size,
					item.WeightInGrams,
					item.Cost,
					piece.VariableColourComponent,
					piece.ColourVariablesUsed,
					item.Components,
					piece.Craft.Inputs.Select(x => x.Definition).ToArray(),
					piece.Craft.Tools.Select(x => x.Definition).ToArray(),
					piece.IntentionallySharedOrGeneric);
			})
			.ToArray();

	internal static IReadOnlyCollection<EraItemSpecTestData> MedievalExplicitOutfitPieceItemSpecsForTesting =>
		MedievalExplicitOutfitPieceItemSpecs()
			.Select(x => new EraItemSpecTestData(
				x.StableReference,
				x.Noun,
				x.ShortDescription,
				x.FullDescription,
				x.Size,
				x.Quality,
				x.WeightInGrams,
				x.Cost,
				x.Material,
				x.MaterialType,
				x.Tags,
				x.Components,
				x.BuilderNotes))
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalOutfitReferencedItemStableReferencesForTesting =>
		MedievalOutfits
			.SelectMany(x => x.SlotItemStableReferences.Values)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalItemStableReferencesForTesting =>
		MedievalAllItemSpecs()
			.Select(x => x.StableReference)
			.ToArray();

	internal static IReadOnlyCollection<EraItemSpecTestData> MedievalItemSpecsForTesting =>
		MedievalAllItemSpecs()
			.Select(x => new EraItemSpecTestData(
				x.StableReference,
				x.Noun,
				x.ShortDescription,
				x.FullDescription,
				x.Size,
				x.Quality,
				x.WeightInGrams,
				x.Cost,
				x.Material,
				x.MaterialType,
				x.Tags,
				x.Components,
				x.BuilderNotes))
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalLiveComponentStableReferencesForTesting =>
		MedievalAllItemSpecs()
			.Where(x => x.Components.Any(component =>
				component.Contains("SealStamp", StringComparison.OrdinalIgnoreCase) ||
				component.Contains("Sealable", StringComparison.OrdinalIgnoreCase) ||
				component.Contains("MeasuringInstrument", StringComparison.OrdinalIgnoreCase) ||
				component.Contains("OfferingReceiver", StringComparison.OrdinalIgnoreCase)))
			.Select(x => x.StableReference)
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalDeferredComponentGapStableReferencesForTesting =>
		MedievalComponentGapPropItemSpecs()
			.Select(x => x.StableReference)
			.Append("medieval_surveyor_measuring_rope")
			.ToArray();

	internal static IReadOnlyCollection<string> MedievalRequiredComponentNamesForTesting =>
		MedievalAllItemSpecs()
			.Concat(HistoricFoundationItemSpecs())
			.SelectMany(x => x.Components)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

	private static IReadOnlyList<EraItemSpec> MedievalAllItemSpecs()
	{
		return MedievalClothingItemSpecs()
			.Concat(MedievalEquipmentItemSpecs())
			.Concat(MedievalHouseholdToolItemSpecs())
			.Concat(MedievalFoodAndBeverageItemSpecs())
			.Concat(MedievalFurnitureContainerItemSpecs())
			.Concat(MedievalJewelleryDevotionalItemSpecs())
			.Concat(MedievalMedicalApothecaryItemSpecs())
			.Concat(MedievalWritingAdministrationItemSpecs())
			.Concat(MedievalComponentGapPropItemSpecs())
			.Concat(MedievalRepairKitItemSpecs())
			.ToArray();
	}

	private static string SafeMedievalTagName(string text)
	{
		return text
			.Replace("/", "-", StringComparison.Ordinal)
			.Replace("  ", " ", StringComparison.Ordinal)
			.Trim();
	}

	private static string StableReferenceToken(string text)
	{
		var normalised = new string(text
			.Select(x => char.IsLetterOrDigit(x) ? char.ToLowerInvariant(x) : '_')
			.ToArray());
		return string.Join("_", normalised.Split('_', StringSplitOptions.RemoveEmptyEntries));
	}

	internal void SeedMedievalItemsForTesting(FuturemudDatabaseContext context)
	{
		if (!ReferenceEquals(_context, context))
		{
			_context = context;
			InitialiseDependencies();
		}

		SeedHistoricCommonWorkshopItems();
		SeedMedievalClothing();
		SeedMedievalHouseholdCraftTools();
		SeedMedievalWritingAdministrationAndDocuments();
		SeedMedievalMedicalAndApothecaryItems();
		SeedMedievalJewelleryAndDevotionalGoods();
		SeedMedievalArmour();
		SeedMedievalContainers();
		SeedMedievalDoorsLocksAndStrongboxes();
		SeedMedievalRepairKits();
		SeedMedievalHouseholdFurniture();
		SeedMedievalWeaponsShieldsAccessories();
		SeedMedievalFoodAndBeverageItems();
		SeedMedievalComponentGapItems();
		_context!.SaveChanges();
	}
}
