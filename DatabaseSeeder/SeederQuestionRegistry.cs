#nullable enable

using DatabaseSeeder.Seeders;
using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

internal sealed record SeederQuestionEnhancement(
    string? SharedAnswerKey = null,
    Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, string?>? DefaultAnswerResolver = null,
    Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, ConsoleQuestionDisplay>? DisplayResolver = null,
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
            ),
            [BuildKey(nameof(EconomySeeder), "era")] = new(
                SharedAnswerKey: "economy-era",
                AutoReuseLastAnswer: true
            ),
            [BuildKey(nameof(EconomySeeder), "shopper-scale")] = new(
                SharedAnswerKey: "economy-shopper-scale",
                AutoReuseLastAnswer: true
            ),
            [BuildKey(nameof(CelestialSeeder), "sunepoch")] = new(
                DefaultAnswerResolver: CelestialSeeder.ResolveSunEpochDefault,
                DisplayResolver: CelestialSeeder.ResolveSunEpochDisplay
            ),
            [BuildKey(nameof(CelestialSeeder), "moonepoch")] = new(
                DefaultAnswerResolver: CelestialSeeder.ResolveMoonEpochDefault,
                DisplayResolver: CelestialSeeder.ResolveMoonEpochDisplay
            ),
            [BuildKey(nameof(CelestialSeeder), "gasgiantsunepoch")] = new(
                DefaultAnswerResolver: CelestialSeeder.ResolveGasGiantSunEpochDefault,
                DisplayResolver: CelestialSeeder.ResolveGasGiantSunEpochDisplay
            ),
            [BuildKey(nameof(CelestialSeeder), "gasgiantmoonepoch")] = new(
                DefaultAnswerResolver: CelestialSeeder.ResolveGasGiantMoonEpochDefault,
                DisplayResolver: CelestialSeeder.ResolveGasGiantMoonEpochDisplay
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
        if (!Enhancements.TryGetValue(BuildKey(seeder.GetType().Name, question.Id), out SeederQuestionEnhancement? enhancement))
        {
            return question;
        }

        return question with
        {
            SharedAnswerKey = enhancement.SharedAnswerKey,
            DefaultAnswerResolver = enhancement.DefaultAnswerResolver,
            DisplayResolver = enhancement.DisplayResolver,
            AutoReuseLastAnswer = enhancement.AutoReuseLastAnswer
        };
    }

    private static string BuildKey(string seederTypeName, string questionId)
    {
        return $"{seederTypeName}:{questionId}";
    }
}
