#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MudSharp.Database;

public class FuturemudDatabaseContextFactory : IDesignTimeDbContextFactory<FuturemudDatabaseContext>
{
	public FuturemudDatabaseContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<FuturemudDatabaseContext>();
		optionsBuilder.UseLazyLoadingProxies();
		optionsBuilder.UseMySql(
			"server=localhost;port=3306;database=dbo;uid=futuremud;password=rpiengine2020",
			ServerVersion.Parse("8.0.36-mysql"));

		return new FuturemudDatabaseContext(optionsBuilder.Options);
	}
}
