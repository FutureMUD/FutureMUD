#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Planes;
using NativeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SelectivePlanarInventoryTests
{
	private static MudSharp.Models.GameItemComponentProto Row(string type, string xml = "<Definition/>") => new()
	{
		Id = 41, Type = type, Name = type, Definition = xml,
		EditableItem = new() { RevisionStatus = 2 }
	};
	private sealed class ContainerProto(IFuturemud world) : ContainerGameItemComponentProto(Row("Container"), world) { }
	private sealed class WearableProto(IFuturemud world) : WearableGameItemComponentProto(Row("Wearable"), world) { }
	private sealed class CallbackContainer(ContainerGameItemComponentProto proto, IGameItem parent) : ContainerGameItemComponent(proto, parent, temporary: true)
	{
		public Action<IGameItem>? AfterTake;
		public override IGameItem Take(ICharacter actor, IGameItem item, int quantity)
		{
			var result = base.Take(actor, item, quantity);
			AfterTake?.Invoke(result);
			return result;
		}
	}
	private sealed class CallbackWearable(WearableGameItemComponentProto proto, IGameItem parent) : WearableGameItemComponent(proto, parent, null!, temporary: true)
	{
		public Action<IBody?>? AfterWear;
		public override void UpdateWear(IBody body, IWearProfile profile)
		{
			base.UpdateWear(body, profile); AfterWear?.Invoke(body);
		}
	}
	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> OtherActor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoom> Origin { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoom> Elsewhere { get; } = new() { DefaultValue = DefaultValue.Mock };
		public List<IGameItem> Floor { get; } = [];
		public List<IGameItem> Items { get; } = [];
		public List<ICharacter> Observers { get; } = [];
		public NativeBody Body { get; }
		public NativeBody OtherBody { get; }
		public IRoom CurrentRoom;
		public RoomLayer Layer = RoomLayer.GroundLevel;
		private readonly IGrab _hand = Mock.Of<IGrab>();
		private readonly IWield _wield = Mock.Of<IWield>();
		private readonly IWield _secondWield = Mock.Of<IWield>();
		private readonly IWear _wear = Mock.Of<IWear>();
		private readonly IWearlocProfile _wearloc = Mock.Of<IWearlocProfile>(x => x.PreventsRemoval);
		private readonly IWearProfile _profile;
		public Fixture()
		{
			CurrentRoom = Origin.Object;
			World.SetupGet(x => x.MagicCasting).Returns((IMagicCastingService)null!);
			World.SetupGet(x => x.ProximityEventService).Returns(() => null!);
			World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => Items));
			var prime = Mock.Of<IPlane>(x => x.Id == 1 && x.Name == "Material");
			var shadow = Mock.Of<IPlane>(x => x.Id == 9 && x.Name == "Shadow");
			World.SetupGet(x => x.DefaultPlane).Returns(prime);
			World.SetupGet(x => x.Planes).Returns(MagicCastingFixture.Collection(() => new[] { prime, shadow }));
			Origin.SetupGet(x => x.Gameworld).Returns(World.Object);
			Origin.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!);
			Elsewhere.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!);
			Origin.SetupGet(x => x.GameItems).Returns(Floor);
			Origin.SetupGet(x => x.Characters).Returns(Observers);
			Origin.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>())).Callback<IGameItem, bool>((item, _) => Floor.Add(item));
			Origin.Setup(x => x.Extract(It.IsAny<IGameItem>())).Callback<IGameItem>(item => Floor.RemoveAll(x => ReferenceEquals(x, item)));
			Actor.SetupGet(x => x.Location).Returns(() => CurrentRoom);
			Actor.SetupGet(x => x.RoomLayer).Returns(() => Layer);
			Actor.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(CurrentRoom, Layer, null));
			OtherActor.SetupGet(x => x.Location).Returns(Origin.Object);
			Body = NewBody(Actor); OtherBody = NewBody(OtherActor);
			var profile = new Mock<IWearProfile>();
			profile.SetupGet(x => x.AllProfiles).Returns(new Dictionary<IWear, IWearlocProfile> { [_wear] = _wearloc });
			_profile = profile.Object;
		}
		private NativeBody NewBody(Mock<ICharacter> actor)
		{
			var body = TestObjectFactory.CreateUninitialized<NativeBody>();
			body.SetNoSave(true); body.Actor = actor.Object;
			typeof(PerceivedItem).GetField("_positionState", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, PositionUndefined.Instance);
			actor.SetupGet(x => x.Body).Returns(body);
			actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			actor.SetupGet(x => x.PositionState).Returns(PositionUndefined.Instance);
			typeof(NativeBody).GetProperty(nameof(NativeBody.Gameworld))!.SetValue(body, World.Object);
			typeof(NativeBody).GetProperty(nameof(NativeBody.Prototype))!.SetValue(body,
				Mock.Of<IBodyPrototype>(x => x.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
			typeof(PerceivedItem).GetProperty(nameof(NativeBody.EffectHandler))!.SetValue(body, new EffectHandler(body));
			foreach (var name in new[] { "_prosthetics", "_wounds", "_severedRoots", "_wieldedItems", "_heldItems", "_wornItems", "_implants", "_bodyparts", "_organs", "_bones", "_holdlocs", "_carriedItems" })
			{
				var field = Field(name); field.SetValue(body, Activator.CreateInstance(field.FieldType));
			}
			Field("_bodyparts").SetValue(body, new HashSet<IBodypart> { _hand, _wield, _secondWield, _wear });
			var handler = new EffectHandler(actor.Object);
			actor.SetupGet(x => x.Effects).Returns(() => handler.Effects);
			actor.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(null)).Returns(() => handler.Effects.OfType<IPlanarOverlayEffect>());
			actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(handler.AddEffect);
			actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(handler.RemoveEffect);
			body.RecalculateItemHelpers();
			return body;
		}
		public GameItem Item(string name, bool ready = false, bool visible = false)
		{
			var proto = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
			proto.SetupGet(x => x.Gameworld).Returns(World.Object);
			proto.SetupGet(x => x.Name).Returns(name);
			proto.SetupGet(x => x.ShortDescription).Returns(name);
			proto.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			proto.SetupGet(x => x.Components).Returns([]);
			var item = new GameItem(proto.Object); item.SetNoSave(true);
			Components(item).Add(new HoldableGameItemComponent((HoldableGameItemComponentProto)null!, item, temporary: true));
			if (ready || visible) item.AddEffect(new EtherealItemAffinityEffect(item, 9, ready, visible));
			Items.Add(item); return item;
		}
		public CallbackContainer Bag(GameItem item)
		{
			var component = new CallbackContainer(new ContainerProto(World.Object), item);
			Components(item).Add(component); return component;
		}
		public CallbackWearable Wearable(GameItem item)
		{
			var component = new CallbackWearable(new WearableProto(World.Object), item);
			Components(item).Add(component); return component;
		}
		public static void DisintegratesOnTransition(GameItem item)
		{
			var proto = TestObjectFactory.CreateUninitialized<TransientItemGameItemComponentProto>();
			typeof(TransientItemGameItemComponentProto).GetProperty(nameof(proto.Triggers))!.SetValue(proto, ItemRetirementTrigger.EtherealTransition);
			Components(item).Add(new TransientItemGameItemComponent(proto, item, temporary: true));
		}
		public void Equip(GameItem item, string state, NativeBody? body = null)
		{
			body ??= Body;
			item.Get(body);
			switch (state)
			{
				case "held":
					((List<Tuple<IGameItem, IGrab>>)Field("_heldItems").GetValue(body)!).Add(Tuple.Create((IGameItem)item, _hand));
					break;
				case "wielded":
				case "two-handed":
					var wield = item.GetItemType<IWieldable>();
					if (wield is null) { wield = new WieldablePropGameItemComponent((WieldablePropGameItemComponentProto)null!, item, temporary: true); Components(item).Add(wield); }
					wield.PrimaryWieldedLocation = _wield;
					((List<Tuple<IGameItem, IWield>>)Field("_wieldedItems").GetValue(body)!).Add(Tuple.Create((IGameItem)item, _wield));
					if (state == "two-handed")
						((List<Tuple<IGameItem, IWield>>)Field("_wieldedItems").GetValue(body)!).Add(Tuple.Create((IGameItem)item, _secondWield));
					break;
				case "worn":
					var wearable = item.GetItemType<IWearable>();
					if (wearable is null) { wearable = new WearableGameItemComponent(new WearableProto(World.Object), item, body, temporary: true); Components(item).Add(wearable); }
					wearable.UpdateWear(body, _profile);
					((List<(IGameItem, IWear, IWearlocProfile)>)Field("_wornItems").GetValue(body)!).Add((item, _wear, _wearloc));
					break;
			}
			body.RecalculateItemHelpers();
		}
		public void Transfer(GameItem item)
		{
			Body.Take(item); Equip(item, "held", OtherBody);
		}
		public static PlanarPresenceDefinition Shadow(bool retain = true) => new([9], [9], [9],
			Enum.GetValues<PlanarInteractionKind>().ToDictionary(x => x, _ => (IEnumerable<long>)new long[] { 9 }),
			true, false, false, false, true, false, false, "noncorporeal", retain);
		public IEffect Enter(bool spell = false, bool retain = true)
		{
			IEffect effect = spell ? new SpellPlanarStateEffect(Actor.Object, Mock.Of<IMagicSpellEffectParent>(), Shadow(retain)) :
				new PlanarStateEffect(Actor.Object, Shadow(retain));
			Actor.Object.AddEffect(effect); return effect;
		}
		public void AssertFloor(GameItem item)
		{
			Assert.AreSame(Origin.Object, item.Location, item.Name);
			Assert.IsNull(item.ContainedIn, item.Name); Assert.IsNull(item.InInventoryOf, item.Name);
			Assert.AreEqual(1, Floor.Count(x => ReferenceEquals(x, item)), item.Name);
			Assert.IsFalse(Body.DirectItems.Any(x => ReferenceEquals(x, item)), item.Name);
		}
		public static List<IGameItemComponent> Components(GameItem item) =>
			(List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!;
		private static FieldInfo Field(string name) => typeof(NativeBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
	}

	[TestMethod]
	[DataRow("held", true)][DataRow("wielded", true)][DataRow("worn", true)]
	[DataRow("held", false)][DataRow("wielded", false)][DataRow("worn", false)]
	[DataRow("two-handed", true)][DataRow("two-handed", false)]
	public void SelectiveTransition_NativeEquipment_PreservesOnlyReadyRoots(string state, bool ready)
	{
		var f = new Fixture(); var item = f.Item("equipment", ready); f.Equip(item, state);
		var shadow = f.Enter();
		if (ready)
		{
			Assert.AreSame(f.Body, item.InInventoryOf);
			Assert.IsTrue(f.Body.DirectItems.Contains(item)); Assert.AreEqual(0, f.Floor.Count);
			Assert.IsTrue(item.EffectsOfType<EtherealItemAffinityEffect>().Single().IsVisibleToThatPlane);
			if (state is "wielded" or "two-handed") Assert.IsNotNull(item.GetItemType<IWieldable>().PrimaryWieldedLocation);
			if (state == "worn") Assert.AreSame(f.Body, item.GetItemType<IWearable>().WornBy);
			f.Actor.Object.RemoveEffect(shadow, true);
			var affinity = item.EffectsOfType<EtherealItemAffinityEffect>().Single();
			Assert.IsFalse(affinity.IsVisibleToThatPlane); Assert.IsTrue(affinity.IsReadyForTransition);
			Assert.AreSame(f.Body, item.InInventoryOf);
		}
		else
		{
			f.AssertFloor(item);
			if (state is "wielded" or "two-handed") Assert.IsNull(item.GetItemType<IWieldable>().PrimaryWieldedLocation);
			if (state == "worn") Assert.IsNull(item.GetItemType<IWearable>().WornBy);
		}
	}

	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void SelectiveTransition_NestedNativeContainers_KeepReadyChildrenInTheirImmediateParent(bool readyRoot)
	{
		var f = new Fixture();
		var root = f.Item("root bag", readyRoot); var rootBag = f.Bag(root);
		var readyBagItem = f.Item("ready inner bag", true); var readyBag = f.Bag(readyBagItem);
		var plainBagItem = f.Item("plain inner bag"); var plainBag = f.Bag(plainBagItem);
		var readyInsideReady = f.Item("ready in ready", true); var plainInsideReady = f.Item("plain in ready");
		var readyInsidePlain = f.Item("ready in plain", true); var plainInsidePlain = f.Item("plain in plain");
		readyBag.Put(null, readyInsideReady, false); readyBag.Put(null, plainInsideReady, false);
		plainBag.Put(null, readyInsidePlain, false); plainBag.Put(null, plainInsidePlain, false);
		rootBag.Put(null, readyBagItem, false); rootBag.Put(null, plainBagItem, false);
		f.Equip(root, "held"); f.Enter();
		Assert.AreSame(root, readyBagItem.ContainedIn); Assert.AreSame(readyBagItem, readyInsideReady.ContainedIn);
		Assert.AreSame(plainBagItem, readyInsidePlain.ContainedIn);
		f.AssertFloor(plainBagItem); f.AssertFloor(plainInsideReady); f.AssertFloor(plainInsidePlain);
		if (readyRoot) Assert.AreSame(f.Body, root.InInventoryOf); else f.AssertFloor(root);
		Assert.AreEqual(readyRoot ? 3 : 4, f.Floor.Count);
	}

	[TestMethod]
	[DataRow(false)][DataRow(true)]
	public void DefaultTransition_NativeBagContents_AreNotRecursivelyFiltered(bool readyRoot)
	{
		var f = new Fixture(); var root = f.Item("ordinary bag", readyRoot); var bag = f.Bag(root);
		var child = f.Item("ordinary child"); bag.Put(null, child, false); f.Equip(root, "held");
		f.Enter(retain: false);
		f.AssertFloor(root); Assert.AreSame(root, child.ContainedIn); Assert.AreEqual(1, f.Floor.Count);
	}

	[TestMethod]
	[DataRow("held", false)][DataRow("held", true)]
	[DataRow("wielded", false)][DataRow("wielded", true)]
	[DataRow("worn", false)][DataRow("worn", true)]
	public void SelectiveTransition_NativeDisintegratePolicy_RetiresCarriedItemsAndExemptsWornOrWielded(string state, bool ready)
	{
		var f = new Fixture(); var item = f.Item("transient equipment", ready);
		Fixture.DisintegratesOnTransition(item);
		f.Equip(item, state); f.Enter();
		Assert.AreEqual(state == "held", item.Deleted);
		if (state == "held") { Assert.IsFalse(f.Body.DirectItems.Contains(item)); Assert.AreEqual(0, f.Floor.Count); }
		else if (ready) Assert.AreSame(f.Body, item.InInventoryOf);
		else f.AssertFloor(item);
	}

	[TestMethod]
	[DataRow(false, false)][DataRow(false, true)][DataRow(true, false)][DataRow(true, true)]
	public void SightOnlyOverlay_AddAndRemoval_PreserveVisibilityUntilActualShadowReturn(bool spell, bool bodyOverlay)
	{
		var f = new Fixture(); var item = f.Item("ready equipment", true); f.Equip(item, "held");
		var shadow = f.Enter(spell); var affinity = item.EffectsOfType<EtherealItemAffinityEffect>().Single();
		var definition = new PlanarPresenceDefinition([], [], [17], new Dictionary<PlanarInteractionKind, IEnumerable<long>>(),
			false, true, false, false, false, false, false, "");
		IPerceivable owner = bodyOverlay ? f.Body : f.Actor.Object;
		var sight = new PlanarStateEffect(owner, definition, 200, overridesBase: false);
		owner.AddEffect(sight);
		Assert.IsTrue(affinity.IsVisibleToThatPlane); Assert.AreSame(f.Body, item.InInventoryOf);
		owner.RemoveEffect(sight, true);
		Assert.IsTrue(affinity.IsVisibleToThatPlane); Assert.IsTrue(affinity.IsReadyForTransition);
		f.Actor.Object.RemoveEffect(shadow, true);
		Assert.IsFalse(affinity.IsVisibleToThatPlane); Assert.IsTrue(affinity.IsReadyForTransition);
		Assert.AreSame(f.Body, item.InInventoryOf);
	}

	[TestMethod]
	public void DetachedNativeRoots_FirstDropMovesActor_BothDrainToCapturedOrigin()
	{
		var f = new Fixture(); var first = f.Item("first"); var second = f.Item("second");
		f.Equip(first, "held"); f.Equip(second, "held");
		var callback = new Mock<IGameItemComponent>(); var calls = 0;
		callback.Setup(x => x.Taken()).Callback(() => { ++calls; f.CurrentRoom = f.Elsewhere.Object; });
		Fixture.Components(first).Add(callback.Object);
		f.Enter();
		Assert.AreEqual(1, calls); Assert.AreSame(f.Elsewhere.Object, f.Actor.Object.Location);
		f.AssertFloor(first); f.AssertFloor(second); Assert.AreEqual(0, f.Body.DirectItems.Count());
	}

	[TestMethod]
	public void NativeContainerTake_MovesActor_DetachedChildDrainsWithoutTouchingLaterRoot()
	{
		var f = new Fixture(); var bagItem = f.Item("ready bag", true); var bag = f.Bag(bagItem);
		var child = f.Item("ejected child"); var later = f.Item("later root");
		bag.Put(null, child, false); f.Equip(bagItem, "held"); f.Equip(later, "held");
		var called = false; bag.AfterTake = _ => { called = true; f.CurrentRoom = f.Elsewhere.Object; };
		f.Enter();
		Assert.IsTrue(called); f.AssertFloor(child);
		Assert.AreSame(f.Body, later.InInventoryOf); Assert.AreSame(f.Body, bagItem.InInventoryOf);
	}

	[TestMethod]
	public void NativeDropCallback_TransfersLaterDetachedRoot_DoesNotReclaimForeignInventory()
	{
		var f = new Fixture(); var first = f.Item("first"); var second = f.Item("second");
		f.Equip(first, "held"); f.Equip(second, "held");
		var callback = new Mock<IGameItemComponent>();
		callback.Setup(x => x.Taken()).Callback(() => f.Equip(second, "held", f.OtherBody));
		Fixture.Components(first).Add(callback.Object);
		f.Enter();
		f.AssertFloor(first); Assert.AreSame(f.OtherBody, second.InInventoryOf);
		Assert.IsTrue(f.OtherBody.HeldItems.Contains(second)); Assert.IsFalse(f.Floor.Contains(second));
	}

	[TestMethod]
	public void NativeContainerTake_TransfersLaterBag_DoesNotTraverseForeignContents()
	{
		var f = new Fixture(); var first = f.Item("ready bag", true); var bag = f.Bag(first);
		var child = f.Item("ejected child"); bag.Put(null, child, false);
		var later = f.Item("later bag"); var laterBag = f.Bag(later);
		var laterChild = f.Item("foreign child"); laterBag.Put(null, laterChild, false);
		f.Equip(first, "held"); f.Equip(later, "held");
		var called = false; bag.AfterTake = _ => { called = true; f.Transfer(later); };
		f.Enter();
		Assert.IsTrue(called); f.AssertFloor(child); Assert.AreSame(f.OtherBody, later.InInventoryOf);
		Assert.AreSame(later, laterChild.ContainedIn); Assert.IsFalse(f.Floor.Contains(laterChild));
		Assert.IsFalse(f.Floor.Contains(later)); Assert.AreSame(f.Body, first.InInventoryOf);
	}

	[TestMethod]
	public void VisibilityCallback_TransfersLaterRoot_DoesNotChangeForeignAffinity()
	{
		var f = new Fixture(); var first = f.Item("first", true); var second = f.Item("second", true);
		f.Equip(first, "held"); f.Equip(second, "held");
		var observer = new Mock<ICharacter>(); var transferred = false;
		observer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns<IPerceivable, PerceiveIgnoreFlags>((target, _) =>
		{
			if (!transferred && ReferenceEquals(target, first)) { transferred = true; f.Transfer(second); }
			return false;
		});
		f.Observers.Add(observer.Object); f.Enter();
		Assert.IsTrue(transferred); Assert.AreSame(f.OtherBody, second.InInventoryOf);
		Assert.IsFalse(second.EffectsOfType<EtherealItemAffinityEffect>().Single().IsVisibleToThatPlane);
		Assert.IsFalse(f.Floor.Contains(second));
	}

	[TestMethod]
	public void VisibilityCallback_TransfersCurrentRoot_DoesNotChangeForeignAffinity()
	{
		var f = new Fixture(); var item = f.Item("current root", true); f.Equip(item, "held");
		var observer = new Mock<ICharacter>(); var transferred = false;
		observer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(() =>
		{
			if (!transferred) { transferred = true; f.Transfer(item); }
			return false;
		});
		f.Observers.Add(observer.Object); f.Enter();
		Assert.IsTrue(transferred); Assert.AreSame(f.OtherBody, item.InInventoryOf);
		Assert.IsFalse(item.EffectsOfType<EtherealItemAffinityEffect>().Single().IsVisibleToThatPlane);
		Assert.AreEqual(0, f.Floor.Count);
	}

	[TestMethod]
	public void VisibilityCallback_RemovesCurrentAffinity_DoesNotMutateDetachedEffectOrThrow()
	{
		var f = new Fixture(); var item = f.Item("root", true); f.Equip(item, "held");
		var affinity = item.EffectsOfType<EtherealItemAffinityEffect>().Single();
		var observer = new Mock<ICharacter>(); var removed = false;
		observer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(() =>
		{
			if (!removed) { removed = true; item.RemoveEffect(affinity); }
			return false;
		});
		f.Observers.Add(observer.Object); f.Enter();
		Assert.IsTrue(removed); Assert.IsFalse(affinity.IsVisibleToThatPlane);
		Assert.IsFalse(item.Effects.Contains(affinity)); Assert.AreSame(f.Body, item.InInventoryOf);
	}

	[TestMethod]
	public void VisibilityCallback_DispelsCurrentAffinity_DoesNotRearmSpentEffect()
	{
		var f = new Fixture(); var item = f.Item("root", true); f.Equip(item, "held");
		var affinity = item.EffectsOfType<EtherealItemAffinityEffect>().Single();
		var observer = new Mock<ICharacter>(); var dispelled = false;
		observer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(() =>
		{
			if (!dispelled) { dispelled = true; Assert.IsTrue(affinity.TryDispel(1)); }
			return false;
		});
		f.Observers.Add(observer.Object); f.Enter();
		Assert.IsTrue(dispelled); Assert.IsFalse(affinity.IsReadyForTransition);
		Assert.IsFalse(affinity.IsVisibleToThatPlane); Assert.IsTrue(item.Effects.Contains(affinity));
		Assert.AreSame(f.Body, item.InInventoryOf);
	}

	[TestMethod]
	public void VisibilityCallback_AcquiresTransientRoot_DoesNotRetireUncapturedInventory()
	{
		var f = new Fixture(); var first = f.Item("ready root", true); var acquired = f.Item("callback acquisition");
		Fixture.DisintegratesOnTransition(acquired); f.Equip(first, "held");
		var observer = new Mock<ICharacter>(); var called = false;
		observer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(() =>
		{
			if (!called) { called = true; f.Equip(acquired, "held"); }
			return false;
		});
		f.Observers.Add(observer.Object); f.Enter();
		Assert.IsTrue(called); Assert.IsFalse(acquired.Deleted);
		Assert.AreSame(f.Body, acquired.InInventoryOf); Assert.IsTrue(f.Body.HeldItems.Contains(acquired));
		Assert.IsFalse(f.Floor.Contains(acquired)); Assert.AreSame(f.Body, first.InInventoryOf);
	}

	[TestMethod]
	public void WearRemovalCallback_TransfersLaterRoot_DoesNotClearForeignCustody()
	{
		var f = new Fixture(); var first = f.Item("first worn"); var second = f.Item("second worn");
		var wearable = f.Wearable(first); f.Equip(first, "worn"); f.Equip(second, "worn");
		var transferred = false;
		wearable.AfterWear = body => { if (body is null && !transferred) { transferred = true; f.Transfer(second); } };
		f.Enter();
		Assert.IsTrue(transferred); f.AssertFloor(first); Assert.AreSame(f.OtherBody, second.InInventoryOf);
		Assert.IsTrue(f.OtherBody.HeldItems.Contains(second)); Assert.IsFalse(f.Floor.Contains(second));
	}

	[TestMethod]
	public void WearRemovalCallback_AcquiresNewRoot_PreservesUncapturedInventory()
	{
		var f = new Fixture(); var first = f.Item("worn"); var acquired = f.Item("callback acquisition");
		var wearable = f.Wearable(first); f.Equip(first, "worn");
		var called = false;
		wearable.AfterWear = body => { if (body is null && !called) { called = true; f.Equip(acquired, "held"); } };
		f.Enter();
		Assert.IsTrue(called); f.AssertFloor(first); Assert.AreSame(f.Body, acquired.InInventoryOf);
		Assert.IsTrue(f.Body.HeldItems.Contains(acquired)); Assert.IsFalse(f.Floor.Contains(acquired));
	}

	[TestMethod]
	public void WearRemovalCallback_ReequipsLaterRoot_PreservesNewSlotAndCustody()
	{
		var f = new Fixture(); var first = f.Item("first worn"); var second = f.Item("reequipped root");
		var wearable = f.Wearable(first); f.Equip(first, "worn"); f.Equip(second, "worn");
		var called = false;
		wearable.AfterWear = body =>
		{
			if (body is not null || called) return;
			called = true; f.Body.Take(second); f.Equip(second, "held");
		};
		f.Enter();
		Assert.IsTrue(called); f.AssertFloor(first); Assert.AreSame(f.Body, second.InInventoryOf);
		Assert.IsTrue(f.Body.HeldItems.Contains(second)); Assert.IsFalse(f.Floor.Contains(second));
	}
}
