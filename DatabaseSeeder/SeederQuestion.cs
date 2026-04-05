#nullable enable

using MudSharp.Database;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder;

public sealed record SeederQuestion(
    string Id,
    string Question,
    Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
    Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator,
    string? SharedAnswerKey = null,
    Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, string?>? DefaultAnswerResolver = null,
    Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, ConsoleQuestionDisplay>? DisplayResolver = null,
    bool AutoReuseLastAnswer = false)
{
    public ConsoleQuestionDisplay ResolveDisplay(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> currentAnswers)
    {
        return DisplayResolver?.Invoke(context, currentAnswers) ?? new ConsoleQuestionDisplay(Question);
    }

    public static SeederQuestion FromLegacy(
        (string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
            Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator) legacyQuestion)
    {
        return new SeederQuestion(
            legacyQuestion.Id,
            legacyQuestion.Question,
            legacyQuestion.Filter,
            legacyQuestion.Validator
        );
    }
}
