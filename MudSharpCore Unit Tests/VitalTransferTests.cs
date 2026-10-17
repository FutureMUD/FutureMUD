#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Testing.EnvironmentalMagic;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VitalTransferTests
{
	[DataTestMethod]
	[DataRow(1, 5.0)]
	[DataRow(5, 25.0)]
	[DataRow(7, 35.0)]
	public void Siphon_ExternalGrade_NativeWoundAndHealingBudget(int grade, double expected)
	{
		var f = new Fixture();
		var effect = f.Siphon(grade);
		var result = effect.Apply(f.Source.Object, f.Victim.Object, OpposedOutcomeDegree.Total,
			SpellPower.RecklesslyPowerful, f.Parent.Object, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(expected, f.Body.Wounds.Single().CurrentDamage, 0.000001);
		Assert.AreEqual(100 - expected, f.SourceWound.CurrentDamage, 0.000001);
		Assert.IsInstanceOfType(f.Body.Wounds.Single(), typeof(SimpleOrganicWound));
		f.Saves.Verify(x => x.AddInitialisation(It.IsAny<ILateInitialisingItem>()), Times.AtLeastOnce);
	}

	[DataTestMethod]
	[DataRow(0.0, 0.0)]
	[DataRow(0.25, 5.0)]
	[DataRow(1.0, 20.0)]
	public void Siphon_NativeNaturalArmour_OnlyPassedDamageHeals(double pass, double expected)
	{
		var f = new Fixture();
		f.SetNaturalArmour(pass);
		using var receipt = new VitalTransferReceipt(f.Source.Object, f.Victim.Object);
		var wounds = f.Body.PassiveSufferDamage(f.Damage(20, DamageType.Arcane, receipt)).ToArray();
		Assert.AreEqual(expected, receipt.DeliveredDamage, 0.000001);
		Assert.AreEqual(expected, receipt.Settle(1, 20), 0.000001);
		Assert.AreEqual(100 - expected, f.SourceWound.CurrentDamage, 0.000001);
		Assert.AreEqual(expected > 0 ? 1 : 0, wounds.Length);
	}

	[TestMethod]
	public void Receipt_ExistingCellularWound_OnlyNativeIncrementAndOneSettlement()
	{
		var f = new Fixture();
		var old = f.Body.PassiveSufferDamage(f.Damage(40, DamageType.Cellular)).Single();
		using var receipt = new VitalTransferReceipt(f.Source.Object, f.Victim.Object);
		Assert.AreSame(old, f.Body.PassiveSufferDamage(f.Damage(5, DamageType.Cellular, receipt)).Single());
		Assert.AreEqual(45, old.CurrentDamage);
		Assert.AreEqual(5, receipt.DeliveredDamage);
		Assert.AreEqual(5, receipt.Settle(1, 99));
		Assert.AreEqual(0, receipt.Settle(1, 99));
		Assert.IsNull(receipt.BeginMeasurement(f.Body, f.Damage(5, DamageType.Cellular, receipt)));
	}

	[TestMethod]
	public void Receipt_NativeBodypartCapAndHealingOverfill_ConservesActualDamage()
	{
		var f = new Fixture();
		f.Race.Setup(x => x.ModifiedHitpoints(f.Part.Object)).Returns(3);
		f.SourceWound.CurrentDamage = 2;
		using var receipt = new VitalTransferReceipt(f.Source.Object, f.Victim.Object);
		f.Body.PassiveSufferDamage(f.Damage(50, DamageType.Arcane, receipt)).ToArray();
		Assert.AreEqual(3, receipt.DeliveredDamage);
		Assert.AreEqual(2, receipt.Settle(1, 50));
		Assert.AreEqual(0, f.SourceWound.CurrentDamage);
		f.SourceBody.Verify(x => x.EvaluateWounds(), Times.Once);
	}

	[TestMethod]
	public void Siphon_WoundEventAddsUnrelatedDamage_DoesNotCreditCallbackInjury()
	{
		var f = new Fixture();
		f.Body.OnWounded += (_, wound) => wound.CurrentDamage += 80;
		var result = f.Siphon(1).Apply(f.Source.Object, f.Victim.Object, OpposedOutcomeDegree.Total,
			SpellPower.Standard, f.Parent.Object, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(85, f.Body.Wounds.Single().CurrentDamage);
		Assert.AreEqual(95, f.SourceWound.CurrentDamage);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Siphon_WoundCallbackSwapsBody_NoHealingRedirect(bool source)
	{
		var f = new Fixture();
		var replacement = new Mock<IBody>();
		f.Body.OnWounded += (_, _) =>
		{
			if (source) f.Source.SetupGet(x => x.Body).Returns(replacement.Object);
			else f.Victim.SetupGet(x => x.Body).Returns(replacement.Object);
		};
		f.Siphon(1).Apply(f.Source.Object, f.Victim.Object, OpposedOutcomeDegree.Total,
			SpellPower.Standard, f.Parent.Object, []);
		Assert.AreEqual(100, f.SourceWound.CurrentDamage);
		replacement.Verify(x => x.EvaluateWounds(), Times.Never);
	}

	[TestMethod]
	public void Siphon_BodyChangedAwayAndBack_LatchesInvalidation()
	{
		var f = new Fixture();
		f.Body.OnWounded += (_, _) => f.Source.Raise(x => x.CurrentBodyChanged += null,
			f.Source.Object, f.SourceBody.Object, new Mock<IBody>().Object);
		f.Siphon(1).Apply(f.Source.Object, f.Victim.Object, OpposedOutcomeDegree.Total,
			SpellPower.Standard, f.Parent.Object, []);
		Assert.AreEqual(100, f.SourceWound.CurrentDamage);
	}

	[TestMethod]
	public void Receipt_HealingEligibilityCallbackInvalidatesBody_NoMutation()
	{
		var f = new Fixture();
		var wound = new Mock<IWound>();
		wound.SetupGet(x => x.Parent).Returns(f.Source.Object);
		wound.SetupProperty(x => x.CurrentDamage, 50);
		wound.Setup(x => x.CanBeTreated(TreatmentType.Mend)).Returns(() =>
		{
			f.Source.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object);
			return Difficulty.Automatic;
		});
		f.SourceWounds.Clear(); f.SourceWounds.Add(wound.Object);
		using var receipt = new VitalTransferReceipt(f.Source.Object, f.Victim.Object);
		f.Body.PassiveSufferDamage(f.Damage(10, DamageType.Cellular, receipt)).ToArray();
		Assert.AreEqual(0, receipt.Settle(1, 10));
		Assert.AreEqual(50, wound.Object.CurrentDamage);
	}

	[DataTestMethod]
	[DataRow(1, 0.1)]
	[DataRow(5, 0.5)]
	[DataRow(7, 0.5)]
	public void Edge_GradeAndNativeWeaponHarm_ClampsFraction(int grade, double fraction)
	{
		var f = new Fixture();
		f.Enchant(grade);
		f.Body.CommandSufferDamage(f.Damage(20, DamageType.Arcane, tool: f.Weapon.Object)).ToArray();
		Assert.AreEqual(100 - 20 * fraction, f.SourceWound.CurrentDamage, 0.000001);
		Assert.AreEqual(20, f.Body.Wounds.Single().CurrentDamage);
	}

	[DataTestMethod]
	[DataRow("miss")]
	[DataRow("block")]
	[DataRow("zero")]
	[DataRow("armour")]
	[DataRow("unwielded")]
	[DataRow("removed")]
	public void Edge_NoEligibleWeaponHarm_NoHealing(string cause)
	{
		var f = new Fixture();
		f.Enchant(7);
		if (cause == "armour") f.SetNaturalArmour(0);
		if (cause == "unwielded") f.SourceBody.SetupGet(x => x.WieldedItems).Returns([]);
		if (cause == "removed") f.WeaponEffects.Clear();
		if (cause == "block") f.Weapon.Object.CommandSufferDamage(f.Damage(20, tool: f.Weapon.Object)).ToArray();
		else if (cause != "miss") f.Body.CommandSufferDamage(f.Damage(cause == "zero" ? 0 : 20,
			DamageType.Arcane, tool: f.Weapon.Object)).ToArray();
		Assert.AreEqual(100, f.SourceWound.CurrentDamage);
	}

	[TestMethod]
	public void Edge_OverlappingEffectsAndRepeatedResultProcessing_OnlyOneShare()
	{
		var f = new Fixture();
		f.Enchant(1); f.Enchant(7);
		var wounds = f.Body.CommandSufferDamage(f.Damage(20, DamageType.Arcane, tool: f.Weapon.Object)).ToArray();
		Assert.AreEqual(90, f.SourceWound.CurrentDamage);
		wounds.ProcessPassiveWounds(); wounds.ProcessPassiveWounds();
		Assert.AreEqual(90, f.SourceWound.CurrentDamage);
	}

	[TestMethod]
	public void Receipt_DuplicateMeasurementAndForeignItemWound_OnlyNativeOwnedDelta()
	{
		var f = new Fixture();
		using var receipt = new VitalTransferReceipt(f.Source.Object, f.Victim.Object);
		var packet = f.Damage(5, DamageType.Cellular, receipt);
		var measure = receipt.BeginMeasurement(f.Body, packet)!;
		var wound = f.Body.PassiveSufferDamage(f.Damage(5, DamageType.Cellular)).Single();
		var foreign = new Mock<IWound>(); foreign.SetupGet(x => x.CurrentDamage).Returns(200);
		foreign.SetupGet(x => x.Parent).Returns(f.Weapon.Object);
		measure.Complete([wound, wound, foreign.Object]); measure.Complete([wound]);
		Assert.AreEqual(5, receipt.DeliveredDamage);
		Assert.AreEqual(5, receipt.Settle(1, 5));
	}

	[TestMethod]
	public void Siphon_NativeBodypartAmplifiesDamage_HealingRetainsGradeCap()
	{
		var f = new Fixture();
		f.Part.SetupGet(x => x.DamageModifier).Returns(4);
		f.Siphon(1).Apply(f.Source.Object, f.Victim.Object, OpposedOutcomeDegree.Total,
			SpellPower.Standard, f.Parent.Object, []);
		Assert.AreEqual(20, f.Body.Wounds.Single().CurrentDamage);
		Assert.AreEqual(95, f.SourceWound.CurrentDamage);
	}

	[TestMethod]
	public void Receipt_HealingEligibilityTransfersWoundParent_DoesNotMutateForeignWound()
	{
		var f = new Fixture();
		var wound = new Mock<IWound>();
		wound.SetupProperty(x => x.Parent, f.Source.Object);
		wound.SetupProperty(x => x.CurrentDamage, 50);
		wound.Setup(x => x.CanBeTreated(TreatmentType.Mend)).Returns(() =>
		{
			wound.Object.Parent = f.Victim.Object;
			return Difficulty.Automatic;
		});
		f.SourceWounds.Clear(); f.SourceWounds.Add(wound.Object);
		using var receipt = new VitalTransferReceipt(f.Source.Object, f.Victim.Object);
		f.Body.PassiveSufferDamage(f.Damage(10, DamageType.Cellular, receipt)).ToArray();
		Assert.AreEqual(0, receipt.Settle(1, 10));
		Assert.AreEqual(50, wound.Object.CurrentDamage);
	}

	[TestMethod]
	public void Edge_PostDamageOutputRemovesEnchant_NoHealingAfterRemoval()
	{
		var f = new Fixture();
		f.Enchant(7);
		Mock.Get(f.Victim.Object.OutputHandler).SetupGet(x => x.Perceiver)
			.Callback(() => f.WeaponEffects.Clear()).Returns(f.Victim.Object);
		f.Body.CommandSufferDamage(f.Damage(20, DamageType.Arcane, tool: f.Weapon.Object)).ToArray();
		Assert.AreEqual(20, f.Body.Wounds.Single().CurrentDamage);
		Assert.AreEqual(100, f.SourceWound.CurrentDamage);
	}

	[TestMethod]
	public void StockDefinitions_ExplicitPolicyAndDefaults_RoundTrip()
	{
		var f = new Fixture();
		foreach (var content in new[] { ArmageddonVitalTransferSpellContent.VitalSiphon(1, "return true"),
			ArmageddonVitalTransferSpellContent.DevouringTouch(1, "return true"), ArmageddonVitalTransferSpellContent.LeechingEdge() })
		{
			var xml = content.BuildDefinition(1, 2, 3).Element("Effects")!.Elements().Single();
			var loaded = SpellEffectFactory.LoadEffect(xml, f.NativeSpell);
			Assert.IsTrue(XNode.DeepEquals(xml, loaded.SaveToXml()));
		}
		Assert.AreEqual("60*grade", ArmageddonVitalTransferSpellContent.LeechingEdge().DurationFormula);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonVitalTransferSpellContent.VitalSiphon(0, "return true"));
	}

	[DataTestMethod]
	[DataRow(false, 1.25)]
	[DataRow(true, 0.625)]
	public void NativeDamage_OrderedPolicyAggravatesMeasuredWound_OnlyFrozenDeltaHeals(bool edge, double healed)
	{
		var f = new Fixture();
		f.SetNaturalArmour(0.25);
		if (edge) f.Enchant(7);
		var policyAddedDamage = false;
		using var scope = f.EnterOrdered(() =>
		{
			if (!policyAddedDamage && f.Body.Wounds.SingleOrDefault() is { } wound)
			{
				policyAddedDamage = true;
				wound.CurrentDamage += 80;
			}
			return true;
		});
		if (edge) f.Body.CommandSufferDamage(f.Damage(5, tool: f.Weapon.Object)).ToArray();
		else f.Siphon(1).Apply(f.Source.Object, f.Victim.Object, OpposedOutcomeDegree.Total,
			SpellPower.Standard, f.Parent.Object, []);
		Assert.IsTrue(policyAddedDamage, "The actual command authority policy must execute after native injury.");
		Assert.AreEqual(81.25, f.Body.Wounds.Single().CurrentDamage, 0.000001);
		Assert.AreEqual(100 - healed, f.SourceWound.CurrentDamage, 0.000001);
	}

	[DataTestMethod]
	[DataRow(false, "victim")]
	[DataRow(true, "victim")]
	[DataRow(true, "enchantment")]
	[DataRow(true, "weapon")]
	public void Healing_OrderedPolicyInvalidatesAfterMendEligibility_NoSetterMutation(bool edge, string change)
	{
		var f = new Fixture();
		if (edge) f.Enchant(7);
		var eligibilityReached = false;
		var policyInvalidated = false;
		f.Source.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns<IWound>(wound =>
		{
			eligibilityReached = true;
			return f.Source.Object.HealthStrategy.GetSeverityFor(wound, f.Source.Object);
		});
		using var scope = f.EnterOrdered(() =>
		{
			if (eligibilityReached && !policyInvalidated)
			{
				policyInvalidated = true;
				switch (change)
				{
					case "victim": f.Victim.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object); break;
					case "enchantment": f.WeaponEffects.Clear(); break;
					case "weapon": f.SourceBody.SetupGet(x => x.WieldedItems).Returns([]); break;
				}
			}
			return true;
		});
		if (edge) f.Body.CommandSufferDamage(f.Damage(20, tool: f.Weapon.Object)).ToArray();
		else f.Siphon(1).Apply(f.Source.Object, f.Victim.Object, OpposedOutcomeDegree.Total,
			SpellPower.Standard, f.Parent.Object, []);
		Assert.IsTrue(policyInvalidated, "Policy must invalidate at the continuation after native Mend eligibility.");
		Assert.AreEqual(edge ? 20 : 5, f.Body.Wounds.Single().CurrentDamage);
		Assert.AreEqual(100, f.SourceWound.CurrentDamage);
		f.SourceBody.Verify(x => x.EvaluateWounds(), Times.Never);
	}

	private sealed class Fixture
	{
		internal Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<ISaveManager> Saves { get; } = new();
		internal Mock<ICharacter> Source { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<IBody> SourceBody { get; } = new();
		internal Mock<ICharacter> Victim { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<IRace> Race { get; } = new();
		internal Mock<IExternalBodypart> Part { get; } = new();
		internal NativeBody Body { get; }
		internal SimpleOrganicWound SourceWound { get; }
		internal List<IWound> SourceWounds { get; } = [];
		internal Mock<IGameItem> Weapon { get; } = new();
		internal List<IEffect> WeaponEffects { get; } = [];
		internal Mock<IMagicSpellEffectParent> Parent { get; } = new();
		internal MagicSpell NativeSpell { get; }

		internal Fixture()
		{
			Source.As<ICharacterInstance>();
			World.SetupGet(x => x.SaveManager).Returns(Saves.Object);
			World.SetupGet(x => x.Characters).Returns(new EnvironmentalMagicTestRegistry<ICharacter>());
			var expression = new Mock<ITraitExpression>(); expression.SetupGet(x => x.Id).Returns(1);
			expression.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>())).Returns(100);
			var expressions = new EnvironmentalMagicTestRegistry<ITraitExpression>(); expressions.Add(expression.Object);
			World.SetupGet(x => x.TraitExpressions).Returns(expressions);
			SimpleLivingHealthStrategy.RegisterHealthStrategyLoader();
			var strategy = BaseHealthStrategy.LoadStrategy(new MudSharp.Models.HealthStrategy
			{
				Id = 1, Name = "Vital transfer native health", Type = "SimpleLiving",
				Definition = "<Definition><MaximumHitPointsExpression>1</MaximumHitPointsExpression><MaximumStunExpression>1</MaximumStunExpression><MaximumPainExpression>1</MaximumPainExpression><HealingTickDamageExpression>1</HealingTickDamageExpression><HealingTickStunExpression>1</HealingTickStunExpression><HealingTickPainExpression>1</HealingTickPainExpression></Definition>"
			}, World.Object);
			Part.SetupGet(x => x.Id).Returns(1); Part.SetupGet(x => x.RelativeHitChance).Returns(1);
			Part.SetupGet(x => x.DamageModifier).Returns(1); Part.SetupGet(x => x.PainModifier).Returns(1); Part.SetupGet(x => x.StunModifier).Returns(1);
			Race.Setup(x => x.ModifiedHitpoints(Part.Object)).Returns(1000);
			Race.SetupGet(x => x.DamageToleranceModifier).Returns(1);
			Source.SetupGet(x => x.Id).Returns(1); Source.SetupGet(x => x.Body).Returns(SourceBody.Object);
			Source.SetupGet(x => x.Gameworld).Returns(World.Object); Source.SetupGet(x => x.Race).Returns(Race.Object);
			Source.SetupGet(x => x.State).Returns(CharacterState.Awake); Source.SetupGet(x => x.HealthStrategy).Returns(strategy);
			Source.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns<IWound>(wound => strategy.GetSeverityFor(wound, Source.Object));
			SourceBody.SetupGet(x => x.Actor).Returns(Source.Object); SourceBody.SetupGet(x => x.Race).Returns(Race.Object);
			SourceBody.SetupGet(x => x.Wounds).Returns(SourceWounds); SourceBody.Setup(x => x.HitpointsForBodypart(Part.Object)).Returns(1000);
			SourceWound = new SimpleOrganicWound(World.Object, Source.Object, 100, 1, 0, DamageType.Cellular, Part.Object, null, null, null);
			SourceWounds.Add(SourceWound);
			Body = TestObjectFactory.CreateUninitialized<NativeBody>();
			typeof(ConcreteBody).GetProperty(nameof(Body.Gameworld))!.SetValue(Body, World.Object);
			typeof(PerceivedItem).GetProperty(nameof(Body.EffectHandler))!.SetValue(Body, new EffectHandler(Body));
			foreach (var name in new[] { "_merits", "_limbs", "_wounds", "_bodyparts", "_bones", "_organs", "_allItems", "_wornItems",
				"_cachedEffects", "_cachedOrganFunctionsByOrgan", "_cachedOrganFunctionsByType", "_implants", "_prosthetics", "_externalItems", "_heldItems", "_wieldedItems" })
			{
				var field = Field(name); field.SetValue(Body, Activator.CreateInstance(field.FieldType));
			}
			((ICollection<IBodypart>)Field("_bodyparts").GetValue(Body)!).Add(Part.Object);
			Field("_healthStrategy").SetValue(Body, strategy);
			typeof(ConcreteBody).GetProperty(nameof(Body.Race))!.SetValue(Body, Race.Object);
			Body.Actor = Victim.Object;
			Victim.SetupGet(x => x.Id).Returns(2); Victim.SetupGet(x => x.Body).Returns(Body);
			Victim.SetupGet(x => x.State).Returns(CharacterState.Awake); Victim.SetupGet(x => x.Gameworld).Returns(World.Object);
			Victim.SetupGet(x => x.Race).Returns(Race.Object); Victim.SetupGet(x => x.Wounds).Returns(() => Body.Wounds);
			Victim.SetupGet(x => x.HealthStrategy).Returns(strategy); Victim.SetupGet(x => x.LineFormatLength).Returns(100);
			Mock.Get(Victim.Object.Account).SetupGet(x => x.InnerLineFormatLength).Returns(100);
			Victim.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns<IWound>(wound => strategy.GetSeverityFor(wound, Victim.Object));
			Victim.Setup(x => x.ProcessPassiveWound(It.IsAny<IWound>())).Callback<IWound>(Body.ProcessPassiveWound);
			Weapon.SetupGet(x => x.Gameworld).Returns(World.Object); Weapon.SetupGet(x => x.Effects).Returns(WeaponEffects);
			Weapon.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns(new Mock<IMeleeWeapon>().Object);
			SourceBody.SetupGet(x => x.WieldedItems).Returns([Weapon.Object]);
			NativeSpell = new MagicCastingFixture().NewSpell(5, "Vital transfer", "");
		}

		internal IDisposable EnterOrdered(Func<bool> policy)
		{
			var identity = new Mock<ICharacterIdentity>();
			identity.SetupGet(x => x.Id).Returns(1);
			identity.SetupGet(x => x.Instances).Returns([(ICharacterInstance)Source.Object]);
			Source.SetupGet(x => x.Identity).Returns(identity.Object);
			Source.SetupGet(x => x.InstanceId).Returns(1);
			var roots = new All<ICharacter>(); roots.Add(Source.Object);
			World.SetupGet(x => x.Actors).Returns(roots);
			World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
			World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
			var room = new Mock<IRoom>(); room.SetupGet(x => x.Characters).Returns([Source.Object]);
			Source.SetupGet(x => x.Location).Returns(room.Object);
			var authority = CommandExecutionAuthority.Prepare(Source.Object, Source.Object, "vital transfer", policy);
			return CommandExecutionScope.EnterDispatch(authority, Source.Object);
		}

		private static FieldInfo Field(string name)
		{
			for (var type = typeof(ConcreteBody); type is not null; type = type.BaseType)
				if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) is { } field) return field;
			throw new InvalidOperationException(name);
		}
		internal Damage Damage(double amount, DamageType type = DamageType.Arcane, IMagicVitalTransferDamage? receipt = null, IGameItem? tool = null) => new()
		{ ActorOrigin = Source.Object, Bodypart = Part.Object, DamageType = type, DamageAmount = amount, VitalTransfer = receipt!, ToolOrigin = tool! };
		internal VitalSiphonEffect Siphon(int grade)
		{
			typeof(MagicSpell).GetProperty("InvocationGrade", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(NativeSpell, grade);
			return new(new XElement("Effect", new XAttribute("type", "vitalsiphon"), new XElement("DamageType", (int)DamageType.Arcane),
				new XElement("DamagePerGrade", 5), new XElement("HealingCapPerGrade", 5)), NativeSpell);
		}
		internal void Enchant(int grade)
		{
			Siphon(grade);
			var parent = new Mock<IMagicSpellEffectParent>(); parent.SetupGet(x => x.Owner).Returns(Weapon.Object);
			parent.SetupGet(x => x.Spell).Returns(NativeSpell);
			var template = new LeechingEdgeEffect(new XElement("Effect", new XAttribute("type", "leechingedge"),
				new XElement("FractionPerGrade", 0.1), new XElement("MaximumFraction", 0.5)), NativeSpell);
			var effect = template.GetOrApplyEffect(Source.Object, Weapon.Object, OpposedOutcomeDegree.Total, SpellPower.Standard, parent.Object, [])!;
			parent.SetupGet(x => x.SpellEffects).Returns([effect]);
			WeaponEffects.Add(parent.Object); WeaponEffects.Add(effect);
		}
		internal void SetNaturalArmour(double pass)
		{
			var definition = new XElement("Definition");
			foreach (var name in new[] { "DissipateExpressions", "AbsorbExpressions", "DissipateExpressionsPain", "AbsorbExpressionsPain", "DissipateExpressionsStun", "AbsorbExpressionsStun" })
				definition.Add(new XElement(name, new XElement("Expression", new XAttribute("damagetype", (int)DamageType.Arcane),
					name == "AbsorbExpressions" ? $"damage*{pass.ToString(System.Globalization.CultureInfo.InvariantCulture)}" : name.EndsWith("Pain") ? "0" : name.EndsWith("Stun") ? "0" : "damage")));
			Race.SetupGet(x => x.NaturalArmourType).Returns(new ArmourType(new MudSharp.Models.ArmourType
			{ Id = 1, Name = "Native test armour", Definition = definition.ToString() }, World.Object));
		}
	}

	private sealed class NativeBody : ConcreteBody
	{
		private NativeBody() : base(null!, null!, (ICharacterTemplate)null!) { }
		public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
			bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => "a native test body";
	}
}
