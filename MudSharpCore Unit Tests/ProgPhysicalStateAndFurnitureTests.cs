#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.Characters;
using MudSharp.FutureProg.Functions.GameItem;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProgPhysicalStateAndFurnitureTests
{
	private static Mock<T> ProgMock<T>() where T : class, IProgVariable
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.GetObject).Returns(mock.Object);
		return mock;
	}

	[TestMethod]
	public void DrunkennessLevel_MapsNativeStatusToSeverity()
	{
		foreach (var (status, expected) in new[]
		{
			(NeedsResult.Sober, 0), (NeedsResult.Buzzed, 1), (NeedsResult.Tipsy, 2),
			(NeedsResult.Drunk, 3), (NeedsResult.VeryDrunk, 4),
			(NeedsResult.BlackoutDrunk, 5), (NeedsResult.Paralytic, 5)
		})
		{
			var actor = ProgMock<ICharacter>();
			var needs = new Mock<INeedsModel>();
			needs.SetupGet(x => x.Status).Returns(status);
			actor.SetupGet(x => x.NeedsModel).Returns(needs.Object);
			var function = new PhysicalStateFunction([new ConstantFunction(actor.Object)], "drunkennesslevel");
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			Assert.AreEqual(expected, Convert.ToInt32(function.Result.GetObject));
		}
	}

	[TestMethod]
	public void SpendStamina_RequiresNativeAffordabilityAndNeverSpendsOnFailure()
	{
		var body = new Mock<IBody>();
		body.Setup(x => x.CanSpendStamina(4.5)).Returns(true);
		body.Setup(x => x.CanSpendStamina(5.0)).Returns(false);
		var actor = ProgMock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);

		var affordable = Spend(actor.Object, 4.5m);
		Assert.AreEqual(StatementResult.Normal, affordable.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, affordable.Result.GetObject);
		body.Verify(x => x.SpendStamina(4.5), Times.Once);

		var unaffordable = Spend(actor.Object, 5.0m);
		unaffordable.Execute(Mock.Of<IVariableSpace>());
		Assert.AreEqual(false, unaffordable.Result.GetObject);
		var invalid = Spend(actor.Object, -1m);
		invalid.Execute(Mock.Of<IVariableSpace>());
		Assert.AreEqual(false, invalid.Result.GetObject);
		body.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Once,
			"Neither an unaffordable nor invalid amount should spend stamina.");
		body.Verify(x => x.CanSpendStamina(5.0), Times.Once);
	}

	[TestMethod]
	public void IsOffhand_UsesCurrentLeftRightHandAlignment()
	{
		var body = new Mock<IBody>();
		var actor = ProgMock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Handedness).Returns(Alignment.Right);
		var item = ProgMock<IGameItem>();
		var leftHand = new Mock<IBodypart>();
		leftHand.SetupGet(x => x.Alignment).Returns(Alignment.Left);
		var rightHand = new Mock<IBodypart>();
		rightHand.SetupGet(x => x.Alignment).Returns(Alignment.Right);
		var neutralLocation = new Mock<IBodypart>();
		neutralLocation.SetupGet(x => x.Alignment).Returns(Alignment.Irrelevant);

		body.Setup(x => x.HoldOrWieldLocFor(item.Object)).Returns(leftHand.Object);
		Assert.IsTrue(ExecuteOffhand(actor.Object, item.Object));
		body.Setup(x => x.HoldOrWieldLocFor(item.Object)).Returns(rightHand.Object);
		Assert.IsFalse(ExecuteOffhand(actor.Object, item.Object));
		body.Setup(x => x.HoldOrWieldLocFor(item.Object)).Returns(neutralLocation.Object);
		Assert.IsFalse(ExecuteOffhand(actor.Object, item.Object));
	}

	[TestMethod]
	public void AttachChair_UsesTableCapacityAndReusesExistingAssignment()
	{
		var (chairItem, chair, tableItem, table, function) = ChairAndTable();
		table.SetupGet(x => x.Chairs).Returns(Array.Empty<IChair>());
		table.Setup(x => x.CanAddChair(null!, chair.Object)).Returns(false);
		Assert.IsFalse(Execute(function));
		chair.Verify(x => x.SetTable(It.IsAny<ITable>(), It.IsAny<bool>()), Times.Never);
		table.Verify(x => x.CanAddChair(null!, chair.Object), Times.Once);

		chair.SetupGet(x => x.Table).Returns(table.Object);
		Assert.IsTrue(Execute(function));
		chair.Verify(x => x.SetTable(It.IsAny<ITable>(), It.IsAny<bool>()), Times.Never,
			"The same existing chair-table assignment is idempotent.");
	}

	[TestMethod]
	public void AttachChair_DoesNotStealChairFromAnotherTable()
	{
		var (chairItem, chair, tableItem, table, function) = ChairAndTable();
		var originalTable = new Mock<ITable>();
		chair.SetupGet(x => x.Table).Returns(originalTable.Object);
		table.Setup(x => x.CanAddChair(null!, chair.Object)).Returns(true);
		Assert.IsFalse(Execute(function));
		chair.Verify(x => x.SetTable(It.IsAny<ITable>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void AttachChair_RejectsSelfAndNestedContainmentCycles()
	{
		var sharedItem = ProgMock<IGameItem>();
		var selfChair = new Mock<IChair>();
		var selfTable = new Mock<ITable>();
		selfChair.SetupGet(x => x.Parent).Returns(sharedItem.Object);
		selfTable.SetupGet(x => x.Parent).Returns(sharedItem.Object);
		sharedItem.Setup(x => x.GetItemType<IChair>()).Returns(selfChair.Object);
		sharedItem.Setup(x => x.GetItemType<ITable>()).Returns(selfTable.Object);
		var selfFunction = new AttachChairFunction([new ConstantFunction(sharedItem.Object), new ConstantFunction(sharedItem.Object)]);
		Assert.IsFalse(Execute(selfFunction));
		selfChair.Verify(x => x.SetTable(It.IsAny<ITable>(), It.IsAny<bool>()), Times.Never);

		var (chairItem, chair, tableItem, table, nestedFunction) = ChairAndTable();
		var container = ProgMock<IGameItem>();
		tableItem.SetupGet(x => x.ContainedIn).Returns(container.Object);
		container.SetupGet(x => x.ContainedIn).Returns(chairItem.Object);
		table.Setup(x => x.CanAddChair(null!, chair.Object)).Returns(true);
		Assert.IsFalse(Execute(nestedFunction));
		chair.Verify(x => x.SetTable(It.IsAny<ITable>(), It.IsAny<bool>()), Times.Never);
	}

	private static PhysicalStateFunction Spend(ICharacter actor, decimal amount) =>
		new([new ConstantFunction(actor), new ConstantFunction(new NumberVariable(amount))], "spendstamina");

	private static bool ExecuteOffhand(ICharacter actor, IGameItem item)
	{
		var function = new PhysicalStateFunction([new ConstantFunction(actor), new ConstantFunction(item)], "isoffhand");
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		return (bool)function.Result.GetObject;
	}

	private static AttachChairFunction AttachChairEffect(IList<IFunction> parameters) => new(parameters);

	private static bool Execute(IFunction function)
	{
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		return (bool)function.Result.GetObject;
	}

	private static (Mock<IGameItem> ChairItem, Mock<IChair> Chair, Mock<IGameItem> TableItem, Mock<ITable> Table,
		AttachChairFunction Function) ChairAndTable()
	{
		var chairItem = ProgMock<IGameItem>();
		var chair = new Mock<IChair>();
		chair.SetupGet(x => x.Parent).Returns(chairItem.Object);
		chairItem.Setup(x => x.GetItemType<IChair>()).Returns(chair.Object);
		chair.SetupGet(x => x.Table).Returns((ITable)null!);
		var tableItem = ProgMock<IGameItem>();
		var table = new Mock<ITable>();
		table.SetupGet(x => x.Parent).Returns(tableItem.Object);
		tableItem.Setup(x => x.GetItemType<ITable>()).Returns(table.Object);
		return (chairItem, chair, tableItem, table,
			new AttachChairFunction([new ConstantFunction(chairItem.Object), new ConstantFunction(tableItem.Object)]));
	}
}
