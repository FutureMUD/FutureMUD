#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Editor;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class ItemSkinEditingSecurityTests
{
	[DataTestMethod]
	[DataRow("edit 1", RevisionStatus.UnderDesign)]
	[DataRow("edit PublicSkin", RevisionStatus.PendingRevision)]
	[DataRow("edit 1 1", RevisionStatus.UnderDesign)]
	[DataRow("edit 1", RevisionStatus.Current)]
	[DataRow("edit PublicSkin", RevisionStatus.Rejected)]
	public void Edit_PublicSkinWithoutOwnership_CannotOpenMutateOrRevise(string command, RevisionStatus status)
	{
		var f = new Fixture(status);
		f.Run(command);
		Assert.AreEqual(0, f.Effects.Count);
		f.Skin.Verify(x => x.CreateNewRevision(It.IsAny<ICharacter>()), Times.Never);
		f.Skin.Verify(x => x.ChangeStatus(It.IsAny<RevisionStatus>(), It.IsAny<string>(), It.IsAny<IAccount>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Edit_AuthorisedOwnerOrAdministrator_CanOpenAndChangeDraft(bool admin)
	{
		var f = new Fixture(RevisionStatus.UnderDesign, !admin, admin);
		f.Run("edit 1");
		Assert.AreSame(f.Skin.Object, f.Effects.OfType<BuilderEditingEffect<IGameItemSkin>>().Single().EditingItem);
		f.Run("set sdesc a new description");
		f.Skin.Verify(x => x.BuildingCommand(f.Actor.Object, It.Is<StringStack>(s => s.RemainingArgument == "sdesc a new description")), Times.Once);
	}

	[DataTestMethod]
	[DataRow("set sdesc changed", RevisionStatus.UnderDesign)]
	[DataRow("set name Changed", RevisionStatus.UnderDesign)]
	[DataRow("edit submit", RevisionStatus.UnderDesign)]
	[DataRow("edit delete", RevisionStatus.UnderDesign)]
	[DataRow("edit obsolete", RevisionStatus.Current)]
	public void Mutation_OwnershipRevokedAfterSelection_DoesNotMutate(string command, RevisionStatus status)
	{
		var f = new Fixture(status, true);
		f.Effects.Add(new BuilderEditingEffect<IGameItemSkin>(f.Actor.Object) { EditingItem = f.Skin.Object });
		f.OwnsSkin = false;
		f.Run(command);
		f.Skin.Verify(x => x.BuildingCommand(It.IsAny<ICharacter>(), It.IsAny<StringStack>()), Times.Never);
		f.Skin.Verify(x => x.ChangeStatus(It.IsAny<RevisionStatus>(), It.IsAny<string>(), It.IsAny<IAccount>()), Times.Never);
		f.World.VerifyGet(x => x.SaveManager, Times.Never);
		Assert.AreEqual(1, f.Effects.Count);
	}

	[TestMethod]
	public void Set_DraftApprovedWhileSelected_RequiresPlayerToOpenANewRevision()
	{
		var f = new Fixture(RevisionStatus.UnderDesign, true);
		f.Run("edit 1");
		f.Skin.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		f.Run("set sdesc changed");
		f.Skin.Verify(x => x.BuildingCommand(It.IsAny<ICharacter>(), It.IsAny<StringStack>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(true, false, RevisionStatus.PendingRevision, true)]
	[DataRow(true, false, RevisionStatus.Current, false)]
	[DataRow(false, false, RevisionStatus.UnderDesign, false)]
	[DataRow(false, true, RevisionStatus.Current, true)]
	public void DescriptionEditor_PermissionsAndStatusChangeBeforeSubmission_RechecksAuthority(
		bool owner, bool adminAtSave, RevisionStatus statusAtSave, bool allowed)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var progs = new All<IFutureProg>();
		progs.Add(Mock.Of<IFutureProg>(x => x.Id == 1));
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		var protos = new RevisableAll<IGameItemProto>();
		protos.Add(Mock.Of<IGameItemProto>(x => x.Id == 1 && x.Status == RevisionStatus.Current &&
		                                    x.FullDescription == "A plain object."));
		world.SetupGet(x => x.ItemProtos).Returns(protos);
		var skin = new GameItemSkin(new MudSharp.Models.GameItemSkin
		{
			Id = 1, Name = "Skin", ItemProtoId = 1, CanUseSkinProgId = 1,
			FullDescription = "Original description.",
			EditableItem = new MudSharp.Models.EditableItem
			{
				BuilderAccountId = owner ? 1 : 2, RevisionStatus = (int)RevisionStatus.PendingRevision
			}
		}, world.Object);
		var skins = new RevisableAll<IGameItemSkin>();
		skins.Add(skin);
		world.SetupGet(x => x.ItemSkins).Returns(skins);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.Id == 1));
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(80);
		actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		Action<string, IOutputHandler, object[]>? submit = null;
		object[]? arguments = null;
		actor.Setup(x => x.EditorMode(It.IsAny<Action<string, IOutputHandler, object[]>>(),
				It.IsAny<Action<IOutputHandler, object[]>>(), It.IsAny<double>(), It.IsAny<string>(),
				It.IsAny<EditorOptions>(), It.IsAny<object[]>()))
			.Callback<Action<string, IOutputHandler, object[]>, Action<IOutputHandler, object[]>, double, string, EditorOptions, object[]>(
				(post, _, _, _, _, args) => { submit = post; arguments = args; });
		Assert.IsTrue(skin.BuildingCommand(actor.Object, new StringStack("desc")));
		Assert.IsNotNull(submit);
		skin.ChangeStatus(statusAtSave, "Reviewed while the editor was open",
			owner ? actor.Object.Account : Mock.Of<IAccount>(x => x.Id == 2));
		skin.Changed = false;
		actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(adminAtSave);
		submit("Replacement description.", actor.Object.OutputHandler, arguments!);
		Assert.AreEqual(allowed ? "Replacement description." : "Original description.", skin.FullDescription);
		Assert.AreEqual(allowed, skin.Changed);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IGameItemSkin> Skin { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public bool OwnsSkin { get; set; }

		public Fixture(RevisionStatus status, bool owner = false, bool admin = false)
		{
			OwnsSkin = owner;
			Skin.SetupGet(x => x.Id).Returns(1);
			Skin.SetupGet(x => x.RevisionNumber).Returns(1);
			Skin.SetupGet(x => x.Name).Returns("PublicSkin");
			Skin.SetupGet(x => x.IsPublic).Returns(true);
			Skin.SetupGet(x => x.Status).Returns(status);
			Skin.Setup(x => x.IsAssociatedBuilder(Actor.Object)).Returns(() => OwnsSkin);
			var skins = new RevisableAll<IGameItemSkin>();
			skins.Add(Skin.Object);
			World.SetupGet(x => x.ItemSkins).Returns(skins);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(admin);
			Actor.Setup(x => x.EffectsOfType<BuilderEditingEffect<IGameItemSkin>>()).Returns(() => Effects.OfType<BuilderEditingEffect<IGameItemSkin>>());
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Actor.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()))
				.Callback<Predicate<IEffect>, bool>((predicate, _) => Effects.RemoveAll(predicate));
		}

		public void Run(string command) => BaseBuilderModule.GenericRevisableBuildingCommand(Actor.Object,
			new StringStack(command), EditableRevisableItemHelper.ItemSkinHelper);
	}
}
