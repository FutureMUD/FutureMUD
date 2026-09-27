#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;

#nullable enable

namespace DatabaseSeeder;

/// <summary>Executable catalogue evidence from seeders, including dynamic animal and optional Antiquity content.</summary>
internal static class EnvironmentalExposureCatalogueCapture
{
	internal static bool TryHandle(string[] args)
	{
		if (!args.Contains("--capture-environmental-exposure", StringComparer.OrdinalIgnoreCase)) return false;
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var output = Path.Combine(root, "Design Documents", "Environment", "Environmental_Exposure_Catalogue.json");
		var dbName = "ExposureCatalogue-" + Guid.NewGuid().ToString("N");
		var provenance = new CatalogueProvenance();
		var baselineSeeders = Array.Empty<string>();
		var hasFullProvenance = false;
		var resumeIndex = Array.FindIndex(args, x => x.Equals("--resume-owned", StringComparison.OrdinalIgnoreCase));
		var resume = resumeIndex >= 0;
		var native = resume || args.Contains("--native", StringComparer.OrdinalIgnoreCase);
		if (resume && (resumeIndex + 1 >= args.Length || !System.Text.RegularExpressions.Regex.IsMatch(args[resumeIndex + 1], @"\Afuturemud_exposure_(?:[a-f0-9]{24}|[a-f0-9]{32})\z")))
			throw new ArgumentException("Resume requires the exact owned futuremud_exposure_<hex identity> fixture name.");
		string? checkpoint = null;
		var builder = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseLazyLoadingProxies().AddInterceptors(provenance);
		if (native)
		{
			dbName = resume ? args[resumeIndex + 1] : "futuremud_exposure_" + Guid.NewGuid().ToString("N")[..24];
			checkpoint = Path.Combine(root, ".artifacts", "exposure-capture-" + dbName + ".json");
			if (resume && !File.Exists(checkpoint)) throw new InvalidOperationException("No local ownership receipt exists for this fixture; resume refused.");
			if (resume)
			{
				using var receipt = JsonDocument.Parse(File.ReadAllText(checkpoint));
				var saved = receipt.RootElement;
				hasFullProvenance = saved.TryGetProperty("Complete", out var complete) && complete.GetBoolean() &&
					(saved.TryGetProperty("HasFullProvenance", out var full) ? full.GetBoolean() :
						saved.TryGetProperty("Resumed", out var resumed) && !resumed.GetBoolean());
				if (hasFullProvenance && saved.TryGetProperty("Provenance", out var producers))
				{
					foreach (var entry in producers.EnumerateObject()) provenance.Producers[entry.Name] = entry.Value.EnumerateArray().Select(x => x.GetString()!).ToHashSet();
					var completed = saved.TryGetProperty("BaselineCompletedSeeders", out var prior) && prior.GetArrayLength() > 0 ? prior : saved.GetProperty("CompletedSeeders");
					baselineSeeders = completed.EnumerateArray().Select(x => x.GetString()!).ToArray();
				}
				else hasFullProvenance = false;
			}
			if (!DebugSeederConnection.TryCreateConnectionString(dbName, out var connection, out var error)) throw new InvalidOperationException(error);
			var coordinator = new DatabaseUpgradeCoordinator();
			if (!resume)
			{
				Console.WriteLine($"Creating owned disposable native fixture {dbName}; no existing game is selected.");
				coordinator.RecreateEmptyDatabase(connection);
				coordinator.ImportBlankDatabaseSnapshot(connection, Path.Combine(root, "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.sql"), BlankDatabaseSnapshotManifest.DatabaseNamePlaceholder);
				Directory.CreateDirectory(Path.GetDirectoryName(checkpoint)!);
				File.WriteAllText(checkpoint, JsonSerializer.Serialize(new { Database = dbName, Complete = false }));
			}
			builder.UseMySql(connection, ServerVersion.AutoDetect(connection));
			output = Path.ChangeExtension(output, ".native.json");
		}
		else builder.UseInMemoryDatabase(dbName, x => x.EnableNullChecks(false)).ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
		var options = builder.Options;
		FuturemudDatabaseContext Context() => new(options);
		var baseline = DebugSeederReplayProfiles.All.Single(x => x.Id == "industrial-neutral");
		var steps = baseline.Steps.Select(step => step with
		{
			Answers = step.Answers.Select(a => step.SeederType == typeof(ItemSeeder) && a.Id == "eras"
				? a with { Answer = "antiquity medieval renaissance earlymodern" }
				: step.SeederType == typeof(CultureSeeder) && a.Id == "culturepacks" ? a with { Answer = "antiquity" }
				: step.SeederType == typeof(UsefulSeeder) && a.Id == "modernitems" ? a with { Answer = "yes" } : a).ToArray()
		}).ToArray();
		var profile = baseline with { Id = "exposure-catalogue-union", Name = "Exposure catalogue union", Steps = steps };
		var seeders = steps.Select(x => (IDatabaseSeeder)Activator.CreateInstance(x.SeederType)!).ToArray();
		SeederReplayRunResult result;
		if (resume)
		{
			using var repair = Context();
			// Older captures used names exceeding Pomelo's advisory-lock name limit. Apply the
			// generated pending range through the repository SQL executor on this owned fixture.
			var pendingSql = repair.GetService<IMigrator>().GenerateScript(repair.Database.GetAppliedMigrations().LastOrDefault() ?? "0",
				options: MigrationsSqlGenerationOptions.Idempotent);
			if (!DebugSeederConnection.TryCreateConnectionString(dbName, out var resumeConnection, out var resumeError)) throw new InvalidOperationException(resumeError);
			new MySqlDatabaseBackupService().ExecuteSqlScript(resumeConnection, pendingSql);
			var exposure = seeders.OfType<EnvironmentalExposureSeeder>().Single();
			provenance.Producer = nameof(EnvironmentalExposureSeeder);
			Console.WriteLine($"Resuming only Environmental Exposure on owned fixture {dbName}.");
			var execution = SeederExecutionService.Execute(repair, exposure, ((IDatabaseSeeder)exposure).Questions,
				new Dictionary<string, string> { ["natural"] = "yes", ["fantasy"] = "yes" }, typeof(Program).Assembly.GetName().Version!);
			result = new(profile, execution.Success ? new[] { exposure.Name } : Array.Empty<string>(), execution.Success ? null : exposure.Name,
				Array.Empty<string>(), execution.Success ? null : "Exposure fixture reconciliation failed.", execution.Exception, new(Array.Empty<string>()));
		}
		else result = SeederReplayRunner.Run(profile, seeders, Context, typeof(Program).Assembly.GetName().Version!, message =>
		{
			var seeder = seeders.FirstOrDefault(x => message == $"Running {x.Name}...");
			if (seeder is not null) provenance.Producer = seeder.GetType().Name;
			Console.WriteLine(message);
		});
		if (checkpoint is not null) File.WriteAllText(checkpoint, JsonSerializer.Serialize(new { Database = dbName, Complete = result.Success,
			Resumed = resume, result.CompletedSeeders, BaselineCompletedSeeders = baselineSeeders,
			HasFullProvenance = !resume || hasFullProvenance, Provenance = provenance.Producers }));
		if (!result.Success) throw new InvalidOperationException($"Catalogue capture stopped at {result.FailedSeeder}: {result.Failure}", result.Exception);
		using var context = Context();
		var rows = ExposureCatalogueAudit.Capture(context);
		var matrix = EnvironmentalExposureSeeder.Profiles.SelectMany(profile => Enum.GetValues<ExposureMaterialFamily>().Select(family => new
		{
			Source = profile.Name, profile.Fantasy, profile.Gas, profile.Category, profile.MinimumTemperature,
			Family = family.ToString(), Response = EnvironmentalExposureSeeder.Response(profile, family),
			InhalationResponse = profile.Gas ? EnvironmentalExposureSeeder.Response(profile, family, MudSharp.Form.Material.ExposureRoute.Inhalation) : null
		})).ToArray();
		Directory.CreateDirectory(Path.GetDirectoryName(output)!);
		File.WriteAllText(output, JsonSerializer.Serialize(new
		{
			SchemaVersion = 1, Capture = native ? "Executed seeders against disposable MySQL; not gameplay calibration" : "Executed seeders against EF InMemory; not a native database or gameplay calibration", Database = native ? dbName : "in-memory disposable",
			Profile = profile.Id, ItemEras = "antiquity medieval renaissance earlymodern", Culture = "antiquity",
			ExcludedInstallation = "Industrial ItemSeeder is blocked by its existing ProductionReady guard (464 food concepts). It creates no unique fluids or materials. Modern Useful components remain selected.",
			ProviderLimitations = native ? "None of the InMemory substitutions were used" : "Transactions ignored; required-property checks disabled because the provider does not apply MySQL column defaults. This capture does not verify relational constraints.",
			CompletedSeeders = result.CompletedSeeders, BaselineCompletedSeeders = baselineSeeders, ResumedExposureOnly = resume,
			ProvenanceLimitations = resume && !hasFullProvenance ? "This capture observes the final exposure rerun only. Earlier seed stages are documented in the baseline fixture log; their per-row producer map is not reconstructed." : "None",
			ProvenanceMethod = resume && hasFullProvenance ? "Observed full-install ownership receipt merged with this observed exposure rerun on the same owned database." : "Observed in this execution.",
			ConflictsAndDeferredEntries = seeders.OfType<EnvironmentalExposureSeeder>().Single().ReportNotes,
			Equipment = context.GameItemProtos.Where(x => x.UniqueName.StartsWith("exposure_")).ToList().Select(x => new
			{
				x.Id, x.RevisionNumber, x.UniqueName, x.MaterialId, x.HealthStrategyId,
				Components = x.GameItemProtosGameItemComponentProtos.Select(c => c.GameItemComponent.Name).Order().ToArray()
			}).ToArray(), Rows = rows, Matrix = matrix,
			Provenance = provenance.Producers.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value.Order().ToArray())
		}, new JsonSerializerOptions { WriteIndented = true }));
		static string Csv(string? text) => "\"" + (text ?? "").Replace("\"", "\"\"") + "\"";
		var csv = new StringBuilder("Kind,Id,Name,StableIdentifier,Family,Disposition,Reason,Tags,Aliases,HeatThresholdCelsius,Transmission,Reactions,Owners,AnatomyReferences,DrugId,SubstanceBindings\n");
		foreach (var row in rows) csv.AppendLine(string.Join(",", new[] { row.Kind, row.Id.ToString(), row.Name, row.StableIdentifier,
			row.Family, row.Disposition, row.Reason, string.Join(" | ", row.Tags), string.Join(" | ", row.Aliases),
			row.HeatThresholdCelsius?.ToString(System.Globalization.CultureInfo.InvariantCulture), row.Transmission, row.Reactions,
			string.Join(" | ", row.Owners), row.AnatomyReferences.ToString(), row.DrugId?.ToString(), string.Join(" | ", row.SubstanceBindings) }.Select(Csv)));
		File.WriteAllText(Path.ChangeExtension(output, ".csv"), csv.ToString());
		Console.WriteLine($"Captured {rows.Count} live seeded catalogue rows and {matrix.Length} family interactions: {output}");
		return true;
	}

	private sealed class CatalogueProvenance : SaveChangesInterceptor
	{
		public string Producer { get; set; } = "capture";
		public Dictionary<string, HashSet<string>> Producers { get; } = new();
		private object[] _pending = Array.Empty<object>();
		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			_pending = eventData.Context!.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified &&
				x.Entity is MudSharp.Models.Material or MudSharp.Models.Liquid or MudSharp.Models.Gas).Select(x => x.Entity).ToArray();
			return result;
		}
		public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
		{
			foreach (var entity in _pending)
			{
				var key = entity switch
				{
					MudSharp.Models.Material m => $"material:{m.Id}", MudSharp.Models.Liquid l => $"liquid:{l.Id}",
					MudSharp.Models.Gas g => $"gas:{g.Id}", _ => throw new InvalidOperationException()
				};
				if (!Producers.TryGetValue(key, out var sources)) Producers[key] = sources = new();
				sources.Add(Producer);
			}
			_pending = Array.Empty<object>();
			return result;
		}
	}
}
#endif
