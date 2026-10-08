using MailKit.Net.Smtp;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using System.Numerics;

namespace MudSharp.Economy.Shops;

public class PermanentShop : Shop, IPermanentShop
{
    public PermanentShop(Models.Shop shop, IFuturemud gameworld) : base(shop, gameworld)
    {
        foreach (IRoom room in shop.ShopsStoreroomRooms.SelectNotNull(x => gameworld.Rooms.Get(x.RoomId)))
        {
            AddShopfrontRoom(room);
        }

        _stockroomRoom = gameworld.Rooms.Get(shop.StockroomId ?? 0);
        if (_stockroomRoom is not null)
        {
            AddRoomToStore(_stockroomRoom);
        }

        _workshopRoom = gameworld.Rooms.Get(shop.WorkshopRoomId ?? 0);
        if (_workshopRoom is not null)
        {
            AddRoomToStore(_workshopRoom);
        }

        foreach (ShopsTill item in shop.ShopsTills)
        {
            _tillItemIds.Add(item.GameItemId);
        }

        Changed = false;

        InitialiseShop();
    }

    public PermanentShop(IEconomicZone zone, IRoom originalShopFront, string name) : this(zone, originalShopFront, name, "Permanent")
    {
    }

    protected PermanentShop(IEconomicZone zone, IRoom originalShopFront, string name, string shopType) : base(zone, originalShopFront, name, shopType)
    {
        AddShopfrontRoom(originalShopFront);
        InitialiseShop();
    }

    protected override void InitialiseShop()
    {
        foreach (IRoom room in AllShopRooms)
        {
            room.Shop = this;
        }
    }

    public override void PostLoadInitialisation()
    {
        foreach (IMerchandise merchandise in Merchandises)
        {
            List<IGameItem> stocked = StockedItems(merchandise).ToList();
            _stockedMerchandiseCounts[merchandise] = stocked.Sum(x => x.Quantity);
            foreach (IGameItem item in stocked)
            {
                _stockedMerchandise.Add(merchandise, item.Id);
            }
        }
    }

    protected override void Save(Models.Shop dbitem)
    {
        dbitem.StockroomId = StockroomRoom?.Id;
        dbitem.WorkshopRoomId = WorkshopRoom?.Id;
        FMDB.Context.ShopsTills.RemoveRange(dbitem.ShopsTills);
        foreach (long item in _tillItemIds)
        {
            Models.GameItem dbtill = FMDB.Context.GameItems.Find(item);
            if (dbtill != null)
            {
                dbitem.ShopsTills.Add(new ShopsTill { Shop = dbitem, GameItemId = item });
            }
        }

        FMDB.Context.ShopsStoreroomRooms.RemoveRange(dbitem.ShopsStoreroomRooms);
        foreach (IRoom room in ShopfrontRooms)
        {
            dbitem.ShopsStoreroomRooms.Add(new ShopsStoreroomRoom { Shop = dbitem, RoomId = room.Id });
        }
    }

    private void RemoveRoomFromStore(IRoom room)
    {
        room.Shop = null;
        MudDateTime time = room.DateTime();
        foreach (IGameItem item in room.GameItems.SelectMany(x => x.DeepItems))
        {
            if (item.AffectedBy<ItemOnDisplayInShop>())
            {
                DisposeFromStock(null, item);
            }
        }
    }

    private void AddRoomToStore(IRoom room)
    {
        room.Shop = this;
        room.RoomRequestsDeletion -= Room_RoomRequestsDeletion;
        room.RoomRequestsDeletion += Room_RoomRequestsDeletion;
        room.RoomProposedForDeletion -= Room_RoomProposedForDeletion;
        room.RoomProposedForDeletion += Room_RoomProposedForDeletion;
    }

    private void Room_RoomProposedForDeletion(IRoom room, ProposalRejectionResponse response)
    {
        if (room == WorkshopRoom)
        {
            response.RejectWithReason($"That room is a workshop room for shop #{Id:N0} ({Name.ColourName()})");
            return;
        }

        if (room == StockroomRoom)
        {
            response.RejectWithReason($"That room is a stockroom for shop #{Id:N0} ({Name.ColourName()})");
            return;
        }
    }

