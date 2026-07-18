#nullable enable

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class WebApplicationTests
{
	[TestMethod]
	public async Task PublicRoutesHealthFeedAndLegacyRedirectsWork()
	{
		await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
		{
			builder.UseEnvironment("Testing");
			builder.ConfigureLogging(logging => logging.ClearProviders());
			builder.ConfigureServices(services => services.AddDataProtection().UseEphemeralDataProtectionProvider());
		});
		var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

		var home = await client.GetAsync("/");
		Assert.AreEqual(HttpStatusCode.OK, home.StatusCode);
		var homeHtml = await home.Content.ReadAsStringAsync();
		StringAssert.Contains(homeHtml, "https://discord.gg/fyKnckr4PG");
		StringAssert.Contains(homeHtml, "https://github.com/FutureMUD/FutureMUD");
		StringAssert.Contains(homeHtml, "Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License");
		StringAssert.Contains(homeHtml, "/images/gameplay/medical-treatment.png");
		StringAssert.Contains(homeHtml, "/images/gameplay/combat.png");
		StringAssert.Contains(homeHtml, "data-carousel");
		StringAssert.Contains(homeHtml, "data-nav-disclosure");
		foreach (var documentationPath in new[]
		{
			"/docs/commands",
			"/docs/futureprog/functions",
			"/docs/futureprog/types",
			"/docs/futureprog/collections",
			"/docs/items/components",
			"/patch-notes"
		})
		{
			StringAssert.Contains(homeHtml, $"href=\"{documentationPath}\"");
		}
		var medicalScreenshot = await client.GetAsync("/images/gameplay/medical-treatment.png");
		Assert.AreEqual(HttpStatusCode.OK, medicalScreenshot.StatusCode);
		Assert.AreEqual("image/png", medicalScreenshot.Content.Headers.ContentType?.MediaType);
		var combatScreenshot = await client.GetAsync("/images/gameplay/combat.png");
		Assert.AreEqual(HttpStatusCode.OK, combatScreenshot.StatusCode);
		Assert.AreEqual("image/png", combatScreenshot.Content.Headers.ContentType?.MediaType);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/js/site.js")).StatusCode);

		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/about")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/license")).StatusCode);
		var gettingStarted = await client.GetAsync("/getting-started");
		Assert.AreEqual(HttpStatusCode.OK, gettingStarted.StatusCode);
		var requirementsHtml = await gettingStarted.Content.ReadAsStringAsync();
		StringAssert.Contains(requirementsHtml, "Minimum requirements");
		StringAssert.Contains(requirementsHtml, "MySQL Server 8.0");

		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/patch-notes")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/patch-notes/website-publishing-rollout")).StatusCode);
		Assert.AreEqual(HttpStatusCode.NotFound, (await client.GetAsync("/patch-notes/not-a-real-note")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/news/feed.xml")).StatusCode);
		var sitemap = await client.GetAsync("/sitemap.xml");
		Assert.AreEqual(HttpStatusCode.OK, sitemap.StatusCode);
		var sitemapHtml = await sitemap.Content.ReadAsStringAsync();
		StringAssert.Contains(sitemapHtml, "https://futuremud.com/patch-notes/website-publishing-rollout");
		StringAssert.Contains(sitemapHtml, "https://futuremud.com/docs/futureprog/types");
		StringAssert.Contains(sitemapHtml, "https://futuremud.com/docs/futureprog/collections");
		StringAssert.Contains(sitemapHtml, "https://futuremud.com/docs/items/components");
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
		var legacy = await client.GetAsync("/ProgFunctionsAlphabetically.html");
		Assert.AreEqual(HttpStatusCode.MovedPermanently, legacy.StatusCode);
		Assert.AreEqual("/docs/futureprog/functions", legacy.Headers.Location?.ToString());
		var legacyTypes = await client.GetAsync("/ProgTypeHelps.html");
		Assert.AreEqual(HttpStatusCode.MovedPermanently, legacyTypes.StatusCode);
		Assert.AreEqual("/docs/futureprog/types", legacyTypes.Headers.Location?.ToString());
		var legacyCollections = await client.GetAsync("/ProgCollectionHelps.html");
		Assert.AreEqual(HttpStatusCode.MovedPermanently, legacyCollections.StatusCode);
		Assert.AreEqual("/docs/futureprog/collections", legacyCollections.Headers.Location?.ToString());
		foreach (var documentationPath in new[]
		{
			"/docs/commands",
			"/docs/futureprog/functions",
			"/docs/futureprog/types",
			"/docs/futureprog/collections",
			"/docs/items/components"
		})
		{
			Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync(documentationPath)).StatusCode);
		}
		Assert.AreEqual(HttpStatusCode.NotFound, (await client.GetAsync("/news/not-a-real-post")).StatusCode);
	}
}
