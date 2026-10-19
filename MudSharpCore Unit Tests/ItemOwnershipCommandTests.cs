#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemOwnershipCommandTests
{
	private sealed class Fixture
	{
		internal Mock<ICharacter> Actor = new();
		internal Mock<IGameItem> Item = new(), Box = new();
		internal Mock<IContainer> Container = new();
		internal Mock<IItemTakeInterdictionEffect> Mark = new();
		internal Mock<ISpellPocket> Pocket = new();
		internal Mock<IFuturemud> World = new();
		internal Mock<IRoom> Room = new();
		internal Mock<IOutputHandler> Output = new();
		internal Mock<IBody> Body;
		internal List<IEffect> Effects = [];
		internal List<IGameItem> Held = [];
		internal bool Allowed, Staff;
		internal Fixture()
		{
			Body = PhysicalManipulationTestHelper.SetUpUsableHands(Actor);
			Actor.SetupGet(x => x.Id).Returns(10);
			Actor.SetupGet(x => x.InstanceId).Returns(10);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Location).Returns(Room.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Output.SetupGet(x => x.Perceiver).Returns(Actor.Object);
			Room.SetupGet(x => x.Characters).Returns([Actor.Object]);
			Room.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns([Actor.Object]);
			Actor.SetupGet(x => x.MaximumDragWeight).Returns(1000);
			Actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
			Actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, Actor.Object));
			Actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(() => Staff);
			Actor.Setup(x => x.TargetLocal("item")).Returns(Item.Object);
			Actor.Setup(x => x.TargetLocalItem("item")).Returns(Item.Object);
			Actor.Setup(x => x.TargetLocalItem("box")).Returns(Box.Object);
			Actor.Setup(x => x.TargetHeldItem("item")).Returns(Item.Object);
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((x, _) => Effects.Add(x));
			Body.SetupGet(x => x.HeldOrWieldedItems).Returns(Held);
			Body.SetupGet(x => x.ExternalItems).Returns([]);
			Item.SetupGet(x => x.Gameworld).Returns(World.Object);
			Item.SetupGet(x => x.Weight).Returns(500);
			Item.SetupGet(x => x.Location).Returns(Room.Object);
			Item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, Item.Object));
			Item.Setup(x => x.EffectsOfType<IItemTakeInterdictionEffect>(It.IsAny<Predicate<IItemTakeInterdictionEffect>>())).Returns([Mark.Object]);
			Mark.Setup(x => x.AllowsTake(Actor.Object)).Returns(() => Allowed);
			Box.Setup(x => x.GetItemType<IContainer>()).Returns(Container.Object);
			Container.Setup(x => x.CanPut(Item.Object)).Returns(true);
			Container.Setup(x => x.CanTake(Actor.Object, Item.Object, 0)).Returns(true);
			Container.SetupGet(x => x.Contents).Returns([Item.Object]);
			Room.Setup(x => x.CanGet(It.IsAny<IGameItem>(), Actor.Object)).Returns(true);
			Room.Setup(x => x.CanGetAccess(Box.Object, Actor.Object)).Returns(true);
			World.Setup(x => x.GetStaticBool("PlayersCanJunk")).Returns(true);
			Held.Add(Item.Object);
		}
		internal void Command(string name, string text) => typeof(ManipulationModule)
			.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [Actor.Object, text]);
		internal void SetPocket(SpellPocketAccess access = SpellPocketAccess.Creator, long creator = 10)
		{
			Item.Setup(x => x.IsItemType<ISpellPocket>()).Returns(true);
			Item.Setup(x => x.GetItemType<ISpellPocket>()).Returns(Pocket.Object);
			Pocket.SetupGet(x => x.Anchor).Returns(new SpellPocketAnchor(1, 1, new(1, 10, SizeCategory.Normal, 60, access, 1)));
			Item.SetupGet(x => x.SpellCreationOrigin).Returns(new SpellOwnedItemOrigin(Guid.NewGuid(), SpellLifecycleMode.TemporaryCleanup, DateTime.UtcNow.AddMinutes(1), creator, 1, 1));
		}
	}

	[TestMethod]
	public void Drag_ProtectedItem_NonOwnerCannotStart()
	{
		var f = new Fixture(); f.Command("Drag", "drag item");
		f.Actor.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never);
		f.Mark.Verify(x => x.AllowsTake(f.Actor.Object), Times.Once);
	}

	[TestMethod]
	public void DragHelper_ProtectedItem_NonOwnerCannotJoin()
	{
		var f = new Fixture(); var leader = new Mock<ICharacter>();
		leader.SetupGet(x => x.Id).Returns(20); leader.SetupGet(x => x.InstanceId).Returns(20);
		var drag = new Dragging(leader.Object, null!, f.Item.Object);
		leader.Setup(x => x.EffectsOfType<Dragging>(It.IsAny<Predicate<Dragging>>())).Returns([drag]);
		f.Actor.Setup(x => x.TargetActor("leader", PerceiveIgnoreFlags.None)).Returns(leader.Object);
		f.Command("Drag", "drag help leader");
		Assert.AreEqual(0, drag.Helpers.Count());
		f.Mark.Verify(x => x.AllowsTake(f.Actor.Object), Times.Once);
	}

	[TestMethod]
	public void Haul_ExistingDrag_NonOwnerCannotBypassTakeInterdiction()
	{
		var f = new Fixture(); var drag = new Dragging(f.Actor.Object, null!, f.Item.Object);
		f.Item.Setup(x => x.EffectsOfType<Dragging.DragTarget>(It.IsAny<Predicate<Dragging.DragTarget>>()))
			.Returns([new Dragging.DragTarget(f.Item.Object, drag)]);
		f.Command("Haul", "haul item box");
		f.Container.Verify(x => x.Put(f.Actor.Object, f.Item.Object, It.IsAny<bool>()), Times.Never);
		f.Item.Verify(x => x.Get(null), Times.Never);
	}

	[TestMethod]
	public void Haul_AuthorizedHeavyDrag_DoesNotRequireLifting()
	{
		var f = new Fixture { Allowed = true }; var drag = new Dragging(f.Actor.Object, null!, f.Item.Object);
		f.Item.Setup(x => x.EffectsOfType<Dragging.DragTarget>(It.IsAny<Predicate<Dragging.DragTarget>>()))
			.Returns([new Dragging.DragTarget(f.Item.Object, drag)]);
		f.Body.Setup(x => x.CanGet(f.Item.Object, 0, ItemCanGetIgnore.None)).Returns(false);
		f.Command("Haul", "haul item box");
		f.Container.Verify(x => x.Put(f.Actor.Object, f.Item.Object, It.IsAny<bool>()), Times.Once);
		f.Item.Verify(x => x.Get(null), Times.Once);
	}

	[TestMethod]
	public void HaulOut_ProtectedContents_NonOwnerCannotExtract()
	{
		var f = new Fixture(); f.Command("Haul", "haul out #1 box");
		f.Container.Verify(x => x.Take(f.Item.Object), Times.Never);
		f.Mark.Verify(x => x.AllowsTake(f.Actor.Object), Times.Once);
	}

	[TestMethod]
	public void Haul_AuthorizationRevokedByOutput_DoesNotTransfer()
	{
		var f = new Fixture { Allowed = true };
		f.Body.Setup(x => x.CanGet(f.Item.Object, 0, ItemCanGetIgnore.None)).Returns(true);
		f.Output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback(() => f.Allowed = false);
		f.Command("Haul", "haul item box");
		f.Container.Verify(x => x.Put(f.Actor.Object, f.Item.Object, It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void Junk_ForeignCreatorPocket_RefusesBeforeConfirmationOrDeletion()
	{
		var f = new Fixture(); f.SetPocket(creator: 20);
		f.Pocket.SetupGet(x => x.Contents).Returns([f.Item.Object]);
			f.Item.Setup(x => x.GetItemType<IContainer>()).Returns(f.Pocket.Object);
		f.Command("Junk", "junk item");
		Assert.AreEqual(0, f.Effects.Count);
		f.Item.Verify(x => x.Delete(), Times.Never);
		Assert.IsFalse(f.Output.Invocations.Any(x => x.Method.Name == "Send" && x.Arguments[0] is IOutput));
	}

	[DataTestMethod]
	[DataRow(false)] [DataRow(true)]
	public void Junk_CreatorOrStaff_UsesExistingPocketDeletionWithoutTaking(bool staff)
	{
		var f = new Fixture { Staff = staff }; f.SetPocket(creator: staff ? 20 : 10);
		f.Command("Junk", "junk item");
		f.Item.Verify(x => x.Delete(), Times.Once);
		f.Body.Verify(x => x.Take(f.Item.Object), Times.Never);
	}

	[TestMethod]
	public void Junk_CanonicalCreatorIdentityAndHeldDeletionKeepExistingCustody()
	{
		var f = new Fixture(); f.SetPocket();
		f.Actor.SetupGet(x => x.Id).Returns(99);
		f.Actor.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 10));
		f.Command("Junk", "junk item");
		f.Item.Verify(x => x.Delete(), Times.Once);
		Assert.AreSame(f.Item.Object, f.Held.Single());
		Assert.IsTrue(f.Output.Invocations.Any(x => x.Method.Name == "Send" &&
			x.Arguments[0] is string text && text.Contains("remains in your custody")));
	}

	[TestMethod]
	public void Junk_ConfirmationRechecksStaffAndCustody()
	{
		foreach (var revoke in new[] { "staff", "custody" })
		{
			var f = new Fixture { Staff = true }; f.SetPocket(creator: 20);
			f.Pocket.SetupGet(x => x.Contents).Returns([f.Item.Object]);
			f.Item.Setup(x => x.GetItemType<IContainer>()).Returns(f.Pocket.Object);
			f.Command("Junk", "junk item");
			if (revoke == "staff") f.Staff = false; else f.Held.Clear();
			f.Effects.OfType<Accept>().Single().Proposal.Accept("");
			f.Item.Verify(x => x.Delete(), Times.Never);
			Assert.IsFalse(f.Output.Invocations.Any(x => x.Method.Name == "Send" && x.Arguments[0] is IOutput));
		}
	}

	[TestMethod]
	public void Junk_MissingCreatorRefusesButBearerAndOrdinaryItemsKeepExistingPermission()
	{
		var denied = new Fixture(); denied.SetPocket(creator: 0); denied.Command("Junk", "junk item");
		denied.Item.Verify(x => x.Delete(), Times.Never);
		var bearer = new Fixture(); bearer.SetPocket(SpellPocketAccess.Bearer, 20); bearer.Command("Junk", "junk item");
		bearer.Item.Verify(x => x.Delete(), Times.Once);
		var ordinary = new Fixture(); ordinary.Command("Junk", "junk item");
		ordinary.Body.Verify(x => x.Take(ordinary.Item.Object), Times.Once);
		ordinary.Item.Verify(x => x.Delete(), Times.Once);
	}
}
