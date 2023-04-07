using MudSharp.Accounts;
using MudSharp.Commands.Modules;

namespace MudSharp.Commands.Trees;

internal class GuestCommandTree : ActorCommandTree
{
	protected GuestCommandTree()
	{
	}

	public new static GuestCommandTree Instance { get; } = new()
	{
		PermissionLevel = PermissionLevel.Guest
	};

	protected override void ProcessCommands()
	{
		base.ProcessCommands();
		Commands.AddFrom(PlayerOnlyModule.Instance.Commands);
	}
}