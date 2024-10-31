using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiceTests
{
	[TestMethod]
	public void TestIsDiceExpression()
	{
		Assert.IsTrue(Dice.IsDiceExpression("1d20"));
		Assert.IsTrue(Dice.IsDiceExpression("5"));
		Assert.IsTrue(Dice.IsDiceExpression("5+1d20"));
		Assert.IsTrue(Dice.IsDiceExpression("1d20+5"));
		Assert.IsTrue(Dice.IsDiceExpression("4d6k3"));
		Assert.IsTrue(Dice.IsDiceExpression("4d6k3;5;-2;1d6"));
		Assert.IsFalse(Dice.IsDiceExpression("text"));
		Assert.IsFalse(Dice.IsDiceExpression("1/2"));

		for (var i = 0; i < 1000; i++)
		{
			var result = Dice.Roll("1d20");
			Assert.IsTrue(result > 0);
			Assert.IsTrue(result < 21);
		}

		for (var i = 0; i < 200; i++)
		{
			var result = Dice.Roll("5");
			Assert.IsTrue(result == 5);
		}

		for (var i = 0; i < 10000; i++)
		{
			var result = Dice.Roll("1d10+1d10+1d10+5");
			Assert.IsTrue(result > 7);
			Assert.IsTrue(result < 36);
		}
	}
}