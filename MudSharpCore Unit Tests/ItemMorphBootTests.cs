#nullable enable
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

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

	[DataTestMethod]
	[DataRow(1e-300)]
	[DataRow(1e-7)]
	[DataRow(double.NaN)]
	[DataRow(double.PositiveInfinity)]
	public void StartMorphTimer_UnrepresentableRefrigeratedDeadline_PreservesProgressUntilEnvironmentChanges(double rate)
	{
		var clock = new Clock();
		using var scope = RuntimeClock.Push(clock);
		var scheduler = new Scheduler(clock);
		var world = Mock.Of<IFuturemud>(x => x.Scheduler == scheduler);
		var item = (ColdItem)RuntimeHelpers.GetUninitializedObject(typeof(ColdItem));
		item.Configure(world, Mock.Of<IGameItemProto>(x => x.RefrigerationSensitive));
		var modifier = new Mock<IItemTimeRateModifier>();
		modifier.Setup(x => x.RateMultiplierFor(ItemTimeRateType.Morph)).Returns(rate);
		var container = Mock.Of<IGameItem>(x => x.Components == new IGameItemComponent[] { modifier.Object });
		typeof(GameItem).GetField("_containedIn", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.SetValue(item, container);
		var remaining = TimeSpan.FromDays(1);
		item.CachedMorphTime = remaining;
		item.StartMorphTimer();
		Assert.AreEqual(remaining, item.CachedMorphTime);
		Assert.AreEqual(DateTime.MinValue, item.MorphTime);
		Assert.IsNull(scheduler.NextTriggerUtc);
		modifier.Setup(x => x.RateMultiplierFor(ItemTimeRateType.Morph)).Returns(1.0);
		item.StartMorphTimer();
		Assert.IsNull(item.CachedMorphTime);
		Assert.AreEqual(clock.GetUtcNow().UtcDateTime.AddDays(1), item.MorphTime);
		Assert.AreEqual(remaining, scheduler.RemainingDuration(item, ScheduleType.Morph));
	}

	private sealed class Clock : TimeProvider
	{
		public override DateTimeOffset GetUtcNow() => new(2042, 1, 1, 0, 0, 0, TimeSpan.Zero);
	}

	[DataTestMethod]
	[DataRow(false, "NaN")]
	[DataRow(false, "Infinity")]
	[DataRow(false, "-Infinity")]
	[DataRow(true, "NaN")]
	[DataRow(true, "Infinity")]
	[DataRow(true, "-Infinity")]
	public void RefrigerationRateBuilder_NonFiniteValue_RejectsWithoutChangingThePrototype(bool implant, string value)
	{
		GameItemComponentProto proto = implant ? TestObjectFactory.CreateUninitialized<ImplantRefrigeratorGameItemComponentProto>() :
			TestObjectFactory.CreateUninitialized<RefrigeratorGameItemComponentProto>();
		var rate = proto.GetType().GetProperty("PoweredClosedRate")!;
		rate.SetValue(proto, 0.1);
		var account = Mock.Of<IAccount>(x => x.Culture == CultureInfo.InvariantCulture);
		var actor = Mock.Of<ICharacter>(x => x.Account == account && x.OutputHandler == Mock.Of<IOutputHandler>());
		Assert.IsFalse(proto.BuildingCommand(actor, new StringStack($"poweredclosed {value}")));
		Assert.AreEqual(0.1, rate.GetValue(proto));
		Assert.IsFalse(proto.Changed);
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
