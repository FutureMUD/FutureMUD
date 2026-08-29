#if DEBUG
#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

internal sealed record SeederReplayAnswer(string Id, string Answer);

internal sealed record SeederReplayStep(Type SeederType, IReadOnlyList<SeederReplayAnswer> Answers);

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
	string? FailedSeeder,
	IReadOnlyList<string> UnstartedSeeders,
	string? Failure,
	Exception? Exception,
	SeederReplayValidationResult Validation)
{
	internal bool Success => Validation.IsValid && string.IsNullOrWhiteSpace(Failure) && Exception is null;
}

internal static class DebugSeederConnection
{
	internal const string DefaultConnectionString =
		"server=localhost;port=3307;database=demo_dbo;uid=futuremud;password=rpiengine2020;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=300000;";

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

internal static class SeederReplayRunner
{
	private static readonly Type[] ExcludedSeederTypes = [typeof(SkillSeeder)];

	internal static SeederReplayValidationResult Validate(SeederReplayProfile profile,
		IEnumerable<IDatabaseSeeder> seeders)
	{
		var errors = new List<string>();
		var seederList = seeders
			.Where(x => x.Enabled)
			.ToList();
		var seedersByType = seederList
			.GroupBy(x => x.GetType())
			.ToDictionary(x => x.Key, x => x.First());
		var dependencyPlan = SeederCatalogue.GetDependencyPlan(seederList);
		if (dependencyPlan.Errors.Any())
		{
			errors.AddRange(dependencyPlan.Errors.Select(x => $"Seeder dependency registry: {x}"));
			return new SeederReplayValidationResult(errors);
		}

		var expectedSeeders = dependencyPlan.OrderedSeeders
			.Where(x => !ExcludedSeederTypes.Contains(x.GetType()))
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
			if (!seedersByType.TryGetValue(step.SeederType, out IDatabaseSeeder? seeder))
			{
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

	internal static SeederReplayRunResult Run(
		SeederReplayProfile profile,
		IEnumerable<IDatabaseSeeder> seeders,
		Func<FuturemudDatabaseContext> contextFactory,
		Version version,
		Action<string>? progress = null)
	{
		var seederList = seeders
			.Where(x => x.Enabled)
			.ToList();
		var validation = Validate(profile, seederList);
		if (!validation.IsValid)
		{
			return FailureResult(profile, [], null, null, "The selected replay profile is no longer valid.", null,
				validation);
		}

		try
		{
			using var preflightContext = contextFactory();
			if (preflightContext.Accounts.Any() || preflightContext.SeederChoices.Any())
			{
				return FailureResult(profile, [], null, null,
					"Replay profiles require a freshly migrated, unseeded database. This database already has bootstrap or seeder-answer data.",
					null, validation);
			}
		}
		catch (Exception exception)
		{
			return FailureResult(profile, [], null, null, "Could not inspect the target database before replay.", exception,
				validation);
		}

		var seedersByType = seederList.ToDictionary(x => x.GetType());
		var completedSeeders = new List<string>();
		foreach (SeederReplayStep step in profile.Steps)
		{
			IDatabaseSeeder seeder = seedersByType[step.SeederType];
			progress?.Invoke($"Running {seeder.Name}...");
			try
			{
				using FuturemudDatabaseContext context = contextFactory();
				SeederAssessment assessment = seeder.AssessSeedData(context);
				if (assessment.Status == SeederAssessmentStatus.Blocked)
				{
					return FailureResult(profile, completedSeeders, seeder.Name, step,
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
						return FailureResult(profile, completedSeeders, seeder.Name, step,
							$"{seeder.Name} needs an answer for active question {question.Id}.", null, validation);
					}

					SeederQuestionValidationResult answerValidation =
						SeederQuestionWorkflow.Validate(question, answer, context);
					if (!answerValidation.Success)
					{
						return FailureResult(profile, completedSeeders, seeder.Name, step,
							$"{seeder.Name} answer for {question.Id} is no longer valid: {answerValidation.Error}", null,
							validation);
					}

					answers[question.Id] = answer;
				}

				SeederExecutionResult execution = SeederExecutionService.Execute(context, seeder, questions, answers, version);
				if (!execution.Success)
				{
					return FailureResult(profile, completedSeeders, seeder.Name, step,
						$"{seeder.Name} failed during replay.", execution.Exception, validation);
				}

				completedSeeders.Add(seeder.Name);
			}
			catch (Exception exception)
			{
				return FailureResult(profile, completedSeeders, seeder.Name, step,
					$"{seeder.Name} could not be prepared for replay.", exception, validation);
			}
		}

		return new SeederReplayRunResult(profile, completedSeeders, null, [], null, null, validation);
	}

	private static SeederReplayRunResult FailureResult(
		SeederReplayProfile profile,
		IReadOnlyList<string> completedSeeders,
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
			failedSeeder,
			unstartedSteps.Select(x => x.SeederType.Name).ToList(),
			failure,
			exception,
			validation);
	}
}

internal static class DebugSeederReplayProfiles
{
	private const string DebugPassword = "DebugReplayOnly!2026";

