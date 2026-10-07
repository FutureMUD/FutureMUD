using MudSharp.Database;

namespace MudSharp.Construction.Boundary;

public class ExitManager : IExitManager, IHaveFuturemud
{
    protected readonly CollectionDictionary<(IRoom Room, IRoomOverlay Overlay), IExit> RoomExitDictionary =
        new();

    protected readonly CollectionDictionary<IRoom, IExit> TransientExitDictionary = new();

    protected readonly DictionaryWithDefault<long, IExit> MasterExitList = new();

    public ExitManager(IFuturemud gameworld)
    {
        Gameworld = gameworld;
        PathfindingService = new PathfindingService(gameworld);
		SpatialPathfinder = new SpatialPathfinder();
    }

    public IFuturemud Gameworld { get; protected set; }
	public event Action<ITransientExit> TransientExitRegistered;
	public event Action<ITransientExit, ITransientExit> TransientExitReplaced;
	public event Action<ITransientExit> TransientExitUnregistered;

    public IPathfindingService PathfindingService { get; }
	public ISpatialPathfinder SpatialPathfinder { get; }

    /// <summary>
    ///     Called the first time that an exit for a particular cell and/or overlay is requested. Initialises the cell in the
    ///     manager.
    /// </summary>
    /// <param name="cell">The cell which is being initialised</param>
    /// <param name="overlay">The overlay which is being initialised (if not specified, initialise all overlays)</param>
    public void InitialiseRoom(IRoom room, IRoomOverlay overlay)
    {
        if (overlay == null)
        {
            foreach (IRoomOverlay item in room.Overlays)
            {
                InitialiseRoom(room, item);
            }

            return;
        }

        if (RoomExitDictionary.ContainsKey((room, overlay)))
        {
            return;
        }

        using (new FMDB())
        {
            List<Models.Exit> exits =
                FMDB.Context.Exits.Where(
                        x => (x.RoomId1 == room.Id || x.RoomId2 == room.Id) && overlay.ExitIDs.Contains(x.Id))
                    .ToList();
            List<IExit> exitList = new();
            foreach (Models.Exit exit in exits)
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

            RoomExitDictionary.AddRange((room, overlay), exitList);
        }

        room.OnExitsInitialised();
    }

    #region IExitManager Implementation

    public IRoomExit GetExit(IRoom room, CardinalDirection direction, IPerceiver voyeur)
    {
        IRoomOverlay overlay = room.GetOverlayFor(voyeur);
        if (overlay == null)
        {
            overlay = room.CurrentOverlay;
        }

        InitialiseRoom(room, overlay);

        IExit exit =
            RoomExitDictionary[(room, overlay)]
                .Where(x => overlay.ExitIDs.Contains(x.Id))
                .Concat(TransientExitDictionary[room])
                .FirstOrDefault(x => x.RoomExitFor(room).OutboundDirection == direction);
        IRoomExit cellExit = exit?.RoomExitFor(room);
        if (cellExit?.MovementTransition(voyeur).TransitionType ==
            RoomMovementTransition.NoViableTransition)
        {
            return null;
        }

        return exit?.RoomExitFor(room);
    }

    public IRoomExit GetExit(IRoom room, string verb, string target, IPerceiver voyeur, IRoomOverlay overlay = null)
    {
        if (overlay == null)
        {
            overlay = room.CurrentOverlay;
        }

        if (!RoomExitDictionary.ContainsKey((room, overlay)))
        {
            InitialiseRoom(room, overlay);
        }

        List<IExit> exits =
            RoomExitDictionary[(room, overlay)]
                .Where(x => overlay.ExitIDs.Contains(x.Id))
                .Concat(TransientExitDictionary[room])
                .Where(x => x.IsExit(room, verb) && voyeur.CanSee(x))
                .OrderBy(x => x.RoomExitFor(room).OutboundDirection.ExitCommandPriority())
                .ToList();
        IRoomExit exit = null;
        if (!string.IsNullOrEmpty(target))
        {
            exit = exits.Select(x => x.RoomExitFor(room)).GetFromItemListByKeyword(target, voyeur);
        }
        else
        {
            exit = exits.Any() ? exits.First().RoomExitFor(room) : null;
        }

        if (exit?.MovementTransition(voyeur).TransitionType == RoomMovementTransition.NoViableTransition)
        {
            return null;
        }

        return exit;
    }

