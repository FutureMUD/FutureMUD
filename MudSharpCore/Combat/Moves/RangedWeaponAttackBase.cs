using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using ExpressionEngine;
using MudSharp.GameItems;
using MudSharp.RPG.Law;

namespace MudSharp.Combat.Moves;

public abstract class RangedWeaponAttackBase : CombatMoveBase, IRangedWeaponAttackMove
{
	protected RangedWeaponAttackBase(ICharacter assailant, IPerceiver target, IRangedWeapon weapon)
	{
		Assailant = assailant;
		if (target is ICharacter tch)
		{
			_characterTargets.Add(tch);
		}

		if (target != null)
		{
			_targets.Add(target);
		}

		Weapon = weapon;
	}

	private void DetermineTargetBodypart(IHaveABody target, Outcome outcome)
	{
		if (target.Body == null)
		{
			return;
		}

		if (Assailant.TargettedBodypart is not null && !target.Body.Bodyparts.Contains(Assailant.TargettedBodypart))
		{
			Assailant.TargettedBodypart = null;
		}

		if (Assailant.TargettedBodypart == null || Assailant.MeleeRange)
		{
			TargetBodypart = target.Body.RandomBodyPartGeometry(Orientation.Centre, Alignment.Front, Facing.Front);
			return;
		}

		switch (outcome)
		{
			case Outcome.MinorPass:
				TargetBodypart =
					target.Body.RandomBodyPartGeometry(
						Assailant.TargettedBodypart.Orientation.RaiseUp(Dice.Roll(1, 2) == 1 ? 1 : -1),
						Assailant.TargettedBodypart.Alignment, Facing.Front);
				return;
			case Outcome.Pass:
				TargetBodypart =
					target.Body.RandomBodyPartGeometry(Assailant.TargettedBodypart.Orientation,
						Assailant.TargettedBodypart.Alignment, Facing.Front);
				return;
			case Outcome.MajorPass:
				TargetBodypart = Assailant.TargettedBodypart;
				return;
			default:
				TargetBodypart = target.Body.RandomBodyPartGeometry(Orientation.Centre, Alignment.Front, Facing.Front);
				return;
		}
	}

	private double GetPenaltyForTargeting(IBody target, int range)
	{
		if (Assailant.TargettedBodypart == null || target?.Bodyparts.Any() != true)
		{
			return 0.0;
		}

		if (Assailant.MeleeRange)
		{
			return 0.0;
		}

		TargetExpression.Parameters["range"] = range;
		TargetExpression.Parameters["average"] = target.Bodyparts.Average(x => x.RelativeHitChance);
		TargetExpression.Parameters["chance"] = Assailant.TargettedBodypart.RelativeHitChance;
#if DEBUG
		Console.WriteLine(
			$"Target Range Penalty r{range} a{target.Bodyparts.Average(x => x.RelativeHitChance)} c{Assailant.TargettedBodypart.RelativeHitChance}: {Convert.ToDouble(TargetExpression.Evaluate())}");
#endif
		return Convert.ToDouble(TargetExpression.Evaluate());
	}

	private static Expression _targetExpression;

	public static Expression TargetExpression => _targetExpression ??= new Expression(Futuremud.Games.First()
		.GetStaticConfiguration("RangedCombatTargetBodypartExpression"));

	public bool SuppressAttackMessage { get; init; }

	protected abstract BuiltInCombatMoveType MoveType { get; }

	public IRangedWeapon Weapon { get; set; }

