#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class StripConsentSecurityTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void StripOther_AcceptedRequest_GrantsNoInventoryConsentDuringDelay(bool hasClothing)
	{
		var actor = new Mock<ICharacter>();
		var target = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var room = Mock.Of<IRoom>();
		actor.SetupGet(x => x.Location).Returns(room);
		target.SetupGet(x => x.Location).Returns(room);
		target.Setup(x => x.ColocatedWith(actor.Object)).Returns(true);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		target.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		target.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		target.SetupGet(x => x.Body).Returns(body.Object);
		var item = new Mock<IGameItem>();
		item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(item.Object, other));
		var wear = new Mock<IWearable>();
		var profile = new Mock<IWearProfile>();
		profile.SetupGet(x => x.AllProfiles).Returns(new Dictionary<IWear, IWearlocProfile>());
		wear.SetupGet(x => x.CurrentProfile).Returns(profile.Object);
		item.Setup(x => x.GetItemType<IWearable>()).Returns(wear.Object);
		var worn = hasClothing ? new List<IGameItem> { item.Object } : [];
		body.SetupGet(x => x.DirectWornItems).Returns(worn);
		var effects = new List<IEffect>();
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		target.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), false)).Callback<IEffect, bool>((e, _) => effects.Remove(e));
		body.Setup(x => x.CanBeRemoved(item.Object, actor.Object)).Returns(() =>
			effects.OfType<StripItemConsent>().Any(x => x.Item == item.Object && x.Stripper == actor.Object));
		body.Setup(x => x.RemoveItem(item.Object, It.IsAny<IEmote>(), actor.Object))
			.Callback(() => worn.Remove(item.Object));
		Accept? proposal = null;
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((e, _) => proposal = e as Accept);
		SimpleCharacterAction? action = null;
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((e, _) => action = e as SimpleCharacterAction);

		typeof(InventoryModule).GetMethod("StripOther", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [actor.Object, target.Object, new StringStack(string.Empty), null]);
		Assert.IsNotNull(proposal);
		proposal.Proposal.Accept(string.Empty);
		Assert.IsNotNull(action);
		Assert.AreEqual(0, effects.Count);
		target.Verify(x => x.AddEffect(It.Is<IEffect>(e => e is BeDressedEffect), It.IsAny<TimeSpan>()), Times.Never);
		action.Action(actor.Object);
		Assert.AreEqual(0, effects.Count);
		body.Verify(x => x.RemoveItem(item.Object, It.IsAny<IEmote>(), actor.Object), hasClothing ? Times.Once() : Times.Never());
	}

	[TestMethod]
	public void StripConsent_NativeRemovalGate_AdmitsOnlyMatchingWornItemAndStripper()
	{
		var (actor, target, body, selected, other, held, effects) = NativeFixture();
		Assert.IsFalse(body.CanBeRemoved(selected, actor.Object));
		Assert.IsTrue(InventoryModule.WithStripItemConsent(actor.Object, target.Object, selected, true, () =>
		{
			Assert.IsTrue(body.CanBeRemoved(selected, actor.Object));
			Assert.IsFalse(body.CanBeRemoved(other, actor.Object));
			Assert.IsFalse(body.CanBeRemoved(held, actor.Object));
			Assert.AreEqual(0, effects.OfType<BeDressedEffect>().Count());
			return true;
		}));
		Assert.IsFalse(body.CanBeRemoved(selected, actor.Object));
		Assert.AreEqual(0, effects.Count);
		InventoryModule.WithStripItemConsent(actor.Object, target.Object, held, true, () =>
		{
			Assert.IsFalse(body.CanBeRemoved(held, actor.Object));
			return true;
		});
		var otherStripper = new Mock<ICharacter>();
		InventoryModule.WithStripItemConsent(otherStripper.Object, target.Object, selected, true, () =>
		{
			Assert.IsFalse(body.CanBeRemoved(selected, actor.Object));
			return true;
		});
	}

	[TestMethod]
	public void StripConsent_RemovalThrows_AlwaysRevokesConsent()
	{
		var (actor, target, body, selected, _, _, effects) = NativeFixture();
		Assert.ThrowsException<InvalidOperationException>(() => InventoryModule.WithStripItemConsent(
			actor.Object, target.Object, selected, true, () => throw new InvalidOperationException("Test removal failure")));
		Assert.AreEqual(0, effects.Count);
		Assert.IsFalse(body.CanBeRemoved(selected, actor.Object));
	}

	private static (Mock<ICharacter> Actor, Mock<ICharacter> Target, Body Body, IGameItem Selected,
		IGameItem Other, IGameItem Held, List<IEffect> Effects) NativeFixture()
	{
		var actor = new Mock<ICharacter>();
		var target = new Mock<ICharacter>();
		var handBody = new Mock<IBody>();
		handBody.SetupGet(x => x.HoldLocs).Returns([Mock.Of<IGrab>()]);
		handBody.Setup(x => x.CanUseBodypart(It.IsAny<IBodypart>())).Returns(CanUseBodypartResult.CanUse);
		handBody.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		actor.SetupGet(x => x.Body).Returns(handBody.Object);
		actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
		var body = TestObjectFactory.CreateUninitialized<Body>();
		var prototype = new Mock<IBodyPrototype>();
		prototype.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		typeof(Body).GetProperty(nameof(Body.Prototype))!.SetValue(body, prototype.Object);
		typeof(Body).GetProperty(nameof(Body.EffectHandler))!.SetValue(body, Mock.Of<IEffectHandler>());
		body.Actor = target.Object;
		target.SetupGet(x => x.Body).Returns(body);
		target.SetupGet(x => x.State).Returns(CharacterState.Awake);
		var effects = new List<IEffect>();
		target.Setup(x => x.EffectsOfType<StripItemConsent>(It.IsAny<Predicate<StripItemConsent>>()))
			.Returns(() => effects.OfType<StripItemConsent>());
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		target.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), false)).Callback<IEffect, bool>((e, _) => effects.Remove(e));
		IGameItem Item()
		{
			var item = new Mock<IGameItem>();
			item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(item.Object, other));
			item.SetupGet(x => x.InInventoryOf).Returns(body);
			item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			return item.Object;
		}
		var selected = Item();
		var other = Item();
		var held = Item();
		SetField(body, "_directItems", new List<IGameItem> { selected, other, held });
		SetField(body, "_directWornItems", new List<IGameItem> { selected, other });
		SetField(body, "_wornItems", new List<(IGameItem, IWear, IWearlocProfile)>
		{
			(selected, Mock.Of<IWear>(), Mock.Of<IWearlocProfile>()),
			(other, Mock.Of<IWear>(), Mock.Of<IWearlocProfile>())
		});
		return (actor, target, body, selected, other, held, effects);
	}

	private static void SetField(Body body, string field, object value)
	{
		typeof(Body).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, value);
	}
}
