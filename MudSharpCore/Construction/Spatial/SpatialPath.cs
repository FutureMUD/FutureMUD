#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

public sealed record LinearRoutePathStep(
	SpatialLocation Origin,
	SpatialLocation Destination,
	IRouteCellDefinition RouteCell,
	RouteCellDirection Direction,
	double DistanceMetres,
	double RoomEquivalentCost) : ILinearRoutePathStep;

public sealed record ExitTraversalPathStep(
	SpatialLocation Origin,
	SpatialLocation Destination,
	ICellExit Exit,
	double RoomEquivalentCost) : IExitTraversalPathStep;

/// <summary>
/// Immutable result of a weighted hybrid spatial path search.
/// </summary>
public sealed class SpatialPath : ISpatialPath
{
	private readonly IReadOnlyList<ISpatialPathStep> _steps;
	private readonly IReadOnlyList<ICellExit> _traversedExits;

	public SpatialPath(
		SpatialLocation origin,
		SpatialLocation destination,
		IEnumerable<ISpatialPathStep> steps)
	{
		Origin = origin;
		Destination = destination;

		var materialisedSteps = steps?.ToArray() ?? throw new ArgumentNullException(nameof(steps));
		_steps = Array.AsReadOnly(materialisedSteps);
		_traversedExits = Array.AsReadOnly(materialisedSteps
			.OfType<IExitTraversalPathStep>()
			.Select(x => x.Exit)
			.ToArray());
		RouteDistanceMetres = materialisedSteps
			.OfType<ILinearRoutePathStep>()
			.Sum(x => x.DistanceMetres);
		RoomEquivalentCost = materialisedSteps.Sum(x => x.RoomEquivalentCost);
	}

	public SpatialLocation Origin { get; }
	public SpatialLocation Destination { get; }
	public IReadOnlyList<ISpatialPathStep> Steps => _steps;
	public double RouteDistanceMetres { get; }
	public double RoomEquivalentCost { get; }
	public IReadOnlyList<ICellExit> TraversedExits => _traversedExits;
}
