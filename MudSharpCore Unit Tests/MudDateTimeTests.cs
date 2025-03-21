using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;
using Moq;
using MudSharp.Framework.Save;

namespace MudSharp_Unit_Tests;

/// <summary>
/// Summary description for DateTimeTests
/// </summary>
[TestClass]
public class MudDateTimeTests
{
	public MudDateTimeTests()
	{
		//
		// TODO: Add constructor logic here
		//
	}

	private TestContext testContextInstance;

	/// <summary>
	///Gets or sets the test context which provides
	///information about and functionality for the current test run.
	///</summary>
	public TestContext TestContext {
		get {
			return testContextInstance;
		}
		set {
			testContextInstance = value;
		}
	}

	private static ICalendar _testCalendar;
	private static IClock _testClock;
	private static IMudTimeZone _cstTimezone;
	private static IMudTimeZone _utcTimezone;
	private static IFuturemud _gameworld;

	#region Additional test attributes
	//
	// You can use the following additional attributes as you write your tests:
	[ClassInitialize]
	public static void MyClassInitialize(TestContext testContext) {
		var saveManagerMock = new Mock<ISaveManager>();
		saveManagerMock.Setup(x => x.Add(It.IsAny<ISaveable>()));

		var clocks = new All<IClock>();

		var calendars = new All<ICalendar>();

		var mock = new Mock<IFuturemud>();
		mock.SetupGet(t => t.Clocks).Returns(clocks);
		mock.SetupGet(t => t.Calendars).Returns(calendars);
		mock.SetupGet(t => t.SaveManager).Returns(saveManagerMock.Object);
		_gameworld = mock.Object;

		_testCalendar = new Calendar(XElement.Parse(
				@"<calendar><alias>labmud</alias><shortname>The LabMUD Calendar</shortname><fullname>The LabMUD Calendar</fullname><description><![CDATA[The calendar used by test subjects in the Lab, and by which all dates of importance to test subjects are communicated.]]></description><shortstring>$dd/$mo/$yy $ee</shortstring><longstring>$nz$ww the $dt of $mf, year $yy $EE</longstring><wordystring>$nz$ww the $dt of $mf, year $yy $EE</wordystring><plane>earth</plane><feedclock>0</feedclock><epochyear>1</epochyear><weekdayatepoch>5</weekdayatepoch><ancienterashortstring>B.T</ancienterashortstring><ancienteralongstring>before Tranquility</ancienteralongstring><modernerashortstring>A.T</modernerashortstring><moderneralongstring>after Tranquility</moderneralongstring><weekdays><weekday>Work Day</weekday><weekday>Sports Day</weekday><weekday>Science Day</weekday><weekday>Hump Day</weekday><weekday>Garbage Day</weekday><weekday>Laundry Day</weekday><weekday>Lazy Day</weekday></weekdays><months><month><alias>archimedes</alias><shortname>arc</shortname><fullname>Archimedes</fullname><nominalorder>1</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays></specialdays><nonweekdays /></month><month><alias>brahe</alias><shortname>bra</shortname><fullname>Brahe</fullname><nominalorder>2</nominalorder><normaldays>28</normaldays><intercalarydays/><specialdays /><nonweekdays /></month><month><alias>copernicus</alias><shortname>cop</shortname><fullname>Copernicus</fullname><nominalorder>3</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>darwin</alias><shortname>dar</shortname><fullname>Darwin</fullname><nominalorder>4</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>einstein</alias><shortname>ein</shortname><fullname>Einstein</fullname><nominalorder>5</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>faraday</alias><shortname>far</shortname><fullname>Faraday</fullname><nominalorder>6</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>galileo</alias><shortname>gal</shortname><fullname>Galileo</fullname><nominalorder>7</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>hippocrates</alias><shortname>hip</shortname><fullname>Hippocrates</fullname><nominalorder>8</nominalorder><normaldays>28</normaldays><specialdays /><intercalarydays><intercalary><insertdays>1</insertdays><specialdays><specialday day=""29"" short=""Aldrin Day"" long=""Aldrin Day"" /></specialdays><nonweekdays><nonweekday>29</nonweekday></nonweekdays><removenonweekdays /><removespecialdays /><intercalaryrule><offset>-31</offset><divisor>4</divisor><exceptions><intercalaryrule><offset>-31</offset><divisor>100</divisor><exceptions><intercalaryrule><offset>-31</offset><divisor>400</divisor><exceptions /><ands /><ors /></intercalaryrule></exceptions><ands /><ors /></intercalaryrule></exceptions><ands /><ors /></intercalaryrule></intercalary></intercalarydays><nonweekdays /></month><month><alias>imhotep</alias><shortname>imh</shortname><fullname>Imhotep</fullname><nominalorder>9</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>jung</alias><shortname>jun</shortname><fullname>Jung</fullname><nominalorder>10</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>kepler</alias><shortname>kep</shortname><fullname>Kepler</fullname><nominalorder>11</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>lavoisier</alias><shortname>lav</shortname><fullname>Lavoisier</fullname><nominalorder>12</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>mendel</alias><shortname>men</shortname><fullname>Mendel</fullname><nominalorder>13</nominalorder><normaldays>28</normaldays><intercalarydays></intercalarydays><specialdays /><nonweekdays /></month></months><intercalarymonths><intercalarymonth><position>14</position><month><alias>tranquility</alias><shortname>tra</shortname><fullname>Tranquility</fullname><nominalorder>14</nominalorder><normaldays>1</normaldays><intercalarydays/><specialdays><specialday day=""1"" short=""Armstrong Day"" long=""Armstrong Day"" /></specialdays><nonweekdays><nonweekday>1</nonweekday></nonweekdays></month><intercalaryrule><offset>0</offset><divisor>1</divisor><exceptions/><ands /><ors /></intercalaryrule></intercalarymonth></intercalarymonths></calendar>"),
			_gameworld) {
			Id = 1
		};
		_testCalendar.SetDate("27/jun/34");

		_testClock = new Clock(XElement.Parse(@"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4""/>    <CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12""/>    <CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18""/>    <CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22""/>  </CrudeTimeIntervals></Clock>"), _gameworld) {Id = 1};

		_testCalendar.FeedClock = _testClock;
		_utcTimezone = new MudTimeZone(1, 0, 0, "Universal Time Clock", "UTC");
		_testClock.AddTimezone(_utcTimezone);
		_cstTimezone = new MudTimeZone(2, -6, 0, "Central Time", "CST");
		_testClock.AddTimezone(_cstTimezone);
		_testClock.SetTime(new MudTime(0, 0, 0, _utcTimezone, _testClock, true));
	}
	//
	// Use ClassCleanup to run code after all tests in a class have run
	// [ClassCleanup()]
	// public static void MyClassCleanup() { }
		
