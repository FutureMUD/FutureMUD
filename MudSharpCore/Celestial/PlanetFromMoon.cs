using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Time;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Celestial;

/// <summary>
/// Represents a parent planet as viewed from the surface of its moon.
/// Relies on an existing <see cref="PlanetaryMoon"/> for orbital data and
/// a sun for illumination and eclipse checks.
/// </summary>
public class PlanetFromMoon : PerceivedItem, ICelestialObject
{
    public override string FrameworkItemType => "Celestial";

    public PlanetaryMoon Moon { get; set; }
    public ICelestialObject Sun { get; set; }

    public double PeakIllumination { get; set; }
    public double AngularRadius { get; set; }

    public List<CelestialTrigger> Triggers { get; } = new();
    protected readonly CircularRange<string> AzimuthDescriptions = new();
    protected readonly CircularRange<string> ElevationDescriptions = new();

    public PlanetFromMoon(PlanetaryMoon moon, ICelestialObject sun)
    {
        Moon = moon;
        Sun = sun;
    }

    public PlanetFromMoon(XElement root, IFuturemud game)
    {
        Gameworld = game;
        var moonId = long.Parse(root.Element("Moon")!.Value);
        Moon = (PlanetaryMoon)game.CelestialObjects.Get(moonId)!;
        var sunId = long.Parse(root.Element("Sun")!.Value);
        Sun = game.CelestialObjects.Get(sunId);
        PeakIllumination = root.Element("PeakIllumination")?.Value.GetDouble() ?? 0.0;
        AngularRadius = root.Element("AngularRadius")?.Value.GetDouble() ?? 0.0;
        _name = root.Element("Name")?.Value ?? "Planet";
    }

    public PlanetFromMoon(MudSharp.Models.Celestial celestial, IFuturemud game)
        : this(XElement.Parse(celestial.Definition), game)
    {
        IdInitialised = true;
        _id = celestial.Id;
    }

    public override void Register(IOutputHandler handler) { }
    public override ProgVariableTypes Type => ProgVariableTypes.Error;
    public override object DatabaseInsert() => null;
    public override void SetIDFromDatabase(object dbitem) { }

    public PerceptionTypes PerceivableTypes => PerceptionTypes.AllVisual;

    public double CurrentDayNumber => Moon.CurrentDayNumber;
    public double CurrentCelestialDay => Moon.CurrentCelestialDay;
    public double CelestialDaysPerYear => Moon.CelestialDaysPerYear;

    public event CelestialUpdateHandler? MinuteUpdateEvent;
    public void AddMinutes(int numberOfMinutes) { }
    public void AddMinutes() { MinuteUpdateEvent?.Invoke(this); }

    private static readonly double OneMinuteTimeFraction = 1.0 / 1440.0;

    protected CelestialMoveDirection CurrentDirection(GeographicCoordinate geography)
    {
        var dn = CurrentDayNumber;
        var current = ElevationAngle(dn, geography);
        var former = ElevationAngle(dn - OneMinuteTimeFraction, geography);
        return current >= former ? CelestialMoveDirection.Ascending : CelestialMoveDirection.Descending;
    }

    private (double RA, double Dec) PlanetEquatorialCoordinates(double dayNumber)
    {
        var (moonRa, moonDec) = Moon.EquatorialCoordinates(dayNumber);
        return ((moonRa + Math.PI).Modulus(2 * Math.PI), -moonDec);
    }

    private double SiderealTime(double dayNumber, GeographicCoordinate coordinate)
    {
        return (Moon.SiderealTimeAtEpoch + Moon.SiderealTimePerDay * (dayNumber - Moon.DayNumberAtEpoch) + coordinate.Longitude)
            .Modulus(2 * Math.PI);
    }

    private double HourAngle(double dayNumber, GeographicCoordinate coordinate, double ra)
    {
        return SiderealTime(dayNumber, coordinate) - ra;
    }

    private double ElevationAngle(double dayNumber, GeographicCoordinate geography)
    {
        var (ra, dec) = PlanetEquatorialCoordinates(dayNumber);
        var ha = HourAngle(dayNumber, geography, ra);
        return Math.Asin(Math.Sin(geography.Latitude) * Math.Sin(dec) +
                         Math.Cos(geography.Latitude) * Math.Cos(dec) * Math.Cos(ha));
    }

