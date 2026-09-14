#nullable enable

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EfDatabaseMigrationServiceTests
{
	[TestMethod]
	public void MigrateToLatest_DoesNotTargetAnIndividualMigration()
	{
		FakeMigrator migrator = new();

		EfDatabaseMigrationService.MigrateToLatest(migrator);

		Assert.AreEqual(1, migrator.MigrateCalls);
		Assert.IsNull(migrator.TargetMigration);
	}

	private sealed class FakeMigrator : IMigrator
	{
		public int MigrateCalls { get; private set; }
		public string? TargetMigration { get; private set; }

		public void Migrate(string? targetMigration = null)
		{
			MigrateCalls++;
			TargetMigration = targetMigration;
		}

		public Task MigrateAsync(string? targetMigration = null, CancellationToken cancellationToken = default)
		{
			Migrate(targetMigration);
			return Task.CompletedTask;
		}

		public string GenerateScript(string? fromMigration = null, string? toMigration = null,
			MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
		{
			return string.Empty;
		}

		public bool HasPendingModelChanges()
		{
			return false;
		}
	}
}
