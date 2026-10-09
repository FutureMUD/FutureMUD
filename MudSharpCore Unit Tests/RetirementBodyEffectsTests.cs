#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RetirementBodyEffectsTests
{
	private sealed class OtherSpinalEffect(IBody owner, ILimb limb) : LimbSpinalDamageEffect(owner, limb);

	[TestMethod]
	public void CapturedDerivedEffects_PreserveInstancesAndDetectChangedLimbOrMembership()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var limb = Mock.Of<ILimb>();
		var body = new Mock<IBody>(); body.SetupGet(x => x.Gameworld).Returns(world.Object);
		body.SetupGet(x => x.Actor).Returns(Mock.Of<ICharacter>(x => x.State == CharacterState.Dead));
		body.SetupGet(x => x.Limbs).Returns([limb]);
		var spinal = new LimbSpinalDamageEffect(body.Object, limb);
		var effects = new List<IEffect> { spinal }; body.SetupGet(x => x.Effects).Returns(effects);
		Assert.IsTrue(RetirementBodyEffects.TryCapture(body.Object, out var unchanged));
		Assert.IsTrue(unchanged()); Assert.AreSame(spinal, effects[0]);
		spinal.Limb = Mock.Of<ILimb>(); Assert.IsFalse(unchanged());
		spinal.Limb = limb; effects.Clear(); Assert.IsFalse(unchanged());
		effects.Add(new LimbSpinalDamageEffect(body.Object, limb)); Assert.IsFalse(unchanged());
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	[DataRow(4)]
	[DataRow(5)]
	[DataRow(6)]
	public void UnprovenEffectsOrChangedDerivedState_Refuse(int corruption)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var limb = Mock.Of<ILimb>(); var body = new Mock<IBody>();
		body.SetupGet(x => x.Gameworld).Returns(world.Object);
		body.SetupGet(x => x.Actor).Returns(Mock.Of<ICharacter>(x => x.State == CharacterState.Dead));
		body.SetupGet(x => x.Limbs).Returns([limb]);
		IEffect effect = new LimbSpinalDamageEffect(body.Object, limb);
		switch (corruption)
		{
			case 0: body.SetupGet(x => x.Actor).Returns(Mock.Of<ICharacter>(x => x.State == CharacterState.Awake)); break;
			case 1: effect = Mock.Of<IEffect>(); break;
			case 2: effect = new OtherSpinalEffect(body.Object, limb); break;
			case 3: effect = new LimbSpinalDamageEffect(Mock.Of<IBody>(), limb); break;
			case 4: effect = new LimbSpinalDamageEffect(body.Object, Mock.Of<ILimb>()); break;
			case 5: ((LimbSpinalDamageEffect)effect).ApplicabilityProg = Mock.Of<IFutureProg>(); break;
			case 6: world.Setup(x => x.EffectScheduler.IsScheduled(effect)).Returns(true); break;
		}
		body.SetupGet(x => x.Effects).Returns([effect]);
		Assert.IsFalse(RetirementBodyEffects.TryCapture(body.Object, out var unchanged));
		Assert.IsFalse(unchanged());
	}
}
