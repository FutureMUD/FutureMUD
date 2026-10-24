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
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.PerceptionEngine;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class FiniteArmourTests
{
	[TestMethod]
	[DataRow(1, 0)] [DataRow(4, 1)] [DataRow(7, 4)]
	public void ShelteringVeil_OutsideWaterConsumesOneRankedClericalUnit(int grade, int rank)
	{
		var f = new Fixture("sheltering_veil", true, rank);
		var result = f.F.Service.Cast(new MagicCastingIntent(f.F.Actor.Object, f.F.Earth.Id, 1, grade, false, "target", OriginId: Guid.NewGuid()));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(2, f.ComponentQuantity); Assert.AreEqual(1, f.ComponentWrites);
		Assert.IsTrue(f.Effects.OfType<SpellArmourProtectionEffect>().Any());
	}

	[TestMethod]
	public void ShelteringVeil_MissingHighRankRefusesBeforePayment()
	{
		var f = new Fixture("sheltering_veil", true, 0);
		var before = f.F.Balances[f.F.Resources[1]];
		var result = f.F.Service.Cast(new MagicCastingIntent(f.F.Actor.Object, f.F.Earth.Id, 1, 7, false, "target", OriginId: Guid.NewGuid()));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(before, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(0, f.ComponentWrites); Assert.IsFalse(f.Effects.OfType<SpellArmourProtectionEffect>().Any());
	}

	[TestMethod]
	public void ShelteringVeil_MultiplePlaneWaterMembershipWaivesOnlyMaterial()
	{
		var f = new Fixture("sheltering_veil", true, 0); f.WaterPresence = true; f.Components.Clear();
		var result = f.F.Service.Cast(new MagicCastingIntent(f.F.Actor.Object, f.F.Earth.Id, 1, 7, false, "target", OriginId: Guid.NewGuid()));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(0, f.ComponentWrites); Assert.IsTrue(f.Effects.OfType<SpellArmourProtectionEffect>().Any());
	}

	[TestMethod]
	public void ShelteringVeil_WaterMembershipDriftBeforePaymentRefuses()
	{
		var f = new Fixture("sheltering_veil", true, 4);
		var before = f.F.Balances[f.F.Resources[1]];
		f.F.Checkpoint = stage => { if (stage == "BeforePayment") f.WaterPresence = true; };
		var result = f.F.Service.Cast(new MagicCastingIntent(f.F.Actor.Object, f.F.Earth.Id, 1, 1, false, "target", OriginId: Guid.NewGuid()));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(before, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(0, f.ComponentWrites);
	}

	[TestMethod]
	public void ShelteringVeil_DirectEffectFactoryCannotBypassMaterialExecution()
	{
		var f = new Fixture("sheltering_veil", true, 4);
		var parent = new MagicSpellParent(f.Victim.Object, f.Spell, f.F.Actor.Object);
		Assert.ThrowsExactly<InvalidOperationException>(() =>
			f.Spell.SpellEffects.Single().GetOrApplyEffect(f.F.Actor.Object, f.Victim.Object,
				default, SpellPower.Weak, parent, []));
		Assert.AreEqual(0, f.ComponentWrites); Assert.IsFalse(f.Effects.Any());
	}

	[TestMethod,
	 DataRow("flame_mantle", 1),
	 DataRow("flame_mantle", 7),
	 DataRow("sheltering_veil", 1),
	 DataRow("sheltering_veil", 7),
	 DataRow("unyielding_veil", 1),
	 DataRow("unyielding_veil", 7),
	 DataRow("mist_shield", 1),
	 DataRow("mist_shield", 7),
	 DataRow("earthen_ward", 1),
	 DataRow("earthen_ward", 7),
	 DataRow("stone_skin", 1),
	 DataRow("stone_skin", 7),
	 DataRow("wind_mantle", 1),
	 DataRow("wind_mantle", 7),
	 DataRow("wind_shield", 1),
	 DataRow("wind_shield", 7),
	 DataRow("shadow_mantle", 1),
	 DataRow("shadow_mantle", 7),
	 DataRow("charged_aegis", 1),
	 DataRow("charged_aegis", 7)]
	public void StockPaidCast_NativeHitsCannotExceedFrozenBudget(string key, int grade)
	{
		var f = new Fixture(key); var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message + string.Join(";", f.F.Messages));
		var armour = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		var parent = f.Effects.OfType<MagicSpellParent>().Single();
		var maximum = (key == "unyielding_veil" ? 20 : 10) * grade;
		Assert.AreEqual(maximum, armour.ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(f.Victim.Object));
		Assert.AreEqual(TimeSpan.FromSeconds(60 * grade), parent.ResolvedDuration);
		Assert.AreEqual(1000 - (grade == 7 ? 50 : key is "mist_shield" or "wind_shield" ? 20 : 7), f.F.Balances[f.F.Resources[1]], 1e-8);
		if (key == "stone_skin") Assert.AreEqual(-grade, f.Effects.OfType<SpellTraitBoostEffect>().Single().Bonus);
		if (key == "flame_mantle") Assert.IsTrue(armour.ArmourConfiguration.FullDescriptionAddendum.Contains("warm glow"));
		// First hit spends 4, the second crosses the remaining-capacity boundary; native body creates all wounds.
		f.Hit(8); Assert.AreEqual(4, armour.TotalDamageAbsorbed, 1e-8);
		f.Hit(2 * maximum); Assert.AreEqual(maximum, armour.TotalDamageAbsorbed, 1e-8);
		Assert.IsFalse(f.Effects.Contains(armour));
		Assert.AreEqual(maximum + 8, f.Body.Wounds.Sum(x => x.CurrentDamage), 1e-8);
		var previous = f.Body.Wounds.Sum(x => x.CurrentDamage); f.Hit(12);
		Assert.AreEqual(previous + 12, f.Body.Wounds.Sum(x => x.CurrentDamage), 1e-8);
		Assert.IsTrue(f.Body.Wounds.All(x => x is SimpleOrganicWound));
		Assert.AreEqual($"{(key == "unyielding_veil" ? 20 : 10)}*grade", ((SpellArmourEffect)f.Spell.SpellEffects.First()).ArmourConfiguration.MaximumDamageAbsorbed.OriginalFormulaText);
	}
	[TestMethod]
	public void WrapperConfigurationClone_PreservesBindingWhereOriginalFormulaWouldBecomeZero()
	{
		var f = new Fixture(); var copy = f.Spell.CastingCopy(f.F.Actor.Object, f.F.Traits[0], 7, SpellPower.ExtremelyStrong, Difficulty.Normal);
		var config = ((SpellArmourEffect)copy.SpellEffects.First()).ArmourConfiguration;
		Assert.AreEqual("0", config.MaximumDamageAbsorbed.OriginalFormulaText, "Negative control: rebuilding this inherited text discards the invocation binding.");
		Assert.AreEqual(0, new TraitExpression(config.MaximumDamageAbsorbed.OriginalFormulaText, f.F.World.Object).Evaluate(f.F.Actor.Object));
		Assert.AreEqual(70, new MagicArmourConfiguration(config).MaximumDamageAbsorbed.Evaluate(f.F.Actor.Object));
		Assert.AreEqual(70, ((SpellArmourEffect)copy.SpellEffects.First().Clone()).ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(f.F.Actor.Object));
	}
	[TestMethod]
	public void NativeTransforms_PartialLastHitScalesPainStunAndPreservesPhysicalMetadata()
	{
		var f = new Fixture(); f.Cast(1); var armour = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		var guard = new CountGuard(); var incoming = f.Packet(8, guard); var wounds = new List<IWound>();
		var first = armour.PassiveSufferDamage(incoming, ref wounds);
		Assert.AreEqual(4, first.DamageAmount); Assert.AreEqual(2, first.PainAmount); Assert.AreEqual(1, first.StunAmount);
		Assert.AreEqual(DamageType.Crushing, first.DamageType);
		Assert.AreSame(f.Body, first.TargetBody); Assert.AreSame(guard, first.MutationGuard);
		var preview = armour.PreviewDamage(f.Packet(20, guard)); Assert.AreEqual(14, preview.DamageAmount); Assert.AreEqual(4, armour.TotalDamageAbsorbed);
		var last = armour.SufferDamage(f.Packet(20, guard), ref wounds);
		Assert.AreEqual(14, last.DamageAmount); Assert.AreEqual(11, last.PainAmount, 1e-8); Assert.AreEqual(9.5, last.StunAmount, 1e-8);
		Assert.AreEqual(DamageType.Slashing, last.DamageType); Assert.AreEqual(10, armour.TotalDamageAbsorbed);
		Assert.AreSame(f.Body, last.TargetBody); Assert.AreSame(guard, last.MutationGuard);
		Assert.AreSame(incoming, armour.PreviewDamage(incoming), "Exhausted retained references do not refill.");
		Assert.IsTrue(guard.Calls >= 6);
	}
	[TestMethod, DataRow(false), DataRow(true)]
	public void NativePenetrationCallback_BodyOrGuardDriftCannotSpendBudgetOrCreateWound(bool guardDrift)
	{
		var f = new Fixture(); f.Cast(1); var armour = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		var check = new Mock<ICheck>(); var guard = new CountGuard(); var reached = false; f.F.World.Setup(x => x.GetCheck(CheckType.PenetrationDefenseCheck)).Returns(check.Object);
		check.Setup(x => x.Check(f.Victim.Object, Difficulty.Normal, (IPerceivable)null!, (IUseTrait)null!, 0.0, TraitUseType.Practical, It.IsAny<(string, object)[]>()))
			.Callback(() => { reached = true; if (guardDrift) guard.Reject = true; else f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()); }).Returns(CheckOutcome.SimpleOutcome(CheckType.PenetrationDefenseCheck, Outcome.Pass));
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Body.PassiveSufferDamage(f.Packet(8, guard)).ToArray());
		Assert.IsTrue(reached, "The actual native ArmourType penetration check ran its callback.");
		Assert.AreEqual(0, armour.TotalDamageAbsorbed); Assert.IsFalse(f.Body.Wounds.Any());
	}
	[TestMethod]
	public void NativeMutationGuard_StalePacketCannotSpendProtectionOrWound()
	{
		var f = new Fixture(); f.Cast(1); var armour = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Body.PassiveSufferDamage(f.Packet(8, new CountGuard { Reject = true })).ToArray());
		Assert.AreEqual(0, armour.TotalDamageAbsorbed); Assert.IsFalse(f.Body.Wounds.Any());
	}
	[TestMethod]
	public void ColdParentReload_PreservesUsedBudgetAndRemainingCapacity()
	{
		var f = new Fixture(); f.Cast(1); f.Hit(8);
		var parent = f.Effects.OfType<MagicSpellParent>().Single(); var saved = parent.SaveToXml([]);
		f.Effects.Clear(); MagicSpellParent.InitialiseEffectType(); SpellArmourProtectionEffect.InitialiseEffectType();
		var loaded = (MagicSpellParent)Effect.LoadEffect(saved, f.Victim.Object); f.Effects.Add(loaded);
		var armour = (SpellArmourProtectionEffect)loaded.SpellEffects.Single();
		Assert.AreEqual(4, armour.TotalDamageAbsorbed); Assert.AreEqual(10, armour.ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(f.Victim.Object));
		f.Hit(20); Assert.AreEqual(10, armour.TotalDamageAbsorbed); Assert.AreEqual(18, f.Body.Wounds.Sum(x => x.CurrentDamage), 1e-8);
	}
	[TestMethod]
	public void AbsentFiniteMarker_PreservesLegacyZeroBudgetAndWholeHitSemantics()
	{
		var f = new Fixture(); var config = ((SpellArmourEffect)f.Spell.SpellEffects.First()).ArmourConfiguration;
		var xml = new XElement("Effect"); config.SaveToXml(xml); xml.Element("FiniteBudget")!.Remove(); xml.Element("MaximumDamageAbsorbed")!.Value = "0";
		var legacy = new MagicArmourConfiguration(xml, f.F.World.Object); Assert.IsFalse(legacy.FiniteBudget);
		var effect = new SpellArmourProtectionEffect(f.Victim.Object, new MagicSpellParent(f.Victim.Object, f.Spell, f.F.Actor.Object), legacy);
		var wounds = new List<IWound>(); Assert.AreEqual(50, effect.PassiveSufferDamage(f.Packet(100), ref wounds).DamageAmount);
		Assert.AreEqual(50, effect.PassiveSufferDamage(f.Packet(100), ref wounds).DamageAmount); Assert.AreEqual(100, effect.TotalDamageAbsorbed);
	}
	[TestMethod, DataRow(true), DataRow(false)]
	public void NativeCompleteMitigation_NullPacketsStillSpendAndClampFiniteCapacity(bool dissipate)
	{
		var f = new Fixture(); f.Cast(1); var effect = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		var definition = new XElement("Definition");
		foreach (var name in new[] { "DissipateExpressions", "AbsorbExpressions", "DissipateExpressionsPain", "AbsorbExpressionsPain", "DissipateExpressionsStun", "AbsorbExpressionsStun" })
			definition.Add(new XElement(name, new XElement("Expression", new XAttribute("damagetype", (int)DamageType.Slashing),
				name.StartsWith("Dissipate") && !dissipate ? "damage" : "0")));
		effect.ArmourConfiguration.ArmourType = new ArmourType(new MudSharp.Models.ArmourType
			{ Id = 19, Name = "Complete native mitigation", MinimumPenetrationDegree = (int)OpposedOutcomeDegree.Major, Definition = definition.ToString() }, f.F.World.Object);
		f.Hit(4); Assert.AreEqual(4, effect.TotalDamageAbsorbed); Assert.IsFalse(f.Body.Wounds.Any());
		f.Hit(20); Assert.AreEqual(10, effect.TotalDamageAbsorbed); Assert.AreEqual(14, f.Body.Wounds.Sum(x => x.CurrentDamage), 1e-8);
		Assert.IsFalse(f.Effects.Contains(effect));
	}
	[TestMethod]
	public void FiniteZeroCapacity_IsTransparentWhileLegacyZeroRemainsUnlimited()
	{
		var f = new Fixture(); var config = new MagicArmourConfiguration(((SpellArmourEffect)f.Spell.SpellEffects.First()).ArmourConfiguration)
		{ MaximumDamageAbsorbed = new TraitExpression("0", f.F.World.Object) };
		var parent = new MagicSpellParent(f.Victim.Object, f.Spell, f.F.Actor.Object);
		var effect = new SpellArmourProtectionEffect(f.Victim.Object, parent, config);
		parent.AddSpellEffect(effect); f.Effects.Add(parent); f.Effects.Add(effect);
		var damage = f.Packet(100); var wounds = new List<IWound>();
		Assert.AreSame(damage, effect.PassiveSufferDamage(damage, ref wounds)); Assert.AreEqual(0, effect.TotalDamageAbsorbed);
	}

	[TestMethod,
	 DataRow("flame_mantle"),
	 DataRow("sheltering_veil"),
	 DataRow("unyielding_veil"),
	 DataRow("mist_shield"),
	 DataRow("earthen_ward"),
	 DataRow("stone_skin"),
	 DataRow("wind_mantle"),
	 DataRow("wind_shield"),
	 DataRow("shadow_mantle"),
	 DataRow("charged_aegis")]
	public void StockDefinition_OwnedTrueProgCompilesWithTypedCharacterFilterAndNativeApplies(string key)
	{
		FutureProgTestBootstrap.EnsureInitialised(); var f = new MagicCastingFixture();
		var content = ArmageddonArmourSpellContent.Create("arm.spell." + key, 9, 9, 3);
		var row = content.EligibilityRow(41); Assert.IsNotNull(row); row.Id = 777;
		Assert.AreEqual("return true", row.FunctionText);
		var prog = new FutureProg(row, f.World.Object);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.AreEqual(ProgVariableTypes.Boolean, prog.ReturnType);
		Assert.IsTrue(prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]));
		Assert.IsTrue(prog.ExecuteWithStatus(out var permitted, f.Actor.Object, f.Actor.Object)); Assert.AreEqual(true, permitted);
		var definition = content.BuildDefinition(10, 1, prog.Id);
		Assert.AreEqual(777L, (long)definition.Element("Trigger")!.Element("TargetFilterProg")!);
		Assert.AreEqual(777L, (long)definition.Element("Effects")!.Elements().First().Element("ArmourAppliesProg")!);
		Assert.AreEqual(9L, (long)definition.Element("Effects")!.Elements().First().Element("ArmourType")!);
		Assert.AreEqual(9L, (long)definition.Element("Effects")!.Elements().First().Element("ArmourMaterial")!);
	}

	[TestMethod, DataRow(false), DataRow(true)]
	public void NativePenetrationCallback_RemovedChildOrParentCannotAttenuateOrSpend(bool removeParent)
	{
		var f = new Fixture(); f.Cast(1); var armour = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		var parent = f.Effects.OfType<MagicSpellParent>().Single(); var reached = false;
		var check = new Mock<ICheck>(); f.F.World.Setup(x => x.GetCheck(CheckType.PenetrationDefenseCheck)).Returns(check.Object);
		check.Setup(x => x.Check(f.Victim.Object, Difficulty.Normal, (IPerceivable)null!, (IUseTrait)null!, 0.0, TraitUseType.Practical, It.IsAny<(string, object)[]>()))
			.Callback(() => { reached = true; f.Victim.Object.RemoveEffect(removeParent ? parent : armour, true); })
			.Returns(CheckOutcome.SimpleOutcome(CheckType.PenetrationDefenseCheck, Outcome.Pass));
		f.Hit(8); Assert.IsTrue(reached); Assert.AreEqual(0, armour.TotalDamageAbsorbed);
		Assert.AreEqual(8, f.Body.Wounds.Sum(x => x.CurrentDamage), 1e-8);
		Assert.IsTrue(f.Body.Wounds.All(x => x.DamageType == DamageType.Slashing));
	}
	[TestMethod]
	public void CommittedSpend_AfterCleanSaveMarksOwnerEffectsForPersistenceButPreviewAndZeroDoNot()
	{
		var f = new Fixture(); f.Cast(1); var armour = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		var parent = f.Effects.OfType<MagicSpellParent>().Single(); parent.SaveToXml([]); f.Victim.Object.EffectsChanged = false;
		armour.PreviewDamage(f.Packet(8)); Assert.IsFalse(f.Victim.Object.EffectsChanged); Assert.AreEqual(0, armour.TotalDamageAbsorbed);
		var wounds = new List<IWound>(); armour.PassiveSufferDamage(f.Packet(0), ref wounds); Assert.IsFalse(f.Victim.Object.EffectsChanged);
		// Entire native packet dissipates: no wound/removal can accidentally dirty the character for us.
		var definition = new XElement("Definition");
		foreach (var name in new[] { "DissipateExpressions", "AbsorbExpressions", "DissipateExpressionsPain", "AbsorbExpressionsPain", "DissipateExpressionsStun", "AbsorbExpressionsStun" })
			definition.Add(new XElement(name, new XElement("Expression", new XAttribute("damagetype", (int)DamageType.Slashing), "0")));
		armour.ArmourConfiguration.ArmourType = new ArmourType(new MudSharp.Models.ArmourType
			{ Id = 19, Name = "Complete native mitigation", MinimumPenetrationDegree = (int)OpposedOutcomeDegree.Major, Definition = definition.ToString() }, f.F.World.Object);
		f.Hit(4); Assert.IsFalse(f.Body.Wounds.Any()); Assert.IsTrue(f.Effects.Contains(armour)); Assert.AreEqual(4, armour.TotalDamageAbsorbed);
		Assert.IsTrue(f.Victim.Object.EffectsChanged, "Character.Save writes retained children only when this native dirty flag is set.");
		Assert.AreEqual(4.0, (double)parent.SaveToXml([]).Descendants("TotalDamageAbsorbed").Single());
	}
	[TestMethod, DataRow("-5", false), DataRow("NaN", false), DataRow("Infinity", false),
	 DataRow("-5", true), DataRow("NaN", true), DataRow("Infinity", true)]
	public void ColdMalformedFiniteState_IsRejectedBeforeDamageOrNativeRoll(string spent, bool maximum)
	{
		var f = new Fixture(); f.Cast(1); var saved = f.Effects.OfType<MagicSpellParent>().Single().SaveToXml([]);
		saved.Descendants(maximum ? "MaximumDamageAbsorbed" : "TotalDamageAbsorbed").Single().Value = spent;
		MagicSpellParent.InitialiseEffectType(); SpellArmourProtectionEffect.InitialiseEffectType();
		Assert.ThrowsExactly<InvalidOperationException>(() => Effect.LoadEffect(saved, f.Victim.Object));
		Assert.IsFalse(f.Body.Wounds.Any());
		f.F.Check.Verify(x => x.Check(f.Victim.Object, Difficulty.Normal, (IPerceivable)null!, (IUseTrait)null!, 0.0, TraitUseType.Practical, It.IsAny<(string, object)[]>()), Times.Never);
	}
	[TestMethod]
	public void ColdOverspent_IsExhaustedWithoutRefill()
	{
		var f = new Fixture(); f.Cast(1); var saved = f.Effects.OfType<MagicSpellParent>().Single().SaveToXml([]);
		saved.Descendants("TotalDamageAbsorbed").Single().Value = "15"; f.Effects.Clear();
		MagicSpellParent.InitialiseEffectType(); SpellArmourProtectionEffect.InitialiseEffectType();
		var parent = (MagicSpellParent)Effect.LoadEffect(saved, f.Victim.Object); f.Effects.Add(parent);
		f.Hit(8); Assert.AreEqual(8, f.Body.Wounds.Sum(x => x.CurrentDamage), 1e-8);
		Assert.AreEqual(15, ((SpellArmourProtectionEffect)parent.SpellEffects.Single()).TotalDamageAbsorbed);
	}

	[TestMethod, DataRow(-5.0), DataRow(double.NaN), DataRow(double.PositiveInfinity)]
	public void RuntimeMalformedSpent_IsRejectedBeforeNativeRollOrDirtyFlag(double spent)
	{
		var f = new Fixture(); f.Cast(1); var armour = f.Effects.OfType<SpellArmourProtectionEffect>().Single();
		typeof(SpellArmourProtectionEffect).GetProperty(nameof(SpellArmourProtectionEffect.TotalDamageAbsorbed))!.SetValue(armour, spent);
		f.Victim.Object.EffectsChanged = false;
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Hit(8));
		Assert.IsFalse(f.Body.Wounds.Any()); Assert.IsFalse(f.Victim.Object.EffectsChanged);
		f.F.Check.Verify(x => x.Check(f.Victim.Object, Difficulty.Normal, (IPerceivable)null!, (IUseTrait)null!, 0.0, TraitUseType.Practical, It.IsAny<(string, object)[]>()), Times.Never);
	}

	private sealed class CountGuard : IDamageMutationGuard
	{
		public int Calls; public bool Reject;
		public void Validate(IHaveWounds owner, IDamage damage) { Calls++; if (Reject) throw new InvalidOperationException("stale test packet"); }
	}
	private sealed class Fixture
	{
		internal MagicCastingFixture F { get; } = new();
		internal Mock<ICharacter> Victim { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<IExternalBodypart> Part { get; } = new();
		internal List<IEffect> Effects { get; } = [];
		internal NativeBody Body { get; }
		internal MagicSpell Spell { get; }
		internal List<IGameItem> Components { get; } = [];
		internal int ComponentQuantity { get; private set; } = 3;
		internal int ComponentWrites { get; private set; }
		internal bool WaterPresence { get; set; }
		internal Fixture(string key = "flame_mantle", bool sourceMaterial = false, int componentRank = 4)
		{
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object);
			var allowed = Mock.Get(F.World.Object.FutureProgs.Get(1));
			allowed.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			allowed.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			object permitted = true; allowed.Setup(x => x.ExecuteWithStatus(out permitted, It.IsAny<object[]>())).Returns(true);
			var race = new Mock<IRace>(); race.SetupGet(x => x.DamageToleranceModifier).Returns(1);
			race.Setup(x => x.ModifiedHitpoints(Part.Object)).Returns(1000);
			Part.SetupGet(x => x.Id).Returns(1); Part.SetupGet(x => x.RelativeHitChance).Returns(1);
			Part.SetupGet(x => x.DamageModifier).Returns(1); Part.SetupGet(x => x.PainModifier).Returns(1); Part.SetupGet(x => x.StunModifier).Returns(1);
			var expression = new Mock<ITraitExpression>(); expression.SetupGet(x => x.Id).Returns(90);
			expression.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>())).Returns(100);
			F.Expressions.Add(expression.Object); SimpleLivingHealthStrategy.RegisterHealthStrategyLoader();
			var strategy = BaseHealthStrategy.LoadStrategy(new MudSharp.Models.HealthStrategy { Id = 90, Name = "Native paid test", Type = "SimpleLiving",
				Definition = "<Definition><MaximumHitPointsExpression>90</MaximumHitPointsExpression><MaximumStunExpression>90</MaximumStunExpression><MaximumPainExpression>90</MaximumPainExpression><HealingTickDamageExpression>90</HealingTickDamageExpression><HealingTickStunExpression>90</HealingTickStunExpression><HealingTickPainExpression>90</HealingTickPainExpression></Definition>" }, F.World.Object);
			F.Actor.SetupGet(x => x.Race).Returns(race.Object); F.Actor.SetupGet(x => x.HealthStrategy).Returns(strategy);
			F.Actor.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns<IWound>(w => strategy.GetSeverityFor(w, F.Actor.Object));
			F.Body.SetupGet(x => x.Race).Returns(race.Object); F.Body.Setup(x => x.HitpointsForBodypart(Part.Object)).Returns(1000);
			Body = TestObjectFactory.CreateUninitialized<NativeBody>();
			typeof(ConcreteBody).GetProperty(nameof(Body.Prototype))!.SetValue(Body,
				Mock.Of<IBodyPrototype>(x => x.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
			typeof(ConcreteBody).GetProperty(nameof(Body.Gameworld))!.SetValue(Body, F.World.Object);
			typeof(PerceivedItem).GetProperty(nameof(Body.EffectHandler))!.SetValue(Body, new EffectHandler(Body));
			foreach (var name in new[] { "_traits", "_merits", "_limbs", "_wounds", "_bodyparts", "_bones", "_organs", "_allItems", "_wornItems", "_wornItemsOnly", "_cachedEffects",
				"_cachedOrganFunctionsByOrgan", "_cachedOrganFunctionsByType", "_implants", "_prosthetics", "_externalItems", "_heldItems", "_wieldedItems" })
			{ var field = Field(name); field.SetValue(Body, Activator.CreateInstance(field.FieldType)); }
			((ICollection<IBodypart>)Field("_bodyparts").GetValue(Body)!).Add(Part.Object);
			Field("_healthStrategy").SetValue(Body, strategy); typeof(ConcreteBody).GetProperty(nameof(Body.Race))!.SetValue(Body, race.Object);
			Body.Actor = Victim.Object;
			Victim.SetupGet(x => x.Id).Returns(201); Victim.SetupGet(x => x.InstanceId).Returns(201); Victim.SetupGet(x => x.Body).Returns(Body);
			Victim.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(other => ReferenceEquals(other, Victim.Object));
			Victim.SetupGet(x => x.Identity).Returns((ICharacterIdentity)null!);
			Victim.SetupGet(x => x.Gameworld).Returns(F.World.Object); Victim.SetupGet(x => x.GetObject).Returns(Victim.Object);
			Victim.SetupGet(x => x.State).Returns(CharacterState.Awake); Victim.SetupGet(x => x.Race).Returns(race.Object);
			Victim.SetupGet(x => x.Location).Returns(F.Actor.Object.Location); Victim.SetupGet(x => x.Wounds).Returns(() => Body.Wounds);
			Victim.SetupGet(x => x.HealthStrategy).Returns(strategy); Mock.Get(Victim.Object.Account).SetupGet(x => x.InnerLineFormatLength).Returns(100);
			Victim.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns<IWound>(w => strategy.GetSeverityFor(w, Victim.Object));
			Victim.Setup(x => x.ProcessPassiveWound(It.IsAny<IWound>())).Callback<IWound>(Body.ProcessPassiveWound);

			Victim.SetupGet(x => x.Effects).Returns(Effects);
			Victim.SetupProperty(x => x.EffectsChanged);
			Victim.Setup(x => x.EffectsOfType<IMagicArmour>(It.IsAny<Predicate<IMagicArmour>>())).Returns(() => Effects.OfType<IMagicArmour>());
			Victim.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Victim.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => Effects.Add(effect));
			Victim.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { Effects.Remove(effect); if (fire) effect.RemovalEffect(); });
			F.Actor.Setup(x => x.TargetActorOrCorpse("target")).Returns(Victim.Object);
			var native = new XElement("Definition");
			foreach (var name in new[] { "DissipateExpressions", "AbsorbExpressions", "DissipateExpressionsPain", "AbsorbExpressionsPain", "DissipateExpressionsStun", "AbsorbExpressionsStun" })
				native.Add(new XElement(name, new XElement("Expression", new XAttribute("damagetype", (int)DamageType.Slashing),
					name.StartsWith("Dissipate") ? "damage" : name.EndsWith("Pain") ? "damage*0.25" : name.EndsWith("Stun") ? "damage*0.125" : "damage*0.5")));
			native.Add(new XElement("DamageTransformations", new XElement("Transform", new XAttribute("fromtype", (int)DamageType.Slashing), new XAttribute("totype", (int)DamageType.Crushing), new XAttribute("severity", (int)WoundSeverity.Horrifying))));
			var armourType = new ArmourType(new MudSharp.Models.ArmourType { Id = 9, Name = "Authored native test profile", MinimumPenetrationDegree = (int)OpposedOutcomeDegree.Major, Definition = native.ToString() }, F.World.Object);
			var material = Mock.Of<ISolid>(x => x.Id == 9);
			F.World.SetupGet(x => x.ArmourTypes).Returns(MagicCastingFixture.Collection<IArmourType>(() => [armourType]));
			F.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection<ISolid>(() => [material]));
			F.World.Setup(x => x.GetCheck(CheckType.PenetrationDefenseCheck)).Returns(F.Check.Object);
			SpellArmourEffect.RegisterFactory(); F.Spells.Remove(F.Spell);
			long[] rankIds = [];
			if (sourceMaterial)
			{
				var ranks = Enumerable.Range(0, 5).Select(index =>
				{
					var tag = new Mock<ITag>(); tag.SetupGet(x => x.Id).Returns(index + 1);
					tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag other) => other.Id <= index + 1);
					return tag.Object;
				}).ToArray();
				for (var index = 1; index < ranks.Length; index++) Mock.Get(ranks[index]).SetupGet(x => x.Parent).Returns(ranks[index - 1]);
				rankIds = ranks.Select(x => x.Id).ToArray();
				F.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => ranks));
				var water = Mock.Of<IPlane>(x => x.Id == 301);
				F.World.SetupGet(x => x.Planes).Returns(MagicCastingFixture.Collection(() => new[] { water }));
				F.Body.SetupGet(x => x.BasePlanarPresence).Returns(() => PlanarPresenceDefinition.DefaultMaterial(WaterPresence ? new long[] { 1, 301 } : new long[] { 1 }));
				var item = new Mock<IGameItem>(); var stack = new Mock<IStackable>(); var prototype = new Mock<IGameItemProto>();
				prototype.SetupGet(x => x.Id).Returns(999); prototype.SetupGet(x => x.Tags).Returns(new[] { ranks[componentRank] });
				item.SetupGet(x => x.Id).Returns(123); item.SetupGet(x => x.Prototype).Returns(prototype.Object);
				item.SetupGet(x => x.Tags).Returns(new[] { ranks[componentRank] });
				item.SetupGet(x => x.Components).Returns(new[] { stack.Object });
				item.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
				item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag tag) => tag.Id <= componentRank + 1);
				item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns((IGameItem other) => ReferenceEquals(other, item.Object));
				item.SetupGet(x => x.InInventoryOf).Returns(F.Body.Object);
				item.SetupGet(x => x.Quantity).Returns(() => ComponentQuantity);
				item.SetupGet(x => x.DeepItems).Returns(new[] { item.Object });
				stack.SetupGet(x => x.Quantity).Returns(() => ComponentQuantity);
				stack.SetupSet(x => x.Quantity = It.IsAny<int>()).Callback<int>(value => { ComponentWrites++; ComponentQuantity = value; });
				Components.Add(item.Object); F.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => Components));
				F.Actor.SetupGet(x => x.Inventory).Returns(() => Components);
				F.Body.SetupGet(x => x.HeldItems).Returns(() => Components);
				F.Body.SetupGet(x => x.HeldOrWieldedItems).Returns(() => Components);
				F.Body.SetupGet(x => x.WieldedItems).Returns([]); F.Body.SetupGet(x => x.WornItems).Returns([]);
			}
			var content = sourceMaterial ? ArmageddonArmourSpellContent.ShelteringVeil(9, 9, 301, rankIds) :
				ArmageddonArmourSpellContent.Create("arm.spell." + key, 9, 9, 3);
			var cost = content.CostRow(1); cost.Id = 1; var duration = content.DurationRow(1); duration.Id = 2;
			F.Expressions.RemoveAll(x => x.Id is 1 or 2); F.Expressions.Add(new TraitExpression(cost, F.World.Object)); F.Expressions.Add(new TraitExpression(duration, F.World.Object));
			var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = content.BuildDefinition(10, 1, 1).ToString();
			Spell = new MagicSpell(row, F.World.Object); Assert.IsTrue(Spell.ReadyForGame, Spell.ReadyForGame ? "" : Spell.WhyNotReadyForGame(F.Actor.Object)); F.Spells.Add(Spell); F.Skills[1] = 100; F.Acquire(7); F.Balances[F.Resources[1]] = 1000;
		}
		internal MagicCastingResult Cast(int grade)
		{
			var result = F.Service.Cast(new MagicCastingIntent(F.Actor.Object, F.Earth.Id, 1, grade, false, "target", OriginId: Guid.NewGuid()));
			Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message + string.Join(";", F.Messages));
			return result;
		}
		internal Damage Packet(double amount, IDamageMutationGuard? guard = null) => new() { DamageAmount = amount, PainAmount = amount, StunAmount = amount,
			DamageType = DamageType.Slashing, Bodypart = Part.Object, TargetBody = Body, ActorOrigin = F.Actor.Object, MutationGuard = guard! };
		internal void Hit(double amount) { foreach (var wound in Body.PassiveSufferDamage(Packet(amount)).ToArray()) Body.ProcessPassiveWound(wound); }
		private static FieldInfo Field(string name)
		{
			for (var type = typeof(ConcreteBody); type is not null; type = type.BaseType)
				if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) is { } field) return field;
			throw new InvalidOperationException(name);
		}
	}
	private sealed class NativeBody : ConcreteBody
	{
		private NativeBody() : base(null!, null!, (ICharacterTemplate)null!) { }
		public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
			bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => "a native test body";
	}
}
