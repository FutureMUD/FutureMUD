#nullable enable

using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreIndustrialFoodCatalogueTests
{
	[TestMethod]
	public void PreparedFoodDefinitions_PreserveItemDescriptionsAndCopySuppliedTaste()
	{
		var item = ItemSeeder.PreIndustrialFoodItemsForTesting
			.First(x => x.Kind == FoodCatalogueKind.Prepared) with { Taste = "A test flavour supplied by the caller." };
		var definition = XElement.Parse(ItemSeeder.PreIndustrialPreparedFoodDefinitionForTesting(item));
		Assert.AreEqual(string.Empty, definition.Element("Short")?.Value);
		Assert.AreEqual(string.Empty, definition.Element("Full")?.Value);
		Assert.AreEqual(item.Taste, definition.Element("Taste")?.Value);
	}
}
