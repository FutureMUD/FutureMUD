using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Combat.Moves;

public class MeleeWeaponAttack : WeaponAttackMove
{
	public MeleeWeaponAttack(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target)
		: base(attack)
	{
#if DEBUG
		if (!owner.ColocatedWith(target))
		{
			throw new ApplicationException("Melee attack at range!");
		}
#endif
		Assailant = owner;
		Weapon = weapon;
		if (target == null && owner.CombatTarget is ICharacter ct)
		{
			_characterTargets.Add(ct);
		}
		else if (target != null)
		{
			_characterTargets.Add(target);
		}
	}

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.UseWeaponAttack;

	public override string Description => "Attacking with a melee weapon";

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
	public override int Reach => Weapon.WeaponType.Reach; // TODO - natural reach

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = CharacterTargets.First() };
		}

		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var attackRoll = Gameworld.GetCheck(Check)
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty,
			                          Weapon.WeaponType.AttackTrait,
			                          defenderMove.Assailant, Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		if (defenderMove.Assailant is not IHaveWounds defenderHaveWounds)
		{
			throw new ApplicationException(
				$"Defender {defenderMove.Assailant.FrameworkItemType} ID {defenderMove.Assailant.Id:N0} did not have wounds in ResolveMove.");
		}

		DetermineTargetBodypart(defenderMove, attackRoll[CheckDifficulty]);

		var attackEmote =
			string.Format(
				Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, Weapon.Parent,
					Attack, MoveType, attackRoll[CheckDifficulty], null), "",
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
							Assailant, defenderHaveWounds, Weapon.Parent, null, wardResult.WardWeapon?.Parent),
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
			$"Unknown defenderMove in MeleeWeaponAttack.ResolveMove: {defenderMove.Description}");
	}

	private CombatMoveResult ResolveBlock(BlockMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll, IHaveWounds defenderHaveWounds,
		string attackEmote, WardResult wardResult)
	{
		var blockBonus = BlockMove.GetBlockBonus(this, defenderMove.Alignment, defenderMove.Shield);
		var targetBonus = Assailant.GetBonusForDefendersFromTargeting();
		var defenderDifficulty = Attack.Profile.BaseBlockDifficulty.StageUp(defenderMove.DifficultStageUps);
		var blockCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .CheckAgainstAllDifficulties(defenderMove.Assailant, defenderDifficulty,
			                          defenderMove.Shield.ShieldType.BlockTrait, Assailant,
			                          blockBonus - defenderMove.Assailant.GetDefensiveAdvantagePenaltyFromTargeting() +
			                          targetBonus +
			                          defenderMove.Shield.ShieldType.BlockBonus +
			                          defenderMove.Assailant.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(defenderMove.Assailant)));
		defenderMove.Assailant.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, blockCheck, CheckDifficulty, defenderDifficulty);