	// Use TestInitialize to run code before running each test 
	[TestInitialize()]
	public void MyTestInitialize()
	{
		_testCalendar.SetDate("27/jun/34");
		_testClock.SetTime(new MudTime(0, 0, 0, _utcTimezone, _testClock, true));
	}
		
	// Use TestCleanup to run code after each test has run
	// [TestCleanup()]
	// public void MyTestCleanup() { }
	//
	#endregion

	[TestMethod]
	public void TestIntervals() {
		var assertionHit1 = false;
		var assertionHit2 = false;
		var listener2 = new DateListener(_testCalendar, 28, "jung", 34, _utcTimezone, 0, objs => assertionHit1 = true, new object[] { }, "Test");
		var listener1 = new DateListener(_testCalendar, 1, "kepler", 34, _utcTimezone, 0, objs => assertionHit2 = true, new object[] { }, "Test");

		_testCalendar.CurrentDate.AdvanceDays(1);
		Assert.AreEqual(true, assertionHit1, "Didn't advance from 27th to 28th");

		var expectedNextDate = _testCalendar.GetDate("1/kep/34");

		var daily = new RecurringInterval
		{
			IntervalAmount = 1,
			Modifier = 0,
			Type = IntervalType.Daily
		};

		Assert.AreEqual(expectedNextDate.GetDateString(), daily.GetNextDateExclusive(_testCalendar, _testCalendar.CurrentDate).GetDateString(), "The daily interval after the 28th was not the 1st");
		_testCalendar.CurrentDate.AdvanceDays(1);
		Assert.AreEqual(true, assertionHit2, "Didn't advance to new month");
	}

