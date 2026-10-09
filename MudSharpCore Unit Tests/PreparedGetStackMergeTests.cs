#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
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
	public void EmptyStack_GetAndMerge_RefuseWithoutEnteringCommittedMerge(int quantity)
	{
		var (survivor, source, _) = Pair(5, quantity);
		Assert.AreEqual(ItemGetResponse.NoGetEffect, source.CanGet());
		Assert.AreEqual(ItemGetResponse.NoGetEffect, source.CanGet(0));
		Assert.AreEqual(ItemGetResponse.NoGetEffect, source.CanGet(1, ItemCanGetIgnore.IgnoreCombat | ItemCanGetIgnore.IgnoreInventoryPlans));
		Assert.IsFalse(survivor.CanMerge(source));
		Assert.IsFalse(source.CanMerge(survivor));
		Assert.AreEqual(5, survivor.Quantity);
		Assert.AreEqual(quantity, source.Quantity);
	}

	[TestMethod]
	public void CommittedGetStackMerge_AdditionalComponentRemnant_CannotBePickedUpOrMergedAgain()
	{
		var (survivor, source, holder) = Pair(5, 3);
		((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(source)!)
			.Add(Mock.Of<IGameItemComponent>());
		survivor.MergeCommittedStackForGet(source, holder);
		Assert.IsFalse(source.Deleted);
		Assert.AreEqual(8, survivor.Quantity);
		Assert.AreEqual(0, source.Quantity);
		Assert.AreEqual(ItemGetResponse.NoGetEffect, source.CanGet(0));
		Assert.IsFalse(survivor.CanMerge(source));
		source.GetItemType<IStackable>()!.Quantity = 2;
		Assert.IsTrue(survivor.CanMerge(source), "A legitimate later refill remains mergeable.");
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
