using System.Text.Json;
using TerrainPlanner.Contracts;
using TerrainPlanner.Contracts.StackTransport;

namespace TerrainPlanner.Tests;

[TestClass]
public class PlannerStackTests
{
	private static readonly Dictionary<long, TagCatalogueItem> Tags = new() { [9] = new(9, "grove", "grove", null) };
	private static readonly Dictionary<long, string> Colours = new() { [9] = "#123456" };
	private static PlannerProject Project(PlannerStack stack) => stack.ToProject("Stack", "revision", Tags, Colours);
	private static PlannerStack Pair()
	{
		var stack = new PlannerStack(2, 2);
		stack.Levels[0].PaintTerrain([new(0, 0), new(1, 1)], 1);
		stack.AddLevel(-1, 0);
		stack.SetLink(new(0, 0, -1, Climb: true, Fall: true, Difficulty: 8));
		return stack;
	}

	[TestMethod]
	public void Project_SchemaOneUpgradeAndSchemaTwoRoundTrip_PreservesContentAndMetadata()
	{
		var old = new PlannerProject { SchemaVersion = 1, Width = 2, Height = 2,
			Rooms = [new() { X = 0, Y = 1, TerrainId = 45, Tags = [new(9, "grove")], UnresolvedFeatures = ["missing"] }] };
		var stack = PlannerStack.FromProject(old);
		Assert.AreEqual(45L, stack.Levels[0].RoomAt(0, 1).TerrainId);
		stack.AddLevel(-1, 0);
		var saved = Project(stack);
		Assert.AreEqual(2, saved.SchemaVersion);
		var loaded = JsonSerializer.Deserialize<PlannerProject>(JsonSerializer.Serialize(saved))!;
		var restored = PlannerStack.FromProject(loaded);
		Assert.AreEqual("Stack", loaded.Name);
		Assert.AreEqual("revision", loaded.CatalogueRevision);
		Assert.AreEqual("#123456", loaded.TagColours[9]);
		CollectionAssert.AreEqual(new[] { "missing" }, restored.Levels[-1].RoomAt(0, 1).UnresolvedFeatures.ToArray());
		CollectionAssert.AreEqual(new long[] { 9 }, restored.Levels[0].RoomAt(0, 1).TagIds.ToArray());
	}

	[TestMethod]
	public void ResizeAndErase_PruneLinksAndPreserveSouthwestOnAllLevels()
	{
		var stack = Pair();
		stack.SetLink(new(1, 1, -1, Suppressed: true));
		stack.Resize(1, 1);
		Assert.AreEqual(1, stack.Width);
		Assert.AreEqual(2, stack.OccupiedCount);
		Assert.AreEqual(1, stack.Links.Count());
		stack.Levels[0].PaintTerrain([new(0, 0)], 0);
		stack.PruneLinks();
		Assert.AreEqual(0, stack.Links.Count());
		Assert.IsFalse(stack.HasConnection(0, 0, -1));
	}

	[TestMethod]
	public void History_LevelLinkAndPaintEdits_UndoesAndRedoesWholeStack()
	{
		var stack = Pair();
		var history = new PlannerStackHistory();
		history.Record(PlannerStackHistory.Capture(stack));
		stack.RenumberLevel(-1, -2);
		Assert.IsFalse(stack.HasConnection(0, 0, -2));
		Assert.AreEqual(0, stack.Links.Count());
		stack = history.Undo(stack);
		Assert.IsTrue(stack.GetLink(0, 0, -1).Fall);
		Assert.AreEqual(8, stack.GetLink(0, 0, -1).Difficulty);
		stack = history.Redo(stack);
		Assert.IsTrue(stack.Levels.ContainsKey(-2));
		history.Record(PlannerStackHistory.Capture(stack));
		stack.DeleteLevel(-2);
		stack = history.Undo(stack);
		Assert.AreEqual(2, stack.Levels.Count);
	}

	[TestMethod]
	public void Export_UnknownReferences_BlocksWhileProjectPreservesThem()
	{
		var stack = Pair();
		stack.Levels[-1].PaintTag([new(0, 0)], 9, true);
		Assert.ThrowsException<InvalidDataException>(() => stack.Export(new HashSet<long> { 1 }, new HashSet<long>()));
		Assert.ThrowsException<InvalidDataException>(() => stack.Export(new HashSet<long>(), new HashSet<long> { 9 }));
		var token = PlannerStackExport.Encode(stack.Export(new HashSet<long> { 1 }, new HashSet<long> { 9 }));
		var decoded = TerrainStackCodec.Decode(TerrainStackCodec.WrapForEditor(token));
		Assert.AreEqual(4, decoded.Rooms.Length);
		Assert.IsTrue(decoded.Links.Single().Fall);
	}

	[TestMethod]
	public void Levels_DuplicateAndLimits_RejectWithoutChangingStack()
	{
		var stack = Pair();
		Assert.ThrowsException<InvalidDataException>(() => stack.AddLevel(-1));
		Assert.ThrowsException<InvalidDataException>(() => stack.RenumberLevel(-1, 0));
		Assert.ThrowsException<InvalidDataException>(() => stack.Resize(201, 2));
		Assert.ThrowsException<InvalidDataException>(() => PlannerStack.ValidateSize(200, 200, 6));
		Assert.AreEqual(2, stack.Levels.Count);
		Assert.AreEqual(2, stack.Width);
		Assert.IsFalse(stack.HasConnection(0, 0, int.MaxValue));
	}
}
