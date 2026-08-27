using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

#nullable enable

namespace MudSharp.Commands.Helpers;

internal readonly record struct EditableItemNameValidationResult(bool IsValid, string? Name, string? Error)
{
	public static EditableItemNameValidationResult Success(string name)
	{
		return new EditableItemNameValidationResult(true, name, null);
	}

	public static EditableItemNameValidationResult Failure(string error)
	{
		return new EditableItemNameValidationResult(false, null, error);
	}
}

internal sealed record EditableItemNameRenameEntry<T>(
	T Item,
	long Id,
	int? RevisionNumber,
	RevisionStatus? Status,
	string OldName,
	string NewName)
{
	public bool ChangesName => !string.Equals(OldName, NewName, StringComparison.Ordinal);
}

internal sealed class EditableItemNameRenamePlan<T>
{
	public EditableItemNameRenamePlan(
		IReadOnlyList<EditableItemNameRenameEntry<T>> entries,
		IReadOnlyList<string> errors)
	{
		Entries = entries;
		Errors = errors;
	}

	public IReadOnlyList<EditableItemNameRenameEntry<T>> Entries { get; }
	public IReadOnlyList<string> Errors { get; }
	public bool IsValid => Errors.Count == 0;

	public bool TryApply(Action<T, string> applyRename, out int changedCount)
	{
		changedCount = 0;
		if (!IsValid)
		{
			return false;
		}

		foreach (var entry in Entries.Where(x => x.ChangesName))
		{
			applyRename(entry.Item, entry.NewName);
			changedCount++;
		}

		return true;
	}
}

internal static class EditableItemNameBulkRenamePlanner
{
	public static EditableItemNameRenamePlan<T> CreatePlan<T>(
		IEnumerable<T> items,
		string pattern,
		string replacement,
		Func<T, long> idSelector,
		Func<T, int?> revisionSelector,
		Func<T, RevisionStatus?> statusSelector,
		Func<T, string> nameSelector,
		Func<T, object?> scopeSelector,
		Func<T, string, EditableItemNameValidationResult> normaliseName,
		TimeSpan? regexTimeout = null)
	{
		var itemList = items.ToList();
		var entries = new List<EditableItemNameRenameEntry<T>>();
		var errors = new List<string>();
		Regex regex;

		try
		{
			regex = new Regex(pattern, RegexOptions.None,
				regexTimeout ?? PrototypeUniqueNameBulkRenamePlanner.DefaultRegexTimeout);
		}
		catch (ArgumentException ex)
		{
			errors.Add($"The match regular expression is invalid: {ex.Message}");
			return new EditableItemNameRenamePlan<T>(entries, errors);
		}

		var replacementError = ValidateReplacement(regex, replacement);
		if (replacementError is not null)
		{
			errors.Add(replacementError);
			return new EditableItemNameRenamePlan<T>(entries, errors);
		}

		foreach (var item in itemList)
		{
			var oldName = nameSelector(item) ?? string.Empty;
			try
			{
				if (!regex.IsMatch(oldName))
				{
					continue;
				}

				var proposedName = regex.Replace(oldName, replacement);
				var result = normaliseName(item, proposedName);
				if (!result.IsValid || result.Name is null)
				{
					errors.Add(
						$"{DescribeItem(idSelector(item), revisionSelector(item))} would have an invalid name: {result.Error}");
					entries.Add(new EditableItemNameRenameEntry<T>(
						item,
						idSelector(item),
						revisionSelector(item),
						statusSelector(item),
						oldName,
						proposedName));
					continue;
				}

				entries.Add(new EditableItemNameRenameEntry<T>(
					item,
					idSelector(item),
					revisionSelector(item),
					statusSelector(item),
					oldName,
					result.Name));
			}
			catch (RegexMatchTimeoutException)
			{
				errors.Add(
					$"The regular expression exceeded its {regex.MatchTimeout.TotalSeconds:N0}-second timeout while processing {DescribeItem(idSelector(item), revisionSelector(item))}.");
				return new EditableItemNameRenamePlan<T>(entries, errors);
			}
			catch (ArgumentException ex)
			{
				errors.Add($"The replacement expression is invalid: {ex.Message}");
				return new EditableItemNameRenamePlan<T>(entries, errors);
			}
		}

		var entriesByIdentity = entries.ToDictionary(x => (x.Id, x.RevisionNumber));
		var finalItems = itemList.Select(item =>
		{
			var id = idSelector(item);
			var revision = revisionSelector(item);
			entriesByIdentity.TryGetValue((id, revision), out var entry);
			return new
			{
				Id = id,
				Revision = revision,
				Status = statusSelector(item),
				Scope = scopeSelector(item),
				Name = entry?.NewName ?? nameSelector(item) ?? string.Empty,
				IsTarget = entry is not null
			};
		});

		foreach (var scope in finalItems.GroupBy(x => x.Scope))
		{
			foreach (var conflict in scope
			         .Where(x => !string.IsNullOrWhiteSpace(x.Name))
			         .GroupBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
			         .Where(x => x.Any(y => y.IsTarget) && x.Select(y => y.Id).Distinct().Count() > 1))
			{
				var conflictingItems = conflict
					.OrderBy(x => x.Id)
					.ThenBy(x => x.Revision)
					.Select(x => DescribeItem(x.Id, x.Revision, x.Status))
					.ListToString();
				errors.Add(
					$"The name {conflict.Key} would be shared by distinct item IDs in the same name scope: {conflictingItems}.");
			}
		}

		return new EditableItemNameRenamePlan<T>(entries, errors);
	}

