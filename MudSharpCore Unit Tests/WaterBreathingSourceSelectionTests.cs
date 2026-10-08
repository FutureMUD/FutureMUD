#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Magic;
using MudSharp.Magic.WaterBreathing;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WaterBreathingSourceSelectionTests
{
	private static readonly MagicSpellLifetimePolicy Policy = new("test.water.source", 600, 36);

	[DataTestMethod]
	[DataRow(1, 0, 3, 1, 3)]
	[DataRow(2, 1, 6, 1, 6)]
	[DataRow(3, 1, 9, 1, 9)]
	[DataRow(4, 2, 12, 2, 12)]
	[DataRow(5, 2, 15, 2, 15)]
	[DataRow(6, 3, 18, 3, 18)]
	[DataRow(7, 3, 21, 3, 21)]
	public void SourceDraw_AllGrades_UsesInclusiveBoundsAndClampsOnlyAfterDraw(int grade, int lower, int upper, int lowUnits, int highUnits)
	{
		var calls = 0;
		var low = WaterBreathingDurationSelection.Select(grade, SpellPower.ExtremelyWeak, Policy,
			(from, to) => { calls++; Assert.AreEqual(lower, from); Assert.AreEqual(upper, to); return from; });
		var high = WaterBreathingDurationSelection.Select(grade, SpellPower.ExtremelyWeak, Policy, (_, to) => to);
		Assert.AreEqual(1, calls); Assert.AreEqual(lower, low.Draw); Assert.AreEqual(lowUnits, low.DurationUnits);
		Assert.AreEqual(highUnits, high.DurationUnits); Assert.AreEqual(TimeSpan.FromSeconds(lowUnits * 600), low.Increment);
		Assert.AreEqual(TimeSpan.FromSeconds(highUnits * 600), high.Increment); Assert.AreEqual(grade, low.Grade);
		Assert.AreEqual(SpellPower.ExtremelyWeak, low.Power, "Source grade must not be inferred from the native power enum.");
	}

	[TestMethod]
	public void SourceDraw_GradeOne_ZeroAndOneClampToOneButBothDraw()
	{
		var calls = 0;
		foreach (var raw in new[] { 0, 1 })
		{
			var value = WaterBreathingDurationSelection.Select(1, SpellPower.Standard, Policy, (_, _) => { calls++; return raw; });
			Assert.AreEqual(1, value.DurationUnits); Assert.AreEqual(raw, value.Draw);
		}
		Assert.AreEqual(2, calls);
	}

	[TestMethod]
	public void Selection_RepeatedReadsAndBindingChecks_DoNotRedrawOrDerivePower()
	{
		var calls = 0;
		var value = WaterBreathingDurationSelection.Select(7, SpellPower.ExtremelyWeak, Policy, (_, _) => { calls++; return 17; });
		for (var repeat = 0; repeat < 20; repeat++)
		{
			Assert.AreEqual(TimeSpan.FromSeconds(10200), value.Increment);
			Assert.IsTrue(value.MatchesBinding(7, SpellPower.ExtremelyWeak, new(Policy.Group, 600, 36)));
			Assert.IsFalse(value.MatchesBinding(6, SpellPower.ExtremelyWeak, Policy));
			Assert.IsFalse(value.MatchesBinding(7, SpellPower.ExtremelyStrong, Policy));
			Assert.IsFalse(value.MatchesBinding(7, SpellPower.ExtremelyWeak, Policy with { MaximumUnits = 35 }));
		}
		Assert.AreEqual(1, calls);
		foreach (var property in typeof(WaterBreathingDurationSelection).GetProperties()) Assert.IsFalse(property.CanWrite);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(8)]
	public void Selection_InvalidGrade_RefusesWithoutDrawing(int grade)
	{
		var calls = 0;
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => WaterBreathingDurationSelection.Select(grade,
			SpellPower.Standard, Policy, (_, _) => { calls++; return 1; }));
		Assert.AreEqual(0, calls);
	}

	[TestMethod]
	public void Selection_InvalidPowerOrPolicy_RefusesWithoutDrawing()
	{
		var calls = 0; int Sample(int _, int __) { calls++; return 1; }
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => WaterBreathingDurationSelection.Select(1, (SpellPower)999, Policy, Sample));
		Assert.ThrowsException<ArgumentException>(() => WaterBreathingDurationSelection.Select(1, SpellPower.Standard, Policy with { Group = "invalid group" }, Sample));
		Assert.ThrowsException<ArgumentException>(() => WaterBreathingDurationSelection.Select(1, SpellPower.Standard, Policy with { UnitSeconds = 0 }, Sample));
		Assert.ThrowsException<ArgumentException>(() => WaterBreathingDurationSelection.Select(1, SpellPower.Standard, Policy with { MaximumUnits = 0 }, Sample));
		Assert.AreEqual(0, calls);
	}

	[DataTestMethod]
	[DataRow(1, -1)]
	[DataRow(1, 4)]
	[DataRow(7, 2)]
	[DataRow(7, 22)]
	public void Selection_OutOfRangeSampler_RefusesWithoutClampingInvalidValues(int grade, int draw)
		=> Assert.ThrowsException<InvalidOperationException>(() => WaterBreathingDurationSelection.Select(grade, SpellPower.Standard, Policy, (_, _) => draw));

	[TestMethod]
	public void FluidScope_OnlyExplicitWaterReferences_AreGrantedWithoutGasOrAliasFallback()
	{
		var water = Liquid(1); var saltWater = Liquid(2); var acid = Liquid(3);
		acid.Setup(x => x.CountsAs(water.Object)).Returns(true);
		var gas = new Mock<IGas>(); gas.SetupGet(x => x.Id).Returns(1);
		var scope = new WaterBreathingFluidScope([water.Object, saltWater.Object]);
		Assert.IsTrue(scope.Allows(water.Object)); Assert.IsTrue(scope.Allows(saltWater.Object));
		Assert.IsFalse(scope.Allows(acid.Object)); Assert.IsFalse(scope.Allows(gas.Object)); Assert.IsFalse(scope.Allows(null));
		acid.Verify(x => x.CountsAs(It.IsAny<IFluid>()), Times.Never);
	}

	[TestMethod]
	public void FluidScope_InputMutationOrCatalogueReplacement_DoesNotBroadenCapturedGrant()
	{
		var water = Liquid(1); var replacement = Liquid(1); var acid = Liquid(3);
		var configured = new List<ILiquid> { water.Object };
		var scope = new WaterBreathingFluidScope(configured); configured[0] = acid.Object;
		Assert.IsTrue(scope.Allows(water.Object)); Assert.IsFalse(scope.Allows(acid.Object)); Assert.IsFalse(scope.Allows(replacement.Object));
		Assert.IsTrue(scope.MatchesConfiguredLiquids([water.Object])); Assert.IsFalse(scope.MatchesConfiguredLiquids([replacement.Object]));
		Assert.ThrowsException<NotSupportedException>(() => ((IList<long>)scope.LiquidIds)[0] = 3);
		water.SetupGet(x => x.Id).Returns(99);
		Assert.IsFalse(scope.Allows(water.Object)); Assert.IsFalse(scope.MatchesConfiguredLiquids([water.Object])); Assert.AreEqual(1L, scope.LiquidIds[0]);
	}

	[TestMethod]
	public void FluidScope_EmptyUnresolvedOrDuplicateIds_Refuses()
	{
		Assert.ThrowsException<ArgumentException>(() => new WaterBreathingFluidScope([]));
		Assert.ThrowsException<ArgumentException>(() => new WaterBreathingFluidScope([null!]));
		Assert.ThrowsException<ArgumentException>(() => new WaterBreathingFluidScope([Liquid(0).Object]));
		Assert.ThrowsException<ArgumentException>(() => new WaterBreathingFluidScope([Liquid(1).Object, Liquid(1).Object]));
	}

	private static Mock<ILiquid> Liquid(long id)
	{
		var liquid = new Mock<ILiquid>(); liquid.SetupGet(x => x.Id).Returns(id); return liquid;
	}
}
