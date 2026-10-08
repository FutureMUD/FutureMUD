#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;
using MagicSpell = MudSharp.Models.MagicSpell;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonPreparedWorldSeederTests
{
	private static ArmageddonWaterSeeInstallPlan WaterSeePlan(FuturemudDatabaseContext db, ArmageddonPreparedWorldBindings bindings)
	{
		long Skill(string key) => db.SeederManagedRecords.Single(x => x.StableKey == key + ".skill").LogicalId!.Value;
		return new(true, bindings.Utilities.School, bindings.Utilities.Resource, bindings.Utilities.AlwaysFalseProg,
			Skill(ArmageddonWaterSeeInstaller.WaterBreathingKey), Skill(ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey), bindings.WaterSee!);
	}
	private static ArmageddonInstallResult RunWaterSee(FuturemudDatabaseContext db, ArmageddonWaterSeeInstallPlan plan,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		db.ChangeTracker.Clear(); return ArmageddonWaterSeeInstaller.Install(db, plan, checkpoint);
	}

	[DataTestMethod]
	[DataRow("empty-water")][DataRow("duplicate-water")][DataRow("missing-water")][DataRow("negative-water")]
	[DataRow("missing-silt")][DataRow("missing-shadow")][DataRow("missing-tag")][DataRow("duplicate-tag")]
	[DataRow("wrong-rank-order")][DataRow("missing-ancestor")][DataRow("cycle")][DataRow("null-water")][DataRow("null-tags")]
	public void WaterSeeInvalidNativeMappingsRefuseBeforeCompositionWrites(string invalid)
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			if (invalid == "missing-ancestor") { db.Tags.Find(201L)!.ParentId = 999; db.SaveChanges(); }
			if (invalid == "cycle") { db.Tags.Find(201L)!.ParentId = 205; db.SaveChanges(); }
			var selection = bindings.WaterSee!;
			selection = invalid switch
			{
				"empty-water" => selection with { WaterLiquids = [] }, "duplicate-water" => selection with { WaterLiquids = [1, 1] },
				"missing-water" => selection with { WaterLiquids = [999] }, "negative-water" => selection with { WaterLiquids = [-1] },
				"missing-silt" => selection with { SiltTerrain = 999 }, "missing-shadow" => selection with { ShadowTerrain = 999 },
				"missing-tag" => selection with { DivinationRankTags = [201, 202, 203, 204, 999] },
				"duplicate-tag" => selection with { DivinationRankTags = [201, 202, 203, 204, 204] },
				"wrong-rank-order" => selection with { DivinationRankTags = [205, 204, 203, 202, 201] },
				"null-water" => selection with { WaterLiquids = null! }, "null-tags" => selection with { DivinationRankTags = null! }, _ => selection
			};
			var ids = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
			var policy = CapabilityMeritPolicy(db);
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { WaterSee = selection });
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, result.Describe()); Assert.AreEqual(0, result.Modules.Count);
			Assert.AreEqual(ids.Count, db.SeederManagedRecords.Count()); Assert.AreEqual(policy, CapabilityMeritPolicy(db));
			Assert.AreEqual(0, db.SeederManagedRecords.Count(x => x.Module == ArmageddonWaterSeeInstaller.Module));
		}
	}

	[TestMethod]
	public void WaterSeeTransitiveRankAncestryAndSameTerrainMappingRemainSupported()
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			db.Tags.Add(new() { Id = 301, Name = "explicit intermediary", ParentId = 201 });
			db.Tags.Find(202L)!.ParentId = 301; db.SaveChanges();
			bindings = bindings with { WaterSee = bindings.WaterSee! with { ShadowTerrain = 101 } };
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var see = db.MagicSpells.Single(x => db.SeederManagedRecords.Any(y => y.StableKey == ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey && y.LogicalId == x.Id));
			var scope = XElement.Parse(see.Definition).Descendants("SourceScope").Single();
			Assert.AreEqual(101L, (long)scope.Attribute("silt")!); Assert.AreEqual(101L, (long)scope.Attribute("shadow")!);
		}
	}

	[DataTestMethod]
	[DataRow("unowned-skill")][DataRow("wrong-owner-scope")][DataRow("foreign-skill-claim")][DataRow("skill-version")]
	[DataRow("wine-retired")][DataRow("wine-version")][DataRow("wine-incomplete")][DataRow("false-prog")]
	public void WaterSeeStandaloneRequiresOwnedSourceSkillsAndCompleteLegitimatePrerequisites(string invalid)
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { WaterSee = null }));
			var plan = WaterSeePlan(db, bindings);
			var owner = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonWaterSeeInstaller.WaterBreathingKey + ".skill");
			var wine = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedProvisionContent.DrawWineKey);
			if (invalid == "unowned-skill") plan = plan with { WaterBreathingSkill = 10 };
			if (invalid == "wrong-owner-scope") db.TraitDefinitions.Find(plan.WaterBreathingSkill)!.OwnerScope = 0;
			if (invalid == "foreign-skill-claim") db.SeederManagedRecords.Add(new() { Seeder = "external", Module = "external", StableKey = "external", EntityType = owner.EntityType, LogicalId = owner.LogicalId });
			if (invalid == "skill-version") owner.ManifestVersion = "future";
			if (invalid == "wine-retired") wine.Retired = true;
			if (invalid == "wine-version") wine.ManifestVersion = "future";
			if (invalid == "wine-incomplete") db.MagicSpells.Find(wine.LogicalId)!.Definition = "<Definition><StockIdentity>arm.spell.draw_wine</StockIdentity></Definition>";
			if (invalid == "false-prog") db.FutureProgs.Find(plan.AlwaysFalseProg)!.FunctionText = "return true";
			db.SaveChanges(); var policy = CapabilityMeritPolicy(db); var count = db.SeederManagedRecords.Count();
			var result = RunWaterSee(db, plan);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join(Environment.NewLine, result.Messages));
			Assert.AreEqual(count, db.SeederManagedRecords.Count()); Assert.AreEqual(policy, CapabilityMeritPolicy(db));
			Assert.AreEqual(0, db.SeederManagedRecords.Count(x => x.Module == ArmageddonWaterSeeInstaller.Module));
		}
	}

	[DataTestMethod]
	[DataRow("deleted")][DataRow("retired")][DataRow("foreign")][DataRow("version")][DataRow("missing-record")][DataRow("identity")]
	public void WaterSeeOwnedDamageBlocksSelectedAndNullWithoutRepairOrResurrection(string invalid)
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonWaterSeeInstaller.WaterBreathingKey);
			if (invalid == "deleted") db.Remove(db.MagicSpells.Find(record.LogicalId)!);
			if (invalid == "retired") record.Retired = true;
			if (invalid == "version") record.ManifestVersion = "future";
			if (invalid == "foreign") db.SeederManagedRecords.Add(new() { Seeder = "external", Module = "external", StableKey = "external", EntityType = record.EntityType, LogicalId = record.LogicalId });
			if (invalid == "missing-record") db.Remove(record);
			if (invalid == "identity") db.MagicSpells.Find(record.LogicalId)!.Definition = "<Definition/>";
			db.SaveChanges(); var count = db.MagicSpells.Count(); var policy = WaterSeePolicy(db); var caps = CapabilityMeritPolicy(db);
			foreach (var selection in new[] { bindings, bindings with { WaterSee = null } })
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), selection);
				Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status); Assert.AreEqual(0, result.Modules.Count);
				Assert.AreEqual(count, db.MagicSpells.Count()); Assert.AreEqual(policy, WaterSeePolicy(db)); Assert.AreEqual(caps, CapabilityMeritPolicy(db));
			}
		}
	}

	[TestMethod]
	public void WaterSeeModulePreservesBuilderPayloadDurationCostEligibilityAndUnownedCopies()
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			long Id(string stableKey) => db.SeederManagedRecords.Single(x => x.StableKey == stableKey).LogicalId!.Value;
			var key = ArmageddonWaterSeeInstaller.WaterBreathingKey; var spell = db.MagicSpells.Find(Id(key))!;
			spell.Name = "builder water"; var xml = XElement.Parse(spell.Definition); xml.Descendants("LifetimePolicy").Single().Remove(); spell.Definition = xml.ToString();
			db.TraitExpressions.Find(Id(key + ".duration"))!.Expression = "120*grade"; db.TraitExpressions.Find(Id(key + ".cost"))!.Expression = "9*grade";
			db.FutureProgs.Find(Id(key + ".eligibility"))!.FunctionText = "return false";
			var clone = (MagicSpell)db.Entry(spell).CurrentValues.ToObject(); clone.Id = 0; clone.Name = "unowned copy"; db.MagicSpells.Add(clone); db.SaveChanges();
			var policy = WaterSeePolicy(db); var provisions = ProvisionPolicy(db);
			foreach (var selection in new[] { bindings, bindings with { WaterSee = null, Provisions = null } })
			{
				Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), selection));
				Assert.AreEqual(policy, WaterSeePolicy(db)); Assert.AreEqual(provisions, ProvisionPolicy(db));
				Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == nameof(MagicSpell) && x.LogicalId == clone.Id));
			}
		}
	}

	[TestMethod]
	public void WaterSeeDeclineNeedsNoDatabaseAndTrackedContextRefuses()
	{
		var (db, bindings) = WaterSeeFixture();
		Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { WaterSee = null }));
		var plan = WaterSeePlan(db, bindings); db.MagicSpells.First();
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, ArmageddonWaterSeeInstaller.Install(db, plan).Status);
		db.Dispose(); Assert.AreEqual(ArmageddonInstallStatus.Declined, ArmageddonWaterSeeInstaller.Install(db, plan with { Install = false }).Status);
	}

	[DataTestMethod]
	[DataRow("missing")][DataRow("changed")]
	public void WaterSeeStandaloneRefusesRetainedStockIdentityDamageWithoutRepair(string damage)
	{
		var (db, bindings) = WaterSeeFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var plan = WaterSeePlan(db, bindings);
			var row = db.MagicSpells.Single(x => db.SeederManagedRecords.Any(r => r.StableKey == ArmageddonWaterSeeInstaller.WaterBreathingKey && r.LogicalId == x.Id));
			var xml = XElement.Parse(row.Definition);
			if (damage == "missing") xml.Element("StockIdentity")!.Remove();
			else xml.Element("StockIdentity")!.Value = "builder.changed.identity";
			row.Definition = xml.ToString(); db.SaveChanges();
			var policy = WaterSeePolicy(db); var caps = CapabilityMeritPolicy(db);
			var result = RunWaterSee(db, plan);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status);
			StringAssert.Contains(string.Join(Environment.NewLine, result.Messages), "stock identity");
			Assert.AreEqual(policy, WaterSeePolicy(db)); Assert.AreEqual(caps, CapabilityMeritPolicy(db));
		}
	}
}
