using MudSharp.GameItems;

namespace MudSharp.Construction.Grids;

public class ElectricalGrid : GridBase, IElectricalGrid
{
    public ElectricalGrid(Models.Grid grid, IFuturemud gameworld) : base(grid, gameworld)
    {
        XElement root = XElement.Parse(grid.Definition);
        foreach (XElement element in root.Elements("Consumer"))
        {
            _connectedConsumerIds.Add(EndpointId.FromXml(element));
        }

        foreach (XElement element in root.Elements("Producer"))
        {
            _connectedProducerIds.Add(EndpointId.FromXml(element));
        }
    }

    public ElectricalGrid(IFuturemud gameworld, IRoom initialLocation) : base(gameworld, initialLocation)
    {
    }

    public ElectricalGrid(IElectricalGrid rhs) : base(rhs)
    {
    }

    protected override XElement SaveDefinition()
    {
        XElement @base = base.SaveDefinition();
        foreach (IConsumePower consumer in _connectedConsumers)
        {
            @base.Add(EndpointId.ToXml("Consumer", consumer));
        }

        foreach (IProducePower producer in _connectedProducers)
        {
            @base.Add(EndpointId.ToXml("Producer", producer));
        }

        @base.Add(_connectedConsumerIds.Select(x => x.ToXml("Consumer")));
        @base.Add(_connectedProducerIds.Select(x => x.ToXml("Producer")));
        return @base;
    }

    public override string GridType => "Electrical";

    public override void LoadTimeInitialise()
    {
        base.LoadTimeInitialise();
        foreach (EndpointId endpoint in _connectedConsumerIds.ToArray())
        {
            IConsumePower consumer = FindEndpoint<IConsumePower>(endpoint);
            if (consumer is null) continue;
            _connectedConsumerIds.Remove(endpoint);
            if (RestoreAffiliation(consumer) && !_connectedConsumers.Contains(consumer))
            {
                _connectedConsumers.Add(consumer);
                _idleConsumers.Add(consumer);
            }
        }

        foreach (EndpointId endpoint in _connectedProducerIds.ToArray())
        {
            IProducePower producer = FindEndpoint<IProducePower>(endpoint);
            if (producer is null) continue;
            _connectedProducerIds.Remove(endpoint);
            if (RestoreAffiliation(producer) && !_connectedProducers.Contains(producer))
            {
                _connectedProducers.Add(producer);
            }
        }

        RecalculateGrid();
    }

    public double TotalSupply => _connectedProducers.Sum(x => x.MaximumPowerInWatts);
    public double TotalDrawdown => _connectedConsumers.Except(_idleConsumers).Sum(x => x.PowerConsumptionInWatts);

    private readonly List<EndpointId> _connectedConsumerIds = new();
    private readonly List<IConsumePower> _connectedConsumers = new();

    private readonly List<EndpointId> _connectedProducerIds = new();
    private readonly List<IProducePower> _connectedProducers = new();

    private readonly List<IConsumePower> _idleConsumers = new();

    private bool RestoreAffiliation(IGameItemComponent component)
    {
        if (component is not ICanConnectToElectricalGrid endpoint) return true;
        if (endpoint.ElectricalGrid is null) endpoint.ElectricalGrid = this;
        // Configured endpoints join as part of their own activation.
        return false;
    }

    private T FindEndpoint<T>(EndpointId endpoint) where T : class, IGameItemComponent
    {
        if (endpoint.ItemId != 0)
        {
            var item = Gameworld.Items.Get(endpoint.ItemId) ?? Locations.SelectMany(x => x.GameItems)
                .FirstOrDefault(x => x.Id == endpoint.ItemId);
            return item?.GetItemTypes<T>().FirstOrDefault(x => x.Id == endpoint.ComponentId);
        }

        // The historical grid writer stored the component ID as element text.
        return Gameworld.Items.Concat(Locations.SelectMany(x => x.GameItems))
            .SelectMany(x => x.GetItemTypes<T>()).FirstOrDefault(x => x.Id == endpoint.LegacyId);
    }

    public void JoinGrid(IConsumePower consumer)
    {
        if (_connectedConsumers.Contains(consumer)) return;
        _idleConsumers.Add(consumer);
        _connectedConsumers.Add(consumer);
        Changed = true;
        RecalculateGrid();
    }

