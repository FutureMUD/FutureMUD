#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Vehicles;
using System;
using System.Linq;
using System.Xml.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleComponentBuilderTests
{
	[TestMethod]
	public void ComponentEditNewTypeText_UsesWholeRemainderForMultiWordTypes()
	{
		Assert.AreEqual("vehicle installable", ItemBuilderModule.ComponentEditNewTypeText(new StringStack("vehicle installable")));
		Assert.AreEqual("vehicle installable", ItemBuilderModule.ComponentEditNewTypeText(new StringStack("\"vehicle installable\"")));
		Assert.AreEqual("vehicle access point", ItemBuilderModule.ComponentEditNewTypeText(new StringStack("vehicle access point")));
	}

	[TestMethod]
	public void ComponentTypeHelpLookupText_CollapsesSpacedVehicleInstallableInput()
	{
		Assert.AreEqual(
			ItemBuilderModule.ComponentTypeHelpLookupText("VehicleInstallable"),
			ItemBuilderModule.ComponentTypeHelpLookupText("vehicle installable"));
		Assert.AreEqual(
			ItemBuilderModule.ComponentTypeHelpLookupText("Vehicle_Cargo-Space"),
			ItemBuilderModule.ComponentTypeHelpLookupText("vehicle cargo space"));
	}

	[TestMethod]
	public void GameItemComponentManager_RegistersVehicleInstallableBuilderAndHelp()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var help = manager.TypeHelpInfo.FirstOrDefault(x => x.Name.EqualTo("VehicleInstallable")).Help;

		Assert.IsTrue(primaryTypes.Any(x => x.EqualTo("vehicle installable")));
		Assert.IsFalse(string.IsNullOrWhiteSpace(help));
		StringAssert.Contains(help, "mount <type>");
		StringAssert.Contains(help, "role <role>");
		StringAssert.Contains(help, "mincondition <percent>");
		StringAssert.Contains(help, "movementcondition <percent>");
		Assert.IsTrue(primaryTypes.Any(x => x.EqualTo("vehicle oar")));
		Assert.IsTrue(primaryTypes.Any(x => x.EqualTo("outboard motor")));
		Assert.IsFalse(string.IsNullOrWhiteSpace(manager.TypeHelpInfo
			.FirstOrDefault(x => x.Name.EqualTo("VehicleOar")).Help));
		Assert.IsFalse(string.IsNullOrWhiteSpace(manager.TypeHelpInfo
			.FirstOrDefault(x => x.Name.EqualTo("OutboardMotor")).Help));
	}

	[DataTestMethod]
	[DataRow("Vehicle Exterior", "vehicle <vehicle proto>")]
	[DataRow("Vehicle Access Point", "vehicle <vehicle proto> <access id>")]
	[DataRow("Vehicle Cargo Space", "vehicle <vehicle proto> <cargo id>")]
	[DataRow("Vehicle Installable", "mount <type>")]
	[DataRow("RidingGear", "role <role>")]
	[DataRow("HitchGear", "role <role>")]
	[DataRow("Vehicle Oar", "efficiency <multiplier>")]
	[DataRow("Outboard Motor", "output <multiplier>")]
	public void VehicleComponentProtos_SurfaceSpecificBuilderHelp(string componentType, string expectedHelp)
	{
		var gameworld = new Mock<IFuturemud>();
		var proto = new GameItemComponentManager().GetProto(CreateComponentProto(componentType), gameworld.Object) as GameItemComponentProto;

		Assert.IsNotNull(proto);
		StringAssert.Contains(proto!.ShowBuildingHelp, expectedHelp);
		Assert.AreNotEqual("This component does not yet have specific help.", proto.ShowBuildingHelp);
	}

	[TestMethod]
	public void VehicleInstallableComponentProto_LoadsConditionThresholds()
	{
		var gameworld = new Mock<IFuturemud>();
		var definition = new XElement("Definition",
			new XElement("MountType", "engine"),
			new XElement("Role", "drive"),
			new XElement("MinimumFunctionalCondition", 0.25),
			new XElement("MinimumMovementCondition", 0.60));

		var proto = new GameItemComponentManager().GetProto(
			CreateComponentProto("Vehicle Installable", definition.ToString(SaveOptions.DisableFormatting)),
			gameworld.Object) as VehicleInstallableGameItemComponentProto;

		Assert.IsNotNull(proto);
		Assert.AreEqual(0.25, proto!.MinimumFunctionalCondition);
		Assert.AreEqual(0.60, proto.MinimumMovementCondition);
	}

	[TestMethod]
	public void RidingAndHitchGearComponentProtos_LoadSemanticRoleDefinitions()
	{
		var gameworld = new Mock<IFuturemud>();
		var manager = new GameItemComponentManager();
		var ridingDefinition = new XElement("Definition",
			new XElement("Roles", RidingGearRole.Bridle | RidingGearRole.Reins | RidingGearRole.BitlessControl),
			new XElement("ControlBonus", 4.5),
			new XElement("StabilityBonus", 1.5));
		var hitchDefinition = new XElement("Definition",
			new XElement("Roles", HitchGearRole.Yoke | HitchGearRole.Traces),
			new XElement("MaximumUsers", 2),
			new XElement("EffortMultiplier", 2.25),
			new XElement("MaximumTowedWeight", 450.0));

		var riding = manager.GetProto(CreateComponentProto("RidingGear", ridingDefinition.ToString(SaveOptions.DisableFormatting)),
			gameworld.Object) as RidingGearGameItemComponentProto;
		var hitch = manager.GetProto(CreateComponentProto("HitchGear", hitchDefinition.ToString(SaveOptions.DisableFormatting)),
			gameworld.Object) as HitchGearGameItemComponentProto;

		Assert.IsNotNull(riding);
		Assert.IsTrue(riding!.Roles.HasFlag(RidingGearRole.BitlessControl));
		Assert.AreEqual(4.5, riding.ControlBonus);
		Assert.AreEqual(1.5, riding.StabilityBonus);
		Assert.IsNotNull(hitch);
		Assert.IsTrue(hitch!.Roles.HasFlag(HitchGearRole.Yoke));
		Assert.IsTrue(hitch.Roles.HasFlag(HitchGearRole.Traces));
		Assert.AreEqual(2, hitch.MaximumUsers);
		Assert.AreEqual(2.25, hitch.EffortMultiplier);
		Assert.AreEqual(450.0, hitch.MaximumTowedWeight);
	}

	[TestMethod]
	public void WaterPropulsionComponentProtos_LoadCompatibleXmlDefinitions()
	{
		var gameworld = new Mock<IFuturemud>();
		var manager = new GameItemComponentManager();
		var oarDefinition = new XElement("Definition",
			new XElement("EfficiencyMultiplier", 1.75));
		var motorDefinition = new XElement("Definition",
			new XElement("EnergySource", OutboardMotorEnergySource.Electric),
			new XElement("OutputMultiplier", 2.5),
			new XElement("RequiredPowerSpikeInWatts", 750.0));

		var oar = manager.GetProto(CreateComponentProto("Vehicle Oar",
			oarDefinition.ToString(SaveOptions.DisableFormatting)), gameworld.Object) as VehicleOarGameItemComponentProto;
		var motor = manager.GetProto(CreateComponentProto("Outboard Motor",
			motorDefinition.ToString(SaveOptions.DisableFormatting)), gameworld.Object) as OutboardMotorGameItemComponentProto;

		Assert.IsNotNull(oar);
		Assert.AreEqual(1.75, oar!.EfficiencyMultiplier);
		Assert.IsNotNull(motor);
		Assert.AreEqual(OutboardMotorEnergySource.Electric, motor!.EnergySource);
		Assert.AreEqual(2.5, motor.OutputMultiplier);
		Assert.AreEqual(750.0, motor.RequiredPowerSpikeInWatts);
	}

	private static DbGameItemComponentProto CreateComponentProto(string componentType, string definition = "<Definition />")
	{
		return new DbGameItemComponentProto
		{
			Id = 1L,
			Name = componentType,
			Description = $"Test {componentType}",
			Type = componentType,
			Definition = definition,
			RevisionNumber = 0,
			EditableItem = new DbEditableItem
			{
				Id = 1L,
				BuilderAccountId = 1L,
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current
			}
		};
	}
}