    public IRoomExit GetExitKeyword(IRoom room, string keyword, IPerceiver voyeur, IRoomOverlay overlay = null)
    {
        if (overlay == null)
        {
            overlay = room.CurrentOverlay;
        }

        if (!RoomExitDictionary.ContainsKey((room, overlay)))
        {
            InitialiseRoom(room, overlay);
        }

        List<IExit> exits =
            RoomExitDictionary[(room, overlay)]
                .Where(x => overlay.ExitIDs.Contains(x.Id))
                .Concat(TransientExitDictionary[room])
                .Where(x => x.IsExitKeyword(room, keyword) && voyeur.CanSee(x))
                .OrderBy(x => x.RoomExitFor(room).OutboundDirection.ExitCommandPriority())
                .ToList();
        IRoomExit cellExit = exits.FirstOrDefault()?.RoomExitFor(room);
        if (cellExit?.MovementTransition(voyeur).TransitionType ==
            RoomMovementTransition.NoViableTransition)
        {
            return null;
        }

        return cellExit;
    }

    public IEnumerable<IRoomExit> GetExitsFor(IRoom room, IRoomOverlay overlay = null, RoomLayer? layer = null)
    {
        if (overlay == null)
        {
            overlay = room.CurrentOverlay;
        }

        InitialiseRoom(room, overlay);

        return
            RoomExitDictionary[(room, overlay)]
                .Where(x => overlay.ExitIDs.Contains(x.Id))
                .Concat(TransientExitDictionary[room])
                .Select(x => x.RoomExitFor(room))
                .Where(x => layer == null || x.WhichLayersExitAppears().Contains(layer.Value))
                .ToList();
    }

    /// <summary>
    /// Retrieves all exits for the specified cell and overlay combination
    /// </summary>
    /// <param name="cell">The cell for which to request the exit information</param>
    /// <param name="package">The overlay package for which you want to get exits</param>
    /// <returns>An IEnumerable of all the ICellExits for this cell and overlay package</returns>
    public IEnumerable<IRoomExit> GetExitsFor(IRoom room, IRoomOverlayPackage package, RoomLayer? layer = null)
    {
        IRoomOverlay overlay = room.GetOverlay(package);
        if (overlay == null)
        {
            return Enumerable.Empty<IRoomExit>();
        }

        InitialiseRoom(room, overlay);

        return
            RoomExitDictionary[(room, overlay)].Where(x => overlay.ExitIDs.Contains(x.Id))
                                                           .Concat(TransientExitDictionary[room])
                                                           .Select(x => x.RoomExitFor(room))
                                                           .Where(x => layer == null || x.WhichLayersExitAppears()
                                                               .Contains(layer.Value))
                                                           .ToList();
    }

    public void PreloadCriticalExits()
    {
        using (new FMDB())
        {
            foreach (Models.Exit exit in FMDB.Context.Exits.Where(x => x.DoorId.HasValue || x.FallRoom.HasValue).ToList())
            {
                InitialiseRoom(Gameworld.Rooms.Get(exit.RoomId1), null);
                InitialiseRoom(Gameworld.Rooms.Get(exit.RoomId2), null);
            }
        }
    }

    public IEnumerable<IRoomExit> GetAllExits(IRoom room)
    {
        // Initialise each of the overlays for the cell
        foreach (IRoomOverlay overlay in room.Overlays)
        {
            InitialiseRoom(room, overlay);
        }

        return
            RoomExitDictionary.Where(x => x.Key.Item1 == room)
                              .SelectMany(x => x.Value)
                              .Concat(TransientExitDictionary[room])
                              .Distinct()
                              .Select(x => x.RoomExitFor(room));
    }

