#nullable enable
using MudSharp.Effects.Concrete;
namespace MudSharp.NPC.AI;
public partial class AnimalAI
{
	private bool RespondToHuntThreat(ICharacter actor, AnimalHuntEffect hunt)
	{
		var threat = ObservedCharacters(actor).Where(x => x.Id != hunt.TargetId && !IsSociallyTrusted(actor, x))
			.FirstOrDefault(x => x.CombatTarget == actor || AwarenessThreatProg.ExecuteBool(false, actor, x) &&
			                    AssessPrey(actor, x).Score < HuntThreshold(actor, true));
		if (threat is null) return false;
		if (actor.Combat is not null) { hunt.Phase = AnimalHuntPhase.Abandoned; return false; }
		if (!TryAwarenessResponse(actor, threat) && !TryThreatResponse(actor, threat)) return false;
		actor.RemoveEffect(hunt);
		return true;
	}
	internal static bool BeginGroupHunt(ICharacter actor, ICharacter target) => actor is INPC npc &&
		npc.AIs.OfType<AnimalAI>().Any(ai => ai.Hunting.Enabled ? ai.BeginHunt(actor, target) : ai.TryHungryPredatorAttack(actor, target));

}
