using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DictionaryWithDefaultExtensionsTests
{
    [TestMethod]
    public void ToDictionaryWithDefault_KeySelector_AssignsValuesCorrectly()
    {
        int[] numbers = new[] { 1, 2, 3 };
        DictionaryWithDefault<int, int> dict = numbers.ToDictionaryWithDefault(n => n * 2);

        Assert.AreEqual(1, dict[2], "Key selector did not map value 1 correctly.");
        Assert.AreEqual(2, dict[4], "Key selector did not map value 2 correctly.");
        Assert.AreEqual(3, dict[6], "Key selector did not map value 3 correctly.");
    }

    [TestMethod]
    public void ToDictionaryWithDefault_KeyValueSelectorAndComparer_UsesComparer()
    {
        string[] words = new[] { "apple", "banana", "cherry" };
        DictionaryWithDefault<string, int> dict = words.ToDictionaryWithDefault(s => s, s => s.Length, StringComparer.OrdinalIgnoreCase);

        Assert.AreEqual(5, dict["APPLE"], "Comparer did not allow case-insensitive lookup for APPLE.");
        Assert.AreEqual(6, dict["Banana"], "Comparer did not allow case-insensitive lookup for Banana.");
        Assert.AreEqual(6, dict["CHERRY"], "Comparer did not allow case-insensitive lookup for CHERRY.");
    }

    [TestMethod]
    public void DictionaryWithDefault_ReturnsDefaultValueForMissingKey()
    {
        int[] numbers = new[] { 1, 2, 3 };
        DictionaryWithDefault<int, int> dict = numbers.ToDictionaryWithDefault(n => n);

        Assert.AreEqual(0, dict[10], "Dictionary did not return default value for missing key.");
    }
}
