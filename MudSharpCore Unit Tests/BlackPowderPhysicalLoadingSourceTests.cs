#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BlackPowderPhysicalLoadingSourceTests
{
	[TestMethod]
	public void MusketLoading_SplitsLocatedCommodityAndKeepsPhysicalConsumables()
	{
		var source = ReadSource("MudSharpCore", "GameItems", "Components", "MusketGameItemComponent.cs");

		StringAssert.Contains(source, "powderSource.GetByWeight(loader.Body");
		StringAssert.Contains(source, "ContainLoadedItem(wad)");
		StringAssert.Contains(source, "_matchCord = installed");
		StringAssert.Contains(source, "_ignitionStone = installed");
		StringAssert.Contains(source, "PileGameItemComponentProto.CreateNewBundle");
		StringAssert.Contains(source, "CreateLoadingBundlePlan");
		StringAssert.Contains(source, "CreateBoundPlan");
		StringAssert.Contains(source, "item => item == Parent");
		StringAssert.Contains(source, "new XElement(\"CleaningRod\"");
		StringAssert.Contains(source, "_prototype.CleaningRodTag");
		StringAssert.Contains(source, "_cleaningRod?.Parent.Login()");
		StringAssert.Contains(source, "RestoreToolAttachment(loader, cleaningRod, restoreCleaningRodAttachment)");
		StringAssert.Contains(source, "RestoreToolAttachment(loader, ramrod, restoreRamrodAttachment)");
		StringAssert.Contains(source, "RestoreToolAttachment(actor, ramrod, restoreToolAttachment)");
		StringAssert.Contains(source, "item.InInventoryOf?.Take(item)");
		StringAssert.Contains(source, "item.Location?.Extract(item)");
		Assert.IsFalse(source.Contains("ReleaseSplitSourceFromHands", StringComparison.Ordinal));
		StringAssert.Contains(source, "IsReadied || !IsLoaded");
		StringAssert.Contains(source, "stone.IsA(_prototype.IgnitionSourceTag)");
		StringAssert.Contains(source, "You must unjam {Parent.HowSeen(loader)} before loading it.");
		StringAssert.Contains(source, "You must unready {Parent.HowSeen(loader)} before unloading it.");
		StringAssert.Contains(source, "is already fully loaded.");
		var prototypeSource = ReadSource("MudSharpCore", "GameItems", "Prototypes",
			"MusketGameItemComponentProto.cs");
		Assert.IsFalse(prototypeSource.Contains("DesiredItemState.InRoom", StringComparison.Ordinal));
		StringAssert.Contains(prototypeSource, "null, 1, originalReference: \"ball\"");
		StringAssert.Contains(prototypeSource, "null, 1, originalReference: \"wad\"");
		StringAssert.Contains(prototypeSource, "null, 1, originalReference: \"cartridge\"");
		StringAssert.Contains(prototypeSource, "null, 1, originalReference: \"matchcord\"");
		StringAssert.Contains(prototypeSource, "useRetrievedItemAsResult: true");
		var inventoryPlanSource = ReadSource("MudSharpCore", "GameItems", "Inventory", "Plans",
			"InventoryPlanTemplate.cs");
		StringAssert.Contains(inventoryPlanSource, "UseRetrievedItemAsResult");
		StringAssert.Contains(inventoryPlanSource, "PrimaryTarget = gottenItem");
		StringAssert.Contains(inventoryPlanSource, "actor.Body.Get(item, quantity, null, silent,");
		Assert.IsFalse(source.Contains("CommodityGameItemComponentProto.CreateNewCommodity", StringComparison.Ordinal));
		Assert.IsFalse(source.Contains("Name.Contains(\"match cord\"", StringComparison.Ordinal));
	}

	[TestMethod]
	public void AttachCommand_RoutesBeltableMusketToolsToTheCompatibleAttachmentSlot()
	{
		var source = ReadSource("MudSharpCore", "Commands", "Modules", "ManipulationModule.cs");

		StringAssert.Contains(source, "actor.TargetItem(ss.PeekSpeech())?.GetItemType<MusketGameItemComponent>()");
		StringAssert.Contains(source, "musket.CanAttachBeltable(targetAsBeltable) == IBeltCanAttachBeltableResult.NotValidType");
		Assert.IsTrue(source.IndexOf("ss.PeekSpeech()", StringComparison.Ordinal) <
		              source.IndexOf("musket.TryInstallIgnitionStone", StringComparison.Ordinal));
	}

	[TestMethod]
	public void ArtilleryLoading_PersistsEveryChargeSidePhysicalItem()
	{
		var source = ReadSource("MudSharpCore", "GameItems", "Components", "ArtilleryPieceGameItemComponent.cs");

		StringAssert.Contains(source, "_powderCharge");
		StringAssert.Contains(source, "_wad");
		StringAssert.Contains(source, "_primerCharge");
		StringAssert.Contains(source, "_fuse");
		StringAssert.Contains(source, "CreateStagePlan(loader)");
		StringAssert.Contains(source, "powderSource.GetByWeight(loader.Body");
		StringAssert.Contains(source, "primerSource.GetByWeight(loader.Body");
		StringAssert.Contains(source, "CharacterState.Able.HasFlag(actor.State)");
		StringAssert.Contains(source, "ArtilleryLoadingStage.Empty => \"sponge and clear\"");
		StringAssert.Contains(source, "CreateTaggedPlan(actor, _prototype.LinstockTag, \"linstock\")");
		StringAssert.Contains(source, "linstockPlan?.FinalisePlan()");
		StringAssert.Contains(source, "int quantity = 1");
		StringAssert.Contains(source, "reference, 0)");
		StringAssert.Contains(source, "UseRetrievedItemAsResult = quantity == 1");
		StringAssert.Contains(source, "firingTarget ?? Parent");
		Assert.IsFalse(source.Contains("Set a reachable indirect firing solution or select a visible target before firing.", StringComparison.Ordinal));
		StringAssert.Contains(source, "stand|stands down $0 from immediate ignition readiness");
		StringAssert.Contains(source, "Stand down the artillery piece from ignition readiness before unloading it.");
		Assert.IsFalse(source.Contains("actor.State.HasFlag(CharacterState.Able)", StringComparison.Ordinal));
	}

	[TestMethod]
	public void FireIntoAir_ClearsOutOfCombatAimAfterDischarge()
	{
		var source = ReadSource("MudSharpCore", "Commands", "Modules", "CombatModule.cs");
		var airFire = source.IndexOf("aiming.Aim.Weapon.Fire(actor, null", StringComparison.Ordinal);
		Assert.IsTrue(airFire >= 0);
		var cleanup = source.IndexOf("actor.RemoveEffect(aiming, true);", airFire, StringComparison.Ordinal);
		Assert.IsTrue(cleanup > airFire);
		var returnStatement = source.IndexOf("return;", airFire, StringComparison.Ordinal);
		Assert.IsTrue(returnStatement > cleanup);
	}

	[TestMethod]
	public void TimedMusketLoading_BlocksOverlappingCommandsAndMovement()
	{
		var source = ReadSource("MudSharpCore", "Effects", "Concrete", "LoadingMusket.cs");
		StringAssert.Contains(source, "_blocks.Add(\"general\")");
		StringAssert.Contains(source, "_blocks.Add(\"movement\")");
		StringAssert.Contains(source, "LDescAddendum = \"loading $1\"");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..",
			Path.Combine(parts))));
	}
}
