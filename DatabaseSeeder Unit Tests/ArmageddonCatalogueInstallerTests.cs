#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Health;
using MudSharp.Models;
using Material = MudSharp.Models.Material;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonCatalogueInstallerTests
{
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void Composite_CompleteFixedDirectionRetainsAllIdentitiesOnRerun(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var installed = ArmageddonCatalogueInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		var catalogue = new ArmageddonStockCatalogue();
		foreach (var blueprint in catalogue.Blueprints)
			Assert.AreEqual(!blueprint.DeferredToS4, db.SeederManagedRecords.Any(x => x.StableKey == blueprint.Key), blueprint.Key);
		Assert.AreEqual(49, db.NpcTemplates.Count());
		Assert.AreEqual(10, db.MagicCapabilities.Count());
		Assert.AreEqual(classes ? 10 : 0, db.ChargenRoles.Count());
		Assert.AreEqual(0, db.MagicSpells.Count());
		Assert.AreEqual(0, db.Characters.Count());
		Assert.AreEqual(0, db.GameItems.Count());
		var identities = db.SeederManagedRecords.ToDictionary(x => x.StableKey,
			x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		var rerun = ArmageddonCatalogueInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(identities.Count, db.SeederManagedRecords.Count());
		foreach (var row in db.SeederManagedRecords)
			Assert.AreEqual(identities[row.StableKey], (row.LogicalId, row.RevisionNumber, row.SeedBaseline, row.AppliedAt), row.StableKey);
	}

	[TestMethod]
	public void Composite_LaterFailureResumesAndPreservesBuilderMedicalAndControllerEdits()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var stopped = ArmageddonCatalogueInstaller.Install(factory, (batch, stage) =>
		{
			if (batch == "specialised-stock" && stage == ArmageddonInstallCheckpoint.PreflightComplete)
				throw new InvalidOperationException("Injected later batch failure");
		});
		Assert.AreEqual(ArmageddonInstallStatus.Failed, stopped.Status);
		db.ChangeTracker.Clear();
		Assert.AreEqual(49, db.NpcTemplates.Count());
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.item.restorative_mud"));
		var resumed = ArmageddonCatalogueInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, resumed.Status, string.Join("\n", resumed.Messages));
		db.ChangeTracker.Clear();
		var poison = db.Drugs.Single(x => x.Id == resumed.Identities["arm.effect.poison"]);
		poison.IntensityPerGram = 17;
		var guardian = db.ArtificialIntelligences.Single(x => x.Id == resumed.Identities["arm.ai.air_guardian"]);
		guardian.Definition = "<Definition><Mode>companion</Mode><Purpose>owned_guard</Purpose><Permitted><Order>stop</Order></Permitted><LossChanceCap>0.1</LossChanceCap></Definition>";
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueInstaller.Install(factory).Status);
		db.ChangeTracker.Clear();
		Assert.AreEqual(17.0, db.Drugs.Single(x => x.Id == poison.Id).IntensityPerGram);
		Assert.AreEqual(guardian.Definition, db.ArtificialIntelligences.Single(x => x.Id == guardian.Id).Definition);
	}

	[TestMethod]
	public void SpecialStock_UsesNativePhysicalDoseAndExplicitInfectionAlias()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(factory).Status);
		var special = ArmageddonCatalogueSpecials.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, special.Status, string.Join("\n", special.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(8, special.Identities.Count);
		var mudId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.restorative_mud.component").LogicalId!.Value;
		var mud = XElement.Parse(db.GameItemComponentProtos.Single(x => x.Id == mudId).Definition);
		Assert.AreEqual(1.0, (double)mud.Element("TotalGrams")!);
		Assert.AreEqual(1.0, (double)mud.Element("NativeHealingDose")!.Attribute("grams")!);
		Assert.AreEqual(10.0, (double)mud.Element("NativeHealingDose")!.Attribute("budgetPerGrade")!);
		Assert.AreEqual(0, mud.Element("Drugs")!.Elements().Count());
		Assert.AreEqual(1.0, db.GameItemProtos.Single(x => x.Id == special.Identities["arm.item.restorative_mud"]).Weight);
		var poison = db.Drugs.Single(x => x.Id == special.Identities["arm.effect.poison"]);
		Assert.AreEqual((int)DrugVector.Touched, poison.DrugVectors);
		Assert.AreEqual((long)InfectionType.SownSickness, special.Identities["arm.effect.disease"]);
		var disease = db.SeederManagedRecords.Single(x => x.StableKey == "arm.effect.disease");
		Assert.IsNull(disease.LogicalId);
		StringAssert.Contains(disease.SeedBaseline!, nameof(InfectionType));
	}

    [TestMethod]
    public void SpecialStock_HumanOnlyHeadBackFallbackRetainsIdAfterRenameAndOptionalAnimalHead()
    {
        using var db = Fixture();
        db.BodypartShapes.RemoveRange(db.BodypartShapes.Where(x => x.Name == "head"));
        var head = new BodypartShape { Name = "Head Back" };
        db.BodypartShapes.Add(head); db.SaveChanges();
        var factory = ArmageddonInstallationSettingsTests.Factory(db);
        Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(factory).Status);
        var result = ArmageddonCatalogueSpecials.Install(factory);
        Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("; ", result.Messages));
        db.ChangeTracker.Clear();
        Assert.AreEqual(head.Id, ArmageddonCatalogueSpecials.ResolveHeadShape(db));
        db.BodypartShapes.Single(x => x.Id == head.Id).Name = "Builder retained attachment";
        db.BodypartShapes.Add(new BodypartShape { Name = "Head" }); db.SaveChanges();
        Assert.AreEqual(head.Id, ArmageddonCatalogueSpecials.ResolveHeadShape(db));
        Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSpecials.Install(factory).Status);
        db.ChangeTracker.Clear();
        var wearId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.hovering_light.wear_profile").LogicalId;
        Assert.AreEqual(head.Id, (long)XElement.Parse(db.WearProfiles.Single(x => x.Id == wearId).WearlocProfiles).Element("Shape")!.Attribute("ShapeId")!);
    }

	internal static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false, bool enableStockForNewInstall = false)
	{
		var db = ArmageddonCatalogueCreatureTests.Fixture(mode, classes, enableStockForNewInstall);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db)).Status);
		db.Materials.AddRange(new Material { Id = 3, Name = "clay", BehaviourType = (int)MaterialBehaviourType.Soil, Density = 1.21 },
			new Material { Id = 4, Name = "food", BehaviourType = (int)MaterialBehaviourType.Food, Density = 1000 });
		if (!db.BodypartShapes.Any(x => x.Name == "head")) db.BodypartShapes.Add(new() { Name = "head" });
		db.StackDecorators.Add(new() { Id = 1, Name = "Suffix", Type = "Suffix", Definition = "<Definition/>" });
		db.ForagableProfiles.Add(new() { Id = 1, RevisionNumber = 0, Name = "Finite clay",
			EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } });
		db.ForagableProfilesMaximumYields.Add(new() { ForagableProfileId = 1, ForagableProfileRevisionNumber = 0, ForageType = "clay", Yield = 10 });
		db.Terrains.AddRange(new Terrain { Id = 1, Name = "Hallway", ForagableProfileId = 1 }, new Terrain { Id = 2, Name = "Cave" });
		foreach (var name in new[] { "Knife", "Longsword", "Club", "Improvised Bludgeon", "Short Spear" })
			db.WeaponTypes.Add(new() { Name = name, AttackTraitId = db.Languages.Single().LinkedTraitId, ParryTraitId = db.Languages.Single().LinkedTraitId, Reach = 1 });
		db.SaveChanges(); db.ChangeTracker.Clear(); return db;
	}
}
