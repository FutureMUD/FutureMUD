using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CollectionDictionaryTests
{
    [TestMethod]
    public void Add_AddRange_Indexer_CreateListsOnDemand()
    {
        var cd = new CollectionDictionary<string, int>();

        // Add should create list for new key
        cd.Add("A", 1);
        Assert.AreEqual(1, cd["A"].Count, "Add did not create a list for key A.");

        // AddRange(IEnumerable<(T Key, U Value)>) should also create lists
        cd.AddRange(new[]
        {
            ("B", 2),
            ("B", 3),
            ("C", 4)
        });
        Assert.AreEqual(2, cd["B"].Count, "AddRange did not add multiple items for key B.");
        Assert.AreEqual(1, cd["C"].Count, "AddRange did not create list for key C.");

        // AddRange(T, IEnumerable<U>) should append values
        cd.AddRange("A", new[] {5, 6});
        Assert.AreEqual(3, cd["A"].Count, "AddRange(key, IEnumerable) did not append values to key A.");

        // Indexer getter should create an empty list for missing key
        Assert.AreEqual(0, cd["D"].Count, "Indexer getter did not create an empty list on demand for key D.");
    }

    [TestMethod]
    public void RemoveMethods_RemoveExpectedEntries()
    {
        var cd = new CollectionDictionary<string, int>();
        cd.Add("A", 1);
        cd.Add("A", 2);
        cd.Add("B", 3);
        cd.Add("C", 4);

        cd.RemoveRange("A", new[] {1});
        Assert.AreEqual(false, cd["A"].Contains(1), "RemoveRange did not remove the specified value.");
        Assert.AreEqual(1, cd["A"].Count, "RemoveRange removed incorrect number of items.");

        cd.RemoveAllKeys(k => k == "B" || k == "C");
        Assert.AreEqual(false, cd.ContainsKey("B"), "RemoveAllKeys did not remove key B.");
        Assert.AreEqual(false, cd.ContainsKey("C"), "RemoveAllKeys did not remove key C.");
        Assert.AreEqual(true, cd.ContainsKey("A"), "RemoveAllKeys removed an unexpected key.");
    }

    [TestMethod]
    public void Swap_SetValueAtIndex_InvalidIndicesReturnFalse()
    {
        var cd = new CollectionDictionary<string, int>();
        cd.AddRange("A", new[] {1, 2, 3});

        Assert.AreEqual(false, cd.Swap("A", -1, 1), "Swap should return false for a negative index.");
        Assert.AreEqual(false, cd.Swap("A", 0, 5), "Swap should return false for an index beyond the list length.");

        Assert.AreEqual(false, cd.SetValueAtIndex("A", -1, 99), "SetValueAtIndex should return false for a negative index.");
        Assert.AreEqual(false, cd.SetValueAtIndex("A", 5, 99), "SetValueAtIndex should return false for an index beyond the list length.");
    }

    [TestMethod]
    public void AsReadOnlyCollectionDictionary_ReturnsReadOnlyEnumerables()
    {
        var cd = new CollectionDictionary<string, int>();
        cd.Add("A", 1);
        var ro = cd.AsReadOnlyCollectionDictionary();

        var keysCollection = ro.Keys as ICollection<string>;
        Assert.IsNotNull(keysCollection, "Keys did not return a collection.");
        Assert.AreEqual(true, keysCollection.IsReadOnly, "Keys collection was not read-only.");

        var values = ro["A"] as ICollection<int>;
        Assert.IsNotNull(values, "Indexer did not return a collection.");
        Assert.AreEqual(true, values.IsReadOnly, "Values enumeration was not read-only.");
    }
}
