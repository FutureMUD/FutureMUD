#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body.Traits;

/// <summary>A role's own entitlement, independent of display names and native additive role grants.</summary>
public sealed record RoleSkillRule(long RoleId, long TraitId, bool Primary, double Opening, double Cap,
	long? ParentTraitId = null, double Threshold = 0, double OverlapBonus = 0);

public sealed record RoleSkillEntitlement(long TraitId, double Opening, double Cap, long? ParentTraitId,
	double Threshold, double OverlapBonus)
{
	public bool InitiallyGranted => ParentTraitId is null;
	public double ComposedOpening => Math.Min(Cap, Opening + OverlapBonus);
}

/// <summary>Composes primary trees and secondary associations without bypassing a primary prerequisite.</summary>
public sealed class RoleSkillProgression
{
	public RoleSkillProgression(IEnumerable<RoleSkillRule> rules)
	{
		Rules = rules.ToArray();
		if (Rules.Any(x => x.RoleId <= 0 || x.TraitId <= 0 || x.ParentTraitId is <= 0 ||
			!double.IsFinite(x.Opening) || !double.IsFinite(x.Cap) || !double.IsFinite(x.Threshold) ||
			!double.IsFinite(x.OverlapBonus) || x.Opening < 0 || x.Cap < x.Opening || x.Threshold < 0 ||
			x.OverlapBonus < 0 || !x.Primary && x.ParentTraitId is not null) ||
			Rules.GroupBy(x => (x.RoleId, x.TraitId)).Any(x => x.Count() != 1))
			throw new ArgumentException("Role skill rules require unique role/skill identities and finite nonnegative values.", nameof(rules));
		foreach (var rule in Rules.Where(x => x.ParentTraitId.HasValue))
		{
			var path = new HashSet<long> { rule.TraitId };
			var current = rule;
			while (current.ParentTraitId is { } parent)
			{
				if (!path.Add(parent)) throw new ArgumentException("Role skill trees must be acyclic.", nameof(rules));
				current = Rules.SingleOrDefault(x => x.RoleId == rule.RoleId && x.Primary && x.TraitId == parent)
					?? throw new ArgumentException("A primary prerequisite must belong to the same role.", nameof(rules));
			}
		}
	}

	public IReadOnlyList<RoleSkillRule> Rules { get; }

	public IReadOnlyDictionary<long, RoleSkillEntitlement> Compose(IEnumerable<long> selectedRoles)
	{
		var roles = selectedRoles.ToHashSet();
		return Rules.Where(x => roles.Contains(x.RoleId)).GroupBy(x => x.TraitId).ToDictionary(x => x.Key, group =>
		{
			var primary = group.Where(x => x.Primary).ToArray();
			if (primary.Length > 1) throw new InvalidOperationException("Multiple primary roles claim the same skill.");
			var secondary = group.Where(x => !x.Primary).ToArray();
			if (secondary.Length > 1) throw new InvalidOperationException("Multiple secondary roles claim the same skill.");
			var chosen = primary.FirstOrDefault() ?? secondary.Single();
			return new RoleSkillEntitlement(chosen.TraitId, chosen.Opening, chosen.Cap, chosen.ParentTraitId,
				chosen.Threshold, primary.Length == 1 ? secondary.FirstOrDefault()?.OverlapBonus ?? 0 : 0);
		});
	}
}
