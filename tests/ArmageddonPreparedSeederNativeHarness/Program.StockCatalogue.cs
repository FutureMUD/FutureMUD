#nullable enable
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockCatalogueAcceptance(TestDatabase database, Assembly assembly, bool qualifyReruns = true)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		using (var db = factory()) ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple,
			capacityAttribute: db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id);
		var progression = ArmageddonProgressionInstaller.Install(factory);
		Require(progression.Status == ArmageddonInstallStatus.Completed, string.Join("\n", progression.Messages));
		var players = TraditionPlayers(database);
		if (qualifyReruns)
		{
			var stopped = ArmageddonCatalogueInstaller.Install(factory, (batch, stage) =>
			{ if (batch == "specialised-stock" && stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new IOException("S3 composite later-batch interruption"); });
			Require(stopped.Status == ArmageddonInstallStatus.Failed, "Composite did not stop at the later batch: " + string.Join("\n", stopped.Messages));
			using (var db = factory()) Require(db.NpcTemplates.Count(x => x.UniqueName.StartsWith("arm.npc.")) == 49 &&
				!db.SeederManagedRecords.Any(x => x.StableKey == "arm.item.restorative_mud"), "Later rollback lost earlier templates or retained specialised stock.");
			var lostAck = ArmageddonCatalogueInstaller.Install(factory, (batch, stage) =>
			{ if (batch == "specialised-stock" && stage == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("S3 composite lost acknowledgement"); });
			Require(lostAck.Status == ArmageddonInstallStatus.CommittedConfirmationFailed, string.Join("\n", lostAck.Messages));
			var before = PreparedDatabaseChecksum(database);
			var rerun = ArmageddonCatalogueInstaller.Install(factory);
			Require(rerun.Status == ArmageddonInstallStatus.Completed && before == PreparedDatabaseChecksum(database),
				"Composite rerun changed native rows: " + string.Join("\n", rerun.Messages));
		}
		else
		{
			var installed = ArmageddonCatalogueInstaller.Install(factory);
			Require(installed.Status == ArmageddonInstallStatus.Completed, string.Join("\n", installed.Messages));
		}
		Require(players == TraditionPlayers(database), "Catalogue installation changed player state.");
		using (var db = factory())
		{
			foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => !x.DeferredToS4))
				Require(db.SeederManagedRecords.Any(x => x.StableKey == blueprint.Key && !x.Retired), "Missing native blueprint: " + blueprint.Key);
			Require(db.Database.GetPendingMigrations().Count() == 0, "Catalogue target did not apply the track migration.");
		}
		RunStockAnatomyReader(database.Name);
		if (!qualifyReruns) SaveCatalogueFixture(database);
		Console.WriteLine(qualifyReruns
			? "ARMS3-composite=passed actual-MySQL later-rollback retained-earlier-batches lost-ack resume all-198-blueprints no-op-all-table-checksum no-player-writes"
			: "ARMS3-composite-boot-install=passed actual-MySQL fresh-all-198-blueprints no-player-writes rerun-fault-gate-not-repeated");
		RunPreparedReplayBoot(database, assembly, "PreparedCatalogueBootSmoke.py", "prepared-catalogue-boot-native");
	}
}