	[TestMethod]
	public void TestIntervalsWithDifferentTimezones()
	{
		var assertionHit1 = false;
		var listener1 = new DateListener(_testCalendar, 28, "jung", 34, _cstTimezone, 0, objs => assertionHit1 = true, new object[] { }, "Test");
		_testCalendar.CurrentDate.AdvanceDays(1);
		listener1.CancelListener();
		Assert.AreEqual(false, assertionHit1, "Advanced from 27th to 28th in the CST timezone, should have only advanced in UTC timezone");
	}

	[TestMethod]
	public void TestDateAddMethods()
	{
		var clock = new MudSharp.TimeAndDate.Time.Clock(XElement.Parse(@"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4""/>    <CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12""/>    <CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18""/>    <CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22""/>  </CrudeTimeIntervals></Clock>"), _gameworld)
		{
			Id = 1
		};
		var calendar = new MudSharp.TimeAndDate.Date.Calendar(XElement.Parse(@"<calendar>
  <alias>gregorian</alias>
  <shortname>Gregorian Calendar (EN-UK)</shortname>
  <fullname>The Gregorian Calendar, in English with British Date Display, circa 2012</fullname>
  <description><![CDATA[The calendar created by pope Gregory to replace the Julian calendar. English edition.]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $yy A.D</longstring>
  <wordystring>$NZ$ww on this $DT day of the month of $mf, in the $YO year of our Lord</wordystring>
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
		<specialday day=""26"" short=""Australia Day"" long=""Australia Day"" />
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
		<intercalary>
		  <insertdays>1</insertdays>
		  <nonweekdays />
		  <removenonweekdays />
		  <specialdays>
			<specialday day=""29"" short=""Backwards Day"" long=""Backwards Day"" />
		  </specialdays>
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
		</intercalary>
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
</calendar>"), _gameworld)
		{
			Id = 1,
			FeedClock = clock
		};
		calendar.SetDate("15/01/2020");

		var cd = calendar.CurrentDate;

		cd.AdvanceDays(1);
		Assert.IsTrue(cd.Day == 16 && cd.Month.Alias == "january" && cd.Year == 2020, $"Advancing 1 day from 15th to 16th January failed: {cd.ToString()}");

		cd.AdvanceDays(15);
		Assert.IsTrue(cd.Day == 31 && cd.Month.Alias == "january" && cd.Year == 2020, $"Advancing 15 days from 16th to 31st January failed: {cd.ToString()}");

		cd.AdvanceDays(2);
		Assert.IsTrue(cd.Day == 2 && cd.Month.Alias == "february" && cd.Year == 2020, $"Advancing 2 days from 31st Jan to 2nd Feb failed: {cd.ToString()}");

		cd.AdvanceDays(-2);
		Assert.IsTrue(cd.Day == 31 && cd.Month.Alias == "january" && cd.Year == 2020, $"Advancing -2 days to 31st Jan from 2nd Feb failed: {cd.ToString()}");

		cd.AdvanceDays(-31);
		Assert.IsTrue(cd.Day == 31 && cd.Month.Alias == "december" && cd.Year == 2019, $"Advancing -31 days from 31st January to 31st December failed: {cd.ToString()}");

		cd.AdvanceMonths(1, false, true);
		Assert.IsTrue(cd.Day == 31 && cd.Month.Alias == "january" && cd.Year == 2020, $"Advancing 1 month from 31st December to 31st January failed: {cd.ToString()}");

		cd.AdvanceMonths(1, false, true);
		Assert.IsTrue(cd.Day == 29 && cd.Month.Alias == "february" && cd.Year == 2020, $"Advancing 1 month from 31st January to 29th February failed: {cd.ToString()}");

		cd.AdvanceMonths(1, false, true);
		Assert.IsTrue(cd.Day == 29 && cd.Month.Alias == "march" && cd.Year == 2020, $"Advancing 1 month from 29th February to 29th March failed: {cd.ToString()}");

		calendar.SetDate("31/01/2020");
		cd = calendar.CurrentDate;

		cd.AdvanceMonths(2, false, true);
		Assert.IsTrue(cd.Day == 31 && cd.Month.Alias == "march" && cd.Year == 2020, $"Advancing 2 months from 31st January to 31st March failed: {cd.ToString()}");

		cd.AdvanceMonths(-4, false, true);
		Assert.IsTrue(cd.Day == 30 && cd.Month.Alias == "november" && cd.Year == 2019, $"Advancing -4 months from 31st March to 30th November failed: {cd.ToString()}");

		cd.AdvanceYears(1, false);
		Assert.IsTrue(cd.Day == 30 && cd.Month.Alias == "november" && cd.Year == 2020, $"Advancing 1 year from 30 November 2019 to 30 November 2020 failed: {cd.ToString()}");

		cd.AdvanceYears(2, false);
		Assert.IsTrue(cd.Day == 30 && cd.Month.Alias == "november" && cd.Year == 2022, $"Advancing 2 years from 30 November 2020 to 30 November 2022 failed: {cd.ToString()}");

		cd.AdvanceYears(-10, false);
		Assert.IsTrue(cd.Day == 30 && cd.Month.Alias == "november" && cd.Year == 2012, $"Advancing -10 years from 30 November 2022 to 30 November 2012 failed: {cd.ToString()}");

		calendar.SetDate("29/02/2020");
		cd = calendar.CurrentDate;

		cd.AdvanceYears(1, false);
		Assert.IsTrue(cd.Day == 28 && cd.Month.Alias == "february" && cd.Year == 2021, $"Advancing 1 year from 29 Feb 2020 to 28 Feb 2021 failed: {cd.ToString()}");

		cd.AdvanceYears(-1, false);
		Assert.IsTrue(cd.Day == 28 && cd.Month.Alias == "february" && cd.Year == 2020, $"Advancing -1 year to 29 Feb 2020 from 28 Feb 2021 failed: {cd.ToString()}");

		calendar.SetDate("29/02/2020");
		cd = calendar.CurrentDate;

		cd.AdvanceYears(1, true);
		Assert.IsTrue(cd.Day == 1 && cd.Month.Alias == "march" && cd.Year == 2021, $"Advancing 1 year (with date normalisation) from 29 Feb 2020 to 1st Mar 2021 failed: {cd.ToString()}");

		calendar.SetDate("01/01/2020");
		cd = calendar.CurrentDate;

		cd.AdvanceDays(366);
		Assert.IsTrue(cd.Day == 1 && cd.Month.Alias == "january" && cd.Year == 2021, $"Advancing 366 days from 31 Jan 2020 to 31 Jan 2021 failed: {cd.ToString()}");

		cd.AdvanceDays(-366);
		Assert.IsTrue(cd.Day == 1 && cd.Month.Alias == "january" && cd.Year == 2020, $"Advancing -366 days to 31 Jan 2020 from 31 Jan 2021 failed: {cd.ToString()}");
	}

	[TestMethod]
	public void TestMudTimeSpan()
	{
		var result = MudTimeSpan.TryParse("24:0:0", out var mts);
		Assert.IsTrue(result, "Was not valid using simple colon form for 24days.");
		Assert.AreEqual<TimeSpan>(TimeSpan.FromDays(24), mts);

		result = MudTimeSpan.TryParse("0:24:0:0", out mts);
		Assert.IsTrue(result, "Was not valid using simple colon form for 24hrs.");
		Assert.AreEqual<TimeSpan>(TimeSpan.FromHours(24), mts);

		result = MudTimeSpan.TryParse("24 days", out mts);
		Assert.IsTrue(result, "Was not valid using text - 24 days.");
		Assert.AreEqual<TimeSpan>(TimeSpan.FromDays(24), mts);

		result = MudTimeSpan.TryParse("2 months 5 days", out mts);
		Assert.IsTrue(result, "Was not valid using text - 2 months 5 days.");
		Assert.AreEqual<TimeSpan>(TimeSpan.FromDays(65), mts);

		result = MudTimeSpan.TryParse("3m", out mts);
		Assert.IsTrue(result, "Was not valid using text - 3m.");
		Assert.AreEqual<TimeSpan>(TimeSpan.FromMinutes(3), mts);

		result = MudTimeSpan.TryParse("3.000s", System.Globalization.CultureInfo.CreateSpecificCulture("de-DE"), out mts);
		Assert.IsTrue(result, "Was not valid using german text - 3.000s.");
		Assert.AreEqual<TimeSpan>(TimeSpan.FromSeconds(3000), mts);

		Assert.AreEqual(3, MudTimeSpan.FromDays(3).DayComponentOnly);
		Assert.AreEqual(10, MudTimeSpan.FromMinutes(10).MinuteComponentOnly);
		Assert.AreEqual(2, MudTimeSpan.FromMinutes(120).HourComponentOnly);
		Assert.AreEqual(0, MudTimeSpan.FromMinutes(120).MinuteComponentOnly);

		Assert.AreEqual<string>("5 weeks 302400000ms", MudTimeSpan.FromWeeks(5, 3.5).GetRoundTripParseText);
		Assert.AreEqual<string>("30000ms", MudTimeSpan.FromSeconds(30).GetRoundTripParseText);
		Assert.AreEqual<string>("zero", MudTimeSpan.Zero.GetRoundTripParseText);

		mts = MudTimeSpan.FromMonths(3, 14.357);
		Assert.IsTrue(mts.Equals(MudTimeSpan.Parse(mts.GetRoundTripParseText)), "The round-tripped MudTimeSpan was not equal to itself");
	}

	[TestMethod]
	public void TestMudTimeSpanDescribe()
	{
		Assert.AreEqual<string>("less than a second", MudTimeSpan.Zero.Describe());
		Assert.AreEqual<string>("3 days", MudTimeSpan.FromDays(3).Describe());
		Assert.AreEqual<string>("3 weeks", MudTimeSpan.FromWeeks(3).Describe());
		Assert.AreEqual<string>("1 week and 1 day", MudTimeSpan.FromWeeks(1, 1.0).Describe());
		Assert.AreEqual<string>("4 minutes and 30 seconds", MudTimeSpan.FromMinutes(4.5).Describe());
	}

	[TestMethod]
	public void TestTimeCompare()
	{
		Assert.IsTrue(new MudDateTime("2/kep/22 UTC 14:22:17", _testCalendar, _testClock) < new MudDateTime("2/kep/22 UTC 14:28:17", _testCalendar, _testClock));
		Assert.IsTrue(new MudDateTime("2/kep/22 UTC 14:22:17", _testCalendar, _testClock) < new MudDateTime("2/kep/22 UTC 15:22:17", _testCalendar, _testClock));
		Assert.IsTrue(new MudDateTime("2/kep/22 UTC 14:22:17", _testCalendar, _testClock) < new MudDateTime("2/kep/22 UTC 14:22:18", _testCalendar, _testClock));
		Assert.IsTrue(new MudDateTime("2/kep/22 CST 14:22:17", _testCalendar, _testClock) < new MudDateTime("2/kep/22 CST 14:22:18", _testCalendar, _testClock));
		Assert.IsTrue(new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock) < new MudDateTime("2/kep/22 CST 3:22:18", _testCalendar, _testClock));
		Assert.IsFalse(new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock) < new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock));
		Assert.IsTrue(new MudDateTime("1/kep/22 CST 3:22:17", _testCalendar, _testClock) < new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock));
		Assert.IsTrue(new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock).CompareTo(new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock)) == 0);
		Assert.IsFalse(new MudDateTime("2/kep/22 UTC 8:22:17", _testCalendar, _testClock).CompareTo(new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock)) == 0);
		Assert.IsTrue(new MudDateTime("2/kep/22 UTC 9:22:17", _testCalendar, _testClock).CompareTo(new MudDateTime("2/kep/22 CST 3:22:17", _testCalendar, _testClock)) == 0);
	}

	[TestMethod]
	public void TestConversionBetweenCalendars()
	{
		var calendar1 = new Calendar(XElement.Parse(@"<calendar>
   <alias>eldarin-sindarin</alias>
   <shortname>Eldarin Calendar (Sindarin)</shortname>
   <fullname>The Eldarin Calendar, in Sindarin</fullname>
   <description><![CDATA[The Eldarin Calendar of the Sindarin elves, with Sindarin month and holiday names]]></description>
   <shortstring>$dd/$mo/$yy</shortstring>
   <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
   <wordystring>$NZ$ww on this $DT day of the month of $mf, in the $YO $EE</wordystring>
   <plane>arda</plane>
   <feedclock>0</feedclock>
   <epochyear>0</epochyear>
   <weekdayatepoch>1</weekdayatepoch>
   <ancienterashortstring>bef.</ancienterashortstring>
   <ancienteralongstring>Before the Third Age</ancienteralongstring>
   <modernerashortstring>T.A.</modernerashortstring>
   <moderneralongstring>Third Age</moderneralongstring>
   <weekdays>
	 <weekday>Orgilion</weekday>
	 <weekday>Oranor</weekday>
	 <weekday>Orithil</weekday>
	 <weekday>Orgaladhad</weekday>
	 <weekday>Ormenel</weekday>
	 <weekday>Orbelain</weekday>
   </weekdays>
   <months>
	<month>
	   <alias>yestare</alias>
	   <shortname>yes</shortname>
	   <fullname>Yestarë</fullname>
	   <nominalorder>1</nominalorder>
	   <normaldays>1</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>ethuil</alias>
	   <shortname>eth</shortname>
	   <fullname>Ethuil</fullname>
	   <nominalorder>2</nominalorder>
	   <normaldays>54</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>laer</alias>
	   <shortname>lae</shortname>
	   <fullname>Laer</fullname>
	   <nominalorder>3</nominalorder>
	   <normaldays>72</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>iavas</alias>
	   <shortname>iav</shortname>
	   <fullname>Iavas</fullname>
	   <nominalorder>4</nominalorder>
	   <normaldays>54</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>enedhin</alias>
	   <shortname>ene</shortname>
	   <fullname>Enedhin</fullname>
	   <nominalorder>5</nominalorder>
	   <normaldays>3</normaldays>
	   <intercalarydays>
		<intercalary>
		  <insertdays>3</insertdays>
		  <specialdays>
			</specialdays>
		  <nonweekdays/>
			  <removenonweekdays />
			  <removespecialdays />
			  <intercalaryrule>
				<offset>0</offset>
				<divisor>12</divisor>
				<exceptions>
				  <intercalaryrule>
					<offset>0</offset>
					<divisor>144</divisor>
					<exceptions>
					</exceptions>
					<ands />
					<ors />
				  </intercalaryrule>
				</exceptions>
				<ands />
				<ors />
			  </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>firith</alias>
	   <shortname>fir</shortname>
	   <fullname>Firith</fullname>
	   <nominalorder>6</nominalorder>
	   <normaldays>54</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>rhiw</alias>
	   <shortname>rhi</shortname>
	   <fullname>Rhîw</fullname>
	   <nominalorder>7</nominalorder>
	   <normaldays>72</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>echuir</alias>
	   <shortname>ech</shortname>
	   <fullname>Echuir</fullname>
	   <nominalorder>8</nominalorder>
	   <normaldays>54</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>penninor</alias>
	   <shortname>pen</shortname>
	   <fullname>Penninor</fullname>
	   <nominalorder>9</nominalorder>
	   <normaldays>1</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
   </months>
   <intercalarymonths>
   </intercalarymonths>
 </calendar>"), _gameworld)
		{
			Id = 1,
			FeedClock = _testClock
		};
		calendar1.SetDate("47/iavas/2481");

		var calendar2 = new Calendar(XElement.Parse(@"<calendar>
   <alias>kings-reckoning-sindarin</alias>
   <shortname>King's Reckoning Calendar (Sindarin)</shortname>
   <fullname>The King's Reckoning Calendar, in Sindarin</fullname>
   <description><![CDATA[The King's Reckoning Calendar of the 2nd Age Numenoreans, with Sindarin month and holiday names]]></description>
   <shortstring>$dd/$mo/$yy</shortstring>
   <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
   <wordystring>$NZ$ww on this $DT day of the month of $mf, in the $YO $EE</wordystring>
   <plane>arda</plane>
   <feedclock>0</feedclock>
   <epochyear>0</epochyear>
   <weekdayatepoch>1</weekdayatepoch>
   <ancienterashortstring>bef.</ancienterashortstring>
   <ancienteralongstring>Before the Third Age</ancienteralongstring>
   <modernerashortstring>T.A.</modernerashortstring>
   <moderneralongstring>Third Age</moderneralongstring>
   <weekdays>
	 <weekday>Orgilion</weekday>
	 <weekday>Oranor</weekday>
	 <weekday>Orithil</weekday>
	 <weekday>Orgaladh</weekday>
	 <weekday>Ormenel</weekday>
	 <weekday>Orbelain</weekday>
	<weekday>Oraearon</weekday>
   </weekdays>
   <months>
	<month>
	   <alias>yestare</alias>
	   <shortname>yes</shortname>
	   <fullname>Yestarë</fullname>
	   <nominalorder>1</nominalorder>
	   <normaldays>1</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>narwain</alias>
	   <shortname>nar</shortname>
	   <fullname>Narwain</fullname>
	   <nominalorder>2</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>ninui</alias>
	   <shortname>nin</shortname>
	   <fullname>Nínui</fullname>
	   <nominalorder>3</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>gwaeron</alias>
	   <shortname>gwa</shortname>
	   <fullname>Gwaeron</fullname>
	   <nominalorder>4</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>gwirith</alias>
	   <shortname>gwi</shortname>
	   <fullname>Gwirith</fullname>
	   <nominalorder>5</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays>
	  </intercalarydays>
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>lothron</alias>
	   <shortname>lot</shortname>
	   <fullname>Lothron</fullname>
	   <nominalorder>6</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>norui</alias>
	   <shortname>nor</shortname>
	   <fullname>Nórui</fullname>
	   <nominalorder>7</nominalorder>
	   <normaldays>31</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>enedhin</alias>
	   <shortname>ene</shortname>
	   <fullname>Enedhin</fullname>
	   <nominalorder>8</nominalorder>
	   <normaldays>1</normaldays>
	   <intercalarydays>
	  <intercalary>
		  <insertdays>1</insertdays>
		  <specialdays>
			</specialdays>
			<nonweekdays>
			  </nonweekdays>
			  <removenonweekdays />
			  <removespecialdays />
			  <intercalaryrule>
				<offset>0</offset>
				<divisor>4</divisor>
				<exceptions>
				  <intercalaryrule>
					<offset>0</offset>
					<divisor>100</divisor>
					<exceptions>
					</exceptions>
					<ands />
					<ors />
				  </intercalaryrule>
				</exceptions>
				<ands />
				<ors />
			  </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	 <month>
	   <alias>cerveth</alias>
	   <shortname>cer</shortname>
	   <fullname>Cerveth</fullname>
	   <nominalorder>9</nominalorder>
	   <normaldays>31</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	<month>
	   <alias>urui</alias>
	   <shortname>uru</shortname>
	   <fullname>Urui</fullname>
	   <nominalorder>10</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	<month>
	   <alias>ivanneth</alias>
	   <shortname>iva</shortname>
	   <fullname>Ivanneth</fullname>
	   <nominalorder>10</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	<month>
	   <alias>narbeleth</alias>
	   <shortname>nar</shortname>
	   <fullname>Narbeleth</fullname>
	   <nominalorder>11</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	<month>
	   <alias>hithui</alias>
	   <shortname>hit</shortname>
	   <fullname>Hithui</fullname>
	   <nominalorder>12</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	<month>
	   <alias>Girithron</alias>
	   <shortname>gir</shortname>
	   <fullname>Girithron</fullname>
	   <nominalorder>13</nominalorder>
	   <normaldays>30</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
	<month>
	   <alias>penninor</alias>
	   <shortname>pen</shortname>
	   <fullname>Penninor</fullname>
	   <nominalorder>14</nominalorder>
	   <normaldays>1</normaldays>
	   <intercalarydays />
	   <specialdays>
	  </specialdays>
	   <nonweekdays />
	 </month>
   </months>
   <intercalarymonths>
   </intercalarymonths>
 </calendar>"), _gameworld)
		{
			Id = 2,
			FeedClock = _testClock
		};
		calendar2.SetDate("23/norui/2481");

		Assert.IsTrue(calendar1.CurrentDate.DayNumberInYear() == calendar2.CurrentDate.DayNumberInYear(), $"1st Calendar was day #{calendar1.CurrentDate.DayNumberInYear()} and 2nd was {calendar2.CurrentDate.DayNumberInYear()}");
		var date1to2 = calendar1.CurrentDate.ConvertToOtherCalendar(calendar2);
		Assert.IsTrue(date1to2.Equals(calendar2.CurrentDate));

	}

	[TestMethod]
	public void TestTimeDescribers()
	{
		Assert.AreEqual("1:38:49 a.m", _testClock.DisplayTime(new MudTime(49, 38, 1, _utcTimezone, _testClock, 0), "$j:$m:$s $i"));
		Assert.AreEqual("12:00:00 a.m", _testClock.DisplayTime(new MudTime(0, 0, 0, _utcTimezone, _testClock, 0), "$j:$m:$s $i"));
		Assert.AreEqual("12:00:01 a.m", _testClock.DisplayTime(new MudTime(1, 0, 0, _utcTimezone, _testClock, 0), "$j:$m:$s $i"));
		Assert.AreEqual("11:59:59 p.m", _testClock.DisplayTime(new MudTime(59, 59, 23, _utcTimezone, _testClock, 0), "$j:$m:$s $i"));
		Assert.AreEqual("23:59:59", _testClock.DisplayTime(new MudTime(59, 59, 23, _utcTimezone, _testClock, 0), "$h:$m:$s"));
		Assert.AreEqual("1:38:49", _testClock.DisplayTime(new MudTime(49, 38, 1, _utcTimezone, _testClock, 0), "$h:$m:$s"));
		Assert.AreEqual("twelve o'clock a.m", _testClock.DisplayTime(new MudTime(59, 59, 23, _utcTimezone, _testClock, 0), "$c $l"));
		Assert.AreEqual("twelve o'clock a.m", _testClock.DisplayTime(new MudTime(0, 0, 0, _utcTimezone, _testClock, 0), "$c $l"));

		// Testing the incorrect one
		Assert.AreEqual("twelve o'clock p.m", _testClock.DisplayTime(new MudTime(59, 59, 23, _utcTimezone, _testClock, 0), "$c $i"));
	}
}