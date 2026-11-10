#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Discord;

/// <summary>An explicitly published, character-independent remote action backed by a trusted FutureProg.</summary>
public interface IDiscordHook : IFrameworkItem
{
	string Key { get; }
	string Category { get; }
	string Description { get; }
	string Usage { get; }
	IFutureProg? Prog { get; }
	bool Enabled { get; }
	PermissionLevel MinimumAuthority { get; }
	bool RequireConfirmation { get; }
	int CooldownSeconds { get; }
	int Revision { get; }
}
