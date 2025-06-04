using MudSharp.Magic.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;
using MudSharp.Health;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine;
using MudSharp.Effects.Concrete;

namespace MudSharp.Combat.Moves;

public class MagicPowerAttackMove : WeaponAttackMove, IMagicPowerAttackMove
{
	public IMagicAttackPower AttackPower { get; }
	public override BuiltInCombatMoveType MoveType => AttackPower.MoveType;
	public override int Reach => AttackPower.Reach;

	public MagicPowerAttackMove(ICharacter attacker, IEnumerable<ICharacter> targets, IMagicAttackPower power) : base(
		power.WeaponAttack)
	{
		Assailant = attacker;
		foreach (var target in targets)
		{
			_characterTargets.Add(target);
		}

		AttackPower = power;
	}

	public MagicPowerAttackMove(ICharacter attacker, ICharacter target, IMagicAttackPower power) : base(
		power.WeaponAttack)
	{
		Assailant = attacker;
		_characterTargets.Add(target);
		AttackPower = power;
	}

	public override string Description =>
		$"Attacking {CharacterTargets.Select(x => x.HowSeen(x, flags: PerceiveIgnoreFlags.IgnoreSelf)).ListToString()} with the {AttackPower.Name.Colour(AttackPower.School.PowerListColour)} {AttackPower.School.SchoolAdjective} power.";

	public override double BaseDelay => AttackPower.BaseDelay;

	public override ExertionLevel AssociatedExertion => AttackPower.ExertionLevel;

