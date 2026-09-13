#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StrictExpressionTests
{
	[DataTestMethod]
	[DataRow("0", 0.0)]
	[DataRow("max(0, amount - 3)", 4.0)]
	[DataRow("if(amount > 0, amount * 2, 0)", 14.0)]
	public void TryEvaluateDoubleWith_ValidFormula_DistinguishesZeroFromFailure(string formula, double expected)
	{
		IExpression expression = new Expression(formula);
		Assert.IsTrue(expression.TryEvaluateDoubleWith(new Dictionary<string, object> { ["amount"] = 7.0 },
			out var result, out var error), error);
		Assert.AreEqual(expected, result);
		Assert.AreEqual(string.Empty, error);
	}

	[DataTestMethod]
	[DataRow("unknownfunction(1)")]
	[DataRow("if(true, 0, unknownfunction(1))")]
	[DataRow("1 +")]
	[DataRow("0 / 0")]
	[DataRow("1.0 / 0.0")]
	[DataRow("sqrt(-1)")]
	[DataRow("not(1, 2)")]
	[DataRow("'not a number'")]
	public void TryEvaluateDoubleWith_InvalidFormula_ReturnsDiagnosticWithoutGlobalError(string formula)
	{
		var expression = new Expression(formula);
		var events = 0;
		EventHandler<string> handler = (_, _) => events++;
		Expression.ExpressionError += handler;
		try
		{
			Assert.IsFalse(expression.TryEvaluateDoubleWith(new Dictionary<string, object>(),
				out var result, out var error));
			Assert.AreEqual(0.0, result);
			Assert.IsFalse(string.IsNullOrWhiteSpace(error));
			Assert.AreEqual(0, events);
		}
		finally
		{
			Expression.ExpressionError -= handler;
		}
	}

	[TestMethod]
	public void TryEvaluateDoubleWith_MissingInput_DoesNotApplyLegacyImplicitZero()
	{
		var expression = new Expression("first + second");
		var values = new Dictionary<string, object> { ["first"] = 1.0 };
		Assert.IsFalse(expression.TryEvaluateDoubleWith(values, out _, out var error));
		StringAssert.Contains(error, "second");
		Assert.AreEqual(1.0, expression.EvaluateDoubleWith(values));
		values["second"] = 4.0;
		Assert.IsTrue(expression.TryEvaluateDoubleWith(values, out var result, out error), error);
		Assert.AreEqual(5.0, result);
	}

	[TestMethod]
	public void FunctionNames_ReportsFunctionsInUnreachedBranchesAndDiceShorthand()
	{
		var expression = new Expression("if(true, max(1, 2), 2d6)");
		CollectionAssert.AreEquivalent(new[] { "if", "max", "dice" },
			expression.FunctionNames.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
		Assert.IsTrue(Expression.IsSupportedFunction("MAX"));
		Assert.IsFalse(Expression.IsSupportedFunction("unknownfunction"));
	}
}
