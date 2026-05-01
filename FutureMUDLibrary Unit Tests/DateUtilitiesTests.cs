using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using System;

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
}
