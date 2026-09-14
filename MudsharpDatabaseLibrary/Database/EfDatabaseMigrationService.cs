#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Database;

public sealed class EfDatabaseMigrationService : IDatabaseMigrationService
{
    public IReadOnlyList<string> GetPendingMigrations(string connectionString)
    {
        using FuturemudDatabaseContext context = CreateContext(connectionString);
        return context.Database.GetPendingMigrations().ToList();
    }

    public void ApplyMigrations(string connectionString, IReadOnlyList<string> migrations,
        Action<DatabaseMigrationProgress>? progressAction = null)
    {
        if (migrations.Count == 0)
        {
            return;
        }

        List<string> migrationNames = migrations.ToList();
        int migrationIndex = 0;
        using FuturemudDatabaseContext context = CreateContext(connectionString, _ =>
        {
            if (migrationIndex >= migrationNames.Count)
            {
                return;
            }

            progressAction?.Invoke(new DatabaseMigrationProgress
            {
                MigrationName = migrationNames[migrationIndex],
                CurrentMigrationNumber = migrationIndex + 1,
                TotalMigrations = migrationNames.Count
            });
            migrationIndex++;
        });
        IMigrator migrator = context.GetService<IMigrator>();
        MigrateToLatest(migrator);
    }

    public string? GetLatestMigrationId(string connectionString)
    {
        using FuturemudDatabaseContext context = CreateMetadataOnlyContext(connectionString);
        return context.Database.GetMigrations().LastOrDefault();
    }

    internal static void MigrateToLatest(IMigrator migrator)
    {
        migrator.Migrate();
    }

    private static FuturemudDatabaseContext CreateContext(string connectionString,
        Action<EventData>? migrationApplyingAction = null)
    {
        DbContextOptionsBuilder<FuturemudDatabaseContext> optionsBuilder = new();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        if (migrationApplyingAction is not null)
        {
            optionsBuilder.LogTo(
                (eventId, _) => eventId == RelationalEventId.MigrationApplying,
                migrationApplyingAction);
        }

        return new FuturemudDatabaseContext(optionsBuilder.Options);
    }

    private static FuturemudDatabaseContext CreateMetadataOnlyContext(string connectionString)
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)))
            .Options;
        return new FuturemudDatabaseContext(options);
    }
}
