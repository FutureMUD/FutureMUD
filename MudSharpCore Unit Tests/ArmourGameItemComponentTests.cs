#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmourGameItemComponentTests
{
	[TestMethod]
	public void CreateArmourUseContext_FullyAbsorbedDamage_RecordsZeroPassedDamage()
	{
		var damage = new Mock<IDamage>();
		damage.SetupGet(x => x.DamageAmount).Returns(10.0);
		damage.SetupGet(x => x.PenetrationOutcome).Returns(Outcome.Fail);

		var context = ArmourGameItemComponent.CreateArmourUseContext(damage.Object, null);

		Assert.AreEqual(ItemConditionUseKind.ArmourAbsorb, context.UseKind);
		Assert.AreEqual(Outcome.Fail, context.Outcome);
		Assert.AreEqual(10.0, context.Damage);
		Assert.AreEqual(10.0, context.Absorbed);
		Assert.AreEqual(0.0, context.Passed);
	}

	[TestMethod]
	public void CreateArmourUseContext_PartiallyAbsorbedDamage_RecordsPassedAndAbsorbedDamage()
	{
		var damage = new Mock<IDamage>();
		damage.SetupGet(x => x.DamageAmount).Returns(10.0);
		damage.SetupGet(x => x.PenetrationOutcome).Returns(Outcome.Pass);
		var passThroughDamage = new Mock<IDamage>();
		passThroughDamage.SetupGet(x => x.DamageAmount).Returns(4.0);

		var context = ArmourGameItemComponent.CreateArmourUseContext(damage.Object, passThroughDamage.Object);

		Assert.AreEqual(Outcome.Pass, context.Outcome);
		Assert.AreEqual(10.0, context.Damage);
		Assert.AreEqual(6.0, context.Absorbed);
		Assert.AreEqual(4.0, context.Passed);
	}
}
