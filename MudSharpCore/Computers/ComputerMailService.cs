#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Computers;

public sealed class ComputerMailService : IComputerMailService
{
	private sealed class RuntimeMailAccount : IComputerMailAccount
	{
		public long Id { get; init; }
		public long DomainId { get; init; }
		public string UserName { get; init; } = string.Empty;
		public string DomainName { get; init; } = string.Empty;
		public string Address => $"{UserName}@{DomainName}";
		public bool Enabled { get; init; }
	}

	private readonly IFuturemud _gameworld;

	public ComputerMailService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public bool IsMailServiceEnabled(IComputerHost host)
	{
		return host.IsNetworkServiceEnabled("mail");
	}

	public bool SetMailServiceEnabled(IComputerHost host, bool enabled, out string error)
	{
		return host.SetNetworkServiceEnabled("mail", enabled, out error);
	}

	public IEnumerable<ComputerMailDomainInfo> GetHostedDomains(IComputerHost host)
	{
		if (host.OwnerHostItemId is not > 0)
		{
			return Enumerable.Empty<ComputerMailDomainInfo>();
		}

		using (new FMDB())
		{
			return FMDB.Context.ComputerMailDomains
				.AsNoTracking()
				.Where(x => x.HostItemId == host.OwnerHostItemId.Value)
				.OrderBy(x => x.DomainName)
				.Select(x => new ComputerMailDomainInfo
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

	public IEnumerable<IComputerMailAccount> GetAccounts(IComputerHost host, string? domainName = null)
	{
		if (host.OwnerHostItemId is not > 0)
		{
			return Enumerable.Empty<IComputerMailAccount>();
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
					return Enumerable.Empty<IComputerMailAccount>();
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

	public ComputerMailAuthenticationResult Authenticate(IComputerHost sourceHost, string address, string password)
	{
		if (!TrySplitAddress(address, out var userName, out var domainName, out var error))
		{
			return new ComputerMailAuthenticationResult { Success = false, ErrorMessage = error };
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			return new ComputerMailAuthenticationResult
			{
				Success = false,
				ErrorMessage = "You must supply a password."
			};
		}

		if (!TryResolveHostedDomain(sourceHost, domainName, out var targetHost, out var domain, out error))
		{
			return new ComputerMailAuthenticationResult { Success = false, ErrorMessage = error };
		}

		using (new FMDB())
		{
			var account = FMDB.Context.ComputerMailAccounts
				.AsNoTracking()
				.FirstOrDefault(x => x.ComputerMailDomainId == domain.Id && x.UserName == userName);
			if (account is null || !account.IsEnabled)
			{
				return new ComputerMailAuthenticationResult
				{
					Success = false,
					ErrorMessage = $"There is no enabled mail account named {address.ColourName()}."
				};
			}

			if (!SecurityUtilities.VerifyPassword(password, account.PasswordHash, account.PasswordSalt))
			{
				return new ComputerMailAuthenticationResult
				{
					Success = false,
					ErrorMessage = "That password is not correct."
				};
			}

			return new ComputerMailAuthenticationResult
			{
				Success = true,
				Account = new RuntimeMailAccount
				{
					Id = account.Id,
					DomainId = domain.Id,
					UserName = account.UserName,
					DomainName = domain.DomainName,
					Enabled = account.IsEnabled
				}
			};
		}
	}

	public IComputerMailAccount? GetAccount(IComputerHost sourceHost, long accountId, out string error)
	{
		using (new FMDB())
		{
			var account = FMDB.Context.ComputerMailAccounts
				.AsNoTracking()
				.Include(x => x.ComputerMailDomain)
				.FirstOrDefault(x => x.Id == accountId);
			if (account is null || !account.IsEnabled)
			{
				error = "That mail account is no longer available.";
				return null;
			}

			var domain = new ComputerMailDomainInfo
			{
				Id = account.ComputerMailDomain.Id,
				DomainName = account.ComputerMailDomain.DomainName,
				HostItemId = account.ComputerMailDomain.HostItemId,
				Enabled = account.ComputerMailDomain.Enabled
			};

			if (!domain.Enabled)
			{
				error = $"The domain {domain.DomainName.ColourName()} is not currently accepting mail.";
				return null;
			}

			var targetHost = ResolveHost(sourceHost, domain.HostItemId);
			if (targetHost is null || !targetHost.IsNetworkServiceEnabled("mail"))
			{
				error = $"The mail service for {domain.DomainName.ColourName()} is not currently available.";
				return null;
			}

			if (!CanReachHost(sourceHost, targetHost))
			{
				error = $"The mail service for {domain.DomainName.ColourName()} is not reachable from {sourceHost.Name.ColourName()}.";
				return null;
			}

			error = string.Empty;
			return new RuntimeMailAccount
			{
				Id = account.Id,
				DomainId = account.ComputerMailDomainId,
				UserName = account.UserName,
				DomainName = account.ComputerMailDomain.DomainName,
				Enabled = account.IsEnabled
			};
		}
	}

	public IEnumerable<ComputerMailMessageHeader> GetMailboxHeaders(IComputerHost sourceHost, IComputerMailAccount account,
		bool includeDeleted = false)
	{
		if (!TryEnsureReachableAccount(sourceHost, account, out _, out _))
		{
			return Enumerable.Empty<ComputerMailMessageHeader>();
		}

		using (new FMDB())
		{
			var query = FMDB.Context.ComputerMailMailboxEntries
				.AsNoTracking()
				.Include(x => x.ComputerMailMessage)
				.Where(x => x.ComputerMailAccountId == account.Id);
			if (!includeDeleted)
			{
				query = query.Where(x => !x.IsDeleted);
			}

			return query
				.OrderByDescending(x => x.DeliveredAtUtc)
				.ThenByDescending(x => x.Id)
				.Select(ToHeader)
				.ToList();
		}
	}

	public ComputerMailMessageDetails? ReadMessage(IComputerHost sourceHost, IComputerMailAccount account, long mailboxEntryId,
		out string error)
	{
		if (!TryEnsureReachableAccount(sourceHost, account, out var domain, out error))
		{
			return null;
		}

		using (new FMDB())
		{
			var entry = FMDB.Context.ComputerMailMailboxEntries
				.Include(x => x.ComputerMailMessage)
				.Include(x => x.ComputerMailAccount)
				.ThenInclude(x => x.ComputerMailDomain)
				.FirstOrDefault(x => x.Id == mailboxEntryId && x.ComputerMailAccountId == account.Id);
			if (entry is null)
			{
				error = $"There is no such message in {account.Address.ColourName()}.";
				return null;
			}

			if (entry.ComputerMailAccount.ComputerMailDomainId != domain.Id)
			{
				error = "That message does not belong to the currently authenticated mailbox.";
				return null;
			}

			if (!entry.IsRead)
			{
				entry.IsRead = true;
				FMDB.Context.SaveChanges();
			}

			error = string.Empty;
			return new ComputerMailMessageDetails
			{
				Header = ToHeader(entry),
				Body = entry.ComputerMailMessage.Body
			};
		}
	}

	public bool DeleteMessage(IComputerHost sourceHost, IComputerMailAccount account, long mailboxEntryId, out string error)
	{
		if (!TryEnsureReachableAccount(sourceHost, account, out var domain, out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var entry = FMDB.Context.ComputerMailMailboxEntries
				.Include(x => x.ComputerMailAccount)
				.FirstOrDefault(x => x.Id == mailboxEntryId && x.ComputerMailAccountId == account.Id);
			if (entry is null)
			{
				error = $"There is no such message in {account.Address.ColourName()}.";
				return false;
			}

			if (entry.ComputerMailAccount.ComputerMailDomainId != domain.Id)
			{
				error = "That message does not belong to the currently authenticated mailbox.";
				return false;
			}

			entry.IsDeleted = true;
			FMDB.Context.SaveChanges();
		}

		error = string.Empty;
		return true;
	}

	public bool SendMessage(IComputerHost sourceHost, IComputerMailAccount senderAccount, string recipientAddress,
		string subject, string body, out string error)
	{
		if (!TryEnsureReachableAccount(sourceHost, senderAccount, out _, out error))
		{
			return false;
		}

		if (!TrySplitAddress(recipientAddress, out _, out var recipientDomain, out error))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(subject))
		{
			error = "You must set a subject before posting the message.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(body))
		{
			error = "You must set a body before posting the message.";
			return false;
		}

		if (!TryResolveHostedDomain(sourceHost, recipientDomain, out _, out var domain, out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var recipientAccount = FMDB.Context.ComputerMailAccounts
				.FirstOrDefault(x =>
					x.ComputerMailDomainId == domain.Id &&
					$"{x.UserName}@{domain.DomainName}" == recipientAddress.ToLowerInvariant());
			if (recipientAccount is null || !recipientAccount.IsEnabled)
			{
				error = $"There is no enabled mailbox for {recipientAddress.ColourName()}.";
				return false;
			}

			var message = new ComputerMailMessage
			{
				SenderAddress = senderAccount.Address,
				RecipientAddress = recipientAddress.ToLowerInvariant(),
				Subject = subject.Trim(),
				Body = body,
				SentAtUtc = DateTime.UtcNow
			};
			FMDB.Context.ComputerMailMessages.Add(message);
			FMDB.Context.SaveChanges();

			FMDB.Context.ComputerMailMailboxEntries.Add(new ComputerMailMailboxEntry
			{
				ComputerMailAccountId = recipientAccount.Id,
				ComputerMailMessageId = message.Id,
				IsSentFolder = false,
				IsRead = false,
				IsDeleted = false,
				DeliveredAtUtc = message.SentAtUtc
			});

			FMDB.Context.ComputerMailMailboxEntries.Add(new ComputerMailMailboxEntry
			{
				ComputerMailAccountId = senderAccount.Id,
				ComputerMailMessageId = message.Id,
				IsSentFolder = true,
				IsRead = true,
				IsDeleted = false,
				DeliveredAtUtc = message.SentAtUtc
			});

			FMDB.Context.SaveChanges();
		}

		error = string.Empty;
		return true;
	}

	public IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId)
	{
		if (!applicationId.EqualTo("mail") || !host.IsNetworkServiceEnabled("mail"))
		{
			return Enumerable.Empty<string>();
		}

		return GetHostedDomains(host)
			.Where(x => x.Enabled)
			.Select(x => x.DomainName)
			.OrderBy(x => x)
			.ToList();
	}

	private bool TryEnsureReachableAccount(IComputerHost sourceHost, IComputerMailAccount account,
		out ComputerMailDomainInfo domain, out string error)
	{
		domain = default!;
		using (new FMDB())
		{
			var dbAccount = FMDB.Context.ComputerMailAccounts
				.AsNoTracking()
				.Include(x => x.ComputerMailDomain)
				.FirstOrDefault(x => x.Id == account.Id);
			if (dbAccount is null || !dbAccount.IsEnabled)
			{
				error = $"The mail account {account.Address.ColourName()} is not available.";
				return false;
			}

			domain = new ComputerMailDomainInfo
			{
				Id = dbAccount.ComputerMailDomain.Id,
				DomainName = dbAccount.ComputerMailDomain.DomainName,
				HostItemId = dbAccount.ComputerMailDomain.HostItemId,
				Enabled = dbAccount.ComputerMailDomain.Enabled
			};
		}

		if (!domain.Enabled)
		{
			error = $"The domain {domain.DomainName.ColourName()} is not currently accepting mail.";
			return false;
		}

		var targetHost = ResolveHost(sourceHost, domain.HostItemId);
		if (targetHost is null || !targetHost.IsNetworkServiceEnabled("mail"))
		{
			error = $"The mail service for {domain.DomainName.ColourName()} is not currently available.";
			return false;
		}

		if (!CanReachHost(sourceHost, targetHost))
		{
			error = $"The mail service for {domain.DomainName.ColourName()} is not reachable from {sourceHost.Name.ColourName()}.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private bool TryResolveHostedDomain(IComputerHost sourceHost, string domainName, out IComputerHost targetHost,
		out ComputerMailDomainInfo domain, out string error)
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
				error = $"There is no registered mail domain named {domainName.ColourName()}.";
				return false;
			}

			domain = new ComputerMailDomainInfo
			{
				Id = dbDomain.Id,
				DomainName = dbDomain.DomainName,
				HostItemId = dbDomain.HostItemId,
				Enabled = dbDomain.Enabled
			};
		}

		if (!domain.Enabled)
		{
			error = $"The domain {domain.DomainName.ColourName()} is not currently accepting mail.";
			return false;
		}

		targetHost = ResolveHost(sourceHost, domain.HostItemId)!;
		if (targetHost is null)
		{
			error = $"The host serving {domain.DomainName.ColourName()} is not currently available.";
			return false;
		}

		if (!targetHost.IsNetworkServiceEnabled("mail"))
		{
			error = $"The host serving {domain.DomainName.ColourName()} is not currently advertising mail.";
			return false;
		}

		if (!CanReachHost(sourceHost, targetHost))
		{
			error = $"The mail service for {domain.DomainName.ColourName()} is not reachable from {sourceHost.Name.ColourName()}.";
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

		return _gameworld.ComputerExecutionService.GetReachableHosts(sourceHost)
			.Select(x => x.Host)
			.FirstOrDefault(x => x.OwnerHostItemId == hostItemId);
	}

	private bool CanReachHost(IComputerHost sourceHost, IComputerHost targetHost)
	{
		if (ReferenceEquals(sourceHost, targetHost) || sourceHost.OwnerHostItemId == targetHost.OwnerHostItemId)
		{
			return true;
		}

		return _gameworld.ComputerExecutionService.GetReachableHosts(sourceHost)
			.Any(x => x.Host.OwnerHostItemId == targetHost.OwnerHostItemId);
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

		var parts = address.Trim().ToLowerInvariant().Split('@', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
			error = "Mail domains must be non-empty and include at least one period.";
			return false;
		}

		return true;
	}

	private static RuntimeMailAccount ToRuntimeAccount(ComputerMailAccount account)
	{
		return new RuntimeMailAccount
		{
			Id = account.Id,
			DomainId = account.ComputerMailDomainId,
			UserName = account.UserName,
			DomainName = account.ComputerMailDomain.DomainName,
			Enabled = account.IsEnabled
		};
	}

	private static ComputerMailMessageHeader ToHeader(ComputerMailMailboxEntry entry)
	{
		return new ComputerMailMessageHeader
		{
			MailboxEntryId = entry.Id,
			SenderAddress = entry.ComputerMailMessage.SenderAddress,
			RecipientAddress = entry.ComputerMailMessage.RecipientAddress,
			Subject = entry.ComputerMailMessage.Subject,
			SentAtUtc = entry.ComputerMailMessage.SentAtUtc,
			DeliveredAtUtc = entry.DeliveredAtUtc,
			IsRead = entry.IsRead,
			IsDeleted = entry.IsDeleted,
			IsSentFolder = entry.IsSentFolder
		};
	}

	private static void DeleteOrphanMessages(FuturemudDatabaseContext context, IEnumerable<long> candidateMessageIds)
	{
		var messageIds = candidateMessageIds.Distinct().ToList();
		if (!messageIds.Any())
		{
			return;
		}

		var orphans = context.ComputerMailMessages
			.Include(x => x.MailboxEntries)
			.Where(x => messageIds.Contains(x.Id))
			.Where(x => !x.MailboxEntries.Any())
			.ToList();
		if (!orphans.Any())
		{
			return;
		}

		context.ComputerMailMessages.RemoveRange(orphans);
	}
}
