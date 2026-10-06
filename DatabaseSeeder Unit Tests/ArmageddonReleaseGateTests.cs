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
	public void EnabledCatalogueAndDependencyPlanMatchBuildAvailability()
	{
		IDatabaseSeeder seeder = new ArmageddonMagicSeeder();
		var catalogue = SeederCatalogue.GetEnabledSeeders();
#if DEBUG
		Assert.IsTrue(seeder.Enabled);
		Assert.AreEqual(1, catalogue.Count(x => x is ArmageddonMagicSeeder));
#else
		Assert.IsFalse(seeder.Enabled);
		Assert.IsFalse(catalogue.Any(x => x is ArmageddonMagicSeeder));
#endif
		Assert.IsFalse(typeof(ArmageddonMagicSeeder).GetProperty(nameof(ArmageddonMagicSeeder.Enabled))!.CanWrite);
		var plan = SeederCatalogue.GetDependencyPlan(catalogue);
		Assert.AreEqual(0, plan.Errors.Count, string.Join("\n", plan.Errors));
		Assert.AreEqual(catalogue.Count, plan.OrderedSeeders.Count);
	}

	[TestMethod]
	public void EnvironmentalExposureOptionalOrderingAcceptsAbsentArmageddon()
	{
		IDatabaseSeeder exposure = new EnvironmentalExposureSeeder();
		Assert.IsTrue(exposure.Metadata.OrderAfterSeederTypes!.Contains(typeof(ArmageddonMagicSeeder)));
		Assert.IsFalse(exposure.Metadata.RequiredSeederTypes.Contains(typeof(ArmageddonMagicSeeder)));
		var catalogue = SeederCatalogue.GetEnabledSeeders().Where(x => x is not ArmageddonMagicSeeder).ToArray();
		var plan = SeederCatalogue.GetDependencyPlan(catalogue);
		Assert.AreEqual(0, plan.Errors.Count, string.Join("\n", plan.Errors));
		var types = plan.OrderedSeeders.Select(x => x.GetType()).ToList();
		Assert.IsFalse(types.Contains(typeof(ArmageddonMagicSeeder)));
		Assert.IsTrue(types.IndexOf(typeof(CoreDataSeeder)) < types.IndexOf(typeof(EnvironmentalExposureSeeder)));
		Assert.IsTrue(types.Contains(typeof(EnvironmentalExposureSeeder)));
	}

	[TestMethod]
	public void MetadataMatchesBuildAvailability()
	{
		var metadata = new ArmageddonMagicSeeder().Metadata;
#if DEBUG
		Assert.AreEqual(SeederRepeatabilityMode.Idempotent, metadata.RepeatabilityMode);
		Assert.AreEqual(SeederUpdateCapability.RepairExisting, metadata.UpdateCapability);
		Assert.IsTrue(metadata.RequiredSeederTypes.Contains(typeof(CoreDataSeeder)));
#else
		Assert.AreEqual(SeederRepeatabilityMode.OneShot, metadata.RepeatabilityMode);
		Assert.AreEqual(SeederUpdateCapability.None, metadata.UpdateCapability);
		Assert.AreEqual(0, metadata.RequiredSeederTypes.Count);
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage, metadata.RerunSummary);
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage, metadata.UpdateSummary);
#endif
	}

	[TestMethod]
	public void QuestionsAndDescriptionsMatchBuildAvailability()
	{
		IDatabaseSeeder seeder = new ArmageddonMagicSeeder();
#if DEBUG
		Assert.AreEqual(2, seeder.Questions.Count());
		Assert.AreEqual(2, seeder.SeederQuestions.Count());
		StringAssert.Contains(seeder.FullDescription, "Development-only");
		Assert.AreEqual("no", seeder.Questions.First().DefaultAnswerResolver!(null!, new Dictionary<string, string>()));
#else
		Assert.AreEqual(0, seeder.Questions.Count());
		Assert.AreEqual(0, seeder.SeederQuestions.Count());
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage, seeder.Tagline);
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage, seeder.FullDescription);
#endif
	}

	[TestMethod]
	public void DirectEntryPointsHonorBuildAvailability()
	{
		IDatabaseSeeder seeder = new ArmageddonMagicSeeder();
#if DEBUG
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase("armageddon_gate_debug_" + Guid.NewGuid().ToString("N")).Options);
		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(db));
		Assert.AreEqual(SeederAssessmentStatus.ReadyToInstall, seeder.AssessSeedData(db).Status);
		StringAssert.Contains(seeder.SeedData(null!, new Dictionary<string, string> { [ArmageddonMagicSeeder.InstallQuestion] = "no" }), "declined");
#else
		// A null context makes any accidental database access fail immediately.
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(null!));
		Assert.AreEqual(SeederAssessmentStatus.Blocked, seeder.AssessSeedData(null!).Status);
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage, seeder.AssessSeedData(null!).Explanation);
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage,
			seeder.SeedData(null!, new Dictionary<string, string> { [ArmageddonMagicSeeder.InstallQuestion] = "yes", [ArmageddonMagicSeeder.BindingsQuestion] = "invalid" }));
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage, seeder.SeedData(null!, null!));
#endif
	}

#if !DEBUG
	[TestMethod]
	public void DisabledDirectCallsPreserveExistingOwnedData()
	{
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase("armageddon_gate_release_" + Guid.NewGuid().ToString("N")).Options);
		var record = new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = "historical", EntityType = "MagicSpell",
			StableKey = "builder-owned-sentinel", LogicalId = 42, AppliedFingerprint = "retained", SeedBaseline = "<Definition>existing</Definition>",
			ManifestVersion = "historical", AppliedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
		db.SeederManagedRecords.Add(record); db.SaveChanges(); db.ChangeTracker.Clear();
		IDatabaseSeeder seeder = new ArmageddonMagicSeeder();
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(db));
		Assert.AreEqual(SeederAssessmentStatus.Blocked, seeder.AssessSeedData(db).Status);
		Assert.AreEqual(ArmageddonMagicSeeder.ReleaseDisabledMessage,
			seeder.SeedData(db, new Dictionary<string, string> { [ArmageddonMagicSeeder.InstallQuestion] = "yes" }));
		Assert.IsFalse(db.ChangeTracker.HasChanges());
		var saved = db.SeederManagedRecords.Single();
		Assert.AreEqual(record.Id, saved.Id); Assert.AreEqual(record.LogicalId, saved.LogicalId);
		Assert.AreEqual(record.SeedBaseline, saved.SeedBaseline); Assert.AreEqual(record.AppliedFingerprint, saved.AppliedFingerprint);
		Assert.AreEqual(record.AppliedAt, saved.AppliedAt); Assert.AreEqual(record.ManifestVersion, saved.ManifestVersion);
		Assert.IsFalse(saved.Retired);
	}
#endif
}
