#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RecurringIntervalTests
{
	[TestMethod]
	public void TryParse_OrdinalDayForms()
	{
		Assert.IsTrue(RecurringInterval.TryParse("every month on day 15", out var dayInterval));
		Assert.AreEqual(IntervalType.OrdinalDayOfMonth, dayInterval.Type);
		Assert.AreEqual(1, dayInterval.IntervalAmount);
		Assert.AreEqual(15, dayInterval.Modifier);

		Assert.IsTrue(RecurringInterval.TryParse("every 2 months on the 15th", out var ordinalInterval));
		Assert.AreEqual(IntervalType.OrdinalDayOfMonth, ordinalInterval.Type);
		Assert.AreEqual(2, ordinalInterval.IntervalAmount);
		Assert.AreEqual(15, ordinalInterval.Modifier);

		Assert.IsTrue(RecurringInterval.TryParse("every month on last day", out var lastInterval));
		Assert.AreEqual(IntervalType.OrdinalDayOfMonth, lastInterval.Type);
		Assert.AreEqual(-1, lastInterval.Modifier);
	}

	[TestMethod]
	public void TryParse_OrdinalWeekdayUsesCalendarWeekdayNames()
	{
		var calendar = CreateCalendar("Marketday", "Moonday", "Starday", "Fireday", "Waterday", "Restday");

		Assert.IsTrue(RecurringInterval.TryParse("every 3 months on the 12th or last Marketday", calendar, out var interval, out var error), error);

		Assert.AreEqual(IntervalType.OrdinalWeekdayOfMonth, interval.Type);
		Assert.AreEqual(3, interval.IntervalAmount);
		Assert.AreEqual(12, interval.Modifier);
		Assert.AreEqual(0, interval.SecondaryModifier);
		Assert.AreEqual(OrdinalFallbackMode.OrLast, interval.OrdinalFallbackMode);
	}

	[TestMethod]
	public void TryParse_CustomWeekdayNameRequiresCalendar()
	{
		Assert.IsFalse(RecurringInterval.TryParse("every month on the 5th Marketday", out var interval));
		Assert.IsNull(interval);
	}

	[TestMethod]
	public void ToString_RoundTripsOrdinalWeekdayWithoutCalendar()
	{
		var calendar = CreateCalendar("Marketday", "Moonday", "Starday");
		Assert.IsTrue(RecurringInterval.TryParse("every month on the 5th or last Marketday", calendar, out var interval, out var error), error);

		var roundTrip = RecurringInterval.Parse(interval.ToString());

		Assert.AreEqual(interval.Type, roundTrip.Type);
		Assert.AreEqual(interval.IntervalAmount, roundTrip.IntervalAmount);
		Assert.AreEqual(interval.Modifier, roundTrip.Modifier);
		Assert.AreEqual(interval.SecondaryModifier, roundTrip.SecondaryModifier);
		Assert.AreEqual(interval.OrdinalFallbackMode, roundTrip.OrdinalFallbackMode);
	}

	[TestMethod]
	public void TryParse_LastWeekdayUsesFallbackModeExactOnly()
	{
		var calendar = CreateCalendar("Marketday", "Moonday", "Starday");

		Assert.IsTrue(RecurringInterval.TryParse("every month on last Moonday", calendar, out var interval, out var error), error);

		Assert.AreEqual(IntervalType.OrdinalWeekdayOfMonth, interval.Type);
		Assert.AreEqual(-1, interval.Modifier);
		Assert.AreEqual(1, interval.SecondaryModifier);
		Assert.AreEqual(OrdinalFallbackMode.ExactOnly, interval.OrdinalFallbackMode);
	}

	[TestMethod]
	public void Describe_UsesCalendarWeekdayNames()
	{
		var calendar = CreateCalendar("Marketday", "Moonday", "Starday");
		Assert.IsTrue(RecurringInterval.TryParse("every month on the 5th or last Marketday", calendar, out var interval, out var error), error);

		Assert.AreEqual("every month on the 5th or last Marketday", interval.Describe(calendar));
	}

	private static ICalendar CreateCalendar(params string[] weekdays)
	{
		var mock = new Mock<ICalendar>();
		mock.SetupGet(x => x.Weekdays).Returns(new List<string>(weekdays));
		mock.SetupGet(x => x.Name).Returns("Calendar");
		mock.SetupGet(x => x.ShortName).Returns("Calendar");
		mock.SetupGet(x => x.FrameworkItemType).Returns("Calendar");
		return mock.Object;
	}
}
