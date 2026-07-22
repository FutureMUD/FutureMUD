#nullable enable

namespace MudSharp.Models;

public partial class RouteExitAnchor
{
	public long ExitId { get; set; }
	public long RouteCellId { get; set; }
	public decimal MinimumPositionMetres { get; set; }
	public decimal MaximumPositionMetres { get; set; }
	public decimal ArrivalPositionMetres { get; set; }

	public virtual Exit Exit { get; set; } = null!;
	public virtual RouteCell RouteCell { get; set; } = null!;
}
