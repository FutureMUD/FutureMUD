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
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.NPC.AI.Strategies;
using MudSharp.PerceptionEngine;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PathingAIDoorSmashScheduleTests
{
    private sealed class TestPathingAI : PathingAIBase
    {
        public TestPathingAI()
        {
        }

        public TestPathingAI(IFuturemud gameworld) : base(gameworld, "test", "test")
        {
        }

        public DateTime Clock { get; set; }
        public int SmashCount { get; private set; }
        public bool Enabled { get; set; } = true;

		public void SetDelayProg(IFutureProg? prog) => DoorSmashDelayProg = prog;
		public void EnableDoorSmashing() => SmashLockedDoors = true;
		public void EnableCloseDoorsBehind() => CloseDoorsBehind = true;
		public bool RunCheckSmash(ICharacter character) => CheckSmash(character);
		public bool RunFiveSecondTick(ICharacter character) => FiveSecondTick(character);
		public void RunFollowPathAction(ICharacter character, FollowingPath path) => FollowPathAction(character, path);
		public bool RunSuitability(ICharacter character, ICellExit exit) => GetSuitabilityFunction(character)(exit);
		public void RunCheckCloseDoor(ICharacter character, ICellExit exit) => CheckCloseDoor(character, exit);
        public string SavedDefinition => PrepareDefinitionForSave(SaveToXml());

        protected override DateTime UtcNow => Clock;
        protected override bool Smash(ICharacter ch, ICellExit exit)
        {
            SmashCount++;
            return true;
        }

        protected override bool IsPathingEnabled(ICharacter character) => Enabled;
        protected override (ICell? Target, IEnumerable<ICellExit>) GetPath(ICharacter ch) =>
            (null, Enumerable.Empty<ICellExit>());
        protected override string SaveToXml() => "<Definition><OpenDoors>false</OpenDoors></Definition>";
    }

    private sealed record SmashFixture(
		Mock<ICharacter> Character,
		Mock<ICell> Origin,
		Mock<IDoor> Door,
		Mock<IGameItem> DoorItem,
		Mock<IExit> Exit,
        Mock<ICellExit> CellExit,
        BreakDownDoor Focus,
		List<IEffect> Effects,
		List<INaturalAttack> NaturalAttacks);

    private static SmashFixture CreateFixture()
    {
        var origin = new Mock<ICell>();
        var door = new Mock<IDoor>();
        door.SetupGet(x => x.IsOpen).Returns(false);
		door.SetupGet(x => x.CanPlayersSmash).Returns(true);
		var doorItem = new Mock<IGameItem>();
		var destroyable = new Mock<IDestroyable>();
		doorItem.Setup(x => x.GetItemType<IDestroyable>()).Returns(destroyable.Object);
		door.SetupGet(x => x.Parent).Returns(doorItem.Object);
        var exit = new Mock<IExit>();
        exit.SetupGet(x => x.Door).Returns(door.Object);
        var cellExit = new Mock<ICellExit>();
        cellExit.SetupGet(x => x.Origin).Returns(origin.Object);
        cellExit.SetupGet(x => x.Exit).Returns(exit.Object);

        var character = new Mock<ICharacter>();
        character.SetupGet(x => x.Location).Returns(origin.Object);
		var naturalAttacks = new List<INaturalAttack> { new Mock<INaturalAttack>().Object };
		var race = new Mock<IRace>();
		race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanUseWeapons = false });
		race.Setup(x => x.UsableNaturalWeaponAttacks(character.Object, doorItem.Object, false,
			It.IsAny<BuiltInCombatMoveType[]>())).Returns(naturalAttacks);
		character.SetupGet(x => x.Race).Returns(race.Object);
		character.Setup(x => x.CanCross(cellExit.Object)).Returns((false, null!));
		character.Setup(x => x.CanMove(cellExit.Object, It.IsAny<CanMoveFlags>()))
		         .Returns(new CanMoveResponse { Result = false, ErrorMessage = "closed door" });
        var focus = new BreakDownDoor(character.Object, cellExit.Object);
        var effects = new List<IEffect> { focus };
        character.SetupGet(x => x.Effects).Returns(effects);
        character.Setup(x => x.AffectedBy<BreakDownDoor>()).Returns(() => effects.Contains(focus));
        character.Setup(x => x.EffectsOfType<BreakDownDoor>(It.IsAny<Predicate<BreakDownDoor>>()))
            .Returns((Predicate<BreakDownDoor>? predicate) =>
                effects.OfType<BreakDownDoor>().Where(x => predicate is null || predicate(x)));
        character.Setup(x => x.EffectsOfType<FollowingPath>(It.IsAny<Predicate<FollowingPath>>()))
            .Returns((Predicate<FollowingPath>? predicate) =>
                effects.OfType<FollowingPath>().Where(x => predicate is null || predicate(x)));
        character.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()))
            .Callback((Predicate<IEffect> predicate, bool _) => effects.RemoveAll(predicate));
		character.Setup(x => x.RemoveAllEffects<BreakDownDoor>(It.IsAny<Predicate<BreakDownDoor>>(), It.IsAny<bool>()))
		         .Callback((Predicate<BreakDownDoor>? predicate, bool _) =>
			         effects.RemoveAll(x => x is BreakDownDoor focusEffect &&
			                                (predicate is null || predicate(focusEffect))));
		character.Setup(x => x.RemoveAllEffects<FollowingPath>(It.IsAny<Predicate<FollowingPath>>(), It.IsAny<bool>()))
		         .Callback((Predicate<FollowingPath>? predicate, bool _) =>
			         effects.RemoveAll(x => x is FollowingPath pathEffect &&
			                                (predicate is null || predicate(pathEffect))));
		character.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
		         .Callback((IEffect effect, bool _) => effects.Remove(effect));
		return new SmashFixture(character, origin, door, doorItem, exit, cellExit, focus, effects, naturalAttacks);
    }

	private sealed class TestFollowingPath : FollowingPath
	{
		public TestFollowingPath(ICharacter owner, IEnumerable<ICellExit> exits) : base(owner, exits)
		{
		}

		public MovementStrategyResult RunTryMoveThroughExit(ICharacter character, ICellExit exit) =>
			TryMoveThroughExit(character, exit);
	}

	private static void Bind(SmashFixture fixture, TestPathingAI ai)
	{
		var path = fixture.Effects.OfType<FollowingPath>().FirstOrDefault() ??
		           new FollowingPath(fixture.Character.Object, Array.Empty<ICellExit>());
		path.PathingOwner = ai;
		fixture.Focus.PathingEpisode = path;
		if (!fixture.Effects.Contains(path))
		{
			fixture.Effects.Add(path);
		}
	}

    private static Mock<IFutureProg> DelayProg(long id, Func<ICharacter, ICellExit, decimal> delay)
    {
        var prog = new Mock<IFutureProg>();
        prog.SetupGet(x => x.Id).Returns(id);
        prog.SetupGet(x => x.Name).Returns($"Delay {id}");
        prog.Setup(x => x.ExecuteDecimal(It.IsAny<object[]>()))
            .Returns((object[] args) => delay((ICharacter)args[0], (ICellExit)args[1]));
        return prog;
    }

	private static void EnableDoorMovement(SmashFixture fixture)
	{
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.GravityModel).Returns(GravityModel.Normal);
		fixture.Origin.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain.Object);
		fixture.Character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		fixture.Character.Setup(x => x.CanMove(It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
		fixture.Character.Setup(x => x.CanMove(fixture.CellExit.Object, It.IsAny<CanMoveFlags>()))
		       .Returns(CanMoveResponse.True);
		fixture.Character.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
		       .Callback((IEffect effect) => fixture.Effects.Add(effect));
	}

	[TestMethod]
	public void NativeWeaponSmash_BindsSelectedWeaponToRealMove()
	{
		var character = new Mock<ICharacter>();
		var target = new Mock<IGameItem>();
		var weapon = new Mock<IMeleeWeapon>();
		var attack = new Mock<IWeaponAttack>();

		var move = PathingAIBase.CreateMeleeWeaponSmashMove(character.Object, target.Object, weapon.Object,
			attack.Object);

		Assert.AreSame(character.Object, move.Assailant);
		Assert.AreSame(target.Object, move.Target);
		Assert.AreSame(weapon.Object, move.Weapon);
		Assert.AreSame(attack.Object, move.Attack);
		Assert.IsNull(move.ParentItem);
	}

	[TestMethod]
	public void NativeWeaponSmash_RealConsumerUsesBoundWeaponAndCurrentFormulaEvaluation()
	{
		var character = new Mock<ICharacter>();
		var target = new Mock<IGameItem>();
		var weaponItem = new Mock<IGameItem>();
		weaponItem.SetupGet(x => x.Quality).Returns(ItemQuality.Standard);
		var weapon = new Mock<IMeleeWeapon>();
		weapon.SetupGet(x => x.Parent).Returns(weaponItem.Object);
		var weaponType = new Mock<IWeaponType>();
		var trait = new Mock<ITraitDefinition>();
		weaponType.SetupGet(x => x.AttackTrait).Returns(trait.Object);
		weapon.SetupGet(x => x.WeaponType).Returns(weaponType.Object);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.LegalAuthorities).Returns(new All<MudSharp.RPG.Law.ILegalAuthority>());
		character.SetupGet(x => x.Gameworld).Returns(world.Object);
		character.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(character.Object, Difficulty.Easy, trait.Object, target.Object, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponCheck, Outcome.Pass));
		world.Setup(x => x.GetCheck(CheckType.MeleeWeaponCheck)).Returns(check.Object);
		var expression = new MudSharp.Body.Traits.TraitExpression("10 + degree + quality", world.Object);
		var profile = new Mock<IDamageProfile>();
		profile.SetupGet(x => x.DamageExpression).Returns(expression);
		profile.SetupGet(x => x.BaseAngleOfIncidence).Returns(Math.PI / 2.0);
		var attack = new Mock<IWeaponAttack>();
		attack.SetupGet(x => x.Profile).Returns(profile.Object);
		var messages = new Mock<ICombatMessageManager>();
		messages.Setup(x => x.GetMessageFor(character.Object, target.Object, weaponItem.Object, attack.Object,
			BuiltInCombatMoveType.MeleeWeaponSmashItem, Outcome.Pass, null)).Returns("A measured strike.");
		world.SetupGet(x => x.CombatMessageManager).Returns(messages.Object);
		IDamage? targetDamage = null;
		IDamage? weaponDamage = null;
		target.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(damage => targetDamage = damage)
			.Returns(Array.Empty<IWound>());
		weaponItem.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(damage => weaponDamage = damage)
			.Returns(Array.Empty<IWound>());

		var move = PathingAIBase.CreateMeleeWeaponSmashMove(character.Object, target.Object, weapon.Object, attack.Object);
		var result = move.ResolveMove(null!);

		Assert.IsTrue(result.MoveWasSuccessful);
		Assert.IsNotNull(targetDamage);
		Assert.IsNotNull(weaponDamage);
		var expected = 10 + (int)new OpposedOutcome(Outcome.Pass, Outcome.NotTested).Degree + (int)ItemQuality.Standard;
		Assert.AreEqual(expected, targetDamage.DamageAmount, 0.000001);
		Assert.AreEqual(expected * 0.05, weaponDamage.DamageAmount, 0.000001);
		Assert.AreSame(weaponItem.Object, targetDamage.ToolOrigin);
		check.VerifyAll();
		messages.VerifyAll();
	}

	[TestMethod]
	public void RepeatedClosedDoorRetries_ReuseFocusDeadlineAndCallbackSchedule()
	{
		var fixture = CreateFixture();
		fixture.Effects.Remove(fixture.Focus);
		EnableDoorMovement(fixture);
		var now = new DateTime(2026, 8, 23, 2, 0, 0, DateTimeKind.Utc);
		var prog = DelayProg(77, (_, _) => 30_000M);
		var ai = new TestPathingAI { Clock = now };
		ai.SetDelayProg(prog.Object);
		var path = new TestFollowingPath(fixture.Character.Object, new[] { fixture.CellExit.Object })
		{
			PathingOwner = ai,
			SmashLockedDoors = true
		};
		var blocker = new BlockingDelayedAction(fixture.Character.Object, _ => { }, "waiting to move",
			"general", string.Empty);

		Assert.AreEqual(MovementStrategyResult.Waiting,
			path.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object));
		var focus = fixture.Effects.OfType<BreakDownDoor>().Single();
		fixture.Effects.Add(blocker);
		Assert.IsFalse(ai.RunCheckSmash(fixture.Character.Object));
		var initialDue = focus.NextSmashAttemptUtc;

		for (var i = 0; i < 3; i++)
		{
			Assert.AreEqual(MovementStrategyResult.Waiting,
				path.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object));
		}

		Assert.AreSame(focus, fixture.Effects.OfType<BreakDownDoor>().Single());
		Assert.AreEqual(now.AddSeconds(30), initialDue);
		Assert.AreEqual(initialDue, focus.NextSmashAttemptUtc);
		prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Once);

		fixture.Effects.Remove(blocker);
		ai.Clock = initialDue!.Value;
		Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));
		Assert.AreEqual(1, ai.SmashCount);
		Assert.AreSame(focus, fixture.Effects.OfType<BreakDownDoor>().Single());
		Assert.AreEqual(ai.Clock.AddSeconds(30), focus.NextSmashAttemptUtc);
		prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Exactly(2));
	}

	[TestMethod]
	public void DoorFocusUniqueness_IsScopedByEpisodeAndExit()
	{
		var fixture = CreateFixture();
		fixture.Effects.Remove(fixture.Focus);
		EnableDoorMovement(fixture);
		var otherExit = new Mock<ICellExit>();
		otherExit.SetupGet(x => x.Origin).Returns(fixture.Origin.Object);
		otherExit.SetupGet(x => x.Exit).Returns(fixture.CellExit.Object.Exit);
		fixture.Character.Setup(x => x.CanMove(otherExit.Object, It.IsAny<CanMoveFlags>()))
		       .Returns(CanMoveResponse.True);
		var first = new TestFollowingPath(fixture.Character.Object, new[] { fixture.CellExit.Object })
		{
			SmashLockedDoors = true
		};
		var second = new TestFollowingPath(fixture.Character.Object, new[] { fixture.CellExit.Object })
		{
			SmashLockedDoors = true
		};

		first.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object);
		first.RunTryMoveThroughExit(fixture.Character.Object, otherExit.Object);
		second.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object);
		first.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object);
		first.RunTryMoveThroughExit(fixture.Character.Object, otherExit.Object);
		second.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object);

		var foci = fixture.Effects.OfType<BreakDownDoor>().ToList();
		Assert.AreEqual(3, foci.Count);
		Assert.AreEqual(2, foci.Count(x => ReferenceEquals(x.PathingEpisode, first)));
		Assert.AreEqual(1, foci.Count(x => ReferenceEquals(x.PathingEpisode, second)));
		Assert.AreEqual(2, foci.Count(x => ReferenceEquals(x.Exit, fixture.CellExit.Object)));
		Assert.AreEqual(1, foci.Count(x => ReferenceEquals(x.Exit, otherExit.Object)));
	}

	[DataTestMethod]
	[DataRow(true, true, true, true)]
	[DataRow(false, true, true, false)]
	[DataRow(true, false, true, false)]
	[DataRow(true, true, false, false)]
	public void RouteSuitability_RequiresPermissionDestroyabilityAndUsableAttack(
		bool canPlayersSmash,
		bool destroyable,
		bool hasAttack,
		bool expected)
	{
		var fixture = CreateFixture();
		fixture.Door.SetupGet(x => x.CanPlayersSmash).Returns(canPlayersSmash);
		fixture.DoorItem.Setup(x => x.GetItemType<IDestroyable>())
		       .Returns(destroyable ? new Mock<IDestroyable>().Object : null!);
		if (!hasAttack)
		{
			fixture.NaturalAttacks.Clear();
		}

		var ai = new TestPathingAI();
		ai.EnableDoorSmashing();

		Assert.AreEqual(expected, ai.RunSuitability(fixture.Character.Object, fixture.CellExit.Object));
	}

	[TestMethod]
	public void RaceFilteredNaturalAttack_NormalOrInjuredFallbackRemainsFeasible()
	{
		var fixture = CreateFixture();
		var ai = new TestPathingAI();
		ai.EnableDoorSmashing();

		Assert.IsTrue(ai.RunSuitability(fixture.Character.Object, fixture.CellExit.Object));
		fixture.NaturalAttacks[0] = new Mock<INaturalAttack>().Object;
		Assert.IsTrue(ai.RunSuitability(fixture.Character.Object, fixture.CellExit.Object));
	}

	[TestMethod]
	public void NoUsableAttack_TerminatesOwnedEpisodeWithoutCallbackOrFocusChurn()
	{
		var fixture = CreateFixture();
		fixture.NaturalAttacks.Clear();
		var prog = DelayProg(77, (_, _) => 30_000M);
		var ai = new TestPathingAI();
		ai.SetDelayProg(prog.Object);
		Bind(fixture, ai);
		var path = fixture.Focus.PathingEpisode!;

		Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));

		Assert.IsFalse(fixture.Effects.Contains(path));
		Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
		Assert.AreEqual(0, ai.SmashCount);
		prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void OverdueFocus_BlockedRetriesPreserveDueUntilOneRealAttemptReschedules()
	{
		var fixture = CreateFixture();
		EnableDoorMovement(fixture);
		var now = new DateTime(2026, 8, 23, 3, 0, 0, DateTimeKind.Utc);
		var prog = DelayProg(77, (_, _) => 20_000M);
		var ai = new TestPathingAI { Clock = now };
		ai.SetDelayProg(prog.Object);
		var path = new TestFollowingPath(fixture.Character.Object, new[] { fixture.CellExit.Object })
		{
			PathingOwner = ai,
			SmashLockedDoors = true
		};
		fixture.Focus.PathingEpisode = path;
		fixture.Effects.Add(path);
		fixture.Focus.NextSmashAttemptUtc = now.AddSeconds(-5);
		var originalDue = fixture.Focus.NextSmashAttemptUtc;
		var blocker = new BlockingDelayedAction(fixture.Character.Object, _ => { }, "waiting to move",
			"general", string.Empty);
		fixture.Effects.Add(blocker);

		Assert.IsFalse(ai.RunCheckSmash(fixture.Character.Object));
		for (var i = 0; i < 3; i++)
		{
			path.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object);
		}

		Assert.AreSame(fixture.Focus, fixture.Effects.OfType<BreakDownDoor>().Single());
		Assert.AreEqual(originalDue, fixture.Focus.NextSmashAttemptUtc);
		Assert.AreEqual(0, ai.SmashCount);
		prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Never);

		fixture.Effects.Remove(blocker);
		Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));
		Assert.AreEqual(1, ai.SmashCount);
		Assert.AreEqual(now.AddSeconds(20), fixture.Focus.NextSmashAttemptUtc);
		prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Once);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void DoorRemovedOrReplacedOpen_CleansFocusAndContinuesEpisode(bool replacement)
	{
		var fixture = CreateFixture();
		var ai = new TestPathingAI();
		Bind(fixture, ai);
		var path = fixture.Focus.PathingEpisode!;
		if (replacement)
		{
			var openReplacement = new Mock<IDoor>();
			openReplacement.SetupGet(x => x.IsOpen).Returns(true);
			fixture.Exit.SetupGet(x => x.Door).Returns(openReplacement.Object);
		}
		else
		{
			fixture.Exit.SetupGet(x => x.Door).Returns((IDoor)null!);
		}

		Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));

		Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
		Assert.IsFalse(fixture.Effects.Contains(path));
		Assert.AreEqual(0, ai.SmashCount);
	}

	[DataTestMethod]
	[DataRow(CharacterState.Dead)]
	[DataRow(CharacterState.Stasis)]
	[DataRow(CharacterState.Paralysed)]
	public void DeadStasisOrUnableState_CleansOnlyOwnedTransientEpisode(CharacterState state)
	{
		var fixture = CreateFixture();
		fixture.Character.SetupGet(x => x.State).Returns(state);
		var owner = new TestPathingAI();
		Bind(fixture, owner);
		var path = fixture.Focus.PathingEpisode!;

		Assert.IsFalse(owner.HandleEvent(EventType.FiveSecondTick, fixture.Character.Object));

		Assert.IsFalse(fixture.Effects.Contains(path));
		Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void MultiplePathingAIs_ProcessOnlyTheirOwnEpisodeRegardlessOfOrder(bool siblingFirst)
	{
		var fixture = CreateFixture();
		var ownerProg = DelayProg(77, (_, _) => 30_000M);
		var siblingProg = DelayProg(78, (_, _) => throw new AssertFailedException("Sibling callback ran."));
		var now = new DateTime(2026, 8, 23, 0, 0, 0, DateTimeKind.Utc);
		var owner = new TestPathingAI { Clock = now };
		owner.SetDelayProg(ownerProg.Object);
		var sibling = new TestPathingAI { Clock = now, Enabled = false };
		sibling.SetDelayProg(siblingProg.Object);
		Bind(fixture, owner);
		var path = fixture.Focus.PathingEpisode!;
		path.SmashLockedDoors = true;

		if (siblingFirst)
		{
			Assert.IsFalse(sibling.RunFiveSecondTick(fixture.Character.Object));
			Assert.IsTrue(owner.RunFiveSecondTick(fixture.Character.Object));
		}
		else
		{
			Assert.IsTrue(owner.RunFiveSecondTick(fixture.Character.Object));
			Assert.IsFalse(sibling.RunFiveSecondTick(fixture.Character.Object));
		}

		Assert.IsTrue(fixture.Effects.Contains(path));
		Assert.IsTrue(fixture.Effects.Contains(fixture.Focus));
		Assert.AreEqual(now.AddSeconds(30), fixture.Focus.NextSmashAttemptUtc);
		Assert.IsTrue(path.SmashLockedDoors);
		ownerProg.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Once);
		siblingProg.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void SiblingCannotReconfigureAnOwnedPath()
	{
		var fixture = CreateFixture();
		var owner = new TestPathingAI();
		var sibling = new TestPathingAI();
		Bind(fixture, owner);
		var path = fixture.Focus.PathingEpisode!;
		path.OpenDoors = true;
		path.UseKeys = true;
		path.SmashLockedDoors = true;

		sibling.RunFollowPathAction(fixture.Character.Object, path);

		Assert.AreSame(owner, path.PathingOwner);
		Assert.IsTrue(path.OpenDoors);
		Assert.IsTrue(path.UseKeys);
		Assert.IsTrue(path.SmashLockedDoors);
		Assert.IsTrue(fixture.Effects.Contains(path));
	}

	[TestMethod]
	public void SiblingOwnedCloseBehindPath_SuppressesEarlyEventClose()
	{
		var fixture = CreateFixture();
		var owner = new TestPathingAI();
		var sibling = new TestPathingAI();
		owner.EnableCloseDoorsBehind();
		var siblingPath = new FollowingPath(fixture.Character.Object, Array.Empty<ICellExit>())
		{
			PathingOwner = sibling,
			CloseDoorsBehind = true
		};
		fixture.Effects.Add(siblingPath);
		var body = new Mock<IBody>();
		fixture.Character.SetupGet(x => x.Body).Returns(body.Object);
		fixture.Door.SetupGet(x => x.IsOpen).Returns(true);
		var destination = new Mock<ICell>();
		fixture.CellExit.SetupGet(x => x.Destination).Returns(destination.Object);
		fixture.Origin.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		destination.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());

		owner.RunCheckCloseDoor(fixture.Character.Object, fixture.CellExit.Object);

		body.Verify(x => x.Close(fixture.Door.Object, null!, null!), Times.Never);
	}

	[TestMethod]
	public void DisablingOwnerClearsOnlyItsEpisode()
	{
		var fixture = CreateFixture();
		var disabledOwner = new TestPathingAI { Enabled = false };
		Bind(fixture, disabledOwner);
		var disabledPath = fixture.Focus.PathingEpisode!;
		var sibling = new TestPathingAI();
		var siblingPath = new FollowingPath(fixture.Character.Object, Array.Empty<ICellExit>())
		{
			PathingOwner = sibling
		};
		var siblingFocus = new BreakDownDoor(fixture.Character.Object, fixture.CellExit.Object)
		{
			PathingEpisode = siblingPath
		};
		fixture.Effects.Add(siblingPath);
		fixture.Effects.Add(siblingFocus);

		Assert.IsTrue(disabledOwner.RunCheckSmash(fixture.Character.Object));

		Assert.IsFalse(fixture.Effects.Contains(disabledPath));
		Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
		Assert.IsTrue(fixture.Effects.Contains(siblingPath));
		Assert.IsTrue(fixture.Effects.Contains(siblingFocus));
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void InvalidOriginOrOpenDoorClearsOnlyOwningEpisode(bool openDoor)
	{
		var fixture = CreateFixture();
		var owner = new TestPathingAI();
		Bind(fixture, owner);
		var ownerPath = fixture.Focus.PathingEpisode!;
		var sibling = new TestPathingAI();
		var siblingPath = new FollowingPath(fixture.Character.Object, Array.Empty<ICellExit>())
		{
			PathingOwner = sibling
		};
		var siblingFocus = new BreakDownDoor(fixture.Character.Object, fixture.CellExit.Object)
		{
			PathingEpisode = siblingPath
		};
		fixture.Effects.Add(siblingPath);
		fixture.Effects.Add(siblingFocus);
		if (openDoor)
		{
			fixture.Door.SetupGet(x => x.IsOpen).Returns(true);
		}
		else
		{
			fixture.Character.SetupGet(x => x.Location).Returns(new Mock<ICell>().Object);
		}

		Assert.IsTrue(owner.RunCheckSmash(fixture.Character.Object));

		Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
		Assert.IsFalse(fixture.Effects.Contains(ownerPath));
		Assert.IsTrue(fixture.Effects.Contains(siblingPath));
		Assert.IsTrue(fixture.Effects.Contains(siblingFocus));
	}

	[TestMethod]
	public void CompletingPathClearsOnlyItsAssociatedDoorFocus()
	{
		var fixture = CreateFixture();
		var owner = new TestPathingAI();
		Bind(fixture, owner);
		var completedPath = fixture.Focus.PathingEpisode!;
		var siblingPath = new FollowingPath(fixture.Character.Object, Array.Empty<ICellExit>());
		var siblingFocus = new BreakDownDoor(fixture.Character.Object, fixture.CellExit.Object)
		{
			PathingEpisode = siblingPath
		};
		fixture.Effects.Add(siblingPath);
		fixture.Effects.Add(siblingFocus);

		completedPath.FollowPathAction();

		Assert.IsFalse(fixture.Effects.Contains(completedPath));
		Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
		Assert.IsTrue(fixture.Effects.Contains(siblingPath));
		Assert.IsTrue(fixture.Effects.Contains(siblingFocus));
	}

	[TestMethod]
	public void DoorFocusCreatedByMovementIsAssociatedWithItsPathEpisode()
	{
		var fixture = CreateFixture();
		fixture.Effects.Remove(fixture.Focus);
		EnableDoorMovement(fixture);
		var path = new TestFollowingPath(fixture.Character.Object, new[] { fixture.CellExit.Object })
		{
			SmashLockedDoors = true
		};

		var result = path.RunTryMoveThroughExit(fixture.Character.Object, fixture.CellExit.Object);
		var focus = fixture.Effects.OfType<BreakDownDoor>().Single();

		Assert.AreEqual(MovementStrategyResult.Waiting, result);
		Assert.AreSame(path, focus.PathingEpisode);
	}

	[TestMethod]
	public void OwnerlessPathStillSupportsItsDirectConsumer()
	{
		var fixture = CreateFixture();
		var path = new FollowingPath(fixture.Character.Object, Array.Empty<ICellExit>());
		fixture.Effects.Add(path);

		path.FollowPathAction();

		Assert.IsNull(path.PathingOwner);
		Assert.IsFalse(fixture.Effects.Contains(path));
	}

    [TestMethod]
    public void ConfiguredCallback_DelaysFirstAttemptAndReschedulesAfterDueAttempt()
    {
        var fixture = CreateFixture();
        var prog = DelayProg(77, (_, _) => 60_000M);
        var ai = new TestPathingAI { Clock = new DateTime(2026, 8, 22, 0, 0, 0, DateTimeKind.Utc) };
        ai.SetDelayProg(prog.Object);
		Bind(fixture, ai);

        Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));
        Assert.AreEqual(0, ai.SmashCount);
        Assert.AreEqual(ai.Clock.AddMinutes(1), fixture.Focus.NextSmashAttemptUtc);

        ai.Clock = ai.Clock.AddSeconds(59);
        ai.RunCheckSmash(fixture.Character.Object);
        Assert.AreEqual(0, ai.SmashCount);

        ai.Clock = ai.Clock.AddSeconds(1);
        ai.RunCheckSmash(fixture.Character.Object);
        Assert.AreEqual(1, ai.SmashCount);
        Assert.AreEqual(ai.Clock.AddMinutes(1), fixture.Focus.NextSmashAttemptUtc);
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Exactly(2));
    }

    [TestMethod]
    public void ConfiguredCallback_KeepsIndependentDeadlinesForEachCharacter()
    {
        var first = CreateFixture();
        var second = CreateFixture();
        var prog = DelayProg(77, (character, _) => ReferenceEquals(character, first.Character.Object) ? 10_000M : 20_000M);
        var ai = new TestPathingAI { Clock = new DateTime(2026, 8, 22, 0, 0, 0, DateTimeKind.Utc) };
        ai.SetDelayProg(prog.Object);
		Bind(first, ai);
		Bind(second, ai);

        ai.RunCheckSmash(first.Character.Object);
        ai.RunCheckSmash(second.Character.Object);

        Assert.AreEqual(ai.Clock.AddSeconds(10), first.Focus.NextSmashAttemptUtc);
        Assert.AreEqual(ai.Clock.AddSeconds(20), second.Focus.NextSmashAttemptUtc);
        Assert.AreNotEqual(first.Focus.NextSmashAttemptUtc, second.Focus.NextSmashAttemptUtc);
    }

    [TestMethod]
    public void ReconstructedFocus_RollsFreshFutureDelayBeforeAnyAttempt()
    {
        var fixture = CreateFixture();
        Assert.IsNull(fixture.Focus.NextSmashAttemptUtc);
        var prog = DelayProg(77, (_, _) => 30_000M);
		var now = new DateTime(2026, 8, 23, 4, 0, 0, DateTimeKind.Utc);
		var ai = new TestPathingAI { Clock = now };
        ai.SetDelayProg(prog.Object);
		Bind(fixture, ai);

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(0, ai.SmashCount);
		Assert.AreEqual(now.AddSeconds(30), fixture.Focus.NextSmashAttemptUtc);
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Once);
    }

    [TestMethod]
    public void ConfiguredCallback_InitialisesOnceWhileMovementIsBlockedAndHonoursDueTimeAfterUnblock()
    {
        var fixture = CreateFixture();
        var followingPath = new FollowingPath(fixture.Character.Object, new[] { fixture.CellExit.Object });
        var blocker = new BlockingDelayedAction(fixture.Character.Object, _ => { }, "waiting to move",
            "general", string.Empty);
        fixture.Effects.Add(followingPath);
        fixture.Effects.Add(blocker);
        var prog = DelayProg(77, (_, _) => 30_000M);
        var ai = new TestPathingAI { Clock = new DateTime(2026, 8, 22, 0, 0, 0, DateTimeKind.Utc) };
        ai.SetDelayProg(prog.Object);
		Bind(fixture, ai);

        Assert.IsFalse(ai.RunCheckSmash(fixture.Character.Object));
        Assert.AreEqual(ai.Clock.AddSeconds(30), fixture.Focus.NextSmashAttemptUtc);
        Assert.AreEqual(0, ai.SmashCount);
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Once);

        ai.Clock = ai.Clock.AddSeconds(10);
        Assert.IsFalse(ai.RunCheckSmash(fixture.Character.Object));
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Once);

        fixture.Effects.Remove(blocker);
        ai.Clock = ai.Clock.AddSeconds(19);
        Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));
        Assert.AreEqual(0, ai.SmashCount);

        ai.Clock = ai.Clock.AddSeconds(1);
        Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));
        Assert.AreEqual(1, ai.SmashCount);
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Exactly(2));
    }

    [TestMethod]
    public void InvalidOriginOrOpenDoor_RemovesFocusWithoutCallingCallbackOrSmashing()
    {
        var fixture = CreateFixture();
        fixture.Character.SetupGet(x => x.Location).Returns(new Mock<ICell>().Object);
        var prog = DelayProg(77, (_, _) => 1M);
        var ai = new TestPathingAI { Clock = DateTime.UtcNow };
        ai.SetDelayProg(prog.Object);
		Bind(fixture, ai);

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(0, ai.SmashCount);
        Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Never);
    }

    [TestMethod]
    public void DisabledPathing_RemovesFocusWithoutCallingCallbackOrSmashing()
    {
        var fixture = CreateFixture();
        var prog = DelayProg(77, (_, _) => 1M);
        var ai = new TestPathingAI { Clock = DateTime.UtcNow, Enabled = false };
        ai.SetDelayProg(prog.Object);
		Bind(fixture, ai);

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(0, ai.SmashCount);
        Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Never);
    }

    [TestMethod]
    public void AbsentCallback_PreservesImmediateLegacyAttempt()
    {
        var fixture = CreateFixture();
        var ai = new TestPathingAI { Clock = DateTime.UtcNow };
		Bind(fixture, ai);

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(1, ai.SmashCount);
        Assert.IsNull(fixture.Focus.NextSmashAttemptUtc);
    }

    [TestMethod]
    public void DefinitionSave_WritesCanonicalOptionalCallbackElement()
    {
        var ai = new TestPathingAI();
        ai.SetDelayProg(DelayProg(77, (_, _) => 1M).Object);

        var definition = XElement.Parse(ai.SavedDefinition);

        Assert.AreEqual("77", definition.Element("DoorSmashDelayProg")?.Value);
        StringAssert.Contains(ai.HelpText, "Number(Character, Exit)");
        StringAssert.Contains(ai.HelpText, "milliseconds");
    }

    [TestMethod]
    public void Builder_AcceptsOnlyNumberCharacterExitCallback()
    {
        var valid = new Mock<IFutureProg>();
        valid.SetupGet(x => x.Public).Returns(true);
        valid.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
        valid.SetupGet(x => x.Parameters).Returns(new[] { ProgVariableTypes.Character, ProgVariableTypes.Exit });
        valid.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
            .Returns<IEnumerable<ProgVariableTypes>>(x =>
                x.SequenceEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Exit }));

        var invalid = new Mock<IFutureProg>();
        invalid.SetupGet(x => x.Public).Returns(true);
        invalid.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
        invalid.SetupGet(x => x.Parameters).Returns(new[] { ProgVariableTypes.Character });
        invalid.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(false);

        var collection = new Mock<IUneditableAll<IFutureProg>>();
        collection.Setup(x => x.GetByIdOrName("valid", It.IsAny<bool>())).Returns(valid.Object);
        collection.Setup(x => x.GetByIdOrName("invalid", It.IsAny<bool>())).Returns(invalid.Object);
        var gameworld = new Mock<IFuturemud>();
        gameworld.SetupGet(x => x.FutureProgs).Returns(collection.Object);
        gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        var output = new Mock<IOutputHandler>();
        output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
        var actor = new Mock<ICharacter>();
        actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
        var ai = new TestPathingAI(gameworld.Object);

        Assert.IsTrue(ai.BuildingCommand(actor.Object, new StringStack("smashdelay valid")));
        Assert.AreSame(valid.Object, ai.DoorSmashDelayProg);
        Assert.IsFalse(ai.BuildingCommand(actor.Object, new StringStack("smashdelay invalid")));
        Assert.AreSame(valid.Object, ai.DoorSmashDelayProg);
        Assert.IsTrue(ai.BuildingCommand(actor.Object, new StringStack("smashdelay none")));
        Assert.IsNull(ai.DoorSmashDelayProg);
    }

    [TestMethod]
    public void PathToLocationLoadAndSave_RoundTripsCallbackWithoutChangingOwningDefinition()
    {
        var prog = DelayProg(77, (_, _) => 1M);
        var progs = new Mock<IUneditableAll<IFutureProg>>();
        progs.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => id == 77 ? prog.Object : null!);
        var gameworld = new Mock<IFuturemud>();
        gameworld.SetupGet(x => x.FutureProgs).Returns(progs.Object);
        var model = new ArtificialIntelligence
        {
            Id = 1,
            Name = "pathing",
            Type = "PathToLocation",
            Definition = "<Definition><PathingEnabledProg>0</PathingEnabledProg><OnStartToPathProg>0</OnStartToPathProg><TargetLocationProg>0</TargetLocationProg><FallbackLocationProg>0</FallbackLocationProg><WayPointsProg>0</WayPointsProg><OpenDoors>false</OpenDoors><UseKeys>false</UseKeys><SmashLockedDoors>true</SmashLockedDoors><CloseDoorsBehind>false</CloseDoorsBehind><UseDoorguards>false</UseDoorguards><MoveEvenIfObstructionInWay>false</MoveEvenIfObstructionInWay><DoorSmashDelayProg>77</DoorSmashDelayProg></Definition>"
        };
        var constructor = typeof(PathToLocationAI).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
            null, new[] { typeof(ArtificialIntelligence), typeof(IFuturemud) }, null);
        Assert.IsNotNull(constructor);
        var ai = (PathToLocationAI)constructor.Invoke(new object[] { model, gameworld.Object });
        var prepare = typeof(ArtificialIntelligenceBase).GetMethod("DefinitionForSave", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(prepare);

        var saved = XElement.Parse((string)prepare.Invoke(ai, null)!);

        Assert.AreSame(prog.Object, ai.DoorSmashDelayProg);
        Assert.AreEqual("77", saved.Element("DoorSmashDelayProg")?.Value);
    }
}
