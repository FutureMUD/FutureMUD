#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Traps;
using MudSharp.Work.Crafts;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PredatorHuntingTests
{
	[DataTestMethod]
	[DataRow("flying")]
	[DataRow("climbing")]
	[DataRow("swimming")]
	[DataRow("standing")]
	public void Grapple_PreyPostureRefresh_PreservesCarrierSupportPosture(string posture)
	{
		var f = new Fixture();
		MudSharp.Body.Position.IPositionState position = posture switch
		{
			"flying" => PositionFlying.Instance,
			"climbing" => PositionClimbing.Instance,
			"swimming" => PositionSwimming.Instance,
			_ => PositionStanding.Instance
		};
		f.Actor.SetupProperty(x => x.PositionState, position);
		f.Target.SetupGet(x => x.PositionState).Returns(PositionSprawled.Instance);
		_ = new Grappling(f.Actor.Object, f.Target.Object);
		f.Target.Raise(x => x.OnPositionChanged += null, f.Target.Object);
		Assert.AreSame(posture == "standing" ? PositionKneeling.Instance : position, f.Actor.Object.PositionState);
	}

	[TestMethod]
	public void Dropper_ApproachesGroundPreyByFlyingDownWithoutAttackingAcrossLayers()
	{
		var f = new Fixture();
		var combat = new Mock<ICombat>();
		combat.SetupGet(x => x.Combatants).Returns([f.Actor.Object, f.Target.Object]);
		f.Actor.SetupGet(x => x.Combat).Returns(combat.Object);
		f.Actor.SetupGet(x => x.CombatTarget).Returns(f.Target.Object);
		f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		f.Actor.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		f.Actor.SetupGet(x => x.CombatSettings.MovementManagement).Returns(AutomaticMovementSettings.FullyAutomatic);
		f.Actor.SetupGet(x => x.CombatSettings.AutomaticallyMoveTowardsTarget).Returns(true);
		f.Actor.As<IFly>().Setup(x => x.CanDive()).Returns((true, ""));
		var strategy = MudSharp.Combat.Strategies.DropperStrategy.Instance;
		var movement = strategy.GetType().GetMethod("HandleCombatMovement", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var move = movement.Invoke(strategy, [f.Actor.Object]) as LayerChangeMove;
		Assert.IsNotNull(move);
		Assert.AreEqual(LayerChangeMove.DesiredLayerChange.FlyDown, move.DesiredLayer);
		// Even a stale melee flag must not permit an ordinary attack between layers.
		f.Actor.SetupGet(x => x.MeleeRange).Returns(true);
		f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(true);
		foreach (var guardedStrategy in new object[] { strategy, MudSharp.Combat.Strategies.DrownerStrategy.Instance })
		{
			var attacks = guardedStrategy.GetType().GetMethod("HandleAttacks", BindingFlags.Instance | BindingFlags.NonPublic)!;
			Assert.IsNull(attacks.Invoke(guardedStrategy, [f.Actor.Object]));
			var approach = guardedStrategy.GetType().GetMethod("HandleCombatMovement", BindingFlags.Instance | BindingFlags.NonPublic)!;
			Assert.IsTrue(approach.Invoke(guardedStrategy, [f.Actor.Object]) is null or LayerChangeMove);
		}
	}

	[TestMethod]
	public void ItemTrap_LoadedBeforePlacement_ObservesEntryAfterLoginWithoutDuplicateEvents()
	{
		var f = new Fixture();
		var service = new ProximityEventService();
		f.World.SetupGet(x => x.ProximityEventService).Returns(service);
		var item = new Mock<IGameItem>();
		ICell? itemLocation = null;
		var outside = new Mock<ICell>();
		ICell preyLocation = outside.Object;
		item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		item.SetupGet(x => x.LocationLevelPerceivable).Returns(item.Object);
		item.SetupGet(x => x.Location).Returns(() => itemLocation!);
		item.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(itemLocation!, RoomLayer.GroundLevel));
		item.SetupGet(x => x.TargetedBy).Returns([]);
		f.Target.SetupGet(x => x.Location).Returns(() => preyLocation);
		f.Target.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(preyLocation, RoomLayer.GroundLevel));
		item.Setup(x => x.GetProximity(f.Target.Object)).Returns(() => ReferenceEquals(preyLocation, itemLocation)
			? Proximity.Distant : Proximity.Unapproximable);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(1);
		template.SetupGet(x => x.Triggers).Returns([new TrapTriggerDefinition(TrapTriggerType.Proximity)]);
		var templates = new RevisableAll<ITrapTemplate>(); templates.Add(template.Object);
		f.World.SetupGet(x => x.TrapTemplates).Returns(templates);
		var trap = new TrapEffect(item.Object, template.Object);
		trap.InitialEffect();
		itemLocation = f.Cell.Object;
		trap.Login();
		trap.Login();
		using (var change = service.BeginChange(ProximityChangeCause.Movement, f.Target.Object))
		{
			preyLocation = f.Cell.Object;
			change.Complete();
		}
		item.Verify(x => x.HandleEvent(EventType.PerceivableProximityChanged, It.IsAny<object[]>()), Times.Once);
		trap.RemovalEffect();
	}

	[TestMethod]
	public void TrapCapture_SpentTrapReceipt_RoundtripsAndRemainsOwnedAndLocal()
	{
		var f = new Fixture();
		f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Eligible;
		f.Ai.Hunting.Opening = AnimalHuntOpening.TrapWait;
		TrapRestraintEffect.InitialiseEffectType();
		var restraint = new TrapRestraintEffect(f.Target.Object, Guid.NewGuid(), "webbed", 10, 7);
		var loaded = (TrapRestraintEffect)Effect.LoadEffect(restraint.SaveToXml([]), f.Target.Object);
		f.Target.Setup(x => x.EffectsOfType<TrapRestraintEffect>()).Returns([loaded]);
		Assert.AreEqual(10, loaded.CreatorId);
		Assert.AreEqual(7, loaded.OriginCellId);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object), "The spent trap is absent but its applied restraint is authoritative.");
		f.Target.Setup(x => x.EffectsOfType<TrapRestraintEffect>()).Returns([new TrapRestraintEffect(f.Target.Object, restraint.TrapInstanceId, "webbed", 999, 7)]);
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Target.Setup(x => x.EffectsOfType<TrapRestraintEffect>()).Returns([new TrapRestraintEffect(f.Target.Object, restraint.TrapInstanceId, "webbed", 10, 99)]);
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Target.Setup(x => x.EffectsOfType<TrapRestraintEffect>()).Returns([]);
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void HeldPrey_FallSupport_RequiresControlColocationAndCapableCarrier()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(true);
		f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		f.Target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		f.Actor.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		f.Actor.SetupGet(x => x.MaximumDragWeight).Returns(30);
		f.Target.SetupGet(x => x.Weight).Returns(20);
		f.Actor.Setup(x => x.CanContinueFlying(null)).Returns((true, ""));
		var held = new Mock<IBeingGrappled>();
		var grip = new Mock<IGrappling>();
		held.SetupGet(x => x.Grappling).Returns(grip.Object);
		grip.SetupGet(x => x.CharacterOwner).Returns(f.Actor.Object);
		f.Target.Setup(x => x.CombinedEffectsOfType<IBeingGrappled>()).Returns([held.Object]);
		Assert.IsFalse(CombatForcedMovementUtilities.IsSupportedByGrapple(f.Target.Object));
		held.SetupGet(x => x.UnderControl).Returns(true);
		Assert.IsTrue(CombatForcedMovementUtilities.IsSupportedByGrapple(f.Target.Object));
		f.Actor.Setup(x => x.CombinedEffectsOfType<IEffect>()).Returns([new BlockLayerChange(f.Actor.Object)]);
		Assert.IsFalse(CombatForcedMovementUtilities.CanHaulTarget(f.Actor.Object, f.Target.Object));
		Assert.IsTrue(CombatForcedMovementUtilities.IsSupportedByGrapple(f.Target.Object), "A layer cooldown prevents another carry action, not passive support.");
		f.Actor.Setup(x => x.CanContinueFlying(null)).Returns((false, "injured"));
		Assert.IsFalse(CombatForcedMovementUtilities.IsSupportedByGrapple(f.Target.Object));
		f.Actor.SetupGet(x => x.PositionState).Returns(PositionClimbing.Instance);
		f.Actor.SetupGet(x => x.Race.CanClimb).Returns(true);
		Assert.IsTrue(CombatForcedMovementUtilities.IsSupportedByGrapple(f.Target.Object));
		f.Actor.SetupGet(x => x.MaximumDragWeight).Returns(10);
		Assert.IsFalse(CombatForcedMovementUtilities.IsSupportedByGrapple(f.Target.Object));
		f.Actor.SetupGet(x => x.MaximumDragWeight).Returns(30);
		f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		Assert.IsFalse(CombatForcedMovementUtilities.IsSupportedByGrapple(f.Target.Object));
	}

	[TestMethod]
	public void TreePull_AssumesClimbingWithoutGivingPreyAnIndependentSafePosture()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(true);
		f.Actor.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		f.Target.SetupProperty(x => x.PositionState, PositionSprawled.Instance);
		f.Actor.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		f.Target.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		f.Actor.Setup(x => x.Teleport(f.Cell.Object, RoomLayer.InTrees, false, false, It.IsAny<double?>()))
			.Callback(() => f.Actor.Object.RoomLayer = RoomLayer.InTrees);
		f.Target.Setup(x => x.Teleport(f.Cell.Object, RoomLayer.InTrees, false, false, It.IsAny<double?>()))
			.Callback(() => f.Target.Object.RoomLayer = RoomLayer.InTrees);
		f.Actor.SetupGet(x => x.MaximumDragWeight).Returns(30);
		f.Target.SetupGet(x => x.Weight).Returns(20);
		f.Actor.Setup(x => x.CouldTransitionToLayer(RoomLayer.InTrees)).Returns(true);
		f.Cell.Setup(x => x.Terrain(f.Target.Object).TerrainLayers).Returns([RoomLayer.GroundLevel, RoomLayer.InTrees]);
		Assert.IsTrue(CombatForcedMovementUtilities.TryForceLayerMovement(f.Actor.Object, f.Target.Object,
			RoomLayer.InTrees, ForcedMovementVerbs.Pull, 1, out var why), why);
		Assert.AreSame(PositionClimbing.Instance, f.Actor.Object.PositionState);
		Assert.AreSame(PositionSprawled.Instance, f.Target.Object.PositionState);
	}

	[TestMethod]
	public void Dropper_InClinch_KeepsContactAndSelectsSeizingAttack()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.MeleeRange).Returns(true);
		f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(true);
		f.Actor.SetupGet(x => x.CombatTarget).Returns(f.Target.Object);
		f.Actor.Setup(x => x.CanContinueFlying(null)).Returns((true, ""));
		f.Actor.SetupGet(x => x.MaximumDragWeight).Returns(30);
		f.Target.SetupGet(x => x.Weight).Returns(20);
		f.Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		f.Actor.Setup(x => x.EffectsOfType<ClinchEffect>(It.IsAny<Predicate<ClinchEffect>>()))
			.Returns([new ClinchEffect(f.Actor.Object, f.Target.Object)]);
		var natural = new Mock<INaturalAttack> { DefaultValue = DefaultValue.Mock };
		natural.SetupGet(x => x.Attack.Weighting).Returns(1);
		f.Actor.Setup(x => x.Race.UsableNaturalWeaponAttacks(f.Actor.Object, f.Target.Object, false,
			BuiltInCombatMoveType.InitiateGrapple)).Returns([natural.Object]);
		var strategy = MudSharp.Combat.Strategies.DropperStrategy.Instance;
		var breaking = typeof(MudSharp.Combat.Strategies.DropperStrategy).GetMethod("HandleClinchBreaking", BindingFlags.Instance | BindingFlags.NonPublic)!;
		Assert.IsNull(breaking.Invoke(strategy, [f.Actor.Object, true]));
		var attacks = typeof(MudSharp.Combat.Strategies.DropperStrategy).GetMethod("HandleAttacks", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var staminaProperty = typeof(CombatBase).GetProperty("PowerMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!;
		var previousStaminaExpression = staminaProperty.GetValue(null);
		try
		{
			staminaProperty.SetValue(null, Mock.Of<ITraitExpression>());
			Assert.IsInstanceOfType(attacks.Invoke(strategy, [f.Actor.Object]), typeof(InitiateGrappleMove));
		}
		finally
		{
			staminaProperty.SetValue(null, previousStaminaExpression);
		}
	}

	[TestMethod]
	public void ShelterCraft_LoadedAfterAi_PreservesReferenceAndResolvesWhenAvailable()
	{
		var f = new Fixture("<Home type='Denning'><BurrowCraftId>8</BurrowCraftId></Home>");
		f.World.Setup(x => x.Crafts.Get(8)).Returns((ICraft)null!);
		Assert.IsNull(f.Ai.BurrowCraft);
		var save = typeof(AnimalAI).GetMethod("SaveToXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
		Assert.AreEqual("8", XElement.Parse((string)save.Invoke(f.Ai, null)!).Element("Home")!.Element("BurrowCraftId")!.Value);
		var craft = new Mock<ICraft>(); craft.SetupGet(x => x.Id).Returns(8);
		f.World.Setup(x => x.Crafts.Get(8)).Returns(craft.Object);
		Assert.AreSame(craft.Object, f.Ai.BurrowCraft);
	}

	[TestMethod]
	public void HungryTrapPredator_AwayFromWeb_StartsOwnedHomePathBeforeHiding()
	{
		var f = new Fixture("<Home type='Denning' />");
		f.Ai.Hunting.Opening = AnimalHuntOpening.TrapWait;
		f.Actor.SetupGet(x => x.Corpse).Returns((ICorpse)null!);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Id).Returns(99);
		destination.SetupGet(x => x.Location).Returns(destination.Object);
		var cells = new All<ICell>(); cells.Add(f.Cell.Object); cells.Add(destination.Object);
		f.World.SetupGet(x => x.Cells).Returns(cells);
		var exit = new Mock<MudSharp.Construction.Boundary.ICellExit>();
		exit.SetupGet(x => x.Origin).Returns(f.Cell.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		f.Cell.Setup(x => x.ExitsFor(null, true)).Returns([exit.Object]);
		f.Actor.Setup(x => x.CanCross(exit.Object)).Returns((true, null!));
		f.Actor.Setup(x => x.CanMove(exit.Object, It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
		f.Actor.Setup(x => x.CanMoveForPathPlanning(exit.Object, It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
		var home = new NpcHomeBaseEffect(f.Actor.Object);
		home.SetHomeCell(destination.Object);
		f.Actor.Setup(x => x.CombinedEffectsOfType<NpcHomeBaseEffect>()).Returns([home]);
		FollowingPath? created = null;
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
		{
			created = effect as FollowingPath;
			f.Actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		});
		var prepare = typeof(AnimalAI).GetMethod("PrepareHuntingSite", BindingFlags.NonPublic | BindingFlags.Instance)!;
		Assert.IsTrue((bool)prepare.Invoke(f.Ai, [f.Actor.Object])!);
		Assert.IsNotNull(created);
		Assert.AreSame(f.Ai, created.PathingOwner);
		Assert.AreSame(exit.Object, created.Exits.Peek());
		f.Actor.Verify(x => x.ExecuteCommand("hide"), Times.Never);
	}

	[TestMethod]
	public void HungryTrapPredator_CanConstructItsMissingHuntingSite()
	{
		var f = new Fixture("<Home type='Denning'><BurrowCraftId>8</BurrowCraftId></Home>");
		f.Ai.Hunting.Opening = AnimalHuntOpening.TrapWait;
		var craft = new Mock<ICraft>();
		craft.Setup(x => x.CanDoCraft(f.Actor.Object, null, true, true)).Returns((true, ""));
		f.World.Setup(x => x.Crafts.Get(8)).Returns(craft.Object);
		var prepare = typeof(AnimalAI).GetMethod("PrepareHuntingSite", BindingFlags.NonPublic | BindingFlags.Instance)!;
		prepare.Invoke(f.Ai, [f.Actor.Object]);
		craft.Verify(x => x.BeginCraft(f.Actor.Object), Times.Once);
	}

	[TestMethod]
	public void HuntingSettings_LegacyAndRoundtrip_PreserveOptInAndExactIds()
	{
		Assert.IsFalse(AnimalHuntingSettings.Load(null).Enabled);
		var settings = new AnimalHuntingSettings { Enabled = true, ClassificationProgId = 9007199254740993,
			People = AnimalPeoplePreyPolicy.Desperate, Opening = AnimalHuntOpening.Ambush,
			Followup = AnimalHuntFollowup.Extract, PreferredLayer = RoomLayer.InTrees, MinimumSizeDifference = -3 };
		settings.IncludedRaces.UnionWith([9, 2]); settings.ExcludedRaces.Add(4); settings.PreferredRaces[2] = 12.5;
		settings.Weights["weapons"] = -30;
		var loaded = AnimalHuntingSettings.Load(XElement.Parse(settings.Save().ToString()));
		Assert.AreEqual(settings.Save().ToString(), loaded.Save().ToString());
		Assert.AreEqual(9007199254740993, loaded.ClassificationProgId);
	}

	[TestMethod]
	public void HuntingSettings_InvalidNumbers_FallBackAndClamp()
	{
		var settings = AnimalHuntingSettings.Load(XElement.Parse("""
			<Hunting enabled="true"><Engage>60</Engage><Abandon>99</Abandon><Range>-1</Range>
			<TimeoutSeconds>NaN</TimeoutSeconds><People>999</People><ClassificationProg>Infinity</ClassificationProg></Hunting>
			"""));
		Assert.AreEqual(60, settings.AbandonThreshold); Assert.AreEqual(1, settings.PursuitRange);
		Assert.AreEqual(TimeSpan.FromMinutes(5), settings.PursuitTimeout);
		Assert.AreEqual(AnimalPeoplePreyPolicy.Never, settings.People); Assert.AreEqual(0, settings.ClassificationProgId);
	}

	[TestMethod]
	public void Assessment_ObservableFactorsAndOwnCondition_ProduceDocumentedScore()
	{
		var score = AnimalThreatAssessment.Instance.Assess(new(1, .5, 0, 1, 2, 1, .25, .5), new AnimalHuntingSettings().Weights);
		Assert.AreEqual(65.0, score.Score, 0.001);
		Assert.AreEqual(-10, score.Contributions["weapons"]); Assert.AreEqual(-5, score.Contributions["owninjury"]);
		Assert.AreEqual(0, AnimalThreatAssessment.Instance.Assess(new(-100, 0, 0, 0, -100, 1, 1, 1), new AnimalHuntingSettings().Weights).Score);
		Assert.AreEqual(100, AnimalThreatAssessment.Instance.Assess(new(100, 1, 1, 1, 100, 0, 0, 0), new AnimalHuntingSettings().Weights).Score);
	}

	[DataTestMethod]
	[DataRow(AnimalPeoplePreyPolicy.Never, NeedsResult.Starving, false)]
	[DataRow(AnimalPeoplePreyPolicy.Desperate, NeedsResult.Peckish, false)]
	[DataRow(AnimalPeoplePreyPolicy.Desperate, NeedsResult.Hungry, false)]
	[DataRow(AnimalPeoplePreyPolicy.Desperate, NeedsResult.Starving, true)]
	[DataRow(AnimalPeoplePreyPolicy.Eligible, NeedsResult.Hungry, true)]
	public void Prey_PeoplePolicy_RequiresActualStarvation(AnimalPeoplePreyPolicy policy, NeedsResult needs, bool eligible)
	{
		var f = new Fixture(); f.Ai.Hunting.People = policy;
		f.Actor.SetupGet(x => x.NeedsModel.Status).Returns(needs);
		Assert.AreEqual(eligible, f.Ai.PreyRejection(f.Actor.Object, f.Target.Object) is null);
	}

	[TestMethod]
	public void Hunting_CombatPermissions_ApplyBeforeAcquisitionAndDuringTactics()
	{
		var f = new Fixture(); f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Eligible;
		f.Actor.SetupGet(x => x.CombatSettings.AttackDisarmed).Returns(false);
		f.TargetRace.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanUseWeapons = true });
		f.Target.SetupGet(x => x.CombatSettings.FallbackToUnarmedIfNoWeapon).Returns(false);
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Target.SetupGet(x => x.CombatSettings.FallbackToUnarmedIfNoWeapon).Returns(true);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Target.SetupGet(x => x.CombatSettings.FallbackToUnarmedIfNoWeapon).Returns(false);
		f.Actor.SetupGet(x => x.CombatTarget).Returns(f.Target.Object);
		var hunt = new AnimalHuntEffect(f.Actor.Object, f.Ai, f.Target.Object) { Phase = AnimalHuntPhase.Fighting };
		f.World.Setup(x => x.TryGetCharacter(20, true)).Returns(f.Target.Object);
		Assert.IsTrue(f.Ai.SelectHuntMove(f.Actor.Object, hunt, out var move));
		Assert.AreEqual(AnimalHuntPhase.Abandoned, hunt.Phase);
		Assert.IsNull(move);
	}

	[TestMethod]
	public void Hunting_RaceAndPacifismPermissions_CannotBeOverriddenByPreyPolicy()
	{
		var f = new Fixture(); f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Eligible;
		f.Actor.SetupGet(x => x.Race.CombatSettings).Returns(new RacialCombatSettings { CanAttack = false });
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Actor.SetupGet(x => x.Race.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true });
		var peaceful = new Mock<IPacifismEffect>(); peaceful.SetupGet(x => x.IsSuperPeaceful).Returns(true);
		f.Actor.Setup(x => x.Body.EffectsOfType<IPacifismEffect>(It.IsAny<Predicate<IPacifismEffect>>())).Returns([peaceful.Object]);
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Actor.Setup(x => x.Body.EffectsOfType<IPacifismEffect>(It.IsAny<Predicate<IPacifismEffect>>())).Returns([]);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void Assessment_ExtremeWeights_CannotOverflowOrProduceNaN()
	{
		var weights = new AnimalHuntingSettings().Weights;
		weights["size"] = double.MaxValue;
		weights["support"] = -double.MaxValue;
		var result = AnimalThreatAssessment.Instance.Assess(new(3, 0, 0, 0, 3, 0, 0, 0), weights);
		Assert.AreEqual(50.0, result.Score);
		Assert.IsTrue(result.Contributions.Values.All(double.IsFinite));
	}

	[TestMethod]
	public void Hunting_Readiness_RejectsMissingProgAndUnspecifiedExtractionLayer()
	{
		var f = new Fixture();
		f.Ai.Hunting.ClassificationProgId = 0;
		Assert.IsTrue(f.Ai.IsReadyToBeUsed);
		f.Ai.Hunting.EligibilityProgId = 404;
		Assert.IsFalse(f.Ai.IsReadyToBeUsed);
		f.Ai.Hunting.EligibilityProgId = 0;
		f.Ai.Hunting.Followup = AnimalHuntFollowup.Extract;
		Assert.IsFalse(f.Ai.IsReadyToBeUsed);
		f.Ai.Hunting.PreferredLayer = RoomLayer.InTrees;
		Assert.IsTrue(f.Ai.IsReadyToBeUsed);
	}

	[TestMethod]
	public void Prey_ExcludedAncestor_RemainsExcludedWhenStarvingAndHelpless()
	{
		var f = new Fixture(); f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Eligible;
		var parent = new Mock<IRace>(); parent.SetupGet(x => x.Id).Returns(99);
		f.TargetRace.SetupGet(x => x.ParentRace).Returns(parent.Object);
		f.Ai.Hunting.ExcludedRaces.Add(99);
		f.Target.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		f.Actor.SetupGet(x => x.NeedsModel.Status).Returns(NeedsResult.Starving);
		Assert.AreEqual("excluded lineage", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		Assert.IsTrue(AnimalAI.RaceInLineage(f.TargetRace.Object, [99]));
		parent.SetupGet(x => x.ParentRace).Returns(f.TargetRace.Object);
		Assert.IsFalse(AnimalAI.RaceInLineage(f.TargetRace.Object, [100])); // Corrupt ancestry cannot loop forever.
	}

	[TestMethod]
	public void Prey_OptionalSizeBounds_AreNotAnImplicitBodyMassLimit()
	{
		var f = new Fixture(); f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Eligible;
		f.Target.Setup(x => x.CurrentContextualSize(SizeContext.Scan)).Returns(SizeCategory.Huge);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Ai.Hunting.MaximumSizeDifference = 0;
		Assert.AreEqual("outside apparent size bounds", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Target.VerifyGet(x => x.Weight, Times.Never);
	}

	[TestMethod]
	public void Assessment_TargetHiddenConditionAndInvisibleWeapon_AreNotRead()
	{
		var f = new Fixture();
		var weapon = new Mock<IGameItem>();
		f.Target.SetupGet(x => x.Body.WieldedItems).Returns([weapon.Object]);
		f.Actor.Setup(x => x.CanSee(weapon.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Assert.AreEqual(50, f.Ai.AssessPrey(f.Actor.Object, f.Target.Object).Score);
		f.Target.VerifyGet(x => x.HealthStrategy, Times.Never);
		f.Target.VerifyGet(x => x.CurrentStamina, Times.Never);
		f.Target.VerifyGet(x => x.MaximumStamina, Times.Never);
		f.Target.Verify(x => x.VisibleWounds(f.Actor.Object, WoundExaminationType.Glance), Times.Once);
		weapon.Verify(x => x.GetItemType<IMeleeWeapon>(), Times.Never);
	}

	[TestMethod]
	public void TrapWait_OnlyOwnAppliedRestraintOrAllowedHelplessOpportunityQualifies()
	{
		var f = new Fixture(); f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Eligible; f.Ai.Hunting.Opening = AnimalHuntOpening.TrapWait;
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		var id = Guid.NewGuid(); var trap = new Mock<ITrap>();
		trap.SetupGet(x => x.CreatorId).Returns(f.Actor.Object.Id); trap.SetupGet(x => x.InstanceId).Returns(id);
		f.Cell.Setup(x => x.EffectsOfType<ITrap>()).Returns([trap.Object]);
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object), "Deployment alone is not capture.");
		f.Target.Setup(x => x.EffectsOfType<TrapRestraintEffect>()).Returns([new TrapRestraintEffect(f.Target.Object, id, "webbed")]);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		Assert.AreEqual(6.25, f.Ai.AssessPrey(f.Actor.Object, f.Target.Object).Contributions["vulnerability"]);
		trap.SetupGet(x => x.CreatorId).Returns(999);
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Target.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Ai.Hunting.Opportunistic = false;
		Assert.IsNotNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void Hunt_SavingTwoOwners_RestoresIndependentIntentWithoutExecuting()
	{
		var f = new Fixture(); var first = new AnimalHuntEffect(f.Actor.Object, f.Ai, f.Target.Object);
		var second = new AnimalHuntEffect(f.Target.Object, f.Ai, f.Actor.Object);
		first.RecordVenomDelivery(); first.Phase = AnimalHuntPhase.Shadowing;
		AnimalHuntEffect.InitialiseEffectType();
		var xml = first.SaveToXml([]);
		var restored = (AnimalHuntEffect)Effect.LoadEffect(xml, f.Actor.Object);
		Assert.AreEqual(first.Deadline, restored.Deadline); Assert.AreEqual(first.LastSeen, restored.LastSeen);
		Assert.AreEqual(first.TargetId, restored.TargetId); Assert.AreEqual(AnimalHuntPhase.Shadowing, restored.Phase);
		Assert.IsTrue(restored.VenomDelivered); Assert.IsFalse(second.VenomDelivered);
		f.Actor.Verify(x => x.Engage(It.IsAny<IPerceiver>(), It.IsAny<bool>()), Times.Never);
		f.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void Hunt_ExpiredSavedIntent_AbandonsWithoutReplay()
	{
		var f = new Fixture(); var saved = new AnimalHuntEffect(f.Actor.Object, f.Ai, f.Target.Object).SaveToXml([]);
		saved.Element("Effect")!.Element("Deadline")!.Value = DateTime.UtcNow.AddDays(-1).ToString("O");
		AnimalHuntEffect.InitialiseEffectType();
		var restored = (AnimalHuntEffect)Effect.LoadEffect(saved, f.Actor.Object);
		var advance = typeof(AnimalAI).GetMethod("AdvanceHunt", BindingFlags.NonPublic | BindingFlags.Instance)!;
		Assert.IsFalse((bool)advance.Invoke(f.Ai, [f.Actor.Object, restored])!);
		Assert.AreEqual(AnimalHuntPhase.Abandoned, restored.Phase);
		f.Actor.Verify(x => x.RemoveEffect(restored, false), Times.Once);
	}

	[DataTestMethod]
	[DataRow(false, false)] [DataRow(true, true)]
	public void VenomHunt_OnlyDeliveryReceiptStartsWithdrawal(bool delivered, bool expected)
	{
		var f = new Fixture(); f.Ai.Hunting.Followup = AnimalHuntFollowup.VenomWithdrawal;
		var hunt = new AnimalHuntEffect(f.Actor.Object, f.Ai, f.Target.Object); var move = new Mock<ICombatMove>();
		move.SetupGet(x => x.CharacterTargets).Returns([f.Target.Object]);
		f.Ai.HuntMoveResolved(hunt, move.Object, new CombatMoveResult { MoveWasSuccessful = true, EnvenomDelivered = delivered });
		Assert.AreEqual(expected, hunt.VenomDelivered);
		Assert.AreEqual(expected ? AnimalHuntPhase.Withdrawing : AnimalHuntPhase.Approach, hunt.Phase);
	}

	[TestMethod]
	public void Dropper_AlreadyAirborneAndBurdenChanges_RechecksPhysicalLimits()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(true);
		f.Actor.Setup(x => x.CanContinueFlying(null)).Returns((true, ""));
		f.Actor.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		f.Actor.SetupGet(x => x.MaximumDragWeight).Returns(30);
		f.Target.SetupGet(x => x.Weight).Returns(20);
		Assert.IsTrue(CombatForcedMovementUtilities.CanCarryFlying(f.Actor.Object, f.Target.Object));
		var carried = new Mock<IGameItem>(); carried.SetupGet(x => x.Weight).Returns(15);
		f.Target.SetupGet(x => x.Body.ExternalItems).Returns([carried.Object]);
		Assert.IsFalse(CombatForcedMovementUtilities.CanCarryFlying(f.Actor.Object, f.Target.Object));
		f.Actor.Verify(x => x.CanFly(), Times.Never);
	}

	[TestMethod]
	public void Ambush_RequiresVisibleLivingPreyAndLegalLayerIngress()
	{
		var f = new Fixture();
		var attack = new Mock<IAmbushAttack>();
		attack.SetupGet(x => x.SourceLayers).Returns([RoomLayer.InTrees]);
		attack.SetupGet(x => x.DestinationLayers).Returns([RoomLayer.GroundLevel]);
		f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		f.Cell.Setup(x => x.Terrain(f.Actor.Object).TerrainLayers).Returns([RoomLayer.GroundLevel, RoomLayer.InTrees]);
		f.Actor.Setup(x => x.CanClimbDown()).Returns((true, ""));
		Assert.IsTrue(AmbushAttackMove.CanAmbush(f.Actor.Object, f.Target.Object, attack.Object));
		f.Actor.Setup(x => x.CanClimbDown()).Returns((false, "restrained"));
		Assert.IsFalse(AmbushAttackMove.CanAmbush(f.Actor.Object, f.Target.Object, attack.Object));
		f.Actor.Setup(x => x.CanClimbDown()).Returns((true, ""));
		f.Target.SetupGet(x => x.State).Returns(CharacterState.Dead);
		Assert.IsFalse(AmbushAttackMove.CanAmbush(f.Actor.Object, f.Target.Object, attack.Object));
		Assert.IsFalse(AmbushAttackMove.CanAmbush(f.Actor.Object, f.Actor.Object, attack.Object));
	}

	[DataTestMethod]
	[DataRow(true, true, true)]
	[DataRow(false, true, false)]
	[DataRow(true, false, false)]
	public void Ambush_UnderwaterToSurface_RequiresSwimmingAndLegalAscent(bool canSwim, bool canAscend, bool expected)
	{
		var f = new Fixture();
		var attack = new Mock<IAmbushAttack>();
		attack.SetupGet(x => x.SourceLayers).Returns([RoomLayer.Underwater]);
		attack.SetupGet(x => x.DestinationLayers).Returns([RoomLayer.GroundLevel]);
		f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.Underwater);
		f.Actor.SetupGet(x => x.PositionState).Returns(PositionSwimming.Instance);
		f.Actor.SetupGet(x => x.Race.CanSwim).Returns(canSwim);
		f.Actor.As<ISwim>().Setup(x => x.CanAscend()).Returns((canAscend, "ascent blocked"));
		f.Cell.Setup(x => x.Terrain(f.Actor.Object).TerrainLayers).Returns([RoomLayer.GroundLevel, RoomLayer.Underwater]);

		Assert.AreEqual(expected, AmbushAttackMove.CanAmbush(f.Actor.Object, f.Target.Object, attack.Object));
		f.Actor.Verify(x => x.CanClimbUp(), Times.Never);
		f.Actor.Verify(x => x.Teleport(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<bool>(), It.IsAny<bool>(),
			It.IsAny<double?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void Trap_CaptureReceiptFollowsAppliedRestraint_NotDeploymentOrInvalidPayload()
	{
		var f = new Fixture();
		f.Cell.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(91);
		template.SetupGet(x => x.SourceKind).Returns(TrapSourceKind.Natural);
		var templates = new RevisableAll<ITrapTemplate>(); templates.Add(template.Object);
		f.World.SetupGet(x => x.TrapTemplates).Returns(templates);
		f.World.Setup(x => x.TryGetCharacter(10, true)).Returns(f.Actor.Object);
		var trap = new TrapEffect(f.Cell.Object, template.Object, f.Actor.Object);
		var payload = new Mock<ITrapPayload>();
		payload.SetupGet(x => x.Parameters).Returns(new Dictionary<string, string> { ["duration"] = "invalid" });
		var execute = typeof(TrapEffect).GetMethod("ExecuteRestraintPayload", BindingFlags.NonPublic | BindingFlags.Instance)!;
		execute.Invoke(trap, [payload.Object, f.Target.Object]);
		f.Actor.Verify(x => x.HandleEvent(EventType.TrapCaughtPrey, It.IsAny<object[]>()), Times.Never);
		var applied = false;
		f.Target.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => applied = effect is TrapRestraintEffect restraint &&
				restraint.TrapInstanceId == trap.InstanceId && restraint.CreatorId == 10 && restraint.OriginCellId == 7);
		f.Actor.Setup(x => x.HandleEvent(EventType.TrapCaughtPrey, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) =>
			{
				Assert.IsTrue(applied);
				Assert.AreSame(f.Target.Object, arguments[1]);
				Assert.AreEqual(trap.InstanceId.ToString(), arguments[2]);
			});
		payload.SetupGet(x => x.Parameters).Returns(new Dictionary<string, string> { ["duration"] = "00:00:20" });
		execute.Invoke(trap, [payload.Object, f.Target.Object]);
		f.Actor.Verify(x => x.HandleEvent(EventType.TrapCaughtPrey, It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void VenomHunt_RecoveredPreyDuringEngagementDelay_RemainsShadowed()
	{
		var f = new Fixture(); f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Eligible;
		var hunt = new AnimalHuntEffect(f.Actor.Object, f.Ai, f.Target.Object) { Phase = AnimalHuntPhase.Shadowing };
		f.Actor.Setup(x => x.EffectsOfType<CreaturePursuitEffect>(It.IsAny<Predicate<CreaturePursuitEffect>>())).Returns([hunt]);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object, true));
		Assert.IsTrue(f.Ai.AssessPrey(f.Actor.Object, f.Target.Object).Score >= f.Ai.HuntThreshold(f.Actor.Object, true));
		var expired = typeof(AnimalAI).GetMethod("HuntExpired", BindingFlags.NonPublic | BindingFlags.Instance)!;
		Assert.IsFalse((bool)expired.Invoke(f.Ai, [f.Actor.Object, hunt])!, "Fixture must have a reachable pursuit origin and an unexpired observation.");
		BlockingDelayedAction? delayed = null;
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => delayed = effect as BlockingDelayedAction);
		var queue = typeof(AnimalAI).GetMethod("QueueHuntEngagement", BindingFlags.NonPublic | BindingFlags.Instance)!;
		queue.Invoke(f.Ai, [f.Actor.Object, f.Target.Object, hunt]);
		Assert.IsNotNull(delayed);
		delayed.ExpireEffect();
		Assert.AreEqual(AnimalHuntPhase.Shadowing, hunt.Phase);
		f.Actor.Verify(x => x.Engage(It.IsAny<IPerceiver>(), It.IsAny<bool>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("Inherit", AnimalEngagementPurpose.Hunt, false)]
	[DataRow("Attack", AnimalEngagementPurpose.Territory, true)]
	public void ContextualAttack_AtHome_PreservesHuntVersusDefencePurpose(string response, AnimalEngagementPurpose purpose, bool allowed)
	{
		var f = new Fixture($"<Home type='Territorial' /><Threat><TerritoryResponse>{response}</TerritoryResponse><HungryPreyResponse>Attack</HungryPreyResponse></Threat>");
		var territory = new Territory(f.Actor.Object); territory.AddCell(f.Cell.Object);
		f.Actor.Setup(x => x.CombinedEffectsOfType<Territory>()).Returns([territory]);
		f.Ai.Hunting.People = AnimalPeoplePreyPolicy.Never;
		var decision = f.Ai.ResolveThreatDecision(f.Actor.Object, f.Target.Object);
		Assert.AreEqual(AnimalThreatResponseType.Attack, decision.Response);
		Assert.AreEqual(purpose, decision.Purpose);
		var apply = typeof(AnimalAI).GetMethod("TryApplyThreatResponse", BindingFlags.NonPublic | BindingFlags.Instance)!;
		Assert.AreEqual(allowed, (bool)apply.Invoke(f.Ai, [f.Actor.Object, f.Target.Object, decision.Response, decision.Purpose])!);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), allowed ? Times.Once() : Times.Never());
	}

	[TestMethod]
	public void CombatAction_CancelledCarryResponse_DoesNotChargePreyStamina()
	{
		var f = new Fixture();
		var move = new Mock<ICombatMove>();
		var defence = new Mock<ICombatMove>();
		move.SetupGet(x => x.CharacterTargets).Returns([f.Target.Object]);
		move.Setup(x => x.ResolveMove(defence.Object)).Returns(new CombatMoveResult
		{
			MoveWasSuccessful = true, DefenderResponseWasUsed = false
		});
		defence.SetupGet(x => x.Assailant).Returns(f.Target.Object);
		defence.SetupGet(x => x.StaminaCost).Returns(7);
		defence.Setup(x => x.UsesStaminaWithResult(It.IsAny<CombatMoveResult>())).Returns(true);
		f.TargetRace.SetupGet(x => x.RaceUsesStamina).Returns(true);
		f.Target.Setup(x => x.ResponseToMove(move.Object, f.Actor.Object)).Returns(defence.Object);
		var combat = new Mock<CombatBase> { CallBase = true };
		combat.Protected().Setup("HandleCombatResult", ItExpr.IsAny<IPerceiver>(), ItExpr.IsAny<ICombatMove>(),
			ItExpr.IsNull<ICombatMove>(), ItExpr.IsAny<CombatMoveResult>());
		combat.Object.CombatAction(f.Actor.Object, move.Object);
		f.Target.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
		defence.Verify(x => x.UsesStaminaWithResult(It.IsAny<CombatMoveResult>()), Times.Never);
	}

	[TestMethod]
	public void Carry_LostControlReleasesWithoutChargingCarryStamina()
	{
		var f = new Fixture();
		var attack = new Mock<IForcedMovementAttack>();
		var move = new ForcedMovementMove(f.Actor.Object, f.Target.Object, attack.Object, ForcedMovementVerbs.Pull, RoomLayer.InAir) { RequiresFlight = true };
		var result = move.ResolveMove(null!);
		Assert.IsFalse(move.UsesStaminaWithResult(result));
		Assert.IsFalse(result.DefenderResponseWasUsed);
		Assert.IsFalse(result.MoveWasSuccessful);
	}

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Target { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRace> TargetRace { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICell> Cell { get; } = new() { DefaultValue = DefaultValue.Mock };
		public AnimalAI Ai { get; }
		public Fixture(string configuration = "")
		{
			var always = new Mock<IFutureProg>(); always.SetupGet(x => x.Id).Returns(1);
			always.Setup(x => x.ExecuteBool(It.IsAny<bool>(), It.IsAny<object[]>())).Returns(true);
			always.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
			World.SetupGet(x => x.AlwaysTrueProg).Returns(always.Object); World.SetupGet(x => x.AlwaysFalseProg).Returns(always.Object);
			World.SetupGet(x => x.AlwaysOneProg).Returns(always.Object);
			var progs = new All<IFutureProg>(); progs.Add(always.Object); World.SetupGet(x => x.FutureProgs).Returns(progs);
			Cell.SetupGet(x => x.Id).Returns(7); Cell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition)null!);
			Cell.SetupGet(x => x.Location).Returns(Cell.Object); Cell.SetupGet(x => x.GameItems).Returns([]);
			Cell.SetupGet(x => x.Characters).Returns([Actor.Object, Target.Object]);
			Actor.SetupGet(x => x.Location).Returns(Cell.Object); Target.SetupGet(x => x.Location).Returns(Cell.Object);
			Actor.SetupGet(x => x.Id).Returns(10); Target.SetupGet(x => x.Id).Returns(20);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object); Target.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake); Target.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.NeedsModel.Status).Returns(NeedsResult.Hungry);
			Actor.Setup(x => x.CanSee(Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Actor.Setup(x => x.CanEngage(Target.Object)).Returns(true);
			Actor.SetupGet(x => x.Race.CanEatCorpses).Returns(true);
			Actor.SetupGet(x => x.Race.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true });
			Actor.Setup(x => x.Race.CanEatCorpseMaterial(It.IsAny<MudSharp.Form.Material.ISolid>())).Returns(true);
			Actor.SetupGet(x => x.CombatSettings.AttackHelpless).Returns(true);
			Actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
			Actor.SetupGet(x => x.Movement).Returns((IMovement)null!);
			Actor.SetupGet(x => x.RidingMount).Returns((ICharacter)null!);
			TargetRace.SetupGet(x => x.Id).Returns(8); TargetRace.SetupGet(x => x.CorpseModel.CreateCorpse).Returns(true);
			Target.SetupGet(x => x.Race).Returns(TargetRace.Object);
			Actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance); Target.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			Actor.SetupGet(x => x.MaximumStamina).Returns(100); Actor.SetupGet(x => x.CurrentStamina).Returns(100);
			Actor.Setup(x => x.HealthStrategy.CurrentHealthPercentage(Actor.Object)).Returns(1);
			var cells = new All<ICell>(); cells.Add(Cell.Object); World.SetupGet(x => x.Cells).Returns(cells);
			var model = new MudSharp.Models.ArtificialIntelligence { Id = 1, Name = "Hunter", Type = "Animal", Definition = $"<Definition><Feeding type='Predator' /><Hunting enabled='true'><ClassificationProg>1</ClassificationProg></Hunting>{configuration}</Definition>" };
			Ai = (AnimalAI)typeof(AnimalAI).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
				[typeof(MudSharp.Models.ArtificialIntelligence), typeof(IFuturemud)], null)!.Invoke([model, World.Object]);
		}
	}
}
