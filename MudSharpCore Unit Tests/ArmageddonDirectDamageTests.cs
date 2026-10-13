#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Form.Material;
using MudSharp.Combat;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Capabilities;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.FutureProg;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class ArmageddonDirectDamageTests
{
	[DataTestMethod]
	[DataRow("arm.spell.ember_lance", 1, 5, 2, 20)]
	[DataRow("arm.spell.ember_lance", 7, 5, 2, 20)]
	[DataRow("arm.spell.pillar_of_flame", 1, 8, 3, 7)]
	[DataRow("arm.spell.pillar_of_flame", 7, 8, 3, 7)]
	[DataRow("arm.spell.lightning_lance", 1, 5, 2, 20)]
	[DataRow("arm.spell.lightning_lance", 7, 5, 2, 20)]
	[DataRow("arm.spell.mind_scour", 1, 5, 2, 20)]
	[DataRow("arm.spell.mind_scour", 7, 5, 2, 20)]
	public void FactoryPaidNativeCast_ExactGradeInjuryAndIndependentPain(string key, int grade, int damage, int pain, int minimum)
	{
		var f = new Fixture(key);
		f.F.Balances[f.F.Resources[1]] = 1000;
		var intent = f.Intent(grade);
		var result = f.F.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1000 - new ControlledSpellEfficiency(minimum, 1).Cost(7, grade), f.F.Balances[f.F.Resources[1]], 0.000001);
		var wound = f.Body.Wounds.Single();
		Assert.AreEqual((double)damage * grade, wound.CurrentDamage);
		Assert.AreEqual((double)pain * grade, wound.CurrentPain);
		Assert.AreEqual(0.0, wound.CurrentStun);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
		Assert.AreEqual(1, f.Body.Wounds.Count());
	}

	[DataTestMethod, DataRow("trait"), DataRow("bodypart")]
	public void PaidExpressionOrBodypartCallback_CannotRebindFrozenVictim(string callback)
	{
		var f = new Fixture(); var committed = false; var invalidated = false;
		f.F.Checkpoint = stage => { if (stage == "Committed") committed = true; };
		if (callback == "trait")
			f.F.Actor.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>()))
				.Returns<ITraitDefinition, TraitBonusContext>((trait, _) => { if (committed) { invalidated = true; f.Drift("body"); } return f.F.Skills[trait.Id]; });
		else f.Part.SetupGet(x => x.RelativeHitChance).Returns(() => { if (committed) { invalidated = true; f.Drift("body"); } return 1; });
		var result = f.F.Service.Cast(f.Intent(1));
		Assert.IsTrue(invalidated); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(f.Body.Wounds.Any()); Assert.AreEqual(80.0, f.F.Balances[f.F.Resources[1]]);
	}

	[TestMethod]
	public void InsufficientReserve_RefusesWithNoNativeMutation()
	{
		var f = new Fixture(); f.F.Balances[f.F.Resources[1]] = 0;
		var result = f.F.Service.Cast(f.Intent(1));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status); Assert.AreEqual(0.0, f.F.Balances[f.F.Resources[1]]);
		Assert.IsFalse(f.Body.Wounds.Any());
	}

	[TestMethod]
	public void NativeBodyFinalAuthorityCallback_QuarantinesWithoutStaleInjury()
	{
		var f = new Fixture(); var reached = false; var invalidated = false;
		f.Part.SetupGet(x => x.NaturalArmourType).Returns(() => { reached = true; return null!; });
		var authority = CommandExecutionAuthority.Prepare(f.F.Actor.Object, f.F.Actor.Object, "direct damage", () =>
		{
			if (reached && !invalidated) { invalidated = true; f.Drift("body"); }
			return true;
		});
		using var scope = CommandExecutionScope.EnterDispatch(authority, f.F.Actor.Object);
		var intent = f.Intent(1); var result = f.F.Service.Cast(intent);
		Assert.IsTrue(invalidated); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(f.Body.Wounds.Any()); Assert.AreEqual(80.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
	}

	[DataTestMethod, DataRow(false), DataRow(true)]
	public void NativeGearPositiveControl_UsesActualItemWounds(bool prosthetic)
	{
		var f = new Fixture(); var gear = f.InstallNativeGear(prosthetic);
		var result = f.F.Service.Cast(f.Intent(1));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsInstanceOfType(gear.GetItemType<IDestroyable>(), typeof(DestroyableGameItemComponent));
		Assert.AreEqual(5.0, gear.Wounds.Sum(x => x.CurrentDamage), 0.000001);
		Assert.IsInstanceOfType(gear.Wounds.Single(), typeof(SimpleWound));
		Assert.AreEqual(prosthetic ? 0.0 : 2.5, f.Body.Wounds.Sum(x => x.CurrentDamage), 0.000001);
	}

	[DataTestMethod, DataRow(false, "body"), DataRow(false, "gear"), DataRow(true, "body"), DataRow(true, "gear")]
	public void NativeItemFinalAuthorityCallback_CannotRedirectBodyOrGear(bool prosthetic, string change)
	{
		var f = new Fixture(); var gear = f.InstallNativeGear(prosthetic); var checkpoints = 0; var invalidated = false;
		var authority = CommandExecutionAuthority.Prepare(f.F.Actor.Object, f.F.Actor.Object, "native direct gear", () =>
		{
			if (new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(GameItem) && x.GetMethod()!.Name == nameof(GameItem.PassiveSufferDamage))) checkpoints++;
			if (checkpoints != 2 || invalidated) return true;
			invalidated = true;
			if (change == "body") f.Drift("body");
			else f.RemoveNativeGear();
			return true;
		});
		using var scope = CommandExecutionScope.EnterDispatch(authority, f.F.Actor.Object);
		var intent = f.Intent(1); var result = f.F.Service.Cast(intent);
		Assert.IsTrue(checkpoints >= 2); Assert.IsTrue(invalidated);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(gear.Wounds.Any()); Assert.IsFalse(f.Body.Wounds.Any());
		Assert.AreEqual(80.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
	}

	[TestMethod]
	public void Factory_MindScourRequiresExactConfiguredNativeResistance()
	{
		var f = new Fixture("arm.spell.mind_scour");
		Assert.AreEqual(1L, f.Spell.SnapshotModel().ResistingTraitDefinitionId);
		Assert.AreEqual((int)Difficulty.Normal, f.Spell.SnapshotModel().ResistingDifficulty);
		Assert.AreEqual(DamageType.Arcane, ((DamageEffect)f.Spell.SpellEffects.Single()).DamageType);
	}

	[DataTestMethod, DataRow("body"), DataRow("location"), DataRow("layer"), DataRow("registry")]
	public void FrozenSelectionBeforePayment_RefusesWithoutDebitOrWounds(string drift)
	{
		var f = new Fixture();
		f.F.Checkpoint = stage => { if (stage == "BeforePayment") f.Drift(drift); };
		var result = f.F.Service.Cast(f.Intent(1));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.IsFalse(f.Body.Wounds.Any());
	}

	[DataTestMethod, DataRow("body"), DataRow("location"), DataRow("layer"), DataRow("registry")]
	public void FrozenSelectionAfterPayment_QuarantinesWithoutMutationOrReplay(string drift)
	{
		var f = new Fixture();
		f.F.Checkpoint = stage => { if (stage == "Committed") f.Drift(drift); };
		var intent = f.Intent(1);
		var result = f.F.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsFalse(f.Body.Wounds.Any());
		var paid = f.F.Balances[f.F.Resources[1]];
		Assert.IsTrue(paid < 100);
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
		Assert.AreEqual(paid, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual("NeedsReview", f.F.Store.Operation(result.OperationId!.Value)!.Stage);
	}

	[DataTestMethod, DataRow("-1"), DataRow("1/0"), DataRow("0/0")]
	public void FixedInvalidIndependentPain_RefusesBeforePayment(string formula)
	{
		var f = new Fixture();
		((DamageEffect)f.Spell.SpellEffects.Single()).PainExpression = new TraitExpression(formula, f.F.World.Object);
		Assert.IsFalse(f.Spell.ReadyForGame);
		StringAssert.Contains(string.Join(" ", f.Spell.GradeConfigurationErrors()), "finite");
		var result = f.F.Service.Cast(f.Intent(1));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.IsFalse(f.Body.Wounds.Any());
	}

	[TestMethod]
	public void SavedDefinition_ColdFactoryRetainsOptInAndIndependentExpressions()
	{
		var f = new Fixture();
		var cold = new MagicSpell(f.Spell.SnapshotModel(), f.F.World.Object);
		var effect = (DamageEffect)cold.SpellEffects.Single();
		Assert.IsTrue(effect.IdentitySafeDamage);
		Assert.AreEqual("5*grade", effect.DamageExpression.OriginalFormulaText);
		Assert.AreEqual("2*grade", effect.PainExpression!.OriginalFormulaText);
		Assert.AreEqual("0", effect.StunExpression!.OriginalFormulaText);
		CollectionAssert.AreEqual(new[] { "DamageExpression", "PainExpression", "StunExpression" },
			MudSharp.Magic.Vancian.ScrollSpellCompatibility.Expressions(effect).Select(x => x.Field).ToArray());
	}

	[TestMethod]
	public void LegacyDefinition_PreservesAbsentPainStunAndPolicy()
	{
		var f = new Fixture();
		var legacy = f.F.NewSpell(9, "legacy", "<Effect type='damage'><DamageType>22</DamageType><DamageExpression>3</DamageExpression><Bodypart>1</Bodypart><Limb>-1</Limb></Effect>");
		var effect = (DamageEffect)legacy.SpellEffects.Single();
		Assert.IsFalse(effect.IdentitySafeDamage); Assert.IsNull(effect.PainExpression); Assert.IsNull(effect.StunExpression);
		IDamage? delivered = null;
		f.Victim.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(d => { delivered = d; return []; });
		effect.Apply(f.F.Actor.Object, f.Victim.Object, OpposedOutcomeDegree.Total, SpellPower.Standard, Mock.Of<IMagicSpellEffectParent>(), []);
		Assert.IsNotNull(delivered); Assert.AreEqual(3.0, delivered.DamageAmount);
		Assert.AreEqual(3.0, delivered.PainAmount); Assert.AreEqual(3.0, delivered.StunAmount);
		Assert.IsNull(delivered.MutationGuard);
	}


	[TestMethod]
	public void UnsupportedPolicyVersion_FailsClosedDuringFactoryLoad()
	{
		var f = new Fixture();
		Assert.ThrowsException<FormatException>(() => f.F.NewSpell(9, "unknown policy",
			"<Effect type='damage'><DamageType>22</DamageType><DamageExpression>5</DamageExpression><IdentitySafeDamage schema='1' version='2'/></Effect>"));
	}

	[TestMethod]
	public void Builder_IndependentFieldsAndOptInRemainEditable()
	{
		var f = new Fixture(); var effect = (DamageEffect)f.Spell.SpellEffects.Single();
		Assert.IsTrue(effect.BuildingCommand(f.F.Actor.Object, new StringStack("pain damage")));
		Assert.IsNull(effect.PainExpression); Assert.IsNull(effect.SaveToXml().Element("PainExpression"));
		Assert.IsTrue(effect.BuildingCommand(f.F.Actor.Object, new StringStack("stun 3*grade")));
		Assert.AreEqual("3*grade", effect.StunExpression!.OriginalFormulaText);
		Assert.IsTrue(effect.BuildingCommand(f.F.Actor.Object, new StringStack("identitysafe")));
		Assert.IsFalse(effect.IdentitySafeDamage); Assert.IsTrue(f.Spell.Changed);
	}

	[TestMethod]
	public void MindScourNativeResistance_SuppressesPaidInjury()
	{
		var f = new Fixture("arm.spell.mind_scour");
		f.F.Check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable, double, TraitUseType, (string, object)[]>((owner, _, _, _, _, _, _) =>
				Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck,
					ReferenceEquals(owner, f.Victim.Object) ? Outcome.MajorPass : Outcome.MinorPass)));
		var result = f.F.Service.Cast(f.Intent(1));
		Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.IsFalse(f.Body.Wounds.Any()); Assert.AreEqual(80.0, f.F.Balances[f.F.Resources[1]]);
	}


	[TestMethod]
	public void IdentitySafeDamage_StoredAdapterRefusesUnqualifiedLiveContext()
	{
		var f = new Fixture();
		Assert.IsFalse(f.Spell.ScrollInscriptionAllowed);
		Assert.IsTrue(MudSharp.Magic.Vancian.ScrollSpellCompatibility.Errors(f.Spell, false)
			.Any(x => x.Contains("live prepared victim", StringComparison.Ordinal)));
	}

	private sealed class Fixture
	{
		internal DirectDamageCastingFixture F { get; } = new();
		internal Mock<ICharacter> Victim { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<IExternalBodypart> Part { get; } = new();
		internal Mock<IGameItem> Item { get; } = new();
		internal List<IEffect> ItemEffects { get; } = [];
		internal NativeBody Body { get; }
		internal SimpleOrganicWound SourceWound { get; }
		internal MagicSpell Spell { get; }
		internal Fixture(string key = "arm.spell.ember_lance")
		{
			Victim.As<ICharacterInstance>();
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object);
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
			SourceWound = new(F.World.Object, F.Actor.Object, 100, 1, 0, DamageType.Cellular, Part.Object, null, null, null);
			F.Body.SetupGet(x => x.Wounds).Returns([SourceWound]);
			Body = TestObjectFactory.CreateUninitialized<NativeBody>();
			typeof(ConcreteBody).GetProperty(nameof(Body.Prototype))!.SetValue(Body,
				Mock.Of<IBodyPrototype>(x => x.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
			typeof(ConcreteBody).GetProperty(nameof(Body.Gameworld))!.SetValue(Body, F.World.Object);
			typeof(PerceivedItem).GetProperty(nameof(Body.EffectHandler))!.SetValue(Body, new EffectHandler(Body));
			foreach (var name in new[] { "_traits", "_merits", "_limbs", "_wounds", "_bodyparts", "_bones", "_organs", "_allItems", "_wornItems", "_cachedEffects",
				"_cachedOrganFunctionsByOrgan", "_cachedOrganFunctionsByType", "_implants", "_prosthetics", "_externalItems", "_heldItems", "_wieldedItems", "_wornItemsOnly", "_directWornItems" })
			{ var field = Field(name); field.SetValue(Body, Activator.CreateInstance(field.FieldType)); }
			((ICollection<IBodypart>)Field("_bodyparts").GetValue(Body)!).Add(Part.Object);
			Field("_healthStrategy").SetValue(Body, strategy); typeof(ConcreteBody).GetProperty(nameof(Body.Race))!.SetValue(Body, race.Object);
			Body.Actor = Victim.Object;
			Victim.SetupGet(x => x.Id).Returns(201); Victim.SetupGet(x => x.InstanceId).Returns(201); Victim.SetupGet(x => x.Body).Returns(Body);
			Victim.SetupGet(x => x.Identity).Returns((ICharacterIdentity)null!);
			Victim.SetupGet(x => x.Gameworld).Returns(F.World.Object); Victim.SetupGet(x => x.GetObject).Returns(Victim.Object);
			Victim.SetupGet(x => x.State).Returns(CharacterState.Awake); Victim.SetupGet(x => x.Race).Returns(race.Object);
			Victim.SetupGet(x => x.Location).Returns(F.Actor.Object.Location); Victim.SetupGet(x => x.Wounds).Returns(() => Body.Wounds);
			Victim.SetupGet(x => x.HealthStrategy).Returns(strategy); Mock.Get(Victim.Object.Account).SetupGet(x => x.InnerLineFormatLength).Returns(100);
			Victim.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns<IWound>(w => strategy.GetSeverityFor(w, Victim.Object));
			Victim.Setup(x => x.ProcessPassiveWound(It.IsAny<IWound>())).Callback<IWound>(Body.ProcessPassiveWound);
			Item.SetupGet(x => x.Id).Returns(301); Item.SetupGet(x => x.Gameworld).Returns(F.World.Object); Item.SetupGet(x => x.GetObject).Returns(Item.Object);
			Item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Item.SetupGet(x => x.Location).Returns(F.Actor.Object.Location); Item.SetupGet(x => x.Effects).Returns(ItemEffects);
			var weapon = Mock.Of<IMeleeWeapon>(x => x.Parent == Item.Object); Item.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns(weapon);
			Item.SetupGet(x => x.Components).Returns([weapon]);
			Item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(ItemEffects.Add);
			Item.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => ItemEffects.Add(effect));
			F.Body.SetupGet(x => x.WieldedItems).Returns([Item.Object]);
			F.Actor.Setup(x => x.TargetActorOrCorpse("target")).Returns(Victim.Object); F.Actor.Setup(x => x.TargetItem("target")).Returns(Item.Object);
			F.Spells.Remove(F.Spell);
			var content = ArmageddonDirectDamageSpellContent.Create(new(1, Difficulty.Normal, DamageType.Arcane)).Single(x => x.Content.Key == key).Content;
			var cost = content.CostRow(1); cost.Id = 61; F.Expressions.Add(new TraitExpression(cost, F.World.Object));
			var duration = content.DurationRow(1); duration.Id = 62; F.Expressions.Add(new TraitExpression(duration, F.World.Object));
			var model = content.SpellRow(1, 1, 1); model.Id = 1; model.EffectDurationExpressionId = 62;
			model.Definition = content.BuildDefinition(10, 61, 0).ToString();
			Spell = new MagicSpell(model, F.World.Object); F.Spells.Add(Spell);
			F.Skills[1] = 100; F.Acquire(7);
			if (key == "arm.spell.mind_scour")
				F.Check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
					It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
					.Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable, double, TraitUseType, (string, object)[]>((owner, _, _, _, _, _, _) =>
						Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck,
							ReferenceEquals(owner, Victim.Object) ? Outcome.MajorFail : Outcome.MajorPass)));
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object);

			F.Actor.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 100 && x.Instances == new[] { (ICharacterInstance)F.Actor.Object }));
			Victim.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 201 && x.Instances == new[] { (ICharacterInstance)Victim.Object }));
			var actors = new All<ICharacter>(); actors.Add(F.Actor.Object); actors.Add(Victim.Object);
			F.World.SetupGet(x => x.Actors).Returns(actors); F.World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
			F.World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
			Mock.Get(F.Actor.Object.Location).SetupGet(x => x.Characters).Returns(new[] { F.Actor.Object, Victim.Object });
			F.World.SetupGet(x => x.BodypartPrototypes).Returns(MagicCastingFixture.Collection(() => new IBodypart[] { Part.Object }));

		}
		internal NativeGear InstallNativeGear(bool prosthetic)
		{
			GameItemHealthStrategy.RegisterHealthStrategyLoader();
			var strategy = BaseHealthStrategy.LoadStrategy(new MudSharp.Models.HealthStrategy
			{ Id = 91, Name = "Native item test", Type = "GameItem", Definition = "<Definition />" }, F.World.Object);
			var prototype = new Mock<IGameItemProto>(); prototype.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			prototype.SetupGet(x => x.Components).Returns([]); prototype.SetupGet(x => x.Name).Returns("contact gear");
			prototype.SetupGet(x => x.HealthStrategy).Returns(strategy); prototype.SetupGet(x => x.Material).Returns(Mock.Of<ISolid>());
			var gear = new NativeGear(prototype.Object);
			F.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new IGameItem[] { gear }));
			typeof(GameItem).GetProperty(nameof(GameItem.Location))!.SetValue(gear, F.Actor.Object.Location);
			var components = (List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(gear)!;
			var destroyablePrototype = (DestroyableGameItemComponentProto)typeof(DestroyableGameItemComponentProto)
				.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
				.Invoke([new MudSharp.Models.GameItemComponentProto { Id = 92, Name = "native destroyable", EditableItem = new MudSharp.Models.EditableItem(), Definition = "<Definition><HpExpression>1000</HpExpression><DamageMultipliers><DamageMultiplier type='" + (int)DamageType.Burning + "' multiplier='1' /></DamageMultipliers></Definition>" }, F.World.Object]);
			components.Add(new DestroyableGameItemComponent(destroyablePrototype, gear, true));
			if (prosthetic)
			{
				var replacement = Mock.Of<IProsthetic>(x => x.Parent == gear && x.TargetBodypart == Part.Object && x.InstalledBody == Body);
				components.Add(replacement); ((ICollection<IProsthetic>)Field("_prosthetics").GetValue(Body)!).Add(replacement); return gear;
			}
			var definition = new XElement("Definition");
			foreach (var name in new[] { "DissipateExpressions", "AbsorbExpressions", "DissipateExpressionsPain", "AbsorbExpressionsPain", "DissipateExpressionsStun", "AbsorbExpressionsStun" })
				definition.Add(new XElement(name, new XElement("Expression", new XAttribute("damagetype", (int)DamageType.Burning),
					name == "AbsorbExpressions" ? "damage*0.5" : name.EndsWith("Pain") || name.EndsWith("Stun") ? "0" : "damage")));
			var armourType = new ArmourType(new MudSharp.Models.ArmourType { Id = 9, Name = "Native worn armour", MinimumPenetrationDegree = (int)OpposedOutcomeDegree.Major, Definition = definition.ToString() }, F.World.Object);
			var armour = new Mock<IArmour>(); armour.SetupGet(x => x.Parent).Returns(gear); armour.SetupGet(x => x.ArmourType).Returns(armourType);
			armour.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>(), ref It.Ref<List<IWound>>.IsAny))
				.Returns(new ArmourDamage((IDamage damage, ref List<IWound> wounds) => armourType.AbsorbDamage(damage, armour.Object, Victim.Object, ref wounds, true)));
			components.Add(armour.Object);
			var wear = Mock.Of<IWear>(); Part.Setup(x => x.CountsAs(wear)).Returns(true);
			((List<(IGameItem, IWear, IWearlocProfile)>)Field("_wornItems").GetValue(Body)!).Add((gear, wear, Mock.Of<IWearlocProfile>()));
			((ICollection<IGameItem>)Field("_wornItemsOnly").GetValue(Body)!).Add(gear); return gear;
		}
		internal void RemoveNativeGear()
		{
			((ICollection<IGameItem>)Field("_wornItemsOnly").GetValue(Body)!).Clear();
			((List<(IGameItem, IWear, IWearlocProfile)>)Field("_wornItems").GetValue(Body)!).Clear();
			((ICollection<IProsthetic>)Field("_prosthetics").GetValue(Body)!).Clear();
		}

		internal void Drift(string kind)
		{
			if (kind == "body") Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Actor == Victim.Object));
			if (kind == "location") Victim.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
			if (kind == "layer") Victim.SetupGet(x => x.RoomLayer).Returns((RoomLayer)1);
			if (kind == "registry") F.World.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
		}
		internal MagicCastingIntent Intent(int grade) => new(F.Actor.Object, F.Earth.Id, 1, grade, false, "target", OriginId: Guid.NewGuid());
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
	private delegate IDamage ArmourDamage(IDamage damage, ref List<IWound> wounds);
	private sealed class NativeGear(IGameItemProto prototype) : GameItem(prototype)
	{
		public override string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short,
			bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => "native contact gear";
	}
}

