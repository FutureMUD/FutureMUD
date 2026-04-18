#nullable enable

using System;

namespace MudSharp.Models;

public partial class ComputerMailMailboxEntry
{
	public long Id { get; set; }
	public long ComputerMailAccountId { get; set; }
	public long ComputerMailMessageId { get; set; }
	public bool IsSentFolder { get; set; }
	public bool IsRead { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime DeliveredAtUtc { get; set; }

	public virtual ComputerMailAccount ComputerMailAccount { get; set; } = null!;
	public virtual ComputerMailMessage ComputerMailMessage { get; set; } = null!;
}
