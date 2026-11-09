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
	public void Catalogue_TypedReferencesResolveInDependencyOrder()
	{
		var catalogue = new ArmageddonStockCatalogue();

		var visited = new HashSet<string>();
		foreach (var row in catalogue.Blueprints)
		{
			Assert.IsTrue(row.Dependencies.All(visited.Contains), row.Key);
			Assert.IsTrue(row.References.All(x => row.Dependencies.Contains(x.Key) && catalogue.ByKey[x.Key].Kind == x.Kind), row.Key);
			visited.Add(row.Key);
		}

	}

	[TestMethod]
	public void Catalogue_WrongTypedReferenceRejected_NarrativeNumbersDoNotBecomeReferences()
	{
		var json = JsonNode.Parse(ArmageddonStockManifest.Read())!;
		var expectedCount = new ArmageddonStockCatalogue(json.ToJsonString()).Blueprints.Count;
		var rows = json["prototype_catalogue"]!.AsArray();
		var creature = rows.First(x => x!["kind"]!.GetValue<string>() == "approved_simple_npc_template")!;
		var race = creature["configuration"]!["race_bundle"]!.GetValue<string>();
		var ai = creature["configuration"]!["ai_bundle"]!.GetValue<string>();
		creature["configuration"]!["race_bundle"] = ai;
		Assert.ThrowsExactly<InvalidOperationException>(() => new ArmageddonStockCatalogue(json.ToJsonString()));
		creature["configuration"]!["race_bundle"] = race;
		creature["configuration"]!["author_notes"] = "Character #1234, arm.race.unknown and 987654321 are narrative examples.";
		Assert.AreEqual(expectedCount, new ArmageddonStockCatalogue(json.ToJsonString()).Blueprints.Count);
	}

	[TestMethod]
	[TestCategory("Integration")]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void SupplyBatch_RerunPreservesManagedRecordsAndCreatesNoLiveEntities(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var initialClaims = db.SeederManagedRecords.Count();
		var result = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey,
			x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		Assert.IsTrue(before.Count > initialClaims, "Installation must create its own managed records.");
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(factory).Status);
		db.ChangeTracker.Clear();
		Assert.AreEqual(before.Count, db.SeederManagedRecords.Count());
		foreach (var record in db.SeederManagedRecords)
			Assert.AreEqual(before[record.StableKey],
				(record.LogicalId, record.RevisionNumber, record.SeedBaseline, record.AppliedAt), record.StableKey);
		Assert.AreEqual(0, db.GameItems.Count());
		Assert.AreEqual(0, db.Characters.Count());
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
		var beforeItems = db.GameItemProtos.Count();
		var item = db.GameItemProtos.Include(x => x.EditableItem).Single(x => x.Id == first.Identities["arm.item.component_blank"]);
		item.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
		db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Failed, ArmageddonCatalogueSupplies.Install(factory).Status);
		Assert.AreEqual(beforeItems, db.GameItemProtos.Count());
		db.GameItemProtos.Remove(item); db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Failed, ArmageddonCatalogueSupplies.Install(factory).Status);
		Assert.AreEqual(beforeItems - 1, db.GameItemProtos.Count());
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void SupplyBatch_LegacyNamesRepairInPlaceAndPreserveIndependentBuilderEdits(bool editNoun, bool editUniqueName)
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var beforeCount = db.GameItemProtos.Count();
		var originalNames = db.GameItemProtos.ToDictionary(x => x.Id, x => (x.Name, x.UniqueName));
		foreach (var item in db.GameItemProtos)
		{
			var record = db.SeederManagedRecords.Single(x => x.EntityType == nameof(GameItemProto) && x.LogicalId == item.Id);
			var baseline = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
			baseline["Name"] = System.Text.Json.JsonSerializer.Serialize(item.UniqueName);
			baseline.Remove("UniqueName");
			record.SeedBaseline = System.Text.Json.JsonSerializer.Serialize(baseline);
			item.Name = item.UniqueName;
			item.UniqueName = null!;
		}
		var id = first.Identities["arm.material.abjuration.rank0"];
		var edited = db.GameItemProtos.Single(x => x.Id == id);
		if (editNoun) edited.Name = "reagent";
		if (editUniqueName) edited.UniqueName = "Builder's abjuration reagent";
		db.SaveChanges();
		db.ChangeTracker.Clear();
		for (var run = 0; run < 2; run++)
		{
			var result = ArmageddonCatalogueSupplies.Install(factory);
			Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
			CollectionAssert.AreEquivalent(first.Identities.ToArray(), result.Identities.ToArray());
			db.ChangeTracker.Clear();
			var actual = db.GameItemProtos.Single(x => x.Id == id);
			Assert.AreEqual(editNoun ? "reagent" : originalNames[id].Name, actual.Name);
			Assert.AreEqual(editUniqueName ? "Builder's abjuration reagent" : originalNames[id].UniqueName, actual.UniqueName);
			Assert.AreEqual(beforeCount, db.GameItemProtos.Count());
			Assert.AreEqual(0, db.GameItemProtos.Count(x => x.Name.StartsWith("Armageddon ")));
			Assert.AreEqual(0, db.GameItemProtos.Count(x => x.UniqueName == null || x.UniqueName == ""));
		}
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void SupplyBatch_UnownedUniqueOrLegacyLabelCollisionIsRejected(bool legacy, bool rerun)
	{
		using var db = Fixture(ArmageddonProgressionMode.Simple, false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var label = "Armageddon " + new ArmageddonStockCatalogue().ByKey["arm.item.focus_blank"].Name;
		long? targetId = null;
		if (rerun)
		{
			var first = ArmageddonCatalogueSupplies.Install(factory);
			Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
			db.ChangeTracker.Clear();
			targetId = first.Identities["arm.item.focus_blank"];
			var target = db.GameItemProtos.Single(x => x.Id == targetId);
			target.Name = label;
			target.UniqueName = null!;
			var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.focus_blank");
			var baseline = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
			baseline["Name"] = System.Text.Json.JsonSerializer.Serialize(label);
			baseline.Remove("UniqueName");
			record.SeedBaseline = System.Text.Json.JsonSerializer.Serialize(baseline);
		}
		var collisionLabel = label.ToLowerInvariant();
		db.GameItemProtos.Add(new GameItemProto
		{
			Id = 9000, Name = legacy ? collisionLabel : "blank", UniqueName = legacy ? null! : collisionLabel,
			EditableItem = new EditableItem { RevisionStatus = (int)RevisionStatus.Current }
		});
		db.SaveChanges();
		var result = ArmageddonCatalogueSupplies.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Failed, result.Status);
		Assert.IsTrue(result.Messages.Any(x => x.Contains("collision")), string.Join("\n", result.Messages));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == nameof(GameItemProto) && x.LogicalId == 9000));
		if (rerun)
		{
			var target = db.GameItemProtos.Single(x => x.Id == targetId);
			Assert.AreEqual(label, target.Name);
			Assert.IsNull(target.UniqueName);
		}
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
