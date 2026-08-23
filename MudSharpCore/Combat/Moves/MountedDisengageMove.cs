#nullable enable

using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public sealed class MountedDisengageMove : CombatMoveBase
{
	public override string Description => "using mounted momentum to break out of melee";
	public override double StaminaCost => ChargeToMeleeMove.MoveStaminaCost(Assailant);
	public override double BaseDelay => 0.0;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var context = MountedCombatService.Instance.ResolveContext(Assailant);
		if (context is null || Assailant.CombatTarget is not ICharacter target)
		{
			return CombatMoveResult.Irrelevant;
		}

		var attackCheckType = MountedCombatService.Instance.ChargeCheckType(context);
		var attackCheck = Gameworld.GetCheck(attackCheckType);
		if (attackCheck.Type != attackCheckType)
		{
			attackCheck = Gameworld.GetCheck(CheckType.GenericSkillCheck);
		}

		var defenseCheck = Gameworld.GetCheck(CheckType.OpposeMountedChargeCheck);
		if (defenseCheck.Type != CheckType.OpposeMountedChargeCheck)
		{
			defenseCheck = Gameworld.GetCheck(CheckType.GenericSkillCheck);
		}

		var attackRoll = attackCheck.Check(Assailant, Difficulty.Normal, null, target, context.Momentum);
		var defenseRoll = defenseCheck.Check(target, Difficulty.Normal, null, Assailant);
		var opposed = new OpposedOutcome(attackRoll, defenseRoll);
		if (opposed.Outcome == OpposedOutcomeDirection.Opponent)
		{
			target.OffensiveAdvantage += 1.0 + (int)opposed.Degree;
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ try|tries to sweep clear of $1 on $2, but $1 check|checks the breakaway!", Assailant,
				Assailant, target, context.Conveyance), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
			return new CombatMoveResult
			{
				MoveWasSuccessful = false,
				AttackerOutcome = attackRoll,
				DefenderOutcome = defenseRoll,
				RecoveryDifficulty = Difficulty.Hard
			};
		}

		Assailant.MeleeRange = false;
		if (target.CombatTarget == Assailant)
		{
			target.MeleeRange = false;
		}

		Assailant.DefensiveAdvantage += 1.0 + (int)opposed.Degree;
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ use|uses $2's momentum to sweep past $1 and break clear of melee", Assailant,
			Assailant, target, context.Conveyance), style: OutputStyle.CombatMessage,
			flags: OutputFlags.InnerWrap));
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			AttackerOutcome = attackRoll,
			DefenderOutcome = defenseRoll
		};
	}
}
