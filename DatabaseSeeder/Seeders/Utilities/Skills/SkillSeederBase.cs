using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public abstract class SkillSeederBase : IDatabaseSeeder
{
    private static readonly (Difficulty Difficulty, int Modifier)[] StandardSkillModifiers =
    [
        (Difficulty.Automatic, 100),
        (Difficulty.Trivial, 75),
        (Difficulty.ExtremelyEasy, 50),
        (Difficulty.VeryEasy, 33),
        (Difficulty.Easy, 15),
        (Difficulty.Normal, 0),
        (Difficulty.Hard, -10),
        (Difficulty.VeryHard, -20),
        (Difficulty.ExtremelyHard, -33),
        (Difficulty.Insane, -50),
        (Difficulty.Impossible, -100)
    ];

    private static readonly (Difficulty Difficulty, int Modifier)[] LanguageModifiers =
    [
        (Difficulty.Automatic, 100),
        (Difficulty.Trivial, 55),
        (Difficulty.ExtremelyEasy, 35),
        (Difficulty.VeryEasy, 22),
        (Difficulty.Easy, 11),
        (Difficulty.Normal, 0),
        (Difficulty.Hard, -10),
        (Difficulty.VeryHard, -20),
        (Difficulty.ExtremelyHard, -40),
        (Difficulty.Insane, -60),
        (Difficulty.Impossible, -100)
    ];

    private static readonly (Difficulty Difficulty, int Modifier)[] PerceptionModifiers =
    [
        (Difficulty.Automatic, 150),
        (Difficulty.Trivial, 100),
        (Difficulty.ExtremelyEasy, 75),
        (Difficulty.VeryEasy, 50),
        (Difficulty.Easy, 30),
        (Difficulty.Normal, 10),
        (Difficulty.Hard, -10),
        (Difficulty.VeryHard, -20),
        (Difficulty.ExtremelyHard, -30),
        (Difficulty.Insane, -50),
        (Difficulty.Impossible, -100)
    ];

    private static readonly (Difficulty Difficulty, int Modifier)[] CapabilityModifiers =
    [
        (Difficulty.Automatic, 100),
        (Difficulty.Trivial, 55),
        (Difficulty.ExtremelyEasy, 35),
        (Difficulty.VeryEasy, 22),
        (Difficulty.Easy, 11),
        (Difficulty.Normal, 0),
        (Difficulty.Hard, -10),
        (Difficulty.VeryHard, -20),
        (Difficulty.ExtremelyHard, -40),
        (Difficulty.Insane, -60),
        (Difficulty.Impossible, -100)
    ];

    private static readonly (Difficulty Difficulty, int Modifier)[] HealthModifiers =
    [
        (Difficulty.Automatic, 10),
        (Difficulty.Trivial, 8),
        (Difficulty.ExtremelyEasy, 6),
        (Difficulty.VeryEasy, 4),
        (Difficulty.Easy, 2),
        (Difficulty.Normal, 0),
        (Difficulty.Hard, -1),
        (Difficulty.VeryHard, -2),
        (Difficulty.ExtremelyHard, -3),
        (Difficulty.Insane, -4),
        (Difficulty.Impossible, -5)
    ];

    private static readonly (Difficulty Difficulty, int Modifier)[] ZeroModifiers =
    [
        (Difficulty.Automatic, 0),
        (Difficulty.Trivial, 0),
        (Difficulty.ExtremelyEasy, 0),
        (Difficulty.VeryEasy, 0),
        (Difficulty.Easy, 0),
        (Difficulty.Normal, 0),
        (Difficulty.Hard, 0),
        (Difficulty.VeryHard, 0),
        (Difficulty.ExtremelyHard, 0),
        (Difficulty.Insane, 0),
        (Difficulty.Impossible, 0)
    ];

    protected static IReadOnlyDictionary<string, CheckTemplate> SeedCheckTemplates(
        FuturemudDatabaseContext context,
        bool branching)
    {
        Dictionary<string, CheckTemplate> checkTemplates = new(StringComparer.OrdinalIgnoreCase);

        void AddTemplate(
            string name,
            string method,
            bool improveTraits,
            short failIfTraitMissingMode,
            bool canBranchIfTraitMissing,
            string? definition,
            IEnumerable<(Difficulty Difficulty, int Modifier)> modifiers)
        {
            CheckTemplate template = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.CheckTemplates,
                name,
                x => x.Name,
                () =>
                {
                    CheckTemplate created = new();
                    context.CheckTemplates.Add(created);
                    return created;
                });

            template.Name = name;
            template.CheckMethod = method;
            template.ImproveTraits = improveTraits;
            template.FailIfTraitMissingMode = failIfTraitMissingMode;
            template.CanBranchIfTraitMissing = canBranchIfTraitMissing;
            template.Definition = definition;

            foreach (CheckTemplateDifficulty? existing in template.CheckTemplateDifficulties.ToList())
            {
                context.CheckTemplateDifficulties.Remove(existing);
            }

            foreach ((Difficulty difficulty, int modifier) in modifiers)
            {
                template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
                {
                    CheckTemplate = template,
                    Difficulty = (int)difficulty,
                    Modifier = modifier
                });
            }

            checkTemplates[name] = template;
        }

        AddTemplate("Skill Check", "Standard", true, 0, branching, null, StandardSkillModifiers);
        AddTemplate("Skill Check Fail If Missing", "Standard", true, 2, branching, null, StandardSkillModifiers);
        AddTemplate("Skill Check No Improvement", "Standard", false, 0, false, null, StandardSkillModifiers);
        AddTemplate("Language Check", "Standard", true, 2, branching, null, LanguageModifiers);
        AddTemplate("Perception Check", "Standard", true, 0, branching, null, PerceptionModifiers);
        AddTemplate("Branch Check", "Branch", false, 0, false, null, ZeroModifiers);
        AddTemplate(
            "Passive Perception Check",
            "PassivePerception",
            true,
            2,
            branching,
            @"<Definition minimum_time=""300"" maximum_time=""600"">
 	<CoreTraitExpression>50</CoreTraitExpression>
    <!-- Below is an example of how you might change this up once you have skills in, by adding in spot as a skill -->
    <!-- <CoreTraitExpression>spot:17</CoreTraitExpression> -->
 	<PassiveFuzziness>0.05</PassiveFuzziness>
 </Definition>",
            PerceptionModifiers);
        AddTemplate("Capability Check", "Static", false, 0, false, null, CapabilityModifiers);
        AddTemplate("Project Check", "Standard", true, 0, branching, null, CapabilityModifiers);
        AddTemplate("Bonus Absent Check", "BonusAbsent", true, 0, branching, null, CapabilityModifiers);
        AddTemplate("Health Check", "Standard", false, 0, false, null, HealthModifiers);
        AddTemplate("Static Check", "Static", false, 1, false, null, ZeroModifiers);

        context.SaveChanges();
        return checkTemplates;
    }

    protected static (
        TraitDecorator general,
        TraitDecorator crafting,
        TraitDecorator languageDecorator,
        TraitDecorator veterancy,
        TraitDecorator professional,
        Improver languageImprover,
        Improver generalImprover) SeedSkillImprovers(
            FuturemudDatabaseContext context,
            string skillGainModel)
    {
        TraitDecorator general = EnsureDecorator(
            context,
            "General Skill",
            "Range",
            "<ranges name=\"General Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"15\" text=\"Familiar\"/><range low=\"15\" high=\"30\" text=\"Competent\"/><range low=\"30\" high=\"45\" text=\"Skilled\"/><range low=\"45\" high=\"60\" text=\"Expert\"/><range low=\"60\" high=\"75\" text=\"Masterful\"/><range low=\"75\" high=\"85\" text=\"Epic\"/><range low=\"85\" high=\"95\" text=\"Legendary\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>");
        context.SaveChanges();

		context.StaticConfigurations.Find("DefaultSkillDecorator")!.Definition = general.Id.ToString();
        context.SaveChanges();

        TraitDecorator crafting = EnsureDecorator(
            context,
            "Crafting Skill",
            "Range",
            "<ranges name=\"Craft Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"15\" text=\"Novice\"/><range low=\"15\" high=\"30\" text=\"Apprentice\"/><range low=\"30\" high=\"45\" text=\"Journeyman\"/><range low=\"45\" high=\"60\" text=\"Adept\"/><range low=\"60\" high=\"75\" text=\"Master\"/><range low=\"75\" high=\"85\" text=\"Grand Master\"/><range low=\"85\" high=\"95\" text=\"Legendary Master\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>");
        TraitDecorator professional = EnsureDecorator(
            context,
            "Professional Skill",
            "Range",
            "<ranges name=\"Professional Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"15\" text=\"Amateur\"/><range low=\"15\" high=\"30\" text=\"Competent\"/><range low=\"30\" high=\"45\" text=\"Professional\"/><range low=\"45\" high=\"60\" text=\"Specialist\"/><range low=\"60\" high=\"75\" text=\"Expert\"/><range low=\"75\" high=\"85\" text=\"Renowned Expert\"/><range low=\"85\" high=\"95\" text=\"Virtuoso\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>");
        TraitDecorator languageDecorator = EnsureDecorator(
            context,
            "Language Skill",
            "Range",
            "<ranges name=\"Language Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Incompetent\"/><range low=\"0\" high=\"30\" text=\"Amateur\"/><range low=\"30\" high=\"45\" text=\"Coherent\"/><range low=\"45\" high=\"70\" text=\"Conversant\"/><range low=\"70\" high=\"95\" text=\"Fluent\"/><range low=\"95\" high=\"120\" text=\"Articulate\"/><range low=\"120\" high=\"500\" text=\"Masterful\"/></ranges>");
        TraitDecorator veterancy = EnsureDecorator(
            context,
            "Veterancy Skill",
            "Range",
            "<ranges name=\"Veterancy Skill Range\"  prefix=\"(\" suffix=\")\" colour_capped=\"true\" colour_buffed=\"true\"><range low=\"-100\" high=\"0\" text=\"Civilian\"/><range low=\"0\" high=\"15\" text=\"Greenhorn\"/><range low=\"15\" high=\"30\" text=\"Recruit\"/><range low=\"30\" high=\"45\" text=\"Blooded\"/><range low=\"45\" high=\"60\" text=\"Experienced\"/><range low=\"60\" high=\"75\" text=\"Veteran\"/><range low=\"75\" high=\"85\" text=\"Grizzled\"/><range low=\"85\" high=\"95\" text=\"Legendary\"/><range low=\"95\" high=\"500\" text=\"Godly\"/></ranges>");
        context.SaveChanges();

        Improver languageImprover;
        Improver generalImprover;
        switch (skillGainModel)
        {
            case "rpi":
                languageImprover = EnsureImprover(
                    context,
                    "Language Improver",
                    "classic",
                    "<Definition Chance=\"0.025\" Expression=\"1\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"200\" NoGainSecondsDiceExpression=\"1d500+4000\"/>");
                generalImprover = EnsureImprover(
                    context,
                    "Skill Improver",
                    "classic",
                    "<Definition Chance=\"0.1\" Expression=\"1\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"100\" NoGainSecondsDiceExpression=\"1d500+4000\"/>");
                break;
            case "labmud":
                languageImprover = EnsureImprover(
                    context,
                    "Language Improver",
                    "classic",
                    "<Definition Chance=\"0.025\" Expression=\"max(1,5-(variable/30))\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"50\" NoGainSecondsDiceExpression=\"1d500+4000\"/>");
                generalImprover = EnsureImprover(
                    context,
                    "Skill Improver",
                    "classic",
                    "<Definition Chance=\"0.1\" Expression=\"max(1,5-(variable/10))\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"10\" NoGainSecondsDiceExpression=\"1d500+4000\"/>");
                break;
            case "armageddon":
                languageImprover = EnsureImprover(
                    context,
                    "Language Improver",
                    "classic",
                    "<Definition Chance=\"0.025\" Expression=\"1\" ImproveOnFail=\"true\" ImproveOnSuccess=\"false\" DifficultyThresholdInterval=\"200\" NoGainSecondsDiceExpression=\"1d500+4000\"/>");
                generalImprover = EnsureImprover(
                    context,
                    "Skill Improver",
                    "branching",
                    @"<Definition Chance=""0.05"" Expression=""max(1,5-(variable/12))"" ImproveOnFail=""true"" ImproveOnSuccess=""false"" DifficultyThresholdInterval=""1000"" NoGainSecondsDiceExpression=""1d500+4000"">
 	<Branches>
        <!-- base = id of skill triggering branch, branch = id of skill to branch, on = level of base skill required, at = opening value of branched skill --> 
 		<Branch base=""0"" branch=""0"" on=""70"" at=""10""/>
 	</Branches>
 </Definition>");
                break;
            case "successtree":
                languageImprover = EnsureImprover(
                    context,
                    "Language Improver",
                    "classic",
                    "<Definition Chance=\"0.025\" Expression=\"max(1,5-(variable/30))\" ImproveOnFail=\"false\" ImproveOnSuccess=\"true\" DifficultyThresholdInterval=\"50\" NoGainSecondsDiceExpression=\"1d500+4000\"/>");
                generalImprover = EnsureImprover(
                    context,
                    "Skill Improver",
                    "branching",
                    @"<Definition Chance=""0.05"" Expression=""max(1,5-(variable/12))"" ImproveOnFail=""false"" ImproveOnSuccess=""true"" DifficultyThresholdInterval=""10"" NoGainSecondsDiceExpression=""1d500+4000"">
 	<Branches>
        <!-- base = id of skill triggering branch, branch = id of skill to branch, on = level of base skill required, at = opening value of branched skill --> 
 		<Branch base=""0"" branch=""0"" on=""70"" at=""10""/>
 	</Branches>
 </Definition>");
                break;
            default:
                goto case "labmud";
        }

        context.SaveChanges();
		context.StaticConfigurations.Find("DefaultSkillImprover")!.Definition = generalImprover.Id.ToString();
        context.SaveChanges();
        return (general, crafting, languageDecorator, veterancy, professional, languageImprover, generalImprover);
    }

    protected static TraitExpression EnsureTraitExpression(
        FuturemudDatabaseContext context,
        string name,
        string expression)
    {
        TraitExpression traitExpression = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.TraitExpressions,
            name,
            x => x.Name,
            () =>
            {
                TraitExpression created = new();
                context.TraitExpressions.Add(created);
                return created;
            });
        traitExpression.Name = name;
        traitExpression.Expression = expression;
        return traitExpression;
    }

    protected static Check EnsureCheck(
        FuturemudDatabaseContext context,
        CheckType type,
        string expressionName,
        string expression,
        long templateId,
        Difficulty maximumImprovementDifficulty)
    {
        TraitExpression traitExpression = EnsureTraitExpression(context, expressionName, expression);
        context.SaveChanges();

        Check? check = context.Checks.FirstOrDefault(x => x.Type == (int)type);
        if (check is null)
        {
            check = new Check { Type = (int)type };
            context.Checks.Add(check);
        }

        check.CheckTemplateId = templateId;
        check.MaximumDifficultyForImprovement = (int)maximumImprovementDifficulty;
        check.TraitExpression = traitExpression;
        check.TraitExpressionId = traitExpression.Id;
        return check;
    }

    protected static TraitDefinition EnsureSkillDefinition(
        FuturemudDatabaseContext context,
        string name,
        Action<TraitDefinition> update)
    {
        TraitDefinition trait = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.TraitDefinitions,
            name,
            x => x.Name,
            () =>
            {
                TraitDefinition created = new();
                context.TraitDefinitions.Add(created);
                return created;
            });
        trait.Name = name;
        update(trait);
        trait.OwnerScope = (int)TraitOwnerScope.Character;
        return trait;
    }

    protected static Language EnsureLanguage(
        FuturemudDatabaseContext context,
        string name,
        Action<Language> update)
    {
        Language language = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.Languages,
            name,
            x => x.Name,
            () =>
            {
                Language created = new();
                context.Languages.Add(created);
                return created;
            });
        language.Name = name;
        update(language);
        return language;
    }

    protected static Accent EnsureAccent(
        FuturemudDatabaseContext context,
        Language language,
        string name,
        Action<Accent> update)
    {
        Accent accent = SeederRepeatabilityHelper.EnsureEntity(
            context.Accents,
            x => x.LanguageId == language.Id &&
                 string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase),
            x => x.LanguageId == language.Id,
            () =>
            {
                Accent created = new();
                context.Accents.Add(created);
                return created;
            });
        accent.Name = name;
        accent.Language = language;
        accent.LanguageId = language.Id;
        update(accent);
        return accent;
    }

    protected static Helpfile EnsureHelpfile(
        FuturemudDatabaseContext context,
        string name,
        Action<Helpfile> update)
    {
        Helpfile helpfile = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.Helpfiles,
            name,
            x => x.Name,
            () =>
            {
                Helpfile created = new();
                context.Helpfiles.Add(created);
                return created;
            });
        helpfile.Name = name;
        update(helpfile);
        return helpfile;
    }

    private static TraitDecorator EnsureDecorator(
        FuturemudDatabaseContext context,
        string name,
        string type,
        string contents)
    {
        TraitDecorator decorator = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.TraitDecorators,
            name,
            x => x.Name,
            () =>
            {
                TraitDecorator created = new();
                context.TraitDecorators.Add(created);
                return created;
            });
        decorator.Name = name;
        decorator.Type = type;
        decorator.Contents = contents;
        return decorator;
    }

    private static Improver EnsureImprover(
        FuturemudDatabaseContext context,
        string name,
        string type,
        string definition)
    {
        Improver improver = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.Improvers,
            name,
            x => x.Name,
            () =>
            {
                Improver created = new();
                context.Improvers.Add(created);
                return created;
            });
        improver.Name = name;
        improver.Type = type;
        improver.Definition = definition;
        return improver;
    }

    #region Implementation of IDatabaseSeeder

    /// <inheritdoc />
    public abstract
        IEnumerable<(string Id, string Question,
            Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
            Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
    { get; }

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
