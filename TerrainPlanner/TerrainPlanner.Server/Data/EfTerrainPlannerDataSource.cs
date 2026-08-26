using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TerrainPlanner.Contracts;

namespace TerrainPlanner.Server.Data;

public sealed class EfTerrainPlannerDataSource : ITerrainPlannerDataSource
{
	private readonly IDbContextFactory<FuturemudDatabaseContext> _contextFactory;

	public EfTerrainPlannerDataSource(IDbContextFactory<FuturemudDatabaseContext> contextFactory)
	{
		_contextFactory = contextFactory;
	}

	public async Task<PlannerAccountRecord?> FindAccountByNameAsync(string normalizedName,
		CancellationToken cancellationToken)
	{
		await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
		return await AccountQuery(context)
			.SingleOrDefaultAsync(account => account.Name == normalizedName, cancellationToken);
	}

	public async Task<PlannerAccountRecord?> FindAccountByIdAsync(long accountId, CancellationToken cancellationToken)
	{
		await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
		return await AccountQuery(context)
			.SingleOrDefaultAsync(account => account.Id == accountId, cancellationToken);
	}

	public async Task<CatalogueResult<TerrainCatalogueItem>> GetTerrainsAsync(CancellationToken cancellationToken)
	{
		await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
		var items = await context.Terrains
			.AsNoTracking()
			.OrderBy(terrain => terrain.Name)
			.Select(terrain => new TerrainCatalogueItem(
				terrain.Id,
				terrain.Name,
				terrain.TerrainEditorColour ?? "#FF808080",
				terrain.TerrainEditorText ?? string.Empty))
			.ToListAsync(cancellationToken);
		return new CatalogueResult<TerrainCatalogueItem>(ComputeRevision(items), items);
	}

	public async Task<CatalogueResult<TagCatalogueItem>> GetTagsAsync(CancellationToken cancellationToken)
	{
		await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
		var records = await context.Tags
			.AsNoTracking()
			.Select(tag => new TagRecord(tag.Id, tag.Name, tag.ParentId))
			.ToListAsync(cancellationToken);
		var byId = records.ToDictionary(record => record.Id);
		var items = records
			.Select(record => new TagCatalogueItem(record.Id, record.Name, BuildFullName(record, byId), record.ParentId))
			.OrderBy(tag => tag.FullName, StringComparer.OrdinalIgnoreCase)
			.ToList();
		return new CatalogueResult<TagCatalogueItem>(ComputeRevision(items), items);
	}

	public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
			return await context.Database.CanConnectAsync(cancellationToken);
		}
		catch
		{
			return false;
		}
	}

	private static IQueryable<PlannerAccountRecord> AccountQuery(FuturemudDatabaseContext context) =>
		context.Accounts
			.AsNoTracking()
			.Select(account => new PlannerAccountRecord(
				account.Id,
				account.Name,
				account.Password,
				account.Salt,
				account.IsRegistered,
				account.AccessStatus,
				account.AuthorityGroupId,
				account.AuthorityGroup == null ? null : account.AuthorityGroup.Name,
				account.AuthorityGroup == null ? null : account.AuthorityGroup.AuthorityLevel));

	private static string BuildFullName(TagRecord record, IReadOnlyDictionary<long, TagRecord> byId)
	{
		var names = new Stack<string>();
		var visited = new HashSet<long>();
		TagRecord? current = record;
		while (current is not null)
		{
			if (!visited.Add(current.Id))
			{
				throw new InvalidDataException($"Tag hierarchy contains a cycle at tag #{current.Id}.");
			}

			names.Push(current.Name);
			current = current.ParentId is { } parentId && byId.TryGetValue(parentId, out var parent) ? parent : null;
		}

		return string.Join(" / ", names);
	}

	private static string ComputeRevision<T>(IReadOnlyList<T> items)
	{
		var json = JsonSerializer.Serialize(items);
		return $"\"{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant()}\"";
	}

	private sealed record TagRecord(long Id, string Name, long? ParentId);
}
