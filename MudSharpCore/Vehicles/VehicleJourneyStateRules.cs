#nullable enable

using MudSharp.Framework;

namespace MudSharp.Vehicles;

/// <summary>
/// Deterministic transition and recovery rules shared by live journeys and focused tests.
/// </summary>
public static class VehicleJourneyStateRules
{
	public static bool IsTerminal(VehicleJourneyState state)
	{
		return state is VehicleJourneyState.Arrived or VehicleJourneyState.Cancelled or VehicleJourneyState.Faulted;
	}

	public static bool CanTransition(VehicleJourneyState from, VehicleJourneyState to)
	{
		if (from == to)
		{
			return true;
		}
		if (IsTerminal(from))
		{
			return false;
		}
		if (to is VehicleJourneyState.Cancelled or VehicleJourneyState.Faulted)
		{
			return true;
		}
		return from switch
		{
			VehicleJourneyState.Scheduled => to is VehicleJourneyState.Boarding or VehicleJourneyState.Held,
			VehicleJourneyState.Boarding => to is VehicleJourneyState.Departing or VehicleJourneyState.Held or VehicleJourneyState.Arrived,
			VehicleJourneyState.Held => to is VehicleJourneyState.Boarding or VehicleJourneyState.Departing,
			VehicleJourneyState.Departing => to is VehicleJourneyState.EnRoute or VehicleJourneyState.Held,
			VehicleJourneyState.EnRoute => to is VehicleJourneyState.Dwelling or VehicleJourneyState.Arrived,
			VehicleJourneyState.Dwelling => to is VehicleJourneyState.Boarding or VehicleJourneyState.Departing or VehicleJourneyState.Held or VehicleJourneyState.Arrived,
			_ => false
		};
	}

	public static TimeSpan DowntimeDelay(VehicleJourneyState state, DateTimeOffset checkpoint, DateTimeOffset now)
	{
		return AccruesDowntimeDelay(state) && now > checkpoint
			? now - checkpoint
			: TimeSpan.Zero;
	}

	/// <summary>
	/// Active operational states accrue displayed delay while the engine is offline. Scheduled
	/// journeys remain governed by recurrence recovery, while terminal journeys remain fixed.
	/// </summary>
	public static bool AccruesDowntimeDelay(VehicleJourneyState state)
	{
		return state is VehicleJourneyState.Boarding or
		       VehicleJourneyState.Held or
		       VehicleJourneyState.Departing or
		       VehicleJourneyState.EnRoute or
		       VehicleJourneyState.Dwelling;
	}

	public static bool IsPhysicallyInFlight(VehicleJourneyState state)
	{
		return state is VehicleJourneyState.Departing or VehicleJourneyState.EnRoute;
	}

	/// <summary>
	/// Resolves the durable state after physical arrival. A failed boarding/docking operation is
	/// a journey fault at both intermediate and terminal stops: it must never masquerade as a
	/// successful arrival or permit the next leg to depart.
	/// </summary>
	public static VehicleJourneyState ArrivalState(bool boardingOpened, bool terminal)
	{
		return boardingOpened
			? terminal ? VehicleJourneyState.Arrived : VehicleJourneyState.Dwelling
			: VehicleJourneyState.Faulted;
	}

	public static VehicleJourneyState ResolveDurableState(
		VehicleJourneyState journeyRowState,
		VehicleJourneyState? latestEventState)
	{
		return latestEventState ?? journeyRowState;
	}

	public static bool HoldHasExpired(DateTimeOffset holdStarted, DateTimeOffset now, TimeSpan maximumHold)
	{
		return maximumHold <= TimeSpan.Zero || now - holdStarted >= maximumHold;
	}

