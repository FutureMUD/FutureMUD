#nullable enable

using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class PullToMeleeMove : MeleeWeaponAttack
{
	public PullToMeleeMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target)
		: base(owner, weapon, attack, target)
	{
	}

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.PullToMelee;
	public override string Description => "Pulling an opponent into melee range";

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
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty,
			                          Weapon.WeaponType.AttackTrait, target, Assailant.OffensiveAdvantage);
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
			PullTargetIntoMelee(Assailant, target);
			if (Weapon.WeaponType.Name.EqualTo("Hooked Polearm") && target.RidingMount is not null)
			{
				target.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ are|is pulled from $1 by the hooked polearm!", target, target, target.RidingMount),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
				target.DoCombatKnockdown(Math.Max(1, (int)opposed.Degree));
			}
			return;
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				"$1 resist|resists being hauled into melee range.", Assailant, Assailant, target),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
	}

	internal static void PullTargetIntoMelee(ICharacter assailant, ICharacter target)
	{
		if (!assailant.ColocatedWith(target) || assailant.Combat != target.Combat)
		{
			return;
		}

		if (assailant.CombatTarget == target)
		{
			assailant.MeleeRange = true;
		}

		if (target.CombatTarget == assailant)
		{
			target.MeleeRange = true;
		}
	}
}
