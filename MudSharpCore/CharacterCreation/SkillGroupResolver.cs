using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;

#nullable enable
namespace MudSharp.CharacterCreation;

public sealed class SkillGroupResolver
{
	private readonly IChargen _chargen;
	private readonly IFutureProg? _freeSkills;
	private readonly HashSet<long>? _independent;
	public List<IChargenSkillSelectionGroup> Groups { get; } = [];
	public Dictionary<string, List<ITraitDefinition>> Candidates { get; } = [];
	public List<string> Errors { get; } = [];
	public List<string> Changes { get; } = [];
	public ChargenSkillClaims Claims => _chargen.SkillClaims;
	public bool Complete => Errors.Count == 0 && Groups.All(x => Claims.Groups[x.StableKey].Complete && Claims.Groups[x.StableKey].Skills.Count >= x.MinimumPicks);

	public SkillGroupResolver(IChargen chargen, IFutureProg? freeSkills, IEnumerable<long>? independent = null)
	{
		_chargen = chargen;
		_freeSkills = freeSkills;
		_independent = independent?.ToHashSet();
		var legacy = !Claims.Initialised;
		if (legacy)
		{
			Claims.Ordinary.UnionWith(chargen.SelectedSkills.Select(x => x.Id));
			Claims.Initialised = true;
		}
		Revalidate();
		if (legacy) Claims.Ordinary.ExceptWith(Claims.Mandatory);
	}

	public void Revalidate()
	{
		Errors.Clear(); Groups.Clear(); Candidates.Clear();
		try
		{
			var independent = (_independent ?? Claims.Independent).Concat(_chargen.SelectedRoles.SelectMany(x => x.TraitAdjustments)
				.Where(x => x.Value.giveIfMissing && x.Key is ISkillDefinition).Select(x => x.Key.Id)).ToHashSet();
			var context = Chargen.CreateSkillEvaluationContext(_chargen, independent);
			var free = (_freeSkills?.ExecuteCollection<ITraitDefinition>(context) ?? []).ToList();
			if (free.Any(x => x is not ISkillDefinition)) throw new InvalidOperationException("Mandatory skills contain an invalid trait.");
			Claims.Mandatory.Clear();
			Claims.Mandatory.UnionWith(independent);
			Claims.Mandatory.UnionWith(free.Select(x => x.Id));
			foreach (var group in (_chargen.Gameworld.ChargenSkillSelectionGroups ?? []).Where(x => x.Enabled && !x.Retired)
				.OrderBy(x => x.DisplayOrder).ThenBy(x => x.StableKey, StringComparer.Ordinal))
			{
				var problems = group.Validate().ToList();
				if (problems.Count > 0) { Errors.Add($"{group.Name}: {string.Join(" ", problems)}"); continue; }
				if (!Evaluate(group.EligibilityProg!, null)) continue;
				var candidates = new List<ITraitDefinition>();
				foreach (var skill in group.Members)
				{
					if (skill is not ISkillDefinition) throw new InvalidOperationException($"{group.Name} contains an invalid skill.");
					if (group.ExistingSkillPolicy == ExistingSkillPolicy.NewOnly && Claims.Mandatory.Contains(skill.Id)) continue;
					if (!skill.ChargenAvailable(Chargen.CreateSkillEvaluationContext(_chargen, Claims.Mandatory))) continue;
					if (group.MemberEligibilityProg is not null && !Evaluate(group.MemberEligibilityProg, skill)) continue;
					candidates.Add(skill);
				}
				Groups.Add(group);
				Candidates[group.StableKey] = candidates;
				if (!Claims.Groups.TryGetValue(group.StableKey, out var claim))
					Claims.Groups[group.StableKey] = claim = new ChargenSkillGroupClaim { GroupId = group.Id, Revision = group.Revision };
				var removed = claim.Skills.RemoveWhere(x => candidates.All(y => y.Id != x));
				claim.Credits.Clear();
				claim.Credits.UnionWith(claim.Skills.Intersect(Claims.Mandatory));
				if (claim.Revision != group.Revision || removed > 0)
				{
					claim.Complete = false; claim.Revision = group.Revision;
					Changes.Add($"{group.Name} changed; review your retained choices.");
				}
				if (claim.Skills.Count < group.MinimumPicks || claim.Skills.Count > group.MaximumPicks) claim.Complete = false;
			}
			// Configuration failures retain unresolved claims and block progression, never waive an entitlement.
			if (Errors.Count == 0)
			{
				foreach (var key in Claims.Groups.Keys.Where(x => Groups.All(y => y.StableKey != x)).ToList())
				{
					Claims.Groups.Remove(key);
					Changes.Add("A group no longer applies. Its ordinary and independently owned skills remain.");
				}
				if (!Feasible()) Errors.Add("These group choices cannot satisfy every required minimum. Unpick or reallocate a choice, or ask staff to repair the groups.");
			}
		}
		catch (Exception ex) { Errors.Add($"Skill group configuration error: {ex.Message}"); }
		Project();
	}

