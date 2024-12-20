﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
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

	public IEnumerable<ICell> Cells => Rooms.SelectMany(x => x.Cells).Distinct();
	public IEnumerable<IZone> Zones => Rooms.Select(x => x.Zone).Distinct();

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

	public override ProgVariableTypes Type { get; } = ProgVariableTypes.Error;
	public override string FrameworkItemType => "Area";
}