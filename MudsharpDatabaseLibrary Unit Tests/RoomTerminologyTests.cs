#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Migrations;

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
		Assert.AreEqual(93,operations.OfType<DropForeignKeyOperation>().Count());
		Assert.AreEqual(93,operations.OfType<AddForeignKeyOperation>().Count());
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
		Assert.IsTrue(preflight.Contains("@FutureMUD_RoomTerminologyMaintenance"));
		Assert.IsTrue(preflight.Contains("complete contraction first"));
		Assert.IsTrue(preflight.Contains("unclassified foreign key"));
		Assert.IsTrue(preflight.Contains("information_schema.VIEWS"));
		Assert.IsTrue(preflight.Contains("information_schema.EVENTS"));
		Assert.IsTrue(preflight.Contains("BeforeCount"));
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
