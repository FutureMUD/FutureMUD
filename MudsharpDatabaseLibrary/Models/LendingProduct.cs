namespace MudSharp.Models;

public partial class LendingProduct
{
	public long Id { get; set; }
	public long LenderId { get; set; }
	public string LenderType { get; set; }
	public long EconomicZoneId { get; set; }
	public string Name { get; set; }
	public string Definition { get; set; }
	public bool Enabled { get; set; }
}
