using System;
using System.Collections.Generic;
using System.Linq;

// Sourced from http://stackoverflow.com/a/1287572
// Author: Jon Skeet

namespace MudSharp.ThirdPartyCode;

public static class ShuffleExtension
{
	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
	{
		var elements = source.ToArray();
		for (var i = elements.Length - 1; i >= 0; i--)
		{
			// Swap element "i" with a random earlier element it (or itself)
			// ... except we don't really need to swap it fully, as we can
			// return it immediately, and afterwards it's irrelevant.
			var swapIndex = rng.Next(i + 1);
			yield return elements[swapIndex];
			elements[swapIndex] = elements[i];
		}
	}
}