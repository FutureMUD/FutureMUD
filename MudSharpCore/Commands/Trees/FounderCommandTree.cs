using MudSharp.Accounts;
using MudSharp.Commands.Modules;

namespace MudSharp.Commands.Trees;

internal class FounderCommandTree : AdminCommandTree
{
	protected FounderCommandTree()
	{
	}

	public new static FounderCommandTree Instance { get; } = new()
	{
		PermissionLevel = PermissionLevel.Founder
	};

	protected override void ProcessCommands()
	{
		base.ProcessCommands();
		Commands.AddFrom(ImplementorModule.Instance.Commands);
	}
}