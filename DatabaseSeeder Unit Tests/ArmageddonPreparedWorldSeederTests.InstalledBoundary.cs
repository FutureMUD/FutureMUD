#nullable enable
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonPreparedWorldSeederTests
{
	[TestMethod]
	public void InstalledSenseLeavesNativeVocabularyUnauthoredForTheBuilder()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var id = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedUtilityContent.SenseEnchantmentKey).LogicalId!.Value;
			var source = XElement.Parse(db.MagicSpells.AsNoTracking().Single(x => x.Id == id).Definition);
			Assert.IsFalse(source.Descendants("Incantation").Any(), "Installer must not invent historical category words or select a player's spoken language.");
			Assert.AreEqual(0, db.CharacterAcquiredSpells.Count());
			Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
			Assert.AreEqual(0, db.CharacterTraits.Count());
			Assert.AreEqual(0, db.CharactersMagicResources.Count());
		}
	}

	[TestMethod]
	public void InstalledSenseKeepsExplicitBuilderDependenciesOutsidePackageOwnership()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var template = db.MagicCapabilities.AsNoTracking().Single();
			var resource = db.MagicResources.AsNoTracking().Single();
			var school = db.MagicSchools.AsNoTracking().Single();
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == "MagicCapability" && x.LogicalId == template.Id));
			Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == "MagicResource" && x.LogicalId == resource.Id));
			Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == "MagicSchool" && x.LogicalId == school.Id));
			Assert.AreEqual(template.Definition, db.MagicCapabilities.AsNoTracking().Single(x => x.Id == template.Id).Definition);
			Assert.AreEqual(resource.Definition, db.MagicResources.AsNoTracking().Single(x => x.Id == resource.Id).Definition);
		}
	}

	[TestMethod]
	public void InstalledSenseBareNativeSkillLevelCapabilityCannotSupplyPaidGathering()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var template = db.MagicCapabilities.Single();
			var xml = XElement.Parse(template.Definition); xml.Element("Gathering")!.Remove();
			template.Definition = xml.ToString(); db.SaveChanges();
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status);
			Assert.AreEqual(0, db.CharacterAcquiredSpells.Count());
			Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
			Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == "MagicCapability"));
		}
	}

	[TestMethod]
	public void InstalledSensePaidGatheringDoesNotBecomePassiveReserveRefill()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			foreach (var cap in db.MagicCapabilities.AsNoTracking().Where(x => x.Name.StartsWith("Armageddon partial ")))
			{
				var xml = XElement.Parse(cap.Definition);
				Assert.AreEqual("false", (string?)xml.Element("Casting")!.Attribute("passive"));
				Assert.IsFalse(xml.Element("Regenerators")!.Elements().Any());
				var methods = xml.Element("Gathering")!.Elements("Method").ToArray();
				Assert.AreEqual(1, methods.Length);
				Assert.AreEqual("Self", (string?)methods[0].Attribute("kind"));
				Assert.IsTrue((double?)methods[0].Attribute("stamina") > 0);
			}
			Assert.AreEqual(0, db.CharactersMagicResources.Count());
		}
	}
}
