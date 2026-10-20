#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonHeritageProfile(string Key, string Name, string? AncestralLanguage, string? SecondaryLanguage, int SecondaryValue);
public sealed record ArmageddonUpbringing(string Key, string Name, string Language, string Accent, bool Urban);

/// <summary>Approved nonphysical graph. New races remain explicitly unavailable physical scaffolds.</summary>
public static partial class ArmageddonHeritage
{
	public const string Module = "setting-heritage";
	public static IReadOnlyList<ArmageddonHeritageProfile> Profiles { get; } = Array.AsReadOnly(new ArmageddonHeritageProfile[]
	{
		new("human", "Human", null, null, 0), new("city_elf", "City Elf", "allundean", "sirihish", 100),
		new("desert_elf", "Desert Elf", "allundean", null, 0), new("dwarf", "Dwarf", "mirukkim", "sirihish", 100),
		new("half_elf", "Half-Elf", null, "allundean", 60), new("half_giant", "Half-Giant", null, null, 0), new("mul", "Mul", null, "mirukkim", 100)
	});
	public static IReadOnlyList<ArmageddonUpbringing> Cultures { get; } = Array.AsReadOnly(new ArmageddonUpbringing[]
	{
		new("allanaki", "Allanaki", "sirihish", "Southern", true), new("labyrinth", "Labyrinth", "sirihish", "Rinthi", true),
		new("tuluki", "Tuluki", "sirihish", "Northern", true), new("nomadic", "Nomadic", "bendune", "Tribal", false),
		new("desert_elven", "Desert Elven", "allundean", "Desert", false)
	});

	public static bool Compatible(string profile, ArmageddonUpbringing culture) => culture.Urban ? profile != "desert_elf" :
		culture.Key == "nomadic" ? profile == "human" : profile == "desert_elf";

	public static IReadOnlyDictionary<string, int> Grants(ArmageddonHeritageProfile profile, ArmageddonUpbringing culture)
	{
		if (!Compatible(profile.Key, culture)) throw new ArgumentException("Unapproved profile/upbringing combination.");
		var grants = new Dictionary<string, int> { [profile.AncestralLanguage ?? culture.Language] = 200 };
		if (profile.SecondaryLanguage is { } secondary) grants[secondary] = Math.Max(grants.GetValueOrDefault(secondary), profile.SecondaryValue);
		return grants;
	}

	private static T Apply<T>(FuturemudDatabaseContext db, string key, T desired, ICollection<string> messages) where T : class =>
		ArmageddonSettingOwnership.Apply(db, Module, key, desired, messages);
	private static string Json<T>(T value) => JsonSerializer.Serialize(value);
	private static void Preflight<T>(FuturemudDatabaseContext db, IEnumerable<(string Key, string Name)> identities) where T : class
	{
		var names = identities.ToArray();
		var field = db.Model.FindEntityType(typeof(T))!.FindProperty("Name")?.Name ?? "FunctionName";
		var installed = db.Set<T>().AsEnumerable().Select(x => (Row: x, Name: (string?)db.Entry(x).Property(field).CurrentValue)).ToArray();
		foreach (var (key, name) in names)
		{
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
			if (record is null)
			{
				if (installed.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException($"Unowned heritage {typeof(T).Name} collision: {name}.");
				continue;
			}
			if (record.Module != Module || record.EntityType != typeof(T).Name || record.Retired || record.LogicalId is not { } id || db.Find<T>(id) is null || record.SeedBaseline is null ||
				db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == id))
				throw new InvalidOperationException($"Invalid typed heritage ownership: {key}.");
		}
	}

