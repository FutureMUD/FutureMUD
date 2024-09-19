using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.ThirdPartyCode;

namespace MudSharp.Framework;

public static class PathSearch
{
	public static bool RespectClosedDoors(ICellExit exit)
	{
		return exit.Exit.Door?.IsOpen != false;
	}

	public static bool IncludeUnlockedDoors(ICellExit exit)
	{
		return exit.Exit.Door?.IsOpen != false || exit.Exit.Door.Locks.All(x => !x.IsLocked);
	}

	public static bool IncludeFireableDoors(ICellExit exit)
	{
		return exit.Exit.Door?.IsOpen != false || exit.Exit.Door.CanFireThrough;
	}

	public static bool IgnorePresenceOfDoors(ICellExit exit)
	{
		return true;
	}

	public static Func<ICellExit, bool> PathRespectClosedDoors(ICharacter who)
	{
		return exit => exit.Exit.Door?.IsOpen != false &&
		               who.CurrentContextualSize(SizeContext.CellExit) <= exit.Exit.MaximumSizeToEnter;
	}

	public static Func<ICellExit, bool> PathIncludeUnlockedDoors(ICharacter who)
	{
		return exit => (exit.Exit.Door?.IsOpen != false || exit.Exit.Door.Locks.All(x => !x.IsLocked)) &&
		               who.CurrentContextualSize(SizeContext.CellExit) <= exit.Exit.MaximumSizeToEnter;
	}

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

