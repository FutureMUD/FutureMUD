#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic.Environment;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalMagicOptionsTests
{
	[DataTestMethod]
	[DataRow("EnvironmentalMagicActiveSeconds", "0")]
	[DataRow("EnvironmentalMagicAuditSeconds", "30")]
	[DataRow("EnvironmentalMagicMaximumCellVisits", "0")]
	[DataRow("EnvironmentalMagicMaximumOutputWork", "7")]
	[DataRow("EnvironmentalMagicBudgetMilliseconds", "NaN")]
	[DataRow("EnvironmentalMagicBudgetMilliseconds", "Infinity")]
	public void InvalidDeploymentEdit_PreservesThePreviouslyAcceptedBudgets(string name, string value)
	{
		var original = new EnvironmentalMagicOptions();
		Assert.ThrowsException<ArgumentException>(() => original.WithSetting(name, value));
		Assert.AreEqual(new EnvironmentalMagicOptions(), original);
	}

	[TestMethod]
	public void ValidDeploymentEdit_UpdatesOneBudgetAndRetainsOtherLimits()
	{
		var original = new EnvironmentalMagicOptions();
		var changed = original.WithSetting("EnvironmentalMagicBudgetMilliseconds", "7.5");
		Assert.AreEqual(7.5, changed.SoftBudgetMilliseconds);
		Assert.AreEqual(original.MaximumCellVisits, changed.MaximumCellVisits);
		Assert.AreEqual(original.ActiveCadenceSeconds, changed.ActiveCadenceSeconds);
		Assert.AreEqual(original.ReconciliationSeconds, changed.ReconciliationSeconds);
		Assert.AreEqual(5.0, original.SoftBudgetMilliseconds);
	}
}
