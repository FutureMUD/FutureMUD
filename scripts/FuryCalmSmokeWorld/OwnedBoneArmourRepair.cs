#nullable enable

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Character;
using MySql.Data.MySqlClient;

// Qualification fixture only. The caller verifies the owned loopback database and stopped MUD.
internal static class OwnedBoneArmourRepair
{
	private const string OldFormula = "max(damage*0.1,damage-(quality * 2 * strength/115000)))";
	private const string CorrectFormula = "max(damage*0.1,damage-(quality * 2 * strength/115000))";
	private const string SourceHash = "4406b47b2b151d7108e3c2b4f608931ac110cc7ab72ae16342fcbdc0b1403e4b";
	private sealed record ArmourRow(long Id, string Name, int MinimumPenetrationDegree,
		double BaseDifficultyDegrees, double StackedDifficultyDegrees, string Hex);
	private sealed record Repair(long Id, string OldHex, string NewHex, bool Changed);

	internal static void Run(MySqlConnection sql, string runRoot, string receiptPath,
		Func<Dictionary<string, string>> checksums)
	{
		var sourcePath = Path.GetFullPath(Environment.GetEnvironmentVariable("FUTUREMUD_NPC_ARMOUR_SOURCE_RECEIPT")
			?? throw new InvalidOperationException("Missing retained malformed-armour receipt."));
		Require(sourcePath.Equals(Path.Combine(runRoot, "smoke-invocation-880b44100290", "receipt.json"),
			StringComparison.OrdinalIgnoreCase), "Repair requires the exact retained receipt in this owned world.");
		var sourceBytes = File.ReadAllBytes(sourcePath);
		Require(Hash(sourceBytes) == SourceHash, "Retained malformed-armour receipt changed.");
		using var source = JsonDocument.Parse(sourceBytes);
		var root = source.RootElement;
		Require(root.GetProperty("status").GetString() == "FAIL" && root.GetProperty("runnerStatus").GetString() == "FAIL" &&
			root.GetProperty("cleanup").GetProperty("mysqlStopped").GetBoolean() &&
			root.GetProperty("serverProcesses").EnumerateArray().Any() &&
			root.GetProperty("serverProcesses").EnumerateArray().All(x => x.GetProperty("returncode").GetInt32() == 0 &&
				x.GetProperty("collectorStopped").GetBoolean()), "Source must be a stopped failed native receipt.");
		var pinned = root.GetProperty("archiveReferenceEvidence").GetProperty("ArmourTypes").GetString()!
			.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimEnd('\r').Split('\t'))
			.ToDictionary(x => long.Parse(x[0]), x => x[1]);

