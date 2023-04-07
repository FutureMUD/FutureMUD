using System;
using System.Collections.Generic;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public abstract class SkillSeederBase : IDatabaseSeeder
{
	protected static IReadOnlyDictionary<string, CheckTemplate> SeedCheckTemplates(FuturemudDatabaseContext context,
		bool branching)
	{
		var checkTemplates = new Dictionary<string, CheckTemplate>(StringComparer.OrdinalIgnoreCase);
		var template = new CheckTemplate
		{
			Id = 1,
			Name = "Skill Check",
			CheckMethod = "Standard",
			ImproveTraits = true,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = branching
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 75 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 33 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 15 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -33 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 2,
			Name = "Skill Check Fail If Missing",
			CheckMethod = "Standard",
			ImproveTraits = true,
			FailIfTraitMissingMode = 2,
			CanBranchIfTraitMissing = branching
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 75 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 33 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 15 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -33 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 3,
			Name = "Skill Check No Improvement",
			CheckMethod = "Standard",
			ImproveTraits = false,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = false
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 75 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 33 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 15 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -33 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 4,
			Name = "Language Check",
			CheckMethod = "Standard",
			ImproveTraits = true,
			FailIfTraitMissingMode = 2,
			CanBranchIfTraitMissing = branching
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 55 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 35 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 22 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 11 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -40 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -60 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 5,
			Name = "Perception Check",
			CheckMethod = "Standard",
			ImproveTraits = true,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = branching
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 150 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 75 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 30 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -30 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 6,
			Name = "Branch Check",
			CheckMethod = "Branch",
			ImproveTraits = false,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = false
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = 0 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 7,
			Name = "Passive Perception Check",
			CheckMethod = "PassivePerception",
			ImproveTraits = true,
			FailIfTraitMissingMode = 2,
			CanBranchIfTraitMissing = branching,
			Definition = @"<Definition minimum_time=""300"" maximum_time=""600"">
 	<CoreTraitExpression>50</CoreTraitExpression>
    <!-- Below is an example of how you might change this up once you have skills in, by adding in spot as a skill -->
    <!-- <CoreTraitExpression>spot:17</CoreTraitExpression> -->
 	<PassiveFuzziness>0.05</PassiveFuzziness>
 </Definition>"
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 150 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 75 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 30 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -30 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -50 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 8,
			Name = "Capability Check",
			CheckMethod = "Static",
			ImproveTraits = false,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = false
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 55 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 35 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 22 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 11 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -40 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -60 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 9,
			Name = "Project Check",
			CheckMethod = "Standard",
			ImproveTraits = true,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = branching
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 55 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 35 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 22 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 11 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -40 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -60 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 10,
			Name = "Bonus Absent Check",
			CheckMethod = "BonusAbsent",
			ImproveTraits = true,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = branching
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 100 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 55 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 35 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 22 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 11 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -20 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -40 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -60 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -100 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 11,
			Name = "Health Check",
			CheckMethod = "Standard",
			ImproveTraits = false,
			FailIfTraitMissingMode = 0,
			CanBranchIfTraitMissing = false
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 10 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 8 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 6 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 4 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 2 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = -1 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = -2 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = -3 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = -4 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = -5 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		template = new CheckTemplate
		{
			Id = 12,
			Name = "Static Check",
			CheckMethod = "Static",
			ImproveTraits = false,
			FailIfTraitMissingMode = 1,
			CanBranchIfTraitMissing = false
		};
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Automatic, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Trivial, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyEasy, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryEasy, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Easy, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Normal, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Hard, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.VeryHard, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.ExtremelyHard, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Insane, Modifier = 0 });
		template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{ CheckTemplate = template, Difficulty = (int)Difficulty.Impossible, Modifier = 0 });
		context.CheckTemplates.Add(template);
		checkTemplates[template.Name] = template;

		context.SaveChanges();
		return checkTemplates;
	}

	protected static (TraitDecorator general, TraitDecorator crafting, TraitDecorator languageDecorator, TraitDecorator
		veterancy, TraitDecorator professional, Improver languageImprover, Improver generalImprover) SeedSkillImprovers(
			FuturemudDatabaseContext context,
			string skillGainModel)
	{
		var general = new TraitDecorator
		{
			Name = "General Skill",
			Type = "Range",
			Contents =
				"<ranges name=\"General Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"15\" text=\"Familiar\"/><range low=\"15\" high=\"30\" text=\"Competent\"/><range low=\"30\" high=\"45\" text=\"Skilled\"/><range low=\"45\" high=\"60\" text=\"Expert\"/><range low=\"60\" high=\"75\" text=\"Masterful\"/><range low=\"75\" high=\"85\" text=\"Epic\"/><range low=\"85\" high=\"95\" text=\"Legendary\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>"
		};
		context.TraitDecorators.Add(general);
		context.SaveChanges();

		context.StaticConfigurations.Find("DefaultSkillDecorator").Definition = general.Id.ToString();
		context.SaveChanges();

		var crafting = new TraitDecorator
		{
			Name = "Crafting Skill",
			Type = "Range",
			Contents =
				"<ranges name=\"Craft Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"15\" text=\"Novice\"/><range low=\"15\" high=\"30\" text=\"Apprentice\"/><range low=\"30\" high=\"45\" text=\"Journeyman\"/><range low=\"45\" high=\"60\" text=\"Adept\"/><range low=\"60\" high=\"75\" text=\"Master\"/><range low=\"75\" high=\"85\" text=\"Grand Master\"/><range low=\"85\" high=\"95\" text=\"Legendary Master\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>"
		};
		context.TraitDecorators.Add(crafting);
		context.SaveChanges();

		var professional = new TraitDecorator
		{
			Name = "Professional Skill",
			Type = "Range",
			Contents =
				"<ranges name=\"Professional Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"15\" text=\"Amateur\"/><range low=\"15\" high=\"30\" text=\"Competent\"/><range low=\"30\" high=\"45\" text=\"Professional\"/><range low=\"45\" high=\"60\" text=\"Specialist\"/><range low=\"60\" high=\"75\" text=\"Expert\"/><range low=\"75\" high=\"85\" text=\"Renowned Expert\"/><range low=\"85\" high=\"95\" text=\"Virtuoso\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>"
		};
		context.TraitDecorators.Add(professional);
		context.SaveChanges();

		var languageDecorator = new TraitDecorator
		{
			Name = "Language Skill",
			Type = "Range",
			Contents =
				"<ranges name=\"Language Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"30\" text=\"Amateur\"/><range low=\"30\" high=\"45\" text=\"Coherent\"/><range low=\"45\" high=\"70\" text=\"Conversant\"/><range low=\"70\" high=\"95\" text=\"Fluent\"/><range low=\"95\" high=\"120\" text=\"Articulate\"/><range low=\"120\" high=\"500\" text=\"Masterful\"/></ranges>"
		};
		context.TraitDecorators.Add(languageDecorator);
		context.SaveChanges();

		var veterancy = new TraitDecorator
		{
			Name = "Veterancy Skill",
			Type = "Range",
			Contents =
				"<ranges name=\"Veterancy Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Civilian\"/><range low=\"0\" high=\"15\" text=\"Greenhorn\"/><range low=\"15\" high=\"30\" text=\"Recruit\"/><range low=\"30\" high=\"45\" text=\"Blooded\"/><range low=\"45\" high=\"60\" text=\"Experienced\"/><range low=\"60\" high=\"75\" text=\"Veteran\"/><range low=\"75\" high=\"85\" text=\"Grizzled\"/><range low=\"85\" high=\"95\" text=\"Legendary\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>"
		};
		context.TraitDecorators.Add(veterancy);
		context.SaveChanges();

		Improver languageImprover, generalImprover;
		switch (skillGainModel)
		{
			case "rpi":
				languageImprover = new Improver
				{
					Name = "Language Improver",
					Type = "classic",
					Definition =
						"<Definition Chance=\"0.025\" Expression=\"1\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"200\" NoGainSecondsDiceExpression=\"1d500+4000\"/>"
				};
				generalImprover = new Improver
				{
					Name = "Skill Improver",
					Type = "classic",
					Definition =
						"<Definition Chance=\"0.1\" Expression=\"1\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"100\" NoGainSecondsDiceExpression=\"1d500+4000\"/>"
				};
				break;
			case "labmud":
				languageImprover = new Improver
				{
					Name = "Language Improver",
					Type = "classic",
					Definition =
						"<Definition Chance=\"0.025\" Expression=\"max(1,5-(variable/30))\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"50\" NoGainSecondsDiceExpression=\"1d500+4000\"/>"
				};
				generalImprover = new Improver
				{
					Name = "Skill Improver",
					Type = "classic",
					Definition =
						"<Definition Chance=\"0.1\" Expression=\"max(1,5-(variable/10))\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"10\" NoGainSecondsDiceExpression=\"1d500+4000\"/>"
				};
				break;
			case "armageddon":
				languageImprover = new Improver
				{
					Name = "Language Improver",
					Type = "classic",
					Definition =
						"<Definition Chance=\"0.025\" Expression=\"1\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"200\" NoGainSecondsDiceExpression=\"1d500+4000\"/>"
				};
				generalImprover = new Improver
				{
					Name = "Skill Improver",
					Type = "branching",
					Definition =
						@"<Definition Chance=""0.05"" Expression=""max(1,5-(variable/12))"" ImproveOnFail=""true"" ImproveOnSuccess=""false"" DifficultyThresholdInterval=""1000"" NoGainSecondsDiceExpression=""1d500+4000"">
 	<Branches>
        <!-- base = id of skill triggering branch, branch = id of skill to branch, on = level of base skill required, at = opening value of branched skill --> 
 		<Branch base=""0"" branch=""0"" on=""70"" at=""10""/>
 	</Branches>
 </Definition>"
				};
				break;
			case "successtree":
				languageImprover = new Improver
				{
					Name = "Language Improver",
					Type = "classic",
					Definition =
						"<Definition Chance=\"0.025\" Expression=\"max(1,5-(variable/30))\" ImproveOnFail=\"false\" ImproveOnSuccess=\"true\" DifficultyThresholdInterval=\"50\" NoGainSecondsDiceExpression=\"1d500+4000\"/>"
				};
				generalImprover = new Improver
				{
					Name = "Skill Improver",
					Type = "branching",
					Definition =
						@"<Definition Chance=""0.05"" Expression=""max(1,5-(variable/12))"" ImproveOnFail=""false"" ImproveOnSuccess=""true"" DifficultyThresholdInterval=""10"" NoGainSecondsDiceExpression=""1d500+4000"">
 	<Branches>
        <!-- base = id of skill triggering branch, branch = id of skill to branch, on = level of base skill required, at = opening value of branched skill --> 
 		<Branch base=""0"" branch=""0"" on=""70"" at=""10""/>
 	</Branches>
 </Definition>"
				};
				break;
			default:
				goto case "labmud";
		}

		context.Improvers.Add(languageImprover);
		context.Improvers.Add(generalImprover);
		context.SaveChanges();
		context.StaticConfigurations.Find("DefaultSkillImprover").Definition = generalImprover.Id.ToString();
		context.SaveChanges();
		return (general, crafting, languageDecorator, veterancy, professional, languageImprover, generalImprover);
	}

	#region Implementation of IDatabaseSeeder

	/// <inheritdoc />
	public abstract
		IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions { get; }

	/// <inheritdoc />
	public abstract string SeedData(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers);

	/// <inheritdoc />
	public abstract ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context);

	/// <inheritdoc />
	public abstract int SortOrder { get; }

	/// <inheritdoc />
	public abstract string Name { get; }

	/// <inheritdoc />
	public abstract string Tagline { get; }

	/// <inheritdoc />
	public abstract string FullDescription { get; }

	#endregion
}