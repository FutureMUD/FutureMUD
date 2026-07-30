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

internal sealed record PrototypeUniqueNameRenameEntry<T>(
	T Prototype,
	long Id,
	int RevisionNumber,
	RevisionStatus Status,
	string OldName,
	string? NewName)
{
	public bool ChangesName => !string.Equals(OldName, NewName, StringComparison.Ordinal);
}

internal sealed class PrototypeUniqueNameRenamePlan<T>
{
	public PrototypeUniqueNameRenamePlan(
		IReadOnlyList<PrototypeUniqueNameRenameEntry<T>> entries,
		IReadOnlyList<string> errors)
	{
		Entries = entries;
		Errors = errors;
	}

	public IReadOnlyList<PrototypeUniqueNameRenameEntry<T>> Entries { get; }
	public IReadOnlyList<string> Errors { get; }
	public bool IsValid => Errors.Count == 0;

	public bool TryApply(Action<T, string?> applyRename, out int changedCount)
	{
		changedCount = 0;
		if (!IsValid)
		{
			return false;
		}

		foreach (var entry in Entries.Where(x => x.ChangesName))
		{
			applyRename(entry.Prototype, entry.NewName);
			changedCount++;
		}

		return true;
	}
}

internal static class PrototypeUniqueNameBulkRenamePlanner
{
	internal static readonly TimeSpan DefaultRegexTimeout = TimeSpan.FromSeconds(1);

	public static PrototypeUniqueNameRenamePlan<T> CreatePlan<T>(
		IEnumerable<T> prototypes,
		string pattern,
		string replacement,
		Func<T, long> idSelector,
		Func<T, int> revisionSelector,
		Func<T, RevisionStatus> statusSelector,
		Func<T, string?> uniqueNameSelector,
		Func<string?, string?> normaliseUniqueName,
		Func<string?, bool> isValidUniqueName,
		TimeSpan? regexTimeout = null)
	{
		var activePrototypes = prototypes
			.Where(x => IsActive(statusSelector(x)))
			.ToList();
		var entries = new List<PrototypeUniqueNameRenameEntry<T>>();
		var errors = new List<string>();
		Regex regex;

		try
		{
			regex = new Regex(pattern, RegexOptions.None, regexTimeout ?? DefaultRegexTimeout);
		}
		catch (ArgumentException ex)
		{
			errors.Add($"The match regular expression is invalid: {ex.Message}");
			return new PrototypeUniqueNameRenamePlan<T>(entries, errors);
		}

		var replacementError = ValidateReplacement(regex, replacement);
		if (replacementError is not null)
		{
			errors.Add(replacementError);
			return new PrototypeUniqueNameRenamePlan<T>(entries, errors);
		}

		foreach (var prototype in activePrototypes)
		{
			var oldName = uniqueNameSelector(prototype);
			if (string.IsNullOrWhiteSpace(oldName))
			{
				continue;
			}

			try
			{
				if (!regex.IsMatch(oldName))
				{
					continue;
				}

				var newName = normaliseUniqueName(regex.Replace(oldName, replacement));
				entries.Add(new PrototypeUniqueNameRenameEntry<T>(
					prototype,
					idSelector(prototype),
					revisionSelector(prototype),
					statusSelector(prototype),
					oldName,
					newName));
			}
			catch (RegexMatchTimeoutException)
			{
				errors.Add(
					$"The regular expression exceeded its {regex.MatchTimeout.TotalSeconds:N0}-second timeout while processing prototype #{idSelector(prototype)}r{revisionSelector(prototype)}.");
				return new PrototypeUniqueNameRenamePlan<T>(entries, errors);
			}
			catch (ArgumentException ex)
			{
				errors.Add($"The replacement expression is invalid: {ex.Message}");
				return new PrototypeUniqueNameRenamePlan<T>(entries, errors);
			}
		}

		foreach (var entry in entries.Where(x => !isValidUniqueName(x.NewName)))
		{
			errors.Add(
				$"Prototype #{entry.Id}r{entry.RevisionNumber} would have the entirely numeric unique name {entry.NewName}.");
		}

		var entriesByRevision = entries
			.ToDictionary(x => (x.Id, x.RevisionNumber));
		var finalNames = activePrototypes
			.Select(x =>
			{
				var id = idSelector(x);
				var revision = revisionSelector(x);
				entriesByRevision.TryGetValue((id, revision), out var entry);
				return new
				{
					Prototype = x,
					Id = id,
					Revision = revision,
					Status = statusSelector(x),
					Name = entry is null ? normaliseUniqueName(uniqueNameSelector(x)) : entry.NewName,
					IsTarget = entry is not null
				};
			})
			.Where(x => x.Name is not null)
			.GroupBy(x => x.Name!, StringComparer.InvariantCultureIgnoreCase)
			.Where(x => x.Any(y => y.IsTarget) && x.Select(y => y.Id).Distinct().Count() > 1);

		foreach (var conflict in finalNames)
		{
			var conflictingRevisions = conflict
				.OrderBy(x => x.Id)
				.ThenBy(x => x.Revision)
				.Select(x => $"#{x.Id}r{x.Revision} ({x.Status.Describe()})")
				.ListToString();
			errors.Add(
				$"The unique name {conflict.Key} would be shared by distinct prototype IDs: {conflictingRevisions}.");
		}

		return new PrototypeUniqueNameRenamePlan<T>(entries, errors);
	}

