#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class EconomySnapshot
{
	public EconomySnapshot()
	{
		Entries = new HashSet<EconomySnapshotEntry>();
	}

	public long Id { get; set; }
	public DateTime RealDateTime { get; set; }
	public long? EconomicZoneId { get; set; }
	public long? FinancialPeriodId { get; set; }
	public string? MudDateTime { get; set; }
	public int Reason { get; set; }

	public virtual EconomicZone? EconomicZone { get; set; }
	public virtual FinancialPeriod? FinancialPeriod { get; set; }
	public virtual ICollection<EconomySnapshotEntry> Entries { get; set; }
}
