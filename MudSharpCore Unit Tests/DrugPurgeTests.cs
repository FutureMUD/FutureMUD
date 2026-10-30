#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugPurgeTests
{
	private const string Family = "arm.effect.poison";
	private static DrugDosage Dose(IDrug drug, string? family = null) => new() { Drug = drug, Grams = 1,
		OriginalVector = DrugVector.Touched, SpellOriginId = family is null ? null : Guid.NewGuid(),
		SpellOriginFamily = family!, SpellOriginFormula = "0.001*grade" };

	[TestMethod]
	public void DrugPurge_ExplicitFamilyPreservesMundaneAndOtherOriginsAndOtherDrugs()
	{
		var f = new MagicCastingFixture(); var drug = Mock.Of<IDrug>(x => x.Id == 81);
		var other = Mock.Of<IDrug>(x => x.Id == 82);
		var active = new List<DrugDosage> { Dose(drug, Family), Dose(drug), Dose(drug, "other"), Dose(other) };
		var latent = new List<DrugDosage> { Dose(drug, Family), Dose(other, Family) };
		SetupDoses(f, active, latent);
		Assert.IsTrue(DrugPurgeRules.HasMatching(f.Actor.Object, drug, Family));
		Assert.AreEqual(4, active.Count, "Queries do not remove doses.");
		Assert.IsTrue(DrugPurgeRules.Purge(f.Actor.Object, drug, Family));
		Assert.AreEqual(3, active.Count); Assert.AreEqual(1, latent.Count);
		Assert.IsTrue(active.Any(x => x.Drug == drug && x.SpellOriginId is null));
		Assert.IsTrue(active.Any(x => x.SpellOriginFamily == "other"));
		Assert.AreSame(other, latent.Single().Drug);
		Assert.IsFalse(DrugPurgeRules.Purge(f.Actor.Object, drug, Family));
	}

	[TestMethod]
	public void AdditionalVenoms_RoundTripAndCureOnlyConfiguredDrugsAndExactPrimaryFamily()
	{
		var f = new MagicCastingFixture();
		var poison = Mock.Of<IDrug>(x => x.Id == 81 && x.Name == "owned toxin");
		var venom = Mock.Of<IDrug>(x => x.Id == 82 && x.Name == "venom");
		var medicine = Mock.Of<IDrug>(x => x.Id == 83 && x.Name == "medicine");
		f.World.SetupGet(x => x.Drugs).Returns(MagicCastingFixture.Collection(() => new[] { poison, venom, medicine }));
		var active = new List<DrugDosage> { Dose(poison, Family), Dose(poison), Dose(poison, "other"), Dose(venom), Dose(medicine) };
		var latent = new List<DrugDosage> { Dose(venom), Dose(medicine) }; SetupDoses(f, active, latent);
		SpellEffectFactory.SetupFactory();
		var xml = ArmageddonToxinSpellContent.Create(81).Single(x => x.Content.Key == ArmageddonToxinSpellContent.PurgeToxinKey)
			.Content.BuildDefinition(10, 1, 0).Element("Effects")!.Elements().Single();
		xml.Add(new XElement("AdditionalDrugs", new XElement("Drug", 82)));
		var effect = (RemovePoisonEffect)SpellEffectFactory.LoadEffect(xml, f.Spell);
		var clone = (RemovePoisonEffect)effect.Clone();
		Assert.AreEqual(82L, (long)clone.SaveToXml().Element("AdditionalDrugs")!.Element("Drug")!);
		Assert.IsTrue(clone.HasMatchingPoison(f.Actor.Object)); Assert.AreEqual(5, active.Count);
		var result = clone.Apply(f.Actor.Object, f.Actor.Object, OpposedOutcomeDegree.None, SpellPower.Standard,
			new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object), []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(3, active.Count); Assert.AreEqual(1, latent.Count);
		Assert.IsTrue(active.Any(x => x.SpellOriginFamily == "other"));
		Assert.IsTrue(active.Any(x => x.Drug == poison && x.SpellOriginId is null));
		Assert.IsTrue(active.Any(x => x.Drug == medicine)); Assert.AreSame(medicine, latent.Single().Drug);
		Assert.IsFalse(clone.HasMatchingPoison(f.Actor.Object));
	}

	[TestMethod]
	public void DrugPurge_StopsWhenAnEffectCallbackChangesThePhysicalBody()
	{
		var f = new MagicCastingFixture(); var drug = Mock.Of<IDrug>(x => x.Id == 81);
		var effect = new SpellPoisonEffect(f.Actor.Object, new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object),
			drug, DrugVector.Touched, 1, "0.001*grade", ownedFamily: Family);
		SetupDoses(f, [Dose(drug, Family)], []);
		f.Actor.Setup(x => x.EffectsOfType<SpellPoisonEffect>(It.IsAny<Predicate<SpellPoisonEffect>>())).Returns([effect]);
		f.Actor.Setup(x => x.RemoveEffect(effect, true)).Callback(() => f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()));
		Assert.ThrowsExactly<InvalidOperationException>(() => DrugPurgeRules.Purge(f.Actor.Object, drug, Family));
		f.Body.Verify(x => x.RemoveDrugDosages(It.IsAny<Predicate<DrugDosage>>()), Times.Never);
	}

	[TestMethod]
	public void OrderedCure_RemovesOnlyFirstPresentFamilyIncludingAllStagesAndAftereffects()
	{
		var f = new MagicCastingFixture();
		var drugs = Enumerable.Range(81, 5).Select(id => Mock.Of<IDrug>(x => x.Id == id)).ToArray();
		f.World.SetupGet(x => x.Drugs).Returns(MagicCastingFixture.Collection(() => drugs));
		var active = new List<DrugDosage> { Dose(drugs[1]), Dose(drugs[2]), Dose(drugs[3]), Dose(drugs[4]) };
		SetupDoses(f, active, []);
		var aftermath = new List<IDrugAftereffect> { new TimedMaximumHitPointsEffect(f.Body.Object, -5, 82) };
		f.Body.Setup(x => x.EffectsOfType<IDrugAftereffect>(It.IsAny<Predicate<IDrugAftereffect>>())).Returns(() => aftermath);
		f.Body.Setup(x => x.RemoveEffect(It.IsAny<IDrugAftereffect>(), true)).Callback< MudSharp.Effects.IEffect, bool>((effect, _) => aftermath.Remove((IDrugAftereffect)effect));
		SpellEffectFactory.SetupFactory();
		var xml = ArmageddonToxinSpellContent.Create(81).Single(x => x.Content.Key == ArmageddonToxinSpellContent.PurgeToxinKey).Content.BuildDefinition(10, 1, 0).Element("Effects")!.Elements().Single();
		xml.Add(new XElement("DrugGroups", new XElement("Group", new XElement("Drug", 81)),
			new XElement("Group", new XElement("Drug", 82), new XElement("Drug", 83), new XElement("Drug", 84)),
			new XElement("Group", new XElement("Drug", 85))), new XElement("FirstMatchingGroup", true), new XElement("RemoveAftereffects", true));
		var cure = (RemovePoisonEffect)SpellEffectFactory.LoadEffect(xml, f.Spell);
		var captured = cure.CapturePreparedSelection(f.Actor.Object, f.Actor.Object)!;
		var clone = (RemovePoisonEffect)cure.Clone();
		Assert.IsTrue(clone.TryReusePreparedSelection(captured, f.Actor.Object, f.Actor.Object, out _));
		var result = clone.Apply(f.Actor.Object, f.Actor.Object, OpposedOutcomeDegree.None, SpellPower.Standard, new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object), []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(85L, active.Single().Drug.Id);
		Assert.AreEqual(0, aftermath.Count);
	}

	[TestMethod]
	public void Aftereffects_AreOptInAndCanBeRemovedAfterTheDrugHasCleared()
	{
		var f = new MagicCastingFixture(); var drug = Mock.Of<IDrug>(x => x.Id == 81);
		SetupDoses(f, [], []);
		var aftermath = new List<IDrugAftereffect> { new TimedMaximumHitPointsEffect(f.Body.Object, -5, 81) };
		f.Body.Setup(x => x.EffectsOfType<IDrugAftereffect>(It.IsAny<Predicate<IDrugAftereffect>>())).Returns(() => aftermath);
		f.Body.Setup(x => x.RemoveEffect(It.IsAny<IDrugAftereffect>(), true)).Callback<MudSharp.Effects.IEffect, bool>((effect, _) => aftermath.Remove((IDrugAftereffect)effect));
		Assert.IsFalse(DrugPurgeRules.Purge(f.Actor.Object, drug, null));
		Assert.AreEqual(1, aftermath.Count);
		Assert.IsTrue(DrugPurgeRules.HasMatching(f.Actor.Object, drug, null, true));
		Assert.IsTrue(DrugPurgeRules.Purge(f.Actor.Object, drug, null, true));
		Assert.AreEqual(0, aftermath.Count);
	}

	private static void SetupDoses(MagicCastingFixture f, List<DrugDosage> active, List<DrugDosage> latent)
	{
		f.Body.SetupGet(x => x.ActiveDrugDosages).Returns(() => active);
		f.Body.SetupGet(x => x.LatentDrugDosages).Returns(() => latent);
		f.Actor.Setup(x => x.EffectsOfType<SpellPoisonEffect>(It.IsAny<Predicate<SpellPoisonEffect>>())).Returns([]);
		f.Body.Setup(x => x.RemoveDrugDosages(It.IsAny<Predicate<DrugDosage>>()))
			.Callback<Predicate<DrugDosage>>(p => { active.RemoveAll(p); latent.RemoveAll(p); });
	}
}
