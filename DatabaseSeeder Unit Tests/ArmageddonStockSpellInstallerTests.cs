#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;
using MagicSpell = MudSharp.Models.MagicSpell;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonStockSpellInstallerTests
{
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void Install_FixedDirectionCanonicalDefaultsStableChildrenAndClosedAdmissions(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var before = db.MagicCapabilities.ToDictionary(x => x.Id, x => x.Definition);
		var installed = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		Assert.AreEqual(db.SeederManagedRecords.Count(x => x.Module == ArmageddonStockSpellInstaller.Module && !x.Retired), installed.Identities.Count);
		db.ChangeTracker.Clear();
		var spell = db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSpellContent.ReadAttunementKey).LogicalId);
		var definition = XElement.Parse(spell.Definition);
		Assert.AreEqual(ArmageddonStockSpellContent.ReadAttunementKey, (string?)definition.Element("StockIdentity"));
		Assert.AreEqual("character", (string?)definition.Element("Trigger")!.Attribute("type"));
		Assert.AreEqual("readattunement", (string?)definition.Element("Effects")!.Elements().Single().Attribute("type"));
		Assert.AreEqual(20.0, (double)definition.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
		CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), definition.Element("ControlledPower")!.Elements("Grade").Select(x => (int)x.Attribute("number")!).ToArray());
		CollectionAssert.AreEqual(Enumerable.Range(2, 7).ToArray(), definition.Element("ControlledPower")!.Elements("Grade").Select(x => (int)x.Attribute("power")!).ToArray());
		Assert.AreEqual("60*grade", db.TraitExpressions.Single(x => x.Id == spell.EffectDurationExpressionId).Expression);
		Assert.AreEqual("20*grade", db.TraitExpressions.Single(x => x.Id == installed.Identities[ArmageddonStockSpellContent.ReadAttunementKey + ".cost"]).Expression);
		Assert.AreEqual(installed.Identities[ArmageddonStockSpellContent.ReadAttunementKey], spell.Id);
		var traitKey = mode == ArmageddonProgressionMode.Classic ? ArmageddonStockSpellContent.ReadAttunementKey + ".skill" : "arm.skill.water_casting";
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == traitKey).LogicalId, spell.CastingTraitDefinitionId);
		Assert.IsFalse(spell.ScrollInscriptionAllowed);
		Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
		foreach (var capability in db.MagicCapabilities) Assert.AreEqual(before[capability.Id], capability.Definition);
		Assert.IsTrue(db.ChargenRoles.All(x => x.AvailabilityProgId == ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]));
		var snapshot = db.SeederManagedRecords.Where(x => x.Module == ArmageddonStockSpellInstaller.Module).ToDictionary(x => x.StableKey, x => (x.LogicalId, x.SeedBaseline, x.AppliedAt));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		db.ChangeTracker.Clear();
		foreach (var record in db.SeederManagedRecords.Where(x => x.Module == ArmageddonStockSpellInstaller.Module))
			Assert.AreEqual(snapshot[record.StableKey], (record.LogicalId, record.SeedBaseline, record.AppliedAt), record.StableKey);
		ArmageddonInstallation.Prepare(factory, mode); // The fixed package recognises this declared ownership module on rerun.
	}

	[TestMethod]
	public void ApprovedManifest_ReadAttunementDefaultMatchesLockedInventory()
	{
		using var json = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var entry = json.RootElement.GetProperty("entries").EnumerateArray().Single(x => x.GetProperty("key").GetString() == ArmageddonStockSpellContent.ReadAttunementKey);
		Assert.AreEqual(20.0, entry.GetProperty("minimum_energy").GetDouble());
		Assert.AreEqual("Water", entry.GetProperty("native_school").GetString());
		Assert.AreEqual("character", entry.GetProperty("target_type").GetString());
	}

	[TestMethod]
	public void Rerun_PreservesBuilderFieldsAndExpressionIdentity()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		db.ChangeTracker.Clear(); var spell = db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSpellContent.ReadAttunementKey).LogicalId);
		spell.Name = "Builder's Attunement"; spell.Description = "Custom presentation";
		var definition = XElement.Parse(spell.Definition); definition.Element("ControlledPower")!.Element("Efficiency")!.SetAttributeValue("minimum", 23);
		spell.Definition = definition.ToString();
		var duration = db.TraitExpressions.Single(x => x.Id == spell.EffectDurationExpressionId); duration.Expression = "90*grade";
		var durationId = duration.Id; var spellId = spell.Id; db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		db.ChangeTracker.Clear(); spell = db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSpellContent.ReadAttunementKey).LogicalId);
		Assert.AreEqual(spellId, spell.Id); Assert.AreEqual(durationId, spell.EffectDurationExpressionId);
		Assert.AreEqual("Builder's Attunement", spell.Name); Assert.AreEqual("Custom presentation", spell.Description);
		Assert.AreEqual(23.0, (double)XElement.Parse(spell.Definition).Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
		Assert.AreEqual("90*grade", db.TraitExpressions.Single(x => x.Id == durationId).Expression);
	}

	[DataTestMethod]
	[DataRow("name")][DataRow("identity")]
	public void Install_UnownedCollisionRefusesWithoutClaimingOrCreatingChildren(string collision)
	{
		using var db = Fixture();
		db.MagicSpells.Add(new MagicSpell { Name = collision == "name" ? "READ ATTUNEMENT" : "Clone", Definition = collision == "identity" ? "<Definition><StockIdentity>arm.spell.read_attunement</StockIdentity></Definition>" : "<Definition/>" });
		db.SaveChanges(); var count = db.SeederManagedRecords.Count(); var expressions = db.TraitExpressions.Count();
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db)).Status);
		Assert.AreEqual(count, db.SeederManagedRecords.Count()); Assert.AreEqual(expressions, db.TraitExpressions.Count());
	}

	[DataTestMethod]
	[DataRow("retired")][DataRow("deleted")][DataRow("child")][DataRow("identity")][DataRow("module")]
	public void Rerun_InvalidRetainedOwnershipHoldsWithoutResurrection(string corruption)
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		db.ChangeTracker.Clear(); var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSpellContent.ReadAttunementKey);
		switch (corruption)
		{
			case "retired": record.Retired = true; break;
			case "deleted": db.MagicSpells.Remove(db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSpellContent.ReadAttunementKey).LogicalId)); break;
			case "child": db.TraitExpressions.Remove(db.TraitExpressions.Single(x => x.Id == db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSpellContent.ReadAttunementKey).LogicalId).EffectDurationExpressionId)); break;
			case "identity": db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSpellContent.ReadAttunementKey).LogicalId).Definition = "<Definition/>"; break;
			case "module": record.Module = "unknown"; break;
		}
		db.SaveChanges(); var count = db.SeederManagedRecords.Count(); var spells = db.MagicSpells.Count(); var expressions = db.TraitExpressions.Count();
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, ArmageddonStockSpellInstaller.Install(factory).Status);
		Assert.AreEqual(count, db.SeederManagedRecords.Count()); Assert.AreEqual(spells, db.MagicSpells.Count()); Assert.AreEqual(expressions, db.TraitExpressions.Count());
	}

	[TestMethod]
	public void ConfirmationFailure_FreshContextResolvesSameCommittedIdentities()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonStockSpellInstaller.Install(factory, (_, stage) =>
		{
			if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("Injected confirmation loss");
		});
		Assert.AreEqual(ArmageddonInstallStatus.CommittedConfirmationFailed, first.Status);
		var second = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, second.Status);
		foreach (var pair in first.Identities) Assert.AreEqual(pair.Value, second.Identities[pair.Key]);
		Assert.AreEqual(db.SeederManagedRecords.Count(x => x.Module == ArmageddonStockSpellInstaller.Module && !x.Retired), second.Identities.Count);
		Assert.AreEqual(ArmageddonStockSpellInstaller.CanonicalKeys.Count, db.MagicSpells.Count());
	}

	[TestMethod]
	public void Classic_ReconciledLegacySkillRetainsModuleAndNativeIdentity()
	{
		using var db = Fixture(ArmageddonProgressionMode.Classic);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var claim = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSpellContent.ReadAttunementKey + ".skill");
		// S2 deliberately retains a compatible earlier tradition module while stamping its stock version.
		claim.Module = ArmageddonTraditionInstaller.Module;
		var id = claim.LogicalId;
		db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(factory).Status);
		var installed = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(id, db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSpellContent.ReadAttunementKey).LogicalId).CastingTraitDefinitionId);
		Assert.AreEqual(ArmageddonTraditionInstaller.Module, db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSpellContent.ReadAttunementKey + ".skill").Module);
	}

	private static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false)
	{
		var db = ArmageddonCatalogueInstallerTests.Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		ArmageddonInstallation.Prepare(factory, mode);
		db.TraitDefinitions.Add(new() { Name = "Native Fixture Strength", Alias = "str", Type = 1, DerivedType = 0, OwnerScope = 0, TraitGroup = "Physical" });
		db.TraitDefinitions.Add(new() { Name = "Constitution", Alias = "con", Type = 1, DerivedType = 0, OwnerScope = 0, TraitGroup = "Physical" });
		db.TraitDefinitions.Add(new() { Name = "Native Fixture Agility", Alias = "agi", Type = 1, DerivedType = 0, OwnerScope = 0, TraitGroup = "Physical" });
		db.FutureProgs.Single(x => x.Id == ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]).AcceptsAnyParameters = true;
		db.Planes.Add(new() { Name = "Native fixture material plane", Alias = "material", Description = "Fixture", RoomDescriptionAddendum = "", RoomNameFormat = "{0}", RemoteObservationTag = "", IsDefault = true });
		db.Gases.Single(x => x.Id == 1).Density = 1;
		foreach (var terrain in db.Terrains)
		{
			terrain.TerrainBehaviourMode = terrain.Id == 2 ? "cave" : "indoors";
			terrain.AtmosphereId = 1; terrain.AtmosphereType = "gas";
		}
		var package = new RoomOverlayPackage { Id = 11, RevisionNumber = 0, Name = "Native fixture package",
			EditableItem = new() { RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
		db.RoomOverlayPackages.Add(package);
		var room = new Room { Id = 73, Temporary = false };
		db.Rooms.Add(room);
		var overlay = new RoomOverlay { Id = 74, Room = room, RoomId = room.Id, TerrainId = 1,
			RoomOverlayPackage = package, RoomOverlayPackageId = package.Id, RoomOverlayPackageRevisionNumber = 0 };
		db.RoomOverlays.Add(overlay); room.CurrentOverlay = overlay; room.CurrentOverlayId = overlay.Id;
		db.ChargenScreenStoryboards.Add(new() { Id = 100, ChargenStage = (int)MudSharp.CharacterCreation.ChargenStage.SelectStartingLocation,
			ChargenType = "StartingLocationPicker", StageDefinition = "<Screen><Locations><Location><Location>999</Location></Location><Location><Location>73</Location></Location></Locations></Screen>" });
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(factory).Status);
		var catalogue = ArmageddonCatalogueInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, catalogue.Status, string.Join("\n", catalogue.Messages));
		return db;
	}
}