    private void Room_RoomRequestsDeletion(object sender, EventArgs e)
    {
        IRoom room = (IRoom)sender;
        RemoveShopfrontRoom(room);
    }

    public void AddShopfrontRoom(IRoom room)
    {
        if (!_shopfrontRooms.Contains(room))
        {
            AddRoomToStore(room);
            _shopfrontRooms.Add(room);
            Changed = true;
        }
    }

    public void RemoveShopfrontRoom(IRoom room)
    {
        _shopfrontRooms.Remove(room);
        RemoveRoomFromStore(room);
        Changed = true;
    }

    public void AddTillItem(IGameItem till)
    {
        _tillItemIds.Add(till.Id);
        Changed = true;
    }

    public void RemoveTillItem(IGameItem till)
    {
        _tillItemIds.Remove(till.Id);
        Changed = true;
    }

    public void AddDisplayContainer(IGameItem item)
    {
        _displayContainerIds.Add(item.Id);
        Changed = true;
    }

    public void RemoveDisplayContainer(IGameItem item)
    {
        _displayContainerIds.Remove(item.Id);
        Changed = true;
    }

    private readonly List<IRoom> _shopfrontRooms = new();
    public IEnumerable<IRoom> ShopfrontRooms => _shopfrontRooms;
    private IRoom _workshopRoom;
    private IRoom _stockroomRoom;
    public IRoom WorkshopRoom
    {
        get => _workshopRoom;
        set
        {
            if (_workshopRoom != null && value != _workshopRoom)
            {
                RemoveRoomFromStore(_workshopRoom);
            }

            RemoveRoomFromStore(value);
            AddRoomToStore(value);
            _workshopRoom = value;
            Changed = true;
        }
    }

    public IRoom StockroomRoom
    {
        get => _stockroomRoom;
        set
        {
            if (_stockroomRoom != null && value != _stockroomRoom)
            {
                RemoveRoomFromStore(_stockroomRoom);
            }

            RemoveRoomFromStore(value);
            AddRoomToStore(value);
            _stockroomRoom = value;
            Changed = true;
        }
    }
    public IEnumerable<IRoom> AllShopRooms => ShopfrontRooms.Concat(new[]
    {
        WorkshopRoom,
        StockroomRoom
    }.WhereNotNull(x => x));

    private IEnumerable<IRoom> PublicSaleStockRooms => ShopfrontRooms.Concat(new[]
    {
        StockroomRoom
    }.WhereNotNull(x => x));

    private readonly HashSet<long> _tillItemIds = new();

    public IEnumerable<IGameItem> TillItems =>
        AllShopRooms.SelectMany(x => x.GameItems).Where(x => _tillItemIds.Contains(x.Id));

    private readonly HashSet<long> _displayContainerIds = new();

    public IEnumerable<IGameItem> DisplayContainers => AllShopRooms.SelectMany(x => x.GameItems)
                                                                   .Where(x => _displayContainerIds.Contains(x.Id));

