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
    public Area(ICell firstCell, string name) : base(firstCell.Gameworld)
    {
        using (new FMDB())
        {
            Areas dbitem = new() { Name = name };
            dbitem.AreasCells.Add(new AreasCells { Area = dbitem, CellId = firstCell.Id });
            FMDB.Context.Areas.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
            _name = name;
        }
        IdInitialised = true;
        Gameworld.Add(this);
        Attach(firstCell);
    }

    private void Attach(ICell cell)
    {
        _cells.Add(cell);
        cell.AddArea(this);
        cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
        cell.CellRequestsDeletion += Cell_CellRequestsDeletion;
    }

    private void Cell_CellRequestsDeletion(object sender, EventArgs e)
    {
        Remove((ICell)sender);
    }

    public Area(Areas area, IFuturemud gameworld) : base(gameworld)
    {
        _id = area.Id;
        IdInitialised = true;
        _name = area.Name;
        _weatherController = Gameworld.WeatherControllers.Get(area.WeatherControllerId ?? 0L);
        foreach (var membership in area.AreasCells)
        {
            var cell = Gameworld.Cells.Get(membership.CellId)
                ?? throw new InvalidOperationException($"Area #{Id} references missing cell #{membership.CellId}.");
            Attach(cell);
        }
    }

    public void Add(ICell cell)
    {
        if (_cells.Contains(cell)) return;
        Attach(cell);
        Changed = true;
    }

    public void Remove(ICell cell)
    {
        if (!_cells.Remove(cell)) return;
        cell.RemoveArea(this);
        cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
        Changed = true;
    }

    public override void Save()
    {
        Areas dbitem = FMDB.Context.Areas.Find(Id);
        dbitem.Name = Name;
        dbitem.WeatherControllerId = WeatherController?.Id;
		var desiredIds = _cells.Select(x => x.Id).ToHashSet();
		FMDB.Context.AreasCells.RemoveRange(dbitem.AreasCells.Where(x => !desiredIds.Contains(x.CellId)).ToList());
		var existingIds = dbitem.AreasCells.Select(x => x.CellId).ToHashSet();
		foreach (var cell in _cells.Where(x => !existingIds.Contains(x.Id)))
			dbitem.AreasCells.Add(new AreasCells { Area = dbitem, CellId = cell.Id });
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
			foreach (var cell in Cells.OfType<Cell>()) cell.RefreshWeatherSubscriptions();
            Changed = true;
        }
    }

    private readonly List<ICell> _cells = new();
    private IWeatherController _weatherController;


    public override IEnumerable<ICell> Cells => _cells;
    public IEnumerable<IZone> Zones => Cells.Select(x => x.OwningZone).Distinct();

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
                return new CollectionVariable(Cells.ToList(), ProgVariableTypes.Location);
            case "now":
                return new MudDateTime(Calendars.First().CurrentDate, Calendars.First().FeedClock.CurrentTime, Calendars.First().FeedClock.PrimaryTimezone);
            case "characters":
                return new CollectionVariable(Cells.SelectMany(x => x.Characters).ToList(), ProgVariableTypes.Character);
            case "items":
                return new CollectionVariable(Cells.SelectMany(x => x.GameItems).ToList(), ProgVariableTypes.Character);
            case "timeofday":
                return new TextVariable(CurrentTimeOfDay.DescribeEnum());
            default:
                return base.GetProperty(property);
        }
    }

    public override ProgVariableTypes Type => ProgVariableTypes.Area;

    #endregion
}
