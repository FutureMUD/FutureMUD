#nullable enable

namespace MudSharp.Construction;

/// <summary>
/// Describes the spatial model used by a cell.
/// </summary>
public enum CellSpatialType
{
	Ordinary = 0,
	LinearRoute = 1
}

/// <summary>
/// Direction of travel along a linear route cell.
/// </summary>
public enum RouteCellDirection
{
	Negative = -1,
	Positive = 1
}
