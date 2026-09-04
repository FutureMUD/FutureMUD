using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using System;

namespace FutureMUDLibrary_Unit_Tests;

[TestClass]
public class LiquidFreshnessTests
{
	[TestMethod]
	public void ResolveFreshness_UsesEffectiveResultAndNeverReversesStage()
	{
		var stale = Liquid(2, "stale milk");
		var spoiled = Liquid(3, "spoiled milk");
		var fresh = Liquid(1, "milk");
		fresh.SetupGet(x => x.FreshnessConfiguration).Returns(new LiquidFreshnessConfiguration(
			TimeSpan.FromHours(1), TimeSpan.FromHours(2), stale.Object, spoiled.Object));
		var instance = new LiquidInstance { Liquid = fresh.Object, Amount = 1.0 };
		var origin = instance.LastFreshnessResolution;

		Assert.IsTrue(instance.ResolveFreshness(origin.AddMinutes(90), 1.0));
		Assert.AreEqual(LiquidFreshnessStage.Stale, instance.FreshnessStage);
		Assert.AreSame(stale.Object, instance.Liquid);
		instance.ResolveFreshness(origin.AddMinutes(91), 0.0);
		Assert.AreEqual(TimeSpan.FromMinutes(90), instance.EffectiveAge);
		instance.ResolveFreshness(origin.AddMinutes(30), 1.0);
		Assert.AreEqual(LiquidFreshnessStage.Stale, instance.FreshnessStage);
	}

	[TestMethod]
	public void Merge_WeightsAgeAndPreservesWorstReachedStage()
	{
		var stale = Liquid(2, "stale");
		var spoiled = Liquid(3, "spoiled");
		var fresh = Liquid(1, "fresh");
		fresh.SetupGet(x => x.FreshnessConfiguration).Returns(new LiquidFreshnessConfiguration(
			TimeSpan.FromHours(10), TimeSpan.FromHours(20), stale.Object, spoiled.Object));
		var first = new LiquidInstance { Liquid = fresh.Object, Amount = 9.0 };
		var second = new LiquidInstance { Liquid = fresh.Object, Amount = 1.0 };
		var now = first.LastFreshnessResolution > second.LastFreshnessResolution ? first.LastFreshnessResolution : second.LastFreshnessResolution;
		first.ResolveFreshness(now.AddMinutes(1), 1.0);
		second.ResolveFreshness(now.AddHours(15), 1.0);

		first.MergeOtherIntoSelf(second);

		Assert.AreEqual(10.0, first.Amount);
		Assert.IsTrue(first.EffectiveAge < TimeSpan.FromHours(10));
		Assert.AreEqual(LiquidFreshnessStage.Stale, first.FreshnessStage);
	}

	[TestMethod]
	public void Split_PreservesFreshnessState()
	{
		var liquid = Liquid(1, "juice");
		var stale = Liquid(2, "stale juice");
		var spoiled = Liquid(3, "spoiled juice");
		liquid.SetupGet(x => x.FreshnessConfiguration).Returns(new LiquidFreshnessConfiguration(TimeSpan.FromHours(1), TimeSpan.FromHours(2), stale.Object, spoiled.Object));
		var instance = new LiquidInstance { Liquid = liquid.Object, Amount = 4.0 };
		instance.ResolveFreshness(instance.LastFreshnessResolution.AddMinutes(90), 1.0);

		var split = instance.SplitVolume(1.5);

		Assert.AreEqual(2.5, instance.Amount);
		Assert.AreEqual(1.5, split.Amount);
		Assert.AreEqual(instance.EffectiveAge, split.EffectiveAge);
		Assert.AreEqual(instance.FreshnessStage, split.FreshnessStage);
	}

	private static Mock<ILiquid> Liquid(long id, string name)
	{
		var mock = new Mock<ILiquid>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		return mock;
	}
}
