#nullable enable
using System;
using System.Collections.Generic;
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

[TestClass]
public class ArmageddonPierceInstallerTests
{
	private static (FuturemudDatabaseContext Db, ArmageddonPierceInstallPlan Plan, ArmageddonTraditionInstallPlan Traditions) Fixture()
	{
		var (db, traditions) = ArmageddonTraditionInstallerTests.Fixture();
		var result = ArmageddonTraditionInstaller.Install(db, traditions);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		return (db, new(true, 1, 1, 1, result.Identities[ArmageddonReviewedPierceContent.Key + ".skill"]), traditions);
	}
	private static ArmageddonInstallResult Run(FuturemudDatabaseContext db, ArmageddonPierceInstallPlan plan)
	{ db.ChangeTracker.Clear(); return ArmageddonPierceInstaller.Install(db, plan); }
	private static void Completed(ArmageddonInstallResult result) =>
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));

	[TestMethod] public void FourOwnedRecordsCloseOnlyTheHistoricalSenseChild()
	{
		var (db, plan, traditions) = Fixture(); using (db)
		{
			var previous = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
			var result = Run(db, plan); Completed(result);
			Assert.AreEqual(4, result.Identities.Count); Assert.AreEqual(175, db.SeederManagedRecords.Count());
			var bindings = new Dictionary<string, long>(traditions.ImplementedSpells) { [ArmageddonReviewedPierceContent.Key] = result.Identities[ArmageddonReviewedPierceContent.Key] };
			db.ChangeTracker.Clear();
			var expanded = ArmageddonTraditionInstaller.Install(db, traditions with { ImplementedSpells = bindings });
			Assert.AreEqual(ArmageddonInstallStatus.Completed, expanded.Status, string.Join("\n", expanded.Messages));
			Assert.AreEqual(4, expanded.AvailableSpells.Count); Assert.AreEqual(78, expanded.UnavailableSpells.Count);
			foreach (var cap in db.MagicCapabilities.Where(x => x.Id != 1))
			{
				var row = XElement.Parse(cap.Definition).Element("Casting")!.Elements("Admission").Single(x => (long)x.Attribute("spell")! == bindings[ArmageddonReviewedPierceContent.Key]);
				Assert.AreEqual(30, (double)row.Attribute("opening")!); Assert.AreEqual(90, (double)row.Attribute("rawCap")!);
				Assert.AreEqual(80, (double)row.Element("Prerequisite")!.Attribute("proficiency")!);
				Assert.AreEqual(bindings[ArmageddonReviewedUtilityContent.SenseEnchantmentKey], (long)row.Element("Prerequisite")!.Attribute("spell")!);
			}
			Assert.IsTrue(db.SeederManagedRecords.Where(x => x.Module != ArmageddonPierceInstaller.Module).AsEnumerable().All(x => previous[x.StableKey] == x.LogicalId));
			Assert.AreEqual(0, db.CharacterCastingEnrolments.Count()); Assert.AreEqual(0, db.CharacterTraits.Count());
		}
	}
	[TestMethod] public void DefinitionUsesReviewedNativeAccumulationAndTargetEligibility()
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			var result = Run(db, plan); Completed(result);
			var spell = db.MagicSpells.Find(result.Identities[ArmageddonReviewedPierceContent.Key])!;
			var definition = XElement.Parse(spell.Definition); var policy = definition.Descendants("LifetimePolicy").Single();
			Assert.AreEqual("accumulate", (string)policy.Attribute("mode")!); Assert.AreEqual(600, (int)policy.Attribute("unitSeconds")!);
			Assert.AreEqual(48, (int)policy.Attribute("maximumUnits")!); Assert.IsTrue((bool)policy.Attribute("retainStrongestGrade")!);
			Assert.AreEqual("3000*grade", db.TraitExpressions.Find(spell.EffectDurationExpressionId)!.Expression);
			Assert.IsTrue(spell.AppliedEffectsAreExclusive); Assert.IsFalse(spell.ScrollInscriptionAllowed);
			var prog = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == result.Identities[ArmageddonReviewedPierceContent.Key + ".eligibility"]);
			Assert.AreEqual(ArmageddonReviewedPierceContent.EligibilitySource, prog.FunctionText); Assert.AreEqual(2, prog.FutureProgsParameters.Count);
		}
	}
	[TestMethod] public void RerunPreservesIdentitiesBaselinesTimesAndBuilderOverrides()
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			var installed = Run(db, plan); Completed(installed);
			var before = db.SeederManagedRecords.Where(x => x.Module == ArmageddonPierceInstaller.Module).AsNoTracking().ToDictionary(x => x.StableKey, x => (x.LogicalId, x.SeedBaseline, x.AppliedAt));
			Completed(Run(db, plan));
			foreach (var row in db.SeederManagedRecords.Where(x => x.Module == ArmageddonPierceInstaller.Module)) Assert.AreEqual(before[row.StableKey], (row.LogicalId, row.SeedBaseline, row.AppliedAt));
			var spell = db.MagicSpells.Find(installed.Identities[ArmageddonReviewedPierceContent.Key])!; spell.Name = "Builder Pierce";
			var xml = XElement.Parse(spell.Definition); xml.Descendants("LifetimePolicy").Single().Remove(); spell.Definition = xml.ToString(); spell.AppliedEffectsAreExclusive = false;
			var clone = (MagicSpell)db.Entry(spell).CurrentValues.ToObject(); clone.Id = 0; clone.Name = "Unowned Pierce clone"; db.MagicSpells.Add(clone);
			var cost = db.TraitExpressions.Find(installed.Identities[ArmageddonReviewedPierceContent.Key + ".cost"])!; cost.Expression = "9*grade"; db.SaveChanges();
			Completed(Run(db, plan));
			var retained = db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id);
			Assert.AreEqual("Builder Pierce", retained.Name); Assert.AreEqual(xml.ToString(), retained.Definition); Assert.IsFalse(retained.AppliedEffectsAreExclusive);
			Assert.AreEqual("9*grade", db.TraitExpressions.Find(cost.Id)!.Expression);
			Assert.IsFalse(db.SeederManagedRecords.Any(x => x.EntityType == nameof(MagicSpell) && x.LogicalId == clone.Id));
		}
	}
	[TestMethod] public void DeletedRetiredAndForeignClaimedOwnedRowsBlockWithoutResurrection()
	{
		foreach (var scenario in new[] { "deleted", "retired", "foreign", "version" })
		{
			var (db, plan, _) = Fixture(); using (db)
			{
				var result = Run(db, plan); Completed(result); var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedPierceContent.Key);
				if (scenario == "deleted") db.Remove(db.MagicSpells.Find(record.LogicalId)!);
				else if (scenario == "retired") record.Retired = true;
				else if (scenario == "version") record.ManifestVersion = "unsupported";
				else db.SeederManagedRecords.Add(new() { Seeder = "foreign", Module = "foreign", StableKey = "foreign", EntityType = record.EntityType, LogicalId = record.LogicalId });
				db.SaveChanges(); var count = db.MagicSpells.Count();
				Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(count, db.MagicSpells.Count());
			}
		}
	}
	[TestMethod]
	[DataRow("school")][DataRow("resource")][DataRow("skill")][DataRow("scope")][DataRow("prog")][DataRow("progtype")]
	public void InvalidNativeDependenciesFailClosed(string selection)
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			if (selection == "scope") db.TraitDefinitions.Find(plan.PierceSkill)!.OwnerScope = 0;
			if (selection == "progtype") db.FutureProgs.Find(plan.AlwaysFalseProg)!.FunctionText = "return true";
			db.SaveChanges(); plan = selection switch { "school" => plan with { School = long.MaxValue }, "resource" => plan with { Resource = long.MaxValue },
				"skill" => plan with { PierceSkill = long.MaxValue }, "prog" => plan with { AlwaysFalseProg = long.MaxValue }, _ => plan };
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status);
			Assert.AreEqual(0, db.SeederManagedRecords.Count(x => x.Module == ArmageddonPierceInstaller.Module)); Assert.AreEqual(5, db.MagicSpells.Count());
		}
	}
	[TestMethod] public void UnownedIdentityAndCrossModuleKeysCannotBeAdopted()
	{
		foreach (var foreign in new[] { false, true })
		{
			var (db, plan, _) = Fixture(); using (db)
			{
				if (foreign) db.SeederManagedRecords.Add(new() { Seeder = ArmageddonPierceInstaller.Package, Module = "other", StableKey = ArmageddonReviewedPierceContent.Key, EntityType = nameof(MagicSpell), LogicalId = 999 });
				else db.MagicSpells.Add(new() { Name = "Renamed unowned detection", Definition = ArmageddonReviewedPierceContent.PierceConcealment().BuildDefinition(1, 1, 0).ToString() });
				db.SaveChanges(); Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status);
				Assert.AreEqual(0, db.SeederManagedRecords.Count(x => x.Module == ArmageddonPierceInstaller.Module));
			}
		}
	}
	[TestMethod] public void DeclineDoesNotInspectDisposedContext()
	{
		var (db, plan, _) = Fixture(); db.Dispose();
		Assert.AreEqual(ArmageddonInstallStatus.Declined, ArmageddonPierceInstaller.Install(db, plan with { Install = false }).Status);
	}
}
