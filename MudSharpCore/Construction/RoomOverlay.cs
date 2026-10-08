using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp.Construction;

public class RoomOverlay : SaveableItem, IEditableRoomOverlay
{
    private double _addedLight;

    private double _ambientLightFactor;


    private string _roomDescription;

    private string _roomName;

    protected List<long> _exitIDs;

    private IHearingProfile _hearingProfile;

    private RoomOutdoorsType _outdoorsType;

    private ITerrain _terrain;

    private IFluid _atmosphere;

    public IFluid Atmosphere
    {
        get => _atmosphere;
        set
        {
			using var exposureChange = EnvironmentalExposureService.ChangingEnvironment(Room);
            _atmosphere = value;
            Changed = true;
        }
    }

    private bool _safeQuit;

    public bool SafeQuit
    {
        get => _safeQuit;
        set
        {
            _safeQuit = value;
            Changed = true;
        }
    }

    public RoomOverlay(MudSharp.Models.RoomOverlay overlay, IRoom room, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        Room = room;
        LoadFromDatabase(overlay);
    }

    protected RoomOverlay(IRoomOverlay rhs, IRoomOverlayPackage package)
    {
        Package = package;
        Room = rhs.Room;
        Gameworld = rhs.Gameworld;
        using (new FMDB())
        {
            Models.RoomOverlay dboverlay = new()
            {
                Name = rhs.Name,
                RoomId = rhs.Room.Id,
                RoomDescription = rhs.RoomDescription,
                RoomName = rhs.RoomName,
                RoomOverlayPackageId = package.Id,
                RoomOverlayPackageRevisionNumber = package.RevisionNumber,
                TerrainId = rhs.Terrain.Id,
                OutdoorsType = (int)rhs.OutdoorsType,
                HearingProfileId = rhs.HearingProfile?.Id,
                AmbientLightFactor = rhs.AmbientLightFactor,
                AddedLight = rhs.AddedLight,
                AtmosphereId = rhs.Atmosphere?.Id,
                AtmosphereType = rhs.Atmosphere is ILiquid ? "liquid" : "gas",
                SafeQuit = rhs.SafeQuit
            };
            foreach (long exit in rhs.ExitIDs)
            {
                dboverlay.RoomOverlaysExits.Add(new RoomOverlayExit { RoomOverlay = dboverlay, ExitId = exit });
            }

            FMDB.Context.RoomOverlays.Add(dboverlay);
            FMDB.Context.SaveChanges();
            LoadFromDatabase(dboverlay);
        }
    }

    public RoomOverlay(IRoom room, IRoomOverlayPackage package)
    {
        Gameworld = room.Gameworld;
        Package = package;
        Room = room;
        Models.Terrain terrain = FMDB.Context.Terrains.First(x => x.DefaultTerrain);
        using (new FMDB())
        {
            Models.RoomOverlay dboverlay = new()
            {
                Name = package.Name,
                RoomId = Room.Id,
                RoomDescription =
                    "This is a newly built location that has not yet been described. It should not be approved for use in game.",
                RoomName = "An Unnamed Location",
                RoomOverlayPackageId = package.Id,
                RoomOverlayPackageRevisionNumber = package.RevisionNumber,
                Terrain = terrain,
                OutdoorsType = terrain.DefaultRoomOutdoorsType,
                AddedLight = 0,
                AmbientLightFactor = (RoomOutdoorsType)terrain.DefaultRoomOutdoorsType switch
                {
                    RoomOutdoorsType.Outdoors => 1.0,
                    RoomOutdoorsType.Indoors => 0.25,
                    RoomOutdoorsType.IndoorsClimateExposed => 0.9,
                    RoomOutdoorsType.IndoorsWithWindows => 0.35,
                    RoomOutdoorsType.IndoorsNoLight => 0.0,
                    _ => 1.0
                },
                AtmosphereId = Gameworld.GetStaticLong("DefaultAtmosphereId"),
                AtmosphereType = Gameworld.GetStaticConfiguration("DefaultAtmosphereType"),
                SafeQuit = Gameworld.GetStaticBool("RoomsSafeQuitByDefault")
            };
            FMDB.Context.RoomOverlays.Add(dboverlay);
            FMDB.Context.SaveChanges();
            LoadFromDatabase(dboverlay);
        }
    }

