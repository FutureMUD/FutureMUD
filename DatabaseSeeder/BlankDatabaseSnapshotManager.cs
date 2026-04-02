#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using MudSharp.Database;

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
		var manifestPath = GetManifestPath(assetDirectory);
		if (!File.Exists(manifestPath))
		{
			return null;
		}

		return JsonSerializer.Deserialize<BlankDatabaseSnapshotManifest>(
			File.ReadAllText(manifestPath),
			JsonOptions);
	}

	public static BlankDatabaseSnapshotManifest Refresh(string assetDirectory, IDatabaseUpgradeCoordinator coordinator,
		Version productVersion)
	{
		var snapshotPath = GetSnapshotPath(assetDirectory);
		var latestMigrationId = coordinator.GetLatestMigrationId(SnapshotRefreshConnectionString);
		if (string.IsNullOrWhiteSpace(latestMigrationId))
		{
			throw new InvalidOperationException("Could not determine the latest EF migration for the blank database snapshot.");
		}

		coordinator.CreateBlankDatabaseSnapshot(
			new DatabaseUpgradeRequest
			{
				ConnectionString = SnapshotRefreshConnectionString,
				WorkingDirectory = assetDirectory,
				ExecutableType = "DatabaseSeederSnapshotRefresh"
			},
			snapshotPath,
			DatabaseNamePlaceholder);

		var manifest = new BlankDatabaseSnapshotManifest
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
		var directory = new DirectoryInfo(Path.GetFullPath(startDirectory));
		while (directory is not null)
		{
			if (directory.GetFiles("DatabaseSeeder.csproj").Any())
			{
				return directory.FullName;
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
	public required string SnapshotPath { get; init; }
	public required string ManifestPath { get; init; }
	public BlankDatabaseSnapshotManifest? Manifest { get; init; }
	public required bool CanUseSnapshot { get; init; }
	public required string Reason { get; init; }
}

public static class BlankDatabaseSnapshotManager
{
	public static BlankDatabaseSnapshotAssessment Assess(string assetDirectory, string? latestMigrationId)
	{
		var snapshotPath = BlankDatabaseSnapshotManifest.GetSnapshotPath(assetDirectory);
		var manifestPath = BlankDatabaseSnapshotManifest.GetManifestPath(assetDirectory);
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

		var manifest = BlankDatabaseSnapshotManifest.Load(assetDirectory);
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
