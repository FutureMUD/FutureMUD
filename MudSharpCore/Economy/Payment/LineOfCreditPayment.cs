using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Economy.Currency;

namespace MudSharp.Economy.Payment;

public class LineOfCreditPayment : IPaymentMethod
{
	public LineOfCreditPayment(ICharacter actor, ILineOfCreditAccount account)
	{
		Actor = actor;
		Account = account;
	}

	public ICharacter Actor { get; set; }
	public ILineOfCreditAccount Account { get; set; }

	public decimal AccessibleMoneyForPayment()
	{
		if (Account.IsAuthorisedToUse(Actor, 0) == LineOfCreditAuthorisationFailureReason.None)
		{
			return Account.MaximumAuthorisedToUse(Actor);
		}

		return 0.0M;
	}

	public ICurrency Currency => Account.Currency;

	public void TakePayment(decimal price)
	{
		Account.ChargeAccount(price);
	}

	/// <inheritdoc />
	public decimal AccessibleMoneyForCredit()
	{
		return decimal.MaxValue;
	}

	/// <inheritdoc />
	public void GivePayment(decimal price)
	{
		Account.PayoffAccount(price);
	}
}