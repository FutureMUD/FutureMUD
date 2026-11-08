using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MudSharp.Database;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Formats.Tar;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using CraftPhase = MudSharp.Models.CraftPhase;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	internal sealed record CraftPhaseSpec
	{
		public int Seconds { get; init; }
		public string Echo { get; init; } = string.Empty;
		public string FailEcho { get; init; } = string.Empty;
		public ExertionLevel Exertion { get; init; } = ExertionLevel.Stasis;
		public double Stamina { get; init; }
	}

	internal sealed record CraftInputSpec(
		string ImportText,
		string InputType,
		string Details,
		IReadOnlyList<string> Options,
		double QualityWeight)
	{
		[System.Text.Json.Serialization.JsonIgnore]
		public CatalogueCommodityInput? CatalogueInput { get; init; }
		[System.Text.Json.Serialization.JsonIgnore]
		public CatalogueRecipeInput? RecipeInput { get; init; }
	}

	internal sealed record CraftToolSpec(
		string ImportText,
		string ToolType,
		string Details,
		IReadOnlyList<string> Options,
		double QualityWeight,
		bool UseToolDuration)
	{
		[System.Text.Json.Serialization.JsonIgnore]
		public CatalogueTagTool? CatalogueTool { get; init; }
		[System.Text.Json.Serialization.JsonIgnore]
		public CatalogueRecipeTool? RecipeTool { get; init; }
	}

	internal sealed record CraftProductSpec(
		string ImportText,
		string ProductType,
		string Details,
		IReadOnlyList<string> Options,
		bool IsFailProduct,
		int? MaterialDefiningInputIndex)
	{
		[System.Text.Json.Serialization.JsonIgnore]
		public CatalogueSimpleProduct? CatalogueProduct { get; init; }
		[System.Text.Json.Serialization.JsonIgnore]
		public CatalogueRecipeProduct? RecipeProduct { get; init; }
	}

	internal sealed record CraftDefinitionSpec
	{
		public string? StableReference { get; init; }
		public bool PreserveAuthoredText { get; init; }
		public string Name { get; init; } = string.Empty;
		public string Category { get; init; } = string.Empty;
		public string Blurb { get; init; } = string.Empty;
		public string Action { get; init; } = string.Empty;
		public string ActiveCraftItemSdesc { get; init; } = string.Empty;
		public FutureProg? AppearProg { get; init; }
		public FutureProg? CanUseProg { get; init; }
		public FutureProg? WhyCannotUseProg { get; init; }
		public FutureProg? OnStartProg { get; init; }
		public FutureProg? OnFinishProg { get; init; }
		public FutureProg? OnCancelProg { get; init; }
		public TraitDefinition? Trait { get; init; }
		public Difficulty Difficulty { get; init; } = Difficulty.Normal;
		public Outcome Threshold { get; init; } = Outcome.MinorFail;
		public int FreeChecks { get; init; } = 5;
		public int FailPhase { get; init; } = 1;
		public bool Interruptable { get; init; }
		public IReadOnlyList<CraftPhaseSpec> Phases { get; init; } = [];
		public IReadOnlyList<CraftInputSpec> Inputs { get; init; } = [];
		public IReadOnlyList<CraftToolSpec> Tools { get; init; } = [];
		public IReadOnlyList<CraftProductSpec> Products { get; init; } = [];
		public IReadOnlyList<CraftProductSpec> FailProducts { get; init; } = [];
	}

	// Exact names still win; these are stock skill package variants and legacy spellings in priority order.
	private static readonly IReadOnlyList<IReadOnlyList<string>> StockSkillPackageTraitAliases =
	[
		["Armourcrafting", "Armourer", "Armour Crafting", "Armour Craft", "Armorcrafting", "Armorer", "Armor Crafting", "Armor Craft"],
		["Weaponcrafting", "Weaponsmith", "Weapon Crafting", "Weapon Craft", "Weaponcraft", "Weaponsmithing"],
		["Blacksmithing", "Metalcraft", "Metalworking", "Metal Work", "Metalwork", "Smithing"],
		["Silversmithing", "Silversmith", "Silver Smithing", "Silver Smith"],
		["Goldsmithing", "Goldsmith", "Gold Smithing", "Gold Smith"],
		["Bowmaking", "Bowyer", "Bow Making", "Bowyer Craft"],
		["Fletching", "Fletcher"],
		["Pottery", "Potter"],
		["Bonecarving", "Bonecarver"],
		["Weaving", "Weaver"],
		["Spinning", "Spinner"],
		["Threshing", "Thresher"],
		["Milling", "Miller"],
		["Baking", "Baker"],
		["Dyeing", "Dyecraft", "Dye Craft", "Dyer"],
		["Glassworking", "Glasswork", "Glass Work", "Glassworker"],
		["Glassblowing", "Glassblower", "Glass Blowing"],
		["Gemcraft", "Gem Craft"],
		["Lapidary", "Gemcutting", "Gem Cutting"],
		["Perfumery", "Perfumer"],
		["Brewing", "Brewer"],
		["Distilling", "Distiller"],
		["Cooking", "Cookery", "Cook"],
		["Carpentry", "Woodcraft", "Woodworking", "Wood Work", "Woodwork", "Carpenter", "Joinery"],
		["Basketry", "Basketmaker", "Basket Making"],
		["Coopering", "Cooper"],
		["Ropemaking", "Ropemaker", "Rope Making"],
		["Fulling", "Fuller"],
		["Lacquerwork", "Lacquerer", "Lacquer Work"],
		["Lumberjacking", "Lumberjack"],
		["Masonry", "Stonecraft", "Stoneworking", "Stone Work", "Stonework", "Mason"],
		["Scrimshawing", "Scrimshaw"],
		["Tailoring", "Textilecraft", "Textile Craft", "Textilework", "Textile Work", "Tailor"],
		["Mechanics", "Mechanic"],
		["Cobbling", "Cobbler"],
		["Locksmithing", "Locksmith"],
		["Wheelmaking", "Wheelwright", "Wheel Making"],
		["Candlemaking", "Candlery", "Candle Making"],
		["Leathermaking", "Hideworking", "Leatherworking", "Leather Work", "Leatherwork", "Leather Making", "Hide Work"],
		["Parchmentmaking", "Parchmenter", "Parchment Making"],
		["Papermaking", "Papermaker", "Paper Making"],
		["Bookbinding", "Bookbinder", "Book Binding"],
		["Calligraphy", "Calligrapher"],
		["Scribing", "Scribe"],
		["Woodblock Printing", "Block Printer", "WoodblockPrinting"],
		["Movable Type Printing", "Printer", "MovableTypePrinting"],
		["Gunsmithing", "Gunsmith", "Gun Smithing"],
		["Powdermaking", "Powdermaker", "Powder Making"],
		["Lensmaking", "Lensmaker", "Lens Making"],
		["Clockmaking", "Clockmaker", "Clock Making"],
		["Instrument Making", "Instrument Maker", "InstrumentMaking"],
		["Navigation", "Navigator"],
		["Cartography", "Cartographer"],
		["Surveying", "Surveyor"],
		["Engraving", "Engraver"],
		["Chemistry", "Chemist"],
		["Winemaking", "Winemaker", "Wine Making"],
		["Foraging", "Forage"],
		["Skinning", "Skin"],
		["Butchering", "Butchery"],
		["Salvaging", "Salvage"],
		["Surviving", "Survival"],
		["Animal Breeding", "Husbandry"],
		["Beekeeping", "Beekeeper"],
		["Apothecary", "Pharmacology"],
		["Medicine", "First Aid", "Healing", "Patient Care", "Diagnosis", "Surgery", "Pharmacology", "Herbalism"],
		["Law", "Lawyer"],
		["Labouring", "Labourer", "Laboring", "Laborer"],
		["Supervising", "Supervisor"],
		["Painting", "Paint"],
		["Civil Engineering", "Civil Engineer"],
		["Constructing", "Construction"],
		["Administration", "Administrator"],
		["Performance", "Performer"],
		["Investigation", "Investigator"],
		["Smelting", "Smelter"]
	];

	private static readonly IReadOnlyDictionary<string, string> HellenicAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_short_wool_chiton"] = "a short $colour wool chiton",
			["antiquity_wool_himation"] = "a $colour wool himation",
			["antiquity_fine_linen_chiton"] = "a fine $colour linen chiton",
			["antiquity_fine_wool_himation"] = "a fine $colour himation",
			["antiquity_short_wool_chlamys"] = "a short $colour wool chlamys",
			["antiquity_full_length_wool_peplos"] = "a full-length $colour peplos",
			["antiquity_full_wool_himation"] = "a full $colour wool himation",
			["antiquity_fine_long_linen_chiton"] = "a fine long $colour chiton",
			["antiquity_fine_full_wool_himation"] = "a fine full $colour himation",
			["antiquity_light_linen_head_veil"] = "a $colour linen head veil",
			["antiquity_wool_exomis"] = "a $colour wool exomis",
			["antiquity_heavy_wool_zeira"] = "a heavy $colour wool zeira",
			["antiquity_fine_ritual_wool_peplos"] = "a fine $colour ritual peplos",
			["antiquity_wool_travel_leg_wraps"] = "a pair of $colour wool travel leg wraps",
			["antiquity_wool_hand_wraps_hellenic"] = "a pair of $colour wool hand wraps"
		};

	private static readonly IReadOnlyDictionary<string, string> EgyptianAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["adjacent_antiquity_narrow_linen_kilt"] = "a $colour narrow linen kilt",
			["adjacent_antiquity_linen_shoulder_cloth"] = "a $colour linen shoulder cloth",
			["adjacent_antiquity_sleeveless_linen_tunic"] = "a $colour sleeveless linen tunic",
			["adjacent_antiquity_fringed_linen_robe"] = "a fine $colour fringed robe",
			["adjacent_antiquity_tasseled_linen_shawl"] = "a fine $colour tasseled shawl",
			["adjacent_antiquity_tall_linen_headdress"] = "a tall $colour linen headdress",
			["adjacent_antiquity_beaded_linen_girdle"] = "a beaded $colour linen girdle",
			["adjacent_antiquity_linen_bead_apron"] = "a beaded $colour linen apron",
			["adjacent_antiquity_linen_scribe_kilt"] = "a $colour linen scribe's kilt",
			["adjacent_antiquity_temple_linen_shift"] = "a fine $colour temple linen shift",
			["antiquity_egyptian_spotted_hide_priest_mantle"] = "a spotted hide shoulder mantle"
		};

	private static readonly IReadOnlyDictionary<string, string> RomanAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_knee_length_wool_tunica"] = "a knee-length $colour tunica",
			["antiquity_wool_travel_mantle"] = "a $colour wool travel mantle",
			["antiquity_fine_linen_tunica"] = "a fine $colour linen tunica",
			["antiquity_wool_toga"] = "a $colour wool toga",
			["antiquity_long_wool_tunica"] = "a long $colour wool tunica",
			["antiquity_wool_palla"] = "a $colour wool palla",
			["antiquity_fine_long_linen_tunica"] = "a fine long $colour tunica",
			["antiquity_wool_stola"] = "a $colour wool stola",
			["antiquity_fine_wool_palla"] = "a fine $colour palla",
			["antiquity_hooded_wool_paenula"] = "a hooded $colour wool paenula",
			["antiquity_fine_toga_praetexta"] = "a fine $colour toga praetexta",
			["antiquity_wool_udones"] = "a pair of $colour wool udones",
			["antiquity_wool_hand_wraps"] = "a pair of $colour wool hand wraps",
			["antiquity_roman_wool_focale"] = "a $colour wool focale",
			["antiquity_roman_wool_sagum"] = "a short $colour wool sagum",
			["antiquity_italic_white_wool_infula"] = "a white wool infula headband"
		};

	private static readonly IReadOnlyDictionary<string, string> CelticAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_sleeved_common_wool_tunic"] = "a $colour sleeved wool tunic",
			["antiquity_wool_braccae"] = "a pair of $colour wool braccae",
			["antiquity_rectangular_wool_cloak"] = "a $colour rectangular wool cloak",
			["antiquity_fine_bordered_wool_tunic"] = "a fine $colour bordered tunic",
			["antiquity_fine_wool_braccae"] = "a pair of fine $colour braccae",
			["antiquity_fine_checked_wool_cloak"] = "a $colour1 and $colour2 checked cloak",
			["antiquity_long_sleeved_wool_tunic"] = "a long $colour wool tunic",
			["antiquity_wool_wrap_skirt"] = "a $colour wool wrap skirt",
			["antiquity_broad_wool_mantle"] = "a $colour broad wool mantle",
			["antiquity_fine_sleeved_wool_gown"] = "a fine $colour sleeved gown",
			["antiquity_fine_bordered_wool_mantle"] = "a fine $colour bordered mantle",
			["antiquity_linen_shoulder_veil"] = "a $colour linen veil",
			["antiquity_heavy_hooded_wool_cloak"] = "a heavy $colour hooded cloak",
			["antiquity_wool_work_smock"] = "a $colour wool work smock",
			["antiquity_fine_ritual_wool_mantle"] = "a fine $colour ritual mantle",
			["antiquity_wool_winter_mittens"] = "a pair of $colour wool winter mittens",
			["antiquity_checked_wool_leg_wraps"] = "a pair of $colour wool leg wraps"
		};

	private static readonly IReadOnlyDictionary<string, string> GermanicAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_straight_wool_tunic"] = "a straight $colour wool tunic",
			["antiquity_narrow_wool_trousers"] = "a pair of narrow $colour trousers",
			["antiquity_heavy_wool_cloak"] = "a heavy $colour wool cloak",
			["antiquity_fine_banded_wool_tunic"] = "a fine $colour banded tunic",
			["antiquity_fine_tapered_wool_trousers"] = "a pair of fine $colour trousers",
			["antiquity_fur_lined_wool_cloak"] = "a $colour fur-lined cloak",
			["antiquity_long_straight_wool_tunic"] = "a long straight $colour tunic",
			["antiquity_overlapping_wool_skirt"] = "a $colour overlapping wool skirt",
			["antiquity_checked_wool_scarf"] = "a $colour1 and $colour2 wool scarf",
			["antiquity_woolly_skin_cape"] = "a woolly skin cape",
			["antiquity_fine_long_wool_gown"] = "a fine $colour long gown",
			["antiquity_fine_heavy_wool_mantle"] = "a fine $colour wool mantle",
			["antiquity_linen_head_veil"] = "a $colour linen head veil",
			["antiquity_heavy_felt_hood"] = "a heavy $colour felt hood",
			["antiquity_fine_seeress_wool_mantle"] = "a fine $colour seeress mantle",
			["antiquity_felt_hand_mitts"] = "a pair of $colour felt hand-mitts",
			["antiquity_wool_footwraps"] = "a pair of $colour wool footwraps",
			["antiquity_wool_ear_band"] = "a $colour wool ear band"
		};

	private static readonly IReadOnlyDictionary<string, string> KushiteAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["adjacent_antiquity_narrow_linen_kilt"] = "a $colour narrow linen kilt",
			["adjacent_antiquity_linen_shoulder_cloth"] = "a $colour linen shoulder cloth",
			["adjacent_antiquity_cotton_wrap_skirt"] = "a $colour cotton wrap skirt",
			["adjacent_antiquity_sleeveless_linen_tunic"] = "a $colour sleeveless linen tunic",
			["adjacent_antiquity_fringed_linen_robe"] = "a fine $colour fringed robe",
			["adjacent_antiquity_tasseled_linen_shawl"] = "a fine $colour tasseled shawl",
			["adjacent_antiquity_tall_linen_headdress"] = "a tall $colour linen headdress",
			["adjacent_antiquity_beaded_linen_girdle"] = "a beaded $colour linen girdle",
			["adjacent_antiquity_cotton_draped_dress"] = "a fine $colour draped cotton dress",
			["adjacent_antiquity_linen_bead_apron"] = "a beaded $colour linen apron",
			["adjacent_antiquity_plain_cotton_headcloth"] = "a $colour cotton headcloth",
			["adjacent_antiquity_cotton_field_wrap"] = "a $colour cotton field wrap",
			["adjacent_antiquity_fine_ritual_cotton_mantle"] = "a fine $colour ritual cotton mantle"
		};

	private static readonly IReadOnlyDictionary<string, string> PunicAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_short_fitted_linen_tunic"] = "a short fitted $colour tunic",
			["antiquity_patterned_linen_waistcloth"] = "a patterned $colour waistcloth",
			["antiquity_short_sleeved_linen_overblouse"] = "a short-sleeved $colour overblouse",
			["antiquity_long_linen_inner_robe"] = "a long $colour linen robe",
			["antiquity_one_shoulder_wool_mantle"] = "a $colour one-shoulder mantle",
			["antiquity_long_folded_linen_robe"] = "a long folded $colour robe",
			["antiquity_loose_linen_hood"] = "a loose $colour linen hood",
			["antiquity_fine_full_linen_gown"] = "a fine full $colour gown",
			["antiquity_left_shoulder_overdrape"] = "a $colour left-shoulder overdrape",
			["antiquity_star_bordered_linen_robe"] = "a star-bordered $colour robe",
			["antiquity_sailor_wool_wind_cloak"] = "a $colour sailor's wool cloak",
			["antiquity_fine_temple_linen_veil"] = "a fine $colour temple veil"
		};

	private static readonly IReadOnlyDictionary<string, string> PersianAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_sarapis_wool_tunic"] = "a $colour wool sarapis",
			["antiquity_fine_sarapis_linen_tunic"] = "a fine $colour sarapis",
			["antiquity_anaxyrides_wool_trousers"] = "a pair of $colour anaxyrides",
			["antiquity_fine_patterned_anaxyrides"] = "a pair of $colour1 and $colour2 anaxyrides",
			["antiquity_wool_kandys"] = "a $colour wool kandys",
			["antiquity_fine_wool_kandys"] = "a fine $colour kandys",
			["antiquity_pleated_court_robe"] = "a pleated $colour court robe",
			["antiquity_fine_pleated_court_gown"] = "a fine pleated $colour gown",
			["antiquity_wide_cloth_belt"] = "a wide $colour cloth belt",
			["antiquity_fine_wide_cloth_belt"] = "a fine wide $colour belt",
			["antiquity_full_head_and_neck_veil"] = "a full $colour head veil",
			["antiquity_heavy_felt_riding_coat"] = "a heavy $colour felt riding coat",
			["antiquity_fine_magian_linen_robe"] = "a fine $colour magian robe",
			["antiquity_felt_riding_mittens"] = "a pair of $colour felt riding mittens",
			["antiquity_wool_boot_liners"] = "a pair of $colour wool boot liners"
		};

	private static readonly IReadOnlyDictionary<string, string> EtruscanAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["adjacent_antiquity_short_sleeved_linen_tunic"] = "a $colour short-sleeved tunic",
			["adjacent_antiquity_bordered_wool_tunic"] = "a fine $colour bordered tunic",
			["adjacent_antiquity_curved_tebenna"] = "a $colour curved tebenna",
			["adjacent_antiquity_fine_curved_tebenna"] = "a fine $colour tebenna",
			["adjacent_antiquity_wrapped_linen_skirt"] = "a $colour wrapped linen skirt",
			["adjacent_antiquity_rectangular_shoulder_cloak"] = "a $colour shoulder cloak",
			["adjacent_antiquity_fitted_linen_gown"] = "a fine $colour fitted gown",
			["adjacent_antiquity_linen_head_mantle"] = "a $colour linen head mantle",
			["adjacent_antiquity_wool_craftsman_tunic"] = "a $colour craftsman's wool tunic",
			["adjacent_antiquity_priestly_linen_veil"] = "a fine $colour priestly veil"
		};

	private static readonly IReadOnlyDictionary<string, string> AnatolianAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["adjacent_antiquity_belted_wool_tunic"] = "a $colour belted wool tunic",
			["adjacent_antiquity_fine_banded_tunic"] = "a $colour1 and $colour2 banded tunic",
			["adjacent_antiquity_banded_leg_wraps"] = "a pair of $colour leg wraps",
			["adjacent_antiquity_hooded_wool_cloak"] = "a $colour hooded wool cloak",
			["adjacent_antiquity_forward_pointing_felt_cap"] = "a $colour forward-pointing cap",
			["adjacent_antiquity_short_wool_cape"] = "a short $colour wool cape",
			["adjacent_antiquity_fine_patterned_wool_robe"] = "a fine $colour patterned robe",
			["adjacent_antiquity_fringed_wool_mantle"] = "a fine $colour fringed mantle",
			["adjacent_antiquity_wool_wrapped_skirt"] = "a $colour wrapped wool skirt",
			["adjacent_antiquity_fine_rectangular_veil"] = "a fine $colour rectangular veil",
			["adjacent_antiquity_heavy_felt_winter_cap"] = "a heavy $colour felt winter cap",
			["adjacent_antiquity_priestly_banded_wool_robe"] = "a fine $colour priestly banded robe",
			["adjacent_antiquity_felt_hand_mitts"] = "a pair of $colour felt hand-mitts",
			["adjacent_antiquity_wool_neck_wrap"] = "a $colour wool neck wrap"
		};

	private static readonly IReadOnlyDictionary<string, string> ScythianSarmatianAntiquityClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["adjacent_antiquity_felt_riding_cap"] = "a $colour felt riding cap",
			["adjacent_antiquity_tall_felt_cap"] = "a tall $colour felt cap",
			["adjacent_antiquity_riding_tunic"] = "a $colour long-sleeved riding tunic",
			["adjacent_antiquity_wool_riding_trousers"] = "a pair of $colour riding trousers",
			["adjacent_antiquity_patterned_riding_trousers"] = "a pair of fine $colour trousers",
			["adjacent_antiquity_open_riding_caftan"] = "a $colour open riding caftan",
			["adjacent_antiquity_fur_trimmed_caftan"] = "a fine $colour fur-trimmed caftan",
			["adjacent_antiquity_split_riding_skirt"] = "a $colour split riding skirt",
			["adjacent_antiquity_long_felt_coat"] = "a long $colour felt coat",
			["adjacent_antiquity_horseherder_wool_tunic"] = "a $colour horseherder's wool tunic",
			["adjacent_antiquity_ritual_felt_cap"] = "a fine $colour ritual felt cap",
			["adjacent_antiquity_felt_mittens"] = "a pair of $colour felt mittens",
			["adjacent_antiquity_felt_boot_liners"] = "a pair of $colour felt boot liners"
		};

	private static readonly IReadOnlyDictionary<string, string> AntiquityLeatherClothingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_plain_leather_belt"] = "a plain brown leather belt",
			["antiquity_bronze_buckled_leather_belt"] = "a $colour bronze-buckled belt",
			["antiquity_plain_leather_sandals"] = "a pair of plain leather sandals",
			["antiquity_fine_leather_sandals"] = "a pair of fine $colour sandals",
			["antiquity_soft_leather_shoes"] = "a pair of soft leather shoes",
			["antiquity_ankle_leather_boots"] = "a pair of ankle leather boots",
			["antiquity_low_strapped_leather_shoes"] = "a pair of strapped leather shoes",
			["antiquity_soft_leather_riding_boots"] = "a pair of soft leather riding boots",
			["adjacent_antiquity_pointed_leather_shoes"] = "a pair of pointed leather shoes",
			["adjacent_antiquity_fine_pointed_leather_shoes"] = "a pair of fine $colour pointed shoes",
			["adjacent_antiquity_soft_riding_boots"] = "a pair of soft riding boots",
			["adjacent_antiquity_fine_linen_sandals"] = "a pair of fine linen-strapped sandals"
		};

	private static readonly IReadOnlyDictionary<string, string> AntiquityLeatherArmourStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_shield_common_round_hide_shield"] = "a round hide shield",
			["antiquity_shield_punic_round_hide_shield"] = "a round hide-faced shield",
			["antiquity_shield_persian_round_dhal"] = "a round hide dhal shield",
			["antiquity_shield_egyptian_hide_shield"] = "a tall hide shield",
			["antiquity_shield_anatolian_tall_hide_shield"] = "a tall hide body shield",
			["antiquity_shield_scythian_small_hide_shield"] = "a small hide horseman shield",
			["antiquity_shield_sarmatian_round_targe"] = "a round leather targe",
			["antiquity_shield_kushite_large_oxhide_shield"] = "a large oxhide shield",
			["antiquity_celtic_dyed_leather_scale_vest"] = "a $colour leather scale vest",
			["antiquity_celtic_bronze_studded_leather_belt"] = "a bronze-studded leather war belt",
			["antiquity_celtic_leather_war_bracers"] = "a pair of leather war bracers",
			["antiquity_celtic_fur_lined_war_boots"] = "a pair of fur-lined war boots",
			["antiquity_germanic_broad_leather_war_belt"] = "a broad leather war belt",
			["antiquity_germanic_hide_war_bracers"] = "a pair of hide war bracers",
			["antiquity_germanic_fur_cuffed_high_boots"] = "a pair of fur-cuffed high boots",
			["antiquity_etruscan_pteruges_leather_girdle"] = "a leather pteruges girdle",
			["antiquity_etruscan_bronze_studded_sandals"] = "a pair of bronze-studded sandals",
			["antiquity_roman_plated_military_belt"] = "a plated leather military belt",
			["antiquity_roman_aproned_military_belt"] = "an apron-fronted military belt",
			["antiquity_roman_reinforced_caligae"] = "a pair of reinforced leather caligae",
			["antiquity_roman_leather_field_boots"] = "a pair of leather field boots",
			["antiquity_hellenic_leather_pteruges_girdle"] = "a leather pteruges girdle",
			["antiquity_hellenic_bronze_studded_sandals"] = "a pair of bronze-studded sandals",
			["antiquity_punic_bronze_studded_girdle"] = "a bronze-studded leather girdle",
			["antiquity_persian_scale_anaxyrides"] = "a pair of scale anaxyrides",
			["antiquity_persian_soft_riding_boots"] = "a pair of armoured riding boots",
			["antiquity_egyptian_leather_scale_cuirass"] = "a leather scale cuirass",
			["antiquity_egyptian_scale_kilt_guard"] = "a leather scale kilt guard",
			["antiquity_egyptian_leather_archer_bracer"] = "a leather archer bracer",
			["antiquity_anatolian_leather_cavalry_boots"] = "a pair of leather cavalry boots",
			["antiquity_scythian_leather_scale_corselet"] = "a leather scale corselet",
			["antiquity_scythian_conical_scale_cap"] = "a conical scale war cap",
			["antiquity_scythian_scale_trousers"] = "a pair of leather scale trousers",
			["antiquity_scythian_high_riding_boots"] = "a pair of high riding boots",
			["antiquity_kushite_leather_scale_breastguard"] = "a leather scale breastguard",
			["antiquity_kushite_leather_archer_bracer"] = "a leather archer bracer",
			["antiquity_kushite_leather_kilt_guard"] = "a leather kilt guard",
			["antiquity_kushite_sand_armoured_sandals"] = "a pair of armoured leather sandals"
		};

	private static readonly IReadOnlyDictionary<string, string> AntiquityLeatherContainerStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_accessory_common_leather_arrow_quiver"] = "a $colour leather arrow quiver",
			["antiquity_accessory_common_tall_arrow_quiver"] = "a tall leather arrow quiver",
			["antiquity_accessory_common_sling_bullet_pouch"] = "a sling-stone pouch",
			["antiquity_accessory_common_lead_bullet_pouch"] = "a reinforced bullet pouch",
			["antiquity_accessory_common_small_knife_sheath"] = "a small leather knife sheath",
			["antiquity_accessory_common_dagger_sheath"] = "a leather dagger sheath",
			["antiquity_accessory_common_straight_sword_scabbard"] = "a straight leather sword scabbard",
			["antiquity_accessory_common_long_sword_scabbard"] = "a long leather sword scabbard",
			["antiquity_accessory_common_large_weapon_sling"] = "a large leather weapon sling",
			["antiquity_accessory_common_javelin_quiver"] = "a leather javelin quiver",
			["antiquity_accessory_common_broad_weapon_belt"] = "a broad leather weapon belt",
			["antiquity_accessory_celtic_decorated_sword_scabbard"] = "a decorated sword scabbard",
			["antiquity_accessory_celtic_broad_baldric"] = "a broad leather sword baldric",
			["antiquity_accessory_germanic_seax_sheath"] = "a broad seax sheath",
			["antiquity_accessory_germanic_axe_belt_loop"] = "a leather axe belt loop",
			["antiquity_accessory_roman_gladius_scabbard"] = "a fitted gladius scabbard",
			["antiquity_accessory_roman_pugio_sheath"] = "a decorated pugio sheath",
			["antiquity_accessory_roman_spatha_scabbard"] = "a long spatha scabbard",
			["antiquity_accessory_roman_pilum_carrying_sleeve"] = "a pilum carrying sleeve",
			["antiquity_accessory_roman_plumbata_pouch"] = "a weighted dart pouch",
			["antiquity_accessory_roman_military_weapon_belt"] = "a fitted military weapon belt",
			["antiquity_accessory_hellenic_xiphos_scabbard"] = "a short xiphos scabbard",
			["antiquity_accessory_hellenic_kopis_sheath"] = "a curved kopis sheath",
			["antiquity_accessory_hellenic_bolt_quiver"] = "a short bolt quiver",
			["antiquity_accessory_punic_javelin_quiver"] = "a back-slung javelin quiver",
			["antiquity_accessory_punic_short_sword_sheath"] = "a bronze-fitted sword sheath",
			["antiquity_accessory_persian_gorytos_quiver"] = "a decorated leather gorytos",
			["antiquity_accessory_persian_akinakes_scabbard"] = "a short decorated scabbard",
			["antiquity_accessory_persian_bowcase_strap"] = "a leather bowcase strap",
			["antiquity_accessory_persian_arrow_pouch"] = "a small arrowhead pouch",
			["antiquity_accessory_egyptian_khopesh_sheath"] = "a curved khopesh sheath",
			["antiquity_accessory_egyptian_linen_arrow_quiver"] = "a linen-covered arrow quiver",
			["antiquity_accessory_etruscan_bronze_fitted_scabbard"] = "a bronze-fitted sword scabbard",
			["antiquity_accessory_etruscan_spear_sling"] = "a leather spear carrying sling",
			["antiquity_accessory_etruscan_dagger_sheath"] = "a small bronze-edged sheath",
			["antiquity_accessory_anatolian_arrow_quiver"] = "a patterned arrow quiver",
			["antiquity_accessory_anatolian_curved_sword_sheath"] = "a curved sword sheath",
			["antiquity_accessory_scythian_gorytos"] = "a tooled leather gorytos",
			["antiquity_accessory_scythian_axe_loop"] = "a horseman axe loop",
			["antiquity_accessory_scythian_akinakes_scabbard"] = "a short akinakes scabbard",
			["antiquity_accessory_kushite_tall_arrow_quiver"] = "a tall leather arrow quiver",
			["antiquity_accessory_kushite_bow_carrying_sling"] = "a leather bow carrying sling",
			["antiquity_smoked_hide_meat_bag"] = "a smoked hide provision bag",
			["antiquity_leather_document_case"] = "a folded leather document case",
			["antiquity_leather_mirror_case"] = "a tooled leather mirror case",
			["antiquity_plain_leather_belt_pouch"] = "a plain leather belt pouch",
			["antiquity_double_strap_travel_pack"] = "a double-strapped leather pack",
			["antiquity_fur_lined_forager_bag"] = "a fur-lined forager's bag",
			["antiquity_deer_leather_game_bag"] = "a deer-leather game bag",
			["antiquity_folded_tablet_wallet"] = "a folded leather tablet wallet",
			["antiquity_round_coin_purse"] = "a round leather coin purse",
			["antiquity_leather_dispatch_satchel"] = "a leather dispatch satchel",
			["antiquity_wide_belt_document_pouch"] = "a wide leather document pouch",
			["antiquity_steppe_saddlebag_pack"] = "a steppe leather saddlebag pack",
			["antiquity_steppe_gorytos_case"] = "a tooled leather gorytos case",
			["antiquity_fur_provision_pouch"] = "a fur provision pouch",
			["antiquity_liquid_steppe_milk_skin"] = "a dark leather milk skin",
			["antiquity_liquid_plain_leather_waterskin"] = "a plain leather waterskin",
			["antiquity_liquid_wide_mouth_waterskin"] = "a wide-mouthed waterskin",
			["antiquity_liquid_leather_belt_oil_flask"] = "a leather belt oil flask",
			["antiquity_liquid_hide_ale_skin"] = "a hide ale skin",
			["antiquity_liquid_birch_stoppered_mead_skin"] = "a birch-stoppered mead skin",
			["antiquity_liquid_soldier_shoulder_canteen"] = "a leather-covered shoulder canteen",
			["antiquity_liquid_sailor_water_skin"] = "a tar-dark sailor waterskin",
			["antiquity_liquid_caravan_waterskin"] = "a large caravan waterskin",
			["antiquity_liquid_silver_tipped_belt_flask"] = "a silver-tipped belt flask",
			["antiquity_liquid_felt_covered_riding_canteen"] = "a felt-covered riding canteen",
			["antiquity_liquid_saddle_waterskin"] = "a saddle-hung waterskin",
			["antiquity_liquid_steppe_kumis_skin"] = "a patched fermented-milk skin",
			["antiquity_liquid_tooled_leather_flask"] = "a tooled leather flask",
			["antiquity_tableware_scythian_leather_travel_cup"] = "a folded leather travel cup"
		};

	private static readonly IReadOnlyDictionary<string, string> AntiquityLeatherFurnishingStableReferences =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity_hide_sleeping_roll"] = "a hide sleeping roll",
			["antiquity_leather_floor_cover"] = "a leather floor cover",
			["antiquity_fur_throw"] = "a soft fur throw",
			["antiquity_fur_door_hanging"] = "a fur doorway hanging",
			["antiquity_leather_tent_door_flap"] = "a leather tent door flap"
		};

	private sealed record CraftValidationError(string CraftName, string Kind, string ImportText, string Message)
	{
		public override string ToString()
		{
			return $"Craft '{CraftName}' {Kind} '{ImportText}': {Message}";
		}
	}

	private FutureProg EnsureFutureProg(string name, string category, string subcategory, ProgVariableTypes returnType,
		string comment, IEnumerable<(ProgVariableTypes Type, string Name)> parameters, string text)
	{
		var parameterList = parameters.ToArray();
		var manifestDefinition = new ProgManifestDefinition(
			name,
			category,
			subcategory,
			(long)returnType,
			comment,
			text,
			parameterList.Select(x => new ProgParameterManifestDefinition((long)x.Type, x.Name)).ToArray());
		var manifestEntry = RegisterManifestAggregate("prog", name, manifestDefinition);
		if (_manifestCaptureOnly && _progs.TryGetValue(name, out var capturedProg))
		{
			return capturedProg;
		}

		ProgManifestDefinition LiveDefinition(FutureProg prog)
		{
			var progParameters = prog.FutureProgsParameters
				.Concat(_context!.FutureProgsParameters.Local.Where(x => x.FutureProgId == prog.Id))
				.Concat(_context.FutureProgsParameters.Where(x => x.FutureProgId == prog.Id))
				.GroupBy(x => x.ParameterIndex)
				.Select(x => x.First())
				.OrderBy(x => x.ParameterIndex)
				.Select(x => new ProgParameterManifestDefinition(x.ParameterType, x.ParameterName))
				.ToArray();
			return new ProgManifestDefinition(
				prog.FunctionName,
				prog.Category,
				prog.Subcategory,
				prog.ReturnType,
				prog.FunctionComment,
				prog.FunctionText,
				progParameters);
		}

		FutureProg ReconcileExisting(FutureProg prog)
		{
			var disposition = InspectManifestAggregate(manifestEntry, prog.Id, LiveDefinition(prog));
			if (disposition is ManifestAggregateDisposition.Customized or ManifestAggregateDisposition.Unchanged)
			{
				return prog;
			}

			prog.Category = category;
			prog.Subcategory = subcategory;
			prog.ReturnType = (long)returnType;
			prog.FunctionComment = comment;
			prog.FunctionText = text;
			_context!.FutureProgsParameters.RemoveRange(
				_context.FutureProgsParameters.Where(x => x.FutureProgId == prog.Id));
			prog.FutureProgsParameters.Clear();
			var index = 0;
			foreach (var parameter in parameterList)
			{
				prog.FutureProgsParameters.Add(new FutureProgsParameter
				{
					FutureProg = prog,
					ParameterIndex = index++,
					ParameterType = (long)parameter.Type,
					ParameterName = parameter.Name
				});
			}
			CompleteManifestAggregate(manifestEntry, prog.Id, manifestDefinition, disposition);
			return prog;
		}

		if (_progs.TryGetValue(name, out var existing))
		{
			return ReconcileExisting(existing);
		}

		var contextProg = _context!.FutureProgs.Local
			.AsEnumerable()
			.FirstOrDefault(x =>
			x.FunctionName.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
			_context.FutureProgs.AsEnumerable().FirstOrDefault(x => x.FunctionName.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (contextProg is not null)
		{
			_progs[name] = contextProg;
			return ReconcileExisting(contextProg);
		}

		FutureProg prog = new()
		{
			FunctionName = name,
			Category = category,
			Subcategory = subcategory,
			ReturnType = (long)returnType,
			FunctionComment = comment,
			FunctionText = text,
			StaticType = (int)FutureProgStaticType.NotStatic,
			AcceptsAnyParameters = false
		};
		_context.FutureProgs.Add(prog);
		int index = 0;
		foreach ((ProgVariableTypes type, string? varname) in parameterList)
		{
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = index++,
				ParameterType = (long)type,
				ParameterName = varname
			});
		}

		_progs[name] = prog;
		CompleteGeneratedManifestAggregate(manifestEntry, prog, manifestDefinition);
		return prog;
	}

    private void CreateProgs()
    {
		foreach (FutureProg prog in _context!.FutureProgs)
        {
            _progs[prog.FunctionName] = prog;
        }

        void AddProg(string name, string category, string subcategory, ProgVariableTypes returnType, string comment, IEnumerable<(ProgVariableTypes Type, string Name)> parameters, string text)
        {
			EnsureFutureProg(name, category, subcategory, returnType, comment, parameters, text);
        }

		const string constructionCooldown = "constructioncooldown";
		const long constructionOwnerType = 8;
		const string constructionDefault = "<var>01/01/0001 00:00:00</var>";
		var variableManifest = new VariableManifestDefinition(
			constructionOwnerType,
			constructionCooldown,
			(long)ProgVariableTypes.DateTime,
			constructionDefault);
		var variableEntry = RegisterManifestAggregate("variable", constructionCooldown, variableManifest);
		var variable = _context.VariableDefinitions.AsEnumerable().FirstOrDefault(x =>
			x.Property == constructionCooldown && x.OwnerType == constructionOwnerType);
		var variableDefault = _context.VariableDefaults.AsEnumerable().FirstOrDefault(x =>
			x.Property == constructionCooldown && x.OwnerType == constructionOwnerType);
		if (variable is null)
		{
			variable = new VariableDefinition
			{
				ContainedType = (long)ProgVariableTypes.DateTime,
				OwnerType = constructionOwnerType,
				Property = constructionCooldown
			};
			_context.VariableDefinitions.Add(variable);
		}
		if (variableDefault is null)
		{
			variableDefault = new VariableDefault
			{
				OwnerType = constructionOwnerType,
				Property = constructionCooldown,
				DefaultValue = constructionDefault
			};
			_context.VariableDefaults.Add(variableDefault);
		}
		var variableDisposition = _context.Entry(variable).State == EntityState.Added ||
		                          _context.Entry(variableDefault).State == EntityState.Added
			? ManifestAggregateDisposition.Insert
			: InspectManifestAggregate(variableEntry, FindManagedRecord(variableEntry.EntityType, variableEntry.StableKey)?.LogicalId, new VariableManifestDefinition(
				variable.OwnerType,
				variable.Property,
				variable.ContainedType,
				variableDefault.DefaultValue));
		if (variableDisposition is ManifestAggregateDisposition.Insert or ManifestAggregateDisposition.Update)
		{
			variable.ContainedType = (long)ProgVariableTypes.DateTime;
			variableDefault.DefaultValue = constructionDefault;
			CompleteManifestAggregate(variableEntry, null, variableManifest, variableDisposition);
		}


        AddProg("CanDigGrave", "Crafting", "General", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], @"return IsTagged(@ch.Location.Terrain, ""Diggable Soil"")");
        AddProg("WhyCannotDigGrave", "Crafting", "General", ProgVariableTypes.Text, "", [(ProgVariableTypes.Character, "ch")], @"return ""There is no easily diggable soil here, so you cannot dig a grave.""");
        AddProg("CanDoConstructionCooldown", "Crafting", "Cooldowns", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], @"if (not(IsTagged(@ch.Location.Terrain, ""Terrestrial"")))
  return false
