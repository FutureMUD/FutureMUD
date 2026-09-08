using System.Security.Cryptography;
using MudSharp.CharacterCreation;

#nullable enable
namespace MudSharp.NPC.Templates;

public enum GeneratedOptionalSkillPolicy { Decline, Fill }

public static class GeneratedSkillGroupAdapter
{
	/// <summary>Explicit opt-in only. Hand-authored values are independent grants and remain authoritative.</summary>
	public static ChargenSkillClaims Apply(SimpleCharacterTemplate template, int seed,
		GeneratedOptionalSkillPolicy optionalPolicy, ChargenSkillClaims? saved = null)
	{
		if (!Enum.IsDefined(optionalPolicy)) throw new ArgumentOutOfRangeException(nameof(optionalPolicy));
		var previousAwards = template.SkillGroupClaims?.GroupSkills.Except(template.SkillGroupClaims.Mandatory).ToHashSet() ?? [];
		var independent = template.SelectedSkills.Select(x => x.Id).Except(previousAwards).ToList();
		var chargen = (Chargen)Chargen.CreateSkillEvaluationContext(template, independent);
		saved ??= template.SkillGroupClaims;
		chargen.RestoreSkillClaims(saved is null ? new ChargenSkillClaims() : ChargenSkillClaims.Load(saved.Save()));
		chargen.SkillClaims.Independent.Clear();
		chargen.SkillClaims.Independent.UnionWith(independent);
		chargen.SkillClaims.IndependentValues.Clear();
		foreach (var value in template.SkillValues.Where(x => independent.Contains(x.Item1.Id)))
			chargen.SkillClaims.IndependentValues[value.Item1.Id] = value.Item2;
		var storyboard = template.Gameworld.ChargenStoryboard?.StageScreenMap.GetValueOrDefault(ChargenStage.SelectSkills);
		var resolver = new SkillGroupResolver(chargen, Chargen.FreeSkillsProgForStoryboard(storyboard), independent);
		if (resolver.Errors.Count > 0) throw new InvalidOperationException(string.Join(" ", resolver.Errors));
		foreach (var group in resolver.Groups)
		{
			var claim = chargen.SkillClaims.Groups[group.StableKey];
			if (claim.Complete) continue;
			var target = optionalPolicy == GeneratedOptionalSkillPolicy.Fill ? group.MaximumPicks : group.MinimumPicks;
			foreach (var candidate in resolver.Candidates[group.StableKey].OrderBy(x =>
				Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(FormattableString.Invariant($"{seed}:{group.StableKey}:{x.Id}")))), StringComparer.Ordinal))
			{
				if (claim.Skills.Count >= target) break;
				resolver.Pick(group, candidate);
			}
			var error = resolver.Finish(group);
			if (error is not null) throw new InvalidOperationException(error);
		}
		template.SkillValues.RemoveAll(x => previousAwards.Contains(x.Item1.Id) && !chargen.SkillClaims.GroupSkills.Contains(x.Item1.Id));
		foreach (var skill in chargen.SelectedSkills.Where(x => template.SkillValues.All(y => y.Item1.Id != x.Id)))
		{
			var value = Convert.ToDouble(template.SelectedCulture.SkillStartingValueProg.Execute(chargen, skill, 0));
			template.SkillValues.Add((skill, Math.Min(value, chargen.TraitMaxValue(skill))));
		}
		foreach (var skill in chargen.SelectedSkills)
		{
			foreach (var language in template.Gameworld.Languages?.Where(x => x.LinkedTrait == skill) ?? [])
			{
				if (template.SelectedAccents.Any(x => x.Language == language)) continue;
				var nonNative = template.Gameworld.FutureProgs?.FirstOrDefault(x =>
					x.FunctionName.Equals($"CultureNonNativeAccent{language.Id}", StringComparison.OrdinalIgnoreCase));
				var isNative = nonNative is not null && !nonNative.ExecuteBool(chargen);
				bool Learner(MudSharp.Communication.Language.IAccent candidate) => candidate == language.DefaultLearnerAccent ||
					new[] { "learner", "foreign", "crude" }.Contains(candidate.Group, StringComparer.OrdinalIgnoreCase) ||
					new[] { "learner", "foreign", "crude" }.Contains(candidate.Name, StringComparer.OrdinalIgnoreCase);
				var accent = isNative
					? language.Accents.Where(x => !Learner(x) && x.IsAvailableInChargen(chargen)).OrderBy(x => x.Id).FirstOrDefault()
					: language.DefaultLearnerAccent is { } learner && learner.IsAvailableInChargen(chargen) ? learner
					: language.Accents.Where(x => Learner(x) && x.IsAvailableInChargen(chargen)).OrderBy(x => x.Id).FirstOrDefault();
				if (accent is null)
					throw new InvalidOperationException($"Language {language.Name} ({language.Id}) has no eligible {(isNative ? "native/regional" : "learner/foreign")} accent for ethnicity {template.SelectedEthnicity?.Id}; learner pointer {language.DefaultLearnerAccent?.Id}. Check accent availability restrictions.");
				template.SelectedAccents.Add(accent);
			}
		}
		template.SkillGroupClaims = chargen.SkillClaims;
		return chargen.SkillClaims;
	}
}
