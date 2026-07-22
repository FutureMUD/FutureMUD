#nullable enable

using MudSharp.Character;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class PullToMeleeUnarmedMove : NaturalAttackMove
{
	public PullToMeleeUnarmedMove(ICharacter owner, INaturalAttack attack, ICharacter target)
		: base(owner, attack, target)
	{
	}

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.PullToMeleeUnarmed;
	public override string Description => "Pulling an opponent into melee range with a natural attack";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var result = base.ResolveMove(defenderMove);
		var target = CharacterTargets.First();
		if (!ShouldResolvePull(result, target))
		{
			return result;
		}

		ResolvePullContest(target);
		return result;
	}

	private bool ShouldResolvePull(CombatMoveResult result, ICharacter target)
	{
		return !target.State.HasFlag(CharacterState.Dead) &&
		       result.MoveWasSuccessful &&
		       result.AttackerOutcome.IsPass() &&
		       Attack is ISecondaryDifficultyAttack;
	}

	private void ResolvePullContest(ICharacter target)
	{
		var attackRoll = Gameworld.GetCheck(CheckType.ForcedMovementCheck)
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty, null, target,
			                          Assailant.OffensiveAdvantage);
		var defenseDifficulty = ((ISecondaryDifficultyAttack)Attack).SecondaryDifficulty;
		var defenseRoll = Gameworld.GetCheck(CheckType.OpposeForcedMovementCheck)
		                           .CheckAgainstAllDifficulties(target, defenseDifficulty, null, Assailant,
			                           target.DefensiveAdvantage - GetPositionPenalty(Assailant.GetFacingFor(target)));
		Assailant.OffensiveAdvantage = 0;
		target.DefensiveAdvantage = 0;

		var opposed = new OpposedOutcome(attackRoll, defenseRoll, CheckDifficulty, defenseDifficulty);
		if (opposed.Outcome == OpposedOutcomeDirection.Proponent)
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ haul|hauls $1 into melee range!", Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			PullToMeleeMove.PullTargetIntoMelee(Assailant, target);
			return;
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				"$1 resist|resists being hauled into melee range.", Assailant, Assailant, target),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
	}
}