end if
var last as datetime
last = getregister(@ch, ""constructioncooldown"")
if (isnull(@last))
  return true
end if
return @last <= now()");
        AddProg("WhyCantDoConstructionCooldown", "Crafting", "Cooldowns", ProgVariableTypes.Text, "", [(ProgVariableTypes.Character, "ch")], @"if (not(IsTagged(@ch.Location.Terrain, ""Terrestrial"")))
  return ""The ground here is not suitable for laying down foundations for dwellings.""
end if
return ""You are still too fatigued from your recent construction-related exertion.""");
        AddProg("ApplyConstructionCooldown", "Crafting", "Cooldowns", ProgVariableTypes.Void, "", [(ProgVariableTypes.Character, "ch")], @"setregister @ch ""constructioncooldown"" (now() + 16h) ");
        AddProg("CanForageClay", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], @"if (@ch.Layer != ""GroundLevel"")
  return false
end if
if (IsSwimLayer(@ch.Location, @ch.Layer))
  return false
end if
if (IsTagged(@ch.Location.Terrain, ""Foragable Clay""))
  return false
end if
return true");
        AddProg("WhyCantForageClay", "Crafting", "Access", ProgVariableTypes.Text, "", [(ProgVariableTypes.Character, "ch")], @"if (@ch.Layer != ""GroundLevel"")
return ""You can only forage for clay when you are at ground level.""
end if
if (IsSwimLayer(@ch.Location, @ch.Layer))
return ""You cannot forage for clay while swimming or underwater.""
end if
return ""There is no useful clay that is accessible in the biome you're in.""");

		TraitDefinition ResolveTrait(params string[] names)
		{
			foreach (var name in names)
			{
				if (_traits.TryGetValue(name, out var trait))
				{
					return trait;
				}
			}

			return _traits.First().Value;
		}

		string HasTraitText(TraitDefinition trait) => $@"{FutureProgReferenceComment("Trait", trait.Id, trait.Name)}
