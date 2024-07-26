using System.Collections.Generic;

namespace MudSharp.Framework;

public interface IKeywordedItem : IFrameworkItem, IKeyworded {
	public new IEnumerable<string> Keywords => new ExplodedString(Name).Words;
}