using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace TerrainPlanner.Server.Authentication;

public sealed class PlannerCookieAuthenticationEvents : CookieAuthenticationEvents
{
	public const string AuthorityLevelClaim = "futuremud:authority-level";
	public const string IssuedAtClaim = "futuremud:issued-at";
	public static readonly TimeSpan AbsoluteLifetime = TimeSpan.FromHours(8);
	private readonly IAccountAuthenticationService _authenticationService;

	public PlannerCookieAuthenticationEvents(IAccountAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
	{
		if (!long.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId) ||
			!long.TryParse(context.Principal?.FindFirstValue(IssuedAtClaim), out var issuedAtSeconds) ||
			DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(issuedAtSeconds) >= AbsoluteLifetime)
		{
			await RejectAsync(context);
			return;
		}

		var identity = await _authenticationService.ValidateAsync(accountId, context.HttpContext.RequestAborted);
		if (identity is null)
		{
			await RejectAsync(context);
			return;
		}

		var currentAuthority = context.Principal?.FindFirstValue(AuthorityLevelClaim);
		if (!string.Equals(currentAuthority, identity.AuthorityLevel.ToString(), StringComparison.Ordinal))
		{
			await RejectAsync(context);
		}
	}

	public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
	{
		context.Response.StatusCode = StatusCodes.Status401Unauthorized;
		return Task.CompletedTask;
	}

	public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
	{
		context.Response.StatusCode = StatusCodes.Status403Forbidden;
		return Task.CompletedTask;
	}

	private static async Task RejectAsync(CookieValidatePrincipalContext context)
	{
		context.RejectPrincipal();
		await context.HttpContext.SignOutAsync();
	}
}