	private bool Evaluate(IFutureProg prog, ITraitDefinition? member)
	{
		var context = Chargen.CreateSkillEvaluationContext(_chargen, Claims.Mandatory);
		var value = member is null ? prog.Execute(context) : prog.Execute(context, member);
		return value is bool boolean ? boolean : throw new InvalidOperationException($"Prog {prog.FunctionName} returned no Boolean result.");
	}

	private bool Feasible() => SkillGroupAllocation.IsFeasible(Groups.Select(x => new SkillGroupAllocation(x.StableKey,
		x.MinimumPicks, x.MaximumPicks, Candidates[x.StableKey].Select(y => y.Id).ToList(), Claims.Groups[x.StableKey].Skills)));

	public string? Pick(IChargenSkillSelectionGroup group, ITraitDefinition skill)
	{
		if (Errors.Count > 0) return "Resolve the reported allocation or configuration errors before adding another choice.";
		if (!Candidates.ContainsKey(group.StableKey)) return "This group has a configuration error.";
		var claim = Claims.Groups[group.StableKey];
		if (!Candidates[group.StableKey].Contains(skill)) return "That skill is not eligible.";
		if (Claims.Groups.Values.Any(x => x.Skills.Contains(skill.Id))) return "That skill already occupies a group slot. Unpick it from its owner first.";
		claim.Skills.Add(skill.Id);
		if (!Feasible()) { claim.Skills.Remove(skill.Id); return "That choice would exceed a bound or leave another required group impossible."; }
		if (Claims.Mandatory.Contains(skill.Id)) claim.Credits.Add(skill.Id);
		claim.Complete = false;
		Project();
		return null;
	}

	public void Unpick(IChargenSkillSelectionGroup group, ITraitDefinition skill)
	{
		var claim = Claims.Groups[group.StableKey];
		claim.Skills.Remove(skill.Id); claim.Credits.Remove(skill.Id); claim.Complete = false;
		Revalidate();
	}

	public string? Finish(IChargenSkillSelectionGroup group)
	{
		var claim = Claims.Groups[group.StableKey];
		if (Errors.Count > 0 || claim.Skills.Count < group.MinimumPicks || claim.Skills.Count > group.MaximumPicks)
			return "The required counts must be satisfied before finishing this decision.";
		claim.Complete = true;
		return null;
	}

	public void CaptureOrdinary()
	{
		CaptureOrdinaryClaims(_chargen);
		Project();
	}

	public static void CaptureOrdinaryClaims(IChargen chargen)
	{
		if (chargen.SkillClaims?.Initialised != true) return;
		var claims = chargen.SkillClaims;
		var covered = claims.Mandatory.Concat(claims.GroupSkills).ToHashSet();
		claims.Ordinary.RemoveWhere(x => !covered.Contains(x) && chargen.SelectedSkills.All(y => y.Id != x));
		claims.Ordinary.UnionWith(chargen.SelectedSkills.Where(x => !covered.Contains(x.Id)).Select(x => x.Id));
	}

	public void Project()
	{
		_chargen.SelectedSkills = Claims.Selected.Select(x => _chargen.Gameworld.Traits.Get(x)).Where(x => x is ISkillDefinition).ToList();
	}

	public static IEnumerable<ITraitDefinition> FreeSkills(IChargen chargen, IEnumerable<ITraitDefinition> legacy) =>
		chargen.SkillClaims?.Initialised == true
			? chargen.SkillClaims.Mandatory.Concat(chargen.SkillClaims.GroupSkills).Distinct().Select(x => chargen.Gameworld.Traits.Get(x)).OfType<ITraitDefinition>()
			: legacy;

	public static IEnumerable<ITraitDefinition> MandatorySkills(IChargen chargen, IFutureProg? prog) =>
		chargen.SkillClaims?.Initialised == true
			? chargen.SkillClaims.Mandatory.Select(x => chargen.Gameworld.Traits.Get(x)).OfType<ITraitDefinition>()
			: prog?.ExecuteCollection<ITraitDefinition>(chargen) ?? [];

	public static IEnumerable<ITraitDefinition> UnavailableOrdinarySkills(IChargen chargen) =>
		chargen.SkillClaims?.Initialised == true
			? chargen.SkillClaims.ChargeableOrdinary.Select(x => chargen.Gameworld.Traits.Get(x)).OfType<ITraitDefinition>()
				.Where(x => !x.ChargenAvailable(chargen)) : [];
}
