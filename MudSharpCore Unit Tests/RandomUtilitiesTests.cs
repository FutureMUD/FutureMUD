using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RandomUtilitiesTests
{
    [TestInitialize]
    public void Init() => SeedRandom(1);

    private static void SeedRandom(int seed)
    {
        var newRandom = new Random(seed);
        var fields = typeof(Random).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            field.SetValue(Constants.Random, field.GetValue(newRandom));
        }
    }

    private static List<T> ShuffleExpected<T>(IEnumerable<T> source, int seed)
    {
        var array = source.ToArray();
        var rnd = new Random(seed);
        for (var i = array.Length; i > 1; i--)
        {
            var j = rnd.Next(i);
            (array[j], array[i - 1]) = (array[i - 1], array[j]);
        }
        return array.ToList();
    }

    private static List<T> PickRandomExpected<T>(IList<T> source, int picks, int seed)
    {
        var rnd = new Random(seed);
        var items = new HashSet<T>();
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

        var rnd = new Random(seed);
        var choices = source.Select(x => (Value: x, Weight: weightSelector(x))).ToList();
        if (choices.Count <= picks)
        {
            return choices.Select(x => x.Value).ToList();
        }

        var sum = choices.Sum(x => x.Weight);
        var len = choices.Count;
        var results = new List<T>(picks);
        while (picks > 0)
        {
            var roll = rnd.NextDouble() * sum;
            for (var i = 0; i < len; i++)
            {
                if (choices[i].Weight <= 0)
                {
                    continue;
                }

                if ((roll -= choices[i].Weight) <= 0.0)
                {
                    var (value, weight) = choices[i];
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
        var data = Enumerable.Range(1, 5).ToList();
        var shuffled = data.Shuffle().ToList();
        CollectionAssert.AreEquivalent(data, shuffled);
        var expected = ShuffleExpected(data, 1);
        CollectionAssert.AreEqual(expected, shuffled);
    }

    [TestMethod]
    public void PickRandom_SelectsUniqueItemsAndValidatesCounts()
    {
        var data = Enumerable.Range(1, 5).ToList();
        var result = data.PickRandom(3).ToList();
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(3, result.Distinct().Count());
        var expected = PickRandomExpected(data, 3, 1).OrderBy(x => x).ToList();
        var actual = result.OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(expected, actual);

        Assert.ThrowsException<ArgumentException>(() => data.PickRandom(6));
        Assert.ThrowsException<ArgumentException>(() => data.PickRandom(0));
    }

    [TestMethod]
    public void PickUpToRandom_SelectsUpToCountAndValidates()
    {
        var data = Enumerable.Range(1, 5).ToList();
        var result = data.PickUpToRandom(3).ToList();
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(3, result.Distinct().Count());
        CollectionAssert.AreEqual(PickUpToRandomExpected(data, 3, 1), result);

        var all = data.PickUpToRandom(10).ToList();
        CollectionAssert.AreEquivalent(data, all);

        Assert.ThrowsException<ArgumentException>(() => data.PickUpToRandom(0));
    }

    [TestMethod]
    public void TakeRandom_HonoursWeightsAndPickCounts()
    {
        var data = new[] { ("A", 1.0), ("B", 2.0), ("C", 3.0) };
        var result = data.TakeRandom(2, x => x.Item2).ToList();
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result.Distinct().Count());
        var expected = TakeRandomExpected(data, 2, x => x.Item2, 1);
        CollectionAssert.AreEqual(expected, result);

        var empty = data.TakeRandom(0, x => x.Item2);
        Assert.IsFalse(empty.Any());
    }

    [TestMethod]
    public void GetWeightedRandom_TupleInt_BiasesSelection()
    {
        var options = new[] { ("A", 1), ("B", 2), ("C", 3) };
        var counts = new Dictionary<string, int> { { "A", 0 }, { "B", 0 }, { "C", 0 } };
        for (var i = 0; i < 6000; i++)
        {
            var pick = options.GetWeightedRandom();
            counts[pick]++;
        }
        Assert.IsTrue(counts["C"] > counts["B"]);
        Assert.IsTrue(counts["B"] > counts["A"]);
    }

    [TestMethod]
    public void GetWeightedRandom_TupleDouble_BiasesSelection()
    {
        var options = new[] { ("A", 1.0), ("B", 2.0), ("C", 3.0) };
        var counts = new Dictionary<string, int> { { "A", 0 }, { "B", 0 }, { "C", 0 } };
        for (var i = 0; i < 6000; i++)
        {
            var pick = options.GetWeightedRandom();
            counts[pick]++;
        }
        Assert.IsTrue(counts["C"] > counts["B"]);
        Assert.IsTrue(counts["B"] > counts["A"]);
    }

    [TestMethod]
    public void GetWeightedRandom_EvaluatorDouble_BiasesSelection()
    {
        var options = new[] { 1.0, 2.0, 3.0 };
        var counts = new Dictionary<double, int> { { 1.0, 0 }, { 2.0, 0 }, { 3.0, 0 } };
        for (var i = 0; i < 6000; i++)
        {
            var pick = options.GetWeightedRandom(x => x);
            counts[pick]++;
        }
        Assert.IsTrue(counts[3.0] > counts[2.0]);
        Assert.IsTrue(counts[2.0] > counts[1.0]);
    }

    [TestMethod]
    public void GetWeightedRandom_EvaluatorInt_BiasesSelection()
    {
        var options = new[] { 1, 3, 5 };
        var counts = new Dictionary<int, int> { { 1, 0 }, { 3, 0 }, { 5, 0 } };
        for (var i = 0; i < 6000; i++)
        {
            var pick = options.GetWeightedRandom(x => x);
            counts[pick]++;
        }
        Assert.IsTrue(counts[5] > counts[3]);
        Assert.IsTrue(counts[3] > counts[1]);
    }

    [TestMethod]
    public void GetRandomElement_ReturnsItemFromSource()
    {
        var data = new List<int> { 1, 2, 3 };
        var value = data.GetRandomElement();
        Assert.IsTrue(data.Contains(value));
    }

    [TestMethod]
    public void RandomNormal_ReturnsValueWithinBounds()
    {
        const double mean = 10.0;
        const double stdev = 2.0;
        var value = RandomUtilities.RandomNormal(mean, stdev);
        Assert.IsTrue(value >= mean - 6 * stdev && value <= mean + 6 * stdev);
    }

    [TestMethod]
    public void RandomNormalWithSkew_ReturnsValueWithinBounds()
    {
        const double mean = 10.0;
        const double stdev = 2.0;
        const double skew = 1.5;
        var value = RandomUtilities.RandomNormal(mean, stdev, skew);
        Assert.IsTrue(value >= mean - 6 * stdev && value <= mean + 6 * stdev);
    }

    [TestMethod]
    public void RandomNormalOverRange_ProducesValueWithinBounds()
    {
        const double min = 10.0;
        const double max = 20.0;
        var value = RandomUtilities.RandomNormalOverRange(min, max);
        var stdev = (max - min) / 8.0;
        var mean = (max + min) / 2.0;
        Assert.IsTrue(value >= mean - 6 * stdev && value <= mean + 6 * stdev);
    }
}

