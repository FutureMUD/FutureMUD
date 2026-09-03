#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class IndustrialisedClothingReuseTests
{
	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void GeneratedIdentity_OutfitsReceiveProvenanceIdsAtFirstItemFlush(bool autoDetectChanges)
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var seeder = SeedOutfitsBeforeFlush(context);
		var originalMetadata = context.SeederManagedRecords.Local.Where(x => x.EntityType == "outfit")
			.ToDictionary(x => x.StableKey, IdentityIndependentMetadata);
		context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChanges;
		typeof(ItemSeeder).GetMethod("SaveItemChangesBeforeCrafting", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, null);
		context.ChangeTracker.Clear();
		foreach (var outfit in context.OutfitTemplates)
		{
			var key = outfit.Name == "Fixture first ensemble" ? "fixture_first" : "fixture_second";
			var record = context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == key);
			Assert.AreEqual(outfit.Id, record.LogicalId,
				"The first installation must not depend on an unchanged rerun to record generated IDs.");
			Assert.AreEqual(originalMetadata[key], IdentityIndependentMetadata(record),
				"Binding a generated ID must not reapply stock, redate ownership or change its fingerprint.");
		}
		Assert.AreEqual(0, PendingIdentityCount(seeder));
	}

	[TestMethod]
	public void GeneratedIdentity_RecipeAccessProgsReceiveProvenanceIdsOnFirstInstallation()
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		InstallClothingCrafts(context, CatalogueWithTwoCrafts());
		context.ChangeTracker.Clear();
		var records = context.SeederManagedRecords.Where(x => x.EntityType == "prog").ToArray();
		Assert.AreEqual(6, records.Length);
		foreach (var record in records)
			Assert.AreEqual(context.FutureProgs.Single(x => x.FunctionName == record.StableKey).Id, record.LogicalId);
	}

	[DataTestMethod]
	[DataRow("temporary")]
	[DataRow("deleted")]
	[DataRow("detached")]
	[DataRow("provenance-id")]
	[DataRow("fingerprint")]
	[DataRow("retired")]
	[DataRow("record-detached")]
	[DataRow("competing-owner")]
	public void GeneratedIdentity_LaterInvalidBindingCannotPartiallyUpdateProvenance(string fault)
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var seeder = SeedOutfitsBeforeFlush(context);
		context.SaveChanges(); // Allocate actual IDs without running the deferred provenance pass yet.
		var later = context.OutfitTemplates.Single(x => x.Name == "Fixture second ensemble");
		var record = context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == "fixture_second");
		switch (fault)
		{
			case "temporary": context.Entry(later).Property(x => x.Id).IsTemporary = true; break;
			case "deleted": context.OutfitTemplates.Remove(later); break;
			case "detached": context.Entry(later).State = EntityState.Detached; break;
			case "provenance-id": record.LogicalId = 987654; break;
			case "fingerprint": record.AppliedFingerprint = "Changed outside the pending identity contract"; break;
			case "retired": record.Retired = true; break;
			case "record-detached": context.Entry(record).State = EntityState.Detached; break;
			case "competing-owner":
				var other = new SeederManagedRecord { Seeder = "Items", EntityType = "outfit", StableKey = "other_owner", LogicalId = later.Id };
				context.SeederManagedRecords.Add(other);
				var index = (Dictionary<string, SeederManagedRecord>)typeof(ItemSeeder)
					.GetField("_managedRecordsByIdentity", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(seeder)!;
				index.Add("outfit\u001fother_owner", other);
				break;
		}
		var first = context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == "fixture_first");
		var before = System.Text.Json.JsonSerializer.Serialize(new[] { first, record });
		var error = Assert.ThrowsException<TargetInvocationException>(() => typeof(ItemSeeder)
			.GetMethod("ReconcileGeneratedManifestIdentities", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, null));
		Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
		Assert.IsNull(first.LogicalId, "The complete pending batch must validate before its first ID is assigned.");
		Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(new[] { first, record }));
		Assert.AreEqual(2, PendingIdentityCount(seeder));
	}

	[TestMethod]
	public void GeneratedIdentity_BindsExactCreatedEntityRatherThanMutableNameOrMarker()
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var seeder = SeedOutfitsBeforeFlush(context);
		var target = context.OutfitTemplates.Local.Single(x => x.Name == "Fixture second ensemble");
		target.Name = "Renamed before the first flush";
		target.Description = "A modified description without any stock marker.";
		typeof(ItemSeeder).GetMethod("SaveManifestChanges", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, null);
		var id = target.Id;
		context.ChangeTracker.Clear();
		Assert.AreEqual(id, context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == "fixture_second").LogicalId);
		Assert.AreEqual("Renamed before the first flush", context.OutfitTemplates.Single(x => x.Id == id).Name);
	}

	[TestMethod]
	public void GeneratedIdentity_FailedProvenanceSaveRetainsPendingBindingsForRetry()
	{
		var interceptor = new FailGeneratedIdentitySaveOnce();
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).AddInterceptors(interceptor).Options);
		SeedSkinPrerequisites(context);
		var seeder = SeedOutfitsBeforeFlush(context);
		interceptor.Armed = true;
		var flush = typeof(ItemSeeder).GetMethod("SaveManifestChanges", BindingFlags.Instance | BindingFlags.NonPublic)!;
		Assert.ThrowsException<TargetInvocationException>(() => flush.Invoke(seeder, null));
		Assert.AreEqual(2, PendingIdentityCount(seeder));
		Assert.IsTrue(context.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == "outfit").All(x => x.LogicalId == null));
		flush.Invoke(seeder, null);
		Assert.AreEqual(0, PendingIdentityCount(seeder));
		Assert.AreEqual(2, context.OutfitTemplates.Count());
		Assert.IsTrue(context.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == "outfit").All(x => x.LogicalId > 0));
		// This proves retained in-memory retry state, not relational transaction rollback.
	}

	private sealed class FailGeneratedIdentitySaveOnce : SaveChangesInterceptor
	{
		public bool Armed { get; set; }
		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			if (Armed && eventData.Context!.ChangeTracker.Entries<SeederManagedRecord>()
				.Any(x => x.State == EntityState.Modified && x.Entity.EntityType == "outfit" && x.Entity.LogicalId > 0))
			{
				Armed = false;
				throw new InvalidOperationException("Injected provenance-save failure.");
			}
			return result;
		}
	}

	private static int PendingIdentityCount(ItemSeeder seeder)
	{
		var pending = typeof(ItemSeeder).GetField("_pendingGeneratedManifestIdentities", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(seeder)!;
		return (int)pending.GetType().GetProperty("Count")!.GetValue(pending)!;
	}

	private static string IdentityIndependentMetadata(SeederManagedRecord record) =>
		System.Text.Json.JsonSerializer.Serialize(new
		{
			record.Seeder, record.EntityType, record.StableKey, record.Module,
			record.AppliedFingerprint, record.AppliedAt, record.Retired
		});

	private static ItemSeeder SeedOutfitsBeforeFlush(FuturemudDatabaseContext context)
	{
		var seeder = new ItemSeeder(CatalogueWithTwoOutfits());
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		seeder.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		typeof(ItemSeeder).GetMethod("SeedIndustrialisedClothingPresentations", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, ["industrial"]);
		return seeder;
	}
}
