using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Construction;

public class Shard : Location, IEditableShard
{
	protected List<ICalendar> _calendars = new();

	protected List<ICelestialObject> _celestials = new();

	protected List<IClock> _clocks = new();

	protected All<IRoom> _rooms = new();

	protected All<IZone> _zones = new();

	public IEnumerable<IZone> Zones => _zones;
	public IEnumerable<IRoom> Rooms => _rooms;
	public override IEnumerable<ICell> Cells => _rooms.SelectMany(x => x.Cells);

	public Shard(IFuturemud game, ISkyDescriptionTemplate skyTemplate, string name) : base(game)
	{
		using (new FMDB())
		{
			var dbitem = new Models.Shard
			{
				Name = name,
				MinimumTerrestrialLux = 0.000537,
				SkyDescriptionTemplateId = skyTemplate.Id,
				SphericalRadiusMetres = 6371000.0
			};
			FMDB.Context.Shards.Add(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDB(dbitem);
		}
	}

	public Shard(MudSharp.Models.Shard plane, IFuturemud game) : base(game)
	{
		LoadFromDB(plane);
	}

	public override string FrameworkItemType => "Shard";

	public void Register(IRoom room)
	{
		_rooms.Add(room);
	}

	public void Unregister(IRoom zone)
	{
		_rooms.Remove(zone);
	}

	public void Register(IZone zone)
	{
		_zones.Add(zone);
	}

	public void Unregister(IZone zone)
	{
		_zones.Remove(zone);
	}

	public IRoom DetermineRoomByCoordinates(int x, int y, int z)
	{
		return (from room in Gameworld.Rooms
		        where room.X == x && room.Y == y && room.Z == z
		        select room).FirstOrDefault();
	}

	public IRoom DetermineRoomByDirection(IRoom fromRoom, CardinalDirection direction)
	{
		var x = fromRoom.X;
		var y = fromRoom.Y;
		var z = fromRoom.Z;

		switch (direction)
		{
			case CardinalDirection.North:
				y++;
				break;
			case CardinalDirection.NorthEast:
				y++;
				x++;
				break;
			case CardinalDirection.East:
				x++;
				break;
			case CardinalDirection.SouthEast:
				x++;
				y--;
				break;
			case CardinalDirection.South:
				y--;
				break;
			case CardinalDirection.SouthWest:
				y--;
				x--;
				break;
			case CardinalDirection.West:
				x--;
				break;
			case CardinalDirection.NorthWest:
				x--;
				y++;
				break;
			case CardinalDirection.Up:
				z++;
				break;
			case CardinalDirection.Down:
				z--;
				break;
		}

		return (from room in Gameworld.Rooms
		        where room.X == x && room.Y == y && room.Z == z
		        select room).FirstOrDefault();
	}

	public override IEnumerable<ICalendar> Calendars => _calendars;
	public override IEnumerable<IClock> Clocks => _clocks;
	public override IEnumerable<ICelestialObject> Celestials => _celestials;

	public override IMudTimeZone TimeZone(IClock whichClock)
	{
		return whichClock.PrimaryTimezone;
	}

	public override CelestialInformation GetInfo(ICelestialObject celestial)
	{
		return null;
	}

	public double MinimumTerrestrialLux { get; set; }

	public double SphericalRadiusMetres { get; set; } = 6371000.0;

	public ISkyDescriptionTemplate SkyDescriptionTemplate { get; set; }

	public string DescribeSky(double skyBrightness)
	{
		return
			SkyDescriptionTemplate.SkyDescriptions.Find(
				LuxToSkyBrightness(Math.Max(skyBrightness, MinimumTerrestrialLux)));
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Shards.Find(Id);
			dbitem.Name = Name;
			dbitem.MinimumTerrestrialLux = MinimumTerrestrialLux;
			dbitem.SkyDescriptionTemplateId = SkyDescriptionTemplate.Id;
			dbitem.SphericalRadiusMetres = SphericalRadiusMetres;
			FMDB.Context.ShardsCalendars.RemoveRange(dbitem.ShardsCalendars);
			foreach (var item in Calendars)
			{
				var newItem = new ShardsCalendars();
				FMDB.Context.ShardsCalendars.Add(newItem);
				newItem.CalendarId = item.Id;
				newItem.Shard = dbitem;
			}

			FMDB.Context.ShardsClocks.RemoveRange(dbitem.ShardsClocks);
			foreach (var item in Clocks)
			{
				var newItem = new ShardsClocks();
				FMDB.Context.ShardsClocks.Add(newItem);
				newItem.ClockId = item.Id;
				newItem.Shard = dbitem;
			}

			FMDB.Context.ShardsCelestials.RemoveRange(dbitem.ShardsCelestials);
			foreach (var item in Celestials)
			{
				var newItem = new ShardsCelestials();
				FMDB.Context.ShardsCelestials.Add(newItem);
				newItem.CelestialId = item.Id;
				newItem.Shard = dbitem;
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public IEditableShard GetEditableShard => this;

	public static void RegisterPerceivableType(IFuturemud gameworld)
	{
		gameworld.RegisterPerceivableType("Shard", id => gameworld.Shards.Get(id));
	}

	private void LoadFromDB(MudSharp.Models.Shard plane)
	{
		_name = plane.Name;
		_id = plane.Id;
		MinimumTerrestrialLux = plane.MinimumTerrestrialLux;

		foreach (var clock in plane.ShardsClocks)
		{
			_clocks.Add(Gameworld.Clocks.Get(clock.ClockId));
		}

		foreach (var calendar in plane.ShardsCalendars)
		{
			_calendars.Add(Gameworld.Calendars.Get(calendar.CalendarId));
		}

		foreach (var celestial in plane.ShardsCelestials)
		{
			_celestials.Add(Gameworld.CelestialObjects.Get(celestial.CelestialId));
		}

		SkyDescriptionTemplate = Gameworld.SkyDescriptionTemplates.Get(plane.SkyDescriptionTemplateId);
		SphericalRadiusMetres = plane.SphericalRadiusMetres;
		//LoadEffects();
	}

	public void Dispose()
	{
		Gameworld.Destroy(this);
		GC.SuppressFinalize(this);
	}

	private double LuxToSkyBrightness(double lux)
	{
		return 12.58 - 2.5 * Math.Log10(lux * 1.08 / 3.4);
	}


	public IWeatherController WeatherController => null;

	#region IFutureProgVariable Members

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "type", ProgVariableTypes.Text },
			{ "effects", ProgVariableTypes.Effect | ProgVariableTypes.Collection },
			{ "zones", ProgVariableTypes.Zone | ProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "type", "Returns the name of the framework item type, for example, character or gameitem or clan" },
			{ "effects", "" },
			{ "zones", "" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Shard, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public override IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "zones":
				return new CollectionVariable(_zones.ToList(), ProgVariableTypes.Zone);
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "type":
				return new TextVariable(FrameworkItemType);
			case "effects":
				return new CollectionVariable(EffectHandler.Effects.Where(x => x.Applies()).ToList(),
					ProgVariableTypes.Effect);
			default:
				return base.GetProperty(property);
		}
	}

	public override ProgVariableTypes Type => ProgVariableTypes.Shard;

	#endregion

	#region IEditableShard Members

	List<IClock> IEditableShard.Clocks => _clocks;

	List<ICalendar> IEditableShard.Calendars => _calendars;

	List<ICelestialObject> IEditableShard.Celestials => _celestials;

	public void SetName(string name)
	{
		_name = name;
	}

	#endregion
}