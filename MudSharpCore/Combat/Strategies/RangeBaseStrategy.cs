using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Strategies;

public abstract class RangeBaseStrategy : StrategyBase
{
    #region Implementation of ICombatStrategy

    protected virtual ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
        IPerceiver assailant)
    {
        if (defender.CombatTarget == assailant || !defender.MeleeRange)
        {
            List<IMeleeWeapon> receivingWeapons =
                defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
                        .Where(
                            x =>
                                x.WeaponType.Attacks.Any(
                                    y =>
                                        y.MoveType == BuiltInCombatMoveType.ReceiveCharge &&
                                        (y.UsabilityProg?.ExecuteBool(defender, move.Assailant, x.Parent) ?? true)))
                        .ToList();
            if (receivingWeapons.Any())
            {
                IMeleeWeapon weapon = receivingWeapons.WhereMax(x => x.WeaponType.Reach).First();
                return new ReceiveChargeMove(defender, weapon,
                    weapon.WeaponType.Attacks.Where(
                              x =>
                                  x.MoveType == BuiltInCombatMoveType.ReceiveCharge &&
                                  (x.UsabilityProg?.ExecuteBool(defender, move.Assailant, weapon.Parent) ?? true))
                          .GetWeightedRandom(x => x.Weighting), assailant as ICharacter);
            }
        }

        return null;
    }

    protected virtual ICombatMove ResponseToMoveToMelee(MoveToMeleeMove move, ICharacter defender,
        IPerceiver assailant)
    {
        return null;
    }

    protected virtual ICombatMove ResponseToFireAndAdvance(FireAndAdvanceToMeleeMove move, ICharacter defender,
        IPerceiver assailant)
    {
        return null;
    }

    protected virtual ICombatMove ResponseToRangedWeaponAttack(IRangedWeaponAttackMove move, ICharacter defender)
    {
        if (defender.Cover != null)
        {
            return new HelplessDefenseMove { Assailant = defender };
        }

        ICharacter assailant = move.Assailant;

        IShield shield =
            defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IShield>())
                    .FirstMax(x => x.ShieldType.BlockBonus);
        double shieldBonus = shield != null
            ? BlockMove.GetBlockBonus(move, defender.Body.AlignmentOf(shield.Parent), shield)
            : 0;
        Difficulty finalBlockDifficulty = move.Weapon.BaseBlockDifficulty.ApplyBonus(shieldBonus);
        Difficulty finalDodgeDifficulty = move.Weapon.BaseDodgeDifficulty;

        double finalBlockTargetNumber = defender.Gameworld.GetCheck(CheckType.BlockCheck).TargetNumber(defender, finalBlockDifficulty, shield?.ShieldType.BlockTrait, assailant);
        double finalDodgeTargetNumber = defender.Gameworld.GetCheck(CheckType.DodgeCheck).TargetNumber(defender, finalDodgeDifficulty, null, assailant);

        bool canBlock = shield is not null && move.Weapon.BaseBlockDifficulty != Difficulty.Impossible && defender.CanSpendStamina(BlockMove.MoveStaminaCost(assailant, defender, shield));
        bool canDodge = defender.RidingMount is null && move.Weapon.BaseDodgeDifficulty != Difficulty.Impossible && defender.CanSpendStamina(DodgeRangeMove.MoveStaminaCost(defender));

        switch (defender.PreferredDefenseType)
        {
            case DefenseType.Block:
                if (canBlock)
                {
                    return new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
                }
                break;
            case DefenseType.Dodge:
                if (canDodge)
                {
                    return new DodgeRangeMove { Assailant = defender };
                }
                break;
        }

        // Otherwise fall back to default handling
        // Calculate the success chance of the various defense types
        List<(DefenseType Defense, double Chance)> availableDefenses = new(3);
        if (canBlock)
        {
            availableDefenses.Add((DefenseType.Block, finalBlockTargetNumber));
        }
        if (canDodge)
        {
            availableDefenses.Add((DefenseType.Dodge, finalDodgeTargetNumber));
        }

        if (availableDefenses.Count == 0)
        {
            return new HelplessDefenseMove
            {
                Assailant = defender
            };
        }

        // Act based on the best one
        switch (availableDefenses.OrderByDescending(x => x.Chance).First().Defense)
        {
            case DefenseType.Block:
                return new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
            case DefenseType.Dodge:
                return new DodgeRangeMove { Assailant = defender };
        }

        return new HelplessDefenseMove
        {
            Assailant = defender
        };
    }

    protected virtual ICombatMove ResponseToRangedNaturalAttack(IRangedAttackMove move, IWeaponAttackMove weaponMove,
        ICharacter defender)
    {
        if (defender.Cover != null)
        {
            return new HelplessDefenseMove { Assailant = defender };
        }

        ICharacter assailant = move.Assailant;
        IShield shield =
            defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IShield>())
                    .FirstMax(x => x.ShieldType.BlockBonus);
        double shieldBonus = shield != null
            ? BlockMove.GetBlockBonus(weaponMove, defender.Body.AlignmentOf(shield.Parent), shield)
            : 0;
        Difficulty finalBlockDifficulty = weaponMove.Attack.Profile.BaseBlockDifficulty.ApplyBonus(shieldBonus);
        Difficulty finalDodgeDifficulty = weaponMove.Attack.Profile.BaseDodgeDifficulty;

        double finalBlockTargetNumber = defender.Gameworld.GetCheck(CheckType.BlockCheck)
                                       .TargetNumber(defender, finalBlockDifficulty, shield?.ShieldType.BlockTrait, assailant);
        double finalDodgeTargetNumber = defender.Gameworld.GetCheck(CheckType.DodgeCheck)
                                       .TargetNumber(defender, finalDodgeDifficulty, null, assailant);

        bool canBlock = shield is not null &&
                       weaponMove.Attack.Profile.BaseBlockDifficulty != Difficulty.Impossible &&
                       defender.CanSpendStamina(BlockMove.MoveStaminaCost(assailant, defender, shield));
        bool canDodge = defender.RidingMount is null &&
                       weaponMove.Attack.Profile.BaseDodgeDifficulty != Difficulty.Impossible &&
                       defender.CanSpendStamina(DodgeRangeMove.MoveStaminaCost(defender));

        if (!canBlock && !canDodge)
        {
            return new HelplessDefenseMove { Assailant = defender };
        }

        return canBlock && finalBlockTargetNumber >= finalDodgeTargetNumber
            ? new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant }
            : new DodgeRangeMove { Assailant = defender };
    }

    public override ICombatMove ResponseToMove(ICombatMove move, IPerceiver defender, IPerceiver assailant)
    {
        if (!(defender is ICharacter defenseCharacter))
        {
            return null; // TODO - item specific helpless defense
        }

        if (move is ScreechAttackMove)
        {
            return new HelplessDefenseMove { Assailant = defenseCharacter };
        }

        IWeaponAttackMove moveAsWeaponAttack = move as IWeaponAttackMove;
        IRangedWeaponAttackMove moveAsRangedAttack = move as IRangedWeaponAttackMove;

        if (!defenseCharacter.Race.CombatSettings.CanDefend)
        {
            return new HelplessDefenseMove { Assailant = defenseCharacter };
        }

        if (defenseCharacter.State.HasFlag(CharacterState.Sleeping) ||
            defenseCharacter.State.HasFlag(CharacterState.Unconscious) ||
            defenseCharacter.State.HasFlag(CharacterState.Paralysed) ||
            defenseCharacter.Effects.Any(x => x.IsBlockingEffect("general")))
        {
            return moveAsWeaponAttack != null || moveAsRangedAttack != null
                ? new HelplessDefenseMove { Assailant = defenseCharacter }
                : null;
        }

        if (move is IRangedWeaponAttackMove rwam)
        {
            return ResponseToRangedWeaponAttack(rwam, defenseCharacter);
        }

        if (move is IRangedAttackMove ram && move is IWeaponAttackMove wam)
        {
            return ResponseToRangedNaturalAttack(ram, wam, defenseCharacter);
        }

        if (move is ChargeToMeleeMove chargeToMeleeMove)
        {
            return ResponseToChargeToMelee(chargeToMeleeMove, defenseCharacter, assailant);
        }

        if (move is MoveToMeleeMove moveToMeleeMove)
        {
            return ResponseToMoveToMelee(moveToMeleeMove, defenseCharacter, assailant);
        }

        if (move is FireAndAdvanceToMeleeMove fireAndAdvanceMove)
        {
            return ResponseToFireAndAdvance(fireAndAdvanceMove, defenseCharacter, assailant);
        }

        return null;
    }

    private ICombatMove DoRangedWeaponAttack(ICharacter combatant)
    {
        List<IRangedWeapon> rangedWeapons =
            GetReadyRangedWeapons(combatant).ToList();
        if ((combatant.CombatSettings.RangedManagement == AutomaticRangedSettings.FullyAutomatic ||
             (combatant.CombatSettings.RangedManagement == AutomaticRangedSettings.ContinueFiringOnly &&
              combatant.EffectsOfType<OpenedFire>().Any())) &&
            (rangedWeapons.Any() || GetNotReadyButLoadableWeapons(combatant).Any()))
        {
            return AttemptUseRangedWeapon(combatant);
        }

        if (combatant.CombatSettings.FallbackToUnarmedIfNoWeapon)
        {
            return AttemptUseRangedNaturalAttack(combatant);
        }

        return null;
    }

    protected virtual ICombatMove HandleGeneralAttacks(ICharacter combatant)
    {
        double roll = Constants.Random.NextDouble();
        if (combatant.CombatSettings.WeaponUsePercentage > 0 &&
            roll <= combatant.CombatSettings.WeaponUsePercentage)
        {
            ICombatMove move = DoRangedWeaponAttack(combatant);
            if (move != null)
            {
                return move;
            }

            if (combatant.CombatSettings.MoveToMeleeIfCannotEngageInRangedCombat &&
                !GetReadyRangedWeapons(combatant).Any() &&
                !GetNotReadyButLoadableWeapons(combatant).Any() &&
                AttemptGetRangedWeapon(combatant) == null
               )
            {
                CannotMakeAValidRangedAttack(combatant);
                return null;
            }
        }

        roll -= combatant.CombatSettings.WeaponUsePercentage;
        if (combatant.CombatSettings.NaturalWeaponPercentage > 0.0 &&
            roll <= combatant.CombatSettings.NaturalWeaponPercentage)
        {
            return AttemptUseRangedNaturalAttack(combatant);
        }

        roll -= combatant.CombatSettings.NaturalWeaponPercentage;
        if (combatant.CombatSettings.MagicUsePercentage > 0 && roll <= combatant.CombatSettings.MagicUsePercentage)
        {
            return AttemptUseMagic(combatant);
        }

        roll -= combatant.CombatSettings.MagicUsePercentage;
        if (combatant.CombatSettings.PsychicUsePercentage > 0 && roll <= combatant.CombatSettings.PsychicUsePercentage)
        {
            return AttemptUsePsychicAbility(combatant);
        }

        return null;
    }

    protected override ICombatMove HandleAttacks(IPerceiver combatant)
    {
        if (!(combatant is ICharacter ch))
        {
            return null;
        }

        if (!WillAttackTarget(ch))
        {
            return null;
        }

        return HandleGeneralAttacks(ch);
    }

    protected virtual ICombatMove HandleMoveToMelee(ICharacter ch)
    {
        if (ch.CombatSettings.MovementManagement == AutomaticMovementSettings.FullyManual)
        {
            return null;
        }

        return null;
    }

    protected override ICombatMove HandleObligatoryCombatMoves(IPerceiver combatant)
    {
        ICombatMove result = base.HandleObligatoryCombatMoves(combatant);
        if (result != null)
        {
            return result;
        }

        if (combatant is ICharacter ch && !combatant.EffectsOfType<PassiveInterdiction>().Any() &&
            !combatant.EffectsOfType<Rescue>().Any())
        {
            IGuardCharacterEffect guard = combatant.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
            if (guard == null || !guard.Interdicting || !guard.Targets.Any())
            {
                return null;
            }

            ICharacter targetNeedsRescuing =
                guard?.Targets.FirstOrDefault(guardedCh =>
                    guardedCh.Location == ch.Location &&
                    ch.CombatTarget != guardedCh &&
                    ch.Combat.Combatants.Any(
                        combatCh => combatCh != ch &&
                                    combatCh.Aim?.Target == guardedCh &&
                                    ch.CanSee(combatCh)));
            if (targetNeedsRescuing != null)
            {
                return new InterposeMove(ch, targetNeedsRescuing);
            }
        }

        return null;
    }

    #endregion

    protected void CannotMakeAValidRangedAttack(ICharacter ch)
    {
        if (ch.CombatSettings.MoveToMeleeIfCannotEngageInRangedCombat)
        {
            ch.OutputHandler.Send(
                "You realise that there is no way for you to engage your opponent at range, and resolve to get into the melee.");
            ch.CombatStrategyMode = CombatStrategyMode.FullAdvance;
        }
    }

    protected virtual ICombatMove HandleChangeLayer(ICharacter ch)
    {
        if (ch.RoomLayer.IsLowerThan(ch.CombatTarget.RoomLayer))
        {
            if (ch.Location.IsUnderwaterLayer(ch.RoomLayer) && ((ISwim)ch).CanAscend().Truth)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.SwimUp);
            }

            if (ch.CanFly().Truth && ch.PositionState != PositionFlying.Instance)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.Fly);
            }

            if (ch.PositionState == PositionFlying.Instance && ((IFly)ch).CanAscend().Truth)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.FlyUp);
            }

            if (ch.CanClimbUp().Truth)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.ClimbUp);
            }
        }
        else
        {
            if (ch.Location.IsSwimmingLayer(ch.RoomLayer) && ((ISwim)ch).CanDive().Truth)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.SwimDown);
            }

            if (ch.CanLand().Truth && ch.RoomLayer.IsLowerThan(RoomLayer.InAir) &&
                ch.PositionState == PositionFlying.Instance)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.Land);
            }

            if (ch.PositionState == PositionFlying.Instance && ((IFly)ch).CanDive().Truth)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.FlyDown);
            }

            if (ch.CanClimbDown().Truth)
            {
                return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.ClimbDown);
            }
        }

        return null;
    }

    public Func<ICellExit, bool> GetPathFunction(ICharacter ch)
    {
        return exit =>
        {
            if (exit.Exit.Door?.IsOpen == false)
            {
                if (!ch.Body.CouldOpen(exit.Exit.Door))
                {
                    return false;
                }
            }

            (CellMovementTransition transition, RoomLayer layer) = exit.MovementTransition(ch);
            switch (transition)
            {
                case CellMovementTransition.NoViableTransition:
                case CellMovementTransition.FallExit:
                    return false;
                case CellMovementTransition.SwimOnly:
                    return ch.Gameworld.GetCheck(CheckType.SwimStayAfloatCheck).WouldBeAbjectFailure(ch);
            }

            return true;
        };
    }
}
