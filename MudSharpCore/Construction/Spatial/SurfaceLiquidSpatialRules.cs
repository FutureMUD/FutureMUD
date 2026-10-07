#nullable enable

namespace MudSharp.Construction;

/// <summary>
/// Coordinate rules for virtual surface liquids in RouteRooms. A null coordinate denotes
/// uniform environmental liquid (for example rainfall); point spills retain an exact coordinate.
/// </summary>
public static class SurfaceLiquidSpatialRules
{
	public static double? Normalise(IRouteRoomDefinition? route, double? coordinateMetres)
	{
		if (route is null)
		{
			if (coordinateMetres.HasValue)
			{
				throw new ArgumentException("An ordinary room surface cannot have a RouteRoom coordinate.",
					nameof(coordinateMetres));
			}

			return null;
		}

		if (!coordinateMetres.HasValue)
		{
			return null;
		}

		if (!double.IsFinite(coordinateMetres.Value) || coordinateMetres.Value < 0.0 ||
			coordinateMetres.Value > route.LengthMetres)
		{
			throw new ArgumentOutOfRangeException(
				nameof(coordinateMetres),
				$"A surface-liquid coordinate must be between 0 and {route.LengthMetres:N3} metres.");
		}

		return Math.Round(coordinateMetres.Value, 3, MidpointRounding.AwayFromZero);
	}

	public static bool IsVisible(
		double? surfaceCoordinateMetres,
		double? viewerCoordinateMetres,
		double immediateDistanceMetres)
	{
		if (!surfaceCoordinateMetres.HasValue)
		{
			return true;
		}

		return viewerCoordinateMetres.HasValue &&
		       double.IsFinite(immediateDistanceMetres) &&
		       immediateDistanceMetres >= 0.0 &&
		       Math.Abs(surfaceCoordinateMetres.Value - viewerCoordinateMetres.Value) <= immediateDistanceMetres;
	}
}
