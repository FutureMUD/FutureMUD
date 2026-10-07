using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Construction.Boundary;

public class Exit : PerceivedItem, IExit
{
    private readonly List<IRoom> _cells = new();

    private readonly IRoomExit[] RoomExits = new IRoomExit[2];

    /// <summary>
    ///     Constructs an Exit from information pertaining to a Non-Cardinal Exit
    /// </summary>
    /// <param name="gameworld"></param>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <param name="timeMultiplier"></param>
    /// <param name="template"></param>
    /// <param name="outboundTarget"></param>
    /// <param name="inboundTarget"></param>
    /// <param name="outboundKeyword"></param>
    /// <param name="inboundKeyword"></param>
    public Exit(IFuturemud gameworld, IRoom origin, IRoom destination, double timeMultiplier,
        INonCardinalExitTemplate template, string outboundKeyword, string inboundKeyword, string outboundTarget,
        string inboundTarget)
    {
        Gameworld = gameworld;
        using (new FMDB())
        {
            Models.Exit dbexit = new()
            {
                RoomId1 = origin.Id,
                RoomId2 = destination.Id,
                Direction1 = (int)CardinalDirection.Unknown,
                Direction2 = (int)CardinalDirection.Unknown,
                TimeMultiplier = timeMultiplier,
                AcceptsDoor = false,

                InboundDescription1 = template.OriginInboundPreface,
                InboundDescription2 = template.DestinationInboundPreface,
                OutboundDescription1 = template.OriginOutboundPreface,
                OutboundDescription2 = template.DestinationOutboundPreface,

                Verb1 = template.OutboundVerb,
                Verb2 = template.InboundVerb,

                OutboundTarget1 = outboundTarget,
                InboundTarget2 = outboundTarget,
                OutboundTarget2 = inboundTarget,
                InboundTarget1 = inboundTarget,

                Keywords1 =
                    GetKeywordsFromSDesc(outboundTarget)
                        .Concat(new[] { outboundKeyword })
                        .ToList()
                        .ListToString(separator: " ", conjunction: ""),
                Keywords2 =
                    GetKeywordsFromSDesc(inboundTarget)
                        .Concat(new[] { inboundKeyword })
                        .ToList()
                        .ListToString(separator: " ", conjunction: ""),

                PrimaryKeyword1 = outboundKeyword,
                PrimaryKeyword2 = inboundKeyword,

                MaximumSizeToEnter = (int)SizeCategory.Titanic,
                MaximumSizeToEnterUpright = (int)SizeCategory.Titanic,
                BlockedLayers = BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(),
                IsClimbExit = false,
                ClimbDifficulty = (int)Difficulty.Normal
            };

            FMDB.Context.Exits.Add(dbexit);
            FMDB.Context.SaveChanges();
            LoadFromDatabase(dbexit);
        }
    }

