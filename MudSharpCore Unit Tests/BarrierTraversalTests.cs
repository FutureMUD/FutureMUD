#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Combat.Moves;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Form.Material;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.Movement;
using MudSharp.Construction.Boundary;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class BarrierTraversalTests
{
	[TestMethod]
	public void LateCrossingDenial_AfterInitialAdmissionStopsWithoutDamageOrRelocation()
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		var queries = 0;
		f.Victim.Setup(x => x.CanCross(f.Direction.Object)).Returns(() => (++queries == 1, null!));
		var movement = f.Move(); movement.IntermediateStep();
		Assert.AreEqual(2, queries, "The initial native movement query admitted, then final contact admission refused.");
		Assert.IsTrue(movement.Cancelled); Assert.IsFalse(f.Body.Wounds.Any());
		f.Victim.Verify(x => x.ExecuteMove(It.IsAny<IMovement>(), It.IsAny<IMoveSpeed>()), Times.Never);
	}
	[DataTestMethod, DataRow(false, "body"), DataRow(false, "exit"), DataRow(false, "gear"),
		DataRow(true, "body"), DataRow(true, "exit"), DataRow(true, "gear")]
	public void NativeItemFinalAuthorityCheckpoint_StaleContactCannotWoundGearOrBody(bool prosthetic, string change)
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		var itemAuthorityChecks = 0; var invalidated = false;
		var gear = f.InstallNativeGear(prosthetic);
		var source = f.OrderedSource();
		var authority = CommandExecutionAuthority.Prepare(source.Object, source.Object, "barrier item checkpoint", () =>
		{
			// Native GameItemHealth calls authority at entry and again after native destroyable scaling.
			// Count only policies running inside that exact native item method, not body/armour checks.
			if (new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(GameItem) && x.GetMethod()!.Name == nameof(GameItem.PassiveSufferDamage)))
				itemAuthorityChecks++;
			if (itemAuthorityChecks != 2 || invalidated) return true;
			invalidated = true;
			if (change == "body") f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Actor == f.Victim.Object));
			if (change == "exit") f.Reverse.SetupGet(x => x.Destination).Returns(Mock.Of<IRoom>());
			if (change == "gear") f.RemoveNativeGear();
			return true;
		});
		using var scope = CommandExecutionScope.EnterDispatch(authority, source.Object);
		var movement = f.Move(); movement.IntermediateStep(); movement.IntermediateStep();
		Assert.IsTrue(itemAuthorityChecks >= 2, "Native GameItemHealth reached its final authority checkpoint after native destroyable scaling.");
		Assert.IsTrue(invalidated); Assert.IsTrue(movement.Cancelled);
		Assert.IsFalse(gear.Wounds.Any()); Assert.IsFalse(f.Body.Wounds.Any());
		f.Victim.Verify(x => x.ExecuteMove(It.IsAny<IMovement>(), It.IsAny<IMoveSpeed>()), Times.Never);
	}
	[DataTestMethod, DataRow(false), DataRow(true)]
	public void NativeDestroyableContactGear_PositiveControlProducesNativeItemWound(bool prosthetic)
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		var gear = f.InstallNativeGear(prosthetic);
		var movement = f.Move(); movement.IntermediateStep();
		Assert.IsTrue(movement.Cancelled);
		Assert.IsInstanceOfType(gear.GetItemType<IDestroyable>(), typeof(DestroyableGameItemComponent));
		Assert.AreEqual(3.0, gear.Wounds.Sum(x => x.CurrentDamage), 0.000001);
		Assert.IsInstanceOfType(gear.Wounds.Single(), typeof(SimpleWound));
		Assert.AreEqual(prosthetic ? 0.0 : 1.5, f.Body.Wounds.Sum(x => x.CurrentDamage), 0.000001);
		f.Victim.Verify(x => x.ExecuteMove(It.IsAny<IMovement>(), It.IsAny<IMoveSpeed>()), Times.Never);
	}
	[DataTestMethod, DataRow(DamageType.Burning, 1), DataRow(DamageType.Burning, 7), DataRow(DamageType.Piercing, 1), DataRow(DamageType.Piercing, 7), DataRow(DamageType.Slashing, 1), DataRow(DamageType.Slashing, 7)]
	public void PaidBarrier_NativeMovementInflictsExactGradeThenBlocksOnce(DamageType type, int grade)
	{
		var f = new Fixture(type); var intent = f.Intent(grade);
		var result = f.F.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var child = f.ExitEffects.OfType<SpellDamagingExitBarrierEffect>().Single();
		Assert.AreEqual(3.0 * grade, child.DamageAmount); Assert.AreEqual(type, child.DamageType);
		Assert.AreEqual(TimeSpan.FromSeconds(60 * grade), f.ExitEffects.OfType<MagicSpellParent>().Single().ResolvedDuration);
		Assert.IsFalse(f.Victim.Object.CanCross(f.Direction.Object).Success); Assert.IsFalse(f.Body.Wounds.Any());
		var movement = f.Move(); movement.IntermediateStep();
		Assert.IsTrue(movement.Cancelled); Assert.AreEqual(3.0 * grade, f.Body.Wounds.Sum(x => x.CurrentDamage));
		Assert.IsInstanceOfType(f.Body.Wounds.Single(), typeof(SimpleOrganicWound));
		f.Victim.Verify(x => x.ExecuteMove(It.IsAny<IMovement>(), It.IsAny<IMoveSpeed>()), Times.Never);
		movement.IntermediateStep(); Assert.AreEqual(3.0 * grade, f.Body.Wounds.Sum(x => x.CurrentDamage));
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
		Assert.AreEqual(100.0 - 5 * grade, f.F.Balances[f.F.Resources[1]]);
		f.Move().IntermediateStep(); Assert.AreEqual(6.0 * grade, f.Body.Wounds.Sum(x => x.CurrentDamage));
	}
	[DataTestMethod, DataRow("door"), DataRow("guard"), DataRow("movement"), DataRow("legacy")]
	public void OrdinaryDeniedAttemptAndQueries_NeverInflictContactDamage(string reason)
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		if (reason == "door") f.DoorDenied = true;
		if (reason == "guard") f.GuardDenied = true;
		if (reason == "movement") f.Victim.Setup(x => x.CanMove(f.Direction.Object, CanMoveFlags.None)).Returns(new CanMoveResponse { Result = false, ErrorMessage = "Denied." });
		if (reason == "legacy") f.ExitEffects.Add(new SpellExitBarrierEffect(f.Boundary.Object, Mock.Of<IMagicSpellEffectParent>()));
		for (var i = 0; i < 3; ++i) Assert.IsFalse(f.Victim.Object.CanCross(f.Direction.Object).Success);
		var movement = f.Move(); movement.IntermediateStep(); movement.IntermediateStep();
		Assert.IsTrue(movement.Cancelled); Assert.IsFalse(f.Body.Wounds.Any());
	}
	[DataTestMethod, DataRow("body"), DataRow("world"), DataRow("opposite"), DataRow("remote")]
	public void InvalidBarrierTarget_RefusesBeforePayment(string cause)
	{
		var f = new Fixture();
		if (cause == "body") f.F.Body.SetupGet(x => x.Actor).Returns(Mock.Of<ICharacter>());
		if (cause == "world") f.Destination.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		if (cause == "opposite") f.Reverse.SetupGet(x => x.Exit).Returns(Mock.Of<IExit>());
		if (cause == "remote") f.Boundary.Setup(x => x.RoomExitFor(f.Origin.Object)).Returns((IRoomExit)null!);
		var result = f.F.Service.Cast(f.Intent(1)); Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.IsFalse(f.ExitEffects.Any());
	}
	[DataTestMethod, DataRow("BeforePayment"), DataRow("Committed")]
	public void PaidExitSelectionDrift_RefusesOrQuarantinesWithoutReplay(string stage)
	{
		var f = new Fixture(); var reached = false;
		f.F.Checkpoint = current => { if (current == stage) { reached = true; f.Reverse.SetupGet(x => x.Destination).Returns(Mock.Of<IRoom>()); } };
		var intent = f.Intent(1); var result = f.F.Service.Cast(intent);
		Assert.IsTrue(reached); Assert.AreEqual(stage == "BeforePayment" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(stage == "BeforePayment" ? 100.0 : 95.0, f.F.Balances[f.F.Resources[1]]); Assert.IsFalse(f.ExitEffects.Any());
		if (stage == "Committed") Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(intent).Status);
	}
	[DataTestMethod, DataRow("body"), DataRow("exit"), DataRow("door")]
	public void NativePreMutationCallback_CannotRedirectOrRepeatContact(string cause)
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		var reached = false;
		f.Part.SetupGet(x => x.NaturalArmourType).Returns(() =>
		{
			reached = true;
			if (cause == "body") f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Actor == f.Victim.Object));
			if (cause == "exit") f.Reverse.SetupGet(x => x.Destination).Returns(Mock.Of<IRoom>());
			if (cause == "door") f.DoorDenied = true;
			return null!;
		});
		var movement = f.Move(); movement.IntermediateStep(); movement.IntermediateStep();
		Assert.IsTrue(reached); Assert.IsTrue(movement.Cancelled); Assert.IsFalse(f.Body.Wounds.Any());
	}
	[TestMethod]
	public void NativeArmourCopies_PreserveGuardAndZeroHarmIsStillBlocked()
	{
		foreach (var pass in new[] { 0.0, 0.25 })
		{
			var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
			var definition = new XElement("Definition");
			foreach (var name in new[] { "DissipateExpressions", "AbsorbExpressions", "DissipateExpressionsPain", "AbsorbExpressionsPain", "DissipateExpressionsStun", "AbsorbExpressionsStun" })
				definition.Add(new XElement(name, new XElement("Expression", new XAttribute("damagetype", (int)DamageType.Burning),
					name == "AbsorbExpressions" ? $"damage*{pass.ToString(System.Globalization.CultureInfo.InvariantCulture)}" : name.EndsWith("Pain") || name.EndsWith("Stun") ? "0" : "damage")));
			Mock.Get(f.Body.Race).SetupGet(x => x.NaturalArmourType).Returns(new ArmourType(new MudSharp.Models.ArmourType
			{ Id = 1, Name = "Native barrier armour", Definition = definition.ToString() }, f.F.World.Object));
			var movement = f.Move(); movement.IntermediateStep(); Assert.IsTrue(movement.Cancelled);
			Assert.AreEqual(3 * pass, f.Body.Wounds.Sum(x => x.CurrentDamage), 0.000001);
			if (pass > 0)
			{
				Assert.ThrowsException<InvalidOperationException>(() => f.Body.PassiveSufferDamage(new Damage
				{ ActorOrigin = f.F.Actor.Object, Bodypart = f.Part.Object, DamageType = DamageType.Burning, DamageAmount = 3, MutationGuard = new RejectGuard() }).ToArray());
				Assert.AreEqual(3 * pass, f.Body.Wounds.Sum(x => x.CurrentDamage), 0.000001);
			}
		}
	}
	private sealed class RejectGuard : IDamageMutationGuard
	{ public void Validate(IHaveWounds body, IDamage damage) => throw new InvalidOperationException("Rejected by test mutation guard."); }
	[TestMethod]
	public void ReentrantMovementDuringNativeContact_CannotCrossOrHarmTwice()
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		var movement = f.Move(); var nested = false;
		f.Part.SetupGet(x => x.NaturalArmourType).Returns(() =>
		{
			if (!nested) { nested = true; movement.IntermediateStep(); }
			return null!;
		});
		movement.IntermediateStep(); Assert.IsTrue(nested); Assert.IsTrue(movement.Cancelled); Assert.IsFalse(f.Body.Wounds.Any());
		f.Victim.Verify(x => x.ExecuteMove(It.IsAny<IMovement>(), It.IsAny<IMoveSpeed>()), Times.Never);
	}
	[TestMethod]
	public void AuthorityPolicyAtNativeCheckpoint_CannotInjureAStaleVictimBody()
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		var source = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock }; source.As<ICharacterInstance>();
		var body = Mock.Of<IBody>(x => x.Actor == source.Object);
		source.SetupGet(x => x.Body).Returns(body); source.SetupGet(x => x.Gameworld).Returns(f.F.World.Object);
		source.SetupGet(x => x.Id).Returns(300); source.SetupGet(x => x.InstanceId).Returns(300); source.SetupGet(x => x.State).Returns(CharacterState.Awake);
		source.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 300 && x.Instances == new[] { (ICharacterInstance)source.Object }));
		source.SetupGet(x => x.Location).Returns(f.Origin.Object); f.Origin.SetupGet(x => x.Characters).Returns(new[] { source.Object, f.Victim.Object });
		var roots = new All<ICharacter>(); roots.Add(source.Object);
		f.F.World.SetupGet(x => x.Actors).Returns(roots); f.F.World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>()); f.F.World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
		var checkpoint = false; var invalidated = false;
		f.Part.SetupGet(x => x.NaturalArmourType).Returns(() => { checkpoint = true; return null!; });
		var authority = CommandExecutionAuthority.Prepare(source.Object, source.Object, "barrier crossing", () =>
		{
			if (checkpoint) { invalidated = true; f.Victim.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Actor == f.Victim.Object)); }
			return true;
		});
		using var scope = CommandExecutionScope.EnterDispatch(authority, source.Object);
		var movement = f.Move(); movement.IntermediateStep(); movement.IntermediateStep();
		Assert.IsTrue(invalidated); Assert.IsTrue(movement.Cancelled); Assert.IsFalse(f.Body.Wounds.Any());
	}
	[DataTestMethod, DataRow(false), DataRow(true)]
	public void NativePlanarBypassOrDifferentLayer_HasNoPhysicalContact(bool differentLayer)
	{
		var f = new Fixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		if (differentLayer) f.Victim.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		else
		{
			var plane = new PlanarPresenceDefinition([1], [1], [1], Enum.GetValues<PlanarInteractionKind>().ToDictionary(x => x, _ => (IEnumerable<long>)new long[] { 1 }),
				true, true, true, true, false, false, false, "");
			Mock.Get(f.Body.Prototype).SetupGet(x => x.BasePlanarPresence).Returns(plane);
		}
		Assert.IsTrue(f.Victim.Object.CanCross(f.Direction.Object).Success);
		f.Move().IntermediateStep(); Assert.IsFalse(f.Body.Wounds.Any());
		f.Victim.Verify(x => x.ExecuteMove(It.IsAny<IMovement>(), It.IsAny<IMoveSpeed>()), Times.Once);
	}
	[TestMethod]
	public void ParentColdRoundTrip_DeadlineIsNotResetAndOrphansDoNotBlock()
	{
		var clock = new Clock(); using var time = RuntimeClock.Push(clock); var f = new Fixture();
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.F.Service.Cast(f.Intent(1)).Status);
		var parent = f.ExitEffects.OfType<MagicSpellParent>().Single(); var child = f.ExitEffects.OfType<SpellDamagingExitBarrierEffect>().Single();
		var deadline = child.ExpiryUtc; clock.Now += TimeSpan.FromSeconds(40);
		var xml = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromSeconds(20) }); f.ExitEffects.Clear();
		SpellDamagingExitBarrierEffect.InitialiseEffectType();
		var restored = (MagicSpellParent)typeof(MagicSpellParent).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
			null, [typeof(XElement), typeof(IPerceivable)], null)!.Invoke([xml, f.Boundary.Object]);
		f.ExitEffects.Add(restored); var loaded = f.ExitEffects.OfType<SpellDamagingExitBarrierEffect>().Single();
		Assert.AreEqual(deadline, loaded.ExpiryUtc); Assert.IsTrue(loaded.IsActive);
		f.ExitEffects.Remove(restored); Assert.IsFalse(loaded.IsActive); Assert.IsTrue(f.Victim.Object.CanCross(f.Direction.Object).Success);
		f.ExitEffects.Add(restored); clock.Now += TimeSpan.FromSeconds(21);
		Assert.IsFalse(loaded.IsActive); Assert.IsTrue(f.Victim.Object.CanCross(f.Direction.Object).Success); Assert.IsFalse(f.Body.Wounds.Any());
	}
	[TestMethod]
	public void ThreeStockDefinitions_DeclareSameBoundedNativeContract()
	{
		var contents = new[] { ArmageddonBarrierSpellContent.FlameBarrier(), ArmageddonBarrierSpellContent.ThornBarrier(), ArmageddonBarrierSpellContent.BladeBarrier() };
		foreach (var content in contents)
		{
			Assert.AreEqual("60*grade", content.DurationFormula); Assert.AreEqual(10.0, content.MinimumEnergy);
			var xml = content.BuildDefinition(1, 2, 0); var effect = xml.Element("Effects")!.Element("Effect")!;
			Assert.AreEqual("exit", (string?)xml.Element("Trigger")!.Attribute("type")); Assert.AreEqual("damagingexitbarrier", (string?)effect.Attribute("type"));
			Assert.AreEqual(3.0, (double)effect.Element("DamagePerGrade")!); Assert.IsFalse(xml.Element("Plan")!.Elements().Any());
			StringAssert.Contains(content.Description, "authored native damage choice");
		}
	}
	private sealed class Clock : TimeProvider
	{ public DateTimeOffset Now = new(2026, 10, 13, 0, 0, 0, TimeSpan.Zero); public override DateTimeOffset GetUtcNow() => Now; }
	private sealed class Fixture
	{
		internal MagicCastingFixture F { get; } = new();
		internal Mock<ICharacter> Victim { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<IExternalBodypart> Part { get; } = new();
		internal Mock<IExit> Boundary { get; } = new();
		internal Mock<IRoomExit> Direction { get; } = new();
		internal Mock<IRoomExit> Reverse { get; } = new();
		internal Mock<IRoom> Destination { get; } = new();
		internal Mock<IRoom> Origin { get; }
		internal bool DoorDenied;
		internal bool GuardDenied;
		internal List<IEffect> ExitEffects { get; } = [];
		internal NativeBody Body { get; }
		internal SimpleOrganicWound SourceWound { get; }
		internal MagicSpell Spell { get; }
		internal Fixture(DamageType type = DamageType.Burning)
		{
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object);
			Origin = Mock.Get(F.Actor.Object.Location);
			Origin.SetupGet(x => x.Id).Returns(10); Origin.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			Destination.SetupGet(x => x.Id).Returns(11); Destination.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			Origin.SetupGet(x => x.Characters).Returns(new[] { F.Actor.Object, Victim.Object });
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
			Boundary.SetupGet(x => x.Id).Returns(20); Boundary.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			Boundary.SetupGet(x => x.GetObject).Returns(Boundary.Object); Boundary.SetupGet(x => x.Rooms).Returns(new[] { Origin.Object, Destination.Object });
			Boundary.SetupGet(x => x.Effects).Returns(ExitEffects);
			Boundary.Setup(x => x.RoomExitFor(Origin.Object)).Returns(Direction.Object); Boundary.Setup(x => x.RoomExitFor(Destination.Object)).Returns(Reverse.Object);
			Direction.SetupGet(x => x.Exit).Returns(Boundary.Object); Direction.SetupGet(x => x.Origin).Returns(Origin.Object); Direction.SetupGet(x => x.Destination).Returns(Destination.Object); Direction.SetupGet(x => x.Opposite).Returns(Reverse.Object);
			Reverse.SetupGet(x => x.Exit).Returns(Boundary.Object); Reverse.SetupGet(x => x.Origin).Returns(Destination.Object); Reverse.SetupGet(x => x.Destination).Returns(Origin.Object); Reverse.SetupGet(x => x.Opposite).Returns(Direction.Object);
			Origin.Setup(x => x.GetExitKeyword("target", F.Actor.Object)).Returns(Direction.Object);
			Boundary.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(ExitEffects.Add);
			Boundary.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => ExitEffects.Add(effect));
			Victim.SetupProperty(x => x.Movement); Victim.SetupGet(x => x.RidingMount).Returns((ICharacter)null!);
			Victim.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel); Victim.SetupGet(x => x.QueuedMoveCommands).Returns(new Queue<string>());
			Victim.Setup(x => x.MoveSpeed(It.IsAny<IRoomExit>())).Returns(0);
			Victim.Setup(x => x.CanMove(Direction.Object, CanMoveFlags.None)).Returns(CanMoveResponse.True);
			Victim.Setup(x => x.CanCross(Direction.Object)).Returns(() =>
				(DoorDenied || GuardDenied || ExitEffects.OfType<IExitBarrierEffect>().Any(x => ExitBarrierTraversal.Blocks(x, Victim.Object, Direction.Object)))
				? (false, new MudSharp.PerceptionEngine.Outputs.EmoteOutput(new MudSharp.PerceptionEngine.Parsers.Emote("@ stop|stops.", Victim.Object, Victim.Object))) : (true, null!));
			F.World.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(1000);
			DamagingExitBarrierEffect.RegisterFactory(); F.Spells.Remove(F.Spell);
			Spell = F.NewSpell(1, "Contact barrier", $"<Effect type='damagingexitbarrier'><DamageType>{(int)type}</DamageType><DamagePerGrade>3</DamagePerGrade></Effect>");
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("trigger new exit")));
			Spell.EffectDurationExpression = new TraitExpression("60*grade", F.World.Object); F.Skills[1] = 100; F.Acquire(7);
		}
		internal MagicCastingIntent Intent(int grade) => new(F.Actor.Object, F.Earth.Id, 1, grade, false, "target", OriginId: Guid.NewGuid());
		internal Mock<ICharacter> OrderedSource()
		{
			var source = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock }; source.As<ICharacterInstance>();
			var body = Mock.Of<IBody>(x => x.Actor == source.Object);
			source.SetupGet(x => x.Body).Returns(body); source.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			source.SetupGet(x => x.Id).Returns(300); source.SetupGet(x => x.InstanceId).Returns(300); source.SetupGet(x => x.State).Returns(CharacterState.Awake);
			source.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 300 && x.Instances == new[] { (ICharacterInstance)source.Object }));
			source.SetupGet(x => x.Location).Returns(Origin.Object); Origin.SetupGet(x => x.Characters).Returns(new[] { source.Object, Victim.Object });
			var roots = new All<ICharacter>(); roots.Add(source.Object);
			F.World.SetupGet(x => x.Actors).Returns(roots); F.World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>()); F.World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
			return source;
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
			typeof(GameItem).GetProperty(nameof(GameItem.Location))!.SetValue(gear, Origin.Object);
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
		internal Movement Move()
		{
			var movement = new Movement(Victim.Object, null!, [], [], [Victim.Object], [], [], [], Direction.Object);
			Victim.Object.Movement = movement; return movement;
		}
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
