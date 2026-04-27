using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgDateTimeFunctionTests
{
	private static IFuturemud _gameworld = null!;
	private static ICalendar _calendar = null!;
	private static IClock _clock = null!;
	private static IMudTimeZone _timezone = null!;

	[ClassInitialize]
	public static void ClassInitialize(TestContext testContext)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		_gameworld = FutureProgTestBootstrap.Gameworld;

		var saveManager = new Mock<ISaveManager>();
		saveManager.Setup(x => x.Add(It.IsAny<ISaveable>()));

		var clocks = new All<IClock>();
		var calendars = new All<ICalendar>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Clocks).Returns(clocks);
		gameworld.SetupGet(x => x.Calendars).Returns(calendars);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);

		_clock = new Clock(XElement.Parse(
			@"<Clock><Alias>UTC</Alias><Description>Universal Time Clock</Description><ShortDisplayString>$j:$m:$s $i</ShortDisplayString><SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString><LongDisplayString>$c $i</LongDisplayString><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay><InGameSecondsPerRealSecond>1</InGameSecondsPerRealSecond><SecondFixedDigits>2</SecondFixedDigits><MinuteFixedDigits>2</MinuteFixedDigits><HourFixedDigits>0</HourFixedDigits><NoZeroHour>true</NoZeroHour><NumberOfHourIntervals>2</NumberOfHourIntervals><HourIntervalNames><HourIntervalName>a.m</HourIntervalName><HourIntervalName>p.m</HourIntervalName></HourIntervalNames><HourIntervalLongNames><HourIntervalLongName>in the morning</HourIntervalLongName><HourIntervalLongName>in the afternoon</HourIntervalLongName></HourIntervalLongNames><CrudeTimeIntervals><CrudeTimeInterval text=""day"" Lower=""0"" Upper=""24"" /></CrudeTimeIntervals></Clock>"),
			gameworld.Object)
		{
			Id = 1
		};
		_timezone = new MudTimeZone(1, 0, 0, "Universal Time Clock", "UTC");
		_clock.AddTimezone(_timezone);
		_clock.SetTime(new MudTime(0, 0, 0, _timezone, _clock, true));
		clocks.Add(_clock);

		var calendar = new Calendar(1, gameworld.Object);
		calendar.SetNoSave(true);
		calendar.SetupTestData();
		calendar.SetDate("1/jan/2026");
		calendars.Add(calendar);
		_calendar = calendar;
	}

	[TestMethod]
	public void NextWeekday_SystemDateTime_ReturnsNthWeekday()
	{
		var prog = Compile<DateTime>(
			"NextSystemWeekday",
			ProgVariableTypes.DateTime,
			[
				Tuple.Create(ProgVariableTypes.DateTime, "date")
			],
			@"return NextWeekday(@date, ""Monday"", 2)");

		var result = prog.Execute<System.DateTime>(new System.DateTime(2026, 4, 22, 15, 30, 0, DateTimeKind.Utc));

		Assert.AreEqual(new System.DateTime(2026, 5, 4, 15, 30, 0, DateTimeKind.Utc), result);
	}

	[TestMethod]
	public void NextWeekday_SystemDateTime_NegativeOccurrenceReturnsPreviousWeekday()
	{
		var prog = Compile<DateTime>(
			"NextSystemWeekdayNegative",
			ProgVariableTypes.DateTime,
			[
				Tuple.Create(ProgVariableTypes.DateTime, "date")
			],
			@"return NextWeekday(@date, ""Mon"", -1)");

		var result = prog.Execute<System.DateTime>(new System.DateTime(2026, 4, 22, 15, 30, 0, DateTimeKind.Utc));

		Assert.AreEqual(new System.DateTime(2026, 4, 20, 15, 30, 0, DateTimeKind.Utc), result);
	}

	[TestMethod]
	public void LastWeekday_MudDateTime_ReturnsPreviousWeekday()
	{
		var prog = Compile<MudDateTime>(
			"LastMudWeekday",
			ProgVariableTypes.MudDateTime,
			[
				Tuple.Create(ProgVariableTypes.MudDateTime, "date")
			],
			@"return LastWeekday(@date, ""Friday"")");
		var reference = new MudDateTime(_calendar.GetDate("8/apr/2026"),
			new MudTime(0, 0, 9, _timezone, _clock, false), _timezone);

		var result = prog.Execute<MudDateTime>(reference);

		Assert.AreEqual("3/april/2026", result.Date.GetDateString());
		Assert.AreEqual("Friday", result.Date.Weekday);
		Assert.AreEqual(9, result.Time.Hours);
	}

	[TestMethod]
	public void NextWeekday_MudDateTime_NegativeOccurrenceReturnsPreviousWeekday()
	{
		var prog = Compile<MudDateTime>(
			"NextMudWeekdayNegative",
			ProgVariableTypes.MudDateTime,
			[
				Tuple.Create(ProgVariableTypes.MudDateTime, "date")
			],
			@"return NextWeekday(@date, ""Friday"", -1)");
		var reference = new MudDateTime(_calendar.GetDate("8/apr/2026"),
			new MudTime(0, 0, 9, _timezone, _clock, false), _timezone);

		var result = prog.Execute<MudDateTime>(reference);

		Assert.AreEqual("3/april/2026", result.Date.GetDateString());
		Assert.AreEqual("Friday", result.Date.Weekday);
	}

	[TestMethod]
	public void Between_Number_IsInclusiveAndOrderInsensitive()
	{
		var prog = Compile<bool>(
			"BetweenNumber",
			ProgVariableTypes.Boolean,
			[
				Tuple.Create(ProgVariableTypes.Number, "value")
			],
			"return Between(@value, 10, 1)");

		Assert.IsTrue(prog.ExecuteBool(10M));
		Assert.IsTrue(prog.ExecuteBool(5M));
		Assert.IsFalse(prog.ExecuteBool(11M));
	}

	[TestMethod]
	public void Between_DateTime_IsInclusiveAndOrderInsensitive()
	{
		var prog = Compile<bool>(
			"BetweenDateTime",
			ProgVariableTypes.Boolean,
			[
				Tuple.Create(ProgVariableTypes.DateTime, "date"),
				Tuple.Create(ProgVariableTypes.DateTime, "low"),
				Tuple.Create(ProgVariableTypes.DateTime, "high")
			],
			"return Between(@date, @high, @low)");
		var low = new System.DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc);
		var high = new System.DateTime(2026, 4, 22, 0, 0, 0, DateTimeKind.Utc);

		Assert.IsTrue(prog.ExecuteBool(low, low, high));
		Assert.IsTrue(prog.ExecuteBool(new System.DateTime(2026, 4, 21, 0, 0, 0, DateTimeKind.Utc), low, high));
		Assert.IsFalse(prog.ExecuteBool(new System.DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc), low, high));
	}

	[TestMethod]
	public void Between_MudDateTime_IsInclusiveAndOrderInsensitive()
	{
		var prog = Compile<bool>(
			"BetweenMudDateTime",
			ProgVariableTypes.Boolean,
			[
				Tuple.Create(ProgVariableTypes.MudDateTime, "date"),
				Tuple.Create(ProgVariableTypes.MudDateTime, "low"),
				Tuple.Create(ProgVariableTypes.MudDateTime, "high")
			],
			"return Between(@date, @high, @low)");
		var low = new MudDateTime(_calendar.GetDate("20/apr/2026"),
			new MudTime(0, 0, 0, _timezone, _clock, false), _timezone);
		var middle = new MudDateTime(_calendar.GetDate("21/apr/2026"),
			new MudTime(0, 0, 0, _timezone, _clock, false), _timezone);
		var high = new MudDateTime(_calendar.GetDate("22/apr/2026"),
			new MudTime(0, 0, 0, _timezone, _clock, false), _timezone);
		var outside = new MudDateTime(_calendar.GetDate("23/apr/2026"),
			new MudTime(0, 0, 0, _timezone, _clock, false), _timezone);

		Assert.IsTrue(prog.ExecuteBool(low, low, high));
		Assert.IsTrue(prog.ExecuteBool(middle, low, high));
		Assert.IsFalse(prog.ExecuteBool(outside, low, high));
	}

	[TestMethod]
	public void Between_TimeSpan_IsInclusiveAndOrderInsensitive()
	{
		var prog = Compile<bool>(
			"BetweenTimeSpan",
			ProgVariableTypes.Boolean,
			[
				Tuple.Create(ProgVariableTypes.TimeSpan, "duration"),
				Tuple.Create(ProgVariableTypes.TimeSpan, "low"),
				Tuple.Create(ProgVariableTypes.TimeSpan, "high")
			],
			"return Between(@duration, @high, @low)");

		Assert.IsTrue(prog.ExecuteBool(TimeSpan.FromHours(2), TimeSpan.FromHours(1), TimeSpan.FromHours(3)));
		Assert.IsFalse(prog.ExecuteBool(TimeSpan.FromHours(4), TimeSpan.FromHours(1), TimeSpan.FromHours(3)));
	}

	[TestMethod]
	public void ToMudDate_InvalidText_ReturnsNever()
	{
		var prog = Compile<MudDateTime>(
			"ToMudDateInvalidText",
			ProgVariableTypes.MudDateTime,
			[
				Tuple.Create(ProgVariableTypes.Calendar, "calendar"),
				Tuple.Create(ProgVariableTypes.Clock, "clock")
			],
			@"return ToDate(@calendar, @clock, ""not-a-date"")");

		var result = prog.Execute<MudDateTime>(_calendar, _clock);

		Assert.IsTrue(result.Equals(MudDateTime.Never));
	}

	private static FutureProg Compile<T>(string name, ProgVariableTypes returnType,
		IEnumerable<Tuple<ProgVariableTypes, string>> parameters, string functionText)
	{
		var prog = new FutureProg(_gameworld, name, returnType, parameters, functionText);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		return prog;
	}
}
