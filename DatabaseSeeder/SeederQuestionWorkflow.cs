#nullable enable

using MudSharp.Database;
using System.Collections.Generic;

namespace DatabaseSeeder;

internal readonly record struct SeederQuestionValidationResult(bool Success, string Error);

internal static class SeederQuestionWorkflow
{
	internal static bool IsActive(
		SeederQuestion question,
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return question.Filter(context, answers);
	}

	internal static SeederQuestionValidationResult Validate(
		SeederQuestion question,
		string answer,
		FuturemudDatabaseContext context)
	{
		(bool success, string error) = question.Validator(answer, context);
		return new SeederQuestionValidationResult(success, error);
	}
}
