#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonImmolateDefinitionTests
{
	[TestMethod]
	public void Immolate_AuthoredNativeStrikeAndPersistentFire_MatchLockedGradeContract()
	{
		var contribution = ArmageddonImmolateSpellContent.Create();
		Assert.AreEqual("arm.capability.fire", contribution.NativeCapabilityKey);
		var content = contribution.Content;
		Assert.AreEqual("arm.spell.immolate", content.Key);
		Assert.AreEqual(20.0, content.MinimumEnergy);
		Assert.AreEqual("30*grade", content.DurationFormula);
		var definition = content.BuildDefinition(10, 11, 0);
		Assert.AreEqual("character", definition.Element("Trigger")!.Attribute("type")!.Value);
		Assert.AreEqual("false", definition.Element("Trigger")!.Element("CanTargetSelf")!.Value);
		var effects = definition.Element("Effects")!.Elements().ToArray();
		Assert.AreEqual(2, effects.Length);
		Assert.AreEqual("damage", effects[0].Attribute("type")!.Value);
		Assert.AreEqual((int)DamageType.Burning, (int)effects[0].Element("DamageType")!);
		Assert.AreEqual("3*grade", effects[0].Element("DamageExpression")!.Value);
		Assert.AreEqual("0", effects[0].Element("PainExpression")!.Value);
		Assert.IsNotNull(effects[0].Element("IdentitySafeDamage"));
		Assert.AreEqual("burning", effects[1].Attribute("type")!.Value);
		Assert.AreEqual("2*grade", effects[1].Element("DamageFormula")!.Value);
		Assert.AreEqual("10", effects[1].Element("TickSeconds")!.Value);
		Assert.AreEqual("true", effects[1].Element("SelfOxidising")!.Value);
	}
}
