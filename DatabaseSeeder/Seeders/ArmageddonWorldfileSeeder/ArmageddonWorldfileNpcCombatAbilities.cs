#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal sealed record NpcSpellRule(string SourceName, string Condition = "always", bool Self = false);
	internal sealed record NpcSavedSpell(int SourceIndex, int Proficiency, int MaximumGrade, NpcSpellRule Rule);

	// These are native policy mappings of the reachable guild dispatch, not a grant of a guild curriculum.
	internal static IReadOnlyList<NpcSpellRule> NpcSpellPriority(int guild) => guild switch
	{
		1 => [new("lightning bolt"), new("fireball")],
		5 => [new("heal", "injured", true), new("dispel magick", "buffed"), new("silence", "classic-caster-unsilenced"),
			new("blind", "sighted"), new("banishment"), new("fireball"), new("health drain")],
		9 => [new("dispel magick", "buffed"), new("blind", "sighted"), new("aura drain", "classic-caster-aura"),
			new("demonfire"), new("fireball"), new("flamestrike")],
		10 => [new("heal", "injured", true), new("calm", "angered"), new("poison", "unpoisoned"),
			new("health drain"), new("silence", "caster-unsilenced")],
		11 => [new("stone skin", "unbuffed", true), new("feeblemind", "classic-caster-unbuffed"),
			new("strength", "unbuffed", true), new("weaken", "unbuffed"), new("stamina drain"),
			new("banishment", "above-second-tier"), new("repel", "losing"), new("blind", "sighted")],
		12 => [new("banishment", "outdoors"), new("repel"), new("teleport")],
		14 => [new("blind", "sighted"), new("curse", "source-blind-slot"), new("fear", "unbuffed")],
		15 => [new("paralyze", "mobile"), new("slow", "unbuffed"), new("lightning bolt")],
		16 => [new("psionic suppression", "unsuppressed"), new("aura drain")],
		_ => []
	};

	internal static IReadOnlyList<NpcSavedSpell> NpcSavedCombatSpells(ArmWorldNpc npc, IReadOnlyList<string> names)
	{
		var saved = npc.Skills.Where(x => x.Length >= 2).GroupBy(x => x[0]).ToDictionary(x => x.Key, x => x.Last()[1]);
		var byName = names.Select((name, index) => (name, index))
			.GroupBy(x => x.name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.First().index, StringComparer.OrdinalIgnoreCase);
		var result = NpcSpellPriority(npc.Identity[3])
			.Where(x => byName.TryGetValue(x.SourceName, out var index) && saved.ContainsKey(index))
			.Where(x => x.Condition != "source-blind-slot" || byName.TryGetValue("blind", out var blind) && saved.ContainsKey(blind))
			.Select(x => new NpcSavedSpell(byName[x.SourceName], Math.Clamp(saved[byName[x.SourceName]], 0, 100),
				NpcSpellGrade(saved[byName[x.SourceName]], 50), x)).ToList();
		if ((npc.Flags[0] & 1) == 0 && (npc.Identity[4] > 0 || npc.Clans.Length > 0) &&
			npc.Identity[3] is 1 or 5 or 9 or 10 or 11 or 12 or 14 or 15 or 16 &&
			byName.TryGetValue("heal", out var heal) && saved.TryGetValue(heal, out var healing) &&
			!result.Any(x => x.SourceIndex == heal) &&
			(!byName.TryGetValue("rescue", out var rescue) || !saved.ContainsKey(rescue)))
			result.Add(new(heal, Math.Clamp(healing, 0, 100), NpcSpellGrade(healing, 50), new("heal", "ally-only")));
		return result;
	}

	/// <summary>Source power tiers 0..6 become native grades 1..7; zero means insufficient Mana.</summary>
	internal static int NpcSpellGrade(int proficiency, double mana)
	{
		if (!double.IsFinite(mana) || mana < 10) return 0;
		var reduction = mana >= 50 ? 1 : mana >= 33 ? 2 : mana >= 20 ? 3 : 4;
		var learnedLevel = Math.Max(0, (Math.Clamp(proficiency, 0, 100) - 30) / 10);
		return Math.Clamp(learnedLevel - reduction, 0, 6) + 1;
	}

	internal static string[] NpcSavedCombatManoeuvres(ArmWorldNpc npc, IReadOnlyList<string> names)
	{
		if (NpcSpellPriority(npc.Identity[3]).Count > 0) return [];
		var saved = npc.Skills.Where(x => x.Length >= 2 && x[0] >= 0 && x[0] < names.Count)
			.Select(x => names[x[0]]).ToHashSet(StringComparer.OrdinalIgnoreCase);
		return new[] { "disarm", "kick", "bash" }.Where(saved.Contains).ToArray();
	}
}
