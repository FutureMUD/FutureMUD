using System;

namespace MudSharp.Models;

public partial class LendingPosting
{
	public long Id { get; set; }
	public long ContractId { get; set; }
	public long? ActorId { get; set; }
	public string OperationKey { get; set; }
	public string Kind { get; set; }
	public string MudDate { get; set; }
	public string Detail { get; set; }
	public DateTime RealDate { get; set; }
	public decimal Principal { get; set; }
	public decimal Interest { get; set; }
	public decimal Amount { get; set; }
}
