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
		var downloads = await client.GetStringAsync("/downloads");
		StringAssert.Contains(downloads, "release-header");
		StringAssert.Contains(downloads, "download-item");
		StringAssert.Contains(downloads, "FutureMUD Terrain Planner");
		StringAssert.Contains(downloads, "Version <strong>1.2.3</strong>");
		StringAssert.Contains(downloads, $"href=\"/downloads/terrainplanner/1.2.3/{artifact.FileName}\"");
		StringAssert.Contains(downloads, $"href=\"/downloads/terrainplanner/1.2.3/{artifact.FileName}.sha256\"");
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
					AdminHelp = "Administrator-only telescope controls.",
					ConditionalHelp =
					[
						new ConditionalCommandHelpDocument
						{
							Condition = "CanSeeHiddenThings",
							Help = "Conditional-only hidden sight controls."
						}
					]
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
			],
			ProgFunctions =
			[
				new ProgFunctionDocument
				{
					Slug = "addhealingeffect",
					Name = "addhealingeffect",
					Category = "Effects",
					Overloads =
					[
						new ProgFunctionOverloadDocument
						{
							ReturnType = "Effect",
							Contexts = ["standard futureprog", "computer function"],
							GeneralHelp = "Adds a healing effect without rendering <script>bad()</script>.",
							Help = "Adds a healing effect without rendering <script>bad()</script>.",
							Parameters =
							[
								new ProgFunctionParameterDocument
								{
									Name = "perceivable",
									Type = "Perceivable",
									Help = "The target that receives the effect."
								}
							]
						},
						new ProgFunctionOverloadDocument
						{
							ReturnType = "Effect",
							Contexts = ["standard futureprog"],
							Help = "Legacy combined overload help.",
							Parameters =
							[
								new ProgFunctionParameterDocument { Name = "targetId", Type = "Number" }
							]
						}
					]
				}
			],
			ProgTypes =
			[
				new ProgTypeDocument
				{
					Slug = "mud-date-time",
					Name = "Mud Date Time",
					Properties =
					[
						new ProgTypePropertyDocument
						{
							Name = "mudinstant",
							Type = "Text",
							Help = "Returns the absolute instant."
						}
					]
				}
			],
			CollectionExtensions =
			[
				new CollectionExtensionDocument
				{
					Slug = "where",
					Name = "where",
					ReturnType = "Collection (Same Type)",
					Contexts = ["standard futureprog"],
					Help = "Filters the collection using a boolean expression."
				}
			],
			ItemComponents =
			[
				new ItemComponentHelpDocument
				{
					Slug = "container",
					Name = "Container",
					Blurb = "Makes an item a \x1B[1;32m[container]\x1B[0;39m with a \x1B[38;5;171msignal consumer\x1B[0m <script>bad()</script>.",
					BuilderHelp = "Use #3capacity <weight>#0 and \x1B[36mcontents\x1B[0m <img src=x onerror=bad()> to configure it."
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
		StringAssert.Contains(detail, "help-variant--admin");
		StringAssert.Contains(detail, "help-variant--conditional");
		Assert.IsFalse(detail.Contains("<script>alert", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(detail, "&lt;script&gt;alert(1)&lt;/script&gt;");
		StringAssert.Contains(await client.GetStringAsync("/docs/commands?q=telescope"), "movement-look");
		StringAssert.Contains(await client.GetStringAsync("/docs/commands?q=hidden+sight"), "movement-look");

		var functionList = await client.GetStringAsync("/docs/futureprog/functions?q=perceivable");
		StringAssert.Contains(functionList, "addhealingeffect");
		var functionDetail = await client.GetStringAsync("/docs/futureprog/functions/addhealingeffect");
		StringAssert.Contains(functionDetail, "function-overload");
		StringAssert.Contains(functionDetail, "fp-function");
		StringAssert.Contains(functionDetail, "function-parameters");
		StringAssert.Contains(functionDetail, "Included in the function description.");
		Assert.IsFalse(functionDetail.Contains("<script>bad", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(functionDetail, "&lt;script&gt;bad()&lt;/script&gt;");

		var typeList = await client.GetStringAsync("/docs/futureprog/types?q=mudinstant");
		StringAssert.Contains(typeList, "mud-date-time");
		var typeDetail = await client.GetStringAsync("/docs/futureprog/types/mud-date-time");
		StringAssert.Contains(typeDetail, "type-properties");
		StringAssert.Contains(typeDetail, ".mudinstant");

		var collectionDetail = await client.GetStringAsync("/docs/futureprog/collections/where");
		StringAssert.Contains(collectionDetail, "collection-extension-help");
		StringAssert.Contains(collectionDetail, "@CollectionVariable");
		StringAssert.Contains(collectionDetail, "Collection (Same Type)");

		var componentList = await client.GetStringAsync("/docs/items/components");
		StringAssert.Contains(componentList, "ansi-bright-green");
		StringAssert.Contains(componentList, "ansi-bright-pink");
		Assert.IsFalse(componentList.Contains('\x1B'));
		Assert.IsFalse(componentList.Contains("<script>bad", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(componentList, "&lt;script&gt;bad()&lt;/script&gt;");
		var componentDetail = await client.GetStringAsync("/docs/items/components/container");
		StringAssert.Contains(componentDetail, "component-builder-help");
		StringAssert.Contains(componentDetail, "ansi-yellow");
		StringAssert.Contains(componentDetail, "ansi-bright-green");
		StringAssert.Contains(componentDetail, "ansi-bright-pink");
		StringAssert.Contains(componentDetail, "ansi-cyan");
		Assert.IsFalse(componentDetail.Contains('\x1B'));
		Assert.IsFalse(componentDetail.Contains("<script>bad", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(componentDetail.Contains("<img src=x", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(componentDetail, "&lt;img src=x onerror=bad()&gt;");
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
