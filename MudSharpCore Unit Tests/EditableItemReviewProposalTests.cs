using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EditableItemReviewProposalTests
{
	[TestMethod]
	public void Accept_SubmittedItemCannotSubmit_DoesNotObsoleteCurrentRevision()
	{
		var current = CreateProto(10, 0, RevisionStatus.Current, canSubmit: true, "current item", "");
		var pending = CreateProto(10, 1, RevisionStatus.PendingRevision, canSubmit: false, "pending item",
			"duplicate unique name");
		var itemProtos = new RevisableAll<IGameItemProto>();
		itemProtos.Add(current.Object);
		itemProtos.Add(pending.Object);

		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		var account = new Mock<IAccount>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ItemProtos).Returns(itemProtos);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.Account).Returns(account.Object);

		var proposal = new EditableItemReviewProposal<IGameItemProto>(actor.Object, [pending.Object]);

		proposal.Accept("approval");

		pending.Verify(x => x.CanSubmit(), Times.Once);
		pending.Verify(x => x.WhyCannotSubmit(), Times.Once);
		current.Verify(x => x.ChangeStatus(It.IsAny<RevisionStatus>(), It.IsAny<string>(), It.IsAny<IAccount>()),
			Times.Never);
		pending.Verify(x => x.ChangeStatus(It.IsAny<RevisionStatus>(), It.IsAny<string>(), It.IsAny<IAccount>()),
			Times.Never);
		output.Verify(x => x.Send(It.Is<string>(text => text.Contains("duplicate unique name")), true, false),
			Times.Once);
	}

	private static Mock<IGameItemProto> CreateProto(long id, int revision, RevisionStatus status, bool canSubmit,
		string editHeader, string whyCannotSubmit)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id);
		proto.SetupGet(x => x.RevisionNumber).Returns(revision);
		proto.SetupGet(x => x.Status).Returns(status);
		proto.SetupGet(x => x.Name).Returns(editHeader);
		proto.Setup(x => x.CanSubmit()).Returns(canSubmit);
		proto.Setup(x => x.WhyCannotSubmit()).Returns(whyCannotSubmit);
		proto.Setup(x => x.EditHeader()).Returns(editHeader);
		return proto;
	}
}
