#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonPreparedWorldSeederTests
{
	private static ArmageddonPreparedWorldBindings WaterSeeSelections(FuturemudDatabaseContext db,
		ArmageddonPreparedWorldBindings bindings)
	{
		db.Terrains.AddRange(new() { Id = 101, Name = "explicit Silt" }, new() { Id = 102, Name = "explicit Shadow" });
		for (var rank = 0; rank < 5; rank++)
			db.Tags.Add(new() { Id = 201 + rank, Name = "explicit Divination " + rank, ParentId = rank == 0 ? null : 200 + rank });
		db.SaveChanges(); db.ChangeTracker.Clear();
		return bindings with { WaterSee = new([1], 101, 102, [201, 202, 203, 204, 205]) };
	}
	private static (FuturemudDatabaseContext Db, ArmageddonPreparedWorldBindings Bindings) WaterSeeFixture()
	{
		var (db, bindings) = Fixture();
		Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
		bindings = ProvisionSelections(db, bindings);
		bindings = WaterSeeSelections(db, bindings);
		return (db, bindings);
	}
	private static string WaterSeePolicy(FuturemudDatabaseContext db) => JsonSerializer.Serialize(new
	{
		Records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonWaterSeeInstaller.Module)
			.OrderBy(x => x.StableKey).Select(x => new { x.StableKey, x.LogicalId, x.SeedBaseline, x.AppliedFingerprint, x.AppliedAt }).ToArray(),
		Spells = db.MagicSpells.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonWaterSeeInstaller.Module && y.EntityType == "MagicSpell" && y.LogicalId == x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Definition }).ToArray(),
		Expressions = db.TraitExpressions.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonWaterSeeInstaller.Module && y.EntityType == "TraitExpression" && y.LogicalId == x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Expression }).ToArray(),
		Progs = db.FutureProgs.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonWaterSeeInstaller.Module && y.EntityType == "FutureProg" && y.LogicalId == x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.FunctionText }).ToArray()
	});

	[TestMethod]
	public void WaterSeeExplicitSelectionClosesNineAdmissionsOnceWithoutPlayerMutation()
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			var before = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
			var finalReconciles = 0;
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{ if (module == ArmageddonTraditionInstaller.Module + ":admissions" && point == ArmageddonInstallCheckpoint.PreflightComplete) finalReconciles++; });
			Completed(result); Assert.AreEqual(1, finalReconciles); Assert.AreEqual(6, result.Modules.Count);
			Assert.AreEqual(211, db.SeederManagedRecords.Count()); Assert.AreEqual(10, db.MagicSpells.Count());
			Assert.AreEqual(8, result.Modules.Single(x => x.Module == ArmageddonWaterSeeInstaller.Module).Identities.Count);
			Assert.IsTrue(result.Availability.All(x => x.StoredAdmissions.Count == 9 && x.WithoutStoredAdmission.Count == 73));
			Assert.IsTrue(db.SeederManagedRecords.AsNoTracking().AsEnumerable().Where(x => before.ContainsKey(x.StableKey)).All(x => before[x.StableKey] == x.LogicalId));
			foreach (var cap in db.MagicCapabilities.Where(x => x.Name.StartsWith("Armageddon partial ")))
			{
				var admissions = XElement.Parse(cap.Definition).Element("Casting")!.Elements("Admission").ToArray();
				foreach (var (key, parent) in new[] { (ArmageddonWaterSeeInstaller.WaterBreathingKey, ArmageddonReviewedProvisionContent.DrawWineKey),
					(ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey, ArmageddonReviewedPierceContent.Key) })
				{
					long Id(string stableKey) => db.SeederManagedRecords.Single(x => x.StableKey == stableKey).LogicalId!.Value;
					var admission = admissions.Single(x => (long?)x.Attribute("spell") == Id(key));
					Assert.AreEqual(30d, (double)admission.Attribute("opening")!); Assert.AreEqual(90d, (double)admission.Attribute("rawCap")!);
					Assert.AreEqual(Id(key + ".skill"), (long)admission.Attribute("trait")!);
					Assert.AreEqual(Id(parent), (long)admission.Element("Prerequisite")!.Attribute("spell")!);
					Assert.AreEqual(80d, (double)admission.Element("Prerequisite")!.Attribute("proficiency")!);
					Assert.AreEqual(1, (int)admission.Element("Prerequisite")!.Attribute("grade")!);
				}
			}
			Assert.AreEqual(0, db.CharacterTraits.Count()); Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
			Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.CharactersMagicResources.Count()); Assert.AreEqual(0, db.ChargenRoles.Count());
			var policy = WaterSeePolicy(db); var provisions = ProvisionPolicy(db); var capabilities = CapabilityMeritPolicy(db);
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			Assert.AreEqual(policy, WaterSeePolicy(db)); Assert.AreEqual(provisions, ProvisionPolicy(db)); Assert.AreEqual(capabilities, CapabilityMeritPolicy(db));
		}
	}

	[TestMethod]
	public void WaterSeeAbsentWineBlocksBeforeAnyModuleWriteWithoutAdoptingUnownedContent()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			bindings = WaterSeeSelections(db, bindings);
			var fake = ArmageddonReviewedProvisionContent.DrawWine(1).SpellRow(1, 10, 1);
			fake.Definition = "<Definition><StockIdentity>arm.spell.draw_wine</StockIdentity></Definition>";
			db.MagicSpells.Add(fake); db.SaveChanges(); db.ChangeTracker.Clear();
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status); Assert.AreEqual(0, result.Modules.Count);
			Assert.AreEqual(0, db.SeederManagedRecords.Count()); Assert.AreEqual(1, db.MagicSpells.Count());
			StringAssert.Contains(result.Describe(), "owned Draw Wine");
		}
	}

	[TestMethod]
	public void WaterSeeNullPreservesOwnedContentAndCarriesAdmissionsWithoutReconcilingMappings()
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var policy = WaterSeePolicy(db); var provisions = ProvisionPolicy(db); var capabilities = CapabilityMeritPolicy(db);
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { WaterSee = null, Provisions = null });
			Completed(result); Assert.AreEqual(4, result.Modules.Count);
			Assert.IsFalse(result.Modules.Any(x => x.Module == ArmageddonWaterSeeInstaller.Module));
			Assert.IsTrue(result.Availability.All(x => x.StoredAdmissions.Count == 9));
			Assert.AreEqual(policy, WaterSeePolicy(db)); Assert.AreEqual(provisions, ProvisionPolicy(db)); Assert.AreEqual(capabilities, CapabilityMeritPolicy(db));
		}
	}

	[DataTestMethod]
	[DataRow("sorcerer", ArmageddonWaterSeeInstaller.WaterBreathingKey)]
	[DataRow("preserver", ArmageddonWaterSeeInstaller.WaterBreathingKey)]
	[DataRow("defiler", ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey)]
	public void WaterSeeBuilderAdmissionRemovalSurvivesSelectedAndNullReruns(string variant, string key)
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var spell = db.SeederManagedRecords.Single(x => x.StableKey == key).LogicalId!.Value;
			var cap = db.MagicCapabilities.Single(x => x.Name == "Armageddon partial " + variant); var xml = XElement.Parse(cap.Definition);
			xml.Element("Casting")!.Elements("Admission").Single(x => (long?)x.Attribute("spell") == spell).Remove();
			cap.Definition = xml.ToString(); db.SaveChanges(); var policy = CapabilityMeritPolicy(db);
			foreach (var selection in new[] { bindings, bindings with { WaterSee = null, Provisions = null } })
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), selection); Completed(result);
				var actual = result.Availability.Single(x => x.Variant == variant);
				Assert.AreEqual(8, actual.StoredAdmissions.Count); Assert.IsFalse(actual.StoredAdmissions.Contains(key));
				Assert.AreEqual(policy, CapabilityMeritPolicy(db));
			}
		}
	}

	[TestMethod]
	public void WaterSeeOldBindingJsonRemainsCompatibleAndNewSelectionRemainsStrict()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var old = JsonNode.Parse(ArmageddonMagicSeeder.SerializeBindings(bindings))!.AsObject(); old.Remove("WaterSee");
			Assert.IsTrue(ArmageddonMagicSeeder.ValidateBindings(old.ToJsonString(), db).Success);
			bindings = WaterSeeSelections(db, bindings);
			var selected = JsonNode.Parse(ArmageddonMagicSeeder.SerializeBindings(bindings))!.AsObject();
			selected["WaterSee"]!["GuessedTerrain"] = 101;
			Assert.IsFalse(ArmageddonMagicSeeder.ValidateBindings(selected.ToJsonString(), db).Success);
			Assert.AreEqual(0, db.SeederManagedRecords.Count());
		}
	}

	[TestMethod]
	public void WaterSeeComposerFreezesCallerMappingsBeforeEarlierModuleCallbacks()
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			var water = new List<long> { 1 }; var ranks = new List<long> { 201, 202, 203, 204, 205 };
			bindings = bindings with { WaterSee = new(water, 101, 102, ranks) };
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{ if (module == ArmageddonMagicInstaller.Module && point == ArmageddonInstallCheckpoint.PreflightComplete) { water[0] = 999; ranks.Reverse(); } });
			Completed(result);
			var spell = db.MagicSpells.Single(x => db.SeederManagedRecords.Any(r => r.StableKey == ArmageddonWaterSeeInstaller.WaterBreathingKey && r.LogicalId == x.Id));
			Assert.AreEqual(1L, (long)XElement.Parse(spell.Definition).Descendants("WaterScope").Single().Element("Liquid")!.Attribute("id")!);
		}
	}

	[TestMethod]
	public void WaterSeeAfterCommitLostAcknowledgementKeepsPreviousPolicyAndResumesSameEightIds()
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { WaterSee = null }));
			var previous = CapabilityMeritPolicy(db);
			var stopped = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{ if (module == ArmageddonWaterSeeInstaller.Module && point == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("Lost Water/See acknowledgement"); });
			Assert.AreEqual(ArmageddonInstallStatus.CommittedConfirmationFailed, stopped.Status);
			Assert.AreEqual(previous, CapabilityMeritPolicy(db)); Assert.AreEqual(211, db.SeederManagedRecords.Count());
			var ids = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
			var resumed = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(resumed);
			Assert.IsTrue(resumed.Availability.All(x => x.StoredAdmissions.Count == 9));
			Assert.IsTrue(db.SeederManagedRecords.AsNoTracking().AsEnumerable().All(x => ids[x.StableKey] == x.LogicalId));
		}
	}
}
