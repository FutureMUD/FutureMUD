#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemInteractionPolicyTests
{
	private static T Prototype<T>(IFuturemud world, string xml) where T : GameItemComponentProto
	{
		var model = VancianItemTests.Prototype(typeof(T).Name);
		model.Definition = xml;
		return (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic, null, [model, world], null)!;
	}

	[TestMethod]
	public void IndependentPermissions_RoundTripAndCopyWithoutRemovingHoldability()
	{
		var f = new VancianItemTests.ItemFixture(); var item = f.Item(101);
		var proto = Prototype<ItemPermissionsGameItemComponentProto>(f.F.World.Object, "<Definition><Restrictions>38</Restrictions></Definition>");
		var component = (ItemPermissionsGameItemComponent)proto.CreateNew(item.Object, temporary: true);
		item.Setup(x => x.GetItemTypes<IItemInteractionPermissions>()).Returns([component]);
		Assert.IsFalse(item.Object.Permits(ItemInteractionRestriction.Drop));
		Assert.IsFalse(item.Object.Permits(ItemInteractionRestriction.Give));
		Assert.IsFalse(item.Object.Permits(ItemInteractionRestriction.RoomListing));
		Assert.IsTrue(item.Object.Permits(ItemInteractionRestriction.Put));
		Assert.IsTrue(item.Object.Permits(ItemInteractionRestriction.Pickup));
		component.SetPermission(ItemInteractionRestriction.Drop, true);
		var saved = new MudSharp.Models.GameItemComponent { Id = 102, Definition = VancianItemTests.Serialize(component) };
		var restored = new ItemPermissionsGameItemComponent(saved, proto, item.Object);
		Assert.AreEqual(component.Restrictions, restored.Restrictions);
		Assert.AreEqual(component.Restrictions, ((IItemInteractionPermissions)restored.Copy(item.Object, true)).Restrictions);
		Assert.IsFalse(item.Object.Permits(ItemInteractionRestriction.Give));
	}

	[TestMethod]
	public void PickupCallback_IsNotAFeasibilityQuery_AndOneShotSurvivesReload()
	{
		var f = new VancianItemTests.ItemFixture(); var item = f.Item(201);
		var callback = new Mock<IFutureProg>(); callback.SetupGet(x => x.Id).Returns(99);
		callback.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		callback.Setup(x => x.MatchesParameters(It.IsAny<System.Collections.Generic.IEnumerable<ProgVariableTypes>>())).Returns(true);
		callback.Setup(x => x.ExecuteBool(false, It.IsAny<object[]>())).Returns(false);
		var progs = new All<IFutureProg>(); progs.Add(callback.Object); f.F.World.SetupGet(x => x.FutureProgs).Returns(progs);
		var proto = Prototype<ItemPermissionsGameItemComponentProto>(f.F.World.Object, "<Definition><PickupAttemptProg>99</PickupAttemptProg><PickupAttemptOnceOnly>true</PickupAttemptOnceOnly></Definition>");
		var component = (ItemPermissionsGameItemComponent)proto.CreateNew(item.Object, temporary: true);
		item.Setup(x => x.GetItemTypes<IItemInteractionPermissions>()).Returns([component]);
		Assert.IsTrue(item.Object.Permits(ItemInteractionRestriction.Pickup));
		callback.Verify(x => x.ExecuteBool(false, It.IsAny<object[]>()), Times.Never);
		Assert.IsFalse(component.HandlePickupAttempt(f.F.Actor.Object));
		var restored = new ItemPermissionsGameItemComponent(new() { Id = 202, Definition = VancianItemTests.Serialize(component) }, proto, item.Object);
		Assert.IsTrue(restored.HandlePickupAttempt(f.F.Actor.Object));
		callback.Verify(x => x.ExecuteBool(false, It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void MissingPickupCallback_FailsClosedWithoutConsumingOneShot()
	{
		var f = new VancianItemTests.ItemFixture(); var item = f.Item(203);
		f.F.World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var proto = Prototype<ItemPermissionsGameItemComponentProto>(f.F.World.Object, "<Definition><PickupAttemptProg>99</PickupAttemptProg><PickupAttemptOnceOnly>true</PickupAttemptOnceOnly></Definition>");
		var component = (ItemPermissionsGameItemComponent)proto.CreateNew(item.Object, temporary: true);
		Assert.IsFalse(component.HandlePickupAttempt(f.F.Actor.Object));
		StringAssert.Contains(VancianItemTests.Serialize(component), "<PickupAttempted>false</PickupAttempted>");
	}

	[TestMethod]
	public void TransientPolicy_OnlyRestoredInstancesRetireOnLoad_CopiesAreNew()
	{
		var f = new VancianItemTests.ItemFixture(); var item = f.Item(301);
		var proto = Prototype<TransientItemGameItemComponentProto>(f.F.World.Object, "<Definition/>");
		var component = (ITransientItemPolicy)proto.CreateNew(item.Object, temporary: true);
		Assert.IsFalse(component.RetireOnLoad); Assert.IsTrue(component.RetireOnLogout);
		var loaded = new TransientItemGameItemComponent(new() { Id = 302, Definition = "<Definition/>" }, proto, item.Object);
		Assert.IsTrue(loaded.RetireOnLoad);
		Assert.IsFalse(((ITransientItemPolicy)loaded.Copy(item.Object, true)).RetireOnLoad);
	}

	[TestMethod]
	public void ExplicitRetirement_DoesNotRetireOnUnconfiguredEvents()
	{
		var f = new VancianItemTests.ItemFixture(); var item = f.Item(401);
		var proto = Prototype<TransientItemGameItemComponentProto>(f.F.World.Object,
			"<Definition><RetireOnReload>false</RetireOnReload><RetireOnLogout>false</RetireOnLogout><Triggers>1</Triggers></Definition>");
		var component = (ITransientItemPolicy)proto.CreateNew(item.Object, temporary: true);
		item.Setup(x => x.GetItemTypes<ITransientItemPolicy>()).Returns([component]);
		Assert.IsFalse(component.RetireOnLoad); Assert.IsFalse(component.RetireOnLogout);
		Assert.IsFalse(ItemRetirement.Retire(item.Object, ItemRetirementTrigger.None));
		Assert.IsFalse(ItemRetirement.Retire(item.Object, ItemRetirementTrigger.TransientBodyDeath));
		item.Verify(x => x.Delete(), Times.Never);
		Assert.IsTrue(ItemRetirement.Retire(item.Object, ItemRetirementTrigger.BulkDrop));
		item.Verify(x => x.Delete(), Times.Once);
	}

	[TestMethod]
	public void DetectionVisibility_UsesRequiredSenseAndDoesNotChangePhysicalState()
	{
		var f = new VancianItemTests.ItemFixture(); var item = f.Item(501);
		var viewer = new Mock<ICharacter>();
		viewer.Setup(x => x.GetPerception(It.IsAny<PerceptionTypes>())).Returns(PerceptionTypes.VisualMagical);
		var invisibility = new DetectionVisibility(item.Object, PerceptionTypes.VisualMagical);
		var ethereal = new DetectionVisibility(item.Object, PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal);
		Assert.IsFalse(invisibility.Applies(viewer.Object));
		Assert.IsTrue(ethereal.Applies(viewer.Object));
		viewer.Setup(x => x.GetPerception(It.IsAny<PerceptionTypes>())).Returns(PerceptionTypes.SenseEthereal);
		Assert.IsFalse(ethereal.Applies(viewer.Object));
		Assert.IsTrue(invisibility.Applies(viewer.Object));
		Assert.IsNull(item.Object.BasePlanarPresence);
	}

	[TestMethod]
	public void GlowIsDescriptive_HumRespectsNativeDeafnessAndRoomBoundary()
	{
		var f = new VancianItemTests.ItemFixture(); var item = f.Item(601);
		var viewer = new Mock<ICharacter>(); viewer.Setup(x => x.CanHear(item.Object)).Returns(false);
		var glow = new ItemPresentation(item.Object, false); var hum = new ItemPresentation(item.Object, true);
		Assert.IsTrue(glow.DescriptionAdditionApplies(viewer.Object));
		Assert.AreEqual("(glowing)", glow.GetAddendumText(false));
		Assert.IsFalse(hum.DescriptionAdditionApplies(viewer.Object));
		Assert.IsFalse((object)glow is MudSharp.Effects.Interfaces.IAreaLightEffect);
	}
	[TestMethod]
	[DataRow("container")]
	[DataRow("recipient")]
	public void PickupCallback_ChangingCapturedSourceAbortsOuterPickup(string destination)
	{
		var item = new Mock<IGameItem>(); var actor = new Mock<ICharacter>();
		var permission = new Mock<IItemInteractionPermissions>();
		item.Setup(x => x.GetItemTypes<IItemInteractionPermissions>()).Returns([permission.Object]);
		var current = "ground";
		permission.Setup(x => x.HandlePickupAttempt(actor.Object)).Callback(() => current = destination).Returns(true);
		Assert.IsFalse(ItemPickupAttempt.TryRun(item.Object, actor.Object, () => current == "ground"));
		Assert.AreEqual(destination, current);
		permission.Verify(x => x.HandlePickupAttempt(actor.Object), Times.Once);
	}

	[TestMethod]
	public void PickupCallback_RejectsBeforeAnySplitOrTransfer()
	{
		var item = new Mock<IGameItem>(); var actor = new Mock<ICharacter>();
		var permission = new Mock<IItemInteractionPermissions>();
		item.Setup(x => x.GetItemTypes<IItemInteractionPermissions>()).Returns([permission.Object]);
		permission.Setup(x => x.HandlePickupAttempt(actor.Object)).Returns(false);
		Assert.IsFalse(ItemPickupAttempt.TryRun(item.Object, actor.Object, () => true));
		item.Verify(x => x.GetByWeight(It.IsAny<MudSharp.Body.IBody>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Retirement_RechecksLaterRootsAfterDeletionCallbacks(bool equipped)
	{
		var actor = new Mock<ICharacter>(); var body = new Mock<MudSharp.Body.IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		var first = new Mock<IGameItem>(); var second = new Mock<IGameItem>();
		var policy = new Mock<ITransientItemPolicy>(); policy.SetupGet(x => x.Triggers).Returns(ItemRetirementTrigger.TransientBodyDeath);
		foreach (var item in new[] { first, second })
		{
			item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
			item.Setup(x => x.GetItemTypes<ITransientItemPolicy>()).Returns([policy.Object]);
		}
		var roots = new System.Collections.Generic.List<IGameItem> { first.Object, second.Object };
		var worn = new System.Collections.Generic.List<IGameItem>();
		body.SetupGet(x => x.DirectItems).Returns(() => roots);
		body.SetupGet(x => x.WornItems).Returns(() => worn);
		body.SetupGet(x => x.WieldedItems).Returns(Array.Empty<IGameItem>());
		first.Setup(x => x.Delete()).Callback(() => { if (equipped) worn.Add(second.Object); else roots.RemoveAt(1); });
		ItemRetirement.RetireCarriedItems(actor.Object, ItemRetirementTrigger.TransientBodyDeath);
		first.Verify(x => x.Delete(), Times.Once);
		second.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void CarriedHum_DeafWearerDoesNotBypassHearingWithSelfPerception()
	{
		var source = new Mock<ICharacter>(); var body = new Mock<MudSharp.Body.IBody>();
		var room = new Mock<MudSharp.Construction.IRoom>(); var world = new Mock<IFuturemud>();
		source.SetupGet(x => x.Body).Returns(body.Object);
		source.SetupGet(x => x.Location).Returns(room.Object);
		source.Setup(x => x.IsSelf(source.Object)).Returns(true);
		source.Setup(x => x.CanHear(It.IsAny<IPerceivable>())).Returns(false);
		var items = Enumerable.Range(0, 3).Select(_ =>
		{
			var item = new Mock<IGameItem>(); item.SetupGet(x => x.Gameworld).Returns(world.Object);
			item.SetupGet(x => x.Effects).Returns(new MudSharp.Effects.IEffect[] { new ItemPresentation(item.Object, true) });
			return item.Object;
		}).ToArray();
		body.SetupGet(x => x.DirectItems).Returns(items);
		body.SetupGet(x => x.ExternalItemsForOtherActors).Returns(items);
		body.SetupGet(x => x.WornItems).Returns(Array.Empty<IGameItem>());
		Assert.AreEqual("", ItemPresentation.DescribeCarriedHum(source.Object, source.Object));
	}

}
