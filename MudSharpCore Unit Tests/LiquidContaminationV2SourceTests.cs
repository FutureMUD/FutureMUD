#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LiquidContaminationV2SourceTests
{
	[TestMethod]
	public void RainWeatherEvent_DoesNotFanOutThroughAllCellContents()
	{
		var rain = File.ReadAllText(GetSourcePath("MudSharpCore", "Climate", "WeatherEvents", "RainWeatherEvent.cs"));
		var controller = File.ReadAllText(GetSourcePath("MudSharpCore", "Climate", "WeatherController.cs"));

		var methodStart = rain.IndexOf("public override void OnFiveSecondEvent", StringComparison.Ordinal);
		var nextRegion = rain.IndexOf("#region Overrides", methodStart, StringComparison.Ordinal);
		Assert.IsTrue(methodStart > 0);
		Assert.IsTrue(nextRegion > methodStart);

		var method = rain[methodStart..nextRegion];
		Assert.IsFalse(method.Contains("cell.GameItems", StringComparison.Ordinal));
		Assert.IsFalse(method.Contains("cell.Characters", StringComparison.Ordinal));
		Assert.IsFalse(method.Contains("ExposeToPrecipitation", StringComparison.Ordinal));
		StringAssert.Contains(controller, "!CurrentWeatherEvent.RequiresRoomFiveSecondTick");
	}

	[TestMethod]
	public void PuddleTopUp_UsesVirtualRoomSurfaceInsteadOfCreatingSavedItems()
	{
		var puddleProto = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Prototypes",
			"PuddleGameItemComponentProto.cs"));
		var methodStart = puddleProto.IndexOf("public static void TopUpOrCreateNewPuddle", StringComparison.Ordinal);
		var methodEnd = puddleProto.IndexOf("\n        }", methodStart, StringComparison.Ordinal);
		Assert.IsTrue(methodStart > 0);
		Assert.IsTrue(methodEnd > methodStart);

		var method = puddleProto[methodStart..methodEnd];
		StringAssert.Contains(method, "AddLiquidToSurface");
		Assert.IsFalse(method.Contains("CreateNewPuddle(mixture", StringComparison.Ordinal));
	}

	[TestMethod]
	public void RoomWeatherResolution_ConsolidatesLegacyPuddleItemsBeforePresentation()
	{
		var cellSurface = File.ReadAllText(GetSourcePath("MudSharpCore", "Construction", "Cell.SurfaceLiquid.cs"));
		var bodyPerception = File.ReadAllText(GetSourcePath("MudSharpCore", "Body", "Implementations", "BodyPerception.cs"));
		var puddleComponent = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components", "PuddleGameItemComponent.cs"));

		StringAssert.Contains(cellSurface, "ConsolidateLegacyPuddles(layer);");
		StringAssert.Contains(cellSurface, "!Gameworld.GetStaticBool(\"PuddlesEnabled\")");
		StringAssert.Contains(cellSurface, "x.GetItemType<PuddleGameItemComponent>()");
		StringAssert.Contains(cellSurface, "state.AddLiquid(puddle.LiquidMixture);");
		StringAssert.Contains(cellSurface, "item.Delete();");
		StringAssert.Contains(puddleComponent, "public override void Delete()");
		StringAssert.Contains(puddleComponent, "DeregisterEvents();");

		var resolveIndex = bodyPerception.IndexOf("Location.ResolveRoomWeatherExposure(Actor);", StringComparison.Ordinal);
		var snapshotIndex = bodyPerception.IndexOf("List<IGameItem> items = Location.LayerGameItems", StringComparison.Ordinal);
		Assert.IsTrue(resolveIndex > 0);
		Assert.IsTrue(snapshotIndex > resolveIndex, "Legacy puddles must be consolidated before the visible item snapshot is taken.");
	}

	[TestMethod]
	public void RoomLook_RendersVirtualSurfaceOnlyThroughCellDescription()
	{
		var cellDescription = File.ReadAllText(GetSourcePath("MudSharpCore", "Construction", "CellDescription.cs"));
		var bodyPerception = File.ReadAllText(GetSourcePath("MudSharpCore", "Body", "Implementations", "BodyPerception.cs"));

		StringAssert.Contains(cellDescription, "DescribeLiquidSurface(voyeur?.RoomLayer ?? RoomLayer.GroundLevel, voyeur, colour)");
		Assert.IsFalse(bodyPerception.Contains("Location.DescribeLiquidSurface", StringComparison.Ordinal),
			"The room surface is already part of Location.HowSeen and must not be appended again by the outer look routine.");
	}

	[TestMethod]
	public void DebugEvaporate_DriesVirtualRoomSurfacesAsWellAsLegacyPuddles()
	{
		var staffModule = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "StaffModule.cs"));
		var methodStart = staffModule.IndexOf("private static void DebugEvaporate", StringComparison.Ordinal);
		var methodEnd = staffModule.IndexOf("private static void DebugFixBites", methodStart, StringComparison.Ordinal);
		Assert.IsTrue(methodStart > 0);
		Assert.IsTrue(methodEnd > methodStart);

		var method = staffModule[methodStart..methodEnd];
		StringAssert.Contains(method, "x.GetItemType<PuddleGameItemComponent>()");
		StringAssert.Contains(method, "actor.Gameworld.Cells");
		StringAssert.Contains(method, "x.SurfaceLiquidStates");
		StringAssert.Contains(method, "state.Dry(state.LiquidVolume, roomSurface: true);");
	}

	[TestMethod]
	public void EmptyOntoGround_UsesVirtualRoomSurface()
	{
		var manipulation = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "ManipulationModule.cs"));
		StringAssert.Contains(manipulation, "PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, actor.Location, actor.RoomLayer, actor);");
		Assert.IsFalse(manipulation.Contains("PuddleGameItemComponentProto.CreateNewPuddle(mixture, actor.Location", StringComparison.Ordinal));
	}

	[TestMethod]
	public void PuddlesDisabled_DoesNotDisableLazyRainExposure()
	{
		var cellSurface = File.ReadAllText(GetSourcePath("MudSharpCore", "Construction", "Cell.SurfaceLiquid.cs"));
		var methodStart = cellSurface.IndexOf("public void ResolveRoomWeatherExposure", StringComparison.Ordinal);
		var methodEnd = cellSurface.IndexOf("private void LoadSurfaceLiquidState", methodStart, StringComparison.Ordinal);
		Assert.IsTrue(methodStart > 0);
		Assert.IsTrue(methodEnd > methodStart);

		var method = cellSurface[methodStart..methodEnd];
		StringAssert.Contains(method, "_lastWeatherExposureByLayer");
		StringAssert.Contains(method, "if (Gameworld.GetStaticBool(\"PuddlesEnabled\"))");
		StringAssert.Contains(method, "item.ExposeToPrecipitation");
		StringAssert.Contains(method, "ch.Body.ExposeToPrecipitation");
		Assert.IsFalse(method.Contains("if (!Gameworld.GetStaticBool(\"PuddlesEnabled\"))", StringComparison.Ordinal));
	}

	[TestMethod]
	public void InitialInserts_PersistSurfaceLiquidState()
	{
		var gameItem = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "GameItem.cs"));
		var itemInsertStart = gameItem.IndexOf("public override object DatabaseInsert()", StringComparison.Ordinal);
		var itemInsertEnd = gameItem.IndexOf("SaveMorphProgress(dbitem);", itemInsertStart, StringComparison.Ordinal);
		Assert.IsTrue(itemInsertStart > 0);
		Assert.IsTrue(itemInsertEnd > itemInsertStart);

		var itemInsert = gameItem[itemInsertStart..itemInsertEnd];
		StringAssert.Contains(itemInsert, "SurfaceLiquidData = SaveSurfaceLiquidState()");

		var body = File.ReadAllText(GetSourcePath("MudSharpCore", "Body", "Implementations", "Body.cs"));
		var bodyInsertStart = body.IndexOf("public override object DatabaseInsert()", StringComparison.Ordinal);
		var bodyInsertEnd = body.IndexOf("foreach (IBodypart item in _severedRoots)", bodyInsertStart, StringComparison.Ordinal);
		Assert.IsTrue(bodyInsertStart > 0);
		Assert.IsTrue(bodyInsertEnd > bodyInsertStart);

		var bodyInsert = body[bodyInsertStart..bodyInsertEnd];
		StringAssert.Contains(bodyInsert, "SurfaceLiquidData = SaveSurfaceLiquidState()");
	}

	[TestMethod]
	public void Cleaning_RoutesCleaningLiquidIntoSurfaceState()
	{
		var gameModule = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules",
			"GameModule.cs"));
		var cleanStart = gameModule.IndexOf("private static Action<IPerceivable> CleanAction", StringComparison.Ordinal);
		var cleanEnd = gameModule.IndexOf("private static Queue<ICleanableEffect> GetCleanableEffectQueue", cleanStart, StringComparison.Ordinal);
		Assert.IsTrue(cleanStart > 0);
		Assert.IsTrue(cleanEnd > cleanStart);

		var cleanAction = gameModule[cleanStart..cleanEnd];
		StringAssert.Contains(cleanAction, "ISurfaceContaminable surface");
		StringAssert.Contains(cleanAction, "surface.SurfaceLiquidState.AddLiquid");
		Assert.IsFalse(cleanAction.Contains("new LiquidContamination", StringComparison.Ordinal));
	}

	[TestMethod]
	public void Cleaning_DoesNotQueueEffectsWithoutRequiredLiquid()
	{
		var gameModule = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules",
			"GameModule.cs"));
		var queueStart = gameModule.IndexOf("private static Queue<ICleanableEffect> GetCleanableEffectQueue", StringComparison.Ordinal);
		var queueEnd = gameModule.IndexOf("private static void CleanTarget", queueStart, StringComparison.Ordinal);
		Assert.IsTrue(queueStart > 0);
		Assert.IsTrue(queueEnd > queueStart);

		var cleanQueue = gameModule[queueStart..queueEnd];
		StringAssert.Contains(cleanQueue, "x.LiquidRequired != null");
		Assert.IsFalse(cleanQueue.Contains("x.LiquidRequired == null ||", StringComparison.Ordinal));
	}

	[TestMethod]
	public void WashingMachine_SpinDriesSurfaceStateAndCompletesCycle()
	{
		var washingMachine = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components",
			"WashingMachineGameItemComponent.cs"));

		StringAssert.Contains(washingMachine, "item.SurfaceLiquidState.Dry");
		StringAssert.Contains(washingMachine, "CurrentCycle = WashingMachineCycles.None;");
	}

	private static string GetSourcePath(params string[] parts)
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "MudSharpCore")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate the repository root from the test output directory.");
		return Path.Combine(directory.FullName, Path.Combine(parts));
	}
}
