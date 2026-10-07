using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomSpatialContractionTests
{
	[TestMethod]
	public void Contraction_AssertsFinalMappingBeforeDestructiveDdlAndKeepsInitialProvenance()
	{
		var operations = new CellSpatialContraction().UpOperations.ToList();
		var firstDrop = operations.FindIndex(x => x is DropTableOperation or DropColumnOperation or DropForeignKeyOperation);
		Assert.IsTrue(firstDrop > 0);
		var precedingSql = operations.Take(firstDrop).OfType<SqlOperation>().Select(x => x.Sql).ToList();
		Assert.IsTrue(precedingSql[0].Contains("@FutureMUD_CellSpatialReconcile"));
		Assert.IsTrue(precedingSql[0].Contains("COUNT(*)>1"));
		Assert.IsTrue(precedingSql[0].Contains("information_schema.VIEWS"));
		Assert.IsTrue(precedingSql[0].Contains("information_schema.ROUTINES"));
		Assert.IsTrue(precedingSql[0].Contains("information_schema.TRIGGERS"));
		Assert.IsTrue(precedingSql[0].Contains("information_schema.EVENTS"));
		Assert.IsTrue(precedingSql.Any(x => x.Contains("(TRUE)")));
		var copy = precedingSql.Single(x => x.Contains("START TRANSACTION"));
		Assert.IsTrue(copy.Contains("r.Id=c.RoomId"));
		Assert.IsTrue(copy.Contains("CellRoomContractionLedger"));
		Assert.IsFalse(copy.Contains("INSERT INTO CellRoomMigrationLedger"));
		Assert.IsFalse(copy.Contains("UPDATE CellRoomMigrationLedger"));
		Assert.IsFalse(copy.Contains("INSERT IGNORE"));
		CollectionAssert.AreEquivalent(new[] { "Rooms", "Areas_Rooms" }, operations.OfType<DropTableOperation>().Select(x => x.Name).ToArray());
		Assert.AreEqual("RoomId", operations.OfType<DropColumnOperation>().Single().Name);
		Assert.ThrowsException<NotSupportedException>(() => _ = new CellSpatialContraction().DownOperations);
	}

	[TestMethod]
	public void CurrentModel_RequiresDirectOwnershipAndRetainsIndependentHistoricalLedgers()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=127.0.0.1;database=unused;uid=unused;password=unused", ServerVersion.Parse("8.0.36-mysql")).Options;
		using var context = new FuturemudDatabaseContext(options);
		var model = context.GetService<IDesignTimeModel>().Model;
		Assert.IsNull(model.FindEntityType("MudSharp.Models.Cell"));
		Assert.IsNull(model.FindEntityType("MudSharp.Models.AreasCells"));
		var room = model.FindEntityType(typeof(Room))!;
		Assert.IsNull(room.FindProperty("RoomId"));
		foreach (var name in new[] { "ZoneId", "X", "Y", "Z" }) Assert.IsFalse(room.FindProperty(name)!.IsNullable);
		Assert.AreEqual(DeleteBehavior.Restrict, room.GetForeignKeys().Single(x => x.Properties.Count == 1 && x.Properties[0].Name == "ZoneId").DeleteBehavior);
		Assert.IsFalse(room.GetIndexes().Any(x => x.IsUnique && x.Properties.Any(p => p.Name is "X" or "Y" or "Z")));
		foreach (var type in new[] { typeof(RoomSpatialMigrationLedger), typeof(RoomSpatialAreaMigrationLedger), typeof(RoomSpatialContractionLedger), typeof(RoomSpatialAreaContractionLedger) })
			Assert.IsFalse(model.FindEntityType(type)!.GetForeignKeys().Any());
	}
}
