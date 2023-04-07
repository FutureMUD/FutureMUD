using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Banking;

public class BankAccountTransaction : LateInitialisingItem, IBankAccountTransaction
{
	public BankAccountTransaction(Models.BankAccountTransaction transaction, IBankAccount account)
	{
		Gameworld = account.Gameworld;
		Id = transaction.Id;
		IdInitialised = true;
		Account = account;
		TransactionType = (BankTransactionType)transaction.TransactionType;
		TransactionDescription = transaction.TransactionDescription;
		Amount = transaction.Amount;
		AccountBalanceAfter = transaction.AccountBalanceAfter;
		TransactionTime = new MudDateTime(transaction.TransactionTime, Gameworld);
	}

	public BankAccountTransaction(IBankAccount account, BankTransactionType type, decimal amount, decimal amountAfter,
		MudDateTime transactionTime, string description)
	{
		Gameworld = account.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		Account = account;
		TransactionType = type;
		Amount = amount;
		AccountBalanceAfter = amountAfter;
		TransactionTime = transactionTime;
		TransactionDescription = description;
	}

	public override string FrameworkItemType => "BankTransaction";

	public override void Save()
	{
		// Bank Transactions shouldn't change after being initialised
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.BankAccountTransaction
		{
			BankAccountId = Account.Id,
			TransactionType = (int)TransactionType,
			TransactionDescription = TransactionDescription,
			TransactionTime = TransactionTime.GetDateTimeString(),
			Amount = Amount,
			AccountBalanceAfter = AccountBalanceAfter
		};
		FMDB.Context.BankAccountTransactions.Add(dbitem);
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((Models.BankAccountTransaction)dbitem).Id;
	}

	public IBankAccount Account { get; set; }
	public BankTransactionType TransactionType { get; set; }
	public decimal Amount { get; set; }
	public decimal AccountBalanceAfter { get; set; }
	public MudDateTime TransactionTime { get; set; }
	public string TransactionDescription { get; set; }
}