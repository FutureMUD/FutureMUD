#nullable enable

using MudSharp.Construction;
using MudSharp.Character.Heritage;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine;

/// <summary>
/// Shared, non-echoing target acquisition for artificial intelligence. It deliberately follows the
/// visual rules of a player scan so that a remote target is not a free omniscient lookup.
/// </summary>
internal static class ScanTargetAcquisition
{
	public static IReadOnlyList<ICharacter> AcquireVisibleCharacters(ICharacter observer, uint maximumRange)
	{
		if (observer.Location is null || maximumRange == 0)
		{
			return [];
		}

		Dictionary<Difficulty, CheckOutcome> outcomes = observer.Gameworld
			.GetCheck(CheckType.ScanPerceptionCheck)
			.CheckAgainstAllDifficulties(observer, Difficulty.Normal, null);
		List<ICharacter> acquired = [];
		foreach (ScanCandidate candidate in Candidates(observer, maximumRange))
		{
			if (candidate.Cell is null || ReferenceEquals(candidate.Target, observer) ||
				!candidate.Target.RoomLayer.CanBeSeenFromLayer(observer.RoomLayer) ||
				!observer.CanSee(candidate.Target, PerceiveIgnoreFlags.IgnoreObscured))
			{
				continue;
			}

			Difficulty difficulty = candidate.Cell.SpotDifficulty(observer)
				.StageUp(Math.Max(0, candidate.Distance - 1));
			if (candidate.Target.CurrentContextualSize(SizeContext.Scan) < MinimumVisibleSize(observer, outcomes,
				difficulty))
			{
				continue;
			}

			observer.SeeTarget(candidate.Target);
			acquired.Add(candidate.Target);
		}

		return acquired;
	}

	/// <summary>
	/// Revalidates a remembered target without rerolling the scan. This is intentionally stricter than
	/// the generic seen-target memory: it preserves doors, corners, layers and current visibility.
	/// </summary>
	public static bool IsCurrentVisibleRangedTarget(ICharacter observer, ICharacter target, uint maximumRange)
	{
		return IsVisibleRangedTarget(observer, target, maximumRange, true);
	}

	/// <summary>
	/// Checks the current line of sight and range without rolling another scan. A wildlife group can
	/// share a leader or sentry's successful scan with members that independently have this same
	/// current view, rather than rolling the same scan for every animal.
	/// </summary>
	public static bool IsVisibleRangedTarget(ICharacter observer, ICharacter target, uint maximumRange,
		bool requireSeenTarget)
	{
		if (observer.Location is null || target.Location is null || maximumRange == 0 ||
			(requireSeenTarget && !observer.SeenTargets.Contains(target)) ||
			!target.RoomLayer.CanBeSeenFromLayer(observer.RoomLayer) ||
			!observer.CanSee(target, PerceiveIgnoreFlags.IgnoreObscured))
		{
			return false;
		}

		if (observer.Location.RouteDefinition is not null)
		{
			if (!ReferenceEquals(observer.Location, target.Location))
			{
				return false;
			}

			SpatialLocation origin = RouteSpatialService.Instance.GetEffectiveLocation(observer);
			SpatialLocation destination = RouteSpatialService.Instance.GetEffectiveLocation(target);
			double? separation = RouteSpatialService.Instance.GetExactSeparation(origin, destination);
			return separation.HasValue &&
			       separation.Value <= maximumRange * observer.Location.RouteDefinition.MetresPerRoomEquivalent;
		}

		return observer.Location.RouteDefinition is null &&
		       target.Location.RouteDefinition is null &&
		       observer.CellsInVicinity(maximumRange, true, true).Contains(target.Location);
	}

	private static IEnumerable<ScanCandidate> Candidates(ICharacter observer, uint maximumRange)
	{
		if (observer.Location!.RouteDefinition is { } route)
		{
			SpatialLocation origin = RouteSpatialService.Instance.GetEffectiveLocation(observer);
			double maximumDistanceMetres = maximumRange * route.MetresPerRoomEquivalent;
			return RouteSpatialService.Instance
				.GetPerceivablesWithinAcrossLayers(origin, maximumDistanceMetres)
				.OfType<ICharacter>()
				.Select(target => new ScanCandidate(
					target,
					target.Location,
					RouteDistance(observer, target, route.MetresPerRoomEquivalent)))
				.Where(x => x.Cell is not null);
		}

		return observer.CellsAndDistancesInVicinity(maximumRange, true, true)
			.SelectMany(x => x.Cell.Characters.Select(character => new ScanCandidate(character, x.Cell, x.Distance)));
	}

	private static int RouteDistance(ICharacter observer, ICharacter target, double metresPerRoomEquivalent)
	{
		SpatialLocation origin = RouteSpatialService.Instance.GetEffectiveLocation(observer);
		SpatialLocation destination = RouteSpatialService.Instance.GetEffectiveLocation(target);
		double? separation = RouteSpatialService.Instance.GetExactSeparation(origin, destination);
		// A scan may legitimately see between compatible route layers, whereas exact combat
		// separation deliberately requires the same layer. The route query has already limited
		// candidates to this cell and radius, so retain their horizontal distance for scan size
		// difficulty rather than overflowing to an artificial automatic check.
		if (!separation.HasValue && ReferenceEquals(origin.Cell, destination.Cell) &&
			origin.RoutePositionMetres.HasValue && destination.RoutePositionMetres.HasValue)
		{
			separation = Math.Abs(origin.RoutePositionMetres.Value - destination.RoutePositionMetres.Value);
		}

		return separation.HasValue
			? (int)Math.Min(int.MaxValue - (int)Difficulty.Impossible,
				Math.Ceiling(separation.Value / metresPerRoomEquivalent))
			: int.MaxValue - (int)Difficulty.Impossible;
	}

	private static SizeCategory MinimumVisibleSize(ICharacter observer,
		IReadOnlyDictionary<Difficulty, CheckOutcome> outcomes, Difficulty difficulty)
	{
		SizeCategory minimum = observer.CurrentContextualSize(SizeContext.None);
		return outcomes[difficulty].Outcome switch
		{
			Outcome.MajorFail => minimum.ChangeSize(2),
			Outcome.Fail => minimum.ChangeSize(1),
			Outcome.MinorPass => minimum.ChangeSize(-1),
			Outcome.Pass => minimum.ChangeSize(-2),
			Outcome.MajorPass => minimum.ChangeSize(-3),
			_ => minimum
		};
	}

	private sealed record ScanCandidate(ICharacter Target, ICell? Cell, int Distance);
}