#if DEBUG
		Console.WriteLine(
			$"MeleeWeaponAttack Block Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif

		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.StunExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.PainExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;

		var wounds = new List<IWound>();
		var wardWounds = new List<IWound>();

		if (result.Outcome == OpposedOutcomeDirection.Proponent)
		{
			var finalDamage = new Damage
			{
				ActorOrigin = Assailant,
				LodgableItem = null,
				ToolOrigin = Weapon.Parent,
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
				defenderMove.Shield.Parent, Attack, defenderMove.MoveType, blockCheck[defenderDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(blockEmote, "", TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant, defenderMove.Assailant, Weapon.Parent,
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
				defenderMove.Shield.Parent, Attack, defenderMove.MoveType, blockCheck[defenderDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(blockEmote, "", TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant, defenderMove.Assailant, Weapon.Parent,
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
		return new CombatMoveResult
		{
			MoveWasSuccessful = result.Outcome == OpposedOutcomeDirection.Proponent,
			AttackerOutcome = attackRoll[CheckDifficulty],
			DefenderOutcome = blockCheck[defenderDifficulty],
			RecoveryDifficulty = attackRoll[CheckDifficulty].IsPass()
				? RecoveryDifficultySuccess
				: RecoveryDifficultyFailure,
			WoundsCaused = wounds,
			SelfWoundsCaused = wardWounds
		};
	}

	private CombatMoveResult ResolveParry(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote, ParryMove parry)
	{
		var targetBonus = Assailant.GetBonusForDefendersFromTargeting();
		var defenderDifficulty = Attack.Profile.BaseParryDifficulty.StageUp(parry.DifficultStageUps);
		var parryCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .CheckAgainstAllDifficulties(defenderMove.Assailant, defenderDifficulty,
			                          parry.Weapon.WeaponType.ParryTrait,
			                          Assailant,
			                          parry.Weapon.WeaponType.ParryBonus -
			                          defenderMove.Assailant.GetDefensiveAdvantagePenaltyFromTargeting() + targetBonus +
			                          parry.Assailant.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(defenderMove.Assailant)));
		parry.Assailant.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, parryCheck, CheckDifficulty, defenderDifficulty);
#if DEBUG
		Console.WriteLine(
			$"MeleeWeaponAttack Parry Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif

		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.StunExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.PainExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;

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
				ToolOrigin = Weapon.Parent,
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
				parry.Weapon.Parent, Attack, parry.MoveType, parryCheck[defenderDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(parryEmote, "", TargetBodypart.FullDescription())}".Fullstop(),
						Assailant, Assailant, defenderMove.Assailant, Weapon.Parent, parry.Weapon.Parent, null),
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
				parry.Weapon.Parent, Attack, parry.MoveType, parryCheck[defenderDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{parryEmote}".Fullstop(), Assailant, Assailant, defenderMove.Assailant,
						Weapon.Parent, parry.Weapon.Parent, null), style: OutputStyle.CombatMessage,
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
				if (!defenderMove.Assailant.CombatSettings.ForbiddenIntentions.HasFlag(CombatMoveIntentions.Disarm)
				    && Assailant.Body.CanBeDisarmed(Weapon.Parent, defenderMove.Assailant))
				{
					var secondCheck = Gameworld.GetCheck(defenderMove.Check)
					                           .Check(defenderMove.Assailant, defenderMove.CheckDifficulty,
						                           parry.Weapon.WeaponType.ParryTrait,
						                           Assailant, parry.Weapon.WeaponType.ParryBonus);
					if (secondCheck.IsPass() && RandomUtilities.Random(1, 2) == 1)
					{
						Assailant.OutputHandler.Handle(
							new EmoteOutput(new Emote("@ lose|loses &0's grip on $1, sending it flying!",
									Assailant, Assailant, Weapon.Parent), style: OutputStyle.CombatMessage,
								flags: OutputFlags.InnerWrap));
						Assailant.Body.Take(Weapon.Parent);
						Weapon.Parent.RoomLayer = Assailant.RoomLayer;
						Assailant.Location.Insert(Weapon.Parent);
						Weapon.Parent.AddEffect(new CombatNoGetEffect(Weapon.Parent,
							Assailant.Combat), TimeSpan.FromSeconds(90));
						Assailant.AddEffect(new CombatGetItemEffect(Assailant, Weapon.Parent));
					}
				}
				else if (!defenderMove.Assailant.CombatSettings.ForbiddenIntentions.HasFlag(
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
			AttackerOutcome = attackRoll[CheckDifficulty],
			DefenderOutcome = parryCheck[defenderDifficulty],
			RecoveryDifficulty = attackRoll[CheckDifficulty].IsPass()
				? RecoveryDifficultySuccess
				: RecoveryDifficultyFailure,
			WoundsCaused = wounds,
			SelfWoundsCaused = wardWounds
		};
	}

	private CombatMoveResult ResolveDodge(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote, DodgeMove dodge, WardResult wardResult)
	{
		var targetBonus = Assailant.GetBonusForDefendersFromTargeting();
		var defenderDifficulty = Attack.Profile.BaseDodgeDifficulty.StageUp(dodge.DifficultStageUps);
		var dodgeCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .CheckAgainstAllDifficulties(defenderMove.Assailant, defenderDifficulty, null,
			                          Assailant,
			                          targetBonus - defenderMove.Assailant.GetDefensiveAdvantagePenaltyFromTargeting() +
			                          dodge.Assailant.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(defenderMove.Assailant)));
		dodge.Assailant.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, dodgeCheck, CheckDifficulty, defenderDifficulty);
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
			var (damage, selfDamage) = GetDamagePlusSelfDamageForWeapon(Weapon, attackRoll[CheckDifficulty],
				result.Degree, defenderHaveWounds, TargetBodypart, finalAngle);

			var dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(defenderMove.Assailant, Assailant,
				Weapon.Parent, Attack, dodge.MoveType, dodgeCheck[defenderDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(dodgeEmote, "", TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant, defenderMove.Assailant, Weapon.Parent, null,
						wardResult?.WardWeapon?.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			selfWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));
			wounds.AddRange(defenderHaveWounds.PassiveSufferDamage(damage));
			wounds.AddRange(Weapon.Parent.PassiveSufferDamage(selfDamage));
			CheckLodged(wounds);
			CheckLodged(selfWounds);
			wounds.ProcessPassiveWounds();
			selfWounds.ProcessPassiveWounds();
		}
		else
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(defenderMove.Assailant, Assailant,
				Weapon.Parent, Attack, dodge.MoveType, dodgeCheck[defenderDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{wardResult?.WardEmotes ?? ""}{dodgeEmote}".Fullstop(), Assailant,
						Assailant, defenderMove.Assailant, Weapon.Parent, null, wardResult?.WardWeapon?.Parent),
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
			AttackerOutcome = attackRoll[CheckDifficulty],
			DefenderOutcome = dodgeCheck[defenderDifficulty],
			RecoveryDifficulty = attackRoll[CheckDifficulty].IsPass()
				? RecoveryDifficultySuccess
				: RecoveryDifficultyFailure,
			WoundsCaused = wounds,
			SelfWoundsCaused = selfWounds
		};
	}

	private CombatMoveResult ResolveHelplessDefense(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote)
	{
		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var result = new OpposedOutcome(attackRoll[CheckDifficulty], Outcome.NotTested);
#if DEBUG
		Console.WriteLine(
			$"MeleeWeaponAttack HelplessDefenseMove Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var (damage, selfDamage) = GetDamagePlusSelfDamageForWeapon(Weapon, attackRoll[CheckDifficulty], result.Degree,
			defenderHaveWounds, TargetBodypart, Attack.Profile.BaseAngleOfIncidence);

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}, and #1 %1|are|is struck on &1's {TargetBodypart.FullDescription()}!",
					Assailant, Assailant, defenderMove.Assailant, Weapon.Parent, null, null),
				style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		var wounds = defenderHaveWounds.PassiveSufferDamage(damage).ToList();
		wounds.AddRange(Weapon.Parent.PassiveSufferDamage(selfDamage));
		CheckLodged(wounds);
		wounds.ProcessPassiveWounds();
		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		defenderMove.Assailant.Body?.SetExertionToMinimumLevel(defenderMove.AssociatedExertion);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			AttackerOutcome = attackRoll[CheckDifficulty],
			RecoveryDifficulty = attackRoll[CheckDifficulty].IsPass()
				? RecoveryDifficultySuccess
				: RecoveryDifficultyFailure,
			WoundsCaused = wounds
		};
	}
}