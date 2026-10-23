#nullable enable
using MudSharp.Database;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DatabaseSeeder;

public enum DatabaseBootstrapMode
{
    SnapshotImport,
    FreshMigration,
    GuardedMigration
}

public sealed class BlankDatabaseSnapshotManifest
{
    public const string SnapshotFileName = "BlankDatabaseSnapshot.sql";
    public const string ManifestFileName = "BlankDatabaseSnapshot.manifest.json";
	internal const string SnapshotResourceName = "DatabaseSeeder.BlankDatabaseSnapshot.sql";
	internal const string ManifestResourceName = "DatabaseSeeder.BlankDatabaseSnapshot.manifest.json";
	private static readonly Lazy<BlankDatabaseSnapshotManifest?> EmbeddedManifest = new(() =>
	{
		using var stream = typeof(BlankDatabaseSnapshotManifest).Assembly.GetManifestResourceStream(ManifestResourceName);
		return stream is null ? null : JsonSerializer.Deserialize<BlankDatabaseSnapshotManifest>(stream, JsonOptions);
	});

	public static BlankDatabaseSnapshotManifest? LoadEmbedded() => EmbeddedManifest.Value;
	public static Stream OpenEmbeddedSnapshot() => typeof(BlankDatabaseSnapshotManifest).Assembly.GetManifestResourceStream(SnapshotResourceName)
		?? throw new FileNotFoundException("The executable contains no blank database snapshot.", SnapshotResourceName);

    public const string DatabaseNamePlaceholder = "__FUTUREMUD_DATABASE__";
    private const string SnapshotRefreshConnectionString =
        "server=localhost;port=3306;database=futuremud_snapshot;uid=futuremud;password=snapshot;Default Command Timeout=300000;";

    public required string ProductVersion { get; init; }
    public required string LatestMigrationId { get; init; }
    public required DateTime GeneratedUtc { get; init; }

    public static string GetSnapshotPath(string assetDirectory)
    {
        return Path.Combine(assetDirectory, SnapshotFileName);
    }

    public static string GetManifestPath(string assetDirectory)
    {
        return Path.Combine(assetDirectory, ManifestFileName);
    }

    public static BlankDatabaseSnapshotManifest? Load(string assetDirectory)
    {
        string manifestPath = GetManifestPath(assetDirectory);
        if (!File.Exists(manifestPath))
        {
            return null;
        }

        return JsonSerializer.Deserialize<BlankDatabaseSnapshotManifest>(
            File.ReadAllText(manifestPath),
            JsonOptions);
    }

    public static BlankDatabaseSnapshotManifest Refresh(string assetDirectory, IDatabaseUpgradeCoordinator coordinator,
        Version productVersion, string? snapshotRefreshConnectionString = null)
    {
        snapshotRefreshConnectionString = string.IsNullOrWhiteSpace(snapshotRefreshConnectionString)
            ? SnapshotRefreshConnectionString
            : snapshotRefreshConnectionString;
        string snapshotPath = GetSnapshotPath(assetDirectory);
        string? latestMigrationId = coordinator.GetLatestMigrationId(snapshotRefreshConnectionString);
        if (string.IsNullOrWhiteSpace(latestMigrationId))
        {
            throw new InvalidOperationException("Could not determine the latest EF migration for the blank database snapshot.");
        }

        coordinator.CreateBlankDatabaseSnapshot(
            new DatabaseUpgradeRequest
            {
                ConnectionString = snapshotRefreshConnectionString,
                WorkingDirectory = assetDirectory,
                ExecutableType = "DatabaseSeederSnapshotRefresh"
            },
            snapshotPath,
            DatabaseNamePlaceholder);

        BlankDatabaseSnapshotManifest manifest = new()
        {
            ProductVersion = productVersion.ToString(),
            LatestMigrationId = latestMigrationId,
            GeneratedUtc = DateTime.UtcNow
        };
        File.WriteAllText(
            GetManifestPath(assetDirectory),
            JsonSerializer.Serialize(manifest, JsonOptions));
        return manifest;
    }

