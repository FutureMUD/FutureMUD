#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction.Grids;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Framework.Revision;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemGridLifecycleTests
{
    [TestMethod]
    public void GridPowerSupply_RetainsConfigurationWhileSuspendedAndRegistersOncePerActiveInterval()
    {
        var gameworld = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
        var parent = new Mock<IGameItem>();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        var component = new GridPowerSupplyGameItemComponent(CreatePrototype(gameworld.Object), parent.Object, true);
        var grid = new Mock<IElectricalGrid>();

        component.ElectricalGrid = grid.Object;
        grid.Verify(x => x.JoinGrid((IConsumePower)component), Times.Never);

        component.Login();
        component.Login();
        grid.Verify(x => x.JoinGrid((IConsumePower)component), Times.Once);
        Assert.AreSame(grid.Object, component.ElectricalGrid);

        component.Quit();
        component.Quit();
        grid.Verify(x => x.LeaveGrid((IConsumePower)component), Times.Once);
        Assert.AreSame(grid.Object, component.ElectricalGrid);

        component.Login();
        grid.Verify(x => x.JoinGrid((IConsumePower)component), Times.Exactly(2));
        component.Delete();
        grid.Verify(x => x.LeaveGrid((IConsumePower)component), Times.Exactly(2));
        Assert.IsNull(component.ElectricalGrid);
    }

    [TestMethod]
    public void LegacyProducerId_UsesComponentNamespaceEvenWhenAnItemIdCollides()
    {
        var world = new Mock<IFuturemud>();
        var items = new All<IGameItem>();
        world.SetupGet(x => x.Items).Returns(items);
        items.Add(ProducerItem(itemId: 10, componentId: 31, watts: 100));
        items.Add(ProducerItem(itemId: 31, componentId: 32, watts: 13));
        var grid = new ElectricalGrid(new MudSharp.Models.Grid
        {
            Id = 77,
            Definition = "<Definition><Producer>31</Producer></Definition>"
        }, world.Object);

        grid.LoadTimeInitialise();
        grid.LoadTimeInitialise();

        Assert.AreEqual(100.0, grid.TotalSupply);

        static IGameItem ProducerItem(long itemId, long componentId, double watts)
        {
            var item = new Mock<IGameItem>();
            item.SetupGet(x => x.Id).Returns(itemId);
            var producer = new Mock<IProducePower>();
            producer.SetupGet(x => x.Id).Returns(componentId);
            producer.SetupGet(x => x.MaximumPowerInWatts).Returns(watts);
            producer.SetupGet(x => x.Parent).Returns(item.Object);
            item.Setup(x => x.GetItemTypes<IProducePower>()).Returns([producer.Object]);
            return item.Object;
        }
    }

    private static GridPowerSupplyGameItemComponentProto CreatePrototype(IFuturemud gameworld)
    {
        var model = new MudSharp.Models.GameItemComponentProto
        {
            Id = 1,
            Name = "Grid Power Supply",
            Description = "Lifecycle test",
            RevisionNumber = 1,
            Definition = "<Definition><Wattage>50</Wattage></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (GridPowerSupplyGameItemComponentProto)typeof(GridPowerSupplyGameItemComponentProto)
            .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
            .Invoke([model, gameworld]);
    }
}
