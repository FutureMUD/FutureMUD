using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class UnlimitedGeneratorGameItemComponent : GameItemComponent, IProducePower, ISwitchable, IOnOff
{
    protected UnlimitedGeneratorGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (UnlimitedGeneratorGameItemComponentProto)newProto;
    }

    private readonly List<IConsumePower> _connectedConsumers = new();
    private readonly List<IConsumePower> _powerUsers = new();

    #region IOnOff Implementation

    private bool _switchedOn;
    private bool _active;

    public bool SwitchedOn
    {
        get => _switchedOn;
        set
        {
            if (_switchedOn == value) return;
            _switchedOn = value;
            Changed = true;
            if (value) CheckOn();
            else SuspendOutput();
        }
    }

    #endregion

    #region Constructors

    public UnlimitedGeneratorGameItemComponent(UnlimitedGeneratorGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public UnlimitedGeneratorGameItemComponent(Models.GameItemComponent component,
        UnlimitedGeneratorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public UnlimitedGeneratorGameItemComponent(UnlimitedGeneratorGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _switchedOn = rhs.SwitchedOn;
    }

    protected void LoadFromXml(XElement root)
    {
        _switchedOn = (bool?)root.Attribute("On") ?? false;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new UnlimitedGeneratorGameItemComponent(this, newParent, temporary);
    }

    public override void Login()
    {
        if (_active) return;
        _active = true;
        CheckOn();
    }

    public override void Quit()
    {
        _active = false;
        SuspendOutput();
    }

    public override void Delete()
    {
        Quit();
        _connectedConsumers.Clear();
        base.Delete();
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
                new XAttribute("On", SwitchedOn))
            .ToString();
    }

    #endregion

    #region Override of Base

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Short ||
               type == DescriptionType.Full;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Short:
                return $"{description}{(SwitchedOn ? " (on)".FluentColour(Telnet.BoldWhite, colour) : "")}";
            case DescriptionType.Full:
                StringBuilder sb = new();
                sb.Append(description);
                sb.AppendLine();
                sb.AppendLine($"It is{(SwitchedOn ? "" : " not")} switched on.".Colour(Telnet.Yellow));
                return sb.ToString();
        }

        throw new NotSupportedException("Invalid Decorate type in UnlimitedGeneratorGameItemComponent.Decorate");
    }

    public override int DecorationPriority => int.MaxValue;

    public override bool PreventsMerging(IGameItemComponent component)
    {
        return true;
    }

    #endregion

    #region IProducePower

    public bool PrimaryLoadTimePowerProducer => true;
    public bool PrimaryExternalConnectionPowerProducer => false;
    public double FuelLevel => 1.0;
    public bool ProducingPower => _active && SwitchedOn;

    public double MaximumPowerInWatts => ProducingPower
        ? _prototype.WattageProvided * Parent.EffectsOfType<IMagicPowerOrFuelEnhancementEffect>(x =>
            x.AppliesToPoweredItem(Parent)).Aggregate(1.0, (current, effect) => current * effect.PowerProductionMultiplier)
        : 0.0;

    private double _spikeDrawdown;
    internal double RouteCheckpointSpikeDrawdown
    {
        get => _spikeDrawdown;
        set => _spikeDrawdown = value;
    }

    public bool CanBeginDrawDown(double wattage)
    {
        return SwitchedOn && ProducingPower &&
               _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
               MaximumPowerInWatts;
    }

    public bool CanDrawdownSpike(double wattage)
    {
        return SwitchedOn &&
               _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
               MaximumPowerInWatts && ProducingPower;
    }

    public bool DrawdownSpike(double wattage)
    {
        if (!CanDrawdownSpike(wattage))
        {
            return false;
        }

        _spikeDrawdown += wattage;
        return true;
    }

    public void BeginDrawdown(IConsumePower item)
    {
        if (!_connectedConsumers.Contains(item))
        {
            _connectedConsumers.Add(item);

            if (ProducingPower &&
                _powerUsers.Sum(x => x.PowerConsumptionInWatts) + item.PowerConsumptionInWatts <=
                MaximumPowerInWatts)
            {
                _powerUsers.Add(item);
                item.OnPowerCutIn();
            }
        }
    }

    public void EndDrawdown(IConsumePower item)
    {
        _connectedConsumers.Remove(item);
        if (_powerUsers.Remove(item)) item.OnPowerCutOut();
    }

    #endregion

    #region ISwitchable

    public IEnumerable<string> SwitchSettings => new[] { "on", "off" };

    public bool CanSwitch(ICharacter actor, string setting)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out _, Parent))
		{
			return false;
		}

        return (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn) ||
               (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
            ;
    }

    public string WhyCannotSwitch(ICharacter actor, string setting)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
		{
			return manipulationReason;
		}

        if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
        {
            return $"{Parent.HowSeen(actor)} is already on.";
        }

        if (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn)
        {
            return $"{Parent.HowSeen(actor)} is already off.";
        }

        return $"{Parent.HowSeen(actor)} cannot be switched to {setting} at this time.";
    }

    public bool Switch(ICharacter actor, string setting)
    {
		if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
		{
			return false;
		}

        if (!CanSwitch(actor, setting))
        {
            return false;
        }

        return setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase)
            ? SwitchOn(actor)
            : SwitchOff(actor);
    }

    private bool SwitchOn(ICharacter actor)
    {
        SwitchedOn = true;
        _prototype.SwitchOnProg?.Execute(actor, Parent);
        return true;
    }

    private bool _heartbeatOn;

    private void CheckOn()
    {
        if (!ProducingPower || _heartbeatOn) return;

        _spikeDrawdown = 0.0;
        _heartbeatOn = true;
        Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
        foreach (var item in _connectedConsumers.ToArray())
        {
            if (!ProducingPower) break;
            if (!_connectedConsumers.Contains(item) || _powerUsers.Contains(item) ||
                _powerUsers.Sum(x => x.PowerConsumptionInWatts) + item.PowerConsumptionInWatts + _spikeDrawdown >
                MaximumPowerInWatts) continue;
            _powerUsers.Add(item);
            item.OnPowerCutIn();
        }
    }

    private void SuspendOutput()
    {
        if (_heartbeatOn)
        {
            _heartbeatOn = false;
            Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
        }

        _spikeDrawdown = 0.0;
        var powered = _powerUsers.ToArray();
        _powerUsers.Clear();
        foreach (var consumer in powered) consumer.OnPowerCutOut();
    }

    private void HeartbeatManager_SecondHeartbeat()
    {
        _spikeDrawdown = 0.0;
    }

    private bool SwitchOff(ICharacter actor)
    {
        _prototype.SwitchOffProg?.Execute(actor, Parent);
        SwitchedOn = false;
        return true;
    }

    #endregion
}
