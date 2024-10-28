using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Date;

#nullable enable
namespace MudSharp.Economy
{
	public enum BankAccountStatus
	{
		Active,
		Locked,
		Suspended,
		Closed
	}

	public enum BankTransactionType
	{
		Withdrawal,
		Deposit,
		DepositFromTransfer,
		Transfer,
		InterBankTransfer,
		CurrencyConversion,
		InterestCharge,
		InterestEarned,
		ServiceFee,
		OverdraftFee,
		LoanRepayment,
		OverdueFee,
		AccountCredit,
		DepositFromTransaction,
		WithdrawalFromTransaction,
		WithdrawalFromTransfer,
	}

	public interface IBank : IFrameworkItem, ISaveable, IEditableItem, IProgVariable
	{
		string Code { get; }
		IEconomicZone EconomicZone { get; }
		IEnumerable<IBankAccount> BankAccounts { get; }
		ICurrency PrimaryCurrency { get; }
		DecimalCounter<ICurrency> CurrencyReserves { get; }
		DecimalCounter<(ICurrency From, ICurrency To)> ExchangeRates { get; }
		IEnumerable<IBankAccountType> BankAccountTypes { get; }
		IEnumerable<ICell> BranchLocations { get; }
		void AddAccount(IBankAccount newAccount);
		(bool Truth, string Error) CanOpenAccount(ICharacter actor, IBankAccountType type);
		IEnumerable<ICharacter> BankManagers { get; }
		void AddManager(ICharacter manager);
		void RemoveManager(ICharacter manager);
		bool IsManager(ICharacter actor);
		IEnumerable<IBankManagerAuditLog> AuditLogs { get; }
		void ManagerCommand(ICharacter manager, StringStack command);
		void ReferenceDateOnDateChanged();
	}
}
