#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class TelephoneGameItemComponent : GameItemComponent, ITelephone, ITelephoneNumberOwner,
	IConnectable, ICanConnectToTelecommunicationsGrid
{
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
	private TelephoneGameItemComponentProto _prototype;
	private ITelecommunicationsGrid? _directGrid;
	private string? _directPhoneNumber;
	private string? _preferredNumber;
	private bool _allowSharedNumber;
	private bool _powered;
	private bool _ringHeartbeatSubscribed;
	private bool _switchedOn;
	private bool _isOffHook;
	private bool _isRinging;
	private ITelephoneCall? _currentCall;
	private IProducePower? _connectedPowerSource;
	private ConnectorType? _connectedPowerSourceConnector;
	private ITelephoneNumberOwner? _connectedLineOwner;

	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TelephoneGameItemComponentProto)newProto;
	}

	public TelephoneGameItemComponent(TelephoneGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		_switchedOn = true;
		parent.OnConnected += Parent_OnConnected;
		parent.OnDisconnected += Parent_OnDisconnected;
	}

	public TelephoneGameItemComponent(Models.GameItemComponent component, TelephoneGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
		parent.OnConnected += Parent_OnConnected;
		parent.OnDisconnected += Parent_OnDisconnected;
	}

	public TelephoneGameItemComponent(TelephoneGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_switchedOn = rhs._switchedOn;
		_preferredNumber = rhs._preferredNumber;
		_allowSharedNumber = rhs._allowSharedNumber;
		newParent.OnConnected += Parent_OnConnected;
		newParent.OnDisconnected += Parent_OnDisconnected;
	}

	private void LoadFromXml(XElement root)
	{
		_switchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "true");
		_preferredNumber = root.Element("PreferredNumber")?.Value;
		_allowSharedNumber = bool.Parse(root.Element("AllowSharedNumber")?.Value ?? "false");
		_directGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
		var element = root.Element("ConnectedItems");
		if (element == null)
		{
			return;
		}

		foreach (var item in element.Elements("Item"))
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

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TelephoneGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", _directGrid?.Id ?? 0),
			new XElement("SwitchedOn", _switchedOn),
			new XElement("PreferredNumber", _preferredNumber ?? string.Empty),
			new XElement("AllowSharedNumber", _allowSharedNumber),
			new XElement("ConnectedItems",
				from item in ConnectedItems
				select
					new XElement("Item",
						new XAttribute("id", item.Item2.Parent.Id),
						new XAttribute("connectiontype", item.Item1),
						new XAttribute("independent", item.Item2.Independent)))
		).ToString();
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _pendingLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null || gitem.Location != Parent.Location)
			{
				continue;
			}

			foreach (var connectable in gitem.GetItemTypes<IConnectable>())
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

		foreach (var item in _pendingDependentLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1) ?? Gameworld.TryGetItem(item.Item1, true);
			if (gitem == null)
			{
				continue;
			}

			gitem.FinaliseLoadTimeTasks();
			foreach (var connectable in gitem.GetItemTypes<IConnectable>())
			{
				Connect(null, connectable);
				break;
			}
		}

		_pendingDependentLoadTimeConnections.Clear();
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
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _pendingLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null || gitem.Location != Parent.Location)
			{
				continue;
			}

			foreach (var connectable in gitem.GetItemTypes<IConnectable>())
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

		foreach (var item in _pendingDependentLoadTimeConnections.ToList())
		{
			var gitem = Gameworld.Items.Get(item.Item1);
			if (gitem == null)
			{
				gitem = Gameworld.TryGetItem(item.Item1, true);
				if (gitem == null)
				{
					continue;
				}

				gitem.FinaliseLoadTimeTasks();
			}

			foreach (var connectable in gitem.GetItemTypes<IConnectable>())
			{
				connectable.Connect(null, this);
				break;
			}
		}

		_pendingDependentLoadTimeConnections.Clear();
		ResolveTelecommunicationsGrid();
	}

	public IEnumerable<ConnectorType> Connections => _prototype.Connections;
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			var rvar = new List<ConnectorType>(Connections);
			foreach (var item in ConnectedItems)
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
		var connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
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
		Parent_OnConnected(other, type);
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
			foreach (var connection in _connectedItems.Where(x => x.Item2 == other).ToList())
			{
				Parent.DisconnectedItem(other, connection.Item1);
				other.Parent.DisconnectedItem(this, connection.Item1);
				Parent_OnDisconnected(other, connection.Item1);
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
		if (other.Parent.GetItemType<ICanConnectToTelecommunicationsGrid>()?.TelecommunicationsGrid is { } teleGrid)
		{
			TelecommunicationsGrid ??= teleGrid;
		}

		if (!type.Powered)
		{
			return;
		}

		var power = other.Parent.GetItemTypes<IProducePower>()
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
		if (other.Parent == _connectedPowerSource?.Parent && _connectedPowerSourceConnector?.CompatibleWith(type) == true)
		{
			if (_connectedPowerSource.ProducingPower)
			{
				OnPowerCutOut();
			}

			_connectedPowerSource.EndDrawdown(this);
			_connectedPowerSource = null;
			_connectedPowerSourceConnector = null;
		}

		var otherTelecomGrid = other.Parent.GetItemType<ICanConnectToTelecommunicationsGrid>()?.TelecommunicationsGrid;
		if (otherTelecomGrid != null && TelecommunicationsGrid == otherTelecomGrid)
		{
			ResolveTelecommunicationsGrid();
		}
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

		var sb = new StringBuilder();
		sb.AppendLine(description);
		sb.AppendLine($"It is currently switched {(_switchedOn ? "on".ColourValue() : "off".ColourError())}.");
		sb.AppendLine($"It is {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}.");
		sb.AppendLine($"Its number is {(PhoneNumber?.ColourValue() ?? "unassigned".ColourError())}.");
		sb.AppendLine(
			$"It is connected to {(TelecommunicationsGrid == null ? "no telecommunications grid".ColourError() : $"grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
		sb.AppendLine(
			$"Its handset is {(IsOffHook ? "off the hook".ColourError() : "on the hook".ColourValue())}.");
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

		foreach (var item in ConnectedItems)
		{
			sb.AppendLine(
				$"It is currently connected to {item.Item2.Parent.HowSeen(voyeur)} by a {item.Item1.ConnectionType.Colour(Telnet.Green)} connection.");
		}

		return sb.ToString();
	}

	public ITelephoneNumberOwner? NumberOwner => _connectedLineOwner ?? (_directGrid != null ? this : null);
	public string? PhoneNumber => NumberOwner?.PhoneNumber ?? _directPhoneNumber;

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

	public bool IsPowered => _powered;
	public bool IsOffHook => _isOffHook;
	public bool CanReceiveCalls =>
		_switchedOn && IsPowered && TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber) &&
		!_isOffHook && _currentCall == null;

	public bool IsRinging => _isRinging;
	public bool IsConnected => (_currentCall?.Participants ?? Array.Empty<ITelephone>()).Contains(this) &&
	                           _currentCall?.IsConnected == true;
	public bool IsEngaged => _currentCall != null || _isOffHook;
	public ITelephoneCall? CurrentCall => _currentCall;
	public IEnumerable<ITelephone> ConnectedPhones =>
		(_currentCall?.Participants ?? Array.Empty<ITelephone>()).Where(x => x != this).ToList();
	public ITelephone? ConnectedPhone => ConnectedPhones.FirstOrDefault();
	public ITelecommunicationsGrid? TelecommunicationsGrid => _connectedLineOwner?.TelecommunicationsGrid ?? _directGrid;

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

	public IEnumerable<ITelephone> ConnectedTelephones => [this];

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

	public double PowerConsumptionInWatts => _switchedOn ? _prototype.Wattage : 0.0;

	public void OnPowerCutIn()
	{
		_powered = true;
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		if (_currentCall != null || _isOffHook)
		{
			EndCall(_currentCall);
		}
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

		Parent.OutputHandler.Handle(
			new LanguageOutput(
				new Emote("@ reproduce|reproduces speech over the line.", Parent, Parent),
				spokenLanguage,
				null,
				flags: OutputFlags.PurelyAudible
			)
		);
	}

	public IEnumerable<string> SwitchSettings => ["on", "off"];

	public bool CanSwitch(ICharacter actor, string setting)
	{
		return setting.Equals("on", StringComparison.InvariantCultureIgnoreCase) ? !_switchedOn
			: setting.Equals("off", StringComparison.InvariantCultureIgnoreCase) && _switchedOn;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
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
		if (!CanDial(actor, number, out error))
		{
			return false;
		}

		_isOffHook = true;
		Changed = true;
		return TelecommunicationsGrid!.TryStartCall(this, number, out error);
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
		StopRinging();
		_currentCall = call;
		_isOffHook = true;
		_isRinging = false;
		Changed = true;
	}

	public void EndCall(ITelephoneCall? call, bool notifyGrid = true)
	{
		if (call != null && _currentCall != null && !ReferenceEquals(call, _currentCall))
		{
			return;
		}

		var existingCall = _currentCall;
		StopRinging();
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
		if (!_isRinging || !_switchedOn || !IsPowered)
		{
			return;
		}

		Parent.Handle(
			new EmoteOutput(new Emote(_prototype.RingEmote, Parent, Parent), flags: OutputFlags.PurelyAudible),
			OutputRange.Local
		);
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

	private void Parent_OnConnected(IConnectable other, ConnectorType type)
	{
		if (other.Parent.GetItemType<ITelephoneNumberOwner>() is { } owner)
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

		var power = other.Parent.GetItemTypes<IProducePower>()
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
		if (other.Parent == _connectedLineOwner?.Parent)
		{
			if (_currentCall != null || _isOffHook)
			{
				EndCall(_currentCall);
			}

			_connectedLineOwner = null;
			Changed = true;
		}

		if (other.Parent != _connectedPowerSource?.Parent ||
		    _connectedPowerSourceConnector?.Equals(type) != true)
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

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		if (!_connectedItems.Any())
		{
			return false;
		}

		foreach (var connectedItem in _connectedItems.Select(x => x.Item2).ToList())
		{
			connectedItem.RawDisconnect(this, true);
			var newItemConnectable = newItem?.GetItemType<IConnectable>();
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
		var preventingItems = ConnectedItems.Where(
				x => x.Item2.Independent &&
				     (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
		                              .ToList();
		return
			$"{Parent.HowSeen(mover)} is still connected to {preventingItems.Select(x => x.Item2.Parent.HowSeen(mover)).ListToString()}.";
	}

	public override void ForceMove()
	{
		foreach (var item in ConnectedItems.Where(
				x => x.Item2.Independent &&
				     (x.Item2.Parent.InInventoryOf == null || x.Item2.Parent.InInventoryOf != Parent.InInventoryOf))
		                                 .ToList())
		{
			RawDisconnect(item.Item2, true);
		}
	}

	public override bool Take(IGameItem item)
	{
		if (_connectedItems.RemoveAll(x => x.Item2.Parent == item) <= 0)
		{
			return false;
		}

		Changed = true;
		return true;
	}

	public IEnumerable<ConnectorType> Connections => _prototype.Connections;
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _connectedItems;

	public IEnumerable<ConnectorType> FreeConnections
	{
		get
		{
			var remaining = new List<ConnectorType>(Connections);
			foreach (var item in ConnectedItems)
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
		var connection = FreeConnections.FirstOrDefault(x => other.FreeConnections.Any(y => y.CompatibleWith(x)));
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
			foreach (var connection in _connectedItems.Where(x => x.Item2 == other).ToList())
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

	string ICanConnectToGrid.GridType => "Telecommunications";

	IGrid? ICanConnectToGrid.Grid
	{
		get => _directGrid;
		set => SetDirectGrid(value as ITelecommunicationsGrid);
	}
}
