﻿using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy.Currency;

namespace MudSharp.Economy.Payment;

/// <summary>
/// A Cash payment type designed to be used with a shop
/// </summary>
public class ShopCashPayment : CashPayment
{
	public ShopCashPayment(ICurrency currency, IShop shop, ICharacter actor) : base(currency, actor)
	{
		Shop = shop;
	}

	public IShop Shop { get; set; }

	public override void TakePayment(decimal price)
	{
		var currency = Actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true);
		var targetCoins = Currency.FindCurrency(currency, price);
		var value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
		var containers = targetCoins.SelectNotNull(x => x.Key.Parent.ContainedIn).Distinct();
		var change = value - price;
		foreach (var item in targetCoins.Where(item =>
					 !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		var counter = new Counter<ICoin>();
		foreach (var item in targetCoins)
			foreach (var coin in item.Value)
			{
				counter[coin.Key] += coin.Value;
			}

		var newPile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency, counter);
		Shop.AddCurrencyToShop(newPile);

		if (change <= 0.0M)
		{
			return;
		}


		// Now figure out any change
		var piles = Shop.GetCurrencyPilesForShop();
		var changeCoins = Currency.FindCurrency(piles, change);
		var changeCoinsValue = changeCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
		if (changeCoinsValue < change)
		{
			return;
		}

		foreach (var item in changeCoins.Where(item =>
					 !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		var changePile =
			CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency,
				Currency.FindCoinsForAmount(change, out _));
		foreach (var item in containers)
		{
			var container = item.GetItemType<IContainer>();
			if (container.CanPut(changePile))
			{
				container.Put(null, changePile);
				break;
			}
		}

		if (!changePile.Deleted && changePile.ContainedIn == null)
		{
			if (Actor.Body.CanGet(changePile, 0))
			{
				Actor.Body.Get(changePile);
			}
			else
			{
				Actor.Location.Insert(changePile);
			}
		}

		if (changeCoinsValue > change)
		{
			Shop.AddCurrencyToShop(CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency,
					Currency.FindCoinsForAmount(changeCoinsValue - change, out _)));
		}
	}

	/// <inheritdoc />
	public override void GivePayment(decimal price)
	{
		var currency = Shop.GetCurrencyPilesForShop();
		var targetCoins = Currency.FindCurrency(currency, price);
		var value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
		var containers = targetCoins.SelectNotNull(x => x.Key.Parent.ContainedIn).Distinct();
		var change = value - price;
		foreach (var item in targetCoins.Where(item =>
					 !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		var counter = new Counter<ICoin>();
		foreach (var item in targetCoins)
			foreach (var coin in item.Value)
			{
				counter[coin.Key] += coin.Value;
			}

		var newPile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency, counter);
		if (Actor.Body.CanGet(newPile, 0))
		{
			Actor.Body.Get(newPile);
		}
		else
		{
			Actor.Location.Insert(newPile);
		}

		if (change <= 0.0M)
		{
			return;
		}


		// Now figure out any change
		var piles = Actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true);
		var changeCoins = Currency.FindCurrency(piles, change);
		var changeCoinsValue = changeCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
		if (changeCoinsValue < change)
		{
			return;
		}

		foreach (var item in changeCoins.Where(item =>
					 !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		var changePile =
			CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency,
				Currency.FindCoinsForAmount(change, out _));
		foreach (var item in containers)
		{
			var container = item.GetItemType<IContainer>();
			if (container.CanPut(changePile))
			{
				container.Put(null, changePile);
				break;
			}
		}

		if (!changePile.Deleted && changePile.ContainedIn == null)
		{
			Shop.AddCurrencyToShop(changePile);
		}

		if (changeCoinsValue > change)
		{
			var excessPile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency, Currency.FindCoinsForAmount(changeCoinsValue - change, out _));
			if (Actor.Body.CanGet(excessPile, 0))
			{
				Actor.Body.Get(excessPile, silent: true);
			}
			else
			{
				Actor.Location.Insert(excessPile);
			}
		}
	}

	/// <inheritdoc />
	public override decimal AccessibleMoneyForCredit()
	{
		return Shop.GetCurrencyPilesForShop().Sum(x => x.TotalValue);
	}
}