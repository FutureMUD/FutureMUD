using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Celestial;

public class NewSun : PerceivedItem, ICelestialObject
{
	public override string FrameworkItemType => "Celestial";

	public ICalendar Calendar { get; set; }
	public IClock Clock { get; init; }

	public double MeanAnomalyAngleAtEpoch { get; set; }
	public double AnomalyChangeAnglePerDay { get; set; }
	public double EclipticLongitude { get; set; }
	public double EquatorialObliquity { get; set; }

	public MudDate EpochDate { get; set; }
	public double DayNumberAtEpoch { get; set; }
	public double SiderealTimeAtEpoch { get; set; }
	public double SiderealTimePerDay { get; set; }

	public double KepplerC1Approximant { get; set; }
	public double KepplerC2Approximant { get; set; }
	public double KepplerC3Approximant { get; set; }
	public double KepplerC4Approximant { get; set; }
	public double KepplerC5Approximant { get; set; }
	public double KepplerC6Approximant { get; set; }

		public double PeakIllumination { get; protected set; }
		public double AlphaScatteringConstant { get; protected set; }
		public double BetaScatteringConstant { get; protected set; }
		public double AtmosphericDensityScalingFactor { get; protected set; }
		public double PlanetaryRadius { get; protected set; }
		public override SizeCategory Size => SizeCategory.Titanic;

	public List<CelestialTrigger> Triggers { get; } = new();
	protected readonly CircularRange<string> AzimuthDescriptions = new();
	protected readonly CircularRange<string> ElevationDescriptions = new();

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("Illumination");
		if (element != null)
		{
			PeakIllumination = element.Element("PeakIllumination")?.Value.GetDouble() ?? 0;
			AlphaScatteringConstant = element.Element("AlphaScatteringConstant")?.Value.GetDouble() ?? 0;
			BetaScatteringConstant = element.Element("BetaScatteringConstant")?.Value.GetDouble() ?? 0;
			PlanetaryRadius = element.Element("PlanetaryRadius")?.Value.GetDouble() ?? 0;
			AtmosphericDensityScalingFactor =
				element.Element("AtmosphericDensityScalingFactor")?.Value.GetDouble() ?? 0;
		}

		element = root.Element("Calendar");
		Calendar = Gameworld.Calendars.Get(long.Parse(element.Value));

		element = root.Element("Orbital");
		CelestialDaysPerYear = element.Element("CelestialDaysPerYear")?.Value.GetDouble() ?? 0;
		MeanAnomalyAngleAtEpoch = element.Element("MeanAnomalyAngleAtEpoch")?.Value.GetDouble() ?? 0;
		AnomalyChangeAnglePerDay = element.Element("AnomalyChangeAnglePerDay")?.Value.GetDouble() ?? 0;
		EclipticLongitude = element.Element("EclipticLongitude")?.Value.GetDouble() ?? 0;
		EquatorialObliquity = element.Element("EquatorialObliquity")?.Value.GetDouble() ?? 0;
		DayNumberAtEpoch = element.Element("DayNumberAtEpoch")?.Value.GetDouble() ?? 0;
		SiderealTimeAtEpoch = element.Element("SiderealTimeAtEpoch")?.Value.GetDouble() ?? 0;
		SiderealTimePerDay = element.Element("SiderealTimePerDay")?.Value.GetDouble() ?? 0;
		KepplerC1Approximant = element.Element("KepplerC1Approximant")?.Value.GetDouble() ?? 0;
		KepplerC2Approximant = element.Element("KepplerC2Approximant")?.Value.GetDouble() ?? 0;
		KepplerC3Approximant = element.Element("KepplerC3Approximant")?.Value.GetDouble() ?? 0;
		KepplerC4Approximant = element.Element("KepplerC4Approximant")?.Value.GetDouble() ?? 0;
		KepplerC5Approximant = element.Element("KepplerC5Approximant")?.Value.GetDouble() ?? 0;
		KepplerC6Approximant = element.Element("KepplerC6Approximant")?.Value.GetDouble() ?? 0;
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

		element = root.Element("ElevationDescriptions");
		if (element != null)
		{
			foreach (var sub in element.Elements("Description"))
			{
				ElevationDescriptions.Add(new BoundRange<string>(sub.Value,
					Convert.ToDouble(sub.Attribute("lower").Value), Convert.ToDouble(sub.Attribute("upper").Value)));
			}

			ElevationDescriptions.Sort();
		}

		element = root.Element("AzimuthDescriptions");
		if (element != null)
		{
			foreach (var sub in element.Elements("Description"))
			{
				AzimuthDescriptions.Add(new BoundRange<string>(sub.Value,
					Convert.ToDouble(sub.Attribute("lower").Value), Convert.ToDouble(sub.Attribute("upper").Value)));
			}

			AzimuthDescriptions.Sort();
		}

