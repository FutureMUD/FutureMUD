#nullable enable annotations

using MudSharp.Construction.Grids;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class GridPowerSupplyGameItemComponent : GameItemComponent, IProducePower, IConsumePower,
    ICanConnectToElectricalGrid
{
    protected GridPowerSupplyGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (GridPowerSupplyGameItemComponentProto)newProto;
    }

    private IElectricalGrid _grid;
    private bool _gridActive;

    public IElectricalGrid ElectricalGrid
    {
        get => _grid;
        set
        {
            if (ReferenceEquals(_grid, value)) return;
            if (_gridActive) _grid?.LeaveGrid((IConsumePower)this);
            _grid = value;
            if (_gridActive) _grid?.JoinGrid((IConsumePower)this);
            Changed = true;
        }
    }

    public override void Login()
    {
        base.Login();
        if (_gridActive) return;
        _gridActive = true;
        _grid?.JoinGrid((IConsumePower)this);
        _grid?.RecalculateGrid();
    }

    public override void Quit()
    {
        if (!_gridActive) return;
        _gridActive = false;
        if (_powered) OnPowerCutOut();
        _grid?.LeaveGrid((IConsumePower)this);
        base.Quit();
    }

    public override void Delete()
    {
        Quit();
        ElectricalGrid = null;
        base.Delete();
    }

    #region Constructors

    public GridPowerSupplyGameItemComponent(GridPowerSupplyGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public GridPowerSupplyGameItemComponent(MudSharp.Models.GameItemComponent component,
        GridPowerSupplyGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public GridPowerSupplyGameItemComponent(GridPowerSupplyGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    protected void LoadFromXml(XElement root)
    {
        XElement element = root.Element("Grid");
        if (element != null)
        {
            ElectricalGrid = Gameworld.Grids.Get(long.Parse(element.Value)) as IElectricalGrid;
        }
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new GridPowerSupplyGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition", new XElement("Grid", ElectricalGrid?.Id ?? 0)).ToString();
    }

    #endregion

    #region Implementation of IProducePower

    public bool PrimaryLoadTimePowerProducer => true;
    public bool PrimaryExternalConnectionPowerProducer => false;
    public double FuelLevel => 1.0;
    private readonly List<IConsumePower> _connectedConsumers = new();
    private readonly List<IConsumePower> _powerUsers = new();
    private bool _powered;

    public void BeginDrawdown(IConsumePower item)
    {
        if (!_connectedConsumers.Contains(item))
        {
            _connectedConsumers.Add(item);
        }

        if (ProducingPower && !_powerUsers.Contains(item))
        {
            _powerUsers.Add(item);
            item.OnPowerCutIn();
        }

        ElectricalGrid?.RecalculateGrid();
    }

    public void EndDrawdown(IConsumePower item)
    {
        _connectedConsumers.Remove(item);
        if (_powerUsers.Remove(item))
        {
            item.OnPowerCutOut();
        }
        ElectricalGrid?.RecalculateGrid();
    }

    public bool CanBeginDrawDown(double wattage)
    {
        return RegisteredDrawdown + wattage <= MaximumPowerInWatts;
    }

    public bool CanDrawdownSpike(double wattage)
    {
        return ProducingPower && CurrentDrawdown + wattage <= MaximumPowerInWatts;
    }

    public bool DrawdownSpike(double wattage)
    {
        return ProducingPower &&
               CurrentDrawdown + wattage <= MaximumPowerInWatts &&
               (ElectricalGrid?.DrawdownSpike(wattage) ?? false);
    }

    public double MaximumPowerInWatts => _prototype.Wattage *
                                         Parent.EffectsOfType<IMagicPowerOrFuelEnhancementEffect>(x =>
                                                   x.AppliesToPoweredItem(Parent))
                                               .Aggregate(1.0,
                                                   (current, effect) => current *
                                                                        effect.PowerProductionMultiplier);

    public bool ProducingPower => ElectricalGrid != null && _powered;

    private double CurrentDrawdown => _powerUsers.Sum(x => x.PowerConsumptionInWatts);
    private double RegisteredDrawdown => _connectedConsumers.Sum(x => x.PowerConsumptionInWatts);

    #endregion

    #region IConsumePower Implementation

    public double PowerConsumptionInWatts => CurrentDrawdown;

    public void OnPowerCutIn()
    {
        if (_powered) return;
        _powered = true;
        var newPowerUsers = _connectedConsumers.Where(x => !_powerUsers.Contains(x)).ToList();
        _powerUsers.AddRange(newPowerUsers);
        foreach (IConsumePower item in newPowerUsers)
        {
            item.OnPowerCutIn();
        }
    }

    public void OnPowerCutOut()
    {
        if (!_powered && _powerUsers.Count == 0) return;
        _powered = false;
        var previousPowerUsers = _powerUsers.ToList();
        _powerUsers.Clear();
        foreach (IConsumePower item in previousPowerUsers)
        {
            item.OnPowerCutOut();
        }
    }

    #endregion

    #region ICanConnectToGrid Implementation

    string ICanConnectToGrid.GridType => "Electrical";

    IGrid? ICanConnectToGrid.Grid
    {
        get => ElectricalGrid;
        set => ElectricalGrid = value as IElectricalGrid;
    }

    #endregion
}