    public RoomOverlay(IRoom room, IRoomOverlayPackage package, IRoom templateRoom)
    {
        Gameworld = room.Gameworld;
        Package = package;
        Room = room;
        using (new FMDB())
        {
            Models.RoomOverlay dboverlay = new()
            {
                Name = package.Name,
                RoomId = Room.Id,
                RoomDescription = templateRoom?.CurrentOverlay.RoomDescription ??
                                  "This is a newly built location that has not yet been described. It should not be approved for use in game.",
                RoomName = templateRoom?.CurrentOverlay.RoomName ?? "An Unnamed Location",
                RoomOverlayPackageId = package.Id,
                RoomOverlayPackageRevisionNumber = package.RevisionNumber
            };
            long terrainId = templateRoom?.CurrentOverlay.Terrain.Id ?? 0;
            dboverlay.Terrain = FMDB.Context.Terrains.FirstOrDefault(x => x.Id == terrainId) ??
                                FMDB.Context.Terrains.First(x => x.DefaultTerrain);
            dboverlay.OutdoorsType = (int)(templateRoom?.CurrentOverlay.OutdoorsType ?? RoomOutdoorsType.Outdoors);
            dboverlay.AddedLight = templateRoom?.CurrentOverlay.AddedLight ?? 0;
            dboverlay.AmbientLightFactor = templateRoom?.CurrentOverlay.AmbientLightFactor ?? 1.0;
            dboverlay.SafeQuit = templateRoom?.SafeQuit ?? Gameworld.GetStaticBool("RoomsSafeQuitByDefault");
            if (templateRoom != null)
            {
                dboverlay.AtmosphereId = templateRoom.CurrentOverlay.Atmosphere?.Id;
                dboverlay.AtmosphereType = templateRoom.CurrentOverlay.Atmosphere is ILiquid ? "liquid" : "gas";
            }
            else
            {
                dboverlay.AtmosphereId = Gameworld.GetStaticLong("DefaultAtmosphereId");
                dboverlay.AtmosphereType = Gameworld.GetStaticConfiguration("DefaultAtmosphereType");
            }

            FMDB.Context.RoomOverlays.Add(dboverlay);
            FMDB.Context.SaveChanges();
            LoadFromDatabase(dboverlay);
        }
    }

    public override string FrameworkItemType => "RoomOverlay";

    public override string Name => Package.Name;

    public IRoomOverlayPackage Package { get; protected set; }

    /// <summary>
    ///     When this RoomOverlay is in place, this is the Name of the Room
    /// </summary>
    public string RoomName
    {
        get => _roomName;
        set
        {
            _roomName = value;
            Changed = true;
        }
    }

    /// <summary>
    ///     When this RoomOverlay is in place, this is the Description of the Room
    /// </summary>
    public string RoomDescription
    {
        get => _roomDescription;
        set
        {
            _roomDescription = value;
            Changed = true;
        }
    }

    public ITerrain Terrain
    {
        get => _terrain;
        set
        {
			using var exposureChange = EnvironmentalExposureService.ChangingEnvironment(Room);
            _terrain = value;
            _atmosphere = _terrain?.Atmosphere;
			(Room as Room)?.RefreshWeatherSubscriptions();
			if (Room.CurrentOverlay == this)
			{
				Room.SynchroniseForagableProfile();
				Gameworld.EnvironmentalMagic?.RoomTerrainChanged(Room);
			}
            Changed = true;
        }
    }