    public override object DatabaseInsert()
    {
        Models.Exit dbexit = new()
        {
            RoomId1 = _cells[0].Id,
            RoomId2 = _cells[1].Id
        };
        dbexit.RoomId1 = RoomExits[0].Origin.Id;
        dbexit.RoomId2 = RoomExits[1].Origin.Id;
        dbexit.Direction1 = (int)RoomExits[0].OutboundDirection;
        dbexit.Direction2 = (int)RoomExits[1].OutboundDirection;
        dbexit.TimeMultiplier = TimeMultiplier;
        dbexit.AcceptsDoor = AcceptsDoor;
        dbexit.DoorSize = AcceptsDoor ? (int)DoorSize : (int?)null;
        dbexit.MaximumSizeToEnter = (int)MaximumSizeToEnter;
        dbexit.MaximumSizeToEnterUpright = (int)MaximumSizeToEnterUpright;
        dbexit.ClimbDifficulty = (int)ClimbDifficulty;
        dbexit.IsClimbExit = IsClimbExit;
        dbexit.BlockedLayers = BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues();
        FMDB.Context.Exits.Add(dbexit);
        if (RoomExits[0] is NonCardinalRoomExit nexit1)
        {
            NonCardinalRoomExit nexit2 = (NonCardinalRoomExit)RoomExits[1];
            dbexit.InboundDescription1 = nexit1.InboundDescription;
            dbexit.InboundDescription2 = nexit2.InboundDescription;
            dbexit.OutboundDescription1 = nexit1.OutboundDescription;
            dbexit.OutboundDescription2 = nexit2.OutboundDescription;

            dbexit.Verb1 = nexit1.Verb;
            dbexit.Verb2 = nexit2.Verb;

            dbexit.OutboundTarget1 = nexit1.OutboundTarget;
            dbexit.OutboundTarget2 = nexit2.OutboundTarget;
            dbexit.InboundTarget1 = nexit1.InboundTarget;
            dbexit.InboundTarget2 = nexit2.InboundTarget;

            dbexit.Keywords1 =
                GetKeywordsFromSDesc(nexit1.OutboundTarget)
                    .Concat(new[] { nexit1.PrimaryKeyword })
                    .ToList()
                    .ListToString(separator: " ", conjunction: "");
            dbexit.Keywords2 =
                GetKeywordsFromSDesc(nexit2.OutboundTarget)
                    .Concat(new[] { nexit2.PrimaryKeyword })
                    .ToList()
                    .ListToString(separator: " ", conjunction: "");

            dbexit.PrimaryKeyword1 = nexit1.PrimaryKeyword;
            dbexit.PrimaryKeyword2 = nexit2.PrimaryKeyword;
        }

        return dbexit;
    }

    public Exit(IFuturemud gameworld, IRoom origin, IRoom destination, Exit otherExit)
    {
        Gameworld = gameworld;
        _noSave = true;
        TimeMultiplier = otherExit.TimeMultiplier;

        AcceptsDoor = otherExit.AcceptsDoor;
        DoorSize = otherExit.DoorSize;
        _cells.Add(origin);
        _cells.Add(destination);

        if (otherExit.RoomExits[0] is NonCardinalRoomExit exit)
        {
            RoomExits[0] = new NonCardinalRoomExit(this, exit, origin, destination);
            RoomExits[1] =
                new NonCardinalRoomExit(this, (NonCardinalRoomExit)otherExit.RoomExits[1], destination, origin);
        }
        else
        {
            RoomExits[0] = new RoomExit(this, otherExit.RoomExits[0], origin, destination);
            RoomExits[1] = new RoomExit(this, otherExit.RoomExits[1], destination, origin);
        }

        MaximumSizeToEnter = otherExit.MaximumSizeToEnter;
        MaximumSizeToEnterUpright = otherExit.MaximumSizeToEnterUpright;

        _noSave = false;
    }

    /// <summary>
    ///     Constructs an Exit designed to function as a Cardinal exit
    /// </summary>
    /// <param name="gameworld"></param>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <param name="outboundDirection"></param>
    /// <param name="inboundDirection"></param>
    /// <param name="timeMultiplier"></param>
    public Exit(IFuturemud gameworld, IRoom origin, IRoom destination, CardinalDirection outboundDirection,
        CardinalDirection inboundDirection, double timeMultiplier)
    {
        Gameworld = gameworld;
        using (new FMDB())
        {
            Models.Exit dbexit = new()
            {
                RoomId1 = origin.Id,
                RoomId2 = destination.Id,
                Direction1 = (int)outboundDirection,
                Direction2 = (int)inboundDirection,
                TimeMultiplier = timeMultiplier,
                AcceptsDoor = false,
                MaximumSizeToEnter = (int)SizeCategory.Titanic,
                MaximumSizeToEnterUpright = (int)SizeCategory.Titanic,
                BlockedLayers = string.Empty
            };
            FMDB.Context.Exits.Add(dbexit);
            FMDB.Context.SaveChanges();
            LoadFromDatabase(dbexit);
        }
    }

    public Exit(MudSharp.Models.Exit exit, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        LoadFromDatabase(exit);
    }

