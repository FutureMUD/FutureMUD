#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.FutureProg;

namespace DatabaseSeeder.Seeders;

/// <summary>Approved adaptation, retaining canonical pools and curated non-random background constraints.</summary>
internal static class ArmageddonNaming
{
	private static readonly JsonDocument Document = JsonDocument.Parse(typeof(ArmageddonNaming).Assembly
		.GetManifestResourceStream("DatabaseSeeder.Armageddon.naming.json")
		?? throw new InvalidOperationException("Missing approved naming manifest."));
	internal static JsonElement Data => Document.RootElement;
	internal static JsonElement[] Cultures => Data.GetProperty("cultures").EnumerateArray().ToArray();
	internal static string Id(JsonElement culture) => culture.GetProperty("id").GetString()!;
	internal static string Name(JsonElement culture) => "Armageddon " + culture.GetProperty("name").GetString()!;
	internal static string Key(JsonElement culture) => "arm.naming." + Id(culture);
	internal static string[] Eligible(JsonElement culture, int gender)
	{
		var pool = Data.GetProperty("personal_pools").GetProperty(culture.GetProperty("personal_pool_id").GetString()!);
		string[] Values(string field) => pool.GetProperty(field).EnumerateArray().Select(x => x.GetString()!).ToArray();
		var shared = Values("gender_unspecified");
		var result = gender switch { 2 => Values("male").Concat(shared), 3 => Values("female").Concat(shared),
			_ => Values("male").Concat(Values("female")).Concat(shared) };
		var values = result.Distinct(StringComparer.Ordinal).ToArray();
		return values.Length > 0 ? values : Values("male").Concat(Values("female")).Concat(shared).Distinct(StringComparer.Ordinal).ToArray();
	}

	internal static string Definition(JsonElement culture)
	{
		var personal = culture.GetProperty("elements").EnumerateArray().Single(x => x.GetProperty("id").GetString() == "personal_name");
		return new XElement("NameCulture",
			new XElement("PreserveNameCase", true), new XElement("ExactRandomProfileGender", true),
			new XElement("AllowTypographicPunctuation", true),
			new XElement("Patterns", Enumerable.Range(0, 6).Select(x => new XElement("Pattern", new XAttribute("Style", x), new XAttribute("Text", "{0}"), new XAttribute("Params", "0")))),
			new XElement("Elements", new XElement("Element", new XAttribute("Usage", 0), new XAttribute("MinimumCount", 1), new XAttribute("MaximumCount", 1),
				new XAttribute("Name", personal.GetProperty("label").GetString()!), new XCData(personal.GetProperty("description").GetString()!))),
			new XElement("NameEntryRegex", new XCData(@"^(?<birthname>[\p{L}\p{M} '’‐-]+)$")),
			new XElement("CuratedNamingDefinition", new XCData(JsonSerializer.Serialize(new
			{
				culture, source = Data.GetProperty("source"), components = Data.GetProperty("component_collections"),
				defaults = Data.GetProperty("culture_element_defaults"), random_defaults = Data.GetProperty("random_profile_defaults"),
				interpretation = "Supplementary elements are curated background definitions, disabled in generic random generation. No unrelated NameUsage slots encode tribe, clan, affiliation or earned path."
			})))).ToString();
	}
}

