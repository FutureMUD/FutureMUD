#nullable enable

using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	public void Shelters_RealTypedSourcesAndApprovedDefaults_PreserveWorldAndClosedGates()
	{
		using var db = Fixture();
		var rooms = db.Rooms.Count();
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		foreach (var shelter in ArmageddonShelterSpellContent.Contributions)
		{
			var approved = manifest.RootElement.GetProperty("entries").EnumerateArray().Single(x => x.GetProperty("key").GetString() == shelter.Key);
			Assert.AreEqual(approved.GetProperty("minimum_energy").GetDouble(), shelter.MinimumEnergy);
			var spell = db.MagicSpells.Single(x => x.Id == installed.Identities[shelter.Key]);
			var definition = XElement.Parse(spell.Definition); var effect = definition.Element("Effects")!.Elements().Single();
			Assert.AreEqual("room", (string?)definition.Element("Trigger")!.Attribute("type"));
			Assert.AreEqual("createshelter", (string?)effect.Attribute("type"));
			Assert.AreEqual(0L, (long)effect.Element("TemplateRoom")!);
			Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == shelter.TemplateKey).LogicalId, (long)effect.Element("AutobuilderTemplate")!);
			Assert.IsTrue(db.AutobuilderRoomTemplates.Any(x => x.Id == (long)effect.Element("AutobuilderTemplate")!));
			Assert.AreEqual(73L, (long)effect.Element("FallbackRoom")!, "Choose the actual configured safe start, never guess Room1.");
			Assert.IsTrue((bool)effect.Element("CapacityByGrade")!); Assert.AreEqual(1, (int)effect.Element("MaximumOccupants")!);
			Assert.AreEqual(300.0, (double)effect.Element("SecondsPerGrade")!);
			CollectionAssert.AreEqual(new long[] { 1, 2 }, effect.Elements("Terrain").Select(x => (long)x).ToArray());
			Assert.AreEqual(shelter.MinimumEnergy, (double)definition.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
			Assert.AreEqual("0", db.TraitExpressions.Single(x => x.Id == spell.EffectDurationExpressionId).Expression);
			Assert.IsFalse(spell.ScrollInscriptionAllowed); Assert.IsFalse(definition.Element("Plan")!.HasElements);
			if (shelter.Kind == SpellShelterKind.SpringHaven)
			{
				Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.haven_water").LogicalId, (long)effect.Element("WaterPrototype")!);
				Assert.AreEqual(ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.Water], (long)effect.Element("Liquid")!);
				Assert.AreEqual(0.5, (double)effect.Element("LitresPerGrade")!);
			}
			if (shelter.Kind == SpellShelterKind.SeveringRefuge)
			{
				var ward = SpellShelterWardConfiguration.Load(effect.Element("Ward")!);
				Assert.AreEqual(MagicInterdictionCoverage.Both, ward.Coverage); Assert.AreEqual(7, ward.SchoolIds.Count);
				CollectionAssert.AreEquivalent(new[] { "water", "earth", "fire", "wind", "lightning", "shadow", "void" }
					.Select(x => db.SeederManagedRecords.Single(y => y.StableKey == "arm.school." + x).LogicalId!.Value).ToArray(), ward.SchoolIds.ToArray());
			}
		}
		Assert.AreEqual(rooms, db.Rooms.Count()); Assert.AreEqual(0, db.Characters.Count()); Assert.AreEqual(0, db.GameItems.Count());
		var aliasRecord = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSpellInstaller.FallbackBindingKey);
		Assert.AreEqual(ArmageddonCatalogueLinks.AliasType, aliasRecord.EntityType); Assert.IsNull(aliasRecord.LogicalId);
		var alias = JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(aliasRecord.SeedBaseline!)!;
		Assert.AreEqual(nameof(Room), alias.NativeType); Assert.AreEqual(73L, alias.NativeId);
	}

	[TestMethod]
	[DataRow("missing-template")]
	[DataRow("retired-template")]
	[DataRow("wrong-type")]
	[DataRow("bad-interior")]
	[DataRow("missing-water-alias")]
	[DataRow("scripted-water")]
	[DataRow("small-water")]
	[DataRow("unsafe-fallback")]
	public void Shelters_InvalidTypedDependency_HoldsBeforeThisContributionAllocation(string change)
	{
		using var db = Fixture();
		var claim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.roomtemplate.spring_haven");
		var template = db.AutobuilderRoomTemplates.Single(x => x.Id == claim.LogicalId);
		if (change == "missing-template") db.AutobuilderRoomTemplates.Remove(template);
		if (change == "retired-template") claim.Retired = true;
		if (change == "wrong-type") template.TemplateType = "random description";
		if (change == "bad-interior") { var xml = XElement.Parse(template.Definition); xml.Element("Default")!.Element("Terrain")!.Element("DefaultTerrain")!.Value = "9999"; template.Definition = xml.ToString(); }
		if (change == "missing-water-alias") db.SeederManagedRecords.Remove(db.SeederManagedRecords.Single(x => x.StableKey == "arm.liquid.water"));
		if (change is "scripted-water" or "small-water")
		{
			var componentClaim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.haven_water.component");
			var component = db.GameItemComponentProtos.Single(x => x.Id == componentClaim.LogicalId && x.RevisionNumber == componentClaim.RevisionNumber);
			var xml = XElement.Parse(component.Definition); xml.SetAttributeValue(change == "scripted-water" ? "AdjustQuantityProg" : "LiquidCapacity", 1);
			component.Definition = xml.ToString();
		}
		if (change == "unsafe-fallback") db.Rooms.Single(x => x.Id == 73).Temporary = true;
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
		Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == ArmageddonStockSpellContent.ReadAttunementKey));
		Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == "arm.spell.call_outsider"));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.spell.spring_haven"));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == ArmageddonStockSpellInstaller.FallbackBindingKey));
	}

	[TestMethod]
	public void Shelters_RetainedFallbackBecomesUnsafe_HoldsWithoutChoosingAnotherStart()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonStockSpellInstaller.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var before = db.MagicSpells.ToDictionary(x => x.Id, x => x.Definition);
		var alias = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSpellInstaller.FallbackBindingKey);
		var baseline = alias.SeedBaseline;
		db.Rooms.Single(x => x.Id == 73).HostedVehicleId = 900;
		db.ChargenScreenStoryboards.Single(x => x.ChargenType == "StartingLocationPicker").StageDefinition = "<Definition><Locations><Location><Location>1</Location></Location></Locations></Definition>";
		db.SaveChanges(); db.ChangeTracker.Clear();
		var rerun = ArmageddonStockSpellInstaller.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Blocked, rerun.Status);
		Assert.AreEqual(baseline, db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSpellInstaller.FallbackBindingKey).SeedBaseline);
		foreach (var spell in db.MagicSpells) Assert.AreEqual(before[spell.Id], spell.Definition);
	}
}
