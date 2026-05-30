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

	private sealed record MedievalItemSpec(
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
		string[] Tags,
		string[] Components,
		string? BuilderNotes = null,
		string? MorphToUniqueReference = null,
		string? MorphEmote = null,
		TimeSpan? MorphTimer = null,
		string? DestroyedItemUniqueReference = null);

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

	private sealed record MedievalOutfitSlot(
		string Key,
		string Display,
		bool RequiredForAllOutfits,
		string[] RequiredForRoles);

	private sealed record MedievalOutfitSpec(
		string OutfitReference,
		string CultureKey,
		string SexGenderPresentation,
		string SocialClassRole,
		string DisplayName,
		IReadOnlyDictionary<string, string> SlotItemStableReferences,
		IReadOnlyCollection<string> IntentionallySharedOrGenericSlots);

	private sealed record MedievalOutfitPieceSpec(
		string OutfitReference,
		string CultureKey,
		string SexGenderPresentation,
		string SocialClassRole,
		string SlotKey,
		string PieceName,
		string StableReference,
		bool CultureSpecificOrClusterSpecific);

	private sealed record MedievalAuthoredOutfitPieceSpec(
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
		string[] Tags,
		string[] Components,
		string[] CraftInputs,
		string[] CraftTools,
		string? ImplementationNotes,
		string? AuthoringGuidelineNotes);

	private sealed record MedievalAuthoredOutfitPieceCraftSpec(
		string StableReference,
		string[] Inputs,
		string[] Tools);

	internal sealed record MedievalAuthoredOutfitPieceTestData(
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
		IReadOnlyCollection<string> ColourVariablesUsed,
		IReadOnlyCollection<string> Tags,
		IReadOnlyCollection<string> Components,
		IReadOnlyCollection<string> CraftInputs,
		IReadOnlyCollection<string> CraftTools,
		string? ImplementationNotes,
		string? AuthoringGuidelineNotes);

	internal sealed record MedievalItemSpecTestData(
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

	private const string MedievalOutfitSlotUnderlayer = "underlayer";
	private const string MedievalOutfitSlotLowerBody = "lower_body";
	private const string MedievalOutfitSlotLegOrSockLayer = "leg_or_sock_layer";
	private const string MedievalOutfitSlotFootwear = "footwear";
	private const string MedievalOutfitSlotBodywear = "bodywear";
	private const string MedievalOutfitSlotOuterwear = "outerwear";
	private const string MedievalOutfitSlotHeadwear = "headwear";
	private const string MedievalOutfitSlotBeltOrSash = "belt_or_sash";
	private const string MedievalOutfitSlotWornContainer = "worn_container";
	private const string MedievalOutfitSlotFastenerOrJewellery = "fastener_or_jewellery";
	private const string MedievalOutfitSlotRoleItem = "role_item";

	private static readonly MedievalOutfitSlot[] MedievalOutfitSlots =
	[
		new(MedievalOutfitSlotUnderlayer, "Underlayer", true, []),
		new(MedievalOutfitSlotLowerBody, "Lower Body", true, []),
		new(MedievalOutfitSlotLegOrSockLayer, "Leg/Sock Layer", true, []),
		new(MedievalOutfitSlotFootwear, "Footwear", true, []),
		new(MedievalOutfitSlotBodywear, "Bodywear", true, []),
		new(MedievalOutfitSlotOuterwear, "Outerwear", true, []),
		new(MedievalOutfitSlotHeadwear, "Headwear", true, []),
		new(MedievalOutfitSlotBeltOrSash, "Belt or Sash", true, []),
		new(MedievalOutfitSlotWornContainer, "Worn Container", true, []),
		new(MedievalOutfitSlotFastenerOrJewellery, "Fastener/Jewellery", true, []),
		new(MedievalOutfitSlotRoleItem, "Role Item", false, ["merchant", "religious", "military"])
	];

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

	private static readonly MedievalOutfitPieceSpec[] MedievalExplicitOutfitPieces =
		BuildMedievalExplicitOutfitPieces();

	private static readonly MedievalOutfitSpec[] MedievalOutfits = BuildMedievalOutfits();


	private static readonly IReadOnlyDictionary<string, MedievalAuthoredOutfitPieceSpec>
		MedievalAuthoredOutfitPieces =
			BuildMedievalAuthoredOutfitPieceDictionary(BuildMedievalAuthoredOutfitPieces());

	private static readonly IReadOnlyDictionary<string, MedievalAuthoredOutfitPieceCraftSpec>
		MedievalAuthoredOutfitPieceCraftSpecs =
			MedievalAuthoredOutfitPieces.Values
				.Where(x => x.CraftInputs.Length > 0 || x.CraftTools.Length > 0)
				.Select(x => new MedievalAuthoredOutfitPieceCraftSpec(
					x.StableReference,
					x.CraftInputs,
					x.CraftTools))
				.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);

	private static IReadOnlyList<MedievalItemSpec> HistoricFoundationItemSpecs()
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

	private static IReadOnlyList<MedievalItemSpec> MedievalClothingItemSpecs()
	{
		return MedievalExplicitOutfitPieceItemSpecs();
	}
	private static IReadOnlyList<MedievalItemSpec> MedievalEquipmentItemSpecs()
	{
		var specs = new List<MedievalItemSpec>();
		foreach (var culture in MedievalCultureProfiles)
		{
			specs.Add(new MedievalItemSpec(
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
			specs.Add(new MedievalItemSpec(
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
			specs.Add(new MedievalItemSpec(
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
			new MedievalItemSpec(
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
			new MedievalItemSpec(
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

	private static IReadOnlyList<MedievalItemSpec> MedievalEquipmentAccessoryItemSpecs(MedievalCultureProfile culture)
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

	private static IReadOnlyList<MedievalItemSpec> MedievalHouseholdToolItemSpecs()
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

	private static IReadOnlyList<MedievalItemSpec> MedievalFoodAndBeverageItemSpecs()
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

	private static IReadOnlyList<MedievalItemSpec> MedievalFoodwayItemSpecs(MedievalCultureProfile culture)
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

	private static IReadOnlyList<MedievalItemSpec> MedievalFurnitureContainerItemSpecs()
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

	private static IReadOnlyList<MedievalItemSpec> MedievalJewelleryDevotionalItemSpecs()
	{
		return MedievalCultureProfiles
			.Select(culture => new MedievalItemSpec(
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

	private static IReadOnlyList<MedievalItemSpec> MedievalMedicalApothecaryItemSpecs()
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

	private static IReadOnlyList<MedievalItemSpec> MedievalWritingAdministrationItemSpecs()
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

	private static IReadOnlyList<MedievalItemSpec> MedievalCultureAdministrationItemSpecs(MedievalCultureProfile culture)
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

	private static IReadOnlyList<MedievalItemSpec> MedievalComponentGapPropItemSpecs()
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

	private static IReadOnlyList<MedievalItemSpec> MedievalRepairKitItemSpecs()
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

	private static MedievalOutfitPieceSpec[] BuildMedievalExplicitOutfitPieces()
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
						return new MedievalOutfitPieceSpec(
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
					(MedievalOutfitSlotUnderlayer, pieces[0]),
					(MedievalOutfitSlotLowerBody, pieces[1]),
					(MedievalOutfitSlotLegOrSockLayer, pieces[1]),
					(MedievalOutfitSlotFootwear, pieces[2]),
					(MedievalOutfitSlotBodywear, pieces[3]),
					(MedievalOutfitSlotOuterwear, pieces[4]),
					(MedievalOutfitSlotHeadwear, pieces[5]),
					(MedievalOutfitSlotBeltOrSash, pieces[6]),
					(MedievalOutfitSlotWornContainer, pieces[7]),
					(MedievalOutfitSlotFastenerOrJewellery, pieces[8]),
					(MedievalOutfitSlotRoleItem, pieces[9])
				];
			}

			return AssignMedievalOutfitPieces(
				pieces,
				[
					MedievalOutfitSlotUnderlayer,
					MedievalOutfitSlotLowerBody,
					MedievalOutfitSlotLegOrSockLayer,
					MedievalOutfitSlotFootwear,
					MedievalOutfitSlotBodywear,
					MedievalOutfitSlotOuterwear,
					MedievalOutfitSlotHeadwear,
					MedievalOutfitSlotBeltOrSash,
					MedievalOutfitSlotWornContainer,
					MedievalOutfitSlotFastenerOrJewellery
				]);
		}

		if (pieces.Length >= 11)
		{
			return AssignMedievalOutfitPieces(
				pieces,
				[
					MedievalOutfitSlotUnderlayer,
					MedievalOutfitSlotLowerBody,
					MedievalOutfitSlotLegOrSockLayer,
					MedievalOutfitSlotFootwear,
					MedievalOutfitSlotBodywear,
					MedievalOutfitSlotOuterwear,
					MedievalOutfitSlotHeadwear,
					MedievalOutfitSlotBeltOrSash,
					MedievalOutfitSlotWornContainer,
					MedievalOutfitSlotFastenerOrJewellery,
					MedievalOutfitSlotRoleItem
				]);
		}

		if (pieces.Length == 9 && IsMedievalOutfitFootwearPiece(pieces[2]))
		{
			return
			[
				(MedievalOutfitSlotUnderlayer, pieces[0]),
				(MedievalOutfitSlotLowerBody, pieces[1]),
				(MedievalOutfitSlotLegOrSockLayer, pieces[1]),
				(MedievalOutfitSlotFootwear, pieces[2]),
				(MedievalOutfitSlotBodywear, pieces[3]),
				(MedievalOutfitSlotOuterwear, pieces[4]),
				(MedievalOutfitSlotHeadwear, pieces[5]),
				(MedievalOutfitSlotBeltOrSash, pieces[6]),
				(MedievalOutfitSlotWornContainer, pieces[7]),
				(MedievalOutfitSlotFastenerOrJewellery, pieces[8]),
				(MedievalOutfitSlotRoleItem, pieces[8])
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
				(MedievalOutfitSlotUnderlayer, pieces[0]),
				(MedievalOutfitSlotLowerBody, pieces[1]),
				(MedievalOutfitSlotLegOrSockLayer, pieces[1]),
				(MedievalOutfitSlotFootwear, pieces[2]),
				(MedievalOutfitSlotBodywear, pieces[3]),
				(MedievalOutfitSlotOuterwear, pieces[4]),
				(MedievalOutfitSlotHeadwear, pieces[5]),
				(MedievalOutfitSlotBeltOrSash, pieces[6]),
				(MedievalOutfitSlotWornContainer, pieces[7]),
				(MedievalOutfitSlotFastenerOrJewellery, pieces[8]),
				(MedievalOutfitSlotRoleItem, pieces[9])
			];
		}

		return
		[
			(MedievalOutfitSlotUnderlayer, pieces[0]),
			(MedievalOutfitSlotLowerBody, pieces[1]),
			(MedievalOutfitSlotLegOrSockLayer, pieces[2]),
			(MedievalOutfitSlotFootwear, pieces[3]),
			(MedievalOutfitSlotBodywear, pieces[4]),
			(MedievalOutfitSlotOuterwear, pieces[5]),
			(MedievalOutfitSlotHeadwear, pieces[6]),
			(MedievalOutfitSlotBeltOrSash, pieces[7]),
			(MedievalOutfitSlotWornContainer, pieces[8]),
			(MedievalOutfitSlotFastenerOrJewellery, pieces[9]),
			(MedievalOutfitSlotRoleItem, pieces[9])
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
		return MedievalOutfitSlots
			.Single(x => x.Key.Equals(MedievalOutfitSlotRoleItem, StringComparison.OrdinalIgnoreCase))
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

	private static IReadOnlyDictionary<string, MedievalAuthoredOutfitPieceSpec> BuildMedievalAuthoredOutfitPieceDictionary(
		IReadOnlyList<MedievalAuthoredOutfitPieceSpec> rows)
	{
		var duplicates = rows
			.GroupBy(x => x.StableReference, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToArray();
		if (duplicates.Length > 0)
		{
			throw new ApplicationException($"Duplicate authored medieval outfit-piece rows: {string.Join(", ", duplicates)}");
		}

		var authored = rows.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);
		var explicitStableReferences = MedievalExplicitOutfitPieces
			.Select(x => x.StableReference)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var missing = explicitStableReferences
			.Where(x => !authored.ContainsKey(x))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();
		if (missing.Length > 0)
		{
			throw new ApplicationException($"Missing authored medieval outfit-piece rows: {string.Join(", ", missing)}");
		}

		var stale = authored.Keys
			.Where(x => !explicitStableReferences.Contains(x))
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();
		if (stale.Length > 0)
		{
			throw new ApplicationException($"Authored medieval outfit-piece rows no longer used by the outfit catalogue: {string.Join(", ", stale)}");
		}

		return authored;
	}

	private static MedievalAuthoredOutfitPieceSpec AuthoredOutfitPiece(
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
		string[] tags,
		string[] components,
		string[] craftInputs,
		string[] craftTools,
		string? implementationNotes,
		string? authoringGuidelineNotes)
	{
		var explicitPieces = MedievalExplicitOutfitPieces
			.Where(x => x.StableReference.Equals(stableReference, StringComparison.OrdinalIgnoreCase))
			.ToArray();
		if (explicitPieces.Length == 0)
		{
			throw new ApplicationException($"Authored medieval outfit-piece row {stableReference} does not match any explicit outfit-piece stable reference.");
		}

		var first = explicitPieces[0];
		if (!first.OutfitReference.Equals(outfitReference, StringComparison.OrdinalIgnoreCase) ||
		    !first.CultureKey.Equals(cultureKey, StringComparison.OrdinalIgnoreCase) ||
		    !first.SexGenderPresentation.Equals(sexGenderPresentation, StringComparison.OrdinalIgnoreCase) ||
		    !first.SocialClassRole.Equals(socialClassRole, StringComparison.OrdinalIgnoreCase) ||
		    !first.PieceName.Equals(pieceName, StringComparison.OrdinalIgnoreCase) ||
		    !explicitPieces.Any(x => x.SlotKey.Equals(slotKey, StringComparison.OrdinalIgnoreCase)))
		{
			throw new ApplicationException($"Authored medieval outfit-piece row {stableReference} does not match the explicit outfit-piece catalogue metadata.");
		}

		return new MedievalAuthoredOutfitPieceSpec(
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
			tags,
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

	private static string[] BuildMedievalAuthoredOutfitPieceComponents(
		MedievalAuthoredOutfitPieceSpec authoredSpec)
	{
		var components = authoredSpec.Components.ToList();

		void AddIfMissing(string component)
		{
			if (!components.Contains(component, StringComparer.OrdinalIgnoreCase))
			{
				components.Add(component);
			}
		}

		AddIfMissing("Holdable");
		if (!string.IsNullOrWhiteSpace(authoredSpec.VariableColourComponent))
		{
			AddIfMissing(authoredSpec.VariableColourComponent);
		}

		return components
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private static string BuildMedievalAuthoredOutfitPieceBuilderNotes(
		MedievalAuthoredOutfitPieceSpec authoredSpec)
	{
		var lines = new List<string>
		{
			"Authored explicit medieval outfit piece.",
			$"Outfit reference: {authoredSpec.OutfitReference}.",
			$"Culture key: {authoredSpec.CultureKey}.",
			$"Sex/gender presentation: {authoredSpec.SexGenderPresentation}.",
			$"Social class/role: {authoredSpec.SocialClassRole}.",
			$"Slot: {authoredSpec.SlotKey}.",
			$"Piece target: {authoredSpec.PieceName}."
		};

		if (!string.IsNullOrWhiteSpace(authoredSpec.AuthoringGuidelineNotes))
		{
			lines.Add($"Authoring notes: {authoredSpec.AuthoringGuidelineNotes}.");
		}

		if (!string.IsNullOrWhiteSpace(authoredSpec.ImplementationNotes))
		{
			lines.Add($"Implementation notes: {authoredSpec.ImplementationNotes}.");
		}

		return string.Join("\n", lines);
	}

	private static IReadOnlyList<MedievalItemSpec> MedievalExplicitOutfitPieceItemSpecs()
	{
		return MedievalExplicitOutfitPieces
			.GroupBy(x => x.StableReference, StringComparer.OrdinalIgnoreCase)
			.Select(group => BuildMedievalExplicitOutfitPieceItemSpec(group.First(), group.Select(x => x.SlotKey).ToArray()))
			.ToArray();
	}

	private static MedievalItemSpec BuildMedievalExplicitOutfitPieceItemSpec(MedievalOutfitPieceSpec piece,
		IReadOnlyCollection<string> slotKeys)
	{
		if (!MedievalAuthoredOutfitPieces.TryGetValue(piece.StableReference, out var authoredSpec))
		{
			throw new ApplicationException($"Cannot build explicit medieval outfit piece {piece.StableReference} without an authored row.");
		}

		return new MedievalItemSpec(
			piece.StableReference,
			authoredSpec.Noun,
			authoredSpec.ShortDescription,
			authoredSpec.FullDescription,
			authoredSpec.Size,
			authoredSpec.Quality,
			authoredSpec.WeightInGrams,
			authoredSpec.Cost,
			authoredSpec.Material,
			authoredSpec.MaterialType,
			authoredSpec.Tags,
			BuildMedievalAuthoredOutfitPieceComponents(authoredSpec),
			BuildMedievalAuthoredOutfitPieceBuilderNotes(authoredSpec));
	}

	private static MedievalOutfitSpec[] BuildMedievalOutfits()
	{
		return MedievalCultureProfiles
			.SelectMany(culture => MedievalOutfitSexGenderPresentationKeys.SelectMany(sex =>
				MedievalOutfitSocialClassRoleKeys.Select(role => BuildMedievalOutfit(culture, sex, role))))
			.ToArray();
	}

	private static MedievalOutfitSpec BuildMedievalOutfit(MedievalCultureProfile culture, string sexGenderPresentation,
		string socialClassRole)
	{
		var outfitReference = $"medieval_outfit_{culture.Key}_{sexGenderPresentation}_{socialClassRole}";
		var explicitPieces = MedievalExplicitOutfitPieces
			.Where(x => x.OutfitReference.Equals(outfitReference, StringComparison.OrdinalIgnoreCase))
			.ToArray();
		if (explicitPieces.Length == 0)
		{
			throw new ApplicationException($"Medieval outfit {outfitReference} has no explicit authored outfit pieces.");
		}

		return new MedievalOutfitSpec(
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
	internal static IReadOnlyCollection<string> HistoricFoundationStableReferencesForTesting =>
		HistoricFoundationItemSpecs()
			.Select(x => x.StableReference)
			.ToArray();

	internal static IReadOnlyCollection<MedievalItemSpecTestData> HistoricFoundationItemSpecsForTesting =>
		HistoricFoundationItemSpecs()
			.Select(x => new MedievalItemSpecTestData(
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

	internal static IReadOnlyCollection<(string Key, bool RequiredForAllOutfits, IReadOnlyCollection<string> RequiredForRoles)> MedievalOutfitSlotsForTesting =>
		MedievalOutfitSlots
			.Select(x => (x.Key, x.RequiredForAllOutfits, (IReadOnlyCollection<string>)x.RequiredForRoles.ToArray()))
			.ToArray();

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
		MedievalAuthoredOutfitPieces.Values
			.Select(x => new MedievalAuthoredOutfitPieceTestData(
				x.StableReference,
				x.OutfitReference,
				x.CultureKey,
				x.SexGenderPresentation,
				x.SocialClassRole,
				x.SlotKey,
				x.PieceName,
				x.Noun,
				x.ShortDescription,
				x.FullDescription,
				x.Material,
				x.MaterialType,
				x.Quality,
				x.Size,
				x.WeightInGrams,
				x.Cost,
				x.VariableColourComponent,
				x.ColourVariablesUsed,
				x.Tags,
				x.Components,
				x.CraftInputs,
				x.CraftTools,
				x.ImplementationNotes,
				x.AuthoringGuidelineNotes))
			.ToArray();

	internal static void MissingAuthoredOutfitPieceRowsThrowForTesting()
	{
		BuildMedievalAuthoredOutfitPieceDictionary(BuildMedievalAuthoredOutfitPieces()
			.Skip(1)
			.ToArray());
	}

		internal static IReadOnlyCollection<MedievalItemSpecTestData> MedievalExplicitOutfitPieceItemSpecsForTesting =>
		MedievalExplicitOutfitPieceItemSpecs()
			.Select(x => new MedievalItemSpecTestData(
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

	internal static IReadOnlyCollection<MedievalItemSpecTestData> MedievalItemSpecsForTesting =>
		MedievalAllItemSpecs()
			.Select(x => new MedievalItemSpecTestData(
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

	private static IReadOnlyList<MedievalItemSpec> MedievalAllItemSpecs()
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
