using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;

namespace MudSharp.Combat.Moves;

public class TakedownMove : WeaponAttackMove
{
	public TakedownMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(attack.Attack)
	{
		Assailant = owner;
		NaturalAttack = attack;
		if (target == null && owner.CombatTarget is ICharacter ct)
		{
			_characterTargets.Add(ct);
		}
		else if (target != null)
		{
			_characterTargets.Add(target);
		}

		CharacterTarget = _characterTargets.Single();
		TargetBodypart = target.Body.RandomBodyPartGeometry(Attack.Orientation, Attack.Alignment, Facing.Front, false);
	}

	public ICharacter CharacterTarget { get; set; }

	public Alignment Alignment => Bodypart.Alignment.LeftRightOnly() == Alignment.Left
		? Attack.Alignment.SwitchLeftRight()
		: Attack.Alignment;

	public INaturalAttack NaturalAttack { get; set; }
	public IBodypart Bodypart => NaturalAttack.Bodypart;

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
	public override Difficulty CheckDifficulty => Attack.Profile.BaseAttackerDifficulty;
	public override CheckType Check => CheckType.TakedownCheck;
	public override int Reach => 0; // TODO - natural reach

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.TakedownMove;

	public override string Description => "Performing a takedown";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var attackRoll = Gameworld.GetCheck(Check)
		                          .Check(Assailant, CheckDifficulty, defenderMove.Assailant, null,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		if (defenderMove.Assailant is not IHaveWounds defenderHaveWounds)
		{
			throw new ApplicationException(
				$"Defender {defenderMove.Assailant.FrameworkItemType} ID {defenderMove.Assailant.Id:N0} did not have wounds in ResolveMove.");
		}

		DetermineTargetBodypart(defenderMove, attackRoll);

		var attackEmote =
			string.Format(
				      Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, null, Attack,
					      MoveType, attackRoll.Outcome, Bodypart),
				      Bodypart.FullDescription(), TargetBodypart.FullDescription())
			      .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(attackEmote, Assailant, Assailant, CharacterTarget)));

		var result = new OpposedOutcome(attackRoll, Outcome.NotTested);
		var degree = result.Degree;

		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(NaturalAttack);
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.StunExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(NaturalAttack);
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.PainExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(NaturalAttack);

		var damageResult =
			Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.UnarmedDamageCalculation);
		var stunResult =
			Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.UnarmedDamageCalculation);
		var painResult =
			Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.UnarmedDamageCalculation);

		CharacterTarget.SetPosition(PositionSprawled.Instance, PositionModifier.None, Assailant, null);
		Assailant.SetPosition(PositionProne.Instance, PositionModifier.None, CharacterTarget, null);
		Gameworld.Scheduler.DelayScheduleType(CharacterTarget, ScheduleType.Combat,
			TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("TakedownReelTime")));
		CharacterTarget.AddEffect(new Staggered(CharacterTarget),
			TimeSpan.FromMilliseconds(Gameworld.GetStaticDouble("StaggeringBlowStaggerEffectLength")));
		CharacterTarget.DefensiveAdvantage -= Gameworld.GetStaticInt("TakedownDefensiveAdvantage");

		var wounds = CharacterTarget.PassiveSufferDamage(new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = null,
			AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
			Bodypart = TargetBodypart,
			DamageAmount = damageResult * 2.0 * Attack.Profile.BaseAngleOfIncidence / Math.PI,
			DamageType = Attack.Profile.DamageType,
			PainAmount = painResult * 2.0 * Attack.Profile.BaseAngleOfIncidence / Math.PI,
			PenetrationOutcome = Outcome.NotTested,
			ShockAmount = 0,
			StunAmount = stunResult * 2.0 * Attack.Profile.BaseAngleOfIncidence / Math.PI
		});
		wounds.ProcessPassiveWounds();

		return new CombatMoveResult
		{
			AttackerOutcome = attackRoll,
			DefenderOutcome = Outcome.NotTested,
			RecoveryDifficulty = Attack.RecoveryDifficultySuccess,
			MoveWasSuccessful = true,
			SelfWoundsCaused = Enumerable.Empty<IWound>(),
			WoundsCaused = wounds
		};
	}
}