#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Arenas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaSideIndexUtilitiesTests
{
    [TestMethod]
    public void ResolveEvenlySpacedStartCells_TwoSidesAcrossNineCells_KeepsSidesFarApart()
    {
        IReadOnlyDictionary<int, int> starts = ArenaSideIndexUtilities.ResolveEvenlySpacedStartCells(
            new List<int> { 0, 1 },
            9,
            2);

        Assert.AreEqual(2, starts[0]);
        Assert.AreEqual(6, starts[1]);

        int distance = Math.Abs(starts[0] - starts[1]);
        int wrappedDistance = Math.Min(distance, 9 - distance);
        Assert.AreEqual(4, wrappedDistance);
    }

    [TestMethod]
    public void ResolveEvenlySpacedStartCells_NormalisesRotationAndDeduplicatesSides()
    {
        IReadOnlyDictionary<int, int> starts = ArenaSideIndexUtilities.ResolveEvenlySpacedStartCells(
            new List<int> { 3, 3, 7, 9 },
            9,
            12);

        CollectionAssert.AreEquivalent(new[] { 3, 7, 9 }, starts.Keys.ToArray());
        Assert.AreEqual(3, starts[3]);
        Assert.AreEqual(6, starts[7]);
        Assert.AreEqual(0, starts[9]);
    }
}
