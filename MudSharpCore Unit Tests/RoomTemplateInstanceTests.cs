#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Body;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomTemplateInstanceTests
{
	[TestMethod]
	public void InventoryDerivedLocationDoesNotCreateOrExposeAnAuthoredDwelling()
	{
		var proto = (DwellingGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(DwellingGameItemComponentProto));
		typeof(DwellingGameItemComponentProto).GetProperty(nameof(DwellingGameItemComponentProto.TemplateRoomIds))!.SetValue(proto, new long[] { 100 });
		var world = new Mock<IFuturemud>();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		parent.SetupGet(x => x.Location).Returns(new Mock<IRoom>().Object);
		parent.SetupGet(x => x.InInventoryOf).Returns(new Mock<IBody>().Object);
		var component = new DwellingGameItemComponent(proto, parent.Object, temporary: true);
		parent.Raise(x => x.OnLocationChanged += null!, parent.Object, null!);
		Assert.IsNull(component.EntranceRoom);
	}

	[TestMethod]
	public void InteriorPopulationWaitsUntilPlacementHasReturned()
	{
		var world = new Mock<IFuturemud>();
		var rooms = new All<IRoom>();
		var progs = new All<IFutureProg>();
		world.SetupGet(x => x.Rooms).Returns(rooms);
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		var scheduler = new Mock<IScheduler>();
		world.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		ISchedule? pending = null;
		scheduler.Setup(x => x.AddSchedule(It.IsAny<ISchedule>())).Callback<ISchedule>(x => pending = x);
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(10);
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		room.Setup(x => x.EffectsOfType<RoomTemplateInstance>(It.IsAny<Predicate<RoomTemplateInstance>>()))
			.Returns([new RoomTemplateInstance(room.Object, 100, false, 5)]);
		rooms.Add(room.Object);
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(5);
		progs.Add(prog.Object);
		RoomTemplateInstance.InitialiseRooms([room.Object]);
		RoomTemplateInstance.InitialiseRooms([room.Object]);
		scheduler.Verify(x => x.AddSchedule(It.IsAny<ISchedule>()), Times.Once,
			"Several placements must coalesce a shared population refresh before the scheduler fires.");
		prog.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
		Assert.IsNotNull(pending);
		pending.Fire();
		prog.Verify(x => x.Execute(It.Is<object[]>(x => x.Length == 1 && ReferenceEquals(x[0], room.Object))), Times.Once);
	}

	[TestMethod]
	public void TemplateIdentityAndInitializerSurviveEffectPersistence()
	{
		RoomTemplateInstance.InitialiseEffectType();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		var effect = new RoomTemplateInstance(room.Object, 13352, false, 123);
		var xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var restored = (RoomTemplateInstance)Effect.LoadEffect(xml, room.Object);
		Assert.AreEqual(13352L, restored.TemplateRoomId);
		Assert.AreEqual(123L, restored.OnLoadProgId);
		Assert.IsFalse(restored.IsTemplate);
		Assert.IsTrue(restored.SavingEffect);
	}

	[TestMethod]
	public void CopyingATemplatePreservesItsSourceIdentityAndDoesNotCopyTheTemplateMarker()
	{
		var world = new Mock<IFuturemud>();
		var template = new Mock<IRoom>();
		template.SetupGet(x => x.Gameworld).Returns(world.Object);
		template.SetupGet(x => x.Id).Returns(100);
		template.SetupGet(x => x.UniqueName).Returns("Vnum13352");
		template.Setup(x => x.SaveEffects()).Returns(new XElement("Effects"));
		var marker = new RoomTemplateInstance(template.Object, 100, true, 123);
		template.Setup(x => x.EffectsOfType<RoomTemplateInstance>(It.IsAny<Predicate<RoomTemplateInstance>>()))
			.Returns([marker]);
		var instance = new Mock<IRoom>();
		instance.SetupGet(x => x.Gameworld).Returns(world.Object);
		var effects = new List<IEffect>();
		instance.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		RoomTemplateInstance.CopyTemplate(template.Object, instance.Object, "Vehicle7");
		Assert.AreEqual(1, effects.Count);
		var copied = (RoomTemplateInstance)effects[0];
		Assert.AreEqual(100L, copied.TemplateRoomId);
		Assert.AreEqual(123L, copied.OnLoadProgId);
		Assert.IsFalse(copied.IsTemplate);
		instance.Verify(x => x.TrySetUniqueName("Vnum13352_Vehicle7", out It.Ref<string>.IsAny), Times.Once);
	}
}
