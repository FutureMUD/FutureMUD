using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RandomUtilitiesTests
{
    [TestInitialize]
    public void Init()
    {
        SeedRandom(1);
    }

    private static void SeedRandom(int seed)
    {
        Random newRandom = new(seed);
        FieldInfo[] fields = typeof(Random).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            field.SetValue(Constants.Random, field.GetValue(newRandom));
        }
    }

    private static List<T> ShuffleExpected<T>(IEnumerable<T> source, int seed)
    {
        T[] array = source.ToArray();
        Random rnd = new(seed);
        for (int i = array.Length; i > 1; i--)
        {
            int j = rnd.Next(i);
            (array[j], array[i - 1]) = (array[i - 1], array[j]);
        }
        return array.ToList();
    }

    private static List<T> PickRandomExpected<T>(IList<T> source, int picks, int seed)
    {
        Random rnd = new(seed);
        HashSet<T> items = new();
        while (picks > 0)
        {
            if (items.Add(source[rnd.Next(source.Count)]))
            {
                picks--;
            }
        }
        return items.ToList();
    }

    private static List<T> PickUpToRandomExpected<T>(IList<T> source, int picks, int seed)
    {
        if (picks > source.Count)
        {
            return source.ToList();
        }
        return ShuffleExpected(source, seed).Take(picks).ToList();
    }

    private static List<T> TakeRandomExpected<T>(IEnumerable<T> source, int picks, Func<T, double> weightSelector, int seed)
    {
        if (picks < 1)
        {
            return new List<T>();
        }

        Random rnd = new(seed);
        List<(T Value, double Weight)> choices = source.Select(x => (Value: x, Weight: weightSelector(x))).ToList();
        if (choices.Count <= picks)
        {
            return choices.Select(x => x.Value).ToList();
        }

        double sum = choices.Sum(x => x.Weight);
        int len = choices.Count;
        List<T> results = new(picks);
        while (picks > 0)
        {
            double roll = rnd.NextDouble() * sum;
            for (int i = 0; i < len; i++)
            {
                if (choices[i].Weight <= 0)
                {
                    continue;
                }

                if ((roll -= choices[i].Weight) <= 0.0)
                {
                    (T value, double weight) = choices[i];
                    results.Add(value);
                    choices.RemoveAt(i);
                    len--;
                    sum -= weight;
                    break;
                }
            }
            picks--;
        }

        return results;
    }

    [TestMethod]
    public void Shuffle_RearrangesElementsButPreservesContents()
    {
        List<int> data = Enumerable.Range(1, 5).ToList();
        List<int> shuffled = data.Shuffle().ToList();
        CollectionAssert.AreEquivalent(data, shuffled);
    }

    [TestMethod]
    public void PickRandom_SelectsUniqueItemsAndValidatesCounts()
    {
        List<int> data = Enumerable.Range(1, 5).ToList();
        List<int> result = data.PickRandom(3).ToList();
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(3, result.Distinct().Count());
        List<int> expected = PickRandomExpected(data, 3, 1).OrderBy(x => x).ToList();
        List<int> actual = result.OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(expected, actual);

        Assert.ThrowsException<ArgumentException>(() => data.PickRandom(6));
        Assert.ThrowsException<ArgumentException>(() => data.PickRandom(0));
    }

    [TestMethod]
    public void PickUpToRandom_SelectsUpToCountAndValidates()
    {
        List<int> data = Enumerable.Range(1, 5).ToList();
        List<int> result = data.PickUpToRandom(3).ToList();
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(3, result.Distinct().Count());

        List<int> all = data.PickUpToRandom(10).ToList();
        CollectionAssert.AreEquivalent(data, all);

        Assert.ThrowsException<ArgumentException>(() => data.PickUpToRandom(0));
    }

    [TestMethod]
    public void TakeRandom_HonoursWeightsAndPickCounts()
    {
        (string, double)[] data = new[] { ("A", 1.0), ("B", 2.0), ("C", 3.0) };
        List<(string, double)> result = data.TakeRandom(2, x => x.Item2).ToList();
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result.Distinct().Count());
        List<(string, double)> expected = TakeRandomExpected(data, 2, x => x.Item2, 1);
        CollectionAssert.AreEqual(expected, result);

        IEnumerable<(string, double)> empty = data.TakeRandom(0, x => x.Item2);
        Assert.IsFalse(empty.Any());
    }

    [TestMethod]
    public void GetWeightedRandom_TupleInt_BiasesSelection()
    {
        (string, int)[] options = new[] { ("A", 1), ("B", 2), ("C", 3) };
        Dictionary<string, int> counts = new()
        { { "A", 0 }, { "B", 0 }, { "C", 0 } };
        for (int i = 0; i < 6000; i++)
        {
            string pick = options.GetWeightedRandom();
            counts[pick]++;
        }
        Assert.IsTrue(counts["C"] > counts["B"]);
        Assert.IsTrue(counts["B"] > counts["A"]);
    }

    [TestMethod]
    public void GetWeightedRandom_TupleDouble_BiasesSelection()
    {
        (string, double)[] options = new[] { ("A", 1.0), ("B", 2.0), ("C", 3.0) };
        Dictionary<string, int> counts = new()
        { { "A", 0 }, { "B", 0 }, { "C", 0 } };
        for (int i = 0; i < 6000; i++)
        {
            string pick = options.GetWeightedRandom();
            counts[pick]++;
        }
        Assert.IsTrue(counts["C"] > counts["B"]);
        Assert.IsTrue(counts["B"] > counts["A"]);
    }

    [TestMethod]
    public void GetWeightedRandom_EvaluatorDouble_BiasesSelection()
    {
        double[] options = new[] { 1.0, 2.0, 3.0 };
        Dictionary<double, int> counts = new()
        { { 1.0, 0 }, { 2.0, 0 }, { 3.0, 0 } };
        for (int i = 0; i < 6000; i++)
        {
            double pick = options.GetWeightedRandom(x => x);
            counts[pick]++;
        }
        Assert.IsTrue(counts[3.0] > counts[2.0]);
        Assert.IsTrue(counts[2.0] > counts[1.0]);
    }

    [TestMethod]
    public void GetWeightedRandom_EvaluatorInt_BiasesSelection()
    {
        int[] options = new[] { 1, 3, 5 };
        Dictionary<int, int> counts = new()
        { { 1, 0 }, { 3, 0 }, { 5, 0 } };
        for (int i = 0; i < 6000; i++)
        {
            int pick = options.GetWeightedRandom(x => x);
            counts[pick]++;
        }
        Assert.IsTrue(counts[5] > counts[3]);
        Assert.IsTrue(counts[3] > counts[1]);
    }

    [TestMethod]
    public void GetRandomElement_ReturnsItemFromSource()
    {
        List<int> data = new()
        { 1, 2, 3 };
        int value = data.GetRandomElement();
        Assert.IsTrue(data.Contains(value));
    }

    [TestMethod]
    public void RandomNormal_ReturnsValueWithinBounds()
    {
        const double mean = 10.0;
        const double stdev = 2.0;
        double value = RandomUtilities.RandomNormal(mean, stdev);
        Assert.IsTrue(value >= mean - 6 * stdev && value <= mean + 6 * stdev);
    }

    [TestMethod]
    public void RandomNormalWithSkew_ReturnsValueWithinBounds()
    {
        const double mean = 10.0;
        const double stdev = 2.0;
        const double skew = 1.5;
        double value = RandomUtilities.RandomNormal(mean, stdev, skew);
        Assert.IsTrue(value >= mean - 6 * stdev && value <= mean + 6 * stdev);
    }

    [TestMethod]
    public void RandomNormalOverRange_ProducesValueWithinBounds()
    {
        const double min = 10.0;
        const double max = 20.0;
        double value = RandomUtilities.RandomNormalOverRange(min, max);
        double stdev = (max - min) / 8.0;
        double mean = (max + min) / 2.0;
        Assert.IsTrue(value >= mean - 6 * stdev && value <= mean + 6 * stdev);
    }
}

