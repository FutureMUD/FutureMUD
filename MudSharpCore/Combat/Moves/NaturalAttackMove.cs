using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class NaturalAttackMove : WeaponAttackMove
{
	public NaturalAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(attack.Attack)
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
	}

	#region Overrides of CombatMoveBase

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.NaturalWeaponAttack;
	public override string Description => "Attacking with a natural attack";

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
	public override CheckType Check => CheckType.NaturalWeaponAttack;
	public override int Reach => 0; // TODO - natural reach

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = Assailant.CombatTarget as ICharacter };
		}

		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var attackRoll = Gameworld.GetCheck(Check)
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty, null, defenderMove.Assailant,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		if (defenderMove.Assailant is not IHaveWounds defenderHaveWounds)
		{
			throw new ApplicationException(
				$"Defender {defenderMove.Assailant.FrameworkItemType} ID {defenderMove.Assailant.Id:N0} did not have wounds in ResolveMove.");
		}

		DetermineTargetBodypart(defenderMove, attackRoll[CheckDifficulty]);

		var attackEmote =
			string.Format(
				      Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, null, Attack,
					      MoveType, attackRoll[CheckDifficulty], Bodypart),
				      Bodypart.FullDescription(), TargetBodypart.FullDescription())
			      .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());


		if (defenderMove is HelplessDefenseMove || defenderMove is TooExhaustedMove)
		{
			return ResolveHelplessDefenseMove(defenderMove, attackRoll, defenderHaveWounds, attackEmote);
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
				return new CombatMoveResult
				{
					RecoveryDifficulty = RecoveryDifficultyFailure
				};
			}

			var newEffect = new WardBeaten(defenderMove.Assailant, defenderMove.Assailant.Combat);
			defenderMove.Assailant.AddEffect(newEffect);
			defenderMove = defenderMove.Assailant.ResponseToMove(this, Assailant);
			defenderMove.Assailant.RemoveEffect(newEffect);
		}

		if (defenderMove is DodgeMove dodge)
		{
			return ResolveDodgeMove(defenderMove, dodge, attackRoll, defenderHaveWounds, attackEmote, wardResult);
		}

		if (defenderMove is ParryMove parry)
		{
			return ResolveParryMove(defenderMove, parry, attackRoll, defenderHaveWounds, attackEmote);
		}

		if (defenderMove is BlockMove block)
		{
			return ResolveBlockMove(defenderMove, block, attackRoll, defenderHaveWounds, attackEmote, wardResult);
		}

		throw new NotImplementedException();
	}


	private CombatMoveResult ResolveBlockMove(ICombatMove defenderMove, BlockMove block,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote, WardResult wardResult)
	{
		var targetBonus = Assailant.GetBonusForDefendersFromTargeting();
		var defenderDifficulty = Attack.Profile.BaseBlockDifficulty.StageUp(block.DifficultStageUps);
		var blockCheck = Gameworld.GetCheck(defenderMove.Check)
		                          .CheckAgainstAllDifficulties(defenderMove.Assailant, defenderDifficulty,
			                          block.Shield.ShieldType.BlockTrait,
			                          Assailant,
			                          block.Shield.ShieldType.BlockBonus -
			                          defenderMove.Assailant.GetDefensiveAdvantagePenaltyFromTargeting() + targetBonus +
			                          block.Assailant.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(defenderMove.Assailant)));
		block.Assailant.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, blockCheck, CheckDifficulty, defenderDifficulty);
