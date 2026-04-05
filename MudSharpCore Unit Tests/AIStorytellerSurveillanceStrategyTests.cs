using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.AIStorytellers;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerSurveillanceStrategyTests
{
    [TestMethod]
    public void GetCells_CombinesZonesAndIncludes_ThenRemovesExclusions()
    {
        (Mock<IFuturemud> gameworld, Mock<IZone> zone, Mock<ICell> cell1, Mock<ICell> cell2, Mock<ICell> cell3) = BuildWorld();
        AIStorytellerSurveillanceStrategy strategy = new(gameworld.Object, string.Empty);
        strategy.Zones.Add(zone.Object);
        strategy.IncludedCells.Add(cell3.Object);
        strategy.ExcludedCells.Add(cell2.Object);

        List<long> cells = strategy.GetCells(gameworld.Object).Select(x => x.Id).OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(new List<long> { 1L, 3L }, cells);
    }

    [TestMethod]
    public void SaveDefinition_RoundTripsThroughConstructor()
    {
        (Mock<IFuturemud> gameworld, Mock<IZone> zone, Mock<ICell> cell1, Mock<ICell> cell2, Mock<ICell> cell3) = BuildWorld();
        AIStorytellerSurveillanceStrategy original = new(gameworld.Object, string.Empty);
        original.Zones.Add(zone.Object);
        original.IncludedCells.Add(cell3.Object);
        original.ExcludedCells.Add(cell2.Object);

        string xml = original.SaveDefinition();
        AIStorytellerSurveillanceStrategy loaded = new(gameworld.Object, xml);

        CollectionAssert.AreEquivalent(new List<long> { 100L }, loaded.Zones.Select(x => x.Id).ToList());
        CollectionAssert.AreEquivalent(new List<long> { 3L }, loaded.IncludedCells.Select(x => x.Id).ToList());
        CollectionAssert.AreEquivalent(new List<long> { 2L }, loaded.ExcludedCells.Select(x => x.Id).ToList());
        CollectionAssert.AreEqual(new List<long> { 1L, 3L },
            loaded.GetCells(gameworld.Object).Select(x => x.Id).OrderBy(x => x).ToList());
    }

    private static (Mock<IFuturemud> Gameworld, Mock<IZone> Zone, Mock<ICell> Cell1, Mock<ICell> Cell2, Mock<ICell> Cell3)
        BuildWorld()
    {
        Mock<ICell> cell1 = new();
        cell1.SetupGet(x => x.Id).Returns(1L);
        cell1.SetupGet(x => x.Name).Returns("Cell One");

        Mock<ICell> cell2 = new();
        cell2.SetupGet(x => x.Id).Returns(2L);
        cell2.SetupGet(x => x.Name).Returns("Cell Two");

        Mock<ICell> cell3 = new();
        cell3.SetupGet(x => x.Id).Returns(3L);
        cell3.SetupGet(x => x.Name).Returns("Cell Three");

        Mock<IZone> zone = new();
        zone.SetupGet(x => x.Id).Returns(100L);
        zone.SetupGet(x => x.Name).Returns("Central Zone");
        zone.SetupGet(x => x.Cells).Returns([cell1.Object, cell2.Object]);

        Mock<IUneditableAll<IZone>> zoneRepo = BuildRepository(new[] { zone.Object });
        Mock<IUneditableAll<ICell>> cellRepo = BuildRepository(new[] { cell1.Object, cell2.Object, cell3.Object });

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.Zones).Returns(zoneRepo.Object);
        gameworld.SetupGet(x => x.Cells).Returns(cellRepo.Object);

        return (gameworld, zone, cell1, cell2, cell3);
    }

    private static Mock<IUneditableAll<T>> BuildRepository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
    {
        List<T> list = items.ToList();
        Dictionary<long, T> byId = list.ToDictionary(x => x.Id, x => x);
        Mock<IUneditableAll<T>> repo = new();
        repo.Setup(x => x.Get(It.IsAny<long>()))
            .Returns((long id) => byId.TryGetValue(id, out T value) ? value : null);
        repo.Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
        repo.SetupGet(x => x.Count).Returns(list.Count);
        return repo;
    }
}
