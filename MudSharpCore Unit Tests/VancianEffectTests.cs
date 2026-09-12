using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianEffectTests
{
	[TestMethod]
	public void StoredDamageAndHealingUseCreatorTraitsAndLiveOpposedOutcomeWithReaderAttribution()
	{
		var f = new VancianTestFixture(); f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.None)).Returns(12);
		var spell = VancianSnapshotTests.Spell(f,"<Effect type='damage'><DamageType>0</DamageType><DamageExpression>source:1 + outcome + castinglevel</DamageExpression><Limb>-1</Limb></Effect><Effect type='heal'><HealingAmount>source:1 + outcome</HealingAmount><HealWorstWoundsFirst>true</HealWorstWoundsFirst><HealOverflow>true</HealOverflow></Effect>");
		var snapshot = StoredSpellSnapshot.Capture(spell,f.Actor.Object,f.Capability.Object,3,SpellPower.Strong,7,f.Clock.Now.UtcDateTime);
		var reader = new Mock<ICharacter>() { DefaultValue = DefaultValue.Mock }; var target = new Mock<ICharacter>() { DefaultValue = DefaultValue.Mock };
		reader.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(),It.IsAny<TraitBonusContext>())).Returns(900);
		f.Actor.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(),It.IsAny<TraitBonusContext>())).Returns(500);
		var wound = new Mock<IWound>(); wound.SetupProperty(x => x.CurrentDamage,100.0); wound.Setup(x => x.CanBeTreated(TreatmentType.Mend)).Returns(Difficulty.Normal);
		target.SetupGet(x => x.Body.Wounds).Returns([wound.Object]); var invocation = snapshot.CreateSpell(f.World.Object);
		var outcome = OpposedOutcomeDegree.Moderate;
		foreach (var effect in invocation.SpellEffects) effect.GetOrApplyEffect(reader.Object,target.Object,outcome,SpellPower.Strong,null!,[]);
		target.Verify(x => x.SufferDamage(It.Is<IDamage>(d => d.DamageAmount == 15 + (int)outcome && ReferenceEquals(d.ActorOrigin,reader.Object))),Times.Once);
		Assert.AreEqual(88-(int)outcome,wound.Object.CurrentDamage);
		reader.Verify(x => x.TraitValue(It.IsAny<ITraitDefinition>(),It.IsAny<TraitBonusContext>()),Times.Never);
		f.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(),It.IsAny<bool>()),Times.Never);
	}
	[TestMethod]
	public void RetainedArmourAndParentRoundTripAfterSourceDeletionWithoutLosingReaderOrNumbers()
	{
		var f = new VancianTestFixture(); var armour = new Mock<IArmourType>(); armour.SetupGet(x => x.Id).Returns(1);
		var material = new Mock<ISolid>(); material.SetupGet(x => x.Id).Returns(1);
		f.World.SetupGet(x => x.ArmourTypes).Returns(VancianTestFixture.Collection<IArmourType>(() => [armour.Object]));
		f.World.SetupGet(x => x.Materials).Returns(VancianTestFixture.Collection<ISolid>(() => [material.Object]));
		var applies = f.Prog("candidates",_ => true);
		var xml = $"<Effect type='spellarmour'><ArmourAppliesProg>{applies.Object.Id}</ArmourAppliesProg><ArmourType>1</ArmourType><ArmourMaterial>1</ArmourMaterial><MaximumDamageAbsorbed>source:1 + casterlevel</MaximumDamageAbsorbed></Effect>";
		var spell = VancianSnapshotTests.Spell(f,xml); spell.EffectDurationExpression = new TraitExpression("variable + castinglevel",f.World.Object);
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.None)).Returns(12);
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.SpellDuration)).Returns(40);
		var snapshot = StoredSpellSnapshot.Capture(spell,f.Actor.Object,f.Capability.Object,3,SpellPower.Strong,7,f.Clock.Now.UtcDateTime);
		var reader = new Mock<ICharacterInstance>() { DefaultValue = DefaultValue.Mock }; reader.SetupGet(x => x.Id).Returns(80); reader.SetupGet(x => x.InstanceId).Returns(81);
		reader.SetupGet(x => x.Gameworld).Returns(f.World.Object); reader.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(),It.IsAny<TraitBonusContext>())).Returns(999);
		var identity = new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.Instances).Returns([reader.Object]); reader.SetupGet(x => x.Identity).Returns(identity.Object);
		f.World.Setup(x => x.TryGetCharacter(80,true)).Returns(reader.Object); f.World.Setup(x => x.Actors.Has(reader.Object)).Returns(true);
		var invocation = snapshot.CreateSpell(f.World.Object); var parent = new MagicSpellParent(reader.Object,invocation,reader.Object);
		var child = (SpellArmourProtectionEffect)invocation.SpellEffects.Single().GetOrApplyEffect(reader.Object,reader.Object,OpposedOutcomeDegree.Marginal,SpellPower.Strong,parent,[])!;
		parent.AddSpellEffect(child); var saved = parent.SaveToXml([]);
		MagicSpellParent.InitialiseEffectType(); SpellArmourProtectionEffect.InitialiseEffectType(); f.Spells.Remove(spell);
		var restored = (MagicSpellParent)Effect.LoadEffect(saved,reader.Object); var restoredChild = (SpellArmourProtectionEffect)restored.SpellEffects.Single();
		Assert.AreSame(reader.Object,restored.Caster); Assert.AreSame(restored,restoredChild.ParentEffect);
		Assert.AreEqual(19,restoredChild.ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(reader.Object));
		Assert.AreEqual(43,((MagicSpell)restored.Spell).EffectDurationExpression.Evaluate(reader.Object,f.Trait.Object,TraitBonusContext.SpellDuration));
		f.World.Verify(x => x.TryGetCharacter(10,It.IsAny<bool>()),Times.Never);
	}
	[DataTestMethod]
	[DataRow("blindness")]
	[DataRow("sleep")]
	[DataRow("infravision")]
	[DataRow("glow")]
	public void StoredStatusAndPerceptionFamiliesCreateRealReaderOwnedEffects(string type)
	{
		var f = new VancianTestFixture(); var spell = VancianSnapshotTests.Spell(f);
		f.World.Setup(x => x.LightModel.GetIlluminationDescription(It.IsAny<double>())).Returns("bright");
		var configured = SpellEffectFactory.LoadEffectFromBuilderInput(type,new StringStack(""),spell).Trigger;
		spell = VancianSnapshotTests.Spell(f,configured.SaveToXml().ToString()); spell.EffectDurationExpression = new TraitExpression("60",f.World.Object);
		var snapshot = StoredSpellSnapshot.Capture(spell,f.Actor.Object,f.Capability.Object,1,SpellPower.Standard,3,f.Clock.Now.UtcDateTime);
		var reader = new Mock<ICharacter>() { DefaultValue = DefaultValue.Mock }; reader.SetupGet(x => x.Gameworld).Returns(f.World.Object); reader.SetupGet(x => x.Id).Returns(90);
		var invocation = snapshot.CreateSpell(f.World.Object); var parent = new MagicSpellParent(reader.Object,invocation,reader.Object);
		var child = invocation.SpellEffects.Single().GetOrApplyEffect(reader.Object,reader.Object,OpposedOutcomeDegree.Marginal,SpellPower.Standard,parent,[]);
		Assert.IsNotNull(child); Assert.AreSame(reader.Object,child.Owner); Assert.AreSame(reader.Object,parent.Caster);
	}
	[TestMethod]
	public void StoredBoostAndRemovalKeepFrozenScalarsAndApplyToLiveTarget()
	{
		var f = new VancianTestFixture(); var spell = VancianSnapshotTests.Spell(f,"<Effect type='boost' trait='1' bonus='5' context='0'/><Effect type='removesleep'/>");
		spell.EffectDurationExpression = new TraitExpression("60",f.World.Object);
		var invocation = StoredSpellSnapshot.Capture(spell,f.Actor.Object,f.Capability.Object,1,SpellPower.Standard,3,f.Clock.Now.UtcDateTime).CreateSpell(f.World.Object);
		var target = new Mock<ICharacter>() { DefaultValue = DefaultValue.Mock }; target.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var parent = new MagicSpellParent(target.Object,invocation,f.Actor.Object);
		Assert.IsNotNull(invocation.SpellEffects.First().GetOrApplyEffect(f.Actor.Object,target.Object,OpposedOutcomeDegree.Marginal,SpellPower.Standard,parent,[]));
		Assert.AreEqual(5,((TraitBoostEffect)invocation.SpellEffects.First()).Bonus);
		invocation.SpellEffects.Last().GetOrApplyEffect(f.Actor.Object,target.Object,OpposedOutcomeDegree.Marginal,SpellPower.Standard,parent,[]);
		target.Verify(x => x.RemoveAllEffects<SpellSleepEffect>(null,true),Times.Once);
	}
}
