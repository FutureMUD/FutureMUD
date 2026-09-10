#nullable enable

using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
    private readonly Dictionary<string, Language> _languages = new(StringComparer.OrdinalIgnoreCase);
    internal static IReadOnlyCollection<string> RpiLegacyMiddleEarthLanguageNamesForTesting =>
    [
        "Taliska",
        "Haladin",
        "Thrunon",
        "Beast-Tongue",
        "Valarin",
        "Nandorin",
        "Druag",
        "Atliduk",
        "Adunaic",
        "Haradaic",
        "Westron",
        "Dunael",
        "Labba",
        "Norliduk",
        "Rohirric",
        "Talathic",
        "Umitic",
        "Nahaiduk",
        "Pukael",
        "Sindarin",
        "Quenya",
        "Silvan",
        "Avarin",
        "Khuzdul",
        "Orkish",
        "Black Speech",
        "Trollish"
    ];

    public void AddLanguage(string name, string unknownDescription, FutureProg? canSelectProg = null)
    {
        Language language = EnsureLanguage(name, unknownDescription, canSelectProg);
        Accent accent = EnsureAccent(
            language,
            "Foreign",
            "with a foreign accent",
            "with a foreign accent",
            (int)Difficulty.Normal,
            $"The heavily-foreign accent of a non-native learner of the {name} language",
            "Foreign");
        language.DefaultLearnerAccent = accent;
        _context.SaveChanges();
    }

    private void AddAccent(string language, string name, string suffix, string vague, int difficulty,
        string description, string group, FutureProg? prog = null)
    {
        EnsureAccent(_languages[language], name, suffix, vague, difficulty, description, group, prog);
    }

    private void AddAccent(string language, string name, string suffix, string vague, Difficulty difficulty,
        string description, string group, FutureProg? prog = null)
    {
        EnsureAccent(_languages[language], name, suffix, vague, (int)difficulty, description, group, prog);
    }

    private void AddMutualIntelligability(string from, string to, Difficulty difficulty, bool twoWay = false)
    {
        EnsureMutualIntelligability(from, to, difficulty, twoWay);
    }

    public void AddScript(string name, string known, string unknown, string description, string subtype, double length,
        double ink, params string[] languages)
    {
        Script? existingScript = _context.Scripts
            .AsEnumerable()
            .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        List<Language> existingLanguages = existingScript is null
            ? []
            : _context.ScriptsDesignedLanguages
                .Where(x => x.ScriptId == existingScript.Id)
                .Select(x => x.Language)
                .ToList();
        foreach (Language language in existingLanguages)
        {
            _languages.TryAdd(language.Name, language);
        }

        string[] designedLanguages = languages
            .Concat(existingLanguages.Select(x => x.Name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        StringBuilder sb = new();
        sb.AppendLine("switch (@skill.Name)");
        foreach (string language in designedLanguages)
        {
            sb.AppendLine($"  case (\"{language}\")");
            sb.AppendLine("    return true");
        }

        sb.AppendLine("end switch");
        sb.AppendLine("return false");

        FutureProg prog = SeederRepeatabilityHelper.EnsureProg(
            _context,
            $"CanPick{name.Replace("-", "").Replace(" ", "")}ScriptKnowledge",
            "Knowledges",
            "Scripts",
            ProgVariableTypes.Boolean,
            $"Controls whether someone can pick the {name} script knowledge during character creation. Will let them pick it if they have one of the language skills that the script is designed for.",
            sb.ToString(),
            true,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Chargen, "ch"),
            (ProgVariableTypes.Trait, "skill"));
        EnsureScript(name, known, unknown, description, subtype, length, ink, prog, designedLanguages);
    }
}
