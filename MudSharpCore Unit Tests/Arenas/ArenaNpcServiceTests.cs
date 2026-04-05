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
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;

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
}
