using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.GameItems.Groups {
    /// <summary>
    ///     An IGameItemGroupForm is an interface for the subforms of an IGameItemGroup
    /// </summary>
    public interface IGameItemGroupForm : IFrameworkItem, ISaveable {
        /// <summary>
        ///     Determines whether this form applies in the given cell
        /// </summary>
        /// <param name="cell">The cell against which to check</param>
        /// <returns>True if it applies</returns>
        bool Applies(ICell cell);

        bool Applies(long cellId);
        bool SpecialFormFor(long cellId);

        /// <summary>
        ///     Describe the item stack to the voyeur as what they will see in the ldesc
        /// </summary>
        /// <param name="voyeur">The person observing the item stack</param>
        /// <param name="items">The item stack</param>
        /// <returns>A string representing the ldesc seen</returns>
        string Describe(IPerceiver voyeur, IEnumerable<IGameItem> items);

        /// <summary>
        ///     Handles building related input from a character
        /// </summary>
        /// <param name="actor">The actor entering the building command</param>
        /// <param name="command">A stringstack of the building command argument</param>
        void BuildingCommand(ICharacter actor, StringStack command);

        /// <summary>
        ///     Return the description shown when someone uses the look command to look at the stack
        /// </summary>
        /// <param name="voyeur">The person observing the item stack</param>
        /// <param name="items">The item stack</param>
        /// <returns>A string representing the desc seen</returns>
        string LookDescription(IPerceiver voyeur, IEnumerable<IGameItem> items);

        /// <summary>
        ///     Show the IGameItemGroupForm to a builder
        /// </summary>
        /// <param name="voyeur">The builder in question</param>
        /// <returns>A string describing building information about the IGameItemGroupForm</returns>
        string Show(IPerceiver voyeur);
    }
}