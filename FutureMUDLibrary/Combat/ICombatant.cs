using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Combat {
    public interface ICombatant : ILocateable {
        /// <summary>
        ///     The combat in which this combatant is presently participating
        /// </summary>
        ICombat Combat { get; set; }

        /// <summary>
        ///     The combatant which this combatant is targeting
        /// </summary>
        IPerceiver CombatTarget { get; set; }

        /// <summary>
        ///     The advantage this combatant has gained with regards to defense
        /// </summary>
        int DefensiveAdvantage { get; set; }

        /// <summary>
        ///     The advantage this combatant has gained with regards to offense
        /// </summary>
        int OffensiveAdvantage { get; set; }

        /// <summary>
        ///     The defense type preferred to defend against incoming attacks
        /// </summary>
        DefenseType PreferredDefenseType { get; set; }

        CombatStrategyMode CombatStrategyMode { get; set; }

        IAimInformation Aim { get; set; }

        IBodypart TargettedBodypart { get; set; }

        /// <summary>
        /// This property controls whether the combatant is in melee range with their target.
        /// </summary>
        bool MeleeRange { get; set; }

        /// <summary>
        /// This property calculates whether a combatant is in melee range with their target or is themselves engaged in melee by another
        /// </summary>
        bool IsEngagedInMelee { get; }

        ICombatantCover Cover { get; set; }

        /// <summary>
        ///     Asks the combatant to acquire a new target, if they don't already have one
        /// </summary>
        void AcquireTarget();

        /// <summary>
        ///     Gives the combatant a chance to evaluate the situation and decide whether to remain in the combat
        /// </summary>
        /// <returns>True if the combatant is still fighting</returns>
        bool CheckCombatStatus();

        /// <summary>
        /// </summary>
        /// <param name="move"></param>
        /// <param name="assailant"></param>
        /// <returns></returns>
        ICombatMove ResponseToMove(ICombatMove move, IPerceiver assailant);

        ItemQuality NaturalWeaponQuality(INaturalAttack attack);

        ICombatMove ChooseMove();

        bool CanTruce();
        string WhyCannotTruce();

        Facing GetFacingFor(ICombatant opponent, bool reset = false);

        bool CanEngage(IPerceiver target);
        string WhyCannotEngage(IPerceiver target);
        bool Engage(IPerceiver target, bool ranged = false);

        event PerceivableEvent OnJoinCombat;
        event PerceivableEvent OnEngagedInMelee;
        event PerceivableEvent OnLeaveCombat;

        /// <summary>
        /// Takes a SelectedCombatAction and either executes the action if the combatant is idle or queues it if they aren't
        /// </summary>
        /// <param name="action">The SelectedCombatAction to queue or execute</param>
        /// <returns>True if the action was queued, false otherwise.</returns>
        bool TakeOrQueueCombatAction(ISelectedCombatAction action);
        ICharacterCombatSettings CombatSettings { get; set; }

        double GetBonusForDefendersFromTargeting();
        double GetDefensiveAdvantagePenaltyFromTargeting();
    }
}