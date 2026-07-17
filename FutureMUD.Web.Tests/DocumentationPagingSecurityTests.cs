#nullable enable

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Documentation;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class DocumentationPagingSecurityTests
{
	private string _root = null!;

	[TestInitialize]
	public void Initialise() =>
		_root = Path.Combine(Path.GetTempPath(), $"futuremud-web-doc-paging-tests-{Guid.NewGuid():N}");

	[TestCleanup]
	public void Cleanup()
	{
		if (Directory.Exists(_root))
		{
			Directory.Delete(_root, true);
		}
	}

	[TestMethod]
	public async Task MaximumEntryCatalogueSearchIsInputCappedAndRendersOnlyOneBoundedPage()
	{
		var documentation = Path.Combine(_root, "documentation", "live");
		Directory.CreateDirectory(documentation);
		var catalogue = new DocumentationCatalogue
		{
			EngineVersion = "1.2.3",
			SourceRevision = new string('a', 40),
			GeneratedAtUtc = DateTimeOffset.UtcNow,
			Commands = Enumerable.Range(0, 50_000)
				.Select(index => new CommandHelpDocument
				{
					Slug = $"command-{index:D5}",
					Name = $"{new string('c', 200)} {index:D5}",
					Module = $"module-{index:D5}",
					Audience = "player",
					CommandWords = ["command"],
					DefaultHelp = "help",
					AdminHelp = "help"
				})
				.ToList()
		};
		await using (var stream = File.Create(Path.Combine(documentation, "catalogue.json")))
		{
			await JsonSerializer.SerializeAsync(
				stream,
				catalogue,
				new JsonSerializerOptions(JsonSerializerDefaults.Web));
		}

		await using var factory = CreateFactory();
		using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		var path = $"/docs/commands?q={new string('c', 250)}&pageNumber=999999";

		using var response = await client.GetAsync(path);
		var html = await response.Content.ReadAsStringAsync();

		Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		StringAssert.Contains(html, "50000 result(s)");
		StringAssert.Contains(html, "Page 500 of 500");
		Assert.AreEqual(100, Regex.Matches(html, "href=\"/docs/commands/command-").Count);
		Assert.AreEqual(256, Regex.Matches(html, "<option value=\"module-[0-9]{5}\"").Count);
		StringAssert.Contains(html, "command-49900");
		StringAssert.Contains(html, "command-49999");
		Assert.IsFalse(html.Contains("command-49899", StringComparison.Ordinal));

		var queryValue = Regex.Match(html, "name=\"q\" value=\"([^\"]*)\"");
		Assert.IsTrue(queryValue.Success);
		Assert.AreEqual(200, WebUtility.HtmlDecode(queryValue.Groups[1].Value).Length);
	}

	private WebApplicationFactory<Program> CreateFactory() => new WebApplicationFactory<Program>()
		.WithWebHostBuilder(builder =>
		{
			builder.UseEnvironment("Testing");
			builder.ConfigureLogging(logging => logging.ClearProviders());
			builder.ConfigureServices(services =>
				services.AddDataProtection().UseEphemeralDataProtectionProvider());
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
