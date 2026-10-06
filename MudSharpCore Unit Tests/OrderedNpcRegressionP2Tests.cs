#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

public partial class QueuedCommandAuthorityTests
{
	[TestMethod]
	public void TeleportReconciliation_PreservesDifferentPhysicalInstanceOfSameCanonicalCharacter()
	{
		var moved = new Mock<MudSharp.Character.ICharacter>();
		var other = new Mock<MudSharp.Character.ICharacter>();
		moved.SetupGet(x => x.Id).Returns(50);
		moved.SetupGet(x => x.InstanceId).Returns(11);
		other.SetupGet(x => x.Id).Returns(50);
		other.SetupGet(x => x.InstanceId).Returns(12);
		other.Setup(x => x.Equals(moved.Object)).Returns(true);
		var source = new Mock<ICell>();
		var target = new Mock<ICell>();
		source.SetupGet(x => x.Characters).Returns([other.Object]);
		target.SetupGet(x => x.Characters).Returns([moved.Object]);
		moved.SetupGet(x => x.Location).Returns(target.Object);
		Assert.IsTrue(source.Object.Characters.Contains(moved.Object), "Reproduce canonical equality across separate physical instances.");
		typeof(MudSharp.Character.Character).GetMethod("ReconcileTeleportCellMembership", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [moved.Object, target.Object, source.Object]);
		source.Verify(x => x.Leave(It.IsAny<MudSharp.Character.ICharacter>()), Times.Never);
		Assert.AreSame(other.Object, source.Object.Characters.Single());
	}

