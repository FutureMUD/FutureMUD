#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.Health.Infections;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonConditionSpellContentTests
{
	[TestMethod]
	public void Factory_DiseasePairRoundTripsOwnedModeAndLeavesLegacyAbsent()
	{
		var f = new MagicCastingFixture();
		SpellEffectFactory.SetupFactory();
		foreach (var contribution in ArmageddonConditionSpellContent.Create(new(5, 2)))
		{
			var source = contribution.Content.BuildDefinition(10, 1, 0).Element("Effects")!.Elements().Single();
			var template = SpellEffectFactory.LoadEffect(source, f.Spell);
			Assert.IsTrue(XNode.DeepEquals(source, template.SaveToXml()));
			Assert.IsTrue(XNode.DeepEquals(source, template.Clone().SaveToXml()));
			Assert.AreEqual("arm.effect.disease", (string?)source.Element("OwnedFamily"));
			Assert.AreEqual((int)DrugVector.Touched, (int)source.Element("ExposureVector")!);
			var legacy = new XElement(source);
			legacy.Element("OwnedFamily")!.Remove(); legacy.Element("ExposureVector")!.Remove();
			Assert.IsNull(SpellEffectFactory.LoadEffect(legacy, f.Spell).SaveToXml().Element("OwnedFamily"));
		}
	}

	[DataTestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void SowSickness_CreatesRealNativeNonspreadingInfectionWithoutIdentityFlush(int grade)
	{
		var f = new Fixture();
		f.Base.Body.Setup(x => x.AddInfection(It.IsAny<IInfection>())).Callback<IInfection>(f.Infections.Add);
		var template = f.Template("sow_sickness");
		((DiseaseEffect)template).IntensityExpression = new MudSharp.Body.Traits.TraitExpression(grade.ToString(), f.Base.World.Object);
		var effect = (SpellDiseaseEffect)template.GetOrApplyEffect(f.Base.Actor.Object, f.Base.Actor.Object,
			OpposedOutcomeDegree.Moderate, SpellPower.ExtremelyStrong, f.Parent, [])!;
		effect.InitialEffect(); effect.InitialEffect();
		Assert.AreEqual(1, f.Infections.Count);
		var infection = (SownSicknessInfection)f.Infections.Single();
		Assert.AreEqual(grade, infection.Grade);
		Assert.AreEqual((double)grade, infection.Intensity);
		Assert.AreEqual(1.0, infection.Virulence);
		Assert.IsFalse(infection.InfectionCanSpread());
		Assert.AreSame(f.Base.Body.Object, infection.Owner);
		Assert.AreSame(infection, effect.AppliedInfection);
		Mock.Get(f.Base.World.Object.SaveManager).Verify(x => x.Flush(), Times.Never);
		infection.InitialiseWithoutPersistence(91);
		Assert.AreEqual(91L, effect.AppliedInfectionId);
		Assert.IsTrue(f.Base.Actor.Object.EffectsChanged);
		var saved = effect.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		Assert.AreEqual(91L, (long)saved.Element("Effect")!.Element("AppliedInfectionId")!);
		Assert.AreEqual(200L, (long)saved.Element("Effect")!.Element("OwnedBodyId")!);
		Assert.IsTrue(f.Base.Body.Object.Changed);
	}

	[TestMethod]
	public void SowSickness_PreparedBodyChangeRefusesBeforeNativeExposure()
	{
		var f = new Fixture(); var template = f.Template("sow_sickness");
		var prepared = (IMagicSpellEffectPreparedSelection)template;
		var token = prepared.CapturePreparedSelection(f.Base.Actor.Object, f.Base.Actor.Object)!;
		f.ChangeBody();
		Assert.IsFalse(prepared.TryReusePreparedSelection(token, f.Base.Actor.Object, f.Base.Actor.Object, out _));
		Assert.ThrowsException<InvalidOperationException>(() => template.GetOrApplyEffect(f.Base.Actor.Object,
			f.Base.Actor.Object, OpposedOutcomeDegree.Moderate, SpellPower.Standard, f.Parent, []));
		f.Base.Body.Verify(x => x.AddInfection(It.IsAny<IInfection>()), Times.Never);
	}

	[TestMethod]
	public void SowSickness_NativeExposureCallbackBodyChangeRetainsOriginalInfectionAndDirtyBody()
	{
		var f = new Fixture();
		f.Base.Body.Setup(x => x.AddInfection(It.IsAny<IInfection>())).Callback<IInfection>(x =>
		{ f.Infections.Add(x); f.ChangeBody(); });
		var template = (DiseaseEffect)f.Template("sow_sickness");
		template.IntensityExpression = new MudSharp.Body.Traits.TraitExpression("1", f.Base.World.Object);
		var effect = (SpellDiseaseEffect)template.GetOrApplyEffect(f.Base.Actor.Object,
			f.Base.Actor.Object, OpposedOutcomeDegree.Moderate, SpellPower.Standard, f.Parent, [])!;
		Assert.ThrowsException<InvalidOperationException>(effect.InitialEffect);
		Assert.AreSame(f.Infections.Single(), effect.AppliedInfection);
		Assert.AreSame(f.Base.Body.Object, effect.OwnedBody);
		Assert.IsTrue(f.Base.Body.Object.Changed);
	}

	[TestMethod]
	public void PurgeDisease_RemovesOnlyOwnedFamilyExactInfectionAndNoopsWhenAbsent()
	{
		var f = new Fixture();
		var owned = f.LoadOwned(91, "arm.effect.disease");
		var unrelated = f.LoadOwned(92, "other.family");
		var natural = f.Infection(93);
		f.Effects.Add(owned); f.Effects.Add(unrelated); f.Infections.Add(natural.Object);
		var cure = (IMagicSpellEffectOperation)f.Template("purge_disease");
		Assert.AreEqual(MagicEffectOperationStatus.Applied, cure.Apply(f.Base.Actor.Object, f.Base.Actor.Object,
			OpposedOutcomeDegree.Moderate, SpellPower.ExtremelyStrong, f.Parent, []).Status);
		Assert.IsFalse(f.Effects.Contains(owned)); Assert.IsTrue(f.Effects.Contains(unrelated));
		Assert.IsFalse(f.Infections.Any(x => x.Id == 91));
		Assert.IsTrue(f.Infections.Any(x => x.Id == 92)); Assert.IsTrue(f.Infections.Contains(natural.Object));
		Mock.Get(owned.AppliedInfection ?? f.Deleted.Single()).Verify(x => x.Delete(), Times.Once);
		natural.Verify(x => x.Delete(), Times.Never);
		Assert.AreEqual(MagicEffectOperationStatus.NoChange, cure.Apply(f.Base.Actor.Object, f.Base.Actor.Object,
			OpposedOutcomeDegree.Moderate, SpellPower.Standard, f.Parent, []).Status);
	}

	[TestMethod]
	public void PurgeDisease_RemovalCallbackBodyChangeThrowsAfterExactCapturedBodyCleanup()
	{
		var f = new Fixture(); var effect = f.LoadOwned(91, "arm.effect.disease"); f.Effects.Add(effect);
		f.Base.Body.Setup(x => x.RemoveInfection(It.IsAny<IInfection>())).Callback<IInfection>(x =>
		{ f.Infections.Remove(x); f.ChangeBody(); });
		var cure = (IMagicSpellEffectOperation)f.Template("purge_disease");
		Assert.ThrowsException<InvalidOperationException>(() => cure.Apply(f.Base.Actor.Object, f.Base.Actor.Object,
			OpposedOutcomeDegree.Moderate, SpellPower.Standard, f.Parent, []));
		Assert.AreEqual(0, f.Infections.Count); Assert.AreEqual(1, f.Deleted.Count);
		Assert.IsTrue(f.Base.Body.Object.Changed);
	}

	[TestMethod]
	public void Reload_MissingExactNativeInfectionRefusesReexposure()
	{
		var f = new Fixture(); var original = f.LoadOwned(91, "arm.effect.disease");
		var saved = original.SaveToXml(new Dictionary<IEffect, TimeSpan>()); f.Infections.Clear();
		var loaded = (SpellDiseaseEffect)Effect.LoadEffect(saved, f.Base.Actor.Object);
		loaded.ParentEffect = f.Parent;
		Assert.ThrowsException<InvalidOperationException>(loaded.InitialEffect);
		f.Base.Body.Verify(x => x.AddInfection(It.IsAny<IInfection>()), Times.Never);
	}

	[TestMethod]
	public void Reload_DifferentBodyPreservesOriginalOwnershipIdentityAndRefusesExposure()
	{
		var f = new Fixture(); var original = f.LoadOwned(91, "arm.effect.disease");
		var saved = original.SaveToXml(new Dictionary<IEffect, TimeSpan>()); f.ChangeBody();
		var loaded = (SpellDiseaseEffect)Effect.LoadEffect(saved, f.Base.Actor.Object); loaded.ParentEffect = f.Parent;
		Assert.IsNull(loaded.OwnedBody); Assert.IsNull(loaded.AppliedInfection);
		Assert.ThrowsException<InvalidOperationException>(loaded.InitialEffect);
		Assert.AreEqual(200L, (long)loaded.SaveToXml(new Dictionary<IEffect, TimeSpan>()).Element("Effect")!.Element("OwnedBodyId")!);
		Assert.AreEqual(1, f.Infections.Count);
	}

	[TestMethod]
	public void Expiry_AfterLiveBodyChangeStillCleansOriginalBodyOnly()
	{
		var f = new Fixture(); var original = f.LoadOwned(91, "arm.effect.disease");
		f.ChangeBody(); original.RemovalEffect();
		Assert.AreEqual(0, f.Infections.Count); Assert.AreEqual(1, f.Deleted.Count);
		Mock.Get(f.Base.Actor.Object.Body).Verify(x => x.RemoveInfection(It.IsAny<IInfection>()), Times.Never);
		Assert.IsTrue(f.Base.Body.Object.Changed);
	}

	[DataTestMethod]
	[DataRow("0")]
	[DataRow("8")]
	[DataRow("-1")]
	[DataRow("1/0")]
	public void OwnedMode_InvalidIntensityRefusesBeforeNativeExposure(string formula)
	{
		var f = new Fixture(); var template = (DiseaseEffect)f.Template("sow_sickness");
		template.IntensityExpression = new MudSharp.Body.Traits.TraitExpression(formula, f.Base.World.Object);
		Assert.ThrowsException<InvalidOperationException>(() => template.GetOrApplyEffect(f.Base.Actor.Object,
			f.Base.Actor.Object, OpposedOutcomeDegree.Moderate, SpellPower.Standard, f.Parent, []));
		Assert.AreEqual(0, f.Infections.Count);
	}

	[TestMethod]
	public void LegacyDisease_AbsentOwnershipStillUsesNativeBuilderConfiguration()
	{
		var f = new Fixture(); var source = new XElement("Effect", new XAttribute("type", "disease"),
			new XElement("InfectionType", (int)InfectionType.Simple), new XElement("VirulenceDifficulty", (int)Difficulty.Hard),
			new XElement("IntensityFormula", "2.5"), new XElement("VirulenceFormula", "0.4"));
		var template = (DiseaseEffect)SpellEffectFactory.LoadEffect(source, f.Base.Spell);
		Assert.IsNull(template.DefinitionError);
		var report = template.Apply(f.Base.Actor.Object, f.Base.Actor.Object, OpposedOutcomeDegree.Moderate, SpellPower.Standard, f.Parent, []);
		Assert.AreEqual(MagicEffectOperationStatus.Unknown, report.Status);
		var effect = (SpellDiseaseEffect)report.Effect!;
		Assert.IsNull(effect.OwnedFamily); Assert.AreEqual(2.5, effect.Intensity); Assert.AreEqual(0.4, effect.Virulence);
		Assert.IsNull(effect.SaveToXml(new Dictionary<IEffect, TimeSpan>()).Element("Effect")!.Element("OwnedFamily"));
	}

	[TestMethod]
	public void Reload_ParentKeepsRemainingLifetimeAndExpiryDeletesOnlyTrackedNativeInfection()
	{
		var f = new Fixture(); var clock = new TestClock(f.Base.Now); using var time = RuntimeClock.Push(clock);
		var scheduler = new EffectScheduler(f.Base.World.Object, clock); f.Base.World.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var child = f.LoadOwned(91, "arm.effect.disease"); f.Parent.AddSpellEffect(child);
		scheduler.AddSchedule(new EffectSchedule(f.Parent, TimeSpan.FromSeconds(420)));
		clock.Advance(TimeSpan.FromSeconds(50));
		var saved = f.Parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [f.Parent] = scheduler.RemainingDuration(f.Parent) });
		scheduler.Unschedule(f.Parent);
		var loaded = (MagicSpellParent)Effect.LoadEffect(saved, f.Base.Actor.Object);
		var loadedChild = (SpellDiseaseEffect)loaded.SpellEffects.Single();
		Assert.AreSame(f.Infections.Single(), loadedChild.AppliedInfection); Assert.AreEqual("arm.effect.disease", loadedChild.OwnedFamily);
		Assert.AreSame(f.Base.Body.Object, loadedChild.OwnedBody); Assert.AreEqual(DrugVector.Touched, loadedChild.ExposureVector);
		f.Effects.Add(loaded); scheduler.AddSchedule(new EffectSchedule(loaded, TimeSpan.FromMilliseconds((long)saved.Element("Remaining")!)));
		Assert.AreEqual(TimeSpan.FromSeconds(370), scheduler.RemainingDuration(loaded));
		clock.Advance(TimeSpan.FromSeconds(369)); scheduler.CheckSchedules(); Assert.AreEqual(1, f.Infections.Count);
		clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules(); Assert.AreEqual(0, f.Infections.Count);
		Assert.AreEqual(1, f.Deleted.Count);
	}

	private sealed class TestClock(DateTime now) : TimeProvider
	{
		private DateTime _now = now;
		public override DateTimeOffset GetUtcNow() => new(_now);
		public void Advance(TimeSpan duration) => _now += duration;
	}
	private sealed class Fixture
	{
		public MagicCastingFixture Base { get; } = new();
		public List<IInfection> Infections { get; } = [];
		public List<IInfection> Deleted { get; } = [];
		public List<IEffect> Effects { get; } = [];
		public MagicSpellParent Parent { get; }
		public Fixture()
		{
			SpellEffectFactory.SetupFactory(); SpellDiseaseEffect.InitialiseEffectType(); MagicSpellParent.InitialiseEffectType();
			Base.Body.SetupProperty(x => x.Changed); Base.Actor.SetupProperty(x => x.Changed);
			Base.Actor.SetupProperty(x => x.EffectsChanged);
			Base.Body.SetupGet(x => x.PartInfections).Returns(() => Infections);
			Base.Body.Setup(x => x.RemoveInfection(It.IsAny<IInfection>())).Callback<IInfection>(x => Infections.Remove(x));
			Base.Actor.SetupGet(x => x.Effects).Returns(() => Effects);
			Base.Actor.Setup(x => x.EffectsOfType<SpellDiseaseEffect>(It.IsAny<Predicate<SpellDiseaseEffect>>()))
				.Returns<Predicate<SpellDiseaseEffect>>(predicate => Effects.OfType<SpellDiseaseEffect>().Where(x => predicate is null || predicate(x)));
			Base.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Base.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) =>
			{ Effects.Remove(effect); if (fire) effect.RemovalEffect(); Base.World.Object.EffectScheduler.Unschedule(effect); });
			Parent = new(Base.Actor.Object, Base.Spell, Base.Actor.Object);
		}
		public IMagicSpellEffectTemplate Template(string suffix) => SpellEffectFactory.LoadEffect(
			ArmageddonConditionSpellContent.Create(new(5, 2)).Single(x => x.Content.Key == "arm.spell." + suffix)
				.Content.BuildDefinition(10, 1, 0).Element("Effects")!.Elements().Single(), Base.Spell);
		public void ChangeBody() => Base.Actor.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object);
		public Mock<IInfection> Infection(long id)
		{
			var infection = new Mock<IInfection>(); infection.SetupGet(x => x.Id).Returns(id);
			infection.SetupGet(x => x.InfectionType).Returns(InfectionType.SownSickness);
			infection.Setup(x => x.Delete()).Callback(() => Deleted.Add(infection.Object)); return infection;
		}
		public SpellDiseaseEffect LoadOwned(long id, string family)
		{
			var infection = Infection(id); Infections.Add(infection.Object);
			var source = new SpellDiseaseEffect(Base.Actor.Object, Parent, InfectionType.SownSickness, Difficulty.Normal,
				1, 1, "grade", "1", ownedFamily: family, exposureVector: DrugVector.Touched, ownedBody: Base.Body.Object)
				.SaveToXml(new Dictionary<IEffect, TimeSpan>());
			source.Element("Effect")!.Element("AppliedInfectionId")!.Value = id.ToString();
			var loaded = (SpellDiseaseEffect)Effect.LoadEffect(source, Base.Actor.Object); loaded.ParentEffect = Parent; return loaded;
		}
	}
}
