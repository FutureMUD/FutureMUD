using System;

namespace MudSharp.Models;

public class VirtualCashLedgerEntry
{
	public long Id { get; set; }
	public string OwnerType { get; set; }
	public long OwnerId { get; set; }
	public long CurrencyId { get; set; }
	public DateTime RealDateTime { get; set; }
	public string MudDateTime { get; set; }
	public long? ActorId { get; set; }
	public string ActorName { get; set; }
	public long? CounterpartyId { get; set; }
	public string CounterpartyType { get; set; }
	public string CounterpartyName { get; set; }
	public decimal Amount { get; set; }
	public decimal BalanceAfter { get; set; }
	public string SourceKind { get; set; }
	public string DestinationKind { get; set; }
	public long? LinkedBankAccountId { get; set; }
	public string ReferenceType { get; set; }
	public long? ReferenceId { get; set; }
	public string Reference { get; set; }
	public string Reason { get; set; }

	public virtual Currency Currency { get; set; }
	public virtual BankAccount LinkedBankAccount { get; set; }
}
