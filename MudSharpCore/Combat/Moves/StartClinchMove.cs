using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class StartClinchMove : WeaponAttackMove
{
	public StartClinchMove(ICharacter target) : base(null)
	{
		CharacterTarget = target;
	}

	public static bool CanClinchWhileMounted(ICharacter assailant, ICharacter target)
	{
		if (assailant.RidingMount is null)
		{
			return true;
		}

		if (target.RidingMount is not null)
		{
			return true;
		}

		return target.SizeStanding > assailant.SizeStanding;
	}

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.StartClinch;

	public override string Description => "Attempting to begin a clinch";

	public override CheckType Check => CheckType.StartClinch;

	public override Difficulty RecoveryDifficultySuccess => Difficulty.Easy;

	public override Difficulty RecoveryDifficultyFailure => Difficulty.Normal;

	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	public override double BaseDelay => 0.5; // TODO - programmatically loaded

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double BaseStaminaCost(IFuturemud gameworld)
	{
		return gameworld.GetStaticDouble("StartClinchMoveStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public ICharacter CharacterTarget { get; set; }

	#region Overrides of WeaponAttackMove

	public override int Reach => 0;

	#endregion

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
	if (!CanClinchWhileMounted(Assailant, CharacterTarget))
	{
		Assailant.OutputHandler.Send(
				$"You cannot reach {CharacterTarget.HowSeen(Assailant)} to clinch while you are mounted.");
		return new CombatMoveResult
		{
			RecoveryDifficulty = RecoveryDifficultyFailure
		};
	}

	defenderMove = defenderMove ?? new HelplessDefenseMove() { Assailant = CharacterTarget };
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

		var attackEmote = Gameworld.CombatMessageManager.GetMessageFor(Assailant, defenderMove.Assailant, null, null,
			BuiltInCombatMoveType.StartClinch, attackRoll[CheckDifficulty], null);

		var ward = defenderMove as WardDefenseMove;
		WardResult wardResult = null;
		if (ward != null)
		{
			wardResult = ResolveWard(ward);
			if (wardResult.WardSucceeded)
			{
				Assailant.OutputHandler.Handle(
					new EmoteOutput(
						new Emote($"{attackEmote}{wardResult.WardEmotes}".Fullstop(), Assailant, Assailant,
							defenderHaveWounds, null, null, wardResult.WardWeapon?.Parent),
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

		if (defenderMove is HelplessDefenseMove || ward != null)
		{
			return ResolveHelplessDefense(attackEmote, wardResult);
		}

		if (defenderMove is DodgeMove dodge)
		{
			return ResolveDodgeMove(dodge, attackRoll, attackEmote, wardResult);
		}

		throw new NotImplementedException(
			$"Unknown defenderMove type in StartClinchMove.ResolveMove - {defenderMove.GetType().Name}");
	}

	private CombatMoveResult ResolveDodgeMove(DodgeMove dodge, IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll,
		string attackEmote,
		WardResult wardResult)
	{
		var dodgeDifficulty = Difficulty.Normal;
		if (CharacterTarget.CombatStrategyMode == CombatStrategyMode.Ward)
		{
			dodgeDifficulty = CharacterTarget.CombatTarget == Assailant ? Difficulty.VeryEasy : Difficulty.Easy;
		}
		else if (CharacterTarget.CombatTarget != Assailant)
		{
			dodgeDifficulty = Difficulty.VeryHard;
		}

		var dodgeCheck = Gameworld.GetCheck(CheckType.ResistClinch)
		                          .CheckAgainstAllDifficulties(CharacterTarget, dodgeDifficulty, null, Assailant,
			                          dodge.Assailant.DefensiveAdvantage -
			                          GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
		dodge.Assailant.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, dodgeCheck, CheckDifficulty, dodgeDifficulty);
#if DEBUG
		Console.WriteLine($"StartClinchMove Dodge Outcome: {result.Degree.Describe()} to {result.Outcome.Describe()}");
#endif

		if (result.Outcome == OpposedOutcomeDirection.Proponent)
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant, null, null,
				BuiltInCombatMoveType.ResistClinch, dodgeCheck[dodgeDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{dodgeEmote}"
							.Fullstop(), Assailant, Assailant, CharacterTarget, null, null,
						wardResult?.WardWeapon?.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			ProcessWardFreeAttack(Assailant, CharacterTarget, wardResult);
			Assailant.AddEffect(new ClinchEffect(Assailant, CharacterTarget));
			CharacterTarget.AddEffect(new ClinchEffect(CharacterTarget, Assailant));
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				AttackerOutcome = attackRoll[CheckDifficulty],
				DefenderOutcome = dodgeCheck[dodgeDifficulty],
				RecoveryDifficulty = RecoveryDifficultySuccess
			};
		}
		else
		{
			var dodgeEmote = Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant, null, null,
				BuiltInCombatMoveType.ResistClinch, dodgeCheck[dodgeDifficulty], null);
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						$"{attackEmote}{wardResult?.WardEmotes ?? ""}{string.Format(dodgeEmote, "", TargetBodypart?.FullDescription() ?? "")}"
							.Fullstop(), Assailant, Assailant, CharacterTarget, null, null,
						wardResult?.WardWeapon?.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			ProcessWardFreeAttack(Assailant, CharacterTarget, wardResult);
			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[CheckDifficulty],
				DefenderOutcome = dodgeCheck[dodgeDifficulty],
				RecoveryDifficulty = RecoveryDifficultyFailure
			};
		}
	}

	private CombatMoveResult ResolveHelplessDefense(string attackEmote, WardResult wardResult)
	{
		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}{wardResult?.WardEmotes ?? ""}, and $1 $1|are|is unable to stop &0.",
					Assailant, Assailant,
					CharacterTarget, null, null, wardResult?.WardWeapon?.Parent), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		ProcessWardFreeAttack(Assailant, CharacterTarget, wardResult);
		Assailant.AddEffect(new ClinchEffect(Assailant, CharacterTarget));
		CharacterTarget.AddEffect(new ClinchEffect(CharacterTarget, Assailant));
		return new CombatMoveResult
		{
			MoveWasSuccessful = true
		};
	}
}