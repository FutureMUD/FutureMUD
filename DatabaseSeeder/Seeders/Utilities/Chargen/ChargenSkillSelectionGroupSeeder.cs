using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

#nullable enable
namespace DatabaseSeeder.Seeders.Utilities.Chargen;

public sealed record SkillGroupSeedDefinition(string StableKey, string Name, string Description,
	int MinimumPicks, int MaximumPicks, int DisplayOrder, ExistingSkillPolicy ExistingSkillPolicy,
	bool Enabled, IReadOnlyList<TraitDefinition> Members, IFutureProg EligibilityProg, IFutureProg? MemberEligibilityProg = null);

/// <summary>Content boundary: callers resolve their skills and compiled progs; no storyboard dependency.</summary>
public static class ChargenSkillSelectionGroupSeeder
{
	public static ChargenSkillSelectionGroup Upsert(FuturemudDatabaseContext context, SkillGroupSeedDefinition definition,
		ICollection<string> conflicts)
	{
		static bool ProgValid(IFutureProg? prog, bool member) => prog is not null && prog.ReturnType == ProgVariableTypes.Boolean &&
			!prog.AcceptsAnyParameters && prog.Parameters.SequenceEqual(member ? [ProgVariableTypes.Chargen, ProgVariableTypes.Trait] : [ProgVariableTypes.Chargen]) && prog.Compile();
		if (string.IsNullOrWhiteSpace(definition.StableKey) || definition.StableKey.Length > 191 ||
			string.IsNullOrWhiteSpace(definition.Name) || definition.Name.Length > 200 ||
			definition.MinimumPicks < 0 || definition.MaximumPicks < definition.MinimumPicks || definition.MinimumPicks > definition.Members.Count ||
			definition.Members.Select(x => x.Id).Distinct().Count() != definition.Members.Count ||
			definition.Members.Any(x => x.Id <= 0 || context.TraitDefinitions.Find(x.Id) is not { } installed ||
				(TraitType)installed.Type is not (TraitType.Skill or TraitType.DerivedSkill or TraitType.TheoreticalSkill) ||
				(TraitOwnerScope)installed.OwnerScope is not (TraitOwnerScope.Body or TraitOwnerScope.Character)) ||
			!ProgValid(definition.EligibilityProg, false) || context.FutureProgs.Find(definition.EligibilityProg.Id) is null ||
			(definition.MemberEligibilityProg is not null && (!ProgValid(definition.MemberEligibilityProg, true) || context.FutureProgs.Find(definition.MemberEligibilityProg.Id) is null)))
			throw new ArgumentException("A group requires resolved genuine skills, compiled compatible progs and valid bounds.", nameof(definition));

		var desired = new ChargenSkillSelectionGroup
		{
			StableKey = definition.StableKey, Name = definition.Name, Description = definition.Description,
			MinimumPicks = definition.MinimumPicks, MaximumPicks = definition.MaximumPicks,
			DisplayOrder = definition.DisplayOrder, ExistingSkillPolicy = (int)definition.ExistingSkillPolicy,
			Enabled = definition.Enabled, EligibilityProgId = definition.EligibilityProg.Id,
			MemberEligibilityProgId = definition.MemberEligibilityProg?.Id
		};
		foreach (var (member, i) in definition.Members.Select((x, i) => (x, i)))
			desired.Members.Add(new ChargenSkillSelectionGroupMember { TraitDefinitionId = member.Id, DisplayOrder = i });
		var next = Fields(desired);
		var current = context.ChargenSkillSelectionGroups.Include(x => x.Members).SingleOrDefault(x => x.StableKey == definition.StableKey);
		if (current is null)
		{
			desired.SeedBaseline = JsonSerializer.Serialize(next);
			context.ChargenSkillSelectionGroups.Add(desired);
			context.SaveChanges();
			return desired;
		}
		if (current.SeedBaseline is null) throw new InvalidOperationException("The stable key belongs to an unmanaged group; refusing to take ownership.");
		var previous = JsonSerializer.Deserialize<Dictionary<string, string>>(current.SeedBaseline)!;
		var actual = Fields(current);
		var merged = Merge(previous, actual, next, conflicts);
		if (!actual.OrderBy(x => x.Key).SequenceEqual(merged.OrderBy(x => x.Key)))
		{
			Apply(context, current, merged);
			current.Revision++;
		}
		current.SeedBaseline = JsonSerializer.Serialize(next);
		if (current.MinimumPicks > current.Members.Count || current.MaximumPicks < current.MinimumPicks || current.Retired)
		{
			current.Enabled = false;
			conflicts.Add("Merged group is retired or has unfillable bounds; left disabled.");
		}
		context.SaveChanges();
		return current;
	}

	public static Dictionary<string, string> Merge(IReadOnlyDictionary<string, string> previous,
		IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> desired, ICollection<string> conflicts)
	{
		var merged = current.ToDictionary(x => x.Key, x => x.Value);
		foreach (var key in previous.Keys.Union(desired.Keys))
		{
			previous.TryGetValue(key, out var old); current.TryGetValue(key, out var actual); desired.TryGetValue(key, out var next);
			if (actual == old)
			{
				if (next is null) merged.Remove(key); else merged[key] = next;
			}
			else if (actual != next && old != next) conflicts.Add($"Builder edit retained: {key}");
		}
		return merged;
	}

	private static Dictionary<string, string> Fields(ChargenSkillSelectionGroup group)
	{
		var fields = new Dictionary<string, string>
		{
			["name"] = group.Name, ["description"] = group.Description,
			["minimum"] = group.MinimumPicks.ToString(), ["maximum"] = group.MaximumPicks.ToString(),
			["order"] = group.DisplayOrder.ToString(), ["existing"] = group.ExistingSkillPolicy.ToString(),
			["enabled"] = group.Enabled.ToString(), ["eligibility"] = (group.EligibilityProgId ?? 0).ToString(),
			["filter"] = (group.MemberEligibilityProgId ?? 0).ToString()
		};
		foreach (var member in group.Members) fields[$"member:{member.TraitDefinitionId}"] = member.DisplayOrder.ToString();
		return fields;
	}

	private static void Apply(FuturemudDatabaseContext context, ChargenSkillSelectionGroup group, Dictionary<string, string> fields)
	{
		group.Name = fields["name"]; group.Description = fields["description"];
		group.MinimumPicks = int.Parse(fields["minimum"]); group.MaximumPicks = int.Parse(fields["maximum"]);
		group.DisplayOrder = int.Parse(fields["order"]); group.ExistingSkillPolicy = int.Parse(fields["existing"]);
		group.Enabled = bool.Parse(fields["enabled"]);
		group.EligibilityProgId = long.Parse(fields["eligibility"]) is var eligibility && eligibility != 0 ? eligibility : null;
		group.MemberEligibilityProgId = long.Parse(fields["filter"]) is var filter && filter != 0 ? filter : null;
		foreach (var member in group.Members.Where(x => !fields.ContainsKey($"member:{x.TraitDefinitionId}")).ToList())
		{
			context.ChargenSkillSelectionGroupMembers.Remove(member); group.Members.Remove(member);
		}
		foreach (var field in fields.Where(x => x.Key.StartsWith("member:", StringComparison.Ordinal)))
		{
			var id = long.Parse(field.Key[7..]);
			var member = group.Members.SingleOrDefault(x => x.TraitDefinitionId == id);
			if (member is null) group.Members.Add(new ChargenSkillSelectionGroupMember { TraitDefinitionId = id, DisplayOrder = int.Parse(field.Value) });
			else member.DisplayOrder = int.Parse(field.Value);
		}
	}
}
