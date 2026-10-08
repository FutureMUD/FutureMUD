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

public abstract partial class CreatureAIBase : PathingAIBase
{
	protected void LoadCreatureDefinition(XElement root)
	{
		Hunting = AnimalHuntingSettings.Load(root.Element("Hunting"));
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
		MovementRoomProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("MovementCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		PreferredHabitatProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("PreferredHabitatProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		ToleratedHabitatProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("ToleratedHabitatProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		AmphibiousLandRoomProg =
			Gameworld.FutureProgs.Get(long.Parse(movement.Element("AmphibiousLandCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysTrueProg;
		AmphibiousWaterRoomProg =
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
		_burrowCraftId = long.Parse(home.Element("BurrowCraftId")?.Value ?? "0");
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

		XElement awareness = root.Element("Awareness") ?? new XElement("Awareness");
		AwarenessStrategy = ParseEnum(awareness.Attribute("type")?.Value, AnimalAwarenessStrategyType.None);
		AwarenessThreatProg =
			Gameworld.FutureProgs.Get(long.Parse(awareness.Element("ThreatProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		AwarenessAvoidRoomProg =
			Gameworld.FutureProgs.Get(long.Parse(awareness.Element("AvoidCellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;
		AwarenessRange = int.Parse(awareness.Element("Range")?.Value ?? "5");
		AwarenessMemoryMinutes = int.Parse(awareness.Element("MemoryMinutes")?.Value ?? "10");
		SensesStrategy = ParseEnum(awareness.Element("Senses")?.Value, AnimalSensesStrategyType.None);

		XElement refuge = root.Element("Refuge") ?? new XElement("Refuge");
		RefugeStrategy = ParseEnum(refuge.Attribute("type")?.Value, AnimalRefugeStrategyType.None);
		RefugeLayer = ParseEnum(refuge.Element("Layer")?.Value, RoomLayer.HighInTrees);
		RefugeReturnSeconds = int.Parse(refuge.Element("ReturnSeconds")?.Value ?? "60");
		RefugeRoomProg =
			Gameworld.FutureProgs.Get(long.Parse(refuge.Element("CellProg")?.Value ?? "0")) ??
			Gameworld.AlwaysFalseProg;

	}

	protected IEnumerable<XElement> SaveCreatureDefinition() => new XElement[]
	{
		Hunting.Save(),
			new XElement("Movement",
				new XAttribute("type", MovementStrategy),
				new XElement("Range", MovementRange),
				new XElement("AmphibiousWaterBias", AmphibiousWaterBias),
				new XElement("WanderChancePerMinute", WanderChancePerMinute),
				new XElement("WanderEmote", new XCData(WanderEmote)),
				new XElement("MovementEnabledProg", MovementEnabledProg?.Id ?? 0),
				new XElement("MovementCellProg", MovementRoomProg?.Id ?? 0),
				new XElement("PreferredHabitatProg", PreferredHabitatProg?.Id ?? 0),
				new XElement("ToleratedHabitatProg", ToleratedHabitatProg?.Id ?? 0),
				new XElement("AmphibiousLandCellProg", AmphibiousLandRoomProg?.Id ?? 0),
				new XElement("AmphibiousWaterCellProg", AmphibiousWaterRoomProg?.Id ?? 0),
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
				new XElement("BurrowCraftId", _burrowCraftId),
				new XElement("BurrowSiteProg", BurrowSiteProg?.Id ?? 0),
				new XElement("BuildEnabledProg", BuildEnabledProg?.Id ?? 0),
				new XElement("HomeLocationProg", HomeLocationProg?.Id ?? 0),
				new XElement("AnchorItemProg", AnchorItemProg?.Id ?? 0)),
			new XElement("Awareness",
				new XAttribute("type", AwarenessStrategy),
				new XElement("ThreatProg", AwarenessThreatProg?.Id ?? 0),
				new XElement("AvoidCellProg", AwarenessAvoidRoomProg?.Id ?? 0),
				new XElement("Range", AwarenessRange),
				new XElement("MemoryMinutes", AwarenessMemoryMinutes),
				new XElement("Senses", SensesStrategy)),
			new XElement("Refuge",
				new XAttribute("type", RefugeStrategy),
				new XElement("Layer", RefugeLayer),
				new XElement("CellProg", RefugeRoomProg?.Id ?? 0),
				new XElement("ReturnSeconds", RefugeReturnSeconds))
	};

	protected void SetCreatureDefaults()
	{
		MovementStrategy = AnimalMovementStrategyType.Ground;
		HomeStrategy = AnimalHomeStrategyType.None;
		WaterStrategy = AnimalWaterStrategyType.Drink;
		AwarenessStrategy = AnimalAwarenessStrategyType.None;
		RefugeStrategy = AnimalRefugeStrategyType.None;
		SensesStrategy = AnimalSensesStrategyType.None;
		MovementRange = DefaultGroundRange;
		AmphibiousWaterBias = 0.50;
		WanderChancePerMinute = 0.33;
		WanderEmote = string.Empty;
		EngageDelayDiceExpression = "1000+1d1000";
		EngageEmote = string.Empty;
		PostureEmote = string.Empty;
		PostureDurationDiceExpression = "1d20+20";
		AwarenessRange = 5;
		AwarenessMemoryMinutes = 10;
		RefugeReturnSeconds = 60;
		TargetFlyingLayer = RoomLayer.InAir;
		TargetRestingLayer = RoomLayer.HighInTrees;
		PreferredTreeLayer = RoomLayer.HighInTrees;
		SecondaryTreeLayer = RoomLayer.InTrees;
		RefugeLayer = RoomLayer.HighInTrees;
		WillShareTerritory = false;
		WillShareTerritoryWithOtherRaces = true;
		AllowGroupShelterSharing = false;
		if (Gameworld is not null)
		{
			MovementEnabledProg = Gameworld.AlwaysTrueProg;
			MovementRoomProg = Gameworld.AlwaysTrueProg;
			PreferredHabitatProg = Gameworld.AlwaysTrueProg;
			ToleratedHabitatProg = Gameworld.AlwaysTrueProg;
			AmphibiousLandRoomProg = Gameworld.AlwaysTrueProg;
			AmphibiousWaterRoomProg = Gameworld.AlwaysTrueProg;
			AllowDescentProg = Gameworld.AlwaysFalseProg;
			SuitableTerritoryProg = Gameworld.AlwaysTrueProg;
			DesiredTerritorySizeProg = Gameworld.AlwaysOneProg;
			BurrowSiteProg = Gameworld.AlwaysTrueProg;
			BuildEnabledProg = Gameworld.AlwaysTrueProg;
			AwarenessThreatProg = Gameworld.AlwaysFalseProg;
			AwarenessAvoidRoomProg = Gameworld.AlwaysFalseProg;
			RefugeRoomProg = Gameworld.AlwaysFalseProg;
		}
	}
}
