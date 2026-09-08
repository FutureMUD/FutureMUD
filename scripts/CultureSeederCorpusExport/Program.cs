#nullable enable

using System.Reflection;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

// Explicit, offline export of the unmodified generator. Never connects to MySQL.
if (args.Length is < 1 or > 2)
{
	throw new ArgumentException("Supply a new output directory for the original generated corpus.");
}

var output = Path.GetFullPath(args[0]);
Directory.CreateDirectory(output);
var packs = new[] { "none", "earthantiquity", "earthdarkagesandmedieval", "earthrenaissanceeurope",
	"earthrenaissanceworldexpansion", "earthmodern", "middleearth" };
if (args.Length == 2)
{
	if (!packs.Contains(args[1])) throw new ArgumentException("Unknown source pack.");
	packs = [args[1]];
}
var options = new JsonSerializerOptions { WriteIndented = true };
foreach (var pack in packs)
{
	var path = Path.Combine(output, $"{pack}.json");
	if (File.Exists(path))
	{
		throw new IOException($"Refusing to overwrite original corpus: {path}");
	}

	using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
	var human = new Race { Name = "Human" };
	var intelligence = new TraitDefinition { Name = "Intelligence", Alias = "int", Type = 1 };
	context.Races.Add(human);
	context.TraitDefinitions.Add(intelligence);
	context.TraitDecorators.Add(new TraitDecorator { Name = "Language Skill" });
	context.Improvers.Add(new Improver { Name = "Language Improver" });
	context.LanguageDifficultyModels.Add(new LanguageDifficultyModels { Name = "Default" });
	context.BodyProtos.Add(new BodyProto { Name = "Humanoid" });
	context.BodypartShapes.Add(new BodypartShape { Name = "hand" });
	context.FutureProgs.AddRange(new FutureProg { FunctionName = "AlwaysTrue", FunctionText = "return true" },
		new FutureProg { FunctionName = "AlwaysFalse", FunctionText = "return false" });
	context.SaveChanges();
	var seeder = new CultureSeeder();
	typeof(CultureSeeder).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!
		.SetValue(seeder, context);
	// Middle-Earth accents reference heritage predicates. Record their symbolic source
	// bindings without pretending that a fixture is the original compiled predicate.
	var externalProgReferences = new Dictionary<long, string>();
	if (pack == "middleearth")
	{
		var references = new Dictionary<string, string[]>
		{
			["_ethnicProgs"] = ["Gondorian Dunedain", "Arnorian Dunedain", "Haradrim", "Variag",
				"Noldor", "Silvan", "Black Numenorean"],
			["_raceProgs"] = ["Orc", "Elf", "Dwarf", "Human", "Troll"],
			["_cultureProgs"] = ["Corsair"]
		};
		foreach (var (field, names) in references)
		{
			var dictionary = (Dictionary<string, FutureProg>)typeof(CultureSeeder)
				.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(seeder)!;
			foreach (var name in names)
			{
				var reference = new FutureProg { FunctionName = $"ExternalReference:{field}:{name}" };
				context.FutureProgs.Add(reference);
				context.SaveChanges();
				dictionary.Add(name, reference);
				externalProgReferences.Add(reference.Id, $"{field}[\"{name}\"]");
			}
		}
	}
	typeof(CultureSeeder).GetMethod("SeedSimple", BindingFlags.Instance | BindingFlags.NonPublic)!
		.Invoke(seeder, [context]);
	if (pack != "none")
	{
		seeder.SeedCulturePacks(context, new Dictionary<string, string>
		{
			["culturepacks"] = pack, ["seednames"] = "yes", ["seedlanguages"] = "yes",
			["seedheritage"] = "no", ["seedsignedlanguages"] = "yes"
		});
	}
	typeof(CultureSeeder).GetMethod("EnsureFallbackRandomNameProfiles", BindingFlags.Instance | BindingFlags.NonPublic)!
		.Invoke(seeder, []);
	context.SaveChanges();
	// Preserve every scalar, including XML definitions, weights, usage, gender and original
	// local foreign keys. Source pack + entity type + original ID qualifies those references.
	var types = new HashSet<Type>
	{
		typeof(NameCulture), typeof(RandomNameProfile), typeof(RandomNameProfilesElements),
		typeof(RandomNameProfilesDiceExpressions), typeof(Language), typeof(Accent),
		typeof(MutualIntelligability), typeof(Script), typeof(ScriptsDesignedLanguage),
		typeof(Knowledge), typeof(FutureProg), typeof(FutureProgsParameter),
		typeof(TraitDefinition), typeof(TraitExpression), typeof(SignedLanguage),
		typeof(SignedLanguageVariety), typeof(SignedLanguageArticulationProfile),
		typeof(SignedLanguageArticulationRequirement)
	};
	var tables = context.ChangeTracker.Entries()
		.Where(x => types.Contains(x.Metadata.ClrType))
		.GroupBy(x => x.Metadata.ClrType.Name)
		.OrderBy(x => x.Key, StringComparer.Ordinal)
		.ToDictionary(x => x.Key, x => x.Select(entry => entry.Properties
			.OrderBy(p => p.Metadata.Name, StringComparer.Ordinal)
			.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue))
			.OrderBy(row => JsonSerializer.Serialize(row), StringComparer.Ordinal).ToArray());
	File.WriteAllText(path, JsonSerializer.Serialize(new
	{
		SchemaVersion = 1, SourcePack = pack,
		Scope = "Original generated naming, language, accent and script corpus; fixture prerequisites are included. Heritage was not executed.",
		ExternalProgReferences = externalProgReferences, Tables = tables
	}, options));
	Console.WriteLine($"{pack}: {tables.GetValueOrDefault(nameof(NameCulture))?.Length ?? 0} name cultures, " +
		$"{tables.GetValueOrDefault(nameof(RandomNameProfile))?.Length ?? 0} profiles, " +
		$"{tables.GetValueOrDefault(nameof(RandomNameProfilesElements))?.Length ?? 0} elements, " +
		$"{tables.GetValueOrDefault(nameof(Accent))?.Length ?? 0} accents.");
}
