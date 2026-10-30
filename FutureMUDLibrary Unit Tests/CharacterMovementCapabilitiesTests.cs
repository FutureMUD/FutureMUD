#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterMovementCapabilitiesTests
{
	[TestMethod]
	public void ClimbingCapability_CombinesRaceAndApplicableEffectsWithoutBypassingNativeRestrictions()
	{
		var actor = new Mock<ICharacter>(); var race = new Mock<IRace>(); var effect = new Mock<IClimbingEffect>();
		actor.SetupGet(x => x.Race).Returns(race.Object);
		actor.Setup(x => x.CombinedEffectsOfType<IClimbingEffect>()).Returns([effect.Object]);
		Assert.IsFalse(actor.Object.HasClimbingAbility());
		effect.Setup(x => x.Applies()).Returns(true); Assert.IsTrue(actor.Object.HasClimbingAbility());
		effect.Setup(x => x.Applies()).Returns(false); Assert.IsFalse(actor.Object.HasClimbingAbility());
		race.SetupGet(x => x.CanClimb).Returns(true); Assert.IsTrue(actor.Object.HasClimbingAbility());
		actor.Setup(x => x.CombinedEffectsOfType<IClimbingEffect>()).Returns([]); Assert.IsTrue(actor.Object.HasClimbingAbility());
		race.SetupGet(x => x.CanClimb).Returns(false); Assert.IsFalse(actor.Object.HasClimbingAbility());
	}
}
