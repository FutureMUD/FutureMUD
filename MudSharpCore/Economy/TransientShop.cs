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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy;

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
        throw new NotImplementedException();
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

    public override IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise, List<(IGameItem Item, IGameItem Container)> purchasedItems = null)
    {
		if (CurrentStall is null)
		{
			return Enumerable.Empty<IGameItem>();
		}

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

		foreach (var item in newItems)
		{
			item.AddEffect(new ItemOnDisplayInShop(item, this, merchandise));
			CurrentStall.Put(null, item, false);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			item.Login();
			_stockedMerchandise.Add(merchandise, item.Id);
			_stockedMerchandiseCounts[merchandise] += item.Quantity;
		}

		AddTransaction(new TransactionRecord(ShopTransactionType.Restock, Currency, this,
			EconomicZone.ZoneForTimePurposes.DateTime(), null, merchandise.EffectiveAutoReorderPrice * originalQuantity, 0.0M));
		return newItems;
	}

    public override IEnumerable<IGameItem> StockedItems(IMerchandise merchandise)
    {
		if (CurrentStall is null)
		{
			return Enumerable.Empty<IGameItem>();
		}

		return CurrentStall.Contents.Where(x => x.AffectedBy<ItemOnDisplayInShop>(merchandise));
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

    public override void CheckFloat()
    {
		var cashInRegister = CurrentStall.Contents
			.RecursiveGetItems<ICurrencyPile>(false)
			.Where(x => x.Currency == Currency)
			.Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value));
		if (cashInRegister > CashBalance)
		{
			AddTransaction(new TransactionRecord(ShopTransactionType.Float, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), null, cashInRegister - CashBalance, 0.0M));
			CashBalance = cashInRegister;
			Changed = true;
			return;
		}

		if (cashInRegister < CashBalance)
		{
			AddTransaction(new TransactionRecord(ShopTransactionType.Withdrawal, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), null, CashBalance - cashInRegister, 0.0M));
			CashBalance = cashInRegister;
			Changed = true;
			return;
		}
	}

    public IShopStall CurrentStall { get; set; }
}