    public override IEnumerable<IGameItem> DoAutoRestockForMerchandise(IMerchandise merchandise, List<(IGameItem Item, IGameItem Container)> purchasedItems = null)
    {
        int quantityToRestock = merchandise.MinimumStockLevels - _stockedMerchandiseCounts[merchandise];
        int originalQuantity = quantityToRestock;
        List<IGameItem> newItems = new();
        DictionaryWithDefault<IGameItem, IGameItem> newItemsOriginalContainers = new();
        if (merchandise.PreserveVariablesOnReorder && purchasedItems != null)
        {
            foreach ((IGameItem Item, IGameItem Container) item in purchasedItems)
            {
                IGameItem newItem = item.Item.DeepCopy(true, false);
                newItem.Skin = merchandise.Skin;
                newItems.Add(newItem);
                newItemsOriginalContainers[newItem] = item.Container;
                quantityToRestock -= newItem.Quantity;
            }
        }

        if (quantityToRestock > 0)
        {
            if (merchandise.Item.Components.Any(x => x is StackableGameItemComponentProto))
            {
                IGameItem newItem = merchandise.Item.CreateNew(null);
                newItem.Skin = merchandise.Skin;
                newItem.GetItemType<StackableGameItemComponent>().Quantity = quantityToRestock;
                newItems.Add(newItem);
                Gameworld.Add(newItem);
            }
            else
            {
                for (int i = 0; i < quantityToRestock; i++)
                {
                    IGameItem newItem = merchandise.Item.CreateNew(null);
                    newItem.Skin = merchandise.Skin;
                    newItems.Add(newItem);
                    Gameworld.Add(newItem);
                }
            }
        }

        if (!newItems.Any())
        {
            return newItems;
        }

        foreach (IGameItem item in newItems)
        {
            item.SetOwner(this);
            item.AddEffect(new ItemOnDisplayInShop(item, this, merchandise));
            SortItemToStorePhysicalLocation(item, merchandise, newItemsOriginalContainers[item]);
            item.HandleEvent(EventType.ItemFinishedLoading, item);
            item.Login();
            _stockedMerchandise.Add(merchandise, item.Id);
            _stockedMerchandiseCounts[merchandise] += item.Quantity;
        }

        AddTransaction(new TransactionRecord(ShopTransactionType.Restock, Currency, this,
            ShopfrontRooms.First().DateTime(), null, merchandise.EffectiveAutoReorderPrice * originalQuantity, 0.0M, merchandise));
        return newItems;
    }

    /// <inheritdoc />
    public override void SortItemToStorePhysicalLocation(IGameItem item, IMerchandise merchandise, IGameItem container)
    {
        if (container is not null && container.GetItemType<IContainer>()?.CanPut(item) == true)
        {
            container.GetItemType<IContainer>().Put(null, item, false);
        }
        else if (merchandise.PreferredDisplayContainer is not null &&
                 merchandise.PreferredDisplayContainer.GetItemType<IContainer>()?.CanPut(item) == true)
        {
            merchandise.PreferredDisplayContainer.GetItemType<IContainer>().Put(null, item, false);
        }
        else
        {

            IRoom targetRoom = StockroomRoom;
            if (targetRoom == null)
            {
                targetRoom = ShopfrontRooms.First();
            }
            targetRoom.Insert(item);
        }
    }

    #region Overrides of Shop

    /// <inheritdoc />
    public override IEnumerable<IGameItem> DoAutostockAllMerchandise()
    {
        List<IGameItem> stocked = new();
        List<IGameItem> items = AllShopRooms.SelectMany(x => x.GameItems).SelectMany(x => x.DeepItems).ToList();
        foreach (IGameItem item in items)
        {
            if (item.AffectedBy<ItemOnDisplayInShop>(this))
            {
                continue;
            }

            IMerchandise merch = Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item));
            if (merch == null)
            {
                continue;
            }

