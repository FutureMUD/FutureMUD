using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using MudSharp.Character.Heritage;

namespace MudSharp.Combat.Moves;

public class ExtendGrappleMove : NaturalAttackMove
{
	public LimbType TargetLimbType => ((ITargetLimbWeaponAttack)Attack).TargetLimbType;
	public ICharacter CharacterTarget { get; set; }
	public override CheckType Check => CheckType.ExtendGrappleCheck;

	public ExtendGrappleMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
	{
		CharacterTarget = target;
	}

	public override string Description => "Extending a grapple to additional limbs.";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove == null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = CharacterTarget };
		}

		var potentialLimbs = CharacterTarget.Body.Limbs
		                                    .Where(x =>
			                                    x.LimbType == TargetLimbType &&
			                                    CharacterTarget.Body.BodypartsForLimb(x).Any() &&
			                                    !CharacterTarget.Body.EffectsOfType<ILimbIneffectiveEffect>().Any(y =>
				                                    y.Reason == LimbIneffectiveReason.Grappling && y.AppliesToLimb(x))
		                                    ).ToList();
		var targetLimb = potentialLimbs.GetRandomElement();
		if (targetLimb == null)
		{
			throw new ApplicationException("ExtendGrappleMove couldn't find a limb to target.");
		}

		WorsenCombatPosition(CharacterTarget, Assailant);
		var attackerSize = Assailant.CurrentContextualSize(SizeContext.GrappleAttack);
		var defenderSize = Assailant.CurrentContextualSize(SizeContext.GrappleDefense);

		var offset = (attackerSize - defenderSize) *
		             Gameworld.GetStaticDouble("InitiateGrappleOffsetPerSizeDifference");
		var baseDifficulty = CheckDifficulty.ApplyBonus(offset);

		var attackRoll = Gameworld.GetCheck(Check)
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty.ApplyBonus(offset), null,
			                          CharacterTarget,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		var attackEmote =
			string.Format(
				      Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, null, Attack,
					      BuiltInCombatMoveType.ExtendGrapple, attackRoll[baseDifficulty], null),
				      Bodypart.FullDescription(), targetLimb.Name.ToLowerInvariant())
			      .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());

		if (defenderMove is HelplessDefenseMove)
		{
			return ResolveMoveHelpless(defenderMove, attackRoll, attackEmote, targetLimb, baseDifficulty);
		}

		if (defenderMove is DodgeMove)
		{
			return ResolveMoveDodge(defenderMove, attackRoll, attackEmote, targetLimb, offset, baseDifficulty);
		}

		if (defenderMove is CounterGrappleMove)
		{
			return ResolveMoveCounter(defenderMove, attackRoll, attackEmote, targetLimb, offset, baseDifficulty);
		}

		throw new NotImplementedException();
	}

	private CombatMoveResult ResolveMoveCounter(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll, string attackEmote, ILimb targetLimb, double offset,
		Difficulty attackerDifficulty)
	{
		var defenseDifficulty = Attack.Profile.BaseParryDifficulty.ApplyBonus(-1 * offset);
		var counterCheck = Gameworld.GetCheck(defenderMove.Check)
		                            .CheckAgainstAllDifficulties(CharacterTarget, defenseDifficulty, null, Assailant,
			                            CharacterTarget.DefensiveAdvantage -
			                            GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
		CharacterTarget.DefensiveAdvantage = 0;
		var result = new OpposedOutcome(attackRoll, counterCheck, attackerDifficulty, defenseDifficulty);

		if (result.Outcome == OpposedOutcomeDirection.Proponent || result.Outcome == OpposedOutcomeDirection.Stalemate)
		{
			var counterEmote = string.Format(Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget,
					Assailant,
					null, Attack,
					BuiltInCombatMoveType.CounterGrapple, counterCheck[defenseDifficulty], Bodypart),
				Bodypart.FullDescription(), targetLimb.Name.ToLowerInvariant());
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			var grapple = Assailant.EffectsOfType<IGrappling>().First();
			grapple.AddLimb(targetLimb);
			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[attackerDifficulty],
				DefenderOutcome = counterCheck[defenseDifficulty],
				MoveWasSuccessful = true,
				RecoveryDifficulty = RecoveryDifficultySuccess
			};
		}
		else
		{
			var grapple = Assailant.EffectsOfType<IGrappling>().First();
			Assailant.RemoveEffect(grapple, true);
			var effect = CharacterTarget.CombinedEffectsOfType<Grappling>().FirstOrDefault(x => x.Target == Assailant);
			if (effect == null)
			{
				effect = new Grappling(CharacterTarget, Assailant);
				CharacterTarget.AddEffect(effect);
			}

			var freeLimbs = 0;
			switch (result.Degree)
			{
				case OpposedOutcomeDegree.Moderate:
					freeLimbs = 1;
					break;
				case OpposedOutcomeDegree.Major:
					freeLimbs = 2;
					break;
				case OpposedOutcomeDegree.Total:
					freeLimbs = 3;
					break;
			}

			var potentialLimbs = Assailant.Body.Limbs.Where(x =>
				                              !Assailant.Body.EffectsOfType<ILimbIneffectiveEffect>().Any(y =>
					                              y.Reason == LimbIneffectiveReason.Grappling && y.AppliesToLimb(x)) &&
				                              x.LimbType == TargetLimbType)
			                              .ToList();
			for (var i = 0; i < freeLimbs; i++)
			{
				if (!potentialLimbs.Any())
				{
					break;
				}

				var limb = potentialLimbs.GetRandomElement();
				effect.AddLimb(limb);
				potentialLimbs.Remove(limb);
			}

			var counterEmote = string.Format(Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant,
					null, Attack,
					BuiltInCombatMoveType.CounterGrapple, counterCheck[defenseDifficulty], Bodypart),
				effect.LimbsUnderControl.Select(x => x.Name.ToLowerInvariant()).ListToString());
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"{attackEmote}{counterEmote}", Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[attackerDifficulty],
				DefenderOutcome = counterCheck[defenseDifficulty],
				MoveWasSuccessful = false,
				RecoveryDifficulty = RecoveryDifficultyFailure
			};
		}
	}

	private CombatMoveResult ResolveMoveDodge(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll, string attackEmote, ILimb targetLimb, double offset,
		Difficulty attackerDifficulty)
	{
		var defenseDifficulty = Attack.Profile.BaseDodgeDifficulty.ApplyBonus(-1 * offset);
		var check = Gameworld.GetCheck(CheckType.DodgeCheck);
		var result = check.CheckAgainstAllDifficulties(CharacterTarget, defenseDifficulty, null, Assailant,
			CharacterTarget.DefensiveAdvantage);
		CharacterTarget.DefensiveAdvantage = 0;
		var opposed = new OpposedOutcome(attackRoll, result, attackerDifficulty, defenseDifficulty);

		if (opposed.Outcome == OpposedOutcomeDirection.Proponent ||
		    opposed.Outcome == OpposedOutcomeDirection.Stalemate)
		{
			var failEmote = string.Format(
				Gameworld.CombatMessageManager.GetFailMessageFor(CharacterTarget, Assailant, null, null,
					BuiltInCombatMoveType.DodgeExtendGrapple, result[defenseDifficulty], null),
				Bodypart.FullDescription(), targetLimb.Name.ToLowerInvariant());
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"{attackEmote}{failEmote}".Fullstop(), Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			var grapple = Assailant.EffectsOfType<IGrappling>().First();
			grapple.AddLimb(targetLimb);
			return new CombatMoveResult
			{
				AttackerOutcome = attackRoll[attackerDifficulty],
				DefenderOutcome = result[defenseDifficulty],
				MoveWasSuccessful = true,
				RecoveryDifficulty = RecoveryDifficultySuccess
			};
		}

		var defenseEmote = string.Format(
			Gameworld.CombatMessageManager.GetMessageFor(CharacterTarget, Assailant, null, null,
				BuiltInCombatMoveType.DodgeExtendGrapple, result[defenseDifficulty], null), Bodypart.FullDescription(),
			targetLimb.Name.ToLowerInvariant());
		Assailant.OutputHandler.Handle(new EmoteOutput(
			new Emote($"{attackEmote}{defenseEmote}".Fullstop(), Assailant, Assailant, CharacterTarget),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		return new CombatMoveResult
		{
			AttackerOutcome = attackRoll[attackerDifficulty],
			DefenderOutcome = result[defenseDifficulty],
			MoveWasSuccessful = false,
			RecoveryDifficulty = RecoveryDifficultyFailure
		};
	}

	private CombatMoveResult ResolveMoveHelpless(ICombatMove defenderMove,
		IReadOnlyDictionary<Difficulty, CheckOutcome> attackRoll, string attackEmote, ILimb targetLimb,
		Difficulty primaryDifficulty)
	{
		WorsenCombatPosition(CharacterTarget, Assailant);
		var result = new OpposedOutcome(attackRoll[primaryDifficulty], Outcome.NotTested);
		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote($"{attackEmote}, and #1 %1|are|is successful!",
					Assailant, Assailant, CharacterTarget, null), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		var grapple = Assailant.EffectsOfType<IGrappling>().First();
		grapple.AddLimb(targetLimb);
		return new CombatMoveResult
		{
			AttackerOutcome = attackRoll[primaryDifficulty],
			DefenderOutcome = Outcome.NotTested,
			MoveWasSuccessful = true,
			RecoveryDifficulty = Difficulty.Automatic
		};
	}
}