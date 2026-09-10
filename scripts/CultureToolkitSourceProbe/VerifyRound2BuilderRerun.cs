#nullable enable

using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MySqlConnector;

internal static class VerifyRound2BuilderRerun
{
	public static void ReadGraphs(string connection, string receiptPath, string outputPath)
	{
		using var receipt = JsonDocument.Parse(File.ReadAllText(receiptPath));
		var database = receipt.RootElement.GetProperty("Database").GetString()!;
		if (receipt.RootElement.GetProperty("Status").GetString() != "passed" || !database.StartsWith("futuremud_culture_live_2145_r2", StringComparison.Ordinal))
			throw new InvalidOperationException("A successful round-two disposable receipt is required.");
		var builder = new MySqlConnectionStringBuilder(connection) { Database = database };
		if (builder.Server is not ("localhost" or "127.0.0.1" or "::1")) throw new InvalidOperationException("Local fixtures only.");
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString)).Options);
		var records = context.SeederManagedRecords.Where(x => x.Seeder == "CultureSeeder" && x.EntityType == "Script").ToArray();
		var graphs = records.Select(record =>
		{
			var script = context.Scripts.Include(x => x.Knowledge).Single(x => x.Id == record.LogicalId);
			var members = context.ScriptsDesignedLanguages.Where(x => x.ScriptId == script.Id)
				.Select(x => new { x.LanguageId, x.Language.Name, x.Language.LinkedTraitId }).OrderBy(x => x.LanguageId).ToArray();
			var progId = context.SeederManagedRecords.Single(x => x.Seeder == "CultureSeeder" && x.EntityType == "FutureProg" && x.StableKey == record.StableKey + ".script-acquisition").LogicalId;
			var prog = context.FutureProgs.Find(progId)!;
			var eligible = System.Text.RegularExpressions.Regex.Matches(prog.FunctionText, @"@skill\.id == (\d+)")
				.Select(x => long.Parse(x.Groups[1].Value)).Distinct().Order().ToArray();
			if (!eligible.SequenceEqual(members.Select(x => x.LinkedTraitId).Distinct().Order()) || script.Knowledge.CanAcquireProgId != progId)
				throw new InvalidOperationException($"Pristine script {record.StableKey} differs from its generated acquisition graph.");
			return new { StableKey = record.StableKey, ScriptId = script.Id, Members = members, AcquisitionProgId = progId,
				GeneratedEligibleTraitIds = eligible, FunctionText = prog.FunctionText, MatchesStockGraph = true };
		}).ToArray();
		File.WriteAllText(outputPath, JsonSerializer.Serialize(new { Status = "passed", Database = database,
			Scope = "Read-only actual MySQL graph/prog receipt after committed rerun. Exact generated trait predicates match effective membership; runtime execution is separately covered by tests.",
			ScriptGraphs = graphs }, new JsonSerializerOptions { WriteIndented = true }));
	}

	public static void Run(string connection, string receiptPath, string outputPath)
	{
		using var receipt = JsonDocument.Parse(File.ReadAllText(receiptPath));
		var database = receipt.RootElement.GetProperty("Database").GetString()!;
		if (receipt.RootElement.GetProperty("Status").GetString() != "passed" ||
			!database.StartsWith("futuremud_culture_live_2145_r2", StringComparison.Ordinal))
			throw new InvalidOperationException("A successful round-two disposable import receipt is required.");
		var builder = new MySqlConnectionStringBuilder(connection) { Database = database };
		if (builder.Server is not ("localhost" or "127.0.0.1" or "::1")) throw new InvalidOperationException("Local fixtures only.");
		var era = receipt.RootElement.GetProperty("Report").GetProperty("Era").GetString()!;
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString)).Options);
		using var transaction = context.Database.BeginTransaction();
		var latin = context.Scripts.Include(x => x.Knowledge).Single(x => x.Name == "Latin");
		var deleted = context.ScriptsDesignedLanguages.First(x => x.ScriptId == latin.Id);
		var removedLanguageId = deleted.LanguageId;
		var removedTraitId = context.Languages.Find(removedLanguageId)!.LinkedTraitId;
		context.ScriptsDesignedLanguages.Remove(deleted);
		var trait = Copy(context, context.TraitDefinitions.Find(removedTraitId)!);
		trait.Name = "Round Two Builder Language";
		context.TraitDefinitions.Add(trait);
		context.SaveChanges();
		var language = Copy(context, context.Languages.Find(removedLanguageId)!);
		language.Name = trait.Name;
		language.LinkedTraitId = trait.Id;
		context.Languages.Add(language);
		context.SaveChanges();
		context.ScriptsDesignedLanguages.Add(new ScriptsDesignedLanguage { ScriptId = latin.Id, LanguageId = language.Id });
		var profileId = context.SeederManagedRecords.First(x => x.Seeder == "CultureSeeder" && x.EntityType == "RandomNameProfile" &&
			x.StableKey.StartsWith("names.target.") && !x.StableKey.Contains(".shared.")).LogicalId!.Value;
		var entries = context.RandomNameProfilesElements.Where(x => x.RandomNameProfileId == profileId && x.NameUsage == 0).Take(2).ToArray();
		var editedName = entries[0].Name;
		var deletedName = entries[1].Name;
		entries[0].Weighting = 7;
		context.RandomNameProfilesElements.Remove(entries[1]);
		context.RandomNameProfilesElements.Add(new RandomNameProfilesElements { RandomNameProfileId = profileId, NameUsage = 0, Name = "BuilderExample", Weighting = 3 });
		var accentId = context.SeederManagedRecords.First(x => x.Seeder == "CultureSeeder" && x.EntityType == "AccentAvailability").LogicalId;
		var accent = context.Accents.Find(accentId)!;
		var customPointer = context.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		accent.ChargenAvailabilityProgId = customPointer;
		context.SaveChanges();
		var reports = new List<CultureToolkitInstallReport>();
		for (var pass = 0; pass < 2; pass++)
		{
			Console.WriteLine($"Builder preservation pass {pass + 1}: {database}.");
			reports.Add(CultureToolkitInstaller.Install(context, era, true, true, true, Console.WriteLine));
			if (context.ScriptsDesignedLanguages.Find(latin.Id, removedLanguageId) is not null ||
				context.ScriptsDesignedLanguages.Find(latin.Id, language.Id) is null) throw new InvalidOperationException("Builder script graph changed.");
			var progId = context.SeederManagedRecords.Single(x => x.Seeder == "CultureSeeder" && x.EntityType == "FutureProg" && x.StableKey == "latin.script-acquisition").LogicalId;
			var body = context.FutureProgs.Find(progId)!.FunctionText;
			if (body.Contains($"@skill.id == {removedTraitId}\n", StringComparison.Ordinal) ||
				System.Text.RegularExpressions.Regex.IsMatch(body, $@"@skill\.id == {removedTraitId}(?!\d)")) throw new InvalidOperationException("Deleted language remains in stock acquisition.");
			if (!System.Text.RegularExpressions.Regex.IsMatch(body, $@"@skill\.id == {trait.Id}(?!\d)")) throw new InvalidOperationException("Builder language missing from stock acquisition.");
			if (context.RandomNameProfilesElements.Find(profileId, 0, editedName)!.Weighting != 7 ||
				context.RandomNameProfilesElements.Find(profileId, 0, deletedName) is not null ||
				context.RandomNameProfilesElements.Find(profileId, 0, "BuilderExample")!.Weighting != 3) throw new InvalidOperationException("Builder names changed.");
			if (accent.ChargenAvailabilityProgId != customPointer) throw new InvalidOperationException("Builder accent pointer changed.");
		}
		var graph = context.ScriptsDesignedLanguages.Where(x => x.ScriptId == latin.Id).Select(x => new { x.LanguageId, x.Language.LinkedTraitId }).ToArray();
		transaction.Rollback();
		transaction.Dispose();
		context.ChangeTracker.Clear();
		var acquisitionId = context.SeederManagedRecords.Single(x => x.Seeder == "CultureSeeder" && x.EntityType == "FutureProg" && x.StableKey == "latin.script-acquisition").LogicalId;
		var originalBody = context.FutureProgs.Find(acquisitionId)!.FunctionText;
		var beforeFailure = new[] { context.Scripts.Count(), context.Knowledges.Count(), context.ScriptsDesignedLanguages.Count(), context.FutureProgs.Count(), context.SeederManagedRecords.Count() };
		string compileFailure;
		using (var failureTransaction = context.Database.BeginTransaction())
		{
			context.FutureProgs.Find(acquisitionId)!.FunctionText = "return @RoundTwoMissingFunction()";
			context.ScriptsDesignedLanguages.Remove(context.ScriptsDesignedLanguages.Find(latin.Id, removedLanguageId)!);
			context.SaveChanges();
			var languages = context.SeederManagedRecords.Where(x => x.Seeder == "CultureSeeder" && x.EntityType == "Language").ToArray()
				.ToDictionary(x => x.StableKey, x => context.Languages.Find(x.LogicalId)!);
			try
			{
				CultureToolkitScriptSeeder.Upsert(context, new CultureToolkitCatalogue(), new CultureToolkitCatalogue().Compose(era),
					new Dictionary<string, FuturemudDatabaseContext>(), languages, new Dictionary<string, Script>(), []);
				throw new InvalidOperationException("Invalid generated dependency unexpectedly compiled.");
			}
			catch (InvalidOperationException error) when (error.Message.Contains("failed compilation", StringComparison.Ordinal))
			{
				compileFailure = error.Message;
			}
			failureTransaction.Rollback();
		}
		context.ChangeTracker.Clear();
		var afterFailure = new[] { context.Scripts.Count(), context.Knowledges.Count(), context.ScriptsDesignedLanguages.Count(), context.FutureProgs.Count(), context.SeederManagedRecords.Count() };
		if (!beforeFailure.SequenceEqual(afterFailure) || context.FutureProgs.Find(acquisitionId)!.FunctionText != originalBody ||
			context.ScriptsDesignedLanguages.Find(latin.Id, removedLanguageId) is null) throw new InvalidOperationException("Compilation failure was not atomic.");
		File.WriteAllText(outputPath, JsonSerializer.Serialize(new { Status = "passed", Database = database,
			Scope = "Actual MySQL full same-era toolkit rerun twice with builder membership, name and accent changes. All fixture edits and reruns rolled back after verification; pristine import retained. Generated progs compiled by installer; graph assertions here are structural, runtime execution is separately unit-tested.",
			RemovedLanguageId = removedLanguageId, AddedLanguageId = language.Id, AddedTraitId = trait.Id,
			ProfileId = profileId, EditedName = editedName, DeletedName = deletedName, AccentId = accentId, Graph = graph, Reports = reports,
			AtomicCompilationRollback = true, ExpectedCompileFailure = compileFailure }, new JsonSerializerOptions { WriteIndented = true }));
	}

	private static T Copy<T>(FuturemudDatabaseContext context, T source) where T : class, new()
	{
		var copy = new T();
		var model = context.Model.FindEntityType(typeof(T))!;
		var primary = model.FindPrimaryKey()!.Properties;
		foreach (var property in model.GetProperties().Where(x => x.PropertyInfo is not null && !primary.Contains(x)))
			property.PropertyInfo!.SetValue(copy, property.PropertyInfo.GetValue(source));
		return copy;
	}
}
