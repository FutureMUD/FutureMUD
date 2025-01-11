using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Celestial;

public class Sun : CelestialObject, IXmlLoadable, IXmlSavable
{
	public override bool IdHasBeenRegistered => true;

	#region Testing Only

	public void SetupEarthSun()
	{
		_name = "sol";
		AltitudeOfSolarDisc = 0.014486;
		CurrentMinutesInCelestialYear = 0;
		DayNumberOfVernalEquinox = 79.92;
		// Confirm this is correct. This is my best guess and it SHOULD be constant as we are using tropical year as solar year
		DayNumberStaticOffsetAxial = -80;
		DayNumberStaticOffsetElliptical = -2;
		MinutesPerDay = 1440;
		MinutesPerYear = 525949;
		MinutesPerYearFraction = 0.02;
		OrbitalDaysPerYear = 365.242374;
		OrbitalEccentricity = 0.016713;
		OrbitalInclination = 23.45 * Math.PI / 180;
		OrbitalRotationPerDay = 6.30063859969952;

		Triggers.Add(new CelestialTrigger(-0.87 * Math.PI / 180, CelestialMoveDirection.Ascending,
			"The edge of the sun rises over the horizon as dawn breaks."));
		Triggers.Add(new CelestialTrigger(-0.87 * Math.PI / 180, CelestialMoveDirection.Descending,
			"The sun says its goodbyes for the day and sets on the horizon"));
		Triggers.Add(new CelestialTrigger(-12.00 * Math.PI / 180, CelestialMoveDirection.Ascending,
			"The first faint traces of light begin to dim the eastern sky as dawn approaches."));
		Triggers.Add(new CelestialTrigger(-12.00 * Math.PI / 180, CelestialMoveDirection.Descending,
			"The last traces of light leave the western sky, and the night begins."));
		Triggers.Add(new CelestialTrigger(-6.00 * Math.PI / 180, CelestialMoveDirection.Ascending,
			"The eastern sky begins to come alive with colour and light as dawn approaches."));
		Triggers.Add(new CelestialTrigger(-6.00 * Math.PI / 180, CelestialMoveDirection.Descending,
			"The glow in the western sky, the last remenants of the day that was, fade away to a dim memory, heralding the evening."));
		Triggers.Add(new CelestialTrigger(3.00 * Math.PI / 180, CelestialMoveDirection.Descending,
			"Shadows lengthen and the western sky turns shades of orange and pink as the sun dips low to the horizon."));
	}

	#endregion

	#region Save

