#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
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
        Assert.IsTrue(ScavengeAI.IsValidOnScavengeProg(onProg.Object));
        Assert.IsFalse(ScavengeAI.IsValidWillScavengeProg(onProg.Object));
    }

    [TestMethod]
    public void AggressiveAiTypes_HandleCharacterEnterCellWitnessEvent()
    {
        AggressivePatherAI aggressivePather = CreatePrivateParameterless<AggressivePatherAI>();
        TrackingAggressorAI trackingAggressor = CreatePrivateParameterless<TrackingAggressorAI>();
        TerritorialPredatorAI territorialPredator = CreatePrivateParameterless<TerritorialPredatorAI>();
        DenningPredatorAI denningPredator = CreatePrivateParameterless<DenningPredatorAI>();
        TerritorialForagerAI territorialForager = CreatePrivateParameterless<TerritorialForagerAI>();
        DenningForagerAI denningForager = CreatePrivateParameterless<DenningForagerAI>();

        Assert.IsTrue(aggressivePather.HandlesEvent(EventType.CharacterEnterCellWitness));
        Assert.IsTrue(trackingAggressor.HandlesEvent(EventType.CharacterEnterCellWitness));
        Assert.IsTrue(territorialPredator.HandlesEvent(EventType.CharacterEnterCellWitness));
        Assert.IsTrue(denningPredator.HandlesEvent(EventType.CharacterEnterCellWitness));
        Assert.IsTrue(denningPredator.HandlesEvent(EventType.CharacterDiesWitness));
        Assert.IsTrue(territorialForager.HandlesEvent(EventType.TenSecondTick));
        Assert.IsTrue(denningForager.HandlesEvent(EventType.TenSecondTick));
    }

    [TestMethod]
    public void PredatorAiTypes_HandleFoodResumeEvents()
    {
        HungryAggressorAI hungryAggressor = CreatePrivateParameterless<HungryAggressorAI>();
        TerritorialPredatorAI territorialPredator = CreatePrivateParameterless<TerritorialPredatorAI>();
        DenningPredatorAI denningPredator = CreatePrivateParameterless<DenningPredatorAI>();
        TerritorialForagerAI territorialForager = CreatePrivateParameterless<TerritorialForagerAI>();
        DenningForagerAI denningForager = CreatePrivateParameterless<DenningForagerAI>();

        Assert.IsTrue(hungryAggressor.HandlesEvent(EventType.CharacterEnterCellFinish));
        Assert.IsTrue(hungryAggressor.HandlesEvent(EventType.LeaveCombat));
        Assert.IsTrue(territorialPredator.HandlesEvent(EventType.CharacterEnterCellFinish));
        Assert.IsTrue(territorialPredator.HandlesEvent(EventType.LeaveCombat));
        Assert.IsTrue(denningPredator.HandlesEvent(EventType.LeaveCombat));
        Assert.IsTrue(territorialForager.HandlesEvent(EventType.CharacterEnterCellFinish));
        Assert.IsTrue(territorialForager.HandlesEvent(EventType.LeaveCombat));
        Assert.IsTrue(denningForager.HandlesEvent(EventType.CharacterEnterCellFinish));
        Assert.IsTrue(denningForager.HandlesEvent(EventType.LeaveCombat));
    }

    [TestMethod]
    public void PredatorAIHelpers_IsHungry_UsesNeedModelHungerStatus()
    {
        Mock<INeedsModel> hungryNeeds = new();
        hungryNeeds.SetupGet(x => x.Status).Returns(NeedsResult.Hungry);
        Mock<ICharacter> hungryCharacter = new();
        hungryCharacter.SetupGet(x => x.NeedsModel).Returns(hungryNeeds.Object);

        Mock<INeedsModel> fullNeeds = new();
        fullNeeds.SetupGet(x => x.Status).Returns(NeedsResult.Full);
        Mock<ICharacter> fullCharacter = new();
        fullCharacter.SetupGet(x => x.NeedsModel).Returns(fullNeeds.Object);

        Assert.IsTrue(PredatorAIHelpers.IsHungry(hungryCharacter.Object));
        Assert.IsFalse(PredatorAIHelpers.IsHungry(fullCharacter.Object));
    }

    [TestMethod]
    public void PredatorAIHelpers_CouldEatAfterKilling_RequiresEdibleCorpseMaterial()
    {
        Mock<ISolid> corpseMaterial = new();
        Mock<ICorpseModel> corpseModel = new();
        corpseModel.SetupGet(x => x.CreateCorpse).Returns(true);
        corpseModel.Setup(x => x.CorpseMaterial(0.0)).Returns(corpseMaterial.Object);

        Mock<IRace> predatorRace = new();
        predatorRace.SetupGet(x => x.CanEatCorpses).Returns(true);
        predatorRace.Setup(x => x.CanEatCorpseMaterial(corpseMaterial.Object)).Returns(true);
        Mock<IRace> targetRace = new();
        targetRace.SetupGet(x => x.CorpseModel).Returns(corpseModel.Object);

        Mock<ICharacter> predator = new();
        predator.SetupGet(x => x.Race).Returns(predatorRace.Object);
        Mock<ICharacter> target = new();
        target.SetupGet(x => x.Race).Returns(targetRace.Object);

        Assert.IsTrue(PredatorAIHelpers.CouldEatAfterKilling(predator.Object, target.Object));

        predatorRace.Setup(x => x.CanEatCorpseMaterial(corpseMaterial.Object)).Returns(false);

        Assert.IsFalse(PredatorAIHelpers.CouldEatAfterKilling(predator.Object, target.Object));
    }

    [TestMethod]
    public void NpcBurrowFoodEffect_SaveToXml_PersistsPendingVictimAndFoodItem()
    {
        Mock<IFuturemud> gameworld = new();
        Mock<ICharacter> owner = new();
        owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        Mock<ICharacter> victim = new();
        victim.SetupGet(x => x.Id).Returns(12L);
        Mock<IGameItem> food = new();
        food.SetupGet(x => x.Id).Returns(34L);

        NpcBurrowFoodEffect effect = new(owner.Object);
        effect.SetPendingVictim(victim.Object);
        effect.SetFoodItem(food.Object);

        string xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan>()).ToString();

        StringAssert.Contains(xml, "PendingVictimId=\"12\"");
        StringAssert.Contains(xml, "FoodItemId=\"34\"");
    }

    [TestMethod]
    public void SelfCareAI_DetermineRequiredSelfCare_PrefersBindingBeforeSuturing()
    {
        Mock<IWound> bleedingWound = CreateWound(BleedStatus.Bleeding);
        Mock<IWound> traumaControlledWound = CreateWound(BleedStatus.TraumaControlled, Difficulty.Easy);

        SelfCareAI.RequiredSelfCare result = SelfCareAI.DetermineRequiredSelfCare(
            new[] { bleedingWound.Object, traumaControlledWound.Object },
            true);

        Assert.AreEqual(SelfCareAI.RequiredSelfCare.Bind, result);
    }

    [TestMethod]
    public void SelfCareAI_DetermineRequiredSelfCare_RequiresSuturingCapability()
    {
        Mock<IWound> traumaControlledWound = CreateWound(BleedStatus.TraumaControlled, Difficulty.Easy);

        SelfCareAI.RequiredSelfCare withoutTools = SelfCareAI.DetermineRequiredSelfCare(
            new[] { traumaControlledWound.Object },
            false);
        SelfCareAI.RequiredSelfCare withTools = SelfCareAI.DetermineRequiredSelfCare(
            new[] { traumaControlledWound.Object },
            true);

        Assert.AreEqual(SelfCareAI.RequiredSelfCare.None, withoutTools);
        Assert.AreEqual(SelfCareAI.RequiredSelfCare.Suture, withTools);
    }

    [TestMethod]
    public void SelfCareAI_CellHasHostileNpcs_IgnoresPausedAggressiveNpcs()
    {
        Mock<ICharacter> self = new();
        Mock<INPC> pausedAggressiveNpc = CreateAggressiveNpc(paused: true);
        Mock<ICell> cell = new();
        cell.SetupGet(x => x.Characters).Returns(new ICharacter[] { self.Object, pausedAggressiveNpc.Object });

        bool hasHostiles = SelfCareAI.CellHasHostileNpcs(cell.Object, self.Object);

        Assert.IsFalse(hasHostiles);
    }

    [TestMethod]
    public void SelfCareAI_CellHasHostileNpcs_DetectsActiveAggressiveNpcs()
    {
        Mock<ICharacter> self = new();
        Mock<INPC> aggressiveNpc = CreateAggressiveNpc(paused: false);
        Mock<ICell> cell = new();
        cell.SetupGet(x => x.Characters).Returns(new ICharacter[] { self.Object, aggressiveNpc.Object });

        bool hasHostiles = SelfCareAI.CellHasHostileNpcs(cell.Object, self.Object);

        Assert.IsTrue(hasHostiles);
    }

    [TestMethod]
    public void SelfCareAI_GetSafeExitForSelfCare_SelectsSafeReachableDestination()
    {
        Mock<ICharacter> self = new();
        Mock<ICell> currentCell = new();
        Mock<ICell> hostileDestination = new();
        Mock<ICell> safeDestination = new();
        Mock<ICellExit> hostileExit = new();
        Mock<ICellExit> safeExit = new();

        Mock<INPC> hostileNpc = CreateAggressiveNpc(paused: false);

        hostileDestination.SetupGet(x => x.Characters).Returns(new ICharacter[] { hostileNpc.Object });
        safeDestination.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());

        hostileExit.SetupGet(x => x.Destination).Returns(hostileDestination.Object);
        safeExit.SetupGet(x => x.Destination).Returns(safeDestination.Object);

        currentCell.Setup(x => x.ExitsFor(self.Object, true)).Returns(new[] { hostileExit.Object, safeExit.Object });
        self.SetupGet(x => x.Location).Returns(currentCell.Object);
        self.Setup(x => x.CanMove(hostileExit.Object, It.IsAny<CanMoveFlags>())).Returns(new CanMoveResponse
        {
            Result = false,
            ErrorMessage = "blocked",
            WouldBeAbleToCross = null,
            HighestMovingPositionState = null,
            FastestMoveSpeed = null
        });
        self.Setup(x => x.CanMove(safeExit.Object, It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);

        ICellExit? chosenExit = SelfCareAI.GetSafeExitForSelfCare(self.Object);

        Assert.AreSame(safeExit.Object, chosenExit);
    }

    private static IArenaParticipant MockParticipant(ICharacter character, int sideIndex)
    {
        Mock<IArenaParticipant> participant = new();
        participant.SetupGet(x => x.Character).Returns(character);
        participant.SetupGet(x => x.CharacterId).Returns(character.Id);
        participant.SetupGet(x => x.SideIndex).Returns(sideIndex);
        return participant.Object;
    }

    private static Mock<IWound> CreateWound(BleedStatus bleedStatus, Difficulty closeDifficulty = Difficulty.Impossible)
    {
        Mock<IWound> wound = new();
        wound.SetupGet(x => x.BleedStatus).Returns(bleedStatus);
        wound.Setup(x => x.CanBeTreated(TreatmentType.Close)).Returns(closeDifficulty);
        return wound;
    }

    private static Mock<INPC> CreateAggressiveNpc(bool paused)
    {
        Mock<IArtificialIntelligence> aggressiveAi = new();
        aggressiveAi.SetupGet(x => x.CountsAsAggressive).Returns(true);

        Mock<INPC> npc = new();
        npc.SetupGet(x => x.State).Returns(CharacterState.Able);
        npc.SetupGet(x => x.AIs).Returns(new[] { aggressiveAi.Object });
        npc.Setup(x => x.AffectedBy<IPauseAIEffect>()).Returns(paused);
        return npc;
    }
}
