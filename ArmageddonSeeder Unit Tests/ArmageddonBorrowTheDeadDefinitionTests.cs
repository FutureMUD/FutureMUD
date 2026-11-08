#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonBorrowTheDeadDefinitionTests
{
	[TestMethod]
	public void BorrowTheDead_UsesCorpseTriggerFiniteGradeDurationAndSafePossessionDefaults()
	{
		var contribution = ArmageddonBorrowTheDeadSpellContent.Create();
		Assert.AreEqual("arm.capability.void", contribution.NativeCapabilityKey);
		var content = contribution.Content;
		Assert.AreEqual(33.0, content.MinimumEnergy);
		Assert.AreEqual("60*grade", content.DurationFormula);
		Assert.AreEqual("33*grade", content.CostRow(1).Expression);
		var definition = content.BuildDefinition(10, 11, 0);
		Assert.AreEqual("corpse", (string?)definition.Element("Trigger")!.Attribute("type"));
		var effect = definition.Element("Effects")!.Elements().Single();
		Assert.AreEqual("possesscorpse", (string?)effect.Attribute("type"));
		Assert.AreEqual("false", (string?)effect.Element("AllowAdmins"));
		Assert.AreEqual("false", (string?)effect.Element("AllowSkeletal"));
		Assert.AreEqual("true", (string?)effect.Element("AllowFinal"));
		Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
	}
}