			foreach (var npc in exit.Origin.Characters.OfType<INPC>().Concat(exit.Destination.Characters.OfType<INPC>()))
			{
				var doorguardAI = npc.AIs.OfType<DoorguardAI>().ToList();
				foreach (var ai in doorguardAI)
				{
					var response = ai.WouldOpen(npc, who, exit);
					switch (response.Response)
					{
						case WouldOpenResponseType.WontOpen:
							continue;

						case WouldOpenResponseType.WillOpenIfSocial:
							var social =
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
	[Obsolete]
	public static int DistanceBetweenObsolete(this IPerceivable source, IPerceivable target, uint maximumDistance)
	{
		if (source == null || target == null || source.Location == null || target.Location == null)
		{
			return -1;
		}

		if (source.Location == target.Location)
		{
			return 0;
		}

		var locationsConsidered = new HashSet<ICell> { source.Location };
		var generationCells = new List<ICell> { source.Location };
		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationCells.ToList();
			generationCells.Clear();
			foreach (var cell in thisGeneration)
			foreach (var exit in cell.ExitsFor(null))
			{
				if (locationsConsidered.Contains(exit.Destination))
				{
					continue;
				}

				if (exit.Destination == target.Location)
				{
					return generation;
				}

				generationCells.Add(exit.Destination);
				locationsConsidered.Add(exit.Destination);
			}
		}

		return -1;
	}

	/// <summary>
	///     Determines the minimum number of exits between source's location and target's location using A* search
	/// </summary>
	/// <param name="source">The source IPerceivable</param>
	/// <param name="target">The target IPerceivable</param>
	/// <param name="maximumDistance">The maximum distance traversed before the algorithm gives up</param>
	/// <returns>The number of exits between the two locations</returns>
	public static int DistanceBetween(this IPerceivable source, IPerceivable target, uint maximumDistance)
	{
		if (source?.Location == target?.Location)
		{
			return 0;
		}

		if (source == null || target == null || source.Location == null || target.Location == null)
		{
			return 0;
		}

		var queue = new RandomAccessPriorityQueue<double, Node<ICellExit>>();
		foreach (var item in source.Location.ExitsFor(null))
		{
			queue.Enqueue(
				Hypotenuse(source.Location.Room.X, item.Destination.Room.X, source.Location.Room.Y,
					item.Destination.Room.Y, source.Location.Room.Z, item.Destination.Room.Z),
				new Node<ICellExit>(item));
		}

		var locationsConsidered = new HashSet<ICell> { source.Location };

		while (true)
		{
			if (queue.Count == 0)
			{
				break;
			}

			var next = queue.DequeueValue();
			if (next.Ancestors.Count() >= maximumDistance)
			{
				continue;
			}

			foreach (var exit in next.Value.Destination.ExitsFor(null))
			{
				if (exit.Destination == target.Location)
				{
					return next.SelfAndAncestors.Count() + 1;
				}

				if (locationsConsidered.Contains(exit.Destination))
					// TODO - re-splice shorter routes when found
				{
					continue;
				}

				locationsConsidered.Add(exit.Destination);
				var node = new Node<ICellExit>(exit);
				node.AddParent(next);
				queue.Enqueue(
					Hypotenuse(exit.Destination.Room.X, target.Location.Room.X, exit.Destination.Room.Y,
						target.Location.Room.Y, exit.Destination.Room.Z, target.Location.Room.Z), node);
			}
		}

		return -1;
	}

	public static bool DistanceBetweenLessThanOrEqual(this IPerceivable source, IPerceivable target,
		uint desiredDistance)
	{
		return DistanceBetween(source, target, desiredDistance) != -1;
	}

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

		var locationsConsidered = new HashSet<ICell> { source.Location };
		var exits = source.Location.ExitsFor(null);
		if (permittedDirections == null)
		{
			permittedDirections = exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct();
		}

		var generationExits =
			new List<PolyNode<CellDirectionSearch>>(
				source.Location.ExitsFor(null)
				      .Where(x => permittedDirections.Contains(x.OutboundDirection))
				      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
				      {
					      Exit = x,
					      PreviousDirection = CardinalDirection.Unknown,
					      PermittedDirections = permittedDirections
				      })));

		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			var generationDictionary = new Dictionary<ICell, List<PolyNode<CellDirectionSearch>>>();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Contains(exit.Value.Exit.Destination))
				{
					if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
					{
						foreach (var node in generationDictionary[exit.Value.Exit.Destination])
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
				foreach (var otherExit in exit.Value.Exit.Destination.ExitsFor(null))
				{
					if (!exit.Value.PermittedDirections.Contains(otherExit.OutboundDirection))
					{
						continue;
					}

					if (!(otherExit.Exit.Door?.IsOpen ?? true) && !otherExit.Exit.Door.CanFireThrough)
					{
						continue;
					}

					var newNode = new PolyNode<CellDirectionSearch>(new CellDirectionSearch
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
	///     Returns all cells which lie between two perceivables exclusive of the cells they are in, using the A* algorithm
	/// </summary>
	/// <param name="source">The source IPerceivable</param>
	/// <param name="target">The target IPerceivable</param>
	/// <param name="maximumDistance">The maximum distance traversed before the algorithm gives up</param>
	/// <returns>A collection of ICells between the two targets, exclusive of the cells they begin in</returns>
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

		var queue = new RandomAccessPriorityQueue<double, Node<ICellExit>>();
		foreach (var item in source.Location.ExitsFor(null))
		{
			queue.Enqueue(Hypotenuse(source.Location.Room, target.Location.Room), new Node<ICellExit>(item));
		}

		var locationsConsidered = new HashSet<ICell> { source.Location };

		while (true)
		{
			if (queue.Count == 0)
			{
				break;
			}

			var next = queue.DequeueValue();
			if (next.Ancestors.Count() >= maximumDistance)
			{
				continue;
			}

			foreach (var exit in next.Value.Destination.ExitsFor(null))
			{
				if (exit.Destination == target.Location)
				{
					return next.SelfAndAncestors.Values().Select(x => x.Destination).Reverse().ToList();
				}

				if (locationsConsidered.Contains(exit.Destination))
					// TODO - re-splice shorter routes when found
				{
					continue;
				}

				locationsConsidered.Add(exit.Destination);
				var node = new Node<ICellExit>(exit);
				node.AddParent(next);
				queue.Enqueue(
					Hypotenuse(exit.Destination.Room.X, target.Location.Room.X, exit.Destination.Room.Y,
						target.Location.Room.Y, exit.Destination.Room.Z, target.Location.Room.Z), node);
			}
		}

		return Enumerable.Empty<ICell>();
	}

	/// <summary>
	///     Returns all Cell Exits which lie between two perceivables, using the A* algorithm
	/// </summary>
	/// <param name="source">The source IPerceivable</param>
	/// <param name="target">The target IPerceivable</param>
	/// <param name="maximumDistance">The maximum distance traversed before the algorithm gives up</param>
	/// <returns>A collection of ICellExits between the two targets</returns>
	[Obsolete]
	public static IEnumerable<ICellExit> ExitsBetweenObsolete(this IPerceivable source, IPerceivable target,
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

		var locationsConsidered = new HashSet<ICell> { source.Location };
		var generationExits =
			new List<Node<ICellExit>>(source.Location.ExitsFor(null).Select(x => new Node<ICellExit>(x)));
		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Contains(exit.Value.Destination))
				{
					continue;
				}

				if (exit.Value.Destination == target.Location)
				{
					return exit.SelfAndAncestors.Values().Reverse().ToList();
				}

				locationsConsidered.Add(exit.Value.Destination);
				foreach (var otherExit in exit.Value.Destination.ExitsFor(null))
				{
					var newNode = new Node<ICellExit>(otherExit);
					exit.Add(newNode);
					generationExits.Add(newNode);
				}
			}
		}

		return Enumerable.Empty<ICellExit>();
	}

	private static double Hypotenuse(double x1, double x2, double y1, double y2, double z1, double z2)
	{
		return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
	}

	private static double Hypotenuse(IRoom room1, IRoom room2)
	{
		return Math.Sqrt((room2.X - room1.X) * (room2.X - room1.X) + (room2.Y - room1.Y) * (room2.Y - room1.Y) +
		                 (room2.Z - room1.Z) * (room2.Z - room1.Z));
	}

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

		var queue = new RandomAccessPriorityQueue<double, Node<ICellExit>>();
		foreach (var item in source.Location.ExitsFor(null))
		{
			queue.Enqueue(Hypotenuse(source.Location.Room, target.Location.Room), new Node<ICellExit>(item));
		}

		var locationsConsidered = new HashSet<ICell> { source.Location };

		while (true)
		{
			if (queue.Count == 0)
			{
				break;
			}

			var next = queue.DequeueValue();
			if (next.Ancestors.Count() >= maximumDistance)
			{
				continue;
			}

			foreach (var exit in next.Value.Destination.ExitsFor(null))
			{
				if (exit.Destination == target.Location)
				{
					var newNode = new Node<ICellExit>(exit);
					next.Add(newNode);
					return newNode.SelfAndAncestors.Values().Reverse().ToList();
				}

				if (locationsConsidered.Contains(exit.Destination))
					// TODO - re-splice shorter routes when found
				{
					continue;
				}

				locationsConsidered.Add(exit.Destination);
				var node = new Node<ICellExit>(exit);
				node.AddParent(next);
				queue.Enqueue(
					Hypotenuse(exit.Destination.Room.X, target.Location.Room.X, exit.Destination.Room.Y,
						target.Location.Room.Y, exit.Destination.Room.Z, target.Location.Room.Z), node);
			}
		}

		return Enumerable.Empty<ICellExit>();
	}

	/// <summary>
	///     Returns all cells within a specified range of a perceivable source, including fitness evaluators to determine
	///     the inclusion or exclusion of any given cell or exit.
	/// </summary>
	/// <param name="source">The source perceivable to radiate out from</param>
	/// <param name="maximumDistance">The maximum distance to search - highly recommended that this number is low</param>
	/// <param name="cellExitFitnessEvaluator">An evaluator function for exits being considered to be traversed</param>
	/// <param name="cellFitnessEvaluator">An evaluator function for cells being considered to be entered</param>
	/// <returns>A list of cells in range sorted by distance from source</returns>
	public static IEnumerable<ICell> CellsInVicinity(this IPerceivable source, uint maximumDistance,
		Func<ICellExit, bool> cellExitFitnessEvaluator,
		Func<ICell, bool> cellFitnessEvaluator)
	{
		var locationsConsidered = new List<ICell> { source.Location };
		var exits = source.Location.ExitsFor(null);

		bool ExitSuitable(ICellExit exit)
		{
			return cellExitFitnessEvaluator(exit) && cellFitnessEvaluator(exit.Destination);
		}

		var generationExits =
			new List<PolyNode<CellDirectionSearch>>(
				source.Location.ExitsFor(null)
				      .Where(x => ExitSuitable(x))
				      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
				      {
					      Exit = x,
					      PreviousDirection = CardinalDirection.Unknown
				      }))
			);

		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			var generationDictionary = new Dictionary<ICell, List<PolyNode<CellDirectionSearch>>>();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Contains(exit.Value.Exit.Destination))
				{
					if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
					{
						foreach (var node in generationDictionary[exit.Value.Exit.Destination])
						{
							if (!ExitSuitable(exit.Value.Exit))
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
				foreach (var otherExit in exit.Value.Exit.Destination.ExitsFor(null))
				{
					if (!ExitSuitable(otherExit))
					{
						continue;
					}

					var newNode = new PolyNode<CellDirectionSearch>(new CellDirectionSearch
					{
						Exit = otherExit,
						PreviousDirection = exit.Value.Exit.OutboundDirection
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
	///     Returns all cells within a specified range of a perceivable source, optionally respecting doors and/or corners,
	///     ordered by distance from source
	/// </summary>
	/// <param name="source">The source perceivable to radiate out from</param>
	/// <param name="maximumDistance">The maximum distance to search - highly recommended that this number is low</param>
	/// <param name="respectDoors">Whether closed opaque doors block the search</param>
	/// <param name="respectCorners">Whether the "corners" algorithm should apply, e.g. when doing "aim" type actions</param>
	/// <param name="permittedDirections">Which directions are allowed, otherwise all are.</param>
	/// <param name="straightDirection">The direction being scanned from in cases where corners are to be respected."</param>
	/// <returns>A list of cells in range sorted by distance from source</returns>
	public static IEnumerable<ICell> CellsInVicinity(this IPerceivable source, uint maximumDistance,
		bool respectDoors, bool respectCorners, IEnumerable<CardinalDirection> permittedDirections = null,
		CardinalDirection straightDirection = CardinalDirection.Unknown)
	{
		var locationsConsidered = new List<ICell> { source.Location };
		var exits = source.Location.ExitsFor(null, true);
		if (permittedDirections == null)
		{
			permittedDirections = exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct();
		}

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

		var generationExits =
			new List<PolyNode<CellDirectionSearch>>(
				source.Location.ExitsFor(null, true)
				      .Where(x => ExitSuitable(x, permittedDirections))
				      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
				      {
					      Exit = x,
					      PreviousDirection = CardinalDirection.Unknown,
					      PermittedDirections = permittedDirections
				      }))
			);

		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			var generationDictionary = new Dictionary<ICell, List<PolyNode<CellDirectionSearch>>>();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Contains(exit.Value.Exit.Destination))
				{
					if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
					{
						foreach (var node in generationDictionary[exit.Value.Exit.Destination])
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

				locationsConsidered.Add(exit.Value.Exit.Destination);
				generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<CellDirectionSearch>>();
				foreach (var otherExit in exit.Value.Exit.Destination.ExitsFor(null))
				{
					if (!ExitSuitable(otherExit, exit.Value.PermittedDirections))
					{
						continue;
					}

					var newNode = new PolyNode<CellDirectionSearch>(new CellDirectionSearch
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

	public static IEnumerable<(ICell Cell, int Distance)> CellsAndDistancesInVicinity(this IPerceivable source,
		uint maximumDistance,
		Func<ICellExit, bool> cellExitFitnessEvaluator,
		Func<ICell, bool> cellFitnessEvaluator)
	{
		var locationsConsidered = new List<(ICell, int)> { (source.Location, 0) };
		var exits = source.Location.ExitsFor(null, true);

		bool ExitSuitable(ICellExit exit)
		{
			return cellExitFitnessEvaluator(exit) && cellFitnessEvaluator(exit.Destination);
		}

		var generationExits =
			new List<PolyNode<CellDirectionSearch>>(
				source.Location.ExitsFor(null, true)
				      .Where(x => ExitSuitable(x))
				      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
				      {
					      Exit = x,
					      PreviousDirection = CardinalDirection.Unknown,
					      PermittedDirections = Constants.CardinalDirections
				      }))
			);

		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			var generationDictionary = new Dictionary<ICell, List<PolyNode<CellDirectionSearch>>>();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Any(x => x.Item1.Equals(exit.Value.Exit.Destination)))
				{
					if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
					{
						foreach (var node in generationDictionary[exit.Value.Exit.Destination])
						{
							if (!ExitSuitable(exit.Value.Exit))
							{
								continue;
							}

							exit.Add(node);
						}
					}

					continue;
				}

				locationsConsidered.Add((exit.Value.Exit.Destination, generation));
				generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<CellDirectionSearch>>();
				foreach (var otherExit in exit.Value.Exit.Destination.ExitsFor(null))
				{
					if (!ExitSuitable(otherExit))
					{
						continue;
					}

					var newNode = new PolyNode<CellDirectionSearch>(new CellDirectionSearch
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

	public static IEnumerable<(ICell Cell, int Distance)> CellsAndDistancesInVicinity(this IPerceivable source,
		uint maximumDistance,
		bool respectDoors, bool respectCorners, IEnumerable<CardinalDirection> permittedDirections = null)
	{
		var locationsConsidered = new List<(ICell, int)> { (source.Location, 0) };
		var exits = source.Location.ExitsFor(null, true);
		if (permittedDirections == null)
		{
			permittedDirections = exits.Select(y => y.OutboundDirection).Except(CardinalDirection.Unknown).Distinct();
		}

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

		var generationExits =
			new List<PolyNode<CellDirectionSearch>>(
				source.Location.ExitsFor(null, true)
				      .Where(x => ExitSuitable(x, permittedDirections))
				      .Select(x => new PolyNode<CellDirectionSearch>(new CellDirectionSearch
				      {
					      Exit = x,
					      PreviousDirection = CardinalDirection.Unknown,
					      PermittedDirections = permittedDirections
				      }))
			);

		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			var generationDictionary = new Dictionary<ICell, List<PolyNode<CellDirectionSearch>>>();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Any(x => x.Item1.Equals(exit.Value.Exit.Destination)))
				{
					if (generationDictionary.ContainsKey(exit.Value.Exit.Destination))
					{
						foreach (var node in generationDictionary[exit.Value.Exit.Destination])
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

				locationsConsidered.Add((exit.Value.Exit.Destination, generation));
				generationDictionary[exit.Value.Exit.Destination] = new List<PolyNode<CellDirectionSearch>>();
				foreach (var otherExit in exit.Value.Exit.Destination.ExitsFor(null, true))
				{
					if (!ExitSuitable(otherExit, exit.Value.PermittedDirections))
					{
						continue;
					}

					var newNode = new PolyNode<CellDirectionSearch>(new CellDirectionSearch
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

	private static bool CanTraverse(ICellExit exit, bool openDoors, bool pathTransparentDoors,
		bool pathFireableDoors)
	{
		if (exit.Exit.Door?.IsOpen ?? true)
		{
			return true;
		}

		if (openDoors & exit.Exit.Door.Locks.All(x => !x.IsLocked))
		{
			return true;
		}

		if (pathTransparentDoors && exit.Exit.Door.CanSeeThrough(null))
		{
			return true;
		}

		return pathFireableDoors && exit.Exit.Door.CanFireThrough;
	}

	/// <summary>
	///     Returns all Cell Exits which lie between two perceivables which can actually be traversed, using the A* algorithm
	/// </summary>
	/// <param name="source">The source IPerceivable</param>
	/// <param name="target">The target IPerceivable</param>
	/// <param name="maximumDistance">The maximum distance traversed before the algorithm gives up</param>
	/// <param name="openDoors">Whether closed but unlocked doors should be considered traversible</param>
	/// <returns>A collection of traversible ICellExits between the two targets</returns>
	public static IEnumerable<ICellExit> PathBetween(this IPerceivable source, IPerceivable target,
		uint maximumDistance, bool openDoors, bool pathTransparentDoors = false, bool pathFireableDoors = false)
	{
		if (source?.Location == target?.Location ||
			source == null || target == null || source.Location == null || target.Location == null)
		{
			return Enumerable.Empty<ICellExit>();
		}

		var queue = new RandomAccessPriorityQueue<double, Node<ICellExit>>();
		var initialGScore = 0.0;
		foreach (var exit in source.Location.ExitsFor(null, true))
		{
			if (!CanTraverse(exit, openDoors, pathTransparentDoors, pathFireableDoors))
			{
				continue;
			}

			var gScore = initialGScore + 1; // Assuming uniform cost
			var hScore = Hypotenuse(exit.Destination.Room, target.Location.Room);
			var fScore = gScore + hScore;
			var node = new Node<ICellExit>(exit) { GScore = gScore };
			queue.Enqueue(fScore, node);
		}

		var locationsConsidered = new Dictionary<ICell, double> { { source.Location, initialGScore } };

		while (queue.Count > 0)
		{
			var next = queue.DequeueValue();
			var currentLocation = next.Value.Destination;

			if (currentLocation == target.Location)
			{
				return next.SelfAndAncestors.Values().Reverse().ToList();
			}

			if (next.GScore >= maximumDistance)
			{
				continue;
			}

			foreach (var exit in currentLocation.ExitsFor(null, true))
			{
				if (!CanTraverse(exit, openDoors, pathTransparentDoors, pathFireableDoors))
				{
					continue;
				}

				var tentativeGScore = next.GScore + 1; // Assuming uniform cost

				if (locationsConsidered.TryGetValue(exit.Destination, out var existingGScore))
				{
					if (tentativeGScore >= existingGScore)
						continue;
				}

				locationsConsidered[exit.Destination] = tentativeGScore;

				var hScore = Hypotenuse(exit.Destination.Room, target.Location.Room);
				var fScore = tentativeGScore + hScore;
				var newNode = new Node<ICellExit>(exit) { GScore = tentativeGScore };
				newNode.AddParent(next);
				queue.Enqueue(fScore, newNode);
			}
		}

		return Enumerable.Empty<ICellExit>();
	}

	/// <summary>
	///     Returns all Cell Exits which lie between two perceivables which can actually be traversed, using the A* algorithm
	/// </summary>
	/// <param name="source">The source IPerceivable</param>
	/// <param name="target">The target IPerceivable</param>
	/// <param name="maximumDistance">The maximum distance traversed before the algorithm gives up</param>
	/// <param name="suitabilityFunction">A function that evaluates the fitness of an individual exit for use in this path</param>
	/// <returns>A collection of traversible ICellExits between the two targets</returns>
	public static IEnumerable<ICellExit> PathBetween(this IPerceivable source, IPerceivable target,
		uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
	{
		if (source?.Location == target?.Location ||
			source == null || target == null || source.Location == null || target.Location == null)
		{
			return Enumerable.Empty<ICellExit>();
		}

		var queue = new RandomAccessPriorityQueue<double, Node<ICellExit>>();
		var initialGScore = 0.0;
		foreach (var exit in source.Location.ExitsFor(null, true))
		{
			if (!suitabilityFunction(exit))
				continue;

			var gScore = initialGScore + 1; // Assuming uniform cost
			var hScore = Hypotenuse(exit.Destination.Room, target.Location.Room);
			var fScore = gScore + hScore;
			var node = new Node<ICellExit>(exit) { GScore = gScore };
			queue.Enqueue(fScore, node);
		}

		var locationsConsidered = new Dictionary<ICell, double> { { source.Location, initialGScore } };

		while (queue.Count > 0)
		{
			var next = queue.DequeueValue();
			var currentLocation = next.Value.Destination;

			if (currentLocation == target.Location)
			{
				return next.SelfAndAncestors.Values().Reverse().ToList();
			}

			if (next.GScore >= maximumDistance)
			{
				continue;
			}

			foreach (var exit in currentLocation.ExitsFor(null, true))
			{
				if (!suitabilityFunction(exit))
					continue;

				var tentativeGScore = next.GScore + 1; // Assuming uniform cost

				if (locationsConsidered.TryGetValue(exit.Destination, out var existingGScore))
				{
					if (tentativeGScore >= existingGScore)
						continue;
				}

				locationsConsidered[exit.Destination] = tentativeGScore;

				var hScore = Hypotenuse(exit.Destination.Room, target.Location.Room);
				var fScore = tentativeGScore + hScore;
				var newNode = new Node<ICellExit>(exit) { GScore = tentativeGScore };
				newNode.AddParent(next);
				queue.Enqueue(fScore, newNode);
			}
		}

		return Enumerable.Empty<ICellExit>();
	}

	/// <summary>
	///     Returns all Cell Exits which lie between a perceivable and the first found target which can actually be traversed, using the A* algorithm
	/// </summary>
	/// <param name="source">The source IPerceivable</param>
	/// <param name="target">The target IPerceivable</param>
	/// <param name="maximumDistance">The maximum distance traversed before the algorithm gives up</param>
	/// <param name="suitabilityFunction">A function that evaluates the fitness of an individual exit for use in this path</param>
	/// <returns>A collection of traversible ICellExits between the two targets</returns>
	public static IEnumerable<ICellExit> PathBetween(this IPerceivable source, IEnumerable<IPerceivable> targets,
		uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
	{
		if (source == null || !targets.Any() || targets.Any(x => x.Location == source.Location))
		{
			return Enumerable.Empty<ICellExit>();
		}

		var queue = new RandomAccessPriorityQueue<double, Node<ICellExit>>();
		foreach (var item in source.Location.ExitsFor(null, true))
		{
			queue.Enqueue(targets.Min(x => Hypotenuse(source.Location.Room, x.Location.Room)),
				new Node<ICellExit>(item));
		}

		var locationsConsidered = new HashSet<ICell> { source.Location };

		while (true)
		{
			if (queue.Count == 0)
			{
				break;
			}

			var next = queue.DequeueValue();
			if (suitabilityFunction(next.Value) && targets.Any(x => x.Location == next.Value.Destination))
			{
				return next.SelfAndAncestors.Values().Reverse().ToList();
			}

			if (next.Ancestors.Count() >= maximumDistance)
			{
				continue;
			}

			foreach (var exit in next.Value.Destination.ExitsFor(null, true))
			{
				if (!suitabilityFunction(exit))
				{
					continue;
				}

				if (targets.Any(x => x.Location == exit.Destination))
				{
					var newNode = new Node<ICellExit>(exit);
					next.Add(newNode);
					return newNode.SelfAndAncestors.Values().Reverse().ToList();
				}

				if (locationsConsidered.Contains(exit.Destination))
					// TODO - re-splice shorter routes when found
				{
					continue;
				}

				locationsConsidered.Add(exit.Destination);
				var node = new Node<ICellExit>(exit);
				node.AddParent(next);
				queue.Enqueue(targets.Min(x => Hypotenuse(exit.Destination.Room, x.Location.Room)), node);
			}
		}

		return Enumerable.Empty<ICellExit>();
	}

	/// <summary>
	///     Searches for a target using the A* algorithm (favouring the nearest) and then returns the target and the path to that target
	/// </summary>
	/// <param name="source">The source IPerceivable</param>
	/// <param name="target">The target IPerceivable, which can be a cell, a character or an item</param>
	/// <param name="targetFunction">A function that evalutes the fitness of an IPerceivable as a target</param>
	/// <param name="maximumDistance">The maximum distance traversed before the algorithm gives up</param>
	/// <param name="suitabilityFunction">A function that evaluates the fitness of an individual exit for use in this path</param>
	/// <returns>A collection of traversible ICellExits between the two targets</returns>
	public static Tuple<IPerceivable, IEnumerable<ICellExit>> AcquireTargetAndPath(this IPerceivable source,
		Func<IPerceivable, bool> targetFunction, uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
	{
		if (source?.Location == null)
		{
			return Tuple.Create(default(IPerceivable), Enumerable.Empty<ICellExit>());
		}

		var homeTarget = source.Location.Perceivables.FirstOrDefault(targetFunction);
		if (homeTarget != null)
		{
			return Tuple.Create(homeTarget, Enumerable.Empty<ICellExit>());
		}

		var locationsConsidered = new HashSet<ICell> { source.Location };
		var generationExits =
			new List<Node<ICellExit>>(source.Location.ExitsFor(null, true).Select(x => new Node<ICellExit>(x)));
		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Contains(exit.Value.Destination))
				{
					continue;
				}

				if (!suitabilityFunction(exit.Value))
				{
					continue;
				}

				var exitTarget = exit.Value.Destination.Perceivables.FirstOrDefault(targetFunction) ??
				                 (targetFunction(exit.Value.Destination) ? exit.Value.Destination : null);
				if (exitTarget != null)
				{
					return Tuple.Create(exitTarget, exit.SelfAndAncestors.Values().Reverse());
				}

				locationsConsidered.Add(exit.Value.Destination);
				foreach (var otherExit in exit.Value.Destination.ExitsFor(null, true))
				{
					var newNode = new Node<ICellExit>(otherExit);
					exit.Add(newNode);
					generationExits.Add(newNode);
				}
			}
		}

		return Tuple.Create(default(IPerceivable), Enumerable.Empty<ICellExit>());
	}

	public static List<(T Target, IEnumerable<ICellExit> Path)> AcquireAllTargetsAndPaths<T>(this IPerceivable source,
		Func<T, bool> targetFunction, uint maximumDistance, Func<ICellExit, bool> suitabilityFunction)
		where T : class, IPerceivable
	{
		var list = new List<(T Target, IEnumerable<ICellExit> Path)>();
		if (source?.Location == null)
		{
			return list;
		}

		var homeTargets = source.Location.Perceivables.Where(x => targetFunction(x as T)).ToList();
		if (homeTargets.Any())
		{
			list.AddRange(homeTargets.Select(x => ((T)x, Enumerable.Empty<ICellExit>())));
		}

		var locationsConsidered = new HashSet<ICell> { source.Location };
		var generationExits =
			new List<Node<ICellExit>>(source.Location.ExitsFor(null, true).Select(x => new Node<ICellExit>(x)));
		var generation = 0;
		while (generation++ < maximumDistance)
		{
			var thisGeneration = generationExits.ToList();
			generationExits.Clear();
			foreach (var exit in thisGeneration)
			{
				if (locationsConsidered.Contains(exit.Value.Destination))
				{
					continue;
				}

				if (!suitabilityFunction(exit.Value))
				{
					continue;
				}

				var exitTarget = exit.Value.Destination.Perceivables.Where(x => targetFunction(x as T)).ToList();
				if (exitTarget.Any())
				{
					var path = (IEnumerable<ICellExit>)exit.SelfAndAncestors.Values().Reverse().ToList();
					list.AddRange(exitTarget.Select(x => ((T)x, path)));
				}

				if (targetFunction(exit.Value.Destination as T))
				{
					var path = (IEnumerable<ICellExit>)exit.SelfAndAncestors.Values().Reverse().ToList();
					list.Add(((T)exit.Value.Destination, path));
				}

				locationsConsidered.Add(exit.Value.Destination);
				foreach (var otherExit in exit.Value.Destination.ExitsFor(null, true))
				{
					var newNode = new Node<ICellExit>(otherExit);
					exit.Add(newNode);
					generationExits.Add(newNode);
				}
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
		long priorNumber = 1;

		foreach (var number in source.Select(x => x.Id).OrderBy(n => n))
		{
			var difference = number - priorNumber;

			if (difference > 1)
			{
				return priorNumber + 1;
			}

			priorNumber = number;
		}

		return priorNumber + 1;
	}
}