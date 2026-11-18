#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemReplacementLifecycleTests
{
	[TestMethod]
	[DataRow(false, "floor")]
	[DataRow(false, "held")]
	[DataRow(false, "contained")]
	[DataRow(false, "belt")]
	[DataRow(false, "loose chair")]
	[DataRow(false, "loose door")]
	[DataRow(false, "refused swap")]
	[DataRow(false, "redirected held")]
	[DataRow(false, "redirected belt")]
	[DataRow(false, "redirected worn")]
	[DataRow(true, "floor")]
	[DataRow(true, "held")]
	[DataRow(true, "contained")]
	[DataRow(true, "belt")]
	[DataRow(true, "loose chair")]
	[DataRow(true, "loose door")]
	[DataRow(true, "refused swap")]
	[DataRow(true, "redirected held")]
	[DataRow(true, "redirected belt")]
	[DataRow(true, "redirected worn")]
	public void Replacement_UsesOneDestinationAndPublishesBeforeLoginAndFinishedEvent(bool death, string destination)
	{
		var fixture = new Fixture();
		using var clock = RuntimeClock.Push(fixture.Clock);
		var source = fixture.Source;
		var replacement = fixture.Replacement;
		var redirected = fixture.NewItem("redirect box");
		var redirectedContents = fixture.AddContainer(redirected);
		var originalContainer = fixture.NewItem("original box");
		var originalContents = fixture.AddContainer(originalContainer);
		var beltItem = fixture.NewItem("belt");
		var belt = (IBelt)fixture.AddComponent(beltItem, "Belt", "<Definition MaximumNumberOfBeltedItems=\"2\" MaximumSize=\"6\"/>");
		fixture.Room.Object.Insert(redirected, false);
		fixture.Room.Object.Insert(originalContainer, false);
		fixture.Room.Object.Insert(beltItem, false);
		var expectedRoom = false;
		var expectedHeld = false;
		ContainerGameItemComponent? expectedContainer = null;
		IBelt? expectedBelt = null;

		switch (destination)
		{
			case "floor":
				fixture.Room.Object.Insert(source, false);
				expectedRoom = true;
				break;
			case "held":
				fixture.Hold(source);
				expectedHeld = true;
				break;
			case "contained":
				originalContents.Put(null, source, allowMerge: false);
				expectedContainer = originalContents;
				break;
			case "belt":
			case "redirected belt":
				if (destination == "redirected belt")
				{
					fixture.AddRedirect(source, () => redirectedContents.Put(null, replacement, allowMerge: false), reportsPlacement: false);
					expectedContainer = redirectedContents;
				}
				else
				{
					expectedBelt = belt;
				}

				var attachable = (IBeltable)fixture.AddComponent(source, "Beltable");
				fixture.AddComponent(replacement, "Beltable");
				belt.AddConnectedItem(attachable);
				break;
			case "loose chair":
			case "loose door":
				var type = destination == "loose chair" ? "Chair" : "Door";
				fixture.AddComponent(source, type);
				fixture.AddComponent(replacement, type);
				fixture.Room.Object.Insert(source, false);
				expectedRoom = true;
				break;
			case "refused swap":
				fixture.Hold(source, allowSwap: false);
				expectedRoom = true;
				break;
			case "redirected held":
				fixture.Hold(source);
				fixture.AddRedirect(source, () => redirectedContents.Put(null, replacement, allowMerge: false), reportsPlacement: death);
				expectedContainer = redirectedContents;
				break;
			case "redirected worn":
				var wearable = (IWearable)fixture.AddComponent(source, "Wearable");
				wearable.UpdateWear(fixture.Body.Object, null!);
				fixture.AddRedirect(source, () => redirectedContents.Put(null, replacement, allowMerge: false), reportsPlacement: false);
				expectedContainer = redirectedContents;
				break;
			default:
				Assert.Fail($"Unknown destination {destination}.");
				break;
		}

		void AssertPlacement()
		{
			Assert.AreEqual(expectedRoom, fixture.RoomItems.Contains(replacement), "Room membership");
			Assert.AreEqual(expectedRoom, ReferenceEquals(fixture.Room.Object, replacement.DirectLocation), "Direct room");
			Assert.AreEqual(expectedHeld, fixture.Hand.Contains(replacement), "Body membership");
			Assert.AreEqual(expectedHeld, ReferenceEquals(fixture.Body.Object, replacement.InInventoryOf), "Direct holder");
			Assert.AreSame(expectedContainer?.Parent, replacement.ContainedIn, "Direct container");
			Assert.AreEqual(ReferenceEquals(expectedContainer, originalContents), originalContents.Contents.Contains(replacement));
			Assert.AreEqual(ReferenceEquals(expectedContainer, redirectedContents), redirectedContents.Contents.Contains(replacement));
			Assert.AreSame(expectedBelt, replacement.GetItemType<IBeltable>()?.ConnectedTo);
			Assert.AreEqual(expectedBelt is not null, belt.ConnectedItems.Any(x => ReferenceEquals(x.Parent, replacement)));
		}

		fixture.ObservePublication(AssertPlacement);
		fixture.Replace(death);

		Assert.IsTrue(source.Deleted);
		Assert.IsNull(source.ContainedIn);
		Assert.IsNull(source.InInventoryOf);
		Assert.IsFalse(fixture.RoomItems.Contains(source) || fixture.Hand.Contains(source) || originalContents.Contents.Contains(source));
		Assert.IsFalse(belt.ConnectedItems.Any(x => ReferenceEquals(x.Parent, source)));
		AssertPlacement();
		Assert.AreSame(replacement, fixture.World.Object.TryGetItem(replacement.Id, false));
		CollectionAssert.AreEqual(new[] { "retire", "register", "login", "finished" }, fixture.Publication);
	}

	[TestMethod]
	public void Die_ReentrantDeathObserver_CreatesOneReplacement()
	{
		var fixture = new Fixture();
		fixture.Room.Object.Insert(fixture.Source, false);
		var calls = 0;
		fixture.Source.OnDeath += _ =>
		{
			if (++calls == 1) fixture.Source.Die();
		};
		fixture.ObservePublication(() => Assert.IsTrue(fixture.RoomItems.Contains(fixture.Replacement)));

		fixture.Source.Die();

		Assert.AreEqual(1, calls);
		CollectionAssert.AreEqual(new[] { "retire", "register", "login", "finished" }, fixture.Publication);
	}

	[TestMethod]
	public void Die_ObserverDeletesSource_DoesNotConstructReplacement()
	{
		var fixture = new Fixture();
		fixture.Room.Object.Insert(fixture.Source, false);
		fixture.Source.OnDeath += _ => fixture.Source.Delete();

		Assert.IsNull(fixture.Source.Die());

		Mock.Get(fixture.Source.Prototype).Verify(x => x.LoadDestroyedItem(fixture.Source), Times.Never);
		CollectionAssert.AreEqual(new[] { "retire" }, fixture.Publication);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Replacement_SuspendsDuringLogin_DoesNotRaiseFinishedEvent(bool death)
	{
		var fixture = new Fixture();
		using var clock = RuntimeClock.Push(fixture.Clock);
		fixture.Room.Object.Insert(fixture.Source, false);
		fixture.ObservePublication(() => Assert.IsTrue(fixture.RoomItems.Contains(fixture.Replacement)), fixture.Replacement.Quit);

		fixture.Replace(death);

		Assert.IsNull(fixture.World.Object.TryGetItem(fixture.Replacement.Id, false));
		CollectionAssert.AreEqual(new[] { "retire", "register", "login", "withdraw" }, fixture.Publication);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Replacement_NoResult_RetiresOriginalWithoutPublishing(bool death)
	{
		var fixture = new Fixture(result: false);
		using var clock = RuntimeClock.Push(fixture.Clock);
		fixture.Room.Object.Insert(fixture.Source, false);

		fixture.Replace(death);

		Assert.IsTrue(fixture.Source.Deleted);
		Assert.IsFalse(fixture.RoomItems.Contains(fixture.Source));
		CollectionAssert.AreEqual(new[] { "retire" }, fixture.Publication);
	}

	private sealed class Fixture
	{
		private readonly All<IGameItem> _registered = new();
		private readonly Scheduler _scheduler;
		internal Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<IRoom> Room { get; } = new();
		internal Mock<IBody> Body { get; } = new();
		internal List<IGameItem> RoomItems { get; } = [];
		internal List<IGameItem> Hand { get; } = [];
		internal List<string> Publication { get; } = [];
		internal TestClock Clock { get; } = new();
		internal TestItem Source { get; }
		internal TestItem Replacement { get; }

		internal Fixture(bool result = true)
		{
			_scheduler = new Scheduler(Clock);
			World.SetupGet(x => x.Scheduler).Returns(_scheduler);
			World.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item =>
			{
				_registered.Add(item);
				Publication.Add("register");
			});
			World.Setup(x => x.TryGetItem(It.IsAny<long>(), false)).Returns((long id, bool _) => _registered.Get(id)!);
			World.Setup(x => x.Destroy(It.IsAny<IGameItem>())).Callback<IGameItem>(item =>
			{
				_registered.RemoveExact(item);
				Publication.Add(ReferenceEquals(item, Source) ? "retire" : "withdraw");
			});
			Room.SetupGet(x => x.GameItems).Returns(RoomItems);
			Room.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>())).Callback((IGameItem item, bool _) =>
			{
				RoomItems.Add(item);
				((TestItem)item).SetRoom(Room.Object);
			});
			Room.Setup(x => x.Extract(It.IsAny<IGameItem>())).Callback<IGameItem>(item => RoomItems.Remove(item));
			var actor = new Mock<ICharacter>();
			actor.SetupGet(x => x.Location).Returns(Room.Object);
			actor.SetupGet(x => x.Body).Returns(Body.Object);
			actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(Room.Object, RoomLayer.GroundLevel));
			Body.SetupGet(x => x.Actor).Returns(actor.Object);
			Body.SetupGet(x => x.Location).Returns(Room.Object);
			Body.Setup(x => x.Take(It.IsAny<IGameItem>())).Callback<IGameItem>(item =>
			{
				if (Hand.Remove(item)) item.Get(null);
			});

			Replacement = NewItem("replacement", 101);
			Source = NewItem("source");
			AddComponent(Source, "Holdable");
			AddComponent(Replacement, "Holdable");
			Mock.Get(Source.Prototype).Setup(x => x.LoadMorphedItem(Source)).Returns(result ? Replacement : null!);
			Mock.Get(Source.Prototype).Setup(x => x.LoadDestroyedItem(Source)).Returns(result ? Replacement : null!);
		}

		internal TestItem NewItem(string name, long id = 0) => new(
			Mock.Of<IGameItemProto>(x => x.Name == name && x.Gameworld == World.Object), id);

		internal IGameItemComponent AddComponent(TestItem item, string type, string definition = "<Definition/>")
		{
			var proto = new GameItemComponentManager().GetProto(new MudSharp.Models.GameItemComponentProto
			{
				Id = 1, Type = type, Definition = definition, EditableItem = new MudSharp.Models.EditableItem()
			}, World.Object)!;
			var component = proto.CreateNew(item, temporary: true);
			item.Attach(component);
			return component;
		}

		internal ContainerGameItemComponent AddContainer(TestItem item) =>
			(ContainerGameItemComponent)AddComponent(item, "Container", "<Definition WeightLimit=\"100\" MaximumContentsSize=\"6\"/>");

		internal void Hold(TestItem item, bool allowSwap = true)
		{
			Hand.Add(item);
			item.Get(Body.Object);
			Body.Setup(x => x.SwapInPlace(item, Replacement)).Returns(() =>
			{
				if (!allowSwap || !Hand.Remove(item)) return false;
				Hand.Add(Replacement);
				Replacement.GetItemType<IHoldable>()!.HeldBy = Body.Object;
				return true;
			});
		}

		internal void AddRedirect(TestItem item, Action redirect, bool reportsPlacement)
		{
			var component = new Mock<IGameItemComponent>();
			component.Setup(x => x.HandleDieOrMorph(Replacement, It.IsAny<IRoom>(), It.IsAny<SpatialLocation?>()))
				.Returns(() => { redirect(); return reportsPlacement; });
			item.Attach(component.Object);
		}

		internal void ObservePublication(Action checkPlacement, Action? afterLogin = null)
		{
			var component = new Mock<IGameItemComponent>();
			component.Setup(x => x.Login()).Callback(() =>
			{
				Assert.AreSame(Replacement, World.Object.TryGetItem(Replacement.Id, false));
				Assert.IsTrue(Source.Deleted);
				checkPlacement();
				Publication.Add("login");
				afterLogin?.Invoke();
			});
			component.Setup(x => x.HandleEvent(EventType.ItemFinishedLoading, It.IsAny<object[]>())).Returns((EventType _, object[] args) =>
			{
				Assert.AreSame(Replacement, args.Single());
				checkPlacement();
				Publication.Add("finished");
				return false;
			});
			Replacement.Attach(component.Object);
		}

		internal void Replace(bool death)
		{
			if (death)
			{
				Source.Die();
				return;
			}
			Source.CachedMorphTime = TimeSpan.FromSeconds(1);
			Source.Login();
			Clock.Now += TimeSpan.FromSeconds(2);
			_scheduler.CheckSchedules();
		}
	}

	private sealed class TestClock : TimeProvider
	{
		internal DateTimeOffset Now { get; set; } = new(2042, 1, 1, 0, 0, 0, TimeSpan.Zero);
		public override DateTimeOffset GetUtcNow() => Now;
	}

	private sealed class TestItem : GameItem
	{
		internal TestItem(IGameItemProto proto, long id) : base(proto, null, ItemQuality.Standard, deferSpellInitialisation: true)
		{
			_id = id;
			IdInitialised = id > 0;
			Register(Mock.Of<IOutputHandler>());
		}
		internal void Attach(IGameItemComponent component) => _components.Add(component);
		internal void SetRoom(IRoom room) => SetPreparedSpatialState(new SpatialLocation(room, RoomLayer));
	}
}
