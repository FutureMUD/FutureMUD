#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingAuditTests
{
	[TestMethod]
	public void Audit_NonemptyGraph_AccountsForEverySourceRowAndExactRelationships()
	{
		var document = IndustrialisedClothingCraftPlanTests.Document();
		var audit = IndustrialisedClothingAudit.Generate(document, "source-hash");
		var rows = Rows(audit);
		Assert.AreEqual(15, rows.Length);
		Assert.AreEqual(13, rows.Select(x => x["RecordType"]).Distinct().Count());
		Assert.AreEqual(15, rows.Select(x => (x["RecordType"], x["RecordKey"])).Distinct().Count());
		Assert.IsTrue(rows.All(x => x["EraAdmissions"] == "industrial;modern" && x["CatalogueSourceSha256"] == "source-hash"));
		Assert.IsTrue(rows.All(x => x["SourceFile"].StartsWith("Clothing/", StringComparison.Ordinal) && int.Parse(x["SourceLine"], CultureInfo.InvariantCulture) >= 2));
		Assert.IsTrue(rows.All(x => x["Validation"] == "structure-validated;database-unverified;production-unreviewed"));
		var product = rows.Single(x => x["RecordType"] == "craft-product" && x["RecordKey"] == "sew_coat/success/1");
		Assert.AreEqual("coat", product["ItemReference"]);
		Assert.AreEqual("trimmed_coat", product["SkinReference"]);
		CollectionAssert.AreEquivalent(new[] { "craft:sew_coat", "craft-input:sew_coat/1", "item:coat", "item-skin:trimmed_coat" },
			JsonSerializer.Deserialize<string[]>(product["Dependencies"])!);
		using var selected = JsonDocument.Parse(product["ResolvedColourSelections"]);
		Assert.AreEqual(1, selected.RootElement.GetProperty("colour").GetProperty("InputOrder").GetInt32());
		var entry = rows.Single(x => x["RecordType"] == "outfit-entry");
		using var defaults = JsonDocument.Parse(entry["ResolvedColourSelections"]);
		Assert.AreEqual("cream", defaults.RootElement.GetProperty("colour").GetString());
		using var skin = JsonDocument.Parse(rows.Single(x => x["RecordType"] == "skin")["SourceRecord"]);
		Assert.AreEqual(document.Skins.Single().FullDescription, skin.RootElement.GetProperty("FullDescription").GetString());
		Assert.AreEqual(JsonValueKind.Null, skin.RootElement.GetProperty("QualityOverride").ValueKind);
		Assert.AreEqual("Draft", skin.RootElement.GetProperty("ReviewStatus").GetString());
	}

	[TestMethod]
	public void Audit_UnskinnedProductsAndEntries_DoNotAcquireAnImplicitSkin()
	{
		var document = IndustrialisedClothingCraftPlanTests.Document();
		document = document with
		{
			OutfitEntries = document.OutfitEntries.Select(x => x with { SkinReference = "" }).ToArray(),
			CraftProducts = document.CraftProducts.Select(x => x with { SkinReference = "" }).ToArray()
		};
		var rows = Rows(IndustrialisedClothingAudit.Generate(document, "hash"));
		foreach (var row in rows.Where(x => x["RecordType"] is "outfit-entry" or "craft-product"))
		{
			Assert.AreEqual("", row["SkinReference"]);
			Assert.IsFalse(row["Dependencies"].Contains("item-skin:", StringComparison.Ordinal));
		}
	}

	[TestMethod]
	public void Audit_IsInvariantAndDeterministicWithStableSourceOrdering()
	{
		var document = IndustrialisedClothingCraftPlanTests.Document();
		var expected = IndustrialisedClothingAudit.Generate(document, "hash");
		var previous = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			Assert.AreEqual(expected, IndustrialisedClothingAudit.Generate(document with
			{
				CraftPhases = document.CraftPhases.Reverse().ToArray(),
				CraftProducts = document.CraftProducts.Reverse().ToArray()
			}, "hash"));
		}
		finally { CultureInfo.CurrentCulture = previous; }
	}

	[TestMethod]
	public void Audit_PaletteAdmissions_UseCanonicalUnionIndependentOfConsumerOrdering()
	{
		var document = IndustrialisedClothingCraftPlanTests.Document();
		var outfit = document.Outfits.Single();
		var entry = document.OutfitEntries.Single();
		document = document with
		{
			Outfits = [outfit with { EraAdmissions = ["modern"] }, outfit with
				{ Source = new("Clothing/outfits.tsv", 3), StableReference = "older_outfit", EraAdmissions = ["industrial"] }],
			OutfitEntries = [entry, entry with { Source = new("Clothing/outfit-entries.tsv", 3), OutfitReference = "older_outfit" }]
		};
		var audit = IndustrialisedClothingAudit.Generate(document, "hash");
		Assert.AreEqual("industrial;modern", Rows(audit).Single(x => x["RecordType"] == "palette")["EraAdmissions"]);
		Assert.AreEqual(audit, IndustrialisedClothingAudit.Generate(document with
		{
			Outfits = document.Outfits.Reverse().ToArray(),
			OutfitEntries = document.OutfitEntries.Reverse().ToArray()
		}, "hash"));
	}

	[TestMethod]
	public void ItemAudit_JoinsNormalizedClothingCraftAndOutfitParticipation()
	{
		var source = IndustrialisedItemCatalogue.Document;
		var document = source with
		{
			Items = [source.Items.First() with { StableReference = "coat", Craftable = false }],
			Crafts = [], Outfits = [], Clothing = IndustrialisedClothingCraftPlanTests.Document()
		};
		var cells = IndustrialisedCatalogueAudit.Generate(document, "hash").Split('\n')[1].Split('\t');
		Assert.AreEqual("true", cells[10]);
		Assert.AreEqual("sew_coat", cells[14]);
		Assert.AreEqual("test_outfit", cells[15]);
	}

	[TestMethod]
	public void RefreshAndCheck_NonemptyClothingGraph_PreserveAllAuthoredSourcesAndDetectAuditDrift()
	{
		using var fixture = new AuditDirectory();
		var before = fixture.SourceBytes();
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, false);
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, true);
		Assert.AreEqual(15, Rows(File.ReadAllText(fixture.ClothingAuditPath)).Length);
		Assert.AreEqual(364, IndustrialisedClothingDependencyPlanTests.Rows(File.ReadAllText(fixture.DependencyAuditPath)).Length);
		AssertSourcesUnchanged(before, fixture.SourceBytes());
		File.AppendAllText(fixture.ClothingAuditPath, "stale\n");
		var stale = File.ReadAllBytes(fixture.ClothingAuditPath);
		StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() =>
			IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, true)).Message, IndustrialisedClothingAudit.RelativePath);
		CollectionAssert.AreEqual(stale, File.ReadAllBytes(fixture.ClothingAuditPath));
		AssertSourcesUnchanged(before, fixture.SourceBytes());
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, false);
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, true);
		File.Delete(fixture.ClothingAuditPath);
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, true));
	}

	[TestMethod]
	public void Refresh_InvalidCraftPhase_DoesNotPartiallyRewriteAnyAudit()
	{
		using var fixture = new AuditDirectory();
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, false);
		var itemAudit = File.ReadAllBytes(fixture.ItemAuditPath);
		var clothingAudit = File.ReadAllBytes(fixture.ClothingAuditPath);
		var dependencyAudit = File.ReadAllBytes(fixture.DependencyAuditPath);
		var path = Path.Combine(fixture.CataloguePath, "Clothing", "craft-phases.tsv");
		File.WriteAllText(path, File.ReadAllText(path).Replace("$p1", "$p99", StringComparison.Ordinal));
		var ex = Assert.ThrowsException<InvalidDataException>(() => IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, false));
		StringAssert.Contains(ex.Message, "Clothing/craft-phases.tsv:3:");
		CollectionAssert.AreEqual(itemAudit, File.ReadAllBytes(fixture.ItemAuditPath));
		CollectionAssert.AreEqual(clothingAudit, File.ReadAllBytes(fixture.ClothingAuditPath));
		CollectionAssert.AreEqual(dependencyAudit, File.ReadAllBytes(fixture.DependencyAuditPath));
	}

	[TestMethod]
	public void CheckAndRefresh_RejectDependencyAuditDriftAndPreserveAllAuditsOnInvalidScope()
	{
		using var fixture = new AuditDirectory();
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, false);
		File.AppendAllText(fixture.DependencyAuditPath, "stale\n");
		StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() =>
			IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, true)).Message, IndustrialisedClothingDependencyAudit.RelativePath);
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, false);
		IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, true);
		var paths = new[] { fixture.ItemAuditPath, fixture.ClothingAuditPath, fixture.DependencyAuditPath };
		var before = paths.ToDictionary(x => x, File.ReadAllBytes);
		var inventory = Path.Combine(fixture.Root, IndustrialisedClothingDependencyAudit.InventoryPath);
		File.WriteAllText(inventory, File.ReadAllText(inventory).Replace("| button_drawers |", "| unapproved_drawers |", StringComparison.Ordinal));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedCatalogueAudit.RefreshOrCheck(fixture.Root, false));
		foreach (var path in paths) CollectionAssert.AreEqual(before[path], File.ReadAllBytes(path));
	}

	private static Dictionary<string, string>[] Rows(string audit)
	{
		var lines = audit.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		Assert.AreEqual(IndustrialisedClothingAudit.Header, lines[0]);
		var headers = lines[0].Split('\t');
		return lines.Skip(1).Select(line =>
		{
			var cells = line.Split('\t');
			Assert.AreEqual(headers.Length, cells.Length);
			return headers.Zip(cells).ToDictionary(x => x.First, x => x.Second, StringComparer.Ordinal);
		}).ToArray();
	}

	private static void AssertSourcesUnchanged(Dictionary<string, byte[]> before, Dictionary<string, byte[]> after)
	{
		CollectionAssert.AreEquivalent(before.Keys.ToArray(), after.Keys.ToArray());
		foreach (var (path, bytes) in before) CollectionAssert.AreEqual(bytes, after[path], path);
	}

	private sealed class AuditDirectory : IDisposable
	{
		private readonly DirectoryInfo _directory = Directory.CreateTempSubdirectory("FutureMUD-clothing-audit-");
		internal string Root => _directory.FullName;
		internal string CataloguePath => Path.Combine(Root, "DatabaseSeeder", "Seeders", "IndustrialisedCatalogue");
		internal string ItemAuditPath => Path.Combine(Root, IndustrialisedCatalogueAudit.RelativePath);
		internal string ClothingAuditPath => Path.Combine(Root, IndustrialisedClothingAudit.RelativePath);
		internal string DependencyAuditPath => Path.Combine(Root, IndustrialisedClothingDependencyAudit.RelativePath);

		internal AuditDirectory()
		{
			foreach (var relative in IndustrialisedClothingDependencyAudit.InputPaths)
			{
				var target = Path.Combine(Root, relative);
				Directory.CreateDirectory(Path.GetDirectoryName(target)!);
				File.Copy(Path.Combine(ItemSeederManifestCatalogue.FindRepositoryRoot(), relative), target);
			}
			var source = Path.Combine(ItemSeederManifestCatalogue.FindRepositoryRoot(), "DatabaseSeeder", "Seeders", "IndustrialisedCatalogue");
			foreach (var file in Directory.EnumerateFiles(source, "*.tsv", SearchOption.AllDirectories))
			{
				var path = Path.Combine(CataloguePath, Path.GetRelativePath(source, file));
				Directory.CreateDirectory(Path.GetDirectoryName(path)!);
				File.Copy(file, path);
			}
			var sources = IndustrialisedClothingCatalogueTests.Fixture();
			sources["craft-products.tsv"] = sources["craft-products.tsv"].Replace("true\tUnusedInput\t1\t\t0.25", "true\tCommodity\tcotton\t\t125", StringComparison.Ordinal);
			foreach (var (file, text) in sources) File.WriteAllText(Path.Combine(CataloguePath, "Clothing", file), text);
		}

		internal Dictionary<string, byte[]> SourceBytes() => Directory.EnumerateFiles(CataloguePath, "*.tsv", SearchOption.AllDirectories)
			.ToDictionary(x => Path.GetRelativePath(CataloguePath, x), File.ReadAllBytes, StringComparer.Ordinal);
		public void Dispose() => _directory.Delete(true);
	}
}
