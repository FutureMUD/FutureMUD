#nullable enable

namespace MudSharp.Models;

public class EconomySnapshotEntry
{
	public long Id { get; set; }
	public long EconomySnapshotId { get; set; }
	public long CurrencyId { get; set; }
	public int Metric { get; set; }
	public int ControlBucket { get; set; }
	public decimal Amount { get; set; }
	public decimal GlobalBaseValue { get; set; }
	public int EntityCount { get; set; }

	public virtual EconomySnapshot EconomySnapshot { get; set; } = null!;
	public virtual Currency Currency { get; set; } = null!;
}
