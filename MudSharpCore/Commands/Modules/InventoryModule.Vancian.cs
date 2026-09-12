#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class InventoryModule
{
	[PlayerCommand("Spellbook", "spellbook")]
	[HelpInfo("spellbook", MagicModule.SpellbookHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void SpellbookCommand(ICharacter actor, string input) => MagicModule.SpellbookCommand(actor, input);

	[PlayerCommand("SpellScroll", "spellscroll")]
	[HelpInfo("spellscroll", MagicModule.SpellScrollHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void SpellScrollCommand(ICharacter actor, string input) => MagicModule.SpellScrollCommand(actor, input);
}
