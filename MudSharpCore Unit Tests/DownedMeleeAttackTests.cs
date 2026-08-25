#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat.Moves;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DownedMeleeAttackTests
{
	[TestMethod]
	public void ResolveExpression_BlankConfigurationUsesStaggeringFallback()
	{
		Assert.AreEqual(
			"(2*str:1)+(damage/6)+(stun/12)",
			DownedMeleeAttack.ResolveExpression(string.Empty, "(2*str:1)+(damage/6)+(stun/12)"));
	}

	[TestMethod]
	public void ResolveExpression_CustomDownedExpressionWins()
	{
		Assert.AreEqual(
			"damage + stun",
			DownedMeleeAttack.ResolveExpression("damage + stun", "fallback"));
	}
}
