using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;

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
        FutureProgVariableCompileInfo compileInfo = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Item];

        Assert.IsTrue(compileInfo.PropertyTypeMap.ContainsKey("owner"));
        Assert.AreEqual(ProgVariableTypes.Character | ProgVariableTypes.Clan, compileInfo.PropertyTypeMap["owner"]);
    }

    [TestMethod]
    public void IsOwner_ShouldReturnTrueForDirectCharacterOwner()
    {
        Mock<ICharacter> owner = CreateCharacterMock();
        Mock<IGameItem> item = CreateItemMock();
        item.Setup(x => x.IsOwnedBy(owner.Object)).Returns(true);

        FutureProg prog = CompileBoolProg(
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
        Mock<ICharacter> actor = CreateCharacterMock();
        Mock<ICharacter> allyOwner = CreateCharacterMock();
        actor.Setup(x => x.IsAlly(allyOwner.Object)).Returns(true);

        Mock<IGameItem> item = CreateItemMock();
        item.SetupGet(x => x.Owner).Returns(allyOwner.Object);
        item.Setup(x => x.IsOwnedBy(actor.Object)).Returns(false);

        FutureProg prog = CompileBoolProg(
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
        Mock<IClan> clan = CreateClanMock();
        Mock<IClanMembership> membership = new();
        membership.SetupGet(x => x.IsArchivedMembership).Returns(false);
        membership.SetupGet(x => x.Clan).Returns(clan.Object);
        membership.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.UseClanProperty);

        Mock<ICharacter> actor = CreateCharacterMock();
        actor.SetupGet(x => x.ClanMemberships).Returns([membership.Object]);

        Mock<IGameItem> item = CreateItemMock();
        item.SetupGet(x => x.Owner).Returns(clan.Object);
        item.Setup(x => x.IsOwnedBy(actor.Object)).Returns(false);

        FutureProg prog = CompileBoolProg(
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
        Mock<IClan> clan = CreateClanMock();
        Mock<IGameItem> item = CreateItemMock();

        FutureProg prog = CompileBoolProg(
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
        Mock<ICharacter> owner = CreateCharacterMock();
        Mock<IGameItem> containedItem = CreateItemMock();
        Mock<IGameItem> beltedItem = CreateItemMock();
        Mock<IGameItem> rootItem = CreateItemMock();

        Mock<IContainer> container = new();
        container.SetupGet(x => x.Contents).Returns([containedItem.Object]);

        Mock<IBeltable> beltable = new();
        beltable.SetupGet(x => x.Parent).Returns(beltedItem.Object);

        Mock<IBelt> belt = new();
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

        FutureProg prog = CompileBoolProg(
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
        Mock<IGameItem> item = CreateItemMock();
        FutureProg prog = CompileBoolProg(
            "OwnershipClear",
            [Tuple.Create(ProgVariableTypes.Item, "item")],
            "return ClearOwnership(@item)");

        Assert.IsTrue(prog.ExecuteBool(item.Object));
        item.Verify(x => x.ClearOwner(), Times.Once);
    }

    private static FutureProg CompileBoolProg(string name, IEnumerable<Tuple<ProgVariableTypes, string>> parameters,
        string functionText)
    {
        FutureProg prog = new(_gameworld, name, ProgVariableTypes.Boolean, parameters, functionText);
        prog.Compile();
        Assert.IsTrue(string.IsNullOrEmpty(prog.CompileError), prog.CompileError);
        return prog;
    }

    private static Mock<ICharacter> CreateCharacterMock()
    {
        Mock<ICharacter> mock = new();
        mock.SetupGet(x => x.ClanMemberships).Returns(Array.Empty<IClanMembership>());
        mock.Setup(x => x.IsAlly(It.IsAny<ICharacter>())).Returns(false);
        mock.SetupGet(x => x.Id).Returns(1L);
        mock.SetupGet(x => x.Name).Returns("Test Character");
        mock.SetupGet(x => x.FrameworkItemType).Returns("Character");

        Mock<IProgVariable> progVariable = mock.As<IProgVariable>();
        progVariable.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
        progVariable.SetupGet(x => x.GetObject).Returns(() => mock.Object);
        progVariable.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);

        return mock;
    }

    private static Mock<IClan> CreateClanMock()
    {
        Mock<IClan> mock = new();
        mock.SetupGet(x => x.Id).Returns(2L);
        mock.SetupGet(x => x.Name).Returns("Test Clan");
        mock.SetupGet(x => x.FrameworkItemType).Returns("Clan");

        Mock<IProgVariable> progVariable = mock.As<IProgVariable>();
        progVariable.SetupGet(x => x.Type).Returns(ProgVariableTypes.Clan);
        progVariable.SetupGet(x => x.GetObject).Returns(() => mock.Object);
        progVariable.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);

        return mock;
    }

    private static Mock<IGameItem> CreateItemMock()
    {
        Mock<IGameItem> mock = new();
        mock.SetupGet(x => x.Id).Returns(3L);
        mock.SetupGet(x => x.Name).Returns("Test Item");
        mock.SetupGet(x => x.FrameworkItemType).Returns("Item");
        mock.SetupGet(x => x.Owner).Returns((IFrameworkItem?)null);
        mock.Setup(x => x.IsOwnedBy(It.IsAny<IFrameworkItem>())).Returns(false);
        mock.Setup(x => x.GetItemTypes<IContainer>()).Returns(Array.Empty<IContainer>());
        mock.Setup(x => x.GetItemTypes<IBelt>()).Returns(Array.Empty<IBelt>());
        mock.Setup(x => x.GetItemTypes<ISheath>()).Returns(Array.Empty<ISheath>());

        Mock<IProgVariable> progVariable = mock.As<IProgVariable>();
        progVariable.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
        progVariable.SetupGet(x => x.GetObject).Returns(() => mock.Object);
        progVariable.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);

        return mock;
    }
}
