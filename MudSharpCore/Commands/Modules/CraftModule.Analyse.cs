#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts;

namespace MudSharp.Commands.Modules;

internal partial class CraftModule
{
	[PlayerCommand("Analyse", "analyse", "analyze")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("analyse", @"Analyse a visible item in your inventory to recognise possible crafting recipes you understand.
It shows ingredient and tool requirements, including authored fire and workstation tools.

Syntax: #3analyse <carried item>#0 (or #3analyze <carried item>#0).

This capability must be enabled for your character by your game. Recipe visibility, use restrictions and
crafting proficiency still apply. Analysis does not consume the item or teach recipes or skills.
Scripted or variable outputs without a reliable reverse match cannot be identified.", AutoHelp.HelpArgOrNoArg)]
	protected static void Analyse(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What carried item do you want to analyse?");
			return;
		}

		if (!actor.Gameworld.GetCheck(CheckType.AnalyzeEligibilityCheck)
			.Check(actor, Difficulty.Normal, (MudSharp.Body.Traits.ITraitDefinition?)null).IsPass())
		{
			actor.OutputHandler.Send("You do not have the ability to analyse crafting recipes.");
			return;
		}

		var item = actor.TargetPersonalItem(ss.SafeRemainingArgument);
		if (item is null)
		{
			actor.OutputHandler.Send("You do not see any such item in your inventory.");
			return;
		}

		var recipes = CraftAnalysis.FindRecipes(actor, item, actor.Gameworld.Crafts);
		actor.OutputHandler.Send(CraftAnalysis.DescribeRecipes(actor, item, recipes));
	}
}
