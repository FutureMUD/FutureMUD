using MudSharp.Framework.Revision;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.NPC.Templates;

public static class NPCTemplateLookupExtensions
{
	public static string? NormaliseUniqueName(string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
	}

	public static bool IsValidUniqueName(string? value)
	{
		var normalised = NormaliseUniqueName(value);
		return normalised is null || !long.TryParse(normalised, out _);
	}

	public static bool IsActiveForUniqueName(this RevisionStatus status)
	{
		return status == RevisionStatus.Current ||
		       status == RevisionStatus.PendingRevision ||
		       status == RevisionStatus.UnderDesign;
	}

	public static INPCTemplate? FindByUniqueName(this IEnumerable<INPCTemplate> templates, string? uniqueName, bool activeOnly = true)
	{
		var normalised = NormaliseUniqueName(uniqueName);
		if (normalised is null)
		{
			return null;
		}

		var matches = templates
		              .Where(x => x.UniqueName?.Equals(normalised, StringComparison.InvariantCultureIgnoreCase) == true);
		if (activeOnly)
		{
			matches = matches.Where(x => x.Status.IsActiveForUniqueName());
		}

		var list = matches.ToList();
		return list.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       list.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision || x.Status == RevisionStatus.UnderDesign) ??
		       (list.Count > 0 ? list.OrderByDescending(x => x.RevisionNumber).First() : null);
	}

	public static INPCTemplate? GetByIdOrUniqueNameOrName(this IEnumerable<INPCTemplate> templates, string value, bool permitAbbreviations = true)
	{
		var list = templates.ToList();
		if (long.TryParse(value, out var id))
		{
			return BestCurrentMatch(list.Where(x => x.Id == id));
		}

		return list.FindByUniqueName(value) ?? GetByLegacyName(list, value, permitAbbreviations, preferEditing: false);
	}

	public static INPCTemplate? GetByIdOrUniqueNameOrNameForEditing(this IEnumerable<INPCTemplate> templates, string value)
	{
		var list = templates.ToList();
		if (long.TryParse(value, out var id))
		{
			return BestEditingMatch(list.Where(x => x.Id == id));
		}

		return FindByUniqueNameForEditing(list, value) ?? GetByLegacyName(list, value, permitAbbreviations: true, preferEditing: true);
	}

	public static INPCTemplate? GetByIdOrUniqueNameOrNameRevisable(this IEnumerable<INPCTemplate> templates, string value)
	{
		return templates.GetByIdOrUniqueNameOrName(value);
	}

	public static INPCTemplate? GetActiveUniqueNameConflict(this IEnumerable<INPCTemplate> templates, string? uniqueName, long ownId)
	{
		var normalised = NormaliseUniqueName(uniqueName);
		if (normalised is null)
		{
			return null;
		}

		return templates
		       .Where(x => x.Id != ownId)
		       .Where(x => x.Status.IsActiveForUniqueName())
		       .FirstOrDefault(x => x.UniqueName?.Equals(normalised, StringComparison.InvariantCultureIgnoreCase) == true);
	}

	public static IEnumerable<INPCTemplate> WithBuilderText(this IEnumerable<INPCTemplate> templates, string text)
	{
		return templates.Where(x => x.HasBuilderSearchText(text));
	}

	public static bool HasBuilderSearchText(this INPCTemplate template, string text)
	{
		return template.UniqueName?.Contains(text, StringComparison.InvariantCultureIgnoreCase) == true ||
		       template.BuilderNotes?.Contains(text, StringComparison.InvariantCultureIgnoreCase) == true;
	}

	private static INPCTemplate? FindByUniqueNameForEditing(IEnumerable<INPCTemplate> templates, string value)
	{
		var normalised = NormaliseUniqueName(value);
		if (normalised is null)
		{
			return null;
		}

		var matches = templates
		              .Where(x => x.Status.IsActiveForUniqueName())
		              .Where(x => x.UniqueName?.Equals(normalised, StringComparison.InvariantCultureIgnoreCase) == true)
		              .ToList();
		return matches.FirstOrDefault(x => x.Status == RevisionStatus.UnderDesign) ??
		       matches.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision) ??
		       matches.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       (matches.Count > 0 ? matches.OrderByDescending(x => x.RevisionNumber).First() : null);
	}

	private static INPCTemplate? GetByLegacyName(IEnumerable<INPCTemplate> templates, string value, bool permitAbbreviations, bool preferEditing)
	{
		var list = templates
		           .Where(x => x.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
		           .ToList();
		if (permitAbbreviations && list.Count == 0)
		{
			list = templates
			       .Where(x => x.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
			       .ToList();
		}

		return preferEditing ? BestEditingMatch(list) : BestCurrentMatch(list);
	}

	private static INPCTemplate? BestCurrentMatch(IEnumerable<INPCTemplate> templates)
	{
		var list = templates.ToList();
		return list.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       list.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision || x.Status == RevisionStatus.UnderDesign) ??
		       (list.Count > 0 ? list.OrderByDescending(x => x.RevisionNumber).First() : null);
	}

	private static INPCTemplate? BestEditingMatch(IEnumerable<INPCTemplate> templates)
	{
		var list = templates.ToList();
		return list.FirstOrDefault(x => x.Status == RevisionStatus.UnderDesign) ??
		       list.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision) ??
		       list.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       (list.Count > 0 ? list.OrderByDescending(x => x.RevisionNumber).First() : null);
	}
}