	public override double StaminaCost => AttackPower.StaminaCost;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = CharacterTargets.First() };
		}

		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var attackRoll = Gameworld.GetCheck(Check)
		                          .Check(Assailant, CheckDifficulty, AttackPower.AttackerTrait, defenderMove.Assailant,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		if (defenderMove.Assailant is not IHaveWounds defenderHaveWounds)
		{
			throw new ApplicationException(
				$"Defender {defenderMove.Assailant.FrameworkItemType} ID {defenderMove.Assailant.Id:N0} did not have wounds in ResolveMove.");
		}

		DetermineTargetBodypart(defenderMove, attackRoll);
		AttackPower.UseAttackPower(this);

		var attackEmote =
			string.Format(
				Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, null,
					AttackPower.WeaponAttack, AttackPower.MoveType, attackRoll.Outcome, null), "",
				TargetBodypart.FullDescription());

		if (defenderMove is HelplessDefenseMove || defenderMove is TooExhaustedMove)
		{
			return ResolveHelplessDefense(defenderMove, attackRoll, defenderHaveWounds, attackEmote);
		}

		var ward = defenderMove as WardDefenseMove;
		WardResult wardResult = null;
		if (ward != null)
		{
			wardResult = ResolveWard(ward);
			if (wardResult.WardSucceeded)
			{
				Assailant.OutputHandler.Handle(
					new EmoteOutput(new Emote($"{attackEmote}{wardResult.WardEmotes}".Fullstop(), Assailant,
							Assailant, defenderHaveWounds, null, null, wardResult.WardWeapon?.Parent),
						style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
				return new CombatMoveResult { RecoveryDifficulty = RecoveryDifficultyFailure };
			}

			var newEffect = new WardBeaten(defenderMove.Assailant, defenderMove.Assailant.Combat);
			defenderMove.Assailant.AddEffect(newEffect);
			defenderMove = defenderMove.Assailant.ResponseToMove(this, Assailant);
			defenderMove.Assailant.RemoveEffect(newEffect);
		}

		if (defenderMove is HelplessDefenseMove || defenderMove is TooExhaustedMove)
		{
			return ResolveHelplessDefense(defenderMove, attackRoll, defenderHaveWounds, attackEmote);
		}

		if (defenderMove is DodgeMove dodge)
		{
			return ResolveDodge(defenderMove, attackRoll, defenderHaveWounds, attackEmote, dodge, wardResult);
		}

		if (defenderMove is ParryMove parry)
		{
			return ResolveParry(defenderMove, attackRoll, defenderHaveWounds, attackEmote, parry);
		}

		if (defenderMove is BlockMove block)
		{
			return ResolveBlock(block, attackRoll, defenderHaveWounds, attackEmote, wardResult);
		}

		throw new NotImplementedException(
			$"Unknown defenderMove in MagicPowerAttackMove.ResolveMove: {defenderMove.Description}");
	}

	private CombatMoveResult ResolveBlock(BlockMove defenderMove, CheckOutcome attackRoll,
		IHaveWounds defenderHaveWounds,
		string attackEmote, WardResult wardResult)
	{
		var blockBonus = BlockMove.GetBlockBonus(this, defenderMove.Alignment, defenderMove.Shield);
		var targetBonus = Assailant.GetBonusForDefendersFromTargeting();
		var blockCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .Check(defenderMove.Assailant,
			                          Attack.Profile.BaseBlockDifficulty.StageUp(defenderMove.DifficultStageUps),
			                          defenderMove.Shield.ShieldType.BlockTrait, Assailant,
			                          blockBonus + defenderMove.Shield.ShieldType.BlockBonus + targetBonus +
                                                  defenderMove.Assailant.DefensiveAdvantage -
                                                  GetPositionPenalty(Assailant.GetFacingFor(defenderMove.Assailant)));
                defenderMove.Assailant.DefensiveAdvantage = 0;
                var bsuccess = blockCheck.SuccessDegrees();
                if (bsuccess > 0)
                {
                        var adv = bsuccess switch
                        {
                                1 => Gameworld.GetStaticDouble("BlockDefensiveAdvantageMinorPass"),
                                2 => Gameworld.GetStaticDouble("BlockDefensiveAdvantagePass"),
                                _ => Gameworld.GetStaticDouble("BlockDefensiveAdvantageMajorPass")
                        };
                        defenderMove.Assailant.DefensiveAdvantage += adv;
                }
                if (blockCheck.FailureDegrees() >= 2)
                {
                        defenderMove.Assailant.SpendStamina(Gameworld.GetStaticDouble("BlockFailureAdditionalStamina"));
                }
                var result = new OpposedOutcome(attackRoll, blockCheck);
#if DEBUG
		Console.WriteLine(
			$"MeleeWeaponAttack Block Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif

		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)result.Degree;

		var wounds = new List<IWound>();
		var wardWounds = new List<IWound>();

		if (result.Outcome == OpposedOutcomeDirection.Proponent)
		{
			var finalDamage = new Damage
			{
				ActorOrigin = Assailant,
				LodgableItem = null,
				ToolOrigin = null,
				AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
				Bodypart = TargetBodypart,
				DamageAmount =
					Attack.Profile.DamageExpression.Evaluate(Assailant,
						context: TraitBonusContext.ArmedDamageCalculation) * 2 * Attack.Profile.BaseAngleOfIncidence /
					Math.PI,
				DamageType = Attack.Profile.DamageType,
				PainAmount =
					Attack.Profile.PainExpression.Evaluate(Assailant,
						context: TraitBonusContext.ArmedDamageCalculation) * 2 * Attack.Profile.BaseAngleOfIncidence /
					Math.PI,
				PenetrationOutcome =
					Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
					         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType),
						         defenderMove.Assailant,
						         Weapon),
				ShockAmount = 0,
				StunAmount =
					Attack.Profile.StunExpression.Evaluate(Assailant,
						context: TraitBonusContext.ArmedDamageCalculation) * 2 * Attack.Profile.BaseAngleOfIncidence /
					Math.PI
			};

			var blockEmote = Gameworld.CombatMessageManager.GetFailMessageFor(defenderMove.Assailant, Assailant,
				defenderMove.Shield.Parent, Attack, defenderMove.MoveType, blockCheck.Outcome, null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(blockEmote, "", TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant, defenderMove.Assailant, null,
						defenderMove.Shield.Parent, wardResult?.WardWeapon?.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			wardWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));
			wounds.AddRange(defenderHaveWounds.PassiveSufferDamage(finalDamage));
			wounds.AddRange(defenderMove.Shield.Parent.PassiveSufferDamage(finalDamage));
			CheckLodged(wounds);
			wardWounds.ProcessPassiveWounds();
			wounds.ProcessPassiveWounds();
		}
		else
		{
			var blockEmote = Gameworld.CombatMessageManager.GetMessageFor(defenderMove.Assailant, Assailant,
				defenderMove.Shield.Parent, Attack, defenderMove.MoveType, blockCheck.Outcome, null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(blockEmote, "", TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant, defenderMove.Assailant, null,
						defenderMove.Shield.Parent, wardResult?.WardWeapon?.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

			// The shield itself will still suffer damage
			var angleMultiplier = 0.0;
			switch (result.Degree)
			{
				case OpposedOutcomeDegree.None:
					angleMultiplier = 0.9;
					break;
				case OpposedOutcomeDegree.Marginal:
					angleMultiplier = 0.8;
					break;
				case OpposedOutcomeDegree.Minor:
					angleMultiplier = 0.7;
					break;
				case OpposedOutcomeDegree.Moderate:
					angleMultiplier = 0.6;
					break;
				case OpposedOutcomeDegree.Major:
					angleMultiplier = 0.5;
					break;
				case OpposedOutcomeDegree.Total:
					angleMultiplier = 0.4;
					break;
			}

			var finalAngle = Attack.Profile.BaseAngleOfIncidence * angleMultiplier;

			var finalDamage = new Damage
			{
				ActorOrigin = Assailant,
				LodgableItem = null,
				ToolOrigin = null,
				AngleOfIncidentRadians = finalAngle,
				Bodypart = null,
				DamageAmount =
					Attack.Profile.DamageExpression.Evaluate(Assailant,
						context: TraitBonusContext.ArmedDamageCalculation) * 2 * finalAngle / Math.PI,
				DamageType = Attack.Profile.DamageType,
				PainAmount = 0,
				PenetrationOutcome = Outcome.MajorFail,
				ShockAmount = 0,
				StunAmount = 0
			};
			wounds.AddRange(defenderMove.Shield.Parent.PassiveSufferDamage(finalDamage));
			CheckLodged(wounds);
			wounds.ProcessPassiveWounds();
		}

		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		defenderMove.Assailant.Body?.SetExertionToMinimumLevel(defenderMove.AssociatedExertion);
                var recovery = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure;
                if (blockCheck.FailureDegrees() == 2)
                {
                        recovery = recovery.StageUp(1);
                }
                else if (blockCheck.FailureDegrees() >= 3)
                {
                        recovery = recovery.StageUp(2);
                }
                return new CombatMoveResult
                {
                        MoveWasSuccessful = result.Outcome == OpposedOutcomeDirection.Proponent,
                        AttackerOutcome = attackRoll,
                        DefenderOutcome = blockCheck,
                        RecoveryDifficulty = recovery,
                        WoundsCaused = wounds,
                        SelfWoundsCaused = wardWounds
                };
	}

	private CombatMoveResult ResolveParry(ICombatMove defenderMove, CheckOutcome attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote, ParryMove parry)
	{
		var targetBonus = Assailant.GetBonusForDefendersFromTargeting();
		var parryCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .Check(defenderMove.Assailant,
			                          Attack.Profile.BaseParryDifficulty.StageUp(parry.DifficultStageUps),
			                          parry.Weapon.WeaponType.ParryTrait,
			                          Assailant,
			                          parry.Weapon.WeaponType.ParryBonus + targetBonus +
                                                  parry.Assailant.DefensiveAdvantage -
                                                  GetPositionPenalty(Assailant.GetFacingFor(defenderMove.Assailant)));
                parry.Assailant.DefensiveAdvantage = 0;
                var parryDelay = Gameworld.GetStaticDouble("ParryDelaySeconds") *
                                   (1.0 - parryCheck.CheckDegrees() / 6.0);
                Gameworld.Scheduler.DelayScheduleType(defenderMove.Assailant, ScheduleType.Combat,
                        TimeSpan.FromSeconds(parryDelay));
                var advantageLoss = parryCheck.CheckDegrees() - attackRoll.CheckDegrees();
                if (advantageLoss > 0)
                {
                        Assailant.DefensiveAdvantage -=
                                advantageLoss * Gameworld.GetStaticDouble("ParryAdvantagePenaltyPerDegree");
                }
                var result = new OpposedOutcome(attackRoll, parryCheck);
#if DEBUG
		Console.WriteLine(
			$"MeleeWeaponAttack Parry Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif

		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)result.Degree;

		var wounds = new List<IWound>();
		var wardWounds = new List<IWound>();

		if (result.Outcome == OpposedOutcomeDirection.Proponent)
		{
			// Parry is a little more forgiving if you fail than dodge
			var angleMultiplier = 1.0;
			switch (result.Degree)
			{
				case OpposedOutcomeDegree.None:
					angleMultiplier = 0.25;
					break;
				case OpposedOutcomeDegree.Marginal:
					angleMultiplier = 0.4;
					break;
				case OpposedOutcomeDegree.Minor:
					angleMultiplier = 0.55;
					break;
				case OpposedOutcomeDegree.Moderate:
					angleMultiplier = 0.7;
					break;
				case OpposedOutcomeDegree.Major:
					angleMultiplier = 0.85;
					break;
				case OpposedOutcomeDegree.Total:
					angleMultiplier = 1.0;
					break;
			}

			var finalAngle = Attack.Profile.BaseAngleOfIncidence * angleMultiplier;
			var finalDamage = new Damage
			{
				ActorOrigin = Assailant,
				LodgableItem = null,
				ToolOrigin = null,
				AngleOfIncidentRadians = finalAngle,
				Bodypart = TargetBodypart,
				DamageAmount =
					Attack.Profile.DamageExpression.Evaluate(Assailant,
						context: TraitBonusContext.ArmedDamageCalculation) * 2 * finalAngle / Math.PI,
				DamageType = Attack.Profile.DamageType,
				PainAmount =
					Attack.Profile.PainExpression.Evaluate(Assailant,
						context: TraitBonusContext.ArmedDamageCalculation) * 2 * finalAngle / Math.PI,
				PenetrationOutcome =
					Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
					         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType),
						         defenderMove.Assailant,
						         Weapon),
				ShockAmount = 0,
				StunAmount =
					Attack.Profile.StunExpression.Evaluate(Assailant,
						context: TraitBonusContext.ArmedDamageCalculation) * 2 * finalAngle / Math.PI
			};

			var parryEmote = Gameworld.CombatMessageManager.GetFailMessageFor(defenderMove.Assailant, Assailant,
				parry.Weapon.Parent, Attack, parry.MoveType, parryCheck.Outcome, null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(parryEmote, "", TargetBodypart.FullDescription())}".Fullstop(),
						Assailant, Assailant, defenderMove.Assailant, null, parry.Weapon.Parent, null),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			wounds.AddRange(defenderHaveWounds.PassiveSufferDamage(finalDamage));
			CheckLodged(wounds);
			wardWounds.AddRange(parry.Weapon.Parent.PassiveSufferDamage(finalDamage));
			CheckLodged(wardWounds);
			wounds.ProcessPassiveWounds();
			wardWounds.ProcessPassiveWounds();
		}
		else
		{
			var parryEmote = Gameworld.CombatMessageManager.GetMessageFor(defenderMove.Assailant, Assailant,
				parry.Weapon.Parent, Attack, parry.MoveType, parryCheck.Outcome, null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{parryEmote}".Fullstop(), Assailant, Assailant, defenderMove.Assailant,
						null, parry.Weapon.Parent, null), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));

			// The parrying weapon itself may still suffer damage
			var angleMultiplier = 0.0;
			switch (result.Degree)
			{
				case OpposedOutcomeDegree.None:
					angleMultiplier = 0.25;
					break;
				case OpposedOutcomeDegree.Marginal:
					angleMultiplier = 0.175;
					break;
				case OpposedOutcomeDegree.Minor:
					angleMultiplier = 0.1;
					break;
				case OpposedOutcomeDegree.Moderate:
					angleMultiplier = 0.05;
					break;
				case OpposedOutcomeDegree.Major:
					angleMultiplier = 0;
					break;
				case OpposedOutcomeDegree.Total:
					angleMultiplier = 0;
					break;
			}

			var finalAngle = Attack.Profile.BaseAngleOfIncidence * angleMultiplier;

			var finalDamage = new Damage
			{
				ActorOrigin = Assailant,
				LodgableItem = null,
				ToolOrigin = null,
				AngleOfIncidentRadians = finalAngle,
				Bodypart = null,
				DamageAmount = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI,
				DamageType = Attack.Profile.DamageType,
				PainAmount = 0,
				PenetrationOutcome = Outcome.MajorFail,
				ShockAmount = 0,
				StunAmount = 0
			};
			wardWounds.AddRange(parry.Weapon.Parent.PassiveSufferDamage(finalDamage));
			CheckLodged(wardWounds);
			wardWounds.ProcessPassiveWounds();

			if (!parry.Weapon.Parent.Destroyed && result.Degree == OpposedOutcomeDegree.Total)
			{
				if (!defenderMove.Assailant.CombatSettings.ForbiddenIntentions.HasFlag(
					    CombatMoveIntentions.Advantage))
				{
					//If our parry was a total success, give defense penalty to target that got parried
					//based on outcome of a second parry check.
					Assailant.DefensiveAdvantage -= (int)Gameworld.GetCheck(defenderMove.Check)
					                                              .Check(defenderMove.Assailant,
						                                              defenderMove.CheckDifficulty,
						                                              parry.Weapon.WeaponType.AttackTrait,
						                                              Assailant,
						                                              parry.Weapon.WeaponType.ParryBonus).Outcome - 1.0;
					defenderMove.Assailant.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								"@'s highly successful defense puts $1 into a position open to counterattack!",
								defenderMove.Assailant, defenderMove.Assailant, Assailant),
							style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
				}
			}
		}

		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		defenderMove.Assailant.Body?.SetExertionToMinimumLevel(defenderMove.AssociatedExertion);
		return new CombatMoveResult
		{
			MoveWasSuccessful = result.Outcome == OpposedOutcomeDirection.Proponent,
			AttackerOutcome = attackRoll,
			DefenderOutcome = parryCheck,
			RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
			WoundsCaused = wounds,
			SelfWoundsCaused = wardWounds
		};
	}

	private CombatMoveResult ResolveDodge(ICombatMove defenderMove, CheckOutcome attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote, DodgeMove dodge, WardResult wardResult)
	{
		var targetBonus = Assailant.GetBonusForDefendersFromTargeting();
		var dodgeCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .Check(defenderMove.Assailant,
			                          Attack.Profile.BaseDodgeDifficulty.StageUp(dodge.DifficultStageUps),
			                          Assailant, null,
                                                  targetBonus + dodge.Assailant.DefensiveAdvantage -
                                                  GetPositionPenalty(Assailant.GetFacingFor(defenderMove.Assailant)));
                dodge.Assailant.DefensiveAdvantage = 0;
                var dodgeDelay = Gameworld.GetStaticDouble("DodgeDelaySeconds") *
                                   (1.0 - dodgeCheck.CheckDegrees() / 6.0);
                Gameworld.Scheduler.DelayScheduleType(defenderMove.Assailant, ScheduleType.Combat,
                        TimeSpan.FromSeconds(dodgeDelay));
                var dsuccess = dodgeCheck.SuccessDegrees();
                if (dsuccess > 0)
                {
                        var adv = dsuccess switch
                        {
                                1 => Gameworld.GetStaticDouble("DodgeDefensiveAdvantageMinorPass"),
                                2 => Gameworld.GetStaticDouble("DodgeDefensiveAdvantagePass"),
                                _ => Gameworld.GetStaticDouble("DodgeDefensiveAdvantageMajorPass")
                        };
                        defenderMove.Assailant.DefensiveAdvantage += adv;
                }
                if (dodgeCheck.Outcome == Outcome.MajorFail &&
                    RandomUtilities.Random(0.0, 1.0) < Gameworld.GetStaticDouble("DodgeMajorFailFallChance") &&
                    defenderMove.Assailant.PositionState.Upright)
                {
                        defenderMove.Assailant.OutputHandler.Handle(
                                new EmoteOutput(new Emote("@ slip|slips and fall|falls to the ground while dodging!", defenderMove.Assailant)));
                        defenderMove.Assailant.SetPosition(PositionSprawled.Instance, PositionModifier.None,
                                defenderMove.Assailant.PositionTarget, null);
                }
                var result = new OpposedOutcome(attackRoll, dodgeCheck);
#if DEBUG
		Console.WriteLine(
			$"MeleeWeaponAttack Dodge Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif

		var wounds = new List<IWound>();
		var selfWounds = new List<IWound>();

		if (result.Outcome == OpposedOutcomeDirection.Proponent ||
		    result.Outcome == OpposedOutcomeDirection.Stalemate)
		{
			// Dodge is unforgiving if you lose the check
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

			Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
			Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)result.Degree;
			Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)result.Degree;
			Attack.Profile.DamageExpression.Formula.Parameters["quality"] = 0;
			Attack.Profile.StunExpression.Formula.Parameters["quality"] = 0;
			Attack.Profile.PainExpression.Formula.Parameters["quality"] = 0;

			var damageResult =
				Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.ArmedDamageCalculation);
			var stunResult =
				Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.ArmedDamageCalculation);
			var painResult =
				Attack.Profile.DamageExpression.Evaluate(Assailant, context: TraitBonusContext.ArmedDamageCalculation);

			var damage = new Damage
			{
				ActorOrigin = Assailant,
				LodgableItem = null,
				ToolOrigin = null,
				AngleOfIncidentRadians = finalAngle,
				Bodypart = TargetBodypart,
				DamageAmount = damageResult * 2.0 * finalAngle / Math.PI,
				DamageType = Attack.Profile.DamageType,
				PainAmount = painResult * 2.0 * finalAngle / Math.PI,
				PenetrationOutcome =
					Gameworld.GetCheck(CheckType.MeleeWeaponPenetrateCheck)
					         .Check(Assailant, GetPenetrationDifficulty(Attack.Profile.DamageType),
						         _characterTargets.First()),
				ShockAmount = 0,
				StunAmount = stunResult * 2.0 * finalAngle / Math.PI
			};

			var dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(defenderMove.Assailant, Assailant,
				null, Attack, dodge.MoveType, dodgeCheck.Outcome, null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(dodgeEmote, "", TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant, defenderMove.Assailant, null, null,
						wardResult?.WardWeapon?.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			selfWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));
			wounds.AddRange(defenderHaveWounds.PassiveSufferDamage(damage));
			CheckLodged(wounds);
			CheckLodged(selfWounds);
			wounds.ProcessPassiveWounds();
			selfWounds.ProcessPassiveWounds();
		}
		else
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(defenderMove.Assailant, Assailant,
				null, Attack, dodge.MoveType, dodgeCheck.Outcome, null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{wardResult?.WardEmotes ?? ""}{dodgeEmote}".Fullstop(), Assailant,
						Assailant, defenderMove.Assailant, null, null, wardResult?.WardWeapon?.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			selfWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));
			selfWounds.ProcessPassiveWounds();
			CheckLodged(selfWounds);
			if (Assailant.State != CharacterState.Dead && result.Degree == OpposedOutcomeDegree.Total)
			{
				if (!defenderMove.Assailant.CombatSettings.ForbiddenIntentions.HasFlag(CombatMoveIntentions.Flank))
				{
					var previousFacing = defenderMove.Assailant.GetFacingFor(Assailant);
					ImproveCombatPosition(defenderMove.Assailant, Assailant);
					var newFacing = defenderMove.Assailant.GetFacingFor(Assailant);

					defenderMove.Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("PerfectDodgeAdvantage");

					if (previousFacing != newFacing)
					{
						defenderMove.Assailant.OutputHandler.Handle(
							new EmoteOutput(
								new Emote(
									$"@'s masterful dodge puts &0 into a perfect position to counterattack!",
									defenderMove.Assailant, defenderMove.Assailant, Assailant),
								flags: OutputFlags.InnerWrap));
					}
				}
			}
		}

		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		defenderMove.Assailant.Body?.SetExertionToMinimumLevel(defenderMove.AssociatedExertion);
		return new CombatMoveResult
		{
			MoveWasSuccessful = result.Outcome == OpposedOutcomeDirection.Proponent,
			AttackerOutcome = attackRoll,
			DefenderOutcome = dodgeCheck,
			RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
			WoundsCaused = wounds,
			SelfWoundsCaused = selfWounds
		};
	}

	private CombatMoveResult ResolveHelplessDefense(ICombatMove defenderMove, CheckOutcome attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote)
	{
		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var result = new OpposedOutcome(attackRoll, Outcome.NotTested);
#if DEBUG
		Console.WriteLine(
			$"MagicPowerAttack HelplessDefenseMove Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var (damage, selfDamage) = GetDamagePlusSelfDamageForWeapon(Weapon, attackRoll, result.Degree,
			defenderHaveWounds, TargetBodypart, Attack.Profile.BaseAngleOfIncidence);

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}, and #1 %1|are|is struck on &1's {TargetBodypart.FullDescription()}!",
					Assailant, Assailant, defenderMove.Assailant, null, null, null), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		var wounds = defenderHaveWounds.PassiveSufferDamage(damage).ToList();
		CheckLodged(wounds);
		wounds.ProcessPassiveWounds();
		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		defenderMove.Assailant.Body?.SetExertionToMinimumLevel(defenderMove.AssociatedExertion);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			AttackerOutcome = attackRoll,
			RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
			WoundsCaused = wounds
		};
	}
}