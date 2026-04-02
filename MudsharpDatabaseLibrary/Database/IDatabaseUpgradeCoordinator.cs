#nullable enable
using System;

namespace MudSharp.Database;

public interface IDatabaseUpgradeCoordinator
{
	DatabaseUpgradePreparation PrepareForStartup(DatabaseUpgradeRequest request);
	void ApplyPreparedMigrations(DatabaseUpgradePreparation preparation,
		Action<DatabaseMigrationProgress>? progressAction = null);
	void RollbackPreparedUpgrade(DatabaseUpgradePreparation preparation, Exception? exception = null);
	void CompletePreparedUpgrade(DatabaseUpgradePreparation preparation);
	bool DatabaseLooksBlank(string connectionString);
	string? GetLatestMigrationId(string connectionString);
	string GenerateBlankDatabaseSnapshotScript(string connectionString, string databaseNamePlaceholder);
	void ImportBlankDatabaseSnapshot(string connectionString, string scriptPath, string databaseNamePlaceholder);
	void RecreateEmptyDatabase(string connectionString);
	string CreateBackup(DatabaseUpgradeRequest request);
}
