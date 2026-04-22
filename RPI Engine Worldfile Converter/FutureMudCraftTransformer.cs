#nullable enable

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

public enum CraftConversionStatus
{
	Ready,
	Deferred,
}

public sealed record CraftConversionWarning(string Code, string Message);

public sealed record ConvertedCraftClanRequirement(
	string CanonicalAlias,
	string FullName,
	string? RankName,
	IReadOnlyList<string> LegacyAliases);

public sealed record ConvertedCraftConstraintSet(
	bool HiddenFromCraftList,
	IReadOnlyList<string> LegacyFlags,
	IReadOnlyList<string> TerrainNames,
	IReadOnlyList<string> Seasons,
	IReadOnlyList<string> WeatherStates,
	IReadOnlyList<string> OpeningTraitNames,
	IReadOnlyList<string> OpeningSourceNames,
	IReadOnlyList<string> RaceNames,
	IReadOnlyList<ConvertedCraftClanRequirement> ClanRequirements,
	int FollowersRequired,
	int IcDelay,
	int FailDelay);

public sealed record ConvertedCraftCheckDefinition(
	string TraitName,
	int PhaseNumber,
	RpiCraftCheckKind Kind,
	string SourceName,
	string RawExpression,
	string DifficultyName);

public sealed record ConvertedCraftPhaseDefinition
{
	public required int PhaseNumber { get; init; }
	public required int Seconds { get; init; }
	public required string Echo { get; init; }
	public required string FailEcho { get; init; }
	public required string ExertionLevelName { get; init; }
	public required double StaminaUsage { get; init; }
	public required RpiCraftEchoSet RawEchoes { get; init; }
	public IReadOnlyList<string> RawLines { get; init; } = Array.Empty<string>();
}

public sealed record GeneratedCraftTagDefinition(
	string TagName,
	IReadOnlyList<int> SourceVnums,
	bool Reusable,
	string Reason);

public sealed record CraftFutureProgPlan(
	string FunctionName,
	string Kind,
	string Description,
	string? PreviewText);

public sealed record ConvertedCraftInputDefinition
{
	public required int SourceSlot { get; init; }
	public required int InputIndex { get; init; }
	public required string InputType { get; init; }
	public required int Quantity { get; init; }
	public int? SourceVnum { get; init; }
	public string? SourceItemKey { get; init; }
	public string? TagName { get; init; }
	public bool UsesGeneratedTag { get; init; }
	public IReadOnlyList<int> SourceVnums { get; init; } = Array.Empty<int>();
	public IReadOnlyList<CraftConversionWarning> Warnings { get; init; } = Array.Empty<CraftConversionWarning>();
}

public sealed record ConvertedCraftToolDefinition
{
	public required int SourceSlot { get; init; }
	public required string ToolType { get; init; }
	public required string DesiredStateName { get; init; }
	public int? SourceVnum { get; init; }
	public string? SourceItemKey { get; init; }
	public string? TagName { get; init; }
	public bool UsesGeneratedTag { get; init; }
	public bool UseToolDuration { get; init; }
	public IReadOnlyList<int> SourceVnums { get; init; } = Array.Empty<int>();
	public IReadOnlyList<CraftConversionWarning> Warnings { get; init; } = Array.Empty<CraftConversionWarning>();
}

public sealed record ConvertedCraftProgCase(int InputSourceVnum, int OutputSourceVnum);

public sealed record ConvertedCraftProductDefinition
{
	public required int SourceSlot { get; init; }
	public required string ProductType { get; init; }
	public required bool IsFailProduct { get; init; }
	public required int Quantity { get; init; }
	public int? OutputVnum { get; init; }
	public string? OutputSourceItemKey { get; init; }
	public bool LegacyGiveOutput { get; init; }
	public int? SourceInputSlot { get; init; }
	public int? SourceInputIndex { get; init; }
	public double? PercentageRecovered { get; init; }
	public string? GeneratedProgName { get; init; }
	public IReadOnlyList<ConvertedCraftProgCase> ProgCases { get; init; } = Array.Empty<ConvertedCraftProgCase>();
	public IReadOnlyList<int> SourceVnums { get; init; } = Array.Empty<int>();
	public IReadOnlyList<CraftConversionWarning> Warnings { get; init; } = Array.Empty<CraftConversionWarning>();
}

public sealed record ConvertedCraftDefinition
{
	public required string SourceFile { get; init; }
	public required int SourceLineNumber { get; init; }
	public required int CraftNumber { get; init; }
	public required string SourceKey { get; init; }
	public required string CraftName { get; init; }
	public required string SubcraftName { get; init; }
	public required string Command { get; init; }
	public required string Name { get; init; }
	public required string Category { get; init; }
	public required string Blurb { get; init; }
	public required string ActionDescription { get; init; }
	public required string ActiveCraftItemSdesc { get; init; }
	public required CraftConversionStatus Status { get; init; }
	public required ConvertedCraftConstraintSet Constraints { get; init; }
	public ConvertedCraftCheckDefinition? PrimaryCheck { get; init; }
	public IReadOnlyList<ConvertedCraftCheckDefinition> AdditionalChecks { get; init; } = Array.Empty<ConvertedCraftCheckDefinition>();
	public int FreeSkillChecks { get; init; }
	public string CheckDifficultyName { get; init; } = "Automatic";
	public string FailThresholdName { get; init; } = "MinorFail";
	public int FailPhase { get; init; }
	public IReadOnlyList<ConvertedCraftPhaseDefinition> Phases { get; init; } = Array.Empty<ConvertedCraftPhaseDefinition>();
	public IReadOnlyList<ConvertedCraftInputDefinition> Inputs { get; init; } = Array.Empty<ConvertedCraftInputDefinition>();
	public IReadOnlyList<ConvertedCraftToolDefinition> Tools { get; init; } = Array.Empty<ConvertedCraftToolDefinition>();
	public IReadOnlyList<ConvertedCraftProductDefinition> Products { get; init; } = Array.Empty<ConvertedCraftProductDefinition>();
	public IReadOnlyList<GeneratedCraftTagDefinition> GeneratedTags { get; init; } = Array.Empty<GeneratedCraftTagDefinition>();
	public IReadOnlyList<CraftFutureProgPlan> FutureProgPlans { get; init; } = Array.Empty<CraftFutureProgPlan>();
	public IReadOnlyList<string> RawSourceLines { get; init; } = Array.Empty<string>();
	public IReadOnlyList<RpiCraftParseWarning> ParseWarnings { get; init; } = Array.Empty<RpiCraftParseWarning>();
	public IReadOnlyList<CraftConversionWarning> Warnings { get; init; } = Array.Empty<CraftConversionWarning>();
}

public sealed record CraftConversionResult(
	IReadOnlyList<ConvertedCraftDefinition> Crafts,
	IReadOnlyList<GeneratedCraftTagDefinition> GeneratedTags,
	IReadOnlyDictionary<string, int> DeferredReasonCounts);

internal sealed record NormalisedClanRule(string CanonicalAlias, string? FullNameOverride, IReadOnlyList<string> LegacyAliases);

