using System;
using System.Collections.Generic;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Selected stock vocabulary; source indices, external grades and native power are distinct coordinates.</summary>
public static class ArmageddonPowerVocabulary
{
	public static IReadOnlyList<string> Words { get; } = Array.AsReadOnly(new[] { "wek", "yuqa", "kral", "een", "pav", "sul", "mon" });
	private static readonly SpellPower[] NativePowers = [SpellPower.ExtremelyWeak, SpellPower.VeryWeak, SpellPower.Weak,
		SpellPower.Standard, SpellPower.Strong, SpellPower.VeryStrong, SpellPower.ExtremelyStrong];
	public static int SourceIndex(int grade) => grade is >= 1 and <= 7 ? grade - 1 : throw new ArgumentOutOfRangeException(nameof(grade));
	public static int Grade(int sourceIndex) => sourceIndex is >= 0 and <= 6 ? sourceIndex + 1 : throw new ArgumentOutOfRangeException(nameof(sourceIndex));
	public static SpellPower NativePower(int grade) => NativePowers[SourceIndex(grade)];
	public static string Word(int grade) => Words[SourceIndex(grade)];
	public static bool TryParse(string? word, out int grade)
	{
		for (var i = 0; i < Words.Count; i++)
			if (string.Equals(Words[i], word, StringComparison.OrdinalIgnoreCase)) { grade = Grade(i); return true; }
		grade = 0;
		return false;
	}
}
