using System;
using System.Linq;
using System.Reflection;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

public static class ThermalDebug {
    public static void Main() {
        var gameworld = new Mock<IFuturemud>();
        gameworld.SetupGet(x => x.HeartbeatManager).Returns(new Mock<IHeartbeatManager>().Object);
        gameworld.SetupGet(x => x.EffectScheduler).Returns(new Mock<IEffectScheduler>().Object);

        var parent = new Mock<IGameItem>();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        parent.SetupGet(x => x.TrueLocations).Returns(Enumerable.Empty<MudSharp.Construction.ICell>());
        parent.SetupGet(x => x.Effects).Returns(Enumerable.Empty<MudSharp.Effects.IEffect>());
        parent.SetupGet(x => x.Components).Returns(Enumerable.Empty<IGameItemComponent>());
        parent.SetupGet(x => x.RoomLayer).Returns(MudSharp.Framework.RoomLayer.GroundLevel);
        parent.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<MudSharp.PerceptionEngine.OutputRange>()));
        parent.Setup(x => x.Handle(It.IsAny<MudSharp.PerceptionEngine.IOutput>(), It.IsAny<MudSharp.PerceptionEngine.OutputRange>()));
        parent.Setup(x => x.Delete());
        parent.Setup(x => x.GetItemType<IProducePower>()).Returns((IProducePower)null);

        var tag = new Mock<ITag>();
        var proto = (SolidFuelHeaterCoolerGameItemComponentProto)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(SolidFuelHeaterCoolerGameItemComponentProto));
        SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.AmbientHeat), 4.0);
        SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.ActiveDescriptionAddendum), "active");
        SetProperty(proto, nameof(ThermalSourceGameItemComponentProto.InactiveDescriptionAddendum), "inactive");
        SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOnEmote), "@ on");
        SetProperty(proto, nameof(SwitchableThermalSourceGameItemComponentProto.SwitchOffEmote), "@ off");
        SetProperty(proto, nameof(SolidFuelHeaterCoolerGameItemComponentProto.FuelTag), tag.Object);
        SetProperty(proto, nameof(SolidFuelHeaterCoolerGameItemComponentProto.MaximumFuelWeight), 5.0);
        SetProperty(proto, nameof(SolidFuelHeaterCoolerGameItemComponentProto.SecondsPerUnitWeight), 2.0);

        var component = new SolidFuelHeaterCoolerGameItemComponent(proto, parent.Object, true);
        typeof(SwitchableThermalSourceGameItemComponent).GetField("_switchedOn", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(component, true);

        var fuelOne = CreateFuelItem(11L, 1.0, tag.Object, true);
        var fuelTwo = CreateFuelItem(12L, 1.0, tag.Object, true);
        component.Put(null, fuelOne.Object);
        component.Put(null, fuelTwo.Object);
        Console.WriteLine($"Before burns count={component.Contents.Count()}");
        Dump(component);
        component.BurnFuel(1.0);
        Console.WriteLine($"After burn1 count={component.Contents.Count()}");
        Dump(component);
        component.BurnFuel(1.0);
        Console.WriteLine($"After burn2 count={component.Contents.Count()}");
        Dump(component);
    }

    static void Dump(SolidFuelHeaterCoolerGameItemComponent component){
        var t = typeof(SolidFuelHeaterCoolerGameItemComponent);
        var remaining = (double)t.GetField("_remainingBurnSeconds", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(component);
        var current = (IGameItem)t.GetField("_currentFuelItem", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(component);
        Console.WriteLine($"remaining={remaining}, current={(current==null?"null":current.Id.ToString())}, switched={component.SwitchedOn}");
    }

    static Mock<IGameItem> CreateFuelItem(long id, double weight, ITag tag, bool isFuel) {
        var item = new Mock<IGameItem>();
        item.SetupGet(x => x.Id).Returns(id);
        item.SetupGet(x => x.Weight).Returns(weight);
        item.Setup(x => x.IsA(tag)).Returns(isFuel);
        item.SetupProperty(x => x.ContainedIn);
        item.Setup(x => x.Delete());
        return item;
    }

    static void SetProperty(object target, string propertyName, object value) {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.SetValue(target, value);
    }
}
