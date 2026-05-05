using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HumanSeederWearProfileTests
{
	private sealed record ParsedWearPart(
		string Name,
		bool Mandatory,
		bool NoArmour,
		bool Transparent,
		bool PreventsRemoval,
		bool HidesSevered);

	private sealed record ParsedDirectWearProfile(string Name, IReadOnlyDictionary<string, ParsedWearPart> Parts);

	[TestMethod]
	public void AdditionalHumanWearProfiles_ExpandCatalogueWithinRequestedRange()
	{
		IReadOnlyList<string> issues = HumanSeeder.ValidateAdditionalHumanWearProfilesForTesting();
		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues));

		int baseline = HumanSeeder.HumanWearProfileBaselineCountForTesting;
		int expansion = HumanSeeder.HumanWearProfileExpansionCountForTesting;
		int minimum = (int)Math.Ceiling(baseline * 0.10);
		int maximum = (int)Math.Floor(baseline * 0.20);

		Assert.IsTrue(expansion >= minimum,
			$"Expected at least {minimum} new wear profiles for a 10% expansion of {baseline}.");
		Assert.IsTrue(expansion <= maximum,
			$"Expected no more than {maximum} new wear profiles for a 20% expansion of {baseline}.");

		List<string> names = HumanSeeder.AdditionalHumanWearProfileNamesForTesting.ToList();
		string[] expectedNames =
		[
			"Headband",
			"Turban",
			"Veil",
			"Blindfold",
			"Gag",
			"Long Coat",
			"Robe",
			"Tabard",
			"Apron",
			"Poncho",
			"Leggings",
			"Tights",
			"Loincloth",
			"Fingerless Gloves",
			"Mittens",
			"Backplate",
			"Sabatons",
			"Sash",
			"Bandolier",
			"Armlet",
			"Toe Ring"
		];

		CollectionAssert.AreEquivalent(expectedNames, names);
	}

	[TestMethod]
	public void AdditionalHumanWearProfiles_ReferenceKnownBodypartsAndShapes()
	{
		string source = ReadHumanBodypartSource();
		HashSet<string> bodyparts = Regex.Matches(source, @"CreateBodypart\(baseHumanoid,\s*""(?<name>[^""]+)""")
		                                  .Cast<Match>()
		                                  .Select(x => x.Groups["name"].Value)
		                                  .ToHashSet(StringComparer.OrdinalIgnoreCase);
		HashSet<string> shapes = Regex.Matches(source, @"AddShape\(""(?<name>[^""]+)""\)")
		                              .Cast<Match>()
		                              .Select(x => x.Groups["name"].Value)
		                              .ToHashSet(StringComparer.OrdinalIgnoreCase);

		List<string> unknownLocations = new();
		foreach ((string name, string type, IReadOnlyList<(string Location, int Count)> locations) in
		         HumanSeeder.AdditionalHumanWearProfileDefinitionsForTesting)
		{
			HashSet<string> validLocations = type == "Direct" ? bodyparts : shapes;
			foreach ((string location, int _) in locations)
			{
				if (!validLocations.Contains(location))
				{
					unknownLocations.Add($"{name} references unknown {type.ToLowerInvariant()} location {location}.");
				}
			}
		}

		Assert.AreEqual(0, unknownLocations.Count, string.Join(Environment.NewLine, unknownLocations));
	}

	[TestMethod]
	public void AdditionalHumanWearComponents_DefineBandanaAsMultiProfileComponent()
	{
		IReadOnlyList<string> issues = HumanSeeder.ValidateAdditionalHumanWearComponentsForTesting();
		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues));

		var bandana = HumanSeeder.AdditionalHumanWearComponentDefinitionsForTesting
		                         .Single(x => x.Name == "Wear_Bandana");

		Assert.AreEqual("Headband", bandana.DefaultProfileName);
		CollectionAssert.AreEqual(
			new[] { "Headband", "Kerchief", "Armlet" },
			bandana.ProfileNames.ToArray());
	}

	[TestMethod]
	public void AuthoredDirectWearProfiles_DoNotHaveUnexplainedLeftRightFlagDrift()
	{
		Dictionary<string, ParsedDirectWearProfile> profiles = ParseDirectWearProfiles(ReadHumanBodypartSource());
		(string Right, string Left)[] pairs =
		[
			("rbreast", "lbreast"),
			("rnipple", "lnipple"),
			("rshoulder", "lshoulder"),
			("rshoulderblade", "lshoulderblade"),
			("rupperarm", "lupperarm"),
			("relbow", "lelbow"),
			("rforearm", "lforearm"),
			("rwrist", "lwrist"),
			("rhand", "lhand"),
			("rthumb", "lthumb"),
			("rindexfinger", "lindexfinger"),
			("rmiddlefinger", "lmiddlefinger"),
			("rringfinger", "lringfinger"),
			("rpinkyfinger", "lpinkyfinger"),
			("rhip", "lhip"),
			("rbuttock", "lbuttock"),
			("rthigh", "lthigh"),
			("rthighback", "lthighback"),
			("rknee", "lknee"),
			("rkneeback", "lkneeback"),
			("rshin", "lshin"),
			("rcalf", "lcalf"),
			("rankle", "lankle"),
			("rheel", "lheel"),
			("rfoot", "lfoot"),
			("reyesocket", "leyesocket"),
			("reye", "leye"),
			("rbrow", "lbrow"),
			("rcheek", "lcheek"),
			("rtemple", "ltemple"),
			("rear", "lear")
		];

		List<string> issues = new();
		foreach (ParsedDirectWearProfile profile in profiles.Values)
		{
			foreach ((string right, string left) in pairs)
			{
				if (!profile.Parts.TryGetValue(right, out ParsedWearPart rightPart) ||
				    !profile.Parts.TryGetValue(left, out ParsedWearPart leftPart))
				{
					continue;
				}

				if (rightPart.Mandatory != leftPart.Mandatory ||
				    rightPart.NoArmour != leftPart.NoArmour ||
				    rightPart.Transparent != leftPart.Transparent ||
				    rightPart.PreventsRemoval != leftPart.PreventsRemoval ||
				    rightPart.HidesSevered != leftPart.HidesSevered)
				{
					issues.Add($"{profile.Name} has mismatched flags for {right}/{left}.");
				}
			}
		}

		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues));
	}

	[TestMethod]
	public void AuthoredWearProfiles_FixKnownTruthfulnessIssues()
	{
		string source = ReadHumanBodypartSource();
		Dictionary<string, ParsedDirectWearProfile> directProfiles = ParseDirectWearProfiles(source);

		Assert.IsTrue(directProfiles["Eyes"].Parts["reye"].Transparent,
			"Contact lens style eye wear should leave the right eye visible.");
		Assert.IsTrue(directProfiles["Eyes"].Parts["leye"].Transparent,
			"Contact lens style eye wear should leave the left eye visible.");

		ParsedDirectWearProfile plackart = directProfiles["Plackart"];
		Assert.IsTrue(plackart.Parts.ContainsKey("belly"));
		Assert.IsTrue(plackart.Parts.ContainsKey("abdomen"));
		Assert.IsFalse(plackart.Parts.ContainsKey("uback"),
			"A plackart is lower front torso armour; back protection should use Backplate.");

		List<string> genitalIssues = directProfiles.Values
		                                           .Where(x => x.Parts.ContainsKey("penis") &&
		                                                       x.Parts.ContainsKey("testicles") &&
		                                                       x.Parts["penis"].Transparent !=
		                                                       x.Parts["testicles"].Transparent)
		                                           .Select(x => x.Name)
		                                           .ToList();
		Assert.AreEqual(0, genitalIssues.Count,
			"Profiles should not hide the penis while making testicles transparent: " +
			string.Join(", ", genitalIssues));

		Match lipRing = Regex.Match(source,
			@"AddWearProfileShape\(""Lip Ring"",\s*""worn in"",\s*""put"",\s*""puts"",\s*""in"",\s*""Inserted into a lip piercing""(?<body>.*?)\);",
			RegexOptions.Singleline);
		Assert.IsTrue(lipRing.Success, "Lip Ring should describe a lip piercing with runtime-grammatical wear text.");
		StringAssert.Contains(lipRing.Groups["body"].Value, "(\"mouth\", 1");
		Assert.IsFalse(lipRing.Groups["body"].Value.Contains("\"eyebrow\"", StringComparison.OrdinalIgnoreCase),
			"Lip Ring should not target the eyebrow shape.");
	}

	private static Dictionary<string, ParsedDirectWearProfile> ParseDirectWearProfiles(string source)
	{
		Regex profileRegex = new(@"AddWearProfileDirect\(""(?<name>[^""]+)""(?<body>.*?)\);",
			RegexOptions.Singleline);
		Regex partRegex = new(
			@"\(""(?<part>[^""]+)"",\s*(?<count>\d+),\s*Mandatory:\s*(?<mandatory>true|false),\s*NoArmour:\s*(?<noarmour>true|false),\s*Transparent:\s*(?<transparent>true|false),\s*PreventsRemoval:\s*(?<prevents>true|false),\s*HidesSevered:\s*(?<hides>true|false)\)",
			RegexOptions.Singleline | RegexOptions.IgnoreCase);

		return profileRegex.Matches(source)
		                   .Cast<Match>()
		                   .Select(match => new ParsedDirectWearProfile(
			                   match.Groups["name"].Value,
			                   partRegex.Matches(match.Groups["body"].Value)
			                            .Cast<Match>()
			                            .Select(part => new ParsedWearPart(
				                            part.Groups["part"].Value,
				                            bool.Parse(part.Groups["mandatory"].Value),
				                            bool.Parse(part.Groups["noarmour"].Value),
				                            bool.Parse(part.Groups["transparent"].Value),
				                            bool.Parse(part.Groups["prevents"].Value),
				                            bool.Parse(part.Groups["hides"].Value)))
			                            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			                            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase)))
		                   .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
		                   .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
	}

	private static string ReadHumanBodypartSource()
	{
		return File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "HumanSeederBodyparts.cs"));
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
