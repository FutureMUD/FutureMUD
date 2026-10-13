#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWindForceDefinitionTests
{
	[TestMethod]
	public void WindForce_ExactExitMovementDefinitionsHaveBoundedSelectedGradePolicy()
	{
		var definitions = ArmageddonWindForceSpellContent.Create().ToDictionary(x => x.Content.Key);
		Assert.AreEqual(2, definitions.Count);
		foreach (var key in new[] { ArmageddonWindForceSpellContent.GustHandsKey, ArmageddonWindForceSpellContent.DrivingGustKey })
		{
			var contribution = definitions[key];
			Assert.AreEqual("arm.capability.wind", contribution.NativeCapabilityKey);
			Assert.AreEqual("7*grade", contribution.Content.CostRow(1).Expression);
			Assert.AreEqual("0", contribution.Content.DurationFormula);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(0, 2, 0));
			var definition = contribution.Content.BuildDefinition(1, 2, 0);
			Assert.AreEqual("characterexit", (string?)definition.Element("Trigger")!.Attribute("type"));
			Assert.AreEqual(false, (bool)definition.Element("Trigger")!.Element("CanTargetSelf")!);
			var effect = definition.Element("Effects")!.Elements().Single();
			Assert.AreEqual("forcedpathmovement", (string?)effect.Attribute("type"));
			Assert.AreEqual(1, (int)effect.Element("Steps")!);
			Assert.AreEqual(false, (bool)effect.Element("AllowFallExits")!);
			var bindings = definition.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().ToArray();
			if (key == ArmageddonWindForceSpellContent.GustHandsKey)
			{
				Assert.AreEqual(1, bindings.Length);
				Assert.AreEqual("forcedpathmovement", (string?)bindings[0].Attribute("effect"));
				Assert.AreEqual("Steps", (string?)bindings[0].Attribute("field"));
				Assert.AreEqual("grade", bindings[0].Value);
			}
			else Assert.AreEqual(0, bindings.Length);
		}
	}
}
