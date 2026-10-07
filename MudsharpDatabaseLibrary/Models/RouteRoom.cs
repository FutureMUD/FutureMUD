#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public partial class RouteRoom
{
	public RouteRoom()
	{
		Landmarks = new HashSet<RouteRoomLandmark>();
		ExitAnchors = new HashSet<RouteExitAnchor>();
		ActiveMotions = new HashSet<ActiveRouteMotion>();
	}

	public long RoomId { get; set; }
	public decimal LengthMetres { get; set; }
	public decimal DefaultPositionMetres { get; set; }
	public string PositiveDirectionName { get; set; } = null!;
	public string NegativeDirectionName { get; set; } = null!;
	public decimal MetresPerRoomEquivalent { get; set; }
	public long TopologyVersion { get; set; }

	public virtual Room Room { get; set; } = null!;
	public virtual ICollection<RouteRoomLandmark> Landmarks { get; set; }
	public virtual ICollection<RouteExitAnchor> ExitAnchors { get; set; }
	public virtual ICollection<ActiveRouteMotion> ActiveMotions { get; set; }
}
