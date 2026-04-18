#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class ComputerMailAccount
{
	public ComputerMailAccount()
	{
		MailboxEntries = new HashSet<ComputerMailMailboxEntry>();
	}

	public long Id { get; set; }
	public long ComputerMailDomainId { get; set; }
	public string UserName { get; set; } = null!;
	public string PasswordHash { get; set; } = null!;
	public long PasswordSalt { get; set; }
	public bool IsEnabled { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime LastModifiedAtUtc { get; set; }

	public virtual ComputerMailDomain ComputerMailDomain { get; set; } = null!;
	public virtual ICollection<ComputerMailMailboxEntry> MailboxEntries { get; set; }
}
