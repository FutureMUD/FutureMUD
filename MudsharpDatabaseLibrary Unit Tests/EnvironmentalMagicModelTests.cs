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

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalMagicModelTests
{
	private static FuturemudDatabaseContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void EnvironmentalMigration_PreservesExistingBalancesAndLeavesProfilesUnassigned()
	{
		var migration = new EnvironmentalMagic();
		var columns = migration.UpOperations.OfType<AddColumnOperation>().ToArray();
		Assert.AreEqual(3, columns.Length);
		Assert.AreEqual(0, columns.Single(x => x.Name == nameof(Room.EnvironmentalMagicBindingMode)).DefaultValue);
		Assert.IsTrue(columns.Where(x => x.Name == nameof(Room.EnvironmentalMagicProfileId)).All(x => x.IsNullable));
		CollectionAssert.AreEquivalent(new[] { "CellEnvironmentalStates", "EnvironmentalMagicOperations" },
			migration.UpOperations.OfType<CreateTableOperation>().Select(x => x.Name).ToArray());
		Assert.AreEqual(0, migration.UpOperations.OfType<InsertDataOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<UpdateDataOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<DeleteDataOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<SqlOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<AlterColumnOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<DropTableOperation>().Count());
	}

	[TestMethod]
	public void EnvironmentalBindings_UpgradeToInheritanceWithoutAttachingProfiles()
	{
		using var context = CreateContext();
		var model = context.GetService<IDesignTimeModel>().Model;
		var room = model.FindEntityType(typeof(Room))!;
		var terrain = model.FindEntityType(typeof(Terrain))!;

		Assert.AreEqual(0, room.FindProperty(nameof(Room.EnvironmentalMagicBindingMode))!.GetDefaultValue());
		Assert.IsTrue(room.FindProperty(nameof(Room.EnvironmentalMagicProfileId))!.IsNullable);
		Assert.IsTrue(terrain.FindProperty(nameof(Terrain.EnvironmentalMagicProfileId))!.IsNullable);
		Assert.AreEqual(0, new Room().EnvironmentalMagicBindingMode);
		Assert.IsNull(new Room().EnvironmentalMagicProfileId);
		Assert.IsNull(new Terrain().EnvironmentalMagicProfileId);
		Assert.IsNull(new Room().EnvironmentalState);
	}

	[TestMethod]
	public void EnvironmentalBindings_KeepMissingProfileIdentityForDiagnostics()
	{
		using var context = CreateContext();
		var model = context.GetService<IDesignTimeModel>().Model;
		foreach (var ownerType in new[] { typeof(Room), typeof(Terrain) })
		{
			var owner = model.FindEntityType(ownerType)!;
			Assert.IsFalse(owner.GetForeignKeys()
				.Any(x => x.Properties.Any(p => p.Name == nameof(Room.EnvironmentalMagicProfileId))));
			Assert.IsTrue(owner.GetIndexes()
				.Any(x => x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(Room.EnvironmentalMagicProfileId) })));
		}
	}

	[TestMethod]
	public void EnvironmentalState_UsesOnePhysicalRoomKeyAndOptimisticConcurrency()
	{
		using var context = CreateContext();
		var state = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(RoomEnvironmentalState))!;
		CollectionAssert.AreEqual(new[] { nameof(RoomEnvironmentalState.RoomId) },
			state.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(ValueGenerated.Never, state.FindProperty(nameof(RoomEnvironmentalState.RoomId))!.ValueGenerated);
		Assert.IsTrue(state.FindProperty(nameof(RoomEnvironmentalState.Revision))!.IsConcurrencyToken);
		var owner = state.GetForeignKeys().Single();
		Assert.AreEqual(typeof(Room), owner.PrincipalEntityType.ClrType);
		Assert.AreEqual(DeleteBehavior.Cascade, owner.DeleteBehavior);
		Assert.IsTrue(owner.IsUnique);
		Assert.IsTrue(owner.IsRequired);
		Assert.AreEqual(nameof(Room.EnvironmentalState), owner.PrincipalToDependent!.Name);
	}

	[TestMethod]
	public void EnvironmentalState_DefaultsPreserveNoDamageAndNoDestructiveHistory()
	{
		using var context = CreateContext();
		var state = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(RoomEnvironmentalState))!;
		Assert.AreEqual(1, state.FindProperty(nameof(RoomEnvironmentalState.SchemaVersion))!.GetDefaultValue());
		Assert.AreEqual(0L, state.FindProperty(nameof(RoomEnvironmentalState.Revision))!.GetDefaultValue());
		Assert.AreEqual(0.0, state.FindProperty(nameof(RoomEnvironmentalState.ScarDamage))!.GetDefaultValue());
		Assert.AreEqual(0.0, state.FindProperty(nameof(RoomEnvironmentalState.RecentPressure))!.GetDefaultValue());
		Assert.AreEqual(3600.0, state.FindProperty(nameof(RoomEnvironmentalState.PressureHalfLifeSeconds))!.GetDefaultValue());
		Assert.AreEqual(0.0, state.FindProperty(nameof(RoomEnvironmentalState.PressureDecayAnchor))!.GetDefaultValue());
		Assert.IsTrue(state.FindProperty(nameof(RoomEnvironmentalState.PressureProfileId))!.IsNullable);
		foreach (var property in new[] { nameof(RoomEnvironmentalState.LastDefileUtc), nameof(RoomEnvironmentalState.PressureReferenceUtc) })
		{
			Assert.IsTrue(state.FindProperty(property)!.IsNullable);
			Assert.AreEqual("datetime(6)", state.FindProperty(property)!.GetColumnType());
		}

		var record = new RoomEnvironmentalState();
		Assert.AreEqual(1, record.SchemaVersion);
		Assert.AreEqual(3600.0, record.PressureHalfLifeSeconds);
		Assert.IsNull(record.LastDefileUtc);
		Assert.IsNull(record.PressureReferenceUtc);
	}

	[TestMethod]
	public void OperationReceipt_HasDurableCallerIdentityAndNoCascadingOwners()
	{
		using var context = CreateContext();
		var operation = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(EnvironmentalMagicOperation))!;
		CollectionAssert.AreEqual(new[] { nameof(EnvironmentalMagicOperation.Id) },
			operation.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(typeof(Guid), operation.FindProperty(nameof(EnvironmentalMagicOperation.Id))!.ClrType);
		Assert.AreEqual(ValueGenerated.Never, operation.FindProperty(nameof(EnvironmentalMagicOperation.Id))!.ValueGenerated);
		Assert.AreEqual(0, operation.GetForeignKeys().Count());
		Assert.AreEqual("datetime(6)", operation.FindProperty(nameof(EnvironmentalMagicOperation.AtUtc))!.GetColumnType());
		Assert.IsTrue(operation.FindProperty(nameof(EnvironmentalMagicOperation.ActorId))!.IsNullable);
		CollectionAssert.AreEqual(new[] { nameof(EnvironmentalMagicOperation.RoomId), nameof(EnvironmentalMagicOperation.AtUtc) },
			operation.GetIndexes().Single().Properties.Select(x => x.Name).ToArray());
	}

	[TestMethod]
	public void OperationReceipt_DuplicateDeliveryCannotTrackTwoRowsForOneIdentity()
	{
		using var context = CreateContext();
		var operationId = Guid.NewGuid();
		context.EnvironmentalMagicOperations.Add(new EnvironmentalMagicOperation { Id = operationId, RoomId = 1 });
		Assert.ThrowsException<InvalidOperationException>(() => context.EnvironmentalMagicOperations.Add(
			new EnvironmentalMagicOperation { Id = operationId, RoomId = 2 }));
	}
}
