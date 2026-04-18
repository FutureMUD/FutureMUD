#nullable enable
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class LiquidConsumingPropGameItemComponent : GameItemComponent, ILiquidContainer, IConnectable
{
    protected LiquidConsumingPropGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    private LiquidMixture? _liquidMixture;
    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
    private bool _heartbeatOn;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (LiquidConsumingPropGameItemComponentProto)newProto;
        if (LiquidMixture != null && LiquidMixture.TotalVolume > LiquidCapacity)
        {
            LiquidMixture.SetLiquidVolume(LiquidCapacity);
        }
    }

    public LiquidConsumingPropGameItemComponent(LiquidConsumingPropGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public LiquidConsumingPropGameItemComponent(Models.GameItemComponent component,
        LiquidConsumingPropGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public LiquidConsumingPropGameItemComponent(LiquidConsumingPropGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        if (rhs.LiquidMixture != null)
        {
            LiquidMixture = new LiquidMixture(rhs.LiquidMixture);
        }
    }

    private void LoadFromXml(XElement root)
    {
        if (root.Element("Mix") != null)
        {
            LiquidMixture = new LiquidMixture(root.Element("Mix")!, Gameworld);
        }
        else if (root.Element("NoLiquid") == null && root.Element("Liquid") != null)
        {
            ILiquid? liquid = Gameworld.Liquids.Get(long.Parse(root.Element("Liquid")!.Value));
            if (liquid != null)
            {
                LiquidMixture = new LiquidMixture(new[]
                {
                    new LiquidInstance
                    {
                        Liquid = liquid,
                        Amount = double.Parse(root.Element("LiquidQuantity")?.Value ?? "0.0")
                    }
                }, Gameworld);
            }
        }

        XElement? connectors = root.Element("ConnectedItems");
        if (connectors != null)
        {
            foreach (XElement item in connectors.Elements("Item"))
            {
                ConnectorType connector = new(item.Attribute("connectiontype")!.Value);
                if (item.Attribute("independent")?.Value == "false")
                {
                    _pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value), connector));
                }
                else
                {
                    _pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value), connector));
                }
            }
        }
    }

    public override void FinaliseLoad()
    {
        foreach (Tuple<long, ConnectorType>? item in _pendingLoadTimeConnections.ToList())
        {
            IGameItem? gitem = Gameworld.Items.Get(item.Item1);
            if (gitem == null || gitem.Location != Parent.Location)
            {
                continue;
            }

            foreach (IConnectable connectable in gitem.GetItemTypes<IConnectable>())
            {
                if (!connectable.CanConnect(null, this))
                {
                    continue;
                }

                connectable.Connect(null, this);
                break;
            }
        }

        _pendingLoadTimeConnections.Clear();

        foreach (Tuple<long, ConnectorType>? item in _pendingDependentLoadTimeConnections.ToList())
        {
            IGameItem? gitem = Gameworld.Items.Get(item.Item1);
            if (gitem == null)
            {
                gitem = Gameworld.TryGetItem(item.Item1, true);
                if (gitem == null)
                {
                    continue;
                }

                gitem.FinaliseLoadTimeTasks();
            }

            foreach (IConnectable connectable in gitem.GetItemTypes<IConnectable>())
            {
                connectable.Connect(null, this);
                break;
            }
        }

        _pendingDependentLoadTimeConnections.Clear();
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new LiquidConsumingPropGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"),
            new XElement("ConnectedItems",
                from item in ConnectedItems
                select new XElement("Item",
                    new XAttribute("id", item.Item2.Parent.Id),
                    new XAttribute("connectiontype", item.Item1),
                    new XAttribute("independent", item.Item2.Independent)))
        ).ToString();
    }

    private void CheckHeartbeat()
    {
        if (_heartbeatOn && (LiquidMixture?.IsEmpty != false || ConsumptionPerSecond <= 0.0))
        {
            EndHeartbeat();
            return;
        }

        if (!_heartbeatOn && LiquidMixture?.IsEmpty == false && ConsumptionPerSecond > 0.0)
        {
            StartHeartbeat();
        }
    }

    private void StartHeartbeat()
    {
        if (_heartbeatOn)
        {
            return;
        }

        Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManagerOnSecondHeartbeat;
        _heartbeatOn = true;
    }

    private void HeartbeatManagerOnSecondHeartbeat()
    {
        if (LiquidMixture?.IsEmpty != false)
        {
            CheckHeartbeat();
            return;
        }

        ReduceLiquidQuantity(Math.Min(ConsumptionPerSecond, LiquidVolume), null!, "consumed");
        CheckHeartbeat();
    }

    private void EndHeartbeat()
    {
        if (!_heartbeatOn)
        {
            return;
        }

        Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
        _heartbeatOn = false;
    }

    public override void Delete()
    {
        base.Delete();
        EndHeartbeat();
        _connectedItems.Clear();
    }

    public override void Quit()
    {
        base.Quit();
        EndHeartbeat();
    }

    public override void Login()
    {
        base.Login();
        CheckHeartbeat();
    }

    public LiquidMixture? LiquidMixture
    {
        get => _liquidMixture;
        set
        {
            _liquidMixture = value;
            Changed = true;
            CheckHeartbeat();
        }
    }

    public double LiquidCapacity => _prototype.LiquidCapacity;
    public bool CanBeEmptiedWhenInRoom => _prototype.CanBeEmptiedWhenInRoom;
    public double ConsumptionPerSecond => _prototype.ConsumptionPerSecond;
    public bool Transparent => _prototype.Transparent;
    public string ContentsPreposition => _prototype.ContentsPreposition;
    public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;

    private void AdjustLiquidQuantity(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return;
        }

        LiquidMixture.AddLiquidVolume(amount);
        if (LiquidMixture.IsEmpty)
        {
            LiquidMixture = null;
        }

        Changed = true;
        CheckHeartbeat();
    }

    public void AddLiquidQuantity(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return;
        }

        if (LiquidMixture.TotalVolume + amount > LiquidCapacity)
        {
            amount = LiquidCapacity - LiquidMixture.TotalVolume;
        }

        if (LiquidMixture.TotalVolume + amount < 0)
        {
            amount = -1 * LiquidMixture.TotalVolume;
        }

        AdjustLiquidQuantity(amount, who, action);
    }

    public void ReduceLiquidQuantity(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return;
        }

        if (LiquidMixture.TotalVolume - amount > LiquidCapacity)
        {
            amount = (LiquidCapacity - LiquidMixture.TotalVolume) * -1;
        }

        if (LiquidMixture.TotalVolume - amount < 0)
        {
            amount = LiquidMixture.TotalVolume;
        }

        AdjustLiquidQuantity(amount * -1, who, action);
    }

    public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
    {
        if (otherMixture == null)
        {
            return;
        }

        if (LiquidMixture == null)
        {
            LiquidMixture = otherMixture.Clone(Math.Min(otherMixture.TotalVolume, LiquidCapacity));
        }
        else
        {
            double remainingCapacity = LiquidCapacity - LiquidMixture.TotalVolume;
            if (remainingCapacity <= 0.0)
            {
                return;
            }

            LiquidMixture.AddLiquid(otherMixture.Clone(Math.Min(otherMixture.TotalVolume, remainingCapacity)));
        }

        if (LiquidMixture.IsEmpty)
        {
            LiquidMixture = null;
        }

        Changed = true;
        CheckHeartbeat();
    }

    public LiquidMixture? RemoveLiquidAmount(double amount, ICharacter who, string action)
    {
        if (LiquidMixture == null)
        {
            return null;
        }

        LiquidMixture newMixture = LiquidMixture.RemoveLiquidVolume(amount);
        Changed = true;
        if (LiquidMixture.IsEmpty)
        {
            LiquidMixture = null;
        }

        CheckHeartbeat();
        return newMixture;
    }

    public bool IsOpen => true;
    public bool CanOpen(IBody opener)
    {
        return false;
    }

    public WhyCannotOpenReason WhyCannotOpen(IBody opener)
    {
        return WhyCannotOpenReason.NotOpenable;
    }

    public void Open()
    {
    }
    public bool CanClose(IBody closer)
    {
        return false;
    }

    public WhyCannotCloseReason WhyCannotClose(IBody closer)
    {
        return WhyCannotCloseReason.NotOpenable;
    }

    public void Close()
    {
    }
    public event OpenableEvent? OnOpen;
    public event OpenableEvent? OnClose;

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Contents || type == DescriptionType.Full || type == DescriptionType.Evaluate;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Contents:
                if (LiquidMixture?.IsEmpty == false)
                {
                    return $"It is currently filled with {LiquidMixture.ColouredLiquidLongDescription}.";
                }

                return $"{description}\n\nIt is currently empty.";
            case DescriptionType.Evaluate:
                return $"{description}\nIt can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} and consumes {ConsumptionPerSecond:N2} units per second.";
            case DescriptionType.Full:
                StringBuilder sb = new();
                sb.AppendLine(description);
                sb.AppendLine(
                    $"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} and consumes {ConsumptionPerSecond:N2} units per second.");
                sb.AppendLine(
                    $"It is {(Transparent ? "transparent".ColourValue() : "not transparent".ColourError())} and can {(CanBeEmptiedWhenInRoom ? "" : "not ")}be emptied when in the room.");
                if (LiquidMixture?.IsEmpty == false)
                {
                    sb.AppendLine($"It currently contains {LiquidMixture.ColouredLiquidLongDescription}.");
                }

                if (ConnectedItems.Any())
                {
                    sb.AppendLine();
                }

                foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
                {
                    sb.AppendLine(
                        $"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
                }

                return sb.ToString();
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    public IEnumerable<ConnectorType> Connections => _prototype.Connections;
    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

    public IEnumerable<ConnectorType> FreeConnections
    {
        get
        {
            List<ConnectorType> rvar = new(Connections);
            foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
            {
                rvar.Remove(item.Item1);
            }

            return rvar;
        }
    }

    public bool Independent => true;

    public bool CanBeConnectedTo(IConnectable other)
    {
        return true;
    }

    public bool CanConnect(ICharacter actor, IConnectable other)
    {
        if (!FreeConnections.Any() || !other.FreeConnections.Any())
        {
            return false;
        }

        return other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)) &&
               other.CanBeConnectedTo(this);
    }

    public void Connect(ICharacter actor, IConnectable other)
    {
        ConnectorType? connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
        if (connection == null)
        {
            return;
        }

        RawConnect(other, connection);
        other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(connection)));
        Changed = true;
    }

    public void RawConnect(IConnectable other, ConnectorType type)
    {
        _connectedItems.Add(Tuple.Create(type, other));
        _pendingLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
        _pendingDependentLoadTimeConnections.RemoveAll(x => x.Item1 == other.Parent.Id && x.Item2.CompatibleWith(type));
        Parent.ConnectedItem(other, type);
        Changed = true;
    }

    public string WhyCannotConnect(ICharacter actor, IConnectable other)
    {
        if (!FreeConnections.Any())
        {
            return
                $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as the former has no free connection points.";
        }

        if (!other.FreeConnections.Any())
        {
            return
                $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as the latter has no free connection points.";
        }

        if (!other.FreeConnections.Any(x => _prototype.Connections.Any(x.CompatibleWith)))
        {
            return
                $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as none of the free connection points are compatible.";
        }

        return !other.CanBeConnectedTo(this)
            ? $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} as that item cannot be connected to."
            : $"You cannot connect {Parent.HowSeen(actor)} to {other.Parent.HowSeen(actor)} for an unknown reason.";
    }

    public bool CanDisconnect(ICharacter actor, IConnectable other)
    {
        return _connectedItems.Any(x => x.Item2 == other);
    }

    public void Disconnect(ICharacter actor, IConnectable other)
    {
        RawDisconnect(other, true);
    }

    public void RawDisconnect(IConnectable other, bool handleEvents)
    {
        if (handleEvents)
        {
            other.RawDisconnect(this, false);
            foreach (Tuple<ConnectorType, IConnectable>? connection in _connectedItems.Where(x => x.Item2 == other).ToList())
            {
                Parent.DisconnectedItem(other, connection.Item1);
                other.Parent.DisconnectedItem(this, connection.Item1);
            }
        }

        _connectedItems.RemoveAll(x => x.Item2 == other);
        Changed = true;
    }

    public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
    {
        return _connectedItems.All(x => x.Item2 != other)
            ? $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} because they are not connected!"
            : $"You cannot disconnect {Parent.HowSeen(actor)} from {other.Parent.HowSeen(actor)} for an unknown reason";
    }

    public bool CanBeDisconnectedFrom(IConnectable other)
    {
        return true;
    }
}