internal sealed record ResolvedCraftRequirement(
	string Type,
	int Quantity,
	int? SourceVnum,
	string? SourceItemKey,
	string? TagName,
	bool UsesGeneratedTag,
	IReadOnlyList<int> SourceVnums,
	IReadOnlyList<CraftConversionWarning> Warnings);

internal sealed record SourceProductCandidate(
	int PhaseNumber,
	int SourceSlot,
	int Quantity,
	bool IsFailProduct,
	bool LegacyGiveOutput,
	IReadOnlyList<int> SourceVnums,
	string RawDefinition);

public sealed class FutureMudCraftTransformer
{
	private static readonly HashSet<string> GenericCraftTags = new(StringComparer.OrdinalIgnoreCase)
	{
		"Food",
		"Lamps",
		"Luxury Wares",
		"Military Goods",
		"Open Container",
		"Porous Container",
		"Repairing",
		"Standard Tools",
		"Torches",
		"Watertight Container",
		"Bodywear",
		"Legwear",
		"Footwear",
		"Belts",
		"Gloves",
		"Hats",
		"Mail Armour",
		"Leather Armour",
		"Plate Armour",
		"Primitive Armour",
	};

	private static readonly Dictionary<string, string> FamilyTokens = new(StringComparer.OrdinalIgnoreCase)
	{
		["knife"] = "Knife",
		["knives"] = "Knife",
		["needle"] = "Sewing Needle",
		["scissors"] = "Scissors",
		["hammer"] = "Hammer",
		["anvil"] = "Anvil",
		["tongs"] = "Forge Tongs",
		["bellows"] = "Bellows",
		["thread"] = "Thread",
		["string"] = "String",
		["branch"] = "Branch",
		["pole"] = "Pole",
		["rivet"] = "Rivet",
		["tie"] = "Tie",
		["padding"] = "Padding",
		["stud"] = "Armouring Studs",
		["studs"] = "Armouring Studs",
		["blade"] = "Blade",
		["tusk"] = "Tusk",
		["fire"] = "Fire",
		["water"] = "Water",
	};

	private static readonly HashSet<string> IgnoredTokens = new(StringComparer.OrdinalIgnoreCase)
	{
		"a",
		"an",
		"and",
		"as",
		"at",
		"for",
		"from",
		"in",
		"into",
		"item",
		"long",
		"small",
		"short",
		"large",
		"the",
		"to",
		"with",
	};

