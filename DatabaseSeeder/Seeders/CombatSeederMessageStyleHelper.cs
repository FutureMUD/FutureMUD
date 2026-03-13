#nullable enable

using System;

namespace DatabaseSeeder.Seeders;

internal enum SeedCombatMessageStyle
{
	Compact,
	Sentences,
	Sparse
}

internal enum SeedCombatHitVerb
{
	GetHit,
	BeHit
}

internal static class CombatSeederMessageStyleHelper
{
	internal static SeedCombatMessageStyle Parse(string? value)
	{
		return value?.Trim().ToLowerInvariant() switch
		{
			"sparse" => SeedCombatMessageStyle.Sparse,
			"sentences" => SeedCombatMessageStyle.Sentences,
			_ => SeedCombatMessageStyle.Compact
		};
	}

	internal static string AttackSuffix(SeedCombatMessageStyle style)
	{
		return style == SeedCombatMessageStyle.Compact ? "" : ".";
	}

	internal static string FormatAttackMessage(string message, SeedCombatMessageStyle style)
	{
		return style == SeedCombatMessageStyle.Compact
			? TrimTerminalPunctuation(message)
			: EnsureTerminalPunctuation(message, '.');
	}

	internal static string FormatStandaloneMessage(string message)
	{
		return EnsureTerminalPunctuation(message, '.');
	}

	internal static string BuildDefenseSuccess(SeedCombatMessageStyle style, string clause)
	{
		return $"{SuccessPrefix(style)}{TrimLeadingPunctuation(clause)}";
	}

	internal static string BuildDefenseFailure(SeedCombatMessageStyle style, string clause, string hitClause,
		SeedCombatHitVerb hitVerb = SeedCombatHitVerb.GetHit)
	{
		var trimmedClause = TrimLeadingPunctuation(clause);
		var trimmedHitClause = TrimLeadingPunctuation(hitClause);
		return style switch
		{
			SeedCombatMessageStyle.Compact =>
				$", and {trimmedClause} {CompactClauseJoiner(hitVerb)}{trimmedHitClause}",
			SeedCombatMessageStyle.Sparse =>
				$".\n{trimmedClause}\n#1 {SparseOrSentenceHitConnector(hitVerb)}{trimmedHitClause}",
			_ =>
				$". {trimmedClause}. #1 {SparseOrSentenceHitConnector(hitVerb)}{trimmedHitClause}"
		};
	}

	internal static string SuccessPrefix(SeedCombatMessageStyle style)
	{
		return style switch
		{
			SeedCombatMessageStyle.Compact => ", but ",
			SeedCombatMessageStyle.Sparse => ".\n",
			_ => ". "
		};
	}

	internal static string FailurePrefix(SeedCombatMessageStyle style)
	{
		return style switch
		{
			SeedCombatMessageStyle.Compact => ", and ",
			SeedCombatMessageStyle.Sparse => ".\n",
			_ => ". "
		};
	}

	private static string CompactClauseJoiner(SeedCombatHitVerb hitVerb)
	{
		return hitVerb switch
		{
			SeedCombatHitVerb.BeHit => "and %1|are|is ",
			_ => "but %1|get|gets "
		};
	}

	private static string SparseOrSentenceHitConnector(SeedCombatHitVerb hitVerb)
	{
		return hitVerb switch
		{
			SeedCombatHitVerb.BeHit => "%1|are|is ",
			_ => "%1|get|gets "
		};
	}

	private static string EnsureTerminalPunctuation(string message, char punctuation)
	{
		var trimmed = message.TrimEnd();
		if (trimmed.Length == 0)
		{
			return trimmed;
		}

		return EndsWithTerminalPunctuation(trimmed)
			? trimmed
			: $"{trimmed}{punctuation}";
	}

	private static string TrimTerminalPunctuation(string message)
	{
		return message.TrimEnd(' ', '.', '!', '?');
	}

	private static string TrimLeadingPunctuation(string message)
	{
		return message.TrimStart(' ', '.', ',', '\t', '\r', '\n');
	}

	private static bool EndsWithTerminalPunctuation(string message)
	{
		return message.EndsWith('.')
			|| message.EndsWith('!')
			|| message.EndsWith('?');
	}
}
