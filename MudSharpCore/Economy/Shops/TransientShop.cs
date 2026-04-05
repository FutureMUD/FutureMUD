using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy.Shops;

public class TransientShop : Shop, ITransientShop
{
    public TransientShop(IEconomicZone zone, string name) : base(zone, null, name, "Transient")
    {
        InitialiseShop();
    }

    public TransientShop(Models.Shop shop, IFuturemud gameworld) : base(shop, gameworld)
    {
        InitialiseShop();
    }

    protected override void Save(Models.Shop dbitem)
    {
        // Do nothing
    }

    public override IEnumerable<ICell> CurrentLocations
    {
        get
        {
            if (CurrentStall is null)
            {
                return Enumerable.Empty<ICell>();
            }

            return new List<ICell> { CurrentStall.Parent.TrueLocations.First() };
        }
    }

    protected override (bool Truth, string Reason) CanBuyInternal(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method, string extraArguments = null)
    {
        return (true, string.Empty);
    }

    public override (bool Truth, string Reason) CanSellInternal(ICharacter actor, IMerchandise merchandise, IPaymentMethod method,
        IGameItem item)
    {
        if (CurrentStall is null)
        {
            return (false, "the shop has not been set up to have a stall.");
        }

        if (!CurrentStall.CanPut(item))
        {
            return (false, "the stall is too full to fit that item.");
        }

        return (true, string.Empty);
    }

    public override IEnumerable<IGameItem> DoAutoRestockForMerchandise(IMerchandise merchandise, List<(IGameItem Item, IGameItem Container)> purchasedItems = null)
    {
        if (CurrentStall is null)
        {
            return Enumerable.Empty<IGameItem>();
        }

        int quantityToRestock = merchandise.MinimumStockLevels - _stockedMerchandiseCounts[merchandise];
        int originalQuantity = quantityToRestock;
        List<IGameItem> newItems = new();
        if (merchandise.PreserveVariablesOnReorder && purchasedItems != null)
        {
            foreach ((IGameItem Item, IGameItem Container) item in purchasedItems)
            {
                IGameItem newItem = item.Item.DeepCopy(true, false);
                newItem.Skin = merchandise.Skin;
                newItems.Add(newItem);
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
            item.AddEffect(new ItemOnDisplayInShop(item, this, merchandise));
            SortItemToStorePhysicalLocation(item, merchandise, null);
            item.HandleEvent(EventType.ItemFinishedLoading, item);
            item.Login();
            _stockedMerchandise.Add(merchandise, item.Id);
            _stockedMerchandiseCounts[merchandise] += item.Quantity;
        }

        AddTransaction(new TransactionRecord(ShopTransactionType.Restock, Currency, this,
            EconomicZone.ZoneForTimePurposes.DateTime(), null, merchandise.EffectiveAutoReorderPrice * originalQuantity, 0.0M, merchandise));
        return newItems;
    }

    /// <inheritdoc />
    public override void SortItemToStorePhysicalLocation(IGameItem item, IMerchandise merchandise, IGameItem container)
    {
        CurrentStall?.Put(null, item, false);
    }

    #region Overrides of Shop

    /// <inheritdoc />
    public override IEnumerable<IGameItem> DoAutostockAllMerchandise()
    {
        List<IGameItem> stocked = new();
        if (CurrentStall is null)
        {
            return stocked;
        }

        List<IGameItem> items = CurrentStall.Contents.SelectMany(x => x.DeepItems).ToList();
        foreach (IGameItem item in items)
        {
            if (item.AffectedBy<ItemOnDisplayInShop>(this))
            {
                continue;
            }

            IMerchandise merch = Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item)) ??
                        _merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item, true));
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
        if (CurrentStall is null)
        {
            return stocked;
        }

        foreach (IGameItem item in CurrentStall.Contents.SelectMany(x => x.DeepItems).ToList())
        {
            if (item.AffectedBy<ItemOnDisplayInShop>(this))
            {
                continue;
            }

            IMerchandise merch = Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item)) ??
                        _merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item, true));
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
        if (CurrentStall is null)
        {
            return Enumerable.Empty<IGameItem>();
        }

        return CurrentStall.Contents.SelectMany(x => x.DeepItems).Where(x => x.AffectedBy<ItemOnDisplayInShop>(merchandise)).Distinct();
    }

    public override IEnumerable<IGameItem> AllStockedItems
    {
        get
        {
            if (CurrentStall is null)
            {
                return Enumerable.Empty<IGameItem>();
            }

            return CurrentStall.Contents.SelectMany(x => x.DeepItems).Where(x => x.AffectedBy<ItemOnDisplayInShop>()).Distinct();
        }
    }


    protected override void ShowInfo(ICharacter actor, StringBuilder sb)
    {
        // Do nothing
    }

    public override (int OnFloorCount, int InStockroomCount) StocktakeMerchandise(IMerchandise whichMerchandise)
    {
        if (!_merchandises.Contains(whichMerchandise))
        {
            return (0, 0);
        }

        RecalculateStockedItems(whichMerchandise, 0);
        return (0, _stockedMerchandiseCounts[whichMerchandise]);
    }

    public override bool IsReadyToDoBusiness => CurrentStall is not null;

    public override void CheckFloat()
    {
        if (CurrentStall is null)
        {
            return;
        }

        decimal cashInRegister = CurrentStall.Contents
            .RecursiveGetItems<ICurrencyPile>(false)
            .Where(x => x.Currency == Currency)
            .Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value));
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

    public IShopStall CurrentStall { get; set; }

    public override IReadOnlyDictionary<ICurrencyPile, Dictionary<ICoin, int>> GetCurrencyForShop(decimal amount)
    {
        return Currency.FindCurrency(CurrentStall?.Contents.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()).ToList() ?? Enumerable.Empty<ICurrencyPile>(), amount);
    }

    public override IEnumerable<ICurrencyPile> GetCurrencyPilesForShop()
    {
        List<ICurrencyPile> piles = new();
        if (CurrentStall is not null)
        {
            piles.AddRange(CurrentStall.Contents.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()));
        }
        return piles;
    }

    public override void AddCurrencyToShop(IGameItem currencyPile)
    {
        if (CurrentStall is null)
        {
            return;
        }

        CurrentStall.Put(null, currencyPile, true);
    }
}
