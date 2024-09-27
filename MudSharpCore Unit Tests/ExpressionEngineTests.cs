using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests
{
	[TestClass]
	public class ExpressionEngineTests
	{
		[TestMethod]
		public void TestVariables()
		{
			var expr1 = new ExpressionEngine.Expression("20+(str*3)");

			Assert.AreEqual(50.0, expr1.EvaluateDoubleWith(("str", 10)));
			Assert.AreEqual(56.0, expr1.EvaluateDoubleWith(("str", 12)));
			
			var expr2 = new ExpressionEngine.Expression("max(str,dex)");

			Assert.AreEqual(40.0, expr2.EvaluateDoubleWith(("str", 25.0),("dex",40.0)));
			Assert.AreEqual(60.0, expr2.EvaluateDoubleWith(("str", 60.0),("dex",40.0)));
		}

		[TestMethod]
		public void TestFunctions()
		{
			var expr1 =  new ExpressionEngine.Expression("rand(1,10)");
			var results = new Counter<double>();
			for (var i = 0; i < 1000; i++)
			{
				var result = expr1.EvaluateDouble();
				Assert.IsTrue(result >= 1 && result <= 10, $"Value from random expression was out of range: {result}");
				results.Increment(result);
			}

			Assert.IsTrue(results.Keys.Count > 1, "There was only one result for rand");

			var expr2 = new  ExpressionEngine.Expression("3d6+5");
			var results2 = new Counter<int>();
			for (var i = 0; i < 1000; i++)
			{
				var result = Convert.ToInt32(expr2.Evaluate());
				Assert.IsTrue(result >= 8 && result <= 23, $"Value from dice expression was out of range: {result}");
				results2.Increment(result);
			}

			Assert.IsTrue(results2.Keys.Count > 1, "There was only one result for dice expression");
		}

		[TestMethod]
		public void TestRandomFunctionOverrides()
		{
			var expr1 = new ExpressionEngine.Expression("rand(0.7,1.0)");
			var foundDoubles = false;
			for (var i = 0; i < 100; i++)
			{
				var result = expr1.EvaluateDouble();
				if (Math.Abs(result % 1) < double.Epsilon)
				{
					continue;
				}

				foundDoubles = true;
			}

			Assert.IsTrue(foundDoubles, "Did not find any doubles in rand(0.7,1.0) after 100 iterations, only integers");

			var expr2 = new ExpressionEngine.Expression("rand(1,6)");
			for (var i = 0; i < 1000; i++)
			{
				var result = expr2.EvaluateDouble();
				if (Math.Abs(result % 1) < double.Epsilon)
				{
					continue;
				}

				Assert.Fail($"Encountered a non-integer value in rand(1,6) -> {result}");
			}
		}

		[TestMethod]
		public void TestEnums()
		{
			var expr = new ExpressionEngine.Expression("enum");
			Assert.AreEqual(9.0, expr.EvaluateDoubleWith(("enum", MudSharp.RPG.Checks.Difficulty.Insane)), "The enum did not convert");
		}

		[TestMethod]
		public void CheckLogic()
		{
			var expr = new ExpressionEngine.Expression("if(which='one',1,0)+if(which='two',2,0)+if(which='three',3,0)");
			Assert.AreEqual(1.0, expr.EvaluateDoubleWith(("which", "one")), "The logical expression did not work");
			Assert.AreEqual(3.0, expr.EvaluateDoubleWith(("which", "three")), "The logical expression did not work");
		}
	}
}
