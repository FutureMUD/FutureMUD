#nullable enable

using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PatrolMemberMigrationTests
{
	[TestMethod]
	public void PatrolMemberModel_KeepsLegacyPrimaryKey()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(
				"server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		var entityType = context.Model.FindEntityType(typeof(PatrolMember));
		Assert.IsNotNull(entityType);

		var key = entityType.FindPrimaryKey();
		Assert.IsNotNull(key);

		CollectionAssert.AreEqual(
			new[] { nameof(PatrolMember.PatrolId), nameof(PatrolMember.CharacterId) },
			key.Properties.Select(x => x.Name).ToArray());

		var instanceProperty = entityType.FindProperty(nameof(PatrolMember.CharacterInstanceId));
		Assert.IsNotNull(instanceProperty);
		Assert.AreEqual("bigint(20)", instanceProperty.GetColumnType());
	}

	[TestMethod]
	public void CharacterInstanceNpcPatrolStableInstances_DoesNotRebuildPatrolMembersPrimaryKey()
	{
		var operations = GetUpOperations(new CharacterInstanceNpcPatrolStableInstances());

		Assert.IsFalse(
			operations
				.OfType<DropPrimaryKeyOperation>()
				.Any(x => x.Table == "PatrolMembers"),
			"The migration must not drop PatrolMembers.PRIMARY; mature MySQL databases can have dependent foreign keys.");

		Assert.IsFalse(
			operations
				.OfType<AddPrimaryKeyOperation>()
				.Any(x => x.Table == "PatrolMembers" && x.Columns.Contains("CharacterInstanceId")),
			"CharacterInstanceId is actor-reference metadata and must not become part of the PatrolMembers primary key.");
	}

	private static MigrationOperation[] GetUpOperations(Migration migration)
	{
		var migrationBuilder = new MigrationBuilder("MySql");
		var up = migration.GetType().GetMethod("Up", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(up);

		up.Invoke(migration, new object[] { migrationBuilder });
		return migrationBuilder.Operations.ToArray();
	}
}
