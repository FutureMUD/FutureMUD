using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class DiceTests
{
	private static void SeedRandom(int seed)
	{
		var newRandom = new Random(seed);
		var fields = typeof(Random).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (var field in fields)
		{
			field.SetValue(Constants.Random, field.GetValue(newRandom));
		}
	}

	[TestMethod]
	public void IsDiceExpression_ValidExpressions_ReturnsTrue()
	{
		Assert.IsTrue(Dice.IsDiceExpression("1d20"));
		Assert.IsTrue(Dice.IsDiceExpression("5"));
		Assert.IsTrue(Dice.IsDiceExpression("5+1d20"));
		Assert.IsTrue(Dice.IsDiceExpression("1d20+5"));
		Assert.IsTrue(Dice.IsDiceExpression("4d6k3"));
		Assert.IsTrue(Dice.IsDiceExpression("4d6k3;5;-2;1d6"));
		Assert.IsTrue(Dice.IsDiceExpression("2d6 k1 m1"));
		Assert.IsTrue(Dice.IsDiceExpression("3d6e6"));
		Assert.IsTrue(Dice.IsDiceExpression("2d8R3"));
	}

	[TestMethod]
	public void IsDiceExpression_InvalidExpressions_ReturnsFalse()
	{
		Assert.IsFalse(Dice.IsDiceExpression("text"));
		Assert.IsFalse(Dice.IsDiceExpression("1/2"));
		Assert.IsFalse(Dice.IsDiceExpression("1d6x"));
		Assert.IsFalse(Dice.IsDiceExpression("1d6k"));
	}

	[TestMethod]
	public void RollExpression_DeterministicConstantsAndSemicolons_ReturnsExpected()
	{
		Assert.AreEqual(5, Dice.Roll("5"));
		Assert.AreEqual(3, Dice.Roll("1d1+1d1+1"));
		Assert.AreEqual(6, Dice.Roll("1d1;2d1;3"));
	}

	[TestMethod]
	public void RollExpression_KeepOptions_ReturnExpectedTotals()
	{
		Assert.AreEqual(2, Dice.Roll("3d1k2"));
		Assert.AreEqual(1, Dice.Roll("3d1l1"));
	}

	[TestMethod]
	public void RollExpression_MinMaxAndSignApplied()
	{
		Assert.AreEqual(5, Dice.Roll("2d1m5"));
		Assert.AreEqual(1, Dice.Roll("2d1M1"));
		Assert.AreEqual(-1, Dice.Roll("-2d1M1"));
	}

	[TestMethod]
	public void RollExpression_CombinesOptions_AppliesInOrder()
	{
		Assert.AreEqual(3, Dice.Roll("3d1k2m3"));
	}

	[TestMethod]
	public void RollExpression_RerollOnce_IncludesDiscardedRolls()
	{
		var (result, rolls) = Dice.Roll("1d1r1", true);
		Assert.AreEqual(1, result);
		CollectionAssert.AreEqual(new List<int> { 1, 1 }, rolls.ToList());

		var (_, emptyRolls) = Dice.Roll("1d1r1", false);
		Assert.IsFalse(emptyRolls.Any());
	}

	[TestMethod]
	public void RollExpression_RerollUntilThresholdTooHigh_ReturnsZeroAndNoRolls()
	{
		var (result, rolls) = Dice.Roll("1d6R6", true);
		Assert.AreEqual(0, result);
		Assert.IsFalse(rolls.Any());
	}

	[TestMethod]
	public void RollExpression_Explode_AddsExtraRoll()
	{
		var seed = FindSeedForExplode(2, 2);
		var expectedRolls = GetExplodingRolls(seed, 1, 2, 2);
		SeedRandom(seed);

		var (result, rolls) = Dice.Roll("1d2e2", true);
		var rollList = rolls.ToList();

		CollectionAssert.AreEqual(expectedRolls, rollList);
		Assert.AreEqual(expectedRolls.Sum(), result);
		Assert.AreEqual(expectedRolls.Count, rollList.Count);
	}

	[TestMethod]
	public void RollNumeric_ReturnsTotalWithBonus()
	{
		Assert.AreEqual(5, Dice.Roll(3, 1, 2));
	}

	private static int FindSeedForExplode(int sides, int threshold, int maxSeed = 10000)
	{
		for (var seed = 1; seed <= maxSeed; seed++)
		{
			var random = new Random(seed);
			var first = random.Next(1, sides + 1);
			var second = random.Next(1, sides + 1);
			if (first >= threshold && second < threshold)
			{
				return seed;
			}
		}

		Assert.Fail($"Unable to find seed for explode tests within 1..{maxSeed}.");
		return -1;
	}

	private static List<int> GetExplodingRolls(int seed, int dice, int sides, int threshold)
	{
		var random = new Random(seed);
		var rolls = new List<int>();
		var remaining = dice;
		while (remaining > 0)
		{
			var roll = random.Next(1, sides + 1);
			rolls.Add(roll);
			remaining--;
			if (roll >= threshold)
			{
				remaining++;
			}
		}

		return rolls;
	}
}
