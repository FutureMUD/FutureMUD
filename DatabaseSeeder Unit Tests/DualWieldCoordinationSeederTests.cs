#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using Check = MudSharp.Models.Check;
using TraitExpression = MudSharp.Models.TraitExpression;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DualWieldCoordinationSeederTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);

	private static TraitDefinition Skill(long id = 10, string name = "Native coordination") => new()
	{
		Id = id, Name = name, Type = 0, OwnerScope = 1,
		Expression = new TraitExpression { Name = "Custom cap", Expression = "37" }
	};

	private static Check Coordination(FuturemudDatabaseContext context) => context.Checks
		.Include(x => x.CheckTemplate).ThenInclude(x => x.CheckTemplateDifficulties)
		.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
		.Single(x => x.Type == (int)CheckType.DualWieldCoordinationCheck);

	[TestMethod]
	public void FreshBinding_UsesNativeNumericSkillAndRequiredNonbranchingPolicyBeforeEnabling()
	{
		using var context = Context();
		var skill = Skill();
		context.Add(skill);
		context.SaveChanges();
		var placeholder = DualWieldCoordinationSeederHelper.EnsureUnboundCheck(context);
		Assert.AreEqual("0", placeholder.TraitExpression.Expression);
		Assert.AreEqual(0, context.StaticConfigurations.Count());

		var result = DualWieldCoordinationSeederHelper.InstallFresh(context, skill);

		Assert.IsTrue(result.Success, result.Message);
		var check = Coordination(context);
		Assert.AreEqual($"dualwield:{skill.Id}{{nobranch}}", check.TraitExpression.Expression);
		var parameter = check.TraitExpression.TraitExpressionParameters.Single();
		Assert.AreEqual(skill.Id, parameter.TraitDefinitionId);
		Assert.IsTrue(parameter.CanImprove);
		Assert.IsFalse(parameter.CanBranch);
		Assert.AreEqual("Standard", check.CheckTemplate.CheckMethod);
		Assert.IsTrue(check.CheckTemplate.ImproveTraits);
		Assert.AreEqual((short)FailIfTraitMissingType.FailIfAnyMissing, check.CheckTemplate.FailIfTraitMissingMode);
		Assert.IsFalse(check.CheckTemplate.CanBranchIfTraitMissing);
		Assert.AreEqual(11, check.CheckTemplate.CheckTemplateDifficulties.Count);
		Assert.AreEqual("true", context.StaticConfigurations.Single(x =>
			x.SettingName == DualWieldCoordinationSeederHelper.EnabledSetting).Definition);
		Assert.AreEqual("37", skill.Expression.Expression);
		Assert.AreEqual(0, context.Traits.Count());
	}

	[TestMethod]
	public void Binding_RerunAndTraitRename_PreserveIdsCustomFormulaTemplateAndExplicitSettings()
	{
		using var context = Context();
		var skill = Skill();
		context.Add(skill);
		context.StaticConfigurations.AddRange(
			new StaticConfiguration { SettingName = DualWieldCoordinationSeederHelper.EnabledSetting, Definition = "false" },
			new StaticConfiguration { SettingName = "DualWieldDoubleAttackRecoveryMultiplier", Definition = "1.8" });
		context.SaveChanges();
		Assert.IsTrue(DualWieldCoordinationSeederHelper.InstallFresh(context, skill).Success);
		var check = Coordination(context);
		var expressionId = check.TraitExpressionId;
		var templateId = check.CheckTemplateId;
		check.TraitExpression.Expression += "+7";
		check.CheckTemplate.CheckTemplateDifficulties.Single(x => x.Difficulty == (int)Difficulty.Normal).Modifier = 4;
		skill.Name = "Renamed builder skill";
		context.SaveChanges();
		var counts = (context.Checks.Count(), context.TraitExpressions.Count(), context.CheckTemplates.Count(),
			context.StaticConfigurations.Count());

		var result = DualWieldCoordinationSeederHelper.BindExistingSkill(context, skill.Id, enableFeature: true);

		Assert.IsTrue(result.Success, result.Message);
		Assert.IsFalse(result.Changed);
		Assert.AreEqual(expressionId, check.TraitExpressionId);
		Assert.AreEqual(templateId, check.CheckTemplateId);
		Assert.AreEqual($"dualwield:{skill.Id}{{nobranch}}+7", check.TraitExpression.Expression);
		Assert.AreEqual(4.0, check.CheckTemplate.CheckTemplateDifficulties.Single(x => x.Difficulty == (int)Difficulty.Normal).Modifier);
		Assert.AreEqual("false", context.StaticConfigurations.Single(x =>
			x.SettingName == DualWieldCoordinationSeederHelper.EnabledSetting).Definition);
		Assert.AreEqual("1.8", context.StaticConfigurations.Single(x => x.SettingName == "DualWieldDoubleAttackRecoveryMultiplier").Definition);
		Assert.AreEqual(counts, (context.Checks.Count(), context.TraitExpressions.Count(), context.CheckTemplates.Count(),
			context.StaticConfigurations.Count()));
	}

	[TestMethod]
	public void ExistingWorldBinding_ReplacesOnlyZeroPlaceholderAndDoesNotRewriteSharedExpression()
	{
		using var context = Context();
		var skill = Skill();
		var shared = new TraitExpression { Name = "Loader fallback", Expression = "0" };
		var ordinary = new CheckTemplate { Name = "Skill Check", CheckMethod = "Standard", CanBranchIfTraitMissing = true };
		context.Add(skill);
		context.Checks.AddRange(
			new Check { Type = (int)CheckType.DualWieldCoordinationCheck, TraitExpression = shared, CheckTemplate = ordinary },
			new Check { Type = (int)CheckType.GenericSkillCheck, TraitExpression = shared, CheckTemplate = ordinary });
		context.SaveChanges();
		var sharedId = shared.Id;

		var result = DualWieldCoordinationSeederHelper.BindExistingSkill(context, skill.Id);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreNotEqual(sharedId, Coordination(context).TraitExpressionId);
		Assert.AreEqual("0", context.Checks.Single(x => x.Type == (int)CheckType.GenericSkillCheck).TraitExpression.Expression);
		Assert.AreEqual("false", context.StaticConfigurations.Single(x =>
			x.SettingName == DualWieldCoordinationSeederHelper.EnabledSetting).Definition);
	}

	[DataTestMethod]
	[DataRow("50")]
	[DataRow("custom:10{nobranch}+5")]
	public void ExistingCustomConfiguration_IsPreservedWithoutAnyMutation(string formula)
	{
		using var context = Context();
		context.Add(Skill());
		var check = new Check
		{
			Type = (int)CheckType.DualWieldCoordinationCheck,
			TraitExpression = new TraitExpression { Name = "Builder formula", Expression = formula },
			CheckTemplate = new CheckTemplate { Name = "Builder policy", CheckMethod = "Standard", CanBranchIfTraitMissing = true }
		};
		context.Add(check);
		context.SaveChanges();
		var expressionId = check.TraitExpressionId;
		var templateId = check.CheckTemplateId;

		var result = DualWieldCoordinationSeederHelper.BindExistingSkill(context, 10, enableFeature: true);

		Assert.IsFalse(result.Success);
		Assert.IsFalse(result.Changed);
		Assert.AreEqual(formula, check.TraitExpression.Expression);
		Assert.AreEqual(expressionId, check.TraitExpressionId);
		Assert.AreEqual(templateId, check.CheckTemplateId);
		Assert.AreEqual(1, context.CheckTemplates.Count());
		Assert.AreEqual(1, context.TraitExpressions.Count(x => x.Name == "Builder formula"));
		Assert.AreEqual(0, context.StaticConfigurations.Count());
		Assert.IsFalse(context.ChangeTracker.HasChanges());
	}

	[DataTestMethod]
	[DataRow("dualwield:10{nobranch}+again:10{nobranch}")]
	[DataRow("dualwield:10{nobranch}+unknown")]
	[DataRow("dualwield:10{nobranch}+")]
	[DataRow("dualwield:10{NOBRANCH}")]
	public void Binding_UnsupportedCustomLearningOrFormula_IsPreservedWithoutEnabling(string formula)
	{
		using var context = Context();
		context.Add(Skill());
		context.SaveChanges();
		Assert.IsTrue(DualWieldCoordinationSeederHelper.BindExistingSkill(context, 10).Success);
		var check = Coordination(context);
		check.TraitExpression.Expression = formula;
		context.SaveChanges();

		var result = DualWieldCoordinationSeederHelper.BindExistingSkill(context, 10, enableFeature: true);

		Assert.IsFalse(result.Success);
		Assert.AreEqual(formula, check.TraitExpression.Expression);
		Assert.AreEqual("false", context.StaticConfigurations.Single(x =>
			x.SettingName == DualWieldCoordinationSeederHelper.EnabledSetting).Definition);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
	}

	[TestMethod]
	public void Binding_CompatibleSupplementaryAttribute_PreservesFormulaAndLearningFlags()
	{
		using var context = Context();
		context.AddRange(Skill(), new TraitDefinition { Id = 20, Name = "Dexterity", Type = 1 });
		context.SaveChanges();
		Assert.IsTrue(DualWieldCoordinationSeederHelper.BindExistingSkill(context, 10).Success);
		var check = Coordination(context);
		check.TraitExpression.Expression += "+dex:20{noimprove,nobranch}/2";
		context.SaveChanges();
		var customFormula = check.TraitExpression.Expression;

		var result = DualWieldCoordinationSeederHelper.BindExistingSkill(context, 10);

		Assert.IsTrue(result.Success, result.Message);
		Assert.IsFalse(result.Changed);
		Assert.AreEqual(customFormula, check.TraitExpression.Expression);
	}

	[DataTestMethod]
	[DataRow(999L, 0, 1)]
	[DataRow(10L, 1, 1)]
	[DataRow(10L, 0, 0)]
	public void Binding_InvalidOrNoncharacterSkill_IsRejectedBeforeChanges(long requestedId, int type, int scope)
	{
		using var context = Context();
		var skill = Skill();
		skill.Type = type;
		skill.OwnerScope = scope;
		context.Add(skill);
		context.SaveChanges();

		var result = DualWieldCoordinationSeederHelper.BindExistingSkill(context, requestedId);

		Assert.IsFalse(result.Success);
		Assert.IsFalse(result.Changed);
		Assert.AreEqual(0, context.Checks.Count());
		Assert.AreEqual(0, context.CheckTemplates.Count());
		Assert.AreEqual(0, context.StaticConfigurations.Count());
		Assert.IsFalse(context.ChangeTracker.HasChanges());
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void BothGeneralSkillRoutes_CreateUnboundPlaceholderAndPreserveSubsequentBinding(bool package)
	{
		using var context = Context();
		context.TraitDefinitions.AddRange(
			new TraitDefinition { Name = "Body", Alias = "body", Type = 1 },
			new TraitDefinition { Name = "Intellect", Alias = "int", Type = 1 },
			new TraitDefinition { Name = "Spirit", Alias = "wil", Type = 1 });
		var skill = Skill();
		context.Add(skill);
		context.SaveChanges();
		var templates = (IReadOnlyDictionary<string, CheckTemplate>)typeof(SkillSeederBase)
			.GetMethod("SeedCheckTemplates", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [context, true])!;
		context.SaveChanges();
		var aliases = new[]
		{
			"Medicine", "Perception", "Stealth", "Security", "Athletics", "Survival", "Armour Use",
			"Poisoning", "Groom", "Literacy", "Handwriting", "Tattoo", "Meteorology"
		}.ToDictionary(x => x, _ => skill);
		void SeedChecks()
		{
			if (package)
			{
				typeof(SkillPackageSeeder).GetMethod("SeedChecks", BindingFlags.NonPublic | BindingFlags.Instance)!
					.Invoke(new SkillPackageSeeder(), [context, new Dictionary<string, string>(), templates, aliases]);
			}
			else
			{
				typeof(SkillSeeder).GetMethod("SeedChecks", BindingFlags.NonPublic | BindingFlags.Instance)!
					.Invoke(new SkillSeeder(), [context, new Dictionary<string, string> { ["branching"] = "yes" }]);
			}
		}

		SeedChecks();
		Assert.AreEqual("0", Coordination(context).TraitExpression.Expression);
		Assert.IsFalse(context.StaticConfigurations.Any(x => x.SettingName.StartsWith("DualWield")));
		Assert.IsTrue(DualWieldCoordinationSeederHelper.BindExistingSkill(context, skill.Id).Success);
		var boundId = Coordination(context).TraitExpressionId;

		SeedChecks();

		Assert.AreEqual(boundId, Coordination(context).TraitExpressionId);
		Assert.AreEqual($"dualwield:{skill.Id}{{nobranch}}", Coordination(context).TraitExpression.Expression);
		Assert.IsFalse(Coordination(context).CheckTemplate.CanBranchIfTraitMissing);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void UniversalCombatSkills_EnsureCoordinationBeforeEveryWeaponPackage(bool gerund)
	{
		using var context = Context();
		context.AddRange(
			new TraitDefinition { Name = "Strength", Alias = "str", Type = 1 },
			new TraitDefinition { Name = "Agility", Alias = "agi", Type = 1 },
			new TraitDefinition { Name = gerund ? "Tracking" : "Track", Type = 0, OwnerScope = 1 },
			new TraitDefinition { Name = "Armour Use", Type = 0, OwnerScope = 1,
				Expression = new TraitExpression { Name = "Armour cap", Expression = "70" } },
			new FutureProg { FunctionName = "AlwaysTrue" }, new FutureProg { FunctionName = "AlwaysFalse" },
			new Improver { Name = "Skill Improver" }, new TraitDecorator { Name = "General Skill" },
			new TraitDecorator { Name = "Veterancy Skill" });
		context.SaveChanges();
		var answers = new Dictionary<string, string> { ["parryoption"] = "no" };
		var method = typeof(CombatSeeder).GetMethod("SeedUniversalSkills", BindingFlags.NonPublic | BindingFlags.Instance)!;

		var skills = (IReadOnlyDictionary<string, TraitDefinition>)method.Invoke(new CombatSeeder(), [context, answers])!;
		var coordination = skills[gerund ? "Dual-Wielding" : "Dual-Wield"];
		Assert.AreEqual(0, coordination.Type);
		Assert.AreEqual(1, coordination.OwnerScope);
		Assert.AreEqual("70", coordination.Expression.Expression);
		Assert.IsTrue(DualWieldCoordinationSeederHelper.InstallFresh(context, coordination).Success);
		method.Invoke(new CombatSeeder(), [context, answers]);
		Assert.AreEqual(1, context.TraitDefinitions.Count(x => x.Name == coordination.Name));
		Assert.AreEqual(coordination.Id, Coordination(context).TraitExpression.TraitExpressionParameters.Single().TraitDefinitionId);
	}
}
