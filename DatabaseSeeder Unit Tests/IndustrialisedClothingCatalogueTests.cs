#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingCatalogueTests
{
	[TestMethod]
	public void EmbeddedSchemas_ArePresentButDoNotPretendToBeFinishedContent()
	{
		var document = IndustrialisedItemCatalogue.Document.Clothing;
		Assert.AreEqual(13, IndustrialisedClothingCatalogue.Headers.Count);
		Assert.AreEqual(0, document.Bases.Count);
		Assert.AreEqual(0, document.Skins.Count);
		Assert.AreEqual(0, document.Crafts.Count);
		Assert.AreEqual(0, document.Outfits.Count);
	}

	[TestMethod]
	public void AuthoredContract_PreservesProseRoutesNullQualityAndExactSkinProducts()
	{
		var d = Load(Fixture());
		Assert.AreEqual(ClothingProductionRoute.Hand, d.Bases.Single().ProductionRoute);
		Assert.IsNull(d.Skins.Single().QualityOverride);
		Assert.AreEqual("A narrow band of embroidery follows the edge of the $colour cloth.", d.Skins.Single().FullDescription);
		Assert.AreEqual("trimmed_coat", d.CraftProducts.Single(x => !x.FailureProduct).SkinReference);
		Assert.AreEqual(1, d.CraftColours.Single().InputOrder);
		Assert.AreEqual("cream", d.OutfitColours.Single().Value);
		Assert.AreEqual(2, d.CraftPhases.Count);
		Assert.AreEqual("Clothing/skins.tsv", d.Skins.Single().Source.File);
		Assert.AreEqual(2, d.Skins.Single().Source.Line);
	}

	[TestMethod]
	public void UnskinnedDefaults_AreValidOutfitAndCraftProducts()
	{
		var sources = Fixture();
		ReplaceCell(sources, "outfit-entries.tsv", 4, string.Empty);
		ReplaceCell(sources, "craft-products.tsv", 5, string.Empty);
		var d = Load(sources);
		Assert.AreEqual(string.Empty, d.OutfitEntries.Single().SkinReference);
		Assert.AreEqual(string.Empty, d.CraftProducts.First().SkinReference);
	}

	[DataTestMethod]
	[DataRow("skins.tsv", 1, "absent", "Skin must identify")]
	[DataRow("skins.tsv", 6, "Batch", "normal production route")]
	[DataRow("skins.tsv", 7, "Good", "approval reference")]
	[DataRow("skins.tsv", 2, "modern", "not admitted")]
	[DataRow("colours.tsv", 0, "absent", "Unknown colour presentation")]
	[DataRow("colours.tsv", 5, "red", "permitted value")]
	[DataRow("colours.tsv", 6, "black", "no fixed-colour")]
	[DataRow("outfit-entries.tsv", 0, "absent", "undeclared outfit")]
	[DataRow("outfit-entries.tsv", 2, "2", "contiguous")]
	[DataRow("outfit-entries.tsv", 3, "absent", "undeclared outfit")]
	[DataRow("outfit-entries.tsv", 4, "absent", "skin/base mismatch")]
	[DataRow("outfit-entries.tsv", 6, "Container", "requires an explicit")]
	[DataRow("outfit-entries.tsv", 7, "coat_entry", "earlier in wear order")]
	[DataRow("outfit-entries.tsv", 8, "absent", "Unknown outfit palette")]
	[DataRow("outfit-colours.tsv", 1, "absent", "Unknown outfit colour entry")]
	[DataRow("crafts.tsv", 13, "3", "Failure phase")]
	[DataRow("craft-phases.tsv", 0, "absent", "Unknown craft")]
	[DataRow("craft-inputs.tsv", 1, "3", "contiguous")]
	[DataRow("craft-products.tsv", 4, "absent", "declared base")]
	[DataRow("craft-products.tsv", 5, "absent", "skin/base mismatch")]
	[DataRow("craft-products.tsv", 6, "1.5", "integer")]
	[DataRow("craft-products.tsv", 7, "9", "material-defining input")]
	[DataRow("craft-colours.tsv", 1, "3", "Unknown item product")]
	[DataRow("craft-colours.tsv", 4, "blue", "exactly one")]
	[DataRow("craft-colours.tsv", 5, "9", "Unknown colour source input")]
	public void InvalidGraphs_FailWithSourceLocations(string file, int column, string value, string diagnostic)
	{
		var sources = Fixture();
		ReplaceCell(sources, file, column, value);
		var ex = Assert.ThrowsException<InvalidDataException>(() => Load(sources));
		StringAssert.Contains(ex.Message, "Clothing/");
		StringAssert.Contains(ex.Message, diagnostic);
	}

	[TestMethod]
	public void MissingDuplicateAndUnexpectedTablesCannotBeSilentlyIgnored()
	{
		var fixture = Fixture();
		fixture.Remove("skins.tsv");
		StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() => Load(fixture)).Message, "required exactly once");
		var sources = Sources(Fixture()).ToArray();
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCatalogue.Load(sources.Append(sources[0])));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCatalogue.Load(sources.Append(new("Clothing/typo.tsv", "test"))));
	}

	[TestMethod]
	public void DuplicateChannelsAndEntryKeysFailRatherThanTakingTheLastRow()
	{
		foreach (var name in new[] { "colours.tsv", "outfit-entries.tsv", "outfit-colours.tsv", "craft-colours.tsv" })
		{
			var fixture = Fixture();
			fixture[name] += fixture[name].Split('\n')[1] + "\n";
			StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() => Load(fixture)).Message, "Duplicate key");
		}
	}

	internal static Dictionary<string, string> Fixture()
	{
		var sources = IndustrialisedClothingCatalogue.Headers.ToDictionary(x => x.Key, x => x.Value + "\n", StringComparer.Ordinal);
		void Row(string name, params string[] values) => sources[name] += string.Join('\t', values) + "\n";
		Row("bases.tsv", "coat", "industrial;modern", "outerwear", "Hand", "A distinct fitted outer layer.", "Stock quality assessed independently.", "Draft", "Test-only authored fixture.");
		Row("skins.tsv", "trimmed_coat", "coat", "industrial;modern", "coat", "an embroidered $colour coat",
			"A narrow band of embroidery follows the edge of the $colour cloth.", "Hand", "", "", "Compatible embroidered edge.", "Draft", "Test-only fixture.");
		Row("colours.tsv", "coat", "colour", "Garment Colour", "All Colours", "blue;cream;black", "blue", "", "", "Dyeable textile.");
		Row("palettes.tsv", "neutral", "colour", "blue");
		Row("outfits.tsv", "test_outfit", "Test ensemble", "A fixture for ordered clothing.", "industrial;modern", "Draft", "Fixture, not stock.");
		Row("outfit-entries.tsv", "test_outfit", "coat_entry", "1", "coat", "trimmed_coat", "Coat", "Worn", "", "neutral", "Test wear profile.");
		Row("outfit-colours.tsv", "test_outfit", "coat_entry", "colour", "cream");
		Row("crafts.tsv", "sew_coat", "sew coat", "Tailoring", "Sew the outer garment.", "sewing a coat", "an in-progress coat craft",
			"industrial;modern", "Hand", "Tailoring", "0", "Normal", "MinorFail", "5", "2", "false", "Draft", "Fixture, not an accepted production recipe.");
		Row("craft-phases.tsv", "sew_coat", "1", "30", "$0 cut|cuts $i1 with $t1.", "$0 cut|cuts $i1 with $t1.");
		Row("craft-phases.tsv", "sew_coat", "2", "60", "$0 finish|finishes $p1.", "$0 recover|recovers $f1.");
		Row("craft-inputs.tsv", "sew_coat", "1", "Commodity", "cotton", "500", "1");
		Row("craft-tools.tsv", "sew_coat", "1", "Tools / Scissors", "Held", "1", "true");
		Row("craft-products.tsv", "sew_coat", "1", "false", "Item", "coat", "trimmed_coat", "1", "");
		Row("craft-products.tsv", "sew_coat", "1", "true", "UnusedInput", "1", "", "0.25", "");
		Row("craft-colours.tsv", "sew_coat", "1", "false", "colour", "", "1");
		return sources;
	}

	internal static void ReplaceCell(Dictionary<string, string> sources, string name, int column, string value)
	{
		var lines = sources[name].Split('\n');
		var fields = lines[1].Split('\t');
		fields[column] = value;
		lines[1] = string.Join('\t', fields);
		sources[name] = string.Join('\n', lines);
	}

	internal static IndustrialisedClothingCatalogueDocument Load(Dictionary<string, string> sources) => IndustrialisedClothingCatalogue.Load(Sources(sources));
	private static IEnumerable<IndustrialisedCatalogueSource> Sources(Dictionary<string, string> sources) => sources.Select(x => new IndustrialisedCatalogueSource($"Clothing/{x.Key}", x.Value));
}
