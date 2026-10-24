#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonStockProgressionTests
{
	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void InstallationMatrix_StagesOwnedNativeFoundations_PreservesRerunAndBuilderEdits(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture(classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		ArmageddonInstallation.Prepare(factory, mode);
		var result = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		Assert.AreEqual(10, db.MagicCapabilities.Count());
		Assert.AreEqual(10, db.Merits.Count());
		Assert.AreEqual(classes ? 10 : 0, db.ChargenRoles.Count());
		Assert.AreEqual(classes ? 10 : 0, db.ChargenRolesMerits.Count());
		Assert.AreEqual(0, db.ChargenRolesTraits.Count());
		Assert.AreEqual(mode == ArmageddonProgressionMode.Classic ? 153 : 11,
			db.TraitDefinitions.Count(x => x.OwnerScope == 1));
		Assert.IsTrue(db.TraitDefinitions.Where(x => x.OwnerScope == 1).All(x => x.BranchMultiplier == 0));
		Assert.AreEqual(8, db.MagicResources.Count(x => x.MagicResourceType == 1));
		Assert.AreEqual(1, db.MagicResources.Count(x => x.MagicResourceType == 0));
		var sourceId = result.Identities[ArmageddonProgressionInstaller.SourceResourceKey];
		Assert.IsTrue(db.MagicCapabilities.AsEnumerable().All(x => (long)XElement.Parse(x.Definition).Element("Casting")!.Attribute("source")! == sourceId));
		Assert.AreEqual(7, db.MagicGenerators.Count());
		Assert.AreEqual(0, db.MagicSpells.Count());
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
		foreach (var cap in db.MagicCapabilities)
		{
			var casting = XElement.Parse(cap.Definition).Element("Casting")!;
			Assert.IsFalse((bool)casting.Attribute("enabled")!);
			Assert.AreEqual(0, casting.Elements("Admission").Count());
			Assert.AreEqual(0, XElement.Parse(cap.Definition).Element("Regenerators")!.Elements().Count());
		}
		var owned = db.SeederManagedRecords.Where(x => x.Module == ArmageddonProgressionInstaller.Module).ToArray();
		var before = owned.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.SeedBaseline, x.AppliedAt));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(factory).Status);
		db.ChangeTracker.Clear();
		foreach (var record in db.SeederManagedRecords.Where(x => x.Module == ArmageddonProgressionInstaller.Module))
			Assert.AreEqual(before[record.StableKey], (record.LogicalId, record.SeedBaseline, record.AppliedAt), record.StableKey);
		var skill = db.TraitDefinitions.First(x => x.OwnerScope == 1);
		skill.Name = "Builder renamed proficiency";
		skill.ChargenBlurb = "Builder customisation";
		if (classes)
		{
			db.ChargenRolesTraits.Add(new ChargenRolesTrait
			{
				ChargenRoleId = db.ChargenRoles.First().Id, TraitId = skill.Id, Amount = 5, GiveIfDoesntHave = true
			});
			db.ChargenRoles.First().ChargenBlurb = "Custom class blurb";
		}
		db.SaveChanges();
		var editedId = skill.Id;
		result = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status);
		db.ChangeTracker.Clear();
		Assert.AreEqual("Builder renamed proficiency", db.TraitDefinitions.Single(x => x.Id == editedId).Name);
		Assert.AreEqual("Builder customisation", db.TraitDefinitions.Single(x => x.Id == editedId).ChargenBlurb);
		if (classes)
		{
			Assert.IsTrue(db.ChargenRolesTraits.Any(x => x.Amount == 5));
			Assert.IsTrue(db.ChargenRoles.Any(x => x.ChargenBlurb == "Custom class blurb"));
		}
		ArmageddonInstallation.Prepare(factory, mode); // Typed preflight recognises the new ownership types.
	}

	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void CompletePolicyProjection_ExactRostersNativeTraitsCapsAndTypedEdges(ArmageddonProgressionMode mode)
	{
		var stock = new ArmageddonStockProgression(mode);
		var traits = stock.Bindings.Select(x => x.TraitKey).Distinct().Select((x, i) => (x, id: 100L + i)).ToDictionary(x => x.x, x => x.id);
		var spells = stock.Capabilities.SelectMany(x => x.SpellKeys).Distinct().Select((x, i) => (x, id: 1000L + i)).ToDictionary(x => x.x, x => x.id);
		var admissions = 0;
		var supports = 0;
		foreach (var cap in stock.Capabilities)
		{
			var xml = stock.Casting(cap, traits, 1, spells);
			Assert.IsTrue((bool)xml.Attribute("enabled")!);
			Assert.IsTrue((bool)xml.Attribute("enrolOnClassSelection")!);
			var own = stock.Bindings.Where(x => x.CapabilityKey == cap.Key).ToArray();
			Assert.AreEqual(cap.SpellKeys.Count, xml.Elements("Admission").Count());
			foreach (var binding in own)
			{
				var node = xml.Elements().Single(x => (Guid)x.Attribute("key")! == ArmageddonStockProgression.Identity(cap.Key, binding.ContentKey));
				Assert.AreEqual(traits[binding.TraitKey], (long)node.Attribute("trait")!);
				Assert.AreEqual(binding.Opening, (double)node.Attribute("opening")!);
				Assert.AreEqual(binding.RawCap, (double)node.Attribute("rawCap")!);
				Assert.AreEqual(binding.Starting, (bool)node.Attribute("starting")!);
				if (!binding.Support) Assert.AreEqual(1, (int)node.Attribute("min")!);
				if (mode == ArmageddonProgressionMode.Simple && !binding.Support)
				{
					Assert.AreEqual(traits[cap.MainTraitKey], (long)node.Attribute("trait")!);
					Assert.AreEqual(binding.MainThreshold, (double?)node.Elements("Prerequisite").First().Attribute("proficiency"));
					Assert.AreEqual("trait", (string?)node.Elements("Prerequisite").First().Attribute("kind"));
				}
			}
			admissions += xml.Elements("Admission").Count();
			supports += xml.Elements("SupportGrant").Count();
			var incomplete = spells.Where(x => x.Key != cap.SpellKeys.Last()).ToDictionary();
			var staged = stock.Casting(cap, traits, 1, incomplete);
			Assert.IsFalse((bool)staged.Attribute("enabled")!);
			Assert.AreEqual(0, staged.Elements().Count());
		}
		Assert.AreEqual(419, admissions);
		Assert.AreEqual(mode == ArmageddonProgressionMode.Classic ? 10 : 20, supports);
		var mend = stock.Bindings.Where(x => x.ContentKey == "arm.spell.mend_flesh").ToArray();
		Assert.IsTrue(mend.Any());
		Assert.AreEqual(mode == ArmageddonProgressionMode.Classic ? 90.0 : 100.0, mend.Single(x => x.CapabilityKey == "arm.capability.water").RawCap);
		Assert.AreEqual(mode == ArmageddonProgressionMode.Classic ? 60.0 : 100.0, mend.Single(x => x.CapabilityKey == "arm.capability.sorcerer").RawCap);
	}

	[TestMethod]
	public void Ownership_UnownedCollisionAndDeletedOwnedSkill_FailWithoutAdoption()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture(false);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple);
		db.MagicCapabilities.Add(new MagicCapability { Name = "Armageddon Earth" });
		db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, ArmageddonProgressionInstaller.Install(factory).Status);
		Assert.AreEqual(1, db.SeederManagedRecords.Count());
		db.MagicCapabilities.Remove(db.MagicCapabilities.Single());
		db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(factory).Status);
		var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.skill.earth_casting");
		db.TraitDefinitions.Remove(db.TraitDefinitions.Single(x => x.Id == record.LogicalId));
		db.SaveChanges();
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, ArmageddonProgressionInstaller.Install(factory).Status);
		Assert.AreEqual(10, db.MagicCapabilities.Count());
	}
}
