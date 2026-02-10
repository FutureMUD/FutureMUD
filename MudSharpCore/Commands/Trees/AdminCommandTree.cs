using MudSharp.Accounts;
using MudSharp.Commands.Modules;

namespace MudSharp.Commands.Trees;

internal class AdminCommandTree : GuideCommandTree
{
	protected AdminCommandTree()
	{
	}

	public static AdminCommandTree JuniorAdminCommandTree { get; } = new()
	{
		PermissionLevel = PermissionLevel.JuniorAdmin
	};

	public static AdminCommandTree StandardAdminCommandTree { get; } = new()
	{
		PermissionLevel = PermissionLevel.Admin
	};

	public static AdminCommandTree SeniorAdminCommandTree { get; } = new()
	{
		PermissionLevel = PermissionLevel.SeniorAdmin
	};

	public static AdminCommandTree HighAdminCommandTree { get; } = new()
	{
		PermissionLevel = PermissionLevel.HighAdmin
	};

	protected override void ProcessCommands()
	{
		base.ProcessCommands();
		Commands.AddFrom(StorytellerModule.Instance.Commands);
		Commands.AddFrom(AIStorytellerModule.Instance.Commands);
		Commands.AddFrom(RoomBuilderModule.Instance.Commands);
		Commands.AddFrom(StaffModule.Instance.Commands);
		Commands.AddFrom(ProgModule.Instance.Commands);
		Commands.AddFrom(MagicModule.Instance.Commands);
		Commands.AddFrom(ChargenModule.Instance.Commands);
	}
}
