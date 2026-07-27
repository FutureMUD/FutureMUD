#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleEngineInstallationTests
{
	[TestMethod]
	public void CanInstall_MismatchedEngineFormFactor_RejectsOtherwiseCompatibleModule()
	{
		var installation = CreateInstallation("motorcycle");
		var item = CreateEngineItem("motorcycle", "marine");

		var result = installation.CanInstall(new Mock<ICharacter>().Object, item.Object, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "marine");
		StringAssert.Contains(reason, "motorcycle");
	}

	[TestMethod]
	public void CanInstall_MatchingEngineFormFactor_AcceptsCompatibleModule()
	{
		var installation = CreateInstallation("motorcycle");
		var item = CreateEngineItem("motorcycle", "motorcycle");

		var result = installation.CanInstall(new Mock<ICharacter>().Object, item.Object, out var reason);

		Assert.IsTrue(result, reason);
	}

	private static VehicleInstallation CreateInstallation(string mountType)
	{
		var point = new Mock<IVehicleInstallationPointPrototype>();
		point.SetupGet(x => x.Id).Returns(10L);
		point.SetupGet(x => x.MountType).Returns(mountType);
		point.SetupGet(x => x.RequiredRole).Returns(string.Empty);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.InstallationPoints).Returns([point.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.Setup(x => x.IsDisabledByDamage(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
			.Returns(false);
		var dbitem = new MudSharp.Models.VehicleInstallation
		{
			Id = 20L,
			VehicleInstallationPointProtoId = point.Object.Id
		};
		return new VehicleInstallation(vehicle.Object, dbitem);
	}

	private static Mock<IGameItem> CreateEngineItem(string moduleMountType, string engineFormFactor)
	{
		var installable = new Mock<IVehicleInstallable>();
		installable.SetupGet(x => x.MountType).Returns(moduleMountType);
		installable.SetupGet(x => x.Role).Returns(string.Empty);
		var engine = new Mock<IVehicleEngine>();
		engine.SetupGet(x => x.FormFactor).Returns(engineFormFactor);
		var item = new Mock<IGameItem>();
		item.Setup(x => x.GetItemType<IVehicleInstallable>()).Returns(installable.Object);
		item.Setup(x => x.GetItemType<IVehicleEngine>()).Returns(engine.Object);
		return item;
	}
}
