#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RecurringIntervalNumericValidationTests
{
	[DataTestMethod]
	[DataRow("every 2147483648 days")]
	[DataRow("every 2147483648 months on day 15")]
	[DataRow("every 2147483648 months on last day")]
	[DataRow("every 2147483648 months on last weekday 0")]
	[DataRow("every 2147483648 months on the 5th weekday 0")]
	[DataRow("every 2147483648 months on the 5th or last weekday 0")]
	[DataRow("every month on day 2147483648")]
	[DataRow("every month on the 2147483648th")]
	[DataRow("every month on the 2147483648th weekday 0")]
	[DataRow("every month on the 2147483648th or last weekday 0")]
	[DataRow("every days +2147483648")]
	[DataRow("every days -2147483649")]
	[DataRow("every month on last weekday 2147483648")]
	public void TryParse_Overflow_ReturnsFalseWithErrorAndNoInterval(string text)
	{
		AssertRejected(text);
	}

	[DataTestMethod]
	[DataRow("every {0} days")]
	[DataRow("every {0} months on day 15")]
	[DataRow("every {0} months on last weekday 0")]
	[DataRow("every {0} months on the 5th weekday 0")]
	[DataRow("every month on day {0}")]
	[DataRow("every month on the {0}th")]
	[DataRow("every month on the {0}th weekday 0")]
	[DataRow("every month on the {0}th or last weekday 0")]
	public void TryParse_ThousandsOfDigits_ReturnsFalseWithoutThrowing(string format)
	{
		AssertRejected(string.Format(format, new string('9', 4096)));
	}

	[TestMethod]
	public void TryParse_ThousandsOfLeadingZeros_PreservesRepresentableAmount()
	{
		Assert.IsTrue(RecurringInterval.TryParse("every " + new string('0', 4096) + "1 days", null,
			out var interval, out var error), error);
		Assert.AreEqual(IntervalType.Daily, interval.Type);
		Assert.AreEqual(1, interval.IntervalAmount);
	}

	[DataTestMethod]
	[DataRow("every 2147483647 days", IntervalType.Daily, int.MaxValue, 0, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every 2147483647 months on day 15", IntervalType.OrdinalDayOfMonth, int.MaxValue, 15, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every 2147483647 months on last day", IntervalType.OrdinalDayOfMonth, int.MaxValue, -1, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every 2147483647 months on last weekday 0", IntervalType.OrdinalWeekdayOfMonth, int.MaxValue, -1, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every 2147483647 months on the 5th weekday 0", IntervalType.OrdinalWeekdayOfMonth, int.MaxValue, 5, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every 2147483647 months on the 5th or last weekday 0", IntervalType.OrdinalWeekdayOfMonth, int.MaxValue, 5, OrdinalFallbackMode.OrLast)]
	[DataRow("every month on day 2147483647", IntervalType.OrdinalDayOfMonth, 1, int.MaxValue, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every month on the 2147483647th", IntervalType.OrdinalDayOfMonth, 1, int.MaxValue, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every month on the 2147483647th weekday 0", IntervalType.OrdinalWeekdayOfMonth, 1, int.MaxValue, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every month on the 2147483647th or last weekday 0", IntervalType.OrdinalWeekdayOfMonth, 1, int.MaxValue, OrdinalFallbackMode.OrLast)]
	[DataRow("every days +2147483647", IntervalType.Daily, 1, int.MaxValue, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every days -2147483648", IntervalType.Daily, 1, int.MinValue, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every 000000000000000000000001 days", IntervalType.Daily, 1, 0, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every month on day 000000000000000000000001", IntervalType.OrdinalDayOfMonth, 1, 1, OrdinalFallbackMode.ExactOnly)]
	[DataRow("every month on the 000000000000000000000001st weekday 0", IntervalType.OrdinalWeekdayOfMonth, 1, 1, OrdinalFallbackMode.ExactOnly)]
	public void TryParse_RepresentableBoundary_PreservesInterval(string text, IntervalType type, int amount,
		int modifier, OrdinalFallbackMode fallback)
	{
		Assert.IsTrue(RecurringInterval.TryParse(text, null, out var interval, out var error), error);
		Assert.AreEqual(string.Empty, error);
		Assert.AreEqual(type, interval.Type);
		Assert.AreEqual(amount, interval.IntervalAmount);
		Assert.AreEqual(modifier, interval.Modifier);
		Assert.AreEqual(0, interval.SecondaryModifier);
		Assert.AreEqual(fallback, interval.OrdinalFallbackMode);

		if (type is IntervalType.OrdinalDayOfMonth or IntervalType.OrdinalWeekdayOfMonth)
		{
			var roundTrip = RecurringInterval.Parse(interval.ToString());
			Assert.AreEqual(interval.ToString(), roundTrip.ToString());
		}
	}

	[DataTestMethod]
	[DataRow("every 0 days")]
	[DataRow("every 0 months on day 15")]
	[DataRow("every 0 months on last weekday 0")]
	[DataRow("every 0 months on the 5th weekday 0")]
	[DataRow("every month on day 0")]
	[DataRow("every month on the 0th weekday 0")]
	[DataRow("every month on last weekday -1")]
	[DataRow("every month on last weekday 7")]
	public void TryParse_ExistingInvalidRange_RemainsRejected(string text)
	{
		AssertRejected(text);
	}

	[DataTestMethod]
	[DataRow("every 2147483648 days")]
	[DataRow("every month on day 2147483648")]
	[DataRow("every month on the 2147483648th weekday 0")]
	public void TryParse_Overloads_RejectOverflow(string text)
	{
		Assert.IsFalse(RecurringInterval.TryParse(text, out var interval));
		Assert.IsNull(interval);
		Assert.IsFalse(RecurringInterval.TryParse(text, null, out interval));
		Assert.IsNull(interval);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => RecurringInterval.Parse(text));
	}

	private static void AssertRejected(string text)
	{
		Assert.IsFalse(RecurringInterval.TryParse(text, null, out var interval, out var error), text);
		Assert.IsNull(interval);
		Assert.IsFalse(string.IsNullOrWhiteSpace(error), text);
	}
}
