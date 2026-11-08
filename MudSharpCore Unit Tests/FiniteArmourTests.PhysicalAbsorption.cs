#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Health;
using Moq;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

public partial class FiniteArmourTests
{
	[TestMethod]
	public void PhysicalAbsorption_RealBodyPipelineSubtractsBeforeMultiplicativeProtection()
	{
		var f = ProtectionFixture();
		f.Victim.Setup(x => x.EffectsOfType<IPhysicalDamageAbsorptionEffect>(It.IsAny<Predicate<IPhysicalDamageAbsorptionEffect>>()))
			.Returns(() => f.Effects.OfType<IPhysicalDamageAbsorptionEffect>());
		f.Effects.Add(new PhysicalDamageAbsorptionEffect(f.Victim.Object, 5));
		f.Effects.Add(new DamageProtectionStatus(f.Victim.Object, 0.5, 0));
		IDamage? damage = f.Packet(25);
		IDamage? inward = f.Packet(15);
		Assert.IsTrue(f.Body.TryApplyDamageProtection(ref damage, ref inward));
		Assert.AreEqual(10d, damage!.DamageAmount, 0.000001);
		Assert.AreEqual(5d, inward!.DamageAmount, 0.000001);
	}
}
