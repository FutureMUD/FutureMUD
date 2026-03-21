#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using MudSharp.Database;

namespace DatabaseSeeder;

internal sealed record SeederQuestionEnhancement(
	string? SharedAnswerKey = null,
	Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, string?>? DefaultAnswerResolver = null,
	bool AutoReuseLastAnswer = false);

public static class SeederQuestionRegistry
{
	private static readonly IReadOnlyDictionary<string, SeederQuestionEnhancement> Enhancements =
		new Dictionary<string, SeederQuestionEnhancement>(StringComparer.OrdinalIgnoreCase)
		{
			[BuildKey(nameof(CombatSeeder), "messagestyle")] = new(
				SharedAnswerKey: "combat-message-style",
				DefaultAnswerResolver: (context, _) => CombatSeederMessageStyleHelper.GetRecordedChoice(context),
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(CombatSeeder), "random")] = new(
				SharedAnswerKey: "damage-randomness",
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(AnimalSeeder), "messagestyle")] = new(
				SharedAnswerKey: "combat-message-style",
				DefaultAnswerResolver: (context, _) => CombatSeederMessageStyleHelper.GetRecordedChoice(context),
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(AnimalSeeder), "random")] = new(
				SharedAnswerKey: "damage-randomness",
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(AnimalSeeder), "model")] = new(
				SharedAnswerKey: "nonhuman-health-model",
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(MythicalAnimalSeeder), "messagestyle")] = new(
				SharedAnswerKey: "combat-message-style",
				DefaultAnswerResolver: (context, _) => CombatSeederMessageStyleHelper.GetRecordedChoice(context),
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(MythicalAnimalSeeder), "random")] = new(
				SharedAnswerKey: "damage-randomness",
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(MythicalAnimalSeeder), "model")] = new(
				SharedAnswerKey: "nonhuman-health-model",
				AutoReuseLastAnswer: true
			),
			[BuildKey(nameof(HumanSeeder), "model")] = new(
				SharedAnswerKey: "human-health-model",
				AutoReuseLastAnswer: true
			)
		};

	public static IEnumerable<SeederQuestion> GetQuestions(
		IDatabaseSeeder seeder,
		IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> legacyQuestions)
	{
		return legacyQuestions
			.Select(SeederQuestion.FromLegacy)
			.Select(question => Enrich(seeder, question))
			.ToList();
	}

	private static SeederQuestion Enrich(IDatabaseSeeder seeder, SeederQuestion question)
	{
		if (!Enhancements.TryGetValue(BuildKey(seeder.GetType().Name, question.Id), out var enhancement))
		{
			return question;
		}

		return question with
		{
			SharedAnswerKey = enhancement.SharedAnswerKey,
			DefaultAnswerResolver = enhancement.DefaultAnswerResolver,
			AutoReuseLastAnswer = enhancement.AutoReuseLastAnswer
		};
	}

	private static string BuildKey(string seederTypeName, string questionId)
	{
		return $"{seederTypeName}:{questionId}";
	}
}
