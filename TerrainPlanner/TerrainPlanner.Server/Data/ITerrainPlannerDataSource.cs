using TerrainPlanner.Contracts;

namespace TerrainPlanner.Server.Data;

public interface ITerrainPlannerDataSource
{
	Task<PlannerAccountRecord?> FindAccountByNameAsync(string normalizedName, CancellationToken cancellationToken);
	Task<PlannerAccountRecord?> FindAccountByIdAsync(long accountId, CancellationToken cancellationToken);
	Task<CatalogueResult<TerrainCatalogueItem>> GetTerrainsAsync(CancellationToken cancellationToken);
	Task<CatalogueResult<TagCatalogueItem>> GetTagsAsync(CancellationToken cancellationToken);
	Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}

public sealed record PlannerAccountRecord(
	long Id,
	string Name,
	string PasswordHash,
	long Salt,
	bool IsRegistered,
	int AccessStatus,
	long? AuthorityGroupId,
	string? AuthorityName,
	int? AuthorityLevel);

public sealed record CatalogueResult<T>(string Revision, IReadOnlyList<T> Items);
