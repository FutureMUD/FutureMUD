using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CollectionDictionaryExtensionsTests
{
    [TestMethod]
    public void ToCollectionDictionary_ValueTupleEnumerable_GroupsValues()
    {
        (string Key, int Value)[] items = new (string Key, int Value)[]
        {
            ("A", 1),
            ("B", 2),
            ("A", 3)
        };

        CollectionDictionary<string, int> cd = items.ToCollectionDictionary<string, int>();

        CollectionAssert.AreEquivalent(new[] { "A", "B" }, cd.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 1, 3 }, cd["A"]);
        CollectionAssert.AreEquivalent(new[] { 2 }, cd["B"]);
    }

    [TestMethod]
    public void ToCollectionDictionary_TupleEnumerable_GroupsValues()
    {
        Tuple<string, int>[] items = new[]
        {
            Tuple.Create("A", 1),
            Tuple.Create("B", 2),
            Tuple.Create("A", 3)
        };

        CollectionDictionary<string, int> cd = items.ToCollectionDictionary<string, int>();

        CollectionAssert.AreEquivalent(new[] { "A", "B" }, cd.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 1, 3 }, cd["A"]);
        CollectionAssert.AreEquivalent(new[] { 2 }, cd["B"]);
    }

    [TestMethod]
    public void ToCollectionDictionary_KeyValuePairEnumerable_GroupsValues()
    {
        List<KeyValuePair<string, int>> items = new()
        {
            new("A", 1),
            new("B", 2),
            new("A", 3)
        };

        CollectionDictionary<string, int> cd = items.ToCollectionDictionary<string, int>();

        CollectionAssert.AreEquivalent(new[] { "A", "B" }, cd.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 1, 3 }, cd["A"]);
        CollectionAssert.AreEquivalent(new[] { 2 }, cd["B"]);
    }

    [TestMethod]
    public void ToCollectionDictionary_TransformsToNewKeyValueTypes()
    {
        CollectionDictionary<int, string> cd = new();
        cd.AddRange(1, new[] { "one", "uno" });
        cd.Add(2, "two");
        cd.Add(3, "three");

        CollectionDictionary<string, int> transformed = cd.ToCollectionDictionary<int, string, string, int>(
            k => k % 2 == 0 ? "even" : "odd",
            v => v.Length);

        CollectionAssert.AreEquivalent(new[] { "odd", "even" }, transformed.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 3, 3, 5 }, transformed["odd"]);
        CollectionAssert.AreEquivalent(new[] { 3 }, transformed["even"]);
    }
}