		element = root.Element("Name");
		if (element != null)
		{
			_name = element.Value;
		}
	}

	public NewSun(XElement root, IClock clock, IFuturemud game)
	{
		Gameworld = game;
		Clock = clock;
		Clock.MinutesUpdated += AddMinutes;
		LoadFromXml(root);
	}

	public NewSun(MudSharp.Models.Celestial celestial, IFuturemud game)
	{
		IdInitialised = true;
		_id = celestial.Id;
		Gameworld = game;
		Clock = Gameworld.Clocks.Get(celestial.FeedClockId);
		Clock.MinutesUpdated += AddMinutes;
		LoadFromXml(XElement.Parse(celestial.Definition));
	}

	public double MeanAnomaly(double dayNumber)
	{
		return (MeanAnomalyAngleAtEpoch + AnomalyChangeAnglePerDay * Math.Truncate(dayNumber - DayNumberAtEpoch))
			.Modulus(2 * Math.PI);
	}

	public double TrueAnomaly(double dayNumber)
	{
		var mean = MeanAnomaly(dayNumber);
		return (mean +
		        KepplerC1Approximant * Math.Sin(mean) +
		        KepplerC2Approximant * Math.Sin(2 * mean) +
		        KepplerC3Approximant * Math.Sin(3 * mean) +
		        KepplerC4Approximant * Math.Sin(4 * mean) +
		        KepplerC5Approximant * Math.Sin(5 * mean) +
		        KepplerC6Approximant * Math.Sin(6 * mean)
			).Modulus(2 * Math.PI);
	}

	public double EclipticLongitudeOfSun(double dayNumber)
	{
		return (TrueAnomaly(dayNumber) + EclipticLongitude + Math.PI).Modulus(2 * Math.PI);
	}

	public double RightAscension(double dayNumber)
	{
		var eclipticLongitude = EclipticLongitudeOfSun(dayNumber);
		return Math.Atan2(Math.Sin(eclipticLongitude) * Math.Cos(EquatorialObliquity), Math.Cos(eclipticLongitude));
	}

	public double Declension(double dayNumber)
	{
		return Math.Asin(Math.Sin(EclipticLongitudeOfSun(dayNumber)) * Math.Sin(EquatorialObliquity));
	}

	public double SiderealTime(double dayNumber, GeographicCoordinate coordinate)
	{
		return (SiderealTimeAtEpoch + SiderealTimePerDay * (dayNumber - DayNumberAtEpoch) + coordinate.Longitude)
			.Modulus(2 * Math.PI);
	}

	public double HourAngle(double dayNumber, GeographicCoordinate coordinate)
	{
		return SiderealTime(dayNumber, coordinate) - RightAscension(dayNumber);
	}

	public double Azimuth(double dayNumber, GeographicCoordinate coordinate)
	{
		var ha = HourAngle(dayNumber, coordinate);
		return Math.Atan2(Math.Sin(ha),
			Math.Cos(ha) * Math.Sin(coordinate.Latitude) -
			Math.Tan(Declension(dayNumber)) * Math.Cos(coordinate.Latitude));
	}

	public double Altitude(double dayNumber, GeographicCoordinate coordinate)
	{
		var decl = Declension(dayNumber);
		return Math.Asin(Math.Sin(coordinate.Latitude) * Math.Sin(decl) + Math.Cos(coordinate.Latitude) *
			Math.Cos(decl) * Math.Cos(HourAngle(dayNumber, coordinate)));
	}

	public override void Register(IOutputHandler handler)
	{
		// Do nothing
	}

	public override ProgVariableTypes Type => ProgVariableTypes.Error;

	public override object DatabaseInsert()
	{
		return null;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		// Do nothing
	}

	public PerceptionTypes PerceivableTypes => PerceptionTypes.AllVisual;

	public void AddMinutes(int numberOfMinutes)
	{
		// Do nothing
	}

	public void AddMinutes()
	{
		MinuteUpdateEvent?.Invoke(this);
	}

	public double CurrentCelestialDay =>
		((Calendar.CurrentDate - EpochDate).Days + Clock.CurrentTime.TimeFraction).Modulus(CelestialDaysPerYear);

	public double CurrentDayNumber => (Calendar.CurrentDate - EpochDate).Days + Clock.CurrentTime.TimeFraction +
	                                  DayNumberAtEpoch + 0.5;

	public double CelestialDaysPerYear { get; set; }

	public event CelestialUpdateHandler MinuteUpdateEvent;

	public double CurrentElevationAngle(GeographicCoordinate geography)
	{
		return Altitude(CurrentDayNumber, geography);
	}

	public double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle)
	{
		return Azimuth(CurrentDayNumber, geography);
	}

	private double LightLevelParameterU(double elevationAngle)
	{
		return Math.Sqrt(PlanetaryRadius / (2 * AtmosphericDensityScalingFactor)) * Math.Sin(elevationAngle);
	}

	private double LightLevelParameterL(double elevationAngle)
	{
		var u = LightLevelParameterU(elevationAngle);
		var erfU = NumberUtilities.Erfc(u);
		var exp = Math.Exp(Math.Pow(u, 2));
		var expTerm = exp * erfU;
		return Math.Sqrt(Math.PI * AtmosphericDensityScalingFactor * PlanetaryRadius * 0.5) *
		       (elevationAngle > 0
			       ? expTerm
			       : 1 + NumberUtilities.Erf(u));
	}

	private double LightLevelParameterH(double elevationAngle)
	{
		return elevationAngle > 0
			? 0
			: PlanetaryRadius * (0.5 - 0.5 * Math.Cos(2 * elevationAngle));
	}

	private double LightLevelParameterRhoH(double elevationAngle)
	{
		return Math.Exp(-1 * LightLevelParameterH(elevationAngle) / AtmosphericDensityScalingFactor);
	}

	private double LightLevelParameterE1(double elevationAngle)
	{
		var L = LightLevelParameterL(elevationAngle);
		var result = elevationAngle > 0 ? PeakIllumination * Math.Exp(-1 * AlphaScatteringConstant * L) : 0;
		return !double.IsNaN(result) ? result : 0.0;
	}

	private double LightLevelParameterE2(double elevationAngle)
	{
		var u = LightLevelParameterU(elevationAngle);
		var L = LightLevelParameterL(elevationAngle);
		var RhoH = LightLevelParameterRhoH(elevationAngle);

		var factor1 = 1 -
		              Math.Exp(-1 * AlphaScatteringConstant * L -
		                       1.75 * (AlphaScatteringConstant - BetaScatteringConstant) *
		                       AtmosphericDensityScalingFactor * RhoH);
		var factor2 = BetaScatteringConstant / AlphaScatteringConstant * RhoH;
		var factor3 = AtmosphericDensityScalingFactor /
		              (L -
		               1.75 * (AlphaScatteringConstant / BetaScatteringConstant - 1) * AtmosphericDensityScalingFactor *
		               RhoH);
		var factor4 = 0.44 *
		              Math.Exp(-1.75 *
		                       ((AlphaScatteringConstant - BetaScatteringConstant) * AtmosphericDensityScalingFactor));

		var result = PeakIllumination * factor1 * factor2 * factor3 * factor4;
		return !double.IsNaN(result) ? result : 0.0;
	}

	public double CurrentIllumination(GeographicCoordinate geography)
	{
		var angle = CurrentElevationAngle(geography);
		if (double.IsNaN(angle))
		{
			return PeakIllumination;
		}

		var E1 = LightLevelParameterE1(angle);
		var E2 = LightLevelParameterE2(angle);
		return E1 + E2;
	}

	private static readonly double OneMinuteTimeFraction = 1.0 / 1440.0;

	protected CelestialMoveDirection CurrentDirection(GeographicCoordinate geography)
	{
		var dn = CurrentDayNumber;
		var current = Altitude(dn, geography);
		var former = Altitude(dn - OneMinuteTimeFraction, geography);
		return current >= former ? CelestialMoveDirection.Ascending : CelestialMoveDirection.Descending;
	}

	public CelestialInformation CurrentPosition(GeographicCoordinate geography)
	{
		var elevationAngle = CurrentElevationAngle(geography);
		var azimuth = CurrentAzimuthAngle(geography, elevationAngle);

		return new CelestialInformation(this, azimuth, elevationAngle, CurrentDirection(geography));
	}

	public CelestialInformation ReturnNewCelestialInformation(ILocation location,
		CelestialInformation celestialStatus, GeographicCoordinate geography)
	{
		var newStatus = CurrentPosition(geography);

		if (celestialStatus == null)
		{
			newStatus.Direction = CelestialMoveDirection.Ascending;
			return newStatus;
		}

		newStatus.Direction = celestialStatus.LastAscensionAngle > newStatus.LastAscensionAngle
			? CelestialMoveDirection.Descending
			: CelestialMoveDirection.Ascending;

		// Process Triggers
		if (ShouldEcho(celestialStatus, newStatus))
		{
			EchoTriggerToZone(GetZoneDisplayTrigger(celestialStatus, newStatus), location);
		}

		return newStatus;
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

	public string Describe(CelestialInformation info)
	{
		return
			$"{Name} {string.Format(ElevationDescriptions.Get(info.LastAscensionAngle), AzimuthDescriptions.Get(info.LastAzimuthAngle))}";
	}

	public bool CelestialAngleIsUsedToDetermineTimeOfDay => true;

	public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography)
	{
		var position = CurrentPosition(geography);
		if (position == null)
		{
			return TimeOfDay.Night;
		}

		if (position.LastAscensionAngle > 0.0)
		{
			if (position.Direction == CelestialMoveDirection.Ascending)
			{
				return TimeOfDay.Morning;
			}

			return TimeOfDay.Afternoon;
		}

		// 12 degrees below horizon is a good general measure of the beginning of dawn or end of dusk
		if (position.LastAscensionAngle < 0.20944)
		{
			return TimeOfDay.Night;
		}

		if (position.Direction == CelestialMoveDirection.Ascending)
		{
			return TimeOfDay.Dawn;
		}

		return TimeOfDay.Dusk;
	}
}