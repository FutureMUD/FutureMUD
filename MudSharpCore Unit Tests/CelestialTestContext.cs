#nullable enable

using Moq;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using RuntimeCalendar = MudSharp.TimeAndDate.Date.Calendar;
using RuntimeClock = MudSharp.TimeAndDate.Time.Clock;

namespace MudSharp_Unit_Tests;

internal sealed class CelestialTestContext
{
    public IFuturemud Gameworld { get; }
    public RuntimeCalendar Calendar { get; }
    public RuntimeClock Clock { get; }
    public NewSun Sun { get; }
    public PlanetaryMoon Moon { get; }
    public PlanetFromMoon Planet { get; }
    public SunFromPlanetaryMoon SunFromMoon { get; }
    public GeographicCoordinate ZeroGeography { get; } = new(0.0, 0.0, 0.0, 0.0);
    public GeographicCoordinate DefaultGeography { get; } = new(0.3, 0.1, 0.0, 0.0);

    public CelestialTestContext(
        IFuturemud gameworld,
        RuntimeCalendar calendar,
        RuntimeClock clock,
        NewSun sun,
        PlanetaryMoon moon,
        PlanetFromMoon planet,
        SunFromPlanetaryMoon sunFromMoon)
    {
        Gameworld = gameworld;
        Calendar = calendar;
        Clock = clock;
        Sun = sun;
        Moon = moon;
        Planet = planet;
        SunFromMoon = sunFromMoon;
    }

    public void SetDateTime(string date, int hour, int minute, int second)
    {
        Calendar.SetDate(date);
        Clock.CurrentTime.SetTime(hour, minute, second);
    }

    public NewSun CreateSunWithDayOffset(double currentDayNumberOffset)
    {
        return new NewSun(
            XElement.Parse(CelestialTestXml.EarthSunXml(currentDayNumberOffset)),
            Clock,
            Gameworld
        );
    }
}

