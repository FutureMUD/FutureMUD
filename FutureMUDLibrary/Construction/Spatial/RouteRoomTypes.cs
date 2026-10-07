#nullable enable

namespace MudSharp.Construction;

/// <summary>
/// Describes the spatial model used by a room.
/// </summary>
public enum RoomSpatialType
{
	Ordinary = 0,
	LinearRoute = 1
}

/// <summary>
/// Direction of travel along a linear route room.
/// </summary>
public enum RouteRoomDirection
{
	Negative = -1,
	Positive = 1
}
