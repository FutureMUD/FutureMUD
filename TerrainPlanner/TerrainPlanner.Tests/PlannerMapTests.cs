using TerrainPlanner.Contracts;

namespace TerrainPlanner.Tests;

[TestClass]
public class PlannerMapTests
{
	[TestMethod]
	public void TerrainMaskUsesBottomLeftRowMajorOrdering()
	{
		var map = new PlannerMap(2, 2);
		map.PaintTerrain([new(0, 0)], 11);
		map.PaintTerrain([new(1, 0)], 12);
		map.PaintTerrain([new(0, 1)], 21);
		map.PaintTerrain([new(1, 1)], 22);

		Assert.AreEqual("11,12,21,22", MaskSerializer.ExportTerrainMask(map));
	}

	[TestMethod]
	public void ClearingTerrainAlsoClearsEveryTag()
	{
		var map = new PlannerMap(1, 1);
		map.PaintTerrain([new(0, 0)], 7);
		map.PaintTag([new(0, 0)], 101, true);
		map.PaintTag([new(0, 0)], 102, true);

		map.PaintTerrain([new(0, 0)], 0);

		Assert.AreEqual(0, map.CellAt(0, 0).TerrainId);
		Assert.AreEqual(0, map.CellAt(0, 0).TagIds.Count);
	}

	[TestMethod]
	public void ResizePreservesTheBottomLeftOverlap()
	{
		var map = new PlannerMap(3, 3);
		map.PaintTerrain([new(0, 0)], 1);
		map.PaintTerrain([new(2, 2)], 9);

		var cropped = map.Resize(2, 2);

		Assert.AreEqual(1, cropped.CellAt(0, 0).TerrainId);
		Assert.IsFalse(cropped.Cells.Any(cell => cell.TerrainId == 9));
	}

	[TestMethod]
	public void FloodAndRectangleRespectLayerSemantics()
	{
		var map = new PlannerMap(3, 2);
		map.PaintRectangle(new(0, 0), new(1, 1), PlannerLayer.Terrain, 4);
		map.PaintTerrain([new(2, 0), new(2, 1)], 8);

		map.FillTerrain(new(0, 0), 5);
		map.PaintRectangle(new(0, 0), new(2, 0), PlannerLayer.Tags, 44);

		Assert.IsTrue(map.Cells.Where(cell => cell.X < 2).All(cell => cell.TerrainId == 5));
		Assert.IsTrue(map.CellAt(0, 0).TagIds.Contains(44));
		Assert.IsTrue(map.CellAt(2, 0).TagIds.Contains(44));
	}

	[TestMethod]
	public void UndoRedoRestoresMultiTagEdits()
	{
		var map = new PlannerMap(1, 1);
		var history = new MapHistory();
		history.Record(map.PaintTerrain([new(0, 0)], 5));
		history.Record(map.PaintTag([new(0, 0)], 6, true));

		Assert.IsTrue(history.Undo(map));
		Assert.IsFalse(map.CellAt(0, 0).TagIds.Contains(6));
		Assert.IsTrue(history.Redo(map));
		Assert.IsTrue(map.CellAt(0, 0).TagIds.Contains(6));
	}

	[TestMethod]
	public void BatchedStrokeChangesMergeIntoOneUndoOperation()
	{
		var map = new PlannerMap(3, 1);
		var history = new MapHistory();
		var first = map.PaintTerrain([new(0, 0), new(1, 0)], 5);
		var second = map.PaintTerrain([new(1, 0), new(2, 0)], 5);
		history.Record(MapChangeSet.Merge([first, second]));

		Assert.IsTrue(history.Undo(map));
		Assert.IsTrue(map.Cells.All(cell => cell.TerrainId == 0));
		Assert.IsFalse(history.CanUndo);
	}

	[TestMethod]
	public void ProjectRoundTripPreservesTagsColoursAndUnknownNames()
	{
		var map = new PlannerMap(1, 1);
		map.PaintTerrain([new(0, 0)], 5);
		map.PaintTag([new(0, 0)], 7, true);
		var tags = new Dictionary<long, TagCatalogueItem>
		{
			[7] = new(7, "forest", "world - forest", null)
		};

		var project = map.ToProject("West March", "revision", tags, new Dictionary<long, string> { [7] = "#336699" });
		var restored = PlannerMap.FromProject(project);

		Assert.AreEqual(5, restored.CellAt(0, 0).TerrainId);
		Assert.IsTrue(restored.CellAt(0, 0).TagIds.Contains(7));
		Assert.AreEqual("#336699", project.TagColours[7]);
		Assert.AreEqual("forest", project.Cells[0].Tags[0].Name);
	}
}