#if DEBUG
		Console.WriteLine(
			$"NaturalAttackMove Block Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var wounds = new List<IWound>();
		var selfWounds = new List<IWound>();

		if (result.Outcome == OpposedOutcomeDirection.Proponent ||
		    result.Outcome == OpposedOutcomeDirection.Stalemate)
		{
			var finalAngle = Attack.Profile.BaseAngleOfIncidence;
			var damages = GetDamagePlusSelfDamage(defenderMove.Assailant, Bodypart, TargetBodypart, null,
				attackRoll[CheckDifficulty], Attack.Profile.DamageType, finalAngle, NaturalAttack, result.Degree);
			var finalDamage = damages.Item1;
			var selfDamage = damages.Item2;

			var blockEmote = Gameworld.CombatMessageManager.GetFailMessageFor(defenderMove.Assailant, Assailant,
				block.Shield.Parent, Attack, block.MoveType, blockCheck[defenderDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(blockEmote, "", TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant, defenderMove.Assailant, null, block.Shield.Parent,
						wardResult?.WardWeapon?.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			selfWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));
			wounds.AddRange(defenderHaveWounds.PassiveSufferDamage(finalDamage));
			selfWounds.AddRange(Assailant.PassiveSufferDamage(selfDamage));
			wounds.AddRange(block.Shield.Parent.PassiveSufferDamage(finalDamage));
			wounds.ProcessPassiveWounds();
			selfWounds.ProcessPassiveWounds();
		}
		else
		{
			var blockEmote = Gameworld.CombatMessageManager.GetMessageFor(defenderMove.Assailant, Assailant,
				block.Shield.Parent, Attack, block.MoveType, blockCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{wardResult?.WardEmotes ?? ""}{blockEmote}".Fullstop(), Assailant,
						Assailant,
						defenderMove.Assailant, null, block.Shield.Parent, wardResult?.WardWeapon?.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			selfWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));

			// The blocking shield itself may still suffer damage
			var angleMultiplier = 0.0;
			switch (result.Degree)
			{
				case OpposedOutcomeDegree.None:
					angleMultiplier = 1;
					break;
				case OpposedOutcomeDegree.Marginal:
					angleMultiplier = 0.95;
					break;
				case OpposedOutcomeDegree.Minor:
					angleMultiplier = 0.9;
					break;
				case OpposedOutcomeDegree.Moderate:
					angleMultiplier = 0.85;
					break;
				case OpposedOutcomeDegree.Major:
					angleMultiplier = 0.8;
					break;
				case OpposedOutcomeDegree.Total:
					angleMultiplier = 0.75;
					break;
			}

			var finalAngle = Attack.Profile.BaseAngleOfIncidence * angleMultiplier;

			var damages = GetDamagePlusSelfDamage(defenderMove.Assailant, Bodypart, null,
				block.Shield.Parent.Material as ISolid, attackRoll[CheckDifficulty], Attack.Profile.DamageType,
				finalAngle,
				NaturalAttack, result.Degree);
			var finalDamage = damages.Item1;
			var selfDamage = damages.Item2;
			wounds.AddRange(block.Shield.Parent.PassiveSufferDamage(finalDamage));
			selfWounds.AddRange(Assailant.PassiveSufferDamage(selfDamage));
			wounds.ProcessPassiveWounds();
			selfWounds.ProcessPassiveWounds();
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
			SelfWoundsCaused = selfWounds
		};
	}

	private CombatMoveResult ResolveParryMove(ICombatMove defenderMove, ParryMove parry,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote)
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
			$"NaturalAttackMove Parry Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var wounds = new List<IWound>();
		var selfWounds = new List<IWound>();

		if (result.Outcome == OpposedOutcomeDirection.Proponent ||
		    result.Outcome == OpposedOutcomeDirection.Stalemate)
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
			// TODO

			var finalAngle = Attack.Profile.BaseAngleOfIncidence * angleMultiplier;
			var damages = GetDamagePlusSelfDamage(defenderMove.Assailant, Bodypart, TargetBodypart, null,
				attackRoll[CheckDifficulty], Attack.Profile.DamageType, finalAngle, NaturalAttack, result.Degree);
			var finalDamage = damages.Item1;
			var selfDamage = damages.Item2;

			var parryEmote = Gameworld.CombatMessageManager.GetFailMessageFor(defenderMove.Assailant, Assailant,
				parry.Weapon.Parent, Attack, parry.MoveType, parryCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(parryEmote, Bodypart.FullDescription(), TargetBodypart.FullDescription())}!",
						Assailant, Assailant, defenderMove.Assailant, null, parry.Weapon.Parent),
					style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			wounds.AddRange(defenderHaveWounds.PassiveSufferDamage(finalDamage));
			selfWounds.AddRange(Assailant.PassiveSufferDamage(selfDamage));
			wounds.AddRange(parry.Weapon.Parent.PassiveSufferDamage(finalDamage));
			wounds.ProcessPassiveWounds();
			selfWounds.ProcessPassiveWounds();
		}
		else
		{
			var parryEmote = Gameworld.CombatMessageManager.GetMessageFor(defenderMove.Assailant, Assailant,
				parry.Weapon.Parent, Attack, parry.MoveType, parryCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{string.Format(parryEmote, Bodypart.FullDescription(), TargetBodypart.FullDescription())}"
							.Fullstop(), Assailant, Assailant,
						defenderMove.Assailant, null, parry.Weapon.Parent), style: OutputStyle.CombatMessage,
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

			var damages = GetDamagePlusSelfDamage(defenderMove.Assailant, Bodypart, null,
				parry.Weapon.Parent.Material as ISolid, attackRoll[CheckDifficulty], Attack.Profile.DamageType,
				finalAngle,
				NaturalAttack, result.Degree);
			var finalDamage = damages.Item1;
			var selfDamage = damages.Item2;
			wounds.AddRange(parry.Weapon.Parent.PassiveSufferDamage(finalDamage));
			selfWounds.AddRange(Assailant.PassiveSufferDamage(selfDamage));
			wounds.ProcessPassiveWounds();
			selfWounds.ProcessPassiveWounds();

			if (!Assailant.State.HasFlag(CharacterState.Dead) && !parry.Weapon.Parent.Destroyed &&
			    result.Degree == OpposedOutcomeDegree.Total)
			{
				if (!defenderMove.Assailant.CombatSettings.ForbiddenIntentions.HasFlag(
					    CombatMoveIntentions.Advantage))
				{
					//If our parry was a total success, give defense penalty to target that got parried
					//based on outcome of a second parry check.
					Assailant.DefensiveAdvantage -= (int)Gameworld.GetCheck(defenderMove.Check)
					                                              .Check(defenderMove.Assailant,
						                                              defenderMove.CheckDifficulty,
						                                              parry.Weapon.WeaponType.ParryTrait,
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
			SelfWoundsCaused = selfWounds
		};
	}

	private CombatMoveResult ResolveDodgeMove(ICombatMove defenderMove, DodgeMove dodge,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote, WardResult wardResult)
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
			$"NaturalAttackMove Dodge Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
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
			var bodypart = defenderMove.Assailant.Body.RandomBodyPartGeometry(Attack.Orientation, Alignment,
				Assailant.GetFacingFor(defenderMove.Assailant, true));

			var damages = GetDamagePlusSelfDamage(defenderMove.Assailant, Bodypart, bodypart, null,
				attackRoll[CheckDifficulty], Attack.Profile.DamageType, finalAngle, NaturalAttack, result.Degree);
			var finalDamage = damages.Item1;
			var selfDamage = damages.Item2;

			var dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(defenderMove.Assailant, Assailant,
				null, Attack,
				dodge.MoveType, dodgeCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(dodgeEmote, Bodypart.FullDescription(), bodypart.FullDescription())}"
							.Fullstop(),
						Assailant, Assailant, defenderMove.Assailant, null, null, wardResult?.WardWeapon?.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			selfWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));
			wounds.AddRange(defenderHaveWounds.PassiveSufferDamage(finalDamage));
			selfWounds.AddRange(Assailant.PassiveSufferDamage(selfDamage));
			wounds.ProcessPassiveWounds();
			selfWounds.ProcessPassiveWounds();
		}
		else
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(defenderMove.Assailant, Assailant, null,
				Attack, dodge.MoveType, dodgeCheck[defenderDifficulty], Bodypart);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"{attackEmote}{wardResult?.WardEmotes ?? ""}{dodgeEmote}".Fullstop(), Assailant,
						Assailant,
						defenderMove.Assailant, null, null, wardResult?.WardWeapon?.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			selfWounds.AddRange(ProcessWardFreeAttack(Assailant, defenderMove.Assailant, wardResult));
			selfWounds.ProcessPassiveWounds();

			if (!Assailant.State.HasFlag(CharacterState.Dead) && result.Degree == OpposedOutcomeDegree.Total)
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

	protected CombatMoveResult ResolveHelplessDefenseMove(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		IHaveWounds defenderHaveWounds, string attackEmote)
	{
		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var result = new OpposedOutcome(attackRoll[CheckDifficulty], Outcome.NotTested);
#if DEBUG
		Console.WriteLine(
			$"NaturalAttackMove HelplessDefenseMove Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif
		var finalAngle = Attack.Profile.BaseAngleOfIncidence;
		var damages = GetDamagePlusSelfDamage(defenderMove.Assailant, Bodypart, TargetBodypart, null,
			attackRoll[CheckDifficulty],
			Attack.Profile.DamageType, finalAngle, NaturalAttack, result.Degree);
		var finalDamage = damages.Item1;
		var selfDamage = damages.Item2;

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}, and #1 %1|are|is struck on &1's {TargetBodypart.FullDescription()}!",
					Assailant,
					Assailant, defenderMove.Assailant), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		var wounds = defenderHaveWounds.PassiveSufferDamage(finalDamage).ToList();
		var selfWounds = Assailant.PassiveSufferDamage(selfDamage);
		wounds.ProcessPassiveWounds();
		selfWounds.ProcessPassiveWounds();
		Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
		defenderMove.Assailant.Body?.SetExertionToMinimumLevel(defenderMove.AssociatedExertion);
		return new CombatMoveResult
		{
			MoveWasSuccessful = result.Outcome == OpposedOutcomeDirection.Proponent,
			AttackerOutcome = attackRoll[CheckDifficulty],
			RecoveryDifficulty = attackRoll[CheckDifficulty].IsPass()
				? RecoveryDifficultySuccess
				: RecoveryDifficultyFailure,
			WoundsCaused = wounds,
			SelfWoundsCaused = selfWounds
		};
	}

	#endregion
}