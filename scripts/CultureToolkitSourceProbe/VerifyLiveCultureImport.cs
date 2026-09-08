#nullable enable

using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MySqlConnector;

internal static class VerifyLiveCultureImport
{
	public static void Run(string sourceConnection, string database, string era, string reportPath, bool resume = false, bool verifyExisting = false)
	{
		if (database.Length > 40 || !Regex.IsMatch(database, "^futuremud_culture_live_2145_[a-z0-9_]+$"))
			throw new ArgumentException("A uniquely named CultureSeeder disposable database of at most 40 characters is required for the MySQL migration lock.");
		new CultureToolkitCatalogue().Compose(era);
		var builder = new MySqlConnectionStringBuilder(sourceConnection) { Database = "" };
		if (builder.Server is not ("localhost" or "127.0.0.1" or "::1")) throw new InvalidOperationException("Live fixture creation is restricted to the local development server.");
		var completed = new List<string>();
		if (resume || verifyExisting)
		{
			using var receipt = JsonDocument.Parse(File.ReadAllText(reportPath));
			if (receipt.RootElement.GetProperty("Database").GetString() != database || receipt.RootElement.GetProperty("Status").GetString() != (verifyExisting ? "passed" : "failed"))
				throw new InvalidOperationException("The matching prior import receipt is required for this exact disposable database and operation.");
			completed.AddRange(receipt.RootElement.GetProperty("Completed").EnumerateArray().Select(x => x.GetString()!));
			if (!completed.Contains(nameof(ChargenSeeder)) || completed.Contains(nameof(CultureSeeder)) != verifyExisting)
				throw new InvalidOperationException("Resume is limited to a failed CultureSeeder import after completed stock prerequisites.");
		}
		using (var server = new MySqlConnection(builder.ConnectionString))
		{
			server.Open();
			using var check = server.CreateCommand();
			check.CommandText = "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = @name";
			check.Parameters.AddWithValue("@name", database);
			var exists = Convert.ToInt32(check.ExecuteScalar()) != 0;
			if (exists != (resume || verifyExisting)) throw new InvalidOperationException($"Database {database} does not match the requested fresh/resume state; nothing was modified.");
			if (!resume && !verifyExisting)
			{
				using var create = server.CreateCommand();
				create.CommandText = $"CREATE DATABASE `{database}` CHARACTER SET utf8mb4";
				create.ExecuteNonQuery();
			}
		}
		builder.Database = database;
		try
		{
			using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString)).Options);
			if (!resume && !verifyExisting)
			{
				Console.WriteLine($"Migrating new disposable database {database}.");
				context.Database.Migrate();
				completed.Add("EF migrations");
			}
			var assembly = typeof(CultureSeeder).Assembly;
			var profilesType = assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles")!;
			var profiles = (IEnumerable)profilesType.GetProperty("All", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
			var profile = profiles.Cast<object>().Single(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)! == "medieval-standard");
			var steps = (IEnumerable)profile.GetType().GetProperty("Steps")!.GetValue(profile)!;
			var selected = new[] { typeof(CoreDataSeeder), typeof(TimeSeeder), typeof(AttributeSeeder), assembly.GetType("DatabaseSeeder.Seeders.SkillPackageSeeder")!, typeof(HumanSeeder), typeof(ChargenSeeder), typeof(CultureSeeder) };
			var execute = assembly.GetType("DatabaseSeeder.SeederExecutionService")!.GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Static)!;
			foreach (var step in steps)
			{
				var type = (Type)step.GetType().GetProperty("SeederType")!.GetValue(step)!;
				if (!selected.Contains(type) || completed.Contains(type.Name)) continue;
				var answers = ((IEnumerable)step.GetType().GetProperty("Answers")!.GetValue(step)!).Cast<object>()
					.ToDictionary(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)!, x => (string)x.GetType().GetProperty("Answer")!.GetValue(x)!);
				if (type == typeof(CultureSeeder)) answers["culturepacks"] = era;
				var seeder = (IDatabaseSeeder)Activator.CreateInstance(type)!;
				foreach (var question in seeder.Questions.Where(x => x.Filter(context, answers)))
				{
					var validation = question.Validator(answers[question.Id], context);
					if (!validation.Success) throw new InvalidOperationException($"{type.Name}:{question.Id}: {validation.error}");
				}
				Console.WriteLine($"Seeding {type.Name} into {database}.");
				var execution = execute.Invoke(null, [context, seeder, seeder.Questions, answers, assembly.GetName().Version!])!;
				if (!(bool)execution.GetType().GetProperty("Success")!.GetValue(execution)!)
					throw (Exception)execution.GetType().GetProperty("Exception")!.GetValue(execution)!;
				completed.Add(type.Name);
			}
			var before = Counts(context);
			using var transaction = context.Database.BeginTransaction();
			var report = CultureToolkitInstaller.Install(context, era, true, true, true, Console.WriteLine);
			if (!before.SequenceEqual(Counts(context))) throw new InvalidOperationException("The live second pass changed entity counts.");
			if (report.Conflicts.Count != 0) throw new InvalidOperationException("Live fixture conflicts: " + string.Join("\n", report.Conflicts));
			transaction.Commit();
			File.WriteAllText(reportPath, JsonSerializer.Serialize(new
			{
				Scope = "Actual local MySQL fresh EF migration and stock seeder import, followed by a committed CultureSeeder rerun. IDs are actual IDs in the named disposable database. No telnet/editor execution.",
				Database = database, Status = "passed", Completed = completed, Report = report
			}, new JsonSerializerOptions { WriteIndented = true }));
			Console.WriteLine($"Live import and rerun passed: {database}.");
		}
		catch (Exception error)
		{
			Environment.ExitCode = 1;
			File.WriteAllText(verifyExisting ? reportPath + ".failed-rerun.json" : reportPath,
				JsonSerializer.Serialize(new { Database = database, Status = "failed", Completed = completed, Error = error.GetBaseException().Message }, new JsonSerializerOptions { WriteIndented = true }));
			Console.WriteLine($"Live import stopped in {database}: {error.GetBaseException().Message}");
		}
	}

	private static int[] Counts(FuturemudDatabaseContext context) =>
		[context.Cultures.Count(), context.Ethnicities.Count(), context.Languages.Count(), context.Accents.Count(),
			context.RandomNameProfiles.Count(), context.RandomNameProfilesElements.Count(), context.SeederManagedRecords.Count(), context.FutureProgs.Count()];
}
