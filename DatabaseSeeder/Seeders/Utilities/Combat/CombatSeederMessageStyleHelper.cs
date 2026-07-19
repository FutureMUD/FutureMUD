#nullable enable

using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private static readonly string[] CombatSeederNames = ["Combat", "Combat Seeder"];
    private const string CombatSeederMessageStyleChoice = "messagestyle";

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

    internal static string? GetRecordedChoice(FuturemudDatabaseContext context)
    {
        return context.SeederChoices
            .Where(x => CombatSeederNames.Contains(x.Seeder) && x.Choice == CombatSeederMessageStyleChoice)
            .OrderByDescending(x => x.DateTime)
            .ThenByDescending(x => x.Id)
            .Select(x => x.Answer)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
    }

    internal static IReadOnlyDictionary<string, string> MergeQuestionAnswersWithRecordedChoice(
        FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        if (questionAnswers.ContainsKey(CombatSeederMessageStyleChoice))
        {
            return questionAnswers;
        }

        string? recordedChoice = GetRecordedChoice(context);
        if (string.IsNullOrWhiteSpace(recordedChoice))
        {
            return questionAnswers;
        }

        Dictionary<string, string> effectiveAnswers = new(questionAnswers, StringComparer.OrdinalIgnoreCase)
        {
            [CombatSeederMessageStyleChoice] = recordedChoice
        };
        return effectiveAnswers;
    }

    internal static string BuildDefenseSuccess(SeedCombatMessageStyle style, string clause)
    {
        return $"{SuccessPrefix(style)}{TrimLeadingPunctuation(clause)}";
    }

    internal static string BuildDefenseFailure(SeedCombatMessageStyle style, string clause, string hitClause,
        SeedCombatHitVerb hitVerb = SeedCombatHitVerb.GetHit)
    {
        string trimmedClause = TrimLeadingPunctuation(clause);
        string trimmedHitClause = TrimLeadingPunctuation(hitClause);
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
        string trimmed = message.TrimEnd();
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
