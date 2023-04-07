using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat {
    public interface IRangedWeaponType : IFrameworkItem {
        /// <summary>
        ///     Trait used to line up a shot and take it with this weapon
        /// </summary>
        ITraitDefinition FireTrait { get; }

        /// <summary>
        ///     Trait used to load, prepare, unload this weapon
        /// </summary>
        ITraitDefinition OperateTrait { get; }

        /// <summary>
        ///     Whether this weapon can be fired when in melee range
        /// </summary>
        bool FireableInMelee { get; }

        /// <summary>
        ///     The default number of rooms that this weapon can fire at a target
        /// </summary>
        uint DefaultRangeInRooms { get; }

        /// <summary>
        ///     A TraitExpression showing what bonus should be applied to the shot. Parameters are quality, range, inmelee (0 or
        ///     1), aim% (0-1)
        /// </summary>
        ITraitExpression AccuracyBonusExpression { get; }

        /// <summary>
        ///     A TraitExpression showing what damage bonus should be applied to the shot. Parameters are quality, range
        /// </summary>
        ITraitExpression DamageBonusExpression { get; }

        AmmunitionLoadType AmmunitionLoadType { get; }

        string SpecificAmmunitionGrade { get; }

        int AmmunitionCapacity { get; }
        RangedWeaponType RangedWeaponType { get; }

        double StaminaToFire { get; }
        double StaminaPerLoadStage { get; }
        double CoverBonus { get; }

        Difficulty BaseAimDifficulty { get; }

        string Show(ICharacter character);

        CheckType FireCheck { get; }

        double LoadCombatDelay { get; }

        double ReadyCombatDelay { get; }

        double FireCombatDelay { get; }

        double AimBonusLostPerShot { get; }

        bool RequiresFreeHandToReady { get; }
        bool AlwaysRequiresTwoHandsToWield { get; }
        WeaponClassification Classification { get; }
    }
}