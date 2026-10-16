#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EffectScheduleDiagnosticsTests
{
	[TestMethod]
	public void DebugInfoString_ReportsConcreteEffectAndActualOwnerWithoutFiring()
	{
		var owner = new Mock<IPerceivable>();
		owner.SetupGet(x => x.FrameworkItemType).Returns("Body");
		owner.SetupGet(x => x.Id).Returns(8_000_000_001L);
		var effect = new Mock<IEffect>();
		effect.SetupGet(x => x.FrameworkItemType).Returns("NoTraitGain");
		effect.SetupGet(x => x.Owner).Returns(owner.Object);
		effect.Setup(x => x.Describe(It.IsAny<IPerceiver>())).Returns("Skill cooldown");
		var schedule = new EffectSchedule(effect.Object, TimeSpan.FromSeconds(30));

		StringAssert.Contains(schedule.DebugInfoString, "NoTraitGain on Body #8000000001: Skill cooldown");
		effect.Verify(x => x.ExpireEffect(), Times.Never);
		owner.VerifyGet(x => x.Gameworld, Times.Never);
	}
}
