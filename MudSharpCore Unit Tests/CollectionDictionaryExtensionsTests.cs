using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CollectionDictionaryExtensionsTests
{
    [TestMethod]
    public void ToCollectionDictionary_ValueTupleEnumerable_GroupsValues()
    {
        var items = new (string Key, int Value)[]
        {
            ("A", 1),
            ("B", 2),
            ("A", 3)
        };

        var cd = items.ToCollectionDictionary<string, int>();

        CollectionAssert.AreEquivalent(new[] { "A", "B" }, cd.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 1, 3 }, cd["A"]);
        CollectionAssert.AreEquivalent(new[] { 2 }, cd["B"]);
    }

    [TestMethod]
    public void ToCollectionDictionary_TupleEnumerable_GroupsValues()
    {
        var items = new[]
        {
            Tuple.Create("A", 1),
            Tuple.Create("B", 2),
            Tuple.Create("A", 3)
        };

        var cd = items.ToCollectionDictionary<string, int>();

        CollectionAssert.AreEquivalent(new[] { "A", "B" }, cd.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 1, 3 }, cd["A"]);
        CollectionAssert.AreEquivalent(new[] { 2 }, cd["B"]);
    }

    [TestMethod]
    public void ToCollectionDictionary_KeyValuePairEnumerable_GroupsValues()
    {
        var items = new List<KeyValuePair<string, int>>
        {
            new("A", 1),
            new("B", 2),
            new("A", 3)
        };

        var cd = items.ToCollectionDictionary<string, int>();

        CollectionAssert.AreEquivalent(new[] { "A", "B" }, cd.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 1, 3 }, cd["A"]);
        CollectionAssert.AreEquivalent(new[] { 2 }, cd["B"]);
    }

    [TestMethod]
    public void ToCollectionDictionary_TransformsToNewKeyValueTypes()
    {
        var cd = new CollectionDictionary<int, string>();
        cd.AddRange(1, new[] { "one", "uno" });
        cd.Add(2, "two");
        cd.Add(3, "three");

        var transformed = cd.ToCollectionDictionary<int, string, string, int>(
            k => k % 2 == 0 ? "even" : "odd",
            v => v.Length);

        CollectionAssert.AreEquivalent(new[] { "odd", "even" }, transformed.Keys.ToList());
        CollectionAssert.AreEquivalent(new[] { 3, 3, 5 }, transformed["odd"]);
        CollectionAssert.AreEquivalent(new[] { 3 }, transformed["even"]);
    }
}

