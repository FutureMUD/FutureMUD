using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Foraging;

namespace MudSharp.Construction;

public class Zone : Location, IEditableZone
{
	protected readonly All<IRoom> _rooms = new();

	protected readonly Dictionary<ICelestialObject, CelestialInformation> CelestialInfo =
		new();

	private readonly Dictionary<ICelestialObject, double> LightLevelDictionary =
		new();

	private double _ambientLightPollution;

	private GeographicCoordinate _geography;

	public IEnumerable<IRoom> Rooms => _rooms;
	public override IEnumerable<ICell> Cells => _rooms.SelectMany(x => x.Cells);

	public Zone(MudSharp.Models.Zone zone, IFuturemud game) : base(game)
	{
		_noSave = true;
		_id = zone.Id;
		_name = zone.Name;
		Shard = game.Shards.Get(zone.ShardId);
		AmbientLightPollution = zone.AmbientLightPollution;
		Geography = new GeographicCoordinate(zone.Latitude, zone.Longitude, zone.Elevation,
			Shard.SphericalRadiusMetres);
		_foragableProfileId = zone.ForagableProfileId ?? 0;

		TimeZones = new Dictionary<IClock, IMudTimeZone>();
		foreach (var timezone in zone.ZonesTimezones)
		{
			TimeZones.Add(Gameworld.Clocks.Get(timezone.ClockId),
				Gameworld.Clocks.Get(timezone.ClockId).Timezones.Get(timezone.TimezoneId));
		}

		foreach (var clock in Clocks.Where(x => !TimeZones.ContainsKey(x)))
		{
			TimeZones[clock] = clock.PrimaryTimezone;
		}

		WeatherController = Gameworld.WeatherControllers.Get(zone.WeatherControllerId ?? 0L);

		InitialiseCelestials();
		Shard.Register(this);
		_noSave = false;
		//LoadEffects();
	}

	public override string FrameworkItemType => "Zone";

	public IShard Shard { get; }

	public ICell DefaultCell => Cells.FirstOrDefault();

	public long P => Shard.Id;

	public double AmbientLightPollution
	{
		get => _ambientLightPollution;
		set
		{
			_ambientLightPollution = value;
			if (!_noSave)
			{
				Changed = true;
			}
		}
	}

	public TimeOfDay CurrentTimeOfDay
	{
		get
		{
			var sunPosition = Celestials.FirstOrDefault(x => x.CelestialAngleIsUsedToDetermineTimeOfDay)
			                            ?.CurrentPosition(Geography);
			if (sunPosition == null)
			{
				return TimeOfDay.Night;
			}

			if (sunPosition.LastAscensionAngle > 0.05)
			{
				if (sunPosition.Direction == CelestialMoveDirection.Ascending)
				{
					return TimeOfDay.Morning;
				}

				return TimeOfDay.Afternoon;
			}

			// 12 degrees below horizon is a good general measure of the beginning of dawn or end of dusk
			if (sunPosition.LastAscensionAngle < -0.20944)
			{
				return TimeOfDay.Night;
			}

			if (sunPosition.Direction == CelestialMoveDirection.Ascending)
			{
				return TimeOfDay.Dawn;
			}

			return TimeOfDay.Dusk;
		}
	}

	// TODO - weather effects
	public double CurrentLightLevel { get; private set; }

	public void RecalculateLightLevel()
	{
		CurrentLightLevel = AmbientLightPollution + LightLevelDictionary.Sum(x => x.Value) +
		                    EffectsOfType<IAreaLightEffect>(x => x.Applies()).Sum(x => x.AddedLight);
	}

	public void Register(IRoom room)
	{
		_rooms.Add(room);
		Shard.Register(room);
	}

	public void Unregister(IRoom room)
	{
		_rooms.Remove(room);
		Shard.Unregister(room);
	}

	public GeographicCoordinate Geography
	{
		get => _geography;
		set
		{
			_geography = value;
			Changed = true;
		}
	}

	public override CelestialInformation GetInfo(ICelestialObject celestial)
	{
		return CelestialInfo.ContainsKey(celestial) ? CelestialInfo[celestial] : null;
	}

	public void SetName(string name)
	{
		_name = name;
		Changed = true;
	}

	public override void Enter(ICharacter movingCharacter, ICellExit exit = null, bool noSave = false,
		RoomLayer roomLayer = RoomLayer.GroundLevel)
	{
		base.Enter(movingCharacter, exit);
		Shard.Enter(movingCharacter, exit);
	}

	public override void Leave(ICharacter movingCharacter)
	{
		base.Leave(movingCharacter);
		Shard.Leave(movingCharacter);
	}

	public override void Extract(IGameItem thing)
	{
		base.Extract(thing);
		Shard.Extract(thing);
	}

