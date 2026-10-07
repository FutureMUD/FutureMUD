using MudSharp.Character.Heritage;
using MudSharp.Commands.Socials;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.ThirdPartyCode;
using System.Runtime.CompilerServices;

namespace MudSharp.Framework;

public static class PathSearch
{
    /// <summary>
    ///     Allows travel through exits with no door or with a door that is currently open.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns><see langword="true" /> when the exit has no closed door blocking it.</returns>
    public static bool RespectClosedDoors(IRoomExit exit)
    {
        return exit.Exit.Door?.IsOpen != false;
    }

    /// <summary>
    ///     Allows travel through open exits and through closed doors that have no locked locks.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns><see langword="true" /> when the exit is open, doorless, or its door is closed but unlocked.</returns>
    public static bool IncludeUnlockedDoors(IRoomExit exit)
    {
        return exit.Exit.Door?.IsOpen != false || exit.Exit.Door.Locks.All(x => !x.IsLocked);
    }

    /// <summary>
    ///     Allows travel or line-style pathing through open exits and through closed doors that can be fired through.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns><see langword="true" /> when the exit is open, doorless, or the door permits fire through it.</returns>
    public static bool IncludeFireableDoors(IRoomExit exit)
    {
        return exit.Exit.Door?.IsOpen != false || exit.Exit.Door.CanFireThrough;
    }

    /// <summary>
    ///     Allows every exit regardless of door state.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns>Always <see langword="true" />.</returns>
    public static bool IgnorePresenceOfDoors(IRoomExit exit)
    {
        return true;
    }

    /// <summary>
    ///     Builds a suitability function that ignores doors but rejects exits too small for a character.
    /// </summary>
    /// <param name="who">The character whose current cell-exit size is compared to each exit.</param>
    /// <returns>A predicate suitable for <c>PathBetween</c> overloads that accept an exit suitability function.</returns>
    public static Func<IRoomExit, bool> PathIgnoreDoors(ICharacter who)
    {
        return exit => who.CurrentContextualSize(SizeContext.RoomExit) <= exit.Exit.MaximumSizeToEnter;
    }

    /// <summary>
    ///     Builds a suitability function that rejects closed doors and exits too small for a character.
    /// </summary>
    /// <param name="who">The character whose current cell-exit size is compared to each exit.</param>
    /// <returns>A predicate suitable for ordinary movement path searches.</returns>
    public static Func<IRoomExit, bool> PathRespectClosedDoors(ICharacter who)
    {
        return exit => exit.Exit.Door?.IsOpen != false &&
                       who.CurrentContextualSize(SizeContext.RoomExit) <= exit.Exit.MaximumSizeToEnter;
    }

    /// <summary>
    ///     Builds a suitability function that permits doorless/open exits, closed unlocked doors, and exits large enough
    ///     for a character.
    /// </summary>
    /// <param name="who">The character whose current cell-exit size is compared to each exit.</param>
    /// <returns>A predicate suitable for path searches that assume the actor can open unlocked doors.</returns>
    public static Func<IRoomExit, bool> PathIncludeUnlockedDoors(ICharacter who)
    {
        return exit => (exit.Exit.Door?.IsOpen != false || exit.Exit.Door.Locks.All(x => !x.IsLocked)) &&
                       who.CurrentContextualSize(SizeContext.RoomExit) <= exit.Exit.MaximumSizeToEnter;
    }

    /// <summary>
    ///     Builds a suitability function that permits exits a character can traverse directly, open, or reasonably have
    ///     opened by eligible doorguard NPCs.
    /// </summary>
    /// <param name="who">The character whose size, body capabilities, socials, and local NPC helpers are considered.</param>
    /// <returns>
    ///     A predicate suitable for AI and movement planning where closed doors may be passable through interaction.
    /// </returns>
    public static Func<IRoomExit, bool> PathIncludeUnlockableDoors(ICharacter who)
    {
        return exit =>
        {
            if (who.CurrentContextualSize(SizeContext.RoomExit) > exit.Exit.MaximumSizeToEnter)
            {
                return false;
            }

            if (exit.Exit.Door?.IsOpen != false)
            {
                return true;
            }

            if (who.Body.CouldOpen(exit.Exit.Door))
            {
                return true;
            }

            foreach (INPC npc in exit.Origin.Characters.OfType<INPC>().Concat(exit.Destination.Characters.OfType<INPC>()))
            {
                List<DoorguardAI> doorguardAI = npc.AIs.OfType<DoorguardAI>().ToList();
                foreach (DoorguardAI ai in doorguardAI)
                {
                    WouldOpenResponse response = ai.WouldOpen(npc, who, exit);
                    switch (response.Response)
                    {
                        case WouldOpenResponseType.WontOpen:
                            continue;

                        case WouldOpenResponseType.WillOpenIfSocial:
                            ISocial social =
                                who.Gameworld.Socials.FirstOrDefault(x => x.Applies(who, response.Social, false));
                            if (social == null)
                            {
                                continue;
                            }

                            return true;
                        case WouldOpenResponseType.WillOpenIfMove:
                        case WouldOpenResponseType.WillOpenIfKnock:
                            return true;
                    }
                }
            }

            return false;
        };
    }
}

public static class PerceivedItemExtensions
{
    private const double SameDepthHeuristicTieBreaker = 0.001;

    private sealed class RoomReferenceComparer : IEqualityComparer<IRoom>
    {
        public static readonly RoomReferenceComparer Instance = new();

        public bool Equals(IRoom x, IRoom y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(IRoom obj)
        {
            return obj == null ? 0 : RuntimeHelpers.GetHashCode(obj);
        }
    }

    private sealed class PathSearchStep
    {
        public IRoom Room { get; init; }
        public IRoomExit Exit { get; init; }
        public PathSearchStep Parent { get; init; }
        public int Distance { get; init; }
    }

    private static HashSet<IRoom> NewRoomSet()
    {
        return new HashSet<IRoom>(RoomReferenceComparer.Instance);
    }

    private static Dictionary<IRoom, int> NewRoomDistanceDictionary()
    {
        return new Dictionary<IRoom, int>(RoomReferenceComparer.Instance);
    }

