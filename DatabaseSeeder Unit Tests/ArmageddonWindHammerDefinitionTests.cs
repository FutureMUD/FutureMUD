using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWindHammerDefinitionTests
{
	[TestMethod]
	public void WindHammer_DeclaresOneLinkedHitAndExitStepWithLockedEnergy()
	{
		var contribution = ArmageddonWindHammerSpellContent.Create();
		Assert.AreEqual("arm.capability.wind", contribution.NativeCapabilityKey);
		var content = contribution.Content;
		Assert.AreEqual(33.0, content.MinimumEnergy);
		Assert.AreEqual("33*grade", content.CostRow(1).Expression);
		var root = content.BuildDefinition(10, 11, 0);
		Assert.AreEqual("characterexit", (string?)root.Element("Trigger")!.Attribute("type"));
		Assert.AreEqual("false", (string?)root.Element("Trigger")!.Element("CanTargetSelf"));
		var effect = root.Element("Effects")!.Elements().Single();
		Assert.AreEqual("windhammer", (string?)effect.Attribute("type"));
		Assert.AreEqual("1", (string?)effect.Attribute("schema"));
		var damage = effect.Element("Damage")!.Elements().Single();
		Assert.AreEqual("damage", (string?)damage.Attribute("type"));
		Assert.AreEqual((int)DamageType.Shockwave, (int)damage.Element("DamageType")!);
		Assert.AreEqual("5*grade", (string?)damage.Element("DamageExpression"));
		Assert.AreEqual("0", (string?)damage.Element("PainExpression"));
		Assert.AreEqual("0", (string?)damage.Element("StunExpression"));
		Assert.AreEqual("1", (string?)damage.Element("IdentitySafeDamage")!.Attribute("version"));
		Assert.AreEqual(0, root.Element("Plan")!.Elements().Count());
	}
}
