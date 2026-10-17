#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonNeedHarmDefinitionTests
{
	[TestMethod]
	public void NeedHarm_TwoDefinitionsBindNativeNeedsAndSelectedGradeDamage()
	{
		var definitions = ArmageddonNeedHarmSpellContent.Create().ToDictionary(x => x.Content.Key);
		Assert.AreEqual(2, definitions.Count);
		foreach (var key in new[] { ArmageddonNeedHarmSpellContent.DesiccateKey, ArmageddonNeedHarmSpellContent.HeadlongRevelKey })
		{
			var contribution = definitions[key];
			var fire = key == ArmageddonNeedHarmSpellContent.DesiccateKey;
			Assert.AreEqual(fire ? "arm.capability.fire" : "arm.capability.water", contribution.NativeCapabilityKey);
			Assert.AreEqual("7*grade", contribution.Content.CostRow(1).Expression);
			Assert.AreEqual("0", contribution.Content.DurationFormula);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(1, 0, 0));
			var definition = contribution.Content.BuildDefinition(1, 2, 0);
			Assert.AreEqual("character", (string?)definition.Element("Trigger")!.Attribute("type"));
			var effects = definition.Element("Effects")!.Elements().ToArray();
			Assert.AreEqual(fire ? 2 : 1, effects.Length);
			Assert.AreEqual("needdelta", (string?)effects[0].Attribute("type"));
			var binding = definition.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().Single();
			Assert.AreEqual(fire ? "ThirstDelta" : "DrunkDelta", (string?)binding.Attribute("field"));
			Assert.AreEqual(fire ? "-5*grade" : "0.01*grade", binding.Value);
			if (fire)
			{
				Assert.AreEqual("damage", (string?)effects[1].Attribute("type"));
				Assert.AreEqual((int)DamageType.Burning, (int)effects[1].Element("DamageType")!);
				Assert.AreEqual("2*grade", (string?)effects[1].Element("DamageExpression"));
			}
		}
	}
}
