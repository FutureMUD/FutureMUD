namespace MudSharp.Models;

public class VirtualCashBalance
{
	public long Id { get; set; }
	public string OwnerType { get; set; }
	public long OwnerId { get; set; }
	public long CurrencyId { get; set; }
	public decimal Balance { get; set; }

	public virtual Currency Currency { get; set; }
}
