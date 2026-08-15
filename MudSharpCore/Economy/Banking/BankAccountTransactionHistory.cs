using Microsoft.EntityFrameworkCore;

namespace MudSharp.Economy.Banking;

public static class BankAccountTransactionHistory
{
	public const int MaximumTransactionHistoryEntries = 100;

	public static List<Models.BankAccountTransaction> LoadMostRecent(
		IQueryable<Models.BankAccountTransaction> transactions, long bankAccountId)
	{
		return transactions
			.AsNoTracking()
			.Where(x => x.BankAccountId == bankAccountId)
			.OrderByDescending(x => x.Id)
			.Take(MaximumTransactionHistoryEntries)
			.ToList();
	}

	public static List<Models.BankAccountTransaction> LoadOlderThan(
		IQueryable<Models.BankAccountTransaction> transactions, long bankAccountId, long transactionId)
	{
		return transactions
			.AsNoTracking()
			.Where(x => x.BankAccountId == bankAccountId && x.Id < transactionId)
			.OrderByDescending(x => x.Id)
			.Take(MaximumTransactionHistoryEntries)
			.ToList();
	}
}
