#nullable enable

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreparedItemReachSnapshotTests
{
	[TestMethod]
	public void PrepareItemReachSnapshot_AcyclicAncestry_DetectsChangedEdge()
	{
		var child = Item(); var parent = Item();
		SetParent(child, parent);
		var unchanged = Prepare(child);
		Assert.IsNotNull(unchanged);
		Assert.IsTrue(unchanged());
		SetParent(child, null);
		Assert.IsFalse(unchanged());
	}

	[TestMethod]
	public void PrepareItemReachSnapshot_ExistingCycle_RefusesBeforeRecursiveProperties()
	{
		var child = Item(); var parent = Item();
		SetParent(child, parent); SetParent(parent, child);
		Assert.IsNull(Prepare(child));
	}

	[TestMethod]
	public void PrepareItemReachSnapshot_LaterEdgeBecomesCycle_RefusesBeforeRecursiveProperties()
	{
		var child = Item(); var parent = Item();
		SetParent(child, parent);
		var unchanged = Prepare(child);
		Assert.IsNotNull(unchanged);
		SetParent(parent, child);
		Assert.IsFalse(unchanged());
	}

	private static Func<bool>? Prepare(GameItem item) =>
		(Func<bool>?)typeof(MudSharp.Body.Implementations.Body)
			.GetMethod("PrepareItemReachSnapshot", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [item]);

	private static void SetParent(GameItem item, GameItem? parent) =>
		typeof(GameItem).GetField("_containedIn", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, parent);

	private static GameItem Item()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		proto.SetupGet(x => x.Gameworld).Returns(world.Object);
		proto.SetupGet(x => x.Components).Returns([]);
		proto.SetupGet(x => x.Name).Returns("reach fixture");
		return new GameItem(proto.Object);
	}
}
