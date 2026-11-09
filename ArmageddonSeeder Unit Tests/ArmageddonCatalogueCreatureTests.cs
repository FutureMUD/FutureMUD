#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonCatalogueCreatureTests
{
	[TestMethod]
	[TestCategory("Integration")]
	[DataRow(ArmageddonProgressionMode.Classic, false)][DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)][DataRow(ArmageddonProgressionMode.Simple, true)]
	public void Creatures_RerunPreservesManagedRecordsAndCreatesNoLiveEntities(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var initialClaims = db.SeederManagedRecords.Count();
		var result = ArmageddonCatalogueCreatures.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey,
			x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		Assert.IsTrue(before.Count > initialClaims, "Installation must create its own managed records.");
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueCreatures.Install(factory).Status);
		db.ChangeTracker.Clear();
		Assert.AreEqual(before.Count, db.SeederManagedRecords.Count());
		foreach (var record in db.SeederManagedRecords)
			Assert.AreEqual(before[record.StableKey],
				(record.LogicalId, record.RevisionNumber, record.SeedBaseline, record.AppliedAt), record.StableKey);
		Assert.AreEqual(0, db.GameItems.Count());
		Assert.AreEqual(0, db.Characters.Count());
	}

	[TestMethod]
	public void Creatures_BuilderDefinitionAndAiSelectionArePreserved()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var result = ArmageddonCatalogueCreatures.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status);
		db.ChangeTracker.Clear();
		var template = db.NpcTemplates.Include(x => x.NpctemplatesArtificalIntelligences).Single(x => x.Id == result.Identities["arm.npc.earth_mount.g1"]);
		var xml = XElement.Parse(template.Definition); xml.Element("SelectedFullDesc")!.Value = "Builder-authored creature description.";
		template.Definition = xml.ToString();
		template.Name = "Builder mount";
		var mountAiId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.ai.earth_mount.riding").LogicalId!.Value;
		db.Set<NpcTemplatesArtificalIntelligences>().Remove(template.NpctemplatesArtificalIntelligences.Single(x => x.AiId == mountAiId));
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueCreatures.Install(factory).Status);
		db.ChangeTracker.Clear();
		Assert.AreEqual("Builder mount", db.NpcTemplates.Single(x => x.Id == template.Id).Name);
		Assert.AreEqual("Builder-authored creature description.", XElement.Parse(db.NpcTemplates.Single(x => x.Id == template.Id).Definition).Element("SelectedFullDesc")!.Value);
		Assert.IsFalse(db.Set<NpcTemplatesArtificalIntelligences>().Any(x => x.NpcTemplateId == template.Id && x.AiId == mountAiId));
	}

	[TestMethod]
	public void Creatures_MissingControllerRefusesBeforeTemplateCreation()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var mountAiId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.ai.earth_mount.riding").LogicalId!.Value;
		db.ArtificialIntelligences.Remove(db.ArtificialIntelligences.Single(x => x.Id == mountAiId)); db.SaveChanges();
		Assert.AreNotEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueCreatures.Install(factory).Status);
		Assert.AreEqual(0, db.NpcTemplates.Count());
	}

	[TestMethod]
	public void Creatures_UnownedUniqueNameRefusesAdoption()
	{
		using var db = Fixture();
		db.NpcTemplates.Add(new() { Id = 900, RevisionNumber = 0, Type = "Simple", Name = "Builder creature",
			UniqueName = "arm.npc.earth_mount.g1", Definition = "<Definition/>", EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } });
		db.SaveChanges();
		var result = ArmageddonCatalogueCreatures.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreNotEqual(ArmageddonInstallStatus.Completed, result.Status);
		Assert.IsTrue(result.Messages.Any(x => x.Contains("unique-name collision", StringComparison.Ordinal)));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.npc.earth_mount.g1"));
		Assert.AreEqual("Builder creature", db.NpcTemplates.Single(x => x.Id == 900).Name);
	}

	internal static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false, bool enableStockForNewInstall = false)
	{
		var db = ArmageddonCatalogueCreatureFoundationTests.Fixture(mode, classes, enableStockForNewInstall);
		Assert.AreEqual(ArmageddonInstallStatus.Completed,
			ArmageddonCatalogueCreatureFoundations.Install(ArmageddonInstallationSettingsTests.Factory(db)).Status);
		db.ChangeTracker.Clear();
		var calendar = db.Calendars.Single(); calendar.Date = "1/mon/100";
		calendar.Definition = CalendarDefinition();
		db.Accents.Add(new() { Id = 1, LanguageId = 1, Name = "Native test accent" });
		var types = new[] { CheckType.NaturalWeaponAttack, CheckType.DodgeCheck, CheckType.FlyCheck,
			CheckType.SearchForTracksCheck, CheckType.SearchForTracksByScentScheck };
		var declaredSkills = db.TraitDefinitions.Select(x => x.Id).ToHashSet();
		for (var index = 0; index < types.Length; ++index)
		{
			var id = 500L + Math.Min(index, 3);
			if (declaredSkills.Add(id)) db.TraitDefinitions.Add(new() { Id = id, Name = "Native check skill " + id, Type = 0 });
			var expressionId = 600L + index;
			db.TraitExpressions.Add(new() { Id = expressionId, Name = "Native dependency " + types[index], Expression = $"check:{id} + 147" });
			db.Checks.Add(new() { Type = (int)types[index], TraitExpressionId = expressionId });
		}
		db.SaveChanges(); db.ChangeTracker.Clear();
		var controllers = ArmageddonCatalogueControllers.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, controllers.Status, string.Join("\n", controllers.Messages));
		db.ChangeTracker.Clear();
		return db;
	}

	private static string CalendarDefinition() => new XElement("calendar", new XElement("alias", "test"),
		new XElement("shortname", "Test"), new XElement("fullname", "Test calendar"), new XElement("description", "A test calendar."),
		new XElement("shortstring", "$dd/$mo/$yy"), new XElement("longstring", "$dd/$mo/$yy"), new XElement("wordystring", "$dd/$mo/$yy"),
		new XElement("plane", "earth"), new XElement("weekdayatepoch", 0), new XElement("epochyear", 1),
		new XElement("ancienterashortstring", "B"), new XElement("ancienteralongstring", "Before"),
		new XElement("modernerashortstring", "A"), new XElement("moderneralongstring", "After"),
		new XElement("weekdays", new XElement("weekday", "Day")), new XElement("months", new XElement("month",
			new XElement("alias", "mon"), new XElement("shortname", "mon"), new XElement("fullname", "Month"),
			new XElement("nominalorder", 1), new XElement("normaldays", 30), new XElement("intercalarydays"),
			new XElement("specialdays"), new XElement("nonweekdays"))), new XElement("intercalarymonths")).ToString();
}
