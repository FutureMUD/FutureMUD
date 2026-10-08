#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Estates;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class RemainsRuntimeBoundaryTests
{
	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public Mock<ICorpse> Corpse { get; } = new();
		public Mock<ISeveredBodypart> Part { get; } = new();
		public Mock<IRoom> Source { get; } = new();
		public Mock<IOutputHandler> Output { get; } = new();
		public Mock<INeedsModel> Needs { get; } = new();
		public RuntimeBody Eater { get; }

		public Fixture()
		{
			World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns("Enabled");
			World.Setup(x => x.UnitManager.BaseFluidToLitres).Returns(1.0);
			World.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
			World.SetupGet(x => x.Rooms).Returns(new All<IRoom>());
			World.SetupGet(x => x.Estates).Returns(new All<IEstate>());
			var items = new All<IGameItem>();
			Item.SetupGet(x => x.Id).Returns(1);
			items.Add(Item.Object);
			World.SetupGet(x => x.Items).Returns(items);
			Item.SetupGet(x => x.Gameworld).Returns(World.Object);
			Item.SetupGet(x => x.Location).Returns(Source.Object);
			Item.SetupGet(x => x.LocationLevelPerceivable).Returns(Item.Object);
			Item.SetupGet(x => x.SurfaceLiquidState).Returns(new SurfaceLiquidState(World.Object));
			var material = new Mock<ISolid>(); material.SetupGet(x => x.HeatDamagePoint).Returns(100.0);
			Item.SetupGet(x => x.Material).Returns(material.Object);
			Source.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(120.0);
			Item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Actor.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Location).Returns(Source.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Actor.SetupGet(x => x.NeedsModel).Returns(Needs.Object);
			Actor.SetupGet(x => x.Race.CanEatCorpses).Returns(true);
			Actor.Setup(x => x.Race.CanEatCorpseMaterial(It.IsAny<IMaterial>())).Returns(true);
			Actor.SetupGet(x => x.Race.BiteWeight).Returns(1.0);
			Actor.SetupGet(x => x.Race.EatCorpseEmoteText).Returns("@ eat|eats {0}$1.");
			Actor.Setup(x => x.Race.GetCorpseNeedFulfill(It.IsAny<IMaterial>(), It.IsAny<double>())).Returns(Mock.Of<INeedFulfiller>());
			Corpse.SetupGet(x => x.Parent).Returns(Item.Object);
			Corpse.SetupGet(x => x.RepresentsFinalCharacterDeath).Returns(true);
			Corpse.SetupGet(x => x.OriginalCharacter).Returns(Actor.Object);
			Item.Setup(x => x.GetItemType<ICorpse>()).Returns(Corpse.Object);
			Part.SetupGet(x => x.Parent).Returns(Item.Object);
			Eater = (RuntimeBody)RuntimeHelpers.GetUninitializedObject(typeof(RuntimeBody)); Eater.Actor = Actor.Object;
			var prototype = new Mock<IBodyPrototype>();
			prototype.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			typeof(RuntimeBody).GetProperty(nameof(RuntimeBody.Prototype))!.SetValue(Eater, prototype.Object);
			typeof(PerceivedItem).GetProperty(nameof(PerceivedItem.EffectHandler))!.SetValue(Eater, new EffectHandler(Eater));
			Actor.SetupGet(x => x.Body).Returns(Eater);
			Item.SetupGet(x => x.InInventoryOf).Returns(Eater);
			typeof(RuntimeBody).GetField("_cachedOrganFunctionsByType", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(Eater, new DoubleCounter<Type> { [typeof(EsophagusProto)] = 1.0 });
			typeof(RuntimeBody).GetField("_externalItems", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(Eater, new List<IGameItem>());
		}
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Exposure_UnresolvedCorpse_RefreshAndRegistrationRetainOrdinaryItemTracking(bool refresh)
	{
		var f = new Fixture(); var now = new DateTime(2026, 10, 3, 0, 0, 0, DateTimeKind.Utc);
		var service = new EnvironmentalExposureService(f.World.Object, () => now);
		if (refresh) service.Refresh(); else service.Track(f.Item.Object);
		Assert.AreEqual(1, service.ActiveCount);
		var remains = (ConditionalWeakTable<IBody, IGameItem>)typeof(EnvironmentalExposureService)
			.GetField("_remains", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(service)!;
		Assert.AreEqual(0, remains.Count());
	}

	[DataTestMethod]
	[DataRow(true, false)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(false, true)]
	public void Eating_UnresolvedAnatomy_RefusesEligibilityAndExecutionWithoutConsumption(bool corpse, bool execute)
	{
		var f = new Fixture();
		var result = execute
			? corpse ? f.Eater.Eat(f.Corpse.Object, 1.0, null) : f.Eater.Eat(f.Part.Object, 1.0, null)
			: corpse ? f.Eater.CanEat(f.Corpse.Object, 1.0) : f.Eater.CanEat(f.Part.Object, 1.0);
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.ErrorMessage, "original body");
		f.Needs.Verify(x => x.FulfilNeeds(It.IsAny<INeedFulfiller>(), It.IsAny<bool>()), Times.Never);
		f.Actor.Verify(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()), Times.Never);
		f.Item.Verify(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()), Times.Never);
		f.Item.Verify(x => x.Delete(), Times.Never);
		f.Corpse.VerifySet(x => x.EatenWeight = It.IsAny<double>(), Times.Never);
		f.Part.VerifySet(x => x.EatenWeight = It.IsAny<double>(), Times.Never);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void Morgue_UnresolvedOrDifferentBodyOwner_RefusesBeforeEstateRelocationAndEffects(int failure)
	{
		var f = new Fixture();
		f.Item.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		if (failure == 1)
		{
			f.Corpse.SetupGet(x => x.OriginalCharacter).Returns((ICharacter)null!);
			f.Corpse.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		}
		if (failure == 2)
		{
			ResolveCorpse(f, false); var current = new Mock<IBody>(); current.SetupGet(x => x.Id).Returns(1);
			f.Actor.SetupGet(x => x.Body).Returns(current.Object);
		}
		// Intake must not use the held-item body (the eater) as the corpse anatomy.
		var zone = new Mock<IEconomicZone>(); var storage = new Mock<IRoom>();
		zone.SetupGet(x => x.MorgueStorageRoom).Returns(storage.Object);
		var relocations = 0; var effects = 0;
		storage.Setup(x => x.Insert(f.Item.Object, true)).Callback(() => relocations++);
		f.Item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback(() => effects++);
		Exception? error = null;
		try { Assert.IsNull(MorgueService.IntakeCorpse(zone.Object, f.Item.Object)); }
		catch (Exception ex) { error = ex; }
		Assert.IsNull(error, $"Exception {error?.GetType().Name}; relocations {relocations}; effects {effects}");
		Assert.AreEqual(0, relocations); Assert.AreEqual(0, effects);
		f.World.VerifyGet(x => x.Estates, Times.Never);
		f.Source.Verify(x => x.Extract(f.Item.Object), Times.Never);
		f.Item.VerifySet(x => x.RoomLayer = It.IsAny<RoomLayer>(), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Eating_ResolvedAnatomy_StillConsumesAndDispatchesEvents(bool corpse)
	{
		var f = new Fixture(); var body = Mock.Of<IBody>();
		f.Corpse.SetupGet(x => x.Body).Returns(body); f.Part.SetupGet(x => x.OriginalBody).Returns(body);
		f.Corpse.SetupGet(x => x.RemainingEdibleWeight).Returns(2.0); f.Part.SetupGet(x => x.RemainingEdibleWeight).Returns(2.0);
		var result = corpse ? f.Eater.Eat(f.Corpse.Object, 1.0, null) : f.Eater.Eat(f.Part.Object, 1.0, null);
		Assert.IsTrue(result.Success);
		f.Needs.Verify(x => x.FulfilNeeds(It.IsAny<INeedFulfiller>(), It.IsAny<bool>()), Times.Once);
		f.Actor.Verify(x => x.HandleEvent(EventType.CharacterEat, It.IsAny<object[]>()), Times.Once);
		f.Item.Verify(x => x.HandleEvent(EventType.ItemEaten, It.IsAny<object[]>()), Times.Once);
		if (corpse) f.Corpse.VerifySet(x => x.EatenWeight = 1.0, Times.Once); else f.Part.VerifySet(x => x.EatenWeight = 1.0, Times.Once);
	}
}
