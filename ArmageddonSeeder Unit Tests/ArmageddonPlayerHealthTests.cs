#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPlayerHealthTests
{
	[TestMethod]
	public void NormalSetting_InstallsRegisteredPlayerPoolsAndPreservesBuilderCalibration()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		db.HealthStrategies.Add(new HealthStrategy { Id = 900000, Name = "Human HP", Type = "BrainHitpoints",
			Definition = "<Definition><MaximumHitPointsExpression>1</MaximumHitPointsExpression><HealingTickDamageExpression>1</HealingTickDamageExpression></Definition>" });
		db.SaveChanges(); var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var owner = MudSharp.FutureProg.ProgVariableTypes.Race.ToStorageString();
		foreach (var policy in ArmageddonPlayerHealth.Profiles)
		{
			var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.race." + policy.Profile);
			var race = db.Races.Single(x => x.Id == record.LogicalId);
			var health = db.HealthStrategies.Single(x => x.Id == race.DefaultHealthStrategyId);
			var xml = XElement.Parse(health.Definition);
			Assert.AreEqual("BrainHitpoints", health.Type); Assert.AreEqual("true", xml.Element("UseStun")!.Value);
			Assert.AreEqual("-11", xml.Element("DeathHitPoints")!.Value);
			var hp = db.TraitExpressions.Single(x => x.Id == (long)xml.Element("MaximumHitPointsExpression")!);
			StringAssert.Contains(hp.Expression, " prog="); StringAssert.Contains(hp.Expression, " class=");
			Assert.AreEqual(3, db.VariableValues.Count(x => x.ReferenceId == race.Id && x.ReferenceTypeDefinition == owner));
		}
		var giant = db.SeederManagedRecords.Single(x => x.StableKey == "arm.race.half_giant").LogicalId!.Value;
		var value = db.VariableValues.Single(x => x.ReferenceTypeDefinition == owner && x.ReferenceId == giant && x.ReferenceProperty == ArmageddonPlayerHealth.SizeBonus);
		value.ValueDefinition = "<var>100</var>"; db.SaveChanges();
		var count = db.HealthStrategies.Count();
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual(count, db.HealthStrategies.Count()); Assert.AreEqual("<var>100</var>", value.ValueDefinition);
		Assert.IsTrue(db.SeederManagedRecords.Where(x => x.Module == ArmageddonPlayerHealth.RegisterModule).ToArray()
			.All(x => ArmageddonPlayerHealth.ValidRegisterRecord(db, x)));
		Assert.IsTrue(db.SeederManagedRecords.Where(x => x.Module == ArmageddonPlayerHealth.RegisterModule).ToArray()
			.All(x => !string.IsNullOrEmpty(x.ManifestVersion) && x.AppliedAt.Year >= 1000), "Ownership stamps must be valid MySQL datetime values.");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		compiler.Compile(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonPlayerBaseCapacity").Id);
	}

	[TestMethod]
	public void PrivateCorpus_TranslatedNativeTargetsMatchApprovedExpectedCalibration()
	{
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		for (var endurance = 0; endurance <= 100; endurance++)
			Assert.AreEqual(80.5 + endurance + source.CombatCalibration!.EnduranceSaves![endurance], ArmageddonPlayerHealth.DefaultBasePools[endurance], 1e-8);
		foreach (var policy in ArmageddonPlayerHealth.Profiles)
		{
			var expected = source.PhysicalGenerators.Single(x => x.Race == policy.SourceRace).SizeBonuses!.Sum(x => x.Key * x.Value);
			Assert.AreEqual(expected, policy.Size, 1e-8);
		}
	}

	[TestMethod]
	public void PrivateCorpus_ImportCompositionPreservesBuilderHealthChoice()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		db.HealthStrategies.Add(new HealthStrategy { Id = 900000, Name = "Human HP", Type = "BrainHitpoints", Definition = "<Definition><MaximumHitPointsExpression>1</MaximumHitPointsExpression><HealingTickDamageExpression>1</HealingTickDamageExpression></Definition>" });
		db.SaveChanges(); var messages = new List<string>(); ArmageddonHeritage.Install(db, messages);
		var id = db.SeederManagedRecords.Single(x => x.StableKey == "arm.race.human").LogicalId!.Value;
		var race = db.Races.Single(x => x.Id == id); race.DefaultHealthStrategyId = 100; db.SaveChanges();
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual(100L, race.DefaultHealthStrategyId);
		// Compose only the health-import stage; the heritage fixture intentionally omits full chargen/world foundations.
		using (var prepared = ArmageddonInstallationSettingsTests.Fixture())
		{
			var settings = ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(prepared));
			var stored = prepared.SeederManagedRecords.Single(x => x.StableKey == ArmageddonInstallation.SettingsKey);
			var copy = (SeederManagedRecord)prepared.Entry(stored).CurrentValues.ToObject(); copy.Id = 0;
			db.SeederManagedRecords.Add(copy);
			foreach (var mirror in prepared.SeederChoices.Where(x => x.Seeder == "__shared__").ToArray())
			{
				db.SeederChoices.RemoveRange(db.SeederChoices.Where(x => x.Seeder == mirror.Seeder && x.Choice == mirror.Choice));
				db.SeederChoices.Add(new SeederChoice { Seeder = mirror.Seeder, Choice = mirror.Choice, Answer = mirror.Answer, Version = mirror.Version, DateTime = mirror.DateTime });
			}
			var kilogram = settings.NativeBindings[ArmageddonNativeBinding.Kilogram];
			if (!db.UnitsOfMeasure.Any(x => x.Id == kilogram)) db.UnitsOfMeasure.Add((UnitOfMeasure)prepared.Entry(prepared.UnitsOfMeasure.Single(x => x.Id == kilogram)).CurrentValues.ToObject());
			db.SaveChanges();
		}
		var writer = new ArmageddonWorldfileWriter(db, ArmageddonWorldfileTestData.Load().ForInstallation());
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPlayerHealthPools", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		Assert.AreEqual(100L, race.DefaultHealthStrategyId, "Worldfile composition must not reclaim a builder-selected health strategy.");
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual(100L, race.DefaultHealthStrategyId, "Shared Setting ownership must remain valid after the private import.");
	}
}
