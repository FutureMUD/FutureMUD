#nullable enable
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ThermalSourceComponentTests
{
    [TestMethod]
    public void ThermalSourceTemperatureModel_UsesIndoorAmbientRulesAndProximity()
    {
        Mock<IProduceHeat> heat = new();
        heat.SetupGet(x => x.CurrentAmbientHeat).Returns(10.0);
        heat.Setup(x => x.CurrentHeat(Proximity.Immediate)).Returns(4.0);
        heat.Setup(x => x.CurrentHeat(Proximity.VeryDistant)).Returns(1.0);

        Mock<IGameItem> sameLayerItem = CreateHeatItem(1L, heat.Object);
        Mock<IGameItem> otherLayerItem = CreateHeatItem(2L, heat.Object);
        Mock<IPerceiver> target = new();
        target.Setup(x => x.GetProximity(sameLayerItem.Object)).Returns(Proximity.Immediate);
        target.Setup(x => x.GetProximity(otherLayerItem.Object)).Returns(Proximity.VeryDistant);

        Mock<ICell> cell = new();
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
        IFuturemud gameworld = CreateGameworld().Object;
        IGameItem parent = CreateParent(gameworld).Object;
        ElectricHeaterCoolerGameItemComponentProto proto = CreateElectricProto(ambient: 6.0, intimate: 9.0, wattage: 75.0);
        ElectricHeaterCoolerGameItemComponent component = new(proto, parent, true);
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
        IFuturemud gameworld = CreateGameworld().Object;
        Mock<ILiquid> fuel = CreateLiquid("lamp oil");
        LiquidMixture mixture = new(fuel.Object, 5.0, gameworld);
        Mock<ILiquidContainer> container = new();
        container.SetupGet(x => x.LiquidMixture).Returns(mixture);
        container.Setup(x => x.RemoveLiquidAmount(2.0, null, "burn")).Returns(mixture);

        Mock<IGameItem> sourceParent = new();
        sourceParent.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container.Object);

        Mock<IConnectable> sourceConnectable = new();
        sourceConnectable.SetupGet(x => x.Parent).Returns(sourceParent.Object);

        FuelHeaterCoolerGameItemComponentProto proto = CreateFuelProto(FuelHeaterCoolerFuelMedium.Liquid, ambient: 8.0, burnRate: 2.0, liquidFuel: fuel.Object);
        FuelHeaterCoolerGameItemComponent component = new(proto, CreateParent(gameworld).Object, true);
        SetSwitchableState(component, true);
        component.RawConnect(sourceConnectable.Object, proto.Connector);

        Assert.AreEqual(8.0, component.CurrentAmbientHeat, 0.0001);

        component.BurnFuel(1.0);

        container.Verify(x => x.RemoveLiquidAmount(2.0, null, "burn"), Times.Once);
    }

    [TestMethod]
    public void FuelHeaterCooler_GasModeOnlyProducesHeatForMatchingGas()
    {
        IFuturemud gameworld = CreateGameworld().Object;
        Mock<IGas> validGas = CreateGas("propane", matches: true);
        Mock<IGas> invalidGas = CreateGas("steam", matches: false);
        Mock<IGasSupply> supply = new();
        supply.SetupGet(x => x.Gas).Returns(validGas.Object);
        supply.Setup(x => x.ConsumeGas(3.0)).Returns(true);

        Mock<IGameItem> sourceParent = new();
        sourceParent.Setup(x => x.GetItemType<IGasSupply>()).Returns(supply.Object);

        Mock<IConnectable> sourceConnectable = new();
        sourceConnectable.SetupGet(x => x.Parent).Returns(sourceParent.Object);

        FuelHeaterCoolerGameItemComponentProto proto = CreateFuelProto(FuelHeaterCoolerFuelMedium.Gas, ambient: 7.0, burnRate: 3.0, gasFuel: validGas.Object);
        FuelHeaterCoolerGameItemComponent component = new(proto, CreateParent(gameworld).Object, true);
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
        IFuturemud gameworld = CreateGameworld().Object;
        Mock<IGameItem> deletingParent = CreateParent(gameworld);
        ConsumableHeaterCoolerGameItemComponentProto deleteProto = CreateConsumableProto(ambient: 5.0, seconds: 2, spentProto: null);
        ConsumableHeaterCoolerGameItemComponent deletingComponent = new(deleteProto, deletingParent.Object, true);

        deletingComponent.AdvanceSeconds(2);
        deletingParent.Verify(x => x.Delete(), Times.Once);

        Mock<IGameItem> replacementParent = CreateParent(gameworld);
        Mock<IGameItem> replacementItem = new();
        replacementItem.SetupProperty(x => x.RoomLayer);
        Mock<IGameItemProto> spentProto = new();
        spentProto.Setup(x => x.CreateNew(null)).Returns(replacementItem.Object);
        ConsumableHeaterCoolerGameItemComponentProto replaceProto = CreateConsumableProto(ambient: 5.0, seconds: 2, spentProto: spentProto.Object);
        ConsumableHeaterCoolerGameItemComponent replacingComponent = new(replaceProto, replacementParent.Object, true);

        replacingComponent.AdvanceSeconds(2);

        spentProto.Verify(x => x.CreateNew(null), Times.Once);
        replacementParent.Verify(x => x.Delete(), Times.Once);
        replacementItem.Verify(x => x.Login(), Times.Once);
    }

    [TestMethod]
    public void SolidFuelHeaterCooler_OnlyAcceptsTaggedFuelAndBurnsSequentially()
    {
        IFuturemud gameworld = CreateGameworld().Object;
        Mock<ITag> tag = new();
        SolidFuelHeaterCoolerGameItemComponentProto proto = CreateSolidFuelProto(tag.Object, ambient: 4.0, maxWeight: 5.0, secondsPerWeight: 2.0);
        SolidFuelHeaterCoolerGameItemComponent component = new(proto, CreateParent(gameworld).Object, true);
        SetSwitchableState(component, true);

        Mock<IGameItem> fuelOne = CreateFuelItem(11L, weight: 1.0, tag.Object, isFuel: true);
        Mock<IGameItem> fuelTwo = CreateFuelItem(12L, weight: 1.0, tag.Object, isFuel: true);
        Mock<IGameItem> rubbish = CreateFuelItem(13L, weight: 1.0, tag.Object, isFuel: false);

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
        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.HeartbeatManager).Returns(new Mock<IHeartbeatManager>().Object);
        gameworld.SetupGet(x => x.EffectScheduler).Returns(new Mock<IEffectScheduler>().Object);
        return gameworld;
    }

    private static Mock<IGameItem> CreateParent(IFuturemud gameworld)
    {
        Mock<IGameItem> parent = new();
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
        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Id).Returns(id);
        item.SetupGet(x => x.DeepItems).Returns([item.Object]);
        item.Setup(x => x.GetItemTypes<IProduceHeat>()).Returns([heat]);
        return item;
    }

    private static Mock<IGameItem> CreateFuelItem(long id, double weight, ITag tag, bool isFuel)
    {
        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Id).Returns(id);
        item.SetupGet(x => x.Weight).Returns(weight);
        item.Setup(x => x.IsA(tag)).Returns(isFuel);
        item.SetupProperty(x => x.ContainedIn);
        item.Setup(x => x.Delete());
        return item;
    }

    private static Mock<ILiquid> CreateLiquid(string name)
    {
        Mock<ILiquid> liquid = new();
        liquid.SetupGet(x => x.Name).Returns(name);
        liquid.Setup(x => x.LiquidCountsAs(It.IsAny<ILiquid>())).Returns<ILiquid>(other => ReferenceEquals(other, liquid.Object));
        liquid.Setup(x => x.LiquidCountsAsQuality(It.IsAny<ILiquid>())).Returns(ItemQuality.Standard);
        return liquid;
    }

    private static Mock<IGas> CreateGas(string name, bool matches)
    {
        Mock<IGas> gas = new();
        gas.SetupGet(x => x.Name).Returns(name);
        gas.Setup(x => x.CountsAs(It.IsAny<IFluid>())).Returns(matches);
        return gas;
    }

    private static ElectricHeaterCoolerGameItemComponentProto CreateElectricProto(double ambient, double intimate, double wattage)
    {
        ElectricHeaterCoolerGameItemComponentProto proto = (ElectricHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(ElectricHeaterCoolerGameItemComponentProto));
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
        FuelHeaterCoolerGameItemComponentProto proto = (FuelHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(FuelHeaterCoolerGameItemComponentProto));
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
        ConsumableHeaterCoolerGameItemComponentProto proto = (ConsumableHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(ConsumableHeaterCoolerGameItemComponentProto));
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
        SolidFuelHeaterCoolerGameItemComponentProto proto = (SolidFuelHeaterCoolerGameItemComponentProto)FormatterServices.GetUninitializedObject(typeof(SolidFuelHeaterCoolerGameItemComponentProto));
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
        FieldInfo field = typeof(SwitchableThermalSourceGameItemComponent)
            .GetField("_switchedOn", BindingFlags.Instance | BindingFlags.NonPublic)!;
        field.SetValue(component, value);
    }

    private static void SetProperty(object target, string propertyName, object? value)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        property.SetValue(target, value);
    }
}
