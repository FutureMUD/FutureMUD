#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleProjectionRecoveryTests
{
	[TestMethod]
	public void LocalItemTargets_IncludeVehicleProjectionItemsWithoutCellPlacement()
	{
		var projection = new Mock<IGameItem>();
		var provider = new Mock<IProvideItemTargetProjections>();
		provider.SetupGet(x => x.TargetProjections).Returns([projection.Object]);
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Components).Returns([provider.Object]);

		var targets = MudSharp.Character.Character.IncludeTargetProjections([exterior.Object]).ToList();

		CollectionAssert.AreEqual(
			new List<IGameItem> { exterior.Object, projection.Object },
			targets);
	}

	[TestMethod]
	public void SpatiallyHostedProjection_InheritsExteriorSpatialLocation()
	{
		var cell = new Mock<MudSharp.Construction.ICell>();
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Location).Returns(cell.Object);
		exterior.SetupGet(x => x.RoomLayer).Returns(MudSharp.Construction.RoomLayer.InTrees);
		exterior.SetupGet(x => x.RoutePositionMetres).Returns(7_150.0);
		exterior.SetupGet(x => x.LocationLevelPerceivable).Returns(exterior.Object);
		exterior.Setup(x => x.TrueLocationsExcept(It.IsAny<List<IGameItem>>())).Returns([cell.Object]);
		var component = new Mock<IGameItemComponent>();
		component.As<IProvideItemSpatialHost>()
		         .SetupGet(x => x.SpatialHost)
		         .Returns(exterior.Object);
		var projection = (GameItem)RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
		typeof(GameItem)
			.GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(projection, new List<IGameItemComponent> { component.Object });

		Assert.AreSame(cell.Object, projection.Location);
		Assert.AreEqual(MudSharp.Construction.RoomLayer.InTrees, projection.RoomLayer);
		Assert.AreEqual(7_150.0, projection.RoutePositionMetres);
		Assert.AreSame(exterior.Object, projection.LocationLevelPerceivable);
		CollectionAssert.AreEqual(new List<MudSharp.Construction.ICell> { cell.Object }, projection.TrueLocations.ToList());
	}

	[TestMethod]
	public void ProjectionRecovery_CreatesMissingAccessAndCargoItemsOnce()
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(42L);
		vehicle.SetupGet(x => x.Name).Returns("Test Train");
		vehicle.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var exteriorComponent = new Mock<IVehicleExterior>();
		exteriorComponent.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		var exterior = new Mock<IGameItem>();
		exterior.Setup(x => x.GetItemType<IVehicleExterior>()).Returns(exteriorComponent.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);

		IGameItem? accessProjection = null;
		var accessItem = new Mock<IGameItem>();
		accessItem.SetupGet(x => x.Id).Returns(901L);
		var accessComponent = new Mock<IVehicleAccessPointItem>();
		accessItem.Setup(x => x.GetItemType<IVehicleAccessPointItem>()).Returns(accessComponent.Object);
		var accessItemPrototype = new Mock<IGameItemProto>();
		accessItemPrototype.Setup(x => x.IsItemType<IVehicleAccessPointItemPrototype>()).Returns(true);
		accessItemPrototype.Setup(x => x.CreateNew((ICharacter?)null)).Returns(accessItem.Object);
		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		accessPrototype.SetupGet(x => x.ProjectionItemPrototype).Returns(accessItemPrototype.Object);
		var access = new Mock<IVehicleAccessPoint>();
		access.SetupGet(x => x.Id).Returns(101L);
		access.SetupGet(x => x.Name).Returns("Passenger Doors");
		access.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		access.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		access.SetupGet(x => x.ProjectionItem).Returns(() => accessProjection!);
		access.SetupGet(x => x.ProjectionItemId).Returns(() => accessProjection?.Id);
		access.Setup(x => x.LinkProjectionItem(It.IsAny<IGameItem>()))
			.Callback((IGameItem item) => accessProjection = item);

		IGameItem? cargoProjection = null;
		var cargoItem = new Mock<IGameItem>();
		cargoItem.SetupGet(x => x.Id).Returns(902L);
		var cargoComponent = new Mock<IVehicleCargoSpaceItem>();
		cargoItem.Setup(x => x.GetItemType<IVehicleCargoSpaceItem>()).Returns(cargoComponent.Object);
		var cargoItemPrototype = new Mock<IGameItemProto>();
		cargoItemPrototype.Setup(x => x.IsItemType<IVehicleCargoSpaceItemPrototype>()).Returns(true);
		cargoItemPrototype.Setup(x => x.IsItemType<IContainerPrototype>()).Returns(true);
		cargoItemPrototype.Setup(x => x.CreateNew((ICharacter?)null)).Returns(cargoItem.Object);
		var cargoPrototype = new Mock<IVehicleCargoSpacePrototype>();
		cargoPrototype.SetupGet(x => x.ProjectionItemPrototype).Returns(cargoItemPrototype.Object);
		var cargo = new Mock<IVehicleCargoSpace>();
		cargo.SetupGet(x => x.Id).Returns(102L);
		cargo.SetupGet(x => x.Name).Returns("Baggage Hold");
		cargo.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		cargo.SetupGet(x => x.Prototype).Returns(cargoPrototype.Object);
		cargo.SetupGet(x => x.ProjectionItem).Returns(() => cargoProjection!);
		cargo.SetupGet(x => x.ProjectionItemId).Returns(() => cargoProjection?.Id);
		cargo.Setup(x => x.LinkProjectionItem(It.IsAny<IGameItem>()))
			.Callback((IGameItem item) => cargoProjection = item);

		vehicle.SetupGet(x => x.AccessPoints).Returns([access.Object]);
		vehicle.SetupGet(x => x.CargoSpaces).Returns([cargo.Object]);
		var service = new VehicleFleetOperationsService(
			new Mock<IVehicleOperationalReadinessService>().Object,
			new Mock<IVehicleHitchGraphService>().Object);

		var first = service.Recover(vehicle.Object, VehicleRecoveryMode.Projection, true);
		var second = service.Recover(vehicle.Object, VehicleRecoveryMode.Projection, true);

		Assert.AreEqual(2, first.Findings.Count);
		Assert.IsTrue(first.Findings.All(x => x.Action == VehicleRecoveryAction.Repaired));
		Assert.IsFalse(second.Findings.Any());
		Assert.AreSame(accessItem.Object, accessProjection);
		Assert.AreSame(cargoItem.Object, cargoProjection);
		accessItemPrototype.Verify(x => x.CreateNew((ICharacter?)null), Times.Once);
		cargoItemPrototype.Verify(x => x.CreateNew((ICharacter?)null), Times.Once);
		accessComponent.Verify(x => x.LinkAccessPoint(access.Object), Times.Once);
		cargoComponent.Verify(x => x.LinkCargoSpace(cargo.Object), Times.Once);
		access.Verify(x => x.LinkProjectionItem(accessItem.Object), Times.Once);
		cargo.Verify(x => x.LinkProjectionItem(cargoItem.Object), Times.Once);
		gameworld.Verify(x => x.Add(accessItem.Object), Times.Once);
		gameworld.Verify(x => x.Add(cargoItem.Object), Times.Once);
		saveManager.Verify(x => x.Flush(), Times.Exactly(4));
	}

	[TestMethod]
	public void AccessProjectionRecovery_MissingRequiredComponent_ReportsFailureWithoutRegisteringItem()
	{
		var gameworld = new Mock<IFuturemud>();
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(42L);
		vehicle.SetupGet(x => x.Name).Returns("Test Train");
		vehicle.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var exteriorComponent = new Mock<IVehicleExterior>();
		exteriorComponent.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		var exterior = new Mock<IGameItem>();
		exterior.Setup(x => x.GetItemType<IVehicleExterior>()).Returns(exteriorComponent.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		var invalidItem = new Mock<IGameItem>();
		invalidItem.Setup(x => x.GetItemType<IVehicleAccessPointItem>()).Returns((IVehicleAccessPointItem)null!);
		var itemPrototype = new Mock<IGameItemProto>();
		itemPrototype.Setup(x => x.CreateNew((ICharacter?)null)).Returns(invalidItem.Object);
		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		accessPrototype.SetupGet(x => x.ProjectionItemPrototype).Returns(itemPrototype.Object);
		var access = new Mock<IVehicleAccessPoint>();
		access.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		access.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		access.SetupGet(x => x.ProjectionItem).Returns((IGameItem)null!);
		vehicle.SetupGet(x => x.AccessPoints).Returns([access.Object]);
		vehicle.SetupGet(x => x.CargoSpaces).Returns([]);
		var service = new VehicleFleetOperationsService(
			new Mock<IVehicleOperationalReadinessService>().Object,
			new Mock<IVehicleHitchGraphService>().Object);

		var result = service.Recover(vehicle.Object, VehicleRecoveryMode.Projection, true);

		Assert.AreEqual(1, result.Findings.Count);
		Assert.AreEqual(VehicleRecoveryAction.Warning, result.Findings[0].Action);
		StringAssert.Contains(result.Findings[0].Hint, "has no vehicle access-point component");
		itemPrototype.Verify(x => x.CreateNew(It.IsAny<ICharacter?>()), Times.Never);
		gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
		access.Verify(x => x.LinkProjectionItem(It.IsAny<IGameItem>()), Times.Never);
	}
}
