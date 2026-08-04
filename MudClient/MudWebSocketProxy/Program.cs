using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudWebSocketProxy.Handlers;
using MudWebSocketProxy.Security;

namespace MudWebSocketProxy;


public class Program
{
	public static void Main(string[] args)
	{
		var settingsPath = GetSettingsPath(args);
		var builder = WebApplication.CreateBuilder(new WebApplicationOptions
		{
			Args = args,
			ContentRootPath = AppContext.BaseDirectory
		});

		// Add WebSocket support
		builder.Services.AddWebSockets(options =>
		{
			options.KeepAliveInterval = TimeSpan.FromSeconds(120);
		});

		// Add configuration support
		builder.Configuration
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
		if (settingsPath is not null)
		{
			builder.Configuration.AddJsonFile(settingsPath, optional: false, reloadOnChange: true);
		}
		builder.Configuration
			.AddEnvironmentVariables()
			.AddCommandLine(args);
		ProxyConfigurationValidator.Validate(builder.Configuration);

		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(policy =>
			{
				policy.SetIsOriginAllowed(origin => IsConfiguredOriginAllowed(origin, builder.Configuration))
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials();
			});
		});

		builder.Services.AddTransient<WebSocketHandler>();
		builder.Services.AddSingleton(ProxyLimits.FromConfiguration(builder.Configuration));
		builder.Services.AddSingleton<ProxyConnectionLimiter>();

		builder.Logging.ClearProviders();
		builder.Logging.AddConsole();

		var app = builder.Build();
		var forwardedHeadersOptions = new ForwardedHeadersOptions
		{
			ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
			ForwardLimit = 1
		};
		forwardedHeadersOptions.KnownProxies.Add(IPAddress.Loopback);
		forwardedHeadersOptions.KnownProxies.Add(IPAddress.IPv6Loopback);
		app.UseForwardedHeaders(forwardedHeadersOptions);
		app.UseCors();
		app.UseWebSockets();

		var webSocketPath = builder.Configuration["WebSocketServer:Path"] ?? "/ws";

		app.Use(async (context, next) =>
		{
			if (context.Request.Path == webSocketPath && context.WebSockets.IsWebSocketRequest)
			{
				if (!IsRequestOriginAllowed(context, builder.Configuration))
				{
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					await context.Response.WriteAsync("WebSocket origin is not allowed by WebSocketServer:AllowedOrigins.");
					return;
				}

				var connectionLimiter = context.RequestServices.GetRequiredService<ProxyConnectionLimiter>();
				var clientAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
				using var connectionLease = connectionLimiter.TryAcquire(clientAddress);
				if (connectionLease == null)
				{
					context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
					await context.Response.WriteAsync("Too many active WebSocket connections.");
					return;
				}

				var webSocket = await context.WebSockets.AcceptWebSocketAsync();
				var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
				await webSocketHandler.HandleWebSocketAsync(context, webSocket);
			}
			else if (context.Request.Path == webSocketPath)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsync($"WebSocket proxy endpoint '{webSocketPath}' requires a WebSocket upgrade request.");
			}
			else
			{
				await next();
			}
		});

		app.MapGet("/health", () => Results.Ok(new
		{
			Status = "ok"
		}));

		app.Run();
	}

	private static bool IsRequestOriginAllowed(HttpContext context, IConfiguration configuration)
	{
		var origin = context.Request.Headers.Origin.ToString();
		var requireOrigin = configuration.GetValue("WebSocketServer:RequireOrigin", true);
		return WebSocketOriginPolicy.IsAllowed(
			origin,
			requireOrigin,
			configuration
				.GetSection("WebSocketServer:AllowedOrigins")
				.GetChildren()
				.Select(section => section.Value));
	}

	private static bool IsConfiguredOriginAllowed(string origin, IConfiguration configuration)
	{
		return WebSocketOriginPolicy.IsAllowed(
			origin,
			true,
			configuration
				.GetSection("WebSocketServer:AllowedOrigins")
				.GetChildren()
				.Select(section => section.Value));
	}

	private static string? GetSettingsPath(IReadOnlyList<string> args)
	{
		string? path = null;
		for (var index = 0; index < args.Count; index++)
		{
			if (!string.Equals(args[index], "--settings", StringComparison.Ordinal))
			{
				continue;
			}
			if (path is not null || index + 1 >= args.Count || !Path.IsPathFullyQualified(args[index + 1]))
			{
				throw new InvalidOperationException("--settings must occur once and use an absolute path.");
			}
			path = args[++index];
		}
		return path;
	}
}
