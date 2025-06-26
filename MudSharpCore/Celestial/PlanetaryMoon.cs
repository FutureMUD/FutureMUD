using System;
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

    public ICalendar Calendar { get; init; }
    public IClock Clock { get; init; }

    public double MeanAnomalyAngleAtEpoch { get; init; }
    public double AnomalyChangeAnglePerDay { get; init; }
    public double ArgumentOfPeriapsis { get; init; }
    public double LongitudeOfAscendingNode { get; init; }
    public double OrbitalInclination { get; init; }
    public double OrbitalEccentricity { get; init; }

    public MudDate EpochDate { get; init; }
    public double DayNumberAtEpoch { get; init; }
    public double SiderealTimeAtEpoch { get; init; }
    public double SiderealTimePerDay { get; init; }

    public double PeakIllumination { get; init; }
    public double FullMoonReferenceDay { get; init; }

    public PlanetaryMoon(ICalendar calendar, IClock clock)
    {
        Calendar = calendar;
        Clock = clock;
        Clock.MinutesUpdated += AddMinutes;
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

    public double CelestialDaysPerYear { get; init; }

    public event CelestialUpdateHandler MinuteUpdateEvent;

    public void AddMinutes(int numberOfMinutes)
    {
        // No internal state to track
    }

    public void AddMinutes()
    {
        MinuteUpdateEvent?.Invoke(this);
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

    public double CurrentElevationAngle(GeographicCoordinate geography)
    {
        var day = CurrentDayNumber;
        var (ra, dec) = EquatorialCoordinates(day);
        var ha = HourAngle(day, geography, ra);
        return Math.Asin(Math.Sin(geography.Latitude) * Math.Sin(dec) +
                         Math.Cos(geography.Latitude) * Math.Cos(dec) * Math.Cos(ha));
    }

    public double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle)
    {
        var day = CurrentDayNumber;
        var (ra, dec) = EquatorialCoordinates(day);
        var ha = HourAngle(day, geography, ra);
        return Math.Atan2(Math.Sin(ha), Math.Cos(ha) * Math.Sin(geography.Latitude) -
                                        Math.Tan(dec) * Math.Cos(geography.Latitude));
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
        var direction = CelestialMoveDirection.Ascending; // Simplified
        return new CelestialInformation(this, azimuth, elevation, direction);
    }

    public CelestialInformation ReturnNewCelestialInformation(ILocation location,
        CelestialInformation celestialStatus, GeographicCoordinate coordinate)
    {
        return CurrentPosition(coordinate);
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

        if (frac < 0.1875) return MoonPhase.WaningGibbous;
        if (frac < 0.3125) return MoonPhase.LastQuarter;
        if (frac < 0.4375) return MoonPhase.WaningCrescent;
        if (frac < 0.5625) return MoonPhase.New;
        if (frac < 0.6875) return MoonPhase.WaxingCrescent;
        if (frac < 0.8125) return MoonPhase.FirstQuarter;
        return MoonPhase.WaxingGibbous;
    }
}

