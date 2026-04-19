#nullable enable

using System.Collections.Generic;

namespace MudSharp.Computers;

internal interface IComputerFtpAccountStore
{
	IEnumerable<IComputerFtpAccount> FtpAccounts { get; }
	bool CreateFtpAccount(string userName, string passwordHash, long passwordSalt, out string error);
	bool SetFtpAccountEnabled(string userName, bool enabled, out string error);
	bool SetFtpAccountPassword(string userName, string passwordHash, long passwordSalt, out string error);
}
