#nullable enable

using MudSharp.Database;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder;

internal sealed record SeederExecutionResult(bool Success, string? Message, Exception? Exception)
{
	public static SeederExecutionResult Failed(Exception exception)
	{
		return new SeederExecutionResult(false, null, exception);
	}

	public static SeederExecutionResult Completed(string message)
	{
		return new SeederExecutionResult(true, message, null);
	}
}

internal static class SeederExecutionService
{
	internal static SeederExecutionResult Execute(
		FuturemudDatabaseContext context,
		IDatabaseSeeder seeder,
		IEnumerable<SeederQuestion> questions,
		IReadOnlyDictionary<string, string> answers,
		Version version)
	{
		try
		{
			var result = seeder.SeedData(context, answers);
			SeederAnswerMemory.PersistAnswers(context, seeder, questions, answers, version.ToString(), DateTime.UtcNow);
			context.SaveChanges();
			return SeederExecutionResult.Completed(result);
		}
		catch (Exception exception)
		{
			context.Database.CurrentTransaction?.Rollback();
			context.ChangeTracker.Clear();
			return SeederExecutionResult.Failed(exception);
		}
	}
}
