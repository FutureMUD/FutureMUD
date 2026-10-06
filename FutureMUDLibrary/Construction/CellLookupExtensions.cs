using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Construction;

public static class CellLookupExtensions
{
	public const int MaximumUniqueNameLength = 255;

	public static string? NormaliseUniqueName(string? value) =>
		string.IsNullOrWhiteSpace(value) ? null : value.Trim();

	public static bool IsValidUniqueName(string? value)
	{
		var name = NormaliseUniqueName(value);
		return name is null || (name.Length <= MaximumUniqueNameLength && !long.TryParse(name, out _));
	}

	public static ICell? FindByUniqueName(this IEnumerable<ICell> cells, string? value)
	{
		var name = NormaliseUniqueName(value);
		if (name is null) return null;
		var matches = cells.Where(x => name.Equals(x.UniqueName, StringComparison.InvariantCultureIgnoreCase))
			.GroupBy(x => x.Id).Select(x => x.First()).Take(2).ToArray();
		if (matches.Length > 1)
			throw new InvalidOperationException($"Cell unique name '{name}' is ambiguous between cell IDs {matches[0].Id} and {matches[1].Id}.");
		return matches.FirstOrDefault();
	}

	public static ICell? GetUniqueNameConflict(this IEnumerable<ICell> cells, string? value, long ownId)
	{
		var name = NormaliseUniqueName(value);
		return name is null ? null : cells.FirstOrDefault(x => x.Id != ownId &&
			name.Equals(x.UniqueName, StringComparison.InvariantCultureIgnoreCase));
	}

	/// <summary>Numeric IDs retain precedence; keys are exact; the collection owns legacy name matching.</summary>
	public static ICell? GetByIdOrUniqueNameOrName(this IUneditableAll<ICell> cells, string value,
		bool permitAbbreviations = true)
	{
		if (long.TryParse(value, out var id)) return cells.Get(id);
		return cells.FindByUniqueName(value) ?? cells.GetByIdOrName(value, permitAbbreviations);
	}
}
