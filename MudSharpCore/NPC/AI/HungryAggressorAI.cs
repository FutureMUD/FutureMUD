using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Models;
using System.Text;

namespace MudSharp.NPC.AI;

public class HungryAggressorAI : AggressorAI
{
	public HungryAggressorAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
	}

	private HungryAggressorAI(IFuturemud gameworld, string name) : base(gameworld, name, "HungryAggressor")
	{
	}

	private HungryAggressorAI()
	{
	}

	public static void RegisterLoader()
	{
		RegisterAIType("HungryAggressor", (ai, gameworld) => new HungryAggressorAI(ai, gameworld));
		RegisterAIBuilderInformation("hungryaggressor", (gameworld, name) => new HungryAggressorAI(gameworld, name),
			new HungryAggressorAI().HelpText);
	}

	public override bool CheckForAttack(ICharacter aggressor, ICharacter target)
	{
		return PredatorAIHelpers.CheckForAttack(aggressor, target, WillAttackProg, EngageDelayDiceExpression,
			EngageEmote, true);
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.TenSecondTick:
			case EventType.CharacterEnterCellFinish:
			case EventType.LeaveCombat:
				ICharacter ch = (ICharacter)arguments[0];
				if (NpcSurvivalAIHelpers.TryDrinkIfThirsty(ch))
				{
					return true;
				}

				if (NpcSurvivalAIHelpers.IsThirsty(ch))
				{
					return false;
				}

				if (PredatorAIHelpers.EatLocalCorpseIfHungry(ch))
				{
					return true;
				}

				break;
			case EventType.CharacterEnterCellWitness:
				ch = (ICharacter)arguments[3];
				if (NpcSurvivalAIHelpers.TryDrinkIfThirsty(ch))
				{
					return true;
				}

				if (NpcSurvivalAIHelpers.IsThirsty(ch))
				{
					return false;
				}

				if (PredatorAIHelpers.EatLocalCorpseIfHungry(ch))
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
				case EventType.CharacterEnterCellFinish:
				case EventType.LeaveCombat:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
		sb.AppendLine("Predation: Only attacks while hungry, only targets edible corpses, and eats local corpses before hunting.");
		return sb.ToString();
	}
}
