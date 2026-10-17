using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;

#nullable enable

namespace DatabaseSeeder.Seeders;

/// <summary>Additive optional checks. No skill creation, player grants, or feature enablement.</summary>
public static class GuardingCheckSeederHelper
{
	public const string TemplateName = "Guarding Challenge Check";
	private static readonly CheckType[] Types = [CheckType.GuardingExitChallengeCheck, CheckType.GuardingItemChallengeCheck];
	private static readonly int[] Modifiers = [100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100];
	public static string ExpressionName(CheckType type) => $"{type} Formula";

	public static void EnsureChecks(FuturemudDatabaseContext context)
	{
		foreach (var type in Types)
		{
			if (context.Checks.Any(x => x.Type == (int)type)) continue;
			context.Checks.Add(new Check
			{
				Type = (int)type, MaximumDifficultyForImprovement = (int)Difficulty.Impossible,
				CheckTemplate = EnsureTemplate(context),
				TraitExpression = new TraitExpression { Name = ExpressionName(type), Expression = "15" }
			});
			context.SaveChanges();
		}
		if (!context.StaticConfigurations.Any(x => x.SettingName == "ArmageddonHostileGuardAttemptsEnabled"))
			context.StaticConfigurations.Add(new StaticConfiguration { SettingName = "ArmageddonHostileGuardAttemptsEnabled", Definition = "false" });
		context.SaveChanges();
	}

	/// <summary>Explicitly bind both stock placeholders to the same existing character skill ID.</summary>
	public static bool BindExistingSkill(FuturemudDatabaseContext context, long traitId, out string message)
	{
		var skill = context.TraitDefinitions.SingleOrDefault(x => x.Id == traitId);
		if (skill is null || skill.Type != 0 || skill.OwnerScope != (int)TraitOwnerScope.Character)
		{
			message = "Choose an existing character skill ID. No configuration changed.";
			return false;
		}
		var checks = context.Checks.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
			.Include(x => x.CheckTemplate).Where(x => Types.Select(y => (int)y).Contains(x.Type)).ToArray();
		var formula = $"max(15, guarding:{traitId}{{nobranch}})";
		var existingTemplate = context.CheckTemplates.SingleOrDefault(x => x.Name == TemplateName);
		// Preflight both bindings before writing either. Builder-owned expressions/templates always win.
		if (existingTemplate is not null && !CompatibleTemplate(existingTemplate) ||
		    checks.Any(x => !CompatibleTemplate(x.CheckTemplate) ||
		    x.TraitExpression.Expression != formula &&
		    !(x.TraitExpression.Name == ExpressionName((CheckType)x.Type) && x.TraitExpression.Expression.Trim() == "15" &&
		      x.TraitExpression.TraitExpressionParameters.Count == 0) ||
		    x.TraitExpression.Expression == formula && x.TraitExpression.TraitExpressionParameters.Any(p =>
		        p.Parameter != "guarding" || p.TraitDefinitionId != traitId || !p.CanImprove || p.CanBranch)))
		{
			message = "Existing Guarding formula or template is customised; review it explicitly. No configuration changed.";
			return false;
		}
		using var transaction = context.Database.IsRelational() && context.Database.CurrentTransaction is null
			? context.Database.BeginTransaction() : null;
		EnsureChecks(context);
		var createdParameters = new List<TraitExpressionParameters>();
		foreach (var type in Types)
		{
			var check = context.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)type);
			if (check.TraitExpression.Expression == formula) continue;
			// Replace, never edit a possibly shared expression row.
			var expression = new TraitExpression { Name = ExpressionName(type), Expression = formula };
			var parameter = new TraitExpressionParameters
			{
				Parameter = "guarding", TraitDefinition = skill, CanImprove = true, CanBranch = false
			};
			expression.TraitExpressionParameters.Add(parameter);
			createdParameters.Add(parameter);
			check.TraitExpression = expression;
		}
		context.SaveChanges();
		// Legacy bit columns default to true; EF may omit false values on insert.
		// Persist explicit policy only on the new parameter rows owned by this binding.
		foreach (var parameter in createdParameters)
		{
			parameter.CanBranch = false;
			parameter.CanImprove = true;
			context.Entry(parameter).Property(x => x.CanBranch).IsModified = true;
			context.Entry(parameter).Property(x => x.CanImprove).IsModified = true;
		}
		if (createdParameters.Count > 0) context.SaveChanges();
		transaction?.Commit();
		message = $"Both Guarding reactions now use existing character skill #{traitId}. Settings and player values were preserved.";
		return true;
	}

	private static bool CompatibleTemplate(CheckTemplate template) => template.Name == TemplateName &&
		template.CheckMethod == "Standard" && template.ImproveTraits && !template.CanBranchIfTraitMissing &&
		template.FailIfTraitMissingMode == (short)FailIfTraitMissingType.DoNotFail;

	private static CheckTemplate EnsureTemplate(FuturemudDatabaseContext context)
	{
		var existing = context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).SingleOrDefault(x => x.Name == TemplateName);
		if (existing is not null)
		{
			if (!CompatibleTemplate(existing)) throw new InvalidOperationException("Custom Guarding check template was preserved; review its policy before installing missing checks.");
			return existing;
		}
		var standard = context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).FirstOrDefault(x => x.Name == "Skill Check" || x.Name == "SkillCheck");
		var template = new CheckTemplate
		{
			Name = TemplateName, Definition = "<Definition />", CheckMethod = "Standard", ImproveTraits = true,
			CanBranchIfTraitMissing = false, FailIfTraitMissingMode = (short)FailIfTraitMissingType.DoNotFail
		};
		foreach (var difficulty in Enum.GetValues<Difficulty>())
			template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{
				Difficulty = (int)difficulty,
				Modifier = standard?.CheckTemplateDifficulties.SingleOrDefault(x => x.Difficulty == (int)difficulty)?.Modifier ?? Modifiers[(int)difficulty]
			});
		context.CheckTemplates.Add(template);
		return template;
	}
}
