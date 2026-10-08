using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MudSharp.Migrations;
using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunRoomSpatialRegexRegression()
	{
		string Env(string key) => Environment.GetEnvironmentVariable(key) ?? throw new InvalidOperationException("Missing private regression input: " + key);
		var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
		var workspace = Directory.GetParent(repository)!.FullName;
		var diagnosis = Path.GetFullPath(Env("FUTUREMUD_CELL_REGEX_DIAGNOSIS"));
		var output = Path.GetFullPath(Env("FUTUREMUD_CELL_REGEX_OUTPUT"));
		Require(diagnosis.StartsWith(Path.Combine(workspace, "fullboot-qualification") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase), "Diagnosis must be retained owned-fixture evidence.");
		Require(output.StartsWith(Path.Combine(workspace, "regex-qualification") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && !Directory.Exists(output), "Regression output must be a new owned directory.");
		using var metadata = JsonDocument.Parse(File.ReadAllText(diagnosis));
		var representative = metadata.RootElement.GetProperty("RepresentativePayload");
		Require(representative.GetProperty("Status").GetString() == "REPRODUCED" && !representative.GetProperty("SensitiveColumn").GetBoolean(), "A non-sensitive fixture reproduction is required.");
		var payloadFile = Path.GetFullPath(representative.GetProperty("PayloadFile").GetString()!);
		Require(Path.GetDirectoryName(payloadFile) == Path.GetDirectoryName(diagnosis), "Payload must belong to the exact diagnosis directory.");
		var payload = File.ReadAllText(payloadFile);
		var payloadHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));
		Require(payloadHash == representative.GetProperty("Sha256").GetString() && payload.Length == representative.GetProperty("Characters").GetInt32(), "Representative payload differs from retained failure evidence.");
		var oldPattern = metadata.RootElement.GetProperty("CandidatePattern").GetString()!;
		var migrations = new (Migration Migration, string Procedure, string Argument)[]
		{
			(new CellSpatialExpansion(), "fm_cell_spatial_preflight_20261006143539", ""),
			(new CellSpatialContraction(), "fm_cell_spatial_contract_20261006161646", "FALSE")
		};
		var guards = migrations.Select(x => (Sql: x.Migration.UpOperations.OfType<SqlOperation>().First().Sql, x.Procedure, x.Argument)).ToArray();
		string Pattern(string sql)
		{
			var matches = Regex.Matches(sql, @"SET @fm_cell_spatial_pattern='(?<pattern>(?:''|[^'])*)';", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
			Require(matches.Count == 2, "Expected exact production type/general pattern assignments.");
			return matches[1].Groups["pattern"].Value.Replace("''", "'");
		}
		var pattern = Pattern(guards[0].Sql);
		const string oldPrefix = "([[:alnum:]_]*Type";
		Require(oldPattern.StartsWith(oldPrefix, StringComparison.Ordinal), "Unexpected original attribute prefix.");
		Require(pattern == Pattern(guards[1].Sql) && pattern == "(Type" + oldPattern[oldPrefix.Length..], "Only the redundant attribute prefix may change.");
		Directory.CreateDirectory(output);
		using var database = TestDatabase.CreateFresh("futuremud_land_", historicalExpanded: true);
		using (var context = NewIndependentContext(database.ConnectionString))
			context.GetService<IMigrator>().Migrate(context.Database.GetMigrations().Single(x => x.EndsWith("_CellUniqueNames", StringComparison.Ordinal)));
		using var connection = new MySqlConnection(new MySqlConnectionStringBuilder(database.ConnectionString) { AllowUserVariables = true }.ConnectionString);
		OwnedConnections.Validate("regex-connection-before-open", connection);
		connection.Open();
		OwnedConnections.Validate("regex-connection-open", connection);
		void Sql(string sql) => new MySqlCommand(sql, connection) { CommandTimeout = 60 }.ExecuteNonQuery();
		int Limit() => Convert.ToInt32(new MySqlCommand("SELECT @@regexp_time_limit", connection).ExecuteScalar(), CultureInfo.InvariantCulture);
		bool Match(string value, string expression)
		{
			using var command = new MySqlCommand("SELECT REGEXP_LIKE(@payload,@pattern,'i')", connection) { CommandTimeout = 10 };
			command.Parameters.AddWithValue("@payload", value); command.Parameters.AddWithValue("@pattern", expression);
			return Convert.ToBoolean(command.ExecuteScalar(), CultureInfo.InvariantCulture);
		}
		var limit = Limit();
		Require(limit == metadata.RootElement.GetProperty("RegexpTimeLimit").GetInt32(), "Regression requires the same unchanged regex resource limit.");
		var reproduced = false;
		try { Match(payload, oldPattern); }
		catch (MySqlException exception) when (exception.Message.Contains("Timeout exceeded in regular expression match", StringComparison.Ordinal)) { reproduced = true; }
		Require(reproduced, "Original production pattern must reproduce the captured timeout.");
		var payloadTimer = Stopwatch.StartNew();
		Require(!Match(payload, pattern), "The captured stock definition must complete and contain no typed Room match.");
		payloadTimer.Stop();
		Sql("CREATE TABLE aaa_regex_payload (Id INT PRIMARY KEY, Payload LONGTEXT, RefType VARCHAR(255)) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci");
		Sql("SET @FutureMUD_CellSpatialMaintenance=1, @FutureMUD_CellSpatialContractionMaintenance=1");
		var cases = new (string Name, string Value, bool RefType, bool Refused, bool CompareOld)[]
		{
			("captured-stock-definition", payload, false, false, false),
			("large-negative-identifier", new string('a', 131072), false, false, false),
			("large-negative-xml", string.Concat(Enumerable.Repeat("<Name>NumberOfFeaturesPerRoom</Name>\n", 4096)), false, false, false),
			("json", "{\"AnchorType\":\"Room\"}", false, true, true),
			("xml", "<AnchorType>Room</AnchorType>", false, true, true),
			("attribute", "TargetType='Room'", false, true, true),
			("case-and-whitespace", "targetTYPE \t= \n\"rOoM\"", false, true, true),
			("xml-whitespace", "<AnchorType> \nroom\t </Anything>", false, true, true),
			("unicode-prefix", "前缀Type='Room'", false, true, true),
			("mismatched-quotes", "Type='Room\"", false, true, true),
			("embedded-type", "junkTargetType='Room'junk", false, true, true),
			("type-column", " Room ", true, true, false),
			("type-column-case", "\tROOM\n", true, true, false),
			("type-column-cell", "Cell", true, false, false),
			("cell-type", "Type=\"Cell\"", false, false, true),
			("plural", "Type='Rooms'", false, false, true),
			("bare-builder-vocabulary", "Build room 100 beside the Room cellar.", false, false, true),
			("escaped-json-unchanged", "{\"Type\":\"\\u0052oom\"}", false, false, true),
			("long-attribute-prefix", new string('a', 131072) + "Type='Room'", false, true, false),
			("long-xml-prefix", "<" + new string('a', 1024) + "Type>Room</x>", false, true, false)
		};
		var results = new List<object>();
		var status = "FAIL";
		try
		{
			foreach (var guard in guards)
			{
				if (guard.Argument == "FALSE")
				{
					Sql("DELETE FROM aaa_regex_payload");
					using var context = NewIndependentContext(database.ConnectionString);
					context.GetService<IMigrator>().Migrate(context.Database.GetMigrations().Single(x => x.EndsWith("_CellSpatialExpansion", StringComparison.Ordinal)));
				}
				Sql(guard.Sql);
				try
				{
					foreach (var test in cases)
					{
						Sql("DELETE FROM aaa_regex_payload");
						using (var insert = new MySqlCommand("INSERT INTO aaa_regex_payload VALUES(1,@payload,@type)", connection))
						{
							insert.Parameters.AddWithValue("@payload", test.RefType ? DBNull.Value : test.Value);
							insert.Parameters.AddWithValue("@type", test.RefType ? test.Value : DBNull.Value); insert.ExecuteNonQuery();
						}
						if (test.CompareOld) Require(Match(test.Value, oldPattern) == Match(test.Value, pattern), "Original/corrected semantics differ: " + test.Name);
						var schema = CaptureSpatialSchema(connection);
						var values = CaptureSpatialValues(connection, schema);
						var definitions = CaptureSpatialTableDefinitions(connection, schema);
						var historySchema = new Dictionary<string, string[]> { ["__efmigrationshistory"] = ["MigrationId", "ProductVersion"] };
						var history = CaptureSpatialValues(connection, historySchema);
						var timer = Stopwatch.StartNew();
						var refused = false;
						try { Sql($"CALL `{guard.Procedure}`({guard.Argument})"); }
						catch (MySqlException exception) when (exception.Number == 1644 && exception.Message.Contains("Room reference in aaa_regex_payload.", StringComparison.Ordinal)) { refused = true; }
						Require(refused == test.Refused, "Actual production guard result differs: " + guard.Procedure + "/" + test.Name);
						Require(JsonSerializer.Serialize(schema) == JsonSerializer.Serialize(CaptureSpatialSchema(connection)) && SpatialValuesEqual(values, CaptureSpatialValues(connection, schema)) && SpatialValuesEqual(definitions, CaptureSpatialTableDefinitions(connection, schema)), "Guard mutated original schema/values: " + test.Name);
						Require(SpatialValuesEqual(history, CaptureSpatialValues(connection, historySchema)), "Guard changed migration history.");
						Require(Limit() == limit, "Regex resource limit changed.");
						results.Add(new { Guard = guard.Procedure, Case = test.Name, Refused = refused, OriginalMatchCompared = test.CompareOld, TablesPreserved = schema.Count, ElapsedMilliseconds = timer.ElapsedMilliseconds });
						Console.WriteLine($"CellRegex guard={guard.Procedure} case={test.Name} PASS refused={refused} preservation=PASS");
					}
				}
				finally { Sql($"DROP PROCEDURE `{guard.Procedure}`"); }
			}
			status = "PASS";
			return 0;
		}
		finally
		{
			File.WriteAllText(Path.Combine(output, "regex-regression.json"), JsonSerializer.Serialize(new { Status = status, Diagnosis = diagnosis, DiagnosisSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(diagnosis))), PayloadSha256 = payloadHash, PayloadCharacters = payload.Length, OriginalTimeoutReproduced = reproduced, CorrectedPayloadElapsedMilliseconds = payloadTimer.ElapsedMilliseconds, RegexpTimeLimitBefore = limit, RegexpTimeLimitAfter = Limit(), ProductionPattern = pattern, Guards = guards.Select(x => new { x.Procedure, SqlSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(x.Sql))) }), Cases = results }, new JsonSerializerOptions { WriteIndented = true }));
		}
	}
}
