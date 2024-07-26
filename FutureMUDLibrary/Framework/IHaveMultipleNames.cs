using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework;
public interface IHaveMultipleNames : IFrameworkItem
{
	IEnumerable<string> Names { get; }
}

#nullable enable
public static class MultipleNamesExtensions
{
	public static T? GetByIdOrNames<T>(this IEnumerable<T> items, string text) where T : IHaveMultipleNames
	{
		if (long.TryParse(text, out var id))
		{
			return items.FirstOrDefault(x => x.Id == id);
		}

		var itemList = items.ToList();
		return itemList.FirstOrDefault(x => x.Names.Any(y => y.EqualTo(text))) ??
		       itemList.FirstOrDefault(x => x.Names.Any(y => y.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)));
	}
}
