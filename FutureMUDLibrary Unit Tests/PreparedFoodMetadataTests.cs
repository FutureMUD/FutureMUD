using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems.Interfaces;
using System.Linq;

namespace FutureMUDLibrary_Unit_Tests;

[TestClass]
public class PreparedFoodMetadataTests
{
	[TestMethod]
	public void IngredientMetadata_RoundTripsAndClonePreservesAllergens()
	{
		var ingredient = new FoodIngredientInstance
		{
			Role = "binder",
			Description = "wheat flour",
			TasteText = "mild wheat",
			Category = FoodIngredientCategory.Grain,
			Weight = 100.0
		};
		ingredient.Allergens.Add(MajorFoodAllergen.GlutenCereal);

		var loaded = FoodIngredientInstance.LoadFromXml(ingredient.SaveToXml());
		var clone = loaded.Clone();

		Assert.AreEqual(FoodIngredientCategory.Grain, loaded.Category);
		CollectionAssert.AreEquivalent(new[] { MajorFoodAllergen.GlutenCereal }, loaded.Allergens.ToArray());
		CollectionAssert.AreEquivalent(loaded.Allergens.ToArray(), clone.Allergens.ToArray());
	}

	[TestMethod]
	public void LegacyIngredientXml_LoadsNeutralStructuredMetadata()
	{
		var ingredient = FoodIngredientInstance.LoadFromXml(System.Xml.Linq.XElement.Parse(
			"<Ingredient role='base' source='0' material='0' liquid='0' weight='1' volume='0' quality='5'><Description><![CDATA[rice]]></Description><Taste><![CDATA[rice]]></Taste></Ingredient>"));

		Assert.AreEqual(FoodIngredientCategory.Other, ingredient.Category);
		Assert.AreEqual(0, ingredient.Allergens.Count);
	}
}
