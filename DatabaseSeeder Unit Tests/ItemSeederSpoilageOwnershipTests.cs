#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using System;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederSpoilageOwnershipTests
{
	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Spoilage_FirstFlushBindsGeneratedIdentity(bool autoDetect)
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var seeder = new ItemSeeder();
		var rule = seeder.InstallCatalogueSpoilageForTesting(context, flush: false);
		var record = context.SeederManagedRecords.Local.Single(x => x.EntityType == "commodity-spoilage");
		Assert.IsNull(record.LogicalId, "The CLR key must not be recorded before the normal database flush.");
		var appliedAt = record.AppliedAt;
		var fingerprint = record.AppliedFingerprint;
		context.ChangeTracker.AutoDetectChangesEnabled = autoDetect;
		typeof(ItemSeeder).GetMethod("SaveManifestChanges", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(seeder, null);
		Assert.IsTrue(rule.Id > 0);
		Assert.AreEqual(rule.Id, record.LogicalId);
		Assert.AreEqual(appliedAt, record.AppliedAt);
		Assert.AreEqual(fingerprint, record.AppliedFingerprint);
	}

	[TestMethod]
	public void Spoilage_LegacyZeroIdentityRepairsOnlyTheUntouchedBinding()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var rule = new ItemSeeder().InstallCatalogueSpoilageForTesting(context);
		var record = context.SeederManagedRecords.Single(x => x.EntityType == "commodity-spoilage");
		var appliedAt = record.AppliedAt;
		var fingerprint = record.AppliedFingerprint;
		record.LogicalId = 0;
		context.SaveChanges();
		new ItemSeeder().InstallCatalogueSpoilageForTesting(context);
		Assert.AreEqual(rule.Id, record.LogicalId);
		Assert.AreEqual(appliedAt, record.AppliedAt);
		Assert.AreEqual(fingerprint, record.AppliedFingerprint);
		Assert.AreEqual(1, context.CommoditySpoilageRules.Count());
	}

	[TestMethod]
	[DataRow("customized")]
	[DataRow("missing")]
	[DataRow("competing-owner")]
	[DataRow("negative-id")]
	[DataRow("negative-missing")]
	[DataRow("negative-renamed")]
	[DataRow("null-missing")]
	[DataRow("null-renamed")]
	public void Spoilage_UnprovenLegacyOwnershipFailsBeforeMutation(string fault)
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var rule = new ItemSeeder().InstallCatalogueSpoilageForTesting(context);
		var record = context.SeederManagedRecords.Single(x => x.EntityType == "commodity-spoilage");
		record.LogicalId = fault.StartsWith("negative", StringComparison.Ordinal) ? -1 :
			fault.StartsWith("null", StringComparison.Ordinal) ? null : 0;
		if (fault == "customized") rule.Description = "Builder's retained spoilage rule";
		if (fault.Contains("missing", StringComparison.Ordinal)) context.CommoditySpoilageRules.Remove(rule);
		if (fault.Contains("renamed", StringComparison.Ordinal)) rule.Name = "Builder's renamed rule";
		if (fault == "competing-owner") context.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Seeder = "Items", EntityType = record.EntityType, StableKey = "Another rule", Module = "antiquity",
			LogicalId = rule.Id, AppliedFingerprint = record.AppliedFingerprint, ManifestVersion = record.ManifestVersion
		});
		context.SaveChanges();
		var priorId = record.LogicalId;
		var priorCount = context.CommoditySpoilageRules.Count();
		Assert.ThrowsExactly<InvalidOperationException>(() => new ItemSeeder().InstallCatalogueSpoilageForTesting(context));
		Assert.AreEqual(priorId, record.LogicalId);
		Assert.AreEqual(priorCount, context.CommoditySpoilageRules.Count());
		Assert.IsFalse(context.CommoditySpoilageRules.Local.Any(x => context.Entry(x).State == Microsoft.EntityFrameworkCore.EntityState.Added));
		if (fault == "customized") Assert.AreEqual("Builder's retained spoilage rule", rule.Description);
	}

	[TestMethod]
	public void Spoilage_UnrelatedAddedTargetCannotBeAdopted()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var rule = new CommoditySpoilageRule { Name = "Fixture spoilage", Description = "Builder's pending rule" };
		context.CommoditySpoilageRules.Add(rule);
		Assert.ThrowsExactly<InvalidOperationException>(() => new ItemSeeder().InstallCatalogueSpoilageForTesting(context));
		Assert.AreEqual("Builder's pending rule", rule.Description);
		Assert.AreEqual(0, context.SeederManagedRecords.Local.Count);
	}

	[TestMethod]
	public void Spoilage_ManagedRenamePreservesItsTargetAndBlocksACompetingName()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var rule = new ItemSeeder().InstallCatalogueSpoilageForTesting(context);
		var id = rule.Id;
		rule.Name = "Builder's renamed rule";
		context.SaveChanges();
		new ItemSeeder().InstallCatalogueSpoilageForTesting(context);
		Assert.AreEqual(id, context.CommoditySpoilageRules.Single().Id);
		Assert.AreEqual("Builder's renamed rule", rule.Name);
		context.CommoditySpoilageRules.Add(new CommoditySpoilageRule { Name = "Fixture spoilage", Description = "A conflicting target" });
		context.SaveChanges();
		Assert.ThrowsExactly<InvalidOperationException>(() => new ItemSeeder().InstallCatalogueSpoilageForTesting(context));
		Assert.AreEqual(2, context.CommoditySpoilageRules.Count());
		Assert.AreEqual("Builder's renamed rule", rule.Name);
	}
}