return @ch.Skills.Any(x, @x.Id == {trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
		string MinimumTraitText(TraitDefinition trait, int minimum) => $@"{FutureProgReferenceComment("Trait", trait.Id, trait.Name)}
return GetTrait(@ch, ToTrait(""{trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)}"")) >= {minimum.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

		AddProg("HasCarpentryAdept", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], MinimumTraitText(ResolveTrait("Carpentry", "Woodcraft"), 46));
		AddProg("HasCarpentryMaster", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], MinimumTraitText(ResolveTrait("Carpentry", "Woodcraft"), 61));
		AddProg("HasLabouring", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Labouring", "Labourer")));
		AddProg("HasCarpentry", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Carpentry", "Woodcraft")));
		AddProg("HasConstruction", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Constructing", "Construction")));
		AddProg("HasButchering", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Butchering", "Butchery", "Surviving", "Survival")));
		AddProg("HasCooking", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Cooking", "Cook", "Surviving", "Survival")));
		AddProg("HasThreshing", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Threshing", "Thresher", "Farming")));
		AddProg("HasMilling", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Milling", "Miller", "Cooking", "Cook")));
		AddProg("HasBrewing", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Brewing", "Brewer", "Cooking", "Cook")));
		AddProg("HasBlacksmithing", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Blacksmithing", "Metalcraft")));
		AddProg("HasWeaponcrafting", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Weaponcrafting", "Weaponsmith")));
		AddProg("HasArmourcrafting", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Armourcrafting", "Armourer")));
		AddProg("HasSurvival", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Surviving", "Survival")));
		AddProg("HasTailoring", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Tailoring", "Tailor")));
		AddProg("HasLeatherworking", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Leathermaking", "Hideworking")));
		AddProg("HasLocksmithing", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Locksmithing", "Locksmith")));
		AddProg("HasForestry", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Lumberjacking", "Lumberjack")));
		AddProg("HasFarming", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Farming")));
		AddProg("HasPottery", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Pottery", "Potter")));
		AddProg("HasBasicPharmacology", "Crafting", "Access", ProgVariableTypes.Boolean, "", [(ProgVariableTypes.Character, "ch")], HasTraitText(ResolveTrait("Pharmacology", "Medicine")));

		if (!_manifestCaptureOnly)
		{
			SaveManifestChanges();
		}
    }

    private Regex ConversionRegex = new(@"^(?<type>\w+?) - (?<details>.+)$", RegexOptions.IgnoreCase);

    private Regex TagInputRegex = new(@"^(?<quantity>\d+)x an item with the (?<tag>.+) tag$", RegexOptions.IgnoreCase);

    private Regex TagVariableInputRegex = new(@"^(?<quantity>\d+)x an item with the (?<tag>.+?) tag(?: with variables? (?<variables>.+))?$", RegexOptions.IgnoreCase);

    private Regex ConditionRepairInputRegex = new(@"^(?<repair>[0-9,.]+%) repair of an item with the (?<tag>.+) tag$", RegexOptions.IgnoreCase);

    private Regex SimpleItemInputRegex = new(@"^(?<quantity>\d+)x (?:(?<sdesc>@[a-z][a-z0-9_]*)|(?<sdesc>.+) \(#(?<craftid>\d+)\))$", RegexOptions.IgnoreCase);

    private Regex SimpleMaterialInputRegex = new(@"^(?<quantity>\d+)x an item with material tagged as (?<tag>.+)$", RegexOptions.IgnoreCase);

    private Regex SimpleMaterialExactInputRegex = new(@"^(?<quantity>\d+)x an item (?:made of|with material) (?<material>.+)$", RegexOptions.IgnoreCase);

    private Regex CommodityInputRegex = new(@"^(?:(?<kgs>\d+(?:\.\d+)?) kilograms?)*\s*(?:(?<grams>\d+(?:\.\d+)?) grams?)* of (?<material>.+)\s*$", RegexOptions.IgnoreCase);

    private Regex CommodityTagInputRegex = new(@"^(?:(?<kgs>\d+(?:\.\d+)?) kilograms?)*\s*(?:(?<grams>\d+(?:\.\d+)?) grams?)* of a material tagged as (?<material>.+)\s*$", RegexOptions.IgnoreCase);

    private Regex LiquidUseInputRegex = new(@"^(?:(?<litres>\d+(?:\.\d+)?) litres?)*\s*(?:(?<millilitres>\d+(?:\.\d+)?) millilitres?)* of (?<liquid>.+)\s*$", RegexOptions.IgnoreCase);

    private Regex LiquidTagUseInputRegex = new(@"^(?:(?<litres>\d+(?:\.\d+)?) litres?)*\s*(?:(?<millilitres>\d+(?:\.\d+)?) millilitres?)* of a liquid tagged (?<liquid>.+)\s*$", RegexOptions.IgnoreCase);

    private Regex ToolRegex = new(@"^(?<state>.+) - (?<details>.+)$", RegexOptions.IgnoreCase);

    private Regex TagToolRegex = new(@"^an item with the (?<tag>.+) tag$", RegexOptions.IgnoreCase);

    private Regex SimpleProductRegex = new(@"^(?<quantity>\d+)x (?:(?<sdesc>@[a-z][a-z0-9_]*)|(?<sdesc>.+) \(#(?<craftid>\d+)\))$", RegexOptions.IgnoreCase);

    private Regex LiquidProductRegex = new(@"^(?<quantity>\d+)x (?<sdesc>.+) \(#(?<craftid>\d+)\) filled with (?:(?<litres>\d+(?:\.\d+)?) litres?)*\s*(?:(?<millilitres>\d+(?:\.\d+)?) millilitres?)* of (?<liquid>.+)$", RegexOptions.IgnoreCase);

    private Regex ItemReferenceRegex = new(@"^(?<sdesc>.+?)(?:\s*\(#(?<craftid>\d+)\))?$", RegexOptions.IgnoreCase);

    private Regex CommodityProductRegex = new(@"^(?:(?<kgs>\d+(?:\.\d+)?) kilograms?)*\s*(?:(?<grams>\d+(?:\.\d+)?) grams?)* of (?<material>.+)\s* commodity$", RegexOptions.IgnoreCase);

    private Regex ScrapInputProductRegex = new(@"^(?<percentage>[0-9,.]+%) by weight of (?<inputdesc>.+) \(\$i(?<input>\d+)\)$", RegexOptions.IgnoreCase);

    private Regex UnusedInputProductRegex = new(@"^(?<percentage>[0-9,.]+%) of (?<inputdesc>.+) \(\$i(?<input>\d+)\)$", RegexOptions.IgnoreCase);

    private Regex BloodTypingProductRegex = new(@"^test (?:(?<litres>\d+(?:\.\d+)?) litres?)*\s*(?:(?<millilitres>\d+(?:\.\d+)?) millilitres?)* of (?<liquid>.+) \(\$i(?<input>\d+)\) against the (?<model>.+) blood model$", RegexOptions.IgnoreCase);

    private Regex SimpleBloodTypingProductRegex = new(@"^(?:test\s+)?\$i(?<input>\d+) against (?:the )?(?<model>.+?)(?: blood model)?$", RegexOptions.IgnoreCase);

    private Regex DnaTestProductRegex = new(@"^(?:compare|test) \$i(?<input1>\d+) (?:and|against|with) \$i(?<input2>\d+)$", RegexOptions.IgnoreCase);

    private Regex MoneyProductRegex = new(@"^(?<amount>[0-9,.]+) (?:of|in) (?<currency>.+)$", RegexOptions.IgnoreCase);

    private Regex NpcProductRegex = new(@"^(?<quantity>\d+)x (?<template>.+?)(?: \(#(?<templateid>\d+)\))?$", RegexOptions.IgnoreCase);

    private Regex VariableInputMappingRegex = new(@"^(?<definition>.+?)\s*(?:=|from)\s*\$i(?<input>\d+)$", RegexOptions.IgnoreCase);

    private Regex InputVariableSpecificRegex = new(@"^(?<definition>.+?)\s*:\s*(?<specifics>.+)$", RegexOptions.IgnoreCase);

    private Regex SpecificVariableMappingRegex = new(@"^(?<item>.+?)\s*=\s*(?<value>.+)$", RegexOptions.IgnoreCase);

    private Regex ProgVariableMappingRegex = new(@"^(?<definition>.+?)\s*=\s*(?<prog>.+)$", RegexOptions.IgnoreCase);

    private Regex IngredientSlotRegex = new(@"\$i(?<input>\d+)\s*(?:=|:|as)\s*(?<role>[^,;]+)", RegexOptions.IgnoreCase);

    private static bool HasValue(Group group)
    {
        return group.Success && !string.IsNullOrWhiteSpace(group.Value);
    }

    private static double ParseDoubleValue(string value)
    {
        return double.Parse(value.Replace(",", ""), NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static decimal ParseDecimalValue(string value)
    {
        return decimal.Parse(value.Replace(",", ""), NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static int ParseIntValue(string value)
    {
        return int.Parse(value.Replace(",", ""), NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static double ParseMass(Match match)
    {
        return (HasValue(match.Groups["kgs"]) ? 1000.0 * ParseDoubleValue(match.Groups["kgs"].Value) : 0.0) +
               (HasValue(match.Groups["grams"]) ? ParseDoubleValue(match.Groups["grams"].Value) : 0.0);
    }

    private static double ParseVolume(Match match)
    {
        return (HasValue(match.Groups["litres"]) ? ParseDoubleValue(match.Groups["litres"].Value) : 0.0) +
               (HasValue(match.Groups["millilitres"]) ? 0.001 * ParseDoubleValue(match.Groups["millilitres"].Value) : 0.0);
    }

    private static Match RequireMatch(Regex regex, string text, string error)
    {
        Match match = regex.Match(text.Trim());
        if (!match.Success)
        {
            throw new ApplicationException(error);
        }

        return match;
    }

    private static (string Main, List<string> Options) SplitDetails(string details)
    {
        List<string> parts = details.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                    .ToList();
        return (parts.FirstOrDefault() ?? string.Empty, parts.Skip(1).ToList());
    }

	private (string Type, string Details, List<string> Options) ParseConversionImport(string text, string error)
	{
		Match match = RequireMatch(ConversionRegex, text, error);
		(string details, List<string> options) = SplitDetails(match.Groups["details"].Value);
		return (match.Groups["type"].Value.Trim(), details, options);
	}

	private static bool StartsWithOption(string option, string keyword)
	{
		return option.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) &&
		       (option.Length == keyword.Length || char.IsWhiteSpace(option[keyword.Length]));
	}

	private static string GetOptionRemainder(string option, string keyword)
	{
		return option.Length == keyword.Length ? string.Empty : option[keyword.Length..].Trim();
	}

	private static bool TryParseImportBoolean(string value, out bool result)
	{
		if (value.EqualToAny("on", "true", "yes", "1"))
		{
			result = true;
			return true;
		}

		if (value.EqualToAny("off", "false", "no", "0"))
		{
			result = false;
			return true;
		}

		result = false;
		return false;
	}

	private static double ParseQualityWeight(IEnumerable<string> options, string importText)
	{
		double quality = 1.0;
		foreach (string option in options.Where(x => StartsWithOption(x, "quality") || StartsWithOption(x, "qualityweight")))
		{
			string keyword = StartsWithOption(option, "qualityweight") ? "qualityweight" : "quality";
			string valueText = GetOptionRemainder(option, keyword);
			if (string.IsNullOrWhiteSpace(valueText) ||
			    !double.TryParse(valueText.Replace(",", ""), NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out quality) ||
			    quality < 0.0)
			{
				throw new ApplicationException($"Invalid quality weight in import '{importText}'");
			}
		}

		return quality;
	}

	private static bool ParseUseToolDuration(IEnumerable<string> options, string importText)
	{
		bool useToolDuration = true;
		foreach (string option in options.Where(x => StartsWithOption(x, "usetool")))
		{
			string valueText = GetOptionRemainder(option, "usetool");
			if (string.IsNullOrWhiteSpace(valueText) || !TryParseImportBoolean(valueText, out useToolDuration))
			{
				throw new ApplicationException($"Invalid usetool option in import '{importText}'");
			}
		}

		return useToolDuration;
	}

	private CraftInputSpec ParseInputSpec(string text)
	{
		(string type, string details, List<string> options) = ParseConversionImport(text, "Invalid input definition");
		return new CraftInputSpec(text, type, details, options, ParseQualityWeight(options, text));
	}

	private CraftToolSpec ParseToolSpec(string text)
	{
		(string type, string details, List<string> options) = ParseConversionImport(text, "Invalid tool definition");
		return new CraftToolSpec(text, type, details, options, ParseQualityWeight(options, text),
			ParseUseToolDuration(options, text));
	}

	private CraftProductSpec ParseProductSpec(string text, bool isFailProduct, int? materialDefiningInputIndex = null)
	{
		(string type, string details, List<string> options) = ParseConversionImport(text, "Invalid product definition");
		return new CraftProductSpec(text, type, details, options, isFailProduct, materialDefiningInputIndex);
	}

    private MudSharp.Models.Tag LookupTag(string text)
    {
		if (_tagsByFullPath.TryGetValue(text.Trim(), out var qualifiedTag)) return qualifiedTag;
        if (_tags.TryGetValue(text.Trim(), out MudSharp.Models.Tag? tag))
        {
            return tag;
        }

        throw new ApplicationException($"Unknown tag {text}");
    }

    private Material LookupMaterial(string text)
    {
        if (_materials.TryGetValue(text.Trim(), out Material? material))
        {
            return material;
        }

        throw new ApplicationException($"Unknown material {text}");
    }

    private Liquid LookupLiquid(string text)
    {
        if (_liquids.TryGetValue(text.Trim(), out Liquid? liquid))
        {
            return liquid;
        }

        throw new ApplicationException($"Unknown liquid {text}");
    }

    private GameItemProto? LookupItem(Match match)
    {
		var reference = match.Groups["sdesc"].Value.Trim();
		if (reference.StartsWith('@') && !HasValue(match.Groups["craftid"])) return _itemsByStableReference.GetValueOrDefault(reference[1..]);
        if (HasValue(match.Groups["craftid"]) && long.TryParse(match.Groups["craftid"].Value, out long id))
        {
			if (_itemsById.TryGetValue(id, out GameItemProto? cachedItem))
			{
				return cachedItem;
			}

            return _context!.GameItemProtos.FirstOrDefault(x => x.Id == id);
        }

        return _items.TryGetValue(match.Groups["sdesc"].Value.Trim(), out GameItemProto? item) ? item : null;
    }

    private GameItemProto LookupItem(string text)
    {
		if (text.Trim().StartsWith('@')) return _itemsByStableReference.GetValueOrDefault(text.Trim()[1..]) ??
			throw new ApplicationException($"Unknown stable item reference {text}");
        Match match = RequireMatch(ItemReferenceRegex, text, $"Invalid item reference {text}");
        if (HasValue(match.Groups["craftid"]) && long.TryParse(match.Groups["craftid"].Value, out long id))
        {
			if (_itemsById.TryGetValue(id, out GameItemProto? cachedItem))
			{
				return cachedItem;
			}

            GameItemProto? itemById = _context!.GameItemProtos.FirstOrDefault(x => x.Id == id);
            if (itemById is not null)
            {
                return itemById;
            }
        }

        if (_items.TryGetValue(match.Groups["sdesc"].Value.Trim(), out GameItemProto? item))
        {
            return item;
        }

        throw new ApplicationException($"Unknown item {text}");
    }

    private CharacteristicDefinition LookupCharacteristicDefinition(string text)
    {
        string trimmed = text.Trim();
        CharacteristicDefinition? definition = long.TryParse(trimmed.TrimStart('#'), out long id)
            ? _context!.CharacteristicDefinitions.AsEnumerable().FirstOrDefault(x => x.Id == id)
            : _context!.CharacteristicDefinitions.AsEnumerable().FirstOrDefault(x => x.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
        return definition ?? throw new ApplicationException($"Unknown characteristic definition {text}");
    }

	private CharacteristicValue LookupCharacteristicValue(CharacteristicDefinition definition, string text)
	{
		string trimmed = text.Trim();
		CharacteristicValue? value;
		if (long.TryParse(trimmed.TrimStart('#'), out long id))
		{
			value = _context!.CharacteristicValues.AsEnumerable().FirstOrDefault(x => x.Id == id);
			if (value is not null && !IsCharacteristicValueForDefinition(definition, value))
			{
				value = null;
			}
		}
		else
		{
			value = _context!.CharacteristicValues.AsEnumerable().FirstOrDefault(x => x.DefinitionId == definition.Id && x.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase)) ??
			        _context.CharacteristicValues.AsEnumerable().FirstOrDefault(x => x.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase) && IsCharacteristicValueForDefinition(definition, x));
		}

		return value ?? throw new ApplicationException($"Unknown value {text} for characteristic {definition.Name}");
	}

	private bool IsCharacteristicValueForDefinition(CharacteristicDefinition definition, CharacteristicValue value)
	{
		CharacteristicDefinition? current = definition;
		while (current is not null)
		{
			if (value.DefinitionId == current.Id)
			{
				return true;
			}

			current = current.ParentId is null
				? null
				: _context!.CharacteristicDefinitions.Local.FirstOrDefault(x => x.Id == current.ParentId.Value) ??
				  _context.CharacteristicDefinitions.FirstOrDefault(x => x.Id == current.ParentId.Value);
		}

		return false;
	}

	private long LookupCommodityPileTagId(IEnumerable<string> options)
	{
		long tagId = 0;
		foreach (string option in options.Where(x => StartsWithOption(x, "piletag") || StartsWithOption(x, "pile tag")))
		{
			string keyword = StartsWithOption(option, "piletag") ? "piletag" : "pile tag";
			string tagText = GetOptionRemainder(option, keyword);
			if (tagText.EqualToAny("none", "clear", "0"))
			{
				tagId = 0;
				continue;
			}

			tagId = LookupTag(tagText).Id;
		}

		return tagId;
	}

	private (CharacteristicDefinition Definition, string ValueText) ParseCommodityInputCharacteristicRequirement(string text)
	{
		int equalsIndex = text.IndexOf('=');
		if (equalsIndex > 0)
		{
			return (LookupCharacteristicDefinition(text[..equalsIndex]), text[(equalsIndex + 1)..].Trim());
		}

		Match idMatch = Regex.Match(text, @"^(?<definition>#?\d+)\s+(?<value>.+)$", RegexOptions.IgnoreCase);
		if (idMatch.Success)
		{
			return (LookupCharacteristicDefinition(idMatch.Groups["definition"].Value), idMatch.Groups["value"].Value.Trim());
		}

		foreach (CharacteristicDefinition definition in _context!.CharacteristicDefinitions.AsEnumerable()
		                                            .OrderByDescending(x => x.Name.Length))
		{
			if (!text.StartsWith(definition.Name, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			if (text.Length <= definition.Name.Length || !char.IsWhiteSpace(text[definition.Name.Length]))
			{
				continue;
			}

			return (definition, text[definition.Name.Length..].Trim());
		}

		Match match = RequireMatch(ProgVariableMappingRegex, text, "Invalid commodity characteristic requirement");
		return (LookupCharacteristicDefinition(match.Groups["definition"].Value), match.Groups["prog"].Value.Trim());
	}

	private XElement BuildCommodityInputCharacteristics(IEnumerable<string> options)
	{
		string mode = "any";
		List<XElement> requirements = [];
		foreach (string option in options.Where(x => StartsWithOption(x, "characteristic")))
		{
			string text = GetOptionRemainder(option, "characteristic");
			if (text.EqualTo("any"))
			{
				mode = "any";
				requirements.Clear();
				continue;
			}

			if (text.EqualTo("none"))
			{
				mode = "none";
				requirements.Clear();
				continue;
			}

			(CharacteristicDefinition definition, string valueText) = ParseCommodityInputCharacteristicRequirement(text);
			mode = "specific";
			requirements.Add(new XElement("Characteristic",
				new XAttribute("definition", definition.Id),
				new XAttribute("value", valueText.EqualTo("any") ? 0 : LookupCharacteristicValue(definition, valueText).Id)
			));
		}

		return new XElement("Characteristics",
			new XAttribute("mode", mode),
			mode.EqualTo("specific") ? requirements : Enumerable.Empty<XElement>()
		);
	}

    private GameItemSkin? LookupSkin(GameItemProto item, string text)
    {
        string trimmed = text.Trim();
		if (trimmed.StartsWith('@'))
		{
			var reference = trimmed[1..];
			var logicalId = FindManagedRecord("item-skin", reference)?.LogicalId;
			var matches = _context!.GameItemSkins.Local.Concat(_context.GameItemSkins.Include(x => x.EditableItem).AsEnumerable())
				.Where(x => x.ItemProtoId == item.Id && (logicalId.HasValue ? x.Id == logicalId : x.Name == reference))
				.Where(x => x.EditableItem?.RevisionStatus == 4)
				.DistinctBy(x => (x.Id, x.RevisionNumber)).ToArray();
			if (matches.Length != 1)
				throw new ApplicationException($"Missing or ambiguous stable skin reference {reference} for item {item.UniqueName}.");
			return matches[0];
		}
        if (trimmed.EqualToAny("none", "clear", "0"))
        {
            return null;
        }

        return long.TryParse(trimmed.TrimStart('#'), out long id)
            ? _context!.GameItemSkins.FirstOrDefault(x => x.Id == id && x.ItemProtoId == item.Id)
            : _context!.GameItemSkins.FirstOrDefault(x =>
                x.ItemProtoId == item.Id &&
                ((x.Name != null && x.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase)) ||
                 (x.ShortDescription != null && x.ShortDescription.Equals(trimmed, StringComparison.OrdinalIgnoreCase))));
    }

    private FutureProg LookupProg(string text)
    {
        string trimmed = text.Trim();
        if (_progs.TryGetValue(trimmed, out FutureProg? prog))
        {
            return prog;
        }

		FutureProg? contextProg = long.TryParse(trimmed.TrimStart('#'), out long id)
			? _context!.FutureProgs.FirstOrDefault(x => x.Id == id)
			: _context!.FutureProgs.Local
				.AsEnumerable()
				.FirstOrDefault(x => x.FunctionName.Equals(trimmed, StringComparison.OrdinalIgnoreCase)) ??
			  _context.FutureProgs
				.AsEnumerable()
				.FirstOrDefault(x => x.FunctionName.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
        return contextProg ?? throw new ApplicationException($"Unknown prog {text}");
    }

    private BloodModel LookupBloodModel(string text)
    {
        string trimmed = text.Trim();
        BloodModel? model = long.TryParse(trimmed.TrimStart('#'), out long id)
            ? _context!.BloodModels.FirstOrDefault(x => x.Id == id)
            : _context!.BloodModels.FirstOrDefault(x => x.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
        return model ?? throw new ApplicationException($"Unknown blood model {text}");
    }

    private Currency LookupCurrency(string text)
    {
        string trimmed = text.Trim();
        Currency? currency = long.TryParse(trimmed.TrimStart('#'), out long id)
            ? _context!.Currencies.FirstOrDefault(x => x.Id == id)
            : _context!.Currencies.FirstOrDefault(x => x.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
        return currency ?? throw new ApplicationException($"Unknown currency {text}");
    }

    private NpcTemplate LookupNpcTemplate(Match match)
    {
        if (HasValue(match.Groups["templateid"]) && long.TryParse(match.Groups["templateid"].Value, out long id))
        {
            NpcTemplate? templateById = _context!.NpcTemplates.FirstOrDefault(x => x.Id == id);
            if (templateById is not null)
            {
                return templateById;
            }
        }

        string name = match.Groups["template"].Value.Trim();
        return _context!.NpcTemplates.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
               throw new ApplicationException($"Unknown NPC template {name}");
    }

    private long LookupSkinId(GameItemProto item, IEnumerable<string> options)
    {
        foreach (string option in options)
        {
            if (!option.StartsWith("skin ", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            GameItemSkin? skin = LookupSkin(item, option[5..]);
            if (skin is null)
            {
                return 0;
            }

            return skin.Id;
        }

        return 0;
    }

    private CraftInput GetInputByOneBasedIndex(Craft craft, int index)
    {
        if (index < 1)
        {
            throw new ApplicationException("Input indexes are one-based");
        }

        CraftInput? input = craft.CraftInputs
                                 .OrderBy(x => x.OriginalAdditionTime)
                                 .ThenBy(x => x.Id)
                                 .ElementAtOrDefault(index - 1);
        return input ?? throw new ApplicationException($"No input exists at index {index}");
    }

    private (string TypeName, string Definition) BuildInputDefinition(string text)
	{
		return BuildInputDefinition(ParseInputSpec(text));
	}

    private (string TypeName, string Definition) BuildInputDefinition(CraftInputSpec spec)
    {
		if (spec.RecipeInput is { } recipeInput) return BuildRecipeInput(recipeInput);
		if (spec.CatalogueInput is { } input)
			return ("Commodity", new XElement("Definition",
				new XElement("Material", LookupMaterial(input.Material).Id), new XElement("Weight", input.Grams),
				new XElement("CommodityPileTag", input.PileTag is null ? 0 : LookupTag(input.PileTag).Id),
				BuildCommodityInputCharacteristics([])).ToString());
        string details = spec.Details;
        Match innerMatch;
        switch (spec.InputType.ToLowerInvariant())
        {
            case "tag":
                innerMatch = RequireMatch(TagInputRegex, details, "Invalid tag input definition");
                return ("Tag", new XElement("Definition",
                    new XElement("TargetTagId", LookupTag(innerMatch.Groups["tag"].Value).Id),
                    new XElement("Quantity", ParseIntValue(innerMatch.Groups["quantity"].Value))
                ).ToString());
            case "tagvariable":
                innerMatch = RequireMatch(TagVariableInputRegex, details, "Invalid tag variable input definition");
                return ("TagVariable", new XElement("Definition",
                    new XElement("TargetTagId", LookupTag(innerMatch.Groups["tag"].Value).Id),
                    new XElement("Quantity", ParseIntValue(innerMatch.Groups["quantity"].Value)),
                    from definition in ParseCharacteristicDefinitions(innerMatch.Groups["variables"].Value)
                    select new XElement("Variable", definition.Id)
                ).ToString());
            case "conditionrepair":
            case "repair":
                innerMatch = RequireMatch(ConditionRepairInputRegex, details, "Invalid condition repair input definition");
                if (!innerMatch.Groups["repair"].Value.TryParsePercentage(out double repairAmount))
                {
                    throw new ApplicationException("Invalid repair percentage");
                }

                return ("ConditionRepair", new XElement("Definition",
                    new XElement("TargetTagId", LookupTag(innerMatch.Groups["tag"].Value).Id),
                    new XElement("RepairAmount", repairAmount)
                ).ToString());
            case "commodity":
                innerMatch = RequireMatch(CommodityInputRegex, details, "Invalid commodity input definition");
                return ("Commodity", new XElement("Definition",
                    new XElement("Material", LookupMaterial(innerMatch.Groups["material"].Value).Id),
                    new XElement("Weight", ParseMass(innerMatch)),
                    new XElement("CommodityPileTag", LookupCommodityPileTagId(spec.Options)),
                    BuildCommodityInputCharacteristics(spec.Options)
                ).ToString());
            case "liquidtaguse":
            case "tagliquid":
                innerMatch = RequireMatch(LiquidTagUseInputRegex, details, "Invalid tagged liquid input definition");
                return ("LiquidTagUse", new XElement("Definition",
                    new XElement("Tag", LookupTag(innerMatch.Groups["liquid"].Value).Id),
                    new XElement("Amount", ParseVolume(innerMatch))
                ).ToString());
            case "liquiduse":
            case "liquid":
                innerMatch = RequireMatch(LiquidUseInputRegex, details, "Invalid liquid input definition");
                return ("LiquidUse", new XElement("Definition",
                    new XElement("Liquid", LookupLiquid(innerMatch.Groups["liquid"].Value).Id),
                    new XElement("Amount", ParseVolume(innerMatch))
                ).ToString());
            case "commoditytag":
                innerMatch = RequireMatch(CommodityTagInputRegex, details, "Invalid commodity tag input definition");
                return ("CommodityTag", new XElement("Definition",
                    new XElement("MaterialTag", LookupTag(innerMatch.Groups["material"].Value).Id),
                    new XElement("Weight", ParseMass(innerMatch)),
                    new XElement("CommodityPileTag", LookupCommodityPileTagId(spec.Options)),
                    BuildCommodityInputCharacteristics(spec.Options)
                ).ToString());
            case "simpleitem":
            case "simple":
                innerMatch = RequireMatch(SimpleItemInputRegex, details, "Invalid simple item input definition");
                GameItemProto? item = LookupItem(innerMatch);
                if (item is null)
                {
                    throw new ApplicationException("Unknown simple item input item");
                }

                return ("SimpleItem", new XElement("Definition",
                    new XElement("TargetItemId", item.Id),
                    new XElement("Quantity", ParseIntValue(innerMatch.Groups["quantity"].Value))
                ).ToString());
            case "simplematerial":
                innerMatch = SimpleMaterialInputRegex.Match(details);
				if (innerMatch.Success)
				{
					return ("SimpleMaterial", new XElement("Definition",
						new XElement("TargetMaterialTag", LookupTag(innerMatch.Groups["tag"].Value).Id),
						new XElement("TargetMaterial", 0),
						new XElement("Quantity", ParseIntValue(innerMatch.Groups["quantity"].Value))
					).ToString());
				}

                innerMatch = RequireMatch(SimpleMaterialExactInputRegex, details, "Invalid simple material input definition");
                return ("SimpleMaterial", new XElement("Definition",
                    new XElement("TargetMaterialTag", 0),
                    new XElement("TargetMaterial", LookupMaterial(innerMatch.Groups["material"].Value).Id),
                    new XElement("Quantity", ParseIntValue(innerMatch.Groups["quantity"].Value))
                ).ToString());
            default:
                throw new ApplicationException("Unknown input type");
        }
    }

    private IEnumerable<CharacteristicDefinition> ParseCharacteristicDefinitions(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        foreach (string item in text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            yield return LookupCharacteristicDefinition(item);
        }
    }

    private CraftInput ConvertToInput(Craft craft, CraftInputSpec spec)
    {
        (string typename, string definition) = BuildInputDefinition(spec);
        CraftInput input = new()
        {
			Id = _deferCraftProductSave ? _nextCraftInputId++ : 0,
            Craft = craft,
            InputQualityWeight = spec.QualityWeight,
            OriginalAdditionTime = DateTime.UtcNow,
            Definition = definition,
            InputType = typename
        };
		_context!.CraftInputs.Add(input);
        return input;
    }

    private (string TypeName, DesiredItemState State, string Definition) BuildToolDefinition(string text)
	{
		return BuildToolDefinition(ParseToolSpec(text));
	}

    private (string TypeName, DesiredItemState State, string Definition) BuildToolDefinition(CraftToolSpec spec)
    {
		if (spec.RecipeTool is { } recipeTool) return BuildRecipeTool(recipeTool);
		if (spec.CatalogueTool is { } tool)
			return ("TagTool", tool.State, new XElement("Definition", new XElement("TargetItemTag", LookupTag(tool.Tag).Id)).ToString());
        Match toolMatch = RequireMatch(ToolRegex, spec.Details, "Invalid tool state definition");
        if (!toolMatch.Groups["state"].Value.TryParseEnum(out DesiredItemState state))
        {
            throw new ApplicationException("Unknown State");
        }

        switch (spec.ToolType.ToLowerInvariant())
        {
            case "tagtool":
            case "tag":
                Match innerMatch = RequireMatch(TagToolRegex, toolMatch.Groups["details"].Value, "Invalid tag tool definition");
                return ("TagTool", state, new XElement("Definition",
                    new XElement("TargetItemTag", LookupTag(innerMatch.Groups["tag"].Value).Id)
                ).ToString());
            case "simpletool":
            case "simple":
                GameItemProto item = LookupItem(toolMatch.Groups["details"].Value);
                return ("SimpleTool", state, new XElement("Definition",
                    new XElement("TargetItemId", item.Id)
                ).ToString());
            default:
                throw new ApplicationException("Unknown tool type");
        }
    }

    private CraftTool ConvertToTool(Craft craft, CraftToolSpec spec)
    {
        (string typename, DesiredItemState state, string definition) = BuildToolDefinition(spec);
        CraftTool tool = new()
        {
            Craft = craft,
            OriginalAdditionTime = DateTime.UtcNow,
            ToolType = typename,
            ToolQualityWeight = spec.QualityWeight,
            UseToolDuration = spec.UseToolDuration,
            DesiredState = (int)state,
            Definition = definition
        };
		_context!.CraftTools.Add(tool);
        return tool;
    }

    private (GameItemProto Item, int Quantity, long SkinId) ParseProductItem(string details, List<string> options)
    {
        Match innerMatch = RequireMatch(SimpleProductRegex, details, "Invalid item product definition");
        GameItemProto? item = LookupItem(innerMatch);
        if (item is null)
        {
            throw new ApplicationException("Unknown product item");
        }

        return (item, ParseIntValue(innerMatch.Groups["quantity"].Value), LookupSkinId(item, options));
    }

    private XElement BuildItemProductDefinition(string details, List<string> options)
    {
        (GameItemProto item, int quantity, long skinId) = ParseProductItem(details, options);
        return new XElement("Definition",
            new XElement("ProductProducedId", item.Id),
            new XElement("Quantity", quantity),
            new XElement("Skin", skinId)
        );
    }

    private XElement BuildCookedFoodProductDefinition(Craft craft, string details, List<string> options)
    {
        (GameItemProto item, int quantity, long skinId) = ParseProductItem(details, options);
        bool removeEffects = false;
        List<XElement> ingredientSlots = new();
        foreach (string option in options)
        {
            if (option.StartsWith("purify", StringComparison.OrdinalIgnoreCase) ||
                option.StartsWith("removedrugs", StringComparison.OrdinalIgnoreCase) ||
                option.StartsWith("removeeffects", StringComparison.OrdinalIgnoreCase) ||
                option.StartsWith("cleanse", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = option.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                removeEffects = parts.Length == 1 || !parts[1].EqualToAny("off", "false", "no");
                continue;
            }

            if (!option.StartsWith("ingredient", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (Match slot in IngredientSlotRegex.Matches(option))
            {
                CraftInput input = GetInputByOneBasedIndex(craft, ParseIntValue(slot.Groups["input"].Value));
                ingredientSlots.Add(new XElement("Slot",
                    new XAttribute("input", input.Id),
                    new XAttribute("role", slot.Groups["role"].Value.Trim().ToLowerInvariant())
                ));
            }
        }

        return new XElement("Definition",
            new XElement("ProductProducedId", item.Id),
            new XElement("Quantity", quantity),
            new XElement("Skin", skinId),
            new XElement("RemoveDrugsAndFoodEffects", removeEffects),
            new XElement("IngredientSlots", ingredientSlots)
        );
    }

    private XElement BuildProgCookedFoodProductDefinition(Craft craft, string details, List<string> options)
    {
        var prog = LookupProg(details);
        bool removeEffects = false;
        List<XElement> ingredientSlots = new();
        foreach (string option in options)
        {
            if (option.StartsWith("purify", StringComparison.OrdinalIgnoreCase) ||
                option.StartsWith("removedrugs", StringComparison.OrdinalIgnoreCase) ||
                option.StartsWith("removeeffects", StringComparison.OrdinalIgnoreCase) ||
                option.StartsWith("cleanse", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = option.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                removeEffects = parts.Length == 1 || !parts[1].EqualToAny("off", "false", "no");
                continue;
            }

            if (!option.StartsWith("ingredient", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (Match slot in IngredientSlotRegex.Matches(option))
            {
                CraftInput input = GetInputByOneBasedIndex(craft, ParseIntValue(slot.Groups["input"].Value));
                ingredientSlots.Add(new XElement("Slot",
                    new XAttribute("input", input.Id),
                    new XAttribute("role", slot.Groups["role"].Value.Trim().ToLowerInvariant())
                ));
            }
        }

        return new XElement("Definition",
            new XElement("ItemProg", prog.Id),
            new XElement("Quantity", 1),
            new XElement("Skin", 0),
            new XElement("RemoveDrugsAndFoodEffects", removeEffects),
            new XElement("IngredientSlots", ingredientSlots)
        );
    }

    private XElement BuildLiquidProductDefinition(string details, List<string> options)
    {
        Match innerMatch = RequireMatch(LiquidProductRegex, details, "Invalid liquid product definition");
        GameItemProto? item = LookupItem(innerMatch);
        if (item is null)
        {
            throw new ApplicationException("Unknown liquid product item");
        }

        double volume = ParseVolume(innerMatch);
        if (volume <= 0.0)
        {
            throw new ApplicationException("Liquid product volume must be positive");
        }

        return new XElement("Definition",
            new XElement("ProductProducedId", item.Id),
            new XElement("Quantity", ParseIntValue(innerMatch.Groups["quantity"].Value)),
            new XElement("Liquid", LookupLiquid(innerMatch.Groups["liquid"].Value).Id),
            new XElement("LiquidVolume", volume),
            new XElement("Skin", LookupSkinId(item, options))
        );
    }

    private IEnumerable<XElement> BuildSimpleVariableElements(List<string> options)
    {
        foreach (string option in options.Where(x => x.StartsWith("variable", StringComparison.OrdinalIgnoreCase)))
        {
            string text = Regex.Replace(option, @"^variables?\s+", "", RegexOptions.IgnoreCase);
            foreach (string mapping in text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                Match match = RequireMatch(VariableInputMappingRegex, mapping, "Invalid variable mapping");
                CharacteristicDefinition definition = LookupCharacteristicDefinition(match.Groups["definition"].Value);
                int inputIndex = ParseIntValue(match.Groups["input"].Value) - 1;
                yield return new XElement("Variable",
                    new XAttribute("inputindex", inputIndex),
                    definition.Id
                );
            }
        }
    }

    private XElement BuildSimpleVariableProductDefinition(string details, List<string> options)
    {
        XElement definition = BuildItemProductDefinition(details, options);
        definition.Add(BuildSimpleVariableElements(options));
		foreach (var option in options.Where(x => x.StartsWith("fixedvariable ", StringComparison.OrdinalIgnoreCase)))
		{
			var match = RequireMatch(ProgVariableMappingRegex, option["fixedvariable ".Length..].Trim(), "Invalid fixed-variable mapping");
			var characteristic = LookupCharacteristicDefinition(match.Groups["definition"].Value);
			var value = LookupCharacteristicValue(characteristic, match.Groups["prog"].Value);
			definition.Add(new XElement("FixedVariable", new XAttribute("value", value.Id), characteristic.Id));
		}
		var rules = definition.Elements().Where(x => x.Name == "Variable" || x.Name == "FixedVariable").ToArray();
		if (rules.Select(x => x.Value).Distinct().Count() != rules.Length)
		{
			throw new ApplicationException("Each variable-product characteristic must have exactly one input or selected-value rule.");
		}
		if (rules.Where(x => x.Name == "Variable").Any(x => (int)x.Attribute("inputindex")! < 0))
		{
			throw new ApplicationException("Variable-product source inputs are one-based and must be positive.");
		}
        return definition;
    }

    private XElement BuildInputVariableProductDefinition(string details, List<string> options)
    {
        (GameItemProto item, int quantity, long skinId) = ParseProductItem(details, options);
        Dictionary<CharacteristicDefinition, int> variableIndexes = new();
        Dictionary<CharacteristicDefinition, List<(long Proto, long Value)>> variableSpecifics = new();

        foreach (string option in options)
        {
            if (option.StartsWith("variable", StringComparison.OrdinalIgnoreCase))
            {
                string text = Regex.Replace(option, @"^variables?\s+", "", RegexOptions.IgnoreCase);
                foreach (string mapping in text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    Match match = RequireMatch(VariableInputMappingRegex, mapping, "Invalid input-variable index mapping");
                    variableIndexes[LookupCharacteristicDefinition(match.Groups["definition"].Value)] = ParseIntValue(match.Groups["input"].Value);
                }
                continue;
            }

            if (!option.StartsWith("specific", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            Match specificMatch = RequireMatch(InputVariableSpecificRegex, option["specific".Length..].Trim(), "Invalid input-variable specific mapping");
            CharacteristicDefinition definition = LookupCharacteristicDefinition(specificMatch.Groups["definition"].Value);
            if (!variableSpecifics.ContainsKey(definition))
            {
                variableSpecifics[definition] = new List<(long Proto, long Value)>();
            }

            foreach (string mapping in specificMatch.Groups["specifics"].Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                Match itemMatch = RequireMatch(SpecificVariableMappingRegex, mapping, "Invalid item-to-value mapping");
                GameItemProto specificItem = LookupItem(itemMatch.Groups["item"].Value);
                CharacteristicValue value = LookupCharacteristicValue(definition, itemMatch.Groups["value"].Value);
                variableSpecifics[definition].Add((specificItem.Id, value.Id));
            }
        }

        return new XElement("Definition",
            new XElement("ProductProducedId", item.Id),
            new XElement("Quantity", quantity),
            new XElement("Skin", skinId),
            from entry in variableIndexes
            select new XElement("Variable",
                new XAttribute("definition", entry.Key.Id),
                new XAttribute("index", entry.Value),
                from specific in variableSpecifics.GetValueOrDefault(entry.Key, [])
                select new XElement("Specific",
                    new XAttribute("value", specific.Value),
                    new XAttribute("proto", specific.Proto)
                )
            )
        );
    }

    private XElement BuildProgVariableProductDefinition(string details, List<string> options)
    {
        XElement definition = BuildItemProductDefinition(details, options);
        foreach (string option in options.Where(x => x.StartsWith("variable", StringComparison.OrdinalIgnoreCase)))
        {
            string text = Regex.Replace(option, @"^variables?\s+", "", RegexOptions.IgnoreCase);
            foreach (string mapping in text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                Match match = RequireMatch(ProgVariableMappingRegex, mapping, "Invalid prog-variable mapping");
                CharacteristicDefinition variable = LookupCharacteristicDefinition(match.Groups["definition"].Value);
                FutureProg prog = LookupProg(match.Groups["prog"].Value);
                definition.Add(new XElement("Variable",
                    new XAttribute("prog", prog.Id),
                    new XAttribute("inputindex", prog.Id),
                    variable.Id
                ));
            }
        }

        return definition;
    }

    private IEnumerable<XElement> BuildCommodityCharacteristicElements(Craft craft, List<string> options)
    {
        foreach (string option in options.Where(x => x.StartsWith("characteristic", StringComparison.OrdinalIgnoreCase)))
        {
            string text = option["characteristic".Length..].Trim();
            Match fromMatch = Regex.Match(text, @"^(?<definition>.+?)\s+from\s+\$i(?<input>\d+)$", RegexOptions.IgnoreCase);
            if (fromMatch.Success)
            {
                CharacteristicDefinition definition = LookupCharacteristicDefinition(fromMatch.Groups["definition"].Value);
                yield return new XElement("Characteristic",
                    new XAttribute("definition", definition.Id),
                    new XAttribute("value", 0),
                    new XAttribute("input", ParseIntValue(fromMatch.Groups["input"].Value) - 1)
                );
                continue;
            }

            Match valueMatch = RequireMatch(ProgVariableMappingRegex, text, "Invalid commodity characteristic mapping");
            CharacteristicDefinition fixedDefinition = LookupCharacteristicDefinition(valueMatch.Groups["definition"].Value);
            CharacteristicValue value = LookupCharacteristicValue(fixedDefinition, valueMatch.Groups["prog"].Value);
            yield return new XElement("Characteristic",
                new XAttribute("definition", fixedDefinition.Id),
                new XAttribute("value", value.Id),
                new XAttribute("input", -1)
            );
        }
    }

    private XElement BuildCommodityProductDefinition(Craft craft, string details, List<string> options)
    {
        Match innerMatch = RequireMatch(CommodityProductRegex, details, "Invalid commodity product definition");
        long tagId = 0;
        foreach (string option in options.Where(x => x.StartsWith("tag ", StringComparison.OrdinalIgnoreCase)))
        {
            tagId = LookupTag(option[4..]).Id;
        }

        return new XElement("Definition",
            new XElement("Material", LookupMaterial(innerMatch.Groups["material"].Value).Id),
            new XElement("Weight", ParseMass(innerMatch)),
            new XElement("Tag", tagId),
            new XElement("Characteristics", BuildCommodityCharacteristicElements(craft, options))
        );
    }

    private (string TypeName, string Definition) BuildProductDefinition(Craft craft, string text)
	{
		return BuildProductDefinition(craft, ParseProductSpec(text, false));
	}

    private (string TypeName, string Definition) BuildProductDefinition(Craft craft, CraftProductSpec spec)
    {
		if (spec.RecipeProduct is { } recipeProduct) return BuildRecipeProduct(craft, recipeProduct);
		if (spec.CatalogueProduct is { } product)
			return ("SimpleProduct", new XElement("Definition",
				new XElement("ProductProducedId", LookupReworkItem(product.ItemStableReference).Id),
				new XElement("Quantity", product.Quantity), new XElement("Skin", 0)).ToString());
        string details = spec.Details;
		List<string> options = spec.Options.ToList();
        Match innerMatch;
        CraftInput input;
        switch (spec.ProductType.ToLowerInvariant())
        {
            case "simpleproduct":
            case "simple":
                return ("SimpleProduct", BuildItemProductDefinition(details, options).ToString());
            case "cookedfoodproduct":
            case "cookedfood":
            case "cooked":
                return ("CookedFoodProduct", BuildCookedFoodProductDefinition(craft, details, options).ToString());
            case "progcookedfoodproduct":
            case "progcookedfood":
            case "prog cooked food":
                return ("ProgCookedFoodProduct", BuildProgCookedFoodProductDefinition(craft, details, options).ToString());
            case "liquidproduct":
            case "liquid":
            case "liquidoutput":
            case "filledliquid":
                return ("LiquidProduct", BuildLiquidProductDefinition(details, options).ToString());
            case "simplevariableproduct":
            case "simplevariable":
            case "variable":
                return ("SimpleVariableProduct", BuildSimpleVariableProductDefinition(details, options).ToString());
            case "inputvariableproduct":
            case "inputvariable":
                return ("InputVariable", BuildInputVariableProductDefinition(details, options).ToString());
            case "progvariableproduct":
            case "progvariable":
                return ("ProgVariableProduct", BuildProgVariableProductDefinition(details, options).ToString());
            case "commodityproduct":
            case "commodity":
                return ("CommodityProduct", BuildCommodityProductDefinition(craft, details, options).ToString());
            case "moneyproduct":
            case "money":
                innerMatch = RequireMatch(MoneyProductRegex, details, "Invalid money product definition");
                return ("MoneyProduct", new XElement("Definition",
                    new XElement("Currency", LookupCurrency(innerMatch.Groups["currency"].Value).Id),
                    new XElement("Amount", ParseDecimalValue(innerMatch.Groups["amount"].Value))
                ).ToString());
            case "npcproduct":
            case "npc":
                innerMatch = RequireMatch(NpcProductRegex, details, "Invalid NPC product definition");
                NpcTemplate template = LookupNpcTemplate(innerMatch);
                long onLoadProg = 0;
                foreach (string option in options.Where(x => x.StartsWith("prog ", StringComparison.OrdinalIgnoreCase)))
                {
                    onLoadProg = LookupProg(option[5..]).Id;
                }

                return ("NPCProduct", new XElement("Definition",
                    new XElement("Quantity", ParseIntValue(innerMatch.Groups["quantity"].Value)),
                    new XElement("Template", template.Id),
                    new XElement("OnLoadProg", onLoadProg)
                ).ToString());
            case "prog":
            case "progproduct":
                return ("Prog", new XElement("Definition",
                    new XElement("ItemProg", LookupProg(details).Id)
                ).ToString());
            case "dnatest":
                innerMatch = RequireMatch(DnaTestProductRegex, details, "Invalid DNA test product definition");
                CraftInput input1 = GetInputByOneBasedIndex(craft, ParseIntValue(innerMatch.Groups["input1"].Value));
                CraftInput input2 = GetInputByOneBasedIndex(craft, ParseIntValue(innerMatch.Groups["input2"].Value));
                return ("DNATest", new XElement("Definition",
                    new XElement("WhichInputId1", input1.Id),
                    new XElement("WhichInputId2", input2.Id)
                ).ToString());
            case "scrapinput":
            case "scrap":
                innerMatch = RequireMatch(ScrapInputProductRegex, details, "Invalid scrap input product definition");
                if (!innerMatch.Groups["percentage"].Value.TryParsePercentage(out double percentage))
                {
                    throw new ApplicationException("Invalid percentage");
                }

                input = GetInputByOneBasedIndex(craft, ParseIntValue(innerMatch.Groups["input"].Value));
                long scrapTagId = 0;
                foreach (string option in options.Where(x => x.StartsWith("tag ", StringComparison.OrdinalIgnoreCase)))
                {
                    scrapTagId = LookupTag(option[4..]).Id;
                }

                return ("ScrapInput", new XElement("Definition",
                    new XElement("WhichInputId", input.Id),
                    new XElement("PercentageRecovered", percentage),
                    new XElement("Tag", scrapTagId)
                ).ToString());
            case "bloodtyping":
                innerMatch = BloodTypingProductRegex.Match(details);
                if (!innerMatch.Success)
                {
                    innerMatch = RequireMatch(SimpleBloodTypingProductRegex, details, "Invalid blood typing product definition");
                }

                input = GetInputByOneBasedIndex(craft, ParseIntValue(innerMatch.Groups["input"].Value));
                return ("BloodTyping", new XElement("Definition",
                    new XElement("WhichInputId", input.Id),
                    new XElement("BloodModel", LookupBloodModel(innerMatch.Groups["model"].Value).Id)
                ).ToString());
            case "unusedinput":
                innerMatch = RequireMatch(UnusedInputProductRegex, details, "Invalid unused input product definition");
                if (!innerMatch.Groups["percentage"].Value.TryParsePercentage(out percentage))
                {
                    throw new ApplicationException("Invalid percentage");
                }

                input = GetInputByOneBasedIndex(craft, ParseIntValue(innerMatch.Groups["input"].Value));
                return ("UnusedInput", new XElement("Definition",
                    new XElement("WhichInputId", input.Id),
                    new XElement("PercentageRecovered", percentage)
                ).ToString());
            default:
                throw new ApplicationException("Unknown product type");
        }
    }

    private CraftProduct ConvertToProduct(Craft craft, CraftProductSpec spec)
    {
        (string typename, string definition) = BuildProductDefinition(craft, spec);
        return new CraftProduct()
        {
            Craft = craft,
            IsFailProduct = spec.IsFailProduct,
            OriginalAdditionTime = DateTime.UtcNow,
            ProductType = typename,
            Definition = definition,
            MaterialDefiningInputIndex = spec.MaterialDefiningInputIndex
        };
    }

    private MudSharp.Models.Craft? AddCraft(string name, string category, string blurb, string action, string itemsdesc, string appearProg, string? canUseProg, string? whyCantProg, string? onFinishProg, MudSharp.Models.TraitDefinition trait, Difficulty difficulty, Outcome threshold, int freeChecks, int failPhase, bool interrupatable, IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs, IEnumerable<string> tools, IEnumerable<string> products, IEnumerable<string> failProducts, List<(int Product, int Input)>? productMaterialInputIndexes = null, List<(int Product, int Input)>? failProductMaterialInputIndexes = null, string? onStartProg = null, string? onCancelProg = null)
    {
        List<CraftValidationError> importErrors = [];
		var inputSpecs = ParseImportSpecs(inputs, name, "input", ParseInputSpec, importErrors);
		var toolSpecs = ParseImportSpecs(tools, name, "tool", ParseToolSpec, importErrors);
		var productSpecs = ParseProductSpecs(products, name, false, productMaterialInputIndexes, importErrors);
		var failProductSpecs = ParseProductSpecs(failProducts, name, true, failProductMaterialInputIndexes, importErrors);

		var spec = new CraftDefinitionSpec
		{
			Name = name,
			Category = category,
			Blurb = blurb,
			Action = action,
			ActiveCraftItemSdesc = itemsdesc,
			AppearProg = LookupProg(appearProg),
			CanUseProg = canUseProg is not null ? LookupProg(canUseProg) : null,
			WhyCannotUseProg = whyCantProg is not null ? LookupProg(whyCantProg) : null,
			OnStartProg = onStartProg is not null ? LookupProg(onStartProg) : null,
			OnFinishProg = onFinishProg is not null ? LookupProg(onFinishProg) : null,
			OnCancelProg = onCancelProg is not null ? LookupProg(onCancelProg) : null,
			Trait = trait,
			Difficulty = difficulty,
			Threshold = threshold,
			FreeChecks = freeChecks,
			FailPhase = failPhase,
			Interruptable = interrupatable,
			Phases = phases.Select(x => new CraftPhaseSpec
			{
				Seconds = x.Seconds,
				Echo = x.Echo,
				FailEcho = x.FailEcho
			}).ToList(),
			Inputs = inputSpecs,
			Tools = toolSpecs,
			Products = productSpecs,
			FailProducts = failProductSpecs
		};

		return AddCraft(spec, importErrors);
    }

	private MudSharp.Models.Craft? AddCraft(string name, string category, string blurb, string action, string itemsdesc, string traitName, int? minimumTraitValue, Difficulty difficulty, Outcome threshold, int freeChecks, int failPhase, bool interrupatable, IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs, IEnumerable<string> tools, IEnumerable<string> products, IEnumerable<string> failProducts, List<(int Product, int Input)>? productMaterialInputIndexes = null, List<(int Product, int Input)>? failProductMaterialInputIndexes = null, string? onFinishProg = null, string? onStartProg = null, string? onCancelProg = null)
	{
		(FutureProg appearProg, FutureProg canUseProg, FutureProg whyCannotUseProg, TraitDefinition trait) =
			EnsureTraitGateProgs(traitName, minimumTraitValue);
		return AddCraft(
			name,
			category,
			blurb,
			action,
			itemsdesc,
			appearProg.FunctionName,
			canUseProg.FunctionName,
			whyCannotUseProg.FunctionName,
			onFinishProg,
			trait,
			difficulty,
			threshold,
			freeChecks,
			failPhase,
			interrupatable,
			phases,
			inputs,
			tools,
			products,
			failProducts,
			productMaterialInputIndexes,
			failProductMaterialInputIndexes,
			onStartProg,
			onCancelProg
		);
	}

	private MudSharp.Models.Craft? AddCraft(string name, string category, string blurb, string action,
		string itemsdesc, string knowledgeName, string traitName, int? minimumTraitValue, Difficulty difficulty,
		Outcome threshold, int freeChecks, int failPhase, bool interrupatable,
		IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
		IEnumerable<string> tools, IEnumerable<string> products, IEnumerable<string> failProducts,
		List<(int Product, int Input)>? productMaterialInputIndexes = null,
		List<(int Product, int Input)>? failProductMaterialInputIndexes = null, string? onFinishProg = null,
		string? onStartProg = null, string? onCancelProg = null, string knowledgeType = "Crafting",
		string knowledgeSubtype = "General", string? knowledgeDescription = null, string? knowledgeLongDescription = null,
		int knowledgeLearningSessionsRequired = 1, Difficulty knowledgeLearnDifficulty = Difficulty.Normal,
		Difficulty knowledgeTeachDifficulty = Difficulty.Normal,
		LearnableType knowledgeLearnable = LearnableType.LearnableAtSkillUp | LearnableType.LearnableFromTeacher)
	{
		(FutureProg appearProg, FutureProg canUseProg, FutureProg whyCannotUseProg, TraitDefinition trait, _) =
			EnsureKnowledgeGateProgs(
				knowledgeName,
				traitName,
				minimumTraitValue,
				knowledgeType,
				knowledgeSubtype,
				knowledgeDescription,
				knowledgeLongDescription,
				knowledgeLearningSessionsRequired,
				knowledgeLearnDifficulty,
				knowledgeTeachDifficulty,
				knowledgeLearnable);
		return AddCraft(
			name,
			category,
			blurb,
			action,
			itemsdesc,
			appearProg.FunctionName,
			canUseProg.FunctionName,
			whyCannotUseProg.FunctionName,
			onFinishProg,
			trait,
			difficulty,
			threshold,
			freeChecks,
			failPhase,
			interrupatable,
			phases,
			inputs,
			tools,
			products,
			failProducts,
			productMaterialInputIndexes,
			failProductMaterialInputIndexes,
			onStartProg,
			onCancelProg
		);
	}

	private List<TSpec> ParseImportSpecs<TSpec>(IEnumerable<string> imports, string craftName, string kind,
		Func<string, TSpec> parser, List<CraftValidationError> errors)
	{
		List<TSpec> specs = [];
		foreach (string import in imports)
		{
			try
			{
				specs.Add(parser(import));
			}
			catch (ApplicationException ex)
			{
				errors.Add(new CraftValidationError(craftName, kind, import, ex.Message));
			}
		}

		return specs;
	}

	private List<CraftProductSpec> ParseProductSpecs(IEnumerable<string> imports, string craftName, bool isFailProduct,
		List<(int Product, int Input)>? materialInputIndexes, List<CraftValidationError> errors)
	{
		List<CraftProductSpec> specs = [];
		int index = 0;
		foreach (string import in imports)
		{
			index++;
			try
			{
				specs.Add(ParseProductSpec(import, isFailProduct, GetMaterialDefiningInputIndex(materialInputIndexes, index)));
			}
			catch (ApplicationException ex)
			{
				errors.Add(new CraftValidationError(craftName, isFailProduct ? "fail product" : "product", import, ex.Message));
			}
		}

		return specs;
	}

	private static CraftDefinitionSpec NormaliseCraftDefinition(CraftDefinitionSpec spec)
	{
		if (spec.PreserveAuthoredText) return spec;
		if (spec.Phases.Count == 0)
		{
			return spec;
		}

		var phases = spec.Phases.ToArray();
		var missingToolIndexes = Enumerable.Range(1, spec.Tools.Count)
			.Where(toolIndex => phases.All(phase => !ReferencesTool(phase.Echo, toolIndex)))
			.ToList();
		if (missingToolIndexes.Count > 0)
		{
			var workPhaseIndex = Math.Clamp(spec.FailPhase - 2, 0, phases.Length - 1);
			phases[workPhaseIndex] = phases[workPhaseIndex] with
			{
				Echo = AppendToolReferences(phases[workPhaseIndex].Echo, missingToolIndexes)
			};
		}

		for (var phaseIndex = 0; phaseIndex < Math.Min(spec.FailPhase - 1, phases.Length); phaseIndex++)
		{
			phases[phaseIndex] = phases[phaseIndex] with { FailEcho = phases[phaseIndex].Echo };
		}

		return spec with
		{
			ActiveCraftItemSdesc = NormaliseActiveCraftItemSdesc(spec.ActiveCraftItemSdesc),
			Phases = phases
		};
	}

	private static bool ReferencesTool(string echo, int toolIndex)
	{
		return Regex.IsMatch(echo, $@"\$t{toolIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)}(?!\d)", RegexOptions.IgnoreCase);
	}

	private static string AppendToolReferences(string echo, IReadOnlyList<int> toolIndexes)
	{
		var references = toolIndexes
			.Select(index => $"$t{index.ToString(System.Globalization.CultureInfo.InvariantCulture)}")
			.ToList();
		var referenceText = references.Count switch
		{
			1 => references[0],
			2 => string.Join(" and ", references),
			_ => $"{string.Join(", ", references.Take(references.Count - 1))} and {references[^1]}"
		};
		var sentence = echo.TrimEnd();
		sentence = sentence.TrimEnd('.', '!', '?');
		return $"{sentence}, using {referenceText}.";
	}

	private static string NormaliseActiveCraftItemSdesc(string activeCraftItemSdesc)
	{
		var text = activeCraftItemSdesc.Trim();
		if (text.EndsWith(" in progress", StringComparison.OrdinalIgnoreCase))
		{
			text = text[..^" in progress".Length];
		}
		else if (text.EndsWith(" being made", StringComparison.OrdinalIgnoreCase))
		{
			text = text[..^" being made".Length];
		}
		else if (text.EndsWith(" process", StringComparison.OrdinalIgnoreCase))
		{
			text = text[..^" process".Length];
		}
		else
		{
			return activeCraftItemSdesc;
		}

		var subject = Regex.Replace(text.Trim(), @"^(?:a|an|the)\s+", string.Empty, RegexOptions.IgnoreCase);
		subject = Regex.Replace(subject,
			@"^(?:assemble|build|construct|craft|create|cut|forge|form|make|prepare|process|sew|shape|smelt)\s+",
			string.Empty, RegexOptions.IgnoreCase);
		subject = Regex.Replace(subject, @"\s+into\s+", " ", RegexOptions.IgnoreCase);
		subject = Regex.Replace(subject, @"\s+", " ").Trim();
		return string.IsNullOrWhiteSpace(subject) ? activeCraftItemSdesc : $"an in-progress {subject} craft";
	}

	private MudSharp.Models.Craft AddCraft(CraftDefinitionSpec spec, IEnumerable<CraftValidationError>? importErrors = null)
	{
		spec = NormaliseCraftDefinition(spec);
		List<CraftValidationError> errors = _manifestCaptureOnly
			? []
			: importErrors?.ToList() ?? [];
		if (!_manifestCaptureOnly)
		{
			errors.AddRange(ValidateCraftSpec(spec));
		}
		if (errors.Count > 0)
		{
			throw new ApplicationException(
				$"Craft '{spec.Name}' has {errors.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)} validation error(s):\n - {string.Join("\n - ", errors.Select(x => x.ToString()))}");
		}

		var stableKey = spec.StableReference ?? $"{spec.Category.Trim()}/{spec.Name.Trim()}";
		if (CatalogueCraftAuthoringTrace is not null && spec.AppearProg is not null &&
		    _capturedCraftGates.TryGetValue(spec.AppearProg.FunctionName, out var capturedGate))
			_capturedRecipeGates.TryAdd(stableKey, capturedGate);
		CatalogueCraftAuthoringTrace?.Invoke(new
		{
			StableKey = stableKey, Module = _activeManifestModule, EraAdmissions = _activeManifestEras,
			spec.StableReference, spec.PreserveAuthoredText,
			Gate = spec.AppearProg is null ? null : _capturedCraftGates.GetValueOrDefault(spec.AppearProg.FunctionName),
			Definition = BuildCraftManifestDefinition(spec), CallStack = Environment.StackTrace
		});
		_definitionCapture?.Supplement("craft", stableKey, new
		{
			ExplicitStableReference = spec.StableReference, spec.PreserveAuthoredText,
			QualityFormula = "5 + (outcome/3) + (variable/20)", CheckQualityWeighting = 1.0,
			InputQualityWeighting = 1.0, ToolQualityWeighting = 1.0, IsPracticalCheck = true
		});
		var existing = !_manifestCaptureOnly && spec.StableReference is not null
			? ResolveOwnedCraftTarget(stableKey, spec.Name, spec.Category, _context!.Crafts.Local.ToArray())
			: FindExistingCraft(spec.Name, spec.Category);
		if (!_manifestCaptureOnly && spec.StableReference is not null)
			ValidateUnmanagedCraftSignature(stableKey, existing, spec);
		if (IsManifestAggregateRegistered("craft", stableKey))
		{
			return existing ??
			       _craftsByNameAndCategory[CraftLookupKey(spec.Name, spec.Category)];
		}

		var manifestDefinition = BuildCraftManifestDefinition(spec);
		var manifestEntry = RegisterManifestAggregate(
			"craft",
			stableKey,
			manifestDefinition,
			GetCraftProductManifestDependencies(spec));
		var managedRecord = FindManagedRecord(manifestEntry.EntityType, manifestEntry.StableKey);
		if (_manifestCaptureOnly)
		{
			if (existing is not null)
			{
				return existing;
			}

			var capturedCraft = new Craft
			{
				Id = _nextCraftId++,
				RevisionNumber = 0,
				Name = spec.Name,
				Category = spec.Category
			};
			_craftsByNameAndCategory[CraftLookupKey(spec.Name, spec.Category)] = capturedCraft;
			return capturedCraft;
		}
		if (existing is not null)
		{
			var liveFingerprint = ItemSeederManifestCatalogue.Fingerprint(BuildLiveCraftManifestDefinition(existing));
			if (managedRecord is not null && managedRecord.LogicalId is not null && managedRecord.LogicalId != existing.Id)
			{
				IncrementManifestResult(manifestEntry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"ItemSeeder ownership conflict for craft:{stableKey}: provenance names logical ID {managedRecord.LogicalId:N0}, but the natural key resolves to {existing.Id:N0}.");
			}

			if (managedRecord is not null &&
			    !liveFingerprint.Equals(managedRecord.AppliedFingerprint, StringComparison.OrdinalIgnoreCase))
			{
				if (IsRepairableMissingCraftStock(existing, spec))
				{
					RebuildCraft(existing, spec);
					var repairedFingerprint = ItemSeederManifestCatalogue.Fingerprint(BuildLiveCraftManifestDefinition(existing));
					RecordAppliedManifestEntry(manifestEntry, existing.Id, existing.RevisionNumber, repairedFingerprint);
					IncrementManifestResult(manifestEntry.Module, x => x with { Updated = x.Updated + 1 });
					QueueDeferredCraftPersistence();
					return existing;
				}

				MarkManifestAggregateCustomized(manifestEntry.EntityType, manifestEntry.StableKey);
				IncrementManifestResult(manifestEntry.Module, x => x with { Customized = x.Customized + 1 });
				return existing;
			}

			CraftLiveDefinition expectedDefinition;
			try
			{
				expectedDefinition = BuildExpectedLiveCraftDefinition(existing, spec);
			}
			catch (Exception exception) when (managedRecord is null)
			{
				IncrementManifestResult(manifestEntry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"Unmanaged craft conflict for '{stableKey}'. The legacy row cannot be proven to have the complete stock signature and will not be claimed or overwritten.",
					exception);
			}

			var expectedFingerprint = ItemSeederManifestCatalogue.Fingerprint(expectedDefinition);
			if (managedRecord is null && !liveFingerprint.Equals(expectedFingerprint, StringComparison.OrdinalIgnoreCase))
			{
				IncrementManifestResult(manifestEntry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"Unmanaged craft conflict for '{stableKey}'. The legacy row does not have the complete stock signature and will not be claimed or overwritten.");
			}

			if (liveFingerprint.Equals(expectedFingerprint, StringComparison.OrdinalIgnoreCase))
			{
				RecordAppliedManifestEntry(manifestEntry, existing.Id, existing.RevisionNumber, liveFingerprint);
				IncrementManifestResult(manifestEntry.Module, x => x with { Unchanged = x.Unchanged + 1 });
				return existing;
			}

			RebuildCraft(existing, spec);
			var rebuiltFingerprint = ItemSeederManifestCatalogue.Fingerprint(BuildLiveCraftManifestDefinition(existing));
			RecordAppliedManifestEntry(manifestEntry, existing.Id, existing.RevisionNumber, rebuiltFingerprint);
			IncrementManifestResult(manifestEntry.Module, x => x with { Updated = x.Updated + 1 });
			QueueDeferredCraftPersistence();
			return existing;
		}

        Craft dbitem = new()
        {
			Id = _nextCraftId++,
            RevisionNumber = 0,
            EditableItem = new EditableItem
            {
                RevisionNumber = 0,
                RevisionStatus = 4,
                BuilderAccountId = _dbAccount.Id,
                BuilderDate = _now,
                BuilderComment = "Auto-generated by the system",
                ReviewerAccountId = _dbAccount.Id,
                ReviewerComment = "Auto-generated by the system",
                ReviewerDate = _now
            },
		};
		_context!.Crafts.Add(dbitem);
		RebuildCraft(dbitem, spec, removeExistingChildren: false);
		_craftsByNameAndCategory[CraftLookupKey(dbitem.Name, dbitem.Category)] = dbitem;
		var appliedFingerprint = ItemSeederManifestCatalogue.Fingerprint(BuildLiveCraftManifestDefinition(dbitem));
		RecordAppliedManifestEntry(manifestEntry, dbitem.Id, dbitem.RevisionNumber, appliedFingerprint);
		IncrementManifestResult(manifestEntry.Module, x => x with { Inserted = x.Inserted + 1 });
		QueueDeferredCraftPersistence();
		return dbitem;
	}

	private IEnumerable<string> GetCraftProductManifestDependencies(CraftDefinitionSpec spec)
	{
		if (spec.StableReference is not null)
		{
			foreach (var reference in spec.Inputs.Select(x => x.Details).Concat(spec.Products.Concat(spec.FailProducts).Select(x => x.Details))
				.SelectMany(x => Regex.Matches(x, @"@(?<reference>[a-z][a-z0-9_]*)").Select(m => m.Groups["reference"].Value))
				.Distinct(StringComparer.Ordinal)) yield return $"item:{reference}";
			foreach (var skin in spec.Products.Concat(spec.FailProducts).SelectMany(x => x.Options)
				.Where(x => x.StartsWith("skin ", StringComparison.Ordinal)).Select(x => x[5..].TrimStart('@')).Distinct(StringComparer.Ordinal))
				yield return $"item-skin:{skin}";
			yield break;
		}
		foreach (var product in spec.Products)
		{
			var match = Regex.Match(product.Details, @"\(#(?<id>\d+)\)");
			if (!match.Success || !long.TryParse(match.Groups["id"].Value, out var itemId) ||
			    !_itemStableReferencesById.TryGetValue(itemId, out var stableReference))
			{
				continue;
			}

			yield return $"item:{stableReference}";
		}
	}

	private bool IsRepairableMissingCraftStock(Craft craft, CraftDefinitionSpec spec)
	{
		if (!craft.Name.Equals(spec.Name, StringComparison.Ordinal) ||
		    !craft.Category.Equals(spec.Category, StringComparison.Ordinal) ||
		    craft.Blurb != spec.Blurb ||
		    craft.ActionDescription != spec.Action ||
		    craft.ActiveCraftItemSdesc != spec.ActiveCraftItemSdesc ||
		    craft.AppearInCraftsListProgId != spec.AppearProg!.Id ||
		    craft.CanUseProgId != spec.CanUseProg?.Id ||
		    craft.WhyCannotUseProgId != spec.WhyCannotUseProg?.Id ||
		    craft.OnUseProgStartId != spec.OnStartProg?.Id ||
		    craft.OnUseProgCompleteId != spec.OnFinishProg?.Id ||
		    craft.OnUseProgCancelId != spec.OnCancelProg?.Id ||
		    craft.CheckTraitId != spec.Trait!.Id ||
		    craft.CheckDifficulty != (int)spec.Difficulty ||
		    craft.FailThreshold != (int)spec.Threshold ||
		    craft.FreeSkillChecks != spec.FreeChecks ||
		    craft.FailPhase != spec.FailPhase ||
		    craft.Interruptable != spec.Interruptable ||
		    craft.QualityFormula != "5 + (outcome/3) + (variable/20)" ||
		    craft.CheckQualityWeighting != 1.0 ||
		    craft.InputQualityWeighting != 1.0 ||
		    craft.ToolQualityWeighting != 1.0 ||
		    !craft.IsPracticalCheck)
		{
			return false;
		}

		var expectedProductCount = spec.Products.Count + spec.FailProducts.Count;
		var hasDeletion = craft.CraftPhases.Count < spec.Phases.Count ||
		                  craft.CraftInputs.Count < spec.Inputs.Count ||
		                  craft.CraftTools.Count < spec.Tools.Count ||
		                  craft.CraftProducts.Count < expectedProductCount;
		if (!hasDeletion || craft.CraftPhases.Count > spec.Phases.Count ||
		    craft.CraftInputs.Count > spec.Inputs.Count || craft.CraftTools.Count > spec.Tools.Count ||
		    craft.CraftProducts.Count > expectedProductCount)
		{
			return false;
		}

		var expectedPhases = spec.Phases.Select((x, index) => new CraftLivePhase(
			index + 1, x.Seconds, x.Echo, string.IsNullOrWhiteSpace(x.FailEcho) ? x.Echo : x.FailEcho,
			(int)x.Exertion, x.Stamina)).ToDictionary(x => x.Number);
		if (craft.CraftPhases.Any(x => !expectedPhases.TryGetValue(x.PhaseNumber, out var expected) ||
		    expected != new CraftLivePhase(x.PhaseNumber, x.PhaseLengthInSeconds, x.Echo, x.FailEcho,
			    x.ExertionLevel, x.StaminaUsage)))
		{
			return false;
		}

		var expectedInputs = spec.Inputs.Select(x =>
		{
			var built = BuildInputDefinition(x);
			return new CraftLiveInput(built.TypeName, x.QualityWeight, built.Definition);
		}).ToArray();
		var liveInputs = craft.CraftInputs.OrderBy(x => x.OriginalAdditionTime).ThenBy(x => x.Id)
			.Select(x => new CraftLiveInput(x.InputType, x.InputQualityWeight, x.Definition)).ToArray();
		if (!liveInputs.SequenceEqual(expectedInputs.Take(liveInputs.Length)))
		{
			return false;
		}

		var expectedTools = spec.Tools.Select(x =>
		{
			var built = BuildToolDefinition(x);
			return new CraftLiveTool(built.TypeName, x.QualityWeight, (int)built.State, x.UseToolDuration,
				built.Definition);
		}).ToArray();
		var liveTools = craft.CraftTools.OrderBy(x => x.OriginalAdditionTime).ThenBy(x => x.Id)
			.Select(x => new CraftLiveTool(x.ToolType, x.ToolQualityWeight, x.DesiredState, x.UseToolDuration,
				x.Definition)).ToArray();
		return liveTools.SequenceEqual(expectedTools.Take(liveTools.Length));
	}

	private CraftLiveDefinition BuildExpectedLiveCraftDefinition(Craft craft, CraftDefinitionSpec spec)
	{
		var inputs = spec.Inputs
			.Select(x =>
			{
				var definition = BuildInputDefinition(x);
				return new CraftLiveInput(definition.TypeName, x.QualityWeight, definition.Definition);
			})
			.ToArray();
		var tools = spec.Tools
			.Select(x =>
			{
				var definition = BuildToolDefinition(x);
				return new CraftLiveTool(
					definition.TypeName,
					x.QualityWeight,
					(int)definition.State,
					x.UseToolDuration,
					definition.Definition);
			})
			.ToArray();
		var products = spec.Products.Concat(spec.FailProducts)
			.Select(x =>
			{
				var definition = BuildProductDefinition(craft, x);
				return new CraftLiveProduct(
					definition.TypeName,
					x.IsFailProduct,
					x.MaterialDefiningInputIndex,
					definition.Definition);
			})
			.OrderBy(x => x.IsFailProduct)
			.ToArray();

		return new CraftLiveDefinition(
			spec.Name,
			spec.Category,
			spec.Blurb,
			spec.Action,
			spec.ActiveCraftItemSdesc,
			spec.AppearProg!.Id,
			spec.CanUseProg?.Id,
			spec.WhyCannotUseProg?.Id,
			spec.OnStartProg?.Id,
			spec.OnFinishProg?.Id,
			spec.OnCancelProg?.Id,
			spec.Trait!.Id,
			(int)spec.Difficulty,
			(int)spec.Threshold,
			spec.FreeChecks,
			spec.FailPhase,
			spec.Interruptable,
			"5 + (outcome/3) + (variable/20)",
			1.0,
			1.0,
			1.0,
			true,
			spec.Phases.Select((x, index) => new CraftLivePhase(
				index + 1,
				x.Seconds,
				x.Echo,
				string.IsNullOrWhiteSpace(x.FailEcho) ? x.Echo : x.FailEcho,
				(int)x.Exertion,
				x.Stamina)).ToArray(),
			inputs,
			tools,
			products);
	}

	private void RebuildCraft(Craft craft, CraftDefinitionSpec spec, bool removeExistingChildren = true)
	{
		if (!string.IsNullOrEmpty(craft.Name) && !string.IsNullOrEmpty(craft.Category))
		{
			var previousKey = CraftLookupKey(craft.Name, craft.Category);
			if (_craftsByNameAndCategory.GetValueOrDefault(previousKey) == craft) _craftsByNameAndCategory.Remove(previousKey);
		}
		if (removeExistingChildren)
		{
			_context!.CraftProducts.RemoveRange(craft.CraftProducts);
			_context.CraftTools.RemoveRange(craft.CraftTools);
			_context.CraftInputs.RemoveRange(craft.CraftInputs);
			_context.CraftPhases.RemoveRange(craft.CraftPhases);
			craft.CraftProducts.Clear();
			craft.CraftTools.Clear();
			craft.CraftInputs.Clear();
			craft.CraftPhases.Clear();
		}

		craft.Name = spec.Name;
		craft.Category = spec.Category;
		_craftsByNameAndCategory[CraftLookupKey(craft.Name, craft.Category)] = craft;
		craft.Blurb = spec.Blurb;
		craft.ActionDescription = spec.Action;
		craft.ActiveCraftItemSdesc = spec.ActiveCraftItemSdesc;
		craft.AppearInCraftsListProgId = spec.AppearProg!.Id;
		craft.CanUseProgId = spec.CanUseProg?.Id;
		craft.WhyCannotUseProgId = spec.WhyCannotUseProg?.Id;
		craft.OnUseProgStartId = spec.OnStartProg?.Id;
		craft.OnUseProgCompleteId = spec.OnFinishProg?.Id;
		craft.OnUseProgCancelId = spec.OnCancelProg?.Id;
		craft.CheckTrait = spec.Trait!;
		craft.CheckTraitId = spec.Trait!.Id;
		craft.CheckDifficulty = (int)spec.Difficulty;
		craft.FailThreshold = (int)spec.Threshold;
		craft.FreeSkillChecks = spec.FreeChecks;
		craft.FailPhase = spec.FailPhase;
		craft.Interruptable = spec.Interruptable;
		craft.QualityFormula = "5 + (outcome/3) + (variable/20)";
		craft.CheckQualityWeighting = 1.0;
		craft.InputQualityWeighting = 1.0;
		craft.ToolQualityWeighting = 1.0;
		craft.IsPracticalCheck = true;

		var phaseNumber = 1;
		foreach (var phase in spec.Phases)
		{
			craft.CraftPhases.Add(new CraftPhase
			{
				Craft = craft,
				Echo = phase.Echo,
				FailEcho = string.IsNullOrWhiteSpace(phase.FailEcho) ? phase.Echo : phase.FailEcho,
				PhaseLengthInSeconds = phase.Seconds,
				PhaseNumber = phaseNumber++,
				ExertionLevel = (int)phase.Exertion,
				StaminaUsage = phase.Stamina
			});
		}

		foreach (var input in spec.Inputs)
		{
			craft.CraftInputs.Add(ConvertToInput(craft, input));
		}

		foreach (var tool in spec.Tools)
		{
			craft.CraftTools.Add(ConvertToTool(craft, tool));
		}

		if (!_deferCraftProductSave)
		{
			SaveManifestChanges();
		}
		foreach (var product in spec.Products.Concat(spec.FailProducts))
		{
			craft.CraftProducts.Add(ConvertToProduct(craft, product));
		}

		if (!_deferCraftProductSave)
		{
			SaveManifestChanges();
		}
	}

	private void BeginDeferredCraftPersistence()
	{
		_nextCraftInputId = _context!.CraftInputs
			.OrderByDescending(x => x.Id)
			.Select(x => x.Id)
			.FirstOrDefault() + 1;
		_deferredCraftPersistenceCount = 0;
	}

	private void QueueDeferredCraftPersistence()
	{
		if (!_deferCraftProductSave)
		{
			return;
		}

		_deferredCraftPersistenceCount++;
		if (_deferredCraftPersistenceCount >= 128)
		{
			FlushDeferredCraftPersistence();
		}
	}

	private void FlushDeferredCraftPersistence()
	{
		if (!_deferCraftProductSave || _deferredCraftPersistenceCount == 0)
		{
			return;
		}

		SaveCraftChanges();
		DetachTrackedEntities(entity => entity is Craft or
			CraftPhase or
			CraftInput or
			CraftTool or
			CraftProduct or
			SeederManagedRecord or
			EditableItem);
		_deferredCraftPersistenceCount = 0;
	}

	private void SaveCraftChanges()
	{
		SaveManifestChanges();
	}

    private static int? GetMaterialDefiningInputIndex(List<(int Product, int Input)>? indexes, int productNumber)
    {
        (int Product, int Input) match = indexes?.FirstOrDefault(x => x.Product == productNumber) ?? default;
        return match.Product == 0 ? null : Math.Max(0, match.Input - 1);
    }

	private static string CraftLookupKey(string name, string category)
	{
		return $"{name.Trim()}\u001F{category.Trim()}";
	}

	private Craft? FindExistingCraft(string name, string category)
	{
		return _craftsByNameAndCategory.TryGetValue(CraftLookupKey(name, category), out Craft? craft)
			? craft
			: null;
	}

	private IReadOnlyList<CraftValidationError> ValidateCraftSpec(CraftDefinitionSpec spec)
	{
		List<CraftValidationError> errors = [];
		if (string.IsNullOrWhiteSpace(spec.Name))
		{
			errors.Add(new CraftValidationError(spec.Name, "craft", spec.Name, "Craft name cannot be blank"));
		}

		if (string.IsNullOrWhiteSpace(spec.Category))
		{
			errors.Add(new CraftValidationError(spec.Name, "craft", spec.Category, "Craft category cannot be blank"));
		}

		if (spec.AppearProg is null)
		{
			errors.Add(new CraftValidationError(spec.Name, "craft", "appear prog", "Craft must have an appear prog"));
		}

		if (spec.Trait is null)
		{
			errors.Add(new CraftValidationError(spec.Name, "craft", "trait", "Craft must have a check trait"));
		}

		if (spec.Phases.Count == 0)
		{
			errors.Add(new CraftValidationError(spec.Name, "phase", "phases", "Craft must have at least one phase"));
		}

		if (spec.FailPhase < 1 || spec.FailPhase > Math.Max(1, spec.Phases.Count))
		{
			errors.Add(new CraftValidationError(spec.Name, "phase", spec.FailPhase.ToString(System.Globalization.CultureInfo.InvariantCulture), "Fail phase must be a one-based phase number"));
		}

		for (int i = 0; i < spec.Phases.Count; i++)
		{
			CraftPhaseSpec phase = spec.Phases[i];
			if (phase.Seconds <= 0)
			{
				errors.Add(new CraftValidationError(spec.Name, "phase", (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), "Phase seconds must be positive"));
			}

			if (phase.Stamina < 0.0)
			{
				errors.Add(new CraftValidationError(spec.Name, "phase", (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), "Phase stamina cannot be negative"));
			}
		}

		Craft validationCraft = new();
		for (int i = 0; i < spec.Inputs.Count; i++)
		{
			CraftInputSpec input = spec.Inputs[i];
			try
			{
				BuildInputDefinition(input);
			}
			catch (ApplicationException ex)
			{
				errors.Add(new CraftValidationError(spec.Name, "input", input.ImportText, ex.Message));
			}

			validationCraft.CraftInputs.Add(new CraftInput
			{
				Id = i + 1,
				InputType = input.InputType,
				Definition = "<Definition />",
				OriginalAdditionTime = _now.AddTicks(i + 1)
			});
		}

		foreach (CraftToolSpec tool in spec.Tools)
		{
			try
			{
				BuildToolDefinition(tool);
			}
			catch (ApplicationException ex)
			{
				errors.Add(new CraftValidationError(spec.Name, "tool", tool.ImportText, ex.Message));
			}
		}

		ValidateProductSpecs(spec, validationCraft, spec.Products, "product", errors);
		ValidateProductSpecs(spec, validationCraft, spec.FailProducts, "fail product", errors);
		return errors;
	}

	private void ValidateProductSpecs(CraftDefinitionSpec spec, Craft validationCraft, IEnumerable<CraftProductSpec> products,
		string kind, List<CraftValidationError> errors)
	{
		foreach (CraftProductSpec product in products)
		{
			if (product.MaterialDefiningInputIndex is < 0 || product.MaterialDefiningInputIndex >= spec.Inputs.Count)
			{
				errors.Add(new CraftValidationError(spec.Name, kind, product.ImportText, "Material-defining input index is outside the craft input list"));
			}

			try
			{
				BuildProductDefinition(validationCraft, product);
			}
			catch (ApplicationException ex)
			{
				errors.Add(new CraftValidationError(spec.Name, kind, product.ImportText, ex.Message));
			}
		}
	}

	private void IndexStockSkillPackageTraitAliases()
	{
		foreach (IReadOnlyList<string> aliases in StockSkillPackageTraitAliases)
		{
			TraitDefinition? trait = aliases
				.Select(LookupLoadedTraitDefinition)
				.FirstOrDefault(x => x is not null);
			if (trait is null)
			{
				continue;
			}

			foreach (string alias in aliases.Where(alias => !_traits.ContainsKey(alias)))
			{
				_traits[alias] = trait;
			}
		}
	}

	private TraitDefinition? LookupLoadedTraitDefinition(string traitName)
	{
		if (_traits.TryGetValue(traitName, out TraitDefinition? trait) && trait is not null)
		{
			return trait;
		}

		string normalisedName = NormaliseTraitLookupName(traitName);
		return _traits.Values.FirstOrDefault(x =>
			NormaliseTraitLookupName(x.Name).Equals(normalisedName, StringComparison.Ordinal));
	}

	private static string NormaliseTraitLookupName(string traitName)
	{
		return new string(traitName.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
	}

	private IEnumerable<string> TraitDefinitionLookupNames(string traitName)
	{
		string trimmedName = traitName.Trim();
		HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
		if (names.Add(trimmedName))
		{
			yield return trimmedName;
		}

		string normalisedName = NormaliseTraitLookupName(trimmedName);
		foreach (IReadOnlyList<string> aliases in StockSkillPackageTraitAliases.Where(x =>
			         x.Any(alias => NormaliseTraitLookupName(alias).Equals(normalisedName, StringComparison.Ordinal))))
		{
			foreach (string alias in aliases)
			{
				if (names.Add(alias))
				{
					yield return alias;
				}
			}
		}
	}

	private TraitDefinition LookupTraitDefinition(string traitName)
	{
		List<string> attemptedNames = TraitDefinitionLookupNames(traitName).ToList();
		foreach (string name in attemptedNames)
		{
			TraitDefinition? trait = LookupLoadedTraitDefinition(name);
			if (trait is not null)
			{
				return trait;
			}
		}
		if (_manifestCaptureOnly)
		{
			var capturedTrait = new TraitDefinition
			{
				Id = -(_traits.Count + 1L),
				Name = traitName.Trim(),
				Alias = traitName.Trim(),
				TraitGroup = "Captured ItemSeeder Dependency",
				ChargenBlurb = string.Empty,
				ValueExpression = "0"
			};
			_traits[traitName] = capturedTrait;
			return capturedTrait;
		}

		throw new ApplicationException(
			$"Unknown trait {traitName}. Tried: {string.Join(", ", attemptedNames)}");
	}

	private static string EscapeFutureProgText(string text)
	{
		return text.Replace(@"\", @"\\").Replace("\"", "\\\"");
	}

	private static string FutureProgReferenceComment(string type, long id, string name)
	{
		return $"// {type} {id.ToString(System.Globalization.CultureInfo.InvariantCulture)} = \"{EscapeFutureProgText(name)}\"";
	}

	private static string AbbreviateFutureProgNamePart(string text)
	{
		var abbreviation = new StringBuilder();
		foreach (Match match in Regex.Matches(text, @"[A-Z]+(?=[A-Z][a-z]|\b)|[A-Z]?[a-z]+|\d+"))
		{
			var part = match.Value;
			abbreviation.Append(part[..Math.Min(3, part.Length)]);
		}

		return abbreviation.Length > 0 ? abbreviation.ToString() : "Ref";
	}

	private static string TraitGateProgName(string purpose, TraitDefinition trait, int? minimumTraitValue)
	{
		var minimum = minimumTraitValue?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
		return $"Cr{purpose}T{AbbreviateFutureProgNamePart(trait.Name)}{trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)}{minimum}";
	}

	private static string KnowledgeGateProgName(string purpose, MudSharp.Models.Knowledge knowledge, TraitDefinition trait,
		int? minimumTraitValue)
	{
		var knowledgePart = $"K{AbbreviateFutureProgNamePart(knowledge.Name)}{knowledge.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
		if (minimumTraitValue is null)
		{
			return $"Cr{purpose}{knowledgePart}";
		}

		return $"Cr{purpose}{knowledgePart}T{AbbreviateFutureProgNamePart(trait.Name)}{trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)}{minimumTraitValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
	}

	private void SaveFutureProgsIfRequired(params FutureProg[] progs)
	{
		if (!_manifestCaptureOnly &&
		    progs.Any(prog => _context!.Entry(prog).State is EntityState.Added or EntityState.Modified))
		{
			SaveCraftChanges();
		}
	}

	private FutureProg EnsureAlwaysTrueProg()
	{
		// AlwaysTrue is core infrastructure, not ItemSeeder-owned content. Reuse it
		// verbatim so its independently maintained signature is never claimed or
		// treated as an unsafe ItemSeeder conflict.
		if (_progs.TryGetValue("AlwaysTrue", out var existingProg))
		{
			return existingProg;
		}

		return EnsureFutureProg("AlwaysTrue", "Utility", "General", ProgVariableTypes.Boolean, "",
			[], "return true");
	}

	private MudSharp.Models.Knowledge EnsureCraftKnowledge(string knowledgeName, string knowledgeType,
		string knowledgeSubtype, string? knowledgeDescription, string? knowledgeLongDescription,
		int learningSessionsRequired, Difficulty learnDifficulty, Difficulty teachDifficulty, LearnableType learnable)
	{
		string trimmedName = knowledgeName.Trim();
		if (string.IsNullOrWhiteSpace(trimmedName))
		{
			throw new ApplicationException("Knowledge name cannot be blank");
		}
		if (IsManifestAggregateRegistered("knowledge", trimmedName))
		{
			return _context!.Knowledges.Local.FirstOrDefault(x =>
				       x.Name.Equals(trimmedName, StringComparison.OrdinalIgnoreCase)) ??
			       _context.Knowledges.AsEnumerable().First(x =>
				       x.Name.Equals(trimmedName, StringComparison.OrdinalIgnoreCase));
		}

		FutureProg alwaysTrueProg = EnsureAlwaysTrueProg();
		string description = string.IsNullOrWhiteSpace(knowledgeDescription)
			? $"Knowledge of {trimmedName}"
			: knowledgeDescription.Trim();
		string longDescription = string.IsNullOrWhiteSpace(knowledgeLongDescription)
			? $"Specialised knowledge required to perform crafts gated by {trimmedName}."
			: knowledgeLongDescription.Trim();

		MudSharp.Models.Knowledge knowledge = SeederRepeatabilityHelper.EnsureNamedEntity(
			_context!.Knowledges,
			trimmedName,
			x => x.Name,
			() =>
			{
				MudSharp.Models.Knowledge created = new();
				_context.Knowledges.Add(created);
				return created;
			});
		var manifestDefinition = new KnowledgeManifestDefinition(
			trimmedName,
			string.IsNullOrWhiteSpace(knowledgeType) ? "Crafting" : knowledgeType.Trim(),
			string.IsNullOrWhiteSpace(knowledgeSubtype) ? "General" : knowledgeSubtype.Trim(),
			description,
			longDescription,
			(int)learnable,
			(int)learnDifficulty,
			(int)teachDifficulty,
			Math.Max(1, learningSessionsRequired),
			alwaysTrueProg.FunctionName,
			alwaysTrueProg.FunctionName);
		var manifestEntry = RegisterManifestAggregate("knowledge", trimmedName, manifestDefinition);
		var isNew = _context.Entry(knowledge).State == EntityState.Added;
		if (!isNew)
		{
			var liveDefinition = new KnowledgeManifestDefinition(
				knowledge.Name,
				knowledge.Type,
				knowledge.Subtype,
				knowledge.Description,
				knowledge.LongDescription,
				knowledge.LearnableType,
				knowledge.LearnDifficulty,
				knowledge.TeachDifficulty,
				knowledge.LearningSessionsRequired,
				knowledge.CanAcquireProg?.FunctionName ??
				_context.FutureProgs.Where(x => x.Id == knowledge.CanAcquireProgId).Select(x => x.FunctionName).FirstOrDefault() ?? string.Empty,
				knowledge.CanLearnProg?.FunctionName ??
				_context.FutureProgs.Where(x => x.Id == knowledge.CanLearnProgId).Select(x => x.FunctionName).FirstOrDefault() ?? string.Empty);
			var disposition = InspectManifestAggregate(manifestEntry, knowledge.Id, liveDefinition);
			if (disposition is ManifestAggregateDisposition.Customized or ManifestAggregateDisposition.Unchanged)
			{
				return knowledge;
			}
		}

		knowledge.Name = trimmedName;
		knowledge.Type = string.IsNullOrWhiteSpace(knowledgeType) ? "Crafting" : knowledgeType.Trim();
		knowledge.Subtype = string.IsNullOrWhiteSpace(knowledgeSubtype) ? "General" : knowledgeSubtype.Trim();
		knowledge.Description = description;
		knowledge.LongDescription = longDescription;
		knowledge.LearnableType = (int)learnable;
		knowledge.LearnDifficulty = (int)learnDifficulty;
		knowledge.TeachDifficulty = (int)teachDifficulty;
		knowledge.LearningSessionsRequired = Math.Max(1, learningSessionsRequired);
		knowledge.CanAcquireProg = alwaysTrueProg;
		knowledge.CanLearnProg = alwaysTrueProg;
		if (isNew)
		{
			CompleteGeneratedManifestAggregate(manifestEntry, knowledge, manifestDefinition);
			if (!_manifestCaptureOnly)
			{
				SaveCraftChanges();
			}
		}
		else
		{
			CompleteManifestAggregate(manifestEntry, knowledge.Id, manifestDefinition, ManifestAggregateDisposition.Update);
		}
		return knowledge;
	}

	private (FutureProg AppearProg, FutureProg CanUseProg, FutureProg WhyCannotUseProg, TraitDefinition Trait) EnsureTraitGateProgs(string traitName, int? minimumTraitValue)
	{
		TraitDefinition trait = LookupTraitDefinition(traitName);
		var definitions = TraitGateDefinitions(trait, minimumTraitValue);
		if (!_manifestCaptureOnly)
		{
			// Load the existing parameter graph once per dependency context, not once per recipe.
			_traitGateProgRows ??= _context!.FutureProgs.Include(x => x.FutureProgsParameters).ToArray();
			var rows = _traitGateProgRows.Concat(_context!.FutureProgs.Local).Distinct().ToArray();
			// Validate every gate before creating even the first prog; preserve renamed owned targets.
			var targets = definitions.Select(x => ResolveTraitGateProgOwnership(x, rows)).ToArray();
			for (var i = 0; i < definitions.Length; i++)
				if (targets[i] is { } target) _progs[definitions[i].Name] = target;
		}
		var gates = definitions.Select(definition => EnsureFutureProg(definition.Name, definition.Category,
			definition.Subcategory, (ProgVariableTypes)definition.ReturnType, definition.Comment,
			definition.Parameters.Select(x => ((ProgVariableTypes)x.Type, x.Name)), definition.Text)).ToArray();
		SaveFutureProgsIfRequired(gates);
		if (CatalogueCraftAuthoringTrace is not null)
			_capturedCraftGates[gates[0].FunctionName] = new { Kind = "Trait", Trait = traitName, MinimumTraitValue = minimumTraitValue };
		return (gates[0], gates[1], gates[2], trait);
	}

	private static ProgManifestDefinition[] TraitGateDefinitions(TraitDefinition trait, int? minimumTraitValue)
	{
		string traitComment = FutureProgReferenceComment("Trait", trait.Id, trait.Name);
		string booleanText = minimumTraitValue is null
			? $@"{traitComment}
return @ch.Skills.Any(x, @x.Id == {trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)})"
			: $@"{traitComment}
return GetTrait(@ch, ToTrait(""{trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)}"")) >= {minimumTraitValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
		string traitText = EscapeFutureProgText(trait.Name);
		string whyText = minimumTraitValue is null
			? $@"return ""You do not have the {traitText} skill."""
			: $@"return ""You need at least {minimumTraitValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)} in {traitText} to do that craft.""";

		return
		[
			new(TraitGateProgName("Ap", trait, minimumTraitValue), "Crafting", "Access", (long)ProgVariableTypes.Boolean, "",
				booleanText, [new((long)ProgVariableTypes.Character, "ch")]),
			new(TraitGateProgName("Use", trait, minimumTraitValue), "Crafting", "Access", (long)ProgVariableTypes.Boolean, "",
				booleanText, [new((long)ProgVariableTypes.Character, "ch")]),
			new(TraitGateProgName("Why", trait, minimumTraitValue), "Crafting", "Access", (long)ProgVariableTypes.Text, "",
				whyText, [new((long)ProgVariableTypes.Character, "ch")])
		];
	}

	private (FutureProg AppearProg, FutureProg CanUseProg, FutureProg WhyCannotUseProg, TraitDefinition Trait,
		MudSharp.Models.Knowledge Knowledge) EnsureKnowledgeGateProgs(string knowledgeName, string traitName,
		int? minimumTraitValue, string knowledgeType, string knowledgeSubtype, string? knowledgeDescription,
		string? knowledgeLongDescription, int knowledgeLearningSessionsRequired, Difficulty knowledgeLearnDifficulty,
		Difficulty knowledgeTeachDifficulty, LearnableType knowledgeLearnable)
	{
		TraitDefinition trait = LookupTraitDefinition(traitName);
		MudSharp.Models.Knowledge knowledge = EnsureCraftKnowledge(
			knowledgeName,
			knowledgeType,
			knowledgeSubtype,
			knowledgeDescription,
			knowledgeLongDescription,
			knowledgeLearningSessionsRequired,
			knowledgeLearnDifficulty,
			knowledgeTeachDifficulty,
			knowledgeLearnable);

		string knowledgeId = knowledge.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
		string traitId = trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
		string knowledgeComment = FutureProgReferenceComment("Knowledge", knowledge.Id, knowledge.Name);
		string traitComment = FutureProgReferenceComment("Trait", trait.Id, trait.Name);
		string knowledgeCheck = $@"@ch.Knowledges.Any(x, @x.Id == {knowledgeId})";
		string booleanText = minimumTraitValue is null
			? $@"{knowledgeComment}
return {knowledgeCheck}"
			: $@"{knowledgeComment}
if (not({knowledgeCheck}))
	return false
end if
{traitComment}
return GetTrait(@ch, ToTrait(""{traitId}"")) >= {minimumTraitValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
		string knowledgeText = EscapeFutureProgText(knowledge.Name);
		string traitText = EscapeFutureProgText(trait.Name);
		string whyText = minimumTraitValue is null
			? $@"return ""You do not know the {knowledgeText} knowledge."""
			: $@"{knowledgeComment}
if (not({knowledgeCheck}))
	return ""You do not know the {knowledgeText} knowledge.""
end if
return ""You need at least {minimumTraitValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)} in {traitText} to do that craft.""";

		FutureProg appearProg = EnsureFutureProg(KnowledgeGateProgName("Ap", knowledge, trait, minimumTraitValue), "Crafting", "Access", ProgVariableTypes.Boolean, "",
			[(ProgVariableTypes.Character, "ch")], booleanText);
		FutureProg canUseProg = EnsureFutureProg(KnowledgeGateProgName("Use", knowledge, trait, minimumTraitValue), "Crafting", "Access", ProgVariableTypes.Boolean, "",
			[(ProgVariableTypes.Character, "ch")], booleanText);
		FutureProg whyCannotUseProg = EnsureFutureProg(KnowledgeGateProgName("Why", knowledge, trait, minimumTraitValue), "Crafting", "Access", ProgVariableTypes.Text, "",
			[(ProgVariableTypes.Character, "ch")], whyText);
		SaveFutureProgsIfRequired(appearProg, canUseProg, whyCannotUseProg);
		if (CatalogueCraftAuthoringTrace is not null)
			_capturedCraftGates[appearProg.FunctionName] = new
			{
				Kind = "Knowledge", Trait = traitName, MinimumTraitValue = minimumTraitValue,
				Knowledge = knowledgeName, KnowledgeType = knowledgeType, KnowledgeSubtype = knowledgeSubtype,
				KnowledgeDescription = knowledgeDescription, KnowledgeLongDescription = knowledgeLongDescription,
				KnowledgeLearningSessionsRequired = knowledgeLearningSessionsRequired,
				KnowledgeLearnDifficulty = knowledgeLearnDifficulty, KnowledgeTeachDifficulty = knowledgeTeachDifficulty,
				KnowledgeLearnable = knowledgeLearnable
			};
		return (appearProg, canUseProg, whyCannotUseProg, trait, knowledge);
	}

	private void InitialiseCraftAuthoringForTesting(FuturemudDatabaseContext context)
	{
		_context = context;
		_questionAnswers = new Dictionary<string, string>();
		_missingTags.Clear();
		_progs = new Dictionary<string, FutureProg>(StringComparer.InvariantCultureIgnoreCase);
		_traits = new DictionaryWithDefault<string, TraitDefinition>(StringComparer.OrdinalIgnoreCase);
		InitialiseDependencies();
		foreach (FutureProg prog in context.FutureProgs)
		{
			_progs[prog.FunctionName] = prog;
		}
		_nextCraftId = context.Crafts.Select(x => x.Id).ToList().DefaultIfEmpty(0).Max(x => x) + 1;
	}

	internal MudSharp.Models.Craft AddCraftForTesting(FuturemudDatabaseContext context, CraftDefinitionSpec spec)
	{
		InitialiseCraftAuthoringForTesting(context);
		return AddCraft(spec);
	}

	internal MudSharp.Models.Craft AddCraftWithDeferredPersistenceForTesting(
		FuturemudDatabaseContext context,
		CraftDefinitionSpec spec)
	{
		InitialiseCraftAuthoringForTesting(context);
		var previousDeferCraftProductSave = _deferCraftProductSave;
		var previousAutoDetectChanges = _context!.ChangeTracker.AutoDetectChangesEnabled;
		_deferCraftProductSave = true;
		_context.ChangeTracker.AutoDetectChangesEnabled = false;
		BeginDeferredCraftPersistence();
		try
		{
			var craft = AddCraft(spec);
			FlushDeferredCraftPersistence();
			return context.Crafts
				.Include(x => x.CraftPhases)
				.Include(x => x.CraftInputs)
				.Include(x => x.CraftTools)
				.Include(x => x.CraftProducts)
				.Single(x => x.Id == craft.Id && x.RevisionNumber == craft.RevisionNumber);
		}
		finally
		{
			_deferCraftProductSave = previousDeferCraftProductSave;
			_context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetectChanges;
			_deferredCraftPersistenceCount = 0;
		}
	}

	internal MudSharp.Models.Craft AddCraftFromImportsForTesting(FuturemudDatabaseContext context, string name, string category,
		IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs, IEnumerable<string> tools,
		IEnumerable<string> products, IEnumerable<string> failProducts, List<(int Product, int Input)>? productMaterialInputIndexes = null,
		List<(int Product, int Input)>? failProductMaterialInputIndexes = null, string traitName = "Crafting",
		string appearProg = "AlwaysTrue", string? canUseProg = null, string? whyCantProg = null, string? onFinishProg = null,
		string? onStartProg = null, string? onCancelProg = null, Difficulty difficulty = Difficulty.Normal,
		Outcome threshold = Outcome.MinorFail, int freeChecks = 5, int failPhase = 1, bool interruptable = false,
		string blurb = "test craft", string action = "testing", string itemSdesc = "a test craft event")
	{
		InitialiseCraftAuthoringForTesting(context);
		return AddCraft(name, category, blurb, action, itemSdesc, appearProg, canUseProg, whyCantProg, onFinishProg,
			       LookupTraitDefinition(traitName), difficulty, threshold, freeChecks, failPhase, interruptable, phases,
			       inputs, tools, products, failProducts, productMaterialInputIndexes, failProductMaterialInputIndexes,
			       onStartProg, onCancelProg) ??
		       throw new ApplicationException($"Craft '{name}' was not created.");
	}

	internal MudSharp.Models.Craft AddTraitGatedCraftFromImportsForTesting(FuturemudDatabaseContext context, string name,
		string category, string traitName, int? minimumTraitValue, IEnumerable<(int Seconds, string Echo, string FailEcho)> phases,
		IEnumerable<string> inputs, IEnumerable<string> tools, IEnumerable<string> products, IEnumerable<string> failProducts)
	{
		InitialiseCraftAuthoringForTesting(context);
		return AddCraft(name, category, "test craft", "testing", "a test craft event", traitName, minimumTraitValue,
			       Difficulty.Normal, Outcome.MinorFail, 5, 1, false, phases, inputs, tools, products, failProducts) ??
		    throw new ApplicationException($"Craft '{name}' was not created.");
	}

	internal MudSharp.Models.Craft AddKnowledgeGatedCraftFromImportsForTesting(FuturemudDatabaseContext context,
		string name, string category, string knowledgeName, string traitName, int? minimumTraitValue,
		IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
		IEnumerable<string> tools, IEnumerable<string> products, IEnumerable<string> failProducts,
		string knowledgeType = "Crafting", string knowledgeSubtype = "General", string? knowledgeDescription = null,
		string? knowledgeLongDescription = null)
	{
		InitialiseCraftAuthoringForTesting(context);
		return AddCraft(name, category, "test craft", "testing", "a test craft event", knowledgeName, traitName,
			       minimumTraitValue, Difficulty.Normal, Outcome.MinorFail, 5, 1, false, phases, inputs, tools, products,
			       failProducts, knowledgeType: knowledgeType, knowledgeSubtype: knowledgeSubtype,
			       knowledgeDescription: knowledgeDescription, knowledgeLongDescription: knowledgeLongDescription) ??
		       throw new ApplicationException($"Craft '{name}' was not created.");
	}

	private void SeedCrafts()
    {
		if (_questionAnswers?.TryGetValue("eras", out var eras) != true || string.IsNullOrWhiteSpace(eras))
		{
			return;
		}

        // Reset nextID
		_nextCraftId = _context!.Crafts.Select(x => x.Id).ToList().DefaultIfEmpty(0).Max(x => x) + 1;

		var previousDeferCraftProductSave = _deferCraftProductSave;
		var previousAutoDetectChanges = _context!.ChangeTracker.AutoDetectChangesEnabled;
		_deferCraftProductSave = true;
		_context.ChangeTracker.AutoDetectChangesEnabled = false;
		BeginDeferredCraftPersistence();
		try
		{
			RunSeedStage("Creating compiled catalogue crafts", () => SeedCompiledCatalogueCrafts(eras));
			FlushDeferredCraftPersistence();
		}
		finally
		{
			_deferCraftProductSave = previousDeferCraftProductSave;
			_context!.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetectChanges;
			_deferredCraftPersistenceCount = 0;
		}
	}

	private void SeedBlackPowderCraftsOnly()
	{
		if (_questionAnswers?.TryGetValue("eras", out var eras) != true || string.IsNullOrWhiteSpace(eras) ||
		    !HasAnyEra(eras, "renaissance", "earlymodern"))
		{
			return;
		}

		_nextCraftId = _context!.Crafts.Select(x => x.Id).ToList().DefaultIfEmpty(0).Max() + 1;
		var previousDeferCraftProductSave = _deferCraftProductSave;
		var previousAutoDetectChanges = _context.ChangeTracker.AutoDetectChangesEnabled;
		_deferCraftProductSave = true;
		_context.ChangeTracker.AutoDetectChangesEnabled = false;
		BeginDeferredCraftPersistence();
		try
		{
			RunSeedStage("Reconciling the physical black-powder craft", () =>
			{
				using var manifestModule = UseManifestModule("crafts", "renaissance", "earlymodern");
				InstallCatalogueRecipe(Recipes09Stock.PowdermakingMillAndCornBlackPowder());
			});
			FlushDeferredCraftPersistence();
		}
		finally
		{
			_deferCraftProductSave = previousDeferCraftProductSave;
			_context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetectChanges;
			_deferredCraftPersistenceCount = 0;
		}
	}
}