            AddToStock(null, item, merch);
            stocked.Add(item);
        }

        return stocked;
    }

    #endregion

    public override IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise)
    {
        List<IGameItem> stocked = new();
        foreach (IGameItem item in AllShopRooms.SelectMany(x => x.GameItems).SelectMany(x => x.DeepItems).ToList())
        {
            if (item.AffectedBy<ItemOnDisplayInShop>(this))
            {
                continue;
            }

            IMerchandise merch = Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item));
            if (merch == null)
            {
                continue;
            }

            AddToStock(null, item, merch);
            stocked.Add(item);
        }

        return stocked;
    }

    public override IEnumerable<IGameItem> StockedItems(IMerchandise merchandise)
    {
        List<IRoom> shopfrontRooms = ShopfrontRooms.ToList();
        List<IRoom> stockRooms = PublicSaleStockRooms.ToList();
        return shopfrontRooms
            .SelectMany(x => x.Characters)
            .SelectMany(x => x.Body.HeldItems)
            .Concat(
                stockRooms
                    .SelectMany(x => x.GameItems)
                    .SelectMany(x => x.ShallowItems)
            )
            .Where(x => x.AffectedBy<ItemOnDisplayInShop>(merchandise))
            .Distinct();
    }

    public override IEnumerable<IGameItem> AllStockedItems
    {
        get
        {
            List<IRoom> shopfrontRooms = ShopfrontRooms.ToList();
            List<IRoom> stockRooms = PublicSaleStockRooms.ToList();
            return shopfrontRooms
                .SelectMany(x => x.Characters)
                .SelectMany(x => x.Body.HeldItems)
                .Concat(
                    stockRooms
                        .SelectMany(x => x.GameItems)
                        .SelectMany(x => x.ShallowItems)
                )
                .Where(x => x.AffectedBy<ItemOnDisplayInShop>())
                .Distinct();
        }
    }

    public override void CheckFloat()
    {
        decimal cashInRegister =
            GetCurrencyPilesForShop()
                .Where(x => x.Currency == Currency)
                .Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value)) + CashBalance;
        if (cashInRegister > ExpectedCashBalance)
        {
            AddTransaction(new TransactionRecord(ShopTransactionType.Deposit, Currency, this,
                EconomicZone.ZoneForTimePurposes.DateTime(), null, cashInRegister - ExpectedCashBalance, 0.0M, null));
            ExpectedCashBalance = cashInRegister;
            Changed = true;
            return;
        }

        if (cashInRegister < ExpectedCashBalance)
        {
            AddTransaction(new TransactionRecord(ShopTransactionType.Withdrawal, Currency, this,
                EconomicZone.ZoneForTimePurposes.DateTime(), null, ExpectedCashBalance - cashInRegister, 0.0M, null));
            ExpectedCashBalance = cashInRegister;
            Changed = true;
            return;
        }
    }
    protected override void ShowInfo(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine();
        if (IsEmployee(actor) || actor.IsAdministrator())
        {
            if (actor.IsAdministrator())
            {
                sb.AppendLine($"These are the locations for this store:");
                sb.AppendLine($"\tWorkshop: {WorkshopRoom?.GetFriendlyReference(actor) ?? "None".ColourError()}");
                sb.AppendLine($"\tStockroom: {StockroomRoom?.GetFriendlyReference(actor) ?? "None".ColourError()}");
                foreach (IRoom room in ShopfrontRooms)
                {
                    sb.AppendLine($"\tShopfront: {room.GetFriendlyReference(actor)}");
                }
            }

            else
            {
                if (actor.Location == WorkshopRoom)
                {
                    sb.AppendLine("The location you are currently in is the workshop for this store.".ColourCommand());
                }
                else if (actor.Location == StockroomRoom)
                {
                    sb.AppendLine("The location you are currently in is the stockroom for this store.".ColourCommand());
                }
                else if (ShopfrontRooms.Contains(actor.Location))
                {
                    sb.AppendLine("The location you are currently in is the shopfront for this store.".ColourCommand());
                }
            }

            if (TillItems.Any())
            {
                sb.AppendLine();
            }
            foreach (IGameItem item in TillItems)
            {
                sb.AppendLine($"{item.HowSeen(actor, true)} is a till for this store.");
            }

            foreach (IGameItem item in DisplayContainers)
            {
                sb.AppendLine($"{item.HowSeen(actor, true)} is a display container for this store.");
            }
        }
    }

    public override (int OnFloorCount, int InStockroomCount) StocktakeMerchandise(IMerchandise whichMerchandise)
    {
        if (!_merchandises.Contains(whichMerchandise))
        {
            return (0, 0);
        }

        RecalculateStockedItems(whichMerchandise, 0);
        int floorStock =
            ShopfrontRooms.SelectMany(x => x.Characters).SelectMany(x => x.Body.HeldItems)
                          .Concat(
                              ShopfrontRooms
                                  .SelectMany(x => x.GameItems)
                                  .SelectMany(x => x.DeepItems)
                          )
                          .Where(x => x.AffectedBy<ItemOnDisplayInShop>(whichMerchandise))
                          .Distinct()
                          .Sum(x => x.Quantity);
        return (floorStock, _stockedMerchandiseCounts[whichMerchandise] - floorStock);
    }

    public override (double OnFloorWeight, double InStockroomWeight) StocktakeMerchandiseWeight(IMerchandise whichMerchandise)
    {
        if (!_merchandises.Contains(whichMerchandise))
        {
            return (0.0, 0.0);
        }

        RecalculateStockedItems(whichMerchandise, 0);
        var floorStock =
            ShopfrontRooms.SelectMany(x => x.Characters).SelectMany(x => x.Body.HeldItems)
                          .Concat(
                              ShopfrontRooms
                                  .SelectMany(x => x.GameItems)
                                  .SelectMany(x => x.DeepItems)
                          )
                          .Where(x => x.AffectedBy<ItemOnDisplayInShop>(whichMerchandise))
                          .Distinct()
                          .Sum(x => x.GetItemType<ICommodity>()?.Weight ?? 0.0);
        var totalStock = StockedItems(whichMerchandise)
                         .Sum(x => x.GetItemType<ICommodity>()?.Weight ?? 0.0);
        return (floorStock, Math.Max(0.0, totalStock - floorStock));
    }

    protected override (bool Truth, string Reason) CanBuyInternal(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method, string extraArguments = null)
    {
        if (method is CashPayment)
        {
            if (!TillItems.Any() && StockroomRoom is null)
            {
                return (false, "the store is currently missing its till, and so cannot do cash transactions.");
            }
        }

        return (true, string.Empty);
    }

    /// <inheritdoc />
    public override (bool Truth, string Reason) CanSellInternal(ICharacter actor, IMerchandise merchandise, IPaymentMethod method,
        IGameItem item)
    {
        return (true, string.Empty);
    }

    public override IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "shopfront":
                return new CollectionVariable(ShopfrontRooms.ToList(), ProgVariableTypes.Location);
            case "storeroom":
                return StockroomRoom;
            case "workshop":
                return WorkshopRoom;
            case "tills":
                return new CollectionVariable(TillItems.ToList(), ProgVariableTypes.Item);
            default:
                return base.GetProperty(property);
        }
    }

    public override IEnumerable<IRoom> CurrentLocations => ShopfrontRooms;

    public override bool IsReadyToDoBusiness => TillItems.Any() || StockroomRoom is not null;

    public override IReadOnlyDictionary<ICurrencyPile, Dictionary<ICoin, int>> GetCurrencyForShop(decimal amount)
    {
        if (TillItems.Any())
        {
            return Currency.FindCurrency(TillItems.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()).ToList(), amount);
        }

        if (StockroomRoom is not null)
        {
            return Currency.FindCurrency(StockroomRoom.GameItems.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()).ToList(), amount);
        }

        return new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>();
    }

    public override IEnumerable<ICurrencyPile> GetCurrencyPilesForShop()
    {
        List<ICurrencyPile> piles = new();
        piles.AddRange(TillItems.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()));
        if (StockroomRoom is not null)
        {
            piles.AddRange(StockroomRoom.GameItems
                                        .Where(x => !TillItems.Contains(x))
                                        .SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()));
        }
        return piles;
    }

    public override void AddCurrencyToShop(IGameItem currencyPile)
    {
        currencyPile.SetOwner(this);
        foreach (IGameItem item in TillItems)
        {
            IContainer itemContainer = item.GetItemType<IContainer>();
            if (itemContainer is null)
            {
                continue;
            }

            itemContainer.Put(null, currencyPile);
            return;
        }

        if (StockroomRoom is not null)
        {
            StockroomRoom.Insert(currencyPile);
            return;
        }

        if (ShopfrontRooms.Any())
        {
            ShopfrontRooms.First().Insert(currencyPile);
            Gameworld.SystemMessage($"The shop {Name.ColourName()} inserted money on the public ground because it had no tills or stockroom.", true);
            return;
        }

        ICurrencyPile currencyItem = currencyPile.GetItemType<ICurrencyPile>();
        Gameworld.SystemMessage($"The shop {Name.ColourName()} junked {currencyItem.Currency.Describe(currencyItem.TotalValue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} because it had nowhere to put it.", true);
    }
}