		ArmourRow[] Read(MySqlTransaction? transaction = null)
		{
			using var command = new MySqlCommand("SELECT Id,Name,MinimumPenetrationDegree,BaseDifficultyDegrees," +
				"StackedDifficultyDegrees,HEX(Definition) FROM ArmourTypes ORDER BY Id" +
				(transaction is null ? "" : " FOR UPDATE"), sql, transaction);
			using var reader = command.ExecuteReader();
			var rows = new List<ArmourRow>();
			while (reader.Read()) rows.Add(new(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2),
				reader.GetDouble(3), reader.GetDouble(4), reader.GetString(5)));
			return rows.ToArray();
		}

		Repair[] Plan(ArmourRow[] rows) => new[] { (5L, "Human Natural Bone Armour"), (42L, "Non-Human Natural Bone Armour") }
			.Select(target =>
			{
				var row = rows.Single(x => x.Id == target.Item1);
				Require(row.Name == target.Item2 && row.MinimumPenetrationDegree == 1 && row.BaseDifficultyDegrees == 0 &&
					row.StackedDifficultyDegrees == 0, "Target metadata changed; retain builder edits.");
				var oldText = new UTF8Encoding(false, true).GetString(Convert.FromHexString(pinned[row.Id]));
				var xml = XElement.Parse(oldText);
				Require(xml.Element("DissipateExpressions")!.Elements("Expression")
					.Single(x => (string?)x.Attribute("damagetype") == "1").Value == OldFormula &&
					oldText.Split(OldFormula, StringSplitOptions.None).Length == 2, "Pinned stock formula is not the exact known defect.");
				var corrected = oldText.Replace(OldFormula, CorrectFormula, StringComparison.Ordinal);
				var newHex = Convert.ToHexString(Encoding.UTF8.GetBytes(corrected));
				Require(row.Hex == pinned[row.Id] || row.Hex == newHex, "Complete target definition changed; retain builder edits.");
				Require(!NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(MudSharp.Models.ArmourType),
					"Definition", corrected, 10, 11), "Corrected stock definition must pass the unchanged typed classifier.");
				return new Repair(row.Id, pinned[row.Id], newHex, row.Hex != newHex);
			}).ToArray(); // Validate both complete rows before the first UPDATE.

		var beforeChecksums = checksums();
		var before = Read();
		var plans = Plan(before);
		Require(plans.All(x => x.Changed) || plans.All(x => !x.Changed), "Repair requires both retained old rows or both exact corrected rows.");
		var preservedBuilderEdits = new List<string>();
		foreach (var target in plans)
		{
			foreach (var edit in new[] { "definition", "metadata" })
			{
				var edited = before.Select(row => row.Id != target.Id ? row : edit == "definition" ?
					row with { Hex = row.Hex + "20" } : row with { BaseDifficultyDegrees = 1 }).ToArray();
				try { Plan(edited); throw new Exception("Builder edit was accepted."); }
				catch (InvalidOperationException) { preservedBuilderEdits.Add($"{target.Id}:{edit}:REFUSED"); }
			}
		}

		int Apply(bool failSecondCompare = false)
		{
			using var transaction = sql.BeginTransaction();
			var lockedPlans = Plan(Read(transaction));
			var changed = 0;
			try
			{
				foreach (var plan in lockedPlans.Where(x => x.Changed))
				{
					using var command = new MySqlCommand("UPDATE ArmourTypes SET Definition=CONVERT(@definition USING utf8mb4) " +
						"WHERE Id=@id AND BINARY Definition=@old", sql, transaction);
					command.Parameters.AddWithValue("@id", plan.Id);
					command.Parameters.AddWithValue("@definition", Convert.FromHexString(plan.NewHex));
					command.Parameters.AddWithValue("@old", failSecondCompare && changed == 1 ? Array.Empty<byte>() : Convert.FromHexString(plan.OldHex));
					Require(command.ExecuteNonQuery() == 1, "Complete-definition compare failed; roll back both rows.");
					changed++;
				}
				transaction.Commit();
				return changed;
			}
			catch { transaction.Rollback(); throw; }
		}

		var rollbackProbe = "NOT_RUN_ALREADY_REPAIRED";
		if (plans.All(x => x.Changed))
		{
			try { Apply(true); throw new Exception("Forced second compare unexpectedly succeeded."); }
			catch (InvalidOperationException error) when (error.Message.StartsWith("Complete-definition compare failed", StringComparison.Ordinal)) { }
			Require(Read().SequenceEqual(before) && Equal(beforeChecksums, checksums()), "Rollback probe changed the world.");
			rollbackProbe = "PASS_SECOND_COMPARE_FAILED_FIRST_UPDATE_ROLLED_BACK";
		}
		var changedRows = Apply();
		var after = Read();
		Require(after.SequenceEqual(before.Select(row => plans.SingleOrDefault(x => x.Id == row.Id) is { } plan ?
			row with { Hex = plan.NewHex } : row)), "Repair changed an unselected row or target metadata.");
		var afterChecksums = checksums();
		Require(Equal(beforeChecksums.Where(x => x.Key != "ArmourTypes"), afterChecksums.Where(x => x.Key != "ArmourTypes")),
			"Repair changed another database table.");
		Require(Apply() == 0 && after.SequenceEqual(Read()) && Equal(afterChecksums, checksums()), "Repair rerun is not a no-op.");
		File.WriteAllText(receiptPath, JsonSerializer.Serialize(new { Status = "PASS", SourceReceipt = sourcePath,
			SourceReceiptSha256 = SourceHash, ChangedRows = changedRows, BuilderEditRefusals = preservedBuilderEdits,
			RollbackProbe = rollbackProbe, RerunChangedRows = 0, Before = before, After = after,
			ChecksumsBefore = beforeChecksums, ChecksumsAfter = afterChecksums }, new JsonSerializerOptions { WriteIndented = true }));
	}

	private static bool Equal(IEnumerable<KeyValuePair<string, string>> first, IEnumerable<KeyValuePair<string, string>> second)
		=> first.OrderBy(x => x.Key).SequenceEqual(second.OrderBy(x => x.Key));
	private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
	private static void Require(bool test, string reason) { if (!test) throw new InvalidOperationException(reason); }
}
