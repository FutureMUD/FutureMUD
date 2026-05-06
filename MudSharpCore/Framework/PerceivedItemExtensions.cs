using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Commands.Socials;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.ThirdPartyCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MudSharp.Framework;

public static class PathSearch
{
    /// <summary>
    ///     Allows travel through exits with no door or with a door that is currently open.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns><see langword="true" /> when the exit has no closed door blocking it.</returns>
    public static bool RespectClosedDoors(ICellExit exit)
    {
        return exit.Exit.Door?.IsOpen != false;
    }

    /// <summary>
    ///     Allows travel through open exits and through closed doors that have no locked locks.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns><see langword="true" /> when the exit is open, doorless, or its door is closed but unlocked.</returns>
    public static bool IncludeUnlockedDoors(ICellExit exit)
    {
        return exit.Exit.Door?.IsOpen != false || exit.Exit.Door.Locks.All(x => !x.IsLocked);
    }

    /// <summary>
    ///     Allows travel or line-style pathing through open exits and through closed doors that can be fired through.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns><see langword="true" /> when the exit is open, doorless, or the door permits fire through it.</returns>
    public static bool IncludeFireableDoors(ICellExit exit)
    {
        return exit.Exit.Door?.IsOpen != false || exit.Exit.Door.CanFireThrough;
    }

    /// <summary>
    ///     Allows every exit regardless of door state.
    /// </summary>
    /// <param name="exit">The exit being considered by a path search.</param>
    /// <returns>Always <see langword="true" />.</returns>
    public static bool IgnorePresenceOfDoors(ICellExit exit)
    {
        return true;
    }

    /// <summary>
    ///     Builds a suitability function that ignores doors but rejects exits too small for a character.
    /// </summary>
    /// <param name="who">The character whose current cell-exit size is compared to each exit.</param>
    /// <returns>A predicate suitable for <c>PathBetween</c> overloads that accept an exit suitability function.</returns>
    public static Func<ICellExit, bool> PathIgnoreDoors(ICharacter who)
    {
        return exit => who.CurrentContextualSize(SizeContext.CellExit) <= exit.Exit.MaximumSizeToEnter;
    }

    /// <summary>
    ///     Builds a suitability function that rejects closed doors and exits too small for a character.
    /// </summary>
    /// <param name="who">The character whose current cell-exit size is compared to each exit.</param>
    /// <returns>A predicate suitable for ordinary movement path searches.</returns>
    public static Func<ICellExit, bool> PathRespectClosedDoors(ICharacter who)
    {
        return exit => exit.Exit.Door?.IsOpen != false &&
                       who.CurrentContextualSize(SizeContext.CellExit) <= exit.Exit.MaximumSizeToEnter;
    }

    /// <summary>
    ///     Builds a suitability function that permits doorless/open exits, closed unlocked doors, and exits large enough
    ///     for a character.
    /// </summary>
    /// <param name="who">The character whose current cell-exit size is compared to each exit.</param>
    /// <returns>A predicate suitable for path searches that assume the actor can open unlocked doors.</returns>
    public static Func<ICellExit, bool> PathIncludeUnlockedDoors(ICharacter who)
    {
        return exit => (exit.Exit.Door?.IsOpen != false || exit.Exit.Door.Locks.All(x => !x.IsLocked)) &&
                       who.CurrentContextualSize(SizeContext.CellExit) <= exit.Exit.MaximumSizeToEnter;
    }

