#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using MySql.Data.MySqlClient;

namespace MudSharp.Database;

public sealed class DatabaseUpgradeCoordinator : IDatabaseUpgradeCoordinator
{
	private readonly IDatabaseMigrationService _migrationService;
	private readonly IDatabaseBackupService _backupService;

	public DatabaseUpgradeCoordinator()
		: this(new EfDatabaseMigrationService(), new MySqlDatabaseBackupService())
	{
	}

	public DatabaseUpgradeCoordinator(IDatabaseMigrationService migrationService, IDatabaseBackupService backupService)
	{
		_migrationService = migrationService;
		_backupService = backupService;
	}

	public DatabaseUpgradePreparation PrepareForStartup(DatabaseUpgradeRequest request)
	{
		ValidateRequest(request);
		Directory.CreateDirectory(request.WorkingDirectory);

		var settings = DatabaseBackupSettings.Load(request.WorkingDirectory);
		var stateFilePath = DatabaseUpgradeState.GetStateFilePath(request.WorkingDirectory);
		var databaseName = new MySqlConnectionStringBuilder(request.ConnectionString).Database;
		var restoredPreviousFailedUpgrade = false;

		if (File.Exists(stateFilePath))
		{
			var state = LoadState(stateFilePath);
			if (settings.AutoRollbackOnFailedUpgrade &&
			    state?.DatabaseName == databaseName &&
			    !string.IsNullOrWhiteSpace(state.BackupFilePath) &&
			    File.Exists(state.BackupFilePath))
			{
				_backupService.RestoreBackup(request.ConnectionString, state.BackupFilePath);
				state.RollbackSucceeded = true;
				state.RollbackCompletedUtc = DateTime.UtcNow;
				ArchiveStateFile(stateFilePath, "recovered", state);
				restoredPreviousFailedUpgrade = true;
			}
		}

		var pendingMigrations = _migrationService.GetPendingMigrations(request.ConnectionString);
		var preparation = new DatabaseUpgradePreparation
		{
			Request = request,
			Settings = settings,
			PendingMigrations = pendingMigrations,
			DatabaseName = databaseName,
			StateFilePath = stateFilePath,
			RestoredPreviousFailedUpgrade = restoredPreviousFailedUpgrade
		};

		if (!preparation.HasPendingMigrations)
		{
			return preparation;
		}

		if (settings.AutoBackupBeforeMigrations)
		{
			preparation.BackupFilePath = _backupService.CreateBackup(request.ConnectionString,
				settings.ResolveBackupDirectory(request.WorkingDirectory));
			PruneBackups(settings.ResolveBackupDirectory(request.WorkingDirectory), settings.RetainedBackupCount);
		}

		var upgradeState = new DatabaseUpgradeState
		{
			DatabaseName = databaseName,
			ExecutableType = request.ExecutableType,
			BackupFilePath = preparation.BackupFilePath,
			UpgradeStartedUtc = DateTime.UtcNow,
			PendingMigrations = pendingMigrations.ToList(),
			TargetMigration = pendingMigrations.LastOrDefault(),
			MigrationAttempted = false
		};
		SaveState(stateFilePath, upgradeState);
		return preparation;
	}

	public void ApplyPreparedMigrations(DatabaseUpgradePreparation preparation,
		Action<DatabaseMigrationProgress>? progressAction = null)
	{
		if (!preparation.HasPendingMigrations)
		{
			return;
		}

		var state = LoadState(preparation.StateFilePath) ?? new DatabaseUpgradeState
		{
			DatabaseName = preparation.DatabaseName,
			ExecutableType = preparation.Request.ExecutableType,
			BackupFilePath = preparation.BackupFilePath,
			UpgradeStartedUtc = DateTime.UtcNow,
			PendingMigrations = preparation.PendingMigrations.ToList(),
			TargetMigration = preparation.TargetMigration
		};
		state.MigrationAttempted = true;
		SaveState(preparation.StateFilePath, state);
		_migrationService.ApplyMigrations(preparation.Request.ConnectionString, preparation.PendingMigrations, progressAction);
	}

	public void RollbackPreparedUpgrade(DatabaseUpgradePreparation preparation, Exception? exception = null)
	{
		if (!preparation.HasPendingMigrations || string.IsNullOrWhiteSpace(preparation.BackupFilePath))
		{
			return;
		}

		var state = LoadState(preparation.StateFilePath) ?? new DatabaseUpgradeState
		{
			DatabaseName = preparation.DatabaseName,
			ExecutableType = preparation.Request.ExecutableType,
			BackupFilePath = preparation.BackupFilePath,
			UpgradeStartedUtc = DateTime.UtcNow,
			PendingMigrations = preparation.PendingMigrations.ToList(),
			TargetMigration = preparation.TargetMigration,
			MigrationAttempted = true
		};
		state.LastError = exception?.ToString();
		state.LastFailureUtc = DateTime.UtcNow;
		SaveState(preparation.StateFilePath, state);

		_backupService.RestoreBackup(preparation.Request.ConnectionString, preparation.BackupFilePath);
		state.RollbackSucceeded = true;
		state.RollbackCompletedUtc = DateTime.UtcNow;
		ArchiveStateFile(preparation.StateFilePath, "rolled-back", state);
	}

