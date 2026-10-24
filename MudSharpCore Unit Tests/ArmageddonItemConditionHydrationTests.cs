#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonItemConditionHydrationTests
{
	[TestMethod]
	public void ItemConditionDefinitions_HydrateAsReadyNativeSpells()
	{
		var fixture = new MagicCastingFixture();
		foreach (var (index, contribution) in ArmageddonItemConditionSpellContent.Create().Select((x, i) => (i, x)))
		{
			var content = contribution.Content;
			var row = content.SpellRow(1, 1, 1);
			row.Id = 2200 + index;
			row.EffectDurationExpressionId = 2;
			row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			var spell = new MagicSpell(row, fixture.World.Object);
			Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, content.Key);
			Assert.IsTrue(spell.ReadyForGame, content.Key);
		}
	}

	[TestMethod]
	public void BorrowTheDeadDefinition_HydratesCorpseTriggerAndNativePossession()
	{
		var fixture = new MagicCastingFixture();
		var content = ArmageddonBorrowTheDeadSpellContent.Create().Content;
		var row = content.SpellRow(1, 1, 1);
		row.Id = 2210;
		row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(row, fixture.World.Object);
		Assert.AreEqual(0, spell.GradeConfigurationErrors().Count);
		Assert.IsTrue(spell.ReadyForGame);
		Assert.IsInstanceOfType(spell.SpellEffects.Single(), typeof(PossessCorpseSpellEffect));
	}

	[TestMethod]
	public void BorrowTheDead_PreparedSelectionRefusesAnItemWithoutACorpseBeforePayment()
	{
		var fixture = new MagicCastingFixture();
		var content = ArmageddonBorrowTheDeadSpellContent.Create().Content;
		var row = content.SpellRow(1, 1, 1);
		row.Id = 2211;
		row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(row, fixture.World.Object);
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.PrimaryInstance).Returns(new Mock<ICharacterInstance>().Object);
		fixture.Actor.SetupGet(x => x.Identity).Returns(identity.Object);
		var item = new Mock<IGameItem>();
		var effect = spell.SpellEffects.OfType<PossessCorpseSpellEffect>().Single();
		Assert.ThrowsExactly<System.InvalidOperationException>(() =>
			effect.CapturePreparedSelection(fixture.Actor.Object, item.Object));
	}

	[TestMethod]
	public void BorrowTheDead_PreparedSelectionFreezesOriginalBodyAndRoom()
	{
		var fixture = new MagicCastingFixture();
		var content = ArmageddonBorrowTheDeadSpellContent.Create().Content;
		var row = content.SpellRow(1, 1, 1);
		row.Id = 2212;
		row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(row, fixture.World.Object);
		var primary = new Mock<ICharacterInstance>();
		primary.SetupGet(x => x.Id).Returns(100);
		primary.SetupGet(x => x.InstanceId).Returns(100);
		primary.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		primary.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		fixture.Actor.SetupGet(x => x.Identity).Returns(identity.Object);
		var item = new Mock<IGameItem>();
		var corpse = new Mock<ICorpse>();
		var body = new Mock<IBody>();
		var original = new Mock<ICharacter>();
		corpse.SetupGet(x => x.OriginalBody).Returns(body.Object);
		corpse.SetupGet(x => x.OriginalCharacter).Returns(original.Object);
		item.SetupGet(x => x.Id).Returns(91);
		item.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		item.SetupGet(x => x.TrueLocations).Returns([fixture.Actor.Object.Location]);
		item.Setup(x => x.GetItemType<ICorpse>()).Returns(corpse.Object);
		item.Setup(x => x.EffectsOfType<IPossessedBodyEffect>(It.IsAny<Predicate<IPossessedBodyEffect>>())).Returns([]);
		item.Setup(x => x.EffectsOfType<ILiveBodyPossessionEffect>(It.IsAny<Predicate<ILiveBodyPossessionEffect>>())).Returns([]);
		item.Setup(x => x.EffectsOfType<ICorpsePossessionEffect>(It.IsAny<Predicate<ICorpsePossessionEffect>>())).Returns([]);
		fixture.World.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection<ICharacter>(() => []));
		fixture.World.SetupGet(x => x.CachedActors).Returns(MagicCastingFixture.Collection<ICharacter>(() => []));
		fixture.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new[] { item.Object }));
		var effect = spell.SpellEffects.OfType<PossessCorpseSpellEffect>().Single();
		var token = effect.CapturePreparedSelection(fixture.Actor.Object, item.Object);
		Assert.IsNotNull(token);
		Assert.IsTrue(effect.TryConfirmPreparedSelection(fixture.Actor.Object, item.Object, out _));
		corpse.SetupGet(x => x.OriginalBody).Returns(new Mock<IBody>().Object);
		Assert.IsFalse(effect.TryConfirmPreparedSelection(fixture.Actor.Object, item.Object, out _));
	}
}
