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
		Assert.AreEqual(0, columns.Single(x => x.Name == nameof(Cell.EnvironmentalMagicBindingMode)).DefaultValue);
		Assert.IsTrue(columns.Where(x => x.Name == nameof(Cell.EnvironmentalMagicProfileId)).All(x => x.IsNullable));
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
		var cell = model.FindEntityType(typeof(Cell))!;
		var terrain = model.FindEntityType(typeof(Terrain))!;

		Assert.AreEqual(0, cell.FindProperty(nameof(Cell.EnvironmentalMagicBindingMode))!.GetDefaultValue());
		Assert.IsTrue(cell.FindProperty(nameof(Cell.EnvironmentalMagicProfileId))!.IsNullable);
		Assert.IsTrue(terrain.FindProperty(nameof(Terrain.EnvironmentalMagicProfileId))!.IsNullable);
		Assert.AreEqual(0, new Cell().EnvironmentalMagicBindingMode);
		Assert.IsNull(new Cell().EnvironmentalMagicProfileId);
		Assert.IsNull(new Terrain().EnvironmentalMagicProfileId);
		Assert.IsNull(new Cell().EnvironmentalState);
	}

	[TestMethod]
	public void EnvironmentalBindings_KeepMissingProfileIdentityForDiagnostics()
	{
		using var context = CreateContext();
		var model = context.GetService<IDesignTimeModel>().Model;
		foreach (var ownerType in new[] { typeof(Cell), typeof(Terrain) })
		{
			var owner = model.FindEntityType(ownerType)!;
			Assert.IsFalse(owner.GetForeignKeys()
				.Any(x => x.Properties.Any(p => p.Name == nameof(Cell.EnvironmentalMagicProfileId))));
			Assert.IsTrue(owner.GetIndexes()
				.Any(x => x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(Cell.EnvironmentalMagicProfileId) })));
		}
	}

	[TestMethod]
	public void EnvironmentalState_UsesOnePhysicalCellKeyAndOptimisticConcurrency()
	{
		using var context = CreateContext();
		var state = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(CellEnvironmentalState))!;
		CollectionAssert.AreEqual(new[] { nameof(CellEnvironmentalState.CellId) },
			state.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(ValueGenerated.Never, state.FindProperty(nameof(CellEnvironmentalState.CellId))!.ValueGenerated);
		Assert.IsTrue(state.FindProperty(nameof(CellEnvironmentalState.Revision))!.IsConcurrencyToken);
		var owner = state.GetForeignKeys().Single();
		Assert.AreEqual(typeof(Cell), owner.PrincipalEntityType.ClrType);
		Assert.AreEqual(DeleteBehavior.Cascade, owner.DeleteBehavior);
		Assert.IsTrue(owner.IsUnique);
		Assert.IsTrue(owner.IsRequired);
		Assert.AreEqual(nameof(Cell.EnvironmentalState), owner.PrincipalToDependent!.Name);
	}

	[TestMethod]
	public void EnvironmentalState_DefaultsPreserveNoDamageAndNoDestructiveHistory()
	{
		using var context = CreateContext();
		var state = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(CellEnvironmentalState))!;
		Assert.AreEqual(1, state.FindProperty(nameof(CellEnvironmentalState.SchemaVersion))!.GetDefaultValue());
		Assert.AreEqual(0L, state.FindProperty(nameof(CellEnvironmentalState.Revision))!.GetDefaultValue());
		Assert.AreEqual(0.0, state.FindProperty(nameof(CellEnvironmentalState.ScarDamage))!.GetDefaultValue());
		Assert.AreEqual(0.0, state.FindProperty(nameof(CellEnvironmentalState.RecentPressure))!.GetDefaultValue());
		Assert.AreEqual(3600.0, state.FindProperty(nameof(CellEnvironmentalState.PressureHalfLifeSeconds))!.GetDefaultValue());
		Assert.AreEqual(0.0, state.FindProperty(nameof(CellEnvironmentalState.PressureDecayAnchor))!.GetDefaultValue());
		Assert.IsTrue(state.FindProperty(nameof(CellEnvironmentalState.PressureProfileId))!.IsNullable);
		foreach (var property in new[] { nameof(CellEnvironmentalState.LastDefileUtc), nameof(CellEnvironmentalState.PressureReferenceUtc) })
		{
			Assert.IsTrue(state.FindProperty(property)!.IsNullable);
			Assert.AreEqual("datetime(6)", state.FindProperty(property)!.GetColumnType());
		}

		var record = new CellEnvironmentalState();
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
		CollectionAssert.AreEqual(new[] { nameof(EnvironmentalMagicOperation.CellId), nameof(EnvironmentalMagicOperation.AtUtc) },
			operation.GetIndexes().Single().Properties.Select(x => x.Name).ToArray());
	}

	[TestMethod]
	public void OperationReceipt_DuplicateDeliveryCannotTrackTwoRowsForOneIdentity()
	{
		using var context = CreateContext();
		var operationId = Guid.NewGuid();
		context.EnvironmentalMagicOperations.Add(new EnvironmentalMagicOperation { Id = operationId, CellId = 1 });
		Assert.ThrowsException<InvalidOperationException>(() => context.EnvironmentalMagicOperations.Add(
			new EnvironmentalMagicOperation { Id = operationId, CellId = 2 }));
	}
}
