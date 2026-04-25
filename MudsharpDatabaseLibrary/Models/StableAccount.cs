using System.Collections.Generic;

namespace MudSharp.Models;

public class StableAccount
{
	public StableAccount()
	{
		AccountUsers = new HashSet<StableAccountUser>();
	}

	public long Id { get; set; }
	public long StableId { get; set; }
	public string AccountName { get; set; }
	public long AccountOwnerId { get; set; }
	public string AccountOwnerName { get; set; }
	public decimal Balance { get; set; }
	public decimal CreditLimit { get; set; }
	public bool IsSuspended { get; set; }

	public virtual Stable Stable { get; set; }
	public virtual Character AccountOwner { get; set; }
	public virtual ICollection<StableAccountUser> AccountUsers { get; set; }
}
