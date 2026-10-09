#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace FutureMUDLibrary_Unit_Tests;

[TestClass]
public class DiceIntegerSecurityTests
{
	[DataTestMethod]
	[DataRow("999999999999999999999999d6")]
	[DataRow("1d999999999999999999999999")]
	[DataRow("1d6 e999999999999999999999999")]
	[DataRow("1d6 m999999999999999999999999")]
	[DataRow("999999999999999999999999")]
	public void TryValidateDiceExpression_IntegerOverflow_RejectsAndRollReturnsZero(string expression)
	{
		Assert.IsFalse(Dice.TryValidateDiceExpression(expression, out var error));
		Assert.IsFalse(string.IsNullOrWhiteSpace(error));
		Assert.AreEqual(0, Dice.Roll(expression));
	}
}
