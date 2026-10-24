#nullable enable
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PsionicRejuvenationPolicyTests
{
	[TestMethod]
	public void RecoveryIsLimitedBySkillStunBudgetAndMissingCondition()
	{
		var limited = PsionicRejuvenationPolicy.Plan(50, 100, 100, 10, 0, 100, true);
		Assert.AreEqual(50 / 1.5, limited.Change, 0.000001);
		Assert.AreEqual(50, limited.StunCost, 0.000001);
		var nearlyFull = PsionicRejuvenationPolicy.Plan(50, 100, 100, 98, 0, 100, true);
		Assert.AreEqual(2, nearlyFull.Change);
		Assert.AreEqual(3, nearlyFull.StunCost);
	}
	[TestMethod]
	public void ExchangeLeavesOneStunAndFailureCannotCrossTheConditionFloor()
	{
		var recovery = PsionicRejuvenationPolicy.Plan(100, 10, 100, 1, 1, 100, true);
		Assert.AreEqual(9, recovery.Change);
		Assert.AreEqual(9, recovery.StunCost);
		var failure = PsionicRejuvenationPolicy.Plan(100, 100, 100, 3, 1, 100, false);
		Assert.AreEqual(-2, failure.Change);
		Assert.AreEqual(2, failure.StunCost);
		var ordinaryFailure = PsionicRejuvenationPolicy.Plan(50, 100, 100, 90, 0, 100, false);
		Assert.AreEqual(-50 / 1.5 * 2 / 3, ordinaryFailure.Change, 0.000001);
		Assert.AreEqual(50 * 2.0 / 3, ordinaryFailure.StunCost, 0.000001);
	}
	[TestMethod]
	public void FailedRecoveryStillCostsStunWhenAlreadyHungryOrThirsty()
	{
		var hunger = PsionicRejuvenationPolicy.Plan(50, 100, 100, -2, -6, 24, false);
		Assert.AreEqual(-4, hunger.Change);
		Assert.AreEqual(6, hunger.StunCost);
		var thirst = PsionicRejuvenationPolicy.Plan(50, 100, 100, -1, -4, 8, false);
		Assert.AreEqual(-3, thirst.Change);
		Assert.AreEqual(4.5, thirst.StunCost);
	}

	[TestMethod]
	public void InvalidOrExhaustedExchangesProduceNoRecoveryOrCost()
	{
		foreach (var skill in new[] { 0.0, -20, double.NaN, double.PositiveInfinity })
			Assert.AreEqual(default, PsionicRejuvenationPolicy.Plan(skill, 100, 100, 0, 0, 100, true));
		Assert.AreEqual(default, PsionicRejuvenationPolicy.Plan(100, 1, 100, 0, 0, 100, true));
		Assert.AreEqual(default, PsionicRejuvenationPolicy.Plan(100, 100, 100, 100, 0, 100, true));
		Assert.AreEqual(default, PsionicRejuvenationPolicy.Plan(100, 100, 100, -1, 0, 100, true));
	}
}
