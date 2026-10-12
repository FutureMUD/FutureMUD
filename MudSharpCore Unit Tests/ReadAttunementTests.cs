#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ReadAttunementTests
{
	[TestMethod]
	public void LearnedAffiliations_RequiresExactCompletedEnrolmentAndDoesNotInferSharedKnowledge()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Assert.AreEqual(0, f.Service.LearnedAffiliations(f.Actor.Object).Count);
		var policy = f.Earth.CastingPolicy!;
		f.Store.Enrolments[(100, policy.Identity)] = new(100, policy.Identity, f.Earth.Id, f.Now, 0);
		Assert.AreEqual(0, f.Service.LearnedAffiliations(f.Actor.Object).Count);
		f.Store.Enrolments[(100, policy.Identity)] = new(100, policy.Identity, f.Sorcerer.Id, f.Now, 1);
		Assert.AreEqual(0, f.Service.LearnedAffiliations(f.Actor.Object).Count);
		f.Store.Enrolments[(100, policy.Identity)] = new(100, Guid.NewGuid(), f.Earth.Id, f.Now, 1);
		Assert.AreEqual(0, f.Service.LearnedAffiliations(f.Actor.Object).Count);
		f.Store.Enrolments[(100, policy.Identity)] = new(100, policy.Identity, f.Earth.Id, f.Now, 1);
		var result = f.Service.LearnedAffiliations(f.Actor.Object);
		Assert.AreEqual(1, result.Count); Assert.AreEqual(f.Earth.Id, result[0].CapabilityId);
		Assert.IsTrue(result[0].Active, "The completed history is learned; current attached permission can reactivate it.");
		Assert.AreEqual(1, f.Store.Writes); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Flushes);
	}

	[TestMethod]
	public void LearnedAffiliations_CurrentPermissionCanBecomeInactiveAndReattachWithoutCreatingHistory()
	{
		var f = new MagicCastingFixture();
		f.Store.Enrolments[(100, f.Earth.CastingPolicy!.Identity)] = new(100, f.Earth.CastingPolicy.Identity, f.Earth.Id, f.Now, 1);
		Assert.IsTrue(f.Service.LearnedAffiliations(f.Actor.Object).Single().Active);
		f.ActiveCapabilities.Remove(f.Earth);
		f.Restart();
		Assert.IsFalse(f.Service.LearnedAffiliations(f.Actor.Object).Single().Active);
		f.ActiveCapabilities.Add(f.Earth);
		Assert.IsTrue(f.Service.LearnedAffiliations(f.Actor.Object).Single().Active);
		f.Store.Enrolments.Clear();
		Assert.AreEqual(0, f.Service.LearnedAffiliations(f.Actor.Object).Count);
		Assert.AreEqual(0, f.Store.Writes); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Flushes);
	}

	[TestMethod]
	public void LearnedAffiliations_SecondaryBodyReadsCanonicalHistoryWithItsOwnPermission()
	{
		var f = new MagicCastingFixture();
		var primary = new Mock<ICharacterInstance>(); primary.SetupGet(x => x.Id).Returns(100);
		var identity = new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		var secondary = new Mock<ICharacter>(); secondary.SetupGet(x => x.Id).Returns(200);
		secondary.SetupGet(x => x.Identity).Returns(identity.Object);
		secondary.SetupGet(x => x.Capabilities).Returns(Array.Empty<IMagicCapability>());
		f.Store.Enrolments[(100, f.Earth.CastingPolicy!.Identity)] = new(100, f.Earth.CastingPolicy.Identity, f.Earth.Id, f.Now, 1);
		Assert.AreEqual("Earth", f.Service.LearnedAffiliations(secondary.Object).Single().Name);
		Assert.IsFalse(f.Service.LearnedAffiliations(secondary.Object).Single().Active);
		Assert.AreEqual(0, f.Store.Writes);
	}

	[TestMethod]
	public void Overlay_SeparatesAssignedAffinitiesFromLearnedHistoryAndSuppressesInvisibleTargets()
	{
		var f = new MagicCastingFixture(); var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var affinity = new Mock<IElementalAffinityMerit>(); affinity.SetupGet(x => x.ElementalAffinities).Returns(new[] { "Wind", "Earth", "earth" });
		target.SetupGet(x => x.Merits).Returns(new IMerit[] { affinity.Object });
		var service = new Mock<IMagicCastingService>(MockBehavior.Strict);
		service.Setup(x => x.LearnedAffiliations(target.Object)).Returns(new[] { new MagicCastingAffiliation(9, "Water", false) });
		f.World.SetupGet(x => x.MagicCasting).Returns(service.Object);
		f.Actor.Setup(x => x.CanSee(target.Object, PerceiveIgnoreFlags.None)).Returns(true);
		var effect = new SpellReadAttunementEffect(target.Object, new Mock<IMagicSpellEffectParent>().Object, 100, 100);
		var text = effect.GetAdditionalText(f.Actor.Object, false);
		StringAssert.Contains(text, "Assigned elemental affinities: Earth and Wind.");
		StringAssert.Contains(text, "Learned magical affiliations: Water (inactive).");
		f.Actor.Setup(x => x.CanSee(target.Object, PerceiveIgnoreFlags.None)).Returns(false);
		Assert.IsFalse(effect.DescriptionAdditionApplies(f.Actor.Object));
		Assert.AreEqual(string.Empty, effect.GetAdditionalText(f.Actor.Object, false));
		service.Verify(x => x.LearnedAffiliations(target.Object), Times.Once); service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void Overlay_UnassignedAndUnavailableAreDifferentFromNoLearnedHistory()
	{
		var f = new MagicCastingFixture(); var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		target.SetupGet(x => x.Name).Returns("Fire Sorcerer Earth Elemental");
		f.Actor.Setup(x => x.CanSee(target.Object, PerceiveIgnoreFlags.None)).Returns(true);
		var effect = new SpellReadAttunementEffect(target.Object, new Mock<IMagicSpellEffectParent>().Object, 100, 100);
		StringAssert.Contains(effect.GetAdditionalText(f.Actor.Object, false), "Assigned elemental affinities: Unassigned.");
		StringAssert.Contains(effect.GetAdditionalText(f.Actor.Object, false), "Learned magical affiliations: None.");
		var service = new Mock<IMagicCastingService>();
		service.Setup(x => x.LearnedAffiliations(target.Object)).Throws(new InvalidOperationException("private provider details"));
		f.World.SetupGet(x => x.MagicCasting).Returns(service.Object);
		var text = effect.GetAdditionalText(f.Actor.Object, false);
		StringAssert.Contains(text, "Learned magical affiliations: Unavailable.");
		Assert.IsFalse(text.Contains("private provider"));
	}

	[TestMethod]
	public void Overlay_TemplateAndSavedChildRoundTripThroughNativeFactories()
	{
		var f = new MagicCastingFixture();
		var template = SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='readattunement'/>"), f.Spell);
		Assert.AreEqual("readattunement", template.Clone().SaveToXml().Attribute("type")!.Value);
		Assert.IsFalse(template.IsInstantaneous); Assert.IsTrue(template.IsCompatibleWithTrigger(f.Spell.Trigger));
		SpellReadAttunementEffect.InitialiseEffectType();
		var child = new SpellReadAttunementEffect(f.Actor.Object, new Mock<IMagicSpellEffectParent>().Object, 100, 100);
		var saved = child.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = Effect.LoadEffect(saved, f.Actor.Object);
		Assert.IsInstanceOfType(loaded, typeof(SpellReadAttunementEffect));
		Assert.AreEqual(100L, ((SpellReadAttunementEffect)loaded).CasterInstanceId);
	}

	[TestMethod]
	public void Overlay_OnlyExactCasterInstanceCanReadItsSelectedTarget()
	{
		var f = new MagicCastingFixture(); var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var service = new Mock<IMagicCastingService>(MockBehavior.Strict);
		f.World.SetupGet(x => x.MagicCasting).Returns(service.Object);
		var effect = new SpellReadAttunementEffect(target.Object, new Mock<IMagicSpellEffectParent>().Object, 100, 100);
		f.Actor.Setup(x => x.CanSee(target.Object, PerceiveIgnoreFlags.None)).Returns(true);
		f.Actor.SetupGet(x => x.InstanceId).Returns(101);
		Assert.IsFalse(effect.DescriptionAdditionApplies(f.Actor.Object));
		Assert.AreEqual(string.Empty, effect.GetAdditionalText(f.Actor.Object, false));
		f.Actor.SetupGet(x => x.InstanceId).Returns(100); f.Actor.SetupGet(x => x.Id).Returns(999);
		Assert.IsFalse(effect.DescriptionAdditionApplies(f.Actor.Object));
		service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void PhysicalReferences_LiveAndNestedSavedChildDeclareOnlyExactCasterFields()
	{
		var f = new MagicCastingFixture();
		var child = new SpellReadAttunementEffect(f.Actor.Object, new Mock<IMagicSpellEffectParent>().Object, 17, 23);
		var live = child.PhysicalReferences.ToArray();
		Assert.IsTrue(live.Any(x => x.Kind == PhysicalEntityKind.Character && x.Id == 17));
		Assert.IsTrue(live.Any(x => x.Kind == PhysicalEntityKind.CharacterInstance && x.Id == 23));
		var envelope = new XElement("Effects", new XElement("Effect", new XElement("Type", "MagicSpellParent"),
			new XElement("Effect", new XElement("Caster", 1), new XElement("CasterInstance", 2),
				new XElement("StoredSpell", new XElement("Text", "CharacterId 91 BodyId 91")),
				new XElement("Children", child.SaveToXml(new Dictionary<IEffect, TimeSpan>())))));
		var saved = PhysicalReferenceCodecs.Effects(envelope.ToString()).ToArray();
		Assert.IsTrue(saved.Any(x => x.Kind == PhysicalEntityKind.Character && x.Id == 17));
		Assert.IsTrue(saved.Any(x => x.Kind == PhysicalEntityKind.CharacterInstance && x.Id == 23));
		Assert.IsFalse(saved.Any(x => x.Id == 91 || x.Kind == PhysicalEntityKind.Body));
		var missing = "<Effects><Effect><Type>SpellReadAttunement</Type><Effect><CasterInstanceId>23</CasterInstanceId></Effect></Effect></Effects>";
		Assert.ThrowsException<FormatException>(() => PhysicalReferenceCodecs.Effects(missing).ToArray());
	}

	[TestMethod]
	public void PaidCast_AttachesOverlayToNormalParentAndUnfundedRetryHasNoSideEffects()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		var effects = (List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects; effects.Clear();
		effects.Add(SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='readattunement'/>"), f.Spell));
		var children = new List<IEffect>(); MagicSpellParent? parent = null;
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(children.Add);
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, duration) =>
		{
			Assert.IsTrue(duration > TimeSpan.Zero); parent = effect as MagicSpellParent;
		});
		var result = f.Service.Cast(f.Intent(1, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(95.0, f.Balances[f.Resources[1]]); Assert.IsNotNull(parent);
		var child = parent.SpellEffects.Single();
		Assert.IsInstanceOfType(child, typeof(SpellReadAttunementEffect));
		MagicSpellParent.InitialiseEffectType(); SpellReadAttunementEffect.InitialiseEffectType();
		var saved = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromMinutes(1) });
		var loaded = (MagicSpellParent)Effect.LoadEffect(saved, f.Actor.Object);
		var restoredChild = (SpellReadAttunementEffect)loaded.SpellEffects.Single();
		Assert.AreEqual(100L, restoredChild.CasterId); Assert.AreEqual(100L, restoredChild.CasterInstanceId);
		Assert.AreSame(loaded, restoredChild.ParentEffect);
		parent.RemovalEffect(); f.Actor.Verify(x => x.RemoveEffect(child, true), Times.Once);
		var rolls = f.Rolls; var count = children.Count; f.Balances[f.Resources[1]] = 0;
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(1, false)).Status);
		Assert.AreEqual(rolls, f.Rolls); Assert.AreEqual(count, children.Count); Assert.AreEqual(0.0, f.Balances[f.Resources[1]]);
	}
}