	[TestMethod]
	public void OrderedDisplacement_SourceCellWithDestinationLayerRemainsInvalid()
	{
		using var d = new DisplacementFixture();
		d.Reach("gap");
		d.F.Actor.Object.RoomLayer = RoomLayer.InAir;
		Assert.IsFalse(d.Receipt.Continue(), "The Teleport repair must not authorize arbitrary mixed source/destination positions.");
		Assert.IsFalse(d.Receipt.BeginEnter(d.F.Actor.Object, d.Destination.Object));
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("owner-before")]
	[DataRow("owner-predicate")]
	[DataRow("survivor-before")]
	[DataRow("survivor-predicate")]
	[DataRow("source-predicate")]
	[DataRow("rebind-predicate")]
	[DataRow("description-predicate")]
	public void NativeGetCompletion_RevalidatesMergeAfterRemovalAndPredicateCallbacks(string change)
	{
		var f = new Fixture();
		var body = TestObjectFactory.CreateUninitialized<Body>();
		body.Actor = f.Actor.Object;
		f.Actor.SetupGet(x => x.Body).Returns(body);
		var source = new Mock<IGameItem>();
		var survivor = new Mock<IGameItem>();
		var sourceHold = new Mock<IHoldable>();
		var survivorHold = new Mock<IHoldable>();
		sourceHold.SetupProperty(x => x.HeldBy, body);
		survivorHold.SetupProperty(x => x.HeldBy, body);
		source.Setup(x => x.GetItemType<IHoldable>()).Returns(sourceHold.Object);
		survivor.Setup(x => x.GetItemType<IHoldable>()).Returns(survivorHold.Object);
		var held = new List<Tuple<IGameItem, IGrab>> { Tuple.Create(survivor.Object, Mock.Of<IGrab>()) };
		Set(body, "_heldItems", held);
		var wielded = typeof(Body).GetField("_wieldedItems", BindingFlags.Instance | BindingFlags.NonPublic)!;
		wielded.SetValue(body, Activator.CreateInstance(wielded.FieldType));
		var owner = new ItemOwnershipReference("Character", 30);
		ItemOwnershipReference? survivorOwner = owner;
		source.SetupGet(x => x.OwnershipReference).Returns(owner);
		survivor.SetupGet(x => x.OwnershipReference).Returns(() => survivorOwner);
		string? description = null;
		survivor.SetupGet(x => x.OverrideSdesc).Returns(() => description!);
		var other = Mock.Of<ICell>();
		ICell? sourceLocation = null;
		source.SetupGet(x => x.Location).Returns(() => sourceLocation!);
		source.Setup(x => x.Drop(It.IsAny<ICell>())).Callback<ICell>(cell =>
		{
			sourceHold.Object.HeldBy = null;
			sourceLocation = cell;
		}).Returns(source.Object);
		void RelocateSurvivor()
		{
			held.Clear();
			survivorHold.Object.HeldBy = null;
			survivor.SetupGet(x => x.Location).Returns(other);
		}
		if (change == "owner-before") survivorOwner = new ItemOwnershipReference("Character", 2);
		if (change == "survivor-before") RelocateSurvivor();
		survivor.Setup(x => x.CanMerge(source.Object)).Callback(() =>
		{
			if (change == "owner-predicate") survivorOwner = new ItemOwnershipReference("Character", 2);
			if (change == "survivor-predicate") RelocateSurvivor();
			if (change == "source-predicate") { sourceHold.Object.HeldBy = null; sourceLocation = other; }
			if (change == "rebind-predicate") body.Actor = f.Owner.Object;
			if (change == "description-predicate") description = "a changed stack";
		}).Returns(() => survivorOwner == owner);
		var preparedType = typeof(Body).GetNestedType("PreparedGet", BindingFlags.NonPublic)!;
		var placement = Activator.CreateInstance(preparedType, [null!, survivor.Object, f.Actor.Object, f.Actor.Object.Location, RoomLayer.GroundLevel])!;
		var result = (bool)typeof(Body).GetMethod("CompleteGetPlacement", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(body, [source.Object, placement])!;
		Assert.AreEqual(change == "valid", result);
		survivor.Verify(x => x.Merge(source.Object), change == "valid" ? Times.Once : Times.Never);
		if (change == "source-predicate") Assert.AreSame(other, sourceLocation, "Preserve callback-established source custody.");
		else if (change != "valid") Assert.AreSame(f.Actor.Object.Location, sourceLocation, "Complete the already accepted Get with a safe floor fallback.");
	}

	[DataTestMethod]
	[DataRow("source")]
	[DataRow("survivor")]
	public void AmmoUnloadMerge_ComponentPrototypeCallbackPreservesBothStacks(string change)
	{
		var f = new Fixture();
		var body = TestObjectFactory.CreateUninitialized<Body>();
		body.Actor = f.Actor.Object;
		f.Actor.SetupGet(x => x.Body).Returns(body);
		var source = new Mock<IGameItem>();
		var survivor = new Mock<IGameItem>();
		var sourceHold = new Mock<IHoldable>();
		var survivorHold = new Mock<IHoldable>();
		sourceHold.SetupProperty(x => x.HeldBy, body);
		survivorHold.SetupProperty(x => x.HeldBy, body);
		source.Setup(x => x.GetItemType<IHoldable>()).Returns(sourceHold.Object);
		survivor.Setup(x => x.GetItemType<IHoldable>()).Returns(survivorHold.Object);
		var held = new List<Tuple<IGameItem, IGrab>> { Tuple.Create(survivor.Object, Mock.Of<IGrab>()) };
		Set(body, "_heldItems", held);
		var wielded = typeof(Body).GetField("_wieldedItems", BindingFlags.Instance | BindingFlags.NonPublic)!;
		wielded.SetValue(body, Activator.CreateInstance(wielded.FieldType));
		var owner = new ItemOwnershipReference("Character", 30);
		ItemOwnershipReference? survivorOwner = owner;
		source.SetupGet(x => x.OwnershipReference).Returns(owner);
		survivor.SetupGet(x => x.OwnershipReference).Returns(() => survivorOwner);
		string? description = null;
		survivor.SetupGet(x => x.OverrideSdesc).Returns(() => description!);
		var other = Mock.Of<ICell>();
		ICell? sourceLocation = null;
		source.SetupGet(x => x.Location).Returns(() => sourceLocation!);
		source.Setup(x => x.Drop(It.IsAny<ICell>())).Callback<ICell>(cell =>
		{
			sourceHold.Object.HeldBy = null;
			sourceLocation = cell;
		}).Returns(source.Object);
		var sourceComponent = new Mock<IGameItemComponent>(); var survivorComponent = new Mock<IGameItemComponent>();
		IGameItemComponentProto sourceProto = Mock.Of<IGameItemComponentProto>(), survivorProto = Mock.Of<IGameItemComponentProto>();
		sourceComponent.SetupGet(x => x.Prototype).Returns(() => sourceProto); survivorComponent.SetupGet(x => x.Prototype).Returns(() => survivorProto);
		source.SetupGet(x => x.Components).Returns([sourceComponent.Object]); survivor.SetupGet(x => x.Components).Returns([survivorComponent.Object]);
		source.SetupGet(x => x.Quantity).Returns(3); survivor.SetupGet(x => x.Quantity).Returns(5);
		survivor.Setup(x => x.CanMerge(source.Object)).Callback(() =>
		{
			if (change == "source") sourceProto = Mock.Of<IGameItemComponentProto>(); else survivorProto = Mock.Of<IGameItemComponentProto>();
		}).Returns(true);
		var preparedType = typeof(Body).GetNestedType("PreparedGet", BindingFlags.NonPublic)!;
		var placement = Activator.CreateInstance(preparedType, [null!, survivor.Object, f.Actor.Object, f.Actor.Object.Location, RoomLayer.GroundLevel])!;
		object?[] arguments = [source.Object, placement, null, true, true];
		var result = (bool)typeof(Body).GetMethod("CompleteGetPlacementWithResult", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(body, arguments)!;
		Assert.IsFalse(result); survivor.Verify(x => x.Merge(source.Object), Times.Never);
		source.Verify(x => x.Delete(), Times.Never); Assert.AreEqual(3, source.Object.Quantity); Assert.AreEqual(5, survivor.Object.Quantity);
		Assert.AreSame(f.Actor.Object.Location, sourceLocation);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("chamber-refusal")]
	[DataRow("direct")]
	[DataRow("empty")]
	[DataRow("ejection-expire")]
	[DataRow("policy-foreign")]
	public void NativeInternalMagazineReady_ReportsAdmissionAndCommitsOnlyAcceptedChambering(string scenario)
	{
		var f = new Fixture();
		var hand = Mock.Of<IGrab>();
		f.Body.SetupGet(x => x.HoldLocs).Returns([hand]);
		f.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns((true, string.Empty));
		var proto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		typeof(FirearmBaseGameItemComponentProto).GetField("_rangedWeaponType", BindingFlags.NonPublic | BindingFlags.Instance)!
			.SetValue(proto, Mock.Of<IRangedWeaponType>());
		proto.ReadyEmote = "@ ready|readies $1.";
		var gun = new InternalMagazineGunGameItemComponent(proto, f.Item.Object, temporary: true);
		var ammo = new Mock<IAmmo>();
		var round = new Mock<IGameItem>();
		round.SetupGet(x => x.Quantity).Returns(1);
		round.SetupProperty(x => x.ContainedIn, f.Item.Object);
		round.Setup(x => x.Equals(round.Object)).Returns(true);
		round.Setup(x => x.GetItemType<IAmmo>()).Returns(ammo.Object);
		ammo.SetupGet(x => x.Parent).Returns(round.Object);
		var magazine = scenario == "empty" ? new List<IGameItem>() : new List<IGameItem> { round.Object };
		Set(gun, "_roundsInMagazine", magazine);
		Mock<IGameItem>? ejectedSource = null; var foreignBag = Mock.Of<IGameItem>();
		if (scenario is "ejection-expire" or "policy-foreign")
		{
			var ejected = ejectedSource = new Mock<IGameItem>();
			var oldAmmo = new Mock<IAmmo>();
			oldAmmo.SetupGet(x => x.Parent).Returns(ejected.Object);
			ejected.SetupProperty(x => x.ContainedIn, f.Item.Object);
			ICell? ejectedCell = null; ejected.SetupGet(x => x.Location).Returns(() => ejectedCell!);
			ejected.Setup(x => x.Drop(f.Actor.Object.Location)).Callback(() => ejectedCell = f.Actor.Object.Location).Returns(ejected.Object);
			gun.ChamberedRound = oldAmmo.Object;
			Mock.Get(f.Actor.Object.Location).Setup(x => x.Insert(ejected.Object, It.IsAny<bool>())).Callback(() =>
			{
				Assert.IsNull(gun.ChamberedRound, "Publish ejection only after clearing the exact chamber slot.");
				f.Grant = null;
			});
		}
		var entered = false;
		f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() =>
		{
			if (!new StackTrace().GetFrames().Any(x => x.GetMethod()?.Name == "ChamberRound")) return true;
			entered = true;
			if (scenario == "policy-foreign" && new StackTrace().GetFrames().Any(x => x.GetMethod()?.Name == "Detach"))
			{ using var independent = CommandExecutionScope.EnterIndependent(); ejectedSource!.Object.ContainedIn = foreignBag; }
			return scenario != "chamber-refusal";
		});
		using var execution = scenario == "direct" ? CommandExecutionScope.EnterIndependent() : CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get firearm", () => f.Prog.Object.ExecuteBool([]) == true), f.Actor.Object);
		var accepted = gun.Ready(f.Actor.Object);
		Assert.AreEqual(scenario is not ("chamber-refusal" or "policy-foreign"), accepted);
		if (scenario != "direct") Assert.IsTrue(entered, "Exercise the actual override admission gate.");
		Assert.AreEqual(scenario == "chamber-refusal", CommandExecutionScope.RejectedBeforeCommit);
		Assert.AreSame(scenario is "empty" or "chamber-refusal" or "ejection-expire" or "policy-foreign" ? null : ammo.Object, gun.ChamberedRound);
		Assert.AreEqual(scenario is "chamber-refusal" or "ejection-expire" or "policy-foreign" ? 1 : 0, magazine.Count);
	}


	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void NativeReadyMove_UsesActualReadyOutcomeForSuccessAndStamina(bool ordered, bool accepted)
	{
		var f = new Fixture();
		var weapon = new Mock<IRangedWeapon>();
		weapon.Setup(x => x.CanReady(f.Actor.Object)).Returns(true);
		weapon.Setup(x => x.Ready(f.Actor.Object)).Returns(accepted);
		var move = new ReadyRangedWeaponMove { Assailant = f.Actor.Object, Weapon = weapon.Object };
		if (ordered) CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get firearm", () => true).Bind(move);
		var result = CommandExecutionScope.Resolve(move, null!);
		Assert.AreEqual(accepted, result.MoveWasSuccessful);
		Assert.AreEqual(accepted, move.UsesStaminaWithResult(result));
		weapon.Verify(x => x.Ready(f.Actor.Object), Times.Once);
	}
	[DataTestMethod]
	[DataRow("cleared")]
	[DataRow("replacement")]
	[DataRow("foreign")]
	[DataRow("throw")]
	[DataRow("throw-claim")]
	public void NativeSingleFire_PinsAcceptedRoundAndCompletesExactCasingOnFailure(string scenario)
	{
		var f = new Fixture(); var hand = Mock.Of<IGrab>();
		f.Actor.SetupGet(x => x.SpatialLocation).Returns(new MudSharp.Framework.SpatialLocation(f.Actor.Object.Location, RoomLayer.GroundLevel, null));
		f.Body.SetupGet(x => x.HoldLocs).Returns([hand]); f.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns((true, string.Empty));
		var proto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		typeof(FirearmBaseGameItemComponentProto).GetField("_rangedWeaponType", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(proto, Mock.Of<IRangedWeaponType>());
		typeof(FirearmBaseGameItemComponentProto).GetField("<AttachmentSlots>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(proto, new List<FirearmAttachmentSlot>());
		typeof(FirearmBaseGameItemComponentProto).GetField("<FireModes>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(proto, new List<FirearmFireMode> { new(FirearmFireModeType.Single, 1, 0, 0, 0) });
		proto.CycleType = FirearmCycleType.Manual; proto.FireEmote = "@ fire|fires $2.";
		var gun = new InternalMagazineGunGameItemComponent(proto, f.Item.Object, temporary:true);
		var round = new Mock<IGameItem>(); var ammo = new Mock<IAmmo>();
		round.SetupProperty(x => x.ContainedIn, f.Item.Object); round.SetupGet(x => x.Quantity).Returns(1); round.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponent>());
		ammo.SetupGet(x => x.Parent).Returns(round.Object); ammo.SetupGet(x => x.AmmoType).Returns(Mock.Of<MudSharp.Combat.IAmmunitionType>());
		gun.ChamberedRound = ammo.Object;
		var casing = new Mock<IGameItem>(); var casingHold = new Mock<IHoldable>(); casingHold.SetupProperty(x => x.HeldBy);
		casing.SetupGet(x => x.InInventoryOf).Returns(() => casingHold.Object.HeldBy!);
		casing.Setup(x => x.GetItemType<IHoldable>()).Returns(casingHold.Object); casing.SetupGet(x => x.Quantity).Returns(1); casing.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponent>());
		ICell? casingCell = null; casing.SetupGet(x => x.Location).Returns(() => casingCell!);
		Mock.Get(f.Actor.Object.Location).Setup(x => x.Insert(casing.Object, It.IsAny<bool>())).Callback(() => casingCell = f.Actor.Object.Location);
		ammo.SetupGet(x => x.GetFiredWasteItem).Returns(casing.Object);
		var failure = new InvalidOperationException("exact shot observer failure");
		ammo.Setup(x => x.Fire(It.IsAny<MudSharp.Character.ICharacter>(), It.IsAny<MudSharp.Framework.IPerceiver>(), It.IsAny<MudSharp.RPG.Checks.Outcome>(), It.IsAny<MudSharp.RPG.Checks.Outcome>(), It.IsAny<MudSharp.RPG.Checks.OpposedOutcome>(), It.IsAny<IBodypart>(), It.IsAny<IGameItem>(), It.IsAny<IRangedWeaponType>(), It.IsAny<MudSharp.PerceptionEngine.IEmoteOutput>(), It.IsAny<RangedFireContext>()))
			.Callback(() => { if (scenario == "throw-claim") casingHold.Object.HeldBy = f.Body.Object; throw failure; });
		var firePolicyCalls = 0; var admittedGates = new HashSet<int>(); var changedAtAdmission = false;
		f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() =>
		{
			var frames = new StackTrace().GetFrames();
			var gate = Array.FindIndex(frames, x => x.GetMethod()?.DeclaringType == typeof(CommandExecutionScope) && x.GetMethod()?.Name == "TryContinue");
			var caller = gate >= 0 && gate + 1 < frames.Length ? frames[gate + 1] : null;
			if (caller?.GetMethod()?.DeclaringType == typeof(FirearmBaseGameItemComponent) && caller.GetMethod()?.Name == "Fire" && admittedGates.Add(caller.GetILOffset())) ++firePolicyCalls;
			if (!scenario.StartsWith("throw") && !changedAtAdmission && firePolicyCalls == 2)
			{
				changedAtAdmission = true; using var independent = CommandExecutionScope.EnterIndependent();
				if (scenario == "cleared") { gun.ChamberedRound = null; round.Object.ContainedIn = null; }
				if (scenario == "replacement") gun.ChamberedRound = Mock.Of<IAmmo>();
				if (scenario == "foreign") round.Object.ContainedIn = Mock.Of<IGameItem>();
			}
			return true;
		});
		using var execution = CommandExecutionScope.EnterDispatch(CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get firearm", () => f.Prog.Object.ExecuteBool([]) == true), f.Actor.Object);
		void Fire() => gun.Fire(f.Actor.Object, null!, MudSharp.RPG.Checks.Outcome.MajorFail, MudSharp.RPG.Checks.Outcome.MajorPass, null!, null!, null!, null!);
		if (scenario.StartsWith("throw"))
		{
			Assert.AreSame(failure, Assert.ThrowsException<InvalidOperationException>(Fire));
			Assert.IsNull(gun.ChamberedCasing); Assert.IsNull(casing.Object.ContainedIn);
			if (scenario == "throw") Assert.AreSame(f.Actor.Object.Location, casingCell);
			else { Assert.AreSame(f.Body.Object, casingHold.Object.HeldBy); Assert.IsNull(casingCell); }
		}
		else
		{
			Fire(); ammo.VerifyGet(x => x.GetFiredWasteItem, Times.Never);
			Assert.AreEqual(2, firePolicyCalls, "Replace the exact participant at the final retained-grant policy gate.");
			if (scenario == "replacement") Assert.AreNotSame(ammo.Object, gun.ChamberedRound);
		}
	}

	[TestMethod]
	public void PreparedComponentFloor_PreservesExactParticipantWithoutSecondMerge()
	{
		var f = new Fixture(); var item = new Mock<IGameItem>(); ICell? cell = null;
		item.SetupGet(x => x.Location).Returns(() => cell!); item.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		item.Setup(x => x.Drop(f.Actor.Object.Location)).Callback(() => cell = f.Actor.Object.Location).Returns(item.Object);
		var destination = Mock.Get(f.Actor.Object.Location);
		destination.Setup(x => x.Insert(item.Object, It.IsAny<bool>())).Callback<IGameItem,bool>((_,newStack) => Assert.IsTrue(newStack, "An exact captured receipt cannot merge into a different floor identity."));
		using var execution = CommandExecutionScope.EnterDispatch(CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get firearm", () => true),f.Actor.Object);
		var complete = ComponentUnloadCompletion.PrepareFloor(f.Actor.Object,item.Object,f.Actor.Object);
		Assert.IsNotNull(complete); complete();
		destination.Verify(x => x.Insert(item.Object,true),Times.Once); destination.Verify(x => x.Insert(item.Object,false),Times.Never);
		Assert.AreSame(f.Actor.Object.Location,cell);
	}

}
