#nullable enable

using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using MudSharp.Database;

internal static class BenchmarkSourceStaging
{
	public static void Run(FuturemudDatabaseContext installed, string reportPath)
	{
		var results = new List<object>();
		foreach (var module in new[] { "earthantiquity", "earthdarkagesandmedieval", "earthrenaissanceeurope", "earthrenaissanceworldexpansion" })
		{
			var clock = Stopwatch.StartNew();
			using var immediate = (FuturemudDatabaseContext)typeof(CultureSeeder)
				.GetMethod("CreateToolkitPrerequisiteContext", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [installed])!;
			var seeder = new CultureSeeder();
			typeof(CultureSeeder).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(seeder, immediate);
			typeof(CultureSeeder).GetMethod("SeedSimple", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, [immediate]);
			seeder.SeedCulturePacks(immediate, new Dictionary<string, string>
			{
				["culturepacks"] = module, ["seednames"] = "yes", ["seedlanguages"] = "yes", ["seedheritage"] = "yes"
			});
			typeof(CultureSeeder).GetMethod("EnsureFallbackRandomNameProfiles", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, []);
			immediate.SaveChanges();
			var immediateSeconds = clock.Elapsed.TotalSeconds;
			clock.Restart();
			using var deferred = (FuturemudDatabaseContext)typeof(CultureSeeder)
				.GetMethod("BuildToolkitSource", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [installed, module])!;
			var deferredSeconds = clock.Elapsed.TotalSeconds;
			string Names(FuturemudDatabaseContext context)
			{
				var plan = CultureToolkitSourceNames.Describe(new Dictionary<string, FuturemudDatabaseContext> { [module] = context });
				return JsonSerializer.Serialize(new
				{
					Cultures = plan.Cultures.OrderBy(x => x.Key).Select(x => new { x.Key, x.Definition.Name, x.Definition.Definition }),
					Profiles = plan.Profiles.OrderBy(x => x.Key).Select(x => new
					{
						x.Key, x.CultureKey, x.SuggestionProgName, x.Definition.Name, x.Definition.Gender,
						Elements = x.Definition.RandomNameProfilesElements.OrderBy(y => y.NameUsage).ThenBy(y => y.Name, StringComparer.Ordinal)
							.Select(y => new { y.NameUsage, y.Name, y.Weighting }),
						Dice = x.Definition.RandomNameProfilesDiceExpressions.OrderBy(y => y.NameUsage).Select(y => new { y.NameUsage, y.DiceExpression })
					})
				});
			}
			if (Names(immediate) != Names(deferred)) throw new InvalidOperationException($"Staging changed the naming corpus for {module}.");
			results.Add(new { Module = module, ImmediateSeconds = immediateSeconds, DeferredSeconds = deferredSeconds,
				NameElements = deferred.RandomNameProfilesElements.Count(), NamingCorpusEqual = true });
			Console.WriteLine($"{module}: immediate {immediateSeconds:N2}s, deferred {deferredSeconds:N2}s; naming corpus equal.");
			File.WriteAllText(reportPath, JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
		}
	}
}
