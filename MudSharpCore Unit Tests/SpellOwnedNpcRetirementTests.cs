#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellOwnedNpcRetirementTests
{
	private static Mock<IGameItem> Item(long id, IFuturemud world)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.As<ILateInitialisingItem>().SetupGet(x => x.IdHasBeenRegistered).Returns(true);
		item.SetupGet(x => x.Gameworld).Returns(world);
		return item;
	}

	private static IBody Body(IFuturemud world, params IGameItem[] items) =>
		Mock.Of<IBody>(x => x.Gameworld == world && x.AllItems == items);

	[TestMethod]
	public void Custody_NestedContainerLockAndAttachment_PreservesOneRootAndEveryIdentity()
	{
		var world = Mock.Of<IFuturemud>();
		var bag = Item(1, world); var gun = Item(2, world); var scope = Item(3, world); var key = Item(4, world);
		var container = new Mock<IContainer>(); container.SetupGet(x => x.Contents).Returns([gun.Object]);
		var locked = new Mock<ILockable>(); locked.SetupGet(x => x.Locks).Returns([Mock.Of<ILock>(x => x.Parent == key.Object)]);
		bag.SetupGet(x => x.Components).Returns([container.Object, locked.Object]);
		var host = new Mock<IFirearmAttachmentHost>();
		host.SetupGet(x => x.InstalledAttachments).Returns(new Dictionary<string, IFirearmAttachment>()
		{
			["optics"] = Mock.Of<IFirearmAttachment>(x => x.Parent == scope.Object)
		});
		gun.Setup(x => x.GetItemType<IFirearmAttachmentHost>()).Returns(host.Object);
		Assert.IsTrue(SpellOwnedNpcService.TryCaptureForeignCustody(Body(world, bag.Object), out var roots, out var graph, out var error), error);
		CollectionAssert.AreEqual(new[] { bag.Object }, roots);
		CollectionAssert.AreEquivalent(new[] { bag.Object, gun.Object, scope.Object, key.Object }, graph);
	}

	[TestMethod]
	public void Custody_CycleBelowValidRoot_RefusesWithoutCallingAnyTransferOrDelete()
	{
		var world = Mock.Of<IFuturemud>(); var root = Item(1, world); var child = Item(2, world);
		var outer = new Mock<IContainer>(); outer.SetupGet(x => x.Contents).Returns([child.Object]);
		var inner = new Mock<IContainer>(); inner.SetupGet(x => x.Contents).Returns([child.Object]);
		root.SetupGet(x => x.Components).Returns([outer.Object]); child.SetupGet(x => x.Components).Returns([inner.Object]);
		Assert.IsFalse(SpellOwnedNpcService.TryCaptureForeignCustody(Body(world, root.Object), out _, out _, out _));
		root.Verify(x => x.Delete(), Times.Never); child.Verify(x => x.Delete(), Times.Never);
		root.Verify(x => x.Get(It.IsAny<IBody>()), Times.Never);
	}

	[TestMethod]
	public void Custody_UnregisteredItem_RefusesBeforeSideEffectingIdGetter()
	{
		var world = Mock.Of<IFuturemud>(); var item = Item(1, world);
		item.As<ILateInitialisingItem>().SetupGet(x => x.IdHasBeenRegistered).Returns(false);
		item.SetupGet(x => x.Id).Throws(new InvalidOperationException("ID access would flush unrelated saves."));
		Assert.IsFalse(SpellOwnedNpcService.TryCaptureForeignCustody(Body(world, item.Object), out _, out _, out _));
		item.VerifyGet(x => x.Id, Times.Never);
	}

	[TestMethod]
	public void Custody_DuplicatePersistedIdentityOrDifferentWorld_Refuses()
	{
		var world = Mock.Of<IFuturemud>(); var first = Item(1, world); var second = Item(1, world);
		Assert.IsFalse(SpellOwnedNpcService.TryCaptureForeignCustody(Body(world, first.Object, second.Object), out _, out _, out _));
		second.SetupGet(x => x.Id).Returns(2); second.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		Assert.IsFalse(SpellOwnedNpcService.TryCaptureForeignCustody(Body(world, first.Object, second.Object), out _, out _, out _));
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1001)]
	public void Retirement_InvalidBound_RefusesBeforeDatabaseAccess(int limit)
	{
		var service = new SpellOwnedNpcService(Mock.Of<IFuturemud>());
		Assert.ThrowsException<ArgumentException>(() => service.ReconcileRetirements(DateTime.UtcNow, limit));
		Assert.ThrowsException<ArgumentException>(() => service.ReconcileRetirements(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)));
	}

	[TestMethod]
	public void Delete_PostCallbackConservationHold_RetainsEventsAndDoesNotReplayNotificationOnRetry()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		var item = new GameItem(proto);
		((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item)!).Add(Mock.Of<ICorpse>());
		var service = new Mock<ISpellOwnedNpcService>(); world.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var diagnostic = string.Empty; var gate = 0;
		service.Setup(x => x.TryPrepareRemainsRemoval(item, out diagnostic, false)).Returns(() => ++gate == 1);
		service.Setup(x => x.TryNotifyRemainsDeletion(item, It.IsAny<Action>())).Returns<IGameItem, Action>((_, notify) => { notify(); return true; });
		var notifications = 0; item.OnDeleted += _ => notifications++;
		item.OnQuit += _ => Assert.Fail("Refused removal sent quit.");
		item.Delete(); item.Delete();
		Assert.IsFalse(item.Deleted); Assert.AreEqual(1, notifications);
		Assert.IsNotNull(typeof(PerceivedItem).GetField("OnDeleted", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item));
		Assert.IsNotNull(typeof(PerceivedItem).GetField("OnQuit", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item));
		world.Verify(x => x.Destroy(item), Times.Never);
		world.Verify(x => x.SaveManager.Abort(item), Times.Never);
	}

	[TestMethod]
	public void Delete_ThrowingObserver_HoldsWithoutRepeatingPartialSideEffectsOrFinalizing()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var item = new GameItem(Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object));
		var notifications = 0;
		item.OnDeleted += _ => { notifications++; throw new InvalidOperationException("partial callback"); };
		item.OnQuit += _ => Assert.Fail("Failed deletion sent quit.");
		Assert.AreEqual("partial callback", Assert.ThrowsException<InvalidOperationException>(item.Delete).Message);
		Assert.AreEqual("Deletion observers failed; native removal remains held.", Assert.ThrowsException<InvalidOperationException>(item.Delete).Message);
		Assert.IsFalse(item.Deleted); Assert.AreEqual(1, notifications);
		Assert.IsNotNull(typeof(PerceivedItem).GetField("OnDeleted", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item));
		Assert.IsNotNull(typeof(PerceivedItem).GetField("OnQuit", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item));
		world.Verify(x => x.Destroy(item), Times.Never);
		world.Verify(x => x.SaveManager.Abort(item), Times.Never);
	}

	[TestMethod]
	public void Delete_DurableNotificationRefusal_DoesNotLatchPastTheGateOrReleaseNativeEvents()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var item = new GameItem(Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object));
		((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item)!).Add(Mock.Of<ICorpse>());
		var service = new Mock<ISpellOwnedNpcService>(); world.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var diagnostic = string.Empty; var attempts = 0; var notifications = 0;
		service.Setup(x => x.TryPrepareRemainsRemoval(item, out diagnostic, false)).Returns(() => notifications == 0);
		service.Setup(x => x.TryNotifyRemainsDeletion(item, It.IsAny<Action>())).Returns<IGameItem, Action>((_, notify) =>
		{
			if (++attempts < 3) return false;
			notify(); return true;
		});
		item.OnDeleted += _ => notifications++;
		item.OnQuit += _ => Assert.Fail("Held notification released native quit.");
		item.Delete(); item.Delete(); item.Delete(); item.Delete();
		Assert.AreEqual(3, attempts); Assert.AreEqual(1, notifications); Assert.IsFalse(item.Deleted);
		world.Verify(x => x.Destroy(item), Times.Never); world.Verify(x => x.SaveManager.Abort(item), Times.Never);
		Assert.IsNotNull(typeof(PerceivedItem).GetField("OnQuit", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item));
	}

	[TestMethod]
	public void Delete_CapturedForeignChild_RefusesBeforeObserversOrSaveSuppression()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var item = new GameItem(Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object));
		var calls = 0; item.OnDeleted += _ => calls++;
		using (ForeignCustodyTransferContext.Enter(Mock.Of<IBody>(), [item], Mock.Of<MudSharp.Construction.IRoom>()))
			Assert.ThrowsException<InvalidOperationException>(item.Delete);
		Assert.IsFalse(item.Deleted); Assert.AreEqual(0, calls);
		world.Verify(x => x.Destroy(item), Times.Never); world.Verify(x => x.SaveManager.Abort(item), Times.Never);
		Assert.IsNotNull(typeof(PerceivedItem).GetField("OnDeleted", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item));
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	[DataRow(4)]
	[DataRow(5)]
	[DataRow(6)]
	public void Read_RemovalJournalMissingOrOutOfOrderProof_Refuses(int corruption)
	{
		var now = new DateTime(2026, 10, 3, 0, 0, 0, DateTimeKind.Utc);
		var row = new MudSharp.Models.MagicSpellLifecycle
		{
			Id = Guid.NewGuid(), SpellId = 1, Grade = 3, CreatorId = 1, Family = "test",
			Mode = (int)SpellLifecycleMode.DeathOnExpiry, CreatedUtc = now, DeadlineUtc = now.AddMinutes(1),
			State = (int)SpellLifecycleState.RemainsPending, Reason = (int)SpellRetirementReason.EarlyDeath,
			DeathObservedUtc = now.AddSeconds(1), RemainsItemId = 42, UpdatedUtc = now.AddSeconds(5), Version = 1,
			RemainsRemovalRequestedUtc = now.AddSeconds(2), RemainsNotificationAttemptedUtc = now.AddSeconds(3),
			RemainsNotificationCompletedUtc = now.AddSeconds(4),
			Entities = [new() { Kind = (int)SpellOwnedEntityKind.Body, EntityId = 1 }]
		};
		var good = SpellOwnedLifecycleStore.Read(row);
		Assert.AreEqual(DateTimeKind.Utc, good.RemainsRemovalRequestedUtc!.Value.Kind);
		switch (corruption)
		{
			case 0: row.RemainsRemovalRequestedUtc = null; break;
			case 1: row.RemainsNotificationAttemptedUtc = null; break;
			case 2: row.RemainsItemId = null; break;
			case 3: row.RemainsRemovalRequestedUtc = now; break;
			case 4: row.RemainsNotificationCompletedUtc = now.AddSeconds(2); break;
			case 5: row.RemainsNotificationCompletedUtc = now.AddSeconds(6); break;
			case 6: row.Mode = (int)SpellLifecycleMode.Permanent; row.DeadlineUtc = null; break;
		}
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedLifecycleStore.Read(row));
	}

	[TestMethod]
	public void Morph_RemainsConservationHold_DoesNotConstructOrActivateReplacement()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = new Mock<IGameItemProto>(); proto.SetupGet(x => x.Gameworld).Returns(world.Object);
		var item = new GameItem(proto.Object);
		((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item)!).Add(Mock.Of<ICorpse>());
		var service = new Mock<ISpellOwnedNpcService>(); world.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var diagnostic = string.Empty;
		service.Setup(x => x.TryPrepareRemainsRemoval(item, out diagnostic, true)).Returns(false);
		typeof(GameItem).GetMethod("Morph", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(item, [item]);
		Assert.IsFalse(item.Deleted); proto.Verify(x => x.LoadMorphedItem(It.IsAny<IGameItem>()), Times.Never);
		world.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
	}
}
