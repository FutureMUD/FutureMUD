using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSeederMessageStyleHelperTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void FormatAttackMessage_SentenceStyle_AddsSingleTerminalFullStop()
	{
		const string raw = "@ plunge|plunges $2 deep into $1's throat";

		Assert.AreEqual(
			$"{raw}.",
			CombatSeederMessageStyleHelper.FormatAttackMessage(raw, SeedCombatMessageStyle.Sentences));
		Assert.AreEqual(
			$"{raw}.",
			CombatSeederMessageStyleHelper.FormatAttackMessage($"{raw}.", SeedCombatMessageStyle.Sentences));
	}

	[TestMethod]
	public void BuildDefenseFailure_CompactStyle_UsesInlineJoiner()
	{
		var message = CombatSeederMessageStyleHelper.BuildDefenseFailure(
			SeedCombatMessageStyle.Compact,
			"#1 %1|attempt|attempts to dodge out of the way",
			"hit on &1's {1}");

		Assert.AreEqual(
			", and #1 %1|attempt|attempts to dodge out of the way but %1|get|gets hit on &1's {1}",
			message);
	}

	[TestMethod]
	public void BuildDefenseFailure_SparseStyle_UsesSeparateHitLine()
	{
		var message = CombatSeederMessageStyleHelper.BuildDefenseFailure(
			SeedCombatMessageStyle.Sparse,
			"#1 %1|attempt|attempts to parry with $3",
			"hit on &1's {1}");

		Assert.AreEqual(
			".\n#1 %1|attempt|attempts to parry with $3\n#1 %1|get|gets hit on &1's {1}",
			message);
	}

	[TestMethod]
	public void BuildDefenseFailure_BeHitCompactStyle_UsesAndIsHitJoiner()
	{
		var message = CombatSeederMessageStyleHelper.BuildDefenseFailure(
			SeedCombatMessageStyle.Compact,
			"#1 %1|offer|offers no defense",
			"hit on &1's {1}",
			SeedCombatHitVerb.BeHit);

		Assert.AreEqual(
			", and #1 %1|offer|offers no defense and %1|are|is hit on &1's {1}",
			message);
	}

	[TestMethod]
	public void RepresentativeComposition_NaturalAndRangedMessages_AreGrammatical()
	{
		var natural = CombatSeederMessageStyleHelper.FormatAttackMessage(
			"@ rake|rakes &0's {0} across $1 with @hand precision",
			SeedCombatMessageStyle.Sentences);
		var renderedNatural = string.Format(natural, "claws").Replace("@hand", "left");
		Assert.AreEqual("@ rake|rakes &0's claws across $1 with left precision.", renderedNatural);

		var ranged = string.Format(
			CombatSeederMessageStyleHelper.FormatStandaloneMessage("@ stand|stands and {0} $2 at $1"),
			"fire|fires");
		Assert.AreEqual("@ stand|stands and fire|fires $2 at $1.", ranged);
	}

	[TestMethod]
	public void GetRecordedChoice_CombatSeederHistory_ReturnsLatestRecordedAnswer()
	{
		using var context = BuildContext();
		context.SeederChoices.AddRange(
			new SeederChoice
			{
				Id = 1,
				Seeder = "Combat Seeder",
				Choice = "messagestyle",
				Answer = "sentences",
				Version = "1.0.0",
				DateTime = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
			},
			new SeederChoice
			{
				Id = 2,
				Seeder = "Combat Seeder",
				Choice = "messagestyle",
				Answer = "sparse",
				Version = "1.0.0",
				DateTime = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc)
			},
			new SeederChoice
			{
				Id = 3,
				Seeder = "Animal Seeder",
				Choice = "messagestyle",
				Answer = "compact",
				Version = "1.0.0",
				DateTime = new DateTime(2026, 3, 3, 0, 0, 0, DateTimeKind.Utc)
			});
		context.SaveChanges();

		Assert.AreEqual("sparse", CombatSeederMessageStyleHelper.GetRecordedChoice(context));
	}

	[TestMethod]
	public void MergeQuestionAnswersWithRecordedChoice_MissingMessagestyle_AddsCombatSeederAnswer()
	{
		using var context = BuildContext();
		context.SeederChoices.Add(new SeederChoice
		{
			Id = 1,
			Seeder = "Combat Seeder",
			Choice = "messagestyle",
			Answer = "sparse",
			Version = "1.0.0",
			DateTime = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc)
		});
		context.SaveChanges();

		var answers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["model"] = "full"
		};

		var effectiveAnswers = CombatSeederMessageStyleHelper.MergeQuestionAnswersWithRecordedChoice(context, answers);

		Assert.AreEqual("sparse", effectiveAnswers["messagestyle"]);
		Assert.AreEqual("full", effectiveAnswers["model"]);
	}

	[TestMethod]
	public void NonHumanSeederQuestions_MessagestyleFilter_UsesCombatSeederChoiceWhenPresent()
	{
		using var context = BuildContext();
		var question = NonHumanSeederQuestions.GetQuestions().First(x => x.Id == "messagestyle");

		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>()),
			"Without a recorded combat seeder choice, the non-human seeder should still ask the question.");

		context.SeederChoices.Add(new SeederChoice
		{
			Id = 1,
			Seeder = "Combat Seeder",
			Choice = "messagestyle",
			Answer = "compact",
			Version = "1.0.0",
			DateTime = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc)
		});
		context.SaveChanges();

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>()),
			"Once the combat seeder has recorded a message style, non-human seeders should reuse it instead of re-asking.");
	}
}
