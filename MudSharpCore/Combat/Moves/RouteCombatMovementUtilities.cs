#nullable enable

using MudSharp.Construction;

namespace MudSharp.Combat.Moves;

/// <summary>
/// Physical RouteCell movement rules shared by combat strategies and moves. Melee flags are
/// never allowed to substitute for longitudinal convergence or separation.
/// </summary>
public static class RouteCombatMovementUtilities
{
	public static bool ArePhysicallyImmediate(IPerceiver first, IPerceiver second)
	{
		if (!ReferenceEquals(first.Location, second.Location) || first.RoomLayer != second.RoomLayer)
		{
			return false;
		}

		if (first.Location.RouteDefinition is null)
		{
			return true;
		}

		var spatial = RouteSpatialService.Instance;
		var firstLocation = spatial.GetEffectiveLocation(first);
		var secondLocation = spatial.GetEffectiveLocation(second);
		var separation = spatial.GetExactSeparation(firstLocation, secondLocation);
		if (!separation.HasValue)
		{
			return false;
		}

		return separation.Value <= RouteSpatialConfiguration
			.FromGameworld(first.Gameworld)
			.ImmediateDistanceMetres;
	}

	public static bool TryEnterMelee(ICharacter assailant, ICharacter target)
	{
		if (!ArePhysicallyImmediate(assailant, target))
		{
			return false;
		}

		assailant.MeleeRange = true;
		target.MeleeRange = true;
		return true;
	}

	public static bool TryAdvanceTowardImmediate(ICharacter assailant, ICharacter target)
	{
		if (!TryGetRoutePair(assailant, target, out var assailantLocation, out var targetLocation))
		{
			return false;
		}

		var immediateDistance = RouteSpatialConfiguration
			.FromGameworld(assailant.Gameworld)
			.ImmediateDistanceMetres;
		var separation = targetLocation - assailantLocation;
		var absoluteSeparation = Math.Abs(separation);
		if (absoluteSeparation <= immediateDistance)
		{
			return false;
		}

		var gaitMultiplier = Math.Max(0.05, assailant.CurrentSpeed?.Multiplier ?? 1.0);
		var advanceDistance = Math.Max(immediateDistance, 2.8 / gaitMultiplier);
		var actualAdvance = Math.Min(absoluteSeparation - immediateDistance, advanceDistance);
		assailant.SetRoutePosition(assailantLocation + Math.Sign(separation) * actualAdvance);
		return true;
	}

	public static bool TryRetreatAlongRoute(ICharacter mover, IEnumerable<ICharacter> threats)
	{
		var origin = RouteSpatialService.Instance.GetEffectiveLocation(mover);
		var route = origin.Cell?.RouteDefinition;
		if (route is null || !origin.RoutePositionMetres.HasValue)
		{
			return false;
		}

		var threatPositions = threats
			.Where(x => !ReferenceEquals(x, mover))
			.Select(x => RouteSpatialService.Instance.GetEffectiveLocation(x))
			.Where(x => ReferenceEquals(x.Cell, origin.Cell) && x.Layer == origin.Layer &&
			            x.RoutePositionMetres.HasValue)
			.Select(x => x.RoutePositionMetres!.Value)
			.ToArray();
		if (threatPositions.Length == 0)
		{
			return false;
		}

		var current = origin.RoutePositionMetres.Value;
		var immediateDistance = RouteSpatialConfiguration
			.FromGameworld(mover.Gameworld)
			.ImmediateDistanceMetres;
		var gaitMultiplier = Math.Max(0.05, mover.CurrentSpeed?.Multiplier ?? 1.0);
		var retreatDistance = Math.Max(immediateDistance + 0.001, 2.8 / gaitMultiplier);
		var candidates = new[]
		{
			Math.Max(0.0, current - retreatDistance),
			Math.Min(route.LengthMetres, current + retreatDistance)
		}
			.Distinct()
			.Select(x => (Position: x, Separation: threatPositions.Min(y => Math.Abs(x - y))))
			.OrderByDescending(x => x.Separation)
			.ThenByDescending(x => Math.Abs(x.Position - current))
			.ToList();
		var currentSeparation = threatPositions.Min(x => Math.Abs(current - x));
		var best = candidates
			.Where(x => x.Separation > immediateDistance &&
			            x.Separation > currentSeparation + 0.0005)
			.Select(x => ((double Position, double Separation)?)x)
			.FirstOrDefault();
		if (!best.HasValue || Math.Abs(best.Value.Position - current) <= 0.0005)
		{
			return false;
		}

		mover.SetRoutePosition(best.Value.Position);
		return true;
	}

	private static bool TryGetRoutePair(
		ICharacter first,
		ICharacter second,
		out double firstPosition,
		out double secondPosition)
	{
		var firstLocation = RouteSpatialService.Instance.GetEffectiveLocation(first);
		var secondLocation = RouteSpatialService.Instance.GetEffectiveLocation(second);
		if (firstLocation.Cell?.RouteDefinition is null ||
		    !ReferenceEquals(firstLocation.Cell, secondLocation.Cell) ||
		    firstLocation.Layer != secondLocation.Layer ||
		    !firstLocation.RoutePositionMetres.HasValue ||
		    !secondLocation.RoutePositionMetres.HasValue)
		{
			firstPosition = 0.0;
			secondPosition = 0.0;
			return false;
		}

		firstPosition = firstLocation.RoutePositionMetres.Value;
		secondPosition = secondLocation.RoutePositionMetres.Value;
		return true;
	}
}