    private Exit(Exit rhs)
    {
        Gameworld = rhs.Gameworld;
        using (new FMDB())
        {
            IRoomExit exit1 = rhs.RoomExits[0];
            IRoomExit exit2 = rhs.RoomExits[1];
            Models.Exit dbexit = new()
            {
                RoomId1 = exit1.Origin.Id,
                RoomId2 = exit2.Origin.Id,
                Direction1 = (int)exit1.OutboundDirection,
                Direction2 = (int)exit2.OutboundDirection,
                TimeMultiplier = rhs.TimeMultiplier,
                AcceptsDoor = rhs.AcceptsDoor,
                DoorSize = rhs.AcceptsDoor ? (int)rhs.DoorSize : (int?)null,
                MaximumSizeToEnter = (int)rhs.MaximumSizeToEnter,
                MaximumSizeToEnterUpright = (int)rhs.MaximumSizeToEnterUpright,
                BlockedLayers = rhs.BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(),
                ClimbDifficulty = (int)rhs.ClimbDifficulty,
                FallRoom = rhs.FallRoom?.Id,
                IsClimbExit = rhs.IsClimbExit
            };

            if (exit1 is NonCardinalRoomExit)
            {
                NonCardinalRoomExit nexit1 = exit1 as NonCardinalRoomExit;
                NonCardinalRoomExit nexit2 = exit2 as NonCardinalRoomExit;
                dbexit.InboundDescription1 = nexit1.InboundDescription;
                dbexit.InboundDescription2 = nexit2.InboundDescription;
                dbexit.OutboundDescription1 = nexit1.OutboundDescription;
                dbexit.OutboundDescription2 = nexit2.OutboundDescription;

                dbexit.Verb1 = nexit1.Verb;
                dbexit.Verb2 = nexit2.Verb;

                dbexit.OutboundTarget1 = nexit1.OutboundTarget;
                dbexit.OutboundTarget2 = nexit2.OutboundTarget;
                dbexit.InboundTarget1 = nexit1.InboundTarget;
                dbexit.InboundTarget2 = nexit2.InboundTarget;

                dbexit.Keywords1 =
                    GetKeywordsFromSDesc(nexit1.OutboundTarget)
                        .Concat(new[] { nexit1.PrimaryKeyword })
                        .ToList()
                        .ListToString(separator: " ", conjunction: "");
                dbexit.Keywords2 =
                    GetKeywordsFromSDesc(nexit2.OutboundTarget)
                        .Concat(new[] { nexit2.PrimaryKeyword })
                        .ToList()
                        .ListToString(separator: " ", conjunction: "");

                dbexit.PrimaryKeyword1 = nexit1.PrimaryKeyword;
                dbexit.PrimaryKeyword2 = nexit2.PrimaryKeyword;
            }

            FMDB.Context.Exits.Add(dbexit);
            FMDB.Context.SaveChanges();
            LoadFromDatabase(dbexit);
        }
    }

    public override string ToString()
    {
        return $"Exit {Id:N0} from {_cells[0].Name} ({_cells[0].Id:N0}) to {_cells[1].Name} ({_cells[1].Id})";
    }

    public override string FrameworkItemType => "Exit";

    #region IFutureProgVariable Implementation

    public override ProgVariableTypes Type => ProgVariableTypes.Error;

    #endregion

    public IEnumerable<IRoom> Rooms => _cells;

    #region Overrides of PerceivedItem

    public override IRoom Location => RoomExits[0].Origin;

    #endregion

    public bool AcceptsDoor { get; set; }
    public SizeCategory DoorSize { get; set; }
    public IDoor Door { get; set; }
    public double TimeMultiplier { get; set; }

    public IRoom FallRoom { get; set; }
    public bool IsClimbExit { get; set; }
    public Difficulty ClimbDifficulty { get; set; }

    public SizeCategory MaximumSizeToEnterUpright { get; set; }

    public SizeCategory MaximumSizeToEnter { get; set; }

    public IRoom Opposite(IRoom room)
    {
        return RoomExitFor(room)?.Destination;
    }

    public IRoomExit RoomExitFor(IRoom room)
    {
        return RoomExits.FirstOrDefault(x => x.Origin == room);
    }

    public bool IsExit(IRoom room, string verb)
    {
        return RoomExitFor(room).IsExit(verb);
    }

