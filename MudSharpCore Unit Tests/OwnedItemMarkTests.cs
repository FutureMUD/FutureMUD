#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class OwnedItemMarkTests
{
	[TestMethod]
	public void CustodianGlyph_OnlyCurrentLegalOwnerCanTake_AndAnchorUsesExactItemIdentity()
	{
		var fixture = new MagicCastingFixture();
		var item = new Mock<IGameItem>();
		var effects = new List<IEffect>();
		var owner = new ItemOwnershipReference("Character", fixture.Actor.Object.InstanceId);
		ItemOwnershipReference? currentOwner = owner;
		item.SetupGet(x => x.Id).Returns(200);
		item.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		item.SetupGet(x => x.Effects).Returns(() => effects);
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		item.SetupGet(x => x.OwnershipReference).Returns(() => currentOwner);
		item.Setup(x => x.IsOwnedBy(It.IsAny<ICharacter>())).Returns<ICharacter>(actor =>
			currentOwner == owner && ReferenceEquals(actor, fixture.Actor.Object));
		var parent = new MagicSpellParent(item.Object, fixture.Spell, fixture.Actor.Object);
		var glyph = new SpellCustodianGlyphEffect(item.Object, parent, owner);
		var anchor = new SpellAnchorRuneEffect(item.Object, parent, owner);
		parent.AddSpellEffect(glyph);
		parent.AddSpellEffect(anchor);
		effects.Add(parent);
		effects.Add(glyph);
		effects.Add(anchor);
		var stranger = new Mock<ICharacter>();

		Assert.IsTrue(glyph.IsActive);
		Assert.IsTrue(glyph.AllowsTake(fixture.Actor.Object));
		Assert.IsFalse(glyph.AllowsTake(stranger.Object));
		Assert.IsTrue(anchor.IsActive);
		Assert.AreEqual(200L, ((ISpellTravelAnchorEffect)anchor).ItemId);
		Assert.AreEqual(owner, anchor.OwnerReference);
		currentOwner = new ItemOwnershipReference("Character", owner.Id + 1);
		Assert.IsFalse(glyph.IsActive);
		Assert.IsTrue(glyph.AllowsTake(stranger.Object));
		Assert.IsFalse(anchor.IsActive);
	}

	[TestMethod]
	public void OwnedItemMarks_ColdReloadRetainsOwnerAndExactItemBinding()
	{
		var fixture = new MagicCastingFixture();
		var item = new Mock<IGameItem>();
		var effects = new List<IEffect>();
		var owner = new ItemOwnershipReference("Character", fixture.Actor.Object.InstanceId);
		item.SetupGet(x => x.Id).Returns(210);
		item.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		item.SetupGet(x => x.OwnershipReference).Returns(owner);
		item.SetupGet(x => x.Effects).Returns(() => effects);
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		item.Setup(x => x.IsOwnedBy(fixture.Actor.Object)).Returns(true);
		MagicSpellParent.InitialiseEffectType();
		SpellCustodianGlyphEffect.InitialiseEffectType();
		SpellAnchorRuneEffect.InitialiseEffectType();
		var parent = new MagicSpellParent(item.Object, fixture.Spell, fixture.Actor.Object);
		parent.AddSpellEffect(new SpellCustodianGlyphEffect(item.Object, parent, owner));
		parent.AddSpellEffect(new SpellAnchorRuneEffect(item.Object, parent, owner));
		var saved = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromSeconds(30) });
		var reloaded = (MagicSpellParent)Effect.LoadEffect(saved, item.Object);
		effects.Add(reloaded);
		var glyph = reloaded.SpellEffects.OfType<SpellCustodianGlyphEffect>().Single();
		var anchor = reloaded.SpellEffects.OfType<SpellAnchorRuneEffect>().Single();
		Assert.IsTrue(glyph.IsActive);
		Assert.IsTrue(anchor.IsActive);
		Assert.AreEqual(owner, glyph.OwnerReference);
		Assert.AreEqual(owner, anchor.OwnerReference);
		Assert.AreEqual(210L, anchor.ItemId);
		Assert.IsFalse(glyph.AllowsTake(new Mock<ICharacter>().Object));
	}
}
