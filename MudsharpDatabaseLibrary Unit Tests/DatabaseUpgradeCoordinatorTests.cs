using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DatabaseUpgradeCoordinatorTests
{
	[TestMethod]
	public void LoadSettings_CreatesDefaultSettingsFile()
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
	public void PrepareForStartup_WithoutPendingMigrations_DoesNotCreateBackupOrState()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService();
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, backupService);

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
	public void RollbackPreparedUpgrade_RestoresBackupAndArchivesState()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService(["20260401010101_First"]);
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, backupService);
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
	public void CompletePreparedUpgrade_RemovesStateFile()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService(["20260401010101_First"]);
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, backupService);
		var preparation = coordinator.PrepareForStartup(CreateRequest(harness.DirectoryPath));

		coordinator.CompletePreparedUpgrade(preparation);

		Assert.IsFalse(File.Exists(preparation.StateFilePath));
	}

	[TestMethod]
	public void CreateBlankDatabaseSnapshot_ExportsMigratedScratchDatabaseWithPlaceholderName()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService(["20260401010101_First", "20260402020202_Second"]);
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, backupService);
		var snapshotPath = Path.Combine(harness.DirectoryPath, "BlankDatabaseSnapshot.sql");

		coordinator.CreateBlankDatabaseSnapshot(
			CreateRequest(harness.DirectoryPath),
			snapshotPath,
			"__FUTUREMUD_DATABASE__");

		Assert.AreEqual(1, backupService.RecreateEmptyDatabaseCalls);
		Assert.AreEqual(1, backupService.CreateBackupCalls);
		Assert.AreEqual(2, migrationService.AppliedMigrations.Count);
		var snapshotContents = File.ReadAllText(snapshotPath);
		StringAssert.Contains(snapshotContents, "__FUTUREMUD_DATABASE__");
		Assert.IsFalse(snapshotContents.Contains("futuremud_tests", StringComparison.Ordinal));
	}

	[TestMethod]
	public void ImportBlankDatabaseSnapshot_RestoresPlaceholderAdjustedDump()
	{
		using var harness = new TemporaryDirectoryHarness();
		var migrationService = new FakeMigrationService();
		var backupService = new FakeBackupService(harness.DirectoryPath);
		var coordinator = new DatabaseUpgradeCoordinator(migrationService, backupService);
		var snapshotPath = Path.Combine(harness.DirectoryPath, "BlankDatabaseSnapshot.sql");
		File.WriteAllText(snapshotPath, "CREATE DATABASE `__FUTUREMUD_DATABASE__`;");

		coordinator.ImportBlankDatabaseSnapshot(
			CreateRequest(harness.DirectoryPath).ConnectionString,
			snapshotPath,
			"__FUTUREMUD_DATABASE__");

		Assert.AreEqual(1, backupService.RestoreBackupCalls);
		Assert.IsNotNull(backupService.LastRestoreContents);
		var importedContents = backupService.LastRestoreContents!;
		StringAssert.Contains(importedContents, "futuremud_tests");
		Assert.IsFalse(importedContents.Contains("__FUTUREMUD_DATABASE__", StringComparison.Ordinal));
	}

	private static DatabaseUpgradeRequest CreateRequest(string workingDirectory)
	{
		return new DatabaseUpgradeRequest
		{
			ConnectionString =
				"server=localhost;port=3306;database=futuremud_tests;uid=futuremud;password=tests;Default Command Timeout=300000;",
			WorkingDirectory = workingDirectory,
			ExecutableType = "UnitTests"
		};
	}

	private sealed class FakeMigrationService : IDatabaseMigrationService
	{
		private readonly IReadOnlyList<string> _pendingMigrations;
		public List<string> AppliedMigrations { get; } = [];

		public FakeMigrationService(IReadOnlyList<string>? pendingMigrations = null)
		{
			_pendingMigrations = pendingMigrations ?? Array.Empty<string>();
		}

		public IReadOnlyList<string> GetPendingMigrations(string connectionString)
		{
			return _pendingMigrations;
		}

		public void ApplyMigrations(string connectionString, IReadOnlyList<string> migrations,
			Action<DatabaseMigrationProgress>? progressAction = null)
		{
			var index = 1;
			foreach (var migration in migrations)
			{
				AppliedMigrations.Add(migration);
				progressAction?.Invoke(new DatabaseMigrationProgress
				{
					MigrationName = migration,
					CurrentMigrationNumber = index++,
					TotalMigrations = migrations.Count
				});
			}
		}

		public string? GetLatestMigrationId(string connectionString)
		{
			return _pendingMigrations.LastOrDefault();
		}
	}

	private sealed class FakeBackupService : IDatabaseBackupService
	{
		private readonly string _workingDirectory;

		public FakeBackupService(string workingDirectory)
		{
			_workingDirectory = workingDirectory;
		}

		public int CreateBackupCalls { get; private set; }
		public int RestoreBackupCalls { get; private set; }
		public int RecreateEmptyDatabaseCalls { get; private set; }
		public string? LastRestoreContents { get; private set; }

		public string CreateBackup(string connectionString, string backupDirectory)
		{
			CreateBackupCalls++;
			Directory.CreateDirectory(backupDirectory);
			var path = Path.Combine(backupDirectory, $"futuremud-tests-{CreateBackupCalls:00}.sql");
			File.WriteAllText(path, "CREATE DATABASE `futuremud_tests`;");
			return path;
		}

		public void RestoreBackup(string connectionString, string backupFilePath)
		{
			RestoreBackupCalls++;
			Assert.IsTrue(File.Exists(backupFilePath));
			LastRestoreContents = File.ReadAllText(backupFilePath);
		}

		public bool DatabaseLooksBlank(string connectionString)
		{
			return false;
		}

		public void RecreateEmptyDatabase(string connectionString)
		{
			RecreateEmptyDatabaseCalls++;
			Directory.CreateDirectory(_workingDirectory);
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
