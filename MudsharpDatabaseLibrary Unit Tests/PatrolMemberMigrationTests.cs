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

	[TestMethod]
	public void ActorReferenceModel_DoesNotHardForeignKeyCharacterInstances()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(
				"server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		AssertNoCharacterInstanceForeignKey<VehicleOccupancy>(context, nameof(VehicleOccupancy.CharacterInstanceId));
		AssertNoCharacterInstanceForeignKey<VehicleHitchLink>(context, nameof(VehicleHitchLink.SourceCharacterInstanceId));
		AssertNoCharacterInstanceForeignKey<VehicleHitchLink>(context, nameof(VehicleHitchLink.TargetCharacterInstanceId));
		AssertNoCharacterInstanceForeignKey<ArenaSignup>(context, nameof(ArenaSignup.ActiveCharacterInstanceId));
	}

	[TestMethod]
	public void CharacterInstanceActorReferences_DoesNotAddHardCharacterInstanceForeignKeys()
	{
		var operations = GetUpOperations(new CharacterInstanceActorReferences());

		Assert.IsFalse(
			operations
				.OfType<AddForeignKeyOperation>()
				.Any(x => x.PrincipalTable == "CharacterInstances"),
			"Actor-reference columns should remain indexed nullable ids; stale instance refs are handled by diagnostics.");
	}

	private static MigrationOperation[] GetUpOperations(Migration migration)
	{
		var migrationBuilder = new MigrationBuilder("MySql");
		var up = migration.GetType().GetMethod("Up", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(up);

		up.Invoke(migration, new object[] { migrationBuilder });
		return migrationBuilder.Operations.ToArray();
	}

	private static void AssertNoCharacterInstanceForeignKey<TEntity>(
		FuturemudDatabaseContext context,
		string propertyName)
	{
		var entityType = context.Model.FindEntityType(typeof(TEntity));
		Assert.IsNotNull(entityType);

		var property = entityType.FindProperty(propertyName);
		Assert.IsNotNull(property, $"{typeof(TEntity).Name}.{propertyName} should remain mapped as an id column.");

		Assert.IsFalse(
			entityType
				.GetForeignKeys()
				.Any(x =>
					x.PrincipalEntityType.ClrType == typeof(CharacterInstance) &&
					x.Properties.Any(y => y.Name == propertyName)),
			$"{typeof(TEntity).Name}.{propertyName} should not be a hard database foreign key.");
	}
}
