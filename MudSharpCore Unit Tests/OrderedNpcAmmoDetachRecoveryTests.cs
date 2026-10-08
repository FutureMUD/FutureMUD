#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

public partial class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("unload", "throw")]
	[DataRow("unload", "foreign")]
	[DataRow("unload", "recovery-throw")]
	[DataRow("cycle", "throw")]
	[DataRow("cycle", "foreign")]
	[DataRow("cycle", "recovery-throw")]
	public void NativeGunDetach_PostSlotClearFailureRecoversOnlyUnclaimedParticipant(string operation, string change)
	{
		var f = new Fixture();
		var hand = Mock.Of<IGrab>();
		f.Body.SetupGet(x => x.HoldLocs).Returns([hand]);
		f.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponentProto>());
		var round = new GameItem(prototype.Object);
		round.ContainedIn = f.Item.Object;
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns((true, string.Empty));
		var gunProto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		gunProto.UnloadEmote = "@ unload|unloads $1.";
		var gun = new InternalMagazineGunGameItemComponent(gunProto, f.Item.Object, temporary: true);
		f.Item.Setup(x => x.GetItemType<InternalMagazineGunGameItemComponent>()).Returns(gun);
		var magazine = new List<IGameItem>(); Set(gun, "_roundsInMagazine", magazine);
		if (operation == "unload") magazine.Add(round);
		else { var ammo = new Mock<IAmmo>(); ammo.SetupGet(x => x.Parent).Returns(round); gun.ChamberedRound = ammo.Object; }
		var batch = new Mock<IProximityChangeBatch>(); var proximity = new Mock<IProximityEventService>();
		world.SetupGet(x => x.ProximityEventService).Returns(proximity.Object);
		proximity.Setup(x => x.BeginChange(ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>())).Returns(batch.Object);
		var original = new InvalidOperationException("post-slot-clear callback");
		var secondary = new ApplicationException("captured floor recovery");
		var foreign = Mock.Of<IGameItem>(); var callbacks = 0;
		batch.Setup(x => x.Complete()).Callback(() =>
		{
			if (callbacks != 0 || round.ContainedIn is not null) return;
			++callbacks;
			Assert.IsNull(round.ContainedIn); Assert.AreEqual(0, magazine.Count); Assert.IsNull(gun.ChamberedRound);
			if (change == "foreign") round.ContainedIn = foreign;
			throw original;
		});
		var room = Mock.Get(f.Actor.Object.Location);
		var items = new List<IGameItem>(); room.SetupGet(x => x.GameItems).Returns(items);
		room.Setup(x => x.Insert(round, true)).Callback(() => { if (change == "recovery-throw") throw secondary; items.Add(round); });
		using var execution = CommandExecutionScope.EnterDispatch(CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "unload firearm", () => true), f.Actor.Object);
		Exception? observed = null;
		try
		{
			if (operation == "unload") gun.Unload(f.Actor.Object);
			else
			{
				try { typeof(InternalMagazineGunGameItemComponent).GetMethod("ChamberRound", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(gun, [f.Actor.Object]); }
				catch (System.Reflection.TargetInvocationException error) when (error.InnerException is not null) { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error.InnerException).Throw(); }
			}
		}
		catch (Exception error) { observed = error; }
		Assert.AreSame(original, observed, "Recovery preserves original exception object even if recovery also fails. Observed: " + observed);
		Assert.AreEqual(1, callbacks); Assert.AreEqual(0, magazine.Count); Assert.IsNull(gun.ChamberedRound);
		Assert.AreEqual(1, round.Quantity); Assert.IsFalse(round.Deleted);
		Assert.AreSame(change == "foreign" ? foreign : null, round.ContainedIn);
		Assert.AreSame(change == "foreign" ? null : f.Actor.Object.Location, round.DirectLocation);
		Assert.AreEqual(change == "throw" ? 1 : 0, items.Count);
		room.Verify(x => x.Insert(round, true), change == "foreign" ? Times.Never : Times.Once);
	}
}