    private static List<IRoomExit> BuildPath(PathSearchStep step)
    {
        List<IRoomExit> path = new(step.Distance);
        PathSearchStep current = step;
        while (current != null)
        {
            path.Add(current.Exit);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    private static List<IRoomExit> FindShortestExitPath(IRoom source, IEnumerable<IRoom> targets,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction, bool ignoreLayers)
    {
        if (source == null || targets == null || suitabilityFunction == null)
        {
            return new List<IRoomExit>();
        }

		// Exit-only paths cannot represent longitudinal travel inside a RouteCell. Failing closed here
		// prevents legacy AI, combat, range, and following callers from treating a many-kilometre cell
		// as a single-room shortcut. Coordinate-aware callers must use ISpatialPathfinder.
		if (source.RouteDefinition is not null)
		{
			return new List<IRoomExit>();
		}

        HashSet<IRoom> targetSet = NewRoomSet();
        List<IRoom> targetRooms = new();
        foreach (IRoom target in targets)
        {
            if (target == null)
            {
				continue;
			}

			if (target.RouteDefinition is not null)
			{
				return new List<IRoomExit>();
            }

            if (ReferenceEquals(source, target))
            {
                return new List<IRoomExit>();
            }

            if (targetSet.Add(target) && target != null)
            {
                targetRooms.Add(target);
            }
        }

        if (targetSet.Count == 0 || maximumDistance == 0)
        {
            return new List<IRoomExit>();
        }

        Dictionary<IRoom, int> bestDistances = NewRoomDistanceDictionary();
        bestDistances[source] = 0;

        RandomAccessPriorityQueue<double, PathSearchStep> queue = new();
        foreach (IRoomExit exit in source.ExitsFor(null, ignoreLayers))
        {
			if (!suitabilityFunction(exit) || exit.Destination == null ||
				exit.Destination.RouteDefinition is not null)
            {
                continue;
            }

            PathSearchStep step = new()
            {
                Room = exit.Destination,
                Exit = exit,
                Distance = 1
            };

            if (targetSet.Contains(step.Room))
            {
                return BuildPath(step);
            }

            if (bestDistances.TryGetValue(step.Room, out int existingDistance) && existingDistance <= step.Distance)
            {
                continue;
            }

            bestDistances[step.Room] = step.Distance;
            queue.Enqueue(SearchPriority(step.Distance, step.Room, targetRooms), step);
        }

        while (queue.Count > 0)
        {
            PathSearchStep next = queue.DequeueValue();
            if (bestDistances.TryGetValue(next.Room, out int bestDistance) && next.Distance > bestDistance)
            {
                continue;
            }

            if (next.Distance >= maximumDistance)
            {
                continue;
            }

            foreach (IRoomExit exit in next.Room.ExitsFor(null, ignoreLayers))
            {
				if (!suitabilityFunction(exit) || exit.Destination == null ||
					exit.Destination.RouteDefinition is not null)
                {
                    continue;
                }

                int tentativeDistance = next.Distance + 1;
                if (tentativeDistance > maximumDistance)
                {
                    continue;
                }

                if (bestDistances.TryGetValue(exit.Destination, out int existingDistance) &&
                    existingDistance <= tentativeDistance)
                {
                    continue;
                }

                PathSearchStep step = new()
                {
                    Room = exit.Destination,
                    Exit = exit,
                    Parent = next,
                    Distance = tentativeDistance
                };

                if (targetSet.Contains(step.Room))
                {
                    return BuildPath(step);
                }

                bestDistances[step.Room] = step.Distance;
                queue.Enqueue(SearchPriority(step.Distance, step.Room, targetRooms), step);
            }
        }

        return new List<IRoomExit>();
    }

    internal static List<IRoomExit> FindShortestExitPathForPathfinding(IRoom source, IEnumerable<IRoom> targets,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction, bool ignoreLayers)
    {
        return FindShortestExitPath(source, targets, maximumDistance, suitabilityFunction, ignoreLayers);
    }

    private static List<IRoomExit> FindPath(IRoom source, IReadOnlyCollection<IRoom> targets,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction, bool ignoreLayers, PathSearchOptions options)
    {
		if (source?.RouteDefinition is not null || targets?.Any(x => x?.RouteDefinition is not null) == true)
		{
			return new List<IRoomExit>();
		}

		Func<IRoomExit, bool> routeSafeSuitability = exit =>
			exit.Destination?.RouteDefinition is null && suitabilityFunction(exit);
        options ??= PathSearchOptions.Exact;
        if (options.Algorithm == PathSearchAlgorithm.Exact ||
            options.Algorithm == PathSearchAlgorithm.Automatic && maximumDistance < options.HierarchicalThreshold)
        {
			return FindShortestExitPath(source, targets, maximumDistance, routeSafeSuitability, ignoreLayers);
        }

        IPathfindingService service = source?.Gameworld?.ExitManager?.PathfindingService;
        if (service == null)
        {
            return new List<IRoomExit>();
        }

		return service.TryFindLongRangePath(source, targets, maximumDistance, routeSafeSuitability, ignoreLayers,
            options, out IReadOnlyList<IRoomExit> path)
            ? path.ToList()
            : new List<IRoomExit>();
    }

    private static double SearchPriority(int distance, IRoom room, IReadOnlyCollection<IRoom> targetRooms)
    {
        if (room == null || targetRooms == null || targetRooms.Count == 0)
        {
            return distance;
        }

        double bestSquaredDistance = double.MaxValue;
        foreach (IRoom targetRoom in targetRooms)
        {
            double squaredDistance = SquaredDistance(room, targetRoom);
            if (squaredDistance < bestSquaredDistance)
            {
                bestSquaredDistance = squaredDistance;
            }
        }

        if (bestSquaredDistance == double.MaxValue)
        {
            return distance;
        }

        return distance + SameDepthHeuristicTieBreaker * bestSquaredDistance / (bestSquaredDistance + 1.0);
    }

    private static double SquaredDistance(IRoom room1, IRoom room2)
    {
        double x = room2.StoredCoordinates.X - room1.StoredCoordinates.X;
        double y = room2.StoredCoordinates.Y - room1.StoredCoordinates.Y;
        double z = room2.StoredCoordinates.Z - room1.StoredCoordinates.Z;
        return x * x + y * y + z * z;
    }

    private static List<IRoom> FindRoomsInVicinity(IRoom source, uint maximumDistance,
        Func<IRoomExit, bool> suitabilityFunction, bool ignoreLayers)
    {
        List<IRoom> rooms = new();
        if (source == null || suitabilityFunction == null)
        {
            return rooms;
        }

        HashSet<IRoom> seen = NewRoomSet();
        Queue<(IRoom Room, int Distance)> queue = new();
        seen.Add(source);
        rooms.Add(source);
		// This compatibility API has no coordinate-bearing result type. It must not flatten a
		// RouteCell into a single room or enter one as a one-room shortcut; spatial callers use
		// ISpatialPathfinder instead.
		if (source.RouteDefinition is not null)
		{
			return rooms;
		}

        queue.Enqueue((source, 0));

        while (queue.Count > 0)
        {
            (IRoom room, int distance) = queue.Dequeue();
            if (distance >= maximumDistance)
            {
                continue;
            }

            foreach (IRoomExit exit in room.ExitsFor(null, ignoreLayers))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null ||
					exit.Destination.RouteDefinition is not null || !seen.Add(exit.Destination))
                {
                    continue;
                }

                rooms.Add(exit.Destination);
                queue.Enqueue((exit.Destination, distance + 1));
            }
        }

        return rooms;
    }

