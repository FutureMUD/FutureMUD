#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Combat;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

[TestClass]
public class InternalMagazineReloadTests
{
	[DataTestMethod]
	[DataRow("chamber")]
	[DataRow("magazine")]
	[DataRow("casing")]
	[DataRow("chamber-claimed")]
	[DataRow("magazine-claimed")]
	[DataRow("casing-claimed")]
	[DataRow("missing")]
	public void NativeInternalMagazineLoad_RestoresExactChildCustodyWithoutGameplayCallbacks(string slot)
	{
		var claimed = slot.EndsWith("-claimed", StringComparison.Ordinal);
		slot = slot.Split('-')[0];
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var parent = new Mock<IGameItem>(); parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		var foreign = claimed ? Mock.Of<IGameItem>() : null;
		var child = new Mock<IGameItem>(); child.SetupProperty(x => x.ContainedIn, foreign);
		child.Setup(x => x.LoadTimeSetContainedIn(parent.Object)).Callback(() => child.Object.ContainedIn = parent.Object);
		var ammo = new Mock<IAmmo>(); ammo.SetupGet(x => x.Parent).Returns(child.Object);
		child.Setup(x => x.GetItemType<IAmmo>()).Returns(ammo.Object);
		world.Setup(x => x.TryGetItem(It.IsAny<long>(), true)).Returns<long, bool>((id, _) => id == 11 ? child.Object : null!);
		var proto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		typeof(FirearmBaseGameItemComponentProto).GetField("<FireModes>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(proto, new List<FirearmFireMode> { new(FirearmFireModeType.Single, 1, 0, 0, 0) });
		proto.CycleType = FirearmCycleType.Manual;
		var xml = new XElement("Definition", new XElement("ChamberedRound", slot == "chamber" ? 11 : 0),
			new XElement("ChamberedCasing", slot == "casing" ? 11 : 0),
			new XElement("RoundsInMagazine", slot is "magazine" or "missing" ? new XElement("Round", slot == "missing" ? 999 : 11) : null));
		var gun = new InternalMagazineGunGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 1, Definition = xml.ToString() }, proto, parent.Object);
		Assert.AreSame(slot == "chamber" && !claimed ? ammo.Object : null, gun.ChamberedRound);
		Assert.AreSame(slot == "casing" && !claimed ? child.Object : null, gun.ChamberedCasing);
		CollectionAssert.AreEqual(slot == "magazine" && !claimed ? new[] { child.Object } : Array.Empty<IGameItem>(), gun.MagazineContents.ToArray());
		Assert.AreSame(claimed ? foreign : slot == "missing" ? null : parent.Object, child.Object.ContainedIn);
		child.Verify(x => x.LoadTimeSetContainedIn(parent.Object), slot == "missing" || claimed ? Times.Never : Times.Once);
		child.VerifySet(x => x.Changed = It.IsAny<bool>(), Times.Never);
		parent.VerifySet(x => x.Changed = It.IsAny<bool>(), Times.Never);
	}
}

public partial class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("expire")]
	[DataRow("replace")]
	public void NativeInternalMagazineUnready_OutputCallbackPreservesExactSlotAndCommitsOnlyAdmittedDetach(string change)
	{
		var f = new Fixture();
		f.Item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var hand = Mock.Of<IGrab>(); f.Body.SetupGet(x => x.HoldLocs).Returns([hand]);
		f.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns((true, string.Empty));
		var proto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		proto.UnreadyEmote = "@ unready|unreadies $1.";
		var gun = new InternalMagazineGunGameItemComponent(proto, f.Item.Object, temporary: true);
		f.World.SetupGet(x => x.SaveManager).Returns(Mock.Of<MudSharp.Framework.Save.ISaveManager>());
		gun.SetNoSave(false);
		Mock.Get(f.Actor.Object.OutputHandler).SetupGet(x => x.Perceiver).Returns(f.Actor.Object);
		Mock.Get(f.Actor.Object.Location).Setup(x => x.LayerCharacters(It.IsAny<MudSharp.Construction.RoomLayer>())).Returns([f.Actor.Object]);
		var item = new Mock<IGameItem>(); item.SetupProperty(x => x.ContainedIn, f.Item.Object);
		var hold = new Mock<IHoldable>(); hold.SetupProperty(x => x.HeldBy, null);
		item.Setup(x => x.GetItemType<IHoldable>()).Returns(hold.Object);
		item.SetupGet(x => x.InInventoryOf).Returns(() => hold.Object.HeldBy);
		var ammo = new Mock<IAmmo>(); ammo.SetupGet(x => x.Parent).Returns(item.Object); gun.ChamberedRound = ammo.Object;
		var replacement = Mock.Of<IAmmo>();
		f.Body.Setup(x => x.CanGet(item.Object, 0)).Returns(true);
		f.Body.Setup(x => x.Get(item.Object, 0, It.IsAny<MudSharp.PerceptionEngine.IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()))
			.Callback(() => { Assert.IsNull(gun.ChamberedRound); Assert.IsTrue(gun.Changed); hold.Object.HeldBy = f.Body.Object; });
		Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<MudSharp.PerceptionEngine.IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback(() => { if (change == "expire") f.Grant = null; if (change == "replace") gun.ChamberedRound = replacement; }).Returns(true);
		using var execution = CommandExecutionScope.EnterDispatch(CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "unready gun", () => true), f.Actor.Object);
		Assert.AreEqual(change == "valid", gun.Unready(f.Actor.Object));
		Assert.AreSame(change == "valid" ? null : change == "replace" ? replacement : ammo.Object, gun.ChamberedRound);
		Assert.AreSame(change == "valid" ? null : f.Item.Object, item.Object.ContainedIn);
		Assert.AreSame(change == "valid" ? f.Body.Object : null, hold.Object.HeldBy);
		f.Body.Verify(x => x.Get(item.Object, 0, It.IsAny<MudSharp.PerceptionEngine.IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()), change == "valid" ? Times.Once : Times.Never);
	}
}
