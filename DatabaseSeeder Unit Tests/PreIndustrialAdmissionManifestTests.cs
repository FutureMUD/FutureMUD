#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreIndustrialAdmissionManifestTests
{
	private const int ExpectedAdmissionCount = 385;

	private static readonly IReadOnlyDictionary<string, string> ManifestFiles =
		new Dictionary<string, string>
		{
			["Medieval"] = "FutureMUD_Medieval_Shared_Baseline_Admission_Manifest.md",
			["Renaissance"] = "FutureMUD_Renaissance_Shared_Baseline_Admission_Manifest.md",
			["Early Modern"] = "FutureMUD_EarlyModern_Shared_Baseline_Admission_Manifest.md"
		};

	private static readonly string[] HistoricallySensitiveTradePackages =
	[
		"preindustrial_trade_tea_chest",
		"preindustrial_trade_coffee_sack",
		"preindustrial_trade_cacao_sack",
		"preindustrial_trade_tobacco_bale",
		"preindustrial_trade_sugar_hogshead",
		"preindustrial_trade_spice_chest",
		"preindustrial_trade_indigo_cake_box",
		"preindustrial_trade_porcelain_packing_crate",
		"preindustrial_trade_glass_bottle_crate",
		"preindustrial_trade_silk_bale",
		"preindustrial_trade_cotton_bale"
	];

	[DataTestMethod]
	[DataRow("Medieval")]
	[DataRow("Renaissance")]
	[DataRow("Early Modern")]
	public void AdmissionManifest_ContainsExactLiveSharedInventoryWithCompleteDecisions(string era)
	{
		var expectedSources = ExpectedSources();
		var path = ManifestPath(era);
		Assert.IsTrue(File.Exists(path), $"Missing {era} shared-baseline admission manifest.");

		var source = File.ReadAllText(path);
		StringAssert.Contains(source, "**Status:** complete populated admission registry.");
		Assert.IsFalse(source.Contains("TODO", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(source.Contains("TBD", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(source.Contains("not yet populated", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(source.Contains("policy template", StringComparison.OrdinalIgnoreCase));

		var records = ReadRecords(path);
		Assert.AreEqual(ExpectedAdmissionCount, records.Count,
			$"{era} must contain one admission decision for every live shared prototype.");
		CollectionAssert.AreEquivalent(expectedSources.Keys.ToArray(), records.Keys.ToArray(),
			$"{era} does not exactly match the live shared-baseline inventory.");

		foreach (var (stableReference, record) in records)
		{
			Assert.AreEqual(expectedSources[stableReference], record.LiveSource,
				$"{era} has the wrong live source for {stableReference}.");
			Assert.IsTrue(record.RequiredDecisionFields.All(x => !string.IsNullOrWhiteSpace(x)),
				$"{era} has an incomplete admission decision for {stableReference}.");
		}
	}

	[TestMethod]
	public void AdmissionManifests_KeepHistoricallySensitiveTechnologyAndTradeGatesExplicit()
	{
		var medieval = ReadRecords(ManifestPath("Medieval"));
		var renaissance = ReadRecords(ManifestPath("Renaissance"));
		var earlyModern = ReadRecords(ManifestPath("Early Modern"));

		var medievalPrinting = medieval.Values
			.Where(x => x.StableReference.StartsWith("preindustrial_printing_", StringComparison.Ordinal))
			.ToArray();
		Assert.AreEqual(11, medievalPrinting.Length);
		Assert.IsTrue(medievalPrinting.All(x => x.Availability == "Not admitted"));

		var medievalFirearms = medieval.Values
			.Where(x => x.StableReference.StartsWith("preindustrial_firearms_", StringComparison.Ordinal))
			.ToArray();
		Assert.AreEqual(10, medievalFirearms.Length);
		Assert.IsTrue(medievalFirearms.All(x =>
			x.Availability == "Not admitted" &&
			x.DateWindow == "Not before 1450 CE for this firearm-support suite"));

		Assert.AreEqual("Not admitted", medieval["preindustrial_optics_telescope"].Availability);
		StringAssert.Contains(medieval["preindustrial_optics_telescope"].DateWindow, "1608");
		Assert.AreEqual("Not admitted", renaissance["preindustrial_optics_telescope"].Availability);
		StringAssert.Contains(renaissance["preindustrial_optics_telescope"].DateWindow, "1608");
		Assert.AreEqual("Restricted specialist", earlyModern["preindustrial_optics_telescope"].Availability);
		StringAssert.Contains(earlyModern["preindustrial_optics_telescope"].DateWindow, "1608-1750");

		Assert.AreEqual("Not admitted", medieval["preindustrial_navigation_mariner_astrolabe"].Availability);
		StringAssert.Contains(medieval["preindustrial_navigation_mariner_astrolabe"].DateWindow, "late fifteenth");
		Assert.AreEqual("Restricted specialist",
			renaissance["preindustrial_navigation_mariner_astrolabe"].Availability);
		Assert.AreEqual("1450-1600 CE",
			renaissance["preindustrial_navigation_mariner_astrolabe"].DateWindow);

		foreach (var stableReference in HistoricallySensitiveTradePackages)
		{
			foreach (var (era, records) in new[]
			         {
				         ("Medieval", medieval),
				         ("Renaissance", renaissance),
				         ("Early Modern", earlyModern)
			         })
			{
				Assert.AreNotEqual("Ordinary", records[stableReference].Availability,
					$"{era} must retain an explicit contact, specialist, or exclusion gate for {stableReference}.");
			}
		}
	}

	private static IReadOnlyDictionary<string, string> ExpectedSources()
	{
		var expected = PreIndustrialBaselineExpectations.RequiredAliases
			.ToDictionary(x => x.Value, x => x.Key, StringComparer.OrdinalIgnoreCase);

		foreach (var spec in ItemSeeder.PreIndustrialNewItemSpecsForTesting)
		{
			expected.Add(spec.StableReference, "shared-authored");
		}

		Assert.AreEqual(ExpectedAdmissionCount, expected.Count);
		return expected;
	}

	private static IReadOnlyDictionary<string, ManifestRecord> ReadRecords(string path)
	{
		var records = new Dictionary<string, ManifestRecord>(StringComparer.OrdinalIgnoreCase);
		var insideRecords = false;
		var endedRecords = false;

		foreach (var line in File.ReadLines(path))
		{
			if (line == "<!-- admission-records:start -->")
			{
				insideRecords = true;
				continue;
			}

			if (line == "<!-- admission-records:end -->")
			{
				endedRecords = true;
				break;
			}

			if (!insideRecords || !line.StartsWith("| `preindustrial_", StringComparison.Ordinal))
			{
				continue;
			}

			var cells = line
				.Split('|')
				.Skip(1)
				.SkipLast(1)
				.Select(x => x.Trim().Trim('`'))
				.ToArray();
			Assert.AreEqual(9, cells.Length, $"Malformed admission row: {line}");

			var record = new ManifestRecord(
				cells[0],
				cells[1],
				cells[2],
				cells[3],
				cells[4],
				cells[5],
				cells[6],
				cells[7],
				cells[8]);
			Assert.IsTrue(records.TryAdd(record.StableReference, record),
				$"Duplicate admission row for {record.StableReference}.");
		}

		Assert.IsTrue(insideRecords, $"Manifest {path} has no admission-record boundary.");
		Assert.IsTrue(endedRecords, $"Manifest {path} has no closing admission-record boundary.");
		return records;
	}

	private static string ManifestPath(string era)
	{
		return Path.Combine(
			SourceRoot(),
			"Design Documents",
			"Seeding",
			ManifestFiles[era]);
	}

	private static string SourceRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate repository root from test output path.");
		return directory.FullName;
	}

	private sealed record ManifestRecord(
		string StableReference,
		string LiveSource,
		string Family,
		string CultureOrContactScope,
		string DateWindow,
		string AdmittingContext,
		string Availability,
		string TradeOrContactStatus,
		string ComponentReality)
	{
		public IReadOnlyCollection<string> RequiredDecisionFields =>
		[
			StableReference,
			LiveSource,
			Family,
			CultureOrContactScope,
			DateWindow,
			AdmittingContext,
			Availability,
			TradeOrContactStatus,
			ComponentReality
		];
	}
}
