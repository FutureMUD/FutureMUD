using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Framework;

public interface IKeyworded {
	IEnumerable<string> Keywords { get; }

	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return Keywords;
	}

	public bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false)
	{
		var keywords = GetKeywordsFor(voyeur).ToArray();
		return 
			abbreviated ?
				keywords.Any(x => x.StartsWith(targetKeyword, StringComparison.InvariantCultureIgnoreCase)) :
				keywords.Any(x => x.EqualTo(targetKeyword));
	}

	public bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false)
	{
		var keywords = GetKeywordsFor(voyeur).ToArray();
		return abbreviated
			? targetKeywords.All(x =>
				keywords.Any(y => y.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
			: targetKeywords.All(x => keywords.Any(y => y.EqualTo(x)));
	}
}