#nullable enable

using System.Collections.Generic;

namespace MudSharp.Computers;

public interface IComputerNetworkAccount
{
	long Id { get; }
	long DomainId { get; }
	string UserName { get; }
	string DomainName { get; }
	string Address { get; }
	bool Enabled { get; }
}

public class ComputerNetworkDomainInfo
{
	public long Id { get; init; }
	public string DomainName { get; init; } = string.Empty;
	public long HostItemId { get; init; }
	public bool Enabled { get; init; }
}

public sealed class ComputerNetworkAuthenticationResult
{
	public bool Success { get; init; }
	public string ErrorMessage { get; init; } = string.Empty;
	public IComputerNetworkAccount? Account { get; init; }
}

public interface IComputerNetworkIdentityService
{
	IEnumerable<ComputerNetworkDomainInfo> GetHostedDomains(IComputerHost host);
	bool RegisterDomain(IComputerHost host, string domainName, out string error);
	bool RemoveDomain(IComputerHost host, string domainName, out string error);
	bool SetDomainEnabled(IComputerHost host, string domainName, bool enabled, out string error);
	IEnumerable<IComputerNetworkAccount> GetAccounts(IComputerHost host, string? domainName = null);
	bool CreateAccount(IComputerHost host, string address, string password, out string error);
	bool SetAccountEnabled(IComputerHost host, string address, bool enabled, out string error);
	bool SetAccountPassword(IComputerHost host, string address, string password, out string error);
	ComputerNetworkAuthenticationResult Authenticate(IComputerHost sourceHost, string address, string password);
	IComputerNetworkAccount? GetAccount(IComputerHost sourceHost, long accountId, out string error);
	IEnumerable<string> GetAdvertisedDomainDetails(IComputerHost host);
}
