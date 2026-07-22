#nullable enable

using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Movement;

/// <summary>
/// An immutable, deterministic longitudinal journey inside one RouteCell.
/// </summary>
public sealed class LinearRouteMovementSegment : ISpatialMovementSegment
{
	public LinearRouteMovementSegment(
		SpatialLocation origin,
		SpatialLocation destination,
		double speedMetresPerSecond)
	{
		if (!ReferenceEquals(origin.Cell, destination.Cell) || origin.Layer != destination.Layer)
		{
			throw new ArgumentException("A linear RouteCell segment must remain in one cell and layer.");
		}

		var route = origin.Cell.RouteDefinition ??
		            throw new ArgumentException("A linear RouteCell segment requires a RouteCell.", nameof(origin));
		if (!origin.RoutePositionMetres.HasValue || !destination.RoutePositionMetres.HasValue ||
			!double.IsFinite(origin.RoutePositionMetres.Value) ||
			!double.IsFinite(destination.RoutePositionMetres.Value) ||
			origin.RoutePositionMetres.Value < 0.0 || origin.RoutePositionMetres.Value > route.LengthMetres ||
			destination.RoutePositionMetres.Value < 0.0 || destination.RoutePositionMetres.Value > route.LengthMetres)
		{
			throw new ArgumentOutOfRangeException(nameof(destination),
				"RouteCell segment coordinates must be finite and inside the RouteCell bounds.");
		}

		if (!double.IsFinite(speedMetresPerSecond) || speedMetresPerSecond <= 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(speedMetresPerSecond),
				"RouteCell travel speed must be finite and positive.");
		}

		Origin = origin;
		Destination = destination;
		SpeedMetresPerSecond = speedMetresPerSecond;
		DistanceMetres = Math.Abs(destination.RoutePositionMetres.Value - origin.RoutePositionMetres.Value);
		Direction = destination.RoutePositionMetres.Value >= origin.RoutePositionMetres.Value
			? RouteCellDirection.Positive
			: RouteCellDirection.Negative;
		Duration = TimeSpan.FromSeconds(DistanceMetres / speedMetresPerSecond);
	}

	public SpatialLocation Origin { get; }
	public SpatialLocation Destination { get; }
	public RouteCellDirection Direction { get; }
	public double DistanceMetres { get; }
	public double SpeedMetresPerSecond { get; }
	public TimeSpan Duration { get; }

	public SpatialLocation PositionAt(TimeSpan elapsed)
	{
		if (DistanceMetres <= 0.0 || elapsed >= Duration)
		{
			return Destination;
		}

		if (elapsed <= TimeSpan.Zero)
		{
			return Origin;
		}

		var fraction = Math.Clamp(elapsed.TotalSeconds / Duration.TotalSeconds, 0.0, 1.0);
		var position = Origin.RoutePositionMetres!.Value +
		               (Destination.RoutePositionMetres!.Value - Origin.RoutePositionMetres.Value) * fraction;
		return new SpatialLocation(Origin.Cell, Origin.Layer, position);
	}
}
