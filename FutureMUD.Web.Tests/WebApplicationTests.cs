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

		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/about")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/news/feed.xml")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/sitemap.xml")).StatusCode);
		Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
		var legacy = await client.GetAsync("/ProgFunctionsAlphabetically.html");
		Assert.AreEqual(HttpStatusCode.MovedPermanently, legacy.StatusCode);
		Assert.AreEqual("/docs/futureprog/functions", legacy.Headers.Location?.ToString());
		Assert.AreEqual(HttpStatusCode.NotFound, (await client.GetAsync("/news/not-a-real-post")).StatusCode);
	}
}