	public override void Insert(IGameItem thing, bool newStack)
	{
		base.Insert(thing, newStack);
		Shard.Insert(thing, newStack);
	}

	public override IEnumerable<ICalendar> Calendars => Shard.Calendars;

	public override IEnumerable<ICelestialObject> Celestials => Shard.Celestials;

	public override IEnumerable<IClock> Clocks => Shard.Clocks;

	private IWeatherController _weather;

	public IWeatherController WeatherController
	{
		get => _weather;
		set
		{
			_weather = value;
			Changed = true;
		}
	}

	public override IMudTimeZone TimeZone(IClock whichClock)
	{
		return TimeZones.TryGetValue(whichClock ?? Clocks.FirstOrDefault(), out var value)
			? value
			: throw new ApplicationException(
				"Zone.TimeZone(whichClock) was asked for a timezone for a clock it doesn't have.");
	}

	public override MudDate Date(ICalendar whichCalendar)
	{
		if (Calendars.Contains(whichCalendar))
		{
			return Calendars.First(x => x == whichCalendar).CurrentDate.GetDateByTime(Time(whichCalendar.FeedClock));
		}

		return Calendars.First().CurrentDate.GetDateByTime(Time(Calendars.First().FeedClock));
	}

	public Dictionary<IClock, IMudTimeZone> TimeZones { get; protected set; }

	public override MudTime Time(IClock whichClock)
	{
		if (Clocks.Contains(whichClock))
		{
			return
				Clocks.First(x => x == whichClock)
				      .CurrentTime
				      .GetTimeByTimezone(TimeZones[Clocks.First(x => x == whichClock)]);
		}

		return
			Clocks.First()
			      .CurrentTime
			      .GetTimeByTimezone(TimeZones[Clocks.First()]);
	}

	public string DescribeSky => Shard.DescribeSky(CurrentLightLevel);

	public IEditableZone GetEditableZone => this;

