#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.Templates;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StockStableMountTests
{
	[TestMethod]
	public void Stock_ValidContainer_CreatesOneNativeStayAndBindsItsTicket()
	{
		var world = new Mock<IFuturemud>();
		var template = new Mock<INPCTemplate>();
		var stable = new Mock<IStable>();
		var room = new Mock<IRoom>();
		var npc = new Mock<ICharacter>();
		var stay = new Mock<IStableStay>();
		var parent = new Mock<IGameItem>();
		var container = new Mock<IContainer>();
		var item = new Mock<IGameItem>();
		var ticket = new Mock<IStableTicket>();
		var contents = new List<IGameItem>();
		parent.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		item.Setup(x => x.GetItemType<IStableTicket>()).Returns(ticket.Object);
		container.SetupGet(x => x.Contents).Returns(contents);
		container.Setup(x => x.Put(null, item.Object, false)).Callback(() =>
		{
			contents.Add(item.Object);
			item.SetupGet(x => x.ContainedIn).Returns(parent.Object);
		});
		template.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		template.Setup(x => x.CreateNewCharacter(It.IsAny<SpatialLocation>())).Returns(npc.Object);
		stable.SetupGet(x => x.Location).Returns(room.Object);
		stable.Setup(x => x.RegisterStay(npc.Object, null, 20m)).Returns(stay.Object);
		var result = StockStableMountFunction.Stock(world.Object, template.Object, stable.Object, parent.Object, null, 20m, () => item.Object);
		Assert.AreSame(item.Object, result);
		stable.Verify(x => x.RegisterStay(npc.Object, null, 20m), Times.Once);
		ticket.Verify(x => x.InitialiseTicket(stay.Object), Times.Once);
		item.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void Stock_RefusedPlacement_DoesNotCreateAnAnimalOrStay()
	{
		var world = new Mock<IFuturemud>();
		var template = new Mock<INPCTemplate>();
		template.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		var stable = new Mock<IStable>();
		var parent = new Mock<IGameItem>();
		var container = new Mock<IContainer>();
		var item = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		container.SetupGet(x => x.Contents).Returns([]);
		item.Setup(x => x.GetItemType<IStableTicket>()).Returns(Mock.Of<IStableTicket>());
		Assert.IsNull(StockStableMountFunction.Stock(world.Object, template.Object, stable.Object, parent.Object, null, 20m, () => item.Object));
		template.Verify(x => x.CreateNewCharacter(It.IsAny<SpatialLocation>()), Times.Never);
		stable.Verify(x => x.RegisterStay(It.IsAny<ICharacter>(), It.IsAny<ICharacter?>(), It.IsAny<decimal>()), Times.Never);
		item.Verify(x => x.Delete(), Times.Once);
	}
}
