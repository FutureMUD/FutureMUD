#nullable enable

namespace MudSharp.Models;

public partial class CommoditySpoilageRule
{
	public long Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public bool Enabled { get; set; }
	public int Priority { get; set; }
	public long? MaterialId { get; set; }
	public long? MaterialTagId { get; set; }
	public long? CommodityTagId { get; set; }
	public long ResultMaterialId { get; set; }
	public long? ResultCommodityTagId { get; set; }
	public long SecondsUntilSpoiled { get; set; }
	public string? SpoilEcho { get; set; }

	public virtual Material? Material { get; set; }
	public virtual Tag? MaterialTag { get; set; }
	public virtual Tag? CommodityTag { get; set; }
	public virtual Material ResultMaterial { get; set; } = null!;
	public virtual Tag? ResultCommodityTag { get; set; }
}