internal static class CelestialTestFactory
{
    public static CelestialTestContext CreateEarthSystem()
    {
        Mock<ISaveManager> saveManager = new();
        saveManager.Setup(x => x.Add(It.IsAny<ISaveable>()));

        All<IClock> clocks = new();
        All<ICalendar> calendars = new();
        All<ICelestialObject> celestials = new();

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.Clocks).Returns(clocks);
        gameworld.SetupGet(x => x.Calendars).Returns(calendars);
        gameworld.SetupGet(x => x.CelestialObjects).Returns(celestials);
        gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);

        RuntimeClock clock = new(
            XElement.Parse(
                @"<Clock>
	<Alias>UTC</Alias>
	<Description>Universal Time Clock</Description>
	<ShortDisplayString>$j:$m:$s $i</ShortDisplayString>
	<SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>
	<LongDisplayString>$c $i</LongDisplayString>
	<SecondsPerMinute>60</SecondsPerMinute>
	<MinutesPerHour>60</MinutesPerHour>
	<HoursPerDay>24</HoursPerDay>
	<InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>
	<SecondFixedDigits>2</SecondFixedDigits>
	<MinuteFixedDigits>2</MinuteFixedDigits>
	<HourFixedDigits>0</HourFixedDigits>
	<NoZeroHour>true</NoZeroHour>
	<NumberOfHourIntervals>2</NumberOfHourIntervals>
	<HourIntervalNames>
		<HourIntervalName>a.m</HourIntervalName>
		<HourIntervalName>p.m</HourIntervalName>
	</HourIntervalNames>
	<HourIntervalLongNames>
		<HourIntervalLongName>in the morning</HourIntervalLongName>
		<HourIntervalLongName>in the afternoon</HourIntervalLongName>
	</HourIntervalLongNames>
	<CrudeTimeIntervals>
		<CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4"" />
		<CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12"" />
		<CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18"" />
		<CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22"" />
	</CrudeTimeIntervals>
</Clock>"),
            gameworld.Object,
            new MudTimeZone(1, 0, 0, "UTC+0", "utc"),
            12,
            0,
            0)
        {
            Id = 1
        };
        clocks.Add(clock);

        RuntimeCalendar calendar = new(
            XElement.Parse(
                @"<calendar>
	<alias>gregorian</alias>
	<shortname>Gregorian</shortname>
	<fullname>Gregorian Calendar</fullname>
	<description>Test</description>
	<shortstring>$dd/$mo/$yy</shortstring>
	<longstring>$dd $mo $yy</longstring>
	<wordystring>$dd $mo $yy</wordystring>
	<plane>earth</plane>
	<feedclock>0</feedclock>
	<epochyear>2010</epochyear>
	<weekdayatepoch>4</weekdayatepoch>
	<ancienterashortstring>BC</ancienterashortstring>
	<ancienteralongstring>before Christ</ancienteralongstring>
	<modernerashortstring>AD</modernerashortstring>
	<moderneralongstring>year of our Lord</moderneralongstring>
	<weekdays>
		<weekday>Monday</weekday>
		<weekday>Tuesday</weekday>
		<weekday>Wednesday</weekday>
		<weekday>Thursday</weekday>
		<weekday>Friday</weekday>
		<weekday>Saturday</weekday>
		<weekday>Sunday</weekday>
	</weekdays>
	<months>
		<month><alias>january</alias><shortname>jan</shortname><fullname>January</fullname><nominalorder>1</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>february</alias><shortname>feb</shortname><fullname>February</fullname><nominalorder>2</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>march</alias><shortname>mar</shortname><fullname>March</fullname><nominalorder>3</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>april</alias><shortname>apr</shortname><fullname>April</fullname><nominalorder>4</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>may</alias><shortname>may</shortname><fullname>May</fullname><nominalorder>5</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>june</alias><shortname>jun</shortname><fullname>June</fullname><nominalorder>6</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>july</alias><shortname>jul</shortname><fullname>July</fullname><nominalorder>7</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>august</alias><shortname>aug</shortname><fullname>August</fullname><nominalorder>8</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>september</alias><shortname>sep</shortname><fullname>September</fullname><nominalorder>9</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>october</alias><shortname>oct</shortname><fullname>October</fullname><nominalorder>10</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>november</alias><shortname>nov</shortname><fullname>November</fullname><nominalorder>11</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>december</alias><shortname>dec</shortname><fullname>December</fullname><nominalorder>12</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
	</months>
	<intercalarymonths />
</calendar>"),
            gameworld.Object)
        {
            Id = 1
        };
        calendar.FeedClock = clock;
        calendar.SetDate("1/jan/2000");
        calendars.Add(calendar);

        NewSun sun = new(XElement.Parse(CelestialTestXml.EarthSunXml()), clock, gameworld.Object);
        celestials.Add(sun);

        PlanetaryMoon moon = new(calendar, clock)
        {
            CelestialDaysPerYear = 29.530588,
            MeanAnomalyAngleAtEpoch = 2.355556,
            AnomalyChangeAnglePerDay = 0.228027,
            ArgumentOfPeriapsis = 5.552765,
            LongitudeOfAscendingNode = 2.18244,
            OrbitalInclination = 0.0898,
            OrbitalEccentricity = 0.0549,
            OrbitalSemiMajorAxis = 384400.0,
            DayNumberAtEpoch = 2451545.0,
            SiderealTimeAtEpoch = 4.889488,
            SiderealTimePerDay = 6.300388,
            EpochDate = calendar.GetDate("21/jan/2000"),
            PeakIllumination = 1.0,
            FullMoonReferenceDay = 0.0
        };
        celestials.Add(moon);

        PlanetFromMoon planet = new(moon, sun)
        {
            PeakIllumination = 1.0,
            AngularRadius = 0.0165,
            SunAngularRadius = 0.004654793
        };
        celestials.Add(planet);

        SunFromPlanetaryMoon sunFromMoon = new(moon, sun);
        celestials.Add(sunFromMoon);

        return new CelestialTestContext(gameworld.Object, calendar, clock, sun, moon, planet, sunFromMoon);
    }
}