    public RoomOutdoorsType OutdoorsType
    {
        get => _outdoorsType;
        set
        {
            _outdoorsType = value;
            Changed = true;
        }
    }

    public IHearingProfile HearingProfile
    {
        get => _hearingProfile;
        set
        {
            _hearingProfile = value;
            Changed = true;
        }
    }

    public double AmbientLightFactor
    {
        get => _ambientLightFactor;
        set
        {
            _ambientLightFactor = value;
            Changed = true;
        }
    }

    public double AddedLight
    {
        get => _addedLight;
        set
        {
            _addedLight = value;
            Changed = true;
        }
    }

    /// <summary>
    ///     The ID numbers of the Exits which are in place when this RoomOverlay is selected
    /// </summary>
    public IEnumerable<long> ExitIDs => _exitIDs;

    public IRoom Room { get; protected set; }

    public IEditableRoomOverlay CreateClone(IRoomOverlayPackage package)
    {
        return new RoomOverlay(this, package);
    }


    public void AddExit(IExit exit)
    {
        _exitIDs.Add(exit.Id);
        Changed = true;
        Gameworld.ExitManager.UpdateRoomOverlayExits(Room, this);
    }

    public void RemoveExit(IExit exit)
    {
        _exitIDs.Remove(exit.Id);
        Changed = true;
        Gameworld.ExitManager.UpdateRoomOverlayExits(Room, this);
    }

    public override void Save()
    {
        using (new FMDB())
        {
            Models.RoomOverlay dboverlay = FMDB.Context.RoomOverlays.Find(Id);
            dboverlay.Name = Name;
            dboverlay.RoomName = RoomName;
            dboverlay.RoomDescription = RoomDescription;
            dboverlay.TerrainId = Terrain.Id;
            dboverlay.OutdoorsType = (int)OutdoorsType;
            dboverlay.HearingProfileId = HearingProfile?.Id;
            dboverlay.AddedLight = AddedLight;
            dboverlay.AmbientLightFactor = AmbientLightFactor;
            dboverlay.AtmosphereId = Atmosphere?.Id;
            dboverlay.AtmosphereType = Atmosphere is ILiquid ? "liquid" : "gas";
            dboverlay.SafeQuit = SafeQuit;
            FMDB.Context.RoomOverlaysExits.RemoveRange(dboverlay.RoomOverlaysExits);
            foreach (long exit in ExitIDs)
            {
                dboverlay.RoomOverlaysExits.Add(new RoomOverlayExit { RoomOverlay = dboverlay, ExitId = exit });
            }

            FMDB.Context.SaveChanges();
        }

        Changed = false;
    }

    private void LoadFromDatabase(Models.RoomOverlay overlay)
    {
        _noSave = true;
        Package = Gameworld.RoomOverlayPackages.Get(overlay.RoomOverlayPackageId,
            overlay.RoomOverlayPackageRevisionNumber);
        _id = overlay.Id;
        RoomName = overlay.RoomName;
        RoomDescription = overlay.RoomDescription;
        _exitIDs = overlay.RoomOverlaysExits.Select(x => x.ExitId).ToList();
        _terrain = Gameworld.Terrains.Get(overlay.TerrainId);
        HearingProfile = Gameworld.HearingProfiles.Get(overlay.HearingProfileId ?? 0);
        OutdoorsType = (RoomOutdoorsType)overlay.OutdoorsType;
        AmbientLightFactor = overlay.AmbientLightFactor;
        AddedLight = overlay.AddedLight;
        _safeQuit = overlay.SafeQuit;
        if (overlay.AtmosphereId != null)
        {
            Atmosphere = overlay.AtmosphereType.Equals("gas", StringComparison.InvariantCultureIgnoreCase)
                ? (IFluid)Gameworld.Gases.Get(overlay.AtmosphereId.Value)
                : Gameworld.Liquids.Get(overlay.AtmosphereId.Value);
        }

        _noSave = false;
    }
}
