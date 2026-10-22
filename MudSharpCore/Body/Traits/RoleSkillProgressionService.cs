#nullable enable

using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Effects.Concrete;
using MudSharp.Logging;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Traits;

public static class RoleSkillProgressionService
{
	private static IEnumerable<IRoleSkillProgressionMerit> Merits(ICharacterTemplate template) =>
		(template.SelectedRoles ?? []).SelectMany(x => x.AdditionalMerits ?? []).Concat(template.SelectedMerits ?? [])
			.OfType<IRoleSkillProgressionMerit>().DistinctBy(x => x.Id).Where(x => x.AppliesToTemplate(template));
	public static bool Applies(ICharacterTemplate template) => Merits(template).Any();

	public static IReadOnlyDictionary<long, RoleSkillEntitlement> Entitlements(ICharacterTemplate template) =>
		Compose(Merits(template), (template.SelectedRoles ?? []).Select(x => x.Id));
	public static IReadOnlyDictionary<long, RoleSkillEntitlement> Entitlements(ICharacter character) =>
		Compose((character.Merits ?? []).OfType<IRoleSkillProgressionMerit>().Where(x => x.Applies(character)), (character.Roles ?? []).Select(x => x.Id));

	private static IReadOnlyDictionary<long, RoleSkillEntitlement> Compose(IEnumerable<IRoleSkillProgressionMerit> merits, IEnumerable<long> roles)
	{
		var ids = roles.ToHashSet();
		var rules = merits.SelectMany(x => x.Progression.Rules).Where(x => ids.Contains(x.RoleId)).Distinct();
		return new RoleSkillProgression(rules).Compose(ids);
	}

	public static IEnumerable<ITraitDefinition> InitialSkills(ICharacterTemplate template) =>
		Entitlements(template).Values.Where(x => x.InitiallyGranted).Select(x => template.Gameworld.Traits.Get(x.TraitId));

	public static double StartingValue(ICharacterTemplate template, ITraitDefinition skill, double otherwise) =>
		Entitlements(template).TryGetValue(skill.Id, out var entitlement) &&
		(entitlement.InitiallyGranted || template.SelectedSkills.Any(x => x.Id == skill.Id))
			? Math.Max(otherwise, entitlement.ComposedOpening) : otherwise;

	public static bool CanAcquire(ICharacter character, ITraitDefinition skill)
	{
		var entitlements = Entitlements(character);
		// This adapter adds prerequisite gates to the selected tree. It does not replace a
		// world's LearnableProg authority for other skills (including heard languages).
		return !entitlements.TryGetValue(skill.Id, out var entitlement) || PrerequisiteMet(character, entitlement);
	}
	private static bool PrerequisiteMet(ICharacter character, RoleSkillEntitlement entitlement) =>
		entitlement.ParentTraitId is not { } parent || character.HasTrait(character.Gameworld.Traits.Get(parent)) &&
		character.TraitRawValue(character.Gameworld.Traits.Get(parent)) >= entitlement.Threshold;

	/// <summary>Only new character creation creates bonus claims. Login and installer reruns never enrol characters.</summary>
	public static void InitialiseNewCharacter(ICharacter character)
	{
		var entitlements = Entitlements(character);
		if (entitlements.Count == 0) return;
		if (character.EffectsOfType<RoleSkillBonusClaims>().Any()) throw new InvalidOperationException("Role skill claims already initialised.");
		var pending = entitlements.Values.Where(x => !x.InitiallyGranted && x.OverlapBonus > 0 &&
			!character.HasTrait(character.Gameworld.Traits.Get(x.TraitId))).ToDictionary(x => x.TraitId, x => x.ComposedOpening);
		character.AddEffect(new RoleSkillBonusClaims(character, pending));
		foreach (var entitlement in entitlements.Values.Where(x => x.InitiallyGranted || character.HasTrait(character.Gameworld.Traits.Get(x.TraitId))))
		{
			var skill = character.Gameworld.Traits.Get(entitlement.TraitId);
			if (!character.HasTrait(skill)) character.AddTrait(skill, entitlement.ComposedOpening);
			else if (character.TraitRawValue(skill) < entitlement.ComposedOpening) character.SetTraitValue(skill, entitlement.ComposedOpening);
		}
	}

	public static void EvaluateBranches(IHaveTraits user)
	{
		var character = user as ICharacter ?? (user as IBody)?.Actor;
		if (character is null || !CheckLearningScope.CanContinue(user)) return;
		foreach (var entitlement in Entitlements(character).Values.Where(x => !x.InitiallyGranted))
		{
			var skill = (ISkillDefinition)character.Gameworld.Traits.Get(entitlement.TraitId);
			if (character.HasTrait(skill) || !PrerequisiteMet(character, entitlement) || !skill.CanLearn(character)) continue;
			if (!CheckLearningScope.CanContinue(user)) break;
			if (character.HasTrait(skill)) continue;
			if (!Entitlements(character).TryGetValue(skill.Id, out var current) || current != entitlement || !PrerequisiteMet(character, current)) continue;
			if (character.AddTrait(skill, entitlement.Opening))
				character.Gameworld.LogManager.CustomLogEntry(LogEntryType.SkillBranch, character, skill, Outcome.MajorPass);
		}
	}
}
