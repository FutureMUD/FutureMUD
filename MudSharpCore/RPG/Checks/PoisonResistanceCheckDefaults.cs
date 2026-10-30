using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

#nullable enable
namespace MudSharp.RPG.Checks;

/// <summary>Physiological defaults for a missing poison check on runtime upgrade; existing checks are untouched.</summary>
internal static class PoisonResistanceCheckDefaults
{
	internal static Models.Check Create(FuturemudDatabaseContext db)
	{
		var endurance = db.TraitDefinitions.Where(x => x.Type == 1).ToArray().FirstOrDefault(x =>
			new[] { "Endurance", "Constitution", "Hardiness", "Resilience", "Stamina" }.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
		var template = db.CheckTemplates.Include(x => x.CheckTemplateDifficulties).SingleOrDefault(x => x.Name == "Poison Resistance Check");
		if (template is null)
		{
			var standard = db.CheckTemplates.Include(x => x.CheckTemplateDifficulties).FirstOrDefault(x => x.Name == "Skill Check No Improvement");
			var modifiers = new[] { 100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100 };
			template = new CheckTemplate { Name = "Poison Resistance Check", CheckMethod = "Standard", ImproveTraits = false,
				CanBranchIfTraitMissing = false, FailIfTraitMissingMode = 0 };
			foreach (var difficulty in Enum.GetValues<Difficulty>())
				template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty { Difficulty = (int)difficulty,
					Modifier = standard?.CheckTemplateDifficulties.FirstOrDefault(x => x.Difficulty == (int)difficulty)?.Modifier ?? modifiers[(int)difficulty] });
		}
		var expression = new Models.TraitExpression { Name = "Poison Resistance Formula", Expression = endurance is null ? "0" : "endurance * 5" };
		if (endurance is not null) expression.TraitExpressionParameters.Add(new TraitExpressionParameters
		{ Parameter = "endurance", TraitDefinitionId = endurance.Id, CanImprove = false, CanBranch = false });
		return new Models.Check { Type = (int)CheckType.PoisonResistanceCheck, CheckTemplate = template, TraitExpression = expression,
			MaximumDifficultyForImprovement = (int)Difficulty.Automatic };
	}
}
