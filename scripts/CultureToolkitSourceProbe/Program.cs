using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

if (args is ["--build-contracts", var debugAssembly, var releaseAssembly, var buildReport])
{
	bool HasReplay(string path)
	{
		using var stream = File.OpenRead(path);
		using var pe = new PEReader(stream);
		var reader = pe.GetMetadataReader();
		return reader.TypeDefinitions.Any(handle => reader.GetString(reader.GetTypeDefinition(handle).Name) == "DebugSeederReplayProfiles");
	}
	var debug = HasReplay(debugAssembly);
	var release = HasReplay(releaseAssembly);
	File.WriteAllText(buildReport, JsonSerializer.Serialize(new { DebugReplayTypePresent = debug, ReleaseReplayTypePresent = release }, new JsonSerializerOptions { WriteIndented = true }));
	if (!debug || release) throw new InvalidOperationException("Debug replay build separation failed.");
	Console.WriteLine("Actual PE metadata confirms replay profiles are Debug-only.");
	return;
}

if (args is ["--archived-names", var corpus, var report])
{
	VerifyArchivedNames.Run(corpus, report);
	return;
}
if (args is ["--archived-languages", var languageCorpus, var languageReport])
{
	VerifyArchivedLanguages.Run(languageCorpus, languageReport);
	return;
}

// Reads only prerequisites from the configured database. Every seeder write is to an
// isolated InMemory context; the source connection never receives SaveChanges or SQL writes.
var connection = Environment.GetEnvironmentVariable("FUTUREMUD_SOURCE_PROBE_CONNECTION")
	?? throw new InvalidOperationException("Supply FUTUREMUD_SOURCE_PROBE_CONNECTION in the process environment.");
if (args is ["--round2-live-graphs", var graphSource, var graphOutput])
{
	VerifyRound2BuilderRerun.ReadGraphs(connection, graphSource, graphOutput);
	return;
}
if (args is ["--round2-builder-rerun", var priorReceipt, var builderReceipt])
{
	VerifyRound2BuilderRerun.Run(connection, priorReceipt, builderReceipt);
	return;
}
if (args is ["--live-receipt", var receiptDatabase, var receiptOutput])
{
	VerifyLiveReceipt.Run(connection, receiptDatabase, receiptOutput);
	return;
}
if (args is ["--live-import", var database, var era, var liveReport])
{
	VerifyLiveCultureImport.Run(connection, database, era, liveReport);
	return;
}
if (args is ["--live-resume", var resumeDatabase, var resumeEra, var resumeReport])
{
	VerifyLiveCultureImport.Run(connection, resumeDatabase, resumeEra, resumeReport, true);
	return;
}
if (args is ["--live-rerun", var rerunDatabase, var rerunEra, var rerunReport])
{
	VerifyLiveCultureImport.Run(connection, rerunDatabase, rerunEra, rerunReport, verifyExisting: true);
	return;
}
using var installed = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
	.UseMySql(connection, ServerVersion.AutoDetect(connection)).Options);
if (args is ["--benchmark-staging", var timingReport])
{
	BenchmarkSourceStaging.Run(installed, timingReport);
	return;
}
if (args is ["--round2-optional", var round2OptionalReport])
{
	VerifyToolkitInstall.Run(installed, round2OptionalReport, optional: true, round2: true);
	return;
}
if (args is ["--install-fixtures", var fixtureReport])
{
	VerifyToolkitInstall.Run(installed, fixtureReport);
	return;
}
if (args is ["--optional-fixtures", var optionalReport])
{
	VerifyToolkitInstall.Run(installed, optionalReport, true);
	return;
}
if (args is ["--upgrade-fixture", var upgradeReport])
{
	VerifyToolkitInstall.Run(installed, upgradeReport, upgrade: true);
	return;
}
var results = new List<object>();
var stages = new Dictionary<string, FuturemudDatabaseContext>();
var failed = false;
foreach (var pack in new[] { "earthantiquity", "earthdarkagesandmedieval", "earthrenaissanceeurope", "earthrenaissanceworldexpansion" })
{
	try
	{
		var stage = (FuturemudDatabaseContext)typeof(CultureSeeder).GetMethod("BuildToolkitSource", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [installed, pack])!;
		stages[pack] = stage;
		var result = new
		{
			SourcePack = pack, Status = "staged", NameCultures = stage.NameCultures.Count(), Profiles = stage.RandomNameProfiles.Count(),
			NameElements = stage.RandomNameProfilesElements.Count(), Accents = stage.Accents.Count(),
			Languages = stage.Languages.Select(x => x.Name).ToArray(),
			Ethnicities = stage.Ethnicities.Include(x => x.EthnicitiesNameCultures).ThenInclude(x => x.NameCulture).AsEnumerable()
				.Select(x => new { x.Name, x.ChargenBlurb, x.EthnicGroup, x.EthnicSubgroup, NamingStructures = x.EthnicitiesNameCultures.Select(n => n.NameCulture.Name).Distinct().ToArray() }).ToArray(),
			Cultures = stage.Cultures.Select(x => x.Name).ToArray()
		};
		results.Add(result);
		Console.WriteLine($"{pack}: {result.Ethnicities.Length} ethnicities, {result.Cultures.Length} cultures, {result.Profiles} naming profiles, {result.Accents} accents staged.");
	}
	catch (Exception error)
	{
		failed = true;
		var reason = error.GetBaseException().Message;
		results.Add(new { SourcePack = pack, Status = "failed", Reason = reason });
		Console.WriteLine($"{pack}: source staging failed: {reason}");
	}
}
if (installed.ChangeTracker.HasChanges()) throw new InvalidOperationException("Source context was unexpectedly modified; no writes were sent.");
if (args.Length == 1) File.WriteAllText(Path.GetFullPath(args[0]), JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
if (!failed && args.Length == 1)
{
	var genericHuman = installed.Ethnicities.AsNoTracking().Include(x => x.EthnicitiesCharacteristics)
		.Single(x => x.Name == "Admin" && x.ParentRace.Name == "Human");
	var catalogue = new CultureToolkitCatalogue();
	var plans = new[] { "antiquity", "darkages", "medieval", "renaissance", "earlymodern" }.Select(era => new
	{
		Era = era, Bindings = CultureToolkitHeritageSources.Describe(catalogue.Compose(era), stages, genericHuman).Select(x => new
		{
			x.Key, x.Module, SourceName = x.Template.Name, x.NamingStructureOverride, x.BindingReason, x.Aliases,
			IsSuppliedOverlay = x.Overlay.HasValue
		}).ToArray()
	}).ToArray();
	File.WriteAllText(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[0]))!, "CultureSeeder_Redesign_Heritage_Composition.json"),
		JsonSerializer.Serialize(new { Scope = "Source-template composition from isolated stages with read-only installed Human prerequisites; no live heritage writes or active source bindings claimed.", Plans = plans }, new JsonSerializerOptions { WriteIndented = true }));
	Console.WriteLine($"Heritage source templates resolved: {plans.SelectMany(x => x.Bindings).Where(x => x.IsSuppliedOverlay).Select(x => x.Key).Distinct().Count()}/68 supplied overlays.");
}
foreach (var stage in stages.Values) stage.Dispose();
Environment.ExitCode = failed ? 1 : 0;
