using MudSharp.Accounts;
using MudSharp.Commands.Modules;

namespace MudSharp.Commands.Trees;

internal class GuideCommandTree : PlayerCommandTree
{
	protected GuideCommandTree()
	{
	}

	public new static GuideCommandTree Instance { get; } = new()
	{
		PermissionLevel = PermissionLevel.Guide
	};

	protected override void ProcessCommands()
	{
		base.ProcessCommands();
		Commands.AddFrom(GuideModule.Instance.Commands);
		Commands.AddFrom(BuilderModule.Instance.Commands);
		Commands.AddFrom(CombatBuilderModule.Instance.Commands);
		Commands.AddFrom(ActivityBuilderModule.Instance.Commands);
		Commands.AddFrom(NPCBuilderModule.Instance.Commands);
		Commands.AddFrom(ItemBuilderModule.Instance.Commands);
	}
}