    private double AzimuthAngle(double dayNumber, GeographicCoordinate geography)
    {
        var (ra, dec) = PlanetEquatorialCoordinates(dayNumber);
        var ha = HourAngle(dayNumber, geography, ra);
        return Math.Atan2(Math.Sin(ha),
                          Math.Cos(ha) * Math.Sin(geography.Latitude) -
                          Math.Tan(dec) * Math.Cos(geography.Latitude));
    }

    public (double RA, double Dec) EquatorialCoordinates(double dayNumber) => PlanetEquatorialCoordinates(dayNumber);

    public double CurrentElevationAngle(GeographicCoordinate geography) => ElevationAngle(CurrentDayNumber, geography);
    public double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle) => AzimuthAngle(CurrentDayNumber, geography);

    private double PlanetPhaseAngle()
    {
        var cycleDay = (Moon.CurrentCelestialDay - Moon.FullMoonReferenceDay).Modulus(Moon.CelestialDaysPerYear);
        var moonPhase = 2 * Math.PI * (cycleDay / Moon.CelestialDaysPerYear);
        return (moonPhase + Math.PI).Modulus(2 * Math.PI);
    }

    public double CurrentIllumination(GeographicCoordinate geography)
    {
        var phase = PlanetPhaseAngle();
        return PeakIllumination * (1 + Math.Cos(phase)) / 2.0;
    }

    public CelestialInformation CurrentPosition(GeographicCoordinate geography)
    {
        var elevation = CurrentElevationAngle(geography);
        var azimuth = CurrentAzimuthAngle(geography, elevation);
        return new CelestialInformation(this, azimuth, elevation, CurrentDirection(geography));
    }

    public CelestialInformation ReturnNewCelestialInformation(ILocation location, CelestialInformation celestialStatus, GeographicCoordinate coordinate)
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

    public string Describe(CelestialInformation info) => $"{Name} is {CurrentPhase().Describe()}";

    public bool CelestialAngleIsUsedToDetermineTimeOfDay => false;
    public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography) => Sun?.CurrentTimeOfDay(geography) ?? TimeOfDay.Night;

    public MoonPhase CurrentPhase()
    {
        return Moon.CurrentPhase() switch
        {
            MoonPhase.New => MoonPhase.Full,
            MoonPhase.WaxingCrescent => MoonPhase.WaningGibbous,
            MoonPhase.FirstQuarter => MoonPhase.LastQuarter,
            MoonPhase.WaxingGibbous => MoonPhase.WaningCrescent,
            MoonPhase.Full => MoonPhase.New,
            MoonPhase.WaningGibbous => MoonPhase.WaxingCrescent,
            MoonPhase.LastQuarter => MoonPhase.FirstQuarter,
            MoonPhase.WaningCrescent => MoonPhase.WaxingGibbous,
            _ => MoonPhase.New
        };
    }

    public bool IsSunEclipsed(GeographicCoordinate geography)
    {
        if (Sun == null) return false;
        var planet = CurrentPosition(geography);
        var star = Sun.CurrentPosition(geography);
        var separation = AngularSeparation(planet, star);
        return separation < AngularRadius;
    }

    private static double AngularSeparation(CelestialInformation a, CelestialInformation b)
    {
        return Math.Acos(
            Math.Sin(a.LastAscensionAngle) * Math.Sin(b.LastAscensionAngle) +
            Math.Cos(a.LastAscensionAngle) * Math.Cos(b.LastAscensionAngle) * Math.Cos(a.LastAzimuthAngle - b.LastAzimuthAngle));
    }

    protected CelestialTrigger GetZoneDisplayTrigger(CelestialInformation oldStatus, CelestialInformation newStatus)
    {
        return Triggers.First(x => x.Threshold.Between(oldStatus.LastAscensionAngle, newStatus.LastAscensionAngle) &&
                                   x.Direction == newStatus.Direction);
    }

    public bool ShouldEcho(CelestialInformation oldStatus, CelestialInformation newStatus)
    {
        return Triggers.Any(x => x.Threshold.Between(oldStatus.LastAscensionAngle, newStatus.LastAscensionAngle) &&
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
