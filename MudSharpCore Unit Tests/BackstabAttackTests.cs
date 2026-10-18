#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Combat.Strategies;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Save;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Magic.Powers;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class BackstabAttackTests
{
	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ICharacter> Target { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IBody> TargetBody { get; } = new();
		public Mock<IBodypart> Part { get; } = new();
		public Mock<ICharacterCombatSettings> Settings { get; } = new();
		public Mock<IRace> Race { get; } = new();
		public Mock<IMeleeWeapon> Weapon { get; } = new();
		public Mock<IWeaponType> WeaponType { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public Mock<ICombat> Combat { get; } = new();
		public Mock<ICheck> Resistance { get; } = new();
		public Mock<IScheduler> Scheduler { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public List<IGameItem> Equipment { get; } = [];
		public BackstabAttack Attack { get; }
		public NaturalAttack Natural { get; }
		public Fixture(bool natural = false)
		{
			PositionState.SetupPositions();
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			Actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, Actor.Object));
			Target.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, Target.Object));
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			World.SetupGet(x => x.TraitExpressions).Returns(new All<ITraitExpression>());
			World.SetupGet(x => x.BodypartShapes).Returns(new All<IBodypartShape>());
			World.SetupGet(x => x.ManualCombatCommands).Returns(new All<IManualCombatCommand>());
			World.SetupGet(x => x.Scheduler).Returns(Scheduler.Object);
			World.Setup(x => x.GetCheck(CheckType.StaggeringBlowDefense)).Returns(Resistance.Object);
			var type = natural ? BuiltInCombatMoveType.BackstabAttackUnarmed : BuiltInCombatMoveType.BackstabAttack;
			Attack = (BackstabAttack)MudSharp.Combat.WeaponAttack.LoadWeaponAttack(new MudSharp.Models.WeaponAttack
			{
				Id = 100, Name = "Test Backstab", MoveType = (int)type, MaximumTargets = 50,
				RequiredPositionStateIds = PositionStanding.Instance.Id.ToString(), Weighting = 100,
				Intentions = (long)CombatMoveIntentions.Attack, StaminaCost = 1, BaseDelay = 1
			}, World.Object);
			Attack.Profile = new SimpleDamageProfile
			{
				DamageExpression = new TraitExpression("10", World.Object),
				PainExpression = new TraitExpression("2", World.Object),
				StunExpression = new TraitExpression("3", World.Object),
				BaseAngleOfIncidence = Math.PI / 2, DamageType = DamageType.Piercing,
				BaseAttackerDifficulty = Difficulty.Normal
			};
			Natural = new NaturalAttack { Attack = Attack, Bodypart = Part.Object, Quality = ItemQuality.Standard };
			var account = Mock.Of<IAccount>(x => x.Culture == CultureInfo.InvariantCulture);
			Actor.SetupGet(x => x.Account).Returns(account);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Target.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Target.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupProperty(x => x.Combat, Combat.Object);
			Target.SetupProperty(x => x.Combat, Combat.Object);
			Actor.SetupProperty(x => x.CombatTarget, Target.Object);
			Target.SetupProperty(x => x.CombatTarget, Actor.Object);
			Combat.SetupGet(x => x.Combatants).Returns([Actor.Object, Target.Object]);
			Actor.SetupGet(x => x.CombatSettings).Returns(Settings.Object);
			Target.SetupGet(x => x.CombatSettings).Returns(Settings.Object);
			Actor.SetupProperty(x => x.OffensiveAdvantage, 0.0);
			Actor.SetupProperty(x => x.DefensiveAdvantage, 0.0);
			Target.SetupProperty(x => x.OffensiveAdvantage, 0.0);
			Target.SetupProperty(x => x.DefensiveAdvantage, 0.0);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Target.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			Target.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			Actor.SetupGet(x => x.MeleeRange).Returns(true);
			Actor.Setup(x => x.GetFacingFor(Target.Object, It.IsAny<bool>())).Returns(Facing.Rear);
			Actor.Setup(x => x.CanSee(Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Target.Setup(x => x.CanSee(Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Actor.Setup(x => x.ColocatedWith(Target.Object)).Returns(true);
			Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Target.SetupGet(x => x.Body).Returns(TargetBody.Object);
			TargetBody.SetupGet(x => x.Bodyparts).Returns([Part.Object]);
			Actor.SetupGet(x => x.Race).Returns(Race.Object);
			Race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true, CanDefend = true, CanUseWeapons = true });
			Body.SetupGet(x => x.Bodyparts).Returns([Part.Object]);
			Body.SetupGet(x => x.WieldedItems).Returns(Equipment);
			Body.SetupGet(x => x.Implants).Returns([]);
			Body.SetupGet(x => x.Prosthetics).Returns([]);
			Body.Setup(x => x.WieldedHandCount(Item.Object)).Returns(1);
			Body.Setup(x => x.CanUseBodypart(Part.Object)).Returns(CanUseBodypartResult.CanUse);
			Body.Setup(x => x.HeldItemsFor(Part.Object)).Returns([]);
			Body.Setup(x => x.WieldedItemsFor(Part.Object)).Returns([]);
			Settings.SetupGet(x => x.MeleeAttackOrderPreferences).Returns([MeleeAttackOrderPreference.Weapon]);
			Settings.SetupGet(x => x.ClassificationsAllowed).Returns([WeaponClassification.Lethal]);
			Weapon.SetupGet(x => x.Classification).Returns(WeaponClassification.Lethal);
			Weapon.SetupGet(x => x.Parent).Returns(Item.Object);
			Weapon.SetupGet(x => x.WeaponType).Returns(WeaponType.Object);
			Item.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns(Weapon.Object);
			WeaponType.Setup(x => x.UsableAttacks(Actor.Object, Item.Object, Target.Object, It.IsAny<AttackHandednessOptions>(),
				It.IsAny<bool>(), It.IsAny<BuiltInCombatMoveType[]>())).Returns((IPerceiver a, IGameItem w, IPerceiver t,
				AttackHandednessOptions h, bool ignore, BuiltInCombatMoveType[] types) =>
				Attack.UsableAttack(a, w, t, h, ignore, types) ? [Attack] : []);
			Race.Setup(x => x.UsableNaturalWeaponAttacks(Actor.Object, Target.Object, It.IsAny<bool>(), It.IsAny<BuiltInCombatMoveType[]>()))
				.Returns((ICharacter a, IPerceiver t, bool ignore, BuiltInCombatMoveType[] types) =>
					natural && Natural.UsableAttack(a, t, ignore, types) ? [Natural] : []);
			Actor.SetupGet(x => x.Effects).Returns(Effects);
			Actor.Setup(x => x.EffectsOfType<UnseenCombatOpening>(It.IsAny<Predicate<UnseenCombatOpening>>()))
				.Returns((Predicate<UnseenCombatOpening>? p) => Effects.OfType<UnseenCombatOpening>().Where(x => p?.Invoke(x) != false));
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((e, _) => { Effects.Remove(e); e.RemovalEffect(); });
			if (!natural) Equipment.Add(Item.Object);
			typeof(CombatBase).GetProperty("PowerMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!
				.SetValue(null, new TraitExpression("1", World.Object));
		}
		public void Capture(bool unseen = true)
		{
			Actor.Object.Combat = null;
			using var capture = UnseenCombatOpening.Capture(Actor.Object, Target.Object, unseen);
			Actor.Object.Combat = Combat.Object;
		}
		public void SetResistance(Outcome outcome) => Resistance.Setup(x => x.Check(Target.Object, It.IsAny<Difficulty>(),
			Actor.Object, It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.StaggeringBlowDefense, outcome));
		public Mock<ICheck> ConfigureResolution(Outcome outcome)
		{
			var check = new Mock<ICheck>();
			check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
				It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
				It.IsAny<(string, object)[]>())).Returns(() => Enum.GetValues<Difficulty>().ToDictionary(x => x,
				x => CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponCheck, outcome)));
			check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(),
				It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponPenetrateCheck, Outcome.Pass));
			World.Setup(x => x.GetCheck(It.Is<CheckType>(x => x != CheckType.StaggeringBlowDefense))).Returns(check.Object);
			Actor.Setup(x => x.GetDifficultyForTool(Item.Object, It.IsAny<Difficulty>())).Returns(Difficulty.Normal);
			Target.SetupGet(x => x.Race).Returns(Race.Object);
			Body.Setup(x => x.WieldedHand(Item.Object)).Returns(Mock.Of<IWield>());
			var solid = Mock.Of<ISolid>(x => x.Density == 1.0);
			Item.SetupGet(x => x.Material).Returns(solid);
			Body.Setup(x => x.GetMaterial(Part.Object)).Returns(solid);
			TargetBody.Setup(x => x.GetMaterial(Part.Object)).Returns(solid);
			TargetBody.Setup(x => x.RandomBodyPartGeometry(It.IsAny<Orientation>(), It.IsAny<Alignment>(), It.IsAny<Facing>(),
				It.IsAny<bool>())).Returns(Part.Object);
			Part.Setup(x => x.FullDescription()).Returns("test bodypart");
			var messages = new Mock<ICombatMessageManager>();
			messages.Setup(x => x.GetMessageFor(It.IsAny<ICharacter>(), It.IsAny<IPerceiver>(), It.IsAny<IGameItem>(),
				It.IsAny<IWeaponAttack>(), It.IsAny<BuiltInCombatMoveType>(), It.IsAny<Outcome>(), It.IsAny<IBodypart>()))
				.Returns("@ attack|attacks $1");
			messages.Setup(x => x.GetFailMessageFor(It.IsAny<ICharacter>(), It.IsAny<IPerceiver>(), It.IsAny<IGameItem>(),
				It.IsAny<IWeaponAttack>(), It.IsAny<BuiltInCombatMoveType>(), It.IsAny<Outcome>(), It.IsAny<IBodypart>()))
				.Returns(" and @ fail|fails to evade $1");
			World.SetupGet(x => x.CombatMessageManager).Returns(messages.Object);
			return check;
		}
	}

	[TestMethod]
	public void Configuration_LoadFactoryAndCloneData_PreserveEverySettingAndForceSingleTarget()
	{
		var f = new Fixture();
		Assert.AreEqual(1, f.Attack.MaximumTargets);
		Assert.AreEqual(Difficulty.Normal, f.Attack.SecondaryDifficulty);
		foreach (var command in new[] { "resist hard", "reel 1.5", "attackeradvantage 2 3", "targetdisadvantage 4 5", "penetration 6", "armourbypass 25%" })
			Assert.IsTrue(f.Attack.BuildingCommand(f.Actor.Object, new StringStack(command)), command);
		var row = new MudSharp.Models.WeaponAttack();
		typeof(BackstabAttack).GetMethod("AddAttackSpecificCloneData", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(f.Attack, [row]);
		var copy = new Fixture(true).Attack;
		copy.LoadConfiguration(XElement.Parse(row.AdditionalInfo));
		Assert.AreEqual(f.Attack.SaveConfiguration().ToString(), copy.SaveConfiguration().ToString());
		Assert.AreEqual(1, row.MaximumTargets);
		Assert.IsTrue(copy.IsAttackType<IBackstabAttack>());
	}

	[DataTestMethod]
	[DataRow("reel -1")]
	[DataRow("reel NaN")]
	[DataRow("penetration Infinity")]
	[DataRow("attackeradvantage 1 -1")]
	[DataRow("targetdisadvantage 1 NaN")]
	[DataRow("armourbypass 101%")]
	[DataRow("armourbypass -1%")]
	[DataRow("resist nonsense")]
	[DataRow("multitargets 2")]
	public void Builder_InvalidSettingsRejectedWithoutMutation(string command)
	{
		var f = new Fixture();
		var before = f.Attack.SaveConfiguration().ToString();
		Assert.IsFalse(f.Attack.BuildingCommand(f.Actor.Object, new StringStack(command)));
		Assert.AreEqual(before, f.Attack.SaveConfiguration().ToString());
	}

	[DataTestMethod]
	[DataRow(Facing.Front, false)]
	[DataRow(Facing.LeftFlank, false)]
	[DataRow(Facing.RightFlank, false)]
	[DataRow(Facing.Rear, true)]
	public void Eligibility_UsesRearFacingWithoutConsumingIt(Facing facing, bool expected)
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(facing);
		Assert.AreEqual(expected, BackstabCombat.IsEligible(f.Actor.Object, f.Target.Object));
		f.Actor.Verify(x => x.GetFacingFor(f.Target.Object, true), Times.Never);
	}

	[TestMethod]
	public void Opening_CapturesBeforeRevealingAndCannotRefreshByHidingRetargetingOrMerging()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front);
		f.Capture();
		Assert.IsTrue(BackstabCombat.IsEligible(f.Actor.Object, f.Target.Object));
		f.Actor.Object.CombatTarget = Mock.Of<ICharacter>();
		f.Actor.Object.CombatTarget = f.Target.Object;
		UnseenCombatOpening.Consume(f.Actor.Object, f.Target.Object);
		f.Target.Setup(x => x.CanSee(f.Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		UnseenCombatOpening.Capture(f.Actor.Object, f.Target.Object, true);
		Assert.IsFalse(BackstabCombat.IsEligible(f.Actor.Object, f.Target.Object));
		var merged = Mock.Of<ICombat>();
		f.Actor.Object.Combat = merged;
		f.Target.Object.Combat = merged;
		Assert.IsFalse(BackstabCombat.IsEligible(f.Actor.Object, f.Target.Object));
		f.Target.Raise(x => x.OnLeaveCombat += null, f.Target.Object);
		Assert.AreEqual(0, f.Effects.Count);
	}

	[TestMethod]
	public void Opening_VisibleEngagementCannotBecomeUnseenOpening()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front);
		f.Capture(false);
		f.Target.Setup(x => x.CanSee(f.Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Assert.IsFalse(BackstabCombat.IsEligible(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void Opening_AbortedEntryDiscardsSnapshotBeforeLaterVisibleEngagement()
	{
		var f = new Fixture();
		f.Actor.Object.Combat = null;
		using (UnseenCombatOpening.Capture(f.Actor.Object, f.Target.Object, true)) { }
		Assert.AreEqual(0, f.Effects.Count);
		f.Capture(false);
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void RealBackstabResolution_ConsumesOpeningAndMakesOneBoostedPenetrationCheck(bool natural)
	{
		var f = new Fixture(natural);
		var check = f.ConfigureResolution(Outcome.MajorPass);
		f.Attack.LoadConfiguration(XElement.Parse("<Backstab><Penetration>7</Penetration><ArmourBypass>1</ArmourBypass></Backstab>"));
		f.Capture();
		f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front);
		IDamage? delivered = null;
		f.Target.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(d => delivered = d).Returns([]);
		var move = BackstabCombat.FindAttack(f.Actor.Object, f.Target.Object, out _)!.CreateMove(f.Actor.Object, f.Target.Object);
		var result = move.ResolveMove(new HelplessDefenseMove { Assailant = f.Target.Object });
		Assert.IsTrue(result.MoveWasSuccessful);
		Assert.IsNotNull(delivered);
		Assert.IsTrue(delivered!.BypassWornArmour);
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
		var rolls = check.Invocations.Where(x => x.Method.Name == "Check").ToList();
		Assert.AreEqual(1, rolls.Count);
		Assert.AreEqual(7.0, rolls[0].Arguments[4]);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ManualSelectionAndFactory_ProduceCorrectSingleTargetMove(bool natural)
	{
		var f = new Fixture(natural);
		f.Attack.MaximumTargets = 20;
		var choice = BackstabCombat.FindAttack(f.Actor.Object, f.Target.Object, out var error);
		Assert.IsNotNull(choice, error);
		var move = choice!.CreateMove(f.Actor.Object, f.Target.Object);
		Assert.IsInstanceOfType(move, natural ? typeof(BackstabAttackUnarmedMove) : typeof(BackstabAttackMove));
		Assert.AreEqual(1, move.CharacterTargets.Count());
		Assert.IsFalse(move is MultiTargetCombatMove);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void AutomaticSelection_ConsidersAuthoredBackstab(bool natural)
	{
		var f = new Fixture(natural);
		var method = typeof(StandardMeleeStrategy).GetMethod(natural ? "AttemptUseNaturalAttack" : "AttemptUseWeapon",
			BindingFlags.Instance | BindingFlags.NonPublic)!;
		var move = method.Invoke(CombatStrategyFactory.GetStrategy(CombatStrategyMode.StandardMelee), [f.Actor.Object]);
		Assert.IsInstanceOfType(move, natural ? typeof(BackstabAttackUnarmedMove) : typeof(BackstabAttackMove));
	}

	[DataTestMethod]
	[DataRow("rear", true)]
	[DataRow("unseen", true)]
	[DataRow("detected", false)]
	[DataRow("layer", false)]
	[DataRow("range", false)]
	[DataRow("stamina", false)]
	[DataRow("bodypart", false)]
	public void HunterOpening_SelectsOnlyEligibleAffordableNaturalBackstab(string scenario, bool expected)
	{
		var f = new Fixture(true);
		if (scenario is "unseen" or "detected")
		{
			f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front);
			if (scenario == "unseen") f.Capture();
		}
		if (scenario == "layer") f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		if (scenario == "range") f.Actor.SetupGet(x => x.MeleeRange).Returns(false);
		if (scenario == "stamina") f.Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(false);
		if (scenario == "bodypart") f.Body.SetupGet(x => x.Bodyparts).Returns([]);
		var move = CreatureAIBase.SelectHuntOpeningBackstab(f.Actor.Object, f.Target.Object);
		Assert.AreEqual(expected, move is BackstabAttackUnarmedMove);
		if (scenario == "unseen") Assert.IsTrue(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ManualBinding_UsesTheSharedBackstabFactory(bool natural)
	{
		var f = new Fixture(natural);
		var command = Mock.Of<IManualCombatCommand>(x => x.ActionKind == ManualCombatActionKind.WeaponAttack && x.WeaponAttack == f.Attack);
		Mock.Get(command).Setup(x => x.IsUsableBy(f.Actor.Object, f.Target.Object)).Returns(true);
		var result = ManualCombatCommandResolver.TryResolve(f.Actor.Object, command, f.Target.Object);
		Assert.IsTrue(result.Success, result.Error);
		Assert.IsInstanceOfType(result.Move, natural ? typeof(BackstabAttackUnarmedMove) : typeof(BackstabAttackMove));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void BackstabCommand_CanInitiateUnseenAndQueueSelectedAttack(bool natural)
	{
		var f = new Fixture(natural);
		f.Actor.Object.Combat = null;
		f.Actor.Object.CombatTarget = null;
		f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front);
		f.Target.Setup(x => x.CanSee(f.Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		f.Actor.Setup(x => x.TargetActor("victim", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Target.Object);
		f.Actor.Setup(x => x.CanEngage(f.Target.Object)).Returns(true);
		f.Actor.Setup(x => x.Engage(f.Target.Object, false, false)).Callback(() =>
		{
			using var capture = UnseenCombatOpening.Capture(f.Actor.Object, f.Target.Object, true);
			f.Actor.Object.Combat = f.Combat.Object;
			f.Actor.Object.CombatTarget = f.Target.Object;
			f.Target.Setup(x => x.CanSee(f.Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		}).Returns(true);
		MudSharp.Effects.Interfaces.ISelectedCombatAction? queued = null;
		f.Actor.Setup(x => x.TakeOrQueueCombatAction(It.IsAny<MudSharp.Effects.Interfaces.ISelectedCombatAction>()))
			.Callback<MudSharp.Effects.Interfaces.ISelectedCombatAction>(x => queued = x).Returns(true);
		typeof(CombatModule).GetMethod("Backstab", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [f.Actor.Object, "backstab victim"]);
		f.Actor.Verify(x => x.Engage(f.Target.Object, false, false), Times.Once);
		Assert.IsNotNull(queued);
		Assert.IsInstanceOfType(queued!.GetMove(f.Actor.Object), natural ? typeof(BackstabAttackUnarmedMove) : typeof(BackstabAttackMove));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OrdinaryAttacks_StillConsumeOpeningAndDoNotAcquireBackstabArmourBenefits(bool natural)
	{
		var f = new Fixture(natural);
		var check = f.ConfigureResolution(Outcome.MajorPass);
		f.Attack.MoveType = natural ? BuiltInCombatMoveType.NaturalWeaponAttack : BuiltInCombatMoveType.UseWeaponAttack;
		f.Attack.LoadConfiguration(XElement.Parse("<Backstab><Penetration>7</Penetration><ArmourBypass>1</ArmourBypass></Backstab>"));
		f.Capture();
		IDamage? delivered = null;
		f.Target.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(x => delivered = x).Returns([]);
		var move = natural ? CombatMoveFactory.CreateNaturalWeaponAttack(f.Actor.Object, f.Natural, f.Target.Object) :
			CombatMoveFactory.CreateWeaponAttack(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object);
		move.ResolveMove(new HelplessDefenseMove { Assailant = f.Target.Object });
		Assert.IsNotNull(delivered);
		Assert.IsFalse(delivered!.BypassWornArmour);
		Assert.AreEqual(0.0, check.Invocations.Single(x => x.Method.Name == "Check").Arguments[4]);
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}

	[DataTestMethod]
	[DataRow(false, Outcome.MajorFail)]
	[DataRow(true, Outcome.MajorFail)]
	[DataRow(false, Outcome.MinorPass)]
	[DataRow(true, Outcome.MinorPass)]
	public void FirstAttempt_MissOrSuccessfulDefenceConsumesUnseenOpening(bool natural, Outcome attackOutcome)
	{
		var f = new Fixture(natural);
		var check = f.ConfigureResolution(attackOutcome);
		check.Setup(x => x.CheckAgainstAllDifficulties(f.Target.Object, It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(() => Enum.GetValues<Difficulty>().ToDictionary(x => x,
				x => CheckOutcome.SimpleOutcome(CheckType.DodgeCheck, Outcome.MajorPass)));
		f.Capture();
		f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front);
		var move = BackstabCombat.FindAttack(f.Actor.Object, f.Target.Object, out _)!.CreateMove(f.Actor.Object, f.Target.Object);
		var result = move.ResolveMove(new DodgeMove { Assailant = f.Target.Object });
		Assert.IsFalse(result.MoveWasSuccessful);
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
		f.Target.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void ClinchAttack_ConsumesUnseenOpeningBeforeAttackCheck()
	{
		var f = new Fixture();
		var check = f.ConfigureResolution(Outcome.MajorPass);
		f.Capture();
		var consumedBeforeCheck = false;
		check.Setup(x => x.Check(f.Actor.Object, It.IsAny<Difficulty>(), f.WeaponType.Object.AttackTrait,
			f.Target.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Callback(() => consumedBeforeCheck = !UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponCheck, Outcome.MajorPass));
		new ClinchAttackMove(f.Actor.Object, f.Target.Object, f.Attack, f.Weapon.Object)
			.ResolveMove(new HelplessDefenseMove { Assailant = f.Target.Object });
		Assert.IsTrue(consumedBeforeCheck);
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void FirstAttempt_MissedMagicAttackConsumesUnseenOpening()
	{
		var f = new Fixture();
		f.ConfigureResolution(Outcome.MajorFail);
		f.Capture();
		var power = new Mock<IMagicAttackPower>();
		power.SetupGet(x => x.WeaponAttack).Returns(f.Attack);
		power.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.MagicPowerAttack);
		power.Setup(x => x.CanInvokePower(f.Actor.Object, f.Target.Object)).Returns(true);
		var move = new MagicPowerAttackMove(f.Actor.Object, f.Target.Object, power.Object);
		Assert.IsFalse(move.ResolveMove(new HelplessDefenseMove { Assailant = f.Target.Object }).MoveWasSuccessful);
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}

	[DataTestMethod]
	[DataRow(false, "facing")]
	[DataRow(false, "equipment")]
	[DataRow(false, "target")]
	[DataRow(false, "stamina")]
	[DataRow(true, "bodypart")]
	public void QueuedBackstab_RevalidatesWithoutSpendingStamina(bool natural, string lost)
	{
		var f = new Fixture(natural);
		var choice = BackstabCombat.FindAttack(f.Actor.Object, f.Target.Object, out _)!;
		var selected = SelectedCombatAction.GetEffectBackstab(f.Actor.Object, f.Target.Object, choice);
		var move = selected.GetMove(f.Actor.Object);
		switch (lost)
		{
			case "facing": f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front); break;
			case "equipment": f.Equipment.Clear(); break;
			case "target": f.Target.Object.Combat = null; break;
			case "stamina": f.Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(false); break;
			case "bodypart": f.Body.SetupGet(x => x.Bodyparts).Returns([]); break;
		}
		var result = move.ResolveMove(new HelplessDefenseMove { Assailant = f.Target.Object });
		Assert.AreSame(CombatMoveResult.Irrelevant, result);
		Assert.IsFalse(move.UsesStaminaWithResult(result));
		f.Actor.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void QueuedBackstab_MovedTargetCancelsBeforeConstructingContactMove(bool natural)
	{
		var f = new Fixture(natural);
		f.Capture();
		var choice = BackstabCombat.FindAttack(f.Actor.Object, f.Target.Object, out _)!;
		var selected = SelectedCombatAction.GetEffectBackstab(f.Actor.Object, f.Target.Object, choice);
		f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(false);
		Assert.IsNull(selected.GetMove(f.Actor.Object));
		Assert.IsTrue(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
		f.Actor.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(Outcome.MinorFail, 1)]
	[DataRow(Outcome.Fail, 2)]
	[DataRow(Outcome.MajorFail, 3)]
	[DataRow(Outcome.MinorPass, 0)]
	[DataRow(Outcome.Pass, 0)]
	[DataRow(Outcome.MajorPass, 0)]
	public void SecondaryEffects_ShareResistanceAndScaleByFailureDegrees(Outcome outcome, int degrees)
	{
		var f = new Fixture();
		f.Attack.LoadConfiguration(XElement.Parse("<Backstab><Reel>1.5</Reel><AttackerOffence>2</AttackerOffence><AttackerDefence>3</AttackerDefence><TargetOffence>4</TargetOffence><TargetDefence>5</TargetDefence></Backstab>"));
		f.SetResistance(outcome);
		var wound = Mock.Of<IWound>(x => x.Parent == f.Target.Object && x.CurrentPain == 1.0);
		var previousSpeed = CombatBase.CombatSpeedMultiplier;
		try
		{
			CombatBase.CombatSpeedMultiplier = 2.0;
			BackstabCombat.ApplyEffects(f.Actor.Object, f.Target.Object, f.Attack,
				new CombatMoveResult { MoveWasSuccessful = true, WoundsCaused = [wound] });
		}
		finally { CombatBase.CombatSpeedMultiplier = previousSpeed; }
		Assert.AreEqual(2.0 * degrees, f.Actor.Object.OffensiveAdvantage);
		Assert.AreEqual(3.0 * degrees, f.Actor.Object.DefensiveAdvantage);
		Assert.AreEqual(-4.0 * degrees, f.Target.Object.OffensiveAdvantage);
		Assert.AreEqual(-5.0 * degrees, f.Target.Object.DefensiveAdvantage);
		f.Scheduler.Verify(x => x.DelayScheduleType(f.Target.Object, ScheduleType.Combat, TimeSpan.FromSeconds(3.0 * degrees)),
			degrees > 0 ? Times.Once() : Times.Never());
		Assert.AreEqual(1, f.Resistance.Invocations.Count);
	}

	[DataTestMethod]
	[DataRow("armour")]
	[DataRow("self")]
	[DataRow("zero")]
	[DataRow("miss")]
	[DataRow("absorbed")]
	public void SecondaryEffects_RequireActualVictimInjury(string kind)
	{
		var f = new Fixture();
		f.Attack.LoadConfiguration(XElement.Parse("<Backstab><Reel>1</Reel></Backstab>"));
		var wound = new Mock<IWound>();
		wound.SetupGet(x => x.Parent).Returns(kind == "armour" ? f.Item.Object : kind == "self" ? f.Actor.Object : f.Target.Object);
		wound.SetupGet(x => x.CurrentDamage).Returns(kind == "zero" ? 0.0 : 10.0);
		BackstabCombat.ApplyEffects(f.Actor.Object, f.Target.Object, f.Attack, new CombatMoveResult
		{
			MoveWasSuccessful = kind != "miss", WoundsCaused = kind == "absorbed" ? [] : [wound.Object]
		});
		Assert.AreEqual(0, f.Resistance.Invocations.Count);
	}

	[DataTestMethod]
	[DataRow(0.0, false)]
	[DataRow(1.0, true)]
	public void ArmourModifiers_OnlyChangeVictimCopyAndPreserveCopies(double chance, bool expected)
	{
		var f = new Fixture();
		f.Attack.LoadConfiguration(new XElement("Backstab", new XElement("ArmourBypass", chance), new XElement("Penetration", 17)));
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(f.Actor.Object, Difficulty.Easy, f.Target.Object, f.Weapon.Object, 17,
			TraitUseType.Practical, It.IsAny<(string, object)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponPenetrateCheck, Outcome.MajorPass));
		f.World.Setup(x => x.GetCheck(CheckType.MeleeWeaponPenetrateCheck)).Returns(check.Object);
		var original = new Damage { DamageAmount = 10, PenetrationOutcome = Outcome.Fail };
		var move = new BackstabAttackMove(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object);
		var outcome = (Outcome)typeof(WeaponAttackMove).GetMethod("RollTargetPenetration", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(move, [f.Target.Object, f.Weapon.Object, CheckType.MeleeWeaponPenetrateCheck])!;
		var modified = BackstabCombat.ModifyDamage(f.Attack, new Damage(original) { PenetrationOutcome = outcome });
		Assert.AreEqual(expected, modified.BypassWornArmour);
		Assert.AreEqual(Outcome.MajorPass, modified.PenetrationOutcome);
		Assert.IsFalse(original.BypassWornArmour);
		Assert.AreEqual(Outcome.Fail, original.PenetrationOutcome);
		Assert.AreEqual(expected, new Damage(modified).BypassWornArmour);
		Assert.AreEqual(expected, new Damage(modified, 0.5).BypassWornArmour);
		Assert.AreEqual(1, check.Invocations.Count);
	}

	[TestMethod]
	public void CommittedRangedShotConsumesOpeningButRefusedShotDoesNot()
	{
		var f = new Fixture();
		f.Capture();
		using (HostileAttackAdmission.BeginAttempt(f.Actor.Object, f.Target.Object)) HostileAttackAdmission.RecordShotAdmissionRefused();
		Assert.IsTrue(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
		using (HostileAttackAdmission.BeginAttempt(f.Actor.Object, f.Target.Object)) HostileAttackAdmission.RecordShotCommitted();
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}

	[DataTestMethod]
	[DataRow(true, true, false)]
	[DataRow(false, true, true)]
	[DataRow(true, false, true)]
	public void NativeBow_OnlyCommittedLoadedShotConsumesOpening(bool loaded, bool accessible, bool expectedAvailable)
	{
		var f = new Fixture();
		f.Capture();
		var bow = TestObjectFactory.CreateUninitialized<BowGameItemComponent>();
		typeof(BowGameItemComponent).GetProperty("Parent")!.SetValue(bow, f.Item.Object);
		var prototype = TestObjectFactory.CreateUninitialized<BowGameItemComponentProto>();
		typeof(BowGameItemComponent).GetField("_prototype", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(bow, prototype);
		typeof(BowGameItemComponentProto).GetField("_rangedWeaponType", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(prototype, Mock.Of<IRangedWeaponType>());
		typeof(BowGameItemComponentProto).GetField("<ConditionMaintenance>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(prototype, new ConditionMaintenanceProfile("0"));
		var hand = Mock.Of<IGrab>();
		f.Body.SetupGet(x => x.HoldLocs).Returns([hand]);
		f.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns((accessible, "blocked for test"));
		var room = Mock.Of<IRoom>();
		f.Actor.SetupGet(x => x.Location).Returns(room);
		f.Target.SetupGet(x => x.Location).Returns(room);
		var projectile = new Mock<IGameItem>();
		projectile.SetupProperty(x => x.ContainedIn, f.Item.Object);
		var ammo = Mock.Of<IAmmo>(x => x.Parent == projectile.Object);
		bow.LoadedAmmo = loaded ? ammo : null;
		bow.IsReadied = loaded;
		bow.Fire(f.Actor.Object, f.Target.Object, Outcome.Fail, Outcome.Fail,
			new OpposedOutcome(Outcome.Fail, Outcome.Pass), f.Part.Object, null, f.Target.Object);
		Assert.AreEqual(expectedAvailable, UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}
}
