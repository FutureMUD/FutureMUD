#nullable enable

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MudSharp.Database;

public class FuturemudDatabaseContextFactory : IDesignTimeDbContextFactory<FuturemudDatabaseContext>
{
    public FuturemudDatabaseContext CreateDbContext(string[] args)
    {
		var connectionString = Environment.GetEnvironmentVariable("FUTUREMUD_EF_CONNECTION_STRING");
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			// Model generation needs provider metadata, not access to an operational account.
			connectionString = "server=localhost;port=3306;database=futuremud_design_time;uid=futuremud_design_time";
		}

        DbContextOptionsBuilder<FuturemudDatabaseContext> optionsBuilder = new();
        optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.Parse("8.0.36-mysql"));

        return new FuturemudDatabaseContext(optionsBuilder.Options) { ConnectionString = connectionString };
    }
}
