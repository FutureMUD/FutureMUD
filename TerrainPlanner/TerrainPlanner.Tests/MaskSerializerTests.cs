using TerrainPlanner.Contracts;

namespace TerrainPlanner.Tests;

[TestClass]
public class MaskSerializerTests
{
	private static readonly TagCatalogueItem Forest = new(1, "forest", "world - forest", null);
	private static readonly TagCatalogueItem Road = new(2, "road", "world - route - road", null);

	[TestMethod]
	public void MultiTagFeatureMaskRoundTripsWithBlankEntries()
	{
		var map = new PlannerMap(2, 2);
		MaskSerializer.ImportTerrainMask(map, "5,5,0,5", new HashSet<long> { 5 });
		var byName = new Dictionary<string, TagCatalogueItem>(StringComparer.OrdinalIgnoreCase)
		{
			[Forest.ShortName] = Forest,
			[Road.ShortName] = Road
		};

		MaskSerializer.ImportFeatureMask(map, "forest|road,,,road", byName);

		var byId = byName.Values.ToDictionary(tag => tag.Id);
		Assert.AreEqual("forest|road,,,road", MaskSerializer.ExportFeatureMask(map, byId));
	}

	[TestMethod]
	public void UnknownFeaturesArePreservedAndVisibleOnExport()
	{
		var map = new PlannerMap(1, 1);
		map.PaintTerrain([new(0, 0)], 5);

		MaskSerializer.ImportFeatureMask(map, "removed-tag", new Dictionary<string, TagCatalogueItem>());

		Assert.AreEqual("removed-tag", MaskSerializer.ExportFeatureMask(map, new Dictionary<long, TagCatalogueItem>()));
	}

	[TestMethod]
	[DataRow("bad,name")]
	[DataRow("bad|name")]
	[DataRow("bad\nname")]
	public void DelimiterNamesAreRejected(string name)
	{
		Assert.ThrowsException<InvalidDataException>(() => MaskSerializer.ValidateFeatureName(name));
	}

	[TestMethod]
	public void UnknownTerrainIdsAndWrongDimensionsAreRejected()
	{
		var map = new PlannerMap(2, 1);
		Assert.ThrowsException<InvalidDataException>(() =>
			MaskSerializer.ImportTerrainMask(map, "1,999", new HashSet<long> { 1 }));
		Assert.ThrowsException<InvalidDataException>(() =>
			MaskSerializer.ImportFeatureMask(map, "", new Dictionary<string, TagCatalogueItem>()));
	}
}
