#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public class StableStay
{
	public StableStay()
	{
		LedgerEntries = new HashSet<StableStayLedgerEntry>();
	}

	public long Id { get; set; }
	public long StableId { get; set; }
	public long MountId { get; set; }
	public long? MountInstanceId { get; set; }
	public long? OriginalOwnerId { get; set; }
	public string? OriginalOwnerName { get; set; }
	public string LodgedDateTime { get; set; } = null!;
	public string LastDailyFeeDateTime { get; set; } = null!;
	public string? ClosedDateTime { get; set; }
	public int Status { get; set; }
	public long? TicketItemId { get; set; }
	public string TicketToken { get; set; } = null!;
	public decimal AmountOwing { get; set; }

	public virtual Stable Stable { get; set; } = null!;
	public virtual Character Mount { get; set; } = null!;
	public virtual Character? OriginalOwner { get; set; }
	public virtual GameItem? TicketItem { get; set; }
	public virtual ICollection<StableStayLedgerEntry> LedgerEntries { get; set; } = null!;
}
