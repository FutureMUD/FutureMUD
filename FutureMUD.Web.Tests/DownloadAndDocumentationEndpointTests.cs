#nullable enable

using FutureMUD.Web.Publishing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Documentation;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class DownloadAndDocumentationEndpointTests
{
	private string _root = null!;

	[TestInitialize]
	public void Initialise() => _root = Path.Combine(Path.GetTempPath(), $"futuremud-web-http-tests-{Guid.NewGuid():N}");

	[TestCleanup]
	public void Cleanup()
	{
		if (Directory.Exists(_root))
		{
			Directory.Delete(_root, true);
		}
	}

	[TestMethod]
	public async Task VersionedDownloadSupportsRangesEtagsChecksumsAndNoCacheLatestRedirect()
	{
		var bytes = Enumerable.Range(0, 32).Select(value => (byte)value).ToArray();
		var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
		var live = Path.Combine(_root, "releases", "live", "terrainplanner");
		Directory.CreateDirectory(live);
		var artifact = new ReleaseArtifactRequest
		{
			ArtifactId = "win-x64",
			Runtime = "win-x64",
			FileName = "terrainplanner-1.2.3-win-x64.zip",
			Size = bytes.Length,
			Sha256 = sha
		};
		await File.WriteAllBytesAsync(Path.Combine(live, artifact.FileName), bytes);
		await File.WriteAllTextAsync(Path.Combine(live, "release.json"), JsonSerializer.Serialize(new PublicRelease
		{
			Product = "terrainplanner",
			Version = "1.2.3",
			SourceCommit = new string('a', 40),
			PublishedAtUtc = DateTimeOffset.UtcNow,
			Artifacts = [artifact]
		}, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

		await using var factory = CreateFactory();
		var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		var request = new HttpRequestMessage(HttpMethod.Get, $"/downloads/terrainplanner/1.2.3/{artifact.FileName}");
		request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(2, 5);
		var response = await client.SendAsync(request);

		Assert.AreEqual(HttpStatusCode.PartialContent, response.StatusCode);
		CollectionAssert.AreEqual(bytes[2..6], await response.Content.ReadAsByteArrayAsync());
		Assert.AreEqual($"\"{sha}\"", response.Headers.ETag?.Tag);
		StringAssert.Contains(response.Headers.CacheControl?.ToString(), "immutable");
		Assert.AreEqual(sha, response.Headers.GetValues("X-Checksum-SHA256").Single());

		var latest = await client.GetAsync("/downloads/terrainplanner/latest/win-x64");
		Assert.AreEqual(HttpStatusCode.Redirect, latest.StatusCode);
		StringAssert.Contains(latest.Headers.CacheControl?.ToString(), "no-store");
		var checksum = await client.GetStringAsync($"/downloads/terrainplanner/1.2.3/{artifact.FileName}.sha256");
		StringAssert.Contains(checksum, sha);
	}

	[TestMethod]
	public async Task DownloadRejectsPersistedReleaseIdentityMismatch()
	{
		var bytes = new byte[] { 1, 2, 3 };
		var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
		var live = Path.Combine(_root, "releases", "live", "terrainplanner");
		Directory.CreateDirectory(live);
		const string fileName = "terrainplanner-1.2.3-win-x64.zip";
		await File.WriteAllBytesAsync(Path.Combine(live, fileName), bytes);
		await File.WriteAllTextAsync(Path.Combine(live, "release.json"), JsonSerializer.Serialize(new PublicRelease
		{
			Product = "engine",
			Version = "1.2.3",
			SourceCommit = new string('a', 40),
			PublishedAtUtc = DateTimeOffset.UtcNow,
			Artifacts =
			[
				new ReleaseArtifactRequest
				{
					ArtifactId = "win-x64",
					Runtime = "win-x64",
					FileName = fileName,
					Size = bytes.Length,
					Sha256 = sha
				}
			]
		}, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

		await using var factory = CreateFactory();
		var client = factory.CreateClient();
		var response = await client.GetAsync($"/downloads/terrainplanner/1.2.3/{fileName}");

		Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
	}

	[TestMethod]
	public async Task DocumentationSearchFiltersAndDetailEncodesHelpText()
	{
		var documentation = Path.Combine(_root, "documentation", "live");
		Directory.CreateDirectory(documentation);
		await File.WriteAllTextAsync(Path.Combine(documentation, "catalogue.json"), JsonSerializer.Serialize(new DocumentationCatalogue
		{
			EngineVersion = "1.2.3",
			SourceRevision = new string('b', 40),
			GeneratedAtUtc = DateTimeOffset.UtcNow,
			Commands =
			[
				new CommandHelpDocument
				{
					Slug = "movement-look",
					Name = "Look",
					Module = "Movement",
					Audience = "player",
					PermissionLevel = "Player",
					CommandWords = ["look"],
					DefaultHelp = "#2Look safely#0 <script>alert(1)</script>",
					AdminHelp = "#2Look safely#0 <script>alert(1)</script>"
				},
				new CommandHelpDocument
				{
					Slug = "perception-look",
					Name = "Look",
					Module = "Perception",
					Audience = "player",
					PermissionLevel = "Player",
					CommandWords = ["look"],
					DefaultHelp = "A duplicate command name with a stable module slug.",
					AdminHelp = "A duplicate command name with a stable module slug."
				}
			]
		}, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

		await using var factory = CreateFactory();
		var client = factory.CreateClient();
		var search = await client.GetStringAsync("/docs/commands?q=look&audience=player");
		StringAssert.Contains(search, "movement-look");
		StringAssert.Contains(search, "perception-look");
		var detail = await client.GetStringAsync("/docs/commands/movement-look");
		StringAssert.Contains(detail, "ansi-green");
		Assert.IsFalse(detail.Contains("<script>alert", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(detail, "&lt;script&gt;alert(1)&lt;/script&gt;");
	}

	private WebApplicationFactory<Program> CreateFactory() => new WebApplicationFactory<Program>()
		.WithWebHostBuilder(builder =>
		{
			builder.UseEnvironment("Testing");
			builder.ConfigureLogging(logging => logging.ClearProviders());
			builder.ConfigureServices(services => services.AddDataProtection().UseEphemeralDataProtectionProvider());
			builder.ConfigureAppConfiguration((_, configuration) =>
			{
				configuration.AddInMemoryCollection(new Dictionary<string, string?>
				{
					["FutureMUD:DataRoot"] = _root,
					["FutureMUD:MinimumFreeBytes"] = "0"
				});
			});
		});
}
