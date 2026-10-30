#nullable enable

using System;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.GameItems;

public static class KeywordDescriptionMatcher
{
	public static bool Matches(string keywords, string query)
	{
		if (string.IsNullOrWhiteSpace(keywords) || string.IsNullOrWhiteSpace(query)) return false;
		var alternatives = keywords.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		return query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).All(alternatives.Contains);
	}
}
