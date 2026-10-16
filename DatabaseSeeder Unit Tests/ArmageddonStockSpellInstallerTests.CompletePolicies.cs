#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void CompleteInstall_MaterialisesAllInactiveAdmissionsAndSupports_PreservesRerun(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var answers = new Dictionary<string, string> { [ArmageddonMagicSeeder.FullInstallQuestion] = "yes", [ArmageddonMagicSeeder.ModeQuestion] = mode.ToString() };
		var report = new ArmageddonMagicSeeder().SeedData(db, answers);
		StringAssert.Contains(report, "availability disabled");
		db.ChangeTracker.Clear();
		var claims = db.SeederManagedRecords.ToDictionary(x => x.StableKey);
		var stock = new ArmageddonStockProgression(mode);
		var admissions = 0; var supports = 0;
		foreach (var spec in stock.Capabilities)
		{
			var cap = db.MagicCapabilities.Single(x => x.Id == claims[spec.Key].LogicalId);
			var root = XElement.Parse(cap.Definition); var casting = root.Element("Casting")!;
			Assert.IsFalse((bool)casting.Attribute("enabled")!, spec.Key);
			var actualSpells = casting.Elements("Admission").Select(x => (long)x.Attribute("spell")!).ToArray();
			CollectionAssert.AreEquivalent(spec.SpellKeys.Select(x => claims[x].LogicalId!.Value).ToArray(), actualSpells, spec.Key);
			Assert.AreEqual(6, casting.Elements("Admission").Count(x => (bool)x.Attribute("starting")!) + (spec.Key.Contains("sorcerer") || spec.Key.Contains("preserver") || spec.Key.Contains("defiler") ? 2 : 0), spec.Key);
			var sorcerer = ArmageddonStockProgression.IsSorcerer(spec);
			Assert.AreEqual((mode == ArmageddonProgressionMode.Classic ? 1 : 2) + (sorcerer ? 1 : 0), casting.Elements("SupportGrant").Count(), spec.Key);
			Assert.AreEqual(spec.Key is "arm.capability.sorcerer" or "arm.capability.preserver" or "arm.capability.defiler" ? 0 : 1,
				root.Element("Regenerators")!.Elements().Count(), spec.Key);
			var methods = root.Element("Gathering")?.Elements("Method").ToArray() ?? [];
			CollectionAssert.AreEqual(spec.Key switch
			{
				"arm.capability.sorcerer" => new[] { "Self", "Land" },
				"arm.capability.preserver" => ["Self"], "arm.capability.defiler" => ["Land"], _ => Array.Empty<string>()
			}, methods.Select(x => (string)x.Attribute("kind")!).ToArray(), spec.Key);
			foreach (var method in methods)
			{
				Assert.AreEqual(0.25, (double)method.Attribute("duration")!);
				Assert.IsTrue((long)method.Attribute("permission")! > 0);
				Assert.IsTrue((long)method.Attribute("damageProg")! > 0);
				Assert.AreEqual(claims[spec.ResourceKey].LogicalId, (long)method.Attribute("destination")!);
			}
			if (sorcerer)
			{
				var grant = casting.Elements("SupportGrant").Single(x => (long)x.Attribute("trait")! == claims[ArmageddonProgressionInstaller.GatheringTraitKey].LogicalId);
				Assert.AreEqual(10.0, (double)grant.Attribute("opening")!); Assert.AreEqual(90.0, (double)grant.Attribute("rawCap")!);
			}
			admissions += actualSpells.Length; supports += casting.Elements("SupportGrant").Count();
		}
		Assert.AreEqual(419, admissions);
		Assert.AreEqual(mode == ArmageddonProgressionMode.Classic ? 13 : 23, supports);
		Assert.AreEqual(classes ? 10 : 0, db.ChargenRoles.Count());
		Assert.IsTrue(db.ChargenRoles.All(x => x.AvailabilityProgId == ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]));
		Assert.AreEqual(0, db.CharacterAcquiredSpells.Count()); Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
		Assert.AreEqual(0, db.CharactersMagicResources.Count());
		Assert.AreEqual(0, db.VariableValues.Count());
		var ordinaryRerun = ArmageddonProgressionInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ordinaryRerun.Status, string.Join("\n", ordinaryRerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(4, db.MagicCapabilities.AsEnumerable().Sum(x => XElement.Parse(x.Definition).Element("Gathering")?.Elements("Method").Count() ?? 0));
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		new ArmageddonMagicSeeder().SeedData(db, answers);
		db.ChangeTracker.Clear();
		foreach (var claim in db.SeederManagedRecords)
			Assert.AreEqual(before[claim.StableKey], (claim.LogicalId, claim.RevisionNumber, claim.SeedBaseline, claim.AppliedAt), claim.StableKey);
		var blueprints = new ArmageddonStockCatalogue().Blueprints;
		var missing = blueprints.Where(x => !claims.ContainsKey(x.Key)).Select(x => x.Key).ToArray();
		Assert.AreEqual(0, missing.Length, string.Join(", ", missing));
		foreach (var key in new[] { "arm.binding.material.wood", "arm.binding.material.clay", "arm.liquid.water", "arm.effect.disease" })
		{
			Assert.AreEqual("ArmageddonCatalogueAlias", claims[key].EntityType, key);
			Assert.IsNull(claims[key].LogicalId, "A native binding must not claim ownership of its target.");
		}
	}

	[DataTestMethod]
	[DataRow("deleted")][DataRow("retired")][DataRow("type")][DataRow("identity")]
	public void CompletePolicy_MissingCanonicalIdentityHoldsWithoutDowngrade(string corruption)
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(factory, completeStock: true).Status);
		db.ChangeTracker.Clear();
		var baseline = db.MagicCapabilities.ToDictionary(x => x.Id, x => x.Definition);
		var claimsBefore = db.SeederManagedRecords.Where(x => x.EntityType == nameof(MagicCapability)).ToDictionary(x => x.Id, x => (x.SeedBaseline, x.AppliedAt));
		var claim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.spell.earthen_ward");
		if (corruption == "deleted") db.MagicSpells.Remove(db.MagicSpells.Single(x => x.Id == claim.LogicalId));
		else if (corruption == "retired") claim.Retired = true;
		else if (corruption == "type") claim.EntityType = nameof(TraitDefinition);
		else db.MagicSpells.Single(x => x.Id == claim.LogicalId).Definition = "<Definition><StockIdentity>arm.spell.wrong</StockIdentity></Definition>";
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		foreach (var cap in db.MagicCapabilities) Assert.AreEqual(baseline[cap.Id], cap.Definition);
		foreach (var record in db.SeederManagedRecords.Where(x => x.EntityType == nameof(MagicCapability)))
			Assert.AreEqual(claimsBefore[record.Id], (record.SeedBaseline, record.AppliedAt));
	}

	[TestMethod]
	public void CompletePolicy_PreservesBuilderDefinitionAndRetainsHydrationOnOrdinaryReconciliation()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(factory, completeStock: true).Status);
		db.ChangeTracker.Clear();
		var cap = db.MagicCapabilities.First(); var editedId = cap.Id;
		var definition = XElement.Parse(cap.Definition); definition.Element("ConcentrationCapabilityExpression")!.Value = "2";
		cap.Definition = definition.ToString(); db.SaveChanges();
		var result = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(definition.ToString(), db.MagicCapabilities.Single(x => x.Id == editedId).Definition);
		Assert.IsTrue(db.MagicCapabilities.AsEnumerable().All(x => XElement.Parse(x.Definition).Element("Casting")!.Elements("Admission").Any()));
		Assert.IsTrue(result.Messages.Any(x => x.Contains("preserved builder edit Definition")));
	}
	[TestMethod]
	public void CompletePolicy_UnknownLaterMarkerVersionRefusesBeforeAnyProgressionWrite()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(factory, completeStock: true).Status);
		db.ChangeTracker.Clear();
		var record = db.SeederManagedRecords.Where(x => x.EntityType == nameof(MagicCapability)).OrderBy(x => x.Id).Last();
		var values = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		values["$completeStockPolicy"] = JsonSerializer.Serialize("unrecognised-future-version");
		record.SeedBaseline = JsonSerializer.Serialize(values); db.SaveChanges();
		var definitions = db.MagicCapabilities.ToDictionary(x => x.Id, x => x.Definition);
		var metadata = db.SeederManagedRecords.ToDictionary(x => x.Id, x => (x.SeedBaseline, x.AppliedAt));
		var result = ArmageddonProgressionInstaller.Install(factory, completeStock: true);
		Assert.AreEqual(ArmageddonInstallStatus.Failed, result.Status);
		StringAssert.Contains(string.Join("\n", result.Messages), "incompatible complete-stock policy version");
		db.ChangeTracker.Clear();
		foreach (var cap in db.MagicCapabilities) Assert.AreEqual(definitions[cap.Id], cap.Definition);
		foreach (var claim in db.SeederManagedRecords) Assert.AreEqual(metadata[claim.Id], (claim.SeedBaseline, claim.AppliedAt));
	}

}
