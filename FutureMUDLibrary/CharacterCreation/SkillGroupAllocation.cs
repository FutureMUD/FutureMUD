using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace MudSharp.CharacterCreation;

public sealed record SkillGroupAllocation(string Key, int Minimum, int Maximum,
	IReadOnlyCollection<long> Candidates, IReadOnlyCollection<long> Claims)
{
	/// <summary>Matches each outstanding slot to a distinct trait, including multi-pick groups.</summary>
	public static bool IsFeasible(IEnumerable<SkillGroupAllocation> source)
	{
		var groups = source.ToList();
		var claimed = new HashSet<long>();
		foreach (var group in groups)
		{
			if (group.Minimum < 0 || group.Maximum < group.Minimum || group.Claims.Count > group.Maximum ||
				group.Claims.Any(x => !group.Candidates.Contains(x) || !claimed.Add(x))) return false;
		}
		var slots = groups.SelectMany(x => Enumerable.Repeat(x.Candidates.Where(y => !claimed.Contains(y)).ToArray(),
			Math.Max(0, x.Minimum - x.Claims.Count))).ToList();
		var owners = new Dictionary<long, int>();
		bool Match(int slot, HashSet<long> visited)
		{
			foreach (var skill in slots[slot])
			{
				if (!visited.Add(skill)) continue;
				if (owners.TryGetValue(skill, out var owner) && !Match(owner, visited)) continue;
				owners[skill] = slot;
				return true;
			}
			return false;
		}
		return Enumerable.Range(0, slots.Count).All(x => Match(x, []));
	}
}
