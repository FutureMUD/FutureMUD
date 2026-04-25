using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using System;
using System.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StableTicketComponentTests
{
	[TestMethod]
	public void RegisterComponentInitialiser_StableTicket_IsDatabaseOnlyAndReadOnly()
	{
		var manager = new GameItemComponentManager();
		var gameworld = new Mock<IFuturemud>();
		var account = new Mock<IAccount>();

		Assert.IsFalse(manager.PrimaryTypes.Any(x => x.Equals("stableticket", StringComparison.OrdinalIgnoreCase)));
		Assert.IsNull(manager.GetProto("stableticket", gameworld.Object, account.Object));

		var proto = manager.GetProto(CreateStableTicketDbProto(), gameworld.Object);

		Assert.IsInstanceOfType(proto, typeof(StableTicketGameItemComponentProto));
		Assert.IsTrue(proto.ReadOnly);
		Assert.IsTrue(proto.PreventManualLoad);
	}

	[TestMethod]
	public void Copy_StableTicketToDifferentItem_InvalidatesTicket()
	{
		var gameworld = new Mock<IFuturemud>();
		var stables = new All<IStable>();
		var stable = new Mock<IStable>();
		var stay = new Mock<IStableStay>();
		var originalParent = CreateItem(101L, gameworld.Object);
		var copiedParent = CreateItem(202L, gameworld.Object);
		var proto = (StableTicketGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreateStableTicketDbProto(), gameworld.Object);

		stable.SetupGet(x => x.Id).Returns(1L);
		stable.SetupGet(x => x.Name).Returns("Stable");
		stay.SetupGet(x => x.Id).Returns(42L);
		stay.SetupGet(x => x.TicketToken).Returns("stable-token");
		stay.Setup(x => x.TicketMatches(originalParent.Object, "stable-token")).Returns(true);
		stay.Setup(x => x.TicketMatches(copiedParent.Object, "stable-token")).Returns(false);
		stable.SetupGet(x => x.Stays).Returns(new[] { stay.Object });
		stables.Add(stable.Object);
		gameworld.SetupGet(x => x.Stables).Returns(stables);

		var ticket = new StableTicketGameItemComponent(proto, originalParent.Object, true);
		ticket.InitialiseTicket(stay.Object);

		Assert.IsTrue(ticket.IsValid);

		var copiedTicket = (StableTicketGameItemComponent)ticket.Copy(copiedParent.Object, true);

		Assert.AreEqual(ticket.StableStayId, copiedTicket.StableStayId);
		Assert.AreEqual(ticket.TicketToken, copiedTicket.TicketToken);
		Assert.IsFalse(copiedTicket.IsValid);
		stay.Verify(x => x.RegisterTicket(101L, "stable-token"), Times.Once);
	}

	private static Mock<IGameItem> CreateItem(long id, IFuturemud gameworld)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		return item;
	}

	private static DbGameItemComponentProto CreateStableTicketDbProto()
	{
		return new DbGameItemComponentProto
		{
			Id = 1L,
			Name = "Stable Ticket",
			Description = "Marks an item as a system-generated stable ticket",
			Type = "Stable Ticket",
			Definition = "<Definition />",
			RevisionNumber = 0,
			EditableItem = new DbEditableItem
			{
				Id = 1L,
				BuilderAccountId = 1L,
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current
			}
		};
	}
}
