#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Body;
using RuntimeBody = MudSharp.Body.Implementations.Body;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EtherealItemMagicTests
{
	private sealed class Fixture
	{
		public MagicCastingFixture Base { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public Mock<IRoom> Room { get; } = new();
		public double Weight = 2;
		public bool Deleted;
		public Fixture()
		{
			typeof(MagicSpell).GetProperty("InvocationGrade", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(Base.Spell, 1);
			Base.Actor.SetupGet(x => x.Location).Returns(Room.Object);
			Base.Actor.Setup(x => x.CanSee(Item.Object, It.IsAny<MudSharp.Framework.PerceiveIgnoreFlags>())).Returns(true);
			Room.Setup(x => x.CanGetAccess(Item.Object, Base.Actor.Object)).Returns(true);
			Item.SetupGet(x => x.Id).Returns(902);
			Item.SetupGet(x => x.Gameworld).Returns(Base.World.Object);
			Item.SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(1));
			Item.SetupGet(x => x.TrueLocations).Returns([Room.Object]);
			Item.SetupGet(x => x.Location).Returns(Room.Object);
			Item.SetupGet(x => x.Weight).Returns(() => Weight);
			Item.SetupGet(x => x.Deleted).Returns(() => Deleted);
			Item.Setup(x => x.Delete()).Callback(() => Deleted = true);
			Item.SetupGet(x => x.Effects).Returns(() => Effects);
			Item.Setup(x => x.EffectsOfType<EtherealItemAffinityEffect>(null)).Returns(() => Effects.OfType<EtherealItemAffinityEffect>());
			Item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Item.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((e, _) => Effects.Remove(e));
			Base.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection<IGameItem>(() => [Item.Object]));
		}
		public EtherealItemAttunementEffect Load(double risk = 0) => (EtherealItemAttunementEffect)SpellEffectFactory.LoadEffect(
			new XElement("Effect", new XAttribute("type", "etherealitemattunement"), new XElement("PlaneId", 9),
				new XElement("AutoAdvance", true), new XElement("MassPerGrade", 3), new XElement("RiskPercentPerGrade", risk)), Base.Spell);
		public void Apply(EtherealItemAttunementEffect effect)
		{
			effect.CapturePreparedSelection(Base.Actor.Object, Item.Object);
			Assert.AreEqual(MagicEffectOperationStatus.Applied,
				effect.Apply(Base.Actor.Object, Item.Object, OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, null!, []).Status);
		}
	}

	[TestMethod]
	public void ItemAttunement_TwoStagesPersistAndDoNotAllowAThirdStage()
	{
		var f = new Fixture(); f.Apply(f.Load());
		var ready = f.Effects.OfType<EtherealItemAffinityEffect>().Single();
		Assert.IsTrue(ready.IsReadyForTransition); Assert.IsFalse(ready.IsVisibleToThatPlane);
		f.Apply(f.Load());
		var revealed = f.Effects.OfType<EtherealItemAffinityEffect>().Single();
		Assert.IsTrue(revealed.IsReadyForTransition); Assert.IsTrue(revealed.IsVisibleToThatPlane);
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Load().CapturePreparedSelection(f.Base.Actor.Object, f.Item.Object));
		EtherealItemAffinityEffect.InitialiseEffectType();
		var restored = (EtherealItemAffinityEffect)Effect.LoadEffect(revealed.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Item.Object);
		Assert.IsTrue(restored.IsReadyForTransition); Assert.IsTrue(restored.IsVisibleToThatPlane);
		Assert.IsTrue(restored.ClearEtherealVisibility()); Assert.IsTrue(restored.IsReadyForTransition);
		Assert.IsTrue(restored.TryDispel(1)); Assert.IsFalse(restored.IsReadyForTransition);
	}

	[TestMethod]
	public void ItemAttunement_StrictWeightLimitAndPreparedWeightFence()
	{
		var f = new Fixture { Weight = 3 }; var effect = f.Load();
		Assert.ThrowsExactly<InvalidOperationException>(() => effect.CapturePreparedSelection(f.Base.Actor.Object, f.Item.Object));
		f.Weight = 2; effect.CapturePreparedSelection(f.Base.Actor.Object, f.Item.Object); f.Weight = 2.5;
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Base.Actor.Object, f.Item.Object, out _));
		Assert.AreEqual(0, f.Effects.Count);
	}

	[TestMethod]
	public void ItemAttunement_DestructionRiskUsesNativeDeleteOnOrdinaryItems()
	{
		var f = new Fixture(); f.Apply(f.Load(100));
		Assert.IsTrue(f.Deleted); Assert.AreEqual(0, f.Effects.Count);
	}
	[TestMethod]
	public void DefaultTransition_DoesNotReclaimInventoryTransferredByRetirementCallback()
	{
		var body = TestObjectFactory.CreateUninitialized<RuntimeBody>();
		var f = new Fixture(); body.Actor = f.Base.Actor.Object; f.Base.Actor.SetupGet(x => x.Body).Returns(body);
		foreach (var name in new[] { "_heldItems", "_wieldedItems", "_wornItems", "_wornItemsOnly", "_implants", "_prosthetics", "_directItems" })
		{
			var field = typeof(RuntimeBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		var retired = new Mock<IGameItem>(); var moved = new Mock<IGameItem>(); var deleted = false;
		var otherBody = Mock.Of<IBody>(); IBody movedOwner = body;
		retired.SetupGet(x => x.InInventoryOf).Returns(body); retired.SetupGet(x => x.Deleted).Returns(() => deleted);
		moved.SetupGet(x => x.InInventoryOf).Returns(() => movedOwner);
		var policy = Mock.Of<ITransientItemPolicy>(x => x.Triggers == ItemRetirementTrigger.EtherealTransition);
		retired.Setup(x => x.GetItemTypes<ITransientItemPolicy>()).Returns([policy]);
		retired.Setup(x => x.Delete()).Callback(() => { deleted = true; movedOwner = otherBody; });
		var roots = new List<Tuple<IGameItem, IGrab>> { Tuple.Create(retired.Object, Mock.Of<IGrab>()), Tuple.Create(moved.Object, Mock.Of<IGrab>()) };
		typeof(RuntimeBody).GetField("_heldItems", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, roots);
		typeof(RuntimeBody).GetField("_directItems", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, new List<IGameItem> { retired.Object, moved.Object });
		body.EjectInventoryForPlanarTransition([]);
		Assert.IsTrue(deleted); Assert.AreSame(otherBody, moved.Object.InInventoryOf);
		moved.Verify(x => x.Drop(It.IsAny<IRoom>()), Times.Never);
	}

	[TestMethod]
	public void SelectiveTransition_DoesNotEmptyAnotherRootTransferredByAContainerCallback()
	{
		var body = TestObjectFactory.CreateUninitialized<RuntimeBody>(); var f = new Fixture();
		body.Actor = f.Base.Actor.Object; f.Base.Actor.SetupGet(x => x.Body).Returns(body);
		foreach (var name in new[] { "_heldItems", "_wieldedItems", "_wornItems", "_wornItemsOnly", "_implants", "_prosthetics", "_directItems" })
		{
			var field = typeof(RuntimeBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		var first = new Mock<IGameItem>(); var second = new Mock<IGameItem>(); var child = new Mock<IGameItem>(); var otherChild = new Mock<IGameItem>();
		var firstContainer = new Mock<IContainer>(); var secondContainer = new Mock<IContainer>();
		IBody owner = body; IGameItem? childParent = first.Object; IRoom? childRoom = null;
		first.SetupGet(x => x.InInventoryOf).Returns(() => owner); second.SetupGet(x => x.InInventoryOf).Returns(() => owner);
		child.SetupGet(x => x.ContainedIn).Returns(() => childParent); child.SetupGet(x => x.Location).Returns(() => childRoom);
		otherChild.SetupGet(x => x.ContainedIn).Returns(second.Object);
		first.Setup(x => x.GetItemType<IContainer>()).Returns(firstContainer.Object);
		second.Setup(x => x.GetItemType<IContainer>()).Returns(secondContainer.Object);
		firstContainer.SetupGet(x => x.Contents).Returns([child.Object]); secondContainer.SetupGet(x => x.Contents).Returns([otherChild.Object]);
		firstContainer.Setup(x => x.Take(f.Base.Actor.Object, child.Object, 0)).Returns(() =>
		{ childParent = null; owner = Mock.Of<IBody>(); return child.Object; });
		child.Setup(x => x.Drop(It.IsAny<IRoom>())).Returns<IRoom>(room => { childRoom = room; return child.Object; });
		typeof(RuntimeBody).GetField("_heldItems", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body,
			new List<Tuple<IGameItem, IGrab>> { Tuple.Create(first.Object, Mock.Of<IGrab>()), Tuple.Create(second.Object, Mock.Of<IGrab>()) });
		typeof(RuntimeBody).GetField("_directItems", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, new List<IGameItem> { first.Object, second.Object });
		body.EjectInventoryForPlanarTransition([9], retainPlaneSpecificInventory: true);
		secondContainer.Verify(x => x.Take(It.IsAny<MudSharp.Character.ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<int>()), Times.Never);
		Assert.AreSame(second.Object, otherChild.Object.ContainedIn); Assert.AreSame(f.Room.Object, childRoom);
	}

}
