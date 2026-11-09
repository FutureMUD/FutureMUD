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
	[TestCategory("Integration")]
	public void Shelters_RealTypedSourcesAndApprovedDefaults_PreserveWorldAndClosedGates()
	{
		using var db = Fixture();
		var rooms = db.Rooms.Count();
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		foreach (var shelter in ArmageddonShelterSpellContent.Contributions)
		{
			var spell = db.MagicSpells.Single(x => x.Id == installed.Identities[shelter.Key]);
			var effect = XElement.Parse(spell.Definition).Element("Effects")!.Elements().Single();
			Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == shelter.TemplateKey).LogicalId,
				(long)effect.Element("AutobuilderTemplate")!);
			Assert.IsTrue(db.AutobuilderRoomTemplates.Any(x => x.Id == (long)effect.Element("AutobuilderTemplate")!));
			Assert.AreEqual(73L, (long)effect.Element("FallbackRoom")!, "Use the fixture-selected safe fallback, not Room 1.");
			if (shelter.Kind == SpellShelterKind.SeveringRefuge)
			{
				var ward = SpellShelterWardConfiguration.Load(effect.Element("Ward")!);
				Assert.IsTrue(ward.SchoolIds.All(id => db.MagicSchools.Any(x => x.Id == id)));
			}
		}
		Assert.AreEqual(rooms, db.Rooms.Count()); Assert.AreEqual(0, db.Characters.Count()); Assert.AreEqual(0, db.GameItems.Count());
		var aliasRecord = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSpellInstaller.FallbackBindingKey);
		Assert.AreEqual(ArmageddonCatalogueLinks.AliasType, aliasRecord.EntityType); Assert.IsNull(aliasRecord.LogicalId);
		var alias = JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(aliasRecord.SeedBaseline!)!;
		Assert.AreEqual(nameof(Room), alias.NativeType); Assert.AreEqual(73L, alias.NativeId);
	}

	[TestMethod]
	[TestCategory("Integration")]
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
	[TestCategory("Integration")]
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
