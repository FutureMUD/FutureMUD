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

public class AnimalAI : PathingAIBase
{
	private const int DefaultGroundRange = 10;
	private const int DefaultSwimRange = 15;
	private const int DefaultFlyRange = 30;
	private const int DefaultArborealRange = 10;
	private const int DefaultNeedRange = 20;

	public AnimalMovementStrategyType MovementStrategy { get; private set; }
	public AnimalHomeStrategyType HomeStrategy { get; private set; }
	public AnimalFeedingStrategyType FeedingStrategy { get; private set; }
	public AnimalWaterStrategyType WaterStrategy { get; private set; }
	public AnimalThreatStrategyType ThreatStrategy { get; private set; }
	public AnimalAwarenessStrategyType AwarenessStrategy { get; private set; }
	public AnimalRefugeStrategyType RefugeStrategy { get; private set; }
	public AnimalActivityStrategyType ActivityStrategy { get; private set; }
	public AnimalDormancyMode DormancyMode { get; private set; }
	public AnimalSensesStrategyType SensesStrategy { get; private set; }
	/// <summary>
	/// When enabled, an NPC carrying this profile receives an active needs model if its template
	/// would otherwise supply passive or no needs. Omitted legacy XML remains false.
	/// </summary>
	public bool UseActiveNeeds { get; private set; }
	public bool WaterEnabled => WaterStrategy != AnimalWaterStrategyType.Off;

	public IFutureProg MovementEnabledProg { get; private set; } = null!;
	public IFutureProg MovementCellProg { get; private set; } = null!;
	public IFutureProg PreferredHabitatProg { get; private set; } = null!;
	public IFutureProg ToleratedHabitatProg { get; private set; } = null!;
	public IFutureProg AmphibiousLandCellProg { get; private set; } = null!;
	public IFutureProg AmphibiousWaterCellProg { get; private set; } = null!;
	public IFutureProg AllowDescentProg { get; private set; } = null!;
	public IFutureProg SuitableTerritoryProg { get; private set; } = null!;
	public IFutureProg DesiredTerritorySizeProg { get; private set; } = null!;
	public IFutureProg BurrowSiteProg { get; private set; } = null!;
	public IFutureProg BuildEnabledProg { get; private set; } = null!;
	public IFutureProg WillAttackProg { get; private set; } = null!;
	public IFutureProg AwarenessThreatProg { get; private set; } = null!;
	public IFutureProg AwarenessAvoidCellProg { get; private set; } = null!;
	public IFutureProg RefugeCellProg { get; private set; } = null!;
	public IFutureProg ShelterNeededProg { get; private set; } = null!;
	public IFutureProg ShelterCellProg { get; private set; } = null!;
	public IFutureProg SeasonalCellProg { get; private set; } = null!;
	public IFutureProg NestSiteProg { get; private set; } = null!;
	public IFutureProg ProtectProg { get; private set; } = null!;
	public IFutureProg? HomeLocationProg { get; private set; }
	public IFutureProg? AnchorItemProg { get; private set; }
	public ICraft? BurrowCraft { get; private set; }
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

	public int MovementRange { get; private set; }
	public double AmphibiousWaterBias { get; private set; }
	public double WanderChancePerMinute { get; private set; }
	public string WanderEmote { get; private set; } = string.Empty;
	public string EngageDelayDiceExpression { get; private set; } = "1000+1d1000";
	public string EngageEmote { get; private set; } = string.Empty;
	public string PostureEmote { get; private set; } = string.Empty;
	public string PostureDurationDiceExpression { get; private set; } = "1d20+20";
	public int AwarenessRange { get; private set; }
	public int AwarenessMemoryMinutes { get; private set; }
	public int RefugeReturnSeconds { get; private set; }
	public RoomLayer TargetFlyingLayer { get; private set; }
	public RoomLayer TargetRestingLayer { get; private set; }
	public RoomLayer PreferredTreeLayer { get; private set; }
	public RoomLayer SecondaryTreeLayer { get; private set; }
	public RoomLayer RefugeLayer { get; private set; }
	public bool ActivitySleepEnabled { get; private set; }
	public string ActivityRestEmote { get; private set; } = string.Empty;
	public bool EcologyShelterEnabled { get; private set; }
	public bool EcologySeasonalEnabled { get; private set; }
	public bool EcologyNestingEnabled { get; private set; }
	public bool EcologyParentingEnabled { get; private set; }
	public bool WillShareTerritory { get; private set; }
	public bool WillShareTerritoryWithOtherRaces { get; private set; }
	public bool AllowGroupShelterSharing { get; private set; }
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
		MovementStrategy = AnimalMovementStrategyType.Ground;
		HomeStrategy = AnimalHomeStrategyType.None;
		FeedingStrategy = AnimalFeedingStrategyType.None;
		WaterStrategy = AnimalWaterStrategyType.Drink;
		ThreatStrategy = AnimalThreatStrategyType.Passive;
		AwarenessStrategy = AnimalAwarenessStrategyType.None;
		RefugeStrategy = AnimalRefugeStrategyType.None;
		ActivityStrategy = AnimalActivityStrategyType.Always;
		DormancyMode = AnimalDormancyMode.Rest;
		SensesStrategy = AnimalSensesStrategyType.None;
		UseActiveNeeds = false;
		MovementRange = DefaultGroundRange;
		AmphibiousWaterBias = 0.50;
		WanderChancePerMinute = 0.33;
		WanderEmote = string.Empty;
		EngageDelayDiceExpression = "1000+1d1000";
		EngageEmote = string.Empty;
		PostureEmote = string.Empty;
		PostureDurationDiceExpression = "1d20+20";
		OrdinaryThreatResponse = AnimalThreatResponseType.Inherit;
		HungryPreyResponse = AnimalThreatResponseType.Inherit;
		AttackedThreatResponse = AnimalThreatResponseType.Inherit;
		TerritoryThreatResponse = AnimalThreatResponseType.Inherit;
		ParentingThreatResponse = AnimalThreatResponseType.Inherit;
		SeasonalThreatResponse = AnimalThreatResponseType.Inherit;
		AwarenessRange = 5;
		AwarenessMemoryMinutes = 10;
		RefugeReturnSeconds = 60;
		TargetFlyingLayer = RoomLayer.InAir;
		TargetRestingLayer = RoomLayer.HighInTrees;
		PreferredTreeLayer = RoomLayer.HighInTrees;
		SecondaryTreeLayer = RoomLayer.InTrees;
		RefugeLayer = RoomLayer.HighInTrees;
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
		WillShareTerritory = false;
		WillShareTerritoryWithOtherRaces = true;
		AllowGroupShelterSharing = false;