	private static readonly IReadOnlyDictionary<string, string> SkillTraitAliases =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["brawling"] = "Brawl",
			["small blade"] = "Light-Edge",
			["small-blade"] = "Light-Edge",
			["sword"] = "Medium-Edge",
			["axe"] = "Medium-Edge",
			["polearm"] = "Polearm",
			["club"] = "Medium-Blunt",
			["flail"] = "Heavy-Blunt",
			["double handed"] = "Heavy-Edge",
			["double-handed"] = "Heavy-Edge",
			["sole wield"] = "Block",
			["sole-wield"] = "Block",
			["shield use"] = "Block",
			["shield-use"] = "Block",
			["dual wield"] = "Dual-Wielding",
			["dual-wield"] = "Dual-Wielding",
			["throwing"] = "Throw",
			["blowgun"] = "Throw",
			["sling"] = "Sling",
			["hunting bow"] = "Bows",
			["hunting-bow"] = "Bows",
			["warbow"] = "Bows",
			["avert"] = "Parry",
			["sneak"] = "Sneak",
			["hide"] = "Hide",
			["steal"] = "Steal",
			["picklock"] = "Pick Locks",
			["pick lock"] = "Pick Locks",
			["forage"] = "Forage",
			["barter"] = "Barter",
			["ride"] = "Ride",
			["butchery"] = "Skin",
			["poisoning"] = "Pharmacology",
			["herbalism"] = "Herbalist",
			["dodge"] = "Dodge",
			["metalcraft"] = "Blacksmith",
			["woodcraft"] = "Carpenter",
			["lumberjack"] = "Lumberjack",
			["cookery"] = "Cook",
			["hideworking"] = "Tanner",
			["brewing"] = "Brewer",
			["literacy"] = "Literacy",
			["apothecary"] = "Pharmacology",
			["mining"] = "Miner",
			["tracking"] = "Track",
			["healing"] = "First Aid",
			["astronomy"] = "Astronomy",
		};

	private static readonly IReadOnlyDictionary<string, string> AttributeTraitAliases =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["str"] = "Strength",
			["strength"] = "Strength",
			["dex"] = "Dexterity",
			["dexterity"] = "Dexterity",
			["agi"] = "Agility",
			["agility"] = "Agility",
			["con"] = "Constitution",
			["constitution"] = "Constitution",
			["int"] = "Intelligence",
			["intelligence"] = "Intelligence",
			["wil"] = "Willpower",
			["will"] = "Willpower",
			["willpower"] = "Willpower",
			["per"] = "Perception",
			["perception"] = "Perception",
			["aur"] = "Aura",
			["aura"] = "Aura",
		};

	private static readonly IReadOnlyDictionary<int, string> LegacyOpeningSkills =
		new Dictionary<int, string>
		{
			[1] = "Brawling",
			[2] = "Small-Blade",
			[3] = "Sword",
			[4] = "Axe",
			[5] = "Polearm",
			[6] = "Club",
			[7] = "Flail",
			[8] = "Double-Handed",
			[9] = "Sole-Wield",
			[10] = "Shield-Use",
			[11] = "Dual-Wield",
			[12] = "Throwing",
			[13] = "Blowgun",
			[14] = "Sling",
			[15] = "Hunting-bow",
			[16] = "Warbow",
			[19] = "Sneak",
			[20] = "Hide",
			[21] = "Steal",
			[22] = "Picklock",
			[23] = "Forage",
			[24] = "Barter",
			[25] = "Ride",
			[26] = "Butchery",
			[27] = "Poisoning",
			[28] = "Herbalism",
			[38] = "Dodge",
			[39] = "Metalcraft",
			[40] = "Woodcraft",
			[41] = "Lumberjack",
			[42] = "Cookery",
			[43] = "Hideworking",
			[44] = "Brewing",
			[45] = "Literacy",
			[46] = "Apothecary",
			[47] = "Mining",
			[48] = "Tracking",
			[49] = "Healing",
			[71] = "Astronomy",
		};

	private static readonly IReadOnlyDictionary<string, string> LegacyRaceNames =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["0"] = "Human",
			["1"] = "Human",
			["2"] = "Human",
			["3"] = "Orc",
			["4"] = "Troll",
			["5"] = "Elf",
			["6"] = "Elf",
			["7"] = "Troll",
			["8"] = "Elf",
			["9"] = "Wolf",
			["10"] = "Spider",
			["11"] = "Wraith",
			["12"] = "Horse",
			["13"] = "Horse",
			["14"] = "Bird",
			["15"] = "Wolfspawn",
			["16"] = "Ent",
			["17"] = "Hobbit",
			["18"] = "Dwarf",
			["19"] = "Mule",
			["20"] = "Donkey",
			["21"] = "Warg",
			["22"] = "Dog",
			["23"] = "Cat",
			["24"] = "Snake",
			["25"] = "Boar",
			["26"] = "Uruk",
			["27"] = "Human",
			["28"] = "Human",
		};

	private static readonly IReadOnlyDictionary<string, NormalisedClanRule> CanonicalClanRules =
		new Dictionary<string, NormalisedClanRule>(StringComparer.OrdinalIgnoreCase)
		{
			["mordor_char"] = new("mordor_char", "Minas Morgul", ["mm_denizens"]),
			["mm_denizens"] = new("mordor_char", "Minas Morgul", ["mm_denizens"]),
			["malred"] = new("malred", "Malred Family", ["housemalred"]),
			["housemalred"] = new("malred", "Malred Family", ["housemalred"]),
			["rogues"] = new("rogues", "Rogues' Fellowship", ["rouges"]),
			["rouges"] = new("rogues", "Rogues' Fellowship", ["rouges"]),
			["hawk_dove_2"] = new("hawk_dove_2", "Hawk and Dove", ["hawk_and_dove"]),
			["hawk_and_dove"] = new("hawk_dove_2", "Hawk and Dove", ["hawk_and_dove"]),
			["seekers"] = new("seekers", "Seekers", Array.Empty<string>()),
			["shadow-cult"] = new("shadow-cult", "Shadow Cult", Array.Empty<string>()),
			["tirithguard"] = new("tirithguard", "Minas Tirith Guard", Array.Empty<string>()),
			["eradan_battalion"] = new("eradan_battalion", "Eradan Battalion", Array.Empty<string>()),
		};

	private static readonly IReadOnlyDictionary<string, string[]> SeasonMonthAliases =
		new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["spring"] =
			[
				"tuile", "ethuil", "coire", "echuir",
				"sulime", "viresse", "gwaeron", "gwirith",
				"rethe", "astron", "afteryule", "solmath"
			],
			["summer"] =
			[
				"laire", "laer",
				"lotesse", "narie", "Loende", "cermie",
				"lothron", "norui", "enedhin", "cerveth", "urime", "urui",
				"thrimidge", "forelithe", "lithe", "afterlithe"
			],
			["autumn"] =
			[
				"yav", "iavas", "quelle",
				"yavannie", "narquelie",
				"ivanneth", "narbeleth",
				"wedmath", "halimath", "winterfilth"
			],
			["winter"] =
			[
				"yestare", "mettare", "firith", "hrive", "rhiw", "penninor",
				"narvinye", "nenime", "hisime", "ringare",
				"narwain", "ninui", "hithui", "Girithron",
				"firstyul", "blotmath", "foreyule", "lastyule"
			]
		};

	private readonly IReadOnlyDictionary<int, ConvertedItemDefinition> _itemsByVnum;
	private readonly Dictionary<string, GeneratedCraftTagDefinition> _generatedTags = new(StringComparer.OrdinalIgnoreCase);

	public FutureMudCraftTransformer(IEnumerable<ConvertedItemDefinition> convertedItems)
	{
		_itemsByVnum = convertedItems.ToDictionary(x => x.Vnum, x => x);
	}

	public CraftConversionResult Convert(IEnumerable<RpiCraftDefinition> crafts)
	{
		var converted = crafts
			.OrderBy(x => x.CraftNumber)
			.Select(ConvertCraft)
			.ToList();

		var deferredReasons = converted
			.Where(x => x.Status == CraftConversionStatus.Deferred)
			.SelectMany(x => x.Warnings.Where(y => y.Code.StartsWith("deferred", StringComparison.OrdinalIgnoreCase)))
			.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.Count(), StringComparer.OrdinalIgnoreCase);

		return new CraftConversionResult(
			converted,
			_generatedTags.Values.OrderBy(x => x.TagName, StringComparer.OrdinalIgnoreCase).ToList(),
			deferredReasons);
	}

	private ConvertedCraftDefinition ConvertCraft(RpiCraftDefinition craft)
	{
		List<CraftConversionWarning> warnings = [];
		var inputSlots = new SortedDictionary<int, RpiCraftItemList>();
		var toolSlots = new SortedDictionary<string, RpiCraftItemList>(StringComparer.OrdinalIgnoreCase);
		List<SourceProductCandidate> productCandidates = [];
		List<SourceProductCandidate> failProductCandidates = [];
		List<RpiCraftCheckRecord> allChecks = [];

		foreach (var phase in craft.Phases.OrderBy(x => x.PhaseNumber))
		{
			if (phase.SkillCheck is not null)
			{
				allChecks.Add(phase.SkillCheck);
			}

			if (phase.AttributeCheck is not null)
			{
				allChecks.Add(phase.AttributeCheck);
			}

			if (phase.LoadMobVnum is not null)
			{
				warnings.Add(new CraftConversionWarning(
					"deferred-phase-mob-output",
					$"Phase {phase.PhaseNumber} loads mobile vnum {phase.LoadMobVnum.Value}, which is deferred until NPC import exists."));
			}

			if (!string.IsNullOrWhiteSpace(phase.SpellDefinition))
			{
				warnings.Add(new CraftConversionWarning(
					"unsupported-phase-spell",
					$"Phase {phase.PhaseNumber} preserves legacy spell data only."));
			}

			if (!string.IsNullOrWhiteSpace(phase.OpenDefinition))
			{
				warnings.Add(new CraftConversionWarning(
					"unsupported-phase-open",
					$"Phase {phase.PhaseNumber} preserves legacy open data only."));
			}

			if (!string.IsNullOrWhiteSpace(phase.RequirementDefinition))
			{
				warnings.Add(new CraftConversionWarning(
					"unsupported-phase-requirement",
					$"Phase {phase.PhaseNumber} preserves legacy requirement data only."));
			}

			foreach (var item in phase.ItemLists.OrderBy(x => x.Slot))
			{
				switch (item.Role)
				{
					case RpiCraftItemRole.Used:
						AddOrWarn(inputSlots, item.Slot, item, warnings, craft, "used input");
						break;
					case RpiCraftItemRole.Held:
					case RpiCraftItemRole.InRoom:
					case RpiCraftItemRole.Worn:
					case RpiCraftItemRole.Wielded:
						AddOrWarn(toolSlots, $"{item.Role}:{item.Slot}", item, warnings, craft, "tool");
						break;
					case RpiCraftItemRole.Produced:
					case RpiCraftItemRole.Give:
						productCandidates.Add(new SourceProductCandidate(
							phase.PhaseNumber,
							item.Slot,
							item.Quantity,
							false,
							item.Role == RpiCraftItemRole.Give,
							item.Vnums,
							item.RawDefinition));
						break;
					default:
						warnings.Add(new CraftConversionWarning(
							"unknown-item-role",
							$"Craft slot {item.Slot} in phase {phase.PhaseNumber} used unsupported role '{item.Role}' and was preserved as raw metadata only."));
						break;
				}
			}

			foreach (var failItem in phase.FailItemLists.OrderBy(x => x.Slot))
			{
				failProductCandidates.Add(new SourceProductCandidate(
					phase.PhaseNumber,
					failItem.Slot,
					failItem.Quantity,
					true,
					false,
					failItem.Vnums,
					failItem.RawDefinition));
			}

			if (phase.ToolList is not null)
			{
				AddOrWarn(toolSlots, $"tool:{phase.ToolList.Slot}", phase.ToolList, warnings, craft, "tool");
			}
		}

		if (craft.FailObjectVnums.Count > 0)
		{
			failProductCandidates.Add(new SourceProductCandidate(
				craft.Phases.Count == 0 ? 0 : craft.Phases.Max(x => x.PhaseNumber),
				0,
				1,
				true,
				false,
				craft.FailObjectVnums,
				string.Join(" ", craft.FailObjectVnums)));
		}

		if (craft.FailMobVnums.Count > 0)
		{
			warnings.Add(new CraftConversionWarning(
				"deferred-failmob-output",
				$"Craft declares failmobs ({string.Join(", ", craft.FailMobVnums)}), which are deferred until NPC import exists."));
		}

		var hidden = craft.Constraints.Flags.Any(x => x.Equals("hidden", StringComparison.OrdinalIgnoreCase));
		var legacyFlags = craft.Constraints.Flags
			.Where(x => !x.Equals("hidden", StringComparison.OrdinalIgnoreCase))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
		if (legacyFlags.Count > 0)
		{
			warnings.Add(new CraftConversionWarning(
				"legacy-flags-preserved",
				$"Unsupported legacy craft flags were preserved as metadata: {string.Join(", ", legacyFlags)}."));
		}

		var terrains = craft.Constraints.Sectors
			.Select(MapSectorToTerrain)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		var openingTraits = craft.Constraints.OpeningSkillIds
			.Select(ResolveOpeningTrait)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Cast<string>()
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		var openingSourceNames = craft.Constraints.OpeningSkillIds
			.Select(x => LegacyOpeningSkills.TryGetValue(x, out var name) ? name : $"Skill #{x}")
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		var raceNames = craft.Constraints.RaceIds
			.Select(ResolveRaceName)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Cast<string>()
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		var clanRequirements = craft.Constraints.ClanRequirements
			.Select(NormaliseClanRequirement)
			.ToList();

		var constraints = new ConvertedCraftConstraintSet(
			hidden,
			legacyFlags,
			terrains,
			craft.Constraints.Seasons.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
			craft.Constraints.WeatherStates.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
			openingTraits,
			openingSourceNames,
			raceNames,
			clanRequirements,
			craft.Constraints.FollowersRequired,
			craft.Constraints.IcDelay,
			craft.Constraints.FailDelay);

		if (craft.Constraints.IcDelay > 0)
		{
			warnings.Add(new CraftConversionWarning(
				"legacy-ic-delay",
				$"Legacy IC delay {craft.Constraints.IcDelay} was preserved as provenance only."));
		}

		if (craft.Constraints.FailDelay > 0)
		{
			warnings.Add(new CraftConversionWarning(
				"legacy-fail-delay",
				$"Legacy fail delay {craft.Constraints.FailDelay} was preserved as provenance only."));
		}

		var checkDefinitions = ResolveChecks(allChecks, warnings);
		var (primaryCheck, additionalChecks, freeSkillChecks, checkDifficulty) = SelectPrimaryCheck(checkDefinitions, warnings);
		var phases = craft.Phases
			.OrderBy(x => x.PhaseNumber)
			.Select(x => ConvertPhase(x, craft.FailureMessage, warnings))
			.ToList();
		var inputDefinitions = BuildInputs(craft, inputSlots.Values.OrderBy(x => x.Slot).ToList(), warnings);
		var inputIndexBySourceSlot = inputDefinitions.ToDictionary(x => x.SourceSlot, x => x.InputIndex);
		var toolDefinitions = BuildTools(craft, toolSlots.Values.ToList(), warnings);
		var products = BuildProducts(
			craft,
			productCandidates,
			failProductCandidates,
			inputDefinitions,
			inputIndexBySourceSlot,
			warnings);
		var progPlans = BuildProgPlans(craft, constraints, products);

		var status = warnings.Any(x => x.Code.StartsWith("deferred", StringComparison.OrdinalIgnoreCase))
			? CraftConversionStatus.Deferred
			: CraftConversionStatus.Ready;

		if (products.Count == 0)
		{
			status = CraftConversionStatus.Deferred;
			warnings.Add(new CraftConversionWarning(
				"deferred-no-products",
				"Craft did not produce any importable products in pass one."));
		}

		return new ConvertedCraftDefinition
		{
			SourceFile = craft.SourceFile,
			SourceLineNumber = craft.SourceLineNumber,
			CraftNumber = craft.CraftNumber,
			SourceKey = craft.SourceKey,
			CraftName = craft.CraftName,
			SubcraftName = craft.SubcraftName,
			Command = craft.Command,
			Name = BuildCraftName(craft),
			Category = Humanise(craft.CraftName),
			Blurb = BuildBlurb(craft),
			ActionDescription = BuildActionDescription(craft),
			ActiveCraftItemSdesc = BuildActionDescription(craft),
			Status = status,
			Constraints = constraints,
			PrimaryCheck = primaryCheck,
			AdditionalChecks = additionalChecks,
			FreeSkillChecks = freeSkillChecks,
			CheckDifficultyName = checkDifficulty,
			FailThresholdName = "MinorFail",
			FailPhase = phases.Count == 0 ? 1 : phases.Max(x => x.PhaseNumber),
			Phases = phases,
			Inputs = inputDefinitions,
			Tools = toolDefinitions,
			Products = products,
			GeneratedTags = inputDefinitions
				.SelectMany(x => x.UsesGeneratedTag && x.TagName is not null && _generatedTags.ContainsKey(x.TagName)
					? new[] { _generatedTags[x.TagName] }
					: Array.Empty<GeneratedCraftTagDefinition>())
				.Concat(toolDefinitions
					.SelectMany(x => x.UsesGeneratedTag && x.TagName is not null && _generatedTags.ContainsKey(x.TagName)
						? new[] { _generatedTags[x.TagName] }
						: Array.Empty<GeneratedCraftTagDefinition>()))
				.DistinctBy(x => x.TagName, StringComparer.OrdinalIgnoreCase)
				.ToList(),
			FutureProgPlans = progPlans,
			RawSourceLines = craft.RawLines,
			ParseWarnings = craft.ParseWarnings,
			Warnings = warnings
				.Concat(inputDefinitions.SelectMany(x => x.Warnings))
				.Concat(toolDefinitions.SelectMany(x => x.Warnings))
				.Concat(products.SelectMany(x => x.Warnings))
				.Distinct()
				.ToList()
		};
	}

	private static void AddOrWarn<TKey>(
		IDictionary<TKey, RpiCraftItemList> dictionary,
		TKey key,
		RpiCraftItemList value,
		ICollection<CraftConversionWarning> warnings,
		RpiCraftDefinition craft,
		string kind)
		where TKey : notnull
	{
		if (dictionary.TryGetValue(key, out var existing))
		{
			if (!existing.RawDefinition.Equals(value.RawDefinition, StringComparison.OrdinalIgnoreCase))
			{
				warnings.Add(new CraftConversionWarning(
					"duplicate-slot-definition",
					$"{craft.SourceKey} redefined {kind} slot {value.Slot}; keeping the earliest definition."));
			}

			return;
		}

		dictionary[key] = value;
	}

	private List<ConvertedCraftInputDefinition> BuildInputs(
		RpiCraftDefinition craft,
		IReadOnlyList<RpiCraftItemList> inputs,
		ICollection<CraftConversionWarning> warnings)
	{
		List<ConvertedCraftInputDefinition> results = [];
		var nextIndex = 1;
		foreach (var input in inputs)
		{
			var resolved = ResolveRequirement(craft, input, isTool: false);
			results.Add(new ConvertedCraftInputDefinition
			{
				SourceSlot = input.Slot,
				InputIndex = nextIndex++,
				InputType = resolved.Type,
				Quantity = resolved.Quantity,
				SourceVnum = resolved.SourceVnum,
				SourceItemKey = resolved.SourceItemKey,
				TagName = resolved.TagName,
				UsesGeneratedTag = resolved.UsesGeneratedTag,
				SourceVnums = resolved.SourceVnums,
				Warnings = resolved.Warnings
			});
		}

		if (results.Count == 0)
		{
			warnings.Add(new CraftConversionWarning("missing-inputs", "Craft has no consumable inputs."));
		}

		return results;
	}

	private List<ConvertedCraftToolDefinition> BuildTools(
		RpiCraftDefinition craft,
		IReadOnlyList<RpiCraftItemList> tools,
		ICollection<CraftConversionWarning> warnings)
	{
		List<ConvertedCraftToolDefinition> results = [];
		foreach (var tool in tools.OrderBy(x => x.Slot))
		{
			var resolved = ResolveRequirement(craft, tool, isTool: true);
			results.Add(new ConvertedCraftToolDefinition
			{
				SourceSlot = tool.Slot,
				ToolType = resolved.Type == "SimpleItem" ? "SimpleTool" : "TagTool",
				DesiredStateName = MapDesiredState(tool.Role),
				SourceVnum = resolved.SourceVnum,
				SourceItemKey = resolved.SourceItemKey,
				TagName = resolved.TagName,
				UsesGeneratedTag = resolved.UsesGeneratedTag,
				UseToolDuration = false,
				SourceVnums = resolved.SourceVnums,
				Warnings = resolved.Warnings
			});
		}

		if (results.Count == 0)
		{
			warnings.Add(new CraftConversionWarning("missing-tools", "Craft has no mapped tools; this may be intentional."));
		}

		return results;
	}

	private List<ConvertedCraftProductDefinition> BuildProducts(
		RpiCraftDefinition craft,
		IReadOnlyList<SourceProductCandidate> productCandidates,
		IReadOnlyList<SourceProductCandidate> failProductCandidates,
		IReadOnlyList<ConvertedCraftInputDefinition> inputs,
		IReadOnlyDictionary<int, int> inputIndexBySourceSlot,
		ICollection<CraftConversionWarning> warnings)
	{
		List<ConvertedCraftProductDefinition> results = [];
		var variantStartSlot = craft.VariantLink.StartKey;
		var variantEndSlot = craft.VariantLink.EndKey;
		var keyedProduct = variantStartSlot is not null && variantEndSlot is not null
			? productCandidates.FirstOrDefault(x => x.SourceSlot == variantEndSlot.Value)
			: null;
		var keyedCases = keyedProduct is not null && variantStartSlot is not null
			? BuildKeyedCases(craft, variantStartSlot.Value, keyedProduct, warnings)
			: Array.Empty<ConvertedCraftProgCase>();

		if (keyedProduct is not null && keyedCases.Count > 0)
		{
			var variantStartSlotValue = variantStartSlot!.Value;
			results.Add(new ConvertedCraftProductDefinition
			{
				SourceSlot = keyedProduct.SourceSlot,
				ProductType = "Prog",
				IsFailProduct = false,
				Quantity = 1,
				LegacyGiveOutput = keyedProduct.LegacyGiveOutput,
				SourceInputSlot = variantStartSlotValue,
				SourceInputIndex = inputIndexBySourceSlot.TryGetValue(variantStartSlotValue, out var inputIndex) ? inputIndex : null,
				GeneratedProgName = $"RPICraft_{craft.CraftNumber.ToString("D4", CultureInfo.InvariantCulture)}_Product_{keyedProduct.SourceSlot.ToString("D2", CultureInfo.InvariantCulture)}",
				ProgCases = keyedCases,
				SourceVnums = keyedProduct.SourceVnums,
				Warnings =
				[
					..(keyedProduct.LegacyGiveOutput
						? new[]
						{
							new CraftConversionWarning(
								"legacy-give-output",
								"Legacy give output was imported as a normal craft product that will appear in the room.")
						}
						: Array.Empty<CraftConversionWarning>())
				]
			});
		}

		foreach (var candidate in productCandidates
			         .Where(x => keyedProduct is null || x.SourceSlot != keyedProduct.SourceSlot)
			         .OrderBy(x => x.PhaseNumber)
			         .ThenBy(x => x.SourceSlot))
		{
			results.Add(ConvertSimpleProduct(candidate, false, craft, warnings));
		}

		foreach (var candidate in failProductCandidates.OrderBy(x => x.PhaseNumber).ThenBy(x => x.SourceSlot))
		{
			var recoveredInput = FindRecoveredInput(candidate, inputs, inputIndexBySourceSlot);
			if (recoveredInput is not null)
			{
				results.Add(recoveredInput);
				continue;
			}

			results.Add(ConvertSimpleProduct(candidate, true, craft, warnings));
		}

		return results;
	}

	private ConvertedCraftProductDefinition ConvertSimpleProduct(
		SourceProductCandidate candidate,
		bool isFailProduct,
		RpiCraftDefinition craft,
		ICollection<CraftConversionWarning> warnings)
	{
		var outputVnum = candidate.SourceVnums.FirstOrDefault();
		List<CraftConversionWarning> productWarnings = [];
		if (candidate.SourceVnums.Count > 1)
		{
			productWarnings.Add(new CraftConversionWarning(
				"collapsed-multi-output",
				$"Slot {candidate.SourceSlot} listed multiple output vnums; pass one uses the first ({outputVnum})."));
		}

		if (candidate.LegacyGiveOutput)
		{
			productWarnings.Add(new CraftConversionWarning(
				"legacy-give-output",
				"Legacy give output was imported as a normal craft product that will appear in the room."));
		}

		if (outputVnum == 0)
		{
			productWarnings.Add(new CraftConversionWarning(
				"missing-output-vnum",
				$"Craft slot {candidate.SourceSlot} had no usable output vnum."));
		}

		return new ConvertedCraftProductDefinition
		{
			SourceSlot = candidate.SourceSlot,
			ProductType = "SimpleProduct",
			IsFailProduct = isFailProduct,
			Quantity = Math.Max(candidate.Quantity, 1),
			OutputVnum = outputVnum == 0 ? null : outputVnum,
			OutputSourceItemKey = outputVnum != 0 && _itemsByVnum.TryGetValue(outputVnum, out var item) ? item.SourceKey : null,
			LegacyGiveOutput = candidate.LegacyGiveOutput,
			SourceVnums = candidate.SourceVnums,
			Warnings = productWarnings
		};
	}

	private ConvertedCraftProductDefinition? FindRecoveredInput(
		SourceProductCandidate candidate,
		IReadOnlyList<ConvertedCraftInputDefinition> inputs,
		IReadOnlyDictionary<int, int> inputIndexBySourceSlot)
	{
		var recoveredInput = inputs.FirstOrDefault(x =>
			x.SourceVnum is not null &&
			candidate.SourceVnums.Count == 1 &&
			x.SourceVnum.Value == candidate.SourceVnums[0] &&
			x.Quantity > 0);
		if (recoveredInput is null)
		{
			return null;
		}

		var percentageRecovered = Math.Min(1.0, Math.Round((double)candidate.Quantity / recoveredInput.Quantity, 4, MidpointRounding.AwayFromZero));
		return new ConvertedCraftProductDefinition
		{
			SourceSlot = candidate.SourceSlot,
			ProductType = "UnusedInput",
			IsFailProduct = true,
			Quantity = candidate.Quantity,
			OutputVnum = recoveredInput.SourceVnum,
			OutputSourceItemKey = recoveredInput.SourceItemKey,
			SourceInputSlot = recoveredInput.SourceSlot,
			SourceInputIndex = inputIndexBySourceSlot.TryGetValue(recoveredInput.SourceSlot, out var inputIndex) ? inputIndex : null,
			PercentageRecovered = percentageRecovered,
			SourceVnums = candidate.SourceVnums,
			Warnings =
			[
				new CraftConversionWarning(
					"unused-input-product",
					$"Fail product slot {candidate.SourceSlot} was mapped to UnusedInput using source slot {recoveredInput.SourceSlot}.")
			]
		};
	}

	private IReadOnlyList<ConvertedCraftProgCase> BuildKeyedCases(
		RpiCraftDefinition craft,
		int startSlot,
		SourceProductCandidate keyedProduct,
		ICollection<CraftConversionWarning> warnings)
	{
		var startInput = craft.Phases
			.SelectMany(x => x.ItemLists)
			.FirstOrDefault(x => x.Slot == startSlot && x.Role == RpiCraftItemRole.Used);
		if (startInput is null || startInput.Vnums.Count == 0 || keyedProduct.SourceVnums.Count == 0)
		{
			warnings.Add(new CraftConversionWarning(
				"missing-keyed-link",
				$"Craft start_key/end_key could not be resolved for slots {startSlot}/{keyedProduct.SourceSlot}."));
			return Array.Empty<ConvertedCraftProgCase>();
		}

		if (startInput.Vnums.Count != keyedProduct.SourceVnums.Count)
		{
			warnings.Add(new CraftConversionWarning(
				"keyed-output-mismatch",
				$"Keyed input slot {startSlot} and output slot {keyedProduct.SourceSlot} had different list lengths; pass one truncated to the shortest list."));
		}

		var count = Math.Min(startInput.Vnums.Count, keyedProduct.SourceVnums.Count);
		List<ConvertedCraftProgCase> results = [];
		for (var i = 0; i < count; i++)
		{
			results.Add(new ConvertedCraftProgCase(startInput.Vnums[i], keyedProduct.SourceVnums[i]));
		}

		return results;
	}

	private static string BuildCraftName(RpiCraftDefinition craft)
	{
		var command = Humanise(craft.Command);
		var subcraft = Humanise(craft.SubcraftName);
		return $"{command} {subcraft}".Trim();
	}

	private static string BuildBlurb(RpiCraftDefinition craft)
	{
		var subcraft = Humanise(craft.SubcraftName);
		var command = Humanise(craft.Command);
		return $"{command} {subcraft}".Trim();
	}

	private static string BuildActionDescription(RpiCraftDefinition craft)
	{
		return $"{Gerundise(Humanise(craft.Command))} {Humanise(craft.SubcraftName)}".Trim();
	}

	private static string Humanise(string value)
	{
		return value.Replace('_', ' ').Trim();
	}

	private static string Gerundise(string command)
	{
		if (string.IsNullOrWhiteSpace(command))
		{
			return "crafting";
		}

		var trimmed = command.Trim();
		if (trimmed.EndsWith("ie", StringComparison.OrdinalIgnoreCase))
		{
			return $"{trimmed[..^2]}ying";
		}

		if (trimmed.EndsWith('e') && !trimmed.EndsWith("ee", StringComparison.OrdinalIgnoreCase))
		{
			return $"{trimmed[..^1]}ing";
		}

		return $"{trimmed}ing";
	}

	private ConvertedCraftPhaseDefinition ConvertPhase(
		RpiCraftPhaseRecord phase,
		string? failureMessage,
		ICollection<CraftConversionWarning> warnings)
	{
		if (phase.Cost?.Hits > 0)
		{
			warnings.Add(new CraftConversionWarning(
				"legacy-hit-cost",
				$"Phase {phase.PhaseNumber} had a legacy hit cost of {phase.Cost.Hits}, which is preserved as provenance only."));
		}

		return new ConvertedCraftPhaseDefinition
		{
			PhaseNumber = phase.PhaseNumber,
			Seconds = Math.Max(phase.Seconds, 1),
			Echo = ChooseEcho(phase.Echoes.Third, phase.Echoes.First, phase.Echoes.Second, phase.Echoes.Self, phase.Echoes.Group) ?? "You work carefully on the craft.",
			FailEcho = ChooseEcho(phase.Echoes.ThirdFail, phase.Echoes.FirstFail, phase.Echoes.SecondFail, phase.Echoes.GroupFail, failureMessage) ?? "The crafting attempt goes awry.",
			ExertionLevelName = DetermineExertionLevelName(phase.Cost?.Moves ?? 0),
			StaminaUsage = phase.Cost?.Moves ?? 0,
			RawEchoes = phase.Echoes,
			RawLines = phase.RawLines
		};
	}

	private static string? ChooseEcho(params string?[] candidates)
	{
		return candidates.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim();
	}

	private static string DetermineExertionLevelName(int moves)
	{
		return moves switch
		{
			<= 0 => "Rest",
			<= 10 => "Low",
			<= 25 => "Normal",
			<= 45 => "Heavy",
			<= 70 => "VeryHeavy",
			_ => "ExtremelyHeavy",
		};
	}

	private List<ConvertedCraftCheckDefinition> ResolveChecks(
		IReadOnlyList<RpiCraftCheckRecord> checks,
		ICollection<CraftConversionWarning> warnings)
	{
		List<ConvertedCraftCheckDefinition> results = [];
		foreach (var check in checks.OrderBy(x => x.PhaseNumber))
		{
			var trait = ResolveTraitName(check);
			if (string.IsNullOrWhiteSpace(trait))
			{
				warnings.Add(new CraftConversionWarning(
					"unresolved-craft-trait",
					$"Could not resolve {check.Kind} '{check.SourceName}' in phase {check.PhaseNumber}."));
				continue;
			}

			results.Add(new ConvertedCraftCheckDefinition(
				trait,
				check.PhaseNumber,
				check.Kind,
				check.SourceName,
				check.RawExpression,
				MapDifficultyName(check.Dice * check.Sides)));
		}

		return results;
	}

	private static (ConvertedCraftCheckDefinition? Primary, IReadOnlyList<ConvertedCraftCheckDefinition> Additional, int FreeChecks, string DifficultyName)
		SelectPrimaryCheck(
			IReadOnlyList<ConvertedCraftCheckDefinition> checks,
			ICollection<CraftConversionWarning> warnings)
	{
		if (checks.Count == 0)
		{
			return (null, Array.Empty<ConvertedCraftCheckDefinition>(), 0, "Automatic");
		}

		var ordered = checks
			.OrderBy(x => x.PhaseNumber)
			.ThenBy(x => x.Kind == RpiCraftCheckKind.Skill ? 1 : 0)
			.ToList();
		var distinctTraits = ordered
			.Select(x => x.TraitName)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
		var primary = ordered.Last();
		if (distinctTraits.Count > 1)
		{
			warnings.Add(new CraftConversionWarning(
				"multi-check-reduced",
				$"Craft used multiple distinct checks ({string.Join(", ", distinctTraits)}); pass one uses {primary.TraitName} as the primary craft check."));
		}

		var freeChecks = distinctTraits.Count == 1
			? Math.Max(0, ordered.Count - 1)
			: 0;
		return (primary, ordered.Where(x => x != primary).ToList(), freeChecks, primary.DifficultyName);
	}

	private static string MapDifficultyName(int difficultyScore)
	{
		return difficultyScore switch
		{
			<= 5 => "Trivial",
			<= 10 => "VeryEasy",
			<= 20 => "Easy",
			<= 30 => "Normal",
			<= 40 => "Hard",
			<= 60 => "VeryHard",
			<= 100 => "ExtremelyHard",
			_ => "Insane",
		};
	}

	private string? ResolveTraitName(RpiCraftCheckRecord check)
	{
		var key = NormaliseToken(check.SourceName);
		return check.Kind == RpiCraftCheckKind.Skill
			? SkillTraitAliases.TryGetValue(key, out var trait) ? trait : null
			: AttributeTraitAliases.TryGetValue(key, out var attribute) ? attribute : null;
	}

	private static string? ResolveOpeningTrait(int openingId)
	{
		if (!LegacyOpeningSkills.TryGetValue(openingId, out var legacy))
		{
			return null;
		}

		var key = NormaliseToken(legacy);
		return SkillTraitAliases.TryGetValue(key, out var trait) ? trait : null;
	}

	private static string? ResolveRaceName(int raceId)
	{
		return LegacyRaceNames.TryGetValue(raceId.ToString(CultureInfo.InvariantCulture), out var race)
			? race
			: null;
	}

	private static ConvertedCraftClanRequirement NormaliseClanRequirement(RpiCraftClanRequirement requirement)
	{
		var rule = CanonicalClanRules.TryGetValue(requirement.ClanAlias, out var canonical)
			? canonical
			: new NormalisedClanRule(requirement.ClanAlias, RpiClanRankSlots.TitleCaseAlias(requirement.ClanAlias), Array.Empty<string>());
		return new ConvertedCraftClanRequirement(
			rule.CanonicalAlias,
			rule.FullNameOverride ?? RpiClanRankSlots.TitleCaseAlias(rule.CanonicalAlias),
			string.IsNullOrWhiteSpace(requirement.RankName) ? null : requirement.RankName,
			rule.LegacyAliases);
	}

	private ResolvedCraftRequirement ResolveRequirement(RpiCraftDefinition craft, RpiCraftItemList item, bool isTool)
	{
		List<CraftConversionWarning> warnings = [];
		var vnums = item.Vnums.Distinct().ToList();
		if (vnums.Count == 0)
		{
			return new ResolvedCraftRequirement(
				"SimpleItem",
				Math.Max(item.Quantity, 1),
				null,
				null,
				null,
				false,
				Array.Empty<int>(),
				[
					new CraftConversionWarning(
						"missing-input-vnums",
						$"Slot {item.Slot} did not contain any usable vnums.")
				]);
		}

		if (vnums.Count == 1)
		{
			var vnum = vnums[0];
			return new ResolvedCraftRequirement(
				"SimpleItem",
				Math.Max(item.Quantity, 1),
				vnum,
				_itemsByVnum.TryGetValue(vnum, out var convertedItem) ? convertedItem.SourceKey : null,
				null,
				false,
				vnums,
				_itemsByVnum.ContainsKey(vnum)
					? warnings
					: warnings.Append(new CraftConversionWarning(
						"missing-item-mapping",
						$"Craft slot {item.Slot} referenced item vnum {vnum}, but no converted item mapping was available.")).ToList());
		}

		var referenced = vnums
			.Where(_itemsByVnum.ContainsKey)
			.Select(vnum => _itemsByVnum[vnum])
			.ToList();
		if (referenced.Count == vnums.Count)
		{
			var commonTag = referenced
				.Select(x => x.TagNames.Where(IsReusableCraftTag).ToHashSet(StringComparer.OrdinalIgnoreCase))
				.Aggregate((left, right) =>
				{
					left.IntersectWith(right);
					return left;
				})
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.FirstOrDefault();
			if (!string.IsNullOrWhiteSpace(commonTag))
			{
				return new ResolvedCraftRequirement(
					"Tag",
					Math.Max(item.Quantity, 1),
					null,
					null,
					commonTag,
					false,
					vnums,
					warnings);
			}

			var familyName = ChooseFamilyName(referenced);
			if (!string.IsNullOrWhiteSpace(familyName))
			{
				var tagName = $"RPI Family - {familyName}";
				RegisterGeneratedTag(tagName, vnums, true, $"Family tag for {familyName}.");
				return new ResolvedCraftRequirement(
					"Tag",
					Math.Max(item.Quantity, 1),
					null,
					null,
					tagName,
					true,
					vnums,
					warnings);
			}
		}

		var explicitTag = $"RPI Craft Set {craft.CraftNumber.ToString("D4", CultureInfo.InvariantCulture)} {(isTool ? "Tool" : "Input")} {item.Slot.ToString("D2", CultureInfo.InvariantCulture)}";
		RegisterGeneratedTag(explicitTag, vnums, false, $"Explicit set tag for craft {craft.SourceKey} slot {item.Slot}.");
		warnings.Add(new CraftConversionWarning(
			"explicit-set-tag",
			$"Craft slot {item.Slot} used a generated explicit-set tag because no safe shared family could be inferred."));
		return new ResolvedCraftRequirement(
			"Tag",
			Math.Max(item.Quantity, 1),
			null,
			null,
			explicitTag,
			true,
			vnums,
			warnings);
	}

	private static bool IsReusableCraftTag(string tag)
	{
		return !GenericCraftTags.Contains(tag);
	}

	private static string? ChooseFamilyName(IReadOnlyList<ConvertedItemDefinition> items)
	{
		if (items.Count == 0)
		{
			return null;
		}

		HashSet<string>? shared = null;
		foreach (var item in items)
		{
			var tokens = ExtractFamilyTokens(item).ToHashSet(StringComparer.OrdinalIgnoreCase);
			shared = shared is null
				? tokens
				: shared.Intersect(tokens, StringComparer.OrdinalIgnoreCase).ToHashSet(StringComparer.OrdinalIgnoreCase);
			if (shared.Count == 0)
			{
				return null;
			}
		}

		if (shared is null || shared.Count == 0)
		{
			return null;
		}

		foreach (var token in FamilyTokens.Keys)
		{
			if (shared.Contains(token))
			{
				return FamilyTokens[token];
			}
		}

		var best = shared
			.OrderBy(x => x.Length)
			.FirstOrDefault();
		return string.IsNullOrWhiteSpace(best)
			? null
			: CultureInfo.InvariantCulture.TextInfo.ToTitleCase(best);
	}

	private static IEnumerable<string> ExtractFamilyTokens(ConvertedItemDefinition item)
	{
		var source = $"{item.BaseName} {item.Keywords} {item.ShortDescription}";
		foreach (var token in Regex.Split(source.ToLowerInvariant(), @"[^a-z0-9]+"))
		{
			if (string.IsNullOrWhiteSpace(token) || IgnoredTokens.Contains(token))
			{
				continue;
			}

			yield return token;
		}
	}

	private void RegisterGeneratedTag(string tagName, IReadOnlyCollection<int> sourceVnums, bool reusable, string reason)
	{
		if (_generatedTags.TryGetValue(tagName, out var existing))
		{
			_generatedTags[tagName] = existing with
			{
				SourceVnums = existing.SourceVnums
					.Concat(sourceVnums)
					.Distinct()
					.OrderBy(x => x)
					.ToList(),
				Reusable = existing.Reusable || reusable
			};
			return;
		}

		_generatedTags[tagName] = new GeneratedCraftTagDefinition(
			tagName,
			sourceVnums.Distinct().OrderBy(x => x).ToList(),
			reusable,
			reason);
	}

	private static string MapDesiredState(RpiCraftItemRole role)
	{
		return role switch
		{
			RpiCraftItemRole.Held => "Held",
			RpiCraftItemRole.Wielded => "Wielded",
			RpiCraftItemRole.Worn => "Worn",
			RpiCraftItemRole.InRoom => "InRoom",
			_ => "Held",
		};
	}

	private static string MapSectorToTerrain(string sector)
	{
		return NormaliseToken(sector) switch
		{
			"inside" => "Hall",
			"city" => "Urban Street",
			"road" => "Compacted Dirt Road",
			"trail" => "Trail",
			"field" => "Field",
			"woods" => "Broadleaf Forest",
			"forest" => "Broadleaf Forest",
			"hills" => "Hills",
			"mountains" or "mountain" => "Mountainside",
			"swamp" => "Temperate Freshwater Swamp",
			"dock" => "Riverbank",
			"crowsnest" => "Rooftop",
			"pasture" => "Pasture",
			"heath" => "Heath",
			"pit" => "Dungeon",
			"lean to" or "leanto" => "Cave Entrance",
			"lake" => "Lake",
			"river" => "River",
			"ocean" => "Ocean",
			"reef" => "Reef",
			"underwater" => "Deep Lake",
			_ => Humanise(sector),
		};
	}

	private static string NormaliseToken(string value)
	{
		return Regex.Replace(value, @"[^a-z0-9]+", " ", RegexOptions.IgnoreCase)
			.Trim()
			.ToLowerInvariant();
	}

	private static IReadOnlyList<CraftFutureProgPlan> BuildProgPlans(
		RpiCraftDefinition craft,
		ConvertedCraftConstraintSet constraints,
		IReadOnlyList<ConvertedCraftProductDefinition> products)
	{
		List<CraftFutureProgPlan> plans = [];
		var prefix = $"RPICraft_{craft.CraftNumber.ToString("D4", CultureInfo.InvariantCulture)}";
		if (constraints.HiddenFromCraftList ||
		    constraints.TerrainNames.Count > 0 ||
		    constraints.Seasons.Count > 0 ||
		    constraints.WeatherStates.Count > 0 ||
		    constraints.OpeningTraitNames.Count > 0 ||
		    constraints.RaceNames.Count > 0 ||
		    constraints.ClanRequirements.Count > 0 ||
		    constraints.FollowersRequired > 0)
		{
			plans.Add(new CraftFutureProgPlan(
				$"{prefix}_Appear",
				"AppearInCraftsListProg",
				"Controls whether the craft appears in listings.",
				constraints.HiddenFromCraftList ? "return false" : "return <can use logic>"));
			plans.Add(new CraftFutureProgPlan(
				$"{prefix}_CanUse",
				"CanUseProg",
				"Controls whether the craft can be started.",
				BuildConstraintPreview(constraints, returnsText: false)));
			plans.Add(new CraftFutureProgPlan(
				$"{prefix}_Why",
				"WhyCannotUseProg",
				"Explains why the craft cannot be started.",
				BuildConstraintPreview(constraints, returnsText: true)));
		}

		foreach (var product in products.Where(x => x.ProductType == "Prog" && !string.IsNullOrWhiteSpace(x.GeneratedProgName)))
		{
			plans.Add(new CraftFutureProgPlan(
				product.GeneratedProgName!,
				"ProgProduct",
				$"Selects the keyed output for source input slot {product.SourceInputSlot}.",
				$"Maps {product.ProgCases.Count} input prototype(s) to output prototype(s)."));
		}

		return plans;
	}

	private static string BuildConstraintPreview(ConvertedCraftConstraintSet constraints, bool returnsText)
	{
		List<string> pieces = [];
		if (constraints.OpeningTraitNames.Count > 0)
		{
			pieces.Add($"opening traits: {string.Join(", ", constraints.OpeningTraitNames)}");
		}

		if (constraints.TerrainNames.Count > 0)
		{
			pieces.Add($"terrains: {string.Join(", ", constraints.TerrainNames)}");
		}

		if (constraints.Seasons.Count > 0)
		{
			pieces.Add($"seasons: {string.Join(", ", constraints.Seasons)}");
		}

		if (constraints.WeatherStates.Count > 0)
		{
			pieces.Add($"weather: {string.Join(", ", constraints.WeatherStates)}");
		}

		if (constraints.RaceNames.Count > 0)
		{
			pieces.Add($"races: {string.Join(", ", constraints.RaceNames)}");
		}

		if (constraints.ClanRequirements.Count > 0)
		{
			pieces.Add($"clans: {string.Join(", ", constraints.ClanRequirements.Select(x => x.CanonicalAlias))}");
		}

		if (constraints.FollowersRequired > 0)
		{
			pieces.Add($"followers: {constraints.FollowersRequired}");
		}

		if (pieces.Count == 0)
		{
			return returnsText ? "return \"\"" : "return true";
		}

		return returnsText
			? $"return first failing reason for {string.Join("; ", pieces)}"
			: $"return all of {string.Join("; ", pieces)}";
	}

	internal static IReadOnlyList<string> ResolveSeasonAliases(IEnumerable<string> seasons)
	{
		var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var season in seasons)
		{
			if (SeasonMonthAliases.TryGetValue(NormaliseToken(season), out var values))
			{
				aliases.UnionWith(values);
			}
		}

		return aliases.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
	}
}