	private static string DescribeItem(long id, int? revision, RevisionStatus? status = null)
	{
		var result = revision.HasValue ? $"item #{id}r{revision}" : $"item #{id}";
		return status.HasValue ? $"{result} ({status.Value.Describe()})" : result;
	}

	private static string? ValidateReplacement(Regex regex, string replacement)
	{
		var groupNames = regex.GetGroupNames().ToHashSet(StringComparer.Ordinal);
		var groupNumbers = regex.GetGroupNumbers().ToHashSet();

		for (var i = 0; i < replacement.Length; i++)
		{
			if (replacement[i] != '$' || i + 1 >= replacement.Length)
			{
				continue;
			}

			var next = replacement[i + 1];
			if (next is '$' or '&' or '`' or '\'' or '_' or '+')
			{
				i++;
				continue;
			}

			if (next == '{')
			{
				var closingBrace = replacement.IndexOf('}', i + 2);
				if (closingBrace < 0)
				{
					return "The replacement expression contains an unterminated ${group} reference.";
				}

				var groupName = replacement[(i + 2)..closingBrace];
				if (groupName.Length == 0 || !groupNames.Contains(groupName))
				{
					return $"The replacement expression refers to unknown regex group {replacement[i..(closingBrace + 1)]}.";
				}

				i = closingBrace;
				continue;
			}

			if (!char.IsDigit(next))
			{
				continue;
			}

			var end = i + 2;
			while (end < replacement.Length && char.IsDigit(replacement[end]))
			{
				end++;
			}

			var groupText = replacement[(i + 1)..end];
			if (!int.TryParse(groupText, out var groupNumber) || !groupNumbers.Contains(groupNumber))
			{
				return $"The replacement expression refers to unknown regex group ${groupText}.";
			}

			i = end - 1;
		}

		return null;
	}
}

internal static class EditableItemNameBulkRenameCommand
{
	public static void Execute(ICharacter actor, StringStack command, EditableItemHelper helper)
	{
		var items = helper.GetAllEditableItems(actor).ToList();
		var commandName = string.IsNullOrWhiteSpace(helper.CommandName)
			? helper.ItemName.ToLowerInvariant()
			: helper.CommandName;
		Execute(
			actor,
			command,
			$"{commandName} rename",
			items,
			helper.ItemName,
			helper.ItemNamePlural,
			x => x.Id,
			_ => null,
			_ => null,
			x => x.Name,
			helper.NameScopeKeyFunc,
			helper.TryNormaliseNameForBulkRename,
			helper.SetNameFromValidatedBulkRenameAction);
	}

