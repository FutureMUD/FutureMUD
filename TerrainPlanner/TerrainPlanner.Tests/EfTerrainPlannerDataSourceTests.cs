using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using TerrainPlanner.Server.Data;

namespace TerrainPlanner.Tests;

[TestClass]
public class EfTerrainPlannerDataSourceTests
{
	[TestMethod]
	public void AccountLookupFiltersBeforeProjectingThePlannerRecord()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(
				"Server=127.0.0.1;Port=3306;Database=futuremud;User ID=terrainplanner;Password=unused",
				new MySqlServerVersion(new Version(8, 0, 0)))
			.Options;
		using var context = new FuturemudDatabaseContext(options);

		var query = EfTerrainPlannerDataSource.ProjectAccounts(context.Accounts
			.Where(account => account.Name == "builder"));
		var sql = query.ToQueryString();

		StringAssert.Contains(sql, "WHERE", StringComparison.OrdinalIgnoreCase);
		StringAssert.Contains(sql, "AuthorityGroups", StringComparison.OrdinalIgnoreCase);
	}
}
