using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Payment;
using MudSharp.GameItems.Prototypes;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Components;
using MudSharp.Events;
using MudSharp.Models;

namespace MudSharp.Economy;

public class PermanentShop : Shop, IPermanentShop
{
    public PermanentShop(Models.Shop shop, IFuturemud gameworld) : base(shop, gameworld)
    {
        _shopfrontCells.AddRange(shop.ShopsStoreroomCells.Select(x => gameworld.Cells.Get(x.CellId)));
        _stockroomCell = gameworld.Cells.Get(shop.StockroomCellId ?? 0);
        _workshopCell = gameworld.Cells.Get(shop.WorkshopCellId ?? 0);
        foreach (var item in shop.ShopsTills)
        {
            _tillItemIds.Add(item.GameItemId);
        }

        InitialiseShop();
    }

    public PermanentShop(IEconomicZone zone, ICell originalShopFront, string name) : base(zone, originalShopFront, name, "Permanent")
    {
        InitialiseShop();
    }

    protected override void InitialiseShop()
    {
        foreach (var cell in AllShopCells)
        {
            cell.Shop = this;
        }
    }

    public override void PostLoadInitialisation()
    {
        foreach (var merchandise in Merchandises)
        {
            var stocked = StockedItems(merchandise).ToList();
            _stockedMerchandiseCounts[merchandise] = stocked.Sum(x => x.Quantity);
            foreach (var item in stocked)
            {
                _stockedMerchandise.Add(merchandise, item.Id);
            }
        }
    }

    protected override void Save(Models.Shop dbitem)
    {
        dbitem.StockroomCellId = StockroomCell?.Id;
        dbitem.WorkshopCellId = WorkshopCell?.Id;
        FMDB.Context.ShopsTills.RemoveRange(dbitem.ShopsTills);
        foreach (var item in _tillItemIds)
        {
            var dbtill = FMDB.Context.GameItems.Find(item);
            if (dbtill != null)
            {
                dbitem.ShopsTills.Add(new ShopsTill { Shop = dbitem, GameItemId = item });
            }
        }

        FMDB.Context.ShopsStoreroomCells.RemoveRange(dbitem.ShopsStoreroomCells);
        foreach (var cell in ShopfrontCells)
        {
            dbitem.ShopsStoreroomCells.Add(new ShopsStoreroomCell { Shop = dbitem, CellId = cell.Id });
        }
    }

    private void RemoveCellFromStore(ICell cell)
    {
        cell.Shop = null;
        var time = cell.DateTime();
        foreach (var item in cell.GameItems.SelectMany(x => x.DeepItems))
        {
            if (item.AffectedBy<ItemOnDisplayInShop>())
            {
                DisposeFromStock(null, item);
            }
        }
    }

    private void AddCellToStore(ICell cell)
    {
        cell.Shop = this;
        cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
        cell.CellRequestsDeletion += Cell_CellRequestsDeletion;
        cell.CellProposedForDeletion -= Cell_CellProposedForDeletion;
        cell.CellProposedForDeletion += Cell_CellProposedForDeletion;
    }

    private void Cell_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
    {
        if (cell == WorkshopCell)
        {
            response.RejectWithReason($"That room is a workshop room for shop #{Id:N0} ({Name.ColourName()})");
            return;
        }

        if (cell == StockroomCell)
        {
            response.RejectWithReason($"That room is a stockroom for shop #{Id:N0} ({Name.ColourName()})");
            return;
        }
    }

    private void Cell_CellRequestsDeletion(object sender, EventArgs e)
    {
        var cell = (ICell)sender;
        RemoveShopfrontCell(cell);
    }

    public void AddShopfrontCell(ICell cell)
    {
        if (!_shopfrontCells.Contains(cell))
        {
            AddCellToStore(cell);
            _shopfrontCells.Add(cell);
            Changed = true;
        }
    }

