using MudSharp.Accounts;
using MudSharp.Framework;
using TerrainPlanner.Contracts;
using TerrainPlanner.Server.Authentication;
using TerrainPlanner.Server.Data;

namespace TerrainPlanner.Tests;

[TestClass]
public class AuthenticationTests
{
	[TestMethod]
	public async Task ExistingAdminAndHigherAccountsAuthenticate()
	{
		foreach (var level in new[] { PermissionLevel.Admin, PermissionLevel.HighAdmin, PermissionLevel.Founder })
		{
			var source = new FakeDataSource(CreateAccount(level));
			var service = new AccountAuthenticationService(source);

			var identity = await service.AuthenticateAsync(" Builder ", "legacy password", CancellationToken.None);

			Assert.IsNotNull(identity, $"Expected {level} to authenticate.");
			Assert.AreEqual("builder", source.LastNormalizedName);
		}
	}

	[TestMethod]
	public async Task JuniorAdminAndPlayersAreRejected()
	{
		foreach (var level in new[] { PermissionLevel.Player, PermissionLevel.Guide, PermissionLevel.JuniorAdmin })
		{
			var service = new AccountAuthenticationService(new FakeDataSource(CreateAccount(level)));
			Assert.IsNull(await service.AuthenticateAsync("builder", "legacy password", CancellationToken.None));
		}
	}

	[TestMethod]
	public async Task InvalidSuspendedAndUnregisteredAccountsAreRejected()
	{
		var valid = CreateAccount(PermissionLevel.Admin);
		var invalidPassword = new AccountAuthenticationService(new FakeDataSource(valid));
		Assert.IsNull(await invalidPassword.AuthenticateAsync("builder", "wrong", CancellationToken.None));

		var suspended = valid with { AccessStatus = (int)AccountStatus.Suspended };
		Assert.IsNull(await new AccountAuthenticationService(new FakeDataSource(suspended))
			.AuthenticateAsync("builder", "legacy password", CancellationToken.None));

		var unregistered = valid with { IsRegistered = false };
		Assert.IsNull(await new AccountAuthenticationService(new FakeDataSource(unregistered))
			.AuthenticateAsync("builder", "legacy password", CancellationToken.None));
	}

	private static PlannerAccountRecord CreateAccount(PermissionLevel level)
	{
		const long salt = 8675309;
		return new PlannerAccountRecord(1, "Builder", SecurityUtilities.GetPasswordHash("legacy password", salt), salt,
			true, (int)AccountStatus.Normal, 1, level.ToString(), (int)level);
	}

	private sealed class FakeDataSource(PlannerAccountRecord? account) : ITerrainPlannerDataSource
	{
		public string? LastNormalizedName { get; private set; }

		public Task<PlannerAccountRecord?> FindAccountByNameAsync(string normalizedName, CancellationToken cancellationToken)
		{
			LastNormalizedName = normalizedName;
			return Task.FromResult(account);
		}

		public Task<PlannerAccountRecord?> FindAccountByIdAsync(long accountId, CancellationToken cancellationToken) =>
			Task.FromResult(account);

		public Task<CatalogueResult<TerrainCatalogueItem>> GetTerrainsAsync(CancellationToken cancellationToken) =>
			throw new NotSupportedException();

		public Task<CatalogueResult<TagCatalogueItem>> GetTagsAsync(CancellationToken cancellationToken) =>
			throw new NotSupportedException();

		public Task<bool> CanConnectAsync(CancellationToken cancellationToken) => Task.FromResult(true);
	}
}
