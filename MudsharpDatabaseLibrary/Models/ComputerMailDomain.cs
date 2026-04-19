#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class ComputerMailDomain
{
	public ComputerMailDomain()
	{
		Accounts = new HashSet<ComputerMailAccount>();
	}

	public long Id { get; set; }
	public string DomainName { get; set; } = null!;
	public long HostItemId { get; set; }
	public bool Enabled { get; set; }
	public DateTime CreatedAtUtc { get; set; }

	public virtual GameItem HostItem { get; set; } = null!;
	public virtual ICollection<ComputerMailAccount> Accounts { get; set; }
}
