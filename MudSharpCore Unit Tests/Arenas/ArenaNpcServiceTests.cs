#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.FutureProg;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaNpcServiceTests
{
    [TestMethod]
    public void ReturnNpc_RestoreGet_DoesNotUseIgnoreFreeHands()
    {
        Mock<IFuturemud> gameworld = new();
        ArenaNpcService service = new(gameworld.Object);
        Mock<ICell> location = new();
        Mock<IBody> body = new();
        Mock<ICharacter> npc = new();
        npc.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        npc.SetupGet(x => x.Location).Returns(location.Object);
        npc.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
        npc.SetupGet(x => x.State).Returns(CharacterState.Awake);
        npc.SetupGet(x => x.Body).Returns(body.Object);

        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.SetupGet(x => x.Id).Returns(99L);

        IGameItem? containedIn = null;
        ICell? itemLocation = null;
        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Deleted).Returns(false);
        item.SetupGet(x => x.ContainedIn).Returns(() => containedIn!);
        item.SetupGet(x => x.Location).Returns(() => itemLocation!);
        item.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);

        IBody? inventoryOwner = null;
        item.SetupGet(x => x.InInventoryOf).Returns(() => inventoryOwner!);

        ArenaNpcPreparationEffect effect = new(npc.Object, 99L, false);
        effect.CaptureItem(item.Object, InventoryState.Held, null, null);
        npc.Setup(x => x.CombinedEffectsOfType<ArenaNpcPreparationEffect>())
            .Returns([effect]);

        ItemCanGetIgnore? capturedFlags = null;
        body.Setup(x => x.Get(item.Object, It.IsAny<int>(), It.IsAny<IEmote>(), It.IsAny<bool>(),
                It.IsAny<ItemCanGetIgnore>()))
            .Callback<IGameItem, int, IEmote?, bool, ItemCanGetIgnore>((_, _, _, _, flags) =>
            {
                capturedFlags = flags;
                inventoryOwner = body.Object;
            });

        service.ReturnNpc(npc.Object, arenaEvent.Object, resurrect: false, fullRestoreBeforeInventory: false);

        Assert.IsTrue(capturedFlags.HasValue);
        Assert.AreEqual(ItemCanGetIgnore.IgnoreWeight, capturedFlags.Value);
        Assert.IsFalse(capturedFlags.Value.HasFlag(ItemCanGetIgnore.IgnoreFreeHands));
    }

    [TestMethod]
    public void ReturnNpc_NoHandsRestorePath_PlacesItemNearNpc()
    {
        Mock<IFuturemud> gameworld = new();
        ArenaNpcService service = new(gameworld.Object);
        Mock<ICell> location = new();
        Mock<IBody> body = new();
        Mock<ICharacter> npc = new();
        npc.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        npc.SetupGet(x => x.Location).Returns(location.Object);
        npc.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
        npc.SetupGet(x => x.State).Returns(CharacterState.Awake);
        npc.SetupGet(x => x.Body).Returns(body.Object);

        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.SetupGet(x => x.Id).Returns(100L);

        IGameItem? containedIn = null;
        ICell? itemLocation = null;
        IBody? inventoryOwner = null;
        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Deleted).Returns(false);
        item.SetupGet(x => x.ContainedIn).Returns(() => containedIn!);
        item.SetupGet(x => x.Location).Returns(() => itemLocation!);
        item.SetupGet(x => x.InInventoryOf).Returns(() => inventoryOwner!);
        item.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);

        ArenaNpcPreparationEffect effect = new(npc.Object, 100L, false);
        effect.CaptureItem(item.Object, InventoryState.Held, null, null);
        npc.Setup(x => x.CombinedEffectsOfType<ArenaNpcPreparationEffect>())
            .Returns([effect]);

        body.Setup(x => x.Get(item.Object, It.IsAny<int>(), It.IsAny<IEmote>(), It.IsAny<bool>(),
                It.IsAny<ItemCanGetIgnore>()))
            .Throws(new InvalidOperationException("No usable hands."));

        service.ReturnNpc(npc.Object, arenaEvent.Object, resurrect: false, fullRestoreBeforeInventory: false);

        location.Verify(x => x.Insert(item.Object, true), Times.Once);
        item.Verify(x => x.Delete(), Times.Never);
    }

    [TestMethod]
	public void ReturnNpc_FullRestoreBeforeInventory_RestoresBodyBeforeGet()
	{
        Mock<IFuturemud> gameworld = new();
        ArenaNpcService service = new(gameworld.Object);
        Mock<ICell> location = new();
        Mock<IBody> body = new();
        Mock<ICharacter> npc = new();
        npc.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        npc.SetupGet(x => x.Location).Returns(location.Object);
        npc.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
        npc.SetupGet(x => x.State).Returns(CharacterState.Awake);
        npc.SetupGet(x => x.Body).Returns(body.Object);

        Mock<IArenaEvent> arenaEvent = new();
        arenaEvent.SetupGet(x => x.Id).Returns(101L);

        IGameItem? containedIn = null;
        ICell? itemLocation = null;
        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Deleted).Returns(false);
        item.SetupGet(x => x.ContainedIn).Returns(() => containedIn!);
        item.SetupGet(x => x.Location).Returns(() => itemLocation!);
        item.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);

        IBody? inventoryOwner = null;
        item.SetupGet(x => x.InInventoryOf).Returns(() => inventoryOwner!);

        ArenaNpcPreparationEffect effect = new(npc.Object, 101L, false);
        effect.CaptureItem(item.Object, InventoryState.Held, null, null);
        npc.Setup(x => x.CombinedEffectsOfType<ArenaNpcPreparationEffect>())
            .Returns([effect]);

        List<string> callOrder = new();
        body.SetupSet(x => x.HeldBreathTime = It.IsAny<TimeSpan>())
            .Callback(() => callOrder.Add("HeldBreath"));
        body.Setup(x => x.RestoreAllBodypartsOrgansAndBones())
            .Callback(() => callOrder.Add("Restore"));
        body.Setup(x => x.Sober())
            .Callback(() => callOrder.Add("Sober"));
        body.Setup(x => x.CureAllWounds())
            .Callback(() => callOrder.Add("Cure"));
        body.SetupGet(x => x.MaximumStamina).Returns(100.0);
        body.SetupSet(x => x.CurrentStamina = It.IsAny<double>())
            .Callback(() => callOrder.Add("Stamina"));
        body.SetupGet(x => x.TotalBloodVolumeLitres).Returns(5.0);
        body.SetupSet(x => x.CurrentBloodVolumeLitres = It.IsAny<double>())
            .Callback(() => callOrder.Add("Blood"));
        body.Setup(x => x.EndHealthTick())
            .Callback(() => callOrder.Add("EndTick"));
        body.Setup(x => x.Get(item.Object, It.IsAny<int>(), It.IsAny<IEmote>(), It.IsAny<bool>(),
                It.IsAny<ItemCanGetIgnore>()))
            .Callback(() =>
            {
                callOrder.Add("Get");
                inventoryOwner = body.Object;
            });

        service.ReturnNpc(npc.Object, arenaEvent.Object, resurrect: false, fullRestoreBeforeInventory: true);

        Assert.IsTrue(callOrder.Contains("Restore"));
        Assert.IsTrue(callOrder.Contains("Get"));
		Assert.IsTrue(callOrder.IndexOf("Restore") < callOrder.IndexOf("Get"));
		body.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()), Times.Once);
	}

	[TestMethod]
	public void AutoFill_StableNpcCandidates_MustMeetSideRatingRequirement()
	{
		Mock<IArenaRatingsService> ratingsService = new();
		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.ArenaRatingsService).Returns(ratingsService.Object);
		ArenaNpcService service = new(gameworld.Object);

		Mock<IFutureProg> eligibilityProg = new();
		eligibilityProg.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);

		Mock<ICombatantClass> combatantClass = new();
		combatantClass.SetupGet(x => x.EligibilityProg).Returns(eligibilityProg.Object);

		Mock<IArenaEventTypeSide> side = new();
		side.SetupGet(x => x.Index).Returns(1);
		side.SetupGet(x => x.MinimumRating).Returns(1800.0m);
		side.SetupGet(x => x.MaximumRating).Returns(() => null);
		side.SetupGet(x => x.EligibleClasses).Returns([combatantClass.Object]);
		side.SetupGet(x => x.NpcLoaderProg).Returns(() => null);

		Mock<INPC> lowRatedNpc = new();
		lowRatedNpc.SetupGet(x => x.IsPlayerCharacter).Returns(false);
		lowRatedNpc.SetupGet(x => x.State).Returns(CharacterState.Awake);
		lowRatedNpc.SetupGet(x => x.Id).Returns(1001L);
		lowRatedNpc.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		Mock<INPC> championNpc = new();
		championNpc.SetupGet(x => x.IsPlayerCharacter).Returns(false);
		championNpc.SetupGet(x => x.State).Returns(CharacterState.Awake);
		championNpc.SetupGet(x => x.Id).Returns(1002L);
		championNpc.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		ratingsService.Setup(x => x.GetRating(lowRatedNpc.Object, combatantClass.Object)).Returns(1700.0m);
		ratingsService.Setup(x => x.GetRating(championNpc.Object, combatantClass.Object)).Returns(1900.0m);

		Mock<ICell> stableCell = new();
		stableCell.SetupGet(x => x.Characters).Returns([lowRatedNpc.Object, championNpc.Object]);

		Mock<IArenaEventType> eventType = new();
		eventType.SetupGet(x => x.Sides).Returns([side.Object]);

		Mock<ICombatArena> arena = new();
		arena.SetupGet(x => x.NpcStablesCells).Returns([stableCell.Object]);
		arena.SetupGet(x => x.ActiveEvents).Returns(Enumerable.Empty<IArenaEvent>());

		Mock<IArenaEvent> arenaEvent = new();
		arenaEvent.SetupGet(x => x.EventType).Returns(eventType.Object);
		arenaEvent.SetupGet(x => x.Arena).Returns(arena.Object);

		List<ICharacter> selected = service.AutoFill(arenaEvent.Object, 1, 1).ToList();

		Assert.AreEqual(1, selected.Count);
		Assert.AreSame(championNpc.Object, selected[0]);
		ratingsService.Verify(x => x.GetRating(lowRatedNpc.Object, combatantClass.Object), Times.Once);
		ratingsService.Verify(x => x.GetRating(championNpc.Object, combatantClass.Object), Times.Once);
	}
}