    public static string FindProjectAssetDirectory(string startDirectory)
    {
        DirectoryInfo? directory = new(Path.GetFullPath(startDirectory));
        while (directory is not null)
        {
            if (directory.GetFiles("DatabaseSeeder.csproj").Any())
            {
                return Path.Combine(directory.FullName, "Assets", "Database");
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not locate the DatabaseSeeder project directory from {startDirectory}.");
    }

    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        WriteIndented = true
    };
}

public sealed class BlankDatabaseSnapshotAssessment
{
	public bool UsesEmbeddedSnapshot { get; init; }
    public required string SnapshotPath { get; init; }
    public required string ManifestPath { get; init; }
    public BlankDatabaseSnapshotManifest? Manifest { get; init; }
    public required bool CanUseSnapshot { get; init; }
    public required string Reason { get; init; }
}

public static class BlankDatabaseSnapshotManager
{
	public static BlankDatabaseSnapshotAssessment AssessEmbedded(string? latestMigrationId)
	{
		var manifest = BlankDatabaseSnapshotManifest.LoadEmbedded();
		var hasSnapshot = typeof(BlankDatabaseSnapshotManifest).Assembly.GetManifestResourceNames()
			.Contains(BlankDatabaseSnapshotManifest.SnapshotResourceName, StringComparer.Ordinal);
		var reason = !hasSnapshot ? "The executable contains no blank database snapshot."
			: manifest is null ? "The executable contains no blank database snapshot metadata."
			: string.IsNullOrWhiteSpace(latestMigrationId) ? "Could not determine the latest migration id for snapshot validation."
			: manifest.LatestMigrationId != latestMigrationId
				? $"Blank database snapshot is stale. Manifest has {manifest.LatestMigrationId} but latest migration is {latestMigrationId}."
				: null;
		return new()
		{
			SnapshotPath = BlankDatabaseSnapshotManifest.SnapshotResourceName,
			ManifestPath = BlankDatabaseSnapshotManifest.ManifestResourceName,
			UsesEmbeddedSnapshot = true, Manifest = manifest, CanUseSnapshot = reason is null,
			Reason = reason ?? $"Blank database snapshot is current at migration {latestMigrationId}."
		};
	}

    public static BlankDatabaseSnapshotAssessment Assess(string assetDirectory, string? latestMigrationId)
    {
        string snapshotPath = BlankDatabaseSnapshotManifest.GetSnapshotPath(assetDirectory);
        string manifestPath = BlankDatabaseSnapshotManifest.GetManifestPath(assetDirectory);
        if (!File.Exists(snapshotPath))
        {
            return new BlankDatabaseSnapshotAssessment
            {
                SnapshotPath = snapshotPath,
                ManifestPath = manifestPath,
                CanUseSnapshot = false,
                Reason = $"Blank database snapshot file was not found at {snapshotPath}."
            };
        }

        BlankDatabaseSnapshotManifest? manifest = BlankDatabaseSnapshotManifest.Load(assetDirectory);
        if (manifest is null)
        {
            return new BlankDatabaseSnapshotAssessment
            {
                SnapshotPath = snapshotPath,
                ManifestPath = manifestPath,
                CanUseSnapshot = false,
                Reason = $"Blank database snapshot manifest was not found at {manifestPath}."
            };
        }

        if (string.IsNullOrWhiteSpace(latestMigrationId))
        {
            return new BlankDatabaseSnapshotAssessment
            {
                SnapshotPath = snapshotPath,
                ManifestPath = manifestPath,
                Manifest = manifest,
                CanUseSnapshot = false,
                Reason = "Could not determine the latest migration id for snapshot validation."
            };
        }

        if (!string.Equals(manifest.LatestMigrationId, latestMigrationId, StringComparison.Ordinal))
        {
            return new BlankDatabaseSnapshotAssessment
            {
                SnapshotPath = snapshotPath,
                ManifestPath = manifestPath,
                Manifest = manifest,
                CanUseSnapshot = false,
                Reason =
                    $"Blank database snapshot is stale. Manifest has {manifest.LatestMigrationId} but latest migration is {latestMigrationId}."
            };
        }

        return new BlankDatabaseSnapshotAssessment
        {
            SnapshotPath = snapshotPath,
            ManifestPath = manifestPath,
            Manifest = manifest,
            CanUseSnapshot = true,
            Reason = $"Blank database snapshot is current at migration {latestMigrationId}."
        };
    }

    public static DatabaseBootstrapMode SelectBootstrapMode(bool databaseLooksBlank,
        BlankDatabaseSnapshotAssessment assessment)
    {
        if (!databaseLooksBlank)
        {
            return DatabaseBootstrapMode.GuardedMigration;
        }

        return assessment.CanUseSnapshot
            ? DatabaseBootstrapMode.SnapshotImport
            : DatabaseBootstrapMode.FreshMigration;
    }
}
