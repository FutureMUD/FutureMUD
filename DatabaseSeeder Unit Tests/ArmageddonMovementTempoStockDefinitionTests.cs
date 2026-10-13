#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonMovementTempoStockDefinitionTests
{
	[TestMethod]
	public void MovementTempo_ThreeLockedDefaultsUseNativeMovementEffectAndSelectedGradePolicy()
	{
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonMovementTempoSpellContent.Create(0));
		var rows = ArmageddonMovementTempoSpellContent.Create(42).ToDictionary(x => x.Content.Key);
		Assert.AreEqual(3, rows.Count);
		foreach (var (key, school, minimum, slow, selfOnly) in new[]
		{
			("arm.spell.fleet_step", "earth", 10.0, false, true),
			("arm.spell.lightning_pace", "lightning", 33.0, false, true),
			("arm.spell.leadfoot", "lightning", 33.0, true, false)
		})
		{
			var contribution = rows[key];
			Assert.AreEqual("arm.capability." + school, contribution.NativeCapabilityKey);
			var content = contribution.Content;
			Assert.AreEqual(minimum, content.MinimumEnergy);
			Assert.AreEqual("60*grade", content.DurationFormula);
			Assert.AreEqual(selfOnly ? "return @target == @caster" : null, content.EligibilitySource);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(1, 2, -1));
			if (selfOnly) Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(1, 2, 0));
			var definition = content.BuildDefinition(1, 2, selfOnly ? 3 : 0);
			Assert.AreEqual("character", (string?)definition.Element("Trigger")!.Attribute("type"));
			var effects = definition.Element("Effects")!.Elements().ToArray();
			Assert.AreEqual(2, effects.Length);
			var effect = effects[0];
			Assert.AreEqual("movementtempo", (string?)effect.Attribute("type"));
			Assert.AreEqual(slow, (bool)effect.Element("Slow")!);
			Assert.AreEqual("0.05", effect.Element("RatePerGrade")!.Value);
			if (key == "arm.spell.fleet_step")
			{
				Assert.AreEqual("staminaexpendrate", (string?)effects[1].Attribute("type"));
				Assert.AreEqual("0.95", effects[1].Element("Multiplier")!.Value);
				Assert.IsFalse(definition.Element("ControlledPower")!.Element("ScalarBindings")!.HasElements);
			}
			else
			{
				Assert.AreEqual("boost", (string?)effects[1].Attribute("type"));
				Assert.AreEqual(42L, (long)effects[1].Attribute("trait")!);
				var binding = definition.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().Single();
				Assert.AreEqual("1", (string?)binding.Attribute("index"));
				Assert.AreEqual(slow ? "-grade" : "grade", binding.Value);
			}
			Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
		}
	}
}
