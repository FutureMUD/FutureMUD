#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederAutobuilderTests
{
	private static Dictionary<string, string> BuildAutobuilderOnlyAnswers()
	{
		return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["ai"] = "no",
			["covers"] = "no",
			["items"] = "no",
			["modernitems"] = "no",

			["autobuilder"] = "yes",
			["hints"] = "no",
			["dreams"] = "no"
		};
	}

    [TestMethod]
    public void GeneratedTemplates_BindCanonicalCoreIdsAndPreserveUnownedOrEditedLegacyTemplates()
    {
        using var context = TerrainSeederFixture.CreateSeeded();
        var seeder = new UsefulSeeder();
        seeder.SeedTerrainAutobuilderForTesting(context);
        var room = context.AutobuilderRoomTemplates.Single(x => x.Name == UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single());
        var xml = XElement.Parse(room.Definition);
        var binding = xml.Element("FrameworkTagBindings")!.Elements("Binding").Single(x => (string?)x.Attribute("selector") == "Mineral Resource");
        var definition = CoreTagCatalogue.Declarations.Single(x => x.Family == "useful_autobuilder" && x.Name == "Mineral Resource");
        Assert.AreEqual(CoreTagCatalogue.Require(context, definition.Path).Id.ToString(), binding.Attribute("id")!.Value);
        Assert.IsFalse(xml.Element("FrameworkTagBindings")!.Elements("Binding").Any(x => (string?)x.Attribute("selector") == "Physical Primary"));
        var edited = "<Template><BuilderEdit>retained</BuilderEdit></Template>";
        room.Definition = edited;
        context.SaveChanges();
        seeder.SeedTerrainAutobuilderForTesting(context);
        Assert.AreEqual(edited, room.Definition);
        context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords.Where(x => x.Seeder == nameof(UsefulSeeder) && x.Module == "Autobuilder"));
        context.SaveChanges();
        seeder.SeedTerrainAutobuilderForTesting(context);
        Assert.AreEqual(edited, room.Definition);
    }

	[TestMethod]
	public void ClassifyAutobuilderPackagePresence_NonePartialAndFull_ReturnExpectedStates()
	{
		using var context = TerrainSeederFixture.CreateSeeded();
		UsefulSeeder seeder = new();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
		{
			Id = 100000,
			Name = UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single(),
			TemplateType = "room random description",
			Definition = "<Template />"
		});
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		seeder.SeedTerrainAutobuilderForTesting(context);

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));
	}

	[TestMethod]
	public void SeederQuestions_AutobuilderQuestionAppearsOnlyWhenTerrainCatalogueExistsAndPackageIncomplete()
	{
		using var context = TerrainSeederFixture.CreateEmpty();
		TerrainSeederFixture.SeedAccount(context);
		UsefulSeeder seeder = new();
		var question = seeder.SeederQuestions.Single(x => x.Id == "autobuilder");

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>()));

		TerrainSeederFixture.SeedTerrainFoundations(context);

		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>()));

		context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
		{
			Id = 100001,
			Name = UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single(),
			TemplateType = "room random description",
			Definition = "<Template />"
		});
		context.SaveChanges();

		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>()),
			"Partial autobuilder installs should still offer the question so reruns can repair the package.");

		seeder.SeedTerrainAutobuilderForTesting(context);

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>()));
	}

	[TestMethod]
	public void SeedData_AutobuilderOnlyAnswers_InstallAndRerunWithoutDuplicateIds()
	{
		using var context = TerrainSeederFixture.CreateSeeded(withAccount: true);
		var seeder = new UsefulSeeder();
		var prior0 = context.AutobuilderRoomTemplates.Select(x => x.Id).ToArray();
		var prior1 = context.AutobuilderAreaTemplates.Select(x => x.Id).ToArray();
		seeder.SeedData(context, BuildAutobuilderOnlyAnswers());
		var installed0 = context.AutobuilderRoomTemplates.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		var installed1 = context.AutobuilderAreaTemplates.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed1.Except(prior1).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedData(context, BuildAutobuilderOnlyAnswers());
		CollectionAssert.AreEqual(installed0, context.AutobuilderRoomTemplates.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed1, context.AutobuilderAreaTemplates.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

    [TestMethod]
    public void AutobuilderRerun_DoesNotRepairMissingCoreTagOrRebindMissingOwnedTemplate()
    {
        using var context = TerrainSeederFixture.CreateSeeded(withAccount: true);
        var seeder = new UsefulSeeder();
        seeder.SeedTerrainAutobuilderForTesting(context);
        var room = context.AutobuilderRoomTemplates.Single(x => x.Name == UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single());
        room.Definition = "<Template><Descriptions /></Template>";
        var area = context.AutobuilderAreaTemplates.Single(x => x.Name == UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting.Single());
        context.AutobuilderAreaTemplates.Remove(area);
        var tag = context.Tags.Single(x => x.Name == UsefulSeeder.StockAutobuilderTagNamesForTesting.Last());
        context.Tags.Remove(tag);
        context.SaveChanges();
        var error = Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedTerrainAutobuilderForTesting(context));
        StringAssert.Contains(error.Message, "missing");
        Assert.IsFalse(context.Tags.Any(x => x.Id == tag.Id));
        Assert.AreEqual("<Template><Descriptions /></Template>", room.Definition);
        Assert.IsFalse(context.AutobuilderAreaTemplates.Any(x => x.Id == area.Id || x.Name == UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting.Single()));
        context.Tags.Add(tag); context.SaveChanges();
        error = Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedTerrainAutobuilderForTesting(context));
        StringAssert.Contains(error.Message, "Missing owned autobuilder template");
        Assert.IsFalse(context.AutobuilderAreaTemplates.Any(x => x.Id == area.Id || x.Name == UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting.Single()));
    }

    [TestMethod]
    public void AutobuilderRerun_RetainsRenamedRoomAndAreaIdsAndRejectsRetiredOwnership()
    {
        using var context = TerrainSeederFixture.CreateSeeded(withAccount: true);
        var seeder = new UsefulSeeder();
        seeder.SeedTerrainAutobuilderForTesting(context);
        var room = context.AutobuilderRoomTemplates.Single(x => x.Name == UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single());
        var area = context.AutobuilderAreaTemplates.Single(x => x.Name == UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting.Single());
        var ids = (room.Id, area.Id);
        room.Name = "Builder room"; area.Name = "Builder area";
        context.SaveChanges();
        seeder.SeedTerrainAutobuilderForTesting(context);
        Assert.AreEqual(1, context.AutobuilderRoomTemplates.Count(x => x.Id == ids.Item1 || x.Name == UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single()));
        Assert.AreEqual(1, context.AutobuilderAreaTemplates.Count(x => x.Id == ids.Item2 || x.Name == UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting.Single()));
        Assert.AreEqual(ids, (room.Id, area.Id));
        Assert.AreEqual("Builder room", room.Name);
        Assert.AreEqual("Builder area", area.Name);
        foreach (var type in new[] { nameof(AutobuilderRoomTemplate), nameof(AutobuilderAreaTemplate) })
        {
            var record = context.SeederManagedRecords.Single(x => x.EntityType == type);
            record.Retired = true; context.SaveChanges();
            StringAssert.Contains(Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedTerrainAutobuilderForTesting(context)).Message, "Invalid autobuilder ownership");
            record.Retired = false; context.SaveChanges();
        }
        context.AutobuilderRoomTemplates.Remove(room); context.SaveChanges();
        StringAssert.Contains(Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedTerrainAutobuilderForTesting(context)).Message, "Missing owned autobuilder template");
        Assert.IsFalse(context.AutobuilderRoomTemplates.Any(x => x.Id == room.Id || x.Name == UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single()));
    }
}
