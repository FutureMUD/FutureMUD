using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Combat.ScatterStrategies;

internal readonly record struct CellScatterInfo(ICell Cell, int Distance, CardinalDirection DirectionFromOrigin);

internal static class ScatterStrategyUtilities
{
	public static RangedScatterResult? CreateResult(
		CellScatterInfo info,
		IPerceiver originalTarget,
		IPerceiver? struckTarget)
	{
		if (!TryResolveImpactLocation(info.Cell, originalTarget, struckTarget, out var impact))
		{
			return null;
		}

		return new RangedScatterResult(
			info.Cell,
			impact.Layer,
			info.DirectionFromOrigin,
			info.Distance,
			struckTarget,
			impact.RoutePositionMetres);
	}

	public static IReadOnlyCollection<IPerceiver> GetCandidatesAtImpact(
		CellScatterInfo info,
		IPerceiver originalTarget,
		bool sameLayerOnly)
	{
		if (!TryResolveImpactLocation(info.Cell, originalTarget, null, out var impact))
		{
			return Array.Empty<IPerceiver>();
		}

		if (info.Cell.RouteDefinition is null)
		{
			return info.Cell.Perceivables
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
		if (scatter.Cell.RouteDefinition is null)
		{
			return scatter.Cell.Perceivables
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
		if (!ReferenceEquals(scatter.Cell, candidate.Location))
		{
			return Proximity.Unapproximable;
		}

		if (ReferenceEquals(scatter.Target, candidate))
		{
			return Proximity.Intimate;
		}

		if (scatter.Cell.RouteDefinition is null)
		{
			return candidate.RoomLayer == scatter.RoomLayer
				? Proximity.Distant
				: Proximity.VeryDistant;
		}

		var candidateLocation = RouteSpatialService.Instance.GetEffectiveLocation(candidate);
		if (!RouteSpatialService.Instance.TryValidateLocation(scatter.ImpactLocation, out _) ||
			!RouteSpatialService.Instance.TryValidateLocation(candidateLocation, out _) ||
			!ReferenceEquals(scatter.Cell, candidateLocation.Cell))
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
		ICell cell,
		IPerceiver originalTarget,
		IPerceiver? struckTarget,
		out SpatialLocation impact)
	{
		if (cell.RouteDefinition is null)
		{
			impact = new SpatialLocation(
				cell,
				struckTarget?.RoomLayer ?? originalTarget.RoomLayer);
			return true;
		}

		if (struckTarget is not null &&
			TryGetEffectiveRouteLocation(struckTarget, cell, out impact))
		{
			return true;
		}

		return TryGetEffectiveRouteLocation(originalTarget, cell, out impact);
	}

	private static bool TryGetEffectiveRouteLocation(
		IPerceiver perceiver,
		ICell expectedCell,
		out SpatialLocation location)
	{
		location = RouteSpatialService.Instance.GetEffectiveLocation(perceiver);
		if (!ReferenceEquals(location.Cell, expectedCell))
		{
			location = new SpatialLocation(
				perceiver.Location,
				perceiver.RoomLayer,
				perceiver.RoutePositionMetres);
		}

		return ReferenceEquals(location.Cell, expectedCell) &&
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
    /// Returns each reachable cell once within range using the shortest distance from the origin. If multiple paths
    /// exist, the cell is still returned once and the direction reflects the first step of that shortest path.
    /// </summary>
    /// <param name="originalTarget">The perceiver used as the origin.</param>
    /// <param name="range">The maximum range to search.</param>
    /// <param name="respectDoors">Whether closed doors block traversal.</param>
    /// <returns>A list of unique reachable cells with distance and direction.</returns>
    public static IReadOnlyList<CellScatterInfo> GetCellInfos(IPerceiver originalTarget, uint range, bool respectDoors)
    {
        if (originalTarget.Location is null)
        {
            return Array.Empty<CellScatterInfo>();
        }

        List<(ICell Cell, int Distance)> cells = originalTarget.CellsAndDistancesInVicinity(range, respectDoors, false).ToList();
        if (cells.Count == 0)
        {
            return Array.Empty<CellScatterInfo>();
        }

        List<CellScatterInfo> result = new(cells.Count);
        foreach ((ICell cell, int distance) in cells)
        {
            CardinalDirection direction = CardinalDirection.Unknown;
            if (distance > 0)
            {
                DummyPerceiver dummy = new(location: cell)
                {
                    RoomLayer = originalTarget.RoomLayer
                };
                List<ICellExit> pathToCell = originalTarget
                        .PathBetween(dummy, (uint)(distance + 1), false, false, true)
                        .ToList();
                if (pathToCell.Count > 0)
                {
                    direction = pathToCell[0].OutboundDirection;
                }
            }

            result.Add(new CellScatterInfo(cell, distance, direction));
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
