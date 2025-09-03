using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Celestial;

/// <summary>
///     Represents the sun as viewed from the surface of a moon orbiting a planet.
///     Relies on an existing <see cref="PlanetaryMoon"/> for sidereal timing and
///     a <see cref="NewSun"/> for orbital position.
/// </summary>
public class SunFromPlanetaryMoon : PerceivedItem, ICelestialObject
{
    public override string FrameworkItemType => "Celestial";

    public PlanetaryMoon Moon { get; set; }
    public NewSun Sun { get; set; }

    public double PeakIllumination { get; set; }
    public double AlphaScatteringConstant { get; set; }
    public double BetaScatteringConstant { get; set; }
    public double AtmosphericDensityScalingFactor { get; set; }
    public double PlanetaryRadius { get; set; }

    public List<CelestialTrigger> Triggers { get; } = new();
    protected readonly CircularRange<string> AzimuthDescriptions = new();
    protected readonly CircularRange<string> ElevationDescriptions = new();

    public SunFromPlanetaryMoon(PlanetaryMoon moon, NewSun sun)
    {
        Moon = moon;
        Sun = sun;
        PeakIllumination = sun.PeakIllumination;
        AlphaScatteringConstant = sun.AlphaScatteringConstant;
        BetaScatteringConstant = sun.BetaScatteringConstant;
        AtmosphericDensityScalingFactor = sun.AtmosphericDensityScalingFactor;
        PlanetaryRadius = sun.PlanetaryRadius;
    }

    public SunFromPlanetaryMoon(XElement root, IFuturemud game)
    {
        Gameworld = game;
        var moonId = long.Parse(root.Element("Moon")!.Value);
        Moon = (PlanetaryMoon)game.CelestialObjects.Get(moonId)!;
        var sunId = long.Parse(root.Element("Sun")!.Value);
        Sun = (NewSun)game.CelestialObjects.Get(sunId)!;
        var illum = root.Element("Illumination");
        if (illum != null)
        {
            PeakIllumination = illum.Element("PeakIllumination")?.Value.GetDouble() ?? 0.0;
            AlphaScatteringConstant = illum.Element("AlphaScatteringConstant")?.Value.GetDouble() ?? 0.0;
            BetaScatteringConstant = illum.Element("BetaScatteringConstant")?.Value.GetDouble() ?? 0.0;
            AtmosphericDensityScalingFactor = illum.Element("AtmosphericDensityScalingFactor")?.Value.GetDouble() ?? 0.0;
            PlanetaryRadius = illum.Element("PlanetaryRadius")?.Value.GetDouble() ?? 0.0;
        }
        else
        {
            PeakIllumination = Sun.PeakIllumination;
            AlphaScatteringConstant = Sun.AlphaScatteringConstant;
            BetaScatteringConstant = Sun.BetaScatteringConstant;
            AtmosphericDensityScalingFactor = Sun.AtmosphericDensityScalingFactor;
            PlanetaryRadius = Sun.PlanetaryRadius;
        }
        _name = root.Element("Name")?.Value ?? "Sun";
    }

    public SunFromPlanetaryMoon(MudSharp.Models.Celestial celestial, IFuturemud game)
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

    public double CurrentDayNumber => Sun.CurrentDayNumber;
    public double CurrentCelestialDay => Sun.CurrentCelestialDay;
    public double CelestialDaysPerYear => Sun.CelestialDaysPerYear;

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

    private (double RA, double Dec) SunEquatorialCoordinates(double dayNumber)
    {
        var (moonRa, moonDec) = Moon.EquatorialCoordinates(dayNumber);
        var sunRa = Sun.RightAscension(dayNumber);
        var sunDec = Sun.Declension(dayNumber);

        var sunX = Math.Cos(sunRa) * Math.Cos(sunDec);
        var sunY = Math.Sin(sunRa) * Math.Cos(sunDec);
        var sunZ = Math.Sin(sunDec);

        var moonX = Math.Cos(moonRa) * Math.Cos(moonDec);
        var moonY = Math.Sin(moonRa) * Math.Cos(moonDec);
        var moonZ = Math.Sin(moonDec);

        var x = sunX - moonX;
        var y = sunY - moonY;
        var z = sunZ - moonZ;
        var r = Math.Sqrt(x * x + y * y + z * z);
        var ra = Math.Atan2(y, x).Modulus(2 * Math.PI);
        var dec = Math.Asin(z / r);
        return (ra, dec);
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
        var (ra, dec) = SunEquatorialCoordinates(dayNumber);
        var ha = HourAngle(dayNumber, geography, ra);
        return Math.Asin(Math.Sin(geography.Latitude) * Math.Sin(dec) +
                         Math.Cos(geography.Latitude) * Math.Cos(dec) * Math.Cos(ha));
    }

    private double AzimuthAngle(double dayNumber, GeographicCoordinate geography)
    {
        var (ra, dec) = SunEquatorialCoordinates(dayNumber);
        var ha = HourAngle(dayNumber, geography, ra);
        return Math.Atan2(Math.Sin(ha),
                          Math.Cos(ha) * Math.Sin(geography.Latitude) -
                          Math.Tan(dec) * Math.Cos(geography.Latitude));
    }

    public double CurrentElevationAngle(GeographicCoordinate geography) => ElevationAngle(CurrentDayNumber, geography);
    public double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle) => AzimuthAngle(CurrentDayNumber, geography);

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
                       1.75 * (AlphaScatteringConstant / BetaScatteringConstant - 1) * AtmosphericDensityScalingFactor * RhoH);
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

    public CelestialInformation CurrentPosition(GeographicCoordinate geography)
    {
        var elevationAngle = CurrentElevationAngle(geography);
        var azimuth = CurrentAzimuthAngle(geography, elevationAngle);
        return new CelestialInformation(this, azimuth, elevationAngle, CurrentDirection(geography));
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

    public string Describe(CelestialInformation info) => $"{Name} is visible in the sky.";

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
            return position.Direction == CelestialMoveDirection.Ascending ? TimeOfDay.Morning : TimeOfDay.Afternoon;
        }

        if (position.LastAscensionAngle < 0.20944)
        {
            return TimeOfDay.Night;
        }

        return position.Direction == CelestialMoveDirection.Ascending ? TimeOfDay.Dawn : TimeOfDay.Dusk;
    }
}

