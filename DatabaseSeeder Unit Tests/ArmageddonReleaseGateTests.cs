#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonReleaseGateTests
{
	[TestMethod]
	public void EnabledCatalogueAndDependencyPlanAreAvailableInDebugAndRelease()
	{
		IDatabaseSeeder seeder = new ArmageddonSettingSeeder();
		var catalogue = SeederCatalogue.GetEnabledSeeders();
		Assert.IsTrue(seeder.Enabled);
		Assert.AreEqual(1, catalogue.Count(x => x is ArmageddonSettingSeeder));
		Assert.IsFalse(typeof(ArmageddonSettingSeeder).GetProperty(nameof(ArmageddonSettingSeeder.Enabled))!.CanWrite);
		var plan = SeederCatalogue.GetDependencyPlan(catalogue);
		Assert.AreEqual(0, plan.Errors.Count, string.Join("\n", plan.Errors));
		Assert.AreEqual(catalogue.Count, plan.OrderedSeeders.Count);
	}

	[TestMethod]
	public void EnvironmentalExposureOptionalOrderingAcceptsAbsentArmageddon()
	{
		IDatabaseSeeder exposure = new EnvironmentalExposureSeeder();
		Assert.IsTrue(exposure.Metadata.OrderAfterSeederTypes!.Contains(typeof(ArmageddonSettingSeeder)));
		Assert.IsFalse(exposure.Metadata.RequiredSeederTypes.Contains(typeof(ArmageddonSettingSeeder)));
		var catalogue = SeederCatalogue.GetEnabledSeeders().Where(x => x is not ArmageddonSettingSeeder).ToArray();
		var plan = SeederCatalogue.GetDependencyPlan(catalogue);
		Assert.AreEqual(0, plan.Errors.Count, string.Join("\n", plan.Errors));
		var types = plan.OrderedSeeders.Select(x => x.GetType()).ToList();
		Assert.IsFalse(types.Contains(typeof(ArmageddonSettingSeeder)));
		Assert.IsTrue(types.IndexOf(typeof(CoreDataSeeder)) < types.IndexOf(typeof(EnvironmentalExposureSeeder)));
		Assert.IsTrue(types.Contains(typeof(EnvironmentalExposureSeeder)));
	}

	[TestMethod]
	public void MetadataMatchesBuildAvailability()
	{
		var metadata = new ArmageddonSettingSeeder().Metadata;
		Assert.AreEqual(SeederRepeatabilityMode.Idempotent, metadata.RepeatabilityMode);
		Assert.AreEqual(SeederUpdateCapability.RepairExisting, metadata.UpdateCapability);
		Assert.IsTrue(metadata.RequiredSeederTypes.Contains(typeof(CoreDataSeeder)));
	}

	[TestMethod]
	public void QuestionsAndDescriptionsAreAvailableInDebugAndRelease()
	{
		IDatabaseSeeder seeder = new ArmageddonSettingSeeder();
		Assert.AreEqual(6, seeder.Questions.Count());
		Assert.AreEqual(6, seeder.SeederQuestions.Count());
		CollectionAssert.AreEqual(new[] { ArmageddonSettingSeeder.FullInstallQuestion, ArmageddonSettingSeeder.ModeQuestion,
			ArmageddonSettingSeeder.CapacityQuestion, ArmageddonSettingSeeder.NativeBindingsQuestion, ArmageddonSettingSeeder.InstallQuestion,
			ArmageddonSettingSeeder.BindingsQuestion }, seeder.Questions.Select(x => x.Id).ToArray());
		Assert.IsFalse(string.IsNullOrWhiteSpace(seeder.FullDescription));
		Assert.AreEqual("no", seeder.Questions.First().DefaultAnswerResolver!(null!, new Dictionary<string, string>()));
	}

	[TestMethod]
	public void DirectEntryPointsHonorBuildAvailability()
	{
		IDatabaseSeeder seeder = new ArmageddonSettingSeeder();
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase("armageddon_gate_debug_" + Guid.NewGuid().ToString("N")).Options);
		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(db));
		Assert.AreEqual(SeederAssessmentStatus.Blocked, seeder.AssessSeedData(db).Status);
		StringAssert.Contains(seeder.SeedData(null!, new Dictionary<string, string> { [ArmageddonSettingSeeder.InstallQuestion] = "no" }), "declined");
	}

	[TestMethod]
	public void FreshOptInAvailabilityIsDurableAndExistingSettingsAreNotActivated()
	{
		using var fresh = ArmageddonInstallationSettingsTests.Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(fresh);
		Assert.IsTrue(ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple, enableStockForNewInstall: true).StockAvailabilityByDefault);
		Assert.IsTrue(ArmageddonInstallation.Prepare(factory).StockAvailabilityByDefault);
		var staged = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, staged.Status);
		fresh.ChangeTracker.Clear();
		// A partial installation never exposes Classes or casting before real admissions exist.
		Assert.IsTrue(fresh.ChargenRoles.All(x => x.AvailabilityProgId == 2));
		Assert.IsTrue(fresh.MagicCapabilities.AsEnumerable().All(x => !(bool)System.Xml.Linq.XElement.Parse(x.Definition).Element("Casting")!.Attribute("enabled")!));

		using var legacy = ArmageddonInstallationSettingsTests.Fixture();
		var legacyFactory = ArmageddonInstallationSettingsTests.Factory(legacy);
		ArmageddonInstallation.Prepare(legacyFactory);
		var before = legacy.SeederManagedRecords.AsNoTracking().Single().SeedBaseline;
		Assert.IsFalse(before!.Contains("StockAvailabilityByDefault"));
		Assert.IsFalse(ArmageddonInstallation.Prepare(legacyFactory, enableStockForNewInstall: true).StockAvailabilityByDefault);
		Assert.AreEqual(before, legacy.SeederManagedRecords.AsNoTracking().Single().SeedBaseline);
	}

}
