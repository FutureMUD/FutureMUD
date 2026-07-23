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
	public void AdditionalHumanWearProfiles_IncludeMedievalJewelleryDependencyProfiles()
	{
		IReadOnlyList<string> issues = HumanSeeder.ValidateAdditionalHumanWearProfilesForTesting();
		Assert.AreEqual(0, issues.Count, string.Join(Environment.NewLine, issues));

		List<string> names = HumanSeeder.AdditionalHumanWearProfileNamesForTesting.ToList();
		string[] expectedNames =
		[
			"Headband",
			"Brooch",
			"Brooches",
			"Pin",
			"Badge",
			"Hairpin",
			"Hairpins",
			"Hair Comb",
			"Hair Combs",
			"Hair Ornament",
			"Hair Ornaments",
			"Temple Rings",
			"Circlet",
			"Diadem",
			"Coronet",
			"Crown",
			"Chaplet",
			"Wreath",
			"Head Garland",
			"Forehead Ornament",
			"Neck Garland",
			"Torc",
			"Neck Ring",
			"Wrist Garland",
			"Ankle Garland",
			"Waist Chain",
			"Girdle Ornament",
			"Belt Ornament",
			"Belt Plaques",
			"Waist Ornament",
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
			"Tassets",
			"Sash",
			"Bandolier",
			"Armlet",
			"Toe Ring",
			"Leg Wraps",
			"Overshoes",
			"Head Veil",
			"Hood",
			"Detachable Sleeves",
			"Skirt Support",
			"Partlet",
			"Long Open Robe",
			"Stays",
			"Breeches",
			"ArmHarness",
			"LegHarness",
			"ShoulderArmHarness",
			"HalfArmourHarness",
			"ThreeQuarterHarness",
			"FullPlateHarness",
			"Breechcloth"
		];

		CollectionAssert.AreEquivalent(expectedNames, names);

		string[] requiredComponents =
		[
			"Wear_Brooch",
			"Wear_Brooches",
			"Wear_Pin",
			"Wear_Badge",
			"Wear_Hairpin",
			"Wear_Hairpins",
			"Wear_Hair_Comb",
			"Wear_Hair_Combs",
			"Wear_Hair_Ornament",
			"Wear_Hair_Ornaments",
			"Wear_Temple_Rings",
			"Wear_Circlet",
			"Wear_Diadem",
			"Wear_Coronet",
			"Wear_Crown",
			"Wear_Chaplet",
			"Wear_Wreath",
			"Wear_Head_Garland",
			"Wear_Neck_Garland",
			"Wear_Wrist_Garland",
			"Wear_Ankle_Garland",
			"Wear_Torc",
			"Wear_Neck_Ring",
			"Wear_Waist_Chain",
			"Wear_Girdle_Ornament",
			"Wear_Belt_Ornament",
			"Wear_Belt_Plaques",
			"Wear_Waist_Ornament",
			"Wear_Forehead_Ornament"
		];

		HashSet<string> generatedComponentNames = names
		                                         .Select(x => $"Wear_{x.Replace(' ', '_')}")
		                                         .ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (string componentName in requiredComponents)
		{
			Assert.IsTrue(generatedComponentNames.Contains(componentName),
				$"{componentName} should be generated from an additional human wear profile.");
		}
	}

	[TestMethod]
	public void AdditionalHumanWearProfiles_DefineEarlyModernCoverageAndLayering()
	{
		var profiles = HumanSeeder.AdditionalHumanWearProfileLayeringForTesting
			.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		Dictionary<string, string[]> expectedParts = new(StringComparer.OrdinalIgnoreCase)
		{
			["Stays"] = ["rbreast", "lbreast", "uback", "abdomen", "belly", "lback"],
			["Breeches"] =
			[
				"groin", "rhip", "lhip", "rbuttock", "lbuttock", "rthigh", "lthigh", "rthighback",
				"lthighback", "rknee", "lknee"
			],
			["ArmHarness"] = ["rupperarm", "lupperarm", "relbow", "lelbow", "rforearm", "lforearm"],
			["LegHarness"] = ["rthigh", "lthigh", "rknee", "lknee", "rshin", "lshin", "rfoot", "lfoot"],
			["ShoulderArmHarness"] =
			[
				"rshoulder", "lshoulder", "rupperarm", "lupperarm", "relbow", "lelbow", "rforearm",
				"lforearm"
			],
			["HalfArmourHarness"] =
			[
				"rbreast", "lbreast", "uback", "abdomen", "rshoulder", "lshoulder", "rupperarm", "lupperarm",
				"rhip", "lhip"
			],
			["ThreeQuarterHarness"] =
			[
				"rbreast", "lbreast", "uback", "abdomen", "rupperarm", "lupperarm", "relbow", "lelbow",
				"rforearm", "lforearm", "rhand", "lhand", "rhip", "lhip", "rthigh", "lthigh", "rknee",
				"lknee"
			],
			["FullPlateHarness"] =
			[
				"rbreast", "lbreast", "uback", "lback", "abdomen", "belly", "rshoulder", "lshoulder",
				"rupperarm", "lupperarm", "relbow", "lelbow", "rforearm", "lforearm", "rhand", "lhand",
				"rhip", "lhip", "rthigh", "lthigh", "rknee", "lknee", "rshin", "lshin", "rfoot", "lfoot"
			]
		};

		foreach ((string name, string[] expected) in expectedParts)
		{
			Assert.IsTrue(profiles.TryGetValue(name, out var profile),
				$"{name} should be an authored direct wear profile.");
			CollectionAssert.AreEquivalent(expected, profile.Locations.Select(x => x.Location).ToArray(), name);
		}

		Assert.IsTrue(profiles["Stays"].Locations.All(x => x.NoArmour),
			"Stays should remain a non-armour underlayer.");
		Assert.IsTrue(profiles["Breeches"].Locations.All(x => x.NoArmour),
			"Breeches should remain ordinary clothing compatible with separate armour.");
		foreach (string name in expectedParts.Keys.Where(x => x.EndsWith("Harness", StringComparison.Ordinal)))
		{
			Assert.IsTrue(profiles[name].Locations.All(x => !x.NoArmour),
				$"{name} should expose armour-capable coverage.");
			Assert.IsTrue(profiles[name].Bulky, $"{name} should occupy the bulky armour layer.");
		}
		Assert.IsFalse(profiles["Stays"].Bulky, "Stays should fit beneath bulky outer garments.");
		Assert.IsFalse(profiles["Breeches"].Bulky, "Breeches should fit beneath separate leg armour.");

		string source = ReadHumanBodypartSource();
		StringAssert.Contains(source,
			"StockDirectWearProfile(\"Stays\", \"worn beneath\", \"lace\", \"laces\", \"beneath\"");
		StringAssert.Contains(source,
			"StockDirectWearProfile(\"ThreeQuarterHarness\", \"worn over\", \"buckle\", \"buckles\", \"over\"");
		StringAssert.Contains(source,
			"StockDirectWearProfile(\"FullPlateHarness\", \"worn over\", \"buckle\", \"buckles\", \"over\"");
	}

	[TestMethod]
	public void AdditionalHumanWearProfiles_IncludeRenaissanceClothingFoundationProfiles()
	{
		string[] expectedNames =
		[
			"Leg Wraps", "Overshoes", "Head Veil", "Hood", "Detachable Sleeves", "Skirt Support", "Partlet",
			"Long Open Robe", "Breechcloth"
		];

		CollectionAssert.IsSubsetOf(
			expectedNames,
			HumanSeeder.AdditionalHumanWearProfileNamesForTesting.ToArray());
		foreach (string name in expectedNames)
		{
			Assert.IsTrue(
				HumanSeeder.AdditionalHumanWearProfileDefinitionsForTesting.Any(x =>
					x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Locations.Count > 0),
				$"{name} should have concrete body coverage.");
		}
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
		return SeederSourceTestHelper.ReadPartialFamily("HumanSeeder");
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
