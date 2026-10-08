using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Combat.ScatterStrategies;

internal readonly record struct RoomScatterInfo(IRoom Room, int Distance, CardinalDirection DirectionFromOrigin);

internal static class ScatterStrategyUtilities
{
	public static RangedScatterResult? CreateResult(
		RoomScatterInfo info,
		IPerceiver originalTarget,
		IPerceiver? struckTarget)
	{
		if (!TryResolveImpactLocation(info.Room, originalTarget, struckTarget, out var impact))
		{
			return null;
		}

		return new RangedScatterResult(
			info.Room,
			impact.Layer,
			info.DirectionFromOrigin,
			info.Distance,
			struckTarget,
			impact.RoutePositionMetres);
	}

	public static IReadOnlyCollection<IPerceiver> GetCandidatesAtImpact(
		RoomScatterInfo info,
		IPerceiver originalTarget,
		bool sameLayerOnly)
	{
		if (!TryResolveImpactLocation(info.Room, originalTarget, null, out var impact))
		{
			return Array.Empty<IPerceiver>();
		}

		if (info.Room.RouteDefinition is null)
		{
			return info.Room.Perceivables
				.Where(x => !sameLayerOnly || x.RoomLayer == impact.Layer)
				.OfType<IPerceiver>()
				.ToArray();
		}

		var configuration = ResolveSpatialConfiguration(originalTarget.Gameworld);
		var candidates = sameLayerOnly
			? RouteSpatialService.Instance.GetPerceivablesWithin(
				impact,
				configuration.ImmediateDistanceMetres)
			: RouteSpatialService.Instance.GetPerceivablesWithinAcrossLayers(
				impact,
				configuration.ImmediateDistanceMetres);
		return candidates.OfType<IPerceiver>().ToArray();
	}

	public static IReadOnlyCollection<IPerceivable> GetPerceivablesNearImpact(
		RangedScatterResult scatter,
		IFuturemud? gameworld,
		Proximity maximumProximity,
		bool sameLayerOnly)
	{
		if (scatter.Room.RouteDefinition is null)
		{
			return scatter.Room.Perceivables
				.Where(x => !sameLayerOnly || x.RoomLayer == scatter.RoomLayer)
				.ToArray();
		}

		if (!RouteSpatialService.Instance.TryValidateLocation(scatter.ImpactLocation, out _))
		{
			return Array.Empty<IPerceivable>();
		}

		var configuration = ResolveSpatialConfiguration(gameworld);
		var maximumDistance = MaximumDistanceFor(maximumProximity, configuration);
		return sameLayerOnly
			? RouteSpatialService.Instance.GetPerceivablesWithin(scatter.ImpactLocation, maximumDistance)
			: RouteSpatialService.Instance.GetPerceivablesWithinAcrossLayers(
				scatter.ImpactLocation,
				maximumDistance);
	}

	public static Proximity GetImpactProximity(
		RangedScatterResult scatter,
		IPerceivable candidate,
		IFuturemud? gameworld)
	{
		if (!ReferenceEquals(scatter.Room, candidate.Location))
		{
			return Proximity.Unapproximable;
		}

		if (ReferenceEquals(scatter.Target, candidate))
		{
			return Proximity.Intimate;
		}

		if (scatter.Room.RouteDefinition is null)
		{
			return candidate.RoomLayer == scatter.RoomLayer
				? Proximity.Distant
				: Proximity.VeryDistant;
		}

		var candidateLocation = RouteSpatialService.Instance.GetEffectiveLocation(candidate);
		if (!RouteSpatialService.Instance.TryValidateLocation(scatter.ImpactLocation, out _) ||
			!RouteSpatialService.Instance.TryValidateLocation(candidateLocation, out _) ||
			!ReferenceEquals(scatter.Room, candidateLocation.Room))
		{
			return Proximity.Unapproximable;
		}

		var separation = Math.Abs(
			scatter.RoutePositionMetres!.Value - candidateLocation.RoutePositionMetres!.Value);
		var configuration = ResolveSpatialConfiguration(gameworld);
		var proximity = ProximityForSeparation(separation, configuration);
		return candidateLocation.Layer != scatter.RoomLayer && proximity < Proximity.VeryDistant
			? Proximity.VeryDistant
			: proximity;
	}

