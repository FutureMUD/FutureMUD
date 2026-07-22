#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public partial class RouteCell
{
	public RouteCell()
	{
		Landmarks = new HashSet<RouteCellLandmark>();
		ExitAnchors = new HashSet<RouteExitAnchor>();
		ActiveMotions = new HashSet<ActiveRouteMotion>();
	}

	public long CellId { get; set; }
	public decimal LengthMetres { get; set; }
	public decimal DefaultPositionMetres { get; set; }
	public string PositiveDirectionName { get; set; } = null!;
	public string NegativeDirectionName { get; set; } = null!;
	public decimal MetresPerRoomEquivalent { get; set; }
	public long TopologyVersion { get; set; }

	public virtual Cell Cell { get; set; } = null!;
	public virtual ICollection<RouteCellLandmark> Landmarks { get; set; }
	public virtual ICollection<RouteExitAnchor> ExitAnchors { get; set; }
	public virtual ICollection<ActiveRouteMotion> ActiveMotions { get; set; }
}
