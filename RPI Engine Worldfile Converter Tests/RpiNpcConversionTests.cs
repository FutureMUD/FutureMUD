#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Form.Shape;
using RPI_Engine_Worldfile_Converter;

namespace RPI_Engine_Worldfile_Converter_Tests;

[TestClass]
public class RpiNpcConversionTests
{
	[TestMethod]
	public void NpcParser_ReadsFixtureCorpus_AndCapturesTypedSidecars()
	{
		var parser = new RpiNpcWorldfileParser();
		var corpus = parser.ParseDirectory(GetNpcFixtureDirectory());

		Assert.AreEqual(7, corpus.Npcs.Count);
		Assert.AreEqual(0, corpus.Failures.Count);

		var variableCommoner = corpus.Npcs.Single(x => x.Vnum == 1001);
		Assert.IsTrue(variableCommoner.Flags.HasFlag(RpiNpcFlags.Variable));
		Assert.AreEqual(1, variableCommoner.Skills.Count);

		var shopkeeper = corpus.Npcs.Single(x => x.Vnum == 1002);
		Assert.IsNotNull(shopkeeper.Shop);
		Assert.AreEqual(100, shopkeeper.Shop!.ShopVnum);
		Assert.AreEqual(1, shopkeeper.ClanMemberships.Count);

		var spider = corpus.Npcs.Single(x => x.Vnum == 1003);
		Assert.IsNotNull(spider.Venom);
		Assert.IsTrue(spider.ActFlags.HasFlag(RpiNpcActFlags.Venom));

		var vehicle = corpus.Npcs.Single(x => x.Vnum == 1004);
		Assert.IsTrue(vehicle.ActFlags.HasFlag(RpiNpcActFlags.Vehicle));

		var warg = corpus.Npcs.Single(x => x.Vnum == 1006);
		Assert.IsNotNull(warg.Morph);
		Assert.AreEqual(1007, warg.Morph!.MorphTo);
	}

	[TestMethod]
	public void NpcTransformer_MapsFixtureCorpus_ToExpectedTemplateKindsRacesAndWarnings()
	{
		var parser = new RpiNpcWorldfileParser();
		var corpus = parser.ParseDirectory(GetNpcFixtureDirectory());
		var transformer = new FutureMudNpcTransformer(new Dictionary<int, (string GroupKey, string ZoneName)>
		{
			[1] = ("minas-morgul", "Minas Morgul")
		});
		var conversion = transformer.Convert(corpus.Npcs);
		var npcs = conversion.Npcs.ToDictionary(x => x.Vnum);

		var aragorn = npcs[1000];
		Assert.AreEqual(NpcConversionStatus.Ready, aragorn.Status);
		Assert.AreEqual(NpcTemplateKind.Simple, aragorn.TemplateKind);
		Assert.AreEqual("Human", aragorn.RaceName);
		Assert.AreEqual("Gondorian Dunedain", aragorn.EthnicityName);
		Assert.AreEqual("Gondorian", aragorn.CultureName);
		Assert.AreEqual("Aragorn", aragorn.TechnicalName);
		Assert.IsTrue(aragorn.UsesSourceDerivedName);

		var commoner = npcs[1001];
		Assert.AreEqual(NpcTemplateKind.Variable, commoner.TemplateKind);
		Assert.AreEqual(NpcConversionStatus.Ready, commoner.Status);
		Assert.IsFalse(commoner.UsesSourceDerivedName);
		Assert.AreEqual(2, commoner.GenderChances.Count);

		var merchant = npcs[1002];
		Assert.AreEqual(RpiNpcClassification.Merchant, merchant.Classification);
		Assert.IsTrue(merchant.HasShopData);
		Assert.IsTrue(merchant.Warnings.Any(x => x.Code == "deferred-shop-data"));
		Assert.IsTrue(merchant.Warnings.Any(x => x.Code == "deferred-clan-membership"));

		var spider = npcs[1003];
		Assert.AreEqual("Giant Spider", spider.RaceName);
		Assert.AreEqual("Animal", spider.CultureName);
		Assert.IsTrue(spider.Warnings.Any(x => x.Code == "deferred-venom"));

		var vehicle = npcs[1004];
		Assert.AreEqual(NpcConversionStatus.Deferred, vehicle.Status);
		Assert.AreEqual(RpiNpcClassification.Vehicle, vehicle.Classification);
		Assert.IsTrue(vehicle.Warnings.Any(x => x.Code == "deferred-vehicle"));

		var orc = npcs[1005];
		CollectionAssert.Contains(orc.ArtificialIntelligenceNames.ToList(), "TrackingAggressiveToAllOtherSpecies");
		Assert.AreEqual("Orc", orc.RaceName);
		Assert.AreEqual("Uruk", orc.EthnicityName);
		Assert.AreEqual("Mordorian Orc", orc.CultureName);
		Assert.IsTrue(orc.Warnings.Any(x => x.Code == "memory-approximated-as-tracking"));
		Assert.IsTrue(orc.Warnings.Any(x => x.Code == "deferred-enforcer"));

		var warg = npcs[1006];
		Assert.AreEqual("Warg", warg.RaceName);
		Assert.AreEqual("Animal", warg.CultureName);
		Assert.IsTrue(warg.HasMorphData);
		Assert.IsTrue(warg.Warnings.Any(x => x.Code == "deferred-morph"));
	}

