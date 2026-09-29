#nullable enable
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.NPC.AI.Groups;
using MudSharp.Work.Crafts;
using MoreLinq;

namespace MudSharp.NPC.AI;

public enum AnimalMovementStrategyType
{
	Ground,
	Swim,
	Fly,
	Arboreal,
	Amphibious
}

public enum AnimalHomeStrategyType
{
	None,
	Territorial,
	Denning
}

public enum AnimalFeedingStrategyType
{
	None,
	Predator,
	DenPredator,
	Forager,
	Scavenger,
	Opportunist,
	Omnivore,
	DenOmnivore
}

public enum AnimalWaterStrategyType
{
	Off,
	Drink,
	Immerse,
	Surface
}

public enum AnimalThreatStrategyType
{
	Passive,
	Flee,
	Defend,
	HungryPredator
}

public enum AnimalAwarenessStrategyType
{
	None,
	Wary,
	Wimpy,
	Skittish,
	Guarding
}

public enum AnimalRefugeStrategyType
{
	None,
	Home,
	Den,
	Trees,
	Sky,
	Water,
	Prog
}

public enum AnimalActivityStrategyType
{
	Always,
	Diurnal,
	Nocturnal,
	Crepuscular,
	Custom
}

/// <summary>
/// Specifies how an inactive seasonal animal rests. Older XML implicitly uses Rest, preserving
/// its existing activity behaviour; Hibernation and Torpor make a configured dormant season
/// authoritative until survival needs, combat or an immediate threat interrupts it.
/// </summary>
public enum AnimalDormancyMode
{
	Rest,
	Hibernation,
	Torpor
}

/// <summary>
/// The response an animal takes when a contextual threat rule applies. Inherit keeps the
/// legacy <see cref="AnimalThreatStrategyType"/> behaviour, which is also the default for
/// older XML definitions.
/// </summary>
public enum AnimalThreatResponseType
{
	Inherit,
	Ignore,
	Avoid,
	Flee,
	Posture,
	Attack
}

/// <summary>
/// Extra animal sensory behaviour layered over the existing awareness strategies.
/// </summary>
public enum AnimalSensesStrategyType
{
	None,
	Vigilant,
	Hiding,
	Stalking,
	Tracking
}

public partial class AnimalAI : CreatureAIBase
{
	public AnimalFeedingStrategyType FeedingStrategy { get; private set; }
	public AnimalThreatStrategyType ThreatStrategy { get; private set; }
	public AnimalActivityStrategyType ActivityStrategy { get; private set; }
	public AnimalDormancyMode DormancyMode { get; private set; }
	public bool UseActiveNeeds { get; private set; }
	public IFutureProg WillAttackProg { get; private set; } = null!;
	public IFutureProg ShelterNeededProg { get; private set; } = null!;
	public IFutureProg ShelterCellProg { get; private set; } = null!;
	public IFutureProg SeasonalCellProg { get; private set; } = null!;
	public IFutureProg NestSiteProg { get; private set; } = null!;
	public IFutureProg ProtectProg { get; private set; } = null!;
	public AnimalThreatResponseType OrdinaryThreatResponse { get; private set; }
	public AnimalThreatResponseType HungryPreyResponse { get; private set; }
	public AnimalThreatResponseType AttackedThreatResponse { get; private set; }
	public AnimalThreatResponseType TerritoryThreatResponse { get; private set; }
	public AnimalThreatResponseType ParentingThreatResponse { get; private set; }
	public AnimalThreatResponseType SeasonalThreatResponse { get; private set; }

	private readonly List<TimeOfDay> _activeTimesOfDay = new();
	private readonly List<string> _dormantSeasonGroups = new();
	private readonly List<string> _aggressiveSeasonGroups = new();
	private readonly List<string> _nestingSeasonGroups = new();
	private readonly Dictionary<string, IFutureProg> _seasonalHabitatProgs =
		new(StringComparer.InvariantCultureIgnoreCase);

	public bool ActivitySleepEnabled { get; private set; }
	public string ActivityRestEmote { get; private set; } = string.Empty;
	public bool EcologyShelterEnabled { get; private set; }
	public bool EcologySeasonalEnabled { get; private set; }
	public bool EcologyNestingEnabled { get; private set; }
	public bool EcologyParentingEnabled { get; private set; }
	public IEnumerable<TimeOfDay> ActiveTimesOfDay => _activeTimesOfDay;
	public IEnumerable<string> DormantSeasonGroups => _dormantSeasonGroups;
	public IEnumerable<string> AggressiveSeasonGroups => _aggressiveSeasonGroups;
	public IEnumerable<string> NestingSeasonGroups => _nestingSeasonGroups;
	public IReadOnlyDictionary<string, IFutureProg> SeasonalHabitatProgs => _seasonalHabitatProgs;

	public override bool CountsAsAggressive => ThreatStrategy.In(AnimalThreatStrategyType.Defend,
		AnimalThreatStrategyType.HungryPredator) ||
		new[]
		{
			OrdinaryThreatResponse, HungryPreyResponse, AttackedThreatResponse, TerritoryThreatResponse,
			ParentingThreatResponse, SeasonalThreatResponse
		}.Contains(AnimalThreatResponseType.Attack);

	public override bool IsReadyToBeUsed => GetReadiness().Ready;

	protected AnimalAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private AnimalAI(IFuturemud gameworld, string name) : base(gameworld, name, "Animal")
	{
		SetDefaults();
		DatabaseInitialise();
	}

	private AnimalAI()
	{
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Animal", (ai, gameworld) => new AnimalAI(ai, gameworld));
		RegisterAIBuilderInformation("animal", (gameworld, name) => new AnimalAI(gameworld, name),
			new AnimalAI().HelpText);
	}

	private void SetDefaults()
	{
		SetCreatureDefaults();
		FeedingStrategy = AnimalFeedingStrategyType.None;
		ThreatStrategy = AnimalThreatStrategyType.Passive;
		ActivityStrategy = AnimalActivityStrategyType.Always;
		DormancyMode = AnimalDormancyMode.Rest;
		UseActiveNeeds = false;
		OrdinaryThreatResponse = AnimalThreatResponseType.Inherit;
		HungryPreyResponse = AnimalThreatResponseType.Inherit;
		AttackedThreatResponse = AnimalThreatResponseType.Inherit;
		TerritoryThreatResponse = AnimalThreatResponseType.Inherit;
		ParentingThreatResponse = AnimalThreatResponseType.Inherit;
		SeasonalThreatResponse = AnimalThreatResponseType.Inherit;
		ActivitySleepEnabled = false;
		ActivityRestEmote = string.Empty;
		EcologyShelterEnabled = false;
		EcologySeasonalEnabled = false;
		EcologyNestingEnabled = false;
		EcologyParentingEnabled = false;
		_activeTimesOfDay.Clear();
		_activeTimesOfDay.AddRange(Enum.GetValues<TimeOfDay>());
		_dormantSeasonGroups.Clear();
		_aggressiveSeasonGroups.Clear();
		_nestingSeasonGroups.Clear();
		_seasonalHabitatProgs.Clear();

		if (Gameworld is not null)
		{
			WillAttackProg = Gameworld.AlwaysFalseProg;
			ShelterNeededProg = Gameworld.AlwaysFalseProg;
			ShelterCellProg = Gameworld.AlwaysFalseProg;
			SeasonalCellProg = Gameworld.AlwaysFalseProg;
			NestSiteProg = Gameworld.AlwaysFalseProg;
			ProtectProg = Gameworld.AlwaysFalseProg;
		}
	}

