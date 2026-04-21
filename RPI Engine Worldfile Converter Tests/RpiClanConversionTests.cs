#nullable enable

using System;
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

		Assert.IsTrue(conversion.UnresolvedAliasCounts.ContainsKey("com-priests"));
		Assert.AreEqual(1, conversion.UnresolvedAliasCounts["com-priests"]);

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
