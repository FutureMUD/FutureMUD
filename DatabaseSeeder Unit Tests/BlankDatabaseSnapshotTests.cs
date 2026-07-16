#nullable enable
using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BlankDatabaseSnapshotTests
{
	private const string LatestMigrationId = "20260701122720_ClanHallCellsForEmploymentHosts";

    [TestMethod]
    public void CommittedBlankSnapshotManifest_TracksLatestMigration()
    {
        string assetDirectory = GetDatabaseSeederProjectDirectory();
        BlankDatabaseSnapshotManifest? manifest = BlankDatabaseSnapshotManifest.Load(assetDirectory);

        Assert.IsNotNull(manifest);
        Assert.IsTrue(File.Exists(BlankDatabaseSnapshotManifest.GetSnapshotPath(assetDirectory)));

        string latestMigrationId = GetLatestMigrationIdFromSource();
        Assert.AreEqual(latestMigrationId, manifest!.LatestMigrationId);
    }

    [TestMethod]
    public void Assess_WhenManifestIsCurrent_BlankDatabaseUsesSnapshotImport()
    {
        using TemporaryDirectoryHarness harness = new();
        WriteSnapshotFiles(harness.DirectoryPath, LatestMigrationId);

        BlankDatabaseSnapshotAssessment assessment = BlankDatabaseSnapshotManager.Assess(
            harness.DirectoryPath,
            LatestMigrationId);
        DatabaseBootstrapMode mode = BlankDatabaseSnapshotManager.SelectBootstrapMode(databaseLooksBlank: true, assessment);

        Assert.IsTrue(assessment.CanUseSnapshot);
        Assert.AreEqual(DatabaseBootstrapMode.SnapshotImport, mode);
    }

    [TestMethod]
    public void Assess_WhenManifestIsStale_BlankDatabaseFallsBackToFreshMigrations()
    {
        using TemporaryDirectoryHarness harness = new();
        WriteSnapshotFiles(harness.DirectoryPath, "20260401010101_PreviousMigration");

        BlankDatabaseSnapshotAssessment assessment = BlankDatabaseSnapshotManager.Assess(
            harness.DirectoryPath,
            LatestMigrationId);
        DatabaseBootstrapMode mode = BlankDatabaseSnapshotManager.SelectBootstrapMode(databaseLooksBlank: true, assessment);

        Assert.IsFalse(assessment.CanUseSnapshot);
        StringAssert.Contains(assessment.Reason, "stale");
        Assert.AreEqual(DatabaseBootstrapMode.FreshMigration, mode);
    }

	[TestMethod]
	public void Refresh_WhenConnectionStringIsSupplied_UsesThatDatabase()
	{
		using TemporaryDirectoryHarness harness = new();
		const string connectionString =
			"server=localhost;port=3307;database=demo_dbo;uid=tester;password=secret;";
		FakeDatabaseUpgradeCoordinator coordinator = new();

		BlankDatabaseSnapshotManifest manifest = BlankDatabaseSnapshotManifest.Refresh(
			harness.DirectoryPath,
			coordinator,
			new Version(3, 0, 0),
			connectionString);

		Assert.AreEqual(connectionString, coordinator.LatestMigrationConnectionString);
		Assert.AreEqual(connectionString, coordinator.SnapshotRequest?.ConnectionString);
		Assert.AreEqual("3.0.0", manifest.ProductVersion);
	}

    private static string GetDatabaseSeederProjectDirectory()
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "DatabaseSeeder"));
    }

    private static string GetLatestMigrationIdFromSource()
    {
        string migrationsDirectory = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "MudsharpDatabaseLibrary",
            "Migrations"));
        return Directory
            .GetFiles(migrationsDirectory, "*.cs")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x => x != "FutureMUDContextModelSnapshot")
            .Where(x => !x!.EndsWith(".Designer", StringComparison.Ordinal))
            .OrderBy(x => x, StringComparer.Ordinal)
            .Last()!;
    }

    private static void WriteSnapshotFiles(string directoryPath, string latestMigrationId)
    {
        File.WriteAllText(
            BlankDatabaseSnapshotManifest.GetSnapshotPath(directoryPath),
            "CREATE TABLE `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL);");
        File.WriteAllText(
            BlankDatabaseSnapshotManifest.GetManifestPath(directoryPath),
            $@"{{
  ""ProductVersion"": ""2.3.0.0"",
  ""LatestMigrationId"": ""{latestMigrationId}"",
  ""GeneratedUtc"": ""2026-04-03T00:00:00Z""
}}");
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

	private sealed class FakeDatabaseUpgradeCoordinator : IDatabaseUpgradeCoordinator
	{
		public string? LatestMigrationConnectionString { get; private set; }
		public DatabaseUpgradeRequest? SnapshotRequest { get; private set; }

		public string? GetLatestMigrationId(string connectionString)
		{
			LatestMigrationConnectionString = connectionString;
			return LatestMigrationId;
		}

		public void CreateBlankDatabaseSnapshot(DatabaseUpgradeRequest request, string snapshotPath,
			string databaseNamePlaceholder)
		{
			SnapshotRequest = request;
			File.WriteAllText(snapshotPath, databaseNamePlaceholder);
		}

		public DatabaseUpgradePreparation PrepareForStartup(DatabaseUpgradeRequest request) =>
			throw new NotSupportedException();

		public void ApplyPreparedMigrations(DatabaseUpgradePreparation preparation,
			Action<DatabaseMigrationProgress>? progressAction = null) => throw new NotSupportedException();

		public void RollbackPreparedUpgrade(DatabaseUpgradePreparation preparation, Exception? exception = null) =>
			throw new NotSupportedException();

		public void CompletePreparedUpgrade(DatabaseUpgradePreparation preparation) => throw new NotSupportedException();
		public bool DatabaseLooksBlank(string connectionString) => throw new NotSupportedException();

		public void ImportBlankDatabaseSnapshot(string connectionString, string scriptPath,
			string databaseNamePlaceholder) => throw new NotSupportedException();

		public void RecreateEmptyDatabase(string connectionString) => throw new NotSupportedException();
		public string CreateBackup(DatabaseUpgradeRequest request) => throw new NotSupportedException();
	}
}
