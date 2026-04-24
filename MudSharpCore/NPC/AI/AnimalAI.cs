#nullable enable
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Work.Crafts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public enum AnimalMovementStrategyType
{
	Ground,
	Swim,
	Fly,
	Arboreal
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
	Opportunist
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
	public AnimalThreatStrategyType ThreatStrategy { get; private set; }
	public AnimalAwarenessStrategyType AwarenessStrategy { get; private set; }
	public AnimalRefugeStrategyType RefugeStrategy { get; private set; }
	public AnimalActivityStrategyType ActivityStrategy { get; private set; }
	public bool WaterEnabled { get; private set; }

	public IFutureProg MovementEnabledProg { get; private set; } = null!;
	public IFutureProg MovementCellProg { get; private set; } = null!;
	public IFutureProg AllowDescentProg { get; private set; } = null!;
	public IFutureProg SuitableTerritoryProg { get; private set; } = null!;
	public IFutureProg DesiredTerritorySizeProg { get; private set; } = null!;
	public IFutureProg BurrowSiteProg { get; private set; } = null!;
	public IFutureProg BuildEnabledProg { get; private set; } = null!;
	public IFutureProg WillAttackProg { get; private set; } = null!;
	public IFutureProg AwarenessThreatProg { get; private set; } = null!;
	public IFutureProg AwarenessAvoidCellProg { get; private set; } = null!;
	public IFutureProg RefugeCellProg { get; private set; } = null!;
	public IFutureProg? HomeLocationProg { get; private set; }
	public IFutureProg? AnchorItemProg { get; private set; }
	public ICraft? BurrowCraft { get; private set; }

	private readonly List<TimeOfDay> _activeTimesOfDay = new();

	public int MovementRange { get; private set; }
	public double WanderChancePerMinute { get; private set; }
	public string WanderEmote { get; private set; } = string.Empty;
	public string EngageDelayDiceExpression { get; private set; } = "1000+1d1000";
	public string EngageEmote { get; private set; } = string.Empty;
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
	public bool WillShareTerritory { get; private set; }
	public bool WillShareTerritoryWithOtherRaces { get; private set; }
	public IEnumerable<TimeOfDay> ActiveTimesOfDay => _activeTimesOfDay;

	public override bool CountsAsAggressive => ThreatStrategy.In(AnimalThreatStrategyType.Defend,
		AnimalThreatStrategyType.HungryPredator);

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
		ThreatStrategy = AnimalThreatStrategyType.Passive;
		AwarenessStrategy = AnimalAwarenessStrategyType.None;
		RefugeStrategy = AnimalRefugeStrategyType.None;
		ActivityStrategy = AnimalActivityStrategyType.Always;
		WaterEnabled = true;
		MovementRange = DefaultGroundRange;
		WanderChancePerMinute = 0.33;
		WanderEmote = string.Empty;
		EngageDelayDiceExpression = "1000+1d1000";
		EngageEmote = string.Empty;
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
		_activeTimesOfDay.Clear();
		_activeTimesOfDay.AddRange(Enum.GetValues<TimeOfDay>());
		WillShareTerritory = false;
		WillShareTerritoryWithOtherRaces = true;

		if (Gameworld is not null)
		{
			MovementEnabledProg = Gameworld.AlwaysTrueProg;
			MovementCellProg = Gameworld.AlwaysTrueProg;
			AllowDescentProg = Gameworld.AlwaysFalseProg;
			SuitableTerritoryProg = Gameworld.AlwaysTrueProg;
			DesiredTerritorySizeProg = Gameworld.AlwaysOneProg;
			BurrowSiteProg = Gameworld.AlwaysTrueProg;
			BuildEnabledProg = Gameworld.AlwaysTrueProg;
			WillAttackProg = Gameworld.AlwaysFalseProg;
			AwarenessThreatProg = Gameworld.AlwaysFalseProg;
			AwarenessAvoidCellProg = Gameworld.AlwaysFalseProg;
			RefugeCellProg = Gameworld.AlwaysFalseProg;
		}
	}

	protected override void LoadFromXML(XElement root)
	{
		SetDefaults();
		base.LoadFromXML(root);

		XElement movement = root.Element("Movement") ?? new XElement("Movement");
		MovementStrategy = ParseEnum(movement.Attribute("type")?.Value, AnimalMovementStrategyType.Ground);
		MovementRange = int.Parse(movement.Element("Range")?.Value ?? DefaultRangeFor(MovementStrategy).ToString());
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
		EngageDelayDiceExpression = feeding.Element("EngageDelayDiceExpression")?.Value ?? "1000+1d1000";
		EngageEmote = feeding.Element("EngageEmote")?.Value ?? string.Empty;

		XElement water = root.Element("Water") ?? new XElement("Water");
		WaterEnabled = bool.Parse(water.Attribute("enabled")?.Value ?? "true");

		XElement threat = root.Element("Threat") ?? new XElement("Threat");
		ThreatStrategy = ParseEnum(threat.Attribute("type")?.Value, AnimalThreatStrategyType.Passive);

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

		XElement refuge = root.Element("Refuge") ?? new XElement("Refuge");
		RefugeStrategy = ParseEnum(refuge.Attribute("type")?.Value, AnimalRefugeStrategyType.None);
		RefugeLayer = ParseEnum(refuge.Element("Layer")?.Value, RoomLayer.HighInTrees);
		RefugeReturnSeconds = int.Parse(refuge.Element("ReturnSeconds")?.Value ?? "60");
		RefugeCellProg =
			Gameworld.FutureProgs.Get(long.Parse(refuge.Element("CellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;

		XElement activity = root.Element("Activity") ?? new XElement("Activity");
		ActivityStrategy = ParseEnum(activity.Attribute("type")?.Value, AnimalActivityStrategyType.Always);
		ActivitySleepEnabled = bool.Parse(activity.Element("SleepEnabled")?.Value ?? "false");
		ActivityRestEmote = activity.Element("RestEmote")?.Value ?? string.Empty;
		LoadActiveTimes(activity);
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
				new XElement("WanderChancePerMinute", WanderChancePerMinute),
				new XElement("WanderEmote", new XCData(WanderEmote)),
				new XElement("MovementEnabledProg", MovementEnabledProg?.Id ?? 0),
				new XElement("MovementCellProg", MovementCellProg?.Id ?? 0),
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
				new XElement("BurrowCraftId", BurrowCraft?.Id ?? 0),
				new XElement("BurrowSiteProg", BurrowSiteProg?.Id ?? 0),
				new XElement("BuildEnabledProg", BuildEnabledProg?.Id ?? 0),
				new XElement("HomeLocationProg", HomeLocationProg?.Id ?? 0),
				new XElement("AnchorItemProg", AnchorItemProg?.Id ?? 0)),
			new XElement("Feeding",
				new XAttribute("type", FeedingStrategy),
				new XElement("WillAttackProg", WillAttackProg?.Id ?? 0),
				new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
				new XElement("EngageEmote", new XCData(EngageEmote))),
			new XElement("Water", new XAttribute("enabled", WaterEnabled)),
			new XElement("Threat", new XAttribute("type", ThreatStrategy)),
			new XElement("Awareness",
				new XAttribute("type", AwarenessStrategy),
				new XElement("ThreatProg", AwarenessThreatProg?.Id ?? 0),
				new XElement("AvoidCellProg", AwarenessAvoidCellProg?.Id ?? 0),
				new XElement("Range", AwarenessRange),
				new XElement("MemoryMinutes", AwarenessMemoryMinutes)),
			new XElement("Refuge",
				new XAttribute("type", RefugeStrategy),
				new XElement("Layer", RefugeLayer),
				new XElement("CellProg", RefugeCellProg?.Id ?? 0),
				new XElement("ReturnSeconds", RefugeReturnSeconds)),
			new XElement("Activity",
				new XAttribute("type", ActivityStrategy),
				new XElement("SleepEnabled", ActivitySleepEnabled),
				new XElement("RestEmote", new XCData(ActivityRestEmote)),
				_activeTimesOfDay.Select(x => new XElement("ActiveTime", x))),
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
		if (feeding == AnimalFeedingStrategyType.DenPredator && home != AnimalHomeStrategyType.Denning)
		{
			return (false, "den-predator feeding requires denning home behavior");
		}

		if (threat == AnimalThreatStrategyType.HungryPredator &&
		    !feeding.In(AnimalFeedingStrategyType.Predator, AnimalFeedingStrategyType.DenPredator))
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

		return (true, string.Empty);
	}

	private (bool Ready, string Reason) GetReadiness()
	{
		return ValidateConfiguration(HomeStrategy, FeedingStrategy, ThreatStrategy, MovementStrategy,
			RefugeStrategy, ActivityStrategy, _activeTimesOfDay);
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
		_ => NoFeedingStrategy.Instance
	};

	private IAnimalWaterStrategy WaterStrategyHandler => WaterEnabled
		? EnabledWaterStrategy.Instance
		: DisabledWaterStrategy.Instance;

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
		sb.AppendLine($"Movement Enabled Prog: {MovementEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Movement Cell Prog: {MovementCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
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
		sb.AppendLine($"Burrow Craft: {BurrowCraft?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Burrow Site Prog: {BurrowSiteProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Build Enabled Prog: {BuildEnabledProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Home Location Prog: {HomeLocationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Anchor Item Prog: {AnchorItemProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine($"Feeding: {FeedingStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Water: {(WaterEnabled ? "enabled" : "disabled").ColourValue()}");
		sb.AppendLine($"Threat: {ThreatStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Attack Prog: {WillAttackProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} milliseconds");
		sb.AppendLine($"Engage Emote: {EngageEmote.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine($"Awareness: {AwarenessStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Threat Filter Prog: {AwarenessThreatProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Avoid Cell Prog: {AwarenessAvoidCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Awareness Range: {AwarenessRange.ToString("N0", actor).ColourValue()} rooms");
		sb.AppendLine($"Threat Memory: {AwarenessMemoryMinutes.ToString("N0", actor).ColourValue()} minutes");
		sb.AppendLine();
		sb.AppendLine($"Refuge: {RefugeStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Refuge Layer: {RefugeLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Refuge Cell Prog: {RefugeCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Refuge Return Delay: {RefugeReturnSeconds.ToString("N0", actor).ColourValue()} seconds");
		sb.AppendLine();
		sb.AppendLine($"Activity: {ActivityStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"Active Times: {_activeTimesOfDay.Select(x => x.DescribeEnum().ColourName()).ListToString()}");
		sb.AppendLine($"Sleep When Inactive: {ActivitySleepEnabled.ToColouredString()}");
		sb.AppendLine($"Rest Emote: {ActivityRestEmote.ColourCommand()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3movement ground|swim|fly|arboreal#0 - sets the movement strategy
	#3movement range <number>#0 - sets the path search range
	#3movement chance <%>#0 - sets the ambient movement chance per minute
	#3movement enabled <prog>#0 - sets whether ambient movement is enabled
	#3movement room <prog>#0 - sets which cells can be ambient movement targets
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
	#3home craft <craft|clear>#0 - sets the optional burrow craft
	#3home site <prog>#0 - sets suitable burrow cells
	#3home location <prog|clear>#0 - sets fallback home location
	#3home enabled <prog>#0 - sets whether burrow building is active
	#3home anchor <prog|clear>#0 - sets burrow anchor detection
	#3feeding none|predator|denpredator|forager|scavenger|opportunist#0 - sets feeding behavior
	#3feeding attackprog <prog>#0 - sets predator target selection
	#3feeding delay <dice>#0 - sets predator attack delay
	#3feeding emote <text|clear>#0 - sets predator engage emote
	#3water on|off#0 - toggles thirst and water-memory behavior
	#3threat passive|flee|defend|hungrypredator#0 - sets threat behavior
	#3awareness none|wary|wimpy|skittish|guarding#0 - sets non-combat awareness behavior
	#3awareness threat <prog>#0 - sets the character filter for disliked or feared targets
	#3awareness avoid <prog>#0 - sets the cell filter for places this animal avoids
	#3awareness range <rooms>#0 - sets how far the animal notices threats
	#3awareness memory <minutes>#0 - sets how long threat locations are remembered
	#3refuge none|home|den|trees|sky|water|prog#0 - sets where the animal retreats or rests
	#3refuge layer <layer>#0 - sets the refuge layer for trees or sky
	#3refuge cell <prog>#0 - sets the refuge-cell selector for prog refuge
	#3refuge return <seconds>#0 - sets the return delay after refuge work
	#3activity always|diurnal|nocturnal|crepuscular|custom#0 - sets active periods
	#3activity active <timeofday...>#0 - sets active times for custom activity
	#3activity sleep on|off#0 - toggles sleeping while inactive at refuge
	#3activity restemote <text|clear>#0 - sets an optional rest emote";

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
			case "range":
				return BuildingCommandMovementRange(actor, command);
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
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out RoomLayer value))
		{
			actor.OutputHandler.Send("You must specify a valid room layer.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.OutputHandler.Send($"The {label} layer is now {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTreeLayer(ICharacter actor, StringStack command, Action<RoomLayer> setter, string label)
	{
		if (command.IsFinished ||
		    !command.SafeRemainingArgument.TryParseEnum(out RoomLayer value) ||
		    !value.In(RoomLayer.InTrees, RoomLayer.HighInTrees))
		{
			actor.OutputHandler.Send("You must specify either #3InTrees#0 or #3HighInTrees#0."
			                         .SubstituteANSIColour());
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
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Character }
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
			case "attackprog":
			case "attack":
				return BuildingCommandAttackProg(actor, command);
			case "delay":
			case "engagedelay":
				return BuildingCommandEngageDelay(actor, command);
			case "emote":
			case "engageemote":
				return BuildingCommandEngageEmote(actor, command);
		}

		actor.OutputHandler.Send(TypeHelpText.SubstituteANSIColour());
		return false;
	}

	private bool SetFeedingStrategy(ICharacter actor, AnimalFeedingStrategyType strategy)
	{
		FeedingStrategy = strategy;
		if (strategy.In(AnimalFeedingStrategyType.Predator, AnimalFeedingStrategyType.DenPredator) &&
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
			WaterEnabled = !WaterEnabled;
		}
		else
		{
			switch (command.PopForSwitch())
			{
				case "on":
				case "yes":
				case "true":
					WaterEnabled = true;
					break;
				case "off":
				case "no":
				case "false":
					WaterEnabled = false;
					break;
				default:
					actor.OutputHandler.Send("You must specify either #3on#0 or #3off#0.".SubstituteANSIColour());
					return false;
			}
		}

		Changed = true;
		actor.OutputHandler.Send($"This animal AI will {WaterEnabled.NowNoLonger()} care about thirst and remembered water sources.");
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
			case "restemote":
			case "emote":
				return BuildingCommandActivityRestEmote(actor, command);
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
			actor.OutputHandler.Send("You must specify one or more active times of day, or #3all#0.".SubstituteANSIColour());
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
						actor.OutputHandler.Send($"The text {token.ColourCommand()} is not a valid time of day.");
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

				if (EvaluateImmediateNeedsAndFeeding(ch))
				{
					return true;
				}

				if (type == EventType.TenSecondTick && TryThreatResponse(ch, null))
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

				if (EvaluateImmediateNeedsAndFeeding(ch) ||
				    EvaluateActivity(ch))
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

				return TryThreatResponse(ch, (ICharacter)arguments[0]);
			case EventType.LayerChangeBlockExpired:
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

		if (WaterStrategyHandler.TrySatisfyImmediateNeed(this, character))
		{
			return true;
		}

		if (WaterStrategyHandler.IsThirsty(this, character))
		{
			return false;
		}

		if (FeedingStrategyHandler.TrySatisfyImmediateNeed(this, character))
		{
			return true;
		}

		if (ShouldReturnToRefuge(character) && TryMoveToRefugeLayer(character))
		{
			return true;
		}

		if (SurvivalNeedsSatisfied(character))
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

	private Func<ICellExit, bool> GetAnimalSuitabilityFunction(ICharacter character, bool ignoreSafeMovement = false)
	{
		Func<ICellExit, bool> baseSuitability = base.GetSuitabilityFunction(character, ignoreSafeMovement);
		return exit => baseSuitability(exit) && !ShouldAvoidCell(character, exit.Destination);
	}

	private IEnumerable<ICharacter> VisibleAwarenessThreats(ICharacter character, ICharacter? witnessedTarget)
	{
		HashSet<ICharacter> threats = new();
		if (witnessedTarget is not null &&
		    !ReferenceEquals(witnessedTarget, character) &&
		    AwarenessThreatProg.ExecuteBool(false, character, witnessedTarget) &&
		    character.CanSee(witnessedTarget))
		{
			threats.Add(witnessedTarget);
		}

		IEnumerable<ICharacter> candidates = character.Location
		                                              .LayerCharacters(character.RoomLayer)
		                                              .Concat(AwarenessRange > 0
			                                              ? character.Location.CellsInVicinity((uint)AwarenessRange, true, true)
			                                                         .SelectMany(x => x.Characters)
			                                              : Enumerable.Empty<ICharacter>());

		foreach (ICharacter target in candidates.Distinct())
		{
			if (ReferenceEquals(target, character) ||
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
		if (character.Combat is not null ||
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
			                           !ReferenceEquals(y, character) &&
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
			AnimalRefugeStrategyType.Water => NpcSurvivalAIHelpers.HasLocalWaterSource(character),
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
		    ActivityStrategyHandler.IsActive(this, character))
		{
			return false;
		}

		if (!IsAtRefuge(character))
		{
			CheckPathingEffect(character, true);
			return true;
		}

		return TrySleepAtRefuge(character);
	}

	private bool TrySleepAtRefuge(ICharacter character)
	{
		if (!ActivitySleepEnabled ||
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

	private bool TryThreatResponse(ICharacter character, ICharacter? witnessedTarget)
	{
		if (character.Combat is not null ||
		    character.Movement is not null ||
		    character.Effects.Any(x => x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")))
		{
			return false;
		}

		return ThreatStrategyHandler.TryRespond(this, character, witnessedTarget);
	}

	private bool TryHungryPredatorAttack(ICharacter character, ICharacter target)
	{
		return PredatorAIHelpers.CheckForAttack(character, target, WillAttackProg, EngageDelayDiceExpression,
			EngageEmote, true);
	}

	private bool TryDefensiveAttack(ICharacter character, ICharacter target)
	{
		return HomeStrategyHandler.IsDefendingLocation(this, character) &&
		       PredatorAIHelpers.CheckForAttack(character, target, WillAttackProg, EngageDelayDiceExpression,
			       EngageEmote, false);
	}

	private bool TryFlee(ICharacter character, ICharacter target)
	{
		if (!WillAttackProg.ExecuteBool(character, target))
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
		if (FeedingStrategy != AnimalFeedingStrategyType.DenPredator)
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

		IActiveCraftGameItemComponent? interruptedCraft = character.Location.LayerGameItems(character.RoomLayer)
			.SelectNotNull(x => x.GetItemType<IActiveCraftGameItemComponent>())
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
			return;
		}

		home.ClearAnchorItem();
		IGameItem? anchor = DenBuilderAI.SelectAnchorItem(character, AnchorItemProg);
		if (anchor is not null)
		{
			home.SetAnchorItem(anchor);
		}
	}

	private void EvaluateTerritory(ICharacter character)
	{
		Territory territoryEffect = character.CombinedEffectsOfType<Territory>().FirstOrDefault();
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
			if (SuitableTerritoryProg.Execute<bool?>(character.Location, character) == true &&
			    !claimedTerritory.Contains(character.Location))
			{
				territoryEffect.AddCell(character.Location);
				return;
			}

			(IPerceivable target, IEnumerable<ICellExit> _) = character.AcquireTargetAndPath(
				loc => SuitableTerritoryProg.Execute<bool?>(loc, character) == true &&
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
			               .Where(x => SuitableTerritoryProg.Execute<bool?>(x.Destination, character) == true &&
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
		       .SelectNotNull(x => x.CombinedEffectsOfType<Territory>().FirstOrDefault())
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

		if (AwarenessStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		if (WaterStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		if (FeedingStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		if (ShouldReturnToRefuge(ch))
		{
			return true;
		}

		if (ActivityStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		if (HomeStrategyHandler.WouldMove(this, ch))
		{
			return true;
		}

		return MovementEnabledProg.ExecuteBool(false, ch) &&
		       RandomUtilities.DoubleRandom(0.0, 1.0) <= WanderChancePerMinute;
	}

	private bool HasLocalFoodOpportunity(ICharacter ch)
	{
		return FeedingStrategyHandler.HasLocalFoodOpportunity(this, ch);
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		(ICell? target, IEnumerable<ICellExit> path) = AwarenessStrategyHandler.GetPath(this, ch);
		if (target is not null && path.Any())
		{
			return (target, path);
		}

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

		if (ShouldReturnToRefuge(ch))
		{
			(target, path) = GetRefugePath(ch);
			if (target is not null && path.Any())
			{
				return (target, path);
			}
		}

		(target, path) = ActivityStrategyHandler.GetPath(this, ch);
		if (target is not null && path.Any())
		{
			return (target, path);
		}

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

		return (null, Enumerable.Empty<ICellExit>());
	}

	private (ICell? Target, IEnumerable<ICellExit> Path) GetPredatorFoodPath(ICharacter ch)
	{
		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = ch.AcquireTargetAndPath(
			x => x is ICharacter target && PredatorAIHelpers.WillAttack(ch, target, WillAttackProg, true),
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
		Territory territory = ch.CombinedEffectsOfType<Territory>().FirstOrDefault();
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

	private sealed class EnabledWaterStrategy : IAnimalWaterStrategy
	{
		public static EnabledWaterStrategy Instance { get; } = new();

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
			                .Any(x => PredatorAIHelpers.WillAttack(character, x, ai.WillAttackProg, true));
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
			if (witnessedTarget is not null)
			{
				return ai.TryFlee(character, witnessedTarget);
			}

			foreach (ICharacter target in character.Location.LayerCharacters(character.RoomLayer)
			                                    .Except(character)
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
			if (witnessedTarget is not null)
			{
				return TryAttack(ai, character, witnessedTarget);
			}

			foreach (ICharacter target in character.Location.LayerCharacters(character.RoomLayer)
			                                    .Except(character)
			                                    .Shuffle())
			{
				if (TryAttack(ai, character, target))
				{
					return true;
				}
			}

			uint range = (uint)character.Body.WieldedItems
			                       .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
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
			return NpcSurvivalAIHelpers.GetPathToWater(character, ai.GetAnimalSuitabilityFunction(character),
				DefaultNeedRange);
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
			return new FollowingMultiLayerPath(character, path, character.RoomLayer, character.RoomLayer);
		}

		private static bool CellSupportsSwimming(ICharacter character, ICell cell)
		{
			return cell.IsSwimmingLayer(character.RoomLayer) ||
			       cell.Terrain(character)?.TerrainLayers.Any(cell.IsSwimmingLayer) == true;
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

	private static (ICell? Target, IEnumerable<ICellExit> Path) GetWeightedAmbientPath(
		AnimalAI ai,
		ICharacter character,
		Func<AnimalAI, ICharacter, ICell, bool> predicate)
	{
		List<(ICell Cell, int Distance)> vicinity = character.CellsAndDistancesInVicinity(
				(uint)ai.MovementRange,
				ai.GetAnimalSuitabilityFunction(character, true),
				cell => predicate(ai, character, cell))
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
