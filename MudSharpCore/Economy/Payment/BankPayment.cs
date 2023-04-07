using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Economy.Payment;

public class BankPayment : IPaymentMethod
{
	public ICharacter Actor { get; }
	public IBankPaymentItem Item { get; }
	public IShop Shop { get; }

	public BankPayment(ICharacter actor, IBankPaymentItem item, IShop shop)
	{
		Actor = actor;
		Item = item;
		Shop = shop;
	}

	#region Implementation of IPaymentMethod

	/// <inheritdoc />
	public decimal AccessibleMoneyForPayment()
	{
		if (!Item.BankAccount.IsAuthorisedPaymentItem(Item))
		{
			return 0.0M;
		}

		return Item.BankAccount.MaximumWithdrawal();
	}

	/// <inheritdoc />
	public ICurrency Currency => Item.BankAccount.Currency;

	/// <inheritdoc />
	public void TakePayment(decimal price)
	{
		Item.BankAccount.WithdrawFromTransaction(price, $"Purchase at {Shop.Name}");
		Shop.BankAccount!.DepositFromTransaction(price, "Point of Sale");
		if (Item.CurrentUsesRemaining > 0)
		{
			Item.CurrentUsesRemaining--;
		}
	}

	#endregion
}