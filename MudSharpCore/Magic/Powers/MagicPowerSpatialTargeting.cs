#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Magic.Powers;

/// <summary>
/// Adapts the legacy magic distance categories to exact RouteCell locality. Ordinary-cell
/// target discovery retains its historical behavior.
/// </summary>
internal static class MagicPowerSpatialTargeting
{
	private const double AdjacentRoomEquivalentCost = 1.0;

	public static IReadOnlyCollection<IPerceivable> AcquireTargets(
		ICharacter owner,
		MagicPowerDistance distance,
		IRouteSpatialService? spatialService = null,
		SpatialPerceivableReachability? reachability = null)
	{
		ArgumentNullException.ThrowIfNull(owner);
		spatialService ??= RouteSpatialService.Instance;
		reachability ??= SpatialPerceivableReachability.Instance;

		return distance switch
		{
			MagicPowerDistance.SameLocationOnly => AcquireSameLocation(owner, spatialService),
			MagicPowerDistance.AdjacentLocationsOnly => AcquireAdjacent(owner, spatialService, reachability),
			_ => throw new ArgumentOutOfRangeException(nameof(distance), distance,
				"Spatial magic target discovery only supports local and adjacent distances.")
		};
	}

	public static bool IsInRange(
		ICharacter owner,
		ICharacter target,
		MagicPowerDistance distance,
		IRouteSpatialService? spatialService = null,
		ISpatialPathfinder? pathfinder = null)
	{
		ArgumentNullException.ThrowIfNull(owner);
		ArgumentNullException.ThrowIfNull(target);
		spatialService ??= RouteSpatialService.Instance;

		if (distance == MagicPowerDistance.SameLocationOnly)
		{
			return owner.Location?.RouteDefinition is null
				? ReferenceEquals(owner.Location, target.Location)
				: spatialService.GetProximity(owner, target) <= Proximity.Immediate;
		}

		if (distance != MagicPowerDistance.AdjacentLocationsOnly)
		{
			throw new ArgumentOutOfRangeException(nameof(distance), distance,
				"Spatial magic range checks only support local and adjacent distances.");
		}

		if (owner.Location?.RouteDefinition is null && target.Location?.RouteDefinition is null)
		{
			return owner.Location.CellsInVicinity(1, static _ => true, static _ => true)
				.Any(x => ReferenceEquals(x, target.Location));
		}

		if (spatialService.GetProximity(owner, target) <= Proximity.Immediate)
		{
			return true;
		}

		var origin = spatialService.GetEffectiveLocation(owner);
		var destination = spatialService.GetEffectiveLocation(target);
		if (!spatialService.TryValidateLocation(origin, out _) ||
			!spatialService.TryValidateLocation(destination, out _))
		{
			return false;
		}

		pathfinder ??= owner.Gameworld.ExitManager.SpatialPathfinder;
		return pathfinder.TryFindPath(
			origin,
			destination,
			static _ => true,
			false,
			AdjacentRoomEquivalentCost,
			out var path) &&
		       path is not null &&
		       path.RoomEquivalentCost <= AdjacentRoomEquivalentCost;
	}

	private static IReadOnlyCollection<IPerceivable> AcquireSameLocation(
		ICharacter owner,
		IRouteSpatialService spatialService)
	{
		if (owner.Location.RouteDefinition is null)
		{
			return owner.Location.Characters
				.Cast<IPerceivable>()
				.Concat(owner.Location.GameItems)
				.ToArray();
		}

		var origin = spatialService.GetEffectiveLocation(owner);
		if (!spatialService.TryValidateLocation(origin, out _))
		{
			return Array.Empty<IPerceivable>();
		}

		var immediateDistance = owner.Gameworld.GetStaticDouble("RouteCellImmediateDistanceMetres");
		if (!double.IsFinite(immediateDistance) || immediateDistance <= 0.0)
		{
			immediateDistance = RouteSpatialConfiguration.Default.ImmediateDistanceMetres;
		}

		return spatialService
			.GetPerceivablesWithinAcrossLayers(origin, immediateDistance)
			.Where(x => spatialService.GetProximity(owner, x) <= Proximity.Immediate)
			.ToArray();
	}

	private static IReadOnlyCollection<IPerceivable> AcquireAdjacent(
		ICharacter owner,
		IRouteSpatialService spatialService,
		SpatialPerceivableReachability reachability)
	{
		var touchesRouteTopology = owner.Location.RouteDefinition is not null ||
		                          owner.Location.ExitsFor(null, true)
			                          .Any(x => x.Destination?.RouteDefinition is not null);
		if (!touchesRouteTopology)
		{
			return owner.Location.Characters
				.Cast<IPerceivable>()
				.Concat(owner.Location.GameItems)
				.Concat(owner.Location.ExitsFor(owner)
					.Select(x => x.Destination)
					.Where(x => x is not null)
					.SelectMany(x => x.Characters
						.Cast<IPerceivable>()
						.Concat(x.GameItems)))
				.Distinct((IEqualityComparer<IPerceivable>)ReferenceEqualityComparer.Instance)
				.ToArray();
		}

		var origin = spatialService.GetEffectiveLocation(owner);
		if (!spatialService.TryValidateLocation(origin, out _))
		{
			return Array.Empty<IPerceivable>();
		}

		var results = new HashSet<IPerceivable>(
			AcquireSameLocation(owner, spatialService),
			ReferenceEqualityComparer.Instance);
		foreach (var candidate in reachability.Find(
			origin,
			AdjacentRoomEquivalentCost,
			ignoreLayers: false))
		{
			results.Add(candidate.Perceivable);
		}

		return results.ToArray();
	}
}
