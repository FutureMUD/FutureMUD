#nullable enable
using Microsoft.EntityFrameworkCore;
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

        using FuturemudDatabaseContext context = CreateContext(connectionString);
        IMigrator migrator = context.GetService<IMigrator>();
        int index = 1;
        foreach (string migration in migrations)
        {
            progressAction?.Invoke(new DatabaseMigrationProgress
            {
                MigrationName = migration,
                CurrentMigrationNumber = index,
                TotalMigrations = migrations.Count
            });
            migrator.Migrate(migration);
            index++;
        }
    }

    public string? GetLatestMigrationId(string connectionString)
    {
        using FuturemudDatabaseContext context = CreateMetadataOnlyContext(connectionString);
        return context.Database.GetMigrations().LastOrDefault();
    }

    private static FuturemudDatabaseContext CreateContext(string connectionString)
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static FuturemudDatabaseContext CreateMetadataOnlyContext(string connectionString)
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)))
            .Options;
        return new FuturemudDatabaseContext(options);
    }
}
