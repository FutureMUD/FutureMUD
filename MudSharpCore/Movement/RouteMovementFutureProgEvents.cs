#nullable enable

using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;

namespace MudSharp.Movement;

/// <summary>
/// Publishes the stable, legacy-type-compatible FutureProg event contract for longitudinal
/// RouteCell movement. Callers deliberately supply only the character cohort and vehicle
/// exterior projections; dragged cargo and hitch gear are not independent hook targets.
/// </summary>
internal static class RouteMovementFutureProgEvents
{
	private const double PositionToleranceMetres = 0.0005;

	public static void Begin(IEnumerable<IPerceivable> movers, RouteMovementHookContext context)
	{
		Dispatch(movers, EventType.RouteMovementBegin, mover =>
		[
			mover,
			context.RouteCell,
			context.OriginMetres,
			context.DestinationMetres,
			context.Direction.DescribeEnum(),
			context.SpeedMetresPerSecond,
			context.OperationId.ToString("D")
		]);
	}

	public static void Progress(
		IEnumerable<IPerceivable> movers,
		RouteMovementHookContext context,
		double previousMetres,
		double currentMetres)
	{
		if (Math.Abs(currentMetres - previousMetres) < PositionToleranceMetres)
		{
			return;
		}

		var targets = UniqueTargets(movers);
		foreach (var mover in targets)
		{
			mover.HandleEvent(
				EventType.RoutePositionChanged,
				mover,
				context.RouteCell,
				previousMetres,
				currentMetres,
				context.OperationId.ToString("D"));
			mover.HandleEvent(
				EventType.RouteMovementProgress,
				mover,
				context.RouteCell,
				previousMetres,
				currentMetres,
				context.DestinationMetres,
				context.Direction.DescribeEnum(),
				context.SpeedMetresPerSecond,
				context.OperationId.ToString("D"));
		}
	}

	public static void Complete(IEnumerable<IPerceivable> movers, RouteMovementHookContext context)
	{
		Dispatch(movers, EventType.RouteMovementComplete, mover =>
		[
			mover,
			context.RouteCell,
			context.OriginMetres,
			context.DestinationMetres,
			context.Direction.DescribeEnum(),
			context.OperationId.ToString("D")
		]);
	}

	public static void Cancelled(
		IEnumerable<IPerceivable> movers,
		RouteMovementHookContext context,
		double currentMetres,
		string reason)
	{
		Dispatch(movers, EventType.RouteMovementCancelled, mover =>
		[
			mover,
			context.RouteCell,
			context.OriginMetres,
			currentMetres,
			context.DestinationMetres,
			context.Direction.DescribeEnum(),
			reason,
			context.OperationId.ToString("D")
		]);
	}

	private static void Dispatch(
		IEnumerable<IPerceivable> movers,
		EventType eventType,
		Func<IPerceivable, dynamic[]> arguments)
	{
		foreach (var mover in UniqueTargets(movers))
		{
			mover.HandleEvent(eventType, arguments(mover));
		}
	}

	private static IReadOnlyCollection<IPerceivable> UniqueTargets(IEnumerable<IPerceivable> movers)
	{
		return movers
			.Where(x => x is not null)
			.Distinct<IPerceivable>(ReferenceEqualityComparer.Instance)
			.ToArray();
	}
}

internal readonly record struct RouteMovementHookContext(
	Guid OperationId,
	ICell RouteCell,
	double OriginMetres,
	double DestinationMetres,
	RouteCellDirection Direction,
	double SpeedMetresPerSecond);
