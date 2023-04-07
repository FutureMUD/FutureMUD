using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Economy.Payment;

public class OtherCashPayment : CashPayment
{
	public OtherCashPayment(ICurrency currency, ICharacter actor) : base(currency, actor)
	{
	}

	#region Overrides of CashPayment

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

		if (change <= 0.0M)
		{
			return;
		}

		var changeItem =
			GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency,
				Currency.FindCoinsForAmount(change, out _));
		foreach (var item in containers)
		{
			var container = item.GetItemType<IContainer>();
			if (container.CanPut(changeItem))
			{
				container.Put(null, changeItem);
				break;
			}
		}

		if (!changeItem.Deleted && changeItem.ContainedIn == null)
		{
			if (Actor.Body.CanGet(changeItem, 0))
			{
				Actor.Body.Get(changeItem, silent: true);
			}
			else
			{
				Actor.Location.Insert(changeItem, true);
				Actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
			}
		}
	}

	#endregion
}