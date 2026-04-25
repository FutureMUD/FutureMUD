#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class CellPhoneTowerGameItemComponent : GameItemComponent, ICellPhoneTower, ICanConnectToTelecommunicationsGrid,
    ISwitchable
{
    private CellPhoneTowerGameItemComponentProto _prototype;
    private ITelecommunicationsGrid? _grid;
    private bool _switchedOn;
    private bool _powered;

    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (CellPhoneTowerGameItemComponentProto)newProto;
    }

    public CellPhoneTowerGameItemComponent(CellPhoneTowerGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
        _switchedOn = true;
    }

    public CellPhoneTowerGameItemComponent(Models.GameItemComponent component, CellPhoneTowerGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public CellPhoneTowerGameItemComponent(CellPhoneTowerGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _switchedOn = rhs._switchedOn;
    }

    private void LoadFromXml(XElement root)
    {
        _switchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "true");
        TelecommunicationsGrid =
            Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new CellPhoneTowerGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
            new XElement("SwitchedOn", _switchedOn)
        ).ToString();
    }

    public ITelecommunicationsGrid? TelecommunicationsGrid
    {
        get => _grid;
        set
        {
            if (_grid == value)
            {
                return;
            }

            _grid?.LeaveGrid((IConsumePower)this);
            _grid = value;
            _grid?.JoinGrid((IConsumePower)this);
            Changed = true;
        }
    }

    public bool IsPowered => _powered && _switchedOn;

    public bool ProvidesCoverage(IZone zone)
    {
        return IsPowered &&
               TelecommunicationsGrid != null &&
               Parent.TrueLocations.Any(x => x != null && x.Zone == zone);
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        if (type != DescriptionType.Full)
        {
            return base.Decorate(voyeur, name, description, type, colour, flags);
        }

        StringBuilder sb = new();
        sb.AppendLine(description);
        sb.AppendLine($"It is currently switched {(_switchedOn ? "on".ColourValue() : "off".ColourError())}.");
        sb.AppendLine($"It is {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}.");
        sb.AppendLine(
            $"It is connected to {(TelecommunicationsGrid == null ? "no telecommunications grid".ColourError() : $"grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
        sb.AppendLine(
            $"It currently {(IsPowered ? "provides".ColourValue() : "does not provide".ColourError())} cellular service to this zone.");
        return sb.ToString();
    }

    public double PowerConsumptionInWatts => _switchedOn ? _prototype.Wattage : 0.0;

    public void OnPowerCutIn()
    {
        _powered = true;
    }

    public void OnPowerCutOut()
    {
        _powered = false;
    }

    public bool SwitchedOn
    {
        get => _switchedOn;
        set
        {
            _switchedOn = value;
            Changed = true;
        }
    }

    public IEnumerable<string> SwitchSettings => ["on", "off"];

    public bool CanSwitch(ICharacter actor, string setting)
    {
        return setting.Equals("on", System.StringComparison.InvariantCultureIgnoreCase) ? !_switchedOn
            : setting.Equals("off", System.StringComparison.InvariantCultureIgnoreCase) && _switchedOn;
    }

    public string WhyCannotSwitch(ICharacter actor, string setting)
    {
        return setting.Equals("on", System.StringComparison.InvariantCultureIgnoreCase)
            ? $"{Parent.HowSeen(actor, true)} is already on."
            : $"{Parent.HowSeen(actor, true)} is already off.";
    }

    public bool Switch(ICharacter actor, string setting)
    {
        if (!CanSwitch(actor, setting))
        {
            return false;
        }

        _switchedOn = setting.Equals("on", System.StringComparison.InvariantCultureIgnoreCase);
        Changed = true;
        return true;
    }

    public override void Delete()
    {
        base.Delete();
        TelecommunicationsGrid = null;
    }

    string ICanConnectToGrid.GridType => "Telecommunications";

    IGrid? ICanConnectToGrid.Grid
    {
        get => TelecommunicationsGrid;
        set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
    }

    ITelecommunicationsGrid? ICanConnectToTelecommunicationsGrid.TelecommunicationsGrid
    {
        get => TelecommunicationsGrid;
        set => TelecommunicationsGrid = value;
    }
}
