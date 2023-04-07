using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Construction;

public class Room : Location, IDisposable, IRoom
{
	protected List<ICell> _cells = new();

	public Room(ICharacter initiator, ICellOverlayPackage package) : base(initiator.Location.Room.Zone.Gameworld)
	{
		Zone = initiator.Location.Room.Zone;

		using (new FMDB())
		{
			var dbroom = new Models.Room
			{
				ZoneId = initiator.Location.Room.Zone.Id
			};
			FMDB.Context.Rooms.Add(dbroom);
			FMDB.Context.SaveChanges();
			_id = dbroom.Id;
			_name = string.Empty;
			_cells.Add(new Cell(package, this));
		}

		Gameworld.Add(this);
		Zone.Register(this);
	}

	public Room(IZone zone, ICellOverlayPackage package, ICell templateCell, bool temporary) : base(zone.Gameworld)
	{
		Zone = zone;
		using (new FMDB())
		{
			var dbroom = new Models.Room
			{
				ZoneId = Zone.Id
			};
			FMDB.Context.Rooms.Add(dbroom);
			FMDB.Context.SaveChanges();
			_id = dbroom.Id;
			_name = string.Empty;
			_cells.Add(new Cell(package, this, templateCell, temporary));
		}

		Gameworld.Add(this);
		Zone.Register(this);
	}

	public Room(MudSharp.Models.Room room, IZone zone) : base(zone.Gameworld)
	{
		_id = room.Id;
		Zone = zone;
		_name = string.Empty;
		X = room.X;
		Y = room.Y;
		Z = room.Z;
		Zone.Register(this);
	}

	public string LocationString => $"{P}:{X},{Y},{Z}";

	public void Dispose()
	{
		Gameworld.Destroy(this);
		GC.SuppressFinalize(this);
	}

	public override string FrameworkItemType => "Room";
	public IEnumerable<ICell> Cells => _cells;

	public void Register(ICell cell)
	{
		_cells.Add(cell);
	}

	public void Destroy(ICell cell)
	{
		_cells.Remove(cell);
	}

	public void DestroyRoom(ICell fallbackCell)
	{
		var action = DestroyRoomWithDatabaseAction(fallbackCell);
		Gameworld.SaveManager.Flush();
		action.Invoke();
	}

	public Action DestroyRoomWithDatabaseAction(ICell fallbackCell)
	{
		var cellActions = new List<Action>();
		foreach (var cell in _cells.ToList())
		{
			cellActions.Add(cell.DestroyWithDatabaseAction(fallbackCell));
		}

		_cells.Clear();
		_gameItems.Clear();
		_characters.Clear();
		Gameworld.SaveManager.Abort(this);
		Gameworld.EffectScheduler.Destroy(this);
		Gameworld.Destroy(this);

		return () =>
		{
			EffectHandler.RemoveAllEffects();
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				foreach (var action in cellActions)
				{
					action?.Invoke();
				}

				var dbitem = FMDB.Context.Rooms.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.Rooms.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		};
	}

	public IZone Zone { get; private set; }

	public IShard Shard => Zone.Shard;

	public long P => Zone.P;

	public int X { get; set; }

	public int Y { get; set; }

	public int Z { get; set; }

	public override IEnumerable<ICalendar> Calendars => Zone.Calendars;

	public override IEnumerable<ICelestialObject> Celestials => Zone.Celestials;

	public override IEnumerable<IClock> Clocks => Zone.Clocks;

	public readonly List<IArea> _areas = new();
	public IEnumerable<IArea> Areas => _areas;

	public override IMudTimeZone TimeZone(IClock whichClock)
	{
		return Zone.TimeZone(whichClock);
	}

	public override void Insert(IGameItem thing, bool newStack)
	{
		base.Insert(thing, newStack);
		Zone.Insert(thing, newStack);
	}

	public override void Extract(IGameItem thing)
	{
		base.Extract(thing);
		Zone.Extract(thing);
	}

	public override void Enter(ICharacter movingCharacter, ICellExit exit = null, bool noSave = false,
		RoomLayer roomLayer = RoomLayer.GroundLevel)
	{
		base.Enter(movingCharacter, exit);
		Zone.Enter(movingCharacter, exit);
	}

	public override void Leave(ICharacter movingCharacter)
	{
		base.Leave(movingCharacter);
		Zone.Leave(movingCharacter);
	}

	public TimeOfDay CurrentTimeOfDay => Zone.CurrentTimeOfDay;

	public override CelestialInformation GetInfo(ICelestialObject celestial)
	{
		return Zone.GetInfo(celestial);
	}

	public override MudDate Date(ICalendar whichCalendar)
	{
		return Zone.Date(whichCalendar);
	}

	public override MudTime Time(IClock whichClock)
	{
		return Zone.Time(whichClock);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Rooms.Find(Id);
		dbitem.X = X;
		dbitem.Y = Y;
		dbitem.Z = Z;
		dbitem.ZoneId = Zone.Id;
		Changed = false;
	}

	#region IFutureProgVariable Implementation

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Error;

	#endregion

	public static void RegisterPerceivableType(IFuturemud gameworld)
	{
		gameworld.RegisterPerceivableType("Room", id => gameworld.Rooms.Get(id));
	}

	public void SetName(string name)
	{
		_name = name;
	}

	public void SetNewZone(IZone zone)
	{
		Zone.Unregister(this);
		Zone = zone;
		Zone.Register(this);
		Changed = true;
	}

	public void AddArea(IArea area)
	{
		if (!_areas.Contains(area))
		{
			_areas.Add(area);
			foreach (var cell in Cells)
			{
				cell.AreaAdded(area);
			}
		}
	}

	public void RemoveArea(IArea area)
	{
		if (_areas.Remove(area))
		{
			foreach (var cell in Cells)
			{
				cell.AreaRemoved(area);
			}
		}
	}
}