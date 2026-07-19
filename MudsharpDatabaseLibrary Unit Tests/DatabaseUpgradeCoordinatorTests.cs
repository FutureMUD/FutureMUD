#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DatabaseUpgradeCoordinatorTests
{
	[TestMethod]
	public void LoadSettings_MissingFile_CreatesDefaults()
	{
		using var harness = new TemporaryDirectoryHarness();

		var settings = DatabaseBackupSettings.Load(harness.DirectoryPath);

		Assert.AreEqual(5, settings.RetainedBackupCount);
		Assert.AreEqual("./Backups", settings.BackupDirectory);
		Assert.IsTrue(settings.AutoBackupBeforeMigrations);
		Assert.IsTrue(settings.AutoRollbackOnFailedUpgrade);
		Assert.IsTrue(File.Exists(DatabaseBackupSettings.GetSettingsFilePath(harness.DirectoryPath)));
	}

	[TestMethod]
	public void LoadSettings_InvalidValues_RepairsSupportedDefaults()
	{
		using var harness = new TemporaryDirectoryHarness();
		var path = DatabaseBackupSettings.GetSettingsFilePath(harness.DirectoryPath);
		File.WriteAllText(path, """{"RetainedBackupCount":0,"BackupDirectory":" "}""");

		var settings = DatabaseBackupSettings.Load(harness.DirectoryPath);

		Assert.AreEqual(DatabaseBackupSettings.Default.RetainedBackupCount, settings.RetainedBackupCount);
		Assert.AreEqual(DatabaseBackupSettings.Default.BackupDirectory, settings.BackupDirectory);
	}

	[TestMethod]
	public void LoadSettings_MalformedJson_ReplacesItWithUsableDefaults()
	{
		using var harness = new TemporaryDirectoryHarness();
		var path = DatabaseBackupSettings.GetSettingsFilePath(harness.DirectoryPath);
		File.WriteAllText(path, "{ definitely not json");

		var settings = DatabaseBackupSettings.Load(harness.DirectoryPath);

		Assert.AreEqual(DatabaseBackupSettings.Default.RetainedBackupCount, settings.RetainedBackupCount);
		using var document = JsonDocument.Parse(File.ReadAllText(path));
		Assert.AreEqual(DatabaseBackupSettings.Default.BackupDirectory,
			document.RootElement.GetProperty(nameof(DatabaseBackupSettings.BackupDirectory)).GetString());
	}

	[DataTestMethod]
	[DataRow("", "working", "Engine")]
	[DataRow("server=localhost;database=dbo;uid=test;password=test", "", "Engine")]
	[DataRow("server=localhost;database=dbo;uid=test;password=test", "working", "")]
	public void PrepareForStartup_MissingRequiredRequestValue_ThrowsArgumentException(
		string connectionString,
		string workingDirectory,
		string executableType)
	{
		using var harness = new TemporaryDirectoryHarness();
		var request = new DatabaseUpgradeRequest
		{
			ConnectionString = connectionString,
			WorkingDirectory = workingDirectory == "working" ? harness.DirectoryPath : workingDirectory,
			ExecutableType = executableType
		};
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), new FakeBackupService(harness.DirectoryPath));

		Assert.ThrowsException<ArgumentException>(() => coordinator.PrepareForStartup(request));
	}

	[TestMethod]
	public void PrepareForStartup_WithoutPendingMigrations_DoesNotCreateBackupOrState()
	{
		using var harness = new TemporaryDirectoryHarness();
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), backupService);

		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		Assert.IsFalse(preparation.HasPendingMigrations);
		Assert.AreEqual(0, backupService.CreateBackupCalls);
		Assert.IsFalse(File.Exists(preparation.StateFilePath));
	}

	[TestMethod]
	public void PrepareForStartup_WithPendingMigrations_CreatesBackupAndWritesState()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService(["20260401010101_First", "20260402020202_Second"]);
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, backupService);

		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		Assert.IsTrue(preparation.HasPendingMigrations);
		Assert.AreEqual(1, backupService.CreateBackupCalls);
		Assert.IsTrue(File.Exists(preparation.StateFilePath));
		Assert.IsFalse(string.IsNullOrWhiteSpace(preparation.BackupFilePath));
		var stateContents = File.ReadAllText(preparation.StateFilePath);
		StringAssert.Contains(stateContents, "\"MigrationAttempted\": false");
		StringAssert.Contains(stateContents, "20260402020202_Second");
	}

	[TestMethod]
	public void PrepareForStartup_BackupsDisabled_WritesRecoverableStateWithoutCreatingBackup()
	{
		using var harness = new TemporaryDirectoryHarness();
		new DatabaseBackupSettings { AutoBackupBeforeMigrations = false }
			.Save(DatabaseBackupSettings.GetSettingsFilePath(harness.DirectoryPath));
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(["20260401010101_First"]), backupService);

		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		Assert.IsTrue(preparation.HasPendingMigrations);
		Assert.IsNull(preparation.BackupFilePath);
		Assert.AreEqual(0, backupService.CreateBackupCalls);
		Assert.IsTrue(File.Exists(preparation.StateFilePath));
	}

	[TestMethod]
	public void PrepareForStartup_MatchingFailedUpgrade_RestoresBackupAndArchivesState()
	{
		using var harness = new TemporaryDirectoryHarness();
		var firstCoordinator = new DatabaseUpgradeCoordinator(
			new FakeMigrationService(["20260401010101_First"]),
			new FakeBackupService(harness.DirectoryPath));
		var firstPreparation = firstCoordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));
		var recoveryBackupService = new FakeBackupService(harness.DirectoryPath);
		var recoveryCoordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), recoveryBackupService);

		var recovered = recoveryCoordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		Assert.IsTrue(recovered.RestoredPreviousFailedUpgrade);
		Assert.AreEqual(1, recoveryBackupService.RestoreBackupCalls);
		Assert.IsFalse(File.Exists(firstPreparation.StateFilePath));
		Assert.AreEqual(1, Directory.GetFiles(harness.DirectoryPath, "db-upgrade-state-recovered-*.json").Length);
	}

	[TestMethod]
	public void PrepareForStartup_MismatchedDatabase_DoesNotRestorePreviousState()
	{
		using var harness = new TemporaryDirectoryHarness();
		var firstCoordinator = new DatabaseUpgradeCoordinator(
			new FakeMigrationService(["20260401010101_First"]),
			new FakeBackupService(harness.DirectoryPath));
		var firstPreparation = firstCoordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));
		var recoveryBackupService = new FakeBackupService(harness.DirectoryPath);
		var recoveryCoordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), recoveryBackupService);

		var result = recoveryCoordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath, "different_database"));

		Assert.IsFalse(result.RestoredPreviousFailedUpgrade);
		Assert.AreEqual(0, recoveryBackupService.RestoreBackupCalls);
		Assert.IsTrue(File.Exists(firstPreparation.StateFilePath));
	}

	[TestMethod]
	public void PrepareForStartup_MissingBackup_DoesNotClaimRecovery()
	{
		using var harness = new TemporaryDirectoryHarness();
		var firstCoordinator = new DatabaseUpgradeCoordinator(
			new FakeMigrationService(["20260401010101_First"]),
			new FakeBackupService(harness.DirectoryPath));
		var firstPreparation = firstCoordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));
		File.Delete(firstPreparation.BackupFilePath!);
		var recoveryBackupService = new FakeBackupService(harness.DirectoryPath);
		var recoveryCoordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), recoveryBackupService);

		var result = recoveryCoordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		Assert.IsFalse(result.RestoredPreviousFailedUpgrade);
		Assert.AreEqual(0, recoveryBackupService.RestoreBackupCalls);
		Assert.IsTrue(File.Exists(firstPreparation.StateFilePath));
	}

	[TestMethod]
	public void ApplyPreparedMigrations_MarksAttemptBeforeApplyingAndForwardsProgress()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService(["First", "Second"]);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, new FakeBackupService(harness.DirectoryPath));
		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));
		var progress = new List<DatabaseMigrationProgress>();

		coordinator.ApplyPreparedMigrations(preparation, progress.Add);

		CollectionAssert.AreEqual(new[] { "First", "Second" }, migrationService.AppliedMigrations.ToArray());
		CollectionAssert.AreEqual(new[] { 1, 2 }, progress.Select(x => x.CurrentMigrationNumber).ToArray());
		StringAssert.Contains(File.ReadAllText(preparation.StateFilePath), "\"MigrationAttempted\": true");
	}

	[TestMethod]
	public void ApplyPreparedMigrations_MigrationFailure_LeavesAttemptedStateForRecovery()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService(["First"]) { ThrowOnApply = true };
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, new FakeBackupService(harness.DirectoryPath));
		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		Assert.ThrowsException<InvalidOperationException>(() => coordinator.ApplyPreparedMigrations(preparation));

		Assert.IsTrue(File.Exists(preparation.StateFilePath));
		StringAssert.Contains(File.ReadAllText(preparation.StateFilePath), "\"MigrationAttempted\": true");
	}

	[TestMethod]
	public void RollbackPreparedUpgrade_RestoresBackupAndArchivesState()
	{
		using var harness = new TemporaryDirectoryHarness();
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(["First"]), backupService);
		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		coordinator.RollbackPreparedUpgrade(preparation, new InvalidOperationException("boom"));

		Assert.AreEqual(1, backupService.RestoreBackupCalls);
		Assert.IsFalse(File.Exists(preparation.StateFilePath));
		var archivedState = Directory.GetFiles(harness.DirectoryPath, "db-upgrade-state-rolled-back-*.json").Single();
		var archivedContents = File.ReadAllText(archivedState);
		StringAssert.Contains(archivedContents, "\"RollbackSucceeded\": true");
		StringAssert.Contains(archivedContents, "boom");
	}

	[TestMethod]
	public void RollbackPreparedUpgrade_RestoreFailure_PreservesFailureStateForNextStartup()
	{
		using var harness = new TemporaryDirectoryHarness();
		var backupService = new FakeBackupService(harness.DirectoryPath) { ThrowOnRestore = true };
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(["First"]), backupService);
		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		Assert.ThrowsException<InvalidOperationException>(() =>
			coordinator.RollbackPreparedUpgrade(preparation, new InvalidOperationException("migration failed")));

		Assert.IsTrue(File.Exists(preparation.StateFilePath));
		var state = File.ReadAllText(preparation.StateFilePath);
		StringAssert.Contains(state, "migration failed");
		StringAssert.Contains(state, "\"RollbackSucceeded\": false");
	}

	[TestMethod]
	public void CompletePreparedUpgrade_RemovesStateFile()
	{
		using var harness = new TemporaryDirectoryHarness();
		var coordinator = new DatabaseUpgradeCoordinator(
			new FakeMigrationService(["First"]),
			new FakeBackupService(harness.DirectoryPath));
		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		coordinator.CompletePreparedUpgrade(preparation);

		Assert.IsFalse(File.Exists(preparation.StateFilePath));
	}

	[TestMethod]
	public void CreateBlankDatabaseSnapshot_ExportsMigratedScratchDatabaseWithPlaceholderName()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService(["First", "Second"]);
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, backupService);
		var snapshotPath = Path.Combine(harness.DirectoryPath, "BlankDatabaseSnapshot.sql");

		coordinator.CreateBlankDatabaseSnapshot(CreateRequest(harness.DirectoryPath), snapshotPath, "__FUTUREMUD_DATABASE__");

		Assert.AreEqual(1, backupService.RecreateEmptyDatabaseCalls);
		Assert.AreEqual(1, backupService.CreateBackupCalls);
		Assert.AreEqual(2, migrationService.AppliedMigrations.Count);
		var snapshotContents = File.ReadAllText(snapshotPath);
		StringAssert.Contains(snapshotContents, "__FUTUREMUD_DATABASE__");
		Assert.IsFalse(snapshotContents.Contains("futuremud_tests", StringComparison.Ordinal));
	}

	[TestMethod]
	public void CreateBlankDatabaseSnapshot_BackupFailure_RemovesScratchDirectory()
	{
		using var harness = new TemporaryDirectoryHarness();
		var backupService = new FakeBackupService(harness.DirectoryPath) { ThrowOnCreateBackup = true };
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), backupService);

		Assert.ThrowsException<InvalidOperationException>(() => coordinator.CreateBlankDatabaseSnapshot(
			CreateRequest(harness.DirectoryPath),
			Path.Combine(harness.DirectoryPath, "BlankDatabaseSnapshot.sql"),
			"__FUTUREMUD_DATABASE__"));

		Assert.IsNotNull(backupService.LastBackupDirectory);
		Assert.IsFalse(Directory.Exists(backupService.LastBackupDirectory));
	}

	[TestMethod]
	public void ImportBlankDatabaseSnapshot_RestoresPlaceholderAdjustedDump()
	{
		using var harness = new TemporaryDirectoryHarness();
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), backupService);
		var snapshotPath = Path.Combine(harness.DirectoryPath, "BlankDatabaseSnapshot.sql");
		File.WriteAllText(snapshotPath, "CREATE DATABASE `__FUTUREMUD_DATABASE__`;");

		coordinator.ImportBlankDatabaseSnapshot(CreateRequest(harness.DirectoryPath).ConnectionString,
			snapshotPath, "__FUTUREMUD_DATABASE__");

		Assert.AreEqual(1, backupService.RestoreBackupCalls);
		Assert.IsNotNull(backupService.LastRestoreContents);
		StringAssert.Contains(backupService.LastRestoreContents, "futuremud_tests");
		Assert.IsFalse(backupService.LastRestoreContents.Contains("__FUTUREMUD_DATABASE__", StringComparison.Ordinal));
	}

	[TestMethod]
	public void ImportBlankDatabaseSnapshot_RestoreFailure_RemovesScratchDirectory()
	{
		using var harness = new TemporaryDirectoryHarness();
		var backupService = new FakeBackupService(harness.DirectoryPath) { ThrowOnRestore = true };
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), backupService);
		var snapshotPath = Path.Combine(harness.DirectoryPath, "BlankDatabaseSnapshot.sql");
		File.WriteAllText(snapshotPath, "CREATE DATABASE `__FUTUREMUD_DATABASE__`;");

		Assert.ThrowsException<InvalidOperationException>(() => coordinator.ImportBlankDatabaseSnapshot(
			CreateRequest(harness.DirectoryPath).ConnectionString,
			snapshotPath,
			"__FUTUREMUD_DATABASE__"));

		Assert.IsNotNull(backupService.LastRestorePath);
		Assert.IsFalse(Directory.Exists(Path.GetDirectoryName(backupService.LastRestorePath)));
	}

	[TestMethod]
	public void CreateBackup_ExceedingRetention_PrunesOldestSqlFiles()
	{
		using var harness = new TemporaryDirectoryHarness();
		new DatabaseBackupSettings { RetainedBackupCount = 2 }
			.Save(DatabaseBackupSettings.GetSettingsFilePath(harness.DirectoryPath));
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(new FakeMigrationService(), backupService);
		var request = CreateRequest(harness.DirectoryPath);

		for (var i = 0; i < 4; i++)
		{
			coordinator.CreateBackup(request);
		}

		var backupDirectory = DatabaseBackupSettings.Load(harness.DirectoryPath)
			.ResolveBackupDirectory(harness.DirectoryPath);
		var remaining = Directory.GetFiles(backupDirectory, "*.sql")
			.Select(Path.GetFileName)
			.OrderBy(x => x)
			.ToArray();
		CollectionAssert.AreEqual(new[] { "futuremud-tests-03.sql", "futuremud-tests-04.sql" }, remaining);
	}

	private static DatabaseUpgradeRequest CreateRequest(string workingDirectory, string database = "futuremud_tests")
	{
		return new DatabaseUpgradeRequest
		{
			ConnectionString = $"server=localhost;port=3306;database={database};uid=futuremud;password=tests;Default Command Timeout=300000;",
			WorkingDirectory = workingDirectory,
			ExecutableType = "UnitTests"
		};
	}

	private sealed class FakeMigrationService(IReadOnlyList<string>? pendingMigrations = null) : IDatabaseMigrationService
	{
		private readonly IReadOnlyList<string> _pendingMigrations = pendingMigrations ?? [];
		public List<string> AppliedMigrations { get; } = [];
		public bool ThrowOnApply { get; init; }

		public IReadOnlyList<string> GetPendingMigrations(string connectionString) => _pendingMigrations;

		public void ApplyMigrations(string connectionString, IReadOnlyList<string> migrations,
			Action<DatabaseMigrationProgress>? progressAction = null)
		{
			for (var index = 0; index < migrations.Count; index++)
			{
				AppliedMigrations.Add(migrations[index]);
				progressAction?.Invoke(new DatabaseMigrationProgress
				{
					MigrationName = migrations[index],
					CurrentMigrationNumber = index + 1,
					TotalMigrations = migrations.Count
				});
			}

			if (ThrowOnApply)
			{
				throw new InvalidOperationException("Migration application failed.");
			}
		}

		public string? GetLatestMigrationId(string connectionString) => _pendingMigrations.LastOrDefault();
	}

	private sealed class FakeBackupService(string workingDirectory) : IDatabaseBackupService
	{
		public int CreateBackupCalls { get; private set; }
		public int RestoreBackupCalls { get; private set; }
		public int RecreateEmptyDatabaseCalls { get; private set; }
		public string? LastRestoreContents { get; private set; }
		public string? LastRestorePath { get; private set; }
		public string? LastBackupDirectory { get; private set; }
		public bool ThrowOnRestore { get; init; }
		public bool ThrowOnCreateBackup { get; init; }

		public string CreateBackup(string connectionString, string backupDirectory)
		{
			CreateBackupCalls++;
			LastBackupDirectory = backupDirectory;
			if (ThrowOnCreateBackup)
			{
				throw new InvalidOperationException("Backup creation failed.");
			}

			Directory.CreateDirectory(backupDirectory);
			var path = Path.Combine(backupDirectory, $"futuremud-tests-{CreateBackupCalls:00}.sql");
			File.WriteAllText(path, "CREATE DATABASE `futuremud_tests`;");
			File.SetCreationTimeUtc(path, new DateTime(2026, 1, 1, 0, CreateBackupCalls, 0, DateTimeKind.Utc));
			return path;
		}

		public void RestoreBackup(string connectionString, string backupFilePath)
		{
			RestoreBackupCalls++;
			LastRestorePath = backupFilePath;
			if (ThrowOnRestore)
			{
				throw new InvalidOperationException("Backup restore failed.");
			}

			Assert.IsTrue(File.Exists(backupFilePath));
			LastRestoreContents = File.ReadAllText(backupFilePath);
		}

		public bool DatabaseLooksBlank(string connectionString) => false;

		public void RecreateEmptyDatabase(string connectionString)
		{
			RecreateEmptyDatabaseCalls++;
			Directory.CreateDirectory(workingDirectory);
		}

		public void ExecuteSqlScript(string connectionString, string scriptContents)
		{
		}
	}

	private sealed class TemporaryDirectoryHarness : IDisposable
	{
		public TemporaryDirectoryHarness()
		{
			DirectoryPath = Path.Combine(Path.GetTempPath(), "FutureMUD-Codex", Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(DirectoryPath);
		}

		public string DirectoryPath { get; }

		public void Dispose()
		{
			if (Directory.Exists(DirectoryPath))
			{
				Directory.Delete(DirectoryPath, recursive: true);
			}
		}
	}
}
