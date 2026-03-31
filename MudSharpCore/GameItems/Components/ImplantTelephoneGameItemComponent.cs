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
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImplantTelephoneGameItemComponent : ImplantBaseGameItemComponent, IImplantReportStatus,
	IImplantRespondToCommands, ITelephone, ITelephoneNumberOwner, ICanConnectToTelecommunicationsGrid
{
	private ImplantTelephoneGameItemComponentProto _prototype;
	private ITelecommunicationsGrid? _grid;
	private string? _phoneNumber;
	private string? _preferredNumber;
	private string? _aliasForCommands;
	private bool _allowSharedNumber;
	private bool _hostedVoicemailEnabled;
	private bool _switchedOn;
	private bool _isOffHook;
	private bool _isRinging;
	private bool _ringHeartbeatSubscribed;
	private ITelephoneCall? _currentCall;

	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantTelephoneGameItemComponentProto)newProto;
	}

	#region Constructors

	public ImplantTelephoneGameItemComponent(ImplantTelephoneGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
		_switchedOn = true;
	}

	public ImplantTelephoneGameItemComponent(Models.GameItemComponent component,
		ImplantTelephoneGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantTelephoneGameItemComponent(ImplantTelephoneGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_switchedOn = rhs._switchedOn;
		_preferredNumber = rhs._preferredNumber;
		_aliasForCommands = rhs._aliasForCommands;
		_allowSharedNumber = rhs._allowSharedNumber;
		_hostedVoicemailEnabled = rhs._hostedVoicemailEnabled;
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		_switchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "true");
		_preferredNumber = root.Element("PreferredNumber")?.Value;
		if (string.IsNullOrWhiteSpace(_preferredNumber))
		{
			_preferredNumber = null;
		}

		_aliasForCommands = root.Element("AliasForCommands")?.Value;
		if (string.IsNullOrWhiteSpace(_aliasForCommands))
		{
			_aliasForCommands = null;
		}

		_allowSharedNumber = bool.Parse(root.Element("AllowSharedNumber")?.Value ?? "false");
		_hostedVoicemailEnabled = bool.Parse(root.Element("HostedVoicemailEnabled")?.Value ?? "false");
		TelecommunicationsGrid =
			Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantTelephoneGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		var definition = SaveToXmlNoTextConversion();
		definition.Add(
			new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
			new XElement("SwitchedOn", _switchedOn),
			new XElement("PreferredNumber", _preferredNumber ?? string.Empty),
			new XElement("AliasForCommands", new XCData(AliasForCommands ?? string.Empty)),
			new XElement("AllowSharedNumber", _allowSharedNumber),
			new XElement("HostedVoicemailEnabled", _hostedVoicemailEnabled)
		);
		return definition.ToString();
	}

	#endregion

	private ICharacter? Actor => InstalledBody?.Actor;

	private bool HasAudioAccess =>
		InstalledBody?.Implants.OfType<IImplantNeuralLink>()
			.Any(x => x.IsLinkedTo(this) && x.DNIConnected && x.PermitsAudio) == true;

	private IEnumerable<IZone> CurrentZones
	{
		get
		{
			var zones = new List<IZone>();
			if (Actor?.Location?.Zone != null)
			{
				zones.Add(Actor.Location.Zone);
			}

			var trueLocations = Parent.TrueLocations ?? Enumerable.Empty<ICell>();
			zones.AddRange(trueLocations
				.Where(x => x?.Zone != null)
				.Select(x => x!.Zone));
			return zones.Distinct().ToList();
		}
	}

	private bool HasCoverageInZone(IZone zone)
	{
		return Gameworld.Items
		                .SelectNotNull(x => x.GetItemType<ICellPhoneTower>())
		                .Any(x => x.TelecommunicationsGrid == TelecommunicationsGrid && x.ProvidesCoverage(zone));
	}

	private void SendInternalMessage(string message)
	{
		if (Actor == null)
		{
			return;
		}

		Actor.OutputHandler.Send(message);
	}

	private void SendInternalOutput(IOutput output)
	{
		Actor?.OutputHandler.Send(output);
	}

	public override void Delete()
	{
		base.Delete();
		TelecommunicationsGrid = null;
		StopRinging();
	}

	public override void Quit()
	{
		StopRinging();
		base.Quit();
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
		sb.AppendLine($"Hosted voicemail is {(HostedVoicemailEnabled ? "enabled".ColourValue() : "disabled".ColourError())} for this line.");
		sb.AppendLine(
			$"It is connected to {(TelecommunicationsGrid == null ? "no telecommunications grid".ColourError() : $"grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
		sb.AppendLine($"It currently {(HasCoverage ? "has signal".ColourValue() : "has no signal".ColourError())}.");
		sb.AppendLine($"It is {(HasAudioAccess ? "linked".ColourValue() : "not linked".ColourError())} to an audio-capable neural interface.");
		sb.AppendLine($"It is {(IsOffHook ? "off the hook".ColourError() : "on the hook".ColourValue())}.");
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

		return sb.ToString();
	}

	#region ITelephone / ITelephoneNumberOwner

	public ITelephoneNumberOwner? NumberOwner => this;
	public string? PhoneNumber => _phoneNumber;

	public string? PreferredNumber
	{
		get => _preferredNumber;
		set
		{
			_preferredNumber = string.IsNullOrWhiteSpace(value) ? null : value;
			Changed = true;
			TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
		}
	}

	public bool AllowSharedNumber
	{
		get => _allowSharedNumber;
		set
		{
			_allowSharedNumber = value;
			Changed = true;
			TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
		}
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

			_grid?.LeaveGrid((ITelephoneNumberOwner)this);
			_grid = value;
			_grid?.JoinGrid((ITelephoneNumberOwner)this);
			Changed = true;
		}
	}

	ITelecommunicationsGrid? ICanConnectToTelecommunicationsGrid.TelecommunicationsGrid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value;
	}

	IGrid ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}

	string ICanConnectToGrid.GridType => "Telecommunications";
	IEnumerable<ITelephone> ITelephoneNumberOwner.ConnectedTelephones => [this];

	public void AssignPhoneNumber(string? number)
	{
		_phoneNumber = number;
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

	public override double PowerConsumptionInWatts => _switchedOn ? base.PowerConsumptionInWatts : 0.0;
	public bool IsPowered => _powered && _switchedOn;
	public bool IsOffHook => _isOffHook;
	public bool HasCoverage => CurrentZones.Any(HasCoverageInZone);
	public bool CanReceiveCalls =>
		IsPowered && TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber) &&
		!_isOffHook && _currentCall == null && HasCoverage;
	public bool IsRinging => _isRinging;
	public bool IsConnected => (_currentCall?.Participants ?? Array.Empty<ITelephone>()).Contains(this) &&
	                           _currentCall?.IsConnected == true;
	public bool IsEngaged => _currentCall != null || _isOffHook;
	public AudioVolume RingVolume => AudioVolume.Silent;
	public ITelephoneCall? CurrentCall => _currentCall;
	public IEnumerable<ITelephone> ConnectedPhones =>
		(_currentCall?.Participants ?? Array.Empty<ITelephone>()).Where(x => x != this).ToList();
	public ITelephone? ConnectedPhone => ConnectedPhones.FirstOrDefault();

	public override void OnPowerCutIn()
	{
		base.OnPowerCutIn();
	}

	public override void OnPowerCutOut()
	{
		base.OnPowerCutOut();
		StopRinging();
		if (_currentCall != null || _isOffHook)
		{
			EndCall(_currentCall);
		}
	}

	public bool ManualTransmit => true;
	public string TransmitPremote => string.Empty;

	public void Transmit(SpokenLanguageInfo spokenLanguage)
	{
		if (!IsConnected || !IsPowered || _currentCall == null)
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
		if (origin == this || !IsPowered || !HasAudioAccess)
		{
			return;
		}

		SendInternalOutput(
			new LanguageOutput(
				new Emote("You receive speech over your implant telephone", Parent, Parent),
				spokenLanguage,
				null,
				flags: OutputFlags.ElectronicOnly
			)
		);
	}

	public IEnumerable<string> SwitchSettings => ["on", "off", "vmon", "vmoff"];

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

		if (setting.Equals("vmon", StringComparison.InvariantCultureIgnoreCase))
		{
			return !HostedVoicemailEnabled;
		}

		return setting.Equals("vmoff", StringComparison.InvariantCultureIgnoreCase) && HostedVoicemailEnabled;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		if (setting.Equals("vmon", StringComparison.InvariantCultureIgnoreCase) ||
		    setting.Equals("vmoff", StringComparison.InvariantCultureIgnoreCase))
		{
			return $"{Parent.HowSeen(actor, true)} already has hosted voicemail {(HostedVoicemailEnabled ? "enabled".ColourValue() : "disabled".ColourError())} for this line.";
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

		if (setting.Equals("vmon", StringComparison.InvariantCultureIgnoreCase) ||
		    setting.Equals("vmoff", StringComparison.InvariantCultureIgnoreCase))
		{
			HostedVoicemailEnabled = setting.Equals("vmon", StringComparison.InvariantCultureIgnoreCase);
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

		if (!IsPowered)
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

		if (!IsPowered || string.IsNullOrWhiteSpace(PhoneNumber))
		{
			error = "That telephone is not ready to make calls right now.";
			return false;
		}

		if (!HasCoverage)
		{
			error = "That implant telephone has no signal.";
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

	public bool HostedVoicemailEnabled
	{
		get => _hostedVoicemailEnabled;
		set
		{
			_hostedVoicemailEnabled = value;
			Changed = true;
		}
	}

	public bool CanSendDigits(ICharacter actor, string digits, out string error)
	{
		if (_currentCall?.IsConnected != true)
		{
			error = "That telephone is not connected to a live call.";
			return false;
		}

		if (!IsPowered || !HasCoverage)
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

		var normalised = new string(digits.Where(x => !char.IsWhiteSpace(x)).ToArray());
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

		if (!IsPowered)
		{
			error = "That telephone cannot answer calls right now.";
			return false;
		}

		if (!HasCoverage)
		{
			error = "That implant telephone has no signal.";
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
		NotifyCallProgress("The call connects.");
	}

	public void NotifyCallProgress(string message)
	{
		SendInternalMessage($"Your implant telephone reports: {message}");
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

		var existingCall = _currentCall;
		StopRinging();
		_currentCall = null;
		_isRinging = false;
		_isOffHook = false;
		Changed = true;
		if (existingCall != null)
		{
			SendInternalMessage("Your implant telephone reports that the line is no longer active.");
		}

		if (notifyGrid && existingCall != null)
		{
			TelecommunicationsGrid?.EndCall(this, existingCall);
		}
	}

	private void Ring()
	{
		if (!_isRinging || !IsPowered || !HasAudioAccess)
		{
			return;
		}

		SendInternalMessage(_prototype.RingText);
	}

	private void RingHeartbeat()
	{
		if (!_isRinging || !IsPowered)
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

	#endregion

	#region Implant Command Interface

	public string ReportStatus()
	{
		var numberText = PhoneNumber?.ColourValue() ?? "unassigned".ColourError();
		var gridText = TelecommunicationsGrid == null
			? "no telecommunications grid".ColourError()
			: $"grid #{TelecommunicationsGrid.Id.ToString("N0")}".ColourValue();
		var callState = IsConnected
			? "connected to a live call".ColourValue()
			: IsRinging
				? "ringing".ColourCommand()
				: _isOffHook
					? "off the hook".ColourCommand()
					: "idle".ColourValue();
		return
			$"* It is an implant telephone and it is {(SwitchedOn ? "on".ColourValue() : "off".ColourError())}.\n* It is {(IsPowered ? "powered".ColourValue() : "unpowered".ColourError())} and currently {(HasCoverage ? "has signal".ColourValue() : "has no signal".ColourError())}.\n* It is connected to {gridText} and its number is {numberText}.\n* It is currently {callState}.\n* It {(HasAudioAccess ? "is".ColourValue() : "is not".ColourError())} linked to an audio-capable neural interface.";
	}

	public string? AliasForCommands
	{
		get => _aliasForCommands;
		set
		{
			_aliasForCommands = string.IsNullOrWhiteSpace(value) ? null : value;
			Changed = true;
		}
	}

	public IEnumerable<string> Commands => ["dial", "answer", "pickup", "hangup", "transmit", "on", "off", "number"];

	public string CommandHelp =>
		"You can use the following options:\n\ton - switches the implant phone on\n\toff - switches the implant phone off\n\tnumber - reports the current assigned number and signal state\n\tdial <number> - dials another number\n\tanswer - answers an incoming call\n\tpickup - takes the line off hook or joins a shared-line call\n\thangup - hangs up the current call or returns the line to the hook\n\ttransmit <message> - speaks over the active call through your neural interface";

	public void IssueCommand(string command, StringStack arguments)
	{
		var whichCommand = Commands.FirstOrDefault(x => x.EqualTo(command)) ??
		                   Commands.FirstOrDefault(x =>
			                   x.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
		switch (whichCommand)
		{
			case "on":
				CommandOn(arguments);
				break;
			case "off":
				CommandOff(arguments);
				break;
			case "number":
				CommandNumber(arguments);
				break;
			case "dial":
				CommandDial(arguments);
				break;
			case "answer":
				CommandAnswer(arguments);
				break;
			case "pickup":
				CommandPickup(arguments);
				break;
			case "hangup":
				CommandHangup(arguments);
				break;
			case "transmit":
				CommandTransmit(arguments);
				break;
		}
	}

	private void CommandOn(StringStack arguments)
	{
		var actor = Actor;
		if (actor == null)
		{
			return;
		}

		if (!_powered)
		{
			SendInternalMessage($"{Parent.HowSeen(actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (SwitchedOn)
		{
			SendInternalMessage($"{Parent.HowSeen(actor, true)} is already switched on.");
			return;
		}

		SwitchedOn = true;
		SendInternalMessage($"You issue the 'on' command to {Parent.HowSeen(actor)} and it switches on.");
	}

	private void CommandOff(StringStack arguments)
	{
		var actor = Actor;
		if (actor == null)
		{
			return;
		}

		if (!_powered)
		{
			SendInternalMessage($"{Parent.HowSeen(actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (!SwitchedOn)
		{
			SendInternalMessage($"{Parent.HowSeen(actor, true)} is already switched off.");
			return;
		}

		SwitchedOn = false;
		if (_currentCall != null || _isOffHook)
		{
			EndCall(_currentCall);
		}

		SendInternalMessage($"You issue the 'off' command to {Parent.HowSeen(actor)} and it switches off.");
	}

	private void CommandNumber(StringStack arguments)
	{
		var actor = Actor;
		if (actor == null)
		{
			return;
		}

		if (arguments.IsFinished)
		{
			SendInternalMessage(
				$"{Parent.HowSeen(actor, true)} is assigned to {(PhoneNumber?.ColourValue() ?? "no number".ColourError())} and currently {(HasCoverage ? "has signal".ColourValue() : "has no signal".ColourError())}.");
			return;
		}

		SendInternalMessage("Telephone numbers are configured through staff tools or FutureProg rather than direct implant commands.");
	}

	private void CommandDial(StringStack arguments)
	{
		if (Actor == null)
		{
			return;
		}

		if (arguments.IsFinished)
		{
			SendInternalMessage("Which number do you want to dial?");
			return;
		}

		var number = arguments.SafeRemainingArgument;
		if (!Dial(Actor, number, out var error))
		{
			SendInternalMessage(error);
			return;
		}

		SendInternalMessage(
			$"You issue a dial command to {Parent.HowSeen(Actor)} for {number.ColourCommand()}.");
	}

	private void CommandAnswer(StringStack arguments)
	{
		if (Actor == null)
		{
			return;
		}

		if (!Answer(Actor, out var error))
		{
			SendInternalMessage(error);
			return;
		}

		SendInternalMessage($"You answer the incoming call with {Parent.HowSeen(Actor)}.");
	}

	private void CommandPickup(StringStack arguments)
	{
		if (Actor == null)
		{
			return;
		}

		if (!PickUp(Actor, out var error))
		{
			SendInternalMessage(error);
			return;
		}

		SendInternalMessage($"You take {Parent.HowSeen(Actor)} off the hook.");
	}

	private void CommandHangup(StringStack arguments)
	{
		if (Actor == null)
		{
			return;
		}

		if (!HangUp(Actor, out var error))
		{
			SendInternalMessage(error);
			return;
		}

		SendInternalMessage($"You hang up {Parent.HowSeen(Actor)}.");
	}

	private void CommandTransmit(StringStack arguments)
	{
		var actor = Actor;
		if (actor == null)
		{
			return;
		}

		if (!IsConnected || _currentCall == null)
		{
			SendInternalMessage($"{Parent.HowSeen(actor, true)} is not currently connected to a live call.");
			return;
		}

		if (!IsPowered)
		{
			SendInternalMessage($"{Parent.HowSeen(actor, true)} is powered down and not responding to commands.");
			return;
		}

		if (actor.CurrentLanguage == null)
		{
			SendInternalMessage("You must first set a speaking language before you can transmit anything.");
			return;
		}

		if (arguments.IsFinished)
		{
			SendInternalMessage("What is it that you want to transmit?");
			return;
		}

		if (arguments.RemainingArgument.Length > 350)
		{
			SendInternalMessage("That is far too much to say at any one time. Keep it under 350 characters.");
			return;
		}

		var langInfo = new SpokenLanguageInfo(
			actor.CurrentLanguage,
			actor.CurrentAccent,
			Form.Audio.AudioVolume.Decent,
			arguments.RemainingArgument,
			Gameworld.GetCheck(RPG.Checks.CheckType.SpokenLanguageSpeakCheck)
			         .Check(actor, RPG.Checks.Difficulty.Easy, actor.CurrentLanguage.LinkedTrait),
			actor,
			null
		);
		SendInternalOutput(
			new LanguageOutput(
				new Emote("You issue a command to $0 through your neural interface and speak over the line.", actor,
					Parent),
				langInfo,
				null,
				flags: OutputFlags.ElectronicOnly
			)
		);
		Transmit(langInfo);
	}

	#endregion
}
