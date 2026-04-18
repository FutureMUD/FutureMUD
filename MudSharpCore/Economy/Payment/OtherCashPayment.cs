using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Economy.Payment;

public class OtherCashPayment : CashPayment
{
    public OtherCashPayment(ICurrency currency, ICharacter actor) : base(currency, actor)
    {
    }

    #region Overrides of CashPayment

    public override void TakePayment(decimal price)
    {
        IEnumerable<ICurrencyPile> currency = Actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true);
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> targetCoins = Currency.FindCurrency(currency, price);
        decimal value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
        IEnumerable<IGameItem> containers = targetCoins.SelectNotNull(x => x.Key.Parent.ContainedIn).Distinct();
        decimal change = value - price;
        foreach (KeyValuePair<ICurrencyPile, Dictionary<ICoin, int>> item in targetCoins.Where(item =>
                     !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
        {
            item.Key.Parent.Delete();
        }

        Counter<ICoin> counter = new();
        foreach (KeyValuePair<ICurrencyPile, Dictionary<ICoin, int>> item in targetCoins)
        {
            foreach (KeyValuePair<ICoin, int> coin in item.Value)
            {
                counter[coin.Key] += coin.Value;
            }
        }

        if (change <= 0.0M)
        {
            return;
        }

        IGameItem changeItem =
            GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency,
                Currency.FindCoinsForAmount(change, out _));
        foreach (IGameItem item in containers)
        {
            IContainer container = item.GetItemType<IContainer>();
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

    /// <inheritdoc />
    public override decimal AccessibleMoneyForCredit()
    {
        return 0.0M;
    }

    /// <inheritdoc />
    public override void GivePayment(decimal price)
    {
        // Do nothing
    }

    #endregion
}