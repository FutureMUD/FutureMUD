using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class OwnershipProgTests
{
	private static IFuturemud _gameworld = null!;

	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		_gameworld = FutureProgTestBootstrap.Gameworld;
	}

	[TestMethod]
	public void ItemOwnerDotReference_ShouldExposeCharacterOrClanType()
	{
		var compileInfo = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Item];

		Assert.IsTrue(compileInfo.PropertyTypeMap.ContainsKey("owner"));
		Assert.AreEqual(ProgVariableTypes.Character | ProgVariableTypes.Clan, compileInfo.PropertyTypeMap["owner"]);
	}

	[TestMethod]
	public void IsOwner_ShouldReturnTrueForDirectCharacterOwner()
	{
		var owner = CreateCharacterMock();
		var item = CreateItemMock();
		item.Setup(x => x.IsOwnedBy(owner.Object)).Returns(true);

		var prog = CompileBoolProg(
			"OwnershipIsOwner",
			new[]
			{
				Tuple.Create(ProgVariableTypes.Item, "item"),
				Tuple.Create(ProgVariableTypes.Character, "ch")
			},
			"return IsOwner(@item, @ch)");

		Assert.IsTrue(prog.ExecuteBool(item.Object, owner.Object));
	}

	[TestMethod]
	public void IsPropertyTrusted_ShouldReturnTrueForAlliedOwner()
	{
		var actor = CreateCharacterMock();
		var allyOwner = CreateCharacterMock();
		actor.Setup(x => x.IsAlly(allyOwner.Object)).Returns(true);

		var item = CreateItemMock();
		item.SetupGet(x => x.Owner).Returns(allyOwner.Object);
		item.Setup(x => x.IsOwnedBy(actor.Object)).Returns(false);

		var prog = CompileBoolProg(
			"OwnershipTrusted",
			new[]
			{
				Tuple.Create(ProgVariableTypes.Item, "item"),
				Tuple.Create(ProgVariableTypes.Character, "ch")
			},
			"return IsPropertyTrusted(@item, @ch)");

		Assert.IsTrue(prog.ExecuteBool(item.Object, actor.Object));
	}

	[TestMethod]
	public void IsPropertyTrustedOrClan_ShouldRespectUseClanPropertyPrivilege()
	{
		var clan = CreateClanMock();
		var membership = new Mock<IClanMembership>();
		membership.SetupGet(x => x.IsArchivedMembership).Returns(false);
		membership.SetupGet(x => x.Clan).Returns(clan.Object);
		membership.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.UseClanProperty);

		var actor = CreateCharacterMock();
		actor.SetupGet(x => x.ClanMemberships).Returns([membership.Object]);

		var item = CreateItemMock();
		item.SetupGet(x => x.Owner).Returns(clan.Object);
		item.Setup(x => x.IsOwnedBy(actor.Object)).Returns(false);

		var prog = CompileBoolProg(
			"OwnershipTrustedClan",
			new[]
			{
				Tuple.Create(ProgVariableTypes.Item, "item"),
				Tuple.Create(ProgVariableTypes.Character, "ch")
			},
			"return IsPropertyTrustedOrClan(@item, @ch)");

		Assert.IsTrue(prog.ExecuteBool(item.Object, actor.Object));
	}

	[TestMethod]
	public void SetOwnership_ShouldAssignClanOwner()
	{
		var clan = CreateClanMock();
		var item = CreateItemMock();

		var prog = CompileBoolProg(
			"OwnershipSetClan",
			new[]
			{
				Tuple.Create(ProgVariableTypes.Item, "item"),
				Tuple.Create(ProgVariableTypes.Clan, "clan")
			},
			"return SetOwnership(@item, @clan)");

		Assert.IsTrue(prog.ExecuteBool(item.Object, clan.Object));
		item.Verify(x => x.SetOwner(clan.Object), Times.Once);
	}

	[TestMethod]
	public void DeepSetOwnership_ShouldClaimContainedAndBeltedItems()
	{
		var owner = CreateCharacterMock();
		var containedItem = CreateItemMock();
		var beltedItem = CreateItemMock();
		var rootItem = CreateItemMock();

		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Contents).Returns([containedItem.Object]);

		var beltable = new Mock<IBeltable>();
		beltable.SetupGet(x => x.Parent).Returns(beltedItem.Object);

		var belt = new Mock<IBelt>();
		belt.SetupGet(x => x.ConnectedItems).Returns([beltable.Object]);

		rootItem.Setup(x => x.GetItemTypes<IContainer>()).Returns([container.Object]);
		rootItem.Setup(x => x.GetItemTypes<IBelt>()).Returns([belt.Object]);
		rootItem.Setup(x => x.GetItemTypes<ISheath>()).Returns(Array.Empty<ISheath>());

		containedItem.Setup(x => x.GetItemTypes<IContainer>()).Returns(Array.Empty<IContainer>());
		containedItem.Setup(x => x.GetItemTypes<IBelt>()).Returns(Array.Empty<IBelt>());
		containedItem.Setup(x => x.GetItemTypes<ISheath>()).Returns(Array.Empty<ISheath>());

		beltedItem.Setup(x => x.GetItemTypes<IContainer>()).Returns(Array.Empty<IContainer>());
		beltedItem.Setup(x => x.GetItemTypes<IBelt>()).Returns(Array.Empty<IBelt>());
		beltedItem.Setup(x => x.GetItemTypes<ISheath>()).Returns(Array.Empty<ISheath>());

		var prog = CompileBoolProg(
			"OwnershipDeepSetCharacter",
			new[]
			{
				Tuple.Create(ProgVariableTypes.Item, "item"),
				Tuple.Create(ProgVariableTypes.Character, "ch")
			},
			"return DeepSetOwnership(@item, @ch)");

		Assert.IsTrue(prog.ExecuteBool(rootItem.Object, owner.Object));
		rootItem.Verify(x => x.SetOwner(owner.Object), Times.Once);
		containedItem.Verify(x => x.SetOwner(owner.Object), Times.Once);
		beltedItem.Verify(x => x.SetOwner(owner.Object), Times.Once);
	}

	[TestMethod]
	public void ClearOwnership_ShouldClearRegisteredOwner()
	{
		var item = CreateItemMock();
		var prog = CompileBoolProg(
			"OwnershipClear",
			[Tuple.Create(ProgVariableTypes.Item, "item")],
			"return ClearOwnership(@item)");

		Assert.IsTrue(prog.ExecuteBool(item.Object));
		item.Verify(x => x.ClearOwner(), Times.Once);
	}

	private static FutureProg CompileBoolProg(string name, IEnumerable<Tuple<ProgVariableTypes, string>> parameters,
		string functionText)
	{
		var prog = new FutureProg(_gameworld, name, ProgVariableTypes.Boolean, parameters, functionText);
		prog.Compile();
		Assert.IsTrue(string.IsNullOrEmpty(prog.CompileError), prog.CompileError);
		return prog;
	}

	private static Mock<ICharacter> CreateCharacterMock()
	{
		var mock = new Mock<ICharacter>();
		mock.SetupGet(x => x.ClanMemberships).Returns(Array.Empty<IClanMembership>());
		mock.Setup(x => x.IsAlly(It.IsAny<ICharacter>())).Returns(false);
		mock.SetupGet(x => x.Id).Returns(1L);
		mock.SetupGet(x => x.Name).Returns("Test Character");
		mock.SetupGet(x => x.FrameworkItemType).Returns("Character");

		var progVariable = mock.As<IProgVariable>();
		progVariable.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		progVariable.SetupGet(x => x.GetObject).Returns(() => mock.Object);
		progVariable.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);

		return mock;
	}

	private static Mock<IClan> CreateClanMock()
	{
		var mock = new Mock<IClan>();
		mock.SetupGet(x => x.Id).Returns(2L);
		mock.SetupGet(x => x.Name).Returns("Test Clan");
		mock.SetupGet(x => x.FrameworkItemType).Returns("Clan");

		var progVariable = mock.As<IProgVariable>();
		progVariable.SetupGet(x => x.Type).Returns(ProgVariableTypes.Clan);
		progVariable.SetupGet(x => x.GetObject).Returns(() => mock.Object);
		progVariable.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);

		return mock;
	}

	private static Mock<IGameItem> CreateItemMock()
	{
		var mock = new Mock<IGameItem>();
		mock.SetupGet(x => x.Id).Returns(3L);
		mock.SetupGet(x => x.Name).Returns("Test Item");
		mock.SetupGet(x => x.FrameworkItemType).Returns("Item");
		mock.SetupGet(x => x.Owner).Returns((IFrameworkItem?)null);
		mock.Setup(x => x.IsOwnedBy(It.IsAny<IFrameworkItem>())).Returns(false);
		mock.Setup(x => x.GetItemTypes<IContainer>()).Returns(Array.Empty<IContainer>());
		mock.Setup(x => x.GetItemTypes<IBelt>()).Returns(Array.Empty<IBelt>());
		mock.Setup(x => x.GetItemTypes<ISheath>()).Returns(Array.Empty<ISheath>());

		var progVariable = mock.As<IProgVariable>();
		progVariable.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		progVariable.SetupGet(x => x.GetObject).Returns(() => mock.Object);
		progVariable.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);

		return mock;
	}
}
