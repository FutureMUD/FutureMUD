#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreparedGetStackMergeTests
{
	[DataTestMethod]
	[DataRow(0)]
	[DataRow(-1)]
	public void EmptyStack_CannotBeRetrievedMergedOrTargeted(int quantity)
	{
		var (survivor, source, _) = Pair(5, quantity);
		Assert.AreEqual(ItemGetResponse.Unpositionable, source.CanGet());
		Assert.AreEqual(ItemGetResponse.Unpositionable, source.CanGet(0));
		Assert.AreEqual(ItemGetResponse.Unpositionable, source.CanGet(1));
		Assert.IsFalse(survivor.CanMerge(source));
		Assert.IsFalse(source.CanMerge(survivor));
		CollectionAssert.AreEqual(new IGameItem[] { survivor },
			MudSharp.Character.Character.IncludeTargetProjections([source, survivor]).ToArray());
		var projections = new Mock<IProvideItemTargetProjections>();
		projections.SetupGet(x => x.TargetProjections).Returns([source, survivor]);
		var host = new Mock<IGameItem>();
		host.SetupGet(x => x.Components).Returns([projections.Object]);
		CollectionAssert.AreEqual(new IGameItem[] { host.Object, survivor },
			MudSharp.Character.Character.IncludeTargetProjections([host.Object]).ToArray());
		Assert.AreEqual(5, survivor.Quantity);
		Assert.AreEqual(quantity, source.Quantity);
	}

	[DataTestMethod]
	[DataRow(0, false)]
	[DataRow(-1, false)]
	[DataRow(2, false)]
	[DataRow(2, true)]
	public void PreparedFloorRecovery_OnlyRestoresPositiveStackValue(int quantity, bool emptyDuringDrop)
	{
		var (_, source, _) = Pair(5, quantity);
		source.GetItemType<IHoldable>()!.HeldBy = null;
		var floor = new Mock<IRoom>();
		var items = new List<IGameItem>();
		floor.SetupGet(x => x.GameItems).Returns(items);
		floor.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()))
			.Callback<IGameItem, bool>((item, _) => items.Add(item));
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(floor.Object);
		if (emptyDuringDrop)
		{
			var component = new Mock<IGameItemComponent>();
			var emptied = false;
			component.Setup(x => x.Taken()).Callback(() =>
			{
				if (emptied) return;
				emptied = true;
				source.GetItemType<IStackable>()!.Quantity = 0;
			});
			((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!
				.GetValue(source)!).Add(component.Object);
		}
		var recover = ComponentUnloadCompletion.PrepareFloor(actor.Object, source, actor.Object);
		Assert.IsNotNull(recover);
		recover();

		Assert.AreEqual(emptyDuringDrop ? 0 : quantity, source.Quantity);
		Assert.AreEqual(quantity > 0 && !emptyDuringDrop ? 1 : 0, items.Count);
		Assert.IsNull(source.GetItemType<IHoldable>()!.HeldBy);
		if (quantity <= 0 || emptyDuringDrop) Assert.IsNull(source.DirectLocation);
		else Assert.AreSame(floor.Object, source.DirectLocation);
		if (emptyDuringDrop)
		{
			source.GetItemType<IStackable>()!.Quantity = 2;
			recover();
			Assert.AreSame(floor.Object, source.DirectLocation);
			CollectionAssert.AreEqual(new IGameItem[] { source }, items);
			Assert.AreEqual(2, source.Quantity);
		}
	}

	[DataTestMethod]
	[DataRow("ammo")]
	[DataRow("foreign-component")]
	[DataRow("callback-component")]
	[DataRow("delete-throw")]
	[DataRow("refill")]
	[DataRow("refill-empty")]
	[DataRow("refill-empty-throw")]
	public void GetCompletion_AbsorbedSourceHasNoEmptyFloorRemnantAndPreservesRefills(string scenario)
	{
		var (survivor, source, _) = Pair(5, 3);
		var body = TestObjectFactory.CreateUninitialized<Body>();
		var floor = new Mock<IRoom>();
		var floorItems = new List<IGameItem>();
		floor.SetupGet(x => x.GameItems).Returns(floorItems);
		floor.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()))
			.Callback<IGameItem, bool>((item, _) => floorItems.Add(item));
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body);
		actor.SetupGet(x => x.Location).Returns(floor.Object);
		actor.SetupGet(x => x.Gameworld).Returns(source.Gameworld);
		body.Actor = actor.Object;
		typeof(Body).GetField("_heldItems", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(body, new List<Tuple<IGameItem, IGrab>> { Tuple.Create<IGameItem, IGrab>(survivor, Mock.Of<IGrab>()) });
		var wielded = typeof(Body).GetField("_wieldedItems", BindingFlags.Instance | BindingFlags.NonPublic)!;
		wielded.SetValue(body, Activator.CreateInstance(wielded.FieldType));
		survivor.GetItemType<IHoldable>()!.HeldBy = body;
		source.GetItemType<IHoldable>()!.HeldBy = body;
		var sourceComponents = (List<IGameItemComponent>)typeof(GameItem)
			.GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(source)!;
		if (scenario == "ammo")
		{
			sourceComponents.Add(new AmmunitionGameItemComponent((AmmunitionGameItemComponentProto)null!, source, temporary: true));
			((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!
				.GetValue(survivor)!).Add(new AmmunitionGameItemComponent((AmmunitionGameItemComponentProto)null!, survivor, temporary: true));
		}
		var foreign = Mock.Of<IGameItemComponent>();
		if (scenario == "foreign-component") sourceComponents.Add(foreign);
		if (scenario == "callback-component") survivor.GetItemType<StackableGameItemComponent>()!.DescriptionUpdate += (_, _) => sourceComponents.Add(foreign);
		var expected = new InvalidOperationException("fixture deletion failure");
		source.OnDeleted += _ =>
		{
			if (scenario == "delete-throw") throw expected;
			if (scenario is "refill" or "refill-empty" or "refill-empty-throw") source.GetItemType<IStackable>()!.Quantity = 2;
			if (scenario is "refill-empty" or "refill-empty-throw")
			{
				var component = new Mock<IGameItemComponent>();
				component.Setup(x => x.Taken()).Callback(() =>
				{
					source.GetItemType<IStackable>()!.Quantity = 0;
					if (scenario == "refill-empty-throw") throw expected;
				});
				sourceComponents.Add(component.Object);
			}
		};
		var preparedType = typeof(Body).GetNestedType("PreparedGet", BindingFlags.NonPublic)!;
		var placement = Activator.CreateInstance(preparedType, [null!, survivor, actor.Object, floor.Object, RoomLayer.GroundLevel])!;
		object?[] arguments = [source, placement, null, true, false];
		var completion = typeof(Body).GetMethod("CompleteGetPlacementWithResult", BindingFlags.Instance | BindingFlags.NonPublic)!;
		if (scenario is "delete-throw" or "refill-empty-throw")
		{
			var error = Assert.ThrowsException<TargetInvocationException>(() => completion.Invoke(body, arguments));
			Assert.AreSame(expected, error.InnerException, error.InnerException?.ToString());
		}
		else Assert.IsTrue((bool)completion.Invoke(body, arguments)!);

		// Reflection copies out parameters only on successful completion.
		if (scenario is not ("delete-throw" or "refill-empty-throw")) Assert.AreSame(survivor, arguments[2]);
		Assert.AreEqual(8, survivor.Quantity);
		Assert.AreEqual(scenario == "refill" ? 2 : 0, source.Quantity);
		Assert.AreEqual(scenario == "ammo", source.Deleted);
		Assert.IsNull(source.GetItemType<IHoldable>()!.HeldBy);
		if (scenario == "refill")
		{
			CollectionAssert.AreEqual(new IGameItem[] { source }, floorItems);
			Assert.AreSame(floor.Object, source.DirectLocation);
			Assert.AreEqual(ItemGetResponse.CanGet, source.CanGet(0));
		}
		else
		{
			Assert.AreEqual(0, floorItems.Count);
			Assert.IsNull(source.DirectLocation);
			Assert.IsFalse(survivor.CanMerge(source));
			Assert.AreNotEqual(ItemGetResponse.CanGet, source.CanGet(0));
			Assert.AreEqual(0, MudSharp.Character.Character.IncludeTargetProjections([source]).Count());
		}
		if (scenario is "foreign-component" or "callback-component") Assert.IsTrue(source.Components.Contains(foreign));
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("refill")]
	[DataRow("relocate")]
	[DataRow("owner")]
	[DataRow("throw")]
	[DataRow("description-refill")]
	[DataRow("foreign-component")]
	public void CommittedGetStackMerge_ObserversSeeConservedValueAndCleanupPreservesMutation(string scenario)
	{
		var (survivor, source, holder) = Pair(5, 3);
		var a = survivor.GetItemType<StackableGameItemComponent>();
		var b = source.GetItemType<StackableGameItemComponent>();
		var descriptions = 0;
		a.DescriptionUpdate += (_, _) =>
		{
			++descriptions;
			Assert.AreEqual(8, a.Quantity);
			Assert.AreEqual(0, b.Quantity);
			if (scenario == "description-refill") b.Quantity = 2;
			if (scenario == "foreign-component")
				((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(source)!).Add(Mock.Of<IGameItemComponent>());
		};
		var deletions = 0;
		source.OnDeleted += _ =>
		{
			++deletions;
			Assert.AreEqual(8, a.Quantity);
			Assert.AreEqual(0, b.Quantity);
			if (scenario == "refill") b.Quantity = 2;
			if (scenario == "relocate") typeof(GameItem).GetProperty(nameof(GameItem.Location))!.SetValue(source, Mock.Of<IRoom>());
			if (scenario == "owner") source.SetOwner(Mock.Of<IFrameworkItem>(x => x.Id == 91 && x.FrameworkItemType == "Organisation"));
			if (scenario == "throw") throw new InvalidOperationException("fixture observer");
		};
		var call = typeof(GameItem).GetMethod("MergeCommittedStackForGet", BindingFlags.Instance | BindingFlags.NonPublic)!;
		if (scenario == "throw")
		{
			var error = Assert.ThrowsException<TargetInvocationException>(() => call.Invoke(survivor, [source, holder]));
			Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
		}
		else call.Invoke(survivor, [source, holder]);
		Assert.AreEqual(8, survivor.Quantity);
		Assert.AreEqual(scenario is "refill" or "description-refill" ? 2 : 0, source.Quantity);
		Assert.AreEqual(1, descriptions);
		Assert.AreEqual(scenario is "description-refill" or "foreign-component" ? 0 : 1, deletions);
		Assert.AreEqual(scenario == "valid", source.Deleted);
		Assert.IsFalse(survivor.Deleted);
		Assert.AreSame(holder, survivor.GetItemType<IHoldable>()!.HeldBy);
		if (scenario == "relocate") Assert.IsNotNull(source.DirectLocation);
		if (scenario == "owner") Assert.AreEqual(new ItemOwnershipReference("Organisation", 91), source.OwnershipReference);
	}

	[TestMethod]
	public void CommittedGetStackMerge_OverflowRefusesBeforeEitherQuantityChanges()
	{
		var (survivor, source, holder) = Pair(int.MaxValue, 1);
		var error = Assert.ThrowsException<TargetInvocationException>(() =>
			typeof(GameItem).GetMethod("MergeCommittedStackForGet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(survivor, [source, holder]));
		Assert.IsInstanceOfType(error.InnerException, typeof(OverflowException));
		Assert.AreEqual(int.MaxValue, survivor.Quantity);
		Assert.AreEqual(1, source.Quantity);
		Assert.IsFalse(source.Deleted);
	}

	[DataTestMethod]
	[DataRow("load", "valid")]
	[DataRow("load", "refill")]
	[DataRow("load", "foreign")]
	[DataRow("unload", "valid")]
	[DataRow("unload", "refill")]
	[DataRow("unload", "foreign")]
	[DataRow("load", "active")]
	[DataRow("unload", "active")]
	public void AmmoMerge_ConservesNativeLeafAndRefusesActiveProjectile(string operation, string mutation)
	{
		var (survivor, source, holder) = Pair(5, 3);
		void AddAmmo(GameItem item)
		{
			((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!)
				.Add(new AmmunitionGameItemComponent((AmmunitionGameItemComponentProto)null!, item, temporary: true));
		}
		AddAmmo(survivor); AddAmmo(source);
		if (operation == "load") source.GetItemType<IHoldable>()!.HeldBy = null;
		if (mutation == "active") typeof(AmmunitionGameItemComponent).GetField("_currentFireContext", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(source.GetItemType<AmmunitionGameItemComponent>(), new RangedFireContext());
		var deletes = 0;
		source.OnDeleted += _ =>
		{
			++deletes; Assert.AreEqual(0, source.Quantity); Assert.AreEqual(8, survivor.Quantity);
			if (mutation == "refill") source.GetItemType<IStackable>()!.Quantity = 2;
		};
		if (mutation == "foreign") ((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(source)!).Add(Mock.Of<IGameItemComponent>());
		var method = typeof(GameItem).GetMethod(operation == "load" ? "MergeCommittedAmmoStackForLoad" : "MergeCommittedAmmoStackForGet", BindingFlags.Instance | BindingFlags.NonPublic)!;
		if (mutation == "active") Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(survivor, [source, holder]));
		else method.Invoke(survivor, [source, holder]);
		Assert.AreEqual(mutation == "active" ? 5 : 8, survivor.Quantity);
		Assert.AreEqual(mutation == "active" ? 3 : mutation == "refill" ? 2 : 0, source.Quantity);
		Assert.AreEqual(mutation == "active" || mutation == "foreign" ? 0 : 1, deletes);
		Assert.AreEqual(mutation == "valid", source.Deleted);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("description-throw")]
	[DataRow("add-throw")]
	[DataRow("reject")]
	public void PreparedAmmoSplit_BindsAndDebitsBeforeCopyObservers(string scenario)
	{
		var (_, source, holder) = Pair(5, 3);
		var world = Mock.Get(source.Gameworld);
		Mock.Get(source.Prototype).SetupGet(x => x.Morphs).Returns(true);
		var morph = typeof(GameItem).GetProperty("CachedMorphTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
		morph.SetValue(source, TimeSpan.FromMinutes(2));
		((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(source)!)
			.Add(new AmmunitionGameItemComponent((AmmunitionGameItemComponentProto)null!, source, temporary: true));
		var expected = new InvalidOperationException("split observer"); GameItem? captured = null; var adds = 0; var guards = 0;
		world.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item =>
		{
			++adds; Assert.AreSame(captured, item); Assert.AreEqual(1, source.Quantity); Assert.AreEqual(2, item.Quantity);
			if (scenario == "add-throw") throw expected;
			if (scenario == "description-throw") item.GetItemType<StackableGameItemComponent>()!.DescriptionUpdate += (_, _) => throw expected;
		});
		var method = typeof(StackableGameItemComponent).GetMethod("SplitPrepared", BindingFlags.Instance | BindingFlags.NonPublic)!;
		object? Invoke() => method.Invoke(source.GetItemType<StackableGameItemComponent>(), [2, (Func<bool>)(() => ++guards != 2 || scenario != "reject"), (Action<GameItem>)(copy => captured = copy)]);
		if (scenario == "valid") { var result = Invoke(); Assert.AreSame(captured, result); }
		else
		{
			var error = Assert.ThrowsException<TargetInvocationException>(() => Invoke());
			if (scenario != "reject") Assert.AreSame(expected, error.InnerException);
		}
		Assert.AreEqual(scenario == "reject" ? 3 : 1, source.Quantity);
		Assert.AreEqual(scenario == "reject" ? 0 : 1, adds);
		Assert.AreSame(holder, source.GetItemType<IHoldable>()!.HeldBy);
		if (captured is not null)
		{
			Assert.AreEqual(2, captured.Quantity); Assert.IsNull(captured.GetItemType<IHoldable>()!.HeldBy);
			Assert.IsNull(captured.DirectLocation); Assert.AreEqual(source.OwnershipReference, captured.OwnershipReference);
			Assert.AreEqual(morph.GetValue(source), morph.GetValue(captured), "Prepared copy preserves the native cached morph timer even when Add/description observers throw.");
		}
	}

	private static (GameItem Survivor, GameItem Source, IBody Holder) Pair(int first, int second)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		proto.SetupGet(x => x.Gameworld).Returns(world.Object);
		proto.SetupGet(x => x.Components).Returns([]);
		proto.SetupGet(x => x.Name).Returns("fixture goods");
		GameItem Item(int quantity)
		{
			var item = new GameItem(proto.Object);
			var components = (List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!;
			var stack = new StackableGameItemComponent((StackableGameItemComponentProto)null!, item, temporary: true) { Quantity = quantity };
			components.Add(stack);
			components.Add(new HoldableGameItemComponent((HoldableGameItemComponentProto)null!, item, temporary: true));
			return item;
		}
		var a = Item(first); var b = Item(second);
		var holder = new Mock<IBody>();
		holder.SetupGet(x => x.HeldOrWieldedItems).Returns([a]);
		a.GetItemType<IHoldable>()!.HeldBy = holder.Object;
		b.GetItemType<IHoldable>()!.HeldBy = holder.Object;
		return (a, b, holder.Object);
	}
}
