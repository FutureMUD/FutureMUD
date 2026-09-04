#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Economy;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
public class EconomyAnalyticsTests
{
	[TestMethod]
	public void Gini_EqualWealth_IsZero()
	{
		Assert.AreEqual(0.0M, EconomyAnalyticsMath.Gini([10.0M, 10.0M, 10.0M, 10.0M]));
	}

	[TestMethod]
	public void Gini_ConcentratedWealth_IsExpectedValue()
	{
		Assert.AreEqual(0.75M, EconomyAnalyticsMath.Gini([0.0M, 0.0M, 0.0M, 100.0M]));
	}

	[TestMethod]
	public void NextPeriodicDue_UsesLastSuccessfulPeriodicSnapshotOnly()
	{
		var lastPeriodic = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);
		Assert.AreEqual(lastPeriodic.AddHours(6),
			EconomyAnalyticsMath.NextPeriodicDue(lastPeriodic, TimeSpan.FromHours(6)));
		Assert.AreEqual(lastPeriodic.AddDays(2),
			EconomyAnalyticsMath.NextPeriodicDue(lastPeriodic, TimeSpan.FromDays(2)));
	}

	[TestMethod]
	public void SnapshotDefaultsAndMinimumInterval_AreStable()
	{
		Assert.AreEqual("true", DefaultStaticSettings.DefaultStaticConfigurations["EconomyAnalyticsSnapshotsEnabled"]);
		Assert.AreEqual("1440", DefaultStaticSettings.DefaultStaticConfigurations["EconomyAnalyticsSnapshotIntervalMinutes"]);
		Assert.AreEqual("true", DefaultStaticSettings.DefaultStaticConfigurations["EconomyAnalyticsRolloverSnapshotsEnabled"]);
		Assert.IsFalse(EconomyAnalyticsMath.IsValidSnapshotInterval(TimeSpan.FromMinutes(59)));
		Assert.IsTrue(EconomyAnalyticsMath.IsValidSnapshotInterval(TimeSpan.FromHours(1)));
	}
}
