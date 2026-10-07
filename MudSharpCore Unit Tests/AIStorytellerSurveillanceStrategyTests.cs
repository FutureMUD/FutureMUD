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
    public void GetRooms_CombinesZonesAndIncludes_ThenRemovesExclusions()
    {
        (Mock<IFuturemud> gameworld, Mock<IZone> zone, Mock<IRoom> cell1, Mock<IRoom> cell2, Mock<IRoom> cell3) = BuildWorld();
        AIStorytellerSurveillanceStrategy strategy = new(gameworld.Object, string.Empty);
        strategy.Zones.Add(zone.Object);
        strategy.IncludedRooms.Add(cell3.Object);
        strategy.ExcludedRooms.Add(cell2.Object);

        List<long> rooms = strategy.GetRooms(gameworld.Object).Select(x => x.Id).OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(new List<long> { 1L, 3L }, rooms);
    }

    [TestMethod]
    public void SaveDefinition_RoundTripsThroughConstructor()
    {
        (Mock<IFuturemud> gameworld, Mock<IZone> zone, Mock<IRoom> cell1, Mock<IRoom> cell2, Mock<IRoom> cell3) = BuildWorld();
        AIStorytellerSurveillanceStrategy original = new(gameworld.Object, string.Empty);
        original.Zones.Add(zone.Object);
        original.IncludedRooms.Add(cell3.Object);
        original.ExcludedRooms.Add(cell2.Object);

        string xml = original.SaveDefinition();
        AIStorytellerSurveillanceStrategy loaded = new(gameworld.Object, xml);

        CollectionAssert.AreEquivalent(new List<long> { 100L }, loaded.Zones.Select(x => x.Id).ToList());
        CollectionAssert.AreEquivalent(new List<long> { 3L }, loaded.IncludedRooms.Select(x => x.Id).ToList());
        CollectionAssert.AreEquivalent(new List<long> { 2L }, loaded.ExcludedRooms.Select(x => x.Id).ToList());
        CollectionAssert.AreEqual(new List<long> { 1L, 3L },
            loaded.GetRooms(gameworld.Object).Select(x => x.Id).OrderBy(x => x).ToList());
    }

    private static (Mock<IFuturemud> Gameworld, Mock<IZone> Zone, Mock<IRoom> Room1, Mock<IRoom> Room2, Mock<IRoom> Room3)
        BuildWorld()
    {
        Mock<IRoom> cell1 = new();
        cell1.SetupGet(x => x.Id).Returns(1L);
        cell1.SetupGet(x => x.Name).Returns("Cell One");

        Mock<IRoom> cell2 = new();
        cell2.SetupGet(x => x.Id).Returns(2L);
        cell2.SetupGet(x => x.Name).Returns("Cell Two");

        Mock<IRoom> cell3 = new();
        cell3.SetupGet(x => x.Id).Returns(3L);
        cell3.SetupGet(x => x.Name).Returns("Cell Three");

        Mock<IZone> zone = new();
        zone.SetupGet(x => x.Id).Returns(100L);
        zone.SetupGet(x => x.Name).Returns("Central Zone");
        zone.SetupGet(x => x.Rooms).Returns([cell1.Object, cell2.Object]);

        Mock<IUneditableAll<IZone>> zoneRepo = BuildRepository(new[] { zone.Object });
        Mock<IUneditableAll<IRoom>> cellRepo = BuildRepository(new[] { cell1.Object, cell2.Object, cell3.Object });

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.Zones).Returns(zoneRepo.Object);
        gameworld.SetupGet(x => x.Rooms).Returns(cellRepo.Object);

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
