using MudSharp.Framework.Revision;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.GameItems;

public static class GameItemProtoLookupExtensions
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

	public static IGameItemProto? FindByUniqueName(this IEnumerable<IGameItemProto> protos, string? uniqueName, bool activeOnly = true)
	{
		var normalised = NormaliseUniqueName(uniqueName);
		if (normalised is null)
		{
			return null;
		}

		var matches = protos
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

	public static IGameItemProto? GetByIdOrUniqueNameOrName(this IEnumerable<IGameItemProto> protos, string value, bool permitAbbreviations = true)
	{
		var list = protos.ToList();
		if (long.TryParse(value, out var id))
		{
			return BestCurrentMatch(list.Where(x => x.Id == id));
		}

		return list.FindByUniqueName(value) ?? GetByLegacyName(list, value, permitAbbreviations, preferEditing: false);
	}

	public static IGameItemProto? GetByIdOrUniqueNameOrNameForEditing(this IEnumerable<IGameItemProto> protos, string value)
	{
		var list = protos.ToList();
		if (long.TryParse(value, out var id))
		{
			return BestEditingMatch(list.Where(x => x.Id == id));
		}

		return FindByUniqueNameForEditing(list, value) ?? GetByLegacyName(list, value, permitAbbreviations: true, preferEditing: true);
	}

	public static IGameItemProto? GetByIdOrUniqueNameOrNameRevisable(this IEnumerable<IGameItemProto> protos, string value)
	{
		return protos.GetByIdOrUniqueNameOrName(value);
	}

	public static IGameItemProto? GetActiveUniqueNameConflict(this IEnumerable<IGameItemProto> protos, string? uniqueName, long ownId)
	{
		var normalised = NormaliseUniqueName(uniqueName);
		if (normalised is null)
		{
			return null;
		}

		return protos
		       .Where(x => x.Id != ownId)
		       .Where(x => x.Status.IsActiveForUniqueName())
		       .FirstOrDefault(x => x.UniqueName?.Equals(normalised, StringComparison.InvariantCultureIgnoreCase) == true);
	}

	public static IEnumerable<IGameItemProto> WithBuilderText(this IEnumerable<IGameItemProto> protos, string text)
	{
		return protos.Where(x => x.HasBuilderSearchText(text));
	}

	public static bool HasBuilderSearchText(this IGameItemProto proto, string text)
	{
		return proto.UniqueName?.Contains(text, StringComparison.InvariantCultureIgnoreCase) == true ||
		       proto.BuilderNotes?.Contains(text, StringComparison.InvariantCultureIgnoreCase) == true;
	}

	private static IGameItemProto? FindByUniqueNameForEditing(IEnumerable<IGameItemProto> protos, string value)
	{
		var normalised = NormaliseUniqueName(value);
		if (normalised is null)
		{
			return null;
		}

		var matches = protos
		              .Where(x => x.Status.IsActiveForUniqueName())
		              .Where(x => x.UniqueName?.Equals(normalised, StringComparison.InvariantCultureIgnoreCase) == true)
		              .ToList();
		return matches.FirstOrDefault(x => x.Status == RevisionStatus.UnderDesign) ??
		       matches.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision) ??
		       matches.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       (matches.Count > 0 ? matches.OrderByDescending(x => x.RevisionNumber).First() : null);
	}

	private static IGameItemProto? GetByLegacyName(IEnumerable<IGameItemProto> protos, string value, bool permitAbbreviations, bool preferEditing)
	{
		var list = protos
		           .Where(x => x.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
		           .ToList();
		if (permitAbbreviations && list.Count == 0)
		{
			list = protos
			       .Where(x => x.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
			       .ToList();
		}

		return preferEditing ? BestEditingMatch(list) : BestCurrentMatch(list);
	}

	private static IGameItemProto? BestCurrentMatch(IEnumerable<IGameItemProto> protos)
	{
		var list = protos.ToList();
		return list.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       list.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision || x.Status == RevisionStatus.UnderDesign) ??
		       (list.Count > 0 ? list.OrderByDescending(x => x.RevisionNumber).First() : null);
	}

	private static IGameItemProto? BestEditingMatch(IEnumerable<IGameItemProto> protos)
	{
		var list = protos.ToList();
		return list.FirstOrDefault(x => x.Status == RevisionStatus.UnderDesign) ??
		       list.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision) ??
		       list.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
		       (list.Count > 0 ? list.OrderByDescending(x => x.RevisionNumber).First() : null);
	}
}
