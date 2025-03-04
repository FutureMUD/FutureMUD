using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;

namespace MudSharp.Combat.Moves;

public class ClinchAttackMove : WeaponAttackMove
{
	public ClinchAttackMove(ICharacter assailant, ICharacter target, IWeaponAttack attack, IMeleeWeapon weapon)
		: base(attack)
	{
		Assailant = assailant;
		CharacterTarget = target;
		Weapon = weapon;
	}

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.ClinchAttack;

	public override string Description => "Attacking within a clinch";

	public override int Reach => 0;

	public ICharacter CharacterTarget { get; set; }

	public Alignment Alignment
		=> Assailant.Body.WieldedHand(Weapon.Parent).Alignment.LeftRightOnly() == Alignment.Left
			? Attack.Alignment.SwitchLeftRight()
			: Attack.Alignment;

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant, Attack);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}


	public static double MoveStaminaCost(ICharacter assailant, IWeaponAttack attack)
	{
		return attack.StaminaCost * CombatBase.PowerMoveStaminaMultiplier(assailant);
	}

	public override double BaseDelay => Attack.BaseDelay;
	public override ExertionLevel AssociatedExertion => Attack.ExertionLevel;
	public override Difficulty RecoveryDifficultyFailure => Attack.RecoveryDifficultyFailure;
	public override Difficulty RecoveryDifficultySuccess => Attack.RecoveryDifficultySuccess;

	public override Difficulty CheckDifficulty =>
		Assailant.GetDifficultyForTool(Weapon.Parent, Attack.Profile.BaseAttackerDifficulty);

	public override CheckType Check => CheckType.MeleeWeaponCheck;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = CharacterTarget };
		}

		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var attackRoll = Gameworld.GetCheck(Check)
		                          .Check(Assailant, CheckDifficulty, Weapon.WeaponType.AttackTrait,
			                          defenderMove.Assailant,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		Assailant.OffensiveAdvantage = 0;
		if (defenderMove.Assailant is not IHaveWounds defenderHaveWounds)
		{
			throw new ApplicationException(
				$"Defender {defenderMove.Assailant.FrameworkItemType} ID {defenderMove.Assailant.Id:N0} did not have wounds in ResolveMove.");
		}

		DetermineTargetBodypart(defenderMove, attackRoll);

		var attackEmote =
			string.Format(
				Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, Weapon.Parent,
					Attack, BuiltInCombatMoveType.ClinchAttack, attackRoll.Outcome, null), "",
				TargetBodypart.FullDescription());

		if (defenderMove is HelplessDefenseMove || defenderMove is TooExhaustedMove)
		{
			return ResolveHelplessDefense(defenderMove, attackRoll, attackEmote);
		}

		if (defenderMove is DodgeMove dodge)
		{
			return ResolveDodge(attackRoll, attackEmote, dodge);
		}

		throw new NotImplementedException(
			$"Unknown move type in response to clinch attack move - {defenderMove.Description}");
	}

	private CombatMoveResult ResolveDodge(CheckOutcome attackRoll, string attackEmote, DodgeMove dodge)
	{
		var dodgeCheck = Gameworld.GetCheck(dodge.Check)
		                          .Check(CharacterTarget, Attack.Profile.BaseDodgeDifficulty, Assailant, null,
			                          dodge.Assailant.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
		dodge.Assailant.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, dodgeCheck);
#if DEBUG
		Console.WriteLine($"ClinchAttack Dodge Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var finalDegree = result.Outcome == OpposedOutcomeDirection.Opponent ? 0 : (int)result.Degree;
		var angleMultiplier = 1.0;
		switch (result.Degree)
		{
			case OpposedOutcomeDegree.None:
				angleMultiplier = 0.5;
				break;
			case OpposedOutcomeDegree.Marginal:
				angleMultiplier = 0.6;
				break;
			case OpposedOutcomeDegree.Minor:
				angleMultiplier = 0.7;
				break;
			case OpposedOutcomeDegree.Moderate:
				angleMultiplier = 0.8;
				break;
			case OpposedOutcomeDegree.Major:
				angleMultiplier = 0.9;
				break;
			case OpposedOutcomeDegree.Total:
				angleMultiplier = 1.0;
				break;
		}

		var finalAngle = Attack.Profile.BaseAngleOfIncidence * angleMultiplier;
		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = finalDegree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = finalDegree;
		Attack.Profile.StunExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = finalDegree;
		Attack.Profile.PainExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;

		var finalDamage = new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = Weapon.Parent,
			AngleOfIncidentRadians = finalAngle,
			Bodypart = TargetBodypart,
			DamageAmount = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI,
			DamageType = Attack.Profile.DamageType,
			PainAmount = Attack.Profile.PainExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI,
			PenetrationOutcome =
				Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
				         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType),
					         Weapon.WeaponType.AttackTrait, CharacterTarget),
			ShockAmount = 0,
			StunAmount = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI
		};

		var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant, Weapon.Parent,
			Attack, BuiltInCombatMoveType.ClinchDodge, dodgeCheck.Outcome, null);
		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					$"{attackEmote}{string.Format(dodgeEmote, "", TargetBodypart.FullDescription())}".Fullstop(),
					Assailant, Assailant, CharacterTarget, Weapon.Parent, null, null), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		var wounds = CharacterTarget.PassiveSufferDamage(finalDamage).ToList();
		wounds.ProcessPassiveWounds();
		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		CharacterTarget.Body?.SetExertionToMinimumLevel(dodge.AssociatedExertion);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
			AttackerOutcome = attackRoll,
			DefenderOutcome = dodgeCheck,
			WoundsCaused = wounds
		};
	}

	private CombatMoveResult ResolveHelplessDefense(ICombatMove defenderMove, CheckOutcome attackRoll,
		string attackEmote)
	{
		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var result = new OpposedOutcome(attackRoll, Outcome.NotTested);
#if DEBUG
		Console.WriteLine(
			$"MeleeWeaponAttack HelplessDefenseMove Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var finalAngle = Attack.Profile.BaseAngleOfIncidence;
		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.StunExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.PainExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;

		var finalDamage = new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = Weapon.Parent,
			AngleOfIncidentRadians = finalAngle,
			Bodypart = TargetBodypart,
			DamageAmount = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI,
			DamageType = Attack.Profile.DamageType,
			PainAmount = Attack.Profile.PainExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI,
			PenetrationOutcome =
				Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
				         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType),
					         Weapon.WeaponType.AttackTrait, defenderMove.Assailant),
			ShockAmount = 0,
			StunAmount = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI
		};

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}, and #1 %1|are|is struck on &1's {TargetBodypart.FullDescription()}!",
					Assailant, Assailant, CharacterTarget, Weapon.Parent, null, null), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));

		var wounds = CharacterTarget.PassiveSufferDamage(finalDamage).ToList();
		wounds.ProcessPassiveWounds();
		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		CharacterTarget.Body?.SetExertionToMinimumLevel(defenderMove.AssociatedExertion);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
			AttackerOutcome = attackRoll,
			WoundsCaused = wounds
		};
	}
}