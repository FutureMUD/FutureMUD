#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonPreparedWorldSeederTests
{
	private static string CapabilityMeritPolicy(FuturemudDatabaseContext db) => JsonSerializer.Serialize(new
	{
		Records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonTraditionInstaller.Module &&
			(x.EntityType == "MagicCapability" || x.EntityType == "Merit")).OrderBy(x => x.StableKey)
			.Select(x => new { x.StableKey, x.LogicalId, x.SeedBaseline, x.AppliedFingerprint }).ToArray(),
		Capabilities = db.MagicCapabilities.AsNoTracking().Where(x => x.Name.StartsWith("Armageddon partial "))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Definition }).ToArray(),
		Merits = db.Merits.AsNoTracking().OrderBy(x => x.Id).Select(x => new { x.Id, x.Definition }).ToArray()
	});

	[DataTestMethod]
	[DataRow("sorcerer")]
	[DataRow("preserver")]
	[DataRow("defiler")]
	public void ExactPierceRemovalSurvivesTwoRerunsAndCompleteStockBaseline(string variant)
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var pierce = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedPierceContent.Key).LogicalId!.Value;
			var cap = db.MagicCapabilities.Single(x => x.Name == "Armageddon partial " + variant);
			var xml = XElement.Parse(cap.Definition);
			xml.Element("Casting")!.Elements("Admission").Single(x => (long?)x.Attribute("spell") == pierce).Remove();
			cap.Definition = xml.ToString(); db.SaveChanges();
			var before = CapabilityMeritPolicy(db);
			for (var i = 0; i < 2; i++)
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(result);
				var stored = result.Availability.Single(x => x.Variant == variant);
				Assert.AreEqual(3, stored.StoredAdmissions.Count);
				Assert.IsFalse(stored.StoredAdmissions.Contains(ArmageddonReviewedPierceContent.Key));
				Assert.AreEqual(xml.ToString(), db.MagicCapabilities.AsNoTracking().Single(x => x.Id == cap.Id).Definition);
				Assert.AreEqual(before, CapabilityMeritPolicy(db), "Bootstrap/final reconciliation advanced capability or merit policy baseline.");
			}
		}
	}

	[DataTestMethod]
	[DataRow(ArmageddonInstallCheckpoint.ContentCreated)]
	[DataRow(ArmageddonInstallCheckpoint.BeforeCommit)]
	public void PierceInterruptionPreservesExistingCapabilityMeritPolicyAndBaseline(ArmageddonInstallCheckpoint boundary)
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var before = CapabilityMeritPolicy(db);
			var stopped = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{
				if (module == ArmageddonPierceInstaller.Module && point == boundary) throw new InvalidOperationException("Bounded Pierce interruption");
			});
			Assert.AreEqual(ArmageddonInstallStatus.Failed, stopped.Status); Assert.AreEqual(3, stopped.Modules.Count);
			Assert.AreEqual(before, CapabilityMeritPolicy(db));
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			Assert.AreEqual(before, CapabilityMeritPolicy(db));
		}
	}

	[TestMethod]
	public void FreshBootstrapCannotCreatePartialCastingAuthorityBeforePierce()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var stopped = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{
				if (module == ArmageddonPierceInstaller.Module && point == ArmageddonInstallCheckpoint.PreflightComplete)
					throw new InvalidOperationException("Before first Pierce content");
			});
			Assert.AreEqual(ArmageddonInstallStatus.Failed, stopped.Status);
			Assert.AreEqual(186, db.SeederManagedRecords.Count());
			Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == "MagicCapability" || x.EntityType == "Merit"));
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			Assert.AreEqual(196, db.SeederManagedRecords.Count());
		}
	}

	[DataTestMethod]
	[DataRow("MagicCapability")]
	[DataRow("Merit")]
	public void BootstrapStillRefusesRetiredCapabilityOrMeritOwnership(string entityType)
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var record = db.SeederManagedRecords.First(x => x.Module == ArmageddonTraditionInstaller.Module && x.EntityType == entityType);
			record.Retired = true; db.SaveChanges();
			var before = CapabilityMeritPolicy(db);
			var stopped = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, stopped.Status); Assert.AreEqual(2, stopped.Modules.Count);
			Assert.AreEqual(before, CapabilityMeritPolicy(db)); Assert.IsTrue(db.SeederManagedRecords.Single(x => x.Id == record.Id).Retired);
		}
	}
}
