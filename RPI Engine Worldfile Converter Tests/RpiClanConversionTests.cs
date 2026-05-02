#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Community;
using RPI_Engine_Worldfile_Converter;

namespace RPI_Engine_Worldfile_Converter_Tests;

[TestClass]
public class RpiClanConversionTests
{
	[TestMethod]
	public void ClanSourceParser_ReadsFixtureSource_AndExtractsAliasMetadata()
	{
		var parser = new RpiClanSourceParser();
		var source = parser.Parse(GetClanSourcePath());

		Assert.AreEqual(9, source.HeaderEntries.Count);
		Assert.AreEqual(0, source.ParseWarnings.Count);
		Assert.IsTrue(source.AliasSources.ContainsKey("gothakra"));
		Assert.IsTrue(source.AliasSources.ContainsKey("seekers"));
		Assert.IsTrue(source.AliasSources.ContainsKey("mordor_char"));

		CollectionAssert.AreEquivalent(
			new[] { "Snaga Uruk", "Snaga" },
			source.AliasSources["gothakra"].DisplayNamesBySlot[RpiClanRankSlot.Recruit].ToArray());
		CollectionAssert.Contains(
			source.AliasSources["seekers"].SynonymsBySlot[RpiClanRankSlot.Recruit].ToList(),
			"squire");
		CollectionAssert.Contains(
			source.AliasSources["mordor_char"].DisplayNamesBySlot[RpiClanRankSlot.Membership].ToList(),
			"Freeman");
	}