public static partial class ArmageddonHeritage
{
	private static void PreflightNaming(FuturemudDatabaseContext db)
	{
		Preflight<NameCulture>(db, ArmageddonNaming.Cultures.Select(x => (ArmageddonNaming.Key(x), ArmageddonNaming.Name(x))));
		Preflight<RandomNameProfile>(db, ArmageddonNaming.Cultures.SelectMany(c => Enumerable.Range(0, 5)
			.Select(g => (ArmageddonNaming.Key(c) + ".gender" + g, ArmageddonNaming.Name(c) + " - " + g))));
		var storyboard = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)MudSharp.CharacterCreation.ChargenStage.SelectName);
		if (storyboard.ChargenType != "NamePicker") throw new InvalidOperationException("Approved naming requires the configurable NamePicker storyboard; custom screen preserved.");
		Preflight<FutureProg>(db, [("arm.chargen.naming." + storyboard.Id, "ArmageddonNamingStyles" + storyboard.Id)]);
		NamingDelegate(db, storyboard);
	}

	private sealed record NameEntry(int Usage, string Name, int Weight);
	private sealed record NameDice(int Usage, string Expression);
	private static void InstallNaming(FuturemudDatabaseContext db, IReadOnlyDictionary<string, Culture> cultures,
		IReadOnlyDictionary<string, Race> races, ICollection<string> messages)
	{
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id;
		var installed = new Dictionary<string, NameCulture>();
		foreach (var culture in ArmageddonNaming.Cultures)
		{
			var key = ArmageddonNaming.Key(culture);
			var native = Apply(db, key, new NameCulture { Name = ArmageddonNaming.Name(culture), Definition = ArmageddonNaming.Definition(culture) }, messages);
			installed[ArmageddonNaming.Id(culture)] = native;
			for (var gender = 0; gender < 5; gender++)
			{
				var profileKey = key + ".gender" + gender;
				var profile = Apply(db, profileKey, new RandomNameProfile { Name = ArmageddonNaming.Name(culture) + " - " + gender,
					NameCultureId = native.Id, Gender = gender, UseForChargenSuggestionsProgId = yes }, messages);
				var actual = db.RandomNameProfilesElements.Where(x => x.RandomNameProfileId == profile.Id).AsEnumerable()
					.Select(x => new NameEntry(x.NameUsage, x.Name, x.Weighting)).OrderBy(x => x.Usage).ThenBy(x => x.Name, StringComparer.Ordinal).ToArray();
				var dice = db.RandomNameProfilesDiceExpressions.Where(x => x.RandomNameProfileId == profile.Id).AsEnumerable()
					.Select(x => new NameDice(x.NameUsage, x.DiceExpression)).OrderBy(x => x.Usage).ToArray();
				var desired = ArmageddonNaming.Eligible(culture, gender).Select(x => new NameEntry(0, x, 1)).OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();
				var merged = ArmageddonSettingLinks.Reconcile(db, profileKey + ".links", nameof(RandomNameProfile), profile.Id,
					new Dictionary<string, string> { ["Names"] = Json(actual), ["Dice"] = Json(dice) },
					new Dictionary<string, string> { ["Names"] = Json(desired), ["Dice"] = Json(new[] { new NameDice(0, "1") }) }, messages);
				var next = JsonSerializer.Deserialize<NameEntry[]>(merged["Names"])!;
				foreach (var row in db.RandomNameProfilesElements.Where(x => x.RandomNameProfileId == profile.Id).ToArray())
				{
					var entry = next.SingleOrDefault(x => x.Usage == row.NameUsage && x.Name == row.Name);
					if (entry is null) db.RandomNameProfilesElements.Remove(row); else row.Weighting = entry.Weight;
				}
				foreach (var entry in next.Where(x => !actual.Any(a => a.Usage == x.Usage && a.Name == x.Name)))
					db.RandomNameProfilesElements.Add(new RandomNameProfilesElements { RandomNameProfileId = profile.Id, NameUsage = entry.Usage, Name = entry.Name, Weighting = entry.Weight });
				var nextDice = JsonSerializer.Deserialize<NameDice[]>(merged["Dice"])!;
				foreach (var row in db.RandomNameProfilesDiceExpressions.Where(x => x.RandomNameProfileId == profile.Id).ToArray())
				{
					var entry = nextDice.SingleOrDefault(x => x.Usage == row.NameUsage);
					if (entry is null) db.RandomNameProfilesDiceExpressions.Remove(row); else row.DiceExpression = entry.Expression;
				}
				foreach (var entry in nextDice.Where(x => !dice.Any(a => a.Usage == x.Usage)))
					db.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions { RandomNameProfileId = profile.Id, NameUsage = entry.Usage, DiceExpression = entry.Expression });
				db.SaveChanges();
			}
		}
		foreach (var (key, culture) in cultures)
			NamingLinks(db, "arm.culture." + key, culture, null, installed[key == "allanaki" ? "allanaki_common" : key == "tuluki" ? "tuluki_common" : key].Id, messages);
		WireNamingChoices(db, cultures, races, installed, messages);
	}

	private static void WireNamingChoices(FuturemudDatabaseContext db, IReadOnlyDictionary<string, Culture> cultures,
		IReadOnlyDictionary<string, Race> races, IReadOnlyDictionary<string, NameCulture> names, ICollection<string> messages)
	{
		var storyboard = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)MudSharp.CharacterCreation.ChargenStage.SelectName);
		var xml = XElement.Parse(storyboard.StageDefinition);
		var key = "arm.chargen.naming." + storyboard.Id;
		var reference = xml.Element("NameCultureOptionsProg")?.Value ?? "0";
		var original = NamingDelegate(db, storyboard);
		var text = new StringBuilder($"// Armageddon naming delegate id: {original}\nvar styles as nameculture collection\n");
		if (original != 0)
		{
			var delegated = db.FutureProgs.Single(x => x.Id == original);
			text.AppendLine($"styles = @{delegated.FunctionName}(@ch)");
		}
		foreach (var culture in Cultures)
		{
			var id = names[culture.Key == "allanaki" ? "allanaki_common" : culture.Key == "tuluki" ? "tuluki_common" : culture.Key].Id;
			text.AppendLine($"if (@ch.Culture == ToCulture({cultures[culture.Key].Id}))\n  additem styles ToNameCulture({id})\nend if");
		}
		foreach (var profile in Profiles)
		{
			var racial = profile.Key is "city_elf" or "desert_elf" ? "elf" : profile.Key;
			text.AppendLine($"if (@ch.Race == ToRace({races[profile.Key].Id}))\n  additem styles ToNameCulture({names[racial].Id})\nend if");
		}
		text.AppendLine("return @styles");
		var wrapper = Prog(db, key, "ArmageddonNamingStyles" + storyboard.Id, text.ToString(), ProgVariableTypes.NameCulture | ProgVariableTypes.Collection, messages, (ProgVariableTypes.Chargen, "ch"));
		var currentUnicode = xml.Element("AllowUnicodeNames")?.Value ?? "false";
		var merged = ArmageddonSettingLinks.Reconcile(db, "arm.chargen.naming-wiring." + storyboard.Id, nameof(ChargenScreenStoryboard), storyboard.Id,
			new Dictionary<string, string> { ["NameCultureOptionsProg"] = Json(reference), ["AllowUnicodeNames"] = Json(currentUnicode) },
			new Dictionary<string, string> { ["NameCultureOptionsProg"] = Json(wrapper.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)), ["AllowUnicodeNames"] = Json("true") }, messages);
		xml.SetElementValue("NameCultureOptionsProg", JsonSerializer.Deserialize<string>(merged["NameCultureOptionsProg"]));
		xml.SetElementValue("AllowUnicodeNames", JsonSerializer.Deserialize<string>(merged["AllowUnicodeNames"]));
		storyboard.StageDefinition = xml.ToString();
	}

	private static long NamingDelegate(FuturemudDatabaseContext db, ChargenScreenStoryboard storyboard)
	{
		var existing = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.chargen.naming." + storyboard.Id);
		var reference = XElement.Parse(storyboard.StageDefinition).Element("NameCultureOptionsProg")?.Value ?? "0";
		if (existing is not null)
		{
			var body = JsonSerializer.Deserialize<string>(JsonSerializer.Deserialize<Dictionary<string, string>>(existing.SeedBaseline!)!["FunctionText"])!;
			const string marker = "// Armageddon naming delegate id: ";
			if (!body.StartsWith(marker, StringComparison.Ordinal)) throw new InvalidOperationException("Owned naming wrapper lacks its original delegate marker.");
			reference = body[marker.Length..].Split('\n')[0].Trim();
		}
		if (!long.TryParse(reference, out var id) || id < 0) throw new InvalidOperationException("Invalid naming-style delegate reference; original preserved.");
		if (id == 0) return 0;
		var delegated = db.FutureProgs.Single(x => x.Id == id);
		var parameters = db.FutureProgsParameters.Where(x => x.FutureProgId == id).OrderBy(x => x.ParameterIndex).ToArray();
		if (!ProgVariableTypes.FromStorageString(delegated.ReturnTypeDefinition).CompatibleWith(ProgVariableTypes.NameCulture | ProgVariableTypes.Collection) ||
			!delegated.AcceptsAnyParameters && (parameters.Length != 1 || !ProgVariableTypes.Chargen.CompatibleWith(ProgVariableTypes.FromStorageString(parameters[0].ParameterTypeDefinition))))
			throw new InvalidOperationException("Custom naming-style delegate has an incompatible signature; original pointer/body preserved.");
		return id;
	}
}
