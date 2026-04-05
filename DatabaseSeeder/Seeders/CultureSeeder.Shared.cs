using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
    private static readonly string[] StockNameCultureMarkers =
    [
        "Simple",
        "Given and Family",
        "Given and Patronym",
        "Given and Toponym"
    ];

    private static readonly string[] StockRandomProfileMarkers =
    [
        "Wild Animal (Male)",
        "Wild Animal (Female)",
        "Wild Animal (Neuter)",
        "Dog (Male)",
        "Dog (Female)",
        "Elephant (Male)",
        "Elephant (Female)"
    ];

    private static readonly string[] StockCulturePackageMarkers =
    [
        "English",
        "Latin",
        "Westron",
        "Germanic",
        "Roman",
        "Gondorian"
    ];

    private static readonly string[] ChargenSizeProgMarkers =
    [
        "MaximumHeightChargen",
        "MinimumHeightChargen",
        "MaximumWeightChargen",
        "MinimumWeightChargen"
    ];

    private NameCulture EnsureNameCulture(string name, string definition)
    {
        NameCulture culture = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.NameCultures,
            name,
            x => x.Name,
            () =>
            {
                NameCulture created = new();
                _context.NameCultures.Add(created);
                return created;
            });

        culture.Name = name;
        culture.Definition = definition;
        _context.SaveChanges();
        _nameCultures[name] = culture;
        return culture;
    }

    private RandomNameProfile EnsureRandomNameProfile(string name, Gender gender, NameCulture culture)
    {
        RandomNameProfile profile = SeederRepeatabilityHelper.EnsureEntity(
            _context.RandomNameProfiles,
            x => x.Gender == (int)gender && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase),
            x => x.Gender == (int)gender,
            () =>
            {
                RandomNameProfile created = new();
                _context.RandomNameProfiles.Add(created);
                return created;
            });

        profile.Name = name;
        profile.Gender = (int)gender;
        profile.NameCulture = culture;

        foreach (RandomNameProfilesDiceExpressions? dice in profile.RandomNameProfilesDiceExpressions.ToList())
        {
            _context.RandomNameProfilesDiceExpressions.Remove(dice);
        }

        foreach (RandomNameProfilesElements? element in profile.RandomNameProfilesElements.ToList())
        {
            _context.RandomNameProfilesElements.Remove(element);
        }

        _addedNames = new();
        _context.SaveChanges();
        return profile;
    }

    private TraitExpression EnsureTraitExpression(string name, string expressionText)
    {
        TraitExpression expression = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.TraitExpressions,
            name,
            x => x.Name,
            () =>
            {
                TraitExpression created = new();
                _context.TraitExpressions.Add(created);
                return created;
            });

        expression.Name = name;
        expression.Expression = expressionText;
        return expression;
    }

    private TraitDefinition EnsureLanguageTrait(string name, FutureProg? canSelectProg)
    {
        TraitDecorator decorator = _context.TraitDecorators.First(x => x.Name == "Language Skill");
        Improver improver = _context.Improvers.First(x => x.Name == "Language Improver");
        string capFormula = _context.Languages.FirstOrDefault()?.LinkedTrait.Expression.Expression ??
                         $"10+(9.5 * {_intelligenceTrait.Alias}:{_intelligenceTrait.Id})";
        TraitExpression expression = EnsureTraitExpression($"{name} Skill Cap", capFormula);

        TraitDefinition trait = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.TraitDefinitions,
            name,
            x => x.Name,
            () =>
            {
                TraitDefinition created = new();
                _context.TraitDefinitions.Add(created);
                return created;
            });

        trait.Name = name;
        trait.Type = 0;
        trait.DecoratorId = decorator.Id;
        trait.TraitGroup = "Language";
        trait.AvailabilityProg = canSelectProg ?? _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
        trait.TeachableProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
        trait.LearnableProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
        trait.TeachDifficulty = 7;
        trait.LearnDifficulty = 7;
        trait.Hidden = false;
        trait.Expression = expression;
        trait.ImproverId = improver.Id;
        trait.DerivedType = 0;
        trait.ChargenBlurb = string.Empty;
        trait.BranchMultiplier = 0.1;
        return trait;
    }

    private Language EnsureLanguage(
        string name,
        string unknownDescription,
        FutureProg? canSelectProg = null)
    {
        TraitDefinition trait = EnsureLanguageTrait(name, canSelectProg);
        Language language = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Languages,
            name,
            x => x.Name,
            () =>
            {
                Language created = new();
                _context.Languages.Add(created);
                return created;
            });

        language.Name = name;
        language.UnknownLanguageDescription = unknownDescription;
        language.LanguageObfuscationFactor = 0.1;
        language.DifficultyModel = 1;
        language.LinkedTrait = trait;

        _context.SaveChanges();
        _languages[name] = language;
        return language;
    }

    private Accent EnsureAccent(
        Language language,
        string name,
        string suffix,
        string vague,
        int difficulty,
        string description,
        string group,
        FutureProg? prog = null)
    {
        Accent accent = SeederRepeatabilityHelper.EnsureEntity(
            _context.Accents,
            x => x.LanguageId == language.Id && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase),
            x => x.LanguageId == language.Id,
            () =>
            {
                Accent created = new();
                _context.Accents.Add(created);
                return created;
            });

        accent.Name = name.TitleCase();
        accent.Suffix = suffix;
        accent.VagueSuffix = vague;
        accent.Difficulty = difficulty;
        accent.Description = description;
        accent.Group = group;
        accent.Language = language;
        accent.ChargenAvailabilityProgId = prog?.Id;
        _context.SaveChanges();
        return accent;
    }

    private void EnsureMutualIntelligability(string from, string to, Difficulty difficulty, bool twoWay = false)
    {
        Language source = _languages[from];
        Language target = _languages[to];

        void Upsert(Language listener, Language heard)
        {
            MutualIntelligability mutual = SeederRepeatabilityHelper.EnsureEntity(
                _context.MutualIntelligabilities,
                x => x.ListenerLanguageId == listener.Id && x.TargetLanguageId == heard.Id,
                () =>
                {
                    MutualIntelligability created = new();
                    _context.MutualIntelligabilities.Add(created);
                    return created;
                });

            mutual.ListenerLanguage = listener;
            mutual.TargetLanguage = heard;
            mutual.IntelligabilityDifficulty = (int)difficulty;
        }

        Upsert(target, source);
        if (twoWay)
        {
            Upsert(source, target);
        }

        _context.SaveChanges();
    }

    private Script EnsureScript(
        string name,
        string known,
        string unknown,
        string description,
        string subtype,
        double length,
        double ink,
        FutureProg canAcquireProg,
        params string[] languages)
    {
        Knowledge knowledge = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Knowledges,
            $"{name} Script",
            x => x.Name,
            () =>
            {
                Knowledge created = new();
                _context.Knowledges.Add(created);
                return created;
            });

        knowledge.Name = $"{name} Script";
        knowledge.Description = $"Knowledge of the use of the {name} Script";
        knowledge.LongDescription = description;
        knowledge.Type = "Script";
        knowledge.Subtype = subtype;
        knowledge.LearnableType = (int)(LearnableType.LearnableAtChargen | LearnableType.LearnableFromTeacher);
        knowledge.LearnDifficulty = (int)Difficulty.VeryHard;
        knowledge.TeachDifficulty = (int)Difficulty.VeryHard;
        knowledge.LearningSessionsRequired = 10;
        knowledge.CanAcquireProg = canAcquireProg;
        knowledge.CanLearnProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");

        Script script = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Scripts,
            name,
            x => x.Name,
            () =>
            {
                Script created = new();
                _context.Scripts.Add(created);
                return created;
            });

        script.Name = name;
        script.DocumentLengthModifier = length;
        script.InkUseModifier = ink;
        script.KnownScriptDescription = known;
        script.UnknownScriptDescription = unknown;
        script.Knowledge = knowledge;

        foreach (ScriptsDesignedLanguage? existing in script.ScriptsDesignedLanguages.ToList())
        {
            _context.ScriptsDesignedLanguages.Remove(existing);
        }

        foreach (string languageName in languages)
        {
            script.ScriptsDesignedLanguages.Add(new ScriptsDesignedLanguage
            {
                Script = script,
                Language = _languages[languageName]
            });
        }

        _context.SaveChanges();
        return script;
    }

    private Ethnicity EnsureEthnicity(
        Race race,
        string name,
        string group,
        string bloodGroup,
        double tempFloor,
        double tempCeiling,
        string subgroup,
        FutureProg? available,
        string description)
    {
        Ethnicity ethnicity = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Ethnicities,
            name,
            x => x.Name,
            () =>
            {
                Ethnicity created = new();
                _context.Ethnicities.Add(created);
                return created;
            });

        ethnicity.Name = name;
        ethnicity.ParentRace = race;
        ethnicity.ChargenBlurb = description;
        ethnicity.EthnicGroup = group;
        ethnicity.EthnicSubgroup = subgroup;
        ethnicity.PopulationBloodModel = _bloodModels[bloodGroup];
        ethnicity.TolerableTemperatureFloorEffect = tempFloor;
        ethnicity.TolerableTemperatureCeilingEffect = tempCeiling;
        ethnicity.AvailabilityProg = available ?? _alwaysTrueProg;

        _context.SaveChanges();
        _ethnicities[name] = ethnicity;
        _ethnicProgs[name] = SeederRepeatabilityHelper.EnsureProg(
            _context,
            $"IsEthnicity{name.CollapseString()}",
            "Character",
            "Ethnicity",
            ProgVariableTypes.Boolean,
            $"Determines whether someone is the {name} ethnicity",
            $"return @ch.Ethnicity == ToEthnicity({ethnicity.Id})",
            true,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Toon, "ch"));
        _context.SaveChanges();
        return ethnicity;
    }

    private Culture EnsureCulture(
        string name,
        string description,
        FutureProg? available,
        FutureProg? skillProg,
        Calendar? calendar)
    {
        Culture culture = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Cultures,
            name,
            x => x.Name,
            () =>
            {
                Culture created = new();
                _context.Cultures.Add(created);
                return created;
            });

        culture.Name = name;
        culture.Description = description;
        culture.TolerableTemperatureCeilingEffect = 0.0;
        culture.TolerableTemperatureFloorEffect = 0.0;
        culture.AvailabilityProg = available ?? _alwaysTrueProg;
        culture.PrimaryCalendarId = calendar?.Id ?? 1;
        culture.SkillStartingValueProg = skillProg ?? _skillStartProg;
        culture.PersonWordFemale = "Woman";
        culture.PersonWordIndeterminate = "Person";
        culture.PersonWordMale = "Man";
        culture.PersonWordNeuter = "Person";
        _context.SaveChanges();
        _cultures[name] = culture;
        _cultureProgs[name] = SeederRepeatabilityHelper.EnsureProg(
            _context,
            $"IsCulture{name.CollapseString()}",
            "Character",
            "Culture",
            ProgVariableTypes.Boolean,
            $"Determines whether someone is the {name} culture",
            $"return @ch.Culture == ToCulture({culture.Id})",
            true,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Toon, "ch"));
        return culture;
    }

    private void ReplaceCultureNameLinks(Culture culture, params (Gender Gender, string NameCulture)[] mappings)
    {
        foreach (CulturesNameCultures? existing in culture.CulturesNameCultures.ToList())
        {
            _context.CulturesNameCultures.Remove(existing);
        }

        foreach ((Gender gender, string? cultureName) in mappings)
        {
            culture.CulturesNameCultures.Add(new CulturesNameCultures
            {
                Culture = culture,
                NameCulture = _context.NameCultures.First(x => x.Name == cultureName),
                Gender = (short)gender
            });
        }

        _context.SaveChanges();
    }
}