internal static class CelestialTestXml
{
    public static string EarthSunXml(double currentDayNumberOffset = 0.5)
    {
        return $@"<Sun>
	<Name>The Sun</Name>
	<Calendar>1</Calendar>
	<Orbital>
		<CelestialDaysPerYear>365.24</CelestialDaysPerYear>
		<MeanAnomalyAngleAtEpoch>6.24006</MeanAnomalyAngleAtEpoch>
		<AnomalyChangeAnglePerDay>0.017202</AnomalyChangeAnglePerDay>
		<EclipticLongitude>1.796595</EclipticLongitude>
		<EquatorialObliquity>0.409093</EquatorialObliquity>
		<OrbitalEccentricity>0.016713</OrbitalEccentricity>
		<OrbitalSemiMajorAxis>149597870.7</OrbitalSemiMajorAxis>
		<ApparentAngularRadius>0.004654793</ApparentAngularRadius>
		<DayNumberAtEpoch>2451545</DayNumberAtEpoch>
		<CurrentDayNumberOffset>{currentDayNumberOffset:R}</CurrentDayNumberOffset>
		<SiderealTimeAtEpoch>4.889488</SiderealTimeAtEpoch>
		<SiderealTimePerDay>6.300388</SiderealTimePerDay>
		<KepplerC1Approximant>0.033419565</KepplerC1Approximant>
		<KepplerC2Approximant>0.000349066</KepplerC2Approximant>
		<KepplerC3Approximant>0.000005235988</KepplerC3Approximant>
		<KepplerC4Approximant>0</KepplerC4Approximant>
		<KepplerC5Approximant>0</KepplerC5Approximant>
		<KepplerC6Approximant>0</KepplerC6Approximant>
		<EpochDate>1-jan-2000</EpochDate>
	</Orbital>
	<Illumination>
		<PeakIllumination>98000</PeakIllumination>
		<AlphaScatteringConstant>0.05</AlphaScatteringConstant>
		<BetaScatteringConstant>0.035</BetaScatteringConstant>
		<PlanetaryRadius>6378</PlanetaryRadius>
		<AtmosphericDensityScalingFactor>6.35</AtmosphericDensityScalingFactor>
	</Illumination>
</Sun>";
    }
}

internal static class CelestialTestAssertions
{
    public const double AngleTolerance = 1.0E-6;
    public const double DayNumberTolerance = 1.0E-6;
    public const double UnitTolerance = 1.0E-6;
    public const double LuxTolerance = 1.0E-3;

    private static readonly MethodInfo SunFromMoonEquatorialCoordinatesMethod =
        typeof(SunFromPlanetaryMoon).GetMethod(
            "SunEquatorialCoordinates",
            BindingFlags.Instance | BindingFlags.NonPublic)!;

    public static (double RA, double Dec) SunFromMoonEquatorialCoordinates(
        SunFromPlanetaryMoon sunFromMoon,
        double dayNumber)
    {
        return ((double RA, double Dec))SunFromMoonEquatorialCoordinatesMethod.Invoke(sunFromMoon, [dayNumber])!;
    }

    public static void SetClockDimensions(RuntimeClock clock, int hoursPerDay, int minutesPerHour)
    {
        SetClockField(clock, "_hoursPerDay", hoursPerDay);
        SetClockField(clock, "_minutesPerHour", minutesPerHour);
    }

    public static double WrappedAbsoluteDifference(double first, double second)
    {
        double difference = (first - second).Modulus(2 * Math.PI);
        if (difference > Math.PI)
        {
            difference = 2 * Math.PI - difference;
        }

        return Math.Abs(difference);
    }

    private static void SetClockField(RuntimeClock clock, string fieldName, int value)
    {
        typeof(RuntimeClock)
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(clock, value);
    }
}