    public IExit GetExitByID(long id)
    {
        return MasterExitList.GetValueOrDefault(id) ??
               TransientExitDictionary.SelectMany(x => x.Value).FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<IExit> TransientExits => TransientExitDictionary
        .SelectMany(x => x.Value)
        .Distinct()
        .ToList();

    public void RegisterTransientExit(IExit exit)
    {
        if (exit is null)
        {
            return;
        }

		var registered = false;
        foreach (var room in exit.Rooms)
        {
            if (!TransientExitDictionary[room].Contains(exit))
            {
                TransientExitDictionary.Add(room, exit);
				registered = true;
            }
        }

		if (!registered)
		{
			return;
		}

        PathfindingService.InvalidateTopology();
		SpatialPathfinder.InvalidateTopology();
		if (exit is ITransientExit transientExit)
		{
			TransientExitRegistered?.Invoke(transientExit);
		}
    }

	public bool ReplaceTransientExit(IExit existingExit, IExit replacementExit)
	{
		if (existingExit is not ITransientExit existingTransient ||
		    replacementExit is not ITransientExit replacementTransient ||
		    !existingTransient.StableKey.Equals(replacementTransient.StableKey, StringComparison.Ordinal) ||
		    !existingExit.Rooms.Select(x => x.Id).OrderBy(x => x)
			    .SequenceEqual(replacementExit.Rooms.Select(x => x.Id).OrderBy(x => x)))
		{
			UnregisterTransientExit(existingExit);
			RegisterTransientExit(replacementExit);
			return false;
		}

		var wasRegistered = existingExit.Rooms.Any(room => TransientExitDictionary[room].Contains(existingExit));
		if (!wasRegistered)
		{
			RegisterTransientExit(replacementExit);
			return false;
		}

		foreach (var room in existingExit.Rooms)
		{
			TransientExitDictionary.Remove(room, existingExit);
		}

		foreach (var room in replacementExit.Rooms)
		{
			if (!TransientExitDictionary[room].Contains(replacementExit))
			{
				TransientExitDictionary.Add(room, replacementExit);
			}
		}

		PathfindingService.InvalidateTopology();
		SpatialPathfinder.InvalidateTopology();
		TransientExitReplaced?.Invoke(existingTransient, replacementTransient);
		return true;
	}

    public void UnregisterTransientExit(IExit exit)
    {
        if (exit is null)
        {
            return;
        }

		var wasRegistered = exit.Rooms.Any(room => TransientExitDictionary[room].Contains(exit));
		if (!wasRegistered)
		{
			return;
		}

        foreach (var room in exit.Rooms)
        {
            TransientExitDictionary.Remove(room, exit);
        }

        PathfindingService.InvalidateTopology();
		SpatialPathfinder.InvalidateTopology();
		if (exit is ITransientExit transientExit)
		{
			TransientExitUnregistered?.Invoke(transientExit);
		}
    }

    public void UpdateRoomOverlayExits(IRoom room, IRoomOverlay overlay)
    {
        // It is only necessary to update if it is a Cell / Cell Overlay combo that we have already loaded. Otherwise it can be caught later.
        if (RoomExitDictionary.ContainsKey((room, overlay)))
        {
            RoomExitDictionary.Remove((room, overlay));
        }

        InitialiseRoom(room, overlay);
        PathfindingService.InvalidateTopology(room);
		SpatialPathfinder.InvalidateTopology(room);
    }

    public void DeleteRoom(IRoom room)
    {
        PathfindingService.InvalidateTopology(room);
		SpatialPathfinder.InvalidateTopology();

        // Initialise the cell so all exits are in memory
        InitialiseRoom(room, null);

        // Get a list of all the exits that we're deleting
        HashSet<IExit> exitsToDelete = new();
        foreach (IRoomOverlay overlay in room.Overlays)
        {
            foreach (long exit in overlay.ExitIDs)
            {
                exitsToDelete.Add(MasterExitList[exit]);
            }

            // Also remove the cell/overlay combo from the master list
            RoomExitDictionary.Remove((room, overlay));
        }

        // Remove the other end exit as well
        List<IRoom> otherRooms = exitsToDelete.SelectMany(x => x.Rooms).Distinct().Except(room).ToList();
        foreach (IRoom other in otherRooms)
        {
            InitialiseRoom(other, null);
            foreach (IRoomOverlay overlay in other.Overlays)
            {
                RoomExitDictionary.RemoveAll((other, overlay), x => x.Rooms.Contains(room));
            }
        }

        // Delete the exits
        foreach (IExit exit in exitsToDelete)
        {
            MasterExitList.Remove(exit.Id);
            foreach (IEditableRoomOverlay overlay in exit.Rooms.First().Overlays)
            {
                overlay.RemoveExit(exit);
            }
            foreach (IEditableRoomOverlay overlay in exit.Rooms.Last().Overlays)
            {
                overlay.RemoveExit(exit);
            }
            exit.Delete();
        }
    }

    #endregion
}
