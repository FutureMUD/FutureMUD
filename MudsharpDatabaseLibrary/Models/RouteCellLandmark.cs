#nullable enable

namespace MudSharp.Models;

public partial class RouteCellLandmark
{
	public long Id { get; set; }
	public long RouteCellId { get; set; }
	public string Name { get; set; } = null!;
	public string Keywords { get; set; } = null!;
	public string Description { get; set; } = null!;
	public decimal PositionMetres { get; set; }
	public int DisplayOrder { get; set; }

	public virtual RouteCell RouteCell { get; set; } = null!;
}
