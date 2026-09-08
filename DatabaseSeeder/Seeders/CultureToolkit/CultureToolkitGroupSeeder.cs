#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders.Utilities.Chargen;
using Microsoft.EntityFrameworkCore;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using DbProg = MudSharp.Models.FutureProg;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureGroupResolution(string StableKey, long GroupId, long EligibilityProgId,
	IReadOnlyList<long> CandidateTraitIds, IReadOnlyList<string> OmittedCandidates);

/// <summary>Only consumes resolved selected-pack identities; never resolves a legacy label globally.</summary>
public static class CultureToolkitGroupSeeder
{
	public static IReadOnlyList<CultureGroupResolution> Upsert(FuturemudDatabaseContext context,
		CultureToolkitCatalogue catalogue, CultureToolkitPack pack,
		IReadOnlyDictionary<string, Culture> cultures, IReadOnlyDictionary<string, Ethnicity> ethnicities,
		IReadOnlyDictionary<string, TraitDefinition> languages, ICollection<string> conflicts)
	{
		// Resolve the entire consumer plan before creating any progs or groups.
		var plans = pack.Groups.Select(group =>
		{
			var key = CultureToolkitCatalogue.Text(group, "key");
			var eligibility = group.GetProperty("eligibility");
			var clauses = new List<string>();
			foreach (var entry in eligibility.GetProperty("culture_packs").EnumerateObject()
				.Where(x => CultureToolkitCatalogue.Strings(x.Value).Contains(pack.Era)))
			{
				if (!cultures.TryGetValue(entry.Name, out var culture) || culture.Id <= 0 || context.Cultures.Find(culture.Id) is null)
					throw new InvalidOperationException($"Unresolved mandatory group culture: {entry.Name} ({pack.Era})");
				clauses.Add($"@ch.culture.id == {culture.Id}");
			}
			foreach (var entry in eligibility.GetProperty("ethnicity_packs").EnumerateObject()
				.Where(x => CultureToolkitCatalogue.Strings(x.Value).Contains(pack.Era)))
			{
				if (!ethnicities.TryGetValue(entry.Name, out var ethnicity) || ethnicity.Id <= 0 || context.Ethnicities.Find(ethnicity.Id) is null)
					throw new InvalidOperationException($"Unresolved mandatory group ethnicity: {entry.Name} ({pack.Era})");
				clauses.Add($"@ch.ethnicity.id == {ethnicity.Id}");
			}
			var recipe = CultureToolkitCatalogue.Text(group.GetProperty("membership_recipe"), "source_key");
			var candidates = catalogue.Candidates(recipe, pack);
			var members = new List<TraitDefinition>();
			var omitted = candidates.Omitted.ToList();
			foreach (var candidate in candidates.CanonicalKeys.Concat(candidates.RetainedSourceReferences))
			{
				if (!languages.TryGetValue(candidate, out var trait))
				{
					omitted.Add(candidate);
					continue;
				}
				if (trait.Id <= 0 || context.TraitDefinitions.Find(trait.Id) is null)
					throw new InvalidOperationException($"Group candidate is not installed: {candidate}");
				if (members.All(x => x.Id != trait.Id)) members.Add(trait);
			}
			return (Group: group, Key: key, Body: "return " + string.Join(" or ", clauses), Members: members, Omitted: omitted);
		}).ToArray();

		var progIds = plans.ToDictionary(x => x.Key, x => UpsertEligibilityProg(context, pack.Era, x.Key, x.Body, conflicts).Id);
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		// Validate every installed eligibility prog before activating any group.
		var compiled = progIds.ToDictionary(x => x.Key, x => compiler.Compile(x.Value));
		var results = new List<CultureGroupResolution>();
		foreach (var plan in plans)
		{
			var group = plan.Group;
			var definition = new SkillGroupSeedDefinition(plan.Key, CultureToolkitCatalogue.Text(group, "label"),
				CultureToolkitCatalogue.Text(group, "description"), group.GetProperty("minimum_picks").GetInt32(),
				group.GetProperty("maximum_picks").GetInt32(), group.GetProperty("display_order").GetInt32(),
				ExistingSkillPolicy.CountKnown, group.GetProperty("enabled_on_seed").GetBoolean(), plan.Members,
				compiled[plan.Key]);
			var installed = ChargenSkillSelectionGroupSeeder.Upsert(context, definition, conflicts);
			results.Add(new CultureGroupResolution(plan.Key, installed.Id, compiled[plan.Key].Id,
				installed.Members.OrderBy(x => x.DisplayOrder).Select(x => x.TraitDefinitionId).ToArray(), plan.Omitted));
		}
		return results;
	}

	private static DbProg UpsertEligibilityProg(FuturemudDatabaseContext context, string era, string groupKey,
		string body, ICollection<string> conflicts)
	{
		var key = $"{groupKey}.eligibility";
		var record = context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == "CultureSeeder" &&
			x.EntityType == "FutureProg" && x.StableKey == key);
		var isNew = record is null;
		var name = "CultureGroupEligibility" + string.Concat(groupKey.Split('.', '-').Select(x => char.ToUpperInvariant(x[0]) + x[1..]));
		DbProg prog;
		if (isNew)
		{
			if (context.FutureProgs.Any(x => x.FunctionName == name))
				throw new InvalidOperationException($"Unowned prog name collision: {name}");
			prog = new DbProg
			{
				FunctionName = name, FunctionText = body, FunctionComment = "Culture toolkit group eligibility; editable in game.",
				Category = "Chargen", Subcategory = "Culture Toolkit", ReturnType = (long)ProgVariableTypes.Boolean,
				AcceptsAnyParameters = false, Public = false, StaticType = 0
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Chargen
			});
			context.FutureProgs.Add(prog);
			context.SaveChanges();
			record = new SeederManagedRecord
			{
				Seeder = "CultureSeeder", Module = era, EntityType = "FutureProg", StableKey = key,
				LogicalId = prog.Id, ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow
			};
			context.SeederManagedRecords.Add(record);
		}
		else
		{
			prog = context.FutureProgs.Find(record!.LogicalId)
				?? throw new InvalidOperationException($"Managed eligibility prog is missing: {key}");
		}
		var current = new Dictionary<string, string> { ["name"] = prog.FunctionName, ["body"] = prog.FunctionText };
		var desired = new Dictionary<string, string> { ["name"] = name, ["body"] = body };
		var merged = SeederManagedRecordReconciler.Reconcile(record!, current, desired, isNew, conflicts);
		prog.FunctionName = merged["name"];
		prog.FunctionText = merged["body"];
		context.SaveChanges();
		return prog;
	}
}
