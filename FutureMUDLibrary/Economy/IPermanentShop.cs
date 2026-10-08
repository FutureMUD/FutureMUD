using MudSharp.Construction;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.Economy;

public interface IPermanentShop : IShop
{
    IEnumerable<IRoom> ShopfrontRooms { get; }
    IRoom WorkshopRoom { get; set; }
    IRoom StockroomRoom { get; set; }
    IEnumerable<IRoom> AllShopRooms { get; }
    IEnumerable<IGameItem> TillItems { get; }
    IEnumerable<IGameItem> DisplayContainers { get; }
    void AddShopfrontRoom(IRoom room);
    void RemoveShopfrontRoom(IRoom room);

    void AddTillItem(IGameItem till);
    void RemoveTillItem(IGameItem till);

    void AddDisplayContainer(IGameItem item);
    void RemoveDisplayContainer(IGameItem item);
}
