#nullable enable

using System;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreTagCatalogueTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);

	[TestMethod]
	public void FreshAndRerun_InstallEveryReviewedPathAndKeepDistinctLeafIdentities()
	{
		using var db = Context();
		var first = CoreTagCatalogue.Ensure(db);
		Assert.AreEqual(2373, CoreTagCatalogue.Declarations.Count);
		Assert.AreEqual(2335, CoreTagCatalogue.Definitions.Count);
		Assert.AreEqual(2335, db.Tags.Count());
		Assert.AreEqual(2147, CoreTagCatalogue.Declarations.Where(x => x.Family.StartsWith("useful")).Select(x => x.Path).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (var definition in CoreTagCatalogue.Definitions)
		{
			Assert.AreEqual(definition.Name, first[definition.Path].Name);
			Assert.AreEqual(definition.ParentPath is { } parent ? first[parent].Id : (long?)null, first[definition.Path].ParentId);
		}
		var ids = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		var second = CoreTagCatalogue.Ensure(db);
		Assert.AreEqual(2335, db.Tags.Count());
		foreach (var definition in CoreTagCatalogue.Definitions) Assert.AreEqual(ids[definition.Key], second[definition.Path].Id);
		Assert.AreNotEqual(first["Materials / Manufactured Materials / Glass"].Id, first["Functions / Repairing / Glass"].Id);
		Assert.AreNotEqual(first["Culture / Renaissance / Shared"].Id, first["Culture / Early Modern / Shared"].Id);
		Assert.AreEqual(0, db.GameItemComponentProtos.Count());
		Assert.AreEqual(0, db.AutobuilderRoomTemplates.Count());
	}

	[TestMethod]
	public void LegacyAndBuilderEdits_KeepIdsParentsJoinsAndImmutableIdentity()
	{
		using var db = Context();
		var root = new Tag { Id = 80, Name = "Functions" };
		var repairing = new Tag { Id = 81, Name = "Repairing", Parent = root };
		var glass = new Tag { Id = 82, Name = "Glass", Parent = repairing };
		db.Tags.AddRange(root, repairing, glass);
		db.SaveChanges();
		var first = CoreTagCatalogue.Ensure(db);
		Assert.AreEqual(82L, first["Functions / Repairing / Glass"].Id);
		var claim = db.SeederManagedRecords.Single(x => x.LogicalId == glass.Id);
		StringAssert.Contains(claim.SeedBaseline!, "\"CreatedByCore\":false");
		glass.Name = "Builder glazing repair";
		glass.Parent = root;
		db.SaveChanges();
		var rerun = CoreTagCatalogue.Ensure(db);
		Assert.AreEqual(82L, rerun["Functions / Repairing / Glass"].Id);
		Assert.AreEqual("Builder glazing repair", glass.Name);
		Assert.AreEqual(root.Id, glass.ParentId);
		Assert.IsTrue(rerun.Messages.Any(x => x.Contains("Preserved builder tag #82")));
		Assert.AreEqual(82L, CoreTagCatalogue.Require(db, "Functions / Repairing / Glass").Id);
	}

	[TestMethod]
	public void ForeignParentAlias_IsReportedAndLeftWithAllExistingReferences()
	{
		using var db = Context();
		var custom = new Tag { Id = 1, Name = "Custom" };
		var foreign = new Tag { Id = 2, Name = "Incense Fuel", Parent = custom };
		db.Tags.AddRange(custom, foreign);
		db.SaveChanges();
		var catalogue = CoreTagCatalogue.Ensure(db);
		Assert.AreNotEqual(foreign.Id, catalogue["Functions / Household Items / Household Religious Items / Incense Fuel"].Id);
		Assert.AreEqual(custom.Id, foreign.ParentId);
		Assert.IsTrue(catalogue.Messages.Any(x => x.Contains("Incense Fuel") && x.Contains("#2")));
	}

	[DataTestMethod]
	[DataRow("duplicate")]
	[DataRow("cycle")]
	[DataRow("orphan")]
	public void AmbiguousLegacyGraph_RefusesBeforeSavingCatalogue(string scenario)
	{
		using var db = Context();
		db.Tags.Add(new Tag { Id = 1, Name = "Functions", ParentId = scenario == "cycle" ? 1 : scenario == "orphan" ? 99 : null });
		if (scenario == "duplicate") db.Tags.Add(new Tag { Id = 2, Name = "Functions" });
		db.SaveChanges();
		var count = db.Tags.Count();
		Assert.ThrowsException<InvalidOperationException>(() => CoreTagCatalogue.Ensure(db));
		Assert.AreEqual(count, db.Tags.Count());
		Assert.AreEqual(0, db.SeederManagedRecords.Count());
	}

	[DataTestMethod]
	[DataRow("retired")]
	[DataRow("type")]
	[DataRow("module")]
	[DataRow("shared-id")]
	public void CorruptClaims_RefuseEnsureAndConsumersBeforeWrites(string scenario)
	{
		using var db = Context();
		db.Tags.Add(new Tag { Id = 1, Name = "Materials" });
		var definition = CoreTagCatalogue.Definitions.First();
		db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = nameof(CoreDataSeeder), Module = scenario == "module" ? "Other" : "TagCatalogue",
			EntityType = scenario == "type" ? nameof(Material) : nameof(Tag), StableKey = definition.Key, LogicalId = 1, Retired = scenario == "retired" });
		if (scenario == "shared-id") db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = nameof(CoreDataSeeder), Module = "TagCatalogue", EntityType = nameof(Tag), StableKey = CoreTagCatalogue.Definitions[1].Key, LogicalId = 1 });
		db.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => CoreTagCatalogue.Ensure(db));
		Assert.ThrowsException<InvalidOperationException>(() => CoreTagCatalogue.Require(db, definition.Path));
		Assert.AreEqual(1, db.Tags.Count());
	}

	[TestMethod]
	public void StaticBindings_BindAllSixAfterIdsAndPreserveCustomValuesAndGuardMaterial()
	{
		using var db = Context();
		var catalogue = CoreTagCatalogue.Ensure(db);
		db.StaticConfigurations.Add(new StaticConfiguration { SettingName = "MusketRamrodTag", Definition = "999999" });
		db.StaticConfigurations.Add(new StaticConfiguration { SettingName = "WadItemTagId", Definition = "0" });
		db.SaveChanges();
		var messages = new System.Collections.Generic.List<string>();
		catalogue.BindSettings(db, messages);
		Assert.AreEqual("999999", db.StaticConfigurations.Find("MusketRamrodTag")!.Definition);
		Assert.IsTrue(messages.Any(x => x.Contains("Preserved MusketRamrodTag")));
		Assert.IsNull(db.StaticConfigurations.Find("GunpowderMaterialId"));
		foreach (var setting in new[] { "MusketCleaningRamrodTag", "MusketUnjammingToolTag", "MusketMatchCordTag", "MusketIgnitionSourceTag", "WadItemTagId" })
			Assert.IsNotNull(db.Tags.Find(long.Parse(db.StaticConfigurations.Find(setting)!.Definition)));
	}

	[TestMethod]
	public void Metadata_RemovesTagOnlyUsefulRequirementsAndKeepsRealOptionalComponents()
	{
		foreach (var seeder in new IDatabaseSeeder[] { new CombatSeeder(), new ArmageddonMagicSeeder(), new EconomySeeder(), new AgricultureSeeder(), new HealthSeeder(), new CookingSeeder(), new RobotSeeder(), new WildlifeCatalogueSeeder() })
			Assert.IsFalse(seeder.Metadata.RequiredSeederTypes.Contains(typeof(UsefulSeeder)), seeder.Name);
		Assert.IsTrue(((IDatabaseSeeder)new ItemSeeder()).Metadata.RequiredSeederTypes.Contains(typeof(UsefulSeeder)));
		Assert.IsTrue(((IDatabaseSeeder)new AnimalButcherySeeder()).Metadata.RequiredSeederTypes.Contains(typeof(UsefulSeeder)));
		using var db = Context();
		var question = new CombatSeeder().SeederQuestions.Single(x => x.Id == "installmuskets");
		Assert.IsFalse(question.Validator("yes", db).Success);
		Assert.IsTrue(question.Validator("no", db).Success);
	}
    [TestMethod]
    public void EarlyFirearms_RequireResolvedSettingsAndRetainValidCustomBindings()
    {
        using var db = Context();
        var catalogue = CoreTagCatalogue.Ensure(db);
        db.GameItemComponentProtos.Add(new GameItemComponentProto { Id = 1, Name = "Approved donor", Type = "Sheath", Description = "Test donor",
            Definition = "<Definition />", EditableItem = new EditableItem { Id = 1, RevisionStatus = 4 } });
        db.SaveChanges();
        var question = new CombatSeeder().SeederQuestions.Single(x => x.Id == "installmuskets");
        StringAssert.Contains(question.Validator("yes", db).error, "MusketRamrodTag");
        var messages = new System.Collections.Generic.List<string>();
        catalogue.BindSettings(db, messages);
        StringAssert.Contains(question.Validator("yes", db).error, "GunpowderMaterialId");
        var powder = new Material { Name = "gunpowder", MaterialDescription = "Test powder", ResidueSdesc = "powder", ResidueDesc = "powder", ResidueColour = "black" };
        db.Materials.Add(powder); db.SaveChanges();
        catalogue.BindSettings(db, messages);
        Assert.IsTrue(question.Validator("yes", db).Success);
        var custom = new Tag { Name = "Builder custom ramrod" };
        db.Tags.Add(custom); db.SaveChanges();
        db.StaticConfigurations.Find("MusketRamrodTag")!.Definition = custom.Id.ToString(); db.SaveChanges();
        catalogue.BindSettings(db, messages);
        Assert.AreEqual(custom.Id.ToString(), db.StaticConfigurations.Find("MusketRamrodTag")!.Definition);
        Assert.IsTrue(question.Validator("yes", db).Success);
        db.StaticConfigurations.Find("WadItemTagId")!.Definition = "999999"; db.SaveChanges();
        StringAssert.Contains(question.Validator("yes", db).error, "WadItemTagId");
    }

}