	private static bool IsActive(RevisionStatus status)
	{
		return status == RevisionStatus.Current ||
		       status == RevisionStatus.PendingRevision ||
		       status == RevisionStatus.UnderDesign;
	}

	private static string? ValidateReplacement(Regex regex, string replacement)
	{
		var groupNames = regex
			.GetGroupNames()
			.ToHashSet(StringComparer.Ordinal);
		var groupNumbers = regex
			.GetGroupNumbers()
			.ToHashSet();

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

internal static class PrototypeUniqueNameBulkRenameCommand
{
	public static void Execute(
		ICharacter actor,
		StringStack command,
		IEnumerable<IEditableUniqueName> prototypes,
		string commandName,
		string prototypeName,
		string prototypeNamePlural,
		Func<string?, string?> normaliseUniqueName,
		Func<string?, bool> isValidUniqueName)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What regular expression do you want to match? The syntax is {($"{commandName} rename <match regex> <replacement text>").ColourCommand()}.");
			return;
		}

		var pattern = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What replacement text do you want to use? Use an empty quoted string {("\"\"").ColourCommand()} to clear matching unique names.");
			return;
		}

		var replacement = command.SafeRemainingArgument;
		var prototypeList = prototypes.ToList();
		var plan = PrototypeUniqueNameBulkRenamePlanner.CreatePlan(
			prototypeList,
			pattern,
			replacement,
			x => x.Id,
			x => x.RevisionNumber,
			x => x.Status,
			x => x.UniqueName,
			normaliseUniqueName,
			isValidUniqueName);

		var output = new StringBuilder();
		if (plan.Entries.Count > 0)
		{
			output.AppendLine(StringUtilities.GetTextTable(
				plan.Entries.Select(x => new[]
				{
					x.Id.ToString("N0", actor),
					x.RevisionNumber.ToString("N0", actor),
					x.Status.Describe(),
					x.OldName,
					x.NewName ?? "None"
				}),
				new[] { "ID", "Revision", "Status", "Old Unique Name", "New Unique Name" },
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

			output.AppendLine($"No {prototypeNamePlural} were renamed.");
			actor.OutputHandler.Send(output.ToString());
			return;
		}

		if (plan.Entries.Count == 0)
		{
			actor.OutputHandler.Send(
				$"The regular expression did not match the unique name of any active {prototypeName}.");
			return;
		}

		plan.TryApply(
			(prototype, newName) => prototype.SetUniqueNameFromValidatedBulkRename(newName),
			out var changedCount);
		output.AppendLine(
			$"Matched {plan.Entries.Count.ToString("N0", actor)} {prototypeNamePlural} and renamed {changedCount.ToString("N0", actor)}.");
		actor.OutputHandler.Send(output.ToString());
	}
}
