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
using CultureInfo = System.Globalization.CultureInfo;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureStartingProgResolution(long NativeBaseProgId, long BackgroundFactorProgId,
	long FixedSkillsProgId, IReadOnlyDictionary<long, long> OriginalToWrapperProgIds, IReadOnlyList<long> SupportingProgIds);

/// <summary>Compiles the existing starting-value hook; no proficiency state or skill ownership is introduced.</summary>
public static class CultureToolkitStartingProgs
{
	public static CultureStartingProgResolution Upsert(FuturemudDatabaseContext context, CultureToolkitCatalogue catalogue,
		CultureToolkitPack pack, IReadOnlyDictionary<string, Culture> cultures, IReadOnlyDictionary<string, Ethnicity> ethnicities,
		IReadOnlyDictionary<string, TraitDefinition> languages, TraitDefinition? literacy, ICollection<string> conflicts,
		IReadOnlyDictionary<string, IReadOnlyList<string>>? retainedNativeBindings = null)
	{
		var factors = new List<(long Trait, double Factor, string Condition)>();
		var mandatory = new Dictionary<long, HashSet<string>>();
		void Add(string key, double factor, string condition, bool grant)
		{
			if (!languages.TryGetValue(key, out var trait))
			{
				if (grant) throw new InvalidOperationException($"Unresolved mandatory native/education language: {key} ({pack.Era}).");
				return;
			}
			factors.Add((trait.Id, factor, condition));
			if (!grant) return;
			if (!mandatory.TryGetValue(trait.Id, out var conditions)) mandatory[trait.Id] = conditions = new HashSet<string>();
			conditions.Add(condition);
		}
		// Ethnicity and culture are explicit installed bindings, never inferred from a trait label.
		foreach (var ethnicity in ethnicities)
		{
			if (retainedNativeBindings?.TryGetValue(ethnicity.Key, out var resolvedNative) == true)
			{
				foreach (var language in resolvedNative) Add(language, 1.0, $"@ch.ethnicity.id == {ethnicity.Value.Id}", true);
				continue;
			}
			var row = catalogue.Document("data.ethnicity_language_defaults.json").GetProperty("defaults").EnumerateArray()
				.SingleOrDefault(x => CultureToolkitCatalogue.Text(x, "ethnicity") == ethnicity.Key);
			if (row.ValueKind == System.Text.Json.JsonValueKind.Undefined || !row.GetProperty("by_era").TryGetProperty(pack.Era, out var native))
				throw new InvalidOperationException($"Retained ethnicity requires its exact source native binding: {ethnicity.Key}");
			foreach (var language in CultureToolkitCatalogue.Strings(native)) Add(language, 1.0, $"@ch.ethnicity.id == {ethnicity.Value.Id}", true);
		}
		var tierFactors = new Dictionary<string, double>
		{
			["native"] = 1.0, ["fluent"] = .9, ["educated"] = .75, ["conversational"] = .5, ["elementary"] = .25
		};
		foreach (var row in pack.Cultures)
		{
			var key = CultureToolkitCatalogue.Text(row, "key");
			if (!cultures.TryGetValue(key, out var culture)) throw new InvalidOperationException($"Unresolved culture {key}");
			var condition = $"@ch.culture.id == {culture.Id}";
			foreach (var language in catalogue.ResolveSelector(CultureToolkitCatalogue.Text(row, "vernacular_selector"), pack.Era, []))
				Add(language, tierFactors[CultureToolkitCatalogue.Text(row, "cultural_vernacular_proficiency")], condition, true);
			foreach (var grant in row.GetProperty("additional_language_grants").EnumerateArray()
				.Where(x => CultureToolkitCatalogue.Text(x, "condition") is "all-listed-packs" || CultureToolkitCatalogue.Text(x, "condition") == pack.Era))
				Add(CultureToolkitCatalogue.Text(grant, "language"), tierFactors[CultureToolkitCatalogue.Text(grant, "proficiency")], condition, true);
		}
		foreach (var group in pack.Groups)
		{
			var eligibility = group.GetProperty("eligibility");
			var conditions = eligibility.GetProperty("culture_packs").EnumerateObject()
				.Where(x => CultureToolkitCatalogue.Strings(x.Value).Contains(pack.Era)).Select(x => $"@ch.culture.id == {cultures[x.Name].Id}")
				.Concat(eligibility.GetProperty("ethnicity_packs").EnumerateObject()
					.Where(x => CultureToolkitCatalogue.Strings(x.Value).Contains(pack.Era)).Select(x => $"@ch.ethnicity.id == {ethnicities[x.Name].Id}"));
			var condition = "(" + string.Join(" or ", conditions) + ")";
			var recipeKey = CultureToolkitCatalogue.Text(group.GetProperty("membership_recipe"), "source_key");
			var candidates = catalogue.Candidates(recipeKey, pack);
			var factor = tierFactors[CultureToolkitCatalogue.Text(catalogue.Document("data.language_choice_groups.json").GetProperty(recipeKey), "proficiency")];
			foreach (var language in candidates.CanonicalKeys.Concat(candidates.RetainedSourceReferences)) Add(language, factor, condition, false);
		}
		foreach (var row in catalogue.Document("data.literacy_script_grants.json").GetProperty("automatic_backgrounds").EnumerateArray())
		{
			if (!cultures.TryGetValue(CultureToolkitCatalogue.Text(row, "culture"), out var culture)) continue;
			if (literacy is null) throw new InvalidOperationException("Learned backgrounds require the installed Literacy trait.");
			if (!mandatory.TryGetValue(literacy.Id, out var conditions)) mandatory[literacy.Id] = conditions = new HashSet<string>();
			conditions.Add($"@ch.culture.id == {culture.Id}");
		}
		var factorBlocks = new List<string>();
		foreach (var tier in factors.GroupBy(x => x.Factor).OrderByDescending(x => x.Key))
		{
			var clauses = tier.GroupBy(x => x.Trait).Select(x => $"(@trait.id == {x.Key} and ({string.Join(" or ", x.Select(y => y.Condition).Distinct())}))");
			foreach (var chunk in clauses.Chunk(8)) factorBlocks.Add($"if ({string.Join(" or ", chunk)})\n  return {tier.Key.ToString(CultureInfo.InvariantCulture)}\nend if\n");
		}
		var fixedBlocks = new List<string>();
		foreach (var entry in mandatory.OrderBy(x => x.Key))
		foreach (var chunk in entry.Value.Chunk(8))
			fixedBlocks.Add($"if (not(Contains(@skills, ToTrait({entry.Key}))) and ({string.Join(" or ", chunk)}))\n  additem skills ToTrait({entry.Key})\nend if\n");
		var nativeBase = CultureToolkitProgSeeder.Upsert(context, pack.Era, "language.native-base", "CultureLanguageNativeBase",
			"return 200", ProgVariableTypes.Number, [], conflicts);
		var backgroundParts = CultureToolkitProgPartitions.Upsert(context, pack.Era, "language.background-factor", "CultureLanguageBackgroundFactor",
			"", factorBlocks, "return 0", ProgVariableTypes.Number, [(ProgVariableTypes.Toon, "ch"), (ProgVariableTypes.Trait, "trait")], conflicts);
		var fixedParts = CultureToolkitProgPartitions.Upsert(context, pack.Era, "language.fixed-skills", "CultureFixedSkills",
			"var skills as trait collection\n", fixedBlocks, "return @skills", ProgVariableTypes.Trait | ProgVariableTypes.Collection, [(ProgVariableTypes.Toon, "ch")], conflicts, "skills");
		var background = backgroundParts.Main;
		var fixedSkills = fixedParts.Main;
		var supporting = backgroundParts.SupportingIds.Concat(fixedParts.SupportingIds).ToArray();

		var wrappers = new Dictionary<long, long>();
		var rewire = new List<(Culture Culture, long Wrapper)>();
		foreach (var culture in cultures)
		{
			var record = CultureToolkitManagedEntities.Find(context, "OriginalStartingProg", culture.Key);
			var originalId = record?.LogicalId ?? culture.Value.SkillStartingValueProgId;
			var original = context.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == originalId);
			CultureToolkitProgSeeder.Validate(original, ProgVariableTypes.Number, ProgVariableTypes.Toon, ProgVariableTypes.Trait, ProgVariableTypes.Number);
			if (context.SeederManagedRecords.Any(x => x.Seeder == "CultureSeeder" && x.EntityType == "FutureProg" &&
				x.StableKey.StartsWith("language.starting-wrapper.") && x.LogicalId == original.Id))
				throw new InvalidOperationException("Refusing to wrap a starting-value wrapper recursively.");
			if (!wrappers.TryGetValue(original.Id, out var wrapperId))
			{
				var body = $"var factor as number\nfactor = @{background.FunctionName}(@ch, @trait)\nif (@factor <= 0)\n  return @{original.FunctionName}(@ch, @trait, @boosts)\nend if\nreturn (@{nativeBase.FunctionName}() * @factor) + (@{original.FunctionName}(@ch, @trait, @boosts) - @{original.FunctionName}(@ch, @trait, 0))";
				var wrapper = CultureToolkitProgSeeder.Upsert(context, pack.Era, $"language.starting-wrapper.{original.Id}",
					$"CultureLanguageStartingValue{original.Id}", body, ProgVariableTypes.Number,
					[(ProgVariableTypes.Toon, "ch"), (ProgVariableTypes.Trait, "trait"), (ProgVariableTypes.Number, "boosts")], conflicts);
				wrappers[original.Id] = wrapperId = wrapper.Id;
			}
			if (record is null)
			{
				context.SeederManagedRecords.Add(new SeederManagedRecord
				{
					Seeder = "CultureSeeder", Module = pack.Era, EntityType = "OriginalStartingProg", StableKey = culture.Key,
					LogicalId = originalId, ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow
				});
				rewire.Add((culture.Value, wrapperId));
			}
			else if (culture.Value.SkillStartingValueProgId != wrapperId)
				conflicts.Add($"{culture.Key}: preserved builder starting-prog reference {culture.Value.SkillStartingValueProgId}; toolkit wrapper is {wrapperId}.");
		}
		context.SaveChanges();
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var id in supporting.Concat(new[] { nativeBase.Id, background.Id, fixedSkills.Id }).Concat(wrappers.Keys).Concat(wrappers.Values)) compiler.Compile(id);
		foreach (var entry in rewire) entry.Culture.SkillStartingValueProgId = entry.Wrapper;
		context.SaveChanges();
		CultureToolkitFreeSkills.Reconcile(context, conflicts);
		return new CultureStartingProgResolution(nativeBase.Id, background.Id, fixedSkills.Id, wrappers, supporting);
	}
}
