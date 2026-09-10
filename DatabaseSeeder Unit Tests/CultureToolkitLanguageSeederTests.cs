#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitLanguageSeederTests
{
	[TestMethod]
	public void NewStageRerunsPreserveBuilderCapAndLearnerDeletionWithoutDuplicateIdentities()
	{
		using var context = Context();
		var prerequisites = Prerequisites(context);
		var catalogue = new CultureToolkitCatalogue();
		var whole = catalogue.Compose("earlymodern");
		const string key = "english.earlymodern";
		var pack = whole with { Languages = whole.Languages.Where(x => CultureToolkitCatalogue.Text(x, "key") == key).ToArray() };
		Assert.AreEqual(1, pack.Languages.Count);
		var conflicts = new List<string>();
		var first = CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack,
			new Dictionary<string, FuturemudDatabaseContext>(), new Dictionary<string, Language>(), prerequisites, conflicts);
		var language = first.Languages[key];
		Assert.IsTrue(language.Accents.Any(x => x.Role == 2));
		var cap = context.TraitExpressions.Find(first.Traits[key].ExpressionId)!;
		StringAssert.StartsWith(cap.Expression, "max(200,");
		cap.Expression = "175";
		language.Name = "Builder English";
		language.Accents.Single(x => x.Role == 2).Role = 1;
		context.SaveChanges();
		for (var i = 0; i < 2; i++) CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack,
			new Dictionary<string, FuturemudDatabaseContext>(), new Dictionary<string, Language>(), prerequisites, conflicts);
		Assert.AreEqual(1, context.Languages.Count());
		Assert.AreEqual(2, context.Accents.Count());
		Assert.AreEqual("175", cap.Expression);
		Assert.AreEqual("Builder English", language.Name);
		Assert.IsFalse(language.Accents.Any(x => x.Role == 2));
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void RetainedWelshLearnerOnlySourceGetsSuppliedLocalAccentWithoutReplacingLearner()
	{
		using var context = Context();
		using var source = Context();
		var prerequisites = Prerequisites(context);
		var catalogue = new CultureToolkitCatalogue();
		var whole = catalogue.Compose("darkages");
		var pack = whole with { Languages = whole.Languages.Where(x => CultureToolkitCatalogue.Text(x, "key") == "welsh").ToArray() };
		var welsh = new Language
		{
			Name = "Welsh", LinkedTrait = new TraitDefinition { Name = "Welsh", Expression = new TraitExpression { Name = "Welsh Skill Cap", Expression = "200" } }
		};
		source.Add(welsh);
		source.SaveChanges();
		var foreign = new Accent { Name = "Foreign", Group = "Foreign", Description = "Retained learner", LanguageId = welsh.Id };
		source.Add(foreign);
		source.SaveChanges();
		foreign.Role = 2;
		source.SaveChanges();
		var stages = new Dictionary<string, FuturemudDatabaseContext> { ["earthrenaissanceeurope"] = source };
		var conflicts = new List<string>();
		var first = CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, stages, new Dictionary<string, Language>(), prerequisites, conflicts);
		var learner = first.Languages["welsh"].Accents.Single(x => x.Role == 2).Id;
		CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, stages, new Dictionary<string, Language>(), prerequisites, conflicts);
		Assert.AreEqual(2, context.Accents.Count());
		Assert.AreEqual(learner, first.Languages["welsh"].Accents.Single(x => x.Role == 2).Id);
		Assert.AreEqual("Retained learner", context.Accents.Find(learner)!.Description);
		Assert.AreEqual(0, conflicts.Count);
		Assert.IsTrue(context.Accents.Any(x => x.Group != "Foreign"));
	}

	[TestMethod]
	public void MissingRetainedDefinitionFailsBeforeCreatingAnyLanguageOrCap()
	{
		using var context = Context();
		var prerequisites = Prerequisites(context);
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose("medieval");
		var count = context.TraitExpressions.Count();
		var exception = Assert.ThrowsException<InvalidOperationException>(() => CultureToolkitLanguageSeeder.Upsert(context,
			catalogue, pack, new Dictionary<string, FuturemudDatabaseContext>(), new Dictionary<string, Language>(), prerequisites, []));
		StringAssert.Contains(exception.Message, "Missing retained source definition");
		Assert.AreEqual(0, context.Languages.Count());
		Assert.AreEqual(count, context.TraitExpressions.Count());
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LegacyUpgradeBindsEachKnownAccentSourceAndRejectsUnrecognisedEdits(bool builderEdited)
	{
		using var context = Context();
		using var dark = Context();
		using var renaissance = Context();
		var prerequisites = Prerequisites(context);
		Language AddLatin(FuturemudDatabaseContext target, string description)
		{
			var language = new Language { Name = "Latin", LinkedTrait = new TraitDefinition
				{ Name = "Latin", Expression = new TraitExpression { Name = "Latin Skill Cap", Expression = "200" } } };
			language.Accents.Add(new Accent { Name = "Crude", Group = "Crude", Description = description, Suffix = "with an accent", VagueSuffix = "with an accent", Difficulty = 6 });
			target.Add(language);
			target.SaveChanges();
			return language;
		}
		AddLatin(dark, "First retained source");
		AddLatin(renaissance, "Second retained source");
		var existing = AddLatin(context, builderEdited ? "Independent builder edit" : "First retained source");
		var catalogue = new CultureToolkitCatalogue();
		var whole = catalogue.Compose("medieval");
		var pack = whole with { Languages = whole.Languages.Where(x => CultureToolkitCatalogue.Text(x, "key") == "latin").ToArray() };
		var stages = new Dictionary<string, FuturemudDatabaseContext> { ["earthdarkagesandmedieval"] = dark, ["earthrenaissanceeurope"] = renaissance };
		void Install() => CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, stages,
			new Dictionary<string, Language> { ["latin"] = existing }, prerequisites, []);
		if (builderEdited)
		{
			StringAssert.Contains(Assert.ThrowsException<InvalidOperationException>(Install).Message, "explicit source binding");
			Assert.IsTrue(context.Accents.Any(x => x.Description == "Independent builder edit"));
			return;
		}
		Install();
		var count = context.Accents.Count();
		Install();
		Assert.AreEqual(count, context.Accents.Count());
		Assert.AreEqual(1, context.Accents.Count(x => x.Description == "First retained source"));
		Assert.AreEqual(1, context.Accents.Count(x => x.Description == "Second retained source"));
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);

	private static CultureLanguagePrerequisites Prerequisites(FuturemudDatabaseContext context)
	{
		var intelligence = new TraitDefinition { Name = "Intelligence", Alias = "int", Type = 1 };
		var decorator = new TraitDecorator();
		var improver = new Improver();
		var difficulty = new LanguageDifficultyModels();
		var alwaysTrue = new FutureProg { FunctionName = "AlwaysTrue" };
		var alwaysFalse = new FutureProg { FunctionName = "AlwaysFalse" };
		context.AddRange(intelligence, decorator, improver, difficulty, alwaysTrue, alwaysFalse);
		context.SaveChanges();
		return new(intelligence, decorator, improver, difficulty, alwaysTrue, alwaysFalse);
	}
}
