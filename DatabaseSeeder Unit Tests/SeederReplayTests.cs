#if DEBUG
#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeederReplayTests
{
	[TestMethod]
	public void StandardProfiles_AreCompleteAndMatchTheCurrentDependencyPlan()
	{
		IReadOnlyList<IDatabaseSeeder> seeders = SeederCatalogue.GetEnabledSeeders();
		Assert.IsTrue(seeders.Any(x => x.GetType() == typeof(SkillSeeder)));
		Assert.IsTrue(((IDatabaseSeeder)new UsefulSeeder()).Metadata.RequiredSeederTypes.Contains(typeof(HumanSeeder)));
		Assert.IsTrue(((IDatabaseSeeder)new AnimalSeeder()).Metadata.RequiredSeederTypes.Contains(typeof(CultureSeeder)));

		foreach (SeederReplayProfile profile in DebugSeederReplayProfiles.All)
		{
			SeederReplayValidationResult validation = SeederReplayRunner.Validate(profile, seeders);
			Assert.IsTrue(validation.IsValid,
				$"{profile.Id}:{Environment.NewLine}{string.Join(Environment.NewLine, validation.Errors)}");
			Assert.IsFalse(profile.Steps.Any(x => x.SeederType == typeof(SkillSeeder)));
			Assert.IsTrue(profile.Steps.Any(x => x.SeederType == typeof(SkillPackageSeeder)));
			var profileSteps = profile.Steps.ToList();
			Assert.IsTrue(profileSteps.FindIndex(x => x.SeederType == typeof(SkillPackageSeeder)) <
			              profileSteps.FindIndex(x => x.SeederType == typeof(TrapSeeder)));
		}
	}

	[TestMethod]
	public void ProfileValidation_RejectsOrderAndQuestionInventoryDrift()
	{
		SeederReplayProfile profile = DebugSeederReplayProfiles.All.First();
		IReadOnlyList<IDatabaseSeeder> seeders = SeederCatalogue.GetEnabledSeeders();
		var reorderedSteps = profile.Steps.ToList();
		(reorderedSteps[0], reorderedSteps[1]) = (reorderedSteps[1], reorderedSteps[0]);
		SeederReplayValidationResult reorderedValidation = SeederReplayRunner.Validate(
			profile with { Steps = reorderedSteps }, seeders);
		Assert.IsFalse(reorderedValidation.IsValid);
		Assert.IsTrue(reorderedValidation.Errors.Any(x => x.Contains("order", StringComparison.OrdinalIgnoreCase)));

		SeederReplayStep coreStep = profile.Steps.First(x => x.SeederType.Name == "CoreDataSeeder");
		SeederReplayStep missingAnswerStep = coreStep with
		{
			Answers = coreStep.Answers.Where(x => x.Id != "password").ToList()
		};
		var missingAnswerSteps = profile.Steps
			.Select(x => x == coreStep ? missingAnswerStep : x)
			.ToList();
		SeederReplayValidationResult missingAnswerValidation = SeederReplayRunner.Validate(
			profile with { Steps = missingAnswerSteps }, seeders);
		Assert.IsFalse(missingAnswerValidation.IsValid);
		Assert.IsTrue(missingAnswerValidation.Errors.Any(x => x.Contains("password", StringComparison.OrdinalIgnoreCase)));

		SeederReplayStep extraAnswerStep = coreStep with
		{
			Answers = [.. coreStep.Answers, new SeederReplayAnswer("removed-question", "yes")]
		};
		var extraAnswerSteps = profile.Steps
			.Select(x => x == coreStep ? extraAnswerStep : x)
			.ToList();
		SeederReplayValidationResult extraAnswerValidation = SeederReplayRunner.Validate(
			profile with { Steps = extraAnswerSteps }, seeders);
		Assert.IsFalse(extraAnswerValidation.IsValid);
		Assert.IsTrue(extraAnswerValidation.Errors.Any(x => x.Contains("removed-question", StringComparison.OrdinalIgnoreCase)));

		SeederReplayValidationResult duplicateValidation = SeederReplayRunner.Validate(profile with
		{
			Steps = [.. profile.Steps, profile.Steps.First()]
		}, seeders);
		Assert.IsFalse(duplicateValidation.IsValid);
		Assert.IsTrue(duplicateValidation.Errors.Any(x => x.Contains("duplicate", StringComparison.OrdinalIgnoreCase)));

		SeederReplayValidationResult removedSeederValidation = SeederReplayRunner.Validate(profile with
		{
			Steps = profile.Steps.Skip(1).ToList()
		}, seeders);
		Assert.IsFalse(removedSeederValidation.IsValid);
		Assert.IsTrue(removedSeederValidation.Errors.Any(x => x.Contains("missing enabled", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void ReplayProfiles_CoreBootstrapAnswersRemainValid()
	{
		foreach (SeederReplayProfile profile in DebugSeederReplayProfiles.All)
		{
			var databaseName = Guid.NewGuid().ToString("N");
			DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(databaseName)
				.Options;
			using var context = new FuturemudDatabaseContext(options);
			SeederReplayStep step = profile.Steps.Single(x => x.SeederType == typeof(CoreDataSeeder));
			var suppliedAnswers = step.Answers.ToDictionary(x => x.Id, x => x.Answer,
				StringComparer.OrdinalIgnoreCase);
			var priorAnswers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			IDatabaseSeeder seeder = new CoreDataSeeder();
			foreach (SeederQuestion question in seeder.Questions)
			{
				Assert.IsTrue(SeederQuestionWorkflow.IsActive(question, context, priorAnswers),
					$"{profile.Id} unexpectedly hid {question.Id}.");
				Assert.IsTrue(suppliedAnswers.TryGetValue(question.Id, out string? answer),
					$"{profile.Id} is missing {question.Id}.");
				SeederQuestionValidationResult answerValidation =
					SeederQuestionWorkflow.Validate(question, answer, context);
				Assert.IsTrue(answerValidation.Success,
					$"{profile.Id} answer for {question.Id} is invalid: {answerValidation.Error}");
				priorAnswers[question.Id] = answer;
			}
		}
	}

	[TestMethod]
	public void DebugConnection_ChangesOnlyTheDatabaseProperty()
	{
		Assert.IsFalse(DebugSeederConnection.TryCreateConnectionString("   ", out _, out string error));
		Assert.IsFalse(string.IsNullOrWhiteSpace(error));

		Assert.IsTrue(DebugSeederConnection.TryCreateConnectionString("replay-database.test", out string connectionString,
			out error), error);
		var builder = new MySqlConnectionStringBuilder(connectionString);
		var template = new MySqlConnectionStringBuilder(DebugSeederConnection.DefaultConnectionString);
		Assert.AreEqual("replay-database.test", builder.Database);
		Assert.AreEqual(template.Server, builder.Server);
		Assert.AreEqual(template.Port, builder.Port);
		Assert.AreEqual(template.UserID, builder.UserID);
		Assert.AreEqual(template.Password, builder.Password);
		Assert.AreEqual(template.SslMode, builder.SslMode);
		Assert.AreEqual(template.AllowPublicKeyRetrieval, builder.AllowPublicKeyRetrieval);
		Assert.AreEqual(template.DefaultCommandTimeout, builder.DefaultCommandTimeout);

		Assert.IsTrue(DebugSeederConnection.TryCreateConnectionString("replay;database=test", out connectionString,
			out error), error);
		builder = new MySqlConnectionStringBuilder(connectionString);
		Assert.AreEqual("replay;database=test", builder.Database);
		Assert.AreEqual("localhost", builder.Server);
	}

	[TestMethod]
	public void ReplayRunner_PersistsCompletedStepAndStopsAfterSeederFailure()
	{
		SuccessfulReplaySeeder.Reset();
		FailingReplaySeeder.Reset();
		LaterReplaySeeder.Reset();
		var databaseName = Guid.NewGuid().ToString("N");
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(databaseName)
			.Options;
		FuturemudDatabaseContext CreateContext() => new(options);

		var successfulSeeder = new SuccessfulReplaySeeder();
		var failingSeeder = new FailingReplaySeeder();
		var profile = new SeederReplayProfile("test", "Test", "Test profile",
		[
			new SeederReplayStep(typeof(SuccessfulReplaySeeder), [new SeederReplayAnswer("answer", "ok")]),
			new SeederReplayStep(typeof(FailingReplaySeeder), []),
			new SeederReplayStep(typeof(LaterReplaySeeder), [])
		]);

		SeederReplayRunResult result = SeederReplayRunner.Run(
			profile,
			[successfulSeeder, failingSeeder, new LaterReplaySeeder()],
			CreateContext,
			new Version(1, 0, 0));

		Assert.IsFalse(result.Success);
		Assert.AreEqual(1, result.CompletedSeeders.Count);
		Assert.AreEqual(1, SuccessfulReplaySeeder.RunCount);
		Assert.AreEqual(1, FailingReplaySeeder.RunCount);
		Assert.IsNotNull(result.Exception);
		Assert.AreEqual(failingSeeder.Name, result.FailedSeeder);
		Assert.AreEqual(0, LaterReplaySeeder.RunCount);
		CollectionAssert.AreEqual(new[] { nameof(LaterReplaySeeder) }, result.UnstartedSeeders.ToList());
		using FuturemudDatabaseContext verificationContext = CreateContext();
		Assert.IsTrue(verificationContext.SeederChoices.Any(x =>
			x.Seeder == successfulSeeder.Name && x.Choice == "answer" && x.Answer == "ok"));
	}

	[TestMethod]
	public void ReplayRunner_RejectsInvalidActiveAnswerBeforeExecutingSeeder()
	{
		SuccessfulReplaySeeder.Reset();
		var databaseName = Guid.NewGuid().ToString("N");
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(databaseName)
			.Options;
		FuturemudDatabaseContext CreateContext() => new(options);
		var seeder = new SuccessfulReplaySeeder();
		var profile = new SeederReplayProfile("invalid", "Invalid", "Invalid answer profile",
		[
			new SeederReplayStep(typeof(SuccessfulReplaySeeder), [new SeederReplayAnswer("answer", "invalid")])
		]);

		SeederReplayRunResult result = SeederReplayRunner.Run(profile, [seeder], CreateContext, new Version(1, 0, 0));

		Assert.IsFalse(result.Success);
		Assert.AreEqual(0, SuccessfulReplaySeeder.RunCount);
		StringAssert.Contains(result.Failure!, "no longer valid");
	}

	[TestMethod]
	public void ReplayRunner_IgnoresInactiveInventoryAnswersAndStopsOnBlockedPrerequisites()
	{
		ConditionalReplaySeeder.Reset();
		LaterReplaySeeder.Reset();
		var databaseName = Guid.NewGuid().ToString("N");
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(databaseName)
			.Options;
		FuturemudDatabaseContext CreateContext() => new(options);
		var conditionalSeeder = new ConditionalReplaySeeder();
		var blockedSeeder = new BlockedReplaySeeder();
		var laterSeeder = new LaterReplaySeeder();
		var profile = new SeederReplayProfile("conditional", "Conditional", "Conditional profile",
		[
			new SeederReplayStep(typeof(ConditionalReplaySeeder),
			[
				new SeederReplayAnswer("active", "ok"),
				new SeederReplayAnswer("inactive", "intentionally-invalid")
			]),
			new SeederReplayStep(typeof(BlockedReplaySeeder), []),
			new SeederReplayStep(typeof(LaterReplaySeeder), [])
		]);

		SeederReplayRunResult result = SeederReplayRunner.Run(profile,
			[conditionalSeeder, blockedSeeder, laterSeeder], CreateContext, new Version(1, 0, 0));

		Assert.IsFalse(result.Success);
		Assert.AreEqual(1, ConditionalReplaySeeder.RunCount);
		Assert.AreEqual(0, LaterReplaySeeder.RunCount);
		Assert.AreEqual(blockedSeeder.Name, result.FailedSeeder);
		CollectionAssert.AreEqual(new[] { nameof(LaterReplaySeeder) }, result.UnstartedSeeders.ToList());
	}

	[TestMethod]
	public void ReplayRunner_RefusesASeededDatabaseBeforeRunningTheFirstStep()
	{
		SuccessfulReplaySeeder.Reset();
		var databaseName = Guid.NewGuid().ToString("N");
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(databaseName)
			.Options;
		FuturemudDatabaseContext CreateContext() => new(options);
		using (FuturemudDatabaseContext seededContext = CreateContext())
		{
			seededContext.SeederChoices.Add(new SeederChoice
			{
				Version = "test",
				Seeder = "Existing",
				Choice = "choice",
				Answer = "answer",
				DateTime = DateTime.UtcNow
			});
			seededContext.SaveChanges();
		}

		var profile = new SeederReplayProfile("seeded", "Seeded", "Seeded profile",
		[
			new SeederReplayStep(typeof(SuccessfulReplaySeeder), [new SeederReplayAnswer("answer", "ok")])
		]);
		SeederReplayRunResult result = SeederReplayRunner.Run(
			profile,
			[new SuccessfulReplaySeeder()],
			CreateContext,
			new Version(1, 0, 0));

		Assert.IsFalse(result.Success);
		Assert.AreEqual(0, SuccessfulReplaySeeder.RunCount);
		StringAssert.Contains(result.Failure!, "freshly migrated, unseeded");
		using FuturemudDatabaseContext verificationContext = CreateContext();
		Assert.AreEqual(1, verificationContext.SeederChoices.Count());
	}

	[TestMethod]
	public void ProfileValidation_TracksEnabledSeederAndQuestionInventoryChanges()
	{
		SeederReplayProfile profile = DebugSeederReplayProfiles.All.First();
		IReadOnlyList<IDatabaseSeeder> seeders = SeederCatalogue.GetEnabledSeeders();

		SeederReplayValidationResult addedSeederValidation = SeederReplayRunner.Validate(
			profile,
			seeders.Append(new AddedReplaySeeder()));
		Assert.IsFalse(addedSeederValidation.IsValid);
		Assert.IsTrue(addedSeederValidation.Errors.Any(x => x.Contains(nameof(AddedReplaySeeder), StringComparison.Ordinal)));

		SeederReplayValidationResult disabledSeederValidation = SeederReplayRunner.Validate(
			profile,
			seeders.Append(new DisabledReplaySeeder()));
		Assert.IsTrue(disabledSeederValidation.IsValid,
			string.Join(Environment.NewLine, disabledSeederValidation.Errors));

		var changedQuestionProfile = new SeederReplayProfile("changed", "Changed", "Changed question inventory",
		[
			new SeederReplayStep(typeof(ChangedQuestionIdReplaySeeder),
				[new SeederReplayAnswer("old-question", "ok")])
		]);
		SeederReplayValidationResult changedQuestionValidation = SeederReplayRunner.Validate(
			changedQuestionProfile,
			[new ChangedQuestionIdReplaySeeder()]);
		Assert.IsFalse(changedQuestionValidation.IsValid);
		Assert.IsTrue(changedQuestionValidation.Errors.Any(x => x.Contains("new-question", StringComparison.Ordinal)));
		Assert.IsTrue(changedQuestionValidation.Errors.Any(x => x.Contains("old-question", StringComparison.Ordinal)));
	}

	private sealed class SuccessfulReplaySeeder : IDatabaseSeeder
	{
		internal static int RunCount { get; private set; }

		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		[
			("answer", "Answer", (_, _) => true,
				(answer, _) => answer == "ok" ? (true, string.Empty) : (false, "Expected ok."))
		];

		public int SortOrder => 0;
		public string Name => "Successful Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			RunCount++;
			return "Completed";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		internal static void Reset()
		{
			RunCount = 0;
		}
	}

	private sealed class FailingReplaySeeder : IDatabaseSeeder
	{
		internal static int RunCount { get; private set; }

		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

		public int SortOrder => 1;
		public string Name => "Failing Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			RunCount++;
			throw new InvalidOperationException("Intentional replay failure.");
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		internal static void Reset()
		{
			RunCount = 0;
		}
	}

	private sealed class ConditionalReplaySeeder : IDatabaseSeeder
	{
		internal static int RunCount { get; private set; }

		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		[
			("active", "Active", (_, _) => true,
				(answer, _) => answer == "ok" ? (true, string.Empty) : (false, "Expected ok.")),
			("inactive", "Inactive", (_, _) => false,
				(_, _) => (false, "This should not be validated."))
		];

		public int SortOrder => 0;
		public string Name => "Conditional Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			RunCount++;
			Assert.IsTrue(questionAnswers.ContainsKey("active"));
			Assert.IsFalse(questionAnswers.ContainsKey("inactive"));
			return "Completed";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		internal static void Reset()
		{
			RunCount = 0;
		}
	}

	private sealed class BlockedReplaySeeder : IDatabaseSeeder
	{
		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

		public int SortOrder => 1;
		public string Name => "Blocked Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			throw new AssertFailedException("A blocked replay seeder must not run.");
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		public SeederAssessment AssessSeedData(FuturemudDatabaseContext context)
		{
			return new SeederAssessment(
				SeederAssessmentStatus.Blocked,
				"Intentional test prerequisite block.",
				["Test prerequisite"],
				[],
				[]);
		}
	}

	private sealed class LaterReplaySeeder : IDatabaseSeeder
	{
		internal static int RunCount { get; private set; }

		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

		public int SortOrder => 2;
		public string Name => "Later Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			RunCount++;
			return "Completed";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		internal static void Reset()
		{
			RunCount = 0;
		}
	}

	private sealed class AddedReplaySeeder : IDatabaseSeeder
	{
		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

		public int SortOrder => int.MaxValue;
		public string Name => "Added Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			return "Completed";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.ReadyToInstall;
		}
	}

	private sealed class DisabledReplaySeeder : IDatabaseSeeder
	{
		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

		public int SortOrder => int.MaxValue;
		public string Name => "Disabled Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";
		public bool Enabled => false;

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			return "Completed";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.ReadyToInstall;
		}
	}

	private sealed class ChangedQuestionIdReplaySeeder : IDatabaseSeeder
	{
		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		[
			("new-question", "Changed question", (_, _) => true, (_, _) => (true, string.Empty))
		];

		public int SortOrder => 0;
		public string Name => "Changed Question Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			return "Completed";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			return ShouldSeedResult.ReadyToInstall;
		}
	}
}
#endif
