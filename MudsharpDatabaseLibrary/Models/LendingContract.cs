namespace MudSharp.Models;

public partial class LendingContract
{
	public long Id { get; set; }
	public long ProductId { get; set; }
	public long LenderId { get; set; }
	public string LenderType { get; set; }
	public long BorrowerId { get; set; }
	public long EconomicZoneId { get; set; }
	public long CurrencyId { get; set; }
	public string Status { get; set; }
	public string Definition { get; set; }
	public string SecurityKey { get; set; }
	public long? EstateId { get; set; }
	public decimal Principal { get; set; }
	public decimal Interest { get; set; }
	public decimal Surplus { get; set; }
}
