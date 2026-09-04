#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Analytics;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

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

	[TestMethod]
	public void EconomyCommand_NoArguments_ShowsHelpWithoutStartingAnalytics()
	{
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var method = typeof(EconomyModule).GetMethod("EconomyAnalytics",
			BindingFlags.Static | BindingFlags.NonPublic);

		Assert.IsNotNull(method);
		method.Invoke(null, [actor.Object, "economy"]);

		output.Verify(x => x.Send(It.Is<string>(text => text.Contains("economy money")), true, false), Times.Once);
		actor.VerifyGet(x => x.Gameworld, Times.Never);
		var help = EconomyModule.Instance.Commands.TCommands["economy"].HelpInfo;
		Assert.IsNotNull(help);
		Assert.AreEqual(AutoHelp.HelpArgOrNoArg, help.AutoHelpSetting);
	}

	[TestMethod]
	public void AnalyticsAggregateQueries_AreRelationallyTranslatable()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		var trendSql = EconomyAnalyticsService.BuildTrendQuery(context.EconomySnapshotEntries
			.Where(x => x.Metric == (int)EconomyHoldingMetric.BroadMoneySupply), 2).ToQueryString();
		var volumeSql = EconomyAnalyticsService.BuildVolumeAggregateQuery(context.EconomicActivityRecords)
			.ToQueryString();

		StringAssert.Contains(trendSql, "ORDER BY");
		StringAssert.Contains(trendSql, "LIMIT");
		StringAssert.Contains(volumeSql, "GROUP BY");
	}
}
