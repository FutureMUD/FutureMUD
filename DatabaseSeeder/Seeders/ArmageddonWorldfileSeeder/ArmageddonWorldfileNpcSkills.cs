#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static readonly IReadOnlyDictionary<string, string> SourceNpcSkillAliases =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["dual wield"] = "arm.skill.dual_wield",
			["throw"] = "arm.skill.throw",
			["pilot"] = "arm.skill.pilot",
			["shield use"] = "arm.skill.shield_use"
		};

	internal static Dictionary<string, long> NpcSkillBindings(IEnumerable<TraitDefinition> traits,
		IEnumerable<SeederManagedRecord> ownership)
	{
		var native = traits.Where(x => x.Type == 0).ToArray();
		var byId = native.ToDictionary(x => x.Id);
		var result = native.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.First().Id, StringComparer.OrdinalIgnoreCase);
		var owned = ownership.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module == ArmageddonSkillPackage.Module && x.EntityType == nameof(TraitDefinition) && !x.Retired && x.LogicalId.HasValue)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value, StringComparer.Ordinal);
		var canonical = new Dictionary<string, long>(StringComparer.Ordinal);
		foreach (var entry in ArmageddonSkillPackage.Skills)
		{
			long id;
			if (owned.TryGetValue(entry.Key, out id))
			{
				if (!byId.TryGetValue(id, out var target) || target.OwnerScope != (int)TraitOwnerScope.Character)
					throw new InvalidOperationException($"Owned NPC skill {entry.Key} has no native character skill.");
			}
			else if (!result.TryGetValue(entry.Name, out id)) continue;
			canonical.Add(entry.Key, id);
			result[entry.Name] = id;
		}
		foreach (var alias in SourceNpcSkillAliases)
			if (canonical.TryGetValue(alias.Value, out var id)) result[alias.Key] = id;
		return result;
	}

	internal sealed record UnmappedNpcSkill(int Index, string? Name, int Value);

	internal static void ApplyNpcRacialSkills(IDictionary<long, double> skills, int sourceRace,
		IReadOnlyDictionary<string, long> bindings)
	{
		string[] grants = sourceRace switch
		{
			2 => ["steal", "haggle"],
			12 => ["sneak", "hide", "climb"],
			4 => ["hide"],
			98 => ["sneak", "hide"],
			_ => []
		};
		foreach (var name in grants)
		{
			if (!bindings.TryGetValue(name, out var id))
				throw new InvalidOperationException($"Source NPC racial skill {name} has no installed trait.");
			var current = skills.TryGetValue(id, out var value) ? value : 0;
			skills[id] = current > 0 ? Math.Min(100, current + 10) : 20;
		}
	}

	internal static IReadOnlyList<UnmappedNpcSkill> ApplyNpcSavedSkills(IDictionary<long, double> skills,
		IEnumerable<int[]> source, IReadOnlyList<string> names, IReadOnlyDictionary<string, long> bindings, long? willpower)
	{
		var unmapped = new List<UnmappedNpcSkill>();
		foreach (var entry in source)
		{
			if (entry.Length < 2 || entry[0] <= 0) continue;
			var name = entry[0] < names.Count ? names[entry[0]] : null;
			// The separate Willpower foundation owns this identity even if its display name changes.
			if (entry[0] == 91 && willpower is { } willpowerId) skills[willpowerId] = Math.Clamp(entry[1], 0, 100);
			else if (name is not null && bindings.TryGetValue(name, out var id)) skills[id] = Math.Clamp(entry[1], 0, 100);
			else unmapped.Add(new(entry[0], name, entry[1]));
		}
		return unmapped;
	}
}
