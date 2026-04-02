#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Database;

public sealed class EfDatabaseMigrationService : IDatabaseMigrationService
{
	public IReadOnlyList<string> GetPendingMigrations(string connectionString)
	{
		using var context = CreateContext(connectionString);
		return context.Database.GetPendingMigrations().ToList();
	}

	public void ApplyMigrations(string connectionString, IReadOnlyList<string> migrations,
		Action<DatabaseMigrationProgress>? progressAction = null)
	{
		if (migrations.Count == 0)
		{
			return;
		}

		using var context = CreateContext(connectionString);
		var migrator = context.GetService<IMigrator>();
		var index = 1;
		foreach (var migration in migrations)
		{
			progressAction?.Invoke(new DatabaseMigrationProgress
			{
				MigrationName = migration,
				CurrentMigrationNumber = index,
				TotalMigrations = migrations.Count
			});
			migrator.Migrate(migration);
			index++;
		}
	}

	public string? GetLatestMigrationId(string connectionString)
	{
		using var context = CreateMetadataOnlyContext(connectionString);
		return context.Database.GetMigrations().LastOrDefault();
	}

	public string GenerateBlankDatabaseSnapshotScript(string connectionString, string databaseNamePlaceholder)
	{
		using var context = CreateMetadataOnlyContext(connectionString);
		var migrator = context.GetService<IMigrator>();
		var migrationScript = migrator.GenerateScript(fromMigration: "0", toMigration: null);
		var escapedName = databaseNamePlaceholder.Replace("`", "``", StringComparison.Ordinal);
		return string.Join(Environment.NewLine,
		[
			$"CREATE DATABASE IF NOT EXISTS `{escapedName}`;",
			$"USE `{escapedName}`;",
			migrationScript
		]);
	}

	private static FuturemudDatabaseContext CreateContext(string connectionString)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static FuturemudDatabaseContext CreateMetadataOnlyContext(string connectionString)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)))
			.Options;
		return new FuturemudDatabaseContext(options);
	}
}
