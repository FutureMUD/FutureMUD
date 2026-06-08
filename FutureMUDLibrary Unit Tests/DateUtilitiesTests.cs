#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using System;
using System.Globalization;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DateUtilitiesTests
{
	[TestMethod]
	public void Describe_NegativeWholeHours_DoesNotTreatSignAsListItem()
	{
		Assert.AreEqual("negative 8 hours", TimeSpan.FromHours(-8).Describe());
		Assert.AreEqual("negative 8 hours", MudTimeSpan.FromHours(-8).Describe());
	}

	[TestMethod]
	public void Describe_NegativeMultiPartDuration_IncludesSameComponentsAsPositiveDuration()
	{
		Assert.AreEqual("1 day, 1 hour, and 1 minute", new TimeSpan(1, 1, 1, 0).Describe());
		Assert.AreEqual("negative 1 day, 1 hour, and 1 minute", new TimeSpan(-1, -1, -1, 0).Describe());
	}

	[TestMethod]
	public void Describe_NegativeSubSecondDuration_PreservesSign()
	{
		Assert.AreEqual("negative less than a second", TimeSpan.FromMilliseconds(-500).Describe());
		Assert.AreEqual("negative 500 milliseconds", TimeSpan.FromMilliseconds(-500).DescribePrecise());
		Assert.AreEqual("-500ms", TimeSpan.FromMilliseconds(-500).DescribePreciseBrief());
		Assert.AreEqual("negative less than a second", new MudTimeSpan(TimeSpan.FromMilliseconds(-500).Ticks).Describe());
	}

	[TestMethod]
	public void Describe_TwoYearDuration_UsesPluralYear()
	{
		Assert.AreEqual("2 years", TimeSpan.FromDays(730).Describe());
		Assert.AreEqual("2 years", TimeSpan.FromDays(730).DescribePrecise());
		Assert.AreEqual("2y", TimeSpan.FromDays(730).DescribePreciseBrief());
	}

	[TestMethod]
	public void TryParseDateTimeOrRelative_DstInvalidLocalTime_ReturnsFalse()
	{
		TimeZoneInfo? timezone = ResolveNewYorkTimeZone();
		if (timezone is null)
		{
			Assert.Inconclusive("No New York time zone was available on this test host.");
		}

		Mock<IAccount> account = new();
		account.SetupGet(x => x.Culture).Returns(CultureInfo.InvariantCulture);
		account.SetupGet(x => x.TimeZone).Returns(timezone!);

		bool result = DateUtilities.TryParseDateTimeOrRelative("2026-03-08 02:30", account.Object, false, out DateTime parsed);

		Assert.IsFalse(result);
		Assert.AreEqual(default, parsed);
	}

	private static TimeZoneInfo? ResolveNewYorkTimeZone()
	{
		foreach (string id in new[] { "America/New_York", "Eastern Standard Time" })
		{
			try
			{
				return TimeZoneInfo.FindSystemTimeZoneById(id);
			}
			catch (TimeZoneNotFoundException)
			{
			}
			catch (InvalidTimeZoneException)
			{
			}
		}

		return null;
	}
}
