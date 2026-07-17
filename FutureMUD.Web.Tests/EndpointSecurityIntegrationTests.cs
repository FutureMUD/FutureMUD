#nullable enable

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class EndpointSecurityIntegrationTests
{
	private const string PublishingToken = "test-publishing-token-with-more-than-thirty-two-characters";
	private string _root = null!;

	[TestInitialize]
	public void Initialise() =>
		_root = Path.Combine(Path.GetTempPath(), $"futuremud-web-security-tests-{Guid.NewGuid():N}");

	[TestCleanup]
	public void Cleanup()
	{
		if (Directory.Exists(_root))
		{
			Directory.Delete(_root, true);
		}
	}

	[TestMethod]
	public async Task UnauthenticatedMalformedReleaseJsonIsRejectedBeforeBinding()
	{
		await using var factory = CreateFactory();
		using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		using var request = new HttpRequestMessage(HttpMethod.Post, "/api/publishing/v1/releases")
		{
			Content = new StringContent("{", Encoding.UTF8, "application/json")
		};

		using var response = await client.SendAsync(request);

		Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[TestMethod]
	public async Task WrongPublishingBearerIsRejectedWithBearerChallenge()
	{
		await using var factory = CreateFactory();
		using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		using var request = new HttpRequestMessage(HttpMethod.Post, "/api/publishing/v1/releases")
		{
			Content = new StringContent("{", Encoding.UTF8, "application/json")
		};
		request.Headers.Authorization = new AuthenticationHeaderValue(
			"Bearer",
			"wrong-publishing-token-with-more-than-thirty-two-characters");

		using var response = await client.SendAsync(request);

		Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
		Assert.IsTrue(response.Headers.WwwAuthenticate.Any(challenge =>
			challenge.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public async Task UnmatchedPublishingPathReturnsNormalNotFound()
	{
		await using var factory = CreateFactory();
		using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		using var request = new HttpRequestMessage(HttpMethod.Get, "/api/publishing/v1/not-a-route");
		request.Headers.Authorization = new AuthenticationHeaderValue(
			"Bearer",
			"wrong-publishing-token-with-more-than-thirty-two-characters");

		using var response = await client.SendAsync(request);

		Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
		Assert.IsFalse(response.Headers.WwwAuthenticate.Any());
	}

	[TestMethod]
	public async Task WrongBearerRateLimitIsPartitionedByRemoteIp()
	{
		await using var factory = CreateFactory();
		var firstAddress = IPAddress.Parse("192.0.2.10");
		var secondAddress = IPAddress.Parse("192.0.2.11");

		for (var request = 0; request < 120; request++)
		{
			Assert.AreEqual(
				StatusCodes.Status401Unauthorized,
				await SendWrongBearerAsync(factory, firstAddress));
		}

		Assert.AreEqual(
			StatusCodes.Status429TooManyRequests,
			await SendWrongBearerAsync(factory, firstAddress));
		Assert.AreEqual(
			StatusCodes.Status401Unauthorized,
			await SendWrongBearerAsync(factory, secondAddress));
	}

	[TestMethod]
	public async Task AuthenticatedOversizedReleaseMetadataIsRejected()
	{
		await using var factory = CreateFactory();
		using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		var json = JsonSerializer.Serialize(new
		{
			product = "engine",
			version = "1.2.3",
			sourceCommit = new string('a', 40),
			artifacts = Array.Empty<object>(),
			padding = new string('a', 70 * 1024)
		});
		using var request = new HttpRequestMessage(HttpMethod.Post, "/api/publishing/v1/releases")
		{
			Content = new StringContent(json, Encoding.UTF8, "application/json")
		};
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", PublishingToken);

		using var response = await client.SendAsync(request);

		Assert.AreEqual(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
	}

	[DataTestMethod]
	[DataRow("/downloads/!/latest/win-x64")]
	[DataRow("/downloads/INVALID/latest/win-x64")]
	[DataRow("/downloads/!/1.2.3/file.zip")]
	[DataRow("/downloads/!/1.2.3/file.zip.sha256")]
	public async Task InvalidDownloadProductIdentifiersReturnNotFound(string path)
	{
		await using var factory = CreateFactory();
		using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

		using var response = await client.GetAsync(path);

		Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
	}

	private static async Task<int> SendWrongBearerAsync(
		WebApplicationFactory<Program> factory,
		IPAddress remoteAddress)
	{
		var context = await factory.Server.SendAsync(requestContext =>
		{
			requestContext.Connection.RemoteIpAddress = remoteAddress;
			requestContext.Request.Scheme = "https";
			requestContext.Request.Method = HttpMethods.Get;
			requestContext.Request.Path = "/api/publishing/v1/releases/00000000000000000000000000000000";
			requestContext.Request.Headers.Authorization =
				"Bearer wrong-publishing-token-with-more-than-thirty-two-characters";
		}, CancellationToken.None);
		return context.Response.StatusCode;
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
					["FutureMUD:MinimumFreeBytes"] = "0",
					["FutureMUD:PublishingTokenSha256"] = Convert.ToHexString(
						SHA256.HashData(Encoding.UTF8.GetBytes(PublishingToken)))
				});
			});
		});
}
