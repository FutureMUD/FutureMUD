#nullable enable

using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;

namespace MudSharp.CharacterCreation;

/// <summary>The native constructor's scoped merit composition, without creating an actor or body.</summary>
internal static class CharacterTemplateMerits
{
	internal static void AddSelected(IEnumerable<ICharacterMerit> selected, MeritScope scope,
		List<IMerit> merits, List<ComboMerit> combos)
	{
		foreach (var merit in selected)
		{
			if (merit is ComboMerit combo) combos.Add(combo);
			if (merit.MeritScope == scope) merits.Add(merit);
		}
	}

	internal static void AddRole(IEnumerable<IMerit> additions, MeritScope scope, List<IMerit> merits,
		List<ComboMerit> combos, IEnumerable<IMerit>? otherMerits = null)
	{
		foreach (var merit in additions)
		{
			if (merits.Contains(merit) || otherMerits?.Contains(merit) == true) continue;
			if (merit is ComboMerit combo) combos.Add(combo);
			if (merit.MeritScope == scope) merits.Add(merit);
		}
	}

	internal static void ExpandCombos(IEnumerable<ComboMerit> combos, MeritScope scope, List<IMerit> merits)
	{
		// Native creation expands one level, in authored order. Nested combos are not recursively expanded.
		foreach (var combo in combos)
			foreach (var included in combo.CharacterMerits.Where(x => x.MeritScope == scope))
				if (!merits.Contains(included)) merits.Add(included);
	}

	internal static IReadOnlyList<IMerit> EffectiveCharacterMerits(ICharacterTemplate template)
	{
		List<IMerit> Build(MeritScope scope, IEnumerable<IMerit>? other = null)
		{
			var merits = new List<IMerit>(); var combos = new List<ComboMerit>();
			AddSelected(template.SelectedMerits, scope, merits, combos);
			foreach (var role in template.SelectedRoles) AddRole(role.AdditionalMerits, scope, merits, combos, other);
			ExpandCombos(combos, scope, merits);
			return merits;
		}
		// The body is constructed first; character role-merit deduplication includes those body merits.
		return Build(MeritScope.Character, Build(MeritScope.Body));
	}
}
