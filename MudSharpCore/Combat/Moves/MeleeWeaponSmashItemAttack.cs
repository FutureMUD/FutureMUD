using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Moves;

public class MeleeWeaponSmashItemAttack : WeaponAttackMove
{
    public MeleeWeaponSmashItemAttack(IWeaponAttack attack) : base(attack)
    {
    }

    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.MeleeWeaponSmashItem;
    public IGameItem Target { get; init; }
    public IGameItem ParentItem { get; init; }

    public override string Description =>
        $"Attacking {Target.HowSeen(Assailant)}{ParentItem?.HowSeen(Assailant).LeadingSpaceIfNotEmpty().Parentheses() ?? ""} with {Weapon.Parent.HowSeen(Assailant)} to smash it.";

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
        CheckOutcome check = Gameworld.GetCheck(CheckType.MeleeWeaponCheck)
                             .Check(Assailant, Difficulty.Easy, Weapon.WeaponType.AttackTrait, Target);
        OpposedOutcome result = new(check, Outcome.NotTested);
        Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)result.Degree;
        Attack.Profile.DamageExpression.Formula.Parameters["quality"] = (int)Weapon.Parent.Quality;

        double damage = Attack.Profile.DamageExpression.Evaluate(Assailant) * 2 * Attack.Profile.BaseAngleOfIncidence /
                     Math.PI;

        Damage finalDamage = new()
        {
            ActorOrigin = Assailant,
            LodgableItem = null,
            ToolOrigin = Weapon.Parent,
            AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
            Bodypart = TargetBodypart,
            DamageAmount = damage,
            DamageType = Attack.Profile.DamageType,
            PainAmount = 0,
            PenetrationOutcome = Outcome.NotTested,
            ShockAmount = 0,
            StunAmount = 0
        };

        IEnumerable<IWound> wounds = Target.PassiveSufferDamage(finalDamage);

        double selfDamageMultiplier = 1.0;
        switch (check.Outcome)
        {
            case Outcome.Fail:
                selfDamageMultiplier = 0.4;
                break;
            case Outcome.MinorFail:
                selfDamageMultiplier = 0.2;
                break;
            case Outcome.MinorPass:
                selfDamageMultiplier = 0.08;
                break;
            case Outcome.Pass:
                selfDamageMultiplier = 0.05;
                break;
            case Outcome.MajorPass:
                selfDamageMultiplier = 0.02;
                break;
        }

        Damage selfDamage = new()
        {
            ActorOrigin = Assailant,
            LodgableItem = null,
            ToolOrigin = Weapon.Parent,
            AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
            Bodypart = TargetBodypart,
            DamageAmount = damage * selfDamageMultiplier,
            DamageType = Attack.Profile.DamageType,
            PainAmount = 0,
            PenetrationOutcome = Outcome.NotTested,
            ShockAmount = 0,
            StunAmount = 0
        };

        IEnumerable<IWound> selfwounds = Weapon.Parent.PassiveSufferDamage(selfDamage);
        string emote = Gameworld.CombatMessageManager.GetMessageFor(Assailant, Target,
            Weapon.Parent, Attack, BuiltInCombatMoveType.MeleeWeaponSmashItem, check.Outcome, null);
        Assailant.OutputHandler.Handle(
            new EmoteOutput(
                new Emote(
                    string.Format(emote.Fullstop(),
                        wounds.Select(x => x.Describe(WoundExaminationType.Glance, Outcome.MajorPass)).ListToString()
                              .IfNullOrWhiteSpace("no real damage")), Assailant, Assailant, Target,
                    Weapon.Parent, ParentItem), style: OutputStyle.CombatMessage,
                flags: OutputFlags.InnerWrap));
        IExit exit = Target.GetItemType<IDoor>()?.InstalledExit;
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
            RecoveryDifficulty = check.Outcome.IsPass() ? Difficulty.Normal : Difficulty.Hard,
            MoveWasSuccessful = true,
            AttackerOutcome = check.Outcome,
            WoundsCaused = wounds,
            SelfWoundsCaused = selfwounds
        };
    }
}