		if (Gameworld is not null)
		{
			MovementEnabledProg = Gameworld.AlwaysTrueProg;
			MovementCellProg = Gameworld.AlwaysTrueProg;
			PreferredHabitatProg = Gameworld.AlwaysTrueProg;
			ToleratedHabitatProg = Gameworld.AlwaysTrueProg;
			AmphibiousLandCellProg = Gameworld.AlwaysTrueProg;
			AmphibiousWaterCellProg = Gameworld.AlwaysTrueProg;
			AllowDescentProg = Gameworld.AlwaysFalseProg;
			SuitableTerritoryProg = Gameworld.AlwaysTrueProg;
			DesiredTerritorySizeProg = Gameworld.AlwaysOneProg;
			BurrowSiteProg = Gameworld.AlwaysTrueProg;
			BuildEnabledProg = Gameworld.AlwaysTrueProg;
			WillAttackProg = Gameworld.AlwaysFalseProg;
			AwarenessThreatProg = Gameworld.AlwaysFalseProg;
			AwarenessAvoidCellProg = Gameworld.AlwaysFalseProg;
			RefugeCellProg = Gameworld.AlwaysFalseProg;
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

		XElement movement = root.Element("Movement") ?? new XElement("Movement");
		MovementStrategy = ParseEnum(movement.Attribute("type")?.Value, AnimalMovementStrategyType.Ground);
		MovementRange = int.Parse(movement.Element("Range")?.Value ?? DefaultRangeFor(MovementStrategy).ToString());
		AmphibiousWaterBias = double.Parse(movement.Element("AmphibiousWaterBias")?.Value ?? "0.5");
		WanderChancePerMinute = double.Parse(movement.Element("WanderChancePerMinute")?.Value ?? "0.33");
		WanderEmote = movement.Element("WanderEmote")?.Value ?? string.Empty;
		TargetFlyingLayer = ParseEnum(movement.Element("TargetFlyingLayer")?.Value, RoomLayer.InAir);
		TargetRestingLayer = ParseEnum(movement.Element("TargetRestingLayer")?.Value, RoomLayer.HighInTrees);
		PreferredTreeLayer = ParseEnum(movement.Element("PreferredTreeLayer")?.Value, RoomLayer.HighInTrees);
		SecondaryTreeLayer = ParseEnum(movement.Element("SecondaryTreeLayer")?.Value, RoomLayer.InTrees);
		MovementEnabledProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("MovementEnabledProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		MovementCellProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("MovementCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		PreferredHabitatProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("PreferredHabitatProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		ToleratedHabitatProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("ToleratedHabitatProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		AmphibiousLandCellProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("AmphibiousLandCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		AmphibiousWaterCellProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("AmphibiousWaterCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		AllowDescentProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("AllowDescentProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;

		XElement home = root.Element("Home") ?? new XElement("Home");
		HomeStrategy = ParseEnum(home.Attribute("type")?.Value, AnimalHomeStrategyType.None);
		SuitableTerritoryProg =
			Gameworld.FutureProgs.Get(long.Parse(home.Element("SuitableTerritoryProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		DesiredTerritorySizeProg =
			Gameworld.FutureProgs.Get(long.Parse(home.Element("DesiredTerritorySizeProg")?.Value ?? "0")) ??
			Gameworld.AlwaysOneProg;
		WillShareTerritory = bool.Parse(home.Element("WillShareTerritory")?.Value ?? "false");
		WillShareTerritoryWithOtherRaces =
			bool.Parse(home.Element("WillShareTerritoryWithOtherRaces")?.Value ?? "true");
		AllowGroupShelterSharing = bool.Parse(home.Element("AllowGroupShelterSharing")?.Value ?? "false");
		long craftId = long.Parse(home.Element("BurrowCraftId")?.Value ?? "0");
		BurrowCraft = craftId > 0 ? Gameworld.Crafts.Get(craftId) : null;
		BurrowSiteProg =
			Gameworld.FutureProgs.Get(long.Parse(home.Element("BurrowSiteProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		BuildEnabledProg =
			Gameworld.FutureProgs.Get(long.Parse(home.Element("BuildEnabledProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		long homeProgId = long.Parse(home.Element("HomeLocationProg")?.Value ?? "0");
		HomeLocationProg = homeProgId > 0 ? Gameworld.FutureProgs.Get(homeProgId) : null;
		long anchorProgId = long.Parse(home.Element("AnchorItemProg")?.Value ?? "0");
		AnchorItemProg = anchorProgId > 0 ? Gameworld.FutureProgs.Get(anchorProgId) : null;

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

		XElement awareness = root.Element("Awareness") ?? new XElement("Awareness");
		AwarenessStrategy = ParseEnum(awareness.Attribute("type")?.Value, AnimalAwarenessStrategyType.None);
		AwarenessThreatProg =
			Gameworld.FutureProgs.Get(long.Parse(awareness.Element("ThreatProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		AwarenessAvoidCellProg =
			Gameworld.FutureProgs.Get(long.Parse(awareness.Element("AvoidCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		AwarenessRange = int.Parse(awareness.Element("Range")?.Value ?? "5");
		AwarenessMemoryMinutes = int.Parse(awareness.Element("MemoryMinutes")?.Value ?? "10");
		SensesStrategy = ParseEnum(awareness.Element("Senses")?.Value, AnimalSensesStrategyType.None);

		XElement refuge = root.Element("Refuge") ?? new XElement("Refuge");
		RefugeStrategy = ParseEnum(refuge.Attribute("type")?.Value, AnimalRefugeStrategyType.None);
		RefugeLayer = ParseEnum(refuge.Element("Layer")?.Value, RoomLayer.HighInTrees);
		RefugeReturnSeconds = int.Parse(refuge.Element("ReturnSeconds")?.Value ?? "60");
		RefugeCellProg =
			Gameworld.FutureProgs.Get(long.Parse(refuge.Element("CellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;

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
			new XElement("Movement",
				new XAttribute("type", MovementStrategy),
				new XElement("Range", MovementRange),
				new XElement("AmphibiousWaterBias", AmphibiousWaterBias),
				new XElement("WanderChancePerMinute", WanderChancePerMinute),
				new XElement("WanderEmote", new XCData(WanderEmote)),
				new XElement("MovementEnabledProg", MovementEnabledProg?.Id ?? 0),
				new XElement("MovementCellProg", MovementCellProg?.Id ?? 0),
				new XElement("PreferredHabitatProg", PreferredHabitatProg?.Id ?? 0),
				new XElement("ToleratedHabitatProg", ToleratedHabitatProg?.Id ?? 0),
				new XElement("AmphibiousLandCellProg", AmphibiousLandCellProg?.Id ?? 0),
				new XElement("AmphibiousWaterCellProg", AmphibiousWaterCellProg?.Id ?? 0),
				new XElement("AllowDescentProg", AllowDescentProg?.Id ?? 0),
				new XElement("TargetFlyingLayer", TargetFlyingLayer),
				new XElement("TargetRestingLayer", TargetRestingLayer),
				new XElement("PreferredTreeLayer", PreferredTreeLayer),
				new XElement("SecondaryTreeLayer", SecondaryTreeLayer)),
			new XElement("Home",
				new XAttribute("type", HomeStrategy),
				new XElement("SuitableTerritoryProg", SuitableTerritoryProg?.Id ?? 0),
				new XElement("DesiredTerritorySizeProg", DesiredTerritorySizeProg?.Id ?? 0),
				new XElement("WillShareTerritory", WillShareTerritory),
				new XElement("WillShareTerritoryWithOtherRaces", WillShareTerritoryWithOtherRaces),
				new XElement("AllowGroupShelterSharing", AllowGroupShelterSharing),
				new XElement("BurrowCraftId", BurrowCraft?.Id ?? 0),
				new XElement("BurrowSiteProg", BurrowSiteProg?.Id ?? 0),
				new XElement("BuildEnabledProg", BuildEnabledProg?.Id ?? 0),
				new XElement("HomeLocationProg", HomeLocationProg?.Id ?? 0),
				new XElement("AnchorItemProg", AnchorItemProg?.Id ?? 0)),
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
			new XElement("Awareness",
				new XAttribute("type", AwarenessStrategy),
				new XElement("ThreatProg", AwarenessThreatProg?.Id ?? 0),
				new XElement("AvoidCellProg", AwarenessAvoidCellProg?.Id ?? 0),
				new XElement("Range", AwarenessRange),
				new XElement("MemoryMinutes", AwarenessMemoryMinutes),
				new XElement("Senses", SensesStrategy)),
			new XElement("Refuge",
				new XAttribute("type", RefugeStrategy),
				new XElement("Layer", RefugeLayer),
				new XElement("CellProg", RefugeCellProg?.Id ?? 0),
				new XElement("ReturnSeconds", RefugeReturnSeconds)),
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
		return ValidateConfiguration(HomeStrategy, FeedingStrategy, ThreatStrategy, MovementStrategy,
			RefugeStrategy, ActivityStrategy, _activeTimesOfDay, WaterStrategy,
			!ReferenceEquals(AmphibiousWaterCellProg, Gameworld.AlwaysFalseProg),
			AwarenessStrategy, EcologyNestingEnabled, !ReferenceEquals(NestSiteProg, Gameworld.AlwaysFalseProg),
			EcologyParentingEnabled, !ReferenceEquals(ProtectProg, Gameworld.AlwaysFalseProg));
	}

	private static TEnum ParseEnum<TEnum>(string? text, TEnum fallback) where TEnum : struct
	{
		return !string.IsNullOrWhiteSpace(text) && Enum.TryParse(text, true, out TEnum value)
			? value
			: fallback;
	}

	private static int DefaultRangeFor(AnimalMovementStrategyType strategy)
	{
		return strategy switch
		{
			AnimalMovementStrategyType.Swim => DefaultSwimRange,
			AnimalMovementStrategyType.Fly => DefaultFlyRange,
			AnimalMovementStrategyType.Arboreal => DefaultArborealRange,
			AnimalMovementStrategyType.Amphibious => DefaultSwimRange,
			_ => DefaultGroundRange
		};
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

	private IAnimalMovementStrategy MovementStrategyHandler => MovementStrategy switch
	{
		AnimalMovementStrategyType.Swim => SwimmingMovementStrategy.Instance,
		AnimalMovementStrategyType.Fly => FlyingMovementStrategy.Instance,
		AnimalMovementStrategyType.Arboreal => ArborealMovementStrategy.Instance,
		AnimalMovementStrategyType.Amphibious => AmphibiousMovementStrategy.Instance,
		_ => GroundMovementStrategy.Instance
	};

	private IAnimalHomeStrategy HomeStrategyHandler => HomeStrategy switch
	{
		AnimalHomeStrategyType.Territorial => TerritorialHomeStrategy.Instance,
		AnimalHomeStrategyType.Denning => DenningHomeStrategy.Instance,
		_ => NoHomeStrategy.Instance
	};

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

	private IAnimalAwarenessStrategy AwarenessStrategyHandler => AwarenessStrategy switch
	{
		AnimalAwarenessStrategyType.Wary => WaryAwarenessStrategy.Instance,
		AnimalAwarenessStrategyType.Wimpy => WimpyAwarenessStrategy.Instance,
		AnimalAwarenessStrategyType.Skittish => SkittishAwarenessStrategy.Instance,
		AnimalAwarenessStrategyType.Guarding => GuardingAwarenessStrategy.Instance,
		_ => NoAwarenessStrategy.Instance
	};

	private IAnimalRefugeStrategy RefugeStrategyHandler => RefugeStrategy switch
	{
		AnimalRefugeStrategyType.Home => HomeRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Den => DenRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Trees => TreesRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Sky => SkyRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Water => WaterRefugeStrategy.Instance,
		AnimalRefugeStrategyType.Prog => ProgRefugeStrategy.Instance,
		_ => NoRefugeStrategy.Instance
	};

	private IAnimalActivityStrategy ActivityStrategyHandler => ActivityStrategy == AnimalActivityStrategyType.Always
		? AlwaysActivityStrategy.Instance
		: TimedActivityStrategy.Instance;

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
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
	#3ecology protect <prog>#0 - sets protected young or friends";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
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

	private bool BuildingCommandMovement(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "ground":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Ground);
			case "swim":
			case "swimming":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Swim);
			case "fly":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Fly);
			case "arboreal":
			case "tree":
			case "trees":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Arboreal);
			case "amphibious":
			case "amphibian":
				return SetMovementStrategy(actor, AnimalMovementStrategyType.Amphibious);
			case "range":
				return BuildingCommandMovementRange(actor, command);
			case "waterbias":
			case "water":
			case "bias":
				return BuildingCommandMovementWaterBias(actor, command);
			case "chance":
				return BuildingCommandMovementChance(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandMovementEnabledProg(actor, command);
			case "room":
			case "roomprog":
			case "cell":
			case "cellprog":
				return BuildingCommandMovementCellProg(actor, command);
			case "preferredhabitat":
			case "preferhabitat":
				return BuildingCommandMovementHabitatProg(actor, command, x => PreferredHabitatProg = x,
					"preferred habitat");
			case "toleratedhabitat":
			case "toleratehabitat":
			case "tolerated":
				return BuildingCommandMovementHabitatProg(actor, command, x => ToleratedHabitatProg = x,
					"tolerated transit habitat");
			case "landprog":
			case "land":
				return BuildingCommandAmphibiousCellProg(actor, command, x => AmphibiousLandCellProg = x, "land");
			case "waterprog":
			case "watercell":
				return BuildingCommandAmphibiousCellProg(actor, command, x => AmphibiousWaterCellProg = x, "water");
			case "flying":
			case "flyinglayer":
				return BuildingCommandLayer(actor, command, x => TargetFlyingLayer = x, "flying travel");
			case "resting":
			case "restinglayer":
				return BuildingCommandLayer(actor, command, x => TargetRestingLayer = x, "resting");
			case "preferred":
			case "preferredlayer":
				return BuildingCommandTreeLayer(actor, command, x => PreferredTreeLayer = x, "preferred");
			case "secondary":
			case "secondarylayer":
				return BuildingCommandTreeLayer(actor, command, x => SecondaryTreeLayer = x, "secondary");
			case "descent":
			case "descentprog":
				return BuildingCommandDescentProg(actor, command);
			case "emote":
			case "wander":
				return BuildingCommandMovementEmote(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool SetMovementStrategy(ICharacter actor, AnimalMovementStrategyType strategy)
	{
		MovementStrategy = strategy;
		MovementRange = DefaultRangeFor(strategy);
		Changed = true;
		actor.OutputHandler.Send(
			$"This animal AI will now use {strategy.DescribeEnum().ColourName()} movement with a range of {MovementRange.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMovementRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 1)
		{
			actor.OutputHandler.Send("You must specify a positive whole number for the movement range.");
			return false;
		}

		MovementRange = value;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now search up to {value.ToString("N0", actor).ColourValue()} cells for movement targets.");
		return true;
	}

	private bool BuildingCommandMovementChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !TerritorialWanderer.TryParseWanderChance(command.SafeRemainingArgument, out double value))
		{
			actor.OutputHandler.Send("You must specify a percentage between 0% and 100%.");
			return false;
		}

		WanderChancePerMinute = value;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now have a {value.ToString("P2", actor).ColourValue()} ambient movement chance each minute.");
		return true;
	}

	private bool BuildingCommandMovementWaterBias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !TerritorialWanderer.TryParseWanderChance(command.SafeRemainingArgument, out double value))
		{
			actor.OutputHandler.Send("You must specify a percentage between 0% and 100%.");
			return false;
		}

		AmphibiousWaterBias = value;
		Changed = true;
		actor.OutputHandler.Send($"Amphibious ambient movement will now prefer water {value.ToString("P2", actor).ColourValue()} of the time.");
		return true;
	}

	private bool BuildingCommandMovementEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether ambient movement is enabled?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MovementEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to control ambient movement.");
		return true;
	}

	private bool BuildingCommandAmphibiousCellProg(ICharacter actor, StringStack command, Action<IFutureProg> setter, string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should evaluate amphibious {label} cells?");
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
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for amphibious {label} cells.");
		return true;
	}

	private bool BuildingCommandMovementCellProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should evaluate ambient movement target cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MovementCellProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for ambient movement targets.");
		return true;
	}

	private bool BuildingCommandLayer(ICharacter actor, StringStack command, Action<RoomLayer> setter, string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a room layer. Valid values are {Enum.GetValues<RoomLayer>().ListToColouredString()}.");
			return false;
		}

		string valueText = command.SafeRemainingArgument;
		if (!valueText.TryParseEnum(out RoomLayer value))
		{
			actor.OutputHandler.Send($"The text {valueText.ColourCommand()} is not a valid room layer. Valid values are {Enum.GetValues<RoomLayer>().ListToColouredString()}.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.OutputHandler.Send($"The {label} layer is now {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTreeLayer(ICharacter actor, StringStack command, Action<RoomLayer> setter, string label)
	{
		RoomLayer[] validLayers = [RoomLayer.InTrees, RoomLayer.HighInTrees];
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a tree layer. Valid values are {validLayers.ListToColouredString()}.");
			return false;
		}

		string valueText = command.SafeRemainingArgument;
		if (!valueText.TryParseEnum(out RoomLayer value) || !validLayers.Contains(value))
		{
			actor.OutputHandler.Send($"The text {valueText.ColourCommand()} is not a valid tree layer. Valid values are {validLayers.ListToColouredString()}.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.OutputHandler.Send($"The {label} tree layer is now {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDescentProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide when arboreal movement may descend?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AllowDescentProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to gate arboreal descent.");
		return true;
	}

	private bool BuildingCommandMovementEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What movement emote should this animal use?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			WanderEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This animal AI will no longer use a movement emote.");
			return true;
		}

		WanderEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {WanderEmote.ColourCommand()} as its movement emote.");
		return true;
	}

	private bool BuildingCommandHome(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
				return SetHomeStrategy(actor, AnimalHomeStrategyType.None);
			case "territorial":
				return SetHomeStrategy(actor, AnimalHomeStrategyType.Territorial);
			case "denning":
			case "den":
			case "burrow":
				return SetHomeStrategy(actor, AnimalHomeStrategyType.Denning);
			case "territoryprog":
			case "territory":
				return BuildingCommandTerritoryProg(actor, command);
			case "size":
			case "sizeprog":
				return BuildingCommandTerritorySizeProg(actor, command);
			case "share":
				return ToggleShareTerritory(actor);
			case "shareother":
			case "shareothers":
				return ToggleShareOtherTerritory(actor);
			case "shareshelter":
			case "sharegroup":
				return ToggleShareGroupShelter(actor);
			case "craft":
			case "burrowcraft":
				return BuildingCommandBurrowCraft(actor, command);
			case "site":
			case "siteprog":
			case "burrowsite":
				return BuildingCommandBurrowSiteProg(actor, command);
			case "location":
			case "locationprog":
			case "homeprog":
				return BuildingCommandHomeLocationProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandBuildEnabledProg(actor, command);
			case "anchor":
			case "anchorprog":
				return BuildingCommandAnchorProg(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool SetHomeStrategy(ICharacter actor, AnimalHomeStrategyType strategy)
	{
		HomeStrategy = strategy;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {strategy.DescribeEnum().ColourName()} home behavior.");
		return true;
	}

	private bool BuildingCommandTerritoryProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide suitable territory cells?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new[] { ProgVariableTypes.Location },
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Character },
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		SuitableTerritoryProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for territory suitability.");
		return true;
	}

	private bool BuildingCommandTerritorySizeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide desired territory size?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Number, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		DesiredTerritorySizeProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for territory size.");
		return true;
	}

	private bool ToggleShareTerritory(ICharacter actor)
	{
		WillShareTerritory = !WillShareTerritory;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will {WillShareTerritory.NowNoLonger()} share territory with others of its race.");
		return true;
	}

	private bool ToggleShareOtherTerritory(ICharacter actor)
	{
		WillShareTerritoryWithOtherRaces = !WillShareTerritoryWithOtherRaces;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will {WillShareTerritoryWithOtherRaces.NowNoLonger()} share territory with other races.");
		return true;
	}

	private bool BuildingCommandBurrowCraft(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which craft should this AI use to build its burrow? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			BurrowCraft = null;
			Changed = true;
			actor.OutputHandler.Send("This animal AI will no longer use a burrow craft.");
			return true;
		}

		ICraft? craft = Gameworld.Crafts.GetByIdOrName(command.SafeRemainingArgument);
		if (craft is null)
		{
			actor.OutputHandler.Send("There is no such craft.");
			return false;
		}

		BurrowCraft = craft;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {craft.Name.ColourName()} to build its burrow.");
		return true;
	}

	private bool BuildingCommandBurrowSiteProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether a cell is suitable for a burrow?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BurrowSiteProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for burrow sites.");
		return true;
	}

	private bool BuildingCommandHomeLocationProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should return the fallback home location? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			HomeLocationProg = null;
			Changed = true;
			actor.OutputHandler.Send("This animal AI will no longer use a fallback home-location prog.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Location, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		HomeLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} as its fallback home source.");
		return true;
	}

	private bool BuildingCommandBuildEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide whether burrow building is enabled?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BuildEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to gate burrow building.");
		return true;
	}

	private bool BuildingCommandAnchorProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify the completed burrow anchor? Use #3clear#0 to remove it."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			AnchorItemProg = null;
			Changed = true;
			actor.OutputHandler.Send("This animal AI will use fallback burrow-anchor detection.");
			return true;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AnchorItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to identify burrow anchors.");
		return true;
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

	private bool BuildingCommandEngageDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send("You must supply a valid dice expression for a number of milliseconds.");
			return false;
		}

		EngageDelayDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before engaging.");
		return true;
	}

	private bool BuildingCommandEngageEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either supply an emote or use #3clear#0 to remove the emote."
			                         .SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			EngageEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This animal AI will no longer use an engage emote.");
			return true;
		}

		Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EngageEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use this engage emote:\n{EngageEmote.ColourCommand()}");
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

	private bool BuildingCommandAwareness(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.None);
			case "wary":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Wary);
			case "wimpy":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Wimpy);
			case "skittish":
			case "skittishbird":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Skittish);
			case "guarding":
			case "guard":
				return SetAwarenessStrategy(actor, AnimalAwarenessStrategyType.Guarding);
			case "senses":
			case "sense":
				return BuildingCommandAwarenessSenses(actor, command);
			case "threat":
			case "threatprog":
				return BuildingCommandAwarenessThreatProg(actor, command);
			case "avoid":
			case "avoidprog":
			case "cell":
			case "cellprog":
				return BuildingCommandAwarenessAvoidProg(actor, command);
			case "range":
				return BuildingCommandAwarenessRange(actor, command);
			case "memory":
				return BuildingCommandAwarenessMemory(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool SetAwarenessStrategy(ICharacter actor, AnimalAwarenessStrategyType strategy)
	{
		AwarenessStrategy = strategy;
		if (strategy.In(AnimalAwarenessStrategyType.Wimpy, AnimalAwarenessStrategyType.Skittish) &&
		    ThreatStrategy == AnimalThreatStrategyType.Passive)
		{
			ThreatStrategy = AnimalThreatStrategyType.Flee;
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {strategy.DescribeEnum().ColourName()} awareness behavior.");
		return true;
	}

	private bool BuildingCommandAwarenessThreatProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify feared or disliked characters?");
			return false;
		}

		IFutureProg? prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AwarenessThreatProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to identify awareness threats.");
		return true;
	}

	private bool BuildingCommandAwarenessAvoidProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify cells this animal avoids?");
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

		AwarenessAvoidCellProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to avoid cells.");
		return true;
	}

	private bool BuildingCommandAwarenessRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
		{
			actor.OutputHandler.Send("You must specify a non-negative number of rooms.");
			return false;
		}

		AwarenessRange = value;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will notice awareness threats within {value.ToString("N0", actor).ColourValue()} rooms.");
		return true;
	}

	private bool BuildingCommandAwarenessMemory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
		{
			actor.OutputHandler.Send("You must specify a non-negative number of minutes.");
			return false;
		}

		AwarenessMemoryMinutes = value;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will remember threat locations for {value.ToString("N0", actor).ColourValue()} minutes.");
		return true;
	}

	private bool BuildingCommandRefuge(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.None);
			case "home":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Home);
			case "den":
			case "burrow":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Den);
			case "trees":
			case "tree":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Trees);
			case "sky":
			case "air":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Sky);
			case "water":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Water);
			case "prog":
				return SetRefugeStrategy(actor, AnimalRefugeStrategyType.Prog);
			case "layer":
				return BuildingCommandLayer(actor, command, x => RefugeLayer = x, "refuge");
			case "cell":
			case "cellprog":
				return BuildingCommandRefugeCellProg(actor, command);
			case "return":
			case "returndelay":
				return BuildingCommandRefugeReturn(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool SetRefugeStrategy(ICharacter actor, AnimalRefugeStrategyType strategy)
	{
		RefugeStrategy = strategy;
		if (strategy == AnimalRefugeStrategyType.Sky)
		{
			MovementStrategy = AnimalMovementStrategyType.Fly;
			RefugeLayer = TargetFlyingLayer;
		}
		else if (strategy == AnimalRefugeStrategyType.Trees)
		{
			MovementStrategy = AnimalMovementStrategyType.Arboreal;
			RefugeLayer = PreferredTreeLayer;
		}
		else if (strategy == AnimalRefugeStrategyType.Den && HomeStrategy == AnimalHomeStrategyType.None)
		{
			HomeStrategy = AnimalHomeStrategyType.Denning;
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {strategy.DescribeEnum().ColourName()} refuge behavior.");
		return true;
	}

	private bool BuildingCommandRefugeCellProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should identify refuge cells?");
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

		RefugeCellProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} to identify refuge cells.");
		return true;
	}

	private bool BuildingCommandRefugeReturn(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
		{
			actor.OutputHandler.Send("You must specify a non-negative number of seconds.");
			return false;
		}

		RefugeReturnSeconds = value;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will wait {value.ToString("N0", actor).ColourValue()} seconds before returning from refuge behavior.");
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

	private static ICharacter? CharacterForEvent(EventType type, dynamic[] arguments)
	{
		return type switch
		{
			EventType.CharacterEnterCellWitness => arguments[3] as ICharacter,
			EventType.CharacterDiesWitness => arguments[1] as ICharacter,
			EventType.EngagedInCombat => arguments[1] as ICharacter,
			_ => arguments.Length > 0 ? arguments[0] as ICharacter : null
		};
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (EventType type in types)
		{
			switch (type)
			{
				case EventType.CharacterEntersGame:
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

	private bool SurvivalNeedsSatisfied(ICharacter character)
	{
		return !FeedingStrategyHandler.IsHungry(this, character) &&
		       !WaterStrategyHandler.IsThirsty(this, character);
	}

	private void EvaluateHomeAndTerritory(ICharacter character)
	{
		HomeStrategyHandler.Evaluate(this, character);
	}

	private TimeSpan AwarenessMemory => TimeSpan.FromMinutes(AwarenessMemoryMinutes);

	private int EffectiveAwarenessRange => SensesStrategy == AnimalSensesStrategyType.Vigilant
		? AwarenessRange + 2
		: AwarenessRange;

	private bool IsGroupControlled(ICharacter character, GroupAIControlScope scope)
	{
		return character is INPC npc &&
		       npc.GroupAI?.GroupAIType is IGroupAIControlPolicy policy &&
		       policy.ControlScope.HasFlag(scope);
	}

	private bool IsSociallyTrusted(ICharacter character, ICharacter target)
	{
		if (ReferenceEquals(character, target) || character.Race.SameRace(target.Race))
		{
			return true;
		}

		return character is INPC npc &&
		       npc.GroupAI?.GroupMembers.ContainsPhysicalInstance(target) == true;
	}

	private bool IsWithinToleratedHabitat(ICharacter character, ICell cell)
	{
		return ToleratedHabitatProg.ExecuteBool(false, character, cell, character.Location);
	}

	private bool IsWithinPreferredHabitat(ICharacter character, ICell cell)
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
	private bool IsSuitableTerritory(ICharacter character, ICell cell)
	{
		if (SuitableTerritoryProg.MatchesParameters(
			    new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }))
		{
			return SuitableTerritoryProg.ExecuteBool(false, character, cell);
		}

		return SuitableTerritoryProg.ExecuteBool(false, cell, character);
	}

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

	private Func<ICellExit, bool> GetAnimalSuitabilityFunction(ICharacter character, bool ignoreSafeMovement = false)
	{
		Func<ICellExit, bool> baseSuitability = base.GetSuitabilityFunction(character, ignoreSafeMovement);
		return exit => baseSuitability(exit) &&
		               MovementStrategyHandler.CellMatches(this, character, exit.Destination) &&
		               IsWithinToleratedHabitat(character, exit.Destination) &&
		               !ShouldAvoidCell(character, exit.Destination);
	}

	private IEnumerable<ICharacter> VisibleAwarenessThreats(ICharacter character, ICharacter? witnessedTarget)
	{
		HashSet<ICharacter> threats = new();
		if (witnessedTarget is not null &&
		    !IsSociallyTrusted(character, witnessedTarget) &&
		    AwarenessThreatProg.ExecuteBool(false, character, witnessedTarget) &&
		    character.CanSee(witnessedTarget))
		{
			threats.Add(witnessedTarget);
		}

		IEnumerable<ICharacter> candidates = character.Location
		                                              .LayerCharacters(character.RoomLayer)
		                                              .Concat(EffectiveAwarenessRange > 0
		                                              ? character.Location.CellsInVicinity((uint)EffectiveAwarenessRange, true, true)
			                                                         .SelectMany(x => x.Characters)
			                                              : Enumerable.Empty<ICharacter>());

		foreach (ICharacter target in candidates.Distinct())
		{
			if (IsSociallyTrusted(character, target) ||
			    !character.CanSee(target) ||
			    !AwarenessThreatProg.ExecuteBool(false, character, target))
			{
				continue;
			}

			threats.Add(target);
		}

		return threats;
	}

	private bool ShouldAvoidCell(ICharacter character, ICell cell)
	{
		if (AwarenessStrategy == AnimalAwarenessStrategyType.None)
		{
			return false;
		}

		if (AwarenessAvoidCellProg.ExecuteBool(false, character, cell, character.Location))
		{
			return true;
		}

		return NpcKnownThreatLocationsEffect.Get(character)?.Knows(cell, AwarenessMemory) == true;
	}

	private void RememberThreats(ICharacter character, IEnumerable<ICharacter> threats)
	{
		List<ICell> cells = threats
		                    .Select(x => x.Location)
		                    .WhereNotNull(x => x)
		                    .Distinct()
		                    .ToList();
		if (!cells.Any())
		{
			return;
		}

		NpcKnownThreatLocationsEffect memory = NpcKnownThreatLocationsEffect.GetOrCreate(character);
		foreach (ICell cell in cells)
		{
			memory.Remember(cell);
		}
	}

	private bool TryAwarenessResponse(ICharacter character, ICharacter? witnessedTarget)
	{
		if (IsGroupControlled(character, GroupAIControlScope.Senses) ||
		    character.Combat is not null ||
		    character.Movement is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement")))
		{
			return false;
		}

		return AwarenessStrategyHandler.TryRespond(this, character, witnessedTarget);
	}

	private void HandleCombatAwareness(ICharacter character)
	{
		if (!AwarenessStrategy.In(AnimalAwarenessStrategyType.Wimpy, AnimalAwarenessStrategyType.Skittish))
		{
			return;
		}

		character.CombatStrategyMode = CombatStrategyMode.Flee;
		if (character.CombatTarget is ICharacter target)
		{
			NpcKnownThreatLocationsEffect.GetOrCreate(character).Remember(target.Location);
		}
	}

	private bool TryMoveAwayFromAwarenessThreats(ICharacter character, IEnumerable<ICharacter> threats)
	{
		List<ICell> threatCells = threats.Select(x => x.Location).Distinct().ToList();
		ICellExit? exit = character.Location.ExitsFor(character)
		                           .Where(GetAnimalSuitabilityFunction(character))
			.Where(x => !threatCells.Contains(x.Destination))
			.Where(x => !x.Destination.Characters.Any(y =>
			                           !IsSociallyTrusted(character, y) &&
			                           AwarenessThreatProg.ExecuteBool(false, character, y)))
		                           .GetRandomElement();
		return exit is not null && character.CanMove(exit) && character.Move(exit);
	}

	private bool TryMoveToRefuge(ICharacter character)
	{
		if (TryMoveToRefugeLayer(character))
		{
			return true;
		}

		(ICell? target, IEnumerable<ICellExit> path) = RefugeStrategyHandler.GetPath(this, character);
		List<ICellExit> exits = path.ToList();
		if (target is null || !exits.Any())
		{
			return false;
		}

		FollowingPath effect = CreatePathingEffect(character, exits);
		character.AddEffect(effect);
		effect.FollowPathAction();
		return true;
	}

	private bool TryMoveToRefugeLayer(ICharacter character)
	{
		if (!RefugeStrategy.In(AnimalRefugeStrategyType.Sky, AnimalRefugeStrategyType.Trees) ||
		    character.RoomLayer == RefugeLayer)
		{
			return false;
		}

		if (RefugeStrategy == AnimalRefugeStrategyType.Trees &&
		    !ArborealWandererAI.CellSupportsTreeLayers(character, character.Location))
		{
			return false;
		}

		FollowingMultiLayerPath effect = new(character, Enumerable.Empty<ICellExit>(), RefugeLayer, RefugeLayer);
		character.AddEffect(effect);
		effect.FollowPathAction();
		return true;
	}

	private bool IsAtRefuge(ICharacter character)
	{
		return RefugeStrategy switch
		{
			AnimalRefugeStrategyType.None => true,
			AnimalRefugeStrategyType.Home => ResolveHomeBase(character).HomeCell is ICell home &&
			                                  ReferenceEquals(home, character.Location),
			AnimalRefugeStrategyType.Den => ResolveHomeBase(character).HomeCell is ICell home &&
			                                 ReferenceEquals(home, character.Location),
			AnimalRefugeStrategyType.Trees => ArborealWandererAI.CellSupportsTreeLayers(character, character.Location) &&
			                                  character.RoomLayer.In(RoomLayer.InTrees, RoomLayer.HighInTrees),
			AnimalRefugeStrategyType.Sky => character.RoomLayer == RefugeLayer,
			AnimalRefugeStrategyType.Water => WaterStrategy == AnimalWaterStrategyType.Drink
				? NpcSurvivalAIHelpers.HasLocalWaterSource(character)
				: NpcSurvivalAIHelpers.HasAquaticWaterSource(character, character.Location,
					WaterStrategy == AnimalWaterStrategyType.Surface),
			AnimalRefugeStrategyType.Prog => RefugeCellProg.ExecuteBool(false, character, character.Location),
			_ => true
		};
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetRefugePath(ICharacter character)
	{
		if (IsAtRefuge(character))
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		return RefugeStrategyHandler.GetPath(this, character);
	}

	private bool ShouldReturnToRefuge(ICharacter character)
	{
		return RefugeStrategy != AnimalRefugeStrategyType.None &&
		       SurvivalNeedsSatisfied(character) &&
		       !IsAtRefuge(character);
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetAvoidancePath(ICharacter character)
	{
		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
			x => x is ICell cell &&
			     !ShouldAvoidCell(character, cell) &&
			     !cell.Characters.Any(y => AwarenessThreatProg.ExecuteBool(false, character, y)),
			(uint)Math.Max(1, AwarenessRange),
			GetAnimalSuitabilityFunction(character));
		return targetPath.Item1 is ICell target && targetPath.Item2.Any()
			? (target, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
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

		if (SensesStrategy.In(AnimalSensesStrategyType.Hiding, AnimalSensesStrategyType.Stalking) &&
		    !character.AffectedBy<ISneakEffect>())
		{
			character.AddEffect(new Sneak(character));
			return true;
		}

		if (SensesStrategy == AnimalSensesStrategyType.Hiding &&
		    !character.AffectedBy<IHideEffect>() &&
		    character.Location.CharactersInSpatialVicinity(character).Except(character).Any(x => character.CanSee(x)))
		{
			character.ExecuteCommand("hide");
			return true;
		}

		return false;
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

		foreach (ICharacter threat in VisibleEcologyCharacters(character).Except(protectedTargets).Shuffle())
		{
			if (IsSociallyTrusted(character, threat) ||
			    !IsParentingThreat(character, threat))
			{
				continue;
			}

			AnimalThreatResponseType response = ResolveThreatResponse(character, threat);
			if (response != AnimalThreatResponseType.Inherit)
			{
				return TryApplyThreatResponse(character, threat, response);
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
		return character.Location
		                .LayerCharacters(character.RoomLayer)
		                .Concat(AwarenessRange > 0
			                ? character.Location.CellsInVicinity((uint)AwarenessRange, true, true)
			                           .SelectMany(x => x.Characters)
				                : Enumerable.Empty<ICharacter>())
		                .Where(x => !ReferenceEquals(character, x) &&
		                            (includeSociallyTrusted || !IsSociallyTrusted(character, x)) &&
		                            character.CanSee(x))
		                .Distinct();
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
		IEnumerable<ICharacter> local = character.Location
			.LayerCharacters(character.RoomLayer)
			.Concat(EffectiveAwarenessRange > 0
				? character.Location.CellsInVicinity((uint)EffectiveAwarenessRange, true, true)
					.SelectMany(x => x.Characters)
				: Enumerable.Empty<ICharacter>());
		if (witnessedTarget is not null)
		{
			local = local.Append(witnessedTarget);
		}

		return local
			.DistinctPhysicalInstances()
			.Where(x => !IsSociallyTrusted(character, x))
			.Where(x => character.CanSee(x))
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

	private AnimalThreatResponseType ResolveThreatResponse(ICharacter character, ICharacter target)
	{
		if ((ReferenceEquals(character.CombatTarget, target) || ReferenceEquals(target.CombatTarget, character)) &&
		    AttackedThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return AttackedThreatResponse;
		}

		if (HasProtectedYoung(character) && IsParentingThreat(character, target) &&
		    ParentingThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return ParentingThreatResponse;
		}

		if (HomeStrategyHandler.IsDefendingLocation(this, character) &&
		    TerritoryThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return TerritoryThreatResponse;
		}

		if (IsAggressiveSeason(character) && SeasonalThreatResponse != AnimalThreatResponseType.Inherit)
		{
			return SeasonalThreatResponse;
		}

		if (PredatorAIHelpers.IsHungry(character) &&
		    WillAttackProg.ExecuteBool(false, character, target) &&
		    HungryPreyResponse != AnimalThreatResponseType.Inherit)
		{
			return HungryPreyResponse;
		}

		return OrdinaryThreatResponse;
	}

	private void EmitPosture(ICharacter character, ICharacter target)
	{
		if (string.IsNullOrWhiteSpace(PostureEmote))
		{
			return;
		}

		Emote emote = new(PostureEmote, character, character, target);
		if (emote.Valid)
		{
			character.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.InnerWrap));
		}
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
				.Where(x => !IsSociallyTrusted(character, x) && character.CanSee(x))
				.ToList();
			if (!activeTargets.Any())
			{
				return (0.0, false, TimeSpan.Zero);
			}

			if (threat >= 1.0)
			{
				foreach (ICharacter activeTarget in activeTargets.Shuffle())
				{
					AnimalThreatResponseType response = ResolveThreatResponse(character, activeTarget);
					if (response == AnimalThreatResponseType.Attack &&
					    PredatorAIHelpers.CheckForAttack(character, activeTarget, WillAttackProg,
						    EngageDelayDiceExpression, EngageEmote, false))
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

	private bool ToggleShareGroupShelter(ICharacter actor)
	{
		AllowGroupShelterSharing = !AllowGroupShelterSharing;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will {AllowGroupShelterSharing.NowNoLonger()} share claimed wildlife shelters with its live group.");
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

	private bool BuildingCommandAwarenessSenses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out AnimalSensesStrategyType strategy))
		{
			actor.OutputHandler.Send($"You must specify an animal senses strategy. Valid values are {Enum.GetValues<AnimalSensesStrategyType>().ListToColouredString()}.");
			return false;
		}

		SensesStrategy = strategy;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use {strategy.DescribeEnum().ColourName()} senses.");
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

	private bool BuildingCommandThreatPostureEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must supply a posture emote, or use #3clear#0 to remove it. $0 is the animal and $1 is its target.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			PostureEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This animal AI will no longer emit a posture emote.");
			return true;
		}

		Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		PostureEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This animal AI will now use the posture emote {PostureEmote.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandThreatPostureDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send("You must specify a valid dice expression for posture duration in seconds.");
			return false;
		}

		PostureDurationDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Postures will now last {PostureDurationDiceExpression.ColourValue()} seconds before escalating.");
		return true;
	}

	private bool BuildingCommandMovementHabitatProg(ICharacter actor, StringStack command, Action<IFutureProg> setter,
		string label)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should identify {label} cells?");
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
		actor.OutputHandler.Send($"This animal AI will now use {prog.MXPClickableFunctionName()} for {label} cells.");
		return true;
	}

	private bool TryContextualThreatResponse(ICharacter character, ICharacter? witnessedTarget)
	{
		List<ICharacter> candidates = ContextualThreatCandidates(character, witnessedTarget).ToList();
		if (SensesStrategy == AnimalSensesStrategyType.Tracking)
		{
			RememberThreats(character, candidates);
		}

		foreach (ICharacter target in candidates.Shuffle())
		{
			AnimalThreatResponseType response = ResolveThreatResponse(character, target);
			if (response == AnimalThreatResponseType.Inherit)
			{
				continue;
			}

			return TryApplyThreatResponse(character, target, response);
		}

		return false;
	}

	private bool TryApplyThreatResponse(ICharacter character, ICharacter target,
		AnimalThreatResponseType response)
	{
		return response switch
		{
			AnimalThreatResponseType.Ignore => true,
			AnimalThreatResponseType.Avoid => character.Combat is null &&
				TryMoveAwayFromAwarenessThreats(character, new[] { target }),
			AnimalThreatResponseType.Flee when character.Combat is not null => SetFleeCombatStrategy(character),
			AnimalThreatResponseType.Flee => TryFlee(character, target),
			AnimalThreatResponseType.Posture => BeginPosturing(character, target),
			AnimalThreatResponseType.Attack => PredatorAIHelpers.CheckForAttack(character, target, WillAttackProg,
				EngageDelayDiceExpression, EngageEmote, false),
			_ => false
		};
	}

	private static bool SetFleeCombatStrategy(ICharacter character)
	{
		character.CombatStrategyMode = CombatStrategyMode.Flee;
		return true;
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
		if (IsSociallyTrusted(character, target))
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
		       PredatorAIHelpers.CheckForAttack(character, target, WillAttackProg, EngageDelayDiceExpression,
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

	private NpcHomeBaseEffect ResolveHomeBase(ICharacter character)
	{
		NpcHomeBaseEffect home = NpcHomeBaseEffect.GetOrCreate(character);
		if (home.HomeCell is not null)
		{
			return home;
		}

		if (HomeLocationProg?.Execute<ICell?>(character) is ICell location)
		{
			home.SetHomeCell(location);
		}

		return home;
	}

	private void EvaluateBurrowLifecycle(ICharacter character)
	{
		if (character.Movement is not null ||
		    character.Combat is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("movement")) ||
		    character.EffectsOfType<IActiveCraftEffect>().Any(x => !ReferenceEquals(x.Component.Craft, BurrowCraft)))
		{
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

		if (!ReferenceEquals(home.HomeCell, character.Location))
		{
			CheckPathingEffect(character, true);
			return;
		}

		RefreshAnchorItem(character, home);
		if (home.AnchorItem is not null || BurrowCraft is null || !BuildEnabledProg.ExecuteBool(true, character))
		{
			return;
		}

		IActiveCraftGameItemComponent? interruptedCraft = character.Location!.LayerGameItems(character.RoomLayer)
			.SelectNotNull(x => x!.GetItemType<IActiveCraftGameItemComponent>())
			.FirstOrDefault(x => ReferenceEquals(x.Craft, BurrowCraft));
		if (interruptedCraft is not null)
		{
			(bool canResume, string _) = BurrowCraft.CanResumeCraft(character, interruptedCraft);
			if (canResume)
			{
				BurrowCraft.ResumeCraft(character, interruptedCraft);
			}

			return;
		}

		(bool canDoCraft, string _) = BurrowCraft.CanDoCraft(character, null, true, true);
		if (canDoCraft)
		{
			BurrowCraft.BeginCraft(character);
		}
	}

	private void RefreshAnchorItem(ICharacter character, NpcHomeBaseEffect home)
	{
		if (home.AnchorItem is not null && ReferenceEquals(home.AnchorItem.Location, home.HomeCell))
		{
			if (AnchorItemProg is null)
			{
				// Legacy AnimalAI XML had no explicit anchor policy. Retain its remembered
				// anchor without claiming an arbitrary item under the new shelter model.
				return;
			}

			if (AnchorItemProg.ExecuteBool(character, home.AnchorItem) &&
			    WildlifeShelterClaimEffect.ClaimOrRefresh(home.AnchorItem, character, AllowGroupShelterSharing))
			{
				return;
			}
		}

		home.ClearAnchorItem();
		IGameItem? anchor = DenBuilderAI.SelectAnchorItem(character, AnchorItemProg, AllowGroupShelterSharing);
		if (anchor is not null &&
		    (AnchorItemProg is null || WildlifeShelterClaimEffect.ClaimOrRefresh(anchor, character, AllowGroupShelterSharing)))
		{
			home.SetAnchorItem(anchor);
		}
	}

	private void EvaluateTerritory(ICharacter character)
	{
		Territory? territoryEffect = character.CombinedEffectsOfType<Territory>().FirstOrDefault();
		if (territoryEffect is null)
		{
			territoryEffect = new Territory(character);
			character.AddEffect(territoryEffect);
		}

		List<ICell> cells = territoryEffect.Cells.ToList();
		if (cells.Count >= DesiredTerritorySizeProg.ExecuteInt(0, character))
		{
			return;
		}

		ICollection<ICell> claimedTerritory = GetClaimedTerritory(character);
		if (cells.Count == 0)
		{
			if (IsSuitableTerritory(character, character.Location) &&
			    !claimedTerritory.Contains(character.Location))
			{
				territoryEffect.AddCell(character.Location);
				return;
			}

			(IPerceivable target, IEnumerable<ICellExit> _) = character.AcquireTargetAndPath(
				loc => loc is ICell candidate && IsSuitableTerritory(character, candidate) &&
				       !claimedTerritory.Contains(loc),
				20,
				GetAnimalSuitabilityFunction(character));
			if (target is ICell cell)
			{
				territoryEffect.AddCell(cell);
			}

			return;
		}

		foreach (ICell cell in territoryEffect.Cells)
		{
			ICell expand = cell
			               .ExitsFor(character, true)
				               .Where(x => IsSuitableTerritory(character, x.Destination) &&
			                           !claimedTerritory.Contains(x.Destination))
			               .Select(x => x.Destination)
			               .GetRandomElement();
			if (expand is not null && !territoryEffect.Cells.Contains(expand))
			{
				territoryEffect.AddCell(expand);
				return;
			}
		}
	}

	private ICollection<ICell> GetClaimedTerritory(ICharacter character)
	{
		if (WillShareTerritory)
		{
			return new List<ICell>();
		}

		IEnumerable<ICharacter> npcs = character.Gameworld.NPCs;
		if (WillShareTerritoryWithOtherRaces)
		{
			npcs = npcs.Where(x => !x.Race.SameRace(character.Race));
		}

		return npcs
		       .SelectNotNull(x => x!.CombinedEffectsOfType<Territory>().FirstOrDefault())
		       .SelectMany(x => x.Cells)
		       .Distinct()
		       .ToList();
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
		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = ch.AcquireTargetAndPath(
			x => x is ICharacter target && !IsSociallyTrusted(ch, target) &&
			     PredatorAIHelpers.WillAttack(ch, target, WillAttackProg, true),
			DefaultNeedRange,
			GetAnimalSuitabilityFunction(ch));
		return targetPath.Item1 is ICharacter prey && targetPath.Item2.Any()
			? (prey.Location, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
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

		foreach (IGameItem item in candidates.Shuffle())
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

	private (ICell? Target, IEnumerable<ICellExit> Path) GetBurrowHomePath(ICharacter ch)
	{
		NpcHomeBaseEffect home = ResolveHomeBase(ch);
		if (home.HomeCell is not null && !ReferenceEquals(home.HomeCell, ch.Location))
		{
			List<ICellExit> homePath = ch.PathBetween(home.HomeCell, DefaultNeedRange, GetAnimalSuitabilityFunction(ch)).ToList();
			return homePath.Any()
				? (home.HomeCell, homePath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		if (home.HomeCell is not null)
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

	private (ICell? Target, IEnumerable<ICellExit> Path) GetTerritoryPath(ICharacter ch)
	{
		Territory? territory = ch.CombinedEffectsOfType<Territory>().FirstOrDefault();
		if (territory is null)
		{
			territory = new Territory(ch);
			ch.AddEffect(territory);
		}

		if (!territory.Cells.Any())
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		if (!territory.Cells.Contains(ch.Location))
		{
			List<ICellExit> path = ch.PathBetween(territory.Cells.Cast<IPerceivable>(), DefaultNeedRange,
				GetAnimalSuitabilityFunction(ch, true)).ToList();
			return path.Any()
				? (path.Last().Destination, path)
				: (null, Enumerable.Empty<ICellExit>());
		}

		List<ICell> targets = territory.Cells
		                               .Where(x => !ReferenceEquals(x, ch.Location))
		                               .Where(x => MovementStrategyHandler.CellMatches(this, ch, x))
		                               .ToList();
		if (!targets.Any())
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		List<ICellExit> targetPath = ch.PathBetween(targets.Cast<IPerceivable>(), (uint)MovementRange,
			GetAnimalSuitabilityFunction(ch, true)).ToList();
		return targetPath.Any()
			? (targetPath.Last().Destination, targetPath)
			: (null, Enumerable.Empty<ICellExit>());
	}

	protected override FollowingPath CreatePathingEffect(ICharacter ch, IEnumerable<ICellExit> path)
	{
		return MovementStrategyHandler.CreatePathingEffect(this, ch, path);
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
			                .Any(x => !ai.IsSociallyTrusted(character, x) &&
			                          PredatorAIHelpers.WillAttack(character, x, ai.WillAttackProg, true));
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

	private interface IAnimalHomeStrategy
	{
		void Evaluate(AnimalAI ai, ICharacter character);
		void EvaluateIdle(AnimalAI ai, ICharacter character);
		bool WouldMove(AnimalAI ai, ICharacter character);
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character);
		bool IsDefendingLocation(AnimalAI ai, ICharacter character);
	}

	private sealed class NoHomeStrategy : IAnimalHomeStrategy
	{
		public static NoHomeStrategy Instance { get; } = new();

		public void Evaluate(AnimalAI ai, ICharacter character)
		{
		}

		public void EvaluateIdle(AnimalAI ai, ICharacter character)
		{
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return false;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		public bool IsDefendingLocation(AnimalAI ai, ICharacter character)
		{
			return true;
		}
	}

	private sealed class TerritorialHomeStrategy : IAnimalHomeStrategy
	{
		public static TerritorialHomeStrategy Instance { get; } = new();

		public void Evaluate(AnimalAI ai, ICharacter character)
		{
			ai.EvaluateTerritory(character);
		}

		public void EvaluateIdle(AnimalAI ai, ICharacter character)
		{
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return character.CombinedEffectsOfType<Territory>().FirstOrDefault() is Territory territory &&
			       territory.Cells.Any() &&
			       !territory.Cells.Contains(character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return ai.GetTerritoryPath(character);
		}

		public bool IsDefendingLocation(AnimalAI ai, ICharacter character)
		{
			return character.CombinedEffectsOfType<Territory>()
			                .FirstOrDefault()
			                ?.Cells
			                .Contains(character.Location) == true;
		}
	}

	private sealed class DenningHomeStrategy : IAnimalHomeStrategy
	{
		public static DenningHomeStrategy Instance { get; } = new();

		public void Evaluate(AnimalAI ai, ICharacter character)
		{
			if (ai.SurvivalNeedsSatisfied(character))
			{
				ai.EvaluateBurrowLifecycle(character);
			}
		}

		public void EvaluateIdle(AnimalAI ai, ICharacter character)
		{
			Evaluate(ai, character);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			if (!ai.SurvivalNeedsSatisfied(character))
			{
				return false;
			}

			NpcHomeBaseEffect home = ai.ResolveHomeBase(character);
			return home.HomeCell is null || !ReferenceEquals(character.Location, home.HomeCell);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return ai.SurvivalNeedsSatisfied(character)
				? ai.GetBurrowHomePath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public bool IsDefendingLocation(AnimalAI ai, ICharacter character)
		{
			return NpcHomeBaseEffect.GetOrCreate(character).HomeCell is ICell home &&
			       ReferenceEquals(home, character.Location);
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
			                                    .Shuffle())
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
			                                    .Shuffle())
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

			foreach (ICharacter target in character.Location.CellsInVicinity(range, true, true)
			                                    .Except(character.Location)
			                                    .SelectMany(x => x.Characters)
			                                    .Where(x => !ai.IsSociallyTrusted(character, x))
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

	private interface IAnimalAwarenessStrategy
	{
		bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget);
		bool WouldMove(AnimalAI ai, ICharacter character);
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character);
	}

	private sealed class NoAwarenessStrategy : IAnimalAwarenessStrategy
	{
		public static NoAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
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

	private sealed class WaryAwarenessStrategy : IAnimalAwarenessStrategy
	{
		public static WaryAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			if (!ai.ShouldAvoidCell(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return ai.ShouldAvoidCell(character, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			if (!WouldMove(ai, character))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			(ICell? target, IEnumerable<ICellExit> path) = ai.GetRefugePath(character);
			return target is not null && path.Any()
				? (target, path)
				: ai.GetAvoidancePath(character);
		}
	}

	private sealed class WimpyAwarenessStrategy : IAnimalAwarenessStrategy
	{
		public static WimpyAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			if (!threats.Any() && !ai.ShouldAvoidCell(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return ai.VisibleAwarenessThreats(character, null).Any() ||
			       ai.ShouldAvoidCell(character, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			if (!WouldMove(ai, character))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			(ICell? target, IEnumerable<ICellExit> path) = ai.GetRefugePath(character);
			return target is not null && path.Any()
				? (target, path)
				: ai.GetAvoidancePath(character);
		}
	}

	private sealed class SkittishAwarenessStrategy : IAnimalAwarenessStrategy
	{
		public static SkittishAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			if (!threats.Any() && !ai.ShouldAvoidCell(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(AnimalAI ai, ICharacter character)
		{
			return WimpyAwarenessStrategy.Instance.WouldMove(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return WimpyAwarenessStrategy.Instance.GetPath(ai, character);
		}
	}

	private sealed class GuardingAwarenessStrategy : IAnimalAwarenessStrategy
	{
		public static GuardingAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(AnimalAI ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			foreach (ICharacter threat in threats.Shuffle())
			{
				if (PredatorAIHelpers.CheckForAttack(character, threat, ai.AwarenessThreatProg,
					    ai.EngageDelayDiceExpression, ai.EngageEmote, false))
				{
					return true;
				}
			}

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

	private interface IAnimalRefugeStrategy
	{
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character);
	}

	private sealed class NoRefugeStrategy : IAnimalRefugeStrategy
	{
		public static NoRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class HomeRefugeStrategy : IAnimalRefugeStrategy
	{
		public static HomeRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			NpcHomeBaseEffect home = ai.ResolveHomeBase(character);
			if (home.HomeCell is null || ReferenceEquals(home.HomeCell, character.Location))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			List<ICellExit> path = character.PathBetween(home.HomeCell, DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character)).ToList();
			return path.Any()
				? (home.HomeCell, path)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class DenRefugeStrategy : IAnimalRefugeStrategy
	{
		public static DenRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return ai.GetBurrowHomePath(character);
		}
	}

	private sealed class TreesRefugeStrategy : IAnimalRefugeStrategy
	{
		public static TreesRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
				x => x is ICell cell && ArborealWandererAI.CellSupportsTreeLayers(character, cell),
				DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character, true));
			return targetPath.Item1 is ICell target && targetPath.Item2.Any()
				? (target, targetPath.Item2)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class SkyRefugeStrategy : IAnimalRefugeStrategy
	{
		public static SkyRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class WaterRefugeStrategy : IAnimalRefugeStrategy
	{
		public static WaterRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			return ai.WaterStrategy == AnimalWaterStrategyType.Drink
				? NpcSurvivalAIHelpers.GetPathToWater(character, ai.GetAnimalSuitabilityFunction(character),
					DefaultNeedRange)
				: NpcSurvivalAIHelpers.GetPathToAquaticWater(character, ai.GetAnimalSuitabilityFunction(character),
					DefaultNeedRange, ai.WaterStrategy == AnimalWaterStrategyType.Surface);
		}
	}

	private sealed class ProgRefugeStrategy : IAnimalRefugeStrategy
	{
		public static ProgRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(AnimalAI ai, ICharacter character)
		{
			Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
				x => x is ICell cell && ai.RefugeCellProg.ExecuteBool(false, character, cell, character.Location),
				DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character));
			return targetPath.Item1 is ICell target && targetPath.Item2.Any()
				? (target, targetPath.Item2)
				: (null, Enumerable.Empty<ICellExit>());
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

	private interface IAnimalMovementStrategy
	{
		bool CellMatches(AnimalAI ai, ICharacter character, ICell cell);
		(ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(AnimalAI ai, ICharacter character);
		FollowingPath CreatePathingEffect(AnimalAI ai, ICharacter character, IEnumerable<ICellExit> path);
	}

	private sealed class GroundMovementStrategy : IAnimalMovementStrategy
	{
		public static GroundMovementStrategy Instance { get; } = new();

		public bool CellMatches(AnimalAI ai, ICharacter character, ICell cell)
		{
			return ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(AnimalAI ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(AnimalAI ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			return new FollowingPath(character, path);
		}
	}

	private sealed class SwimmingMovementStrategy : IAnimalMovementStrategy
	{
		public static SwimmingMovementStrategy Instance { get; } = new();

		public bool CellMatches(AnimalAI ai, ICharacter character, ICell cell)
		{
			return character.Race.CanSwim &&
			       CellSupportsSwimming(character, cell) &&
			       ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(AnimalAI ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(AnimalAI ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			RoomLayer targetLayer = ai.WaterStrategy == AnimalWaterStrategyType.Surface
				? RoomLayer.GroundLevel
				: character.RoomLayer;
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}
	}

	private sealed class FlyingMovementStrategy : IAnimalMovementStrategy
	{
		public static FlyingMovementStrategy Instance { get; } = new();

		public bool CellMatches(AnimalAI ai, ICharacter character, ICell cell)
		{
			return ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(AnimalAI ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(AnimalAI ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			return new FollowingMultiLayerPath(character, path, ai.TargetFlyingLayer, ai.TargetRestingLayer);
		}
	}

	private sealed class ArborealMovementStrategy : IAnimalMovementStrategy
	{
		public static ArborealMovementStrategy Instance { get; } = new();

		public bool CellMatches(AnimalAI ai, ICharacter character, ICell cell)
		{
			return ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location) &&
			       (ArborealWandererAI.CellSupportsTreeLayers(character, cell) ||
			        ai.AllowDescentProg.ExecuteBool(false, character, cell));
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(AnimalAI ai, ICharacter character)
		{
			List<(ICell Cell, int Distance)> treeTargets = character.CellsAndDistancesInVicinity(
					(uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true),
					cell => ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location) &&
					        ai.IsWithinPreferredHabitat(character, cell) &&
					        ArborealWandererAI.CellSupportsTreeLayers(character, cell))
				.ToList();

			ICell? target = treeTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
			if (target is not null)
			{
				List<ICellExit> path = character.PathBetween(target, (uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true)).ToList();
				if (path.Any())
				{
					return (target, path);
				}
			}

			List<(ICell Cell, int Distance)> descentTargets = character.CellsAndDistancesInVicinity(
					(uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true),
					cell => ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location) &&
					        ai.IsWithinPreferredHabitat(character, cell) &&
					        !ArborealWandererAI.CellSupportsTreeLayers(character, cell) &&
					        ai.AllowDescentProg.ExecuteBool(false, character, cell))
				.ToList();
			target = descentTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
			if (target is null)
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			List<ICellExit> descentPath = character.PathBetween(target, (uint)ai.MovementRange,
				ai.GetAnimalSuitabilityFunction(character, true)).ToList();
			return descentPath.Any()
				? (target, descentPath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public FollowingPath CreatePathingEffect(AnimalAI ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			ICell destination = path.Last().Destination;
			RoomLayer targetLayer = ChooseTreeLayer(ai, character, destination);
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}

		private static RoomLayer ChooseTreeLayer(AnimalAI ai, ICharacter character, ICell cell)
		{
			List<RoomLayer> layers = cell.Terrain(character)?.TerrainLayers.ToList() ?? new List<RoomLayer>();
			if (layers.Contains(ai.PreferredTreeLayer))
			{
				return ai.PreferredTreeLayer;
			}

			if (layers.Contains(ai.SecondaryTreeLayer))
			{
				return ai.SecondaryTreeLayer;
			}

			if (layers.Contains(RoomLayer.HighInTrees))
			{
				return RoomLayer.HighInTrees;
			}

			if (layers.Contains(RoomLayer.InTrees))
			{
				return RoomLayer.InTrees;
			}

			return RoomLayer.GroundLevel;
		}
	}

	private sealed class AmphibiousMovementStrategy : IAnimalMovementStrategy
	{
		public static AmphibiousMovementStrategy Instance { get; } = new();

		public bool CellMatches(AnimalAI ai, ICharacter character, ICell cell)
		{
			if (!ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location))
			{
				return false;
			}

			return CellSupportsSwimming(character, cell)
				? ai.AmphibiousWaterCellProg.ExecuteBool(false, character, cell, character.Location)
				: ai.AmphibiousLandCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(AnimalAI ai, ICharacter character)
		{
			bool preferWater = RandomUtilities.DoubleRandom(0.0, 1.0) <= ai.AmphibiousWaterBias;
			(ICell? target, IEnumerable<ICellExit> path) = GetWeightedAmbientPath(ai, character,
				(_, ch, cell) => CellMatches(ai, ch, cell) && CellSupportsSwimming(ch, cell) == preferWater);
			if (target is not null)
			{
				return (target, path);
			}

			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(AnimalAI ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			ICell? destination = path.LastOrDefault()?.Destination;
			RoomLayer targetLayer = destination is not null && CellSupportsSwimming(character, destination)
				? ai.WaterStrategy == AnimalWaterStrategyType.Surface ? RoomLayer.GroundLevel : character.RoomLayer
				: RoomLayer.GroundLevel;
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}
	}

	internal static bool CellSupportsSwimming(ICharacter character, ICell cell)
	{
		return cell.IsSwimmingLayer(character.RoomLayer) ||
		       cell.Terrain(character)?.TerrainLayers.Any(cell.IsSwimmingLayer) == true;
	}

	internal static bool CellSupportsSurfaceWater(ICharacter character, ICell cell)
	{
		return CellSupportsSwimming(character, cell) &&
		       cell.Terrain(character)?.TerrainLayers.Any(x => !x.IsUnderwater()) == true;
	}

	private static (ICell? Target, IEnumerable<ICellExit> Path) GetWeightedAmbientPath(
		AnimalAI ai,
		ICharacter character,
		Func<AnimalAI, ICharacter, ICell, bool> predicate)
	{
		List<(ICell Cell, int Distance)> vicinity = character.CellsAndDistancesInVicinity(
				(uint)ai.MovementRange,
				ai.GetAnimalSuitabilityFunction(character, true),
				cell => predicate(ai, character, cell) && ai.IsWithinPreferredHabitat(character, cell))
			.ToList();
		ICell? target = vicinity.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
		if (target is null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		List<ICellExit> path = character.PathBetween(target, (uint)ai.MovementRange,
			ai.GetAnimalSuitabilityFunction(character, true)).ToList();
		return path.Any()
			? (path.Last().Destination, path)
			: (null, Enumerable.Empty<ICellExit>());
	}
}