	public override void Save()
	{
		using (new FMDB())
		{
			var dbzone = FMDB.Context.Zones.Find(Id);
			dbzone.DefaultCellId = DefaultCell?.Id;
			dbzone.AmbientLightPollution = AmbientLightPollution;
			dbzone.Latitude = Geography.Latitude;
			dbzone.Longitude = Geography.Longitude;
			dbzone.Elevation = Geography.Elevation;
			dbzone.Name = Name;
			dbzone.ForagableProfileId = ForagableProfile?.Id;
			FMDB.Context.ZonesTimezones.RemoveRange(dbzone.ZonesTimezones);
			foreach (var timezone in TimeZones)
			{
				var dbtz = new ZonesTimezones
				{
					TimezoneId = timezone.Value.Id,
					ClockId = timezone.Key.Id,
					Zone = dbzone
				};
				FMDB.Context.ZonesTimezones.Add(dbtz);
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public static void RegisterPerceivableType(IFuturemud gameworld)
	{
		gameworld.RegisterPerceivableType("Zone", id => gameworld.Zones.Get(id));
	}

	public void InitialiseCelestials()
	{
		foreach (var celestial in Celestials)
		{
			celestial.MinuteUpdateEvent -= celestial_MinuteUpdateEvent;
			celestial.MinuteUpdateEvent += celestial_MinuteUpdateEvent;
			CelestialInfo[celestial] = celestial.ReturnNewCelestialInformation(this, null, Geography);
			LightLevelDictionary[celestial] = celestial.CurrentIllumination(Geography);
		}

		RecalculateLightLevel();
	}

	public void DeregisterCelestials()
	{
		foreach (var celestial in Celestials)
		{
			celestial.MinuteUpdateEvent -= celestial_MinuteUpdateEvent;
		}
	}

	private void celestial_MinuteUpdateEvent(ICelestialObject sender)
	{
		CelestialInfo[sender] = sender.ReturnNewCelestialInformation(this, CelestialInfo[sender], Geography);
		LightLevelDictionary[sender] = sender.CurrentIllumination(Geography);
		RecalculateLightLevel();
	}

	public IRoom DetermineRoomByCoordinates(int x, int y, int z)
	{
		return Shard.DetermineRoomByCoordinates(x, y, z);
	}

	public IRoom DetermineRoomByDirection(IRoom fromRoom, CardinalDirection direction)
	{
		return Shard.DetermineRoomByDirection(fromRoom, direction);
	}

	public void PostLoadSetup()
	{
		_noSave = true;
		// Insert anything that needs to happen after all cells are loaded
		_noSave = false;
	}

	private void CellCoordinateVisitor(ICell c, int x, int y, int z, ref HashSet<ICell> alreadyTouched)
	{
		c.Room.X = x;
		c.Room.Y = y;
		c.Room.Z = z;
		c.Room.Changed = true;
		alreadyTouched.Add(c);
		foreach (var exit in c.ExitsFor(null))
		{
			if (alreadyTouched.Contains(exit.Destination))
			{
				continue;
			}

			CellCoordinateVisitor(
				exit.Destination,
				x + exit.OutboundDirection.Eastness() - exit.OutboundDirection.Westness(),
				y + exit.OutboundDirection.Northness() - exit.OutboundDirection.Southness(),
				z + exit.OutboundDirection.Upness() - exit.OutboundDirection.Downness(),
				ref alreadyTouched
			);
		}
	}

	public void CalculateCoordinates()
	{
		var alreadyTouched = new HashSet<ICell>();
		if (!Cells.Any())
		{
			return;
		}

		CellCoordinateVisitor(DefaultCell ?? Cells.First(), 0, 0, 0, ref alreadyTouched);
		var zoneCells = Rooms.SelectMany(x => x.Cells);
		var missingCells = zoneCells.Where(x => !alreadyTouched.Contains(x)).ToList();
		if (missingCells.Any())
		{
			Console.WriteLine(
				$"Warning: The following cells in zone {Id:N0} ({Name}) were not linked to the main grid:\n{missingCells.Select(x => x.Id.ToString("N0")).ArrangeStringsOntoLines(7, 60)}");
		}
	}

	public string ShowToBuilder(ICharacter builder)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Zone #{Id.ToString("N0", builder)} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Shard: {Shard.Name.Colour(Telnet.Green)}");
		if (DefaultCell != null)
		{
			sb.AppendLine($"Default Cell: {DefaultCell.HowSeen(builder)} #{DefaultCell.Id.ToString("N0", builder)}");
		}
		else
		{
			sb.AppendLine($"Default Cell: {"None".Colour(Telnet.Red)}");
		}

		sb.AppendLine(
			$"Latitude: {Geography.Latitude.RadiansToDegrees().ToString("N5", builder).Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Longitude: {Geography.Longitude.RadiansToDegrees().ToString("N5", builder).Colour(Telnet.Green)}");
		sb.AppendLine($"Elevation: {Geography.Elevation.ToString("N0", builder).Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Ambient Light Pollution: {$"{AmbientLightPollution.ToString("N5", builder)} lumens".Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Weather Controller: {(WeatherController != null ? $"{WeatherController.Name.Colour(Telnet.Green)} (#{WeatherController.Id.ToString("N0", builder)})" : "None".Colour(Telnet.Red))}");
		return sb.ToString();
	}

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
			{ "latitude", ProgVariableTypes.Number },
			{ "longitude", ProgVariableTypes.Number },
			{ "elevation", ProgVariableTypes.Number },
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
			{ "id", "The ID of the zone" },
			{ "name", "The name of the zone" },
			{ "type", "Returns the name of the framework item type, for example, character or gameitem or clan" },
			{ "effects", "A collection of effects on this zone" },
			{ "latitude", "The latitude in degrees" },
			{ "longitude", "The longitude in degrees" },
			{ "elevation", "The elevation in metres" },
			{ "rooms", "A collection of the rooms in this zone" },
			{ "now", "The current datetime in this zone" },
			{ "characters", "The characters in this zone" },
			{ "items", "The items in this zone" },
			{ "timeofday", "The current time of day in this zone" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Zone, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public override IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "rooms":
				return new CollectionVariable(_rooms.SelectMany(x => x.Cells).ToList(),
					ProgVariableTypes.Location);
			case "latitude":
				return new NumberVariable(Geography.Latitude.RadiansToDegrees());
			case "longitude":
				return new NumberVariable(Geography.Longitude.RadiansToDegrees());
			case "elevation":
				return new NumberVariable(Geography.Elevation);
			case "now":
				return new MudDateTime(Calendars.First().CurrentDate, Calendars.First().FeedClock.CurrentTime,
					Calendars.First().FeedClock.PrimaryTimezone);
			case "characters":
				return new CollectionVariable(Characters.ToList(), ProgVariableTypes.Character);
			case "items":
				return new CollectionVariable(GameItems.ToList(), ProgVariableTypes.Character);
			case "timeofday":
				return new TextVariable(CurrentTimeOfDay.DescribeEnum());
			default:
				return base.GetProperty(property);
		}
	}

	public override ProgVariableTypes Type => ProgVariableTypes.Zone;

	#endregion

	#region IEditableZone Members

	private long _foragableProfileId;
	private IForagableProfile _foragableProfile;

	public IForagableProfile ForagableProfile
	{
		get
		{
			if (_foragableProfileId != 0)
			{
				_foragableProfile = Gameworld.ForagableProfiles.Get(_foragableProfileId);
				_foragableProfileId = 0;
			}

			return _foragableProfile;
		}
		set
		{
			_foragableProfile = value;
			_foragableProfileId = 0;
			Changed = true;
		}
	}

	#endregion
}