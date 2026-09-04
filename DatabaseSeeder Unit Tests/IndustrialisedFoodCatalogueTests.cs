using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace DatabaseSeeder_Unit_Tests;

[TestClass]
public class IndustrialisedFoodCatalogueTests
{
	[TestMethod]
	public void EmbeddedCatalogue_HasApprovedGate2CountsAndResolvedGraph()
	{
		var food = IndustrialisedItemCatalogue.LoadForTesting().Food;
		Assert.IsNotNull(food);
		Assert.AreEqual(464, food.Concepts.Count);
		Assert.AreEqual(397, food.ItemCount);
		Assert.AreEqual(67, food.LiquidCount);
		Assert.AreEqual(307, food.AdoptedDependencies.Count);
		Assert.AreEqual(26, food.Servings.Count);
		Assert.IsTrue(food.Concepts.All(x => x.ReviewState == IndustrialisedFoodReviewState.DependencyReviewed));
	}

	[TestMethod]
	public void Gate2Catalogue_CannotBeInstalledAsFinishedContent()
	{
		var food = IndustrialisedItemCatalogue.LoadForTesting().Food;
		Assert.IsNotNull(food);
		var exception = Assert.ThrowsException<InvalidDataException>(() => food.EnsureProductionReadyForSeeding());
		StringAssert.Contains(exception.Message, "464");
		StringAssert.Contains(exception.Message, "not ProductionReady");
	}
}
