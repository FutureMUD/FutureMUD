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
	public void BuildDefinition_PropagatesSuppliedComponentTag()
	{
		var contribution = ArmageddonDirectDamageSpellContent.ConsumingFlame(509);
		var definition = contribution.Content.BuildDefinition(10, 11, 0);
		var action = definition.Element("Plan")!.Elements("Phase").Single().Element("Action")!;
		Assert.AreEqual(509L, (long)action.Attribute("tag")!);
	}

	[TestMethod]
	public void ConsumingFlame_RequiresAnExactNativeComponentTag()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonDirectDamageSpellContent.ConsumingFlame(0));
	}
}
