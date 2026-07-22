#nullable enable

using System;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Movement;

/// <summary>
/// A continuous, one-dimensional movement segment. Runtime movement owns its clock and
/// supplies elapsed time to <see cref="PositionAt"/>, which keeps interpolation deterministic
/// and independent of wall-clock time.
/// </summary>
public interface ISpatialMovementSegment
{
	SpatialLocation Origin { get; }
	SpatialLocation Destination { get; }
	RouteCellDirection Direction { get; }
	double DistanceMetres { get; }
	double SpeedMetresPerSecond { get; }
	TimeSpan Duration { get; }

	SpatialLocation PositionAt(TimeSpan elapsed);
}
