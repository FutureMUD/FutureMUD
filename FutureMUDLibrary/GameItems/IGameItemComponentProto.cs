using MudSharp.Character;
using MudSharp.Framework.Revision;

namespace MudSharp.GameItems {
    public interface IGameItemComponentProto : IEditableRevisableItem {
        string Description { get; }
        string TypeDescription { get; }

        /// <summary>
        ///     If true, the specified component is read-only and cannot be changed by the user
        /// </summary>
        bool ReadOnly { get; }

        IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false);
        IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent);
        string ComponentDescriptionOLC(ICharacter actor);

        /// <summary>
        /// Whether or not we should warn the purger before purging this item
        /// </summary>
        bool WarnBeforePurge { get; }

        /// <summary>
        /// If the component gets into the game an alternate way and items with this component should not be permitted to be loaded through the ITEM LOAD command or via progs.
        /// </summary>
        bool PreventManualLoad { get; }
    }
}