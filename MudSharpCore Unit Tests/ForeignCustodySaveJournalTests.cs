#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Magic;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ForeignCustodySaveJournalTests
{
	private static void Raw(object target, string name, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
		{
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is not { } field) continue;
			field.SetValue(target, value); return;
		}
		throw new InvalidOperationException("Missing native fixture field: " + name);
	}

	private static int NeedsCount(RuntimeBody body) =>
		(int)typeof(RuntimeBody).GetField("_needsChangedCount", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(body)!;

	private static (GameItem Item, IMagicResource Resource, Mock<ISaveManager> Saves) ResourceItem()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var saves = new Mock<ISaveManager>(); world.SetupGet(x => x.SaveManager).Returns(saves.Object);
		var item = new GameItem(Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object));
		Raw(item, "_id", 700L); Raw(item, "IdInitialised", true);
		var resource = new Mock<IMagicResource>(); resource.SetupGet(x => x.Id).Returns(901L);
		resource.Setup(x => x.ResourceCap(item)).Returns(100);
		item.AddResource(resource.Object, 80);
		return (item, resource.Object, saves);
	}

	[TestMethod]
	public void SaveMagic_RepeatedCallbackDebits_RearmsCurrentBalanceEvenWhenChangedAlreadyTrue()
	{
		var f = ResourceItem(); var row = new MudSharp.Models.GameItem { Id = f.Item.Id };
		row.GameItemsMagicResources.Add(new() { GameItemId = f.Item.Id, MagicResourceId = f.Resource.Id, Amount = 80 });
		using var transfer = ForeignCustodyTransferContext.Enter(Mock.Of<MudSharp.Body.IBody>(), [f.Item], Mock.Of<ICell>());
		Assert.IsTrue(f.Item.UseResource(f.Resource, 13)); f.Item.SaveMagic(row);
		Assert.IsFalse(f.Item.ResourcesChanged);
		Assert.IsTrue(f.Item.UseResource(f.Resource, 7)); f.Item.SaveMagic(row);
		Assert.IsFalse(f.Item.ResourcesChanged); Assert.IsTrue(f.Item.Changed);
		f.Saves.Invocations.Clear();
		transfer.RestorePendingSaves(action => action());
		Assert.AreEqual(60, f.Item.MagicResourceAmounts[f.Resource]);
		Assert.IsTrue(f.Item.ResourcesChanged); Assert.IsTrue(f.Item.Changed);
		f.Saves.Verify(x => x.Add(f.Item), Times.AtLeastOnce);
		f.Saves.Verify(x => x.Abort(It.IsAny<ISaveable>()), Times.Never);
	}

	[TestMethod]
	public void SaveMagic_WrongScopedRow_RefusesBeforeClearingFlagsOrChangingAmounts()
	{
		var f = ResourceItem(); var row = new MudSharp.Models.GameItem { Id = f.Item.Id + 1 };
		row.GameItemsMagicResources.Add(new() { GameItemId = row.Id, MagicResourceId = f.Resource.Id, Amount = 80 });
		Assert.IsTrue(f.Item.UseResource(f.Resource, 13));
		using var transfer = ForeignCustodyTransferContext.Enter(Mock.Of<MudSharp.Body.IBody>(), [f.Item], Mock.Of<ICell>());
		Assert.ThrowsException<InvalidOperationException>(() => f.Item.SaveMagic(row));
		Assert.AreEqual(80, row.GameItemsMagicResources.Single().Amount);
		Assert.IsTrue(f.Item.ResourcesChanged);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NeedsSave_Rollback_RestoresConsumedCounterAndPreservesLaterPendingProgress(bool laterPending)
	{
		var world = new Mock<IFuturemud>(); var saves = new SaveManager(); world.SetupGet(x => x.SaveManager).Returns(saves);
		var body = (RuntimeBody)RuntimeHelpers.GetUninitializedObject(typeof(RuntimeBody));
		typeof(PerceivedItem).GetProperty("Gameworld")!.SetValue(body, world.Object);
		Raw(body, "_needsChanged", true); Raw(body, "_needsChangedCount", 4);
		using var transfer = ForeignCustodyTransferContext.Enter(body, [], Mock.Of<ICell>());
		ForeignCustodyTransferContext.RecordSave(body); ForeignCustodyTransferContext.RecordNeedsSave(body);
		body.NeedsChanged = false;
		Assert.AreEqual(0, NeedsCount(body));
		if (laterPending) body.NeedsChanged = true;
		var expected = laterPending ? NeedsCount(body) : 4;
		transfer.RestorePendingSaves(action => action());
		Assert.IsTrue(body.NeedsChanged); Assert.AreEqual(expected, NeedsCount(body));
		Assert.IsTrue(body.Changed); Assert.IsTrue(saves.IsQueued(body));
	}

	[TestMethod]
	public void SaveQueue_ActiveCustodyScope_RefusesBeforeDrainingUnrelatedSavesOrInitialisers()
	{
		var saves = new SaveManager(); var sentinel = new Mock<ISaveable>(); var initialiser = new Mock<ILateInitialisingItem>();
		saves.Add(sentinel.Object); saves.AddInitialisation(initialiser.Object);
		using var transfer = ForeignCustodyTransferContext.Enter(Mock.Of<MudSharp.Body.IBody>(), [], Mock.Of<ICell>());
		Assert.ThrowsException<InvalidOperationException>(saves.Flush);
		Assert.ThrowsException<InvalidOperationException>(() => saves.DirectInitialise(initialiser.Object));
		Assert.ThrowsException<InvalidOperationException>(() => saves.FlushLazyLoad(TimeSpan.FromSeconds(1)));
		Assert.IsFalse(saves.Flushing); Assert.IsTrue(saves.IsQueued(sentinel.Object)); Assert.IsTrue(saves.IsQueued(initialiser.Object));
		sentinel.Verify(x => x.Save(), Times.Never); initialiser.Verify(x => x.InitialiseItem(), Times.Never);
	}
}
