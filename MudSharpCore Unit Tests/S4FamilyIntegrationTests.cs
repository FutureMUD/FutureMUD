#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.Testing.EnvironmentalMagic;
using Body = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4FamilyIntegrationTests
{
	[TestMethod]
	public void CustodianGlyph_ConcreteBodyRetrievalDeniesStrangerAndAdmitsCurrentOwner()
	{
		var f = new MagicCastingFixture();
		var body = EmptyBody(f);
		var hand = new Mock<IGrab>();
		hand.Setup(x => x.CanGrab(It.IsAny<IGameItem>(), body)).Returns(WearlocGrabResult.Success);
		Field(body, "_holdlocs").SetValue(body, new List<IGrab> { hand.Object });
		Field(body, "_bodyparts").SetValue(body, new HashSet<IBodypart> { hand.Object });
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(200);
		item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(item.Object, other));
		item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		item.Setup(x => x.CanGet(0, It.IsAny<ItemCanGetIgnore>())).Returns(ItemGetResponse.CanGet);
		var legalOwner = false;
		var owner = new ItemOwnershipReference("Character", f.Actor.Object.InstanceId);
		item.SetupGet(x => x.OwnershipReference).Returns(owner);
		item.Setup(x => x.IsOwnedBy(f.Actor.Object)).Returns(() => legalOwner);
		var parent = new MagicSpellParent(item.Object, f.Spell, f.Actor.Object);
		var glyph = new SpellCustodianGlyphEffect(item.Object, parent, owner);
		parent.AddSpellEffect(glyph);
		item.SetupGet(x => x.Effects).Returns([parent, glyph]);
		item.Setup(x => x.EffectsOfType<IItemTakeInterdictionEffect>()).Returns([glyph]);
		Assert.IsFalse(body.CanGet(item.Object, 0, ItemCanGetIgnore.IgnoreWeight));
		body.Get(item.Object, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
		item.Verify(x => x.Get(It.IsAny<IBody>()), Times.Never);
		Assert.IsFalse(body.DirectItems.Any());
		legalOwner = true;
		Assert.IsTrue(body.CanGet(item.Object, 0, ItemCanGetIgnore.IgnoreWeight));
	}

	[TestMethod]
	public void ThresholdAlarm_ConcreteRoomEntryDispatchesReloadedChildOnceAndRemovalStopsNotice()
	{
		var f = new MagicCastingFixture();
		using var env = new EnvironmentalMagicTestWorld(start: false);
		var room = env.Rooms.First();
		env.World.SetupGet(x => x.Characters).Returns(MagicCastingFixture.Collection<ICharacter>(() => [f.Actor.Object]));
		var parent = new MagicSpellParent(room, f.Spell, f.Actor.Object);
		SpellThresholdAlarmEffect.InitialiseEffectType();
		var original = new SpellThresholdAlarmEffect(room, parent, f.Actor.Object);
		var child = (SpellThresholdAlarmEffect)Effect.LoadEffect(original.SaveToXml(new()), room);
		child.ParentEffect = parent;
		parent.AddSpellEffect(child);
		room.AddEffect(parent);
		room.AddEffect(child);
		var entrant = new Mock<ICharacter>();
		entrant.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.ExternalItems == Array.Empty<IGameItem>()));
		entrant.SetupGet(x => x.Location).Returns(room);
		entrant.SetupGet(x => x.Gameworld).Returns(env.World.Object);
		f.Messages.Clear();
		room.Enter(entrant.Object);
		Assert.AreEqual(1, f.Messages.Count(x => x.Contains("threshold alarm")));
		room.RemoveEffect(parent, true);
		room.Enter(entrant.Object);
		Assert.AreEqual(1, f.Messages.Count(x => x.Contains("threshold alarm")));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PlanarStock_ConcreteInitialEffectEjectsOnlyNoncorporealInventoryAndResolverReturnsOnRemoval(bool manifested)
	{
		var f = new MagicCastingFixture();
		var body = EmptyBody(f);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(200);
		item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(item.Object, other));
		var holdable = new Mock<IHoldable>();
		holdable.SetupProperty(x => x.HeldBy, body);
		item.Setup(x => x.GetItemType<IHoldable>()).Returns(holdable.Object);
		Field(body, "_heldItems").SetValue(body, new List<Tuple<IGameItem, IGrab>> { Tuple.Create(item.Object, Mock.Of<IGrab>()) });
		body.RecalculateItemHelpers();
		var key = manifested ? ArmageddonPlanarSpellContent.CrossTheVeilKey : ArmageddonPlanarSpellContent.ShadowPassageKey;
		var contribution = ArmageddonPlanarSpellContent.Create(2).Single(x => x.Content.Key == key);
		var definition = contribution.Content.BuildDefinition(11, 1, 0).Descendants("PlanarData").Single();
		var presence = PlanarPresenceDefinition.FromXml(definition, f.World.Object);
		var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
		var child = new SpellPlanarStateEffect(f.Actor.Object, parent, presence);
		var overlays = new List<IPlanarOverlayEffect> { child };
		f.Actor.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<Predicate<IPlanarOverlayEffect>>())).Returns(() => overlays);
		parent.AddSpellEffect(child);
		child.InitialEffect();
		CollectionAssert.AreEquivalent(new long[] { 2 }, f.Actor.Object.GetPlanarPresence().PresencePlaneIds.ToArray());
		Assert.AreEqual(manifested, body.HeldItems.Contains(item.Object));
		Assert.AreEqual(manifested ? body : null, holdable.Object.HeldBy);
		item.Verify(x => x.Drop(It.IsAny<IRoom>()), manifested ? Times.Never() : Times.Once());
		Assert.AreEqual(!manifested, f.Actor.Object.GetPlanarPresence().SuspendsPhysicalContact);
		overlays.Clear();
		CollectionAssert.AreEquivalent(new long[] { 1 }, f.Actor.Object.GetPlanarPresence().PresencePlaneIds.ToArray());
		Assert.IsFalse(f.Actor.Object.GetPlanarPresence().SuspendsPhysicalContact);
		Assert.AreEqual(manifested, body.HeldItems.Contains(item.Object), "Removing the overlay must not recreate ejected belongings.");
	}

	[TestMethod]
	public void EarthMount_ExactCreatorAdmissionAndRiderRetirementHoldShareNativeGuards()
	{
		var f = new MagicCastingFixture();
		var mount = new Mock<ICharacter>();
		mount.SetupGet(x => x.Id).Returns(901);
		mount.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		mount.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 902));
		var service = new Mock<ISpellOwnedNpcService>();
		service.Setup(x => x.HasCommandAuthority(mount.Object, f.Actor.Object, It.IsAny<Guid>(),
			f.Actor.Object.Id, f.Actor.Object.InstanceId, It.IsAny<long>(), It.IsAny<int>())).Returns(true);
		f.World.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var now = DateTime.UtcNow;
		var bond = new SpellNpcControlEffect(mount.Object, new(Guid.NewGuid(), 1, 1, f.Actor.Object.Id,
			"earth-mount", SpellLifecycleMode.TemporaryCleanup, now, now.AddMinutes(1)), f.Actor.Object);
		mount.Setup(x => x.EffectsOfType<SpellNpcControlEffect>()).Returns([bond]);
		var native = new SpellOwnedNpcService(f.World.Object);
		Assert.IsTrue(native.HasCreatorMountAdmission(mount.Object, f.Actor.Object));
		Assert.IsFalse(native.HasCreatorMountAdmission(mount.Object, Mock.Of<ICharacter>()));
		mount.SetupGet(x => x.Riders).Returns([f.Actor.Object]);
		Assert.IsTrue(CharacterArchiveService.HasRuntimeDependants(mount.Object), "Expiry must hold a ridden mount before native death/evacuation.");
		mount.SetupGet(x => x.Riders).Returns([]);
		Assert.IsFalse(CharacterArchiveService.HasRuntimeDependants(mount.Object));
		mount.Setup(x => x.EffectsOfType<SpellNpcControlEffect>()).Returns([bond, bond]);
		Assert.IsFalse(native.HasCreatorMountAdmission(mount.Object, f.Actor.Object));
	}

	private static Body EmptyBody(MagicCastingFixture f)
	{
		var body = TestObjectFactory.CreateUninitialized<Body>();
		body.SetNoSave(true);
		body.Actor = f.Actor.Object;
		f.Actor.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		f.Actor.SetupGet(x => x.Body).Returns(body);
		typeof(Body).GetProperty(nameof(Body.Gameworld))!.SetValue(body, f.World.Object);
		typeof(Body).GetProperty(nameof(Body.Prototype))!.SetValue(body,
			Mock.Of<IBodyPrototype>(x => x.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
		typeof(PerceivedItem).GetProperty(nameof(Body.EffectHandler))!.SetValue(body, new EffectHandler(body));
		foreach (var name in new[] { "_prosthetics", "_wounds", "_severedRoots", "_wieldedItems", "_heldItems", "_wornItems", "_implants", "_bodyparts", "_organs", "_bones", "_holdlocs" })
		{
			var field = Field(body, name);
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		body.RecalculateItemHelpers();
		return body;
	}

	private static FieldInfo Field(Body body, string name) => typeof(Body).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
}
