#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MudTimeFactoryTests
{
	[TestMethod]
	public void FromPrimaryTime_AppliesTimezoneOffset()
	{
		var (clock, _, cst) = CreateClock();

		var time = MudTime.FromPrimaryTime(0, 0, 12, cst, clock);

		Assert.AreEqual(6, time.Hours);
		Assert.AreEqual(0, time.Minutes);
		Assert.AreEqual(0, time.Seconds);
		Assert.AreEqual(0, time.DaysOffsetFromDatum);
		Assert.AreEqual(cst, time.Timezone);
		Assert.IsFalse(time.IsPrimaryTime);
	}

	[TestMethod]
	public void FromLocalTime_PreservesWallClockTime()
	{
		var (clock, _, cst) = CreateClock();

		var time = MudTime.FromLocalTime(0, 0, 6, cst, clock);

		Assert.AreEqual(6, time.Hours);
		Assert.AreEqual(0, time.Minutes);
		Assert.AreEqual(0, time.Seconds);
		Assert.AreEqual(cst, time.Timezone);
		Assert.IsFalse(time.IsPrimaryTime);
	}

	[TestMethod]
	public void ParseLocalTime_UsesNamedTimezoneAndMeridianAsWallTime()
	{
		var (clock, _, cst) = CreateClock();

		var time = MudTime.ParseLocalTime("CST 3:15:30 p.m", clock);

		Assert.AreEqual(cst, time.Timezone);
		Assert.AreEqual(15, time.Hours);
		Assert.AreEqual(15, time.Minutes);
		Assert.AreEqual(30, time.Seconds);
	}

	[TestMethod]
	public void TryParseLocalTime_RejectsUnknownPeriodCleanly()
	{
		var (clock, _, _) = CreateClock();

		var result = MudTime.TryParseLocalTime("UTC 3:15:30xm", clock, out var time, out var error);

		Assert.IsFalse(result);
		Assert.IsNull(time);
		StringAssert.Contains(error, "hour period");
	}

	[TestMethod]
	public void CopyOf_CopiesAndCanResetDatumOffset()
	{
		var (clock, utc, _) = CreateClock();
		var time = MudTime.FromLocalTime(10, 20, 3, utc, clock, 2);

		var copy = MudTime.CopyOf(time);
		var reset = MudTime.CopyOf(time, true);

		Assert.AreEqual(2, copy.DaysOffsetFromDatum);
		Assert.AreEqual(0, reset.DaysOffsetFromDatum);
		Assert.AreEqual(time.Hours, reset.Hours);
		Assert.AreEqual(time.Minutes, reset.Minutes);
		Assert.AreEqual(time.Seconds, reset.Seconds);
		Assert.IsFalse(copy.IsPrimaryTime);
		Assert.IsFalse(reset.IsPrimaryTime);
	}

	[TestMethod]
	public void CreatePrimaryTime_AdvancingPastMidnightNotifiesClock()
	{
		var (clock, utc, _) = CreateClock(out var clockMock);
		var time = MudTime.CreatePrimaryTime(0, 0, 23, utc, clock);

		time.AddHours(2);

		Assert.AreEqual(1, time.Hours);
		clockMock.Verify(x => x.AdvanceDays(1), Times.Once);
		clockMock.VerifySet(x => x.Changed = true, Times.AtLeastOnce);
	}

	[TestMethod]
	public void Factories_RejectOutOfRangeComponents()
	{
		var (clock, utc, _) = CreateClock();

		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			MudTime.FromLocalTime(60, 0, 0, utc, clock));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			MudTime.FromLocalTime(0, 60, 0, utc, clock));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			MudTime.FromLocalTime(0, 0, 24, utc, clock));
	}

	private static (IClock Clock, IMudTimeZone Utc, IMudTimeZone Cst) CreateClock()
	{
		return CreateClock(out _);
	}

	private static (IClock Clock, IMudTimeZone Utc, IMudTimeZone Cst) CreateClock(out Mock<IClock> clockMock)
	{
		var timezones = new List<IMudTimeZone>();
		clockMock = new Mock<IClock>();
		var clock = clockMock.Object;
		var utc = CreateTimezone(1, "UTC", "Universal Time", 0, 0, clock);
		var cst = CreateTimezone(2, "CST", "Central Standard Time", -6, 0, clock);
		timezones.Add(utc);
		timezones.Add(cst);

		clockMock.SetupGet(x => x.SecondsPerMinute).Returns(60);
		clockMock.SetupGet(x => x.MinutesPerHour).Returns(60);
		clockMock.SetupGet(x => x.HoursPerDay).Returns(24);
		clockMock.SetupGet(x => x.NumberOfHourIntervals).Returns(2);
		clockMock.SetupGet(x => x.NoZeroHour).Returns(true);
		clockMock.SetupGet(x => x.HourIntervalNames).Returns(["a.m", "p.m"]);
		clockMock.SetupGet(x => x.Timezones).Returns(timezones);
		clockMock.SetupGet(x => x.PrimaryTimezone).Returns(utc);
		clockMock.SetupProperty(x => x.Changed);

		return (clock, utc, cst);
	}

	private static IMudTimeZone CreateTimezone(long id, string alias, string name, int hours, int minutes, IClock clock)
	{
		var mock = new Mock<IMudTimeZone>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(alias);
		mock.SetupGet(x => x.FrameworkItemType).Returns("Timezone");
		mock.SetupGet(x => x.Names).Returns(new[] { alias, name });
		mock.SetupGet(x => x.Alias).Returns(alias);
		mock.SetupGet(x => x.Description).Returns(name);
		mock.SetupGet(x => x.OffsetHours).Returns(hours);
		mock.SetupGet(x => x.OffsetMinutes).Returns(minutes);
		mock.SetupGet(x => x.Clock).Returns(clock);
		return mock.Object;
	}
}
