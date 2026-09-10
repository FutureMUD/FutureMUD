#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitAccentTests
{
	[DataTestMethod]
	[DataRow("classical", "antiquity", true)]
	[DataRow("liturgical", "antiquity", false)]
	[DataRow("liturgical", "darkages", true)]
	[DataRow("neo-classical", "medieval", false)]
	[DataRow("neo-classical", "renaissance", true)]
	[DataRow("carolingian", "antiquity", false)]
	[DataRow("carolingian", "darkages", true)]
	public void RetainedLatinAvailabilityExecutesEraPolicyAndKeepsDisabledFallbacks(string name, string era, bool expected)
	{
		FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		using var context = Context();
		using var source = Context();
		var module = name == "carolingian" ? "earthdarkagesandmedieval" : "earthrenaissanceeurope";
		var original = new Language { Name = "Latin", Accents = [new Accent { Name = name, Group = "native" }, new Accent { Name = "foreign", Group = "foreign" }] };
		source.Add(original);
		source.SaveChanges();
		original.Accents.Single(x => x.Group == "foreign").Role = 2;
		source.SaveChanges();
		var language = new Language { Name = "Latin", Accents = [new Accent { Name = name, Group = "native" }, new Accent { Name = "foreign", Group = "foreign" }] };
		var no = new MudSharp.Models.FutureProg { FunctionName = "AlwaysFalse", FunctionText = "return false", ReturnType = (long)ProgVariableTypes.Boolean };
		context.AddRange(language, no);
		context.SaveChanges();
		language.Accents.Single(x => x.Group == "foreign").Role = 2;
		void Manage(string type, string key, long id) => context.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Seeder = "CultureSeeder", EntityType = type, StableKey = key, Module = era, LogicalId = id,
			ManifestVersion = "fixture", AppliedAt = DateTime.UtcNow,
			SeedBaseline = type == "Accent" ? "{\"ChargenAvailabilityProgId\":\"null\"}" : "{}"
		});
		Manage("Language", "latin", language.Id);
		foreach (var accent in language.Accents) Manage("Accent", $"latin.accent.{module}.{accent.Name}", accent.Id);
		context.SaveChanges();
		var stages = new Dictionary<string, FuturemudDatabaseContext> { [module] = source };
		var languages = new Dictionary<string, Language> { ["latin"] = language };
		var natives = new Dictionary<long, IReadOnlyList<long>> { [language.Id] = [42] };
		var conflicts = new List<string>();
		var report = CultureToolkitAccents.Upsert(context, era, natives, conflicts, new CultureToolkitCatalogue(), stages, languages).Single();
		using (var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray()))
		{
			var retained = language.Accents.Single(x => x.Name == name);
			Assert.AreEqual(expected, compiler.Compile(retained.ChargenAvailabilityProgId!.Value).ExecuteBool(Mock.Of<IChargen>()));
		}
		if (!expected)
		{
			var fallback = report.Details.Single(x => x.StableKey == "latin.accent.local");
			context.Accents.Find(fallback.AccentId)!.ChargenAvailabilityProgId = no.Id;
			context.SaveChanges();
			CultureToolkitAccents.Upsert(context, era, natives, conflicts, new CultureToolkitCatalogue(), stages, languages);
			Assert.AreEqual(no.Id, context.Accents.Find(fallback.AccentId)!.ChargenAvailabilityProgId);
			Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
		}
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SyntheticLearnerSupportsLanguageOnlyFullAndRepeatedLanguageOnlyOrdering(bool repairStockFalse)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var intelligence = new TraitDefinition { Name = "Intelligence", Alias = "int", Type = 1 };
		var decorator = new TraitDecorator();
		var improver = new Improver();
		var difficulty = new LanguageDifficultyModels();
		var yes = new MudSharp.Models.FutureProg { FunctionName = "AlwaysTrue", FunctionText = "return true", ReturnType = (long)ProgVariableTypes.Boolean };
		var no = new MudSharp.Models.FutureProg { FunctionName = "AlwaysFalse", FunctionText = "return false", ReturnType = (long)ProgVariableTypes.Boolean };
		context.AddRange(intelligence, decorator, improver, difficulty, yes, no);
		context.SaveChanges();
		var prerequisites = new CultureLanguagePrerequisites(intelligence, decorator, improver, difficulty, yes, no);
		var catalogue = new CultureToolkitCatalogue();
		var whole = catalogue.Compose("earlymodern");
		var pack = whole with { Languages = whole.Languages.Where(x => CultureToolkitCatalogue.Text(x, "key") == "english.earlymodern").ToArray() };
		var stages = new Dictionary<string, FuturemudDatabaseContext>();
		var conflicts = new List<string>();
		var languages = CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, stages, new Dictionary<string, Language>(), prerequisites, conflicts).Languages;
		var language = languages["english.earlymodern"];
		var learnerId = language.Accents.Single(x => x.Role == 2).Id;
		if (repairStockFalse)
		{
			context.Accents.Find(learnerId)!.ChargenAvailabilityProgId = no.Id;
			var record = context.SeederManagedRecords.Single(x => x.Seeder == "CultureSeeder" && x.EntityType == "Accent" && x.LogicalId == learnerId);
			var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
			baseline[nameof(Accent.ChargenAvailabilityProgId)] = JsonSerializer.Serialize((long?)no.Id);
			record.SeedBaseline = JsonSerializer.Serialize(baseline);
			context.SaveChanges();
		}
		var ch = new Mock<IChargen>();
		var ethnicity = new Mock<IEthnicity>();
		var nativeLanguage = new Mock<MudSharp.Communication.Language.ILanguage>();
		ch.Setup(x => x.GetProperty("nativelanguage")).Returns(nativeLanguage.Object);
		nativeLanguage.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(-1));
		ch.SetupGet(x => x.Type).Returns(ProgVariableTypes.Chargen);
		ch.SetupGet(x => x.GetObject).Returns(ch.Object);
		ch.Setup(x => x.GetProperty("ethnicity")).Returns(ethnicity.Object);
		for (var phase = 0; phase < 4; phase++)
		{
			CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, stages, new Dictionary<string, Language>(), prerequisites, conflicts);
			var natives = phase is 1 or 3 ? new Dictionary<long, IReadOnlyList<long>> { [language.Id] = [42] } : new();
			var resolution = CultureToolkitAccents.Upsert(context, pack.Era, natives, conflicts, catalogue, stages, languages).Single();
			Assert.AreEqual(learnerId, language.Accents.Single(x => x.Role == 2).Id);
			using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
			compiler.Compile(resolution.EligibilityProgId);
			var learner = compiler.Compile(context.Accents.Find(learnerId)!.ChargenAvailabilityProgId!.Value);
			ethnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(43));
		nativeLanguage.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(-1));
			Assert.IsTrue(learner.ExecuteBool(ch.Object));
			ethnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(42));
		nativeLanguage.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(language.Id));
			Assert.IsFalse(learner.ExecuteBool(ch.Object));
			Assert.IsTrue(compiler.Compile(context.Accents.Find(resolution.NativeAccentIds.Single())!.ChargenAvailabilityProgId!.Value).ExecuteBool(ch.Object));
		}
		Assert.AreEqual(2, context.Accents.Count());
		Assert.AreEqual(0, conflicts.Count, string.Join("\n", conflicts));
		var restricted = new Accent { Name = "Restricted regional", Group = "native", LanguageId = language.Id, ChargenAvailabilityProgId = no.Id };
		context.Accents.Add(restricted);
		context.SaveChanges();
		context.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Seeder = "CultureSeeder", EntityType = "Accent", StableKey = "fixture.source-restricted", Module = pack.Era,
			LogicalId = restricted.Id, ManifestVersion = "fixture", AppliedAt = DateTime.UtcNow,
			SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { [nameof(Accent.ChargenAvailabilityProgId)] = JsonSerializer.Serialize((long?)no.Id) })
		});
		context.SaveChanges();
		CultureToolkitAccents.Upsert(context, pack.Era, new Dictionary<long, IReadOnlyList<long>>(), conflicts, catalogue, stages, languages);
		using (var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray()))
		{
			compiler.Compile(no.Id);
			Assert.IsFalse(compiler.Compile(restricted.ChargenAvailabilityProgId!.Value).ExecuteBool(ch.Object), "A genuine source restriction must remain effective.");
		}
		var combined = context.FutureProgs.Find(context.Accents.Find(learnerId)!.ChargenAvailabilityProgId)!;
		combined.FunctionText = "return false";
		language.Accents.Single(x => x.Role == 2).Role = 1;
		// Role and predicate overrides remain authoritative on rerun.
		context.SaveChanges();
		CultureToolkitAccents.Upsert(context, pack.Era, new Dictionary<long, IReadOnlyList<long>>(), conflicts, catalogue, stages, languages);
		Assert.IsFalse(language.Accents.Any(x => x.Role == 2));
		Assert.AreEqual("return false", combined.FunctionText);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void NativeEligibilityExcludesForeignAccentsPreservesLearnerAndBuilderConditions()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var language = new Language { Name = "Fixture" };
		context.Add(language);
		context.SaveChanges();
		var local = new Accent { Name = "Regional", Group = "Native", LanguageId = language.Id };
		var foreign = new Accent { Name = "Foreign", Group = "foreign", LanguageId = language.Id };
		var custom = new Accent { Role = 1, Name = "Custom", Group = "foreign", LanguageId = language.Id, ChargenAvailabilityProgId = 999 };
		context.AddRange(local, foreign, custom);
		context.SaveChanges();
		foreign.Role = 2;
		foreach (var accent in new[] { local, foreign, custom })
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = "CultureSeeder", EntityType = "Accent", StableKey = $"fixture.{accent.Id}", Module = "medieval", LogicalId = accent.Id,
				ManifestVersion = "fixture", AppliedAt = DateTime.UtcNow,
				SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { [nameof(Accent.ChargenAvailabilityProgId)] = "null" })
			});
		context.SaveChanges();
		var bindings = new Dictionary<long, IReadOnlyList<long>> { [language.Id] = [42] };
		var conflicts = new List<string>();
		var first = CultureToolkitAccents.Upsert(context, "medieval", bindings, conflicts).Single();
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		var prog = compiler.Compile(first.EligibilityProgId);
		var ch = new Mock<IChargen>();
		var ethnicity = new Mock<IEthnicity>();
		var nativeLanguage = new Mock<MudSharp.Communication.Language.ILanguage>();
		ch.Setup(x => x.GetProperty("nativelanguage")).Returns(nativeLanguage.Object);
		nativeLanguage.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(-1));
		ch.SetupGet(x => x.Type).Returns(ProgVariableTypes.Chargen);
		ch.SetupGet(x => x.GetObject).Returns(ch.Object);
		ch.Setup(x => x.GetProperty("ethnicity")).Returns(ethnicity.Object);
		ethnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(42));
		nativeLanguage.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(language.Id));
		Assert.IsFalse(prog.ExecuteBool(ch.Object));
		ethnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(43));
		nativeLanguage.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(-1));
		Assert.IsTrue(prog.ExecuteBool(ch.Object));
		Assert.IsTrue(compiler.Compile(local.ChargenAvailabilityProgId!.Value).ExecuteBool(ch.Object));
		Assert.AreEqual(999L, custom.ChargenAvailabilityProgId);
		Assert.AreEqual(2, foreign.Role);
		CultureToolkitAccents.Upsert(context, "medieval", bindings, conflicts);
		Assert.IsTrue(conflicts.All(x => x.Contains($"accent.availability.{custom.Id}")));
		foreign.ChargenAvailabilityProgId = 998;
		context.SaveChanges();
		CultureToolkitAccents.Upsert(context, "medieval", bindings, conflicts);
		Assert.AreEqual(998L, foreign.ChargenAvailabilityProgId);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}
}
