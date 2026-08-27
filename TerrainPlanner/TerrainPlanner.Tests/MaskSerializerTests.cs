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
		var byId = new Dictionary<long, TagCatalogueItem>
		{
			[Forest.Id] = Forest,
			[Road.Id] = Road
		};

		MaskSerializer.ImportFeatureMask(map, "1|2,,,2", byId);

		Assert.AreEqual("1|2,,,2", MaskSerializer.ExportFeatureMask(map));
	}

	[TestMethod]
	public void UnknownTagIdsArePreservedAndVisibleOnExport()
	{
		var map = new PlannerMap(1, 1);
		map.PaintTerrain([new(0, 0)], 5);

		MaskSerializer.ImportFeatureMask(map, "999", new Dictionary<long, TagCatalogueItem>());

		Assert.AreEqual("999", MaskSerializer.ExportFeatureMask(map));
	}

	[DataTestMethod]
	[DataRow("forest")]
	[DataRow("0")]
	[DataRow("-1")]
	[DataRow("1.5")]
	public void NonPositiveOrNonNumericTagIdsAreRejected(string value)
	{
		Assert.ThrowsException<InvalidDataException>(() => MaskSerializer.ParseFeatureTagId(value));
	}

	[TestMethod]
	public void UnknownTerrainIdsAndWrongDimensionsAreRejected()
	{
		var map = new PlannerMap(2, 1);
		Assert.ThrowsException<InvalidDataException>(() =>
			MaskSerializer.ImportTerrainMask(map, "1,999", new HashSet<long> { 1 }));
		Assert.ThrowsException<InvalidDataException>(() =>
			MaskSerializer.ImportFeatureMask(map, "", new Dictionary<long, TagCatalogueItem>()));
	}

	[TestMethod]
	public void DuplicateTagShortNamesRemainIndependentlyRepresentableById()
	{
		var firstRoad = new TagCatalogueItem(10, "road", "world / trade / road", null);
		var secondRoad = new TagCatalogueItem(11, "ROAD", "world / travel / road", null);
		var index = TagCatalogueIndex.Create([Forest, firstRoad, secondRoad]);
		var map = new PlannerMap(1, 1);
		map.PaintTerrain([new(0, 0)], 5);

		Assert.AreEqual(3, index.ById.Count);
		MaskSerializer.ImportFeatureMask(map, "10|11", index.ById);
		Assert.AreEqual("10|11", MaskSerializer.ExportFeatureMask(map));
	}
}
