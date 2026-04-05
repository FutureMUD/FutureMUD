#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

public static class SeederAnswerMemory
{
    private const string SharedSeederName = "__shared__";

    public static string? GetRememberedAnswer(
        FuturemudDatabaseContext context,
        IDatabaseSeeder seeder,
        SeederQuestion question,
        IReadOnlyDictionary<string, string> currentAnswers)
    {
        if (!string.IsNullOrWhiteSpace(question.SharedAnswerKey))
        {
            string? sharedAnswer = GetLatestSharedAnswer(context, question.SharedAnswerKey);
            if (!string.IsNullOrWhiteSpace(sharedAnswer))
            {
                return sharedAnswer;
            }
        }

        return question.DefaultAnswerResolver?.Invoke(context, currentAnswers);
    }

    public static void PersistAnswers(
        FuturemudDatabaseContext context,
        IDatabaseSeeder seeder,
        IEnumerable<SeederQuestion> questions,
        IReadOnlyDictionary<string, string> answers,
        string version,
        DateTime timestamp)
    {
        Dictionary<string, SeederQuestion> questionLookup = questions.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
        HashSet<string> persistedSharedKeys = new(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, string> answer in answers)
        {
            context.SeederChoices.Add(new SeederChoice
            {
                Version = version,
                Seeder = seeder.Name,
                Choice = answer.Key,
                Answer = answer.Value,
                DateTime = timestamp
            });

            if (!questionLookup.TryGetValue(answer.Key, out SeederQuestion? question) ||
                string.IsNullOrWhiteSpace(question.SharedAnswerKey) ||
                !persistedSharedKeys.Add(question.SharedAnswerKey))
            {
                continue;
            }

            context.SeederChoices.Add(new SeederChoice
            {
                Version = version,
                Seeder = SharedSeederName,
                Choice = question.SharedAnswerKey,
                Answer = answer.Value,
                DateTime = timestamp
            });
        }
    }

    private static string? GetLatestSharedAnswer(FuturemudDatabaseContext context, string sharedAnswerKey)
    {
        return context.SeederChoices
            .Where(x => x.Seeder == SharedSeederName && x.Choice == sharedAnswerKey)
            .OrderByDescending(x => x.DateTime)
            .ThenByDescending(x => x.Id)
            .Select(x => x.Answer)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
    }
}
