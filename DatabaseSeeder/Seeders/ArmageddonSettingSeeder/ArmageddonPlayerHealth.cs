#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

/// <summary>Native player health configuration shared by the Setting installer and private content importer.</summary>
internal static class ArmageddonPlayerHealth
{
	internal const string RegisterModule = "health-registers";
	internal const string BasePools = "arm_health_base_pools";
	internal const string SizeBonus = "arm_health_size_bonus";
	internal const string CreationBonus = "arm_health_creation_bonus";
	// Native expected capacity targets, not raw source Endurance records. Index is effective Endurance.
	internal static readonly double[] DefaultBasePools = [30.5, 31.5, 42.5, 53.5, 59.5, 65.5, 71.5, 77.5, 83.5, 89.5, 90.5, 91.5, 92.5, 98.5, 99.5, 100.5, 106.5, 112.5, 118.5, 124.5, 125.5, 131.5, 132.5, 138.5, 144.5, 147.5, 149.5, 150.5, 152.5, 153.5, 155.5, 156.5, 158.5, 159.5, 161.5, 162.5, 164.5, 165.5, 167.5, 168.5, 170.5, 171.5, 173.5, 174.5, 176.5, 177.5, 179.5, 180.5, 178.5, 179.5, 180.5, 181.5, 182.5, 183.5, 184.5, 185.5, 186.5, 187.5, 188.5, 189.5, 190.5, 191.5, 192.5, 193.5, 194.5, 195.5, 196.5, 197.5, 198.5, 199.5, 200.5, 201.5, 202.5, 203.5, 204.5, 205.5, 206.5, 207.5, 208.5, 209.5, 210.5, 211.5, 212.5, 213.5, 214.5, 215.5, 216.5, 217.5, 218.5, 219.5, 220.5, 221.5, 222.5, 223.5, 224.5, 225.5, 226.5, 227.5, 228.5, 229.5, 230.5];

	internal static readonly (string Profile, int SourceRace, double Size, double Creation)[] Profiles =
	[
		("human", 1, 0, 0), ("city_elf", 2, 0, 0), ("dwarf", 3, 0, 0),
		("half_giant", 23, 59.495886858, 24.5), ("mul", 32, 0, 0),
		("half_elf", 33, 0, 0), ("desert_elf", 98, 0, 0)
	];

	private static string Xml(IEnumerable<double> values) => new XElement("var", values.Select(x => new XElement("var", x.ToString("R", CultureInfo.InvariantCulture)))).ToString();
	private static string Xml(double value) => new XElement("var", value.ToString("R", CultureInfo.InvariantCulture)).ToString();
	private static string Owner => ProgVariableTypes.Race.ToStorageString();

	internal static bool ValidRegisterRecord(FuturemudDatabaseContext db, SeederManagedRecord record)
	{
		if (record.Module != RegisterModule || record.Retired || record.SeedBaseline is null) return false;
		if (record.EntityType == "RaceHealthRegister" && record.LogicalId is null)
		{
			if (!record.StableKey.StartsWith("arm.health.register.", StringComparison.Ordinal)) return false;
			var property = record.StableKey["arm.health.register.".Length..];
			var definition = db.VariableDefinitions.SingleOrDefault(x => x.OwnerTypeDefinition == Owner && x.Property == property);
			return definition is not null && definition.ContainedTypeDefinition ==
				(property == BasePools ? ProgVariableTypes.Number | ProgVariableTypes.Collection : ProgVariableTypes.Number).ToStorageString()
				&& db.VariableDefaults.Any(x => x.OwnerTypeDefinition == Owner && x.Property == property);
		}
		return record.EntityType == "RaceHealthValues" && record.LogicalId is { } race && db.Races.Any(x => x.Id == race) &&
			new[] { BasePools, SizeBonus, CreationBonus }.All(property => db.VariableValues.Any(x =>
				x.ReferenceTypeDefinition == Owner && x.ReferenceId == race && x.ReferenceProperty == property && x.ValueTypeDefinition ==
				(property == BasePools ? ProgVariableTypes.Number | ProgVariableTypes.Collection : ProgVariableTypes.Number).ToStorageString()));
	}

