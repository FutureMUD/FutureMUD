#nullable enable
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

/// <summary>Frozen definitions from reviewed commit 7339b816; tests extraction, not a new provision policy.</summary>
[TestClass]
public class ProvisionContentExtractionTests
{
	[DataTestMethod] [DataRow(false)] [DataRow(true)]
	public void FoodDefinition_MatchesReviewedFactory(bool threeFoods)
	{
		var ids = threeFoods ? new long[] { 11, 12, 13 } : new long[] { 11 };
		var reviewed = ReviewedSustainMeal(19, 23, ids);
		Assert.IsTrue(XNode.DeepEquals(reviewed, ArmageddonSustainMealStock.Definition(19, 23, ids)));
		Assert.IsTrue(XNode.DeepEquals(reviewed, ArmageddonReviewedProvisionContent.SustainMeal(ids).BuildDefinition(19, 23, 0)));
	}
	[DataTestMethod] [DataRow(false)] [DataRow(true)]
	public void WineDefinition_MatchesReviewedFactory(bool bonusPlane)
	{
		long? plane = bonusPlane ? 29 : null; var reviewed = ReviewedDrawWine(19, 23, 31, 37, plane);
		Assert.IsTrue(XNode.DeepEquals(reviewed, ArmageddonDrawWineStock.Definition(19, 23, 31, 37, plane)));
		Assert.IsTrue(XNode.DeepEquals(reviewed, ArmageddonReviewedProvisionContent.DrawWine(37, plane).BuildDefinition(19, 23, 31)));
	}
	[DataTestMethod] [DataRow(false)] [DataRow(true)]
	public void SharedMetadata_MatchesReviewedFactory(bool food)
	{
		var content = food ? ArmageddonReviewedProvisionContent.SustainMeal([11,12,13]) : ArmageddonReviewedProvisionContent.DrawWine(37,29);
		var hash = System.Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content.Description)));
		Assert.AreEqual(food ? "8983F4F482FD817710AA5B7525976145385F134E021043DFF2D3D9843D2DA384" : "61790649DECCDB97511722B8BD9A22F7284A56008A668B3F258B73831CA49715", hash);
		Assert.AreEqual(food ? "1350*grade" : "0", content.DurationFormula); Assert.AreEqual(7.0, content.MinimumEnergy);
		Assert.AreEqual(food ? null : ArmageddonDrawWineStock.EligibilitySource, content.EligibilitySource);
		Assert.IsFalse(content.SpellRow(19, 23, 31).ScrollInscriptionAllowed);
	}
	private static XElement ReviewedSustainMeal(long resource, long cost, params long[] prototypes) =>
		ArmageddonUtilityStock.Definition("arm.spell.sustain_meal", "room", resource, cost, 0, 30, 7,
			new XElement("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", "base"),
				new XElement("ItemPrototypeId", prototypes[0]), new XElement("ItemSkinId", 0), new XElement("Quantity", 1),
				new XElement("LoadString", ""), new XElement("Lifecycle", new XAttribute("version", 1),
					new XAttribute("mode", "TemporaryCleanup"), new XElement("Family", "sustain-meal"),
					new XElement("Seconds", "1350*grade"), new XElement("Count", "grade"),
					new XElement("FoodProfiles", new XAttribute("version", 1), new XElement("Profile", new XAttribute("order", 32),
						new XAttribute("predicate", 0), prototypes.Select(id => new XElement("Prototype", id)))))));
	private static XElement ReviewedDrawWine(long resource, long cost, long filter, long wine, long? plane) =>
		ArmageddonUtilityStock.Definition("arm.spell.draw_wine", "item", resource, cost, filter, 30, 7,
			new XElement("Effect", new XAttribute("type", "createliquid"), new XElement("LiquidId", wine),
				new XElement("AmountFormula", "0.5*grade"), new XElement("ContainerFill", new XAttribute("version", 1),
					new XElement("Litres", "0.5*grade"), plane is { } id ? new XElement("BonusPlane", new XAttribute("multiplier", 2), id) : null,
					new XElement("Recipes", new XAttribute("version", 1), new XElement("Recipe", new XAttribute("order", 32),
						new XAttribute("predicate", 0), new XAttribute("liquid", wine))))));
}