	public override void Save()
	{
		using (new FMDB())
		{
			var celestial = FMDB.Context.Celestials.Find(Id);
			celestial.Minutes = CurrentMinutesInCelestialYear;
			celestial.CelestialYear = CurrentCelestialYear;
			celestial.LastYearBump = YearOfLastFractionBump;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	public override object DatabaseInsert()
	{
		return null;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		// Do nothing
	}

	public override void AddMinutes()
	{
		if (++CurrentMinutesInCelestialYear >= MinutesPerYear)
		{
			CurrentMinutesInCelestialYear -= (int)(Math.Floor((double)MinutesPerDay / MinutesPerDay) * MinutesPerDay);
		}

		UpdateZones();
	}

	#region Configuration Properties

	public double PeakIllumination { get; protected set; }

	public double AlphaScatteringConstant { get; protected set; }

	public double BetaScatteringConstant { get; protected set; }

	public double AtmosphericDensityScalingFactor { get; protected set; }

	public double PlanetaryRadius { get; protected set; }

	protected double _orbitalInclination;

	public double OrbitalInclination
	{
		get => _orbitalInclination;
		protected set => _orbitalInclination = value;
	}

	protected double _orbitalDaysPerYear;

	public double OrbitalDaysPerYear
	{
		get => _orbitalDaysPerYear;
		protected set => _orbitalDaysPerYear = value;
	}

	public override double CelestialDaysPerYear => _orbitalDaysPerYear;

	/// <summary>
	///     Units are Radians
	/// </summary>
	protected double _orbitalRotationPerDay;

	public double OrbitalRotationPerDay
	{
		get => _orbitalRotationPerDay;
		protected set => _orbitalRotationPerDay = value;
	}

	protected double _orbitalEccentricity;

	public double OrbitalEccentricity
	{
		get => _orbitalEccentricity;
		protected set => _orbitalEccentricity = value;
	}

	/// <summary>
	///     Units are Radians
	/// </summary>
	protected double _altitudeOfSolarDisc;

	public double AltitudeOfSolarDisc
	{
		get => _altitudeOfSolarDisc;
		protected set => _altitudeOfSolarDisc = value;
	}

	protected int _minutesPerDay;

	public int MinutesPerDay
	{
		get => _minutesPerDay;
		protected set => _minutesPerDay = value;
	}

	/// <summary>
	///     Technically this isn't fixed, but this approximation is good enough. On Earth this goes through a 21000 year cycle.
	///     It should be accurate to within ~5 minutes
	///     throughout the 21000 year cycle regardless
	/// </summary>
	protected double _dayNumberOfVernalEquinox;

	public double DayNumberOfVernalEquinox
	{
		get => _dayNumberOfVernalEquinox;
		protected set => _dayNumberOfVernalEquinox = value;
	}

	/// <summary>
	///     2 for earth. Effectively a phase offset. Not sure how to calculate it for non-earth situations.
	/// </summary>
	protected double _dayNumberStaticOffsetElliptical;

	public double DayNumberStaticOffsetElliptical
	{
		get => _dayNumberStaticOffsetElliptical;
		protected set => _dayNumberStaticOffsetElliptical = value;
	}

	/// <summary>
	///     80 for earth. Effectively a phase offset. Not sure how to calculate it for non-earth situations.
	/// </summary>
	protected double _dayNumberStaticOffsetAxial;

	public double DayNumberStaticOffsetAxial
	{
		get => _dayNumberStaticOffsetAxial;
		protected set => _dayNumberStaticOffsetAxial = value;
	}

	#endregion

	#region Intermediate Calculation Steps

	/// <summary>
	///     The day number N is a number starting at 1.0 and going through to the end of the year,
	///     that represents the time in days since "midnight" on the "1st" of the first month.
	///     It essentially forms a "Phase Shift" for the largely trig-based functions.
	/// </summary>
	/// <param name="minutes">The number of minutes since the start of the celestial year</param>
	/// <returns>The day number</returns>
	public double CalculateDayNumber(int minutes)
	{
		//return ((double)(minutes + MinutesPerDay) / (double)MinutesPerYear) * OrbitalDaysPerYear;
		return 1.0 + (double)minutes / MinutesPerDay;
	}

	public int CalculateIntegerDayNumber(int minutes)
	{
		return 1 + (int)((double)minutes / MinutesPerDay);
	}

	public override double CurrentCelestialDay => CalculateDayNumber(CurrentMinutesInCelestialYear);

	/// <summary>
	///     Most planets do not orbit their stars in a perfectly circular fashion, but rather an ellipse. In addition,
	///     their orbital motion around the star is not constant. This causes very slight shifts in the "apparent" solar time
	///     over the course of the year, which need to be accounted for.
	///     Formula:
	///     C(elliptical) = (λ - σ) * (MINday / ξ)
	///     C(elliptical) = correction in minutes to the true noon
	///     λ - see function CalculateLambda
	///     σ - see function CalculateSigma
	///     ξ - Orbital Rotation of planet per day in radians
	///     MD - Minutes per day
	/// </summary>
	/// <returns>Correction in minutes to true noon</returns>
	public double CalculateEllipticalCorrection()
	{
		return (CalculateLambda(CalculateDayNumber(CurrentMinutesInCelestialYear)) -
		        CalculateSigma(CalculateDayNumber(CurrentMinutesInCelestialYear))) *
		       (MinutesPerDay / OrbitalRotationPerDay);
	}

	/// <summary>
	///     As the planet is tilted on its axis relative to its plane of orbit, a slight correction to the true noon needs to
	///     be made for the differing orientation
	///     of the planet's surface throughout its yearly orbit.
	///     Formula:
	///     C(axial) = (E - atan(sin(E)cos(Φ)) / cos(E)) * (MINday / ξ)
	///     C(axial) = correction in minutes to the true noon
	///     Φ - axial tilt of the planet relative to orbital plane in radians
	///     E - see function CalculateAxialTiltE
	///     ξ - Orbital Rotation of planet per day in radians
	///     MD - Minutes per day
	/// </summary>
	/// <returns>Correction in minutes to the true noon</returns>
	public double CalculateAxialTiltCorrection()
	{
		var E = CalculateAxialTiltE(CalculateDayNumber(CurrentMinutesInCelestialYear));
		return (E - Math.Atan(Math.Sin(E) * Math.Cos(OrbitalInclination) / Math.Cos(E))) *
		       (MinutesPerDay / OrbitalRotationPerDay);
	}

	public double NormaliseAxialTiltE(double axialTiltE)
	{
		return axialTiltE >= 3 * Math.PI / 2
			? axialTiltE - 2 * Math.PI
			: axialTiltE >= Math.PI / 2
				? axialTiltE - Math.PI
				: axialTiltE;
	}

	public double CalculateAxialTiltE(double dayNumber)
	{
		return NormaliseAxialTiltE(2 * Math.PI / OrbitalDaysPerYear * (dayNumber + DayNumberStaticOffsetAxial));
	}

	public double CalculateNoonError()
	{
		return CalculateEllipticalCorrection() + CalculateAxialTiltCorrection();
	}

	public double CalculateNoonError(double dayNum)
	{
		var E = CalculateAxialTiltE(dayNum);
		return (CalculateLambda(dayNum) - CalculateSigma(dayNum)) * (MinutesPerDay / OrbitalRotationPerDay)
		       +
		       (E - Math.Atan(Math.Sin(E) * Math.Cos(OrbitalInclination) / Math.Cos(E))) *
		       (MinutesPerDay / OrbitalRotationPerDay);
	}

	public double CalculateLocalTimeFraction(int minutes, double longitude)
	{
		return (1 + MinutesInThisDay(minutes) / (double)MinutesPerDay + longitude / (2 * Math.PI)) % 1;
	}

	public double CalculateLocalHourAngle(int minutes, double longitude)
	{
		return -1 * (Math.PI - 2 * Math.PI * CalculateLocalTimeFraction(minutes, longitude));
	}

	public int MinutesInThisDay(int minutes)
	{
		return minutes % MinutesPerDay;
	}

	public double CalculateLocalTimeFraction(double longitude, int minuteOffset = 0)
	{
		return (double)MinutesInThisDay(CurrentMinutesInCelestialYear + minuteOffset) / MinutesPerDay +
		       longitude / (2 * Math.PI) % 1;
	}

	public double CalculateLocalHourAngle(double longitude, int minuteOffset = 0)
	{
		return -1 * (Math.PI - 2 * Math.PI * CalculateLocalTimeFraction(longitude, minuteOffset));
	}

	public double CalculateLambda(double dayNumber)
	{
		return 2 * Math.PI / OrbitalDaysPerYear *
		       (dayNumber + DayNumberStaticOffsetElliptical).Wrap(1.0, 1.0 + Math.Floor(OrbitalDaysPerYear));
	}

	public double CalculateSigma(double dayNumber)
	{
		return CalculateLambda(dayNumber) + 2 * OrbitalEccentricity * Math.Sin(CalculateLambda(dayNumber));
	}

	public double CalculateAnalemmaYAxis(double dayNumber)
	{
		return
			Math.Asin(Math.Sin(CalculateSigma(dayNumber) - CalculateSigma(DayNumberOfVernalEquinox)) *
			          Math.Sin(OrbitalInclination));
	}

	public double CalculateElevationCorrection(double elevation)
	{
		return Math.Sqrt(elevation / 1000).DegreesToRadians();
	}

	#endregion

	#region ICelestialObject Methods

	public override bool CelestialAngleIsUsedToDetermineTimeOfDay => true;

	public override double CurrentElevationAngle(GeographicCoordinate geography)
	{
		/* EA = arcsin ( cos(φ) * cos(δ) * cos(θ) + sin(δ) * sin(θ) ) + Hc
		 * Where:
		 * EA = Elevation Angle
		 * δ = Y Axis of Analemma for Today
		 * φ = Hour Angle of Current Time
		 * θ = Latitude of Area
		 * Hc = Correction for Elevation
		 * */
		return
			Math.Asin(Math.Cos(CalculateLocalHourAngle(geography.Longitude)) *
			          Math.Cos(CalculateAnalemmaYAxis(CalculateDayNumber(CurrentMinutesInCelestialYear))) *
			          Math.Cos(geography.Latitude) +
			          Math.Sin(CalculateAnalemmaYAxis(CalculateDayNumber(CurrentMinutesInCelestialYear))) *
			          Math.Sin(geography.Latitude)) + CalculateElevationCorrection(geography.Elevation);
	}

	protected double CurrentElevationAngle(GeographicCoordinate geography, int minuteOffset)
	{
		/* EA = arcsin ( cos(φ) * cos(δ) * cos(θ) + sin(δ) * sin(θ) ) + Hc
		 * Where:
		 * EA = Elevation Angle
		 * δ = Y Axis of Analemma for Today
		 * φ = Hour Angle of Current Time
		 * θ = Latitude of Area
		 * Hc = Correction for Elevation
		 * */
		return
			Math.Asin(Math.Cos(CalculateLocalHourAngle(geography.Longitude, minuteOffset)) *
			          Math.Cos(CalculateAnalemmaYAxis(
				          CalculateIntegerDayNumber(CurrentMinutesInCelestialYear + minuteOffset))) *
			          Math.Cos(geography.Latitude) +
			          Math.Sin(CalculateAnalemmaYAxis(
				          CalculateIntegerDayNumber(CurrentMinutesInCelestialYear + minuteOffset))) *
			          Math.Sin(geography.Latitude)) + CalculateElevationCorrection(geography.Elevation);
	}

	protected CelestialMoveDirection CurrentDirection(GeographicCoordinate geography)
	{
		var current = CurrentElevationAngle(geography);
		var former = CurrentElevationAngle(geography, -1);
		return current >= former ? CelestialMoveDirection.Ascending : CelestialMoveDirection.Descending;
	}

	public override double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle)
	{
		if (double.IsNaN(Math.Cos(elevationAngle)))
		{
			return 0.0;
		}

		var HourAngle = CalculateLocalHourAngle(geography.Longitude);
		var Kappa =
			Math.Acos((Math.Sin(CalculateAnalemmaYAxis(CalculateDayNumber(CurrentMinutesInCelestialYear))) *
			           Math.Cos(geography.Latitude) -
			           Math.Cos(HourAngle) *
			           Math.Cos(CalculateAnalemmaYAxis(CalculateDayNumber(CurrentMinutesInCelestialYear))) *
			           Math.Sin(geography.Latitude)) / Math.Cos(elevationAngle));
		return HourAngle < 0 ? Math.PI - Kappa : Math.PI + Kappa;
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

	private double CurrentAirmass(double elevationAngle)
	{
		var zenith = Math.PI / 2 - elevationAngle;
		return 1 / (Math.Cos(zenith) + 0.025 * Math.Exp(-11 * Math.Cos(zenith)));
	}

	public override double CurrentIllumination(GeographicCoordinate geography)
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

	public override CelestialInformation CurrentPosition(GeographicCoordinate geography)
	{
		var elevationAngle = CurrentElevationAngle(geography);
		var azimuth = CurrentAzimuthAngle(geography, elevationAngle);

		return new CelestialInformation(this, azimuth, elevationAngle, CurrentDirection(geography));
	}

	public override CelestialInformation ReturnNewCelestialInformation(ILocation location,
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

	#endregion

	#region Constructors

	public Sun(int id, IFuturemud game, Clock clock)
	{
		_id = id;
		Gameworld = game;
		Clock = clock;
	}

	public Sun(MudSharp.Models.Celestial celestial, IFuturemud game)
	{
		_id = celestial.Id;
		Gameworld = game;
		LoadFromXml(XElement.Parse(celestial.Definition));
		Clock = Gameworld.Clocks.Get(celestial.FeedClockId);
		_currentMinutesInCelestialYear = celestial.Minutes;
		_yearOfLastFractionBump = celestial.LastYearBump;
		_currentCelestialYear = celestial.CelestialYear;
	}

	public void LoadFromXml(XElement root)
	{
		var element = root.Element("Configuration");
		if (element != null)
		{
			MinutesPerDay = element.Attribute("MinutesPerDay").Value.GetIntFromOrdinal() ?? 0;
			MinutesPerYear = element.Attribute("MinutesPerYear").Value.GetIntFromOrdinal() ?? 0;
			MinutesPerYearFraction = element.Attribute("MinutesPerYearFraction").Value.GetDouble() ?? 0;
			OrbitalDaysPerYear = element.Attribute("OrbitalDaysPerYear").Value.GetDouble() ?? 0;
			YearsBetweenFractionBumps = element.Attribute("YearsBetweenFractionBumps").Value.GetDouble() ?? 0;
		}

		element = root.Element("Illumination");
		if (element != null)
		{
			PeakIllumination = element.Attribute("PeakIllumination").Value.GetDouble() ?? 0;
			AlphaScatteringConstant = element.Attribute("AlphaScatteringConstant").Value.GetDouble() ?? 0;
			BetaScatteringConstant = element.Attribute("BetaScatteringConstant").Value.GetDouble() ?? 0;
			PlanetaryRadius = element.Attribute("PlanetaryRadius").Value.GetDouble() ?? 0;
			AtmosphericDensityScalingFactor =
				element.Attribute("AtmosphericDensityScalingFactor").Value.GetDouble() ?? 0;
		}

		element = root.Element("Orbital");
		if (element != null)
		{
			OrbitalEccentricity = element.Attribute("OrbitalEccentricity").Value.GetDouble() ?? 0;
			OrbitalInclination = element.Attribute("OrbitalInclination").Value.GetDouble() ?? 0;
			OrbitalRotationPerDay = element.Attribute("OrbitalRotationPerDay").Value.GetDouble() ?? 0;
			AltitudeOfSolarDisc = element.Attribute("AltitudeOfSolarDisc").Value.GetDouble() ?? 0;
			DayNumberOfVernalEquinox = element.Attribute("DayNumberOfVernalEquinox").Value.GetDouble() ?? 0;
			DayNumberStaticOffsetAxial = element.Attribute("DayNumberStaticOffsetAxial").Value.GetDouble() ?? 0;
			DayNumberStaticOffsetElliptical =
				element.Attribute("DayNumberStaticOffsetElliptical").Value.GetDouble() ?? 0;
		}

		element = root.Element("Triggers");
		if (element != null)
		{
			foreach (var sub in element.Elements("Trigger"))
			{
				Triggers.Add(
					new CelestialTrigger(
						sub.Attribute("Angle").Value.GetDouble() ?? 0,
						sub.Attribute("Direction").Value == "Ascending"
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
				ElevationDescriptions.Add(new BoundRange<string>(ElevationDescriptions, sub.Attribute("Text").Value,
					Convert.ToDouble(sub.Attribute("Lower").Value), Convert.ToDouble(sub.Attribute("Upper").Value)));
			}

			ElevationDescriptions.Sort();
		}

		element = root.Element("AzimuthDescriptions");
		if (element != null)
		{
			foreach (var sub in element.Elements("Description"))
			{
				AzimuthDescriptions.Add(new BoundRange<string>(ElevationDescriptions, sub.Attribute("Text").Value,
					Convert.ToDouble(sub.Attribute("Lower").Value), Convert.ToDouble(sub.Attribute("Upper").Value)));
			}

			AzimuthDescriptions.Sort();
		}

		element = root.Element("Name");
		if (element != null)
		{
			_name = element.Value;
		}
	}

	public XElement SaveToXml()
	{
		return new XElement("sun", new XElement("Name", Name.Proper()),
			new XElement("Configuration", new XAttribute("MinutesPerDay", MinutesPerDay),
				new XAttribute("MinutesPerYear", MinutesPerYear),
				new XAttribute("MinutesPerYearFraction", MinutesPerYearFraction),
				new XAttribute("OrbitalDaysPerYear", OrbitalDaysPerYear),
				new XAttribute("YearsBetweenFractionBumps", YearsBetweenFractionBumps)),
			new XElement("Orbital", new XAttribute("OrbitalEccentricity", OrbitalEccentricity),
				new XAttribute("OrbitalInclination", OrbitalInclination),
				new XAttribute("OrbitalRotationPerDay", OrbitalRotationPerDay),
				new XAttribute("AltitudeOfSolarDisc", AltitudeOfSolarDisc),
				new XAttribute("DayNumberOfVernalEquinox", DayNumberOfVernalEquinox),
				new XAttribute("DayNumberStaticOffsetAxial", DayNumberStaticOffsetAxial),
				new XAttribute("DayNumberStaticOffsetElliptical", DayNumberStaticOffsetElliptical)),
			new XElement("Triggers",
				new object[]
				{
					from trigger in Triggers
					select
						new XElement("Trigger", new XAttribute("Angle", trigger.Threshold),
							new XAttribute("Direction", trigger.Direction.Describe()),
							new XAttribute("Echo", trigger.Echo))
				}
			));
	}

	#endregion

	#region Echo Triggers

	protected List<CelestialTrigger> _triggers = new();

	public List<CelestialTrigger> Triggers
	{
		get => _triggers;
		protected set => _triggers = value;
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
		location.Characters.ToList().ForEach(x =>
		{
			if (x.CanSee(this))
			{
				x.OutputHandler.Send("\n" + trigger.Echo);
			}
		});
	}

	#endregion
}