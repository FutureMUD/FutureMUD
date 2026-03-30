#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction.Grids;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class TelephoneGameItemComponent : GameItemComponent, ITelephone, ICanConnectToTelecommunicationsGrid, IConnectable
{
	protected TelephoneGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TelephoneGameItemComponentProto)newProto;
	}

	private ITelecommunicationsGrid? _grid;
	private bool _powered;
	private bool _ringHeartbeatSubscribed;
	private ITelephone? _incomingCall;
	private ITelephone? _outgoingCall;
	private readonly List<Tuple<ConnectorType, IConnectable>> _connectedItems = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingLoadTimeConnections = [];
	private readonly List<Tuple<long, ConnectorType>> _pendingDependentLoadTimeConnections = [];
	private IProducePower? _connectedPowerSource;
	private ConnectorType? _connectedPowerSourceConnector;

	public TelephoneGameItemComponent(TelephoneGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		parent.OnConnected += Parent_OnConnected;
		parent.OnDisconnected += Parent_OnDisconnected;
		SwitchedOn = true;
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
		SwitchedOn = rhs.SwitchedOn;
		PreferredNumber = rhs.PreferredNumber;
		newParent.OnConnected += Parent_OnConnected;
		newParent.OnDisconnected += Parent_OnDisconnected;
	}

	private void LoadFromXml(XElement root)
	{
		SwitchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "true");
		PreferredNumber = root.Element("PreferredNumber")?.Value;
		TelecommunicationsGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
		var connectors = root.Element("ConnectedItems");
		if (connectors != null)
		{
			foreach (var item in connectors.Elements("Item"))
			{
				var connector = new ConnectorType(item.Attribute("connectiontype")!.Value);
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

	private void ResolveTelecommunicationsGrid()
	{
		TelecommunicationsGrid = ConnectedItems
			.Select(x => x.Item2.Parent.GetItemType<ICanConnectToTelecommunicationsGrid>()?.TelecommunicationsGrid)
			.FirstOrDefault(x => x != null);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TelephoneGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
			new XElement("SwitchedOn", SwitchedOn),
			new XElement("PreferredNumber", PreferredNumber ?? string.Empty),
			new XElement("ConnectedItems",
				from item in ConnectedItems
				select new XElement("Item",
					new XAttribute("id", item.Item2.Parent.Id),
					new XAttribute("connectiontype", item.Item1),
					new XAttribute("independent", item.Item2.Independent)))
		).ToString();
	}

	public override void Login()
	{
		base.Login();
		Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
	}

	public override void Quit()
	{
		base.Quit();
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		StopRinging();
	}

	public override void Delete()
	{
		base.Delete();
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		TelecommunicationsGrid = null;
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
		sb.AppendLine($"It is currently switched {(SwitchedOn ? "on".ColourValue() : "off".ColourError())}.");
		sb.AppendLine($"It is {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}.");
		sb.AppendLine($"Its number is {(PhoneNumber?.ColourValue() ?? "unassigned".ColourError())}.");
		sb.AppendLine(
			$"It is connected to {(TelecommunicationsGrid == null ? "no telecommunications grid".ColourError() : $"grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
		if (IsConnected)
		{
			sb.AppendLine("It is currently in an active call.");
		}
		else if (IsRinging)
		{
			sb.AppendLine("It is currently ringing.");
		}
		else if (_outgoingCall != null)
		{
			sb.AppendLine("It is currently dialling another number.");
		}

		return sb.ToString();
	}

	public string? PhoneNumber { get; private set; }

	private string? _preferredNumber;
	public string? PreferredNumber
	{
		get => _preferredNumber;
		set
		{
			_preferredNumber = string.IsNullOrWhiteSpace(value) ? null : value;
			Changed = true;
			if (TelecommunicationsGrid != null)
			{
				TelecommunicationsGrid.RequestNumber(this, _preferredNumber);
			}
		}
	}

	public bool IsPowered => _powered;
	public bool CanReceiveCalls => SwitchedOn && IsPowered && TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber);
	public bool IsRinging => _incomingCall != null && !IsConnected;
	public bool IsConnected => ConnectedPhone != null;
	public bool IsEngaged => IsConnected || IsRinging || _outgoingCall != null;
	public ITelephone? ConnectedPhone { get; private set; }

	public ITelecommunicationsGrid? TelecommunicationsGrid
	{
		get => _grid;
		set
		{
			if (_grid == value)
			{
				return;
			}

			_grid?.LeaveGrid(this);
			_grid = value;
			_grid?.JoinGrid(this);
			Changed = true;
		}
	}

	public void AssignPhoneNumber(string? number)
	{
		PhoneNumber = number;
		Changed = true;
	}

	private bool _switchedOn;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			Changed = true;
		}
	}

	public double PowerConsumptionInWatts => SwitchedOn ? _prototype.Wattage : 0.0;

	public void OnPowerCutIn()
	{
		_powered = true;
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		if (IsEngaged)
		{
			EndCall(ConnectedPhone ?? _incomingCall ?? _outgoingCall);
		}
	}

	public bool ManualTransmit => true;
	public string TransmitPremote => _prototype.TransmitPremote;

	public void Transmit(SpokenLanguageInfo spokenLanguage)
	{
		if (!IsConnected || ConnectedPhone == null || !SwitchedOn || !IsPowered)
		{
			return;
		}

		ConnectedPhone.ReceiveTransmission(0.0, spokenLanguage, 0L, this);
	}

	public void ReceiveTransmission(double frequency, string dataTransmission, long encryption, ITransmit origin)
	{
	}

	public void ReceiveTransmission(double frequency, SpokenLanguageInfo spokenLanguage, long encryption, ITransmit origin)
	{
		if (!SwitchedOn || !IsPowered)
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
		return setting.Equals("on", StringComparison.InvariantCultureIgnoreCase) ? !SwitchedOn
			: setting.Equals("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn;
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

		SwitchedOn = setting.Equals("on", StringComparison.InvariantCultureIgnoreCase);
		Changed = true;
		if (!SwitchedOn && IsEngaged)
		{
			EndCall(ConnectedPhone ?? _incomingCall ?? _outgoingCall);
		}

		return true;
	}

	public bool CanDial(ICharacter actor, string number, out string error)
	{
		if (TelecommunicationsGrid == null)
		{
			error = "That telephone is not connected to a telecommunications grid.";
			return false;
		}

		if (!CanReceiveCalls)
		{
			error = "That telephone is not ready to make calls right now.";
			return false;
		}

		if (IsEngaged)
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

		if (!TelecommunicationsGrid!.TryStartCall(this, number, out error))
		{
			_outgoingCall = null;
			return false;
		}

		return true;
	}

	public bool CanAnswer(ICharacter actor, out string error)
	{
		if (!IsRinging || _incomingCall == null)
		{
			error = "That telephone is not ringing.";
			return false;
		}

		if (!CanReceiveCalls)
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

		var caller = _incomingCall!;
		ConnectCall(caller);
		caller.ConnectCall(this);
		return true;
	}

	public bool CanHangUp(ICharacter actor, out string error)
	{
		if (!IsEngaged)
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

		EndCall(ConnectedPhone ?? _incomingCall ?? _outgoingCall);
		return true;
	}

	public void BeginOutgoingCall(ITelephone otherPhone, string number)
	{
		_outgoingCall = otherPhone;
		ConnectedPhone = null;
		Changed = true;
	}

	public void ReceiveIncomingCall(ITelephone caller)
	{
		_incomingCall = caller;
		_outgoingCall = null;
		ConnectedPhone = null;
		Changed = true;
		Ring();
		StartRinging();
	}

	public void ConnectCall(ITelephone otherPhone)
	{
		StopRinging();
		_incomingCall = null;
		_outgoingCall = null;
		ConnectedPhone = otherPhone;
		Changed = true;
	}

	public void EndCall(ITelephone? otherPhone, bool notifyOtherPhone = true)
	{
		StopRinging();
		_incomingCall = null;
		_outgoingCall = null;
		ConnectedPhone = null;
		Changed = true;
		if (notifyOtherPhone && otherPhone != null)
		{
			otherPhone.EndCall(this, false);
		}
	}

	private void Ring()
	{
		if (!CanReceiveCalls)
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
		if (!IsRinging || !CanReceiveCalls)
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

	string ICanConnectToGrid.GridType => "Telecommunications";

	IGrid? ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}
}
