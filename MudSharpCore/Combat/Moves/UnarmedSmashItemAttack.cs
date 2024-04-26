using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

namespace MudSharp.Combat.Moves;

public class UnarmedSmashItemAttack : WeaponAttackMove
{
	public UnarmedSmashItemAttack(IWeaponAttack attack) : base(attack)
	{
	}

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.UnarmedSmashItem;

	public INaturalAttack NaturalAttack { get; init; }
	public IGameItem Target { get; init; }
	public IGameItem ParentItem { get; init; }
	public IBodypart Bodypart => NaturalAttack.Bodypart;

	public override string Description =>
		$"Attacking {Target.HowSeen(Assailant)}{ParentItem?.HowSeen(Assailant).LeadingSpaceIfNotEmpty().Parentheses() ?? ""} with their {Bodypart.FullDescription()} to smash it.";

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
	public override int Reach => 0;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		CrimeExtensions.CheckPossibleCrimeAllAuthorities(Assailant, CrimeTypes.Vandalism, null, Target, "");
		var check = Gameworld.GetCheck(CheckType.NaturalWeaponAttack).Check(Assailant, Difficulty.Easy, Target, null);
		var result = new OpposedOutcome(check, Outcome.NotTested);
		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)NaturalAttack.Quality;

		var damages = GetDamagePlusSelfDamage(null, Bodypart, null, Target.Material, check.Outcome,
			NaturalAttack.Attack.Profile.DamageType, NaturalAttack.Attack.Profile.BaseAngleOfIncidence, NaturalAttack,
			new OpposedOutcome(check.Outcome, Outcome.NotTested).Degree);

		var finalDamage = damages.Item1;
		var wounds = Target.PassiveSufferDamage(finalDamage);
		var selfDamage = damages.Item2;
		var selfwounds = Assailant.PassiveSufferDamage(selfDamage);
		var emote = Gameworld.CombatMessageManager.GetMessageFor(Assailant, Target,
			null, Attack, BuiltInCombatMoveType.UnarmedSmashItem, check.Outcome, Bodypart);
		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					string.Format(emote.Fullstop(), Bodypart.FullDescription(),
						wounds.Select(x => x.Describe(WoundExaminationType.Glance, Outcome.MajorPass)).ListToString()
						      .IfNullOrWhiteSpace("no real damage")), Assailant, Assailant, Target, ParentItem),
				style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		var exit = Target.GetItemType<IDoor>()?.InstalledExit;
		if (exit != null && exit.Door?.Parent == Target && exit.Cells.Contains(Assailant.Location))
		{
			exit.Opposite(Assailant.Location).Handle(new EmoteOutput(
				new Emote("There is a loud thud on @, as if someone is bashing on it from the other side.", Target),
				flags: OutputFlags.PurelyAudible));
		}

		wounds.ProcessPassiveWounds();
		selfwounds.ProcessPassiveWounds();
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = check.Outcome.IsPass() ? Difficulty.Normal : Difficulty.Hard,
			AttackerOutcome = check.Outcome
		};
	}
}