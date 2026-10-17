#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public sealed record PeekBindingResult(bool Success, bool Changed, string Message);

/// <summary>Additive binding of the two Peek checks to an explicitly selected native skill. Never grants player traits.</summary>
public static class PeekSeederHelper
{
	public const string AttemptTemplate = "Peek Attempt Check";
	public const string RevealTemplate = "Peek Reveal Check";
	private static readonly int[] Modifiers = [100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100];
	private static readonly CheckType[] Types = [CheckType.PeekCheck, CheckType.PeekRevealCheck];

	public static void EnsureUnboundChecks(FuturemudDatabaseContext db)
	{
		foreach (var type in Types)
		{
			if (Find(db, type) is not null) continue;
			db.Checks.Add(new Check
			{
				Type = (int)type, CheckTemplate = EnsureTemplate(db, type),
				MaximumDifficultyForImprovement = (int)Difficulty.Impossible,
				TraitExpression = new TraitExpression { Name = $"{type} Formula", Expression = "0" }
			});
		}
		db.SaveChanges();
	}

	public static PeekBindingResult BindExistingSkill(FuturemudDatabaseContext db, long traitId)
	{
		var skill = db.TraitDefinitions.SingleOrDefault(x => x.Id == traitId);
		if (skill is null || skill.Type != (int)TraitType.Skill || skill.OwnerScope != (int)TraitOwnerScope.Character)
			return new(false, false, "Peek must bind to an existing character skill. No configuration changed.");

		// Preflight the whole pair before writing anything. Preserve builder formulas and unrelated shared rows.
		foreach (var type in Types)
		{
			var check = Find(db, type);
			if (check is not null && !Placeholder(check) && !Compatible(check, type, traitId))
				return new(false, false, $"Existing {type} binding was preserved; review it explicitly. No configuration changed.");
			var template = FindTemplate(db, type);
			if ((check is null || Placeholder(check)) && template is not null && !Policy(template, type))
				return new(false, false, $"Existing {template.Name} policy was preserved. No configuration changed.");
		}

		using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction() : null;
		var changed = false;
		var createdParameters = new List<(TraitExpressionParameters Parameter, bool Improve)>();
		foreach (var type in Types)
		{
			var check = Find(db, type);
			if (check is not null && !Placeholder(check)) continue;
			if (check is null)
			{
				check = new Check { Type = (int)type };
				db.Checks.Add(check);
			}
			var reveal = type == CheckType.PeekRevealCheck;
			var expression = new TraitExpression
			{
				Name = $"{type} Formula",
				Expression = $"peek:{traitId}{{{(reveal ? "noimprove,nobranch" : "nobranch")}}}"
			};
			var parameter = new TraitExpressionParameters
			{
				Parameter = "peek", TraitDefinition = skill, CanImprove = !reveal, CanBranch = false
			};
			expression.TraitExpressionParameters.Add(parameter);
			createdParameters.Add((parameter, !reveal));
			check.TraitExpression = expression;
			check.CheckTemplate = EnsureTemplate(db, type);
			check.MaximumDifficultyForImprovement = (int)Difficulty.Impossible;
			changed = true;
		}

		foreach (var (key, value) in new[] { ("PeekCoveredItemDifficulty", "6"), ("PeekCarriedItemDifficulty", "5"), ("PeekContainerItemDifficulty", "5") })
		{
			if (db.StaticConfigurations.Any(x => x.SettingName == key)) continue;
			db.StaticConfigurations.Add(new StaticConfiguration { SettingName = key, Definition = value });
			changed = true;
		}
		db.SaveChanges();
		// These legacy bit columns default to true. EF can treat false on insert as a
		// generated default; explicitly persist the intended flags on our new rows.
		foreach (var (parameter, improve) in createdParameters)
		{
			parameter.CanBranch = false;
			parameter.CanImprove = improve;
			db.Entry(parameter).Property(x => x.CanBranch).IsModified = true;
			db.Entry(parameter).Property(x => x.CanImprove).IsModified = true;
		}
		if (createdParameters.Count > 0) db.SaveChanges();
		transaction?.Commit();
		return new(true, changed, $"Peek checks bound to native skill #{traitId}; player values and existing settings preserved.");
	}

	private static Check? Find(FuturemudDatabaseContext db, CheckType type) => db.Checks
		.Include(x => x.CheckTemplate).ThenInclude(x => x.CheckTemplateDifficulties)
		.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
		.SingleOrDefault(x => x.Type == (int)type);

	private static bool Placeholder(Check check) => check.TraitExpression.Expression.Trim() == "0" &&
		check.TraitExpression.TraitExpressionParameters.Count == 0;

	private static bool Compatible(Check check, CheckType type, long traitId) => Policy(check.CheckTemplate, type) &&
		check.TraitExpression.TraitExpressionParameters.Count == 1 &&
		check.TraitExpression.TraitExpressionParameters.Any(x => x.Parameter == "peek" &&
			x.TraitDefinitionId == traitId && !x.CanBranch && x.CanImprove == (type == CheckType.PeekCheck)) &&
		check.TraitExpression.Expression.Contains($"peek:{traitId}{{{(type == CheckType.PeekCheck ? "nobranch" : "noimprove,nobranch")}}}", StringComparison.Ordinal);

	private static bool Policy(CheckTemplate template, CheckType type) => template.CheckMethod == "Standard" &&
		template.ImproveTraits == (type == CheckType.PeekCheck) && !template.CanBranchIfTraitMissing &&
		template.FailIfTraitMissingMode == (short)FailIfTraitMissingType.FailIfAnyMissing &&
		Enum.GetValues<Difficulty>().All(x => template.CheckTemplateDifficulties.Any(y => y.Difficulty == (int)x));

	private static CheckTemplate? FindTemplate(FuturemudDatabaseContext db, CheckType type) => db.CheckTemplates
		.Include(x => x.CheckTemplateDifficulties)
		.SingleOrDefault(x => x.Name == (type == CheckType.PeekCheck ? AttemptTemplate : RevealTemplate));

	private static CheckTemplate EnsureTemplate(FuturemudDatabaseContext db, CheckType type)
	{
		var template = FindTemplate(db, type);
		if (template is not null)
		{
			if (!Policy(template, type)) throw new InvalidOperationException($"Customised {template.Name} policy was preserved.");
			return template;
		}
		var standard = db.CheckTemplates.Include(x => x.CheckTemplateDifficulties).FirstOrDefault(x => x.Name == "Skill Check");
		template = new CheckTemplate
		{
			Name = type == CheckType.PeekCheck ? AttemptTemplate : RevealTemplate, CheckMethod = "Standard",
			ImproveTraits = type == CheckType.PeekCheck, CanBranchIfTraitMissing = false,
			FailIfTraitMissingMode = (short)FailIfTraitMissingType.FailIfAnyMissing
		};
		foreach (var difficulty in Enum.GetValues<Difficulty>())
			template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{
				Difficulty = (int)difficulty,
				Modifier = standard?.CheckTemplateDifficulties.SingleOrDefault(x => x.Difficulty == (int)difficulty)?.Modifier ?? Modifiers[(int)difficulty]
			});
		db.CheckTemplates.Add(template);
		return template;
	}
}
