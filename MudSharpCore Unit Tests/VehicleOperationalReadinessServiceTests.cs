#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Vehicles;
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
		long? fuelLiquidId = null, double fuelVolumePerMove = 0.0)
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.RequiredPowerSpikeInWatts).Returns(requiredPowerSpikeInWatts);
		profile.SetupGet(x => x.FuelLiquidId).Returns(fuelLiquidId);
		profile.SetupGet(x => x.FuelVolumePerMove).Returns(fuelVolumePerMove);
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

