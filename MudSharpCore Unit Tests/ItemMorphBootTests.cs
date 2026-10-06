#nullable enable
using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemMorphBootTests
{
	[DataTestMethod]
	[DataRow(240)]
	[DataRow(0)]
	public void StartMorphTimer_ColdItem_PreservesDurationWithoutRenderingDescription(int seconds)
	{
		var clock = new Clock();
		using var scope = RuntimeClock.Push(clock);
		var scheduler = new Scheduler(clock);
		var world = Mock.Of<IFuturemud>(x => x.Scheduler == scheduler);
		var item = (ColdItem)RuntimeHelpers.GetUninitializedObject(typeof(ColdItem));
		item.Configure(world, Mock.Of<IGameItemProto>());
		item.CachedMorphTime = TimeSpan.FromSeconds(seconds);

		item.StartMorphTimer();

		Assert.IsNull(item.CachedMorphTime);
		Assert.AreEqual(clock.GetUtcNow().UtcDateTime.AddSeconds(seconds), item.MorphTime);
		Assert.AreEqual(seconds > 0 ? TimeSpan.FromSeconds(seconds) : TimeSpan.FromTicks(1),
			scheduler.RemainingDuration(item, ScheduleType.Morph));
		Assert.AreEqual(TimeSpan.FromSeconds(30), scheduler.RemainingDuration(item, ScheduleType.MorphSaving));
	}

	private sealed class Clock : TimeProvider
	{
		public override DateTimeOffset GetUtcNow() => new(2042, 1, 1, 0, 0, 0, TimeSpan.Zero);
	}

	private sealed class ColdItem : GameItem
	{
		private ColdItem() : base((IGameItemProto)null!) { }
		public void Configure(IFuturemud world, IGameItemProto prototype)
		{
			typeof(GameItem).GetProperty(nameof(Gameworld))!.SetValue(this, world);
			Prototype = prototype;
			_id = 42;
			IdInitialised = true;
		}
		public override string HowSeen(IPerceiver voyeur, bool proper = false,
			DescriptionType type = DescriptionType.Short, bool colour = true,
			PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) =>
			throw new AssertFailedException("Rendering this cold item would resolve its character before boot permits it.");
	}
}
