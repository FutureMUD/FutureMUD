#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonOwnedItemMarkDefinitionTests
{
	[TestMethod]
	public void OwnedItemMarks_StockDefinitionsKeepDistinctDurationCostAndTypedEffectModes()
	{
		var entries = ArmageddonOwnedItemMarkSpellContent.Create().ToDictionary(x => x.Content.Key);
		Assert.AreEqual(2, entries.Count);
		Assert.AreEqual("arm.capability.fire", entries[ArmageddonOwnedItemMarkSpellContent.CustodianGlyphKey].NativeCapabilityKey);
		Assert.AreEqual("arm.capability.wind", entries[ArmageddonOwnedItemMarkSpellContent.AnchorRuneKey].NativeCapabilityKey);
		foreach (var (key, expectedDuration, expectedCost, mode) in new[]
		{
			(ArmageddonOwnedItemMarkSpellContent.CustodianGlyphKey, "60*grade", 10.0, "glyph"),
			(ArmageddonOwnedItemMarkSpellContent.AnchorRuneKey, "300*grade", 50.0, "anchor")
		})
		{
			var entry = entries[key].Content;
			var definition = entry.BuildDefinition(1, 2, 0);
			Assert.AreEqual(expectedDuration, entry.DurationFormula, key);
			Assert.AreEqual(expectedCost, entry.MinimumEnergy, key);
			Assert.AreEqual("item", (string?)definition.Element("Trigger")?.Attribute("type"), key);
			Assert.AreEqual("owneditemmark", (string?)definition.Element("Effects")?.Element("Effect")?.Attribute("type"), key);
			Assert.AreEqual(mode, (string?)definition.Element("Effects")?.Element("Effect")?.Element("Mode"), key);
		}
	}
}