	private static void PreflightNames(FuturemudDatabaseContext db, long storyboardId)
	{
		var languages = ArmageddonSkillPackage.Skills.Where(x => x.Language).Select(x => (Slug: x.Key["arm.skill.".Length..], x.Name)).ToArray();
		Preflight<Language>(db, languages.Select(x => ("arm.language." + x.Slug, x.Name)));
		Preflight<Race>(db, Profiles.Select(x => ("arm.race." + x.Key, "Armageddon " + x.Name)).Append(("arm.race.elf", "Armageddon Elf")));
		Preflight<Ethnicity>(db, Profiles.Select(x => ("arm.ethnicity." + x.Key + ".default", "Armageddon " + x.Name)));
		Preflight<Culture>(db, Cultures.Select(x => ("arm.culture." + x.Key, "Armageddon " + x.Name)));
		Preflight<NameCulture>(db, [("arm.naming.personal", "Armageddon Personal Name")]);
		PreflightNaming(db);
		// Accent names are scoped to languages, unlike the other native named entities.
		foreach (var language in languages)
		{
			var ownedLanguage = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.language." + language.Slug);
			if (ownedLanguage is null) continue;
			var expected = new[] { ("arm.accent." + language.Slug + ".standard", "Standard"), ("arm.accent." + language.Slug + ".learner", "Learner") }
				.Concat(Cultures.Where(x => x.Language == language.Slug).Select(x => ("arm.accent." + language.Slug + "." + x.Accent.ToLowerInvariant(), x.Accent)));
			foreach (var (key, name) in expected)
				if (!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key) && db.Accents.Any(x => x.LanguageId == ownedLanguage.LogicalId && x.Name == name))
					throw new InvalidOperationException($"Unowned heritage accent collision: {language.Name}/{name}.");
		}
		var progs = new List<(string, string)> { ("arm.approval.nomadic", "ArmageddonNomadicBackgroundApproved"), ("arm.chargen.free-skills." + storyboardId, "ArmageddonFreeSkills" + storyboardId) };
		foreach (var culture in Cultures)
		{
			progs.Add(("arm.culture." + culture.Key + ".availability", "ArmageddonCulture" + culture.Key.Replace("_", "") + "Available"));
			progs.Add(("arm.culture." + culture.Key + ".starting", "ArmageddonStarting" + culture.Key.Replace("_", "")));
			progs.Add(("arm.accent." + culture.Language + "." + culture.Accent.ToLowerInvariant() + ".availability", "ArmageddonAccent" + culture.Language + culture.Accent));
		}
		foreach (var language in languages)
		{
			progs.Add(("arm.language." + language.Slug + ".native", "ArmageddonNative" + language.Slug));
			progs.Add(("arm.language." + language.Slug + ".foreign", "ArmageddonForeign" + language.Slug));
		}
		Preflight<FutureProg>(db, progs);
	}

	private sealed record ProgParameter(long Type, string Name);
	private static FutureProg Prog(FuturemudDatabaseContext db, string key, string name, string body, ProgVariableTypes result,
		ICollection<string> messages, params (ProgVariableTypes Type, string Name)[] parameters)
	{
		var prog = Apply(db, key, new FutureProg { FunctionName = name, FunctionText = body, ReturnTypeDefinition = result.ToStorageString(),
			Category = "Armageddon", Subcategory = "Heritage", FunctionComment = "Owned Armageddon heritage binding; physical profiles remain unavailable pending acceptance.",
			Public = false, AcceptsAnyParameters = false, StaticType = 0 }, messages);
		var current = db.FutureProgsParameters.Where(x => x.FutureProgId == prog.Id).OrderBy(x => x.ParameterIndex).ToArray();
		var expected = parameters.Select(x => new ProgParameter((long)x.Type, x.Name)).ToArray();
		var merged = ArmageddonSettingLinks.Reconcile(db, key + ".parameters", nameof(FutureProg), prog.Id,
			new Dictionary<string, string> { ["Parameters"] = Json(current.Select(x => new ProgParameter(x.ParameterType, x.ParameterName)).ToArray()) },
			new Dictionary<string, string> { ["Parameters"] = Json(expected) }, messages);
		var next = JsonSerializer.Deserialize<ProgParameter[]>(merged["Parameters"])!;
		if (!next.SequenceEqual(expected)) throw new InvalidOperationException($"Heritage prog {name} has a custom incompatible signature; preserve and resolve before composition.");
		if (!current.Select(x => new ProgParameter(x.ParameterType, x.ParameterName)).SequenceEqual(next))
		{
			db.FutureProgsParameters.RemoveRange(current);
			foreach (var (value, index) in next.Select((x, i) => (x, i)))
				db.FutureProgsParameters.Add(new FutureProgsParameter { FutureProgId = prog.Id, ParameterIndex = index, ParameterType = value.Type, ParameterName = value.Name });
		}
		db.SaveChanges(); return prog;
	}

	// Authored sweat-only defaults; all physical profiles remain unavailable.
	internal static double SweatRateForProfile(string profile) => profile switch
	{
		"human" or "city_elf" or "desert_elf" or "dwarf" or "half_elf" => 0.02,
		"half_giant" => 0.10,
		"mul" => 0.03,
		_ => throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unknown heritage profile.")
	};

	private static Race Scaffold(FuturemudDatabaseContext db, string key, Race human)
	{
		var baseline = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key)?.SeedBaseline;
		if (baseline is null)
		{
			// EF values include the mapped public height/weight fields, unlike a property-only clone.
			var copy = (Race)db.Entry(human).CurrentValues.Clone().ToObject(); copy.Id = 0; return copy;
		}
		var desired = new Race();
		foreach (var field in JsonSerializer.Deserialize<Dictionary<string, string>>(baseline)!)
		{
			var property = db.Entry(desired).Property(field.Key);
			property.CurrentValue = JsonSerializer.Deserialize(field.Value, property.Metadata.ClrType);
		}
		return desired;
	}

	private static void NamingLinks(FuturemudDatabaseContext db, string key, Culture? culture, Ethnicity? ethnicity, long nameId, ICollection<string> messages)
	{
		var actual = culture is not null ? db.CulturesNameCultures.Where(x => x.CultureId == culture.Id).Select(x => new { x.Gender, x.NameCultureId }).AsEnumerable().Select(x => new NameLink(x.Gender, x.NameCultureId)).ToArray() :
			db.EthnicitiesNameCultures.Where(x => x.EthnicityId == ethnicity!.Id).Select(x => new { x.Gender, x.NameCultureId }).AsEnumerable().Select(x => new NameLink(x.Gender, x.NameCultureId)).ToArray();
		var desired = Enumerable.Range(0, 5).Select(x => new NameLink((short)x, nameId)).ToArray();
		var merged = ArmageddonSettingLinks.Reconcile(db, key + ".links", culture is not null ? nameof(Culture) : nameof(Ethnicity), culture?.Id ?? ethnicity!.Id,
			new Dictionary<string, string> { ["Naming"] = Json(actual.OrderBy(x => x.Gender)) }, new Dictionary<string, string> { ["Naming"] = Json(desired) }, messages);
		var next = JsonSerializer.Deserialize<NameLink[]>(merged["Naming"])!;
		if (culture is not null)
		{
			var joins = db.CulturesNameCultures.Where(x => x.CultureId == culture.Id).ToArray();
			db.CulturesNameCultures.RemoveRange(joins.Where(x => !next.Contains(new NameLink(x.Gender, x.NameCultureId))));
			foreach (var link in next.Where(x => !actual.Contains(x))) db.CulturesNameCultures.Add(new CulturesNameCultures { CultureId = culture.Id, Gender = link.Gender, NameCultureId = link.NameId });
		}
		else
		{
			var joins = db.EthnicitiesNameCultures.Where(x => x.EthnicityId == ethnicity!.Id).ToArray();
			db.EthnicitiesNameCultures.RemoveRange(joins.Where(x => !next.Contains(new NameLink(x.Gender, x.NameCultureId))));
			foreach (var link in next.Where(x => !actual.Contains(x))) db.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures { EthnicityId = ethnicity!.Id, Gender = link.Gender, NameCultureId = link.NameId });
		}
		db.SaveChanges();
	}
	private sealed record NameLink(short Gender, long NameId);

	public static void Install(FuturemudDatabaseContext db, ICollection<string> messages)
	{
		var skills = ArmageddonSkillPackage.Resolve(db);
		var human = db.Races.Single(x => x.Name == "Human");
		var organic = db.Races.Single(x => x.Id == human.ParentRaceId && x.Name == "Organic Humanoid");
		var adminCulture = db.Cultures.Single(x => x.Name == "Admin");
		var adminEthnicity = db.Ethnicities.Single(x => x.ParentRaceId == human.Id && x.Name == "Admin");
		var adminLanguage = db.Languages.Single(x => x.Name == "Admin Speech");
		if (!db.Calendars.Any(x => x.Id == adminCulture.PrimaryCalendarId) || !db.LanguageDifficultyModels.Any(x => x.Id == adminLanguage.DifficultyModel) ||
			adminEthnicity.PopulationBloodModelId is not { } blood || !db.PopulationBloodModels.Any(x => x.Id == blood) ||
			!db.BodyProtos.Any(x => x.Id == human.BaseBodyId) || !db.HealthStrategies.Any(x => x.Id == human.DefaultHealthStrategyId))
			throw new InvalidOperationException("Shared Human calendar/language/body/blood/health foundations are incomplete.");
		foreach (var modelId in new[] { human.DefaultHeightWeightModelMaleId, human.DefaultHeightWeightModelFemaleId,
			human.DefaultHeightWeightModelNeuterId, human.DefaultHeightWeightModelNonBinaryId })
			if (modelId is not { } id || !db.HeightWeightModels.Any(x => x.Id == id))
				throw new InvalidOperationException("Human height/weight model references are incomplete; inactive scaffolds cannot adopt dangling references.");
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id;
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		var storyboard = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)MudSharp.CharacterCreation.ChargenStage.SelectSkills);
		var freeReference = XElement.Parse(storyboard.StageDefinition).Element("FreeSkillsProg")?.Value
			?? throw new InvalidOperationException("Skill storyboard has no FreeSkillsProg composition seam.");
		var oldFree = long.TryParse(freeReference, out var freeId) ? db.FutureProgs.Single(x => x.Id == freeId) : db.FutureProgs.Single(x => x.FunctionName == freeReference);
		PreflightNames(db, storyboard.Id);
		var freeParameters = db.FutureProgsParameters.Where(x => x.FutureProgId == oldFree.Id).OrderBy(x => x.ParameterIndex).ToArray();
		if (oldFree.ReturnType != (long)(ProgVariableTypes.Trait | ProgVariableTypes.Collection) || freeParameters.Length != 1 ||
			freeParameters[0].ParameterType != (long)ProgVariableTypes.Toon && freeParameters[0].ParameterType != (long)ProgVariableTypes.Chargen)
			throw new InvalidOperationException("Existing FreeSkillsProg is incompatible; its body and pointer were preserved.");
		using var transaction = db.Database.IsRelational() && db.Database.CurrentTransaction is null ? db.Database.BeginTransaction() : null;
		var languages = new Dictionary<string, Language>();
		foreach (var entry in ArmageddonSkillPackage.Skills.Where(x => x.Language))
		{
			var slug = entry.Key["arm.skill.".Length..];
			languages[slug] = Apply(db, "arm.language." + slug, new Language { Name = entry.Name, LinkedTraitId = skills[entry.Key].Id,
				DifficultyModel = adminLanguage.DifficultyModel, UnknownLanguageDescription = "an unfamiliar language", LanguageObfuscationFactor = 0.1 }, messages);
		}
		var parent = Scaffold(db, "arm.race.elf", human);
		parent.Name = "Armageddon Elf"; parent.ParentRaceId = organic.Id; parent.AvailabilityProgId = no;
		parent.Description = "Common biological Elf parent; unavailable. Physical values are inactive Human scaffolding pending review.";
		parent.SweatRateInLitresPerMinute = SweatRateForProfile("city_elf");
		parent = Apply(db, "arm.race.elf", parent, messages);
		var races = new Dictionary<string, Race>();
		foreach (var profile in Profiles)
		{
			var key = "arm.race." + profile.Key;
			var race = Scaffold(db, key, human);
			race.Name = "Armageddon " + profile.Name; race.ParentRaceId = profile.Key is "city_elf" or "desert_elf" ? parent.Id : organic.Id;
			race.AvailabilityProgId = no;
			race.Description = $"{profile.Name} mechanical profile. Unavailable pending physical acceptance; current numerical physiology, ages and size are inactive shared-Human scaffolding. No racial immunity or power is implied.";
			race.SweatRateInLitresPerMinute = SweatRateForProfile(profile.Key);
			races[profile.Key] = Apply(db, key, race, messages);
		}
		var naming = Apply(db, "arm.naming.personal", new NameCulture { Name = "Armageddon Personal Name", Definition = new XElement("NameCulture",
			new XElement("Counts", new XElement("Count", new XAttribute("Usage", 0), new XAttribute("Min", 1), new XAttribute("Max", 1))),
			new XElement("Patterns", Enumerable.Range(0, 6).Select(x => new XElement("Pattern", new XAttribute("Style", x), new XAttribute("Text", "{0}"), new XAttribute("Params", "0")))),
			new XElement("Elements", new XElement("Element", new XAttribute("Usage", 0), new XAttribute("MinimumCount", 1), new XAttribute("MaximumCount", 1), new XAttribute("Name", "Personal Name"),
				new XCData("Enter the personal name by which your character is known. Names are free entry; no random-name corpus is installed."))),
			new XElement("NameEntryRegex", new XCData(@"^(?<birthname>[\p{L}\p{M} '-]+)$"))).ToString() }, messages);
		var actualCultures = new Dictionary<string, Culture>();
		foreach (var culture in Cultures)
		{
			var slug = culture.Key;
			var allowed = Profiles.Where(x => Compatible(x.Key, culture)).Select(x => races[x.Key].Id).ToArray();
			var approval = culture.Key == "nomadic" ? Prog(db, "arm.approval.nomadic", "ArmageddonNomadicBackgroundApproved", "return false", ProgVariableTypes.Boolean, messages, (ProgVariableTypes.Chargen, "ch")) : null;
			var body = "return (" + string.Join(" or ", allowed.Select(x => $"@ch.Race == ToRace({x})")) + ")" + (approval is null ? "" : $" and @{approval.FunctionName}(@ch)");
			var available = Prog(db, "arm.culture." + slug + ".availability", "ArmageddonCulture" + slug.Replace("_", "") + "Available", body, ProgVariableTypes.Boolean, messages, (ProgVariableTypes.Chargen, "ch"));
			var starting = new StringBuilder();
			foreach (var profile in Profiles.Where(x => Compatible(x.Key, culture)))
			foreach (var grant in Grants(profile, culture))
				starting.AppendLine($"if (@ch.Race == ToRace({races[profile.Key].Id}) and @skill == ToTrait({skills["arm.skill." + grant.Key].Id}))\n  return {grant.Value}\nend if");
			starting.AppendLine("return 0");
			var start = Prog(db, "arm.culture." + slug + ".starting", "ArmageddonStarting" + slug.Replace("_", ""), starting.ToString(), ProgVariableTypes.Number, messages,
				(ProgVariableTypes.Toon, "ch"), (ProgVariableTypes.Trait, "skill"), (ProgVariableTypes.Number, "boost"));
			var actual = Apply(db, "arm.culture." + slug, new Culture { Name = "Armageddon " + culture.Name, Description = culture.Name + " upbringing. Background only; no city room, spawn, faction or criminal status is imported.",
				NativeLanguageId = languages[culture.Language].Id, AvailabilityProgId = available.Id, PrimaryCalendarId = adminCulture.PrimaryCalendarId, SkillStartingValueProgId = start.Id,
				PersonWordMale = "Man", PersonWordFemale = "Woman", PersonWordNeuter = "Person", PersonWordIndeterminate = "Person", TolerableTemperatureCeilingEffect = 0, TolerableTemperatureFloorEffect = 0 }, messages);
			actualCultures[slug] = actual;
		}
		foreach (var profile in Profiles)
		{
			var key = "arm.ethnicity." + profile.Key + ".default";
			var ethnicity = Apply(db, key, new Ethnicity { Name = "Armageddon " + profile.Name, ParentRaceId = races[profile.Key].Id, AvailabilityProgId = yes,
				NativeLanguageId = profile.AncestralLanguage is { } native ? languages[native].Id : null, PopulationBloodModelId = blood,
				EthnicGroup = profile.Name, EthnicSubgroup = "Default", ChargenBlurb = "Minimum engine identity for the approved profile; broad Human characteristic scaffolds remain unqualified for racial appearance.",
				TolerableTemperatureCeilingEffect = 0, TolerableTemperatureFloorEffect = 0 }, messages);
			EthnicityLinks(db, key, ethnicity, adminEthnicity, null, messages);
		}
		InstallNaming(db, actualCultures, races, messages);
		InstallAccents(db, languages, actualCultures, messages);
		WireFreeSkills(db, storyboard, oldFree, actualCultures, races, skills, messages);
		db.SaveChanges();
		using (var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray()))
			foreach (var record in db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module && x.EntityType == nameof(FutureProg)).ToArray()) compiler.Compile(record.LogicalId!.Value);
		transaction?.Commit();
		messages.Add("Nonphysical heritage graph installed: five languages/cultures, seven exact-profile ethnicities, approved personal-name suggestions with free entry and native200/source secondary grants. All eight new Race rows default unavailable pending physical acceptance; deliberate builder overrides are retained and reported. Bootstrap Human/Admin and existing characters were preserved. No scripts or city spawns installed.");
	}

	private sealed record CharacteristicLink(long Definition, long Profile);
	private static void EthnicityLinks(FuturemudDatabaseContext db, string key, Ethnicity ethnicity, Ethnicity source, long? nameId, ICollection<string> messages)
	{
		// One typed reference inventory per ethnicity contains both naming and characteristics.
		var names = db.EthnicitiesNameCultures.Where(x => x.EthnicityId == ethnicity.Id).AsEnumerable().Select(x => new NameLink(x.Gender, x.NameCultureId)).OrderBy(x => x.Gender).ToArray();
		var current = db.EthnicitiesCharacteristics.Where(x => x.EthnicityId == ethnicity.Id).AsEnumerable().Select(x => new CharacteristicLink(x.CharacteristicDefinitionId, x.CharacteristicProfileId)).OrderBy(x => x.Definition).ToArray();
		var desired = db.EthnicitiesCharacteristics.Where(x => x.EthnicityId == source.Id).AsEnumerable().Select(x => new CharacteristicLink(x.CharacteristicDefinitionId, x.CharacteristicProfileId)).OrderBy(x => x.Definition).ToArray();
		if (desired.Length == 0) throw new InvalidOperationException("Human characteristic profiles are missing.");
		var merged = ArmageddonSettingLinks.Reconcile(db, key + ".links", nameof(Ethnicity), ethnicity.Id,
			new Dictionary<string, string> { ["Naming"] = Json(names), ["Characteristics"] = Json(current) },
			new Dictionary<string, string> { ["Naming"] = Json(nameId is { } namingId ? Enumerable.Range(0, 5).Select(x => new NameLink((short)x, namingId)).ToArray() : Array.Empty<NameLink>()), ["Characteristics"] = Json(desired) }, messages);
		var nextNames = JsonSerializer.Deserialize<NameLink[]>(merged["Naming"])!;
		var oldNames = db.EthnicitiesNameCultures.Where(x => x.EthnicityId == ethnicity.Id).ToArray();
		db.EthnicitiesNameCultures.RemoveRange(oldNames.Where(x => !nextNames.Contains(new NameLink(x.Gender, x.NameCultureId))));
		foreach (var link in nextNames.Where(x => !names.Contains(x))) db.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures { EthnicityId = ethnicity.Id, Gender = link.Gender, NameCultureId = link.NameId });
		var next = JsonSerializer.Deserialize<CharacteristicLink[]>(merged["Characteristics"])!;
		var old = db.EthnicitiesCharacteristics.Where(x => x.EthnicityId == ethnicity.Id).ToArray();
		db.EthnicitiesCharacteristics.RemoveRange(old.Where(x => !next.Any(y => y.Definition == x.CharacteristicDefinitionId)));
		foreach (var link in next)
		{
			var row = old.SingleOrDefault(x => x.CharacteristicDefinitionId == link.Definition);
			if (row is null) { row = new EthnicitiesCharacteristics { EthnicityId = ethnicity.Id, CharacteristicDefinitionId = link.Definition }; db.EthnicitiesCharacteristics.Add(row); }
			row.CharacteristicProfileId = link.Profile;
		}
		db.SaveChanges();
	}

	private static void InstallAccents(FuturemudDatabaseContext db, IReadOnlyDictionary<string, Language> languages, IReadOnlyDictionary<string, Culture> cultures, ICollection<string> messages)
	{
		foreach (var (slug, language) in languages)
		{
			var native = Prog(db, "arm.language." + slug + ".native", "ArmageddonNative" + slug, $"return @ch.nativelanguage.id == {language.Id}", ProgVariableTypes.Boolean, messages, (ProgVariableTypes.Chargen, "ch"));
			var foreign = Prog(db, "arm.language." + slug + ".foreign", "ArmageddonForeign" + slug, $"return @ch.nativelanguage.id != {language.Id}", ProgVariableTypes.Boolean, messages, (ProgVariableTypes.Chargen, "ch"));
			var regional = Cultures.Where(x => x.Language == slug).ToArray();
			foreach (var culture in regional)
			{
				var available = Prog(db, "arm.accent." + slug + "." + culture.Accent.ToLowerInvariant() + ".availability", "ArmageddonAccent" + slug + culture.Accent,
					$"return @ch.Culture == ToCulture({cultures[culture.Key].Id})", ProgVariableTypes.Boolean, messages, (ProgVariableTypes.Chargen, "ch"));
				Apply(db, "arm.accent." + slug + "." + culture.Accent.ToLowerInvariant(), new Accent { LanguageId = language.Id, Name = culture.Accent,
					Suffix = "with a " + culture.Accent.ToLowerInvariant() + " accent", VagueSuffix = "with a regional accent", Group = "regional", Role = 0, Difficulty = 5,
					Description = "Recovered regional identity; native difficulty5 is an authored adaptation, not a historical accent percentage.", ChargenAvailabilityProgId = available.Id }, messages);
			}
			// Explicit neutral scaffolds cover ancestry natives without a recovered regional accent.
			Apply(db, "arm.accent." + slug + ".standard", new Accent { LanguageId = language.Id, Name = "Standard", Role = 0, Difficulty = 5, Group = "native scaffold",
				Suffix = "", VagueSuffix = "", Description = "Authored neutral native accent scaffold; no historical accent corpus is asserted.", ChargenAvailabilityProgId = native.Id }, messages);
			Apply(db, "arm.accent." + slug + ".learner", new Accent { LanguageId = language.Id, Name = "Learner", Role = 2, Difficulty = 6, Group = "learner scaffold",
				Suffix = "with a learner's accent", VagueSuffix = "with an unfamiliar accent", Description = "Authored non-native fallback, not an additional spoken language.", ChargenAvailabilityProgId = foreign.Id }, messages);
		}
	}

	private static void WireFreeSkills(FuturemudDatabaseContext db, ChargenScreenStoryboard storyboard, FutureProg current,
		IReadOnlyDictionary<string, Culture> cultures, IReadOnlyDictionary<string, Race> races, IReadOnlyDictionary<string, TraitDefinition> skills, ICollection<string> messages)
	{
		var key = "arm.chargen.free-skills." + storyboard.Id;
		var existing = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		// Preserve the original delegate identity in the wrapper's original body on rerun; do not wrap itself.
		var delegateId = existing is null ? current.Id : ExtractDelegate(JsonSerializer.Deserialize<string>(JsonSerializer.Deserialize<Dictionary<string, string>>(existing.SeedBaseline!)!["FunctionText"])!);
		var delegateName = db.FutureProgs.Single(x => x.Id == delegateId).FunctionName;
		var text = new StringBuilder($"// Armageddon delegate id: {delegateId}\nvar skills as trait collection\nskills = @{delegateName}(@ch)\n");
		foreach (var culture in Cultures)
		foreach (var profile in Profiles.Where(x => Compatible(x.Key, culture)))
		{
			text.AppendLine($"if (@ch.Culture == ToCulture({cultures[culture.Key].Id}) and @ch.Race == ToRace({races[profile.Key].Id}))");
			foreach (var grant in Grants(profile, culture)) text.AppendLine($"  if (not(Contains(@skills, ToTrait({skills["arm.skill." + grant.Key].Id}))))\n    additem skills ToTrait({skills["arm.skill." + grant.Key].Id})\n  end if");
			text.AppendLine("end if");
		}
		text.AppendLine("return @skills");
		var wrapper = Prog(db, key, "ArmageddonFreeSkills" + storyboard.Id, text.ToString(), ProgVariableTypes.Trait | ProgVariableTypes.Collection, messages, (ProgVariableTypes.Chargen, "ch"));
		var xml = XElement.Parse(storyboard.StageDefinition);
		var reference = xml.Element("FreeSkillsProg")!;
		var merged = ArmageddonSettingLinks.Reconcile(db, "arm.chargen.skills-wiring." + storyboard.Id, nameof(ChargenScreenStoryboard), storyboard.Id,
			new Dictionary<string, string> { ["FreeSkillsProg"] = Json(reference.Value) }, new Dictionary<string, string> { ["FreeSkillsProg"] = Json(wrapper.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)) }, messages);
		reference.Value = JsonSerializer.Deserialize<string>(merged["FreeSkillsProg"])!;
		storyboard.StageDefinition = xml.ToString();
	}
	private static long ExtractDelegate(string text)
	{
		const string marker = "// Armageddon delegate id: ";
		var first = text.Replace("\r\n", "\n").Split('\n')[0];
		if (!first.StartsWith(marker, StringComparison.Ordinal) || !long.TryParse(first[marker.Length..], out var id) || id <= 0)
			throw new InvalidOperationException("Custom free-skill wrapper has no original delegate marker; preserve and resolve composition explicitly.");
		return id;
	}
}
