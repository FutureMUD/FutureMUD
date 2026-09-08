#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders.CultureToolkit;

/// <summary>Broad script membership and ordinary knowledge eligibility remain separate from free writing grants.</summary>
public static class CultureToolkitScriptSeeder
{
	public static IReadOnlyDictionary<string, Script> Upsert(FuturemudDatabaseContext context, CultureToolkitCatalogue catalogue,
		CultureToolkitPack pack, IReadOnlyDictionary<string, FuturemudDatabaseContext> stages,
		IReadOnlyDictionary<string, Language> languages, IReadOnlyDictionary<string, Script> preflightedBindings,
		ICollection<string> conflicts)
	{
		var alwaysTrue = context.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue");
		var policy = catalogue.Document("data.script_policy.json").EnumerateArray().ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"));
		var labels = policy.ToDictionary(x => CultureToolkitCatalogue.Text(x.Value, "label"), x => x.Key, StringComparer.OrdinalIgnoreCase);
		var sources = stages.SelectMany(stage => stage.Value.Scripts.Include(x => x.Knowledge).ThenInclude(x => x.CanAcquireProg)
			.Include(x => x.ScriptsDesignedLanguages).ThenInclude(x => x.Language).AsEnumerable()
			.Select(x => (Module: stage.Key, Script: x, Key: labels.GetValueOrDefault(x.Name) ?? $"source.{stage.Key}.script.{x.Name}")))
			.Where(x => x.Script.ScriptsDesignedLanguages.Any(y => languages.ContainsKey(CultureToolkitLanguageBindings.Key(x.Module, y.Language.Name))))
			.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.ToArray());
		var keys = policy.Where(x => CultureToolkitCatalogue.Strings(x.Value.GetProperty("languages")).Any(languages.ContainsKey))
			.Select(x => x.Key).Union(sources.Keys).ToArray();
		// Reject unresolved labels before any script, knowledge or acquisition prog is written.
		foreach (var key in keys)
		{
			var label = policy.TryGetValue(key, out var row) ? CultureToolkitCatalogue.Text(row, "label") : sources[key][0].Script.Name;
			if (CultureToolkitManagedEntities.Find(context, "Script", key) is null && !preflightedBindings.ContainsKey(key) && context.Scripts.Any(x => x.Name == label))
				throw new InvalidOperationException($"Script {key} ({label}) requires an explicit source binding.");
		}
		var writer = new CultureToolkitEntityWriter(context, pack.Era, conflicts);
		var result = new Dictionary<string, Script>();
		var generated = new List<long>();
		foreach (var key in keys)
		{
			sources.TryGetValue(key, out var rows);
			var source = rows?.FirstOrDefault().Script;
			var hasPolicy = policy.TryGetValue(key, out var row);
			var label = hasPolicy ? CultureToolkitCatalogue.Text(row, "label") : source!.Name;
			var record = CultureToolkitManagedEntities.Find(context, "Script", key);
			var bound = record is null ? preflightedBindings.GetValueOrDefault(key) : context.Scripts.Include(x => x.Knowledge)
				.ThenInclude(x => x.CanAcquireProg).Single(x => x.Id == record.LogicalId);
			var sourceMembers = (rows ?? []).SelectMany(x => x.Script.ScriptsDesignedLanguages.Select(y => CultureToolkitLanguageBindings.Key(x.Module, y.Language.Name)))
				.Where(languages.ContainsKey).Distinct().ToArray();
			var baselineRows = bound is null ? [] : (rows ?? []).Where(x =>
				x.Script.KnownScriptDescription == bound.KnownScriptDescription && x.Script.UnknownScriptDescription == bound.UnknownScriptDescription &&
				x.Script.Knowledge.CanAcquireProg?.FunctionText == bound.Knowledge.CanAcquireProg?.FunctionText).ToArray();
			var baselineMembers = baselineRows.Length == 0 ? sourceMembers : baselineRows
				.SelectMany(x => x.Script.ScriptsDesignedLanguages.Select(y => CultureToolkitLanguageBindings.Key(x.Module, y.Language.Name))).Distinct().ToArray();
			var members = sourceMembers.Concat(hasPolicy ? CultureToolkitCatalogue.Strings(row.GetProperty("languages")).Where(languages.ContainsKey) : [])
				.Distinct().ToArray();
			var allLanguageIds = members.Select(x => languages[x].Id).Concat(bound is null ? [] : context.ScriptsDesignedLanguages
				.Where(x => x.ScriptId == bound.Id).Select(x => x.LanguageId)).Distinct().ToList();
			var traitIds = context.Languages.Where(x => allLanguageIds.Contains(x.Id)).Select(x => x.LinkedTraitId).Distinct().ToArray();
			var body = string.Concat(traitIds.Order().Chunk(8).Select(ids => $"if ({string.Join(" or ", ids.Select(id => $"@skill.id == {id}"))})\n  return true\nend if\n")) + "return false";
			var suffix = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)))[..16];
			var acquire = CultureToolkitProgSeeder.Upsert(context, pack.Era, key + ".script-acquisition", $"CultureScript{suffix}CanPick", body,
				ProgVariableTypes.Boolean, [(ProgVariableTypes.Chargen, "ch"), (ProgVariableTypes.Trait, "skill")], conflicts);
			generated.Add(acquire.Id);
			var knowledge = source is null ? new Knowledge { Name = $"{label} Script", Type = "Script", Subtype = "Writing",
				LearnableType = (int)(MudSharp.RPG.Knowledge.LearnableType.LearnableAtChargen | MudSharp.RPG.Knowledge.LearnableType.LearnableFromTeacher),
				LearnDifficulty = 7, TeachDifficulty = 7, LearningSessionsRequired = 10 } : CultureToolkitEntityWriter.CopyScalars(context, source.Knowledge);
			knowledge.CanAcquireProgId = acquire.Id;
			knowledge.CanLearnProgId = alwaysTrue.Id;
			if (hasPolicy) knowledge.Description = knowledge.LongDescription = CultureToolkitCatalogue.Text(row, "description");
			var originalKnowledge = source is null ? null : CultureToolkitEntityWriter.CopyScalars(context, source.Knowledge);
			if (originalKnowledge is not null)
			{
				originalKnowledge.CanLearnProgId = alwaysTrue.Id;
				// A custom acquisition prog must not be blessed as the old source baseline.
				originalKnowledge.CanAcquireProgId = bound is not null && rows!.Any(x => bound.Knowledge.CanAcquireProg?.FunctionText == x.Script.Knowledge.CanAcquireProg?.FunctionText)
					? bound?.Knowledge.CanAcquireProgId : null;
				if (bound is not null && rows!.Any(x => x.Script.Knowledge.LongDescription == bound.Knowledge.LongDescription))
					originalKnowledge.LongDescription = bound.Knowledge.LongDescription;
				if (bound is not null && rows!.Any(x => x.Script.Knowledge.Subtype == bound.Knowledge.Subtype))
					originalKnowledge.Subtype = bound.Knowledge.Subtype;
			}
			var installedKnowledge = writer.Upsert(key + ".script-knowledge", knowledge, bound?.Knowledge, originalKnowledge);
			var script = source is null ? new Script { DocumentLengthModifier = 1.0, InkUseModifier = 1.0 } : CultureToolkitEntityWriter.CopyScalars(context, source);
			script.Name = label;
			script.KnowledgeId = installedKnowledge.Id;
			if (hasPolicy)
			{
				script.KnownScriptDescription = CultureToolkitCatalogue.Text(row, "known_description");
				script.UnknownScriptDescription = CultureToolkitCatalogue.Text(row, "unknown_description");
			}
			var originalScript = source is null ? null : CultureToolkitEntityWriter.CopyScalars(context, source);
			if (originalScript is not null)
			{
				originalScript.KnowledgeId = bound?.KnowledgeId ?? installedKnowledge.Id;
				if (bound is not null && rows!.Any(x => x.Script.UnknownScriptDescription == bound.UnknownScriptDescription))
					originalScript.UnknownScriptDescription = bound.UnknownScriptDescription;
			}
			var installed = writer.Upsert(key, script, bound, originalScript);
			result[key] = installed;
			foreach (var member in members)
			{
				var link = context.ScriptsDesignedLanguages.Find(installed.Id, languages[member].Id);
				var memberKey = key + ".language." + member;
				var linkRecord = CultureToolkitManagedEntities.Find(context, "ScriptLanguage", memberKey);
				var fresh = linkRecord is null && (bound is null || !baselineMembers.Contains(member));
				var merged = CultureToolkitManagedEntities.Reconcile(context, pack.Era, "ScriptLanguage", memberKey, installed.Id, fresh,
					new Dictionary<string, string> { ["present"] = link is null ? "false" : "true" },
					new Dictionary<string, string> { ["present"] = "true" }, conflicts);
				if (merged["present"] == "true" && link is null) context.ScriptsDesignedLanguages.Add(new ScriptsDesignedLanguage { ScriptId = installed.Id, LanguageId = languages[member].Id });
			}
			context.SaveChanges();
		}
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var id in generated) compiler.Compile(id);
		return result;
	}
}
