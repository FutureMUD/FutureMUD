#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class ItemSkinBulkRenameSecurityTests
{
	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ISaveManager> Save { get; } = new();
		public RevisableAll<IGameItemSkin> Skins { get; } = new();
		public List<string> Messages { get; } = new();
		private readonly Mock<IFuturemud> _world = new();

		public Fixture(bool administrator = false)
		{
			var progs = new All<IFutureProg>();
			progs.Add(Mock.Of<IFutureProg>(x => x.Id == 1));
			_world.SetupGet(x => x.FutureProgs).Returns(progs);
			_world.SetupGet(x => x.ItemSkins).Returns(Skins);
			_world.SetupGet(x => x.SaveManager).Returns(Save.Object);
			Actor.SetupGet(x => x.Gameworld).Returns(_world.Object);
			Actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(administrator);
			Actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.Id == 42 &&
				x.LineFormatLength == 120 && x.Culture == CultureInfo.InvariantCulture));
			var output = new Mock<IOutputHandler>();
			output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<string, bool, bool>((text, _, _) => Messages.Add(text)).Returns(true);
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		}

		public GameItemSkin Add(long id, string name, long owner, RevisionStatus status = RevisionStatus.UnderDesign)
		{
			var skin = new GameItemSkin(new MudSharp.Models.GameItemSkin
			{
				Id = id, Name = name, IsPublic = true, CanUseSkinProgId = 1, RevisionNumber = 1,
				EditableItem = new MudSharp.Models.EditableItem
				{
					BuilderAccountId = owner, RevisionNumber = 1, RevisionStatus = (int)status
				}
			}, _world.Object);
			Skins.Add(skin);
			return skin;
		}

		public void Rename(string input) => EditableItemNameBulkRenameCommand.Execute(Actor.Object,
			new StringStack(input), EditableRevisableItemHelper.ItemSkinHelper);
	}

	[TestMethod]
	public void Rename_PublicSkinOfAnotherBuilder_RejectsTheWholeBatch()
	{
		var f = new Fixture();
		var own = f.Add(1, "Own Skin", 42);
		var victim = f.Add(2, "Victim Skin", 99);
		f.Rename("\"^(Own|Victim) Skin$\" \"Renamed $0\"");
		Assert.AreEqual("Own Skin", own.Name);
		Assert.AreEqual("Victim Skin", victim.Name);
		Assert.IsFalse(own.Changed || victim.Changed);
		Assert.IsTrue(f.Messages.Exists(x => x.Contains("not permitted", StringComparison.Ordinal)));
		Assert.AreEqual(0, f.Save.Invocations.Count);
	}

	[TestMethod]
	public void Rename_OwnApprovedSkin_RequiresADesignRevision()
	{
		var f = new Fixture();
		var skin = f.Add(1, "Own Skin", 42, RevisionStatus.Current);
		f.Rename("\"^Own Skin$\" \"New Skin\"");
		Assert.AreEqual("Own Skin", skin.Name);
		Assert.IsFalse(skin.Changed);
		Assert.IsTrue(f.Messages.Exists(x => x.Contains("under-design revision", StringComparison.Ordinal)));
	}

	[DataTestMethod]
	[DataRow(false, RevisionStatus.UnderDesign)]
	[DataRow(true, RevisionStatus.Current)]
	public void Rename_AuthorisedDraftOrAdministratorMaintenance_PreservesSupportedBehaviour(bool admin, RevisionStatus status)
	{
		var f = new Fixture(admin);
		var skin = f.Add(1, "Own Skin", admin ? 99 : 42, status);
		f.Rename("\"^Own Skin$\" \"New Skin\"");
		Assert.AreEqual("New Skin", skin.Name);
		Assert.IsTrue(skin.Changed);
	}

	[TestMethod]
	public void Rename_UntouchedPublicSkin_StillReservesItsName()
	{
		var f = new Fixture();
		var own = f.Add(1, "Own Skin", 42);
		f.Add(2, "Reserved Skin", 99);
		f.Rename("\"^Own Skin$\" \"Reserved Skin\"");
		Assert.AreEqual("Own Skin", own.Name);
		Assert.IsFalse(own.Changed);
		Assert.IsTrue(f.Messages.Exists(x => x.Contains("shared by distinct item IDs", StringComparison.Ordinal)));
	}
}
