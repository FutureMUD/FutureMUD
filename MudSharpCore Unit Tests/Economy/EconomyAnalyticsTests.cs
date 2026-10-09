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
using MudSharp.Economy.Currency;
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
		Assert.AreEqual("false", DefaultStaticSettings.DefaultStaticConfigurations[EconomyAnalyticsService.CashGiftsEnabledConfiguration]);
		Assert.AreEqual("1440", DefaultStaticSettings.DefaultStaticConfigurations["EconomyAnalyticsSnapshotIntervalMinutes"]);
		Assert.AreEqual("true", DefaultStaticSettings.DefaultStaticConfigurations["EconomyAnalyticsRolloverSnapshotsEnabled"]);
		Assert.AreEqual("0", DefaultStaticSettings.DefaultStaticConfigurations["EconomyAnalyticsGlobalDisplayCurrencyId"]);
		Assert.IsFalse(EconomyAnalyticsMath.IsValidSnapshotInterval(TimeSpan.FromMinutes(59)));
		Assert.IsTrue(EconomyAnalyticsMath.IsValidSnapshotInterval(TimeSpan.FromHours(1)));
	}

	[TestMethod]
	public void GlobalDisplayCurrency_NoConfiguration_UsesFirstCurrency()
	{
		var firstCurrency = new Mock<ICurrency>();
		firstCurrency.SetupGet(x => x.Id).Returns(1L);
		firstCurrency.SetupGet(x => x.Name).Returns("First Currency");
		firstCurrency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(1.0M);
		var secondCurrency = new Mock<ICurrency>();
		secondCurrency.SetupGet(x => x.Id).Returns(2L);
		secondCurrency.SetupGet(x => x.Name).Returns("Second Currency");
		secondCurrency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(2.0M);
		var currencies = new All<ICurrency>();
		currencies.Add(firstCurrency.Object);
		currencies.Add(secondCurrency.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Currencies).Returns(currencies);
		gameworld.Setup(x => x.GetStaticLong(EconomyAnalyticsService.GlobalDisplayCurrencyConfiguration))
			.Returns(0L);

		var service = new EconomyAnalyticsService(gameworld.Object);

		Assert.AreSame(firstCurrency.Object, service.GlobalDisplayCurrency);
	}

	[TestMethod]
	public void GlobalDisplayCurrency_FirstCurrencyCannotConvert_UsesNextUsableCurrency()
	{
		var firstCurrency = new Mock<ICurrency>();
		firstCurrency.SetupGet(x => x.Id).Returns(1L);
		firstCurrency.SetupGet(x => x.Name).Returns("Unconvertible Currency");
		firstCurrency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(0.0M);
		var secondCurrency = new Mock<ICurrency>();
		secondCurrency.SetupGet(x => x.Id).Returns(2L);
		secondCurrency.SetupGet(x => x.Name).Returns("Usable Currency");
		secondCurrency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(2.0M);
		var currencies = new All<ICurrency>();
		currencies.Add(firstCurrency.Object);
		currencies.Add(secondCurrency.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Currencies).Returns(currencies);
		gameworld.Setup(x => x.GetStaticLong(EconomyAnalyticsService.GlobalDisplayCurrencyConfiguration))
			.Returns(0L);

		var service = new EconomyAnalyticsService(gameworld.Object);

		Assert.AreSame(secondCurrency.Object, service.GlobalDisplayCurrency);
	}

	[TestMethod]
	public void DescribeGlobalValue_ConvertsThenUsesCurrencyDescription()
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(4.0M);
		currency.Setup(x => x.Describe(25.0M, CurrencyDescriptionPatternType.ShortDecimal))
			.Returns("twenty-five crowns");

		var description = EconomyModule.DescribeGlobalValue(currency.Object, 100.0M);

		Assert.AreEqual("twenty-five crowns", description);
		currency.Verify(x => x.Describe(25.0M, CurrencyDescriptionPatternType.ShortDecimal), Times.Once);
	}

	[TestMethod]
	public void TrySetGlobalDisplayCurrency_ZeroConversion_IsRejectedWithoutPersistence()
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Name).Returns("Unconvertible Currency");
		currency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(0.0M);
		var gameworld = new Mock<IFuturemud>(MockBehavior.Strict);
		var service = new EconomyAnalyticsService(gameworld.Object);

		var result = service.TrySetGlobalDisplayCurrency(currency.Object, out var error);

		Assert.IsFalse(result);
		StringAssert.Contains(error, "conversion factor is zero");
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

	[TestMethod]
	public void RecordActivity_CashGiftDisabled_RepeatedTransfersNeverEnterTheSaveQueue()
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		world.Setup(x => x.GetStaticBool(EconomyAnalyticsService.CashGiftsEnabledConfiguration)).Returns(false);
		var service = new EconomyAnalyticsService(world.Object);
		for (var i = 0; i < 10000; i++)
		{
			service.RecordActivity(new EconomicActivityEvent(EconomicActivityType.CashGift,
				EconomicVolumeClassification.GeneralTransfer, 1, 1m));
		}
		Assert.IsNull(service.ActivityCoverageStartUtc);
		world.VerifyGet(x => x.SaveManager, Times.Never);
		world.VerifyGet(x => x.Currencies, Times.Never);
	}

	[DataTestMethod]
	[DataRow(EconomicActivityType.CashGift, true)]
	[DataRow(EconomicActivityType.BankTransfer, false)]
	public void RecordActivity_OptedInGiftOrOtherTransfer_PreservesTheLedger(EconomicActivityType type, bool giftsEnabled)
	{
		var currency = Mock.Of<ICurrency>(x => x.Id == 1 && x.BaseCurrencyToGlobalBaseCurrencyConversion == 2m);
		var currencies = new All<ICurrency>();
		currencies.Add(currency);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.Currencies).Returns(currencies);
		world.Setup(x => x.GetStaticBool(EconomyAnalyticsService.CashGiftsEnabledConfiguration)).Returns(giftsEnabled);
		EconomicActivityRecordItem? pending = null;
		world.Setup(x => x.SaveManager.AddInitialisation(It.IsAny<MudSharp.Framework.Save.ILateInitialisingItem>()))
			.Callback<MudSharp.Framework.Save.ILateInitialisingItem>(x => pending = (EconomicActivityRecordItem)x);
		var service = new EconomyAnalyticsService(world.Object);
		service.RecordActivity(new EconomicActivityEvent(type, EconomicVolumeClassification.GeneralTransfer, 1, 3m));
		Assert.IsNotNull(pending);
		Assert.AreEqual(3m, pending.Record.Amount);
		Assert.AreEqual(6m, pending.Record.GlobalBaseValue);
		Assert.IsNotNull(service.ActivityCoverageStartUtc);
	}
}
