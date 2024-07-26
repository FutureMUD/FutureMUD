using MudSharp.Models;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System.Collections.Generic;

namespace MudSharp.TimeAndDate.Time;

public class MudTimeZone : SaveableItem, IMudTimeZone
{
	protected string _alias;
	protected string _description;
	protected int _offsetHours;
	protected int _offsetMinutes;
	public IClock Clock { get; }

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Timezones.Find(Id);
		dbitem.Name = Alias;
		dbitem.Description = Description;
		dbitem.OffsetHours = OffsetHours;
		dbitem.OffsetMinutes = OffsetMinutes;
		Changed = false;
	}

	public MudTimeZone(IClock clock, int offsethours, int offsetminutes, string description, string alias)
	{
		Gameworld = clock.Gameworld;
		Clock = clock;
		using (new FMDB())
		{
			var dbitem = new Timezone();
			FMDB.Context.Timezones.Add(dbitem);
			dbitem.Name = alias;
			dbitem.Description = description;
			dbitem.OffsetHours = offsethours;
			dbitem.OffsetMinutes = offsetminutes;
			dbitem.ClockId = clock.Id;
			FMDB.Context.SaveChanges();
			LoadFromDB(dbitem);
		}
	}

	public MudTimeZone(int id, int offsethours, int offsetminutes, string description, string alias)
	{
		_id = id;
		_offsetHours = offsethours;
		_offsetMinutes = offsetminutes;
		_description = description;
		_alias = alias;
		_name = Alias;
	}

	public MudTimeZone(Timezone zone, IClock clock, IFuturemud game)
	{
		Gameworld = game;
		Clock = clock;
		LoadFromDB(zone);
	}

	public override string FrameworkItemType => "MudTimeZone";

	public int OffsetHours
	{
		get => _offsetHours;
		set
		{
			_offsetHours = value;
			Changed = true;
		}
	}

	public int OffsetMinutes
	{
		get => _offsetMinutes;
		set { _offsetMinutes = value; Changed = true; }
	}

	public string Description
	{
		get => _description;
		set { _description = value; Changed = true; }
	}

	public string Alias
	{
		get => _alias;
		set
		{
			_alias = value;
			_name = value;
		}
	}

	IEnumerable<string> IHaveMultipleNames.Names => [Name, Alias];

	private void LoadFromDB(Timezone zone)
	{
		_id = zone.Id;
		_offsetHours = zone.OffsetHours;
		_offsetMinutes = zone.OffsetMinutes;
		_description = zone.Description;
		_alias = zone.Name;
		_name = Alias;
	}

	public override string ToString()
	{
		return $"Timezone {Alias} {OffsetHours}h {OffsetMinutes}m";
	}

	// TODO - Daylight savings times
}