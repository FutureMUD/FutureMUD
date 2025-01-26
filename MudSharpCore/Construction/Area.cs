using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
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
		Gameworld = Gameworld;
		using (new FMDB())
		{
			var dbitem = new Areas();
			FMDB.Context.Areas.Add(dbitem);
			dbitem.Name = name;
			dbitem.AreasRooms.Add(new AreasRooms { Area = dbitem, RoomId = firstRoom.Id });
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = name;
			_rooms.Add(firstRoom);
		}

		IdInitialised = true;
		Gameworld.Add(this);
		firstRoom.AddArea(this);
		foreach (var cell in firstRoom.Cells)
		{
			cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
			cell.CellRequestsDeletion += Cell_CellRequestsDeletion;
		}
	}

	private void Cell_CellRequestsDeletion(object sender, EventArgs e)
	{
		var cell = (ICell)sender;
		_rooms.RemoveAll(x => x.Cells.Contains(cell));
		cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
		Changed = true;
	}

	public Area(Areas area, IFuturemud gameworld) : base(gameworld)
	{
		Gameworld = gameworld;
		_id = area.Id;
		IdInitialised = true;
		_name = area.Name;
		Weather = Gameworld.WeatherControllers.Get(area.WeatherControllerId ?? 0L);
		foreach (var dbroom in area.AreasRooms)
		{
			var room = Gameworld.Rooms.Get(dbroom.RoomId);
			_rooms.Add(room);
			room.AddArea(this);
			foreach (var cell in room.Cells)
			{
				cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
				cell.CellRequestsDeletion += Cell_CellRequestsDeletion;
			}
		}
	}

	public void Add(IRoom room)
	{
		if (!_rooms.Contains(room))
		{
			_rooms.Add(room);
			Changed = true;
			room.AddArea(this);
			foreach (var cell in room.Cells)
			{
				cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
				cell.CellRequestsDeletion += Cell_CellRequestsDeletion;
			}
		}
	}

	public void Remove(IRoom room)
	{
		if (_rooms.Remove(room))
		{
			Changed = true;
			room.RemoveArea(this);
			foreach (var cell in room.Cells)
			{
				cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
			}
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Areas.Find(Id);
		dbitem.Name = Name;
		dbitem.WeatherControllerId = Weather?.Id;
		FMDB.Context.AreasRooms.RemoveRange(dbitem.AreasRooms);
		foreach (var room in _rooms)
		{
			dbitem.AreasRooms.Add(new AreasRooms { Area = dbitem, RoomId = room.Id });
		}
		
		Changed = false;
	}

	public IWeatherController Weather
	{
		get => _weather;
		set
		{
			_weather = value;
			Changed = true;
		}
	}

	private readonly List<IRoom> _rooms = new();
	private IWeatherController _weather;

	public IEnumerable<IRoom> Rooms => _rooms;

	public override IEnumerable<ICell> Cells => Rooms.SelectMany(x => x.Cells).Distinct();
	public IEnumerable<IZone> Zones => Rooms.Select(x => x.Zone).Distinct();

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