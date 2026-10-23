#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Commands.Modules;

internal partial class BuilderModule
{
	private const string BuilderReferencesHelp = @"Report known references to a definition before changing or retiring it.
	#3builderrefs attack|expression|prog|banktype|item|skin <id|name>#0
Providers report their coverage. Zero known references does not establish safe deletion: unloaded or serialized consumers may exist. This command is read-only and does not enable unsupported deletion.";

	[PlayerCommand("BuilderReferences", "builderrefs")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("builderrefs", BuilderReferencesHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void BuilderReferences(ICharacter actor, string command)
	{
		var input = new StringStack(command);
		input.PopSpeech();
		var family = input.PopSpeech().ToLowerInvariant();
		var key = input.SafeRemainingArgument;
		IFrameworkItem? target = family switch
		{
			"attack" => actor.Gameworld.WeaponAttacks.GetByIdOrName(key),
			"expression" => actor.Gameworld.TraitExpressions.GetByIdOrName(key),
			"prog" => long.TryParse(key, out var id) ? actor.Gameworld.FutureProgs.Get(id) : actor.Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo(key)),
			"banktype" => actor.Gameworld.BankAccountTypes.GetByIdOrName(key),
			"item" => long.TryParse(key, out var id) ? actor.Gameworld.ItemProtos.Get(id) : actor.Gameworld.ItemProtos.GetByName(key),
			"skin" => long.TryParse(key, out var id) ? actor.Gameworld.ItemSkins.Get(id) : actor.Gameworld.ItemSkins.GetByName(key),
			_ => null
		};
		if (target is null) { actor.OutputHandler.Send("No such definition.\n" + BuilderReferencesHelp.SubstituteANSIColour()); return; }
		actor.OutputHandler.Send(BuilderReferenceReport.Describe(actor.Gameworld, target));
	}

	private const string BuilderEditorsHelp = @"#3buildereditors#0 shows your open builder selections and their persistence semantics.
Live setters take effect immediately and use normal saving; some specialised editors save directly or activate after restart. Revisioned definitions require their usual submission/review workflow.
#3close#0 only closes a selection. It never cancels earlier changes. Helper-backed #3undo#0 can restore your last single #3set name#0 (or equivalent name key) within 30 minutes, if the target still exists and its name, permissions and uniqueness rules allow it. Bulk renames and other settings do not have generic undo.";

	[PlayerCommand("BuilderEditors", "buildereditors")]
	[CommandPermission(PermissionLevel.Any)]
	[HelpInfo("buildereditors", BuilderEditorsHelp, AutoHelp.HelpArg)]
	protected static void BuilderEditors(ICharacter actor, string command)
	{
		var selections = actor.CombinedEffectsOfType<IBuilderEditingEffect>().Select(x => x.EditingItem).OfType<IFrameworkItem>().Distinct().ToList();
		if (selections.Count == 0) { actor.OutputHandler.Send("You have no open builder selections."); return; }
		actor.OutputHandler.Send(StringUtilities.GetTextTable(selections.Select(item => new[]
		{
			item.FrameworkItemType, item.Id.ToString("N0", actor), item.Name,
			item is IEditableRevisableItem revision ? $"Revision {revision.RevisionNumber}: {revision.Status.DescribeEnum()} (submit/review)" : "Live edits; normal saving or editor-specific persistence"
		}), new[] { "Family", "ID", "Target", "Commit semantics" }, actor.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));
		actor.OutputHandler.Send(BuilderEditorsHelp.SubstituteANSIColour());
	}
}
