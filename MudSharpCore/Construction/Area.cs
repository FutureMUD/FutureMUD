using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Form.Material;
using MudSharp.Database;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Construction;

public class Area : Location, IEditableArea
{
    public Area(IRoom firstRoom, string name) : base(firstRoom.Gameworld)
    {
        using (new FMDB())
        {
            Areas dbitem = new() { Name = name };
            dbitem.AreasRooms.Add(new AreasRooms { Area = dbitem, RoomId = firstRoom.Id });
            FMDB.Context.Areas.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
            _name = name;
        }
        IdInitialised = true;
        Gameworld.Add(this);
        Attach(firstRoom);
    }

    private void Attach(IRoom room)
    {
        _cells.Add(room);
        room.AddArea(this);
        room.RoomRequestsDeletion -= Room_RoomRequestsDeletion;
        room.RoomRequestsDeletion += Room_RoomRequestsDeletion;
    }

    private void Room_RoomRequestsDeletion(object sender, EventArgs e)
    {
        Remove((IRoom)sender);
    }

    public Area(Areas area, IFuturemud gameworld) : base(gameworld)
    {
        _id = area.Id;
        IdInitialised = true;
        _name = area.Name;
        _weatherController = Gameworld.WeatherControllers.Get(area.WeatherControllerId ?? 0L);
        foreach (var membership in area.AreasRooms)
        {
            var room = Gameworld.Rooms.Get(membership.RoomId)
                ?? throw new InvalidOperationException($"Area #{Id} references missing cell #{membership.RoomId}.");
            Attach(room);
        }
    }

    public void Add(IRoom room)
    {
        if (_cells.Contains(room)) return;
        Attach(room);
        Changed = true;
    }

    public void Remove(IRoom room)
    {
        if (!_cells.Remove(room)) return;
        room.RemoveArea(this);
        room.RoomRequestsDeletion -= Room_RoomRequestsDeletion;
        Changed = true;
    }

    public override void Save()
    {
        Areas dbitem = FMDB.Context.Areas.Find(Id);
        dbitem.Name = Name;
        dbitem.WeatherControllerId = WeatherController?.Id;
		var desiredIds = _cells.Select(x => x.Id).ToHashSet();
		FMDB.Context.AreasRooms.RemoveRange(dbitem.AreasRooms.Where(x => !desiredIds.Contains(x.RoomId)).ToList());
		var existingIds = dbitem.AreasRooms.Select(x => x.RoomId).ToHashSet();
		foreach (var room in _cells.Where(x => !existingIds.Contains(x.Id)))
			dbitem.AreasRooms.Add(new AreasRooms { Area = dbitem, RoomId = room.Id });
        Changed = false;
    }

    public override IWeatherController WeatherController
    {
        get => _weatherController;
        set
        {
			if (ReferenceEquals(_weatherController, value)) return;
			using var exposureChange = EnvironmentalExposureService.ChangingDefinitions(Gameworld);
            _weatherController = value;
			foreach (var room in Rooms.OfType<Room>()) room.RefreshWeatherSubscriptions();
            Changed = true;
        }
    }

    private readonly List<IRoom> _cells = new();
    private IWeatherController _weatherController;


    public override IEnumerable<IRoom> Rooms => _cells;
    public IEnumerable<IZone> Zones => Rooms.Select(x => x.OwningZone).Distinct();

    #region Overrides of Location

    /// <inheritdoc />
    public override IEnumerable<ICalendar> Calendars => Zones.SelectMany(x => x.Calendars).Distinct();

    /// <inheritdoc />
    public override IEnumerable<IClock> Clocks => Zones.SelectMany(x => x.Clocks).Distinct();

    /// <inheritdoc />
    public override IEnumerable<ICelestialObject> Celestials => Zones.SelectMany(x => x.Celestials).Distinct();

    #endregion

    public override CelestialInformation GetInfo(ICelestialObject celestial)
    {
        return Zones.FirstOrDefault(x => x.Celestials.Contains(celestial))?.GetInfo(celestial);
    }

    public override IMudTimeZone TimeZone(IClock whichClock)
    {
        return Zones.FirstOrDefault(x => x.Clocks.Contains(whichClock))?.TimeZone(whichClock);
    }

    public TimeOfDay CurrentTimeOfDay => Zones.FirstOrDefault()?.CurrentTimeOfDay ?? TimeOfDay.Night;

    void IEditableArea.SetName(string name)
    {
        _name = name;
        Changed = true;
    }

    public override string FrameworkItemType => "Area";

    #region IFutureProgVariable Members

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "name", ProgVariableTypes.Text },
            { "type", ProgVariableTypes.Text },
            { "effects", ProgVariableTypes.Effect | ProgVariableTypes.Collection },
            { "rooms", ProgVariableTypes.Collection | ProgVariableTypes.Location },
            { "now", ProgVariableTypes.MudDateTime },
            { "characters", ProgVariableTypes.Character | ProgVariableTypes.Collection },
            { "items", ProgVariableTypes.Item | ProgVariableTypes.Collection },
            { "timeofday", ProgVariableTypes.Text }
        };
    }

    private static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", "The ID of the area" },
            { "name", "The name of the area" },
            { "type", "Returns the name of the framework item type, for example, character or gameitem or clan" },
            { "effects", "A collection of effects on this area" },
            { "rooms", "A collection of the rooms in this area" },
            { "now", "The current datetime in this area" },
            { "characters", "The characters in this area" },
            { "items", "The items in this area" },
            { "timeofday", "The current time of day in this area" }
        };
    }

    public new static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Area, DotReferenceHandler(),
            DotReferenceHelp());
    }

    public override IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "rooms":
                return new CollectionVariable(Rooms.ToList(), ProgVariableTypes.Location);
            case "now":
                return new MudDateTime(Calendars.First().CurrentDate, Calendars.First().FeedClock.CurrentTime, Calendars.First().FeedClock.PrimaryTimezone);
            case "characters":
                return new CollectionVariable(Rooms.SelectMany(x => x.Characters).ToList(), ProgVariableTypes.Character);
            case "items":
                return new CollectionVariable(Rooms.SelectMany(x => x.GameItems).ToList(), ProgVariableTypes.Character);
            case "timeofday":
                return new TextVariable(CurrentTimeOfDay.DescribeEnum());
            default:
                return base.GetProperty(property);
        }
    }

    public override ProgVariableTypes Type => ProgVariableTypes.Area;

    #endregion
}
