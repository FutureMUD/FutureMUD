using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Moves;

public class CoupDeGrace : WeaponAttackMove
{
    public CoupDeGrace(IWeaponAttack attack, ICharacter target) : base(attack)
    {
        Target = target;
        TargetBodypart =
            target.Body.Bodyparts.Where(x => attack.GetAttackType<IFixedBodypartWeaponAttack>().Bodypart == x.Shape)
                  .GetRandomElement();
    }

    public ICharacter Target { get; set; }
    public PlayerEmote Emote { get; init; }

    #region Overrides of CombatMoveBase

    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.CoupDeGrace;
    public override string Description { get; } = "Performing a Coup-De-Grace.";

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        CheckOutcome attackRoll = Gameworld.GetCheck(Check)
                                  .Check(Assailant, CheckDifficulty, Weapon.WeaponType.AttackTrait, Target,
                                      Assailant.OffensiveAdvantage);
        string attackEmote =
            string.Format(
                Gameworld.CombatMessageManager.GetMessageFor(Assailant, Target, Weapon.Parent, Attack,
                    BuiltInCombatMoveType.CoupDeGrace, attackRoll.Outcome, null), TargetBodypart.FullDescription());
        OpposedOutcome result = new(attackRoll, Outcome.NotTested);
        double finalAngle = Attack.Profile.BaseAngleOfIncidence;
        Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
        Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
        Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)result.Degree;
        Attack.Profile.StunExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;
        Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)result.Degree;
        Attack.Profile.PainExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;

        Damage finalDamage = new()
        {
            ActorOrigin = Assailant,
            LodgableItem = null,
            ToolOrigin = Weapon.Parent,
            AngleOfIncidentRadians = finalAngle,
            Bodypart = TargetBodypart,
            DamageAmount = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI,
            DamageType = Attack.Profile.DamageType,
            PainAmount = Attack.Profile.PainExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI,
            PenetrationOutcome = Outcome.MajorPass,
            ShockAmount = 0,
            StunAmount = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * finalAngle / Math.PI
        };

        Assailant.OutputHandler.Handle(
            new MixedEmoteOutput(
                new Emote(string.Format(attackEmote, TargetBodypart.FullDescription()), Assailant, Assailant, Target,
                    Weapon.Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap).Append(Emote));
        List<IWound> wounds = Target.PassiveSufferDamage(finalDamage).ToList();
        wounds.ProcessPassiveWounds();
        Assailant.Body?.SetExertionToMinimumLevel(AssociatedExertion);
        return new CombatMoveResult
        {
            MoveWasSuccessful = true,
            RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
            AttackerOutcome = attackRoll,
            WoundsCaused = wounds
        };
    }

    #endregion

    #region Overrides of WeaponAttackMove

    public override double StaminaCost => Attack.StaminaCost;
    public override double BaseDelay => Attack.BaseDelay;
    public override ExertionLevel AssociatedExertion => Attack.ExertionLevel;
    public override Difficulty RecoveryDifficultyFailure => Attack.RecoveryDifficultyFailure;
    public override Difficulty RecoveryDifficultySuccess => Attack.RecoveryDifficultySuccess;

    public override Difficulty CheckDifficulty =>
        Assailant.GetDifficultyForTool(Weapon.Parent, Attack.Profile.BaseAttackerDifficulty);

    public override CheckType Check => CheckType.MeleeWeaponCheck;
    public override int Reach => Weapon.WeaponType.Reach;

    #endregion
}