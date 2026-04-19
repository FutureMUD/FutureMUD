#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Computers;

public sealed class ComputerNetworkIdentityService : IComputerNetworkIdentityService
{
	private sealed class RuntimeNetworkAccount : IComputerNetworkAccount, IComputerMailAccount
	{
		public long Id { get; init; }
		public long DomainId { get; init; }
		public string UserName { get; init; } = string.Empty;
		public string DomainName { get; init; } = string.Empty;
		public string Address => $"{UserName}@{DomainName}";
		public bool Enabled { get; init; }
	}

	private readonly IFuturemud _gameworld;

	public ComputerNetworkIdentityService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public IEnumerable<ComputerNetworkDomainInfo> GetHostedDomains(IComputerHost host)
	{
		if (host.OwnerHostItemId is not > 0)
		{
			return Enumerable.Empty<ComputerNetworkDomainInfo>();
		}

		using (new FMDB())
		{
			return FMDB.Context.ComputerMailDomains
				.AsNoTracking()
				.Where(x => x.HostItemId == host.OwnerHostItemId.Value)
				.OrderBy(x => x.DomainName)
				.Select(x => new ComputerNetworkDomainInfo
				{
					Id = x.Id,
					DomainName = x.DomainName,
					HostItemId = x.HostItemId,
					Enabled = x.Enabled
				})
				.ToList();
		}
	}

