#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleOperationalReadinessServiceTests
{
	[TestMethod]
	public void CanPerformAction_NoAccessRows_AllowsByDefault()
	{
		var service = new VehicleOperationalReadinessService();
		var actor = CreateCharacter(10);
		var vehicle = CreateVehicle("cart", []);

		var result = service.CanPerformAction(vehicle.Object, actor.Object, VehicleOperationalAction.Control,
			out var access);

		Assert.IsTrue(result);
		Assert.IsTrue(access.Allowed);
		Assert.AreEqual(string.Empty, access.Reason);
	}

	[TestMethod]
	public void CanPerformAction_TaggedRows_RequireMatchingTagAndLevel()
	{
		var service = new VehicleOperationalReadinessService();
		var actor = CreateCharacter(10);
		var other = CreateCharacter(11);
		var vehicle = CreateVehicle("cart",
		[
			CreateAccess(actor.Object, "board", 1),
			CreateAccess(other.Object, "control", 2)
		]);

		var canBoard = service.CanPerformAction(vehicle.Object, actor.Object, VehicleOperationalAction.Board,
			out var boardAccess);
		var canControl = service.CanPerformAction(vehicle.Object, actor.Object, VehicleOperationalAction.Control,
			out var controlAccess);

		Assert.IsTrue(canBoard);
		Assert.AreEqual(1, boardAccess.RequiredLevel);
		Assert.IsFalse(canControl);
		StringAssert.Contains(controlAccess.Reason, "control");
	}

	[TestMethod]
	public void CanPerformAction_LevelThreeAll_GrantsOperationalActions()
	{
		var service = new VehicleOperationalReadinessService();
		var actor = CreateCharacter(10);
		var vehicle = CreateVehicle("cart", [CreateAccess(actor.Object, "board", 3)]);

		var result = service.CanPerformAction(vehicle.Object, actor.Object, VehicleOperationalAction.Repair,
			out var access);

		Assert.IsTrue(result);
		Assert.IsTrue(access.Allowed);
	}

	[TestMethod]
	public void IsInstallationFunctionalForMovement_LowConditionModule_FailsWithReason()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("cart", []);
		var installationPrototype = new Mock<IVehicleInstallationPointPrototype>();
		installationPrototype.SetupGet(x => x.Id).Returns(21);
		installationPrototype.SetupGet(x => x.Name).Returns("engine mount");
		var installation = new Mock<IVehicleInstallation>();
		installation.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		installation.SetupGet(x => x.Prototype).Returns(installationPrototype.Object);
		installation.SetupGet(x => x.IsDisabled).Returns(false);
		var installable = new Mock<IVehicleInstallable>();
		installable.Setup(x => x.IsFunctional(out It.Ref<string>.IsAny))
		           .Returns((out string reason) =>
		           {
			           reason = string.Empty;
			           return true;
		           });
		installable.Setup(x => x.IsFunctionalForMovement(out It.Ref<string>.IsAny))
		           .Returns((out string reason) =>
		           {
			           reason = "condition 40% is below movement threshold 60%";
			           return false;
		           });
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Deleted).Returns(false);
		item.SetupGet(x => x.Destroyed).Returns(false);
		item.Setup(x => x.GetItemType<IVehicleInstallable>()).Returns(installable.Object);
		installation.SetupGet(x => x.InstalledItem).Returns(item.Object);

		var result = service.IsInstallationFunctionalForMovement(installation.Object, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "movement threshold");
	}

	[TestMethod]
	public void EvaluateTowStress_LinkNearCapacity_ReportsFailureChance()
	{
		var graph = new VehicleHitchGraphService();
		var source = CreateStressVehicle(1, "tractor", 10.0);
		var target = CreateStressVehicle(2, "trailer", 96.0);
		var sourcePoint = CreateTowPoint(31, "rear hitch", canTow: true, canBeTowed: false, maxWeight: 100.0);
		var targetPoint = CreateTowPoint(32, "front ring", canTow: false, canBeTowed: true, maxWeight: 100.0);
		var link = new VehicleHitchGraphLink("test", VehicleHitchGraphLinkKind.LegacyVehicleTow,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, source.Object, null, sourcePoint.Object),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, target.Object, null, targetPoint.Object),
			null, null, false, false, string.Empty, null);
		var plan = new VehicleHitchGraphMovePlan(source.Object,
		[
			new VehicleHitchGraphTrainMember(source.Object, 0, null),
			new VehicleHitchGraphTrainMember(target.Object, 1, link)
		], [link], [], 106.0);

		var stress = graph.EvaluateTowStress(plan).Single();

		Assert.IsTrue(stress.IsWarning);
		Assert.IsTrue(stress.CanFail);
		Assert.IsTrue(stress.FailureChance > 0.0);
		Assert.AreEqual(0.96, stress.StressRatio, 0.001);
	}
	[TestMethod]
	public void TryBuildVehicleTrain_PublicMovePlan_RejectsDuplicateSourceTowPoint()
	{
		var graph = new VehicleHitchGraphService();
		var location = new Mock<ICell>().Object;
		var tractor = CreateTrainVehicle(1, "tractor", location, 10.0);
		var trailerOne = CreateTrainVehicle(2, "first trailer", location, 20.0);
		var trailerTwo = CreateTrainVehicle(3, "second trailer", location, 20.0);
		var sourcePoint = CreateTowPoint(31, "rear hitch", canTow: true, canBeTowed: false, maxWeight: 100.0);
		var targetPointOne = CreateTowPoint(32, "front ring", canTow: false, canBeTowed: true, maxWeight: 100.0);
		var targetPointTwo = CreateTowPoint(33, "front eye", canTow: false, canBeTowed: true, maxWeight: 100.0);
		var firstLink = CreateTowLink(101, tractor.Object, trailerOne.Object, sourcePoint.Object, targetPointOne.Object);
		var secondLink = CreateTowLink(102, tractor.Object, trailerTwo.Object, sourcePoint.Object, targetPointTwo.Object);
		tractor.SetupGet(x => x.TowLinks).Returns([firstLink.Object, secondLink.Object]);

		var result = graph.TryBuildVehicleTrain(null, tractor.Object, out _, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "tow point");
	}

	[TestMethod]
	public void ConsumeMovementResources_SkipsPowerCandidateRejectedByPreflight()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("cart", []);
		var switchedOffProducer = new Mock<IProducePower>();
		switchedOffProducer.SetupGet(x => x.ProducingPower).Returns(true);
		switchedOffProducer.Setup(x => x.CanDrawdownSpike(25.0)).Returns(true);
		var switchedOff = new Mock<IOnOff>();
		switchedOff.SetupGet(x => x.SwitchedOn).Returns(false);
		var liveProducer = new Mock<IProducePower>();
		liveProducer.SetupGet(x => x.ProducingPower).Returns(true);
		liveProducer.Setup(x => x.CanDrawdownSpike(25.0)).Returns(true);
		liveProducer.Setup(x => x.DrawdownSpike(25.0)).Returns(true);
		var switchedOn = new Mock<IOnOff>();
		switchedOn.SetupGet(x => x.SwitchedOn).Returns(true);
		var offInstallation = CreateInstallation(vehicle.Object,
			CreateModuleItem(producer: switchedOffProducer.Object, onOff: switchedOff.Object).Object);
		var liveInstallation = CreateInstallation(vehicle.Object,
			CreateModuleItem(producer: liveProducer.Object, onOff: switchedOn.Object).Object);
		vehicle.SetupGet(x => x.Installations).Returns([offInstallation.Object, liveInstallation.Object]);
		var profile = CreateMovementProfile(requiredPowerSpikeInWatts: 25.0);

		service.ConsumeMovementResources(vehicle.Object, profile.Object);

		switchedOffProducer.Verify(x => x.DrawdownSpike(It.IsAny<double>()), Times.Never);
		liveProducer.Verify(x => x.DrawdownSpike(25.0), Times.Once);
	}

	[TestMethod]
	public void HasFuel_WrongLiquid_ReportsWrongFuelCandidateReason()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("cart", []);
		var container = CreateLiquidContainer(liquidId: 99, volume: 10.0);
		var item = CreateModuleItem(containers: [container.Object]);
		var installation = CreateInstallation(vehicle.Object, item.Object);
		vehicle.SetupGet(x => x.Installations).Returns([installation.Object]);
		var profile = CreateMovementProfile(fuelLiquidId: 100, fuelVolumePerMove: 1.0);

		var result = service.HasFuel(vehicle.Object, profile.Object, out var candidates, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "configured fuel");
		StringAssert.Contains(candidates.Single().Reason, "wrong fuel");
	}

	[TestMethod]
	public void HasFuel_MixedFuelRequiresConfiguredFuelVolume()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("cart", []);
		var container = CreateLiquidContainer((100, 0.25), (99, 10.0));
		var item = CreateModuleItem(containers: [container.Object]);
		var installation = CreateInstallation(vehicle.Object, item.Object);
		vehicle.SetupGet(x => x.Installations).Returns([installation.Object]);
		var profile = CreateMovementProfile(fuelLiquidId: 100, fuelVolumePerMove: 1.0);

		var result = service.HasFuel(vehicle.Object, profile.Object, out var candidates, out var reason);
		service.ConsumeMovementResources(vehicle.Object, profile.Object);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "configured fuel");
		StringAssert.Contains(candidates.Single().Reason, "not contain enough");
		container.Verify(x => x.ReduceLiquidQuantity(It.IsAny<double>(), null!, It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void ConsumeMovementResources_MixedFuelConsumesConfiguredFuelOnly()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("cart", []);
		var container = CreateLiquidContainer((100, 2.0), (99, 10.0));
		var item = CreateModuleItem(containers: [container.Object]);
		var installation = CreateInstallation(vehicle.Object, item.Object);
		vehicle.SetupGet(x => x.Installations).Returns([installation.Object]);
		var profile = CreateMovementProfile(fuelLiquidId: 100, fuelVolumePerMove: 1.0);

		service.ConsumeMovementResources(vehicle.Object, profile.Object);

		Assert.AreEqual(1.0, LiquidAmount(container.Object, 100), 0.001);
		Assert.AreEqual(10.0, LiquidAmount(container.Object, 99), 0.001);
		container.Verify(x => x.ReduceLiquidQuantity(It.IsAny<double>(), null!, It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void BuildResourcePlan_ReadyFuelAndPower_AreConsumedOnlyFromSelectedUses()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("cart", []);
		var container = CreateLiquidContainer((100, 2.0), (99, 10.0));
		var fuelInstallation = CreateInstallation(vehicle.Object, CreateModuleItem(containers: [container.Object]).Object);
		var producer = new Mock<IProducePower>();
		producer.SetupGet(x => x.ProducingPower).Returns(true);
		producer.Setup(x => x.CanDrawdownSpike(25.0)).Returns(true);
		producer.Setup(x => x.DrawdownSpike(25.0)).Returns(true);
		var switchable = new Mock<IOnOff>();
		switchable.SetupGet(x => x.SwitchedOn).Returns(true);
		var powerInstallation = CreateInstallation(vehicle.Object,
			CreateModuleItem(producer: producer.Object, onOff: switchable.Object).Object);
		vehicle.SetupGet(x => x.Installations).Returns([fuelInstallation.Object, powerInstallation.Object]);
		var profile = CreateMovementProfile(requiredPowerSpikeInWatts: 25.0, fuelLiquidId: 100,
			fuelVolumePerMove: 1.0);

		var plan = service.BuildResourcePlan(vehicle.Object, profile.Object);
		service.ConsumeMovementResources(plan);

		Assert.IsTrue(plan.IsSatisfied);
		Assert.IsTrue(plan.Uses.Any(x => x.ResourceType == VehicleResourceUseType.Fuel && x.FuelContainer == container.Object));
		Assert.IsTrue(plan.Uses.Any(x => x.ResourceType == VehicleResourceUseType.Power && x.PowerProducer == producer.Object));
		Assert.AreEqual(1.0, LiquidAmount(container.Object, 100), 0.001);
		Assert.AreEqual(10.0, LiquidAmount(container.Object, 99), 0.001);
		producer.Verify(x => x.DrawdownSpike(25.0), Times.Once);
	}

	[TestMethod]
	public void BuildLongitudinalResourcePlan_PoweredProfileScalesFuelByDistanceAndPowerByTime()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("route engine", []);
		var container = CreateLiquidContainer((100, 10.0));
		var fuelInstallation = CreateInstallation(vehicle.Object,
			CreateModuleItem(containers: [container.Object]).Object);
		var producer = new Mock<IProducePower>();
		producer.SetupGet(x => x.ProducingPower).Returns(true);
		producer.Setup(x => x.CanDrawdownSpike(40.0)).Returns(true);
		producer.Setup(x => x.DrawdownSpike(40.0)).Returns(true);
		var switchedOn = new Mock<IOnOff>();
		switchedOn.SetupGet(x => x.SwitchedOn).Returns(true);
		var powerInstallation = CreateInstallation(vehicle.Object,
			CreateModuleItem(producer: producer.Object, onOff: switchedOn.Object).Object);
		vehicle.SetupGet(x => x.Installations).Returns([fuelInstallation.Object, powerInstallation.Object]);
		var profile = CreateMovementProfile(
			fuelLiquidId: 100,
			routeFuelVolumePerMetre: 0.5,
			routePowerDrawWatts: 10.0);

		var plan = service.BuildLongitudinalResourcePlan(
			vehicle.Object,
			profile.Object,
			4.0,
			TimeSpan.FromSeconds(4.0));
		service.ConsumeMovementResources(plan);

		Assert.IsTrue(plan.IsSatisfied, plan.Reason);
		Assert.AreEqual(2.0,
			plan.Uses.Single(x => x.ResourceType == VehicleResourceUseType.Fuel).FuelVolume,
			0.000001);
		Assert.AreEqual(40.0,
			plan.Uses.Single(x => x.ResourceType == VehicleResourceUseType.Power).PowerSpikeInWatts,
			0.000001);
		Assert.AreEqual(8.0, LiquidAmount(container.Object, 100), 0.000001);
		producer.Verify(x => x.DrawdownSpike(40.0), Times.Once);
	}

	[TestMethod]
	public void BuildLongitudinalResourcePlan_ExternallyPulledProfileUsesMotiveStaminaOnly()
	{
		var service = new VehicleOperationalReadinessService();
		var vehicle = CreateVehicle("wagon", []);
		var profile = CreateMovementProfile(
			fuelLiquidId: 100,
			routeFuelVolumePerMetre: 1.0,
			routePowerDrawWatts: 500.0,
			routePropulsionMode: RouteVehiclePropulsionMode.ExternallyPulled);

		var plan = service.BuildLongitudinalResourcePlan(
			vehicle.Object,
			profile.Object,
			1_000.0,
			TimeSpan.FromMinutes(10.0));

		Assert.IsTrue(plan.IsSatisfied, plan.Reason);
		Assert.AreEqual(0, plan.Uses.Count);
		Assert.AreEqual(0, plan.FuelCandidates.Count);
		Assert.AreEqual(0, plan.PowerCandidates.Count);
	}

	[TestMethod]
	public void BuildLongitudinalMovementReadiness_ContinuationAllowsOnlyTheSameActiveMovement()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble(It.IsAny<string>()))
			.Returns((string name) => name switch
			{
				"RouteCellImmediateDistanceMetres" => 3.0,
				"RouteCellProximateDistanceMetres" => 10.0,
				"RouteCellDistantDistanceMetres" => 100.0,
				"RouteCellVeryDistantDistanceMetres" => 500.0,
				"RouteCellDefaultRoomEquivalentMetres" => 100.0,
				_ => 0.0
			});
		var route = new Mock<IRouteCellDefinition>();
		route.SetupGet(x => x.LengthMetres).Returns(1_000.0);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns(route.Object);

		var activeMovement = new Mock<IMovement>();
		var otherMovement = new Mock<IMovement>();
		var actor = CreateCharacter(100L);
		actor.Setup(x => x.SamePhysicalInstance(It.IsAny<IPerceivable>()))
			.Returns((IPerceivable other) => ReferenceEquals(other, actor.Object));
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.SetupGet(x => x.Movement).Returns(activeMovement.Object);
		actor.SetupGet(x => x.Effects).Returns([]);

		var pullerBody = new Mock<IBody>();
		pullerBody.SetupGet(x => x.ExternalItems).Returns([]);
		var puller = CreateCharacter(101L);
		puller.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		puller.SetupGet(x => x.Location).Returns(cell.Object);
		puller.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		puller.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		puller.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 100.0));
		puller.SetupGet(x => x.Body).Returns(pullerBody.Object);
		puller.SetupGet(x => x.MaximumDragWeight).Returns(1_000.0);

		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Id).Returns(200L);
		exterior.SetupGet(x => x.Location).Returns(cell.Object);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		exterior.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 100.0));
		exterior.SetupGet(x => x.Deleted).Returns(false);
		exterior.SetupGet(x => x.Destroyed).Returns(false);
		exterior.SetupGet(x => x.Weight).Returns(10.0);

		var profile = CreateMovementProfile(routePropulsionMode: RouteVehiclePropulsionMode.ExternallyPulled);
		profile.SetupGet(x => x.Id).Returns(300L);
		profile.SetupGet(x => x.Name).Returns("external route movement");
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.Route);
		profile.SetupGet(x => x.RouteSpeedMetresPerSecond).Returns(4.0);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		prototype.SetupGet(x => x.OccupantSlots).Returns([]);
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);

		var vehicle = CreateVehicle("wagon", []);
		vehicle.SetupGet(x => x.Id).Returns(400L);
		vehicle.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Location).Returns(cell.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		vehicle.SetupGet(x => x.Controller).Returns(actor.Object);
		vehicle.SetupGet(x => x.Occupancies).Returns([]);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		vehicle.SetupGet(x => x.Destroyed).Returns(false);
		vehicle.SetupGet(x => x.Disabled).Returns(false);
		vehicle.Setup(x => x.IsDisabledByDamage(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
			.Returns(false);

		var motiveLink = new VehicleHitchGraphLink(
			"motive-root",
			VehicleHitchGraphLinkKind.PersistentHitch,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Character, null, puller.Object, null),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, vehicle.Object, null, null),
			null,
			null,
			false,
			false,
			string.Empty,
			null);
		var movePlan = new VehicleHitchGraphMovePlan(
			vehicle.Object,
			[new VehicleHitchGraphTrainMember(vehicle.Object, 0, null)],
			[motiveLink],
			[],
			10.0);
		var graph = new Mock<IVehicleHitchGraphService>();
		graph.Setup(x => x.TryBuildVehicleTrain(
				gameworld.Object,
				vehicle.Object,
				out It.Ref<VehicleHitchGraphMovePlan>.IsAny,
				out It.Ref<string>.IsAny,
				true))
			.Returns((IFuturemud? _, IVehicle _, out VehicleHitchGraphMovePlan plan, out string reason,
				bool _) =>
			{
				plan = movePlan;
				reason = string.Empty;
				return true;
			});
		graph.Setup(x => x.LinksInvolving(gameworld.Object, exterior.Object)).Returns([motiveLink]);
		graph.Setup(x => x.ValidateLink(motiveLink, out It.Ref<string>.IsAny))
			.Returns((VehicleHitchGraphLink _, out string reason) =>
			{
				reason = string.Empty;
				return true;
			});
		var service = new VehicleOperationalReadinessService(graph.Object,
			new Mock<IVehiclePropulsionService>().Object);

		var initialAttempt = service.BuildLongitudinalMovementReadiness(
			new VehicleLongitudinalReadinessRequest(
				vehicle.Object,
				actor.Object,
				profile.Object,
				120.0,
				TimeSpan.FromSeconds(30.0)));
		var continuation = service.BuildLongitudinalMovementReadiness(
			new VehicleLongitudinalReadinessRequest(
				vehicle.Object,
				actor.Object,
				profile.Object,
				120.0,
				TimeSpan.FromSeconds(30.0),
				ContinuingMovement: activeMovement.Object));
		var mismatchedContinuation = service.BuildLongitudinalMovementReadiness(
			new VehicleLongitudinalReadinessRequest(
				vehicle.Object,
				actor.Object,
				profile.Object,
				120.0,
				TimeSpan.FromSeconds(30.0),
				ContinuingMovement: otherMovement.Object));

		Assert.IsFalse(initialAttempt.CanMove);
		Assert.AreEqual("You cannot begin driving while you are already moving.", initialAttempt.Reason);
		Assert.IsTrue(continuation.CanMove, continuation.Reason);
		Assert.IsFalse(mismatchedContinuation.CanMove);
		Assert.AreEqual("You cannot begin driving while you are already moving.",
			mismatchedContinuation.Reason);
	}

	[TestMethod]
	public void BuildMovementReadiness_ExternalPuller_UsesDragGraphPreservesReasonAndRevalidatesCapacity()
	{
		const string expected = "The rear wagon towbar is broken.";
		var gameworld = new Mock<IFuturemud>();
		var origin = new Mock<ICell>();
		var destination = new Mock<ICell>();
		var actor = CreateCharacter(90L);
		actor.SetupGet(x => x.Location).Returns(origin.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.SetupGet(x => x.Effects).Returns([]);
		var puller = CreateCharacter(91L);
		var pullerBody = new Mock<IBody>();
		pullerBody.SetupGet(x => x.ExternalItems).Returns([]);
		puller.SetupGet(x => x.Body).Returns(pullerBody.Object);
		puller.SetupGet(x => x.MaximumDragWeight).Returns(10.0);
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Location).Returns(origin.Object);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		exterior.SetupGet(x => x.Deleted).Returns(false);
		exterior.SetupGet(x => x.Destroyed).Returns(false);
		var profile = CreateMovementProfile();
		profile.SetupGet(x => x.Id).Returns(5L);
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		prototype.SetupGet(x => x.OccupantSlots).Returns([]);
		var vehicle = CreateVehicle("wagon", []);
		vehicle.SetupGet(x => x.Id).Returns(100L);
		vehicle.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		vehicle.SetupGet(x => x.Location).Returns(origin.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Controller).Returns(actor.Object);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Occupancies).Returns([]);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		vehicle.SetupGet(x => x.Destroyed).Returns(false);
		vehicle.SetupGet(x => x.Disabled).Returns(false);
		vehicle.Setup(x => x.IsDisabledByDamage(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
			.Returns(false);
		var exitModel = new Mock<IExit>();
		exitModel.SetupGet(x => x.MaximumSizeToEnter).Returns(SizeCategory.Enormous);
		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.SetupGet(x => x.Exit).Returns(exitModel.Object);
		var emptyPlan = new VehicleHitchGraphMovePlan(vehicle.Object, [], [], [], 0.0);
		var graph = new Mock<IVehicleHitchGraphService>();
		graph.Setup(x => x.CanDragVehicleTrain(
			gameworld.Object,
			vehicle.Object,
			exit.Object,
			It.Is<IEnumerable<ICharacter>>(characters =>
				characters.Count() == 1 && ReferenceEquals(characters.Single(), puller.Object)),
			out It.Ref<VehicleHitchGraphMovePlan>.IsAny,
			out It.Ref<string>.IsAny))
			.Returns((IFuturemud? _, IVehicle _, ICellExit _, IEnumerable<ICharacter> _,
				out VehicleHitchGraphMovePlan plan, out string reason) =>
			{
				plan = emptyPlan;
				reason = expected;
				return false;
			});
		var service = new VehicleOperationalReadinessService(graph.Object,
			new Mock<IVehiclePropulsionService>().Object);

		var result = service.BuildMovementReadiness(new VehicleMovementReadinessRequest(
			vehicle.Object,
			actor.Object,
			exit.Object,
			profile.Object,
			null,
			[puller.Object]));

		Assert.IsFalse(result.CanMove);
		Assert.AreEqual(expected, result.Reason);
		graph.Verify(x => x.CanDragVehicleTrain(
			gameworld.Object,
			vehicle.Object,
			exit.Object,
			It.IsAny<IEnumerable<ICharacter>>(),
			out It.Ref<VehicleHitchGraphMovePlan>.IsAny,
			out It.Ref<string>.IsAny), Times.Once);

		var overloadedPlan = new VehicleHitchGraphMovePlan(vehicle.Object, [], [], [], 100.0);
		graph.Setup(x => x.CanDragVehicleTrain(
			gameworld.Object,
			vehicle.Object,
			exit.Object,
			It.IsAny<IEnumerable<ICharacter>>(),
			out It.Ref<VehicleHitchGraphMovePlan>.IsAny,
			out It.Ref<string>.IsAny))
			.Returns((IFuturemud? _, IVehicle _, ICellExit _, IEnumerable<ICharacter> _,
				out VehicleHitchGraphMovePlan plan, out string reason) =>
			{
				plan = overloadedPlan;
				reason = string.Empty;
				return true;
			});

		var capacityResult = service.BuildMovementReadiness(new VehicleMovementReadinessRequest(
			vehicle.Object,
			actor.Object,
			exit.Object,
			profile.Object,
			null,
			[puller.Object]));

		Assert.IsFalse(capacityResult.CanMove);
		StringAssert.Contains(capacityResult.Reason, "can only pull");
		StringAssert.Contains(capacityResult.Reason, "vehicle train weighs");
		graph.Verify(x => x.CanMoveVehicleTrain(
			It.IsAny<IFuturemud>(),
			It.IsAny<IVehicle>(),
			It.IsAny<ICellExit>(),
			out It.Ref<VehicleHitchGraphMovePlan>.IsAny,
			out It.Ref<string>.IsAny), Times.Never);
	}

	[TestMethod]
	public void EvaluateTowStress_TowPointOverride_UsesOverridePolicy()
	{
		var graph = new VehicleHitchGraphService();
		var source = CreateStressVehicle(1, "tractor", 10.0);
		var target = CreateStressVehicle(2, "trailer", 90.0);
		var sourcePoint = CreateTowPoint(31, "rear hitch", canTow: true, canBeTowed: false, maxWeight: 100.0);
		sourcePoint.SetupGet(x => x.TowStressWarningRatio).Returns(0.50);
		sourcePoint.SetupGet(x => x.TowStressFailureStartRatio).Returns(0.80);
		sourcePoint.SetupGet(x => x.TowStressMaximumFailureChance).Returns(0.40);
		sourcePoint.SetupGet(x => x.TowStressDamageMultiplier).Returns(0.08);
		var targetPoint = CreateTowPoint(32, "front ring", canTow: false, canBeTowed: true, maxWeight: 100.0);
		var link = new VehicleHitchGraphLink("test", VehicleHitchGraphLinkKind.LegacyVehicleTow,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, source.Object, null, sourcePoint.Object),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, target.Object, null, targetPoint.Object),
			null, null, false, false, string.Empty, null);
		var plan = new VehicleHitchGraphMovePlan(source.Object,
		[
			new VehicleHitchGraphTrainMember(source.Object, 0, null),
			new VehicleHitchGraphTrainMember(target.Object, 1, link)
		], [link], [], 100.0);

		var stress = graph.EvaluateTowStress(plan, VehicleTowStressPolicy.Default).Single();

		Assert.IsTrue(stress.IsWarning);
		Assert.IsTrue(stress.CanFail);
		Assert.AreEqual(0.40, stress.Policy.MaximumFailureChance, 0.001);
		Assert.AreEqual(0.08, stress.Policy.DamageMultiplier, 0.001);
		Assert.AreEqual(0.20, stress.FailureChance, 0.001);
		StringAssert.Contains(stress.Policy.Source, "rear hitch");
	}
	[TestMethod]
	public void RepairHitchLink_ValidTargetWithIncomingLink_Succeeds()
	{
		var service = new VehicleOperationalReadinessService();
		var location = new Mock<ICell>().Object;
		var tractor = CreateTrainVehicle(1, "tractor", location, 10.0);
		var trailer = CreateTrainVehicle(2, "trailer", location, 20.0);
		var sourcePoint = CreateTowPoint(31, "rear hitch", canTow: true, canBeTowed: false, maxWeight: 100.0);
		var targetPoint = CreateTowPoint(32, "front ring", canTow: false, canBeTowed: true, maxWeight: 100.0);
		var towLink = CreateTowLink(101, tractor.Object, trailer.Object, sourcePoint.Object, targetPoint.Object);
		tractor.SetupGet(x => x.TowLinks).Returns([towLink.Object]);
		trailer.SetupGet(x => x.TowLinks).Returns([towLink.Object]);
		var graphLink = new VehicleHitchGraphLink("repair-test", VehicleHitchGraphLinkKind.LegacyVehicleTow,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, tractor.Object, null, sourcePoint.Object),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, trailer.Object, null, targetPoint.Object),
			null, null, false, false, string.Empty, towLink.Object);

		var result = service.RepairHitchLink(graphLink, out var reason);

		Assert.IsTrue(result, reason);
		towLink.Verify(x => x.SetDisabled(false), Times.Once);
		towLink.Verify(x => x.SetDisabled(true), Times.Never);
	}

	private static Mock<IVehicleMovementProfilePrototype> CreateMovementProfile(double requiredPowerSpikeInWatts = 0.0,
		long? fuelLiquidId = null, double fuelVolumePerMove = 0.0,
		double routeFuelVolumePerMetre = 0.0, double routePowerDrawWatts = 0.0,
		RouteVehiclePropulsionMode routePropulsionMode = RouteVehiclePropulsionMode.Powered)
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.RequiredPowerSpikeInWatts).Returns(requiredPowerSpikeInWatts);
		profile.SetupGet(x => x.FuelLiquidId).Returns(fuelLiquidId);
		profile.SetupGet(x => x.FuelVolumePerMove).Returns(fuelVolumePerMove);
		profile.SetupGet(x => x.RouteFuelVolumePerMetre).Returns(routeFuelVolumePerMetre);
		profile.SetupGet(x => x.RoutePowerDrawWatts).Returns(routePowerDrawWatts);
		profile.SetupGet(x => x.RoutePropulsionMode).Returns(routePropulsionMode);
		return profile;
	}

	private static Mock<IVehicleInstallation> CreateInstallation(IVehicle vehicle, IGameItem item)
	{
		var installationPrototype = new Mock<IVehicleInstallationPointPrototype>();
		installationPrototype.SetupGet(x => x.Name).Returns("module bay");
		var installation = new Mock<IVehicleInstallation>();
		installation.SetupGet(x => x.Vehicle).Returns(vehicle);
		installation.SetupGet(x => x.Prototype).Returns(installationPrototype.Object);
		installation.SetupGet(x => x.IsDisabled).Returns(false);
		installation.SetupGet(x => x.InstalledItem).Returns(item);
		return installation;
	}

	private static Mock<IGameItem> CreateModuleItem(IProducePower? producer = null, IOnOff? onOff = null,
		IEnumerable<ILiquidContainer>? containers = null)
	{
		var installable = new Mock<IVehicleInstallable>();
		installable.Setup(x => x.IsFunctional(out It.Ref<string>.IsAny))
		           .Returns((out string reason) =>
		           {
			           reason = string.Empty;
			           return true;
		           });
		installable.Setup(x => x.IsFunctionalForMovement(out It.Ref<string>.IsAny))
		           .Returns((out string reason) =>
		           {
			           reason = string.Empty;
			           return true;
		           });
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Deleted).Returns(false);
		item.SetupGet(x => x.Destroyed).Returns(false);
		item.Setup(x => x.GetItemType<IVehicleInstallable>()).Returns(installable.Object);
		item.Setup(x => x.GetItemType<IProducePower>()).Returns(producer!);
		item.Setup(x => x.GetItemType<IOnOff>()).Returns(onOff!);
		item.Setup(x => x.GetItemTypes<ILiquidContainer>()).Returns(containers ?? []);
		return item;
	}

	private static Mock<ILiquidContainer> CreateLiquidContainer(long liquidId, double volume)
	{
		return CreateLiquidContainer((liquidId, volume));
	}

	private static Mock<ILiquidContainer> CreateLiquidContainer(params (long LiquidId, double Volume)[] contents)
	{
		var gameworld = new Mock<IFuturemud>();
		var instances = contents.Select(x =>
		{
			var liquid = new Mock<ILiquid>();
			liquid.SetupGet(y => y.Id).Returns(x.LiquidId);
			liquid.SetupGet(y => y.Density).Returns(1.0);
			return new LiquidInstance
			{
				Liquid = liquid.Object,
				Amount = x.Volume
			};
		}).ToList();
		var container = new Mock<ILiquidContainer>();
		container.SetupGet(x => x.LiquidVolume).Returns(contents.Sum(x => x.Volume));
		container.SetupGet(x => x.LiquidMixture).Returns(new LiquidMixture(instances, gameworld.Object));
		return container;
	}

	private static double LiquidAmount(ILiquidContainer container, long liquidId)
	{
		return container.LiquidMixture?.Instances
		                .Where(x => x.Liquid.Id == liquidId)
		                .Sum(x => x.Amount) ?? 0.0;
	}

	private static Mock<IVehicle> CreateTrainVehicle(long id, string name, ICell location, double weight)
	{
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Weight).Returns(weight);
		var vehicle = new Mock<IVehicle>();
		var exteriorComponent = new Mock<IVehicleExterior>();
		exteriorComponent.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		exterior.Setup(x => x.GetItemType<IVehicleExterior>()).Returns(exteriorComponent.Object);
		vehicle.SetupGet(x => x.Id).Returns(id);
		vehicle.SetupGet(x => x.Name).Returns(name);
		vehicle.SetupGet(x => x.Location).Returns(location);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Destroyed).Returns(false);
		vehicle.SetupGet(x => x.TowLinks).Returns([]);
		vehicle.Setup(x => x.IsDisabledByDamage(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
		       .Returns(false);
		vehicle.Setup(x => x.DamageDisabledReason(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
		       .Returns(string.Empty);
		return vehicle;
	}

	private static Mock<IVehicleTowLink> CreateTowLink(long id, IVehicle sourceVehicle, IVehicle targetVehicle,
		IVehicleTowPointPrototype sourcePoint, IVehicleTowPointPrototype targetPoint)
	{
		var link = new Mock<IVehicleTowLink>();
		link.SetupGet(x => x.Id).Returns(id);
		link.SetupGet(x => x.SourceVehicleId).Returns(sourceVehicle.Id);
		link.SetupGet(x => x.TargetVehicleId).Returns(targetVehicle.Id);
		link.SetupGet(x => x.SourceTowPointPrototypeId).Returns(sourcePoint.Id);
		link.SetupGet(x => x.TargetTowPointPrototypeId).Returns(targetPoint.Id);
		link.SetupGet(x => x.SourceVehicle).Returns(sourceVehicle);
		link.SetupGet(x => x.TargetVehicle).Returns(targetVehicle);
		link.SetupGet(x => x.SourceTowPoint).Returns(sourcePoint);
		link.SetupGet(x => x.TargetTowPoint).Returns(targetPoint);
		var noHitchItem = (IGameItem)null!;
		link.SetupGet(x => x.HitchItem).Returns(noHitchItem);
		link.SetupGet(x => x.HitchItemId).Returns((long?)null);
		link.SetupGet(x => x.IsManuallyDisabled).Returns(false);
		link.SetupGet(x => x.IsDisabled).Returns(false);
		link.SetupGet(x => x.WhyInvalid).Returns(string.Empty);
		return link;
	}
	private static Mock<ICharacter> CreateCharacter(long id)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.Setup(x => x.SameIdentity(It.IsAny<ICharacter>()))
		         .Returns((ICharacter other) => other?.Id == id);
		character.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		return character;
	}

	private static IVehicleAccessState CreateAccess(ICharacter character, string tag, int level)
	{
		var access = new Mock<IVehicleAccessState>();
		access.SetupGet(x => x.Character).Returns(character);
		access.SetupGet(x => x.AccessTag).Returns(tag);
		access.SetupGet(x => x.AccessLevel).Returns(level);
		return access.Object;
	}

	private static Mock<IVehicle> CreateVehicle(string name, IEnumerable<IVehicleAccessState> accessStates)
	{
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Name).Returns(name);
		vehicle.SetupGet(x => x.AccessStates).Returns(accessStates);
		vehicle.SetupGet(x => x.Installations).Returns([]);
		vehicle.SetupGet(x => x.DamageZones).Returns([]);
		vehicle.Setup(x => x.DamageDisabledReason(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
		       .Returns(string.Empty);
		return vehicle;
	}

	private static Mock<IVehicle> CreateStressVehicle(long id, string name, double weight)
	{
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Weight).Returns(weight);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(id);
		vehicle.SetupGet(x => x.Name).Returns(name);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		return vehicle;
	}

	private static Mock<IVehicleTowPointPrototype> CreateTowPoint(long id, string name, bool canTow, bool canBeTowed,
		double maxWeight)
	{
		var point = new Mock<IVehicleTowPointPrototype>();
		point.SetupGet(x => x.Id).Returns(id);
		point.SetupGet(x => x.Name).Returns(name);
		point.SetupGet(x => x.TowType).Returns("direct");
		point.SetupGet(x => x.CanTow).Returns(canTow);
		point.SetupGet(x => x.CanBeTowed).Returns(canBeTowed);
		point.SetupGet(x => x.MaximumTowedWeight).Returns(maxWeight);
		return point;
	}
}