	protected override void LoadFromXML(XElement root)
	{
		SetDefaults();
		base.LoadFromXML(root);
		LoadCreatureDefinition(root);

		XElement feeding = root.Element("Feeding") ?? new XElement("Feeding");
		FeedingStrategy = ParseEnum(feeding.Attribute("type")?.Value, AnimalFeedingStrategyType.None);
		WillAttackProg =
			Gameworld.FutureProgs.Get(long.Parse(feeding.Element("WillAttackProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		UseActiveNeeds = bool.Parse(feeding.Element("UseActiveNeeds")?.Value ?? "false");
		EngageDelayDiceExpression = feeding.Element("EngageDelayDiceExpression")?.Value ?? "1000+1d1000";
		EngageEmote = feeding.Element("EngageEmote")?.Value ?? string.Empty;

		XElement water = root.Element("Water") ?? new XElement("Water");
		WaterStrategy = water.Attribute("type") is XAttribute waterType
			? ParseEnum(waterType.Value, AnimalWaterStrategyType.Drink)
			: bool.Parse(water.Attribute("enabled")?.Value ?? "true")
				? AnimalWaterStrategyType.Drink
				: AnimalWaterStrategyType.Off;

		XElement threat = root.Element("Threat") ?? new XElement("Threat");
		ThreatStrategy = ParseEnum(threat.Attribute("type")?.Value, AnimalThreatStrategyType.Passive);
		OrdinaryThreatResponse = ParseEnum(threat.Element("OrdinaryResponse")?.Value,
			AnimalThreatResponseType.Inherit);
		HungryPreyResponse = ParseEnum(threat.Element("HungryPreyResponse")?.Value,
			AnimalThreatResponseType.Inherit);
		AttackedThreatResponse = ParseEnum(threat.Element("AttackedResponse")?.Value,
			AnimalThreatResponseType.Inherit);
		TerritoryThreatResponse = ParseEnum(threat.Element("TerritoryResponse")?.Value,
			AnimalThreatResponseType.Inherit);
		ParentingThreatResponse = ParseEnum(threat.Element("ParentingResponse")?.Value,
			AnimalThreatResponseType.Inherit);
		SeasonalThreatResponse = ParseEnum(threat.Element("SeasonalResponse")?.Value,
			AnimalThreatResponseType.Inherit);
		PostureEmote = threat.Element("PostureEmote")?.Value ?? string.Empty;
		PostureDurationDiceExpression = threat.Element("PostureDurationDiceExpression")?.Value ?? "1d20+20";

		XElement activity = root.Element("Activity") ?? new XElement("Activity");
		ActivityStrategy = ParseEnum(activity.Attribute("type")?.Value, AnimalActivityStrategyType.Always);
		DormancyMode = ParseEnum(activity.Element("DormancyMode")?.Value, AnimalDormancyMode.Rest);
		ActivitySleepEnabled = bool.Parse(activity.Element("SleepEnabled")?.Value ?? "false");
		ActivityRestEmote = activity.Element("RestEmote")?.Value ?? string.Empty;
		LoadActiveTimes(activity);
		_dormantSeasonGroups.AddRange(activity.Elements("DormantSeasonGroup")
			.Select(x => x.Value.Trim())
			.Where(x => !string.IsNullOrEmpty(x))
			.Distinct(StringComparer.InvariantCultureIgnoreCase));
		_aggressiveSeasonGroups.AddRange(activity.Elements("AggressiveSeasonGroup")
			.Select(x => x.Value.Trim())
			.Where(x => !string.IsNullOrEmpty(x))
			.Distinct(StringComparer.InvariantCultureIgnoreCase));
		_nestingSeasonGroups.AddRange(activity.Elements("NestingSeasonGroup")
			.Select(x => x.Value.Trim())
			.Where(x => !string.IsNullOrEmpty(x))
			.Distinct(StringComparer.InvariantCultureIgnoreCase));

		XElement ecology = root.Element("Ecology") ?? new XElement("Ecology");
		EcologyShelterEnabled = bool.Parse(ecology.Element("ShelterEnabled")?.Value ?? "false");
		EcologySeasonalEnabled = bool.Parse(ecology.Element("SeasonalEnabled")?.Value ?? "false");
		EcologyNestingEnabled = bool.Parse(ecology.Element("NestingEnabled")?.Value ?? "false");
		EcologyParentingEnabled = bool.Parse(ecology.Element("ParentingEnabled")?.Value ?? "false");
		ShelterNeededProg =
			Gameworld.FutureProgs.Get(long.Parse(ecology.Element("ShelterNeededProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		ShelterCellProg =
			Gameworld.FutureProgs.Get(long.Parse(ecology.Element("ShelterCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		SeasonalCellProg =
			Gameworld.FutureProgs.Get(long.Parse(ecology.Element("SeasonalCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		NestSiteProg =
			Gameworld.FutureProgs.Get(long.Parse(ecology.Element("NestSiteProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		ProtectProg =
			Gameworld.FutureProgs.Get(long.Parse(ecology.Element("ProtectProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		foreach (XElement element in ecology.Elements("SeasonalHabitat"))
		{
			string seasonGroup = element.Attribute("seasonGroup")?.Value.Trim() ?? string.Empty;
			if (string.IsNullOrEmpty(seasonGroup) || !long.TryParse(element.Value, out long progId))
			{
				continue;
			}

			IFutureProg? prog = Gameworld.FutureProgs.Get(progId);
			if (prog is not null)
			{
				_seasonalHabitatProgs[seasonGroup] = prog;
			}
		}
	}

	protected override string SaveToXml()
	{
		return SaveDefinition().ToString();
	}

	internal XElement SaveDefinition()
	{
		return new XElement("Definition",
			SaveCreatureDefinition(),
			new XElement("Feeding",
				new XAttribute("type", FeedingStrategy),
				new XElement("WillAttackProg", WillAttackProg?.Id ?? 0),
				new XElement("UseActiveNeeds", UseActiveNeeds),
				new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
				new XElement("EngageEmote", new XCData(EngageEmote))),
			new XElement("Water", new XAttribute("type", WaterStrategy)),
			new XElement("Threat",
				new XAttribute("type", ThreatStrategy),
				new XElement("OrdinaryResponse", OrdinaryThreatResponse),
				new XElement("HungryPreyResponse", HungryPreyResponse),
				new XElement("AttackedResponse", AttackedThreatResponse),
				new XElement("TerritoryResponse", TerritoryThreatResponse),
				new XElement("ParentingResponse", ParentingThreatResponse),
				new XElement("SeasonalResponse", SeasonalThreatResponse),
				new XElement("PostureEmote", new XCData(PostureEmote)),
				new XElement("PostureDurationDiceExpression", new XCData(PostureDurationDiceExpression))),
			new XElement("Activity",
				new XAttribute("type", ActivityStrategy),
				new XElement("SleepEnabled", ActivitySleepEnabled),
				new XElement("DormancyMode", DormancyMode),
				new XElement("RestEmote", new XCData(ActivityRestEmote)),
				_dormantSeasonGroups.Select(x => new XElement("DormantSeasonGroup", x)),
				_aggressiveSeasonGroups.Select(x => new XElement("AggressiveSeasonGroup", x)),
				_nestingSeasonGroups.Select(x => new XElement("NestingSeasonGroup", x)),
				_activeTimesOfDay.Select(x => new XElement("ActiveTime", x))),
			new XElement("Ecology",
				new XElement("ShelterEnabled", EcologyShelterEnabled),
				new XElement("SeasonalEnabled", EcologySeasonalEnabled),
				new XElement("NestingEnabled", EcologyNestingEnabled),
				new XElement("ParentingEnabled", EcologyParentingEnabled),
				new XElement("ShelterNeededProg", ShelterNeededProg?.Id ?? 0),
				new XElement("ShelterCellProg", ShelterCellProg?.Id ?? 0),
				new XElement("SeasonalCellProg", SeasonalCellProg?.Id ?? 0),
				new XElement("NestSiteProg", NestSiteProg?.Id ?? 0),
				new XElement("ProtectProg", ProtectProg?.Id ?? 0),
				_seasonalHabitatProgs
					.OrderBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase)
					.Select(x => new XElement("SeasonalHabitat",
						new XAttribute("seasonGroup", x.Key), x.Value.Id))),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		);
	}

	internal static (bool Ready, string Reason) ValidateConfiguration(AnimalHomeStrategyType home,
		AnimalFeedingStrategyType feeding, AnimalThreatStrategyType threat)
	{
		return ValidateConfiguration(home, feeding, threat, AnimalMovementStrategyType.Ground,
			AnimalRefugeStrategyType.None, AnimalActivityStrategyType.Always, Enum.GetValues<TimeOfDay>());
	}

	internal static (bool Ready, string Reason) ValidateConfiguration(
		AnimalHomeStrategyType home,
		AnimalFeedingStrategyType feeding,
		AnimalThreatStrategyType threat,
		AnimalMovementStrategyType movement,
		AnimalRefugeStrategyType refuge,
		AnimalActivityStrategyType activity,
		IEnumerable<TimeOfDay> activeTimes)
	{
		return ValidateConfiguration(home, feeding, threat, movement, refuge, activity, activeTimes,
			AnimalWaterStrategyType.Drink, false, AnimalAwarenessStrategyType.None, false, false, false, false);
	}

	internal static (bool Ready, string Reason) ValidateConfiguration(
		AnimalHomeStrategyType home,
		AnimalFeedingStrategyType feeding,
		AnimalThreatStrategyType threat,
		AnimalMovementStrategyType movement,
		AnimalRefugeStrategyType refuge,
		AnimalActivityStrategyType activity,
		IEnumerable<TimeOfDay> activeTimes,
		AnimalWaterStrategyType water,
		bool hasWaterCellProg,
		AnimalAwarenessStrategyType awareness,
		bool ecologyNesting,
		bool hasNestSiteProg,
		bool ecologyParenting,
		bool hasProtectProg)
	{
		if (feeding.In(AnimalFeedingStrategyType.DenPredator, AnimalFeedingStrategyType.DenOmnivore) &&
		    home != AnimalHomeStrategyType.Denning)
		{
			return (false, "den feeding requires denning home behavior");
		}

		if (threat == AnimalThreatStrategyType.HungryPredator &&
		    !feeding.In(AnimalFeedingStrategyType.Predator, AnimalFeedingStrategyType.DenPredator,
			    AnimalFeedingStrategyType.Omnivore, AnimalFeedingStrategyType.DenOmnivore))
		{
			return (false, "hungry-predator threat behavior requires predator feeding behavior");
		}

		if (refuge == AnimalRefugeStrategyType.Den && home != AnimalHomeStrategyType.Denning)
		{
			return (false, "den refuge requires denning home behavior");
		}

		if (refuge == AnimalRefugeStrategyType.Trees && movement != AnimalMovementStrategyType.Arboreal)
		{
			return (false, "tree refuge requires arboreal movement");
		}

		if (refuge == AnimalRefugeStrategyType.Sky && movement != AnimalMovementStrategyType.Fly)
		{
			return (false, "sky refuge requires flying movement");
		}

		if (activity == AnimalActivityStrategyType.Custom && !activeTimes.Any())
		{
			return (false, "custom activity requires at least one active time of day");
		}

		if (water.In(AnimalWaterStrategyType.Immerse, AnimalWaterStrategyType.Surface) &&
		    !movement.In(AnimalMovementStrategyType.Swim, AnimalMovementStrategyType.Amphibious) &&
		    !hasWaterCellProg)
		{
			return (false, "immersion or surface water behavior requires swim, amphibious, or water-cell movement support");
		}

		if (ecologyNesting && home != AnimalHomeStrategyType.Denning && !hasNestSiteProg)
		{
			return (false, "nesting ecology requires denning home behavior or a nest-site prog");
		}

		if (ecologyParenting && awareness != AnimalAwarenessStrategyType.Guarding && !hasProtectProg)
		{
			return (false, "parenting ecology requires guarding awareness or a protect prog");
		}

		return (true, string.Empty);
	}

	private (bool Ready, string Reason) GetReadiness()
	{
		var hunting = HuntingReadiness();
		if (!hunting.Ready) return hunting;
		return ValidateConfiguration(HomeStrategy, FeedingStrategy, ThreatStrategy, MovementStrategy,
			RefugeStrategy, ActivityStrategy, _activeTimesOfDay, WaterStrategy,
			!ReferenceEquals(AmphibiousWaterCellProg, Gameworld.AlwaysFalseProg),
			AwarenessStrategy, EcologyNestingEnabled, !ReferenceEquals(NestSiteProg, Gameworld.AlwaysFalseProg),
			EcologyParentingEnabled, !ReferenceEquals(ProtectProg, Gameworld.AlwaysFalseProg));
	}

	private static IEnumerable<TimeOfDay> DefaultActiveTimesFor(AnimalActivityStrategyType strategy)
	{
		return strategy switch
		{
			AnimalActivityStrategyType.Diurnal => new[] { TimeOfDay.Dawn, TimeOfDay.Morning, TimeOfDay.Afternoon },
			AnimalActivityStrategyType.Nocturnal => new[] { TimeOfDay.Dusk, TimeOfDay.Night },
			AnimalActivityStrategyType.Crepuscular => new[] { TimeOfDay.Dawn, TimeOfDay.Dusk },
			_ => Enum.GetValues<TimeOfDay>()
		};
	}

	private void LoadActiveTimes(XElement activity)
	{
		_activeTimesOfDay.Clear();
		foreach (TimeOfDay time in activity.Elements("ActiveTime")
		                                   .Select(x => x.Value)
		                                   .Where(x => Enum.TryParse(x, true, out TimeOfDay _))
		                                   .Select(x => Enum.Parse<TimeOfDay>(x, true))
		                                   .Distinct())
		{
			_activeTimesOfDay.Add(time);
		}

		if (_activeTimesOfDay.Any())
		{
			return;
		}

		_activeTimesOfDay.AddRange(DefaultActiveTimesFor(ActivityStrategy));
	}

	private IAnimalFeedingStrategy FeedingStrategyHandler => FeedingStrategy switch
	{
		AnimalFeedingStrategyType.Predator => PredatorFeedingStrategy.Instance,
		AnimalFeedingStrategyType.DenPredator => DenPredatorFeedingStrategy.Instance,
		AnimalFeedingStrategyType.Forager => ForagerFeedingStrategy.Instance,
		AnimalFeedingStrategyType.Scavenger => ScavengerFeedingStrategy.Instance,
		AnimalFeedingStrategyType.Opportunist => OpportunistFeedingStrategy.Instance,
		AnimalFeedingStrategyType.Omnivore => OmnivoreFeedingStrategy.Instance,
		AnimalFeedingStrategyType.DenOmnivore => DenOmnivoreFeedingStrategy.Instance,
		_ => NoFeedingStrategy.Instance
	};

	private IAnimalWaterStrategy WaterStrategyHandler => WaterStrategy switch
	{
		AnimalWaterStrategyType.Drink => DrinkWaterStrategy.Instance,
		AnimalWaterStrategyType.Immerse => ImmersionWaterStrategy.Instance,
		AnimalWaterStrategyType.Surface => SurfaceWaterStrategy.Instance,
		_ => DisabledWaterStrategy.Instance
	};

	private IAnimalThreatStrategy ThreatStrategyHandler => ThreatStrategy switch
	{
		AnimalThreatStrategyType.Flee => FleeThreatStrategy.Instance,
		AnimalThreatStrategyType.Defend => DefendThreatStrategy.Instance,
		AnimalThreatStrategyType.HungryPredator => HungryPredatorThreatStrategy.Instance,
		_ => PassiveThreatStrategy.Instance
	};

	private IAnimalActivityStrategy ActivityStrategyHandler => ActivityStrategy == AnimalActivityStrategyType.Always
		? AlwaysActivityStrategy.Instance
		: TimedActivityStrategy.Instance;

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
		sb.AppendLine(ShowHunting(actor));
		(bool ready, string reason) = GetReadiness();
		sb.AppendLine($"Ready: {ready.ToColouredString()}{(ready ? string.Empty : $" - {reason.ColourError()}")}");
		sb.AppendLine();
		sb.AppendLine("Animal Strategies".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Movement: {MovementStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Movement Range: {MovementRange.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Amphibious Water Bias: {AmphibiousWaterBias.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Movement Enabled Prog: {MovementEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Movement Cell Prog: {MovementCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Preferred Habitat Prog: {PreferredHabitatProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Tolerated Habitat Prog: {ToleratedHabitatProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Amphibious Land Cell Prog: {AmphibiousLandCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Amphibious Water Cell Prog: {AmphibiousWaterCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Wander Chance: {WanderChancePerMinute.ToString("P2", actor).ColourValue()} per minute");
		sb.AppendLine($"Wander Emote: {WanderEmote.ColourCommand()}");
		sb.AppendLine($"Flying Layer: {TargetFlyingLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Resting Layer: {TargetRestingLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Preferred Tree Layer: {PreferredTreeLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Secondary Tree Layer: {SecondaryTreeLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Allow Descent Prog: {AllowDescentProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine($"Home: {HomeStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Territory Prog: {SuitableTerritoryProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Territory Size Prog: {DesiredTerritorySizeProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Share Territory: {WillShareTerritory.ToColouredString()}");
		sb.AppendLine($"Share With Other Races: {WillShareTerritoryWithOtherRaces.ToColouredString()}");
		sb.AppendLine($"Share Shelter With Group: {AllowGroupShelterSharing.ToColouredString()}");
		sb.AppendLine($"Burrow Craft: {BurrowCraft?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Burrow Site Prog: {BurrowSiteProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Build Enabled Prog: {BuildEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Home Location Prog: {HomeLocationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Anchor Item Prog: {AnchorItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine($"Feeding: {FeedingStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Water: {WaterStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Threat: {ThreatStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Attack Prog: {WillAttackProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Active Needs: {UseActiveNeeds.ToColouredString()}");
		sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} milliseconds");
		sb.AppendLine($"Engage Emote: {EngageEmote.ColourCommand()}");
		sb.AppendLine($"Threat Responses: ordinary {OrdinaryThreatResponse.DescribeEnum().ColourName()}, hungry prey {HungryPreyResponse.DescribeEnum().ColourName()}, attacked {AttackedThreatResponse.DescribeEnum().ColourName()}, territory {TerritoryThreatResponse.DescribeEnum().ColourName()}, parenting {ParentingThreatResponse.DescribeEnum().ColourName()}, seasonal {SeasonalThreatResponse.DescribeEnum().ColourName()}");
		sb.AppendLine($"Posture Duration: {PostureDurationDiceExpression.ColourValue()}");
		sb.AppendLine($"Posture Emote: {PostureEmote.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine($"Awareness: {AwarenessStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Threat Filter Prog: {AwarenessThreatProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Avoid Cell Prog: {AwarenessAvoidCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Awareness Range: {AwarenessRange.ToString("N0", actor).ColourValue()} rooms");
		sb.AppendLine($"Threat Memory: {AwarenessMemoryMinutes.ToString("N0", actor).ColourValue()} minutes");
		sb.AppendLine($"Senses: {SensesStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine();
		sb.AppendLine($"Refuge: {RefugeStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Refuge Layer: {RefugeLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Refuge Cell Prog: {RefugeCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Refuge Return Delay: {RefugeReturnSeconds.ToString("N0", actor).ColourValue()} seconds");
		sb.AppendLine();
		sb.AppendLine($"Activity: {ActivityStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Active Times: {_activeTimesOfDay.Select(x => x.DescribeEnum().ColourName()).ListToString()}");
		sb.AppendLine($"Sleep When Inactive: {ActivitySleepEnabled.ToColouredString()}");
		sb.AppendLine($"Dormancy Mode: {DormancyMode.DescribeEnum().ColourName()}");
		sb.AppendLine($"Rest Emote: {ActivityRestEmote.ColourCommand()}");
		sb.AppendLine($"Dormant Season Groups: {_dormantSeasonGroups.Select(x => x.ColourName()).ListToString()}");
		sb.AppendLine($"Aggressive Season Groups: {_aggressiveSeasonGroups.Select(x => x.ColourName()).ListToString()}");
		sb.AppendLine($"Nesting Season Groups: {_nestingSeasonGroups.Select(x => x.ColourName()).ListToString()}");
		sb.AppendLine();
		sb.AppendLine($"Ecology Shelter: {EcologyShelterEnabled.ToColouredString()}");
		sb.AppendLine($"Ecology Seasonal: {EcologySeasonalEnabled.ToColouredString()}");
		sb.AppendLine($"Ecology Nesting: {EcologyNestingEnabled.ToColouredString()}");
		sb.AppendLine($"Ecology Parenting: {EcologyParentingEnabled.ToColouredString()}");
		sb.AppendLine($"Shelter Needed Prog: {ShelterNeededProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Shelter Cell Prog: {ShelterCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Seasonal Cell Prog: {SeasonalCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Seasonal Habitat Progs: {_seasonalHabitatProgs.OrderBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase).Select(x => $"{x.Key.ColourName()}: {x.Value.MXPClickableFunctionName()}").ListToString()}");
		sb.AppendLine($"Nest Site Prog: {NestSiteProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Protect Prog: {ProtectProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	/// <summary>
	/// Produces the small amount of per-instance state that is useful when diagnosing live wildlife.
	/// Builder <see cref="Show"/> describes the shared definition; this reports the current animal,
	/// season and group-control context without persisting any diagnostic state.
	/// </summary>
	public string DebugSummary(ICharacter character)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Animal AI #{Id.ToStringN0(character)} ({Name.ColourName()}):");
		sb.AppendLine($"\tStatus: {(IsActivityInactive(character) ? "inactive".Colour(Telnet.BoldYellow) : "active".Colour(Telnet.Green))}");
		
		var season = character.Location?.CurrentSeason(character)?.SeasonGroup ?? "unknown";
		var dormantForSeason = IsSeasonIn(_dormantSeasonGroups, character);
		var restingForTimeOfDay = !ActivityStrategyHandler.IsActive(this, character);
		var activityReason = dormantForSeason
			? $"seasonal dormancy ({season})"
			: restingForTimeOfDay
				? "rest period"
				: "active period";
		sb.AppendLine($"\tActivity: {activityReason.ColourValue()}");
		sb.AppendLine($"\tSeason: {season.ColourValue()}");
		sb.AppendLine($"\tDormancy: {DormancyMode.DescribeEnum().ColourValue()}");


		var habitat = character.Location is null
			? "unknown".Colour(Telnet.Magenta)
			: IsWithinPreferredHabitat(character, character.Location)
				? "preferred".Colour(Telnet.Green)
				: IsWithinToleratedHabitat(character, character.Location)
					? "tolerated transit".Colour(Telnet.Yellow)
					: "forbidden".Colour(Telnet.Red);
		sb.AppendLine($"\tHabitat: {habitat}");

		var groupControl = character is INPC npc &&
		                   npc.GroupAI?.GroupAIType is IGroupAIControlPolicy policy
			? policy.ControlScope.GetSingleFlags().ListToColouredString()
			: "None".ColourValue();
		sb.AppendLine($"\tSurvival Needs: {(SurvivalNeedsSatisfied(character) ? "satisfied".ColourValue() : "urgent".ColourError())}");
		sb.AppendLine($"\tGroup Control: {groupControl}");

		return sb.ToString();
	}

	/// <summary>
	/// Returns whether this animal's own activity policy requires rest. Group controllers use this
	/// to avoid waking a satiated, seasonally dormant or off-period animal merely because the
	/// group controls activity.
	/// </summary>
	public bool IsActivityRestRequired(ICharacter character)
	{
		return IsActivityInactive(character) && SurvivalNeedsSatisfied(character);
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3movement ground|swim|fly|arboreal|amphibious#0 - sets the movement strategy
	#3movement range <number>#0 - sets the path search range
	#3movement waterbias <0-100>#0 - sets amphibious ambient water preference
	#3movement chance <%>#0 - sets the ambient movement chance per minute
	#3movement enabled <prog>#0 - sets whether ambient movement is enabled
	#3movement room <prog>#0 - sets which cells can be ambient movement targets
	#3movement preferredhabitat <prog>#0 - sets habitats preferred for ambient destinations
	#3movement toleratedhabitat <prog>#0 - sets habitats allowed for all animal routing
	#3movement landprog <prog>#0 - sets amphibious land cells
	#3movement waterprog <prog>#0 - sets amphibious water cells
	#3movement flying <layer>#0 - sets the flying travel layer
	#3movement resting <layer>#0 - sets the final/resting layer for flyers
	#3movement preferred <layer>#0 - sets the preferred tree layer
	#3movement secondary <layer>#0 - sets the fallback tree layer
	#3movement descent <prog>#0 - sets when arboreal movement may descend
	#3movement emote <text|clear>#0 - sets the movement emote
	#3home none|territorial|denning#0 - sets home behavior
	#3home territory <prog>#0 - sets suitable territory cells
	#3home size <prog>#0 - sets desired territory size
	#3home share#0 - toggles sharing territory with same-race NPCs
	#3home shareother#0 - toggles sharing territory with other races
	#3home shareshelter#0 - toggles same-live-group sharing of claimed wildlife shelters
	#3home craft <craft|clear>#0 - sets the optional burrow craft
	#3home site <prog>#0 - sets suitable burrow cells
	#3home location <prog|clear>#0 - sets fallback home location
	#3home enabled <prog>#0 - sets whether burrow building is active
	#3home anchor <prog|clear>#0 - sets burrow anchor detection
	#3feeding none|predator|denpredator|forager|scavenger|opportunist|omnivore|denomnivore#0 - sets feeding behavior
	#3feeding needs active|legacy#0 - toggles active hunger and thirst for simple NPCs using this AI
	#3feeding attackprog <prog>#0 - sets predator target selection
	#3feeding delay <dice>#0 - sets predator attack delay
	#3feeding emote <text|clear>#0 - sets predator engage emote
	#3water off|drink|immerse|surface#0 - sets thirst and water-memory behavior
	#3threat passive|flee|defend|hungrypredator#0 - sets legacy threat behavior
	#3threat response <context> <response>#0 - sets ordinary, hungry-prey, attacked, territory, parenting or seasonal response
	#3threat posture <text|clear>#0 - sets the emote used before posture escalation
	#3threat duration <dice>#0 - sets posture duration in seconds
	#3awareness none|wary|wimpy|skittish|guarding#0 - sets non-combat awareness behavior
	#3awareness threat <prog>#0 - sets the character filter for disliked or feared targets
	#3awareness avoid <prog>#0 - sets the cell filter for places this animal avoids
	#3awareness range <rooms>#0 - sets how far the animal notices threats
	#3awareness memory <minutes>#0 - sets how long threat locations are remembered
	#3awareness senses none|vigilant|hiding|stalking|tracking#0 - adds animal-specific senses behavior
	#3refuge none|home|den|trees|sky|water|prog#0 - sets where the animal retreats or rests
	#3refuge layer <layer>#0 - sets the refuge layer for trees or sky
	#3refuge cell <prog>#0 - sets the refuge-cell selector for prog refuge
	#3refuge return <seconds>#0 - sets the return delay after refuge work
#3activity always|diurnal|nocturnal|crepuscular|custom#0 - sets active periods
#3activity active <timeofday...>#0 - sets active times for custom activity
#3activity sleep on|off#0 - toggles sleeping while inactive at refuge
#3activity dormancy rest|hibernation|torpor#0 - selects the seasonal dormant-state policy
	#3activity restemote <text|clear>#0 - sets an optional rest emote
	#3activity dormantseason <season group|clear>#0 - toggles hibernation / torpor for a hemisphere-aware season group
	#3activity aggressiveseason <season group|clear>#0 - toggles an aggressive season group
	#3activity nestingseason <season group|clear>#0 - toggles the hemisphere-aware nesting season group
	#3ecology shelter|seasonal|nesting|parenting on|off#0 - toggles ecology behaviors
	#3ecology shelterneeded <prog>#0 - sets when shelter is required
	#3ecology sheltercell <prog>#0 - sets valid shelter cells
	#3ecology seasonalcell <prog>#0 - sets valid seasonal range cells
	#3ecology seasonalhabitat <season group> <prog|clear>#0 - sets or clears a season-specific preferred habitat
	#3ecology nestsite <prog>#0 - sets valid nest cells
	#3ecology protect <prog>#0 - sets protected young or friends
	#3hunting <on|off|opening|followup|layer|opportunity|range|timeout|lost> <value>#0 - configures hunting tactics
	#3prey <people|selection|include|exclude|prefer|sizes|eligibility|classification|preference> <value>#0 - configures prey policy
	#3assessment <cautious|balanced|bold|engage|abandon|starvation|confidence|weight> <value>#0 - configures observable risk assessment";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "hunting":
			case "prey":
			case "assessment":
				return BuildingCommandHunting(actor, command.GetUndo());
			case "movement":
				return BuildingCommandMovement(actor, command);
			case "home":
				return BuildingCommandHome(actor, command);
			case "feeding":
			case "food":
				return BuildingCommandFeeding(actor, command);
			case "water":
			case "thirst":
				return BuildingCommandWater(actor, command);
			case "threat":
				return BuildingCommandThreat(actor, command);
			case "awareness":
				return BuildingCommandAwareness(actor, command);
			case "refuge":
				return BuildingCommandRefuge(actor, command);
			case "activity":
				return BuildingCommandActivity(actor, command);
			case "ecology":
				return BuildingCommandEcology(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandFeeding(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.None);
			case "predator":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.Predator);
			case "denpredator":
			case "den-predator":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.DenPredator);
			case "forager":
			case "grazer":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.Forager);
			case "scavenger":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.Scavenger);
			case "opportunist":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.Opportunist);
			case "omnivore":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.Omnivore);
			case "denomnivore":
			case "den-omnivore":
				return SetFeedingStrategy(actor, AnimalFeedingStrategyType.DenOmnivore);
			case "attackprog":
			case "attack":
				return BuildingCommandAttackProg(actor, command);
			case "delay":
			case "engagedelay":
				return BuildingCommandEngageDelay(actor, command);
			case "emote":
			case "engageemote":
				return BuildingCommandEngageEmote(actor, command);
			case "needs":
			case "activeneeds":
				return BuildingCommandFeedingNeeds(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool SetFeedingStrategy(ICharacter actor, AnimalFeedingStrategyType strategy)
	{
		FeedingStrategy = strategy;
		if (strategy.In(AnimalFeedingStrategyType.Predator, AnimalFeedingStrategyType.DenPredator,
			    AnimalFeedingStrategyType.Omnivore, AnimalFeedingStrategyType.DenOmnivore) &&
		    ThreatStrategy == AnimalThreatStrategyType.Passive)
		{
			ThreatStrategy = AnimalThreatStrategyType.HungryPredator;
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {strategy.DescribeEnum().ColourName()} feeding behavior.");
		return true;
	}

	private bool BuildingCommandAttackProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control predator or defensive target selection?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillAttackProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for target selection.");
		return true;
	}

	private bool BuildingCommandWater(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			WaterStrategy = WaterStrategy == AnimalWaterStrategyType.Off
				? AnimalWaterStrategyType.Drink
				: AnimalWaterStrategyType.Off;
		}
		else
		{
			switch (command.PopForSwitch())
			{
				case "on":
				case "yes":
				case "true":
				case "drink":
				case "drinking":
					WaterStrategy = AnimalWaterStrategyType.Drink;
					break;
				case "off":
				case "no":
				case "false":
					WaterStrategy = AnimalWaterStrategyType.Off;
					break;
				case "immerse":
				case "immersion":
				case "absorb":
					WaterStrategy = AnimalWaterStrategyType.Immerse;
					break;
				case "surface":
				case "surfacing":
					WaterStrategy = AnimalWaterStrategyType.Surface;
					break;
				default:
					actor.OutputHandler.Send("You must specify #3off#0, #3drink#0, #3immerse#0, or #3surface#0.".SubstituteANSIColour());
					return false;
			}
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {WaterStrategy.DescribeEnum().ColourName()} water behavior.");
		return true;
	}

	private bool BuildingCommandThreat(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify passive, flee, defend, or hungrypredator.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "response":
				return BuildingCommandThreatResponse(actor, command);
			case "posture":
				return BuildingCommandThreatPostureEmote(actor, command);
			case "duration":
			case "postureduration":
				return BuildingCommandThreatPostureDuration(actor, command);
			case "passive":
				ThreatStrategy = AnimalThreatStrategyType.Passive;
				break;
			case "flee":
				ThreatStrategy = AnimalThreatStrategyType.Flee;
				break;
			case "defend":
			case "territorial":
				ThreatStrategy = AnimalThreatStrategyType.Defend;
				break;
			case "hungrypredator":
			case "hungry":
			case "predator":
				ThreatStrategy = AnimalThreatStrategyType.HungryPredator;
				if (FeedingStrategy == AnimalFeedingStrategyType.None)
				{
					FeedingStrategy = AnimalFeedingStrategyType.Predator;
				}
				break;
			default:
				actor.OutputHandler.Send("You must specify passive, flee, defend, or hungrypredator.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {ThreatStrategy.DescribeEnum().ColourName()} threat behavior.");
		return true;
	}

	private bool BuildingCommandActivity(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "always":
				return SetActivityStrategy(actor, AnimalActivityStrategyType.Always);
			case "diurnal":
			case "day":
				return SetActivityStrategy(actor, AnimalActivityStrategyType.Diurnal);
			case "nocturnal":
			case "night":
				return SetActivityStrategy(actor, AnimalActivityStrategyType.Nocturnal);
			case "crepuscular":
			case "twilight":
				return SetActivityStrategy(actor, AnimalActivityStrategyType.Crepuscular);
			case "custom":
				return SetActivityStrategy(actor, AnimalActivityStrategyType.Custom);
			case "active":
			case "times":
				return BuildingCommandActivityActive(actor, command);
			case "sleep":
				return BuildingCommandActivitySleep(actor, command);
			case "dormancy":
			case "dormancymode":
				return BuildingCommandActivityDormancy(actor, command);
			case "restemote":
			case "emote":
				return BuildingCommandActivityRestEmote(actor, command);
			case "dormantseason":
			case "hibernate":
			case "torpor":
				return BuildingCommandActivitySeasonGroup(actor, command, _dormantSeasonGroups,
					"dormant / hibernation");
			case "aggressiveseason":
			case "aggressionseason":
				return BuildingCommandActivitySeasonGroup(actor, command, _aggressiveSeasonGroups,
					"aggressive");
			case "nestingseason":
			case "nestseason":
				return BuildingCommandActivitySeasonGroup(actor, command, _nestingSeasonGroups,
					"nesting");
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool SetActivityStrategy(ICharacter actor, AnimalActivityStrategyType strategy)
	{
		ActivityStrategy = strategy;
		_activeTimesOfDay.Clear();
		_activeTimesOfDay.AddRange(DefaultActiveTimesFor(strategy));
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {strategy.DescribeEnum().ColourName()} activity behavior.");
		return true;
	}

	private bool BuildingCommandActivityActive(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify one or more active times of day. Valid values are {Enum.GetValues<TimeOfDay>().ListToColouredString()}; you may also use #3all#0."
				.SubstituteANSIColour());
			return false;
		}

		List<TimeOfDay> times = new();
		while (!command.IsFinished)
		{
			string token = command.PopSpeech();
			switch (token.ToLowerInvariant())
			{
				case "all":
				case "always":
					times.Clear();
					times.AddRange(Enum.GetValues<TimeOfDay>());
					break;
				case "day":
				case "diurnal":
					times.AddRange(DefaultActiveTimesFor(AnimalActivityStrategyType.Diurnal));
					break;
				case "night":
				case "nocturnal":
					times.AddRange(DefaultActiveTimesFor(AnimalActivityStrategyType.Nocturnal));
					break;
				case "twilight":
				case "crepuscular":
					times.AddRange(DefaultActiveTimesFor(AnimalActivityStrategyType.Crepuscular));
					break;
				default:
					if (!token.TryParseEnum(out TimeOfDay time))
					{
						actor.OutputHandler.Send($"The text {token.ColourCommand()} is not a valid time of day. Valid values are {Enum.GetValues<TimeOfDay>().ListToColouredString()}.");
						return false;
					}

					times.Add(time);
					break;
			}
		}

		ActivityStrategy = AnimalActivityStrategyType.Custom;
		_activeTimesOfDay.Clear();
		_activeTimesOfDay.AddRange(times.Distinct());
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will be active during {_activeTimesOfDay.Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
		return true;
	}

	private bool BuildingCommandActivitySleep(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			ActivitySleepEnabled = !ActivitySleepEnabled;
		}
		else
		{
			switch (command.PopForSwitch())
			{
				case "on":
				case "yes":
				case "true":
					ActivitySleepEnabled = true;
					break;
				case "off":
				case "no":
				case "false":
					ActivitySleepEnabled = false;
					break;
				default:
					actor.OutputHandler.Send("You must specify either #3on#0 or #3off#0.".SubstituteANSIColour());
					return false;
			}
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will {ActivitySleepEnabled.NowNoLonger()} sleep while inactive at refuge.");
		return true;
	}

	private bool BuildingCommandFeedingNeeds(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify #3active#0 or #3legacy#0 needs behavior.".SubstituteANSIColour());
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "active":
			case "on":
			case "yes":
			case "true":
				UseActiveNeeds = true;
				break;
			case "legacy":
			case "off":
			case "no":
			case "false":
				UseActiveNeeds = false;
				break;
			default:
				actor.OutputHandler.Send("You must specify #3active#0 or #3legacy#0 needs behavior.".SubstituteANSIColour());
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will {(UseActiveNeeds ? "use" : "retain legacy")} needs behavior for simple NPCs.");
		return true;
	}

	private bool BuildingCommandActivityDormancy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out AnimalDormancyMode mode))
		{
			actor.OutputHandler.Send($"You must specify a dormancy mode. Valid values are {Enum.GetValues<AnimalDormancyMode>().ListToColouredString()}.");
			return false;
		}

		DormancyMode = mode;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {mode.DescribeEnum().ColourName()} while a configured dormant season is active.");
		return true;
	}

	private bool BuildingCommandActivityRestEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either supply an emote or use #3clear#0 to remove the emote."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			ActivityRestEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This animal AI will no longer use an inactive rest emote.");
			return true;
		}

		Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		ActivityRestEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use this inactive rest emote:\n{ActivityRestEmote.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandEcology(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "shelter":
				return BuildingCommandEcologyToggle(actor, command, value => EcologyShelterEnabled = value, "shelter");
			case "seasonal":
			case "season":
				return BuildingCommandEcologyToggle(actor, command, value => EcologySeasonalEnabled = value, "seasonal range");
			case "nesting":
			case "nest":
				return BuildingCommandEcologyToggle(actor, command, value => EcologyNestingEnabled = value, "nesting");
			case "parenting":
			case "parent":
				return BuildingCommandEcologyToggle(actor, command, value => EcologyParentingEnabled = value, "parenting");
			case "shelterneeded":
			case "needsshelter":
				return BuildingCommandEcologyProg(actor, command, value => ShelterNeededProg = value,
					"when shelter is needed", ProgVariableTypes.Boolean,
					new[] { ProgVariableTypes.Character });
			case "sheltercell":
			case "shelterprog":
				return BuildingCommandEcologyCellProg(actor, command, value => ShelterCellProg = value, "shelter cells");
			case "seasonalcell":
			case "seasonalprog":
			case "seasoncell":
				return BuildingCommandEcologyCellProg(actor, command, value => SeasonalCellProg = value, "seasonal range cells");
			case "seasonalhabitat":
			case "seasonhabitat":
				return BuildingCommandEcologySeasonalHabitat(actor, command);
			case "nestsite":
			case "nestprog":
				return BuildingCommandEcologyCellProg(actor, command, value => NestSiteProg = value, "nest sites");
			case "protect":
			case "protectprog":
				return BuildingCommandEcologyProg(actor, command, value => ProtectProg = value,
					"protected young or friends", ProgVariableTypes.Boolean,
					new[] { ProgVariableTypes.Character, ProgVariableTypes.Character });
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandEcologyToggle(ICharacter actor, StringStack command, Action<bool> setter, string label)
	{
		bool value;
		if (command.IsFinished)
		{
			value = true;
		}
		else
		{
			switch (command.PopForSwitch())
			{
				case "on":
				case "yes":
				case "true":
					value = true;
					break;
				case "off":
				case "no":
				case "false":
					value = false;
					break;
				default:
					actor.OutputHandler.Send("You must specify either #3on#0 or #3off#0.".SubstituteANSIColour());
					return false;
			}
		}

		setter(value);
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will {value.NowNoLonger()} use {label} ecology.");
		return true;
	}

	private bool BuildingCommandEcologyCellProg(ICharacter actor, StringStack command, Action<IFutureProg> setter, string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should identify {label}?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to identify {label}.");
		return true;
	}

	private bool BuildingCommandEcologySeasonalHabitat(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a quoted season group and a habitat prog, or use #3clear#0 to remove every seasonal habitat preference.".SubstituteANSIColour());
			return false;
		}

		string seasonGroup = command.PopSpeech();
		if (seasonGroup.EqualToAny("clear", "none", "remove", "delete"))
		{
			_seasonalHabitatProgs.Clear();
			Changed = true;
			actor.OutputHandler.Send("This animal AI no longer has any season-specific habitat preferences.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a habitat prog, or #3clear#0 to remove this season group's preference.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			if (_seasonalHabitatProgs.Remove(seasonGroup))
			{
				Changed = true;
				actor.OutputHandler.Send($"This animal AI no longer has a seasonal habitat preference for {seasonGroup.ColourValue()}.");
			}
			else
			{
				actor.OutputHandler.Send($"This animal AI did not have a seasonal habitat preference for {seasonGroup.ColourValue()}.");
			}

			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_seasonalHabitatProgs[seasonGroup] = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} as its preferred habitat during {seasonGroup.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEcologyProg(ICharacter actor, StringStack command, Action<IFutureProg> setter,
		string label, ProgVariableTypes returnType, IEnumerable<ProgVariableTypes> parameters)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should identify {label}?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			returnType, parameters).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for {label}.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		ICharacter? ch = CharacterForEvent(type, arguments);
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		var activeHunt = ch.EffectsOfType<AnimalHuntEffect>().FirstOrDefault(x => x.AiId == Id);
		if (!Hunting.Enabled && activeHunt is not null)
		{
			ch.RemoveEffect(activeHunt);
			activeHunt = null;
		}
		if (activeHunt is not null && type.In(EventType.TenSecondTick, EventType.CharacterEnterCellWitness) &&
		    RespondToHuntThreat(ch, activeHunt)) return true;

		if (Hunting.Enabled && type.In(EventType.TenSecondTick, EventType.MinuteTick, EventType.CharacterEnterCellWitness,
			EventType.LeaveCombat, EventType.NPCOnGameLoadFinished, EventType.CharacterStopMovement,
			EventType.CharacterEnterCellFinish, EventType.TrapCaughtPrey) && TickHunt(ch))
		{
			return true;
		}
		if (activeHunt is not null && type.In(EventType.FiveSecondTick, EventType.LayerChangeBlockExpired,
			EventType.CommandDelayExpired, EventType.CharacterStopMovementClosedDoor)) return true;

		if (type == EventType.CharacterDiesWitness)
		{
			FeedingStrategyHandler.HandleWitnessedDeath(this, ch, (ICharacter)arguments[0]);
			return false;
		}

		if (type.In(EventType.EngagedInCombat, EventType.EngageInCombat))
		{
			HandleCombatAwareness(ch);
			return false;
		}

		switch (type)
		{
			case EventType.CharacterEntersGame:
			case EventType.NPCOnGameLoadFinished:
			case EventType.CharacterEnterCellFinish:
			case EventType.CharacterStopMovement:
			case EventType.CharacterStopMovementClosedDoor:
			case EventType.LeaveCombat:
			case EventType.TenSecondTick:
				if (type == EventType.TenSecondTick && !IsGroupControlled(ch, GroupAIControlScope.Senses))
				{
					AcquireRangedTargets(ch);
				}

				if (TryAwarenessResponse(ch, null))
				{
					return true;
				}

				if (type == EventType.TenSecondTick && EvaluateSenses(ch))
				{
					return true;
				}

				if (EvaluateImmediateNeedsAndFeeding(ch))
				{
					return true;
				}

				if (type == EventType.TenSecondTick && TryThreatResponse(ch, null))
				{
					return true;
				}

				if (ShouldRemainAtRest(ch))
				{
					// Returning true is important here: PathingAIBase otherwise starts ambient
					// pathing for NPCOnGameLoadFinished, movement completion and similar events.
					return true;
				}

				if (EvaluateEcology(ch))
				{
					return true;
				}

				if (type != EventType.TenSecondTick)
				{
					CheckPathingEffect(ch, true);
				}
				break;
			case EventType.MinuteTick:
				if (TryAwarenessResponse(ch, null))
				{
					return true;
				}

				if (EvaluateImmediateNeedsAndFeeding(ch))
				{
					return true;
				}

				if (ShouldRemainAtRest(ch))
				{
					// EvaluateActivity may put an animal to sleep at an already-authored refuge,
					// but it never allows the pathing base to start movement or territory work.
					_ = EvaluateActivity(ch);
					return true;
				}

				if (EvaluateEcology(ch) || EvaluateActivity(ch))
				{
					return true;
				}

				EvaluateHomeAndTerritory(ch);
				CheckPathingEffect(ch, true);
				break;
			case EventType.CharacterEnterCellWitness:
				if (TryAwarenessResponse(ch, (ICharacter)arguments[0]))
				{
					return true;
				}

				if (EvaluateImmediateNeedsAndFeeding(ch))
				{
					return true;
				}

				if (TryThreatResponse(ch, (ICharacter)arguments[0]))
				{
					return true;
				}

				if (ShouldRemainAtRest(ch))
				{
					return true;
				}

				return EvaluateEcology(ch);
			case EventType.FiveSecondTick:
			case EventType.CommandDelayExpired:
				if (ShouldRemainAtRest(ch))
				{
					return true;
				}

				break;
			case EventType.LayerChangeBlockExpired:
				if (ShouldRemainAtRest(ch))
				{
					return true;
				}

				CheckPathingEffect(ch, false);
				break;
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (EventType type in types)
		{
			switch (type)
			{
				case EventType.CharacterEntersGame:
				case EventType.TrapCaughtPrey:
				case EventType.NPCOnGameLoadFinished:
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterEnterCellWitness:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.CharacterCannotMove:
				case EventType.CharacterDiesWitness:
				case EventType.EngagedInCombat:
				case EventType.EngageInCombat:
				case EventType.LeaveCombat:
				case EventType.FiveSecondTick:
				case EventType.TenSecondTick:
				case EventType.MinuteTick:
				case EventType.LayerChangeBlockExpired:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	private bool EvaluateImmediateNeedsAndFeeding(ICharacter character)
	{
		if (character.Movement is not null || character.Combat is not null)
		{
			return false;
		}

		// Hunger and thirst are explicitly allowed to interrupt inactivity. Wake before asking the
		// feeding strategies to path or scan; sleeping characters cannot perceive their food or
		// water opportunities even when their policy has correctly left the rest gate open.
		if (!SurvivalNeedsSatisfied(character) && character.State.IsAsleep())
		{
			character.Awaken();
		}

		if (!IsGroupControlled(character, GroupAIControlScope.Feeding) &&
		    WaterStrategyHandler.TrySatisfyImmediateNeed(this, character))
		{
			return true;
		}

		if (!IsGroupControlled(character, GroupAIControlScope.Feeding) && WaterStrategyHandler.IsThirsty(this, character))
		{
			return false;
		}

		if (!IsGroupControlled(character, GroupAIControlScope.Feeding) &&
		    FeedingStrategyHandler.TrySatisfyImmediateNeed(this, character))
		{
			return true;
		}

		if (!IsGroupControlled(character, GroupAIControlScope.Shelter) &&
		    ShouldReturnToRefuge(character) && TryMoveToRefugeLayer(character))
		{
			return true;
		}

		// Den construction and other idle home maintenance are optional long-term behaviour. They
		// must not wake a satiated nocturnal, diurnal or seasonally dormant animal after the
		// immediate needs / refuge work above has finished.
		if (!IsGroupControlled(character, GroupAIControlScope.Shelter) &&
		    SurvivalNeedsSatisfied(character) &&
		    !IsActivityInactive(character))
		{
			HomeStrategyHandler.EvaluateIdle(this, character);
		}

		return false;
	}

	private bool IsHungry(ICharacter character)
	{
		return FeedingStrategyHandler.IsHungry(this, character);
	}

	protected override bool SurvivalNeedsSatisfied(ICharacter character)
	{
		return !FeedingStrategyHandler.IsHungry(this, character) &&
		       !WaterStrategyHandler.IsThirsty(this, character);
	}

	internal static bool CanGroupHuntTarget(ICharacter character, ICharacter target)
	{
		return character is INPC npc &&
		       npc.AIs.OfType<AnimalAI>().Any(ai => ai.CanHuntTarget(character, target));
	}

	internal static bool CanGroupObserveTarget(ICharacter character, ICharacter target)
	{
		return character is INPC npc &&
		       npc.AIs.OfType<AnimalAI>().Any(ai => ai.CanObserveTarget(character, target));
	}

	internal override bool CanHuntTarget(ICharacter character, ICharacter target)
	{
		if (Hunting.Enabled)
		{
			return PreyRejection(character, target) is null &&
			       AssessPrey(character, target).Score >= HuntThreshold(character, false);
		}
		if (target.Location is null || IsSociallyTrusted(character, target) ||
			!MovementStrategyHandler.CanReachTargetLayer(this, character, target.RoomLayer) ||
			!PredatorAIHelpers.WillAttack(character, target, WillAttackProg, true))
		{
			return false;
		}

		if (IsLocalHuntTarget(character, target))
		{
			return character.CanSee(target);
		}

		return ScanTargetAcquisition.IsCurrentVisibleRangedTarget(character, target, EffectiveScanRange(character));
	}

	internal bool CanHuntLocalTarget(ICharacter character, ICharacter target)
	{
		return IsLocalHuntTarget(character, target) && CanHuntTarget(character, target);
	}

	protected override bool IsSociallyTrusted(ICharacter character, ICharacter target)
	{
		if (ReferenceEquals(character, target) || character.Race.SameRace(target.Race))
		{
			return true;
		}

		return character is INPC npc &&
		       npc.GroupAI?.GroupMembers.ContainsPhysicalInstance(target) == true;
	}

	protected override bool IsWithinPreferredHabitat(ICharacter character, ICell cell)
	{
		IFutureProg habitatProg = PreferredHabitatProg;
		string? seasonGroup = character.Location?.CurrentSeason(character)?.SeasonGroup;
		if (!string.IsNullOrWhiteSpace(seasonGroup) && _seasonalHabitatProgs.TryGetValue(seasonGroup, out IFutureProg? seasonalProg))
		{
			habitatProg = seasonalProg;
		}

		return habitatProg.ExecuteBool(false, character, cell, character.Location);
	}

	/// <summary>
	/// Territory selection predates the character-first cell-policy convention used by AnimalAI
	/// movement and ecology. Accept both contracts so old location-first definitions remain valid
	/// while finished wildlife profiles can reuse their habitat progs unchanged.
	/// </summary>
	private bool IsSeasonIn(IEnumerable<string> seasonGroups, ICharacter character)
	{
		string? seasonGroup = character.Location?.CurrentSeason(character)?.SeasonGroup;
		return !string.IsNullOrWhiteSpace(seasonGroup) &&
		       seasonGroups.Any(x => string.Equals(x, seasonGroup, StringComparison.InvariantCultureIgnoreCase));
	}

	private bool IsActivityInactive(ICharacter character)
	{
		return IsSeasonIn(_dormantSeasonGroups, character) ||
		       !ActivityStrategyHandler.IsActive(this, character);
	}

	/// <summary>
	/// Determines whether activity policy must suppress ambient pathing. Callers that process a
	/// direct threat do so before this check, preserving the allowed combat and immediate-threat
	/// interruptions to rest, hibernation and torpor.
	/// </summary>
	private bool ShouldRemainAtRest(ICharacter character)
	{
		return !IsGroupControlled(character, GroupAIControlScope.Activity) &&
		       IsActivityRestRequired(character);
	}

	private bool IsAggressiveSeason(ICharacter character)
	{
		return IsSeasonIn(_aggressiveSeasonGroups, character);
	}

	private bool IsNestingSeason(ICharacter character)
	{
		return !_nestingSeasonGroups.Any() || IsSeasonIn(_nestingSeasonGroups, character);
	}

	private bool EvaluateActivity(ICharacter character)
	{
		if (!SurvivalNeedsSatisfied(character) ||
		    IsActivityInactive(character) == false)
		{
			return false;
		}

		// An animal without an established den, territory or roost still rests in place.
		// Choosing or building long-term shelter belongs to active-period ecology; it must not
		// turn a diurnal, nocturnal or seasonally dormant animal into an ambient pathfinder.
		return IsAtRefuge(character) && TrySleepAtRefuge(character);
	}

	private bool EvaluateSenses(ICharacter character)
	{
		if (IsGroupControlled(character, GroupAIControlScope.Senses) ||
		    IsActivityInactive(character) ||
		    character.Combat is not null ||
		    character.Movement is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement")))
		{
			return false;
		}

		return EvaluateCreatureSenses(character);
	}

	private bool WouldTrackKnownPrey(ICharacter character)
	{
		return SensesStrategy == AnimalSensesStrategyType.Tracking &&
		       PredatorAIHelpers.IsHungry(character) &&
		       NpcKnownThreatLocationsEffect.Get(character)?.KnownThreatLocations(AwarenessMemory)
		       .Any(x => !ReferenceEquals(x, character.Location)) == true;
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetKnownPreyPath(ICharacter character)
	{
		IEnumerable<ICell> locations = NpcKnownThreatLocationsEffect.Get(character)
			?.KnownThreatLocations(AwarenessMemory)
			.Where(x => !ReferenceEquals(x, character.Location))
			?? Enumerable.Empty<ICell>();
		foreach (ICell location in locations)
		{
			List<ICellExit> path = character.PathBetween(location, (uint)MovementRange,
				GetAnimalSuitabilityFunction(character)).ToList();
			if (path.Any())
			{
				return (location, path);
			}
		}

		return (null, Enumerable.Empty<ICellExit>());
	}

	private bool EvaluateEcology(ICharacter character)
	{
		if (character.Combat is not null ||
		    character.Movement is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement")))
		{
			return false;
		}

		if (TryParentalGuard(character))
		{
			return true;
		}

		// Rest is authoritative. Shelter, nesting and seasonal range choices are deliberate
		// long-term behaviour and must not wake a satiated animal outside its active period.
		if (!IsGroupControlled(character, GroupAIControlScope.Activity) &&
		    IsActivityInactive(character) &&
		    SurvivalNeedsSatisfied(character))
		{
			return false;
		}

		if (!SurvivalNeedsSatisfied(character) || !EcologyWouldMove(character))
		{
			return false;
		}

		if (EcologyNestingEnabled && IsNestingSeason(character) && HomeStrategy == AnimalHomeStrategyType.Denning &&
		    IsAtNest(character))
		{
			EvaluateBurrowLifecycle(character);
			return true;
		}

		CheckPathingEffect(character, true);
		return true;
	}

	private bool EcologyWouldMove(ICharacter character)
	{
		if (!SurvivalNeedsSatisfied(character))
		{
			return false;
		}

		return EcologyShelterEnabled && ShelterNeededProg.ExecuteBool(false, character) && !IsAtEcologyCell(character, ShelterCellProg) ||
		       EcologySeasonalEnabled && !IsAtEcologyCell(character, SeasonalCellProg) ||
		       EcologyNestingEnabled && IsNestingSeason(character) && !IsAtNest(character);
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetEcologyPath(ICharacter character)
	{
		if (EcologyShelterEnabled &&
		    ShelterNeededProg.ExecuteBool(false, character) &&
		    !IsAtEcologyCell(character, ShelterCellProg))
		{
			return GetEcologyCellPath(character, ShelterCellProg);
		}

		if (EcologySeasonalEnabled && !IsAtEcologyCell(character, SeasonalCellProg))
		{
			return GetEcologyCellPath(character, SeasonalCellProg);
		}

		if (EcologyNestingEnabled && IsNestingSeason(character) && !IsAtNest(character))
		{
			return GetNestPath(character);
		}

		return (null, Enumerable.Empty<ICellExit>());
	}

	private bool IsAtEcologyCell(ICharacter character, IFutureProg cellProg)
	{
		return cellProg.ExecuteBool(false, character, character.Location, character.Location);
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetEcologyCellPath(ICharacter character, IFutureProg cellProg)
	{
		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
			x => x is ICell cell && cellProg.ExecuteBool(false, character, cell, character.Location),
			DefaultNeedRange,
			GetAnimalSuitabilityFunction(character));
		return targetPath.Item1 is ICell target && targetPath.Item2.Any()
			? (target, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}

	private bool IsAtNest(ICharacter character)
	{
		NpcHomeBaseEffect home = ResolveHomeBase(character);
		if (home.HomeCell is not null)
		{
			return ReferenceEquals(home.HomeCell, character.Location);
		}

		if (NestSiteProg.ExecuteBool(false, character, character.Location, character.Location))
		{
			home.SetHomeCell(character.Location);
			return true;
		}

		return false;
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetNestPath(ICharacter character)
	{
		NpcHomeBaseEffect home = ResolveHomeBase(character);
		if (home.HomeCell is not null)
		{
			List<ICellExit> homePath = character.PathBetween(home.HomeCell, DefaultNeedRange,
				GetAnimalSuitabilityFunction(character)).ToList();
			return homePath.Any()
				? (home.HomeCell, homePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		return GetEcologyCellPath(character, NestSiteProg);
	}

	private bool TryParentalGuard(ICharacter character)
	{
		if (!EcologyParentingEnabled || !IsNestingSeason(character))
		{
			return false;
		}

		List<ICharacter> protectedTargets = VisibleEcologyCharacters(character, includeSociallyTrusted: true)
		                                    .Where(x => ProtectProg.ExecuteBool(false, character, x))
		                                    .ToList();
		if (!protectedTargets.Any())
		{
			return false;
		}

		foreach (ICharacter threat in VisibleEcologyCharacters(character).Except(protectedTargets).Shuffle(Constants.Random))
		{
			if (IsSociallyTrusted(character, threat) ||
			    !IsParentingThreat(character, threat))
			{
				continue;
			}

			var (response, purpose) = ResolveThreatDecision(character, threat);
			if (response != AnimalThreatResponseType.Inherit)
			{
				return TryApplyThreatResponse(character, threat, response, purpose);
			}

			if (PredatorAIHelpers.CheckForAttack(character, threat, WillAttackProg,
				    EngageDelayDiceExpression, EngageEmote, false))
			{
				return true;
			}
		}

		return false;
	}

	private IEnumerable<ICharacter> VisibleEcologyCharacters(ICharacter character, bool includeSociallyTrusted = false)
	{
		return ObservedCharacters(character)
			.Where(x => includeSociallyTrusted || !IsSociallyTrusted(character, x));
	}

	private bool IsParentingThreat(ICharacter character, ICharacter target)
	{
		return AwarenessThreatProg.ExecuteBool(false, character, target) ||
		       WillAttackProg.ExecuteBool(false, character, target);
	}

	private bool TrySleepAtRefuge(ICharacter character)
	{
		bool seasonalDormancy = IsSeasonIn(_dormantSeasonGroups, character) &&
		                         DormancyMode != AnimalDormancyMode.Rest;
		if ((!ActivitySleepEnabled && !seasonalDormancy) ||
		    character.State.IsAsleep() ||
		    character.Combat is not null ||
		    character.Movement is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement")))
		{
			return false;
		}

		if (character.PositionState.CompareTo(character.Race.MinimumSleepingPosition) == PositionHeightComparison.Higher)
		{
			if (!character.CanMovePosition(character.Race.MinimumSleepingPosition))
			{
				return false;
			}

			character.MovePosition(character.Race.MinimumSleepingPosition, null, null);
			return true;
		}

		character.Sleep(string.IsNullOrWhiteSpace(ActivityRestEmote)
			? null
			: new Emote(ActivityRestEmote, character, character));
		return true;
	}

	private IEnumerable<ICharacter> ContextualThreatCandidates(ICharacter character, ICharacter? witnessedTarget)
	{
		IEnumerable<ICharacter> candidates = ObservedCharacters(character);
		if (witnessedTarget is not null)
		{
			candidates = candidates.Append(witnessedTarget);
		}

		return candidates
			.DistinctPhysicalInstances()
			.Where(x => !IsSociallyTrusted(character, x))
			.Where(x => CanObserveTarget(character, x))
			.Where(x => AwarenessThreatProg.ExecuteBool(false, character, x) ||
			            WillAttackProg.ExecuteBool(false, character, x) ||
			            ReferenceEquals(x.CombatTarget, character));
	}

	private bool HasProtectedYoung(ICharacter character)
	{
		return EcologyParentingEnabled && IsNestingSeason(character) &&
		       VisibleEcologyCharacters(character, includeSociallyTrusted: true)
		       .Any(x => ProtectProg.ExecuteBool(false, character, x));
	}

	internal (AnimalThreatResponseType Response, AnimalEngagementPurpose Purpose) ResolveThreatDecision(ICharacter character, ICharacter target)
	{
		if ((ReferenceEquals(character.CombatTarget, target) || ReferenceEquals(target.CombatTarget, character)) &&
		    AttackedThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return (AttackedThreatResponse, AnimalEngagementPurpose.SelfDefence);
		}

		if (HasProtectedYoung(character) && IsParentingThreat(character, target) &&
		    ParentingThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return (ParentingThreatResponse, AnimalEngagementPurpose.ProtectYoung);
		}

		if (HomeStrategyHandler.IsDefendingLocation(this, character) &&
		    TerritoryThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return (TerritoryThreatResponse, AnimalEngagementPurpose.Territory);
		}

		if (IsAggressiveSeason(character) && SeasonalThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return (SeasonalThreatResponse, AnimalEngagementPurpose.SeasonalAggression);
		}

		if (PredatorAIHelpers.IsHungry(character) &&
		    WillAttackProg.ExecuteBool(false, character, target) &&
		    HungryPreyResponse != AnimalThreatResponseType.Inherit)
		{
			return (HungryPreyResponse, AnimalEngagementPurpose.Hunt);
		}

		return (OrdinaryThreatResponse, AnimalEngagementPurpose.ThreatResponse);
	}

	private bool BeginPosturing(ICharacter character, ICharacter target)
	{
		if (character.Combat is not null || character.Movement is not null)
		{
			return false;
		}

		AIPosturingEffect? existing = character.EffectsOfType<AIPosturingEffect>().FirstOrDefault();
		if (existing is not null)
		{
			if (!existing.PosturingTargets.ContainsPhysicalInstance(target))
			{
				existing.PosturingTargets.Add(target);
			}

			return true;
		}

		(double Threat, bool StillPosturing, TimeSpan PostureLength) OnPostureExpired(double threat,
			IEnumerable<ICharacter> targets)
		{
			List<ICharacter> activeTargets = targets
				.Where(x => !IsSociallyTrusted(character, x) && CanObserveTarget(character, x))
				.ToList();
			if (!activeTargets.Any())
			{
				return (0.0, false, TimeSpan.Zero);
			}

			if (threat >= 1.0)
			{
				foreach (ICharacter activeTarget in activeTargets.Shuffle(Constants.Random))
				{
					var (response, purpose) = ResolveThreatDecision(character, activeTarget);
					if (response == AnimalThreatResponseType.Attack &&
					    (Hunting.Enabled ? TryApplyThreatResponse(character, activeTarget, response, purpose) :
					    PredatorAIHelpers.CheckForAttack(character, activeTarget, WillAttackProg,
						    EngageDelayDiceExpression, EngageEmote, false)))
					{
						return (0.0, false, TimeSpan.Zero);
					}
				}

				TryFlee(character, activeTargets.First());
				return (0.0, false, TimeSpan.Zero);
			}

			EmitPosture(character, activeTargets.First());
			return (1.0, true, TimeSpan.FromSeconds(Math.Max(1, Dice.Roll(PostureDurationDiceExpression))));
		}

		AIPosturingEffect effect = new(character, new[] { target }, OnPostureExpired);
		effect.ThreatLevel = 0.0;
		character.AddEffect(effect, TimeSpan.FromSeconds(Math.Max(1, Dice.Roll(PostureDurationDiceExpression))));
		EmitPosture(character, target);
		return true;
	}

	private bool BuildingCommandActivitySeasonGroup(ICharacter actor, StringStack command, List<string> seasonGroups,
		string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must supply a season group name, or use #3clear#0 to remove all {label} season groups.".SubstituteANSIColour());
			return false;
		}

		string value = command.SafeRemainingArgument.Trim();
		if (value.EqualToAny("clear", "none", "remove", "delete"))
		{
			seasonGroups.Clear();
			Changed = true;
			actor.OutputHandler.Send($"This animal AI no longer has any {label} season groups.");
			return true;
		}

		if (seasonGroups.Any(x => string.Equals(x, value, StringComparison.InvariantCultureIgnoreCase)))
		{
			seasonGroups.RemoveAll(x => string.Equals(x, value, StringComparison.InvariantCultureIgnoreCase));
			actor.OutputHandler.Send($"This animal AI will no longer treat {value.ColourValue()} as a {label} season group.");
		}
		else
		{
			seasonGroups.Add(value);
			actor.OutputHandler.Send($"This animal AI will now treat {value.ColourValue()} as a {label} season group.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandThreatResponse(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a context (ordinary, hungryprey, attacked, territory, parenting or seasonal) and a response (inherit, ignore, avoid, flee, posture or attack).");
			return false;
		}

		string context = command.PopForSwitch();
		if (command.IsFinished || !command.PopSpeech().TryParseEnum(out AnimalThreatResponseType response))
		{
			actor.OutputHandler.Send($"You must specify a threat response. Valid values are {Enum.GetValues<AnimalThreatResponseType>().ListToColouredString()}.");
			return false;
		}

		switch (context)
		{
			case "ordinary":
				OrdinaryThreatResponse = response;
				break;
			case "hungryprey":
			case "prey":
			case "hungry":
				HungryPreyResponse = response;
				break;
			case "attacked":
			case "cornered":
				AttackedThreatResponse = response;
				break;
			case "territory":
			case "den":
				TerritoryThreatResponse = response;
				break;
			case "parenting":
			case "young":
				ParentingThreatResponse = response;
				break;
			case "seasonal":
			case "season":
				SeasonalThreatResponse = response;
				break;
			default:
				actor.OutputHandler.Send("That is not a valid threat context.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now {response.DescribeEnum().ColourName()} for {context.ColourValue()} threats.");
		return true;
	}

	private bool TryContextualThreatResponse(ICharacter character, ICharacter? witnessedTarget)
	{
		List<ICharacter> candidates = ContextualThreatCandidates(character, witnessedTarget).ToList();
		if (SensesStrategy == AnimalSensesStrategyType.Tracking)
		{
			RememberThreats(character, candidates);
		}

		foreach (ICharacter target in candidates.Shuffle(Constants.Random))
		{
			var (response, purpose) = ResolveThreatDecision(character, target);
			if (response == AnimalThreatResponseType.Inherit)
			{
				continue;
			}

			return TryApplyThreatResponse(character, target, response, purpose);
		}

		return false;
	}

	private bool TryApplyThreatResponse(ICharacter character, ICharacter target,
		AnimalThreatResponseType response, AnimalEngagementPurpose purpose)
	{
		return response switch
		{
			AnimalThreatResponseType.Ignore => true,
			AnimalThreatResponseType.Avoid => character.Combat is null &&
				TryMoveAwayFromAwarenessThreats(character, new[] { target }),
			AnimalThreatResponseType.Flee when character.Combat is not null => SetFleeCombatStrategy(character),
			AnimalThreatResponseType.Flee => TryFlee(character, target),
			AnimalThreatResponseType.Posture => BeginPosturing(character, target),
			AnimalThreatResponseType.Attack when (Hunting.Enabled
				? purpose == AnimalEngagementPurpose.Hunt
				: PredatorAIHelpers.IsHungry(character) && HungryPreyResponse == AnimalThreatResponseType.Attack) =>
				TryHungryPredatorAttack(character, target),
			AnimalThreatResponseType.Attack => PredatorAIHelpers.CheckForAttack(character, target, Hunting.Enabled ? Gameworld.AlwaysTrueProg : WillAttackProg,
				EngageDelayDiceExpression, EngageEmote, false),
			_ => false
		};
	}

	private bool TryThreatResponse(ICharacter character, ICharacter? witnessedTarget)
	{
		if ((IsGroupControlled(character, GroupAIControlScope.Threats) && character.Combat is null) ||
		    character.Movement is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")))
		{
			return false;
		}

		return TryContextualThreatResponse(character, witnessedTarget) ||
		       (character.Combat is null && ThreatStrategyHandler.TryRespond(this, character, witnessedTarget));
	}

	private bool TryHungryPredatorAttack(ICharacter character, ICharacter target)
	{
		if (Hunting.Enabled)
		{
			return BeginHunt(character, target);
		}
		if (!CanHuntTarget(character, target))
		{
			return false;
		}

		return PredatorAIHelpers.CheckForAttack(character, target, WillAttackProg, EngageDelayDiceExpression,
			EngageEmote, true);
	}

	private bool TryDefensiveAttack(ICharacter character, ICharacter target)
	{
		return !IsSociallyTrusted(character, target) &&
		       HomeStrategyHandler.IsDefendingLocation(this, character) &&
		       PredatorAIHelpers.CheckForAttack(character, target, Hunting.Enabled ? Gameworld.AlwaysTrueProg : WillAttackProg, EngageDelayDiceExpression,
			       EngageEmote, false);
	}

	private bool TryFlee(ICharacter character, ICharacter target)
	{
		if (IsSociallyTrusted(character, target) ||
		    (!WillAttackProg.ExecuteBool(false, character, target) &&
		     !AwarenessThreatProg.ExecuteBool(false, character, target)))
		{
			return false;
		}

		ICellExit? exit = character.Location.ExitsFor(character)
		                           .Where(GetAnimalSuitabilityFunction(character))
		                           .Where(x => !x.Destination.LayerCharacters(character.RoomLayer).Any(y => y != character))
		                           .GetRandomElement();
		if (exit is null || !character.CanMove(exit))
		{
			return false;
		}

		return character.Move(exit);
	}

	private void HandleWitnessedDeath(ICharacter character, ICharacter victim)
	{
		if (!FeedingStrategy.In(AnimalFeedingStrategyType.DenPredator, AnimalFeedingStrategyType.DenOmnivore))
		{
			return;
		}

		bool wasFightingVictim = character.CombatTarget == victim ||
		                         victim.CombatTarget == character ||
		                         (character.Combat is not null && ReferenceEquals(character.Combat, victim.Combat));
		if (!PredatorAIHelpers.IsHungry(character) ||
		    !wasFightingVictim ||
		    !PredatorAIHelpers.CouldEatAfterKilling(character, victim))
		{
			return;
		}

		NpcBurrowFoodEffect burrowFood = NpcBurrowFoodEffect.GetOrCreate(character);
		burrowFood.SetPendingVictim(victim);
		burrowFood.ClearFood();
	}

	private void EvaluateBurrowFoodLifecycle(ICharacter character)
	{
		if (character.Movement is not null || character.Combat is not null)
		{
			return;
		}

		NpcBurrowFoodEffect? burrowFood = NpcBurrowFoodEffect.Get(character);
		if (burrowFood is null || !ResolveBurrowFood(character, burrowFood))
		{
			return;
		}

		ICorpse? corpse = burrowFood.FoodCorpse;
		IGameItem? food = corpse?.Parent;
		if (corpse is null || food is null || !character.CanEat(corpse, character.Race.BiteWeight).Success)
		{
			burrowFood.Clear();
			return;
		}

		NpcHomeBaseEffect home = ResolveHomeBase(character);
		if (home.HomeCell is null)
		{
			if (BurrowSiteProg.ExecuteBool(false, character, character.Location))
			{
				home.SetHomeCell(character.Location);
			}
			else
			{
				CheckPathingEffect(character, true);
				return;
			}
		}

		if (!ReferenceEquals(character.Location, home.HomeCell))
		{
			EnsureDraggingFood(character, food);
			CheckPathingEffect(character, true);
			return;
		}

		StopDraggingFood(character, food);
		if (!ReferenceEquals(food.Location, character.Location) &&
		    character.Body.HeldOrWieldedItems.Contains(food) &&
		    character.Body.CanDrop(food, 0))
		{
			character.Body.Drop(food, silent: true);
		}

		if (ReferenceEquals(food.Location, character.Location) ||
		    character.Body.HeldOrWieldedItems.Contains(food))
		{
			character.Eat(corpse, character.Race.BiteWeight, null);
		}

		if (!PredatorAIHelpers.IsHungry(character) || corpse.Parent?.Location is null)
		{
			burrowFood.Clear();
		}
	}

	private bool ResolveBurrowFood(ICharacter character, NpcBurrowFoodEffect burrowFood)
	{
		if (burrowFood.FoodCorpse is not null)
		{
			return true;
		}

		ICharacter? victim = burrowFood.PendingVictim;
		if (victim?.Corpse?.Parent is IGameItem corpseItem &&
		    PredatorAIHelpers.CouldEatAfterKilling(character, victim))
		{
			burrowFood.SetFoodItem(corpseItem);
			burrowFood.ClearPendingVictim();
			return true;
		}

		if (victim is null)
		{
			burrowFood.Clear();
		}

		return false;
	}

	private static void EnsureDraggingFood(ICharacter character, IGameItem food)
	{
		if (character.EffectsOfType<Dragging>().Any(x => ReferenceEquals(x.Target, food)))
		{
			return;
		}

		if (!ReferenceEquals(food.Location, character.Location) ||
		    food.GetItemType<IHoldable>() is not { IsHoldable: true })
		{
			return;
		}

		character.AddEffect(new Dragging(character, null, food));
	}

	private static void StopDraggingFood(ICharacter character, IGameItem food)
	{
		foreach (Dragging dragging in character.EffectsOfType<Dragging>()
		                                     .Where(x => ReferenceEquals(x.Target, food))
		                                     .ToList())
		{
			character.RemoveEffect(dragging, true);
		}
	}

	protected override bool WouldMove(ICharacter ch)
	{
		if (ch.Combat is not null)
		{
			return false;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Senses) &&
		    AwarenessStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Feeding) && WaterStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Feeding) && FeedingStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Activity) &&
		    IsActivityInactive(ch) &&
		    SurvivalNeedsSatisfied(ch))
		{
			return false;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Shelter) && EcologyWouldMove(ch))
		{
			return true;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Shelter) && ShouldReturnToRefuge(ch))
		{
			return true;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Senses) && WouldTrackKnownPrey(ch))
		{
			return true;
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Movement) && HomeStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		return !IsGroupControlled(ch, GroupAIControlScope.Movement) &&
		       MovementEnabledProg.ExecuteBool(false, ch) &&
		       RandomUtilities.DoubleRandom(0.0, 1.0) <= WanderChancePerMinute;
	}

	private bool HasLocalFoodOpportunity(ICharacter ch)
	{
		return FeedingStrategyHandler.HasLocalFoodOpportunity(this, ch);
	}

	protected override (ICell? Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		(ICell? target, IEnumerable<ICellExit> path) = (null, Enumerable.Empty<ICellExit>());
		if (!IsGroupControlled(ch, GroupAIControlScope.Senses))
		{
			(target, path) = AwarenessStrategyHandler.GetPath(this, ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Feeding))
		{
			(target, path) = WaterStrategyHandler.GetPath(this, ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}

			(target, path) = FeedingStrategyHandler.GetPath(this, ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}
		}

		if (Hunting.Enabled && Hunting.Opening == AnimalHuntOpening.TrapWait &&
		    !ch.EffectsOfType<AnimalHuntEffect>().Any() && !IsGroupControlled(ch, GroupAIControlScope.Movement) &&
		    ResolveHomeBase(ch).HomeCell is { } huntingHome && !ReferenceEquals(huntingHome, ch.Location))
		{
			return (huntingHome, ch.PathBetween(huntingHome, DefaultNeedRange, GetAnimalSuitabilityFunction(ch)));
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Activity) &&
		    IsActivityInactive(ch) &&
		    SurvivalNeedsSatisfied(ch))
		{
			return (ch.Location, Enumerable.Empty<ICellExit>());
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Shelter))
		{
			(target, path) = GetEcologyPath(ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Shelter) && ShouldReturnToRefuge(ch))
		{
			(target, path) = GetRefugePath(ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Senses) && WouldTrackKnownPrey(ch))
		{
			(target, path) = GetKnownPreyPath(ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}
		}

		if (!IsGroupControlled(ch, GroupAIControlScope.Movement))
		{
			(target, path) = HomeStrategyHandler.GetPath(this, ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}

			(ICell? ambientTarget, IEnumerable<ICellExit> ambientPath) = MovementStrategyHandler.GetAmbientPath(this, ch);
			return ambientTarget is not null
				? (ambientTarget, ambientPath)
				: (ch.Location, ambientPath);
		}

		return (ch.Location, Enumerable.Empty<ICellExit>());
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetBurrowFoodPath(ICharacter ch)
	{
		NpcBurrowFoodEffect food = NpcBurrowFoodEffect.Get(ch)!;
		ResolveBurrowFood(ch, food);
		NpcHomeBaseEffect foodHome = ResolveHomeBase(ch);
		if (food.FoodCorpse is not null && foodHome.HomeCell is not null && !ReferenceEquals(foodHome.HomeCell, ch.Location))
		{
			List<ICellExit> foodHomePath = ch.PathBetween(foodHome.HomeCell, DefaultNeedRange, GetAnimalSuitabilityFunction(ch)).ToList();
			return foodHomePath.Any()
				? (foodHome.HomeCell, foodHomePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		if (foodHome.HomeCell is not null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = ch.AcquireTargetAndPath(
			x => x is ICell cell && BurrowSiteProg.ExecuteBool(false, ch, cell),
			DefaultNeedRange,
			GetAnimalSuitabilityFunction(ch));
		return targetPath.Item1 is ICell burrowCell && targetPath.Item2.Any()
			? (burrowCell, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetFoodPath(ICharacter ch)
	{
		if (FeedingStrategy.In(AnimalFeedingStrategyType.Predator, AnimalFeedingStrategyType.DenPredator))
		{
			return GetPredatorFoodPath(ch);
		}

		if (FeedingStrategy == AnimalFeedingStrategyType.Forager)
		{
			return GetForagerFoodPath(ch);
		}

		if (FeedingStrategy == AnimalFeedingStrategyType.Scavenger)
		{
			return GetScavengerFoodPath(ch);
		}

		if (FeedingStrategy == AnimalFeedingStrategyType.Opportunist)
		{
			(ICell? target, IEnumerable<ICellExit> path) = GetScavengerFoodPath(ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}

			return GetForagerFoodPath(ch);
		}

		if (FeedingStrategy.In(AnimalFeedingStrategyType.Omnivore, AnimalFeedingStrategyType.DenOmnivore))
		{
			(ICell? target, IEnumerable<ICellExit> path) = GetScavengerFoodPath(ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}

			(target, path) = GetForagerFoodPath(ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}

			return GetPredatorFoodPath(ch);
		}

		return (null, Enumerable.Empty<ICellExit>());
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetPredatorFoodPath(ICharacter ch)
	{
		foreach (ICharacter prey in RemotePredatorCandidates(ch))
		{
			if (!CanHuntTarget(ch, prey))
			{
				ch.LoseTarget(prey);
				continue;
			}

			if (prey.Location!.RouteDefinition is not null || ch.Location.RouteDefinition is not null)
			{
				if (TryFindSpatialPath(ch, RouteSpatialService.Instance.GetEffectiveLocation(prey), DefaultNeedRange,
						GetAnimalSuitabilityFunction(ch), out _))
				{
					return (prey.Location, Enumerable.Empty<ICellExit>());
				}

				ch.LoseTarget(prey);
				continue;
			}

			List<ICellExit> path = ch.PathBetween(prey.Location, DefaultNeedRange,
				GetAnimalSuitabilityFunction(ch)).ToList();
			if (path.Any())
			{
				return (prey.Location, path);
			}

			ch.LoseTarget(prey);
		}

		return (null, Enumerable.Empty<ICellExit>());
	}

	/// <summary>
	/// Supplies an exact-coordinate RouteCell path for a remotely scanned prey target. The regular
	/// exit path is still used for ordinary cells; keeping the spatial path here avoids reducing a
	/// live RouteCell target to the route's default coordinate.
	/// </summary>
	protected override (ICell? Target, ISpatialPath? Path) GetSpatialPath(ICharacter ch)
	{
		if (!FeedingStrategy.In(AnimalFeedingStrategyType.Predator, AnimalFeedingStrategyType.DenPredator,
			    AnimalFeedingStrategyType.Omnivore, AnimalFeedingStrategyType.DenOmnivore) ||
		    !PredatorAIHelpers.IsHungry(ch) ||
		    HasLocalFoodOpportunity(ch))
		{
			return (null, null);
		}

		foreach (ICharacter prey in RemotePredatorCandidates(ch))
		{
			if (!CanHuntTarget(ch, prey))
			{
				ch.LoseTarget(prey);
				continue;
			}

			if (prey.Location!.RouteDefinition is null && ch.Location.RouteDefinition is null)
			{
				continue;
			}

			if (TryFindSpatialPath(ch, RouteSpatialService.Instance.GetEffectiveLocation(prey), DefaultNeedRange,
					GetAnimalSuitabilityFunction(ch), out ISpatialPath? path) &&
				path is not null && path.Steps.Count > 0)
			{
				return (prey.Location, path);
			}

			ch.LoseTarget(prey);
		}

		return (null, null);
	}

	private IEnumerable<ICharacter> RemotePredatorCandidates(ICharacter character)
	{
		return character.SeenTargets
		                .OfType<ICharacter>()
		                .Where(x => !IsLocalHuntTarget(character, x))
		                .OrderBy(x =>
		                {
			                int distance = character.DistanceBetween(x, DefaultNeedRange);
			                return distance < 0 ? int.MaxValue : distance;
			                })
		                .ToList();
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetForagerFoodPath(ICharacter ch)
	{
		if (ch.CombinedEffectsOfType<Territory>().FirstOrDefault() is Territory territory && territory.Cells.Any())
		{
			List<ICell> territoryCells = territory.Cells
			                                      .Where(x => ForagerAIHelpers.HasFoodOpportunity(ch, x))
			                                      .ToList();
			List<ICellExit> territoryPath = ch.PathBetween(territoryCells.Cast<IPerceivable>(), DefaultNeedRange,
				GetAnimalSuitabilityFunction(ch, true)).ToList();
			if (territoryPath.Any())
			{
				return (territoryPath.Last().Destination, territoryPath);
			}
		}

		Tuple<IPerceivable, IEnumerable<ICellExit>> forageTargetPath = ch.AcquireTargetAndPath(
			x => x is ICell cell && ForagerAIHelpers.HasFoodOpportunity(ch, cell),
			DefaultNeedRange,
			GetAnimalSuitabilityFunction(ch));
		return forageTargetPath.Item1 is ICell target && forageTargetPath.Item2.Any()
			? (target, forageTargetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetScavengerFoodPath(ICharacter ch)
	{
		Tuple<IPerceivable, IEnumerable<ICellExit>> scavengeTargetPath = ch.AcquireTargetAndPath(
			x => x is ICell cell && HasScavengerFoodOpportunity(ch, cell),
			DefaultNeedRange,
			GetAnimalSuitabilityFunction(ch));
		return scavengeTargetPath.Item1 is ICell target && scavengeTargetPath.Item2.Any()
			? (target, scavengeTargetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}

	private bool HasScavengerFoodOpportunity(ICharacter character, ICell cell)
	{
		if (!ForagerAIHelpers.IsHungry(character))
		{
			return false;
		}

		return cell.LayerGameItems(character.RoomLayer)
		           .SelectMany(x => x.ShallowAccessibleItems(character))
		           .Any(x =>
			           x.GetItemType<IEdible>() is IEdible edible &&
			           character.CanEat(edible, edible.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0) ||
			           x.GetItemType<ICorpse>() is ICorpse corpse &&
			           character.CanEat(corpse, character.Race.BiteWeight).Success ||
			           x.GetItemType<ISeveredBodypart>() is ISeveredBodypart bodypart &&
			           character.CanEat(bodypart, character.Race.BiteWeight).Success);
	}

	private bool TryEatLocalScavengerFood(ICharacter character)
	{
		if (!ForagerAIHelpers.IsHungry(character) ||
		    character.State.HasFlag(CharacterState.Dead) ||
		    character.State.HasFlag(CharacterState.Stasis) ||
		    character.Combat is not null ||
		    character.Movement is not null ||
		    !CharacterState.Able.HasFlag(character.State) ||
		    character.Effects.Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement")))
		{
			return false;
		}

		IEnumerable<IGameItem> candidates = character.Body.HeldOrWieldedItems
		                                             .Concat(character.Location.LayerGameItems(character.RoomLayer)
		                                                              .SelectMany(x => x.ShallowAccessibleItems(character)));

		foreach (IGameItem item in candidates.Shuffle(Constants.Random))
		{
			if (item.GetItemType<IEdible>() is IEdible edible &&
			    character.CanEat(edible, edible.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0))
			{
				character.SetTarget(edible.Parent);
				character.SetModifier(PositionModifier.None);
				character.SetEmote(null);
				character.Eat(edible, edible.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0, null);
				return true;
			}

			if (item.GetItemType<ICorpse>() is ICorpse corpse &&
			    character.CanEat(corpse, character.Race.BiteWeight).Success)
			{
				character.SetTarget(corpse.Parent);
				character.SetModifier(PositionModifier.None);
				character.SetEmote(null);
				character.Eat(corpse, character.Race.BiteWeight, null);
				return true;
			}

			if (item.GetItemType<ISeveredBodypart>() is ISeveredBodypart bodypart &&
			    character.CanEat(bodypart, character.Race.BiteWeight).Success)
			{
				character.SetTarget(bodypart.Parent);
				character.SetModifier(PositionModifier.None);
				character.SetEmote(null);
				character.Eat(bodypart, character.Race.BiteWeight, null);
				return true;
			}
		}

		return false;
	}

	private interface IAnimalWaterStrategy
	{
		bool IsThirsty(AnimalAI ai, ICharacter character);
		bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character);
		bool WouldMove(AnimalAI ai, ICharacter character);
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character);
	}

	private sealed class DisabledWaterStrategy : IAnimalWaterStrategy
	{
		public static DisabledWaterStrategy Instance { get; } = new();

		public bool IsThirsty(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class DrinkWaterStrategy : IAnimalWaterStrategy
	{
		public static DrinkWaterStrategy Instance { get; } = new();

		public bool IsThirsty(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.IsThirsty(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.TryDrinkIfThirsty(character);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.IsThirsty(character) &&
			       !NpcSurvivalAIHelpers.HasLocalWaterSource(character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? NpcSurvivalAIHelpers.GetPathToWater(character, ai.GetAnimalSuitabilityFunction(character),
					DefaultNeedRange)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class ImmersionWaterStrategy : IAnimalWaterStrategy
	{
		public static ImmersionWaterStrategy Instance { get; } = new();

		public bool IsThirsty(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.IsThirsty(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.TryHydrateFromAquaticEnvironmentIfThirsty(character, false);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.IsThirsty(character) &&
			       !NpcSurvivalAIHelpers.HasAquaticWaterSource(character, character.Location, false);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? NpcSurvivalAIHelpers.GetPathToAquaticWater(character, ai.GetAnimalSuitabilityFunction(character),
					DefaultNeedRange, false)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class SurfaceWaterStrategy : IAnimalWaterStrategy
	{
		public static SurfaceWaterStrategy Instance { get; } = new();

		public bool IsThirsty(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.IsThirsty(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.TryHydrateFromAquaticEnvironmentIfThirsty(character, true);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return NpcSurvivalAIHelpers.IsThirsty(character) &&
			       !NpcSurvivalAIHelpers.HasAquaticWaterSource(character, character.Location, true);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? NpcSurvivalAIHelpers.GetPathToAquaticWater(character, ai.GetAnimalSuitabilityFunction(character),
					DefaultNeedRange, true)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	private interface IAnimalFeedingStrategy
	{
		bool IsHungry(AnimalAI ai, ICharacter character);
		bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character);
		bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character);
		bool WouldMove(AnimalAI ai, ICharacter character);
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character);
		void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim);
	}

	private sealed class NoFeedingStrategy : IAnimalFeedingStrategy
	{
		public static NoFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
		}
	}

	private sealed class PredatorFeedingStrategy : IAnimalFeedingStrategy
	{
		public static PredatorFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return PredatorAIHelpers.IsHungry(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return PredatorAIHelpers.EatLocalCorpseIfHungry(character);
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return PredatorAIHelpers.FindLocalEdibleCorpse(character) is not null ||
			       character.Location.LayerCharacters(character.RoomLayer)
			                .Except(character)
			                .Any(x => ai.CanHuntLocalTarget(character, x));
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return IsHungry(ai, character) && !HasLocalFoodOpportunity(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			if (!WouldMove(ai, character))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			(ICell? target, IEnumerable<ICellExit> path) = ai.GetScavengerFoodPath(character);
			if (target is not null && path.Any())
			{
				return (target, path);
			}

			(target, path) = ai.GetForagerFoodPath(character);
			if (target is not null && path.Any())
			{
				return (target, path);
			}

			return ai.GetPredatorFoodPath(character);
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
		}
	}

	private sealed class DenPredatorFeedingStrategy : IAnimalFeedingStrategy
	{
		public static DenPredatorFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return PredatorAIHelpers.IsHungry(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			if (NpcBurrowFoodEffect.Get(character)?.HasAnyTarget == true)
			{
				ai.EvaluateBurrowFoodLifecycle(character);
				return true;
			}

			return PredatorAIHelpers.EatLocalCorpseIfHungry(character);
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return PredatorFeedingStrategy.Instance.HasLocalFoodOpportunity(ai, character);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return NpcBurrowFoodEffect.Get(character)?.HasAnyTarget == true ||
			       IsHungry(ai, character) && !HasLocalFoodOpportunity(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			if (NpcBurrowFoodEffect.Get(character)?.HasAnyTarget == true)
			{
				return ai.GetBurrowFoodPath(character);
			}

			return IsHungry(ai, character) && !HasLocalFoodOpportunity(ai, character)
				? ai.GetFoodPath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
			ai.HandleWitnessedDeath(character, victim);
		}
	}

	private sealed class ForagerFeedingStrategy : IAnimalFeedingStrategy
	{
		public static ForagerFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return ForagerAIHelpers.IsHungry(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return ForagerAIHelpers.TrySatisfyHunger(character);
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return ForagerAIHelpers.HasFoodOpportunity(character, character.Location);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return IsHungry(ai, character) && !HasLocalFoodOpportunity(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? ai.GetFoodPath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
		}
	}

	private sealed class ScavengerFeedingStrategy : IAnimalFeedingStrategy
	{
		public static ScavengerFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return ForagerAIHelpers.IsHungry(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return ai.TryEatLocalScavengerFood(character);
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return ai.HasScavengerFoodOpportunity(character, character.Location);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return IsHungry(ai, character) && !HasLocalFoodOpportunity(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? ai.GetFoodPath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
		}
	}

	private sealed class OpportunistFeedingStrategy : IAnimalFeedingStrategy
	{
		public static OpportunistFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return ForagerAIHelpers.IsHungry(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return ai.TryEatLocalScavengerFood(character) ||
			       ForagerAIHelpers.TrySatisfyHunger(character);
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return ai.HasScavengerFoodOpportunity(character, character.Location) ||
			       ForagerAIHelpers.HasFoodOpportunity(character, character.Location);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return IsHungry(ai, character) && !HasLocalFoodOpportunity(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? ai.GetFoodPath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
		}
	}

	private sealed class OmnivoreFeedingStrategy : IAnimalFeedingStrategy
	{
		public static OmnivoreFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return ForagerAIHelpers.IsHungry(character) || PredatorAIHelpers.IsHungry(character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			return ai.TryEatLocalScavengerFood(character) ||
			       ForagerAIHelpers.TrySatisfyHunger(character) ||
			       PredatorAIHelpers.EatLocalCorpseIfHungry(character);
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return ai.HasScavengerFoodOpportunity(character, character.Location) ||
			       ForagerAIHelpers.HasFoodOpportunity(character, character.Location) ||
			       PredatorFeedingStrategy.Instance.HasLocalFoodOpportunity(ai, character);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return IsHungry(ai, character) && !HasLocalFoodOpportunity(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? ai.GetFoodPath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
		}
	}

	private sealed class DenOmnivoreFeedingStrategy : IAnimalFeedingStrategy
	{
		public static DenOmnivoreFeedingStrategy Instance { get; } = new();

		public bool IsHungry(AnimalAI ai, ICharacter character)
		{
			return OmnivoreFeedingStrategy.Instance.IsHungry(ai, character);
		}

		public bool TrySatisfyImmediateNeed(AnimalAI ai, ICharacter character)
		{
			if (NpcBurrowFoodEffect.Get(character)?.HasAnyTarget == true)
			{
				ai.EvaluateBurrowFoodLifecycle(character);
				return true;
			}

			return OmnivoreFeedingStrategy.Instance.TrySatisfyImmediateNeed(ai, character);
		}

		public bool HasLocalFoodOpportunity(AnimalAI ai, ICharacter character)
		{
			return OmnivoreFeedingStrategy.Instance.HasLocalFoodOpportunity(ai, character);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return NpcBurrowFoodEffect.Get(character)?.HasAnyTarget == true ||
			       OmnivoreFeedingStrategy.Instance.WouldMove(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			if (NpcBurrowFoodEffect.Get(character)?.HasAnyTarget == true)
			{
				return ai.GetBurrowFoodPath(character);
			}

			return OmnivoreFeedingStrategy.Instance.GetPath(ai, character);
		}

		public void HandleWitnessedDeath(AnimalAI ai, ICharacter character, ICharacter victim)
		{
			ai.HandleWitnessedDeath(character, victim);
		}
	}

	private interface IAnimalThreatStrategy
	{
		bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget);
	}

	private sealed class PassiveThreatStrategy : IAnimalThreatStrategy
	{
		public static PassiveThreatStrategy Instance { get; } = new();

		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
		{
			return false;
		}
	}

	private sealed class FleeThreatStrategy : IAnimalThreatStrategy
	{
		public static FleeThreatStrategy Instance { get; } = new();

		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
		{
			if (witnessedTarget is not null && !ai.IsSociallyTrusted(character, witnessedTarget))
			{
				return ai.TryFlee(character, witnessedTarget);
			}

			foreach (ICharacter target in character.Location.LayerCharacters(character.RoomLayer)
			                                    .Except(character)
			                                    .Where(x => !ai.IsSociallyTrusted(character, x))
				                                    .Shuffle(Constants.Random))
			{
				if (ai.TryFlee(character, target))
				{
					return true;
				}
			}

			return false;
		}
	}

	private abstract class AttackThreatStrategyBase : IAnimalThreatStrategy
	{
		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
		{
			if (witnessedTarget is not null && !ai.IsSociallyTrusted(character, witnessedTarget))
			{
				return TryAttack(ai, character, witnessedTarget);
			}

			foreach (ICharacter target in character.Location.LayerCharacters(character.RoomLayer)
			                                    .Except(character)
			                                    .Where(x => !ai.IsSociallyTrusted(character, x))
				                                    .Shuffle(Constants.Random))
			{
				if (TryAttack(ai, character, target))
				{
					return true;
				}
			}

			uint range = (uint)character.Body!.WieldedItems
			                       .SelectNotNull(x => x!.GetItemType<IRangedWeapon>())
			                       .Where(x => x.IsReadied || x.CanReady(character))
			                       .Select(x => (int)x.WeaponType.DefaultRangeInRooms)
			                       .DefaultIfEmpty(0)
			                       .Max();
			if (range == 0)
			{
				return false;
			}

			foreach (ICharacter target in ai.ObservedCharacters(character)
			                                    .Where(x => !ai.IsSociallyTrusted(character, x))
			                                    .Where(x => !IsLocalHuntTarget(character, x))
			                                    .Where(x => ScanTargetAcquisition.IsVisibleRangedTarget(character, x, range,
				                                    false))
			                                    .ToList())
			{
				if (TryAttack(ai, character, target))
				{
					return true;
				}
			}

			return false;
		}

		protected abstract bool TryAttack(AnimalAI ai, ICharacter character, ICharacter target);
	}

	private sealed class DefendThreatStrategy : AttackThreatStrategyBase
	{
		public static DefendThreatStrategy Instance { get; } = new();

		protected override bool TryAttack(AnimalAI ai, ICharacter character, ICharacter target)
		{
			return ai.TryDefensiveAttack(character, target);
		}
	}

	private sealed class HungryPredatorThreatStrategy : AttackThreatStrategyBase
	{
		public static HungryPredatorThreatStrategy Instance { get; } = new();

		protected override bool TryAttack(AnimalAI ai, ICharacter character, ICharacter target)
		{
			return ai.TryHungryPredatorAttack(character, target);
		}
	}

	private interface IAnimalActivityStrategy
	{
		bool IsActive(AnimalAI ai, ICharacter character);
		bool WouldMove(AnimalAI ai, ICharacter character);
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character);
	}

	private sealed class AlwaysActivityStrategy : IAnimalActivityStrategy
	{
		public static AlwaysActivityStrategy Instance { get; } = new();

		public bool IsActive(AnimalAI ai, ICharacter character)
		{
			return true;
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class TimedActivityStrategy : IAnimalActivityStrategy
	{
		public static TimedActivityStrategy Instance { get; } = new();

		public bool IsActive(AnimalAI ai, ICharacter character)
		{
			return ai._activeTimesOfDay.Contains(character.Location.CurrentTimeOfDay);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return ai.SurvivalNeedsSatisfied(character) &&
			       !IsActive(ai, character) &&
			       !ai.IsAtRefuge(character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WouldMove(ai, character)
				? ai.GetRefugePath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}


}
