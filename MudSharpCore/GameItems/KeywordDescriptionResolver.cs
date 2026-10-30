#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace MudSharp.GameItems;

public static class KeywordDescriptionResolver
{
	public static KeywordDescription? Find(string query, bool ordinaryTargetResolved,
		IEnumerable<(bool IsVisible, IEnumerable<IKeywordDescriptionProvider> Providers)> targets)
	{
		if (ordinaryTargetResolved) return null;
		return targets.Where(x => x.IsVisible)
			.SelectMany(x => x.Providers)
			.SelectMany(x => x.KeywordDescriptions)
			.FirstOrDefault(x => KeywordDescriptionMatcher.Matches(x.Keywords, query));
	}
}