	public static RouteSpatialConfiguration ResolveSpatialConfiguration(IFuturemud? gameworld)
	{
		var configuration = gameworld is null
			? RouteSpatialConfiguration.Default
			: RouteSpatialConfiguration.FromGameworld(gameworld);
		try
		{
			configuration.Validate();
			return configuration;
		}
		catch (InvalidOperationException)
		{
			return RouteSpatialConfiguration.Default;
		}
	}

	private static bool TryResolveImpactLocation(
		IRoom room,
		IPerceiver originalTarget,
		IPerceiver? struckTarget,
		out SpatialLocation impact)
	{
		if (room.RouteDefinition is null)
		{
			impact = new SpatialLocation(
				room,
				struckTarget?.RoomLayer ?? originalTarget.RoomLayer);
			return true;
		}

		if (struckTarget is not null &&
			TryGetEffectiveRouteLocation(struckTarget, room, out impact))
		{
			return true;
		}

		return TryGetEffectiveRouteLocation(originalTarget, room, out impact);
	}

	private static bool TryGetEffectiveRouteLocation(
		IPerceiver perceiver,
		IRoom expectedRoom,
		out SpatialLocation location)
	{
		location = RouteSpatialService.Instance.GetEffectiveLocation(perceiver);
		if (!ReferenceEquals(location.Room, expectedRoom))
		{
			location = new SpatialLocation(
				perceiver.Location,
				perceiver.RoomLayer,
				perceiver.RoutePositionMetres);
		}

		return ReferenceEquals(location.Room, expectedRoom) &&
		       RouteSpatialService.Instance.TryValidateLocation(location, out _);
	}

	private static double MaximumDistanceFor(
		Proximity proximity,
		RouteSpatialConfiguration configuration)
	{
		return proximity switch
		{
			Proximity.Intimate => 0.0,
			Proximity.Immediate => configuration.ImmediateDistanceMetres,
			Proximity.Proximate => configuration.ProximateDistanceMetres,
			Proximity.Distant => configuration.DistantDistanceMetres,
			_ => configuration.VeryDistantDistanceMetres
		};
	}

	private static Proximity ProximityForSeparation(
		double separation,
		RouteSpatialConfiguration configuration)
	{
		if (separation <= configuration.ImmediateDistanceMetres)
		{
			return Proximity.Immediate;
		}

		if (separation <= configuration.ProximateDistanceMetres)
		{
			return Proximity.Proximate;
		}

		if (separation <= configuration.DistantDistanceMetres)
		{
			return Proximity.Distant;
		}

		return separation <= configuration.VeryDistantDistanceMetres
			? Proximity.VeryDistant
			: Proximity.Unapproximable;
	}

    /// <summary>
    /// Returns each reachable room once within range using the shortest distance from the origin. If multiple paths
    /// exist, the room is still returned once and the direction reflects the first step of that shortest path.
    /// </summary>
    /// <param name="originalTarget">The perceiver used as the origin.</param>
    /// <param name="range">The maximum range to search.</param>
    /// <param name="respectDoors">Whether closed doors block traversal.</param>
    /// <returns>A list of unique reachable rooms with distance and direction.</returns>
    public static IReadOnlyList<RoomScatterInfo> GetRoomInfos(IPerceiver originalTarget, uint range, bool respectDoors)
    {
        if (originalTarget.Location is null)
        {
            return Array.Empty<RoomScatterInfo>();
        }

        List<(IRoom Room, int Distance)> rooms = originalTarget.RoomsAndDistancesInVicinity(range, respectDoors, false).ToList();
        if (rooms.Count == 0)
        {
            return Array.Empty<RoomScatterInfo>();
        }

        List<RoomScatterInfo> result = new(rooms.Count);
        foreach ((IRoom room, int distance) in rooms)
        {
            CardinalDirection direction = CardinalDirection.Unknown;
            if (distance > 0)
            {
                DummyPerceiver dummy = new(location: room)
                {
                    RoomLayer = originalTarget.RoomLayer
                };
                List<IRoomExit> pathToRoom = originalTarget
                        .PathBetween(dummy, (uint)(distance + 1), false, false, true)
                        .ToList();
                if (pathToRoom.Count > 0)
                {
                    direction = pathToRoom[0].OutboundDirection;
                }
            }

            result.Add(new RoomScatterInfo(room, distance, direction));
        }

        return result;
    }

    public static string DescribeFromDirection(CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.Unknown => string.Empty,
            CardinalDirection.Up => " from above",
            CardinalDirection.Down => " from below",
            _ => $" from the {new[] { direction }.DescribeDirection()}"
        };
    }
}
