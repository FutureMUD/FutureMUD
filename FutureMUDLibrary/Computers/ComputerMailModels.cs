#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Computers;

public interface IComputerMailAccount : IComputerNetworkAccount
{
}

public sealed class ComputerMailDomainInfo : ComputerNetworkDomainInfo
{
}

public sealed class ComputerMailMessageHeader
{
	public long MailboxEntryId { get; init; }
	public string SenderAddress { get; init; } = string.Empty;
	public string RecipientAddress { get; init; } = string.Empty;
	public string Subject { get; init; } = string.Empty;
	public DateTime SentAtUtc { get; init; }
	public DateTime DeliveredAtUtc { get; init; }
	public bool IsRead { get; init; }
	public bool IsDeleted { get; init; }
	public bool IsSentFolder { get; init; }
}

public sealed class ComputerMailMessageDetails
{
	public required ComputerMailMessageHeader Header { get; init; }
	public string Body { get; init; } = string.Empty;
}

public sealed class ComputerMailAuthenticationResult
{
	public bool Success { get; init; }
	public string ErrorMessage { get; init; } = string.Empty;
	public IComputerMailAccount? Account { get; init; }
}

public interface IComputerMailService
{
	bool IsMailServiceEnabled(IComputerHost host);
	bool SetMailServiceEnabled(IComputerHost host, bool enabled, out string error);
	IEnumerable<ComputerMailDomainInfo> GetHostedDomains(IComputerHost host);
	bool RegisterDomain(IComputerHost host, string domainName, out string error);
	bool RemoveDomain(IComputerHost host, string domainName, out string error);
	bool SetDomainEnabled(IComputerHost host, string domainName, bool enabled, out string error);
	IEnumerable<IComputerMailAccount> GetAccounts(IComputerHost host, string? domainName = null);
	bool CreateAccount(IComputerHost host, string address, string password, out string error);
	bool SetAccountEnabled(IComputerHost host, string address, bool enabled, out string error);
	bool SetAccountPassword(IComputerHost host, string address, string password, out string error);
	ComputerMailAuthenticationResult Authenticate(IComputerHost sourceHost, string address, string password);
	IComputerMailAccount? GetAccount(IComputerHost sourceHost, long accountId, out string error);
	IEnumerable<ComputerMailMessageHeader> GetMailboxHeaders(IComputerHost sourceHost, IComputerMailAccount account,
		bool includeDeleted = false);
	ComputerMailMessageDetails? ReadMessage(IComputerHost sourceHost, IComputerMailAccount account, long mailboxEntryId,
		out string error);
	bool DeleteMessage(IComputerHost sourceHost, IComputerMailAccount account, long mailboxEntryId, out string error);
	bool SendMessage(IComputerHost sourceHost, IComputerMailAccount senderAccount, string recipientAddress, string subject,
		string body, out string error);
	IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId);
}
