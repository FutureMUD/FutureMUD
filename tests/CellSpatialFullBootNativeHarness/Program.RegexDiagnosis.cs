using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static object DiagnoseRegexPayload(TestDatabase database, string output, string? query, string? pattern)
	{
		Require(query is not null && pattern is not null, "Missing failed regex scan metadata.");
		var match = Regex.Match(query!, @"FROM `(?<table>(?:``|[^`])+)` WHERE REGEXP_LIKE\(`(?<column>(?:``|[^`])+)`",
			RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
		Require(match.Success, "Unrecognized failed scan query; no diagnostic SQL guessed.");
		var table = match.Groups["table"].Value.Replace("``", "`");
		var column = match.Groups["column"].Value.Replace("``", "`");
		string Quote(string identifier) => "`" + identifier.Replace("`", "``") + "`";
		using var connection = database.OpenOwnedConnection();
		var schema = CaptureSpatialSchema(connection);
		Require(schema.TryGetValue(table, out var columns) && columns.Contains(column), "Failed scan metadata is outside owned schema.");
		var keys = new List<string>();
		using (var command = new MySqlCommand("SELECT COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@table AND CONSTRAINT_NAME='PRIMARY' ORDER BY ORDINAL_POSITION", connection))
		{
			command.Parameters.AddWithValue("@table", table);
			using var reader = command.ExecuteReader();
			while (reader.Read()) keys.Add(reader.GetString(0));
		}
		var rows = new List<(string Payload, Dictionary<string, string> Keys)>();
		var bytes = 0L;
		using (var command = new MySqlCommand($"SELECT {Quote(column)}{(keys.Count == 0 ? "" : "," + string.Join(',', keys.Select(Quote)))} FROM {Quote(table)} WHERE {Quote(column)} IS NOT NULL ORDER BY CHAR_LENGTH({Quote(column)}) DESC LIMIT 100000", connection) { CommandTimeout = 60 })
		using (var reader = command.ExecuteReader())
		{
			while (reader.Read())
			{
				var payload = reader.GetString(0);
				bytes += Encoding.UTF8.GetByteCount(payload);
				Require(bytes <= 64 * 1024 * 1024, "Diagnostic payload budget exceeded.");
				rows.Add((payload, keys.Select((key, index) => (key, value: Convert.ToString(reader.GetValue(index + 1), CultureInfo.InvariantCulture)!)).ToDictionary(x => x.key, x => x.value)));
			}
		}
		var timer = Stopwatch.StartNew();
		var tested = 0;
		foreach (var row in rows)
		{
			Require(timer.Elapsed < TimeSpan.FromSeconds(60), "Diagnostic per-row matching budget exceeded.");
			tested++;
			using var command = new MySqlCommand("SELECT REGEXP_LIKE(@payload,@pattern,'i')", connection) { CommandTimeout = 10 };
			command.Parameters.AddWithValue("@payload", row.Payload); command.Parameters.AddWithValue("@pattern", pattern);
			try { command.ExecuteScalar(); }
			catch (MySqlException exception) when (exception.Message.Contains("Timeout exceeded in regular expression match", StringComparison.Ordinal))
			{
				var payloadFile = Path.Combine(output, "regex-timeout-payload.txt");
				var sensitive = Regex.IsMatch(column, "password|secret|token", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
				if (!sensitive) File.WriteAllText(payloadFile, row.Payload, new UTF8Encoding(false));
				return new { Status = "REPRODUCED", Table = table, Column = column, row.Keys, Characters = row.Payload.Length,
					Utf8Bytes = Encoding.UTF8.GetByteCount(row.Payload), Sha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(row.Payload))),
					PayloadFile = sensitive ? null : payloadFile, SensitiveColumn = sensitive, TestedRows = tested, CandidateRows = rows.Count,
					Query = command.CommandText, Error = exception.Message, ElapsedMilliseconds = timer.ElapsedMilliseconds };
			}
		}
		return new { Status = "NOT_REPRODUCED_PER_ROW", Table = table, Column = column, TestedRows = tested, CandidateRows = rows.Count };
	}
}
