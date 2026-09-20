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
public class MagicGatheringOperationModelTests
{
	private static FuturemudDatabaseContext CreateContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void GatheringMigration_AddsOnlyTheDurableReceiptTable()
	{
		MagicGatheringOperations migration = new();
		CreateTableOperation table = migration.UpOperations.OfType<CreateTableOperation>()
			.Single(x => x.Name == "MagicGatheringOperations");
		CollectionAssert.AreEquivalent(new[]
		{
			nameof(MagicGatheringOperation.Id), nameof(MagicGatheringOperation.OwnerId), nameof(MagicGatheringOperation.ActorId),
			nameof(MagicGatheringOperation.BodyId), nameof(MagicGatheringOperation.MagicCapabilityId),
			nameof(MagicGatheringOperation.MethodKey), nameof(MagicGatheringOperation.MethodVersion),
			nameof(MagicGatheringOperation.CellId), nameof(MagicGatheringOperation.SourceProfileId),
			nameof(MagicGatheringOperation.SourceProfileRevision), nameof(MagicGatheringOperation.SourceResourceId),
			nameof(MagicGatheringOperation.DestinationResourceId), nameof(MagicGatheringOperation.Kind),
			nameof(MagicGatheringOperation.RequestedAmount), nameof(MagicGatheringOperation.SourceDebit),
			nameof(MagicGatheringOperation.StaminaCost), nameof(MagicGatheringOperation.DamageCost),
			nameof(MagicGatheringOperation.PainCost), nameof(MagicGatheringOperation.StunCost),
			nameof(MagicGatheringOperation.SourceDebited), nameof(MagicGatheringOperation.BodilyCostApplied),
			nameof(MagicGatheringOperation.DestinationCredited), nameof(MagicGatheringOperation.AccountingPersisted),
			nameof(MagicGatheringOperation.NotificationCompleted), nameof(MagicGatheringOperation.Status),
			nameof(MagicGatheringOperation.CreatedUtc), nameof(MagicGatheringOperation.UpdatedUtc),
			nameof(MagicGatheringOperation.Diagnostic)
		}, table.Columns.Select(x => x.Name).ToArray());
		Assert.AreEqual(0, migration.UpOperations.OfType<InsertDataOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<UpdateDataOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<DeleteDataOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<SqlOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<AlterColumnOperation>().Count());
		Assert.AreEqual(0, migration.UpOperations.OfType<DropTableOperation>().Count());
	}

	[TestMethod]
	public void GatheringReceipt_HasStandaloneIdentityAndTargetedReviewIndexes()
	{
		using FuturemudDatabaseContext context = CreateContext();
		IEntityType receipt = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(MagicGatheringOperation))!;
		CollectionAssert.AreEqual(new[] { nameof(MagicGatheringOperation.Id) },
			receipt.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(ValueGenerated.Never, receipt.FindProperty(nameof(MagicGatheringOperation.Id))!.ValueGenerated);
		Assert.AreEqual(typeof(Guid), receipt.FindProperty(nameof(MagicGatheringOperation.MethodKey))!.ClrType);
		Assert.AreEqual("datetime(6)", receipt.FindProperty(nameof(MagicGatheringOperation.CreatedUtc))!.GetColumnType());
		Assert.AreEqual("datetime(6)", receipt.FindProperty(nameof(MagicGatheringOperation.UpdatedUtc))!.GetColumnType());
		Assert.AreEqual(0, receipt.GetForeignKeys().Count(), "Receipts retain operational diagnostic identity even if referenced world objects later disappear.");
		CollectionAssert.AreEquivalent(new[]
		{
			"IX_MagicGatheringOperations_OwnerId_Status",
			"IX_MagicGatheringOperations_Cell_Source_Status",
			"IX_MagicGatheringOperations_Capability_Method"
		}, receipt.GetIndexes().Select(x => x.GetDatabaseName()).ToArray());
	}

	[TestMethod]
	public void GatheringReceipt_DuplicateOperationIdentityCannotBeTrackedTwice()
	{
		using FuturemudDatabaseContext context = CreateContext();
		Guid id = Guid.NewGuid();
		context.MagicGatheringOperations.Add(new MagicGatheringOperation { Id = id });
		Assert.ThrowsException<InvalidOperationException>(() => context.MagicGatheringOperations.Add(
			new MagicGatheringOperation { Id = id }));
	}

	[TestMethod]
	public void LandGatheringMigration_AddsNullableLegacyColumnsAndIndexedParticipants()
	{
		LandGatheringSourceAccounting migration = new();
		CreateTableOperation participants = migration.UpOperations.OfType<CreateTableOperation>()
			.Single(x => x.Name == "MagicGatheringParticipants");
		CollectionAssert.AreEquivalent(new[] { "OperationId", "CellId", "SourceKey" },
			participants.Columns.Select(x => x.Name).ToArray());
		CollectionAssert.AreEqual(new[] { "OperationId", "SourceKey" },
			participants.PrimaryKey!.Columns.ToArray());
		CollectionAssert.AreEquivalent(new[] { "EcologicalApplied", "EcologicalChildId", "LandDetailJson" },
			migration.UpOperations.OfType<AddColumnOperation>().Select(x => x.Name).ToArray());
		Assert.IsTrue(migration.UpOperations.OfType<AddColumnOperation>()
			.Single(x => x.Name == "LandDetailJson").IsNullable,
			"Existing Self/Gentle rows must upgrade without an invented Land payload.");
		CreateIndexOperation index = migration.UpOperations.OfType<CreateIndexOperation>()
			.Single(x => x.Name == "IX_MagicGatheringParticipants_Cell_Source");
		CollectionAssert.AreEqual(new[] { "CellId", "SourceKey" }, index.Columns);
	}

	[TestMethod]
	public void LandGatheringParticipants_HaveBoundedCellAndSourceLookupIdentity()
	{
		using FuturemudDatabaseContext context = CreateContext();
		IEntityType participant = context.GetService<IDesignTimeModel>().Model
			.FindEntityType(typeof(MagicGatheringParticipant))!;
		CollectionAssert.AreEqual(new[] { "OperationId", "SourceKey" },
			participant.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.AreEqual(150, participant.FindProperty("SourceKey")!.GetMaxLength());
		CollectionAssert.AreEqual(new[] { "CellId", "SourceKey" },
			participant.GetIndexes().Single().Properties.Select(x => x.Name).ToArray());
	}
}
