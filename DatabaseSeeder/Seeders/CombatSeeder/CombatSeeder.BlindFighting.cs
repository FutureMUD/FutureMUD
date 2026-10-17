#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
	internal const string BlindFightingSkillName = "Blind Fighting";

	internal static bool EnsureBlindFighting(FuturemudDatabaseContext context)
	{
		// An existing check may be the compatibility default or a builder's own formula.
		// Neither establishes ownership of a skill or permits replacing that formula.
		if (context.Checks.Any(x => x.Type == (int)CheckType.BlindFightingCheck))
		{
			return false;
		}

		var skill = context.TraitDefinitions
			.Where(x => x.Type == (int)TraitType.Skill)
			.AsEnumerable()
			.FirstOrDefault(x => x.Name.Equals(BlindFightingSkillName, StringComparison.OrdinalIgnoreCase));
		if (skill is null)
		{
			var brawling = context.TraitDefinitions
				.Include(x => x.Expression)
				.First(x => x.Type == (int)TraitType.Skill && (x.Name == "Brawling" || x.Name == "Brawl"));
			skill = new TraitDefinition
			{
				Name = BlindFightingSkillName,
				Type = (int)TraitType.Skill,
				OwnerScope = (int)TraitOwnerScope.Character,
				Expression = new TraitExpression
				{
					Name = $"{BlindFightingSkillName} Skill Cap",
					Expression = brawling.Expression.Expression
				},
				TraitGroup = "Combat",
				AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
				LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachDifficulty = 7,
				LearnDifficulty = 7,
				Hidden = false,
				ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
				DecoratorId = context.TraitDecorators.First(x => x.Name == "General Skill").Id,
				DerivedType = 0,
				ChargenBlurb = string.Empty,
				BranchMultiplier = 1.0
			};
			context.TraitDefinitions.Add(skill);
			context.SaveChanges();
		}

		context.Checks.Add(new Check
		{
			Type = (int)CheckType.BlindFightingCheck,
			CheckTemplate = context.CheckTemplates.First(x => x.Name == "Skill Check"),
			MaximumDifficultyForImprovement = (int)Difficulty.Impossible,
			TraitExpression = new TraitExpression
			{
				Name = "Blind Fighting Check Formula",
				Expression = $"blindfighting:{skill.Id}"
			}
		});
		context.SaveChanges();
		return true;
	}
}
