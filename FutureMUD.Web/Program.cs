#nullable enable

using FutureMUD.Web.Configuration;
using FutureMUD.Web.Publishing;
using FutureMUD.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
	options.SingleLine = true;
	options.TimestampFormat = "yyyy-MM-dd HH:mm:ss zzz ";
});
var configuredDataRoot = builder.Configuration
	.GetSection(FutureMudWebOptions.SectionName)
	.GetValue<string>(nameof(FutureMudWebOptions.DataRoot)) ?? "/var/lib/futuremud-web";

builder.WebHost.ConfigureKestrel(options =>
{
	options.AddServerHeader = false;
	options.Limits.MaxRequestBodySize = ReleaseStore.ChunkSize + 1024;
	options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddRazorPages();
builder.Services.AddMemoryCache(options => options.SizeLimit = 100_000);
builder.Services.AddDataProtection()
	.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(configuredDataRoot, "keys")))
	.SetApplicationName("FutureMUD.Web");
builder.Services.Configure<HstsOptions>(options =>
{
	options.IncludeSubDomains = true;
	options.MaxAge = TimeSpan.FromDays(365);
});
builder.Services.AddResponseCompression();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
	options.KnownIPNetworks.Clear();
	options.KnownProxies.Clear();
	options.KnownProxies.Add(IPAddress.Loopback);
	options.KnownProxies.Add(IPAddress.IPv6Loopback);
});
builder.Services.AddOptions<FutureMudWebOptions>()
	.Bind(builder.Configuration.GetSection(FutureMudWebOptions.SectionName))
	.Validate(options => !string.IsNullOrWhiteSpace(options.DataRoot), "FutureMUD:DataRoot is required.")
	.Validate(options => !string.IsNullOrWhiteSpace(options.ContentRoot), "FutureMUD:ContentRoot is required.")
	.Validate(options => options.MinimumFreeBytes >= 0, "FutureMUD:MinimumFreeBytes cannot be negative.")
	.Validate(options => options.DraftLifetime > TimeSpan.Zero, "FutureMUD:DraftLifetime must be positive.")
	.Validate(options => options.PreviousReleaseLifetime > TimeSpan.Zero, "FutureMUD:PreviousReleaseLifetime must be positive.")
	.Validate(options =>
		string.IsNullOrEmpty(options.PublishingTokenSha256) ||
		(options.PublishingTokenSha256.Length == 64 && options.PublishingTokenSha256.All(Uri.IsHexDigit)),
		"FutureMUD:PublishingTokenSha256 must be empty or exactly 64 hexadecimal characters.")
	.ValidateOnStart();
builder.Services.AddSingleton<MarkdownContentService>();
builder.Services.AddSingleton<DocumentationService>();
builder.Services.AddSingleton<ReleaseProductCatalogue>();
builder.Services.AddSingleton<ReleaseStore>();
builder.Services.AddSingleton<PublishingAuthentication>();
builder.Services.AddHostedService<ReleaseCleanupService>();
builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.AddPolicy("publishing", context =>
		RateLimitPartition.GetFixedWindowLimiter(
			context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
			_ => new FixedWindowRateLimiterOptions
			{
				PermitLimit = 120,
				Window = TimeSpan.FromMinutes(1),
				QueueLimit = 0,
				AutoReplenishment = true
			}));
});

var app = builder.Build();

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
	context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self'; connect-src 'self'; media-src 'self'; frame-src 'none'; object-src 'none'; worker-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'self'";
	context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
	context.Response.Headers["Permissions-Policy"] = "accelerometer=(), autoplay=(), camera=(), display-capture=(), encrypted-media=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), picture-in-picture=(), publickey-credentials-get=(), usb=()";
	context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
	context.Response.Headers["X-Content-Type-Options"] = "nosniff";
	context.Response.Headers["X-Frame-Options"] = "DENY";
	context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
	context.Response.Headers["X-XSS-Protection"] = "0";
	await next();
});
app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
	OnPrepareResponse = context =>
	{
		context.Context.Response.Headers.CacheControl = "public,max-age=604800";
	}
});
app.UseRouting();
app.UseRateLimiter();
app.Use(async (context, next) =>
{
	if (!context.Request.Path.StartsWithSegments("/api/publishing/v1", StringComparison.OrdinalIgnoreCase) ||
		context.GetEndpoint() is null)
	{
		await next();
		return;
	}

	context.Response.Headers.CacheControl = "no-cache,no-store";
	var authentication = context.RequestServices.GetRequiredService<PublishingAuthentication>();
	var result = authentication.Authenticate(context.Request);
	if (result == PublishingAuthenticationResult.Accepted)
	{
		const long createReleaseRequestLimit = 64 * 1024;
		if (HttpMethods.IsPost(context.Request.Method) &&
			context.Request.Path.Equals("/api/publishing/v1/releases", StringComparison.OrdinalIgnoreCase))
		{
			if (context.Request.ContentLength > createReleaseRequestLimit)
			{
				context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
				return;
			}
			var requestSizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
			if (requestSizeFeature is { IsReadOnly: false })
			{
				requestSizeFeature.MaxRequestBodySize = createReleaseRequestLimit;
			}
		}
		await next();
		return;
	}
	if (result == PublishingAuthenticationResult.NotConfigured)
	{
		await Results.Problem("Publishing is not configured.", statusCode: StatusCodes.Status503ServiceUnavailable)
			.ExecuteAsync(context);
		return;
	}
	context.Response.Headers.WWWAuthenticate = "Bearer";
	context.Response.StatusCode = StatusCodes.Status401Unauthorized;
});

app.MapGet("/health/live", (HttpContext context) =>
{
	context.Response.Headers.CacheControl = "no-cache,no-store";
	return Results.Ok(new { status = "live" });
});
app.MapGet("/health/ready", (HttpContext context, ReleaseStore store) =>
{
	context.Response.Headers.CacheControl = "no-cache,no-store";
	return store.IsReady
		? Results.Ok(new { status = "ready" })
		: Results.Problem("Release storage is unavailable.", statusCode: StatusCodes.Status503ServiceUnavailable);
});

app.MapLegacyRedirects();
app.MapPublishingApi();
app.MapDownloadEndpoints();
app.MapSiteMetadata();
app.MapRazorPages();

app.Run();

public partial class Program;
