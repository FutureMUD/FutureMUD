#nullable enable

namespace MudSharp.Character;

/// <summary>The reserved marker is parsed before any target, currency or emote resolution.</summary>
public static class GuardChallengeSyntax
{
	public static bool TryExtract(string input, out string command, out bool requested, out string error, int minimumWords = 2)
	{
		command = input;
		requested = false;
		error = "Place one !! immediately after the target, before any parenthesised emote.";
		if (!input.Contains("!!", StringComparison.Ordinal)) return true;
		var ss = new StringStack(input.Trim());
		var words = 0;
		while (!ss.IsFinished)
		{
			var before = ss.RemainingArgument;
			if (before.StartsWith('(')) return false;
			var word = ss.PopSpeech();
			var raw = before[..(before.Length - ss.RemainingArgument.Length)].Trim();
			if (!word.Contains("!!", StringComparison.Ordinal))
			{
				words++;
				continue;
			}
			if (word != "!!" || raw != "!!" || words < minimumWords) return false;
			var tail = ss.RemainingArgument.Trim();
			if (tail.Contains("!!", StringComparison.Ordinal) ||
			    tail.Length > 0 && (!tail.StartsWith('(') || !tail.EndsWith(')'))) return false;
			var trimmed = input.Trim();
			command = trimmed[..(trimmed.Length - before.Length)].TrimEnd() +
			          (tail.Length == 0 ? string.Empty : " " + tail);
			requested = true;
			return true;
		}
		return false;
	}
}
