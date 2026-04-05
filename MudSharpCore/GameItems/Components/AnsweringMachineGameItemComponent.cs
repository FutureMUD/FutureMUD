#nullable enable
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class AnsweringMachineGameItemComponent : GameItemComponent, IAnsweringMachine, ITelephoneNumberOwner,
    ISelectable, IConnectable, ICanConnectToTelecommunicationsGrid, IContainer, IProducePower
{
    private const string GreetingRecordingName = "__greeting__";
    private const string MessagesPreposition = "in";
    private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
    private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
    private readonly List<IConsumePower> _connectedConsumers = [];
    private readonly List<IConsumePower> _powerUsers = [];
    private readonly List<RecordedAudioSegment> _workingGreetingSegments = [];
    private readonly List<RecordedAudioSegment> _workingMessageSegments = [];
    private readonly List<ScheduledPlaybackSegment> _scheduledGreetingSegments = [];
    private AnsweringMachineGameItemComponentProto _prototype;
    private ITelecommunicationsGrid? _directGrid;
    private string? _directPhoneNumber;
    private string? _preferredNumber;
    private bool _allowSharedNumber;
    private bool _hostedVoicemailEnabled;
    private bool _powered;
    private bool _ringHeartbeatSubscribed;
    private bool _secondHeartbeatSubscribed;
    private bool _switchedOn;
    private bool _isOffHook;
    private bool _isRinging;
    private ITelephoneCall? _currentCall;
    private AudioVolume? _ringVolumeOverride;
    private IProducePower? _connectedPowerSource;
    private ConnectorType? _connectedPowerSourceConnector;
    private ITelephoneNumberOwner? _connectedLineOwner;
    private int _autoAnswerRings;
    private IGameItem? _tapeItem;
    private long _pendingTapeItemId;
    private bool _isRecordingGreeting;
    private long _greetingRecorderId;
    private DateTime? _lastGreetingSegmentUtc;
    private bool _isRecordingMessage;
    private DateTime? _lastMessageSegmentUtc;
    private string? _activeMessageName;
    private DateTime? _activeMessageRecordedAtUtc;
    private DateTime? _scheduledBeepAtUtc;

    public AnsweringMachineGameItemComponent(AnsweringMachineGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
        _autoAnswerRings = proto.DefaultAutoAnswerRings;
        _switchedOn = true;
        parent.OnConnected += Parent_OnConnected;
        parent.OnDisconnected += Parent_OnDisconnected;
    }

    public AnsweringMachineGameItemComponent(Models.GameItemComponent component,
        AnsweringMachineGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
        parent.OnConnected += Parent_OnConnected;
        parent.OnDisconnected += Parent_OnDisconnected;
    }

    public AnsweringMachineGameItemComponent(AnsweringMachineGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _switchedOn = rhs._switchedOn;
        _preferredNumber = rhs._preferredNumber;
        _allowSharedNumber = rhs._allowSharedNumber;
        _hostedVoicemailEnabled = rhs._hostedVoicemailEnabled;
        _ringVolumeOverride = rhs._ringVolumeOverride;
        _autoAnswerRings = rhs._autoAnswerRings;
        newParent.OnConnected += Parent_OnConnected;
        newParent.OnDisconnected += Parent_OnDisconnected;
    }

    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (AnsweringMachineGameItemComponentProto)newProto;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new AnsweringMachineGameItemComponent(this, newParent, temporary);
    }

    private void LoadFromXml(XElement root)
    {
        _switchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "true");
        _preferredNumber = root.Element("PreferredNumber")?.Value;
        _allowSharedNumber = bool.Parse(root.Element("AllowSharedNumber")?.Value ?? "false");
        _hostedVoicemailEnabled = bool.Parse(root.Element("HostedVoicemailEnabled")?.Value ?? "false");
        _autoAnswerRings = int.TryParse(root.Element("AutoAnswerRings")?.Value, out int rings)
            ? rings
            : _prototype.DefaultAutoAnswerRings;
        if (int.TryParse(root.Element("RingVolumeOverride")?.Value, out int ringVolume) &&
            Enum.IsDefined(typeof(AudioVolume), ringVolume))
        {
            _ringVolumeOverride = (AudioVolume)ringVolume;
        }

        _directGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
        _pendingTapeItemId = long.TryParse(root.Element("Tape")?.Value, out long tapeId) ? tapeId : 0L;
        XElement? element = root.Element("ConnectedItems");
        if (element == null)
        {
            return;
        }

        foreach (XElement item in element.Elements("Item"))
        {
            if (item.Attribute("independent")?.Value == "false")
            {
                _pendingDependentLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
                    new ConnectorType(item.Attribute("connectiontype")!.Value)));
            }
            else
            {
                _pendingLoadTimeConnections.Add(Tuple.Create(long.Parse(item.Attribute("id")!.Value),
                    new ConnectorType(item.Attribute("connectiontype")!.Value)));
            }
        }
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Grid", _directGrid?.Id ?? 0),
            new XElement("SwitchedOn", _switchedOn),
            new XElement("PreferredNumber", _preferredNumber ?? string.Empty),
            new XElement("AllowSharedNumber", _allowSharedNumber),
            new XElement("HostedVoicemailEnabled", _hostedVoicemailEnabled),
            new XElement("AutoAnswerRings", _autoAnswerRings),
            new XElement("RingVolumeOverride", _ringVolumeOverride.HasValue ? (int)_ringVolumeOverride.Value : -1),
            new XElement("Tape", _tapeItem?.Id ?? 0L),
            new XElement("ConnectedItems",
                from item in ConnectedItems
                select new XElement("Item",
                    new XAttribute("id", item.Item2.Parent.Id),
                    new XAttribute("connectiontype", item.Item1),
                    new XAttribute("independent", item.Item2.Independent)))
        ).ToString();
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

                Connect(null, connectable);
                break;
            }
        }

        _pendingLoadTimeConnections.Clear();

        foreach (Tuple<long, ConnectorType>? item in _pendingDependentLoadTimeConnections.ToList())
        {
            IGameItem gitem = Gameworld.Items.Get(item.Item1) ?? Gameworld.TryGetItem(item.Item1, true);
            if (gitem == null)
            {
                continue;
            }

            gitem.FinaliseLoadTimeTasks();
            foreach (IConnectable connectable in gitem.GetItemTypes<IConnectable>())
            {
                Connect(null, connectable);
                break;
            }
        }

        _pendingDependentLoadTimeConnections.Clear();

        if (_pendingTapeItemId > 0)
        {
            IGameItem tape = Gameworld.TryGetItem(_pendingTapeItemId, true);
            if (tape != null)
            {
                tape.Get(null);
                tape.LoadTimeSetContainedIn(Parent);
                _tapeItem = tape;
            }

            _pendingTapeItemId = 0L;
        }

        _tapeItem?.FinaliseLoadTimeTasks();
        if (_directGrid != null && _connectedLineOwner == null)
        {
            _directGrid.JoinGrid((ITelephoneNumberOwner)this);
            _directGrid.JoinGrid((IConsumePower)this);
        }

        Changed = true;
    }

    public override void Delete()
    {
        base.Delete();
        Parent.OnConnected -= Parent_OnConnected;
        Parent.OnDisconnected -= Parent_OnDisconnected;
        _connectedPowerSource?.EndDrawdown(this);
        _directGrid?.LeaveGrid((ITelephoneNumberOwner)this);
        _directGrid?.LeaveGrid((IConsumePower)this);
        StopRinging();
        ReleaseSecondHeartbeat();
        foreach (IConsumePower? consumer in _powerUsers.ToList())
        {
            consumer.OnPowerCutOut();
        }

        _tapeItem?.Delete();
        _tapeItem = null;
    }

    public override void Quit()
    {
        _tapeItem?.Quit();
    }

    public override void Login()
    {
        _tapeItem?.Login();
    }

    public IEnumerable<ConnectorType> Connections => _prototype.Connections;
    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;
    public IEnumerable<ConnectorType> FreeConnections
    {
        get
        {
            List<ConnectorType> remaining = new(Connections);
            foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
            {
                remaining.Remove(item.Item1);
            }

            return remaining;
        }
    }

    public bool Independent => true;

    public bool CanBeConnectedTo(IConnectable other)
    {
        return true;
    }

    public bool CanConnect(ICharacter actor, IConnectable other)
    {
        return FreeConnections.Any() &&
               other.FreeConnections.Any() &&
               other.FreeConnections.Any(x => Connections.Any(x.CompatibleWith)) &&
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

        if (!other.FreeConnections.Any(x => Connections.Any(x.CompatibleWith)))
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

    private void Parent_OnConnected(IConnectable other, ConnectorType type)
    {
        if (type.Gender == Gender.Male && other.Parent.GetItemType<ITelephoneNumberOwner>() is { } owner)
        {
            if (_currentCall != null || _isOffHook)
            {
                EndCall(_currentCall);
            }

            _directGrid?.LeaveGrid((ITelephoneNumberOwner)this);
            _directGrid?.LeaveGrid((IConsumePower)this);
            _directGrid = null;
            _connectedLineOwner = owner;
            Changed = true;
        }

        if (!type.Powered)
        {
            return;
        }

        IProducePower? power = other.Parent.GetItemTypes<IProducePower>()
                         .FirstOrDefault(x => x.PrimaryExternalConnectionPowerProducer || x.MaximumPowerInWatts > 0.0);
        if (power == null)
        {
            return;
        }

        _connectedPowerSource = power;
        _connectedPowerSourceConnector = type;
        power.BeginDrawdown(this);
    }

    private void Parent_OnDisconnected(IConnectable other, ConnectorType type)
    {
        if (type.Gender == Gender.Male && other.Parent == _connectedLineOwner?.Parent)
        {
            if (_currentCall != null || _isOffHook)
            {
                EndCall(_currentCall);
            }

            _connectedLineOwner = null;
            Changed = true;
        }

        if (other.Parent != _connectedPowerSource?.Parent || _connectedPowerSourceConnector?.Equals(type) != true)
        {
            return;
        }

        if (_powered)
        {
            OnPowerCutOut();
        }

        _connectedPowerSource.EndDrawdown(this);
        _connectedPowerSource = null;
        _connectedPowerSourceConnector = null;
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
        sb.AppendLine($"Its number is {(PhoneNumber?.ColourValue() ?? "unassigned".ColourError())}.");
        sb.AppendLine($"Its ringer is set to {TelephoneRingSettings.DescribeSetting(RingVolume, false).ColourValue()}.");
        sb.AppendLine($"It answers after {AutoAnswerRings.ToString("N0", voyeur).ColourValue()} rings.");
        sb.AppendLine($"Hosted voicemail is {(HostedVoicemailEnabled ? "enabled".ColourValue() : "disabled".ColourError())} for this line.");
        sb.AppendLine($"Its tape slot currently contains {(_tapeItem?.HowSeen(voyeur) ?? "nothing".ColourError())}.");
        sb.AppendLine($"It has {(GreetingRecording == null ? "no custom greeting".ColourError() : "a custom greeting".ColourValue())} and {MessageRecordings.Count.ToString("N0", voyeur).ColourValue()} saved message{(MessageRecordings.Count == 1 ? string.Empty : "s")}.");
        sb.AppendLine($"It is connected to {(TelecommunicationsGrid == null ? "no telecommunications grid".ColourError() : $"grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
        sb.AppendLine($"Its handset path is {(IsOffHook ? "off the hook".ColourError() : "on the hook".ColourValue())}.");
        if (IsConnected)
        {
            sb.AppendLine("It is currently in an active call.");
        }
        else if (IsRinging)
        {
            sb.AppendLine("It is currently ringing.");
        }
        else if (_currentCall != null)
        {
            sb.AppendLine("It is currently dialling another number.");
        }

        if (_isRecordingGreeting)
        {
            sb.AppendLine("It is currently armed to record a greeting.");
        }

        if (_isRecordingMessage)
        {
            sb.AppendLine("It is currently recording an incoming message.");
        }

        foreach (Tuple<ConnectorType, IConnectable> item in ConnectedItems)
        {
            sb.AppendLine(
                $"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
        }

        return sb.ToString();
    }

    public ITelephoneNumberOwner? NumberOwner => _connectedLineOwner ?? (_directGrid != null ? this : null);
    public string? PhoneNumber => NumberOwner == this ? _directPhoneNumber : NumberOwner?.PhoneNumber;

    public string? PreferredNumber
    {
        get => NumberOwner != null && NumberOwner != this ? NumberOwner.PreferredNumber : _preferredNumber;
        set
        {
            if (NumberOwner != null && NumberOwner != this)
            {
                NumberOwner.PreferredNumber = value;
                return;
            }

            _preferredNumber = string.IsNullOrWhiteSpace(value) ? null : value;
            Changed = true;
            _directGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
        }
    }

    public bool AllowSharedNumber
    {
        get => NumberOwner != null && NumberOwner != this ? NumberOwner.AllowSharedNumber : _allowSharedNumber;
        set
        {
            if (NumberOwner != null && NumberOwner != this)
            {
                NumberOwner.AllowSharedNumber = value;
                return;
            }

            _allowSharedNumber = value;
            Changed = true;
            _directGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
        }
    }

    public bool HostedVoicemailEnabled
    {
        get => NumberOwner != null && NumberOwner != this
            ? NumberOwner.HostedVoicemailEnabled
            : _hostedVoicemailEnabled;
        set
        {
            if (NumberOwner != null && NumberOwner != this)
            {
                NumberOwner.HostedVoicemailEnabled = value;
                return;
            }

            _hostedVoicemailEnabled = value;
            Changed = true;
        }
    }

    public bool IsPowered => _powered;
    public bool IsOffHook => _isOffHook;
    public bool CanReceiveCalls =>
        _switchedOn && IsPowered && TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber) &&
        !_isOffHook && _currentCall == null;
    public bool IsRinging => _isRinging;
    public bool IsConnected => (_currentCall?.Participants ?? Array.Empty<ITelephone>()).Contains(this) &&
                               _currentCall?.IsConnected == true;
    public bool IsEngaged => _currentCall != null || _isOffHook;
    public AudioVolume RingVolume => _ringVolumeOverride ?? _prototype.RingVolume;
    public ITelephoneCall? CurrentCall => _currentCall;
    public IEnumerable<ITelephone> ConnectedPhones =>
        (_currentCall?.Participants ?? Array.Empty<ITelephone>()).Where(x => x != this).ToList();
    public ITelephone? ConnectedPhone => ConnectedPhones.FirstOrDefault();
    public ITelecommunicationsGrid? TelecommunicationsGrid => _connectedLineOwner?.TelecommunicationsGrid ?? _directGrid;
    public int AutoAnswerRings
    {
        get => _autoAnswerRings;
        private set
        {
            _autoAnswerRings = Math.Max(1, value);
            Changed = true;
        }
    }

    public bool IsRecordingGreeting => _isRecordingGreeting;
    public bool IsRecordingMessage => _isRecordingMessage;
    public IAudioStorageTape? Tape => _tapeItem?.GetItemType<IAudioStorageTape>();
    public StoredAudioRecording? GreetingRecording => Tape?.GetRecording(GreetingRecordingName);
    public IReadOnlyList<StoredAudioRecording> MessageRecordings =>
        (Tape?.Recordings ?? [])
        .Where(x => !x.Name.EqualTo(GreetingRecordingName))
        .OrderBy(x => x.RecordedAtUtc)
        .ToList();

    ITelecommunicationsGrid? ICanConnectToTelecommunicationsGrid.TelecommunicationsGrid
    {
        get => _directGrid;
        set => SetDirectGrid(value);
    }

    ITelecommunicationsGrid? ITelephoneNumberOwner.TelecommunicationsGrid
    {
        get => _directGrid;
        set => SetDirectGrid(value);
    }

    public IEnumerable<ITelephone> ConnectedTelephones => (new[] { this }).Concat(GetDownstreamTelephones()).Distinct().ToList();

    public void AssignPhoneNumber(string? number)
    {
        _directPhoneNumber = number;
        Changed = true;
    }

    private void SetDirectGrid(ITelecommunicationsGrid? value)
    {
        if (_directGrid == value)
        {
            return;
        }

        _directGrid?.LeaveGrid((ITelephoneNumberOwner)this);
        _directGrid?.LeaveGrid((IConsumePower)this);
        _directGrid = value;
        if (_directGrid != null && _connectedLineOwner == null)
        {
            _directGrid.JoinGrid((ITelephoneNumberOwner)this);
            _directGrid.JoinGrid((IConsumePower)this);
        }

        Changed = true;
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

    public double PowerConsumptionInWatts => (_switchedOn ? _prototype.Wattage : 0.0) + _powerUsers.Sum(x => x.PowerConsumptionInWatts);

    public void OnPowerCutIn()
    {
        _powered = true;
        foreach (IConsumePower? item in _connectedConsumers.Where(x => !_powerUsers.Contains(x)).ToList())
        {
            _powerUsers.Add(item);
            item.OnPowerCutIn();
        }
    }

    public void OnPowerCutOut()
    {
        _powered = false;
        foreach (IConsumePower? item in _powerUsers.ToList())
        {
            item.OnPowerCutOut();
        }

        _powerUsers.Clear();
        if (_currentCall != null || _isOffHook)
        {
            EndCall(_currentCall);
        }
    }

    public bool PrimaryLoadTimePowerProducer => false;
    public bool PrimaryExternalConnectionPowerProducer => true;
    public double FuelLevel => 1.0;
    public bool ProducingPower => _powered && _switchedOn;
    public double MaximumPowerInWatts => ProducingPower ? _connectedPowerSource?.MaximumPowerInWatts ?? 0.0 : 0.0;

    public void BeginDrawdown(IConsumePower item)
    {
        if (_connectedConsumers.Contains(item))
        {
            return;
        }

        _connectedConsumers.Add(item);
        if (ProducingPower)
        {
            _powerUsers.Add(item);
            item.OnPowerCutIn();
        }

        TelecommunicationsGrid?.RecalculateGrid();
    }

    public void EndDrawdown(IConsumePower item)
    {
        if (!_connectedConsumers.Remove(item))
        {
            return;
        }

        if (_powerUsers.Remove(item))
        {
            item.OnPowerCutOut();
        }

        TelecommunicationsGrid?.RecalculateGrid();
    }

    public bool CanBeginDrawDown(double wattage)
    {
        return ProducingPower;
    }

    public bool CanDrawdownSpike(double wattage)
    {
        return ProducingPower && (_connectedPowerSource?.CanDrawdownSpike(wattage) ?? true);
    }

    public bool DrawdownSpike(double wattage)
    {
        return ProducingPower && (_connectedPowerSource?.DrawdownSpike(wattage) ?? true);
    }

    public bool ManualTransmit => true;
    public string TransmitPremote => _prototype.TransmitPremote;

    public void Transmit(SpokenLanguageInfo spokenLanguage)
    {
        if (!IsConnected || !_switchedOn || !IsPowered || _currentCall == null)
        {
            return;
        }

        _currentCall.RelayTransmission(this, spokenLanguage);
    }

    public void ReceiveTransmission(double frequency, string dataTransmission, long encryption, ITransmit origin)
    {
    }

    public void ReceiveTransmission(double frequency, SpokenLanguageInfo spokenLanguage, long encryption, ITransmit origin)
    {
        if (!_switchedOn || !IsPowered)
        {
            return;
        }

        Parent.OutputHandler?.Handle(
            new LanguageOutput(
                new Emote("@ reproduce|reproduces speech over the line", Parent, Parent),
                spokenLanguage,
                null,
                flags: OutputFlags.PurelyAudible
            )
        );

        if (_isRecordingMessage)
        {
            AppendMessageSegment(spokenLanguage);
        }
    }

    public IEnumerable<string> SwitchSettings => ["on", "off", .. TelephoneRingSettings.LandlineSettings];

    public bool CanSwitch(ICharacter actor, string setting)
    {
        if (setting.Equals("on", StringComparison.InvariantCultureIgnoreCase))
        {
            return !_switchedOn;
        }

        if (setting.Equals("off", StringComparison.InvariantCultureIgnoreCase))
        {
            return _switchedOn;
        }

        return TelephoneRingSettings.TryGetVolumeForSetting(setting, false, out AudioVolume volume) &&
               RingVolume != volume;
    }

    public string WhyCannotSwitch(ICharacter actor, string setting)
    {
        if (TelephoneRingSettings.TryGetVolumeForSetting(setting, false, out _))
        {
            return $"{Parent.HowSeen(actor, true)} is already set to {TelephoneRingSettings.DescribeSetting(RingVolume, false).ColourValue()}.";
        }

        return setting.Equals("on", StringComparison.InvariantCultureIgnoreCase)
            ? $"{Parent.HowSeen(actor, true)} is already on."
            : $"{Parent.HowSeen(actor, true)} is already off.";
    }

    public bool Switch(ICharacter actor, string setting)
    {
        if (!CanSwitch(actor, setting))
        {
            return false;
        }

        if (TelephoneRingSettings.TryGetVolumeForSetting(setting, false, out AudioVolume volume))
        {
            _ringVolumeOverride = volume;
            Changed = true;
            return true;
        }

        _switchedOn = setting.Equals("on", StringComparison.InvariantCultureIgnoreCase);
        Changed = true;
        if (!_switchedOn && (_currentCall != null || _isOffHook))
        {
            EndCall(_currentCall);
        }

        return true;
    }

    public bool CanPickUp(ICharacter actor, out string error)
    {
        if (_isOffHook && _currentCall == null)
        {
            error = "That telephone is already off the hook.";
            return false;
        }

        if (TelecommunicationsGrid == null)
        {
            error = "That telephone is not connected to a telecommunications line.";
            return false;
        }

        if (!_switchedOn || !IsPowered)
        {
            error = "That telephone is not ready to use right now.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public bool PickUp(ICharacter actor, out string error)
    {
        if (!CanPickUp(actor, out error))
        {
            return false;
        }

        if (TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber))
        {
            if (TelecommunicationsGrid.TryPickUp(this, out error))
            {
                return true;
            }

            if (!error.EqualTo("There is no live call on that line right now."))
            {
                return false;
            }
        }

        _isOffHook = true;
        Changed = true;
        error = string.Empty;
        return true;
    }

    public bool CanDial(ICharacter actor, string number, out string error)
    {
        if (_currentCall?.IsConnected == true)
        {
            return CanSendDigits(actor, number, out error);
        }

        if (TelecommunicationsGrid == null)
        {
            error = "That telephone is not connected to a telecommunications grid.";
            return false;
        }

        if (!_switchedOn || !IsPowered || string.IsNullOrWhiteSpace(PhoneNumber))
        {
            error = "That telephone is not ready to make calls right now.";
            return false;
        }

        if (_currentCall != null)
        {
            error = "That telephone is already in use.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(number))
        {
            error = "You must specify a number to dial.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public bool Dial(ICharacter actor, string number, out string error)
    {
        if (_currentCall?.IsConnected == true)
        {
            return SendDigits(actor, number, out error);
        }

        if (!CanDial(actor, number, out error))
        {
            return false;
        }

        _isOffHook = true;
        Changed = true;
        return TelecommunicationsGrid!.TryStartCall(this, number, out error);
    }

    public bool CanSendDigits(ICharacter actor, string digits, out string error)
    {
        if (_currentCall?.IsConnected != true)
        {
            error = "That telephone is not connected to a live call.";
            return false;
        }

        if (!_switchedOn || !IsPowered)
        {
            error = "That telephone is not ready to send keypad digits right now.";
            return false;
        }

        if (!TelephoneNetworkHelpers.TryNormaliseDigits(digits, out _))
        {
            error = "You may only send keypad digits from 0-9, * and #.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public bool SendDigits(ICharacter actor, string digits, out string error)
    {
        if (!CanSendDigits(actor, digits, out error))
        {
            return false;
        }

        string normalised = new(digits.Where(x => !char.IsWhiteSpace(x)).ToArray());
        _currentCall!.RelayDigits(this, normalised);
        error = string.Empty;
        return true;
    }

    public bool CanAnswer(ICharacter actor, out string error)
    {
        if (!_isRinging || _currentCall == null)
        {
            error = "That telephone is not ringing.";
            return false;
        }

        if (!_switchedOn || !IsPowered)
        {
            error = "That telephone cannot answer calls right now.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public bool Answer(ICharacter actor, out string error)
    {
        if (!CanAnswer(actor, out error))
        {
            return false;
        }

        return TelecommunicationsGrid!.TryPickUp(this, out error);
    }

    public bool CanHangUp(ICharacter actor, out string error)
    {
        if (_currentCall == null && !_isOffHook)
        {
            error = "That telephone is not currently in use.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public bool HangUp(ICharacter actor, out string error)
    {
        if (!CanHangUp(actor, out error))
        {
            return false;
        }

        EndCall(_currentCall);
        error = string.Empty;
        return true;
    }

    public void BeginOutgoingCall(ITelephoneCall call, string number)
    {
        _currentCall = call;
        _isOffHook = true;
        _isRinging = false;
        Changed = true;
    }

    public void ReceiveIncomingCall(ITelephoneCall call)
    {
        _currentCall = call;
        _isOffHook = false;
        _isRinging = true;
        Changed = true;
        Ring();
        StartRinging();
    }

    public void ConnectCall(ITelephoneCall call)
    {
        bool wasRinging = _isRinging;
        StopRinging();
        _currentCall = call;
        _isOffHook = true;
        _isRinging = false;
        Changed = true;
        if (wasRinging)
        {
            BeginAnsweringFlow();
        }
    }

    public void NotifyCallProgress(string message)
    {
        Parent.OutputHandler?.Send(message);
    }

    public void ReceiveDigits(ITelephone source, string digits)
    {
        Parent.HandleEvent(EventType.TelephoneDigitsReceived, source.Parent, digits);
    }

    public void EndCall(ITelephoneCall? call, bool notifyGrid = true)
    {
        if (call != null && _currentCall != null && !ReferenceEquals(call, _currentCall))
        {
            return;
        }

        ITelephoneCall? existingCall = _currentCall;
        StopRinging();
        CancelAnsweringFlow();
        FinishMessageRecording();
        _currentCall = null;
        _isRinging = false;
        _isOffHook = false;
        Changed = true;
        if (notifyGrid && existingCall != null)
        {
            TelecommunicationsGrid?.EndCall(this, existingCall);
        }
    }

    private void Ring()
    {
        if (!_isRinging || !_switchedOn || !IsPowered || RingVolume == AudioVolume.Silent)
        {
            return;
        }

        Parent.Handle(
            new AudioOutput(new Emote(_prototype.RingEmote, Parent, Parent), RingVolume,
                flags: OutputFlags.PurelyAudible),
            OutputRange.Local
        );

        foreach (ICell? location in Parent.TrueLocations.Distinct())
        {
            location.HandleAudioEcho("You hear a telephone ringing {0}.", RingVolume, Parent, Parent.RoomLayer);
        }
    }

    private void RingHeartbeat()
    {
        if (!_isRinging || !_switchedOn || !IsPowered)
        {
            StopRinging();
            return;
        }

        Ring();
    }

    private void StartRinging()
    {
        if (_ringHeartbeatSubscribed)
        {
            return;
        }

        Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += RingHeartbeat;
        _ringHeartbeatSubscribed = true;
    }

    private void StopRinging()
    {
        if (!_ringHeartbeatSubscribed)
        {
            return;
        }

        Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= RingHeartbeat;
        _ringHeartbeatSubscribed = false;
    }

    private void EnsureSecondHeartbeat()
    {
        if (_secondHeartbeatSubscribed)
        {
            return;
        }

        Gameworld.HeartbeatManager.SecondHeartbeat += AnsweringMachineHeartbeat;
        _secondHeartbeatSubscribed = true;
    }

    private void ReleaseSecondHeartbeat()
    {
        if (!_secondHeartbeatSubscribed)
        {
            return;
        }

        Gameworld.HeartbeatManager.SecondHeartbeat -= AnsweringMachineHeartbeat;
        _secondHeartbeatSubscribed = false;
    }

    private void AnsweringMachineHeartbeat()
    {
        DateTime now = DateTime.UtcNow;
        foreach (ScheduledPlaybackSegment? segment in _scheduledGreetingSegments.Where(x => x.DueAtUtc <= now).OrderBy(x => x.DueAtUtc).ToList())
        {
            PlayGreetingSegment(segment.Segment);
            _scheduledGreetingSegments.Remove(segment);
        }

        if (_scheduledBeepAtUtc.HasValue && _scheduledBeepAtUtc <= now)
        {
            _scheduledBeepAtUtc = null;
            EmitBeep();
            BeginMessageRecording();
        }

        if (!_scheduledGreetingSegments.Any() && !_scheduledBeepAtUtc.HasValue)
        {
            ReleaseSecondHeartbeat();
        }
    }

    private void BeginAnsweringFlow()
    {
        CancelAnsweringFlow();
        if (GreetingRecording?.Recording.IsEmpty == false)
        {
            ScheduleGreetingPlayback(GreetingRecording.Recording);
            return;
        }

        EmitBeep();
        BeginMessageRecording();
    }

    private void CancelAnsweringFlow()
    {
        _scheduledGreetingSegments.Clear();
        _scheduledBeepAtUtc = null;
        ReleaseSecondHeartbeat();
    }

    private void ScheduleGreetingPlayback(RecordedAudio recording)
    {
        _scheduledGreetingSegments.Clear();
        DateTime due = DateTime.UtcNow;
        foreach (RecordedAudioSegment segment in recording.Segments)
        {
            due = due.Add(segment.DelayBeforeSegment);
            _scheduledGreetingSegments.Add(new ScheduledPlaybackSegment(due, segment));
            due = due.Add(segment.EstimatedSegmentDuration);
        }

        _scheduledBeepAtUtc = due;
        EnsureSecondHeartbeat();
    }

    private void PlayGreetingSegment(RecordedAudioSegment segment)
    {
        if (_currentCall == null)
        {
            CancelAnsweringFlow();
            return;
        }

        SpokenLanguageInfo? spoken = BuildPlaybackLanguage(segment);
        if (spoken == null)
        {
            return;
        }

        Parent.OutputHandler?.Handle(
            new LanguageOutput(
                new Emote("@ play|plays back a recorded voice", Parent, Parent),
                spoken,
                null,
                flags: OutputFlags.PurelyAudible
            )
        );
        _currentCall.RelayTransmission(this, spoken);
    }

    private SpokenLanguageInfo? BuildPlaybackLanguage(RecordedAudioSegment segment)
    {
        ILanguage? language = Gameworld.Languages.Get(segment.LanguageId);
        IAccent? accent = Gameworld.Accents.Get(segment.AccentId);
        if (language == null || accent == null)
        {
            return null;
        }

        return new SpokenLanguageInfo(language, accent, segment.Volume, segment.RawText, segment.Outcome, Parent, null, Parent);
    }

    private void EmitBeep()
    {
        Parent.Handle(
            new AudioOutput(new Emote("@ emit|emits a sharp beep.", Parent, Parent), AudioVolume.Decent,
                flags: OutputFlags.PurelyAudible),
            OutputRange.Local
        );

        foreach (ITelephone? phone in ConnectedPhones.ToList())
        {
            phone.NotifyCallProgress("A sharp beep sounds over the line.");
        }
    }

    private void BeginMessageRecording()
    {
        if (Tape == null || Tape.WriteProtected)
        {
            return;
        }

        _isRecordingMessage = true;
        _workingMessageSegments.Clear();
        _lastMessageSegmentUtc = null;
        _activeMessageRecordedAtUtc = DateTime.UtcNow;
        _activeMessageName = $"message-{_activeMessageRecordedAtUtc.Value:yyyyMMddHHmmss}";
        Changed = true;
    }

    private void AppendMessageSegment(SpokenLanguageInfo spokenLanguage)
    {
        if (Tape == null || string.IsNullOrWhiteSpace(_activeMessageName) || _activeMessageRecordedAtUtc == null)
        {
            _isRecordingMessage = false;
            return;
        }

        DateTime now = DateTime.UtcNow;
        TimeSpan delay = _lastMessageSegmentUtc.HasValue ? now - _lastMessageSegmentUtc.Value : TimeSpan.Zero;
        RecordedAudioSegment segment = RecordedAudioSegment.FromSpokenLanguage(spokenLanguage, delay);
        RecordedAudio recording = new(_workingMessageSegments.Concat([segment]));
        StoredAudioRecording stored = new(_activeMessageName, recording, _activeMessageRecordedAtUtc.Value);
        if (!Tape.CanStoreRecording(stored, out _))
        {
            FinishMessageRecording();
            return;
        }

        _workingMessageSegments.Add(segment);
        _lastMessageSegmentUtc = now;
    }

    private void FinishMessageRecording()
    {
        if (!_isRecordingMessage)
        {
            return;
        }

        _isRecordingMessage = false;
        if (Tape != null && !string.IsNullOrWhiteSpace(_activeMessageName) && _activeMessageRecordedAtUtc != null &&
            _workingMessageSegments.Any())
        {
            RecordedAudio recording = new(_workingMessageSegments);
            Tape.StoreRecording(new StoredAudioRecording(_activeMessageName, recording, _activeMessageRecordedAtUtc.Value), out _);
        }

        _workingMessageSegments.Clear();
        _lastMessageSegmentUtc = null;
        _activeMessageName = null;
        _activeMessageRecordedAtUtc = null;
        Changed = true;
    }

    private void BeginGreetingRecording(ICharacter actor)
    {
        _isRecordingGreeting = true;
        _greetingRecorderId = actor.Id;
        _workingGreetingSegments.Clear();
        _lastGreetingSegmentUtc = null;
        Changed = true;
    }

    private bool StopGreetingRecording(out string error)
    {
        if (!_isRecordingGreeting)
        {
            error = "That answering machine is not currently recording a greeting.";
            return false;
        }

        _isRecordingGreeting = false;
        _greetingRecorderId = 0L;
        if (Tape == null)
        {
            _workingGreetingSegments.Clear();
            error = "There is no tape installed to save the greeting.";
            return false;
        }

        if (!_workingGreetingSegments.Any())
        {
            error = "No greeting was recorded.";
            return false;
        }

        StoredAudioRecording stored = new(GreetingRecordingName, new RecordedAudio(_workingGreetingSegments), DateTime.UtcNow);
        if (!Tape.StoreRecording(stored, out error))
        {
            return false;
        }

        _workingGreetingSegments.Clear();
        _lastGreetingSegmentUtc = null;
        error = string.Empty;
        return true;
    }

    private void AppendGreetingSegment(ICharacter speaker, AudioVolume volume, ILanguage language, IAccent accent,
        string text, IPerceivable? target)
    {
        if (!_isRecordingGreeting || speaker.Id != _greetingRecorderId || speaker.Location == null ||
            !Parent.TrueLocations.Contains(speaker.Location))
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        TimeSpan delay = _lastGreetingSegmentUtc.HasValue ? now - _lastGreetingSegmentUtc.Value : TimeSpan.Zero;
        SpokenLanguageInfo spoken = new(language, accent, volume, text, Outcome.Pass, speaker, target, Parent);
        RecordedAudioSegment segment = RecordedAudioSegment.FromSpokenLanguage(spoken, delay);
        _workingGreetingSegments.Add(segment);
        _lastGreetingSegmentUtc = now;
        Changed = true;
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        switch (type)
        {
            case EventType.CharacterSpeaksWitness:
                AppendGreetingSegment((ICharacter)arguments[0], (AudioVolume)arguments[2], (ILanguage)arguments[3],
                    (IAccent)arguments[4], (string)arguments[5], null);
                return false;
            case EventType.CharacterSpeaksDirectWitness:
                AppendGreetingSegment((ICharacter)arguments[0], (AudioVolume)arguments[3], (ILanguage)arguments[4],
                    (IAccent)arguments[5], (string)arguments[6], (IPerceivable)arguments[1]);
                return false;
        }

        return base.HandleEvent(type, arguments);
    }

    public override bool HandlesEvent(params EventType[] types)
    {
        return types.Contains(EventType.CharacterSpeaksWitness) ||
               types.Contains(EventType.CharacterSpeaksDirectWitness) ||
               base.HandlesEvent(types);
    }

    public bool CanSelect(ICharacter character, string argument)
    {
        return !string.IsNullOrWhiteSpace(argument);
    }

    public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
    {
        StringStack ss = new(argument);
        string verb = ss.PopSpeech().ToLowerInvariant();
        switch (verb)
        {
            case "on":
            case "off":
                if (!Switch(character, verb))
                {
                    character.Send(WhyCannotSwitch(character, verb));
                    return false;
                }

                character.Send($"You switch {Parent.HowSeen(character)} {verb.ColourCommand()}.");
                return true;
            case "rings":
                if (ss.IsFinished || !int.TryParse(ss.SafeRemainingArgument, out int rings) || rings <= 0)
                {
                    character.Send("You must specify a positive number of rings.");
                    return false;
                }

                AutoAnswerRings = rings;
                character.Send($"{Parent.HowSeen(character, true)} will now answer after {AutoAnswerRings.ToString("N0", character).ColourValue()} rings.");
                return true;
            case "greeting":
                return SelectGreeting(character, ss);
            case "voicemail":
                return SelectHostedVoicemail(character, ss);
            case "messages":
                if (!ss.PopSpeech().EqualTo("play"))
                {
                    character.Send("You can only use the messages option as messages play.");
                    return false;
                }

                return PlayMessages(character);
            case "message":
                return PlayMessage(character, ss);
            case "erase":
                return EraseMessages(character, ss);
            default:
                character.Send("That is not a valid answering machine selection.");
                return false;
        }
    }

    private bool SelectGreeting(ICharacter character, StringStack ss)
    {
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "record":
                if (Tape == null)
                {
                    character.Send("There is no tape inserted.");
                    return false;
                }

                if (Tape.WriteProtected)
                {
                    character.Send("The inserted tape is write-protected.");
                    return false;
                }

                BeginGreetingRecording(character);
                character.Send($"You arm {Parent.HowSeen(character)} to record a greeting. Speak in the same location and use {($"select {Parent.HowSeen(character, false, colour: false)} greeting stop").ColourCommand()} when you are done.");
                return true;
            case "stop":
                if (!StopGreetingRecording(out string? error))
                {
                    character.Send(error);
                    return false;
                }

                character.Send($"{Parent.HowSeen(character, true)} saves the new greeting to its tape.");
                return true;
            case "play":
                if (GreetingRecording == null)
                {
                    character.Send("There is no custom greeting recorded.");
                    return false;
                }

                PlayRecordingLocally(GreetingRecording);
                character.Send($"{Parent.HowSeen(character, true)} plays back its greeting.");
                return true;
            case "erase":
                if (Tape == null || !Tape.DeleteRecording(GreetingRecordingName))
                {
                    character.Send("There is no removable greeting recording on the inserted tape.");
                    return false;
                }

                character.Send($"You erase the custom greeting from {Parent.HowSeen(character)}.");
                return true;
            default:
                character.Send("You can use greeting with record, stop, play or erase.");
                return false;
        }
    }

    private bool SelectHostedVoicemail(ICharacter character, StringStack ss)
    {
        string value = ss.PopSpeech().ToLowerInvariant();
        switch (value)
        {
            case "on":
                if (HostedVoicemailEnabled)
                {
                    character.Send("Hosted voicemail is already enabled for this line.");
                    return false;
                }

                HostedVoicemailEnabled = true;
                character.Send($"Hosted voicemail is now enabled for {Parent.HowSeen(character)}.");
                return true;
            case "off":
                if (!HostedVoicemailEnabled)
                {
                    character.Send("Hosted voicemail is already disabled for this line.");
                    return false;
                }

                HostedVoicemailEnabled = false;
                character.Send($"Hosted voicemail is now disabled for {Parent.HowSeen(character)}.");
                return true;
            default:
                character.Send("You can use voicemail with on or off.");
                return false;
        }
    }

    private bool PlayMessages(ICharacter character)
    {
        if (!MessageRecordings.Any())
        {
            character.Send("There are no recorded messages.");
            return false;
        }

        foreach (StoredAudioRecording message in MessageRecordings)
        {
            PlayRecordingLocally(message);
        }

        character.Send($"{Parent.HowSeen(character, true)} plays back all stored messages.");
        return true;
    }

    private bool PlayMessage(ICharacter character, StringStack ss)
    {
        if (ss.IsFinished || !int.TryParse(ss.SafeRemainingArgument, out int index) || index <= 0)
        {
            character.Send("Which message number do you want to play?");
            return false;
        }

        StoredAudioRecording? recording = MessageRecordings.ElementAtOrDefault(index - 1);
        if (recording == null)
        {
            character.Send("There is no message with that index.");
            return false;
        }

        PlayRecordingLocally(recording);
        character.Send($"{Parent.HowSeen(character, true)} plays message #{index.ToString("N0", character)}.");
        return true;
    }

    private bool EraseMessages(ICharacter character, StringStack ss)
    {
        if (Tape == null)
        {
            character.Send("There is no tape inserted.");
            return false;
        }

        string target = ss.PopSpeech().ToLowerInvariant();
        if (target.EqualTo("all"))
        {
            foreach (StoredAudioRecording? message in MessageRecordings.ToList())
            {
                Tape.DeleteRecording(message.Name);
            }

            character.Send($"You erase all stored messages from {Parent.HowSeen(character)}.");
            return true;
        }

        if (!int.TryParse(target, out int index) || index <= 0)
        {
            character.Send("Which message number do you want to erase?");
            return false;
        }

        StoredAudioRecording? recording = MessageRecordings.ElementAtOrDefault(index - 1);
        if (recording == null || !Tape.DeleteRecording(recording.Name))
        {
            character.Send("There is no message with that index.");
            return false;
        }

        character.Send($"You erase message #{index.ToString("N0", character)} from {Parent.HowSeen(character)}.");
        return true;
    }

    private void PlayRecordingLocally(StoredAudioRecording recording)
    {
        foreach (RecordedAudioSegment segment in recording.Recording.Segments)
        {
            SpokenLanguageInfo? spoken = BuildPlaybackLanguage(segment);
            if (spoken == null)
            {
                continue;
            }

            Parent.OutputHandler?.Handle(
                new LanguageOutput(
                    new Emote("@ play|plays back a recorded voice", Parent, Parent),
                    spoken,
                    null,
                    flags: OutputFlags.PurelyAudible
                )
            );
        }
    }

    public IEnumerable<IGameItem> Contents => _tapeItem == null ? [] : [_tapeItem];
    public string ContentsPreposition => MessagesPreposition;
    public bool Transparent => true;
    public bool CanPut(IGameItem item)
    {
        return _tapeItem == null && item.GetItemType<IAudioStorageTape>() != null;
    }

    public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
    {
        _tapeItem = item;
        item.ContainedIn = Parent;
        Changed = true;
    }

    public WhyCannotPutReason WhyCannotPut(IGameItem item)
    {
        if (_tapeItem != null)
        {
            return WhyCannotPutReason.ContainerFull;
        }

        return item.GetItemType<IAudioStorageTape>() == null
            ? WhyCannotPutReason.NotCorrectItemType
            : WhyCannotPutReason.NotContainer;
    }

    public bool CanTake(ICharacter taker, IGameItem item, int quantity)
    {
        return _tapeItem == item && item.CanGet(quantity).AsBool();
    }

    public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
    {
        _tapeItem = null;
        item.ContainedIn = null;
        Changed = true;
        return item;
    }

    public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
    {
        return _tapeItem != item ? WhyCannotGetContainerReason.NotContained : WhyCannotGetContainerReason.NotContainer;
    }

    public int CanPutAmount(IGameItem item)
    {
        return _tapeItem == null ? 1 : 0;
    }

    public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
    {
        if (_tapeItem == null)
        {
            return;
        }

        IGameItem tape = _tapeItem;
        _tapeItem = null;
        tape.ContainedIn = null;
        if (intoContainer != null && intoContainer.CanPut(tape))
        {
            intoContainer.Put(emptier, tape);
        }
        else if (emptier?.Location != null)
        {
            emptier.Location.Insert(tape);
        }
        else
        {
            tape.Delete();
        }

        Changed = true;
    }

    public override bool Take(IGameItem item)
    {
        if (_tapeItem != item)
        {
            return false;
        }

        _tapeItem = null;
        item.ContainedIn = null;
        Changed = true;
        return true;
    }

    public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
    {
        if (_tapeItem != existingItem || newItem.GetItemType<IAudioStorageTape>() == null)
        {
            return false;
        }

        _tapeItem = newItem;
        newItem.ContainedIn = Parent;
        existingItem.ContainedIn = null;
        Changed = true;
        return true;
    }

    public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
    {
        if (_tapeItem != null)
        {
            IContainer? newContainer = newItem?.GetItemType<IContainer>();
            if (newContainer != null && newContainer.CanPut(_tapeItem))
            {
                newContainer.Put(null, _tapeItem);
            }
            else if (location != null)
            {
                _tapeItem.ContainedIn = null;
                location.Insert(_tapeItem);
            }
            else
            {
                _tapeItem.Delete();
            }

            _tapeItem = null;
        }

        foreach (IConnectable? connectedItem in _connectedItems.Select(x => x.Item2).ToList())
        {
            connectedItem.RawDisconnect(this, true);
            IConnectable? newItemConnectable = newItem?.GetItemType<IConnectable>();
            if (newItemConnectable == null)
            {
                location?.Insert(connectedItem.Parent);
                continue;
            }

            if (newItemConnectable.CanConnect(null, connectedItem))
            {
                newItemConnectable.Connect(null, connectedItem);
            }
            else
            {
                location?.Insert(connectedItem.Parent);
            }
        }

        return false;
    }

    public override bool AffectsLocationOnDestruction => true;

    public override bool PreventsMovement()
    {
        return (Parent.InInventoryOf != null && ConnectedItems.Any(x => x.Item2.Independent)) ||
               ConnectedItems.Any(x => x.Item2.Independent && x.Item2.Parent.InInventoryOf != Parent.InInventoryOf);
    }

    public override string WhyPreventsMovement(ICharacter mover)
    {
        List<Tuple<ConnectorType, IConnectable>> preventingItems = ConnectedItems.Where(
                x => x.Item2.Independent &&
                     (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
                                      .ToList();
        return
            $"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
    }

    public override void ForceMove()
    {
        foreach (Tuple<ConnectorType, IConnectable>? item in ConnectedItems.Where(
                x => x.Item2.Independent &&
                     (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
                                         .ToList())
        {
            RawDisconnect(item.Item2, true);
        }
    }

    string ICanConnectToGrid.GridType => "Telecommunications";
    IGrid? ICanConnectToGrid.Grid
    {
        get => _directGrid;
        set => SetDirectGrid(value as ITelecommunicationsGrid);
    }

    private IEnumerable<ITelephone> GetDownstreamTelephones()
    {
        HashSet<long> visited = new()
        { Parent.Id };
        foreach (IGameItem? item in _connectedItems.Where(x => x.Item1.Gender != Gender.Male).Select(x => x.Item2.Parent))
        {
            foreach (ITelephone phone in GetDownstreamTelephones(item, visited))
            {
                yield return phone;
            }
        }
    }

    private IEnumerable<ITelephone> GetDownstreamTelephones(IGameItem item, HashSet<long> visited)
    {
        if (!visited.Add(item.Id))
        {
            yield break;
        }

        if (item.GetItemType<ITelephone>() is { } phone)
        {
            yield return phone;
        }

        if (item.GetItemType<ITelephone>() == null && item.GetItemType<ITelephoneNumberOwner>() != null)
        {
            yield break;
        }

        foreach (IConnectable connectable in item.GetItemTypes<IConnectable>())
        {
            foreach (IGameItem? other in connectable.ConnectedItems.Select(x => x.Item2.Parent))
            {
                foreach (ITelephone otherPhone in GetDownstreamTelephones(other, visited))
                {
                    yield return otherPhone;
                }
            }
        }
    }

    private sealed record ScheduledPlaybackSegment(DateTime DueAtUtc, RecordedAudioSegment Segment);
}
