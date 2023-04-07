using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat {
    public delegate void CombatMergeDelegate(ICombat obsoleteCombat, ICombat newCombat);

    public interface ICombat {
        IEnumerable<IPerceiver> Combatants { get; }

        /// <summary>
        ///     If true, this combat would be considered "Friendly", e.g. a spar, training bout, competition etc.
        /// </summary>
        bool Friendly { get; }

        void JoinCombat(IPerceiver character, Difficulty initialDelayDifficulty = Difficulty.Automatic);

        /// <summary>
        ///     Removes a specified combatant from the combat
        /// </summary>
        /// <param name="character">The character leaving the combat</param>
        /// <returns>True if combat ended entirely because of this action</returns>
        bool LeaveCombat(IPerceiver character);

        bool CanFreelyLeaveCombat(IPerceiver who);

        void EndCombat(bool echo);
        void MergeCombat(ICombat oldCombat);

        /// <summary>
        ///     Ends the combat with no echoes, and no changing of character statuses. Used for two separate combats merging.
        /// </summary>
        void EndCombatNoHandling();

        event EventHandler CombatEnds;
        event CombatMergeDelegate CombatMerged;
        void TruceRequested(IPerceiver combatant);
        string DescribeFor(ICharacter voyeur);

        string LDescAddendumFor(ICombatant combatant, IPerceiver voyeur);

        void CombatAction(IPerceiver perceiver);
        void CombatAction(IPerceiver perceiver, ICombatMove action);
        IEnumerable<IPerceiver> MeleeProximityOfCombatant(IPerceiver combatant);

        void ReevaluateMeleeRange(IPerceiver whoMoved);
    }
}