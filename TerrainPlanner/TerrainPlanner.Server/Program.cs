using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging.EventLog;
using MudSharp.Database;
using System.Net;
using System.Runtime.Versioning;
using System.Threading.RateLimiting;
using TerrainPlanner.Server.Authentication;
using TerrainPlanner.Server.Data;
using TerrainPlanner.Server.Endpoints;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	Args = args,
	ContentRootPath = AppContext.BaseDirectory
});

var runAsWindowsService = OperatingSystem.IsWindows() &&
	args.Contains("--windows-service", StringComparer.OrdinalIgnoreCase);
if (runAsWindowsService && OperatingSystem.IsWindows())
{
	ConfigureWindowsService(builder);
}
else if (OperatingSystem.IsWindows())
{
	builder.Logging.AddFilter<EventLogLoggerProvider>(_ => false);
}

builder.Services.AddDbContextFactory<FuturemudDatabaseContext>((services, options) =>
{
	var configuration = services.GetRequiredService<IConfiguration>();
	var connectionString = configuration.GetConnectionString("Database") ??
		throw new InvalidOperationException("ConnectionStrings:Database must be configured.");
	options.UseMySql(connectionString,
		new MySqlServerVersion(Version.Parse(configuration["TerrainPlanner:MySqlVersion"] ?? "8.0.0")));
});
builder.Services.AddScoped<ITerrainPlannerDataSource, EfTerrainPlannerDataSource>();
builder.Services.AddScoped<IAccountAuthenticationService, AccountAuthenticationService>();
builder.Services.AddSingleton<ILoginAttemptLimiter, LoginAttemptLimiter>();
builder.Services.AddScoped<PlannerCookieAuthenticationEvents>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.Name = "__Host-FutureMUDTerrainPlanner";
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.Cookie.SameSite = SameSiteMode.Strict;
		options.Cookie.Path = "/";
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
		options.SlidingExpiration = true;
		options.EventsType = typeof(PlannerCookieAuthenticationEvents);
	});
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddAntiforgery(options =>
{
	options.HeaderName = "X-XSRF-TOKEN";
	options.Cookie.Name = "__Host-FutureMUDTerrainPlanner-XSRF";
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Strict;
	options.Cookie.Path = "/";
});

var keysPath = builder.Configuration["TerrainPlanner:DataProtectionKeysPath"];
if (!string.IsNullOrWhiteSpace(keysPath))
{
	builder.Services.AddDataProtection()
		.PersistKeysToFileSystem(new DirectoryInfo(keysPath))
		.SetApplicationName("FutureMUD.TerrainPlanner");
}

builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.AddPolicy("login-ip", context => RateLimitPartition.GetFixedWindowLimiter(
		context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
		_ => new FixedWindowRateLimiterOptions
		{
			PermitLimit = 30,
			Window = TimeSpan.FromMinutes(1),
			QueueLimit = 0,
			AutoReplenishment = true
		}));
});

var app = builder.Build();

var forwardedHeaders = new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
	ForwardLimit = 1
};
forwardedHeaders.KnownProxies.Add(IPAddress.Loopback);
forwardedHeaders.KnownProxies.Add(IPAddress.IPv6Loopback);
app.UseForwardedHeaders(forwardedHeaders);

if (app.Environment.IsDevelopment())
{
}
else
{
	app.UseExceptionHandler();
	app.UseHsts();
}

app.Use(async (context, next) =>
{
	context.Response.Headers.XContentTypeOptions = "nosniff";
	context.Response.Headers["Referrer-Policy"] = "no-referrer";
	context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=(), payment=(), usb=()");
	context.Response.Headers.Append("Content-Security-Policy",
		"default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; connect-src 'self'; img-src 'self' data:; font-src 'self'; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'self'");
	await next();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseAntiforgery();

app.MapTerrainPlannerEndpoints();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();

[SupportedOSPlatform("windows")]
static void ConfigureWindowsService(WebApplicationBuilder builder)
{
	builder.Host.UseWindowsService(options => options.ServiceName = "FutureMUDTerrainPlanner");
	builder.Services.Configure<EventLogSettings>(ConfigureEventLogSettings);
}

[SupportedOSPlatform("windows")]
#pragma warning disable CA1416
static void ConfigureEventLogSettings(EventLogSettings options) =>
	options.SourceName = "FutureMUD Terrain Planner";
#pragma warning restore CA1416

public partial class Program;
