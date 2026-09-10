#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureToolkitInstallReport(string Era, IReadOnlyList<string> Conflicts,
	IReadOnlyDictionary<string, long> CultureIds, IReadOnlyDictionary<string, long> EthnicityIds,
	IReadOnlyDictionary<string, long> LanguageIds, IReadOnlyList<CultureNativeBinding> NativeBindings,
	IReadOnlyList<CultureNameResolution> TargetedNames, CultureStartingProgResolution? StartingProgs,
	CultureWritingGrantResolution? WritingProgs, IReadOnlyList<CultureResolvedLanguageEdge> Intelligibility,
	IReadOnlyList<CultureGroupResolution> Groups, IReadOnlyList<string> Caps, IReadOnlyList<CultureAccentResolution> Accents);

/// <summary>One selected toolkit. Prerequisite/source binding is read-only; caller controls the final transaction.</summary>
public static class CultureToolkitInstaller
{
	public static CultureToolkitInstallReport Install(FuturemudDatabaseContext context, string era,
		bool seedNames, bool seedLanguages, bool seedHeritage, Action<string>? progress = null)
	{
		if (context.Database.IsRelational() && context.Database.CurrentTransaction is null)
			throw new InvalidOperationException("Culture toolkit installation requires a caller-owned transaction with rollback on validation failure.");
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose(era);
		if (!seedNames && !seedLanguages && !seedHeritage)
			return new(era, [], new Dictionary<string, long>(), new Dictionary<string, long>(), new Dictionary<string, long>(), [], [], null, null, [], [], [], []);
		using var ownershipLookup = CultureToolkitManagedEntities.BeginLookupScope(context);
		var literacy = seedLanguages && seedHeritage
			? context.TraitDefinitions.SingleOrDefault(x => x.Name == "Literacy" && (x.Type == 0 || x.Type == 2)) : null;
		if (seedLanguages && seedHeritage && literacy is null)
			throw new InvalidOperationException("Historical learned backgrounds require the installed Literacy skill. Install the stock skill package before this toolkit.");
		var legacyWritingBlock = seedLanguages && seedHeritage ? ChargenFreeKnowledgeProgReconciler.CaptureVerifiedCultureBlock(context) : null;
		var installedPack = CultureToolkitManagedEntities.Find(context, "ToolkitInstallation", "selected-era");
		if (installedPack is not null && installedPack.Module != era)
			throw new InvalidOperationException($"This world has toolkit {installedPack.Module}; live era switching is outside this installer.");
		var stages = new Dictionary<string, FuturemudDatabaseContext>();
		try
		{
			foreach (var module in new[] { "earthantiquity", "earthdarkagesandmedieval", "earthrenaissanceeurope", "earthrenaissanceworldexpansion" })
			{
				progress?.Invoke($"Building retained source: {module}...");
				stages[module] = CultureSeeder.BuildToolkitSource(context, module);
			}
			progress?.Invoke("Retained source stages built.");
			var genericHuman = seedHeritage ? context.Ethnicities.Include(x => x.EthnicitiesCharacteristics).SingleOrDefault(x => x.Name == "Admin" && x.ParentRace.Name == "Human")
				?? throw new InvalidOperationException("The installed Human Admin ethnicity is required for explicit generic Human defaults; no unrelated phenotype will be substituted.") : null;
			var heritage = seedHeritage ? CultureToolkitHeritageSources.Describe(pack, stages, genericHuman!) : [];
			var naming = CultureToolkitSourceNames.Describe(stages);
			if (!seedNames)
			{
				var needed = heritage.SelectMany(x => x.NamingStructureOverride is not null ? [x.NamingStructureOverride] :
					x.Template.EthnicitiesNameCultures.Select(y => y.NameCulture.Name))
					.Concat(seedHeritage ? pack.Cultures.Select(x => CultureToolkitCatalogue.Text(x, "naming_fallback")) : []).ToHashSet();
				naming = naming with { Cultures = naming.Cultures.Where(x => needed.Contains(x.Definition.Name)).ToArray(), Profiles = [] };
			}
			var issues = new List<string>();
			var existing = CultureToolkitBindingPreflight.Resolve(context, catalogue, naming,
				seedLanguages ? stages : new Dictionary<string, FuturemudDatabaseContext>(), heritage, issues, pack);
			if (issues.Count > 0) throw new InvalidOperationException("Culture toolkit source binding must be resolved before writes:\n" + string.Join("\n", issues));
			var alwaysTrue = context.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue");
			var alwaysFalse = context.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse");
			var calendar = context.Calendars.First();
			var sourceStarting = stages.Values.First().FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.FunctionName == "SkillStartingValue");
			var originalStarting = context.FutureProgs.Include(x => x.FutureProgsParameters).SingleOrDefault(x => x.FunctionName == "SkillStartingValue");
			if (originalStarting is not null) CultureToolkitProgSeeder.Validate(originalStarting, ProgVariableTypes.Number, ProgVariableTypes.Toon, ProgVariableTypes.Trait, ProgVariableTypes.Number);
			var sourceLanguages = stages.SelectMany(stage => stage.Value.Languages.AsEnumerable().Select(language =>
				(Key: CultureToolkitLanguageBindings.Key(stage.Key, language.Name), Language: language))).ToArray();
			var semanticLanguages = pack.Languages.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"), _ => new Language());
			foreach (var item in sourceLanguages) semanticLanguages.TryAdd(item.Key, item.Language);
			var referenceKeys = semanticLanguages.Keys.ToDictionary(x => x, x => x);
			foreach (var alias in sourceLanguages.GroupBy(x => x.Language.Name))
			{
				var keys = alias.Select(x => x.Key).Distinct().ToArray();
				if (keys.Length != 1) continue;
				semanticLanguages["legacy:" + alias.Key] = semanticLanguages[keys[0]];
				referenceKeys["legacy:" + alias.Key] = keys[0];
			}
			CultureNativeBinding Native(CultureHeritageSource source, IReadOnlyDictionary<string, Language> languages) => source.Overlay.HasValue
				? CultureToolkitNativeBindings.Canonical(catalogue, era, source.Key, languages)
				: CultureToolkitNativeBindings.Source(catalogue, era, source.Module, source.Template, languages);
			var semanticBindings = heritage.ToDictionary(x => x.Key, x => Native(x, semanticLanguages));
			var unresolvedOverlays = heritage.Where(x => x.Overlay.HasValue && !semanticBindings[x.Key].HasResolvedReferences).ToArray();
			if (seedLanguages && unresolvedOverlays.Length > 0)
				throw new InvalidOperationException(string.Join("\n", unresolvedOverlays.SelectMany(x => semanticBindings[x.Key].Unresolved)));
			// All content writes occur after source binding. The caller must roll back any later validation failure.
			var conflicts = new List<string>();
			CultureToolkitKnowledgeIntegration.PreserveLegacyBlock(context, era, legacyWritingBlock);
			var suggestions = context.FutureProgs.Where(x => x.FunctionName == "AlwaysTrue" || x.FunctionName == "AlwaysFalse").ToDictionary(x => x.FunctionName);
			// Bind structures first; defer the large profile corpus until scalar/FK writes are
			// complete so EF does not rescan tens of thousands of unchanged name elements for each accent.
			var installedNames = CultureToolkitSourceNames.Upsert(context, era, naming, existing.NameCultures, existing.Profiles, suggestions, false, conflicts);
			progress?.Invoke("Source naming structures reconciled.");
			if (seedHeritage && originalStarting is null)
				originalStarting = CultureToolkitProgSeeder.Upsert(context, era, "language.original-default", "SkillStartingValue", sourceStarting.FunctionText,
					ProgVariableTypes.Number, sourceStarting.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => ((ProgVariableTypes)x.ParameterType, x.ParameterName)).ToArray(), conflicts);
			CultureLanguageInstallResult? installedLanguages = null;
			IReadOnlyDictionary<string, Script> scripts = new Dictionary<string, Script>();
			IReadOnlyList<CultureResolvedLanguageEdge> edges = [];
			if (seedLanguages)
			{
				var sourceLanguage = stages.Values.First().Languages.Include(x => x.LinkedTrait).First();
				var intelligence = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3).AsEnumerable()
					.First(x => new[] { "Intelligence", "Intellect", "Wisdom", "Mind" }.Contains(x.Name));
				var prerequisites = new CultureLanguagePrerequisites(intelligence, context.TraitDecorators.Find(sourceLanguage.LinkedTrait.DecoratorId)!,
					context.Improvers.Find(sourceLanguage.LinkedTrait.ImproverId)!, context.LanguageDifficultyModels.Find(sourceLanguage.DifficultyModel)!, alwaysTrue, alwaysFalse);
				var activeLegacy = semanticBindings.Values.Where(x => x.HasResolvedReferences).SelectMany(x => x.References)
					.Concat(pack.Groups.SelectMany(x => catalogue.Candidates(CultureToolkitCatalogue.Text(x.GetProperty("membership_recipe"), "source_key"), pack).RetainedSourceReferences))
					.Where(referenceKeys.ContainsKey).Select(x => referenceKeys[x]).ToHashSet();
				var beforeIds = context.Languages.Select(x => x.Id).ToHashSet();
				installedLanguages = CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, stages, existing.Languages, prerequisites, conflicts, activeLegacy, progress);
				scripts = CultureToolkitScriptSeeder.Upsert(context, catalogue, pack, stages, installedLanguages.Languages, existing.Scripts, conflicts);
				var sourceEdges = stages.SelectMany(stage => stage.Value.MutualIntelligabilities.AsEnumerable().Select(edge =>
					new CultureSourceLanguageEdge(CultureToolkitLanguageBindings.Key(stage.Key, stage.Value.Languages.Find(edge.ListenerLanguageId)!.Name),
						CultureToolkitLanguageBindings.Key(stage.Key, stage.Value.Languages.Find(edge.TargetLanguageId)!.Name), edge.IntelligabilityDifficulty,
						$"source.{stage.Key}.intelligibility.{edge.ListenerLanguageId}.{edge.TargetLanguageId}"))).ToArray();
				edges = CultureToolkitIntelligibility.Reconcile(context, catalogue, pack, installedLanguages.Languages, sourceEdges, conflicts,
					installedLanguages.Languages.Values.Select(x => x.Id).Where(x => !beforeIds.Contains(x)).ToHashSet());
				progress?.Invoke("Languages, scripts and directed intelligibility reconciled.");
			}
			IReadOnlyDictionary<string, Culture> cultures = new Dictionary<string, Culture>();
			CultureEthnicityResolution? ethnicities = null;
			if (seedHeritage)
			{
				var definitions = new List<CultureEthnicityDefinition>();
				foreach (var item in heritage)
				{
					var native = Native(item, installedLanguages?.Languages ?? semanticLanguages);
					var desired = CultureToolkitEntityWriter.CopyScalars(context, item.Template);
					desired.ChargenBlurb = CultureToolkitProse.Display(CultureToolkitProse.Rewrite(item.Module, "Ethnicity", item.Template.Name, "ChargenBlurb", item.Template.ChargenBlurb));
					if (CultureToolkitNativeBindings.ExactSource(catalogue, era, item.Module, item.Template.Name) is JsonElement exact)
					{
						if (exact.GetProperty("description_override").ValueKind == JsonValueKind.String)
							desired.ChargenBlurb = CultureToolkitCatalogue.Text(exact, "description_override");
						if (exact.GetProperty("group_override").ValueKind == JsonValueKind.String)
							desired.EthnicGroup = CultureToolkitCatalogue.Text(exact, "group_override");
					}
					if (item.Overlay is JsonElement overlay)
					{
						desired.Name = CultureToolkitCatalogue.Text(overlay, "label");
						desired.ChargenBlurb = CultureToolkitCatalogue.Text(overlay, "replacement_description");
						desired.EthnicGroup = CultureToolkitCatalogue.Text(overlay, "group");
						desired.AvailabilityProgId = alwaysTrue.Id;
						if (item.Module == "human-prerequisite") desired.EthnicSubgroup = desired.Name;
					}
					if (!native.HasResolvedReferences) desired.AvailabilityProgId = alwaysFalse.Id;
					if (!seedLanguages) native = native with { LanguageIds = [], Unresolved = native.Unresolved.Append("Optional language installation skipped; fixed grants were not wired by this run.").ToArray() };
					desired.NativeLanguageId = native.LanguageIds.Select(x => (long?)x).FirstOrDefault();
					var links = item.NamingStructureOverride is not null
						? Enum.GetValues<Gender>().ToDictionary(x => (short)x, _ => installedNames.Cultures[naming.Cultures.Single(x => x.Definition.Name == item.NamingStructureOverride).Key].Id)
						: item.Template.EthnicitiesNameCultures.ToDictionary(x => x.Gender, x => installedNames.SourceCultures[(item.Module, x.NameCultureId)].Id);
					definitions.Add(new(item.Key, item.Template, desired, links, native));
				}
				ethnicities = CultureToolkitEthnicities.Upsert(context, era, definitions, existing.Ethnicities, conflicts);
				var fallbacks = pack.Cultures.Select(x => CultureToolkitCatalogue.Text(x, "naming_fallback")).Distinct().ToDictionary(x => x,
					x => installedNames.Cultures[naming.Cultures.Single(y => y.Definition.Name == x).Key]);
				cultures = CultureToolkitSocialCultures.Upsert(context, pack, fallbacks, calendar, originalStarting!, alwaysTrue, conflicts, catalogue, installedLanguages?.Languages);
			}
			progress?.Invoke("Heritage reconciled.");
			CultureStartingProgResolution? starting = null;
			CultureWritingGrantResolution? writing = null;
			IReadOnlyList<CultureGroupResolution> groups = [];
			IReadOnlyList<CultureAccentResolution> accents = [];
			if (installedLanguages is not null && ethnicities is not null)
			{
				var active = heritage.Where(x => ethnicities.NativeBindings.Single(y => y.SourceIdentity == (x.Overlay.HasValue ? x.Key : $"source.{x.Module}.ethnicity.{x.Template.Name}")).IsResolved)
					.ToDictionary(x => x.Key, x => ethnicities.Ethnicities[x.Key]);
				var natives = heritage.Where(x => active.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => Native(x, installedLanguages.Languages).References);
				var nativeByLanguage = installedLanguages.Languages.Values.Select(x => x.Id).Distinct()
					.ToDictionary(x => x, _ => (IReadOnlyList<long>)Array.Empty<long>());
				foreach (var group in active.SelectMany(x => natives[x.Key]
					.Select(reference => (LanguageId: installedLanguages.Languages[reference].Id, EthnicityId: x.Value.Id))).GroupBy(x => x.LanguageId))
					nativeByLanguage[group.Key] = group.Select(x => x.EthnicityId).Distinct().ToArray();
				accents = CultureToolkitAccents.Upsert(context, era, nativeByLanguage, conflicts, catalogue, stages, installedLanguages.Languages, progress);
				starting = CultureToolkitStartingProgs.Upsert(context, catalogue, pack, cultures, active, installedLanguages.Traits, literacy, conflicts, natives);
				progress?.Invoke("Native accents and starting-value progs compiled.");
				groups = CultureToolkitGroupSeeder.Upsert(context, catalogue, pack, cultures, active, installedLanguages.Traits, conflicts);
				if (literacy is not null) writing = CultureToolkitWritingGrants.Upsert(context, catalogue, pack, cultures, active, natives,
					installedLanguages.Traits, scripts.ToDictionary(x => x.Key, x => context.Knowledges.Find(x.Value.KnowledgeId)!), literacy, conflicts);
				var knowledgeResult = ChargenFreeKnowledgeProgReconciler.Reconcile(context);
				if (knowledgeResult.Status == ChargenFreeKnowledgeProgReconcileStatus.Unsafe || knowledgeResult.Message.Contains(" Skipped "))
					conflicts.Add(knowledgeResult.Message);
			}
			if (installedLanguages is not null && ethnicities is null)
				accents = CultureToolkitAccents.Upsert(context, era, new Dictionary<long, IReadOnlyList<long>>(), conflicts, catalogue, stages, installedLanguages.Languages, progress);
			progress?.Invoke("Reconciling targeted and retained naming profiles...");
			var targeted = seedNames ? CultureToolkitNameSeeder.Upsert(context, catalogue, era, ethnicities?.Ethnicities ?? new Dictionary<string, Ethnicity>(),
				alwaysTrue, conflicts, ethnicities?.OriginalNameCultures) : [];
			if (seedNames) CultureToolkitSourceNames.Upsert(context, era, naming, existing.NameCultures, existing.Profiles, suggestions, true, conflicts);
			progress?.Invoke("Retained naming profiles reconciled.");
			if (installedPack is null) context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = "CultureSeeder", EntityType = "ToolkitInstallation", StableKey = "selected-era", Module = era,
				ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow
			});
			context.SaveChanges();
			return new(era, conflicts.Distinct().ToArray(), cultures.ToDictionary(x => x.Key, x => x.Value.Id),
				ethnicities?.Ethnicities.ToDictionary(x => x.Key, x => x.Value.Id) ?? new Dictionary<string, long>(),
				installedLanguages?.Languages.ToDictionary(x => x.Key, x => x.Value.Id) ?? new Dictionary<string, long>(),
				ethnicities?.NativeBindings ?? [], targeted, starting, writing, edges, groups, installedLanguages?.CapReports ?? [], accents);
		}
		finally { foreach (var stage in stages.Values) stage.Dispose(); }
	}
}
