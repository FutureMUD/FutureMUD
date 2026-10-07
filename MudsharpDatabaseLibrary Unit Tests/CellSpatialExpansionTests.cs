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
public class RoomSpatialExpansionTests
{
	[TestMethod]
	public void Expansion_GuardsBeforeSchemaChangesAndRetainsEveryLegacyStructure()
	{
		var operations = new CellSpatialExpansion().UpOperations;
		Assert.IsTrue(operations.Take(3).All(x => x is SqlOperation));
		Assert.IsTrue(((SqlOperation)operations[0]).Sql.Contains("COUNT(*)>1"));
		Assert.IsTrue(((SqlOperation)operations[0]).Sql.Contains("unknown foreign key"));
		Assert.IsFalse(operations.Any(x => x is DropTableOperation or DropColumnOperation or DropForeignKeyOperation or RenameTableOperation));
		Assert.IsTrue(operations.OfType<AddColumnOperation>().All(x => x.IsNullable && x.DefaultValue is null));
		Assert.AreEqual(4, operations.OfType<AddColumnOperation>().Count());
		var copy = ((SqlOperation)operations.Last()).Sql;
		Assert.IsTrue(copy.Contains("m.CellId=c.Id"));
		Assert.IsFalse(copy.Contains("INSERT IGNORE"));
		Assert.IsFalse(copy.Contains("DELETE FROM"));
		Assert.IsTrue(copy.Contains("START TRANSACTION"));
	}

	[TestMethod]
	public void Model_StagesNullableMetadataWithRestrictedZoneDeletionAndNonuniqueCoordinates()
	{
		// Historical migration model remains nullable even after the current runtime contracts.
		var model = new CellSpatialExpansion().TargetModel;
		var room = model.FindEntityType("MudSharp.Models.Cell")!;
		foreach (var name in new[] { "ZoneId", "X", "Y", "Z" }) Assert.IsTrue(room.FindProperty(name)!.IsNullable);
		Assert.AreEqual(DeleteBehavior.Restrict, room.GetForeignKeys().Single(x => x.Properties.Single().Name == "ZoneId").DeleteBehavior);
		Assert.IsTrue(room.GetForeignKeys().Any(x => x.PrincipalEntityType.Name == "MudSharp.Models.Room"));
		Assert.IsFalse(room.GetIndexes().Any(x => x.IsUnique && x.Properties.Any(p => p.Name is "X" or "Y" or "Z")));
		var areas = model.FindEntityType("MudSharp.Models.AreasCells")!;
		CollectionAssert.AreEqual(new[] { "AreaId", "CellId" }, areas.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.IsFalse(model.FindEntityType("MudSharp.Models.CellRoomMigrationLedger")!.GetForeignKeys().Any());
		Assert.IsFalse(model.FindEntityType("MudSharp.Models.CellRoomAreaMigrationLedger")!.GetForeignKeys().Any());
	}
}
