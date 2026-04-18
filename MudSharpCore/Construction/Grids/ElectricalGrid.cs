using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Construction.Grids;

public class ElectricalGrid : GridBase, IElectricalGrid
{
    public ElectricalGrid(Models.Grid grid, IFuturemud gameworld) : base(grid, gameworld)
    {
        XElement root = XElement.Parse(grid.Definition);
        foreach (XElement element in root.Elements("Consumer"))
        {
            _connectedConsumerIds.Add(long.Parse(element.Value));
        }

        foreach (XElement element in root.Elements("Producer"))
        {
            _connectedProducerIds.Add(long.Parse(element.Value));
        }
    }

    public ElectricalGrid(IFuturemud gameworld, ICell initialLocation) : base(gameworld, initialLocation)
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
            @base.Add(new XElement("Consumer", consumer.Id));
        }

        foreach (IProducePower producer in _connectedProducers)
        {
            @base.Add(new XElement("Producer", producer.Id));
        }

        return @base;
    }

    public override string GridType => "Electrical";

    public override void LoadTimeInitialise()
    {
        base.LoadTimeInitialise();
        foreach (long id in _connectedConsumerIds)
        {
            IConsumePower consumer = Locations.SelectMany(x => x.GameItems).FirstOrDefault(x => x.Id == id)
                                    ?.GetItemType<IConsumePower>();
            if (consumer == null)
            {
                continue;
            }

            _connectedConsumers.Add(consumer);
        }

        foreach (long id in _connectedProducerIds)
        {
            IProducePower producer = Locations.SelectMany(x => x.GameItems).FirstOrDefault(x => x.Id == id)
                                    ?.GetItemType<IProducePower>();
            if (producer == null)
            {
                continue;
            }

            _connectedProducers.Add(producer);
        }

        _connectedConsumerIds.Clear();
        _connectedProducerIds.Clear();
        _idleConsumers.AddRange(_connectedConsumers);
        RecalculateGrid();
    }

    public double TotalSupply => _connectedProducers.Sum(x => x.MaximumPowerInWatts);
    public double TotalDrawdown => _connectedConsumers.Except(_idleConsumers).Sum(x => x.PowerConsumptionInWatts);

    private readonly List<long> _connectedConsumerIds = new();
    private readonly List<IConsumePower> _connectedConsumers = new();

    private readonly List<long> _connectedProducerIds = new();
    private readonly List<IProducePower> _connectedProducers = new();

    private readonly List<IConsumePower> _idleConsumers = new();

    public void JoinGrid(IConsumePower consumer)
    {
        _idleConsumers.Add(consumer);
        _connectedConsumers.Add(consumer);
        Changed = true;
        RecalculateGrid();
    }

    public void LeaveGrid(IConsumePower consumer)
    {
        _idleConsumers.Remove(consumer);
        _connectedConsumers.Remove(consumer);
        Changed = true;
        RecalculateGrid();
    }

    public void JoinGrid(IProducePower producer)
    {
        _connectedProducers.Add(producer);
        Changed = true;
        RecalculateGrid();
    }

    public void LeaveGrid(IProducePower producer)
    {
        _connectedProducers.Remove(producer);
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
                                                    .OrderByDescending(x => x.PowerConsumptionInWatts))
        {
            consumer.OnPowerCutOut();
            _idleConsumers.Add(consumer);

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
                                                        .OrderByDescending(x => x.PowerConsumptionInWatts))
            {
                consumer.OnPowerCutOut();
                _idleConsumers.Add(consumer);

                if (TotalDrawdown <= TotalSupply)
                {
                    break;
                }
            }
        }
        else
        {
            foreach (IConsumePower consumer in _idleConsumers.OrderBy(x => x.PowerConsumptionInWatts))
            {
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