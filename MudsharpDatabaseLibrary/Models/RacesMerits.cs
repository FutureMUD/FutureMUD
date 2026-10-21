#nullable enable

namespace MudSharp.Models;

public partial class RacesMerits
{
	public long RaceId { get; set; }
	public long MeritId { get; set; }
	public virtual Race Race { get; set; } = null!;
	public virtual Merit Merit { get; set; } = null!;
}
