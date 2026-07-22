#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleRouteContractTests
{
	[TestMethod]
	public void DefaultStaticSettings_ContainLockedRouteCellDistances()
	{
		var settings = DefaultStaticSettings.DefaultStaticConfigurations;

		Assert.AreEqual("3", settings["RouteCellImmediateDistanceMetres"]);
		Assert.AreEqual("10", settings["RouteCellProximateDistanceMetres"]);
		Assert.AreEqual("100", settings["RouteCellDistantDistanceMetres"]);
		Assert.AreEqual("500", settings["RouteCellVeryDistantDistanceMetres"]);
		Assert.AreEqual("100", settings["RouteCellDefaultRoomEquivalentMetres"]);
		Assert.AreEqual("30", settings["RouteCellMovementCheckpointSeconds"]);
		Assert.AreEqual("30", settings["VehicleServiceRetrySeconds"]);
		Assert.AreEqual("15", settings["VehicleServiceDefaultMaximumHoldMinutes"]);
		Assert.AreEqual("2", settings["VehicleRouteDefaultDockingToleranceMetres"]);
	}

	[TestMethod]
	public void MovementProfile_DefaultRoutePropertiesPreserveExistingProfiles()
	{
		IVehicleMovementProfilePrototype profile = new MovementProfilePrototypeStub();

		Assert.AreEqual(0.0, profile.RouteSpeedMetresPerSecond);
		Assert.AreEqual(RouteVehiclePropulsionMode.Powered, profile.RoutePropulsionMode);
		Assert.AreEqual(0.0, profile.RouteFuelVolumePerMetre);
		Assert.AreEqual(0.0, profile.RoutePowerDrawWatts);
		Assert.IsFalse(profile.AutomaticOperationCapable);
	}

	[TestMethod]
	public void CompartmentPrototype_DefaultInteriorPropertiesPreserveExistingPrototypes()
	{
		IVehicleCompartmentPrototype compartment = new CompartmentPrototypeStub();

		Assert.IsNull(compartment.InteriorTerrainId);
		Assert.IsNull(compartment.InteriorTerrain);
		Assert.AreEqual(CellOutdoorsType.Indoors, compartment.InteriorOutdoorsType);
	}

	[TestMethod]
	public void RouteStepContracts_AreStronglyTypedSpatialSteps()
	{
		var linearInterfaces = typeof(IVehicleRouteLinearStep).GetInterfaces();
		var exitInterfaces = typeof(IVehicleRouteExitStep).GetInterfaces();

		Assert.IsTrue(linearInterfaces.Contains(typeof(IVehicleRouteStep)));
		Assert.IsTrue(linearInterfaces.Contains(typeof(ILinearRoutePathStep)));
		Assert.IsTrue(exitInterfaces.Contains(typeof(IVehicleRouteStep)));
		Assert.IsTrue(exitInterfaces.Contains(typeof(IExitTraversalPathStep)));
	}

	[TestMethod]
	public void JourneyState_ContainsLockedLifecycleInOrder()
	{
		var states = Enum.GetValues<VehicleJourneyState>();

		CollectionAssert.AreEqual(new[]
		{
			VehicleJourneyState.Scheduled,
			VehicleJourneyState.Boarding,
			VehicleJourneyState.Held,
			VehicleJourneyState.Departing,
			VehicleJourneyState.EnRoute,
			VehicleJourneyState.Dwelling,
			VehicleJourneyState.Arrived,
			VehicleJourneyState.Cancelled,
			VehicleJourneyState.Faulted
		}, states);
	}

	private sealed class MovementProfilePrototypeStub : IVehicleMovementProfilePrototype
	{
		public string Name => "movement";
		public long Id => 1;
		public string FrameworkItemType => "MovementProfilePrototypeStub";
		public VehicleMovementProfileType MovementType => VehicleMovementProfileType.CellExit;
		public VehicleMovementEnvironment MovementEnvironment => VehicleMovementEnvironment.Unrestricted;
		public bool ExposesOccupantsToWater => false;
		public bool IsDefault => true;
		public double RequiredPowerSpikeInWatts => 0.0;
		public long? FuelLiquidId => null;
		public double FuelVolumePerMove => 0.0;
		public string RequiredInstalledRole => string.Empty;
		public bool RequiresTowLinksClosed => false;
		public bool RequiresAccessPointsClosed => false;
		public IEnumerable<IVehiclePropulsionProfilePrototype> PropulsionProfiles =>
			Array.Empty<IVehiclePropulsionProfilePrototype>();
	}

	private sealed class CompartmentPrototypeStub : IVehicleCompartmentPrototype
	{
		public string Name => "compartment";
		public long Id => 1;
		public string FrameworkItemType => "CompartmentPrototypeStub";
		public string Description => "A compartment.";
		public int DisplayOrder => 0;
	}
}
