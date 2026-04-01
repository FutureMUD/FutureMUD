#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ThermalSourceComponentTests
{
	[TestMethod]
	public void ThermalSourceTemperatureModel_UsesIndoorAmbientRulesAndProximity()
	{
		var heat = new Mock<IProduceHeat>();
		heat.SetupGet(x => x.CurrentAmbientHeat).Returns(10.0);
		heat.Setup(x => x.CurrentHeat(Proximity.Immediate)).Returns(4.0);
		heat.Setup(x => x.CurrentHeat(Proximity.VeryDistant)).Returns(1.0);

		var sameLayerItem = CreateHeatItem(1L, heat.Object);
		var otherLayerItem = CreateHeatItem(2L, heat.Object);
		var target = new Mock<IPerceiver>();
		target.Setup(x => x.GetProximity(sameLayerItem.Object)).Returns(Proximity.Immediate);
		target.Setup(x => x.GetProximity(otherLayerItem.Object)).Returns(Proximity.VeryDistant);

		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.GameItems).Returns([sameLayerItem.Object, otherLayerItem.Object]);
		cell.SetupGet(x => x.Characters).Returns(Enumerable.Empty<ICharacter>());

		Assert.AreEqual(20.0, ThermalSourceTemperatureModel.AmbientHeatForCell(cell.Object, CellOutdoorsType.Indoors), 0.0001);
		Assert.AreEqual(10.0, ThermalSourceTemperatureModel.AmbientHeatForCell(cell.Object, CellOutdoorsType.IndoorsClimateExposed), 0.0001);
		Assert.AreEqual(0.0, ThermalSourceTemperatureModel.AmbientHeatForCell(cell.Object, CellOutdoorsType.Outdoors), 0.0001);
		Assert.AreEqual(5.0, ThermalSourceTemperatureModel.ProximityHeatForTarget(cell.Object, target.Object), 0.0001);
	}

	[TestMethod]
	public void ElectricHeaterCooler_OnlyProducesHeatWhenPoweredAndSwitchedOn()
	{
		var gameworld = CreateGameworld().Object;
		var parent = CreateParent(gameworld).Object;
		var proto = CreateElectricProto(ambient: 6.0, intimate: 9.0, wattage: 75.0);
		var component = new ElectricHeaterCoolerGameItemComponent(proto, parent, true);
		SetSwitchableState(component, true);

		Assert.AreEqual(0.0, component.CurrentAmbientHeat, 0.0001);

		component.OnPowerCutIn();

		Assert.AreEqual(6.0, component.CurrentAmbientHeat, 0.0001);
		Assert.AreEqual(9.0, component.CurrentHeat(Proximity.Intimate), 0.0001);
		Assert.AreEqual(75.0, component.PowerConsumptionInWatts, 0.0001);

		component.OnPowerCutOut();

		Assert.AreEqual(0.0, component.CurrentAmbientHeat, 0.0001);
	}

	[TestMethod]
	public void FuelHeaterCooler_LiquidModeConsumesMatchingLiquidOnly()
	{
		var gameworld = CreateGameworld().Object;
		var fuel = CreateLiquid("lamp oil");
		var mixture = new LiquidMixture(fuel.Object, 5.0, gameworld);
		var container = new Mock<ILiquidContainer>();
		container.SetupGet(x => x.LiquidMixture).Returns(mixture);
		container.Setup(x => x.RemoveLiquidAmount(2.0, null, "burn")).Returns(mixture);

		var sourceParent = new Mock<IGameItem>();
		sourceParent.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container.Object);

		var sourceConnectable = new Mock<IConnectable>();
		sourceConnectable.SetupGet(x => x.Parent).Returns(sourceParent.Object);

		var proto = CreateFuelProto(FuelHeaterCoolerFuelMedium.Liquid, ambient: 8.0, burnRate: 2.0, liquidFuel: fuel.Object);
		var component = new FuelHeaterCoolerGameItemComponent(proto, CreateParent(gameworld).Object, true);
		SetSwitchableState(component, true);
		component.RawConnect(sourceConnectable.Object, proto.Connector);

		Assert.AreEqual(8.0, component.CurrentAmbientHeat, 0.0001);

		component.BurnFuel(1.0);

		container.Verify(x => x.RemoveLiquidAmount(2.0, null, "burn"), Times.Once);
	}

	[TestMethod]
	public void FuelHeaterCooler_GasModeOnlyProducesHeatForMatchingGas()
	{
		var gameworld = CreateGameworld().Object;
		var validGas = CreateGas("propane", matches: true);
		var invalidGas = CreateGas("steam", matches: false);
		var supply = new Mock<IGasSupply>();
		supply.SetupGet(x => x.Gas).Returns(validGas.Object);
		supply.Setup(x => x.ConsumeGas(3.0)).Returns(true);

		var sourceParent = new Mock<IGameItem>();
		sourceParent.Setup(x => x.GetItemType<IGasSupply>()).Returns(supply.Object);

		var sourceConnectable = new Mock<IConnectable>();
		sourceConnectable.SetupGet(x => x.Parent).Returns(sourceParent.Object);

		var proto = CreateFuelProto(FuelHeaterCoolerFuelMedium.Gas, ambient: 7.0, burnRate: 3.0, gasFuel: validGas.Object);
		var component = new FuelHeaterCoolerGameItemComponent(proto, CreateParent(gameworld).Object, true);
		SetSwitchableState(component, true);
		component.RawConnect(sourceConnectable.Object, proto.Connector);

		Assert.AreEqual(7.0, component.CurrentAmbientHeat, 0.0001);
		component.BurnFuel(1.0);
		supply.Verify(x => x.ConsumeGas(3.0), Times.Once);

		supply.SetupGet(x => x.Gas).Returns(invalidGas.Object);
		Assert.AreEqual(0.0, component.CurrentAmbientHeat, 0.0001);
	}

	[TestMethod]
	public void ConsumableHeaterCooler_DeletesOrReplacesWhenSpent()
	{
		var gameworld = CreateGameworld().Object;
		var deletingParent = CreateParent(gameworld);
		var deleteProto = CreateConsumableProto(ambient: 5.0, seconds: 2, spentProto: null);
		var deletingComponent = new ConsumableHeaterCoolerGameItemComponent(deleteProto, deletingParent.Object, true);

		deletingComponent.AdvanceSeconds(2);
		deletingParent.Verify(x => x.Delete(), Times.Once);

		var replacementParent = CreateParent(gameworld);
		var replacementItem = new Mock<IGameItem>();
		replacementItem.SetupProperty(x => x.RoomLayer);
		var spentProto = new Mock<IGameItemProto>();
		spentProto.Setup(x => x.CreateNew(null)).Returns(replacementItem.Object);
		var replaceProto = CreateConsumableProto(ambient: 5.0, seconds: 2, spentProto: spentProto.Object);
		var replacingComponent = new ConsumableHeaterCoolerGameItemComponent(replaceProto, replacementParent.Object, true);

		replacingComponent.AdvanceSeconds(2);

		spentProto.Verify(x => x.CreateNew(null), Times.Once);
		replacementParent.Verify(x => x.Delete(), Times.Once);
		replacementItem.Verify(x => x.Login(), Times.Once);
	}

	[TestMethod]
	public void SolidFuelHeaterCooler_OnlyAcceptsTaggedFuelAndBurnsSequentially()
	{
		var gameworld = CreateGameworld().Object;
		var tag = new Mock<ITag>();
		var proto = CreateSolidFuelProto(tag.Object, ambient: 4.0, maxWeight: 5.0, secondsPerWeight: 2.0);
		var component = new SolidFuelHeaterCoolerGameItemComponent(proto, CreateParent(gameworld).Object, true);
		SetSwitchableState(component, true);

		var fuelOne = CreateFuelItem(11L, weight: 1.0, tag.Object, isFuel: true);
		var fuelTwo = CreateFuelItem(12L, weight: 1.0, tag.Object, isFuel: true);
		var rubbish = CreateFuelItem(13L, weight: 1.0, tag.Object, isFuel: false);

		Assert.AreEqual(WhyCannotPutReason.NotCorrectItemType, component.WhyCannotPut(rubbish.Object));

		component.Put(null, fuelOne.Object);
		component.Put(null, fuelTwo.Object);
		component.BurnFuel(1.0);
		component.BurnFuel(1.0);

		Assert.AreEqual(1, component.Contents.Count());
		fuelOne.Verify(x => x.Delete(), Times.Once);
		Assert.AreEqual(4.0, component.CurrentAmbientHeat, 0.0001);
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(new Mock<IHeartbeatManager>().Object);
		gameworld.SetupGet(x => x.EffectScheduler).Returns(new Mock<IEffectScheduler>().Object);
		return gameworld;
	}

	private static Mock<IGameItem> CreateParent(IFuturemud gameworld)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		parent.SetupGet(x => x.TrueLocations).Returns(Enumerable.Empty<ICell>());
		parent.SetupGet(x => x.Effects).Returns(Enumerable.Empty<IEffect>());
		parent.SetupGet(x => x.Components).Returns(Enumerable.Empty<IGameItemComponent>());
		parent.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		parent.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<OutputRange>()));
		parent.Setup(x => x.Handle(It.IsAny<IOutput>(), It.IsAny<OutputRange>()));
		parent.Setup(x => x.Delete());
		parent.Setup(x => x.GetItemType<IProducePower>()).Returns((IProducePower?)null);
		return parent;
	}

	private static Mock<IGameItem> CreateHeatItem(long id, IProduceHeat heat)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.DeepItems).Returns([item.Object]);
		item.Setup(x => x.GetItemTypes<IProduceHeat>()).Returns([heat]);
		return item;
	}

	private static Mock<IGameItem> CreateFuelItem(long id, double weight, ITag tag, bool isFuel)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Weight).Returns(weight);
		item.Setup(x => x.IsA(tag)).Returns(isFuel);
		item.SetupProperty(x => x.ContainedIn);
		item.Setup(x => x.Delete());
		return item;
	}

	private static Mock<ILiquid> CreateLiquid(string name)
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Name).Returns(name);
		liquid.Setup(x => x.LiquidCountsAs(It.IsAny<ILiquid>())).Returns<ILiquid>(other => ReferenceEquals(other, liquid.Object));
		liquid.Setup(x => x.LiquidCountsAsQuality(It.IsAny<ILiquid>())).Returns(ItemQuality.Standard);
		return liquid;
	}

	private static Mock<IGas> CreateGas(string name, bool matches)
	{
		var gas = new Mock<IGas>();
		gas.SetupGet(x => x.Name).Returns(name);
		gas.Setup(x => x.CountsAs(It.IsAny<IFluid>())).Returns(matches);
		return gas;
	}

	private static ElectricHeaterCoolerGameItemComponentProto CreateElectricProto(double ambient, double intimate, double wattage)
	{
		var proto = (ElectricHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(ElectricHeaterCoolerGameItemComponentProto));
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.AmbientHeat), ambient);
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.IntimateHeat), intimate);
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.ActiveDescriptionAddendum), "active");
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.InactiveDescriptionAddendum), "inactive");
		SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOnEmote), "@ hum|hums");
		SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOffEmote), "@ stop|stops");
		SetProperty(proto, nameof(ElectricHeaterCoolerGameItemComponentProto.Wattage), wattage);
		return proto;
	}

	private static FuelHeaterCoolerGameItemComponentProto CreateFuelProto(FuelHeaterCoolerFuelMedium medium, double ambient,
		double burnRate, ILiquid? liquidFuel = null, IGas? gasFuel = null)
	{
		var proto = (FuelHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(FuelHeaterCoolerGameItemComponentProto));
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.AmbientHeat), ambient);
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.ActiveDescriptionAddendum), "active");
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.InactiveDescriptionAddendum), "inactive");
		SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOnEmote), "@ on");
		SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOffEmote), "@ off");
		SetProperty(proto, nameof(FuelHeaterCoolerGameItemComponentProto.FuelMedium), medium);
		SetProperty(proto, nameof(FuelHeaterCoolerGameItemComponentProto.FuelPerSecond), burnRate);
		SetProperty(proto, nameof(FuelHeaterCoolerGameItemComponentProto.Connector), new ConnectorType(MudSharp.Form.Shape.Gender.Male, medium == FuelHeaterCoolerFuelMedium.Gas ? "GasLine" : "LiquidLine", false));
		SetProperty(proto, nameof(FuelHeaterCoolerGameItemComponentProto.LiquidFuel), liquidFuel);
		SetProperty(proto, nameof(FuelHeaterCoolerGameItemComponentProto.GasFuel), gasFuel);
		return proto;
	}

	private static ConsumableHeaterCoolerGameItemComponentProto CreateConsumableProto(double ambient, int seconds, IGameItemProto? spentProto)
	{
		var proto = (ConsumableHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(ConsumableHeaterCoolerGameItemComponentProto));
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.AmbientHeat), ambient);
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.ActiveDescriptionAddendum), "active");
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.InactiveDescriptionAddendum), "inactive");
		SetProperty(proto, nameof(ConsumableHeaterCoolerGameItemComponentProto.SecondsOfFuel), seconds);
		SetProperty(proto, nameof(ConsumableHeaterCoolerGameItemComponentProto.SpentItemProto), spentProto);
		SetProperty(proto, nameof(ConsumableHeaterCoolerGameItemComponentProto.FuelExpendedEcho), "$0 expire|expires");
		return proto;
	}

	private static SolidFuelHeaterCoolerGameItemComponentProto CreateSolidFuelProto(ITag tag, double ambient, double maxWeight,
		double secondsPerWeight)
	{
		var proto = (SolidFuelHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(SolidFuelHeaterCoolerGameItemComponentProto));
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.AmbientHeat), ambient);
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.ActiveDescriptionAddendum), "active");
		SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.InactiveDescriptionAddendum), "inactive");
		SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOnEmote), "@ on");
		SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOffEmote), "@ off");
		SetProperty(proto, nameof(SolidFuelHeaterCoolerGameItemComponentProto.FuelTag), tag);
		SetProperty(proto, nameof(SolidFuelHeaterCoolerGameItemComponentProto.MaximumFuelWeight), maxWeight);
		SetProperty(proto, nameof(SolidFuelHeaterCoolerGameItemComponentProto.SecondsPerUnitWeight), secondsPerWeight);
		return proto;
	}

	private static void SetSwitchableState(object component, bool value)
	{
		var field = typeof(SwitchableThermalSourceGameItemComponent)
			.GetField("_switchedOn", BindingFlags.Instance | BindingFlags.NonPublic)!;
		field.SetValue(component, value);
	}

	private static void SetProperty(object target, string propertyName, object? value)
	{
		var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
		property.SetValue(target, value);
	}
}
