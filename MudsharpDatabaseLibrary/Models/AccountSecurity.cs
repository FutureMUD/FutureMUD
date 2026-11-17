#nullable enable
using System;

namespace MudSharp.Models;

public partial class Account
{
	public long CredentialVersion { get; set; }
	public DateTime? RecoveryExpiresUtc { get; set; }
	public DateTime? RecoveryIssuedUtc { get; set; }
	public long? RecoveryCredentialVersion { get; set; }
	public int RecoveryAttempts { get; set; }
	public int LoginAttempts { get; set; }
	public DateTime? LoginAttemptWindowUtc { get; set; }
	public DateTime? LoginBlockedUntilUtc { get; set; }
}