internal sealed class DirectDamageCastingFixture
{
	public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<ICharacter> Staff { get; } = new();
	public Mock<IBody> Body { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<ICheck> Check { get; } = new();
	public Mock<ITrait> NativeSkill { get; } = new();
	public List<IMagicCapability> Capabilities { get; } = [];
	public List<IMagicCapability> ActiveCapabilities { get; } = [];
	public List<IMagicSpell> Spells { get; } = [];
	public List<ITraitDefinition> Traits { get; } = [];
	public List<ITraitExpression> Expressions { get; } = [];
	public List<IMagicResource> Resources { get; } = [];
	public Dictionary<IMagicResource, double> Balances { get; } = [];
	public Dictionary<long, double> Skills { get; } = new() { [1] = 42, [2] = 62, [3] = 10 };
	public List<string> Messages { get; } = [];
	public CastingMemoryStore Store { get; } = new();
	public MagicCastingService Service { get; private set; } = null!;
	public MagicSpell Spell { get; }
	public SkillLevelBasedMagicCapability Earth { get; }
	public SkillLevelBasedMagicCapability Sorcerer { get; }
	public IMagicSchool School { get; }
	public DateTime Now { get; set; } = new(2026, 9, 27, 1, 0, 0, DateTimeKind.Utc);
	public int SkillUses { get; private set; }
	public int Rolls { get; private set; }
	public int Flushes { get; private set; }
	public double RandomValue { get; set; } = 0.1;
	public int Samples { get; private set; }
	public Action<string>? Checkpoint { get; set; }
	public Outcome Outcome { get; set; } = Outcome.Pass;

	public DirectDamageCastingFixture()
	{
		Actor.As<ICharacterInstance>();
		var school = new Mock<IMagicSchool>();
		school.SetupGet(x => x.Id).Returns(1); school.SetupGet(x => x.Name).Returns("Earth");
		school.SetupGet(x => x.SchoolVerb).Returns("earth"); school.SetupGet(x => x.SchoolAdjective).Returns("earthen");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.Cyan);
		School = school.Object;
		for (var i = 1; i <= 3; ++i)
		{
			var trait = new Mock<ITraitDefinition>(); var id = i;
			trait.SetupGet(x => x.Id).Returns(id); trait.SetupGet(x => x.Name).Returns($"Proficiency {id}");
			trait.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
			trait.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
			Traits.Add(trait.Object);
		}
		foreach (var id in new[] { 10, 11, 12 })
		{
			var resource = new Mock<IMagicResource>(); resource.SetupGet(x => x.Id).Returns(id);
			resource.SetupGet(x => x.Name).Returns($"reserve {id}");
			resource.Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
			Resources.Add(resource.Object); Balances[resource.Object] = 100;
		}
		World.SetupGet(x => x.MagicSchools).Returns(Collection(() => new[] { School }));
		World.SetupGet(x => x.Traits).Returns(Collection(() => Traits));
		World.SetupGet(x => x.TraitExpressions).Returns(Collection(() => Expressions));
		World.SetupGet(x => x.MagicResources).Returns(Collection(() => Resources));
		World.SetupGet(x => x.MagicSpells).Returns(Collection(() => Spells));
		World.SetupGet(x => x.MagicCapabilities).Returns(Collection(() => Capabilities));
		World.SetupGet(x => x.Characters).Returns(Collection<ICharacter>(() => []));
		World.SetupGet(x => x.LegalAuthorities).Returns(Collection<ILegalAuthority>(() => []));
		World.SetupGet(x => x.DefaultPlane).Returns((IPlane)null!);
		var known = new Mock<IFutureProg>(); known.SetupGet(x => x.Id).Returns(1);
		known.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		World.SetupGet(x => x.FutureProgs).Returns(Collection(() => new[] { known.Object }));
		World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(Check.Object);
		World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(Check.Object);
		Check.SetupGet(x => x.MaximumDifficultyForImprovement).Returns(Difficulty.Impossible);
		Check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(() => { Rolls++; return Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome)); });
		Actor.SetupGet(x => x.Id).Returns(100); Actor.SetupGet(x => x.InstanceId).Returns(100);
		Actor.SetupGet(x => x.Identity).Returns((ICharacterIdentity)null!);
		Actor.SetupGet(x => x.GetObject).Returns(() => Actor.Object);
		Actor.SetupGet(x => x.Gameworld).Returns(World.Object); Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		Actor.SetupGet(x => x.Capabilities).Returns(() => ActiveCapabilities); Actor.SetupGet(x => x.Body).Returns(Body.Object);
		var room = new Mock<IRoom>() { DefaultValue = DefaultValue.Mock };
		Actor.SetupGet(x => x.Location).Returns(room.Object);
		Body.SetupGet(x => x.Id).Returns(200); Body.SetupGet(x => x.Gameworld).Returns(World.Object);
		Body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		Body.SetupGet(x => x.FunctioningFreeHands).Returns(new[] { new Mock<IGrab>().Object });
		Body.Setup(x => x.Communications.CanVocalise(Body.Object)).Returns(true);
		Body.Setup(x => x.Communications.CanVocalise(Body.Object, It.IsAny<MudSharp.Form.Audio.AudioVolume>()))
			.Returns(() => Body.Object.Communications.CanVocalise(Body.Object));
		Actor.Setup(x => x.CombinedEffectsOfType<MagicSpellLockout>()).Returns([]);
		Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([]);
		Actor.Setup(x => x.TraitRawValue(It.IsAny<ITraitDefinition>())).Returns<ITraitDefinition>(x => Skills.GetValueOrDefault(x.Id));
		Actor.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>()))
			.Returns<ITraitDefinition, TraitBonusContext>((x, _) => Skills.GetValueOrDefault(x.Id));
		Actor.Setup(x => x.HasTrait(It.IsAny<ITraitDefinition>())).Returns<ITraitDefinition>(x => Skills.ContainsKey(x.Id));
		Actor.Setup(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>())).Returns<ITraitDefinition, double>((t, n) => { Skills[t.Id] = n; return true; });
		Actor.Setup(x => x.GetTrait(It.IsAny<ITraitDefinition>())).Returns(NativeSkill.Object);
		NativeSkill.Setup(x => x.TraitUsed(Actor.Object, It.IsAny<Outcome>(), It.IsAny<Difficulty>(), It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()))
			.Returns(() => { SkillUses++; return true; });
		Actor.SetupGet(x => x.MagicResourceAmounts).Returns(Balances);
		Actor.Setup(x => x.CanUseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Returns<IMagicResource, double>((r, n) => Balances.GetValueOrDefault(r) >= n);
		Actor.Setup(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Returns<IMagicResource, double>((r, n) => { if (Balances[r] < n) return false; Balances[r] -= n; return true; });
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((s, _, _) => Messages.Add(s));
		Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		Staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true); Staff.SetupGet(x => x.Id).Returns(999);
		Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 1, Name = "Cost", Expression = "5*grade" }, World.Object));
		Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 2, Name = "Duration", Expression = "variable+grade*10" }, World.Object));
		Spell = NewSpell(1, "Stone Skin", "<Effect type='boost' trait='3' bonus='0' context='0' />");
		Assert.IsTrue(Spell.BuildingCommand(Actor.Object, new StringStack("grades fixture")));
		Assert.IsTrue(Spell.BuildingCommand(Actor.Object, new StringStack("grades scalar add target 0 boost Bonus -grade")));
		Earth = NewCapability(1, 1, 11, true); Sorcerer = NewCapability(2, 2, 12, false);
		ActiveCapabilities.Add(Earth); ActiveCapabilities.Add(Sorcerer);
		Restart();
	}

	public static IUneditableAll<T> Collection<T>(Func<System.Collections.Generic.IEnumerable<T>> values) where T : class, IFrameworkItem
	{
		var result = new Mock<IUneditableAll<T>>();
		result.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => values().FirstOrDefault(x => x.Id == id)!);
		result.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((s, _) => values().FirstOrDefault(x => x.Id.ToString() == s || x.Name.EqualTo(s))!);
		result.Setup(x => x.GetEnumerator()).Returns(() => values().GetEnumerator());
		result.As<System.Collections.IEnumerable>().Setup(x => x.GetEnumerator()).Returns(() => values().GetEnumerator());
		return result.Object;
	}
	public MagicSpell NewSpell(long id, string name, string effects)
	{
		var spell = new MagicSpell(new MudSharp.Models.MagicSpell
		{
			Id = id, Name = name, MagicSchoolId = 1, SpellKnownProgId = 1, CastingTraitDefinitionId = 1,
			CastingDifficulty = (int)Difficulty.Easy, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			EffectDurationExpressionId = 2, CastingEmote = "casting", FailCastingEmote = "failing", ExclusiveDelay = 5,
			Definition = $"<Spell><Trigger type='self'><MinimumPower>0</MinimumPower><MaximumPower>10</MaximumPower></Trigger><Costs><Cost resource='10' expression='1' /></Costs><Effects>{effects}</Effects><CasterEffects/><Plan><Phase/></Plan></Spell>"
		}, World.Object);
		Spells.Add(spell); return spell;
	}
	public SkillLevelBasedMagicCapability NewCapability(long id, long trait, long reserve, bool passive, XElement? casting = null)
	{
		var root = XElement.Parse("<Definition><ConcentrationTrait>1</ConcentrationTrait><ConcentrationCapabilityExpression>1</ConcentrationCapabilityExpression><ConcentrationDifficultyExpression>5</ConcentrationDifficultyExpression><Regenerators/></Definition>");
		root.Add(casting ?? new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()),
			new XAttribute("enabled", true), new XAttribute("trait", trait), new XAttribute("source", 10), new XAttribute("reserve", reserve),
			new XAttribute("passive", passive), new XAttribute("startingVersion", 1), Admission(1)));
		var capability = (SkillLevelBasedMagicCapability)MagicCapabilityFactory.LoadCapability(new MudSharp.Models.MagicCapability
		{ Id = id, Name = id == 1 ? "Earth" : "Sorcerer", MagicSchoolId = 1, PowerLevel = 1, CapabilityModel = "skilllevel", Definition = root.ToString() }, World.Object);
		Capabilities.Add(capability); return capability;
	}
	public static XElement Admission(long spell) => new("Admission", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", spell), new XAttribute("starting", true), new XAttribute("min", 1), new XAttribute("max", 7));
	public void Restart() { Service = new(World.Object, Store, () => Now, () => { Samples++; return RandomValue; }, x => Checkpoint?.Invoke(x), () => Flushes++); World.SetupGet(x => x.MagicCasting).Returns(Service); }
	public void Acquire(int grade = 2) => Store.Write(acquired: new(100, 1, grade, 1, Now, "test acquisition", DateTime.UnixEpoch, Store.Acquisition(100, 1)?.Version ?? 0));
	public MagicCastingIntent Intent(int grade = 3, bool overreach = true, long capability = 1) => new(Actor.Object, capability, 1, grade, overreach, "self");
}

