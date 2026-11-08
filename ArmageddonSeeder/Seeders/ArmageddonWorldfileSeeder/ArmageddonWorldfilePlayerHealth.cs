#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPlayerHealthPools()
	{
		var messages = new List<string>();
		var strategies = ArmageddonPlayerHealth.InstallDefinitions(_db, messages);
		var targets = _source.CombatCalibration!.EnduranceSaves!.Select((bonus, endurance) => 80.5 + endurance + bonus).ToArray();
		foreach (var policy in ArmageddonPlayerHealth.Profiles)
		{
			var record = _db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.race." + policy.Profile);
			if (record?.LogicalId is not { } id || !strategies.TryGetValue(policy.Profile, out var health)) continue;
			var race = _db.Races.Single(x => x.Id == id);
			var physical = _source.PhysicalGenerators.Single(x => x.Race == policy.SourceRace);
			var size = physical.SizeBonuses!.Sum(x => x.Key * x.Value);
			ArmageddonPlayerHealth.InstallRaceCalibration(_db, race, policy.Profile, messages, targets, size);
			var desired = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
			desired[nameof(race.DefaultHealthStrategyId)] = JsonSerializer.Serialize(health);
			ArmageddonSettingOwnership.Reconcile(_db, record, race, desired, messages);
			Record("player.health." + policy.Profile, "ArmageddonPlayerHealthBinding", health, null,
				new { Race = id, ExpectedSizeBonus = size, Disposition = "Shared Setting health formulas read native race registers; private import supplies calibrated expected values. No saved character state changed." });
		}
		foreach (var message in messages) Adapt("player health ownership", 0, message);
		Flush();
	}
}
