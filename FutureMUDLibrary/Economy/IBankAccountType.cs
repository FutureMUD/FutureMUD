#nullable enable
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Economy;

public interface IBankAccountType : IFrameworkItem, IEditableItem, ISaveable, IProgVariable
{
	(bool Truth, string Reason) CanOpenAccount(ICharacter actor);
	(bool Truth, string Reason) CanOpenAccount(IClan clan);
	(bool Truth, string Reason) CanOpenAccount(IShop shop);
	(bool Truth, string Reason) CanCloseAccount(ICharacter actor, IBankAccount account);
	IBankAccount OpenAccount(ICharacter actor);
	IBankAccount OpenAccount(IClan clan);
	IBankAccount OpenAccount(IShop shop);
	IBank Bank { get; }
	string ShowToCustomer(ICharacter actor);
	string CustomerDescription { get; }
	void DoTransactionFeesWithdrawal(IBankAccount account, decimal amount);
	void DoOverdrawFees(IBankAccount account, decimal amount);
	void DoTransactionFeesDeposit(IBankAccount account, decimal amount);
	void DoTransactionFeesDepositFromTransfer(IBankAccount account, decimal amount);
	void DoTransactionFeesTransfer(IBankAccount account, decimal amount);
	void DoTransactionFeesTransferOtherBank(IBankAccount account, decimal amount);
	void DoDailyAccountFees(IBankAccount account);
	decimal MaximumOverdrawAmount { get; }

	int NumberOfPermittedPaymentItems { get; }
	long? PaymentItemPrototype { get; }
}