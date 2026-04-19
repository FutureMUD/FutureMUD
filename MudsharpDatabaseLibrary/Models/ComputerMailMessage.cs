#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class ComputerMailMessage
{
	public ComputerMailMessage()
	{
		MailboxEntries = new HashSet<ComputerMailMailboxEntry>();
	}

	public long Id { get; set; }
	public string SenderAddress { get; set; } = null!;
	public string RecipientAddress { get; set; } = null!;
	public string Subject { get; set; } = null!;
	public string Body { get; set; } = null!;
	public DateTime SentAtUtc { get; set; }

	public virtual ICollection<ComputerMailMailboxEntry> MailboxEntries { get; set; }
}
