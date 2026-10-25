#nullable enable
using System.Xml.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockProgressionAcceptance(TestDatabase database)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var players = TraditionPlayers(database);
		using (var db = factory())
		{
			var attribute = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id;
			ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple, capacityAttribute: attribute);
		}
		var before = PreparedDatabaseChecksum(database);
		var interrupted = ArmageddonProgressionInstaller.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new InvalidOperationException("S2 injected before commit");
		});
		Require(interrupted.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"S2 actual MySQL rollback changed table contents.");
		Console.WriteLine("ARMS2-rollback=passed actual-MySQL all-table-checksums-preserved");
		var lostAck = ArmageddonProgressionInstaller.Install(factory, stage =>
		{
			if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("S2 injected lost acknowledgement");
		});
		Require(lostAck.Status == ArmageddonInstallStatus.CommittedConfirmationFailed, string.Join("\n", lostAck.Messages));
		using (var db = factory())
		{
			Require(!ArmageddonInstallation.Read(db)!.ClassesEnabled, "Standard replay class direction unexpectedly changed.");
			Require(db.MagicCapabilities.Count(x => db.SeederManagedRecords.Any(r => r.Module == ArmageddonProgressionInstaller.Module && r.EntityType == "MagicCapability" && r.LogicalId == x.Id)) == 10 && db.Merits.Count(x => db.SeederManagedRecords.Any(r => r.Module == ArmageddonProgressionInstaller.Module && r.EntityType == "Merit" && r.LogicalId == x.Id)) == 10,
				"Ten staged capability/merit pairs are not durable.");
			Require(!db.SeederManagedRecords.Any(x => x.Module == ArmageddonProgressionInstaller.Module && x.EntityType == "ChargenRole"), "Classes-off created a package Class role.");
			Require(db.MagicResources.Count(x => x.MagicResourceType == 1 && db.SeederManagedRecords.Any(r => r.Module == ArmageddonProgressionInstaller.Module && r.EntityType == "MagicResource" && r.LogicalId == x.Id)) == 8 &&
				db.MagicResources.Count(x => x.Name == "Source Energy" && x.MagicResourceType == 0) == 1,
				"Common cost unit/eight reserve layout changed.");
			Require(db.SeederManagedRecords.Count(x => x.Module == ArmageddonProgressionInstaller.Module && x.EntityType == "TraitDefinition") == 11,
				"Simple produced per-spell proficiency ghosts.");
			var skillIds = db.SeederManagedRecords.Where(x => x.Module == ArmageddonProgressionInstaller.Module && x.EntityType == "TraitDefinition")
				.Select(x => x.LogicalId!.Value).ToArray();
			Require(db.TraitDefinitions.Where(x => skillIds.Contains(x.Id)).All(x => x.BranchMultiplier == 0),
				"S2 first install retained the provider's native BranchMultiplier default instead of explicit stock zero.");
			var source = db.MagicResources.Single(x => x.Name == "Source Energy").Id;
			foreach (var model in db.MagicCapabilities.Where(x => db.SeederManagedRecords.Any(r => r.Module == ArmageddonProgressionInstaller.Module && r.EntityType == "MagicCapability" && r.LogicalId == x.Id)))
			{
				var casting = XElement.Parse(model.Definition).Element("Casting")!;
				Require(!(bool)casting.Attribute("enabled")! && !casting.Elements().Any() && (long)casting.Attribute("source")! == source,
					"S2 installed a partial active roster or an incorrect common source.");
			}
		}
		Require(players == TraditionPlayers(database), "S2 installation changed player tables.");
		var committed = PreparedDatabaseChecksum(database);
		var ownershipBefore = StockOwnershipSnapshot(factory);
		var rerun = ArmageddonProgressionInstaller.Install(factory);
		var ownershipAfter = StockOwnershipSnapshot(factory);
		var changedKeys = ownershipBefore.Keys.Where(x => !ownershipAfter.TryGetValue(x, out var value) || value != ownershipBefore[x]).ToArray();
		Require(rerun.Status == ArmageddonInstallStatus.Completed && committed == PreparedDatabaseChecksum(database),
			"S2 owned rerun churned IDs, baselines or audit times. Changed owned keys: " + string.Join(", ", changedKeys) + "\n" + string.Join("\n", rerun.Messages));
		using (var db = factory())
		{
			var trait = db.TraitDefinitions.Single(x => x.Id == db.SeederManagedRecords.Single(r => r.StableKey == "arm.skill.earth_casting").LogicalId);
			trait.Name = "Builder renamed Earth Casting";
			trait.ChargenBlurb = "Builder keeps this blurb";
			db.SaveChanges();
		}
		Require(ArmageddonProgressionInstaller.Install(factory).Status == ArmageddonInstallStatus.Completed, "Edited S2 rerun failed.");
		using (var db = factory()) Require(db.TraitDefinitions.Any(x => x.Name == "Builder renamed Earth Casting" && x.ChargenBlurb == "Builder keeps this blurb"), "S2 overwrote a builder edit.");
		Require(players == TraditionPlayers(database), "S2 rerun changed players.");
		Console.WriteLine("ARMS2-installation=passed Simple/classes-off native-seeder batch lost-ack recovery stable-IDs no-rerun-churn builder-edits-preserved no-player-writes; four-mode-class matrix:managed-tests; complete-stock-gameplay:NOT_RUN");
		RunStockProgressionFixtureChecks();
	}

	private static Dictionary<string, string> StockOwnershipSnapshot(Func<FuturemudDatabaseContext> factory)
	{
		using var db = factory();
		return db.SeederManagedRecords.Where(x => x.Module == ArmageddonProgressionInstaller.Module).AsEnumerable()
			.ToDictionary(x => x.StableKey, x => JsonSerializer.Serialize(new { x.LogicalId, x.SeedBaseline, x.AppliedAt, x.AppliedFingerprint }));
	}
}
