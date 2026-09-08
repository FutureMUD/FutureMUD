#nullable enable

using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

internal static class VerifyArchivedLanguages
{
	internal static void Run(string corpus, string report)
	{
		var stages = new Dictionary<string, FuturemudDatabaseContext>();
		try
		{
			foreach (var module in new[] { "earthantiquity", "earthdarkagesandmedieval", "earthrenaissanceeurope", "earthrenaissanceworldexpansion" })
			{
				using var input = JsonDocument.Parse(File.ReadAllText(Path.Combine(corpus, module + ".json")));
				var tables = input.RootElement.GetProperty("Tables");
				var stage = Context();
				stages[module] = stage;
				void Load<T>() where T : class => stage.AddRange(tables.GetProperty(typeof(T).Name).Deserialize<T[]>()!);
				Load<FutureProg>(); Load<FutureProgsParameter>(); Load<TraitDefinition>(); Load<TraitExpression>();
				Load<Language>(); Load<Accent>(); Load<MutualIntelligability>(); Load<Script>(); Load<Knowledge>(); Load<ScriptsDesignedLanguage>();
				stage.SaveChanges();
			}
			var reports = new List<object>();
			var catalogue = new CultureToolkitCatalogue();
			foreach (var era in new[] { "antiquity", "darkages", "medieval", "renaissance", "earlymodern" })
			{
				using var installed = Context();
				var intelligence = new TraitDefinition { Id = 1, Name = "Intelligence", Type = 1, Alias = "int" };
				var trueProg = new FutureProg { FunctionName = "AlwaysTrue", FunctionText = "return true", ReturnType = (long)MudSharp.FutureProg.ProgVariableTypes.Boolean };
				var falseProg = new FutureProg { FunctionName = "AlwaysFalse", FunctionText = "return false", ReturnType = (long)MudSharp.FutureProg.ProgVariableTypes.Boolean };
				var decorator = new TraitDecorator(); var improver = new Improver(); var difficulty = new LanguageDifficultyModels();
				installed.AddRange(intelligence, trueProg, falseProg, decorator, improver, difficulty);
				installed.SaveChanges();
				var prerequisites = new CultureLanguagePrerequisites(intelligence, decorator, improver, difficulty, trueProg, falseProg);
				var pack = catalogue.Compose(era);
				var conflicts = new List<string>();
				Console.WriteLine($"{era}: importing source-bound languages, accents and scripts into InMemory persistence.");
				var first = CultureToolkitLanguageSeeder.Upsert(installed, catalogue, pack, stages, new Dictionary<string, Language>(), prerequisites, conflicts);
				var scripts = CultureToolkitScriptSeeder.Upsert(installed, catalogue, pack, stages, first.Languages, new Dictionary<string, Script>(), conflicts);
				var sourceEdges = stages.SelectMany(stage => stage.Value.MutualIntelligabilities.AsEnumerable().Select(edge =>
					new CultureSourceLanguageEdge(CultureToolkitLanguageBindings.Key(stage.Key, stage.Value.Languages.Find(edge.ListenerLanguageId)!.Name),
						CultureToolkitLanguageBindings.Key(stage.Key, stage.Value.Languages.Find(edge.TargetLanguageId)!.Name), edge.IntelligabilityDifficulty,
						$"source.{stage.Key}.intelligibility.{edge.ListenerLanguageId}.{edge.TargetLanguageId}")))
					.Where(x => first.Languages.ContainsKey(x.ListenerKey) && first.Languages.ContainsKey(x.TargetKey)).ToArray();
				var edges = CultureToolkitIntelligibility.Reconcile(installed, catalogue, pack, first.Languages, sourceEdges, conflicts, first.Languages.Values.Select(x => x.Id).ToHashSet());
				var counts = new { Languages = installed.Languages.Count(), Accents = installed.Accents.Count(), Scripts = installed.Scripts.Count() };
				var second = CultureToolkitLanguageSeeder.Upsert(installed, catalogue, pack, stages, new Dictionary<string, Language>(), prerequisites, conflicts);
				CultureToolkitScriptSeeder.Upsert(installed, catalogue, pack, stages, second.Languages, new Dictionary<string, Script>(), conflicts);
				CultureToolkitIntelligibility.Reconcile(installed, catalogue, pack, second.Languages, sourceEdges, conflicts);
				var stable = counts.Languages == installed.Languages.Count() && counts.Accents == installed.Accents.Count() && counts.Scripts == installed.Scripts.Count();
				reports.Add(new { Era = era, StableCounts = stable, Counts = counts, Conflicts = conflicts, Caps = second.CapReports,
					Languages = second.Languages.Select(x => new { Key = x.Key, x.Value.Id, x.Value.LinkedTraitId, x.Value.Name }),
					Scripts = scripts.Select(x => new { Key = x.Key, x.Value.Id, x.Value.KnowledgeId }), DirectedEdges = edges });
				Console.WriteLine($"{era}: {counts.Languages} languages, {counts.Accents} accents, {counts.Scripts} scripts; rerun stable={stable}, conflicts={conflicts.Count}.");
				if (!stable) throw new InvalidOperationException($"{era}: source import rerun duplicated entities.");
			}
			File.WriteAllText(Path.GetFullPath(report), JsonSerializer.Serialize(new
			{
				Scope = "Actual C# InMemory language/script/import reruns from archived source definitions. IDs are fixture IDs, not live MySQL IDs. Not full toolkit or telnet verification.",
				Eras = reports
			}, new JsonSerializerOptions { WriteIndented = true }));
		}
		finally { foreach (var stage in stages.Values) stage.Dispose(); }
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
}
