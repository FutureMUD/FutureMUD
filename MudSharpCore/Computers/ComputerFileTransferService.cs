#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Computers;

public sealed class ComputerFileTransferService : IComputerFileTransferService
{
	private readonly IFuturemud _gameworld;

	public ComputerFileTransferService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public bool IsFtpServiceEnabled(IComputerHost host)
	{
		return host.IsNetworkServiceEnabled("ftp");
	}

	public bool SetFtpServiceEnabled(IComputerHost host, bool enabled, out string error)
	{
		return host.SetNetworkServiceEnabled("ftp", enabled, out error);
	}

	public IEnumerable<IComputerFtpAccount> GetAccounts(IComputerHost host)
	{
		return host is IComputerFtpAccountStore accountStore
			? accountStore.FtpAccounts
				.OrderBy(x => x.UserName)
				.ToList()
			: Enumerable.Empty<IComputerFtpAccount>();
	}

	public bool CreateAccount(IComputerHost host, string userName, string password, out string error)
	{
		error = string.Empty;
		if (!TryNormaliseUserName(userName, out var normalisedUserName, out error))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			error = "You must supply a password.";
			return false;
		}

		if (host is not IComputerFtpAccountStore accountStore)
		{
			error = $"{host.Name.ColourName()} does not support FTP account storage.";
			return false;
		}

