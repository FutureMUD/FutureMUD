#nullable enable

using System.Reflection;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

internal static class VerifyToolkitInstall
{
	public static void Run(FuturemudDatabaseContext installed, string reportPath, bool optional = false, bool upgrade = false, bool round2 = false)
	{
		var results = new List<object>();
		var eras = new[] { "antiquity", "darkages", "medieval", "renaissance", "earlymodern" };
		var scenarios = upgrade
			? new[] { (Era: "medieval", CultureFirst: false, Flags: 7) }
			: optional
			? eras.SelectMany(era => (round2 ? new[] { 1, 2, 4 } : Enumerable.Range(0, 7)).Select(flags => (Era: era, CultureFirst: false, Flags: flags)))
			: eras.SelectMany(era => new[] { false, true }.Select(first => (Era: era, CultureFirst: first, Flags: 7)));
		foreach (var scenario in scenarios)
		{
			var (era, cultureFirst, flags) = scenario;
			var names = (flags & 1) != 0;
			var languages = (flags & 2) != 0;
			var heritage = (flags & 4) != 0;
			Console.WriteLine($"Installing {era}, culture first: {cultureFirst}, names/languages/heritage: {names}/{languages}/{heritage} into isolated InMemory fixture.");
			try
			{
				using var fixture = (FuturemudDatabaseContext)typeof(CultureSeeder)
					.GetMethod("CreateToolkitPrerequisiteContext", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [installed])!;
				var admin = installed.Ethnicities.AsNoTracking().Single(x => x.Name == "Admin" && x.ParentRace.Name == "Human");
				Copy(fixture, admin);
				foreach (var member in installed.EthnicitiesCharacteristics.AsNoTracking().Where(x => x.EthnicityId == admin.Id)) Copy(fixture, member);
				var literacy = installed.TraitDefinitions.AsNoTracking().Single(x => x.Name == "Literacy" && (x.Type == 0 || x.Type == 2));
				Copy(fixture, literacy);
				if (fixture.TraitExpressions.Find(literacy.ExpressionId) is null)
					Copy(fixture, installed.TraitExpressions.AsNoTracking().Single(x => x.Id == literacy.ExpressionId));
				fixture.Accounts.Add(new Account { Name = "Fixture" });
				fixture.SaveChanges();
				if (upgrade)
				{
					var legacy = new CultureSeeder();
					typeof(CultureSeeder).GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(legacy, fixture);
					typeof(CultureSeeder).GetMethod("SeedSimple", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(legacy, [fixture]);
					legacy.SeedCulturePacks(fixture, new Dictionary<string, string>
					{
						["culturepacks"] = "earthdarkagesandmedieval", ["seednames"] = "yes", ["seedlanguages"] = "yes", ["seedheritage"] = "yes"
					});
					fixture.SaveChanges();
					Console.WriteLine("Retained pre-refactor combined Dark Ages/Medieval baseline installed in the fixture.");
				}
				var answers = new Dictionary<string, string>
				{
					["rpp"] = "no", ["bp"] = "no", ["class"] = "no", ["role-first"] = "race", ["attributemode"] = "order",
					["skillmode"] = "picker", ["merits"] = "merit", ["customdescs"] = "no"
				};
				if (!cultureFirst) new ChargenSeeder().SeedData(fixture, answers);
				var first = new CultureSeeder().SeedData(fixture, new Dictionary<string, string>
				{
					["culturepacks"] = era, ["seednames"] = names ? "yes" : "no", ["seedlanguages"] = languages ? "yes" : "no", ["seedheritage"] = heritage ? "yes" : "no"
				});
				Console.WriteLine(first);
				if (cultureFirst) new ChargenSeeder().SeedData(fixture, answers);
				var before = Counts(fixture);
				Console.WriteLine("Checking second installer pass.");
				var second = CultureToolkitInstaller.Install(fixture, era, names, languages, heritage, Console.WriteLine);
				var after = Counts(fixture);
				if (!before.SequenceEqual(after)) throw new InvalidOperationException($"Rerun grew records: {string.Join(',', before)} -> {string.Join(',', after)}");
				if (!upgrade && (first.Contains("Preserved overrides") || second.Conflicts.Count != 0))
					throw new InvalidOperationException("Fresh fixture conflicts: " + first + "\n" + string.Join("\n", second.Conflicts));
				if (!names && second.TargetedNames.Count != 0 || !languages && second.LanguageIds.Count != 0 ||
					!heritage && second.CultureIds.Count != 0 || (!languages || !heritage) && second.Groups.Count != 0)
					throw new InvalidOperationException("Skipped content was incorrectly reported as installed.");
				foreach (var link in fixture.EthnicitiesNameCultures)
					if (fixture.NameCultures.Find(link.NameCultureId) is null || fixture.Ethnicities.Find(link.EthnicityId) is null)
						throw new InvalidOperationException("Dangling ethnicity/name-culture link.");
				foreach (var language in fixture.Languages)
					if (fixture.TraitDefinitions.Find(language.LinkedTraitId) is null || language.DefaultLearnerAccentId is long accentId && fixture.Accents.Find(accentId) is null)
						throw new InvalidOperationException("Dangling language/trait/learner link.");
				if (heritage)
				foreach (var overlay in new CultureToolkitCatalogue().Compose(era).Ethnicities)
					if (fixture.Ethnicities.Find(second.EthnicityIds[CultureToolkitCatalogue.Text(overlay, "key")])!.AvailabilityProgId !=
						fixture.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id)
						throw new InvalidOperationException("A fresh supplied overlay inherited unavailable template policy.");
				results.Add(new { Era = era, CultureFirst = cultureFirst, Names = names, Languages = languages, Heritage = heritage, LegacyUpgrade = upgrade, Status = "passed", Counts = after, Report = second });
				Console.WriteLine($"{era}: passed, {second.CultureIds.Count} social cultures, {second.Groups.Count} groups; rerun stable.");
			}
			catch (Exception error)
			{
				Environment.ExitCode = 1;
				results.Add(new { Era = era, CultureFirst = cultureFirst, Names = names, Languages = languages, Heritage = heritage, Status = "failed", Error = error.ToString() });
				Console.WriteLine($"{era}: failed: {error.GetBaseException().Message}");
			}
			WriteReport(reportPath, results);
		}
		if (installed.ChangeTracker.HasChanges()) throw new InvalidOperationException("Read-only prerequisite context unexpectedly changed.");
	}

	private static void WriteReport(string reportPath, List<object> results)
	{
		File.WriteAllText(Path.GetFullPath(reportPath), JsonSerializer.Serialize(new
		{
			Scope = "C# integrated installer fixtures using read-only MySQL prerequisites and isolated InMemory writes. All reported entity IDs are fixture IDs, not live content imports. No telnet execution.", Results = results
		}, new JsonSerializerOptions { WriteIndented = true }));
	}

	private static int[] Counts(FuturemudDatabaseContext context) =>
		[context.Cultures.Count(), context.Ethnicities.Count(), context.NameCultures.Count(), context.RandomNameProfiles.Count(),
			context.RandomNameProfilesElements.Count(), context.Languages.Count(), context.Accents.Count(), context.FutureProgs.Count(), context.SeederManagedRecords.Count()];

	private static void Copy<T>(FuturemudDatabaseContext context, T source) where T : class, new()
	{
		var clone = new T();
		foreach (var property in context.Model.FindEntityType(typeof(T))!.GetProperties().Where(x => x.PropertyInfo is not null))
			property.PropertyInfo!.SetValue(clone, property.PropertyInfo.GetValue(source));
		context.Set<T>().Add(clone);
	}
}
