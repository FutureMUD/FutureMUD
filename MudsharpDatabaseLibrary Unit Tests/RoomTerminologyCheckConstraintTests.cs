#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomTerminologyCheckConstraintTests
{
	private const string PayloadName = "CK_VehicleRouteSteps_TypedPayload";
	private static readonly Lazy<string> GeneratedSql = new(GenerateSql);
	private static readonly Lazy<Dictionary<string, Capture>> Captures = new(ReadCaptures);

	[TestMethod]
	public void Naming_EntirePendingUpChain_AccountsForAllChecksAndEveryRenameDependency()
	{
		using var context = Context();
		var assembly = context.GetService<IMigrationsAssembly>();
		var projected = new Dictionary<(string Table, string Name), string>();
		foreach (var pair in assembly.Migrations.Where(x => string.CompareOrdinal(x.Key,
			"20260227120000_ArenaPhaseProgAppearancePayouts") > 0 &&
			string.CompareOrdinal(x.Key, "20261007043900_RoomTerminology") < 0))
		{
			var migration = assembly.CreateMigration(pair.Value, "Pomelo.EntityFrameworkCore.MySql");
			foreach (var operation in migration.UpOperations)
			{
				switch (operation)
				{
					case CreateTableOperation table:
						foreach (var check in table.CheckConstraints) projected.Add((table.Name, check.Name), check.Sql);
						break;
					case AddCheckConstraintOperation check:
						projected.Add((check.Table, check.Name), check.Sql);
						break;
					case DropCheckConstraintOperation check:
						Assert.IsTrue(projected.Remove((check.Table, check.Name)), pair.Key);
						break;
					case DropTableOperation table:
						foreach (var key in projected.Keys.Where(x => x.Table == table.Name).ToArray()) projected.Remove(key);
						break;
					case RenameTableOperation table:
						foreach (var key in projected.Keys.Where(x => x.Table == table.Name).ToArray())
						{
							var expression = projected[key];
							projected.Remove(key);
							projected.Add((table.NewName!, key.Name), expression);
						}
						break;
					case SqlOperation sql:
						Assert.IsFalse(Regex.IsMatch(sql.Sql, @"\b(?:ADD|DROP)\s+(?:CONSTRAINT\s+\S+\s+)?CHECK\b",
							RegexOptions.IgnoreCase), "Unclassified raw CHECK DDL: " + pair.Key);
						break;
				}
			}
		}
		var before = Checks(context, new CellSpatialContraction());
		var target = Checks(context, new RoomTerminology());
		Assert.AreEqual(47, projected.Count);
		Assert.AreEqual(47, before.Count);
		Assert.AreEqual(47, target.Count);
		foreach (var pair in before) Assert.AreEqual(pair.Value, projected[pair.Key], pair.Key.ToString());
		var naming = new RoomTerminology().UpOperations;
		var tableMap = naming.OfType<RenameTableOperation>().ToDictionary(x => x.Name, x => x.NewName!);
		var reverse = tableMap.ToDictionary(x => x.Value, x => x.Key);
		var scope = tableMap.Keys.Concat(naming.OfType<RenameColumnOperation>().Select(x =>
			reverse.GetValueOrDefault(x.Table, x.Table))).ToHashSet();
		Assert.AreEqual(62, scope.Count);
		var dependent = new HashSet<string>();
		foreach (var pair in before)
		{
			var table = tableMap.GetValueOrDefault(pair.Key.Table, pair.Key.Table);
			var name = pair.Key.Name.Replace("Cell", "Room");
			var expected = pair.Value;
			foreach (var column in naming.OfType<RenameColumnOperation>().Where(x => x.Table == table))
				expected = expected.Replace($"`{column.Name}`", $"`{column.NewName}`");
			Assert.AreEqual(expected, target[(table, name)], pair.Key.Name);
			if (table != pair.Key.Table || name != pair.Key.Name || expected != pair.Value) dependent.Add(pair.Key.Name);
			if (scope.Contains(pair.Key.Table))
			{
				var capture = Captures.Value[pair.Key.Name];
				Assert.AreEqual(pair.Key.Table, capture.SourceTable);
				Assert.AreEqual(table, capture.TargetTable);
				Assert.AreEqual(Signature(pair.Value), capture.SourceSignature);
				Assert.AreEqual(Signature(expected), capture.TargetSignature);
				Assert.AreEqual(dependent.Contains(pair.Key.Name), capture.Recreate);
			}
		}
		Assert.AreEqual(28, Captures.Value.Count);
		Assert.AreEqual(10, dependent.Count);
		Assert.AreEqual(37, before.Count - dependent.Count);
		CollectionAssert.AreEquivalent(dependent.ToArray(), naming.OfType<DropCheckConstraintOperation>().Select(x => x.Name).ToArray());
	}

	[TestMethod]
	public void Naming_CheckRemovalAndRestoration_SurroundAllTableAndColumnRenames()
	{
		var operations = new RoomTerminology().UpOperations.ToList();
		var firstRename = operations.FindIndex(x => x is RenameTableOperation or RenameColumnOperation);
		var lastRename = operations.FindLastIndex(x => x is RenameTableOperation or RenameColumnOperation);
		Assert.IsTrue(firstRename > 0);
		foreach (var drop in operations.OfType<DropCheckConstraintOperation>()) Assert.IsTrue(operations.IndexOf(drop) < firstRename, drop.Name);
		var restores = operations.OfType<SqlOperation>()
			.Where(x => x.Sql.Contains("PREPARE fm_room_naming_check_statement_20261007043900")).ToArray();
		Assert.AreEqual(10, restores.Length);
		foreach (var restore in restores) Assert.IsTrue(operations.IndexOf(restore) > lastRename);
		var sql = GeneratedSql.Value;
		Assert.IsTrue(sql.Contains("ALTER TABLE `VehicleRouteSteps` DROP CONSTRAINT `" + PayloadName + "`"));
		Assert.IsTrue(sql.Contains("`OriginRoomId` = `DestinationRoomId`"));
		Assert.IsTrue(sql.IndexOf("CLOSE definitions;", StringComparison.Ordinal) <
			sql.IndexOf("CREATE TEMPORARY TABLE `fm_room_naming_checks_", StringComparison.Ordinal));
		Assert.IsTrue(sql.Contains("CALL `fm_room_terminology_20261007043900`(FALSE)"));
		Assert.IsTrue(sql.Contains("CALL `fm_room_terminology_20261007043900`(TRUE)"));
		Assert.IsFalse(sql.Contains("__CHECK_"));
		Assert.IsFalse(sql.Contains("CREATE TEMPORARY TABLE IF NOT EXISTS"));
		Assert.IsFalse(sql.Contains("DROP TEMPORARY TABLE IF EXISTS"));
		Assert.IsTrue(sql.Contains("DROP TEMPORARY TABLE `fm_room_naming_checks_20261007043900`"));
		Assert.IsFalse(sql.Contains("`)<>28 OR EXISTS"));
		Assert.IsFalse(sql.Contains("`)<>10 OR EXISTS"));
	}

	[DataTestMethod]
	[DataRow("YES", " ENFORCED")]
	[DataRow("NO", " NOT ENFORCED")]
	public void Naming_EachCheckRestore_PreservesBothSupportedEnforcementStates(string flag, string suffix)
	{
		using var context = Context();
		var target = Checks(context, new RoomTerminology());
		foreach (var sql in Restores())
		{
			var selected = RestoreStatement(sql.Sql, flag, true, true);
			var capture = CaptureForRestore(sql.Sql);
			Assert.AreEqual($"ALTER TABLE `{capture.TargetTable}` ADD CONSTRAINT `{capture.TargetName}` CHECK ({target[(capture.TargetTable, capture.TargetName)]}){suffix}", selected);
			Assert.IsTrue(sql.SuppressTransaction);
			Assert.IsTrue(sql.Sql.Contains("DEALLOCATE PREPARE"));
			Assert.IsTrue(sql.Sql.EndsWith("= NULL;", StringComparison.Ordinal));
		}
	}

	[DataTestMethod]
	[DataRow(null, true, true)]
	[DataRow("MAYBE", true, true)]
	[DataRow("yes", true, true)]
	[DataRow("YES", false, true)]
	[DataRow("NO", true, false)]
	public void Naming_LostOrChangedEnforcementCapture_ProducesNoPreparedDdl(string? flag, bool exists, bool recreate)
	{
		foreach (var sql in Restores()) Assert.IsNull(RestoreStatement(sql.Sql, flag, exists, recreate));
	}

	[DataTestMethod]
	[DataRow("model", "YES", null, false)]
	[DataRow("native", "NO", null, false)]
	[DataRow("case-space", "YES", null, false)]
	[DataRow("changed-operator", "YES", null, true)]
	[DataRow("changed-group", "YES", null, true)]
	[DataRow("quoted-string", "YES", null, true)]
	[DataRow("quoted-identifier-space", "YES", null, true)]
	[DataRow("missing", "YES", null, true)]
	[DataRow("model", "MAYBE", null, true)]
	[DataRow("model", "yes", null, true)]
	[DataRow("model", null, null, true)]
	[DataRow("model", "YES", "YES", false)]
	[DataRow("native", "NO", "NO", false)]
	[DataRow("model", "YES", "NO", true)]
	public void Naming_EmittedShapePredicate_RefusesUnreviewedClausesAndEnforcement(string variant,
		string? flag, string? original, bool refused)
	{
		var capture = Captures.Value[PayloadName];
		var expression = variant switch
		{
			"native" => capture.SourceNativeSignature,
			"case-space" => "  " + capture.SourceSignature.ToUpperInvariant() + "  ",
			"changed-operator" => capture.SourceSignature.Replace("originCellId".ToLowerInvariant() + "=", "origincellid<>"),
			"changed-group" => "(" + capture.SourceSignature + ")",
			"quoted-string" => capture.SourceSignature + " AND 'x'='x'",
			"quoted-identifier-space" => capture.SourceSignature.Replace("origincellid", "`Origin CellId`"),
			"missing" => null,
			_ => capture.SourceSignature
		};
		if (original is not null) expression = variant == "native" ? capture.TargetNativeSignature : capture.TargetSignature;
		Assert.AreEqual(refused, EmittedShapeRefuses(expression, flag, original, original is not null));
	}

	[DataTestMethod]
	[DataRow("model")]
	[DataRow("native")]
	public void Naming_Postflight_RequiresRenamedColumnsAndExactCapturedEnforcement(string variant)
	{
		var capture = Captures.Value[PayloadName];
		var target = variant == "native" ? capture.TargetNativeSignature : capture.TargetSignature;
		Assert.IsFalse(EmittedShapeRefuses(target, "NO", "NO", true));
		Assert.IsTrue(EmittedShapeRefuses(target, "YES", "NO", true));
		Assert.IsTrue(EmittedShapeRefuses(capture.SourceSignature, "NO", "NO", true));
		Assert.IsTrue(EmittedShapeRefuses(target, "NO", null, true));
		var sql = GeneratedSql.Value;
		Assert.IsTrue(sql.Contains("BINARY t.ENFORCED<>BINARY expected.OriginalEnforced"));
		Assert.IsTrue(sql.Contains("BINARY t.CONSTRAINT_NAME=BINARY expected.TargetName"));
		Assert.IsTrue(sql.Contains("BINARY t.CONSTRAINT_NAME=BINARY expected.SourceName"));
		Assert.IsTrue(sql.Contains("reviewed check capture is incomplete"));
		Assert.IsTrue(sql.Contains("reviewed check dependency set differs"));
		Assert.IsTrue(sql.Contains("original check enforcement capture is incomplete"));
	}

	[TestMethod]
	public void Naming_EscapedIdentifierCollision_HasDifferentColumnSemanticsAndRefusesBothPhases()
	{
		const string ordinary = "(`TopologyVersion` >= 1)";
		const string escaped = "(`Topology``Version` >= 1)";
		Assert.AreEqual(Signature(ordinary), Signature(escaped), "This fixture reproduces the old normalization collision.");
		using var table = new DataTable();
		table.Columns.Add("TopologyVersion", typeof(int));
		table.Columns.Add("Topology`Version", typeof(int));
		table.Rows.Add(1, 0);
		// Decode MySQL doubled delimiters as an embedded character. Never erase
		// them: the independent row oracle must keep the two columns distinct.
		static string FixtureExpression(string value) => Regex.Replace(value, "`((?:``|[^`])+)`",
			m => "[" + m.Groups[1].Value.Replace("``", "`") + "]");
		Assert.AreEqual(1, table.Select(FixtureExpression(ordinary)).Length);
		Assert.AreEqual(0, table.Select(FixtureExpression(escaped)).Length);
		const string name = "CK_RouteCells_TopologyVersion";
		Assert.IsFalse(EmittedShapeRefuses(ordinary, "YES", null, false, name));
		Assert.IsTrue(EmittedShapeRefuses(escaped, "YES", null, false, name));
		Assert.IsFalse(EmittedShapeRefuses(ordinary, "NO", "NO", true, name));
		Assert.IsTrue(EmittedShapeRefuses(escaped, "NO", "NO", true, name));
		Assert.IsFalse(EmittedShapeRefuses(escaped, "YES", null, false, name, omitEscapeGuard: true),
			"Removing the emitted guard must reproduce the preflight bug.");
		Assert.IsFalse(EmittedShapeRefuses(escaped, "NO", "NO", true, name, omitEscapeGuard: true),
			"Removing the emitted guard must reproduce the postflight bug.");
		Assert.AreEqual(2, Regex.Matches(GeneratedSql.Value, @"OR LOCATE\('``',c.CHECK_CLAUSE\)>0").Count);
	}

	[DataTestMethod]
	[DataRow(false, "`TopologyVersion` >= 1", false)]
	[DataRow(false, "TopologyVersion >= 1", false)]
	[DataRow(false, "(`TopologyVersion` >= 1)", false)]
	[DataRow(false, "(TopologyVersion >= 1)", false)]
	[DataRow(false, "`Topology``Version` >= 1", true)]
	[DataRow(false, "(`Topology``Version` >= 1)", true)]
	[DataRow(true, "`TopologyVersion` >= 1", false)]
	[DataRow(true, "TopologyVersion >= 1", false)]
	[DataRow(true, "(`TopologyVersion` >= 1)", false)]
	[DataRow(true, "(TopologyVersion >= 1)", false)]
	[DataRow(true, "`Topology``Version` >= 1", true)]
	[DataRow(true, "(`Topology``Version` >= 1)", true)]
	public void Naming_EscapedBackticks_RefuseWithoutRejectingOrdinaryIdentifiers(bool postflight,
		string clause, bool refused)
	{
		Assert.AreEqual(refused, EmittedShapeRefuses(clause, "YES", postflight ? "YES" : null,
			postflight, "CK_RouteCells_TopologyVersion"));
	}

	[TestMethod]
	public void Naming_AllScopedChecks_ValidateBothReviewedFormsAndPreserveUntouchedEnforcement()
	{
		foreach (var capture in Captures.Value.Values)
		{
			Assert.IsFalse(EmittedShapeRefuses(capture.SourceSignature, "YES", null, false, capture.SourceName));
			Assert.IsFalse(EmittedShapeRefuses(capture.SourceNativeSignature, "NO", null, false, capture.SourceName));
			Assert.IsFalse(EmittedShapeRefuses(capture.TargetSignature, "YES", "YES", true, capture.SourceName));
			Assert.IsFalse(EmittedShapeRefuses(capture.TargetNativeSignature, "NO", "NO", true, capture.SourceName));
			Assert.IsTrue(EmittedShapeRefuses(capture.TargetSignature, "YES", "NO", true, capture.SourceName));
		}
	}

	[DataTestMethod]
	[DataRow("fixture", "VehicleRouteSteps", PayloadName, "CHECK", true)]
	[DataRow("fixture", "vehicleroutesteps", PayloadName, "CHECK", true)]
	[DataRow("other", "VehicleRouteSteps", PayloadName, "CHECK", false)]
	[DataRow("fixture", "OtherTable", PayloadName, "CHECK", false)]
	[DataRow("fixture", "VehicleRouteSteps", "ck_vehicleroutesteps_typedpayload", "CHECK", false)]
	[DataRow("fixture", "VehicleRouteSteps", "CK_Custom_CellReference", "CHECK", false)]
	[DataRow("fixture", "VehicleRouteSteps", PayloadName, "FOREIGN KEY", false)]
	public void Naming_EmittedMetadataJoin_RequiresExactCheckNameAndCorrectOwner(string schema,
		string table, string name, string kind, bool qualifies)
	{
		var join = Regex.Match(GeneratedSql.Value,
			@"LEFT JOIN information_schema.TABLE_CONSTRAINTS t ON ([\s\S]+?AND t.CONSTRAINT_TYPE='CHECK')");
		Assert.IsTrue(join.Success);
		var capture = Captures.Value[PayloadName];
		var expression = join.Groups[1].Value.Replace("DATABASE()", "'fixture'")
			.Replace("LOWER(t.TABLE_NAME)", Literal(table.ToLowerInvariant()))
			.Replace("LOWER(expected.SourceTable)", Literal(capture.SourceTable.ToLowerInvariant()))
			.Replace("t.CONSTRAINT_SCHEMA", Literal(schema)).Replace("t.CONSTRAINT_TYPE", Literal(kind))
			.Replace("BINARY t.CONSTRAINT_NAME", Literal(name)).Replace("BINARY expected.SourceName", Literal(capture.SourceName));
		using var evaluator = new DataTable { CaseSensitive = true, Locale = CultureInfo.InvariantCulture };
		Assert.AreEqual(qualifies, Convert.ToBoolean(evaluator.Compute(expression, ""), CultureInfo.InvariantCulture));
	}

	[TestMethod]
	public void Naming_UnclassifiedChecks_AreRefusedOnEveryAffectedTableIncludingTablesWithoutKnownChecks()
	{
		var sql = GeneratedSql.Value;
		var scopes = Regex.Matches(sql, @"AND LOWER\(t.TABLE_NAME\) IN \(([^\n]+)\) AND expected.SourceName IS NULL");
		Assert.AreEqual(2, scopes.Count);
		foreach (Match scope in scopes)
		{
			var tables = Regex.Matches(scope.Groups[1].Value, "'([^']+)'").Select(x => x.Groups[1].Value).ToArray();
			Assert.AreEqual(62, tables.Length);
			Assert.IsTrue(tables.Contains("vehicleroutesteps"));
			Assert.IsTrue(tables.Contains("zones"), "Unknown checks on affected tables without model checks must refuse too.");
		}
		Assert.IsTrue(sql.Contains("Room naming: unclassified check "));
		Assert.IsFalse(Captures.Value.ContainsKey(PayloadName.ToLowerInvariant()), "Names are exact and case-sensitive.");
		Assert.IsFalse(Captures.Value.ContainsKey("CK_Custom_CellReference"));
	}

	[DataTestMethod]
	[DataRow(0, 1, 1, 1, true)]
	[DataRow(0, -1, 1, 1, true)]
	[DataRow(0, 0, 1, 1, false)]
	[DataRow(0, 1, 1, 2, false)]
	[DataRow(1, 0, 1, 2, true)]
	[DataRow(2, 1, 1, 1, false)]
	public void Naming_TypedPayload_KeepsRouteTraversalAndExitHopSemantics(int step, int direction,
		int origin, int destination, bool allowed)
	{
		using var context = Context();
		var before = Checks(context, new CellSpatialContraction())[("VehicleRouteSteps", PayloadName)];
		var target = Checks(context, new RoomTerminology())[("VehicleRouteSteps", PayloadName)];
		Assert.AreEqual(before.Replace("`OriginCellId`", "`OriginRoomId`").Replace("`DestinationCellId`", "`DestinationRoomId`"), target);
		var table = new DataTable();
		var columns = Regex.Matches(before, "`([^`]+)`").Select(x => x.Groups[1].Value).Distinct().ToArray();
		foreach (var column in columns) table.Columns.Add(column, typeof(long));
		var row = table.NewRow();
		foreach (var column in columns) row[column] = 1L;
		row["StepType"] = step;
		row["Direction"] = step == 1 ? DBNull.Value : direction;
		row["ExitId"] = step == 1 ? 1L : DBNull.Value;
		row["DistanceMetres"] = step == 1 ? DBNull.Value : 0L;
		row["OriginCellId"] = origin;
		row["DestinationCellId"] = destination;
		table.Rows.Add(row);
		Assert.AreEqual(allowed, table.Select(Regex.Replace(before, "`([^`]+)`", "[$1]")).Length == 1);
		// Rename fixture columns by identity, then evaluate the final predicate.
		table.Columns["OriginCellId"]!.ColumnName = "OriginRoomId";
		table.Columns["DestinationCellId"]!.ColumnName = "DestinationRoomId";
		Assert.AreEqual(allowed, table.Select(Regex.Replace(target, "`([^`]+)`", "[$1]")).Length == 1);
	}

	[TestMethod]
	public void Naming_FinalTargetAndCurrentModel_AgreeAndHistoricalMetadataRetainsOldNames()
	{
		using var context = Context();
		var target = Checks(context, new RoomTerminology());
		var current = context.GetService<IDesignTimeModel>().Model.GetRelationalModel().Tables
			.SelectMany(t => t.CheckConstraints.Select(c => (Key: (t.Name, c.Name), c.Sql)))
			.ToDictionary(x => x.Key, x => x.Sql);
		foreach (var pair in target) Assert.AreEqual(pair.Value, current[pair.Key]);
		Assert.IsFalse(target[("VehicleRouteSteps", PayloadName)].Contains("CellId"));
		Assert.IsTrue(Checks(context, new CellSpatialContraction())[("VehicleRouteSteps", PayloadName)].Contains("`OriginCellId`"));
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseMySql("server=localhost;database=unused;uid=unused;password=unused", ServerVersion.Parse("8.0.45-mysql")).Options);

	private static Dictionary<(string Table, string Name), string> Checks(FuturemudDatabaseContext context, Migration migration) =>
		context.GetService<IModelRuntimeInitializer>().Initialize(migration.TargetModel, designTime: true).GetRelationalModel()
			.Tables.SelectMany(t => t.CheckConstraints.Select(c => (Key: (t.Name, c.Name), c.Sql))).ToDictionary(x => x.Key, x => x.Sql);

	private static string GenerateSql()
	{
		using var context = Context();
		var migration = new RoomTerminology();
		var model = context.GetService<IModelRuntimeInitializer>().Initialize(migration.TargetModel, designTime: true);
		return string.Join("\n", context.GetService<IMigrationsSqlGenerator>().Generate(migration.UpOperations, model)
			.Select(x => x.CommandText)).Replace("\r\n", "\n");
	}

	private sealed record Capture(string SourceTable, string SourceName, string TargetTable, string TargetName,
		string SourceSignature, string SourceNativeSignature, string TargetSignature, string TargetNativeSignature, bool Recreate);

	private static Dictionary<string, Capture> ReadCaptures() => Regex.Matches(GeneratedSql.Value,
		@"INSERT INTO `fm_room_naming_checks_20261007043900` VALUES \('([^']+)','([^']+)','([^']+)','([^']+)','([^']+)','([^']+)','([^']+)','([^']+)',([01]),NULL\);")
		.Select(m => new Capture(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value,
			m.Groups[5].Value, m.Groups[6].Value, m.Groups[7].Value, m.Groups[8].Value, m.Groups[9].Value == "1"))
		.ToDictionary(x => x.SourceName, StringComparer.Ordinal);

	private static SqlOperation[] Restores() => new RoomTerminology().UpOperations.OfType<SqlOperation>()
		.Where(x => x.Sql.Contains("PREPARE fm_room_naming_check_statement_20261007043900")).ToArray();

	private static Capture CaptureForRestore(string sql) => Captures.Value[Regex.Match(sql, "WHERE BINARY SourceName='([^']+)'").Groups[1].Value];

	// Bounded managed adapter for the emitted selector, not a native SQL parser.
	private static string? RestoreStatement(string sql, string? flag, bool exists, bool recreate)
	{
		Assert.IsTrue(sql.Contains("WHERE BINARY SourceName="));
		Assert.IsTrue(sql.Contains("AND NeedsRecreate=1"));
		Assert.IsTrue(sql.Contains("AND BINARY OriginalEnforced IN ('YES','NO')"));
		if (!exists || !recreate || flag is not ("YES" or "NO")) return null;
		var branches = Regex.Match(sql, "CASE BINARY OriginalEnforced WHEN 'YES' THEN '([^']+)' WHEN 'NO' THEN '([^']+)' END");
		Assert.IsTrue(branches.Success);
		var statement = Regex.Match(sql, @"SELECT CONCAT\('([^']+)',CASE").Groups[1].Value;
		return statement + branches.Groups[flag == "YES" ? 1 : 2].Value;
	}

	private static string Signature(string value) => Regex.Replace(value.Replace("`", ""), @"\s", "").ToLowerInvariant();

	// Evaluate the actual emitted OR predicate against managed metadata fixtures.
	// MySQL parser, session and physical DDL still need native qualification.
	private static bool EmittedShapeRefuses(string? clause, string? flag, string? original, bool postflight,
		string captureName = PayloadName, bool omitEscapeGuard = false)
	{
		var guards = Regex.Matches(GeneratedSql.Value, @"WHERE c.CHECK_CLAUSE IS NULL[\s\S]+?   LIMIT 1;");
		Assert.AreEqual(2, guards.Count);
		var expression = guards[postflight ? 1 : 0].Value.Replace("WHERE ", "").Replace("   LIMIT 1;", "");
		if (omitEscapeGuard) expression = expression.Replace("OR LOCATE('``',c.CHECK_CLAUSE)>0", "");
		var capture = Captures.Value[captureName];
		var phase = postflight ? "Target" : "Source";
		var model = postflight ? capture.TargetSignature : capture.SourceSignature;
		var native = postflight ? capture.TargetNativeSignature : capture.SourceNativeSignature;
		var invalidTokens = clause is not null && Regex.IsMatch(Regex.Replace(clause, "`[A-Za-z_][A-Za-z0-9_]*`", ""), "[`'\"]");
		// Fold the emitted literal search independently of the signature/regex
		// adapter. If the production guard disappears these collision tests fail.
		expression = expression.Replace("LOCATE('``',c.CHECK_CLAUSE)>0",
			clause?.IndexOf("``", StringComparison.Ordinal) >= 0 ? "TRUE" : "FALSE");
		expression = Regex.Replace(expression, @"REGEXP_LIKE\(REGEXP_REPLACE\(c.CHECK_CLAUSE,[^\n]+\)", invalidTokens ? "TRUE" : "FALSE");
		// SQL NULL comparison is UNKNOWN. Each such term has an explicit IS NULL
		// refusal in the same OR predicate; fold UNKNOWN to FALSE for this adapter.
		if (clause is null) expression = Regex.Replace(expression,
			@"BINARY LOWER\(REGEXP_REPLACE\(REPLACE\(c.CHECK_CLAUSE[^\n]+\r?\n\s+NOT IN \([^\n]+\)", "FALSE");
		if (flag is null) expression = expression.Replace("BINARY t.ENFORCED NOT IN ('YES','NO')", "FALSE");
		if (flag is null || original is null) expression = expression.Replace("BINARY t.ENFORCED<>BINARY expected.OriginalEnforced", "FALSE");
		expression = expression.Replace("BINARY LOWER(REGEXP_REPLACE(REPLACE(c.CHECK_CLAUSE,'`',''),'[[:space:]]',''))", Literal(clause is null ? null : Signature(clause)));
		expression = expression.Replace("c.CHECK_CLAUSE IS NULL", clause is null ? "TRUE" : "FALSE")
			.Replace("t.ENFORCED IS NULL", flag is null ? "TRUE" : "FALSE")
			.Replace("expected.OriginalEnforced IS NULL", original is null ? "TRUE" : "FALSE")
			.Replace($"BINARY expected.{phase}Signature", Literal(model))
			.Replace($"BINARY expected.{phase}NativeSignature", Literal(native))
			.Replace("BINARY t.ENFORCED", Literal(flag)).Replace("BINARY expected.OriginalEnforced", Literal(original));
		// DataTable compares strings without binary SQL semantics by default.
		using var evaluator = new DataTable { CaseSensitive = true, Locale = CultureInfo.InvariantCulture };
		return Convert.ToBoolean(evaluator.Compute(expression, ""), CultureInfo.InvariantCulture);
	}

	private static string Literal(string? value) => value is null ? "NULL" : "'" + value.Replace("'", "''") + "'";
}
