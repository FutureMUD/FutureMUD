using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Trees;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	[PlayerCommand("Employment", "employment")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("employment", EmploymentCommandService.EmploymentHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Employment(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		new EmploymentCommandService().Execute(actor, ss);
	}
}
