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
			if (scenario == "relocate") typeof(GameItem).GetProperty(nameof(GameItem.Location))!.SetValue(source, Mock.Of<ICell>());
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
