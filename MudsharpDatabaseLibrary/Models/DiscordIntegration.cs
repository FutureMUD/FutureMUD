#nullable enable

using System;

namespace MudSharp.Models;

public class DiscordAccountLink
{
	public string DiscordUserId { get; set; } = "";
	public long AccountId { get; set; }
	public bool NotificationsEnabled { get; set; }
	public DateTime CreatedUtc { get; set; }
	public virtual Account Account { get; set; } = null!;
}

public class DiscordHook
{
	public long Id { get; set; }
	public string Key { get; set; } = "";
	public string Category { get; set; } = "General";
	public string Description { get; set; } = "";
	public string Usage { get; set; } = "";
	public long ProgId { get; set; }
	public int MinimumAuthority { get; set; } = 6;
	public bool Enabled { get; set; }
	public bool RequireConfirmation { get; set; } = true;
	public int CooldownSeconds { get; set; } = 60;
	public int Revision { get; set; } = 1;
	public string AllowedGuildIds { get; set; } = "";
	public string AllowedChannelIds { get; set; } = "";
	public string AllowedAccountIds { get; set; } = "";
	public virtual FutureProg Prog { get; set; } = null!;
}

public class DiscordRemoteInvocation
{
	public string Id { get; set; } = "";
	public string DiscordUserId { get; set; } = "";
	public long? AccountId { get; set; }
	public string Operation { get; set; } = "";
	public string Arguments { get; set; } = "";
	public string Fingerprint { get; set; } = "";
	public long? HookId { get; set; }
	public long? ProgId { get; set; }
	public int? HookRevision { get; set; }
	public string DefinitionFingerprint { get; set; } = "";
	public string Status { get; set; } = "";
	public string ResultText { get; set; } = "";
	public string? ConfirmationId { get; set; }
	public DateTime CreatedUtc { get; set; }
	public DateTime UpdatedUtc { get; set; }
	public DateTime? ExpiresUtc { get; set; }
}

public class DiscordIntegrationSetting
{
	public string Key { get; set; } = "";
	public string Value { get; set; } = "";
}
