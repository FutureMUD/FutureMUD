#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonConsumingFlameDefinitionTests
{
	[TestMethod]
	public void ConsumingFlame_UsesNativeFireDamageAndOneCarriedConsumedComponent()
	{
		var contribution = ArmageddonDirectDamageSpellContent.ConsumingFlame(509);
		Assert.AreEqual("arm.capability.fire", contribution.NativeCapabilityKey);
		Assert.AreEqual("arm.spell.consuming_flame", contribution.Content.Key);
		var definition = contribution.Content.BuildDefinition(10, 11, 0);
		var effect = definition.Element("Effects")!.Elements().Single();
		Assert.AreEqual((int)DamageType.Burning, (int)effect.Element("DamageType")!);
		Assert.AreEqual("8*grade", (string?)effect.Element("DamageExpression"));
		Assert.AreEqual("3*grade", (string?)effect.Element("PainExpression"));
		Assert.AreEqual("0", (string?)effect.Element("StunExpression"));
		Assert.AreEqual("33*grade", contribution.Content.CostRow(1).Expression);
		var action = definition.Element("Plan")!.Elements("Phase").Single().Element("Action")!;
		Assert.AreEqual("consumed", (string?)action.Attribute("state"));
		Assert.AreEqual(509L, (long)action.Attribute("tag")!);
		Assert.AreEqual(1, (int)action.Attribute("quantity")!);
		Assert.AreEqual(true, (bool)action.Attribute("carriedonly")!);
	}

	[TestMethod]
	public void ConsumingFlame_RequiresAnExactNativeComponentTag()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonDirectDamageSpellContent.ConsumingFlame(0));
	}
}