    /// <summary>
    ///     Builds a suitability function that permits exits a character can traverse directly, open, or reasonably have
    ///     opened by eligible doorguard NPCs.
    /// </summary>
    /// <param name="who">The character whose size, body capabilities, socials, and local NPC helpers are considered.</param>
    /// <returns>
    ///     A predicate suitable for AI and movement planning where closed doors may be passable through interaction.
    /// </returns>
    public static Func<ICellExit, bool> PathIncludeUnlockableDoors(ICharacter who)
    {
        return exit =>
        {
            if (who.CurrentContextualSize(SizeContext.CellExit) > exit.Exit.MaximumSizeToEnter)
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

    private sealed class CellReferenceComparer : IEqualityComparer<ICell>
    {
        public static readonly CellReferenceComparer Instance = new();

        public bool Equals(ICell x, ICell y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(ICell obj)
        {
            return obj == null ? 0 : RuntimeHelpers.GetHashCode(obj);
        }
    }

    private sealed class PathSearchStep
    {
        public ICell Cell { get; init; }
        public ICellExit Exit { get; init; }
        public PathSearchStep Parent { get; init; }
        public int Distance { get; init; }
    }

    private static HashSet<ICell> NewCellSet()
    {
        return new HashSet<ICell>(CellReferenceComparer.Instance);
    }

    private static Dictionary<ICell, int> NewCellDistanceDictionary()
    {
        return new Dictionary<ICell, int>(CellReferenceComparer.Instance);
    }

    private static List<ICellExit> BuildPath(PathSearchStep step)
    {
        List<ICellExit> path = new(step.Distance);
        PathSearchStep current = step;
        while (current != null)
        {
            path.Add(current.Exit);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    private static List<ICellExit> FindShortestExitPath(ICell source, IEnumerable<ICell> targets,
        uint maximumDistance, Func<ICellExit, bool> suitabilityFunction, bool ignoreLayers)
    {
        if (source == null || targets == null || suitabilityFunction == null)
        {
            return new List<ICellExit>();
        }

        HashSet<ICell> targetSet = NewCellSet();
        List<IRoom> targetRooms = new();
        foreach (ICell target in targets)
        {
            if (target == null)
            {
                continue;
            }

            if (ReferenceEquals(source, target))
            {
                return new List<ICellExit>();
            }

            if (targetSet.Add(target) && target.Room != null)
            {
                targetRooms.Add(target.Room);
            }
        }

        if (targetSet.Count == 0 || maximumDistance == 0)
        {
            return new List<ICellExit>();
        }

        Dictionary<ICell, int> bestDistances = NewCellDistanceDictionary();
        bestDistances[source] = 0;

        RandomAccessPriorityQueue<double, PathSearchStep> queue = new();
        foreach (ICellExit exit in source.ExitsFor(null, ignoreLayers))
        {
            if (!suitabilityFunction(exit) || exit.Destination == null)
            {
                continue;
            }

            PathSearchStep step = new()
            {
                Cell = exit.Destination,
                Exit = exit,
                Distance = 1
            };

            if (targetSet.Contains(step.Cell))
            {
                return BuildPath(step);
            }

            if (bestDistances.TryGetValue(step.Cell, out int existingDistance) && existingDistance <= step.Distance)
            {
                continue;
            }

            bestDistances[step.Cell] = step.Distance;
            queue.Enqueue(SearchPriority(step.Distance, step.Cell, targetRooms), step);
        }

        while (queue.Count > 0)
        {
            PathSearchStep next = queue.DequeueValue();
            if (bestDistances.TryGetValue(next.Cell, out int bestDistance) && next.Distance > bestDistance)
            {
                continue;
            }

            if (next.Distance >= maximumDistance)
            {
                continue;
            }

            foreach (ICellExit exit in next.Cell.ExitsFor(null, ignoreLayers))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null)
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
                    Cell = exit.Destination,
                    Exit = exit,
                    Parent = next,
                    Distance = tentativeDistance
                };

                if (targetSet.Contains(step.Cell))
                {
                    return BuildPath(step);
                }

                bestDistances[step.Cell] = step.Distance;
                queue.Enqueue(SearchPriority(step.Distance, step.Cell, targetRooms), step);
            }
        }

        return new List<ICellExit>();
    }

    private static double SearchPriority(int distance, ICell cell, IReadOnlyCollection<IRoom> targetRooms)
    {
        if (cell?.Room == null || targetRooms == null || targetRooms.Count == 0)
        {
            return distance;
        }

        double bestSquaredDistance = double.MaxValue;
        foreach (IRoom room in targetRooms)
        {
            double squaredDistance = SquaredDistance(cell.Room, room);
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
        double x = room2.X - room1.X;
        double y = room2.Y - room1.Y;
        double z = room2.Z - room1.Z;
        return x * x + y * y + z * z;
    }

    private static List<ICell> FindCellsInVicinity(ICell source, uint maximumDistance,
        Func<ICellExit, bool> suitabilityFunction, bool ignoreLayers)
    {
        List<ICell> cells = new();
        if (source == null || suitabilityFunction == null)
        {
            return cells;
        }

        HashSet<ICell> seen = NewCellSet();
        Queue<(ICell Cell, int Distance)> queue = new();
        seen.Add(source);
        cells.Add(source);
        queue.Enqueue((source, 0));

        while (queue.Count > 0)
        {
            (ICell cell, int distance) = queue.Dequeue();
            if (distance >= maximumDistance)
            {
                continue;
            }

            foreach (ICellExit exit in cell.ExitsFor(null, ignoreLayers))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null || !seen.Add(exit.Destination))
                {
                    continue;
                }

                cells.Add(exit.Destination);
                queue.Enqueue((exit.Destination, distance + 1));
            }
        }

        return cells;
    }

    private static List<(ICell Cell, int Distance)> FindCellsAndDistancesInVicinity(ICell source,
        uint maximumDistance, Func<ICellExit, bool> suitabilityFunction, bool ignoreLayers)
    {
        List<(ICell Cell, int Distance)> cells = new();
        if (source == null || suitabilityFunction == null)
        {
            return cells;
        }

        HashSet<ICell> seen = NewCellSet();
        Queue<(ICell Cell, int Distance)> queue = new();
        seen.Add(source);
        cells.Add((source, 0));
        queue.Enqueue((source, 0));

        while (queue.Count > 0)
        {
            (ICell cell, int distance) = queue.Dequeue();
            if (distance >= maximumDistance)
            {
                continue;
            }

            foreach (ICellExit exit in cell.ExitsFor(null, ignoreLayers))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null || !seen.Add(exit.Destination))
                {
                    continue;
                }

                int exitDistance = distance + 1;
                cells.Add((exit.Destination, exitDistance));
                queue.Enqueue((exit.Destination, exitDistance));
            }
        }

