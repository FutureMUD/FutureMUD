#nullable enable

namespace MudSharp.Models;

public class StagedPerformanceRecord
{
	public long Id { get; set; }
	public string Kind { get; set; } = string.Empty;
	public string Definition { get; set; } = string.Empty;
}