	internal static Dictionary<string, long> InstallDefinitions(FuturemudDatabaseContext db, ICollection<string> messages)
	{
		var brain = db.HealthStrategies.SingleOrDefault(x => x.Name == "Human HP" && x.Type == "BrainHitpoints");
		if (brain is null)
		{
			messages.Add("Player health calibration requires the native Human HP strategy; this minimal foundation has none.");
			return [];
		}
		foreach (var property in new[] { BasePools, SizeBonus, CreationBonus })
		{
			var type = property == BasePools ? ProgVariableTypes.Number | ProgVariableTypes.Collection : ProgVariableTypes.Number;
			var key = "arm.health.register." + property;
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
			var created = record is null;
			if (created)
			{
				if (db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == Owner && x.Property == property)) throw new InvalidOperationException("Unowned player-health register: " + property);
				db.VariableDefinitions.Add(new() { OwnerTypeDefinition = Owner, Property = property, ContainedTypeDefinition = type.ToStorageString() });
				db.VariableDefaults.Add(new() { OwnerTypeDefinition = Owner, Property = property, DefaultValue = property == BasePools ? Xml(DefaultBasePools) : Xml(0) });
				record = new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = RegisterModule, StableKey = key, EntityType = "RaceHealthRegister", ManifestVersion = ArmageddonSkillPackage.Version, AppliedAt = DateTime.UtcNow };
				db.SeederManagedRecords.Add(record);
			}
			else if (!ValidRegisterRecord(db, record!)) throw new InvalidOperationException("Invalid player-health register ownership: " + property);
			var row = db.VariableDefaults.Local.SingleOrDefault(x => x.OwnerTypeDefinition == Owner && x.Property == property) ?? db.VariableDefaults.Single(x => x.OwnerTypeDefinition == Owner && x.Property == property);
			var desired = property == BasePools ? Xml(DefaultBasePools) : Xml(0);
			var result = SeederManagedRecordReconciler.Reconcile(record!, new Dictionary<string, string> { ["DefaultValue"] = JsonSerializer.Serialize(row.DefaultValue) },
				new Dictionary<string, string> { ["DefaultValue"] = JsonSerializer.Serialize(desired) }, created, messages);
			row.DefaultValue = JsonSerializer.Deserialize<string>(result["DefaultValue"])!;
			record!.ManifestVersion = ArmageddonSkillPackage.Version;
			record.AppliedAt = DateTime.UtcNow;
			db.SaveChanges();
		}
		var endurance = ArmageddonWorldfileWriter.ResolveRecoveryAttribute(db.TraitDefinitions.Where(x => x.Type == 1).ToArray());
		var prog = new FutureProg { FunctionName = "ArmageddonPlayerBaseCapacity", ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(), FunctionText = $$"""
		var pools = getregister(@character.race, "{{BasePools}}")
		var endurance = max(1, min(100, floor(gettrait(@character, totrait({{endurance.Id}})))))
		var basis = 80.5 + @endurance
		if (@pools.Count(value, true) == 101)
			basis = getindex(@pools, @endurance)
		end if
		return @basis + getregister(@character.race, "{{SizeBonus}}") + getregister(@character.race, "{{CreationBonus}}")
		""" };
		var actualProg = ArmageddonHeritage.Prog(db, "arm.health.capacity-prog", prog.FunctionName, prog.FunctionText,
			ProgVariableTypes.Number, messages, (ProgVariableTypes.Character, "character"));
		var warrior = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.class.warrior")?.LogicalId;
		var resultStrategies = new Dictionary<string, long>();
		foreach (var profile in Profiles)
		{
			var definition = XElement.Parse(brain.Definition);
			foreach (var stun in new[] { false, true })
			{
				var end = $"max(1,min(100,floor(endurance:{endurance.Id})))";
				var capacity = $"{{capacity prog={actualProg.Id}}}+5*{{warrior class={warrior?.ToString(CultureInfo.InvariantCulture) ?? "Warrior"}}}";
				var formula = stun ? $"max(1,min(10*({end}),max(2*({end}),{capacity})))" : $"max(1,min(10*({end}),{capacity}))";
				var expression = ArmageddonSettingOwnership.Apply(db, ArmageddonHeritage.Module, "arm.health." + profile.Profile + (stun ? ".stun" : ".hp"),
					new TraitExpression { Name = profile.Profile + (stun ? " Player Stun Capacity" : " Player Health Capacity"), Expression = formula }, messages);
				definition.SetElementValue(stun ? "MaximumStunExpression" : "MaximumHitPointsExpression", expression.Id);
			}
			ArmageddonWorldfileWriter.ConfigureArmageddonHitPointPolicy(definition, endurance.Id);
			definition.SetElementValue("UseStun", true);
			definition.SetElementValue("CheckHeart", false);
			definition.SetElementValue("UseHypoxiaDamage", false);
			definition.SetElementValue("KnockoutOnCritical", false);
			var recovery = db.TraitExpressions.SingleOrDefault(x => x.Name == "Human Stun Heal Per Tick");
			if (recovery is not null) definition.SetElementValue("HealingTickStunExpression", recovery.Id);
			var health = ArmageddonSettingOwnership.Apply(db, ArmageddonHeritage.Module, "arm.health." + profile.Profile + ".strategy",
				new HealthStrategy { Name = profile.Profile + " Player Hit Points", Type = "BrainHitpoints", Definition = definition.ToString() }, messages);
			resultStrategies[profile.Profile] = health.Id;
		}
		return resultStrategies;
	}

	internal static void InstallRaceCalibration(FuturemudDatabaseContext db, Race race, string profile, ICollection<string> messages,
		double[]? basePools = null, double? size = null)
	{
		var policy = Profiles.Single(x => x.Profile == profile);
		var values = new Dictionary<string, string> { [BasePools] = Xml(basePools ?? DefaultBasePools), [SizeBonus] = Xml(size ?? policy.Size), [CreationBonus] = Xml(policy.Creation) };
		var key = "arm.health." + profile + ".register-values";
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		var created = record is null;
		if (created)
		{
			record = new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = RegisterModule, StableKey = key, EntityType = "RaceHealthValues", LogicalId = race.Id, ManifestVersion = ArmageddonSkillPackage.Version, AppliedAt = DateTime.UtcNow };
			db.SeederManagedRecords.Add(record);
		}
		else if (record!.LogicalId != race.Id || !ValidRegisterRecord(db, record)) throw new InvalidOperationException("Invalid race health calibration ownership: " + profile);
		var rows = db.VariableValues.Where(x => x.ReferenceTypeDefinition == Owner && x.ReferenceId == race.Id).ToArray();
		var current = values.ToDictionary(x => x.Key, x => JsonSerializer.Serialize(rows.SingleOrDefault(y => y.ReferenceProperty == x.Key)?.ValueDefinition ?? x.Value));
		var next = SeederManagedRecordReconciler.Reconcile(record!, current, values.ToDictionary(x => x.Key, x => JsonSerializer.Serialize(x.Value)), created, messages);
		foreach (var value in values)
		{
			var row = rows.SingleOrDefault(x => x.ReferenceProperty == value.Key);
			if (row is null)
			{
				if (!created) throw new InvalidOperationException("Missing race health calibration: " + value.Key);
				row = new VariableValue { ReferenceTypeDefinition = Owner, ReferenceId = race.Id, ReferenceProperty = value.Key,
					ValueTypeDefinition = (value.Key == BasePools ? ProgVariableTypes.Number | ProgVariableTypes.Collection : ProgVariableTypes.Number).ToStorageString() };
				db.VariableValues.Add(row);
			}
			row.ValueDefinition = JsonSerializer.Deserialize<string>(next[value.Key])!;
		}
		record!.ManifestVersion = ArmageddonSkillPackage.Version;
		record.AppliedAt = DateTime.UtcNow;
		db.SaveChanges();
	}
}
