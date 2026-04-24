using MudSharp.Character;
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

	public override string Show(ICharacter actor)
	{
		StringBuilder sb = new(base.Show(actor));
		sb.AppendLine("Predation: Only attacks while hungry, and only targets edible corpses.");
		return sb.ToString();
	}
}
