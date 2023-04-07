using MudSharp.Accounts;
using MudSharp.Commands.Modules;

namespace MudSharp.Commands.Trees;

internal class NPCCommandTree : ActorCommandTree
{
	protected NPCCommandTree()
	{
	}

	public new static NPCCommandTree Instance { get; } = new() { PermissionLevel = PermissionLevel.NPC };

	protected override void ProcessCommands()
	{
		base.ProcessCommands();
		Commands.AddFrom(NPCOnlyModule.Instance.Commands);
		Commands.AddFrom(PlayerOnlyModule.Instance.Commands);
	}
}