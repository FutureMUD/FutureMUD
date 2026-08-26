using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TerrainPlanner.Contracts;
using TerrainPlanner.Server.Authentication;
using TerrainPlanner.Server.Data;

namespace TerrainPlanner.Server.Endpoints;

public static class TerrainPlannerEndpoints
{
	public static IEndpointRouteBuilder MapTerrainPlannerEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/health/live", () => Results.Ok(new { status = "ok" }));
		endpoints.MapGet("/health/ready", async (ITerrainPlannerDataSource dataSource, CancellationToken cancellationToken) =>
			await dataSource.CanConnectAsync(cancellationToken)
				? Results.Ok(new { status = "ready" })
				: Results.StatusCode(StatusCodes.Status503ServiceUnavailable));

		var api = endpoints.MapGroup("/api/v1");
		api.MapGet("/auth/antiforgery", (HttpContext context, IAntiforgery antiforgery) =>
		{
			var tokens = antiforgery.GetAndStoreTokens(context);
			return Results.Ok(new AntiforgeryTokenResponse(tokens.RequestToken ?? string.Empty));
		}).AllowAnonymous();
		api.MapPost("/auth/login", LoginAsync)
			.AllowAnonymous()
			.RequireRateLimiting("login-ip");
		api.MapPost("/auth/logout", async (HttpContext context, IAntiforgery antiforgery) =>
		{
			if (!await HasValidAntiforgeryTokenAsync(context, antiforgery))
			{
				return Results.BadRequest();
			}

			await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Results.NoContent();
		}).RequireAuthorization();
		api.MapGet("/auth/session", GetSession).RequireAuthorization();
		api.MapGet("/terrains", async (HttpContext context, ITerrainPlannerDataSource dataSource,
			CancellationToken cancellationToken) =>
			CatalogueResult(context, await dataSource.GetTerrainsAsync(cancellationToken))).RequireAuthorization();
		api.MapGet("/tags", async (HttpContext context, ITerrainPlannerDataSource dataSource,
			CancellationToken cancellationToken) =>
			CatalogueResult(context, await dataSource.GetTagsAsync(cancellationToken))).RequireAuthorization();

		endpoints.MapGet("/Terrain", async (HttpContext context, ITerrainPlannerDataSource dataSource,
			CancellationToken cancellationToken) =>
		{
			context.Response.Headers["Deprecation"] = "true";
			context.Response.Headers.Append("Link", "</api/v1/terrains>; rel=\"successor-version\"");
			context.Response.Headers.Append("Sunset", "Terrain Planner 3.0.0");
			return CatalogueResult(context, await dataSource.GetTerrainsAsync(cancellationToken));
		}).RequireAuthorization();

		return endpoints;
	}

	private static async Task<IResult> LoginAsync(
		LoginRequest request,
		HttpContext context,
		IAccountAuthenticationService authenticationService,
		ILoginAttemptLimiter attemptLimiter,
		IAntiforgery antiforgery)
	{
		if (!await HasValidAntiforgeryTokenAsync(context, antiforgery))
		{
			return Results.BadRequest();
		}

		var address = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		if (!attemptLimiter.TryAcquire(address, request.AccountName))
		{
			return Results.Problem(statusCode: StatusCodes.Status429TooManyRequests,
				title: "Too many login attempts.");
		}

		var identity = await authenticationService.AuthenticateAsync(
			request.AccountName,
			request.Password,
			context.RequestAborted);
		if (identity is null)
		{
			return Results.Unauthorized();
		}

		attemptLimiter.Reset(address, request.AccountName);
		var now = DateTimeOffset.UtcNow;
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, identity.Id.ToString()),
			new Claim(ClaimTypes.Name, identity.Name),
			new Claim(ClaimTypes.Role, identity.AuthorityName),
			new Claim(PlannerCookieAuthenticationEvents.AuthorityLevelClaim, identity.AuthorityLevel.ToString()),
			new Claim(PlannerCookieAuthenticationEvents.IssuedAtClaim, now.ToUnixTimeSeconds().ToString())
		};
		var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
		await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
			new AuthenticationProperties
			{
				IsPersistent = false,
				IssuedUtc = now,
				ExpiresUtc = now + PlannerCookieAuthenticationEvents.AbsoluteLifetime,
				AllowRefresh = true
			});
		return Results.Ok(new AuthSession(true, identity.Id, identity.Name, identity.AuthorityName,
			identity.AuthorityLevel, now + PlannerCookieAuthenticationEvents.AbsoluteLifetime));
	}

	private static IResult GetSession(HttpContext context)
	{
		var principal = context.User;
		var accountId = long.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
		var authorityLevel = int.Parse(principal.FindFirstValue(PlannerCookieAuthenticationEvents.AuthorityLevelClaim)!);
		var issuedAt = DateTimeOffset.FromUnixTimeSeconds(
			long.Parse(principal.FindFirstValue(PlannerCookieAuthenticationEvents.IssuedAtClaim)!));
		return Results.Ok(new AuthSession(
			true,
			accountId,
			principal.Identity?.Name,
			principal.FindFirstValue(ClaimTypes.Role),
			authorityLevel,
			issuedAt + PlannerCookieAuthenticationEvents.AbsoluteLifetime));
	}

	private static IResult CatalogueResult<T>(HttpContext context, CatalogueResult<T> result)
	{
		context.Response.Headers.ETag = result.Revision;
		context.Response.Headers.CacheControl = "private, no-cache";
		if (context.Request.Headers.IfNoneMatch.Any(value => string.Equals(value, result.Revision, StringComparison.Ordinal)))
		{
			return Results.StatusCode(StatusCodes.Status304NotModified);
		}

		return Results.Ok(result.Items);
	}

	private static async Task<bool> HasValidAntiforgeryTokenAsync(HttpContext context, IAntiforgery antiforgery)
	{
		try
		{
			await antiforgery.ValidateRequestAsync(context);
			return true;
		}
		catch (AntiforgeryValidationException)
		{
			return false;
		}
	}
}
