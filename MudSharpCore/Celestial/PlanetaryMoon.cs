using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Time;
using MudSharp.TimeAndDate.Date;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Celestial;

/// <summary>
///     A simple celestial object representing a moon orbiting a planet. The implementation
///     mirrors the approach used in <see cref="NewSun"/> where calculations are based on the
///     current day number rather than updating each minute.
/// </summary>
public class PlanetaryMoon : PerceivedItem, ICelestialObject
{
	public override string FrameworkItemType => "Celestial";

	public ICalendar Calendar { get; set; }
	public IClock Clock { get; set; }

	public double MeanAnomalyAngleAtEpoch { get; set; }
	public double AnomalyChangeAnglePerDay { get; set; }
	public double ArgumentOfPeriapsis { get; set; }
	public double LongitudeOfAscendingNode { get; set; }
	public double OrbitalInclination { get; set; }
	public double OrbitalEccentricity { get; set; }

	public MudDate EpochDate { get; set; }
	public double DayNumberAtEpoch { get; set; }
	public double SiderealTimeAtEpoch { get; set; }
	public double SiderealTimePerDay { get; set; }

	public double PeakIllumination { get; set; }
	public double FullMoonReferenceDay { get; set; }

	public List<CelestialTrigger> Triggers { get; } = new();
	protected readonly CircularRange<string> AzimuthDescriptions = new();
	protected readonly CircularRange<string> ElevationDescriptions = new();

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("Illumination");
		if (element != null)
		{
			PeakIllumination = element.Element("PeakIllumination")?.Value.GetDouble() ?? 0;
			FullMoonReferenceDay = element.Element("FullMoonReferenceDay")?.Value.GetDouble() ?? 0;
		}

		element = root.Element("Calendar");
		Calendar = Gameworld.Calendars.Get(long.Parse(element.Value));

		element = root.Element("Orbital");
		CelestialDaysPerYear = element.Element("CelestialDaysPerYear")?.Value.GetDouble() ?? 0;
		MeanAnomalyAngleAtEpoch = element.Element("MeanAnomalyAngleAtEpoch")?.Value.GetDouble() ?? 0;
		AnomalyChangeAnglePerDay = element.Element("AnomalyChangeAnglePerDay")?.Value.GetDouble() ?? 0;
		ArgumentOfPeriapsis = element.Element("ArgumentOfPeriapsis")?.Value.GetDouble() ?? 0;
		LongitudeOfAscendingNode = element.Element("LongitudeOfAscendingNode")?.Value.GetDouble() ?? 0;
		OrbitalInclination = element.Element("OrbitalInclination")?.Value.GetDouble() ?? 0;
		OrbitalEccentricity = element.Element("OrbitalEccentricity")?.Value.GetDouble() ?? 0;
		DayNumberAtEpoch = element.Element("DayNumberAtEpoch")?.Value.GetDouble() ?? 0;
		SiderealTimeAtEpoch = element.Element("SiderealTimeAtEpoch")?.Value.GetDouble() ?? 0;
		SiderealTimePerDay = element.Element("SiderealTimePerDay")?.Value.GetDouble() ?? 0;
		EpochDate = Calendar.GetDate(element.Element("EpochDate").Value);

		element = root.Element("Triggers");
		if (element != null)
		{
			foreach (var sub in element.Elements("Trigger"))
			{
				Triggers.Add(
						new CelestialTrigger(
								sub.Attribute("angle").Value.GetDouble() ?? 0,
								sub.Attribute("direction").Value == "Ascending"
										? CelestialMoveDirection.Ascending
										: CelestialMoveDirection.Descending,
								sub.Value));
			}
		}

