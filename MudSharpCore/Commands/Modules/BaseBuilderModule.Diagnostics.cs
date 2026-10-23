#nullable enable
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Commands.Modules;

internal abstract partial class BaseBuilderModule
{
	private static bool RestoreBuilderName(ICharacter actor, EditableItemHelper helper, IEditableItem item, string oldName)
	{
		var items = helper.GetAllEditableItems(actor).ToList();
		if (!items.Any(x => ReferenceEquals(x, item))) return false;
		var validation = helper.TryNormaliseNameForBulkRename(item, oldName);
		if (!validation.IsValid || !string.Equals(validation.Name, oldName, StringComparison.OrdinalIgnoreCase)) return false;
		if (items.Any(x => !ReferenceEquals(x, item) && Equals(helper.NameScopeKeyFunc(x), helper.NameScopeKeyFunc(item)) && x.Name.EqualTo(oldName))) return false;
		helper.SetNameFromValidatedBulkRenameAction(item, oldName);
		return true;
	}

	private static bool RestoreBuilderName(ICharacter actor, EditableRevisableItemHelper helper, IEditableRevisableItem item, string oldName)
	{
		var items = helper.GetAllEditableItems(actor).ToList();
		if (!items.Any(x => ReferenceEquals(x, item)) || !CanModifyRevisableItem(actor, helper, item)) return false;
		if (!actor.IsAdministrator() && item.Status is not (RevisionStatus.UnderDesign or RevisionStatus.PendingRevision)) return false;
		var validation = helper.TryNormaliseNameForBulkRename(item, oldName);
		if (!validation.IsValid || !string.Equals(validation.Name, oldName, StringComparison.OrdinalIgnoreCase)) return false;
		if (items.Any(x => x.Id != item.Id && x.Status is RevisionStatus.Current or RevisionStatus.PendingRevision or RevisionStatus.UnderDesign &&
		                   Equals(helper.NameScopeKeyFunc(x), helper.NameScopeKeyFunc(item)) && x.Name.EqualTo(oldName))) return false;
		helper.SetNameFromValidatedBulkRenameAction(item, oldName);
		return true;
	}

	private static void GenericNameUndo(ICharacter actor, object helper)
	{
		var undo = actor.EffectsOfType<BuilderNameUndo>().LastOrDefault(x => ReferenceEquals(x.Helper, helper));
		if (undo is null) { actor.OutputHandler.Send("No recoverable set-name edit remains for this family (30-minute limit)."); return; }
		if (undo.Target.Name != undo.ExpectedName)
		{
			actor.OutputHandler.Send("The name changed again; undo was refused to preserve the newer edit.");
			return;
		}
		if (!undo.Restore()) { actor.OutputHandler.Send("Undo was refused by current existence, permission or name-validation rules."); return; }
		actor.RemoveEffect(undo);
		actor.OutputHandler.Send("The previous name was restored. Other settings are unchanged.");
	}

	private static void GenericValidate(ICharacter actor, StringStack input, EditableRevisableItemHelper helper)
	{
		var item = ResolveRevision(actor, input, helper);
		if (item is null) return;
		actor.OutputHandler.Send(BuilderPreflight.Describe(item, actor.Gameworld));
	}

	private static void GenericValidate(ICharacter actor, StringStack input, EditableItemHelper helper)
	{
		var item = input.IsFinished ? helper.GetEditableItemFunc(actor) : helper.GetEditableItemByIdOrNameFunc(actor, input.SafeRemainingArgument);
		if (item is null) { actor.OutputHandler.Send("Select an item or supply its ID/name to validate."); return; }
		actor.OutputHandler.Send(BuilderPreflight.Describe(item, actor.Gameworld));
	}

	private static IEditableRevisableItem? ResolveRevision(ICharacter actor, StringStack input, EditableRevisableItemHelper helper)
	{
		IEditableRevisableItem? item;
		if (input.IsFinished) item = helper.GetEditableItemFunc(actor);
		else if (!long.TryParse(input.PopSpeech(), out var id)) item = null;
		else if (input.IsFinished) item = helper.GetEditableItemByIdFunc(actor, id);
		else if (int.TryParse(input.PopSpeech(), out var revision) && input.IsFinished) item = helper.GetEditableItemByIdRevNumFunc(actor, id, revision);
		else item = null;
		if (item is not null && helper.CanViewItemFunc(actor, item)) return item;
		actor.OutputHandler.Send("No visible revision was selected. Use an ID and optional revision number, or your current selection.");
		return null;
	}

	private static void GenericRevisionDiff(ICharacter actor, StringStack input, EditableRevisableItemHelper helper)
	{
		var candidate = ResolveRevision(actor, input, helper);
		if (candidate is null) return;
		DisplayRevisionDiff(actor, candidate, helper);
	}

	private static void DisplayRevisionDiff(ICharacter actor, IEditableRevisableItem candidate, EditableRevisableItemHelper helper)
	{
		var current = helper.GetAllEditableItemsByIdFunc(actor, candidate.Id)
			.Where(x => x.Status == RevisionStatus.Current && helper.CanViewItemFunc(actor, x))
			.OrderByDescending(x => x.RevisionNumber).FirstOrDefault();
		var after = BuilderRevisionDiff.Snapshot(candidate);
		if (after is null) { actor.OutputHandler.Send("Field comparison is not yet supported for this family; use show and review history."); return; }
		if (current is null) { actor.OutputHandler.Send("There is no visible approved revision to compare against; this is new content."); return; }
		var before = BuilderRevisionDiff.Snapshot(current)!;
		var changes = BuilderRevisionDiff.Compare(before, after);
		var output = new System.Text.StringBuilder($"Changes from approved revision {current.RevisionNumber} to revision {candidate.RevisionNumber}:\n");
		foreach (var change in changes)
		{
			output.AppendLine(change.Field.ColourName());
			output.AppendLine($"  Before: {change.Before}");
			output.AppendLine($"  After:  {change.After}");
		}
		if (changes.Count == 0) output.AppendLine("No changes in the compared fields.");
		output.AppendLine("Coverage: skin fields, or prototype scalar fields, component references, variables and extra descriptions. Component internals, tags and planar settings require show/review.");
		if (actor.IsAdministrator()) output.AppendLine(BuilderReferenceReport.Describe(actor.Gameworld, candidate));
		actor.OutputHandler.Send(output.ToString());
	}
}
