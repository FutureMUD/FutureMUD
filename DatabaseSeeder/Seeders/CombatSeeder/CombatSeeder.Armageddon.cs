#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
	private const string ArmageddonCombatModule = "setting-combat";
	private static readonly IReadOnlyDictionary<string, string> ArmageddonAttackAssignments = new Dictionary<string, string>
	{
		["Two Handed Sword"] = "two_handed", ["Training Two Handed Sword"] = "two_handed",
		["Two Handed Axe"] = "two_handed", ["Training Two Handed Axe"] = "two_handed",
		["Pike"] = "pike_weapons", ["Training Pike"] = "pike_weapons",
		["Quarterstaff"] = "bludgeoning_weapons", ["Training Quarterstaff"] = "bludgeoning_weapons",
		["Knife"] = "knife_weapons", ["Longsword"] = "slashing_weapons",
		["Club"] = "bludgeoning_weapons", ["Improvised Bludgeon"] = "bludgeoning_weapons",
		["Short Spear"] = "piercing_weapons"
	};

	private static void ValidateArmageddonCombat(FuturemudDatabaseContext db, IReadOnlyDictionary<string, string> answers)
	{
		if (!ArmageddonSkillPackage.IsInstalled(db)) throw new InvalidOperationException("Install the early Armageddon SkillPackage first.");
		_ = ArmageddonSkillPackage.Resolve(db);
		if (!answers["parryoption"].Equals("yes", StringComparison.OrdinalIgnoreCase) ||
			!answers["installmuskets"].Equals("no", StringComparison.OrdinalIgnoreCase) ||
			!answers["installguns"].Equals("no", StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException("Armageddon combat requires independent Parry and no firearms.");
		if (!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == ArmageddonCombatModule) &&
            (HasCombatFoundation(db) || db.WeaponTypes.Any() || db.RangedWeaponTypes.Any() || db.ShieldTypes.Any() ||
			 db.CombatActions.Any() || db.ManualCombatCommands.Any()))
			throw new InvalidOperationException("Existing combat foundations are not an Armageddon new-world package. Conversion requires a separate plan.");
	}

	private static void CompleteArmageddonCombat(FuturemudDatabaseContext db)
	{
		var owned = ArmageddonSkillPackage.Resolve(db);
		GatePendingArmageddonActions(db);
		ArmageddonMundaneBindings.BindDisarm(db, owned);
		// Apply exact authored type assignments once, before recording ownership. Parry remains independent.
		foreach (var assignment in ArmageddonAttackAssignments)
		{
			var weapon = db.WeaponTypes.Single(x => x.Name == assignment.Key);
			weapon.AttackTrait = owned["arm.skill." + assignment.Value];
			weapon.ParryTrait = owned["arm.skill.parry"];
		}
		db.SaveChanges();
		foreach (var weapon in db.WeaponTypes.ToArray())
		{
            ArmageddonSettingOwnership.RecordNew(db, ArmageddonCombatModule,
                "arm.weapon." + weapon.Name.ToLowerInvariant().Replace(' ', '_'), weapon,
                nameof(WeaponType.AttackTraitId), nameof(WeaponType.ParryTraitId));
		}
		db.SaveChanges();
		ValidateArmageddonCombatIdentities(db);
	}

	internal static void GatePendingArmageddonActions(FuturemudDatabaseContext db)
	{
		// Native stock manoeuvres are retained as foundations, but are not source-faithful abilities.
		// Apply only to newly installed content; the Armageddon rerun path never resets builder choices.
		var never = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		foreach (var command in db.ManualCombatCommands.Where(x => x.PrimaryVerb == "bash" || x.PrimaryVerb == "kick"))
		{
			command.PlayerUsable = false;
			command.NpcUsable = false;
			command.UsabilityProgId = never;
		}
		foreach (var action in db.CombatActions.Where(x => x.Name == "Bash")) action.UsabilityProgId = never;
		foreach (var attack in db.WeaponAttacks.Where(x => x.Name == "Knife Backstab" || x.Name == "Dagger Backstab" || x.Name == "Blackjack Sapping Blow"))
			attack.FutureProgId = never;
	}

	private static void ValidateArmageddonCombatIdentities(FuturemudDatabaseContext db)
	{
		var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == ArmageddonCombatModule).ToArray();
		if (records.Length == 0) throw new InvalidOperationException("Armageddon combat ownership is missing.");
		foreach (var record in records)
		{
			var weapon = db.WeaponTypes.SingleOrDefault(x => x.Id == record.LogicalId);
			if (record.Retired || record.EntityType != nameof(WeaponType) || weapon is null ||
				!db.TraitDefinitions.Any(x => x.Id == weapon.AttackTraitId && x.Type == 0) ||
				!db.TraitDefinitions.Any(x => x.Id == weapon.ParryTraitId && x.Type == 0) ||
				db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
				throw new InvalidOperationException($"Invalid or competing Armageddon weapon graph: {record.StableKey}.");
		}
	}
}
