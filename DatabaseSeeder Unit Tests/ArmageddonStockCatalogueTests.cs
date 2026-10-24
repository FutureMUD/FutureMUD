#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Models;
using Material = MudSharp.Models.Material;
using EditableItem = MudSharp.Models.EditableItem;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonStockCatalogueTests
{
	[TestMethod]
	public void Catalogue_CompleteTypedGraph_DependencyOrderAndRecipes()
	{
		var catalogue = new ArmageddonStockCatalogue();
		Assert.AreEqual(210, catalogue.Blueprints.Count);
		Assert.AreEqual(49, catalogue.Blueprints.Count(x => x.Kind == ArmageddonBlueprintKind.Creature));
		Assert.AreEqual(49, catalogue.Blueprints.Count(x => x.Kind == ArmageddonBlueprintKind.MeleeItem));
		Assert.AreEqual(61, catalogue.Blueprints.Count(x => x.Kind == ArmageddonBlueprintKind.ComponentSupply));
		var visited = new HashSet<string>();
		foreach (var row in catalogue.Blueprints)
		{
			Assert.IsTrue(row.Dependencies.All(visited.Contains), row.Key);
			Assert.IsTrue(row.References.All(x => row.Dependencies.Contains(x.Key) && catalogue.ByKey[x.Key].Kind == x.Kind), row.Key);
			visited.Add(row.Key);
		}
		Assert.AreEqual(90, catalogue.ByKey["arm.material.conjuration.rank6"].Recipe!.MinimumComponentCrafting);
		Assert.AreEqual(7.0, catalogue.ByKey["arm.material.conjuration.rank6"].Recipe!.Inputs["arm.item.component_blank"]);
		Assert.AreEqual(420.0, catalogue.ByKey["arm.material.conjuration.rank6"].Recipe!.Seconds);
	}

	[TestMethod]
	public void Catalogue_WrongTypedReferenceRejected_NarrativeNumbersDoNotBecomeReferences()
	{
		var json = JsonNode.Parse(ArmageddonStockManifest.Read())!;
		var rows = json["prototype_catalogue"]!.AsArray();
		var creature = rows.First(x => x!["kind"]!.GetValue<string>() == "approved_simple_npc_template")!;
		var race = creature["configuration"]!["race_bundle"]!.GetValue<string>();
		var ai = creature["configuration"]!["ai_bundle"]!.GetValue<string>();
		creature["configuration"]!["race_bundle"] = ai;
		Assert.ThrowsExactly<InvalidOperationException>(() => new ArmageddonStockCatalogue(json.ToJsonString()));
		creature["configuration"]!["race_bundle"] = race;
		creature["configuration"]!["author_notes"] = "Character #1234, arm.race.unknown and 987654321 are narrative examples.";
		Assert.AreEqual(210, new ArmageddonStockCatalogue(json.ToJsonString()).Blueprints.Count);
	}

	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void SupplyBatch_ExactNativeCountsInputsUnitsAndGates_IdempotentAndNoLiveEntities(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var result = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(64, db.GameItemProtos.Count());
		Assert.AreEqual(64, db.Crafts.Count());
		Assert.AreEqual(61, db.Tags.Count());
		Assert.AreEqual(4, db.AutobuilderRoomTemplates.Count());
		Assert.AreEqual(1, db.Foragables.Count());
		Assert.AreEqual(1, db.ForagableProfilesForagables.Count());
		Assert.AreEqual(0, db.GameItems.Count());
		Assert.AreEqual(0, db.Characters.Count());
		Assert.AreEqual(0, db.Rooms.Count());
		Assert.AreEqual(0, db.MagicSpells.Count());
		Assert.AreEqual(classes ? 10 : 0, db.ChargenRoles.Count());
		var owned = db.SeederManagedRecords.Where(x => x.Module.StartsWith("stock-catalogue")).ToArray();
		var before = owned.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		var childIds = db.CraftInputs.Select(x => x.Id).Concat(db.CraftProducts.Select(x => x.Id)).Order().ToArray();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(factory).Status);
		db.ChangeTracker.Clear();
		foreach (var record in db.SeederManagedRecords.Where(x => x.Module.StartsWith("stock-catalogue")))
			Assert.AreEqual(before[record.StableKey], (record.LogicalId, record.RevisionNumber, record.SeedBaseline, record.AppliedAt), record.StableKey);
		CollectionAssert.AreEqual(childIds, db.CraftInputs.Select(x => x.Id).Concat(db.CraftProducts.Select(x => x.Id)).Order().ToArray());
		var blankId = result.Identities["arm.item.component_blank"];
		Assert.AreEqual(10.0, db.GameItemProtos.Single(x => x.Id == blankId).Weight);
		var blankCraftId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.component_blank.recipe").LogicalId;
		var input = db.CraftInputs.Single(x => x.CraftId == blankCraftId);
		Assert.AreEqual("Commodity", input.InputType);
		Assert.AreEqual(10.0, (double)XElement.Parse(input.Definition).Element("Weight")!);
		Assert.AreEqual(3L, (long)XElement.Parse(input.Definition).Element("Material")!);
		var highCraftId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.material.conjuration.rank6.recipe").LogicalId;
		var high = db.Crafts.Single(x => x.Id == highCraftId);
		Assert.AreEqual(420.0, db.CraftPhases.Single(x => x.CraftPhaseId == high.Id).PhaseLengthInSeconds);
		var highInput = db.CraftInputs.Single(x => x.CraftId == high.Id);
		Assert.AreEqual("SimpleItem", highInput.InputType);
		Assert.AreEqual(7, (int)XElement.Parse(highInput.Definition).Element("Quantity")!);
		Assert.AreEqual(blankId, (long)XElement.Parse(highInput.Definition).Element("TargetItemId")!);
		StringAssert.Contains(db.FutureProgs.Single(x => x.Id == high.CanUseProgId).FunctionText, ">= 90");
		ArmageddonInstallation.Prepare(factory, mode);
	}

	[TestMethod]
	public void SupplyBatch_NativeRecipeProgs_ExecuteNumericTraitLookupAtEveryApprovedThreshold()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var result = ArmageddonCatalogueSupplies.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status);
		db.ChangeTracker.Clear();
		var craftingId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockProgression.CraftTraitKey).LogicalId!.Value;
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Id).Returns(craftingId);
		trait.SetupGet(x => x.Name).Returns("Builder renamed Component Crafting");
		trait.SetupGet(x => x.Type).Returns(ProgVariableTypes.Trait);
		trait.SetupGet(x => x.GetObject).Returns(trait.Object);
		var traits = new All<ITraitDefinition>(); traits.Add(trait.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Traits).Returns(traits);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		MudSharp.FutureProg.FutureProg.Initialise();
		var catalogue = new ArmageddonStockCatalogue();
		foreach (var blueprint in catalogue.Blueprints.Where(x => x.Kind is ArmageddonBlueprintKind.ComponentSupply or ArmageddonBlueprintKind.CraftSupply or ArmageddonBlueprintKind.StackableSupply))
		{
			var craftId = db.SeederManagedRecords.Single(x => x.StableKey == blueprint.Key + ".recipe").LogicalId!.Value;
			var gateId = db.Crafts.Single(x => x.Id == craftId).CanUseProgId;
			var model = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == gateId);
			var prog = FutureProgFactory.CreateNew(model, world.Object);
			Assert.IsTrue(prog.Compile(), prog.CompileError);
			var minimum = blueprint.Recipe!.MinimumComponentCrafting;
			actor.Setup(x => x.TraitValue(trait.Object, TraitBonusContext.None)).Returns(minimum - 1);
			Assert.IsFalse(prog.ExecuteBool(actor.Object), blueprint.Key + " below threshold");
			actor.Setup(x => x.TraitValue(trait.Object, TraitBonusContext.None)).Returns(minimum);
			Assert.IsTrue(prog.ExecuteBool(actor.Object), blueprint.Key + " at threshold");
		}
	}

	[TestMethod]
	public void SupplyBatch_DatetimeRoundTripAndEarlierStockContent_RepairsInPlaceWithoutReplacingChildIds()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(factory).Status);
		db.ChangeTracker.Clear();
		var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.material.abjuration.rank0.recipe");
		var input = db.CraftInputs.Single(x => x.CraftId == record.LogicalId);
		var product = db.CraftProducts.Single(x => x.CraftId == record.LogicalId);
		var phase = db.CraftPhases.Single(x => x.CraftPhaseId == record.LogicalId);
		var phaseKey = (phase.CraftPhaseId, phase.CraftPhaseRevisionNumber, phase.PhaseNumber);
		var inputId = input.Id; var productId = product.Id;
		var inputXml = XElement.Parse(input.Definition); inputXml.Element("Quantity")!.Value = "2"; input.Definition = inputXml.ToString();
		var productXml = XElement.Parse(product.Definition); productXml.Element("Quantity")!.Value = "2"; product.Definition = productXml.ToString();
		input.OriginalAdditionTime = DateTime.SpecifyKind(input.OriginalAdditionTime, DateTimeKind.Unspecified);
		product.OriginalAdditionTime = DateTime.SpecifyKind(product.OriginalAdditionTime, DateTimeKind.Unspecified);
		var baseline = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		foreach (var (field, definition, added) in new[] { ("Inputs", input.Definition, input.OriginalAdditionTime), ("Products", product.Definition, product.OriginalAdditionTime) })
		{
			var children = JsonNode.Parse(baseline[field])!.AsArray();
			children[0]!["Definition"] = definition;
			// Earlier stock used UTC metadata; the native database returns an unzoned DATETIME.
			children[0]!["Added"] = DateTime.SpecifyKind(added, DateTimeKind.Utc).ToString("O");
			baseline[field] = children.ToJsonString();
		}
		record.SeedBaseline = System.Text.Json.JsonSerializer.Serialize(baseline);
		db.SaveChanges(); db.ChangeTracker.Clear();
		var repaired = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, repaired.Status, string.Join("\n", repaired.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(1, (int)XElement.Parse(db.CraftInputs.Single(x => x.Id == inputId).Definition).Element("Quantity")!);
		Assert.AreEqual(1, (int)XElement.Parse(db.CraftProducts.Single(x => x.Id == productId).Definition).Element("Quantity")!);
		var repairedPhase = db.CraftPhases.Single(x => x.CraftPhaseId == record.LogicalId);
		Assert.AreEqual(phaseKey, (repairedPhase.CraftPhaseId, repairedPhase.CraftPhaseRevisionNumber, repairedPhase.PhaseNumber));
		Assert.IsFalse(repaired.Messages.Any(x => x.Contains("preserved builder edit")), string.Join("\n", repaired.Messages));
	}

	[TestMethod]
	public void SupplyBatch_StockInputTypeChange_RequiresNativeRevisionAndRetainsOldInputId()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(factory).Status);
		db.ChangeTracker.Clear();
		var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.material.abjuration.rank0.recipe");
		var input = db.CraftInputs.Single(x => x.CraftId == record.LogicalId); var inputId = input.Id;
		input.InputType = "Commodity";
		input.Definition = "<Definition><Material>3</Material><Weight>10</Weight><CommodityPileTag>0</CommodityPileTag><Characteristics/></Definition>";
		var baseline = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		var children = JsonNode.Parse(baseline["Inputs"])!.AsArray();
		children[0]!["Type"] = input.InputType; children[0]!["Definition"] = input.Definition;
		baseline["Inputs"] = children.ToJsonString(); record.SeedBaseline = System.Text.Json.JsonSerializer.Serialize(baseline);
		db.SaveChanges(); db.ChangeTracker.Clear();
		var blocked = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Failed, blocked.Status);
		Assert.IsTrue(blocked.Messages.Any(x => x.Contains("requires a native craft revision")));
		Assert.AreEqual("Commodity", db.CraftInputs.Single(x => x.Id == inputId).InputType);
	}

	[TestMethod]
	public void SupplyBatch_BuilderFieldsChildrenRemovedForageAndRenamedSharedPrimitivesArePreserved()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		var id = first.Identities["arm.item.component_blank"];
		db.GameItemProtos.Single(x => x.Id == id).ShortDescription = "a builder's handworked blank";
		var craftId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.material.abjuration.rank0.recipe").LogicalId;
		db.CraftInputs.Single(x => x.CraftId == craftId).Definition = "<Definition><TargetItemId>999</TargetItemId><Quantity>2</Quantity></Definition>";
		db.ForagableProfilesForagables.RemoveRange(db.ForagableProfilesForagables);
		db.Materials.Single(x => x.Id == 3).Name = "builder's clay";
		db.Terrains.Single(x => x.Id == 2).Name = "builder's cave";
		db.UnitsOfMeasure.Single(x => x.Id == 1).Name = "builder's mass unit";
		db.SaveChanges();
		var result = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual("a builder's handworked blank", db.GameItemProtos.Single(x => x.Id == id).ShortDescription);
		Assert.AreEqual(999L, (long)XElement.Parse(db.CraftInputs.Single(x => x.CraftId == craftId).Definition).Element("TargetItemId")!);
		Assert.AreEqual(0, db.ForagableProfilesForagables.Count());
		Assert.IsTrue(result.Messages.Any(x => x.Contains("preserved builder edit")));
	}

	[TestMethod]
	public void SupplyBatch_DeletedOwnedPrototypeAndNoncurrentRevision_BlockWithoutRecreating()
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		var item = db.GameItemProtos.Include(x => x.EditableItem).Single(x => x.Id == first.Identities["arm.item.component_blank"]);
		item.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
		db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Failed, ArmageddonCatalogueSupplies.Install(factory).Status);
		Assert.AreEqual(64, db.GameItemProtos.Count());
		db.GameItemProtos.Remove(item); db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Failed, ArmageddonCatalogueSupplies.Install(factory).Status);
		Assert.AreEqual(63, db.GameItemProtos.Count());
	}

	internal static MudSharp.Database.FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode, bool classes)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture(classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		ArmageddonInstallation.Prepare(factory, mode);
		var progression = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, progression.Status);
		db.Materials.Add(new Material { Id = 3, Name = "clay", BehaviourType = (int)MaterialBehaviourType.Soil, Density = 1.21 });
		db.StackDecorators.Add(new StackDecorator { Id = 1, Name = "Suffix", Type = "Suffix", Definition = "<Definition/>" });
		db.ForagableProfiles.Add(new ForagableProfile { Id = 1, RevisionNumber = 0, Name = "Fixture finite clay",
			EditableItem = new EditableItem { RevisionStatus = (int)RevisionStatus.Current } });
		db.ForagableProfilesMaximumYields.Add(new() { ForagableProfileId = 1, ForagableProfileRevisionNumber = 0, ForageType = "clay", Yield = 10 });
		db.Terrains.AddRange(new Terrain { Id = 1, Name = "Hallway", ForagableProfileId = 1 }, new Terrain { Id = 2, Name = "Cave" });
		db.SaveChanges();
		return db;
	}
}
