#nullable enable
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
public class RegionalGatheringModelTests
{
	[TestMethod]
	public void RegionalMigration_CreatesEmptyOwnersAndProfilesWithoutActivatingExistingWorlds()
	{
		var operations = new RegionalLandGathering().UpOperations;
		CollectionAssert.AreEquivalent(new[] { "RegionalGatheringPools", "RegionalGatheringProfiles" }, operations.OfType<CreateTableOperation>().Select(x => x.Name).ToArray());
		Assert.IsTrue(operations.All(x => x is CreateTableOperation or CreateIndexOperation));
		var pool = operations.OfType<CreateTableOperation>().Single(x => x.Name == "RegionalGatheringPools");
		Assert.AreEqual(1, pool.ForeignKeys.Count);
		Assert.AreEqual("RegionalGatheringProfiles", pool.ForeignKeys.Single().PrincipalTable, "Region deletion must retain ecological/payment evidence.");
	}
	[TestMethod]
	public void RegionalModel_UsesPhysicalOwnerCompositeKeyAndOptimisticRevisions()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;database=unused;uid=unused;password=unused", ServerVersion.Parse("8.0.36-mysql")).Options);
		var model = context.GetService<IDesignTimeModel>().Model;
		var pool = model.FindEntityType(typeof(RegionalGatheringPool))!;
		CollectionAssert.AreEqual(new[] { "OwnerKind", "OwnerId" }, pool.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.IsTrue(pool.FindProperty("Revision")!.IsConcurrencyToken);
		Assert.AreEqual(ValueGenerated.Never, pool.FindProperty("OwnerId")!.ValueGenerated);
		Assert.IsTrue(pool.FindProperty("ProfileId")!.IsNullable);
		Assert.AreEqual(DeleteBehavior.Restrict, pool.GetForeignKeys().Single().DeleteBehavior);
		Assert.IsTrue(model.FindEntityType(typeof(RegionalGatheringProfile))!.FindProperty("Revision")!.IsConcurrencyToken);
		Assert.IsTrue(model.FindEntityType(typeof(MagicGatheringParticipant))!.GetIndexes().Any(x => x.Properties.Select(p => p.Name).SequenceEqual(new[] { "SourceKey" })));
	}
}
