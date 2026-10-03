#nullable enable

namespace MudSharp.Economy;

/// <summary>
/// Optional capability for withdrawals whose complete descriptions are validated before funds are debited.
/// </summary>
public interface IPreparedBankAccountWithdrawal
{
	/// <summary>Stores the supplied description without reevaluating currency formatting progs.</summary>
	void WithdrawFromTransaction(decimal amount, string transactionReference, string transactionDescription);
}
