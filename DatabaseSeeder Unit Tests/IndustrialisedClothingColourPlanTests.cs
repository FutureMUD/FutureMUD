#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingColourPlanTests
{
	[TestMethod]
	public void ExplicitInstanceThenEntryThenPalette_PrecedenceIsDeterministicAndDoesNotChangeStock()
	{
		var document = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		var entry = document.OutfitEntries.Single();
		Assert.AreEqual("cream", IndustrialisedClothingColourPlan.OutfitValues(document, entry)["colour"]);
		Assert.AreEqual("black", IndustrialisedClothingColourPlan.OutfitValues(document, entry,
			new Dictionary<string, string> { ["colour"] = "black" })["colour"]);
		Assert.AreEqual("cream", IndustrialisedClothingColourPlan.OutfitValues(document, entry)["colour"]);
		Assert.AreEqual("blue", document.Colours.Single().DefaultValue);
		var withoutEntryDefault = document with { OutfitColours = [] };
		Assert.AreEqual("blue", IndustrialisedClothingColourPlan.OutfitValues(withoutEntryDefault, entry)["colour"]);
		Assert.AreEqual("blue", IndustrialisedClothingColourPlan.OutfitValues(withoutEntryDefault, entry with { SkinReference = "" })["colour"]);
	}

	[TestMethod]
	public void InvalidExplicitChoiceDoesNotSilentlyFallBackToAValidDefault()
	{
		var d = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingColourPlan.OutfitValues(d, d.OutfitEntries.Single(),
			new Dictionary<string, string> { ["colour"] = "red" }));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingColourPlan.OutfitValues(d, d.OutfitEntries.Single(),
			new Dictionary<string, string> { ["coluor"] = "blue" }));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingColourPlan.OutfitValues(d with { OutfitColours = [], Palettes = [] }, d.OutfitEntries.Single()));
	}

	[TestMethod]
	public void SkinsCanNarrowButCannotInventOrBroadenColourBindings()
	{
		var d = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		var source = d.Colours.Single();
		var narrowed = source with { PresentationReference = "trimmed_coat", AllowedValues = new[] { "cream", "black" }, DefaultValue = "cream" };
		var modified = d with { Colours = new[] { source, narrowed } };
		IndustrialisedClothingCatalogue.ValidateStructure(modified);
		Assert.AreEqual("cream", IndustrialisedClothingColourPlan.OutfitValues(modified, d.OutfitEntries.Single())["colour"]);
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingColourPlan.OutfitValues(modified, d.OutfitEntries.Single(),
			new Dictionary<string, string> { ["colour"] = "blue" }));
		foreach (var invalid in new[] { narrowed with { Definition = "Other Definition" }, narrowed with { Profile = "Other Profile" },
			narrowed with { Variable = "colour2" }, narrowed with { AllowedValues = new[] { "red" }, DefaultValue = "red" } })
		{
			Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCatalogue.ValidateStructure(d with { Colours = new[] { source, invalid } }));
		}
	}

	[TestMethod]
	public void CraftColoursRequireACompleteExactMappingInsteadOfRandomOutput()
	{
		var d = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		var selection = d.CraftColours.Single();
		Assert.AreEqual(1, IndustrialisedClothingColourPlan.CraftValues(d, d.CraftProducts.First())["colour"].InputOrder);
		var fixedChoice = d with { CraftColours = new[] { selection with { InputOrder = null, Value = "cream" } } };
		IndustrialisedClothingCatalogue.ValidateStructure(fixedChoice);
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCatalogue.ValidateStructure(d with { CraftColours = [] }));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCatalogue.ValidateStructure(d with
			{ CraftColours = new[] { selection with { InputOrder = null, Value = "red" } } }));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCatalogue.ValidateStructure(d with
			{ CraftColours = new[] { selection with { Variable = "unknown" } } }));
	}
}
