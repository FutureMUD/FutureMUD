#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ZeroGravityTetherSecurityTests
{
	[DataTestMethod]
	[DataRow(true, false)]
	[DataRow(false, true)]
	public void MissingSavedAnchorOrTether_IsRejectedAndSafeToInspect(bool missingAnchor, bool missingTether)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<IFutureProg>>());
		var anchor = Mock.Of<IGameItem>();
		world.Setup(x => x.GetPerceivable("GameItem", 20)).Returns(missingAnchor ? null! : anchor);
		world.Setup(x => x.GetPerceivable("GameItem", 21)).Returns(missingTether ? null! : Mock.Of<IGameItem>());
		var owner = Mock.Of<IPerceivable>(x => x.Gameworld == world.Object);
		var xml = new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Effect",
			new XElement("AnchorType", "GameItem"), new XElement("AnchorId", 20),
			new XElement("PhysicalTetherId", 21), new XElement("MaximumRooms", 3)));
		var effect = (ZeroGravityTether)typeof(ZeroGravityTether)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(XElement), typeof(IPerceivable)], null)!
			.Invoke([xml, owner]);
		Assert.IsTrue(effect.LoadErrors);
		Assert.IsFalse(effect.SavingEffect);
		Assert.IsFalse(effect.BlocksMovementTo(Mock.Of<IRoom>()));
		StringAssert.Contains(effect.Describe(null!), "no longer");
		Assert.IsNotNull(effect.SaveToXml(new Dictionary<IEffect, System.TimeSpan>()));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DeletingLiveAnchorOrTether_RemovesEffectAndReleasesHandlers(bool deleteTether)
	{
		var owner = new Mock<IPerceivable>();
		var anchor = new Mock<IRoom>();
		var tether = new Mock<IGameItem>();
		var effect = new ZeroGravityTether(owner.Object, anchor.Object, 2, tether.Object);
		effect.InitialEffect();
		effect.Login();
		Assert.IsFalse(effect.BlocksMovementTo(anchor.Object));
		if (deleteTether) tether.Raise(x => x.OnDeleted += null, tether.Object);
		else anchor.Raise(x => x.OnDeleted += null, anchor.Object);
		owner.Verify(x => x.RemoveEffect(effect, true), Times.Once);
		Assert.IsFalse(effect.SavingEffect);
		Assert.IsFalse(effect.BlocksMovementTo(Mock.Of<IRoom>()));
		anchor.Raise(x => x.OnDeleted += null, anchor.Object);
		tether.Raise(x => x.OnDeleted += null, tether.Object);
		owner.Verify(x => x.RemoveEffect(effect, true), Times.Once);
	}

	[TestMethod]
	public void Untethering_ReleasesAnchorHandlers()
	{
		var owner = new Mock<IPerceivable>();
		var anchor = new Mock<IPerceivable>();
		var effect = new ZeroGravityTether(owner.Object, anchor.Object, 2);
		effect.InitialEffect();
		effect.RemovalEffect();
		anchor.Raise(x => x.OnDeleted += null, anchor.Object);
		owner.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()), Times.Never);
	}
}
