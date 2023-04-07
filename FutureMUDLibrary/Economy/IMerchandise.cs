using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy
{
    public interface IMerchandise : ISaveable, IKeywordedItem, IFutureProgVariable
    {
        IShop Shop { get; }
        bool AutoReordering { get; }
        decimal AutoReorderPrice { get; }
        bool PreserveVariablesOnReorder { get; }
        int MinimumStockLevels { get; }
        double MinimumStockLevelsByWeight { get; }
        decimal BasePrice { get; }
        IGameItem PreferredDisplayContainer { get; }
        string ListDescription { get; }
        IGameItemProto Item { get; }
        IGameItemSkin Skin { get; }

        bool IsMerchandiseFor(IGameItem item);
        bool BuildingCommand(ICharacter actor, StringStack command);
        void ShowToBuilder(ICharacter actor);
    }
}
