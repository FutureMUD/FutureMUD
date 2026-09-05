#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TelekineticManipulationTests
{
	[TestMethod]
	[DataRow(220.0, 0.001, 10.0, true)]
	[DataRow(10000.0, 0.001, 10.0, true)]
	[DataRow(10001.0, 0.001, 10.0, false)]
	[DataRow(11.0, 1.0, 10.0, false)]
	[DataRow(-1.0, 0.001, 10.0, false)]
	[DataRow(220.0, 0.0, 10.0, false)]
	[DataRow(double.NaN, 0.001, 10.0, false)]
	public void MassLimit_UsesConfiguredWorldUnits(double weight, double conversion, double limit, bool expected)
	{
		Assert.AreEqual(expected, TelekineticManipulation.IsWithinMassLimit(weight, conversion, limit));
	}

	[TestMethod]
	public void Switch_OnlyPermittedMatchingComponentsExecuteAfterPreparation()
	{
		var actor = new Mock<ICharacter>();
		var item = new Mock<IGameItem>();
		var component = new Mock<ISwitchable>();
		component.SetupGet(x => x.SwitchSettings).Returns(["on"]);
		component.Setup(x => x.CanSwitch(actor.Object, "on")).Returns(true);
		component.Setup(x => x.Switch(actor.Object, "on")).Returns(true);
		item.Setup(x => x.GetItemTypes<ISwitchable>()).Returns([component.Object]);
		Assert.IsTrue(TelekineticManipulation.TryPrepare(actor.Object, item.Object, "switch", new("on"), _ => true, out var execute, out _));
		component.Verify(x => x.Switch(It.IsAny<ICharacter>(), It.IsAny<string>()), Times.Never);
		Assert.IsTrue(execute());
		component.Verify(x => x.Switch(actor.Object, "on"), Times.Once);
		Assert.IsFalse(TelekineticManipulation.TryPrepare(actor.Object, item.Object, "switch", new("off"), _ => true, out _, out _));
	}

	[TestMethod]
	public void Select_ForwardsOptionAndHonoursComponentRejection()
	{
		var actor = new Mock<ICharacter>();
		var item = new Mock<IGameItem>();
		var component = new Mock<ISelectable>();
		component.Setup(x => x.CanSelect(actor.Object, "channel two")).Returns(true);
		item.Setup(x => x.GetItemTypes<ISelectable>()).Returns([component.Object]);
		Assert.IsTrue(TelekineticManipulation.TryPrepare(actor.Object, item.Object, "select", new("channel two"), _ => true, out var execute, out _));
		Assert.IsFalse(execute());
		component.Verify(x => x.Select(actor.Object, "channel two", null, true), Times.Once);
		Assert.IsFalse(TelekineticManipulation.TryPrepare(actor.Object, item.Object, "select", new("private"), _ => true, out _, out _));
	}

	[TestMethod]
	public void IneligibleObject_IsRejectedBeforeAnyComponentOperation()
	{
		var item = new Mock<IGameItem>(MockBehavior.Strict);
		Assert.IsFalse(TelekineticManipulation.TryPrepare(Mock.Of<ICharacter>(), item.Object, "open", new(""), _ => false, out _, out _));
		item.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void Pour_ClampsToDestinationCapacityAndDoesNotMutateDuringPreflight()
	{
		var actor = new Mock<ICharacter>();
		var source = new Mock<IGameItem>();
		var destination = new Mock<IGameItem>();
		var cell = new Mock<ICell>();
		source.SetupGet(x => x.Location).Returns(cell.Object);
		source.Setup(x => x.CanGet(0)).Returns(ItemGetResponse.CanGet);
		cell.Setup(x => x.CanGet(source.Object, actor.Object)).Returns(true);
		actor.Setup(x => x.TargetLocalItem("cup")).Returns(destination.Object);
		var from = new Mock<ILiquidContainer>();
		var to = new Mock<ILiquidContainer>();
		var liquid = new LiquidMixture(Mock.Of<ILiquid>(), 10, null);
		from.SetupGet(x => x.IsOpen).Returns(true);
		from.SetupGet(x => x.CanBeEmptiedWhenInRoom).Returns(true);
		from.SetupGet(x => x.LiquidMixture).Returns(liquid);
		from.SetupGet(x => x.LiquidVolume).Returns(10);
		from.Setup(x => x.RemoveLiquidAmount(2, actor.Object, "pour")).Returns(liquid);
		to.SetupGet(x => x.IsOpen).Returns(true);
		to.SetupGet(x => x.LiquidCapacity).Returns(5);
		to.SetupGet(x => x.LiquidVolume).Returns(3);
		source.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(from.Object);
		destination.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(to.Object);
		Assert.IsTrue(TelekineticManipulation.TryPrepare(actor.Object, source.Object, "pour", new("cup"), _ => true, out var execute, out _));
		from.Verify(x => x.RemoveLiquidAmount(It.IsAny<double>(), It.IsAny<ICharacter>(), It.IsAny<string>()), Times.Never);
		Assert.IsTrue(execute());
		to.Verify(x => x.MergeLiquid(liquid, actor.Object, "pour"), Times.Once);
		from.Verify(x => x.RemoveLiquidAmount(2, actor.Object, "pour"), Times.Once);
		to.SetupGet(x => x.IsOpen).Returns(false);
		Assert.IsFalse(TelekineticManipulation.TryPrepare(actor.Object, source.Object, "pour", new("cup"), _ => true, out _, out _));
	}

	[TestMethod]
	public void Empty_UnknownDestinationDoesNotFallBackToDumpingOnGround()
	{
		Assert.IsFalse(TelekineticManipulation.TryPrepare(Mock.Of<ICharacter>(), Mock.Of<IGameItem>(), "empty", new("missing"), _ => true, out _, out _));
	}
}
