#if DEBUG
#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonReplayDispositionTests
{
	[TestMethod]
	public void NotApplicableStep_RequiresLivePredicateAndReason()
	{
		var seeder = new DispositionReplaySeeder();
		var profile = new SeederReplayProfile("missing-disposition-evidence", "Invalid", "Invalid",
		[
			new SeederReplayStep(typeof(DispositionReplaySeeder), [], SeederReplayDisposition.NotApplicable)
		]);

		SeederReplayValidationResult result = SeederReplayRunner.Validate(profile, [seeder]);

		Assert.IsFalse(result.IsValid);
		Assert.IsTrue(result.Errors.Any(x => x.Contains("explicit reason", StringComparison.Ordinal)));
		Assert.IsTrue(result.Errors.Any(x => x.Contains("live-context predicate", StringComparison.Ordinal)));
	}

	[TestMethod]
	public void NotApplicableStep_WhenPredicatePassesSkipsAssessmentAndExecutionAndIsReportedSeparately()
	{
		var seeder = new DispositionReplaySeeder();
		var profile = new SeederReplayProfile("not-applicable", "N/A", "N/A",
		[
			new SeederReplayStep(typeof(DispositionReplaySeeder), [], SeederReplayDisposition.NotApplicable,
				"The Armageddon setting is absent.", _ => true)
		]);
		FuturemudDatabaseContext CreateContext() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options);

		SeederReplayRunResult result = SeederReplayRunner.Run(profile, [seeder], CreateContext, new Version(1, 0));

		Assert.IsTrue(result.Success, result.Failure);
		Assert.AreEqual(0, result.CompletedSeeders.Count);
		CollectionAssert.AreEqual(new[] { seeder.Name }, result.NotApplicableSeeders.ToArray());
		Assert.AreEqual(0, seeder.AssessmentCount);
		Assert.AreEqual(0, seeder.ExecutionCount);
	}

	[TestMethod]
	public void NotApplicableStep_WhenPredicateFailsFailsBeforeAssessmentOrExecution()
	{
		var seeder = new DispositionReplaySeeder();
		var profile = new SeederReplayProfile("unexpectedly-applicable", "Unexpected", "Unexpected",
		[
			new SeederReplayStep(typeof(DispositionReplaySeeder), [], SeederReplayDisposition.NotApplicable,
				"The Armageddon setting is absent.", _ => false)
		]);
		FuturemudDatabaseContext CreateContext() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options);

		SeederReplayRunResult result = SeederReplayRunner.Run(profile, [seeder], CreateContext, new Version(1, 0));

		Assert.IsFalse(result.Success);
		Assert.AreEqual(seeder.Name, result.FailedSeeder);
		Assert.AreEqual(0, result.CompletedSeeders.Count);
		Assert.AreEqual(0, result.NotApplicableSeeders.Count);
		Assert.AreEqual(0, seeder.AssessmentCount);
		Assert.AreEqual(0, seeder.ExecutionCount);
		StringAssert.Contains(result.Failure!, "predicate did not pass");
	}

	[TestMethod]
	public void Profiles_PreserveEnabledInventoryAndDeclareExplicitArmageddonDispositions()
	{
		IReadOnlyList<IDatabaseSeeder> enabledSeeders = SeederCatalogue.GetEnabledSeeders();
		foreach (SeederReplayProfile profile in DebugSeederReplayProfiles.All)
		{
			SeederReplayValidationResult validation = SeederReplayRunner.Validate(profile, enabledSeeders);
			Assert.IsTrue(validation.IsValid,
				$"{profile.Id}:{Environment.NewLine}{string.Join(Environment.NewLine, validation.Errors)}");
			var worldfile = profile.Steps.Single(x => x.SeederType == typeof(ArmageddonWorldfileSeeder));
			if (profile.Id == "armageddon-native")
			{
				Assert.AreEqual(SeederReplayDisposition.Execute, worldfile.Disposition);
			}
			else
			{
				Assert.AreEqual(SeederReplayDisposition.NotApplicable, worldfile.Disposition);
				Assert.IsFalse(string.IsNullOrWhiteSpace(worldfile.NotApplicableReason));
				Assert.IsNotNull(worldfile.NotApplicableWhen);
			}
		}

		SeederReplayProfile armProfile = DebugSeederReplayProfiles.All.Single(x => x.Id == "armageddon-native");
		AssertAnswer(armProfile, typeof(TimeSeeder), "mode", "armageddon");
		AssertAnswer(armProfile, typeof(TimeSeeder), "secondsmultiplier", "6");
		AssertAnswer(armProfile, typeof(TimeSeeder), "startyear", "1");
		AssertAnswer(armProfile, typeof(CelestialSeeder), "packages", "Armageddon");
		AssertAnswer(armProfile, typeof(SkillPackageSeeder), "skillpreset", "Armageddon");
		AssertAnswer(armProfile, typeof(CombatSeeder), "skilloption", "armageddon");
		AssertAnswer(armProfile, typeof(CombatSeeder), "parryoption", "yes");
		AssertAnswer(armProfile, typeof(CombatSeeder), "installmuskets", "no");
		AssertAnswer(armProfile, typeof(CombatSeeder), "installguns", "no");
		AssertAnswer(armProfile, typeof(EconomySeeder), "era", "Medieval Age");
		AssertAnswer(armProfile, typeof(CultureSeeder), "culturepacks", "earthmedievaleurope");
		AssertAnswer(armProfile, typeof(ChargenSeeder), "class", "yes");
		AssertAnswer(armProfile, typeof(ChargenSeeder), "subclass", "yes");
		AssertAnswer(armProfile, typeof(ArmageddonSettingSeeder), ArmageddonSettingSeeder.FullInstallQuestion, "yes");
		AssertAnswer(armProfile, typeof(ArmageddonSettingSeeder), ArmageddonSettingSeeder.InstallQuestion, "no");
		AssertAnswer(armProfile, typeof(ArmageddonSettingSeeder), ArmageddonSettingSeeder.WeatherQuestion, "no");
		AssertAnswer(armProfile, typeof(ArmageddonSettingSeeder), ArmageddonSettingSeeder.ModeQuestion, "Classic");
		AssertAnswer(armProfile, typeof(ArmageddonSettingSeeder), ArmageddonSettingSeeder.CapacityQuestion, "4");
		AssertAnswer(armProfile, typeof(ArmageddonSettingSeeder), ArmageddonSettingSeeder.NativeBindingsQuestion, "{}");
	}

	[TestMethod]
	public void ArmageddonProfileUsesAcceptedCultureAndEconomyAnswers()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options);
		var economyEra = new EconomySeeder().SeederQuestions.Single(x => x.Id == "era");
		var culturePack = new CultureSeeder().SeederQuestions.Single(x => x.Id == "culturepacks");

		Assert.IsTrue(economyEra.Validator("Medieval Age", context).Success);
		Assert.IsTrue(culturePack.Validator("earthmedievaleurope", context).Success);
	}

	[TestMethod]
	public void GenericItemCatalogueIsSkippedOnlyForTheConfirmedArmageddonSkillPackage()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options);
		var step = DebugSeederReplayProfiles.All.Single(x => x.Id == "armageddon-native").Steps.Single(x => x.SeederType == typeof(ItemSeeder));
		Assert.AreEqual(SeederReplayDisposition.NotApplicable, step.Disposition);
		Assert.IsFalse(step.NotApplicableWhen!(context));
		context.SeederChoices.Add(new() { Seeder = "Skill Package", Choice = "skillpreset", Answer = "Armageddon", Version = "test" });
		context.SaveChanges();
		Assert.IsTrue(step.NotApplicableWhen(context));
		foreach (var earth in DebugSeederReplayProfiles.All.Where(x => x.Id != "armageddon-native"))
			Assert.AreEqual(SeederReplayDisposition.Execute, earth.Steps.Single(x => x.SeederType == typeof(ItemSeeder)).Disposition);
	}

	private static void AssertAnswer(SeederReplayProfile profile, Type seederType, string questionId, string expected)
	{
		SeederReplayStep step = profile.Steps.Single(x => x.SeederType == seederType);
		Assert.AreEqual(expected, step.Answers.Single(x => x.Id == questionId).Answer,
			$"Unexpected {questionId} answer in {profile.Id}.");
	}

	private sealed class DispositionReplaySeeder : IDatabaseSeeder
	{
		internal int AssessmentCount { get; private set; }
		internal int ExecutionCount { get; private set; }
		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];
		public int SortOrder => 0;
		public string Name => "Disposition Replay Seeder";
		public string Tagline => "Test";
		public string FullDescription => "Test";

		public SeederAssessment AssessSeedData(FuturemudDatabaseContext context)
		{
			AssessmentCount++;
			return new(SeederAssessmentStatus.ReadyToInstall, "Ready", [], [], []);
		}

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			ExecutionCount++;
			return "Completed";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context) => ShouldSeedResult.ReadyToInstall;
	}
}
#endif
