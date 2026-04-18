#nullable enable

using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private static readonly IReadOnlyDictionary<string, (string Male, string Female)> MedievalEuropeEthnicityNameCultureMappings =
		new Dictionary<string, (string Male, string Female)>(StringComparer.OrdinalIgnoreCase)
		{
			["German"] = ("German", "German"),
			["Austrian"] = ("German", "German"),
			["Dutch"] = ("Dutch", "Dutch"),
			["French"] = ("French", "French"),
			["Occitan"] = ("French", "French"),
			["English"] = ("English", "English"),
			["Venetian"] = ("Italian", "Italian"),
			["Florentine"] = ("Italian", "Italian"),
			["Neapolitan"] = ("Italian", "Italian"),
			["Milanese"] = ("Italian", "Italian"),
			["Sicilian"] = ("Italian", "Italian"),
			["Corsican"] = ("Italian", "Italian"),
			["Sardinian"] = ("Italian", "Italian"),
			["Castilian"] = ("Iberian", "Iberian"),
			["Catalan"] = ("Iberian", "Iberian"),
			["Galicians"] = ("Iberian", "Iberian"),
			["Portugese"] = ("Iberian", "Iberian"),
			["Basque"] = ("Basque", "Basque"),
			["Ashkenazi Jewish"] = ("Jewish Male", "Jewish Female"),
			["Mizrahi Jewish"] = ("Jewish Male", "Jewish Female"),
			["Sephardic Jewish"] = ("Jewish Male", "Jewish Female"),
			["Gaelic"] = ("Irish", "Irish"),
			["Welsh"] = ("Welsh", "Welsh"),
			["Breton"] = ("Breton", "Breton"),
			["Polish"] = ("Polish", "Polish"),
			["Czech"] = ("Western Slavic", "Western Slavic"),
			["Slovak"] = ("Western Slavic", "Western Slavic"),
			["Ruthenian"] = ("Eastern Slavic", "Eastern Slavic"),
			["Ukrainian"] = ("Eastern Slavic", "Eastern Slavic"),
			["Russian"] = ("Eastern Slavic", "Eastern Slavic"),
			["Croat"] = ("Western Slavic", "Western Slavic"),
			["Serb"] = ("Western Slavic", "Western Slavic"),
			["Bosniak"] = ("Western Slavic", "Western Slavic"),
			["Vlach"] = ("Western Slavic", "Western Slavic"),
			["Lithuanian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Estonian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Latvian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Prussian"] = ("Finno-Ugric", "Finno-Ugric"),
			["Hungarian"] = ("Hungarian", "Hungarian"),
			["Norse"] = ("Danish", "Danish"),
			["Danish"] = ("Danish", "Danish"),
			["Swedish"] = ("Swedish", "Swedish"),
			["Icelandic"] = ("Danish", "Danish"),
			["Roman"] = ("Hellenic", "Hellenic"),
			["Ottoman"] = ("Turkish", "Turkish"),
			["Cossack"] = ("Turkish", "Turkish"),
			["Arabic"] = ("Levantine", "Levantine"),
			["Persian"] = ("Persian", "Persian"),
			["Moorish"] = ("Morrocan", "Morrocan"),
			["North African"] = ("Morrocan", "Morrocan")
		};

	private static readonly IReadOnlyDictionary<string, (string Male, string Female)> ModernEthnicityNameCultureMappings =
		CreateModernEthnicityNameCultureMappings();

	private static readonly IReadOnlyList<string> FallbackGivenNames =
		["Alex", "Sam", "Jamie", "Jordan", "Morgan", "Casey", "Riley", "Taylor"];

	private static readonly IReadOnlyList<string> FallbackSurnames =
		["Smith", "Brown", "Carter", "Clarke", "Stone", "Rivers"];

	private static readonly IReadOnlyList<string> FallbackPatronyms =
		["Alexson", "Jamieson", "Jordanson", "Morganson", "Samson"];

	private static readonly IReadOnlyList<string> FallbackToponyms =
		["of Ash", "of Brookside", "of Rivercross", "of Stoneford", "of Westhaven"];

	private sealed record NameCultureElementSeed(NameUsage Usage, int MinimumCount, int MaximumCount);

	private static IReadOnlyDictionary<string, (string Male, string Female)> CreateModernEthnicityNameCultureMappings()
	{
		Dictionary<string, (string Male, string Female)> mappings = new(StringComparer.OrdinalIgnoreCase)
		{
			["Han"] = ("Modern Chinese", "Modern Chinese"),
			["Dravidian"] = ("Modern Indian", "Modern Indian"),
			["Indo-Aryan"] = ("Modern Indian", "Modern Indian")
		};

		foreach (ModernEthnicityProfileSeed seed in ModernEthnicityProfileSeeds)
		{
			mappings[seed.EthnicityName] = (seed.CultureName, seed.CultureName);
		}

		return mappings;
	}

	private void ApplyEthnicityNameCultureMappings(
		IReadOnlyDictionary<string, (string Male, string Female)> mappings)
	{
		foreach ((string ethnicityName, (string maleCulture, string femaleCulture)) in mappings)
		{
			Ethnicity? ethnicity = _context.Ethnicities.FirstOrDefault(x => x.Name == ethnicityName);
			if (ethnicity is null)
			{
				continue;
			}

			ReplaceEthnicityNameLinks(
				ethnicity,
				(Gender.Male, maleCulture),
				(Gender.Female, femaleCulture),
				(Gender.Neuter, maleCulture),
				(Gender.NonBinary, femaleCulture),
				(Gender.Indeterminate, maleCulture));
		}
	}

	private void ApplyMedievalEuropeEthnicityNameCultureMappings()
	{
		ApplyEthnicityNameCultureMappings(MedievalEuropeEthnicityNameCultureMappings);
	}

	private void ApplyModernEthnicityNameCultureMappings()
	{
		ApplyEthnicityNameCultureMappings(ModernEthnicityNameCultureMappings);
	}

	private void EnsureFallbackRandomNameProfiles()
	{
		foreach (NameCulture culture in _context.NameCultures.ToList())
		{
			if (HasReadyRandomNameProfile(culture))
			{
				continue;
			}

			EnsureFallbackRandomNameProfile(culture);
		}
	}

	private bool HasReadyRandomNameProfile(NameCulture culture)
	{
		foreach (RandomNameProfile profile in _context.RandomNameProfiles.Where(x => x.NameCultureId == culture.Id).ToList())
		{
			List<int> usages = _context.RandomNameProfilesDiceExpressions
				.Where(x => x.RandomNameProfileId == profile.Id)
				.Select(x => x.NameUsage)
				.Distinct()
				.ToList();

			if (usages.Count == 0)
			{
				continue;
			}

			if (usages.All(usage => _context.RandomNameProfilesElements.Any(x =>
					x.RandomNameProfileId == profile.Id && x.NameUsage == usage)))
			{
				return true;
			}
		}

		return false;
	}

	private void EnsureFallbackRandomNameProfile(NameCulture culture)
	{
		List<NameCultureElementSeed> elements = ParseNameCultureElements(culture.Definition);
		if (elements.Count == 0)
		{
			return;
		}

		RandomNameProfile profile = EnsureRandomNameProfile($"{culture.Name} Fallback", Gender.NonBinary, culture);
		foreach (NameCultureElementSeed element in elements)
		{
			AddRandomNameDice(profile, element.Usage, BuildFallbackDiceExpression(element.MinimumCount, element.MaximumCount));
			foreach (string name in GetFallbackNames(element.Usage))
			{
				AddRandomNameElement(profile, element.Usage, name, 100);
			}
		}

		_context.SaveChanges();
	}

	private static List<NameCultureElementSeed> ParseNameCultureElements(string definition)
	{
		XElement root = XElement.Parse(definition);
		return root.Element("Elements")?
			.Elements("Element")
			.Select(x => new NameCultureElementSeed(
				(NameUsage)int.Parse(x.Attribute("Usage")!.Value),
				int.Parse(x.Attribute("MinimumCount")!.Value),
				int.Parse(x.Attribute("MaximumCount")!.Value)))
			.ToList() ?? [];
	}

	private static string BuildFallbackDiceExpression(int minimumCount, int maximumCount)
	{
		if (minimumCount == maximumCount)
		{
			return minimumCount.ToString();
		}

		if (minimumCount == 0)
		{
			return $"1d{maximumCount + 1}-1";
		}

		return $"1d{maximumCount - minimumCount + 1}+{minimumCount - 1}";
	}

	private static IReadOnlyList<string> GetFallbackNames(NameUsage usage)
	{
		return usage switch
		{
			NameUsage.Surname => FallbackSurnames,
			NameUsage.Patronym => FallbackPatronyms,
			NameUsage.Matronym => FallbackPatronyms,
			NameUsage.Toponym => FallbackToponyms,
			_ => FallbackGivenNames
		};
	}
}
