#if DEBUG
#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

internal sealed record SeederReplayAnswer(string Id, string Answer);

internal enum SeederReplayDisposition
{
	Execute,
	NotApplicable
}

internal sealed record SeederReplayStep(
	Type SeederType,
	IReadOnlyList<SeederReplayAnswer> Answers,
	SeederReplayDisposition Disposition = SeederReplayDisposition.Execute,
	string? NotApplicableReason = null,
	Func<FuturemudDatabaseContext, bool>? NotApplicableWhen = null);

internal sealed record SeederReplayProfile(
	string Id,
	string Name,
	string Description,
	IReadOnlyList<SeederReplayStep> Steps);

internal sealed record SeederReplayValidationResult(IReadOnlyList<string> Errors)
{
	internal bool IsValid => Errors.Count == 0;
}

internal sealed record SeederReplayRunResult(
	SeederReplayProfile Profile,
	IReadOnlyList<string> CompletedSeeders,
	IReadOnlyList<string> NotApplicableSeeders,
	string? FailedSeeder,
	IReadOnlyList<string> UnstartedSeeders,
	string? Failure,
	Exception? Exception,
	SeederReplayValidationResult Validation)
{
	internal SeederReplayRunResult(SeederReplayProfile profile, IReadOnlyList<string> completedSeeders,
		string? failedSeeder, IReadOnlyList<string> unstartedSeeders, string? failure, Exception? exception,
		SeederReplayValidationResult validation)
		: this(profile, completedSeeders, Array.Empty<string>(), failedSeeder, unstartedSeeders, failure, exception,
			validation)
	{
	}

	internal bool Success => Validation.IsValid && string.IsNullOrWhiteSpace(Failure) && Exception is null;
}

internal static class DebugSeederConnection
{
	internal const string DefaultConnectionString =
		"server=localhost;port=3307;database=demo_dbo;uid=futuremud;password=rpiengine2020;SslMode=Disabled;AllowPublicKeyRetrieval=True;Default Command Timeout=300000;";

	internal static bool TryCreateConnectionString(string? databaseName, out string connectionString,
		out string error)
	{
		connectionString = string.Empty;
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(databaseName))
		{
			error = "You must enter a database name.";
			return false;
		}

		try
		{
			var builder = new MySqlConnectionStringBuilder(DefaultConnectionString)
			{
				Database = databaseName.Trim()
			};
			connectionString = builder.ConnectionString;
			return true;
		}
		catch (Exception exception)
		{
			error = $"The database name could not be used: {exception.Message}";
			return false;
		}
	}

	internal static string GetDatabaseName(string connectionString)
	{
		return new MySqlConnectionStringBuilder(connectionString).Database;
	}
}

internal sealed class ProductReplayRunner(Type[] excludedSeederTypes, Func<FuturemudDatabaseContext, string?> inspectTarget)
{

	internal SeederReplayValidationResult Validate(SeederReplayProfile profile,
		IEnumerable<IDatabaseSeeder> seeders)
	{
		var errors = new List<string>();
		var seederList = seeders
			.Where(x => x.Enabled)
			.ToList();
		var seedersByType = seederList
			.GroupBy(x => x.GetType())
			.ToDictionary(x => x.Key, x => x.First());
		var dependencyPlan = SeederPackagePlan.GetDependencyPlan(seederList);
		if (dependencyPlan.Errors.Any())
		{
			errors.AddRange(dependencyPlan.Errors.Select(x => $"Seeder dependency registry: {x}"));
			return new SeederReplayValidationResult(errors);
		}

		var expectedSeeders = dependencyPlan.OrderedSeeders
			.Where(x => !excludedSeederTypes.Contains(x.GetType()))
			.ToList();
		var stepTypes = profile.Steps.Select(x => x.SeederType).ToList();
		var duplicateStepTypes = stepTypes
			.GroupBy(x => x)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key.Name)
			.ToList();
		if (duplicateStepTypes.Any())
		{
			errors.Add($"The replay profile contains duplicate seeder steps: {string.Join(", ", duplicateStepTypes)}.");
		}

		foreach (Type type in stepTypes.Where(x => !seedersByType.ContainsKey(x)).Distinct())
		{
			errors.Add($"The replay profile references unavailable or disabled seeder {type.Name}.");
		}