    public void RemoveShopfrontCell(ICell cell)
    {
        _shopfrontCells.Remove(cell);
        RemoveCellFromStore(cell);
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

    private readonly List<ICell> _shopfrontCells = new();
    public IEnumerable<ICell> ShopfrontCells => _shopfrontCells;
    private ICell _workshopCell;
    private ICell _stockroomCell;
    public ICell WorkshopCell
    {
        get => _workshopCell;
        set
        {
            if (_workshopCell != null && value != _workshopCell)
            {
                RemoveCellFromStore(_workshopCell);
            }

            _workshopCell = value;
            Changed = true;
        }
    }

    public ICell StockroomCell
    {
        get => _stockroomCell;
        set
        {
            if (_stockroomCell != null && value != _stockroomCell)
            {
                RemoveCellFromStore(_stockroomCell);
            }

            _stockroomCell = value;
            Changed = true;
        }
    }
    public IEnumerable<ICell> AllShopCells => ShopfrontCells.Concat(new[]
    {
        WorkshopCell,
        StockroomCell
    }.WhereNotNull(x => x));

    private readonly HashSet<long> _tillItemIds = new();

    public IEnumerable<IGameItem> TillItems =>
        AllShopCells.SelectMany(x => x.GameItems).Where(x => _tillItemIds.Contains(x.Id));

    private readonly HashSet<long> _displayContainerIds = new();

    public IEnumerable<IGameItem> DisplayContainers => AllShopCells.SelectMany(x => x.GameItems)
                                                                   .Where(x => _displayContainerIds.Contains(x.Id));

    public override IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise, List<(IGameItem Item, IGameItem Container)> purchasedItems = null)
    {
        var quantityToRestock = merchandise.MinimumStockLevels - _stockedMerchandiseCounts[merchandise];
        var originalQuantity = quantityToRestock;
        var newItems = new List<IGameItem>();
        if (merchandise.PreserveVariablesOnReorder && purchasedItems != null)
        {
            foreach (var item in purchasedItems)
            {
                var newItem = item.Item.DeepCopy(true);
                newItem.Skin = merchandise.Skin;
                newItems.Add(newItem);
                quantityToRestock -= newItem.Quantity;
            }
        }

        if (quantityToRestock > 0)
        {
            if (merchandise.Item.Components.Any(x => x is StackableGameItemComponentProto))
            {
                var newItem = merchandise.Item.CreateNew(null);
                newItem.Skin = merchandise.Skin;
                newItem.GetItemType<StackableGameItemComponent>().Quantity = quantityToRestock;
                newItems.Add(newItem);
                Gameworld.Add(newItem);
            }
            else
            {
                for (var i = 0; i < quantityToRestock; i++)
                {
                    var newItem = merchandise.Item.CreateNew(null);
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

        var targetCell = StockroomCell;
        if (targetCell == null)
        {
            targetCell = ShopfrontCells.First();
        }

        foreach (var item in newItems)
        {
            item.AddEffect(new ItemOnDisplayInShop(item, this, merchandise));
            targetCell.Insert(item);
            item.HandleEvent(EventType.ItemFinishedLoading, item);
            item.Login();
            _stockedMerchandise.Add(merchandise, item.Id);
            _stockedMerchandiseCounts[merchandise] += item.Quantity;
        }

        AddTransaction(new TransactionRecord(ShopTransactionType.Restock, Currency, this,
            ShopfrontCells.First().DateTime(), null, merchandise.EffectiveAutoReorderPrice * originalQuantity, 0.0M));
        return newItems;
    }

    public override IEnumerable<IGameItem> StockedItems(IMerchandise merchandise)
    {
        return
            ShopfrontCells.SelectMany(x => x.Characters).SelectMany(x => x.Body.HeldItems)
                          .Concat(
                              ShopfrontCells
                                  .ConcatIfNotNull(StockroomCell)
                                  .SelectMany(x => x.GameItems)
                                  .SelectMany(x => x.ShallowItems)
                          )
                          .Where(x => x.AffectedBy<ItemOnDisplayInShop>(merchandise));
    }
    public override void CheckFloat()
    {
        var cashInRegister = TillItems
                             .RecursiveGetItems<ICurrencyPile>(false)
                             .Where(x => x.Currency == Currency)
                             .Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value));
        if (cashInRegister > CashBalance)
        {
            AddTransaction(new TransactionRecord(ShopTransactionType.Float, Currency, this,
                AllShopCells.First().DateTime(), null, cashInRegister - CashBalance, 0.0M));
            CashBalance = cashInRegister;
            Changed = true;
            return;
        }

        if (cashInRegister < CashBalance)
        {
            AddTransaction(new TransactionRecord(ShopTransactionType.Withdrawal, Currency, this,
                AllShopCells.First().DateTime(), null, CashBalance - cashInRegister, 0.0M));
            CashBalance = cashInRegister;
            Changed = true;
            return;
        }
    }
    protected override void ShowInfo(ICharacter actor, StringBuilder sb)
    {
        if (IsEmployee(actor) || actor.IsAdministrator())
        {
            if (actor.Location == WorkshopCell)
            {
                sb.AppendLine("The location you are currently in is the workshop for this store.");
            }
            else if (actor.Location == StockroomCell)
            {
                sb.AppendLine("The location you are currently in is the stockroom for this store.");
            }
            else
            {
                sb.AppendLine("The location you are currently in is the shopfront for this store.");
            }

            foreach (var item in TillItems)
            {
                sb.AppendLine($"{item.HowSeen(actor, true)} is a till for this store.");
            }

            foreach (var item in DisplayContainers)
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
        var floorStock =
            ShopfrontCells.SelectMany(x => x.Characters).SelectMany(x => x.Body.HeldItems)
                          .Concat(
                              ShopfrontCells
                                  .SelectMany(x => x.GameItems)
                                  .SelectMany(x => x.ShallowItems)
                          )
                          .Where(x => x.AffectedBy<ItemOnDisplayInShop>(whichMerchandise))
                          .Sum(x => x.Quantity);
        return (floorStock, _stockedMerchandiseCounts[whichMerchandise] - floorStock);
    }

    protected override (bool Truth, string Reason) CanBuyInternal(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method, string extraArguments = null)
    {
        if (method is CashPayment)
        {
            if (!TillItems.Any())
            {
                return (false, "the store is currently missing its till, and so cannot do cash transactions.");
            }
        }

        return (true, string.Empty);
    }

    public override IFutureProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "shopfront":
                return new CollectionVariable(ShopfrontCells.ToList(), FutureProgVariableTypes.Location);
            case "storeroom":
                return StockroomCell;
            case "workshop":
                return WorkshopCell;
            case "tills":
                return new CollectionVariable(TillItems.ToList(), FutureProgVariableTypes.Item);
            default:
                return base.GetProperty(property);
        }
    }

    public override IEnumerable<ICell> CurrentLocations => ShopfrontCells;
}
