#nullable enable
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonClearHeadDefinitionTests
{
	[TestMethod]
	public void ClearHead_DefiningExactAlcoholModeMatchesLockedInventoryWithoutOtherNeedsOrVariants()
	{
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var contribution = ArmageddonPrimitiveSpellContent.CreateAlcoholRemovalContribution();
		var content = contribution.Content;
		var entry = manifest.RootElement.GetProperty("entries").EnumerateArray().Single(x => x.GetProperty("key").GetString() == content.Key);
		Assert.AreEqual("arm.spell.clear_head", content.Key);
		Assert.AreEqual("arm.capability.water", contribution.NativeCapabilityKey);
		Assert.AreEqual(entry.GetProperty("minimum_energy").GetDouble(), content.MinimumEnergy);
		Assert.AreEqual("0", content.DurationFormula);
		var definition = content.BuildDefinition(201, 202, 0);
		var effect = definition.Element("Effects")!.Elements().Single();
		Assert.AreEqual("needdelta", (string?)effect.Attribute("type"));
		Assert.AreEqual(0.0, (double)effect.Element("Hunger")!);
		Assert.AreEqual(0.0, (double)effect.Element("Thirst")!);
		Assert.AreEqual(-0.01, (double)effect.Element("Drunk")!);
		Assert.IsTrue((bool)effect.Element("DirectAlcoholRemoval")!);
		var profile = definition.Element("ControlledPower")!;
		Assert.AreEqual(40, (int)profile.Attribute("openingSkill")!);
		Assert.AreEqual(7.0, (double)profile.Element("Efficiency")!.Attribute("minimum")!);
		var binding = profile.Element("ScalarBindings")!.Elements().Single();
		Assert.AreEqual("needdelta", (string?)binding.Attribute("effect"));
		Assert.AreEqual("DrunkDelta", (string?)binding.Attribute("field"));
		Assert.AreEqual("-0.01*grade", binding.Value);
		Assert.IsNull(profile.Element("Area"));
		Assert.IsNull(profile.Element("Incantation"));
		Assert.IsFalse(content.SpellRow(301, 302, 303).ScrollInscriptionAllowed);
		Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
		Assert.AreEqual(0, definition.Element("CasterEffects")!.Elements().Count());
	}
}
