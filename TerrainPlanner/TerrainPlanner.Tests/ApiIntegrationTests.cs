using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MudSharp.Accounts;
using MudSharp.Framework;
using TerrainPlanner.Contracts;
using TerrainPlanner.Server.Data;

namespace TerrainPlanner.Tests;

[TestClass]
public class ApiIntegrationTests
{
	[TestMethod]
	public async Task CatalogueRequiresAdminSessionAndUsesPurposeBuiltDtos()
	{
		await using var factory = new PlannerFactory();
		using var client = factory.CreatePlannerClient();
		Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/terrains")).StatusCode);

		var login = await LoginAsync(client);
		Assert.AreEqual(HttpStatusCode.OK, login.StatusCode);
		Assert.IsTrue(login.Headers.TryGetValues("Set-Cookie", out var cookieHeaders));
		var sessionCookie = cookieHeaders.Single(value => value.StartsWith("__Host-FutureMUDTerrainPlanner=", StringComparison.Ordinal));
		StringAssert.Contains(sessionCookie, "secure", StringComparison.OrdinalIgnoreCase);
		StringAssert.Contains(sessionCookie, "httponly", StringComparison.OrdinalIgnoreCase);
		StringAssert.Contains(sessionCookie, "samesite=strict", StringComparison.OrdinalIgnoreCase);

		var terrains = await client.GetFromJsonAsync<List<TerrainCatalogueItem>>("/api/v1/terrains");
		Assert.AreEqual(1, terrains!.Count);
		Assert.AreEqual(".", terrains![0].EditorText);
		var raw = await client.GetStringAsync("/api/v1/terrains");
		Assert.IsFalse(raw.Contains("account", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(raw.Contains("navigation", StringComparison.OrdinalIgnoreCase));
		var tagResponse = await client.GetAsync("/api/v1/tags");
		var tags = await tagResponse.Content.ReadFromJsonAsync<List<TagCatalogueItem>>();
		Assert.AreEqual("road", tags![0].ShortName);
		StringAssert.Contains(await tagResponse.Content.ReadAsStringAsync(), "shortName");
		using var conditional = new HttpRequestMessage(HttpMethod.Get, "/api/v1/tags");
		conditional.Headers.IfNoneMatch.ParseAdd(tagResponse.Headers.ETag!.Tag);
		Assert.AreEqual(HttpStatusCode.NotModified, (await client.SendAsync(conditional)).StatusCode);
	}

	[TestMethod]
	public async Task AuthorityDemotionImmediatelyRevokesAnExistingSession()
	{
		await using var factory = new PlannerFactory();
		using var client = factory.CreatePlannerClient();
		Assert.AreEqual(HttpStatusCode.OK, (await LoginAsync(client)).StatusCode);

		factory.DataSource.Account = factory.DataSource.Account! with
		{
			AuthorityLevel = (int)PermissionLevel.JuniorAdmin,
			AuthorityName = PermissionLevel.JuniorAdmin.ToString()
		};

		Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/auth/session")).StatusCode);
		Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/tags")).StatusCode);
	}

	[TestMethod]
	public async Task LegacyTerrainAliasIsAuthenticatedAndDeprecated()
	{
		await using var factory = new PlannerFactory();
		using var client = factory.CreatePlannerClient();
		Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/Terrain")).StatusCode);
		await LoginAsync(client);

		var response = await client.GetAsync("/Terrain");

		Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		Assert.AreEqual("true", response.Headers.GetValues("Deprecation").Single());
		Assert.IsTrue(response.Headers.Contains("Sunset"));
	}

	[TestMethod]
	public async Task RemovedWriteEndpointsAreNotAvailable()
	{
		await using var factory = new PlannerFactory();
		using var client = factory.CreatePlannerClient();
		foreach (var path in new[] { "/api/v1/auth/register", "/api/v1/accounts", "/api/v1/database/backup", "/DatabaseBackup" })
		{
			var response = await client.PostAsJsonAsync(path, new { value = "forbidden" });
			Assert.IsTrue(response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.MethodNotAllowed,
				$"Expected {path} to be absent, but received {(int)response.StatusCode}.");
		}
	}

	[TestMethod]
	public async Task ReadinessReflectsDatabaseAvailabilityWhileLivenessDoesNot()
	{
		await using var factory = new PlannerFactory();
		using var client = factory.CreatePlannerClient();
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
		factory.DataSource.DatabaseAvailable = false;
		Assert.AreEqual(HttpStatusCode.ServiceUnavailable, (await client.GetAsync("/health/ready")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
	}

	[TestMethod]
	public async Task LogoutRequiresAntiforgeryAndInvalidatesSession()
	{
		await using var factory = new PlannerFactory();
		using var client = factory.CreatePlannerClient();
		await LoginAsync(client);
		Assert.AreEqual(HttpStatusCode.BadRequest, (await client.PostAsync("/api/v1/auth/logout", null)).StatusCode);

		var token = await GetAntiforgeryTokenAsync(client);
		using var logout = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
		logout.Headers.Add("X-XSRF-TOKEN", token);
		Assert.AreEqual(HttpStatusCode.NoContent, (await client.SendAsync(logout)).StatusCode);
		Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/auth/session")).StatusCode);
	}

	private static async Task<HttpResponseMessage> LoginAsync(HttpClient client)
	{
		var token = await GetAntiforgeryTokenAsync(client);
		using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/login")
		{
			Content = JsonContent.Create(new LoginRequest("builder", "legacy password"))
		};
		request.Headers.Add("X-XSRF-TOKEN", token);
		return await client.SendAsync(request);
	}

	private static async Task<string> GetAntiforgeryTokenAsync(HttpClient client)
	{
		var response = await client.GetFromJsonAsync<AntiforgeryTokenResponse>("/api/v1/auth/antiforgery");
		return response!.Token;
	}

	private sealed class PlannerFactory : WebApplicationFactory<global::Program>
	{
		public FakeDataSource DataSource { get; } = new();

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");
			builder.ConfigureLogging(logging => logging.ClearProviders());
			builder.ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(
				new Dictionary<string, string?>
				{
					["ConnectionStrings:Database"] = "Server=127.0.0.1;Port=65000;Database=unused;User ID=unused;Password=unused",
					["TerrainPlanner:MySqlVersion"] = "8.0.0"
				}));
			builder.ConfigureTestServices(services =>
			{
				services.RemoveAll<ITerrainPlannerDataSource>();
				services.AddSingleton<ITerrainPlannerDataSource>(DataSource);
				services.RemoveAll<IDataProtectionProvider>();
				services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
			});
		}

		public HttpClient CreatePlannerClient() => CreateClient(new WebApplicationFactoryClientOptions
		{
			BaseAddress = new Uri("https://localhost"),
			AllowAutoRedirect = false,
			HandleCookies = true
		});
	}

	private sealed class FakeDataSource : ITerrainPlannerDataSource
	{
		private const long Salt = 123456;
		public PlannerAccountRecord? Account { get; set; } = new(
			1, "Builder", SecurityUtilities.GetPasswordHash("legacy password", Salt), Salt, true,
			(int)AccountStatus.Normal, 1, PermissionLevel.Admin.ToString(), (int)PermissionLevel.Admin);
		public bool DatabaseAvailable { get; set; } = true;

		public Task<PlannerAccountRecord?> FindAccountByNameAsync(string normalizedName, CancellationToken cancellationToken) =>
			Task.FromResult(Account is not null && string.Equals(normalizedName, Account.Name, StringComparison.OrdinalIgnoreCase)
				? Account
				: null);

		public Task<PlannerAccountRecord?> FindAccountByIdAsync(long accountId, CancellationToken cancellationToken) =>
			Task.FromResult(Account?.Id == accountId ? Account : null);

		public Task<CatalogueResult<TerrainCatalogueItem>> GetTerrainsAsync(CancellationToken cancellationToken) =>
			Task.FromResult(new CatalogueResult<TerrainCatalogueItem>("\"terrain-revision\"",
				[new TerrainCatalogueItem(1, "Plains", "#668844", ".")]));

		public Task<CatalogueResult<TagCatalogueItem>> GetTagsAsync(CancellationToken cancellationToken) =>
			Task.FromResult(new CatalogueResult<TagCatalogueItem>("\"tag-revision\"",
				[new TagCatalogueItem(2, "road", "world - route - road", 1)]));

		public Task<bool> CanConnectAsync(CancellationToken cancellationToken) => Task.FromResult(DatabaseAvailable);
	}
}
