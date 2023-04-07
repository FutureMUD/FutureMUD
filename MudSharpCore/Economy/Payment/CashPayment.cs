using System.Linq;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Economy.Payment;

public abstract class CashPayment : IPaymentMethod
{
	protected CashPayment(ICurrency currency, ICharacter actor)
	{
		Currency = currency;
		Actor = actor;
	}

	public ICurrency Currency { get; set; }
	public ICharacter Actor { get; set; }

	protected static decimal CountAccessibleMoney(IGameItem item, ICurrency whichCurrency, bool respectGetRules)
	{
		var currency = item.GetItemType<ICurrencyPile>();
		if (currency != null)
		{
			if (currency.Currency == whichCurrency)
			{
				return currency.Coins.Sum(x => x.Item2 * x.Item1.Value);
			}
		}

		if (respectGetRules && item.IsItemType<IOpenable>() && !item.GetItemType<IOpenable>().IsOpen)
		{
			return 0.0M;
		}

		var container = item.GetItemType<IContainer>();
		return container?.Contents.Sum(contained => CountAccessibleMoney(contained, whichCurrency, respectGetRules)) ??
		       0.0M;
	}

	public decimal AccessibleMoneyForPayment()
	{
		return Actor.Body.ExternalItems.Sum(x => CountAccessibleMoney(x, Currency, true));
	}

	public abstract void TakePayment(decimal price);
}