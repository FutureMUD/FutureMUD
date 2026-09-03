#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedCatalogueSourceTests
{
	[DataTestMethod]
	[DataRow("\n")]
	[DataRow("\r\n")]
	[DataRow("\r")]
	public void Parser_PreservesFinishedTextAndSourceLocations(string newline)
	{
		const string prose = "  Fine seams follow the collar; its $colour cloth has an even, matte surface.  ";
		var source = new IndustrialisedCatalogueSource("Clothing/author.tsv",
			$"\uFEFFKey\tProse{newline}{newline}shirt\t{prose}{newline}");
		var row = source.Read(["Key", "Prose"], (name, line, fields) => (name, line, fields)).Single();
		Assert.AreEqual("Clothing/author.tsv", row.name);
		Assert.AreEqual(3, row.line);
		Assert.AreEqual(prose, row.fields[1]);
	}

	[DataTestMethod]
	[DataRow("key\tProse\na\tb", 1)]
	[DataRow("Key\tProse\na\tb\tc", 2)]
	[DataRow("Key\tProse\na", 2)]
	public void Parser_RejectsHeaderAndColumnDriftWithLocation(string text, int line)
	{
		var ex = Assert.ThrowsException<InvalidDataException>(() =>
			new IndustrialisedCatalogueSource("author.tsv", text).Read(["Key", "Prose"], (_, _, row) => row).ToArray());
		StringAssert.Contains(ex.Message, $"author.tsv:{line}:");
	}

	[TestMethod]
	public void Lists_AreTrimmedOrderedReadOnlyAndExplicitlyEmpty()
	{
		var list = IndustrialisedCatalogueValues.List(" blue ; cream;dark grey ");
		CollectionAssert.AreEqual(new[] { "blue", "cream", "dark grey" }, list.ToArray());
		Assert.AreEqual(0, IndustrialisedCatalogueValues.List(string.Empty).Count);
		Assert.ThrowsException<NotSupportedException>(() => ((System.Collections.Generic.IList<string>)list)[0] = "red");
	}

	[DataTestMethod]
	[DataRow("blue;Blue")]
	[DataRow("blue;")]
	[DataRow("blue;;cream")]
	[DataRow(" ")]
	public void Lists_RejectAmbiguousOrEmptyMembers(string text) =>
		Assert.ThrowsException<FormatException>(() => IndustrialisedCatalogueValues.List(text));

	[TestMethod]
	public void Numbers_UseInvariantParsingWithoutThousandsAmbiguity()
	{
		var previous = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			Assert.AreEqual(1.25m, IndustrialisedCatalogueValues.Decimal("1.25"));
			Assert.AreEqual(125.0, IndustrialisedCatalogueValues.Double("1.25e2"));
			Assert.ThrowsException<FormatException>(() => IndustrialisedCatalogueValues.Decimal("1,25"));
			Assert.ThrowsException<FormatException>(() => IndustrialisedCatalogueValues.Decimal("1,000"));
		}
		finally
		{
			CultureInfo.CurrentCulture = previous;
		}
	}

	[DataTestMethod]
	[DataRow("NaN")]
	[DataRow("Infinity")]
	[DataRow("1e999")]
	public void Numbers_RejectNonFiniteValuesWithSourceLocation(string text)
	{
		var ex = Assert.ThrowsException<InvalidDataException>(() =>
			new IndustrialisedCatalogueSource("weights.tsv", $"Weight\n{text}\n")
				.Read(["Weight"], (_, _, row) => IndustrialisedCatalogueValues.Double(row[0])).ToArray());
		StringAssert.Contains(ex.Message, "weights.tsv:2:");
	}

	[TestMethod]
	public void Enums_RejectNumericValuesEvenWhenTheyNameAnExistingMember()
	{
		Assert.AreEqual(SizeCategory.Small, IndustrialisedCatalogueValues.EnumValue<SizeCategory>("small"));
		Assert.ThrowsException<FormatException>(() => IndustrialisedCatalogueValues.EnumValue<SizeCategory>("999"));
		Assert.ThrowsException<FormatException>(() => IndustrialisedCatalogueValues.EnumValue<SizeCategory>(((int)SizeCategory.Small).ToString(CultureInfo.InvariantCulture)));
	}

	[TestMethod]
	public void DirectoryAndEmbeddedLoading_ProduceIdenticalAuthoredRows()
	{
		var directory = CatalogueDirectory();
		var disk = IndustrialisedItemCatalogue.LoadDirectory(directory);
		var embedded = IndustrialisedItemCatalogue.Document;
		CollectionAssert.AreEqual(embedded.Items.Select(x => (x.Source, x.Line, x.StableReference, x.FullDescription)).ToArray(),
			disk.Items.Select(x => (x.Source, x.Line, x.StableReference, x.FullDescription)).ToArray());
		var sources = Directory.EnumerateFiles(directory, "*.tsv", SearchOption.AllDirectories)
			.Select(path => new IndustrialisedCatalogueSource(Path.GetRelativePath(directory, path).Replace('\\', '/'), File.ReadAllText(path))).ToArray();
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedItemCatalogue.LoadSources(sources.Where(x => x.Name != "crafts.tsv")));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedItemCatalogue.LoadSources(sources.Append(sources[0])));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedItemCatalogue.LoadSources(sources.Append(new("unrecognised.tsv", "Ignored"))));
	}

	[TestMethod]
	public void Audit_IsDeterministicReadsSourcesAndDoesNotClaimProductionAcceptance()
	{
		var directory = CatalogueDirectory();
		var before = IndustrialisedCatalogueAudit.SourceFingerprint(directory);
		var first = IndustrialisedCatalogueAudit.Generate(directory);
		Assert.AreEqual(first, IndustrialisedCatalogueAudit.Generate(directory));
		Assert.AreEqual(before, IndustrialisedCatalogueAudit.SourceFingerprint(directory));
		Assert.AreEqual(IndustrialisedCatalogueAudit.Header, first.Split('\n')[0]);
		Assert.IsTrue(first.Split('\n').Skip(1).Where(x => x.Length > 0)
			.All(x => x.Split('\t')[12] == "parsed;production-unreviewed"));
		StringAssert.Contains(first, "Connectable_Male_To_MainsPlug");
		StringAssert.Contains(first, before);
	}

	private static string CatalogueDirectory() => Path.Combine(ItemSeederManifestCatalogue.FindRepositoryRoot(),
		"DatabaseSeeder", "Seeders", "IndustrialisedCatalogue");
}
