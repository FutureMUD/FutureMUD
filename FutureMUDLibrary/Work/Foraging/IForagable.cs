using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Foraging {
    /// <summary>
    ///     A foragable is an option that may be selected when someone uses the forage command
    /// </summary>
    public interface IForagable : IEditableRevisableItem {
        /// <summary>
        ///     Which keywords for foraging this item may be used, e.g. food, stone, artifacts, etc.
        /// </summary>
        IEnumerable<string> ForagableTypes { get; }

        /// <summary>
        ///     The difficulty of the forage check if specifically foraging for this item - impossible items may not be
        ///     specifically foraged for
        /// </summary>
        Difficulty ForageDifficulty { get; }

        /// <summary>
        ///     The relative chance of foraging this item
        /// </summary>
        int RelativeChance { get; }

        /// <summary>
        ///     The minimum outcome required to forage this item
        /// </summary>
        Outcome MinimumOutcome { get; }

        /// <summary>
        ///     The maximum outcome required to forage this item
        /// </summary>
        Outcome MaximumOutcome { get; }

        /// <summary>
        ///     A string containing a dice expression for how many items to load
        /// </summary>
        string QuantityDiceExpression { get; }

        /// <summary>
        ///     The item prototype to be loaded when this item is foraged
        /// </summary>
        IGameItemProto ItemProto { get; }

        /// <summary>
        ///     An optional prog to execute whenever this item is foraged
        ///     The parameters to the prog are Character, Quantity
        /// </summary>
        IFutureProg OnForageProg { get; }

        /// <summary>
        ///     Determines whether any foragable criteria are met
        /// </summary>
        /// <param name="character">The character doing the foraging</param>
        /// <param name="outcome">The outcome of the forage check</param>
        /// <returns>True if the character can forage this item</returns>
        bool CanForage(ICharacter character, Outcome outcome);
    }
}