    public void LeaveGrid(IConsumePower consumer)
    {
        if (!_connectedConsumers.Remove(consumer)) return;
        var wasIdle = _idleConsumers.Remove(consumer);
        if (!wasIdle) consumer.OnPowerCutOut();
        Changed = true;
        RecalculateGrid();
    }

    public void JoinGrid(IProducePower producer)
    {
        if (_connectedProducers.Contains(producer)) return;
        _connectedProducers.Add(producer);
        Changed = true;
        RecalculateGrid();
    }

    public void LeaveGrid(IProducePower producer)
    {
        if (!_connectedProducers.Remove(producer)) return;
        Changed = true;
        RecalculateGrid();
    }

    public bool DrawdownSpike(double wattage)
    {
        if (!_gridPowered)
        {
            return false;
        }

        if (TotalSupply >= TotalDrawdown + wattage)
        {
            return true;
        }

        double peakDrawdown = TotalDrawdown + wattage;
        foreach (IConsumePower consumer in _connectedConsumers.Except(_idleConsumers)
                                                    .OrderByDescending(x => x.PowerConsumptionInWatts).ToList())
        {
            _idleConsumers.Add(consumer);
            consumer.OnPowerCutOut();

            if (peakDrawdown <= TotalSupply)
            {
                break;
            }
        }

        if (TotalSupply <= 0.0)
        {
            _gridPowered = false;
        }

        return true;
    }

    private bool _gridPowered;

    private readonly record struct EndpointId(long ItemId, long ComponentId, long LegacyId)
    {
        public static EndpointId FromXml(XElement element)
        {
            if (long.TryParse(element.Attribute("item")?.Value, out var itemId) &&
                long.TryParse(element.Attribute("component")?.Value, out var componentId))
            {
                return new EndpointId(itemId, componentId, 0);
            }

            var legacyId = long.Parse(element.Value);
            return new EndpointId(0, 0, legacyId);
        }

        public XElement ToXml(string name) => ItemId == 0 ? new XElement(name, LegacyId) :
            new XElement(name, new XAttribute("item", ItemId), new XAttribute("component", ComponentId));

        public static XElement ToXml(string name, IGameItemComponent component)
        {
            return new XElement(name, new XAttribute("item", component.Parent.Id),
                new XAttribute("component", component.Id));
        }
    }

    public void RecalculateGrid()
    {
        if (!_gridPowered)
        {
            if (TotalSupply > 0)
            {
                _gridPowered = true;
            }
            else
            {
                return;
            }
        }

        if (TotalDrawdown > TotalSupply)
        {
            foreach (IConsumePower consumer in _connectedConsumers.Except(_idleConsumers)
                                                        .OrderByDescending(x => x.PowerConsumptionInWatts).ToList())
            {
                _idleConsumers.Add(consumer);
                consumer.OnPowerCutOut();

                if (TotalDrawdown <= TotalSupply)
                {
                    break;
                }
            }
        }
        else
        {
            foreach (IConsumePower consumer in _idleConsumers.OrderBy(x => x.PowerConsumptionInWatts).ToList())
            {
                if (!_connectedConsumers.Contains(consumer)) continue;
                if (TotalDrawdown + consumer.PowerConsumptionInWatts <= TotalSupply)
                {
                    _idleConsumers.Remove(consumer);
                    consumer.OnPowerCutIn();
                    continue;
                }

                break;
            }
        }

        if (TotalSupply <= 0.0)
        {
            _gridPowered = false;
            return;
        }
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Grid #{Id.ToString("N0", actor)}");
        sb.AppendLine($"Type: {"Electric".Colour(Telnet.BoldMagenta)}");
        sb.AppendLine($"Locations: {Locations.Count().ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Powered: {_gridPowered.ToColouredString()}");
        sb.AppendLine($"Total Supply: {TotalSupply.ToString("N2", actor).ColourValue()}");
        sb.AppendLine($"Total Drawdown: {TotalDrawdown.ToString("N2", actor).ColourValue()}");
        sb.AppendLine($"Consumers: {_connectedConsumers.Count.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Producers: {_connectedProducers.Count.ToString("N0", actor).ColourValue()}");
        return sb.ToString();
    }
}