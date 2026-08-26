using MudSharp.Accounts;
using MudSharp.Framework;
using TerrainPlanner.Server.Data;

namespace TerrainPlanner.Server.Authentication;

public interface IAccountAuthenticationService
{
	Task<PlannerAccountIdentity?> AuthenticateAsync(string accountName, string password,
		CancellationToken cancellationToken);
	Task<PlannerAccountIdentity?> ValidateAsync(long accountId, CancellationToken cancellationToken);
}

public sealed record PlannerAccountIdentity(long Id, string Name, string AuthorityName, int AuthorityLevel);

public sealed class AccountAuthenticationService : IAccountAuthenticationService
{
	private readonly ITerrainPlannerDataSource _dataSource;

	public AccountAuthenticationService(ITerrainPlannerDataSource dataSource)
	{
		_dataSource = dataSource;
	}

	public async Task<PlannerAccountIdentity?> AuthenticateAsync(string accountName, string password,
		CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrEmpty(password))
		{
			return null;
		}

		var account = await _dataSource.FindAccountByNameAsync(accountName.Trim().ToLowerInvariant(), cancellationToken);
		return account is not null && SecurityUtilities.VerifyPassword(password, account.PasswordHash, account.Salt)
			? ToIdentity(account)
			: null;
	}

	public async Task<PlannerAccountIdentity?> ValidateAsync(long accountId, CancellationToken cancellationToken)
	{
		var account = await _dataSource.FindAccountByIdAsync(accountId, cancellationToken);
		return account is null ? null : ToIdentity(account);
	}

	private static PlannerAccountIdentity? ToIdentity(PlannerAccountRecord account)
	{
		if (!account.IsRegistered || account.AccessStatus != (int)AccountStatus.Normal ||
			account.AuthorityLevel is null or < (int)PermissionLevel.Admin)
		{
			return null;
		}

		return new PlannerAccountIdentity(
			account.Id,
			account.Name,
			account.AuthorityName ?? "Administrator",
			account.AuthorityLevel.Value);
	}
}
