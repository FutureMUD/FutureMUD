#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using TraitDefinition = MudSharp.Models.TraitDefinition;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PeekSeederTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
	private static TraitDefinition Skill() => new() { Id = 10, Name = "Peek", Type = 0, OwnerScope = (int)TraitOwnerScope.Character };

	[TestMethod]
	public void BindPair_RerunRetainsIdsCustomisationAndExistingPlayerValues()
	{
		using var db = Context(); var skill = Skill(); db.Add(skill);
		db.CharacterTraits.Add(new CharacterTrait { CharacterId = 100, TraitDefinitionId = skill.Id, Value = 42 });
		db.SaveChanges();
		PeekSeederHelper.EnsureUnboundChecks(db);
		Assert.IsTrue(ArmageddonNonMagicSkillManifest.BindPeek(db, skill.Id).Success);
		var checks = db.Checks.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
			.Include(x => x.CheckTemplate).ThenInclude(x => x.CheckTemplateDifficulties).ToList();
		var ids = checks.Select(x => (x.Type, x.TraitExpressionId, x.CheckTemplateId)).ToArray();
		var attempt = checks.Single(x => x.Type == (int)CheckType.PeekCheck);
		var reveal = checks.Single(x => x.Type == (int)CheckType.PeekRevealCheck);
		Assert.IsTrue(attempt.CheckTemplate.ImproveTraits);
		Assert.IsFalse(reveal.CheckTemplate.ImproveTraits);
		Assert.IsTrue(checks.All(x => !x.CheckTemplate.CanBranchIfTraitMissing && x.CheckTemplate.FailIfTraitMissingMode == 1));
		Assert.IsTrue(checks.All(x => x.TraitExpression.TraitExpressionParameters.Single().TraitDefinitionId == skill.Id));
		Assert.IsFalse(reveal.TraitExpression.TraitExpressionParameters.Single().CanImprove);
		Assert.IsFalse(reveal.TraitExpression.TraitExpressionParameters.Single().CanBranch);
		skill.Name = "Renamed covert inspection";
		attempt.TraitExpression.Expression += "+3";
		reveal.CheckTemplate.CheckTemplateDifficulties.Single(x => x.Difficulty == (int)Difficulty.Hard).Modifier = -12;
		db.StaticConfigurations.Single(x => x.SettingName == "PeekCoveredItemDifficulty").Definition = "7";
		db.SaveChanges();
		var count = (db.Checks.Count(), db.TraitExpressions.Count(), db.CheckTemplates.Count(), db.TraitDefinitions.Count());
		var rerun = ArmageddonNonMagicSkillManifest.BindPeek(db, skill.Id);
		Assert.IsTrue(rerun.Success, rerun.Message); Assert.IsFalse(rerun.Changed);
		CollectionAssert.AreEqual(ids, checks.Select(x => (x.Type, x.TraitExpressionId, x.CheckTemplateId)).ToArray());
		Assert.AreEqual(count, (db.Checks.Count(), db.TraitExpressions.Count(), db.CheckTemplates.Count(), db.TraitDefinitions.Count()));
		Assert.AreEqual(42.0, db.CharacterTraits.Single().Value);
		Assert.AreEqual("7", db.StaticConfigurations.Single(x => x.SettingName == "PeekCoveredItemDifficulty").Definition);
		Assert.IsTrue(attempt.TraitExpression.Expression.EndsWith("+3"));
		Assert.AreEqual(-12.0, reveal.CheckTemplate.CheckTemplateDifficulties.Single(x => x.Difficulty == (int)Difficulty.Hard).Modifier);
	}

	[TestMethod]
	public void ExistingCustomBinding_RejectsWholePairWithoutMutation()
	{
		using var db = Context(); db.Add(Skill()); PeekSeederHelper.EnsureUnboundChecks(db);
		var reveal = db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.PeekRevealCheck);
		reveal.TraitExpression.Expression = "50"; db.SaveChanges();
		Assert.IsFalse(PeekSeederHelper.BindExistingSkill(db, 10).Success);
		Assert.AreEqual("0", db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.PeekCheck).TraitExpression.Expression);
		Assert.AreEqual("50", reveal.TraitExpression.Expression);
		Assert.AreEqual(0, db.StaticConfigurations.Count()); Assert.IsFalse(db.ChangeTracker.HasChanges());
	}

	[TestMethod]
	public void PlaceholderSharedWithOtherCheck_RebindingDoesNotRewriteSharedRow()
	{
		using var db = Context(); db.Add(Skill()); PeekSeederHelper.EnsureUnboundChecks(db);
		var attempt = db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.PeekCheck);
		var shared = attempt.TraitExpression;
		db.Checks.Add(new Check { Type = (int)CheckType.GenericSkillCheck, TraitExpression = shared, CheckTemplate = attempt.CheckTemplate });
		db.SaveChanges(); Assert.IsTrue(PeekSeederHelper.BindExistingSkill(db, 10).Success);
		Assert.AreEqual("0", shared.Expression); Assert.AreNotEqual(shared.Id, attempt.TraitExpressionId);
	}

	[TestMethod]
	public void MinimalSkillSeeder_CreatesDisabledSafePlaceholdersWithoutGrantingPeek()
	{
		using var db = Context();
		typeof(SkillSeeder).GetMethod("SeedChecks", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(new SkillSeeder(), [db, new Dictionary<string, string> { ["branching"] = "yes" }]);
		foreach (var type in ArmageddonNonMagicSkillManifest.PeekChecks.Values)
		{
			var check = db.Checks.Include(x => x.TraitExpression).Include(x => x.CheckTemplate).Single(x => x.Type == (int)type);
			Assert.AreEqual("0", check.TraitExpression.Expression);
			Assert.IsFalse(check.CheckTemplate.CanBranchIfTraitMissing);
			Assert.AreEqual(type == CheckType.PeekCheck, check.CheckTemplate.ImproveTraits);
		}
		Assert.AreEqual(0, db.TraitDefinitions.Count()); Assert.AreEqual(0, db.Traits.Count());
	}

	[TestMethod]
	public void StockCatalogueAndArmageddonManifest_KeepDistinctPeekAndSourceMemberships()
	{
		var seeder = new SkillPackageSeeder();
		foreach (var list in new[] { "CovertInspectionSkills" })
		{
			var details = (IEnumerable<SkillPackageSeeder.SkillDetails>)typeof(SkillPackageSeeder)
				.GetProperty(list, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(seeder)!;
			Assert.AreEqual("Peek", details.Single().ImperativeName);
			Assert.AreEqual("Peek", details.Single().GerundName);
		}
		Assert.AreEqual(1, seeder.ComplexNonGerundSkillNamesForTesting.Count(x => x == "Peek"));
		CollectionAssert.AreEqual(new[] { "Burglar", "Pick-pocket" }, ArmageddonNonMagicSkillManifest.PeekSourceMemberships.ToArray());
		Assert.AreEqual(236, (int)CheckType.PeekCheck); Assert.AreEqual(237, (int)CheckType.PeekRevealCheck);
		Assert.AreEqual(Enum.GetValues<CheckType>().Length, Enum.GetValues<CheckType>().Distinct().Count());
	}

	[TestMethod]
	public void Loader_ConflictingSameNameTemplate_IsPreservedWithStableSafeFallback()
	{
		using var db = Context();
		var custom = new CheckTemplate { Name = PeekSeederHelper.RevealTemplate, CheckMethod = "Static", ImproveTraits = true };
		db.Add(custom); db.SaveChanges();
		CheckTemplate Ensure() => (CheckTemplate)typeof(Futuremud)
			.GetMethod("CreatePeekCheckTemplate", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [db, CheckType.PeekRevealCheck])!;
		var template = Ensure();
		db.SaveChanges();
		Assert.AreNotEqual(custom.Id, template.Id);
		Assert.AreEqual("Static", custom.CheckMethod);
		Assert.AreEqual(1, db.CheckTemplates.Count(x => x.Name == PeekSeederHelper.RevealTemplate));
		Assert.IsFalse(template.ImproveTraits); Assert.IsFalse(template.CanBranchIfTraitMissing);
		Assert.AreEqual(template.Id, Ensure().Id);
		db.Add(Skill()); db.SaveChanges();
		var result = PeekSeederHelper.BindExistingSkill(db, 10);
		Assert.IsFalse(result.Success); Assert.IsFalse(db.ChangeTracker.HasChanges());
	}
}