    public bool IsExitKeyword(IRoom room, string keyword)
    {
        return RoomExitFor(room).IsExitKeyword(keyword);
    }

    private readonly List<RoomLayer> _blockedLayers = new();
    public IEnumerable<RoomLayer> BlockedLayers => _blockedLayers;

    public void AddBlockedLayer(RoomLayer layer)
    {
        _blockedLayers.Add(layer);
        Changed = true;
    }

    public void RemoveBlockedLayer(RoomLayer layer)
    {
        _blockedLayers.Remove(layer);
        Changed = true;
    }

    public override void Register(IOutputHandler handler)
    {
        throw new NotSupportedException();
    }

    public IExit Clone()
    {
        return new Exit(this);
    }

    public void Delete()
    {
        Gameworld.SaveManager.Abort(this);
        if (_id != 0)
        {
            using (new FMDB())
            {
                Door?.Parent.Delete();

                Gameworld.SaveManager.Flush();
                Models.Exit dbitem = FMDB.Context.Exits.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.Exits.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    public override void Save()
    {
        using (new FMDB())
        {
            Models.Exit dbexit = FMDB.Context.Exits.Find(Id);
            dbexit.AcceptsDoor = AcceptsDoor;
            dbexit.DoorSize = AcceptsDoor ? (int)DoorSize : (int?)null;
            dbexit.DoorId = Door?.Parent.Id;
            dbexit.MaximumSizeToEnter = (int)MaximumSizeToEnter;
            dbexit.MaximumSizeToEnterUpright = (int)MaximumSizeToEnterUpright;
            dbexit.IsClimbExit = IsClimbExit;
            dbexit.ClimbDifficulty = (int)ClimbDifficulty;
            dbexit.FallRoom = FallRoom?.Id;
            dbexit.BlockedLayers = BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues();
            FMDB.Context.SaveChanges();
            // TODO - saving cellExit changes too?
        }

        Changed = false;
    }

    public override void SetIDFromDatabase(object dbitem)
    {
        _id = ((MudSharp.Models.Exit)dbitem).Id;
    }

    public void PostLoadTasks(MudSharp.Models.Exit exit)
    {
        if (exit.DoorId.HasValue)
        {
            using (new FMDB())
            {
                IGameItem gitem = Gameworld.TryGetItem(exit.DoorId ?? 0, true);
                IDoor door = gitem?.GetItemType<IDoor>();
                if (door != null)
                {
                    Door = door;
                    door.InstalledExit = this;
                    gitem.FinaliseLoadTimeTasks();
                    gitem.Login();
                }
            }
        }
    }

    private void LoadFromDatabase(MudSharp.Models.Exit exit)
    {
        _id = exit.Id;
        IdInitialised = true;
        _noSave = true;
        TimeMultiplier = exit.TimeMultiplier;

        AcceptsDoor = exit.AcceptsDoor;
        DoorSize = (SizeCategory)(exit.DoorSize ?? 0);
        _cells.Add(Gameworld.Rooms.Get(exit.RoomId1));
        _cells.Add(Gameworld.Rooms.Get(exit.RoomId2));

        if (!string.IsNullOrEmpty(exit.Verb1))
        {
            RoomExits[0] = new NonCardinalRoomExit(this, exit, true);
            RoomExits[1] = new NonCardinalRoomExit(this, exit, false);
        }
        else
        {
            RoomExits[0] = new RoomExit(this, exit, true);
            RoomExits[1] = new RoomExit(this, exit, false);
        }

        MaximumSizeToEnter = (SizeCategory)exit.MaximumSizeToEnter;
        MaximumSizeToEnterUpright = (SizeCategory)exit.MaximumSizeToEnterUpright;
        ClimbDifficulty = (Difficulty)exit.ClimbDifficulty;
        if (exit.FallRoom.HasValue)
        {
            FallRoom = _cells.First(x => x.Id == exit.FallRoom);
        }

        IsClimbExit = exit.IsClimbExit;
        if (!string.IsNullOrEmpty(exit.BlockedLayers))
        {
            _blockedLayers.AddRange(exit.BlockedLayers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(x => (RoomLayer)int.Parse(x)));
        }

        _noSave = false;
    }
}