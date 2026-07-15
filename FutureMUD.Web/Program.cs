#nullable enable

using FutureMUD.Web.Configuration;
using FutureMUD.Web.Publishing;
using FutureMUD.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
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
	options.Limits.MaxRequestBodySize = ReleaseStore.ChunkSize + 1024;
});

builder.Services.AddRazorPages();
builder.Services.AddDataProtection()
	.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(configuredDataRoot, "keys")))
	.SetApplicationName("FutureMUD.Web");
builder.Services.AddResponseCompression();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
	options.KnownIPNetworks.Clear();
	options.KnownProxies.Clear();
	options.KnownProxies.Add(IPAddress.Loopback);
	options.KnownProxies.Add(IPAddress.IPv6Loopback);
});
builder.Services.Configure<FutureMudWebOptions>(builder.Configuration.GetSection(FutureMudWebOptions.SectionName));
builder.Services.AddSingleton<MarkdownContentService>();
builder.Services.AddSingleton<DocumentationService>();
builder.Services.AddSingleton<ReleaseProductCatalogue>();
builder.Services.AddSingleton<ReleaseStore>();
builder.Services.AddSingleton<PublishingAuthentication>();
builder.Services.AddHostedService<ReleaseCleanupService>();
builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.AddFixedWindowLimiter("publishing", limiter =>
	{
		limiter.PermitLimit = 120;
		limiter.Window = TimeSpan.FromMinutes(1);
		limiter.QueueLimit = 0;
		limiter.AutoReplenishment = true;
	});
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
	context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; img-src 'self' data:; style-src 'self'; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'self'";
	context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
	context.Response.Headers["X-Content-Type-Options"] = "nosniff";
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
