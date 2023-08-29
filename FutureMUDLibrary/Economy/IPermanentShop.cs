using MudSharp.Construction;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.Economy;

public interface IPermanentShop : IShop
    {
        IEnumerable<ICell> ShopfrontCells { get; }
        ICell WorkshopCell { get; set; }
        ICell StockroomCell { get; set; }
        IEnumerable<ICell> AllShopCells { get; }
        IEnumerable<IGameItem> TillItems { get; }
        IEnumerable<IGameItem> DisplayContainers { get; }
        void AddShopfrontCell(ICell cell);
        void RemoveShopfrontCell(ICell cell);

        void AddTillItem(IGameItem till);
        void RemoveTillItem(IGameItem till);

        void AddDisplayContainer(IGameItem item);
        void RemoveDisplayContainer(IGameItem item);
    }
