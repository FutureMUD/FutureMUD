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
		StringAssert.Contains(await sitemap.Content.ReadAsStringAsync(), "https://futuremud.com/patch-notes/website-publishing-rollout");
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
		var legacy = await client.GetAsync("/ProgFunctionsAlphabetically.html");
		Assert.AreEqual(HttpStatusCode.MovedPermanently, legacy.StatusCode);
		Assert.AreEqual("/docs/futureprog/functions", legacy.Headers.Location?.ToString());
		Assert.AreEqual(HttpStatusCode.NotFound, (await client.GetAsync("/news/not-a-real-post")).StatusCode);
	}
}
