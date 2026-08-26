using System.Net;
using System.Net.Http.Json;
using TerrainPlanner.Contracts;

namespace TerrainPlanner.Client.Services;

public sealed class PlannerApiClient
{
	private readonly HttpClient _httpClient;

	public PlannerApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<AuthSession?> GetSessionAsync(CancellationToken cancellationToken = default)
	{
		using var response = await _httpClient.GetAsync("api/v1/auth/session", cancellationToken);
		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			return null;
		}

		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<AuthSession>(cancellationToken);
	}

	public async Task<AuthSession?> LoginAsync(string accountName, string password,
		CancellationToken cancellationToken = default)
	{
		var token = await GetAntiforgeryTokenAsync(cancellationToken);
		using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/login")
		{
			Content = JsonContent.Create(new LoginRequest(accountName, password))
		};
		request.Headers.Add("X-XSRF-TOKEN", token);
		using var response = await _httpClient.SendAsync(request, cancellationToken);
		if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
		{
			return null;
		}

		if (response.StatusCode == HttpStatusCode.TooManyRequests)
		{
			throw new InvalidOperationException("Too many login attempts. Wait a minute and try again.");
		}

		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<AuthSession>(cancellationToken);
	}

	public async Task LogoutAsync(CancellationToken cancellationToken = default)
	{
		var token = await GetAntiforgeryTokenAsync(cancellationToken);
		using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/logout");
		request.Headers.Add("X-XSRF-TOKEN", token);
		using var response = await _httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public Task<CatalogueFetch<TerrainCatalogueItem>> GetTerrainsAsync(string? etag = null,
		CancellationToken cancellationToken = default) =>
		GetCatalogueAsync<TerrainCatalogueItem>("api/v1/terrains", etag, cancellationToken);

	public Task<CatalogueFetch<TagCatalogueItem>> GetTagsAsync(string? etag = null,
		CancellationToken cancellationToken = default) =>
		GetCatalogueAsync<TagCatalogueItem>("api/v1/tags", etag, cancellationToken);

	private async Task<string> GetAntiforgeryTokenAsync(CancellationToken cancellationToken)
	{
		var response = await _httpClient.GetFromJsonAsync<AntiforgeryTokenResponse>(
			"api/v1/auth/antiforgery", cancellationToken);
		return response?.Token ?? throw new InvalidOperationException("The server did not issue an antiforgery token.");
	}

	private async Task<CatalogueFetch<T>> GetCatalogueAsync<T>(string uri, string? etag,
		CancellationToken cancellationToken)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, uri);
		if (!string.IsNullOrWhiteSpace(etag))
		{
			request.Headers.TryAddWithoutValidation("If-None-Match", etag);
		}

		using var response = await _httpClient.SendAsync(request, cancellationToken);
		if (response.StatusCode == HttpStatusCode.NotModified)
		{
			return new CatalogueFetch<T>(etag, null, true);
		}

		response.EnsureSuccessStatusCode();
		var items = await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken) ?? [];
		return new CatalogueFetch<T>(response.Headers.ETag?.Tag, items, false);
	}
}

public sealed record CatalogueFetch<T>(string? Revision, IReadOnlyList<T>? Items, bool NotModified);
