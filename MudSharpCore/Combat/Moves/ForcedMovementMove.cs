using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace MudSharp.Combat.Moves;

public class ForcedMovementMove : CombatMoveBase
{
	public ForcedMovementMove(
		ICharacter assailant,
		ICharacter target,
		IForcedMovementAttack attack,
		ForcedMovementVerbs verb,
		ICellExit exit)
	{
		Assailant = assailant;
		CharacterTarget = target;
		Attack = attack;
		Verb = verb;
		Exit = exit;
		MovementType = ForcedMovementTypes.Exit;
		_characterTargets.Add(target);
	}

	public ForcedMovementMove(
		ICharacter assailant,
		ICharacter target,
		IForcedMovementAttack attack,
		ForcedMovementVerbs verb,
		RoomLayer layer)
	{
		Assailant = assailant;
		CharacterTarget = target;
		Attack = attack;
		Verb = verb;
		Layer = layer;
		MovementType = ForcedMovementTypes.Layer;
		_characterTargets.Add(target);
	}

	public ICharacter CharacterTarget { get; }
	public IForcedMovementAttack Attack { get; }
	public ForcedMovementVerbs Verb { get; }
	public ForcedMovementTypes MovementType { get; }
	public ICellExit Exit { get; }
	public RoomLayer? Layer { get; }
	public IMeleeWeapon Weapon { get; init; }
	public INaturalAttack NaturalAttack { get; init; }

	public BuiltInCombatMoveType MoveType => Attack.MoveType;
	public override string Description => $"{VerbPresentParticiple()} an opponent {(MovementType == ForcedMovementTypes.Exit ? "through an exit" : "to another layer")}";
	public override double BaseDelay => Attack.BaseDelay;
	public override ExertionLevel AssociatedExertion => Attack.ExertionLevel;
	public override Difficulty RecoveryDifficultyFailure => Attack.RecoveryDifficultyFailure;
	public override Difficulty RecoveryDifficultySuccess => Attack.RecoveryDifficultySuccess;
	public override Difficulty CheckDifficulty => Weapon is not null
		? Assailant.GetDifficultyForTool(Weapon.Parent, Attack.Profile.BaseAttackerDifficulty)
		: Attack.Profile.BaseAttackerDifficulty;
	public override CheckType Check => NaturalAttack is null ? CheckType.MeleeWeaponCheck : CheckType.NaturalWeaponAttack;
	public override double StaminaCost => NaturalAttack is null
		? MeleeWeaponAttack.MoveStaminaCost(Assailant, Attack)
		: NaturalAttackMove.MoveStaminaCost(Assailant, NaturalAttack.Attack);

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (defenderMove is null)
		{
			defenderMove = new HelplessDefenseMove { Assailant = CharacterTarget, PrimaryTarget = Assailant };
		}

		if (!Attack.ForcedMovementVerbs.HasFlag(Verb) || !Attack.ForcedMovementTypes.HasFlag(MovementType))
		{
			return FailWithMessage("That attack is not configured for this kind of forced movement.");
		}

		if (!CombatForcedMovementUtilities.IsInRange(Assailant, CharacterTarget, Attack.RequiredRange))
		{
			return FailWithMessage($"You are not at the required {Attack.RequiredRange.DescribeEnum().ColourValue()} range.");
		}

		WorsenCombatPosition(defenderMove.Assailant, Assailant);
		var attackRoll = Gameworld.GetCheck(CheckType.ForcedMovementCheck)
		                          .CheckAgainstAllDifficulties(Assailant, CheckDifficulty,
			                          Weapon?.WeaponType.AttackTrait, defenderMove.Assailant,
			                          Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;

		var attackEmote = GetAttackEmote(attackRoll[CheckDifficulty]);
		if (defenderMove is not HelplessDefenseMove and not TooExhaustedMove)
		{
			var defenseRoll = Gameworld.GetCheck(CheckType.OpposeForcedMovementCheck)
			                           .CheckAgainstAllDifficulties(CharacterTarget, Attack.SecondaryDifficulty, null,
				                           Assailant,
				                           CharacterTarget.DefensiveAdvantage -
				                           GetPositionPenalty(Assailant.GetFacingFor(CharacterTarget)));
			CharacterTarget.DefensiveAdvantage = 0;
			var opposed = new OpposedOutcome(attackRoll, defenseRoll, CheckDifficulty, Attack.SecondaryDifficulty);
			if (opposed.Outcome != OpposedOutcomeDirection.Proponent)
			{
				Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"{attackEmote}, but $1 resist|resists being moved.",
						Assailant, Assailant, CharacterTarget),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
				return new CombatMoveResult
				{
					RecoveryDifficulty = RecoveryDifficultyFailure,
					AttackerOutcome = attackRoll[CheckDifficulty],
					DefenderOutcome = defenseRoll[Attack.SecondaryDifficulty]
				};
			}
		}

		if (!TryApplyMovement(out var why))
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"{attackEmote}, but {why}",
					Assailant, Assailant, CharacterTarget),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return new CombatMoveResult
			{
				RecoveryDifficulty = RecoveryDifficultyFailure,
				AttackerOutcome = attackRoll[CheckDifficulty]
			};
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"{attackEmote}, and $1 $1|are|is forced away.",
				Assailant, Assailant, CharacterTarget),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		return new CombatMoveResult
		{
			RecoveryDifficulty = RecoveryDifficultySuccess,
			MoveWasSuccessful = true,
			AttackerOutcome = attackRoll[CheckDifficulty]
		};
	}

	private string GetAttackEmote(CheckOutcome outcome)
	{
		return NaturalAttack is not null
			? string.Format(
				Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, null, Attack, MoveType, outcome, NaturalAttack.Bodypart),
				NaturalAttack.Bodypart.FullDescription(), "")
			: string.Format(
				Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, Weapon?.Parent, Attack, MoveType, outcome, null),
				"", "");
	}

	private CombatMoveResult FailWithMessage(string message)
	{
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(message, Assailant, Assailant, CharacterTarget),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		return new CombatMoveResult { RecoveryDifficulty = RecoveryDifficultyFailure };
	}

	private bool TryApplyMovement(out string why)
	{
		return MovementType == ForcedMovementTypes.Exit
			? CombatForcedMovementUtilities.TryForceExitMovement(Assailant, CharacterTarget, Exit, Verb, out why)
			: CombatForcedMovementUtilities.TryForceLayerMovement(Assailant, CharacterTarget, Layer!.Value, Verb, out why);
	}

	private string VerbPresentParticiple()
	{
		return Verb == ForcedMovementVerbs.Pull ? "Pulling" : "Shoving";
	}
}
