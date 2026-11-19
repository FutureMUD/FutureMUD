#nullable enable

using FutureMUD.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class BuilderGuideTests
{
	[TestMethod]
	public async Task PackagedGuidesAreSearchableAndNavigable()
	{
		await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
		{
			builder.UseEnvironment("Testing");
			builder.ConfigureLogging(logging => logging.ClearProviders());
			builder.ConfigureServices(services => services.AddDataProtection().UseEphemeralDataProtectionProvider());
		});
		using var client = factory.CreateClient();
		StringAssert.Contains(await client.GetStringAsync("/"), "href=\"/docs/builder-guides\"");
		StringAssert.Contains(await client.GetStringAsync("/docs/commands"), "href=\"/docs/builder-guides\"");
		var list = await client.GetStringAsync(BuilderGuideCatalogue.Route);
		StringAssert.Contains(list, "Room Building Builder Guide");
		StringAssert.Contains(list, "name=\"category\"");
		StringAssert.Contains(list, "value=\"Magic\"");

		var search = await client.GetStringAsync($"{BuilderGuideCatalogue.Route}?q=RouteRoom&category=World%20building");
		StringAssert.Contains(search, "href=\"/docs/builder-guides/rooms\"");
		Assert.DoesNotContain("href=\"/docs/builder-guides/vancian-magic\"", search);
		var excluded = await client.GetStringAsync($"{BuilderGuideCatalogue.Route}?q=RouteRoom&category=Magic");
		Assert.DoesNotContain("href=\"/docs/builder-guides/rooms\"", excluded);

		var rooms = await client.GetStringAsync($"{BuilderGuideCatalogue.Route}/rooms");
		StringAssert.Contains(rooms, "<h1>Room Building Builder Guide</h1>");
		StringAssert.Contains(rooms, "<table>");
		StringAssert.Contains(rooms, "href=\"/docs/builder-guides/environmental-magic\"");
		StringAssert.Contains(rooms, "https://github.com/FutureMUD/FutureMUD/blob/master/Design%20Documents/World/Route_Cell_System.md");
		Assert.DoesNotContain("slug: rooms", rooms);
		var items = await client.GetStringAsync($"{BuilderGuideCatalogue.Route}/item-authoring");
		StringAssert.Contains(items, "href=\"#writing-blocks\"");
		StringAssert.Contains(items, "id=\"writing-blocks\"");
		Assert.AreEqual(HttpStatusCode.NotFound, (await client.GetAsync($"{BuilderGuideCatalogue.Route}/autobuilder-seeder-guide")).StatusCode);
		Assert.AreEqual(HttpStatusCode.NotFound, (await client.GetAsync($"{BuilderGuideCatalogue.Route}/rooms/extra")).StatusCode);

		var sitemap = await client.GetStringAsync("/sitemap.xml");
		foreach (var guide in factory.Services.GetRequiredService<BuilderGuideCatalogue>().Guides.Values)
		{
			var detail = $"{BuilderGuideCatalogue.Route}/{guide.Slug}";
			StringAssert.Contains(sitemap, $"<loc>https://futuremud.com{detail}</loc>");
			Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync(detail)).StatusCode, detail);
		}
	}

	[TestMethod]
	public void EmbeddedCollectionMatchesThePublicationFolder()
	{
		var repository = new DirectoryInfo(AppContext.BaseDirectory);
		while (repository is not null && !Directory.Exists(Path.Combine(repository.FullName, "Design Documents")))
		{
			repository = repository.Parent;
		}
		Assert.IsNotNull(repository, "Could not locate the repository's Design Documents directory.");
		var sourceNames = Directory.EnumerateFiles(Path.Combine(repository.FullName, "Design Documents", "Builder Guides"), "*.md")
			.Select(path => BuilderGuideCatalogue.ResourcePrefix + Path.GetFileName(path))
			.ToArray();
		var embeddedNames = typeof(BuilderGuideCatalogue).Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(BuilderGuideCatalogue.ResourcePrefix, StringComparison.Ordinal))
			.ToArray();
		CollectionAssert.AreEquivalent(sourceNames, embeddedNames);
	}

	[TestMethod]
	public void GuideRenderingPreservesStructureAndRestrictsLinkTargets()
	{
		var guides = BuilderGuideCatalogue.Load([
			("First.md", Guide("first", """
				## Command examples

				1. First step
				2. Second step

				[Next](Second.md#details) and [public reference](/docs/commands).

				<span>Raw HTML</span> [unsafe](javascript:noop) <javascript:noop>
				[unsafe reference][bad] [unsafe external](//example.invalid)

				[bad]: data:text/plain,example
				""")),
			("Second.md", Guide("second", "## Details\n\nSome information."))
		]);

		var html = guides["first"].Html;
		StringAssert.Contains(html, "<h2 id=\"command-examples\">");
		StringAssert.Contains(html, "<ol>");
		StringAssert.Contains(html, "href=\"/docs/builder-guides/second#details\"");
		StringAssert.Contains(html, "href=\"/docs/commands\"");
		StringAssert.Contains(html, "&lt;span&gt;");
		Assert.DoesNotContain("href=\"javascript:", html);
		Assert.DoesNotContain("href=\"data:", html);
		Assert.DoesNotContain("href=\"//", html);
	}

	[TestMethod]
	public void CatalogueRejectsMissingMetadataAndDuplicateSlugs()
	{
		Assert.ThrowsExactly<InvalidDataException>(() => BuilderGuideCatalogue.Load([("Draft.md", "# Draft\n\nSome content.")]));
		Assert.ThrowsExactly<InvalidDataException>(() => BuilderGuideCatalogue.Load([
			("First.md", Guide("duplicate", "First.")),
			("Second.md", Guide("duplicate", "Second."))
		]));
	}

	private static string Guide(string slug, string body) => $"""
		---
		slug: {slug}
		category: Test
		summary: Guide summary
		---

		# Title

		{body}
		""";
}
