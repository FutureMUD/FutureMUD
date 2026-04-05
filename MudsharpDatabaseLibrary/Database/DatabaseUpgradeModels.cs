#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MudSharp.Database;

public sealed class DatabaseBackupSettings
{
    public const string SettingsFileName = "DatabaseBackupSettings.json";

    public int RetainedBackupCount { get; set; } = 5;
    public string BackupDirectory { get; set; } = "./Backups";
    public bool AutoBackupBeforeMigrations { get; set; } = true;
    public bool AutoRollbackOnFailedUpgrade { get; set; } = true;

    public static DatabaseBackupSettings Default => new();

    public string ResolveBackupDirectory(string workingDirectory)
    {
        if (string.IsNullOrWhiteSpace(BackupDirectory))
        {
            return Path.GetFullPath(Path.Combine(workingDirectory, "Backups"));
        }

        return Path.IsPathRooted(BackupDirectory)
            ? Path.GetFullPath(BackupDirectory)
            : Path.GetFullPath(Path.Combine(workingDirectory, BackupDirectory));
    }

    public static string GetSettingsFilePath(string workingDirectory)
    {
        return Path.Combine(workingDirectory, SettingsFileName);
    }

    public static DatabaseBackupSettings Load(string workingDirectory)
    {
        string path = GetSettingsFilePath(workingDirectory);
        if (!File.Exists(path))
        {
            DatabaseBackupSettings defaults = Default;
            defaults.Save(path);
            return defaults;
        }

        string json = File.ReadAllText(path);
        DatabaseBackupSettings loaded = JsonSerializer.Deserialize<DatabaseBackupSettings>(json) ?? Default;
        if (loaded.RetainedBackupCount < 1)
        {
            loaded.RetainedBackupCount = Default.RetainedBackupCount;
        }

        if (string.IsNullOrWhiteSpace(loaded.BackupDirectory))
        {
            loaded.BackupDirectory = Default.BackupDirectory;
        }

        return loaded;
    }

    public void Save(string path)
    {
        string json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(path, json);
    }

    internal static JsonSerializerOptions JsonOptions { get; } = new()
    {
        WriteIndented = true
    };
}

public sealed class DatabaseUpgradeState
{
    public const string StateFileName = "db-upgrade-state.json";

    public string DatabaseName { get; set; } = string.Empty;
    public string ExecutableType { get; set; } = string.Empty;
    public string? BackupFilePath { get; set; }
    public DateTime UpgradeStartedUtc { get; set; }
    public List<string> PendingMigrations { get; set; } = [];
    public string? TargetMigration { get; set; }
    public bool MigrationAttempted { get; set; }
    public string? LastError { get; set; }
    public DateTime? LastFailureUtc { get; set; }
    public bool RollbackSucceeded { get; set; }
    public DateTime? RollbackCompletedUtc { get; set; }

    public static string GetStateFilePath(string workingDirectory)
    {
        return Path.Combine(workingDirectory, StateFileName);
    }
}

public sealed class DatabaseUpgradeRequest
{
    public required string ConnectionString { get; init; }
    public required string WorkingDirectory { get; init; }
    public required string ExecutableType { get; init; }
}

public sealed class DatabaseMigrationProgress
{
    public required string MigrationName { get; init; }
    public required int CurrentMigrationNumber { get; init; }
    public required int TotalMigrations { get; init; }
}

public sealed class DatabaseUpgradePreparation
{
    public required DatabaseUpgradeRequest Request { get; init; }
    public required DatabaseBackupSettings Settings { get; init; }
    public required IReadOnlyList<string> PendingMigrations { get; init; }
    public required string DatabaseName { get; init; }
    public required string StateFilePath { get; init; }
    public string? BackupFilePath { get; set; }
    public bool RestoredPreviousFailedUpgrade { get; init; }

    public bool HasPendingMigrations => PendingMigrations.Count > 0;
    public string? TargetMigration => PendingMigrations.LastOrDefault();
}
