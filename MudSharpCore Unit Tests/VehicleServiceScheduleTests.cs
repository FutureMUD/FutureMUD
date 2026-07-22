#nullable enable

using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using MudSharp.Vehicles;

namespace MudSharpCore_Unit_Tests.Vehicles;

[TestClass]
public class VehicleServiceScheduleTests
{
	private ICalendar _calendar = null!;
	private IClock _clock = null!;
	private IMudTimeZone _timezone = null!;

	[TestInitialize]
	public void Initialise()
	{
		var clocks = new All<IClock>();
		var calendars = new All<ICalendar>();
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Clocks).Returns(clocks);
		gameworld.SetupGet(x => x.Calendars).Returns(calendars);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);

		_clock = new Clock(XElement.Parse(
			@"<Clock><Alias>UTC</Alias><Description>UTC</Description><ShortDisplayString>$j:$m:$s $i</ShortDisplayString><SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString><LongDisplayString>$c $i</LongDisplayString><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay><InGameSecondsPerRealSecond>1</InGameSecondsPerRealSecond><SecondFixedDigits>2</SecondFixedDigits><MinuteFixedDigits>2</MinuteFixedDigits><HourFixedDigits>0</HourFixedDigits><NoZeroHour>true</NoZeroHour><NumberOfHourIntervals>2</NumberOfHourIntervals><HourIntervalNames><HourIntervalName>a.m</HourIntervalName><HourIntervalName>p.m</HourIntervalName></HourIntervalNames><HourIntervalLongNames><HourIntervalLongName>in the morning</HourIntervalLongName><HourIntervalLongName>in the afternoon</HourIntervalLongName></HourIntervalLongNames><CrudeTimeIntervals><CrudeTimeInterval text=""day"" Lower=""0"" Upper=""24"" /></CrudeTimeIntervals></Clock>"),
			gameworld.Object)
		{
			Id = 1
		};
		_timezone = new MudTimeZone(1, 0, 0, "Universal Time Clock", "UTC");
		_clock.AddTimezone(_timezone);
		clocks.Add(_clock);

		var calendar = new Calendar(1, gameworld.Object);
		calendar.SetNoSave(true);
		calendar.SetupTestData();
		calendar.SetDate("1/jan/2026");
		calendars.Add(calendar);
		_calendar = calendar;
		SetTime(12, 0);
	}

	[TestMethod]
	public void Constructor_FutureReference_PreservesFirstDeparture()
	{
		var reference = DateTimeAt(12, 30);
		var schedule = new VehicleServiceSchedule(reference, Hourly());

		Assert.IsTrue(schedule.NextDeparture.Equals(reference));
	}

	[TestMethod]
	public void Constructor_MissedRecurrences_SelectsOnlyTheNextFutureOccurrence()
	{
		var schedule = new VehicleServiceSchedule(DateTimeAt(8, 0), Hourly());

		var expected = DateTimeAt(13, 0);
		Assert.IsTrue(schedule.NextDeparture.Equals(expected),
			$"Expected {expected.GetDateTimeString()}, got {schedule.NextDeparture.GetDateTimeString()} (current {_calendar.CurrentDateTime.GetDateTimeString()}).");
	}

	[TestMethod]
	public void ConsumeAndAdvance_LateCallbackConsumesOnceThenSkipsFurtherPastOccurrences()
	{
		var schedule = new VehicleServiceSchedule(DateTimeAt(12, 30), Hourly());
		SetTime(15, 15);

		var consumed = schedule.ConsumeAndAdvance();

		var expectedConsumed = DateTimeAt(12, 30);
		var expectedNext = DateTimeAt(15, 30);
		Assert.IsTrue(consumed.Equals(expectedConsumed),
			$"Expected consumed {expectedConsumed.GetDateTimeString()}, got {consumed.GetDateTimeString()}.");
		Assert.IsTrue(schedule.NextDeparture.Equals(expectedNext),
			$"Expected next {expectedNext.GetDateTimeString()}, got {schedule.NextDeparture.GetDateTimeString()} (current {_calendar.CurrentDateTime.GetDateTimeString()}).");
	}

	[TestMethod]
	public void Constructor_NonPositiveRecurrence_FailsFast()
	{
		var recurrence = new RecurringInterval
		{
			Type = IntervalType.Hourly,
			IntervalAmount = 0
		};

		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			new VehicleServiceSchedule(DateTimeAt(12, 30), recurrence));
	}

	private RecurringInterval Hourly()
	{
		return new RecurringInterval
		{
			Type = IntervalType.Hourly,
			IntervalAmount = 1
		};
	}

	private MudDateTime DateTimeAt(int hour, int minute)
	{
		return new MudDateTime(
			_calendar.GetDate("1/jan/2026"),
			MudTime.CreatePrimaryTime(0, minute, hour, _timezone, _clock),
			_timezone);
	}

	private void SetTime(int hour, int minute)
	{
		_clock.SetTime(MudTime.CreatePrimaryTime(0, minute, hour, _timezone, _clock));
	}
}
