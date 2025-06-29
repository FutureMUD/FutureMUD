using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
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
	private static ICalendar _calendar;
	private static IClock _clock;

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

		_clock = new MudSharp.TimeAndDate.Time.Clock(XElement.Parse(@"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4""/>    <CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12""/>    <CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18""/>    <CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22""/>  </CrudeTimeIntervals></Clock>"),
			_gameworld,
			new MudTimeZone(1,0,0,"UTC+0","utc"),
			12,
			0,
			0) { Id = 1 };
		clocks.Add(_clock);

		_calendar = new MudSharp.TimeAndDate.Date.Calendar(XElement.Parse(
				@"<calendar>
  <alias>gregorian</alias>
  <shortname>Gregorian Calendar (EN-UK)</shortname>
  <fullname>The Gregorian Calendar, in English with British Date Display</fullname>
  <description><![CDATA[The calendar created by pope Gregory to replace the Julian calendar. English edition.]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $yy (A.D)</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of our Lord</wordystring>
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
	<month>
	  <alias>january</alias>
	  <shortname>jan</shortname>
	  <fullname>January</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
	   <specialday day=""1"" short=""New Years Day"" long=""New Years Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>february</alias>
	  <shortname>feb</shortname>
	  <fullname>February</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays>
		<intercalaryday>
		  <insertdays>1</insertdays>
		  <nonweekdays />
		  <removenonweekdays />
		  <specialdays/>
		  <removespecialdays />
		  <intercalaryrule>
			<offset>0</offset>
			<divisor>4</divisor>
			<exceptions>
			  <intercalaryrule>
				<offset>0</offset>
				<divisor>100</divisor>
				<exceptions>
				  <intercalaryrule>
					<offset>0</offset>
					<divisor>400</divisor>
					<exceptions />
					<ands />
					<ors />
				  </intercalaryrule>
				</exceptions>
				<ands />
				<ors />
			  </intercalaryrule>
			</exceptions>
			<ands />
			<ors />
		  </intercalaryrule>
		</intercalaryday>
	  </intercalarydays>
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>march</alias>
	  <shortname>mar</shortname>
	  <fullname>March</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>april</alias>
	  <shortname>apr</shortname>
	  <fullname>April</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>may</alias>
	  <shortname>may</shortname>
	  <fullname>May</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>june</alias>
	  <shortname>jun</shortname>
	  <fullname>June</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>july</alias>
	  <shortname>jul</shortname>
	  <fullname>July</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>august</alias>
	  <shortname>aug</shortname>
	  <fullname>August</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>september</alias>
	  <shortname>sep</shortname>
	  <fullname>September</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>october</alias>
	  <shortname>oct</shortname>
	  <fullname>October</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>november</alias>
	  <shortname>nov</shortname>
	  <fullname>November</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>december</alias>
	  <shortname>dec</shortname>
	  <fullname>December</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""25"" short=""Christmas"" long=""Christmas Day"" />
		<specialday day=""26"" short=""Boxing Day"" long=""Boxing Day"" />
		<specialday day=""31"" short=""New Years Eve"" long=""New Years Eve"" />
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths />
</calendar>")
			, _gameworld)
		{
			Id = 1
		};
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
			EpochDate = _calendar.GetDate("21/jan/2000"),
			PeakIllumination = 1.0,
			FullMoonReferenceDay = 0
		};
	}

	[TestMethod]
		public void TestMoonPhaseCycle()
		{
		_calendar.SetDate("1/dec/1999");
		var sb = new StringBuilder();
		for (var i = 0; i < 100; i++)
		{
			sb.AppendLine($"#{i} - {_calendar.DisplayDate(CalendarDisplayMode.Short)} - {_moon.CurrentPhase()}");
			_calendar.CurrentDate.AdvanceDays(1);
		}

		void TryTest(string date, MoonPhase expected)
		{
			_calendar.SetDate(date);
			Assert.AreEqual(expected, _moon.CurrentPhase(), $"Expected {expected.Describe()} for date {_calendar.DisplayDate(CalendarDisplayMode.Short)}, actual {_moon.CurrentPhase().Describe()}");
		}

		TryTest("16/dec/1999", MoonPhase.FirstQuarter);
		TryTest("23/dec/1999", MoonPhase.Full);
		TryTest("28/dec/1999", MoonPhase.WaningGibbous);
		TryTest("30/dec/1999", MoonPhase.LastQuarter);
		TryTest("21/jan/2000", MoonPhase.Full);
		TryTest("4/jan/2000", MoonPhase.WaningCrescent);
		TryTest("7/jan/2000", MoonPhase.New);
				TryTest("15/jan/2000", MoonPhase.FirstQuarter);
				TryTest("17/jan/2000", MoonPhase.WaxingGibbous);
		}

		[TestMethod]
		public void TestTriggerEchoSelection()
		{
				_moon.Triggers.Clear();
				_moon.Triggers.Add(new CelestialTrigger(-0.015184, CelestialMoveDirection.Ascending, "rise"));
				_moon.Triggers.Add(new CelestialTrigger(-0.015184, CelestialMoveDirection.Descending, "set"));

				var oldStatus = new CelestialInformation(_moon, 0.0, -0.03, CelestialMoveDirection.Ascending);
				var newStatus = new CelestialInformation(_moon, 0.0, 0.0, CelestialMoveDirection.Ascending);

				Assert.IsTrue(_moon.ShouldEcho(oldStatus, newStatus));

				var method = typeof(PlanetaryMoon).GetMethod("GetZoneDisplayTrigger", BindingFlags.NonPublic | BindingFlags.Instance);
				var trigger = (CelestialTrigger)method.Invoke(_moon, new object[] { oldStatus, newStatus });
				Assert.AreEqual("rise", trigger.Echo);
		}
}
