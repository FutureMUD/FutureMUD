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
public class CellUniqueNameModelTests
{
	[TestMethod]
	public void Migration_IsOnlyNullableColumnAndNonuniqueIndexWithNoBackfill()
	{
		var migration = new CellUniqueNames();
		Assert.AreEqual(2, migration.UpOperations.Count);
		var column = migration.UpOperations.OfType<AddColumnOperation>().Single();
		Assert.AreEqual("Cells", column.Table);
		Assert.AreEqual("UniqueName", column.Name);
		Assert.AreEqual("varchar(255)", column.ColumnType);
		Assert.IsTrue(column.IsNullable);
		Assert.IsNull(column.DefaultValue);
		Assert.IsNull(column.DefaultValueSql);
		Assert.IsFalse(migration.UpOperations.OfType<CreateIndexOperation>().Single().IsUnique);
		Assert.AreEqual(2, migration.DownOperations.Count);
		Assert.AreEqual("UniqueName", migration.DownOperations.OfType<DropColumnOperation>().Single().Name);
	}

	[TestMethod]
	public void Model_PreservesRoomForeignKeyAndPrototypeIdentifierStorageContract()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=127.0.0.1;database=unused;uid=unused;password=unused", ServerVersion.Parse("8.0.36-mysql")).Options;
		using var context = new FuturemudDatabaseContext(options);
		var model = context.GetService<IDesignTimeModel>().Model;
		var cell = model.FindEntityType(typeof(Cell))!;
		var key = cell.FindProperty(nameof(Cell.UniqueName))!;
		Assert.IsTrue(key.IsNullable);
		Assert.AreEqual("varchar(255)", key.GetColumnType());
		Assert.AreEqual("utf8mb4_general_ci", key.GetCollation());
		Assert.IsFalse(cell.GetIndexes().Single(x => x.Properties.Contains(key)).IsUnique);
		var room = cell.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(Room));
		Assert.IsTrue(room.IsRequired);
		Assert.IsFalse(room.IsUnique);
		Assert.AreEqual(DeleteBehavior.Cascade, room.DeleteBehavior);
		Assert.IsNull(new Cell().UniqueName);
	}
}
