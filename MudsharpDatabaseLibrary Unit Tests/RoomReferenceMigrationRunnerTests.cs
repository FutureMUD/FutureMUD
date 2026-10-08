#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomReferenceMigrationRunnerTests
{
	private static CommandEventData Event(DbCommand command, bool async = false, CommandSource source = CommandSource.Migrations) =>
		new(null!, (_, _) => "", command.Connection!, command, null, DbCommandMethod.ExecuteNonQuery,
			Guid.NewGuid(), Guid.NewGuid(), async, false, DateTimeOffset.UtcNow, source);

	[DataTestMethod]
	[DataRow("expansion", false)]
	[DataRow("contraction", true)]
	[DataRow("repair", false)]
	[DataRow("repair", true)]
	public async Task Interceptor_FreshStagedAndAppliedNaming_MapsAllOwnersAndIsIdempotent(string stage, bool async)
	{
		using var db = new FakeConnection(stage);
		using var marker = db.Marker(stage);
		var interceptor = new RoomReferenceMigrationInterceptor();
		for (var run = 0; run < 2; run++)
		{
			var result = async
				? await interceptor.NonQueryExecutingAsync(marker, Event(marker, true), default)
				: interceptor.NonQueryExecuting(marker, Event(marker), default);
			Assert.IsTrue(result.HasResult);
			Assert.AreEqual(0, result.Result);
		}
		Assert.AreEqual(2, db.Commits);
		Assert.AreEqual(0, db.Rollbacks);
		Assert.AreEqual(6, db.Updates); // three positional rows (including item tether), Body, spatial and Crime
		foreach (var name in new[] { "Characters", "CharacterInstances", "GameItems" })
		{
			Assert.AreEqual("Cell", db.Rows[name][0]["PositionTargetType"]);
			Assert.AreEqual(8101L, db.Rows[name][0]["PositionTargetId"]);
			StringAssert.Contains((string)db.Rows[name][0]["PositionEmote"]!, "TargetId='8101'");
			Assert.AreEqual(9000L, db.Rows[name][1]["PositionTargetId"]); // colliding Cell namespace never remapped
		}
		Assert.AreEqual(8101L, db.Rows["Crimes"][0]["ThirdPartyId"]);
		Assert.AreEqual(9000L, db.Rows["Crimes"][0]["LocationId"]); // Cell-valued crime location is not part of a typed update
		Assert.AreEqual(db.TemplateDefinition, db.Rows["AutobuilderRoomTemplates"][0]["Definition"]);
		Assert.IsTrue(db.Sql.All(x => !x.Contains("REGEXP") && !x.Contains("`Definition`")));
		Assert.IsTrue(db.Sql.Any(x => stage == "repair" ? x.Contains("`LegacyRoomId` AS ParentId") : x.Contains("ON c.`RoomId`=r.`Id`")));
		Assert.IsFalse(db.Sql.Any(x => x.Contains("FROM `CellRoomMigrationLedger`")));
		Assert.AreEqual(0, db.Opens);
		Assert.IsTrue(db.Sql.All(x => !x.StartsWith("SIGNAL") && !x.StartsWith("-- FutureMUD")));
	}

	[TestMethod]
	public void Runner_NullAndZeroReferences_DoNotInventTargets()
	{
		using var db = new FakeConnection("expansion");
		db.Rows["Characters"][0]["PositionTargetId"] = null;
		db.Rows["Characters"][0]["PositionEmote"] = null;
		db.Rows["Crimes"][0]["ThirdPartyId"] = null;
		using var command = db.Marker("expansion");
		new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default);
		Assert.IsNull(db.Rows["Characters"][0]["PositionTargetId"]);
		Assert.AreEqual("Room", db.Rows["Characters"][0]["PositionTargetType"]);
		Assert.IsNull(db.Rows["Crimes"][0]["ThirdPartyId"]);
	}

	[TestMethod]
	public void Runner_KeysetPages_UnsortedPhysicalRowsAreVisitedExactlyOnce()
	{
		using var db = new FakeConnection("expansion");
		for (var i = 600; i >= 10; i--)
		{
			db.Rows["Characters"].Add(new() { ["Id"] = (long)i, ["PositionTargetType"] = "Room", ["PositionTargetId"] = 9000L, ["PositionEmote"] = "", ["EffectData"] = "<Effects/>" });
		}
		using var command = db.Marker("expansion");
		new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default);
		Assert.AreEqual(597, db.Updates);
		Assert.IsTrue(db.Rows["Characters"].Where(x => (long)x["Id"]! != 2).All(x => Equals(x["PositionTargetId"], 8101L)));
	}

	[DataTestMethod]
	[DataRow("unmapped")]
	[DataRow("empty")]
	[DataRow("duplicate")]
	[DataRow("malformed")]
	[DataRow("custom room")]
	[DataRow("custom area")]
	[DataRow("custom effect")]
	[DataRow("engine")]
	[DataRow("trigger")]
	[DataRow("missing column")]
	[DataRow("partial expansion")]
	[DataRow("partial contraction")]
	[DataRow("partial naming")]
	public void Runner_PrevalidationFailure_DoesNotWriteAnyRow(string scenario)
	{
		using var db = new FakeConnection("expansion");
		switch (scenario)
		{
			case "unmapped": db.Rows["Crimes"][0]["ThirdPartyId"] = 99999L; break;
			case "empty": db.MapRows[0]["ChildId"] = null; break;
			case "duplicate": db.MapRows.Add(new() { ["ParentId"] = 9000L, ["ChildId"] = 77L }); break;
			case "malformed": db.Rows["Crimes"][0]["ThirdPartyIItemType"] = "room"; break;
			case "custom room": db.Rows["AutobuilderRoomTemplates"][0]["TemplateType"] = "custom"; break;
			case "custom area": db.Rows["AutobuilderAreaTemplates"][0]["TemplateType"] = "custom"; break;
			case "custom effect": db.Rows["Bodies"][0]["EffectData"] = "<Effects><Effect><Type>CustomEffect</Type><Effect/></Effect></Effects>"; break;
			case "engine": db.Engines["Crimes"] = "MyISAM"; break;
			case "trigger": db.Triggered.Add("Crimes"); break;
			case "missing column": db.Columns["CharacterInstances"].Remove("EffectData"); break;
			case "partial expansion": db.Columns["Cells"].Add("ZoneId"); break;
			case "partial contraction": db.AddSchema("CellRoomContractionLedger", "RoomId", "CellId"); break;
			case "partial naming": db.AddSchema("RoomSpatialContractionLedger", "LegacyRoomId", "RoomId"); break;
		}
		using var command = db.Marker("expansion");
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default));
		Assert.AreEqual(0, db.Updates);
		Assert.AreEqual(0, db.Commits);
		Assert.AreEqual(1, db.Rollbacks);
		Assert.AreEqual("Room", db.Rows["Characters"][0]["PositionTargetType"]);
	}

	[TestMethod]
	public void Runner_LateMalformedXml_ValidatesWholePlanBeforeWriting()
	{
		using var db = new FakeConnection("expansion");
		db.Rows["Bodies"][0]["EffectData"] = "<Effects><Effect>";
		using var command = db.Marker("expansion");
		var error = Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default));
		StringAssert.Contains(error.Message, "Bodies #4.EffectData");
		Assert.AreEqual(0, db.Updates);
	}

	[TestMethod]
	public void Runner_UpdateFailure_RollsBackEarlierUpdatesAndSafeRerunRemapsOnce()
	{
		using var db = new FakeConnection("expansion") { FailUpdate = 2 };
		using var command = db.Marker("expansion");
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default));
		Assert.AreEqual(1, db.Rollbacks);
		Assert.AreEqual("Room", db.Rows["Characters"][0]["PositionTargetType"]);
		db.FailUpdate = 0;
		new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default);
		Assert.AreEqual("Cell", db.Rows["Characters"][0]["PositionTargetType"]);
		Assert.AreEqual(8101L, db.Rows["Characters"][0]["PositionTargetId"]);
	}

	[TestMethod]
	public void Runner_LostCommitAcknowledgement_RerunDoesNotTranslateChildAgain()
	{
		using var db = new FakeConnection("expansion") { ThrowAfterCommit = true };
		using var command = db.Marker("expansion");
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default));
		Assert.AreEqual("Cell", db.Rows["Characters"][0]["PositionTargetType"]);
		Assert.AreEqual(8101L, db.Rows["Characters"][0]["PositionTargetId"]);
		db.ThrowAfterCommit = false;
		new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default);
		Assert.AreEqual(6, db.Updates);
		Assert.AreEqual(8101L, db.Rows["Characters"][0]["PositionTargetId"]);
	}

	[TestMethod]
	public void Runner_VerificationOnly_RefusesRemainingReferencesThenAcceptsCorrectedRows()
	{
		using var db = new FakeConnection("contraction");
		db.AddSchema("CellRoomContractionLedger", "RoomId", "CellId");
		using var command = db.Marker("verify");
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default));
		Assert.AreEqual(0, db.Updates);
		db.RemoveSchema("CellRoomContractionLedger");
		using var correction = db.Marker("contraction");
		new RoomReferenceMigrationInterceptor().NonQueryExecuting(correction, Event(correction), default);
		db.AddSchema("CellRoomContractionLedger", "RoomId", "CellId");
		Assert.IsTrue(new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default).HasResult);
	}

	[TestMethod]
	public void Interceptor_UnrelatedSourceOrAdditionalStatements_AreNotSuppressed()
	{
		using var db = new FakeConnection("expansion");
		using var command = db.Marker("expansion");
		var interceptor = new RoomReferenceMigrationInterceptor();
		Assert.IsFalse(interceptor.NonQueryExecuting(command, Event(command, source: CommandSource.LinqQuery), default).HasResult);
		command.CommandText += "\nDELETE FROM Characters;";
		Assert.IsFalse(interceptor.NonQueryExecuting(command, Event(command), default).HasResult);
		Assert.AreEqual(0, db.Begins);
	}

	[TestMethod]
	public void Runner_ExistingTransactionOrClosedConnection_RefusesWithoutOpening()
	{
		using var db = new FakeConnection("expansion");
		using var command = db.Marker("expansion");
		command.Transaction = new FakeTransaction(db);
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default));
		command.Transaction = null; db.CurrentState = ConnectionState.Closed;
		Assert.ThrowsException<InvalidOperationException>(() => new RoomReferenceMigrationInterceptor().NonQueryExecuting(command, Event(command), default));
		Assert.AreEqual(0, db.Opens);
	}

	[TestMethod]
	public async Task Runner_Cancellation_RollsBackWithoutSuppressingMarker()
	{
		using var db = new FakeConnection("expansion");
		using var command = db.Marker("expansion");
		using var cancellation = new CancellationTokenSource();
		db.CancelOnQuery = cancellation;
		try { await new RoomReferenceMigrationInterceptor().NonQueryExecutingAsync(command, Event(command, true), default, cancellation.Token); Assert.Fail("Expected cancellation"); }
		catch (OperationCanceledException) { }
		Assert.AreEqual(0, db.Commits);
		Assert.AreEqual(1, db.Rollbacks);
		Assert.AreEqual(0, db.Updates);
	}

	private sealed class FakeConnection : DbConnection
	{
		internal readonly Dictionary<string, List<Dictionary<string, object?>>> Rows = new();
		internal readonly Dictionary<string, HashSet<string>> Columns = new();
		internal readonly Dictionary<string, string> Engines = new();
		internal readonly HashSet<string> Triggered = new();
		internal readonly List<Dictionary<string, object?>> MapRows = new() { new() { ["ParentId"] = 9000L, ["ChildId"] = 8101L }, new() { ["ParentId"] = 8101L, ["ChildId"] = 77L } };
		internal readonly List<string> Sql = new();
		internal string TemplateDefinition = new('x', 1296681);
		internal int Begins, Commits, Rollbacks, Updates, Opens, FailUpdate;
		internal bool ThrowAfterCommit;
		internal CancellationTokenSource? CancelOnQuery;
		internal ConnectionState CurrentState = ConnectionState.Open;
		internal FakeConnection(string stage)
		{
			foreach (var name in new[] { "Characters", "CharacterInstances", "GameItems" })
			{
				AddSchema(name, "Id", "PositionTargetType", "PositionTargetId", "PositionEmote", "EffectData");
				Rows[name].Add(new() { ["Id"] = 1L, ["PositionTargetType"] = "Room", ["PositionTargetId"] = 9000L, ["PositionEmote"] = "<Emote><Token Type='Perceivable' TargetType='Room' TargetId='9000'/></Emote>", ["EffectData"] = name == "GameItems" ? Tether() : "<Effects/>" });
				Rows[name].Add(new() { ["Id"] = 2L, ["PositionTargetType"] = "Cell", ["PositionTargetId"] = 9000L, ["PositionEmote"] = "", ["EffectData"] = "<Effects/>" });
			}
			AddSchema("Bodies", "Id", "EffectData"); Rows["Bodies"].Add(new() { ["Id"] = 4L, ["EffectData"] = Tether() });
			AddSchema("Crimes", "Id", "ThirdPartyIItemType", "ThirdPartyId", "LocationId");
			Rows["Crimes"].Add(new() { ["Id"] = 5L, ["ThirdPartyIItemType"] = "Room", ["ThirdPartyId"] = 9000L, ["LocationId"] = 9000L });
			AddSchema("__EFMigrationsHistory", "MigrationId");
			AddSchema("AutobuilderRoomTemplates", "Id", "TemplateType", "Definition");
			Rows["AutobuilderRoomTemplates"].Add(new() { ["Id"] = 2L, ["TemplateType"] = "room by terrain", ["Definition"] = TemplateDefinition });
			AddSchema("AutobuilderAreaTemplates", "Id", "TemplateType", "Definition");
			Rows["AutobuilderAreaTemplates"].Add(new() { ["Id"] = 1L, ["TemplateType"] = "rectangle", ["Definition"] = "<Config Type='Room'>opaque</Config>" });
			var spatial = stage == "repair" ? "Rooms" : "Cells";
			AddSchema(spatial, "Id", "EffectData", stage == "repair" ? "ZoneId" : "RoomId");
			Rows[spatial].Add(new() { ["Id"] = 8101L, ["EffectData"] = Tether() });
			Rows[spatial].Add(new() { ["Id"] = 77L, ["EffectData"] = "<Effects/>" });
			if (stage != "repair") AddSchema("Rooms", "Id", "ZoneId");
			if (stage is "contraction" or "repair")
			{
				Rows["__EFMigrationsHistory"].Add(new() { ["MigrationId"] = "20261006143539_CellSpatialExpansion" });
				if (stage == "contraction") { Columns["Cells"].Add("ZoneId"); AddSchema("CellRoomMigrationLedger", "RoomId", "CellId"); }
			}
			if (stage == "repair")
			{
				Rows["__EFMigrationsHistory"].Add(new() { ["MigrationId"] = "20261006161646_CellSpatialContraction" });
				Rows["__EFMigrationsHistory"].Add(new() { ["MigrationId"] = "20261007043900_RoomTerminology" });
				AddSchema("RoomSpatialContractionLedger", "LegacyRoomId", "RoomId");
			}
		}
		private static string Tether() => "<Effects><Effect><Type>ZeroGravityTether</Type><Effect><AnchorType>Room</AnchorType><AnchorId>9000</AnchorId><MaximumRooms>3</MaximumRooms></Effect></Effect></Effects>";
		internal void AddSchema(string name, params string[] columns) { Columns[name] = columns.ToHashSet(); Engines[name] = "InnoDB"; Rows[name] = new(); }
		internal void RemoveSchema(string name) { Columns.Remove(name); Engines.Remove(name); Rows.Remove(name); }
		internal DbCommand Marker(string stage)
		{
			var command = CreateCommand();
			command.CommandText = stage switch { "expansion" => RoomReferenceMigrationInterceptor.Expansion, "contraction" => RoomReferenceMigrationInterceptor.Contraction, "verify" => RoomReferenceMigrationInterceptor.Verify, _ => RoomReferenceMigrationInterceptor.Repair };
			return command;
		}
		internal DbDataReader Read(FakeCommand command)
		{
			Sql.Add(command.CommandText); Assert.IsNotNull(command.Transaction);
			CancelOnQuery?.Cancel();
			var sql = command.CommandText;
			if (sql.Contains("information_schema.COLUMNS"))
				return Reader(new[] { "TABLE_NAME", "COLUMN_NAME", "ENGINE" }, Columns.SelectMany(x => x.Value.Select(c => new object?[] { x.Key, c, Engines[x.Key] })));
			if (sql.Contains("information_schema.TRIGGERS"))
				return Reader(new[] { "TRIGGER_NAME" }, Triggered.Contains((string)command.Parameters["@table"].Value) ? new[] { new object?[] { "custom_trigger" } } : Array.Empty<object?[]>());
			if (sql.Contains("AS ParentId")) return DictionaryReader(MapRows);
			var table = Regex.Match(sql, @"FROM `([^`]+)`").Groups[1].Value;
			var rows = Rows[table].AsEnumerable();
			if (sql.Contains("ORDER BY `Id`")) rows = rows.OrderBy(x => (long)x["Id"]!);
			if (sql.Contains("@first") && (int)command.Parameters["@first"].Value == 0) rows = rows.Where(x => (long)x["Id"]! > (long)command.Parameters["@last"].Value);
			if (sql.Contains("LIMIT 256")) rows = rows.Take(256);
			var columns = Regex.Matches(sql.Split(" FROM ")[0], "`([^`]+)`").Select(x => x.Groups[1].Value).ToArray();
			return Reader(columns, rows.Select(x => columns.Select(c => x[c]).ToArray()));
		}
		internal int Update(FakeCommand command)
		{
			Sql.Add(command.CommandText); Assert.IsNotNull(command.Transaction); Updates++;
			if (FailUpdate == Updates) throw new InvalidOperationException("Injected update failure");
			var table = Regex.Match(command.CommandText, @"UPDATE `([^`]+)`").Groups[1].Value;
			var row = Rows[table].Single(x => (long)x["Id"]! == (long)command.Parameters["@id"].Value);
			foreach (Match match in Regex.Matches(command.CommandText, @"`([^`]+)`=@new(\d+)"))
			{
				var column = match.Groups[1].Value; var index = match.Groups[2].Value;
				Assert.AreEqual(row[column] ?? DBNull.Value, command.Parameters["@old" + index].Value);
				row[column] = command.Parameters["@new" + index].Value is DBNull ? null : command.Parameters["@new" + index].Value;
			}
			return 1;
		}
		private static DbDataReader DictionaryReader(List<Dictionary<string, object?>> rows) => Reader(rows[0].Keys.ToArray(), rows.Select(x => x.Values.ToArray()));
		private static DbDataReader Reader(string[] columns, IEnumerable<object?[]> rows)
		{
			var table = new DataTable(); foreach (var column in columns) table.Columns.Add(column, typeof(object));
			foreach (var row in rows) table.Rows.Add(row.Select(x => x ?? DBNull.Value).ToArray()); return table.CreateDataReader();
		}
		[AllowNull] public override string ConnectionString { get; set; } = "";
		public override string Database => "fake"; public override string DataSource => "fake"; public override string ServerVersion => "8.0.36"; public override ConnectionState State => CurrentState;
		public override void ChangeDatabase(string databaseName) => throw new NotSupportedException();
		public override void Close() => CurrentState = ConnectionState.Closed;
		public override void Open() { Opens++; CurrentState = ConnectionState.Open; }
		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) { Assert.AreEqual(IsolationLevel.Serializable, isolationLevel); Begins++; return new FakeTransaction(this); }
		protected override DbCommand CreateDbCommand() => new FakeCommand(this);
	}

	private sealed class FakeTransaction(FakeConnection db) : DbTransaction
	{
		private bool _committed;
		private readonly Dictionary<string, List<Dictionary<string, object?>>> _original = db.Rows.ToDictionary(x => x.Key, x => x.Value.Select(r => new Dictionary<string, object?>(r)).ToList());
		public override IsolationLevel IsolationLevel => IsolationLevel.Serializable;
		protected override DbConnection DbConnection => db;
		public override void Commit() { _committed = true; db.Commits++; if (db.ThrowAfterCommit) throw new InvalidOperationException("Injected lost commit acknowledgement"); }
		public override void Rollback() { db.Rollbacks++; if (_committed) return; db.Rows.Clear(); foreach (var item in _original) db.Rows.Add(item.Key, item.Value); }
	}
	private sealed class FakeCommand(FakeConnection db) : DbCommand
	{
		private readonly Parameters _parameters = new();
		[AllowNull] public override string CommandText { get; set; } = "";
		public override int CommandTimeout { get; set; } = 30;
		public override CommandType CommandType { get; set; } = CommandType.Text;
		public override bool DesignTimeVisible { get; set; }
		public override UpdateRowSource UpdatedRowSource { get; set; }
		protected override DbConnection? DbConnection { get; set; } = db;
		protected override DbTransaction? DbTransaction { get; set; }
		protected override DbParameterCollection DbParameterCollection => _parameters;
		public override void Cancel() { }
		public override int ExecuteNonQuery() => db.Update(this);
		public override object? ExecuteScalar() => throw new NotSupportedException();
		public override void Prepare() { }
		protected override DbParameter CreateDbParameter() => new Parameter();
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => db.Read(this);
	}
	private sealed class Parameter : DbParameter
	{
		public override DbType DbType { get; set; }
		public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
		public override bool IsNullable { get; set; }
		[AllowNull] public override string ParameterName { get; set; } = "";
		[AllowNull] public override string SourceColumn { get; set; } = "";
		public override object? Value { get; set; }
		public override bool SourceColumnNullMapping { get; set; }
		public override int Size { get; set; }
		public override void ResetDbType() { }
	}
	private sealed class Parameters : DbParameterCollection
	{
		private readonly List<DbParameter> _items = new();
		public override int Count => _items.Count; public override object SyncRoot => this;
		public override int Add(object value) { _items.Add((DbParameter)value); return _items.Count - 1; }
		public override void AddRange(Array values) { foreach (var item in values) Add(item!); }
		public override void Clear() => _items.Clear();
		public override bool Contains(object value) => _items.Contains((DbParameter)value);
		public override bool Contains(string value) => IndexOf(value) >= 0;
		public override void CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);
		public override IEnumerator GetEnumerator() => _items.GetEnumerator();
		public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);
		public override int IndexOf(string parameterName) => _items.FindIndex(x => x.ParameterName == parameterName);
		public override void Insert(int index, object value) => _items.Insert(index, (DbParameter)value);
		public override void Remove(object value) => _items.Remove((DbParameter)value);
		public override void RemoveAt(int index) => _items.RemoveAt(index);
		public override void RemoveAt(string parameterName) => RemoveAt(IndexOf(parameterName));
		protected override DbParameter GetParameter(int index) => _items[index];
		protected override DbParameter GetParameter(string parameterName) => _items[IndexOf(parameterName)];
		protected override void SetParameter(int index, DbParameter value) => _items[index] = value;
		protected override void SetParameter(string parameterName, DbParameter value) => SetParameter(IndexOf(parameterName), value);
	}
}