	[TestMethod]
	public void ClanTransformer_MapsFixtureCorpus_ToExpectedClansAndPrivileges()
	{
		var sourceParser = new RpiClanSourceParser();
		var source = sourceParser.Parse(GetClanSourcePath());
		var scanner = new RpiClanRegionReferenceScanner();
		var references = scanner.Scan(GetClanRegionsDirectory(), source);
		var transformer = new FutureMudClanTransformer();
		var conversion = transformer.Convert(source, references);
		var converted = conversion.ConvertedClans.ToDictionary(x => x.CanonicalAlias, StringComparer.OrdinalIgnoreCase);

		Assert.IsFalse(conversion.UnresolvedAliasCounts.ContainsKey("com-priests"));

		var ithilien = converted["ithilien_battalion"];
		CollectionAssert.AreEquivalent(
			new[]
			{
				RpiClanRankSlot.Recruit,
				RpiClanRankSlot.Private,
				RpiClanRankSlot.Corporal,
				RpiClanRankSlot.Sergeant,
				RpiClanRankSlot.Lieutenant,
				RpiClanRankSlot.Captain,
			},
			ithilien.Ranks.Select(x => x.Slot).ToArray());
		Assert.IsFalse(ithilien.Ranks.Any(x => x.Slot == RpiClanRankSlot.General));
		Assert.AreEqual("Lord", ithilien.Ranks.Single(x => x.Slot == RpiClanRankSlot.Captain).Name);
		Assert.AreEqual("Sergeant", ithilien.Ranks.Single(x => x.Slot == RpiClanRankSlot.Sergeant).Name);

		var gothakra = converted["gothakra"];
		Assert.IsTrue(gothakra.Ranks.Any(x => x.Slot == RpiClanRankSlot.Lieutenant));
		Assert.IsFalse(gothakra.Ranks.Any(x => x.Slot == RpiClanRankSlot.Captain));

		var khagdu = converted["khagdu"];
		CollectionAssert.IsSubsetOf(
			new[]
			{
				RpiClanRankSlot.Recruit,
				RpiClanRankSlot.Private,
				RpiClanRankSlot.Corporal,
				RpiClanRankSlot.Sergeant,
				RpiClanRankSlot.Lieutenant,
				RpiClanRankSlot.Captain,
				RpiClanRankSlot.General,
				RpiClanRankSlot.Commander,
				RpiClanRankSlot.Apprentice,
				RpiClanRankSlot.Journeyman,
				RpiClanRankSlot.Master,
			},
			khagdu.Ranks.Select(x => x.Slot).ToArray());

		var mordor = converted["mordor_char"];
		Assert.AreEqual("Minas Morgul", mordor.FullName);
		CollectionAssert.AreEquivalent(
			new[]
			{
				RpiClanRankSlot.Membership,
				RpiClanRankSlot.Apprentice,
				RpiClanRankSlot.Journeyman,
				RpiClanRankSlot.Master,
			},
			mordor.Ranks.Select(x => x.Slot).ToArray());

		var malred = converted["malred"];
		Assert.AreEqual("Malred Family", malred.FullName);
		CollectionAssert.AreEquivalent(
			new[] { RpiClanRankSlot.Journeyman },
			malred.Ranks.Select(x => x.Slot).ToArray());

		var tirithGuard = converted["tirithguard"];
		Assert.AreEqual("Minas Tirith Guard", tirithGuard.FullName);

		var hawkAndDove = converted["hawk_dove_2"];
		CollectionAssert.Contains(hawkAndDove.LegacyAliases.ToList(), "hawk_and_dove");

		var healers = converted["healers"];
		Assert.AreEqual("Healers", healers.FullName);
		CollectionAssert.AreEquivalent(
			new[] { RpiClanRankSlot.Apprentice },
			healers.Ranks.Select(x => x.Slot).ToArray());

		var priests = converted["com_priests"];
		Assert.AreEqual("Cult of Morgoth Priests", priests.FullName);
		CollectionAssert.AreEquivalent(
			new[] { RpiClanRankSlot.Membership },
			priests.Ranks.Select(x => x.Slot).ToArray());

		var sergeant = ithilien.Ranks.Single(x => x.Slot == RpiClanRankSlot.Sergeant);
		var captain = ithilien.Ranks.Single(x => x.Slot == RpiClanRankSlot.Captain);
		var membership = mordor.Ranks.Single(x => x.Slot == RpiClanRankSlot.Membership);
		var master = mordor.Ranks.Single(x => x.Slot == RpiClanRankSlot.Master);

		Assert.IsTrue(((ClanPrivilegeType)sergeant.Privileges).HasFlag(ClanPrivilegeType.CanInduct));
		Assert.IsTrue(((ClanPrivilegeType)sergeant.Privileges).HasFlag(ClanPrivilegeType.CanPromote));
		Assert.IsTrue(((ClanPrivilegeType)sergeant.Privileges).HasFlag(ClanPrivilegeType.CanDemote));
		Assert.AreEqual((long)ClanPrivilegeType.All, captain.Privileges);
		Assert.AreNotEqual((long)ClanPrivilegeType.All, membership.Privileges);
		Assert.AreEqual((long)ClanPrivilegeType.All, master.Privileges);

		var validation = FutureMudClanValidation.Validate(
			new FutureMudClanBaselineCatalog
			{
				CalendarId = 1,
				PayIntervalReferenceDate = "1/1/2000",
				PayIntervalReferenceTime = "Primary 0:0:0",
			},
			conversion.ConvertedClans);
		Assert.IsFalse(validation.Any(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void ClanTransformer_CollapsesAliasPunctuationVariants()
	{
		var source = new RpiClanSourceDocument(
			"synthetic",
			[
				new RpiClanHeaderEntry(1, "Black Watch", "black-watch"),
				new RpiClanHeaderEntry(2, "Black Watch", "black_watch")
			],
			new Dictionary<string, RpiClanAliasSource>(StringComparer.OrdinalIgnoreCase),
			Array.Empty<string>());
		var references = new RpiClanReferenceIndex(
			new Dictionary<string, RpiClanReferenceRecord>(StringComparer.OrdinalIgnoreCase)
			{
				["blackwatch"] = new(
					"blackwatch",
					3,
					[RpiClanRankSlot.Membership],
					Array.Empty<string>())
			});

		var conversion = new FutureMudClanTransformer().Convert(source, references);
		var blackWatch = conversion.ConvertedClans.Single(x => x.CanonicalAlias.Equals("blackwatch", StringComparison.OrdinalIgnoreCase));

		Assert.AreEqual("blackwatch", blackWatch.CanonicalAlias);
		Assert.AreEqual("Black Watch", blackWatch.FullName);
		CollectionAssert.AreEquivalent(
			new[] { "black-watch", "black_watch" },
			blackWatch.LegacyAliases.Where(x => !x.Equals("blackwatch", StringComparison.OrdinalIgnoreCase)).ToArray());
		CollectionAssert.AreEquivalent(
			new[] { RpiClanRankSlot.Membership },
			blackWatch.Ranks.Select(x => x.Slot).ToArray());
	}

	[TestMethod]
	public void ClanTransformer_UsesKnownAliasRepairRules_ForBuilderTypos()
	{
		var source = new RpiClanSourceDocument(
			"synthetic",
			Array.Empty<RpiClanHeaderEntry>(),
			new Dictionary<string, RpiClanAliasSource>(StringComparer.OrdinalIgnoreCase),
			Array.Empty<string>());
		var references = new RpiClanReferenceIndex(
			new Dictionary<string, RpiClanReferenceRecord>(StringComparer.OrdinalIgnoreCase)
			{
				["tecouncil"] = new("tecouncil", 11, Array.Empty<RpiClanRankSlot>(), Array.Empty<string>()),
				["metalsmithsfellow"] = new("metalsmithsfellow", 6, Array.Empty<RpiClanRankSlot>(), Array.Empty<string>()),
				["mm_slaves"] = new("mm_slaves", 2, Array.Empty<RpiClanRankSlot>(), Array.Empty<string>()),
				["mt_ratcatchers"] = new("mt_ratcatchers", 2, Array.Empty<RpiClanRankSlot>(), Array.Empty<string>()),
				["osgi_ratcatchers"] = new("osgi_ratcatchers", 2, Array.Empty<RpiClanRankSlot>(), Array.Empty<string>()),
				["witchking_horse"] = new("witchking_horse", 1, [RpiClanRankSlot.Membership], Array.Empty<string>()),
				["withchking_horde"] = new("withchking_horde", 1, [RpiClanRankSlot.Membership], Array.Empty<string>()),
				["pel_pelenor"] = new("pel_pelenor", 1, [RpiClanRankSlot.Membership], Array.Empty<string>()),
				["osgi_citizensmember"] = new("osgi_citizensmember", 1, [RpiClanRankSlot.Membership], Array.Empty<string>())
			});

		var conversion = new FutureMudClanTransformer().Convert(source, references);
		var converted = conversion.ConvertedClans.ToDictionary(x => x.CanonicalAlias, StringComparer.OrdinalIgnoreCase);

		Assert.AreEqual("Tur Edendor Council", converted["tecouncil"].FullName);
		Assert.AreEqual("Metalsmiths Fellowship", converted["metalsmiths_fellowship"].FullName);
		Assert.AreEqual("Minas Morgul Slaves", converted["mm_slaves"].FullName);
		Assert.AreEqual("Minas Tirith Ratcatchers", converted["mt_ratcatchers"].FullName);
		Assert.AreEqual("Osgiliath Rat Catchers", converted["osgi_ratcatchers"].FullName);
		Assert.AreEqual("Witchking's Horde", converted["witchkings_horde"].FullName);
		CollectionAssert.Contains(converted["witchkings_horde"].LegacyAliases.ToList(), "witchking_horse");
		Assert.AreEqual("Pel Pelennor", converted["pel_pelennor"].FullName);
		Assert.AreEqual("Osgi Citizens", converted["osgi_citizens"].FullName);
		Assert.IsFalse(conversion.UnresolvedAliasCounts.Any());

		foreach (var clan in converted
			         .Where(x => references.ReferencesByAlias.ContainsKey(x.Key) ||
			                     x.Value.LegacyAliases.Any(references.ReferencesByAlias.ContainsKey))
			         .Select(x => x.Value))
		{
			Assert.IsTrue(clan.Ranks.Any(x => x.Slot == RpiClanRankSlot.Membership));
		}
	}

	private static string GetClanSourcePath()
	{
		var candidates = new[]
		{
			Path.Combine(AppContext.BaseDirectory, "Fixtures", "Clans", "clan.cpp"),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Fixtures", "Clans", "clan.cpp")),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter Tests", "Fixtures", "Clans", "clan.cpp")),
		};

		return candidates.First(File.Exists);
	}

	private static string GetClanRegionsDirectory()
	{
		var candidates = new[]
		{
			Path.Combine(AppContext.BaseDirectory, "Fixtures", "Clans", "regions"),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Fixtures", "Clans", "regions")),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter Tests", "Fixtures", "Clans", "regions")),
		};

		return candidates.First(Directory.Exists);
	}
}
