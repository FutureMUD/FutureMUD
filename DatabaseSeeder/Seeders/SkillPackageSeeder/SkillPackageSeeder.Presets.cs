#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

internal partial class SkillPackageSeeder
{
	internal sealed record SkillPreset(string Label, string Complexity, bool Gerund, bool Modern);

	internal static IReadOnlyList<SkillPreset> Presets { get; } = Array.AsReadOnly(
		new[] { "Simple", "Complex", "RPI" }
			.SelectMany(package => new[] { true, false }
				.SelectMany(gerund => new[] { false, true }.Select(modern => new SkillPreset(
					$"{package}-{(gerund ? "Gerund" : "NonGerund")}{(modern ? "-Modern" : "")}",
					package.ToLowerInvariant(), gerund, modern))))
			.Append(new SkillPreset("Armageddon", "armageddon", false, false))
			.ToArray());

	private static readonly string[] LegacyKeys =
		["complexity", "gerund", "modern", "skillcapmodel", "skillgainmodel", "branching"];

	public override IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
	{
		get
		{
			var options = Presets.Select(x => $"#B{x.Label}#F: " + (x.Complexity == "armageddon"
				? "Canonical setting skills; class caps and role branching; new worlds only."
				: $"{x.Complexity} skills, {(x.Gerund ? "ing-style" : "non-gerund")} names, {(x.Modern ? "including modern skills" : "without modern skills")}."));
			yield return ("skillpreset", "Choose a skill package preset. Read down each column; enter the option name.\n\n" +
				string.Join("\n", options) +
				"\n\nSimple groups related skills; Complex separates them; RPI also includes legacy RPI skills. " +
				"Ordinary presets retain your separate cap, improvement and branching choices. " +
				"Armageddon supplies its fixed approved policy. A rerun is not a world-conversion tool.",
				(_, _) => true, ValidatePresetSelection);

			// Retain the old IDs for saved-answer/replay compatibility, without displaying replaced prompts.
			foreach (var question in LegacyQuestions)
			{
				var replaced = question.Id is "complexity" or "gerund" or "modern";
				yield return (question.Id, question.Question,
					(context, answers) => !replaced && !IsArmageddonPreset(answers) && question.Filter(context, answers),
					question.Validator);
			}
		}
	}

	private static SkillPreset? Preset(string label) =>
		Presets.FirstOrDefault(x => x.Label.Equals(label.Trim(), StringComparison.OrdinalIgnoreCase));

	private static bool IsArmageddonPreset(IReadOnlyDictionary<string, string> answers) =>
		answers.TryGetValue("skillpreset", out var label) && Preset(label)?.Complexity == "armageddon";

	private static bool TryBoolean(string value, out bool result)
	{
		result = value.Equals("yes", StringComparison.OrdinalIgnoreCase) || value.Equals("y", StringComparison.OrdinalIgnoreCase);
		return result || value.Equals("no", StringComparison.OrdinalIgnoreCase) || value.Equals("n", StringComparison.OrdinalIgnoreCase);
	}

	private static bool HasValue(IReadOnlyDictionary<string, string> answers, string key, params string[] values) =>
		answers.TryGetValue(key, out var value) && values.Contains(value, StringComparer.OrdinalIgnoreCase);

	private static bool ValidLegacyAnswers(IReadOnlyDictionary<string, string> answers) =>
		LegacyKeys.All(key => answers.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)) &&
		HasValue(answers, "complexity", "simple", "complex", "rpi", "armageddon") &&
		HasValue(answers, "skillcapmodel", "rpi", "class", "flat") &&
		HasValue(answers, "skillgainmodel", "rpi", "labmud", "armageddon", "successtree") &&
		TryBoolean(answers["gerund"], out _) && TryBoolean(answers["modern"], out _) &&
		TryBoolean(answers["branching"], out _);

	private static bool ValidArmageddonPolicy(IReadOnlyDictionary<string, string> answers) =>
		HasValue(answers, "skillcapmodel", "class") && HasValue(answers, "skillgainmodel", "armageddon") &&
		HasValue(answers, "branching", "no", "n") && HasValue(answers, "modern", "no", "n");

	private static SkillPreset? InferLegacyPreset(IReadOnlyDictionary<string, string> answers)
	{
		if (!ValidLegacyAnswers(answers)) return null;
		if (HasValue(answers, "complexity", "armageddon"))
			return ValidArmageddonPolicy(answers) ? Preset("Armageddon") : null;
		TryBoolean(answers["gerund"], out var gerund);
		TryBoolean(answers["modern"], out var modern);
		return Presets.Single(x => x.Complexity.Equals(answers["complexity"], StringComparison.OrdinalIgnoreCase) &&
			x.Gerund == gerund && x.Modern == modern);
	}

	internal static string? RecordedPreset(FuturemudDatabaseContext context) =>
		InferLegacyPreset(SeederAnswerMemory.GetLatestSeederAnswers(context, "Skill Package"))?.Label;

	private static (bool Success, string error) ValidatePresetSelection(string answer, FuturemudDatabaseContext context)
	{
		var selected = Preset(answer);
		if (selected is null) return (false, "Choose one of the listed skill preset names.");
		var recorded = RecordedPreset(context);
		if (recorded is not null && !selected.Label.Equals(recorded, StringComparison.OrdinalIgnoreCase))
			return (false, $"The recorded skill package is {recorded}. Retain that preset for a supported rerun; changing the installed shape requires a separate migration.");
		return (true, string.Empty);
	}

	/// <summary>Shared by direct callers and executor persistence; no database writes or inferred fresh-world policies.</summary>
	public IReadOnlyDictionary<string, string> NormalizeAnswers(IReadOnlyDictionary<string, string> answers)
	{
		var result = new Dictionary<string, string>(answers, StringComparer.OrdinalIgnoreCase);
		var explicitPreset = result.TryGetValue("skillpreset", out var label);
		var preset = explicitPreset ? Preset(label!) : InferLegacyPreset(result);
		if (preset is null)
			throw new InvalidOperationException(explicitPreset
				? "Unknown skill preset. Choose a listed preset name."
				: "Supply a skillpreset, or a complete valid legacy skill package configuration. Armageddon requires class caps, Armageddon improvement, branching=no and modern=no.");

		void Set(string key, string value)
		{
			if (explicitPreset && result.TryGetValue(key, out var supplied))
			{
				var same = key is "gerund" or "modern" or "branching"
					? TryBoolean(supplied, out var actual) && TryBoolean(value, out var expected) && actual == expected
					: supplied.Equals(value, StringComparison.OrdinalIgnoreCase);
				if (!same) throw new InvalidOperationException($"Skill preset {preset.Label} conflicts with legacy answer {key}. Resolve the conflicting configuration before installation.");
			}
			result[key] = value;
		}

		Set("complexity", preset.Complexity);
		Set("gerund", preset.Gerund ? "yes" : "no");
		Set("modern", preset.Modern ? "yes" : "no");
		if (preset.Complexity == "armageddon")
		{
			Set("skillcapmodel", "class");
			Set("skillgainmodel", "armageddon");
			Set("branching", "no");
		}
		if (!ValidLegacyAnswers(result))
			throw new InvalidOperationException("Ordinary skill presets also require valid skillcapmodel, skillgainmodel and branching answers. These choices are not inferred or overwritten.");
		result["skillpreset"] = preset.Label;
		return result;
	}
}
