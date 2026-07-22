#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// Finds weighted paths that can mix ordinary exit traversal with exact longitudinal
/// movement inside linear route cells.
/// </summary>
public interface ISpatialPathfinder
{
	bool TryFindPath(
		SpatialLocation origin,
		SpatialLocation destination,
		Func<ICellExit, bool>? suitabilityFunction,
		bool ignoreLayers,
		double maximumRoomEquivalentCost,
		out ISpatialPath? path);

	/// <summary>
	/// Compatibility adapter for consumers that can execute only ordinary exits. Returns false
	/// when the cheapest spatial path includes any longitudinal route-cell movement.
	/// </summary>
	bool TryFindExitOnlyPath(
		SpatialLocation origin,
		SpatialLocation destination,
		Func<ICellExit, bool>? suitabilityFunction,
		bool ignoreLayers,
		double maximumRoomEquivalentCost,
		out IReadOnlyList<ICellExit> exits);

	void InvalidateTopology(ICell? changedCell = null);
}
