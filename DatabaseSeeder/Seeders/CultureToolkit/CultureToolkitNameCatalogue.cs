#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureNameRepertoire(string StableKey, string Era, NameCulture Culture,
	IReadOnlyList<JsonElement> ActiveEvidence, IReadOnlyList<string> Exclusions);

/// <summary>Builds only the delivered local repertoires; persistence and ethnicity bindings remain explicit.</summary>
public static class CultureToolkitNameCatalogue
{
	public static IReadOnlyList<CultureNameRepertoire> Build(CultureToolkitCatalogue catalogue, string era)
	{
		_ = catalogue.Compose(era);
		var results = new List<CultureNameRepertoire>();
		foreach (var pool in catalogue.Document("data.targeted_name_corpora.json").GetProperty("pools")
			.EnumerateArray().Where(x => CultureToolkitCatalogue.Strings(x.GetProperty("packs")).Contains(era)))
		{
			var key = CultureToolkitCatalogue.Text(pool, "key");
			var model = new NameCulture { Name = CultureToolkitCatalogue.Text(pool, "label") };
			var evidence = new List<JsonElement>();
			var exclusions = new List<string>();
			var allGiven = new List<string>();
			foreach (var (genderKey, gender) in new[] { ("male", Gender.Male), ("female", Gender.Female) })
			{
				var recipe = pool.GetProperty("production_recipe").GetProperty("profiles").GetProperty(genderKey);
				var entries = pool.GetProperty($"given_{genderKey}").EnumerateArray()
					.Where(x => CultureToolkitCatalogue.Strings(x.TryGetProperty("playable_packs", out var playable) ? playable : x.GetProperty("packs")).Contains(era)).ToArray();
				var requirement = catalogue.Document("data.name_playability_policy.json").GetProperty("requirements")
					.EnumerateArray().SingleOrDefault(x => CultureToolkitCatalogue.Text(x, "pool") == key &&
						CultureToolkitCatalogue.Text(x, "pack") == era && CultureToolkitCatalogue.Text(x, "gender") == genderKey);
				if (requirement.ValueKind != JsonValueKind.Undefined &&
					(!recipe.GetProperty("enabled").GetBoolean() || entries.Select(x => CultureToolkitCatalogue.Text(x, "family_key"))
						.Distinct(StringComparer.OrdinalIgnoreCase).Count() < requirement.GetProperty("minimum_distinct_families").GetInt32()))
					throw new InvalidOperationException($"Insufficient playable name families: {key}:{genderKey}:{era}");
				if (!recipe.GetProperty("enabled").GetBoolean() || entries.Length == 0)
				{
					exclusions.Add($"{key}:{genderKey}:{era}: no active replacement; retain source-specific legacy profile.");
					continue;
				}
				// The supplied delivery contains one reviewed display per lemma. Do not quietly
				// turn future aliases into extra chances in the existing weighted-name table.
				if (entries.Select(x => CultureToolkitCatalogue.Text(x, "lemma")).Distinct(StringComparer.OrdinalIgnoreCase).Count() != entries.Length ||
					entries.Select(x => CultureToolkitCatalogue.Text(x, "display")).Distinct(StringComparer.OrdinalIgnoreCase).Count() != entries.Length)
					throw new InvalidOperationException($"Multiple spelling variants require explicit lemma-first selection: {key}:{genderKey}");
				var profile = new RandomNameProfile
				{
					Name = CultureToolkitCatalogue.Text(recipe, "name"), Gender = (int)gender, NameCulture = model
				};
				profile.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
				{
					NameUsage = (int)NameUsage.BirthName, DiceExpression = CultureToolkitCatalogue.Text(recipe, "given_dice")
				});
				profile.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
				{
					NameUsage = (int)NameUsage.Surname, DiceExpression = CultureToolkitCatalogue.Text(recipe, "byname_dice")
				});
				foreach (var entry in entries)
				{
					var display = ValidateDisplay(CultureToolkitCatalogue.Text(entry, "display"), key);
					var weight = entry.TryGetProperty("weight_by_era", out var weights)
						? weights.GetProperty(era).GetInt32() : entry.GetProperty("weight").GetInt32();
					if (weight <= 0) throw new InvalidOperationException($"Non-positive playable name weight: {key}:{display}:{era}");
					profile.RandomNameProfilesElements.Add(new RandomNameProfilesElements
					{
						NameUsage = (int)NameUsage.BirthName, Name = display, Weighting = weight
					});
					allGiven.Add(display);
					evidence.Add(entry);
				}
				foreach (var byname in pool.GetProperty("bynames").EnumerateArray()
					.Where(x => CultureToolkitCatalogue.Text(x, "gender") == genderKey))
				{
					profile.RandomNameProfilesElements.Add(new RandomNameProfilesElements
					{
						NameUsage = (int)NameUsage.Surname,
						Name = ValidateDisplay(CultureToolkitCatalogue.Text(byname, "display"), key), Weighting = 100
					});
				}
				model.RandomNameProfiles.Add(profile);
			}
			model.Definition = Definition(CultureToolkitCatalogue.Text(pool, "description"), allGiven);
			results.Add(new CultureNameRepertoire(key, era, model, evidence, exclusions));
		}
		foreach (var requirement in catalogue.Document("data.name_playability_policy.json").GetProperty("requirements").EnumerateArray()
			.Where(x => CultureToolkitCatalogue.Text(x, "pack") == era))
		{
			var key = CultureToolkitCatalogue.Text(requirement, "pool");
			if (results.All(x => x.StableKey != key)) throw new InvalidOperationException($"Missing required playable name pool: {key}:{era}");
		}
		return results;
	}

	private static string ValidateDisplay(string value, string key)
	{
		var normalized = value.Normalize(NormalizationForm.FormC);
		if (normalized.Any(x => x > 255 || char.IsControl(x)) || string.IsNullOrWhiteSpace(normalized))
			throw new InvalidOperationException($"Unsupported active name display in {key}.");
		return normalized;
	}

	/// <summary>A separate local identity keeps the union out of sex-specific suggestion lists.</summary>
	public static NameCulture NeutralRepertoire(CultureNameRepertoire repertoire)
	{
		var model = new NameCulture { Name = repertoire.Culture.Name + " (Shared Repertoire)", Definition = repertoire.Culture.Definition };
		var profile = new RandomNameProfile
		{
			Name = model.Name, Gender = (int)Gender.NonBinary, NameCulture = model
		};
		profile.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			NameUsage = (int)NameUsage.BirthName, DiceExpression = "1"
		});
		profile.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			NameUsage = (int)NameUsage.Surname, DiceExpression = "0"
		});
		foreach (var entry in repertoire.Culture.RandomNameProfiles.SelectMany(x => x.RandomNameProfilesElements)
			.Where(x => x.NameUsage == (int)NameUsage.BirthName).GroupBy(x => x.Name, StringComparer.Ordinal))
		{
			profile.RandomNameProfilesElements.Add(new RandomNameProfilesElements
			{
				NameUsage = (int)NameUsage.BirthName, Name = entry.Key, Weighting = entry.Max(x => x.Weighting)
			});
		}
		model.RandomNameProfiles.Add(profile);
		return model;
	}

	public static string Definition(string description, IEnumerable<string> reviewedGivenNames)
	{
		// Known compound given names take precedence; arbitrary ambiguous full names can be
		// entered as structured elements through the existing NamePicker.
		var known = reviewedGivenNames.Distinct(StringComparer.Ordinal).OrderByDescending(x => x.Length)
			.ThenBy(x => x, StringComparer.Ordinal).Select(Regex.Escape).ToArray();
		var birth = string.Join("|", known.Append(@"[\p{L}'-]+"));
		var regex = $@"^(?<birthname>(?:{birth}))(?: (?<surname>[\p{{L}}' -]+))?$";
		return new XElement("NameCulture", new XElement("PreserveNameCase", true),
			new XElement("Patterns", Enum.GetValues<NameStyle>().Select(style => new XElement("Pattern",
				new XAttribute("Style", (int)style), new XAttribute("Params", "0,6"),
				new XAttribute("Text", style switch
				{
					NameStyle.GivenOnly or NameStyle.Affectionate => "{0}",
					NameStyle.SurnameOnly => "?surname[{1}][{0}]",
					_ => "{0}?surname[ {1}]"
				})))),
			new XElement("Elements",
				new XElement("Element", new XAttribute("Usage", (int)NameUsage.BirthName), new XAttribute("MinimumCount", 1),
					new XAttribute("MaximumCount", 1), new XAttribute("Name", "Given Name"), new XCData(description)),
				new XElement("Element", new XAttribute("Usage", (int)NameUsage.Surname), new XAttribute("MinimumCount", 0),
					new XAttribute("MaximumCount", 1), new XAttribute("Name", "Byname"),
					new XCData("A household, parentage or place name may distinguish you from others who share your given name."))),
			new XElement("NameEntryRegex", new XCData(regex))).ToString();
	}
}
