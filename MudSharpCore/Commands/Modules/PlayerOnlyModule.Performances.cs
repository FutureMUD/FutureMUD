#nullable enable

using MudSharp.RPG.Performances;

namespace MudSharp.Commands.Modules;

internal partial class PlayerOnlyModule
{
	[PlayerCommand("Performance", "performance")]
	[HelpInfo("performance", PerformanceService.Help, AutoHelp.HelpArgOrNoArg, PerformanceService.StaffHelp)]
	protected static void Performance(ICharacter actor, string input)
	{
		PerformanceService.For(actor.Gameworld).Command(actor, new StringStack(input.RemoveFirstWord()));
	}
}
