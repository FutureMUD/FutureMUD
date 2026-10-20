#nullable enable

using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.Character.Name;

/// <summary>Shared free-entry punctuation policy; typographic variants require explicit culture opt-in.</summary>
public static class NameEntryValidation
{
	private static readonly Regex OrdinaryConsecutive = new(@"['-]\s*['-]");
	private static readonly Regex TypographicConsecutive = new(@"['’‐-]\s*['’‐-]");

	public static bool HasInvalidCharacters(string name, bool typographic) => name.Any(c =>
		!char.IsLetter(c) && c != ' ' && c != '\'' && c != '-' && !(typographic && c is '’' or '‐'));

	public static bool HasConsecutivePunctuation(string name, bool typographic) =>
		(typographic ? TypographicConsecutive : OrdinaryConsecutive).IsMatch(name);
}
