using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatBalanceProfileHelperTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void GetRecordedChoice_HumanSeederHistory_ReturnsLatestRecordedAnswer()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.SeederChoices.AddRange(
			new SeederChoice
			{
				Id = 1,
				Seeder = "Human Seeder",
				Choice = "balance",
				Answer = "stock",
				Version = "1.0.0",
				DateTime = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)
			},
			new SeederChoice
			{
				Id = 2,
				Seeder = "Human Seeder",
				Choice = "balance",
				Answer = "combat-rebalance",
				Version = "1.0.0",
				DateTime = new DateTime(2026, 4, 2, 0, 0, 0, DateTimeKind.Utc)
			});
		context.SaveChanges();

		Assert.AreEqual("combat-rebalance", CombatBalanceProfileHelper.GetRecordedChoice(context));
	}

	[TestMethod]
	public void MergeQuestionAnswersWithRecordedChoice_MissingBalance_AddsRecordedChoice()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.SeederChoices.Add(new SeederChoice
		{
			Id = 1,
			Seeder = "Human Seeder",
			Choice = "balance",
			Answer = "combat-rebalance",
			Version = "1.0.0",
			DateTime = new DateTime(2026, 4, 2, 0, 0, 0, DateTimeKind.Utc)
		});
		context.SaveChanges();

		Dictionary<string, string> answers = new(StringComparer.OrdinalIgnoreCase)
		{
			["model"] = "full"
		};

		IReadOnlyDictionary<string, string> effectiveAnswers =
			CombatBalanceProfileHelper.MergeQuestionAnswersWithRecordedChoice(context, answers);

		Assert.AreEqual("combat-rebalance", effectiveAnswers["balance"]);
		Assert.AreEqual("full", effectiveAnswers["model"]);
	}

	[TestMethod]
	public void HumanSeeder_IsOnlyCombatSeederThatAsksForBalanceProfile()
	{
		Assert.IsTrue(new HumanSeeder().SeederQuestions.Any(x => x.Id == "balance"));
		Assert.IsFalse(new CombatSeeder().SeederQuestions.Any(x => x.Id == "balance"));
		Assert.IsFalse(new AnimalSeeder().SeederQuestions.Any(x => x.Id == "balance"));
		Assert.IsFalse(new MythicalAnimalSeeder().SeederQuestions.Any(x => x.Id == "balance"));
		Assert.IsFalse(new RobotSeeder().SeederQuestions.Any());
	}

	[TestMethod]
	public void SeederQuestionRegistry_HumanBalanceQuestion_IsSharedAndAutoReused()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.SeederChoices.Add(new SeederChoice
		{
			Id = 1,
			Seeder = "Human Seeder",
			Choice = "balance",
			Answer = "combat-rebalance",
			Version = "1.0.0",
			DateTime = new DateTime(2026, 4, 2, 0, 0, 0, DateTimeKind.Utc)
		});
		context.SaveChanges();

		SeederQuestion question = SeederQuestionRegistry
			.GetQuestions(new HumanSeeder(), new HumanSeeder().SeederQuestions)
			.Single(x => x.Id == "balance");

		Assert.AreEqual(CombatBalanceProfileHelper.SharedAnswerKey, question.SharedAnswerKey);
		Assert.IsTrue(question.AutoReuseLastAnswer);
		Assert.AreEqual("combat-rebalance", question.DefaultAnswerResolver!(context, new Dictionary<string, string>()));
	}

	[TestMethod]
	public void CombatSeeder_RandomQuestionFilter_SuppressesQuestionWhenCombatRebalanceRecorded()
	{
		using FuturemudDatabaseContext context = BuildContext();
		var question = new CombatSeeder().SeederQuestions.Single(x => x.Id == "random");

		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>()),
			"Without a stored balance choice, the combat seeder should still ask about stock randomness.");

		context.SeederChoices.Add(new SeederChoice
		{
			Id = 1,
			Seeder = "Human Seeder",
			Choice = "balance",
			Answer = "combat-rebalance",
			Version = "1.0.0",
			DateTime = new DateTime(2026, 4, 2, 0, 0, 0, DateTimeKind.Utc)
		});
		context.SaveChanges();

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>()),
			"The combat rebalance profile should bypass the old randomness question.");
	}

	[TestMethod]
	public void NonHumanSeederQuestions_RandomFilter_SuppressesQuestionWhenCombatRebalanceSelected()
	{
		using FuturemudDatabaseContext context = BuildContext();
		var question = NonHumanSeederQuestions.GetQuestions().Single(x => x.Id == "random");

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>
		{
			["balance"] = "combat-rebalance"
		}));

		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>
		{
			["balance"] = "stock"
		}));
	}
}
