#nullable enable
using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.NPC.AI.Groups;
namespace MudSharp.NPC.AI;
public partial class AnimalAI
{
	protected override void AwarenessStrategyChanged(AnimalAwarenessStrategyType strategy)
	{
		if (strategy.In(AnimalAwarenessStrategyType.Wimpy, AnimalAwarenessStrategyType.Skittish) &&
		    ThreatStrategy == AnimalThreatStrategyType.Passive)
			ThreatStrategy = AnimalThreatStrategyType.Flee;
	}
	protected override double HuntMotivationAdjustment(ICharacter actor) => IsStarving(actor) ? Hunting.StarvationAdjustment : 0;
	protected override string? HuntPolicyRejection(ICharacter actor, ICharacter target, bool continuing)
	{
		var people = Hunting.ClassificationProgId == 0
			? !AnimalLineageHelper.IsAnimal(target)
			: Gameworld.FutureProgs.Get(Hunting.ClassificationProgId)?.ExecuteBool(true, actor, target) != false;
		if (people && (Hunting.People == AnimalPeoplePreyPolicy.Never ||
		              Hunting.People == AnimalPeoplePreyPolicy.Desperate && !IsStarving(actor))) return "people policy";
		if (!PredatorAIHelpers.CouldEatAfterKilling(actor, target)) return "inedible prey";
		if (!PredatorAIHelpers.IsHungry(actor) || NpcSurvivalAIHelpers.IsThirsty(actor)) return "survival needs";
		return null;
	}
	protected override bool CanSeekHunt(ICharacter actor) => IsHungry(actor) &&
		!NpcSurvivalAIHelpers.IsThirsty(actor) && !IsGroupControlled(actor, GroupAIControlScope.Feeding) &&
		PredatorAIHelpers.FindLocalEdibleCorpse(actor) is null;
	protected override bool HuntPolicyExpired(ICharacter actor, CreaturePursuitEffect hunt) =>
		NpcSurvivalAIHelpers.IsThirsty(actor) || !IsHungry(actor);
	protected override CreaturePursuitEffect CreateHunt(ICharacter actor, ICharacter target) => new AnimalHuntEffect(actor, this, target);
	protected override bool HuntCompleted(ICharacter actor, CreaturePursuitEffect hunt) => PredatorAIHelpers.EatLocalCorpseIfHungry(actor);
}
