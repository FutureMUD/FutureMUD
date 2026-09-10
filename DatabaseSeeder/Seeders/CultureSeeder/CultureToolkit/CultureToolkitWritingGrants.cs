#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureWritingGrantResolution(long AppliesProgId, long KnowledgeProgId,
	IReadOnlyDictionary<string, long> ScriptKnowledgeIds, IReadOnlyList<long> SupportingProgIds);

/// <summary>Writing traditions are knowledge grants conditional on literacy and an actual selected language.</summary>
public static class CultureToolkitWritingGrants
{
	public static CultureWritingGrantResolution Upsert(FuturemudDatabaseContext context, CultureToolkitCatalogue catalogue,
		CultureToolkitPack pack, IReadOnlyDictionary<string, Culture> cultures, IReadOnlyDictionary<string, Ethnicity> ethnicities,
		IReadOnlyDictionary<string, IReadOnlyList<string>> natives, IReadOnlyDictionary<string, TraitDefinition> languages,
		IReadOnlyDictionary<string, Knowledge> scriptKnowledges, TraitDefinition literacy, ICollection<string> conflicts)
	{
		var policy = catalogue.Document("data.literacy_script_grants.json");
		var primary = policy.GetProperty("primary_script_by_language").EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString()!);
		var overrides = policy.GetProperty("community_overrides").EnumerateArray().ToDictionary(
			x => CultureToolkitCatalogue.Text(x, "ethnicity"), x => CultureToolkitCatalogue.Text(x, "script"));
		var memberships = catalogue.Document("data.script_policy.json").EnumerateArray().ToDictionary(
			x => CultureToolkitCatalogue.Text(x, "key"), x => CultureToolkitCatalogue.Strings(x.GetProperty("languages")).ToHashSet());
		var grants = new Dictionary<long, HashSet<string>>();
		void Add(string script, IEnumerable<string> candidates, string background)
		{
			var traits = candidates.Where(languages.ContainsKey).Select(x => languages[x].Id).Distinct().ToArray();
			if (traits.Length == 0) return;
			if (!scriptKnowledges.TryGetValue(script, out var knowledge))
				throw new InvalidOperationException($"Unresolved writing tradition {script} in {pack.Era}.");
			if (!grants.TryGetValue(knowledge.Id, out var conditions)) grants[knowledge.Id] = conditions = [];
			conditions.Add($"({background} and @ch.skills.Any(skill, {string.Join(" or ", traits.Select(x => $"@skill.id == {x}"))}))");
		}
		foreach (var row in policy.GetProperty("automatic_backgrounds").EnumerateArray())
		{
			if (!cultures.TryGetValue(CultureToolkitCatalogue.Text(row, "culture"), out var culture)) continue;
			var background = $"@ch.culture.id == {culture.Id}";
			foreach (var script in CultureToolkitCatalogue.Strings(row.GetProperty("fixed_script_keys")))
				Add(script, memberships[script], background);
			foreach (var ethnicity in ethnicities)
			{
				if (!natives.TryGetValue(ethnicity.Key, out var native))
					throw new InvalidOperationException($"Unresolved native writing tradition for {ethnicity.Key}.");
				foreach (var language in native)
				{
					var script = overrides.GetValueOrDefault(ethnicity.Key) ?? primary.GetValueOrDefault(language);
					if (script is null) continue; // Retained undocumented traditions are never inferred from script membership.
					Add(script, [language], $"({background} and @ch.ethnicity.id == {ethnicity.Value.Id})");
				}
			}
		}
		foreach (var group in pack.Groups)
		{
			var eligibility = group.GetProperty("eligibility");
			var conditions = eligibility.GetProperty("culture_packs").EnumerateObject()
				.Where(x => CultureToolkitCatalogue.Strings(x.Value).Contains(pack.Era)).Select(x => $"@ch.culture.id == {cultures[x.Name].Id}")
				.Concat(eligibility.GetProperty("ethnicity_packs").EnumerateObject()
					.Where(x => CultureToolkitCatalogue.Strings(x.Value).Contains(pack.Era)).Select(x => $"@ch.ethnicity.id == {ethnicities[x.Name].Id}"));
			var background = "(" + string.Join(" or ", conditions) + ")";
			var recipe = CultureToolkitCatalogue.Text(group.GetProperty("membership_recipe"), "source_key");
			var candidates = catalogue.Candidates(recipe, pack);
			foreach (var language in candidates.CanonicalKeys.Concat(candidates.RetainedSourceReferences).Where(primary.ContainsKey))
			{
				var communities = overrides.Where(x => ethnicities.ContainsKey(x.Key) &&
					natives.TryGetValue(x.Key, out var native) && native.Contains(language)).ToArray();
				foreach (var community in communities)
					Add(community.Value, [language], $"({background} and @ch.ethnicity.id == {ethnicities[community.Key].Id})");
				var exclusions = string.Concat(communities.Select(x => $" and @ch.ethnicity.id != {ethnicities[x.Key].Id}"));
				Add(primary[language], [language], $"({background}{exclusions})");
			}
		}
		var appliesBody = cultures.Values.Select(x => $"@ch.culture.id == {x.Id}")
			.Concat(ethnicities.Values.Select(x => $"@ch.ethnicity.id == {x.Id}")).Distinct().ToArray();
		var applies = CultureToolkitProgSeeder.Upsert(context, pack.Era, "writing.applies", "CultureWritingPolicyApplies",
			string.Concat(appliesBody.Chunk(8).Select(x => $"if ({string.Join(" or ", x)})\n  return true\nend if\n")) + "return false",
			ProgVariableTypes.Boolean, [(ProgVariableTypes.Chargen, "ch")], conflicts);
		var header = $"var knowledges as knowledge collection\nif (not(@ch.skills.Any(skill, @skill.id == {literacy.Id})))\n  return @knowledges\nend if\n";
		var blocks = new List<string>();
		foreach (var grant in grants.OrderBy(x => x.Key))
		foreach (var conditions in grant.Value.Order().Chunk(8))
			blocks.Add($"if (not(Contains(@knowledges, ToKnowledge({grant.Key}))) and ({string.Join(" or ", conditions)}))\n  additem knowledges ToKnowledge({grant.Key})\nend if\n");
		var partitioned = CultureToolkitProgPartitions.Upsert(context, pack.Era, "writing.knowledges", "CultureWritingKnowledges",
			header, blocks, "return @knowledges", ProgVariableTypes.Knowledge | ProgVariableTypes.Collection, [(ProgVariableTypes.Chargen, "ch")], conflicts, "knowledges");
		var prog = partitioned.Main;
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var id in partitioned.SupportingIds) compiler.Compile(id);
		foreach (var item in new[] { applies, prog }) compiler.Compile(item.Id);
		return new(applies.Id, prog.Id, scriptKnowledges.ToDictionary(x => x.Key, x => x.Value.Id), partitioned.SupportingIds);
	}
}
