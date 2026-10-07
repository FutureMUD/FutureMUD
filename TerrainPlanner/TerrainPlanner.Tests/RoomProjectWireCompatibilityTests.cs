using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrainPlanner.Client.Services;
using TerrainPlanner.Contracts;

namespace TerrainPlanner.Tests;

[TestClass]
public class RoomProjectWireCompatibilityTests
{
	[TestMethod]
	public void BrowserStorage_FrozenSchemaOneProject_LoadEditSavePreservesTilesAndFeatures()
	{
		const string frozen = """
			{"schemaVersion":1,"name":"Frozen grove","width":2,"height":1,"catalogueRevision":"old-revision",
			"cells":[{"x":0,"y":0,"terrainId":45,"tags":[{"id":9,"name":"grove"}],"unresolvedFeatures":["future feature"]},
			{"x":1,"y":0,"terrainId":84,"tags":[{"id":12,"name":"water"}],"unresolvedFeatures":[]}],
			"tagColours":{"9":"#112233","12":"#445566"}}
			""";
		var project = BrowserStorage.Deserialize<PlannerProject>(frozen);
		Assert.AreEqual(1, project.SchemaVersion);
		Assert.AreEqual(2, project.Rooms.Count);
		var map = PlannerMap.FromProject(project);
		Assert.AreEqual(45L, map.RoomAt(0, 0).TerrainId);
		Assert.AreEqual(84L, map.RoomAt(1, 0).TerrainId);
		CollectionAssert.AreEquivalent(new long[] { 9 }, map.RoomAt(0, 0).TagIds.ToArray());
		CollectionAssert.AreEqual(new[] { "future feature" }, map.RoomAt(0, 0).UnresolvedFeatures.ToArray());
		map.PaintTerrain([new GridCoordinate(1, 0)], 85);
		var tags = new Dictionary<long, TagCatalogueItem>
		{
			[9] = new(9, "grove", "a grove", null), [12] = new(12, "water", "water", null)
		};
		var json = BrowserStorage.Serialize(map.ToProject(project.Name, project.CatalogueRevision, tags, project.TagColours));
		using var saved = JsonDocument.Parse(json);
		Assert.AreEqual(2, saved.RootElement.GetProperty("cells").GetArrayLength());
		Assert.IsFalse(saved.RootElement.TryGetProperty("rooms", out _));
		var reloaded = BrowserStorage.Deserialize<PlannerProject>(json);
		Assert.AreEqual("Frozen grove", reloaded.Name);
		Assert.AreEqual("old-revision", reloaded.CatalogueRevision);
		Assert.AreEqual("#112233", reloaded.TagColours[9]);
		Assert.AreEqual("#445566", reloaded.TagColours[12]);
		Assert.AreEqual(85L, reloaded.Rooms.Single(x => x.X == 1).TerrainId);
		Assert.AreEqual("grove", reloaded.Rooms.Single(x => x.X == 0).Tags.Single().Name);
		CollectionAssert.AreEqual(new[] { "future feature" }, reloaded.Rooms.Single(x => x.X == 0).UnresolvedFeatures);
	}
}