		var expectedTypes = expectedSeeders.Select(x => x.GetType()).ToList();
		if (!stepTypes.SequenceEqual(expectedTypes))
		{
			var missing = expectedTypes.Except(stepTypes).Select(x => x.Name).ToList();
			var unexpected = stepTypes.Except(expectedTypes).Select(x => x.Name).ToList();
			if (missing.Any())
			{
				errors.Add($"The replay profile is missing enabled seeders: {string.Join(", ", missing)}.");
			}

			if (unexpected.Any())
			{
				errors.Add($"The replay profile has unexpected seeders: {string.Join(", ", unexpected)}.");
			}

			if (!missing.Any() && !unexpected.Any())
			{
				errors.Add(
					$"The replay profile seeder order no longer matches the dependency plan. Expected: {string.Join(", ", expectedTypes.Select(x => x.Name))}.");
			}
		}

		foreach (SeederReplayStep step in profile.Steps)
		{
			if (step.Disposition == SeederReplayDisposition.NotApplicable)
			{
				if (string.IsNullOrWhiteSpace(step.NotApplicableReason))
				{
					errors.Add($"{step.SeederType.Name} is marked not applicable without an explicit reason.");
				}

				if (step.NotApplicableWhen is null)
				{
					errors.Add($"{step.SeederType.Name} is marked not applicable without a live-context predicate.");
				}
			}
			else if (step.NotApplicableReason is not null || step.NotApplicableWhen is not null)
			{
				errors.Add($"{step.SeederType.Name} has not-applicable metadata but is marked to execute.");
			}

			if (!seedersByType.TryGetValue(step.SeederType, out IDatabaseSeeder? seeder))
			{
				errors.Add($"The replay profile contains a step for unavailable or disabled seeder {step.SeederType.Name}.");
				continue;
			}

			var duplicateAnswerIds = step.Answers
				.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
				.Where(x => x.Count() > 1)
				.Select(x => x.Key)
				.ToList();
			if (duplicateAnswerIds.Any())
			{
				errors.Add($"{seeder.Name} has duplicate replay answers: {string.Join(", ", duplicateAnswerIds)}.");
				continue;
			}

			var expectedQuestionIds = seeder.Questions
				.Select(x => x.Id)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			var actualAnswerIds = step.Answers
				.Select(x => x.Id)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			var missingAnswers = expectedQuestionIds.Except(actualAnswerIds, StringComparer.OrdinalIgnoreCase).ToList();
			var extraAnswers = actualAnswerIds.Except(expectedQuestionIds, StringComparer.OrdinalIgnoreCase).ToList();
			if (missingAnswers.Any())
			{
				errors.Add($"{seeder.Name} is missing replay answers for: {string.Join(", ", missingAnswers)}.");
			}

			if (extraAnswers.Any())
			{
				errors.Add($"{seeder.Name} has answers for unknown questions: {string.Join(", ", extraAnswers)}.");
			}
		}