		element = root.Element("Name");
		if (element != null)
		{
			_name = element.Value;
		}
	}

	public PlanetaryMoon(ICalendar calendar, IClock clock)
	{
		Calendar = calendar;
		Clock = clock;
		Clock.MinutesUpdated += AddMinutes;
	}

	public PlanetaryMoon(XElement root, IClock clock, IFuturemud game)
	{
		Gameworld = game;
		Clock = clock;
		Clock.MinutesUpdated += AddMinutes;
		LoadFromXml(root);
	}

	public PlanetaryMoon(MudSharp.Models.Celestial celestial, IFuturemud game)
	{
		IdInitialised = true;
		_id = celestial.Id;
		Gameworld = game;
		Clock = Gameworld.Clocks.Get(celestial.FeedClockId);
		Clock.MinutesUpdated += AddMinutes;
		LoadFromXml(XElement.Parse(celestial.Definition));
	}

	public override void Register(IOutputHandler handler)
	{
		// Moon does not need to register for output
	}

	public override ProgVariableTypes Type => ProgVariableTypes.Error;

	public override object DatabaseInsert()
	{
		return null;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		// No-op for moons loaded from XML or DB
	}

	public PerceptionTypes PerceivableTypes => PerceptionTypes.AllVisual;

	public double CurrentDayNumber => (Calendar.CurrentDate - EpochDate).Days + Clock.CurrentTime.TimeFraction +
									  DayNumberAtEpoch;

	public double CurrentCelestialDay =>
		((Calendar.CurrentDate - EpochDate).Days + Clock.CurrentTime.TimeFraction).Modulus(CelestialDaysPerYear);

	public double CelestialDaysPerYear { get; set; }

	public event CelestialUpdateHandler MinuteUpdateEvent;

	public void AddMinutes(int numberOfMinutes)
	{
		// No internal state to track
	}

	public void AddMinutes()
	{
		MinuteUpdateEvent?.Invoke(this);
	}

	private static readonly double OneMinuteTimeFraction = 1.0 / 1440.0;

	protected CelestialMoveDirection CurrentDirection(GeographicCoordinate geography)
	{
		var dn = CurrentDayNumber;
		var current = ElevationAngle(dn, geography);
		var former = ElevationAngle(dn - OneMinuteTimeFraction, geography);
		return current >= former ? CelestialMoveDirection.Ascending : CelestialMoveDirection.Descending;
	}

	private double MeanAnomaly(double dayNumber)
	{
		return (MeanAnomalyAngleAtEpoch + AnomalyChangeAnglePerDay * (dayNumber - DayNumberAtEpoch))
			.Modulus(2 * Math.PI);
	}

	private double TrueAnomaly(double dayNumber)
	{
		var m = MeanAnomaly(dayNumber);
		// Use a simple second order approximation of Keppler's equation
		return (m + 2 * OrbitalEccentricity * Math.Sin(m) + 1.25 * OrbitalEccentricity * OrbitalEccentricity * Math.Sin(2 * m))
			.Modulus(2 * Math.PI);
	}

	private (double RA, double Dec) EquatorialCoordinates(double dayNumber)
	{
		var v = TrueAnomaly(dayNumber);
		var wv = v + ArgumentOfPeriapsis;

		var x = Math.Cos(LongitudeOfAscendingNode) * Math.Cos(wv) -
				Math.Sin(LongitudeOfAscendingNode) * Math.Sin(wv) * Math.Cos(OrbitalInclination);
		var y = Math.Sin(LongitudeOfAscendingNode) * Math.Cos(wv) +
				Math.Cos(LongitudeOfAscendingNode) * Math.Sin(wv) * Math.Cos(OrbitalInclination);
		var z = Math.Sin(wv) * Math.Sin(OrbitalInclination);

		var ra = Math.Atan2(y, x).Modulus(2 * Math.PI);
		var dec = Math.Asin(z);
		return (ra, dec);
	}

	private double SiderealTime(double dayNumber, GeographicCoordinate coordinate)
	{
		return (SiderealTimeAtEpoch + SiderealTimePerDay * (dayNumber - DayNumberAtEpoch) + coordinate.Longitude)
			.Modulus(2 * Math.PI);
	}

	private double HourAngle(double dayNumber, GeographicCoordinate coordinate, double rightAscension)
	{
		return SiderealTime(dayNumber, coordinate) - rightAscension;
	}

	private double ElevationAngle(double dayNumber, GeographicCoordinate geography)
	{
		var (ra, dec) = EquatorialCoordinates(dayNumber);
		var ha = HourAngle(dayNumber, geography, ra);
		return Math.Asin(Math.Sin(geography.Latitude) * Math.Sin(dec) +
						 Math.Cos(geography.Latitude) * Math.Cos(dec) * Math.Cos(ha));
	}

	private double AzimuthAngle(double dayNumber, GeographicCoordinate geography)
	{
		var (ra, dec) = EquatorialCoordinates(dayNumber);
		var ha = HourAngle(dayNumber, geography, ra);
		return Math.Atan2(Math.Sin(ha),
						   Math.Cos(ha) * Math.Sin(geography.Latitude) -
						   Math.Tan(dec) * Math.Cos(geography.Latitude));
	}

	public double CurrentElevationAngle(GeographicCoordinate geography)
	{
		return ElevationAngle(CurrentDayNumber, geography);
	}

	public double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle)
	{
		return AzimuthAngle(CurrentDayNumber, geography);
	}

	public double CurrentIllumination(GeographicCoordinate geography)
	{
		// Simple Lambertian model based on phase angle
		var phaseAngle = PhaseAngle();
		return PeakIllumination * (1 + Math.Cos(phaseAngle)) / 2.0;
	}

	public CelestialInformation CurrentPosition(GeographicCoordinate geography)
	{
		var elevation = CurrentElevationAngle(geography);
		var azimuth = CurrentAzimuthAngle(geography, elevation);
		return new CelestialInformation(this, azimuth, elevation, CurrentDirection(geography));
	}

	public CelestialInformation ReturnNewCelestialInformation(ILocation location,
			CelestialInformation celestialStatus, GeographicCoordinate coordinate)
	{
		var newStatus = CurrentPosition(coordinate);

		if (celestialStatus == null)
		{
			newStatus.Direction = CelestialMoveDirection.Ascending;
			return newStatus;
		}

		newStatus.Direction = celestialStatus.LastAscensionAngle > newStatus.LastAscensionAngle
				? CelestialMoveDirection.Descending
				: CelestialMoveDirection.Ascending;

		if (ShouldEcho(celestialStatus, newStatus))
		{
			EchoTriggerToZone(GetZoneDisplayTrigger(celestialStatus, newStatus), location);
		}

		return newStatus;
	}

	public string Describe(CelestialInformation info)
	{
		return $"{Name} is {CurrentPhase().Describe()}";
	}

	public bool CelestialAngleIsUsedToDetermineTimeOfDay => false;

	public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography)
	{
		return TimeOfDay.Night;
	}

	private double PhaseAngle()
	{
		// Phase cycle measured from reference full moon
		var cycleDay = (CurrentCelestialDay - FullMoonReferenceDay).Modulus(CelestialDaysPerYear);
		return 2 * Math.PI * (cycleDay / CelestialDaysPerYear);
	}

	public MoonPhase CurrentPhase()
	{
		var frac = (CurrentCelestialDay - FullMoonReferenceDay).Modulus(CelestialDaysPerYear) / CelestialDaysPerYear;

		if (frac < 0.0625 || frac >= 0.9375) return MoonPhase.Full;
		if (frac < 0.1875) return MoonPhase.WaxingGibbous;
		if (frac < 0.3125) return MoonPhase.FirstQuarter;
		if (frac < 0.4375) return MoonPhase.WaxingCrescent;
		if (frac < 0.5625) return MoonPhase.New;
		if (frac < 0.6875) return MoonPhase.WaningCrescent;
		if (frac < 0.8125) return MoonPhase.LastQuarter;
		return MoonPhase.WaningGibbous;
	}

	protected CelestialTrigger GetZoneDisplayTrigger(CelestialInformation oldStatus, CelestialInformation newStatus)
	{
		return
				Triggers.First(
						x =>
								x.Threshold.Between(oldStatus.LastAscensionAngle, newStatus.LastAscensionAngle) &&
								x.Direction == newStatus.Direction);
	}

	public bool ShouldEcho(CelestialInformation oldStatus, CelestialInformation newStatus)
	{
		return
				Triggers.Any(
						x =>
								x.Threshold.Between(oldStatus.LastAscensionAngle, newStatus.LastAscensionAngle) &&
								x.Direction == newStatus.Direction);
	}

	public void EchoTriggerToZone(CelestialTrigger trigger, ILocation location)
	{
		var echo = $"{trigger.Echo.Fullstop().SubstituteANSIColour().ProperSentences()}";
		foreach (var ch in location.Characters)
		{
			if (!ch.CanSee(this))
			{
				continue;
			}

			if (ch.Location.OutdoorsType(ch).In(CellOutdoorsType.Outdoors))
			{
				ch.OutputHandler.Send(echo);
				continue;
			}

			ch.OutputHandler.Send($"{"[Outside]".ColourValue()} {echo}");
		}
	}
}

