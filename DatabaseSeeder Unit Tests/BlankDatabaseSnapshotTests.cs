using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BlankDatabaseSnapshotTests
{
	private const string LatestMigrationId = "20260501132243_RaceAgeColumnsNoDatabaseDefaults";

    [TestMethod]
    public void CommittedBlankSnapshotManifest_TracksLatestMigration()
    {
        string assetDirectory = GetDatabaseSeederProjectDirectory();
        BlankDatabaseSnapshotManifest manifest = BlankDatabaseSnapshotManifest.Load(assetDirectory);

        Assert.IsNotNull(manifest);
        Assert.IsTrue(File.Exists(BlankDatabaseSnapshotManifest.GetSnapshotPath(assetDirectory)));

        string latestMigrationId = GetLatestMigrationIdFromSource();
        Assert.AreEqual(latestMigrationId, manifest.LatestMigrationId);
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
}