    private static List<(IRoom Room, int Distance)> FindRoomsAndDistancesInVicinity(IRoom source,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction, bool ignoreLayers)
    {
        List<(IRoom Room, int Distance)> rooms = new();
        if (source == null || suitabilityFunction == null)
        {
            return rooms;
        }

        HashSet<IRoom> seen = NewRoomSet();
        Queue<(IRoom Room, int Distance)> queue = new();
        seen.Add(source);
        rooms.Add((source, 0));
		if (source.RouteDefinition is not null)
		{
			return rooms;
		}

        queue.Enqueue((source, 0));

        while (queue.Count > 0)
        {
            (IRoom room, int distance) = queue.Dequeue();
            if (distance >= maximumDistance)
            {
                continue;
            }

            foreach (IRoomExit exit in room.ExitsFor(null, ignoreLayers))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null ||
					exit.Destination.RouteDefinition is not null || !seen.Add(exit.Destination))
                {
                    continue;
                }

                int exitDistance = distance + 1;
                rooms.Add((exit.Destination, exitDistance));
                queue.Enqueue((exit.Destination, exitDistance));
            }
        }

        return rooms;
    }

    private static IPerceivable FirstTargetInRoom(IRoom room, Func<IPerceivable, bool> targetFunction)
    {
        return room.Perceivables.FirstOrDefault(targetFunction) ??
               (targetFunction(room) ? room : null);
    }

    /// <summary>
    ///     Determines the shortest number of cell exits between the source's current location and the target's current
    ///     location.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="target">The perceivable whose location is the destination cell.</param>
    /// <param name="maximumDistance">
    ///     The inclusive maximum number of exits to traverse. Use small values for hot-path checks; a value of
    ///     <c>0</c> only succeeds when both perceivables are already in the same cell.
    /// </param>
    /// <returns>
    ///     <c>0</c> when both perceivables are in the same cell, a positive exit count for the shortest route, or
    ///     <c>-1</c> when either perceivable has no location or no route exists within <paramref name="maximumDistance" />.
    /// </returns>
    public static int DistanceBetween(this IPerceivable source, IPerceivable target, uint maximumDistance)
    {
        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return -1;
        }

		var usesRouteGeometry = source.Location.RouteDefinition is not null ||
		                        target.Location.RouteDefinition is not null;
		var spatialDistance = usesRouteGeometry ? source.RoomEquivalentDistanceBetween(target) : -1.0;
		if (usesRouteGeometry)
		{
			return spatialDistance >= 0.0 && spatialDistance <= maximumDistance
				? (int)Math.Ceiling(spatialDistance)
				: -1;
		}

		if (source.Location == target.Location)
		{
			return 0;
		}

        List<IRoomExit> path = FindShortestExitPath(source.Location, [target.Location], maximumDistance, _ => true,
            false);
        return path.Count == 0 ? -1 : path.Count;
    }

    /// <summary>
    ///     Determines the number of cell exits between two perceivables using the supplied search options. Exact mode
    ///     preserves the ordinary shortest-path behaviour; automatic and hierarchical modes may use the topology index
    ///     for long routes but still validate every returned exit against live state.
    /// </summary>
    public static int DistanceBetween(this IPerceivable source, IPerceivable target, uint maximumDistance,
        PathSearchOptions options)
    {
        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return -1;
        }

		var usesRouteGeometry = source.Location.RouteDefinition is not null ||
		                        target.Location.RouteDefinition is not null;
		var spatialDistance = usesRouteGeometry ? source.RoomEquivalentDistanceBetween(target) : -1.0;
		if (usesRouteGeometry)
		{
			return spatialDistance >= 0.0 && spatialDistance <= maximumDistance
				? (int)Math.Ceiling(spatialDistance)
				: -1;
		}

		if (source.Location == target.Location)
		{
			return 0;
		}

        List<IRoomExit> path = FindPath(source.Location, [target.Location], maximumDistance, _ => true,
            false, options);
        return path.Count == 0 ? -1 : path.Count;
    }

	/// <summary>
	/// Returns the exact hybrid-path cost in room equivalents. RouteCell longitudinal edges use
	/// their authored metres-per-room scale; ordinary exits retain their normal unit cost.
	/// </summary>
	public static double RoomEquivalentDistanceBetween(this IPerceivable source, IPerceivable target)
	{
		if (source is null || target is null || source.Location is null || target.Location is null)
		{
			return -1.0;
		}

		var pathfinder = source.Gameworld?.ExitManager?.SpatialPathfinder;
		if (pathfinder is null ||
			!pathfinder.TryFindPath(
				RouteSpatialService.Instance.GetEffectiveLocation(source),
				RouteSpatialService.Instance.GetEffectiveLocation(target),
				null,
				false,
				double.PositiveInfinity,
				out var path) ||
			path is null)
		{
			return -1.0;
		}

		return path.RoomEquivalentCost;
	}

    /// <summary>
    ///     Tests whether the target can be reached from the source within a maximum exit count.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="target">The perceivable whose location is the destination cell.</param>
    /// <param name="desiredDistance">The inclusive maximum number of exits that may be traversed.</param>
    /// <returns>
    ///     <see langword="true" /> when <see cref="DistanceBetween" /> finds any route within
    ///     <paramref name="desiredDistance" />, including colocated perceivables at distance <c>0</c>.
    /// </returns>
    public static bool DistanceBetweenLessThanOrEqual(this IPerceivable source, IPerceivable target,
        uint desiredDistance)
    {
        return DistanceBetween(source, target, desiredDistance) != -1;
    }

    /// <summary>
    ///     Finds cells underneath the broad corridor of a flight or projectile path between two perceivables.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start of the flight path.</param>
    /// <param name="target">The perceivable whose location is the end of the flight path.</param>
    /// <param name="maximumDistance">The maximum number of generations to trace before giving up.</param>
    /// <param name="permittedDirections">
    ///     Optional outbound directions to seed the corridor. When omitted, all non-unknown exits from the source cell
    ///     are used. As the search advances, directions opposing the current travel direction are dropped to keep the
    ///     corridor moving generally away from the source.
    /// </param>
    /// <returns>
    ///     Cells under the path, excluding the source and target cells. Closed doors block the corridor unless they can
    ///     be fired through. An empty collection means the target is colocated, invalid, adjacent with no intervening
    ///     cells, or unreachable within the limit.
    /// </returns>
    public static IEnumerable<IRoom> RoomsUnderneathFlight(this IPerceivable source, IPerceivable target,
        uint maximumDistance, IEnumerable<CardinalDirection> permittedDirections = null)
    {
        if (Equals(source?.Location, target?.Location))
        {
            return Enumerable.Empty<IRoom>();
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoom>();
        }

        HashSet<IRoom> locationsConsidered = NewRoomSet();
        locationsConsidered.Add(source.Location);
        List<IRoomExit> exits = source.Location.ExitsFor(null).ToList();
        List<CardinalDirection> permittedDirectionList =
            permittedDirections?.Distinct().ToList() ??
            exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct().ToList();

        List<PolyNode<RoomDirectionSearch>> generationExits =
            new(
                exits
                      .Where(x => permittedDirectionList.Contains(x.OutboundDirection))
                      .Select(x => new PolyNode<RoomDirectionSearch>(new RoomDirectionSearch
                      {
                          Exit = x,
                          PreviousDirection = CardinalDirection.Unknown,
                          PermittedDirections = permittedDirectionList
                      })));

        int generation = 0;
        while (generation++ < maximumDistance)
        {
            List<PolyNode<RoomDirectionSearch>> thisGeneration = generationExits.ToList();
            Dictionary<IRoom, List<PolyNode<RoomDirectionSearch>>> generationDictionary = new();
            generationExits.Clear();
            foreach (PolyNode<RoomDirectionSearch> exit in thisGeneration)
            {
                if (locationsConsidered.Contains(exit.Value.Exit.Destination))
                {
                    if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
                    {
                        foreach (PolyNode<RoomDirectionSearch> node in generationDictionary[exit.Value.Exit.Destination])
                        {
                            if (!exit.Value.PermittedDirections.Contains(node.Value.Exit.OutboundDirection))
                            {
                                continue;
                            }

                            if (!(exit.Value.Exit.Exit.Door?.IsOpen ?? true) &&
                                !exit.Value.Exit.Exit.Door.CanFireThrough)
                            {
                                continue;
                            }

                            exit.Add(node);
                        }
                    }

                    continue;
                }

                locationsConsidered.Add(exit.Value.Exit.Destination);
                generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<RoomDirectionSearch>>();
                foreach (IRoomExit otherExit in exit.Value.Exit.Destination.ExitsFor(null))
                {
                    if (!exit.Value.PermittedDirections.Contains(otherExit.OutboundDirection))
                    {
                        continue;
                    }

                    if (!(otherExit.Exit.Door?.IsOpen ?? true) && !otherExit.Exit.Door.CanFireThrough)
                    {
                        continue;
                    }

                    PolyNode<RoomDirectionSearch> newNode = new(new RoomDirectionSearch
                    {
                        Exit = otherExit,
                        PreviousDirection = exit.Value.Exit.OutboundDirection,
                        PermittedDirections =
                            exit.Value.PermittedDirections.Where(
                                x => !x.IsOpposingDirection(exit.Value.Exit.OutboundDirection)).ToList()
                    });
                    exit.Add(newNode);
                    generationExits.Add(newNode);
                    generationDictionary[exit.Value.Exit.Destination].Add(newNode);
                }
            }

            if (generationDictionary.ContainsKey(target.Location))
            {
                return
                    generationDictionary[target.Location].FirstOrDefault()?
                                                         .Ancestors.Select(x => x.Value.Exit.Destination)
                                                         .Except(target.Location)
                                                         .Reverse()
                                                         .ToList() ?? Enumerable.Empty<IRoom>();
            }
        }

        return Enumerable.Empty<IRoom>();
    }

    /// <summary>
    ///     Returns the intermediate destination cells on the shortest route between two perceivables.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="target">The perceivable whose location is the destination cell.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to traverse.</param>
    /// <returns>
    ///     The cells reached by each exit on the route except the final target cell. The source cell is never included.
    ///     If the target is adjacent, colocated, invalid, or unreachable within the limit, the result is empty.
    /// </returns>
    public static IEnumerable<IRoom> RoomsBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance)
    {
        if (source?.Location == target?.Location)
        {
            return Enumerable.Empty<IRoom>();
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoom>();
        }

        List<IRoomExit> path = FindShortestExitPath(source.Location, [target.Location], maximumDistance, _ => true,
            false);
        return path.Count <= 1
            ? Enumerable.Empty<IRoom>()
            : path.Take(path.Count - 1).Select(x => x.Destination).ToList();
    }

    /// <summary>
    ///     Returns the shortest ordered list of exits between two perceivables without applying movement suitability
    ///     rules.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="target">The perceivable whose location is the destination cell.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to traverse.</param>
    /// <returns>
    ///     The exits to take from source to target. The result is empty when the perceivables are colocated, either
    ///     location is missing, or the target cannot be reached within <paramref name="maximumDistance" />. This helper
    ///     does not test doors, size limits, or other movement rules.
    /// </returns>
    public static IEnumerable<IRoomExit> ExitsBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance)
    {
        if (source?.Location == target?.Location)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindShortestExitPath(source.Location, [target.Location], maximumDistance, _ => true, false);
    }

    /// <summary>
    ///     Returns an ordered exit path between two perceivables using the supplied search options. Hierarchical searches
    ///     are intended for long routes and may return a valid route that is not globally shortest.
    /// </summary>
    public static IEnumerable<IRoomExit> ExitsBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance, PathSearchOptions options)
    {
        if (source?.Location == target?.Location)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindPath(source.Location, [target.Location], maximumDistance, _ => true, false, options);
    }

    /// <summary>
    ///     Returns all cells reachable from a source within a maximum exit count while applying caller-provided exit and
    ///     destination-cell filters.
    /// </summary>
    /// <param name="source">The perceivable whose location is the centre of the vicinity search.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to radiate out from the source.</param>
    /// <param name="cellExitFitnessEvaluator">
    ///     Predicate run for each candidate exit before it is traversed. Return <see langword="false" /> to block that
    ///     route.
    /// </param>
    /// <param name="cellFitnessEvaluator">
    ///     Predicate run for the destination cell of a candidate exit. Return <see langword="false" /> to exclude that
    ///     cell and prevent traversal through it.
    /// </param>
    /// <returns>
    ///     Cells in breadth-first order by distance, always including the source cell when it has a location. An invalid
    ///     source or missing evaluator returns an empty collection.
    /// </returns>
    public static IEnumerable<IRoom> RoomsInVicinity(this IPerceivable source, uint maximumDistance,
        Func<IRoomExit, bool> cellExitFitnessEvaluator,
        Func<IRoom, bool> cellFitnessEvaluator)
    {
        if (source?.Location == null || cellExitFitnessEvaluator == null || cellFitnessEvaluator == null)
        {
            return Enumerable.Empty<IRoom>();
        }

        return FindRoomsInVicinity(source.Location, maximumDistance,
            exit => cellExitFitnessEvaluator(exit) && cellFitnessEvaluator(exit.Destination), false);
    }

    /// <summary>
    ///     Returns all cells within a maximum exit count, optionally applying line-of-effect corner logic.
    /// </summary>
    /// <param name="source">The perceivable whose location is the centre of the vicinity search.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to radiate out from the source.</param>
    /// <param name="respectDoors">
    ///     When <see langword="true" />, closed doors block the search unless the door can be fired through.
    /// </param>
    /// <param name="respectCorners">
    ///     When <see langword="true" />, the search carries permitted outbound directions forward to model effects such
    ///     as aiming or seeing around corners. When <see langword="false" />, the search is a simple breadth-first
    ///     vicinity scan and <paramref name="permittedDirections" /> is ignored.
    /// </param>
    /// <param name="permittedDirections">
    ///     Optional starting directions for the corner-respecting search. If omitted, all non-unknown exits from the
    ///     source are used.
    /// </param>
    /// <param name="straightDirection">
    ///     Optional straight-ahead direction for corner-respecting scans that should not pass through door-capable exits
    ///     when turning away from that direction.
    /// </param>
    /// <returns>
    ///     Cells in distance order, always including the source cell when it has a location. An invalid source returns an
    ///     empty collection.
    /// </returns>
    public static IEnumerable<IRoom> RoomsInVicinity(this IPerceivable source, uint maximumDistance,
        bool respectDoors, bool respectCorners, IEnumerable<CardinalDirection> permittedDirections = null,
        CardinalDirection straightDirection = CardinalDirection.Unknown)
    {
        if (source?.Location == null)
        {
            return Enumerable.Empty<IRoom>();
        }

        if (!respectCorners)
        {
            return FindRoomsInVicinity(source.Location, maximumDistance,
                exit => !respectDoors || exit.Exit.Door?.IsOpen != false || exit.Exit.Door.CanFireThrough, true);
        }

		if (source.Location.RouteDefinition is not null)
		{
			return [source.Location];
		}

        List<IRoom> locationsConsidered = new()
        { source.Location };
        HashSet<IRoom> locationsSeen = NewRoomSet();
        locationsSeen.Add(source.Location);
        List<IRoomExit> exits = source.Location.ExitsFor(null, true).ToList();
        List<CardinalDirection> permittedDirectionList =
            permittedDirections?.Distinct().ToList() ??
            exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct().ToList();

        bool ExitSuitable(IRoomExit exit, IEnumerable<CardinalDirection> directions)
        {
            if (respectCorners && directions.Contains(exit.OutboundDirection) == false)
            {
                return false;
            }

            if (respectCorners && exit.Exit.AcceptsDoor && exit.OutboundDirection != straightDirection
                && straightDirection != CardinalDirection.Unknown)
            {
                return false;
            }

            if (respectDoors && exit.Exit.Door?.IsOpen == false &&
                !exit.Exit.Door.CanFireThrough)
            {
                return false;
            }

            return true;
        }

        List<PolyNode<RoomDirectionSearch>> generationExits =
            new(
                exits
					  .Where(x => x.Destination.RouteDefinition is null)
                      .Where(x => ExitSuitable(x, permittedDirectionList))
                      .Select(x => new PolyNode<RoomDirectionSearch>(new RoomDirectionSearch
                      {
                          Exit = x,
                          PreviousDirection = CardinalDirection.Unknown,
                          PermittedDirections = permittedDirectionList
                      }))
            );

        int generation = 0;
        while (generation++ < maximumDistance)
        {
            List<PolyNode<RoomDirectionSearch>> thisGeneration = generationExits.ToList();
            Dictionary<IRoom, List<PolyNode<RoomDirectionSearch>>> generationDictionary = new();
            generationExits.Clear();
            foreach (PolyNode<RoomDirectionSearch> exit in thisGeneration)
            {
                if (locationsSeen.Contains(exit.Value.Exit.Destination))
                {
                    if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
                    {
                        foreach (PolyNode<RoomDirectionSearch> node in generationDictionary[exit.Value.Exit.Destination])
                        {
                            if (!ExitSuitable(exit.Value.Exit, node.Value.PermittedDirections))
                            {
                                continue;
                            }

                            exit.Add(node);
                        }
                    }

                    continue;
                }

				if (exit.Value.Exit.Destination.RouteDefinition is not null)
				{
					continue;
				}

                locationsSeen.Add(exit.Value.Exit.Destination);
                locationsConsidered.Add(exit.Value.Exit.Destination);
                generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<RoomDirectionSearch>>();
                foreach (IRoomExit otherExit in exit.Value.Exit.Destination.ExitsFor(null))
                {
					if (otherExit.Destination.RouteDefinition is not null)
					{
						continue;
					}

                    if (!ExitSuitable(otherExit, exit.Value.PermittedDirections))
                    {
                        continue;
                    }

                    PolyNode<RoomDirectionSearch> newNode = new(new RoomDirectionSearch
                    {
                        Exit = otherExit,
                        PreviousDirection = exit.Value.Exit.OutboundDirection,
                        PermittedDirections =
                            exit.Value.PermittedDirections.Where(
                                x => !x.IsOpposingDirection(exit.Value.Exit.OutboundDirection)).ToList()
                    });
                    exit.Add(newNode);
                    generationExits.Add(newNode);
                    generationDictionary[exit.Value.Exit.Destination].Add(newNode);
                }
            }
        }

        return locationsConsidered;
    }

    /// <summary>
    ///     Returns all cells reachable from a source with their shortest exit-count distance while applying
    ///     caller-provided exit and destination-cell filters.
    /// </summary>
    /// <param name="source">The perceivable whose location is the centre of the vicinity search.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to radiate out from the source.</param>
    /// <param name="cellExitFitnessEvaluator">
    ///     Predicate run for each candidate exit before it is traversed. Return <see langword="false" /> to block that
    ///     route.
    /// </param>
    /// <param name="cellFitnessEvaluator">
    ///     Predicate run for the destination cell of a candidate exit. Return <see langword="false" /> to exclude that
    ///     cell and prevent traversal through it.
    /// </param>
    /// <returns>
    ///     Tuples of cell and distance in breadth-first order, including the source cell at distance <c>0</c>. An invalid
    ///     source or missing evaluator returns an empty collection.
    /// </returns>
    public static IEnumerable<(IRoom Room, int Distance)> RoomsAndDistancesInVicinity(this IPerceivable source,
        uint maximumDistance,
        Func<IRoomExit, bool> cellExitFitnessEvaluator,
        Func<IRoom, bool> cellFitnessEvaluator)
    {
        if (source?.Location == null || cellExitFitnessEvaluator == null || cellFitnessEvaluator == null)
        {
            return Enumerable.Empty<(IRoom Room, int Distance)>();
        }

        return FindRoomsAndDistancesInVicinity(source.Location, maximumDistance,
            exit => cellExitFitnessEvaluator(exit) && cellFitnessEvaluator(exit.Destination), true);
    }

    /// <summary>
    ///     Returns all cells within a maximum exit count with their distances, optionally applying line-of-effect corner
    ///     logic.
    /// </summary>
    /// <param name="source">The perceivable whose location is the centre of the vicinity search.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to radiate out from the source.</param>
    /// <param name="respectDoors">
    ///     When <see langword="true" />, closed doors block the search unless the door can be fired through.
    /// </param>
    /// <param name="respectCorners">
    ///     When <see langword="true" />, carries permitted directions forward to approximate line-of-effect around
    ///     corners. When <see langword="false" />, this is a simple breadth-first scan and
    ///     <paramref name="permittedDirections" /> is ignored.
    /// </param>
    /// <param name="permittedDirections">
    ///     Optional starting directions for the corner-respecting search. If omitted, all non-unknown exits from the
    ///     source are used.
    /// </param>
    /// <returns>
    ///     Tuples of cell and distance in search order, including the source cell at distance <c>0</c>. An invalid source
    ///     returns an empty collection.
    /// </returns>
    public static IEnumerable<(IRoom Room, int Distance)> RoomsAndDistancesInVicinity(this IPerceivable source,
        uint maximumDistance,
        bool respectDoors, bool respectCorners, IEnumerable<CardinalDirection> permittedDirections = null)
    {
        if (source?.Location == null)
        {
            return Enumerable.Empty<(IRoom Room, int Distance)>();
        }

        if (!respectCorners)
        {
            return FindRoomsAndDistancesInVicinity(source.Location, maximumDistance,
                exit => !respectDoors || exit.Exit.Door?.IsOpen != false || exit.Exit.Door.CanFireThrough, true);
        }

        List<(IRoom, int)> locationsConsidered = new()
        { (source.Location, 0) };
        HashSet<IRoom> locationsSeen = NewRoomSet();
        locationsSeen.Add(source.Location);
        List<IRoomExit> exits = source.Location.ExitsFor(null, true).ToList();
        List<CardinalDirection> permittedDirectionList =
            permittedDirections?.Distinct().ToList() ??
            exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct().ToList();

        bool ExitSuitable(IRoomExit exit, IEnumerable<CardinalDirection> directions)
        {
            if (respectCorners && directions.Contains(exit.OutboundDirection) == false)
            {
                return false;
            }

            if (respectDoors && exit.Exit.Door?.IsOpen == false &&
                !exit.Exit.Door.CanFireThrough)
            {
                return false;
            }

            return true;
        }

        List<PolyNode<RoomDirectionSearch>> generationExits =
            new(
                exits
                      .Where(x => ExitSuitable(x, permittedDirectionList))
                      .Select(x => new PolyNode<RoomDirectionSearch>(new RoomDirectionSearch
                      {
                          Exit = x,
                          PreviousDirection = CardinalDirection.Unknown,
                          PermittedDirections = permittedDirectionList
                      }))
            );

        int generation = 0;
        while (generation++ < maximumDistance)
        {
            List<PolyNode<RoomDirectionSearch>> thisGeneration = generationExits.ToList();
            Dictionary<IRoom, List<PolyNode<RoomDirectionSearch>>> generationDictionary = new();
            generationExits.Clear();
            foreach (PolyNode<RoomDirectionSearch> exit in thisGeneration)
            {
                if (locationsSeen.Contains(exit.Value.Exit.Destination))
                {
                    if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
                    {
                        foreach (PolyNode<RoomDirectionSearch> node in generationDictionary[exit.Value.Exit.Destination])
                        {
                            if (!ExitSuitable(exit.Value.Exit, node.Value.PermittedDirections))
                            {
                                continue;
                            }

                            exit.Add(node);
                        }
                    }

                    continue;
                }

                locationsSeen.Add(exit.Value.Exit.Destination);
                locationsConsidered.Add((exit.Value.Exit.Destination, generation));
                generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<RoomDirectionSearch>>();
                foreach (IRoomExit otherExit in exit.Value.Exit.Destination.ExitsFor(null, true))
                {
                    if (!ExitSuitable(otherExit, exit.Value.PermittedDirections))
                    {
                        continue;
                    }

                    PolyNode<RoomDirectionSearch> newNode = new(new RoomDirectionSearch
                    {
                        Exit = otherExit,
                        PreviousDirection = exit.Value.Exit.OutboundDirection,
                        PermittedDirections =
                            exit.Value.PermittedDirections.Where(
                                x => !x.IsOpposingDirection(exit.Value.Exit.OutboundDirection)).ToList()
                    });
                    exit.Add(newNode);
                    generationExits.Add(newNode);
                    generationDictionary[exit.Value.Exit.Destination].Add(newNode);
                }
            }
        }

        return locationsConsidered;
    }

    /// <summary>
    ///     Applies the door-related flags used by the public boolean <c>PathBetween</c> overload.
    /// </summary>
    /// <param name="exit">The exit being considered for traversal.</param>
    /// <param name="openDoors">Whether closed unlocked doors count as passable.</param>
    /// <param name="pathTransparentDoors">Whether closed transparent doors count as passable.</param>
    /// <param name="pathFireableDoors">Whether closed doors that can be fired through count as passable.</param>
    /// <returns><see langword="true" /> when no door blocks the exit under the supplied flags.</returns>
    private static bool CanTraverse(IRoomExit exit, bool openDoors, bool pathTransparentDoors,
        bool pathFireableDoors)
    {
        if (exit.Exit.Door?.IsOpen ?? true)
        {
            return true;
        }

        if (openDoors && exit.Exit.Door.Locks.All(x => !x.IsLocked))
        {
            return true;
        }

        if (pathTransparentDoors && (exit.Exit.Door?.CanSeeThrough(null) ?? false))
        {
            return true;
        }

        return pathFireableDoors && (exit.Exit.Door?.CanFireThrough ?? false);
    }

    /// <summary>
    ///     Returns the shortest ordered exit path between two perceivables using the built-in door traversal flags.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="target">The perceivable whose location is the destination cell.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to traverse.</param>
    /// <param name="openDoors">
    ///     When <see langword="true" />, closed but unlocked doors are treated as passable because the pathing actor is
    ///     assumed able to open them.
    /// </param>
    /// <param name="pathTransparentDoors">
    ///     When <see langword="true" />, closed doors that can be seen through are treated as passable. This is useful
    ///     for line-of-sight or targeting paths, not normal movement.
    /// </param>
    /// <param name="pathFireableDoors">
    ///     When <see langword="true" />, closed doors that can be fired through are treated as passable. This is used by
    ///     ranged attacks and projectile-style checks.
    /// </param>
    /// <returns>
    ///     The exits to take from source to target, or an empty collection when the target is colocated, invalid, blocked
    ///     by the traversal flags, or beyond <paramref name="maximumDistance" />.
    /// </returns>
    public static IEnumerable<IRoomExit> PathBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance, bool openDoors, bool pathTransparentDoors = false, bool pathFireableDoors = false)
    {
        if (source?.Location == target?.Location ||
            source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindShortestExitPath(source.Location, [target.Location], maximumDistance,
            exit => CanTraverse(exit, openDoors, pathTransparentDoors, pathFireableDoors), true);
    }

    /// <summary>
    ///     Returns an ordered exit path between two perceivables using built-in door flags and opt-in long-range search
    ///     options. Existing callers should continue to use the overload without options when they require exact
    ///     shortest-path semantics.
    /// </summary>
    public static IEnumerable<IRoomExit> PathBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance, bool openDoors, PathSearchOptions options, bool pathTransparentDoors = false,
        bool pathFireableDoors = false)
    {
        if (source?.Location == target?.Location ||
            source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindPath(source.Location, [target.Location], maximumDistance,
            exit => CanTraverse(exit, openDoors, pathTransparentDoors, pathFireableDoors), true, options);
    }

    /// <summary>
    ///     Returns the shortest ordered exit path between two perceivables using a caller-supplied exit suitability
    ///     predicate.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="target">The perceivable whose location is the destination cell.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to traverse.</param>
    /// <param name="suitabilityFunction">
    ///     Predicate run before an exit is traversed. Use this for actor size, door handling, terrain restrictions, AI
    ///     constraints, or other movement rules.
    /// </param>
    /// <returns>
    ///     The exits to take from source to target, or an empty collection when the target is colocated, invalid, blocked
    ///     by <paramref name="suitabilityFunction" />, or beyond <paramref name="maximumDistance" />.
    /// </returns>
    public static IEnumerable<IRoomExit> PathBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction)
    {
        if (source?.Location == target?.Location ||
            source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindShortestExitPath(source.Location, [target.Location], maximumDistance, suitabilityFunction, true);
    }

    /// <summary>
    ///     Returns an ordered exit path between two perceivables using a caller-supplied live suitability predicate and
    ///     opt-in long-range search options. Hierarchical mode uses the topology index only for coarse routing; every
    ///     returned exit still passes <paramref name="suitabilityFunction" /> at query time.
    /// </summary>
    public static IEnumerable<IRoomExit> PathBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction, PathSearchOptions options)
    {
        if (source?.Location == target?.Location ||
            source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindPath(source.Location, [target.Location], maximumDistance, suitabilityFunction, true, options);
    }

    /// <summary>
    ///     Returns the shortest ordered exit path from a source to the nearest reachable target in a set.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="targets">Candidate perceivables; their current locations are used as destination cells.</param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to traverse.</param>
    /// <param name="suitabilityFunction">
    ///     Predicate run before an exit is traversed. It must return <see langword="true" /> for every exit in the
    ///     returned path.
    /// </param>
    /// <returns>
    ///     The exits to the nearest reachable target, or an empty collection when there are no valid targets, a target is
    ///     already in the source cell, all targets are blocked, or all targets are beyond
    ///     <paramref name="maximumDistance" />.
    /// </returns>
    public static IEnumerable<IRoomExit> PathBetween(this IPerceivable source, IEnumerable<IPerceivable> targets,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction)
    {
        if (source?.Location == null || targets == null || suitabilityFunction == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        List<IRoom> targetLocations = targets
                                      .Select(x => x?.Location)
                                      .Where(x => x != null)
                                      .Distinct(RoomReferenceComparer.Instance)
                                      .ToList();
        if (!targetLocations.Any() || targetLocations.Any(x => ReferenceEquals(x, source.Location)))
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindShortestExitPath(source.Location, targetLocations, maximumDistance, suitabilityFunction, true);
    }

    /// <summary>
    ///     Returns an ordered exit path from a source to the nearest reachable target in a set using opt-in search
    ///     options. Hierarchical mode may return any live-valid reachable target route rather than the globally nearest
    ///     target.
    /// </summary>
    public static IEnumerable<IRoomExit> PathBetween(this IPerceivable source, IEnumerable<IPerceivable> targets,
        uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction, PathSearchOptions options)
    {
        if (source?.Location == null || targets == null || suitabilityFunction == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        List<IRoom> targetLocations = targets
                                      .Select(x => x?.Location)
                                      .Where(x => x != null)
                                      .Distinct(RoomReferenceComparer.Instance)
                                      .ToList();
        if (!targetLocations.Any() || targetLocations.Any(x => ReferenceEquals(x, source.Location)))
        {
            return Enumerable.Empty<IRoomExit>();
        }

        return FindPath(source.Location, targetLocations, maximumDistance, suitabilityFunction, true, options);
    }

    /// <summary>
    ///     Searches outward from a perceivable and returns the first matching target plus the shortest path to it.
    /// </summary>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="targetFunction">
    ///     Predicate applied to perceivables in each searched cell and to the cell itself. Use type checks inside this
    ///     predicate when only characters, items, cells, or another perceivable category should match.
    /// </param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to traverse.</param>
    /// <param name="suitabilityFunction">
    ///     Predicate run before an exit is traversed. It should encode movement restrictions such as doors, actor size,
    ///     terrain, or AI-specific rules.
    /// </param>
    /// <returns>
    ///     A tuple containing the nearest matching target and the ordered exit path to it. If the target is in the source
    ///     cell, the path is empty. If no target is found, the target item is <see langword="null" /> and the path is
    ///     empty.
    /// </returns>
    public static Tuple<IPerceivable, IEnumerable<IRoomExit>> AcquireTargetAndPath(this IPerceivable source,
        Func<IPerceivable, bool> targetFunction, uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction)
    {
        if (source?.Location == null || targetFunction == null || suitabilityFunction == null)
        {
            return Tuple.Create(default(IPerceivable), Enumerable.Empty<IRoomExit>());
        }

        IPerceivable homeTarget = FirstTargetInRoom(source.Location, targetFunction);
        if (homeTarget != null)
        {
            return Tuple.Create(homeTarget, Enumerable.Empty<IRoomExit>());
        }

        if (maximumDistance == 0)
        {
            return Tuple.Create(default(IPerceivable), Enumerable.Empty<IRoomExit>());
        }

        HashSet<IRoom> locationsConsidered = NewRoomSet();
        Queue<PathSearchStep> queue = new();
        locationsConsidered.Add(source.Location);
        foreach (IRoomExit exit in source.Location.ExitsFor(null, true))
        {
            if (!suitabilityFunction(exit) || exit.Destination == null || !locationsConsidered.Add(exit.Destination))
            {
                continue;
            }

            queue.Enqueue(new PathSearchStep
            {
                Room = exit.Destination,
                Exit = exit,
                Distance = 1
            });
        }

        while (queue.Count > 0)
        {
            PathSearchStep step = queue.Dequeue();
            IPerceivable exitTarget = FirstTargetInRoom(step.Room, targetFunction);
            if (exitTarget != null)
            {
                return Tuple.Create(exitTarget, BuildPath(step).AsEnumerable());
            }

            if (step.Distance >= maximumDistance)
            {
                continue;
            }

            foreach (IRoomExit exit in step.Room.ExitsFor(null, true))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null ||
                    !locationsConsidered.Add(exit.Destination))
                {
                    continue;
                }

                queue.Enqueue(new PathSearchStep
                {
                    Room = exit.Destination,
                    Exit = exit,
                    Parent = step,
                    Distance = step.Distance + 1
                });
            }
        }

        return Tuple.Create(default(IPerceivable), Enumerable.Empty<IRoomExit>());
    }

    /// <summary>
    ///     Searches outward from a perceivable and returns all matching targets of a specific type with their shortest
    ///     paths.
    /// </summary>
    /// <typeparam name="T">The perceivable type to return, such as <see cref="ICharacter" />, <c>IGameItem</c>, or <see cref="ICell" />.</typeparam>
    /// <param name="source">The perceivable whose location is the start cell.</param>
    /// <param name="targetFunction">
    ///     Predicate applied only to perceivables of type <typeparamref name="T" /> and to cells when <typeparamref name="T" />
    ///     is compatible with <see cref="ICell" />.
    /// </param>
    /// <param name="maximumDistance">The inclusive maximum number of exits to traverse.</param>
    /// <param name="suitabilityFunction">Predicate run before an exit is traversed.</param>
    /// <returns>
    ///     A list of all matching targets found within range. Targets in the source cell have an empty path; other
    ///     targets share the shortest path to their cell. An invalid source or missing predicate returns an empty list.
    /// </returns>
    public static List<(T Target, IEnumerable<IRoomExit> Path)> AcquireAllTargetsAndPaths<T>(this IPerceivable source,
        Func<T, bool> targetFunction, uint maximumDistance, Func<IRoomExit, bool> suitabilityFunction)
        where T : class, IPerceivable
    {
        List<(T Target, IEnumerable<IRoomExit> Path)> list = new();
        if (source?.Location == null || targetFunction == null || suitabilityFunction == null)
        {
            return list;
        }

        if (source.Location is T homeRoom && targetFunction(homeRoom))
        {
            list.Add((homeRoom, Enumerable.Empty<IRoomExit>()));
        }

        List<T> homeTargets = source.Location.Perceivables.OfType<T>().Where(targetFunction).ToList();
        if (homeTargets.Any())
        {
            list.AddRange(homeTargets.Select(x => (x, Enumerable.Empty<IRoomExit>())));
        }

        if (maximumDistance == 0)
        {
            return list;
        }

        HashSet<IRoom> locationsConsidered = NewRoomSet();
        Queue<PathSearchStep> queue = new();
        locationsConsidered.Add(source.Location);
        foreach (IRoomExit exit in source.Location.ExitsFor(null, true))
        {
            if (!suitabilityFunction(exit) || exit.Destination == null || !locationsConsidered.Add(exit.Destination))
            {
                continue;
            }

            queue.Enqueue(new PathSearchStep
            {
                Room = exit.Destination,
                Exit = exit,
                Distance = 1
            });
        }

        while (queue.Count > 0)
        {
            PathSearchStep step = queue.Dequeue();
            List<IRoomExit> path = BuildPath(step);

            if (step.Room is T cellTarget && targetFunction(cellTarget))
            {
                list.Add((cellTarget, path));
            }

            List<T> exitTargets = step.Room.Perceivables.OfType<T>().Where(targetFunction).ToList();
            if (exitTargets.Any())
            {
                list.AddRange(exitTargets.Select(x => (x, path.AsEnumerable())));
            }

            if (step.Distance >= maximumDistance)
            {
                continue;
            }

            foreach (IRoomExit exit in step.Room.ExitsFor(null, true))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null ||
                    !locationsConsidered.Add(exit.Destination))
                {
                    continue;
                }

                queue.Enqueue(new PathSearchStep
                {
                    Room = exit.Destination,
                    Exit = exit,
                    Parent = step,
                    Distance = step.Distance + 1
                });
            }
        }

        return list;
    }

    private class RoomDirectionSearch
    {
        public IRoomExit Exit { get; set; }
        public CardinalDirection PreviousDirection { get; set; }
        public IEnumerable<CardinalDirection> PermittedDirections { get; set; }
    }

    /// <summary>
    ///     For a given IEnumerable of IItems, returns the lowest unused Id
    /// </summary>
    /// <typeparam name="T">Any IItem</typeparam>
    /// <param name="source">An IEnumerable of IItems</param>
    /// <returns>The lowest unused Id</returns>
    public static long NextID<T>(this IEnumerable<T> source) where T : IFrameworkItem
    {
        long priorNumber = 0;

        foreach (long number in source.Select(x => x.Id).OrderBy(n => n))
        {
            long difference = number - priorNumber;

            if (difference > 1)
            {
                return priorNumber + 1;
            }

            priorNumber = number;
        }

        return priorNumber + 1;
    }
}
