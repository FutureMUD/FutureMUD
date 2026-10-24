#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DirectionSenseMovementTests
{
	[TestMethod]
	[DataRow("north", "")]
	[DataRow("north north", "north")]
	public void CharacterMove_RawCompassResolvedByFirstParser_RunsOrientationOnceAndMakesNoProgress(string command, string target)
	{
		var f = new Fixture(Outcome.Fail, Outcome.Pass);
		var actor = f.RealCharacter();
		f.Origin.Setup(x => x.GetExit("north", target, actor)).Returns(f.Intended.Object);
		Assert.IsTrue(actor.CanMove(f.Intended.Object, CanMoveFlags.None).Result, "Fixture must admit the requested move.");

		Assert.IsFalse(actor.Move(command));

		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
		Assert.IsNull(actor.Movement);
		f.Origin.Verify(x => x.RegisterMovement(It.IsAny<IMovement>()), Times.Never);
		f.Output.Verify(x => x.Send(DirectionSenseMovement.NoProgressMessage, true, false), Times.Once);
	}

	[TestMethod]
	[DataRow(Outcome.Pass)]
	[DataRow(Outcome.Fail)]
	public void CharacterMove_GuardChallenge_StillChecksOrientationBeforeAnyGuardRoll(Outcome recovery)
	{
		var f = new Fixture(Outcome.Fail, recovery);
		var actor = f.RealCharacter();
		typeof(MudSharp.Character.Character).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, CharacterState.Awake);
		f.World.Setup(x => x.GetStaticBool(HostileGuardChallenge.EnabledSetting)).Returns(true);
		f.Origin.Setup(x => x.GetExit("north", "", actor)).Returns(f.Intended.Object);
		f.Origin.Setup(x => x.ExitsFor(actor, false)).Returns([f.Intended.Object]);
		var guard = new Mock<ICharacter>();
		guard.SetupGet(x => x.Location).Returns(f.Origin.Object);
		guard.SetupGet(x => x.State).Returns(CharacterState.Awake);
		guard.Setup(x => x.ColocatedWith(actor)).Returns(true);
		guard.Setup(x => x.CanSee(actor, PerceiveIgnoreFlags.None)).Returns(true);
		var effect = new Mock<IGuardExitEffect>();
		effect.SetupGet(x => x.Exit).Returns(f.Intended.Object);
		effect.Setup(x => x.Applies()).Returns(true);
		guard.Setup(x => x.EffectsOfType<IGuardExitEffect>(It.IsAny<Predicate<IGuardExitEffect>>())).Returns([effect.Object]);
		f.Origin.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns([guard.Object]);
		Assert.IsFalse(actor.CanCross(f.Intended.Object).Success, "The actual guard must block ordinary crossing.");

		Assert.IsFalse(actor.Move("north !!"));

		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
		Assert.IsNull(actor.Movement);
		f.World.Verify(x => x.GetCheck(CheckType.GuardingExitChallengeCheck), Times.Never);
		f.Origin.Verify(x => x.RegisterMovement(It.IsAny<IMovement>()), Times.Never);
	}

	[TestMethod]
	public void CharacterMove_ChallengeWithoutGuard_DoesNotRollOrientation()
	{
		var f = new Fixture();
		var actor = f.RealCharacter();
		f.World.Setup(x => x.GetStaticBool(HostileGuardChallenge.EnabledSetting)).Returns(true);
		f.Origin.Setup(x => x.GetExit("north", "", actor)).Returns(f.Intended.Object);
		Assert.IsFalse(actor.Move("north !!"));
		Assert.AreEqual(0, f.InitialRolls);
		Assert.AreEqual(0, f.RecoveryRolls);
		f.Output.Verify(x => x.Send("There is no eligible guard blocking that action to challenge.", true, false), Times.Once);
	}

	[TestMethod]
	public void CharacterMove_InvalidCompass_DoesNotRollOrientation()
	{
		var f = new Fixture();
		var actor = f.RealCharacter();
		Assert.IsFalse(actor.Move("north"));
		Assert.AreEqual(0, f.InitialRolls);
		Assert.AreEqual(0, f.RecoveryRolls);
	}

	[TestMethod]
	public void CharacterMove_NonPrimaryRider_UsesOrdinaryRefusalWithoutOrientation()
	{
		var f = new Fixture();
		var actor = f.RealCharacter();
		var mount = new Mock<ICharacter>();
		actor.RidingMount = mount.Object;
		f.Origin.Setup(x => x.GetExit("north", "", actor)).Returns(f.Intended.Object);

		Assert.IsFalse(actor.Move("north"));

		Assert.AreEqual(0, f.InitialRolls);
		Assert.AreEqual(0, f.RecoveryRolls);
		mount.Verify(x => x.RiderMove(f.Intended.Object, actor, It.IsAny<IEmote>(), false), Times.Once);
	}

	[TestMethod]
	public void CharacterMove_ForcedIntendedCommand_CannotForceRandomWaterEntry()
	{
		var f = new Fixture();
		var actor = f.RealCharacter();
		f.Body.SetupGet(x => x.CurrentSpeeds).Returns(new Dictionary<IPositionState, IMoveSpeed>
		{
			[PositionStanding.Instance] = Mock.Of<IMoveSpeed>(),
			[PositionSwimming.Instance] = Mock.Of<IMoveSpeed>()
		});
		f.Wrong.Setup(x => x.MovementTransition(actor)).Returns((RoomMovementTransition.SwimOnly, RoomLayer.GroundLevel));
		f.Origin.Setup(x => x.GetExit("north", "", actor)).Returns(f.Intended.Object);
		f.Origin.Setup(x => x.ExitsFor(actor, false)).Returns([f.Wrong.Object]);
		Assert.IsTrue(actor.CanMove(f.Wrong.Object, CanMoveFlags.IgnoreSafeMovement).Result);
		Assert.IsFalse(actor.CanMove(f.Wrong.Object, CanMoveFlags.None).Result);

		Assert.IsFalse(actor.Move("north !"));

		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
		Assert.IsNull(actor.Movement);
		f.Output.Verify(x => x.Send(It.Is<string>(s => s.Contains("swim")), true, false), Times.Never);
	}

	[TestMethod]
	public void Resolve_FirstPass_UsesIntendedExitAndOnlyOneLearningCheck()
	{
		var f = new Fixture(Outcome.Pass, Outcome.Pass);
		Assert.AreSame(f.Intended.Object, DirectionSenseMovement.Resolve(f.Actor.Object, f.Intended.Object,
			[f.Wrong.Object], _ => throw new AssertFailedException("No alternate is needed.")));
		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(0, f.RecoveryRolls);
	}

	[TestMethod]
	public void Resolve_RecoveryPass_MakesNoProgressAndNeverInspectsAlternatives()
	{
		var f = new Fixture(Outcome.Fail, Outcome.Pass);
		Assert.IsNull(DirectionSenseMovement.Resolve(f.Actor.Object, f.Intended.Object,
			[f.Wrong.Object], _ => throw new AssertFailedException("Recovery must stop the command.")));
		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
		f.Actor.Verify(x => x.Move(It.IsAny<IRoomExit>(), It.IsAny<IEmote>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void Resolve_TwoFailures_ChoosesOnlySafeVisibleUsableWrongCompassExit()
	{
		var f = new Fixture();
		var hidden = f.Exit(CardinalDirection.South);
		f.Body.Setup(x => x.CanSee(f.Origin.Object, hidden.Object, PerceiveIgnoreFlags.None)).Returns(false);
		var falling = f.Exit(CardinalDirection.West);
		falling.SetupGet(x => x.IsFallExit).Returns(true);
		var transitionFall = f.Exit(CardinalDirection.NorthEast);
		transitionFall.Setup(x => x.MovementTransition(f.Actor.Object)).Returns((RoomMovementTransition.FallExit, RoomLayer.GroundLevel));
		var unusable = f.Exit(CardinalDirection.SouthWest);
		var vertical = f.Exit(CardinalDirection.Up);
		var seen = new List<IRoomExit>();

		var selected = DirectionSenseMovement.Resolve(f.Actor.Object, f.Intended.Object,
			[f.Intended.Object, hidden.Object, falling.Object, transitionFall.Object, unusable.Object, vertical.Object, f.Wrong.Object],
			x => { seen.Add(x); return x != unusable.Object; });

		Assert.AreSame(f.Wrong.Object, selected);
		CollectionAssert.AreEquivalent(new[] { unusable.Object, f.Wrong.Object }, seen);
		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
	}

	[TestMethod]
	public void Resolve_TwoFailuresWithoutAlternative_MakesNoProgress()
	{
		var f = new Fixture();
		Assert.IsNull(DirectionSenseMovement.Resolve(f.Actor.Object, f.Intended.Object,
			[f.Intended.Object, f.Wrong.Object], _ => false));
		Assert.AreEqual(1, f.InitialRolls);
		Assert.AreEqual(1, f.RecoveryRolls);
	}

	[TestMethod]
	public void Resolve_InvalidRecoveryPolicyOrFallback_DoesNotRollOrLearn()
	{
		var f = new Fixture();
		f.Recovery.SetupGet(x => x.ImproveTraits).Returns(true);
		Assert.IsNull(DirectionSenseMovement.Resolve(f.Actor.Object, f.Intended.Object, [f.Wrong.Object], _ => true));
		Assert.AreEqual(0, f.InitialRolls);
		f.Recovery.SetupGet(x => x.ImproveTraits).Returns(false);
		f.Initial.SetupGet(x => x.Type).Returns(CheckType.None);
		Assert.IsNull(DirectionSenseMovement.Resolve(f.Actor.Object, f.Intended.Object, [f.Wrong.Object], _ => true));
		Assert.AreEqual(0, f.InitialRolls);
	}

	[TestMethod]
	public void IsActive_UsesAuthoredCharacterAndExteriorLocationPredicateOnlyWhenEnabled()
	{
		var f = new Fixture();
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		prog.SetupGet(x => x.Id).Returns(7L);
		var progs = new All<IFutureProg> { prog.Object };
		prog.SetupGet(x => x.Id).Returns(7L);
		f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		f.World.Setup(x => x.GetStaticLong("DirectionSenseObscurationProg")).Returns(7);
		Assert.IsFalse(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		prog.Verify(x => x.ExecuteBool(It.IsAny<object[]>()), Times.Never);

		f.World.Setup(x => x.GetStaticBool("DirectionSenseEnabled")).Returns(true);
		Assert.IsTrue(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		prog.Verify(x => x.ExecuteBool(It.Is<object[]>(args => args.Length == 2 &&
			ReferenceEquals(args[0], f.Actor.Object) && ReferenceEquals(args[1], f.Origin.Object))), Times.Once);
	}

	[TestMethod]
	public void NoProgress_ClearsQueueSendsGenericOutputWithoutCreatingMovement()
	{
		var f = new Fixture();
		f.Queue.Enqueue("north");
		DirectionSenseMovement.NoProgress(f.Actor.Object);
		Assert.AreEqual(0, f.Queue.Count);
		f.Output.Verify(x => x.Send(DirectionSenseMovement.NoProgressMessage, true, false), Times.Once);
		f.Actor.Verify(x => x.Move(It.IsAny<IRoomExit>(), It.IsAny<IEmote>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void CanStartMovement_LeaveNoneBehind_RevalidatesEachMemberWithoutOutputOrCheckRolls()
	{
		var f = new Fixture();
		var companion = new Mock<ICharacter>();
		companion.SetupGet(x => x.Riders).Returns([]);
		companion.Setup(x => x.EffectsOfType<MudSharp.Effects.Concrete.Dragging>()).Returns([]);
		companion.Setup(x => x.CanMove(f.Wrong.Object, CanMoveFlags.None)).Returns(CanMoveResponse.True);
		f.Actor.SetupGet(x => x.Riders).Returns([]);
		f.Actor.Setup(x => x.EffectsOfType<MudSharp.Effects.Concrete.Dragging>()).Returns([]);
		f.Actor.Setup(x => x.CanMove(f.Wrong.Object, CanMoveFlags.None)).Returns(CanMoveResponse.True);
		f.Actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
		var party = new Mock<IParty>();
		party.SetupGet(x => x.Leader).Returns(f.Actor.Object);
		party.SetupGet(x => x.CharacterMembers).Returns([f.Actor.Object, companion.Object]);
		party.SetupGet(x => x.LeaveNoneBehind).Returns(true);
		f.Actor.SetupGet(x => x.Party).Returns(party.Object);

		Assert.IsTrue(Movement.CanStartMovement(f.Actor.Object, f.Wrong.Object));
		companion.Setup(x => x.CanMove(f.Wrong.Object, CanMoveFlags.None))
			.Returns(new CanMoveResponse { Result = false, ErrorMessage = "Hidden alternate identity" });
		Assert.IsFalse(Movement.CanStartMovement(f.Actor.Object, f.Wrong.Object));
		Assert.AreEqual(0, f.InitialRolls);
		Assert.AreEqual(0, f.RecoveryRolls);
		f.Output.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IBody> Body { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IOutputHandler> Output { get; } = new();
		public Mock<IRoom> Origin { get; } = new();
		public Mock<IRoomExit> Intended { get; }
		public Mock<IRoomExit> Wrong { get; }
		public Mock<ICheck> Initial { get; }
		public Mock<ICheck> Recovery { get; }
		public Queue<string> Queue { get; } = new();
		public int InitialRolls { get; private set; }
		public int RecoveryRolls { get; private set; }

		public Fixture(Outcome initial = Outcome.Fail, Outcome recovery = Outcome.Fail)
		{
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Location).Returns(Origin.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Actor.SetupGet(x => x.QueuedMoveCommands).Returns(Queue);
			Actor.SetupGet(x => x.InstanceId).Returns(10L);
			Actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(other => ReferenceEquals(other, Actor.Object));
			Body.Setup(x => x.CanSee(Origin.Object, It.IsAny<IRoomExit>(), PerceiveIgnoreFlags.None)).Returns(true);
			World.Setup(x => x.GetStaticConfiguration("DirectionSenseDifficulty")).Returns("Normal");
			World.Setup(x => x.GetStaticConfiguration("DirectionSenseRecoveryDifficulty")).Returns("Normal");
			Intended = Exit(CardinalDirection.North);
			Wrong = Exit(CardinalDirection.East);
			Initial = Check(CheckType.DirectionSenseCheck, true, initial, () => InitialRolls++);
			Recovery = Check(CheckType.DirectionSenseRecoveryCheck, false, recovery, () => RecoveryRolls++);
		}

		public Mock<IRoomExit> Exit(CardinalDirection direction)
		{
			var exit = new Mock<IRoomExit>();
			exit.SetupGet(x => x.Origin).Returns(Origin.Object);
			exit.SetupGet(x => x.Destination).Returns(Mock.Of<IRoom>());
			exit.SetupGet(x => x.OutboundDirection).Returns(direction);
			exit.Setup(x => x.MovementTransition(Actor.Object)).Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
			return exit;
		}

		private Mock<ICheck> Check(CheckType type, bool improve, Outcome outcome, Action rolled)
		{
			var check = new Mock<ICheck>();
			var expression = new Mock<ITraitExpression>();
			expression.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>
			{
				["directionsense"] = new() { Trait = Mock.Of<ITraitDefinition>(x => x.Id == 42), CanImprove = improve }
			});
			check.SetupGet(x => x.TargetNumberExpression).Returns(expression.Object);
			check.SetupGet(x => x.Type).Returns(type);
			check.SetupGet(x => x.ImproveTraits).Returns(improve);
			check.SetupGet(x => x.FailIfTraitMissing).Returns(FailIfTraitMissingType.FailIfAnyMissing);
			check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), Difficulty.Normal, (IPerceivable)null!, null, 0.0,
				TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()))
				.Callback(rolled).Returns(CheckOutcome.SimpleOutcome(type, outcome));
			World.Setup(x => x.GetCheck(type)).Returns(check.Object);
			return check;
		}

		public TestCharacter RealCharacter()
		{
			World.SetupGet(x => x.Vehicles).Returns(new All<MudSharp.Vehicles.IVehicle>());
			Body.Setup(x => x.CurrentContextualSize(SizeContext.RoomExit)).Returns(SizeCategory.Normal);
			Body.SetupGet(x => x.CurrentSpeeds).Returns(new Dictionary<IPositionState, IMoveSpeed>
			{
				[PositionStanding.Instance] = Mock.Of<IMoveSpeed>()
			});
			Origin.Setup(x => x.Terrain(It.IsAny<IPerceiver>()).GravityModel).Returns(GravityModel.Normal);
			var actor = TestCharacter.Create(World.Object, Body.Object, Origin.Object, Output.Object);
			foreach (var exit in new[] { Intended, Wrong })
			{
				exit.SetupGet(x => x.Exit).Returns(Mock.Of<IExit>(x => x.MaximumSizeToEnter == SizeCategory.Normal &&
					x.MaximumSizeToEnterUpright == SizeCategory.Normal));
				exit.Setup(x => x.MovementTransition(actor)).Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
			}
			var prog = new Mock<IFutureProg>();
			prog.SetupGet(x => x.Id).Returns(7L);
			prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { prog.Object });
			World.Setup(x => x.GetStaticBool("DirectionSenseEnabled")).Returns(true);
			World.Setup(x => x.GetStaticLong("DirectionSenseObscurationProg")).Returns(7);
			return actor;
		}
	}

	private sealed class TestCharacter : MudSharp.Character.Character
	{
		private TestCharacter() : base(null!, null!, true) { }
		public override IPositionState PositionState { get; set; } = null!;
		public static TestCharacter Create(IFuturemud world, IBody body, IRoom room, IOutputHandler output)
		{
			var actor = TestObjectFactory.CreateUninitialized<TestCharacter>();
			typeof(MudSharp.Character.Character).GetField("_merits", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(actor, new List<MudSharp.RPG.Merits.IMerit>());
			typeof(LateKeywordedInitialisingItem).GetField("<Gameworld>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, world);
			typeof(PerceivedItem).GetField("<EffectHandler>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, Mock.Of<IEffectHandler>());
			typeof(MudSharp.Character.Character).GetField("_riders", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, new List<ICharacter>());
			actor.Body = body;
			actor.Location = room;
			actor.OutputHandler = output;
			actor.PositionState = PositionStanding.Instance;
			actor.QueuedMoveCommands = new Queue<string>();
			return actor;
		}
	}
}
