using MudSharp.Models;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Time;

public class MudTimeZone : FrameworkItem, IMudTimeZone
{
	protected string _alias;
	protected string _description;
	protected int _offsetHours;
	protected int _offsetMinutes;
	public IClock Clock { get; }

	public MudTimeZone(IClock clock, int offsethours, int offsetminutes, string description, string alias)
	{
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

		Clock = clock;
	}

	public MudTimeZone(int id, int offsethours, int offsetminutes, string description, string alias)
	{
		_id = id;
		OffsetHours = offsethours;
		OffsetMinutes = offsetminutes;
		Description = description;
		Alias = alias;
		_name = Alias;
	}

	public MudTimeZone(Timezone zone, IClock clock, IFuturemud game)
	{
		Clock = clock;
		LoadFromDB(zone);
	}

	public override string FrameworkItemType => "MudTimeZone";

	public int OffsetHours
	{
		get => _offsetHours;
		protected set => _offsetHours = value;
	}

	public int OffsetMinutes
	{
		get => _offsetMinutes;
		protected set => _offsetMinutes = value;
	}

	public string Description
	{
		get => _description;
		protected set => _description = value;
	}

	public string Alias
	{
		get => _alias;
		protected set => _alias = value;
	}

	private void LoadFromDB(Timezone zone)
	{
		_id = zone.Id;
		OffsetHours = zone.OffsetHours;
		OffsetMinutes = zone.OffsetMinutes;
		Description = zone.Description;
		Alias = zone.Name;
		_name = Alias;
	}

	public override string ToString()
	{
		return $"Timezone {Alias} {OffsetHours}h {OffsetMinutes}m";
	}

	// TODO - Daylight savings times
}