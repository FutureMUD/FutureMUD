#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class TerritorialForagerAI : TerritorialWanderer
{
	protected TerritorialForagerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	protected TerritorialForagerAI(IFuturemud gameworld, string name, string type = "TerritorialForager") :
		base(gameworld, name, type)
	{
		if (type == "TerritorialForager")
		{
			DatabaseInitialise();
		}
	}

	protected TerritorialForagerAI()
	{
	}

	public static void RegisterLoader()
	{
		RegisterAIType("TerritorialForager", (ai, gameworld) => new TerritorialForagerAI(ai, gameworld));
		RegisterAIBuilderInformation("territorialforager",
			(gameworld, name) => new TerritorialForagerAI(gameworld, name),
			new TerritorialForagerAI().HelpText);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SuitableTerritoryProg", SuitableTerritoryProg?.Id ?? 0),
			new XElement("DesiredTerritorySizeProg", DesiredTerritorySizeProg?.Id ?? 0),
			new XElement("WanderChancePerMinute", WanderChancePerMinute),
			new XElement("WanderEmote", new XCData(WanderEmote)),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("WillShareTerritory", WillShareTerritory),
			new XElement("WillShareTerritoryWithOtherRaces", WillShareTerritoryWithOtherRaces)
		).ToString();
	}

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
		sb.AppendLine("Foraging: Eats local food items, direct edible forage yields, or uses the FORAGE command for edible forageables while hungry.");
		return sb.ToString();
	}

	protected virtual bool EvaluateForageLifecycle(ICharacter character)
	{
		return ForagerAIHelpers.TrySatisfyHunger(character);
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		ICharacter? ch = arguments[0] as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.CharacterEntersGame:
			case EventType.CharacterEnterCellFinish:
			case EventType.LeaveCombat:
			case EventType.TenSecondTick:
			case EventType.MinuteTick:
			case EventType.NPCOnGameLoadFinished:
				if (EvaluateForageLifecycle(ch))
				{
					return true;
				}

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
				case EventType.TenSecondTick:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	protected override bool WouldMove(ICharacter ch)
	{
		if (ForagerAIHelpers.IsHungry(ch) &&
		    !ForagerAIHelpers.HasFoodOpportunity(ch, ch.Location))
		{
			return true;
		}

		return base.WouldMove(ch);
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		if (ForagerAIHelpers.IsHungry(ch) &&
		    !ForagerAIHelpers.HasFoodOpportunity(ch, ch.Location))
		{
			Territory? territory = ch.CombinedEffectsOfType<Territory>().FirstOrDefault();
			if (territory?.Cells.Any() == true)
			{
				List<ICell> territoryCells = territory.Cells
				                                      .Where(x => ForagerAIHelpers.HasFoodOpportunity(ch, x))
				                                      .ToList();
				List<ICellExit> territoryPath = ch.PathBetween(territoryCells, 20, GetSuitabilityFunction(ch, true)).ToList();
				if (territoryPath.Any())
				{
					return (territoryPath.Last().Destination, territoryPath);
				}
			}

			Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = ch.AcquireTargetAndPath(
				x => x is ICell cell && ForagerAIHelpers.HasFoodOpportunity(ch, cell),
				20,
				GetSuitabilityFunction(ch));
			if (targetPath.Item1 is ICell target && targetPath.Item2.Any())
			{
				return (target, targetPath.Item2);
			}
		}

		return base.GetPath(ch);
	}
}
