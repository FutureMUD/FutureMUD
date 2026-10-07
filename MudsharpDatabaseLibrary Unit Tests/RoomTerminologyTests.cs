#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Migrations;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomTerminologyTests
{
	[TestMethod]
	public void Naming_RenamesEveryAffectedTableWithoutDestructiveReplacement()
	{
		var migration = new RoomTerminology();
		var operations = migration.UpOperations;
		Assert.AreEqual(26,operations.OfType<RenameTableOperation>().Count());
		Assert.AreEqual(77,operations.OfType<RenameColumnOperation>().Count());
		Assert.AreEqual(80,operations.OfType<RenameIndexOperation>().Count());
		Assert.AreEqual(92,operations.OfType<DropForeignKeyOperation>().Count());
		Assert.AreEqual(74,operations.OfType<AddForeignKeyOperation>().Count());
		Assert.AreEqual(2,operations.OfType<SqlOperation>().Count(x=>x.Sql.Contains("PREPARE fm_room_naming_optional_statement_20261007043900")));
		Assert.AreEqual(18,operations.OfType<SqlOperation>().Count(x=>x.Sql.Contains("PREPARE fm_room_naming_fk_statement_20261007043900")));
		Assert.AreEqual(9,operations.OfType<AddCheckConstraintOperation>().Count());
		Assert.IsFalse(operations.Any(x=>x is DropTableOperation or CreateTableOperation or DropColumnOperation or AddColumnOperation or DeleteDataOperation or UpdateDataOperation));
		foreach (var alter in operations.OfType<AlterColumnOperation>())
		{
			Assert.IsTrue(alter.IsNullable);
			Assert.AreEqual("RoomId",alter.Name);
			Assert.IsTrue(alter.Table is "RoomSpatialAreaMigrationLedger" or "RoomSpatialAreaContractionLedger");
		}
		Assert.ThrowsException<NotSupportedException>(()=>_ = migration.DownOperations);
		var preflight = operations.OfType<SqlOperation>().First().Sql;
		Assert.IsTrue(preflight.Contains("legacy grouping still exists"));
		Assert.IsTrue(preflight.Contains("complete contraction first"));
		Assert.IsTrue(preflight.Contains("unclassified foreign key"));
		Assert.IsTrue(preflight.Contains("information_schema.VIEWS"));
		Assert.IsTrue(preflight.Contains("information_schema.EVENTS"));
		Assert.IsTrue(preflight.Contains("BeforeCount"));
	}

	[TestMethod]
	public void Naming_LegacyActionVariants_KeepEveryGuardAndCaptureBeforeForeignKeyChanges()
	{
		var operations = new RoomTerminology().UpOperations;
		var preflight = operations.OfType<SqlOperation>().First().Sql;
		var captures = Regex.Matches(preflight,
			@"SELECT '([^']+)','([^']+)',UPDATE_RULE,DELETE_RULE FROM information_schema\.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE\(\) AND LOWER\(TABLE_NAME\)='([^']+)' AND LOWER\(CONSTRAINT_NAME\)='([^']+)'");
		Assert.AreEqual(18,captures.Count);
		Assert.AreEqual(18,captures.Cast<Match>().Select(x=>x.Groups[1].Value).Distinct().Count());
		var variants = captures.Cast<Match>().Select(x=>x.Groups[4].Value).ToHashSet();
		CollectionAssert.AreEquivalent(new[]
		{
			"fk_activeprojects_cells", "fk_cells_foragableyields_cells",
			"fk_cells_magicresources_cells", "fk_cells_magicresources_magicresources",
			"fk_cells_rangedcovers_cells", "fk_cells_rangedcovers_rangedcovers", "fk_cells_tags_tags",
			"fk_characterlog_cells", "fk_clans_administrationcells_cells", "fk_clans_administrationcells_clans",
			"fk_clans_treasurycells_cells", "fk_clans_treasurycells_clans", "fk_crimes_location",
			"fk_hooks_perceivables_cells", "fk_shops_cells_stockroom", "fk_shops_cells_workshop",
			"fk_shops_storeroomcells_cells", "fk_shops_storeroomcells_shops"
		},variants.ToArray());
		var guards = Regex.Matches(preflight,
			@"(?m)^  IF (.+) THEN\r?\n   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs ([^']+)';");
		Assert.AreEqual(93,guards.Count);
		foreach (Match guard in guards)
		{
			var condition = guard.Groups[1].Value;
			Assert.IsTrue(condition.Contains("information_schema.KEY_COLUMN_USAGE"));
			Assert.IsTrue(condition.Contains("REFERENCED_TABLE_SCHEMA=DATABASE()"));
			Assert.IsTrue(condition.Contains("LOWER(REFERENCED_COLUMN_NAME)="));
			Assert.IsTrue(condition.Contains("DELETE_RULE IN ("));
			var updateRules = variants.Contains(guard.Groups[2].Value.ToLowerInvariant())
				? "UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')"
				: "UPDATE_RULE IN ('NO ACTION','RESTRICT')";
			Assert.IsTrue(condition.Contains(updateRules),guard.Groups[2].Value);
			Assert.IsFalse(condition.Contains("UPDATE_RULE IN ('SET NULL"));
		}
		Assert.IsTrue(preflight.IndexOf("CLOSE definitions;",StringComparison.Ordinal)<preflight.IndexOf("CREATE TEMPORARY TABLE `fm_room_naming_fk_actions",StringComparison.Ordinal));
		Assert.IsTrue(preflight.Contains("legacy foreign-key action capture is incomplete"));
		Assert.IsFalse(preflight.Contains("CREATE TEMPORARY TABLE IF NOT EXISTS"));
		Assert.IsFalse(preflight.Contains("DROP TEMPORARY TABLE IF EXISTS"));
		Assert.IsTrue(operations[0] is SqlOperation);
		Assert.IsTrue(operations[1] is SqlOperation call && call.Sql.Contains("(FALSE)"));
		Assert.IsTrue(operations[2] is DropForeignKeyOperation);
	}

	[TestMethod]
	public void Naming_LegacyActions_AreUsedInRenamedModelConstraintsAndComparedInPostflight()
	{
		var migration = new RoomTerminology();
		var preflight = migration.UpOperations.OfType<SqlOperation>().First().Sql;
		var captures = Regex.Matches(preflight,@"SELECT '([^']+)','([^']+)',UPDATE_RULE,DELETE_RULE FROM information_schema\.REFERENTIAL_CONSTRAINTS");
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;database=unused;uid=unused;password=unused",ServerVersion.Parse("8.0.45-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);
		var model = context.GetService<IModelRuntimeInitializer>().Initialize(migration.TargetModel,designTime: true);
		var constraints = model.GetRelationalModel().Tables.SelectMany(x=>x.ForeignKeyConstraints).ToArray();
		foreach (Match capture in captures)
		{
			var name = capture.Groups[1].Value;
			var constraint = constraints.Single(x=>x.Name==name);
			Assert.AreEqual(constraint.Table.Name,capture.Groups[2].Value);
			var columnSql = string.Join(",",constraint.Columns.Select(x=>$"`{x.Name}`"));
			var principalColumnSql = string.Join(",",constraint.PrincipalColumns.Select(x=>$"`{x.Name}`"));
			var statement = $"ALTER TABLE `{constraint.Table.Name}` ADD CONSTRAINT `{name}` FOREIGN KEY ({columnSql}) REFERENCES `{constraint.PrincipalTable.Name}` ({principalColumnSql})";
			var restore = migration.UpOperations.OfType<SqlOperation>().Single(x=>x.Sql.Contains($"WHERE ConstraintName='{name}'"));
			Assert.IsTrue(restore.SuppressTransaction);
			Assert.IsTrue(restore.Sql.Contains($"CONCAT('{statement} ON DELETE ',DeleteRule,' ON UPDATE ',UpdateRule)"));
			Assert.IsTrue(restore.Sql.Contains("AND UpdateRule IN ('NO ACTION','RESTRICT','CASCADE')"));
			Assert.IsTrue(restore.Sql.Contains("DEALLOCATE PREPARE"));
			Assert.IsTrue(restore.Sql.EndsWith("= NULL;",StringComparison.Ordinal));
			Assert.IsFalse(migration.UpOperations.OfType<AddForeignKeyOperation>().Any(x=>x.Name==name));
		}
		Assert.IsTrue(preflight.Contains("current_rule.CONSTRAINT_NAME IS NULL"));
		Assert.IsTrue(preflight.Contains("current_rule.UPDATE_RULE<>original.UpdateRule"));
		Assert.IsTrue(preflight.Contains("current_rule.DELETE_RULE<>original.DeleteRule"));
		Assert.IsTrue(preflight.Contains("original foreign-key actions were not preserved"));
		Assert.IsTrue(preflight.Contains("legacy foreign-key action capture changed after rename"));
		// MySQL cannot reopen one temporary table twice in the same query.
		Assert.IsFalse(preflight.Contains("`fm_room_naming_fk_actions_20261007043900`)<>18 OR EXISTS"));
		Assert.IsTrue(preflight.Contains("DROP TEMPORARY TABLE `fm_room_naming_fk_actions_20261007043900`"));
	}

	[TestMethod]
	public void Naming_ActiveProjectLegacyCascade_DoesNotChangeItsDeleteOrReferenceContract()
	{
		var preflight = new RoomTerminology().UpOperations.OfType<SqlOperation>().First().Sql;
		var guard = Regex.Match(preflight,
			@"(?m)^  IF (.+) THEN\r?\n   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_ActiveProjects_Cells';");
		Assert.IsTrue(guard.Success);
		var condition = guard.Groups[1].Value;
		Assert.IsTrue(condition.Contains("LOWER(COLUMN_NAME)='cellid'"));
		Assert.IsTrue(condition.Contains("LOWER(REFERENCED_TABLE_NAME)='cells'"));
		Assert.IsTrue(condition.Contains("LOWER(REFERENCED_COLUMN_NAME)='id'"));
		Assert.IsTrue(condition.Contains("DELETE_RULE IN ('NO ACTION','RESTRICT')"));
		Assert.IsTrue(condition.Contains("UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')"));
		Assert.IsFalse(condition.Contains("DELETE_RULE IN ('CASCADE')"));
	}

	[TestMethod]
	public void Naming_SourceDiscoveryAndEfDiscovery_AgreeOnTheLatestRealMigration()
	{
		var migrationDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,"..","..","..","..",
			"MudsharpDatabaseLibrary","Migrations"));
		var sourceLatest = Directory.GetFiles(migrationDirectory,"*.cs")
			.Select(Path.GetFileNameWithoutExtension)
			.Where(x=>x!="FutureMUDContextModelSnapshot" && !x!.EndsWith(".Designer",StringComparison.Ordinal))
			.OrderBy(x=>x,StringComparer.Ordinal).Last();
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;database=unused;uid=unused;password=unused",ServerVersion.Parse("8.0.45-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);
		var assemblyLatest = context.GetService<IMigrationsAssembly>().Migrations.Keys
			.OrderBy(x=>x,StringComparer.Ordinal).Last();
		Assert.AreEqual("20261007043900_RoomTerminology",assemblyLatest);
		Assert.AreEqual(assemblyLatest,sourceLatest);
	}

	[TestMethod]
	public void Naming_OrderedSchemaReplay_PreservesUnequalAndCollidingHistoricalIds()
	{
		var tables = new Dictionary<string,Dictionary<string,object?>>
		{
			["Cells"] = new() { ["Id"]=8101L, ["ZoneId"]=2L, ["X"]=4 },
			["Areas_Cells"] = new() { ["AreaId"]=7L,["CellId"]=8101L },
			["CellRoomMigrationLedger"] = new() { ["RoomId"]=9000L,["CellId"]=8101L },
			["CellRoomContractionLedger"] = new() { ["RoomId"]=8101L,["CellId"]=9000L },
			["CellRoomAreaMigrationLedger"] = new() { ["AreaId"]=7L,["RoomId"]=9001L,["CellId"]=null },
			["CellRoomAreaContractionLedger"] = new() { ["AreaId"]=7L,["RoomId"]=9001L,["CellId"]=null }
		};
		foreach (var operation in new RoomTerminology().UpOperations)
		{
			if (operation is RenameTableOperation t && tables.Remove(t.Name,out var row)) tables.Add(t.NewName!,row);
			if (operation is RenameColumnOperation c && tables.TryGetValue(c.Table,out var columns))
			{
				Assert.IsTrue(columns.Remove(c.Name,out var value));
				columns.Add(c.NewName,value);
			}
		}
		Assert.AreEqual(8101L,tables["Rooms"]["Id"]);
		Assert.AreEqual(8101L,tables["Areas_Rooms"]["RoomId"]);
		Assert.AreEqual(9000L,tables["RoomSpatialMigrationLedger"]["LegacyRoomId"]);
		Assert.AreEqual(8101L,tables["RoomSpatialMigrationLedger"]["RoomId"]);
		Assert.AreEqual(8101L,tables["RoomSpatialContractionLedger"]["LegacyRoomId"]);
		Assert.AreEqual(9000L,tables["RoomSpatialContractionLedger"]["RoomId"]);
		Assert.AreEqual(9001L,tables["RoomSpatialAreaContractionLedger"]["LegacyRoomId"]);
		Assert.IsNull(tables["RoomSpatialAreaContractionLedger"]["RoomId"]);
	}

	[TestMethod]
	public void EmptyParentMemberships_KeepOriginalAreaParentPairWithNullChild()
	{
		foreach (var migration in new Migration[] {new CellSpatialExpansion(),new CellSpatialContraction()})
		{
			var operations = migration.UpOperations;
			var ledger = operations.OfType<CreateTableOperation>().Single(x=>x.Name.Contains("Area") && x.Name.Contains("Ledger"));
			Assert.IsTrue(ledger.Columns.Single(x=>x.Name=="CellId").IsNullable);
			Assert.AreEqual(new[] {"AreaId","RoomId"}.Length,ledger.PrimaryKey!.Columns.Length);
			var copy = operations.OfType<SqlOperation>().Single(x=>x.Sql.Contains("START TRANSACTION")).Sql;
			Assert.IsTrue(copy.Contains("LEFT JOIN"));
			Assert.IsTrue(copy.Contains("discarded Area memberships retained in ledgers"));
			Assert.IsFalse(operations.OfType<SqlOperation>().First().Sql.Contains("Area references an empty Room"));
			Assert.IsTrue(operations.OfType<SqlOperation>().First().Sql.Contains("COUNT(*)>1"));
			Assert.IsTrue(migration.TargetModel.FindEntityType("MudSharp.Models."+ledger.Name)!.FindProperty("CellId")!.IsNullable);
		}
	}
}
