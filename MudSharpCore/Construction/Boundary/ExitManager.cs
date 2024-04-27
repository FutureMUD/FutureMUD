using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Construction.Boundary;

public class ExitManager : IExitManager, IHaveFuturemud
{
	protected readonly CollectionDictionary<(ICell Cell, ICellOverlay Overlay), IExit> CellExitDictionary =
		new();

	protected readonly DictionaryWithDefault<long, IExit> MasterExitList = new();

	public ExitManager(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	public IFuturemud Gameworld { get; protected set; }

	/// <summary>
	///     Called the first time that an exit for a particular cell and/or overlay is requested. Initialises the cell in the
	///     manager.
	/// </summary>
	/// <param name="cell">The cell which is being initialised</param>
	/// <param name="overlay">The overlay which is being initialised (if not specified, initialise all overlays)</param>
	public void InitialiseCell(ICell cell, ICellOverlay overlay)
	{
		if (overlay == null)
		{
			foreach (var item in cell.Overlays)
			{
				InitialiseCell(cell, item);
			}

			return;
		}

		if (CellExitDictionary.ContainsKey((cell, overlay)))
		{
			return;
		}

		using (new FMDB())
		{
			var exits =
				FMDB.Context.Exits.Where(
					    x => (x.CellId1 == cell.Id || x.CellId2 == cell.Id) && overlay.ExitIDs.Contains(x.Id))
				    .ToList();
			var exitList = new List<IExit>();
			foreach (var exit in exits)
			{
				IExit newExit;
				if (!MasterExitList.ContainsKey(exit.Id))
				{
					newExit = new Exit(exit, Gameworld);
					MasterExitList.Add(exit.Id, newExit);
					newExit.PostLoadTasks(exit);
				}
				else
				{
					newExit = MasterExitList[exit.Id];
				}

				exitList.Add(newExit);
			}

			CellExitDictionary.AddRange((cell, overlay), exitList);
		}

		cell.OnExitsInitialised();
	}

	#region IExitManager Implementation

	public ICellExit GetExit(ICell cell, CardinalDirection direction, IPerceiver voyeur)
	{
		var overlay = cell.GetOverlayFor(voyeur);
		if (overlay == null)
		{
			overlay = cell.CurrentOverlay;
		}

		InitialiseCell(cell, overlay);

		var exit =
			CellExitDictionary[(cell, overlay)].Where(x => overlay.ExitIDs.Contains(x.Id))
			                                               .FirstOrDefault(x =>
				                                               x.CellExitFor(cell).OutboundDirection == direction);
		var cellExit = exit?.CellExitFor(cell);
		if (cellExit?.MovementTransition(voyeur).TransitionType ==
		    CellMovementTransition.NoViableTransition)
		{
			return null;
		}

		return exit?.CellExitFor(cell);
	}

	public ICellExit GetExit(ICell cell, string verb, string target, IPerceiver voyeur, ICellOverlay overlay = null)
	{
		if (overlay == null)
		{
			overlay = cell.CurrentOverlay;
		}

		if (!CellExitDictionary.ContainsKey((cell, overlay)))
		{
			InitialiseCell(cell, overlay);
		}

		var exits =
			CellExitDictionary[(cell, overlay)]
				.Where(x => overlay.ExitIDs.Contains(x.Id) && x.IsExit(cell, verb) && voyeur.CanSee(x))
				.OrderBy(x => x.CellExitFor(cell).OutboundDirection.ExitCommandPriority())
				.ToList();
		ICellExit exit = null;
		if (!string.IsNullOrEmpty(target))
		{
			exit = exits.Select(x => x.CellExitFor(cell)).GetFromItemListByKeyword(target, voyeur);
		}
		else
		{
			exit = exits.Any() ? exits.First().CellExitFor(cell) : null;
		}

		if (exit?.MovementTransition(voyeur).TransitionType == CellMovementTransition.NoViableTransition)
		{
			return null;
		}

		return exit;
	}

	public ICellExit GetExitKeyword(ICell cell, string keyword, IPerceiver voyeur, ICellOverlay overlay = null)
	{
		if (overlay == null)
		{
			overlay = cell.CurrentOverlay;
		}

		if (!CellExitDictionary.ContainsKey((cell, overlay)))
		{
			InitialiseCell(cell, overlay);
		}

		var exits =
			CellExitDictionary[(cell, overlay)]
				.Where(x => overlay.ExitIDs.Contains(x.Id) && x.IsExitKeyword(cell, keyword) && voyeur.CanSee(x))
				.OrderBy(x => x.CellExitFor(cell).OutboundDirection.ExitCommandPriority())
				.ToList();
		var cellExit = exits.FirstOrDefault()?.CellExitFor(cell);
		if (cellExit?.MovementTransition(voyeur).TransitionType ==
		    CellMovementTransition.NoViableTransition)
		{
			return null;
		}

		return cellExit;
	}

	public IEnumerable<ICellExit> GetExitsFor(ICell cell, ICellOverlay overlay = null, RoomLayer? layer = null)
	{
		if (overlay == null)
		{
			overlay = cell.CurrentOverlay;
		}

		InitialiseCell(cell, overlay);

		return
			CellExitDictionary[(cell, overlay)]
				.Where(x => overlay.ExitIDs.Contains(x.Id))
				.Select(x => x.CellExitFor(cell))
				.Where(x => layer == null || x.WhichLayersExitAppears().Contains(layer.Value))
				.ToList();
	}

	/// <summary>
	/// Retrieves all exits for the specified cell and overlay combination
	/// </summary>
	/// <param name="cell">The cell for which to request the exit information</param>
	/// <param name="package">The overlay package for which you want to get exits</param>
	/// <returns>An IEnumerable of all the ICellExits for this cell and overlay package</returns>
	public IEnumerable<ICellExit> GetExitsFor(ICell cell, ICellOverlayPackage package, RoomLayer? layer = null)
	{
		var overlay = cell.GetOverlay(package);
		if (overlay == null)
		{
			return Enumerable.Empty<ICellExit>();
		}

		InitialiseCell(cell, overlay);

		return
			CellExitDictionary[(cell, overlay)].Where(x => overlay.ExitIDs.Contains(x.Id))
			                                               .Select(x => x.CellExitFor(cell))
			                                               .Where(x => layer == null || x.WhichLayersExitAppears()
				                                               .Contains(layer.Value))
			                                               .ToList();
	}

	public void PreloadCriticalExits()
	{
		using (new FMDB())
		{
			foreach (var exit in FMDB.Context.Exits.Where(x => x.DoorId.HasValue || x.FallCell.HasValue).ToList())
			{
				InitialiseCell(Gameworld.Cells.Get(exit.CellId1), null);
				InitialiseCell(Gameworld.Cells.Get(exit.CellId2), null);
			}
		}
	}

	public IEnumerable<ICellExit> GetAllExits(ICell cell)
	{
		// Initialise each of the overlays for the cell
		foreach (var overlay in cell.Overlays)
		{
			InitialiseCell(cell, overlay);
		}

		return
			CellExitDictionary.Where(x => x.Key.Item1 == cell)
			                  .SelectMany(x => x.Value)
			                  .Distinct()
			                  .Select(x => x.CellExitFor(cell));
	}

	public IExit GetExitByID(long id)
	{
		return MasterExitList.TryGetValue(id, out var value) ? value : null;
	}

	public void UpdateCellOverlayExits(ICell cell, ICellOverlay overlay)
	{
		// It is only necessary to update if it is a Cell / Cell Overlay combo that we have already loaded. Otherwise it can be caught later.
		if (CellExitDictionary.ContainsKey((cell, overlay)))
		{
			CellExitDictionary.Remove((cell, overlay));
			InitialiseCell(cell, overlay);
		}
	}

	public void DeleteCell(ICell cell)
	{
		// Initialise the cell so all exits are in memory
		InitialiseCell(cell, null);

		// Get a list of all the exits that we're deleting
		var exitsToDelete = new HashSet<IExit>();
		foreach (var overlay in cell.Overlays)
		{
			foreach (var exit in overlay.ExitIDs)
			{
				exitsToDelete.Add(MasterExitList[exit]);
			}

			// Also remove the cell/overlay combo from the master list
			CellExitDictionary.Remove((cell, overlay));
		}

		// Remove the other end exit as well
		var otherCells = exitsToDelete.SelectMany(x => x.Cells).Distinct().Except(cell).ToList();
		foreach (var other in otherCells)
		{
			InitialiseCell(other, null);
			foreach (var overlay in other.Overlays) {
				CellExitDictionary.RemoveAll((other, overlay), x => x.Cells.Contains(cell));
			}
		}

		// Delete the exits
		foreach (var exit in exitsToDelete)
		{
			MasterExitList.Remove(exit.Id);
			foreach (IEditableCellOverlay overlay in exit.Cells.First().Overlays)
			{
				overlay.RemoveExit(exit);
			}
			foreach (IEditableCellOverlay overlay in exit.Cells.Last().Overlays)
			{
				overlay.RemoveExit(exit);
			}
			exit.Delete();
		}
	}

	#endregion
}