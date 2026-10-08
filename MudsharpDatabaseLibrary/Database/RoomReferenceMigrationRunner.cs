#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MudSharp.Database;

internal static class RoomReferenceMigrationRunner
{
	private sealed record Table(string Name, HashSet<string> Columns, string? Engine);
	private sealed record Change(string Table, long Id, Dictionary<string, object?> Before, Dictionary<string, object?> After);

	internal static async Task ExecuteAsync(DbCommand markerCommand, string marker, CancellationToken cancellationToken)
	{
		var connection = markerCommand.Connection;
		if (connection?.State != ConnectionState.Open || markerCommand.Transaction is not null)
			throw new InvalidOperationException("Room reference correction requires its original open migration connection without an existing transaction.");
		await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken).ConfigureAwait(false);
		try
		{
			async Task<List<Dictionary<string, object?>>> Query(string sql, params (string Name, object? Value)[] parameters)
			{
				using var command = connection.CreateCommand();
				command.Transaction = transaction;
				command.CommandTimeout = markerCommand.CommandTimeout;
				command.CommandText = sql;
				AddParameters(command, parameters);
				using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
				var rows = new List<Dictionary<string, object?>>();
				while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
				{
					var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
					for (var i = 0; i < reader.FieldCount; i++) row.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
					rows.Add(row);
				}
				return rows;
			}

			var metadata = await Query("""
SELECT c.TABLE_NAME,c.COLUMN_NAME,t.ENGINE FROM information_schema.COLUMNS c
JOIN information_schema.TABLES t ON t.TABLE_SCHEMA=c.TABLE_SCHEMA AND t.TABLE_NAME=c.TABLE_NAME
WHERE c.TABLE_SCHEMA=DATABASE() AND LOWER(c.TABLE_NAME) IN
('cells','rooms','cellroommigrationledger','cellroomcontractionledger','roomspatialcontractionledger',
 'characters','characterinstances','gameitems','bodies','crimes','autobuilderroomtemplates','autobuilderareatemplates','__efmigrationshistory');
""").ConfigureAwait(false);
			var tables = metadata.GroupBy(x => (string)x["TABLE_NAME"]!, StringComparer.Ordinal)
				.Select(x => new Table(x.Key, x.Select(r => (string)r["COLUMN_NAME"]!).ToHashSet(StringComparer.OrdinalIgnoreCase), x.First()["ENGINE"] as string))
				.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
			Table Require(string name, params string[] columns)
			{
				if (!tables.TryGetValue(name, out var table) || columns.Any(x => !table.Columns.Contains(x)))
					throw new InvalidOperationException($"Room reference correction: missing/incompatible {name} schema; partial stages require inspected recovery, not a blind retry.");
				return table;
			}
			var historyTable = Require("__EFMigrationsHistory", "MigrationId");
			var history = (await Query($"SELECT `MigrationId` FROM {Quote(historyTable.Name)} FOR UPDATE;").ConfigureAwait(false))
				.Select(x => (string)x["MigrationId"]!).ToHashSet(StringComparer.Ordinal);
			var expanded = history.Contains("20261006143539_CellSpatialExpansion");
			var contracted = history.Contains("20261006161646_CellSpatialContraction");
			var named = history.Contains("20261007043900_RoomTerminology");
			Table spatial;
			string mappingSql;
			if (marker == RoomReferenceMigrationInterceptor.Repair)
			{
				spatial = Require("Rooms", "Id", "EffectData", "ZoneId");
				var ledger = Require("RoomSpatialContractionLedger", "LegacyRoomId", "RoomId");
				if (!expanded || !contracted || !named || tables.ContainsKey("Cells") || spatial.Columns.Contains("RoomId") || tables.ContainsKey("CellRoomContractionLedger"))
					throw new InvalidOperationException("Room reference correction: inconsistent named schema/history; inspect partial-stage recovery.");
				mappingSql = $"SELECT `LegacyRoomId` AS ParentId,`RoomId` AS ChildId FROM {Quote(ledger.Name)} ORDER BY `LegacyRoomId` FOR UPDATE;";
			}
			else
			{
				spatial = Require("Cells", "Id", "RoomId", "EffectData");
				var parents = Require("Rooms", "Id", "ZoneId");
				var afterExpansion = marker != RoomReferenceMigrationInterceptor.Expansion;
				if (contracted || named || expanded != afterExpansion || spatial.Columns.Contains("ZoneId") != afterExpansion || tables.ContainsKey("RoomSpatialContractionLedger") ||
					(marker == RoomReferenceMigrationInterceptor.Expansion && tables.ContainsKey("CellRoomMigrationLedger")) ||
					(marker != RoomReferenceMigrationInterceptor.Verify && tables.ContainsKey("CellRoomContractionLedger")))
					throw new InvalidOperationException("Room reference correction: inconsistent live schema/history; inspect partial-stage recovery.");
				if (afterExpansion) Require("CellRoomMigrationLedger", "RoomId", "CellId");
				if (marker == RoomReferenceMigrationInterceptor.Verify) Require("CellRoomContractionLedger", "RoomId", "CellId");
				mappingSql = $"SELECT r.`Id` AS ParentId,c.`Id` AS ChildId FROM {Quote(parents.Name)} r LEFT JOIN {Quote(spatial.Name)} c ON c.`RoomId`=r.`Id` ORDER BY r.`Id` FOR UPDATE;";
			}
			var mappingRows = await Query(mappingSql).ConfigureAwait(false);
			var retainedRows = await Query($"SELECT `Id` FROM {Quote(spatial.Name)} ORDER BY `Id` FOR UPDATE;").ConfigureAwait(false);
			var map = new RoomReferenceMap(mappingRows.Select(x => (Id(x["ParentId"]), x["ChildId"] is null ? (long?)null : Id(x["ChildId"]))), retainedRows.Select(x => Id(x["Id"])));

			// These factory discriminators have audited configuration-only serializers. Never select Definition.
			foreach (var (name, supported) in new[]
			{
				("AutobuilderRoomTemplates", new[] { "simple", "room by terrain", "room random description" }),
				("AutobuilderAreaTemplates", new[] { "cylinder", "rectangle", "rectangle diagonals", "terrain rectangle", "terrain feature rectangle", "room by terrain random features" })
			})
			{
				var template = Require(name, "Id", "TemplateType");
				foreach (var row in await Query($"SELECT `Id`,`TemplateType` FROM {Quote(template.Name)} ORDER BY `Id` FOR UPDATE;").ConfigureAwait(false))
				{
					if (row["TemplateType"] is not string kind || !supported.Contains(kind, StringComparer.OrdinalIgnoreCase))
						throw new InvalidOperationException($"Room reference correction: {template.Name} #{Id(row["Id"])} has a custom/unclassified TemplateType; explicitly audit its serializer before cutover. Definition was not inspected or changed.");
				}
			}

			var changes = new List<Change>();
			foreach (var name in new[] { "Characters", "CharacterInstances", "GameItems", "Bodies", spatial.Name, "Crimes" })
			{
				var position = name is "Characters" or "CharacterInstances" or "GameItems";
				var crime = name == "Crimes";
				var columns = crime ? new[] { "ThirdPartyIItemType", "ThirdPartyId" }
					: position ? new[] { "PositionTargetType", "PositionTargetId", "PositionEmote", "EffectData" }
					: new[] { "EffectData" };
				var table = Require(name, new[] { "Id" }.Concat(columns).ToArray());
				var first = true;
				long last = 0;
				while (true)
				{
					var rows = await Query($"SELECT `Id`,{string.Join(",", columns.Select(Quote))} FROM {Quote(table.Name)} WHERE (@first=1 OR `Id`>@last) ORDER BY `Id` LIMIT 256 FOR UPDATE;", ("@first", first ? 1 : 0), ("@last", last)).ConfigureAwait(false);
					if (rows.Count == 0) break;
					foreach (var row in rows)
					{
						var id = Id(row["Id"]);
						var before = new Dictionary<string, object?>();
						var after = new Dictionary<string, object?>();
						void Set(string column, object? value)
						{
							if (Equals(row[column], value)) return;
							before[column] = row[column]; after[column] = value;
						}
						if (position || crime)
						{
							var typeColumn = crime ? "ThirdPartyIItemType" : "PositionTargetType";
							var idColumn = crime ? "ThirdPartyId" : "PositionTargetId";
							var child = map.Convert(row[typeColumn] as string, row[idColumn] is null ? null : Id(row[idColumn]), $"{table.Name} #{id}.{typeColumn}/{idColumn}");
							if (child.HasValue) { Set(typeColumn, "Cell"); Set(idColumn, child.Value); }
						}
						if (position) Set("PositionEmote", RoomReferenceXml.Rewrite(row["PositionEmote"] as string, true, map, $"{table.Name} #{id}.PositionEmote"));
						if (!crime) Set("EffectData", RoomReferenceXml.Rewrite(row["EffectData"] as string, false, map, $"{table.Name} #{id}.EffectData"));
						if (after.Count > 0) changes.Add(new Change(table.Name, id, before, after));
					}
					last = Id(rows.Last()["Id"]); first = false;
				}
			}
			if (marker == RoomReferenceMigrationInterceptor.Verify && changes.Count > 0)
				throw new InvalidOperationException($"Room reference verification: {changes[0].Table} #{changes[0].Id} still contains a legacy Room reference; no parent data may be dropped.");

			// Validate rollback/side-effect boundaries before the first update, even if the last row refused.
			foreach (var tableName in changes.Select(x => x.Table).Distinct(StringComparer.Ordinal))
			{
				if (!string.Equals(tables[tableName].Engine, "InnoDB", StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException($"Room reference correction: {tableName} requires transactional InnoDB storage before correction.");
				var triggers = await Query("SELECT TRIGGER_NAME FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA=DATABASE() AND EVENT_OBJECT_TABLE=@table;", ("@table", tableName)).ConfigureAwait(false);
				if (triggers.Count > 0)
					throw new InvalidOperationException($"Room reference correction: {tableName} has triggers; explicit side-effect disposition required before correction.");
			}
			foreach (var change in changes)
			{
				using var command = connection.CreateCommand();
				command.Transaction = transaction;
				command.CommandTimeout = markerCommand.CommandTimeout;
				var columns = change.After.Keys.ToArray();
				command.CommandText = $"UPDATE {Quote(change.Table)} SET {string.Join(",", columns.Select((x, i) => $"{Quote(x)}=@new{i}"))} WHERE `Id`=@id AND {string.Join(" AND ", columns.Select((x, i) => $"BINARY {Quote(x)} <=> BINARY @old{i}"))};";
				AddParameters(command, new[] { ("@id", (object?)change.Id) }
					.Concat(columns.SelectMany((x, i) => new[] { ($"@new{i}", change.After[x]), ($"@old{i}", change.Before[x]) })).ToArray());
				if (await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false) != 1)
					throw new InvalidOperationException($"Room reference correction: {change.Table} #{change.Id} changed concurrently or was not updated exactly once; keep writers stopped.");
			}
			await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
		}
		catch
		{
			try { await transaction.RollbackAsync(CancellationToken.None).ConfigureAwait(false); } catch { /* Retain the original failure for coordinator recovery. */ }
			throw;
		}
	}

	private static long Id(object? value) => Convert.ToInt64(value, CultureInfo.InvariantCulture);
	internal static string Quote(string value) => "`" + value.Replace("`", "``", StringComparison.Ordinal) + "`";
	private static void AddParameters(DbCommand command, IEnumerable<(string Name, object? Value)> values)
	{
		foreach (var (name, value) in values)
		{
			var parameter = command.CreateParameter(); parameter.ParameterName = name; parameter.Value = value ?? DBNull.Value; command.Parameters.Add(parameter);
		}
	}
}
