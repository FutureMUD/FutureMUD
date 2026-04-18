#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Computers;

public interface IComputerFtpAccount
{
	string UserName { get; }
	bool Enabled { get; }
}

public sealed class ComputerFtpAuthenticationResult
{
	public bool Success { get; init; }
	public string ErrorMessage { get; init; } = string.Empty;
	public IComputerFtpAccount? Account { get; init; }
}

public sealed class ComputerRemoteFileOwnerSummary
{
	public string OwnerIdentifier { get; init; } = string.Empty;
	public string DisplayName { get; init; } = string.Empty;
	public bool IsHostOwner { get; init; }
	public bool AnonymousAccessible { get; init; }
}

public sealed class ComputerRemoteFileSummary
{
	public string HostName { get; init; } = string.Empty;
	public string HostAddress { get; init; } = string.Empty;
	public string OwnerIdentifier { get; init; } = string.Empty;
	public string OwnerDisplayName { get; init; } = string.Empty;
	public string FileName { get; init; } = string.Empty;
	public long SizeInBytes { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime LastModifiedAtUtc { get; init; }
	public bool PubliclyAccessible { get; init; }
	public bool ReadOnly { get; init; }
}

public sealed class ComputerRemoteFileDetails
{
	public required ComputerRemoteFileSummary Summary { get; init; }
	public string TextContents { get; init; } = string.Empty;
}

public interface IComputerFileTransferService
{
	bool IsFtpServiceEnabled(IComputerHost host);
	bool SetFtpServiceEnabled(IComputerHost host, bool enabled, out string error);
	IEnumerable<IComputerFtpAccount> GetAccounts(IComputerHost host);
	bool CreateAccount(IComputerHost host, string userName, string password, out string error);
	bool SetAccountEnabled(IComputerHost host, string userName, bool enabled, out string error);
	bool SetAccountPassword(IComputerHost host, string userName, string password, out string error);
	ComputerFtpAuthenticationResult Authenticate(IComputerHost sourceHost, IComputerHost targetHost, string userName,
		string password);
	IComputerFtpAccount? GetAccount(IComputerHost sourceHost, IComputerHost targetHost, string userName, out string error);
	IEnumerable<ComputerRemoteFileOwnerSummary> GetAccessibleOwners(IComputerHost sourceHost, IComputerHost targetHost,
		IComputerFtpAccount? account, out string error);
	IEnumerable<ComputerRemoteFileSummary> GetFiles(IComputerHost sourceHost, IComputerHost targetHost,
		IComputerFtpAccount? account, string? ownerIdentifier, out string error);
	ComputerRemoteFileDetails? ReadFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount? account,
		string? ownerIdentifier, string fileName, out string error);
	bool WriteFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		string? ownerIdentifier, string fileName, string textContents, out string error);
	bool AppendFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		string? ownerIdentifier, string fileName, string textContents, out string error);
	bool DeleteFile(IComputerHost sourceHost, IComputerHost targetHost, IComputerFtpAccount account,
		string? ownerIdentifier, string fileName, out string error);
	IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId);
}
