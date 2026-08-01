using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudWebSocketProxy.Handlers;

namespace MudWebSocketProxy;


public class Program
{
	public static void Main(string[] args)
	{
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
			.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables()
			.AddCommandLine(args);

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

		builder.Logging.ClearProviders();
		builder.Logging.AddConsole();

		var app = builder.Build();
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
		return string.IsNullOrWhiteSpace(origin) || IsConfiguredOriginAllowed(origin, configuration);
	}

	private static bool IsConfiguredOriginAllowed(string origin, IConfiguration configuration)
	{
		var normalizedOrigin = NormalizeOrigin(origin);
		return configuration
			.GetSection("WebSocketServer:AllowedOrigins")
			.GetChildren()
			.Select(section => section.Value)
			.Where(allowedOrigin => !string.IsNullOrWhiteSpace(allowedOrigin))
			.Select(allowedOrigin => NormalizeOrigin(allowedOrigin!))
			.Any(allowedOrigin => string.Equals(allowedOrigin, normalizedOrigin, StringComparison.OrdinalIgnoreCase));
	}

	private static string NormalizeOrigin(string origin)
	{
		if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
		{
			return uri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
		}

		return origin.Trim().TrimEnd('/');
	}
}
