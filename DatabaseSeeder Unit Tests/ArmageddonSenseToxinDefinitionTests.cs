#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonSenseToxinDefinitionTests
{
	[TestMethod]
	public void SenseToxin_LockedRetainedPermissionAndGradeDefaults()
	{
		var contribution = ArmageddonSenseToxinSpellContent.Create();
		Assert.AreEqual("arm.capability.water", contribution.NativeCapabilityKey);
		var content = contribution.Content;
		Assert.AreEqual(ArmageddonSenseToxinSpellContent.Key, content.Key);
		Assert.AreEqual(7.0, content.MinimumEnergy);
		Assert.AreEqual("60*grade", content.DurationFormula);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(0, 11, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(10, 0, 0));
		var definition = content.BuildDefinition(10, 11, 0);
		Assert.AreEqual("character", definition.Element("Trigger")!.Attribute("type")!.Value);
		Assert.AreEqual("7*grade", content.CostRow(1).Expression);
		Assert.AreEqual(7, definition.Element("ControlledPower")!.Elements("Grade").Count());
		Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
		var effect = definition.Element("Effects")!.Elements().Single();
		Assert.AreEqual("detectpoison", effect.Attribute("type")!.Value);
		Assert.AreEqual(true, (bool)effect.Element("RetainedPermission")!);
	}
}
