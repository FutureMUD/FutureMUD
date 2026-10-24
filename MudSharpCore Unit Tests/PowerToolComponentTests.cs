#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponent = MudSharp.Models.GameItemComponent;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PowerToolComponentTests
{
	[TestMethod]
	public void DurationAwarePowerContract_UsesWattsAndDurationAndPreservesCopyLoadPaths()
	{
		var gameworld = new Mock<IFuturemud>();
		var power = new Mock<IProducePower>();
		var toolTag = new Mock<ITag>();
		var parentTag = new Mock<ITag>();
		parentTag.Setup(x => x.IsA(toolTag.Object)).Returns(true);
		var parent = CreateParent(gameworld.Object, power.Object, parentTag.Object);
		var copyParent = CreateParent(gameworld.Object, power.Object, parentTag.Object);
		var proto = (PowerToolGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreatePrototype(), gameworld.Object)!;
		var component = (PowerToolGameItemComponent)proto.CreateNew(parent.Object, temporary: true);
		var usage = TimeSpan.FromMinutes(2);
		power.Setup(x => x.CanDrawdownSpike(800.0, usage)).Returns(true);

		component.OnPowerCutIn();

		Assert.IsTrue(component.CanUseTool(toolTag.Object, usage));
		component.UseTool(toolTag.Object, usage);
		Assert.IsTrue(parent.Object.Condition < 1.0);
		power.Verify(x => x.CanDrawdownSpike(800.0, usage), Times.Once);
		power.Verify(x => x.DrawdownSpike(800.0, usage), Times.Once);
		power.Verify(x => x.CanDrawdownSpike(It.IsAny<double>()), Times.Never);
		power.Verify(x => x.DrawdownSpike(It.IsAny<double>()), Times.Never);

		Assert.IsInstanceOfType(component.Copy(copyParent.Object, true), typeof(PowerToolGameItemComponent));
		Assert.IsInstanceOfType(proto.LoadComponent(
			new DbGameItemComponent { Id = 99, Definition = "<Definition />" }, copyParent.Object),
			typeof(PowerToolGameItemComponent));
	}

	[TestMethod]
	public void PowerBankDurationAwareSpike_EnforcesOutputAndDebitsWattHours()
	{
		var gameworld = new Mock<IFuturemud>();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var databasePrototype = CreatePrototype();
		databasePrototype.Type = "PowerBank";
		databasePrototype.Name = "PowerBank_USB_C_Standard";
		databasePrototype.Definition = "<Definition><Connectors /><CapacityInWattHours>40</CapacityInWattHours><MaximumInputInWatts>18</MaximumInputInWatts><MaximumOutputInWatts>18</MaximumOutputInWatts><ChargingEfficiency>0.9</ChargingEfficiency><InputConnectors /><OutputConnectors /></Definition>";
		var proto = (PowerBankGameItemComponentProto)new GameItemComponentManager()
			.GetProto(databasePrototype, gameworld.Object)!;
		var component = (PowerBankGameItemComponent)proto.CreateNew(parent.Object, temporary: true);

		Assert.IsTrue(component.CanDrawdownSpike(18.0, TimeSpan.FromHours(2)));
		Assert.IsTrue(component.DrawdownSpike(18.0, TimeSpan.FromHours(2)));
		Assert.AreEqual(4.0, component.WattHoursRemaining, 1.0e-9);
		Assert.IsFalse(component.CanDrawdownSpike(18.0, TimeSpan.FromMinutes(14)));
		Assert.IsFalse(component.CanDrawdownSpike(19.0, TimeSpan.FromSeconds(1)));
	}

	[TestMethod]
	[DataRow(typeof(PowerSupplyGameItemComponent), "_connectedPowerSource")]
	[DataRow(typeof(PowerSocketGameItemComponent), "_connectedPowerSource")]
	[DataRow(typeof(ElectricGridFeederGameItemComponent), "_connectedPowerSource")]
	[DataRow(typeof(TelecommunicationsGridFeederGameItemComponent), "_connectedPowerSource")]
	public void PowerProxy_PreservesDurationAndUpstreamRefusal(Type componentType, string sourceField)
	{
		var constructor = componentType.GetConstructors().Single(x => x.GetParameters().Length == 3 &&
			x.GetParameters()[0].ParameterType.Name.EndsWith("Proto", StringComparison.Ordinal));
		var proxy = (IProducePower)constructor.Invoke([null!, Mock.Of<IGameItem>(), true]);
		var source = new Mock<IProducePower>();
		var duration = TimeSpan.FromMinutes(2);
		componentType.GetField(sourceField, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(proxy, source.Object);
		source.Setup(x => x.CanDrawdownSpike(10.0, duration)).Returns(true);
		source.Setup(x => x.DrawdownSpike(10.0, duration)).Returns(true);
		Assert.IsTrue(proxy.CanDrawdownSpike(10.0, duration));
		Assert.IsTrue(proxy.DrawdownSpike(10.0, duration));
		source.Setup(x => x.CanDrawdownSpike(10.0, duration)).Returns(false);
		source.Setup(x => x.DrawdownSpike(10.0, duration)).Returns(false);
		Assert.IsFalse(proxy.CanDrawdownSpike(10.0, duration));
		Assert.IsFalse(proxy.DrawdownSpike(10.0, duration));
		Assert.IsFalse(proxy.CanDrawdownSpike(10.0, TimeSpan.Zero));
		Assert.IsFalse(proxy.DrawdownSpike(double.NaN, duration));
		source.Verify(x => x.CanDrawdownSpike(It.IsAny<double>()), Times.Never);
		source.Verify(x => x.DrawdownSpike(It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	[DataRow("socket")]
	[DataRow("implant")]
	[DataRow("answering machine")]
	public void PowerSupplyThroughProxy_UsesFullDurationForFiniteBank(string kind)
	{
		var gameworld = new Mock<IFuturemud>();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var model = CreatePrototype(); model.Type = "PowerBank";
		model.Definition = "<Definition><Connectors/><CapacityInWattHours>1</CapacityInWattHours><MaximumInputInWatts>18</MaximumInputInWatts><MaximumOutputInWatts>18</MaximumOutputInWatts><ChargingEfficiency>0.9</ChargingEfficiency><InputConnectors/><OutputConnectors/></Definition>";
		var proto = (PowerBankGameItemComponentProto)new GameItemComponentManager().GetProto(model, gameworld.Object)!;
		var bank = (PowerBankGameItemComponent)proto.CreateNew(parent.Object, temporary: true);
		IProducePower proxy;
		if (kind == "implant")
		{
			var plant = new ImplantPowerPlantGameItemComponent(proto: null!, parent: parent.Object, temporary: true);
			typeof(ImplantPowerPlantGameItemComponent).GetField("_parentProducePower", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(plant, bank);
			var implantSupply = new ImplantPowerSupplyGameItemComponent(proto: null!, parent: parent.Object, temporary: true);
			typeof(ImplantPowerSupplyGameItemComponent).GetField("_powerPlant", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(implantSupply, plant);
			proxy = implantSupply;
		}
		else if (kind == "answering machine")
		{
			var machineModel = CreatePrototype(); machineModel.Type = "AnsweringMachine"; machineModel.Definition = "<Definition/>";
			var machineProto = (AnsweringMachineGameItemComponentProto)new GameItemComponentManager().GetProto(machineModel, gameworld.Object)!;
			var machine = (AnsweringMachineGameItemComponent)machineProto.CreateNew(parent.Object, temporary: true);
			typeof(AnsweringMachineGameItemComponent).GetField("_connectedPowerSource", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(machine, bank);
			Assert.IsFalse(((IProducePower)machine).CanDrawdownSpike(10, TimeSpan.FromMinutes(2)));
			typeof(AnsweringMachineGameItemComponent).GetField("_powered", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(machine, true);
			proxy = machine;
		}
		else
		{
			proxy = new PowerSocketGameItemComponent(proto: null!, parent: parent.Object, temporary: true);
			typeof(PowerSocketGameItemComponent).GetField("_connectedPowerSource", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(proxy, bank);
		}
		IProducePower supply = new PowerSupplyGameItemComponent(proto: null!, parent: parent.Object, temporary: true);
		typeof(PowerSupplyGameItemComponent).GetField("_connectedPowerSource", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(supply, proxy);
		Assert.IsTrue(supply.CanDrawdownSpike(10, TimeSpan.FromMinutes(2)));
		Assert.IsTrue(supply.DrawdownSpike(10, TimeSpan.FromMinutes(2)));
		Assert.AreEqual(2.0 / 3.0, bank.WattHoursRemaining, 1e-9);
		Assert.IsFalse(supply.CanDrawdownSpike(10, TimeSpan.FromMinutes(5)));
		Assert.IsFalse(supply.DrawdownSpike(10, TimeSpan.FromMinutes(5)));
		Assert.AreEqual(2.0 / 3.0, bank.WattHoursRemaining, 1e-9);
	}

	[TestMethod]
	public void ImplantRouter_PreservesDurationWhenSelectingFinitePlant()
	{
		var router = new ImplantPowerRouterGameItemComponent(proto: null!, parent: Mock.Of<IGameItem>(), temporary: true);
		var first = new Mock<IImplantPowerPlant>(); var second = new Mock<IImplantPowerPlant>();
		var plants = (List<IImplantPowerPlant>)typeof(ImplantPowerRouterGameItemComponent)
			.GetField("_connectedPowerPlants", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(router)!;
		plants.AddRange([first.Object, second.Object]);
		var duration = TimeSpan.FromMinutes(2);
		second.Setup(x => x.CanDrawdownSpike(10, duration)).Returns(true);
		second.Setup(x => x.DrawdownSpike(10, duration)).Returns(true);
		Assert.IsTrue(((IImplantPowerPlant)router).CanDrawdownSpike(10, duration));
		Assert.IsTrue(((IImplantPowerPlant)router).DrawdownSpike(10, duration));
		first.Verify(x => x.DrawdownSpike(It.IsAny<double>(), It.IsAny<TimeSpan>()), Times.Never);
		second.Verify(x => x.DrawdownSpike(10, duration), Times.Once);
	}

	private static Mock<IGameItem> CreateParent(IFuturemud gameworld, IProducePower power, ITag tag)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		parent.SetupGet(x => x.Tags).Returns(new[] { tag });
		parent.SetupProperty(x => x.Quality, ItemQuality.Standard);
		parent.SetupProperty(x => x.Condition, 1.0);
		parent.Setup(x => x.GetItemType<IProducePower>()).Returns(power);
		return parent;
	}

	private static DbGameItemComponentProto CreatePrototype()
	{
		return new DbGameItemComponentProto
		{
			Id = 7001,
			RevisionNumber = 1,
			Type = "PowerTool",
			Name = "PowerTool_Workshop",
			Description = "Workshop power tool",
			Definition = "<Definition><Wattage>800</Wattage><MultiplierReductionPerQuality>0.05</MultiplierReductionPerQuality><BaseMultiplier>1.0</BaseMultiplier><ToolDurabilitySecondsExpression>(1+quality) * 7200</ToolDurabilitySecondsExpression></Definition>",
			EditableItem = new DbEditableItem
			{
				Id = 7001,
				RevisionNumber = 1,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow
			}
		};
	}
}