        return cells;
    }

    private static IPerceivable FirstTargetInCell(ICell cell, Func<IPerceivable, bool> targetFunction)
    {
        return cell.Perceivables.FirstOrDefault(targetFunction) ??
               (targetFunction(cell) ? cell : null);
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
        if (source?.Location == target?.Location)
        {
            return 0;
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return -1;
        }

        List<ICellExit> path = FindShortestExitPath(source.Location, [target.Location], maximumDistance, _ => true,
            false);
        return path.Count == 0 ? -1 : path.Count;
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
    public static IEnumerable<ICell> CellsUnderneathFlight(this IPerceivable source, IPerceivable target,
        uint maximumDistance, IEnumerable<CardinalDirection> permittedDirections = null)
    {
        if (Equals(source?.Location, target?.Location))
        {
            return Enumerable.Empty<ICell>();
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<ICell>();
        }

        HashSet<ICell> locationsConsidered = NewCellSet();
        locationsConsidered.Add(source.Location);
        List<ICellExit> exits = source.Location.ExitsFor(null).ToList();
        List<CardinalDirection> permittedDirectionList =
            permittedDirections?.Distinct().ToList() ??
            exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct().ToList();

        List<PolyNode<CellDirectionSearch>> generationExits =
            new(
                exits
                      .Where(x => permittedDirectionList.Contains(x.OutboundDirection))
                      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
                      {
                          Exit = x,
                          PreviousDirection = CardinalDirection.Unknown,
                          PermittedDirections = permittedDirectionList
                      })));

        int generation = 0;
        while (generation++ < maximumDistance)
        {
            List<PolyNode<CellDirectionSearch>> thisGeneration = generationExits.ToList();
            Dictionary<ICell, List<PolyNode<CellDirectionSearch>>> generationDictionary = new();
            generationExits.Clear();
            foreach (PolyNode<CellDirectionSearch> exit in thisGeneration)
            {
                if (locationsConsidered.Contains(exit.Value.Exit.Destination))
                {
                    if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
                    {
                        foreach (PolyNode<CellDirectionSearch> node in generationDictionary[exit.Value.Exit.Destination])
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
                generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<CellDirectionSearch>>();
                foreach (ICellExit otherExit in exit.Value.Exit.Destination.ExitsFor(null))
                {
                    if (!exit.Value.PermittedDirections.Contains(otherExit.OutboundDirection))
                    {
                        continue;
                    }

                    if (!(otherExit.Exit.Door?.IsOpen ?? true) && !otherExit.Exit.Door.CanFireThrough)
                    {
                        continue;
                    }

                    PolyNode<CellDirectionSearch> newNode = new(new CellDirectionSearch
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
                                                         .ToList() ?? Enumerable.Empty<ICell>();
            }
        }

        return Enumerable.Empty<ICell>();
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
    public static IEnumerable<ICell> CellsBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance)
    {
        if (source?.Location == target?.Location)
        {
            return Enumerable.Empty<ICell>();
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<ICell>();
        }

        List<ICellExit> path = FindShortestExitPath(source.Location, [target.Location], maximumDistance, _ => true,
            false);
        return path.Count <= 1
            ? Enumerable.Empty<ICell>()
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
    public static IEnumerable<ICellExit> ExitsBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance)
    {
        if (source?.Location == target?.Location)
        {
            return Enumerable.Empty<ICellExit>();
        }

        if (source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<ICellExit>();
        }

        return FindShortestExitPath(source.Location, [target.Location], maximumDistance, _ => true, false);
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
    public static IEnumerable<ICell> CellsInVicinity(this IPerceivable source, uint maximumDistance,
        Func<ICellExit, bool> cellExitFitnessEvaluator,
        Func<ICell, bool> cellFitnessEvaluator)
    {
        if (source?.Location == null || cellExitFitnessEvaluator == null || cellFitnessEvaluator == null)
        {
            return Enumerable.Empty<ICell>();
        }

        return FindCellsInVicinity(source.Location, maximumDistance,
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
    public static IEnumerable<ICell> CellsInVicinity(this IPerceivable source, uint maximumDistance,
        bool respectDoors, bool respectCorners, IEnumerable<CardinalDirection> permittedDirections = null,
        CardinalDirection straightDirection = CardinalDirection.Unknown)
    {
        if (source?.Location == null)
        {
            return Enumerable.Empty<ICell>();
        }

        if (!respectCorners)
        {
            return FindCellsInVicinity(source.Location, maximumDistance,
                exit => !respectDoors || exit.Exit.Door?.IsOpen != false || exit.Exit.Door.CanFireThrough, true);
        }

        List<ICell> locationsConsidered = new()
        { source.Location };
        HashSet<ICell> locationsSeen = NewCellSet();
        locationsSeen.Add(source.Location);
        List<ICellExit> exits = source.Location.ExitsFor(null, true).ToList();
        List<CardinalDirection> permittedDirectionList =
            permittedDirections?.Distinct().ToList() ??
            exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct().ToList();

        bool ExitSuitable(ICellExit exit, IEnumerable<CardinalDirection> directions)
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

        List<PolyNode<CellDirectionSearch>> generationExits =
            new(
                exits
                      .Where(x => ExitSuitable(x, permittedDirectionList))
                      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
                      {
                          Exit = x,
                          PreviousDirection = CardinalDirection.Unknown,
                          PermittedDirections = permittedDirectionList
                      }))
            );

        int generation = 0;
        while (generation++ < maximumDistance)
        {
            List<PolyNode<CellDirectionSearch>> thisGeneration = generationExits.ToList();
            Dictionary<ICell, List<PolyNode<CellDirectionSearch>>> generationDictionary = new();
            generationExits.Clear();
            foreach (PolyNode<CellDirectionSearch> exit in thisGeneration)
            {
                if (locationsSeen.Contains(exit.Value.Exit.Destination))
                {
                    if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
                    {
                        foreach (PolyNode<CellDirectionSearch> node in generationDictionary[exit.Value.Exit.Destination])
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
                locationsConsidered.Add(exit.Value.Exit.Destination);
                generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<CellDirectionSearch>>();
                foreach (ICellExit otherExit in exit.Value.Exit.Destination.ExitsFor(null))
                {
                    if (!ExitSuitable(otherExit, exit.Value.PermittedDirections))
                    {
                        continue;
                    }

                    PolyNode<CellDirectionSearch> newNode = new(new CellDirectionSearch
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
    public static IEnumerable<(ICell Cell, int Distance)> CellsAndDistancesInVicinity(this IPerceivable source,
        uint maximumDistance,
        Func<ICellExit, bool> cellExitFitnessEvaluator,
        Func<ICell, bool> cellFitnessEvaluator)
    {
        if (source?.Location == null || cellExitFitnessEvaluator == null || cellFitnessEvaluator == null)
        {
            return Enumerable.Empty<(ICell Cell, int Distance)>();
        }

        return FindCellsAndDistancesInVicinity(source.Location, maximumDistance,
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
    public static IEnumerable<(ICell Cell, int Distance)> CellsAndDistancesInVicinity(this IPerceivable source,
        uint maximumDistance,
        bool respectDoors, bool respectCorners, IEnumerable<CardinalDirection> permittedDirections = null)
    {
        if (source?.Location == null)
        {
            return Enumerable.Empty<(ICell Cell, int Distance)>();
        }

        if (!respectCorners)
        {
            return FindCellsAndDistancesInVicinity(source.Location, maximumDistance,
                exit => !respectDoors || exit.Exit.Door?.IsOpen != false || exit.Exit.Door.CanFireThrough, true);
        }

        List<(ICell, int)> locationsConsidered = new()
        { (source.Location, 0) };
        HashSet<ICell> locationsSeen = NewCellSet();
        locationsSeen.Add(source.Location);
        List<ICellExit> exits = source.Location.ExitsFor(null, true).ToList();
        List<CardinalDirection> permittedDirectionList =
            permittedDirections?.Distinct().ToList() ??
            exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct().ToList();

        bool ExitSuitable(ICellExit exit, IEnumerable<CardinalDirection> directions)
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

        List<PolyNode<CellDirectionSearch>> generationExits =
            new(
                exits
                      .Where(x => ExitSuitable(x, permittedDirectionList))
                      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
                      {
                          Exit = x,
                          PreviousDirection = CardinalDirection.Unknown,
                          PermittedDirections = permittedDirectionList
                      }))
            );

        int generation = 0;
        while (generation++ < maximumDistance)
        {
            List<PolyNode<CellDirectionSearch>> thisGeneration = generationExits.ToList();
            Dictionary<ICell, List<PolyNode<CellDirectionSearch>>> generationDictionary = new();
            generationExits.Clear();
            foreach (PolyNode<CellDirectionSearch> exit in thisGeneration)
            {
                if (locationsSeen.Contains(exit.Value.Exit.Destination))
                {
                    if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
                    {
                        foreach (PolyNode<CellDirectionSearch> node in generationDictionary[exit.Value.Exit.Destination])
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
                generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<CellDirectionSearch>>();
                foreach (ICellExit otherExit in exit.Value.Exit.Destination.ExitsFor(null, true))
                {
                    if (!ExitSuitable(otherExit, exit.Value.PermittedDirections))
                    {
                        continue;
                    }

                    PolyNode<CellDirectionSearch> newNode = new(new CellDirectionSearch
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
    private static bool CanTraverse(ICellExit exit, bool openDoors, bool pathTransparentDoors,
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
    public static IEnumerable<ICellExit> PathBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance, bool openDoors, bool pathTransparentDoors = false, bool pathFireableDoors = false)
    {
        if (source?.Location == target?.Location ||
            source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<ICellExit>();
        }

        return FindShortestExitPath(source.Location, [target.Location], maximumDistance,
            exit => CanTraverse(exit, openDoors, pathTransparentDoors, pathFireableDoors), true);
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
    public static IEnumerable<ICellExit> PathBetween(this IPerceivable source, IPerceivable target,
        uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
    {
        if (source?.Location == target?.Location ||
            source == null || target == null || source.Location == null || target.Location == null)
        {
            return Enumerable.Empty<ICellExit>();
        }

        return FindShortestExitPath(source.Location, [target.Location], maximumDistance, suitabilityFunction, true);
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
    public static IEnumerable<ICellExit> PathBetween(this IPerceivable source, IEnumerable<IPerceivable> targets,
        uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
    {
        if (source?.Location == null || targets == null || suitabilityFunction == null)
        {
            return Enumerable.Empty<ICellExit>();
        }

        List<ICell> targetLocations = targets
                                      .Select(x => x?.Location)
                                      .Where(x => x != null)
                                      .Distinct(CellReferenceComparer.Instance)
                                      .ToList();
        if (!targetLocations.Any() || targetLocations.Any(x => ReferenceEquals(x, source.Location)))
        {
            return Enumerable.Empty<ICellExit>();
        }

        return FindShortestExitPath(source.Location, targetLocations, maximumDistance, suitabilityFunction, true);
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
    public static Tuple<IPerceivable, IEnumerable<ICellExit>> AcquireTargetAndPath(this IPerceivable source,
        Func<IPerceivable, bool> targetFunction, uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
    {
        if (source?.Location == null || targetFunction == null || suitabilityFunction == null)
        {
            return Tuple.Create(default(IPerceivable), Enumerable.Empty<ICellExit>());
        }

        IPerceivable homeTarget = FirstTargetInCell(source.Location, targetFunction);
        if (homeTarget != null)
        {
            return Tuple.Create(homeTarget, Enumerable.Empty<ICellExit>());
        }

        if (maximumDistance == 0)
        {
            return Tuple.Create(default(IPerceivable), Enumerable.Empty<ICellExit>());
        }

        HashSet<ICell> locationsConsidered = NewCellSet();
        Queue<PathSearchStep> queue = new();
        locationsConsidered.Add(source.Location);
        foreach (ICellExit exit in source.Location.ExitsFor(null, true))
        {
            if (!suitabilityFunction(exit) || exit.Destination == null || !locationsConsidered.Add(exit.Destination))
            {
                continue;
            }

            queue.Enqueue(new PathSearchStep
            {
                Cell = exit.Destination,
                Exit = exit,
                Distance = 1
            });
        }

        while (queue.Count > 0)
        {
            PathSearchStep step = queue.Dequeue();
            IPerceivable exitTarget = FirstTargetInCell(step.Cell, targetFunction);
            if (exitTarget != null)
            {
                return Tuple.Create(exitTarget, BuildPath(step).AsEnumerable());
            }

            if (step.Distance >= maximumDistance)
            {
                continue;
            }

            foreach (ICellExit exit in step.Cell.ExitsFor(null, true))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null ||
                    !locationsConsidered.Add(exit.Destination))
                {
                    continue;
                }

                queue.Enqueue(new PathSearchStep
                {
                    Cell = exit.Destination,
                    Exit = exit,
                    Parent = step,
                    Distance = step.Distance + 1
                });
            }
        }

        return Tuple.Create(default(IPerceivable), Enumerable.Empty<ICellExit>());
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
    public static List<(T Target, IEnumerable<ICellExit> Path)> AcquireAllTargetsAndPaths<T>(this IPerceivable source,
        Func<T, bool> targetFunction, uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
        where T : class, IPerceivable
    {
        List<(T Target, IEnumerable<ICellExit> Path)> list = new();
        if (source?.Location == null || targetFunction == null || suitabilityFunction == null)
        {
            return list;
        }

        if (source.Location is T homeCell && targetFunction(homeCell))
        {
            list.Add((homeCell, Enumerable.Empty<ICellExit>()));
        }

        List<T> homeTargets = source.Location.Perceivables.OfType<T>().Where(targetFunction).ToList();
        if (homeTargets.Any())
        {
            list.AddRange(homeTargets.Select(x => (x, Enumerable.Empty<ICellExit>())));
        }

        if (maximumDistance == 0)
        {
            return list;
        }

        HashSet<ICell> locationsConsidered = NewCellSet();
        Queue<PathSearchStep> queue = new();
        locationsConsidered.Add(source.Location);
        foreach (ICellExit exit in source.Location.ExitsFor(null, true))
        {
            if (!suitabilityFunction(exit) || exit.Destination == null || !locationsConsidered.Add(exit.Destination))
            {
                continue;
            }

            queue.Enqueue(new PathSearchStep
            {
                Cell = exit.Destination,
                Exit = exit,
                Distance = 1
            });
        }

        while (queue.Count > 0)
        {
            PathSearchStep step = queue.Dequeue();
            List<ICellExit> path = BuildPath(step);

            if (step.Cell is T cellTarget && targetFunction(cellTarget))
            {
                list.Add((cellTarget, path));
            }

            List<T> exitTargets = step.Cell.Perceivables.OfType<T>().Where(targetFunction).ToList();
            if (exitTargets.Any())
            {
                list.AddRange(exitTargets.Select(x => (x, path.AsEnumerable())));
            }

            if (step.Distance >= maximumDistance)
            {
                continue;
            }

            foreach (ICellExit exit in step.Cell.ExitsFor(null, true))
            {
                if (!suitabilityFunction(exit) || exit.Destination == null ||
                    !locationsConsidered.Add(exit.Destination))
                {
                    continue;
                }

                queue.Enqueue(new PathSearchStep
                {
                    Cell = exit.Destination,
                    Exit = exit,
                    Parent = step,
                    Distance = step.Distance + 1
                });
            }
        }

        return list;
    }

    private class CellDirectionSearch
    {
        public ICellExit Exit { get; set; }
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