	internal static IReadOnlyList<SeederReplayProfile> All { get; } =
	[
		CreateProfile(
			"medieval-standard",
			"Medieval Standard",
			"Full Debug replay with medieval time, health, culture and item content.",
			"Debug Medieval",
			"1300",
			"Medieval Age",
			"medieval",
			"medieval",
			"earthdarkagesandmedieval"),
		CreateProfile(
			"renaissance-standard",
			"Renaissance Standard",
			"Full Debug replay with cumulative medieval and Renaissance item content.",
			"Debug Renaissance",
			"1500",
			"Medieval Age",
			"renaissance",
			"medieval renaissance",
			"earthrenaissanceeurope"),
		CreateProfile(
			"early-modern-standard",
			"Early Modern Standard",
			"Full Debug replay based on the Europe/1703 DemoMUD-style configuration.",
			"Debug Early Modern",
			"1703",
			"Early Modern Age",
			"earlymodern",
			"medieval renaissance earlymodern",
			"earthrenaissanceeurope")
	];

	private static SeederReplayProfile CreateProfile(
		string id,
		string name,
		string description,
		string gameName,
		string year,
		string economyEra,
		string healthTechLevel,
		string itemEras,
		string culturePack)
	{
		var epoch = $"1 January {year}";
		var moonEpoch = $"21 January {year}";
		return new SeederReplayProfile(id, name, description,
		[
			Step<CoreDataSeeder>(
				("gamename", gameName),
				("account", "admin"),
				("password", DebugPassword),
				("email", "debug-replay@futuremud.com")),
			Step<TimeSeeder>(
				("secondsmultiplier", "2"),
				("mode", "gregorian-uk"),
				("startyear", year),
				("ardaage", "3")),
			Step<CelestialSeeder>(
				("installsun", "yes"),
				("suncalendar", "1"),
				("sunname", "The Sun"),
				("sunepoch", epoch),
				("installmoon", "yes"),
				("mooncalendar", "1"),
				("moonname", "The Moon"),
				("moonepoch", moonEpoch),
				("installgasgiantmoon", "yes"),
				("gasgiantcalendar", "1"),
				("gasgiantsunepoch", epoch),
				("gasgiantmoonepoch", epoch)),
			Step<AttributeSeeder>(
				("choice", "labmud"),
				("decorator", "labmud")),
			Step<CurrencySeeder>(("currency", "pounds")),
			Step<AIStorytellerSeeder>(("install", "yes")),
			Step<SkillPackageSeeder>(
				("branching", "yes"),
				("skillcapmodel", "rpi"),
				("skillgainmodel", "labmud"),
				("complexity", "complex"),
				("gerund", "no"),
				("modern", "yes")),
			Step<ClanSeeder>(),
			Step<HumanSeeder>(
				("balance", "combat-rebalance"),
				("model", "full"),
				("inventory", "hands"),
				("sever", "yes"),
				("bones", "full"),
				("distinctive", "yes"),
				("nonbinary", "yes"),
				("includeextraperson", "yes")),
			Step<WeatherSeeder>(("rain", "full")),
			Step<LawSeeder>(
				("name", "Debug Authority"),
				("currency", "1"),
				("createai", "yes"),
				("separatepowers", "yes"),
				("punishmentlevel", "western"),
				("classes", "immune sovereign noble officer soldier enforcer citizen non-citizen slave pet felon criminal"),
				("religiouslaws", "yes"),
				("penaltyunits", "4")),
			Step<ChargenSeeder>(
				("rpp", "yes"),
				("rppname", "Roleplay Point/RPP"),
				("bp", "yes"),
				("class", "no"),
				("subclass", "no"),
				("role-first", "race"),
				("attributemode", "points"),
				("skillmode", "boosts"),
				("merits", "merit"),
				("customdescs", "no")),
			Step<UsefulSeeder>(
				("ai", "yes"),
				("covers", "yes"),
				("items", "yes"),
				("modernitems", "yes"),
				("tags", "yes"),
				("autobuilder", "yes"),
				("hints", "yes"),
				("dreams", "yes"),
				("dream-eras", "old modern")),
			Step<TrapSeeder>(),
			Step<EconomySeeder>(
				("era", economyEra),
				("currency", "1"),
				("zone", "1"),
				("shopper-scale", "standard")),
			Step<CombatSeeder>(
				("installmuskets", "yes"),
				("installguns", "yes"),
				("random", "static"),
				("parryoption", "yes"),
				("skilloption", "weapons"),
				("messagestyle", "sparse")),
			Step<CultureSeeder>(
				("culturepacks", culturePack),
				("seednames", "yes"),
				("seedlanguages", "yes"),
				("seedheritage", "yes")),
			Step<StockMeritsSeeder>(),
			Step<AgricultureSeeder>(),
			Step<HealthSeeder>(("techlevel", healthTechLevel)),
			Step<CookingSeeder>(),
			Step<ArenaSeeder>(
				("arena-name", $"{gameName} Arena"),
				("arena-zone", "1")),
			Step<AnimalSeeder>(
				("model", "full"),
				("random", "static"),
				("messagestyle", "sparse")),
			Step<MythicalAnimalSeeder>(
				("model", "full"),
				("random", "static"),
				("messagestyle", "sparse")),
			Step<WildlifeCatalogueSeeder>(),
			Step<RobotSeeder>(),
			Step<ItemSeeder>(
				("eras", itemEras),
				("scope", "all")),
			Step<AnimalButcherySeeder>(),
			Step<SupernaturalSeeder>(
				("model", "full"),
				("random", "static"),
				("messagestyle", "sparse"))
		]);
	}

	private static SeederReplayStep Step<T>(params (string Id, string Answer)[] answers)
		where T : IDatabaseSeeder
	{
		return new SeederReplayStep(typeof(T), answers
			.Select(x => new SeederReplayAnswer(x.Id, x.Answer))
			.ToList());
	}
}
#endif
