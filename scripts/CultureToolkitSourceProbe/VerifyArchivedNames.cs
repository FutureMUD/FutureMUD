#nullable enable

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

internal static class VerifyArchivedNames
{
	internal static void Run(string corpus, string report)
	{
		var stages = new Dictionary<string, FuturemudDatabaseContext>();
		try
		{
			foreach (var pack in new[] { "earthantiquity", "earthdarkagesandmedieval", "earthrenaissanceeurope", "earthrenaissanceworldexpansion" })
			{
				using var input = JsonDocument.Parse(File.ReadAllText(Path.Combine(corpus, pack + ".json")));
				var tables = input.RootElement.GetProperty("Tables");
				var context = Context();
				stages[pack] = context;
				context.AddRange(tables.GetProperty("NameCulture").Deserialize<NameCulture[]>()!);
				context.AddRange(tables.GetProperty("RandomNameProfile").Deserialize<RandomNameProfile[]>()!);
				context.AddRange(tables.GetProperty("RandomNameProfilesElements").Deserialize<RandomNameProfilesElements[]>()!);
				context.AddRange(tables.GetProperty("RandomNameProfilesDiceExpressions").Deserialize<RandomNameProfilesDiceExpressions[]>()!);
				context.SaveChanges();
				if (context.RandomNameProfiles.Any(x => x.UseForChargenSuggestionsProgId != null))
					throw new InvalidOperationException($"{pack}: this archive verifier requires explicit suggestion-prog source binding.");
			}
			var plan = CultureToolkitSourceNames.Describe(stages);
			using var installed = Context();
			var conflicts = new List<string>();
			Console.WriteLine($"Importing {plan.Profiles.Count} distinct retained profiles from four archived modules into InMemory persistence.");
			var first = CultureToolkitSourceNames.Upsert(installed, "medieval", plan, new Dictionary<string, NameCulture>(),
				new Dictionary<string, RandomNameProfile>(), new Dictionary<string, FutureProg>(), true, conflicts);
			var firstProfileCount = installed.RandomNameProfiles.Count();
			var firstElementCount = installed.RandomNameProfilesElements.Count();
			Console.WriteLine($"First import: {firstProfileCount} profiles, {firstElementCount} elements. Checking rerun.");
			CultureToolkitSourceNames.Upsert(installed, "medieval", plan, new Dictionary<string, NameCulture>(),
				new Dictionary<string, RandomNameProfile>(), new Dictionary<string, FutureProg>(), true, conflicts);
			var profiles = plan.Profiles.Select(x => new
			{
				x.Key, Sources = x.Sources.Select(y => new { y.Module, y.Id }), InstalledId = first.Profiles[x.Key].Id,
				SourceHash = Hash(Profile(x.Definition)), InstalledHash = Hash(Profile(first.Profiles[x.Key])),
				SourceElements = x.Definition.RandomNameProfilesElements.Count, InstalledElements = first.Profiles[x.Key].RandomNameProfilesElements.Count
			}).ToArray();
			var cultures = plan.Cultures.Select(x => new { x.Key, InstalledId = first.Cultures[x.Key].Id,
				SourceHash = Hash(x.Definition.Definition), InstalledHash = Hash(first.Cultures[x.Key].Definition),
				ExpectedPresentationHash = Hash(CultureToolkitProse.Rewrite(x.Sources[0].Module, "NameCulture", x.Definition.Name, "Definition", x.Definition.Definition)) }).ToArray();
			var passed = conflicts.Count == 0 && profiles.All(x => x.SourceHash == x.InstalledHash) && cultures.All(x => x.ExpectedPresentationHash == x.InstalledHash) &&
				firstProfileCount == installed.RandomNameProfiles.Count() && firstElementCount == installed.RandomNameProfilesElements.Count();
			File.WriteAllText(Path.GetFullPath(report), JsonSerializer.Serialize(new
			{
				Scope = "Actual C# InMemory import and rerun of four archived historical naming corpora. Original hashes remain separate from reviewed naming-guidance corrections; no MySQL or new historical certification.",
				Passed = passed, SourceProfileAliases = plan.Profiles.Sum(x => x.Sources.Count), DistinctProfiles = firstProfileCount,
				DistinctElements = firstElementCount, Cultures = cultures, Profiles = profiles, Conflicts = conflicts
			}, new JsonSerializerOptions { WriteIndented = true }));
			if (!passed) throw new InvalidOperationException("Retained naming import differs from the archived source; see report.");
			Console.WriteLine($"All {profiles.Length} retained profiles match their sources; {cultures.Length} naming structures match the preserved source plus the explicit prose ledger after rerun.");
		}
		finally { foreach (var stage in stages.Values) stage.Dispose(); }
	}

	private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
	private static string Profile(RandomNameProfile profile) => JsonSerializer.Serialize(new
	{
		profile.Name, profile.Gender,
		Elements = profile.RandomNameProfilesElements.OrderBy(x => x.NameUsage).ThenBy(x => x.Name, StringComparer.Ordinal).Select(x => new { x.Name, x.NameUsage, x.Weighting }),
		Dice = profile.RandomNameProfilesDiceExpressions.OrderBy(x => x.NameUsage).Select(x => new { x.NameUsage, x.DiceExpression })
	});
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
}