	public void CompletePreparedUpgrade(DatabaseUpgradePreparation preparation)
	{
		if (!preparation.HasPendingMigrations)
		{
			return;
		}

		if (File.Exists(preparation.StateFilePath))
		{
			File.Delete(preparation.StateFilePath);
		}
	}

	public bool DatabaseLooksBlank(string connectionString)
	{
		return _backupService.DatabaseLooksBlank(connectionString);
	}

	public string? GetLatestMigrationId(string connectionString)
	{
		return _migrationService.GetLatestMigrationId(connectionString);
	}

	public void ImportBlankDatabaseSnapshot(string connectionString, string scriptPath, string databaseNamePlaceholder)
	{
		var builder = new MySqlConnectionStringBuilder(connectionString);
		var tempDirectory = Path.Combine(Path.GetTempPath(), "FutureMUD-SnapshotImport", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDirectory);
		try
		{
			var tempPath = Path.Combine(tempDirectory, Path.GetFileName(scriptPath));
			var script = File.ReadAllText(scriptPath);
			script = script.Replace(databaseNamePlaceholder, builder.Database, StringComparison.Ordinal);
			File.WriteAllText(tempPath, script);
			_backupService.RestoreBackup(connectionString, tempPath);
		}
		finally
		{
			if (Directory.Exists(tempDirectory))
			{
				Directory.Delete(tempDirectory, recursive: true);
			}
		}
	}

	public void CreateBlankDatabaseSnapshot(DatabaseUpgradeRequest request, string snapshotPath, string databaseNamePlaceholder)
	{
		ValidateRequest(request);
		Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath) ?? request.WorkingDirectory);

		_backupService.RecreateEmptyDatabase(request.ConnectionString);
		var pendingMigrations = _migrationService.GetPendingMigrations(request.ConnectionString);
		_migrationService.ApplyMigrations(request.ConnectionString, pendingMigrations);

		var tempDirectory = Path.Combine(Path.GetTempPath(), "FutureMUD-SnapshotRefresh", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDirectory);
		try
		{
			var exportedPath = _backupService.CreateBackup(request.ConnectionString, tempDirectory);
			var databaseName = new MySqlConnectionStringBuilder(request.ConnectionString).Database;
			var snapshotContents = File.ReadAllText(exportedPath)
				.Replace(databaseName, databaseNamePlaceholder, StringComparison.Ordinal);
			File.WriteAllText(snapshotPath, snapshotContents);
		}
		finally
		{
			if (Directory.Exists(tempDirectory))
			{
				Directory.Delete(tempDirectory, recursive: true);
			}
		}
	}

	public void RecreateEmptyDatabase(string connectionString)
	{
		_backupService.RecreateEmptyDatabase(connectionString);
	}

	public string CreateBackup(DatabaseUpgradeRequest request)
	{
		var settings = DatabaseBackupSettings.Load(request.WorkingDirectory);
		var backupDirectory = settings.ResolveBackupDirectory(request.WorkingDirectory);
		var path = _backupService.CreateBackup(request.ConnectionString, backupDirectory);
		PruneBackups(backupDirectory, settings.RetainedBackupCount);
		return path;
	}

	private static void ValidateRequest(DatabaseUpgradeRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.ConnectionString))
		{
			throw new ArgumentException("Connection string must be supplied.", nameof(request));
		}

		if (string.IsNullOrWhiteSpace(request.WorkingDirectory))
		{
			throw new ArgumentException("Working directory must be supplied.", nameof(request));
		}

		if (string.IsNullOrWhiteSpace(request.ExecutableType))
		{
			throw new ArgumentException("Executable type must be supplied.", nameof(request));
		}
	}

	private static DatabaseUpgradeState? LoadState(string path)
	{
		return JsonSerializer.Deserialize<DatabaseUpgradeState>(File.ReadAllText(path),
			DatabaseBackupSettings.JsonOptions);
	}

	private static void SaveState(string path, DatabaseUpgradeState state)
	{
		File.WriteAllText(path, JsonSerializer.Serialize(state, DatabaseBackupSettings.JsonOptions));
	}

	private static void ArchiveStateFile(string stateFilePath, string reason, DatabaseUpgradeState state)
	{
		var archivePath = Path.Combine(
			Path.GetDirectoryName(stateFilePath) ?? ".",
			$"{Path.GetFileNameWithoutExtension(stateFilePath)}-{reason}-{DateTime.UtcNow:yyyyMMddHHmmss}.json");
		File.WriteAllText(archivePath, JsonSerializer.Serialize(state, DatabaseBackupSettings.JsonOptions));
		File.Delete(stateFilePath);
	}

	private static void PruneBackups(string backupDirectory, int retainedBackupCount)
	{
		if (retainedBackupCount < 1 || !Directory.Exists(backupDirectory))
		{
			return;
		}

		var files = new DirectoryInfo(backupDirectory)
			.GetFiles("*.sql")
			.OrderByDescending(x => x.CreationTimeUtc)
			.Skip(retainedBackupCount)
			.ToList();
		foreach (var file in files)
		{
			file.Delete();
		}
	}
}
