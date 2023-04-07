#nullable enable
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy;

public interface IBankAccount : IFrameworkItem, ISaveable, IFutureProgVariable
{
	IBank Bank { get; }
	int AccountNumber { get; }
	/// <summary>
	/// Returns a friendly roundtripaple reference to the bank account in the form BANKCODE:ACCOUNTNUMBER
	/// </summary>
	string AccountReference { get; }
	IBankAccountType BankAccountType { get; }
	ICurrency Currency { get; }
	decimal CurrentBalance { get; }
	decimal CurrentMonthInterest { get; }
	decimal CurrentMonthFees { get; }

	bool IsAccountOwner(ICharacter character);
	bool IsAccountOwner(IClan clan);
	bool IsAccountOwner(IShop shop);
	bool IsAuthorisedAccountUser(ICharacter character);
	IFrameworkItem AccountOwner { get; }

	bool IsAuthorisedPaymentItem(IBankPaymentItem item);
	void CancelPaymentItems();
	void CancelExistingPaymentItem(IBankPaymentItem item);
	IGameItem? CreateNewPaymentItem();
	int NumberOfIssuedPaymentItems { get; }

	MudDateTime AccountCreationDate { get; }
	BankAccountStatus AccountStatus { get; }
	IEnumerable<IBankAccountTransaction> Transactions { get; }

	IBankAccount NominatedBenefactor { get; }

	decimal MaximumWithdrawal();
	(bool Truth, string Error) CanWithdraw(decimal amount, bool ignoreCurrencyReserves);
	void Withdraw(decimal amount);
	void WithdrawFromTransaction(decimal amount, string transactionReference);
	void WithdrawFromTransfer(decimal amount, string toBankCode, int toAccount, string transferReference);
	void Deposit(decimal amount);
	void DepositFromTransaction(decimal amount, string transactionReference);
	void DepositFromTransfer(decimal amount, string fromBankCode, int fromAccount, string transferReference);
	void DoAccountCredit(decimal amount, string reason);
	void ChargeFee(decimal amount, BankTransactionType transactionType, string feeDescription);
	void DepositInterest(decimal amount);
	void DoDailyFee(decimal amount);
	(bool Truth, string Reason) CloseAccount(ICharacter actor);
	string Show(ICharacter actor);
	string ShowTransactions(ICharacter actor);
	void SetStatus(BankAccountStatus status);
	void AccountRolledOver();
	void FinaliseMonth();
}