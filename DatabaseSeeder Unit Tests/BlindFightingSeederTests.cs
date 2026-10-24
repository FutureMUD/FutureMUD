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
using MudSharp.Models;
using MudSharp.RPG.Checks;
using TraitDefinition = MudSharp.Models.TraitDefinition;
using TraitExpression = MudSharp.Models.TraitExpression;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BlindFightingSeederTests
{
	[TestMethod]
	public void EnsureBlindFighting_ExistingHaggleCheck_PreservesItAndCreatesSeparateCheck()
	{
		using var context = Context();
		SeedCombatSkills(context);
		var haggle = new Check
		{
			Type = 227,
			CheckTemplate = context.CheckTemplates.Single(x => x.Name == "Skill Check"),
			MaximumDifficultyForImprovement = (int)Difficulty.Hard,
			TraitExpression = new TraitExpression { Name = "Builder Haggle", Expression = "variable + 12" }
		};
		context.Checks.Add(haggle);
		context.SaveChanges();
		var expressionId = haggle.TraitExpressionId;

		Assert.IsTrue(CombatSeeder.EnsureBlindFighting(context));
		context.ChangeTracker.Clear();

		Assert.AreEqual(227, (int)CheckType.HaggleCheck, "Preserve the shipped Haggle check identity.");
		var preserved = context.Checks.Include(x => x.TraitExpression)
			.Single(x => x.Type == (int)CheckType.HaggleCheck);
		Assert.AreEqual(expressionId, preserved.TraitExpressionId);
		Assert.AreEqual("variable + 12", preserved.TraitExpression.Expression);
		Assert.AreEqual((int)Difficulty.Hard, preserved.MaximumDifficultyForImprovement);
		Assert.IsTrue(context.Checks.Any(x => x.Type == (int)CheckType.BlindFightingCheck));
		Assert.AreEqual(1, Enum.GetValues<CheckType>().Count(x => (int)x == (int)CheckType.BlindFightingCheck),
			"Blind Fighting must have its own persisted identity.");
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FreshSkillFoundations_DeferBlindFightingUntilCombatCanBindItsSkill(bool useSkillPackage)
	{
		using var context = Context();
		SeedSkillChecks(context, useSkillPackage);
		Assert.IsFalse(context.Checks.Any(x => x.Type == (int)CheckType.BlindFightingCheck));
		SeedCombatSkills(context);

		Assert.IsTrue(CombatSeeder.EnsureBlindFighting(context));
		var skill = context.TraitDefinitions.Include(x => x.Expression)
			.Single(x => x.Name == CombatSeeder.BlindFightingSkillName);
		var check = context.Checks.Include(x => x.CheckTemplate).Include(x => x.TraitExpression)
			.Single(x => x.Type == (int)CheckType.BlindFightingCheck);
		Assert.AreEqual((int)TraitType.Skill, skill.Type);
		Assert.AreEqual((int)TraitOwnerScope.Character, skill.OwnerScope);
		Assert.AreEqual("Combat", skill.TraitGroup);
		Assert.AreEqual("70", skill.Expression.Expression);
		Assert.AreEqual(context.Improvers.Single(x => x.Name == "Skill Improver").Id, skill.ImproverId);
		Assert.AreEqual($"blindfighting:{skill.Id}", check.TraitExpression.Expression);
		Assert.AreEqual("Skill Check", check.CheckTemplate.Name);
		Assert.IsTrue(check.CheckTemplate.ImproveTraits);
		Assert.AreEqual((int)Difficulty.Impossible, check.MaximumDifficultyForImprovement);

		var expressionCount = context.TraitExpressions.Count();
		Assert.IsFalse(CombatSeeder.EnsureBlindFighting(context));
		Assert.AreEqual(1, context.TraitDefinitions.Count(x => x.Name == CombatSeeder.BlindFightingSkillName));
		Assert.AreEqual(expressionCount, context.TraitExpressions.Count());
	}

	[TestMethod]
	public void ExistingSkill_IsReusedWithoutReplacingItsBuilderConfiguration()
	{
		using var context = Context();
		var skill = new TraitDefinition
		{
			Name = "blind fighting",
			Type = (int)TraitType.Skill,
			TraitGroup = "Custom Combat",
			Hidden = true,
			BranchMultiplier = 0.25,
			Expression = new TraitExpression { Name = "Custom Blind Fighting Cap", Expression = "82" }
		};
		context.TraitDefinitions.Add(skill);
		context.SaveChanges();
		var originalId = skill.Id;
		var originalCap = skill.ExpressionId;

		Assert.IsTrue(CombatSeeder.EnsureBlindFighting(context));
		Assert.AreEqual($"blindfighting:{originalId}", context.Checks.Include(x => x.TraitExpression)
			.Single(x => x.Type == (int)CheckType.BlindFightingCheck).TraitExpression.Expression);
		Assert.AreEqual(originalCap, skill.ExpressionId);
		Assert.AreEqual("82", skill.Expression.Expression);
		Assert.AreEqual("Custom Combat", skill.TraitGroup);
		Assert.AreEqual(true, skill.Hidden);
		Assert.AreEqual(0.25, skill.BranchMultiplier);
		Assert.AreEqual(1, context.TraitDefinitions.AsEnumerable()
			.Count(x => x.Name.Equals(CombatSeeder.BlindFightingSkillName, StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	[DataRow("100")]
	[DataRow("min(80, variable) + 5")]
	public void ExistingCheck_IsPreservedAndDoesNotInventASkill(string formula)
	{
		using var context = Context();
		var check = new Check
		{
			Type = (int)CheckType.BlindFightingCheck,
			CheckTemplate = context.CheckTemplates.Single(x => x.Name == "Skill Check No Improvement"),
			MaximumDifficultyForImprovement = (int)Difficulty.Hard,
			TraitExpression = new TraitExpression { Name = "Builder Blind Fighting Formula", Expression = formula }
		};
		context.Checks.Add(check);
		context.SaveChanges();
		var expressionId = check.TraitExpressionId;
		var templateId = check.CheckTemplateId;
		var skillCount = context.TraitDefinitions.Count();
		var expressionCount = context.TraitExpressions.Count();

		Assert.IsFalse(CombatSeeder.EnsureBlindFighting(context));
		Assert.AreEqual(expressionId, check.TraitExpressionId);
		Assert.AreEqual(templateId, check.CheckTemplateId);
		Assert.AreEqual(formula, check.TraitExpression.Expression);
		Assert.AreEqual((int)Difficulty.Hard, check.MaximumDifficultyForImprovement);
		Assert.AreEqual(skillCount, context.TraitDefinitions.Count());
		Assert.AreEqual(expressionCount, context.TraitExpressions.Count());
	}

	[TestMethod]
	public void Rerun_AfterBuilderRenamesSkillAndChangesFormula_PreservesBoth()
	{
		using var context = Context();
		SeedCombatSkills(context);
		CombatSeeder.EnsureBlindFighting(context);
		var skill = context.TraitDefinitions.Include(x => x.Expression)
			.Single(x => x.Name == CombatSeeder.BlindFightingSkillName);
		var check = context.Checks.Include(x => x.TraitExpression)
			.Single(x => x.Type == (int)CheckType.BlindFightingCheck);
		skill.Name = "Echolocation";
		skill.Expression.Expression = "88";
		check.TraitExpression.Expression = $"hearing:{skill.Id} + 10";
		context.SaveChanges();
		var skillCount = context.TraitDefinitions.Count();

		Assert.IsFalse(CombatSeeder.EnsureBlindFighting(context));
		Assert.AreEqual($"hearing:{skill.Id} + 10", check.TraitExpression.Expression);
		Assert.AreEqual("88", skill.Expression.Expression);
		Assert.AreEqual("Echolocation", skill.Name);
		Assert.AreEqual(skillCount, context.TraitDefinitions.Count());
		Assert.IsFalse(context.TraitDefinitions.Any(x => x.Name == CombatSeeder.BlindFightingSkillName));
	}

	private static FuturemudDatabaseContext Context()
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		context.AddRange(new TraitDefinition { Name = "Body", Alias = "body", Type = (int)TraitType.Attribute },
			new TraitDefinition { Name = "Intellect", Alias = "intellect", Type = (int)TraitType.Attribute },
			new TraitDefinition { Name = "Spirit", Alias = "spirit", Type = (int)TraitType.Attribute });
		foreach (var name in new[]
		         {
			         "Medicine", "Perception", "Stealth", "Security", "Athletics", "Survival", "Armour Use",
			         "Riding", "Driving", "Seafaring", "Veterancy", "Poisoning", "Meteorology", "Groom",
			         "Literacy", "Handwriting", "Tattoo", "Law"
		         })
		{
			context.TraitDefinitions.Add(new TraitDefinition
			{
				Name = name,
				Type = (int)TraitType.Skill,
				Expression = new TraitExpression { Name = $"{name} Skill Cap", Expression = "70" }
			});
		}
		context.AddRange(new FutureProg { FunctionName = "AlwaysTrue" }, new FutureProg { FunctionName = "AlwaysFalse" },
			new Improver { Name = "Skill Improver" }, new TraitDecorator { Name = "General Skill" },
			new TraitDecorator { Name = "Veterancy Skill" });
		context.SaveChanges();
		SeedTemplates(context);
		return context;
	}

	private static IReadOnlyDictionary<string, CheckTemplate> SeedTemplates(FuturemudDatabaseContext context) =>
		(IReadOnlyDictionary<string, CheckTemplate>)typeof(SkillSeederBase)
			.GetMethod("SeedCheckTemplates", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [context, true])!;

	private static void SeedCombatSkills(FuturemudDatabaseContext context) =>
		typeof(CombatSeeder).GetMethod("SeedUniversalSkills", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(new CombatSeeder(), [context, new Dictionary<string, string> { ["parryoption"] = "no" }]);

	private static void SeedSkillChecks(FuturemudDatabaseContext context, bool useSkillPackage)
	{
		var answers = new Dictionary<string, string> { ["branching"] = "yes" };
		if (useSkillPackage)
		{
			var skills = context.TraitDefinitions.Where(x => x.Type == (int)TraitType.Skill)
				.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
			typeof(SkillPackageSeeder).GetMethod("SeedChecks", BindingFlags.NonPublic | BindingFlags.Instance)!
				.Invoke(new SkillPackageSeeder(), [context, answers, SeedTemplates(context), skills]);
			return;
		}

		typeof(SkillSeeder).GetMethod("SeedChecks", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(new SkillSeeder(), [context, answers]);
	}
}