		var salt = SecurityUtilities.GetSalt64();
		var hash = SecurityUtilities.GetPasswordHash(password, salt);
		return accountStore.CreateFtpAccount(normalisedUserName, hash, salt, out error);
	}

	public bool SetAccountEnabled(IComputerHost host, string userName, bool enabled, out string error)
	{
		error = string.Empty;
		if (!TryNormaliseUserName(userName, out var normalisedUserName, out error))
		{
			return false;
		}

		if (host is not IComputerFtpAccountStore accountStore)
		{
			error = $"{host.Name.ColourName()} does not support FTP account storage.";
			return false;
		}

		return accountStore.SetFtpAccountEnabled(normalisedUserName, enabled, out error);
	}

	public bool SetAccountPassword(IComputerHost host, string userName, string password, out string error)
	{
		error = string.Empty;
		if (!TryNormaliseUserName(userName, out var normalisedUserName, out error))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			error = "You must supply a password.";
			return false;
		}

		if (host is not IComputerFtpAccountStore accountStore)
		{
			error = $"{host.Name.ColourName()} does not support FTP account storage.";
			return false;
		}

		var salt = SecurityUtilities.GetSalt64();
		var hash = SecurityUtilities.GetPasswordHash(password, salt);
		return accountStore.SetFtpAccountPassword(normalisedUserName, hash, salt, out error);
	}

	public ComputerFtpAuthenticationResult Authenticate(IComputerHost sourceHost, IComputerHost targetHost, string userName,
		string password)
	{
		if (!TryResolveTargetHost(sourceHost, targetHost, out _, out var error))
		{
			return new ComputerFtpAuthenticationResult
			{
				Success = false,
				ErrorMessage = error
			};
		}

		if (!TryNormaliseUserName(userName, out var normalisedUserName, out error))
		{
			return new ComputerFtpAuthenticationResult
			{
				Success = false,
				ErrorMessage = error
			};
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			return new ComputerFtpAuthenticationResult
			{
				Success = false,
				ErrorMessage = "You must supply a password."
			};
		}

		var account = ResolveAccount(targetHost, normalisedUserName);
		if (account is null || !account.Enabled)
		{
			return new ComputerFtpAuthenticationResult
			{
				Success = false,
				ErrorMessage = $"There is no enabled FTP account named {normalisedUserName.ColourName()} on {targetHost.Name.ColourName()}."
			};
		}

		if (!SecurityUtilities.VerifyPassword(password, account.PasswordHash, account.PasswordSalt))
		{
			return new ComputerFtpAuthenticationResult
			{
				Success = false,
				ErrorMessage = "That password is not correct."
			};
		}

		return new ComputerFtpAuthenticationResult
		{
			Success = true,
			Account = account
		};
	}

	public IComputerFtpAccount? GetAccount(IComputerHost sourceHost, IComputerHost targetHost, string userName,
		out string error)
	{
		error = string.Empty;
		if (!TryResolveTargetHost(sourceHost, targetHost, out _, out error))
		{
			return null;
		}

		if (!TryNormaliseUserName(userName, out var normalisedUserName, out error))
		{
			return null;
		}

		var account = ResolveAccount(targetHost, normalisedUserName);
		if (account is null || !account.Enabled)
		{
			error = $"There is no enabled FTP account named {normalisedUserName.ColourName()} on {targetHost.Name.ColourName()}.";
			return null;
		}

		return account;
	}

	public IEnumerable<ComputerRemoteFileOwnerSummary> GetAccessibleOwners(IComputerHost sourceHost, IComputerHost targetHost,
		IComputerFtpAccount? account, out string error)
	{
		error = string.Empty;
		if (!TryResolveTargetHost(sourceHost, targetHost, out _, out error))
		{
			return Enumerable.Empty<ComputerRemoteFileOwnerSummary>();
		}

		var authenticated = account is not null && TryEnsureAuthenticatedAccount(sourceHost, targetHost, account, out error);
		if (account is not null && !authenticated)
		{
			return Enumerable.Empty<ComputerRemoteFileOwnerSummary>();
		}

		return ComputerFileTransferUtilities.EnumerateOwners(targetHost)
			.Where(owner => authenticated || (owner.FileSystem?.Files.Any(x => x.PubliclyAccessible) ?? false))
			.Select(owner => new ComputerRemoteFileOwnerSummary
			{
				OwnerIdentifier = ComputerFileTransferUtilities.GetOwnerIdentifier(targetHost, owner),
				DisplayName = ComputerFileTransferUtilities.DescribeOwner(owner),
				IsHostOwner = owner is IComputerHost,
				AnonymousAccessible = owner.FileSystem?.Files.Any(x => x.PubliclyAccessible) ?? false
			})
			.OrderBy(x => x.IsHostOwner ? 0 : 1)
			.ThenBy(x => x.DisplayName)
			.ToList();
	}

	public IEnumerable<ComputerRemoteFileSummary> GetFiles(IComputerHost sourceHost, IComputerHost targetHost,
		IComputerFtpAccount? account, string? ownerIdentifier, out string error)
	{
		error = string.Empty;
		if (!TryResolveTargetHost(sourceHost, targetHost, out var hostAddress, out error))
		{
			return Enumerable.Empty<ComputerRemoteFileSummary>();
		}

		var authenticated = account is not null && TryEnsureAuthenticatedAccount(sourceHost, targetHost, account, out error);
		if (account is not null && !authenticated)
		{
			return Enumerable.Empty<ComputerRemoteFileSummary>();
		}

		var owner = ResolveRemoteOwner(targetHost, ownerIdentifier, out error);
		if (owner is null)
		{
			return Enumerable.Empty<ComputerRemoteFileSummary>();
		}

		return (owner.FileSystem?.Files ?? Enumerable.Empty<IComputerFile>())
			.Where(x => authenticated || x.PubliclyAccessible)
			.OrderBy(x => x.FileName)
			.ThenBy(x => x.CreatedAtUtc)
			.Select(x => ToSummary(targetHost, hostAddress, owner, x, !authenticated))
			.ToList();
	}

	public ComputerRemoteFileDetails? ReadFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount? account,
		string? ownerIdentifier, string fileName, out string error)
	{
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(fileName))
		{
			error = "You must specify a file name.";
			return null;
		}

		if (!TryResolveTargetHost(sourceHost, targetHost, out var hostAddress, out error))
		{
			return null;
		}

		var authenticated = account is not null && TryEnsureAuthenticatedAccount(sourceHost, targetHost, account, out error);
		if (account is not null && !authenticated)
		{
			return null;
		}

		var owner = ResolveRemoteOwner(targetHost, ownerIdentifier, out error);
		if (owner is null)
		{
			return null;
		}

		var file = owner.FileSystem?.GetFile(fileName);
		if (file is null || (!authenticated && !file.PubliclyAccessible))
		{
			error = $"{ComputerFileTransferUtilities.DescribeOwner(owner).ColourName()} does not expose a readable file named {fileName.ColourName()}.";
			return null;
		}

		return new ComputerRemoteFileDetails
		{
			Summary = ToSummary(targetHost, hostAddress, owner, file, !authenticated),
			TextContents = file.TextContents
		};
	}

	public bool WriteFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		string? ownerIdentifier, string fileName, string textContents, out string error)
	{
		return MutateRemoteFile(sourceHost, targetHost, account, ownerIdentifier, fileName,
			fileSystem => fileSystem.WriteFile(fileName, textContents ?? string.Empty), out error);
	}

	public bool AppendFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		string? ownerIdentifier, string fileName, string textContents, out string error)
	{
		return MutateRemoteFile(sourceHost, targetHost, account, ownerIdentifier, fileName,
			fileSystem => fileSystem.AppendFile(fileName, textContents ?? string.Empty), out error);
	}

	public bool DeleteFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		string? ownerIdentifier, string fileName, out string error)
	{
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(fileName))
		{
			error = "You must specify a file name.";
			return false;
		}

		if (!TryEnsureAuthenticatedAccount(sourceHost, targetHost, account, out error))
		{
			return false;
		}

		var owner = ResolveRemoteOwner(targetHost, ownerIdentifier, out error);
		if (owner is null)
		{
			return false;
		}

		var fileSystem = owner.FileSystem;
		if (fileSystem is null)
		{
			error = $"{ComputerFileTransferUtilities.DescribeOwner(owner).ColourName()} does not expose a writable file system.";
			return false;
		}

		if (!fileSystem.DeleteFile(fileName))
		{
			error = $"{ComputerFileTransferUtilities.DescribeOwner(owner).ColourName()} does not have a file named {fileName.ColourName()}.";
			return false;
		}

		return true;
	}

	public IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId)
	{
		if (!applicationId.EqualTo("ftp") || !host.IsNetworkServiceEnabled("ftp"))
		{
			return Enumerable.Empty<string>();
		}

		var publicCount = ComputerFileTransferUtilities.EnumerateOwners(host)
			.SelectMany(x => x.FileSystem?.Files ?? Enumerable.Empty<IComputerFile>())
			.Count(x => x.PubliclyAccessible);
		return [$"{publicCount.ToString("N0")} public {"file".Pluralise(publicCount != 1)}"];
	}

	private bool MutateRemoteFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		string? ownerIdentifier, string fileName, Action<IComputerFileSystem> action, out string error)
	{
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(fileName))
		{
			error = "You must specify a file name.";
			return false;
		}

		if (!TryEnsureAuthenticatedAccount(sourceHost, targetHost, account, out error))
		{
			return false;
		}

		var owner = ResolveRemoteOwner(targetHost, ownerIdentifier, out error);
		if (owner is null)
		{
			return false;
		}

		var fileSystem = owner.FileSystem;
		if (fileSystem is null)
		{
			error = $"{ComputerFileTransferUtilities.DescribeOwner(owner).ColourName()} does not expose a writable file system.";
			return false;
		}

		action(fileSystem);
		return true;
	}

	private bool TryEnsureAuthenticatedAccount(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		out string error)
	{
		var resolvedAccount = GetAccount(sourceHost, targetHost, account.UserName, out error);
		return resolvedAccount is not null;
	}

	private bool TryResolveTargetHost(IComputerHost sourceHost, IComputerHost targetHost, out string hostAddress,
		out string error)
	{
		hostAddress = targetHost.Name;
		error = string.Empty;
		if (!targetHost.Powered)
		{
			error = $"{targetHost.Name.ColourName()} is not currently powered.";
			return false;
		}

		if (!targetHost.IsNetworkServiceEnabled("ftp"))
		{
			error = $"{targetHost.Name.ColourName()} is not currently advertising FTP.";
			return false;
		}

		if (ReferenceEquals(sourceHost, targetHost) || sourceHost.OwnerHostItemId == targetHost.OwnerHostItemId)
		{
			hostAddress = sourceHost.NetworkAdapters
				.Where(x => x.NetworkReady)
				.Select(x => x.NetworkAddress)
				.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? targetHost.Name;
			return true;
		}

		var summary = _gameworld.ComputerExecutionService.GetReachableHosts(sourceHost)
			.FirstOrDefault(x => x.Host.OwnerHostItemId == targetHost.OwnerHostItemId);
		if (summary is null)
		{
			error = $"{targetHost.Name.ColourName()} is not reachable from {sourceHost.Name.ColourName()}.";
			return false;
		}

		hostAddress = summary.CanonicalAddress;
		return true;
	}

	private ComputerMutableFtpAccount? ResolveAccount(IComputerHost host, string userName)
	{
		return host is not IComputerFtpAccountStore accountStore
			? null
			: accountStore.FtpAccounts
				.OfType<ComputerMutableFtpAccount>()
				.FirstOrDefault(x => x.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
	}

	private IComputerFileOwner? ResolveRemoteOwner(IComputerHost targetHost, string? ownerIdentifier, out string error)
	{
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(ownerIdentifier))
		{
			return targetHost;
		}

		var owner = ComputerFileTransferUtilities.ResolveSelectableOwner(targetHost, ownerIdentifier, out var resolvedError);
		error = resolvedError ?? string.Empty;
		if (owner is not null)
		{
			return owner;
		}

		error = string.IsNullOrEmpty(error)
			? $"There is no remote owner named {ownerIdentifier.ColourName()} on {targetHost.Name.ColourName()}."
			: error;
		return null;
	}

	private static ComputerRemoteFileSummary ToSummary(IComputerHost targetHost, string hostAddress,
		IComputerFileOwner owner, IComputerFile file, bool readOnly)
	{
		return new ComputerRemoteFileSummary
		{
			HostName = targetHost.Name,
			HostAddress = hostAddress,
			OwnerIdentifier = ComputerFileTransferUtilities.GetOwnerIdentifier(targetHost, owner),
			OwnerDisplayName = ComputerFileTransferUtilities.DescribeOwner(owner),
			FileName = file.FileName,
			SizeInBytes = file.SizeInBytes,
			CreatedAtUtc = file.CreatedAtUtc,
			LastModifiedAtUtc = file.LastModifiedAtUtc,
			PubliclyAccessible = file.PubliclyAccessible,
			ReadOnly = readOnly
		};
	}

	private static bool TryNormaliseUserName(string userName, out string normalisedUserName, out string error)
	{
		normalisedUserName = userName.Trim().ToLowerInvariant();
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(normalisedUserName))
		{
			error = "You must supply a user name.";
			return false;
		}

		if (normalisedUserName.Any(char.IsWhiteSpace))
		{
			error = "FTP user names cannot contain whitespace.";
			return false;
		}

		return true;
	}
}
