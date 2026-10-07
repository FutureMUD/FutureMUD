#nullable enable

namespace MudSharp.Models;

public partial class RouteExitAnchor
{
	public long ExitId { get; set; }
	public long RouteRoomId { get; set; }
	public decimal MinimumPositionMetres { get; set; }
	public decimal MaximumPositionMetres { get; set; }
	public decimal ArrivalPositionMetres { get; set; }

	public virtual Exit Exit { get; set; } = null!;
	public virtual RouteRoom RouteRoom { get; set; } = null!;
}
