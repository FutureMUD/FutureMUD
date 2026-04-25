namespace MudSharp.Models;

public class StableAccountUser
{
	public long StableAccountId { get; set; }
	public long AccountUserId { get; set; }
	public string AccountUserName { get; set; }
	public decimal? SpendingLimit { get; set; }

	public virtual StableAccount StableAccount { get; set; }
	public virtual Character AccountUser { get; set; }
}
