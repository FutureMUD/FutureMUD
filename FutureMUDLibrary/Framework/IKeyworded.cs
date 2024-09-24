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

	public bool HasKeyword(string targetKeyword, IPerceiver voyeur, bool abbreviated = false, bool useContainsOverStartsWith = false)
	{
		var keywords = GetKeywordsFor(voyeur).ToArray();
		if (!abbreviated)
		{
			return keywords.Any(x => x.EqualTo(targetKeyword));
		}

		if (useContainsOverStartsWith)
		{
			return keywords.Any(x => x.Contains(targetKeyword, StringComparison.InvariantCultureIgnoreCase));
		}

		return keywords.Any(x => x.StartsWith(targetKeyword, StringComparison.InvariantCultureIgnoreCase));

	}

	public bool HasKeywords(IEnumerable<string> targetKeywords, IPerceiver voyeur, bool abbreviated = false, bool useContainsOverStartsWith = false)
	{
		var keywords = GetKeywordsFor(voyeur).ToArray();
		if (!abbreviated)
		{
			return targetKeywords.All(x => keywords.Any(y => y.EqualTo(x)));
		}

		if (useContainsOverStartsWith)
		{
			return targetKeywords.All(x =>
				keywords.Any(y => y.Contains(x, StringComparison.InvariantCultureIgnoreCase)));
		}

		return targetKeywords.All(x =>
				keywords.Any(y => y.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)));

	}
}