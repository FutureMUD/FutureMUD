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
	public void FixedToxinDefinitions_UseExactOwnedDrugVectorAndSelectedGradePayload()
	{
		var pair = ArmageddonToxinSpellContent.Create(509);
		CollectionAssert.AreEquivalent(new[] { ArmageddonToxinSpellContent.VenomTouchKey,
			ArmageddonToxinSpellContent.PurgeToxinKey }, pair.Select(x => x.Content.Key).ToArray());
		foreach (var contribution in pair)
		{
			Assert.AreEqual("arm.capability.water", contribution.NativeCapabilityKey);
			var definition = contribution.Content.BuildDefinition(10, 11, 0);
			var effect = definition.Element("Effects")!.Elements().Single();
			Assert.AreEqual(509L, (long)effect.Element("Drug")!);
			Assert.AreEqual((int)DrugVector.Touched, (int)effect.Element("Vector")!);
			Assert.AreEqual(ArmageddonToxinSpellContent.OwnedFamily, (string?)effect.Element("OwnedFamily"));
			Assert.AreEqual(ArmageddonToxinSpellContent.GramsFormula, (string?)effect.Element("Formula"));
			Assert.AreEqual("7*grade", contribution.Content.CostRow(1).Expression);
			Assert.AreEqual(contribution.Content.Key == ArmageddonToxinSpellContent.VenomTouchKey
				? "60*grade" : "0", contribution.Content.DurationRow(1).Expression);
		}
	}

	[TestMethod]
	public void ToxinDefinitions_RejectMissingNativeDrugIdentity()
	{
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonToxinSpellContent.Create(0));
	}
}
