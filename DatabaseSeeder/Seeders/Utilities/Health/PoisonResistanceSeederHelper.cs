using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

#nullable enable
namespace DatabaseSeeder.Seeders;

/// <summary>Creates a configurable native physiological poison save without replacing existing builder policy.</summary>
internal static class PoisonResistanceSeederHelper
{
	internal static Check Ensure(FuturemudDatabaseContext db, TraitDefinition? endurance = null,
		Func<long>? expressionId = null, Func<long>? templateId = null)
	{
		var existing = db.Checks.Local.FirstOrDefault(x => x.Type == (int)CheckType.PoisonResistanceCheck) ??
			db.Checks.SingleOrDefault(x => x.Type == (int)CheckType.PoisonResistanceCheck);
		if (existing is not null) return existing;
		endurance ??= db.TraitDefinitions.Where(x => x.Type == 1).ToArray().FirstOrDefault(x =>
			new[] { "Endurance", "Constitution", "Hardiness", "Resilience", "Stamina" }.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
		var template = db.CheckTemplates.Include(x => x.CheckTemplateDifficulties).SingleOrDefault(x => x.Name == "Poison Resistance Check");
		if (template is null)
		{
			var standard = db.CheckTemplates.Include(x => x.CheckTemplateDifficulties).FirstOrDefault(x => x.Name == "Skill Check No Improvement");
			var modifiers = new[] { 100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100 };
			template = new CheckTemplate { Id = templateId?.Invoke() ?? 0, Name = "Poison Resistance Check", CheckMethod = "Standard", ImproveTraits = false,
				CanBranchIfTraitMissing = false, FailIfTraitMissingMode = 0 };
			foreach (var difficulty in Enum.GetValues<Difficulty>())
				template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty { Difficulty = (int)difficulty,
					Modifier = standard?.CheckTemplateDifficulties.FirstOrDefault(x => x.Difficulty == (int)difficulty)?.Modifier ?? modifiers[(int)difficulty] });
			db.CheckTemplates.Add(template);
		}
		var expression = new TraitExpression { Id = expressionId?.Invoke() ?? 0, Name = "Poison Resistance Formula", Expression = endurance is null ? "0" : "endurance * 5" };
		if (endurance is not null) expression.TraitExpressionParameters.Add(new TraitExpressionParameters
		{ Parameter = "endurance", TraitDefinitionId = endurance.Id, CanImprove = false, CanBranch = false });
		var check = new Check { Type = (int)CheckType.PoisonResistanceCheck, CheckTemplate = template, TraitExpression = expression,
			MaximumDifficultyForImprovement = (int)Difficulty.Automatic };
		db.Checks.Add(check);
		return check;
	}
}
