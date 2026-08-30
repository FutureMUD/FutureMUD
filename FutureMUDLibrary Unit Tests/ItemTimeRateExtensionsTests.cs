using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemTimeRateExtensionsTests
{
	[TestMethod]
	public void NearestProviderWinsAcrossNestedContainers()
	{
		var outer = ItemWithModifier(0.1);
		var inner = ItemWithModifier(0.5, outer.Object);
		var contents = ContainedItem(inner.Object);

		Assert.AreEqual(0.5, contents.Object.TimeRateMultiplier(ItemTimeRateType.Morph));
	}

	[TestMethod]
	public void ResolverWalksThroughContainersWithoutProviders()
	{
		var refrigerator = ItemWithModifier(0.1);
		var ordinaryContainer = ContainedItem(refrigerator.Object);
		var contents = ContainedItem(ordinaryContainer.Object);

		Assert.AreEqual(0.1, contents.Object.TimeRateMultiplier(ItemTimeRateType.PreparedFoodFreshness));
	}

	[TestMethod]
	public void UnsupportedRateTypesContinueOutwardAndOtherwiseRemainNormal()
	{
		var outerModifier = new Mock<IItemTimeRateModifier>();
		outerModifier
			.Setup(x => x.RateMultiplierFor(ItemTimeRateType.SurfaceLiquidDrying))
			.Returns(4.0);
		var outer = ItemWithComponents(outerModifier.Object);

		var innerModifier = new Mock<IItemTimeRateModifier>();
		innerModifier
			.Setup(x => x.RateMultiplierFor(ItemTimeRateType.SurfaceLiquidDrying))
			.Returns((double?)null);
		var inner = ItemWithComponents(innerModifier.Object);
		inner.SetupGet(x => x.ContainedIn).Returns(outer.Object);

		var contents = ContainedItem(inner.Object);

		Assert.AreEqual(4.0, contents.Object.TimeRateMultiplier(ItemTimeRateType.SurfaceLiquidDrying));
		Assert.AreEqual(1.0, contents.Object.TimeRateMultiplier(ItemTimeRateType.BiologicalDecay));
	}

	[TestMethod]
	public void NegativeProviderRatesAreClampedToPaused()
	{
		var container = ItemWithModifier(-1.0);
		var contents = ContainedItem(container.Object);

		Assert.AreEqual(0.0, contents.Object.TimeRateMultiplier(ItemTimeRateType.Morph));
	}

	[TestMethod]
	public void RefrigerationRateSelectsAllFourOperatingStates()
	{
		Assert.AreEqual(0.1, ItemTimeRateMath.RefrigerationRate(true, false, 0.1, 0.5, 0.75, 1.0));
		Assert.AreEqual(0.5, ItemTimeRateMath.RefrigerationRate(true, true, 0.1, 0.5, 0.75, 1.0));
		Assert.AreEqual(0.75, ItemTimeRateMath.RefrigerationRate(false, false, 0.1, 0.5, 0.75, 1.0));
		Assert.AreEqual(1.0, ItemTimeRateMath.RefrigerationRate(false, true, 0.1, 0.5, 0.75, 1.0));
	}

	[TestMethod]
	public void EffectiveAndWallDurationConversionsPreserveProgressAndPauseAtZero()
	{
		var wall = TimeSpan.FromMinutes(20.0);
		var effective = ItemTimeRateMath.EffectiveElapsed(wall, 0.25);

		Assert.AreEqual(TimeSpan.FromMinutes(5.0), effective);
		Assert.AreEqual(wall, ItemTimeRateMath.WallDuration(effective, 0.25));
		Assert.IsNull(ItemTimeRateMath.WallDuration(effective, 0.0));
	}

	[TestMethod]
	public void PreservedSensitiveMorphConvertsScheduledWallTimeBackToEffectiveTime()
	{
		var scheduledWallRemaining = TimeSpan.FromMinutes(20.0);

		Assert.AreEqual(TimeSpan.FromMinutes(5.0), ItemTimeRateMath.PreservedMorphRemaining(
			scheduledWallRemaining, true, 0.25));
		Assert.AreEqual(scheduledWallRemaining, ItemTimeRateMath.PreservedMorphRemaining(
			scheduledWallRemaining, false, 0.25));
		Assert.AreEqual(TimeSpan.Zero, ItemTimeRateMath.PreservedMorphRemaining(
			TimeSpan.FromSeconds(-1.0), true, 0.25));
	}

	[TestMethod]
	public void BiologicalDecayIntervalsRetainTheRateActiveDuringEachInterval()
	{
		var refrigeratedInterval = ItemTimeRateMath.EffectiveElapsed(TimeSpan.FromSeconds(59.0), 0.1);
		var ambientInterval = ItemTimeRateMath.EffectiveElapsed(TimeSpan.FromSeconds(1.0), 1.0);

		Assert.AreEqual(TimeSpan.FromSeconds(6.9), refrigeratedInterval + ambientInterval);
	}

	[TestMethod]
	public void PowerBankEnergyMathAppliesEfficiencyAndSimultaneousOutput()
	{
		var charged = PowerBankEnergyMath.ResolveWattHours(0.0, 40.0, 10.0, 0.9, 0.0,
			TimeSpan.FromHours(1.0));
		var simultaneous = PowerBankEnergyMath.ResolveWattHours(10.0, 40.0, 10.0, 0.9, 4.0,
			TimeSpan.FromHours(1.0));

		Assert.AreEqual(9.0, charged, 0.0001);
		Assert.AreEqual(15.0, simultaneous, 0.0001);
		Assert.AreEqual(0.0, PowerBankEnergyMath.ResolveWattHours(1.0, 40.0, 0.0, 1.0, 10.0,
			TimeSpan.FromHours(1.0)));
		Assert.AreEqual(40.0, PowerBankEnergyMath.ResolveWattHours(39.0, 40.0, 10.0, 1.0, 0.0,
			TimeSpan.FromHours(1.0)));
	}

	private static Mock<IGameItem> ItemWithModifier(double rate, IGameItem? containedIn = null)
	{
		var modifier = new Mock<IItemTimeRateModifier>();
		modifier.Setup(x => x.RateMultiplierFor(It.IsAny<ItemTimeRateType>())).Returns(rate);
		var item = ItemWithComponents(modifier.Object);
		if (containedIn is not null)
		{
			item.SetupGet(x => x.ContainedIn).Returns(containedIn);
		}
		return item;
	}

	private static Mock<IGameItem> ContainedItem(IGameItem containedIn)
	{
		var item = ItemWithComponents();
		item.SetupGet(x => x.ContainedIn).Returns(containedIn);
		return item;
	}

	private static Mock<IGameItem> ItemWithComponents(params IGameItemComponent[] components)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Components).Returns(components);
		return item;
	}
}
