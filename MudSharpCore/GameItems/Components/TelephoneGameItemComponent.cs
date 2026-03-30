#nullable enable
using System;
using System.Collections.Generic;
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

public class TelephoneGameItemComponent : GameItemComponent, ITelephone, ICanConnectToTelecommunicationsGrid
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

	public TelephoneGameItemComponent(TelephoneGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		SwitchedOn = true;
	}

	public TelephoneGameItemComponent(Models.GameItemComponent component, TelephoneGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public TelephoneGameItemComponent(TelephoneGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		SwitchedOn = rhs.SwitchedOn;
		PreferredNumber = rhs.PreferredNumber;
	}

	private void LoadFromXml(XElement root)
	{
		SwitchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "true");
		PreferredNumber = root.Element("PreferredNumber")?.Value;
		TelecommunicationsGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
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
			new XElement("PreferredNumber", PreferredNumber ?? string.Empty)
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
