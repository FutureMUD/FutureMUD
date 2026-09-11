using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicalSubstanceTests
{
	private sealed class Fixture
	{
		public Mock<IFuturemud> World = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Actor = new();
		public Mock<IBody> Body = new();
		public Mock<ILiquid> Liquid = new();
		public Mock<IMagicalSubstance> Substance = new();
		public List<IEffect> Effects = new();
		public SubstanceEffectEntry Entry = new() { SpellId = 2 };
		public MagicSpell Spell;
		public double Stamina;
		public Fixture(string effects = "<Effect type='staminadelta'><Formula>10</Formula></Effect>", string trigger = "substancecharacter")
		{
			var school = Mock.Of<IMagicSchool>(x => x.Id == 1);
			World.SetupGet(x => x.MagicSchools).Returns(Collection(school));
			World.SetupGet(x => x.FutureProgs).Returns(Collection<IFutureProg>());
			World.SetupGet(x => x.Traits).Returns(Collection<ITraitDefinition>());
			World.SetupGet(x => x.TraitExpressions).Returns(Collection<ITraitExpression>());
			World.SetupGet(x => x.MagicResources).Returns(Collection<IMagicResource>());
			Spell = new MagicSpell(new MudSharp.Models.MagicSpell { Id = 2, Name = "Test Payload", MagicSchoolId = 1,
				Definition = $"<Definition><Trigger type='{trigger}'/><Costs/><Effects>{effects}</Effects><CasterEffects/><Plan/></Definition>" }, World.Object);
			World.SetupGet(x => x.MagicSpells).Returns(Collection<IMagicSpell>(Spell));
			Substance.SetupGet(x => x.Id).Returns(3);
			World.SetupGet(x => x.MagicalSubstances).Returns(Collection(Substance.Object));
			Substance.SetupGet(x => x.Id).Returns(3); Substance.SetupGet(x => x.Name).Returns("Test Potion");
			Substance.SetupGet(x => x.Gameworld).Returns(World.Object);
			Substance.SetupGet(x => x.Vectors).Returns(DrugVector.Ingested | DrugVector.Touched | DrugVector.Injected | DrugVector.Inhaled);
			Substance.SetupGet(x => x.Power).Returns(SpellPower.Standard);
			Substance.SetupGet(x => x.ReferenceDose).Returns(1);
			Substance.SetupGet(x => x.ClearancePerTick).Returns(0.01);
			Substance.SetupGet(x => x.Entries).Returns(new[] { Entry });
			Substance.SetupGet(x => x.Bindings).Returns(new[] { new SubstanceBinding(SubstanceCarrier.Liquid, 4, 1) });
			Substance.SetupGet(x => x.ReadinessErrors).Returns(() => SubstanceSpellResolver.Errors(Spell, Entry));
			Liquid.SetupGet(x => x.Id).Returns(4); Liquid.SetupGet(x => x.Density).Returns(1);
			World.SetupGet(x => x.Liquids).Returns(Collection(Liquid.Object));
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object); Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Body.SetupGet(x => x.SurfaceLiquidState).Returns(new SurfaceLiquidState(World.Object));
			Body.SetupGet(x => x.Gameworld).Returns(World.Object); Body.SetupGet(x => x.Actor).Returns(Actor.Object);
			Actor.Setup(x => x.GainStamina(It.IsAny<double>())).Callback<double>(x => Stamina += x);
			Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns(Array.Empty<IMagicInterdictionEffect>());
			Actor.Setup(x => x.EffectsOfType<SubstanceExposureEffect>(It.IsAny<Predicate<SubstanceExposureEffect>>())).Returns(() => Effects.OfType<SubstanceExposureEffect>().ToList());
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => Effects.Add(effect));
			Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { if (Effects.Remove(effect) && fire) effect.RemovalEffect(); });
		}
		public LiquidMixture Mix(double amount) => new(Liquid.Object, amount, World.Object);
		private static All<T> Collection<T>(params T[] items) where T : class, IFrameworkItem
		{ var result = new All<T>(); foreach (var item in items) result.Add(item); return result; }
	}
	[TestMethod]
	public void Activation_FractionalSips_EqualWholeDoseAndDoNotUseCasterTraits()
	{
		var f = new Fixture();
		for (var i = 0; i < 10; i++) MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.1), DrugVector.Ingested);
		Assert.AreEqual(10, f.Stamina, 1e-9);
		Assert.IsFalse(f.Spell.CharacterKnowsSpell(f.Actor.Object)); Assert.IsTrue(f.Spell.ReadyForGame);
	}
	[TestMethod]
	public void Activation_TransferSpentLiquid_DoesNotActivateTwice()
	{
		var f = new Fixture(); var mix = f.Mix(1);
		MagicalExposure.Liquid(f.Actor.Object, mix, DrugVector.Touched);
		MagicalExposure.Liquid(f.Actor.Object, mix.Clone(), DrugVector.Ingested);
		Assert.AreEqual(10, f.Stamina, 1e-9);
	}
	[TestMethod]
	public void Activation_OneActionAcrossParts_ChecksTotalExposureOnce()
	{
		var f = new Fixture(); f.Entry.MinimumDose = 1;
		using (MagicalExposure.BeginExposure())
		{
			MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.4), DrugVector.Touched);
			MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.6), DrugVector.Touched);
		}
		Assert.AreEqual(10, f.Stamina, 1e-9);
	}
	[TestMethod]
	public void Activation_SeparateSubthresholdActions_DoNotAccumulateCredit()
	{
		var f = new Fixture(); f.Entry.MinimumDose = 1;
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.5), DrugVector.Touched);
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.5), DrugVector.Touched);
		Assert.AreEqual(0, f.Stamina, 1e-9);
	}
	[TestMethod]
	public void Pulse_Timed_DoesNotBurstOnExposureOrReplayMissedTicks()
	{
		var f = new Fixture(); f.Entry.Lifecycle = SubstanceLifecycle.Periodic; f.Entry.PulseMode = SubstancePulseMode.Timed;
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(1), DrugVector.Ingested);
		Assert.AreEqual(0, f.Stamina);
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		parent.Advance(TimeSpan.FromSeconds(10)); Assert.AreEqual(10, f.Stamina);
		parent.Advance(TimeSpan.FromHours(1)); Assert.AreEqual(20, f.Stamina);
	}
	[TestMethod]
	public void Maintained_Absorption_UpdatesExistingChildAndDispelRemovesParent()
	{
		var f = new Fixture("<Effect type='healingrate'><Multiplier>2</Multiplier><Stages>0</Stages></Effect>");
		f.Entry.Lifecycle = SubstanceLifecycle.Maintained;
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(1), DrugVector.Injected);
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		parent.Advance(TimeSpan.FromSeconds(10));
		var child = f.Effects.OfType<SpellHealingRateEffect>().Single();
		Assert.AreEqual(1.49, child.HealingRateMultiplier, 1e-9);
		parent.Advance(TimeSpan.FromSeconds(10)); Assert.AreSame(child, f.Effects.OfType<SpellHealingRateEffect>().Single());
		f.Actor.Object.RemoveEffect(child, true);
		Assert.IsFalse(f.Effects.OfType<SubstanceExposureEffect>().Any());
	}
	[TestMethod]
	public void Readiness_InstantaneousMaintainedEffect_IsRejected()
	{
		var f = new Fixture(); f.Entry.Lifecycle = SubstanceLifecycle.Maintained;
		Assert.IsTrue(f.Substance.Object.ReadinessErrors.Any());
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(1), DrugVector.Ingested);
		Assert.AreEqual(0, f.Stamina);
	}
	[TestMethod]
	public void Parent_Reload_DoesNotRepeatActivation()
	{
		var f = new Fixture(); f.Entry.Lifecycle = SubstanceLifecycle.Periodic; f.Entry.PulseMode = SubstancePulseMode.Timed;
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(1), DrugVector.Ingested);
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		SubstanceExposureEffect.InitialiseEffectType();
		var xml = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromSeconds(1) });
		var restored = (SubstanceExposureEffect)Effect.LoadEffect(XElement.Parse(xml.ToString()), f.Actor.Object);
		Assert.AreEqual(parent.RemainingSeconds, restored.RemainingSeconds);
		Assert.AreEqual(0, f.Stamina); Assert.IsNull(restored.Caster);
	}
	[TestMethod]
	public void Ward_BlockedActivationSpendsCharge_AndFreshDoseStillWorks()
	{
		var f = new Fixture(); var mix = f.Mix(1);
		var ward = new Mock<IMagicInterdictionEffect>();
		ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.Setup(x => x.ShouldInterdict(It.IsAny<ICharacter>(), It.IsAny<IMagicSchool>())).Returns(true);
		f.Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns(new[] { ward.Object });
		MagicalExposure.Liquid(f.Actor.Object, mix, DrugVector.Ingested);
		Assert.AreEqual(0, f.Stamina);
		Assert.IsTrue(mix.Instances.Single().MagicalCharges[3].Spent.Contains(f.Entry.Key));
		f.Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns(Array.Empty<IMagicInterdictionEffect>());
		MagicalExposure.Liquid(f.Actor.Object, mix.Clone(), DrugVector.Ingested);
		Assert.AreEqual(0, f.Stamina);
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(1), DrugVector.Ingested);
		Assert.AreEqual(10, f.Stamina);
	}
	[TestMethod]
	public void Coating_RemovalEndsMaintainedEffects_AndDispelSuppressesRetainedLiquid()
	{
		var f = new Fixture("<Effect type='healingrate'><Multiplier>2</Multiplier><Stages>0</Stages></Effect>");
		f.Entry.Lifecycle = SubstanceLifecycle.Maintained;
		var mix = f.Mix(1);
		using (MagicalExposure.BeginExposure())
		{
			MagicalExposure.Liquid(f.Actor.Object, mix, DrugVector.Touched, true);
			f.Body.Object.SurfaceLiquidState.AddLiquid(mix);
		}
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		parent.Advance(TimeSpan.FromSeconds(1));
		Assert.AreEqual(2, f.Effects.OfType<SpellHealingRateEffect>().Single().HealingRateMultiplier);
		f.Actor.Object.RemoveEffect(parent, true);
		var transfer = f.Body.Object.SurfaceLiquidState.RemoveLiquidVolume(1)!;
		Assert.IsTrue(transfer.Instances.Single().MagicalCharges[3].Suppressed.Contains(f.Entry.Key));
		MagicalExposure.Liquid(f.Actor.Object, transfer, DrugVector.Touched, true);
		Assert.IsFalse(f.Effects.OfType<SubstanceExposureEffect>().Any());
		using (MagicalExposure.BeginExposure())
		{
			var fresh = f.Mix(1);
			MagicalExposure.Liquid(f.Actor.Object, fresh, DrugVector.Touched, true);
			f.Body.Object.SurfaceLiquidState.AddLiquid(fresh);
		}
		parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		parent.Advance(TimeSpan.FromSeconds(1));
		f.Body.Object.SurfaceLiquidState.RemoveLiquidVolume(1);
		parent.Advance(TimeSpan.FromSeconds(1));
		Assert.IsFalse(f.Effects.OfType<SubstanceExposureEffect>().Any());
		f.Body.Verify(x => x.ResolveSurfaceLiquidDrying(), Times.AtLeastOnce);
	}
	[TestMethod]
	public void TimedDuration_SmallDoseSchedulesExactExpiry_AndDurationCapApplies()
	{
		var f = new Fixture("<Effect type='healingrate'><Multiplier>2</Multiplier><Stages>0</Stages></Effect>");
		f.Entry.DurationSeconds = 60; f.Entry.MaximumDurationSeconds = 90;
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.001), DrugVector.Ingested);
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		Assert.AreEqual(0.06, parent.RemainingSeconds, 1e-9);
		f.Actor.Verify(x => x.Reschedule(parent, It.Is<TimeSpan>(t => Math.Abs(t.TotalSeconds - 0.06) < 1e-9)), Times.AtLeastOnce);
		parent.SetRemaining(TimeSpan.FromHours(1));
		Assert.AreEqual(90, parent.RemainingSeconds);
		parent.SetRemaining(TimeSpan.Zero);
		Assert.IsFalse(f.Effects.OfType<SubstanceExposureEffect>().Any());
	}
	[DataTestMethod]
	[DataRow(SubstanceStacking.Aggregate, 4.0, 1)]
	[DataRow(SubstanceStacking.Replace, 3.0, 1)]
	[DataRow(SubstanceStacking.Strongest, 3.0, 1)]
	[DataRow(SubstanceStacking.Independent, 5.0, 2)]
	public void Stacking_FollowsConfiguredPolicy(SubstanceStacking stacking, double expected, int parents)
	{
		var f = new Fixture("<Effect type='healingrate'><Multiplier>2</Multiplier><Stages>0</Stages></Effect>");
		f.Entry.Stacking = stacking; f.Entry.Scaling = SubstanceScaling.Magnitude;
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(1), DrugVector.Ingested);
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(2), DrugVector.Ingested);
		Assert.AreEqual(parents, f.Effects.OfType<SubstanceExposureEffect>().Count());
		Assert.AreEqual(expected, f.Effects.OfType<SpellHealingRateEffect>().Sum(x => x.HealingRateMultiplier), 1e-9);
	}

	[TestMethod]
	public void ItemOil_UpdatesSameChildAsCoatingDrains()
	{
		var f = new Fixture("<Effect type='weight'><Weight>10</Weight></Effect>", "substanceitem");
		f.Entry.Lifecycle = SubstanceLifecycle.Maintained;
		var item = new Mock<IGameItem>(); var effects = new List<IEffect>();
		var surface = new SurfaceLiquidState(f.World.Object);
		item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		item.SetupGet(x => x.SurfaceLiquidState).Returns(surface);
		item.Setup(x => x.EffectsOfType<SubstanceExposureEffect>(It.IsAny<Predicate<SubstanceExposureEffect>>())).Returns(() => effects.OfType<SubstanceExposureEffect>().ToList());
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => effects.Add(effect));
		item.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { if (effects.Remove(effect) && fire) effect.RemovalEffect(); });
		using (MagicalExposure.BeginExposure())
		{
			var mix = f.Mix(1); MagicalExposure.Liquid(item.Object, mix, DrugVector.Touched, true); surface.AddLiquid(mix);
		}
		var parent = effects.OfType<SubstanceExposureEffect>().Single();
		parent.Advance(TimeSpan.FromSeconds(1));
		var child = effects.OfType<SpellWeightEffect>().Single();
		Assert.AreEqual(10, child.AddedWeight);
		surface.RemoveLiquidVolume(0.75);
		parent.Advance(TimeSpan.FromSeconds(1));
		Assert.AreSame(child, effects.OfType<SpellWeightEffect>().Single());
		Assert.AreEqual(2.5, child.AddedWeight, 1e-9);
	}
	private sealed class FoodProto(IFuturemud world) : PreparedFoodGameItemComponentProto(
		new MudSharp.Models.GameItemComponentProto { Id = 5, Name = "Test food", Definition = "", EditableItem = new MudSharp.Models.EditableItem() }, world);
	[TestMethod]
	public void Food_PartialBitesAndStackServings_ConserveAbsorbedMagic()
	{
		var f = new Fixture();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		item.SetupGet(x => x.Prototype).Returns(Mock.Of<IGameItemProto>(x => x.Id == 6));
		var stack = new Mock<IStackable>(); stack.SetupProperty(x => x.Quantity, 2);
		item.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
		item.SetupGet(x => x.Quantity).Returns(() => stack.Object.Quantity);
		var proto = new FoodProto(f.World.Object); proto.Profile.Bites = 2; proto.Profile.ServingScope = FoodServingScope.PerStackUnit;
		var food = new PreparedFoodGameItemComponent(proto, item.Object, true);
		food.AddMagicalIngredients(f.Mix(1));
		for (var i = 0; i < 4; i++) food.Eat(f.Body.Object, 1);
		Assert.AreEqual(10, f.Stamina, 1e-9);
		item.Verify(x => x.Delete(), Times.Once);
	}

	[DataTestMethod]
	[DataRow("heal")]
	[DataRow("mend")]
	[Timeout(3000)]
	public void HealingOverflow_ConsumesEachWoundOnceAndTerminatesWhenDoseExceedsAllDamage(string type)
	{
		var f = new Fixture($"<Effect type='{type}'><HealWorstWoundsFirst>true</HealWorstWoundsFirst><HealOverflow>true</HealOverflow><HealingAmount>20</HealingAmount></Effect>");
		var first = new Mock<IWound>(); first.SetupProperty(x => x.CurrentDamage, 5.0);
		var second = new Mock<IWound>(); second.SetupProperty(x => x.CurrentDamage, 3.0);
		f.Body.SetupGet(x => x.Wounds).Returns(new[] { first.Object, second.Object });
		f.Actor.SetupGet(x => x.Wounds).Returns(new[] { first.Object, second.Object });
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(1), DrugVector.Ingested);
		Assert.AreEqual(0, first.Object.CurrentDamage);
		Assert.AreEqual(0, second.Object.CurrentDamage);
		if (type == "heal") f.Body.Verify(x => x.EvaluateWounds(), Times.Once);
		else f.Actor.Verify(x => x.EvaluateWounds(), Times.Once);
	}

	[TestMethod]
	public void FormulaScaling_DoesNotChangeAnEqualResourceIdentifier()
	{
		var f = new Fixture(); var resource = Mock.Of<IMagicResource>(x => x.Id == 10);
		var resources = new Mock<IUneditableAll<IMagicResource>>(); resources.Setup(x => x.Get(10)).Returns(resource);
		f.World.SetupGet(x => x.MagicResources).Returns(resources.Object);
		var effect = SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='magicresourcedelta'><Resource>10</Resource><Formula>10</Formula></Effect>"), f.Spell);
		SubstanceSpellResolver.Apply(effect, f.Spell, new SubstanceResolutionContext(f.Actor.Object, f.Substance.Object, 0.25),
			new MagicSpellParent(f.Actor.Object, f.Spell, null!), 0.25);
		f.Actor.Verify(x => x.AddResource(resource, 2.5), Times.Once);
		Assert.AreEqual("10", effect.SaveToXml().Element("Formula")!.Value);
	}

	[TestMethod]
	public void FormulaOverflow_SkipsPayloadWithoutThrowingOrGrantingNonFiniteResources()
	{
		var f = new Fixture("<Effect type='staminadelta'><Formula>1e308</Formula></Effect>");
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(10), DrugVector.Ingested);
		Assert.AreEqual(0, f.Stamina);
	}

	[TestMethod]
	public void ItemTransformation_IsRejectedBeforeExposure()
	{
		var f = new Fixture("<Effect type='transformform'/>", "substanceitem");
		Assert.IsTrue(f.Substance.Object.ReadinessErrors.Any(x => x.Contains("requires a character target")));
		var item = Mock.Of<IGameItem>(x => x.Gameworld == f.World.Object);
		MagicalExposure.Liquid(item, f.Mix(1), DrugVector.Touched);
		Assert.IsNull(SubstanceSpellResolver.Apply(f.Spell.SpellEffects.Single(), f.Spell,
			new SubstanceResolutionContext(item, f.Substance.Object, 1), new MagicSpellParent(item, f.Spell, null!), 1));
	}

	[DataTestMethod]
	[DataRow(SubstanceStacking.Strongest, SubstanceScaling.Duration)]
	[DataRow(SubstanceStacking.Replace, SubstanceScaling.Duration)]
	[DataRow(SubstanceStacking.Aggregate, SubstanceScaling.Magnitude)]
	[DataRow(SubstanceStacking.Independent, SubstanceScaling.Duration)]
	public void PersistentActivation_SplitActionHasWholeDoseLifetimeAndMagnitude(SubstanceStacking stacking, SubstanceScaling scaling)
	{
		var f = new Fixture("<Effect type='healingrate'><Multiplier>2</Multiplier><Stages>0</Stages></Effect>");
		f.Entry.Stacking = stacking; f.Entry.Scaling = scaling; f.Entry.MinimumDose = 1;
		using (MagicalExposure.BeginExposure())
		{
			MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.5), DrugVector.Ingested);
			MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.5), DrugVector.Ingested);
		}
		Assert.AreEqual(60, f.Effects.OfType<SubstanceExposureEffect>().Single().RemainingSeconds);
		Assert.AreEqual(2, f.Effects.OfType<SpellHealingRateEffect>().Single().HealingRateMultiplier);
	}

	[TestMethod]
	public void StrongestCoating_GroupsLotsAndSuppressesEveryRetainedPart()
	{
		var f = new Fixture("<Effect type='healingrate'><Multiplier>2</Multiplier><Stages>0</Stages></Effect>");
		f.Entry.Lifecycle = SubstanceLifecycle.Maintained; f.Entry.Stacking = SubstanceStacking.Strongest; f.Entry.MinimumDose = 1;
		using (MagicalExposure.BeginExposure())
		{
			foreach (var quantity in new[] { 0.4, 0.6 })
			{
				var mix = f.Mix(quantity);
				MagicalExposure.Liquid(f.Actor.Object, mix, DrugVector.Touched, true);
				f.Body.Object.SurfaceLiquidState.AddLiquid(mix);
			}
		}
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		parent.Advance(TimeSpan.FromSeconds(1));
		Assert.AreEqual(2, f.Effects.OfType<SpellHealingRateEffect>().Single().HealingRateMultiplier);
		f.Actor.Object.RemoveEffect(parent, true);
		Assert.IsTrue(MagicalExposure.RetainedLiquids(f.Actor.Object).All(x => x.Instance.MagicalCharges[3].Suppressed.Contains(f.Entry.Key)));
	}

	[TestMethod]
	public void TimedAggregate_ContinuousExposureKeepsBoundedSavedStateAndSurvivesReload()
	{
		var f = new Fixture(); f.Entry.Lifecycle = SubstanceLifecycle.Periodic; f.Entry.PulseMode = SubstancePulseMode.Timed;
		MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.1), DrugVector.Inhaled);
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		for (var i = 0; i < 1000; i++)
		{
			parent.Advance(TimeSpan.FromSeconds(1));
			MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.1), DrugVector.Inhaled);
		}
		var xml = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromSeconds(1) });
		Assert.AreEqual(1, xml.Descendants("Dose").Count());
		Assert.AreEqual(0, xml.Descendants("Charge").Count());
		Assert.AreEqual(600, parent.RemainingSeconds);
		SubstanceExposureEffect.InitialiseEffectType();
		var restored = (SubstanceExposureEffect)Effect.LoadEffect(XElement.Parse(xml.ToString()), f.Actor.Object);
		Assert.AreEqual(600, restored.RemainingSeconds);
		var previous = f.Stamina;
		restored.Advance(TimeSpan.FromSeconds(10));
		Assert.AreEqual(previous + 10, f.Stamina);
	}

	[TestMethod]
	public void StrongestInternalDose_AbsorbsTheCompleteAction()
	{
		var f = new Fixture("<Effect type='healingrate'><Multiplier>2</Multiplier><Stages>0</Stages></Effect>");
		f.Entry.Lifecycle = SubstanceLifecycle.Maintained; f.Entry.Stacking = SubstanceStacking.Strongest; f.Entry.MinimumDose = 0.4;
		using (MagicalExposure.BeginExposure())
		{
			MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.5), DrugVector.Injected);
			MagicalExposure.Liquid(f.Actor.Object, f.Mix(0.5), DrugVector.Injected);
		}
		f.Effects.OfType<SubstanceExposureEffect>().Single().Advance(TimeSpan.FromSeconds(10));
		Assert.AreEqual(1.49, f.Effects.OfType<SpellHealingRateEffect>().Single().HealingRateMultiplier, 1e-9);
	}

	[TestMethod]
	public void TimedAggregate_ReloadRetainsCoatingSuppressionButPrunesRemovedLots()
	{
		var f = new Fixture(); f.Entry.Lifecycle = SubstanceLifecycle.Periodic; f.Entry.PulseMode = SubstancePulseMode.Timed;
		for (var i = 0; i < 2; i++)
		{
			using (MagicalExposure.BeginExposure())
			{
				var mix = f.Mix(1);
				MagicalExposure.Liquid(f.Actor.Object, mix, DrugVector.Touched, true);
				f.Body.Object.SurfaceLiquidState.AddLiquid(mix);
			}
		}
		var parent = f.Effects.OfType<SubstanceExposureEffect>().Single();
		var xml = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromSeconds(1) });
		Assert.AreEqual(1, xml.Descendants("Dose").Count());
		Assert.AreEqual(2, xml.Descendants("Charge").Count());
		SubstanceExposureEffect.InitialiseEffectType();
		var restored = (SubstanceExposureEffect)Effect.LoadEffect(XElement.Parse(xml.ToString()), f.Actor.Object);
		restored.RemovalEffect();
		Assert.IsTrue(MagicalExposure.RetainedLiquids(f.Actor.Object).All(x => x.Instance.MagicalCharges[3].Suppressed.Contains(f.Entry.Key)));
		f.Body.Object.SurfaceLiquidState.RemoveLiquidVolume(2);
		parent.Advance(TimeSpan.FromSeconds(1));
		xml = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromSeconds(1) });
		Assert.AreEqual(0, xml.Descendants("Charge").Count());
		Assert.AreEqual(119, parent.RemainingSeconds);
	}

}