	public bool RegisterDomain(IComputerHost host, string domainName, out string error)
	{
		error = string.Empty;
		if (host.OwnerHostItemId is not > 0)
		{
			error = "That host does not have a persistent item identity.";
			return false;
		}

		if (!TryNormaliseDomain(domainName, out var normalisedDomain, out error))
		{
			return false;
		}

		using (new FMDB())
		{
			if (FMDB.Context.ComputerMailDomains.Any(x => x.DomainName == normalisedDomain))
			{
				error = $"The domain {normalisedDomain.ColourName()} is already registered.";
				return false;
			}

			FMDB.Context.ComputerMailDomains.Add(new ComputerMailDomain
			{
				DomainName = normalisedDomain,
				HostItemId = host.OwnerHostItemId.Value,
				Enabled = true,
				CreatedAtUtc = DateTime.UtcNow
			});
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public bool RemoveDomain(IComputerHost host, string domainName, out string error)
	{
		error = string.Empty;
		if (host.OwnerHostItemId is not > 0)
		{
			error = "That host does not have a persistent item identity.";
			return false;
		}

		if (!TryNormaliseDomain(domainName, out var normalisedDomain, out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var domain = FMDB.Context.ComputerMailDomains
				.Include(x => x.Accounts)
				.ThenInclude(x => x.MailboxEntries)
				.FirstOrDefault(x => x.HostItemId == host.OwnerHostItemId.Value && x.DomainName == normalisedDomain);
			if (domain is null)
			{
				error = $"{host.Name.ColourName()} does not host the domain {normalisedDomain.ColourName()}.";
				return false;
			}

			var messageIds = domain.Accounts
				.SelectMany(x => x.MailboxEntries)
				.Select(x => x.ComputerMailMessageId)
				.Distinct()
				.ToList();

			FMDB.Context.ComputerMailDomains.Remove(domain);
			FMDB.Context.SaveChanges();
			DeleteOrphanMessages(FMDB.Context, messageIds);
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public bool SetDomainEnabled(IComputerHost host, string domainName, bool enabled, out string error)
	{
		error = string.Empty;
		if (host.OwnerHostItemId is not > 0)
		{
			error = "That host does not have a persistent item identity.";
			return false;
		}

		if (!TryNormaliseDomain(domainName, out var normalisedDomain, out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var domain = FMDB.Context.ComputerMailDomains
				.FirstOrDefault(x => x.HostItemId == host.OwnerHostItemId.Value && x.DomainName == normalisedDomain);
			if (domain is null)
			{
				error = $"{host.Name.ColourName()} does not host the domain {normalisedDomain.ColourName()}.";
				return false;
			}

			domain.Enabled = enabled;
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public IEnumerable<IComputerNetworkAccount> GetAccounts(IComputerHost host, string? domainName = null)
	{
		if (host.OwnerHostItemId is not > 0)
		{
			return Enumerable.Empty<IComputerNetworkAccount>();
		}

		using (new FMDB())
		{
			var query = FMDB.Context.ComputerMailAccounts
				.AsNoTracking()
				.Include(x => x.ComputerMailDomain)
				.Where(x => x.ComputerMailDomain.HostItemId == host.OwnerHostItemId.Value);

			if (!string.IsNullOrWhiteSpace(domainName))
			{
				if (!TryNormaliseDomain(domainName, out var normalisedDomain, out _))
				{
					return Enumerable.Empty<IComputerNetworkAccount>();
				}

				query = query.Where(x => x.ComputerMailDomain.DomainName == normalisedDomain);
			}

			return query
				.OrderBy(x => x.ComputerMailDomain.DomainName)
				.ThenBy(x => x.UserName)
				.Select(ToRuntimeAccount)
				.ToList();
		}
	}

	public bool CreateAccount(IComputerHost host, string address, string password, out string error)
	{
		error = string.Empty;
		if (host.OwnerHostItemId is not > 0)
		{
			error = "That host does not have a persistent item identity.";
			return false;
		}

		if (!TrySplitAddress(address, out var userName, out var domainName, out error))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			error = "You must supply a password.";
			return false;
		}

		using (new FMDB())
		{
			var domain = FMDB.Context.ComputerMailDomains
				.FirstOrDefault(x => x.HostItemId == host.OwnerHostItemId.Value && x.DomainName == domainName);
			if (domain is null)
			{
				error = $"{host.Name.ColourName()} does not host the domain {domainName.ColourName()}.";
				return false;
			}

			if (FMDB.Context.ComputerMailAccounts.Any(x =>
				    x.ComputerMailDomainId == domain.Id && x.UserName == userName))
			{
				error = $"The account {address.ColourName()} already exists.";
				return false;
			}

			var salt = SecurityUtilities.GetSalt64();
			FMDB.Context.ComputerMailAccounts.Add(new ComputerMailAccount
			{
				ComputerMailDomainId = domain.Id,
				UserName = userName,
				PasswordSalt = salt,
				PasswordHash = SecurityUtilities.GetPasswordHash(password, salt),
				IsEnabled = true,
				CreatedAtUtc = DateTime.UtcNow,
				LastModifiedAtUtc = DateTime.UtcNow
			});
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public bool SetAccountEnabled(IComputerHost host, string address, bool enabled, out string error)
	{
		error = string.Empty;
		if (host.OwnerHostItemId is not > 0)
		{
			error = "That host does not have a persistent item identity.";
			return false;
		}

		if (!TrySplitAddress(address, out var userName, out var domainName, out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var account = FMDB.Context.ComputerMailAccounts
				.Include(x => x.ComputerMailDomain)
				.FirstOrDefault(x =>
					x.ComputerMailDomain.HostItemId == host.OwnerHostItemId.Value &&
					x.ComputerMailDomain.DomainName == domainName &&
					x.UserName == userName);
			if (account is null)
			{
				error = $"{host.Name.ColourName()} does not host the account {address.ColourName()}.";
				return false;
			}

			account.IsEnabled = enabled;
			account.LastModifiedAtUtc = DateTime.UtcNow;
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public bool SetAccountPassword(IComputerHost host, string address, string password, out string error)
	{
		error = string.Empty;
		if (host.OwnerHostItemId is not > 0)
		{
			error = "That host does not have a persistent item identity.";
			return false;
		}

		if (!TrySplitAddress(address, out var userName, out var domainName, out error))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			error = "You must supply a password.";
			return false;
		}

		using (new FMDB())
		{
			var account = FMDB.Context.ComputerMailAccounts
				.Include(x => x.ComputerMailDomain)
				.FirstOrDefault(x =>
					x.ComputerMailDomain.HostItemId == host.OwnerHostItemId.Value &&
					x.ComputerMailDomain.DomainName == domainName &&
					x.UserName == userName);
			if (account is null)
			{
				error = $"{host.Name.ColourName()} does not host the account {address.ColourName()}.";
				return false;
			}

			var salt = SecurityUtilities.GetSalt64();
			account.PasswordSalt = salt;
			account.PasswordHash = SecurityUtilities.GetPasswordHash(password, salt);
			account.LastModifiedAtUtc = DateTime.UtcNow;
			FMDB.Context.SaveChanges();
		}

		return true;
	}

	public ComputerNetworkAuthenticationResult Authenticate(IComputerHost sourceHost, string address, string password)
	{
		if (!TrySplitAddress(address, out var userName, out var domainName, out var error))
		{
			return new ComputerNetworkAuthenticationResult { Success = false, ErrorMessage = error };
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			return new ComputerNetworkAuthenticationResult
			{
				Success = false,
				ErrorMessage = "You must supply a password."
			};
		}

		if (!TryResolveHostedDomain(sourceHost, domainName, out _, out var domain, out error))
		{
			return new ComputerNetworkAuthenticationResult { Success = false, ErrorMessage = error };
		}

		using (new FMDB())
		{
			var account = FMDB.Context.ComputerMailAccounts
				.AsNoTracking()
				.Include(x => x.ComputerMailDomain)
				.FirstOrDefault(x => x.ComputerMailDomainId == domain.Id && x.UserName == userName);
			if (account is null || !account.IsEnabled)
			{
				return new ComputerNetworkAuthenticationResult
				{
					Success = false,
					ErrorMessage = $"The account {address.ColourName()} is not available."
				};
			}

			var hash = SecurityUtilities.GetPasswordHash(password, account.PasswordSalt);
			if (!string.Equals(hash, account.PasswordHash, StringComparison.InvariantCulture))
			{
				return new ComputerNetworkAuthenticationResult
				{
					Success = false,
					ErrorMessage = "That password is not correct."
				};
			}

			return new ComputerNetworkAuthenticationResult
			{
				Success = true,
				Account = ToRuntimeAccount(account)
			};
		}
	}

	public IComputerNetworkAccount? GetAccount(IComputerHost sourceHost, long accountId, out string error)
	{
		using (new FMDB())
		{
			var account = FMDB.Context.ComputerMailAccounts
				.AsNoTracking()
				.Include(x => x.ComputerMailDomain)
				.FirstOrDefault(x => x.Id == accountId);
			if (account is null || !account.IsEnabled)
			{
				error = "That network account is not available.";
				return null;
			}

			if (!account.ComputerMailDomain.Enabled)
			{
				error = $"The domain {account.ComputerMailDomain.DomainName.ColourName()} is not currently available.";
				return null;
			}

			var targetHost = ResolveHost(sourceHost, account.ComputerMailDomain.HostItemId);
			if (targetHost is null)
			{
				error = $"The host serving {account.ComputerMailDomain.DomainName.ColourName()} is not currently available.";
				return null;
			}

			if (!CanReachHost(sourceHost, targetHost))
			{
				error =
					$"The host serving {account.ComputerMailDomain.DomainName.ColourName()} is not reachable from {sourceHost.Name.ColourName()}.";
				return null;
			}

			error = string.Empty;
			return ToRuntimeAccount(account);
		}
	}

	public IEnumerable<string> GetAdvertisedDomainDetails(IComputerHost host)
	{
		return GetHostedDomains(host)
			.Where(x => x.Enabled)
			.Select(x => x.DomainName)
			.OrderBy(x => x)
			.ToList();
	}

	private static void DeleteOrphanMessages(FuturemudDatabaseContext context, IEnumerable<long> messageIds)
	{
		var ids = messageIds.Distinct().ToList();
		if (!ids.Any())
		{
			return;
		}

		var orphanIds = ids
			.Where(id => !context.ComputerMailMailboxEntries.Any(x => x.ComputerMailMessageId == id))
			.ToList();
		if (!orphanIds.Any())
		{
			return;
		}

		var orphans = context.ComputerMailMessages
			.Where(x => orphanIds.Contains(x.Id))
			.ToList();
		context.ComputerMailMessages.RemoveRange(orphans);
	}

	private bool TryResolveHostedDomain(IComputerHost sourceHost, string domainName, out IComputerHost targetHost,
		out ComputerNetworkDomainInfo domain, out string error)
	{
		targetHost = default!;
		domain = default!;
		using (new FMDB())
		{
			var dbDomain = FMDB.Context.ComputerMailDomains
				.AsNoTracking()
				.FirstOrDefault(x => x.DomainName == domainName);
			if (dbDomain is null)
			{
				error = $"There is no registered network domain named {domainName.ColourName()}.";
				return false;
			}

			domain = new ComputerNetworkDomainInfo
			{
				Id = dbDomain.Id,
				DomainName = dbDomain.DomainName,
				HostItemId = dbDomain.HostItemId,
				Enabled = dbDomain.Enabled
			};
		}

		if (!domain.Enabled)
		{
			error = $"The domain {domain.DomainName.ColourName()} is not currently enabled.";
			return false;
		}

		targetHost = ResolveHost(sourceHost, domain.HostItemId)!;
		if (targetHost is null)
		{
			error = $"The host serving {domain.DomainName.ColourName()} is not currently available.";
			return false;
		}

		if (!CanReachHost(sourceHost, targetHost))
		{
			error = $"The host serving {domain.DomainName.ColourName()} is not reachable from {sourceHost.Name.ColourName()}.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private IComputerHost? ResolveHost(IComputerHost sourceHost, long hostItemId)
	{
		if (sourceHost.OwnerHostItemId == hostItemId)
		{
			return sourceHost;
		}

		var session = GetExecutionSession(sourceHost);
		return _gameworld.ComputerExecutionService.GetReachableHosts(sourceHost, session)
			.Select(x => x.Host)
			.FirstOrDefault(x => x.OwnerHostItemId == hostItemId);
	}

	private bool CanReachHost(IComputerHost sourceHost, IComputerHost targetHost)
	{
		if (ReferenceEquals(sourceHost, targetHost) || sourceHost.OwnerHostItemId == targetHost.OwnerHostItemId)
		{
			return true;
		}

		var session = GetExecutionSession(sourceHost);
		return _gameworld.ComputerExecutionService.GetReachableHosts(sourceHost, session)
			.Any(x => x.Host.OwnerHostItemId == targetHost.OwnerHostItemId);
	}

	private static IComputerTerminalSession? GetExecutionSession(IComputerHost sourceHost)
	{
		var session = ComputerExecutionContextScope.Current?.Session;
		return session is not null && ReferenceEquals(session.Host, sourceHost) ? session : null;
	}

	private static bool TrySplitAddress(string address, out string userName, out string domainName, out string error)
	{
		userName = string.Empty;
		domainName = string.Empty;
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(address))
		{
			error = "You must specify an address in the form user@domain.";
			return false;
		}

		var parts = address.Trim().ToLowerInvariant()
			.Split('@', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
		{
			error = "Addresses must be in the form user@domain.";
			return false;
		}

		userName = parts[0];
		domainName = parts[1];
		return true;
	}

	private static bool TryNormaliseDomain(string domainName, out string normalisedDomain, out string error)
	{
		normalisedDomain = domainName.Trim().ToLowerInvariant();
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(normalisedDomain) || !normalisedDomain.Contains('.'))
		{
			error = "Network domains must be non-empty and include at least one period.";
			return false;
		}

		return true;
	}

	private static RuntimeNetworkAccount ToRuntimeAccount(ComputerMailAccount account)
	{
		return new RuntimeNetworkAccount
		{
			Id = account.Id,
			DomainId = account.ComputerMailDomainId,
			UserName = account.UserName,
			DomainName = account.ComputerMailDomain.DomainName,
			Enabled = account.IsEnabled
		};
	}
}
