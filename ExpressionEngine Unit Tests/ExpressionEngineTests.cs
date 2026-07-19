#nullable enable

using ExpressionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Checks;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExpressionEngineTests
{
	[TestMethod]
	public void EvaluateDoubleWith_OverridesVariablesForEachEvaluation()
	{
		var arithmetic = new Expression("20+(str*3)");

		Assert.AreEqual(50.0, arithmetic.EvaluateDoubleWith(("str", 10)));
		Assert.AreEqual(56.0, arithmetic.EvaluateDoubleWith(("str", 12)));

		var maximum = new Expression("max(str,dex)");

		Assert.AreEqual(40.0, maximum.EvaluateDoubleWith(("str", 25.0), ("dex", 40.0)));
		Assert.AreEqual(60.0, maximum.EvaluateDoubleWith(("str", 60.0), ("dex", 40.0)));
	}

	[DataTestMethod]
	[DataRow("rand(1,10)", 1.0, 10.0, true)]
	[DataRow("dice(3,6)", 3.0, 18.0, true)]
	[DataRow("3d6+5", 8.0, 23.0, true)]
	[DataRow("drand(0.7,1.0)", 0.7, 1.0, false)]
	public void RandomFunctions_StayWithinTheirDocumentedBounds(
		string expression,
		double minimum,
		double maximum,
		bool requiresInteger)
	{
		var subject = new Expression(expression);

		for (var i = 0; i < 64; i++)
		{
			var result = subject.EvaluateDouble();
			Assert.IsTrue(result >= minimum && result <= maximum,
				$"{expression} returned {result}, outside {minimum}-{maximum}.");
			if (requiresInteger)
			{
				Assert.AreEqual(Math.Truncate(result), result, $"{expression} should return whole numbers.");
			}
		}
	}

	[DataTestMethod]
	[DataRow("not(0)", 1.0)]
	[DataRow("not(1)", 0.0)]
	[DataRow("not(-3.5)", 0.0)]
	[DataRow("NOT(0)", 1.0)]
	public void NotFunction_UsesNumericTruthSemantics(string expression, double expected)
	{
		Assert.AreEqual(expected, new Expression(expression).EvaluateDouble());
	}

	[DataTestMethod]
	[DataRow("rand(1)")]
	[DataRow("drand(1)")]
	[DataRow("not(1,2)")]
	[DataRow("dice(10001,6)")]
	[DataRow("dice(1,0)")]
	[DataRow("dice(1,100001)")]
	public void InvalidCustomFunctionArguments_ReturnZeroInsteadOfThrowing(string expression)
	{
		Assert.AreEqual(0.0, new Expression(expression).EvaluateDouble());
	}

	[TestMethod]
	public void Evaluate_EnumParameter_UsesUnderlyingNumericValue()
	{
		var expression = new Expression("enum")
		{
			Parameters = { ["enum"] = Difficulty.Insane }
		};

		Assert.AreEqual(9.0, expression.EvaluateDouble());
	}

	[TestMethod]
	public void EvaluateWith_StringComparisons_AreCaseInsensitive()
	{
		var expression = new Expression("if(which='one',1,0)+if(which='two',2,0)+if(which='three',3,0)");

		Assert.AreEqual(1.0, expression.EvaluateDoubleWith(("which", "ONE")));
		Assert.AreEqual(3.0, expression.EvaluateDoubleWith(("which", "three")));
	}

	[TestMethod]
	public void ParameterNames_ReportsReferencedVariables()
	{
		var expression = new Expression("strength + (dexterity * 2) + strength");

		CollectionAssert.AreEquivalent(
			new[] { "strength", "dexterity" },
			expression.ParameterNames.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
	}

	[TestMethod]
	public void InvalidSyntax_ExposesParserErrorWithoutThrowing()
	{
		var expression = new Expression("1 +");

		Assert.IsTrue(expression.HasErrors());
		Assert.IsFalse(string.IsNullOrWhiteSpace(expression.Error));
		Assert.AreEqual(0.0, expression.EvaluateDouble());
	}

	[TestMethod]
	public void MissingFunction_RaisesExpressionErrorAndReturnsZero()
	{
		string? capturedError = null;
		EventHandler<string> handler = (_, message) => capturedError = message;
		Expression.ExpressionError += handler;

		try
		{
			Assert.AreEqual(0.0, new Expression("missingfunction(1)").EvaluateDouble());
			Assert.IsNotNull(capturedError);
			StringAssert.Contains(capturedError, "missingfunction");
		}
		finally
		{
			Expression.ExpressionError -= handler;
		}
	}

	[TestMethod]
	public void EvaluateDecimalWith_ConvertsNumericResults()
	{
		var expression = new Expression("value / 4");

		Assert.AreEqual(2.5m, expression.EvaluateDecimalWith(("value", 10m)));
	}
}
