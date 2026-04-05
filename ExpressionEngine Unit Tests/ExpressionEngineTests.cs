using ExpressionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExpressionEngineTests
{
    [TestMethod]
    public void TestVariables()
    {
        Expression expr1 = new("20+(str*3)");

        Assert.AreEqual(50.0, expr1.EvaluateDoubleWith(("str", 10)));
        Assert.AreEqual(56.0, expr1.EvaluateDoubleWith(("str", 12)));

        Expression expr2 = new("max(str,dex)");

        Assert.AreEqual(40.0, expr2.EvaluateDoubleWith(("str", 25.0), ("dex", 40.0)));
        Assert.AreEqual(60.0, expr2.EvaluateDoubleWith(("str", 60.0), ("dex", 40.0)));
    }

    [TestMethod]
    public void TestFunctions()
    {
        Expression expr1 = new("rand(1,10)");
        Counter<double> results = new();
        for (int i = 0; i < 1000; i++)
        {
            double result = expr1.EvaluateDouble();
            Assert.IsTrue(result >= 1 && result <= 10, $"Value from random expression was out of range: {result}");
            results.Increment(result);
        }

        Assert.IsTrue(results.Keys.Count > 1, "There was only one result for rand");

        Expression expr2 = new("3d6+5");
        Counter<int> results2 = new();
        for (int i = 0; i < 1000; i++)
        {
            int result = Convert.ToInt32(expr2.Evaluate());
            Assert.IsTrue(result >= 8 && result <= 23, $"Value from dice expression was out of range: {result}");
            results2.Increment(result);
        }

        Assert.IsTrue(results2.Keys.Count > 1, "There was only one result for dice expression");
    }

    [TestMethod]
    public void TestRandomFunctionOverrides()
    {
        Expression expr1 = new("rand(0.7,1.0)");
        bool foundDoubles = false;
        for (int i = 0; i < 100; i++)
        {
            double result = expr1.EvaluateDouble();
            if (Math.Abs(result % 1) < double.Epsilon)
            {
                continue;
            }

            foundDoubles = true;
        }

        Assert.IsTrue(foundDoubles, "Did not find any doubles in rand(0.7,1.0) after 100 iterations, only integers");

        Expression expr2 = new("rand(1,6)");
        for (int i = 0; i < 1000; i++)
        {
            double result = expr2.EvaluateDouble();
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
        Expression expr = new("enum");
        Assert.AreEqual(9.0, expr.EvaluateDoubleWith(("enum", MudSharp.RPG.Checks.Difficulty.Insane)), "The enum did not convert");
    }

    [TestMethod]
    public void CheckLogic()
    {
        Expression expr = new("if(which='one',1,0)+if(which='two',2,0)+if(which='three',3,0)");
        Assert.AreEqual(1.0, expr.EvaluateDoubleWith(("which", "one")), "The logical expression did not work");
        Assert.AreEqual(3.0, expr.EvaluateDoubleWith(("which", "three")), "The logical expression did not work");
    }
}
