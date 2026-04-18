using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CollectionExtensionsTests
{
    [TestMethod]
    public void TestSum2Int()
    {
        int[] data = new[] { 1, 2, 3, 4 };
        (int s1, int s2) = data.Sum2(x => x, x => x * 2);
        Assert.AreEqual(10, s1, "Sum1 incorrect");
        Assert.AreEqual(20, s2, "Sum2 incorrect");
    }

    [TestMethod]
    public void TestSum3Int()
    {
        int[] data = new[] { 1, 2, 3, 4 };
        (int s1, int s2, int s3) = data.Sum3(x => x, x => x * 2, x => x * 3);
        Assert.AreEqual(10, s1, "Sum1 incorrect");
        Assert.AreEqual(20, s2, "Sum2 incorrect");
        Assert.AreEqual(30, s3, "Sum3 incorrect");
    }

    [TestMethod]
    public void TestSum2Double()
    {
        double[] data = new[] { 1.0, 2.0, 3.0 };
        (double s1, double s2) = data.Sum2(x => x, x => x * 0.5);
        Assert.AreEqual(6.0, s1, 1e-6, "Sum1 incorrect");
        Assert.AreEqual(3.0, s2, 1e-6, "Sum2 incorrect");
    }

    [TestMethod]
    public void TestSum3Double()
    {
        double[] data = new[] { 1.0, 2.0, 3.0 };
        (double s1, double s2, double s3) = data.Sum3(x => x, x => x * 2.0, x => x * 0.5);
        Assert.AreEqual(6.0, s1, 1e-6, "Sum1 incorrect");
        Assert.AreEqual(12.0, s2, 1e-6, "Sum2 incorrect");
        Assert.AreEqual(3.0, s3, 1e-6, "Sum3 incorrect");
    }

    [TestMethod]
    public void TestSum2Decimal()
    {
        decimal[] data = new[] { 1.5m, 2.5m, 3.0m };
        (decimal s1, decimal s2) = data.Sum2(x => x, x => x * 2m);
        Assert.AreEqual(7.0m, s1, "Sum1 incorrect");
        Assert.AreEqual(14.0m, s2, "Sum2 incorrect");
    }

    [TestMethod]
    public void TestSum3Long()
    {
        long[] data = new long[] { 1, 2, 3 };
        (long s1, long s2, long s3) = data.Sum3(x => x, x => x * 2, x => x * 3);
        Assert.AreEqual(6L, s1, "Sum1 incorrect");
        Assert.AreEqual(12L, s2, "Sum2 incorrect");
        Assert.AreEqual(18L, s3, "Sum3 incorrect");
    }

    [TestMethod]
    public void TestMinMaxInt()
    {
        int[] data = new[] { -5, 3, 10, -2 };
        (int min, int max) = data.MinMax();
        Assert.AreEqual(-5, min, "Min incorrect");
        Assert.AreEqual(10, max, "Max incorrect");
    }

    [TestMethod]
    public void TestMinMaxIntPredicate()
    {
        int[] data = new[] { -5, 3, 10, -2 };
        (int min, int max) = data.MinMax(x => x > 0);
        Assert.AreEqual(3, min, "Min incorrect");
        Assert.AreEqual(10, max, "Max incorrect");
    }

    [TestMethod]
    public void TestMinMaxDouble()
    {
        double[] data = new[] { -1.5, 0.5, 2.5 };
        (double min, double max) = data.MinMax();
        Assert.AreEqual(-1.5, min, 1e-6, "Min incorrect");
        Assert.AreEqual(2.5, max, 1e-6, "Max incorrect");
    }

    [TestMethod]
    public void TestMinMaxDoublePredicate()
    {
        double[] data = new[] { -1.5, 0.5, 2.5 };
        (double min, double max) = data.MinMax(x => x >= 0);
        Assert.AreEqual(0.5, min, 1e-6, "Min incorrect");
        Assert.AreEqual(2.5, max, 1e-6, "Max incorrect");
    }

    [TestMethod]
    public void TestMinMaxDecimal()
    {
        decimal[] data = new[] { -1.5m, 0m, 2.5m };
        (decimal min, decimal max) = data.MinMax();
        Assert.AreEqual(-1.5m, min, "Min incorrect");
        Assert.AreEqual(2.5m, max, "Max incorrect");
    }

    [TestMethod]
    public void TestMinMaxDecimalPredicate()
    {
        decimal[] data = new[] { -1.5m, 0m, 2.5m };
        (decimal min, decimal max) = data.MinMax(x => x >= 0m);
        Assert.AreEqual(0m, min, "Min incorrect");
        Assert.AreEqual(2.5m, max, "Max incorrect");
    }

    [TestMethod]
    public void TestFirstMaxValueType()
    {
        List<(int id, int score)> data = new()
        {
            (1, -5),
            (2, 0),
            (3, 3),
            (4, 3)
        };
        (int id, int score) result = data.FirstMax(x => x.score);
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.id, "Incorrect element returned");
    }

    [TestMethod]
    public void TestFirstMaxReferenceType()
    {
        List<Tuple<int, int>> data = new()
        {
            Tuple.Create(1, -5),
            Tuple.Create(2, 0),
            Tuple.Create(3, 3),
            Tuple.Create(4, 3),
        };
        Tuple<int, int> result = data.FirstMax(x => x.Item2);
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Item1, "Incorrect element returned");
    }

    [TestMethod]
    public void TestFirstMinValueType()
    {
        List<(int id, int score)> data = new()
        {
            (1, -5),
            (2, 2),
            (3, -3),
            (4, 2)
        };
        (int id, int score) result = data.FirstMin(x => x.score);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.id, "Incorrect element returned");
    }


    [TestMethod]
    public void TestFirstMinReferenceType()
    {
        List<Tuple<int, int>> data = new()
        {
                        Tuple.Create(1, -5),
                        Tuple.Create(2, 0),
                        Tuple.Create(3, 3),
                        Tuple.Create(4, 3),
                };
        Tuple<int, int> result = data.FirstMin(x => x.Item2);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Item1, "Incorrect element returned");
    }

    [TestMethod]
    public void TestFirstMaxDefaultValueBehaviour()
    {
        Assert.AreEqual(0, Array.Empty<int>().FirstMax(x => x, 3));
    }

    [TestMethod]
    public void TestFirstMinDefaultValueBehaviour()
    {
        Assert.AreEqual(0, Array.Empty<int>().FirstMin(x => x, 0));
    }

    [TestMethod]
    public void TestFindSequenceConsecutive()
    {
        int[] data = new[] { 1, 2, 2, 2, 3 };
        List<IEnumerable<int>> results = data.FindSequenceConsecutive(x => x == 2, 2).ToList();
        Assert.AreEqual(2, results.Count, "Unexpected sequence count");
        foreach (IEnumerable<int> seq in results)
        {
            CollectionAssert.AreEqual(new[] { 2, 2 }, seq.ToList(), "Sequence mismatch");
        }
    }

    [TestMethod]
    public void TestFindSequenceConsecutiveNoMatch()
    {
        int[] data = new[] { 1, 3, 1, 3 };
        IEnumerable<IEnumerable<int>> results = data.FindSequenceConsecutive(x => x == 2, 2);
        Assert.AreEqual(0, results.Count());
    }

    [TestMethod]
    public void TestSwapByIndex()
    {
        List<int> list = new()
        { 1, 2, 3 };
        list.SwapByIndex(0, 2);
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, list);
    }

    [TestMethod]
    public void TestSwapByItem()
    {
        List<int> list = new()
        { 1, 2, 3 };
        list.Swap(1, 3);
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, list);
    }
}