	public static void Execute(ICharacter actor, StringStack command, EditableRevisableItemHelper helper)
	{
		var items = helper.GetAllEditableItems(actor)
			.Where(x => x.Status is RevisionStatus.Current or RevisionStatus.PendingRevision or RevisionStatus.UnderDesign)
			.ToList();
		var commandName = string.IsNullOrWhiteSpace(helper.CommandName)
			? helper.ItemName.ToLowerInvariant()
			: helper.CommandName;
		Execute(
			actor,
			command,
			$"{commandName} rename",
			items,
			helper.ItemName,
			helper.ItemNamePlural,
			x => x.Id,
			x => x.RevisionNumber,
			x => x.Status,
			x => x.Name,
			helper.NameScopeKeyFunc,
			helper.TryNormaliseNameForBulkRename,
			helper.SetNameFromValidatedBulkRenameAction);
	}

	private static void Execute<T>(
		ICharacter actor,
		StringStack command,
		string commandName,
		IEnumerable<T> items,
		string itemName,
		string itemNamePlural,
		Func<T, long> idSelector,
		Func<T, int?> revisionSelector,
		Func<T, RevisionStatus?> statusSelector,
		Func<T, string> nameSelector,
		Func<T, object?> scopeSelector,
		Func<T, string, EditableItemNameValidationResult> normaliseName,
		Action<T, string> applyName)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What regular expression do you want to match? The syntax is {($"{commandName} <match regex> <replacement text>").ColourCommand()}.");
			return;
		}

		var pattern = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What replacement text do you want to use?");
			return;
		}

		var replacement = command.SafeRemainingArgument;
		var plan = EditableItemNameBulkRenamePlanner.CreatePlan(
			items,
			pattern,
			replacement,
			idSelector,
			revisionSelector,
			statusSelector,
			nameSelector,
			scopeSelector,
			normaliseName);

		var output = new StringBuilder();
		if (plan.Entries.Count > 0)
		{
			var headers = plan.Entries.Any(x => x.RevisionNumber.HasValue)
				? new[] { "ID", "Revision", "Status", "Old Name", "New Name" }
				: new[] { "ID", "Old Name", "New Name" };
			var rows = plan.Entries.Select(entry => entry.RevisionNumber.HasValue
				? new[]
				{
					entry.Id.ToString("N0", actor),
					entry.RevisionNumber.Value.ToString("N0", actor),
					entry.Status?.Describe() ?? string.Empty,
					entry.OldName,
					entry.NewName
				}
				: new[]
				{
					entry.Id.ToString("N0", actor),
					entry.OldName,
					entry.NewName
				});
			output.AppendLine(StringUtilities.GetTextTable(
				rows,
				headers,
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode));
		}

		if (!plan.IsValid)
		{
			output.AppendLine("The bulk rename cannot proceed:".ColourError());
			foreach (var error in plan.Errors)
			{
				output.AppendLine($"\t{error.ColourError()}");
			}

			output.AppendLine($"No {itemNamePlural} were renamed.");
			actor.OutputHandler.Send(output.ToString());
			return;
		}

		if (plan.Entries.Count == 0)
		{
			actor.OutputHandler.Send($"The regular expression did not match the name of any active {itemName}.");
			return;
		}

		plan.TryApply(applyName, out var changedCount);
		output.AppendLine(
			$"Matched {plan.Entries.Count.ToString("N0", actor)} {itemNamePlural} and renamed {changedCount.ToString("N0", actor)}.");
		actor.OutputHandler.Send(output.ToString());
	}
}
