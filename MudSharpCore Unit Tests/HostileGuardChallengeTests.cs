using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class HostileGuardChallengeTests
{
	[TestMethod]
	[DataRow("north", "north", false)]
	[DataRow("north !", "north !", false)]
	[DataRow("north !!", "north", true)]
	[DataRow("north ! !!", "north !", true)]
	[DataRow("enter gate !! (carefully)", "enter gate (carefully)", true)]
	public void Syntax_Movement_SeparatesSafetyAndChallenge(string input, string expected, bool requested)
	{
		Assert.IsTrue(GuardChallengeSyntax.TryExtract(input, out var command, out var challenge, out _, minimumWords: 1));
		Assert.AreEqual(expected, command);
		Assert.AreEqual(requested, challenge);
	}

	[TestMethod]
	[DataRow("get gem chest !! (carefully)", "get gem chest (carefully)")]
	[DataRow("open chest !!", "open chest")]
	public void Syntax_Item_ConsumesMarkerBeforeMatching(string input, string expected)
	{
		Assert.IsTrue(GuardChallengeSyntax.TryExtract(input, out var command, out var requested, out _));
		Assert.IsTrue(requested); Assert.AreEqual(expected, command);
	}

	[TestMethod]
	[DataRow("get !!")]
	[DataRow("get !! gem")]
	[DataRow("get gem !! chest")]
	[DataRow("get gem !! !!")]
	[DataRow("get \"!!\" chest")]
	[DataRow("open chest (shouts !!)")]
	[DataRow("open chest !! (shouts !!)")]
	[DataRow("get gem!!")]
	public void Syntax_ReservedOrMisplacedMarker_Rejects(string input) =>
		Assert.IsFalse(GuardChallengeSyntax.TryExtract(input, out _, out _, out _));

	[TestMethod]
	public void RoomAccess_OrdinaryAndDisabledChallenge_KeepDenialWithoutRolls()
	{
		var f = new Fixture(); f.AddItemGuard(10, Outcome.Fail);
		Assert.IsFalse(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		f.World.Setup(x => x.GetStaticBool(HostileGuardChallenge.EnabledSetting)).Returns(false);
		Assert.IsNull(HostileGuardChallenge.ForItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Assert.IsFalse(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		Assert.AreEqual(0, f.Rolls.Count);
	}

	[TestMethod]
	public void ItemChallenge_PreviewsAndRechecks_DoNotRerollOrReplay()
	{
		var f = new Fixture(); var guard = f.AddItemGuard(10, Outcome.Fail);
		using var attempt = f.ItemAttempt();
		using var scope = attempt.Enter();
		Assert.IsTrue(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		Assert.IsFalse(HostileGuardChallenge.CanCommitItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Assert.AreEqual(0, f.Rolls.Count);
		Assert.IsTrue(attempt.Resolve());
		Assert.IsTrue(attempt.Resolve());
		Assert.IsTrue(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		Assert.IsTrue(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		CollectionAssert.AreEqual(new[] { guard }, f.Rolls);
		Assert.IsTrue(HostileGuardChallenge.CanCommitItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Assert.IsFalse(HostileGuardChallenge.CanCommitItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Open));
		HostileGuardChallenge.CompleteItem();
		Assert.IsFalse(attempt.Bypasses(f.Actor.Object, f.Item.Object, guard.EffectsOfType<IGuardItemEffect>().Single()));
		Assert.IsFalse(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
	}

	[TestMethod]
	public void MultipleGuards_StableOrderEachReactsOnce_AnySuccessBlocks()
	{
		var f = new Fixture(); var second = f.AddItemGuard(20, Outcome.Fail); var first = f.AddItemGuard(10, Outcome.Pass);
		using var attempt = f.ItemAttempt(); using var scope = attempt.Enter();
		Assert.IsFalse(attempt.Resolve()); Assert.IsFalse(attempt.Resolve());
		CollectionAssert.AreEqual(new[] { first, second }, f.Rolls);
		Assert.IsFalse(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
	}

	[TestMethod]
	public void ItemChallenge_AllyInvisibleDisabledAndAbsentGuards_DoNotReact()
	{
		var f = new Fixture(); var guard = f.AddItemGuard(10, Outcome.Pass);
		Mock.Get(guard).SetupGet(x => x.AllyIDs).Returns(new HashSet<long> { f.Actor.Object.Id });
		Assert.IsNull(HostileGuardChallenge.ForItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Assert.IsTrue(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		Mock.Get(guard).SetupGet(x => x.AllyIDs).Returns(new HashSet<long>());
		Mock.Get(guard).Setup(x => x.CanSee(f.Actor.Object, PerceiveIgnoreFlags.None)).Returns(false);
		Assert.IsNull(HostileGuardChallenge.ForItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Mock.Get(guard).Setup(x => x.CanSee(f.Actor.Object, PerceiveIgnoreFlags.None)).Returns(true);
		Mock.Get(guard).SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		Assert.IsNull(HostileGuardChallenge.ForItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Assert.AreEqual(0, f.Rolls.Count);
	}

	[TestMethod]
	public void ItemChallenge_GuardDepartsOrNewGuardAppears_FailsClosedWithoutReroll()
	{
		var f = new Fixture(); var guard = f.AddItemGuard(10, Outcome.Fail);
		using var attempt = f.ItemAttempt(); using var scope = attempt.Enter(); Assert.IsTrue(attempt.Resolve());
		f.AddItemGuard(20, Outcome.Fail);
		Assert.IsFalse(f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		Mock.Get(guard).SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		Assert.IsFalse(attempt.IsValid);
		Assert.IsFalse(HostileGuardChallenge.CanCommitItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	public void ItemChallenge_NestedSourceOrActorBodyChanges_InvalidatesAuthority()
	{
		var f = new Fixture(); var child = new Mock<IGameItem>(); child.SetupGet(x => x.ContainedIn).Returns(f.Item.Object);
		f.AddItemGuard(10, Outcome.Fail);
		using var attempt = HostileGuardChallenge.ForItem(f.Actor.Object, child.Object, f.Item.Object, GuardChallengeAction.Get)!;
		using var scope = attempt.Enter(); Assert.IsTrue(attempt.Resolve());
		Assert.IsTrue(attempt.Bypasses(f.Actor.Object, f.Item.Object, f.Guards[0].EffectsOfType<IGuardItemEffect>().Single()));
		child.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
		Assert.IsFalse(attempt.IsValid);
		child.SetupGet(x => x.ContainedIn).Returns(f.Item.Object);
		f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		Assert.IsFalse(attempt.IsValid);
	}

	[TestMethod]
	[DataRow("get 2gp !!")]
	[DataRow("get 2kg gem !!")]
	[DataRow("get gem !! !!")]
	[DataRow("get !! gem")]
	public void GetCommand_UnsupportedForms_RejectWithoutTargetMatchingOrRolls(string command)
	{
		var f = new Fixture(); f.AddItemGuard(10, Outcome.Fail); Invoke(typeof(InventoryModule), "Get", f.Actor.Object, command);
		f.Actor.Verify(x => x.TargetLocalItem(It.IsAny<string>()), Times.Never);
		Assert.AreEqual(0, f.Rolls.Count);
	}

	[TestMethod]
	public void GetCommand_ExplicitVersusOrdinaryAndDisabled_OnlyExplicitEnabledCommits()
	{
		var f = new Fixture(); f.AddItemGuard(10, Outcome.Fail); var committed = 0;
		f.Actor.Setup(x => x.TargetLocalItem("gem")).Returns(f.Item.Object);
		f.Body.Setup(x => x.CanGet(f.Item.Object, 0, ItemCanGetIgnore.None)).Returns(() => f.Room.CanGetAccess(f.Item.Object, f.Actor.Object));
		f.Body.Setup(x => x.Get(f.Item.Object, 0, It.IsAny<IEmote>(), false, ItemCanGetIgnore.None)).Callback(() =>
		{
			if (f.Room.CanGetAccess(f.Item.Object, f.Actor.Object)) committed++;
		});
		Invoke(typeof(InventoryModule), "Get", f.Actor.Object, "get gem");
		Assert.AreEqual(0, committed); Assert.AreEqual(0, f.Rolls.Count);
		Invoke(typeof(InventoryModule), "Get", f.Actor.Object, "get gem !!");
		Assert.AreEqual(1, committed); Assert.AreEqual(1, f.Rolls.Count);
		f.World.Setup(x => x.GetStaticBool(HostileGuardChallenge.EnabledSetting)).Returns(false);
		Invoke(typeof(InventoryModule), "Get", f.Actor.Object, "get gem !!");
		Assert.AreEqual(1, committed); Assert.AreEqual(1, f.Rolls.Count);
	}

	private static void Invoke(Type type, string method, ICharacter actor, string command) =>
		type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [actor, command]);

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OpenCommand_LockPreflightAndExplicitChallenge_KeepOtherRestrictions(bool locked)
	{
		var f = new Fixture(); var guard = f.AddItemGuard(10, Outcome.Fail);
		var room = new Mock<IRoom>();
		room.Setup(x => x.ExitsFor(f.Actor.Object, false)).Returns([]);
		room.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns(f.Guards);
		room.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([f.Item.Object]);
		f.Actor.SetupGet(x => x.Location).Returns(room.Object); f.Item.SetupGet(x => x.Location).Returns(room.Object);
		Mock.Get(guard).SetupGet(x => x.Location).Returns(room.Object);
		f.Actor.Setup(x => x.TargetLocalItem("chest")).Returns(f.Item.Object);
		var openable = new Mock<IOpenable>(); openable.SetupGet(x => x.Parent).Returns(f.Item.Object);
		f.Item.Setup(x => x.GetItemType<IOpenable>()).Returns(openable.Object);
		f.Body.Setup(x => x.CanOpen(openable.Object)).Returns(!locked);
		Invoke(typeof(ManipulationModule), "Open", f.Actor.Object, "open chest !!");
		Assert.AreEqual(locked ? 0 : 1, f.Rolls.Count);
		f.Body.Verify(x => x.Open(openable.Object, null!, It.IsAny<IEmote>(), false), locked ? Times.Never() : Times.Once());
	}

	[TestMethod]
	public void OpenCommand_UnsupportedOwnerAndPanelForms_DoNotRoll()
	{
		var f = new Fixture(); f.AddItemGuard(10, Outcome.Fail);
		var room = new Mock<IRoom>(); f.Actor.SetupGet(x => x.Location).Returns(room.Object);
		Invoke(typeof(ManipulationModule), "Open", f.Actor.Object, "open person chest !!");
		Invoke(typeof(ManipulationModule), "Open", f.Actor.Object, "open chest panel !!");
		Assert.AreEqual(0, f.Rolls.Count);
		f.Actor.Verify(x => x.TargetActor(It.IsAny<string>(), It.IsAny<PerceiveIgnoreFlags>()), Times.Never);
	}

	[TestMethod]
	public void CapturedItemAuthority_DisposedDuringRecheck_CannotCommitAfterAmbientScopeEnds()
	{
		var f = new Fixture(); f.AddItemGuard(10, Outcome.Fail);
		using var attempt = f.ItemAttempt(); using var scope = attempt.Enter(); Assert.IsTrue(attempt.Resolve());
		attempt.Dispose();
		Assert.IsFalse(attempt.CanCommitItemAction(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
	}

	[TestMethod]
	public void ItemChallenge_VisibilityOrOriginCoordinateChanges_InvalidatesAuthority()
	{
		var f = new Fixture(); f.AddItemGuard(10, Outcome.Fail);
		using var attempt = f.ItemAttempt(); using var scope = attempt.Enter(); Assert.IsTrue(attempt.Resolve());
		f.Actor.Setup(x => x.CanSee(f.Item.Object, PerceiveIgnoreFlags.None)).Returns(false);
		Assert.IsFalse(attempt.IsValid);
		f.Actor.Setup(x => x.CanSee(f.Item.Object, PerceiveIgnoreFlags.None)).Returns(true);
		f.Actor.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		Assert.IsFalse(attempt.IsValid); Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	public void ItemChallenge_RouteGuardTooFarFromTarget_IsNotABlockingGuardToChallenge()
	{
		var f = new Fixture(); var guard = f.AddItemGuard(10, Outcome.Fail);
		var room = new Mock<IRoom>(); room.SetupGet(x => x.RouteDefinition).Returns(Mock.Of<IRouteRoomDefinition>());
		room.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns(f.Guards);
		room.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([f.Item.Object]);
		f.Actor.SetupGet(x => x.Location).Returns(room.Object); f.Item.SetupGet(x => x.Location).Returns(room.Object);
		Mock.Get(guard).SetupGet(x => x.Location).Returns(room.Object);
		Mock.Get(guard).Setup(x => x.GetProximity(f.Item.Object)).Returns(Proximity.Distant);
		Assert.IsNull(HostileGuardChallenge.ForItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Get));
		Assert.AreEqual(0, f.Rolls.Count);
	}

	[TestMethod]
	public void BodyOpen_GuardChangesDuringPreflight_RevalidatesBeforeMutationWithoutReroll()
	{
		var f = new Fixture(); var guard = f.AddItemGuard(10, Outcome.Fail);
		var body = TestObjectFactory.CreateUninitialized<MudSharp.Body.Implementations.Body>(); body.Actor = f.Actor.Object;
		typeof(MudSharp.Body.Implementations.Body).GetProperty("Prototype")!.SetValue(body, Mock.Of<IBodyPrototype>());
		foreach (var name in new[] { "_bodyparts", "_organs", "_bones", "_limbs", "_prosthetics" })
		{
			var field = typeof(MudSharp.Body.Implementations.Body).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!;
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		var hand = Mock.Of<IGrab>();
		typeof(MudSharp.Body.Implementations.Body).GetField("_holdlocs", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(body, new List<IGrab> { hand });
		typeof(MudSharp.Body.Implementations.Body).GetField("_bodyparts", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(body, new HashSet<IBodypart> { hand });
		typeof(MudSharp.Body.Implementations.Body).GetProperty("EffectHandler")!.SetValue(body, Mock.Of<IEffectHandler>());
		f.Actor.SetupGet(x => x.Body).Returns(body);
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns(() => (f.Room.CanGetAccess(f.Item.Object, f.Actor.Object), "Guard blocks access."));
		var openable = new Mock<IOpenable>(); openable.SetupGet(x => x.Parent).Returns(f.Item.Object);
		openable.Setup(x => x.CanOpen(body)).Returns(true);
		using var attempt = HostileGuardChallenge.ForItem(f.Actor.Object, f.Item.Object, null, GuardChallengeAction.Open)!;
		using var scope = attempt.Enter(); Assert.IsTrue(body.CanOpen(openable.Object)); Assert.IsTrue(attempt.Resolve());
		openable.Setup(x => x.CanOpen(body))
			.Callback(() => Mock.Get(guard).SetupGet(x => x.Location).Returns(Mock.Of<IRoom>())).Returns(true);
		body.Open(openable.Object, null!, null!);
		openable.Verify(x => x.Open(), Times.Never); Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	[DataRow(0.0, 8.0)]
	[DataRow(70.0, 63.0)]
	public void GuardingFormula_BaseFloorIsBeforeDifficultyModifier(double skillValue, double expected)
	{
		var f = new Fixture();
		var skill = Mock.Of<ITraitDefinition>(x => x.Id == 42);
		f.World.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition> { skill });
		var expression = new MudSharp.Body.Traits.TraitExpression(new MudSharp.Models.TraitExpression
		{
			Id = 15, Name = "Guarding", Expression = "max(15, guarding:42{nobranch})"
		}, f.World.Object);
		f.World.SetupGet(x => x.TraitExpressions).Returns(new All<ITraitExpression> { expression });
		f.Actor.Setup(x => x.TraitValue(skill, TraitBonusContext.None)).Returns(skillValue);
		f.Actor.Setup(x => x.EffectsOfType<ICheckBonusEffect>()).Returns([]);
		f.Actor.SetupGet(x => x.Merits).Returns([]);
		var template = new MudSharp.Models.CheckTemplate { Name = "Guarding", ImproveTraits = true };
		foreach (var difficulty in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new()
		{
			Difficulty = (int)difficulty, Modifier = difficulty == Difficulty.Normal ? -7 : 0
		});
		var check = new MudSharp.RPG.Checks.StandardCheck(new MudSharp.Models.Check
		{
			Type = (int)CheckType.GuardingItemChallengeCheck, TraitExpressionId = 15, CheckTemplate = template
		}, f.World.Object);
		var bonusField = typeof(MudSharp.RPG.Checks.StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!;
		var previous = bonusField.GetValue(null);
		try
		{
			bonusField.SetValue(null, 10);
			Assert.AreEqual(expected, check.TargetNumber(f.Actor.Object, Difficulty.Normal, null!));
		}
		finally { bonusField.SetValue(null, previous); }
	}

	[TestMethod]
	public void CreateMovement_Challenge_PreservesSafeMovementAndRollsOnlyAfterPreflight()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Fail);
		f.Actor.Setup(x => x.CanMove(f.Exit.Object, CanMoveFlags.None)).Returns(new CanMoveResponse { Result = false, ErrorMessage = "Safe movement warning" });
		f.Actor.Setup(x => x.CanMove(f.Exit.Object, CanMoveFlags.IgnoreSafeMovement)).Returns(CanMoveResponse.True);
		using (var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!)
		using (attempt.Enter())
			Assert.IsNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object));
		Assert.AreEqual(0, f.Rolls.Count);
		using (var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!)
		using (attempt.Enter())
			Assert.IsNotNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object, ignoreSafeMovement: true));
		Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	public void CanStartMovement_ChallengePreview_DoesNotBindOrRollBeforeActualCreation()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Fail);
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!;
		using var scope = attempt.Enter();
		Assert.IsTrue(Movement.CanStartMovement(f.Actor.Object, f.Exit.Object));
		Assert.IsTrue(Movement.CanStartMovement(f.Actor.Object, f.Exit.Object));
		Assert.AreEqual(0, f.Rolls.Count);
		Assert.IsNotNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object));
		Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	public void CanStartMovement_ChallengeOnlyBlocksLeftBehindMember_RejectsWithoutConsumingChallenge()
	{
		var f = new Fixture(); var guard = f.AddExitGuard(10, Outcome.Fail);
		var member = new Mock<ICharacter>();
		member.SetupGet(x => x.Location).Returns(f.Room);
		member.SetupGet(x => x.Riders).Returns([]);
		member.Setup(x => x.EffectsOfType<IDragParticipant>(It.IsAny<Predicate<IDragParticipant>>())).Returns([]);
		member.Setup(x => x.CanMove(f.Exit.Object, CanMoveFlags.None)).Returns(new CanMoveResponse { Result = false, ErrorMessage = "Cannot move" });
		var effect = guard.EffectsOfType<IGuardExitEffect>().Single();
		Mock.Get(effect).Setup(x => x.PermittedToCross(f.Actor.Object, f.Exit.Object)).Returns(true);
		Mock.Get(effect).Setup(x => x.PermittedToCross(null!, f.Exit.Object)).Returns(true);
		var party = new Mock<IParty>();
		party.SetupGet(x => x.Leader).Returns(f.Actor.Object);
		party.SetupGet(x => x.CharacterMembers).Returns([f.Actor.Object, member.Object]);
		f.Actor.SetupGet(x => x.Party).Returns(party.Object);
		f.Actor.Setup(x => x.SamePhysicalInstance(f.Actor.Object)).Returns(true);
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!;
		using var scope = attempt.Enter();
		Assert.IsTrue(attempt.HasBlockingExitGuards, "Prospective party must have a guard to challenge.");
		Assert.IsFalse(Movement.CanStartMovement(f.Actor.Object, f.Exit.Object));
		Assert.AreEqual(0, f.Rolls.Count);
		Assert.IsNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object));
		Assert.AreEqual(0, f.Rolls.Count);
	}

	[TestMethod]
	public void ExitChallenge_ClosedDoorPreflight_DoesNotRoll()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Fail);
		f.Actor.Setup(x => x.CanCross(f.Exit.Object)).Returns((false, (IEmoteOutput)null!));
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!; using var scope = attempt.Enter();
		Assert.IsNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object)); Assert.AreEqual(0, f.Rolls.Count);
	}

	[TestMethod]
	public void ExitChallenge_DraggedTargetDoesNotNeedVoluntaryMovementAbility()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Fail);
		var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		target.SetupGet(x => x.Location).Returns(f.Room); target.SetupGet(x => x.Riders).Returns([]);
		target.Setup(x => x.CanCross(f.Exit.Object)).Returns((true, (IEmoteOutput)null!));
		target.Setup(x => x.CanMove(f.Exit.Object, It.IsAny<CanMoveFlags>())).Returns(new CanMoveResponse { Result = false, ErrorMessage = "Cannot move independently." });
		var party = new Mock<IParty>(); party.SetupGet(x => x.Leader).Returns(f.Actor.Object);
		party.SetupGet(x => x.CharacterMembers).Returns([f.Actor.Object, target.Object]);
		f.Actor.SetupGet(x => x.Party).Returns(party.Object);
		f.Actor.Setup(x => x.SamePhysicalInstance(f.Actor.Object)).Returns(true);
		typeof(Room).GetProperty("Movements", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(f.Room, new List<IMovement>());
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!;
		using var scope = attempt.Enter();
		Assert.IsTrue(attempt.BindCohort([f.Actor.Object, target.Object])); Assert.IsTrue(attempt.Resolve());
		var movement = new Movement(f.Actor.Object, null!, [], [], [f.Actor.Object], [], [target.Object], [], f.Exit.Object,
			guardChallenge: attempt);
		f.Actor.Setup(x => x.ExecuteMove(movement, It.IsAny<IMoveSpeed>()))
			.Callback(() => throw new InvalidOperationException("Reached the admitted crossing."));
		var reached = Assert.ThrowsException<InvalidOperationException>(movement.IntermediateStep);
		Assert.AreEqual("Reached the admitted crossing.", reached.Message);
		target.Verify(x => x.CanMove(It.IsAny<IRoomExit>(), It.IsAny<CanMoveFlags>()), Times.Never);
		Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	public void ExitChallenge_BarrierArrivingDuringMovement_RetainsOneContactWithoutReroll()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Fail);
		var boundary = new Mock<IExit>(); var reverse = new Mock<IRoomExit>();
		var destination = Mock.Get(f.Exit.Object.Destination);
		var barriers = new List<IEffect>();
		typeof(Room).GetProperty(nameof(Room.Gameworld))!.SetValue(f.Room, f.World.Object);
		typeof(Room).GetProperty("Movements", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(f.Room, new List<IMovement>());
		destination.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		boundary.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		boundary.SetupGet(x => x.Effects).Returns(barriers);
		boundary.Setup(x => x.RoomExitFor(f.Room)).Returns(f.Exit.Object);
		f.Exit.SetupGet(x => x.Exit).Returns(boundary.Object);
		f.Exit.SetupGet(x => x.Opposite).Returns(reverse.Object);
		reverse.SetupGet(x => x.Exit).Returns(boundary.Object);
		reverse.SetupGet(x => x.Origin).Returns(destination.Object);
		reverse.SetupGet(x => x.Destination).Returns(f.Room);
		f.Body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		f.Actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(other => ReferenceEquals(other, f.Actor.Object));
		f.Actor.SetupProperty(x => x.Movement);
		f.Actor.SetupGet(x => x.QueuedMoveCommands).Returns(new Queue<string>());
		f.Actor.Setup(x => x.CanCross(f.Exit.Object)).Returns(() =>
			(!barriers.OfType<IExitBarrierEffect>().Any(x => ExitBarrierTraversal.Blocks(x, f.Actor.Object, f.Exit.Object)),
			 (IEmoteOutput)null!));
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!;
		using var scope = attempt.Enter();
		var movement = (Movement)Movement.CreateMovement(f.Actor.Object, f.Exit.Object)!;
		Assert.IsNotNull(movement); f.Actor.Object.Movement = movement;
		var barrier = new Mock<IExitBarrierTraversalEffect>();
		barrier.SetupGet(x => x.IsActive).Returns(true);
		barrier.Setup(x => x.OccupiesLayerAndPlane(f.Actor.Object)).Returns(true);
		barrier.Setup(x => x.Contact(f.Actor.Object, f.Exit.Object, It.IsAny<Func<bool>>()))
			.Callback<ICharacter, IRoomExit, Func<bool>>((_, _, continuation) => Assert.IsTrue(continuation()));
		barriers.Add(barrier.Object);
		Assert.IsFalse(f.Actor.Object.CanCross(f.Exit.Object).Success, "Access previews remain pure and blocked.");
		movement.IntermediateStep(); movement.IntermediateStep();
		barrier.Verify(x => x.Contact(f.Actor.Object, f.Exit.Object, It.IsAny<Func<bool>>()), Times.Once);
		Assert.IsTrue(movement.Cancelled); Assert.AreEqual(1, f.Rolls.Count);
		f.Actor.Verify(x => x.ExecuteMove(It.IsAny<IMovement>(), It.IsAny<IMoveSpeed>()), Times.Never);
	}

	[TestMethod]
	public void ExitChallenge_BlockedAttempt_DelaysChallengerTwoSecondsWithoutCrime()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Pass);
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!; using var scope = attempt.Enter();
		Assert.IsNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object));
		f.Actor.Verify(x => x.AddEffect(It.IsAny<BlockingCommandDelay>(), TimeSpan.FromSeconds(2)), Times.Once);
		Assert.AreEqual(1, f.Rolls.Count);
		f.World.VerifyGet(x => x.LegalAuthorities, Times.Never);
	}

	[TestMethod]
	public void ExitChallenge_ContextDoesNotAuthorizeAnotherExitOrOrdinaryFollower()
	{
		var f = new Fixture(); var guard = f.AddExitGuard(10, Outcome.Fail);
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!; using var scope = attempt.Enter();
		Assert.IsNotNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object));
		Assert.IsTrue(attempt.Bypasses(f.Actor.Object, f.Exit.Object, guard.EffectsOfType<IGuardExitEffect>().Single()));
		Assert.IsFalse(attempt.Bypasses(f.Actor.Object, Mock.Of<IRoomExit>(), guard.EffectsOfType<IGuardExitEffect>().Single()));
		var follower = new Mock<ICharacter>();
		follower.SetupGet(x => x.Gameworld).Returns(f.World.Object); follower.SetupGet(x => x.Riders).Returns([]);
		follower.Setup(x => x.CanMove(f.Exit.Object, CanMoveFlags.None)).Returns(CanMoveResponse.True);
		follower.Setup(x => x.EffectsOfType<IDragParticipant>(It.IsAny<Predicate<IDragParticipant>>())).Returns([]);
		follower.Setup(x => x.EffectsOfType<ISneakEffect>(It.IsAny<Predicate<ISneakEffect>>())).Returns([]);
		Assert.IsNotNull(Movement.CreateMovement(follower.Object, f.Exit.Object), "Ordinary permitted autofollow must not inherit the bound challenge.");
		Assert.IsFalse(attempt.Bypasses(follower.Object, f.Exit.Object, guard.EffectsOfType<IGuardExitEffect>().Single()));
		Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	public void ExitChallenge_MountOrRiderChanges_InvalidateBoundCohort()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Fail);
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!; using var scope = attempt.Enter();
		Assert.IsNotNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object));
		f.Actor.SetupGet(x => x.RidingMount).Returns(Mock.Of<ICharacter>());
		Assert.IsFalse(attempt.IsValid); Assert.AreEqual(1, f.Rolls.Count);
	}

	[TestMethod]
	public void ExitChallenge_ExistingImmwalkExemption_DoesNotRoll()
	{
		var f = new Fixture(); f.AddExitGuard(10, Outcome.Pass);
		f.Actor.Setup(x => x.AffectedBy<IImmwalkEffect>()).Returns(true);
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!; using var scope = attempt.Enter();
		Assert.IsNull(Movement.CreateMovement(f.Actor.Object, f.Exit.Object)); Assert.AreEqual(0, f.Rolls.Count);
	}

	[TestMethod]
	public void ExitChallenge_GuardInMovingParty_StillReactsToUnpermittedCohortCrossing()
	{
		var f = new Fixture(); var guard = f.AddExitGuard(10, Outcome.Fail);
		var party = new Mock<IParty>(); party.SetupGet(x => x.Leader).Returns(f.Actor.Object);
		party.SetupGet(x => x.CharacterMembers).Returns([f.Actor.Object, guard]);
		f.Actor.SetupGet(x => x.Party).Returns(party.Object); f.Actor.Setup(x => x.SamePhysicalInstance(f.Actor.Object)).Returns(true);
		Mock.Get(guard).SetupGet(x => x.Riders).Returns([]);
		Mock.Get(guard).Setup(x => x.EffectsOfType<IDragParticipant>(It.IsAny<Predicate<IDragParticipant>>())).Returns([]);
		using var attempt = HostileGuardChallenge.ForExit(f.Actor.Object, f.Exit.Object)!; using var scope = attempt.Enter();
		Assert.IsTrue(attempt.BindCohort([f.Actor.Object, guard])); Assert.IsTrue(attempt.Resolve()); Assert.AreEqual(1, f.Rolls.Count);
	}

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Room Room { get; } = TestObjectFactory.CreateUninitialized<Room>();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public Mock<IRoomExit> Exit { get; } = new();
		public List<ICharacter> Guards { get; } = [];
		public List<ICharacter> Rolls { get; } = [];
		private readonly Dictionary<ICharacter, Outcome> _outcomes = new(ReferenceEqualityComparer.Instance);
		public Fixture()
		{
			World.Setup(x => x.GetStaticBool(HostileGuardChallenge.EnabledSetting)).Returns(true);
			World.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(1000);
			Actor.SetupGet(x => x.Id).Returns(1); Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.Location).Returns(Room); Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
			Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), PerceiveIgnoreFlags.None)).Returns(true);
			Actor.SetupGet(x => x.Riders).Returns([]);
			Actor.Setup(x => x.EffectsOfType<IDragParticipant>(It.IsAny<Predicate<IDragParticipant>>())).Returns([]);
			Actor.Setup(x => x.EffectsOfType<ISneakEffect>(It.IsAny<Predicate<ISneakEffect>>())).Returns([]);
			Actor.Setup(x => x.CanMove(It.IsAny<IRoomExit>(), It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
			Actor.Setup(x => x.CanCross(It.IsAny<IRoomExit>())).Returns((true, (IEmoteOutput)null!));
			Actor.Setup(x => x.CanManipulateItem(It.IsAny<IGameItem>())).Returns((true, string.Empty));
			Body.SetupGet(x => x.Actor).Returns(Actor.Object);
			Body.Setup(x => x.CanSee(Room, It.IsAny<IRoomExit>(), PerceiveIgnoreFlags.None)).Returns(true);
			Exit.SetupGet(x => x.Origin).Returns(Room); Exit.SetupGet(x => x.Destination).Returns(Mock.Of<IRoom>());
			Item.SetupGet(x => x.Location).Returns(Room);
			typeof(Location).GetField("_characters", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(Room, Guards);
			typeof(Location).GetField("_gameItems", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(Room, new List<IGameItem> { Item.Object });
			var check = new Mock<ICheck>();
			check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), Difficulty.Normal, Actor.Object, null, 0,
				TraitUseType.Practical, It.IsAny<(string, object)[]>()))
				.Returns((IPerceivableHaveTraits guard, Difficulty _, IPerceivable target, IUseTrait tool, double bonus,
					TraitUseType use, (string, object)[] parameters) =>
				{
					Rolls.Add((ICharacter)guard);
					return CheckOutcome.SimpleOutcome(CheckType.GuardingItemChallengeCheck, _outcomes[(ICharacter)guard]);
				});
			World.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);
		}
		public ICharacter AddItemGuard(long id, Outcome outcome)
		{
			var guard = new Mock<ICharacter>();
			guard.SetupGet(x => x.Id).Returns(id); guard.SetupGet(x => x.Location).Returns(Room);
			guard.SetupGet(x => x.State).Returns(CharacterState.Awake);
			guard.Setup(x => x.ColocatedWith(Actor.Object)).Returns(true);
			guard.Setup(x => x.CanSee(Actor.Object, PerceiveIgnoreFlags.None)).Returns(true);
			guard.SetupGet(x => x.TrustedAllyIDs).Returns(new HashSet<long>());
			guard.SetupGet(x => x.AllyIDs).Returns(new HashSet<long>());
			var effect = new Mock<IGuardItemEffect>();
			effect.SetupGet(x => x.TargetItem).Returns(Item.Object); effect.Setup(x => x.Applies()).Returns(true);
			guard.Setup(x => x.EffectsOfType<IGuardItemEffect>(It.IsAny<Predicate<IGuardItemEffect>>())).Returns([effect.Object]);
			Guards.Add(guard.Object); _outcomes[guard.Object] = outcome;
			return guard.Object;
		}
		public HostileGuardChallenge ItemAttempt() => HostileGuardChallenge.ForItem(Actor.Object, Item.Object, null, GuardChallengeAction.Get)!;
		public ICharacter AddExitGuard(long id, Outcome outcome)
		{
			var guard = AddItemGuard(id, outcome);
			var effect = new Mock<IGuardExitEffect>(); effect.SetupGet(x => x.Exit).Returns(Exit.Object);
			effect.Setup(x => x.Applies()).Returns(true);
			Mock.Get(guard).Setup(x => x.EffectsOfType<IGuardExitEffect>(It.IsAny<Predicate<IGuardExitEffect>>())).Returns([effect.Object]);
			return guard;
		}
	}
}
