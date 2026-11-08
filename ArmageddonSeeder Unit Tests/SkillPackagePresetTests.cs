#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SkillPackagePresetTests
{
	private static FuturemudDatabaseContext Fixture()
	{
		var db = ArmageddonSettingPackageTests.Fixture();
		db.Accounts.Add(new Account { Id = 1, Name = "Installer" });
		db.SaveChanges();
		return db;
	}

	private static Dictionary<string, string> Answers(string label) => label.Equals("Armageddon", StringComparison.OrdinalIgnoreCase)
		? new() { ["skillpreset"] = label }
		: new() { ["skillpreset"] = label, ["skillcapmodel"] = "flat", ["skillgainmodel"] = "labmud", ["branching"] = "yes" };

	[TestMethod]
	[DataRow("Simple-Gerund", "Carrying", true, false, false)]
	[DataRow("Simple-NonGerund", "Armour Use", true, false, false)]
	[DataRow("Simple-Gerund-Modern", "Carrying", true, false, true)]
	[DataRow("Simple-NonGerund-Modern", "Armour Use", true, false, true)]
	[DataRow("Complex-Gerund", "Running", false, false, false)]
	[DataRow("Complex-NonGerund", "Run", false, false, false)]
	[DataRow("Complex-Gerund-Modern", "Running", false, false, true)]
	[DataRow("Complex-NonGerund-Modern", "Run", false, false, true)]
	[DataRow("RPI-Gerund", "Running", false, true, false)]
	[DataRow("RPI-NonGerund", "Run", false, true, false)]
	[DataRow("RPI-Gerund-Modern", "Running", false, true, true)]
	[DataRow("RPI-NonGerund-Modern", "Run", false, true, true)]
	[DataRow("Armageddon", "Backstab", false, false, false)]
	public void EachPreset_InstallsItsExpectedSkillsNamesAndChecks(string label, string representative,
		bool simple, bool rpi, bool modern)
	{
		using var db = Fixture();
		var seeder = new SkillPackageSeeder();
		var answers = Answers(label);
		seeder.SeedData(db, answers);
		Assert.IsTrue(db.TraitDefinitions.Any(x => x.Type == 0 && x.Name == representative), label);
		Assert.IsTrue(db.Checks.Any());
		Assert.IsTrue(db.Languages.Any(x => x.Name == "Admin Speech"));
		Assert.IsTrue(db.NpcSkillPackages.Any());
		if (label == "Armageddon")
		{
			var skills = ArmageddonSkillPackage.Resolve(db);
			Assert.AreEqual(142, skills.Values.Select(x => x.Id).Distinct().Count(), "Resolution also contains two intentional aliases.");
			foreach (var binding in ArmageddonSkillPackage.StaticTraitBindings)
				Assert.AreEqual(skills[binding.Value].Id.ToString(), db.StaticConfigurations.Find(binding.Key)!.Definition);
			Assert.AreEqual("200", skills["arm.skill.sirihish"].Expression.Expression);
			Assert.AreEqual("0", skills["arm.skill.pilot"].Expression.Expression);
			return;
		}
		Assert.AreEqual(simple, db.TraitDefinitions.Any(x => x.Type == 0 && x.Name == "Athletics"));
		Assert.AreEqual(rpi, db.TraitDefinitions.Any(x => x.Type == 0 && x.Name == "Ritual"));
		Assert.AreEqual(modern, db.TraitDefinitions.Any(x => x.Type == 0 && x.Name == "Electronics"));
		Assert.AreEqual("70", db.TraitDefinitions.Single(x => x.Type == 0 && x.Name == representative).Expression.Expression,
			"The preset must preserve the separately selected flat-cap policy.");
	}

	[TestMethod]
	public void PresetCatalogue_EnumeratesAllTwelveOrdinaryShapesAndArmageddon()
	{
		var expected = new[]
		{
			"Simple-Gerund", "Simple-NonGerund", "Complex-Gerund", "Complex-NonGerund", "RPI-Gerund", "RPI-NonGerund",
			"Simple-Gerund-Modern", "Simple-NonGerund-Modern", "Complex-Gerund-Modern", "Complex-NonGerund-Modern",
			"RPI-Gerund-Modern", "RPI-NonGerund-Modern", "Armageddon"
		};
		CollectionAssert.AreEquivalent(expected, SkillPackageSeeder.Presets.Select(x => x.Label).ToArray());
		using var db = Fixture();
		IDatabaseSeeder seeder = new SkillPackageSeeder();
		Assert.AreEqual("skillpreset", seeder.Questions.First().Id);
		foreach (var label in expected)
		{
			var answers = Answers(label);
			var active = seeder.Questions.Where(x => x.Filter(db, answers)).Select(x => x.Id).ToArray();
			CollectionAssert.AreEquivalent(label == "Armageddon" ? new[] { "skillpreset" } :
				new[] { "skillpreset", "skillcapmodel", "skillgainmodel", "branching" }, active);
			Assert.IsTrue(seeder.Questions.First().Validator(label, db).Success);
			StringAssert.Contains(seeder.Questions.First().Question, "#B" + label + "#F:");
		}
		Assert.IsFalse(seeder.Questions.First().Validator("complex", db).Success);
	}

	[TestMethod]
	public void OldAnswers_SuggestPresetAndPersistBothContractsAcrossResume()
	{
		using var db = Fixture();
		IDatabaseSeeder seeder = new SkillPackageSeeder();
		var legacy = new Dictionary<string, string>
		{
			["complexity"] = "complex", ["gerund"] = "no", ["modern"] = "yes",
			["skillcapmodel"] = "flat", ["skillgainmodel"] = "labmud", ["branching"] = "yes"
		};
		// Exercise the public legacy call, then store an old-version six-answer record.
		seeder.SeedData(db, legacy);
		var modifiers = db.CheckTemplateDifficulties.ToDictionary(x => (x.CheckTemplateId, x.Difficulty), x => x.Modifier);
		db.CheckTemplateDifficulties.Remove(db.CheckTemplateDifficulties.First());
		SeederAnswerMemory.PersistAnswers(db, seeder, seeder.Questions, legacy, "legacy", DateTime.UtcNow.AddDays(-1));
		db.SaveChanges();
		db.ChangeTracker.Clear();
		var question = seeder.Questions.First();
		Assert.IsFalse(question.AutoReuseLastAnswer);
		Assert.AreEqual("Complex-NonGerund-Modern", SeederAnswerMemory.GetRememberedAnswer(db, seeder, question,
			new Dictionary<string, string>()));
		var skillIds = db.TraitDefinitions.Where(x => x.Type == 0).ToDictionary(x => x.Name, x => x.Id);
		var result = SeederExecutionService.Execute(db, seeder, seeder.Questions,
			Answers("Complex-NonGerund-Modern"), new Version(1, 0));
		Assert.IsTrue(result.Success, result.Exception?.ToString());
		db.ChangeTracker.Clear();
		var recorded = SeederAnswerMemory.GetLatestSeederAnswers(db, seeder.Name);
		Assert.AreEqual("Complex-NonGerund-Modern", recorded["skillpreset"]);
		foreach (var answer in legacy) Assert.AreEqual(answer.Value, recorded[answer.Key], answer.Key);
		Assert.AreEqual(6, db.SeederChoices.Count(x => x.Version == "legacy"), "Do not delete old answer history.");
		Assert.AreEqual(skillIds.Count, db.TraitDefinitions.Count(x => x.Type == 0));
		Assert.IsTrue(db.TraitDefinitions.Where(x => x.Type == 0).All(x => skillIds[x.Name] == x.Id));
		Assert.AreEqual(modifiers.Count, db.CheckTemplateDifficulties.Count(), "Resume must restore the missing difficulty without duplicate keys.");
		Assert.IsTrue(db.CheckTemplateDifficulties.AsEnumerable().All(x => modifiers[(x.CheckTemplateId, x.Difficulty)] == x.Modifier));
	}

	[TestMethod]
	public void StaleSavedPreset_CannotOverrideNewerCompleteLegacyHistory()
	{
		using var db = Fixture();
		IDatabaseSeeder seeder = new SkillPackageSeeder();
		SeederAnswerMemory.PersistAnswers(db, seeder, seeder.Questions,
			new Dictionary<string, string> { ["skillpreset"] = "Complex-Gerund-Modern" }, "old-preset", DateTime.UtcNow.AddDays(-2));
		var legacy = new Dictionary<string, string>
		{
			["complexity"] = "simple", ["gerund"] = "no", ["modern"] = "no",
			["skillcapmodel"] = "flat", ["skillgainmodel"] = "labmud", ["branching"] = "yes"
		};
		SeederAnswerMemory.PersistAnswers(db, seeder, seeder.Questions, legacy, "newer-legacy", DateTime.UtcNow.AddDays(-1));
		db.SaveChanges();
		var question = seeder.Questions.First();
		var stale = SeederAnswerMemory.GetRememberedAnswer(db, seeder, question, new Dictionary<string, string>());
		Assert.AreEqual("Complex-Gerund-Modern", stale, "The shared memory layer prefers an existing explicit answer.");
		Assert.IsFalse(question.Validator(stale!, db).Success, "Console workflow must reject the stale default.");
		Assert.IsTrue(question.Validator("Simple-NonGerund", db).Success);
		Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedData(db, Answers(stale!)));
		Assert.IsFalse(db.TraitDefinitions.Any(x => x.Type == 0));
	}

	[TestMethod]
	public void ArmageddonPreset_RerunRetainsBuilderAndPlayerValues()
	{
		using var db = Fixture();
		var seeder = new SkillPackageSeeder();
		seeder.SeedData(db, Answers("Armageddon"));
		var skills = ArmageddonSkillPackage.Resolve(db);
		var peek = skills["arm.skill.peek"];
		var id = peek.Id;
		peek.Name = "Builder Peek";
		peek.Expression.Expression = "87";
		db.CharacterTraits.Add(new CharacterTrait { CharacterId = 9000, TraitDefinitionId = id, Value = 57, AdditionalValue = 3 });
		db.SaveChanges();
		db.ChangeTracker.Clear();
		seeder.SeedData(db, Answers("Armageddon"));
		db.ChangeTracker.Clear();
		var retained = ArmageddonSkillPackage.Resolve(db)["arm.skill.peek"];
		Assert.AreEqual(id, retained.Id);
		Assert.AreEqual("Builder Peek", retained.Name);
		Assert.AreEqual("87", retained.Expression.Expression);
		Assert.AreEqual(57.0, db.CharacterTraits.Single().Value);
		Assert.AreEqual(3.0, db.CharacterTraits.Single().AdditionalValue);
		Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedData(db, Answers("Simple-Gerund")));
	}

	[TestMethod]
	public void ConflictingOrIncompleteInputs_FailBeforeWritesAndHaveNoInferredDefault()
	{
		using var db = Fixture();
		var seeder = new SkillPackageSeeder();
		var answers = Answers("Complex-Gerund");
		answers["gerund"] = "no";
		Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedData(db, answers));
		Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedData(db,
			new Dictionary<string, string> { ["skillpreset"] = "Simple-Gerund" }));
		Assert.IsFalse(db.TraitDefinitions.Any(x => x.Type == 0));
		Assert.IsFalse(db.Checks.Any());
		Assert.IsNull(SkillPackageSeeder.RecordedPreset(db));
		SeederAnswerMemory.PersistAnswers(db, seeder, ((IDatabaseSeeder)seeder).Questions,
			new Dictionary<string, string> { ["complexity"] = "simple" }, "legacy", DateTime.UtcNow);
		db.SaveChanges();
		Assert.IsNull(SkillPackageSeeder.RecordedPreset(db));
	}

	[TestMethod]
	public void Normalization_IsIdempotentPreservesCustomPolicyAndAcceptsLegacyBooleanAliases()
	{
		var seeder = new SkillPackageSeeder();
		var legacy = new Dictionary<string, string>
		{
			["complexity"] = "RPI", ["gerund"] = "Y", ["modern"] = "n", ["branching"] = "n",
			["skillcapmodel"] = "class", ["skillgainmodel"] = "successtree"
		};
		var normalized = seeder.NormalizeAnswers(legacy);
		Assert.AreEqual("RPI-Gerund", normalized["skillpreset"]);
		Assert.AreEqual("class", normalized["skillcapmodel"]);
		Assert.AreEqual("successtree", normalized["skillgainmodel"]);
		Assert.AreEqual("n", normalized["branching"]);
		CollectionAssert.AreEquivalent(normalized.ToArray(), seeder.NormalizeAnswers(normalized).ToArray());
	}
}
