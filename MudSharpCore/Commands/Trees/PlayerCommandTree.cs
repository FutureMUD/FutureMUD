using MudSharp.Accounts;
using MudSharp.Commands.Modules;

namespace MudSharp.Commands.Trees;

internal class PlayerCommandTree : ActorCommandTree
{
	protected PlayerCommandTree()
	{
	}

	public new static PlayerCommandTree Instance { get; } = new()
	{
		PermissionLevel = PermissionLevel.Player
	};

	protected override void ProcessCommands()
	{
		base.ProcessCommands();
		Commands.AddFrom(PlayerOnlyModule.Instance.Commands);
		Commands.AddFrom(ItemBuilderModule.Instance.Commands);
	}
}