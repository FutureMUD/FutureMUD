#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonFeignDeathDefinitionTests
{
	[TestMethod]
	public void FeignDeath_DeclaresLockedVoidDurationAndLivingObservationChild()
	{
		var contribution = ArmageddonFeignDeathSpellContent.Create();
		Assert.AreEqual("arm.capability.void", contribution.NativeCapabilityKey);
		var content = contribution.Content;
		Assert.AreEqual(20.0, content.MinimumEnergy);
		Assert.AreEqual("20*grade", content.CostRow(1).Expression);
		Assert.AreEqual("60*grade", content.DurationRow(1).Expression);
		var root = content.BuildDefinition(10, 11, 0);
		Assert.AreEqual("character", (string?)root.Element("Trigger")!.Attribute("type"));
		var effect = root.Element("Effects")!.Elements().Single();
		Assert.AreEqual("feigndeath", (string?)effect.Attribute("type"));
		Assert.AreEqual("1", (string?)effect.Attribute("schema"));
		Assert.AreEqual(0, root.Element("Plan")!.Elements().Count());
	}
}
