using MudSharp.Effects.Concrete;

namespace MudSharp.Combat.Strategies;

public class MeleeShooter : StandardMeleeStrategy
{
    public new static ICombatStrategy Instance { get; } = new MeleeShooter();

    protected MeleeShooter()
    {
    }

    #region Overrides of StandardMeleeStrategy

    public override CombatStrategyMode Mode => CombatStrategyMode.MeleeShooter;

    protected override IEnumerable<IRangedWeapon> GetNotReadyButLoadableWeapons(ICharacter shooter)
    {
        return
            shooter.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
                   .Where(x =>
                       x.WeaponType.FireableInMelee &&
                       !x.ReadyToFire && (
                           (!x.IsLoaded && x.CanLoad(shooter, true)) || (!x.IsReadied && x.CanReady(shooter))));
    }

    protected override IEnumerable<IRangedWeapon> GetReadyRangedWeapons(ICharacter shooter)
    {
        return
            shooter.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
                   .Where(x =>
                       x.WeaponType.FireableInMelee &&
                       x.ReadyToFire &&
                       shooter.CanSpendStamina(x.WeaponType.StaminaToFire))
                   .OrderBy(x => x.Parent.IsItemType<IMeleeWeapon>());
    }

    protected override ICombatMove HandleWeaponAttackRolled(ICharacter combatant)
    {
        if ((combatant.CombatSettings.RangedManagement == AutomaticRangedSettings.FullyAutomatic ||
             (combatant.CombatSettings.RangedManagement == AutomaticRangedSettings.ContinueFiringOnly &&
              combatant.EffectsOfType<OpenedFire>().Any())) &&
            combatant.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
                     .Any(x => x.ReadyToFire && x.WeaponType.FireableInMelee))
        {
            ICombatMove move = AttemptUseRangedWeapon(combatant);
            if (move != null)
            {
                return move;
            }
        }

        return base.HandleWeaponAttackRolled(combatant);
    }

    protected override ICombatMove AttemptGetRangedWeapon(ICharacter ch)
    {
        return BaseAttemptGetRangedWeapon(ch);
    }

    #endregion
}