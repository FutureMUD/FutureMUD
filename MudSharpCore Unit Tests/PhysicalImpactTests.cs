#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class PhysicalImpactTests
{
	private sealed class Fixture : IDisposable
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ICharacter> Target { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IBody> TargetBody { get; } = new();
		public Mock<IRace> Race { get; } = new();
		public Mock<IRoom> Room { get; } = new();
		public Mock<ICombat> Combat { get; } = new();
		public Mock<IAuxiliaryCombatAction> Action { get; } = new();
		public Mock<ICheck> Check { get; } = new();
		public Mock<IScheduler> Scheduler { get; } = new();
		public ITraitDefinition Skill { get; } = Mock.Of<ISkillDefinition>(x => x.Id == 901 && x.OwnerScope == TraitOwnerScope.Character);
		public ITraitDefinition Guard { get; } = Mock.Of<ISkillDefinition>(x => x.Id == 902 && x.OwnerScope == TraitOwnerScope.Character);
		public ITraitDefinition Agility { get; } = Mock.Of<IAttributeDefinition>(x => x.Id == 903);
		public PhysicalImpactProfile Profile { get; }
		public List<IDamage> Damage { get; } = [];
		public List<IDamage> SelfDamage { get; } = [];
		public int ActorSeats { get; private set; }
		public int TargetSeats { get; private set; }
		private readonly PropertyInfo _grace = typeof(CombatBase).GetProperty("GraceMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!;
		private readonly object? _priorGrace;
		public Fixture(bool bash, Outcome attack = Outcome.Pass, Outcome counter = Outcome.MajorFail)
		{
			PositionState.SetupPositions();
			Actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, Actor.Object));
			Target.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, Target.Object));
			Actor.SetupGet(x => x.Id).Returns(101); Target.SetupGet(x => x.Id).Returns(201);
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			World.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition> { Skill, Guard, Agility });
			World.SetupGet(x => x.TraitExpressions).Returns(new All<ITraitExpression>());
			World.SetupGet(x => x.Scheduler).Returns(Scheduler.Object);
			World.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(Check.Object);
			_priorGrace = _grace.GetValue(null);
			_grace.SetValue(null, new TraitExpression("1", World.Object));
			Profile = new PhysicalImpactProfile(Skill, Guard, Agility, new TraitExpression("5", World.Object),
				new TraitExpression("7", World.Object), new TraitExpression(bash ? "3" : "0", World.Object), new TraitExpression(bash ? "4" : "0", World.Object),
				10, 10, OpposedOutcomeDegree.Major, bash, bash, bash, true, !bash, !bash, !bash, true, bash ? 2 : 0);
			Action.As<IPhysicalImpactAuxiliaryAction>().SetupGet(x => x.PhysicalImpact).Returns(Profile);
			Action.SetupGet(x => x.Id).Returns(100);
			Action.SetupGet(x => x.CheckTrait).Returns(Skill);
			Action.SetupGet(x => x.MaximumTargets).Returns(1);
			Action.SetupGet(x => x.MoveDifficulty).Returns(Difficulty.Normal);
			Action.SetupGet(x => x.RecoveryDifficultyFailure).Returns(Difficulty.Hard);
			Action.Setup(x => x.UsableMove(Actor.Object, Target.Object, false)).Returns(true);
			Race.SetupGet(x => x.AuxiliaryActions).Returns([Action.Object]);
			Race.Setup(x => x.UsableAuxiliaryMoves(Actor.Object, Target.Object, false)).Returns([Action.Object]);
			Combat.SetupGet(x => x.Combatants).Returns([Actor.Object, Target.Object]);
			Room.SetupGet(x => x.Characters).Returns([Actor.Object, Target.Object]);
			var part = Mock.Of<IBodypart>();
			var leg = Mock.Of<ILimb>(x => x.LimbType == LimbType.Leg);
			foreach (var (ch, body) in new[] { (Actor, Body), (Target, TargetBody) })
			{
				ch.SetupGet(x => x.Gameworld).Returns(World.Object);
				ch.SetupGet(x => x.Body).Returns(body.Object);
				ch.SetupGet(x => x.Race).Returns(Race.Object);
				ch.SetupGet(x => x.Location).Returns(Room.Object);
				ch.SetupGet(x => x.State).Returns(CharacterState.Awake);
				ch.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
				ch.SetupGet(x => x.Combat).Returns(Combat.Object);
				ch.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
				ch.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
				ch.Setup(x => x.CanMovePosition(PositionSitting.Instance, true)).Returns(true);
				ch.Setup(x => x.TraitValue(Agility, It.IsAny<TraitBonusContext>())).Returns(10);
				body.SetupGet(x => x.Actor).Returns(ch.Object);
				body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
				body.SetupGet(x => x.Bodyparts).Returns([part]);
				body.SetupGet(x => x.RandomBodypart).Returns(part);
				body.SetupGet(x => x.Limbs).Returns([leg]);
				body.Setup(x => x.CanUseLimb(leg)).Returns(CanUseLimbResult.CanUse);
			}
			Actor.SetupGet(x => x.CombatTarget).Returns(Target.Object);
			Target.SetupGet(x => x.CombatTarget).Returns(Actor.Object);
			Actor.SetupGet(x => x.MeleeRange).Returns(true);
			Actor.Setup(x => x.ColocatedWith(Target.Object)).Returns(true);
			Actor.Setup(x => x.CanSee(Target.Object)).Returns(true);
			Actor.Setup(x => x.HasTrait(Skill)).Returns(true);
			Actor.Setup(x => x.TraitValue(Skill, It.IsAny<TraitBonusContext>())).Returns(50);
			Actor.Setup(x => x.SetPosition(PositionSitting.Instance, PositionModifier.None, null, null)).Callback(() => ActorSeats++);
			Target.Setup(x => x.SetPosition(PositionSitting.Instance, PositionModifier.None, null, null)).Callback(() => TargetSeats++);
			TargetBody.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(Damage.Add).Returns([]);
			Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(SelfDamage.Add).Returns([]);
			Check.Setup(x => x.Check(Actor.Object, It.IsAny<Difficulty>(), Skill, Target.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(CheckOutcome.SimpleOutcome(CheckType.AuxiliaryMoveCheck, attack));
			Check.Setup(x => x.Check(Target.Object, It.IsAny<Difficulty>(), Skill, Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatMoveCheck, counter));
			var messages = new Mock<ICombatMessageManager>();
			messages.Setup(x => x.GetMessageFor(Actor.Object, Target.Object, Action.Object, It.IsAny<Outcome>())).Returns("@ strike|strikes $1.");
			messages.Setup(x => x.GetFailMessageFor(Actor.Object, Target.Object, Action.Object, It.IsAny<Outcome>())).Returns("@ miss|misses $1.");
			World.SetupGet(x => x.CombatMessageManager).Returns(messages.Object);
		}
		public AuxiliaryMove Move() => new(Actor.Object, Target.Object, Action.Object);
		public void Dispose() => _grace.SetValue(null, _priorGrace);
	}

	[TestMethod]
	public void ConfiguredImpact_MissingPrimarySkillCannotUseAuxiliaryFallback()
	{
		using var f = new Fixture(true);
		f.World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		f.World.Setup(x => x.GetStaticLong("DefaultAuxiliaryMoveTraitId")).Returns(f.Skill.Id);
		var row = new MudSharp.Models.CombatAction { Id = 100, Name = "Configured", TraitDefinitionId = 999,
			RequiredPositionStateIds = "1", AdditionalInfo = new XElement("Root", f.Profile.Save()).ToString() };
		Assert.ThrowsException<InvalidOperationException>(() => new AuxiliaryCombatAction(row, f.World.Object));
		row.TraitDefinitionId = f.Skill.Id;
		var action = new AuxiliaryCombatAction(row, f.World.Object);
		Assert.AreEqual(f.Profile.Save().ToString(), action.PhysicalImpact!.Save().ToString());
		var saved = new MudSharp.Models.CombatAction();
		typeof(AuxiliaryCombatAction).GetMethod("SaveMoveSpecificData", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(action, [saved]);
		Assert.AreEqual(f.Profile.Save().ToString(), XElement.Parse(saved.AdditionalInfo).Element("PhysicalImpact")!.ToString());
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void PreResolutionBodyReplacement_RejectsWithoutChargingEitherBody(bool actor)
	{
		using var f = new Fixture(true);
		var move = f.Move();
		if (actor) f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		else f.Target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		var result = move.ResolveMove(null!);
		Assert.AreSame(CombatMoveResult.Irrelevant, result);
		Assert.IsFalse(move.UsesStaminaWithResult(result));
		Assert.AreEqual(0, f.Check.Invocations.Count + f.Damage.Count + f.SelfDamage.Count);
	}

	[TestMethod]
	public void ImpactCost_RemainsBoundToOriginalBodyAndPreAttemptSkill()
	{
		using var f = new Fixture(true);
		var move = f.Move();
		var cost = move.StaminaCost;
		f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		f.Actor.Setup(x => x.TraitValue(f.Skill, It.IsAny<TraitBonusContext>())).Returns(95);
		Assert.AreSame(f.Body.Object, move.ImpactStaminaBody);
		Assert.AreEqual(cost, move.StaminaCost);
	}

	[TestMethod]
	public void Builder_InvalidPrimaryTraitAndMultipleTargetsDoNotPoisonSavedProfile()
	{
		using var f = new Fixture(true);
		f.World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var row = new MudSharp.Models.CombatAction { Id = 100, Name = "Configured", TraitDefinitionId = f.Skill.Id,
			RequiredPositionStateIds = "1", AdditionalInfo = new XElement("Root", f.Profile.Save()).ToString() };
		var action = new AuxiliaryCombatAction(row, f.World.Object);
		Assert.IsFalse(action.BuildingCommand(f.Actor.Object, new StringStack("checktrait 903")));
		Assert.IsFalse(action.BuildingCommand(f.Actor.Object, new StringStack("targets 2")));
		Assert.AreSame(f.Skill, action.CheckTrait);
		Assert.AreEqual(1, action.MaximumTargets);
	}

	[TestMethod]
	public void ConfiguredImpact_OpensThroughNativeEngagementAndRevalidatesLiveAction()
	{
		using var f = new Fixture(true);
		var command = new Mock<IManualCombatCommand>();
		command.SetupGet(x => x.ActionKind).Returns(ManualCombatActionKind.AuxiliaryAction);
		command.SetupGet(x => x.AuxiliaryAction).Returns(f.Action.Object);
		command.Setup(x => x.IsUsableBy(f.Actor.Object, f.Target.Object)).Returns(true);
		ICombat? current = null;
		f.Actor.SetupGet(x => x.Combat).Returns(() => current);
		f.Actor.Setup(x => x.CanEngage(f.Target.Object)).Returns(true);
		f.Actor.Setup(x => x.Engage(f.Target.Object, false, false)).Callback(() => current = f.Combat.Object).Returns(true);
		Assert.IsTrue(ManualCombatCommandResolver.TryResolve(f.Actor.Object, command.Object, f.Target.Object).Success);
		f.Actor.Verify(x => x.Engage(f.Target.Object, false, false), Times.Once);
		Assert.AreEqual(0, f.Damage.Count + f.ActorSeats + f.Check.Invocations.Count);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void SuccessfulImpact_DeliversIndependentDamageStunAndOnlyBashSeatsTarget(bool bash)
	{
		using var f = new Fixture(bash);
		var result = f.Move().ResolveMove(null!);
		Assert.IsTrue(result.MoveWasSuccessful);
		Assert.AreEqual(1, f.Damage.Count);
		Assert.AreEqual(5.0, f.Damage[0].DamageAmount);
		Assert.AreEqual(7.0, f.Damage[0].StunAmount);
		Assert.AreSame(f.TargetBody.Object, f.Damage[0].TargetBody);
		Assert.AreEqual(bash ? 1 : 0, f.TargetSeats);
		Assert.AreEqual(0, f.ActorSeats);
		Assert.IsFalse(result.DefenderResponseWasUsed);
		f.Scheduler.Verify(x => x.DelayScheduleType(f.Target.Object, ScheduleType.Combat, It.IsAny<TimeSpan>()), bash ? Times.Once() : Times.Never());
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void OrdinaryMiss_OnlyBashSeatsAttackerWithoutDamage(bool bash)
	{
		using var f = new Fixture(bash, Outcome.Fail);
		var result = f.Move().ResolveMove(null!);
		Assert.IsFalse(result.MoveWasSuccessful);
		Assert.AreEqual(bash ? 1 : 0, f.ActorSeats);
		Assert.AreEqual(0, f.TargetSeats);
		Assert.AreEqual(0, f.Damage.Count + f.SelfDamage.Count);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void StrongCounter_SeatsAttackerAndOnlyBashCounterDamages(bool bash)
	{
		using var f = new Fixture(bash, Outcome.MajorFail, Outcome.MajorPass);
		f.Target.Setup(x => x.HasTrait(f.Skill)).Returns(true);
		var result = f.Move().ResolveMove(null!);
		Assert.IsFalse(result.MoveWasSuccessful);
		Assert.AreEqual(1, f.ActorSeats);
		Assert.AreEqual(0, f.Damage.Count);
		Assert.AreEqual(bash ? 1 : 0, f.SelfDamage.Count);
		if (bash) { Assert.AreEqual(3.0, f.SelfDamage[0].DamageAmount); Assert.AreEqual(4.0, f.SelfDamage[0].StunAmount); }
	}

	[DataTestMethod]
	[DataRow("mounted")]
	[DataRow("tired")]
	[DataRow("down")]
	[DataRow("untrained")]
	public void InadmissibleBash_DoesNotRollOrCommit(string refusal)
	{
		using var f = new Fixture(true);
		if (refusal == "mounted") f.Actor.SetupGet(x => x.RidingMount).Returns(Mock.Of<ICharacter>());
		if (refusal == "tired") f.Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(false);
		if (refusal == "down") f.Target.SetupGet(x => x.PositionState).Returns(PositionSitting.Instance);
		if (refusal == "untrained") f.Actor.Setup(x => x.HasTrait(f.Skill)).Returns(false);
		var move = f.Move();
		var result = move.ResolveMove(null!);
		Assert.AreSame(CombatMoveResult.Irrelevant, result);
		Assert.IsFalse(move.UsesStaminaWithResult(result));
		Assert.AreEqual(0, f.Check.Invocations.Count);
	}

	[TestMethod]
	public void IncompatiblePhase_SeatsOnlyAttackerWithoutAttackCheckOrDamage()
	{
		using var f = new Fixture(true);
		f.TargetBody.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2));
		Assert.IsFalse(f.Move().ResolveMove(null!).MoveWasSuccessful);
		Assert.AreEqual(1, f.ActorSeats);
		Assert.AreEqual(0, f.TargetSeats);
		Assert.AreEqual(0, f.Damage.Count + f.Check.Invocations.Count);
	}

	[TestMethod]
	public void CounterCallbackChangingBody_PreventsContactAndPostureMutation()
	{
		using var f = new Fixture(true, Outcome.MajorFail, Outcome.MajorPass);
		f.Target.Setup(x => x.HasTrait(f.Skill)).Returns(true);
		f.Check.Setup(x => x.Check(f.Target.Object, It.IsAny<Difficulty>(), f.Skill, f.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Callback(() => f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>())).Returns(CheckOutcome.SimpleOutcome(CheckType.CombatMoveCheck, Outcome.MajorPass));
		Assert.IsFalse(f.Move().ResolveMove(null!).MoveWasSuccessful);
		Assert.AreEqual(0, f.ActorSeats + f.TargetSeats + f.Damage.Count + f.SelfDamage.Count);
	}

	[TestMethod]
	public void ProfileRoundTrip_PreservesEveryFieldAndRejectsMissingSkills()
	{
		using var f = new Fixture(true);
		var xml = f.Profile.Save();
		Assert.AreEqual(xml.ToString(), PhysicalImpactProfile.Load(xml, f.World.Object).Save().ToString());
		xml.SetAttributeValue("countertrait", 999);
		Assert.ThrowsException<FormatException>(() => PhysicalImpactProfile.Load(xml, f.World.Object));
	}
}
