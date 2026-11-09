#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonToxinDefinitionTests
{

	[TestMethod]
	public void BuildDefinition_PropagatesSuppliedDrugIdentity()
	{
		foreach (var contribution in ArmageddonToxinSpellContent.Create(509))
		{
			var definition = contribution.Content.BuildDefinition(10, 11, 0);
			var effect = definition.Element("Effects")!.Elements().Single();
			Assert.AreEqual(509L, (long)effect.Element("Drug")!);
		}
	}

	[TestMethod]
	public void ToxinDefinitions_RejectMissingNativeDrugIdentity()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonToxinSpellContent.Create(0));
	}
}