		return new SeederReplayValidationResult(errors);
	}

	internal SeederReplayRunResult Run(
		SeederReplayProfile profile,
		IEnumerable<IDatabaseSeeder> seeders,
		Func<FuturemudDatabaseContext> contextFactory,
		Version version,
		Action<string>? progress = null)
	{
		return RunInternal(profile, seeders, contextFactory, version, progress, null);
	}

	// Fixture preparation still validates the full inventory. This API only bounds execution;
	// it cannot accept an incomplete profile or resume a previously seeded target.
	internal SeederReplayRunResult RunPrefixBefore(
		SeederReplayProfile profile,
		IEnumerable<IDatabaseSeeder> seeders,
		Type stopBeforeSeederType,
		Func<FuturemudDatabaseContext> contextFactory,
		Version version,
		Action<string>? progress = null)
	{
		return RunInternal(profile, seeders, contextFactory, version, progress, stopBeforeSeederType);
	}

	private SeederReplayRunResult RunInternal(
		SeederReplayProfile profile,
		IEnumerable<IDatabaseSeeder> seeders,
		Func<FuturemudDatabaseContext> contextFactory,
		Version version,
		Action<string>? progress,
		Type? stopBeforeSeederType)
	{
		var seederList = seeders
			.Where(x => x.Enabled)
			.ToList();
		var validation = Validate(profile, seederList);
		if (!validation.IsValid)
		{
			return FailureResult(profile, [], [], null, null, "The selected replay profile is no longer valid.", null,
				validation);
		}

		if (stopBeforeSeederType is not null)
		{
			var boundary = profile.Steps.ToList().FindIndex(x => x.SeederType == stopBeforeSeederType);
			if (boundary <= 0)
			{
				validation = new SeederReplayValidationResult([
					"A replay prefix requires a declared boundary after at least one seeder step."
				]);
				return FailureResult(profile, [], [], null, null, "The selected replay prefix is invalid.", null, validation);
			}

			profile = profile with { Steps = profile.Steps.Take(boundary).ToList() };
		}

		try
		{
			using var preflightContext = contextFactory();
			if (inspectTarget(preflightContext) is { } failure)
			{
				return FailureResult(profile, [], [], null, null,
					failure,
					null, validation);
			}
		}
		catch (Exception exception)
		{
			return FailureResult(profile, [], [], null, null, "Could not inspect the target database before replay.", exception,
				validation);
		}

		var seedersByType = seederList.ToDictionary(x => x.GetType());
		var completedSeeders = new List<string>();
		var notApplicableSeeders = new List<string>();
		foreach (SeederReplayStep step in profile.Steps)
		{
			IDatabaseSeeder seeder = seedersByType[step.SeederType];

			progress?.Invoke($"Running {seeder.Name}...");
			try
			{
				using FuturemudDatabaseContext context = contextFactory();
				if (step.Disposition == SeederReplayDisposition.NotApplicable)
				{
					if (step.NotApplicableWhen?.Invoke(context) != true)
					{
						return FailureResult(profile, completedSeeders, notApplicableSeeders, seeder.Name, step,
							$"{seeder.Name} was marked not applicable by the profile but its live-context predicate did not pass.",
							null, validation);
					}

					notApplicableSeeders.Add(seeder.Name);
					progress?.Invoke($"Skipping {seeder.Name}: {step.NotApplicableReason}");
					continue;
				}

				SeederAssessment assessment = seeder.AssessSeedData(context);
				if (assessment.Status == SeederAssessmentStatus.Blocked)
				{
					return FailureResult(profile, completedSeeders, notApplicableSeeders, seeder.Name, step,
						$"{seeder.Name} is blocked: {assessment.Explanation}", null, validation);
				}

				var questions = seeder.Questions.ToList();
				var suppliedAnswers = step.Answers.ToDictionary(x => x.Id, x => x.Answer,
					StringComparer.OrdinalIgnoreCase);
				DictionaryWithDefault<string, string> answers = new();
				foreach (SeederQuestion question in questions)
				{
					if (!SeederQuestionWorkflow.IsActive(question, context, answers))
					{
						continue;
					}

					if (!suppliedAnswers.TryGetValue(question.Id, out string? answer))
					{
						return FailureResult(profile, completedSeeders, notApplicableSeeders, seeder.Name, step,
							$"{seeder.Name} needs an answer for active question {question.Id}.", null, validation);
					}

					SeederQuestionValidationResult answerValidation =
						SeederQuestionWorkflow.Validate(question, answer, context);
					if (!answerValidation.Success)
					{
						return FailureResult(profile, completedSeeders, notApplicableSeeders, seeder.Name, step,
							$"{seeder.Name} answer for {question.Id} is no longer valid: {answerValidation.Error}", null,
							validation);
					}

					answers[question.Id] = answer;
				}

				SeederExecutionResult execution = SeederExecutionService.Execute(context, seeder, questions, answers, version);
				if (!execution.Success)
				{
					return FailureResult(profile, completedSeeders, notApplicableSeeders, seeder.Name, step,
						$"{seeder.Name} failed during replay.", execution.Exception, validation);
				}

				completedSeeders.Add(seeder.Name);
			}
			catch (Exception exception)
			{
				return FailureResult(profile, completedSeeders, notApplicableSeeders, seeder.Name, step,
					$"{seeder.Name} could not be prepared for replay.", exception, validation);
			}
		}

		return new SeederReplayRunResult(profile, completedSeeders, notApplicableSeeders, null, [], null, null, validation);
	}

	private static SeederReplayRunResult FailureResult(
		SeederReplayProfile profile,
		IReadOnlyList<string> completedSeeders,
		IReadOnlyList<string> notApplicableSeeders,
		string? failedSeeder,
		SeederReplayStep? failedStep,
		string failure,
		Exception? exception,
		SeederReplayValidationResult validation)
	{
		IEnumerable<SeederReplayStep> unstartedSteps = failedStep is null
			? profile.Steps
			: profile.Steps
				.SkipWhile(x => !ReferenceEquals(x, failedStep))
				.Skip(1);
		return new SeederReplayRunResult(
			profile,
			completedSeeders,
			notApplicableSeeders,
			failedSeeder,
			unstartedSteps.Select(x => x.SeederType.Name).ToList(),
			failure,
			exception,
			validation);
	}
}
#endif
