#nullable enable
using System;
using System.Collections.Generic;

namespace MudSharp.Database;

public interface IDatabaseMigrationService
{
    IReadOnlyList<string> GetPendingMigrations(string connectionString);
    void ApplyMigrations(string connectionString, IReadOnlyList<string> migrations,
        Action<DatabaseMigrationProgress>? progressAction = null);
    string? GetLatestMigrationId(string connectionString);
}
