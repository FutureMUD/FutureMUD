#nullable enable

namespace MudSharp.Models;

public class RegionalGatheringProfile
{
	public long Id { get; set; }
	public string Name { get; set; } = "";
	public long Revision { get; set; } = 1;
	public string Definition { get; set; } = "";
}

public class RegionalGatheringPool
{
	public string OwnerKind { get; set; } = "";
	public long OwnerId { get; set; }
	public long? ProfileId { get; set; }
	public int Priority { get; set; }
	public long Revision { get; set; }
	public double Stock { get; set; }
	public double Capacity { get; set; }
	public bool Initialised { get; set; }
	public bool Inherit { get; set; }
}
