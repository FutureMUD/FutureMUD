#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcAiRegressionTests
{
    private static T CreatePrivateParameterless<T>() where T : class
    {
        ConstructorInfo? ctor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        Assert.IsNotNull(ctor, $"Could not find a private parameterless constructor for {typeof(T).Name}.");
        return (T)ctor.Invoke(null);
    }

    [TestMethod]
    public void ArenaParticipantAI_GetOpponents_ReturnsOnlyOpposingSideParticipants()
    {
        Mock<ICharacter> actor = new();
        actor.SetupGet(x => x.Id).Returns(1L);
        Mock<ICharacter> ally = new();
        ally.SetupGet(x => x.Id).Returns(2L);
        Mock<ICharacter> enemy = new();
        enemy.SetupGet(x => x.Id).Returns(3L);

        Mock<IArenaEvent> eventMock = new();
        eventMock.SetupGet(x => x.Participants).Returns(new[]
        {
            MockParticipant(actor.Object, 0),
            MockParticipant(ally.Object, 0),
            MockParticipant(enemy.Object, 1)
        });

        List<IArenaParticipant> opponents = ArenaParticipantAI.GetOpponents(eventMock.Object, actor.Object).ToList();

        Assert.AreEqual(1, opponents.Count);
        Assert.AreSame(enemy.Object, opponents[0].Character);
    }

    [TestMethod]
    public void ArborealWandererAI_CellSupportsTreeLayers_DetectsTreeCapableTerrain()
    {
        Mock<ICharacter> character = new();
        Mock<ITerrain> treeTerrain = new();
        treeTerrain.SetupGet(x => x.TerrainLayers).Returns(new[] { RoomLayer.GroundLevel, RoomLayer.InTrees });
        Mock<ITerrain> groundTerrain = new();
        groundTerrain.SetupGet(x => x.TerrainLayers).Returns(new[] { RoomLayer.GroundLevel });

        Mock<ICell> treeCell = new();
        treeCell.Setup(x => x.Terrain(character.Object)).Returns(treeTerrain.Object);
        Mock<ICell> groundCell = new();
        groundCell.Setup(x => x.Terrain(character.Object)).Returns(groundTerrain.Object);

        Assert.IsTrue(ArborealWandererAI.CellSupportsTreeLayers(character.Object, treeCell.Object));
        Assert.IsFalse(ArborealWandererAI.CellSupportsTreeLayers(character.Object, groundCell.Object));
    }

    [TestMethod]
    public void LairScavengerAI_GetScavengedItems_ReturnsOnlyProgApprovedHeldItems()
    {
        Mock<IGameItem> item1 = new();
        Mock<IGameItem> item2 = new();
        Mock<IBody> body = new();
        body.SetupGet(x => x.HeldOrWieldedItems).Returns(new[] { item1.Object, item2.Object });
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Body).Returns(body.Object);

        Mock<IFutureProg> prog = new();
        prog.Setup(x => x.ExecuteBool(false,
                It.Is<object[]>(vars => vars.Length == 2 &&
                                        ReferenceEquals(vars[0], character.Object) &&
                                        ReferenceEquals(vars[1], item1.Object))))
            .Returns(true);
        prog.Setup(x => x.ExecuteBool(false,
                It.Is<object[]>(vars => vars.Length == 2 &&
                                        ReferenceEquals(vars[0], character.Object) &&
                                        ReferenceEquals(vars[1], item2.Object))))
            .Returns(false);

        List<IGameItem> items = LairScavengerAI.GetScavengedItems(character.Object, prog.Object).ToList();

        CollectionAssert.AreEqual(new[] { item1.Object }, items);
    }

    [TestMethod]
    public void DenBuilderAI_SelectAnchorItem_SkipsActiveCraftItemsAndUsesProgApproval()
    {
        RoomLayer roomLayer = RoomLayer.GroundLevel;
        Mock<IGameItem> activeCraftItem = new();
        IActiveCraftGameItemComponent activeCraftComponent = new Mock<IActiveCraftGameItemComponent>().Object;
        activeCraftItem.Setup(x => x.GetItemType<IActiveCraftGameItemComponent>())
            .Returns(() => activeCraftComponent);
        Mock<IGameItem> anchorItem = new();
        anchorItem.Setup(x => x.GetItemType<IActiveCraftGameItemComponent>()).Returns(() => null!);

        Mock<ICell> location = new();
        location.Setup(x => x.LayerGameItems(roomLayer)).Returns(new[] { activeCraftItem.Object, anchorItem.Object });
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Location).Returns(location.Object);
        character.SetupGet(x => x.RoomLayer).Returns(roomLayer);

        Mock<IFutureProg> prog = new();
        prog.Setup(x => x.ExecuteBool(
                It.Is<object[]>(vars => vars.Length == 2 &&
                                        ReferenceEquals(vars[0], character.Object) &&
                                        ReferenceEquals(vars[1], anchorItem.Object))))
            .Returns(true);

        IGameItem? selected = DenBuilderAI.SelectAnchorItem(character.Object, prog.Object);

        Assert.AreSame(anchorItem.Object, selected);
    }

    [TestMethod]
    public void TerritorialWanderer_TryParseWanderChance_AcceptsPercentagesWithinRange()
    {
        Assert.IsTrue(TerritorialWanderer.TryParseWanderChance("25%", out double validValue));
        Assert.AreEqual(0.25, validValue, 0.0001);
        Assert.IsFalse(TerritorialWanderer.TryParseWanderChance("250%", out _));
    }

    [TestMethod]
    public void ScavengeAI_ValidationHelpers_DistinguishExpectedProgShapes()
    {
        Mock<IFutureProg> willProg = new();
        willProg.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        willProg.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);

        Mock<IFutureProg> onProg = new();
        onProg.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Void);
        onProg.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);

        Assert.IsTrue(ScavengeAI.IsValidWillScavengeProg(willProg.Object));
        Assert.IsFalse(ScavengeAI.IsValidOnScavengeProg(willProg.Object));
        Assert.IsTrue(ScavengeAI.IsValidOnScavengeProg(onProg.Object));
        Assert.IsFalse(ScavengeAI.IsValidWillScavengeProg(onProg.Object));
    }

    [TestMethod]
    public void AggressiveAiTypes_HandleCharacterEnterCellWitnessEvent()
    {
        AggressivePatherAI aggressivePather = CreatePrivateParameterless<AggressivePatherAI>();
        TrackingAggressorAI trackingAggressor = CreatePrivateParameterless<TrackingAggressorAI>();

        Assert.IsTrue(aggressivePather.HandlesEvent(EventType.CharacterEnterCellWitness));
        Assert.IsTrue(trackingAggressor.HandlesEvent(EventType.CharacterEnterCellWitness));
    }

    private static IArenaParticipant MockParticipant(ICharacter character, int sideIndex)
    {
        Mock<IArenaParticipant> participant = new();
        participant.SetupGet(x => x.Character).Returns(character);
        participant.SetupGet(x => x.CharacterId).Returns(character.Id);
        participant.SetupGet(x => x.SideIndex).Returns(sideIndex);
        return participant.Object;
    }
}
