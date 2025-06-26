using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.Models;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PlanetaryMoonTests
{
    private static PlanetaryMoon _moon;
    private static IFuturemud _gameworld;
    private static Calendar _calendar;
    private static Clock _clock;

    [ClassInitialize]
    public static void Init(TestContext context)
    {
        var saveManager = new Mock<ISaveManager>();
        saveManager.Setup(x => x.Add(It.IsAny<ISaveable>()));

        var clocks = new All<IClock>();
        var calendars = new All<ICalendar>();

        var mock = new Mock<IFuturemud>();
        mock.SetupGet(x => x.Clocks).Returns(clocks);
        mock.SetupGet(x => x.Calendars).Returns(calendars);
        mock.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
        _gameworld = mock.Object;

        _clock = new Clock(XElement.Parse(@"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=\"night\" Lower=\"-2\" Upper=\"4\"/>    <CrudeTimeInterval text=\"morning\" Lower=\"4\" Upper=\"12\"/>    <CrudeTimeInterval text=\"afternoon\" Lower=\"12\" Upper=\"18\"/>    <CrudeTimeInterval text=\"evening\" Lower=\"18\" Upper=\"22\"/>  </CrudeTimeIntervals></Clock>"),
            _gameworld,
            new MudTimeZone(1,0,0,"UTC+0","utc"),
            12,
            0,
            0) { Id = 1 };
        clocks.Add(_clock);

        _calendar = new Calendar();
        _calendar.SetupTestData();
        _calendar.FeedClock = _clock;
        calendars.Add(_calendar);
        _calendar.SetDate("1/jan/2000");

        _moon = new PlanetaryMoon(_calendar, _clock)
        {
            CelestialDaysPerYear = 29.530588,
            MeanAnomalyAngleAtEpoch = 5.55665,
            AnomalyChangeAnglePerDay = 0.229971,
            ArgumentOfPeriapsis = 5.1985,
            LongitudeOfAscendingNode = 2.18244,
            OrbitalInclination = 0.0898,
            OrbitalEccentricity = 0.0549,
            DayNumberAtEpoch = 2451545,
            SiderealTimeAtEpoch = 4.889488,
            SiderealTimePerDay = 6.300388,
            EpochDate = _calendar.GetDate("1/jan/2000"),
            PeakIllumination = 1.0,
            FullMoonReferenceDay = 0
        };
    }

    [TestMethod]
    public void TestMoonPhaseCycle()
    {
        _calendar.SetDate("1/jan/2000");
        Assert.AreEqual(MoonPhase.Full, _moon.CurrentPhase());

        _calendar.SetDate("8/jan/2000");
        Assert.AreEqual(MoonPhase.FirstQuarter, _moon.CurrentPhase());

        _calendar.SetDate("15/jan/2000");
        Assert.AreEqual(MoonPhase.New, _moon.CurrentPhase());

        _calendar.SetDate("22/jan/2000");
        Assert.AreEqual(MoonPhase.LastQuarter, _moon.CurrentPhase());
    }
}
