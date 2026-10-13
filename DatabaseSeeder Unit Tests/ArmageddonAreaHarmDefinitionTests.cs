#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonAreaHarmDefinitionTests
{
	[TestMethod]
	public void AreaHarm_FourBoundedPhysicalRecipientPolicies_KeepDistinctLockedEffects()
	{
		const long resistance = 17;
		var contributions = ArmageddonAreaHarmSpellContent.Create(resistance).ToDictionary(x => x.Content.Key);
		Assert.AreEqual(4, contributions.Count);
		foreach (var (key, contribution) in contributions)
		{
			var content = contribution.Content;
			var definition = content.BuildDefinition(10, 11, 0);
			Assert.AreEqual("character", (string?)definition.Element("Trigger")!.Attribute("type"), key);
			Assert.AreEqual("false", (string?)definition.Element("Trigger")!.Element("CanTargetSelf"), key);
			var area = definition.Element("ControlledPower")!.Element("Area")!;
			foreach (var (attribute, value) in new[] { ("schema", "1"), ("scope", "0"), ("selection", "0"),
				("identity", "0"), ("targets", "256"), ("applications", "256"), ("caster", "false"),
				("allies", "true"), ("others", "true"), ("layer", "true"), ("staff", "true") })
				Assert.AreEqual(value, (string?)area.Attribute(attribute), key + "/" + attribute);
			Assert.AreEqual("Say", (string?)area.Element("Method")!.Attribute("name"), key);
			var effects = definition.Element("Effects")!.Elements("Effect").ToArray();
			Assert.AreEqual(key == ArmageddonAreaHarmSpellContent.CausticSprayKey ? 1 : 2, effects.Length, key);
			Assert.AreEqual("damage", (string?)effects[0].Attribute("type"), key);
			Assert.AreEqual("3*grade", (string?)effects[0].Element("DamageExpression"), key);
			Assert.IsNotNull(effects[0].Element("IdentitySafeDamage"), key);
		}
		var cinder = contributions[ArmageddonAreaHarmSpellContent.CinderRainKey].Content;
		Assert.AreEqual("arm.capability.fire", contributions[cinder.Key].NativeCapabilityKey);
		Assert.AreEqual("30*grade", cinder.DurationFormula);
		var burning = cinder.BuildDefinition(10, 11, 0).Element("Effects")!.Elements("Effect").Last();
		Assert.AreEqual("burning", (string?)burning.Attribute("type"));
		Assert.AreEqual("2*grade", (string?)burning.Element("DamageFormula"));
		Assert.AreEqual("10", (string?)burning.Element("TickSeconds"));
		var thunder = contributions[ArmageddonAreaHarmSpellContent.ThunderclapKey].Content;
		Assert.AreEqual("arm.capability.water", contributions[thunder.Key].NativeCapabilityKey);
		Assert.AreEqual(resistance, thunder.ResistingTraitId);
		Assert.AreEqual("30*grade", thunder.DurationFormula);
		Assert.AreEqual("deafness", (string?)thunder.BuildDefinition(10, 11, 0).Element("Effects")!.Elements("Effect").Last().Attribute("type"));
		var gust = contributions[ArmageddonAreaHarmSpellContent.ScouringGustKey].Content;
		Assert.AreEqual("arm.capability.wind", contributions[gust.Key].NativeCapabilityKey);
		var stamina = gust.BuildDefinition(10, 11, 0).Element("Effects")!.Elements("Effect").Last();
		Assert.AreEqual("staminadelta", (string?)stamina.Attribute("type"));
		Assert.AreEqual("-grade", (string?)stamina.Element("Formula"));
		Assert.AreEqual("true", (string?)stamina.Element("DirectStaminaAdjustment"));
		var acid = contributions[ArmageddonAreaHarmSpellContent.CausticSprayKey].Content;
		Assert.AreEqual(8.0, acid.MinimumEnergy);
		Assert.AreEqual((int)DamageType.Chemical, (int)acid.BuildDefinition(10, 11, 0).Element("Effects")!.Element("Effect")!.Element("DamageType")!);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonAreaHarmSpellContent.Create(0));
	}
}
