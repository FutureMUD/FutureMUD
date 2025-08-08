using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Construction;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.Models;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PlanetFromMoonTests
{
    private static PlanetaryMoon _moon;
    private static NewSun _sun;
    private static PlanetFromMoon _planet;
    private static IFuturemud _gameworld;
    private static ICalendar _calendar;
    private static IClock _clock;

    [ClassInitialize]
    public static void Init(TestContext context)
    {
        var saveManager = new Mock<ISaveManager>();
        saveManager.Setup(x => x.Add(It.IsAny<ISaveable>()));

        var clocks = new All<IClock>();
        var calendars = new All<ICalendar>();
        var celestials = new All<ICelestialObject>();

        var mock = new Mock<IFuturemud>();
        mock.SetupGet(x => x.Clocks).Returns(clocks);
        mock.SetupGet(x => x.Calendars).Returns(calendars);
        mock.SetupGet(x => x.CelestialObjects).Returns(celestials);
        mock.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
        _gameworld = mock.Object;

        _clock = new MudSharp.TimeAndDate.Time.Clock(XElement.Parse(@"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4""/>    <CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12""/>    <CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18""/>    <CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22""/>  </CrudeTimeIntervals></Clock>"),
                        _gameworld,
                        new MudTimeZone(1,0,0,"UTC+0","utc"),
                        12,
                        0,
                        0) { Id = 1 };
        clocks.Add(_clock);

        _calendar = new MudSharp.TimeAndDate.Date.Calendar(XElement.Parse(@"<calendar><alias>gregorian</alias><shortname>Gregorian</shortname><fullname>Gregorian Calendar</fullname><description>Test</description><shortstring>$dd/$mo/$yy</shortstring><longstring>$dd $mo $yy</longstring><wordystring>$dd $mo $yy</wordystring><plane>earth</plane><feedclock>0</feedclock><epochyear>2010</epochyear><weekdayatepoch>4</weekdayatepoch><ancienterashortstring>BC</ancienterashortstring><ancienteralongstring>before Christ</ancienteralongstring><modernerashortstring>AD</modernerashortstring><moderneralongstring>year of our Lord</moderneralongstring><weekdays><weekday>Monday</weekday><weekday>Tuesday</weekday><weekday>Wednesday</weekday><weekday>Thursday</weekday><weekday>Friday</weekday><weekday>Saturday</weekday><weekday>Sunday</weekday></weekdays><months><month><alias>january</alias><shortname>jan</shortname><fullname>January</fullname><nominalorder>1</nominalorder><normaldays>31</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>february</alias><shortname>feb</shortname><fullname>February</fullname><nominalorder>2</nominalorder><normaldays>28</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>march</alias><shortname>mar</shortname><fullname>March</fullname><nominalorder>3</nominalorder><normaldays>31</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>april</alias><shortname>apr</shortname><fullname>April</fullname><nominalorder>4</nominalorder><normaldays>30</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>may</alias><shortname>may</shortname><fullname>May</fullname><nominalorder>5</nominalorder><normaldays>31</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>june</alias><shortname>jun</shortname><fullname>June</fullname><nominalorder>6</nominalorder><normaldays>30</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>july</alias><shortname>jul</shortname><fullname>July</fullname><nominalorder>7</nominalorder><normaldays>31</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>august</alias><shortname>aug</shortname><fullname>August</fullname><nominalorder>8</nominalorder><normaldays>31</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>september</alias><shortname>sep</shortname><fullname>September</fullname><nominalorder>9</nominalorder><normaldays>30</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>october</alias><shortname>oct</shortname><fullname>October</fullname><nominalorder>10</nominalorder><normaldays>31</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>november</alias><shortname>nov</shortname><fullname>November</fullname><nominalorder>11</nominalorder><normaldays>30</normaldays><intercalarydays/><specialdays/><nonweekdays/></month><month><alias>december</alias><shortname>dec</shortname><fullname>December</fullname><nominalorder>12</nominalorder><normaldays>31</normaldays><intercalarydays/><specialdays/><nonweekdays/></month></months><intercalarymonths/></calendar>"), _gameworld) { Id = 1 };
        _calendar.FeedClock = _clock;
        calendars.Add(_calendar);
        _calendar.SetDate("1/jan/2000");

        _sun = new NewSun(new Celestial
        {
            Id = 1,
            CelestialYear = 0,
            LastYearBump = 0,
            FeedClockId = 1,
            Minutes = 0,
            Seasons = new List<Season>(),
            WeatherControllers = new List<WeatherController>(),
            Definition = @"<Sun><Name>The Sun</Name><Calendar>1</Calendar><Orbital><CelestialDaysPerYear>365.24</CelestialDaysPerYear><MeanAnomalyAngleAtEpoch>6.24006</MeanAnomalyAngleAtEpoch><AnomalyChangeAnglePerDay>0.017202</AnomalyChangeAnglePerDay><EclipticLongitude>1.796595</EclipticLongitude><EquatorialObliquity>0.409093</EquatorialObliquity><DayNumberAtEpoch>2451545</DayNumberAtEpoch><SiderealTimeAtEpoch>4.889488</SiderealTimeAtEpoch><SiderealTimePerDay>6.300388</SiderealTimePerDay><KepplerC1Approximant>0.033419565</KepplerC1Approximant><KepplerC2Approximant>0.000349066</KepplerC2Approximant><KepplerC3Approximant>0.000005235988</KepplerC3Approximant><KepplerC4Approximant>0</KepplerC4Approximant><KepplerC5Approximant>0</KepplerC5Approximant><KepplerC6Approximant>0</KepplerC6Approximant><EpochDate>25-mar-31</EpochDate></Orbital><Illumination><PeakIllumination>98000</PeakIllumination><AlphaScatteringConstant>0.05</AlphaScatteringConstant><BetaScatteringConstant>0.035</BetaScatteringConstant><PlanetaryRadius>6378</PlanetaryRadius><AtmosphericDensityScalingFactor>6.35</AtmosphericDensityScalingFactor></Illumination></Sun>"
        }, _gameworld);
        celestials.Add(_sun);

        _moon = new PlanetaryMoon(_calendar, _clock)
        {
            CelestialDaysPerYear = 29.530588,
            MeanAnomalyAngleAtEpoch = 2.355556,
            AnomalyChangeAnglePerDay = 0.228027,
            ArgumentOfPeriapsis = 5.552765,
            LongitudeOfAscendingNode = 2.18244,
            OrbitalInclination = 0.0898,
            OrbitalEccentricity = 0.0549,
            DayNumberAtEpoch = 2451545,
            SiderealTimeAtEpoch = 4.889488,
            SiderealTimePerDay = 6.300388,
            EpochDate = _calendar.GetDate("21/jan/2000"),
            PeakIllumination = 1.0,
            FullMoonReferenceDay = 0
        };
        celestials.Add(_moon);

        _planet = new PlanetFromMoon(_moon, _sun)
        {
            PeakIllumination = 1.0,
            AngularRadius = 1.0
        };
        celestials.Add(_planet);
    }

    [TestMethod]
    public void EquatorialCoordinatesOppositeMoon()
    {
        var dn = _moon.CurrentDayNumber;
        var (raM, decM) = _moon.EquatorialCoordinates(dn);
        var (raP, decP) = _planet.EquatorialCoordinates(dn);
        Assert.AreEqual((raM + Math.PI).Modulus(2 * Math.PI), raP, 0.000001, "RA should be opposite");
        Assert.AreEqual(-decM, decP, 0.000001, "Dec should be negated");
    }

    [TestMethod]
    public void IlluminationComplementMoon()
    {
        _calendar.SetDate("21/jan/2000");
        _clock.CurrentTime.SetTime(0,0,0);
        var illum = _planet.CurrentIllumination(new GeographicCoordinate(0,0,0,0));
        Assert.IsTrue(illum < 0.02, "Planet should be dark at lunar eclipse");
        _calendar.SetDate("5/feb/2000");
        _clock.CurrentTime.SetTime(0,0,0);
        var illum2 = _planet.CurrentIllumination(new GeographicCoordinate(0,0,0,0));
        Assert.IsTrue(illum2 > 0.98, "Planet should be bright at new moon");
    }

    [TestMethod]
    public void DetectSolarEclipse()
    {
        _calendar.SetDate("21/jan/2000");
        _clock.CurrentTime.SetTime(0,0,0);
        Assert.IsTrue(_planet.IsSunEclipsed(new GeographicCoordinate(0,0,0,0)), "Sun should be eclipsed by planet");
    }
}
