#nullable enable

using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Migrations;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EconomyAnalyticsModelTests
{
	[TestMethod]
	public void Model_HasBoundedQueryIndexesAndSnapshotCascade()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=localhost;port=3306;database=dbo;uid=futuremud;password=unused",
				ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);
		var model = context.GetService<IDesignTimeModel>().Model;
		var activity = model.FindEntityType(typeof(EconomicActivityRecord));
		var snapshot = model.FindEntityType(typeof(EconomySnapshot));
		var entry = model.FindEntityType(typeof(EconomySnapshotEntry));
		Assert.IsNotNull(activity);
		Assert.IsNotNull(snapshot);
		Assert.IsNotNull(entry);
		Assert.IsTrue(activity.GetIndexes().Any(x =>
			x.Properties.Select(y => y.Name).SequenceEqual([
				nameof(EconomicActivityRecord.EconomicZoneId), nameof(EconomicActivityRecord.RealDateTime)])));
		Assert.IsTrue(activity.GetIndexes().Any(x =>
			x.Properties.Select(y => y.Name).SequenceEqual([
				nameof(EconomicActivityRecord.MudCalendarId), nameof(EconomicActivityRecord.MudYear),
				nameof(EconomicActivityRecord.MudMonth), nameof(EconomicActivityRecord.MudDay)])));
		Assert.AreEqual(DeleteBehavior.Cascade,
			entry.GetForeignKeys().Single(x => x.PrincipalEntityType.ClrType == typeof(EconomySnapshot)).DeleteBehavior);
	}

	[TestMethod]
	public void Migration_CreatesAnalyticsTablesAndDefaultConfiguration()
	{
		var builder = new MigrationBuilder("MySql");
		var migration = new AddEconomyAnalytics();
		var up = migration.GetType().GetMethod("Up", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(up);
		up.Invoke(migration, [builder]);
		var tables = builder.Operations.OfType<CreateTableOperation>().Select(x => x.Name).ToList();
		CollectionAssert.IsSubsetOf(new[]
		{
			"EconomicActivityRecords", "EconomySnapshots", "EconomySnapshotEntries"
		}, tables);
		var settings = builder.Operations.OfType<InsertDataOperation>()
			.Single(x => x.Table == "StaticConfigurations");
		Assert.AreEqual(3, settings.Values.GetLength(0));
	}
}
