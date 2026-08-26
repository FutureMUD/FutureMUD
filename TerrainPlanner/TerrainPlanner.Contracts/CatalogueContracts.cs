namespace TerrainPlanner.Contracts;

public sealed record TerrainCatalogueItem(
	long Id,
	string Name,
	string EditorColour,
	string EditorText);

public sealed record TagCatalogueItem(
	long Id,
	string ShortName,
	string FullName,
	long? ParentId);

public sealed record AuthSession(
	bool IsAuthenticated,
	long? AccountId,
	string? AccountName,
	string? AuthorityName,
	int? AuthorityLevel,
	DateTimeOffset? ExpiresAtUtc);

public sealed record LoginRequest(string AccountName, string Password);

public sealed record AntiforgeryTokenResponse(string Token);
