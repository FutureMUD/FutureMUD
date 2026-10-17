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
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonPreparedWorldSeederTests
{
	private static (FuturemudDatabaseContext Db, ArmageddonPreparedWorldBindings Bindings) EmotionalFixture()
	{
		var (db, bindings) = WaterSeeFixture();
		for (var id = 107; id <= 113; id++) db.Terrains.Add(new() { Id = id, Name = "explicit emotional terrain " + id });
		db.TraitDefinitions.Add(new() { Id = 300, Name = "Constitution", Type = 1, OwnerScope = 0, DecoratorId = 1 });
		db.SaveChanges(); db.ChangeTracker.Clear();
		return (db, bindings with { Emotions = new(null, 1, 1, 3, 10, 1, 3,
			Enumerable.Repeat(Difficulty.Normal, 7).ToArray(), new(107, 108, 109, 110, 111, 112, 113)) });
	}
	private static string EmotionalPolicy(FuturemudDatabaseContext db) => JsonSerializer.Serialize(new
	{
		Records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonEmotionalInstaller.Module).OrderBy(x => x.StableKey)
			.Select(x => new { x.StableKey, x.LogicalId, x.SeedBaseline, x.AppliedFingerprint, x.AppliedAt }).ToArray(),
		Spells = db.MagicSpells.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonEmotionalInstaller.Module && y.EntityType == "MagicSpell" && y.LogicalId == x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Definition }).ToArray(),
		Expressions = db.TraitExpressions.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonEmotionalInstaller.Module && y.EntityType == "TraitExpression" && y.LogicalId == x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Expression }).ToArray()
	});

	[TestMethod]
	public void EmotionsComposeTwelveAdmissionsAndPreserveIdsAndPlayerStateOnRerun()
	{
		var (db, bindings) = EmotionalFixture(); using (db)
		{
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(result);
			Assert.AreEqual(217, db.SeederManagedRecords.Count()); Assert.AreEqual(12, db.MagicSpells.Count());
			Assert.AreEqual(6, result.Modules.Single(x => x.Module == ArmageddonEmotionalInstaller.Module).Identities.Count);
			Assert.IsTrue(result.Availability.All(x => x.StoredAdmissions.Count == 12 && x.WithoutStoredAdmission.Count == 70));
			long Id(string key) => db.SeederManagedRecords.AsNoTracking().Single(x => x.StableKey == key).LogicalId!.Value;
			foreach (var cap in db.MagicCapabilities.AsNoTracking().Where(x => x.Name.StartsWith("Armageddon partial ")))
			{
				var admissions = XElement.Parse(cap.Definition).Element("Casting")!.Elements("Admission").ToArray();
				foreach (var (key, parent, rawCap) in new[] { (ArmageddonEmotionalInstaller.FuryKey, "arm.spell.unravel_enchantment", 90d),
					(ArmageddonEmotionalInstaller.CalmKey, ArmageddonEmotionalInstaller.FuryKey, 90d), ("arm.spell.mend_flesh", ArmageddonEmotionalInstaller.CalmKey, 60d) })
				{
					var admission = admissions.Single(x => (long?)x.Attribute("spell") == Id(key));
					Assert.AreEqual(30d, (double)admission.Attribute("opening")!); Assert.AreEqual(rawCap, (double)admission.Attribute("rawCap")!);
					Assert.AreEqual(Id(parent), (long)admission.Element("Prerequisite")!.Attribute("spell")!);
					Assert.AreEqual(80d, (double)admission.Element("Prerequisite")!.Attribute("proficiency")!);
				}
			}
			var furyId = Id(ArmageddonEmotionalInstaller.FuryKey);
			var fury = XElement.Parse(db.MagicSpells.AsNoTracking().Single(x => x.Id == furyId).Definition);
			Assert.AreEqual(300L, (long)fury.Element("Effects")!.Element("Effect")!.Element("SourceProfile")!.Attribute("trait")!);
			var policy = EmotionalPolicy(db); var capabilities = CapabilityMeritPolicy(db);
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			Assert.AreEqual(policy, EmotionalPolicy(db)); Assert.AreEqual(capabilities, CapabilityMeritPolicy(db));
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { Emotions = null, WaterSee = null, Provisions = null }));
			Assert.AreEqual(policy, EmotionalPolicy(db)); Assert.AreEqual(capabilities, CapabilityMeritPolicy(db));
			Assert.AreEqual(0, db.CharacterTraits.Count()); Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
			Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.CharactersMagicResources.Count()); Assert.AreEqual(0, db.ChargenRoles.Count());
		}
	}

	[DataTestMethod]
	[DataRow("missing")]
	[DataRow("ambiguous")]
	[DataRow("derived")]
	[DataRow("override")]
	[DataRow("terrain")]
	[DataRow("savecount")]
	[DataRow("difficulty")]
	[DataRow("eligibility")]
	[DataRow("compile")]
	[DataRow("units")]
	[DataRow("savetrait")]
	public void EmotionsInvalidMappingsRefuseBeforeAnyComposedModuleWrite(string invalid)
	{
		var (db, bindings) = EmotionalFixture(); using (db)
		{
			var selected = bindings.Emotions!;
			switch (invalid)
			{
				case "missing": db.TraitDefinitions.Remove(db.TraitDefinitions.Single(x => x.Id == 300)); break;
				case "ambiguous": db.TraitDefinitions.Add(new() { Id = 301, Name = "Endurance", Type = 1, OwnerScope = 0 }); break;
				case "derived": db.TraitDefinitions.Single(x => x.Id == 300).Type = 3; break;
				case "override": selected = selected with { FuryAttribute = 10 }; break;
				case "terrain": selected = selected with { Terrains = selected.Terrains with { Air = selected.Terrains.Earth } }; break;
				case "savecount": selected = selected with { CalmSaves = [Difficulty.Normal] }; break;
				case "difficulty": selected = selected with { CalmSaves = Enumerable.Repeat((Difficulty)999, 7).ToArray() }; break;
				case "eligibility": selected = selected with { FuryEligibilityProg = 1 }; break;
				case "compile": db.FutureProgs.Single(x => x.Id == 3).FunctionText = "a malformed native prog"; break;
				case "units": selected = selected with { UnitsPerSourcePoint = double.MaxValue }; break;
				case "savetrait": selected = selected with { CalmSaveTrait = 9999 }; break;
			}
			db.SaveChanges(); db.ChangeTracker.Clear(); var before = db.SeederManagedRecords.Count(); var policy = CapabilityMeritPolicy(db);
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { Emotions = selected });
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, result.Describe()); Assert.AreEqual(0, result.Modules.Count);
			Assert.AreEqual(before, db.SeederManagedRecords.Count()); Assert.AreEqual(policy, CapabilityMeritPolicy(db));
		}
	}

	[TestMethod]
	public void EmotionsExplicitOverrideAndStrictJsonPreserveOldBindingsContract()
	{
		var (db, bindings) = EmotionalFixture(); using (db)
		{
			bindings = bindings with { Emotions = bindings.Emotions! with { FuryAttribute = 100 } };
			var json = ArmageddonMagicSeeder.SerializeBindings(bindings); Assert.IsTrue(ArmageddonMagicSeeder.ValidateBindings(json, db).Success);
			var parsed = ArmageddonMagicSeeder.ParseBindings(json); Assert.AreEqual(100L, parsed.Emotions!.FuryAttribute);
			var node = JsonNode.Parse(json)!.AsObject(); node["Emotions"]!["CalmSaves"]![0] = 3;
			Assert.ThrowsException<JsonException>(() => ArmageddonMagicSeeder.ParseBindings(node.ToJsonString()));
			node = JsonNode.Parse(json)!.AsObject(); node["Emotions"]!["GuessedSetting"] = 10;
			Assert.ThrowsException<JsonException>(() => ArmageddonMagicSeeder.ParseBindings(node.ToJsonString()));
			node = JsonNode.Parse(json)!.AsObject(); node.Remove("Emotions"); Assert.IsNull(ArmageddonMagicSeeder.ParseBindings(node.ToJsonString()).Emotions);
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), parsed));
			var spellId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonEmotionalInstaller.FuryKey).LogicalId;
			Assert.AreEqual(100L, (long)XElement.Parse(db.MagicSpells.Single(x => x.Id == spellId).Definition).Element("Effects")!.Element("Effect")!.Element("SourceProfile")!.Attribute("trait")!);
		}
	}

	[TestMethod]
	public void EmotionsBuilderEditsAndAdmissionRemovalSurviveSelectedAndNullReruns()
	{
		var (db, bindings) = EmotionalFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var id = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonEmotionalInstaller.FuryKey).LogicalId;
			var row = db.MagicSpells.Single(x => x.Id == id); row.Name = "builder Fury";
			var xml = XElement.Parse(row.Definition); xml.Element("Effects")!.Element("Effect")!.Element("SourceProfile")!.SetAttributeValue("intensity", 9); row.Definition = xml.ToString();
			var costId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonEmotionalInstaller.FuryKey + ".cost").LogicalId;
			db.TraitExpressions.Single(x => x.Id == costId).Expression = "42";
			var cap = db.MagicCapabilities.Single(x => x.Name == "Armageddon partial sorcerer"); xml = XElement.Parse(cap.Definition);
			xml.Element("Casting")!.Elements("Admission").Single(x => (long?)x.Attribute("spell") == id).Remove(); cap.Definition = xml.ToString();
			db.SaveChanges(); db.ChangeTracker.Clear();
			foreach (var selection in new[] { bindings, bindings with { Emotions = null } })
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), selection); Completed(result);
				Assert.IsFalse(result.Availability.Single(x => x.Variant == "sorcerer").StoredAdmissions.Contains(ArmageddonEmotionalInstaller.FuryKey));
				Assert.AreEqual("builder Fury", db.MagicSpells.AsNoTracking().Single(x => x.Id == id).Name);
				Assert.AreEqual("42", db.TraitExpressions.AsNoTracking().Single(x => x.Id == costId).Expression);
				Assert.AreEqual(9d, (double)XElement.Parse(db.MagicSpells.AsNoTracking().Single(x => x.Id == id).Definition).Element("Effects")!.Element("Effect")!.Element("SourceProfile")!.Attribute("intensity")!);
			}
		}
	}

	[DataTestMethod]
	[DataRow("retired")]
	[DataRow("deleted")]
	[DataRow("version")]
	[DataRow("claim")]
	public void EmotionsInvalidRetainedOwnershipRefusesSelectedAndNullReruns(string invalid)
	{
		var (db, bindings) = EmotionalFixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonEmotionalInstaller.FuryKey);
			if (invalid == "retired") record.Retired = true;
			if (invalid == "deleted") db.MagicSpells.Remove(db.MagicSpells.Single(x => x.Id == record.LogicalId));
			if (invalid == "version") record.ManifestVersion = "unknown";
			if (invalid == "claim") db.SeederManagedRecords.Add(new() { Seeder = "other", Module = "other", StableKey = "other", EntityType = record.EntityType, LogicalId = record.LogicalId });
			db.SaveChanges(); db.ChangeTracker.Clear(); var before = db.SeederManagedRecords.Count();
			foreach (var selection in new[] { bindings, bindings with { Emotions = null } })
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), selection);
				Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status); Assert.AreEqual(0, result.Modules.Count); Assert.AreEqual(before, db.SeederManagedRecords.Count());
			}
		}
	}

	[TestMethod]
	public void EmotionsUnownedSpellCollisionRefusesBeforeCompositionWithoutAdoption()
	{
		var (db, bindings) = EmotionalFixture(); using (db)
		{
			db.MagicSpells.Add(new() { Id = 1000, MagicSchoolId = 1, Name = "Roused Fury", Definition = "<Definition />" });
			db.SaveChanges(); db.ChangeTracker.Clear(); var before = db.SeederManagedRecords.Count();
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, result.Describe()); Assert.AreEqual(0, result.Modules.Count);
			Assert.AreEqual(before, db.SeederManagedRecords.Count()); Assert.IsFalse(db.SeederManagedRecords.Any(x => x.Module == ArmageddonEmotionalInstaller.Module));
			Assert.AreEqual("<Definition />", db.MagicSpells.AsNoTracking().Single(x => x.Id == 1000).Definition);
		}
	}

	[TestMethod]
	public void EmotionsFreezeSelectedSavesAndRecoverLostAcknowledgementWithSameIdentities()
	{
		var (db, bindings) = EmotionalFixture(); using (db)
		{
			var saves = (Difficulty[])bindings.Emotions!.CalmSaves;
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{
				if (module != ArmageddonEmotionalInstaller.Module) return;
				if (point == ArmageddonInstallCheckpoint.PreflightComplete) saves[0] = (Difficulty)999;
				if (point == ArmageddonInstallCheckpoint.AfterCommit) throw new Exception("lost acknowledgement");
			});
			Assert.AreEqual(ArmageddonInstallStatus.CommittedConfirmationFailed, result.Status, result.Describe()); Assert.AreEqual(0, result.Availability.Count);
			var ids = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonEmotionalInstaller.Module).ToDictionary(x => x.StableKey, x => x.LogicalId);
			Assert.AreEqual(6, ids.Count); saves[0] = Difficulty.Normal;
			var completed = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(completed);
			Assert.IsTrue(completed.Availability.All(x => x.StoredAdmissions.Count == 12));
			Assert.IsTrue(db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonEmotionalInstaller.Module).AsEnumerable().All(x => ids[x.StableKey] == x.LogicalId));
		}
	}
}
