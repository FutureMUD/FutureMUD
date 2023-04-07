using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Groups;

namespace MudSharp.GameItems {
    public interface IGameItemGroup : IKeywordedItem, ISaveable {
        IEnumerable<IGameItemGroupForm> Forms { get; }
        string Describe(IPerceiver voyeur, IEnumerable<IGameItem> items, ICell cell);
        void BuildingCommand(ICharacter actor, StringStack command);
        string Show(IPerceiver voyeur);
        string LookDescription(IPerceiver voyeur, IEnumerable<IGameItem> items, ICell cell);
        void RemoveForm(IGameItemGroupForm form);
        event EventHandler OnDelete;
        void Delete();
        bool CannotBeDeleted { get; }
    }
}