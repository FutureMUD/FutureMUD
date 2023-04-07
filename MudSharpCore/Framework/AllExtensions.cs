#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Framework;

public static class AllExtensions
{
	public static List<ITag> FindMatchingTags(this IUneditableAll<ITag> tags, string keyword)
	{
		if (long.TryParse(keyword, out var id))
		{
			if (tags.Has(id))
			{
				return new List<ITag> { tags.Get(id)! };
			}

			return new List<ITag>();
		}

		var results = tags.Where(x => x.Name.EqualTo(keyword)).ToList();
		if (!results.Any())
		{
			results = tags.Where(x => x.FullName.EndsWith(keyword, System.StringComparison.InvariantCultureIgnoreCase))
			              .ToList();
		}

		if (!results.Any())
		{
			results = tags.Where(x => x.Name.StartsWith(keyword, System.StringComparison.InvariantCultureIgnoreCase))
			              .ToList();
		}

		return results;
	}
}