using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

#nullable enable

namespace MudSharp.RPG.Checks;

/// <summary>Missing-check startup backfill. Existing check rows, traits and player values are untouched.</summary>
internal static class GuardingCheckDefaults
{
	internal static CheckTemplate EnsureTemplate(FuturemudDatabaseContext context)
	{
		const string name = "Guarding Challenge Check";
		var existing = context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).SingleOrDefault(x => x.Name == name);
		if (existing is not null) return existing;
		var standard = context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).FirstOrDefault(x => x.Name == "SkillCheck" || x.Name == "Skill Check");
		int[] modifiers = [100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100];
		var template = new CheckTemplate
		{
			Name = name, Definition = "<Definition />", CheckMethod = "Standard", ImproveTraits = true,
			CanBranchIfTraitMissing = false, FailIfTraitMissingMode = (short)FailIfTraitMissingType.DoNotFail
		};
		foreach (var difficulty in Enum.GetValues<Difficulty>())
			template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{
				Difficulty = (int)difficulty,
				Modifier = standard?.CheckTemplateDifficulties.SingleOrDefault(x => x.Difficulty == (int)difficulty)?.Modifier ?? modifiers[(int)difficulty]
			});
		context.CheckTemplates.Add(template);
		return template;
	}
}