	public IBodypart TargetBodypart { get; set; }

	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	public override double StaminaCost => Weapon.WeaponType.StaminaToFire;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var target = CharacterTargets.FirstOrDefault() ?? _targets.FirstOrDefault();
		if (target == null)
		{
			Weapon.Fire(Assailant, null, Outcome.NotTested, Outcome.NotTested,
				new OpposedOutcome(Outcome.NotTested, Outcome.NotTested), null, null, null);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = RecoveryDifficultySuccess,
				AttackerOutcome = Outcome.NotTested,
				DefenderOutcome = Outcome.NotTested
			};
		}

		var targetHb = target as IHaveABody;
		Weapon.WeaponType.AccuracyBonusExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
		var range = Assailant.DistanceBetween(target, 10);
		Weapon.WeaponType.AccuracyBonusExpression.Formula.Parameters["range"] = range;
		Weapon.WeaponType.AccuracyBonusExpression.Formula.Parameters["inmelee"] = Assailant.MeleeRange ? 1 : 0;
		Weapon.WeaponType.AccuracyBonusExpression.Formula.Parameters["aim"] = Assailant.Aim?.AimPercentage ?? 0;
		var check = Gameworld.GetCheck(Weapon.WeaponType.FireCheck);
		var difficulty = Assailant.GetDifficultyForTool(Weapon.Parent, Weapon.WeaponType.BaseAimDifficulty)
		                          .StageUp(Assailant.TargettedBodypart != null ? 1 : 0);

		//Shot might be harder based on how cluttered the environment is
		var visionModifiers = GetSpottingModifier(target.Location.Terrain(Assailant).SpotDifficulty);

		//Shot might be harder based on how dark the target environment is
		var targetIllumination = target.Location.CurrentIllumination(Assailant);
		var illuminationDifficulty =
			Assailant.Gameworld.LightModel.GetSightDifficulty(targetIllumination *
			                                                  Assailant.Race.IlluminationPerceptionMultiplier);
		visionModifiers += GetSpottingModifier(illuminationDifficulty);

		var targetAsCharacter = target as ICharacter;
		var coverDifficulty = Difficulty.Automatic;
		var bUseCover = true;

		//If target is unconscious and shooter is in the same room and there are no other people attacking shooter, shooter can ICly bypass cover
		//under the assumption that they walk around it due to no one stopping them, assuming the shooter is upright
		if (target.Location == Assailant.Location && (targetAsCharacter?.State.HasFlag(CharacterState.Unconscious) ??
		                                              false)
		                                          && Assailant.PositionState.CompareTo(PositionStanding.Instance) ==
		                                          PositionHeightComparison.Equivalent)
		{
			if (Assailant.Combat?.Combatants.All(x => x.CombatTarget != Assailant || x == target) != true)
			{
				bUseCover = false;
			}
		}

		if (bUseCover)
		{
			coverDifficulty = target.Cover?.Cover.MinimumRangedDifficulty ?? difficulty;
			if (target.Cover != null)
			{
				coverDifficulty = coverDifficulty.StageDown((int)Weapon.WeaponType.CoverBonus);
			}

			if (coverDifficulty < difficulty)
			{
				coverDifficulty = difficulty;
			}
		}

		var positionBonus = 0.0;
		//StageUp (or down) based on the size of the target.
		if (targetHb.Body != null)
		{
			positionBonus = GetSizeDifficulty(targetHb.Body.SizeStanding);
			if (target.Location != Assailant.Location)
				//When firing across distances, the target's posture can inflict penalties. 
			{
				positionBonus += GetTargetPositionDifficulty(targetHb.Body.PositionState);
			}
		}
		else if (target is IGameItem)
		{
			var targetItem = target as IGameItem;
			positionBonus += GetSizeDifficulty(targetItem.Size);
		}

		var noPressureBonus = 0.0;
		// Shots are easier when you're not under pressure
		if (Assailant.Combat == null)
		{
			noPressureBonus = Gameworld.GetStaticDouble("NoPressureBonus");
		}

		Tuple<CheckOutcome, CheckOutcome> results = null;
		if (Weapon.ReadyToFire == false)
		{
			//Don't do the check if trying to fire an empty weapon. This would be expoitable for free
			//skillups.
			var failOutcome = new CheckOutcome { Outcome = Outcome.MajorFail };
			results = new Tuple<CheckOutcome, CheckOutcome>(failOutcome, failOutcome);
		}
		else
		{
			if (Assailant == target)
			{
				results = check.MultiDifficultyCheck(Assailant, Difficulty.Automatic, coverDifficulty, target,
					Weapon.WeaponType.FireTrait);
			}
			else
			{
				results = check.MultiDifficultyCheck(Assailant, difficulty, coverDifficulty, target,
					Weapon.WeaponType.FireTrait,
					// Bonuses
					Weapon.WeaponType.AccuracyBonusExpression.Evaluate(Assailant, Weapon.WeaponType.FireTrait) +
					GetPenaltyForTargeting(targetHb.Body, range) +
					noPressureBonus +
					positionBonus +
					visionModifiers
				);
			}
		}

		DetermineTargetBodypart(targetHb, results.Item1);

		if (Assailant.Aim != null)
		{
			if (Assailant.TargettedBodypart != null)
			{
				Assailant.Aim.AimPercentage = 0;
			}
			else
			{
				Assailant.Aim.AimPercentage -= results.Item1 == Outcome.MajorPass
					? 0.66 * Weapon.WeaponType.AimBonusLostPerShot
					: Weapon.WeaponType.AimBonusLostPerShot;
			}
		}

		if (!SuppressAttackMessage)
		{
			var emote =
				string.Format(results.Item1.IsPass()
					? Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, Weapon.Parent, null,
						MoveType, results.Item1, null)
					: Gameworld.CombatMessageManager.GetFailMessageFor(Assailant, target, Weapon.Parent, null,
						MoveType, results.Item1, null), Weapon.FireVerbForEchoes);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(new Emote(emote, Assailant, Assailant, target, Weapon.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}

		if (results.Item1.IsFail())
		{
			Weapon.Fire(Assailant, target, results.Item1, results.Item2,
				new OpposedOutcome(Outcome.Fail, Outcome.Pass), null, null, target);
			return new CombatMoveResult
			{
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = results.Item1.Outcome,
				DefenderOutcome = Outcome.NotTested
			};
		}

		if (defenderMove == null || defenderMove is HelplessDefenseMove || defenderMove is TooExhaustedMove)
		{
			Weapon.Fire(Assailant, target, results.Item1, results.Item2,
				new OpposedOutcome(results.Item1, Outcome.NotTested), TargetBodypart, null, target);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = results.Item1.Outcome,
				DefenderOutcome = Outcome.NotTested
			};
		}

		var targetChar = (ICharacter)target;

		if (targetChar is not null)
		{
			CrimeExtensions.CheckPossibleCrimeAllAuthorities(Assailant, CrimeTypes.AssaultWithADeadlyWeapon, targetChar,
				null, "");
		}
		else
		{
			CrimeExtensions.CheckPossibleCrimeAllAuthorities(Assailant, CrimeTypes.Vandalism, null, (IGameItem)target,
				"");
		}

		if (defenderMove is DodgeRangeMove dodge)
		{
			var dodgeResult = Gameworld.GetCheck(dodge.Check).Check(targetChar, Weapon.BaseDodgeDifficulty, Assailant);
			var opposed = new OpposedOutcome(results.Item1, dodgeResult);
			if (opposed.Outcome == OpposedOutcomeDirection.Proponent ||
			    opposed.Outcome == OpposedOutcomeDirection.Stalemate)
			{
				Weapon.Fire(Assailant, target, results.Item1, results.Item2, opposed, TargetBodypart, new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetFailMessageFor(targetChar, Assailant, Weapon.Parent, null,
							BuiltInCombatMoveType.DodgeRange, dodgeResult, TargetBodypart), target,
						target, Assailant, Weapon.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap), target);
				return new CombatMoveResult
				{
					MoveWasSuccessful = true,
					RecoveryDifficulty = RecoveryDifficultyFailure,
					AttackerOutcome = results.Item1.Outcome,
					DefenderOutcome = dodgeResult.Outcome
				};
			}

			Weapon.Fire(Assailant, target, results.Item1, results.Item2, opposed, TargetBodypart, new EmoteOutput(
				new Emote(
					Gameworld.CombatMessageManager.GetMessageFor(targetChar, Assailant, Weapon.Parent, null,
						BuiltInCombatMoveType.DodgeRange, dodgeResult, TargetBodypart), target,
					target, Assailant, Weapon.Parent), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap), target);
			return new CombatMoveResult
			{
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = results.Item1.Outcome,
				DefenderOutcome = dodgeResult.Outcome
			};
		}

		if (defenderMove is BlockMove block)
		{
			var blockResult = Gameworld.GetCheck(block.Check).Check(targetChar, block.CheckDifficulty, Assailant);
			var opposed = new OpposedOutcome(results.Item1, blockResult);
			if (opposed.Outcome == OpposedOutcomeDirection.Proponent)
			{
				Weapon.Fire(Assailant, target, results.Item1, results.Item2, opposed, TargetBodypart, new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetFailMessageFor(targetChar, Assailant, Weapon.Parent,
							null, BuiltInCombatMoveType.BlockRange, blockResult, TargetBodypart),
						target, target, Assailant, Weapon.Parent, block.Shield.Parent),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap), target);
				return new CombatMoveResult
				{
					MoveWasSuccessful = true,
					RecoveryDifficulty = RecoveryDifficultyFailure,
					AttackerOutcome = results.Item1.Outcome,
					DefenderOutcome = blockResult.Outcome
				};
			}


			Weapon.Fire(Assailant, block.Shield.Parent, results.Item1, results.Item2, opposed, TargetBodypart,
				new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetMessageFor(targetChar, Assailant, Weapon.Parent, null,
							BuiltInCombatMoveType.BlockRange,
							blockResult, TargetBodypart), target,
						target, Assailant, Weapon.Parent, block.Shield.Parent),
					style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap), target);
			return new CombatMoveResult
			{
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = results.Item1.Outcome,
				DefenderOutcome = blockResult.Outcome
			};
		}

		throw new NotImplementedException("Unknown defenderMove in RangedWeaponAttackBase: " +
		                                  defenderMove.Description);
	}

	//This returns baseline modifiers to making the shot based on how hard it is to spot in the
	//terrain, based on lighting and vision
	//Return a positive # since it will be used in a StageUp call
	public double GetSpottingModifier(Difficulty spottingDifficulty)
	{
		switch (spottingDifficulty)
		{
			case Difficulty.Hard:
				return StandardCheck.BonusesPerDifficultyLevel * -1;
			case Difficulty.VeryHard:
				return StandardCheck.BonusesPerDifficultyLevel * -2;
			case Difficulty.ExtremelyHard:
				return StandardCheck.BonusesPerDifficultyLevel * -3;
			case Difficulty.Insane:
				return StandardCheck.BonusesPerDifficultyLevel * -4;
			default:
				return 0;
		}
	}

	public double GetSizeDifficulty(SizeCategory size)
	{
		switch (size)
		{
			case SizeCategory.Nanoscopic:
				return StandardCheck.BonusesPerDifficultyLevel * -6;
			case SizeCategory.Microscopic:
				return StandardCheck.BonusesPerDifficultyLevel * -5;
			case SizeCategory.Miniscule:
				return StandardCheck.BonusesPerDifficultyLevel * -4;
			case SizeCategory.Tiny:
				return StandardCheck.BonusesPerDifficultyLevel * -2.66;
			case SizeCategory.VerySmall:
				return StandardCheck.BonusesPerDifficultyLevel * -1.33;
			case SizeCategory.Small:
				return StandardCheck.BonusesPerDifficultyLevel * -0.5;
			case SizeCategory.Normal:
				return 0;
			case SizeCategory.Large:
				return StandardCheck.BonusesPerDifficultyLevel * 1;
			case SizeCategory.VeryLarge:
				return StandardCheck.BonusesPerDifficultyLevel * 2;
			case SizeCategory.Huge:
				return StandardCheck.BonusesPerDifficultyLevel * 3;
			case SizeCategory.Enormous:
				return StandardCheck.BonusesPerDifficultyLevel * 4;
			case SizeCategory.Gigantic:
				return StandardCheck.BonusesPerDifficultyLevel * 5;
			case SizeCategory.Titanic:
				return StandardCheck.BonusesPerDifficultyLevel * 6;
			default:
				return 0;
		}
	}

	public static double GetTargetPositionDifficulty(IPositionState position)
	{
		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Higher)
		{
			return 0;
		}

		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Equivalent)
		{
			return StandardCheck.BonusesPerDifficultyLevel * -1;
		}

		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Lower)
		{
			return StandardCheck.BonusesPerDifficultyLevel * -2;
		}

		return 0;
	}
}