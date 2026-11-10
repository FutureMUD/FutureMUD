#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Discord;
using MudSharp.Economy;
using MudSharp.Economy.Currency;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordRemoteReportTests
{
	[TestMethod]
	public void TryParseAccountId_NumericAccountArgumentResolvesAsId()
	{
		Assert.IsTrue(DiscordRemoteService.TryParseAccountId("12345", out var id));
		Assert.AreEqual(12345L, id);
		Assert.IsFalse(DiscordRemoteService.TryParseAccountId("player-name", out _));
		Assert.IsFalse(DiscordRemoteService.TryParseAccountId("0", out _));
		Assert.IsFalse(DiscordRemoteService.TryParseAccountId("-12", out _));
	}

	[TestMethod]
	public void FormatEconomyTrendValue_CountMetricIgnoresDisplayCurrencyConversion()
	{
		var displayCurrency = new Mock<ICurrency>();
		displayCurrency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(2.0m);
		displayCurrency.SetupGet(x => x.Name).Returns("Test Currency");

		var result = DiscordRemoteService.FormatEconomyTrendValue(
			EconomyHoldingMetric.PcOpenJobPostings, 10.0m, displayCurrency.Object);

		Assert.AreEqual("10", result);
	}
}
