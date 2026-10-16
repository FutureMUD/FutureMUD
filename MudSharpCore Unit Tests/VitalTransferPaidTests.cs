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
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class VitalTransferPaidTests
{
	[DataTestMethod, DataRow(false, 1), DataRow(false, 7), DataRow(true, 1), DataRow(true, 7)]
	public void PaidNativeCast_GradesApplyOnceAndOriginCannotReplay(bool edge, int grade)
	{
		var f = new Fixture(edge);
		var intent = f.Intent(grade);
		var result = f.F.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(100.0 - 5 * grade, f.F.Balances[f.F.Resources[1]]);
		if (edge)
		{
			var child = f.ItemEffects.OfType<SpellLeechingEdgeEffect>().Single();
			Assert.AreEqual(Math.Min(grade * 0.1, 0.5), child.Fraction, 0.000001);
			Assert.AreEqual(TimeSpan.FromSeconds(60 * grade), f.ItemEffects.OfType<MagicSpellParent>().Single().ResolvedDuration);
			f.Body.CommandSufferDamage(new Damage { ActorOrigin = f.F.Actor.Object, Bodypart = f.Part.Object,
				ToolOrigin = f.Item.Object, DamageType = DamageType.Arcane, DamageAmount = 20 }).ToArray();
			Assert.AreEqual(20, f.Body.Wounds.Single().CurrentDamage);
			Assert.AreEqual(100 - 20 * child.Fraction, f.SourceWound.CurrentDamage, 0.000001);
		}
		else
		{
			Assert.AreEqual(5.0 * grade, f.Body.Wounds.Single().CurrentDamage);
			Assert.AreEqual(100.0 - 5 * grade, f.SourceWound.CurrentDamage);
		}
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
		Assert.AreEqual(100.0 - 5 * grade, f.F.Balances[f.F.Resources[1]]);
	}

	[DataTestMethod, DataRow("self"), DataRow("wrongworld"), DataRow("wrongbody"), DataRow("nonweapon"), DataRow("destroyed")]
	public void InvalidNativeRecipient_RefusesBeforeAnyDebit(string cause)
	{
		var edge = cause is "nonweapon" or "destroyed";
		var f = new Fixture(edge);
		if (cause == "self") f.F.Actor.Setup(x => x.TargetActorOrCorpse("target")).Returns(f.F.Actor.Object);
		if (cause == "wrongworld") f.Victim.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		if (cause == "wrongbody") f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		if (cause == "nonweapon") f.Item.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns((IMeleeWeapon)null!);
		if (cause == "destroyed") f.Item.SetupGet(x => x.Destroyed).Returns(true);
		var result = f.F.Service.Cast(f.Intent(1));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.IsFalse(f.Body.Wounds.Any()); Assert.IsFalse(f.ItemEffects.Any());
	}

	[DataTestMethod, DataRow(false, "BeforePayment"), DataRow(false, "Committed"), DataRow(true, "BeforePayment"), DataRow(true, "Committed")]
	public void CapturedNativeSelectionDrifts_ZeroDebitOrPaidUncertaintyWithoutReplay(bool edge, string stage)
	{
		var f = new Fixture(edge);
		var checkpointReached = false;
		f.F.Checkpoint = current =>
		{
			if (current != stage) return;
			checkpointReached = true;
			if (edge) f.Item.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns(Mock.Of<IMeleeWeapon>(x => x.Parent == f.Item.Object));
			else f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Actor == f.Victim.Object));
		};
		var intent = f.Intent(1);
		var result = f.F.Service.Cast(intent);
		Assert.IsTrue(checkpointReached, result.Message);
		Assert.AreEqual(stage == "BeforePayment" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(stage == "BeforePayment" ? 100.0 : 95.0, f.F.Balances[f.F.Resources[1]]);
		Assert.IsFalse(f.Body.Wounds.Any()); Assert.IsFalse(f.ItemEffects.Any()); Assert.AreEqual(100, f.SourceWound.CurrentDamage);
		if (stage == "Committed")
		{
			Assert.AreEqual("NeedsReview", f.F.Store.Operation(result.OperationId!.Value)!.Stage);
			Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
			Assert.AreEqual(95.0, f.F.Balances[f.F.Resources[1]]);
		}
	}

	[TestMethod]
	public void Siphon_FinalAuthorityAfterBodypartSelectionCannotHarmStaleBody()
	{
		var f = new Fixture(false);
		var source = OrderedSource(f);
		var selected = false; var invalidated = false;
		var oldBody = new Mock<IBody>(); oldBody.SetupGet(x => x.Actor).Returns(f.Victim.Object);
		oldBody.SetupGet(x => x.RandomBodypart).Returns(() => { selected = true; return f.Part.Object; });
		f.Victim.SetupGet(x => x.Body).Returns(oldBody.Object);
		var authority = CommandExecutionAuthority.Prepare(source.Object, source.Object, "siphon", () =>
		{
			if (selected) { invalidated = true; f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()); }
			return true;
		});
		using var scope = CommandExecutionScope.EnterDispatch(authority, source.Object);
		var copy = f.Spell.CastingCopy(source.Object, f.F.Traits[0], 1, SpellPower.ExtremelyWeak, Difficulty.Easy, 7);
		var result = ((VitalSiphonEffect)copy.SpellEffects.Single()).Apply(source.Object, f.Victim.Object,
			OpposedOutcomeDegree.Total, SpellPower.Standard, Mock.Of<IMagicSpellEffectParent>(), []);
		Assert.IsTrue(invalidated); Assert.AreEqual(MagicEffectOperationStatus.Rejected, result.Status);
		oldBody.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
	}

	[TestMethod]
	public void NativeBody_LastAuthorityCheckpointStalesActiveReceipt_AbortsBeforeWoundMutation()
	{
		var f = new Fixture(false);
		var source = OrderedSource(f);
		var nativeCheckpointReached = false; var invalidated = false;
		f.Part.SetupGet(x => x.NaturalArmourType).Returns(() => { nativeCheckpointReached = true; return null!; });
		var authority = CommandExecutionAuthority.Prepare(source.Object, source.Object, "siphon", () =>
		{
			if (nativeCheckpointReached) { invalidated = true; f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()); }
			return true;
		});
		using var scope = CommandExecutionScope.EnterDispatch(authority, source.Object);
		using var receipt = new VitalTransferReceipt(source.Object, f.Victim.Object);
		Assert.ThrowsException<InvalidOperationException>(() => f.Body.PassiveSufferDamage(new Damage
		{
			ActorOrigin = source.Object, TargetBody = f.Body, Bodypart = f.Part.Object,
			DamageType = DamageType.Arcane, DamageAmount = 5, VitalTransfer = receipt
		}).ToArray());
		Assert.IsTrue(invalidated); Assert.IsFalse(f.Body.Wounds.Any()); Assert.AreEqual(0, receipt.DeliveredDamage);
		Assert.AreEqual(0, receipt.Settle(1, 5)); Assert.IsTrue(CommandExecutionScope.HasCommitted);
	}

	private static Mock<ICharacter> OrderedSource(Fixture f)
	{
		var source = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock }; source.As<ICharacterInstance>();
		var sourceBody = Mock.Of<IBody>(x => x.Actor == source.Object);
		source.SetupGet(x => x.Body).Returns(sourceBody); source.SetupGet(x => x.Gameworld).Returns(f.F.World.Object);
		source.SetupGet(x => x.Id).Returns(300); source.SetupGet(x => x.InstanceId).Returns(300); source.SetupGet(x => x.State).Returns(CharacterState.Awake);
		var identity = Mock.Of<ICharacterIdentity>(x => x.Id == 300 && x.Instances == new[] { (ICharacterInstance)source.Object });
		source.SetupGet(x => x.Identity).Returns(identity);
		var room = Mock.Of<IRoom>(x => x.Characters == new[] { source.Object }); source.SetupGet(x => x.Location).Returns(room);
		var roots = new All<ICharacter>(); roots.Add(source.Object);
		f.F.World.SetupGet(x => x.Actors).Returns(roots); f.F.World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
		f.F.World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
		return source;
	}

	private sealed class Fixture
	{
		internal MagicCastingFixture F { get; } = new();
		internal Mock<ICharacter> Victim { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<IExternalBodypart> Part { get; } = new();
		internal Mock<IGameItem> Item { get; } = new();
		internal List<IEffect> ItemEffects { get; } = [];
		internal NativeBody Body { get; }
		internal SimpleOrganicWound SourceWound { get; }
		internal MagicSpell Spell { get; }
		internal Fixture(bool edge)
		{
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
				"_cachedOrganFunctionsByOrgan", "_cachedOrganFunctionsByType", "_implants", "_prosthetics", "_externalItems", "_heldItems", "_wieldedItems" })
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
			Spell = F.NewSpell(1, "Measured vitality", edge ? "<Effect type='leechingedge'><FractionPerGrade>0.1</FractionPerGrade><MaximumFraction>0.5</MaximumFraction></Effect>" :
				$"<Effect type='vitalsiphon'><DamageType>{(int)DamageType.Arcane}</DamageType><DamagePerGrade>5</DamagePerGrade><HealingCapPerGrade>5</HealingCapPerGrade></Effect>");
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("trigger new " + (edge ? "item" : "character"))));
			Spell.EffectDurationExpression = new TraitExpression(edge ? "60*grade" : "0", F.World.Object); F.Skills[1] = 100; F.Acquire(7);
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
}
