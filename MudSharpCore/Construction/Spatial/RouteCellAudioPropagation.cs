#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Construction;

/// <summary>
/// Emits distance-attenuated audio from an exact RouteCell coordinate. It deliberately
/// bypasses cell-wide output because a RouteCell may represent many kilometres.
/// </summary>
internal sealed class RouteCellAudioPropagation
{
	private const double CostEpsilon = 0.000000001;
	private readonly IRouteSpatialService _spatialService;
	private readonly SpatialPerceivableReachability _reachability;

	public RouteCellAudioPropagation(
		IRouteSpatialService spatialService,
		SpatialPerceivableReachability reachability)
	{
		_spatialService = spatialService ?? throw new ArgumentNullException(nameof(spatialService));
		_reachability = reachability ?? throw new ArgumentNullException(nameof(reachability));
	}

	public static RouteCellAudioPropagation Instance { get; } = new(
		RouteSpatialService.Instance,
		SpatialPerceivableReachability.Instance);

	/// <summary>
	/// Determines whether the bounded audio graph reaches RouteCell topology. Purely ordinary
	/// propagation stays on the legacy path; once a RouteCell is reachable, the whole search
	/// uses weighted spatial paths so the linear cell cannot become a one-room shortcut.
	/// </summary>
	public bool RequiresSpatialPropagation(ICell sourceCell, AudioVolume volume)
	{
		ArgumentNullException.ThrowIfNull(sourceCell);
		if (volume == AudioVolume.Silent)
		{
			return false;
		}

		if (sourceCell.RouteDefinition is not null)
		{
			return true;
		}

		var maximumCost = (double)(int)volume;
		var costs = new Dictionary<ICell, double>(ReferenceEqualityComparer.Instance)
		{
			[sourceCell] = 0.0
		};
		var queue = new PriorityQueue<ICell, double>();
		queue.Enqueue(sourceCell, 0.0);
		while (queue.TryDequeue(out var cell, out var queuedCost))
		{
			if (!costs.TryGetValue(cell, out var currentCost) || queuedCost > currentCost + CostEpsilon)
			{
				continue;
			}

			foreach (var exit in cell.ExitsFor(null, true) ?? Array.Empty<Boundary.ICellExit>())
			{
				if (exit?.Destination is not { } destination)
				{
					continue;
				}

				var candidateCost = currentCost + GetExitCost(exit);
				if (candidateCost > maximumCost + CostEpsilon)
				{
					continue;
				}

				if (destination.RouteDefinition is not null)
				{
					return true;
				}

				if (costs.TryGetValue(destination, out var existingCost) &&
					existingCost <= candidateCost + CostEpsilon)
				{
					continue;
				}

				costs[destination] = candidateCost;
				queue.Enqueue(destination, candidateCost);
			}
		}

		return false;
	}

	public bool Propagate(
		ICell sourceCell,
		string audioText,
		AudioVolume volume,
		IPerceiver source,
		RoomLayer originalLayer,
		bool ignoreOriginLayer)
	{
		ArgumentNullException.ThrowIfNull(sourceCell);
		ArgumentNullException.ThrowIfNull(audioText);
		ArgumentNullException.ThrowIfNull(source);

		if (volume == AudioVolume.Silent ||
			!TryResolveSourceLocation(sourceCell, source, originalLayer, out var origin))
		{
			return false;
		}

		var reachable = _reachability.Find(
			origin,
			(int)volume,
			static x => x is ICharacter,
			ignoreLayers: true);
		foreach (var result in reachable)
		{
			if (ReferenceEquals(result.Perceivable, source) ||
				ignoreOriginLayer &&
				result.Location.Layer == originalLayer &&
				result.Path.RoomEquivalentCost <= CostEpsilon)
			{
				continue;
			}

			var adjustedVolume = Attenuate(volume, result.Path.RoomEquivalentCost);
			if (adjustedVolume == AudioVolume.Silent)
			{
				continue;
			}

			var direction = DescribeDirection(origin, result.Location, result.Path);
			var output = new AudioOutput(
				new Emote(
					string.Format(
						audioText,
						direction,
						adjustedVolume.DescribeEnum(true)),
					source),
				adjustedVolume,
				flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers);
			result.Perceivable.OutputHandler?.Send(
				output,
				!output.Style.HasFlag(OutputStyle.NoNewLine),
				!output.Style.HasFlag(OutputStyle.NoPage));
		}

		return true;
	}

	private bool TryResolveSourceLocation(
		ICell sourceCell,
		IPerceiver source,
		RoomLayer originalLayer,
		out SpatialLocation origin)
	{
		var effective = _spatialService.GetEffectiveLocation(source);
		if (ReferenceEquals(effective.Cell, sourceCell) &&
			_spatialService.TryValidateLocation(effective, out _))
		{
			origin = new SpatialLocation(sourceCell, originalLayer, effective.RoutePositionMetres);
			return _spatialService.TryValidateLocation(origin, out _);
		}

		if (source is IGameItem item)
		{
			var owner = item.InInventoryOf ?? (ILocateable?)item.ContainedIn;
			var inheritedPosition = _spatialService.GetInheritedRoutePosition(item, owner);
			if (inheritedPosition.HasValue)
			{
				origin = new SpatialLocation(sourceCell, originalLayer, inheritedPosition);
				return _spatialService.TryValidateLocation(origin, out _);
			}
		}

		origin = default;
		return false;
	}

	private static AudioVolume Attenuate(AudioVolume volume, double roomEquivalentCost)
	{
		var equivalentSteps = roomEquivalentCost <= CostEpsilon
			? 0U
			: (uint)Math.Ceiling(roomEquivalentCost - CostEpsilon);
		var attenuationSteps = equivalentSteps > 0U ? equivalentSteps - 1U : 0U;
		return volume.StageDown(attenuationSteps);
	}

	private static double GetExitCost(Boundary.ICellExit exit)
	{
		var multiplier = exit.Exit?.TimeMultiplier ?? 1.0;
		return double.IsFinite(multiplier) && multiplier > 0.0
			? multiplier
			: 1.0;
	}

	private static string DescribeDirection(
		SpatialLocation source,
		SpatialLocation listener,
		ISpatialPath path)
	{
		if (path.Steps.LastOrDefault() is ILinearRoutePathStep linear)
		{
			var route = listener.Cell.RouteDefinition ?? linear.RouteCell;
			var label = linear.Direction == RouteCellDirection.Positive
				? route.NegativeDirectionName
				: route.PositiveDirectionName;
			return $"from {label}";
		}

		var reverseExits = path.TraversedExits
			.Reverse()
			.Select(x => x.Opposite)
			.Where(x => x is not null)
			.Cast<Boundary.ICellExit>()
			.ToArray();
		if (reverseExits.Length > 0)
		{
			return reverseExits.DescribeDirectionsToFrom();
		}

		if (ReferenceEquals(source.Cell, listener.Cell) &&
			source.RoutePositionMetres.HasValue &&
			listener.RoutePositionMetres.HasValue)
		{
			var delta = source.RoutePositionMetres.Value - listener.RoutePositionMetres.Value;
			if (Math.Abs(delta) <= CostEpsilon)
			{
				return "here";
			}

			var route = source.Cell.RouteDefinition!;
			return $"from {(delta > 0.0 ? route.PositiveDirectionName : route.NegativeDirectionName)}";
		}

		return "nearby";
	}
}
