#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Database;

namespace DatabaseSeeder;

public sealed record SeederQuestion(
	string Id,
	string Question,
	Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
	Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator,
	string? SharedAnswerKey = null,
	Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, string?>? DefaultAnswerResolver = null,
	bool AutoReuseLastAnswer = false)
{
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
