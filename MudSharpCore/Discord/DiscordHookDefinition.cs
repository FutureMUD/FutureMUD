#nullable enable

using MudSharp.Accounts;

namespace MudSharp.Discord;

internal sealed class DiscordHookDefinition(Models.DiscordHook definition, IFuturemud world) : IDiscordHook
{
	public long Id => definition.Id;
	public string Name => definition.Key;
	public string FrameworkItemType => "DiscordHook";
	public string Key => definition.Key;
	public string Category => definition.Category;
	public string Description => definition.Description;
	public string Usage => definition.Usage;
	public IFutureProg? Prog => world.FutureProgs.Get(definition.ProgId);
	public bool Enabled => definition.Enabled;
	public PermissionLevel MinimumAuthority => (PermissionLevel)definition.MinimumAuthority;
	public bool RequireConfirmation => definition.RequireConfirmation;
	public int CooldownSeconds => definition.CooldownSeconds;
	public int Revision => definition.Revision;
}