	/// <summary>
	/// Resolves the start of the current continuous departure-hold episode from durable journey events.
	/// A retry may briefly transition through Departing and then emit another Held event, so the latest
	/// Held event is not necessarily the beginning of the configured maximum-hold window. A successfully
	/// started physical leg emits Departed and is the only event that resets that window.
	/// </summary>
	public static DateTimeOffset? HoldEpisodeStartedAt(IEnumerable<IVehicleJourneyEvent> events)
	{
		var ordered = events
			.OrderBy(x => x.Sequence)
			.ToList();
		var latestSuccessfulDeparture = ordered
			.Where(x => x.EventType == VehicleJourneyEventType.Departed)
			.Select(x => x.Sequence)
			.DefaultIfEmpty(long.MinValue)
			.Max();
		return ordered
			.Where(x => x.Sequence > latestSuccessfulDeparture &&
			            x.EventType == VehicleJourneyEventType.Held)
			.Select(x => (DateTimeOffset?)x.OccurredAtUtc)
			.FirstOrDefault();
	}
}

/// <summary>
/// Resolves the compiled step that contains a vehicle's durable spatial checkpoint. A recovery
/// never guesses when a route revisits the same physical point because doing so could replay an
/// already-completed exit or skip resource-bearing movement.
/// </summary>
public static class VehicleRouteRecoveryRules
{
	private const double CoordinateToleranceMetres = 0.002;

	public static bool IsAtLocation(SpatialLocation current, SpatialLocation expected)
	{
		return LocationsMatch(current, expected);
	}

	public static bool TryResolveStepIndex(
		IReadOnlyList<IVehicleRouteStep> steps,
		SpatialLocation current,
		out int stepIndex,
		out string reason)
	{
		stepIndex = 0;
		if (steps.Count == 0)
		{
			reason = "The compiled route leg has no movement steps.";
			return false;
		}

		var candidates = new HashSet<int>();
		for (var index = 0; index < steps.Count; index++)
		{
			var step = steps[index];
			if (LocationsMatch(current, step.Origin))
			{
				candidates.Add(index);
			}

			if (LocationsMatch(current, step.Destination))
			{
				candidates.Add(index + 1);
			}

			if (step is IVehicleRouteLinearStep linear && IsStrictlyInside(current, linear))
			{
				candidates.Add(index);
			}
		}

		if (candidates.Count == 1)
		{
			stepIndex = candidates.Single();
			reason = string.Empty;
			return true;
		}

		reason = candidates.Count == 0
			? "The vehicle's durable coordinate is not on the pinned route leg."
			: "The vehicle's durable coordinate occurs more than once on the pinned route leg; recovery cannot safely infer progress.";
		return false;
	}

	private static bool LocationsMatch(SpatialLocation first, SpatialLocation second)
	{
		if (!ReferenceEquals(first.Cell, second.Cell) || first.Layer != second.Layer)
		{
			return false;
		}

		if (!first.RoutePositionMetres.HasValue && !second.RoutePositionMetres.HasValue)
		{
			return true;
		}

		return first.RoutePositionMetres.HasValue && second.RoutePositionMetres.HasValue &&
		       Math.Abs(first.RoutePositionMetres.Value - second.RoutePositionMetres.Value) <=
		       CoordinateToleranceMetres;
	}

	private static bool IsStrictlyInside(SpatialLocation current, IVehicleRouteLinearStep step)
	{
		if (!ReferenceEquals(current.Cell, step.Origin.Cell) || current.Layer != step.Origin.Layer ||
		    !current.RoutePositionMetres.HasValue || !step.Origin.RoutePositionMetres.HasValue ||
		    !step.Destination.RoutePositionMetres.HasValue)
		{
			return false;
		}

		var lower = Math.Min(step.Origin.RoutePositionMetres.Value,
			step.Destination.RoutePositionMetres.Value);
		var upper = Math.Max(step.Origin.RoutePositionMetres.Value,
			step.Destination.RoutePositionMetres.Value);
		return current.RoutePositionMetres.Value > lower + CoordinateToleranceMetres &&
		       current.RoutePositionMetres.Value < upper - CoordinateToleranceMetres;
	}
}
