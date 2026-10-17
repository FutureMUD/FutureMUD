#nullable enable

using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void QualifyS6Maintenance(TestDatabase database, string cleanSql, string cleanDatabase)
	{
		var folder = Path.Combine(Path.GetDirectoryName(Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_RECEIPT")!)!, "n24-maintenance");
		Directory.CreateDirectory(folder);
		var mainBefore = PreparedDatabaseChecksum(database);
		var completeSql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
		var cases = new List<object>();
		foreach (var scenario in new[] { "retained-edit-clone", "retired", "deleted" })
		{
			Console.WriteLine("ARMS6-N24-start=" + scenario);
			using var target = TestDatabase.CreateFresh("futuremud_land_");
			target.RestoreCatalogueFixture(completeSql, database.Name);
			var players = TraditionPlayers(target);
			var identities = S6MaintenanceIdentities(target);
			Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(target.ConnectionString);
			const string key = "arm.spell.mend_flesh";
			if (scenario == "retained-edit-clone")
			{
				long spellId, capId, cloneId;
				string capXml, cloneXml;
				using (var db = factory())
				{
					var record = db.SeederManagedRecords.Single(x => x.StableKey == key);
					var spell = db.MagicSpells.Single(x => x.Id == record.LogicalId);
					spellId = spell.Id;
					spell.Name = "Builder Retained Mend";
					spell.Blurb = "An explicitly retained builder customization.";
					var capClaim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.capability.sorcerer");
					var capability = db.MagicCapabilities.Single(x => x.Id == capClaim.LogicalId);
					capId = capability.Id;
					var definition = XElement.Parse(capability.Definition);
					definition.Element("ConcentrationCapabilityExpression")!.Value = "2";
					capability.Definition = capXml = definition.ToString();
					var clone = (Db.MagicSpell)db.Entry(spell).CurrentValues.ToObject();
					clone.Id = 0;
					clone.Name = "Builder Independent Remedy";
					clone.Blurb = "An unowned independent builder clone.";
					db.MagicSpells.Add(clone);
					db.SaveChanges();
					cloneId = clone.Id;
					cloneXml = clone.Definition;
				}
				for (var rerun = 0; rerun < 2; rerun++)
				{
					var report = S6MaintenanceRerun(target);
					Require(report.Contains("preserv", StringComparison.OrdinalIgnoreCase), "N24 did not report retained authoring conflicts.");
					using var db = factory();
					Require(db.MagicSpells.Single(x => x.Id == spellId).Name == "Builder Retained Mend" &&
						db.MagicSpells.Single(x => x.Id == spellId).Blurb == "An explicitly retained builder customization.", "N24 overwrote the builder's spell name/blurb.");
					Require(db.MagicCapabilities.Single(x => x.Id == capId).Definition == capXml, "N24 overwrote a builder-edited native capability definition.");
					Require(db.MagicSpells.Single(x => x.Id == cloneId).Definition == cloneXml &&
						!db.SeederManagedRecords.Any(x => x.EntityType == nameof(Db.MagicSpell) && x.LogicalId == cloneId), "N24 adopted or rewrote the independent clone.");
					Require(BaseValueHash(identities) == BaseValueHash(S6MaintenanceIdentities(target)) && players == TraditionPlayers(target), "N24 rerun changed stable owned identities or player state.");
				}
				cases.Add(new { Case = scenario, Status = "PASS", FullReruns = 2, CanonicalSpells = 152, Blueprints = 210,
					StableIdentities = identities.Count, SpellId = spellId, CapabilityId = capId, CloneId = cloneId,
					RetainedFields = "name/blurb/native capability definition", CloneUnowned = true, PlayerStateUnchanged = true });
				Console.WriteLine("ARMS6-N24-pass=" + scenario);
				continue;
			}
			using (var db = factory())
			{
				var claim = db.SeederManagedRecords.Single(x => x.StableKey == key);
				if (scenario == "retired") claim.Retired = true;
				else db.MagicSpells.Remove(db.MagicSpells.Single(x => x.Id == claim.LogicalId));
				db.SaveChanges();
			}
			var before = PreparedDatabaseChecksum(target);
			Exception? refusal = null;
			try { S6MaintenanceRerun(target); }
			catch (InvalidOperationException error) { refusal = error; }
			Require(refusal is not null && refusal.Message.Contains(key, StringComparison.Ordinal), "N24 intentional removal/retirement was not refused with its stable key.");
			Require(before == PreparedDatabaseChecksum(target) && players == TraditionPlayers(target), "N24 refusal wrote content, resurrected stock or changed players.");
			cases.Add(new { Case = scenario, Status = "PASS", ConflictKey = key, Diagnostic = refusal!.Message,
				AllTableContentUnchanged = true, PlayerStateUnchanged = true, NoResurrection = true });
			Console.WriteLine("ARMS6-N24-pass=" + scenario);
		}

		// Genuine interrupted first installation, rather than pretending a deliberate deletion is unstarted work.
		Console.WriteLine("ARMS6-N24-start=genuine-interrupted-first-install");
		using (var target = TestDatabase.CreateFresh("futuremud_land_"))
		{
			target.RestoreCatalogueFixture(cleanSql, cleanDatabase);
			var players = TraditionPlayers(target);
			Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(target.ConnectionString);
			ArmageddonInstallationSettings settings;
			using (var source = NewIndependentContext(database.ConnectionString)) settings = ArmageddonInstallation.Read(source)!;
			ArmageddonInstallation.Prepare(factory, settings.Mode, capacityAttribute: settings.CapacityAttribute);
			var progression = ArmageddonProgressionInstaller.Install(factory);
			Require(progression.Status == ArmageddonInstallStatus.Completed, "N24 partial progression failed: " + string.Join("\n", progression.Messages));
			var catalogue = ArmageddonCatalogueInstaller.Install(factory);
			Require(catalogue.Status == ArmageddonInstallStatus.Completed, "N24 partial catalogue failed: " + string.Join("\n", catalogue.Messages));
			var last = ArmageddonStockSpellInstaller.CanonicalKeys.Last();
			var faults = 0;
			var failed = ArmageddonStockSpellInstaller.Install(factory, (key, point) =>
			{
				if (key != last || point != ArmageddonInstallCheckpoint.BeforeCommit) return;
				faults++;
				throw new IOException("N24 native interruption before final contribution commit.");
			});
			Require(faults == 1 && failed.Status == ArmageddonInstallStatus.Failed, "N24 requested interruption was not observed exactly once.");
			var retained = S6MaintenanceIdentities(target);
			Require(!retained.ContainsKey(last) && ArmageddonStockSpellInstaller.CanonicalKeys.Count(retained.ContainsKey) == 151,
				"N24 interruption lost earlier committed contributions or retained an uncommitted final spell.");
			Require(players == TraditionPlayers(target), "N24 interrupted installation changed players.");
			S6MaintenanceRerun(target);
			var completed = S6MaintenanceIdentities(target);
			Require(retained.All(x => completed.TryGetValue(x.Key, out var value) && value == x.Value) &&
				ArmageddonStockSpellInstaller.CanonicalKeys.All(completed.ContainsKey) && players == TraditionPlayers(target),
				"N24 retry changed committed identities, failed owned completion or changed players.");
			S6MaintenanceRerun(target);
			Require(BaseValueHash(completed) == BaseValueHash(S6MaintenanceIdentities(target)) && players == TraditionPlayers(target), "N24 completed retry was not stable.");
			cases.Add(new { Case = "genuine-interrupted-first-install", Status = "PASS", FaultPoint = last + ":BeforeCommit",
				RetainedCanonicalSpells = 151, CompletedCanonicalSpells = 152, EarlierIdentitiesPreserved = true, PlayerStateUnchanged = true });
		}
		Console.WriteLine("ARMS6-N24-pass=genuine-interrupted-first-install");
		Require(mainBefore == PreparedDatabaseChecksum(database), "N24 child campaigns changed the original current-package world.");
		var receipt = new { Status = "PASS", Scope = "Complete current stock, native MySQL maintenance with fixture-authored builder edits and clone; no claim to builder UI coverage",
			CompleteDumpSha256 = BaseHash(completeSql), Cases = cases, OriginalWorldUnchanged = true, ChildDatabasesDisposed = 4,
			Availability = "Release and all original stock gates remain disabled" };
		File.WriteAllText(Path.Combine(folder, "receipt.json"), JsonSerializer.Serialize(receipt, BaseJsonOptions));
		Console.WriteLine("ARMS6-N24=" + JsonSerializer.Serialize(receipt));
	}

	private static string S6MaintenanceRerun(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return new ArmageddonMagicSeeder().SeedData(db, new Dictionary<string, string>
		{
			[ArmageddonMagicSeeder.InstallQuestion] = "no", [ArmageddonMagicSeeder.FullInstallQuestion] = "yes"
		});
	}

	private static SortedDictionary<string, string> S6MaintenanceIdentities(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return new(db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package)
			.ToDictionary(x => x.StableKey, x => x.Module + ":" + x.EntityType + ":" + x.LogicalId + ":" + x.RevisionNumber + ":" + x.Retired), StringComparer.Ordinal);
	}
}
