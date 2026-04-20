#nullable enable

using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal enum CombatBalanceProfile
{
	Stock,
	CombatRebalance
}

internal static class CombatBalanceProfileHelper
{
	internal const string QuestionId = "balance";
	internal const string SharedAnswerKey = "combat-balance-profile";
	private const string HumanSeederName = "Human Seeder";

	internal static CombatBalanceProfile Parse(string? value)
	{
		return value?.Trim().ToLowerInvariant() switch
		{
			"combat-rebalance" or "combat rebalance" or "rebalanced" or "rebalance" or "combatrebalance" =>
				CombatBalanceProfile.CombatRebalance,
			_ => CombatBalanceProfile.Stock
		};
	}

	internal static string ToAnswerValue(CombatBalanceProfile profile)
	{
		return profile == CombatBalanceProfile.CombatRebalance ? "combat-rebalance" : "stock";
	}

	internal static bool UsesCombatRebalance(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		return GetSelectedProfile(context, questionAnswers) == CombatBalanceProfile.CombatRebalance;
	}

	internal static CombatBalanceProfile GetSelectedProfile(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers.TryGetValue(QuestionId, out string? explicitAnswer) &&
		    !string.IsNullOrWhiteSpace(explicitAnswer))
		{
			return Parse(explicitAnswer);
		}

		return Parse(GetRecordedChoice(context));
	}

	internal static string? GetRecordedChoice(FuturemudDatabaseContext context)
	{
		return context.SeederChoices
			.Where(x =>
				(x.Seeder == "__shared__" && x.Choice == SharedAnswerKey) ||
				(x.Seeder == HumanSeederName && x.Choice == QuestionId))
			.OrderByDescending(x => x.DateTime)
			.ThenByDescending(x => x.Id)
			.Select(x => x.Answer)
			.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
	}

	internal static IReadOnlyDictionary<string, string> MergeQuestionAnswersWithRecordedChoice(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers.ContainsKey(QuestionId))
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
			[QuestionId] = recordedChoice
		};
		return effectiveAnswers;
	}
}