	[TestMethod]
	public void NpcValidation_UsesFixtureBaseline_WithoutUnexpectedErrors()
	{
		var parser = new RpiNpcWorldfileParser();
		var corpus = parser.ParseDirectory(GetNpcFixtureDirectory());
		var transformer = new FutureMudNpcTransformer(new Dictionary<int, (string GroupKey, string ZoneName)>
		{
			[1] = ("minas-morgul", "Minas Morgul")
		});
		var conversion = transformer.Convert(corpus.Npcs);
		var baseline = BuildBaseline();

		var issues = FutureMudNpcValidation.Validate(baseline, conversion.Npcs);

		Assert.IsFalse(
			issues.Any(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)),
			string.Join(Environment.NewLine, issues.Select(x => $"[{x.Severity}] {x.SourceKey}: {x.Message}")));
		Assert.IsTrue(issues.Any(x => x.SourceKey == conversion.Npcs.Single(y => y.Vnum == 1004).SourceKey &&
		                              x.Severity.Equals("warning", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void NpcTransformer_MapsWraiths_ToSpiritCourtCulture()
	{
		var transformer = new FutureMudNpcTransformer();
		var conversion = transformer.Convert(
		[
			BuildMinimalNpc(2000, "wraith spectre", "a skeletal wraith-like being")
		]);

		var wraith = conversion.Npcs.Single();

		Assert.AreEqual(NpcConversionStatus.Ready, wraith.Status);
		Assert.AreEqual("Wraith", wraith.RaceName);
		Assert.AreEqual("Wraith", wraith.EthnicityName);
		Assert.AreEqual("Spirit Court", wraith.CultureName);
	}

	[TestMethod]
	public void NpcTransformer_MapsStructuredMountCreatures_ToBasicMountAi()
	{
		var transformer = new FutureMudNpcTransformer();
		var conversion = transformer.Convert(
		[
			BuildMinimalNpc(2100, "bay horse mount", "a bay horse") with
			{
				RawActFlags = (long)RpiNpcActFlags.Mount,
				ActFlags = RpiNpcActFlags.Mount,
				LegacyRaceId = 12
			},
			BuildMinimalNpc(2101, "grey donkey mount", "a grey donkey") with
			{
				RawActFlags = (long)RpiNpcActFlags.Mount,
				ActFlags = RpiNpcActFlags.Mount,
				LegacyRaceId = 20
			},
			BuildMinimalNpc(2102, "sure footed mule mount", "a sure-footed mule") with
			{
				RawActFlags = (long)RpiNpcActFlags.Mount,
				ActFlags = RpiNpcActFlags.Mount,
				LegacyRaceId = 19
			},
			BuildMinimalNpc(2103, "war warg wolfspawn mount", "a massive warg") with
			{
				RawActFlags = (long)RpiNpcActFlags.Mount,
				ActFlags = RpiNpcActFlags.Mount,
				LegacyRaceId = 21
			}
		]);

		foreach (var npc in conversion.Npcs)
		{
			Assert.AreEqual(NpcConversionStatus.Ready, npc.Status);
			CollectionAssert.Contains(npc.ArtificialIntelligenceNames.ToList(), "BasicMount");
			Assert.IsFalse(npc.Warnings.Any(x => x.Code == "legacy-mount-unmapped"));
		}
	}

	[TestMethod]
	public void NpcTransformer_WarnsForUnsupportedMountAndPackAnimalFlags()
	{
		var transformer = new FutureMudNpcTransformer();
		var conversion = transformer.Convert(
		[
			BuildMinimalNpc(2110, "guard ACT_MOUNT", "a mounted guard") with
			{
				RawActFlags = (long)RpiNpcActFlags.Mount,
				ActFlags = RpiNpcActFlags.Mount
			},
			BuildMinimalNpc(2111, "pack horse", "a pack horse") with
			{
				RawActFlags = (long)RpiNpcActFlags.PackAnimal,
				ActFlags = RpiNpcActFlags.PackAnimal,
				LegacyRaceId = 12
			}
		]);

		var unsupportedMount = conversion.Npcs.Single(x => x.Vnum == 2110);
		Assert.AreEqual(NpcConversionStatus.Ready, unsupportedMount.Status);
		CollectionAssert.DoesNotContain(unsupportedMount.ArtificialIntelligenceNames.ToList(), "BasicMount");
		Assert.IsTrue(unsupportedMount.Warnings.Any(x => x.Code == "legacy-mount-unmapped"));

		var packAnimal = conversion.Npcs.Single(x => x.Vnum == 2111);
		Assert.AreEqual(NpcConversionStatus.Ready, packAnimal.Status);
		CollectionAssert.DoesNotContain(packAnimal.ArtificialIntelligenceNames.ToList(), "BasicMount");
		CollectionAssert.Contains(packAnimal.DeferredBehaviorFlags.ToList(), "packanimal");
		Assert.IsTrue(packAnimal.Warnings.Any(x => x.Code == "legacy-pack-animal"));
	}

	private static FutureMudNpcBaselineCatalog BuildBaseline()
	{
		return new FutureMudNpcBaselineCatalog
		{
			BuilderAccountId = 1,
			Races = new Dictionary<string, FutureMudNpcRaceReference>(StringComparer.OrdinalIgnoreCase)
			{
				["Human"] = new FutureMudNpcRaceReference(
					1,
					"Human",
					null,
					1,
					10,
					20,
					25,
					65,
					(int)Alignment.Right,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = 101,
						[Gender.Female] = 102,
						[Gender.Neuter] = 101,
						[Gender.NonBinary] = 101,
					},
					[11, 12]),
				["Orc"] = new FutureMudNpcRaceReference(
					2,
					"Orc",
					null,
					2,
					10,
					20,
					18,
					45,
					(int)Alignment.Right,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = 201,
						[Gender.Female] = 202,
						[Gender.Neuter] = 201,
					},
					[21]),
				["Giant Spider"] = new FutureMudNpcRaceReference(
					3,
					"Giant Spider",
					null,
					3,
					10,
					20,
					8,
					20,
					(int)Alignment.Irrelevant,
					new Dictionary<Gender, long>
					{
						[Gender.Neuter] = 301,
					},
					[31]),
				["Warg"] = new FutureMudNpcRaceReference(
					4,
					"Warg",
					null,
					2,
					10,
					20,
					10,
					25,
					(int)Alignment.Right,
					new Dictionary<Gender, long>
					{
						[Gender.Neuter] = 401,
					},
					[41]),
			},
			Ethnicities = new Dictionary<string, FutureMudNpcEthnicityReference>(StringComparer.OrdinalIgnoreCase)
			{
				["Gondorian"] = new FutureMudNpcEthnicityReference(
					11,
					"Gondorian",
					1,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = 501,
						[Gender.Female] = 501,
						[Gender.Neuter] = 501,
						[Gender.NonBinary] = 501,
					}),
				["Gondorian Dunedain"] = new FutureMudNpcEthnicityReference(
					12,
					"Gondorian Dunedain",
					1,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = 501,
						[Gender.Female] = 501,
						[Gender.Neuter] = 501,
						[Gender.NonBinary] = 501,
					}),
				["Uruk"] = new FutureMudNpcEthnicityReference(
					21,
					"Uruk",
					2,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = 601,
						[Gender.Female] = 601,
						[Gender.Neuter] = 601,
					}),
				["Giant Spider"] = new FutureMudNpcEthnicityReference(
					31,
					"Giant Spider",
					3,
					new Dictionary<Gender, long>
					{
						[Gender.Neuter] = 701,
					}),
				["Warg"] = new FutureMudNpcEthnicityReference(
					41,
					"Warg",
					4,
					new Dictionary<Gender, long>
					{
						[Gender.Neuter] = 801,
					}),
			},
			Cultures = new Dictionary<string, FutureMudNpcCultureReference>(StringComparer.OrdinalIgnoreCase)
			{
				["Gondorian"] = new FutureMudNpcCultureReference(
					101,
					"Gondorian",
					1,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = 501,
						[Gender.Female] = 501,
						[Gender.Neuter] = 501,
						[Gender.NonBinary] = 501,
					}),
				["Mordorian Orc"] = new FutureMudNpcCultureReference(
					201,
					"Mordorian Orc",
					1,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = 601,
						[Gender.Female] = 601,
						[Gender.Neuter] = 601,
					}),
				["Animal"] = new FutureMudNpcCultureReference(
					301,
					"Animal",
					1,
					new Dictionary<Gender, long>
					{
						[Gender.Neuter] = 701,
					}),
			},
			TraitIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["Strength"] = 1,
				["Intelligence"] = 2,
				["Willpower"] = 3,
				["Aura"] = 4,
				["Dexterity"] = 5,
				["Constitution"] = 6,
				["Agility"] = 7,
				["Small-Blade"] = 8,
				["Sword"] = 9,
			},
			LanguagesByTraitId = new Dictionary<long, FutureMudNpcLanguageReference>(),
			ArtificialIntelligenceIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["AggressiveToAllOtherSpecies"] = 1,
				["TrackingAggressiveToAllOtherSpecies"] = 2,
				["BasicMount"] = 3,
			},
			NameProfilesByCultureId = new Dictionary<long, Dictionary<Gender, long>>
			{
				[501] = new Dictionary<Gender, long>
				{
					[Gender.Male] = 1001,
					[Gender.Female] = 1002,
					[Gender.Neuter] = 1001,
					[Gender.NonBinary] = 1001,
				},
				[601] = new Dictionary<Gender, long>
				{
					[Gender.Male] = 2001,
					[Gender.Female] = 2002,
					[Gender.Neuter] = 2001,
				},
				[701] = new Dictionary<Gender, long>
				{
					[Gender.Neuter] = 3001,
				},
				[801] = new Dictionary<Gender, long>
				{
					[Gender.Neuter] = 4001,
				},
			},
			CalendarDates = new Dictionary<long, string>
			{
				[1] = "1/January/3000",
			},
		};
	}

	private static string GetNpcFixtureDirectory()
	{
		var candidates = new[]
		{
			Path.Combine(AppContext.BaseDirectory, "Fixtures", "Npcs"),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Fixtures", "Npcs")),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter Tests", "Fixtures", "Npcs"))
		};

		return candidates.First(x => File.Exists(Path.Combine(x, "mobs.1")));
	}

	private static RpiNpcRecord BuildMinimalNpc(int vnum, string keywords, string shortDescription)
	{
		return new RpiNpcRecord
		{
			Vnum = vnum,
			SourceFile = "mobs.0",
			Zone = 0,
			Keywords = keywords,
			ShortDescription = shortDescription,
			LongDescription = $"{shortDescription} is here.",
			FullDescription = $"{shortDescription} lingers here.",
			RawActFlags = 0,
			ActFlags = RpiNpcActFlags.None,
			RawAffectedBy = 0,
			Offense = 0,
			LegacyRaceId = 0,
			Armour = 0,
			HitDiceExpression = "1d1+1",
			DamageDiceExpression = "1d1+0",
			BirthTimestamp = 0,
			Position = 0,
			DefaultPosition = 0,
			Sex = RpiNpcSex.Neutral,
			MerchSeven = 0,
			MaterialsMask = 0,
			VehicleType = 0,
			BuyFlags = 0,
			SkinnedVnum = 0,
			Circle = 0,
			Cell1 = 0,
			CarcassVnum = 0,
			Cell2 = 0,
			PPoints = 0,
			NaturalDelay = 0,
			HelmRoom = 0,
			BodyType = 0,
			PoisonType = 0,
			NaturalAttackType = 0,
			AccessFlags = 0,
			HeightInches = 0,
			Frame = 0,
			NoAccessFlags = 0,
			Cell3 = 0,
			RoomPos = 0,
			Fallback = 0,
			Strength = 10,
			Intelligence = 10,
			Will = 10,
			Aura = 10,
			Dexterity = 10,
			Constitution = 10,
			SpeaksSkillId = 0,
			Agility = 10,
			RawFlags = 0,
			Flags = RpiNpcFlags.None,
			CurrencyType = 0,
			Skills = [],
			ClanMemberships = []
		};
	}
}
