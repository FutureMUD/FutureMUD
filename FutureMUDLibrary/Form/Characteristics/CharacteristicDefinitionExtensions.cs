using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

#nullable enable

namespace MudSharp.Form.Characteristics;

public static class CharacteristicDefinitionExtensions
{
	public const int MaximumPatternInputLength = 256;
	public static readonly TimeSpan MaximumPatternMatchTime = TimeSpan.FromMilliseconds(25);
	private static readonly ConditionalWeakTable<Regex, Regex> _boundedPatterns = new();

	/// <summary>Matches an authored characteristic pattern without letting backtracking escape into the game loop.</summary>
	public static bool MatchesPattern(this ICharacteristicDefinition definition, string? input)
	{
		if (input is null || input.Length > MaximumPatternInputLength) return false;
		var pattern = definition.Pattern;
		if (pattern is null) return false;
		var bounded = pattern.MatchTimeout != Regex.InfiniteMatchTimeout && pattern.MatchTimeout <= MaximumPatternMatchTime
			? pattern
			: _boundedPatterns.GetValue(pattern, source => new Regex(source.ToString(), source.Options, MaximumPatternMatchTime));
		try
		{
			return bounded.IsMatch(input);
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
	}
}
