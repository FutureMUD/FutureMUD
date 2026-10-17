#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPrimitiveScalarDefinitionTests
{
	[DataTestMethod]
	[DataRow("daylight", "room", "roomlight", "AddedLuxPerPower", "20*grade/power", "60*grade")]
	[DataRow("deep_darkness", "room", "roomlight", "AddedLuxPerPower", "-20*grade/power", "60*grade")]
	[DataRow("quickened_healing", "character", "healingrate", "Multiplier", "1+0.1*grade", "120*grade")]
	public void ScalarContent_LockedIdentityDefaultsAndTypedBindingMatchManifest(string suffix, string trigger,
		string effectType, string field, string formula, string duration)
	{
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var contributions = ArmageddonPrimitiveSpellContent.CreateScalarContributions();
		Assert.AreEqual(3, contributions.Count);
		var contribution = contributions.Single(x => x.Content.Key == "arm.spell." + suffix);
		var content = contribution.Content;
		var entry = manifest.RootElement.GetProperty("entries").EnumerateArray().Single(x => x.GetProperty("key").GetString() == content.Key);
		Assert.AreEqual(entry.GetProperty("minimum_energy").GetDouble(), content.MinimumEnergy);
		Assert.AreEqual(entry.GetProperty("name").GetString(), content.Name);
		Assert.AreEqual("arm.capability." + entry.GetProperty("native_school").GetString()!.ToLowerInvariant(), contribution.NativeCapabilityKey);
		Assert.AreEqual(duration, content.DurationFormula);
		var definition = content.BuildDefinition(201, 202, 0);
		Assert.AreEqual(content.Key, definition.Element("StockIdentity")!.Value);
		Assert.AreEqual(trigger, (string?)definition.Element("Trigger")!.Attribute("type"));
		var effect = definition.Element("Effects")!.Elements().Single();
		Assert.AreEqual(effectType, (string?)effect.Attribute("type"));
		var profile = definition.Element("ControlledPower")!;
		Assert.AreEqual(content.MinimumEnergy, (double)profile.Element("Efficiency")!.Attribute("minimum")!);
		var opening = entry.TryGetProperty("classic_sorcerer_row", out var sorcerer) && sorcerer.ValueKind == JsonValueKind.Object
			? sorcerer.GetProperty("opening").GetInt32() : entry.GetProperty("classic_elemental_admissions")[0].GetProperty("opening").GetInt32();
		Assert.AreEqual(opening, (int)profile.Attribute("openingSkill")!);
		var binding = profile.Element("ScalarBindings")!.Elements().Single();
		Assert.AreEqual("target", (string?)binding.Attribute("list"));
		Assert.AreEqual(0, (int)binding.Attribute("index")!);
		Assert.AreEqual(effectType, (string?)binding.Attribute("effect"));
		Assert.AreEqual(field, (string?)binding.Attribute("field"));
		Assert.AreEqual(formula, binding.Value);
		CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), profile.Elements("Grade").Select(x => (int)x.Attribute("number")!).ToArray());
		CollectionAssert.AreEqual(Enumerable.Range((int)SpellPower.ExtremelyWeak, 7).ToArray(), profile.Elements("Grade").Select(x => (int)x.Attribute("power")!).ToArray());
		Assert.IsFalse(content.SpellRow(301, 302, 303).ScrollInscriptionAllowed);
		Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
		Assert.AreEqual(0, definition.Element("CasterEffects")!.Elements().Count());
		Assert.IsNull(profile.Element("Area"));
		Assert.IsNull(profile.Element("Incantation"));
		Assert.IsNull(content.EligibilitySource);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(0, 202, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(201, 0, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(201, 202, -1));
		if (trigger == "room")
		{
			Assert.IsNull(definition.Element("Trigger")!.Element("TargetFilterProg"));
			Assert.ThrowsException<ArgumentException>(() => content.BuildDefinition(201, 202, 203));
		}
		if (effectType == "healingrate") Assert.AreEqual(0, (int)effect.Element("Stages")!);
	}
}
